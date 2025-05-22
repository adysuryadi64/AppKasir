Imports System.Drawing.Printing
Imports System.Drawing.Text
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports ZXing
Imports ZXing.Common




Public Class FormCetakBarcode
    Private configFilePath As String = Path.Combine(Application.StartupPath, "ConfigBarcodeBarang.ini")
    Private Sub FormCetakBarcode_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadFontSizes()
        LoadFontKeComboBox()
        LoadPrinterKeComboBox()
        BarcodePerbaris()
        BuatFileDefault()
        LoadPengaturan()
    End Sub

    ' 🔹 Fungsi untuk mengatur ukuran font berdasarkan panjang teks
    Private Sub LoadFontSizes()
        ' Daftar ukuran font yang umum digunakan
        Dim fontSizes As Integer() = {8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 36, 40, 44, 48, 52, 56, 60, 64, 72}

        ' Tambahkan ukuran font ke ComboBox
        CmbUkuranNama.Items.Clear()
        CmbUkuranHarga.Items.Clear()
        CmbUkuranSatuan.Items.Clear()
        CmbUkuranToko.Items.Clear()

        For Each fontSize As Integer In fontSizes
            CmbUkuranNama.Items.Add(fontSize)
            CmbUkuranHarga.Items.Add(fontSize)
            CmbUkuranSatuan.Items.Add(fontSize)
            CmbUkuranToko.Items.Add(fontSize)
        Next
    End Sub


    ' 🔹 Fungsi untuk mendapatkan daftar font yang terinstal di komputer
    Private Sub LoadFontKeComboBox()
        Dim fonts As New InstalledFontCollection()

        ' Kosongkan ComboBox dulu agar tidak duplikat
        CmbFontNama.Items.Clear()
        CmbFontHarga.Items.Clear()
        CmbFontSatuan.Items.Clear()
        CmbFontToko.Items.Clear()

        ' Tambahkan setiap font yang ditemukan
        For Each fontFamily As FontFamily In fonts.Families
            CmbFontNama.Items.Add(fontFamily.Name)
            CmbFontHarga.Items.Add(fontFamily.Name)
            CmbFontSatuan.Items.Add(fontFamily.Name)
            CmbFontToko.Items.Add(fontFamily.Name)
        Next
    End Sub

    ' 🔹 Fungsi untuk membuat file konfigurasi default jika belum ada
    Public Sub BuatFileDefault()
        Try
            ' Periksa apakah file konfigurasi sudah ada
            If Not File.Exists(configFilePath) Then
                Dim sb As New StringBuilder()

                ' 🔹 Konfigurasi Barcode
                sb.AppendLine()
                sb.AppendLine("[PengaturanBarcode]")
                sb.AppendLine("BarcodeJenisPrinter=") ' Biarkan kosong untuk konfigurasi manual
                sb.AppendLine("BarcodeJenisKertas=A4")
                sb.AppendLine("BarcodeTinggiKertas=297") ' Dalam mm
                sb.AppendLine("BarcodeLebarKertas=210") ' Dalam mm

                sb.AppendLine()
                sb.AppendLine("[BarcodeFont]")
                sb.AppendLine("BarcodeFontNama=Arial")
                sb.AppendLine("BarcodeUkuranNama=12")
                sb.AppendLine("BarcodeBoldNama=False")
                sb.AppendLine("BarcodeFontHarga=Tahoma")
                sb.AppendLine("BarcodeUkuranHarga=14")
                sb.AppendLine("BarcodeBoldHarga=True")
                sb.AppendLine("BarcodeFontSatuan=Tahoma")
                sb.AppendLine("BarcodeUkuranSatuan=10")
                sb.AppendLine("BarcodeBoldSatuan=False")
                sb.AppendLine("BarcodeFontToko=Verdana")
                sb.AppendLine("BarcodeUkuranToko=10")
                sb.AppendLine("BarcodeBoldToko=False")

                sb.AppendLine()
                sb.AppendLine("[PosisiBarcode]")
                sb.AppendLine("BarcodeJumlahPerBaris=3") ' Default 3 label per baris
                sb.AppendLine("BarcodeFormatLabel=2") ' Format label barcode
                sb.AppendLine("LebarBarcode=50")
                sb.AppendLine("TinggiBarcode=30")
                sb.AppendLine("BarcodeStartX=10")
                sb.AppendLine("BarcodeStartY=10")
                sb.AppendLine("BarcodeGapX=5")
                sb.AppendLine("BarcodeGapY=5")

                ' Tulis ke file dengan encoding UTF-8
                File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8)
            End If

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat membuat file konfigurasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' ====== 2. Load Konfigurasi ======
    Public Sub LoadPengaturan()

        ' Periksa apakah file konfigurasi ada
        If Not File.Exists(configFilePath) Then
            MessageBox.Show("File konfigurasi tidak ditemukan. Pastikan telah dibuat sebelumnya.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Membaca file konfigurasi
        Dim lines() As String = File.ReadAllLines(configFilePath)
        Dim config As New Dictionary(Of String, String)

        ' Memproses setiap baris dalam file konfigurasi
        For Each line As String In lines
            If Not String.IsNullOrWhiteSpace(line) AndAlso line.Contains("=") Then
                Dim parts() As String = line.Split(New Char() {"="c}, 2)
                Dim key As String = parts(0).Trim()
                Dim value As String = parts(1).Trim()
                config(key) = value
            End If
        Next

        ' 🔹 Load Data ke ComboBox dan TextBox
        If config.ContainsKey("BarcodeJenisPrinter") Then CmbJenisPrinter.Text = config("BarcodeJenisPrinter")
        If config.ContainsKey("BarcodeJenisKertas") Then CmbJenisKertas.Text = config("BarcodeJenisKertas")
        If config.ContainsKey("BarcodeLebarKertas") Then TxtLebarKertas.Text = config("BarcodeLebarKertas")
        If config.ContainsKey("BarcodeTinggiKertas") Then TxtTinggiKertas.Text = config("BarcodeTinggiKertas")


        ' 🔹 Load Data Font
        If config.ContainsKey("BarcodeFontNama") Then CmbFontNama.Text = config("BarcodeFontNama")
        If config.ContainsKey("BarcodeUkuranNama") Then CmbUkuranNama.Text = config("BarcodeUkuranNama")
        If config.ContainsKey("BarcodeBoldNama") Then ChkBoldNama.Checked = Boolean.Parse(config("BarcodeBoldNama"))

        If config.ContainsKey("BarcodeFontHarga") Then CmbFontHarga.Text = config("BarcodeFontHarga")
        If config.ContainsKey("BarcodeUkuranHarga") Then CmbUkuranHarga.Text = config("BarcodeUkuranHarga")
        If config.ContainsKey("BarcodeBoldHarga") Then ChkBoldHarga.Checked = Boolean.Parse(config("BarcodeBoldHarga"))

        If config.ContainsKey("BarcodeFontSatuan") Then CmbFontSatuan.Text = config("BarcodeFontSatuan")
        If config.ContainsKey("BarcodeUkuranSatuan") Then CmbUkuranSatuan.Text = config("BarcodeUkuranSatuan")
        If config.ContainsKey("BarcodeBoldSatuan") Then ChkBoldSatuan.Checked = Boolean.Parse(config("BarcodeBoldSatuan"))

        If config.ContainsKey("BarcodeFontToko") Then CmbFontToko.Text = config("BarcodeFontToko")
        If config.ContainsKey("BarcodeUkuranToko") Then CmbUkuranToko.Text = config("BarcodeUkuranToko")
        If config.ContainsKey("BarcodeBoldToko") Then ChkBoldToko.Checked = Boolean.Parse(config("BarcodeBoldToko"))

        ' 🔹 Load Posisi dan Jarak
        If config.ContainsKey("BarcodeJumlahPerBaris") Then CmbJumlahBarcodePerBaris.Text = config("BarcodeJumlahPerBaris")
        If config.ContainsKey("BarcodeFormatLabel") Then CmbTipeBarcode.Text = config("BarcodeFormatLabel")
        If config.ContainsKey("LebarBarcode") Then TxtLebarBarcode.Text = config("LebarBarcode")
        If config.ContainsKey("TinggiBarcode") Then TxtTinggiBarcode.Text = config("TinggiBarcode")
        If config.ContainsKey("BarcodeStartX") Then TxtStartX.Text = config("BarcodeStartX")
        If config.ContainsKey("BarcodeStartY") Then TxtStartY.Text = config("BarcodeStartY")
        If config.ContainsKey("BarcodeGapX") Then TxtGabX.Text = config("BarcodeGapX")
        If config.ContainsKey("BarcodeGapY") Then TxtGabY.Text = config("BarcodeGapY")
    End Sub


    Private Sub BtnSimpanPerubahan_Click(sender As Object, e As EventArgs) Handles btnSimpanPerubahan.Click
        SimpanPengaturan()
    End Sub

    ' ====== 3. Simpan Konfigurasi ======
    ' 🔹 Fungsi untuk membuat file konfigurasi default jika belum ada
    Public Sub SimpanPengaturan()
        Try
            Dim sb As New StringBuilder()

            ' 🔹 Pengaturan Barcode
            sb.AppendLine("[PengaturanBarcode]")
            sb.AppendLine("BarcodeJenisPrinter=" & CmbJenisPrinter.Text)
            sb.AppendLine("BarcodeJenisKertas=" & CmbJenisKertas.Text)
            sb.AppendLine("BarcodeLebarKertas=" & TxtLebarKertas.Text)
            sb.AppendLine("BarcodeTinggiKertas=" & TxtTinggiKertas.Text)

            ' 🔹 Konfigurasi Font Barcode
            sb.AppendLine()
            sb.AppendLine("[FontBarcode]")
            sb.AppendLine("BarcodeFontNama=" & CmbFontNama.Text)
            sb.AppendLine("BarcodeUkuranNama=" & CmbUkuranNama.Text)
            sb.AppendLine("BarcodeBoldNama=" & ChkBoldNama.Checked.ToString)
            sb.AppendLine("BarcodeFontHarga=" & CmbFontHarga.Text)
            sb.AppendLine("BarcodeUkuranHarga=" & CmbUkuranHarga.Text)
            sb.AppendLine("BarcodeBoldHarga=" & ChkBoldHarga.Checked.ToString)
            sb.AppendLine("BarcodeFontSatuan=" & CmbFontSatuan.Text)
            sb.AppendLine("BarcodeUkuranSatuan=" & CmbUkuranSatuan.Text)
            sb.AppendLine("BarcodeBoldSatuan=" & ChkBoldSatuan.Checked.ToString)
            sb.AppendLine("BarcodeFontToko=" & CmbFontToko.Text)
            sb.AppendLine("BarcodeUkuranToko=" & CmbUkuranToko.Text)
            sb.AppendLine("BarcodeBoldToko=" & ChkBoldToko.Checked.ToString)

            ' 🔹 Posisi dan Jarak Barcode
            sb.AppendLine()
            sb.AppendLine("[PosisiBarcode]")
            sb.AppendLine("BarcodeJumlahPerBaris=" & CmbJumlahBarcodePerBaris.Text)
            sb.AppendLine("BarcodeFormatLabel=" & CmbTipeBarcode.Text)
            sb.AppendLine("LebarBarcode=" & TxtLebarBarcode.Text)
            sb.AppendLine("TinggiBarcode=" & TxtTinggiBarcode.Text)
            sb.AppendLine("BarcodeStartX=" & TxtStartX.Text)
            sb.AppendLine("BarcodeStartY=" & TxtStartY.Text)
            sb.AppendLine("BarcodeGapX=" & TxtGabX.Text)
            sb.AppendLine("BarcodeGapY=" & TxtGabY.Text)

            ' 🔹 Simpan file dengan encoding UTF-8
            File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8)
        Catch ex As Exception
            MessageBox.Show("Gagal menyimpan konfigurasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' 🔹 Ambil daftar printer yang terinstal
    Private Sub LoadPrinterKeComboBox()
        CmbJenisPrinter.Items.Clear()

        For Each printerName As String In PrinterSettings.InstalledPrinters
            CmbJenisPrinter.Items.Add(printerName)
        Next

        If CmbJenisPrinter.Items.Count > 0 Then
            CmbJenisPrinter.SelectedIndex = 0 ' Pilih printer pertama sebagai default
        End If
    End Sub

    Private Sub CmbJenisPrinter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbJenisPrinter.SelectedIndexChanged
        LoadKertasDariPrinter()
    End Sub

    ' 🔹 Ambil jenis kertas yang didukung oleh printer yang dipilih
    Private Sub LoadKertasDariPrinter()
        CmbJenisKertas.Items.Clear()

        If CmbJenisPrinter.SelectedItem IsNot Nothing Then
            Dim printerName As String = CmbJenisPrinter.SelectedItem.ToString()
            Dim printDoc As New PrintDocument()
            printDoc.PrinterSettings.PrinterName = printerName

            ' Cek apakah printer valid
            If printDoc.PrinterSettings.IsValid Then
                For Each paperSize As PaperSize In printDoc.PrinterSettings.PaperSizes
                    CmbJenisKertas.Items.Add(paperSize.PaperName)
                Next

                If CmbJenisKertas.Items.Count > 0 Then
                    CmbJenisKertas.SelectedIndex = 0 ' Pilih jenis kertas pertama
                End If
            End If
        End If
    End Sub


    Private Sub CmbJenisKertas_TextChanged(sender As Object, e As EventArgs) Handles CmbJenisKertas.TextChanged
        Dim jenisKertas As String = CmbJenisKertas.Text.Trim().ToUpper()

        If jenisKertas.StartsWith("A3") Then
            TxtTinggiKertas.Text = "420"
            TxtLebarKertas.Text = "297"
        ElseIf jenisKertas.StartsWith("A4") Then
            TxtTinggiKertas.Text = "297"
            TxtLebarKertas.Text = "210"
        ElseIf jenisKertas.StartsWith("A5") Then
            TxtTinggiKertas.Text = "210"
            TxtLebarKertas.Text = "148"
        ElseIf jenisKertas.StartsWith("A6") Then
            TxtTinggiKertas.Text = "148"
            TxtLebarKertas.Text = "105"
        ElseIf jenisKertas.StartsWith("B4") Then
            TxtTinggiKertas.Text = "353"
            TxtLebarKertas.Text = "250"
        ElseIf jenisKertas.StartsWith("B5") Then
            TxtTinggiKertas.Text = "250"
            TxtLebarKertas.Text = "176"
        ElseIf jenisKertas.StartsWith("C4") Then
            TxtTinggiKertas.Text = "324"
            TxtLebarKertas.Text = "229"
        ElseIf jenisKertas.StartsWith("C5") Then
            TxtTinggiKertas.Text = "229"
            TxtLebarKertas.Text = "162"
        ElseIf jenisKertas.StartsWith("LETTER") Then
            TxtTinggiKertas.Text = "279"
            TxtLebarKertas.Text = "216"
        ElseIf jenisKertas.StartsWith("LEGAL") Then
            TxtTinggiKertas.Text = "356"
            TxtLebarKertas.Text = "216"
        ElseIf jenisKertas.StartsWith("TABLOID") Then
            TxtTinggiKertas.Text = "432"
            TxtLebarKertas.Text = "279"
        ElseIf jenisKertas.StartsWith("F4") Then
            TxtTinggiKertas.Text = "330"
            TxtLebarKertas.Text = "210"
        ElseIf jenisKertas.StartsWith("KWARTO") Then
            TxtTinggiKertas.Text = "275"
            TxtLebarKertas.Text = "215"
        Else
            TxtTinggiKertas.Text = "297"
            TxtLebarKertas.Text = "210"
        End If
    End Sub


    Private Sub BarcodePerbaris()
        CmbJumlahBarcodePerBaris.Items.Clear()
        CmbTipeBarcode.Items.Clear()

        ' Mengisi pilihan untuk ComboBox Jumlah Barcode Per Baris
        CmbJumlahBarcodePerBaris.Items.Add("1 Barcode")
        CmbJumlahBarcodePerBaris.Items.Add("2 Barcode")
        CmbJumlahBarcodePerBaris.Items.Add("3 Barcode")
        CmbJumlahBarcodePerBaris.Items.Add("4 Barcode")

        ' Mengisi pilihan untuk ComboBox Tipe Barcode
        CmbTipeBarcode.Items.Add("QR Code")
        CmbTipeBarcode.Items.Add("Code 128")
        CmbTipeBarcode.Items.Add("Code 39")
        CmbTipeBarcode.Items.Add("EAN-13")
        CmbTipeBarcode.Items.Add("UPC-A")
        CmbJumlahBarcodePerBaris.SelectedIndex = 2 ' Pilihan default "2 Barcode"
        CmbTipeBarcode.SelectedIndex = 1 ' Pilihan default "Code 128"
    End Sub

    Private Sub BtnResetGap_Click(sender As Object, e As EventArgs) Handles BtnResetGap.Click
        ResetJarakLabel()
    End Sub

    Sub ResetJarakLabel()
        TxtGabX.Text = "5"
        TxtGabY.Text = "5"
    End Sub

    Private Sub BtnResetPosisiXY_Click(sender As Object, e As EventArgs) Handles BtnResetPosisiXY.Click
        ResetPosisiXY()
    End Sub

    Sub ResetPosisiXY()
        TxtStartX.Text = "10"
        TxtStartY.Text = "10"
    End Sub
    Private Sub BtnRestore_Click(sender As Object, e As EventArgs) Handles BtnRestore.Click

        ' Hapus file konfigurasi agar dibuat ulang dengan nilai default
        If File.Exists(configFilePath) Then
            File.Delete(configFilePath)
        End If

        ' Buat ulang file dengan nilai default
        BuatFileDefault()

        ' Muat ulang pengaturan dari file yang baru dibuat
        LoadPengaturan()

    End Sub


    Private Sub BtnPreviewCetak_Click(sender As Object, e As EventArgs) Handles BtnPreviewCetak.Click
        If TxtKodeBarcode.Text = "" Then
            MessageBox.Show("Kode Barcode tidak boleh kosong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtKodeBarcode.Focus()
            Exit Sub
        ElseIf TxtNamaBarang.Text = "" Then
            MessageBox.Show("Nama barang tidak boleh kosong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtNamaBarang.Focus()
            Exit Sub
        ElseIf TxtJumlahCetak.Text = "" Then
            MessageBox.Show("Jumlah cetak tidak boleh kosong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtJumlahCetak.Focus()
            Exit Sub
        Else
            PreviewBarcode()
        End If
    End Sub
    Sub PreviewBarcode()
        Try
            ' 1. Ambil data dari kontrol
            Dim namaToko As String = NAMA_PERUSAHAAN
            Dim namaBarang As String = TxtNamaBarang.Text
            Dim hargaBarang As String = "Rp " & CDbl(TxtHargaBarang.Text).ToString("N0", CultureInfo.CreateSpecificCulture("id-ID"))
            Dim satuanBarang As String = CmbSatuan.Text
            Dim nomorBarcode As String = TxtKodeBarcode.Text

            ' 2. Ambil pengaturan font dan ukuran
            Dim fontNama As New Font(CmbFontNama.Text, CInt(CmbUkuranNama.Text), If(ChkBoldNama.Checked, FontStyle.Bold, FontStyle.Regular))
            Dim fontHarga As New Font(CmbFontHarga.Text, CInt(CmbUkuranHarga.Text), If(ChkBoldHarga.Checked, FontStyle.Bold, FontStyle.Regular))
            Dim fontSatuan As New Font(CmbFontSatuan.Text, CInt(CmbUkuranSatuan.Text), If(ChkBoldSatuan.Checked, FontStyle.Bold, FontStyle.Regular))
            Dim fontToko As New Font(CmbFontToko.Text, CInt(CmbUkuranToko.Text), If(ChkBoldToko.Checked, FontStyle.Bold, FontStyle.Regular))

            ' 3. Ambil posisi dan jarak dari kontrol
            Dim startX As Integer = CInt(TxtStartX.Text) ' Posisi awal X
            Dim startY As Integer = CInt(TxtStartY.Text) ' Posisi awal Y
            Dim gabX As Integer = CInt(TxtGabX.Text) ' Jarak antar barcode horizontal
            Dim gabY As Integer = CInt(TxtGabY.Text) ' Jarak antar barcode vertikal

            ' 4. Buat generator barcode menggunakan ZXing.Net
            Dim writer As New BarcodeWriter()
            writer.Options = New EncodingOptions With {
            .Height = CInt(TxtTinggiBarcode.Text) + 20, ' Tambah tinggi barcode agar lebih besar
            .Width = CInt(TxtLebarBarcode.Text) + 40 ' Tambah lebar barcode agar lebih jelas
        }

            ' 5. Tentukan jenis barcode berdasarkan ComboBox
            Select Case CmbTipeBarcode.Text
                Case "QR Code"
                    writer.Format = BarcodeFormat.QR_CODE
                Case "Code 128"
                    writer.Format = BarcodeFormat.CODE_128
                Case "Code 39"
                    writer.Format = BarcodeFormat.CODE_39
                Case "EAN-13"
                    writer.Format = BarcodeFormat.EAN_13
                Case "UPC-A"
                    writer.Format = BarcodeFormat.UPC_A
                Case Else
                    writer.Format = BarcodeFormat.CODE_128
            End Select

            ' 6. Generate Barcode
            Dim barcodeImage As Image = writer.Write(nomorBarcode)

            ' 7. Tentukan ukuran gambar barcode + teks
            Dim width As Integer = barcodeImage.Width + 50 ' Tambah sedikit padding
            Dim height As Integer = barcodeImage.Height + 120 ' Tambah ruang untuk teks

            ' 8. Buat bitmap untuk menggambar barcode + teks
            Dim finalImage As New Bitmap(width, height)
            Using g As Graphics = Graphics.FromImage(finalImage)
                g.Clear(Color.White) ' Background putih

                ' 1. Gambar Nama Barang (Paling Atas)
                Dim barangSize As SizeF = g.MeasureString(namaBarang, fontNama)
                g.DrawString(namaBarang, fontNama, Brushes.Black, startX + 5, startY)

                ' 2. Gambar Barcode (Tengah)
                Dim barcodePosX As Integer = startX + 40 ' Sedikit ke kanan agar tidak terlalu rapat ke tepi
                Dim barcodePosY As Integer = startY + 15 ' Diberi jarak agar lebih rapi
                g.DrawImage(barcodeImage, barcodePosX, barcodePosY)

                ' 3. Gambar Harga (Di bawah Barcode)
                Dim hargaSize As SizeF = g.MeasureString(hargaBarang, fontHarga)
                g.DrawString(hargaBarang, fontHarga, Brushes.Black, barcodePosX + (barcodeImage.Width - hargaSize.Width) / 2, barcodePosY + barcodeImage.Height + 5)

                ' 4. Gambar Satuan (Di bawah Harga)
                Dim satuanSize As SizeF = g.MeasureString(satuanBarang, fontSatuan)
                g.DrawString(satuanBarang, fontSatuan, Brushes.Black, barcodePosX + (barcodeImage.Width - satuanSize.Width) / 2, barcodePosY + barcodeImage.Height + hargaSize.Height + 10)

                ' 5. Gambar Nama Toko (Vertikal di Sebelah Kiri)
                Dim oldTransform As Drawing2D.Matrix = g.Transform
                g.TranslateTransform(startX + 10, startY + 10) ' Posisi di sebelah kiri
                g.RotateTransform(90) ' Putar teks 90 derajat ke bawah
                g.DrawString(namaToko, fontToko, Brushes.Black, 0, 0)
                g.Transform = oldTransform ' Kembalikan transformasi

            End Using

            ' 9. Kosongkan semua PictureBox sebelum mengisi
            Dim previewBoxes As PictureBox() = {PicPreviewBarcode0, PicPreviewBarcode1, PicPreviewBarcode2,
                                            PicPreviewBarcode3}

            For Each pb As PictureBox In previewBoxes
                pb.Image = Nothing
            Next

            ' 10. Tampilkan barcode sesuai jumlah baris
            Dim jumlahBaris As Integer = CInt(CmbJumlahBarcodePerBaris.Text(0).ToString())
            Dim indeks As Integer()

            Select Case jumlahBaris
                Case 1
                    indeks = {0}
                Case 2
                    indeks = {0, 1}
                Case 3
                    indeks = {0, 1, 2}
                Case Else
                    indeks = {0, 1, 2, 3}
            End Select

            ' 11. Atur posisi sesuai jumlah barcode per baris
            Dim rowIndex As Integer = 0
            Dim colIndex As Integer = 0

            For Each i As Integer In indeks
                If i < previewBoxes.Length Then
                    ' Hitung posisi berdasarkan startX, startY, gabX, dan gabY
                    Dim posX As Integer = startX + (colIndex * (width + gabX))
                    Dim posY As Integer = startY + (rowIndex * (height + gabY))

                    ' Set gambar pada PictureBox
                    Dim tempBitmap As New Bitmap(finalImage)
                    Using g As Graphics = Graphics.FromImage(tempBitmap)
                        g.DrawImage(finalImage, posX, posY)
                    End Using
                    previewBoxes(i).Image = tempBitmap

                    ' Update indeks kolom & baris
                    colIndex += 1
                    If colIndex >= jumlahBaris Then
                        colIndex = 0
                        rowIndex += 1
                    End If
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat menampilkan barcode: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnExportBarcode_Click(sender As Object, e As EventArgs) Handles BtnExportBarcode.Click

    End Sub

    ' ====== 5. Export Barcode ke File ======


    Private Sub BtnCetakBarcode_Click(sender As Object, e As EventArgs) Handles BtnCetakBarcode.Click
        CetakBarcode()
    End Sub
    Private WithEvents PrintDoc As New PrintDocument()

    Sub CetakBarcode()
        Try
            ' Atur DPI agar ukuran sesuai
            PrintDoc.DefaultPageSettings.PrinterResolution.Kind = PrinterResolutionKind.High

            ' Atur ukuran kertas sesuai label printer (misalnya 50x30 mm)
            PrintDoc.DefaultPageSettings.PaperSize = New PaperSize("Label", 200, 120) ' Satuan: 1/100 inci (200 = 2 inci, 120 = 1.2 inci)

            ' Cetak
            PrintDoc.Print()
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat mencetak: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PrintDoc_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDoc.PrintPage
        Try
            Dim g As Graphics = e.Graphics
            g.PageUnit = GraphicsUnit.Pixel
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit

            ' Ambil data dari kontrol
            Dim namaToko As String = NAMA_PERUSAHAAN
            Dim namaBarang As String = TxtNamaBarang.Text
            Dim hargaBarang As String = "Rp " & CDbl(TxtHargaBarang.Text).ToString("N0", CultureInfo.CreateSpecificCulture("id-ID"))
            Dim satuanBarang As String = CmbSatuan.Text
            Dim nomorBarcode As String = TxtKodeBarcode.Text

            ' Ambil pengaturan font dan ukuran
            Dim fontNama As New Font(CmbFontNama.Text, CInt(CmbUkuranNama.Text), If(ChkBoldNama.Checked, FontStyle.Bold, FontStyle.Regular))
            Dim fontHarga As New Font(CmbFontHarga.Text, CInt(CmbUkuranHarga.Text), If(ChkBoldHarga.Checked, FontStyle.Bold, FontStyle.Regular))
            Dim fontSatuan As New Font(CmbFontSatuan.Text, CInt(CmbUkuranSatuan.Text), If(ChkBoldSatuan.Checked, FontStyle.Bold, FontStyle.Regular))
            Dim fontToko As New Font(CmbFontToko.Text, CInt(CmbUkuranToko.Text), If(ChkBoldToko.Checked, FontStyle.Bold, FontStyle.Regular))

            ' Ambil posisi dan jarak dari kontrol
            Dim startX As Integer = CInt(TxtStartX.Text) ' Posisi awal X
            Dim startY As Integer = CInt(TxtStartY.Text) ' Posisi awal Y
            Dim gabX As Integer = CInt(TxtGabX.Text) ' Jarak antar barcode horizontal
            Dim gabY As Integer = CInt(TxtGabY.Text) ' Jarak antar barcode vertikal

            ' Buat generator barcode
            Dim writer As New BarcodeWriter()
            writer.Options = New EncodingOptions With {
            .Height = CInt(TxtTinggiBarcode.Text) + 20,
            .Width = CInt(TxtLebarBarcode.Text) + 40
        }

            ' Tentukan jenis barcode
            Select Case CmbTipeBarcode.Text
                Case "QR Code"
                    writer.Format = BarcodeFormat.QR_CODE
                Case "Code 128"
                    writer.Format = BarcodeFormat.CODE_128
                Case "Code 39"
                    writer.Format = BarcodeFormat.CODE_39
                Case "EAN-13"
                    writer.Format = BarcodeFormat.EAN_13
                Case "UPC-A"
                    writer.Format = BarcodeFormat.UPC_A
                Case Else
                    writer.Format = BarcodeFormat.CODE_128
            End Select

            ' Generate barcode
            Dim barcodeImage As Image = writer.Write(nomorBarcode)

            ' Tentukan ukuran cetak barcode + teks
            Dim width As Integer = barcodeImage.Width + 50
            Dim height As Integer = barcodeImage.Height + 120

            ' Posisi cetak
            Dim posX As Integer = startX
            Dim posY As Integer = startY

            ' Gambar Nama Barang (Paling Atas)
            Dim barangSize As SizeF = g.MeasureString(namaBarang, fontNama)
            g.DrawString(namaBarang, fontNama, Brushes.Black, posX + 5, posY)

            ' Gambar Barcode (Tengah)
            Dim barcodePosX As Integer = posX + 40
            Dim barcodePosY As Integer = posY + 15
            g.DrawImage(barcodeImage, barcodePosX, barcodePosY)

            ' Gambar Harga (Di bawah Barcode)
            Dim hargaSize As SizeF = g.MeasureString(hargaBarang, fontHarga)
            g.DrawString(hargaBarang, fontHarga, Brushes.Black, barcodePosX + (barcodeImage.Width - hargaSize.Width) / 2, barcodePosY + barcodeImage.Height + 5)

            ' Gambar Satuan (Di bawah Harga)
            Dim satuanSize As SizeF = g.MeasureString(satuanBarang, fontSatuan)
            g.DrawString(satuanBarang, fontSatuan, Brushes.Black, barcodePosX + (barcodeImage.Width - satuanSize.Width) / 2, barcodePosY + barcodeImage.Height + hargaSize.Height + 10)

            ' Gambar Nama Toko (Vertikal di Sebelah Kiri)
            Dim oldTransform As Drawing2D.Matrix = g.Transform
            g.TranslateTransform(posX + 10, posY + 10)
            g.RotateTransform(90)
            g.DrawString(namaToko, fontToko, Brushes.Black, 0, 0)
            g.Transform = oldTransform ' Kembalikan transformasi

            ' Periksa apakah ada lebih banyak halaman untuk dicetak
            e.HasMorePages = False

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat mencetak: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub


    Private Sub TxtNamaBarang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNamaBarang.TextChanged
        If Not String.IsNullOrWhiteSpace(TxtNamaBarang.Text) Then
            TampilkanDaftarBarang(TxtNamaBarang.Text)
        Else
            LstBarang.Visible = False
        End If
    End Sub

    Private Sub TxtNamaBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNamaBarang.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                If LstBarang.Items.Count = 1 Then
                    Ambildatalaindaridbbarang(LstBarang.Items(0).ToString())
                ElseIf LstBarang.Items.Count > 0 Then
                    LstBarang.Focus()
                    LstBarang.SelectedIndex = 0
                    e.SuppressKeyPress = True
                End If
            Case Keys.Down
                If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
                    LstBarang.Focus()
                    LstBarang.SelectedIndex = 0
                    e.SuppressKeyPress = True
                End If
            Case Keys.Tab
                TxtJumlahCetak.Focus()
        End Select
    End Sub

    Private Sub TampilkanDaftarBarang(ByVal searchKeyword As String)
        Dim query As String = "SELECT NAMA_BARANG FROM tbl_barang " &
                              "WHERE TRIM(ID_BARANG) LIKE @Nama OR TRIM(NAMA_BARANG) LIKE @Nama " &
                              "OR TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(BARCODE_SEDANG) LIKE @Nama " &
                              "OR TRIM(BARCODE_BESAR) LIKE @Nama ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & searchKeyword & "%")

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                LstBarang.Items.Clear()

                While rd.Read()
                    LstBarang.Items.Add(rd("NAMA_BARANG").ToString())
                End While

                LstBarang.Visible = LstBarang.Items.Count > 0
            End Using
        End Using
    End Sub

    Private Sub LstBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles LstBarang.KeyDown
        If e.KeyCode = Keys.Enter AndAlso LstBarang.SelectedItem IsNot Nothing Then
            Ambildatalaindaridbbarang(LstBarang.SelectedItem.ToString())
            LstBarang.Visible = False
        End If
    End Sub

    Private Sub LstBarang_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LstBarang.MouseClick
        If LstBarang.SelectedItem IsNot Nothing Then
            Ambildatalaindaridbbarang(LstBarang.SelectedItem.ToString())
            LstBarang.Visible = False
        End If
    End Sub

    Dim barcodekecil As String
    Dim Barcodesedang As String
    Dim Barcodebesar As String

    Dim hargajualkecil As Decimal
    Dim hargajualsedang As Decimal
    Dim hargajualbesar As Decimal

    Private Sub Ambildatalaindaridbbarang(ByVal namayangdiambil As String)
        Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                            "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                            "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR " &
                            "FROM tbl_barang WHERE TRIM(NAMA_BARANG) LIKE @NamaBarang OR " &
                            "BARCODE_KECIL LIKE @NamaBarang OR BARCODE_SEDANG LIKE @NamaBarang OR " &
                            "BARCODE_BESAR LIKE @NamaBarang"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NamaBarang", "%" & namayangdiambil.Trim() & "%")

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    With rd
                        TxtKode.Text = If(Not .IsDBNull(0), .GetString(0), "")
                        CmbSatuan.Items.Clear()
                        Dim satuanKecil As String = If(Not .IsDBNull(5), .GetString(5), "")
                        Dim satuanSedang As String = If(Not .IsDBNull(6), .GetString(6), "")
                        Dim satuanBesar As String = If(Not .IsDBNull(7), .GetString(7), "")

                        If Not String.IsNullOrEmpty(satuanKecil) Then CmbSatuan.Items.Add(satuanKecil)
                        If Not String.IsNullOrEmpty(satuanSedang) Then CmbSatuan.Items.Add(satuanSedang)
                        If Not String.IsNullOrEmpty(satuanBesar) Then CmbSatuan.Items.Add(satuanBesar)

                        ' Default ke ukuran kecil
                        hargajualkecil = If(Not .IsDBNull(8), .GetDecimal(8), 0)
                        barcodekecil = If(Not .IsDBNull(2), .GetString(2), "")
                        hargajualsedang = If(Not .IsDBNull(9), .GetDecimal(9), 0)
                        Barcodesedang = .GetString(3)
                        hargajualbesar = If(Not .IsDBNull(10), .GetDecimal(10), 0)
                        Barcodebesar = .GetString(4)


                        CmbSatuan.Text = satuanKecil

                    End With
                End If
            End Using
        End Using
        TxtNamaBarang.Text = namayangdiambil
        TxtJumlahCetak.Focus()
    End Sub

    Private Sub CmbSatuan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbSatuan.SelectedIndexChanged
        If CmbSatuan.SelectedIndex = 0 Then
            TxtHargaBarang.Text = hargajualkecil
            TxtKodeBarcode.Text = barcodekecil
        ElseIf CmbSatuan.SelectedIndex = 1 Then
            TxtHargaBarang.Text = hargajualsedang
            TxtKodeBarcode.Text = Barcodesedang
        ElseIf CmbSatuan.SelectedIndex = 2 Then
            TxtHargaBarang.Text = hargajualbesar
            TxtKodeBarcode.Text = Barcodebesar
        End If
    End Sub
End Class
