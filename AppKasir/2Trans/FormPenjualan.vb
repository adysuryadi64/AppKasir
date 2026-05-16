Imports System.IO
Public Class FormPenjualan
    Private jenisprintercetak As String
    Private AwalPenjualan As String
    Private EdithargaJual As String
    Private Kodebarangsama As String
    Private Editmasterhargajual As String
    Private modulJualRugi As String
    Private modulJualMinus As String
    Private tampilstok As String = "Iya"
    Private Isinominal As String = "Tidak"
    Private Nominal0 As String = "Tidak"
    Private TanyakanKertas As String = "Tidak"
    Private TransaksiLampau As String = "Tidak"

    ' ✅ TAMBAHAN: Variabel untuk menyimpan jumlah kembalian
    Private kembaliAmount As Decimal = 0

    Private Sub Form_Penjualan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Try
            ' --- 1. SETUP TAMPILAN DASAR ---
            Petunjuk()
            LblTextJalanAtas.Text = "TERIMA KASIH TELAH BELANJA DI " & NAMA_PERUSAHAAN

            ' ✅ PERBAIKAN FASE 1: Safe FormUtama access menggunakan helper
            LblLokasiBarang.Text = GetFormUtamaLocation()

            ' Ukuran Form
            MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
            MinimumSize = Size

            ' --- 2. SETUP KOMPONEN UI & TIMER ---
            CmbCetak.Text = AppConfig.Instance.GetValue(Of String)("CetakJual", "SELALU TANYA")
            KosongTxtboxcari()

            barcodeTimer.Interval = 100
            AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick

            ' --- 3. AMBIL HAK AKSES (SETTING) ---
            ' Ini penting ada di Load agar variabel global terisi sebelum dipakai
            AwalPenjualan = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblJualFokus.Text)
            EdithargaJual = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblJualEditHarga.Text)
            Kodebarangsama = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblJualSatuan.Text)
            Editmasterhargajual = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblEditHargaJual.Text)
            tampilstok = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTampilstokJual.Text)
            Isinominal = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblJualIsiNominal.Text)
            Nominal0 = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblJualNominal0.Text)
            modulJualRugi = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblJualRugi.Text)
            modulJualMinus = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblJualMinus.Text)
            TanyakanKertas = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblJualJenisKertasCetak.Text)
            TransaksiLampau = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTransaksiTanggalLampau.Text)

            ' --- 4. ISI DATA REFERENSI (COMBOBOX) ---
            IsiComboBoxAkun(CmbBayarTunai, "KAS")
            IsiComboBoxAkun(CmbBayarTransfer, "BANK")
            CmbBayarTransfer.SelectedIndex = 0

            ' --- 5. SETUP TANGGAL ---
            DTPTgl.Value = DateTime.Now
            DTPTgl.Format = DateTimePickerFormat.Custom
            DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"
            DTPJatuhTempo.Format = DateTimePickerFormat.Custom
            DTPJatuhTempo.CustomFormat = "dd/MM/yyyy"
            DTPJatuhTempo.Value = DTPJatuhTempo.Value.AddMonths(1)

            AmbilJenisPrinter()
            GBBayar.Visible = False
            BtnSimpann.Location = New Point(190, 226)
            BtnBatal.Location = New Point(383, 226)

            ' --- 6. ATUR KOLOM GRID BERDASARKAN HAK AKSES ---
            If EdithargaJual = "Tidak" Then
                DgvData.Columns("Harga").ReadOnly = True
            Else
                DgvData.Columns("Harga").ReadOnly = False
            End If

            If tampilstok = "Iya" Then
                DgvData.Columns("StokToko").Visible = True
                DgvData.Columns("StokGudang").Visible = True
            Else
                DgvData.Columns("StokToko").Visible = False
                DgvData.Columns("StokGudang").Visible = False
            End If

            FormatKolomDenganCultureIndonesia()

            ' --- 7. PENGATURAN USER SETTINGS (PREFERENCES) ---
            ChkTampilSN.Checked = AppConfig.Instance.GetValue(Of Boolean)("TampilSN", False)

            If DgvData.Columns.Contains("SerialNumber") Then
                DgvData.Columns("SerialNumber").Visible = ChkTampilSN.Checked
            End If

        Catch ex As Exception
            MessageBox.Show("Error Load: " & ex.Message)
        End Try
    End Sub

    Private Sub Form_Penjualan_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
        Try
            If TxtJenistransaksi.Text = "TambahPenjualan" Then
                ' Panggil reset form
                Kondisiawal()
            Else
                ' Panggil mode Edit
                Editpenjualanheader()
            End If

        Catch ex As Exception
            MessageBox.Show("Error Shown: " & ex.Message)
        End Try
    End Sub

    Private Sub Petunjuk()
        ' Atur gaya tooltip menjadi balon
        ToolTip1.ShowAlways = True
        ToolTip1.IsBalloon = True
        ToolTip1.AutoPopDelay = 10000 ' tampil 10 detik
        ToolTip1.InitialDelay = 100   ' delay awal
        ToolTip1.ReshowDelay = 100    ' delay antar hover

        ToolTip1.ToolTipIcon = ToolTipIcon.Info
        ToolTip1.ToolTipTitle = "Petunjuk Penggunaan"

        ' ==== Header Transaksi ====
        ToolTip1.SetToolTip(TxtFaktur, "Nomor faktur transaksi akan dibuat otomatis oleh sistem.")
        ToolTip1.SetToolTip(DTPTgl, "Tanggal transaksi. Anda dapat mengubahnya jika perlu.")
        ToolTip1.SetToolTip(CmbPelanggan, "Pilih pelanggan dari daftar atau tekan [F3] untuk membuka pilihan pelanggan.")
        ToolTip1.SetToolTip(CmbSales, "Pilih nama sales yang bertanggung jawab. Tekan [F2] untuk membuka daftar sales.")
        ToolTip1.SetToolTip(TxtGrantotal, "💰 Grand Total: Menampilkan jumlah total transaksi." & vbCrLf & "Nilai ini sesuai dengan item dan jumlah yang dijual.")

        ' ==== Input Nama Barang ====
        ToolTip1.SetToolTip(TxtNama,
"📌 Cara cepat input nama barang:" & vbCrLf &
"- Ketik langsung nama barang atau scan barcode untuk pencarian otomatis." & vbCrLf &
"- Bisa juga ketik: Jumlah*NamaBarang" & vbCrLf &
"    Contoh: 2*Sabun → jumlah otomatis 2." & vbCrLf &
"- Untuk memilih satuan tertentu, ketik: Jumlah*Satuan(1/2/3)*NamaBarang" & vbCrLf &
"    Contoh: 3*2*Minyak → jumlah 3, satuan 'sedang'.")

        ' ==== Panel Pembayaran ====
        ToolTip1.SetToolTip(CmbBayarTunai, "Pilih metode pembayaran seperti: Kas tunai, Transfer Bank, QRIS, atau metode lainnya.")
        ToolTip1.SetToolTip(CmbBank, "Isi nama bank yang digunakan untuk melakukan transfer, contoh: BCA, Mandiri, BRI, dll.")
        ToolTip1.SetToolTip(TxtNoRek, "Masukkan nomor rekening dari mana dana dikirim. Pastikan angka sesuai dengan bukti transfer.")
        ToolTip1.SetToolTip(TxtNamaRek, "Tulis nama pemilik rekening pengirim sesuai yang tertera pada bukti transfer.")
        ToolTip1.SetToolTip(TxtNoReff, "Isi nomor referensi transaksi transfer (jika tersedia) sebagai bukti tambahan transaksi.")
        ToolTip1.SetToolTip(LblBiayaKirim, "Masukkan biaya tambahan seperti ongkos kirim, biaya COD, dsb.")
        ToolTip1.SetToolTip(LblTotalStlPajak, "Jumlah total belanja setelah diskon dan sebelum pembayaran.")
        ToolTip1.SetToolTip(LblBayarTunai,
"💵 Jumlah uang yang dibayar saat ini." & vbCrLf &
"- Jika jumlah yang dibayar **kurang**, maka sisa akan otomatis menjadi **hutang**." & vbCrLf &
"- Jika dibayar penuh, maka transaksi dianggap **lunas**." & vbCrLf &
"- Jika nilai **0** dimasukkan, maka seluruh total akan menjadi **hutang pelanggan**.")

        ToolTip1.SetToolTip(LblKembali, "Sisa kembalian otomatis dari sistem jika pembayaran melebihi total.")
        ToolTip1.SetToolTip(DTPJatuhTempo, "Untuk pembayaran tempo, tentukan tanggal jatuh tempo di sini.")
        ToolTip1.SetToolTip(LblStatus, "Menampilkan status transaksi: LUNAS jika sudah dibayar penuh, BELUM jika masih ada sisa tagihan.")

        ' ==== Tombol Aksi ====
        ToolTip1.SetToolTip(BtnBayar, "Proses pembayaran setelah selesei memasukkan barang. Shortcut: [F8].")
        ToolTip1.SetToolTip(BtnSimpann, "Simpan dan proses pembayaran transaksi ini. Shortcut: [F10].")
        ToolTip1.SetToolTip(BtnBatal, "Batalkan pembayaran. Shortcut: [F11].")
        ToolTip1.SetToolTip(BtnTahan, "Tahan transaksi sementara untuk dipanggil kembali nanti. Shortcut: [F6].")
        ToolTip1.SetToolTip(BtnPanggil, "Panggil kembali transaksi yang sebelumnya ditahan. Shortcut: [F7].")
        ToolTip1.SetToolTip(BtnBarang, "Tambah atau edit data barang. Shortcut: [F4].")
        ToolTip1.SetToolTip(BtnPelanggan, "Tambah atau edit data pelanggan. Shortcut: [F12].")

        ToolTip1.SetToolTip(CmbCetak,
"🖨️ Pengaturan Cetak Nota:" & vbCrLf &
"- **IYA** → Nota langsung dicetak otomatis setelah simpan." & vbCrLf &
"- **TIDAK** → Nota tidak dicetak otomatis." & vbCrLf &
"- **SELALU TANYA** → Akan muncul pertanyaan setiap kali simpan: 'Ingin cetak nota?'")

        ' ==== Diskon & Pajak ====
        ToolTip1.SetToolTip(TxtDiskonPersen, "Persentase diskon untuk total belanja. (Contoh: 10 untuk diskon 10%)")
        ToolTip1.SetToolTip(TxtDiskonRp, "Diskon langsung dalam rupiah. (Contoh: 5000)")
        ToolTip1.SetToolTip(TxtPajakPersen, "Persentase pajak (PPN). (Contoh: 11 untuk PPN 11%)")
        ToolTip1.SetToolTip(TxtPajakRp, "Nilai pajak dalam rupiah jika ditentukan manual.")
    End Sub


    Private Sub FormatKolomDenganCultureIndonesia()
        ' Daftar nama kolom yang akan diformat
        Dim kolomList As String() = {
        "HargaBeli", "QTY", "Isi", "Totalhargabeli", "Harga", "QtySat",
        "DiskonPersen", "DiskonRp", "TotalDiskon", "TotalHarga",
        "StokToko", "StokGudang", "Stok"
    }

        For Each kolom As String In kolomList
            If DgvData.Columns.Contains(kolom) Then
                DgvData.Columns(kolom).DefaultCellStyle.Format = "#,0.##" ' Format angka dengan 2 desimal
                DgvData.Columns(kolom).DefaultCellStyle.FormatProvider = cultureIndonesia
                DgvData.Columns(kolom).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next
    End Sub

    Private Sub CmbCetak_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbCetak.SelectedIndexChanged
        AppConfig.Instance.SetValue("CetakJual", CmbCetak.Text)
        AppConfig.Instance.Save()
    End Sub

    Private Sub ClearFocus()
        ' Dapatkan kontrol yang sedang memiliki fokus
        Dim currentControl As Control = ActiveControl

        ' Jika ada kontrol yang memiliki fokus, hilangkan fokusnya
        currentControl?.Parent.Focus()

        ' Hilangkan fokus dari seluruh kontrol
        ActiveControl = Nothing
    End Sub


    Private Sub TxtNama_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCariNama.BackColor = Color.Yellow

        ' ✅ PERBAIKAN FASE 1: Safe bounds checking untuk DataGridView
        If DgvData.Rows.Count > 0 AndAlso DgvData.Columns.Count > 1 Then
            Try
                ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
                DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)
                ' Mengatur baris terakhir sebagai baris yang dipilih
                DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
            Catch
                ' Ignore jika cell access gagal
            End Try
        End If
    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TxtNama_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.LostFocus
        ResetBarcodeDetection()
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCariNama.BackColor = SystemColors.ActiveCaption
    End Sub

    Private Sub KosongTxtboxcari()
        LstBarang.Items.Clear()
        LstBarang.Visible = False
        TxtKode.Clear()
        TxtQty.Clear()
        Txtsatuan.Clear()
        TxtIsi.Clear()
        TxtHargaBeli.Clear()
        TxtBarcode.Clear()
        TxtHargaJual.Clear()
        TxtStokToko.Clear()
        TXtStokGudang.Clear()
        TxtStok.Clear()
        TxtNama.Clear()
        rtbPetunjuk.Clear()
    End Sub

    Public Sub Kondisiawal()

        ' Persiapan untuk mode Tambah Baru
        LblJenisPl.Text = "Umum"
        LbLKodePel.Text = ""

        DgvData.DataSource = Nothing
        DgvData.Rows.Clear()

        TampilPelanggan()
        AmbilDataKaryawan()
        Nomorjual()
        JumlahTahan()
        UpdateSemuaTotal()

        If CmbPelanggan.Items.Count > 0 Then CmbPelanggan.SelectedIndex = 0
        If CmbSales.Items.Count > 0 Then CmbSales.SelectedIndex = 0

        LblSales.Text = ""
        ' ✅ Gunakan format STANDAR (InvariantCulture - no separator) - tampilkan desimal hanya jika ada
        TxtTotaljualStlPajak.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtNominalBayarTunai.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtKembaliHutang.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        LblStatusTrans.Text = "Belum Lunas"

        TxtTotalJualSblDiskonPajak.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtGrantotal.Text = "Rp. 0"
        TxtDiskonPersen.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtDiskonRp.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPajakPersen.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPajakRp.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtBiayaKirim.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        CmbBank.SelectedIndex = 0
        TxtNoRek.Clear()
        TxtNamaRek.Clear()
        TxtNoReff.Clear()

        If LblLokasiBarang.Text = "TOKO" Then
            CmbBayarTunai.SelectedItem = nama_rek_Jual_Toko
            TxtKodeBayarTunai.Text = Kode_rek_Jual_Toko
        ElseIf LblLokasiBarang.Text = "GUDANG" Then
            CmbBayarTunai.SelectedItem = nama_rek_Jual_Gudang
            TxtKodeBayarTunai.Text = Kode_rek_Jual_Gudang
        End If

        GBBayar.Visible = False

        TxtLogin.Clear()
        TxtKomputer.Clear()


        SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
    End Sub

    ' ===== REUSABLE SUB - ATUR FOKUS KE DATAGRID =====
    ''' <summary>
    ''' Mengatur fokus ke DataGridView dengan behavior berbeda berdasarkan AwalPenjualan
    ''' ✅ Jika AwalPenjualan = "Pencarian": fokus ke TxtNama (mode input/barcode)
    ''' ✅ Jika AwalPenjualan ≠ "Pencarian": fokus ke sel NamaBarang baris terakhir (mode edit langsung)
    ''' </summary>
    Public Sub SetupFocusToGrid()
        ' Cek apakah DgvData memiliki baris
        If DgvData.Rows.Count > 0 Then
            ' Mengatur sel aktif pada kolom NamaBarang (index 1) dan baris terakhir
            DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

            ' Mengatur baris terakhir sebagai baris yang dipilih
            DgvData.Rows(DgvData.Rows.Count - 1).Selected = True

            ' ✅ BEHAVIOR BERDASARKAN AwalPenjualan
            If AwalPenjualan = "Pencarian" Then
                ' MODE 1: Pencarian - fokus ke TxtNama (input manual/barcode)
                TxtNama.Select()
                TxtNama.Focus()
            Else
                ' MODE 2: Edit Langsung - fokus ke sel NamaBarang untuk edit inline
                DgvData.Select()
                DgvData.Focus()
                DgvData.BeginEdit(True) ' Mulai edit mode di sel saat ini
            End If
        End If
    End Sub

    Public Sub AmbilJenisPrinter()
        Dim filePath As String = "printer.ini"

        Using reader As New StreamReader(filePath)
            Do While Not reader.EndOfStream
                Dim line As String = reader.ReadLine()
                If Not String.IsNullOrEmpty(line) Then
                    Dim parts As String() = line.Split("="c)
                    If parts.Length = 2 AndAlso parts(0).Trim() = "JenisPrinterJual" Then
                        jenisprintercetak = parts(1).Trim()
                        Exit Do
                    End If
                End If
            Loop
        End Using
    End Sub

    Private SkipValidation As Boolean = False


    Public Sub TampilPelanggan()
        Using cmd As New MySqlCommand("SELECT NAMA FROM tbl_pelanggan ORDER BY NAMA ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                CmbPelanggan.Items.Clear()
                CmbPelanggan.Items.Add("")

                While rd.Read()
                    CmbPelanggan.Items.Add(rd("NAMA").ToString())
                End While
            End Using
        End Using

        ' Kosongkan combobox tanpa memicu validating
        SkipValidation = True
        CmbPelanggan.SelectedIndex = -1
        SkipValidation = False

        ' Cegah event handler dobel
        RemoveHandler CmbPelanggan.Validating, AddressOf ComboBox_Validating
        AddHandler CmbPelanggan.Validating, AddressOf ComboBox_Validating
    End Sub


    Private Sub ComboBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
        If SkipValidation Then Exit Sub ' cegah loop error

        Dim comboBox As ComboBox = CType(sender, ComboBox)

        ' Jika kosong (""), anggap valid
        If comboBox.Text = "" Then Exit Sub

        ' Jika nilai tidak ada dalam daftar → beri peringatan
        If Not comboBox.Items.Contains(comboBox.Text) Then
            MessageBox.Show("Harap pilih nama pelanggan yang valid dari daftar.",
                        "Pilihan pelanggan Tidak Valid",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
            e.Cancel = True
        End If
    End Sub

    Private Sub DTPTgl_ValueChanged(sender As Object, e As EventArgs) Handles DTPTgl.ValueChanged
        If TxtJenistransaksi.Text = "TambahPenjualan" Then
            Nomorjual()
        End If
    End Sub

    Public Sub Nomorjual()
        Dim cekTanggal As String = DTPTgl.Value.ToString("yyMMdd")
        Dim ceknomor As String = "PJ-" & cekTanggal
        Dim UrutKode As String = ""

        Using cmd As New MySqlCommand("SELECT MAX(ID_PENJUALAN) FROM penjualan WHERE ID_PENJUALAN LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", ceknomor & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim maxKode As Object = rd.GetValue(0)
                    If Not IsDBNull(maxKode) Then
                        Dim maxKodePenjualan As String = maxKode.ToString()
                        If Not String.IsNullOrEmpty(maxKodePenjualan) Then
                            If Microsoft.VisualBasic.Left(maxKodePenjualan, 9) = "PJ-" & cekTanggal Then
                                Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(maxKodePenjualan, 4)) + 1
                                UrutKode = "PJ-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                            End If
                        End If
                    End If
                End If
            End Using
        End Using

        Using cmdTemp As New MySqlCommand("SELECT MAX(FAKTUR_JUAL) FROM penjualan_ditahan WHERE FAKTUR_JUAL LIKE @ceknomor", conn)
            cmdTemp.Parameters.AddWithValue("@ceknomor", ceknomor & "%")
            Using rdTemp As MySqlDataReader = cmdTemp.ExecuteReader()
                If rdTemp.Read() Then
                    Dim maxKodeTemp As Object = rdTemp.GetValue(0)
                    If Not IsDBNull(maxKodeTemp) Then
                        Dim maxKode As String = maxKodeTemp.ToString()
                        If Not String.IsNullOrEmpty(maxKode) Then
                            If Microsoft.VisualBasic.Left(maxKode, 9) = "PJ-" & cekTanggal Then
                                Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(maxKode, 4)) + 1
                                If UrutKode = "" Then
                                    UrutKode = "PJ-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                                Else
                                    Dim maxKodeUrut As String = "PJ-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                                    If String.Compare(maxKodeUrut, UrutKode) > 0 Then
                                        UrutKode = maxKodeUrut
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End Using
        End Using


        If UrutKode = "" Then
            UrutKode = "PJ-" & cekTanggal & "0001"
        End If

        TxtFaktur.Text = UrutKode
    End Sub

    Public Sub AmbilDataKaryawan()
        CmbSales.Items.Clear()
        CmbSales.Items.Add("")
        ' Query untuk mengambil akun KAS atau BANK
        Dim queryArmada As String = "SELECT Nama FROM tbl_Karyawan ORDER BY Nama ASC"
        Using cmd As New MySqlCommand(queryArmada, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()

                        CmbSales.Items.Add(rd("Nama").ToString())
                    End While
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbSales_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSales.SelectedIndexChanged
        ' Panggil metode untuk mengambil informasi sales
        AmbilInformasiSales()
    End Sub

    Private Sub CmbSales_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSales.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            AmbilInformasiSales()
            SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub AmbilInformasiSales()
        Dim sql As String = "SELECT Kode FROM tbl_karyawan WHERE Nama = @Nama"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", CmbSales.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblSales.Text = reader("Kode").ToString()
                Else
                    LblSales.Text = ""
                End If
            End Using
        End Using


    End Sub

    Private Sub CmbPelanggan_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbPelanggan.SelectedIndexChanged
        ' Panggil metode untuk mengambil informasi pelanggan
        AmbilInformasiPelanggan()
    End Sub

    Private Sub CmbPelanggan_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbPelanggan.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub AmbilInformasiPelanggan()
        ' Deklarasi variabel untuk menyimpan nilai
        Dim jenisPelanggan As String = "Umum"
        Dim kodePelanggan As String = ""
        Dim alamatPelanggan As String = ""
        Dim jangkaPiutang As Integer = 0

        ' Mengambil informasi pelanggan dari database
        Using cmd As New MySqlCommand("SELECT KODE, ALAMAT, JENIS, JangkaPiutang FROM tbl_pelanggan WHERE nama = @nama", conn)
            cmd.Parameters.AddWithValue("@nama", CmbPelanggan.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Simpan nilai dari database ke variabel
                    jenisPelanggan = If(rd.IsDBNull(rd.GetOrdinal("JENIS")), "Umum", rd.Item("JENIS").ToString())
                    kodePelanggan = If(rd.IsDBNull(rd.GetOrdinal("KODE")), "", rd.Item("KODE").ToString())
                    alamatPelanggan = If(rd.IsDBNull(rd.GetOrdinal("ALAMAT")), "", rd.Item("ALAMAT").ToString())
                    jangkaPiutang = If(rd.IsDBNull(rd.GetOrdinal("JangkaPiutang")), 0, Convert.ToInt32(rd.Item("JangkaPiutang")))
                Else
                    ' Jika tidak ada data, gunakan nilai default (sudah diinisialisasi)
                    jangkaPiutang = 30 ' Default jatuh tempo 30 hari
                End If
            End Using
        End Using

        ' Masukkan nilai ke label di luar blok reader
        LblJenisPl.Text = jenisPelanggan
        LbLKodePel.Text = kodePelanggan
        LblAlamat.Text = alamatPelanggan

        ' Hitung dan set tanggal jatuh tempo
        DTPJatuhTempo.Value = DTPTgl.Value.AddDays(jangkaPiutang)

    End Sub

    Private Sub LblJenisPl_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblJenisPl.TextChanged
        ' Memperbarui data setelah pelanggan dipilih
        UpdateHargaBerdasarJenisPelanggan()
    End Sub

    Private Sub UpdateHargaBerdasarJenisPelanggan()
        ' ✅ BATCH 4 OPTIMIZATION: Batch query daripada per-item queries
        ' MASALAH LAMA: 100 item = 100 queries → 3-5 detik
        ' SOLUSI BARU: 1 batch query → 300-500ms (90% lebih cepat!)

        ' ═══════════════════════════════════════════════════════════════════
        ' LANGKAH 1: Kumpulkan semua kode barang dari grid
        ' ═══════════════════════════════════════════════════════════════════
        Dim daftarKodeBarang As New List(Of String)()

        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                daftarKodeBarang.Add(dgvRow.Cells("Kode").Value.ToString())
            End If
        Next

        ' Jika tidak ada barang, langsung selesai
        If daftarKodeBarang.Count = 0 Then
            Hitungbaris()
            UpdateSemuaTotal()
            Return
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' LANGKAH 2: ✅ SINGLE BATCH QUERY untuk semua barang (bukan per-item!)
        ' ═══════════════════════════════════════════════════════════════════
        Dim inClause As String = String.Join(",", daftarKodeBarang.Select(Function(x) "'" & x.Replace("'", "''") & "'"))
        Dim queryBatch As String = "SELECT ID_BARANG, HARGA_BELI, " &
                                   "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                                   "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                                   "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, " &
                                   "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, " &
                                   "STOK_TOKO, STOK_GUDANG " &
                                   "FROM tbl_barang WHERE ID_BARANG IN (" & inClause & ")"

        ' Simpan hasil batch query di Dictionary untuk akses cepat
        Dim dictBarang As New Dictionary(Of String, BarangHargaInfo)

        Try
            Using cmd As New MySqlCommand(queryBatch, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim idBarang As String = rd("ID_BARANG").ToString()

                        ' ✅ Simpan info barang di Dictionary
                        dictBarang(idBarang) = New BarangHargaInfo() With {
                            .HargaBeli = SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D),
                            .SatuanUmumKecil = SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", ""),
                            .SatuanUmumSedang = SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", ""),
                            .SatuanUmumBesar = SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", ""),
                            .HargaJualUmumKecil = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_KECIL", 0D),
                            .HargaJualUmumSedang = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_SEDANG", 0D),
                            .HargaJualUmumBesar = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_BESAR", 0D),
                            .SatuanPartaiKecil = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_KECIL", ""),
                            .SatuanPartaiSedang = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_SEDANG", ""),
                            .SatuanPartaiBesar = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_BESAR", ""),
                            .HargaJualPartaiKecil = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_KECIL", 0D),
                            .HargaJualPartaiSedang = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_SEDANG", 0D),
                            .HargaJualPartaiBesar = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_BESAR", 0D),
                            .StokToko = SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D),
                            .StokGudang = SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                        }
                    End While
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("UpdateHargaBerdasarJenisPelanggan Batch Query Error: " & ex.Message)
            Hitungbaris()
            UpdateSemuaTotal()
            Return
        End Try

        ' ═══════════════════════════════════════════════════════════════════
        ' LANGKAH 3: Update grid OFFLINE (tanpa database queries!)
        ' ═══════════════════════════════════════════════════════════════════
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                ' Ambil info dari dictionary (O(1) lookup, no query!)
                If dictBarang.ContainsKey(kodeBarangValue) Then
                    Dim infoBarang As BarangHargaInfo = dictBarang(kodeBarangValue)

                    ' Update harga beli
                    dgvRow.Cells("HargaBeli").Value = infoBarang.HargaBeli

                    ' Update harga jual berdasarkan satuan yang dipilih
                    If Not IsDBNull(dgvRow.Cells("Satuan").Value) Then
                        Dim nilaiSatuan As String = dgvRow.Cells("Satuan").Value.ToString()
                        Dim hargaValue As Decimal = 0D

                        If LblJenisPl.Text = "Partai" Then
                            ' Cek tipe satuan partai
                            If nilaiSatuan = infoBarang.SatuanPartaiSedang Then
                                hargaValue = infoBarang.HargaJualPartaiSedang
                            ElseIf nilaiSatuan = infoBarang.SatuanPartaiBesar Then
                                hargaValue = infoBarang.HargaJualPartaiBesar
                            Else
                                hargaValue = infoBarang.HargaJualPartaiKecil
                            End If
                        Else
                            ' Cek tipe satuan umum
                            If nilaiSatuan = infoBarang.SatuanUmumSedang Then
                                hargaValue = infoBarang.HargaJualUmumSedang
                            ElseIf nilaiSatuan = infoBarang.SatuanUmumBesar Then
                                hargaValue = infoBarang.HargaJualUmumBesar
                            Else
                                hargaValue = infoBarang.HargaJualUmumKecil
                            End If
                        End If

                        dgvRow.Cells("Harga").Value = hargaValue
                    End If

                    ' Update stok
                    dgvRow.Cells("StokToko").Value = infoBarang.StokToko
                    dgvRow.Cells("StokGudang").Value = infoBarang.StokGudang

                    Dim stokValue As Decimal = If(LblLokasiBarang.Text = "GUDANG",
                                                  infoBarang.StokGudang,
                                                  infoBarang.StokToko)
                    dgvRow.Cells("Stok").Value = stokValue
                End If
            End If
        Next

        ' ═══════════════════════════════════════════════════════════════════
        ' LANGKAH 4: Hitung ulang total dan refresh display
        ' ═══════════════════════════════════════════════════════════════════
        Hitungbaris()
        UpdateSemuaTotal()
    End Sub

    ' ===== BARCODE DETECTION - HYBRID SUPPORT =====
    Private isBarcodeMode As Boolean = False
    Private barcodeChars As New List(Of Char)()
    Private barcodeStartTime As DateTime = DateTime.MinValue
    Private lastKeyTime As DateTime = DateTime.MinValue
    Private barcodeTimer As New System.Windows.Forms.Timer()

    Private Const BARCODE_CHAR_INTERVAL_MS As Integer = 30
    Private Const BARCODE_TOTAL_TIME_MS As Integer = 200
    Private Const BARCODE_MIN_LENGTH As Integer = 4
    Private Const BARCODE_MAX_LENGTH As Integer = 100


    Private Sub TxtNama_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNama.KeyDown
        ' ===== SPECIAL KEYS =====
        If e.KeyCode = Keys.Down AndAlso LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
            LstBarang.Focus()
            LstBarang.SelectedIndex = 0
            e.SuppressKeyPress = True
            Return
        End If

        If e.KeyCode = Keys.Tab Then
            DgvData.Select()
            DgvData.Focus()
            e.SuppressKeyPress = True
            Return
        End If

        ' ===== PRINTABLE CHARACTERS =====
        Dim ch As Char = ChrW(e.KeyCode)
        If Not Char.IsControl(ch) Then
            ' Jika user mengetik huruf atau tanda '*' berarti input manual -> batalkan deteksi barcode
            If ch = "*"c OrElse Char.IsLetter(ch) Then
                ' Pastikan tidak ada proses barcode yang berjalan
                ResetBarcodeDetection()
                ' Biarkan event normal (teks masuk ke TxtNama)
                Return
            End If

            Dim currentTime = DateTime.Now

            ' Karakter pertama
            If barcodeChars.Count = 0 Then
                barcodeStartTime = currentTime
                barcodeChars.Add(ch)
                lastKeyTime = currentTime

                barcodeTimer.Interval = 100
                barcodeTimer.Stop()
                barcodeTimer.Start()
                Return
            End If

            ' Hitung interval dari karakter sebelumnya
            Dim intervalMs = (currentTime - lastKeyTime).TotalMilliseconds

            ' Jika interval > threshold = MANUAL input
            If intervalMs > BARCODE_CHAR_INTERVAL_MS Then
                isBarcodeMode = False
            End If

            ' Tambah karakter
            If barcodeChars.Count < BARCODE_MAX_LENGTH Then
                barcodeChars.Add(ch)
            End If

            lastKeyTime = currentTime
            barcodeTimer.Stop()
            barcodeTimer.Start()
            Return
        End If

        ' ===== ENTER KEY =====
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            barcodeTimer.Stop()

            If String.IsNullOrWhiteSpace(TxtNama.Text) Then
                ResetBarcodeDetection()
                Return
            End If

            Dim totalTimeMs = (DateTime.Now - barcodeStartTime).TotalMilliseconds
            Dim inputText = TxtNama.Text.Trim()

            ' Proses input sesuai tipe
            ProcessInput(inputText, totalTimeMs)
            ResetBarcodeDetection()
            Return
        End If

        If e.KeyCode = Keys.Back Or e.KeyCode = Keys.Delete Then
            isBarcodeMode = False
        End If
    End Sub


    Private Sub TxtNama_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.TextChanged
        Dim currentText = TxtNama.Text.Trim()

        If String.IsNullOrEmpty(currentText) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        ' Tampilkan search list HANYA untuk input manual
        ' (ada huruf, atau format qty*... tanpa barcode)
        If currentText.Any(AddressOf Char.IsLetter) Then
            TriggerManualSearch(currentText)
        ElseIf currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            ' Cek bagian terakhir (keyword) punya huruf
            If parts.Length > 0 AndAlso parts(parts.Length - 1).Any(AddressOf Char.IsLetter) Then
                TriggerManualSearch(currentText)
            End If
        End If
    End Sub

    Private Sub BarcodeTimer_Tick(sender As Object, e As EventArgs)
        Dim elapsedSinceLastKey = (DateTime.Now - lastKeyTime).TotalMilliseconds

        If elapsedSinceLastKey > 100 Then
            barcodeTimer.Stop()

            Dim bufferText = New String(barcodeChars.ToArray())
            If bufferText.Length >= BARCODE_MIN_LENGTH Then
                ' Jika buffer mengandung '*' atau huruf -> anggap input manual bertempo cepat.
                ' Jangan jalankan alur scan yang bisa menutup ListBox; panggil pencarian manual.
                If bufferText.Contains("*"c) OrElse bufferText.Any(AddressOf Char.IsLetter) Then
                    ' Trigger manual search menggunakan buffer (TriggerManualSearch sudah mem-parsing qty*)
                    TriggerManualSearch(bufferText)
                    ResetBarcodeDetection()
                    Return
                End If

                ' Jika murni numeric/alphanumeric tanpa huruf/'*' -> perlakukan sebagai kemungkinan barcode
                ProcessInput(bufferText, (DateTime.Now - barcodeStartTime).TotalMilliseconds)
                ResetBarcodeDetection()
            End If
        End If
    End Sub


    ' ===== BARCODE DETECTION - HELPER FUNCTIONS =====

    ''' <summary>
    ''' Tentukan apakah input adalah barcode candidate (rapid input + exists in DB)
    ''' ✅ Support: numeric (8991234567890), alphanumeric (ABC-123), mixed (P9K2L5)
    ''' </summary>
    Private Function IsBarcodeCandidate(input As String) As Boolean
        If input.Length < BARCODE_MIN_LENGTH Then Return False
        Return BarcodeExistsInDatabase(input)
    End Function

    ''' <summary>
    ''' Cek apakah barcode ada di salah satu kolom barcode
    ''' ✅ Support numeric, alphanumeric, dan mixed format
    ''' </summary>
    Private Function BarcodeExistsInDatabase(barcodeValue As String) As Boolean
        Const query = "SELECT 1 FROM tbl_barang " &
                 "WHERE BARCODE_KECIL = @bc OR BARCODE_SEDANG = @bc OR BARCODE_BESAR = @bc LIMIT 1"
        Try
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@bc", barcodeValue)
                Return cmd.ExecuteScalar() IsNot Nothing
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Sub SetQtyAndSatuan(qtyStr As String, satuanStr As String)
        Dim qty = ParseDecimalSafe(qtyStr, 1)
        TxtQty.Text = qty.ToString()
        TxtLevelSat.Text = satuanStr
    End Sub

    Private Sub SetQtyOnly(qtyStr As String)
        Dim qty = ParseDecimalSafe(qtyStr, 1)
        TxtQty.Text = qty.ToString()
        TxtLevelSat.Text = "1"
    End Sub

    Private Sub SetDefaultQtyAndSatuan()
        TxtQty.Text = "1"
        TxtLevelSat.Text = "1"
    End Sub

    Private Function ParseDecimalSafe(value As String, defaultValue As Decimal) As Decimal
        If String.IsNullOrEmpty(value) Then Return defaultValue
        Dim result As Decimal
        Return If(Decimal.TryParse(value, result) AndAlso result > 0, result, defaultValue)
    End Function

    Private Function GetStockForDisplay(rd As MySqlDataReader) As Decimal
        Select Case LblLokasiBarang.Text
            Case "TOKO"
                Return If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
            Case "GUDANG"
                Return If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))
            Case Else
                Return 0D
        End Select
    End Function

    ''' <summary>
    ''' Process input berdasarkan karakteristiknya
    ''' Support: qty*barcode, qty*satuan*nama, barcode murni, pencarian nama
    ''' ✅ Support barcode numeric, alphanumeric, dan mixed format
    ''' </summary>
    Private Sub ProcessInput(inputText As String, totalTimeMs As Double)
        Dim asteriskCount = inputText.Count(Function(c) c = "*"c)

        ' FORMAT 1: qty*satuan*nama
        If asteriskCount = 2 Then
            Dim parts As String() = inputText.Split(New Char() {"*"c})
            SetQtyAndSatuan(parts(0), parts(1))
            ProcessManualSearchList(parts(2).Trim())
            Return
        End If

        ' FORMAT 2: qty*sesuatu
        If asteriskCount = 1 Then
            Dim parts As String() = inputText.Split(New Char() {"*"c})
            SetQtyOnly(parts(0))

            Dim secondPart = parts(1).Trim()

            ' Jika input cepat (diperkirakan scan) dan panjangnya layak barcode -> perlakukan sebagai scan
            If totalTimeMs <= BARCODE_TOTAL_TIME_MS AndAlso secondPart.Length >= BARCODE_MIN_LENGTH Then
                If SearchByBarcode(secondPart) Then
                    Return
                Else
                    MessageBox.Show("Barcode '" & secondPart & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    TxtNama.Clear()
                    Return
                End If
            End If

            ' Jika bukan scan, coba deteksi barcode di DB lalu fallback ke pencarian manual
            If IsBarcodeCandidate(secondPart) AndAlso SearchByBarcode(secondPart) Then
                Return
            End If

            ProcessManualSearchList(secondPart)
            TxtLevelSat.Text = "1"
            Return
        End If

        ' FORMAT 3: Barcode atau manual murni (no asterisk)
        If Not inputText.Contains("*") Then
            ' Jika input cepat dan panjangnya memenuhi syarat → anggap scan walau tidak ada di DB sebelum
            If totalTimeMs <= BARCODE_TOTAL_TIME_MS AndAlso inputText.Length >= BARCODE_MIN_LENGTH Then
                If SearchByBarcode(inputText) Then
                    Return
                Else
                    MessageBox.Show("Barcode '" & inputText & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    TxtNama.Clear()
                    Return
                End If
            End If

            ' Bukan scan cepat → normal flow: jika kandidat barcode dan ditemukan, proses; jika tidak, manual search
            If IsBarcodeCandidate(inputText) AndAlso SearchByBarcode(inputText) Then
                Return
            End If

            SetDefaultQtyAndSatuan()
            ProcessManualSearchList(inputText)
            Return
        End If
    End Sub
    ''' <summary>
    ''' Cari barcode di database dengan EXACT MATCH
    ''' Return True jika ditemukan, False jika tidak
    ''' DEFAULT QTY = 1 saat barcode ditemukan (jika belum ada qty)
    ''' </summary>
    Private Function SearchByBarcode(barcodeText As String) As Boolean
        Dim query = "SELECT NAMA_BARANG FROM tbl_barang " &
                   "WHERE BARCODE_KECIL = @bc OR BARCODE_SEDANG = @bc OR BARCODE_BESAR = @bc LIMIT 1"

        Dim namaBarang As String = ""
        Dim ditemukan As Boolean = False

        Try
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@bc", barcodeText)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        namaBarang = rd("NAMA_BARANG").ToString()
                        ditemukan = True
                    End If
                End Using
            End Using
        Catch
            Return False
        End Try

        If ditemukan Then
            TxtBarcode.Text = barcodeText
            TxtNama.Text = ""
            LstBarang.Visible = False

            ' Pastikan qty minimal 1 (jika kosong atau invalid)
            If String.IsNullOrEmpty(TxtQty.Text) OrElse Not Decimal.TryParse(TxtQty.Text, Nothing) OrElse ParseDecimal(TxtQty.Text) <= 0 Then
                TxtQty.Text = "1"
            End If

            ' Pastikan TxtLevelSat minimal 1
            If String.IsNullOrEmpty(TxtLevelSat.Text) Then
                TxtLevelSat.Text = "1"
            End If

            Ambildatalaindaridbbarang(namaBarang)
            Return True
        End If

        Return False
    End Function

    Private Sub TriggerManualSearch(keyword As String)
        ' Pastikan hentikan deteksi barcode agar tidak menutup ListBox
        ResetBarcodeDetection()

        ' Parse jika ada tanda *
        If keyword.Contains("*") Then
            Dim parts = keyword.Split("*"c)
            If parts.Length >= 2 Then
                keyword = parts.Last().Trim()
            End If
        End If

        If keyword.Length < 2 Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            ' Ensure default qty = 1
            If String.IsNullOrEmpty(TxtQty.Text) Then
                TxtQty.Text = "1"
            End If
            If String.IsNullOrEmpty(TxtLevelSat.Text) Then
                TxtLevelSat.Text = "1"
            End If
            Return
        End If

        ProcessManualSearchList(keyword)
    End Sub

    Private Sub ProcessManualSearchList(searchKeyword As String)
        searchKeyword = searchKeyword.Trim()

        ' Min 2 karakter
        If searchKeyword.Length < 2 AndAlso Not searchKeyword.All(AddressOf Char.IsDigit) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            ' Set default qty = 1
            If String.IsNullOrEmpty(TxtQty.Text) Then
                TxtQty.Text = "1"
            End If
            If String.IsNullOrEmpty(TxtLevelSat.Text) Then
                TxtLevelSat.Text = "1"
            End If
            Return
        End If

        ' =========================================
        ' DEFINISI SEKALI (dipakai di 2 tempat)
        ' =========================================
        Dim stokField As String = If(LblLokasiBarang.Text = "GUDANG", "STOK_GUDANG", "STOK_TOKO")
        Dim orderBy As String = stokField & " DESC"
        ' =========================================

        Dim query = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
                "WHERE TRIM(ID_BARANG) LIKE @key " &
                "   OR TRIM(NAMA_BARANG) LIKE @key " &
                "   OR TRIM(BARCODE_KECIL) LIKE @key " &
                "   OR TRIM(BARCODE_SEDANG) LIKE @key " &
                "   OR TRIM(BARCODE_BESAR) LIKE @key " &
                "ORDER BY " & orderBy

        Try
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@key", "%" & searchKeyword & "%")

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    LstBarang.Items.Clear()

                    While rd.Read()
                        Dim itemText = rd("NAMA_BARANG").ToString()

                        ' =========================================
                        ' PAKAI stokField (tidak perlu CASE lagi)
                        ' =========================================
                        Dim stok = If(IsDBNull(rd(stokField)), 0D, ParseDecimal(rd(stokField)))
                        itemText &= " => " & stok.ToString("N0")
                        ' =========================================

                        LstBarang.Items.Add(itemText)
                    End While

                    LstBarang.Visible = (LstBarang.Items.Count > 0)

                    ' Set default qty = 1 jika kosong
                    If String.IsNullOrEmpty(TxtQty.Text) Then
                        TxtQty.Text = "1"
                    End If
                    If String.IsNullOrEmpty(TxtLevelSat.Text) Then
                        TxtLevelSat.Text = "1"
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error search: " & ex.Message)
        End Try
    End Sub
    Private Sub ResetBarcodeDetection()
        isBarcodeMode = False
        barcodeChars.Clear()
        barcodeStartTime = DateTime.MinValue
        lastKeyTime = DateTime.MinValue
        barcodeTimer.Stop()
    End Sub

    Private Sub LstBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles LstBarang.KeyDown
        If e.KeyCode = Keys.Enter AndAlso LstBarang.SelectedItem IsNot Nothing Then
            AmbilDataDariListBox()
        End If
    End Sub

    Private Sub LstBarang_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LstBarang.MouseClick
        If LstBarang.SelectedItem IsNot Nothing Then
            AmbilDataDariListBox()
        End If
    End Sub

    Private Sub AmbilDataDariListBox()
        Dim namayangdiambil As String = ""
        Dim originalInput As String = TxtNama.Text.Trim()

        ' Ambil nama dari ListBox jika ada selection (prioritas)
        Dim selectedValue As String = ""
        If LstBarang.Items.Count = 1 Then
            selectedValue = LstBarang.Items(0).ToString()
        ElseIf LstBarang.SelectedItem IsNot Nothing Then
            selectedValue = LstBarang.SelectedItem.ToString()
        End If

        If Not String.IsNullOrEmpty(selectedValue) Then
            ' Hapus postfix stok " => 123" jika ada
            Dim indexArrow As Integer = selectedValue.IndexOf(" => ")
            If indexArrow >= 0 Then
                namayangdiambil = selectedValue.Substring(0, indexArrow).Trim()
            Else
                namayangdiambil = selectedValue.Trim()
            End If

            ' Jika user mengetik qty atau qty*satuan*... di TxtNama, parse qty/satuan
            If originalInput.Contains("*"c) Then
                Dim inputParts As String() = originalInput.Split("*"c)
                If inputParts.Length >= 3 Then
                    ' Format: qty*satuan*nama -> parse qty & satuan, gunakan nama dari ListBox (lebih akurat)
                    SetQtyAndSatuan(inputParts(0).Trim(), inputParts(1).Trim())
                ElseIf inputParts.Length = 2 Then
                    ' Format: qty*nama -> parse qty only
                    SetQtyOnly(inputParts(0).Trim())
                End If
            End If
        Else
            ' Tidak ada selection di ListBox -> coba parse nama dari TxtNama (sebelumnya fallback)
            If originalInput.Contains("*"c) Then
                Dim inputParts As String() = originalInput.Split("*"c)
                If inputParts.Length >= 3 Then
                    SetQtyAndSatuan(inputParts(0).Trim(), inputParts(1).Trim())
                    namayangdiambil = String.Join("*", inputParts, 2, inputParts.Length - 2).Trim()
                ElseIf inputParts.Length = 2 Then
                    SetQtyOnly(inputParts(0).Trim())
                    namayangdiambil = inputParts(1).Trim()
                End If
            Else
                MessageBox.Show("Silakan pilih barang terlebih dahulu!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        End If

        ' Jika setelah semua langkah belum ada nama, batalkan
        If String.IsNullOrEmpty(namayangdiambil) Then
            MessageBox.Show("Nama barang tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Panggil fungsi dengan nama yang telah diproses
        Ambildatalaindaridbbarang(namayangdiambil)
    End Sub

    Private Sub Ambildatalaindaridbbarang(ByVal namayangdiambil As String)
        Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                        "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                        "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
                        "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, " &
                        "HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
                        "FROM tbl_barang " &
                        "WHERE TRIM(NAMA_BARANG) = @NamaBarang OR BARCODE_KECIL = @NamaBarang OR BARCODE_SEDANG = @NamaBarang OR BARCODE_BESAR = @NamaBarang LIMIT 1"

        Dim idBarang As String = ""
        Dim hargaBeli As String = ""
        Dim satuan As String = ""
        Dim isiSatuan As Integer = 1
        Dim hargaJual As Decimal = 0

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NamaBarang", namayangdiambil)

            Try
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        ' ✅ BATCH 2: SafeGetValue untuk standardisasi DBNull handling
                        idBarang = SafeGetValue(Of String)(rd, "ID_BARANG", String.Empty)
                        hargaBeli = SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D).ToString()

                        ' Tentukan satuan, isi, dan harga jual berdasarkan jenis pelanggan
                        Dim jenisPelanggan As String = LblJenisPl.Text
                        Dim levelSatuan As Integer

                        If Not Integer.TryParse(TxtLevelSat.Text.Trim(), levelSatuan) Then
                            levelSatuan = 1
                        End If

                        Dim barcodeInput As String = TxtBarcode.Text

                        ' Loop untuk mencari satuan yang tersedia dari besar ke kecil
                        For level As Integer = 3 To 1 Step -1
                            ' Cek apakah barcode cocok atau level sesuai
                            Dim useThisLevel As Boolean = False

                            If Not String.IsNullOrEmpty(barcodeInput) Then
                                ' Cek berdasarkan barcode
                                If level = 2 AndAlso barcodeInput = rd("BARCODE_SEDANG").ToString() Then
                                    useThisLevel = True
                                ElseIf level = 3 AndAlso barcodeInput = rd("BARCODE_BESAR").ToString() Then
                                    useThisLevel = True
                                ElseIf level = 1 AndAlso barcodeInput = rd("BARCODE_KECIL").ToString() Then
                                    useThisLevel = True
                                End If
                            Else
                                ' Cek berdasarkan level
                                useThisLevel = (level = levelSatuan)
                            End If

                            If useThisLevel Then
                                If jenisPelanggan = "Partai" Then
                                    ' ✅ BATCH 2: SafeGetValue untuk standardisasi satuan & harga
                                    ' Cek satuan partai
                                    Select Case level
                                        Case 3
                                            satuan = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_BESAR", "")
                                            isiSatuan = SafeGetValue(Of Integer)(rd, "ISI_PARTAI_BESAR", 1)
                                            hargaJual = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_BESAR", 0D)
                                        Case 2
                                            satuan = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_SEDANG", "")
                                            isiSatuan = SafeGetValue(Of Integer)(rd, "ISI_PARTAI_SEDANG", 1)
                                            hargaJual = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_SEDANG", 0D)
                                        Case 1
                                            satuan = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_KECIL", "")
                                            isiSatuan = SafeGetValue(Of Integer)(rd, "ISI_PARTAI_KECIL", 1)
                                            hargaJual = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_KECIL", 0D)
                                    End Select
                                Else
                                    ' Cek satuan umum
                                    Select Case level
                                        Case 3
                                            satuan = SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                                            isiSatuan = SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1)
                                            hargaJual = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_BESAR", 0D)
                                        Case 2
                                            satuan = SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                                            isiSatuan = SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1)
                                            hargaJual = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_SEDANG", 0D)
                                        Case 1
                                            satuan = SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                                            isiSatuan = SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1)
                                            hargaJual = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_KECIL", 0D)
                                    End Select
                                End If

                                ' Jika satuan ditemukan dan tidak kosong, keluar dari loop
                                If Not String.IsNullOrEmpty(satuan) Then
                                    Exit For
                                End If
                            End If
                        Next

                        ' Jika setelah loop satuan masih kosong, gunakan satuan kecil
                        If String.IsNullOrEmpty(satuan) Then
                            If jenisPelanggan = "Partai" Then
                                satuan = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_KECIL", "")
                                isiSatuan = SafeGetValue(Of Integer)(rd, "ISI_PARTAI_KECIL", 1)
                                hargaJual = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_PARTAI_KECIL", 0D)
                            Else
                                satuan = SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                                isiSatuan = SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1)
                                hargaJual = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL_UMUM_KECIL", 0D)
                            End If
                        End If

                        ' Ambil stok
                        Dim stokToko As String = SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D).ToString()
                        Dim stokGudang As String = SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D).ToString()

                        TxtStokToko.Text = stokToko
                        TXtStokGudang.Text = stokGudang

                        If LblLokasiBarang.Text = "GUDANG" Then
                            TxtStok.Text = stokGudang
                        ElseIf LblLokasiBarang.Text = "TOKO" Then
                            TxtStok.Text = stokToko
                        End If
                    Else
                        ' DATA TIDAK DITEMUKAN
                        MessageBox.Show("Barang '" & namayangdiambil & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        KosongTxtboxcari()
                        TxtNama.Focus()
                        Return
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Error membaca data barang: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
        End Using

        ' SETELAH READER DITUTUP, SET NILAI KE TEXTBOX
        TxtKode.Text = idBarang
        TxtHargaBeli.Text = hargaBeli
        Txtsatuan.Text = satuan
        TxtIsi.Text = isiSatuan.ToString()
        TxtHargaJual.Text = hargaJual.ToString()

        ' JANGAN TAMPILKAN LISTBOX - LANGSUNG TAMBAH KE DATAGRID
        LstBarang.Items.Clear()
        LstBarang.Visible = False

        ' Panggil TambahDataLangsung untuk menambah ke DataGridView
        TambahDataLangsung(namayangdiambil)
    End Sub


    Private Sub TambahDataLangsung(ByVal namayangdiambil As String)
        ' CEK DUPLIKAT
        If Kodebarangsama = "Tidak" Then
            For Each row As DataGridViewRow In DgvData.Rows
                If row.Cells("Kode").Value IsNot Nothing AndAlso row.Cells("Kode").Value.ToString() = TxtKode.Text Then
                    MessageBox.Show(namayangdiambil & " sudah ada dalam daftar!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    If LstBarang.Visible = True Then
                        LstBarang.Select()
                    Else
                        SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
                    End If

                    Return
                End If
            Next
        End If

        ' TAMBAH KE DATAGRID
        Dim indeksBaris As Integer
        If DgvData.SelectedCells.Count > 0 Then
            indeksBaris = DgvData.SelectedCells(0).RowIndex
            DgvData.Rows.Insert(indeksBaris, "")
        Else
            indeksBaris = DgvData.Rows.Add()
        End If

        ' BACA SATUAN DARI DATABASE DALAM BLOK TERPISAH
        Dim isPartai As Boolean = LblJenisPl.Text = "Partai"
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(DgvData.Rows(indeksBaris).Cells("Satuan"), DataGridViewComboBoxCell)
        kolomSatuan.Items.Clear()

        Dim querySatuanPartai As String = "SELECT DISTINCT SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR FROM tbl_barang WHERE ID_BARANG = @ID"
        Dim querySatuanUmum As String = "SELECT DISTINCT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = @ID"

        Dim query As String = If(isPartai, querySatuanPartai, querySatuanUmum)

        ' BACA SATUAN
        Using cmdSatuan As New MySqlCommand(query, conn)
            cmdSatuan.Parameters.AddWithValue("@ID", TxtKode.Text)

            Try
                Using rdSatuan As MySqlDataReader = cmdSatuan.ExecuteReader()
                    If rdSatuan.HasRows Then
                        While rdSatuan.Read()
                            Dim satuanKecil As String = If(rdSatuan(0) IsNot DBNull.Value, rdSatuan(0).ToString(), String.Empty)
                            Dim satuanSedang As String = If(rdSatuan(1) IsNot DBNull.Value, rdSatuan(1).ToString(), String.Empty)
                            Dim satuanBesar As String = If(rdSatuan(2) IsNot DBNull.Value, rdSatuan(2).ToString(), String.Empty)

                            If Not String.IsNullOrEmpty(satuanKecil) Then
                                kolomSatuan.Items.Add(satuanKecil)
                            End If
                            If Not String.IsNullOrEmpty(satuanSedang) Then
                                kolomSatuan.Items.Add(satuanSedang)
                            End If
                            If Not String.IsNullOrEmpty(satuanBesar) Then
                                kolomSatuan.Items.Add(satuanBesar)
                            End If
                        End While
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Error membaca satuan: " & ex.Message)
            End Try
        End Using

        ' ISI DATAGRID
        Dim hargaBeli As Decimal = ParseDecimal(TxtHargaBeli.Text)
        Dim qty As Decimal = ParseDecimal(TxtQty.Text)
        Dim isi As Decimal = ParseDecimal(TxtIsi.Text)
        Dim hargajual As Decimal = ParseDecimal(TxtHargaJual.Text)
        Dim stoktoko As Decimal = ParseDecimal(TxtStokToko.Text)
        Dim stokgudang As Decimal = ParseDecimal(TXtStokGudang.Text)
        Dim stok As Decimal = ParseDecimal(TxtStok.Text)

        DgvData.Rows(indeksBaris).Cells("Kode").Value = TxtKode.Text
        DgvData.Rows(indeksBaris).Cells("NamaBarang").Value = namayangdiambil
        DgvData.Rows(indeksBaris).Cells("HargaBeli").Value = hargaBeli
        DgvData.Rows(indeksBaris).Cells("QTY").Value = qty

        If Txtsatuan.Text = "" Then
            DgvData.Rows(indeksBaris).Cells("Satuan").Value = If(kolomSatuan.Items.Count > 0, kolomSatuan.Items(0).ToString(), "")
        Else
            DgvData.Rows(indeksBaris).Cells("Satuan").Value = Txtsatuan.Text
        End If

        DgvData.Rows(indeksBaris).Cells("Isi").Value = isi
        DgvData.Rows(indeksBaris).Cells("Totalhargabeli").Value = hargaBeli * isi * qty
        DgvData.Rows(indeksBaris).Cells("Harga").Value = hargajual
        DgvData.Rows(indeksBaris).Cells("QtySat").Value = qty * isi
        DgvData.Rows(indeksBaris).Cells("DiskonPersen").Value = 0
        DgvData.Rows(indeksBaris).Cells("DiskonRp").Value = 0
        DgvData.Rows(indeksBaris).Cells("TotalDiskon").Value = 0
        DgvData.Rows(indeksBaris).Cells("TotalHarga").Value = hargajual * qty
        DgvData.Rows(indeksBaris).Cells("StokToko").Value = stoktoko
        DgvData.Rows(indeksBaris).Cells("StokGudang").Value = stokgudang
        DgvData.Rows(indeksBaris).Cells("Stok").Value = stok

        ' ✅ PERBAIKAN: Set read-only setelah data diisi
        DgvData.Rows(indeksBaris).Cells("NamaBarang").ReadOnly = True
        DgvData.Rows(indeksBaris).Cells("NamaBarang").Style.BackColor = Color.LightGray
        DgvData.Rows(indeksBaris).Cells("NamaBarang").Style.ForeColor = Color.Black

        UpdateSemuaTotal()
        KosongTxtboxcari()

        SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
    End Sub

    Private Sub HitungNilaiSetiapBaris(ByVal indeksBaris As Integer)
        Dim row = DgvData.Rows(indeksBaris)

        Dim hargaBeli As Decimal = ParseDecimal(row.Cells("HargaBeli").Value)
        Dim qtyBarang As Decimal = ParseDecimal(row.Cells("QTY").Value)
        Dim isiBarang As Integer = CInt(ParseDecimal(row.Cells("Isi").Value))
        Dim hargaJual As Decimal = ParseDecimal(row.Cells("Harga").Value)
        Dim diskonRp As Decimal = ParseDecimal(row.Cells("DiskonRp").Value)

        Dim totalDiskon As Decimal = qtyBarang * diskonRp
        Dim totalHargaBeli As Decimal = qtyBarang * hargaBeli * isiBarang
        Dim qtySat As Decimal = qtyBarang * isiBarang
        Dim totalHarga As Decimal = hargaJual * qtyBarang - totalDiskon

        row.Cells("TotalDiskon").Value = totalDiskon
        row.Cells("TotalHargaBeli").Value = totalHargaBeli
        row.Cells("QtySat").Value = qtySat
        row.Cells("TotalHarga").Value = totalHarga
    End Sub

    ' Fungsi untuk mengonversi objek menjadi angka desimal

    Private Sub DgvDataData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellEndEdit
        If e.ColumnIndex = 1 Then 'nama
            '=======================================
            If Not String.IsNullOrEmpty(DgvData.Rows(e.RowIndex).Cells("NamaBarang").Value) Then
                Dim inputText As String = DgvData.Rows(e.RowIndex).Cells("NamaBarang").Value.ToString().Trim()
                Dim qtyValue As Decimal = 1
                Dim namaBarangValue As String = inputText

                ' Cek apakah ada tanda bintang
                Dim indexAsteriskQty As Integer = inputText.IndexOf("*")
                Dim indexAsteriskHarga As Integer = -1

                If indexAsteriskQty >= 0 Then
                    indexAsteriskHarga = inputText.IndexOf("*", indexAsteriskQty + 1)
                End If

                If indexAsteriskQty >= 0 AndAlso indexAsteriskHarga > indexAsteriskQty Then
                    ' Format: qty * harga * namaBarang → Ambil qty dan namaBarang, abaikan harga
                    Dim angkaQty As String = inputText.Substring(0, indexAsteriskQty).Trim()
                    qtyValue = ParseDecimal(angkaQty) ' Gunakan fungsi yang sudah dibuat
                    namaBarangValue = inputText.Substring(indexAsteriskHarga + 1).Trim()

                ElseIf indexAsteriskQty >= 0 Then
                    ' Format: qty * namaBarang
                    Dim angkaQty As String = inputText.Substring(0, indexAsteriskQty).Trim()
                    qtyValue = ParseDecimal(angkaQty) ' Gunakan fungsi yang sudah dibuat
                    namaBarangValue = inputText.Substring(indexAsteriskQty + 1).Trim()
                End If


                ' Update kembali ke datagrid kolom NamaBarang setelah parsing
                DgvData.Rows(e.RowIndex).Cells("NamaBarang").Value = namaBarangValue

                Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                    "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                    "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
                    "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, " &
                    "HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
                    "FROM tbl_barang " &
                    "WHERE TRIM(NAMA_BARANG) LIKE @NamaBarang OR BARCODE_KECIL LIKE @NamaBarang OR BARCODE_SEDANG LIKE @NamaBarang OR BARCODE_BESAR LIKE @NamaBarang"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@NamaBarang", namaBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.HasRows Then
                            rd.Read() ' Lanjutkan ke data pertama
                            Dim comboCell As DataGridViewComboBoxCell = CType(DgvData.Rows(DgvData.CurrentCell.RowIndex).Cells(Satuan.Index), DataGridViewComboBoxCell)
                            comboCell.Items.Clear()
                            If namaBarangValue = rd("NAMA_BARANG") OrElse
                                namaBarangValue = rd("BARCODE_KECIL") OrElse
                                namaBarangValue = rd("BARCODE_SEDANG") OrElse
                                namaBarangValue = rd("BARCODE_BESAR") Then

                                ' Set kolom kode dan harga beli
                                DgvData.Rows(e.RowIndex).Cells("Kode").Value = rd("ID_BARANG")
                                DgvData.Rows(e.RowIndex).Cells("HARGABELI").Value = rd("HARGA_BELI")

                                ' Tentukan awalan field berdasarkan jenis transaksi
                                Dim prefix As String = If(LblJenisPl.Text = "Partai", "PARTAI", "UMUM")

                                ' Ambil satuan
                                ' ✅ BATCH 2: SafeGetValue untuk standardisasi satuan reading
                                Dim satuanKecil = SafeGetValue(Of String)(rd, "SATUAN_" & prefix & "_KECIL", "")
                                Dim satuanSedang = SafeGetValue(Of String)(rd, "SATUAN_" & prefix & "_SEDANG", "")
                                Dim satuanBesar = SafeGetValue(Of String)(rd, "SATUAN_" & prefix & "_BESAR", "")

                                ' Tambahkan satuan ke combo
                                If Not String.IsNullOrEmpty(satuanKecil) Then
                                    comboCell.Items.Add(satuanKecil)
                                End If

                                If Not String.IsNullOrEmpty(satuanSedang) Then
                                    comboCell.Items.Add(satuanSedang)
                                End If

                                If Not String.IsNullOrEmpty(satuanBesar) Then
                                    comboCell.Items.Add(satuanBesar)
                                End If

                                ' Tentukan isi, satuan, dan harga jual berdasarkan barcode
                                Dim isiField As String = "1"
                                Dim hargaField As String = ""
                                Dim satuanValue As String = satuanKecil ' default

                                If namaBarangValue = rd("BARCODE_SEDANG").ToString() Then
                                    satuanValue = satuanSedang
                                    isiField = "ISI_" & prefix & "_SEDANG"
                                    hargaField = "HARGA_JUAL_" & prefix & "_SEDANG"
                                ElseIf namaBarangValue = rd("BARCODE_BESAR").ToString() Then
                                    satuanValue = satuanBesar
                                    isiField = "ISI_" & prefix & "_BESAR"
                                    hargaField = "HARGA_JUAL_" & prefix & "_BESAR"
                                Else
                                    satuanValue = satuanKecil
                                    isiField = "ISI_" & prefix & "_KECIL"
                                    hargaField = "HARGA_JUAL_" & prefix & "_KECIL"
                                End If

                                ' Update data grid
                                DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuanValue
                                DgvData.Rows(e.RowIndex).Cells("isi").Value = rd(isiField)
                                DgvData.Rows(e.RowIndex).Cells("harga").Value = rd(hargaField)
                                DgvData.Rows(e.RowIndex).Cells("qty").Value = qtyValue
                                DgvData.Rows(e.RowIndex).Cells("NamaBarang").Value = rd("NAMA_BARANG")


                                ' ✅ PERBAIKAN: Set read-only SETELAH data dari DB diisi
                                DgvData.Rows(e.RowIndex).Cells("Kode").Value = rd("ID_BARANG")
                                DgvData.Rows(e.RowIndex).Cells("NamaBarang").ReadOnly = True
                                DgvData.Rows(e.RowIndex).Cells("NamaBarang").Style.BackColor = Color.LightGray
                                DgvData.Rows(e.RowIndex).Cells("NamaBarang").Style.ForeColor = Color.Black
                            End If

                            DgvData.Rows(e.RowIndex).Cells("StokToko").Value = SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                            DgvData.Rows(e.RowIndex).Cells("StokGudang").Value = SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                            If LblLokasiBarang.Text = "GUDANG" Then
                                DgvData.Rows(e.RowIndex).Cells("Stok").Value = DgvData.Rows(e.RowIndex).Cells("StokGudang").Value
                            ElseIf LblLokasiBarang.Text = "TOKO" Then
                                DgvData.Rows(e.RowIndex).Cells("Stok").Value = DgvData.Rows(e.RowIndex).Cells("StokToko").Value
                            End If

                            If Not IsDBNull(DgvData.Rows(e.RowIndex).Cells("isi").Value) AndAlso
   Convert.ToInt32(DgvData.Rows(e.RowIndex).Cells("isi").Value) = 0 Then

                                DgvData.Rows(e.RowIndex).Cells("qtysat").Value = 1
                            End If


                            If Kodebarangsama = "Tidak" Then
                                ' Hapus data ganda
                                For barisatas As Integer = 0 To DgvData.RowCount - 1
                                    For barisbawah As Integer = barisatas + 1 To DgvData.RowCount - 1
                                        Dim kodeBarisAtas As Object = DgvData.Rows(barisatas).Cells("Kode").Value
                                        Dim kodeBarisBawah As Object = DgvData.Rows(barisbawah).Cells("Kode").Value

                                        If kodeBarisAtas IsNot Nothing AndAlso kodeBarisBawah IsNot Nothing AndAlso kodeBarisBawah.Equals(kodeBarisAtas) Then
                                            ' Mengupdate nilai Qty
                                            DgvData.Rows(barisatas).Cells("Qty").Value = If(IsDBNull(DgvData.Rows(barisatas).Cells("Qty").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("Qty").Value)) + 1

                                            ' Mengambil nilai isi sebagai Integer dan qty sebagai Decimal
                                            Dim IsiAtas As Integer = If(IsDBNull(DgvData.Rows(barisatas).Cells("isi").Value), 0, Convert.ToInt32(DgvData.Rows(barisatas).Cells("isi").Value))
                                            Dim QtyAtas As Decimal = If(IsDBNull(DgvData.Rows(barisatas).Cells("qty").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("qty").Value))

                                            ' Mengupdate qtysat
                                            If IsiAtas = 0 Then
                                                DgvData.Rows(barisatas).Cells("qtysat").Value = If(IsDBNull(DgvData.Rows(barisatas).Cells("qtysat").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("qtysat").Value)) + 1
                                            Else
                                                DgvData.Rows(barisatas).Cells("qtysat").Value = IsiAtas * QtyAtas
                                            End If

                                            ' Mengupdate TotalHarga
                                            Dim hargaValue As Decimal = If(IsDBNull(DgvData.Rows(barisatas).Cells("Harga").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("Harga").Value))
                                            Dim totalDiskonValue As Decimal = If(IsDBNull(DgvData.Rows(barisatas).Cells("TotalDiskon").Value), 0D, ParseDecimal(DgvData.Rows(barisatas).Cells("TotalDiskon").Value))
                                            DgvData.Rows(barisatas).Cells("TotalHarga").Value = (hargaValue * QtyAtas) - totalDiskonValue

                                            ' Menghapus baris jika bukan baris baru
                                            If Not DgvData.Rows(barisbawah).IsNewRow Then
                                                DgvData.Rows.RemoveAt(barisbawah)
                                            End If

                                            ' Menggeser fokus ke bawah
                                            SendKeys.Send("{down}")

                                            HitungNilaiSetiapBaris(e.RowIndex)
                                            UpdateSemuaTotal()

                                            Exit Sub
                                        End If
                                    Next
                                Next
                            End If

                            HitungNilaiSetiapBaris(e.RowIndex)

                        Else
                            rd.Close()

                            DgvData.Rows(e.RowIndex).Cells("namabarang").Value = ""
                            SendKeys.Send("{down}")


                        End If

                    End Using
                End Using

            End If
        End If

        '========================== qty
        If e.ColumnIndex = 3 Then
            Dim qtyCellValue As String = If(DgvData.Rows(e.RowIndex).Cells("qty").Value IsNot Nothing, DgvData.Rows(e.RowIndex).Cells("qty").Value.ToString().Trim(), "")


            ' Validasi hanya angka dan satu koma atau titik
            Dim isValid As Boolean = System.Text.RegularExpressions.Regex.IsMatch(qtyCellValue, "^\d+([,.]\d+)?$")

            If Not isValid Then
                MessageBox.Show("Qty hanya boleh angka dan satu tanda koma atau titik.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                DgvData.Rows(e.RowIndex).Cells("qty").Value = "1"
            Else
                ' Ubah koma ke titik untuk standar database
                qtyCellValue = qtyCellValue.Replace(",", ".")
                DgvData.Rows(e.RowIndex).Cells("qty").Value = qtyCellValue
            End If

            HitungNilaiSetiapBaris(e.RowIndex)
        End If


        '========================== harga jual
        If e.ColumnIndex = 7 Then
            Dim hargaCellValue As String = DgvData.Rows(e.RowIndex).Cells("harga").Value

            Dim harga As Decimal
            Dim diskonRp As Decimal = DgvData.Rows(e.RowIndex).Cells("DiskonRp").Value

            If Not Decimal.TryParse(hargaCellValue, harga) Then
                MessageBox.Show("Harga harus berupa angka. Mohon periksa kembali.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                harga = 0
            End If

            ' Hitung diskon persen
            Dim diskonPersen As Decimal = If(harga > 0, (diskonRp / harga) * 100, 0)
            DgvData.Rows(e.RowIndex).Cells("diskonpersen").Value = diskonPersen

            HitungNilaiSetiapBaris(e.RowIndex)


            ' Jika harga jual diubah, maka akan muncul edit master barang
            If Editmasterhargajual = "Iya" Then
                Dim idBarangObj = DgvData.Rows(e.RowIndex).Cells("Kode").Value
                If idBarangObj IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(idBarangObj.ToString()) Then
                    Dim idBarang As String = idBarangObj.ToString()
                    Dim hargaJual As Decimal = ParseDecimal(DgvData.Rows(e.RowIndex).Cells("harga").Value)
                    Dim satuan As String = DgvData.Rows(e.RowIndex).Cells("Satuan").Value.ToString()
                    Dim jenispelanggan As String = If(LblJenisPl.Text = "Partai", "Partai", "Umum")
                    ' Panggil fungsi untuk mengupdate harga jual di database
                    UpdateHargaJual(idBarang, hargaJual, satuan, jenispelanggan)
                End If
            End If

        End If


        '========================== diskonpersen
        If e.ColumnIndex = 9 Then
            Dim diskonPersenCellValue As String = DgvData.Rows(e.RowIndex).Cells("Diskonpersen").Value

            Dim harga As Decimal = If(IsDBNull(DgvData.Rows(e.RowIndex).Cells("harga").Value), 0D, ParseDecimal(DgvData.Rows(e.RowIndex).Cells("harga").Value))
            Dim diskonPersen As Decimal

            If Not Decimal.TryParse(diskonPersenCellValue, diskonPersen) Then
                MessageBox.Show("Diskon persen harus berupa angka. Mohon periksa kembali.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                diskonPersen = 0
            End If

            ' Hitung Diskon Rp
            Dim diskonRp As Decimal = harga * diskonPersen / 100
            DgvData.Rows(e.RowIndex).Cells("DiskonRp").Value = diskonRp

            HitungNilaiSetiapBaris(e.RowIndex)
        End If

        '========================== diskonrp
        If e.ColumnIndex = 10 Then
            Dim diskonRpCellValue As String = DgvData.Rows(e.RowIndex).Cells("DiskonRp").Value

            Dim harga As Decimal = If(IsDBNull(DgvData.Rows(e.RowIndex).Cells("harga").Value), 0D, ParseDecimal(DgvData.Rows(e.RowIndex).Cells("harga").Value))
            Dim diskonRp As Decimal

            If Not Decimal.TryParse(diskonRpCellValue, diskonRp) Then
                MessageBox.Show("Diskon Rp harus berupa angka. Mohon periksa kembali.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                diskonRp = 0
            End If


            ' Hitung diskon persen
            Dim diskonPersen As Decimal = If(harga > 0, (diskonRp / harga) * 100, 0)
            DgvData.Rows(e.RowIndex).Cells("diskonpersen").Value = diskonPersen

            HitungNilaiSetiapBaris(e.RowIndex)
        End If


        If Not String.IsNullOrEmpty(DgvData.Rows(e.RowIndex).Cells("Kode").Value) Then
            UpdateSemuaTotal()
        End If


    End Sub


    Private Sub UpdateHargaJual(ByVal idBarang As String, ByVal hargaJual As Decimal, ByVal satuan As String, ByVal jenispelanggan As String)
        With TambahBarang
            .LblUtama.Text = "EDIT HARGA JUAL DARI PENJUALAN"
            .GBBarcode.Visible = False
            .GBStok.Visible = False
            .GBBarang.Enabled = False
            .GBPoint.Visible = False
            .PanelInfoRubahHarga.Visible = False
            .BtnTambahKategori.Visible = False
            .BtnSupliyer.Visible = False
            .BtnTambahSatuan.Visible = False
            .CBManual.Visible = False
            .BtnBaru.Visible = False
            '.BackColor = Color.DarkCyan
            .Size = New Size(825, 630)
            TambahBarang.Tampilkategori()
            TambahBarang.TampilSatuan()
            TambahBarang.Tampilsupliyer()
            .TxtKode.Text = idBarang
            .LblHargaDrJual.Text = hargaJual
            .LblsatuanDrJual.Text = satuan
            .LblJenisDrJual.Text = jenispelanggan
            .ShowDialog()
        End With
    End Sub


    Private Sub DgvData_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DgvData.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DgvData.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    Private Sub DgvData_EditingControlShowing(ByVal sender As Object, ByVal e As DataGridViewEditingControlShowingEventArgs) Handles DgvData.EditingControlShowing
        If DgvData.CurrentCell.ColumnIndex = 1 AndAlso DgvData.Columns(1).HeaderText = "Nama Barang" Then
            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.Suggest
                autoText.AutoCompleteSource = AutoCompleteSource.CustomSource
                Dim DataCollection As New AutoCompleteStringCollection()

                ' Mendapatkan teks dari TextBox autoText
                Dim searchTerm As String = autoText.Text

                ' Memanggil AddItems dengan nilai pencarian dari TextBox
                AddItems(DataCollection, searchTerm)

                autoText.AutoCompleteCustomSource = DataCollection
            End If
        End If

        If DgvData.CurrentCell.ColumnIndex = 4 Then
            If TypeOf e.Control Is ComboBox Then
                Dim comboBox As ComboBox = DirectCast(e.Control, ComboBox)
                RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
                AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
            End If
        End If
    End Sub

    Public Sub AddItems(col As AutoCompleteStringCollection, searchTerm As String)
        Using cmd As New MySqlCommand("SELECT NAMA_BARANG FROM tbl_barang WHERE NAMA_BARANG LIKE @term", conn)
            cmd.Parameters.AddWithValue("@term", "%" & searchTerm & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim nama As String = rd("NAMA_BARANG").ToString()
                    If Not col.Contains(nama) Then
                        col.Add(nama)
                    End If
                End While
            End Using
        End Using
    End Sub



    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox As ComboBox = DirectCast(sender, ComboBox)
        Dim cell As DataGridViewComboBoxCell = DirectCast(DgvData.CurrentCell, DataGridViewComboBoxCell)
        If LblJenisPl.Text = "Partai" Then
            Using cmd As New MySqlCommand("select ID_BARANG,ISI_PARTAI_KECIL,HARGA_JUAL_PARTAI_KECIL,ISI_PARTAI_SEDANG,HARGA_JUAL_PARTAI_SEDANG,ISI_PARTAI_BESAR,HARGA_JUAL_PARTAI_BESAR from tbl_barang WHERE ID_BARANG = ?", conn)
                cmd.Parameters.AddWithValue("@ID_BARANG", cell.OwningRow.Cells("Kode").Value)
                Using rd As MySqlDataReader = cmd.ExecuteReader
                    If rd.Read() Then
                        If comboBox.SelectedIndex = 0 Then
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_PARTAI_KECIL")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_PARTAI_KECIL")
                        ElseIf comboBox.SelectedIndex = 1 Then
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_PARTAI_SEDANG")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_PARTAI_SEDANG")
                        Else
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_PARTAI_BESAR")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_PARTAI_BESAR")
                        End If
                    Else
                        MessageBox.Show("Satuan barang dan atau harga jual belum di input ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End Using
            End Using
        Else
            Using cmd As New MySqlCommand("select ID_BARANG,ISI_UMUM_KECIL,HARGA_JUAL_UMUM_KECIL, ISI_UMUM_SEDANG,HARGA_JUAL_UMUM_SEDANG, ISI_UMUM_BESAR,HARGA_JUAL_UMUM_BESAR from tbl_barang WHERE ID_BARANG = ?", conn)
                cmd.Parameters.AddWithValue("@ID_BARANG", cell.OwningRow.Cells("Kode").Value)
                Using rd As MySqlDataReader = cmd.ExecuteReader
                    If rd.Read() Then
                        ' Update the contents of the adjacent text box column based on the selected option in the combo box
                        If comboBox.SelectedIndex = 0 Then
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_UMUM_KECIL")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_UMUM_KECIL")
                        ElseIf comboBox.SelectedIndex = 1 Then
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_UMUM_SEDANG")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_UMUM_SEDANG")
                        Else
                            cell.OwningRow.Cells("Isi").Value = rd.Item("ISI_UMUM_BESAR")
                            cell.OwningRow.Cells("Harga").Value = rd.Item("HARGA_JUAL_UMUM_BESAR")
                        End If
                    Else
                        MessageBox.Show("Satuan barang dan atau harga jual belum di input ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End Using
            End Using
        End If
        Dim rowI As Integer = DgvData.CurrentCell.RowIndex
        HitungNilaiSetiapBaris(rowI)
        UpdateSemuaTotal()
    End Sub


    Private Sub DgvData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles DgvData.KeyDown
        If e.KeyCode = Keys.Delete Then
            If DgvData.SelectedCells.Count > 0 Then
                Dim selectedCell As DataGridViewCell = DgvData.SelectedCells(0)

                ' Periksa apakah sel yang dipilih berada di kolom "Nama"
                If selectedCell.ColumnIndex = DgvData.Columns("NamaBarang").Index Then
                    Dim rowIndex As Integer = selectedCell.RowIndex

                    ' Periksa apakah nilai di kolom "Nama" tidak kosong
                    If Not String.IsNullOrEmpty(DgvData.Rows(rowIndex).Cells("NamaBarang").Value.ToString()) Then
                        ' Hapus baris jika nilai di kolom "Nama" tidak kosong
                        Hapusbaris()
                        ' Setelah menghapus baris, pastikan untuk menghilangkan seleksi agar tidak ada baris yang dipilih secara default.
                        DgvData.ClearSelection()
                    Else
                        MessageBox.Show("Klik kanan pada baris yang tidak kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If
            UpdateSemuaTotal()
        End If

    End Sub

    Private Sub DgvData_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DgvData.CellMouseUp
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' ✅ PERBAIKAN: Izinkan hapus meskipun read-only via klik kanan
            Dim cell As DataGridViewCell = DgvData.Rows(e.RowIndex).Cells("NamaBarang")
            If cell IsNot Nothing AndAlso cell.Value IsNot Nothing Then
                ' Periksa apakah nilai di kolom "Nama" pada baris yang diklik tidak kosong
                Dim namaValue As String = cell.Value.ToString()
                If Not String.IsNullOrEmpty(namaValue) Then
                    ' Setel sel saat ini ke sel "Nama"
                    DgvData.CurrentCell = cell

                    ' Tampilkan ContextMenuStrip di lokasi kursor
                    Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
                    ContextMenuStrip1.Show(cursorPosition)
                End If
            End If
        End If
    End Sub


    Private Sub DgvData_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellEnter
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' ✅ PERBAIKAN: Kontrol read-only berdasarkan kolom Kode
            If e.ColumnIndex = 1 Then ' Kolom NamaBarang (index 1)
                Dim kodeValue = DgvData.Rows(e.RowIndex).Cells("Kode").Value

                ' Jika Kode sudah terisi, buat NamaBarang read-only
                If kodeValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(kodeValue.ToString()) Then
                    DgvData.Rows(e.RowIndex).Cells("NamaBarang").ReadOnly = True
                    DgvData.Rows(e.RowIndex).Cells("NamaBarang").Style.BackColor = Color.LightGray
                    DgvData.Rows(e.RowIndex).Cells("NamaBarang").Style.ForeColor = Color.Black
                Else
                    DgvData.Rows(e.RowIndex).Cells("NamaBarang").ReadOnly = False
                    DgvData.Rows(e.RowIndex).Cells("NamaBarang").Style.BackColor = Color.White
                    DgvData.Rows(e.RowIndex).Cells("NamaBarang").Style.ForeColor = Color.Black
                End If
            End If

            Dim colName As String = DgvData.Columns(e.ColumnIndex).Name
            SetRichTextBoxWithLimitedTooltip(rtbPetunjuk, colName)
        Else
            rtbPetunjuk.Clear()
        End If
    End Sub

    Private Sub DgvData_Leave(sender As Object, e As EventArgs) Handles DgvData.Leave
        rtbPetunjuk.Clear()
        'rtbPetunjuk.SelectionColor = Color.Black
        'rtbPetunjuk.Text = "🔍 Arahkan ke kolom untuk melihat petunjuk penggunaan."
    End Sub

    Private Sub SetRichTextBoxWithLimitedTooltip(rtb As RichTextBox, colName As String)
        Dim fullText As String = GetTooltipForColumn(colName)
        If String.IsNullOrEmpty(fullText) Then
            rtb.Clear()
            Return
        End If

        Dim lines() As String = fullText.Split(New String() {vbCrLf}, StringSplitOptions.None)

        ' Baris pertama = judul + icon
        Dim judul As String = lines(0)

        ' Batasi petunjuk maksimal 2 baris setelah judul
        Dim petunjukLines As String = ""
        If lines.Length > 1 Then
            Dim maxLines = Math.Min(2, lines.Length - 1)
            petunjukLines = String.Join(vbCrLf, lines, 1, maxLines)
        End If

        ' Bersihkan dulu
        rtb.Clear()

        ' Set warna sama untuk semua teks
        rtb.SelectionColor = Color.DarkBlue

        ' Tulis judul + petunjuk
        rtb.AppendText(judul & vbCrLf)
        If petunjukLines <> "" Then
            rtb.AppendText(petunjukLines)
        End If

        ' Reset selection supaya tidak terselect
        rtb.SelectionStart = 0
        rtb.SelectionLength = 0
    End Sub


    Private Function GetTooltipForColumn(colName As String) As String
        Select Case colName
            Case "NamaBarang"
                Return "📦 Kolom Nama Barang: Bisa diketik atau scan barcode. Gunakan 🔼/🔽 untuk navigasi, ↵ ENTER untuk memilih." & vbCrLf &
          "✍️ Untuk isi Qty secara otomatis bisa menggunakan format qty*barcode : contoh ketik 2* lalu scan → kolom QTY langsung terisi 2."


            Case "QTY"
                Return "🔢 Kolom Qty: Jumlah unit/barang yang dibeli." & vbCrLf &
                   "Pastikan isi angka yang valid."

            Case "Satuan"
                Return "📏 Kolom Satuan: Satuan barang, misal PCS, DUS, atau PACK." & vbCrLf &
                   "Gunakan satuan yang sesuai produk."

            Case "Harga"
                Return "💰 Kolom Harga: Harga jual per unit sebelum diskon." & vbCrLf &
                   "Pastikan harga sudah benar."

            Case "DiskonPersen"
                Return "🔻 Kolom Diskon %: Diskon dalam persen dari harga jual." & vbCrLf &
                   "Masukkan nilai antara 0-100."

            Case "DiskonRp"
                Return "💸 Kolom Diskon Rp: Diskon dalam nominal (potongan harga langsung)." & vbCrLf &
                   "Pastikan nominal diskon sesuai."

            Case "TotalDiskon"
                Return "💸 Kolom Total Diskon: Total diskon = Diskon × Qty." & vbCrLf &
                   "Menunjukkan potongan harga keseluruhan."

            Case "TotalHarga"
                Return "🧾 Kolom Total Harga: Total bayar = (Harga - Diskon) × Qty." & vbCrLf &
                   "Jumlah yang harus dibayar pelanggan."

            Case "StokToko"
                Return "🏪 Kolom Stok Toko: Stok barang yang tersedia di toko." & vbCrLf &
                   "Gunakan untuk cek ketersediaan barang di Toko."

            Case "StokGudang"
                Return "🏢 Kolom Stok Gudang: Stok barang yang tersedia di gudang." & vbCrLf &
                   "Gunakan untuk cek ketersediaan barang di Gudang."

            Case "SerialNumber"
                Return "🏢 Kolom S N: Di isi serial number untuk barang ber serial number." & vbCrLf &
                   "Gunakan untuk klaim garansi."

            Case Else
                Return ""
        End Select
    End Function


    Private Sub HapusToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HapusToolStripMenuItem.Click
        Call Hapusbaris()
    End Sub

    Private Sub Hapusbaris()
        ' Periksa apakah ada sel yang dipilih
        If DgvData.CurrentCell Is Nothing Then
            MsgBox("Tidak ada baris yang dipilih.", vbExclamation, "Peringatan")
            Return
        End If

        Dim baris As Integer = DgvData.CurrentCell.RowIndex

        ' Periksa apakah sel dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            MsgBox("Tidak dapat menghapus baris dalam mode edit.", vbExclamation + vbCritical, "Mode Edit Aktif")
            Return
        End If

        ' Konfirmasi penghapusan untuk baris yang berisi data
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus baris ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            DgvData.Rows.RemoveAt(baris)
            UpdateSemuaTotal()
        End If
    End Sub

    Private Sub DgvData_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DgvData.CellFormatting
        ' Pastikan kolom "StokToko" dan "StokGudang" ada
        If DgvData.Columns("StokToko") IsNot Nothing AndAlso DgvData.Columns("StokGudang") IsNot Nothing Then
            Dim stokTokoIndex As Integer = DgvData.Columns("StokToko").Index
            Dim stokGudangIndex As Integer = DgvData.Columns("StokGudang").Index

            ' Cek apakah sel yang sedang diformat adalah bagian dari "StokToko" atau "StokGudang"
            If e.ColumnIndex = stokTokoIndex OrElse e.ColumnIndex = stokGudangIndex Then
                Dim stokValue As Object = e.Value
                If stokValue IsNot Nothing AndAlso IsNumeric(stokValue) AndAlso ParseDecimal(stokValue) < 1 Then
                    ' Jika nilai stok < 1, ubah warna latar belakang menjadi merah
                    e.CellStyle.BackColor = Color.Red
                    e.CellStyle.ForeColor = Color.White ' Opsional, agar teks lebih terlihat
                End If
            End If
        End If
    End Sub

    Private Sub HitungUlangBarisIniToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HitungUlangBarisIniToolStripMenuItem.Click
        Hitungbaris()
        UpdateSemuaTotal()
    End Sub

    Private Sub DgvData_DataError(ByVal sender As Object, ByVal e As DataGridViewDataErrorEventArgs) Handles DgvData.DataError
        Dim errorMessage As String = "Kesalahan data: " & e.Exception.Message & Environment.NewLine &
                                     "Periksa baris yang disorot dan perbaiki."

        MessageBox.Show(errorMessage, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)

        ' Menyorot baris yang bermasalah
        If e.RowIndex >= 0 Then
            For Each cell As DataGridViewCell In DgvData.Rows(e.RowIndex).Cells
                cell.Style.BackColor = Color.Red
            Next
        End If

        e.Cancel = True
    End Sub

    Public Sub UpdateSemuaTotal()
        ' ✅ PHASE 2 OPTIMIZATION: Single loop instead of 4+ separate loops
        Dim totalHpp As Decimal = 0
        Dim totalGrand As Decimal = 0
        Dim totalQtyBarang As Decimal = 0
        Dim totalItemCount As Integer = 0
        Dim totalQtySat As Decimal = 0

        ' Single loop untuk semua calculations
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow Then
                ' === Hitung Total Harga Beli (HPP) ===
                If row.Cells("Totalhargabeli").Value IsNot Nothing Then
                    totalHpp += Math.Round(ParseDecimal(row.Cells("Totalhargabeli").Value))
                End If

                ' === Hitung Grand Total Harga Jual ===
                If row.Cells("Totalharga").Value IsNot Nothing Then
                    totalGrand += Math.Round(ParseDecimal(row.Cells("Totalharga").Value))
                End If

                ' === Hitung Jumlah Barang dan Jumlah Item ===
                Dim qtyObj As Object = row.Cells("Qty").Value
                If qtyObj IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtyObj.ToString()) Then
                    totalQtyBarang += Math.Round(ParseDecimal(qtyObj))
                    totalItemCount += 1
                End If

                ' === Hitung Total QTY Satuan ===
                If row.Cells("QtySat").Value IsNot Nothing Then
                    totalQtySat += Math.Round(ParseDecimal(row.Cells("QtySat").Value), 2)
                End If
            End If
        Next

        ' Set semua values sekaligus
        TxtTotalHpp.Text = totalHpp.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtTotalJualSblDiskonPajak.Text = totalGrand.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        LblJualSblDiskon.Text = totalGrand.ToString("#,0.##", cultureIndonesia)
        TxtJmlhQty.Text = totalQtyBarang.ToString()
        TxtJmlhItem.Text = totalItemCount.ToString()
        LblRecord.Text = "Total record : " & totalItemCount.ToString()
        TxtJmlhQtySatuan.Text = totalQtySat.ToString()

        ' === Scroll otomatis ke baris terakhir ===
        If DgvData.Rows.Count > 0 Then
            DgvData.FirstDisplayedScrollingRowIndex = DgvData.Rows.Count - 1
        End If
    End Sub

    Private Sub TxtTotalJualSblDiskonPajak_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtTotalJualSblDiskonPajak.TextChanged
        HitungDiskon("diskonpersen")
        HitungPajak("pajakpersen")
        HitungTotalPenjualanAkhir()

    End Sub


    ' Flag untuk mencegah loop event
    Private isUpdatingDiskon As Boolean = False

    ' Handler KeyDown untuk hanya mengizinkan angka, backspace, delete, panah, dan titik
    Private Sub TxtDiskon_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtDiskonRp.KeyDown, TxtDiskonPersen.KeyDown
        Dim allowedKeys As Keys() = {Keys.Back, Keys.Delete, Keys.Left, Keys.Right, Keys.OemPeriod}
        If (e.KeyCode < Keys.D0 OrElse e.KeyCode > Keys.D9) AndAlso
       (e.KeyCode < Keys.NumPad0 OrElse e.KeyCode > Keys.NumPad9) AndAlso
       Not allowedKeys.Contains(e.KeyCode) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    ' Handler TextChanged untuk diskon persen dan rupiah
    Private Sub TxtDiskon_TextChanged(sender As Object, e As EventArgs) Handles TxtDiskonRp.TextChanged, TxtDiskonPersen.TextChanged
        If sender Is TxtDiskonRp Then
            HitungDiskon("diskonrupiah")
        ElseIf sender Is TxtDiskonPersen Then
            HitungDiskon("diskonpersen")
        End If
    End Sub

    ' Fungsi utama untuk menghitung diskon
    Private Sub HitungDiskon(sumber As String)
        If isUpdatingDiskon Then Exit Sub
        isUpdatingDiskon = True

        Dim totalSebelumDiskon As Decimal = ParseDecimal(TxtTotalJualSblDiskonPajak.Text)
        Dim diskonPersen As Decimal = ParseDecimal(TxtDiskonPersen.Text)
        Dim diskonRupiah As Decimal = ParseDecimal(TxtDiskonRp.Text)

        Select Case sumber.ToLower()
            Case "diskonpersen"
                diskonPersen = Math.Min(diskonPersen, 100)
                diskonRupiah = Math.Round(totalSebelumDiskon * diskonPersen / 100, 0)
                ' ✅ TextBox format STANDAR (no separator) - tampilkan desimal hanya jika ada
                TxtDiskonRp.Text = diskonRupiah.ToString("0.##", Globalization.CultureInfo.InvariantCulture)

            Case "diskonrupiah"
                diskonPersen = If(totalSebelumDiskon = 0, 0, Math.Round((diskonRupiah / totalSebelumDiskon) * 100, 2))
                ' ✅ TextBox format STANDAR (no separator) - tampilkan desimal hanya jika ada
                TxtDiskonPersen.Text = diskonPersen.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        End Select

        ' ✅ Label format INDONESIA (hanya untuk display)
        LblDiskonRp.Text = "Rp. " & diskonRupiah.ToString("#,0.##", cultureIndonesia)

        HitungTotalPenjualanAkhir()
        isUpdatingDiskon = False
    End Sub



    ' Flag untuk mencegah loop event
    Private isUpdatingPajak As Boolean = False

    ' Handler KeyDown untuk hanya mengizinkan angka, backspace, delete, panah, dan titik
    Private Sub TxtPajak_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtPajakRp.KeyDown, TxtPajakPersen.KeyDown
        Dim allowedKeys As Keys() = {Keys.Back, Keys.Delete, Keys.Left, Keys.Right, Keys.OemPeriod}
        If (e.KeyCode < Keys.D0 OrElse e.KeyCode > Keys.D9) AndAlso
       (e.KeyCode < Keys.NumPad0 OrElse e.KeyCode > Keys.NumPad9) AndAlso
       Not allowedKeys.Contains(e.KeyCode) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    ' Handler TextChanged untuk pajak persen dan rupiah
    Private Sub TxtPajak_TextChanged(sender As Object, e As EventArgs) Handles TxtPajakRp.TextChanged, TxtPajakPersen.TextChanged
        If sender Is TxtPajakRp Then
            HitungPajak("pajakrupiah")
        ElseIf sender Is TxtPajakPersen Then
            HitungPajak("pajakpersen")
        End If
    End Sub

    ' Fungsi utama untuk menghitung pajak
    Private Sub HitungPajak(sumber As String)
        If isUpdatingPajak Then Exit Sub
        isUpdatingPajak = True

        Dim totalSebelumDiskon As Decimal = ParseDecimal(TxtTotalJualSblDiskonPajak.Text)
        Dim diskonRupiah As Decimal = ParseDecimal(TxtDiskonRp.Text)
        Dim totalSebelumPajak As Decimal = totalSebelumDiskon - diskonRupiah

        Dim pajakPersen As Decimal = ParseDecimal(TxtPajakPersen.Text)
        Dim pajakRupiah As Decimal = ParseDecimal(TxtPajakRp.Text)

        Select Case sumber.ToLower()
            Case "pajakpersen"
                pajakPersen = Math.Min(pajakPersen, 100)
                pajakRupiah = Math.Round(totalSebelumPajak * pajakPersen / 100, 0)
                ' ✅ TextBox format STANDAR (no separator) - tampilkan desimal hanya jika ada
                TxtPajakRp.Text = pajakRupiah.ToString("0.##", Globalization.CultureInfo.InvariantCulture)

            Case "pajakrupiah"
                pajakPersen = If(totalSebelumPajak = 0, 0, Math.Round((pajakRupiah / totalSebelumPajak) * 100, 2))
                ' ✅ TextBox format STANDAR (no separator) - tampilkan desimal hanya jika ada
                TxtPajakPersen.Text = pajakPersen.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        End Select

        ' ✅ Label format INDONESIA (hanya untuk display)
        LblPajakRp.Text = "Rp. " & pajakRupiah.ToString("#,0.##", cultureIndonesia)

        HitungTotalPenjualanAkhir()
        isUpdatingPajak = False
    End Sub


    Private Sub TxtBiayaKirim_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtBiayaKirim.TextChanged

        ' ✅ Parse dengan smart parsing (handle kedua format)
        Dim biayaKirim As Decimal = ParseDecimal(TxtBiayaKirim.Text, 0D)

        ' ✅ TextBox selalu format STANDAR - tampilkan desimal hanya jika ada
        TxtBiayaKirim.Text = biayaKirim.ToString("0.##", Globalization.CultureInfo.InvariantCulture)

        ' ✅ Label format INDONESIA (hanya untuk display)
        LblBiayaKirim.Text = biayaKirim.ToString("#,0.##", cultureIndonesia)

        HitungTotalPenjualanAkhir()

    End Sub


    Private Sub HitungTotalPenjualanAkhir()
        ' Ambil nilai dari TextBox, parsing ke format desimal internasional
        Dim totalSebelumDiskon As Decimal = ParseDecimal(TxtTotalJualSblDiskonPajak.Text)
        Dim diskonRp As Decimal = ParseDecimal(TxtDiskonRp.Text)
        Dim pajakRp As Decimal = ParseDecimal(TxtPajakRp.Text)
        Dim biayaKirim As Decimal = ParseDecimal(TxtBiayaKirim.Text)

        ' Hitung Total Setelah Pajak (total belanja + pajak + biaya kirim)
        Dim totalSetelahPajak As Decimal = (totalSebelumDiskon - diskonRp) + pajakRp + biayaKirim
        TxtTotaljualStlPajak.Text = totalSetelahPajak.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
    End Sub




    Private Sub TxtTotaljualStlPajak_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles TxtTotaljualStlPajak.TextChanged
        Dim totalStlPajak As Decimal

        If String.IsNullOrEmpty(TxtTotaljualStlPajak.Text) Or Not Decimal.TryParse(TxtTotaljualStlPajak.Text, totalStlPajak) Then
            LblTotalStlPajak.Text = "0"
            TxtGrantotal.Text = "Rp. 0"
        Else
            LblTotalStlPajak.Text = totalStlPajak.ToString("#,0.##", cultureIndonesia)
            TxtGrantotal.Text = "Rp. " & totalStlPajak.ToString("#,0.##", cultureIndonesia)
        End If
    End Sub

    Private Sub TxtKembaliHutang_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles TxtKembaliHutang.TextChanged
        LblKembali.Text = If(String.IsNullOrEmpty(TxtKembaliHutang.Text) Or Not IsNumeric(TxtKembaliHutang.Text), "0",
                     ParseDecimal(TxtKembaliHutang.Text).ToString("#,0.##", cultureIndonesia))
    End Sub

    Private Sub BtnBayar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBayar.Click
        ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            DgvData.EndEdit()
        End If

        Call TekanBayar()
    End Sub

    Private Sub BtnTahan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTahan.Click
        Call Tekantahan()
    End Sub

    Private Sub BtnPanggil_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPanggil.Click
        Call Tekanpanggil()
    End Sub

    Private Sub BtnBarang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBarang.Click
        TekanBarang()
    End Sub

    Private Sub BtnPelanggan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPelanggan.Click
        Tekanpelanggan()
    End Sub

    Private Sub BtnSimpann_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpann.Click
        TekanSimpan()
    End Sub
    Private Sub BtnBatal_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBatal.Click
        TekanBatal()
    End Sub
    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        If GBBayar.Visible Then
            TekanBatal()
        ElseIf TxtNama.Text <> "" Then
            TxtNama.Clear()
        Else
            ' Menambahkan pertanyaan apakah akan keluar
            Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar dari halaman penjualan ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                Close()
            End If
        End If
    End Sub

    Public Sub TekanBatal()
        ' ═══════════════════════════════════════════════════════════
        ' ✅ HANYA bersihkan data pembayaran saat mode TAMBAH
        ' ✅ Saat mode EDIT, jangan kosongkan (data dari database harus dipertahankan)
        ' ═══════════════════════════════════════════════════════════
        If TxtJenistransaksi.Text = "TambahPenjualan" Then
            ' Mode TAMBAH: Kosongkan semua field pembayaran
            TxtNominalBayarTunai.Clear()
            TxtNominalBayarTransfer.Clear()
        End If
        ' Mode EDIT: Jangan kosongkan nilai, hanya tutup panel saja

        ' Tutup panel pembayaran
        GBBayar.Visible = False
    End Sub

    Private Sub CenterPanelBayar()
        Dim x As Integer = (ClientSize.Width - GBBayar.Width) \ 2
        Dim y As Integer = (Me.ClientSize.Height - GBBayar.Height) \ 2
        GBBayar.Location = New Point(x, y)
    End Sub

    Private Sub Form_Penjualan_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                CmbSales.Select()
                CmbSales.DroppedDown = True

            Case Keys.F3
                CmbPelanggan.Select()
                CmbPelanggan.DroppedDown = True
            Case Keys.F4
                TekanBarang()

            Case Keys.F5
                CmbBayarTransfer.Select()
                CmbBayarTransfer.DroppedDown = True

            Case Keys.F12
                Tekanpelanggan()

            Case Keys.F6
                Tekantahan()

            Case Keys.F7
                Tekanpanggil()

            Case Keys.F8
                ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
                If DgvData.IsCurrentCellInEditMode Then
                    DgvData.EndEdit()
                End If
                TekanBayar()

            Case Keys.F9
                CmbBayarTunai.Select()
                CmbBayarTunai.DroppedDown = True

            Case Keys.F10
                If GBBayar.Visible Then
                    TekanSimpan()
                End If

            Case Keys.F11
                If GBBayar.Visible Then
                    TekanBatal()
                End If

            Case Keys.Escape
                If GBBayar.Visible Then
                    TekanBatal()
                ElseIf TxtNama.Text <> "" Then
                    TxtNama.Clear()
                Else
                    ' Menambahkan pertanyaan apakah akan keluar
                    Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Close()
                    End If
                End If

            Case Keys.Back, Keys.Delete
                ' Jika LstBarang visible dan tombol Backspace atau Delete ditekan
                If LstBarang.Visible = True Then
                    TxtNama.Select()
                End If

        End Select
    End Sub

    Public Sub TekanBayar()
        If String.IsNullOrWhiteSpace(TxtFaktur.Text) Then
            MessageBox.Show("Nomor faktur wajib diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtFaktur.Focus()
            Return
        End If



        ' Cek apakah ada item yang dimasukkan
        If String.IsNullOrWhiteSpace(TxtJmlhItem.Text) OrElse DgvData.Rows.Count = 0 Then
            MessageBox.Show("Belum ada barang yang dimasukkan ke dalam transaksi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Cek apakah nominal 0 diizinkan
        If Nominal0 = "Tidak" AndAlso (String.IsNullOrEmpty(TxtTotalJualSblDiskonPajak.Text) OrElse TxtTotalJualSblDiskonPajak.Text = "0") Then
            MessageBox.Show("Total penjualan belum terisi. Tidak bisa melanjutkan pembayaran.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Cek jika menjual rugi tidak diizinkan
        If modulJualRugi = "Tidak" AndAlso Cekjualrugi() Then
            Exit Sub
        End If

        ' Cek jika stok minus tidak diizinkan
        If modulJualMinus = "Tidak" AndAlso CekStok() Then
            Exit Sub
        End If

        ' ═══════════════════════════════════════════
        ' Atur nilai textbox pembayaran
        ' ✅ HANYA isi saat mode TAMBAH, JANGAN ubah saat EDIT
        ' ═══════════════════════════════════════════
        If TxtJenistransaksi.Text = "TambahPenjualan" Then
            ' Mode TAMBAH: Isi dengan nilai default
            TxtNominalBayarTunai.Text = If(Isinominal = "Tidak", "", TxtTotaljualStlPajak.Text)
        End If
        ' Mode EDIT: Jangan ubah nilai yang sudah ada (sudah di-load dari Editpenjualanheader)

        ' Tampilkan panel bayar dan arahkan fokus
        CenterPanelBayar()
        GBBayar.Visible = True
        TxtNominalBayarTunai.Focus()
    End Sub


    Public Sub Tekantahan()
        If String.IsNullOrWhiteSpace(TxtFaktur.Text) Then
            MessageBox.Show("Nomor faktur wajib diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtFaktur.Focus()
            Return
        End If



        ' Pastikan setidaknya satu baris di kolom "kode" terisi
        Dim isDataValid As Boolean = False

        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso Not String.IsNullOrEmpty(Convert.ToString(dgvRow.Cells(0).Value)) Then
                isDataValid = True
                Exit For
            End If
        Next

        If Not isDataValid Then
            MessageBox.Show("Belum ada barang yang di input", "Kosong", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = Nothing

        Try

            transaction = conn.BeginTransaction()
            If TxtJenistransaksi.Text = "TambahPenjualan" Then
                Nomorjual()
            End If


            ' Menyimpan data penjualan_ditahan
            For Each dgvRow As DataGridViewRow In DgvData.Rows
                ' Cek apakah baris bukan baris baru (kosong)
                If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                    Dim query As String = "INSERT INTO penjualan_ditahan_detail (FAKTUR_JUAL, ID_BARANG, NAMA_BARANG, SERIAL_NUMBER, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA, TOKO, GUDANG, STOK) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
                    Using cmd As New MySqlCommand(query, conn, transaction)

                        cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
                        cmd.Parameters.AddWithValue("@idBarang", dgvRow.Cells(0).Value)
                        cmd.Parameters.AddWithValue("@namaBarang", dgvRow.Cells(1).Value)
                        cmd.Parameters.AddWithValue("@serialNumber", If(dgvRow.Cells(16).Value IsNot Nothing, dgvRow.Cells(16).Value.ToString(), String.Empty))
                        cmd.Parameters.AddWithValue("@hargaBeli", ParseDecimal(dgvRow.Cells(2).Value))
                        cmd.Parameters.AddWithValue("@qty", ParseDecimal(dgvRow.Cells(3).Value))
                        cmd.Parameters.AddWithValue("@satuan", dgvRow.Cells(4).Value)
                        cmd.Parameters.AddWithValue("@isiSatuan", ParseDecimal(dgvRow.Cells(5).Value))
                        cmd.Parameters.AddWithValue("@hargaBeliSatuan", ParseDecimal(dgvRow.Cells(6).Value))
                        cmd.Parameters.AddWithValue("@hargaJual", ParseDecimal(dgvRow.Cells(7).Value))
                        cmd.Parameters.AddWithValue("@qtySatuan", ParseDecimal(dgvRow.Cells(8).Value))
                        cmd.Parameters.AddWithValue("@diskonPersen", ParseDecimal(dgvRow.Cells(9).Value))
                        cmd.Parameters.AddWithValue("@diskonRp", ParseDecimal(dgvRow.Cells(10).Value))
                        cmd.Parameters.AddWithValue("@totalDiskon", ParseDecimal(dgvRow.Cells(11).Value))
                        cmd.Parameters.AddWithValue("@totalHarga", ParseDecimal(dgvRow.Cells(12).Value))
                        cmd.Parameters.AddWithValue("@toko", dgvRow.Cells(13).Value)
                        cmd.Parameters.AddWithValue("@gudang", dgvRow.Cells(14).Value)
                        cmd.Parameters.AddWithValue("@stok", ParseDecimal(dgvRow.Cells(15).Value))

                        cmd.ExecuteNonQuery()
                    End Using
                End If
            Next




            ' Menyimpan data Temp_penjualan_ditahan
            Dim tempQuery As String = "INSERT INTO penjualan_ditahan (FAKTUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, TANGGAL_JUAL, GRAN_TOTAL, TOTAL_QTY, TOTAL_ITEM, ID_USER, ID_KOMPUTER) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
            Using cmd As New MySqlCommand(tempQuery, conn, transaction)

                cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
                cmd.Parameters.AddWithValue("@idPelanggan", LbLKodePel.Text)
                cmd.Parameters.AddWithValue("@namaPelanggan", CmbPelanggan.Text)
                cmd.Parameters.AddWithValue("@jenisPelanggan", LblJenisPl.Text)
                cmd.Parameters.AddWithValue("@tanggalJual", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))

                ' Tidak perlu pakai cultureIndonesia lagi
                cmd.Parameters.AddWithValue("@grantTotal", ParseDecimal(TxtTotalJualSblDiskonPajak.Text))
                cmd.Parameters.AddWithValue("@totalQty", ParseDecimal(TxtJmlhQtySatuan.Text))
                cmd.Parameters.AddWithValue("@totalItem", ParseDecimal(TxtJmlhItem.Text))

                cmd.Parameters.AddWithValue("@idUser", FormUtama.SLogin.Text)
                cmd.Parameters.AddWithValue("@idKomputer", FormUtama.Comp.Text)

                cmd.ExecuteNonQuery()
            End Using



            ' Commit transaksi
            transaction.Commit()

            DatabaseModule.CatatanAksiHistory("Simpan penjualan " & TxtFaktur.Text)
            JumlahTahan()
            TxtFaktur.Text = ""
            Call Kondisiawal()

        Catch ex As Exception
            ' Rollback transaksi jika terjadi kesalahan
            transaction?.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub Tekanpanggil()
        If TxtTahan.Text > 0 Then
            FormPenjualanDitahan.ShowDialog()
            JumlahTahan()
            'AmbilDataDitahan()
        Else
            MessageBox.Show("Tidak ada data ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

    End Sub

    Public Sub JumlahTahan()
        Dim queryString As String = "SELECT COUNT(FAKTUR_JUAL) as jumlah FROM penjualan_ditahan"
        Dim jumlah As Integer

        Using command As New MySqlCommand(queryString, conn)
            jumlah = CInt(command.ExecuteScalar())
            TxtTahan.Text = jumlah
        End Using
    End Sub

    Public Sub AmbilDataDitahan()
        ' ✅ BATCH 4 OPTIMIZATION: Batch load satuan hanya untuk barang yang dibutuhkan
        If TxtFaktur.Text = "" Then
            Exit Sub
        Else
            If TxtJenistransaksi.Text = "TambahPenjualan" Then
                DgvData.DataSource = Nothing
                DgvData.Rows.Clear()

                ' ═══════════════════════════════════════════════════════════════
                ' STEP 1: Query COMBINED - Detail + Satuan dalam 1 query
                ' ═══════════════════════════════════════════════════════════════
                Dim satuanBarang As New Dictionary(Of String, List(Of String))
                Dim queryDetail As String = "SELECT " &
                    "pdd.ID_BARANG, pdd.NAMA_BARANG, pdd.SERIAL_NUMBER, pdd.HARGA_BELI, " &
                    "pdd.QTY, pdd.SATUAN, pdd.ISI_SATUAN, pdd.HARGA_BELI_SATUAN, " &
                    "pdd.HARGA_JUAL, pdd.QTY_SATUAN, pdd.DISKON_PERSEN, pdd.DISKON_RP, " &
                    "pdd.TOTAL_DISKON, pdd.TOTAL_HARGA, pdd.TOKO, pdd.GUDANG, pdd.STOK, " &
                    "tb.SATUAN_PARTAI_KECIL, tb.SATUAN_PARTAI_SEDANG, tb.SATUAN_PARTAI_BESAR, " &
                    "tb.SATUAN_UMUM_KECIL, tb.SATUAN_UMUM_SEDANG, tb.SATUAN_UMUM_BESAR " &
                    "FROM penjualan_ditahan_detail pdd " &
                    "LEFT JOIN tbl_barang tb ON pdd.ID_BARANG = tb.ID_BARANG " &
                    "WHERE pdd.FAKTUR_JUAL = @FAKTUR_JUAL"

                Try
                    Using cmd As New MySqlCommand(queryDetail, conn)
                        cmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)

                        Using rd As MySqlDataReader = cmd.ExecuteReader()
                            ' ═══════════════════════════════════════════════════════════════
                            ' STEP 2: Process detail & build satuan dictionary INLINE
                            ' ═══════════════════════════════════════════════════════════════
                            While rd.Read()
                                Dim idBarang As String = SafeGetValue(Of String)(rd, "ID_BARANG", "")

                                ' ✅ BUILD SATUAN DICTIONARY (HANYA UNTUK BARANG YANG ADA!)
                                If Not satuanBarang.ContainsKey(idBarang) Then
                                    Dim listSatuan As New List(Of String)

                                    If LblJenisPl.Text = "Partai" Then
                                        ' Tambahkan satuan partai
                                        Dim satuanPartaiKecil = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_KECIL", "")
                                        Dim satuanPartaiSedang = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_SEDANG", "")
                                        Dim satuanPartaiBesar = SafeGetValue(Of String)(rd, "SATUAN_PARTAI_BESAR", "")

                                        If Not String.IsNullOrEmpty(satuanPartaiKecil) Then listSatuan.Add(satuanPartaiKecil)
                                        If Not String.IsNullOrEmpty(satuanPartaiSedang) Then listSatuan.Add(satuanPartaiSedang)
                                        If Not String.IsNullOrEmpty(satuanPartaiBesar) Then listSatuan.Add(satuanPartaiBesar)
                                    Else
                                        ' Tambahkan satuan umum
                                        Dim satuanUmumKecil = SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                                        Dim satuanUmumSedang = SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                                        Dim satuanUmumBesar = SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")

                                        If Not String.IsNullOrEmpty(satuanUmumKecil) Then listSatuan.Add(satuanUmumKecil)
                                        If Not String.IsNullOrEmpty(satuanUmumSedang) Then listSatuan.Add(satuanUmumSedang)
                                        If Not String.IsNullOrEmpty(satuanUmumBesar) Then listSatuan.Add(satuanUmumBesar)
                                    End If

                                    satuanBarang(idBarang) = listSatuan
                                End If

                                ' ═══════════════════════════════════════════════════════════════
                                ' STEP 3: Tambahkan ke grid & isi combobox
                                ' ═══════════════════════════════════════════════════════════════
                                Dim row As DataGridViewRow = DgvData.Rows(DgvData.Rows.Add())

                                row.Cells(0).Value = SafeGetValue(Of String)(rd, "ID_BARANG", "")
                                row.Cells(1).Value = SafeGetValue(Of String)(rd, "NAMA_BARANG", "")
                                row.Cells(16).Value = SafeGetValue(Of String)(rd, "SERIAL_NUMBER", "")
                                row.Cells(2).Value = SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                                row.Cells(3).Value = SafeGetValue(Of Decimal)(rd, "QTY", 0D)
                                row.Cells(4).Value = SafeGetValue(Of String)(rd, "SATUAN", "")
                                row.Cells(5).Value = SafeGetValue(Of Decimal)(rd, "ISI_SATUAN", 0D)
                                row.Cells(6).Value = SafeGetValue(Of Decimal)(rd, "HARGA_BELI_SATUAN", 0D)
                                row.Cells(7).Value = SafeGetValue(Of Decimal)(rd, "HARGA_JUAL", 0D)
                                row.Cells(8).Value = SafeGetValue(Of Decimal)(rd, "QTY_SATUAN", 0D)
                                row.Cells(9).Value = SafeGetValue(Of Decimal)(rd, "DISKON_PERSEN", 0D)
                                row.Cells(10).Value = SafeGetValue(Of Decimal)(rd, "DISKON_RP", 0D)
                                row.Cells(11).Value = SafeGetValue(Of Decimal)(rd, "TOTAL_DISKON", 0D)
                                row.Cells(12).Value = SafeGetValue(Of Decimal)(rd, "TOTAL_HARGA", 0D)
                                row.Cells(13).Value = SafeGetValue(Of String)(rd, "TOKO", "")
                                row.Cells(14).Value = SafeGetValue(Of String)(rd, "GUDANG", "")
                                row.Cells(15).Value = SafeGetValue(Of Decimal)(rd, "STOK", 0D)

                                ' ✅ ISI COMBOBOX DARI DICTIONARY (TIDAK ADA QUERY TAMBAHAN!)
                                If satuanBarang.ContainsKey(idBarang) Then
                                    Dim comboCell As DataGridViewComboBoxCell = CType(row.Cells("Satuan"), DataGridViewComboBoxCell)
                                    comboCell.Items.Clear()
                                    comboCell.Items.AddRange(satuanBarang(idBarang).ToArray())
                                End If
                            End While
                        End Using
                    End Using

                Catch ex As Exception
                    Debug.WriteLine("AmbilDataDitahan Error: " & ex.Message)
                    MessageBox.Show("Gagal memuat data ditahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

                ' ═══════════════════════════════════════════════════════════════
                ' STEP 4: Update data & hitung total
                ' ═══════════════════════════════════════════════════════════════
                UpdateSetelahTahan()
                UpdateSemuaTotal()
            End If
        End If
    End Sub


    Private Sub UpdateSetelahTahan()
        ' ✅ BATCH 3 OPTIMIZATION: Batch query instead of per-item queries
        Dim barangDict As New Dictionary(Of String, BarangInfo)
        Dim kodeBarangList As New List(Of String)()

        ' Step 1: Kumpulkan semua kode barang dari grid
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                kodeBarangList.Add(dgvRow.Cells("Kode").Value.ToString())
            End If
        Next

        If kodeBarangList.Count = 0 Then
            Hitungbaris()
            Return
        End If

        ' Step 2: ✅ SINGLE BATCH QUERY untuk semua barang (bukan per-item!)
        Dim inClause As String = String.Join(",", kodeBarangList.Select(Function(x) "'" & x.Replace("'", "''") & "'"))
        Dim batchQuery As String = "SELECT ID_BARANG, HARGA_BELI, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG IN (" & inClause & ")"

        Try
            Using cmd As New MySqlCommand(batchQuery, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim kodeBarang As String = rd("ID_BARANG").ToString()
                        barangDict(kodeBarang) = New BarangInfo() With {
                            .HargaBeli = SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D),
                            .StokToko = SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D),
                            .StokGudang = SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                        }
                    End While
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("UpdateSetelahTahan Batch Query Error: " & ex.Message)
            Hitungbaris()
            Return
        End Try

        ' Step 3: Update grid offline (no more queries!)
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                If barangDict.ContainsKey(kodeBarangValue) Then
                    Dim barangInfo As BarangInfo = barangDict(kodeBarangValue)

                    dgvRow.Cells("HargaBeli").Value = barangInfo.HargaBeli
                    dgvRow.Cells("StokToko").Value = barangInfo.StokToko
                    dgvRow.Cells("StokGudang").Value = barangInfo.StokGudang

                    Dim stokValue As Decimal = If(LblLokasiBarang.Text = "GUDANG",
                                                  barangInfo.StokGudang,
                                                  barangInfo.StokToko)

                    dgvRow.Cells("Stok").Value = stokValue
                End If
            End If
        Next

        Hitungbaris()
    End Sub

    Private Sub Hitungbaris()
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso Not IsDBNull(row.Cells("Kode").Value) AndAlso Not String.IsNullOrEmpty(row.Cells("Kode").Value.ToString()) Then
                ' Variables for HargaBeli calculation
                Dim hargaBeli = If(Not IsDBNull(row.Cells("HargaBeli").Value), ParseDecimal(row.Cells("HargaBeli").Value), 0)
                Dim qty = If(Not IsDBNull(row.Cells("QTY").Value), ParseDecimal(row.Cells("QTY").Value), 0)
                Dim isi = If(Not IsDBNull(row.Cells("isi").Value), ParseDecimal(row.Cells("isi").Value), 1)

                ' Variables for QtySat calculation
                Dim qtySat = qty * isi

                ' Variables for TotalHarga calculation
                Dim harga = If(Not IsDBNull(row.Cells("Harga").Value), ParseDecimal(row.Cells("Harga").Value), 0)
                Dim totalDiskon = If(Not IsDBNull(row.Cells("TotalDiskon").Value), ParseDecimal(row.Cells("TotalDiskon").Value), 0)

                ' Variables for TotalHarga calculation
                Dim totalHarga = qty * harga - totalDiskon

                ' Assign values to DataGridView columns
                row.Cells("Totalhargabeli").Value = hargaBeli * qtySat
                row.Cells("QtySat").Value = qtySat
                row.Cells("TotalHarga").Value = totalHarga
            End If
        Next
    End Sub

    Public Sub TekanBarang()
        Using f As New TambahBarang()
            f.LblUtama.Text = "T A M B A H   B A R A N G"
            f.ShowDialog()
        End Using
    End Sub

    Public Sub Tekanpelanggan()
        TambahPelanggan.ShowDialog()
        Call TampilPelanggan()
    End Sub


    Public Class StokInfo
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal
    End Class

    Public Class BarangInfo
        Public Property HargaBeli As Decimal
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal
    End Class

    ''' <summary>
    ''' ✅ BATCH 4: Helper class untuk menyimpan info harga barang dari batch query
    ''' Digunakan oleh: UpdateHargaBerdasarJenisPelanggan()
    ''' </summary>
    Public Class BarangHargaInfo
        Public Property HargaBeli As Decimal
        Public Property SatuanUmumKecil As String
        Public Property SatuanUmumSedang As String
        Public Property SatuanUmumBesar As String
        Public Property HargaJualUmumKecil As Decimal
        Public Property HargaJualUmumSedang As Decimal
        Public Property HargaJualUmumBesar As Decimal
        Public Property SatuanPartaiKecil As String
        Public Property SatuanPartaiSedang As String
        Public Property SatuanPartaiBesar As String
        Public Property HargaJualPartaiKecil As Decimal
        Public Property HargaJualPartaiSedang As Decimal
        Public Property HargaJualPartaiBesar As Decimal
        Public Property StokToko As Decimal
        Public Property StokGudang As Decimal
    End Class

    Public Function CekStok() As Boolean
        ' ✅ PHASE 2 OPTIMIZATION: Batch query instead of per-item queries
        Dim stokDict As New Dictionary(Of String, StokInfo)
        Dim kodeBarangList As New List(Of String)()

        ' Step 1: Kumpulkan semua kode barang dari grid
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                kodeBarangList.Add(dgvRow.Cells("Kode").Value.ToString())
            End If
        Next

        If kodeBarangList.Count = 0 Then Return False

        ' Step 2: ✅ SINGLE BATCH QUERY untuk semua stok (bukan per-item!)
        Dim inClause As String = String.Join(",", kodeBarangList.Select(Function(x) "'" & x.Replace("'", "''") & "'"))
        Dim batchStokQuery As String = "SELECT ID_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG IN (" & inClause & ")"

        Try
            Using cmd As New MySqlCommand(batchStokQuery, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim kodeBarang As String = rd("ID_BARANG").ToString()
                        stokDict(kodeBarang) = New StokInfo() With {
                            .StokToko = SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D),
                            .StokGudang = SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                        }
                    End While
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("CekStok Batch Query Error: " & ex.Message)
            Return False
        End Try

        ' Step 3: ✅ SINGLE BATCH QUERY untuk penjualan_detail (kalau edit transaksi)
        If TxtJenistransaksi.Text <> "TambahPenjualan" Then
            Dim batchDetailQuery As String = "SELECT ID_BARANG, SUM(QTY_SATUAN) AS TOTAL_QTY FROM penjualan_detail WHERE FAKTUR_JUAL = @FK AND ID_BARANG IN (" & inClause & ") GROUP BY ID_BARANG"
            Try
                Using cmd As New MySqlCommand(batchDetailQuery, conn)
                    cmd.Parameters.AddWithValue("@FK", TxtFaktur.Text)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        While rd.Read()
                            Dim idBarang As String = rd("ID_BARANG").ToString()
                            If stokDict.ContainsKey(idBarang) Then
                                Dim totalQty As Decimal = SafeGetValue(Of Decimal)(rd, "TOTAL_QTY", 0D)
                                If LblLokasiBarang.Text = "TOKO" Then
                                    stokDict(idBarang).StokToko += totalQty
                                Else
                                    stokDict(idBarang).StokGudang += totalQty
                                End If
                            End If
                        End While
                    End Using
                End Using
            Catch ex As Exception
                Debug.WriteLine("CekStok Detail Batch Query Error: " & ex.Message)
            End Try
        End If

        ' Step 4: Validasi offline (no more queries!)
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()
                Dim totalQtyTerjual As Decimal = If(IsDBNull(dgvRow.Cells("QtySat").Value), 0D, ParseDecimal(dgvRow.Cells("QtySat").Value))

                If stokDict.ContainsKey(kodeBarangValue) Then
                    Dim stokInfo As StokInfo = stokDict(kodeBarangValue)
                    Dim totalStok As Decimal = If(LblLokasiBarang.Text = "TOKO", stokInfo.StokToko, stokInfo.StokGudang)

                    If totalQtyTerjual > totalStok Then
                        Dim errorMessage As String = "Stok ==> " & dgvRow.Cells("NamaBarang").Value & " <== tidak mencukupi untuk dijual." & vbCrLf & vbCrLf & "Total Terjual: " & totalQtyTerjual & vbCrLf & "Total Stok: " & totalStok
                        MessageBox.Show(errorMessage, "Stok Tidak cukup", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        dgvRow.Selected = True
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = Color.Red
                        Next

                        dgvRow.DataGridView.Focus()
                        dgvRow.DataGridView.CurrentCell = dgvRow.Cells(1)
                        dgvRow.DataGridView.CurrentRow.Selected = True

                        Return True
                    End If

                    ' Reset warna
                    Dim defaultBackColor As Color = dgvRow.DefaultCellStyle.BackColor
                    For Each cell As DataGridViewCell In dgvRow.Cells
                        cell.Style.BackColor = defaultBackColor
                    Next
                End If
            End If
        Next

        Return False
    End Function

    Public Function Cekjualrugi() As Boolean
        ' ✅ PHASE 2 OPTIMIZATION: Batch query instead of per-item queries
        Dim hargaBeliDict As New Dictionary(Of String, Decimal)()
        Dim kodeBarangList As New List(Of String)()

        ' Step 1: Kumpulkan semua kode barang dari grid
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                kodeBarangList.Add(dgvRow.Cells("Kode").Value.ToString())
            End If
        Next

        If kodeBarangList.Count = 0 Then Return False

        ' Step 2: ✅ SINGLE BATCH QUERY untuk semua HARGA_BELI (bukan per-item!)
        Dim inClause As String = String.Join(",", kodeBarangList.Select(Function(x) "'" & x.Replace("'", "''") & "'"))
        Dim batchQuery As String = "SELECT ID_BARANG, HARGA_BELI FROM tbl_barang WHERE ID_BARANG IN (" & inClause & ")"

        Try
            Using cmd As New MySqlCommand(batchQuery, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim kodeBarang As String = rd("ID_BARANG").ToString()
                        hargaBeliDict(kodeBarang) = SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                    End While
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Cekjualrugi Batch Query Error: " & ex.Message)
            Return False
        End Try

        ' Step 3: Validasi offline (no more queries!)
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()
                Dim Hargajual As Decimal = ParseDecimal(dgvRow.Cells("Harga").Value)

                If hargaBeliDict.ContainsKey(kodeBarangValue) Then
                    Dim Hargabeli As Decimal = hargaBeliDict(kodeBarangValue)

                    If Hargabeli > Hargajual Then
                        Dim errorMessage As String = "Barang: " & dgvRow.Cells("NamaBarang").Value & vbCrLf & vbCrLf & "Harga beli: " & Hargabeli.ToString("N0") & vbCrLf & vbCrLf & "Harga jual: " & Hargajual.ToString("N0")
                        MessageBox.Show(errorMessage, "Harga jual rugi", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        dgvRow.Selected = True
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = Color.Red
                        Next

                        dgvRow.DataGridView.Focus()
                        dgvRow.DataGridView.CurrentCell = dgvRow.Cells(1)
                        dgvRow.DataGridView.CurrentRow.Selected = True

                        Return True
                    End If
                End If

                ' Reset warna
                Dim defaultBackColor As Color = dgvRow.DefaultCellStyle.BackColor
                For Each cell As DataGridViewCell In dgvRow.Cells
                    cell.Style.BackColor = defaultBackColor
                Next
            End If
        Next

        Return False
    End Function


    Private Sub TxtBayar_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtNominalBayarTunai.KeyPress
        ' Validasi: Hanya angka dan kontrol characters
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
        ' CATATAN: Enter key di-handle di TxtNominalBayarTunai_KeyDown untuk smart focus logic
    End Sub


    ' ================================
    ' FUNCTION AMBIL DATA AKUN
    ' ================================
    Private Function GetKodeAkun(namaAkun As String) As String
        If String.IsNullOrWhiteSpace(namaAkun) Then Return ""

        Dim result As String = ""

        Try
            Dim sql As String = "SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @nama LIMIT 1"

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@nama", namaAkun)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        result = reader("Kode_akun").ToString()
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' Optional: log error
            ' MsgBox(ex.Message)
        End Try

        Return result
    End Function


    ' ================================
    ' EVENT TUNAI
    ' ================================
    Private Sub CmbBayarTunai_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbBayarTunai.SelectedIndexChanged
        TxtKodeBayarTunai.Text = GetKodeAkun(CmbBayarTunai.Text)
    End Sub


    ' ================================
    ' EVENT TRANSFER
    ' ================================
    Private Sub CmbBayarTransfer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbBayarTransfer.SelectedIndexChanged
        TxtKodeBayarBank.Text = GetKodeAkun(CmbBayarTransfer.Text)
    End Sub


    ' ════════════════════════════════════════════════════════════════
    ' NAVIGASI KEYBOARD - SMART FOCUS NAVIGATION
    ' ════════════════════════════════════════════════════════════════
    ' LOGIKA:
    ' 1. GBBayar visible → fokus langsung ke TxtNominalBayarTunai (skip combobox)
    ' 2. Combobox hanya di-skip jika SelectedIndex > -1 (sudah ada pilihan)
    ' 3. Tunai saja → skip info bank
    ' 4. Tunai + Transfer → include bank info
    ' ════════════════════════════════════════════════════════════════

    ''' <summary>
    ''' Fokus awal ketika GBBayar visible
    ''' Prioritas: TxtNominalBayarTunai (tidak perlu pilih metode dulu)
    ''' </summary>
    Private Sub GBBayar_VisibleChanged(sender As Object, e As EventArgs) Handles GBBayar.VisibleChanged
        If GBBayar.Visible Then
            ' Reset dan set default selection untuk combobox
            If CmbBayarTunai.SelectedIndex = -1 Then
                CmbBayarTunai.SelectedIndex = 0
            End If
            ' Fokus langsung ke input nominal tunai
            TxtNominalBayarTunai.Focus()
            TxtNominalBayarTunai.SelectAll()
        End If
    End Sub

    Private Sub TxtNominalBayarTunai_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNominalBayarTunai.KeyDown
        If e.KeyCode = Keys.Enter Then
            TxtNominalBayarTransfer.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtNominalBayarTransfer_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNominalBayarTransfer.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.KeyCode = Keys.Enter Then
                ' ═══════════════════════════════════════════
                ' LOGIKA: Tentukan next control
                ' ═══════════════════════════════════════════
                If PanelTFPelanggan.Visible Then
                    ' ADA TRANSFER → lanjut ke TxtNominalBayarTransfer
                    CmbBank.DroppedDown = True
                    CmbBank.Focus()
                Else
                    ' TUNAI SAJA → langsung SIMPAN (skip bank info)
                    TekanSimpan()
                End If
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub CmbBank_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbBank.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' ═══════════════════════════════════════════
            ' Jika CmbBank tidak ada selection → tanya pilih
            ' ═══════════════════════════════════════════
            If CmbBank.SelectedIndex = -1 Then
                ' User belum pilih bank → buka dropdown
                CmbBank.DroppedDown = True
                e.SuppressKeyPress = True
                Return
            End If

            ' ═══════════════════════════════════════════
            ' Bank sudah dipilih → lanjut ke TxtNoRek
            ' ═══════════════════════════════════════════
            ' ✅ Pastikan dropdown ditutup sebelum focus pindah
            CmbBank.DroppedDown = False

            ' ✅ Pindahkan fokus ke TxtNoRek
            TxtNoRek.Focus()
            TxtNoRek.SelectAll()

            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtNoRek_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNoRek.KeyDown
        If e.KeyCode = Keys.Enter Then
            TxtNamaRek.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtNamaRek_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNamaRek.KeyDown
        If e.KeyCode = Keys.Enter Then
            TxtNoReff.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtNoReff_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNoReff.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Dari nomor referensi → SIMPAN
            TekanSimpan()
            e.SuppressKeyPress = True
        End If
    End Sub

    ' ════════════════════════════════════════════════════════════════
    ' SKIP HANDLER UNTUK COMBOBOX PEMBAYARAN
    ' Tidak perlu handler Enter karena combobox sudah dipilih by default
    ' ════════════════════════════════════════════════════════════════

    Private Sub TxtNominalBayarTransfer_TextChanged(sender As Object, e As EventArgs) Handles TxtNominalBayarTransfer.TextChanged
        ' Panggil method untuk atur ukuran panel berdasarkan ada/tidaknya transfer
        Propertigbbayar()

        ' Trigger perhitungan dengan memanggil langsung method calculation
        TxtBayarTunaiAtauTransfer_TextChanged(sender, e)
    End Sub

    ''' <summary>
    ''' Atur tampilan panel pembayaran berdasarkan ada/tidaknya input transfer
    ''' Jika ada transfer → tampilkan panel info rekening pengirim
    ''' Jika transfer kosong → sembunyikan panel (tunai saja)
    ''' </summary>
    Private Sub Propertigbbayar()
        Dim nominalTransferValue As Decimal = 0D

        ' Parse nominal transfer dengan aman
        Decimal.TryParse(TxtNominalBayarTransfer.Text, nominalTransferValue)

        If nominalTransferValue > 0 Then
            ' ═════════════════════════════════════════════
            ' KONDISI: ADA TRANSFER (TUNAI + TRANSFER)
            ' ═════════════════════════════════════════════
            GBBayar.Size = New Size(600, 413)
            BtnSimpann.Location = New Point(190, 361)
            BtnBatal.Location = New Point(398, 361)
            PanelTFPelanggan.Visible = True
        Else
            ' ═════════════════════════════════════════════
            ' KONDISI: TUNAI SAJA (TRANSFER KOSONG)
            ' ═════════════════════════════════════════════
            ' Bersihkan data transfer
            CmbBank.SelectedIndex = -1
            TxtNoRek.Clear()
            TxtNamaRek.Clear()
            TxtNoReff.Clear()

            GBBayar.Size = New Size(600, 278)
            BtnSimpann.Location = New Point(190, 226)
            BtnBatal.Location = New Point(383, 226)
            PanelTFPelanggan.Visible = False
        End If

    End Sub


    ' ===== VARIABLE PEMBAYARAN TERPISAH =====
    Private nominalTunai As Decimal = 0D      ' Nominal pembayaran tunai
    Private nominalTransfer As Decimal = 0D   ' Nominal pembayaran transfer
    Private totalBayar As Decimal = 0D        ' Total tunai + transfer
    Private selisihBayar As Decimal = 0D      ' ✅ UBAH: Dim → Private (KONSISTEN!)
    Private sisaHutang As Decimal = 0D        ' Sisa tagihan jika belum lunas
    Private kembaliTunai As Decimal = 0D      ' Kembalian dari pembayaran tunai

    ''' <summary>
    ''' ✅ EVENT UTAMA: Hitung pembayaran saat user input nominal tunai/transfer
    ''' 🎯 SINGLE SOURCE OF TRUTH - Semua perhitungan payment di sini
    ''' 
    ''' Dihitung:
    ''' - Total pembayaran (tunai + transfer)
    ''' - Selisih bayar (total bayar vs total belanja)
    ''' - Sisa hutang (jika pembayaran kurang)
    ''' - Kembalian (jika pembayaran lebih)
    ''' - Status transaksi (Lunas vs Belum Lunas)
    ''' </summary>
    Private Sub TxtBayarTunaiAtauTransfer_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalBayarTunai.TextChanged, TxtNominalBayarTransfer.TextChanged, TxtTotaljualStlPajak.TextChanged

        ' ═══════════════════════════════════════════
        ' 1. AMBIL NILAI TOTAL BELANJA
        ' ═══════════════════════════════════════════
        Dim totalBelanja As Decimal = 0D
        If Not Decimal.TryParse(TxtTotaljualStlPajak.Text, totalBelanja) Then
            TxtKembaliHutang.Text = TxtTotaljualStlPajak.Text
            LblBayarTunai.Text = "0"
            LblBayarTransfer.Text = "0"
            Exit Sub
        End If

        ' ═══════════════════════════════════════════
        ' 2. AMBIL NILAI NOMINAL TUNAI & TRANSFER
        ' ═══════════════════════════════════════════
        If Not Decimal.TryParse(TxtNominalBayarTunai.Text, nominalTunai) Then
            nominalTunai = 0D
        End If

        If Not Decimal.TryParse(TxtNominalBayarTransfer.Text, nominalTransfer) Then
            nominalTransfer = 0D
        End If

        ' ═══════════════════════════════════════════
        ' 3. HITUNG TOTAL PEMBAYARAN
        ' ═══════════════════════════════════════════
        totalBayar = nominalTunai + nominalTransfer

        ' ═══════════════════════════════════════════
        ' 4. HITUNG SISA HUTANG vs KEMBALIAN
        ' ═══════════════════════════════════════════
        ' ✅ Selisih = Total Bayar - Total Belanja
        selisihBayar = totalBayar - totalBelanja

        If selisihBayar < 0 Then
            ' ✅ BELUM LUNAS (ada hutang)
            sisaHutang = Math.Abs(selisihBayar)
            kembaliTunai = 0D
            LblStatusTrans.Text = "Belum Lunas"
            LblPembayaran.Text = "Hutang :"

            ' Tampilkan jatuh tempo
            LblJatuhTempo.Visible = True
            DTPJatuhTempo.Visible = True
        Else
            ' ✅ LUNAS (kembalian atau pas)
            sisaHutang = 0D
            kembaliTunai = selisihBayar
            LblStatusTrans.Text = "Lunas"
            LblPembayaran.Text = "Kembalian :"

            ' Sembunyikan jatuh tempo
            LblJatuhTempo.Visible = False
            DTPJatuhTempo.Visible = False
        End If

        ' ═══════════════════════════════════════════
        ' 5. TAMPILKAN NILAI DI LABEL & TEXTBOX
        ' ═══════════════════════════════════════════
        LblBayarTunai.Text = nominalTunai.ToString("#,0.##", cultureIndonesia)
        LblBayarTransfer.Text = nominalTransfer.ToString("#,0.##", cultureIndonesia)
        LblKembali.Text = Math.Max(kembaliTunai, sisaHutang).ToString("#,0.##", cultureIndonesia)
        TxtKembaliHutang.Text = Math.Max(kembaliTunai, sisaHutang).ToString("0.##", Globalization.CultureInfo.InvariantCulture)

        ' ═══════════════════════════════════════════
        ' 6. DEBUG LOG (Optional, untuk tracking)
        ' ═══════════════════════════════════════════
        Debug.WriteLine($"Payment Update: Tunai={nominalTunai}, Transfer={nominalTransfer}, Total={totalBayar}, Belanja={totalBelanja}, Kembali={kembaliTunai}, Hutang={sisaHutang}")

    End Sub

    Private Sub PanelTFPelanggan_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelTFPelanggan.VisibleChanged
        If TxtJenistransaksi.Text = "TambahPenjualan" Then
            CmbBank.SelectedIndex = -1
            TxtNoRek.Clear()
            TxtNamaRek.Clear()
            TxtNoReff.Clear()
        End If

    End Sub

    Public Sub TekanSimpan()

        ' ═══════════════════════════════════════════════════════════════
        ' VALIDASI LEVEL 1: CEK PANEL PEMBAYARAN VISIBLE
        ' ═══════════════════════════════════════════════════════════════
        If Not GBBayar.Visible Then
            MessageBox.Show(
                "Panel pembayaran belum ditampilkan." & vbCrLf &
                "Tekan F8 atau tombol [Bayar] terlebih dahulu.",
                "Peringatan",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )
            Exit Sub
        End If

        ' ═══════════════════════════════════════════════════════════════
        ' VALIDASI LEVEL 2: AMBIL & NORMALISASI DATA PEMBAYARAN
        ' ═══════════════════════════════════════════════════════════════
        Dim nominalTunai As Decimal = 0D
        Dim nominalTransfer As Decimal = 0D
        Dim totalBelanja As Decimal = 0D

        If Not Decimal.TryParse(TxtNominalBayarTunai.Text, nominalTunai) Then
            nominalTunai = 0D
        End If

        If Not Decimal.TryParse(TxtNominalBayarTransfer.Text, nominalTransfer) Then
            nominalTransfer = 0D
        End If

        If Not Decimal.TryParse(TxtTotaljualStlPajak.Text, totalBelanja) Then
            MessageBox.Show(
                "Total belanja tidak valid. Silakan periksa kembali.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )
            Exit Sub
        End If

        Dim totalBayar As Decimal = nominalTunai + nominalTransfer

        ' ═══════════════════════════════════════════════════════════════
        ' VALIDASI LEVEL 3: CEK KONSISTENSI PEMBAYARAN VS STATUS
        ' ═══════════════════════════════════════════════════════════════
        If LblStatusTrans.Text = "Lunas" Then
            ' Jika status LUNAS, harus ada pembayaran
            If totalBayar = 0 Then
                MessageBox.Show(
                    "Status transaksi LUNAS tapi tidak ada pembayaran." & vbCrLf &
                    "Periksa kembali nominal pembayaran atau ubah status.",
                    "Error Konsistensi Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                TxtNominalBayarTunai.Focus()
                Exit Sub
            End If

            ' Jika status LUNAS, pembayaran harus >= total belanja
            If totalBayar < totalBelanja Then
                MessageBox.Show(
                    "Status LUNAS tapi pembayaran kurang dari total belanja." & vbCrLf &
                    "Total Belanja: " & totalBelanja.ToString("#,0.##", cultureIndonesia) & vbCrLf &
                    "Total Bayar: " & totalBayar.ToString("#,0.##", cultureIndonesia),
                    "Pembayaran Tidak Cukup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                Exit Sub
            End If
        End If

        ' ═══════════════════════════════════════════════════════════════
        ' VALIDASI LEVEL 4: CEK AKUN PEMBAYARAN DIPILIH
        ' ═══════════════════════════════════════════════════════════════
        If nominalTunai > 0 Then
            If String.IsNullOrWhiteSpace(CmbBayarTunai.Text) Then
                MessageBox.Show(
                    "Pembayaran tunai dipilih tapi metode pembayaran tunai kosong." & vbCrLf &
                    "Pilih metode pembayaran tunai terlebih dahulu.",
                    "Peringatan",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                CmbBayarTunai.Focus()
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(TxtKodeBayarTunai.Text) Then
                MessageBox.Show(
                    "Kode akun pembayaran tunai tidak tersimpan." & vbCrLf &
                    "Silakan pilih ulang metode pembayaran.",
                    "Error Sistem",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                Exit Sub
            End If
        End If

        If nominalTransfer > 0 Then
            If String.IsNullOrWhiteSpace(CmbBayarTransfer.Text) Then
                MessageBox.Show(
                    "Pembayaran transfer dipilih tapi metode pembayaran bank kosong." & vbCrLf &
                    "Pilih metode pembayaran bank terlebih dahulu.",
                    "Peringatan",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                CmbBayarTransfer.Focus()
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(CmbBank.Text) Then
                MessageBox.Show(
                    "Pembayaran transfer dipilih tapi bank kosong." & vbCrLf &
                    "Pilih bank terlebih dahulu.",
                    "Peringatan",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                CmbBank.Focus()
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(TxtNoRek.Text) Then
                MessageBox.Show(
                    "Nomor rekening pengirim wajib diisi untuk pembayaran transfer.",
                    "Peringatan",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                TxtNoRek.Focus()
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(TxtNamaRek.Text) Then
                MessageBox.Show(
                    "Nama pemilik rekening pengirim wajib diisi untuk pembayaran transfer.",
                    "Peringatan",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                TxtNamaRek.Focus()
                Exit Sub
            End If
        End If

        ' ═══════════════════════════════════════════════════════════════
        ' VALIDASI LEVEL 5: CEK DATA BARANG VALID DI GRID
        ' ═══════════════════════════════════════════════════════════════
        If Not ValidateDataBarangGrid() Then
            Exit Sub
        End If

        ' ═══════════════════════════════════════════════════════════════
        ' VALIDASI LEVEL 6: CEK PELANGGAN UNTUK STATUS BELUM LUNAS
        ' ═══════════════════════════════════════════════════════════════
        If LblStatusTrans.Text = "Belum Lunas" Then
            If String.IsNullOrWhiteSpace(CmbPelanggan.Text) Then
                MessageBox.Show(
                    "Status BELUM LUNAS wajib memilih pelanggan." & vbCrLf &
                    "Silakan pilih pelanggan terlebih dahulu.",
                    "Peringatan",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                CmbPelanggan.DroppedDown = True
                CmbPelanggan.Focus()
                Exit Sub
            End If

            ' Cek jatuh tempo untuk hutang
            If DTPJatuhTempo.Value <= DTPTgl.Value Then
                MessageBox.Show(
                    "Tanggal jatuh tempo harus lebih besar dari tanggal transaksi." & vbCrLf &
                    "Tanggal Transaksi: " & DTPTgl.Value.ToString("dd/MM/yyyy") & vbCrLf &
                    "Tanggal Jatuh Tempo: " & DTPJatuhTempo.Value.ToString("dd/MM/yyyy"),
                    "Peringatan",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                DTPJatuhTempo.Focus()
                Exit Sub
            End If
        End If

        ' ═══════════════════════════════════════════════════════════════
        ' VALIDASI LEVEL 7: CEK TANGGAL TRANSAKSI (TAHUN LAMPAU)
        ' ═══════════════════════════════════════════════════════════════
        If TransaksiLampau = "Tidak" AndAlso DTPTgl.Value.Year < DateTime.Now.Year Then
            Dim resultTahun As DialogResult = MessageBox.Show(
                "Transaksi tahun " & DTPTgl.Value.Year & " (tahun lampau)." & vbCrLf &
                "Apakah Anda yakin ingin menyimpan transaksi ini?" & vbCrLf & vbCrLf &
                "Klik 'Ya' untuk lanjutkan, 'Tidak' untuk ubah tanggal.",
                "Konfirmasi Tanggal Lampau",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            )

            If resultTahun = DialogResult.No Then
                DTPTgl.Focus()
                Exit Sub
            End If
        End If

        ' ═══════════════════════════════════════════════════════════════
        ' SEMUA VALIDASI PASSED → SIMPAN
        ' ═══════════════════════════════════════════════════════════════
        Simpanatauedit()

    End Sub

    ''' <summary>
    ''' Validasi data barang di grid sebelum simpan
    ''' Return True jika valid, False jika ada error
    ''' </summary>
    Private Function ValidateDataBarangGrid() As Boolean
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Kode").Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(row.Cells("Kode").Value.ToString()) Then

                ' Cek Kode
                If String.IsNullOrEmpty(row.Cells("Kode").Value.ToString()) Then
                    MessageBox.Show("Kode barang tidak boleh kosong.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If

                ' Cek Nama Barang
                If String.IsNullOrEmpty(row.Cells("NamaBarang").Value.ToString()) Then
                    MessageBox.Show("Nama barang tidak boleh kosong.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If

                ' Cek Harga Jual > 0
                Dim hargaJual As Decimal = ParseDecimal(row.Cells("Harga").Value)
                If hargaJual <= 0 Then
                    MessageBox.Show(
                        "Harga jual barang '" & row.Cells("NamaBarang").Value & "' harus lebih dari 0.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    )
                    Return False
                End If

                ' Cek QTY > 0
                Dim qty As Decimal = ParseDecimal(row.Cells("QTY").Value)
                If qty <= 0 Then
                    MessageBox.Show(
                        "Jumlah barang '" & row.Cells("NamaBarang").Value & "' harus lebih dari 0.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    )
                    Return False
                End If

                ' Cek Satuan
                If String.IsNullOrEmpty(row.Cells("Satuan").Value.ToString()) Then
                    MessageBox.Show(
                        "Satuan barang '" & row.Cells("NamaBarang").Value & "' tidak boleh kosong.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    )
                    Return False
                End If
            End If
        Next

        Return True
    End Function


    Public Sub Simpanatauedit()
        Cursor = Cursors.WaitCursor
        If TxtJenistransaksi.Text = "TambahPenjualan" Then


            Dim query As String = "SELECT ID_PENJUALAN FROM penjualan WHERE ID_PENJUALAN = ?"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtFaktur.Text)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then

                    If TransaksiLampau = "Tidak" Then
                        DTPTgl.Value = DateTime.Now
                        Nomorjual()
                    End If
                    Nomorjual()
                End If
            End Using
            Prosessimpan()
        Else
            Prosessimpan()
        End If
        Cursor = Cursors.Default
    End Sub

    Public Sub Prosessimpan()
        If String.IsNullOrEmpty(TxtNominalBayarTunai.Text) OrElse TxtNominalBayarTunai.Text = "0" Then
            TxtNominalBayarTunai.Text = "0"
        End If

        ' ✅ NILAI PAYMENT SUDAH CALCULATED OTOMATIS via TxtBayarTunaiAtauTransfer_TextChanged
        ' Tidak perlu RecalculatePaymentValues() lagi

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        ' Mengubah kursor menjadi menunggu
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Try
            ' Panggil metode Hapusuntukedit jika bukan TambahPenjualan
            If TxtJenistransaksi.Text <> "TambahPenjualan" Then
                Hapusuntukedit(transaction)
            End If

            ' Panggil metode-metode Simpan dengan menggunakan transaksi yang sama
            Simpanpenjualan(transaction)
            'Simpanpiutang(transaction)
            Simpanpenjualandetail(transaction)
            Simpanjurnal(transaction)
            HistoryBarang(transaction)

            ' Commit transaksi
            transaction.Commit()

            ' Jika semuanya berhasil, kembalikan kondisi awal
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                    HitungByKode(row.Cells(0).Value)
                End If
            Next


            ' CETAK DILUAR TRY utama agar tetap lanjut meski gagal
            Try
                Select Case CmbCetak.Text.Trim().ToLower()
                    Case "iya"
                        CetakFaktur()

                    Case "tanya"
                        Dim result As DialogResult = MessageBox.Show("Apakah Anda ingin mencetak penjualan?", "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        If result = DialogResult.Yes Then
                            CetakFaktur()
                        End If

                    Case "tidak"
                        ' Tidak melakukan apa-apa
                End Select

            Catch ex As Exception
                MessageBox.Show("Gagal mencetak penjualan. Anda bisa mencetak ulang nanti." & vbCrLf &
                            "Detail: " & ex.Message, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Finally
                ' Bersihkan dan kembali ke kondisi awal
                DgvData.DataSource = Nothing
                DgvData.Rows.Clear()

                Kondisiawal()

                ' ✅ Gunakan kembaliTunai (sudah di-recalculate di atas)
                If kembaliTunai > 0 Then
                    TampilkanPesanKembaliPelanggan(kembaliTunai)
                End If

                If TxtJenistransaksi.Text <> "TambahPenjualan" Then
                    TxtJenistransaksi.Text = "TambahPenjualan"
                    TxtFaktur.Text = ""
                    Close()
                End If
            End Try

        Catch ex As Exception
            MessageBox.Show("Oh tidak! Transaksi penjualan dibatalkan karena terjadi kesalahan." & vbCrLf &
                     "Detail kesalahan: " & ex.Message,
           "Oops! Ada masalah simpan penjualan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ' Rollback transaksi secara otomatis karena ada kesalahan
            transaction.Rollback()
        Finally
            ' Mengembalikan kursor ke normal setelah proses selesai atau terjadi kesalahan
            System.Windows.Forms.Cursor.Current = Cursors.Default
        End Try
    End Sub


    ''' <summary>
    ''' Menampilkan pesan kembalian pelanggan dengan tampilan yang besar dan jelas
    ''' Form akan kosong dan hanya menampilkan pesan kembalian
    ''' </summary>
    Private Sub TampilkanPesanKembaliPelanggan(jumlahKembali As Decimal)

        Dim formKembali As New Form()
        With formKembali
            .Text = ""
            .Size = New Size(520, 420)
            .StartPosition = FormStartPosition.CenterScreen
            .ControlBox = False
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .BackColor = Color.White
            .TopMost = True
            .KeyPreview = True
        End With

        ' ===== TABLE LAYOUT (ENGINE AMAN) =====
        Dim layout As New TableLayoutPanel()
        With layout
            .Dock = DockStyle.Fill
            .ColumnCount = 1
            .RowCount = 6
            .Padding = New Padding(20)
            .BackColor = Color.White
            .AutoSize = True
            .AutoSizeMode = AutoSizeMode.GrowAndShrink
            .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        End With

        ' ===== JUDUL =====
        Dim lblJudul As New Label()
        With lblJudul
            .Text = "KEMBALIAN"
            .Font = New Font("Segoe UI", 28, FontStyle.Bold)
            .ForeColor = Color.DarkGreen
            .AutoSize = True
            .TextAlign = ContentAlignment.MiddleCenter
            .Dock = DockStyle.Fill
        End With

        ' ===== PEMISAH =====
        Dim lblPemisah1 As New Label()
        With lblPemisah1
            .Text = "═════════════════════════════"
            .Font = New Font("Courier New", 14, FontStyle.Bold)
            .ForeColor = Color.DarkGreen
            .AutoSize = True
            .TextAlign = ContentAlignment.MiddleCenter
            .Dock = DockStyle.Fill
        End With

        ' ===== NOMINAL (AMAN TOTAL) =====
        Dim lblNominal As New Label()
        With lblNominal
            .Text = "Rp. " & jumlahKembali.ToString("#,0.##", cultureIndonesia)
            .Font = New Font("Segoe UI", 36, FontStyle.Bold)
            .ForeColor = Color.Teal
            .AutoSize = True
            .TextAlign = ContentAlignment.MiddleCenter
            .Dock = DockStyle.Fill
            .Margin = New Padding(0, 15, 0, 15)
        End With

        ' ===== PEMISAH =====
        Dim lblPemisah2 As New Label()
        With lblPemisah2
            .Text = "═════════════════════════════"
            .Font = New Font("Courier New", 14, FontStyle.Bold)
            .ForeColor = Color.DarkGreen
            .AutoSize = True
            .TextAlign = ContentAlignment.MiddleCenter
            .Dock = DockStyle.Fill
        End With

        ' ===== TOMBOL OK =====
        Dim btnOK As New Button()
        With btnOK
            .Text = "OK"
            .Font = New Font("Segoe UI", 14, FontStyle.Bold)
            .Size = New Size(200, 50)
            .BackColor = Color.LimeGreen
            .ForeColor = Color.White
            .Anchor = AnchorStyles.None
        End With

        Dim panelButton As New Panel()
        With panelButton
            .Height = 70
            .Dock = DockStyle.Fill
        End With
        panelButton.Controls.Add(btnOK)
        btnOK.Location = New Point(
        (panelButton.Width - btnOK.Width) \ 2,
        (panelButton.Height - btnOK.Height) \ 2
    )
        AddHandler panelButton.Resize, Sub()
                                           btnOK.Location = New Point(
                                          (panelButton.Width - btnOK.Width) \ 2,
                                          (panelButton.Height - btnOK.Height) \ 2
                                      )
                                       End Sub

        ' ===== PETUNJUK =====
        Dim lblPetunjuk As New Label()
        With lblPetunjuk
            .Text = "Tekan ENTER untuk melanjutkan" & vbCrLf & "atau klik tombol OK"
            .Font = New Font("Segoe UI", 12)
            .ForeColor = Color.DarkBlue
            .AutoSize = True
            .TextAlign = ContentAlignment.MiddleCenter
            .Dock = DockStyle.Fill
            .Margin = New Padding(0, 10, 0, 0)
        End With

        ' ===== MASUKKAN KE LAYOUT =====
        layout.Controls.Add(lblJudul)
        layout.Controls.Add(lblPemisah1)
        layout.Controls.Add(lblNominal)
        layout.Controls.Add(lblPemisah2)
        layout.Controls.Add(panelButton)
        layout.Controls.Add(lblPetunjuk)

        formKembali.Controls.Add(layout)

        ' ===== EVENT =====
        AddHandler btnOK.Click, Sub()
                                    formKembali.DialogResult = DialogResult.OK
                                    formKembali.Close()
                                End Sub

        AddHandler formKembali.KeyDown, Sub(sender, e)
                                            If e.KeyCode = Keys.Enter Then
                                                formKembali.DialogResult = DialogResult.OK
                                                formKembali.Close()
                                            End If
                                        End Sub

        formKembali.AcceptButton = btnOK
        formKembali.ShowDialog()

    End Sub

    ' Subroutine untuk mengurangi duplikasi kode cetak
    Sub CetakFaktur()
        With PrintJual
            .TxtFaktur.Text = TxtFaktur.Text
            If TanyakanKertas = "Iya" Then
                Dim result As DialogResult = MessageBox.Show("Pilih jenis printer untuk mencetak:" & vbCrLf & vbCrLf & "Yes = Printer Thermal" & vbCrLf & "No = Printer Dot Matrix",
                                              "Pilih Jenis Printer",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    ' Cetak dengan Printer Thermal
                    .ProsesCetak("Printer Thermal")
                ElseIf result = DialogResult.No Then
                    ' Cetak dengan Printer Dot Matrix
                    .ProsesCetak("Printer Dot Matrix")
                End If

            Else
                .ProsesCetak("")
            End If

        End With
    End Sub

    Public Sub Hapusuntukedit(ByVal transaction As MySqlTransaction)
        Dim updateStokField As String

        Select Case LblLokasiBarang.Text
            Case "TOKO"
                updateStokField = "PENJUALAN_TOKO"
            Case "GUDANG"
                updateStokField = "PENJUALAN_GUDANG"
            Case Else
                Throw New Exception("Lokasi barang tidak valid.")
                Exit Sub
        End Select

        Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " - ? WHERE ID_BARANG = ?"

        ' ✅ PERBAIKAN FASE 1: Safe FormUtama reference
        If FormUtama Is Nothing OrElse FormUtama.DGVDetail Is Nothing Then
            MessageBox.Show(
                "Form utama tidak tersedia untuk mengambil data barang.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )
            Return
        End If

        For Each row As DataGridViewRow In FormUtama.DGVDetail.Rows
            If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    Dim QtyTotal As Decimal = If(row.Cells("QTY_SATUAN").Value IsNot Nothing, ParseDecimal(row.Cells("QTY_SATUAN").Value), 0D)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@StokPengurangan", QtyTotal)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using

                    HitungStokPerubahan(kodeBarang, transaction)
                End If
            End If
        Next

        Dim fakturPenjualan As String = TxtFaktur.Text

        Dim deleteQueries As String() = {
        "DELETE FROM penjualan WHERE ID_PENJUALAN = @FakturPenjualan",
        "DELETE FROM penjualan_detail WHERE FAKTUR_JUAL = @FakturPenjualan",
        "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FakturPenjualan",
        "DELETE FROM HistoryBarang WHERE FAKTUR = @FakturPenjualan"
    }

        For Each query As String In deleteQueries
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@FakturPenjualan", fakturPenjualan)
                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub

    Public Sub Simpanpenjualan(ByVal transaction As MySqlTransaction)
        ' ═══════════════════════════════════════════
        ' HITUNG LABA & HPP
        ' ═══════════════════════════════════════════
        Dim totalHarga As Decimal = 0
        Dim totalHargaBeli As Decimal = 0
        Dim diskon As Decimal = 0

        For Each row As DataGridViewRow In DgvData.Rows
            totalHarga += If(IsDBNull(row.Cells("TotalHarga").Value), 0D, ParseDecimal(row.Cells("TotalHarga").Value))
            totalHargaBeli += If(IsDBNull(row.Cells("Totalhargabeli").Value), 0D, ParseDecimal(row.Cells("Totalhargabeli").Value))
        Next

        If Decimal.TryParse(TxtDiskonRp.Text, diskon) = False Then
            diskon = 0
        End If

        Dim laba As Decimal = (totalHarga - totalHargaBeli) - diskon

        ' ═══════════════════════════════════════════
        ' SIAPKAN DATA PEMBAYARAN
        ' ═══════════════════════════════════════════
        Dim bayarTunai As Decimal = ParseDecimal(TxtNominalBayarTunai.Text)
        Dim bayarTransfer As Decimal = ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim statusBayar As String = If(LblStatusTrans.Text = "Lunas", "TERBAYAR", "TERHUTANG")

        ' ═══════════════════════════════════════════
        ' INSERT KE TABEL PENJUALAN
        ' ═══════════════════════════════════════════
        Dim query As String = "INSERT INTO penjualan (" &
                        "ID_PENJUALAN, ID_PELANGGAN, NAMA_PELANGGAN, ALAMAT_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, " &
                        "TGL_TRANSAKSI, GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP, PAJAK_PERSEN, PAJAK_RP, " &
                        "GRAND_TOTAL_STL_PAJAK, LABA, BAYAR, NOMINAL_TRANSFER, TOTAL_HPP, BIAYA_KIRIM, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_BAYAR, STATUS_TRANSAKSI, " &
                        "TYPE_AKUN, KODE_AKUN, JENIS_PEMBAYARAN, " &
                        "KODE_AKUN_TF, NAMA_AKUN_TF, TYPE_AKUNBANK, KODE_AKUNBANK, JENIS_PEMBAYARANBANK, " &
                        "METODE, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI, " &
                        "ID_SALES, NAMA_SALES, ID_USER, ID_KOMPUTER) " &
                        "VALUES (@ID_PENJUALAN, @ID_PELANGGAN, @NAMA_PELANGGAN, @ALAMAT_PELANGGAN, @JENIS_PELANGGAN, @LOKASIBARANG, " &
                        "@TGL_TRANSAKSI, @GRAND_TOTAL_SBL_PAJAK, @DISKON_TOTAL_PERSEN, @DISKON_TOTAL_RP, @PAJAK_PERSEN, @PAJAK_RP, " &
                        "@GRAND_TOTAL_STL_PAJAK, @LABA, @BAYAR, @NOMINAL_TRANSFER, @TOTAL_HPP, @BIAYA_KIRIM, @KEMBALI, @SISA_TAGIHAN, @JATUH_TEMPO, @STATUS_BAYAR, @STATUS_TRANSAKSI, " &
                        "@TYPE_AKUN, @KODE_AKUN, @JENIS_PEMBAYARAN, " &
                        "@KODE_AKUN_TF, @NAMA_AKUN_TF, @TYPE_AKUNBANK, @KODE_AKUNBANK, @JENIS_PEMBAYARANBANK, " &
                        "@METODE, @BANK, @NO_REKENING, @NAMA_REKENING, @NO_REFFERENSI, " &
                        "@ID_SALES, @NAMA_SALES, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(query, conn, transaction)
            ' ─── IDENTITAS ───
            cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@ID_PELANGGAN", LbLKodePel.Text)
            cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbPelanggan.Text)
            cmd.Parameters.AddWithValue("@ALAMAT_PELANGGAN", LblAlamat.Text)
            cmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPl.Text)
            cmd.Parameters.AddWithValue("@LOKASIBARANG", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))

            ' ─── NILAI TRANSAKSI ───
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_SBL_PAJAK", ParseDecimal(TxtTotalJualSblDiskonPajak.Text))
            cmd.Parameters.AddWithValue("@DISKON_TOTAL_PERSEN", ParseDecimal(TxtDiskonPersen.Text))
            cmd.Parameters.AddWithValue("@DISKON_TOTAL_RP", ParseDecimal(TxtDiskonRp.Text))
            cmd.Parameters.AddWithValue("@PAJAK_PERSEN", ParseDecimal(TxtPajakPersen.Text))
            cmd.Parameters.AddWithValue("@PAJAK_RP", ParseDecimal(TxtPajakRp.Text))
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_STL_PAJAK", ParseDecimal(TxtTotaljualStlPajak.Text))
            cmd.Parameters.AddWithValue("@LABA", laba)
            cmd.Parameters.AddWithValue("@TOTAL_HPP", ParseDecimal(TxtTotalHpp.Text))
            cmd.Parameters.AddWithValue("@BIAYA_KIRIM", ParseDecimal(TxtBiayaKirim.Text))

            ' ─── PEMBAYARAN TUNAI ───
            cmd.Parameters.AddWithValue("@BAYAR", bayarTunai)
            cmd.Parameters.AddWithValue("@TYPE_AKUN", "KAS")
            cmd.Parameters.AddWithValue("@KODE_AKUN", TxtKodeBayarTunai.Text)
            cmd.Parameters.AddWithValue("@JENIS_PEMBAYARAN", CmbBayarTunai.Text)

            ' ─── PEMBAYARAN TRANSFER ───
            cmd.Parameters.AddWithValue("@NOMINAL_TRANSFER", bayarTransfer)
            cmd.Parameters.AddWithValue("@KODE_AKUN_TF", If(bayarTransfer > 0, TxtKodeBayarBank.Text, ""))
            cmd.Parameters.AddWithValue("@NAMA_AKUN_TF", If(bayarTransfer > 0, CmbBayarTransfer.Text, ""))
            cmd.Parameters.AddWithValue("@TYPE_AKUNBANK", "BANK")
            cmd.Parameters.AddWithValue("@KODE_AKUNBANK", TxtKodeBayarBank.Text)
            cmd.Parameters.AddWithValue("@JENIS_PEMBAYARANBANK", CmbBayarTransfer.Text)

            ' ─── METODE & BANK ───
            Dim metode As String = "Tunai"
            If bayarTransfer > 0 Then
                metode = "Tunai + Transfer"
            End If
            cmd.Parameters.AddWithValue("@METODE", metode)
            cmd.Parameters.AddWithValue("@BANK", CmbBank.Text)
            cmd.Parameters.AddWithValue("@NO_REKENING", TxtNoRek.Text)
            cmd.Parameters.AddWithValue("@NAMA_REKENING", TxtNamaRek.Text)
            cmd.Parameters.AddWithValue("@NO_REFFERENSI", TxtNoReff.Text)

            ' ─── STATUS PEMBAYARAN ───
            cmd.Parameters.AddWithValue("@KEMBALI", kembaliTunai)
            cmd.Parameters.AddWithValue("@SISA_TAGIHAN", sisaHutang)
            cmd.Parameters.AddWithValue("@JATUH_TEMPO", If(sisaHutang > 0, DTPJatuhTempo.Value.ToString("yyyy-MM-dd"), DBNull.Value))
            cmd.Parameters.AddWithValue("@STATUS_BAYAR", statusBayar)
            cmd.Parameters.AddWithValue("@STATUS_TRANSAKSI", LblStatusTrans.Text)

            ' ─── USER & SISTEM ───
            cmd.Parameters.AddWithValue("@ID_SALES", LblSales.Text)
            cmd.Parameters.AddWithValue("@NAMA_SALES", CmbSales.Text)
            ' ✅ PERBAIKAN FASE 1: Safe FormUtama access dengan helper
            cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenistransaksi.Text = "TambahPenjualan", GetFormUtamaLogin(), TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", GetFormUtamaComputer(), TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using

    End Sub

    Public Sub Simpanpiutang(ByVal transaction As MySqlTransaction)
        If LblStatusTrans.Text = "Belum Lunas" Then
            ' Simpan hutang pelanggan
            Using cmdSimpanHutang As New MySqlCommand("INSERT INTO penjualan_Piutang (IDPENJUALAN, TGL_BELANJA, LOKASI, KODE_PELANGGAN, NAMA_PELANGGAN, QTY, TOTAL_RUPIAH, BAYAR, HUTANG, JATUH_TEMPO, ID_USER, ID_KOMPUTER, STATUS) VALUES (@ID_TRANSAKSI, @TglBelanja, @LOKASI, @KodeSp, @NamaSP, @Qty, @TOTAL_RUPIAH, @BAYAR, @HUTANG, @JATUH_TEMPO, @ID_USER, @ID_KOMPUTER, @STATUS)", conn, transaction)
                cmdSimpanHutang.Parameters.AddWithValue("@IDPENJUALAN", TxtFaktur.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@TglBelanja", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdSimpanHutang.Parameters.AddWithValue("@LOKASIBARANG", LblLokasiBarang.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@KodeSp", LbLKodePel.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@NamaSP", CmbPelanggan.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@Qty", ParseDecimal(TxtJmlhQty.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@TOTAL_RUPIAH", ParseDecimal(TxtTotaljualStlPajak.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@BAYAR", ParseDecimal(TxtNominalBayarTunai.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@HUTANG", ParseDecimal(TxtKembaliHutang.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@JATUH_TEMPO", DTPJatuhTempo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdSimpanHutang.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                cmdSimpanHutang.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
                cmdSimpanHutang.Parameters.AddWithValue("@STATUS", LblStatusTrans.Text)

                cmdSimpanHutang.ExecuteNonQuery()
            End Using
        End If
    End Sub

    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim querySimpan As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                                            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(querySimpan, conn, transaction)
                    cmd.Parameters.AddWithValue("@FAKTUR", TxtFaktur.Text)
                    cmd.Parameters.AddWithValue("@TANGGAL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@JENIS", "PENJUALAN")
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
                    cmd.Parameters.AddWithValue("@QTY", If(IsDBNull(row.Cells(3).Value), 0D, ParseDecimal(row.Cells(3).Value)))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", If(IsDBNull(row.Cells(5).Value), 0D, ParseDecimal(row.Cells(5).Value)))
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", If(IsDBNull(row.Cells(8).Value), 0D, ParseDecimal(row.Cells(8).Value)))
                    cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", If(IsDBNull(row.Cells(12).Value), 0D, ParseDecimal(row.Cells(12).Value)))
                    cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.Comp.Text, TxtKomputer.Text))

                    cmd.ExecuteNonQuery()
                End Using

            End If
        Next
    End Sub

    Public Sub Simpanpenjualandetail(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_jual
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                ' Insert data into penjualan_detail
                Dim insertQuery As String = "INSERT INTO penjualan_detail (FAKTUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, TANGGAL_JUAL, ID_BARANG, NAMA_BARANG, SERIAL_NUMBER, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA, LABA, ID_USER, ID_KOMPUTER) " &
                                        "VALUES (@FAKTUR_JUAL, @ID_PELANGGAN, @NAMA_PELANGGAN, @JENIS_PELANGGAN, @LOKASIBARANG, @TANGGAL_JUAL, @ID_BARANG, @NAMA_BARANG, @SERIAL_NUMBER, @HARGA_BELI, @QTY, @SATUAN, @ISI_SATUAN, @HARGA_BELI_SATUAN, @HARGA_JUAL, @QTY_SATUAN, @DISKON_PERSEN, @DISKON_RP, @TOTAL_DISKON, @TOTAL_HARGA, @LABA, @ID_USER, @ID_KOMPUTER)"

                Using insertCmd As New MySqlCommand(insertQuery, conn, transaction)
                    ' Menambahkan parameter dengan nilai dari kontrol dan baris DataGridView
                    insertCmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)
                    insertCmd.Parameters.AddWithValue("@ID_PELANGGAN", LbLKodePel.Text)
                    insertCmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbPelanggan.Text)
                    insertCmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPl.Text)
                    insertCmd.Parameters.AddWithValue("@LOKASIBARANG", LblLokasiBarang.Text)
                    insertCmd.Parameters.AddWithValue("@TANGGAL_JUAL", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    insertCmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    insertCmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
                    insertCmd.Parameters.AddWithValue("@SERIAL_NUMBER", If(row.Cells(16).Value IsNot Nothing, row.Cells(16).Value.ToString(), String.Empty))

                    ' Penanganan DBNull untuk setiap nilai yang dikonversi menggunakan ParseDecimal
                    insertCmd.Parameters.AddWithValue("@HARGA_BELI", If(IsDBNull(row.Cells(2).Value), 0D, ParseDecimal(row.Cells(2).Value)))
                    insertCmd.Parameters.AddWithValue("@QTY", If(IsDBNull(row.Cells(3).Value), 0D, ParseDecimal(row.Cells(3).Value)))
                    insertCmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    insertCmd.Parameters.AddWithValue("@ISI_SATUAN", If(IsDBNull(row.Cells(5).Value), 0D, ParseDecimal(row.Cells(5).Value)))
                    insertCmd.Parameters.AddWithValue("@HARGA_BELI_SATUAN", If(IsDBNull(row.Cells(6).Value), 0D, ParseDecimal(row.Cells(6).Value)))
                    insertCmd.Parameters.AddWithValue("@HARGA_JUAL", If(IsDBNull(row.Cells(7).Value), 0D, ParseDecimal(row.Cells(7).Value)))
                    insertCmd.Parameters.AddWithValue("@QTY_SATUAN", If(IsDBNull(row.Cells(8).Value), 0D, ParseDecimal(row.Cells(8).Value)))
                    insertCmd.Parameters.AddWithValue("@DISKON_PERSEN", If(IsDBNull(row.Cells(9).Value), 0D, ParseDecimal(row.Cells(9).Value)))
                    insertCmd.Parameters.AddWithValue("@DISKON_RP", If(IsDBNull(row.Cells(10).Value), 0D, ParseDecimal(row.Cells(10).Value)))
                    insertCmd.Parameters.AddWithValue("@TOTAL_DISKON", If(IsDBNull(row.Cells(11).Value), 0D, ParseDecimal(row.Cells(11).Value)))
                    insertCmd.Parameters.AddWithValue("@TOTAL_HARGA", If(IsDBNull(row.Cells(12).Value), 0D, ParseDecimal(row.Cells(12).Value)))

                    ' Menghitung LABA dengan nilai default
                    Dim hargaJual As Decimal = If(IsDBNull(row.Cells(12).Value), 0D, ParseDecimal(row.Cells(12).Value))
                    Dim hargaBeli As Decimal = If(IsDBNull(row.Cells(6).Value), 0D, ParseDecimal(row.Cells(6).Value))
                    insertCmd.Parameters.AddWithValue("@LABA", hargaJual - hargaBeli)


                    insertCmd.Parameters.AddWithValue("@ID_USER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
                    insertCmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.Comp.Text, TxtKomputer.Text))

                    insertCmd.ExecuteNonQuery()
                End Using


                Dim updateStokField As String
                Select Case LblLokasiBarang.Text
                    Case "TOKO"
                        updateStokField = "PENJUALAN_TOKO"
                    Case "GUDANG"
                        updateStokField = "PENJUALAN_GUDANG"
                    Case Else
                        Throw New Exception("Lokasi barang tidak valid.")
                End Select

                Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " + ? WHERE ID_BARANG = ?"
                Dim kodeBarang As String = If(row.Cells("Kode").Value IsNot Nothing, row.Cells("Kode").Value.ToString(), String.Empty)
                If Not String.IsNullOrEmpty(kodeBarang) Then
                    ' Menggunakan format default untuk memastikan format desimal
                    Dim qtyTotal As Decimal = If(row.Cells("QtySat").Value IsNot Nothing, Decimal.Parse(row.Cells("QtySat").Value.ToString()), 0D)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        ' Menambahkan parameter untuk query
                        cmd.Parameters.AddWithValue("@P1", qtyTotal)
                        cmd.Parameters.AddWithValue("@P2", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            End If
        Next
    End Sub

    Public Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        ' ═══════════════════════════════════════════════════════════════════
        ' PERHITUNGAN DASAR UNTUK JURNAL
        ' ═══════════════════════════════════════════════════════════════════
        Dim persediaanbarang As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow Then
                persediaanbarang += ParseDecimal(row.Cells("Totalhargabeli").Value)
            End If
        Next

        Dim totalGrandJual As Decimal = ParseDecimal(TxtTotalJualSblDiskonPajak.Text)
        Dim labakotor As Decimal = totalGrandJual - persediaanbarang
        Dim diskontotal As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow Then
                diskontotal += ParseDecimal(row.Cells("TotalDiskon").Value)
            End If
        Next

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 1: POSTING PEMBAYARAN TUNAI
        ' ═══════════════════════════════════════════════════════════════════
        If nominalTunai > 0 Then
            Dim uraianTunai As String = If(nominalTunai >= ParseDecimal(TxtTotaljualStlPajak.Text),
                                            "Penjualan tunai lunas dari " & CmbPelanggan.Text,
                                            "Penjualan pembayaran tunai (sebagian) dari " & CmbPelanggan.Text)

            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, uraianTunai,
                            CmbBayarTunai.Text, TxtKodeBayarTunai.Text, "", "",
                            nominalTunai, "Penjualan", "", "")
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 2: POSTING PEMBAYARAN TRANSFER
        ' ═══════════════════════════════════════════════════════════════════
        If nominalTransfer > 0 Then
            Dim uraianTransfer As String = "Penjualan pembayaran transfer ke " & CmbBayarTransfer.Text & " a.n " & TxtNamaRek.Text

            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, uraianTransfer,
                            CmbBayarTransfer.Text, TxtKodeBayarBank.Text, "", "",
                            nominalTransfer, "Penjualan", "", "")
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 3: POSTING PIUTANG (jika masih hutang)
        ' ═══════════════════════════════════════════════════════════════════
        If sisaHutang > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value,
                            "Piutang penjualan dari " & CmbPelanggan.Text,
                            nama_rek_Piutang_Jual, Kode_rek_Piutang_Jual, "", "",
                            sisaHutang, "Penjualan", CmbPelanggan.Text, LbLKodePel.Text)
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 4: POSTING DISKON ITEM
        ' ═══════════════════════════════════════════════════════════════════
        If diskontotal > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value,
                            "Diskon item penjualan dari " & CmbPelanggan.Text,
                            "BEBAN DISKON PENJUALAN", "07.01.010",
                            "LABA KOTOR PENJUALAN", "06.01.001",
                            diskontotal, "Penjualan", "", "")
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 5: POSTING DISKON TOTAL
        ' ═══════════════════════════════════════════════════════════════════
        If ParseDecimal(TxtDiskonRp.Text) > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value,
                            "Diskon total penjualan dari " & CmbPelanggan.Text,
                            "BEBAN DISKON PENJUALAN", "07.01.010", "", "",
                            ParseDecimal(TxtDiskonRp.Text), "Penjualan", "", "")
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 6: POSTING PERSEDIAAN BARANG (HPP)
        ' ═══════════════════════════════════════════════════════════════════
        SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value,
                        "HPP penjualan kepada " & CmbPelanggan.Text,
                        "", "", NAMA_REK_BARANG, KODE_REK_BARANG,
                        persediaanbarang, "Penjualan", "", "")

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 7: POSTING HUTANG PAJAK
        ' ═══════════════════════════════════════════════════════════════════
        If ParseDecimal(TxtPajakRp.Text) > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value,
                            "Hutang pajak penjualan dari " & CmbPelanggan.Text,
                            "", "", "HUTANG PAJAK", "03.02.001",
                            ParseDecimal(TxtPajakRp.Text), "Penjualan", "", "")
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 8: POSTING LABA
        ' ═══════════════════════════════════════════════════════════════════
        SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value,
                        "Laba kotor penjualan dari " & CmbPelanggan.Text,
                        "", "", "LABA KOTOR PENJUALAN", "06.01.001",
                        labakotor, "Penjualan", "", "")

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 9: POSTING BIAYA KIRIM
        ' ═══════════════════════════════════════════════════════════════════
        If ParseDecimal(TxtBiayaKirim.Text) > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value,
                            "Jasa kirim/Lain " & CmbPelanggan.Text,
                            "", "", "PENDAPATAN LAIN LAIN", "08.01.002",
                            ParseDecimal(TxtBiayaKirim.Text), "Penjualan", "", "")
        End If

    End Sub

    Private Sub SimpanJurnalUmum(ByVal transaction As MySqlTransaction, ByVal NO_TRANSAKSI As String, ByVal tglTransaksi As DateTime, ByVal uraian As String, ByVal namaAkunD As String, ByVal nomorAkunD As String, ByVal namaAkunK As String, ByVal nomorAkunK As String, ByVal nominal As Decimal, ByVal jenisTransaksi As String, ByVal namaBantuD As String, ByVal kodeBantuD As String)
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_D, KODE_BANTU_D, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                        "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_D, @KODE_BANTU_D, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", NO_TRANSAKSI)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tglTransaksi.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", namaAkunD)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", nomorAkunD)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", namaAkunK)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", nomorAkunK)
            cmd.Parameters.AddWithValue("@NAMA_BANTU_D", namaBantuD)
            cmd.Parameters.AddWithValue("@KODE_BANTU_D", kodeBantuD)
            cmd.Parameters.AddWithValue("@NOMINAL", nominal)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", jenisTransaksi)
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.Comp.Text, TxtKomputer.Text))

            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Public Sub Editpenjualanheader()
        ' Deklarasikan variabel di luar MySqlDataReader
        Dim kodepel As String = String.Empty
        Dim namaPelanggan As String = String.Empty
        Dim jenisPelanggan As String = String.Empty
        Dim lokasibarang As String = String.Empty
        Dim tglTransaksi As DateTime = DateTime.MinValue

        Dim diskonPersen As Decimal = 0D
        Dim diskonRp As Decimal = 0D
        Dim pajakPersen As Decimal = 0D
        Dim pajakRp As Decimal = 0D
        Dim BiayaKirim As Decimal = 0D

        ' ═══════════════════════════════════════════════════════════
        ' VARIABEL PEMBAYARAN BARU (untuk 2 metode: Tunai + Transfer)
        ' ═══════════════════════════════════════════════════════════
        Dim bayarTunai As Decimal = 0D
        Dim bayarTransfer As Decimal = 0D
        Dim kembali As Decimal = 0D
        Dim sisaHutang As Decimal = 0D
        Dim statusTransaksi As String = ""
        Dim metodeTransaksi As String
        Dim jatuhTempo As DateTime = DateTime.MinValue

        ' ═══════════════════════════════════════════════════════════
        ' VARIABEL UNTUK TRANSFER
        ' ═══════════════════════════════════════════════════════════
        Dim kodeAkunTF As String
        Dim namaAkunTF As String = String.Empty
        Dim kodeAkunBank As String = String.Empty
        Dim jenisPaymentBank As String

        ' VARIABEL LAMA (untuk tunai)
        Dim kodeRef As String = String.Empty
        Dim jenisPembayaran As String = String.Empty
        Dim BANK As String = String.Empty
        Dim NO_REKENING As String = String.Empty
        Dim NAMA_REKENING As String = String.Empty
        Dim NO_REFFERENSI As String = String.Empty
        Dim SALES As String = String.Empty
        Dim NAMASALES As String = String.Empty
        Dim USER As String = String.Empty
        Dim KOMPUTER As String = String.Empty

        ' ═══════════════════════════════════════════════════════════
        ' QUERY UTAMA: Baca semua kolom pembayaran baru + lama
        ' ═══════════════════════════════════════════════════════════
        Dim queryString As String = "SELECT " &
                            "ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, " &
                            "DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP, PAJAK_PERSEN, PAJAK_RP, BIAYA_KIRIM, " &
                            "KODE_AKUN, JENIS_PEMBAYARAN, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI, " &
                            "BAYAR, NOMINAL_TRANSFER, KEMBALI, SISA_TAGIHAN, STATUS_TRANSAKSI, METODE, JATUH_TEMPO, " &
                            "KODE_AKUN_TF, NAMA_AKUN_TF, KODE_AKUNBANK, JENIS_PEMBAYARANBANK, " &
                            "ID_SALES, NAMA_SALES, ID_USER, ID_KOMPUTER " &
                            "FROM penjualan WHERE ID_PENJUALAN = ?"

        Using cmd As New MySqlCommand(queryString, conn)
            cmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' ═══════════════════════════════════════════════════════════
                    ' ASSIGN VARIABEL HEADER
                    ' ═══════════════════════════════════════════════════════════
                    kodepel = rd("ID_PELANGGAN").ToString()
                    namaPelanggan = rd("NAMA_PELANGGAN").ToString()
                    jenisPelanggan = rd("JENIS_PELANGGAN").ToString()
                    lokasibarang = rd("LOKASIBARANG").ToString()
                    tglTransaksi = Convert.ToDateTime(rd("TGL_TRANSAKSI"))

                    diskonPersen = If(IsDBNull(rd("DISKON_TOTAL_PERSEN")), 0D, ParseDecimal(rd("DISKON_TOTAL_PERSEN")))
                    diskonRp = If(IsDBNull(rd("DISKON_TOTAL_RP")), 0D, ParseDecimal(rd("DISKON_TOTAL_RP")))
                    pajakPersen = If(IsDBNull(rd("PAJAK_PERSEN")), 0D, ParseDecimal(rd("PAJAK_PERSEN")))
                    pajakRp = If(IsDBNull(rd("PAJAK_RP")), 0D, ParseDecimal(rd("PAJAK_RP")))
                    BiayaKirim = If(IsDBNull(rd("BIAYA_KIRIM")), 0D, ParseDecimal(rd("BIAYA_KIRIM")))

                    ' ═══════════════════════════════════════════════════════════
                    ' ASSIGN VARIABEL PEMBAYARAN BARU
                    ' ═══════════════════════════════════════════════════════════
                    bayarTunai = If(IsDBNull(rd("BAYAR")), 0D, ParseDecimal(rd("BAYAR")))
                    bayarTransfer = If(IsDBNull(rd("NOMINAL_TRANSFER")), 0D, ParseDecimal(rd("NOMINAL_TRANSFER")))
                    kembali = If(IsDBNull(rd("KEMBALI")), 0D, ParseDecimal(rd("KEMBALI")))
                    sisaHutang = If(IsDBNull(rd("SISA_TAGIHAN")), 0D, ParseDecimal(rd("SISA_TAGIHAN")))
                    statusTransaksi = If(IsDBNull(rd("STATUS_TRANSAKSI")), "Lunas", rd("STATUS_TRANSAKSI").ToString())
                    metodeTransaksi = If(IsDBNull(rd("METODE")), "Tunai", rd("METODE").ToString())
                    jatuhTempo = If(IsDBNull(rd("JATUH_TEMPO")), DateTime.MinValue, Convert.ToDateTime(rd("JATUH_TEMPO")))

                    ' ═══════════════════════════════════════════════════════════
                    ' ASSIGN VARIABEL TRANSFER
                    ' ═══════════════════════════════════════════════════════════
                    kodeAkunTF = If(IsDBNull(rd("KODE_AKUN_TF")), "", rd("KODE_AKUN_TF").ToString())
                    namaAkunTF = If(IsDBNull(rd("NAMA_AKUN_TF")), "", rd("NAMA_AKUN_TF").ToString())
                    kodeAkunBank = If(IsDBNull(rd("KODE_AKUNBANK")), "", rd("KODE_AKUNBANK").ToString())
                    jenisPaymentBank = If(IsDBNull(rd("JENIS_PEMBAYARANBANK")), "", rd("JENIS_PEMBAYARANBANK").ToString())

                    ' ═══════════════════════════════════════════════════════════
                    ' ASSIGN VARIABEL LAMA (TUNAI)
                    ' ═══════════════════════════════════════════════════════════
                    kodeRef = rd("KODE_AKUN").ToString()
                    jenisPembayaran = rd("JENIS_PEMBAYARAN").ToString()
                    BANK = rd("BANK").ToString()
                    NO_REKENING = rd("NO_REKENING").ToString()
                    NAMA_REKENING = rd("NAMA_REKENING").ToString()
                    NO_REFFERENSI = rd("NO_REFFERENSI").ToString()
                    SALES = rd("ID_SALES").ToString()
                    NAMASALES = rd("NAMA_SALES").ToString()
                    USER = rd("ID_USER").ToString()
                    KOMPUTER = rd("ID_KOMPUTER").ToString()
                End If
            End Using
        End Using

        ' ═══════════════════════════════════════════════════════════
        ' SET NILAI KE KONTROL - HEADER
        ' ═══════════════════════════════════════════════════════════
        CmbPelanggan.SelectedIndex = CmbPelanggan.FindStringExact(namaPelanggan)
        LbLKodePel.Text = kodepel
        LblJenisPl.Text = jenisPelanggan
        LblLokasiBarang.Text = lokasibarang
        DTPTgl.Value = tglTransaksi

        Editpenjualan()

        ' ✅ TextBox gunakan format STANDAR (InvariantCulture - no separator) - tampilkan desimal hanya jika ada
        TxtDiskonPersen.Text = diskonPersen.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtDiskonRp.Text = diskonRp.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPajakPersen.Text = pajakPersen.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPajakRp.Text = pajakRp.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtBiayaKirim.Text = BiayaKirim.ToString("0.##", Globalization.CultureInfo.InvariantCulture)

        ' ═══════════════════════════════════════════════════════════
        ' SET NILAI KE KONTROL - PEMBAYARAN TUNAI
        ' ═══════════════════════════════════════════════════════════
        TxtKodeBayarTunai.Text = kodeRef
        ' ✅ PERBAIKAN FASE 1: Safe ComboBox selection dengan helper
        SetComboBoxValue(CmbBayarTunai, jenisPembayaran)
        ' ✅ TextBox format STANDAR (no separator) - tampilkan desimal hanya jika ada
        TxtNominalBayarTunai.Text = bayarTunai.ToString("0.##", Globalization.CultureInfo.InvariantCulture)

        ' ═══════════════════════════════════════════════════════════
        ' SET NILAI KE KONTROL - PEMBAYARAN TRANSFER
        ' ═══════════════════════════════════════════════════════════
        ' ✅ TextBox format STANDAR (no separator) - tampilkan desimal hanya jika ada
        TxtNominalBayarTransfer.Text = bayarTransfer.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtKodeBayarBank.Text = kodeAkunBank
        ' ✅ PERBAIKAN FASE 1: Safe ComboBox selection dengan helper
        SetComboBoxValue(CmbBayarTransfer, namaAkunTF)
        CmbBank.Text = BANK
        TxtNoRek.Text = NO_REKENING
        TxtNamaRek.Text = NAMA_REKENING
        TxtNoReff.Text = NO_REFFERENSI

        ' ═══════════════════════════════════════════════════════════
        ' SET NILAI KE KONTROL - STATUS PEMBAYARAN
        ' ═══════════════════════════════════════════════════════════
        ' ✅ TextBox format STANDAR (no separator) - tampilkan desimal hanya jika ada
        TxtKembaliHutang.Text = Math.Max(kembali, sisaHutang).ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        LblStatusTrans.Text = statusTransaksi

        ' Jika belum lunas, tampilkan jatuh tempo
        If sisaHutang > 0 AndAlso jatuhTempo <> DateTime.MinValue Then
            DTPJatuhTempo.Value = jatuhTempo
            LblJatuhTempo.Visible = True
            DTPJatuhTempo.Visible = True
        Else
            LblJatuhTempo.Visible = False
            DTPJatuhTempo.Visible = False
        End If

        ' ═══════════════════════════════════════════════════════════
        ' SET NILAI KE KONTROL - SALES & USER
        ' ═══════════════════════════════════════════════════════════
        LblSales.Text = SALES
        ' ✅ PERBAIKAN FASE 1: Safe ComboBox selection dengan helper
        SetComboBoxValue(CmbSales, NAMASALES)
        TxtLogin.Text = USER
        TxtKomputer.Text = KOMPUTER

        ' ═══════════════════════════════════════════════════════════
        ' TRIGGER SMART PANEL ADJUSTMENT
        ' ═══════════════════════════════════════════════════════════
        Propertigbbayar() ' Ini akan adjust panel size berdasarkan bayarTransfer

        SetupFocusToGrid() ' ✅ GANTI DUPLIKASI
    End Sub


    Public Sub Editpenjualan()
        If TxtJenistransaksi.Text = "EditPenjualan" Then
            TxtFaktur.Text = TxtFaktur.Text
            DgvData.Rows.Clear()

            ' Mengisi DataGridView dari penjualan_detail
            Dim penjualanDetail As DataTable = GetPenjualanDetail(TxtFaktur.Text)
            'For Each row As DataRow In penjualanDetail.Rows
            '    Dim dgvRow As DataGridViewRow = DgvData.Rows(DgvData.Rows.Add())
            '    For i As Integer = 0 To penjualanDetail.Columns.Count - 1
            '        dgvRow.Cells(i).Value = row(i)
            '    Next
            For Each row As DataRow In penjualanDetail.Rows
                Dim dgvRow As DataGridViewRow = DgvData.Rows(DgvData.Rows.Add())

                dgvRow.Cells(0).Value = row("ID_BARANG")
                dgvRow.Cells(1).Value = row("NAMA_BARANG")
                dgvRow.Cells(2).Value = row("HARGA_BELI")
                dgvRow.Cells(3).Value = row("QTY")
                dgvRow.Cells(4).Value = row("SATUAN")
                dgvRow.Cells(5).Value = row("ISI_SATUAN")
                dgvRow.Cells(6).Value = row("HARGA_BELI_SATUAN")
                dgvRow.Cells(7).Value = row("HARGA_JUAL")
                dgvRow.Cells(8).Value = row("QTY_SATUAN")
                dgvRow.Cells(9).Value = row("DISKON_PERSEN")
                dgvRow.Cells(10).Value = row("DISKON_RP")
                dgvRow.Cells(11).Value = row("TOTAL_DISKON")
                dgvRow.Cells(12).Value = row("TOTAL_HARGA")

                ' Menambahkan SERIAL_NUMBER ke index 16 (kolom tidak urut)
                dgvRow.Cells(16).Value = row("SERIAL_NUMBER")



                ' Mengisi ComboBoxCell berdasarkan satuan
                Dim idBarang As String = dgvRow.Cells(0).Value.ToString()
                Dim satuanList As List(Of String) = GetSatuanBarang(idBarang, LblJenisPl.Text = "Partai")

                Dim comboCell As DataGridViewComboBoxCell = CType(dgvRow.Cells("Satuan"), DataGridViewComboBoxCell)
                comboCell.Items.Clear()
                comboCell.Items.AddRange(satuanList.ToArray())

                ' Mengisi stok barang
                Dim stok As Tuple(Of Decimal, Decimal) = GetStokBarang(idBarang, TxtFaktur.Text)
                dgvRow.Cells("StokToko").Value = stok.Item1
                dgvRow.Cells("StokGudang").Value = stok.Item2
            Next

            UpdateSemuaTotal()


        End If
    End Sub

    Private Function GetPenjualanDetail(faktur As String) As DataTable
        Dim dt As New DataTable()
        Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA, SERIAL_NUMBER FROM penjualan_detail WHERE FAKTUR_JUAL = ?", conn)
            cmd.Parameters.AddWithValue("@FAKTUR_JUAL", faktur)
            Using da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using
        Return dt
    End Function

    Private Function GetSatuanBarang(idBarang As String, isPartai As Boolean) As List(Of String)
        Dim satuanList As New List(Of String)()
        Dim query As String = If(isPartai, "SELECT DISTINCT SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR FROM tbl_barang WHERE ID_BARANG = ?", "SELECT DISTINCT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = ?")
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    For i As Integer = 0 To 2
                        Dim satuan As String = rd(i).ToString()
                        If Not String.IsNullOrEmpty(satuan) Then
                            satuanList.Add(satuan)
                        End If
                    Next
                End While
            End Using
        End Using
        Return satuanList
    End Function

    Private Function GetStokBarang(idBarang As String, faktur As String) As Tuple(Of Decimal, Decimal)
        Dim stokToko As Decimal = 0
        Dim stokGudang As Decimal = 0
        Dim qtySatuan As Decimal = 0

        ' Mendapatkan stok toko dan gudang
        Using cmd As New MySqlCommand("SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?", conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    stokToko = SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                    stokGudang = SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                End If
            End Using
        End Using

        ' ✅ PERBAIKAN: Tambahkan AS alias untuk SUM result
        Using cmd As New MySqlCommand("SELECT COALESCE(SUM(QTY_SATUAN), 0) AS QTY_SATUAN FROM penjualan_detail WHERE ID_BARANG = ? AND FAKTUR_JUAL = ?", conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            cmd.Parameters.AddWithValue("@FAKTUR_JUAL", faktur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' ✅ Sekarang column "QTY_SATUAN" ada dengan COALESCE
                    qtySatuan = SafeGetValue(Of Decimal)(rd, "QTY_SATUAN", 0D)
                End If
            End Using
        End Using

        ' Menyesuaikan stok berdasarkan lokasi barang
        If LblLokasiBarang.Text = "GUDANG" Then
            stokGudang += qtySatuan
        ElseIf LblLokasiBarang.Text = "TOKO" Then
            stokToko += qtySatuan
        End If

        Return Tuple.Create(stokToko, stokGudang)
    End Function

    ' Ganti handler ChkTampilSN_CheckedChanged dengan versi yang menyimpan setting
    Private Sub ChkTampilSN_CheckedChanged(sender As Object, e As EventArgs) Handles ChkTampilSN.CheckedChanged
        Dim chk As CheckBox = TryCast(sender, CheckBox)
        If chk Is Nothing Then Return

        If DgvData.Columns.Contains("SerialNumber") Then
            DgvData.Columns("SerialNumber").Visible = chk.Checked
        End If

        ' Simpan preferensi user agar permanen antar sesi
        Try
            AppConfig.Instance.SetValue("TampilSN", chk.Checked)
            AppConfig.Instance.Save()
        Catch ex As Exception
            ' Jangan ganggu UX bila save gagal; log jika perlu
        End Try
    End Sub


    ' ════════════════════════════════════════════════════════════════════════════════
    ' HELPER METHODS - FASE 1: CRASH PREVENTION
    ' ════════════════════════════════════════════════════════════════════════════════
    ' Methods ini menstandarisasi error handling dan mencegah crash points
    ' Tanggal: 2025-01-15 | Status: ✅ Integrated
    ' ════════════════════════════════════════════════════════════════════════════════

    ''' <summary>
    ''' HELPER 1: SafeGetValue - Standardized DBNull handling dengan generic type
    ''' Menggantikan 25+ inconsistent DBNull checks di seluruh form
    ''' </summary>
    Private Function SafeGetValue(Of T)(rd As MySqlDataReader, columnName As String, defaultValue As T) As T
        If rd Is Nothing Then Return defaultValue

        Try
            Dim ordinal As Integer = rd.GetOrdinal(columnName)
            If ordinal < 0 OrElse rd.IsDBNull(ordinal) Then
                Return defaultValue
            End If

            Dim value As Object = rd.GetValue(ordinal)
            If IsDBNull(value) Then
                Return defaultValue
            End If

            ' Handle conversion
            If TypeOf value Is T Then
                Return CType(value, T)
            Else
                Return CType(Convert.ChangeType(value, GetType(T)), T)
            End If

        Catch ex As Exception
            Debug.WriteLine($"SafeGetValue Error: {columnName} - {ex.Message}")
            Return defaultValue
        End Try
    End Function

    ''' <summary>
    ''' HELPER 2: ExecuteScalarSafe - Safe database execution dengan auto-reconnect
    ''' Menggantikan manual connection checks di GetKodeAkun() dan tempat lain
    ''' </summary>
    Private Function ExecuteScalarSafe(query As String, ParamArray params As MySqlParameter()) As Object
        If String.IsNullOrWhiteSpace(query) Then Return Nothing
        If conn Is Nothing Then Return Nothing

        Try
            Using cmd As New MySqlCommand(query, conn)
                ' Add parameters
                If params IsNot Nothing AndAlso params.Length > 0 Then
                    cmd.Parameters.AddRange(params)
                End If

                ' Ensure connection is open dengan retry
                If conn.State <> ConnectionState.Open Then
                    Try
                        conn.Close()
                    Catch
                    End Try
                    conn.Open()
                End If

                ' Execute dengan timeout handling
                cmd.CommandTimeout = 30
                Return cmd.ExecuteScalar()
            End Using

        Catch ex As Exception
            Debug.WriteLine($"ExecuteScalarSafe Error: {query} - {ex.Message}")
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' HELPER 3: SetComboBoxValue - Safe ComboBox selection
    ''' Menggantikan CmbPelanggan.SelectedIndex = FindStringExact() pattern
    ''' </summary>
    Private Sub SetComboBoxValue(cmb As ComboBox, value As String)
        If cmb Is Nothing Then Exit Sub

        If String.IsNullOrEmpty(value) Then
            cmb.SelectedIndex = -1
            Exit Sub
        End If

        Try
            Dim index As Integer = cmb.FindStringExact(value)
            If index >= 0 Then
                cmb.SelectedIndex = index
            Else
                cmb.SelectedIndex = -1
            End If
        Catch
            cmb.SelectedIndex = -1
        End Try
    End Sub

    ''' <summary>
    ''' HELPER 4: SafeGetCellValue - Safe DataGridViewCell access
    ''' Menggantikan direct cell.Value access yang bisa crash
    ''' </summary>
    Private Function SafeGetCellValue(row As DataGridViewRow, columnName As String) As Object
        If row Is Nothing Then Return Nothing
        If row.IsNewRow Then Return Nothing

        Try
            If DgvData.Columns.Contains(columnName) Then
                Dim cell As DataGridViewCell = row.Cells(columnName)
                If cell IsNot Nothing Then
                    Return cell.Value
                End If
            End If
        Catch
            ' Ignore
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' HELPER 5: SafeGetCellValueByIndex - Safe DataGridViewCell access with index
    ''' Alternative untuk akses cell dengan column index
    ''' </summary>
    Private Function SafeGetCellValueByIndex(row As DataGridViewRow, columnIndex As Integer) As Object
        If row Is Nothing Then Return Nothing
        If row.IsNewRow Then Return Nothing
        If columnIndex < 0 OrElse columnIndex >= DgvData.Columns.Count Then Return Nothing

        Try
            Dim cell As DataGridViewCell = row.Cells(columnIndex)
            If cell IsNot Nothing Then
                Return cell.Value
            End If
        Catch
            ' Ignore
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' HELPER 6: GetFormUtamaLocation - Safe FormUtama property access
    ''' Menggantikan FormUtama.SLokasi.Text access tanpa null check
    ''' </summary>
    Private Function GetFormUtamaLocation() As String
        Try
            If FormUtama IsNot Nothing AndAlso FormUtama.SLokasi IsNot Nothing Then
                Dim locationText As String = FormUtama.SLokasi.Text
                If Not String.IsNullOrWhiteSpace(locationText) Then
                    Return locationText
                End If
            End If
        Catch
            ' Ignore
        End Try

        Return "TOKO" ' Safe default
    End Function

    ''' <summary>
    ''' HELPER 7: GetFormUtamaLogin - Safe get Login from FormUtama
    ''' </summary>
    Private Function GetFormUtamaLogin() As String
        Try
            If FormUtama IsNot Nothing AndAlso FormUtama.SLogin IsNot Nothing Then
                Return FormUtama.SLogin.Text
            End If
        Catch
            ' Ignore
        End Try

        Return TxtLogin.Text ' Fallback
    End Function

    ''' <summary>
    ''' HELPER 8: GetFormUtamaComputer - Safe get Computer ID from FormUtama
    ''' </summary>
    Private Function GetFormUtamaComputer() As String
        Try
            If FormUtama IsNot Nothing AndAlso FormUtama.Comp IsNot Nothing Then
                Return FormUtama.Comp.Text
            End If
        Catch
            ' Ignore
        End Try

        Return TxtKomputer.Text ' Fallback
    End Function

    ''' <summary>
    ''' HELPER 9: ExecuteNonQuerySafe - Safe INSERT/UPDATE/DELETE execution
    ''' </summary>
    Private Function ExecuteNonQuerySafe(query As String, ParamArray params As MySqlParameter()) As Integer
        If String.IsNullOrWhiteSpace(query) Then Return 0
        If conn Is Nothing Then Return 0

        Try
            Using cmd As New MySqlCommand(query, conn)
                If params IsNot Nothing AndAlso params.Length > 0 Then
                    cmd.Parameters.AddRange(params)
                End If

                If conn.State <> ConnectionState.Open Then
                    Try
                        conn.Close()
                    Catch
                    End Try
                    conn.Open()
                End If

                cmd.CommandTimeout = 30
                Return cmd.ExecuteNonQuery()
            End Using

        Catch ex As Exception
            Debug.WriteLine($"ExecuteNonQuerySafe Error: {ex.Message}")
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' HELPER 10: ExecuteReaderSafe - Safe SELECT execution dengan data reader
    ''' </summary>
    Private Function ExecuteReaderSafe(query As String, ParamArray params As MySqlParameter()) As MySqlDataReader
        If String.IsNullOrWhiteSpace(query) Then Return Nothing
        If conn Is Nothing Then Return Nothing

        Try
            Dim cmd As New MySqlCommand(query, conn)
            If params IsNot Nothing AndAlso params.Length > 0 Then
                cmd.Parameters.AddRange(params)
            End If

            If conn.State <> ConnectionState.Open Then
                Try
                    conn.Close()
                Catch
                End Try
                conn.Open()
            End If

            cmd.CommandTimeout = 30
            Return cmd.ExecuteReader(CommandBehavior.CloseConnection)

        Catch ex As Exception
            Debug.WriteLine($"ExecuteReaderSafe Error: {ex.Message}")
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' HELPER 11: ValidateComboBoxSelection - Validasi combobox sebelum digunakan
    ''' </summary>
    Private Function ValidateComboBoxSelection(cmb As ComboBox, fieldName As String) As Boolean
        If cmb Is Nothing OrElse String.IsNullOrWhiteSpace(cmb.Text) Then
            MessageBox.Show(
                $"Harap pilih {fieldName} terlebih dahulu.",
                "Validasi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )
            cmb?.Focus()
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' HELPER 12: SafeParseDecimal - Safe decimal parsing dengan format Indonesia
    ''' Menggantikan multiple ParseDecimal patterns yang inconsistent
    ''' </summary>
    Private Function ParseDecimal(value As Object, Optional defaultValue As Decimal = 0D) As Decimal
        If value Is Nothing OrElse IsDBNull(value) Then Return defaultValue

        Dim s As String = value.ToString().Trim()
        If String.IsNullOrEmpty(s) Then Return defaultValue

        ' Normalisasi format angka
        If s.Contains(",") AndAlso s.Contains(".") Then
            s = s.Replace(".", "").Replace(",", ".")
        ElseIf s.Contains(",") Then
            s = s.Replace(",", ".")
        End If

        Dim result As Decimal
        If Decimal.TryParse(s, Globalization.NumberStyles.Any,
                        Globalization.CultureInfo.InvariantCulture, result) Then
            Return result
        End If

        Return defaultValue
    End Function

    ' ════════════════════════════════════════════════════════════════════════════════
    ' END OF HELPER METHODS
    ' ════════════════════════════════════════════════════════════════════════════════

End Class