Imports System.Drawing.Printing
Imports System.Drawing.Text
Imports System.IO
Imports System.Text




Public Class FormCetakLabel
    Private configFilePath As String = Path.Combine(Application.StartupPath, "ConfigLabelBarang.ini")

    ' 🔹 Load konfigurasi saat form pertama kali dibuka
    Private Sub FormCetakLabel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        DGVLabel.Rows.Clear()
        LoadFontSizes()
        LoadPrinterKeComboBox()
        LoadFontKeComboBox()
        LoadDariINI()
    End Sub

    Private Sub DGVLabel_CellEndEdit(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DGVLabel.CellEndEdit
        If e.ColumnIndex <> DGVLabel.Columns("Nama").Index Then Exit Sub

        Dim row As DataGridViewRow = DGVLabel.Rows(e.RowIndex)
        Dim namaCellValue As Object = row.Cells("Nama").Value

        If namaCellValue Is Nothing OrElse String.IsNullOrWhiteSpace(namaCellValue.ToString()) Then Exit Sub

        Dim namaValue As String = namaCellValue.ToString().Trim()
        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, 
                        ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, 
                        HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, STOK_TOKO, STOK_GUDANG  
                        FROM tbl_barang 
                        WHERE TRIM(ID_BARANG) LIKE @NamaBarang OR TRIM(NAMA_BARANG) LIKE @NamaBarang 
                        OR TRIM(BARCODE_KECIL) LIKE @NamaBarang OR TRIM(BARCODE_SEDANG) LIKE @NamaBarang 
                        OR TRIM(BARCODE_BESAR) LIKE @NamaBarang"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@NamaBarang", "%" & namaValue & "%") ' Menambahkan wildcard % untuk LIKE

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    row.Cells("kode").Value = rd("ID_BARANG")
                    row.Cells("Nama").Value = rd("NAMA_BARANG")
                    row.Cells("Toko").Value = rd("STOK_TOKO")
                    row.Cells("Gudang").Value = rd("STOK_GUDANG")

                    ' Mengisi ComboBox "Satuan"
                    Dim comboCell As DataGridViewComboBoxCell = TryCast(row.Cells("Satuan"), DataGridViewComboBoxCell)
                    If comboCell IsNot Nothing Then
                        comboCell.Items.Clear()

                        Dim satuan() As String = {rd("SATUAN_UMUM_KECIL").ToString(), rd("SATUAN_UMUM_SEDANG").ToString(), rd("SATUAN_UMUM_BESAR").ToString()}
                        For Each item As String In satuan
                            If Not String.IsNullOrEmpty(item) Then comboCell.Items.Add(item)
                        Next
                    End If

                    ' Menentukan satuan, isi, dan harga
                    Dim satuanTerpilih As String = ""
                    Dim isi As Integer = 1
                    Dim harga As Decimal = 0

                    If namaValue = rd("NAMA_BARANG").ToString() OrElse namaValue = rd("BARCODE_KECIL").ToString() Then
                        satuanTerpilih = rd("SATUAN_UMUM_KECIL")
                        isi = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 0)
                        harga = ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_KECIL"))
                    ElseIf namaValue = rd("BARCODE_SEDANG").ToString() Then
                        satuanTerpilih = rd("SATUAN_UMUM_SEDANG")
                        isi = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 0)
                        harga = ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_SEDANG"))
                    ElseIf namaValue = rd("BARCODE_BESAR").ToString() Then
                        satuanTerpilih = rd("SATUAN_UMUM_BESAR")
                        isi = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 0)
                        harga = ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_BESAR"))
                    End If

                    row.Cells("Satuan").Value = satuanTerpilih
                    row.Cells("Isi").Value = isi
                    row.Cells("Harga").Value = harga
                End If
            End Using
        End Using
    End Sub

    Private Sub DGVLabel_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGVLabel.CellClick
        ' Pastikan indeks kolom valid dan bukan header
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' Periksa apakah kolom yang diklik adalah kolom "Hapus"
            If DGVLabel.Columns(e.ColumnIndex).Name = "Hapus" Then
                ' Pastikan baris tidak kosong sebelum menghapus
                Dim row As DataGridViewRow = DGVLabel.Rows(e.RowIndex)
                Dim isRowEmpty As Boolean = True

                ' Cek apakah ada data di baris tersebut
                For Each cell As DataGridViewCell In row.Cells
                    If cell.Value IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cell.Value.ToString()) Then
                        isRowEmpty = False
                        Exit For
                    End If
                Next

                ' Jika baris tidak kosong, konfirmasi penghapusan
                If Not isRowEmpty Then
                    If MessageBox.Show("Apakah Anda yakin ingin menghapus baris ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        DGVLabel.Rows.RemoveAt(e.RowIndex)
                    End If
                Else
                    MessageBox.Show("Baris ini kosong dan tidak bisa dihapus.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        End If
    End Sub


    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox As ComboBox = DirectCast(sender, ComboBox)
        Dim row As DataGridViewRow = DGVLabel.CurrentRow
        If row Is Nothing Then Exit Sub

        Dim selectedItemId As String = row.Cells("kode").Value.ToString()

        Dim query As String = "SELECT ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, 
                           HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR 
                           FROM tbl_barang WHERE ID_BARANG = @ItemId"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@ItemId", selectedItemId)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Select Case comboBox.SelectedIndex
                        Case 0
                            row.Cells("Isi").Value = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 0)
                            row.Cells("Harga").Value = ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_KECIL"))
                        Case 1
                            row.Cells("Isi").Value = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 0)
                            row.Cells("Harga").Value = ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_SEDANG"))
                        Case Else
                            row.Cells("Isi").Value = ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 0)
                            row.Cells("Harga").Value = ModuleAngka.ParseDecimal(rd("HARGA_JUAL_UMUM_BESAR"))
                    End Select
                Else
                    MessageBox.Show("Satuan barang dan atau harga jual belum diinput!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End Using
        End Using
    End Sub

    Private Sub DGVLabel_EditingControlShowing(ByVal sender As Object, ByVal e As DataGridViewEditingControlShowingEventArgs) Handles DGVLabel.EditingControlShowing
        Dim colIndex As Integer = DGVLabel.CurrentCell.ColumnIndex
        Dim control As Control = e.Control

        If colIndex = DGVLabel.Columns("Nama").Index Then
            Dim autoText As TextBox = TryCast(control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.Suggest
                autoText.AutoCompleteSource = AutoCompleteSource.CustomSource
                Dim dataCollection As New AutoCompleteStringCollection()
                AddItems(dataCollection, autoText.Text.Trim())
                autoText.AutoCompleteCustomSource = dataCollection
            End If
        ElseIf colIndex = DGVLabel.Columns("Satuan").Index Then
            Dim comboBox As ComboBox = TryCast(control, ComboBox)
            If comboBox IsNot Nothing Then
                ' Pastikan kolom benar-benar merupakan DataGridViewComboBoxCell
                Dim comboCell As DataGridViewComboBoxCell = TryCast(DGVLabel.CurrentCell, DataGridViewComboBoxCell)
                If comboCell IsNot Nothing Then
                    ' Hapus event handler lama untuk mencegah duplikasi event
                    RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
                    ' Tambahkan event handler baru
                    AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
                End If
            End If
        End If
    End Sub

    Private Sub AddItems(ByVal col As AutoCompleteStringCollection, ByVal namaValue As String)
        Dim query As String = "SELECT NAMA_BARANG FROM tbl_barang WHERE NAMA_BARANG LIKE @Nama ORDER BY NAMA_BARANG"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & namaValue & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    col.Add(rd("NAMA_BARANG").ToString())
                End While
            End Using
        End Using
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

                ' 🔹 Konfigurasi Ukuran Kertas
                sb.AppendLine("[UkuranKertas]")
                sb.AppendLine("JenisPrinter=") ' Biarkan kosong untuk konfigurasi manual
                sb.AppendLine("JenisKertas=A4")
                sb.AppendLine("TinggiKertas=297") ' Dalam mm
                sb.AppendLine("LebarKertas=210") ' Dalam mm
                sb.AppendLine("BatasAtas=2") ' Margin atas dalam mm
                sb.AppendLine("BatasKiri=2") ' Margin kiri dalam mm

                ' 🔹 Konfigurasi Label
                sb.AppendLine()
                sb.AppendLine("[Label]")
                sb.AppendLine("JumlahPerBaris=3") ' Default 3 label per baris
                sb.AppendLine("JarakX=2") ' Jarak antar label dalam mm
                sb.AppendLine("JarakY=2") ' Jarak vertikal antar label dalam mm
                sb.AppendLine("TinggiLabel=25") ' Tinggi label dalam mm

                ' 🔹 Konfigurasi Format
                sb.AppendLine()
                sb.AppendLine("[Format]")
                sb.AppendLine("FormatLabel=2") ' Konfigurasi pilihan Format label

                ' 🔹 Konfigurasi Font
                sb.AppendLine()
                sb.AppendLine("[Font]")
                sb.AppendLine("WarnaNama=Black") ' Konfigurasi Font Nama Barang
                sb.AppendLine("FontNama=Arial")
                sb.AppendLine("UkuranNama=12")
                sb.AppendLine("BoldNama=False")
                sb.AppendLine("WarnaHarga=Black") ' Konfigurasi Font Harga
                sb.AppendLine("FontHarga=Tahoma")
                sb.AppendLine("UkuranHarga=14")
                sb.AppendLine("BoldHarga=True")
                sb.AppendLine("WarnaSatuan=Black") ' Konfigurasi Font Satuan
                sb.AppendLine("FontSatuan=Tahoma")
                sb.AppendLine("UkuranSatuan=10")
                sb.AppendLine("BoldSatuan=False")
                sb.AppendLine("WarnaToko=Black") ' Konfigurasi Font Nama Toko
                sb.AppendLine("FontToko=Verdana")
                sb.AppendLine("UkuranToko=10")
                sb.AppendLine("BoldToko=False")

                ' Tulis ke file dengan encoding UTF-8
                File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8)
            End If

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat membuat file konfigurasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' 🔹 Fungsi untuk menyimpan nilai kontrol ke file INI
    Private Sub SimpanKeINI()
        Try
            Dim sb As New StringBuilder()

            ' 🔹 Konfigurasi Ukuran Kertas
            sb.AppendLine("[UkuranKertas]")
            sb.AppendLine("JenisPrinter=" & CmbJenisPrinter.Text)
            sb.AppendLine("JenisKertas=" & CmbJenisKertas.Text)
            sb.AppendLine("TinggiKertas=" & TxtPanjangKertas.Text.Replace(",", "."))
            sb.AppendLine("LebarKertas=" & TxtLebarKertas.Text.Replace(",", "."))
            sb.AppendLine("BatasAtas=" & TxtBatasAtas.Text.Replace(",", "."))
            sb.AppendLine("BatasKiri=" & TxtbatasKiri.Text.Replace(",", "."))

            ' 🔹 Konfigurasi Label
            sb.AppendLine()
            sb.AppendLine("[Label]")
            sb.AppendLine("JumlahPerBaris=" & TxtJumlahPerBaris.Text)
            sb.AppendLine("JarakX=" & TxtJarakX.Text.Replace(",", "."))
            sb.AppendLine("JarakY=" & TxtJarakY.Text.Replace(",", "."))
            sb.AppendLine("TinggiLabel=" & TxtTinggiLabel.Text.Replace(",", "."))


            ' 🔹 Konfigurasi Format
            sb.AppendLine()
            sb.AppendLine("[Format]")
            sb.AppendLine("FormatLabel=" & CmbBentuklabel.SelectedIndex)



            ' 🔹 Konfigurasi Font
            sb.AppendLine()
            sb.AppendLine("[Font]")

            ' 🔹 Simpan Warna dengan Nama jika tersedia, atau pakai nilai ARGB
            'sb.AppendLine("WarnaNama=" & GetColorString(KryptonColorBNama.SelectedColor))
            sb.AppendLine("FontNama=" & CmbFontNama.Text)
            sb.AppendLine("UkuranNama=" & CmbUkuranNama.Text)
            sb.AppendLine("BoldNama=" & ChkBoldNama.Checked)

            'sb.AppendLine("WarnaHarga=" & GetColorString(KryptonColorBHarga.SelectedColor))
            sb.AppendLine("FontHarga=" & CmbFontHarga.Text)
            sb.AppendLine("UkuranHarga=" & CmbUkuranHarga.Text)
            sb.AppendLine("BoldHarga=" & ChkBoldHarga.Checked)

            'sb.AppendLine("WarnaSatuan=" & GetColorString(KryptonColorBSatuan.SelectedColor))
            sb.AppendLine("FontSatuan=" & CmbFontSatuan.Text)
            sb.AppendLine("UkuranSatuan=" & CmbUkuranSatuan.Text)
            sb.AppendLine("BoldSatuan=" & ChkBoldSatuan.Checked)

            'sb.AppendLine("WarnaToko=" & GetColorString(KryptonColorBToko.SelectedColor))
            sb.AppendLine("FontToko=" & CmbFontToko.Text)
            sb.AppendLine("UkuranToko=" & CmbUkuranToko.Text)
            sb.AppendLine("BoldToko=" & ChkBoldToko.Checked)

            ' Simpan file dengan encoding UTF-8
            File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8)

        Catch ex As Exception
            MessageBox.Show("Gagal menyimpan konfigurasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' 🔹 Fungsi untuk mendapatkan format warna yang lebih mudah dibaca
    Private Function GetColorString(color As Color) As String
        Return If(color.IsNamedColor, color.Name, color.ToArgb().ToString())
    End Function


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
            TxtPanjangKertas.Text = "420"
            TxtLebarKertas.Text = "297"
        ElseIf jenisKertas.StartsWith("A4") Then
            TxtPanjangKertas.Text = "297"
            TxtLebarKertas.Text = "210"
        ElseIf jenisKertas.StartsWith("A5") Then
            TxtPanjangKertas.Text = "210"
            TxtLebarKertas.Text = "148"
        ElseIf jenisKertas.StartsWith("A6") Then
            TxtPanjangKertas.Text = "148"
            TxtLebarKertas.Text = "105"
        ElseIf jenisKertas.StartsWith("B4") Then
            TxtPanjangKertas.Text = "353"
            TxtLebarKertas.Text = "250"
        ElseIf jenisKertas.StartsWith("B5") Then
            TxtPanjangKertas.Text = "250"
            TxtLebarKertas.Text = "176"
        ElseIf jenisKertas.StartsWith("C4") Then
            TxtPanjangKertas.Text = "324"
            TxtLebarKertas.Text = "229"
        ElseIf jenisKertas.StartsWith("C5") Then
            TxtPanjangKertas.Text = "229"
            TxtLebarKertas.Text = "162"
        ElseIf jenisKertas.StartsWith("LETTER") Then
            TxtPanjangKertas.Text = "279"
            TxtLebarKertas.Text = "216"
        ElseIf jenisKertas.StartsWith("LEGAL") Then
            TxtPanjangKertas.Text = "356"
            TxtLebarKertas.Text = "216"
        ElseIf jenisKertas.StartsWith("TABLOID") Then
            TxtPanjangKertas.Text = "432"
            TxtLebarKertas.Text = "279"
        ElseIf jenisKertas.StartsWith("F4") Then
            TxtPanjangKertas.Text = "330"
            TxtLebarKertas.Text = "210"
        ElseIf jenisKertas.StartsWith("KWARTO") Then
            TxtPanjangKertas.Text = "275"
            TxtLebarKertas.Text = "215"
        Else
            TxtPanjangKertas.Text = "297"
            TxtLebarKertas.Text = "210"
        End If
    End Sub


    ' 🔹 Muat konfigurasi dari file INI
    Private Sub LoadDariINI()
        Try
            BuatFileDefault() ' Pastikan file ada sebelum dibaca

            Dim lines() As String = File.ReadAllLines(configFilePath)
            Dim section As String = ""

            For Each line As String In lines
                line = line.Trim()
                If line = "" Then Continue For ' Lewati baris kosong

                If line.StartsWith("[") And line.EndsWith("]") Then
                    ' Simpan nama bagian (section) yang sedang diproses
                    section = line.Trim("["c, "]"c)
                Else
                    Dim index As Integer = line.IndexOf("="c)
                    If index > 0 Then
                        Dim key As String = line.Substring(0, index).Trim()
                        Dim value As String = line.Substring(index + 1).Trim()

                        Select Case section
                            Case "UkuranKertas"
                                Select Case key
                                    Case "JenisPrinter" : CmbJenisPrinter.Text = value
                                    Case "JenisKertas" : CmbJenisKertas.Text = value
                                    Case "TinggiKertas" : TxtPanjangKertas.Text = SafeParseDecimal(value)
                                    Case "LebarKertas" : TxtLebarKertas.Text = SafeParseDecimal(value)
                                    Case "BatasAtas" : TxtBatasAtas.Text = SafeParseDecimal(value)
                                    Case "BatasKiri" : TxtbatasKiri.Text = SafeParseDecimal(value)
                                End Select

                            Case "Label"
                                Select Case key
                                    Case "JumlahPerBaris" : TxtJumlahPerBaris.Text = SafeParseInt(value)
                                    Case "JarakX" : TxtJarakX.Text = SafeParseDecimal(value)
                                    Case "JarakY" : TxtJarakY.Text = SafeParseDecimal(value)
                                    Case "TinggiLabel" : TxtTinggiLabel.Text = SafeParseDecimal(value)
                                End Select


                            Case "Format"
                                Select Case key
                                    Case "FormatLabel" : CmbBentuklabel.SelectedIndex = SafeParseInt(value)
                                End Select

                            Case "Font"
                                Select Case key
                                    'Case "WarnaNama" : KryptonColorBNama.SelectedColor = SafeParseColor(value)
                                    Case "FontNama" : CmbFontNama.Text = value
                                    Case "UkuranNama" : CmbUkuranNama.Text = SafeParseInt(value)
                                    Case "BoldNama" : ChkBoldNama.Checked = SafeParseBool(value)

                                    'Case "WarnaHarga" : KryptonColorBHarga.SelectedColor = SafeParseColor(value)
                                    Case "FontHarga" : CmbFontHarga.Text = value
                                    Case "UkuranHarga" : CmbUkuranHarga.Text = SafeParseInt(value)
                                    Case "BoldHarga" : ChkBoldHarga.Checked = SafeParseBool(value)

                                    'Case "WarnaSatuan" : KryptonColorBSatuan.SelectedColor = SafeParseColor(value)
                                    Case "FontSatuan" : CmbFontSatuan.Text = value
                                    Case "UkuranSatuan" : CmbUkuranSatuan.Text = SafeParseInt(value)
                                    Case "BoldSatuan" : ChkBoldSatuan.Checked = SafeParseBool(value)

                                    'Case "WarnaToko" : KryptonColorBToko.SelectedColor = SafeParseColor(value)
                                    Case "FontToko" : CmbFontToko.Text = value
                                    Case "UkuranToko" : CmbUkuranToko.Text = SafeParseInt(value)
                                    Case "BoldToko" : ChkBoldToko.Checked = SafeParseBool(value)
                                End Select
                        End Select
                    End If
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Gagal membaca konfigurasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' 🔹 Fungsi untuk mengonversi warna dari Nama atau ARGB
    Private Function SafeParseColor(value As String) As Color
        Try
            If Integer.TryParse(value, Nothing) Then
                Return Color.FromArgb(Integer.Parse(value))
            Else
                Return Color.FromName(value)
            End If
        Catch
            Return Color.Black ' Default jika gagal
        End Try
    End Function

    ' 🔹 Fungsi untuk mengonversi nilai ke Integer dengan aman
    Private Function SafeParseInt(value As String) As Integer
        Dim result As Integer
        If Integer.TryParse(value, result) Then
            Return result
        End If
        Return 0 ' Default jika gagal
    End Function

    ' 🔹 Fungsi untuk mengonversi nilai ke Decimal dengan aman
    Private Function SafeParseDecimal(value As String) As String
        Dim result As Decimal
        If Decimal.TryParse(value, result) Then
            Return result.ToString().Replace(",", ".") ' Gunakan format desimal standar
        End If
        Return "0"
    End Function

    ' 🔹 Fungsi untuk mengonversi nilai ke Boolean dengan aman
    Private Function SafeParseBool(value As String) As Boolean
        Return value.Trim().ToLower() = "true"
    End Function



    ' Variabel untuk menyimpan warna dari KryptonColorButton
    Private WarnaNama As Color = Color.Black
    Private WarnaHarga As Color = Color.Black
    Private WarnaSatuan As Color = Color.Black
    Private WarnaToko As Color = Color.Black

    '' Event untuk mengubah warna teks Nama
    'Private Sub KryptonColorBNama_SelectedColorChanged(sender As Object, e As EventArgs) Handles KryptonColorBNama.SelectedColorChanged
    '    WarnaNama = KryptonColorBNama.SelectedColor
    '    UpdateButtonColor(KryptonColorBNama, WarnaNama)
    'End Sub

    '' Event untuk mengubah warna teks Harga
    'Private Sub KryptonColorBHarga_SelectedColorChanged(sender As Object, e As EventArgs) Handles KryptonColorBHarga.SelectedColorChanged
    '    WarnaHarga = KryptonColorBHarga.SelectedColor
    '    UpdateButtonColor(KryptonColorBHarga, WarnaHarga)
    'End Sub

    '' Event untuk mengubah warna teks Satuan
    'Private Sub KryptonColorBSatuan_SelectedColorChanged(sender As Object, e As EventArgs) Handles KryptonColorBSatuan.SelectedColorChanged
    '    WarnaSatuan = KryptonColorBSatuan.SelectedColor
    '    UpdateButtonColor(KryptonColorBSatuan, WarnaSatuan)
    'End Sub

    '' Event untuk mengubah warna teks Toko
    'Private Sub KryptonColorBToko_SelectedColorChanged(sender As Object, e As EventArgs) Handles KryptonColorBToko.SelectedColorChanged
    '    WarnaToko = KryptonColorBToko.SelectedColor
    '    UpdateButtonColor(KryptonColorBToko, WarnaToko)
    'End Sub

    '' Fungsi untuk memperbarui warna teks tombol tanpa mengubah backcolor
    'Private Sub UpdateButtonColor(btn As KryptonColorButton, warna As Color)
    '    ' Mengubah warna teks tombol berdasarkan warna yang dipilih
    '    btn.StateNormal.Content.ShortText.Color1 = warna
    '    btn.OverrideDefault.Content.ShortText.Color1 = warna
    '    btn.StatePressed.Content.ShortText.Color1 = warna
    '    btn.StateTracking.Content.ShortText.Color1 = warna
    'End Sub


    ' 🔹 Simpan konfigurasi saat tombol ditekan
    Private Sub BtnSimpanPerubahan_Click(sender As Object, e As EventArgs) Handles btnSimpanPerubahan.Click
        SimpanKeINI()
        MessageBox.Show("Konfigurasi berhasil disimpan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BtnCetak_Click(sender As Object, e As EventArgs) Handles BtnCetak.Click
        SetupPrint(False) ' Langsung cetak
    End Sub

    ' Tombol Print Preview
    Private Sub BtnPreview_Click(sender As Object, e As EventArgs) Handles BtnPreview.Click
        SetupPrint(True) ' Pratinjau sebelum cetak
    End Sub


    '--- Deklarasi Variabel Kelas ---
    Private lebarKertas As Integer       ' Lebar kertas dalam mm
    Private panjangKertas As Integer     ' Panjang kertas dalam mm
    Private jumlahPerBaris As Integer    ' Jumlah label per baris
    Private jarakX As Integer            ' Jarak horizontal antar label (mm)
    Private jarakY As Integer            ' Jarak vertikal antar label (mm)
    Private marginAtas As Integer        ' Margin atas dalam mm         
    Private marginKiri As Integer        ' Margin kiri dalam mm
    Private tinggiLabel As Integer       ' Tinggi label dalam mm
    Private daftarData As New List(Of String()) ' Data label
    Private totalRows As Integer         ' Total data
    Private currentRow As Integer        ' Indeks data saat ini

    Private WithEvents PD As New PrintDocument()
    Private PPD As New PrintPreviewDialog()

    '--- Fungsi Konversi mm ke Pixel dengan DPI 100 ---
    Private Function MmToPixel(mm As Single) As Single
        Return (mm / 25.4F) * 100 ' Konversi mm ke pixel dengan DPI 100
    End Function


    '--- Persiapan Cetak ---
    Private Sub SetupPrint(preview As Boolean)
        ' Tentukan printer yang dipilih
        If Not String.IsNullOrEmpty(CmbJenisPrinter.Text) Then
            PD.PrinterSettings.PrinterName = CmbJenisPrinter.Text
        End If

        ' Validasi input menggunakan TryParse
        Integer.TryParse(TxtJumlahPerBaris.Text, jumlahPerBaris)
        Integer.TryParse(TxtJarakX.Text, jarakX)
        Integer.TryParse(TxtJarakY.Text, jarakY)
        Integer.TryParse(TxtBatasAtas.Text, marginAtas)
        Integer.TryParse(TxtbatasKiri.Text, marginKiri)
        Integer.TryParse(TxtTinggiLabel.Text, tinggiLabel)
        Integer.TryParse(TxtLebarKertas.Text, lebarKertas)
        Integer.TryParse(TxtPanjangKertas.Text, panjangKertas)

        ' Ambil data dari DataGridView
        daftarData.Clear()
        For Each row As DataGridViewRow In DGVLabel.Rows
            If Not row.IsNewRow Then
                Dim namaBarang = If(row.Cells(1).Value?.ToString(), "").Trim()
                Dim harga = If(row.Cells(4).Value?.ToString(), "0").Trim()
                Dim satuan = If(row.Cells(2).Value?.ToString(), "").Trim()
                daftarData.Add({namaBarang, harga, satuan})
            End If
        Next
        totalRows = daftarData.Count
        currentRow = 0

        ' Konversi ukuran kertas ke pixel
        Dim lebarKertasPx As Integer = CInt(MmToPixel(lebarKertas))
        Dim panjangKertasPx As Integer = CInt(MmToPixel(panjangKertas))

        ' Set ukuran kertas custom
        PD.DefaultPageSettings.PaperSize = New PaperSize("Custom", lebarKertasPx, panjangKertasPx)

        ' Cek printer valid sebelum mencetak
        If Not PD.PrinterSettings.IsValid Then
            MessageBox.Show("Printer yang dipilih tidak valid atau tidak tersedia.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ' Pilih preview atau langsung cetak
        If preview Then
            PPD.Document = PD
            PPD.ShowDialog()
        Else
            PD.Print()
        End If
    End Sub


    '--- 🔹 Proses Cetak Halaman 🔹 ---'
    Private Sub PD_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PD.PrintPage
        Dim g As Graphics = e.Graphics

        ' 🔹 Konversi SEMUA nilai dari mm ke pixel (DPI 100) SEKALI DI AWAL
        Dim marginKiriPx As Single = MmToPixel(marginKiri)
        Dim marginAtasPx As Single = MmToPixel(marginAtas)
        Dim jarakXPx As Single = MmToPixel(jarakX)
        Dim jarakYPx As Single = MmToPixel(jarakY)
        Dim tinggiLabelPx As Single = MmToPixel(tinggiLabel)
        Dim lebarKertasPx As Single = MmToPixel(lebarKertas)
        Dim panjangKertasPx As Single = MmToPixel(panjangKertas)

        ' 🔹 Hitung total area efektif untuk label (dikurangi margin dan jarak antar label)
        Dim lebarAreaEfektif As Single = lebarKertasPx - (2 * marginKiriPx) - ((jumlahPerBaris - 1) * jarakXPx)

        ' 🔹 Hitung lebar label dengan satuan yang sama (pixel)
        Dim lebarLabelPx As Single = lebarAreaEfektif / jumlahPerBaris

        ' 🔹 Validasi jika lebar label tidak valid
        If lebarLabelPx <= 0 Then
            MessageBox.Show("Lebar label tidak valid! Silakan periksa jumlah per baris atau jarak antar label.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            e.Cancel = True
            Return
        End If

        ' 🔹 Ambil font dari kontrol UI
        Dim fontNama As New Font(CmbFontNama.Text, CSng(CmbUkuranNama.Text), If(ChkBoldNama.Checked, FontStyle.Bold, FontStyle.Regular))
        Dim fontHarga As New Font(CmbFontHarga.Text, CSng(CmbUkuranHarga.Text), If(ChkBoldHarga.Checked, FontStyle.Bold, FontStyle.Regular))
        Dim fontToko As New Font(CmbFontToko.Text, CSng(CmbUkuranToko.Text), If(ChkBoldToko.Checked, FontStyle.Bold, FontStyle.Regular))

        ' 🔹 Format teks agar rata tengah
        Dim sfWrap As New StringFormat() With {
        .Alignment = StringAlignment.Center,
        .LineAlignment = StringAlignment.Center
    }

        ' 🔹 Posisi awal cetak (dalam pixel)
        Dim x As Single = marginKiriPx
        Dim y As Single = marginAtasPx
        Dim printedInThisPage As Integer = 0

        ' 🔹 Loop cetak label
        While currentRow < totalRows
            Dim data = daftarData(currentRow)

            ' 🔹 Gambar kotak label
            g.DrawRectangle(Pens.Black, x, y, lebarLabelPx, tinggiLabelPx)


            If CmbBentuklabel.SelectedIndex = 0 Then

                ' 🔹 Area teks (dibagi rata)
                Dim rectNama As New RectangleF(x, y, lebarLabelPx, tinggiLabelPx * 0.4)
                Dim rectHarga As New RectangleF(x, y + rectNama.Height, lebarLabelPx, tinggiLabelPx * 0.3)
                Dim rectToko As New RectangleF(x, y + rectNama.Height + rectHarga.Height, lebarLabelPx, tinggiLabelPx * 0.3)

                ' 🔹 Cetak teks dengan wrapping
                g.DrawString(data(0), fontNama, Brushes.Black, rectNama, sfWrap) ' Nama Barang
                g.DrawString("Rp. " & ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(data(1))), fontHarga, Brushes.Black, rectHarga, sfWrap) ' Harga
                g.DrawString(data(2), fontToko, Brushes.Black, rectToko, sfWrap) ' Satuan

            ElseIf CmbBentuklabel.SelectedIndex = 1 Then
                ' Atur area teks
                Dim rectNama As New RectangleF(x, y, lebarLabelPx, tinggiLabelPx * 0.35) ' Nama barang
                Dim rectHarga As New RectangleF(x, y + rectNama.Height, lebarLabelPx, tinggiLabelPx * 0.25) ' Harga
                Dim rectSatuan As New RectangleF(x, rectHarga.Bottom, lebarLabelPx, tinggiLabelPx * 0.2) ' Satuan
                Dim rectToko As New RectangleF(x, rectSatuan.Bottom, lebarLabelPx, tinggiLabelPx * 0.2) ' Nama toko

                ' Pastikan teks nama bisa turun ke dua baris jika terlalu panjang
                Dim ukuranNama As SizeF = g.MeasureString(data(0), fontNama, CInt(lebarLabelPx))
                If ukuranNama.Height > rectNama.Height Then
                    rectNama.Height = ukuranNama.Height ' Sesuaikan tinggi nama agar tidak menimpa harga
                    rectHarga.Y = y + rectNama.Height ' Pindahkan harga ke bawah nama
                    rectSatuan.Y = rectHarga.Bottom ' Pindahkan satuan ke bawah harga
                    rectToko.Y = rectSatuan.Bottom ' Pindahkan toko ke bawah satuan
                End If

                ' Gambar teks dengan wrapping
                g.DrawString(data(0), fontNama, New SolidBrush(WarnaNama), rectNama, sfWrap) ' Nama barang
                g.DrawString("Rp. " & ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(data(1))), fontHarga, New SolidBrush(WarnaHarga), rectHarga, sfWrap) ' Harga
                g.DrawString(data(2), fontToko, New SolidBrush(WarnaSatuan), rectSatuan, sfWrap) ' Satuan (pcs, kg, dll.)
                g.DrawString(NAMA_PERUSAHAAN, fontToko, New SolidBrush(WarnaToko), rectToko, sfWrap) ' Nama Toko
            Else
                ' Atur area teks
                Dim rectNama As New RectangleF(x, y, lebarLabelPx, tinggiLabelPx * 0.4) ' Nama barang
                Dim rectHarga As New RectangleF(x, y + rectNama.Height, lebarLabelPx, tinggiLabelPx * 0.3) ' Harga (utuh)
                Dim rectToko As New RectangleF(x, rectHarga.Bottom, lebarLabelPx, tinggiLabelPx * 0.3) ' Nama Toko

                ' Cek apakah harga muat dalam area
                Dim ukuranHarga As SizeF = g.MeasureString("Rp. " & ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(data(1))), fontHarga)
                Dim tampilkanSatuan As Boolean = (ukuranHarga.Width + 30 < lebarLabelPx) ' Tambah 30 pixel buffer

                Dim rectSatuan As RectangleF
                If tampilkanSatuan Then
                    ' Jika cukup ruang, letakkan satuan di kanan harga
                    rectHarga.Width = lebarLabelPx * 0.7
                    rectSatuan = New RectangleF(x + rectHarga.Width, rectHarga.Y, lebarLabelPx * 0.3, rectHarga.Height)
                Else
                    ' Jika tidak cukup ruang, harga mengambil seluruh lebar label
                    rectHarga.Width = lebarLabelPx
                End If

                ' 🔹 Pastikan teks nama bisa turun ke dua baris jika terlalu panjang
                Dim ukuranNama As SizeF = g.MeasureString(data(0), fontNama, CInt(lebarLabelPx))
                If ukuranNama.Height > rectNama.Height Then
                    rectNama.Height = ukuranNama.Height ' Sesuaikan tinggi nama agar tidak menimpa harga
                    rectHarga.Y = y + rectNama.Height ' Pindahkan harga ke bawah nama
                    rectToko.Y = rectHarga.Bottom ' Nama toko tetap di bawah harga
                    rectSatuan.Y = rectHarga.Y ' Pastikan satuan tetap sejajar dengan harga
                End If

                ' 🔹 Gambar teks dengan wrapping
                g.DrawString(data(0), fontNama, New SolidBrush(WarnaNama), rectNama, sfWrap) ' Nama barang
                g.DrawString("Rp. " & ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(data(1))), fontHarga, New SolidBrush(WarnaHarga), rectHarga, sfWrap) ' Harga
                If tampilkanSatuan Then g.DrawString(data(2), fontToko, New SolidBrush(WarnaSatuan), rectSatuan, sfWrap) ' Satuan (jika cukup ruang)
                g.DrawString(NAMA_PERUSAHAAN, fontToko, New SolidBrush(WarnaToko), rectToko, sfWrap) ' Nama Toko

            End If

            ' 🔹 Geser posisi ke kanan
            x += lebarLabelPx + jarakXPx
            printedInThisPage += 1
            currentRow += 1

            ' 🔹 Pindah baris jika sudah penuh
            If printedInThisPage Mod jumlahPerBaris = 0 Then
                x = marginKiriPx
                y += tinggiLabelPx + jarakYPx
            End If

            ' 🔹 Cek apakah harus lanjut ke halaman berikutnya
            If y + tinggiLabelPx > (panjangKertasPx - marginAtasPx) Then
                e.HasMorePages = True
                Return
            End If
        End While

        ' 🔹 Jika semua data sudah dicetak, reset currentRow
        If currentRow >= totalRows Then
            currentRow = 0
            e.HasMorePages = False
        End If
    End Sub


    Private Sub BtnRestore_Click(sender As Object, e As EventArgs) Handles BtnRestore.Click

        ' Hapus file konfigurasi agar dibuat ulang dengan nilai default
        If File.Exists(configFilePath) Then
            File.Delete(configFilePath)
        End If

        ' Buat ulang file dengan nilai default
        BuatFileDefault()

        ' Muat ulang pengaturan dari file yang baru dibuat
        LoadDariINI()

    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs) Handles BtnClear.Click
        DGVLabel.Rows.Clear()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub


End Class