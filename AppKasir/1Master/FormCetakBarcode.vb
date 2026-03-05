
Imports System.Drawing.Printing
Imports System.Drawing.Text
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports ZXing
Imports ZXing.Common

' ================================================================================
' FORM CETAK BARCODE - TSC TTP-224 ENHANCED REFACTORED VERSION v2.0
' ================================================================================

Public Class FormCetakBarcode

    ' ===== TSPL RAW PRINTING API =====
    ' TAMBAHKAN Pack:=1 di sini agar struktur 'padat' dan terbaca benar oleh Windows API
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=1)>
    Private Structure DOCINFOA
        <MarshalAs(UnmanagedType.LPStr)> Public pDocName As String
        <MarshalAs(UnmanagedType.LPStr)> Public pOutputFile As String
        <MarshalAs(UnmanagedType.LPStr)> Public pDataType As String
    End Structure

    <DllImport("winspool.Drv", EntryPoint:="OpenPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function OpenPrinter(<MarshalAs(UnmanagedType.LPStr)> szPrinter As String, ByRef hPrinter As IntPtr, pd As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="ClosePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function ClosePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartDocPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function StartDocPrinter(hPrinter As IntPtr, level As Int32, <[In]()> ByRef di As DOCINFOA) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndDocPrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function EndDocPrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function StartPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function EndPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="WritePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function WritePrinter(hPrinter As IntPtr, pBytes As IntPtr, dwCount As Int32, ByRef dwWritten As Int32) As Boolean
    End Function

    ' ===== CONFIGURATION =====
    Private ReadOnly configFilePath As String = Path.Combine(Application.StartupPath, "ConfigBarcodeBarang.ini")


    ' ===== TSC TTP-224 SPECIAL SETTINGS (UPDATED) =====

    ' Update berdasarkan spesifikasi fisik printer: 112mm
    Private ReadOnly TSC_TTP224_MAX_WIDTH_MM As Single = 112.0F

    ' Kita buat minimum lebih kecil agar fleksibel
    Private ReadOnly TSC_TTP224_MIN_WIDTH_MM As Single = 15.0F
    Private ReadOnly TSC_DOTS_PER_MM As Single = 8.0F

    ' ===== PRINTER CONTEXTS (DEFAULT VALUES) =====
    Private ReadOnly DPIResolusiPrinter As Single = 203.0F
    Private ReadOnly LebarKertasMM As Single = 33.0F
    Private ReadOnly TinggiKertasMM As Single = 15.0F

    ' ===== TEXT TRUNCATION & FORMATTING CONSTANTS =====
    Private ReadOnly TRUNCATE_NAMA_MAX_LENGTH As Integer = 20
    Private ReadOnly TRUNCATE_TOKO_MAX_LENGTH As Integer = 15
    Private ReadOnly TRUNCATE_SUFFIX As String = "..."

    ' ===== TSPL CONSTANTS =====
    Private ReadOnly TSPL_FONT_LARGE As String = "2"
    Private ReadOnly TSPL_FONT_MEDIUM As String = "1"
    Private ReadOnly TSPL_FONT_SMALL As String = "0"

    ' ===== CALIBRATION CONSTANTS =====
    Private ReadOnly DOTS_PER_LABEL_BACKWARD_FEED As Integer = 120

    ' ===== VALIDATION RANGES =====
    Private ReadOnly SPEED_MIN As Integer = 1
    Private ReadOnly SPEED_MAX As Integer = 10
    Private ReadOnly DENSITY_MIN As Integer = 0
    Private ReadOnly DENSITY_MAX As Integer = 15
    Private ReadOnly DARKNESS_MIN As Integer = 0
    Private ReadOnly DARKNESS_MAX As Integer = 15
    Private ReadOnly LABEL_HEIGHT_TSC_THRESHOLD_MM As Single = 20.0F
    Private ReadOnly LABEL_WIDTH_TSC_COMPACT_MM As Single = 40.0F
    Private ReadOnly MAX_PRINT_QUANTITY As Integer = 1000
    Private ReadOnly MAX_BACKWARD_FEED As Integer = 100
    Private ReadOnly PERCENTAGE_TOLERANCE As Single = 0.01F
    Private ReadOnly MARGIN_MIN_MM As Single = 0.0F
    Private ReadOnly MARGIN_MAX_MM As Single = 10.0F

    ' ===== DYNAMIC SETTINGS VARIABLES (SEMUA WAJIB DINAMIS) =====
    Private MarginKiriDinamisMM As Single = 3.0F
    Private MarginAtasDinamisMM As Single = 2.0F
    Private PersentaseLebarAreaBarcodeDinamis As Single = 0.55F
    Private OffsetVertikalBarcodeDinamisPixels As Integer = 5
    Private KecepatanCetakDinamis As Integer = 4
    Private DensitasCetakDinamis As Integer = 10
    Private KegelapanCetakDinamis As Integer = 12
    Private JarakHorizontalDinamisMM As Single = 2.0F
    Private JarakVertikalDinamisMM As Single = 2.0F
    Private PosisiAwalXDinamisMM As Single = 0.0F
    Private PosisiAwalYDinamisMM As Single = 0.0F

    ' ===== GANTI DENGAN KONSTANTA OPTIMAL =====
    Private ReadOnly PERSENTASE_TINGGI_NAMA_DEFAULT As Single = 0.22F
    Private ReadOnly PERSENTASE_TINGGI_BARCODE_DEFAULT As Single = 0.55F
    Private ReadOnly PERSENTASE_TINGGI_TOKO_DEFAULT As Single = 0.23F

    ' ===== DATA CACHE =====
    Private DataBarang As New Dictionary(Of String, Object)

    ' ===== PROPERTIES UNTUK DATA DARI FORM LAIN =====
    Public Property NamaBarangDikirim As String = ""
    Public Property KodeBarangDikirim As String = ""

    ' Variabel flag untuk mencegah update berulang saat loading
    Private isLoadingPreview As Boolean = False

    ' ===== PROPERTY UNTUK PREVIEW BOXES =====
    Private ReadOnly Property AllPreviewBoxes As PictureBox()
        Get
            Return New PictureBox() {PicPreviewLabel0, PicPreviewLabel1, PicPreviewLabel2, PicPreviewLabel3,
                                    PicPreviewLabel4, PicPreviewLabel5, PicPreviewLabel6, PicPreviewLabel7,
                                    PicPreviewLabel8, PicPreviewLabel9, PicPreviewLabel10, PicPreviewLabel11}
        End Get
    End Property

    ' ================================================================================
    ' FORM INITIALIZATION
    ' ================================================================================

    Private Sub FormCetakBarcode_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            isLoadingPreview = True
            LogMessage("Form Cetak Barcode dimuat (TSC Enhanced Mode v2.0)")

            SetupAndClearPreviews()
            InitializeFontControls()
            InitializeOtherCombos()

            BuatFileDefault()
            LoadPengaturan()

            If Not String.IsNullOrWhiteSpace(NamaBarangDikirim) Then
                LogMessage($"Menerima data: NamaBarang='{NamaBarangDikirim}', KodeBarang='{KodeBarangDikirim}'")
                TxtInputNamaBarang.Text = NamaBarangDikirim
                AmbildataLengkapBarang(NamaBarangDikirim)
                TxtJumlahLabelDicetak.Focus()
            Else
                TxtKodeBarcodeInput.Focus()
            End If

            UpdatePrinterStatusLabel()
            isLoadingPreview = False
            UpdatePreviewPictureBoxes()

            LogMessage("Form berhasil diinisialisasi")

        Catch ex As Exception
            LogMessage($"Error inisialisasi form: {ex.Message}", True)
            MessageBox.Show("Error inisialisasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ================================================================================
    ' INITIALIZATION FUNCTIONS
    ' ================================================================================


    ' Fungsi untuk mengambil Dots Per MM dari Driver Windows (bukan Hardcode 8.0)
    Private Function GetDynamicDotsPerMM() As Single
        If IsTSCPrinterSelected() Then
            ' Untuk printer TSC yang diketahui (misal TTP-224) dengan DPI 203, gunakan nilai yang pasti.
            LogMessage("Menggunakan DPMM 8.0 (203 DPI) untuk printer TSC yang terdeteksi.")
            Return TSC_DOTS_PER_MM ' Yaitu 8.0F
        Else
            ' Untuk printer non-TSC, coba baca dari driver
            Try
                If Not String.IsNullOrWhiteSpace(CmbJenisPrinter.Text) Then
                    Dim ps As New PrinterSettings()
                    ps.PrinterName = CmbJenisPrinter.Text
                    If ps.IsValid Then
                        Dim xDpi As Integer = ps.DefaultPageSettings.PrinterResolution.X
                        If xDpi > 0 Then
                            LogMessage($"Menggunakan DPMM {CSng(xDpi / 25.4F)} dari driver untuk printer non-TSC.")
                            Return CSng(xDpi / 25.4F)
                        End If
                    End If
                End If
            Catch ex As Exception
                LogMessage($"Gagal baca DPI driver: {ex.Message}", True)
            End Try
            ' Fallback umum jika gagal membaca driver atau untuk printer non-TSC dengan DPI tidak diketahui
            LogMessage("Menggunakan DPMM default 8.0 (203 DPI) sebagai fallback.")
            Return TSC_DOTS_PER_MM
        End If
    End Function

    ' ================================================================================
    ' ADAPTIVE PERCENTAGE FUNCTIONS
    ' ================================================================================

    Private Function GetAdaptiveNamaPercentage(labelHeightMM As Single) As Single
        ' Untuk label sangat kecil (< 20mm), kurangi persentase nama
        If labelHeightMM < 20.0F Then Return 0.18F
        If labelHeightMM < 25.0F Then Return 0.2F
        Return PERSENTASE_TINGGI_NAMA_DEFAULT ' 0.22F
    End Function

    Private Function GetAdaptiveBarcodePercentage(labelHeightMM As Single) As Single
        ' Untuk label kecil, perbesar area barcode untuk scanning lebih baik
        If labelHeightMM < 20.0F Then Return 0.62F
        If labelHeightMM < 25.0F Then Return 0.58F
        Return PERSENTASE_TINGGI_BARCODE_DEFAULT ' 0.55F
    End Function

    Private Function GetAdaptiveTokoPercentage(labelHeightMM As Single) As Single
        ' Otomatis hitung untuk total 100%
        Return 1.0F - GetAdaptiveNamaPercentage(labelHeightMM) - GetAdaptiveBarcodePercentage(labelHeightMM)
    End Function

    Private Sub InitializeFontControls()
        Try
            ' Font sizes
            Dim fontSizes As Integer() = {6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30}
            Dim sizeCombos = {CmbUkuranFontNama, CmbUkuranFontHarga, CmbUkuranFontSatuan, CmbUkuranFontToko}

            For Each cmb In sizeCombos
                cmb.Items.Clear()
                cmb.Items.AddRange(fontSizes.Select(Function(x) CObj(x)).ToArray())
                ' Default ke index 1 (size 7)
                If cmb.Items.Count > 1 Then cmb.SelectedIndex = 1
            Next

            ' Font families
            Dim fonts = New InstalledFontCollection().Families.Select(Function(f) f.Name).ToArray()
            Dim fontCombos = {CmbFontNamaBarang, CmbFontHargaBarang, CmbFontSatuanBarang, CmbFontNamaToko}

            For Each cmb In fontCombos
                cmb.Items.Clear()
                cmb.Items.AddRange(fonts)
                If cmb.Items.Contains("Arial") Then
                    cmb.SelectedItem = "Arial"
                ElseIf cmb.Items.Count > 0 Then
                    cmb.SelectedIndex = 0
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Error loading fonts: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub InitializeOtherCombos()
        ' Printer
        CmbJenisPrinter.Items.Clear()
        For Each printer As String In PrinterSettings.InstalledPrinters
            CmbJenisPrinter.Items.Add(printer)
        Next

        Dim defaultPrinter As New PrinterSettings()
        If CmbJenisPrinter.Items.Contains(defaultPrinter.PrinterName) Then
            CmbJenisPrinter.SelectedItem = defaultPrinter.PrinterName
        ElseIf CmbJenisPrinter.Items.Count > 0 Then
            CmbJenisPrinter.SelectedIndex = 0
        End If

        ' Barcode Format
        CmbJenisFormatBarcode.Items.Clear()
        CmbJenisFormatBarcode.Items.AddRange(New String() {"CODE 128", "CODE 39", "EAN-13", "UPC-A", "QR CODE"})
        CmbJenisFormatBarcode.SelectedIndex = 0

        ' Kolom per baris
        CmbJumlahKolomPerBaris.Items.Clear()
        CmbJumlahKolomPerBaris.Items.AddRange(New String() {"1", "2", "3", "4"})
        CmbJumlahKolomPerBaris.SelectedIndex = 2 ' Default 3 kolom
    End Sub

    Private Sub SetupAndClearPreviews()
        For Each pb In AllPreviewBoxes
            pb.SizeMode = PictureBoxSizeMode.Zoom
            pb.BackColor = Color.White
            pb.BorderStyle = BorderStyle.FixedSingle
            pb.Visible = False
            pb.Margin = New Padding(2)
            pb.Image = Nothing
        Next
    End Sub

    ' ================================================================================
    ' TSC DETECTION AND VALIDATION
    ' ================================================================================

    Private Function IsTSCPrinterSelected() As Boolean
        Try
            If String.IsNullOrWhiteSpace(CmbJenisPrinter.Text) Then Return False
            Dim printerName As String = CmbJenisPrinter.Text.ToUpper()
            Return printerName.Contains("TSC") OrElse printerName.Contains("TTP-224") OrElse printerName.Contains("TTP224")
        Catch ex As Exception
            LogMessage($"Error detecting printer: {ex.Message}")
            Return False
        End Try
    End Function

    Private Sub UpdatePrinterStatusLabel()
        If IsTSCPrinterSelected() Then
            LabelStatusInfo.Text = "✅ Printer TSC TTP-224 Terdeteksi | Mode: Semicoated Optimized"
            LabelStatusInfo.ForeColor = Color.Green
            LogMessage("TSC TTP-224 detected")
        Else
            LabelStatusInfo.Text = $"ℹ️ Label {LebarKertasMM:F0}×{TinggiKertasMM:F0}mm | DPI: {DPIResolusiPrinter:F0} | TSPL RAW Mode"
            LabelStatusInfo.ForeColor = Color.DarkBlue
        End If
    End Sub

    Private Function ValidateTSCPaperSize() As Boolean
        Try
            If Not IsTSCPrinterSelected() Then Return True

            Dim lebar As Single = GetDynamicLabelWidth()

            If lebar > TSC_TTP224_MAX_WIDTH_MM Then
                If MessageBox.Show($"⚠️ PRINTER TSC TTP-224 WARNING" & vbCrLf &
                                 $"Lebar label ({lebar}mm) melebihi batas maksimal ({TSC_TTP224_MAX_WIDTH_MM}mm)." & vbCrLf &
                                 "Lebar akan disesuaikan otomatis. Lanjutkan?",
                                 "TSC Size Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                    Return False
                End If
            End If

            If lebar < TSC_TTP224_MIN_WIDTH_MM Then
                MessageBox.Show($"⚠️ PRINTER TSC TTP-224 WARNING" & vbCrLf &
                              $"Lebar label ({lebar}mm) di bawah minimum ({TSC_TTP224_MIN_WIDTH_MM}mm)." & vbCrLf &
                              "Kualitas cetak mungkin tidak optimal.",
                              "TSC Size Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Return True

        Catch ex As Exception
            LogMessage($"Error validating TSC paper size: {ex.Message}")
            Return True
        End Try
    End Function

    ' ================================================================================
    ' RAW TSPL PRINTING FUNCTIONS
    ' ================================================================================
    Private Function SendBytesToPrinter(printerName As String, bytes() As Byte) As Boolean
        Dim hPrinter As IntPtr = IntPtr.Zero
        Dim di As New DOCINFOA()
        Dim dwWritten As Integer = 0
        Dim bSuccess As Boolean = False

        Try
            ' Inisialisasi struktur DOCINFOA
            di.pDocName = "TSC Label Job"
            di.pOutputFile = ""  ' <--- PENTING: Set ke string kosong, bukan Nothing
            di.pDataType = "RAW" ' <--- Ini yang harus terbaca benar oleh Windows

            If OpenPrinter(printerName, hPrinter, IntPtr.Zero) Then
                If StartDocPrinter(hPrinter, 1, di) Then
                    If StartPagePrinter(hPrinter) Then
                        Dim pUnmanagedBytes As IntPtr = Marshal.AllocCoTaskMem(bytes.Length)
                        Try
                            Marshal.Copy(bytes, 0, pUnmanagedBytes, bytes.Length)
                            bSuccess = WritePrinter(hPrinter, pUnmanagedBytes, bytes.Length, dwWritten)
                        Finally
                            Marshal.FreeCoTaskMem(pUnmanagedBytes)
                        End Try
                        EndPagePrinter(hPrinter)
                    End If
                    EndDocPrinter(hPrinter)
                End If
                ClosePrinter(hPrinter)
            End If
            Return bSuccess
        Catch ex As Exception
            LogMessage($"Error sending to printer: {ex.Message}", True)
            Return False
        End Try
    End Function

    ' ================================================================================
    ' TSPL GENERATION FUNCTIONS
    ' ================================================================================
    Private Function GenerateTSPLCommands(data As DataLabelBarcode, jumlahCetak As Integer, jumlahKolom As Integer) As String
        Dim sb As New StringBuilder()

        Try
            Dim singleLabelWidthMM As Single = GetDynamicLabelWidth() ' Misal 33mm
            Dim labelHeightMM As Single = GetDynamicLabelHeight()   ' Misal 15mm
            Dim dotsPerMM As Single = GetDynamicDotsPerMM()

            ' --- PERBAIKAN KRITIS PERTAMA: HITUNG TOTAL LEBAR BARIS ---
            ' Hitung lebar total baris berdasarkan jumlah kolom
            ' Rumus: (Lebar Stiker + Gap Horizontal) x Jumlah Kolom
            Dim gapHorizontal As Single = Math.Max(2.0F, JarakHorizontalDinamisMM)
            Dim totalRowWidthMM As Single = (singleLabelWidthMM + gapHorizontal) * jumlahKolom

            Dim labelWidthDots As Integer = CInt(Math.Round(singleLabelWidthMM * dotsPerMM))
            Dim labelHeightDots As Integer = CInt(Math.Round(labelHeightMM * dotsPerMM))
            Dim totalRowWidthDots As Integer = CInt(Math.Round(totalRowWidthMM * dotsPerMM))

            Dim gapVertikal As Single = Math.Max(2.0F, JarakVertikalDinamisMM)

            ' HEADER TSPL
            sb.AppendLine($"! {KecepatanCetakDinamis} {DensitasCetakDinamis} {DensitasCetakDinamis} {labelHeightDots} 1")

            ' --- GUNAKAN TOTAL LEBAR BARIS DI SINI ---
            ' Jika 3 kolom @ 33mm + 2mm gap, SIZE akan menjadi sekitar 105mm
            sb.AppendLine($"SIZE {totalRowWidthMM:F1} mm, {labelHeightMM:F1} mm")

            sb.AppendLine($"GAP {gapHorizontal:F1} mm, {gapVertikal:F1} mm")
            sb.AppendLine("DIRECTION 1,0")
            sb.AppendLine("REFERENCE 0,0")

            sb.AppendLine("SET PRINTKEY OFF")
            sb.AppendLine("SET TEAR ON")
            sb.AppendLine("SET PEEL OFF")
            sb.AppendLine("SET CUTTER OFF")
            sb.AppendLine($"SPEED {KecepatanCetakDinamis}")
            sb.AppendLine($"DENSITY {DensitasCetakDinamis}")

            If IsTSCPrinterSelected() Then
                sb.AppendLine($"SET DARKNESS {KegelapanCetakDinamis}")
            End If

            sb.AppendLine("HOME")
            sb.AppendLine("CLS")

            ' LOOP GAMBAR KOLOM
            ' Karena SIZE sekarang sudah 105mm, posisi REFERENCE 280 dan 560 akan valid
            For col As Integer = 0 To jumlahKolom - 1
                Dim xOffsetMM As Single = col * (singleLabelWidthMM + gapHorizontal)
                Dim xOffsetDots As Integer = CInt(xOffsetMM * dotsPerMM)

                sb.AppendLine($"REFERENCE {xOffsetDots},0")

                ' Panggil fungsi gambar label
                GenerateSingleLabelTSPLRelative(sb, data, dotsPerMM)
            Next

            ' Hitung jumlah set (baris) yang dicetak
            Dim copies As Integer = Math.Ceiling(jumlahCetak / CSng(jumlahKolom))
            sb.AppendLine($"PRINT 1,{copies}")

            LogMessage($"TSPL Generated: RowWidth={totalRowWidthMM:F1}mm (3x{singleLabelWidthMM}mm), LabelHeight={labelHeightMM:F1}mm, Cols={jumlahKolom}, Sets={copies}")
            LogMessage("=== FULL TSPL COMMANDS ===")
            LogMessage(sb.ToString())
            LogMessage("=== END TSPL ===")

            Return sb.ToString()

        Catch ex As Exception
            LogMessage($"Error generating TSPL: {ex.Message}", True)
            Return ""
        End Try
    End Function

    ' ========================================================================
    ' HELPER: Mapping Ukuran Font ke ID Font TSPL (0=Kecil, 1=Sedang, 2=Besar)
    ' ========================================================================
    Private Function GetTSPLFontID(font As Font) As String
        ' Logika sederhana memetakan ukuran font Windows ke Font ID Printer TSPL
        If font.Size < 8 Then
            Return TSPL_FONT_SMALL ' "0"
        ElseIf font.Size >= 8 AndAlso font.Size < 12 Then
            Return TSPL_FONT_MEDIUM ' "1"
        Else
            Return TSPL_FONT_LARGE ' "2"
        End If
    End Function

    ' ========================================================================
    ' 3. GenerateSingleLabelTSPLRelative (FULLY DYNAMIC VERSION)
    ' ========================================================================
    Private Sub GenerateSingleLabelTSPLRelative(sb As StringBuilder, data As DataLabelBarcode, dotsPerMM As Single)
        Try
            Dim labelWidthMM As Single = GetDynamicLabelWidth()
            Dim labelHeightMM As Single = GetDynamicLabelHeight()
            Dim labelWidthDots As Integer = CInt(labelWidthMM * dotsPerMM)
            Dim labelHeightDots As Integer = CInt(labelHeightMM * dotsPerMM)

            ' Gunakan Variabel Dinamis Margin dari Form
            Dim marginXDots As Integer = CInt(MarginKiriDinamisMM * dotsPerMM)
            Dim marginYDots As Integer = CInt(MarginAtasDinamisMM * dotsPerMM)

            ' ================================================================================
            ' 1. PERHITUNGAN TINGGI DINAMIS (BARCODE BESAR)
            ' ================================================================================

            ' Alokasikan ruang vertikal statis untuk teks (Nama & Toko) agar aman
            Dim reservedForNameMM As Single = 4.5F
            Dim reservedForTokoMM As Single = 3.0F

            ' Hitung ruang tersisa untuk barcode (Tinggi Label - Margin Atas - Nama - Toko)
            Dim availableVerticalSpaceMM As Single = labelHeightMM - (MarginAtasDinamisMM + reservedForNameMM + reservedForTokoMM + MarginAtasDinamisMM)

            ' Tentukan Tinggi Barcode: Minimal 8mm, atau 65% dari ruang tersedia
            Dim barcodeHeightMM As Single = Math.Max(8.0F, availableVerticalSpaceMM * 0.65F)
            Dim barcodeHeightDots As Integer = CInt(barcodeHeightMM * dotsPerMM)

            ' Hitung Posisi Y (Vertikal)
            Dim namaY As Integer = marginYDots + 4
            Dim barcodeY As Integer = marginYDots + CInt(reservedForNameMM * dotsPerMM)
            Dim nomorBarcodeY As Integer = barcodeY + barcodeHeightDots + CInt(1.5 * dotsPerMM)

            ' Posisi Toko (Bottom Absolute)
            Dim tokoHeightDots As Integer = 20
            Dim tokoY As Integer = labelHeightDots - marginYDots - tokoHeightDots - 2

            ' ================================================================================
            ' 2. NAMA BARANG (FONT DINAMIS)
            ' ================================================================================
            Dim namaText As String = TruncateText(data.NamaBarang, 30)
            Dim namaX As Integer = labelWidthDots \ 2

            ' Gunakan Mapping Font Dinamis berdasarkan Ukuran Font Nama di Form
            Dim fontNamaID As String = GetTSPLFontID(data.FontNama)
            sb.AppendLine($"TEXT {namaX},{namaY},""{fontNamaID}"",0,1,1,""{namaText}""")

            ' ================================================================================
            ' 3. BARCODE (LEBAR & TINGGI DINAMIS)
            ' ================================================================================

            ' Hitung Lebar Area Barcode Berdasarkan PERSSENTASE DINAMIS dari Form
            ' Lebar Total - (Margin Kiri*2) dikali Persentase Area Barcode
            Dim availableContentWidthDots As Integer = labelWidthDots - (marginXDots * 2)
            Dim barcodeAreaWidthDots As Integer = CInt(availableContentWidthDots * PersentaseLebarAreaBarcodeDinamis)

            Dim barcodeX As Integer = marginXDots
            Dim barcodeType As String = GetTSPLBarcodeType(data.BarcodeFormat)

            If barcodeType = "QRCODE" Then
                Dim qrSize As Integer = Math.Min(barcodeAreaWidthDots, barcodeHeightDots)
                sb.AppendLine($"QRCODE {barcodeX},{barcodeY},H,3,A,0,""{data.NomorBarcode}""")
            Else
                Dim narrowBar As Integer = 2
                Dim wideBar As Integer = CalculateOptimalWideBar(DensitasCetakDinamis, barcodeAreaWidthDots, data.NomorBarcode.Length)

                ' Gunakan Tinggi Barcode Dinamis
                sb.AppendLine($"BARCODE {barcodeX},{barcodeY},""{barcodeType}"",{barcodeHeightDots},1,0,{narrowBar},{wideBar},""{data.NomorBarcode}""")
            End If

            ' ================================================================================
            ' 4. NOMOR BARCODE (FONT DINAMIS)
            ' ================================================================================
            Dim nomorBarcodeX As Integer = barcodeX + (barcodeAreaWidthDots \ 2)

            ' Gunakan Mapping Font Dinamis untuk Nomor Barcode (menggunakan Font Toko kecil sebagai referensi atau font kecil default)
            ' Di sini kita pakai TSPL_FONT_SMALL agar muat, tapi bisa dimodifikasi pakai GetTSPLFontID jika mau dinamis
            sb.AppendLine($"TEXT {nomorBarcodeX},{nomorBarcodeY},""{TSPL_FONT_SMALL}"",0,1,1,""{data.NomorBarcode}""")

            ' ================================================================================
            ' 5. HARGA (FONT DINAMIS & POSISI DINAMIS)
            ' ================================================================================
            Dim hargaValue As Decimal
            If Decimal.TryParse(CleanCurrencyInput(data.HargaBarang), hargaValue) Then
                Dim hargaFormatted As String = FormatHargaForLabel(hargaValue, labelWidthMM)
                Dim satuanText As String = "/" & data.SatuanBarang
                Dim combinedText As String = hargaFormatted & satuanText

                ' Gunakan Mapping Font Dinamis berdasarkan Ukuran Font Harga di Form
                Dim fontHargaID As String = GetTSPLFontID(data.FontHarga)

                ' Posisi X dinamis: Total Lebar - Margin Kiri - Padding kecil
                Dim hargaX As Integer = labelWidthDots - marginXDots - CInt(2 * dotsPerMM)

                ' Posisi Y: Tengah Barcode agar sejajar
                Dim hargaY As Integer = barcodeY + (barcodeHeightDots \ 2)

                sb.AppendLine($"TEXT {hargaX},{hargaY},""{fontHargaID}"",270,1,1,""{combinedText}""")
            End If

            ' ================================================================================
            ' 6. NAMA TOKO (FONT DINAMIS)
            ' ================================================================================
            Dim tokoX As Integer = labelWidthDots \ 2
            Dim tokoText As String = TruncateText(data.NamaToko, 30)

            ' Gunakan Mapping Font Dinamis berdasarkan Ukuran Font Toko di Form
            Dim fontTokoID As String = GetTSPLFontID(data.FontToko)
            sb.AppendLine($"TEXT {tokoX},{tokoY},""{fontTokoID}"",0,1,1,""{tokoText}""")

        Catch ex As Exception
            LogMessage($"Error generating single label TSPL: {ex.Message}", True)
        End Try
    End Sub

    Private Function CalculateOptimalWideBar(density As Integer, barcodeWidth As Integer, barcodeLength As Integer) As Integer
        Dim wideBar As Integer

        Select Case density
            Case 0 To 5
                wideBar = 2
            Case 6 To 10
                wideBar = 3
            Case Else
                wideBar = 4
        End Select

        Dim maxWideBar As Integer = barcodeWidth \ (barcodeLength * 2)
        If wideBar > maxWideBar AndAlso maxWideBar > 1 Then
            wideBar = maxWideBar
        End If

        Return wideBar
    End Function

    Private Function GetTSPLBarcodeType(format As BarcodeFormat) As String
        Select Case format
            Case BarcodeFormat.CODE_128 : Return "128"
            Case BarcodeFormat.CODE_39 : Return "39"
            Case BarcodeFormat.EAN_13 : Return "EAN13"
            Case BarcodeFormat.UPC_A : Return "UPCA"
            Case BarcodeFormat.QR_CODE : Return "QRCODE"
            Case Else : Return "128"
        End Select
    End Function

    ' ================================================================================
    ' DYNAMIC GETTERS (SELALU BACA DARI TEXTBOX)
    ' ================================================================================

    Private Function GetDynamicLabelWidth() As Single
        Dim value As Single = LebarKertasMM
        If Not String.IsNullOrWhiteSpace(TxtLebarLabelMM.Text) Then
            Single.TryParse(TxtLebarLabelMM.Text, value)
        End If
        Return value
    End Function

    Private Function GetDynamicLabelHeight() As Single
        Dim value As Single = TinggiKertasMM
        If Not String.IsNullOrWhiteSpace(TxtTinggiLabelMM.Text) Then
            Single.TryParse(TxtTinggiLabelMM.Text, value)
        End If
        Return value
    End Function

    ' ================================================================================
    ' PREVIEW FUNCTIONS
    ' ================================================================================

    Private Sub UpdatePreviewPictureBoxes()
        Try
            ClearAllPreviews()

            If String.IsNullOrWhiteSpace(TxtKodeBarcodeInput.Text) OrElse
               String.IsNullOrWhiteSpace(TxtInputNamaBarang.Text) OrElse
               String.IsNullOrWhiteSpace(TxtInputHargaBarang.Text) Then
                Exit Sub
            End If

            Dim numColumns As Integer = ExtractNumColumns(CmbJumlahKolomPerBaris.Text)
            Dim numLabels As Integer = ExtractNumLabels(TxtJumlahLabelDicetak.Text)

            For labelIndex As Integer = 0 To numLabels - 1
                GeneratePreviewForLabelIndex(labelIndex, numColumns)
            Next

        Catch ex As Exception
            Debug.WriteLine("Error updating preview: " & ex.Message)
        End Try
    End Sub

    Private Sub GeneratePreviewForLabelIndex(labelIndex As Integer, numColumns As Integer)
        Try
            Dim previewData As DataLabelBarcode = CreateBarcodeData()
            If previewData Is Nothing Then Exit Sub

            Dim currentRow As Integer = labelIndex \ numColumns
            Dim currentCol As Integer = labelIndex Mod numColumns
            Dim gridPosition As Integer = (currentRow * 4) + currentCol

            Dim targetPictureBox As PictureBox = GetPictureBoxForGridPosition(gridPosition)
            If targetPictureBox Is Nothing Then Exit Sub

            Dim previewBitmap As Bitmap = CreatePreviewBitmapFitToPictureBox(previewData, targetPictureBox.Width, targetPictureBox.Height)
            If previewBitmap IsNot Nothing Then
                targetPictureBox.Image = previewBitmap
                targetPictureBox.Visible = True
            End If

        Catch ex As Exception
            Debug.WriteLine("Error generating preview for label " & labelIndex & ": " & ex.Message)
        End Try
    End Sub

    Private Function GetPictureBoxForGridPosition(gridPosition As Integer) As PictureBox
        If gridPosition < 0 OrElse gridPosition >= AllPreviewBoxes.Length Then Return Nothing
        Return AllPreviewBoxes(gridPosition)
    End Function

    Private Sub ClearAllPreviews()
        For Each pb In AllPreviewBoxes
            pb.Image = Nothing
            pb.Visible = True
        Next
    End Sub

    Private Function CreatePreviewBitmapFitToPictureBox(data As DataLabelBarcode, picWidth As Integer, picHeight As Integer) As Bitmap
        Try
            Dim targetWidth As Integer = If(picWidth > 0, picWidth, 300)
            Dim targetHeight As Integer = If(picHeight > 0, picHeight, 150)

            Dim bitmap As New Bitmap(targetWidth, targetHeight)

            Using g As Graphics = Graphics.FromImage(bitmap)
                g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                g.Clear(Color.White)
                DrawSingleLabelScaled(g, 0, 0, data, targetWidth, targetHeight)
            End Using

            Return bitmap

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Sub DrawSingleLabelScaled(g As Graphics, xPixel As Integer, yPixel As Integer,
                                  data As DataLabelBarcode, targetWidth As Integer, targetHeight As Integer)
        Try
            Dim baseWidth As Single = 300.0F
            Dim baseHeight As Single = 150.0F
            Dim scaleX As Single = targetWidth / baseWidth
            Dim scaleY As Single = targetHeight / baseHeight
            Dim scaleFactor As Single = Math.Min(scaleX, scaleY)

            Dim marginLeft As Integer = CInt(8 * scaleFactor)
            Dim marginTop As Integer = CInt(5 * scaleFactor)
            Dim marginRight As Integer = CInt(8 * scaleFactor)
            Dim marginBottom As Integer = CInt(5 * scaleFactor)

            Dim hargaAreaWidth As Integer = CInt((targetWidth - marginLeft - marginRight) * (1.0F - PersentaseLebarAreaBarcodeDinamis))
            Dim barcodeAreaWidth As Integer = targetWidth - marginLeft - marginRight - hargaAreaWidth

            Dim contentHeight As Integer = targetHeight - marginTop - marginBottom

            ' ===== GUNAKAN PERHITUNGAN ADAPTIVE YANG SAMA DENGAN TSPL =====
            Dim labelHeightMM As Single = GetDynamicLabelHeight()
            Dim persentaseNama As Single = GetAdaptiveNamaPercentage(labelHeightMM)
            Dim persentaseBarcode As Single = GetAdaptiveBarcodePercentage(labelHeightMM)
            Dim persentaseToko As Single = GetAdaptiveTokoPercentage(labelHeightMM)

            Dim namaHeight As Integer = CInt(contentHeight * persentaseNama)
            Dim barcodeAreaHeight As Integer = CInt(contentHeight * persentaseBarcode)
            Dim tokoAreaHeight As Integer = CInt(contentHeight * persentaseToko)

            ' ===== SPLIT BARCODE AREA: 80% gambar, 20% nomor =====
            Dim barcodeImageHeight As Integer = CInt(barcodeAreaHeight * 0.8)
            Dim nomorBarcodeHeight As Integer = CInt(barcodeAreaHeight * 0.2)

            ' Nama
            Dim namaRect As New Rectangle(xPixel + marginLeft, yPixel + marginTop, barcodeAreaWidth, namaHeight)
            Dim fontSizeNama As Single = Math.Max(8, 12 * scaleFactor)
            Using fontNama As New Font(data.FontNama.FontFamily, fontSizeNama,
                                   If(ChkBoldNamaBarang.Checked, FontStyle.Bold, FontStyle.Regular))
                g.DrawString(TruncateText(data.NamaBarang, TRUNCATE_NAMA_MAX_LENGTH), fontNama, Brushes.Black, namaRect,
                        New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center, .Trimming = StringTrimming.EllipsisCharacter})
            End Using

            ' Barcode
            Dim barcodeY As Integer = yPixel + marginTop + namaHeight + CInt(2 * scaleFactor)
            Dim barcodeImage As Image = GenerateBarcodeImageScaled(data.NomorBarcode, data.BarcodeFormat, barcodeAreaWidth, barcodeImageHeight, scaleFactor)
            If barcodeImage IsNot Nothing Then
                g.DrawImage(barcodeImage, xPixel + marginLeft, barcodeY, barcodeAreaWidth, barcodeImageHeight)
                barcodeImage.Dispose()
            End If

            ' Nomor barcode
            Dim nomorBarcodeY As Integer = barcodeY + barcodeImageHeight
            Dim nomorBarcodeRect As New Rectangle(xPixel + marginLeft, nomorBarcodeY, barcodeAreaWidth, nomorBarcodeHeight)
            Using fontNomorBarcode As New Font("Arial", Math.Max(6, 7 * scaleFactor), FontStyle.Regular)
                g.DrawString(data.NomorBarcode, fontNomorBarcode, Brushes.Black, nomorBarcodeRect,
                        New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center})
            End Using

            ' Nama toko
            Dim tokoY As Integer = nomorBarcodeY + nomorBarcodeHeight + CInt(2 * scaleFactor)
            Dim tokoRect As New Rectangle(xPixel + marginLeft, tokoY, barcodeAreaWidth, tokoAreaHeight - nomorBarcodeHeight - CInt(4 * scaleFactor))
            Dim fontSizeToko As Single = Math.Max(6, 8 * scaleFactor)
            Using fontToko As New Font(data.FontToko.FontFamily, fontSizeToko,
                                   If(ChkBoldNamaToko.Checked, FontStyle.Bold, FontStyle.Regular))
                g.DrawString(TruncateText(data.NamaToko, TRUNCATE_TOKO_MAX_LENGTH), fontToko, Brushes.Black, tokoRect,
                        New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center})
            End Using

            ' Harga (rotasi 270)
            Dim hargaX As Integer = xPixel + marginLeft + barcodeAreaWidth + CInt(5 * scaleFactor)
            Dim hargaY As Integer = yPixel + marginTop
            Dim hargaAreaHeight As Integer = targetHeight - marginTop - marginBottom

            Dim hargaValue As Decimal = 0
            Dim hargaText As String = "Rp. 0"
            Dim satuanText As String = "/" & data.SatuanBarang

            If Decimal.TryParse(data.HargaBarang.Replace(".", "").Replace(",", ""), hargaValue) Then
                hargaText = FormatHarga(hargaValue)
            End If

            Dim oldState As Drawing2D.GraphicsState = g.Save()
            g.TranslateTransform(hargaX + hargaAreaWidth \ 2, hargaY + hargaAreaHeight \ 2)
            g.RotateTransform(270)

            Dim fontSizeHarga As Single = Math.Max(10, 16 * scaleFactor)
            Dim fontSizeSatuan As Single = Math.Max(8, 12 * scaleFactor)

            Using fontHarga As New Font(data.FontHarga.FontFamily, fontSizeHarga, FontStyle.Bold)
                Using fontSatuan As New Font(data.FontSatuan.FontFamily, fontSizeSatuan, FontStyle.Bold)
                    Dim hargaSize As SizeF = g.MeasureString(hargaText, fontHarga)
                    Dim satuanSize As SizeF = g.MeasureString(satuanText, fontSatuan)

                    Dim spacing As Single = hargaSize.Height * 0.2F
                    Dim totalHeight As Single = hargaSize.Height + spacing + satuanSize.Height
                    Dim startYs As Single = -(totalHeight / 2) + (hargaSize.Height / 2)

                    g.DrawString(hargaText, fontHarga, Brushes.Black, New PointF(-(hargaSize.Width / 2), startYs - (hargaSize.Height / 2)))
                    g.DrawString(satuanText, fontSatuan, Brushes.Black, New PointF(-(satuanSize.Width / 2), startYs + (hargaSize.Height / 2) + spacing))
                End Using
            End Using

            g.Restore(oldState)

            ' Border
            g.DrawRectangle(Pens.LightGray, xPixel, yPixel, targetWidth - 1, targetHeight - 1)
            g.DrawLine(Pens.LightBlue, xPixel + marginLeft + barcodeAreaWidth, yPixel + marginTop,
                   xPixel + marginLeft + barcodeAreaWidth, yPixel + targetHeight - marginBottom)

        Catch ex As Exception
            LogMessage($"Error menggambar preview: {ex.Message}", True)
        End Try
    End Sub
    Private Function GenerateBarcodeImageScaled(barcodeValue As String, barcodeFormat As BarcodeFormat,
                                                 width As Integer, height As Integer, scaleFactor As Single) As Image
        Try
            If String.IsNullOrWhiteSpace(barcodeValue) Then Return Nothing

            Dim writer As New BarcodeWriter() With {
                .Format = barcodeFormat,
                .Options = New EncodingOptions With {
                    .width = width * 2,
                    .height = height * 2,
                    .Margin = Math.Max(1, CInt(2 * scaleFactor)),
                    .PureBarcode = False
                }
            }

            Return writer.Write(barcodeValue)

        Catch ex As Exception
            LogMessage($"Error generating barcode: {ex.Message}", True)
            Return Nothing
        End Try
    End Function

    ' ================================================================================
    ' CONFIGURATION FILE HANDLING
    ' ================================================================================

    Private Sub BuatFileDefault()
        Try
            If Not File.Exists(configFilePath) Then
                Dim defaultConfig As New StringBuilder()
                defaultConfig.AppendLine("[Settings]")
                defaultConfig.AppendLine("FontNama=Arial")
                defaultConfig.AppendLine("UkuranNama=8")
                defaultConfig.AppendLine("BoldNama=False")
                defaultConfig.AppendLine("FontHarga=Arial")
                defaultConfig.AppendLine("UkuranHarga=8")
                defaultConfig.AppendLine("BoldHarga=False")
                defaultConfig.AppendLine("FontSatuan=Arial")
                defaultConfig.AppendLine("UkuranSatuan=8")
                defaultConfig.AppendLine("BoldSatuan=False")
                defaultConfig.AppendLine("FontToko=Arial")
                defaultConfig.AppendLine("UkuranToko=8")
                defaultConfig.AppendLine("BoldToko=False")
                defaultConfig.AppendLine("TipeBarcode=CODE 128")
                defaultConfig.AppendLine("JumlahPerBaris=3")
                defaultConfig.AppendLine("LebarBarcode=33")
                defaultConfig.AppendLine("TinggiBarcode=15")
                defaultConfig.AppendLine("PosisiAwalXDinamisMM=0")
                defaultConfig.AppendLine("PosisiAwalYDinamisMM=0")
                defaultConfig.AppendLine("JarakHorizontalDinamisMM=2")
                defaultConfig.AppendLine("JarakVertikalDinamisMM=2")
                defaultConfig.AppendLine("MarginKiriDinamisMM=3")
                defaultConfig.AppendLine("MarginAtasDinamisMM=2")
                defaultConfig.AppendLine("PersentaseLebarAreaBarcodeDinamis=55")
                defaultConfig.AppendLine("OffsetVertikalBarcodeDinamisPixels=5")
                defaultConfig.AppendLine("KecepatanCetakDinamis=4")
                defaultConfig.AppendLine("DensitasCetakDinamis=10")
                defaultConfig.AppendLine("KegelapanCetakDinamis=12")

                File.WriteAllText(configFilePath, defaultConfig.ToString())
                LogMessage("File konfigurasi default dibuat dengan font size 8 (Font ID 1)")
            End If
        Catch ex As Exception
            LogMessage($"Error membuat file default: {ex.Message}", True)
        End Try
    End Sub

    Private Sub LoadPengaturan()
        Try
            isLoadingPreview = True

            If Not File.Exists(configFilePath) Then
                BuatFileDefault()
            End If

            Dim lines() As String = File.ReadAllLines(configFilePath)

            For Each line As String In lines
                If line.Contains("=") Then
                    Dim parts() As String = line.Split("="c)
                    If parts.Length = 2 Then
                        Dim key As String = parts(0).Trim()
                        Dim value As String = parts(1).Trim()

                        Select Case key
                            Case "FontNama" : SafeSetCombo(CmbFontNamaBarang, value)
                            Case "UkuranNama"
                                ' PERBAIKAN: Parse ke Integer dulu agar cocok dengan item di ComboBox
                                Dim sizeVal As Integer
                                If Integer.TryParse(value, sizeVal) Then
                                    CmbUkuranFontNama.SelectedItem = sizeVal
                                Else
                                    SafeSetCombo(CmbUkuranFontNama, value)
                                End If
                            Case "BoldNama" : ChkBoldNamaBarang.Checked = ParseBool(value)

                            Case "FontHarga" : SafeSetCombo(CmbFontHargaBarang, value)
                            Case "UkuranHarga"
                                ' PERBAIKAN: Parse ke Integer dulu
                                Dim sizeVal As Integer
                                If Integer.TryParse(value, sizeVal) Then
                                    CmbUkuranFontHarga.SelectedItem = sizeVal
                                Else
                                    SafeSetCombo(CmbUkuranFontHarga, value)
                                End If
                            Case "BoldHarga" : ChkBoldHargaBarang.Checked = ParseBool(value)

                            Case "FontSatuan" : SafeSetCombo(CmbFontSatuanBarang, value)
                            Case "UkuranSatuan"
                                ' PERBAIKAN: Parse ke Integer dulu
                                Dim sizeVal As Integer
                                If Integer.TryParse(value, sizeVal) Then
                                    CmbUkuranFontSatuan.SelectedItem = sizeVal
                                Else
                                    SafeSetCombo(CmbUkuranFontSatuan, value)
                                End If
                            Case "BoldSatuan" : ChkBoldSatuanBarang.Checked = ParseBool(value)

                            Case "FontToko" : SafeSetCombo(CmbFontNamaToko, value)
                            Case "UkuranToko"
                                ' PERBAIKAN: Parse ke Integer dulu
                                Dim sizeVal As Integer
                                If Integer.TryParse(value, sizeVal) Then
                                    CmbUkuranFontToko.SelectedItem = sizeVal
                                Else
                                    SafeSetCombo(CmbUkuranFontToko, value)
                                End If
                            Case "BoldToko" : ChkBoldNamaToko.Checked = ParseBool(value)

                            Case "TipeBarcode" : SafeSetCombo(CmbJenisFormatBarcode, value)
                            Case "JumlahPerBaris" : SafeSetCombo(CmbJumlahKolomPerBaris, value)
                            Case "LebarBarcode" : TxtLebarLabelMM.Text = value
                            Case "TinggiBarcode" : TxtTinggiLabelMM.Text = value
                            Case "PosisiAwalXDinamisMM" : TxtPosisiAwalXMM.Text = value : Single.TryParse(value, PosisiAwalXDinamisMM)
                            Case "PosisiAwalYDinamisMM" : TxtPosisiAwalYMM.Text = value : Single.TryParse(value, PosisiAwalYDinamisMM)
                            Case "JarakHorizontalDinamisMM" : TxtJarakHorizontalMM.Text = value : Single.TryParse(value, JarakHorizontalDinamisMM)
                            Case "JarakVertikalDinamisMM" : TxtJarakVertikalMM.Text = value : Single.TryParse(value, JarakVertikalDinamisMM)
                            Case "MarginKiriDinamisMM" : TxtMarginDalamKiriMM.Text = value : Single.TryParse(value, MarginKiriDinamisMM)
                            Case "MarginAtasDinamisMM" : TxtMarginDalamAtasMM.Text = value : Single.TryParse(value, MarginAtasDinamisMM)
                            Case "PersentaseLebarAreaBarcodeDinamis"
                                Dim pct As Single
                                If Single.TryParse(value, pct) Then
                                    PersentaseLebarAreaBarcodeDinamis = pct / 100.0F
                                    TxtPersentaseLebarAreaBarcode.Text = pct.ToString("0")
                                End If
                            Case "OffsetVertikalBarcodeDinamisPixels"
                                TxtOffsetVertikalPixels.Text = value
                                Integer.TryParse(value, OffsetVertikalBarcodeDinamisPixels)
                            Case "KecepatanCetakDinamis"
                                TxtKecepatanCetak.Text = value
                                Dim v As Integer
                                If Integer.TryParse(value, v) AndAlso v >= SPEED_MIN AndAlso v <= SPEED_MAX Then
                                    KecepatanCetakDinamis = v
                                End If
                            Case "DensitasCetakDinamis"
                                TxtDensitasCetak.Text = value
                                Dim v As Integer
                                If Integer.TryParse(value, v) AndAlso v >= DENSITY_MIN AndAlso v <= DENSITY_MAX Then
                                    DensitasCetakDinamis = v
                                End If
                            Case "KegelapanCetakDinamis"
                                Dim v As Integer
                                If Integer.TryParse(value, v) AndAlso v >= DARKNESS_MIN AndAlso v <= DARKNESS_MAX Then
                                    KegelapanCetakDinamis = v
                                End If
                        End Select
                    End If
                End If
            Next

            LogMessage("Pengaturan berhasil dimuat")
            isLoadingPreview = False
            UpdatePreviewIfReady()

        Catch ex As Exception
            LogMessage($"Error loading settings: {ex.Message}", True)
            isLoadingPreview = False
        End Try
    End Sub
    Private Sub SafeSetCombo(cmb As ComboBox, value As String)
        If cmb.Items.Contains(value) Then
            cmb.SelectedItem = value
        End If
    End Sub

    Private Function ParseBool(value As String) As Boolean
        Dim result As Boolean = False
        Boolean.TryParse(value, result)
        Return result
    End Function

    Private Sub SimpanPengaturan()
        Try
            If Not ValidasiPengaturan() Then Exit Sub

            RefreshAllDynamicSettings()

            If Not ValidateDynamicSettings() Then Return

            Dim sb As New StringBuilder()
            sb.AppendLine("[Settings]")
            sb.AppendLine($"FontNama={CmbFontNamaBarang.Text}")
            sb.AppendLine($"UkuranNama={CmbUkuranFontNama.Text}")
            sb.AppendLine($"BoldNama={ChkBoldNamaBarang.Checked}")
            sb.AppendLine($"FontHarga={CmbFontHargaBarang.Text}")
            sb.AppendLine($"UkuranHarga={CmbUkuranFontHarga.Text}")
            sb.AppendLine($"BoldHarga={ChkBoldHargaBarang.Checked}")
            sb.AppendLine($"FontSatuan={CmbFontSatuanBarang.Text}")
            sb.AppendLine($"UkuranSatuan={CmbUkuranFontSatuan.Text}")
            sb.AppendLine($"BoldSatuan={ChkBoldSatuanBarang.Checked}")
            sb.AppendLine($"FontToko={CmbFontNamaToko.Text}")
            sb.AppendLine($"UkuranToko={CmbUkuranFontToko.Text}")
            sb.AppendLine($"BoldToko={ChkBoldNamaToko.Checked}")
            sb.AppendLine($"TipeBarcode={CmbJenisFormatBarcode.Text}")
            sb.AppendLine($"JumlahPerBaris={CmbJumlahKolomPerBaris.Text}")
            sb.AppendLine($"LebarBarcode={TxtLebarLabelMM.Text}")
            sb.AppendLine($"TinggiBarcode={TxtTinggiLabelMM.Text}")
            sb.AppendLine($"PosisiAwalXDinamisMM={PosisiAwalXDinamisMM:F1}")
            sb.AppendLine($"PosisiAwalYDinamisMM={PosisiAwalYDinamisMM:F1}")
            sb.AppendLine($"JarakHorizontalDinamisMM={JarakHorizontalDinamisMM:F1}")
            sb.AppendLine($"JarakVertikalDinamisMM={JarakVertikalDinamisMM:F1}")
            sb.AppendLine($"MarginKiriDinamisMM={MarginKiriDinamisMM:F1}")
            sb.AppendLine($"MarginAtasDinamisMM={MarginAtasDinamisMM:F1}")
            sb.AppendLine($"PersentaseLebarAreaBarcodeDinamis={PersentaseLebarAreaBarcodeDinamis * 100:F0}")
            sb.AppendLine($"OffsetVertikalBarcodeDinamisPixels={OffsetVertikalBarcodeDinamisPixels}")
            sb.AppendLine($"KecepatanCetakDinamis={KecepatanCetakDinamis}")
            sb.AppendLine($"DensitasCetakDinamis={DensitasCetakDinamis}")
            sb.AppendLine($"KegelapanCetakDinamis={KegelapanCetakDinamis}")

            File.WriteAllText(configFilePath, sb.ToString())

            MessageBox.Show("✅ Pengaturan berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LogMessage("Pengaturan disimpan")

        Catch ex As Exception
            LogMessage($"Error saving settings: {ex.Message}", True)
            MessageBox.Show("❌ Error menyimpan pengaturan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub RefreshAllDynamicSettings()
        ' Margin
        Single.TryParse(TxtMarginDalamKiriMM.Text, MarginKiriDinamisMM)
        Single.TryParse(TxtMarginDalamAtasMM.Text, MarginAtasDinamisMM)

        ' Jarak/Gap - ENFORCE MINIMUM 0.5mm
        Dim tempGapH As Single = 0
        Dim tempGapV As Single = 0

        If Single.TryParse(TxtJarakHorizontalMM.Text, tempGapH) Then
            If tempGapH < 2.0F Then ' Sesuaikan dengan minimum 2.0mm
                LogMessage($"⚠️ WARNING: Gap Horizontal {tempGapH}mm terlalu kecil, disesuaikan ke 2.0mm", True)
                TxtJarakHorizontalMM.Text = "2.0"
                JarakHorizontalDinamisMM = 2.0F
            Else
                JarakHorizontalDinamisMM = tempGapH
            End If
        End If

        If Single.TryParse(TxtJarakVertikalMM.Text, tempGapV) Then
            If tempGapV < 2.0F Then ' Sesuaikan dengan minimum 2.0mm
                LogMessage($"⚠️ WARNING: Gap Vertikal {tempGapV}mm terlalu kecil, disesuaikan ke 2.0mm", True)
                TxtJarakVertikalMM.Text = "2.0"
                JarakVertikalDinamisMM = 2.0F
            Else
                JarakVertikalDinamisMM = tempGapV
            End If
        End If

        ' Posisi awal
        Single.TryParse(TxtPosisiAwalXMM.Text, PosisiAwalXDinamisMM)
        Single.TryParse(TxtPosisiAwalYMM.Text, PosisiAwalYDinamisMM)

        ' Persentase lebar area barcode
        Dim persen As Single
        If Single.TryParse(TxtPersentaseLebarAreaBarcode.Text, persen) Then
            PersentaseLebarAreaBarcodeDinamis = Math.Max(0.4F, Math.Min(0.8F, persen / 100.0F))
        End If

        ' Offset & Print settings
        Integer.TryParse(TxtOffsetVertikalPixels.Text, OffsetVertikalBarcodeDinamisPixels)

        Dim intVal As Integer
        If Integer.TryParse(TxtKecepatanCetak.Text, intVal) AndAlso intVal >= SPEED_MIN AndAlso intVal <= SPEED_MAX Then
            KecepatanCetakDinamis = intVal
        End If
        If Integer.TryParse(TxtDensitasCetak.Text, intVal) AndAlso intVal >= DENSITY_MIN AndAlso intVal <= DENSITY_MAX Then
            DensitasCetakDinamis = intVal
        End If
    End Sub

    Private Function ValidateDynamicSettings() As Boolean
        Try
            ' Validasi margin
            If MarginKiriDinamisMM < MARGIN_MIN_MM Or MarginKiriDinamisMM > MARGIN_MAX_MM Then
                MessageBox.Show($"❌ Margin kiri harus antara {MARGIN_MIN_MM}-{MARGIN_MAX_MM}mm!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
            If MarginAtasDinamisMM < MARGIN_MIN_MM Or MarginAtasDinamisMM > MARGIN_MAX_MM Then
                MessageBox.Show($"❌ Margin atas harus antara {MARGIN_MIN_MM}-{MARGIN_MAX_MM}mm!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            ' Validasi speed & density
            If KecepatanCetakDinamis < SPEED_MIN Or KecepatanCetakDinamis > SPEED_MAX Then
                MessageBox.Show($"❌ Speed harus antara {SPEED_MIN}-{SPEED_MAX}!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
            If DensitasCetakDinamis < DENSITY_MIN Or DensitasCetakDinamis > DENSITY_MAX Then
                MessageBox.Show($"❌ Density harus antara {DENSITY_MIN}-{DENSITY_MAX}!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
            ' Validasi gap minimum 2.0mm
            If JarakHorizontalDinamisMM < 2.0F OrElse JarakVertikalDinamisMM < 2.0F Then
                MessageBox.Show("❌ Gap horizontal dan vertikal minimal 2.0mm!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If


            Return True
        Catch ex As Exception
            LogMessage($"Error validating dynamic settings: {ex.Message}", True)
            Return False
        End Try
    End Function

    ' ================================================================================
    ' DATABASE OPERATIONS
    ' ================================================================================

    Private Sub AmbildataLengkapBarang(ByVal pencarian As String)
        Try
            If String.IsNullOrWhiteSpace(pencarian) Then
                MessageBox.Show("Nama barang tidak boleh kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                                "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                                "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR " &
                                "FROM tbl_barang WHERE TRIM(NAMA_BARANG) = @Pencarian OR TRIM(ID_BARANG) = @Pencarian LIMIT 1"

            Using cmd = New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@Pencarian", pencarian.Trim())

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim namaBarang As String = If(Not rd.IsDBNull(1), rd.GetString(1), "")
                        Dim barcodekecil As String = If(Not rd.IsDBNull(2), rd.GetString(2), "")
                        Dim barcodesedang As String = If(Not rd.IsDBNull(3), rd.GetString(3), "")
                        Dim barcodebesar As String = If(Not rd.IsDBNull(4), rd.GetString(4), "")

                        CmbPilihSatuanBarang.Items.Clear()
                        Dim satuanKecil As String = If(Not rd.IsDBNull(5), rd.GetString(5), "")
                        Dim satuanSedang As String = If(Not rd.IsDBNull(6), rd.GetString(6), "")
                        Dim satuanBesar As String = If(Not rd.IsDBNull(7), rd.GetString(7), "")

                        If Not String.IsNullOrEmpty(satuanKecil) Then CmbPilihSatuanBarang.Items.Add(satuanKecil)
                        If Not String.IsNullOrEmpty(satuanSedang) Then CmbPilihSatuanBarang.Items.Add(satuanSedang)
                        If Not String.IsNullOrEmpty(satuanBesar) Then CmbPilihSatuanBarang.Items.Add(satuanBesar)

                        Dim hargajualkecil As Decimal = If(Not rd.IsDBNull(8), rd.GetDecimal(8), 0)
                        Dim hargajualsedang As Decimal = If(Not rd.IsDBNull(9), rd.GetDecimal(9), 0)
                        Dim hargajualbesar As Decimal = If(Not rd.IsDBNull(10), rd.GetDecimal(10), 0)

                        TxtInputNamaBarang.Text = namaBarang

                        If CmbPilihSatuanBarang.Items.Count > 0 Then
                            CmbPilihSatuanBarang.SelectedIndex = 0
                            TxtInputHargaBarang.Text = hargajualkecil.ToString("N0")
                            TxtKodeBarcodeInput.Text = barcodekecil
                        End If

                        DataBarang.Clear()
                        DataBarang("HargaKecil") = hargajualkecil
                        DataBarang("HargaSedang") = hargajualsedang
                        DataBarang("HargaBesar") = hargajualbesar
                        DataBarang("BarcodeKecil") = barcodekecil
                        DataBarang("BarcodeSedang") = barcodesedang
                        DataBarang("BarcodeBesar") = barcodebesar
                        DataBarang("SatuanKecil") = satuanKecil
                        DataBarang("SatuanSedang") = satuanSedang
                        DataBarang("SatuanBesar") = satuanBesar

                        UpdatePreviewPictureBoxes()
                        TxtJumlahLabelDicetak.Focus()
                        TxtJumlahLabelDicetak.SelectAll()
                    Else
                        MessageBox.Show("Data barang tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        TxtKodeBarcodeInput.Focus()
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error ambil data barang: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ================================================================================
    ' VALIDATION
    ' ================================================================================

    Private Function ValidasiInput() As Boolean
        If String.IsNullOrWhiteSpace(TxtKodeBarcodeInput.Text) Then
            MessageBox.Show("❌ Kode Barcode tidak boleh kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtKodeBarcodeInput.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(TxtInputNamaBarang.Text) Then
            MessageBox.Show("❌ Nama barang tidak boleh kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtInputNamaBarang.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(TxtInputHargaBarang.Text) Then
            MessageBox.Show("❌ Harga barang tidak boleh kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtInputHargaBarang.Focus()
            Return False
        End If

        Dim harga As Decimal
        If Not Decimal.TryParse(CleanCurrencyInput(TxtInputHargaBarang.Text), harga) Then
            MessageBox.Show("❌ Harga barang harus berupa angka!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtInputHargaBarang.Focus()
            Return False
        End If

        If harga <= 0 Then
            MessageBox.Show("❌ Harga barang harus lebih dari 0!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtInputHargaBarang.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(CmbPilihSatuanBarang.Text) Then
            MessageBox.Show("❌ Satuan barang harus dipilih!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            CmbPilihSatuanBarang.Focus()
            Return False
        End If

        Dim jumlah As Integer
        If Not Integer.TryParse(TxtJumlahLabelDicetak.Text, jumlah) OrElse jumlah <= 0 OrElse jumlah > MAX_PRINT_QUANTITY Then
            MessageBox.Show($"❌ Jumlah cetak harus 1-{MAX_PRINT_QUANTITY}!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtJumlahLabelDicetak.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(CmbJenisPrinter.Text) Then
            MessageBox.Show("❌ Pilih printer terlebih dahulu!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            CmbJenisPrinter.Focus()
            Return False
        End If

        If IsTSCPrinterSelected() AndAlso Not ValidateTSCPaperSize() Then
            Return False
        End If

        Return True
    End Function

    Private Function ValidasiPengaturan() As Boolean
        If String.IsNullOrEmpty(CmbFontNamaBarang.Text) OrElse String.IsNullOrEmpty(CmbUkuranFontNama.Text) Then
            MessageBox.Show("❌ Font Nama harus diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbFontNamaBarang.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(CmbFontHargaBarang.Text) OrElse String.IsNullOrEmpty(CmbUkuranFontHarga.Text) Then
            MessageBox.Show("❌ Font Harga harus diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbFontHargaBarang.Focus()
            Return False
        End If

        Return True
    End Function

    ' ================================================================================
    ' HELPER FUNCTIONS
    ' ================================================================================

    Private Function CreateBarcodeData() As DataLabelBarcode
        Try
            Dim hargaBarang As Decimal = 0
            If Not String.IsNullOrWhiteSpace(TxtInputHargaBarang.Text) Then
                Decimal.TryParse(CleanCurrencyInput(TxtInputHargaBarang.Text), hargaBarang)
            End If

            Dim labelWidthMM As Single = GetDynamicLabelWidth()
            Dim labelHeightMM As Single = GetDynamicLabelHeight()

            If IsTSCPrinterSelected() AndAlso labelWidthMM > TSC_TTP224_MAX_WIDTH_MM Then
                labelWidthMM = TSC_TTP224_MAX_WIDTH_MM
            End If

            Dim jumlahCetak As Integer = ExtractNumLabels(TxtJumlahLabelDicetak.Text)
            Dim barcodeFormat As BarcodeFormat = ParseBarcodeFormat(CmbJenisFormatBarcode.Text)

            Dim data As New DataLabelBarcode With {
                .NamaBarang = If(String.IsNullOrWhiteSpace(TxtInputNamaBarang.Text), "", TxtInputNamaBarang.Text.Trim()),
                .HargaBarang = hargaBarang.ToString("N0"),
                .SatuanBarang = If(String.IsNullOrWhiteSpace(CmbPilihSatuanBarang.Text), "", CmbPilihSatuanBarang.Text.Trim()),
                .NomorBarcode = If(String.IsNullOrWhiteSpace(TxtKodeBarcodeInput.Text), "", TxtKodeBarcodeInput.Text.Trim()),
                .JumlahCetak = jumlahCetak,
                .NamaToko = If(String.IsNullOrEmpty(NAMA_PERUSAHAAN), "TOKO LANCAR", NAMA_PERUSAHAAN),
                .FontNama = CreateFontFromSelection(CmbFontNamaBarang, CmbUkuranFontNama, ChkBoldNamaBarang, 7),
                .FontHarga = CreateFontFromSelection(CmbFontHargaBarang, CmbUkuranFontHarga, ChkBoldHargaBarang, 7),
                .FontSatuan = CreateFontFromSelection(CmbFontSatuanBarang, CmbUkuranFontSatuan, ChkBoldSatuanBarang, 6),
                .FontToko = CreateFontFromSelection(CmbFontNamaToko, CmbUkuranFontToko, ChkBoldNamaToko, 6),
                .BarcodeFormat = barcodeFormat,
                .LabelWidthMM = labelWidthMM,
                .LabelHeightMM = labelHeightMM,
                .MarginLeftMM = PosisiAwalXDinamisMM,
                .MarginTopMM = PosisiAwalYDinamisMM
            }

            Return data

        Catch ex As Exception
            LogMessage($"Error creating barcode data: {ex.Message}", True)
            Return Nothing
        End Try
    End Function

    Private Function CreateFontFromSelection(fontCmb As ComboBox, sizeCmb As ComboBox, boldChk As CheckBox, defaultSize As Single) As Font
        Dim fName = If(String.IsNullOrWhiteSpace(fontCmb.Text), "Arial", fontCmb.Text)
        Dim fSize As Single = defaultSize
        Single.TryParse(sizeCmb.Text, fSize)
        Return New Font(fName, fSize, If(boldChk.Checked, FontStyle.Bold, FontStyle.Regular))
    End Function

    Private Function ParseBarcodeFormat(formatName As String) As BarcodeFormat
        Select Case formatName.ToUpper()
            Case "QR CODE" : Return BarcodeFormat.QR_CODE
            Case "CODE 128" : Return BarcodeFormat.CODE_128
            Case "CODE 39" : Return BarcodeFormat.CODE_39
            Case "EAN-13" : Return BarcodeFormat.EAN_13
            Case "UPC-A" : Return BarcodeFormat.UPC_A
            Case Else : Return BarcodeFormat.CODE_128
        End Select
    End Function

    Private Function TruncateText(text As String, maxLength As Integer) As String
        If String.IsNullOrEmpty(text) Then Return ""
        If text.Length <= maxLength Then Return text
        Return text.Substring(0, maxLength - TRUNCATE_SUFFIX.Length) & TRUNCATE_SUFFIX
    End Function

    Private Function CleanCurrencyInput(input As String) As String
        Return input.Replace(".", "").Replace(",", "").Replace("Rp", "").Trim()
    End Function

    Private Function FormatHargaForLabel(harga As Decimal, labelWidth As Single) As String
        If IsTSCPrinterSelected() AndAlso labelWidth < LABEL_WIDTH_TSC_COMPACT_MM Then
            Return FormatHarga(harga)
        Else
            Return "Rp." & harga.ToString("N0")
        End If
    End Function

    Private Function FormatHarga(harga As Decimal) As String
        If harga >= 1000000000D Then
            Return "Rp." & (harga / 1000000000D).ToString("0.#") & "M"
        ElseIf harga >= 1000000D Then
            Return "Rp." & (harga / 1000000D).ToString("0.#") & "jt"
        ElseIf harga >= 1000D Then
            Return "Rp." & (harga / 1000D).ToString("0") & "K"
        Else
            Return "Rp." & harga.ToString("N0")
        End If
    End Function

    Private Function ExtractNumColumns(text As String) As Integer
        Try
            If String.IsNullOrWhiteSpace(text) Then Return 1 ' Default ke 1, bukan 2

            ' Handle jika text mengandung angka saja atau format "3 (optimal)"
            Dim cleanText As String = text
            If text.Contains("(") Then
                cleanText = text.Split("("c)(0).Trim()
            End If

            Dim result As Integer
            If Integer.TryParse(cleanText, result) Then
                ' Batasi antara 1-4
                If result < 1 Then Return 1
                If result > 4 Then Return 4
                Return result
            Else
                Return 1 ' Default ke 1 jika parsing gagal
            End If
        Catch ex As Exception
            Debug.WriteLine($"[ExtractNumColumns] Error: {ex.Message}")
            Return 1
        End Try
    End Function

    Private Function ExtractNumLabels(text As String) As Integer
        Try
            If String.IsNullOrWhiteSpace(text) Then Return 1
            Dim result As Integer
            If Integer.TryParse(text, result) AndAlso result >= 1 AndAlso result <= 12 Then
                Return result
            Else
                Return 1
            End If
        Catch ex As Exception
            Return 1
        End Try
    End Function

    Private Sub LogMessage(message As String, Optional isError As Boolean = False)
        Try
            ' Log only to debug/terminal — do not write to file
            Dim logEntry As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " - " &
                                 If(isError, "[ERROR] ", "[INFO] ") & message
            Debug.WriteLine(logEntry)
        Catch ex As Exception
            ' Swallow exceptions to avoid impacting printing flow
        End Try
    End Sub

    ' ================================================================================
    ' BUTTON EVENT HANDLERS
    ' ================================================================================

    Private Sub BtnPreviewLabel_Click(sender As Object, e As EventArgs) Handles BtnPreviewLabel.Click
        If Not ValidasiInput() Then Exit Sub

        Try
            UpdatePreviewPictureBoxes()
            Dim numColumns As Integer = ExtractNumColumns(CmbJumlahKolomPerBaris.Text)

            LabelStatusInfo.Text = $"✅ Preview ditampilkan ({numColumns} kolom)"
            LabelStatusInfo.ForeColor = Color.Green

        Catch ex As Exception
            LogMessage($"Error preview: {ex.Message}", True)
            MessageBox.Show("❌ Error preview: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnCetakLabel_Click(sender As Object, e As EventArgs) Handles BtnCetakLabel.Click
        If Not ValidasiInput() Then Exit Sub

        Try
            RefreshAllDynamicSettings()
            Dim totalCetak As Integer = Integer.Parse(TxtJumlahLabelDicetak.Text)
            Dim jumlahKolom As Integer = ExtractNumColumns(CmbJumlahKolomPerBaris.Text)

            If totalCetak <= 0 OrElse jumlahKolom <= 0 Then Exit Sub

            Dim data As DataLabelBarcode = CreateBarcodeData()
            If data Is Nothing Then Exit Sub

            Dim totalPrinted As Integer = 0
            Dim asciiEncoding As Encoding = Encoding.ASCII

            ' CETAK BATCH PENUH
            Dim fullBatches As Integer = totalCetak \ jumlahKolom
            If fullBatches > 0 Then
                ' Generate TSPL untuk satu set (jumlahKolom label) dan cetak fullBatches kali
                Dim tsplFull As String = GenerateTSPLCommands(data, jumlahKolom, jumlahKolom)
                If String.IsNullOrEmpty(tsplFull) Then
                    MessageBox.Show("Gagal membuat perintah cetak batch (penuh)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                ' Ganti "PRINT 1,1" dengan "PRINT 1,fullBatches"
                tsplFull = tsplFull.Replace("PRINT 1,1", $"PRINT 1,{fullBatches}")
                Dim tsplBytes() As Byte = asciiEncoding.GetBytes(tsplFull)
                If Not SendBytesToPrinter(CmbJenisPrinter.Text, tsplBytes) Then Exit Sub
                totalPrinted += (jumlahKolom * fullBatches)
                LogMessage($"Batch Penuh: {jumlahKolom} x {fullBatches} = {totalPrinted} labels")
            End If

            ' CETAK SISA
            Dim remainder As Integer = totalCetak Mod jumlahKolom
            If remainder > 0 Then
                ' Generate TSPL untuk sisa (remainder label) dan cetak 1 kali
                Dim tsplRemainder As String = GenerateTSPLCommands(data, remainder, remainder)
                If String.IsNullOrEmpty(tsplRemainder) Then
                    MessageBox.Show("Gagal membuat perintah cetak sisa!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                ' GenerateTSPLCommands sudah menghasilkan "PRINT 1,1" untuk sisa, jadi tidak perlu Replace
                Dim tsplBytes() As Byte = asciiEncoding.GetBytes(tsplRemainder)
                If Not SendBytesToPrinter(CmbJenisPrinter.Text, tsplBytes) Then Exit Sub
                totalPrinted += remainder
                LogMessage($"Sisa: {remainder} labels")
            End If

            MessageBox.Show($"✅ Cetak Berhasil!" & vbCrLf &
                      $"Total Label: {totalCetak}" & vbCrLf &
                      $"Terkirim: {totalPrinted}",
                      "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LogMessage($"Cetak selesai: {totalPrinted} labels")

        Catch ex As Exception
            LogMessage($"Error cetak: {ex.Message}", True)
            MessageBox.Show("❌ Error cetak: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub BtnSimpanPengaturan_Click(sender As Object, e As EventArgs) Handles BtnSimpanPengaturan.Click
        SimpanPengaturan()
        UpdatePreviewPictureBoxes()
    End Sub

    Private Sub BtnResetKeDefault_Click(sender As Object, e As EventArgs) Handles BtnResetKeDefault.Click
        Try
            If MessageBox.Show("⚠️ Reset semua pengaturan ke default?", "Konfirmasi",
                             MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                If File.Exists(configFilePath) Then File.Delete(configFilePath)
                BuatFileDefault()
                LoadPengaturan()
                UpdatePreviewPictureBoxes()
                MessageBox.Show("✅ Pengaturan direset ke default!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show("❌ Error reset: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnResetJarakLabel_Click(sender As Object, e As EventArgs) Handles BtnResetJarakLabel.Click
        Try
            If String.IsNullOrWhiteSpace(CmbJenisPrinter.Text) Then
                MessageBox.Show("❌ Pilih printer terlebih dahulu!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Validasi nilai gap
            If JarakHorizontalDinamisMM <= 0 Or JarakVertikalDinamisMM <= 0 Then
                If MessageBox.Show("⚠️ Nilai gap belum diatur!" & vbCrLf &
                         "Gunakan default 2.0 mm?",
                         "Konfirmasi", MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If
                JarakHorizontalDinamisMM = 2.0F
                JarakVertikalDinamisMM = 2.0F
                TxtJarakHorizontalMM.Text = "2.0"
                TxtJarakVertikalMM.Text = "2.0"
            End If

            Dim isTSC As Boolean = IsTSCPrinterSelected()
            Dim msg As String = $"⚠️ KALIBRASI GAP PRINTER{If(isTSC, " TSC", "")}" & vbCrLf &
                       $"Printer: {CmbJenisPrinter.Text}" & vbCrLf &
                       $"Gap H: {JarakHorizontalDinamisMM:F1}mm, V: {JarakVertikalDinamisMM:F1}mm" & vbCrLf &
                       "Lanjutkan?"

            If MessageBox.Show(msg, "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            ' Simpan pengaturan terlebih dahulu
            RefreshAllDynamicSettings()
            SimpanPengaturan()

            ' Generate perintah kalibrasi
            Dim tsplCalibration As String = GenerateGapCalibrationCommands()
            If String.IsNullOrEmpty(tsplCalibration) Then
                MessageBox.Show("❌ Gagal membuat perintah kalibrasi!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            LogMessage("=== GAP CALIBRATION COMMANDS ===")
            LogMessage(tsplCalibration)
            LogMessage("=== END CALIBRATION ===")

            ' **PERBAIKAN: ASCII Encoding**
            Dim asciiEncoding As Encoding = Encoding.ASCII
            Dim tsplBytes() As Byte = asciiEncoding.GetBytes(tsplCalibration)

            Dim success As Boolean = SendBytesToPrinter(CmbJenisPrinter.Text, tsplBytes)

            If success Then
                MessageBox.Show($"✅ Kalibrasi Berhasil{If(isTSC, " (TSC Mode)", "")}" & vbCrLf &
                          $"Gap: {JarakHorizontalDinamisMM:F1}mm x {JarakVertikalDinamisMM:F1}mm",
                          "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LogMessage("Kalibrasi gap berhasil")
                UpdatePreviewPictureBoxes()
            Else
                MessageBox.Show("❌ Gagal mengirim perintah kalibrasi!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            LogMessage($"Error kalibrasi gap: {ex.Message}", True)
            MessageBox.Show("❌ Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Function GenerateGapCalibrationCommands() As String
        Dim sb As New StringBuilder()
        Try
            Dim lebarLabel As Single = GetDynamicLabelWidth()
            Dim tinggiLabel As Single = GetDynamicLabelHeight()

            ' **PERBAIKAN: ENFORCE minimum GAP 2mm**
            Dim gapH As Single = Math.Max(2.0F, JarakHorizontalDinamisMM)
            Dim gapV As Single = Math.Max(2.0F, JarakVertikalDinamisMM)

            ' Warning
            If JarakHorizontalDinamisMM < 2.0F Or JarakVertikalDinamisMM < 2.0F Then
                LogMessage($"⚠️ CALIBRATION: GAP disesuaikan ke minimum 2mm", True)
            End If

            ' Header
            sb.AppendLine($"! {KecepatanCetakDinamis} {DensitasCetakDinamis} {DensitasCetakDinamis} {CInt(tinggiLabel * TSC_DOTS_PER_MM)} 1")
            sb.AppendLine("CLS")
            sb.AppendLine($"SIZE {lebarLabel:F1} mm,{tinggiLabel:F1} mm")

            ' **PERBAIKAN: Gunakan gapH dan gapV yang sudah di-enforce**
            sb.AppendLine($"GAP {gapH:F1} mm,{gapV:F1} mm")

            ' TSC Sensor Calibration
            If IsTSCPrinterSelected() Then
                ' Pastikan printer TTP-224 mendukung perintah SENSOR ADJUST
                ' Jika tidak, hapus baris berikut atau ganti dengan perintah alternatif
                sb.AppendLine("SENSOR ON")
                sb.AppendLine("SENSOR ADJUST") ' Periksa manual printer TTP-224
                sb.AppendLine("SENSOR OFF")
                LogMessage("TSC: Sensor calibration enabled (pastikan printer mendukung SENSOR ADJUST)")
            Else
                sb.AppendLine("GAP SENSE")
            End If

            sb.AppendLine("DIRECTION 1,0")
            sb.AppendLine("REFERENCE 0,0")
            sb.AppendLine("OFFSET 0 mm")
            sb.AppendLine($"SPEED {KecepatanCetakDinamis}")
            sb.AppendLine($"DENSITY {DensitasCetakDinamis}")

            ' Test print - GUNAKAN FONT ID 1 (BUKAN 0)
            sb.AppendLine("BARCODE 50,50,""128"",40,1,0,2,2,""GAP_OK""")
            sb.AppendLine($"TEXT 200,120,1,0,1,1,""Gap:{gapH}mm""")
            sb.AppendLine($"TEXT 200,150,1,0,1,1,""CALIBRATED""")

            sb.AppendLine("PRINT 1,1")
            sb.AppendLine("FEED 50")

            Return sb.ToString()

        Catch ex As Exception
            LogMessage($"Error generating gap calibration: {ex.Message}", True)
            Return ""
        End Try
    End Function


    Private Sub BtnMundurLabel_Click(sender As Object, e As EventArgs) Handles BtnMundurLabel.Click
        Try
            If String.IsNullOrWhiteSpace(CmbJenisPrinter.Text) Then
                MessageBox.Show("❌ Pilih printer terlebih dahulu!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim jumlahLabel As Integer
            If Not Integer.TryParse(TxtJumlahLabelMundur.Text, jumlahLabel) OrElse
           jumlahLabel <= 0 OrElse jumlahLabel > MAX_BACKWARD_FEED Then
                MessageBox.Show($"❌ Jumlah label harus 1-{MAX_BACKWARD_FEED}!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            If MessageBox.Show($"Tarik {jumlahLabel} label mundur?", "Konfirmasi",
                         MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            Dim tsplFeedCommands As String = GeneratePaperFeedBackwardCommands(jumlahLabel)

            ' **PERBAIKAN: ASCII Encoding**
            Dim asciiEncoding As Encoding = Encoding.ASCII
            Dim tsplBytes() As Byte = asciiEncoding.GetBytes(tsplFeedCommands)

            Dim success As Boolean = SendBytesToPrinter(CmbJenisPrinter.Text, tsplBytes)

            If success Then
                MessageBox.Show($"✅ Kertas ditarik {jumlahLabel} label", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LogMessage($"Tarik kertas {jumlahLabel} labels")
            Else
                MessageBox.Show("❌ Gagal tarik kertas!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            LogMessage($"Error tarik kertas: {ex.Message}", True)
            MessageBox.Show("❌ Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Function GeneratePaperFeedBackwardCommands(jumlahLabel As Integer) As String
        Dim sb As New StringBuilder()
        Try
            ' Gunakan tinggi label aktual dalam dot
            Dim labelHeightMM As Single = GetDynamicLabelHeight()
            Dim dotsPerMM As Single = GetDynamicDotsPerMM()
            Dim labelHeightDots As Integer = CInt(Math.Round(labelHeightMM * dotsPerMM))

            sb.AppendLine($"! {KecepatanCetakDinamis} {DensitasCetakDinamis} {DensitasCetakDinamis} {labelHeightDots} 1")
            sb.AppendLine("CLS")

            Dim totalDots As Integer = jumlahLabel * labelHeightDots
            sb.AppendLine($"BACKFEED {totalDots}")

            LogMessage($"BACKFEED command: {totalDots} dots ({jumlahLabel} labels @ {labelHeightDots} dots/label)")

            Return sb.ToString()

        Catch ex As Exception
            LogMessage($"Error generating backfeed: {ex.Message}", True)
            Return ""
        End Try
    End Function

    ' ================================================================================
    ' REALTIME EVENT HANDLERS
    ' ================================================================================

    Private Sub RealtimePreviewTrigger(sender As Object, e As EventArgs) Handles _
           CmbFontNamaBarang.SelectedIndexChanged, CmbUkuranFontNama.SelectedIndexChanged, ChkBoldNamaBarang.CheckedChanged,
           CmbFontHargaBarang.SelectedIndexChanged, CmbUkuranFontHarga.SelectedIndexChanged, ChkBoldHargaBarang.CheckedChanged,
           CmbFontSatuanBarang.SelectedIndexChanged, CmbUkuranFontSatuan.SelectedIndexChanged, ChkBoldSatuanBarang.CheckedChanged,
           CmbFontNamaToko.SelectedIndexChanged, CmbUkuranFontToko.SelectedIndexChanged, ChkBoldNamaToko.CheckedChanged,
           CmbJenisFormatBarcode.SelectedIndexChanged, CmbJumlahKolomPerBaris.SelectedIndexChanged,
           TxtInputNamaBarang.TextChanged, TxtKodeBarcodeInput.TextChanged, TxtInputHargaBarang.TextChanged,
           TxtTinggiLabelMM.TextChanged, TxtJarakHorizontalMM.TextChanged, TxtJarakVertikalMM.TextChanged,
           TxtLebarLabelMM.TextChanged, TxtPosisiAwalXMM.TextChanged, TxtPosisiAwalYMM.TextChanged,
           TxtMarginDalamKiriMM.TextChanged, TxtMarginDalamAtasMM.TextChanged,
           TxtPersentaseLebarAreaBarcode.TextChanged,
           TxtOffsetVertikalPixels.TextChanged, TxtKecepatanCetak.TextChanged, TxtDensitasCetak.TextChanged,
           CmbJenisPrinter.SelectedIndexChanged, CmbPilihSatuanBarang.SelectedIndexChanged

        ParseDynamicVariable(sender)
        UpdatePreviewIfReady()
    End Sub

    Private Sub ParseDynamicVariable(sender As Object)
        Select Case True
            Case sender Is TxtMarginDalamKiriMM
                Single.TryParse(TxtMarginDalamKiriMM.Text, MarginKiriDinamisMM)
            Case sender Is TxtMarginDalamAtasMM
                Single.TryParse(TxtMarginDalamAtasMM.Text, MarginAtasDinamisMM)
            Case sender Is TxtJarakHorizontalMM
                Single.TryParse(TxtJarakHorizontalMM.Text, JarakHorizontalDinamisMM)
            Case sender Is TxtJarakVertikalMM
                Single.TryParse(TxtJarakVertikalMM.Text, JarakVertikalDinamisMM)
            Case sender Is TxtPosisiAwalXMM
                Single.TryParse(TxtPosisiAwalXMM.Text, PosisiAwalXDinamisMM)
                ClampTSCValue(TxtPosisiAwalXMM, Integer.MaxValue, "Start X", True)
            Case sender Is TxtPosisiAwalYMM
                Single.TryParse(TxtPosisiAwalYMM.Text, PosisiAwalYDinamisMM)
                ClampTSCValue(TxtPosisiAwalYMM, Integer.MaxValue, "Start Y", True)
            Case sender Is TxtPersentaseLebarAreaBarcode
                Dim v As Single
                If Single.TryParse(TxtPersentaseLebarAreaBarcode.Text, v) Then
                    PersentaseLebarAreaBarcodeDinamis = Math.Max(0.4F, Math.Min(0.8F, v / 100.0F))
                End If
            Case sender Is TxtOffsetVertikalPixels
                Integer.TryParse(TxtOffsetVertikalPixels.Text, OffsetVertikalBarcodeDinamisPixels)
            Case sender Is TxtKecepatanCetak
                Dim v As Integer
                If Integer.TryParse(TxtKecepatanCetak.Text, v) AndAlso v >= SPEED_MIN AndAlso v <= SPEED_MAX Then
                    KecepatanCetakDinamis = v
                End If
            Case sender Is TxtDensitasCetak
                Dim v As Integer
                If Integer.TryParse(TxtDensitasCetak.Text, v) AndAlso v >= DENSITY_MIN AndAlso v <= DENSITY_MAX Then
                    DensitasCetakDinamis = v
                End If
            Case sender Is CmbJenisPrinter
                UpdatePrinterStatusLabel()
                ClampTSCValue(TxtLebarLabelMM, TSC_TTP224_MAX_WIDTH_MM, "Lebar label")
            Case sender Is TxtLebarLabelMM
                ClampTSCValue(TxtLebarLabelMM, TSC_TTP224_MAX_WIDTH_MM, "Lebar label")
            Case sender Is CmbPilihSatuanBarang
                UpdateSatuanSelection()
        End Select
    End Sub
    Private Sub UpdateSatuanSelection()
        If DataBarang.Count = 0 OrElse CmbPilihSatuanBarang.SelectedItem Is Nothing Then Return

        Dim satuan = CmbPilihSatuanBarang.SelectedItem.ToString()

        If satuan = DataBarang("SatuanKecil").ToString() Then
            TxtInputHargaBarang.Text = Decimal.Parse(DataBarang("HargaKecil").ToString()).ToString("N0")
            TxtKodeBarcodeInput.Text = DataBarang("BarcodeKecil").ToString()
        ElseIf satuan = DataBarang("SatuanSedang").ToString() Then
            TxtInputHargaBarang.Text = Decimal.Parse(DataBarang("HargaSedang").ToString()).ToString("N0")
            TxtKodeBarcodeInput.Text = DataBarang("BarcodeSedang").ToString()
        ElseIf satuan = DataBarang("SatuanBesar").ToString() Then
            TxtInputHargaBarang.Text = Decimal.Parse(DataBarang("HargaBesar").ToString()).ToString("N0")
            TxtKodeBarcodeInput.Text = DataBarang("BarcodeBesar").ToString()
        End If
    End Sub

    Private Sub ClampTSCValue(txt As TextBox, max As Single, fieldName As String, Optional minZero As Boolean = False)
        If Not IsTSCPrinterSelected() Then Return
        Dim val As Single
        If Not Single.TryParse(txt.Text, val) Then Return

        If minZero AndAlso val < 0 Then
            txt.Text = "0"
            LogMessage($"TSC: {fieldName} diset ke 0")
        ElseIf val > max Then
            txt.Text = max.ToString()
            LogMessage($"TSC: {fieldName} disesuaikan ke {max}mm")
        End If
    End Sub

    Private Sub TxtJumlahLabelDicetak_TextChanged(sender As Object, e As EventArgs) Handles TxtJumlahLabelDicetak.TextChanged
        Dim val As Integer
        If Integer.TryParse(TxtJumlahLabelDicetak.Text, val) Then
            If val < 1 Then TxtJumlahLabelDicetak.Text = "1"
            If val > MAX_PRINT_QUANTITY Then TxtJumlahLabelDicetak.Text = MAX_PRINT_QUANTITY.ToString()
        End If
        UpdatePreviewIfReady()
    End Sub

    Private Sub TxtJumlahLabelMundur_TextChanged(sender As Object, e As EventArgs) Handles TxtJumlahLabelMundur.TextChanged
        Dim val As Integer
        If Integer.TryParse(TxtJumlahLabelMundur.Text, val) Then
            If val < 1 Then TxtJumlahLabelMundur.Text = "1"
            If val > MAX_BACKWARD_FEED Then TxtJumlahLabelMundur.Text = MAX_BACKWARD_FEED.ToString()
        End If
    End Sub

    Private Sub UpdatePreviewIfReady()
        If isLoadingPreview Then Return
        If String.IsNullOrWhiteSpace(TxtKodeBarcodeInput.Text) OrElse
           String.IsNullOrWhiteSpace(TxtInputNamaBarang.Text) OrElse
           String.IsNullOrWhiteSpace(TxtInputHargaBarang.Text) Then Return

        Static lastUpdate As DateTime
        If (DateTime.Now - lastUpdate).TotalMilliseconds < 300 Then Return
        lastUpdate = DateTime.Now

        UpdatePreviewPictureBoxes()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class

' ================================================================================
' DATA CLASS
' ================================================================================

Public Class DataLabelBarcode
    Public Property NamaBarang As String
    Public Property HargaBarang As String
    Public Property SatuanBarang As String
    Public Property NomorBarcode As String
    Public Property JumlahCetak As Integer
    Public Property NamaToko As String
    Public Property FontNama As Font
    Public Property FontHarga As Font
    Public Property FontSatuan As Font
    Public Property FontToko As Font
    Public Property BarcodeFormat As ZXing.BarcodeFormat
    Public Property LabelWidthMM As Single
    Public Property LabelHeightMM As Single
    Public Property MarginLeftMM As Single
    Public Property MarginTopMM As Single
End Class