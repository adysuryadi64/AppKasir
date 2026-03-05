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

            ' Cek keamanan agar tidak error jika FormUtama tertutup
            If FormUtama IsNot Nothing Then
                LblLokasiBarang.Text = FormUtama.SLokasi.Text
            Else
                LblLokasiBarang.Text = "TOKO" ' Default aman
            End If

            ' Ukuran Form
            MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
            MinimumSize = Size

            ' --- 2. SETUP KOMPONEN UI & TIMER ---
            CmbCetak.Text = My.Settings.CetakJual
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
            CmbJenisBayar.Items.Clear()
            CmbJenisBayar.Items.AddRange(GetAkunList().ToArray())

            ' --- 5. SETUP TANGGAL ---
            DTPTgl.Format = DateTimePickerFormat.Custom
            DTPTgl.CustomFormat = "dd/MM/yyyy HH:mm:ss"
            DTPJatuhTempo.Format = DateTimePickerFormat.Custom
            DTPJatuhTempo.CustomFormat = "dd/MM/yyyy"
            DTPJatuhTempo.Value = DTPJatuhTempo.Value.AddMonths(1)

            AmbilJenisPrinter()
            GBBayar.Visible = False

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
            Try
                ChkTampilSN.Checked = If(My.Settings.Item("TampilSN") IsNot Nothing, CBool(My.Settings.TampilSN), True)
            Catch
                ' Default
            End Try

            If DgvData.Columns.Contains("SerialNumber") Then
                DgvData.Columns("SerialNumber").Visible = ChkTampilSN.Checked
            End If

            ' ❌ BAGIAN IF/ELSE KONDISIAWAL DIHAPUS DARI SINI
            ' Kita pindahkan logika pemanggilannya ke Shown agar urutannya pasti

        Catch ex As Exception
            MessageBox.Show("Error Load: " & ex.Message)
        End Try
    End Sub

    Private Sub Form_Penjualan_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
        Try
            ' Logika pemisahan Mode Transaksi
            ' Kondisiawal() atau Editpenjualanheader() dipanggil di sini

            If TxtJenistransaksi.Text = "TambahPenjualan" Then

                ' Panggil reset form
                Kondisiawal()

                ' Optional: Set fokus ke pencarian setelah form muncul
                ' TxtCari.Focus() 
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
        ToolTip1.SetToolTip(CmbJenisBayar, "Pilih metode pembayaran seperti: Kas tunai, Transfer Bank, QRIS, atau metode lainnya.")
        ToolTip1.SetToolTip(TxtBank, "Isi nama bank yang digunakan untuk melakukan transfer, contoh: BCA, Mandiri, BRI, dll.")
        ToolTip1.SetToolTip(TxtNoRek, "Masukkan nomor rekening dari mana dana dikirim. Pastikan angka sesuai dengan bukti transfer.")
        ToolTip1.SetToolTip(TxtNamaRek, "Tulis nama pemilik rekening pengirim sesuai yang tertera pada bukti transfer.")
        ToolTip1.SetToolTip(TxtNoReff, "Isi nomor referensi transaksi transfer (jika tersedia) sebagai bukti tambahan transaksi.")
        ToolTip1.SetToolTip(LblBiayaKirim, "Masukkan biaya tambahan seperti ongkos kirim, biaya COD, dsb.")
        ToolTip1.SetToolTip(LblTotalStlPajak, "Jumlah total belanja setelah diskon dan sebelum pembayaran.")
        ToolTip1.SetToolTip(LblBayar,
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
        My.Settings.CetakJual = CmbCetak.Text
        My.Settings.Save()
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
        PanelCariNama.BackColor = Color.Yellow ' Ganti warna fokus sesuai kebutuhan

        ' Cek apakah DgvData memiliki baris
        If DgvData.Rows.Count > 0 Then
            ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
            DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

            ' Mengatur baris terakhir sebagai baris yang dipilih
            DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
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
        TxtTotaljualStlPajak.Text = 0
        TxtNominalBayar.Text = 0
        TxtKembaliHutang.Text = 0
        LblStatusTrans.Text = "Belum Lunas"

        TxtTotalJualSblDiskonPajak.Text = 0
        TxtGrantotal.Text = "Rp. 0"
        TxtDiskonPersen.Text = 0
        TxtDiskonRp.Text = 0
        TxtPajakPersen.Text = 0
        TxtPajakRp.Text = 0
        TxtBiayaKirim.Text = 0
        TxtBank.Clear()
        TxtNoRek.Clear()
        TxtNamaRek.Clear()
        TxtNoReff.Clear()

        TxtTypeAkun.Text = "TUNAI"

        If LblLokasiBarang.Text = "TOKO" Then
            CmbJenisBayar.SelectedItem = nama_rek_Jual_Toko
            TxtKodeRef.Text = Kode_rek_Jual_Toko
        ElseIf LblLokasiBarang.Text = "GUDANG" Then
            CmbJenisBayar.SelectedItem = nama_rek_Jual_Gudang
            TxtKodeRef.Text = Kode_rek_Jual_Gudang
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
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                'Mengumpulkan informasi barang
                Dim sql As String = "SELECT HARGA_BELI, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                   "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
                   "FROM tbl_barang " &
                   "WHERE TRIM(ID_BARANG) LIKE @ID_BARANG"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            dgvRow.Cells("HargaBeli").Value = If(Not IsDBNull(rd("HARGA_BELI")), ParseDecimal(rd("HARGA_BELI")), DBNull.Value)

                            If Not IsDBNull(dgvRow.Cells("Satuan").Value) Then
                                Dim nilaiSatuan As String = dgvRow.Cells("Satuan").Value.ToString()
                                Dim hargaValue As Decimal

                                If LblJenisPl.Text = "Partai" Then
                                    Select Case nilaiSatuan
                                        Case rd("SATUAN_PARTAI_SEDANG").ToString()
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_PARTAI_SEDANG"))
                                        Case rd("SATUAN_PARTAI_BESAR").ToString()
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_PARTAI_BESAR"))
                                        Case Else
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_PARTAI_KECIL"))
                                    End Select
                                Else
                                    Select Case nilaiSatuan
                                        Case rd("SATUAN_UMUM_SEDANG").ToString()
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_UMUM_SEDANG"))
                                        Case rd("SATUAN_UMUM_BESAR").ToString()
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_UMUM_BESAR"))
                                        Case Else
                                            hargaValue = ParseDecimal(rd("HARGA_JUAL_UMUM_KECIL"))
                                    End Select
                                End If

                                dgvRow.Cells("Harga").Value = hargaValue
                            End If

                            dgvRow.Cells("StokToko").Value = If(Not IsDBNull(rd("STOK_TOKO")), ParseDecimal(rd("STOK_TOKO")), DBNull.Value)
                            dgvRow.Cells("StokGudang").Value = If(Not IsDBNull(rd("STOK_GUDANG")), ParseDecimal(rd("STOK_GUDANG")), DBNull.Value)

                            Dim stokValue As Decimal = If(LblLokasiBarang.Text = "GUDANG",
                                                          If(Not IsDBNull(rd("STOK_GUDANG")), ParseDecimal(rd("STOK_GUDANG")), 0D),
                                                          If(Not IsDBNull(rd("STOK_TOKO")), ParseDecimal(rd("STOK_TOKO")), 0D))

                            dgvRow.Cells("Stok").Value = stokValue
                        End If
                    End Using
                End Using
            End If
        Next
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

        Dim query = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
                   "WHERE TRIM(ID_BARANG) LIKE @key " &
                   "   OR TRIM(NAMA_BARANG) LIKE @key " &
                   "   OR TRIM(BARCODE_KECIL) LIKE @key " &
                   "   OR TRIM(BARCODE_SEDANG) LIKE @key " &
                   "   OR TRIM(BARCODE_BESAR) LIKE @key " &
                   "ORDER BY NAMA_BARANG LIMIT 20"

        Try
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@key", "%" & searchKeyword & "%")

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    LstBarang.Items.Clear()

                    While rd.Read()
                        Dim itemText = rd("NAMA_BARANG").ToString()

                        Select Case LblLokasiBarang.Text
                            Case "TOKO"
                                Dim stok = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                                itemText &= " => " & stok.ToString("N0")
                            Case "GUDANG"
                                Dim stok = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))
                                itemText &= " => " & stok.ToString("N0")
                        End Select

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

    ' Modifikasi fungsi TampilkanDaftarBarang untuk handle barcode vs manual
    Private Sub TampilkanDaftarBarang(ByVal searchKeyword As String)
        ' Hanya tampilkan list jika input manual dan ada cukup karakter
        If searchKeyword.Length < 2 AndAlso Not searchKeyword.All(AddressOf Char.IsDigit) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        Dim query As String = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR " &
                         "FROM tbl_barang " &
                         "WHERE TRIM(ID_BARANG) LIKE @Nama " &
                         "   OR TRIM(NAMA_BARANG) LIKE @Nama " &
                         "   OR TRIM(BARCODE_KECIL) LIKE @Nama " &
                         "   OR TRIM(BARCODE_SEDANG) LIKE @Nama " &
                         "   OR TRIM(BARCODE_BESAR) LIKE @Nama " &
                         "ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Nama", "%" & searchKeyword & "%")

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                LstBarang.Items.Clear()

                While rd.Read()
                    Dim itemText As String = rd("NAMA_BARANG").ToString()

                    ' Tambahkan stok berdasarkan lokasi
                    Select Case LblLokasiBarang.Text
                        Case "TOKO"
                            Dim stokToko As Decimal = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                            itemText &= " => " & stokToko.ToString("N0")
                        Case "GUDANG"
                            Dim stokGudang As Decimal = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))
                            itemText &= " => " & stokGudang.ToString("N0")
                    End Select

                    LstBarang.Items.Add(itemText)
                End While

                ' Tampilkan ListBox hanya jika ada hasil
                LstBarang.Visible = LstBarang.Items.Count > 0
            End Using
        End Using
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
                        idBarang = If(Not IsDBNull(rd(0)), rd.GetString(0), String.Empty)
                        hargaBeli = If(Not IsDBNull(rd(2)), rd.GetDecimal(2).ToString(), String.Empty)

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
                                    ' Cek satuan partai
                                    Select Case level
                                        Case 3
                                            satuan = If(Not IsDBNull(rd(17)), rd.GetString(17), "")
                                            isiSatuan = If(Not IsDBNull(rd(20)), rd.GetInt32(20), 1)
                                            hargaJual = If(Not IsDBNull(rd(23)), rd.GetDecimal(23), 0)
                                        Case 2
                                            satuan = If(Not IsDBNull(rd(16)), rd.GetString(16), "")
                                            isiSatuan = If(Not IsDBNull(rd(19)), rd.GetInt32(19), 1)
                                            hargaJual = If(Not IsDBNull(rd(22)), rd.GetDecimal(22), 0)
                                        Case 1
                                            satuan = If(Not IsDBNull(rd(15)), rd.GetString(15), "")
                                            isiSatuan = If(Not IsDBNull(rd(18)), rd.GetInt32(18), 1)
                                            hargaJual = If(Not IsDBNull(rd(21)), rd.GetDecimal(21), 0)
                                    End Select
                                Else
                                    ' Cek satuan umum
                                    Select Case level
                                        Case 3
                                            satuan = If(Not IsDBNull(rd(8)), rd.GetString(8), "")
                                            isiSatuan = If(Not IsDBNull(rd(11)), rd.GetInt32(11), 1)
                                            hargaJual = If(Not IsDBNull(rd(14)), rd.GetDecimal(14), 0)
                                        Case 2
                                            satuan = If(Not IsDBNull(rd(7)), rd.GetString(7), "")
                                            isiSatuan = If(Not IsDBNull(rd(10)), rd.GetInt32(10), 1)
                                            hargaJual = If(Not IsDBNull(rd(13)), rd.GetDecimal(13), 0)
                                        Case 1
                                            satuan = If(Not IsDBNull(rd(6)), rd.GetString(6), "")
                                            isiSatuan = If(Not IsDBNull(rd(9)), rd.GetInt32(9), 1)
                                            hargaJual = If(Not IsDBNull(rd(12)), rd.GetDecimal(12), 0)
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
                                satuan = If(Not IsDBNull(rd(15)), rd.GetString(15), "")
                                isiSatuan = If(Not IsDBNull(rd(18)), rd.GetInt32(18), 1)
                                hargaJual = If(Not IsDBNull(rd(21)), rd.GetDecimal(21), 0)
                            Else
                                satuan = If(Not IsDBNull(rd(6)), rd.GetString(6), "")
                                isiSatuan = If(Not IsDBNull(rd(9)), rd.GetInt32(9), 1)
                                hargaJual = If(Not IsDBNull(rd(12)), rd.GetDecimal(12), 0)
                            End If
                        End If

                        ' Ambil stok
                        Dim stokToko As String = If(Not IsDBNull(rd("STOK_TOKO")), Convert.ToString(rd("STOK_TOKO")), "0")
                        Dim stokGudang As String = If(Not IsDBNull(rd("STOK_GUDANG")), Convert.ToString(rd("STOK_GUDANG")), "0")

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


    'Private Sub Ambildatalaindaridbbarang(ByVal namayangdiambil As String)
    '    Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
    '                        "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
    '                        "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
    '                        "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, " &
    '                        "HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
    '                        "FROM tbl_barang " &
    '                        "WHERE TRIM(NAMA_BARANG) = @NamaBarang OR BARCODE_KECIL = @NamaBarang OR BARCODE_SEDANG = @NamaBarang OR BARCODE_BESAR = @NamaBarang LIMIT 1"

    '    Dim idBarang As String = ""
    '    Dim hargaBeli As String = ""
    '    Dim satuan As String = ""
    '    Dim isiSatuan As Integer = 0
    '    Dim hargaJual As Decimal = 0

    '    ' BACA DATA DARI DATABASE - TUTUP READER SEBELUM PERINTAH LAIN
    '    Using cmd As New MySqlCommand(sql, conn)
    '        cmd.Parameters.AddWithValue("@NamaBarang", namayangdiambil)

    '        Try
    '            Using rd As MySqlDataReader = cmd.ExecuteReader()
    '                If rd.Read() Then
    '                    ' Ambil nilai dari database DALAM BLOK USING
    '                    idBarang = If(Not IsDBNull(rd(0)), rd.GetString(0), String.Empty)
    '                    hargaBeli = If(Not IsDBNull(rd(2)), rd.GetDecimal(2).ToString(), String.Empty)

    '                    ' Tentukan satuan, isi, dan harga jual berdasarkan jenis pelanggan
    '                    If LblJenisPl.Text = "Partai" Then
    '                        ' PARTAI
    '                        If Not String.IsNullOrEmpty(TxtBarcode.Text) Then
    '                            ' Ada barcode - sesuaikan dengan barcode
    '                            If TxtBarcode.Text = rd("BARCODE_SEDANG").ToString() Then
    '                                satuan = If(Not IsDBNull(rd(16)), rd.GetString(16), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(19)), rd.GetInt32(19), 1)
    '                                hargaJual = If(Not IsDBNull(rd(22)), rd.GetDecimal(22), 0)
    '                            ElseIf TxtBarcode.Text = rd("BARCODE_BESAR").ToString() Then
    '                                satuan = If(Not IsDBNull(rd(17)), rd.GetString(17), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(20)), rd.GetInt32(20), 1)
    '                                hargaJual = If(Not IsDBNull(rd(23)), rd.GetDecimal(23), 0)
    '                            Else
    '                                satuan = If(Not IsDBNull(rd(15)), rd.GetString(15), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(18)), rd.GetInt32(18), 1)
    '                                hargaJual = If(Not IsDBNull(rd(21)), rd.GetDecimal(21), 0)
    '                            End If
    '                        Else
    '                            ' Tidak ada barcode - gunakan TxtLevelSat
    '                            If TxtLevelSat.Text = "2" Then
    '                                satuan = If(Not IsDBNull(rd(16)), rd.GetString(16), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(19)), rd.GetInt32(19), 1)
    '                                hargaJual = If(Not IsDBNull(rd(22)), rd.GetDecimal(22), 0)
    '                            ElseIf TxtLevelSat.Text = "3" Then
    '                                satuan = If(Not IsDBNull(rd(17)), rd.GetString(17), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(20)), rd.GetInt32(20), 1)
    '                                hargaJual = If(Not IsDBNull(rd(23)), rd.GetDecimal(23), 0)
    '                            Else
    '                                satuan = If(Not IsDBNull(rd(15)), rd.GetString(15), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(18)), rd.GetInt32(18), 1)
    '                                hargaJual = If(Not IsDBNull(rd(21)), rd.GetDecimal(21), 0)
    '                            End If
    '                        End If
    '                    Else
    '                        ' UMUM (default)
    '                        If Not String.IsNullOrEmpty(TxtBarcode.Text) Then
    '                            If TxtBarcode.Text = rd("BARCODE_SEDANG").ToString() Then
    '                                satuan = If(Not IsDBNull(rd(7)), rd.GetString(7), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(10)), rd.GetInt32(10), 1)
    '                                hargaJual = If(Not IsDBNull(rd(13)), rd.GetDecimal(13), 0)
    '                            ElseIf TxtBarcode.Text = rd("BARCODE_BESAR").ToString() Then
    '                                satuan = If(Not IsDBNull(rd(8)), rd.GetString(8), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(11)), rd.GetInt32(11), 1)
    '                                hargaJual = If(Not IsDBNull(rd(14)), rd.GetDecimal(14), 0)
    '                            Else
    '                                satuan = If(Not IsDBNull(rd(6)), rd.GetString(6), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(9)), rd.GetInt32(9), 1)
    '                                hargaJual = If(Not IsDBNull(rd(12)), rd.GetDecimal(12), 0)
    '                            End If
    '                        Else
    '                            If TxtLevelSat.Text = "2" Then
    '                                satuan = If(Not IsDBNull(rd(7)), rd.GetString(7), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(10)), rd.GetInt32(10), 1)
    '                                hargaJual = If(Not IsDBNull(rd(13)), rd.GetDecimal(13), 0)
    '                            ElseIf TxtLevelSat.Text = "3" Then
    '                                satuan = If(Not IsDBNull(rd(8)), rd.GetString(8), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(11)), rd.GetInt32(11), 1)
    '                                hargaJual = If(Not IsDBNull(rd(14)), rd.GetDecimal(14), 0)
    '                            Else
    '                                satuan = If(Not IsDBNull(rd(6)), rd.GetString(6), String.Empty)
    '                                isiSatuan = If(Not IsDBNull(rd(9)), rd.GetInt32(9), 1)
    '                                hargaJual = If(Not IsDBNull(rd(12)), rd.GetDecimal(12), 0)
    '                            End If
    '                        End If
    '                    End If

    '                    ' Ambil stok
    '                    Dim stokToko As String = If(Not IsDBNull(rd("STOK_TOKO")), Convert.ToString(rd("STOK_TOKO")), "0")
    '                    Dim stokGudang As String = If(Not IsDBNull(rd("STOK_GUDANG")), Convert.ToString(rd("STOK_GUDANG")), "0")

    '                    TxtStokToko.Text = stokToko
    '                    TXtStokGudang.Text = stokGudang

    '                    If LblLokasiBarang.Text = "GUDANG" Then
    '                        TxtStok.Text = stokGudang
    '                    ElseIf LblLokasiBarang.Text = "TOKO" Then
    '                        TxtStok.Text = stokToko
    '                    End If
    '                Else
    '                    ' DATA TIDAK DITEMUKAN
    '                    MessageBox.Show("Barang '" & namayangdiambil & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '                    KosongTxtboxcari()
    '                    TxtNama.Focus()
    '                    Return
    '                End If
    '            End Using
    '        Catch ex As Exception
    '            MessageBox.Show("Error membaca data barang: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '            Return
    '        End Try
    '    End Using

    '    ' SETELAH READER DITUTUP, SET NILAI KE TEXTBOX
    '    TxtKode.Text = idBarang
    '    TxtHargaBeli.Text = hargaBeli
    '    Txtsatuan.Text = satuan
    '    TxtIsi.Text = isiSatuan.ToString()
    '    TxtHargaJual.Text = hargaJual.ToString()

    '    ' JANGAN TAMPILKAN LISTBOX - LANGSUNG TAMBAH KE DATAGRID
    '    LstBarang.Items.Clear()
    '    LstBarang.Visible = False

    '    ' Panggil TambahDataLangsung untuk menambah ke DataGridView
    '    TambahDataLangsung(namayangdiambil)
    'End Sub

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
    Private Function ParseDecimal(ByVal value As Object) As Decimal
        If value Is Nothing Then Return 0D

        ' Konversi ke string dan bersihkan
        Dim s As String = value.ToString().Trim()

        ' Ganti pemisah ribuan dan desimal secara cerdas:
        ' - Jika mengandung koma dan titik, asumsikan koma desimal (format Eropa) → hapus titik, ganti koma jadi titik
        ' - Jika hanya koma, ganti koma jadi titik
        If s.Contains(",") AndAlso s.Contains(".") Then
            s = s.Replace(".", "").Replace(",", ".") ' Contoh: 1.234,56 → 1234.56
        ElseIf s.Contains(",") Then
            s = s.Replace(",", ".") ' Contoh: 12,5 → 12.5
        End If

        ' Coba parsing menggunakan InvariantCulture (pemisah desimal = titik)
        Dim result As Decimal = 0
        Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, result)

        Return result
    End Function


    Private Function ToSafeDecimal(value As Object) As Decimal
        If value Is Nothing OrElse IsDBNull(value) Then Return 0D

        Dim strValue As String = value.ToString().Trim()
        If String.IsNullOrEmpty(strValue) Then Return 0D

        ' Coba parsing langsung (format internasional)
        Dim result As Decimal
        If Decimal.TryParse(strValue, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, result) Then
            Return result
        End If

        ' Jika gagal, coba normalisasi untuk format Indonesia (1.000,50 -> 1000.50)
        strValue = strValue.Replace(".", "").Replace(",", ".")
        Decimal.TryParse(strValue, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, result)

        Return result
    End Function



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
                                Dim satuanKecil = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_" & prefix & "_KECIL")), rd("SATUAN_" & prefix & "_KECIL").ToString(), "")
                                Dim satuanSedang = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_" & prefix & "_SEDANG")), rd("SATUAN_" & prefix & "_SEDANG").ToString(), "")
                                Dim satuanBesar = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_" & prefix & "_BESAR")), rd("SATUAN_" & prefix & "_BESAR").ToString(), "")

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

                            DgvData.Rows(e.RowIndex).Cells("StokToko").Value = rd("STOK_TOKO")
                            DgvData.Rows(e.RowIndex).Cells("StokGudang").Value = rd("STOK_GUDANG")
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
        ' === Hitung Total Harga Beli (HPP) ===
        Dim totalHpp As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Totalhargabeli").Value IsNot Nothing Then
                totalHpp += Math.Round(ParseDecimal(row.Cells("Totalhargabeli").Value))
            End If
        Next
        TxtTotalHpp.Text = totalHpp.ToString()

        ' === Hitung Grand Total Harga Jual ===
        Dim totalGrand As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Totalharga").Value IsNot Nothing Then
                totalGrand += Math.Round(ParseDecimal(row.Cells("Totalharga").Value))
            End If
        Next
        TxtTotalJualSblDiskonPajak.Text = totalGrand.ToString()
        LblJualSblDiskon.Text = totalGrand.ToString("#,0.##", cultureIndonesia)

        ' === Hitung Jumlah Barang dan Jumlah Item ===
        Dim totalQtyBarang As Decimal = 0
        Dim totalItemCount As Integer = 0
        For Each row As DataGridViewRow In DgvData.Rows
            Dim qtyObj As Object = row.Cells("Qty").Value
            If Not row.IsNewRow AndAlso qtyObj IsNot Nothing AndAlso Not String.IsNullOrEmpty(qtyObj.ToString()) Then
                totalQtyBarang += Math.Round(ParseDecimal(qtyObj))
                totalItemCount += 1
            End If
        Next
        TxtJmlhQty.Text = totalQtyBarang.ToString()
        TxtJmlhItem.Text = totalItemCount.ToString()
        LblRecord.Text = "Total record : " & totalItemCount.ToString()

        ' === Hitung Total QTY Satuan ===
        Dim totalQtySat As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("QtySat").Value IsNot Nothing Then
                totalQtySat += Math.Round(ParseDecimal(row.Cells("QtySat").Value), 2)
            End If
        Next
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
                TxtDiskonRp.Text = diskonRupiah.ToString()

            Case "diskonrupiah"
                diskonPersen = If(totalSebelumDiskon = 0, 0, Math.Round((diskonRupiah / totalSebelumDiskon) * 100, 2))
                TxtDiskonPersen.Text = diskonPersen.ToString("#,0.##", cultureIndonesia)
        End Select

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
                TxtPajakRp.Text = pajakRupiah.ToString()

            Case "pajakrupiah"
                pajakPersen = If(totalSebelumPajak = 0, 0, Math.Round((pajakRupiah / totalSebelumPajak) * 100, 2))
                TxtPajakPersen.Text = pajakPersen.ToString("#,0.##", cultureIndonesia)
        End Select

        LblPajakRp.Text = "Rp. " & pajakRupiah.ToString("#,0.##", cultureIndonesia)

        HitungTotalPenjualanAkhir()
        isUpdatingPajak = False
    End Sub


    Private Sub TxtBiayaKirim_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtBiayaKirim.TextChanged

        Dim biayaKirim As Decimal = 0

        ' Parsing Biaya Kirim
        If String.IsNullOrEmpty(TxtBiayaKirim.Text) Or Not Decimal.TryParse(TxtBiayaKirim.Text, biayaKirim) Then
            LblBiayaKirim.Text = "0"
        Else
            LblBiayaKirim.Text = biayaKirim.ToString("#,0.##", cultureIndonesia)
        End If

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
        TxtTotaljualStlPajak.Text = totalSetelahPajak.ToString()
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
        GBBayar.Visible = False
        TxtNominalBayar.Text = 0
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
                CmbJenisBayar.Select()
                CmbJenisBayar.DroppedDown = True

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

        ' Atur nilai textbox pembayaran
        TxtNominalBayar.Text = If(Isinominal = "Tidak", "", TxtTotaljualStlPajak.Text)

        ' Tampilkan panel bayar dan arahkan fokus
        CenterPanelBayar()
        GBBayar.Visible = True
        TxtNominalBayar.Focus()
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
        If TxtFaktur.Text = "" Then
            Exit Sub
        Else
            If TxtJenistransaksi.Text = "TambahPenjualan" Then
                DgvData.DataSource = Nothing
                DgvData.Rows.Clear()

                ' Dictionary untuk menyimpan daftar satuan berdasarkan ID_BARANG
                Dim satuanBarang As New Dictionary(Of String, List(Of String))

                ' Query satu kali untuk mengambil semua satuan
                Dim querySatuan As String = "SELECT ID_BARANG, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang"
                Using cmdSatuan As New MySqlCommand(querySatuan, conn)
                    Using rdSatuan As MySqlDataReader = cmdSatuan.ExecuteReader()
                        While rdSatuan.Read()
                            Dim idBarang As String = rdSatuan("ID_BARANG").ToString()
                            Dim listSatuan As New List(Of String)

                            If LblJenisPl.Text = "Partai" Then
                                ' Tambahkan satuan partai ke daftar
                                For i As Integer = 1 To 3 ' Kolom SATUAN_PARTAI_KECIL, SEDANG, BESAR
                                    Dim satuan As String = rdSatuan(i).ToString()
                                    If Not String.IsNullOrEmpty(satuan) Then listSatuan.Add(satuan)
                                Next
                            Else
                                ' Tambahkan satuan umum ke daftar
                                For i As Integer = 4 To 6 ' Kolom SATUAN_UMUM_KECIL, SEDANG, BESAR
                                    Dim satuan As String = rdSatuan(i).ToString()
                                    If Not String.IsNullOrEmpty(satuan) Then listSatuan.Add(satuan)
                                Next
                            End If

                            ' Simpan ke dictionary
                            satuanBarang(idBarang) = listSatuan
                        End While
                    End Using
                End Using

                ' Query utama untuk data penjualan_ditahan_detail
                Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, SERIAL_NUMBER, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, QTY_SATUAN, DISKON_PERSEN, DISKON_RP, TOTAL_DISKON, TOTAL_HARGA, TOKO, GUDANG, STOK, SISA FROM penjualan_ditahan_detail WHERE FAKTUR_JUAL = ?"

                ' Query utama untuk data penjualan_ditahan_detail
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)

                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        While rd.Read()
                            ' Tambahkan baris ke DataGridView
                            Dim row As DataGridViewRow = DgvData.Rows(DgvData.Rows.Add())
                            row.Cells(0).Value = rd("ID_BARANG")
                            row.Cells(1).Value = rd("NAMA_BARANG")
                            row.Cells(16).Value = rd("SERIAL_NUMBER") ' Tambahkan SERIAL_NUMBER ke kolom 16
                            row.Cells(2).Value = rd("HARGA_BELI")
                            row.Cells(3).Value = rd("QTY")
                            row.Cells(4).Value = rd("SATUAN")
                            row.Cells(5).Value = rd("ISI_SATUAN")
                            row.Cells(6).Value = rd("HARGA_BELI_SATUAN")
                            row.Cells(7).Value = rd("HARGA_JUAL")
                            row.Cells(8).Value = rd("QTY_SATUAN")
                            row.Cells(9).Value = rd("DISKON_PERSEN")
                            row.Cells(10).Value = rd("DISKON_RP")
                            row.Cells(11).Value = rd("TOTAL_DISKON")
                            row.Cells(12).Value = rd("TOTAL_HARGA")
                            row.Cells(13).Value = rd("TOKO")
                            row.Cells(14).Value = rd("GUDANG")
                            row.Cells(15).Value = rd("STOK")

                            ' Ambil ID_BARANG untuk mengisi ComboBoxCell
                            Dim idBarang As String = row.Cells(0).Value.ToString()
                            If satuanBarang.ContainsKey(idBarang) Then
                                Dim comboCell As DataGridViewComboBoxCell = CType(row.Cells("Satuan"), DataGridViewComboBoxCell)
                                comboCell.Items.Clear()
                                comboCell.Items.AddRange(satuanBarang(idBarang).ToArray())
                            End If
                        End While
                    End Using
                End Using


                UpdateSetelahTahan()
                UpdateSemuaTotal()
            End If
        End If
    End Sub


    Private Sub UpdateSetelahTahan()
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                ' Mengumpulkan informasi barang
                Dim sql As String = "SELECT HARGA_BELI, STOK_TOKO, STOK_GUDANG " &
                   "FROM tbl_barang " &
                   "WHERE TRIM(ID_BARANG) LIKE @ID_BARANG"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            dgvRow.Cells("HargaBeli").Value = If(IsDBNull(rd("HARGA_BELI")), 0D, ParseDecimal(rd("HARGA_BELI")))
                            dgvRow.Cells("StokToko").Value = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                            dgvRow.Cells("StokGudang").Value = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))

                            Dim stokValue As Decimal = If(LblLokasiBarang.Text = "GUDANG",
                                                          If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG"))),
                                                          If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO"))))

                            dgvRow.Cells("Stok").Value = stokValue
                        End If
                    End Using
                End Using
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

    Public Function CekStok() As Boolean
        ' Membuat Dictionary untuk menyimpan informasi stok barang dan penjualan
        Dim stokDict As New Dictionary(Of String, StokInfo) ' (stokToko, stokGudang)

        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                ' Mengumpulkan informasi stok barang
                Using cmd As New MySqlCommand("SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE trim(ID_BARANG) like @ID_BARANG", conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            Dim stokToko As Decimal = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                            Dim stokGudang As Decimal = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))

                            stokDict(kodeBarangValue) = New StokInfo() With {
                                .StokToko = stokToko,
                                .StokGudang = stokGudang
                            }
                        End If
                    End Using
                End Using

                If TxtJenistransaksi.Text <> "TambahPenjualan" Then
                    ' Mengumpulkan informasi penjualan
                    Using cmdjual As New MySqlCommand("SELECT ID_BARANG, SUM(QTY_SATUAN) AS TOTAL_QTY FROM penjualan_detail WHERE FAKTUR_JUAL = @FAKTUR_JUAL AND ID_BARANG = @ID_BARANG GROUP BY ID_BARANG", conn)
                        cmdjual.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)
                        cmdjual.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                        Using rdjual As MySqlDataReader = cmdjual.ExecuteReader()
                            While rdjual.Read()
                                Dim idBarang As String = rdjual("ID_BARANG").ToString()

                                ' Memastikan TOTAL_QTY tidak DBNull sebelum konversi
                                Dim totalQtyTerjualRow As Decimal = If(IsDBNull(rdjual("TOTAL_QTY")), 0D, ParseDecimal(rdjual("TOTAL_QTY")))

                                If stokDict.ContainsKey(idBarang) Then
                                    Dim stokInfo As StokInfo = stokDict(idBarang)

                                    If LblLokasiBarang.Text = "TOKO" Then
                                        stokInfo.StokToko += totalQtyTerjualRow
                                    Else
                                        stokInfo.StokGudang += totalQtyTerjualRow
                                    End If
                                End If
                            End While
                        End Using
                    End Using
                End If

                ' Memproses data di DataGridView
                Dim totalQtyTerjual As Decimal = If(IsDBNull(dgvRow.Cells("QtySat").Value), 0D, ParseDecimal(dgvRow.Cells("QtySat").Value))
                If stokDict.ContainsKey(kodeBarangValue) Then
                    Dim stokInfo As StokInfo = stokDict(kodeBarangValue)
                    Dim totalStok As Decimal

                    If LblLokasiBarang.Text = "TOKO" Then
                        totalStok = stokInfo.StokToko
                    Else
                        totalStok = stokInfo.StokGudang
                    End If

                    If totalQtyTerjual > totalStok Then
                        ' Stok tidak mencukupi
                        Dim errorMessage As String = "Stok ==> " & dgvRow.Cells("NamaBarang").Value & " <== tidak mencukupi untuk dijual. " & vbCrLf & vbCrLf & "Total Terjual: " & totalQtyTerjual & vbCrLf & ", Total Stok: " & totalStok
                        MessageBox.Show(errorMessage, "Stok Tidak cukup", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        ' Menyorot baris yang bermasalah
                        dgvRow.Selected = True
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = Color.Red
                        Next

                        ' Setelah menyaring baris, pastikan bahwa baris tersebut terpilih juga
                        dgvRow.DataGridView.Focus()
                        dgvRow.DataGridView.CurrentCell = dgvRow.Cells(1) ' Pilih sel pertama atau sesuaikan dengan indeks kolom yang ingin Anda pilih
                        dgvRow.DataGridView.CurrentRow.Selected = True

                        Return True ' Ada masalah
                    End If

                    ' Simpan warna default
                    Dim defaultBackColor As Color = dgvRow.DefaultCellStyle.BackColor

                    ' Kemudian, untuk mengembalikan sel kembali ke warna default
                    For Each cell As DataGridViewCell In dgvRow.Cells
                        cell.Style.BackColor = defaultBackColor
                    Next
                End If
            End If
        Next

        Return False
    End Function

    Public Function Cekjualrugi() As Boolean
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Kode").Value IsNot Nothing AndAlso dgvRow.Cells("Kode").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Kode").Value.ToString()

                Dim Hargabeli As Decimal = 0
                ' Mengumpulkan informasi barang
                Using cmd As New MySqlCommand("SELECT HARGA_BELI FROM tbl_barang WHERE trim(ID_BARANG) like @ID_BARANG", conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            ' Memastikan HARGA_BELI tidak DBNull sebelum konversi
                            Hargabeli = If(IsDBNull(rd("HARGA_BELI")), 0D, ParseDecimal(rd("HARGA_BELI")))
                        End If
                    End Using
                End Using

                ' Memproses data di DataGridView
                Dim Hargajual As Decimal
                If Decimal.TryParse(dgvRow.Cells("Harga").Value.ToString(), Hargajual) Then
                    If Hargabeli > Hargajual Then
                        ' Harga jual rugi
                        Dim errorMessage As String = "Barang: " & dgvRow.Cells("NamaBarang").Value & vbCrLf & vbCrLf & "Harga beli: " & Hargabeli.ToString("N0") & vbCrLf & vbCrLf & "Harga jual: " & Hargajual.ToString("N0")
                        MessageBox.Show(errorMessage, "Harga jual rugi", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        ' Menyorot baris yang bermasalah
                        dgvRow.Selected = True
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = Color.Red
                        Next

                        ' Setelah menyaring baris, pastikan bahwa baris tersebut terpilih juga
                        dgvRow.DataGridView.Focus()
                        dgvRow.DataGridView.CurrentCell = dgvRow.Cells(1) ' Pilih sel pertama atau sesuaikan dengan indeks kolom yang ingin Anda pilih
                        dgvRow.DataGridView.CurrentRow.Selected = True

                        Return True ' Ada masalah
                    End If
                Else
                    ' Penanganan ketika Hargajual tidak dapat di-parse
                    MessageBox.Show("Harga jual tidak valid untuk barang " & kodeBarangValue, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If


                ' Simpan warna default
                Dim defaultBackColor As Color = dgvRow.DefaultCellStyle.BackColor

                ' Kemudian, untuk mengembalikan sel kembali ke warna default
                For Each cell As DataGridViewCell In dgvRow.Cells
                    cell.Style.BackColor = defaultBackColor
                Next
            End If
        Next

        Return False
    End Function


    Private Sub TxtBayar_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtNominalBayar.KeyPress
        ' Memeriksa apakah karakter yang dimasukkan adalah angka atau kontrol seperti backspace
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True ' Mengabaikan karakter yang tidak diizinkan
        End If
        If e.KeyChar = Chr(13) Then
            If PanelTFPelanggan.Visible = True And TxtNamaRek.Text = "" Then
                TxtNamaRek.Focus()
            Else
                TekanSimpan()
            End If
        End If
    End Sub


    Private Sub CmbJenisBayar_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbJenisBayar.SelectedIndexChanged
        ' Panggil metode untuk mengambil informasi pelanggan
        AmbiuldataRekening()
    End Sub

    Private Sub CmbJenisBayar_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbJenisBayar.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            If PanelTFPelanggan.Visible = True Then
                TxtBank.Focus()
            Else
                TxtNominalBayar.Focus()
            End If
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub


    Dim typeakun As String
    Private Sub AmbiuldataRekening()
        Dim namaAkunD As String = CmbJenisBayar.Text

        Dim sql As String = "SELECT Type_Akun, Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtKodeRef.Text = reader("Kode_akun").ToString()
                    typeakun = reader("Type_Akun").ToString()

                    If typeakun = "BANK" Then
                        ' Set properti untuk kondisi BANK
                        GBBayar.Size = New Size(933, 280)
                        BtnSimpann.Location = New Point(536, 230)
                        BtnBatal.Location = New Point(737, 230)

                        PanelTFPelanggan.Visible = True
                        TxtTypeAkun.Text = "BANK"
                    Else
                        TxtBank.Clear()
                        TxtNoRek.Clear()
                        TxtNamaRek.Clear()
                        TxtNoReff.Clear()
                        ' Set properti untuk kondisi selain BANK (misalnya, TUNAI)
                        GBBayar.Size = New Size(529, 280)
                        BtnSimpann.Location = New Point(115, 230)
                        BtnBatal.Location = New Point(316, 230)

                        PanelTFPelanggan.Visible = False
                        TxtTypeAkun.Text = "TUNAI"
                    End If

                End If
            End Using
        End Using

    End Sub

    Private bantuanBayar As Decimal
    Private kas As Decimal
    Private bayar As Decimal
    Private kembali As Decimal

    Private Sub TxtBayar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalBayar.TextChanged, TxtTotaljualStlPajak.TextChanged
        Dim input As String = TxtNominalBayar.Text.Trim()

        ' Memeriksa apakah input kosong atau tidak valid
        If String.IsNullOrEmpty(input) OrElse Not Decimal.TryParse(input, bayar) Then
            TxtKembaliHutang.Text = TxtTotaljualStlPajak.Text
            LblBayar.Text = "0"
            Exit Sub
        End If

        ' Format jumlah bayar
        LblBayar.Text = bayar.ToString("#,0.##", cultureIndonesia)


        Dim total As Decimal
        If Decimal.TryParse(TxtTotaljualStlPajak.Text, total) Then
            bantuanBayar = total - bayar
            TxtKembaliHutang.Text = Math.Abs(bantuanBayar)
            kembali = Math.Abs(bantuanBayar)
            kas = bayar - kembali

            ' Menampilkan elemen UI sesuai dengan kondisi

            LblJatuhTempo.Visible = False
            DTPJatuhTempo.Visible = False

            ' Memeriksa apakah ada hutang atau kembali
            If bantuanBayar > 0 Then
                LblPembayaran.Text = "Hutang :"
                LblStatusTrans.Text = "Belum Lunas"
                LblJatuhTempo.Visible = True
                DTPJatuhTempo.Visible = True
            Else
                LblPembayaran.Text = "Kembali :"
                LblStatusTrans.Text = "Lunas"
                LblJatuhTempo.Visible = False
                DTPJatuhTempo.Visible = False
            End If
        End If
    End Sub

    Private Sub PanelTFPelanggan_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelTFPelanggan.VisibleChanged
        If TxtJenistransaksi.Text = "TambahPenjualan" Then
            TxtBank.Clear()
            TxtNoRek.Clear()
            TxtNamaRek.Clear()
            TxtNoReff.Clear()
        End If

    End Sub

    Public Sub TekanSimpan()

        ' === Ambil & normalisasi data ===
        Dim tipeAkun As String = TxtTypeAkun.Text.Trim().ToUpper()
        Dim nominal As Decimal = 0D
        Decimal.TryParse(TxtNominalBayar.Text, nominal)

        ' === VALIDASI 1: Transfer wajib nominal > 0 ===
        If tipeAkun = "BANK" AndAlso nominal <= 0 Then
            MessageBox.Show(
            "Jika pembayaran melalui BANK (Transfer), nominal harus diisi sesuai transfer.",
            "Peringatan",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        )
            TxtNominalBayar.Focus()
            Exit Sub
        End If

        ' === VALIDASI 2: Belum lunas wajib pilih pelanggan ===
        If LblStatusTrans.Text = "Belum Lunas" AndAlso
       String.IsNullOrWhiteSpace(CmbPelanggan.Text) Then

            MessageBox.Show(
            "Jika pembayaran belum lunas, pelanggan harus dipilih.",
            "Peringatan",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        )
            CmbPelanggan.DroppedDown = True
            Exit Sub
        End If

        ' === VALIDASI 3: Nominal kosong / nol (KHUSUS NON-BANK) ===
        If tipeAkun <> "BANK" AndAlso nominal <= 0 Then

            Dim metodeBayar As String =
        If(tipeAkun = "", "Tunai", tipeAkun)

            Dim res As DialogResult = MessageBox.Show(
        $"Pembayaran dengan metode {metodeBayar} tanpa nominal." & vbCrLf &
        "Transaksi akan disimpan sebagai BELUM LUNAS." & vbCrLf & vbCrLf &
        "Apakah Anda ingin melanjutkan?",
        "Konfirmasi Pembayaran",
        MessageBoxButtons.OKCancel,
        MessageBoxIcon.Question
    )

            If res <> DialogResult.OK Then
                TxtNominalBayar.Focus()
                Exit Sub
            End If

        End If


        ' === SIMPAN ===
        Simpanatauedit()

    End Sub


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
        If String.IsNullOrEmpty(TxtNominalBayar.Text) OrElse TxtNominalBayar.Text = "0" Then
            TxtNominalBayar.Text = "0"
            TxtTypeAkun.Text = "PIUTANG"
        End If

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


                ' ✅ PERBAIKAN: Simpan nilai kembalian ke variabel level form
                If Decimal.TryParse(TxtKembaliHutang.Text, kembaliAmount) AndAlso kembaliAmount > 0 Then
                    ' Nilai sudah tersimpan di kembaliAmount
                Else
                    kembaliAmount = 0
                End If

                Kondisiawal()

                ' ✅ TAMBAHAN: Tampilkan pesan kembalian SETELAH form bersih
                If kembaliAmount > 0 Then
                    TampilkanPesanKembaliPelanggan(kembaliAmount)
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
        ' menghitung laba dari data di DataGridView (DgvData)

        Dim totalHarga As Decimal = 0
        Dim totalHargaBeli As Decimal = 0
        Dim diskon As Decimal = 0

        ' Loop melalui setiap baris di DgvData untuk menghitung total harga dan total harga beli
        For Each row As DataGridViewRow In DgvData.Rows
            ' Tambahkan totalHarga dan totalHargaBeli dengan nilai dari kolom, anggap 0 jika null
            totalHarga += If(IsDBNull(row.Cells("TotalHarga").Value), 0D, ParseDecimal(row.Cells("TotalHarga").Value))
            totalHargaBeli += If(IsDBNull(row.Cells("Totalhargabeli").Value), 0D, ParseDecimal(row.Cells("Totalhargabeli").Value))
        Next

        ' Cek apakah TxtDiskonRp memiliki nilai dan konversi ke Decimal
        If Decimal.TryParse(TxtDiskonRp.Text, diskon) = False Then
            diskon = 0
        End If

        ' Hitung laba
        Dim laba As Decimal = (totalHarga - totalHargaBeli) - diskon



        Dim query As String = "INSERT INTO penjualan (ID_PENJUALAN, ID_PELANGGAN, NAMA_PELANGGAN, ALAMAT_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, " &
                        "TGL_TRANSAKSI, GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP, PAJAK_PERSEN, PAJAK_RP, " &
                        "GRAND_TOTAL_STL_PAJAK, LABA, BAYAR, TOTAL_HPP, BIAYA_KIRIM, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_BAYAR, STATUS_TRANSAKSI, " &
                        "TYPE_AKUN, KODE_AKUN, JENIS_PEMBAYARAN, METODE, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI, " &
                        "ID_SALES, NAMA_SALES, ID_USER, ID_KOMPUTER) " &
                        "VALUES (@ID_PENJUALAN, @ID_PELANGGAN, @NAMA_PELANGGAN, @ALAMAT_PELANGGAN, @JENIS_PELANGGAN, @LOKASIBARANG, " &
                        "@TGL_TRANSAKSI, @GRAND_TOTAL_SBL_PAJAK, @DISKON_TOTAL_PERSEN, @DISKON_TOTAL_RP, @PAJAK_PERSEN, @PAJAK_RP, " &
                        "@GRAND_TOTAL_STL_PAJAK, @LABA, @BAYAR, @TOTAL_HPP, @BIAYA_KIRIM, @KEMBALI, @SISA_TAGIHAN, @JATUH_TEMPO, @STATUS_BAYAR, @STATUS_TRANSAKSI, " &
                        "@TYPE_AKUN, @KODE_AKUN, @JENIS_PEMBAYARAN, @METODE, @BANK, @NO_REKENING, @NAMA_REKENING, @NO_REFFERENSI, " &
                        "@ID_SALES, @NAMA_SALES, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(query, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtFaktur.Text)
            cmd.Parameters.AddWithValue("@ID_PELANGGAN", LbLKodePel.Text)
            cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbPelanggan.Text)
            cmd.Parameters.AddWithValue("@ALAMAT_PELANGGAN", LblAlamat.Text)
            cmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPl.Text)
            cmd.Parameters.AddWithValue("@LOKASIBARANG", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPTgl.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_SBL_PAJAK", ParseDecimal(TxtTotalJualSblDiskonPajak.Text))
            cmd.Parameters.AddWithValue("@DISKON_TOTAL_PERSEN", ParseDecimal(TxtDiskonPersen.Text))
            cmd.Parameters.AddWithValue("@DISKON_TOTAL_RP", ParseDecimal(TxtDiskonRp.Text))
            cmd.Parameters.AddWithValue("@PAJAK_PERSEN", ParseDecimal(TxtPajakPersen.Text))
            cmd.Parameters.AddWithValue("@PAJAK_RP", ParseDecimal(TxtPajakRp.Text))
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_STL_PAJAK", ParseDecimal(TxtTotaljualStlPajak.Text))
            cmd.Parameters.AddWithValue("@LABA", laba)
            cmd.Parameters.AddWithValue("@BAYAR", ParseDecimal(TxtNominalBayar.Text))
            cmd.Parameters.AddWithValue("@TOTAL_HPP", ParseDecimal(TxtTotalHpp.Text)) 'Di isi hpp pembelian barang
            cmd.Parameters.AddWithValue("@BIAYA_KIRIM", ParseDecimal(TxtBiayaKirim.Text))

            Dim statusbayar As String
            If LblStatusTrans.Text = "Lunas" Then
                statusbayar = "TERBAYAR"
                cmd.Parameters.AddWithValue("@KEMBALI", ParseDecimal(TxtKembaliHutang.Text))
                cmd.Parameters.AddWithValue("@SISA_TAGIHAN", 0)
                cmd.Parameters.AddWithValue("@JATUH_TEMPO", DBNull.Value)
            Else
                statusbayar = "TERHUTANG"
                cmd.Parameters.AddWithValue("@KEMBALI", 0)
                cmd.Parameters.AddWithValue("@SISA_TAGIHAN", ParseDecimal(TxtKembaliHutang.Text))
                cmd.Parameters.AddWithValue("@JATUH_TEMPO", DTPJatuhTempo.Value.ToString("yyyy-MM-dd"))
            End If

            cmd.Parameters.AddWithValue("@STATUS_BAYAR", statusbayar)
            cmd.Parameters.AddWithValue("@STATUS_TRANSAKSI", LblStatusTrans.Text)
            cmd.Parameters.AddWithValue("@TYPE_AKUN", TxtTypeAkun.Text)
            cmd.Parameters.AddWithValue("@KODE_AKUN", TxtKodeRef.Text)
            cmd.Parameters.AddWithValue("@JENIS_PEMBAYARAN", CmbJenisBayar.Text)
            Dim metode As String
            If TxtTypeAkun.Text = "Bank" Then
                metode = "Transfer"
            Else
                metode = "Tunai"
            End If

            cmd.Parameters.AddWithValue("@METODE", metode)

            cmd.Parameters.AddWithValue("@BANK", TxtBank.Text)
            cmd.Parameters.AddWithValue("@NO_REKENING", TxtNoRek.Text)
            cmd.Parameters.AddWithValue("@NAMA_REKENING", TxtNamaRek.Text)
            cmd.Parameters.AddWithValue("@NO_REFFERENSI", TxtNoReff.Text)
            cmd.Parameters.AddWithValue("@ID_SALES", LblSales.Text)
            cmd.Parameters.AddWithValue("@NAMA_SALES", CmbSales.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.SLogin.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(TxtJenistransaksi.Text = "TambahPenjualan", FormUtama.Comp.Text, TxtKomputer.Text))

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
                cmdSimpanHutang.Parameters.AddWithValue("@BAYAR", ParseDecimal(TxtNominalBayar.Text))
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
        Dim nominalkas As Decimal

        ' KAS/BANK
        If bantuanBayar < 0 Then
            nominalkas = kas
        Else
            nominalkas = bayar
        End If

        ' PERSEDIAAN BARANG
        Dim persediaanbarang As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow Then
                persediaanbarang += ParseDecimal(row.Cells("Totalhargabeli").Value)
            End If
        Next

        ' LABA KOTOR PENJUALAN
        Dim labakotor As Decimal = ParseDecimal(TxtTotalJualSblDiskonPajak.Text) - persediaanbarang

        ' SIMPAN JURNAL KAS atau PIUTANG
        If bayar > 0 Then
            Dim uraian As String
            If bantuanBayar <= 0 Then
                uraian = "Dibayar lunas penjualan dari " & CmbPelanggan.Text
            Else
                uraian = "Uang muka pembayaran penjualan dari " & CmbPelanggan.Text
            End If

            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, uraian, CmbJenisBayar.Text, TxtKodeRef.Text, "", "", nominalkas, "Penjualan", "", "")
        Else
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Piutang penjualan dari " & CmbPelanggan.Text, nama_rek_Piutang_Jual, Kode_rek_Piutang_Jual, "", "", kembali, "Penjualan", CmbPelanggan.Text, LbLKodePel.Text)
        End If

        ' SIMPAN JURNAL SISA PIUTANG
        If bayar > 0 And bantuanBayar > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Piutang penjualan dari " & CmbPelanggan.Text, nama_rek_Piutang_Jual, Kode_rek_Piutang_Jual, "", "", kembali, "Penjualan", CmbPelanggan.Text, LbLKodePel.Text)
        End If

        ' SIMPAN JURNAL DISKON ITEM
        Dim diskontotal As Decimal = 0
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow Then
                diskontotal += ParseDecimal(row.Cells("TotalDiskon").Value)
            End If
        Next

        If diskontotal > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Diskon item penjualan dari " & CmbPelanggan.Text, "BEBAN DISKON PENJUALAN", "07.01.010", "LABA KOTOR PENJUALAN", "06.01.001", diskontotal, "Penjualan", "", "")
        End If

        ' SIMPAN JURNAL TOTAL DISKON
        If ParseDecimal(TxtDiskonRp.Text) > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Diskon total penjualan dari " & CmbPelanggan.Text, "BEBAN DISKON PENJUALAN", "07.01.010", "", "", ParseDecimal(TxtDiskonRp.Text), "Penjualan", "", "")
        End If

        ' SIMPAN JURNAL PERSEDIAAN BARANG
        SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "HPP penjualan kepada " & CmbPelanggan.Text, "", "", NAMA_REK_BARANG, KODE_REK_BARANG, persediaanbarang, "Penjualan", "", "")

        ' SIMPAN JURNAL HUTANG PAJAK
        If ParseDecimal(TxtPajakRp.Text) > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Hutang pajak penjualan dari " & CmbPelanggan.Text, "", "", "HUTANG PAJAK", "03.02.001", ParseDecimal(TxtPajakRp.Text), "Penjualan", "", "")
        End If

        ' SIMPAN JURNAL LABA
        SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Laba kotor penjualan dari " & CmbPelanggan.Text, "", "", "LABA KOTOR PENJUALAN", "06.01.001", labakotor, "Penjualan", "", "")

        ' SIMPAN JURNAL BIAYA KIRIM
        If ParseDecimal(TxtBiayaKirim.Text) > 0 Then
            SimpanJurnalUmum(transaction, TxtFaktur.Text, DTPTgl.Value, "Jasa kirim/Lain " & CmbPelanggan.Text, "", "", "PENDAPATAN LAIN LAIN", "08.01.002", ParseDecimal(TxtBiayaKirim.Text), "Penjualan", "", "")
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

        Dim queryString As String = "SELECT ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP, PAJAK_PERSEN, PAJAK_RP, BIAYA_KIRIM, KODE_AKUN, JENIS_PEMBAYARAN, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI, ID_SALES, NAMA_SALES, ID_USER, ID_KOMPUTER FROM penjualan " &
                            "WHERE ID_PENJUALAN = ?"

        Using cmd As New MySqlCommand(queryString, conn)
            cmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtFaktur.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Assign nilai dari MySqlDataReader ke variabel
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

        ' Set nilai ke kontrol
        CmbPelanggan.SelectedIndex = CmbPelanggan.FindStringExact(namaPelanggan)
        LbLKodePel.Text = kodepel
        LblJenisPl.Text = jenisPelanggan
        LblLokasiBarang.Text = lokasibarang
        DTPTgl.Value = tglTransaksi

        Editpenjualan()
        TxtDiskonPersen.Text = diskonPersen.ToString("#,0.##")
        TxtDiskonRp.Text = diskonRp.ToString("#,0.##")
        TxtPajakPersen.Text = pajakPersen.ToString("#,0.##")
        TxtPajakRp.Text = pajakRp.ToString("#,0.##")
        TxtBiayaKirim.Text = BiayaKirim.ToString("#,0.##")
        TxtKodeRef.Text = kodeRef
        CmbJenisBayar.SelectedIndex = CmbJenisBayar.FindStringExact(jenisPembayaran)
        TxtBank.Text = BANK
        TxtNoRek.Text = NO_REKENING
        TxtNamaRek.Text = NAMA_REKENING
        TxtNoReff.Text = NO_REFFERENSI
        LblSales.Text = SALES
        CmbSales.SelectedIndex = CmbSales.FindStringExact(NAMASALES)
        TxtLogin.Text = USER
        TxtKomputer.Text = KOMPUTER

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
                    stokToko = If(IsDBNull(rd("STOK_TOKO")), 0D, ParseDecimal(rd("STOK_TOKO")))
                    stokGudang = If(IsDBNull(rd("STOK_GUDANG")), 0D, ParseDecimal(rd("STOK_GUDANG")))
                End If
            End Using
        End Using

        ' Mendapatkan qty_satuan
        Using cmd As New MySqlCommand("SELECT SUM(QTY_SATUAN) AS QTY_SATUAN FROM penjualan_detail WHERE ID_BARANG = ? AND FAKTUR_JUAL = ?", conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            cmd.Parameters.AddWithValue("@FAKTUR_JUAL", faktur)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    qtySatuan = If(IsDBNull(rd("QTY_SATUAN")), 0D, ParseDecimal(rd("QTY_SATUAN")))
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
            My.Settings.TampilSN = chk.Checked
            My.Settings.Save()
        Catch ex As Exception
            ' Jangan ganggu UX bila save gagal; log jika perlu
        End Try
    End Sub
End Class