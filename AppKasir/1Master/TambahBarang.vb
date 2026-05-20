Public Class TambahBarang

    ' Field level class — dipakai bersama oleh Ubahhargabeli dan UbahhargaJual
    Private _namaBarang As String = String.Empty
    Private _kodeKategori As String = String.Empty
    Private _namaKategori As String = String.Empty
    Private _kodeSupliyer As String = String.Empty
    Private _namaSupliyer As String = String.Empty
    Private _kodeMerk As String = String.Empty
    Private _namaMerk As String = String.Empty
    Private _hargaBeli As Decimal = 0D
    Private _hargabeliterakhir As Decimal = 0D
    Private _satuanUmumKecil As String = String.Empty
    Private _satuanUmumSedang As String = String.Empty
    Private _satuanUmumBesar As String = String.Empty
    Private _isiUmumKecil As Integer = 0
    Private _isiUmumSedang As Integer = 0
    Private _isiUmumBesar As Integer = 0
    Private _hargaJualUmumKecil As Decimal = 0D
    Private _hargaJualUmumSedang As Decimal = 0D
    Private _hargaJualUmumBesar As Decimal = 0D
    Private _satuanPartaiKecil As String = String.Empty
    Private _satuanPartaiSedang As String = String.Empty
    Private _satuanPartaiBesar As String = String.Empty
    Private _isiPartaiKecil As Integer = 0
    Private _isiPartaiSedang As Integer = 0
    Private _isiPartaiBesar As Integer = 0
    Private _hargaJualPartaiKecil As Decimal = 0D
    Private _hargaJualPartaiSedang As Decimal = 0D
    Private _hargaJualPartaiBesar As Decimal = 0D
    Private _stokTokoAwal As Decimal = 0D
    Private _stokGudangAwal As Decimal = 0D
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
    Private Sub Lblutama_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LblHeaderForm.MouseDown
        If e.Button = MouseButtons.Left Then
            dragging = True
            offsetX = e.X
            offsetY = e.Y
        End If
    End Sub

    ' Event untuk memindahkan form saat mouse digerakkan
    Private Sub Lblutama_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LblHeaderForm.MouseMove
        If dragging Then
            Me.Location = New Point(Me.Location.X + e.X - offsetX, Me.Location.Y + e.Y - offsetY)
        End If
    End Sub

    ' Event untuk menghentikan mode drag saat mouse dilepas
    Private Sub Lblutama_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles LblHeaderForm.MouseUp
        dragging = False
    End Sub


    Private Sub TambahBarang_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        TerapkanModeAutoLevelSatuan()
        ' FormBorderStyle=None tidak punya shadow bawaan Windows, jadi perlu warna berbeda.
        ' Light: putih bersih (#FFFFFF) | Dark: Slate-800 (#1E293B)
        Me.BackColor = ModuleTheme.C(
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(30, 41, 59))

        ' Border tepi 6px — warna solid biru Master (#2563EB), sama di light & dark
        ' Biru solid selalu kontras di atas background apapun
        Dim borderColor As Color = Color.FromArgb(37, 99, 235)
        PnlBatas1.BackColor = borderColor
        PnlBatas3.BackColor = borderColor
        PnlBatas2.BackColor = borderColor
        Me.Cursor = Cursors.WaitCursor
        CBSatuanSama.Checked = AppConfig.Instance.GetValue(Of Boolean)("CbSatuansama", False)
        LblJudulStok.Text = "INFORMASI STOK " & FormUtama.StatusLokasi.Text
        Label22.Text = "Lokasi Rak " & FormUtama.StatusLokasi.Text

        If LblHeaderForm.Text = "T A M B A H   B A R A N G" Then
            Me.Size = New Size(1150, 702)
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
            Call TampilMerk()

            Kondisiawal()
            ResetIsi()
            ' Fokus ke txtNama
            TxtNama.Focus()

        ElseIf LblHeaderForm.Text = "E D I T   B A R A N G" Then
            Me.Size = New Size(1150, 702)
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


        ElseIf LblHeaderForm.Text = "EDIT HARGA JUAL DARI PEMBELIAN" Then
            Label44.Visible = True
            TxtHargaBeliTerakhir.Visible = True

            Ubahhargabeli()
            Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
            LblHargaBeli.Text = "Rp. " & ModuleAngka.FormatRupiah(_hargaBeli)
            SetHargaBeliUmum(_hargaBeli)
            SetHargaBeliPartai(_hargaBeli)

            Hitunghargasebelumedit()
            TxtHArgaJUalUmumKecil.Focus()
        ElseIf LblHeaderForm.Text = "EDIT HARGA JUAL DARI PENJUALAN" Then
            UbahhargaJual()
        End If

        Me.Cursor = Cursors.Default
    End Sub


    Public Sub Kondisiawal()
        TxtKode.Enabled = False

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

    Public Sub SetHargaBeliUmum(ByVal _hargaBeli As Decimal)
        Dim _isiUmumKecil As Decimal = ModuleAngka.ParseDecimal(TxtIsiUmumKecil.Text)
        Dim _isiUmumSedang As Decimal = ModuleAngka.ParseDecimal(TxtIsiUmumSedang.Text)
        Dim _isiUmumBesar As Decimal = ModuleAngka.ParseDecimal(TxtIsiUmumBesar.Text)

        TxtHargaBeliUmumKecil.Text = (_hargaBeli * _isiUmumKecil).ToString()
        TxtHargaBeliUmumSedang.Text = (_hargaBeli * _isiUmumSedang).ToString()
        TxtHargaBeliUmumBesar.Text = (_hargaBeli * _isiUmumBesar).ToString()
    End Sub


    Public Sub SetHargaBeliPartai(ByVal _hargaBeli As Decimal)
        Dim _isiPartaiKecil As Decimal = ModuleAngka.ParseDecimal(TxtIsiPartaiKecil.Text)
        Dim _isiPartaiSedang As Decimal = ModuleAngka.ParseDecimal(TxtIsiPartaiSedang.Text)
        Dim _isiPartaiBesar As Decimal = ModuleAngka.ParseDecimal(TxtIsiPartaiBesar.Text)

        TxtHargaBeliPartaiKecil.Text = (_hargaBeli * _isiPartaiKecil).ToString()
        TxtHargaBeliPartaiSedang.Text = (_hargaBeli * _isiPartaiSedang).ToString()
        TxtHargaBeliPartaiBesar.Text = (_hargaBeli * _isiPartaiBesar).ToString()
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

        ' SQL Query
        Dim sql As String = "SELECT NAMA_BARANG, KODE_KATEGORI, NAMA_KATEGORI, KODE_SUPLIYER, " &
                            "NAMA_SUPLIYER, KODE_MERK, NAMA_MERK, HARGA_BELI, HARGA_BELI_TERAKHIR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
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
                    _namaBarang = rd("NAMA_BARANG").ToString()
                    _kodeKategori = rd("KODE_KATEGORI").ToString()
                    _namaKategori = rd("NAMA_KATEGORI").ToString()
                    _kodeSupliyer = rd("KODE_SUPLIYER").ToString()
                    _namaSupliyer = rd("NAMA_SUPLIYER").ToString()
                    _kodeMerk = If(rd("KODE_MERK") IsNot DBNull.Value, rd("KODE_MERK").ToString(), "")
                    _namaMerk = If(rd("NAMA_MERK") IsNot DBNull.Value, rd("NAMA_MERK").ToString(), "")
                    _hargaBeli = If(rd("HARGA_BELI") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_BELI").ToString()), 0D)
                    _hargabeliterakhir = If(rd("HARGA_BELI_TERAKHIR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_BELI_TERAKHIR").ToString()), 0D)
                    _satuanUmumKecil = rd("SATUAN_UMUM_KECIL").ToString()
                    _satuanUmumSedang = rd("SATUAN_UMUM_SEDANG").ToString()
                    _satuanUmumBesar = rd("SATUAN_UMUM_BESAR").ToString()
                    _isiUmumKecil = If(Integer.TryParse(rd("ISI_UMUM_KECIL").ToString(), _isiUmumKecil), _isiUmumKecil, 0)
                    _isiUmumSedang = If(Integer.TryParse(rd("ISI_UMUM_SEDANG").ToString(), _isiUmumSedang), _isiUmumSedang, 0)
                    _isiUmumBesar = If(Integer.TryParse(rd("ISI_UMUM_BESAR").ToString(), _isiUmumBesar), _isiUmumBesar, 0)
                    _hargaJualUmumKecil = If(rd("HARGA_JUAL_UMUM_KECIL") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_KECIL").ToString()), 0D)
                    _hargaJualUmumSedang = If(rd("HARGA_JUAL_UMUM_SEDANG") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_SEDANG").ToString()), 0D)
                    _hargaJualUmumBesar = If(rd("HARGA_JUAL_UMUM_BESAR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_BESAR").ToString()), 0D)
                    _satuanPartaiKecil = rd("SATUAN_PARTAI_KECIL").ToString()
                    _satuanPartaiSedang = rd("SATUAN_PARTAI_SEDANG").ToString()
                    _satuanPartaiBesar = rd("SATUAN_PARTAI_BESAR").ToString()
                    _isiPartaiKecil = If(Integer.TryParse(rd("ISI_PARTAI_KECIL").ToString(), _isiPartaiKecil), _isiPartaiKecil, 0)
                    _isiPartaiSedang = If(Integer.TryParse(rd("ISI_PARTAI_SEDANG").ToString(), _isiPartaiSedang), _isiPartaiSedang, 0)
                    _isiPartaiBesar = If(Integer.TryParse(rd("ISI_PARTAI_BESAR").ToString(), _isiPartaiBesar), _isiPartaiBesar, 0)
                    _hargaJualPartaiKecil = If(rd("HARGA_JUAL_PARTAI_KECIL") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_KECIL").ToString()), 0D)
                    _hargaJualPartaiSedang = If(rd("HARGA_JUAL_PARTAI_SEDANG") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_SEDANG").ToString()), 0D)
                    _hargaJualPartaiBesar = If(rd("HARGA_JUAL_PARTAI_BESAR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_BESAR").ToString()), 0D)
                    _stokTokoAwal = If(rd("STOK_TOKO") IsNot DBNull.Value, Decimal.Parse(rd("STOK_TOKO").ToString()), 0D)
                    _stokGudangAwal = If(rd("STOK_GUDANG") IsNot DBNull.Value, Decimal.Parse(rd("STOK_GUDANG").ToString()), 0D)
                End If
            End Using
        End Using

        ' Setelah using selesai, Anda bisa memasukkan nilai ke textbox
        TxtNama.Text = _namaBarang
        CmbKategori.Text = _namaKategori
        TxtKategori.Text = _kodeKategori
        CmbSupliyer.Text = _namaSupliyer
        TxtKodeSupliyer.Text = _kodeSupliyer
        CmbMerk.Text = _namaMerk
        TxtMerk.Text = _kodeMerk
        TxtHrgBeli.Text = _hargaBeli.ToString("0.##")
        TxtHargaBeliTerakhir.Text = _hargabeliterakhir.ToString("0.##")
        CmbSatUmumKecil.Text = _satuanUmumKecil
        CmbSatUmumSedang.Text = _satuanUmumSedang
        CmbSatUmumBesar.Text = _satuanUmumBesar
        TxtIsiUmumKecil.Text = _isiUmumKecil.ToString()
        TxtIsiUmumSedang.Text = _isiUmumSedang.ToString()
        TxtIsiUmumBesar.Text = _isiUmumBesar.ToString()
        TxtHArgaJUalUmumKecil.Text = _hargaJualUmumKecil.ToString("0.##")
        TxtHArgaJUalUmumSedang.Text = _hargaJualUmumSedang.ToString("0.##")
        TxtHArgaJUalUmumBesar.Text = _hargaJualUmumBesar.ToString("0.##")
        CmbSatPartaiKecil.Text = _satuanPartaiKecil
        CmbSatPartaiSedang.Text = _satuanPartaiSedang
        CmbSatPartaiBesar.Text = _satuanPartaiBesar
        TxtIsiPartaiKecil.Text = _isiPartaiKecil.ToString()
        TxtIsiPartaiSedang.Text = _isiPartaiSedang.ToString()
        TxtIsiPartaiBesar.Text = _isiPartaiBesar.ToString()
        TxtHArgaJualPartaikecil.Text = _hargaJualPartaiKecil.ToString("0.##")
        TxtHArgaJualPartaiSedang.Text = _hargaJualPartaiSedang.ToString("0.##")
        TxtHArgaJualPartaiBesar.Text = _hargaJualPartaiBesar.ToString("0.##")

        LblStokToko.Text = _stokTokoAwal.ToString("0.##")
        LblStokGudang.Text = _stokGudangAwal.ToString("0.##")

        If LblMetode.Text = "Harga Terbaru" Then
            Dim hargaBaru As Decimal = ModuleAngka.ParseDecimal(LblRpBaru.Text)
            TxtHrgBeli.Text = If(hargaBaru > 0, hargaBaru.ToString(), "0")
            LblRpJadi.Text = "Rp. " & ModuleAngka.FormatRupiah(hargaBaru)

            Dim STOK_TOKO As Decimal = ModuleAngka.ParseDecimal(LblStokToko.Text)
            Dim STOK_GUDANG As Decimal = ModuleAngka.ParseDecimal(LblStokGudang.Text)

            Dim totalstoklama As Decimal = If(LblJenis.Text = "Toko", STOK_TOKO, If(LblJenis.Text = "Gudang", STOK_GUDANG, STOK_TOKO + STOK_GUDANG))
            LblQtyLama.Text = Math.Max(totalstoklama, 0).ToString("N0")

        ElseIf LblMetode.Text = "Metode Average (Rata - Rata)" Then
            Dim hargaLama As Decimal = ModuleAngka.ParseDecimal(LblRpLama.Text)
            If hargaLama = 0 Then hargaLama = ModuleAngka.ParseDecimal(LblRpBaru.Text)
            Dim stokToko As Decimal = ModuleAngka.ParseDecimal(LblStokToko.Text)
            Dim stokGudang As Decimal = ModuleAngka.ParseDecimal(LblStokGudang.Text)

            Dim jenis As String = LblJenis.Text

            Dim totalStokLama As Decimal = If(jenis = "Toko", stokToko, If(jenis = "Gudang", stokGudang, stokToko + stokGudang))
            If FormPembelian.TxtJenisTrans.Text = "EditPembelian" Then totalStokLama -= ModuleAngka.ParseDecimal(LblQtySbl.Text)
            totalStokLama = Math.Max(totalStokLama, 0)
            LblQtyLama.Text = totalStokLama.ToString("N0")

            Dim totalHargaLama As Decimal = Math.Max(hargaLama * totalStokLama, 0)

            Dim hargaBaru As Decimal = ModuleAngka.ParseDecimal(LblRpBaru.Text)
            Dim stokBaru As Decimal = ModuleAngka.ParseDecimal(LblQtyBaru.Text)

            Dim totalHargaBaru As Decimal = hargaBaru * stokBaru
            Dim totalStok As Decimal = totalStokLama + stokBaru
            Dim totalHarga As Decimal = totalHargaLama + totalHargaBaru
            Dim hargaJadi As Decimal = If(totalStok > 0, Math.Round(totalHarga / totalStok, 0), hargaBaru)

            TxtHrgBeli.Text = hargaJadi.ToString("0.##")
            LblRpJadi.Text = "Rp. " & ModuleAngka.FormatRupiah(hargaJadi)

        ElseIf LblMetode.Text = "Tidak Ada" Then
            Dim hargaLama As Decimal = ModuleAngka.ParseDecimal(LblRpLama.Text)
            TxtHrgBeli.Text = hargaLama.ToString()
            LblRpJadi.Text = "Rp. " & ModuleAngka.FormatRupiah(hargaLama)

            Dim STOK_TOKO As Decimal = ModuleAngka.ParseDecimal(LblStokToko.Text)
            Dim STOK_GUDANG As Decimal = ModuleAngka.ParseDecimal(LblStokGudang.Text)

            Dim totalstoklama As Decimal = If(LblJenis.Text = "Toko", STOK_TOKO, If(LblJenis.Text = "Gudang", STOK_GUDANG, STOK_TOKO + STOK_GUDANG))
            LblQtyLama.Text = Math.Max(totalstoklama, 0).ToString("N0")
        End If


    End Sub


    Public Sub UbahhargaJual()
        ' SQL Query
        Dim sql As String = "SELECT NAMA_BARANG, KODE_KATEGORI, NAMA_KATEGORI, KODE_SUPLIYER, " &
                            "NAMA_SUPLIYER, KODE_MERK, NAMA_MERK, HARGA_BELI, HARGA_BELI_TERAKHIR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
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
                    _namaBarang = rd("NAMA_BARANG").ToString()
                    _kodeKategori = rd("KODE_KATEGORI").ToString()
                    _namaKategori = rd("NAMA_KATEGORI").ToString()
                    _kodeSupliyer = rd("KODE_SUPLIYER").ToString()
                    _namaSupliyer = rd("NAMA_SUPLIYER").ToString()
                    _kodeMerk = If(rd("KODE_MERK") IsNot DBNull.Value, rd("KODE_MERK").ToString(), "")
                    _namaMerk = If(rd("NAMA_MERK") IsNot DBNull.Value, rd("NAMA_MERK").ToString(), "")
                    _hargaBeli = If(rd("HARGA_BELI") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_BELI").ToString()), 0D)
                    _hargabeliterakhir = If(rd("HARGA_BELI_TERAKHIR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_BELI_TERAKHIR").ToString()), 0D)
                    _satuanUmumKecil = rd("SATUAN_UMUM_KECIL").ToString()
                    _satuanUmumSedang = rd("SATUAN_UMUM_SEDANG").ToString()
                    _satuanUmumBesar = rd("SATUAN_UMUM_BESAR").ToString()
                    _isiUmumKecil = If(Integer.TryParse(rd("ISI_UMUM_KECIL").ToString(), _isiUmumKecil), _isiUmumKecil, 0)
                    _isiUmumSedang = If(Integer.TryParse(rd("ISI_UMUM_SEDANG").ToString(), _isiUmumSedang), _isiUmumSedang, 0)
                    _isiUmumBesar = If(Integer.TryParse(rd("ISI_UMUM_BESAR").ToString(), _isiUmumBesar), _isiUmumBesar, 0)
                    _hargaJualUmumKecil = If(rd("HARGA_JUAL_UMUM_KECIL") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_KECIL").ToString()), 0D)
                    _hargaJualUmumSedang = If(rd("HARGA_JUAL_UMUM_SEDANG") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_SEDANG").ToString()), 0D)
                    _hargaJualUmumBesar = If(rd("HARGA_JUAL_UMUM_BESAR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_UMUM_BESAR").ToString()), 0D)
                    _satuanPartaiKecil = rd("SATUAN_PARTAI_KECIL").ToString()
                    _satuanPartaiSedang = rd("SATUAN_PARTAI_SEDANG").ToString()
                    _satuanPartaiBesar = rd("SATUAN_PARTAI_BESAR").ToString()
                    _isiPartaiKecil = If(Integer.TryParse(rd("ISI_PARTAI_KECIL").ToString(), _isiPartaiKecil), _isiPartaiKecil, 0)
                    _isiPartaiSedang = If(Integer.TryParse(rd("ISI_PARTAI_SEDANG").ToString(), _isiPartaiSedang), _isiPartaiSedang, 0)
                    _isiPartaiBesar = If(Integer.TryParse(rd("ISI_PARTAI_BESAR").ToString(), _isiPartaiBesar), _isiPartaiBesar, 0)
                    _hargaJualPartaiKecil = If(rd("HARGA_JUAL_PARTAI_KECIL") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_KECIL").ToString()), 0D)
                    _hargaJualPartaiSedang = If(rd("HARGA_JUAL_PARTAI_SEDANG") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_SEDANG").ToString()), 0D)
                    _hargaJualPartaiBesar = If(rd("HARGA_JUAL_PARTAI_BESAR") IsNot DBNull.Value, Decimal.Parse(rd("HARGA_JUAL_PARTAI_BESAR").ToString()), 0D)
                    _stokTokoAwal = If(rd("STOK_TOKO") IsNot DBNull.Value, Decimal.Parse(rd("STOK_TOKO").ToString()), 0D)
                    _stokGudangAwal = If(rd("STOK_GUDANG") IsNot DBNull.Value, Decimal.Parse(rd("STOK_GUDANG").ToString()), 0D)
                End If
            End Using
        End Using

        ' Setelah using selesai, Anda bisa memasukkan nilai ke textbox
        TxtNama.Text = _namaBarang
        CmbKategori.Text = _namaKategori
        TxtKategori.Text = _kodeKategori
        CmbSupliyer.Text = _namaSupliyer
        TxtKodeSupliyer.Text = _kodeSupliyer
        CmbMerk.Text = _namaMerk
        TxtMerk.Text = _kodeMerk
        TxtHrgBeli.Text = _hargaBeli.ToString("0.##")
        TxtHargaBeliTerakhir.Text = _hargabeliterakhir.ToString("0.##")
        CmbSatUmumKecil.Text = _satuanUmumKecil
        CmbSatUmumSedang.Text = _satuanUmumSedang
        CmbSatUmumBesar.Text = _satuanUmumBesar
        TxtIsiUmumKecil.Text = _isiUmumKecil.ToString()
        TxtIsiUmumSedang.Text = _isiUmumSedang.ToString()
        TxtIsiUmumBesar.Text = _isiUmumBesar.ToString()
        TxtHArgaJUalUmumKecil.Text = _hargaJualUmumKecil.ToString("0.##")
        TxtHArgaJUalUmumSedang.Text = _hargaJualUmumSedang.ToString("0.##")
        TxtHArgaJUalUmumBesar.Text = _hargaJualUmumBesar.ToString("0.##")
        CmbSatPartaiKecil.Text = _satuanPartaiKecil
        CmbSatPartaiSedang.Text = _satuanPartaiSedang
        CmbSatPartaiBesar.Text = _satuanPartaiBesar
        TxtIsiPartaiKecil.Text = _isiPartaiKecil.ToString()
        TxtIsiPartaiSedang.Text = _isiPartaiSedang.ToString()
        TxtIsiPartaiBesar.Text = _isiPartaiBesar.ToString()
        TxtHArgaJualPartaikecil.Text = _hargaJualPartaiKecil.ToString("0.##")
        TxtHArgaJualPartaiSedang.Text = _hargaJualPartaiSedang.ToString("0.##")
        TxtHArgaJualPartaiBesar.Text = _hargaJualPartaiBesar.ToString("0.##")

        LblStokToko.Text = _stokTokoAwal.ToString("0.##")
        LblStokGudang.Text = _stokGudangAwal.ToString("0.##")

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
                    Dim _namaKategori As String = rd("nama").ToString()
                    ' Tambahkan hanya jika belum ada di ComboBox
                    If Not CmbKategori.Items.Contains(_namaKategori) Then
                        CmbKategori.Items.Add(_namaKategori)
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
        Using cmd As New MySqlCommand("SELECT Nama FROM tbl_supliyer WHERE Status = 'Aktif' ORDER BY Nama ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Do While rd.Read()
                    Dim _namaSupliyer As String = rd.Item("Nama").ToString()
                    If Not CmbSupliyer.Items.Contains(_namaSupliyer) Then
                        CmbSupliyer.Items.Add(_namaSupliyer)
                    End If
                Loop
            End Using
        End Using
    End Sub

    Public Sub TampilMerk()
        CmbMerk.Items.Clear()
        CmbMerk.Items.Add("")
        Using cmd As New MySqlCommand("SELECT kode, nama FROM tbl_merk ORDER BY nama ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbMerk.Items.Add(rd("nama").ToString())
                End While
            End Using
        End Using
    End Sub


    Public Sub GenerateItemCodeAutomatically()
        Dim maxKode As String = ""
        Dim existingKode As New List(Of String)
        Dim _kodeKategori As String = TxtKategori.Text & "-"

        ' Ambil kode yang sudah ada dari database
        Using cmd As New MySqlCommand("SELECT ID_BARANG FROM tbl_barang WHERE ID_BARANG LIKE @CekNomor ORDER BY ID_BARANG", conn)
            cmd.Parameters.AddWithValue("@CekNomor", _kodeKategori & "%")

            Using rdGenerat As MySqlDataReader = cmd.ExecuteReader()
                While rdGenerat.Read()
                    existingKode.Add(rdGenerat(0).ToString())
                End While
            End Using
        End Using


        ' Jika tidak ada kode yang sudah ada
        If existingKode.Count = 0 Then
            TxtKode.Text = _kodeKategori & "000001"
            Exit Sub
        End If

        ' Cari kode berikutnya yang belum terpakai
        For i As Integer = 1 To existingKode.Count
            Dim expectedKode As String = _kodeKategori & i.ToString("000000")
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
                maxKode = _kodeKategori & (Hitung + 1).ToString("000000")
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

    ''' <summary>
    ''' Navigasi Enter: TxtNama → CmbKategori → CmbSupliyer → CmbMerk → TxtHrgBeli → TxtHargaBeliTerakhir → CmbSatUmumKecil
    ''' </summary>
    Private Sub TxtNama_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNama.KeyDown
        If e.KeyCode = Keys.Enter Then
            CmbKategori.Focus()
            CmbKategori.DroppedDown = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub CmbMerk_KeyDown(sender As Object, e As KeyEventArgs) Handles CmbMerk.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            TxtHrgBeli.Focus()
            TxtHrgBeli.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtHrgBeli_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtHrgBeli.KeyDown
        If e.KeyCode = Keys.Enter Then
            TxtHargaBeliTerakhir.Focus()
            TxtHargaBeliTerakhir.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtHargaBeliTerakhir_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtHargaBeliTerakhir.KeyDown
        If e.KeyCode = Keys.Enter Then
            CmbSatUmumKecil.Focus()
            CmbSatUmumKecil.DroppedDown = True
            e.SuppressKeyPress = True
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
                    X += Integer.Parse(Barcode(j))
                Else
                    Y += Integer.Parse(Barcode(j))
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
                    X += Integer.Parse(Barcode(j))
                Else
                    Y += Integer.Parse(Barcode(j))
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
                    X += Integer.Parse(Barcode(j))
                Else
                    Y += Integer.Parse(Barcode(j))
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

    Private Sub BtnTambahMerk_Click(sender As Object, e As EventArgs) Handles BtnTambahMerk.Click
        TambahMerk.ShowDialog()
        Call TampilMerk()
    End Sub

    Private Sub CmbMerk_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbMerk.SelectedIndexChanged
        If String.IsNullOrEmpty(CmbMerk.Text) Then
            TxtMerk.Clear()
            Return
        End If
        Using cmd As New MySqlCommand("SELECT kode FROM tbl_merk WHERE nama = @nama LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@nama", CmbMerk.Text)
            Dim val = cmd.ExecuteScalar()
            TxtMerk.Text = If(val IsNot Nothing AndAlso val IsNot DBNull.Value, val.ToString(), "")
        End Using
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

        If LblHeaderForm.Text = "T A M B A H   B A R A N G" Then
            Call GenerateItemCodeAutomatically()
        End If

    End Sub

    Private Sub CmbKategori_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbKategori.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            CmbSupliyer.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub CBManual_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBManual.CheckedChanged
        If CBManual.Checked = True Then
            TxtKode.Enabled = True
        Else
            TxtKode.Enabled = False
        End If
    End Sub

    Private Sub BtnSupliyer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambahSupliyer.Click
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
            CmbMerk.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtHrgBeli_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHrgBeli.TextChanged
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
        LblHargaBeli.Text = "Rp. " & If(_hargaBeli > 0, ModuleAngka.FormatRupiah(_hargaBeli), "0")
        UpdateHargaBeli(_hargaBeli)
    End Sub

    ''' <summary>
    ''' Saat TxtHrgBeli kehilangan fokus, isi TxtHargaBeliTerakhir secara otomatis
    ''' jika masih kosong atau 0. Jika sudah terisi, biarkan saja.
    ''' </summary>
    Private Sub TxtHrgBeli_Leave(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHrgBeli.Leave
        Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
        Dim hargaBeliTerakhir As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeliTerakhir.Text)
        If hargaBeliTerakhir = 0D AndAlso hargaBeli > 0D Then
            TxtHargaBeliTerakhir.Text = hargaBeli.ToString()
        End If
    End Sub

    Private Sub UpdateHargaBeli(ByVal _hargaBeli As Decimal)
        Dim _isiUmumKecil As Decimal = ModuleAngka.ParseDecimal(TxtIsiUmumKecil.Text)
        Dim _isiUmumSedang As Decimal = ModuleAngka.ParseDecimal(TxtIsiUmumSedang.Text)
        Dim _isiUmumBesar As Decimal = ModuleAngka.ParseDecimal(TxtIsiUmumBesar.Text)
        Dim _isiPartaiKecil As Decimal = ModuleAngka.ParseDecimal(TxtIsiPartaiKecil.Text)
        Dim _isiPartaiSedang As Decimal = ModuleAngka.ParseDecimal(TxtIsiPartaiSedang.Text)
        Dim _isiPartaiBesar As Decimal = ModuleAngka.ParseDecimal(TxtIsiPartaiBesar.Text)

        TxtHargaBeliUmumKecil.Text = (_hargaBeli * _isiUmumKecil).ToString()
        TxtHargaBeliUmumSedang.Text = (_hargaBeli * _isiUmumSedang).ToString()
        TxtHargaBeliUmumBesar.Text = (_hargaBeli * _isiUmumBesar).ToString()
        TxtHargaBeliPartaiKecil.Text = (_hargaBeli * _isiPartaiKecil).ToString()
        TxtHargaBeliPartaiSedang.Text = (_hargaBeli * _isiPartaiSedang).ToString()
        TxtHargaBeliPartaiBesar.Text = (_hargaBeli * _isiPartaiBesar).ToString()
    End Sub


    Private Sub TxtHargaBeliTerakhir_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtHargaBeliTerakhir.TextChanged
        Dim hargaBeliterakhir As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeliTerakhir.Text)
        LblHargaBeliTerakhir.Text = "Rp. " & ModuleAngka.FormatRupiah(hargaBeliterakhir)
    End Sub


    ''' <summary>
    ''' Jika SettingAutoLevelSatuan aktif: semua TxtIsi* di-set ke "1" dan ReadOnly.
    ''' Dipanggil saat Load dan setiap kali satuan dipilih dari ComboBox.
    ''' Tujuan: memastikan isi satuan selalu 1 agar kalkulasi qty di transaksi
    ''' selaras dengan logika auto level (kecil/sedang/besar berdasarkan qty).
    ''' </summary>
    Private Sub TerapkanModeAutoLevelSatuan()
        If Not ModulHakAkses.SettingAutoLevelSatuan Then
            ' Fitur nonaktif — pastikan semua TxtIsi bisa diedit normal
            TxtIsiUmumKecil.ReadOnly = False
            TxtIsiUmumSedang.ReadOnly = False
            TxtIsiUmumBesar.ReadOnly = False
            TxtIsiPartaiKecil.ReadOnly = False
            TxtIsiPartaiSedang.ReadOnly = False
            TxtIsiPartaiBesar.ReadOnly = False
            Return
        End If

        ' Fitur aktif — paksa isi = 1 dan readonly
        TxtIsiUmumKecil.Text = "1" : TxtIsiUmumKecil.ReadOnly = True
        TxtIsiUmumSedang.Text = "1" : TxtIsiUmumSedang.ReadOnly = True
        TxtIsiUmumBesar.Text = "1" : TxtIsiUmumBesar.ReadOnly = True
        TxtIsiPartaiKecil.Text = "1" : TxtIsiPartaiKecil.ReadOnly = True
        TxtIsiPartaiSedang.Text = "1" : TxtIsiPartaiSedang.ReadOnly = True
        TxtIsiPartaiBesar.Text = "1" : TxtIsiPartaiBesar.ReadOnly = True
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
            CmbStokAkhir.Text = CmbSatUmumKecil.Text

            ' Jika auto level aktif, override isi ke 1
            If ModulHakAkses.SettingAutoLevelSatuan Then TxtIsiUmumKecil.Text = "1"

            If CBSatuanSama.Checked Then
                If LblHeaderForm.Text = "T A M B A H   B A R A N G" Then
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
        Select Case e.KeyCode
            Case Keys.Enter, Keys.Down
                CmbSatUmumSedang.Focus()
                e.SuppressKeyPress = True
            Case Keys.Up
                ' Tidak ada di atas — biarkan default
        End Select
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

            ' Jika auto level aktif, override isi ke 1
            If ModulHakAkses.SettingAutoLevelSatuan Then TxtIsiUmumSedang.Text = "1"

            If CBSatuanSama.Checked Then
                If LblHeaderForm.Text = "T A M B A H   B A R A N G" Then
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
        Select Case e.KeyCode
            Case Keys.Enter, Keys.Down
                CmbSatUmumBesar.Focus()
                e.SuppressKeyPress = True
            Case Keys.Up
                CmbSatUmumKecil.Focus()
                e.SuppressKeyPress = True
        End Select
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

            ' Jika auto level aktif, override isi ke 1
            If ModulHakAkses.SettingAutoLevelSatuan Then TxtIsiUmumBesar.Text = "1"

            If CBSatuanSama.Checked Then
                If LblHeaderForm.Text = "T A M B A H   B A R A N G" Then
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
        Select Case e.KeyCode
            Case Keys.Enter
                TxtHArgaJUalUmumKecil.Focus()
                TxtHArgaJUalUmumKecil.SelectAll()
                e.SuppressKeyPress = True
            Case Keys.Down
                CmbSatPartaiKecil.Focus()
                CmbSatPartaiKecil.DroppedDown = True
                e.SuppressKeyPress = True
            Case Keys.Up
                CmbSatUmumSedang.Focus()
                e.SuppressKeyPress = True
        End Select
    End Sub
    Private Sub TxtKategori_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtKategori.TextChanged
        If LblHeaderForm.Text = "T A M B A H   B A R A N G" Then
            Call GenerateItemCodeAutomatically()
        End If

    End Sub
    Private Sub TxtIsiUmumKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiUmumKecil.TextChanged
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
        Dim isiUmum As Decimal = ModuleAngka.ParseDecimal(TxtIsiUmumKecil.Text)
        TxtHargaBeliUmumKecil.Text = (_hargaBeli * isiUmum).ToString()
    End Sub

    Private Sub TxtHArgaJUalUmumKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHArgaJUalUmumKecil.TextChanged, TxtHargaBeliUmumKecil.TextChanged
        Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(TxtHArgaJUalUmumKecil.Text)
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeliUmumKecil.Text)
        Dim laba As Decimal = hargaJual - _hargaBeli
        TxtLabaRpUmumKecil.Text = ModuleAngka.FormatAngka(laba)
        TxtLabaPersenUmumKecil.Text = If(hargaJual <> 0 AndAlso _hargaBeli <> 0,
                                         Math.Round((laba / _hargaBeli) * 100, 2).ToString(), "0")
        LbljualUmumKecil.Text = "Rp. " & ModuleAngka.FormatAngka(hargaJual)
    End Sub


    Private Sub TxtIsiUmumSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiUmumSedang.TextChanged
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
        Dim isiUmum As Decimal = ModuleAngka.ParseDecimal(TxtIsiUmumSedang.Text)
        TxtHargaBeliUmumSedang.Text = (_hargaBeli * isiUmum).ToString()
    End Sub


    Private Sub TxtHArgaJUalUmumSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHArgaJUalUmumSedang.TextChanged, TxtHargaBeliUmumSedang.TextChanged
        Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(TxtHArgaJUalUmumSedang.Text)
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeliUmumSedang.Text)
        Dim laba As Decimal = hargaJual - _hargaBeli
        TxtLabaRpUmumSedang.Text = ModuleAngka.FormatAngka(laba)
        TxtLabaPersenUmumSedang.Text = If(hargaJual <> 0 AndAlso _hargaBeli <> 0,
                                          Math.Round((laba / _hargaBeli) * 100, 2).ToString(), "0")
        LbljualUmumSedang.Text = "Rp. " & ModuleAngka.FormatAngka(hargaJual)
    End Sub


    Private Sub TxtIsiUmumBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiUmumBesar.TextChanged
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
        Dim isiUmum As Decimal = ModuleAngka.ParseDecimal(TxtIsiUmumBesar.Text)
        TxtHargaBeliUmumBesar.Text = (_hargaBeli * isiUmum).ToString()
    End Sub


    Private Sub TxtHArgaJUalUmumBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHArgaJUalUmumBesar.TextChanged, TxtHargaBeliUmumBesar.TextChanged
        Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(TxtHArgaJUalUmumBesar.Text)
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeliUmumBesar.Text)
        Dim laba As Decimal = hargaJual - _hargaBeli
        TxtLabaRpUmumBesar.Text = ModuleAngka.FormatAngka(laba)
        TxtLabaPersenUmumBesar.Text = If(hargaJual <> 0 AndAlso _hargaBeli <> 0,
                                         Math.Round((laba / _hargaBeli) * 100, 2).ToString(), "0")
        LbljualUmumBesar.Text = "Rp. " & ModuleAngka.FormatAngka(hargaJual)
    End Sub

    ''' <summary>
    ''' Navigasi keyboard untuk TxtHArgaJUalUmum (Kecil → Sedang → Besar) dan TxtHArgaJualPartai.
    ''' Enter/Panah Bawah: ke bawah | Panah Atas: ke atas
    ''' </summary>
    Private Sub TxtHArgaJUalUmum_KeyDown(sender As Object, e As KeyEventArgs) _
            Handles TxtHArgaJUalUmumKecil.KeyDown, TxtHArgaJUalUmumSedang.KeyDown, TxtHArgaJUalUmumBesar.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter, Keys.Down
                If sender Is TxtHArgaJUalUmumKecil Then
                    TxtHArgaJUalUmumSedang.Focus() : TxtHArgaJUalUmumSedang.SelectAll()
                ElseIf sender Is TxtHArgaJUalUmumSedang Then
                    TxtHArgaJUalUmumBesar.Focus() : TxtHArgaJUalUmumBesar.SelectAll()
                ElseIf sender Is TxtHArgaJUalUmumBesar Then
                    TxtHArgaJualPartaikecil.Focus() : TxtHArgaJualPartaikecil.SelectAll()
                End If
                e.SuppressKeyPress = True
            Case Keys.Up
                If sender Is TxtHArgaJUalUmumSedang Then
                    TxtHArgaJUalUmumKecil.Focus() : TxtHArgaJUalUmumKecil.SelectAll()
                ElseIf sender Is TxtHArgaJUalUmumBesar Then
                    TxtHArgaJUalUmumSedang.Focus() : TxtHArgaJUalUmumSedang.SelectAll()
                End If
                e.SuppressKeyPress = True
        End Select
    End Sub

    ''' <summary>
    ''' Navigasi keyboard untuk TxtHArgaJualPartai (Kecil → Sedang → Besar).
    ''' Enter/Panah Bawah: ke bawah | Panah Atas: ke atas
    ''' </summary>
    Private Sub TxtHArgaJUalPartai_KeyDown(sender As Object, e As KeyEventArgs) _
            Handles TxtHArgaJualPartaikecil.KeyDown, TxtHArgaJualPartaiSedang.KeyDown, TxtHArgaJualPartaiBesar.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter, Keys.Down
                If sender Is TxtHArgaJualPartaikecil Then
                    TxtHArgaJualPartaiSedang.Focus() : TxtHArgaJualPartaiSedang.SelectAll()
                ElseIf sender Is TxtHArgaJualPartaiSedang Then
                    TxtHArgaJualPartaiBesar.Focus() : TxtHArgaJualPartaiBesar.SelectAll()
                End If
                e.SuppressKeyPress = True
            Case Keys.Up
                If sender Is TxtHArgaJualPartaikecil Then
                    TxtHArgaJUalUmumBesar.Focus() : TxtHArgaJUalUmumBesar.SelectAll()
                ElseIf sender Is TxtHArgaJualPartaiSedang Then
                    TxtHArgaJualPartaikecil.Focus() : TxtHArgaJualPartaikecil.SelectAll()
                ElseIf sender Is TxtHArgaJualPartaiBesar Then
                    TxtHArgaJualPartaiSedang.Focus() : TxtHArgaJualPartaiSedang.SelectAll()
                End If
                e.SuppressKeyPress = True
        End Select
    End Sub

    Private Sub TxtHargaBeliUmumKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliUmumKecil.TextChanged
        LblBeliUmumKecil.Text = ModuleAngka.FormatAngka(ModuleAngka.ParseDecimal(TxtHargaBeliUmumKecil.Text))
    End Sub

    Private Sub TxtHargaBeliUmumSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliUmumSedang.TextChanged
        LblBeliUmumSedang.Text = ModuleAngka.FormatAngka(ModuleAngka.ParseDecimal(TxtHargaBeliUmumSedang.Text))
    End Sub

    Private Sub TxtHargaBeliUmumBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliUmumBesar.TextChanged
        LblBeliUmumBesar.Text = ModuleAngka.FormatAngka(ModuleAngka.ParseDecimal(TxtHargaBeliUmumBesar.Text))
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
            ' Jika auto level aktif, override isi ke 1
            If ModulHakAkses.SettingAutoLevelSatuan Then TxtIsiPartaiKecil.Text = "1"
        Else
            TxtIsiPartaiKecil.Text = "0"
        End If
    End Sub

    Private Sub CmbSatPartaiKecil_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSatPartaiKecil.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter, Keys.Down
                CmbSatPartaiSedang.Focus()
                e.SuppressKeyPress = True
            Case Keys.Up
                CmbSatUmumBesar.Focus()
                e.SuppressKeyPress = True
        End Select
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
            ' Jika auto level aktif, override isi ke 1
            If ModulHakAkses.SettingAutoLevelSatuan Then TxtIsiPartaiSedang.Text = "1"
        Else
            TxtIsiPartaiSedang.Text = "0"
        End If
    End Sub

    Private Sub CmbSatPartaiSedang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSatPartaiSedang.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter, Keys.Down
                CmbSatPartaiBesar.Focus()
                e.SuppressKeyPress = True
            Case Keys.Up
                CmbSatPartaiKecil.Focus()
                e.SuppressKeyPress = True
        End Select
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
            ' Jika auto level aktif, override isi ke 1
            If ModulHakAkses.SettingAutoLevelSatuan Then TxtIsiPartaiBesar.Text = "1"
        Else
            TxtIsiPartaiBesar.Text = "0"
        End If
    End Sub

    Private Sub CmbSatPartaiBesar_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles CmbSatPartaiBesar.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                TxtHArgaJualPartaikecil.Focus()
                TxtHArgaJualPartaikecil.SelectAll()
                e.SuppressKeyPress = True
            Case Keys.Down
                ' Tidak ada di bawah dalam grup ini
            Case Keys.Up
                CmbSatPartaiSedang.Focus()
                e.SuppressKeyPress = True
        End Select
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
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
        Dim _isiPartaiKecil As Decimal = ModuleAngka.ParseDecimal(TxtIsiPartaiKecil.Text)
        TxtHargaBeliPartaiKecil.Text = (_hargaBeli * _isiPartaiKecil).ToString()
    End Sub

    Private Sub UpdateLabaPartaiKecil()
        Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(TxtHArgaJualPartaikecil.Text)
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeliPartaiKecil.Text)
        Dim laba As Decimal = hargaJual - _hargaBeli
        TxtLabaRpPartaiKecil.Text = ModuleAngka.FormatAngka(laba)
        TxtLabaPersenPartaiKecil.Text = If(_hargaBeli <> 0,
                                           Math.Round(laba / _hargaBeli * 100, 2).ToString(), "0")
        LbljualPartaiKecil.Text = "Rp. " & ModuleAngka.FormatAngka(hargaJual)
    End Sub

    Private Sub UpdateHargaBeliPartaiSedang()
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
        Dim _isiPartaiSedang As Decimal = ModuleAngka.ParseDecimal(TxtIsiPartaiSedang.Text)
        TxtHargaBeliPartaiSedang.Text = (_hargaBeli * _isiPartaiSedang).ToString()
    End Sub

    Private Sub UpdateLabaPartaiSedang()
        Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(TxtHArgaJualPartaiSedang.Text)
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeliPartaiSedang.Text)
        Dim laba As Decimal = hargaJual - _hargaBeli
        TxtLabaRpPartaiSedang.Text = ModuleAngka.FormatAngka(laba)
        TxtLabaPersenPartaiSedang.Text = If(_hargaBeli <> 0,
                                            Math.Round(laba / _hargaBeli * 100, 2).ToString(), "0")
        LbljualPartaiSedang.Text = "Rp. " & ModuleAngka.FormatAngka(hargaJual)
    End Sub

    Private Sub UpdateHargaBeliPartaiBesar()
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
        Dim _isiPartaiBesar As Decimal = ModuleAngka.ParseDecimal(TxtIsiPartaiBesar.Text)
        TxtHargaBeliPartaiBesar.Text = (_hargaBeli * _isiPartaiBesar).ToString()
    End Sub

    Private Sub UpdateLabaPartaiBesar()
        Dim hargaJual As Decimal = ModuleAngka.ParseDecimal(TxtHArgaJualPartaiBesar.Text)
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeliPartaiBesar.Text)
        Dim laba As Decimal = hargaJual - _hargaBeli
        TxtLabaRpPartaiBesar.Text = ModuleAngka.FormatAngka(laba)
        TxtLabaPersenPartaiBesar.Text = If(_hargaBeli <> 0,
                                           Math.Round(laba / _hargaBeli * 100, 2).ToString(), "0")
        LbljualPartaiBesar.Text = "Rp. " & ModuleAngka.FormatAngka(hargaJual)
    End Sub


    Private Sub TxtHargaBeliPartaiKecil_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliPartaiKecil.TextChanged
        LblBeliPartaiKecil.Text = ModuleAngka.FormatAngka(ModuleAngka.ParseDecimal(TxtHargaBeliPartaiKecil.Text))
    End Sub

    Private Sub TxtHargaBeliPartaiSedang_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliPartaiSedang.TextChanged
        LblBeliPartaiSedang.Text = ModuleAngka.FormatAngka(ModuleAngka.ParseDecimal(TxtHargaBeliPartaiSedang.Text))
    End Sub

    Private Sub TxtHargaBeliPartaiBesar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtHargaBeliPartaiBesar.TextChanged
        LblBeliPartaiBesar.Text = ModuleAngka.FormatAngka(ModuleAngka.ParseDecimal(TxtHargaBeliPartaiBesar.Text))
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
        Dim stokAwal As Decimal = ModuleAngka.ParseDecimal(TxtStokAwal.Text)
        Dim stokToko As Decimal = ModuleAngka.ParseDecimal(TxtJmlhToko.Text)
        Dim stokGudang As Decimal = ModuleAngka.ParseDecimal(TxtJmlhGudang.Text)
        TxtStokAkhir.Text = If(FormUtama.StatusLokasi.Text = "TOKO",
                               (stokAwal + stokToko).ToString(),
                               (stokAwal + stokGudang).ToString())
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
                    Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    Dim stokToko As Decimal = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                    Dim stokGudang As Decimal = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))

                    nilaiBarang = (stokToko + stokGudang) * _hargaBeli
                End If
            End Using
        End Using
    End Sub
    Private Function CekBarang() As Boolean
        Dim sql As String = "SELECT ID_BARANG, NAMA_BARANG FROM tbl_barang WHERE STATUS = 'Aktif'"
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
        Else
            ' Validasi: jika satuan diisi maka isi tidak boleh 0 atau kosong
            Dim isiUmumKecil As Integer = ModuleAngka.ParseInteger(TxtIsiUmumKecil.Text)
            Dim isiUmumSedang As Integer = ModuleAngka.ParseInteger(TxtIsiUmumSedang.Text)
            Dim isiUmumBesar As Integer = ModuleAngka.ParseInteger(TxtIsiUmumBesar.Text)
            Dim isiPartaiKecil As Integer = ModuleAngka.ParseInteger(TxtIsiPartaiKecil.Text)
            Dim isiPartaiSedang As Integer = ModuleAngka.ParseInteger(TxtIsiPartaiSedang.Text)
            Dim isiPartaiBesar As Integer = ModuleAngka.ParseInteger(TxtIsiPartaiBesar.Text)

            If Not String.IsNullOrWhiteSpace(CmbSatUmumKecil.Text) AndAlso isiUmumKecil < 1 Then
                MessageBox.Show("Isi satuan Umum Kecil tidak boleh 0 atau kosong ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TxtIsiUmumKecil.Focus() : TxtIsiUmumKecil.SelectAll()
                isValid = False
            ElseIf Not String.IsNullOrWhiteSpace(CmbSatUmumSedang.Text) AndAlso isiUmumSedang < 1 Then
                MessageBox.Show("Isi satuan Umum Sedang tidak boleh 0 atau kosong ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TxtIsiUmumSedang.Focus() : TxtIsiUmumSedang.SelectAll()
                isValid = False
            ElseIf Not String.IsNullOrWhiteSpace(CmbSatUmumBesar.Text) AndAlso isiUmumBesar < 1 Then
                MessageBox.Show("Isi satuan Umum Besar tidak boleh 0 atau kosong ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TxtIsiUmumBesar.Focus() : TxtIsiUmumBesar.SelectAll()
                isValid = False
            ElseIf Not String.IsNullOrWhiteSpace(CmbSatPartaiKecil.Text) AndAlso isiPartaiKecil < 1 Then
                MessageBox.Show("Isi satuan Partai Kecil tidak boleh 0 atau kosong ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TxtIsiPartaiKecil.Focus() : TxtIsiPartaiKecil.SelectAll()
                isValid = False
            ElseIf Not String.IsNullOrWhiteSpace(CmbSatPartaiSedang.Text) AndAlso isiPartaiSedang < 1 Then
                MessageBox.Show("Isi satuan Partai Sedang tidak boleh 0 atau kosong ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TxtIsiPartaiSedang.Focus() : TxtIsiPartaiSedang.SelectAll()
                isValid = False
            ElseIf Not String.IsNullOrWhiteSpace(CmbSatPartaiBesar.Text) AndAlso isiPartaiBesar < 1 Then
                MessageBox.Show("Isi satuan Partai Besar tidak boleh 0 atau kosong ... !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TxtIsiPartaiBesar.Focus() : TxtIsiPartaiBesar.SelectAll()
                isValid = False
            End If
        End If

        Return isValid
    End Function

    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpan.Click
        ' Panggil fungsi validasi
        If Not IsInputValid() Then
            Exit Sub ' Jika validasi gagal, keluar dari metode
        End If

        Dim noTransaksi As String = DateTime.Now.ToString("yyyyMMddHHmmss")
        Dim NilaiBarang As Decimal = ModuleAngka.ParseDecimal(TxtStokAkhir.Text)
        Dim _hargaBeli As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
        Dim NilaiBarangAwal As Decimal = ModuleAngka.ParseDecimal(LblStokUntukEdit.Text)
        Dim hargaBeliAwal As Decimal = ModuleAngka.ParseDecimal(LblHargaUntukEdit.Text)

        Dim TotalNilaiBarang As Decimal = NilaiBarang * _hargaBeli
        Dim TotalNilaiBarangAwal As Decimal = NilaiBarangAwal * hargaBeliAwal
        Dim SelisihNilaiBarang As Decimal = TotalNilaiBarang - TotalNilaiBarangAwal


        If LblHeaderForm.Text = "T A M B A H   B A R A N G" Then
            If CekBarang() Then
                Dim transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    Dim query As String = "INSERT INTO tbl_barang (" &
 "ID_BARANG, NAMA_BARANG, KODE_KATEGORI, NAMA_KATEGORI, KODE_SUPLIYER, NAMA_SUPLIYER, KODE_MERK, NAMA_MERK, HARGA_BELI, HARGA_BELI_TERAKHIR, " &
 "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
 "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
 "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
 "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, " &
 "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, " &
 "HARGA_JUAL_PARTAI_BESAR, AWAL_TOKO, TAMBAH_TOKO, KURANG_TOKO, PEMBELIAN_TOKO, PENJUALAN_TOKO, RETUR_BELI_TOKO, " &
 "RETUR_JUAL_TOKO, OPNAME_TOKO, TRANSFER_STOK_MASUK_TOKO, TRANSFER_STOK_KELUAR_TOKO, TRANSFER_BARANG_MASUK_TOKO, TRANSFER_BARANG_KELUAR_TOKO, TRANSFER_CABANG_MASUK_TOKO, TRANSFER_CABANG_KELUAR_TOKO, AWAL_GUDANG, TAMBAH_GUDANG, KURANG_GUDANG, " &
 "PEMBELIAN_GUDANG, PENJUALAN_GUDANG, RETUR_BELI_GUDANG, RETUR_JUAL_GUDANG, OPNAME_GUDANG, TRANSFER_STOK_MASUK_GUDANG, TRANSFER_STOK_KELUAR_GUDANG, " &
 "TRANSFER_BARANG_MASUK_GUDANG, TRANSFER_BARANG_KELUAR_GUDANG, TRANSFER_CABANG_MASUK_GUDANG, TRANSFER_CABANG_KELUAR_GUDANG, SATUAN_STOK, SATUAN_ISI_STOK, STOK_MIN, STOK_MAX, LOKASI_RAK_TOKO, LOKASI_RAK_GUDANG, " &
 "POINT_MEMBER, POINT_KARYAWAN, KOMISI_SALES_RP, KOMISI_SALES_PERSEN) " &
 "VALUES (" &
 "@ID_BARANG, @NAMA_BARANG, @KODE_KATEGORI, @NAMA_KATEGORI, @KODE_SUPLIYER, @NAMA_SUPLIYER, @KODE_MERK, @NAMA_MERK, @HARGA_BELI, @HARGA_BELI_TERAKHIR, " &
 "@BARCODE_KECIL, @BARCODE_SEDANG, @BARCODE_BESAR, @SATUAN_UMUM_KECIL, @SATUAN_UMUM_SEDANG, @SATUAN_UMUM_BESAR, " &
 "@ISI_UMUM_KECIL, @ISI_UMUM_SEDANG, @ISI_UMUM_BESAR, " &
 "@HARGA_JUAL_UMUM_KECIL, @HARGA_JUAL_UMUM_SEDANG, @HARGA_JUAL_UMUM_BESAR, @SATUAN_PARTAI_KECIL, @SATUAN_PARTAI_SEDANG, " &
 "@SATUAN_PARTAI_BESAR, @ISI_PARTAI_KECIL, @ISI_PARTAI_SEDANG, @ISI_PARTAI_BESAR, " &
 "@HARGA_JUAL_PARTAI_KECIL, @HARGA_JUAL_PARTAI_SEDANG, " &
 "@HARGA_JUAL_PARTAI_BESAR, @AWAL_TOKO, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, @AWAL_GUDANG, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, " &
 "@SATUAN_STOK, @SATUAN_ISI_STOK, @STOK_MIN, @STOK_MAX, @LOKASI_RAK_TOKO, @LOKASI_RAK_GUDANG, " &
 "@POINT_MEMBER, @POINT_KARYAWAN, @KOMISI_SALES_RP, @KOMISI_SALES_PERSEN)"



                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@ID_BARANG", StrConv(TxtKode.Text.Trim(), vbUpperCase))
                        cmd.Parameters.AddWithValue("@NAMA_BARANG", StrConv(TxtNama.Text.Trim(), vbProperCase))
                        cmd.Parameters.AddWithValue("@KODE_KATEGORI", TxtKategori.Text.Trim())
                        cmd.Parameters.AddWithValue("@NAMA_KATEGORI", CmbKategori.Text.Trim())
                        cmd.Parameters.AddWithValue("@KODE_SUPLIYER", TxtKodeSupliyer.Text.Trim())
                        cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", CmbSupliyer.Text.Trim())
                        cmd.Parameters.AddWithValue("@KODE_MERK", TxtMerk.Text.Trim())
                        cmd.Parameters.AddWithValue("@NAMA_MERK", CmbMerk.Text.Trim())

                        cmd.Parameters.AddWithValue("@HARGA_BELI", ModuleAngka.ParseDecimal(TxtHrgBeli.Text))
                        cmd.Parameters.AddWithValue("@HARGA_BELI_TERAKHIR", ModuleAngka.ParseDecimal(TxtHrgBeli.Text))

                        cmd.Parameters.AddWithValue("@BARCODE_KECIL", TxtBarcodeUmumKecil.Text.Trim())
                        cmd.Parameters.AddWithValue("@BARCODE_SEDANG", TxtBarcodeUmumSedang.Text.Trim())
                        cmd.Parameters.AddWithValue("@BARCODE_BESAR", TxtBarcodeUmumBesar.Text.Trim())
                        cmd.Parameters.AddWithValue("@SATUAN_UMUM_KECIL", CmbSatUmumKecil.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_UMUM_SEDANG", CmbSatUmumSedang.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_UMUM_BESAR", CmbSatUmumBesar.Text)

                        cmd.Parameters.AddWithValue("@ISI_UMUM_KECIL", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiUmumKecil.Text)))
                        cmd.Parameters.AddWithValue("@ISI_UMUM_SEDANG", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiUmumSedang.Text)))
                        cmd.Parameters.AddWithValue("@ISI_UMUM_BESAR", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiUmumBesar.Text)))

                        cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_KECIL", ModuleAngka.ParseDecimal(TxtHArgaJUalUmumKecil.Text))
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_SEDANG", ModuleAngka.ParseDecimal(TxtHArgaJUalUmumSedang.Text))
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_BESAR", ModuleAngka.ParseDecimal(TxtHArgaJUalUmumBesar.Text))

                        cmd.Parameters.AddWithValue("@SATUAN_PARTAI_KECIL", CmbSatPartaiKecil.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_PARTAI_SEDANG", CmbSatPartaiSedang.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_PARTAI_BESAR", CmbSatPartaiBesar.Text)

                        cmd.Parameters.AddWithValue("@ISI_PARTAI_KECIL", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiPartaiKecil.Text)))
                        cmd.Parameters.AddWithValue("@ISI_PARTAI_SEDANG", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiPartaiSedang.Text)))
                        cmd.Parameters.AddWithValue("@ISI_PARTAI_BESAR", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiPartaiBesar.Text)))

                        cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_KECIL", ModuleAngka.ParseDecimal(TxtHArgaJualPartaikecil.Text))
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_SEDANG", ModuleAngka.ParseDecimal(TxtHArgaJualPartaiSedang.Text))
                        cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_BESAR", ModuleAngka.ParseDecimal(TxtHArgaJualPartaiBesar.Text))

                        If FormUtama.StatusLokasi.Text = "TOKO" Then
                            cmd.Parameters.AddWithValue("@AWAL_TOKO", ModuleAngka.ParseDecimal(TxtStokAwal.Text))
                            cmd.Parameters.AddWithValue("@AWAL_GUDANG", 0)
                        ElseIf FormUtama.StatusLokasi.Text = "GUDANG" Then
                            cmd.Parameters.AddWithValue("@AWAL_TOKO", 0)
                            cmd.Parameters.AddWithValue("@AWAL_GUDANG", ModuleAngka.ParseDecimal(TxtStokAwal.Text))
                        End If

                        cmd.Parameters.AddWithValue("@SATUAN_STOK", CmbSatUmumKecil.Text)
                        cmd.Parameters.AddWithValue("@SATUAN_ISI_STOK", ModuleAngka.ParseInteger(TxtIsiUmumKecil.Text))
                        cmd.Parameters.AddWithValue("@STOK_MIN", ModuleAngka.ParseDecimal(TextMin.Text))
                        cmd.Parameters.AddWithValue("@STOK_MAX", ModuleAngka.ParseDecimal(TxtStokMAx.Text))

                        If FormUtama.StatusLokasi.Text = "TOKO" Then
                            cmd.Parameters.AddWithValue("@LOKASI_RAK_TOKO", TxtLokasiRak.Text)
                            cmd.Parameters.AddWithValue("@LOKASI_RAK_GUDANG", "")
                        ElseIf FormUtama.StatusLokasi.Text = "GUDANG" Then
                            cmd.Parameters.AddWithValue("@LOKASI_RAK_TOKO", "")
                            cmd.Parameters.AddWithValue("@LOKASI_RAK_GUDANG", TxtLokasiRak.Text)
                        End If

                        cmd.Parameters.AddWithValue("@POINT_MEMBER", ModuleAngka.ParseDecimal(TxtPointMember.Text))
                        cmd.Parameters.AddWithValue("@POINT_KARYAWAN", ModuleAngka.ParseDecimal(TxtPointKaryawan.Text))
                        cmd.Parameters.AddWithValue("@KOMISI_SALES_RP", ModuleAngka.ParseDecimal(TxtKomisiSalesRp.Text))
                        cmd.Parameters.AddWithValue("@KOMISI_SALES_PERSEN", ModuleAngka.ParseDecimal(TxtKomisiSalesPersen.Text))

                        cmd.ExecuteNonQuery()
                    End Using

                    If TotalNilaiBarang <> 0 Then
                        Using cmd As New MySqlCommand(
                            "INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
                            "NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, " &
                            "NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                            "VALUES (@no_trx, @tgl, @no_nota, @uraian, " &
                            "@nama_akun_d, @nomor_akun_d, @nama_akun_k, @nomor_akun_k, " &
                            "@nominal, @jenis, @lokasi, @user, @komputer)", conn, transaction)
                            cmd.Parameters.AddWithValue("@no_trx", noTransaksi)
                            cmd.Parameters.AddWithValue("@tgl", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                            cmd.Parameters.AddWithValue("@no_nota", TxtKode.Text)
                            cmd.Parameters.AddWithValue("@uraian", "Tambah barang " & TxtNama.Text)
                            cmd.Parameters.AddWithValue("@nama_akun_d", NAMA_REK_BARANG)
                            cmd.Parameters.AddWithValue("@nomor_akun_d", KODE_REK_BARANG)
                            cmd.Parameters.AddWithValue("@nama_akun_k", LAWAN_NAMA_REK_BARANG)
                            cmd.Parameters.AddWithValue("@nomor_akun_k", LAWAN_KODE_REK_BARANG)
                            cmd.Parameters.AddWithValue("@nominal", TotalNilaiBarang)
                            cmd.Parameters.AddWithValue("@jenis", "TAMBAH BARANG")
                            cmd.Parameters.AddWithValue("@lokasi", FormUtama.StatusLokasi.Text)
                            cmd.Parameters.AddWithValue("@user", FormUtama.StatusNamaUser.Text)
                            cmd.Parameters.AddWithValue("@komputer", FormUtama.StatusNamaPC.Text)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If


                    ' Update saldo akun jurnal secara realtime (hanya jika ada nilai barang)
                    If TotalNilaiBarang <> 0 Then
                        UpdateSaldoAkunDeltaDariFaktur(noTransaksi, transaction)
                    End If

                    ' Recalculate stok barang
                    Dim stokSebelumTambah As Decimal = BacaStokSaatIni(TxtKode.Text, FormUtama.StatusLokasi.Text, transaction)
                    HitungStokPerubahan(TxtKode.Text, transaction)
                    Dim stokSesudahTambah As Decimal = BacaStokSaatIni(TxtKode.Text, FormUtama.StatusLokasi.Text, transaction)
                    Dim auditTambahBarang As New Dictionary(Of String, Decimal)() From {{TxtKode.Text, stokSesudahTambah - stokSebelumTambah}}
                    AuditStokTransaksi(TxtKode.Text, "Tambah Barang Baru", Nothing, Nothing, Nothing, auditTambahBarang, transaction)

                    transaction.Commit()

                    SyncTrigger.BarangBerubah(TxtKode.Text, "INSERT", ModuleVariabel.NamaUser)
                    Call Kondisiawal()
                    GenerateItemCodeAutomatically()
                    TxtNama.Focus()
                Catch ex As Exception
                    transaction.Rollback()
                    MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            End If

        ElseIf LblHeaderForm.Text = "E D I T   B A R A N G" Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' ========================================
                ' START: Audit Trail - Edit Barang
                ' ========================================
                Dim kodeBarang As String = TxtKode.Text
                Dim sbSnapshot As New System.Text.StringBuilder()
                Try
                    Dim sqlLama As String = "SELECT NAMA_BARANG, KODE_KATEGORI, NAMA_KATEGORI, " &
                        "KODE_SUPLIYER, NAMA_SUPLIYER, KODE_MERK, NAMA_MERK, " &
                        "HARGA_BELI, HARGA_BELI_TERAKHIR, " &
                        "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                        "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                        "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                        "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                        "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, " &
                        "ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, " &
                        "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, " &
                        "AWAL_TOKO, AWAL_GUDANG, STOK_MIN, STOK_MAX, " &
                        "LOKASI_RAK_TOKO, LOKASI_RAK_GUDANG, " &
                        "POINT_MEMBER, POINT_KARYAWAN, KOMISI_SALES_RP, KOMISI_SALES_PERSEN " &
                        "FROM tbl_barang WHERE ID_BARANG = @id LIMIT 1"
                    Using cmdLama As New MySqlCommand(sqlLama, conn, transaction)
                        cmdLama.Parameters.AddWithValue("@id", kodeBarang)
                        Using rdLama As MySqlDataReader = cmdLama.ExecuteReader()
                            If rdLama.Read() Then
                                Dim namaBaru As String = StrConv(TxtNama.Text.Trim(), vbProperCase)
                                Dim hargaBeliBaru As Decimal = ModuleAngka.ParseDecimal(TxtHrgBeli.Text)
                                Dim hargaBeliTerakhirBaru As Decimal = ModuleAngka.ParseDecimal(TxtHargaBeliTerakhir.Text)
                                Dim awallStokBaru As Decimal = ModuleAngka.ParseDecimal(TxtStokAwal.Text)
                                Dim stokMinBaru As Decimal = ModuleAngka.ParseDecimal(TextMin.Text)
                                Dim stokMaxBaru As Decimal = ModuleAngka.ParseDecimal(TxtStokMAx.Text)
                                Dim rakBaru As String = If(FormUtama.StatusLokasi.Text = "TOKO", TxtLokasiRak.Text, TxtLokasiRak.Text)
                                Dim pointMemberBaru As Decimal = ModuleAngka.ParseDecimal(TxtPointMember.Text)
                                Dim pointKaryawanBaru As Decimal = ModuleAngka.ParseDecimal(TxtPointKaryawan.Text)
                                Dim komisiSalesRpBaru As Decimal = ModuleAngka.ParseDecimal(TxtKomisiSalesRp.Text)
                                Dim komisiSalesPersenBaru As Decimal = ModuleAngka.ParseDecimal(TxtKomisiSalesPersen.Text)

                                sbSnapshot.AppendLine($"Kode Barang: {kodeBarang}")
                                sbSnapshot.AppendLine($"Nama: {rdLama("NAMA_BARANG")} → {namaBaru}")
                                sbSnapshot.AppendLine($"Kategori: {rdLama("KODE_KATEGORI")} - {rdLama("NAMA_KATEGORI")} → {TxtKategori.Text.Trim()} - {CmbKategori.Text.Trim()}")
                                sbSnapshot.AppendLine($"Supplier: {rdLama("KODE_SUPLIYER")} - {rdLama("NAMA_SUPLIYER")} → {TxtKodeSupliyer.Text.Trim()} - {CmbSupliyer.Text.Trim()}")
                                sbSnapshot.AppendLine($"Merk: {rdLama("KODE_MERK")} - {rdLama("NAMA_MERK")} → {TxtMerk.Text.Trim()} - {CmbMerk.Text.Trim()}")
                                sbSnapshot.AppendLine($"Harga Beli: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_BELI")))} → {ModuleAngka.FormatRupiah(hargaBeliBaru)}")
                                sbSnapshot.AppendLine($"Harga Beli Terakhir: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_BELI_TERAKHIR")))} → {ModuleAngka.FormatRupiah(hargaBeliTerakhirBaru)}")
                                sbSnapshot.AppendLine($"Barcode Kecil: {rdLama("BARCODE_KECIL")} → {TxtBarcodeUmumKecil.Text.Trim()}")
                                sbSnapshot.AppendLine($"Barcode Sedang: {rdLama("BARCODE_SEDANG")} → {TxtBarcodeUmumSedang.Text.Trim()}")
                                sbSnapshot.AppendLine($"Barcode Besar: {rdLama("BARCODE_BESAR")} → {TxtBarcodeUmumBesar.Text.Trim()}")
                                sbSnapshot.AppendLine($"Satuan Umum Kecil: {rdLama("SATUAN_UMUM_KECIL")} (isi: {rdLama("ISI_UMUM_KECIL")}) → {CmbSatUmumKecil.Text} (isi: {TxtIsiUmumKecil.Text})")
                                sbSnapshot.AppendLine($"Satuan Umum Sedang: {rdLama("SATUAN_UMUM_SEDANG")} (isi: {rdLama("ISI_UMUM_SEDANG")}) → {CmbSatUmumSedang.Text} (isi: {TxtIsiUmumSedang.Text})")
                                sbSnapshot.AppendLine($"Satuan Umum Besar: {rdLama("SATUAN_UMUM_BESAR")} (isi: {rdLama("ISI_UMUM_BESAR")}) → {CmbSatUmumBesar.Text} (isi: {TxtIsiUmumBesar.Text})")
                                sbSnapshot.AppendLine($"Harga Jual Umum Kecil: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_UMUM_KECIL")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJUalUmumKecil.Text))}")
                                sbSnapshot.AppendLine($"Harga Jual Umum Sedang: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_UMUM_SEDANG")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJUalUmumSedang.Text))}")
                                sbSnapshot.AppendLine($"Harga Jual Umum Besar: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_UMUM_BESAR")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJUalUmumBesar.Text))}")
                                sbSnapshot.AppendLine($"Satuan Partai Kecil: {rdLama("SATUAN_PARTAI_KECIL")} (isi: {rdLama("ISI_PARTAI_KECIL")}) → {CmbSatPartaiKecil.Text} (isi: {TxtIsiPartaiKecil.Text})")
                                sbSnapshot.AppendLine($"Satuan Partai Sedang: {rdLama("SATUAN_PARTAI_SEDANG")} (isi: {rdLama("ISI_PARTAI_SEDANG")}) → {CmbSatPartaiSedang.Text} (isi: {TxtIsiPartaiSedang.Text})")
                                sbSnapshot.AppendLine($"Satuan Partai Besar: {rdLama("SATUAN_PARTAI_BESAR")} (isi: {rdLama("ISI_PARTAI_BESAR")}) → {CmbSatPartaiBesar.Text} (isi: {TxtIsiPartaiBesar.Text})")
                                sbSnapshot.AppendLine($"Harga Jual Partai Kecil: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_PARTAI_KECIL")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJualPartaikecil.Text))}")
                                sbSnapshot.AppendLine($"Harga Jual Partai Sedang: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_PARTAI_SEDANG")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJualPartaiSedang.Text))}")
                                sbSnapshot.AppendLine($"Harga Jual Partai Besar: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_PARTAI_BESAR")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJualPartaiBesar.Text))}")
                                sbSnapshot.AppendLine($"Awal Stok: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("AWAL_TOKO")))} (Toko) / {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("AWAL_GUDANG")))} (Gudang) → {ModuleAngka.FormatRupiah(awallStokBaru)}")
                                sbSnapshot.AppendLine($"Stok Min: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("STOK_MIN")))} → {ModuleAngka.FormatRupiah(stokMinBaru)}")
                                sbSnapshot.AppendLine($"Stok Max: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("STOK_MAX")))} → {ModuleAngka.FormatRupiah(stokMaxBaru)}")
                                sbSnapshot.AppendLine($"Lokasi Rak: {If(FormUtama.StatusLokasi.Text = "TOKO", rdLama("LOKASI_RAK_TOKO"), rdLama("LOKASI_RAK_GUDANG"))} → {rakBaru}")
                                sbSnapshot.AppendLine($"Point Member: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("POINT_MEMBER")))} → {ModuleAngka.FormatRupiah(pointMemberBaru)}")
                                sbSnapshot.AppendLine($"Point Karyawan: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("POINT_KARYAWAN")))} → {ModuleAngka.FormatRupiah(pointKaryawanBaru)}")
                                sbSnapshot.AppendLine($"Komisi Sales (Rp): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("KOMISI_SALES_RP")))} → {ModuleAngka.FormatRupiah(komisiSalesRpBaru)}")
                                sbSnapshot.AppendLine($"Komisi Sales (%): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("KOMISI_SALES_PERSEN")))} → {ModuleAngka.FormatRupiah(komisiSalesPersenBaru)}")
                            End If
                        End Using
                    End Using
                Catch exDiff As Exception
                    sbSnapshot.AppendLine($"Gagal baca data sebelum edit: {exDiff.Message}")
                End Try
                ModuleAuditTrail.CatatAuditMaster("BRG:" & kodeBarang, "EDIT", "Master Barang", sbSnapshot.ToString(), trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Edit Barang
                ' ========================================

                Dim query As String = "UPDATE tbl_barang SET " &
                         "NAMA_BARANG = @NAMA_BARANG, " &
                         "KODE_KATEGORI = @KODE_KATEGORI, " &
                         "NAMA_KATEGORI = @NAMA_KATEGORI, " &
                         "KODE_SUPLIYER = @KODE_SUPLIYER, " &
                         "NAMA_SUPLIYER = @NAMA_SUPLIYER, " &
                         "KODE_MERK = @KODE_MERK, " &
                         "NAMA_MERK = @NAMA_MERK, " &
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

                If FormUtama.StatusLokasi.Text = "TOKO" Then
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
                    cmd.Parameters.AddWithValue("@KODE_MERK", TxtMerk.Text.Trim())
                    cmd.Parameters.AddWithValue("@NAMA_MERK", CmbMerk.Text.Trim())
                    cmd.Parameters.AddWithValue("@HARGA_BELI", ModuleAngka.ParseDecimal(TxtHrgBeli.Text))
                    cmd.Parameters.AddWithValue("@HARGA_BELI_TERAKHIR", ModuleAngka.ParseDecimal(TxtHargaBeliTerakhir.Text))
                    cmd.Parameters.AddWithValue("@BARCODE_KECIL", TxtBarcodeUmumKecil.Text.Trim())
                    cmd.Parameters.AddWithValue("@BARCODE_SEDANG", TxtBarcodeUmumSedang.Text.Trim())
                    cmd.Parameters.AddWithValue("@BARCODE_BESAR", TxtBarcodeUmumBesar.Text.Trim())
                    cmd.Parameters.AddWithValue("@SATUAN_UMUM_KECIL", CmbSatUmumKecil.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_UMUM_SEDANG", CmbSatUmumSedang.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_UMUM_BESAR", CmbSatUmumBesar.Text)
                    cmd.Parameters.AddWithValue("@ISI_UMUM_KECIL", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiUmumKecil.Text)))
                    cmd.Parameters.AddWithValue("@ISI_UMUM_SEDANG", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiUmumSedang.Text)))
                    cmd.Parameters.AddWithValue("@ISI_UMUM_BESAR", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiUmumBesar.Text)))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_KECIL", ModuleAngka.ParseDecimal(TxtHArgaJUalUmumKecil.Text))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_SEDANG", ModuleAngka.ParseDecimal(TxtHArgaJUalUmumSedang.Text))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_BESAR", ModuleAngka.ParseDecimal(TxtHArgaJUalUmumBesar.Text))
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_KECIL", CmbSatPartaiKecil.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_SEDANG", CmbSatPartaiSedang.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_BESAR", CmbSatPartaiBesar.Text)
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_KECIL", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiPartaiKecil.Text)))
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_SEDANG", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiPartaiSedang.Text)))
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_BESAR", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiPartaiBesar.Text)))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_KECIL", ModuleAngka.ParseDecimal(TxtHArgaJualPartaikecil.Text))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_SEDANG", ModuleAngka.ParseDecimal(TxtHArgaJualPartaiSedang.Text))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_BESAR", ModuleAngka.ParseDecimal(TxtHArgaJualPartaiBesar.Text))
                    cmd.Parameters.AddWithValue("@AWAL_STOK", ModuleAngka.ParseDecimal(TxtStokAwal.Text))
                    cmd.Parameters.AddWithValue("@STOK_MIN", ModuleAngka.ParseDecimal(TextMin.Text))
                    cmd.Parameters.AddWithValue("@SATUAN_STOK", CmbSatUmumKecil.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_ISI_STOK", ModuleAngka.ParseInteger(TxtIsiUmumKecil.Text))
                    cmd.Parameters.AddWithValue("@STOK_MAX", ModuleAngka.ParseDecimal(TxtStokMAx.Text))
                    cmd.Parameters.AddWithValue("@LOKASI_RAK", TxtLokasiRak.Text)
                    cmd.Parameters.AddWithValue("@POINT_MEMBER", ModuleAngka.ParseDecimal(TxtPointMember.Text))
                    cmd.Parameters.AddWithValue("@POINT_KARYAWAN", ModuleAngka.ParseDecimal(TxtPointKaryawan.Text))
                    cmd.Parameters.AddWithValue("@KOMISI_SALES_RP", ModuleAngka.ParseDecimal(TxtKomisiSalesRp.Text))
                    cmd.Parameters.AddWithValue("@KOMISI_SALES_PERSEN", ModuleAngka.ParseDecimal(TxtKomisiSalesPersen.Text))
                    cmd.Parameters.AddWithValue("@ID_BARANG", TxtKode.Text)

                    cmd.ExecuteNonQuery()
                End Using

                If SelisihNilaiBarang <> 0 Then
                    Dim namaAkunD As String = If(SelisihNilaiBarang > 0, NAMA_REK_BARANG, LAWAN_NAMA_REK_BARANG)
                    Dim nomorAkunD As String = If(SelisihNilaiBarang > 0, KODE_REK_BARANG, LAWAN_KODE_REK_BARANG)
                    Dim namaAkunK As String = If(SelisihNilaiBarang > 0, LAWAN_NAMA_REK_BARANG, NAMA_REK_BARANG)
                    Dim nomorAkunK As String = If(SelisihNilaiBarang > 0, LAWAN_KODE_REK_BARANG, KODE_REK_BARANG)

                    Using cmd As New MySqlCommand(
                        "INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
                        "NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, " &
                        "NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                        "VALUES (@no_trx, @tgl, @no_nota, @uraian, " &
                        "@nama_akun_d, @nomor_akun_d, @nama_akun_k, @nomor_akun_k, " &
                        "@nominal, @jenis, @lokasi, @user, @komputer)", conn, transaction)
                        cmd.Parameters.AddWithValue("@no_trx", noTransaksi)
                        cmd.Parameters.AddWithValue("@tgl", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmd.Parameters.AddWithValue("@no_nota", TxtKode.Text)
                        cmd.Parameters.AddWithValue("@uraian", "Edit barang " & TxtNama.Text)
                        cmd.Parameters.AddWithValue("@nama_akun_d", namaAkunD)
                        cmd.Parameters.AddWithValue("@nomor_akun_d", nomorAkunD)
                        cmd.Parameters.AddWithValue("@nama_akun_k", namaAkunK)
                        cmd.Parameters.AddWithValue("@nomor_akun_k", nomorAkunK)
                        cmd.Parameters.AddWithValue("@nominal", Math.Abs(SelisihNilaiBarang))
                        cmd.Parameters.AddWithValue("@jenis", "EDIT BARANG")
                        cmd.Parameters.AddWithValue("@lokasi", FormUtama.StatusLokasi.Text)
                        cmd.Parameters.AddWithValue("@user", FormUtama.StatusNamaUser.Text)
                        cmd.Parameters.AddWithValue("@komputer", FormUtama.StatusNamaPC.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                End If

                ' Update saldo akun jurnal secara realtime (hanya jika ada selisih nilai)
                If SelisihNilaiBarang <> 0 Then
                    UpdateSaldoAkunDeltaDariFaktur(noTransaksi, transaction)
                End If

                ' Recalculate stok barang
                Dim stokSebelumEdit As Decimal = BacaStokSaatIni(TxtKode.Text, FormUtama.StatusLokasi.Text, transaction)
                HitungStokPerubahan(TxtKode.Text, transaction)
                Dim stokSesudahEdit As Decimal = BacaStokSaatIni(TxtKode.Text, FormUtama.StatusLokasi.Text, transaction)
                Dim auditEditBarang As New Dictionary(Of String, Decimal)() From {{TxtKode.Text, Math.Abs(stokSesudahEdit - stokSebelumEdit)}}
                AuditStokTransaksi(TxtKode.Text, "Edit Barang", Nothing, Nothing, Nothing, auditEditBarang, transaction)

                transaction.Commit()

                SyncTrigger.BarangBerubah(TxtKode.Text, "UPDATE", ModuleVariabel.NamaUser)
                Close()
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        ElseIf LblHeaderForm.Text = "EDIT HARGA JUAL DARI PEMBELIAN" Or LblHeaderForm.Text = "EDIT HARGA JUAL DARI PENJUALAN" Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' ========================================
                ' START: Audit Trail - Edit Harga Jual
                ' ========================================
                Dim kodeBarang As String = TxtKode.Text
                Dim sbSnapshot As New System.Text.StringBuilder()
                Try
                    Dim sqlLama As String = "SELECT NAMA_BARANG, " &
                        "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                        "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
                        "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                        "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, " &
                        "ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, " &
                        "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR " &
                        "FROM tbl_barang WHERE ID_BARANG = @id LIMIT 1"
                    Using cmdLama As New MySqlCommand(sqlLama, conn, transaction)
                        cmdLama.Parameters.AddWithValue("@id", kodeBarang)
                        Using rdLama As MySqlDataReader = cmdLama.ExecuteReader()
                            If rdLama.Read() Then
                                sbSnapshot.AppendLine($"Kode Barang: {kodeBarang}")
                                sbSnapshot.AppendLine($"Nama Barang: {rdLama("NAMA_BARANG")}")
                                sbSnapshot.AppendLine($"Harga Jual Umum Kecil: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_UMUM_KECIL")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJUalUmumKecil.Text))}")
                                sbSnapshot.AppendLine($"Harga Jual Umum Sedang: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_UMUM_SEDANG")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJUalUmumSedang.Text))}")
                                sbSnapshot.AppendLine($"Harga Jual Umum Besar: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_UMUM_BESAR")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJUalUmumBesar.Text))}")
                                sbSnapshot.AppendLine($"Harga Jual Partai Kecil: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_PARTAI_KECIL")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJualPartaikecil.Text))}")
                                sbSnapshot.AppendLine($"Harga Jual Partai Sedang: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_PARTAI_SEDANG")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJualPartaiSedang.Text))}")
                                sbSnapshot.AppendLine($"Harga Jual Partai Besar: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdLama("HARGA_JUAL_PARTAI_BESAR")))} → {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(TxtHArgaJualPartaiBesar.Text))}")
                            End If
                        End Using
                    End Using
                Catch exDiff As Exception
                    sbSnapshot.AppendLine($"Gagal baca data sebelum edit: {exDiff.Message}")
                End Try
                ModuleAuditTrail.CatatAuditMaster("BRG:" & kodeBarang, "EDIT", "Edit Harga Jual", sbSnapshot.ToString(), trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Edit Harga Jual
                ' ========================================

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

                    cmd.Parameters.AddWithValue("@ISI_UMUM_KECIL", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiUmumKecil.Text)))
                    cmd.Parameters.AddWithValue("@ISI_UMUM_SEDANG", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiUmumSedang.Text)))
                    cmd.Parameters.AddWithValue("@ISI_UMUM_BESAR", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiUmumBesar.Text)))

                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_KECIL", ModuleAngka.ParseDecimal(TxtHArgaJUalUmumKecil.Text))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_SEDANG", ModuleAngka.ParseDecimal(TxtHArgaJUalUmumSedang.Text))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_UMUM_BESAR", ModuleAngka.ParseDecimal(TxtHArgaJUalUmumBesar.Text))

                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_KECIL", CmbSatPartaiKecil.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_SEDANG", CmbSatPartaiSedang.Text)
                    cmd.Parameters.AddWithValue("@SATUAN_PARTAI_BESAR", CmbSatPartaiBesar.Text)

                    cmd.Parameters.AddWithValue("@ISI_PARTAI_KECIL", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiPartaiKecil.Text)))
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_SEDANG", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiPartaiSedang.Text)))
                    cmd.Parameters.AddWithValue("@ISI_PARTAI_BESAR", Math.Max(1, ModuleAngka.ParseInteger(TxtIsiPartaiBesar.Text)))

                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_KECIL", ModuleAngka.ParseDecimal(TxtHArgaJualPartaikecil.Text))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_SEDANG", ModuleAngka.ParseDecimal(TxtHArgaJualPartaiSedang.Text))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL_PARTAI_BESAR", ModuleAngka.ParseDecimal(TxtHArgaJualPartaiBesar.Text))

                    cmd.Parameters.AddWithValue("@ID_BARANG", TxtKode.Text)
                    cmd.ExecuteNonQuery()
                End Using

                transaction.Commit()

                SyncTrigger.BarangBerubah(TxtKode.Text, "UPDATE", ModuleVariabel.NamaUser)

                GBInput1.Visible = True
                GBInput4.Visible = True
                GBInput.Enabled = True
                GBInput5.Visible = True
                BtnTambahKategori.Visible = True
                BtnTambahSupliyer.Visible = True
                BtnTambahSatuan.Visible = True
                CBManual.Visible = True
                BtnBaru.Visible = True
                'BackColor = Color.DarkCyan
                Size = New Size(1150, 702)
                Close()
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

    End Sub


    ' BersihkanFormatAngka dihapus — gunakan ModuleAngka.ParseDecimal / ModuleAngka.ParseInteger

    Private Sub BtnBaru_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBaru.Click
        Kondisiawal()
        CmbKategori.Items.Clear()
        TxtKategori.Clear()
        CmbSupliyer.Items.Clear()
        TxtKodeSupliyer.Clear()
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
        If LblHeaderForm.Text = "EDIT HARGA JUAL DARI PEMBELIAN" Or LblHeaderForm.Text = "EDIT HARGA JUAL DARI PENJUALAN" Then
            GBInput1.Visible = True
            GBInput4.Visible = True
            GBInput.Enabled = True
            GBInput5.Visible = True
            BtnTambahKategori.Visible = True
            BtnTambahSupliyer.Visible = True
            BtnTambahSatuan.Visible = True
            CBManual.Visible = True
            BtnBaru.Visible = True
            'BackColor = Color.DarkCyan
            Size = New Size(1150, 702)
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
                BtnTambahSupliyer.PerformClick()
            Case Keys.F7
                BtnTambahSatuan.PerformClick()
            Case Keys.Escape
                BtnClose.PerformClick()
        End Select
    End Sub
End Class



