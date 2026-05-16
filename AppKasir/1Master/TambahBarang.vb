Public Class TambahBarang
    Private dragging As Boolean
    Private offsetX As Integer
    Private offsetY As Integer

    Private Barcodecekc As String
    Private Barcodecesd As String
    Private Barcodecebs As String

    Private Sub Tambahbarang_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseDown
        If e.Button = MouseButtons.Left Then
            dragging = True
            offsetX = e.X
            offsetY = e.Y
        End If
    End Sub

    Private Sub Tambahbarang_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseMove
        If dragging Then
            Me.Location = New Point(Me.Location.X + e.X - offsetX, Me.Location.Y + e.Y - offsetY)
        End If
    End Sub

    Private Sub Tambahbarang_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseUp
        dragging = False
    End Sub

    ' Event untuk mengaktifkan mode drag saat mouse ditekan
    Private Sub Lblutama_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LblUtama.MouseDown
        If e.Button = MouseButtons.Left Then
            dragging = True
            offsetX = e.X
            offsetY = e.Y
        End If
    End Sub

    ' Event untuk memindahkan form saat mouse digerakkan
    Private Sub Lblutama_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LblUtama.MouseMove
        If dragging Then
            Me.Location = New Point(Me.Location.X + e.X - offsetX, Me.Location.Y + e.Y - offsetY)
        End If
    End Sub

    ' Event untuk menghentikan mode drag saat mouse dilepas
    Private Sub Lblutama_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LblUtama.MouseUp
        dragging = False
    End Sub


    Private Sub TambahBarang_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        CBSatuanSama.Checked = AppConfig.Instance.GetValue(Of Boolean)("CbSatuansama", False)
        LblJudulStok.Text = "INFORMASI STOK " & FormUtama.SLokasi.Text
        Label22.Text = "Lokasi Rak " & FormUtama.SLokasi.Text

        If LblUtama.Text = "T A M B A H   B A R A N G" Then
            Me.Size = New Size(1143, 619)
            Label44.Visible = False
            TxtHargaBeliTerakhir.Visible = False

            TxtKategori.Clear()
            TxtKodeSupliyer.Clear()

            CmbKategori.SelectedIndex = -1
            CmbSupliyer.SelectedIndex = -1
            CmbSatUmumKecil.SelectedIndex = -1
            CmbSatUmumSedang.SelectedIndex = -1
            CmbSatUmumBesar.SelectedIndex = -1
            CmbSatPartaiKecil.SelectedIndex = -1
            CmbSatPartaiSedang.SelectedIndex = -1
            CmbSatPartaiBesar.SelectedIndex = -1
            CBManual.Visible = True
            PanelInfoRubahHarga.Visible = False
            Call Tampilkategori()
            Call TampilSatuan()
            Call Tampilsupliyer()

            Kondisiawal()
            ResetIsi()
            ' Fokus ke txtNama
            TxtNama.Focus()

        ElseIf LblUtama.Text = "E D I T   B A R A N G" Then
            Me.Size = New Size(1143, 619)
            Label44.Visible = True
            TxtHargaBeliTerakhir.Visible = True
            LblHargaBeliTerakhir.Visible = True
            TxtKode.Enabled = False
            CBManual.Visible = False
            TxtStokAkhir.Enabled = False
            CmbStokAkhir.Enabled = False
            BtnBaru.Visible = False
            PanelInfoRubahHarga.Visible = False
            Hitunghargasebelumedit()
            ' Fokus ke txtNama
            TxtNama.Focus()


        ElseIf LblUtama.Text = "EDIT HARGA JUAL DARI PEMBELIAN" Then
            Label44.Visible = True
            TxtHargaBeliTerakhir.Visible = True

            Ubahhargabeli()
            Dim hargaBeli As Decimal

            If Decimal.TryParse(TxtHrgBeli.Text, hargaBeli) Then
                LblHargaBeli.Text = "Rp. " & hargaBeli.ToString("N2")
                SetHargaBeliUmum(hargaBeli)
                SetHargaBeliPartai(hargaBeli)
            End If

            Hitunghargasebelumedit()
            TxtHArgaJUalUmumKecil.Focus()
        ElseIf LblUtama.Text = "EDIT HARGA JUAL DARI PENJUALAN" Then
            UbahhargaJual()
        End If

        Me.Cursor = Cursors.Default
    End Sub


    Public Sub Kondisiawal()
        TxtKode.Enabled = False
        'GBJualUmum.Enabled = False
        'GBJualPartai.Enabled = False
        'GBStok.Enabled = False
        'GBBarcode.Enabled = False
        'TxtStokAkhir.Enabled = False
        'CmbStokAkhir.Enabled = False

        TxtNama.Clear()
        TxtKode.Clear()
        TxtHrgBeli.Clear()
        TxtHargaBeliTerakhir.Clear()

        TxtBarcodeUmumKecil.Clear()
        TxtBarcodeUmumSedang.Clear()
        TxtBarcodeUmumBesar.Clear()

        ResetHargaBeliUmum()
        ResetHargaBeliPartai()

        ResetHargaJualUmum()
        ResetHargaJualPartai()

        ResetLabaPersenUmum()
        ResetLabaPersenPartai()

        ResetStok()

        ResetCmbSatuan()

        TxtLokasiRak.Clear()
        TxtPointMember.Clear()
        TxtPointKaryawan.Clear()
        TxtKomisiSalesRp.Clear()
        TxtKomisiSalesPersen.Clear()
    End Sub

    Public Sub SetHargaBeliUmum(ByVal hargaBeli As Decimal)
        Dim isiUmumKecil As Decimal = If(Decimal.TryParse(TxtIsiUmumKecil.Text, Nothing), Convert.ToDecimal(TxtIsiUmumKecil.Text), 0)
        Dim isiUmumSedang As Decimal = If(Decimal.TryParse(TxtIsiUmumSedang.Text, Nothing), Convert.ToDecimal(TxtIsiUmumSedang.Text), 0)
        Dim isiUmumBesar As Decimal = If(Decimal.TryParse(TxtIsiUmumBesar.Text, Nothing), Convert.ToDecimal(TxtIsiUmumBesar.Text), 0)

        ' Menghitung harga beli dengan konversi string langsung di dalam operasi
        TxtHargaBeliUmumKecil.Text = (hargaBeli * isiUmumKecil).ToString()
        TxtHargaBeliUmumSedang.Text = (hargaBeli * isiUmumSedang).ToString()
        TxtHargaBeliUmumBesar.Text = (hargaBeli * isiUmumBesar).ToString()
    End Sub


    Public Sub SetHargaBeliPartai(ByVal hargaBeli As Decimal)
        Dim isiPartaiKecil As Decimal = If(Decimal.TryParse(TxtIsiPartaiKecil.Text, Nothing), Convert.ToDecimal(TxtIsiPartaiKecil.Text), 0)
        Dim isiPartaiSedang As Decimal = If(Decimal.TryParse(TxtIsiPartaiSedang.Text, Nothing), Convert.ToDecimal(TxtIsiPartaiSedang.Text), 0)
        Dim isiPartaiBesar As Decimal = If(Decimal.TryParse(TxtIsiPartaiBesar.Text, Nothing), Convert.ToDecimal(TxtIsiPartaiBesar.Text), 0)

        ' Menghitung harga beli dan menampilkan hasilnya
        TxtHargaBeliPartaiKecil.Text = (hargaBeli * isiPartaiKecil).ToString()
        TxtHargaBeliPartaiSedang.Text = (hargaBeli * isiPartaiSedang).ToString()
        TxtHargaBeliPartaiBesar.Text = (hargaBeli * isiPartaiBesar).ToString()
    End Sub


    Public Sub ResetHargaBeliUmum()
        TxtHargaBeliUmumKecil.Text = "0"
        TxtHargaBeliUmumSedang.Text = "0"
        TxtHargaBeliUmumBesar.Text = "0"
    End Sub

    Public Sub ResetHargaBeliPartai()
        TxtHargaBeliPartaiKecil.Text = "0"
        TxtHargaBeliPartaiSedang.Text = "0"
        TxtHargaBeliPartaiBesar.Text = "0"
    End Sub

    Public Sub ResetHargaJualUmum()
        TxtHArgaJUalUmumKecil.Text = "0"
        TxtHArgaJUalUmumSedang.Text = "0"
        TxtHArgaJUalUmumBesar.Text = "0"
    End Sub

    Public Sub ResetHargaJualPartai()
        TxtHArgaJualPartaikecil.Text = "0"
        TxtHArgaJualPartaiSedang.Text = "0"
        TxtHArgaJualPartaiBesar.Text = "0"
    End Sub

    Public Sub ResetLabaPersenUmum()
        TxtLabaPersenUmumKecil.Text = "0"
        TxtLabaPersenUmumSedang.Text = "0"
        TxtLabaPersenUmumBesar.Text = "0"
    End Sub

    Public Sub ResetLabaPersenPartai()
        TxtLabaPersenPartaiKecil.Text = "0"
        TxtLabaPersenPartaiSedang.Text = "0"
        TxtLabaPersenPartaiBesar.Text = "0"
    End Sub

    Public Sub ResetStok()
        TxtStokAwal.Text = "0"
        TxtIsiStokAwal.Text = "0"
        TxtJmlhToko.Text = "0"
        TxtJmlhGudang.Text = "0"
        TxtStokAkhir.Text = "0"
        TxtIsiStokAkhir.Text = "0"
    End Sub

    Public Sub ResetCmbSatuan()
        CmbStokAkhir.SelectedIndex = -1
    End Sub

    Public Sub ResetIsi()
        TxtIsiUmumKecil.Text = "0"
        TxtIsiUmumSedang.Text = "0"
        TxtIsiUmumBesar.Text = "0"
        TxtIsiPartaiKecil.Text = "0"
        TxtIsiPartaiSedang.Text = "0"
        TxtIsiPartaiBesar.Text = "0"
    End Sub



    Private Sub CBSatuanSama_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBSatuanSama.CheckedChanged
        AppConfig.Instance.SetValue("CbSatuansama", CBSatuanSama.Checked)
        AppConfig.Instance.Save()
    End Sub

    Public Sub Ubahhargabeli()
        ' Deklarasikan variabel di luar Using
        Dim namaBarang As String = String.Empty
        Dim kodeKategori As String = String.Empty
        Dim namaKategori As String = String.Empty
        Dim kodeSupliyer As String = String.Empty
        Dim namaSupliyer As String = String.Empty
        Dim hargaBeli As Decimal = 0D
        Dim hargabeliterakhir As Decimal = 0D
        Dim satuanUmumKecil As String = String.Empty
        Dim satuanUmumSedang As String = String.Empty
        Dim satuanUmumBesar As String = String.Empty
        Dim isiUmumKecil As Integer = 0
        Dim isiUmumSedang As Integer = 0
        Dim isiUmumBesar As Integer = 0
        Dim hargaJualUmumKecil As Decimal = 0D
        Dim hargaJualUmumSedang As Decimal = 0D
        Dim hargaJualUmumBesar As Decimal = 0D
        Dim satuanPartaiKecil As String = String.Empty
        Dim satuanPartaiSedang As String = String.Empty
        Dim satuanPartaiBesar As String = String.Empty
        Dim isiPartaiKecil As Integer = 0
        Dim isiPartaiSedang As Integer = 0
        Dim isiPartaiBesar As Integer = 0
        Dim hargaJualPartaiKecil As Decimal = 0D
        Dim hargaJualPartaiSedang As Decimal = 0D
        Dim hargaJualPartaiBesar As Decimal = 0D
        Dim stokTokoAwal As Decimal = 0D
        Dim stokGudangAwal As Decimal = 0D

        ' SQL Query
        Dim sql As String = "SELECT NAMA_BARANG, KODE_KATEGORI, NAMA_KATEGORI, KODE_SUPLIYER, " &
                            "NAMA_SUPLIYER, HARGA_BELI, HARGA_BELI_TERAKHIR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                            "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, HARGA_JUAL_UMUM_KECIL, " &
                            "HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, " &
                            "SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, " &
                            "ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, " &
                            "HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?"

        ' Membaca data menggunakan MySqlCommand
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("ID_BARANG", TxtKode.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Assign values to variables
                    namaBarang = rd("NAMA_BARANG").ToString()
                    kodeKategori = rd("KODE_KATEGORI").ToString()
                    namaKategori = rd("NAMA_KATEGORI").ToString()
                    kodeSupliyer = rd("KODE_SUPLIYER").ToString()
                    namaSupliyer = rd("NAMA_SUPLIYER").ToString()
                    hargaBeli = If(rd("HARGA_BELI") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_BELI").ToString()), 0D)
                    hargabeliterakhir = If(rd("HARGA_BELI_TERAKHIR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_BELI_TERAKHIR").ToString()), 0D)
                    satuanUmumKecil = rd("SATUAN_UMUM_KECIL").ToString()
                    satuanUmumSedang = rd("SATUAN_UMUM_SEDANG").ToString()
                    satuanUmumBesar = rd("SATUAN_UMUM_BESAR").ToString()
                    isiUmumKecil = If(Integer.TryParse(rd("ISI_UMUM_KECIL").ToString(), isiUmumKecil), isiUmumKecil, 0)
                    isiUmumSedang = If(Integer.TryParse(rd("ISI_UMUM_SEDANG").ToString(), isiUmumSedang), isiUmumSedang, 0)
                    isiUmumBesar = If(Integer.TryParse(rd("ISI_UMUM_BESAR").ToString(), isiUmumBesar), isiUmumBesar, 0)
                    hargaJualUmumKecil = If(rd("HARGA_JUAL_UMUM_KECIL") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_KECIL").ToString()), 0D)
                    hargaJualUmumSedang = If(rd("HARGA_JUAL_UMUM_SEDANG") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_SEDANG").ToString()), 0D)
                    hargaJualUmumBesar = If(rd("HARGA_JUAL_UMUM_BESAR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_BESAR").ToString()), 0D)
                    satuanPartaiKecil = rd("SATUAN_PARTAI_KECIL").ToString()
                    satuanPartaiSedang = rd("SATUAN_PARTAI_SEDANG").ToString()
                    satuanPartaiBesar = rd("SATUAN_PARTAI_BESAR").ToString()
                    isiPartaiKecil = If(Integer.TryParse(rd("ISI_PARTAI_KECIL").ToString(), isiPartaiKecil), isiPartaiKecil, 0)
                    isiPartaiSedang = If(Integer.TryParse(rd("ISI_PARTAI_SEDANG").ToString(), isiPartaiSedang), isiPartaiSedang, 0)
                    isiPartaiBesar = If(Integer.TryParse(rd("ISI_PARTAI_BESAR").ToString(), isiPartaiBesar), isiPartaiBesar, 0)
                    hargaJualPartaiKecil = If(rd("HARGA_JUAL_PARTAI_KECIL") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_KECIL").ToString()), 0D)
                    hargaJualPartaiSedang = If(rd("HARGA_JUAL_PARTAI_SEDANG") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_SEDANG").ToString()), 0D)
                    hargaJualPartaiBesar = If(rd("HARGA_JUAL_PARTAI_BESAR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_BESAR").ToString()), 0D)
                    stokTokoAwal = If(rd("STOK_TOKO") IsNot DBNull.Value, Decimal.Parse(rd("STOK_TOKO").ToString()), 0D)
                    stokGudangAwal = If(rd("STOK_GUDANG") IsNot DBNull.Value, Decimal.Parse(rd("STOK_GUDANG").ToString()), 0D)
                End If
            End Using
        End Using

        ' Setelah using selesai, Anda bisa memasukkan nilai ke textbox
        TxtNama.Text = namaBarang
        CmbKategori.Text = namaKategori
        TxtKategori.Text = kodeKategori
        CmbSupliyer.Text = namaSupliyer
        TxtKodeSupliyer.Text = kodeSupliyer
        TxtHrgBeli.Text = hargaBeli.ToString("0.##")
        TxtHargaBeliTerakhir.Text = hargabeliterakhir.ToString("0.##")
        CmbSatUmumKecil.Text = satuanUmumKecil
        CmbSatUmumSedang.Text = satuanUmumSedang
        CmbSatUmumBesar.Text = satuanUmumBesar
        TxtIsiUmumKecil.Text = isiUmumKecil.ToString()
        TxtIsiUmumSedang.Text = isiUmumSedang.ToString()
        TxtIsiUmumBesar.Text = isiUmumBesar.ToString()
        TxtHArgaJUalUmumKecil.Text = hargaJualUmumKecil.ToString("0.##")
        TxtHArgaJUalUmumSedang.Text = hargaJualUmumSedang.ToString("0.##")
        TxtHArgaJUalUmumBesar.Text = hargaJualUmumBesar.ToString("0.##")
        CmbSatPartaiKecil.Text = satuanPartaiKecil
        CmbSatPartaiSedang.Text = satuanPartaiSedang
        CmbSatPartaiBesar.Text = satuanPartaiBesar
        TxtIsiPartaiKecil.Text = isiPartaiKecil.ToString()
        TxtIsiPartaiSedang.Text = isiPartaiSedang.ToString()
        TxtIsiPartaiBesar.Text = isiPartaiBesar.ToString()
        TxtHArgaJualPartaikecil.Text = hargaJualPartaiKecil.ToString("0.##")
        TxtHArgaJualPartaiSedang.Text = hargaJualPartaiSedang.ToString("0.##")
        TxtHArgaJualPartaiBesar.Text = hargaJualPartaiBesar.ToString("0.##")

        LblStokToko.Text = stokTokoAwal.ToString("0.##")
        LblStokGudang.Text = stokGudangAwal.ToString("0.##")

        If LblMetode.Text = "Harga Terbaru" Then
            TxtHrgBeli.Text = If(Not String.IsNullOrEmpty(LblRpBaru.Text) AndAlso Not LblRpBaru.Text = "0", Convert.ToDecimal(LblRpBaru.Text).ToString("N0"), "0")
            LblRpJadi.Text = If(Not String.IsNullOrEmpty(LblRpBaru.Text) AndAlso Not LblRpBaru.Text = "0", Convert.ToDecimal(LblRpBaru.Text).ToString("N2"), "0")

            Dim STOK_TOKO As Decimal = If(Not String.IsNullOrEmpty(LblStokToko.Text) AndAlso Not LblStokToko.Text = "0", Convert.ToDecimal(LblStokToko.Text), 0)
            Dim STOK_GUDANG As Decimal = If(Not String.IsNullOrEmpty(LblStokGudang.Text) AndAlso Not LblStokGudang.Text = "0", Convert.ToDecimal(LblStokGudang.Text), 0)

            Dim totalstoklama As Decimal = If(LblJenis.Text = "Toko", STOK_TOKO, If(LblJenis.Text = "Gudang", STOK_GUDANG, STOK_TOKO + STOK_GUDANG))
            LblQtyLama.Text = Math.Max(totalstoklama, 0).ToString("N0")

        ElseIf LblMetode.Text = "Metode Average (Rata - Rata)" Then
            ' Ambil harga lama dan stok
            Dim hargaLama As Decimal = If(Not Decimal.TryParse(LblRpLama.Text, hargaLama), If(Decimal.TryParse(LblRpBaru.Text, hargaLama), hargaLama, 0), hargaLama)
            Dim stokToko As Decimal = If(Not Decimal.TryParse(LblStokToko.Text, stokToko), 0, stokToko)
            Dim stokGudang As Decimal = If(Not Decimal.TryParse(LblStokGudang.Text, stokGudang), 0, stokGudang)

            Dim jenis As String = LblJenis.Text

            ' Hitung total stok lama dan pastikan tidak kurang dari 0
            Dim totalStokLama As Decimal = If(jenis = "Toko", stokToko, If(jenis = "Gudang", stokGudang, stokToko + stokGudang))
            If FormPembelian.TxtJenisTrans.Text = "EditPembelian" Then totalStokLama -= If(String.IsNullOrEmpty(LblQtySbl.Text), 0D, Convert.ToDecimal(LblQtySbl.Text))
            totalStokLama = Math.Max(totalStokLama, 0)
            LblQtyLama.Text = totalStokLama.ToString("N0")

            ' Hitung total harga lama dan pastikan tidak kurang dari 0
            Dim totalHargaLama As Decimal = Math.Max(hargaLama * totalStokLama, 0)

            ' Ambil harga baru dan stok baru
            Dim hargaBaru As Decimal = If(Not Decimal.TryParse(LblRpBaru.Text, hargaBaru), 0, hargaBaru)
            Dim stokBaru As Decimal = If(Not Decimal.TryParse(LblQtyBaru.Text, stokBaru), 0, stokBaru)

            ' Hitung total harga baru
            Dim totalHargaBaru As Decimal = hargaBaru * stokBaru

            ' Hitung total stok setelah pembelian baru
            Dim totalStok As Decimal = totalStokLama + stokBaru

            ' Hitung total harga setelah pembelian baru
            Dim totalHarga As Decimal = totalHargaLama + totalHargaBaru

            ' Hitung harga jadi menggunakan metode average HPP
            Dim hargaJadi As Decimal = If(totalStok > 0, Math.Round(totalHarga / totalStok, 0), hargaBaru)

            ' Tampilkan hasilnya
            TxtHrgBeli.Text = hargaJadi.ToString("0.##")
            LblRpJadi.Text = hargaJadi.ToString("N2")


        ElseIf LblMetode.Text = "Tidak Ada" Then
            Dim hargaLama As Decimal = 0D ' Inisialisasi hargaLama

            TxtHrgBeli.Text = If(Decimal.TryParse(LblRpLama.Text, hargaLama), hargaLama.ToString(), "0")
            LblRpJadi.Text = hargaLama.ToString("N2") ' Menggunakan hargaLama yang sudah terisi

            Dim STOK_TOKO As Decimal = If(Not Decimal.TryParse(LblStokToko.Text, STOK_TOKO), 0D, STOK_TOKO)
            Dim STOK_GUDANG As Decimal = If(Not Decimal.TryParse(LblStokGudang.Text, STOK_GUDANG), 0D, STOK_GUDANG)

            Dim totalstoklama As Decimal = If(LblJenis.Text = "Toko", STOK_TOKO, If(LblJenis.Text = "Gudang", STOK_GUDANG, STOK_TOKO + STOK_GUDANG))
            LblQtyLama.Text = Math.Max(totalstoklama, 0).ToString("N0")
        End If


    End Sub


    Public Sub UbahhargaJual()
        ' Deklarasikan variabel di luar Using
        Dim namaBarang As String = String.Empty
        Dim kodeKategori As String = String.Empty
        Dim namaKategori As String = String.Empty
        Dim kodeSupliyer As String = String.Empty
        Dim namaSupliyer As String = String.Empty
        Dim hargaBeli As Decimal = 0D
        Dim hargabeliterakhir As Decimal = 0D
        Dim satuanUmumKecil As String = String.Empty
        Dim satuanUmumSedang As String = String.Empty
        Dim satuanUmumBesar As String = String.Empty
        Dim isiUmumKecil As Integer = 0
        Dim isiUmumSedang As Integer = 0
        Dim isiUmumBesar As Integer = 0
        Dim hargaJualUmumKecil As Decimal = 0D
        Dim hargaJualUmumSedang As Decimal = 0D
        Dim hargaJualUmumBesar As Decimal = 0D
        Dim satuanPartaiKecil As String = String.Empty
        Dim satuanPartaiSedang As String = String.Empty
        Dim satuanPartaiBesar As String = String.Empty
        Dim isiPartaiKecil As Integer = 0
        Dim isiPartaiSedang As Integer = 0
        Dim isiPartaiBesar As Integer = 0
        Dim hargaJualPartaiKecil As Decimal = 0D
        Dim hargaJualPartaiSedang As Decimal = 0D
        Dim hargaJualPartaiBesar As Decimal = 0D
        Dim stokTokoAwal As Decimal = 0D
        Dim stokGudangAwal As Decimal = 0D

        ' SQL Query
        Dim sql As String = "SELECT NAMA_BARANG, KODE_KATEGORI, NAMA_KATEGORI, KODE_SUPLIYER, " &
                            "NAMA_SUPLIYER, HARGA_BELI, HARGA_BELI_TERAKHIR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                            "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, HARGA_JUAL_UMUM_KECIL, " &
                            "HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, " &
                            "SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, " &
                            "ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, " &
                            "HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?"

        ' Membaca data menggunakan MySqlCommand
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("ID_BARANG", TxtKode.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Assign values to variables
                    namaBarang = rd("NAMA_BARANG").ToString()
                    kodeKategori = rd("KODE_KATEGORI").ToString()
                    namaKategori = rd("NAMA_KATEGORI").ToString()
                    kodeSupliyer = rd("KODE_SUPLIYER").ToString()
                    namaSupliyer = rd("NAMA_SUPLIYER").ToString()
                    hargaBeli = If(rd("HARGA_BELI") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_BELI").ToString()), 0D)
                    hargabeliterakhir = If(rd("HARGA_BELI_TERAKHIR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_BELI_TERAKHIR").ToString()), 0D)
                    satuanUmumKecil = rd("SATUAN_UMUM_KECIL").ToString()
                    satuanUmumSedang = rd("SATUAN_UMUM_SEDANG").ToString()
                    satuanUmumBesar = rd("SATUAN_UMUM_BESAR").ToString()
                    isiUmumKecil = If(Integer.TryParse(rd("ISI_UMUM_KECIL").ToString(), isiUmumKecil), isiUmumKecil, 0)
                    isiUmumSedang = If(Integer.TryParse(rd("ISI_UMUM_SEDANG").ToString(), isiUmumSedang), isiUmumSedang, 0)
                    isiUmumBesar = If(Integer.TryParse(rd("ISI_UMUM_BESAR").ToString(), isiUmumBesar), isiUmumBesar, 0)
                    hargaJualUmumKecil = If(rd("HARGA_JUAL_UMUM_KECIL") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_KECIL").ToString()), 0D)
                    hargaJualUmumSedang = If(rd("HARGA_JUAL_UMUM_SEDANG") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_SEDANG").ToString()), 0D)
                    hargaJualUmumBesar = If(rd("HARGA_JUAL_UMUM_BESAR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_BESAR").ToString()), 0D)
                    satuanPartaiKecil = rd("SATUAN_PARTAI_KECIL").ToString()
                    satuanPartaiSedang = rd("SATUAN_PARTAI_SEDANG").ToString()
                    satuanPartaiBesar = rd("SATUAN_PARTAI_BESAR").ToString()
                    isiPartaiKecil = If(Integer.TryParse(rd("ISI_PARTAI_KECIL").ToString(), isiPartaiKecil), isiPartaiKecil, 0)
                    isiPartaiSedang = If(Integer.TryParse(rd("ISI_PARTAI_SEDANG").ToString(), isiPartaiSedang), isiPartaiSedang, 0)
                    isiPartaiBesar = If(Integer.TryParse(rd("ISI_PARTAI_BESAR").ToString(), isiPartaiBesar), isiPartaiBesar, 0)
                    hargaJualPartaiKecil = If(rd("HARGA_JUAL_PARTAI_KECIL") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_KECIL").ToString()), 0D)
                    hargaJualPartaiSedang = If(rd("HARGA_JUAL_PARTAI_SEDANG") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_SEDANG").ToString()), 0D)
                    hargaJualPartaiBesar = If(rd("HARGA_JUAL_PARTAI_BESAR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_BESAR").ToString()), 0D)
                    stokTokoAwal = If(rd("STOK_TOKO") IsNot DBNull.Value, Decimal.Parse(rd("STOK_TOKO").ToString()), 0D)
                    stokGudangAwal = If(rd("STOK_GUDANG") IsNot DBNull.Value, Decimal.Parse(rd("STOK_GUDANG").ToString()), 0D)
                End If
            End Using
        End Using

        ' Setelah using selesai, Anda bisa memasukkan nilai ke textbox
        TxtNama.Text = namaBarang
        CmbKategori.Text = namaKategori
        TxtKategori.Text = kodeKategori
        CmbSupliyer.Text = namaSupliyer
        TxtKodeSupliyer.Text = kodeSupliyer
        TxtHrgBeli.Text = hargaBeli.ToString("0.##")
        TxtHargaBeliTerakhir.Text = hargabeliterakhir.ToString("0.##")
        CmbSatUmumKecil.Text = satuanUmumKecil
        CmbSatUmumSedang.Text = satuanUmumSedang
        CmbSatUmumBesar.Text = satuanUmumBesar
        TxtIsiUmumKecil.Text = isiUmumKecil.ToString()
        TxtIsiUmumSedang.Text = isiUmumSedang.ToString()
        TxtIsiUmumBesar.Text = isiUmumBesar.ToString()
        TxtHArgaJUalUmumKecil.Text = hargaJualUmumKecil.ToString("0.##")
        TxtHArgaJUalUmumSedang.Text = hargaJualUmumSedang.ToString("0.##")
        TxtHArgaJUalUmumBesar.Text = hargaJualUmumBesar.ToString("0.##")
        CmbSatPartaiKecil.Text = satuanPartaiKecil
        CmbSatPartaiSedang.Text = satuanPartaiSedang
        CmbSatPartaiBesar.Text = satuanPartaiBesar
        TxtIsiPartaiKecil.Text = isiPartaiKecil.ToString()
        TxtIsiPartaiSedang.Text = isiPartaiSedang.ToString()
        TxtIsiPartaiBesar.Text = isiPartaiBesar.ToString()
        TxtHArgaJualPartaikecil.Text = hargaJualPartaiKecil.ToString("0.##")
        TxtHArgaJualPartaiSedang.Text = hargaJualPartaiSedang.ToString("0.##")
        TxtHArgaJualPartaiBesar.Text = hargaJualPartaiBesar.ToString("0.##")

        LblStokToko.Text = stokTokoAwal.ToString("0.##")
        LblStokGudang.Text = stokGudangAwal.ToString("0.##")

        Dim jenis As String = LblJenisDrJual.Text
        Dim satuan As String = LblsatuanDrJual.Text
        Dim harga As String = LblHargaDrJual.Text

        If jenis = "Umum" Then
            If satuan = CmbSatUmumKecil.Text Then
                TxtHArgaJUalUmumKecil.Text = harga
            ElseIf satuan = CmbSatUmumSedang.Text Then
                TxtHArgaJUalUmumSedang.Text = harga
            ElseIf satuan = CmbSatUmumBesar.Text Then
                TxtHArgaJUalUmumBesar.Text = harga
            End If
        ElseIf jenis = "Partai" Then
            If satuan = CmbSatPartaiKecil.Text Then
                TxtHArgaJualPartaikecil.Text = harga
            ElseIf satuan = CmbSatPartaiSedang.Text Then
                TxtHArgaJualPartaiSedang.Text = harga
            ElseIf satuan = CmbSatPartaiBesar.Text Then
                TxtHArgaJualPartaiBesar.Text = harga
            End If
        End If

    End Sub


    Public Sub Tampilkategori()
        Using cmd As New MySqlCommand("SELECT nama FROM tbl_kategori ORDER BY nama ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim namaKategori As String = rd("nama").ToString()
                    ' Tambahkan hanya jika belum ada di ComboBox
                    If Not CmbKategori.Items.Contains(namaKategori) Then
                        CmbKategori.Items.Add(namaKategori)
                    End If
                End While
            End Using
        End Using
    End Sub


    Public Sub TampilSatuan()
        ' Ambil data satuan dari database
        Using cmd As New MySqlCommand("SELECT nama FROM tbl_satuan ORDER BY nama ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Do While rd.Read()
                    Dim namaSatuan As String = rd.Item("nama").ToString()

                    ' Tambahkan item jika belum ada di ComboBox
                    If Not CmbSatUmumKecil.Items.Contains(namaSatuan) Then
                        CmbSatUmumKecil.Items.Add(namaSatuan)
                    End If
                    If Not CmbSatUmumSedang.Items.Contains(namaSatuan) Then
                        CmbSatUmumSedang.Items.Add(namaSatuan)
                    End If
                    If Not CmbSatUmumBesar.Items.Contains(namaSatuan) Then
                        CmbSatUmumBesar.Items.Add(namaSatuan)
                    End If
                    If Not CmbSatPartaiKecil.Items.Contains(namaSatuan) Then
                        CmbSatPartaiKecil.Items.Add(namaSatuan)
                    End If
                    If Not CmbSatPartaiSedang.Items.Contains(namaSatuan) Then
                        CmbSatPartaiSedang.Items.Add(namaSatuan)
                    End If
                    If Not CmbSatPartaiBesar.Items.Contains(namaSatuan) Then
                        CmbSatPartaiBesar.Items.Add(namaSatuan)
                    End If
                    If Not CmbStokAkhir.Items.Contains(namaSatuan) Then
                        CmbStokAkhir.Items.Add(namaSatuan)
                    End If
                    If Not CmBstokAwal.Items.Contains(namaSatuan) Then
                        CmBstokAwal.Items.Add(namaSatuan)
                    End If
                Loop
            End Using
        End Using

        ' Tambahkan item kosong di posisi terakhir jika belum ada
        If Not CmbSatUmumKecil.Items.Contains("") Then
            CmbSatUmumKecil.Items.Add("")
        End If
        If Not CmbSatUmumSedang.Items.Contains("") Then
            CmbSatUmumSedang.Items.Add("")
        End If
        If Not CmbSatUmumBesar.Items.Contains("") Then
            CmbSatUmumBesar.Items.Add("")
        End If
        If Not CmbSatPartaiKecil.Items.Contains("") Then
            CmbSatPartaiKecil.Items.Add("")
        End If
        If Not CmbSatPartaiSedang.Items.Contains("") Then
            CmbSatPartaiSedang.Items.Add("")
        End If
        If Not CmbSatPartaiBesar.Items.Contains("") Then
            CmbSatPartaiBesar.Items.Add("")
        End If
        If Not CmbStokAkhir.Items.Contains("") Then
            CmbStokAkhir.Items.Add("")
        End If
        If Not CmBstokAwal.Items.Contains("") Then
            CmBstokAwal.Items.Add("")
        End If
    End Sub


    Public Sub Tampilsupliyer()
        Using cmd As New MySqlCommand("SELECT Nama FROM tbl_supliyer ORDER BY Nama ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Do While rd.Read()
                    Dim namaSupliyer As String = rd.Item("Nama").ToString()

                    ' Tambahkan hanya jika item belum ada di ComboBox
                    If Not CmbSupliyer.Items.Contains(namaSupliyer) Then
                        CmbSupliyer.Items.Add(namaSupliyer)
                    End If
                Loop
            End Using
        End Using
    End Sub


    Public Sub GenerateItemCodeAutomatically()
        Dim maxKode As String = ""
        Dim existingKode As New List(Of String)
        Dim kodeKategori As String = TxtKategori.Text & "-"

        ' Ambil kode yang sudah ada dari database
        Using cmd As New MySqlCommand("SELECT ID_BARANG FROM tbl_barang WHERE ID_BARANG LIKE @CekNomor ORDER BY ID_BARANG", conn)
            cmd.Parameters.AddWithValue("@CekNomor", kodeKategori & "%")

            Using rdGenerat As MySqlDataReader = cmd.ExecuteReader()
                While rdGenerat.Read()
                    existingKode.Add(rdGenerat(0).ToString())
                End While
            End Using
        End Using


        ' Jika tidak ada kode yang sudah ada
        If existingKode.Count = 0 Then
            TxtKode.Text = kodeKategori & "000001"
            Exit Sub
        End If

        ' Cari kode berikutnya yang belum terpakai
        For i As Integer = 1 To existingKode.Count
            Dim expectedKode As String = kodeKategori & i.ToString("000000")
            If Not existingKode.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        ' Jika tidak ada kode yang tersedia, gunakan nomor setelah kode terakhir
        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKode(existingKode.Count - 1)
            Dim Hitung As Integer

            If Integer.TryParse(lastKode.Substring(lastKode.Length - 6), Hitung) Then
                maxKode = kodeKategori & (Hitung + 1).ToString("000000")
            End If
        End If
        TxtKode.Text = maxKode

    End Sub

    Private Sub NomorVoucherotomatis()
        Dim UrutKode As String
        Dim ceknomor As String = TxtKategori.Text & "-"

        Using cmd As New MySqlCommand("SELECT MAX(ID_BARANG) FROM tbl_barang WHERE ID_BARANG LIKE @CekNomor", conn)
            cmd.Parameters.AddWithValue("@CekNomor", ceknomor & "%")

            Dim result As Object = cmd.ExecuteScalar()

            If result IsNot Nothing AndAlso Not Convert.IsDBNull(result) Then
                Dim maxId As String = Convert.ToString(result)

                If maxId.StartsWith(ceknomor) Then
                    Dim count As Long = Convert.ToInt64(maxId.Substring(maxId.Length - 6)) + 1
                    UrutKode = ceknomor & count.ToString("000000")
                Else
                    UrutKode = ceknomor & "000001"
                End If
            Else
                UrutKode = ceknomor & "000001"
            End If
        End Using

        TxtKode.Text = UrutKode
    End Sub

    Private Sub TxtNama_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtNama.KeyPress
        ' Mendapatkan status tombol Caps Lock
        Dim capsLock As Boolean = Control.IsKeyLocked(Keys.CapsLock)

        ' Mendapatkan status tombol Shift
        Dim shiftPressed As Boolean = Control.ModifierKeys = Keys.Shift

        ' Cek apakah karakter yang dimasukkan adalah huruf, angka, spasi, backspace, delete, tab, enter,
        ' atau apakah Caps Lock atau Shift aktif
        If Not (Char.IsLetterOrDigit(e.KeyChar) OrElse e.KeyChar = " " OrElse e.KeyChar = ChrW(Keys.Back) OrElse
                e.KeyChar = ChrW(Keys.Delete) OrElse e.KeyChar = ChrW(Keys.Tab) OrElse e.KeyChar = ChrW(Keys.Enter) OrElse
                Char.IsControl(e.KeyChar) OrElse capsLock OrElse shiftPressed) Then
            e.Handled = True ' Jika bukan salah satu dari karakter di atas, tolak karakter tersebut
        End If
    End Sub


    Private Sub BtnGenUmumKecil_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnGenUmumKecil.Click
        Using cmd As New MySqlCommand("SELECT BARCODE_KECIL,BARCODE_SEDANG,BARCODE_BESAR FROM tbl_barang WHERE BARCODE_KECIL = @barcode OR BARCODE_SEDANG = @barcode OR BARCODE_BESAR = @barcode", conn)
            cmd.Parameters.AddWithValue("@barcode", TxtBarcodeUmumKecil.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    Call Barcodekecil()
                End If
            End Using
        End Using
    End Sub

    Public Sub Barcodekecil()
        Dim negara As Integer = "91"
        Randomize()
        Dim produk1 As Integer = 8888
        Dim produk2 As Integer = 1111
        Dim produk As Integer = Int(produk1 * Rnd()) + produk2
        Randomize()
        Dim AngkaMax As Integer = 899999
        Dim AngkaMin As Integer = 100000
        Dim AngkaMcak As Integer = Int(AngkaMax * Rnd()) + AngkaMin
        Dim Barcode As String = negara & produk & AngkaMcak

        Dim X As Integer = 0
        Dim Y As Integer = 0
        Dim j As Integer = 11
        Try
            For i As Integer = 1 To 12
                If i Mod 2 = 0 Then
                    X += Val(Barcode(j))
                Else
                    Y += Val(Barcode(j))
                End If
                j -= 1
            Next

            Dim Z As Integer = X + (3 * Y)
            'first way
            Barcodecekc = ((10 - (Z Mod 10)) Mod 10)

            'Return True
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Return False
        End Try
        TxtBarcodeUmumKecil.Text = Barcode & Barcodecekc
    End Sub

    Private Sub BtnGenUmumSedang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnGenUmumSedang.Click
        Call Barcodesedang()

        Dim sql As String = "SELECT BARCODE_KECIL,BARCODE_SEDANG,BARCODE_BESAR FROM tbl_barang WHERE BARCODE_KECIL = ? OR BARCODE_SEDANG = ? OR BARCODE_BESAR = ?"
        Using cmd As New MySqlCommand(sql, conn)
            Dim barcodeValue As String = TxtBarcodeUmumBesar.Text
            cmd.Parameters.AddWithValue("@barcode", barcodeValue)
            cmd.Parameters.AddWithValue("@barcode2", barcodeValue)
            cmd.Parameters.AddWithValue("@barcode3", barcodeValue)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    Call Barcodesedang()
                End If
            End Using
        End Using
    End Sub

    Public Sub Barcodesedang()
        Dim negara As Integer = "91"
        Randomize()
        Dim produk1 As Integer = 8888
        Dim produk2 As Integer = 1111
        Dim produk As Integer = Int(produk1 * Rnd()) + produk2
        Randomize()
        Dim AngkaMax As Integer = 899999
        Dim AngkaMin As Integer = 100000
        Dim AngkaMcak As Integer = Int(AngkaMax * Rnd()) + AngkaMin
        Dim Barcode As String = negara & produk & AngkaMcak
        Dim X As Integer = 0
        Dim Y As Integer = 0
        Dim j As Integer = 11
        Try
            For i As Integer = 1 To 12
                If i Mod 2 = 0 Then
                    X += Val(Barcode(j))
                Else
                    Y += Val(Barcode(j))
                End If
                j -= 1
            Next

            Dim Z As Integer = X + (3 * Y)
            'first way
            Barcodecesd = ((10 - (Z Mod 10)) Mod 10)

            'Return True
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Return False
        End Try
        TxtBarcodeUmumSedang.Text = Barcode & Barcodecesd

    End Sub

    Private Sub BtnGenUmumBesar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnGenUmumBesar.Click
        Call Barcodebesar()

        Dim sql As String = "SELECT BARCODE_KECIL,BARCODE_SEDANG,BARCODE_BESAR FROM tbl_barang WHERE BARCODE_KECIL = ? OR BARCODE_SEDANG = ? OR BARCODE_BESAR = ?"
        Using cmd As New MySqlCommand(sql, conn)
            Dim barcodeValue As String = TxtBarcodeUmumBesar.Text
            cmd.Parameters.AddWithValue("@barcode", barcodeValue)
            cmd.Parameters.AddWithValue("@barcode2", barcodeValue)
            cmd.Parameters.AddWithValue("@barcode3", barcodeValue)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    Call Barcodebesar()
                End If
            End Using
        End Using
    End Sub

    Public Sub Barcodebesar()
        Dim negara As Integer = "91"
        Randomize()
        Dim produk1 As Integer = 8888
        Dim produk2 As Integer = 1111
        Dim produk As Integer = Int(produk1 * Rnd()) + produk2
        Randomize()
        Dim AngkaMax As Integer = 899999
        Dim AngkaMin As Integer = 100000
        Dim AngkaMcak As Integer = Int(AngkaMax * Rnd()) + AngkaMin
        Dim Barcode As String = negara & produk & AngkaMcak

        Dim X As Integer = 0
        Dim Y As Integer = 0
        Dim j As Integer = 11
        Try
            For i As Integer = 1 To 12
                If i Mod 2 = 0 Then
                    X += Val(Barcode(j))
                Else
                    Y += Val(Barcode(j))
                End If
                j -= 1
            Next

            Dim Z As Integer = X + (3 * Y)
            'first way
            Barcodecebs = ((10 - (Z Mod 10)) Mod 10)

            'Return True
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Return False
        End Try
        TxtBarcodeUmumBesar.Text = Barcode & Barcodecebs
    End Sub

    Private Sub BtnTambahKategori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambahKategori.Click
        TambahKategori.ShowDialog()
        Call Tampilkategori()
    End Sub

    Private Sub BtnTambahSatuan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambahSatuan.Click
        TambahSatuan.ShowDialog()
        Call TampilSatuan()
    End Sub

    Private Sub CmbKategori_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbKategori.SelectedIndexChanged
        Dim kode As String = ""

        Using cmd As New MySqlCommand("SELECT kode FROM tbl_kategori WHERE nama = ?", conn)
            cmd.Parameters.AddWithValue("kategori", CmbKategori.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    kode = rd("kode").ToString()
                End If
            End Using
        End Using

        TxtKategori.Text = kode

        If LblUtama.Text = "T A M B A H   B A R A N G" Then
            Call GenerateItemCodeAutomatically()
        End If

    End Sub

    Private Sub CmbKategori_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbKategori.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            CmbSupliyer.Focus()
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub CBManual_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBManual.CheckedChanged
        If CBManual.Checked = True Then
            TxtKode.Enabled = True
        Else
            TxtKode.Enabled = False
        End If
    End Sub

    Private Sub BtnSupliyer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSupliyer.Click
        TambahSupliyer.ShowDialog()
        Call Tampilsupliyer()
    End Sub

    Private Sub CmbSupliyer_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbSupliyer.SelectedIndexChanged
        Using cmd As New MySqlCommand("SELECT kode FROM tbl_supliyer WHERE nama = ?", conn)
            cmd.Parameters.AddWithValue("supliyer", CmbSupliyer.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TxtKodeSupliyer.Text = rd("kode").ToString()
                End If
            End Using
        End Using

    End Sub

    Private Sub CmbSupliyer_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSupliyer.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtHrgBeli.Focus()
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub TxtHrgBeli_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHrgBeli.TextChanged
        Dim hargaBeli As Decimal

        ' Mengambil nilai harga beli dan validasi
        If Decimal.TryParse(TxtHrgBeli.Text, hargaBeli) AndAlso hargaBeli > 0 Then
            ' Format dengan pemisah ribuan dan maksimal 2 angka desimal jika perlu
            LblHargaBeli.Text = "Rp. " & hargaBeli.ToString("#,0.##", cultureIndonesia)
        Else
            LblHargaBeli.Text = "Rp. 0"
            hargaBeli = 0D
        End If

        ' Menghitung harga beli untuk setiap jenis
        UpdateHargaBeli(hargaBeli)
    End Sub



    'Private Sub DisableControls()
    '    GBJualUmum.Enabled = False
    '    GBJualPartai.Enabled = False
    '    GBStok.Enabled = False
    '    GBBarcode.Enabled = False
    '    GBPoint.Enabled = False
    'End Sub

    'Private Sub EnableControls()
    '    GBJualUmum.Enabled = True
    '    GBJualPartai.Enabled = True
    '    GBStok.Enabled = True
    '    GBBarcode.Enabled = True
    '    GBPoint.Enabled = True
    'End Sub

    Private Sub UpdateHargaBeli(ByVal hargaBeli As Decimal)
        Dim isiUmumKecil, isiUmumSedang, isiUmumBesar, isiPartaiKecil, isiPartaiSedang, isiPartaiBesar As Decimal

        ' Mengambil nilai isi untuk masing-masing jenis
        Decimal.TryParse(TxtIsiUmumKecil.Text, isiUmumKecil)
        Decimal.TryParse(TxtIsiUmumSedang.Text, isiUmumSedang)
        Decimal.TryParse(TxtIsiUmumBesar.Text, isiUmumBesar)
        Decimal.TryParse(TxtIsiPartaiKecil.Text, isiPartaiKecil)
        Decimal.TryParse(TxtIsiPartaiSedang.Text, isiPartaiSedang)
        Decimal.TryParse(TxtIsiPartaiBesar.Text, isiPartaiBesar)

        ' Menghitung dan menetapkan nilai ke TextBox masing-masing jenis
        TxtHargaBeliUmumKecil.Text = (hargaBeli * isiUmumKecil).ToString()
        TxtHargaBeliUmumSedang.Text = (hargaBeli * isiUmumSedang).ToString()
        TxtHargaBeliUmumBesar.Text = (hargaBeli * isiUmumBesar).ToString()
        TxtHargaBeliPartaiKecil.Text = (hargaBeli * isiPartaiKecil).ToString()
        TxtHargaBeliPartaiSedang.Text = (hargaBeli * isiPartaiSedang).ToString()
        TxtHargaBeliPartaiBesar.Text = (hargaBeli * isiPartaiBesar).ToString()
    End Sub


    Private Sub TxtHargaBeliTerakhir_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtHargaBeliTerakhir.TextChanged
        Dim hargaBeliterakhir As Decimal

        ' Mengambil nilai harga beli
        If Not Decimal.TryParse(TxtHargaBeliTerakhir.Text, hargaBeliterakhir) OrElse hargaBeliterakhir <= 0 Then
            hargaBeliterakhir = 0D
        End If

        ' Format dengan maksimal 2 angka di belakang koma (tanpa 0 tambahan)
        LblHargaBeliTerakhir.Text = "Rp. " & hargaBeliterakhir.ToString("#,0.##", cultureIndonesia)
    End Sub


    Private Sub CmbSatUmumKecil_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbSatUmumKecil.SelectedIndexChanged
        If Not String.IsNullOrEmpty(CmbSatUmumKecil.Text) Then
            Using cmd As New MySqlCommand("SELECT isi FROM tbl_satuan WHERE nama = ?", conn)
                cmd.Parameters.AddWithValue("satuan", CmbSatUmumKecil.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        TxtIsiUmumKecil.Text = rd("isi").ToString()
                    End If
                End Using
            End Using

            CmBstokAwal.Text = CmbSatUmumKecil.Text
            CmbStokAkhir.Text = CmbSatUmumKecil.Text

            If CBSatuanSama.Checked Then
                If LblUtama.Text = "T A M B A H   B A R A N G" Then
                    CmbSatPartaiKecil.Text = CmbSatUmumKecil.Text
                Else
                    If CmbSatPartaiKecil.SelectedIndex = 0 Then
                        CmbSatPartaiKecil.Text = CmbSatUmumKecil.Text
                    End If
                End If
            End If

        ElseIf Not CmbSatUmumKecil.DroppedDown Then
            TxtIsiUmumKecil.Text = "0"
        End If
    End Sub

    Private Sub CmbSatUmumKecil_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSatUmumKecil.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtHArgaJUalUmumKecil.Focus()
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub CmbSatUmumSedang_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbSatUmumSedang.SelectedIndexChanged
        If Not String.IsNullOrEmpty(CmbSatUmumSedang.Text) Then
            Using cmd As New MySqlCommand("SELECT isi FROM tbl_satuan WHERE nama = ?", conn)
                cmd.Parameters.AddWithValue("satuan", CmbSatUmumSedang.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        TxtIsiUmumSedang.Text = rd("isi").ToString()
                    End If
                End Using
            End Using

            If CBSatuanSama.Checked Then
                If LblUtama.Text = "T A M B A H   B A R A N G" Then
                    CmbSatPartaiSedang.Text = CmbSatUmumSedang.Text
                Else
                    If CmbSatPartaiSedang.SelectedIndex = 0 Then
                        CmbSatPartaiSedang.Text = CmbSatUmumSedang.Text
                    End If
                End If
            End If

        Else
            TxtIsiUmumSedang.Text = "0"
        End If
    End Sub

    Private Sub CmbSatUmumSedang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSatUmumSedang.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtHArgaJUalUmumSedang.Focus()
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub CmbSatUmumBesar_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbSatUmumBesar.SelectedIndexChanged
        If Not String.IsNullOrEmpty(CmbSatUmumBesar.Text) Then
            Using cmd As New MySqlCommand("SELECT isi FROM tbl_satuan WHERE nama = ?", conn)
                cmd.Parameters.AddWithValue("satuan", CmbSatUmumBesar.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        TxtIsiUmumBesar.Text = rd("isi").ToString()
                    End If
                End Using
            End Using

            If CBSatuanSama.Checked Then
                If LblUtama.Text = "T A M B A H   B A R A N G" Then
                    CmbSatPartaiBesar.Text = CmbSatUmumBesar.Text
                Else
                    If CmbSatPartaiBesar.SelectedIndex = 0 Then
                        CmbSatPartaiBesar.Text = CmbSatUmumBesar.Text
                    End If
                End If
            End If


        Else
            TxtIsiUmumBesar.Text = "0"
        End If
    End Sub

    Private Sub CmbSatUmumBesar_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSatUmumBesar.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtHArgaJUalUmumBesar.Focus()
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub
    Private Sub TxtKategori_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtKategori.TextChanged
        If LblUtama.Text = "T A M B A H   B A R A N G" Then
            Call GenerateItemCodeAutomatically()
        End If

    End Sub
    Private Sub TxtIsiUmumKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiUmumKecil.TextChanged
        Dim hargaBeli As Decimal
        Dim isiUmum As Integer

        If Decimal.TryParse(TxtHrgBeli.Text, hargaBeli) AndAlso Integer.TryParse(TxtIsiUmumKecil.Text, isiUmum) Then
            TxtHargaBeliUmumKecil.Text = (hargaBeli * isiUmum).ToString()
        Else
            TxtHargaBeliUmumKecil.Text = "0"
        End If
    End Sub

    Private Sub TxtHArgaJUalUmumKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHArgaJUalUmumKecil.TextChanged, TxtHargaBeliUmumKecil.TextChanged
        Dim hargaBeli As Decimal
        Dim hargaJual As Decimal

        If Decimal.TryParse(TxtHArgaJUalUmumKecil.Text, hargaJual) AndAlso Decimal.TryParse(TxtHargaBeliUmumKecil.Text, hargaBeli) Then
            TxtLabaRpUmumKecil.Text = FormatNumber((hargaJual - hargaBeli).ToString(), 0)
            If hargaJual <> 0 AndAlso hargaBeli <> 0 Then
                TxtLabaPersenUmumKecil.Text = Math.Round(((hargaJual - hargaBeli) / hargaBeli) * 100, 2).ToString()
            End If
            LbljualUmumKecil.Text = "Rp. " + FormatNumber(hargaJual.ToString(), 0)
        Else
            TxtLabaRpUmumKecil.Text = "0"
            TxtLabaPersenUmumKecil.Text = "0"
            LbljualUmumKecil.Text = "Rp. 0"
        End If
    End Sub


    Private Sub TxtIsiUmumSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiUmumSedang.TextChanged
        Dim hargaBeli As Decimal
        Dim isiUmum As Integer

        If Decimal.TryParse(TxtHrgBeli.Text, hargaBeli) AndAlso Integer.TryParse(TxtIsiUmumSedang.Text, isiUmum) Then
            TxtHargaBeliUmumSedang.Text = (hargaBeli * isiUmum).ToString()
        Else
            TxtHargaBeliUmumSedang.Text = "0"
        End If
    End Sub


    Private Sub TxtHArgaJUalUmumSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHArgaJUalUmumSedang.TextChanged, TxtHargaBeliUmumSedang.TextChanged
        Dim hargaBeli As Decimal
        Dim hargaJual As Decimal

        If Decimal.TryParse(TxtHArgaJUalUmumSedang.Text, hargaJual) AndAlso Decimal.TryParse(TxtHargaBeliUmumSedang.Text, hargaBeli) Then
            TxtLabaRpUmumSedang.Text = FormatNumber((hargaJual - hargaBeli).ToString(), 0)
            If hargaJual <> 0 AndAlso hargaBeli <> 0 Then
                TxtLabaPersenUmumSedang.Text = Math.Round(((hargaJual - hargaBeli) / hargaBeli) * 100, 2).ToString()
            End If
            LbljualUmumSedang.Text = "Rp. " + FormatNumber(hargaJual.ToString(), 0)
        Else
            TxtLabaRpUmumSedang.Text = "0"
            TxtLabaPersenUmumSedang.Text = "0"
            LbljualUmumSedang.Text = "Rp. 0"
        End If
    End Sub


    Private Sub TxtIsiUmumBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiUmumBesar.TextChanged
        Dim hargaBeli As Decimal
        Dim isiUmum As Integer

        If Decimal.TryParse(TxtHrgBeli.Text, hargaBeli) AndAlso Integer.TryParse(TxtIsiUmumBesar.Text, isiUmum) Then
            TxtHargaBeliUmumBesar.Text = (hargaBeli * isiUmum).ToString()
        Else
            TxtHargaBeliUmumBesar.Text = "0"
        End If
    End Sub


    Private Sub TxtHArgaJUalUmumBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHArgaJUalUmumBesar.TextChanged, TxtHargaBeliUmumBesar.TextChanged
        Dim hargaBeli As Decimal
        Dim hargaJual As Decimal

        If Decimal.TryParse(TxtHArgaJUalUmumBesar.Text, hargaJual) AndAlso Decimal.TryParse(TxtHargaBeliUmumBesar.Text, hargaBeli) Then
            TxtLabaRpUmumBesar.Text = FormatNumber((hargaJual - hargaBeli).ToString(), 0)
            If hargaJual <> 0 AndAlso hargaBeli <> 0 Then
                TxtLabaPersenUmumBesar.Text = Math.Round(((hargaJual - hargaBeli) / hargaBeli) * 100, 2).ToString()
            End If
            LbljualUmumBesar.Text = "Rp. " + FormatNumber(hargaJual.ToString(), 0)
        Else
            TxtLabaRpUmumBesar.Text = "0"
            TxtLabaPersenUmumBesar.Text = "0"
            LbljualUmumBesar.Text = "Rp. 0"
        End If
    End Sub


    Private Sub TxtHargaBeliUmumKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliUmumKecil.TextChanged
        LblBeliUmumKecil.Text = FormatNumber(TxtHargaBeliUmumKecil.Text, 0)
    End Sub

    Private Sub TxtHargaBeliUmumSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliUmumSedang.TextChanged
        LblBeliUmumSedang.Text = FormatNumber(TxtHargaBeliUmumSedang.Text, 0)
    End Sub

    Private Sub TxtHargaBeliUmumBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliUmumBesar.TextChanged
        LblBeliUmumBesar.Text = FormatNumber(TxtHargaBeliUmumBesar.Text, 0)
    End Sub

    Private Sub CmbSatPartaiKecil_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbSatPartaiKecil.SelectedIndexChanged
        If Not String.IsNullOrEmpty(CmbSatPartaiKecil.Text) Then
            Using cmd As New MySqlCommand("SELECT isi FROM tbl_satuan WHERE nama = ?", conn)
                cmd.Parameters.AddWithValue("satuan", CmbSatPartaiKecil.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        TxtIsiPartaiKecil.Text = rd("isi").ToString()
                    End If
                End Using
            End Using


        Else
            TxtIsiPartaiKecil.Text = "0"
        End If
    End Sub

    Private Sub CmbSatPartaiKecil_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSatPartaiKecil.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtHArgaJualPartaikecil.Focus()
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub CmbSatPartaiSedang_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbSatPartaiSedang.SelectedIndexChanged
        If Not String.IsNullOrEmpty(CmbSatPartaiSedang.Text) Then
            Using cmd As New MySqlCommand("SELECT isi FROM tbl_satuan WHERE nama = ?", conn)
                cmd.Parameters.AddWithValue("satuan", CmbSatPartaiSedang.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        TxtIsiPartaiSedang.Text = rd("isi").ToString()
                    End If
                End Using
            End Using


        Else
            TxtIsiPartaiSedang.Text = "0"
        End If
    End Sub

    Private Sub CmbSatPartaiSedang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSatPartaiSedang.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtHArgaJualPartaiSedang.Focus()
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub CmbSatPartaiBesar_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbSatPartaiBesar.SelectedIndexChanged
        If Not String.IsNullOrEmpty(CmbSatPartaiBesar.Text) Then
            Using cmd As New MySqlCommand("SELECT isi FROM tbl_satuan WHERE nama = ?", conn)
                cmd.Parameters.AddWithValue("satuan", CmbSatPartaiBesar.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        TxtIsiPartaiBesar.Text = rd("isi").ToString()
                    End If
                End Using
            End Using


        Else
            TxtIsiPartaiBesar.Text = "0"
        End If
    End Sub

    Private Sub CmbSatPartaiBesar_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSatPartaiBesar.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtHArgaJualPartaiBesar.Focus()
            e.SuppressKeyPress = True ' Menghindari bunyi beep pada Enter
        End If
    End Sub

    Private Sub TxtIsiPartaiKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiPartaiKecil.TextChanged
        UpdateHargaBeliPartaiKecil()
    End Sub

    Private Sub TxtHArgaJUalPartaiKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHArgaJualPartaikecil.TextChanged, TxtHargaBeliPartaiKecil.TextChanged
        UpdateLabaPartaiKecil()
    End Sub

    Private Sub TxtIsiPartaiSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiPartaiSedang.TextChanged
        UpdateHargaBeliPartaiSedang()
    End Sub

    Private Sub TxtHArgaJUalPartaiSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHArgaJualPartaiSedang.TextChanged, TxtHargaBeliPartaiSedang.TextChanged
        UpdateLabaPartaiSedang()
    End Sub

    Private Sub TxtIsiPartaiBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiPartaiBesar.TextChanged
        UpdateHargaBeliPartaiBesar()
    End Sub

    Private Sub TxtHArgaJUalPartaiBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHArgaJualPartaiBesar.TextChanged, TxtHargaBeliPartaiBesar.TextChanged
        UpdateLabaPartaiBesar()
    End Sub

    Private Sub UpdateHargaBeliPartaiKecil()
        Dim hargaBeli As Decimal = 0
        Dim isiPartaiKecil As Integer = 0

        If Decimal.TryParse(TxtHrgBeli.Text, hargaBeli) AndAlso Integer.TryParse(TxtIsiPartaiKecil.Text, isiPartaiKecil) Then
            TxtHargaBeliPartaiKecil.Text = (hargaBeli * isiPartaiKecil).ToString()
        Else
            TxtHargaBeliPartaiKecil.Text = "0"
        End If
    End Sub

    Private Sub UpdateLabaPartaiKecil()
        Dim hargaJual As Decimal = 0
        Dim hargaBeli As Decimal = 0

        If Decimal.TryParse(TxtHArgaJualPartaikecil.Text, hargaJual) AndAlso Decimal.TryParse(TxtHargaBeliPartaiKecil.Text, hargaBeli) Then
            TxtLabaRpPartaiKecil.Text = FormatNumber(hargaJual - hargaBeli, 0)

            If hargaBeli <> 0 Then
                TxtLabaPersenPartaiKecil.Text = Math.Round((hargaJual - hargaBeli) / hargaBeli * 100, 2)
            Else
                TxtLabaPersenPartaiKecil.Text = "0"
            End If
        Else
            TxtLabaRpPartaiKecil.Text = "0"
            TxtLabaPersenPartaiKecil.Text = "0"
        End If

        If Not String.IsNullOrEmpty(TxtHArgaJualPartaikecil.Text) AndAlso IsNumeric(TxtHArgaJualPartaikecil.Text) Then
            LbljualPartaiKecil.Text = "Rp. " + FormatNumber(TxtHArgaJualPartaikecil.Text, 0)
        Else
            LbljualPartaiKecil.Text = "Rp. 0"
        End If
    End Sub

    Private Sub UpdateHargaBeliPartaiSedang()
        Dim hargaBeli As Decimal = 0
        Dim isiPartaiSedang As Integer = 0

        If Decimal.TryParse(TxtHrgBeli.Text, hargaBeli) AndAlso Integer.TryParse(TxtIsiPartaiSedang.Text, isiPartaiSedang) Then
            TxtHargaBeliPartaiSedang.Text = (hargaBeli * isiPartaiSedang).ToString()
        Else
            TxtHargaBeliPartaiSedang.Text = "0"
        End If
    End Sub


    Private Sub UpdateLabaPartaiSedang()
        Dim hargaJual As Decimal = 0
        Dim hargaBeli As Decimal = 0

        If Decimal.TryParse(TxtHArgaJualPartaiSedang.Text, hargaJual) AndAlso Decimal.TryParse(TxtHargaBeliPartaiSedang.Text, hargaBeli) Then
            TxtLabaRpPartaiSedang.Text = FormatNumber(hargaJual - hargaBeli, 0)

            If hargaBeli <> 0 Then
                TxtLabaPersenPartaiSedang.Text = Math.Round((hargaJual - hargaBeli) / hargaBeli * 100, 2)
            Else
                TxtLabaPersenPartaiSedang.Text = "0"
            End If
        Else
            TxtLabaRpPartaiSedang.Text = "0"
            TxtLabaPersenPartaiSedang.Text = "0"
        End If

        If Not String.IsNullOrEmpty(TxtHArgaJualPartaiSedang.Text) AndAlso IsNumeric(TxtHArgaJualPartaiSedang.Text) Then
            LbljualPartaiSedang.Text = "Rp. " + FormatNumber(TxtHArgaJualPartaiSedang.Text, 0)
        Else
            LbljualPartaiSedang.Text = "Rp. 0"
        End If
    End Sub


    Private Sub UpdateHargaBeliPartaiBesar()
        Dim hargaBeli As Decimal = 0
        Dim isiPartaiBesar As Integer = 0

        If Decimal.TryParse(TxtHrgBeli.Text, hargaBeli) AndAlso Integer.TryParse(TxtIsiPartaiBesar.Text, isiPartaiBesar) Then
            TxtHargaBeliPartaiBesar.Text = (hargaBeli * isiPartaiBesar).ToString()
        Else
            TxtHargaBeliPartaiBesar.Text = "0"
        End If
    End Sub

    Private Sub UpdateLabaPartaiBesar()
        Dim hargaJual As Decimal = 0
        Dim hargaBeli As Decimal = 0

        If Decimal.TryParse(TxtHArgaJualPartaiBesar.Text, hargaJual) AndAlso Decimal.TryParse(TxtHargaBeliPartaiBesar.Text, hargaBeli) Then
            TxtLabaRpPartaiBesar.Text = FormatNumber(hargaJual - hargaBeli, 0)

            If hargaBeli <> 0 Then
                TxtLabaPersenPartaiBesar.Text = Math.Round((hargaJual - hargaBeli) / hargaBeli * 100, 2)
            Else
                TxtLabaPersenPartaiBesar.Text = "0"
            End If
        Else
            TxtLabaRpPartaiBesar.Text = "0"
            TxtLabaPersenPartaiBesar.Text = "0"
        End If

        If Not String.IsNullOrEmpty(TxtHArgaJualPartaiBesar.Text) AndAlso IsNumeric(TxtHArgaJualPartaiBesar.Text) Then
            LbljualPartaiBesar.Text = "Rp. " + FormatNumber(TxtHArgaJualPartaiBesar.Text, 0)
        Else
            LbljualPartaiBesar.Text = "Rp. 0"
        End If
    End Sub


    Private Sub TxtHargaBeliPartaiKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliPartaiKecil.TextChanged
        LblBeliPartaiKecil.Text = FormatNumber(TxtHargaBeliPartaiKecil.Text, 0)
    End Sub

    Private Sub TxtHargaBeliPartaiSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliPartaiSedang.TextChanged
        LblBeliPartaiSedang.Text = FormatNumber(TxtHargaBeliPartaiSedang.Text, 0)
    End Sub

    Private Sub TxtHargaBeliPartaiBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliPartaiBesar.TextChanged
        LblBeliPartaiBesar.Text = FormatNumber(TxtHargaBeliPartaiBesar.Text, 0)
    End Sub

    Private Sub CmbIsiToko_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbStokAkhir.SelectedIndexChanged, CmBstokAwal.SelectedIndexChanged
        Using cmd As New MySqlCommand("SELECT isi FROM tbl_satuan WHERE nama = ?", conn)
            cmd.Parameters.AddWithValue("satuan", CmbStokAkhir.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TxtIsiStokAkhir.Text = rd("isi").ToString()
                End If
            End Using
        End Using

    End Sub

    Private Sub TxtStokAwal_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtStokAwal.TextChanged
        Dim stokAwal, stokToko, stokGudang As Decimal

        Decimal.TryParse(TxtStokAwal.Text, stokAwal)
        Decimal.TryParse(TxtJmlhToko.Text, stokToko)
        Decimal.TryParse(TxtJmlhGudang.Text, stokGudang)

        ' Menghitung stok akhir berdasarkan lokasi
        TxtStokAkhir.Text = If(FormUtama.SLokasi.Text = "TOKO", (stokAwal + stokToko).ToString(), (stokAwal + stokGudang).ToString())
    End Sub


    Private Sub TextBoxes_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtHrgBeli.KeyPress, TxtHArgaJUalUmumKecil.KeyPress, TxtHArgaJUalUmumSedang.KeyPress, TxtHArgaJUalUmumBesar.KeyPress, TxtHArgaJualPartaikecil.KeyPress, TxtHArgaJualPartaiSedang.KeyPress, TxtHArgaJualPartaiBesar.KeyPress
        If Not (Char.IsDigit(e.KeyChar) OrElse e.KeyChar = "." OrElse e.KeyChar = ",") AndAlso e.KeyChar <> vbBack Then
            e.Handled = True
        End If
    End Sub

    Private Sub TxtIsiStokToko_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtStokAkhir.KeyPress, TxtStokAwal.KeyPress
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub
    Private Sub TxtIsiStokGudang_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub


    Private Sub IsiComboBoxStokAwal()
        Dim satuanKecil As String = CmbSatUmumKecil.Text
        Dim satuanSedang As String = CmbSatUmumSedang.Text
        Dim satuanBesar As String = CmbSatUmumBesar.Text

        If Not String.IsNullOrEmpty(satuanKecil) Or Not String.IsNullOrEmpty(satuanSedang) Or Not String.IsNullOrEmpty(satuanBesar) Then
            CmbStokAkhir.Items.Clear()
            CmBstokAwal.Items.Clear()
            If Not String.IsNullOrEmpty(satuanKecil) Then
                CmbStokAkhir.Items.Add(satuanKecil)
                CmBstokAwal.Items.Add(satuanKecil)
            End If

            If Not String.IsNullOrEmpty(satuanSedang) Then
                CmbStokAkhir.Items.Add(satuanSedang)
                CmBstokAwal.Items.Add(satuanSedang)
            End If

            If Not String.IsNullOrEmpty(satuanBesar) Then
                CmbStokAkhir.Items.Add(satuanBesar)
                CmBstokAwal.Items.Add(satuanBesar)
            End If

            CmbStokAkhir.SelectedIndex = 0
            CmBstokAwal.SelectedIndex = 0
        Else
            MessageBox.Show("Minimal salah satu satuan kecil harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private nilaiBarang As Decimal = 0

    Public Sub Hitunghargasebelumedit()
        Dim sql As String = "SELECT HARGA_BELI, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("ID_BARANG", TxtKode.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim hargaBeli As Decimal = If(Not IsDBNull(rd("HARGA_BELI")), Convert.ToDecimal(rd("HARGA_BELI")), 0D)
                    Dim stokToko As Decimal = If(Not IsDBNull(rd("STOK_TOKO")), Convert.ToDecimal(rd("STOK_TOKO")), 0D)
                    Dim stokGudang As Decimal = If(Not IsDBNull(rd("STOK_GUDANG")), Convert.ToDecimal(rd("STOK_GUDANG")), 0D)

                    nilaiBarang = (stokToko + stokGudang) * hargaBeli
                End If
            End Using
        End Using
    End Sub


    Private Function CekBarang() As Boolean
        Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG FROM tbl_barang"
        'Dim barangDitemukan As Boolean = False

        Using command As New MySqlCommand(sql, conn)

            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        Dim id_barang As String = reader("ID_BARANG").ToString().Trim()
                        Dim nama_barang As String = reader("NAMA_BARANG").ToString().Trim()

                        If id_barang = TxtKode.Text Then
                            MessageBox.Show("kode barang " & TxtKode.Text & " sudah ada. Harap ganti kode barang dengan menambah 1.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                            TxtKode.Focus()
                            CBManual.Checked = True
                            Return False
                        End If
                        If nama_barang = TxtNama.Text Then
                            MessageBox.Show("Nama barang " & TxtNama.Text & " sudah ada. Harap ganti nama barang.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                            TxtNama.Focus()
                            Return False
                        End If
                    End While
                End If
            End Using
        End Using

        Return True
    End Function


    Private Function IsInputValid() As Boolean
        ' Reset validasi awal
        Dim isValid As Boolean = True
        If TxtNama.Text = "" Then
            MessageBox.Show("Nama barang harus diisi ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtNama.Focus()
            isValid = False
        ElseIf TxtNama.Text.Contains(" => ") Then
            MessageBox.Show("Nama barang tidak boleh mengandung karakter ' => ' ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtNama.Focus()
            isValid = False
        ElseIf TxtKode.Text = "" Then
            MessageBox.Show("Kode barang harus diisi ... !!!, dengan cara memilih kategori", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            CmbKategori.DroppedDown = True
            CmbKategori.Focus()
            isValid = False

        ElseIf TxtKategori.Text = "" Then
            MessageBox.Show("Nama kategori harus dipilih ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            CmbKategori.DroppedDown = True
            CmbKategori.Focus()
            isValid = False
        ElseIf CmbSatUmumKecil.SelectedIndex < 0 OrElse CmbSatUmumKecil.SelectedItem Is Nothing Then
            MessageBox.Show("Setidaknya ada satuan yang dipilih ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            CmbSatUmumKecil.DroppedDown = True
            CmbSatUmumKecil.Focus()
            isValid = False
        End If

        Return isValid
    End Function



    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpan.Click
        ' Panggil fungsi validasi
        If Not IsInputValid() Then
            Exit Sub ' Jika validasi gagal, keluar dari metode
        End If

        Dim noTransaksi As String = DateTime.Now.ToString("yyyyMMddHHmmss") ' Format unik berdasarkan tanggal dan waktu saat ini
        ' Hitung dan tambahkan nominal sesuai dengan perhitungan Anda
        Dim NilaiBarang As Decimal = If(Decimal.TryParse(TxtStokAkhir.Text, NilaiBarang), NilaiBarang, 0)
        Dim hargaBeli As Decimal = If(Decimal.TryParse(TxtHrgBeli.Text, hargaBeli), hargaBeli, 0)
        Dim NilaiBarangAwal As Decimal = If(Decimal.TryParse(LblStokUntukEdit.Text, NilaiBarangAwal), NilaiBarangAwal, 0)
        Dim hargaBeliAwal As Decimal = If(Decimal.TryParse(LblHargaUntukEdit.Text, hargaBeliAwal), hargaBeliAwal, 0)

        Dim TotalNilaiBarang As Decimal = NilaiBarang * hargaBeli
        Dim TotalNilaiBarangAwal As Decimal = NilaiBarangAwal * hargaBeliAwal
        Dim SelisihNilaiBarang As Decimal = TotalNilaiBarang - TotalNilaiBarangAwal


        If LblUtama.Text = "T A M B A H   B A R A N G" Then
            If CekBarang() Then
                Dim transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    Dim query As String = "INSERT INTO tbl_barang (" &
 "ID_BARANG, NAMA_BARANG, KODE_KATEGORI, NAMA_KATEGORI, KODE_SUPLIYER, NAMA_SUPLIYER, HARGA_BELI, HARGA_BELI_TERAKHIR, " &
 "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
 "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
 "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
 "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, " &
 "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, " &
 "HARGA_JUAL_PARTAI_BESAR, AWAL_TOKO, TAMBAH_TOKO, KURANG_TOKO, PEMBELIAN_TOKO, PENJUALAN_TOKO, RETUR_BELI_TOKO, " &
 "RETUR_JUAL_TOKO, OPNAME_TOKO, TRANSFER_STOK_MASUK_TOKO, TRANSFER_STOK_KELUAR_TOKO, TRANSFER_BARANG_MASUK_TOKO, TRANSFER_BARANG_KELUAR_TOKO, AWAL_GUDANG, TAMBAH_GUDANG, KURANG_GUDANG, " &
 "PEMBELIAN_GUDANG, PENJUALAN_GUDANG, RETUR_BELI_GUDANG, RETUR_JUAL_GUDANG, OPNAME_GUDANG, TRANSFER_STOK_MASUK_GUDANG, TRANSFER_STOK_KELUAR_GUDANG, " &
 "TRANSFER_BARANG_MASUK_GUDANG, TRANSFER_BARANG_KELUAR_GUDANG, SATUAN_STOK, SATUAN_ISI_STOK, STOK_MIN, STOK_MAX, LOKASI_RAK_TOKO, LOKASI_RAK_GUDANG, " &
 "POINT_MEMBER, POINT_KARYAWAN, KOMISI_SALES_RP, KOMISI_SALES_PERSEN) " &
 "VALUES (" &
 "@ID_BARANG, @NAMA_BARANG, @KODE_KATEGORI, @NAMA_KATEGORI, @KODE_SUPLIYER, @NAMA_SUPLIYER, @HARGA_BELI, @HARGA_BELI_TERAKHIR, " &
 "@BARCODE_KECIL, @BARCODE_SEDANG, @BARCODE_BESAR, @SATUAN_UMUM_KECIL, @SATUAN_UMUM_SEDANG, @SATUAN_UMUM_BESAR, " &
 "@ISI_UMUM_KECIL, @ISI_UMUM_SEDANG, @ISI_UMUM_BESAR, " &
 "@HARGA_JUAL_UMUM_KECIL, @HARGA_JUAL_UMUM_SEDANG, @HARGA_JUAL_UMUM_BESAR, @SATUAN_PARTAI_KECIL, @SATUAN_PARTAI_SEDANG, " &
 "@SATUAN_PARTAI_BESAR, @ISI_PARTAI_KECIL, @ISI_PARTAI_SEDANG, @ISI_PARTAI_BESAR, " &
 "@HARGA_JUAL_PARTAI_KECIL, @HARGA_JUAL_PARTAI_SEDANG, " &
 "@HARGA_JUAL_PARTAI_BESAR, @AWAL_TOKO, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, @AWAL_GUDANG, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, " &
 "@SATUAN_STOK, @SATUAN_ISI_STOK, @STOK_MIN, @STOK_MAX, @LOKASI_RAK_TOKO, @LOKASI_RAK_GUDANG, " &
 "@POINT_MEMBER, @POINT_KARYAWAN, @KOMISI_SALES_RP, @KOMISI_SALES_PERSEN)"



                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@ID_BARANG", StrConv(TxtKode.Text.Trim(), vbUpperCase))
                        cmd.Parameters.AddWithValue("@NAMA_BARANG", StrConv(TxtNama.Text.Trim(), vbProperCase))
                        cmd.Parameters.AddWithValue("@KODE_KATEGORI", TxtKategori.Text.Trim())
                        cmd.Parameters.AddWithValue("@NAMA_KATEGORI", CmbKategori.Text.Trim())
                        cmd.Parameters.AddWithValue("@KODE_SUPLIYER", TxtKodeSupliyer.Text.Trim())
                        cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", CmbSupliyer.Text.Trim())

                        ' Bersihkan dan parse harga beli
                        cmd.Parameters.AddWithValue("@HARGA_BELI", If(String.IsNullOrWhiteSpace(TxtHrgBeli.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHrgBeli.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@HARGA_BELI_TERAKHIR", If(String.IsNullOrWhiteSpace(TxtHrgBeli.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHrgBeli.Text.Trim()))))

                        cmd.Parameters.AddWithValue("@BARCODE_KECIL", TxtBarcodeUmumKecil.Text.Trim())
                        cmd.Parameters.AddWithValue("@BARCODE_SEDANG", TxtBarcodeUmumSedang.Text.Trim())
                        cmd.Parameters.AddWithValue("@BARCODE_BESAR", TxtBarcodeUmumBesar.Text.Trim())
                        cmd.Parameters.AddWithValue("@SATUAN_UMUM_KECIL", CmbSatUmumKecil.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_UMUM_SEDANG", CmbSatUmumSedang.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_UMUM_BESAR", CmbSatUmumBesar.Text)

                        ' Bersihkan dan parse isi umum
                        cmd.Parameters.AddWithValue("@ISI_UMUM_KECIL", If(String.IsNullOrWhiteSpace(TxtIsiUmumKecil.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumKecil.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@ISI_UMUM_SEDANG", If(String.IsNullOrWhiteSpace(TxtIsiUmumSedang.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumSedang.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@ISI_UMUM_BESAR", If(String.IsNullOrWhiteSpace(TxtIsiUmumBesar.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumBesar.Text.Trim()))))

                        ' Bersihkan dan parse harga jual umum
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_KECIL", If(String.IsNullOrWhiteSpace(TxtHArgaJUalUmumKecil.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJUalUmumKecil.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_SEDANG", If(String.IsNullOrWhiteSpace(TxtHArgaJUalUmumSedang.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJUalUmumSedang.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_BESAR", If(String.IsNullOrWhiteSpace(TxtHArgaJUalUmumBesar.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJUalUmumBesar.Text.Trim()))))

                        cmd.Parameters.AddWithValue("@SATUAN_PARTAI_KECIL", CmbSatPartaiKecil.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_PARTAI_SEDANG", CmbSatPartaiSedang.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_PARTAI_BESAR", CmbSatPartaiBesar.Text)

                        ' Bersihkan dan parse isi partai
                        cmd.Parameters.AddWithValue("@ISI_PARTAI_KECIL", If(String.IsNullOrWhiteSpace(TxtIsiPartaiKecil.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiPartaiKecil.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@ISI_PARTAI_SEDANG", If(String.IsNullOrWhiteSpace(TxtIsiPartaiSedang.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiPartaiSedang.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@ISI_PARTAI_BESAR", If(String.IsNullOrWhiteSpace(TxtIsiPartaiBesar.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiPartaiBesar.Text.Trim()))))

                        ' Bersihkan dan parse harga jual partai
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_KECIL", If(String.IsNullOrWhiteSpace(TxtHArgaJualPartaikecil.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJualPartaikecil.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_SEDANG", If(String.IsNullOrWhiteSpace(TxtHArgaJualPartaiSedang.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJualPartaiSedang.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_BESAR", If(String.IsNullOrWhiteSpace(TxtHArgaJualPartaiBesar.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJualPartaiBesar.Text.Trim()))))

                        ' Bersihkan dan parse stok awal dan max
                        If FormUtama.SLokasi.Text = "TOKO" Then
                            cmd.Parameters.AddWithValue("@AWAL_TOKO", If(String.IsNullOrWhiteSpace(TxtStokAwal.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtStokAwal.Text.Trim()))))
                            cmd.Parameters.AddWithValue("@AWAL_GUDANG", 0)
                        ElseIf FormUtama.SLokasi.Text = "GUDANG" Then
                            cmd.Parameters.AddWithValue("@AWAL_TOKO", 0)
                            cmd.Parameters.AddWithValue("@AWAL_GUDANG", If(String.IsNullOrWhiteSpace(TxtStokAwal.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtStokAwal.Text.Trim()))))
                        End If

                        cmd.Parameters.AddWithValue("@SATUAN_STOK", CmbSatUmumKecil.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_ISI_STOK", If(String.IsNullOrWhiteSpace(TxtIsiUmumKecil.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumKecil.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@STOK_MIN", If(String.IsNullOrWhiteSpace(TextMin.Text.Trim()), 0, Decimal.Parse(BersihkanFormatAngka(TextMin.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@STOK_MAX", If(String.IsNullOrWhiteSpace(TxtStokMAx.Text.Trim()), 0, Decimal.Parse(BersihkanFormatAngka(TxtStokMAx.Text.Trim()))))

                        If FormUtama.SLokasi.Text = "TOKO" Then
                            cmd.Parameters.AddWithValue("@LOKASI_RAK_TOKO", TxtLokasiRak.Text)
                            cmd.Parameters.AddWithValue("@LOKASI_RAK_GUDANG", "")
                        ElseIf FormUtama.SLokasi.Text = "GUDANG" Then
                            cmd.Parameters.AddWithValue("@LOKASI_RAK_TOKO", "")
                            cmd.Parameters.AddWithValue("@LOKASI_RAK_GUDANG", TxtLokasiRak.Text)
                        End If

                        cmd.Parameters.AddWithValue("@POINT_MEMBER", If(String.IsNullOrWhiteSpace(TxtPointMember.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtPointMember.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@POINT_KARYAWAN", If(String.IsNullOrWhiteSpace(TxtPointKaryawan.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtPointKaryawan.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@KOMISI_SALES_RP", If(String.IsNullOrWhiteSpace(TxtKomisiSalesRp.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtKomisiSalesRp.Text.Trim()))))
                        cmd.Parameters.AddWithValue("@KOMISI_SALES_PERSEN", If(String.IsNullOrWhiteSpace(TxtKomisiSalesPersen.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtKomisiSalesPersen.Text.Trim()))))

                        cmd.ExecuteNonQuery()
                    End Using

                    If TotalNilaiBarang <> 0 Then
                        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                            "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
                            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                            cmd.Parameters.AddWithValue("@URAIAN", "Tambah barang " & TxtNama.Text)
                            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", LAWAN_NAMA_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", LAWAN_KODE_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NOMINAL", TotalNilaiBarang)
                            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Tambah Barang")
                            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

                            cmd.ExecuteNonQuery()
                        End Using

                    End If


                    transaction.Commit()

                    DatabaseModule.CatatanAksiHistory("Tambah barang " & TxtNama.Text)
                    HitungByKode(TxtKode.Text)
                    Call Kondisiawal()
                    GenerateItemCodeAutomatically()
                    TxtNama.Focus()
                Catch ex As Exception
                    transaction.Rollback()
                    MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            End If

        ElseIf LblUtama.Text = "E D I T   B A R A N G" Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Dim query As String = "UPDATE tbl_barang SET " &
                         "NAMA_BARANG = @NAMA_BARANG, " &
                         "KODE_KATEGORI = @KODE_KATEGORI, " &
                         "NAMA_KATEGORI = @NAMA_KATEGORI, " &
                         "KODE_SUPLIYER = @KODE_SUPLIYER, " &
                         "NAMA_SUPLIYER = @NAMA_SUPLIYER, " &
                         "HARGA_BELI = @HARGA_BELI, " &
                         "HARGA_BELI_TERAKHIR = @HARGA_BELI_TERAKHIR, " &
                         "BARCODE_KECIL = @BARCODE_KECIL, " &
                         "BARCODE_SEDANG = @BARCODE_SEDANG, " &
                         "BARCODE_BESAR = @BARCODE_BESAR, " &
                         "SATUAN_UMUM_KECIL = @SATUAN_UMUM_KECIL, " &
                         "SATUAN_UMUM_SEDANG = @SATUAN_UMUM_SEDANG, " &
                         "SATUAN_UMUM_BESAR = @SATUAN_UMUM_BESAR, " &
                         "ISI_UMUM_KECIL = @ISI_UMUM_KECIL, " &
                         "ISI_UMUM_SEDANG = @ISI_UMUM_SEDANG, " &
                         "ISI_UMUM_BESAR = @ISI_UMUM_BESAR, " &
                         "HARGA_JUAL_UMUM_KECIL = @HARGA_JUAL_UMUM_KECIL, " &
                         "HARGA_JUAL_UMUM_SEDANG = @HARGA_JUAL_UMUM_SEDANG, " &
                         "HARGA_JUAL_UMUM_BESAR = @HARGA_JUAL_UMUM_BESAR, " &
                         "SATUAN_PARTAI_KECIL = @SATUAN_PARTAI_KECIL, " &
                         "SATUAN_PARTAI_SEDANG = @SATUAN_PARTAI_SEDANG, " &
                         "SATUAN_PARTAI_BESAR = @SATUAN_PARTAI_BESAR, " &
                         "ISI_PARTAI_KECIL = @ISI_PARTAI_KECIL, " &
                         "ISI_PARTAI_SEDANG = @ISI_PARTAI_SEDANG, " &
                         "ISI_PARTAI_BESAR = @ISI_PARTAI_BESAR, " &
                         "HARGA_JUAL_PARTAI_KECIL = @HARGA_JUAL_PARTAI_KECIL, " &
                         "HARGA_JUAL_PARTAI_SEDANG = @HARGA_JUAL_PARTAI_SEDANG, " &
                         "HARGA_JUAL_PARTAI_BESAR = @HARGA_JUAL_PARTAI_BESAR, "

                If FormUtama.SLokasi.Text = "TOKO" Then
                    query &= "AWAL_TOKO = @AWAL_STOK, " &
                              "SATUAN_STOK = @SATUAN_STOK, " &
                              "SATUAN_ISI_STOK = @SATUAN_ISI_STOK, " &
                              "STOK_MIN = @STOK_MIN, " &
                              "STOK_MAX = @STOK_MAX, " &
                              "LOKASI_RAK_TOKO = @LOKASI_RAK, " &
                              "POINT_MEMBER = @POINT_MEMBER, " &
                              "POINT_KARYAWAN = @POINT_KARYAWAN, " &
                              "KOMISI_SALES_RP = @KOMISI_SALES_RP, " &
                              "KOMISI_SALES_PERSEN = @KOMISI_SALES_PERSEN " &
                              "WHERE ID_BARANG = @ID_BARANG"
                Else
                    query &= "AWAL_GUDANG = @AWAL_STOK, " &
                              "SATUAN_STOK = @SATUAN_STOK, " &
                              "SATUAN_ISI_STOK = @SATUAN_ISI_STOK, " &
                              "STOK_MIN = @STOK_MIN, " &
                              "STOK_MAX = @STOK_MAX, " &
                              "LOKASI_RAK_GUDANG = @LOKASI_RAK, " &
                              "POINT_MEMBER = @POINT_MEMBER, " &
                              "POINT_KARYAWAN = @POINT_KARYAWAN, " &
                              "KOMISI_SALES_RP = @KOMISI_SALES_RP, " &
                              "KOMISI_SALES_PERSEN = @KOMISI_SALES_PERSEN " &
                              "WHERE ID_BARANG = @ID_BARANG"
                End If

                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", StrConv(TxtNama.Text.Trim(), vbProperCase))
                    cmd.Parameters.AddWithValue("@KODE_KATEGORI", TxtKategori.Text.Trim())
                    cmd.Parameters.AddWithValue("@NAMA_KATEGORI", CmbKategori.Text.Trim())
                    cmd.Parameters.AddWithValue("@KODE_SUPLIYER", TxtKodeSupliyer.Text.Trim())
                    cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", CmbSupliyer.Text.Trim())
                    cmd.Parameters.AddWithValue("@HARGA_BELI", If(String.IsNullOrWhiteSpace(TxtHrgBeli.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHrgBeli.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_BELI_TERAKHIR", If(String.IsNullOrWhiteSpace(TxtHargaBeliTerakhir.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHargaBeliTerakhir.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@BARCODE_KECIL", TxtBarcodeUmumKecil.Text.Trim())
                    cmd.Parameters.AddWithValue("@BARCODE_SEDANG", TxtBarcodeUmumSedang.Text.Trim())
                    cmd.Parameters.AddWithValue("@BARCODE_BESAR", TxtBarcodeUmumBesar.Text.Trim())
                    cmd.Parameters.AddWithValue("@SATUAN_UMUM_KECIL", CmbSatUmumKecil.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_UMUM_SEDANG", CmbSatUmumSedang.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_UMUM_BESAR", CmbSatUmumBesar.Text)
                    cmd.Parameters.AddWithValue("@ISI_UMUM_KECIL", If(String.IsNullOrWhiteSpace(TxtIsiUmumKecil.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumKecil.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@ISI_UMUM_SEDANG", If(String.IsNullOrWhiteSpace(TxtIsiUmumSedang.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumSedang.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@ISI_UMUM_BESAR", If(String.IsNullOrWhiteSpace(TxtIsiUmumBesar.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumBesar.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_KECIL", If(String.IsNullOrWhiteSpace(TxtHArgaJUalUmumKecil.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJUalUmumKecil.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_SEDANG", If(String.IsNullOrWhiteSpace(TxtHArgaJUalUmumSedang.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJUalUmumSedang.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_BESAR", If(String.IsNullOrWhiteSpace(TxtHArgaJUalUmumBesar.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJUalUmumBesar.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_KECIL", CmbSatPartaiKecil.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_SEDANG", CmbSatPartaiSedang.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_BESAR", CmbSatPartaiBesar.Text)
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_KECIL", If(String.IsNullOrWhiteSpace(TxtIsiPartaiKecil.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiPartaiKecil.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_SEDANG", If(String.IsNullOrWhiteSpace(TxtIsiPartaiSedang.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiPartaiSedang.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_BESAR", If(String.IsNullOrWhiteSpace(TxtIsiPartaiBesar.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiPartaiBesar.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_KECIL", If(String.IsNullOrWhiteSpace(TxtHArgaJualPartaikecil.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJualPartaikecil.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_SEDANG", If(String.IsNullOrWhiteSpace(TxtHArgaJualPartaiSedang.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJualPartaiSedang.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_BESAR", If(String.IsNullOrWhiteSpace(TxtHArgaJualPartaiBesar.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJualPartaiBesar.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@AWAL_STOK", If(String.IsNullOrWhiteSpace(TxtStokAwal.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtStokAwal.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@STOK_MIN", If(String.IsNullOrWhiteSpace(TextMin.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TextMin.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@SATUAN_STOK", CmbSatUmumKecil.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_ISI_STOK", If(String.IsNullOrWhiteSpace(TxtIsiUmumKecil.Text.Trim()), 0, Integer.Parse(TxtIsiUmumKecil.Text.Trim())))
                    cmd.Parameters.AddWithValue("@STOK_MAX", If(String.IsNullOrWhiteSpace(TxtStokMAx.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtStokMAx.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@LOKASI_RAK", TxtLokasiRak.Text)
                    cmd.Parameters.AddWithValue("@POINT_MEMBER", If(String.IsNullOrWhiteSpace(TxtPointMember.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtPointMember.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@POINT_KARYAWAN", If(String.IsNullOrWhiteSpace(TxtPointKaryawan.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtPointKaryawan.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@KOMISI_SALES_RP", If(String.IsNullOrWhiteSpace(TxtKomisiSalesRp.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtKomisiSalesRp.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@KOMISI_SALES_PERSEN", If(String.IsNullOrWhiteSpace(TxtKomisiSalesPersen.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtKomisiSalesPersen.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@ID_BARANG", TxtKode.Text)

                    cmd.ExecuteNonQuery()
                End Using



                If SelisihNilaiBarang <> 0 Then
                    Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                           "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                        cmd.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
                        cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmd.Parameters.AddWithValue("@URAIAN", "Edit barang " & TxtNama.Text)

                        If SelisihNilaiBarang > 0 Then
                            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", LAWAN_NAMA_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", LAWAN_KODE_REK_BARANG)
                        Else
                            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", LAWAN_NAMA_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", LAWAN_KODE_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
                            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
                        End If

                        cmd.Parameters.AddWithValue("@NOMINAL", Math.Abs(SelisihNilaiBarang))
                        cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Edit Barang")
                        cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                        cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                        cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

                        cmd.ExecuteNonQuery()
                    End Using

                End If

                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Edit barang " & TxtNama.Text)
                HitungByKode(TxtKode.Text)
                Close()
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        ElseIf LblUtama.Text = "EDIT HARGA JUAL DARI PEMBELIAN" Or LblUtama.Text = "EDIT HARGA JUAL DARI PENJUALAN" Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Dim sql As String = "UPDATE tbl_barang SET " &
    "SATUAN_UMUM_KECIL=@SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG=@SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR=@SATUAN_UMUM_BESAR, " &
    "ISI_UMUM_KECIL=@ISI_UMUM_KECIL, ISI_UMUM_SEDANG=@ISI_UMUM_SEDANG, ISI_UMUM_BESAR=@ISI_UMUM_BESAR, " &
    "HARGA_JUAL_UMUM_KECIL=@HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG=@HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR=@HARGA_JUAL_UMUM_BESAR, " &
    "SATUAN_PARTAI_KECIL=@SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG=@SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR=@SATUAN_PARTAI_BESAR, " &
    "ISI_PARTAI_KECIL=@ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG=@ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR=@ISI_PARTAI_BESAR, " &
    "HARGA_JUAL_PARTAI_KECIL=@HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG=@HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR=@HARGA_JUAL_PARTAI_BESAR " &
    "WHERE ID_BARANG=@ID_BARANG"


                Using cmd As New MySqlCommand(sql, conn, transaction)
                    cmd.Parameters.AddWithValue("@SATUAN_UMUM_KECIL", CmbSatUmumKecil.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_UMUM_SEDANG", CmbSatUmumSedang.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_UMUM_BESAR", CmbSatUmumBesar.Text)

                    ' Bersihkan format angka sebelum parsing
                    cmd.Parameters.AddWithValue("@ISI_UMUM_KECIL", If(String.IsNullOrWhiteSpace(TxtIsiUmumKecil.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumKecil.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@ISI_UMUM_SEDANG", If(String.IsNullOrWhiteSpace(TxtIsiUmumSedang.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumSedang.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@ISI_UMUM_BESAR", If(String.IsNullOrWhiteSpace(TxtIsiUmumBesar.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiUmumBesar.Text.Trim()))))

                    ' Bersihkan format angka sebelum parsing
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_KECIL", If(String.IsNullOrWhiteSpace(TxtHArgaJUalUmumKecil.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJUalUmumKecil.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_SEDANG", If(String.IsNullOrWhiteSpace(TxtHArgaJUalUmumSedang.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJUalUmumSedang.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_BESAR", If(String.IsNullOrWhiteSpace(TxtHArgaJUalUmumBesar.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJUalUmumBesar.Text.Trim()))))

                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_KECIL", CmbSatPartaiKecil.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_SEDANG", CmbSatPartaiSedang.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_BESAR", CmbSatPartaiBesar.Text)

                    ' Bersihkan format angka sebelum parsing
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_KECIL", If(String.IsNullOrWhiteSpace(TxtIsiPartaiKecil.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiPartaiKecil.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_SEDANG", If(String.IsNullOrWhiteSpace(TxtIsiPartaiSedang.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiPartaiSedang.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_BESAR", If(String.IsNullOrWhiteSpace(TxtIsiPartaiBesar.Text.Trim()), 0, Integer.Parse(BersihkanFormatAngka(TxtIsiPartaiBesar.Text.Trim()))))

                    ' Bersihkan format angka sebelum parsing
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_KECIL", If(String.IsNullOrWhiteSpace(TxtHArgaJualPartaikecil.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJualPartaikecil.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_SEDANG", If(String.IsNullOrWhiteSpace(TxtHArgaJualPartaiSedang.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJualPartaiSedang.Text.Trim()))))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_BESAR", If(String.IsNullOrWhiteSpace(TxtHArgaJualPartaiBesar.Text.Trim()), 0D, Decimal.Parse(BersihkanFormatAngka(TxtHArgaJualPartaiBesar.Text.Trim()))))

                    cmd.Parameters.AddWithValue("@ID_BARANG", TxtKode.Text)
                    cmd.ExecuteNonQuery()
                End Using



                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Rubah harga " & TxtNama.Text)

                GBBarcode.Visible = True
                GBStok.Visible = True
                GBBarang.Enabled = True
                GBPoint.Visible = True
                BtnTambahKategori.Visible = True
                BtnSupliyer.Visible = True
                BtnTambahSatuan.Visible = True
                CBManual.Visible = True
                BtnBaru.Visible = True
                'BackColor = Color.DarkCyan
                Size = New Size(1143, 590)
                Close()
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try


        End If

    End Sub


    Private Function BersihkanFormatAngka(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return String.Empty
        Return input.Replace(".", "").Replace(",", ".")
    End Function

    Private Sub BtnBaru_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBaru.Click
        Kondisiawal()
        CmbKategori.Items.Clear()
        TxtKategori.Clear()
        CmbSupliyer.Items.Clear()
        TxtKodeSupliyer.Clear()
    End Sub

    Private Sub BtnMinimize_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnMinimize.Click
        WindowState = FormWindowState.Minimized
    End Sub


    Private Sub TxtBarcodeUmumKecil_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtBarcodeUmumKecil.KeyPress
        If e.KeyChar = Chr(13) Then
            CmbSatUmumKecil.Focus()
        End If
    End Sub

    Private Sub TxtBarcodeUmumSedang_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtBarcodeUmumSedang.KeyPress
        If e.KeyChar = Chr(13) Then
            CmbSatUmumSedang.Focus()
        End If
    End Sub

    Private Sub TxtBarcodeUmumBesar_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtBarcodeUmumBesar.KeyPress
        If e.KeyChar = Chr(13) Then
            CmbSatUmumBesar.Focus()
        End If
    End Sub

    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        If LblUtama.Text = "EDIT HARGA JUAL DARI PEMBELIAN" Or LblUtama.Text = "EDIT HARGA JUAL DARI PENJUALAN" Then
            GBBarcode.Visible = True
            GBStok.Visible = True
            GBBarang.Enabled = True
            GBPoint.Visible = True
            BtnTambahKategori.Visible = True
            BtnSupliyer.Visible = True
            BtnTambahSatuan.Visible = True
            CBManual.Visible = True
            BtnBaru.Visible = True
            'BackColor = Color.DarkCyan
            Size = New Size(1143, 590)
        End If
        Close()
    End Sub

    Private Sub TambahBarang_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                BtnSimpan.PerformClick()
            Case Keys.F3
                BtnBaru.PerformClick()
            Case Keys.F5
                BtnTambahKategori.PerformClick()
            Case Keys.F6
                BtnSupliyer.PerformClick()
            Case Keys.F7
                BtnTambahSatuan.PerformClick()
            Case Keys.Escape
                BtnClose.PerformClick()
        End Select
    End Sub




End Class