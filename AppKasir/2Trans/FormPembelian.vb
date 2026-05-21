Public Class FormPembelian

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION A: PERHITUNGAN PER BARIS DGV
    ' ═══════════════════════════════════════════════════════════════════
    Private _idBarangPerBaris As String = ""              ' Kolom: Id
    Private _namaBarangPerBaris As String = ""            ' Kolom: Nama
    Private _qtyPerBaris As Decimal = 0D                 ' Kolom: Qty
    Private _isiPerBaris As Integer = 0                  ' Kolom: Isi (konversi)
    Private _qtySatPerBaris As Decimal = 0D              ' Kolom: QtySat
    Private _hargaBeliPerBaris As Decimal = 0D            ' Kolom: HargaBeli / Hargabeli
    Private _hargaBeliSatPerBaris As Decimal = 0D         ' Kolom: HargaBeliSatKecil
    Private _totalhargaPerBaris As Decimal = 0D           ' Kolom: Totalharga
    Private _satuanPerBaris As String = ""                ' Kolom: Satuan
    Private _averagePerBaris As Decimal = 0D              ' Kolom: Average
    Private _hargaSebelumnyaPerBaris As Decimal = 0D       ' Kolom: HargaSebelumnya

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION B: PERHITUNGAN AGREGAT (Dari semua baris DGV)
    ' ═══════════════════════════════════════════════════════════════════
    Private _subtotalBarang As Decimal = 0D          ' SUM(Totalharga) - nilai barang murni
    Private _totalQtyDalamSatuanTerkecil As Decimal = 0D  ' SUM(QtySat) untuk stok
    Private _jumlahJenisBarang As Integer = 0        ' COUNT baris - jenis barang

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION C: KOMPONEN TAMBAHAN (Input user)
    ' ═══════════════════════════════════════════════════════════════════
    Private _diskonPersen As Decimal = 0D            ' Persentase diskon (0-100)
    Private _diskonRupiah As Decimal = 0D            ' Nilai diskon dalam rupiah
    Private _ppnPersen As Decimal = 0D               ' Persentase PPN
    Private _ppnRupiah As Decimal = 0D               ' Nilai PPN dalam rupiah
    Private _biayaKirim As Decimal = 0D              ' Biaya kirim

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION D: GRAND TOTAL
    ' ═══════════════════════════════════════════════════════════════════
    Private _grandTotalPembelian As Decimal = 0D     ' subtotal - diskon + ppn + biayaKirim

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION E: PEMBAYARAN
    ' ═══════════════════════════════════════════════════════════════════
    Private _bayarTunai As Decimal = 0D              ' Input bayar tunai
    Private _bayarTransfer As Decimal = 0D           ' Input bayar transfer
    Private _totalBayar As Decimal = 0D              ' bayarTunai + bayarTransfer
    Private _sisaPembayaran As Decimal = 0D          ' grandTotal - totalBayar
    Private _sisaPembayaranMutlak As Decimal = 0D        ' Abs(sisaPembayaran) untuk validasi
    Private _kembalian As Decimal = 0D               ' Jika sisa >= 0
    Private _sisaHutang As Decimal = 0D              ' Jika sisa < 0

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION F: JURNAL (Untuk validasi keseimbangan)
    ' ═══════════════════════════════════════════════════════════════════
    Private _kasTunaiKeluar As Decimal = 0D          ' Kas yang keluar
    Private _debetJurnal As Decimal = 0D             ' Total debet
    Private _kreditJurnal As Decimal = 0D            ' Total kredit

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION G: HEADER TRANSAKSI
    ' ═══════════════════════════════════════════════════════════════════
    Private _idPembelian As String = ""              ' ID transaksi pembelian (TxtFaktur)
    Private _notaPembelian As String = ""            ' Nomor nota (TxtNota)
    Private _tanggalPembelian As DateTime = DateTime.Now  ' DTPTgl
    Private _lokasiBarang As String = ""             ' TOKO atau GUDANG

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION H: DATA SUPPLIER
    ' ═══════════════════════════════════════════════════════════════════
    Private _kodeSupplier As String = ""             ' ID supplier
    Private _namaSupplier As String = ""             ' Nama supplier

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION I: AKUN BAYAR
    ' ═══════════════════════════════════════════════════════════════════
    Private _namaAkunTunai As String = ""            ' Nama akun kas (CmbJenisBayarTunai)
    Private _kodeAkunTunai As String = ""            ' Kode akun kas (TxtJenisBayarTunai)
    Private _namaAkunTransfer As String = ""         ' Nama akun bank (CmbJenisBayarTransfer)
    Private _kodeAkunTransfer As String = ""         ' Kode akun bank (TxtJenisBayarTransfer)

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION J: STATUS TRANSAKSI
    ' ═══════════════════════════════════════════════════════════════════
    Private _statusPembayaran As String = "Lunas"    ' Lunas / Belum Lunas
    Private _jatuhTempo As DateTime = DateTime.Now   ' Tanggal jatuh tempo
    Private _idUser As String = ""                   ' ID user
    Private _idKomputer As String = ""               ' ID komputer

    ' ═══════════════════════════════════════════════════════════════════
    ' SECTION K: EXISTING (Tetap dipertahankan)
    ' ═══════════════════════════════════════════════════════════════════
    Private _isLoadingForm As Boolean = False

    ' Flag: True setelah Form_Shown selesai — cegah SetupFocusToGrid dipanggil saat form belum siap
    Private _formSudahSiap As Boolean = False

    ''' <summary>
    ''' Variabel untuk menyimpan total selisih nilai persediaan akibat perubahan harga pokok barang
    ''' Digunakan untuk mencatat jurnal penyesuaian harga pokok (Requirement 21)
    ''' </summary>
    Private _totalSelisihHargaPokok As Decimal = 0D

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: FUNGSI PERHITUNGAN KEUANGAN (SECTION A-F)
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI SECTION A: PERHITUNGAN PER BARIS DGV"

    ''' <summary>
    ''' Hitung total harga per baris: Totalharga = HargaBeli × Qty
    ''' Dipanggil saat Qty atau HargaBeli berubah
    ''' </summary>
    Private Sub HitungTotalhargaPerBaris(row As DataGridViewRow)
        Try
            If row Is Nothing OrElse row.IsNewRow Then Exit Sub

            _qtyPerBaris = ModuleAngka.ParseDecimal(row.Cells("Qty").Value)
            _hargaBeliPerBaris = ModuleAngka.ParseDecimal(row.Cells("HargaBeli").Value)

            _totalhargaPerBaris = _qtyPerBaris * _hargaBeliPerBaris
            row.Cells("Totalharga").Value = _totalhargaPerBaris

        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungTotalhargaPerBaris: " & ex.Message)
            ' Abaikan error parsing
        End Try
    End Sub

    ''' <summary>
    ''' Hitung qty dalam satuan terkecil: QtySat = Qty × Isi (konversi)
    ''' Dipanggil saat Qty atau Isi berubah
    ''' </summary>
    Private Sub HitungQtySatPerBaris(row As DataGridViewRow)
        Try
            If row Is Nothing OrElse row.IsNewRow Then Exit Sub

            _qtyPerBaris = ModuleAngka.ParseDecimal(row.Cells("Qty").Value)
            _isiPerBaris = ModuleAngka.ParseInteger(row.Cells("Isi").Value, defaultValue:=1)

            ' Handle Isi = 0 → 1
            _isiPerBaris = If(_isiPerBaris = 0, 1, _isiPerBaris)

            _qtySatPerBaris = _qtyPerBaris * _isiPerBaris
            row.Cells("QtySat").Value = _qtySatPerBaris

        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungQtySatPerBaris: " & ex.Message)
            ' Abaikan error parsing
        End Try
    End Sub

    ''' <summary>
    ''' Hitung harga satuan kecil: HargaSatuanKecil = HargaBeli / Isi
    ''' Digunakan untuk HargaBeliSatKecil (harga per pcs) dan Average (HPP)
    ''' Contoh: 1 dos = 20.000, Isi = 24 → HargaSatuanKecil = 833.33 per pcs
    ''' Dipanggil saat HargaBeli atau Isi berubah
    ''' </summary>
    Private Sub HitungHargaSatuanKecil(row As DataGridViewRow)
        Try
            If row Is Nothing OrElse row.IsNewRow Then Exit Sub

            _hargaBeliPerBaris = ModuleAngka.ParseDecimal(row.Cells("HargaBeli").Value)
            _isiPerBaris = ModuleAngka.ParseInteger(row.Cells("Isi").Value, defaultValue:=1)

            ' Handle Isi = 0 → 1
            _isiPerBaris = If(_isiPerBaris = 0, 1, _isiPerBaris)

            Dim hargaSatuanKecil As Decimal = _hargaBeliPerBaris / _isiPerBaris

            ' Assign ke kolom HargaBeliSatKecil (harga per pcs) saja
            ' Kolom Average TIDAK diubah di sini karena menyimpan HPP lama dari database untuk perhitungan average
            _hargaBeliSatPerBaris = hargaSatuanKecil
            row.Cells("HargaBeliSatKecil").Value = hargaSatuanKecil

        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungHargaSatuanKecil: " & ex.Message)
            ' Abaikan error parsing
        End Try
    End Sub

    ''' <summary>
    ''' Update semua perhitungan per baris (QtySat, HargaBeliSatKecil, Totalharga, Average)
    ''' Dipanggil saat baris DGV berubah
    ''' </summary>
    Private Sub UpdatePerhitunganPerBaris(row As DataGridViewRow)
        HitungQtySatPerBaris(row)
        HitungHargaSatuanKecil(row)
        HitungTotalhargaPerBaris(row)
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' FUNGSI SECTION B: PERHITUNGAN AGREGAT (Dari semua baris DGV)
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI SECTION B: PERHITUNGAN AGREGAT"

    ''' <summary>
    ''' Hitung subtotal barang: SUM(Totalharga) dari semua baris DGV
    ''' Dipanggil saat ada perubahan di DGV
    ''' </summary>
    Private Sub HitungSubtotalBarang()
        Try
            _subtotalBarang = 0D

            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso
                   Not String.IsNullOrEmpty(Convert.ToString(row.Cells("Id").Value)) AndAlso
                   row.Cells("Totalharga").Value IsNot Nothing Then
                    _subtotalBarang += ModuleAngka.ParseDecimal(row.Cells("Totalharga").Value)
                End If
            Next

        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungSubtotalBarang: " & ex.Message)
            ' Abaikan error parsing
        End Try
    End Sub

    ''' <summary>
    ''' Hitung total qty dalam satuan terkecil: SUM(QtySat) dari semua baris DGV
    ''' Dipanggil saat ada perubahan di DGV
    ''' </summary>
    Private Sub HitungTotalQtyTerkecil()
        Try
            _totalQtyDalamSatuanTerkecil = 0D

            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells("QtySat").Value IsNot Nothing Then
                    _totalQtyDalamSatuanTerkecil += ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
                End If
            Next

        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungTotalQtyTerkecil: " & ex.Message)
            ' Abaikan error parsing
        End Try
    End Sub

    ''' <summary>
    ''' Hitung jumlah jenis barang: COUNT baris DGV (bukan new row)
    ''' Dipanggil saat ada perubahan di DGV
    ''' </summary>
    Private Sub HitungJumlahJenisBarang()
        Try
            _jumlahJenisBarang = 0

            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() <> "" Then
                    _jumlahJenisBarang += 1
                End If
            Next

        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungJumlahJenisBarang: " & ex.Message)
            ' Abaikan error parsing
        End Try
    End Sub

    ''' <summary>
    ''' Update semua perhitungan agregat (subtotal, total qty, jumlah jenis)
    ''' Dipanggil saat ada perubahan di DGV
    ''' </summary>
    Private Sub UpdatePerhitunganAgregat()
        HitungSubtotalBarang()
        HitungTotalQtyTerkecil()
        HitungJumlahJenisBarang()
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' FUNGSI SECTION C: KOMPONEN TAMBAHAN (Input user)
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI SECTION C: KOMPONEN TAMBAHAN"

    ''' <summary>
    ''' Hitung biaya kirim dari input user
    ''' </summary>
    Private Sub HitungBiayaKirim()
        Try
            _biayaKirim = ModuleAngka.ParseDecimal(TxtBiayaKirim.Text)
        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungBiayaKirim: " & ex.Message)
            _biayaKirim = 0
        End Try
    End Sub

    ''' <summary>
    ''' Sync komponen tambahan dari UI ke class-level variables
    ''' </summary>
    Private Sub SyncKomponenTambahanDariUI()
        _diskonPersen = ModuleAngka.ParseDecimal(TxtDiskonPersen.Text)
        _diskonRupiah = ModuleAngka.ParseDecimal(TxtDiskonRupiah.Text)
        _ppnPersen = ModuleAngka.ParseDecimal(TxtPpnPersen.Text)
        _ppnRupiah = ModuleAngka.ParseDecimal(TxtPpnRupiah.Text)
        _biayaKirim = ModuleAngka.ParseDecimal(TxtBiayaKirim.Text)
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' FUNGSI SECTION D: GRAND TOTAL
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI SECTION D: GRAND TOTAL"

    ''' <summary>
    ''' Hitung grand total pembelian: Subtotal - Diskon + PPN + Biaya Kirim
    ''' Dipanggil saat ada perubahan di DGV atau komponen tambahan
    ''' </summary>
    Private Sub HitungGrandTotalPembelian()
        Try
            _grandTotalPembelian = _subtotalBarang - _diskonRupiah + _ppnRupiah + _biayaKirim

            ' Update UI
            TxtGrandTotalPembelian.Text = ModuleAngka.FormatUntukInput(_grandTotalPembelian)
            TxtGrandtotal.Text = ModuleAngka.FormatRupiah(_grandTotalPembelian)

        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungGrandTotalPembelian: " & ex.Message)
            ' Abaikan error parsing
        End Try
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' FUNGSI SECTION E: PEMBAYARAN
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI SECTION E: PEMBAYARAN"

    ''' <summary>
    ''' Hitung total bayar: Tunai + Transfer
    ''' Dipanggil saat ada perubahan di input pembayaran
    ''' </summary>
    Private Sub HitungTotalBayar()
        Try
            _bayarTunai = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
            _bayarTransfer = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
            _totalBayar = _bayarTunai + _bayarTransfer
        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungTotalBayar: " & ex.Message)
            _totalBayar = 0
        End Try
    End Sub

    ''' <summary>
    ''' Hitung sisa pembayaran: Grand Total - Total Bayar
    ''' Dipanggil saat ada perubahan di input pembayaran
    ''' </summary>
    Private Sub HitungSisaPembayaran()
        Try
            _sisaPembayaran = _grandTotalPembelian - _totalBayar
            _sisaPembayaranMutlak = Math.Abs(_sisaPembayaran)
        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungSisaPembayaran: " & ex.Message)
            _sisaPembayaran = 0
            _sisaPembayaranMutlak = 0
        End Try
    End Sub

    ''' <summary>
    ''' Hitung kembalian atau hutang berdasarkan sisa pembayaran
    ''' Jika sisa &lt; 0: Kembalian (bayar lebih)
    ''' Jika sisa &gt; 0: Hutang (bayar kurang)
    ''' </summary>
    Private Sub HitungKembalianAtauHutang()
        Try
            If _sisaPembayaran < 0 Then
                ' Bayar lebih dari grand total
                _kembalian = Math.Abs(_sisaPembayaran)
                _sisaHutang = 0
                _statusPembayaran = "Lunas"
            ElseIf _sisaPembayaran > 0 Then
                ' Bayar kurang dari grand total
                _kembalian = 0
                _sisaHutang = _sisaPembayaran
                _statusPembayaran = "Belum Lunas"
            Else
                ' Bayar pas
                _kembalian = 0
                _sisaHutang = 0
                _statusPembayaran = "Lunas"
            End If
        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungKembalianAtauHutang: " & ex.Message)
            _kembalian = 0
            _sisaHutang = 0
            _statusPembayaran = "Belum Lunas"
        End Try
    End Sub

    ''' <summary>
    ''' Update label pembayaran di UI
    ''' </summary>
    Private Sub UpdateLabelPembayaran()
        TxtKembaliHutang.Text = ModuleAngka.FormatUntukInput(If(_kembalian > 0, _kembalian, _sisaHutang))
        LblStatusPembayaran.Text = _statusPembayaran

        ' Update label pembayaran berdasarkan kondisi kembalian/hutang
        If _kembalian > 0 Then
            LblPembayaran.Text = "Kembalian :"
        ElseIf _sisaHutang > 0 Then
            LblPembayaran.Text = "Hutang :"
        Else
            LblPembayaran.Text = "Kembalian :"
        End If
    End Sub

    ''' <summary>
    ''' Orkestrasi semua perhitungan pembayaran
    ''' Dipanggil saat ada perubahan di input pembayaran
    ''' </summary>
    Private Sub HitungPembayaran()
        HitungTotalBayar()
        HitungSisaPembayaran()
        HitungKembalianAtauHutang()
        UpdateLabelPembayaran()
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' FUNGSI SECTION F: JURNAL
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI SECTION F: JURNAL"

    ''' <summary>
    ''' Hitung kas tunai keluar untuk jurnal
    ''' Kas tunai keluar = Bayar Tunai
    ''' </summary>
    Private Sub HitungKasTunaiKeluar()
        _kasTunaiKeluar = _bayarTunai
    End Sub

    ''' <summary>
    ''' Hitung total debet jurnal
    ''' Debet = Subtotal - Diskon + PPN + Biaya Kirim
    ''' </summary>
    Private Sub HitungTotalDebet()
        _debetJurnal = _subtotalBarang - _diskonRupiah + _ppnRupiah + _biayaKirim
    End Sub

    ''' <summary>
    ''' Hitung total kredit jurnal
    ''' Kredit = Kas Tunai + Transfer + Hutang
    ''' </summary>
    Private Sub HitungTotalKredit()
        _kreditJurnal = _bayarTunai + _bayarTransfer + _sisaHutang
    End Sub

    ''' <summary>
    ''' Validasi keseimbangan jurnal (Debet = Kredit)
    ''' </summary>
    Private Function ValidasiKeseimbanganJurnal() As Boolean
        Return Math.Abs(_debetJurnal - _kreditJurnal) < 0.01 ' Toleransi 0.01
    End Function

    ''' <summary>
    ''' Orkestrasi semua perhitungan jurnal
    ''' Dipanggil saat ada perubahan yang mempengaruhi jurnal
    ''' </summary>
    Private Sub UpdatePerhitunganJurnal()
        HitungKasTunaiKeluar()
        HitungTotalDebet()
        HitungTotalKredit()
    End Sub

    ''' <summary>
    ''' Debug perhitungan jurnal (untuk development)
    ''' </summary>
    Private Sub DebugPerhitunganJurnal()
        ' Placeholder untuk debugging
        ' Bisa ditambahkan log ke file atau debug TextBox
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' FUNGSI MASTER: UPDATE SEMUA TOTAL
    ' ═══════════════════════════════════════════════════════════════════
#Region "FUNGSI MASTER: UPDATE SEMUA TOTAL"

    ''' <summary>
    ''' Master function untuk menghitung semua total dari DGV ke UI
    ''' Dipanggil saat ada perubahan di DGV
    ''' Urutan: Per Baris → Agregat → Komponen Tambahan → Grand Total → Pembayaran → Jurnal
    ''' </summary>
    Private Sub UpdateSemuaTotal()
        ' Step 1: Update perhitungan per baris untuk semua baris
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow Then
                UpdatePerhitunganPerBaris(row)
            End If
        Next

        ' Step 2: Update perhitungan agregat
        UpdatePerhitunganAgregat()

        ' Step 3: Sync komponen tambahan dari UI
        SyncKomponenTambahanDariUI()

        ' Step 4: Hitung grand total
        HitungGrandTotalPembelian()

        ' Step 5: Hitung pembayaran (HitungTotalBayar di dalamnya sudah sync dari UI)
        HitungPembayaran()

        ' Step 6: Update perhitungan jurnal
        UpdatePerhitunganJurnal()

        ' Step 7: Update UI hasil perhitungan
        UpdateUIHasilPerhitungan()
    End Sub

    ''' <summary>
    ''' Master function untuk menghitung semua total dari class-level variables saja
    ''' Dipanggil saat hanya perlu refresh UI tanpa membaca dari DGV
    ''' </summary>
    Private Sub UpdateSemuaTotalDariVariabel()
        ' Hitung grand total dari variabel
        HitungGrandTotalPembelian()

        ' Hitung pembayaran dari variabel
        HitungPembayaran()

        ' Hitung jurnal dari variabel
        UpdatePerhitunganJurnal()

        ' Update UI
        UpdateUIHasilPerhitungan()
    End Sub

    ''' <summary>
    ''' Update UI dari class-level variables
    ''' </summary>
    Private Sub UpdateUIHasilPerhitungan()
        ' Update komponen tambahan
        TxtDiskonPersen.Text = ModuleAngka.FormatUntukInput(_diskonPersen)
        TxtDiskonRupiah.Text = ModuleAngka.FormatUntukInput(_diskonRupiah)
        TxtPpnPersen.Text = ModuleAngka.FormatUntukInput(_ppnPersen)
        TxtPpnRupiah.Text = ModuleAngka.FormatUntukInput(_ppnRupiah)
        TxtBiayaKirim.Text = ModuleAngka.FormatUntukInput(_biayaKirim)

        ' Update agregat
        TxtSubtotalBarang.Text = ModuleAngka.FormatUntukInput(_subtotalBarang)
        TxtTotalQtyTerkecil.Text = ModuleAngka.FormatUntukInput(_totalQtyDalamSatuanTerkecil)
        TxtTotalHargaPembelian.Text = ModuleAngka.FormatUntukInput(_grandTotalPembelian)
        TxtTotalBayar.Text = ModuleAngka.FormatUntukInput(_totalBayar)

        ' Update grand total
        TxtGrandTotalPembelian.Text = ModuleAngka.FormatUntukInput(_grandTotalPembelian)
        TxtGrandtotal.Text = ModuleAngka.FormatRupiah(_grandTotalPembelian)

        ' Update pembayaran
        TxtNominalBayarTunai.Text = ModuleAngka.FormatUntukInput(_bayarTunai)
        TxtNominalBayarTransfer.Text = ModuleAngka.FormatUntukInput(_bayarTransfer)
        TxtKembaliHutang.Text = ModuleAngka.FormatUntukInput(If(_kembalian > 0, _kembalian, _sisaHutang))
        LblStatusPembayaran.Text = _statusPembayaran
        ' Update label display langsung — tidak bergantung pada TextChanged
        ' (TextChanged punya guard GBBayar.Visible yang bisa memblokir update label)
        LblBayarTunai.Text = "Rp. " & ModuleAngka.FormatRupiah(_bayarTunai)
        LblBayarTransfer.Text = "Rp. " & ModuleAngka.FormatRupiah(_bayarTransfer)
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: FORM LOAD & INITIALIZE
    ' ═══════════════════════════════════════════════════════════════════
#Region "FORM LOAD & INITIALIZE"

    Private ReadOnly tempatSimpan As String
    Private draftPembelianAktif As String = ""


    Private ReadOnly Property IsModeTambahPembelian As Boolean
        Get
            Return String.Equals(TxtJenisTrans.Text, "TambahPembelian", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    ' ── Tangkap keyboard saat ListBox aktif — Enter/Escape dari luar ──────
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
            Select Case keyData
                Case Keys.Down
                    ' Jika ListBox sudah fokus → biarkan Down diteruskan ke ListBox untuk navigasi normal
                    If LstBarang.Focused Then
                        Return MyBase.ProcessCmdKey(msg, keyData)
                    End If
                    ' Simpan teks sebelum pindah agar bisa di-restore saat user tekan Up
                    If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
                        _teksSebelumPindahKeLstBarang = _dgvEditingTextBox.Text
                    Else
                        _teksSebelumPindahKeLstBarang = TxtNama.Text
                    End If
                    ' Masalah: LstBarang.Focus() tidak langsung berhasil karena DGV BeginEdit ulang
                    ' setelah Focus() merebut kembali fokus ke DGV.
                    ' Solusi: nested BeginInvoke — lapis pertama menunggu CellLeave+EditingControlShowing selesai,
                    ' lapis kedua baru panggil Focus() setelah DGV benar-benar selesai BeginEdit ulang.
                    _sedangPindahKeLstBarang = True
                    If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                    Me.BeginInvoke(New Action(Sub()
                                                  Me.BeginInvoke(New Action(Sub()
                                                                                ' EndEdit dulu agar DGV tidak merebut fokus kembali.
                                                                                ' _sedangSetNilaiDariListBox = True agar CellEndEdit tidak memproses teks keyword.
                                                                                If LstBarang.Visible Then
                                                                                    _sedangSetNilaiDariListBox = True
                                                                                    DgvData.EndEdit()
                                                                                    _sedangSetNilaiDariListBox = False
                                                                                    LstBarang.Focus()
                                                                                End If
                                                                                _sedangPindahKeLstBarang = False
                                                                            End Sub))
                                              End Sub))
                    Return True

                Case Keys.Enter
                    ' Enter saat ListBox visible → pilih item yang ter-highlight
                    If LstBarang.SelectedIndex < 0 Then LstBarang.SelectedIndex = 0
                    _sedangPindahKeLstBarang = True
                    AmbilDataDariListBox()
                    _sedangPindahKeLstBarang = False
                    Return True

                Case Keys.Escape
                    TutupListBox()
                    If _konteksLstBarang = "DGV" AndAlso _dgvEditingTextBox IsNot Nothing Then
                        _dgvEditingTextBox.Focus()
                    Else
                        TxtNama.Focus()
                    End If
                    Return True
            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: SUPPLIER
    ' ═══════════════════════════════════════════════════════════════════
#Region "SUPPLIER"

    ' Handler untuk event GotFocus pada TextBox
    Private Sub TxtNama_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        ' Warna fokus ikut tema — konsisten dengan FormJual
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_SearchFocusBg, ModuleTheme.D_SearchFocusBg)

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
        ' Kembalikan ke warna panel sesuai tema — konsisten dengan FormJual
        PanelCari.BackColor = ModuleTheme.C(ModuleTheme.L_Panel, ModuleTheme.D_Panel)
    End Sub

    Private Sub KosongTxtboxcari()
        TxtKode.Clear()
        TxtQty.Clear()
        Txtsatuan.Clear()
        TxtIsi.Clear()
        TxtHarga.Clear()
        TxtBarcode.Clear()
        TxtNama.Clear()
        TxtLevelSat.Clear()
        TxtStokToko.Clear()
        TXtStokGudang.Clear()
    End Sub

    Private Sub Kondisiawaledit()
        GBBayar.Visible = False
        BtnTahan.Visible = False
        BtnPanggil.Visible = False

        DtpTanggalPembelian.Format = DateTimePickerFormat.Custom
        DtpTanggalPembelian.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        DtpJatuhTempo.Format = DateTimePickerFormat.Custom
        DtpJatuhTempo.CustomFormat = "dd/MM/yyyy"

        ' ✅ Reset semua field pembayaran agar tidak ada nilai stale
        TxtNominalBayarTunai.Text = "0"
        TxtKembaliHutang.Text = "0"


        ' ✅ Reset total selisih harga pokok (Requirement 21)
        _totalSelisihHargaPokok = 0D
    End Sub

    Private Sub Kondisiawal()
        DgvData.Rows.Clear()
        TxtNamaSupplier.Clear()
        TxtNotaPembelian.Clear()
        TxtNominalBayarTunai.Text = "0"
        TxtNominalBayarTransfer.Text = "0"
        TxtKembaliHutang.Text = "0"
        TxtGrandTotalPembelian.Text = "0"
        TxtSubtotalBarang.Clear()

        ' Set lebar default GBBayar untuk mode tambah
        GBBayar.Size = New Size(509, 344)
        TxtTotalQtyTerkecil.Text = "0"
        TxtGrandtotal.Text = "Rp. 0"
        LblRecord.Text = "Total record : 0"
        LblStatusPembayaran.Text = "Lunas"
        LblPembayaran.Text = "Kembalian :"
        LblJatuhTempo.Visible = False
        DtpJatuhTempo.Visible = False
        GBBayar.Visible = False

        ' ✅ Reset total selisih harga pokok (Requirement 21)
        _totalSelisihHargaPokok = 0D

        ' Reset diskon, PPN, biaya kirim
        TxtDiskonPersen.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtDiskonRupiah.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPpnPersen.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtPpnRupiah.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        TxtBiayaKirim.Text = 0.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
        LblDiskonRupiah.Text = "Rp. 0"
        LblPpnRupiah.Text = "Rp. 0"
        LblBiayaKirim.Text = "Rp. 0"

        DtpTanggalPembelian.Value = DateTime.Now
        DtpTanggalPembelian.Format = DateTimePickerFormat.Custom
        DtpTanggalPembelian.CustomFormat = "dd/MM/yyyy HH:mm:ss"
        DtpTanggalPembelian.Enabled = True  ' Mode edit: selalu bisa ubah tanggal, tanggal lama bisa lampau

        Dim newDate As Date = DtpTanggalPembelian.Value.AddMonths(1)
        DtpJatuhTempo.Value = newDate
        DtpJatuhTempo.Format = DateTimePickerFormat.Custom
        DtpJatuhTempo.CustomFormat = "dd/MM/yyyy"

        NomorBeli()
        AmbilDataSupplier()
        draftPembelianAktif = ""
        BtnTahan.Visible = True
        BtnPanggil.Visible = True

        ' Hitung ulang semua total setelah reset — pastikan UI konsisten
        UpdateSemuaTotal()

        ' SetupFocusToGrid TIDAK dipanggil di sini — dipanggil dari Form_Shown setelah _formSudahSiap = True
        ' Memanggil dari sini menyebabkan guard _formSudahSiap memblokir fokus (masih False saat Shown berjalan)
    End Sub

    ''' <summary>
    ''' Form Load — setup satu kali saat form pertama dibuat.
    ''' Dipisah dari Form_Shown agar tidak diulang setiap kali form ditampilkan kembali.
    ''' </summary>
    Private Sub Form_Pembelian_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Terapkan tema warna otomatis sesuai kategori form
            ModuleTheme.TerapkanTheme(Me)

            ' Lokasi barang dari FormUtama
            LblLokasiBarang.Text = FormUtama.StatusLokasi.Text

            ' Ukuran Form — kunci agar tidak bisa di-resize melebihi layar
            MaximumSize = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
            MinimumSize = Size

            ' Setup timer barcode dan search — WAJIB dipasang di Load agar aktif sebelum Shown
            barcodeTimer.Interval = 100
            AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick

            ' Kosongkan textbox pencarian saat form pertama dibuka
            KosongTxtboxcari()

            ' Tampilkan metode HPP di label form utama
            LblUpdateHarga.Text = "Metode HPP : " & ModulHakAkses.SettingMetodeUpdateHargaBeli

            ' Setup DGV
            DgvData.EnableHeadersVisualStyles = False
            DgvData.RowHeadersVisible = True

            ' Atur kolom DGV berdasarkan hak akses
            If Not ModulHakAkses.SettingIzinkanUbahHargaBeli Then
                DgvData.Columns("Hargabeli").ReadOnly = True
            Else
                DgvData.Columns("Hargabeli").ReadOnly = False
            End If

            If ModulHakAkses.SettingTampilInfoStok Then
                DgvData.Columns("StokToko").Visible = True
                DgvData.Columns("StokGudang").Visible = True
            Else
                DgvData.Columns("StokToko").Visible = False
                DgvData.Columns("StokGudang").Visible = False
            End If

            ' Format kolom angka DGV
            ModuleAngka.TerapkanFormatKolomAngka(DgvData,
                "Qty", "Isi", "Hargabeli", "HargaBeliSatKecil", "QtySat",
                "Totalharga", "Average", "HargaSebelumnya", "StokToko", "StokGudang")

        Catch ex As Exception
            MessageBox.Show("Error Load: " & ex.Message)
        End Try
    End Sub

    Private Sub Form_Pembelian_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Try
            ' Reset flag — form sedang dalam proses inisialisasi
            _formSudahSiap = False
            _isLoadingForm = True

            ' [FP1-T05-FIX] TAMBAH: Inisialisasi awal form yang terhapus dari ProcessCmdKey
            listSupplier.Visible = False

            ' Isi combobox akun
            IsiComboBoxAkun(CmbAkunTunai, "KAS", "EKUITAS")
            IsiComboBoxAkun(CmbAkunTransfer, "BANK")

            ' Sembunyikan panel pencarian jika setting diaktifkan
            If ModulHakAkses.SettingSembunyikanPencarianAtas Then
                PanelCari.Visible = False
            Else
                PanelCari.Visible = True
            End If

            ' Paksa maximize — WindowState di designer diabaikan saat ShowDialog pada singleton form
            If Me.WindowState <> FormWindowState.Maximized Then
                Me.WindowState = FormWindowState.Maximized
            End If

            ' Paksa refresh TextAlign TxtGrandtotal — saat InitializeComponent form masih kecil
            ' (100x20) sehingga Windows fallback ke Left dan meng-cache posisi teks.
            ' Setelah maximize ukuran sudah benar, reset TextAlign agar Windows recalculate.
            TxtGrandtotal.TextAlign = HorizontalAlignment.Left
            TxtGrandtotal.TextAlign = HorizontalAlignment.Right

            If IsModeTambahPembelian Then
                ' Mode tambah: setup akun default + reset form
                TxtNamaSupplier.Clear()
                KosongkanDataSupplier()
                If LblLokasiBarang.Text = "TOKO" Then
                    CmbAkunTunai.SelectedItem = nama_rek_Beli_toko
                ElseIf LblLokasiBarang.Text = "GUDANG" Then
                    CmbAkunTunai.SelectedItem = nama_rek_Beli_Gudang
                End If
                If CmbAkunTransfer.Items.Count > 0 Then CmbAkunTransfer.SelectedIndex = 0
                AmbilKodeAkun()
                JumlahTahanPembelian()
                Kondisiawal()
            Else
                ' Mode edit: load data faktur yang akan diedit
                Kondisiawaledit()
                AmbilDataPembelian()
                AmbilDaftarBarangEditpembelian()
                BtnTahan.Visible = False
                BtnPanggil.Visible = False
                JumlahTahanPembelian()
            End If

            ' Form sudah selesai render dan data sudah dimuat — aktifkan SetupFocusToGrid
            _isLoadingForm = False
            _formSudahSiap = True

            ' Panggil SetupFocusToGrid di sini — setelah _formSudahSiap = True
            ' Ini pola yang sama dengan FormJual (tidak ada guard _formSudahSiap di FormJual
            ' karena FormJual memanggil SetupFocusToGrid langsung dari Kondisiawal tanpa guard)
            SetupFocusToGrid()

        Catch ex As Exception
            Debug.WriteLine("[ERROR] Shown: " & ex.Message)
            MessageBox.Show("Error Shown: " & ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Mengatur fokus ke Grid atau TxtNama berdasarkan pengaturan.
    ''' Disamakan dengan FormJual: cari baris kosong SETELAH baris terakhir yang terisi.
    ''' </summary>
    Public Sub SetupFocusToGrid()
        Debug.WriteLine("[SetupFocus] ══════════════════════════════════════════")
        Debug.WriteLine("[SetupFocus] DIPANGGIL dari: " & New System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name)

        ' Guard: jika form tidak aktif atau visible, jangan paksa fokus
        If Not Me.Visible OrElse Me.WindowState = FormWindowState.Minimized Then
            Debug.WriteLine("[SetupFocus] KELUAR — form tidak visible atau minimized")
            Return
        End If

        ' Guard: jangan paksa fokus sebelum Form_Shown selesai
        ' ⚠️ PERBEDAAN vs FormJual: FormJual tidak punya guard ini
        ' Jika _formSudahSiap = False saat dipanggil → fokus tidak akan pernah terjadi
        If Not _formSudahSiap Then
            Debug.WriteLine("[SetupFocus] KELUAR — _formSudahSiap = False (FormJual tidak punya guard ini!)")
            Return
        End If

        ' MODE 1: Setting Fokus Otomatis (ke TxtNama)
        ' ⚠️ PERBEDAAN vs FormJual: FormJual pakai TxtNama.Focus(), FormPembelian pakai TxtNama.Select()
        If ModulHakAkses.SettingFokusOtomatis Then
            Debug.WriteLine("[SetupFocus] MODE 1 — SettingFokusOtomatis=True → TxtNama.Select()")
            TxtNama.Select()
            Return
        End If

        Debug.WriteLine("[SetupFocus] MODE 2 — Edit Langsung ke Grid")
        Debug.WriteLine("[SetupFocus] DgvData.Rows.Count = " & DgvData.Rows.Count)

        ' MODE 2: Edit Langsung (ke Grid)
        If DgvData.Rows.Count = 0 Then
            Debug.WriteLine("[SetupFocus] KELUAR — DgvData.Rows.Count = 0")
            Return
        End If

        ' Cari baris kosong SETELAH baris terakhir yang terisi
        Dim targetRow As Integer = 0
        Dim lastFilledRow As Integer = -1

        ' Cari baris terakhir yang terisi (ada Id)
        ' ⚠️ PERBEDAAN vs FormJual: FormJual cek kolom "Kode", FormPembelian cek kolom "Id"
        For i As Integer = DgvData.Rows.Count - 1 To 0 Step -1
            If Not DgvData.Rows(i).IsNewRow Then
                Dim idVal = Convert.ToString(DgvData.Rows(i).Cells("Id").Value).Trim()
                If Not String.IsNullOrEmpty(idVal) Then
                    lastFilledRow = i
                    Exit For
                End If
            End If
        Next

        Debug.WriteLine("[SetupFocus] lastFilledRow = " & lastFilledRow)

        ' Cari baris kosong setelah baris terakhir yang terisi
        If lastFilledRow >= 0 Then
            ' Ada baris terisi, cari baris kosong setelahnya
            Dim foundEmptyRow As Boolean = False
            For i As Integer = lastFilledRow + 1 To DgvData.Rows.Count - 1
                If Not DgvData.Rows(i).IsNewRow Then
                    Dim idVal = Convert.ToString(DgvData.Rows(i).Cells("Id").Value).Trim()
                    If String.IsNullOrEmpty(idVal) Then
                        targetRow = i
                        foundEmptyRow = True
                        Exit For
                    End If
                End If
            Next

            ' Jika tidak ada baris kosong non-IsNewRow, cek IsNewRow — jangan Rows.Add() yang buat baris ekstra
            If Not foundEmptyRow Then
                Dim isNewRowIdx As Integer = -1
                For i As Integer = lastFilledRow + 1 To DgvData.Rows.Count - 1
                    If DgvData.Rows(i).IsNewRow Then
                        isNewRowIdx = i
                        Exit For
                    End If
                Next
                If isNewRowIdx >= 0 Then
                    targetRow = isNewRowIdx
                    Debug.WriteLine("[SetupFocus] Tidak ada baris kosong non-IsNewRow → pakai IsNewRow index " & isNewRowIdx)
                Else
                    ' Tidak ada IsNewRow, jangan paksa add baris baru
                    ' pakai baris aktif jika ada, atau keluar
                    If DgvData.CurrentCell IsNot Nothing Then
                        targetRow = DgvData.CurrentCell.RowIndex
                        Debug.WriteLine("[SetupFocus] Tidak ada IsNewRow → pakai CurrentCell.RowIndex = " & targetRow)
                    Else
                        Debug.WriteLine("[SetupFocus] KELUAR — tidak ada IsNewRow dan CurrentCell = Nothing")
                        Exit Sub
                    End If
                End If
            Else
                Debug.WriteLine("[SetupFocus] Baris kosong non-IsNewRow ditemukan di index " & targetRow)
            End If
        Else
            ' Tidak ada baris terisi, gunakan baris pertama
            targetRow = 0
            Debug.WriteLine("[SetupFocus] Tidak ada baris terisi → targetRow = 0")
        End If

        Debug.WriteLine("[SetupFocus] targetRow = " & targetRow & " | DgvData.Rows.Count = " & DgvData.Rows.Count)

        ' Set CurrentCell dan fokus ke DGV
        ' ⚠️ PERBEDAAN vs FormJual:
        '    FormJual   → 1 lapis BeginInvoke + race condition guard (cek ColumnIndex & RowIndex) + EditingControl?.Focus()
        '    FormPembelian → 2 lapis BeginInvoke tanpa race condition guard + EditingControl?.Select()
        '    Select() tidak selalu memindahkan fokus OS ke kontrol — Focus() lebih andal
        If targetRow < DgvData.Rows.Count Then
            ' Simpan target sebelum BeginInvoke (race condition guard — sama dengan FormJual)
            Dim targetColumnIndex As Integer = 1 ' Kolom Nama (index 1 di FormPembelian)
            Dim targetRowIndex As Integer = targetRow

            Debug.WriteLine("[SetupFocus] Set CurrentCell → kolom " & targetColumnIndex & ", baris " & targetRowIndex)
            Debug.WriteLine("[SetupFocus] Nama kolom index 1 = '" & DgvData.Columns(1).Name & "' (FormJual='Kode', FormPembelian='Nama')")
            Debug.WriteLine("[SetupFocus] EditMode DGV = " & DgvData.EditMode.ToString())

            DgvData.CurrentCell = DgvData(targetColumnIndex, targetRowIndex)
            Me.ActiveControl = DgvData

            Debug.WriteLine("[SetupFocus] CurrentCell set → masuk BeginInvoke lapis 1")

            ' Nested BeginInvoke untuk memastikan form sudah siap sebelum BeginEdit
            DgvData.BeginInvoke(New Action(Sub()
                                               Debug.WriteLine("[SetupFocus] BeginInvoke lapis 1 dieksekusi → masuk lapis 2")
                                               DgvData.BeginInvoke(New Action(Sub()
                                                                                  Debug.WriteLine("[SetupFocus] BeginInvoke lapis 2 dieksekusi")
                                                                                  Debug.WriteLine("[SetupFocus] CurrentCell = " & If(DgvData.CurrentCell Is Nothing, "Nothing", "baris " & DgvData.CurrentCell.RowIndex & " kolom " & DgvData.CurrentCell.ColumnIndex))
                                                                                  If DgvData.CurrentCell IsNot Nothing Then
                                                                                      ' Cek race condition — apakah CurrentCell masih di target?
                                                                                      If DgvData.CurrentCell.ColumnIndex <> targetColumnIndex OrElse DgvData.CurrentCell.RowIndex <> targetRowIndex Then
                                                                                          Debug.WriteLine("[SetupFocus] ⚠️ RACE CONDITION — CurrentCell bergeser! Target=" & targetRowIndex & "," & targetColumnIndex & " Aktual=" & DgvData.CurrentCell.RowIndex & "," & DgvData.CurrentCell.ColumnIndex)
                                                                                      End If
                                                                                      Dim hasilBeginEdit As Boolean = DgvData.BeginEdit(True)
                                                                                      Debug.WriteLine("[SetupFocus] BeginEdit(True) → hasil = " & hasilBeginEdit)
                                                                                      Debug.WriteLine("[SetupFocus] EditingControl = " & If(DgvData.EditingControl Is Nothing, "Nothing ← MASALAH!", DgvData.EditingControl.GetType().Name))
                                                                                      If DgvData.EditingControl IsNot Nothing Then
                                                                                          ' ⚠️ PERBEDAAN: FormJual pakai .Focus(), FormPembelian pakai .Select()
                                                                                          ' .Select() = pilih semua teks, tidak selalu pindah fokus OS
                                                                                          ' .Focus()  = pindah fokus OS ke kontrol — lebih andal
                                                                                          DgvData.EditingControl.Select()
                                                                                          Debug.WriteLine("[SetupFocus] EditingControl.Select() dipanggil")
                                                                                          Debug.WriteLine("[SetupFocus] EditingControl.Focused = " & DgvData.EditingControl.Focused)
                                                                                      End If
                                                                                  Else
                                                                                      Debug.WriteLine("[SetupFocus] ⚠️ CurrentCell = Nothing di dalam BeginInvoke lapis 2 — BeginEdit tidak dipanggil")
                                                                                  End If
                                                                                  Debug.WriteLine("[SetupFocus] ══ SELESAI ══════════════════════════════════")
                                                                              End Sub))
                                           End Sub))
        Else
            Debug.WriteLine("[SetupFocus] KELUAR — targetRow (" & targetRow & ") >= DgvData.Rows.Count (" & DgvData.Rows.Count & ")")
        End If
    End Sub

    Private SkipValidation As Boolean = False




    Public Sub NomorBeli()
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "PB")
            cmd.Parameters.AddWithValue("@tgl", DtpTanggalPembelian.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "pembelian")
            cmd.Parameters.AddWithValue("@kolom", "ID_PEMBELIAN")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            TxtIdPembelian.Text = pNomor.Value?.ToString()
        End Using
    End Sub

    Private Sub Hapusbaris()
        ' Periksa apakah ada sel yang dipilih
        If DgvData.CurrentCell Is Nothing Then
            MessageBox.Show("Tidak ada baris yang dipilih.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        Dim baris As Integer = DgvData.CurrentCell.RowIndex

        ' Periksa apakah baris yang dipilih adalah baris baru
        If DgvData.Rows(baris).IsNewRow Then
            MessageBox.Show("Baris baru tidak dapat dihapus.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        ' Periksa apakah sel dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            MessageBox.Show("Tidak dapat menghapus baris dalam mode edit.", "Mode Edit Aktif", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Konfirmasi penghapusan
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus baris ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            ' Hapus baris jika pengguna menekan Yes
            DgvData.Rows.RemoveAt(baris)
            UpdateSemuaTotal()
            ' [FP1-T14b-3] TAMBAH: SetupFocusToGrid setelah hapus baris untuk UX keyboard
            SetupFocusToGrid()
        End If
    End Sub

    Private Sub KosongkanDataSupplier()
        LblKodeSupplier.Text = ""
        LblAlamatSupplier.Text = ""
        LblKontakSupplier.Text = ""
        DtpJatuhTempo.Value = DtpTanggalPembelian.Value.AddMonths(1)
    End Sub


    Public Class DataSupplier
        Public Property Kode As String
        Public Property Nama As String
        Public Property Alamat As String
        Public Property HP As String
        Public Property JangkaHutang As Integer

        Public Overrides Function ToString() As String
            Return Nama   ' <<< LISTBOX TAMPIL NAMA SAJA
        End Function
    End Class

    Dim IsSelectingSupplier As Boolean = False

    Dim ListDataSupplier As New List(Of DataSupplier)

    Public Sub AmbilDataSupplier()
        ListDataSupplier.Clear()

        Using cmd As New MySqlCommand("SELECT KODE, NAMA, ALAMAT, HP, JangkaHutang FROM tbl_supliyer WHERE Status = 'Aktif' ORDER BY NAMA", conn)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    Dim jangka As Integer = ModuleAngka.SafeGetValue(Of Integer)(rd, "JangkaHutang", 30)
                    If jangka <= 0 Then jangka = 30
                    ListDataSupplier.Add(New DataSupplier With {
                    .Kode = ModuleAngka.SafeGetValue(Of String)(rd, "KODE", ""),
                    .Nama = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA", ""),
                    .Alamat = ModuleAngka.SafeGetValue(Of String)(rd, "ALAMAT", ""),
                    .HP = ModuleAngka.SafeGetValue(Of String)(rd, "HP", ""),
                    .JangkaHutang = jangka
                })
                End While
            End Using
        End Using
    End Sub

    Private Sub FilterSupplier()
        If IsSelectingSupplier Then Exit Sub   ' <<< stop loop

        Dim filter As String = TxtNamaSupplier.Text.Trim().ToLower()

        listSupplier.Items.Clear()

        ' Jika textbox kosong → semua data supplier hilang
        If filter = "" Then
            listSupplier.Visible = False
            KosongkanDataSupplier()
            Exit Sub
        End If

        Dim hasil = ListDataSupplier.
            Where(Function(x) x.Nama.ToLower().Contains(filter) _
                          Or x.HP.ToLower().Contains(filter)).
            ToList()

        If hasil.Count = 0 Then
            listSupplier.Visible = False
            Exit Sub
        End If

        If Not IsModeTambahPembelian Then
            If hasil.Count = 1 Then
                PilihSupplierLangsung(hasil(0), False)   ' ← tetap di txtSupplier
                Exit Sub
            End If
        End If


        For Each s In hasil
            listSupplier.Items.Add(s)
        Next
        AturTinggiListSupplier()
        ' Pastikan tetap di depan setelah ditampilkan
        listSupplier.BringToFront()
        listSupplier.Visible = True

    End Sub

    ''' <summary>
    ''' Tampilkan semua supplier di listSupplier (tanpa filter).
    ''' Dipanggil saat user belum memilih supplier di mode tambah.
    ''' Visibility dihandle oleh FilterSupplier (hanya jika TxtSupplier tidak kosong)
    ''' </summary>
    Private Sub TampilkanSemuaSupplier()
        If IsSelectingSupplier Then Exit Sub

        listSupplier.Items.Clear()

        ' Tampilkan semua supplier dari ListDataSupplier
        For Each s In ListDataSupplier
            listSupplier.Items.Add(s)
        Next

        AturTinggiListSupplier()
        listSupplier.BringToFront()
        ' Visible dihandle oleh FilterSupplier setelah TextChanged trigger
    End Sub

    Private Sub AturTinggiListSupplier()
        Dim baris As Integer = listSupplier.Items.Count

        If baris = 0 Then
            listSupplier.Height = 0
            Return
        End If

        Dim tinggiBaris As Integer = listSupplier.ItemHeight

        If baris <= 20 Then
            listSupplier.Height = baris * tinggiBaris + 4
            listSupplier.ScrollAlwaysVisible = False
        Else
            listSupplier.Height = 20 * tinggiBaris + 4
            listSupplier.ScrollAlwaysVisible = True
        End If
    End Sub


    Private Sub PilihSupplierLangsung(s As DataSupplier, Optional PindahKeBarang As Boolean = False)
        IsSelectingSupplier = True

        TxtNamaSupplier.Text = s.Nama
        LblKodeSupplier.Text = s.Kode
        LblAlamatSupplier.Text = s.Alamat
        LblKontakSupplier.Text = s.HP
        DtpJatuhTempo.Value = DtpTanggalPembelian.Value.AddDays(s.JangkaHutang)

        listSupplier.Items.Clear()
        listSupplier.Visible = False

        TxtNamaSupplier.Select()
        TxtNamaSupplier.SelectionStart = TxtNamaSupplier.Text.Length

        IsSelectingSupplier = False

        If PindahKeBarang Then SetupFocusToGrid()   ' ← Ringkas & efisien
    End Sub


    Private Sub PilihSupplier()
        If listSupplier.SelectedItem Is Nothing Then Exit Sub

        Dim s As DataSupplier = CType(listSupplier.SelectedItem, DataSupplier)

        PilihSupplierLangsung(s)
    End Sub

    Private Sub TxtNamaSupplier_TextChanged(sender As Object, e As EventArgs) Handles TxtNamaSupplier.TextChanged
        ' Cegah listbox tampil saat form load
        If _isLoadingForm Then Return
        FilterSupplier()
    End Sub

    Private Sub TxtNamaSupplier_GotFocus(sender As Object, e As EventArgs) Handles TxtNamaSupplier.GotFocus
    End Sub

    Private Sub TxtNamaSupplier_LostFocus(sender As Object, e As EventArgs) Handles TxtNamaSupplier.LostFocus
    End Sub

    Private Sub TxtNamaSupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtNamaSupplier.KeyDown
        ' Tekan delete → hapus semua data supplier
        If e.KeyCode = Keys.Delete Then
            TxtNamaSupplier.Clear()
            KosongkanDataSupplier()
            listSupplier.Visible = False
            Exit Sub
        End If

        ' Pattern autocomplete: Up/Down hanya ubah SelectedIndex, fokus tetap di TextBox
        If listSupplier.Visible AndAlso listSupplier.Items.Count > 0 Then
            Select Case e.KeyCode
                Case Keys.Down
                    If listSupplier.SelectedIndex < listSupplier.Items.Count - 1 Then
                        listSupplier.SelectedIndex += 1
                        e.Handled = True
                    End If
                Case Keys.Up
                    If listSupplier.SelectedIndex > 0 Then
                        listSupplier.SelectedIndex -= 1
                        e.Handled = True
                    End If
                Case Keys.Enter
                    ' Accept selection dari ListBox
                    If listSupplier.SelectedItem IsNot Nothing Then
                        PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
                        e.Handled = True
                    End If
            End Select
        End If
    End Sub

    Private Sub ListSupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles listSupplier.KeyDown
        If e.KeyCode = Keys.Enter Then
            PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
        End If
    End Sub

    Private Sub ListSupplier_Click(sender As Object, e As EventArgs) Handles listSupplier.Click
        PilihSupplierLangsung(CType(listSupplier.SelectedItem, DataSupplier), True)
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: PEMBAYARAN & KEYBOARD HANDLERS
    ' ═══════════════════════════════════════════════════════════════════
#Region "PEMBAYARAN & KEYBOARD HANDLERS"

    Private Sub CmbJenisBayar_KeyDown(sender As Object, e As KeyEventArgs) Handles CmbAkunTunai.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Tab Then
            ' Validasi: pastikan GBBayar visible
            If Not GBBayar.Visible Then Return

            TxtNominalBayarTunai.Select()
            TxtNominalBayarTunai.SelectAll()
        End If
    End Sub

    ''' <summary>
    ''' Shortcut Enter di panel bayar:
    ''' Cek CmbAkunTunai sudah dipilih, jika belum fokus ke sana
    ''' Jika sudah, pindah ke TxtNominalBayarTransfer
    ''' Enter/Panah Bawah: pindah ke TxtNominalBayarTransfer
    ''' </summary>
    Private Sub TxtNominalBayarTunai_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtNominalBayarTunai.KeyDown
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Down Then
            ' Validasi: pastikan GBBayar visible
            If Not GBBayar.Visible Then Return

            ' Cek apakah CmbAkunTunai sudah dipilih (hanya untuk Enter)
            If e.KeyCode = Keys.Enter AndAlso String.IsNullOrWhiteSpace(CmbAkunTunai.Text) Then
                MessageBox.Show("Silakan pilih akun tunai terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                CmbAkunTunai.Select()
                CmbAkunTunai.DroppedDown = True
                e.SuppressKeyPress = True
                Return
            End If

            ' Pindah ke transfer
            TxtNominalBayarTransfer.Select()
            TxtNominalBayarTransfer.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    ''' <summary>
    ''' Shortcut Enter di panel bayar:
    ''' Cek CmbAkunTransfer sudah dipilih, jika belum fokus ke sana
    ''' Jika sudah, langsung simpan transaksi
    ''' Info bank dihandle melalui function SetupInfoBank() manual
    ''' Panah Atas: pindah ke TxtNominalBayarTunai
    ''' </summary>
    Private Sub TxtNominalBayarTransfer_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtNominalBayarTransfer.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Validasi: pastikan GBBayar visible
            If Not GBBayar.Visible Then Return

            ' Cek apakah ada nominal transfer
            Dim nominalTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)

            If nominalTransfer > 0 Then
                ' Ada transfer → cek CmbAkunTransfer sudah dipilih
                If String.IsNullOrWhiteSpace(CmbAkunTransfer.Text) Then
                    MessageBox.Show("Silakan pilih akun transfer terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    CmbAkunTransfer.Select()
                    CmbAkunTransfer.DroppedDown = True
                    e.SuppressKeyPress = True
                    Return
                End If
            End If

            ' Lanjut simpan transaksi
            SimpanTransaksi()
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Up Then
            ' Navigasi panah atas: pindah ke TxtNominalBayarTunai
            TxtNominalBayarTunai.Select()
            TxtNominalBayarTunai.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub CmbBank_KeyDown_Pembelian(sender As Object, e As KeyEventArgs) Handles CmbBank.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Validasi: pastikan GBBayar visible
            If Not GBBayar.Visible Then Return

            CmbBank.DroppedDown = False
            TxtNoRek.Select()
            TxtNoRek.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtNoRek_KeyDown_Pembelian(sender As Object, e As KeyEventArgs) Handles TxtNoRek.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Validasi: pastikan GBBayar visible
            If Not GBBayar.Visible Then Return

            TxtNamaRek.Select()
            TxtNamaRek.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtNamaRek_KeyDown_Pembelian(sender As Object, e As KeyEventArgs) Handles TxtNamaRek.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Validasi: pastikan GBBayar visible
            If Not GBBayar.Visible Then Return

            TxtNoReff.Select()
            TxtNoReff.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtNoReff_KeyDown_Pembelian(sender As Object, e As KeyEventArgs) Handles TxtNoReff.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Validasi: pastikan GBBayar visible
            If Not GBBayar.Visible Then Return

            SimpanTransaksi()
            e.SuppressKeyPress = True
        End If
    End Sub


    Private Sub CmbJenisBayar_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbAkunTunai.SelectedIndexChanged, CmbAkunTransfer.SelectedIndexChanged
        AmbilKodeAkun()
    End Sub

    ''' <summary>
    ''' Shortcut Enter di CmbAkunTransfer:
    ''' Jika ada nominal transfer → pindah ke info bank (CmbBank)
    ''' Jika tidak ada transfer → langsung simpan transaksi
    ''' </summary>
    Private Sub CmbAkunTransfer_KeyDown(sender As Object, e As KeyEventArgs) Handles CmbAkunTransfer.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Validasi: pastikan GBBayar visible
            If Not GBBayar.Visible Then Return

            CmbAkunTransfer.DroppedDown = False

            ' Cek apakah ada nominal transfer
            Dim nominalTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)

            If nominalTransfer > 0 Then
                ' Ada transfer → pindah ke info bank
                SetupInfoBank()
            Else
                ' Tidak ada transfer → langsung simpan
                SimpanTransaksi()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub


    ''' <summary>
    ''' Ambil kode akun dari tbl_datareferensi berdasarkan nama akun yang dipilih
    ''' di ComboBox, lalu isi TextBox kode yang sesuai.
    ''' </summary>
    Private Sub AmbilKodeAkun()
        Dim sql As String = "SELECT KODE_AKUN FROM tbl_datareferensi WHERE NAMA_AKUN = @NAMA"

        Dim kodeTunai As String = ""
        Dim kodeTransfer As String = ""

        ' ── Kode akun tunai ──────────────────────────────────────────────
        If Not String.IsNullOrEmpty(CmbAkunTunai.Text) Then
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@NAMA", CmbAkunTunai.Text)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        kodeTunai = reader("KODE_AKUN").ToString()
                    End If
                End Using
            End Using
        End If

        ' ── Kode akun transfer ───────────────────────────────────────────
        If Not String.IsNullOrEmpty(CmbAkunTransfer.Text) Then
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@NAMA", CmbAkunTransfer.Text)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        kodeTransfer = reader("KODE_AKUN").ToString()
                    End If
                End Using
            End Using
        End If

        ' Terapkan nilai ke UI setelah koneksi tertutup
        If Not String.IsNullOrEmpty(kodeTunai) Then TxtKodeAkunTunai.Text = kodeTunai
        If Not String.IsNullOrEmpty(kodeTransfer) Then TxtKodeAkunTransfer.Text = kodeTransfer
    End Sub

    ''' <summary>
    ''' Setup fokus ke input info rekening bank pengirim
    ''' Dipanggil manual saat user ingin input info bank
    ''' </summary>
    Public Sub SetupInfoBank()
        CmbBank.Select()
        CmbBank.DroppedDown = True
    End Sub

    ''' <summary>
    ''' Handler KeyDown untuk TxtDiskonRupiah dan TxtDiskonPersen
    ''' Navigasi Enter/Panah Bawah: TxtDiskonRupiah → TxtPpnRupiah, TxtDiskonPersen → TxtPpnPersen
    ''' Validasi: hanya izinkan angka, backspace, delete, titik
    ''' </summary>
    Private Sub TxtDiskon_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtDiskonRupiah.KeyDown, TxtDiskonPersen.KeyDown
        ' Navigasi Enter/Panah Bawah: TxtDiskonRupiah → TxtPpnRupiah
        If (e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Down) AndAlso sender Is TxtDiskonRupiah Then
            TxtPpnRupiah.Select()
            TxtPpnRupiah.SelectAll()
            e.SuppressKeyPress = True
            Return
        End If

        ' Navigasi Enter/Panah Bawah: TxtDiskonPersen → TxtPpnPersen
        If (e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Down) AndAlso sender Is TxtDiskonPersen Then
            TxtPpnPersen.Select()
            TxtPpnPersen.SelectAll()
            e.SuppressKeyPress = True
            Return
        End If

        ' Validasi input angka
        Dim allowedKeys As Keys() = {Keys.Back, Keys.Delete, Keys.Left, Keys.Right, Keys.OemPeriod}
        If (e.KeyCode < Keys.D0 OrElse e.KeyCode > Keys.D9) AndAlso
           (e.KeyCode < Keys.NumPad0 OrElse e.KeyCode > Keys.NumPad9) AndAlso
           Not allowedKeys.Contains(e.KeyCode) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    ''' <summary>
    ''' Handler KeyDown untuk TxtPpnRupiah dan TxtPpnPersen
    ''' Navigasi Panah Atas: TxtPpnRupiah → TxtDiskonRupiah, TxtPpnPersen → TxtDiskonPersen
    ''' Navigasi Enter/Panah Bawah: TxtPpnRupiah → TxtBiayaKirim
    ''' Validasi: hanya izinkan angka, backspace, delete, titik
    ''' </summary>
    Private Sub TxtPpn_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtPpnRupiah.KeyDown, TxtPpnPersen.KeyDown
        ' Navigasi panah atas: TxtPpnRupiah → TxtDiskonRupiah
        If e.KeyCode = Keys.Up AndAlso sender Is TxtPpnRupiah Then
            TxtDiskonRupiah.Select()
            TxtDiskonRupiah.SelectAll()
            e.SuppressKeyPress = True
            Return
        End If

        ' Navigasi Enter/Panah Bawah: TxtPpnRupiah → TxtBiayaKirim
        If (e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Down) AndAlso sender Is TxtPpnRupiah Then
            TxtBiayaKirim.Select()
            TxtBiayaKirim.SelectAll()
            e.SuppressKeyPress = True
            Return
        End If

        ' Navigasi panah atas: TxtPpnPersen → TxtDiskonPersen
        If e.KeyCode = Keys.Up AndAlso sender Is TxtPpnPersen Then
            TxtDiskonPersen.Select()
            TxtDiskonPersen.SelectAll()
            e.SuppressKeyPress = True
            Return
        End If

        ' Validasi input angka
        Dim allowedKeys As Keys() = {Keys.Back, Keys.Delete, Keys.Left, Keys.Right, Keys.OemPeriod}
        If (e.KeyCode < Keys.D0 OrElse e.KeyCode > Keys.D9) AndAlso
           (e.KeyCode < Keys.NumPad0 OrElse e.KeyCode > Keys.NumPad9) AndAlso
           Not allowedKeys.Contains(e.KeyCode) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    ''' <summary>
    ''' Handler KeyDown untuk TxtBiayaKirim
    ''' Navigasi panah: TxtBiayaKirim ↑ TxtPpnRupiah
    ''' Validasi: hanya izinkan angka, backspace, delete, titik
    ''' </summary>
    Private Sub TxtBiayaKirim_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtBiayaKirim.KeyDown
        ' Navigasi panah atas: TxtBiayaKirim → TxtPpnRupiah
        If e.KeyCode = Keys.Up Then
            TxtPpnRupiah.Select()
            TxtPpnRupiah.SelectAll()
            e.SuppressKeyPress = True
            Return
        End If

        ' Validasi input angka
        Dim allowedKeys As Keys() = {Keys.Back, Keys.Delete, Keys.Left, Keys.Right, Keys.OemPeriod}
        If (e.KeyCode < Keys.D0 OrElse e.KeyCode > Keys.D9) AndAlso
           (e.KeyCode < Keys.NumPad0 OrElse e.KeyCode > Keys.NumPad9) AndAlso
           Not allowedKeys.Contains(e.KeyCode) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    ' ===== ADD SETELAH CLASS DECLARATION =====
    Private barcodeChars As New List(Of Char)()
    Private barcodeStartTime As DateTime = DateTime.MinValue
    Private lastKeyTime As DateTime = DateTime.MinValue
    Private isBarcodeMode As Boolean = False
    Private barcodeTimer As New System.Windows.Forms.Timer()

    Private Const BARCODE_CHAR_INTERVAL_MS As Integer = 30
    Private Const BARCODE_TOTAL_TIME_MS As Integer = 200
    Private Const BARCODE_MIN_LENGTH As Integer = 4
    Private Const BARCODE_MAX_LENGTH As Integer = 100

    Private suppressTextChanged As Boolean = False
    Private isUpdatingDiskon As Boolean = False
    Private isUpdatingPajak As Boolean = False

    ' ===== DGV INLINE EDIT CONTEXT TRACKING =====
    Private _dgvEditingTextBox As TextBox = Nothing
    Private _sedangPindahKeLstBarang As Boolean = False
    Private _rowSaatPindahKeLst As Integer = -1
    ' [FP1-T03-1] HAPUS: Flag state navigasi ListView kompleks
    ' Alasan: ListBox tidak memerlukan flag state untuk navigasi keyboard
    ' Private _lstBarangSelectedIndex As Integer = -1  ' DIHAPUS
    ' Private _lstBarangBaruMasuk As Boolean = False  ' DIHAPUS
    Private _konteksLstBarang As String = "TXTNAMA"
    Private _sedangSetNilaiDariListBox As Boolean
    ' [FP1-T07-1] TAMBAH: Flag sederhana untuk ListBox
    ' Simpan teks yang diketik user sebelum pindah ke ListBox.
    ' Dipakai untuk restore teks saat user tekan Up (kembali ke TextBox untuk refine search).
    ' Di-reset ke "" setelah dipakai agar tidak mengganggu sesi berikutnya.
    Private _teksSebelumPindahKeLstBarang As String = ""
    ' Simpan posisi sel DGV saat ListBox dibuka — untuk CellLeave guard.
    ' CellLeave hanya menutup ListBox jika sel yang ditinggalkan BERBEDA dari sel ini.
    Private _listBoxDibukaDiRow As Integer = -1
    Private _listBoxDibukaDiCol As Integer = -1

    ' --- Replace existing TxtNama_KeyDown, TxtNama_TextChanged, ProcessInput and add barcode timer handlers/helpers ---
    Private Sub TxtNama_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNama.KeyDown
        ' [FP1-T04-2] HAPUS: Navigasi Down arrow ke ListView - tidak diperlukan untuk ListBox
        ' Alasan: User akan menggunakan mouse click untuk memilih dari ListBox
        ' Logic navigasi keyboard sederhana akan ditambahkan di TASK-08 untuk Mode 2

        If e.KeyCode = Keys.Tab Then
            DgvData.Select()
            DgvData.Focus()
            e.SuppressKeyPress = True
            Return
        End If

        ' ===== PRINTABLE CHARACTERS =====
        Dim ch As Char = ChrW(e.KeyCode)
        If Not Char.IsControl(ch) Then
            ' If user types a letter or '*' -> manual input, cancel barcode detection
            If ch = "*"c OrElse Char.IsLetter(ch) Then
                ResetBarcodeDetection()
                Return
            End If

            Dim currentTime = DateTime.Now

            ' First character
            If barcodeChars.Count = 0 Then
                barcodeStartTime = currentTime
                barcodeChars.Add(ch)
                lastKeyTime = currentTime

                barcodeTimer.Interval = 100
                barcodeTimer.Stop()
                barcodeTimer.Start()
                Return
            End If

            ' Interval since last key
            Dim intervalMs = (currentTime - lastKeyTime).TotalMilliseconds

            ' If slow typing -> not barcode
            If intervalMs > BARCODE_CHAR_INTERVAL_MS Then
                isBarcodeMode = False
            End If

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

            ' Process input (barcode vs manual) using totalTimeMs heuristic
            ProcessInput(inputText, totalTimeMs)
            ResetBarcodeDetection()
            Return
        End If

        If e.KeyCode = Keys.Back Or e.KeyCode = Keys.Delete Then
            isBarcodeMode = False
        End If
    End Sub

    Private Sub TxtNama_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.TextChanged
        _konteksLstBarang = "TXTNAMA"
        Dim currentText = TxtNama.Text.Trim()

        If String.IsNullOrEmpty(currentText) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        ' Parse format qty*satuan*nama atau qty*nama — set TxtQty & TxtLevelSat sebelum search
        If currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            If parts.Length >= 3 Then
                ' FORMAT: qty*satuan*nama
                SetQtyAndSatuan(parts(0), parts(1))
            ElseIf parts.Length = 2 Then
                ' FORMAT: qty*nama
                SetQtyOnly(parts(0))
            End If
        End If

        ' Show manual search only when user types letters or uses qty*... pattern
        If currentText.Any(AddressOf Char.IsLetter) Then
            TriggerManualSearch(currentText)
        ElseIf currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
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
                ' Jika buffer mengandung '*' atau huruf → input manual bertempo cepat
                If bufferText.Contains("*"c) OrElse bufferText.Any(AddressOf Char.IsLetter) Then
                    If _konteksLstBarang = "DGV" Then
                        ' Jalur DGV — manual search sudah ditangani TextChanged, tidak perlu ulang
                        ResetBarcodeDetection()
                    Else
                        TriggerManualSearch(bufferText)
                        ResetBarcodeDetection()
                    End If
                    Return
                End If

                ' Murni numerik/alphanumeric → kandidat barcode
                If _konteksLstBarang = "DGV" Then
                    ' Jalur DGV — proses barcode langsung ke baris DGV tanpa BeginEdit ulang
                    Dim barisDiisi As Integer = If(DgvData.CurrentCell IsNot Nothing, DgvData.CurrentCell.RowIndex, -1)
                    If barisDiisi >= 0 Then
                        Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
                        If qtyValue <= 0 Then qtyValue = 1D
                        ' Koreksi barisDiisi — cari baris kosong non-IsNewRow pertama
                        For i As Integer = 0 To DgvData.Rows.Count - 1
                            If Not DgvData.Rows(i).IsNewRow Then
                                Dim idVal = Convert.ToString(DgvData.Rows(i).Cells("Id").Value).Trim()
                                Dim namaVal = Convert.ToString(DgvData.Rows(i).Cells("Nama").Value).Trim()
                                If String.IsNullOrEmpty(idVal) AndAlso String.IsNullOrEmpty(namaVal) Then
                                    barisDiisi = i
                                    Exit For
                                End If
                            End If
                        Next
                        ' Set flag, EndEdit, isi baris — tidak perlu BeginEdit ulang
                        _sedangSetNilaiDariListBox = True
                        DgvData.EndEdit(True)
                        DgvData.CurrentCell = Nothing
                        ' Cari nama barang dari barcode
                        Dim namaBarang As String = ""
                        Try
                            Using cmd As New MySqlCommand(
                                "SELECT NAMA_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND " &
                                "(BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc OR ID_BARANG=@bc) LIMIT 1", conn)
                                cmd.Parameters.AddWithValue("@bc", bufferText)
                                Dim result = cmd.ExecuteScalar()
                                If result IsNot Nothing Then namaBarang = result.ToString()
                            End Using
                        Catch
                        End Try
                        If Not String.IsNullOrEmpty(namaBarang) Then
                            TxtBarcode.Text = bufferText
                            IsiBarangKeRow(barisDiisi, namaBarang, qtyValue, barcodeInput:=bufferText)
                            ' Fokus ke IsNewRow berikutnya — set CurrentCell saja, TANPA BeginEdit
                            Dim nextRow As Integer = -1
                            For i As Integer = 0 To DgvData.Rows.Count - 1
                                If DgvData.Rows(i).IsNewRow Then nextRow = i : Exit For
                            Next
                            If nextRow >= 0 Then
                                DgvData.CurrentCell = DgvData(1, nextRow)
                                Me.ActiveControl = DgvData
                            End If
                        Else
                            MessageBox.Show("Barcode '" & bufferText & "' tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                        _sedangSetNilaiDariListBox = False
                    End If
                    ResetBarcodeDetection()
                Else
                    ' Jalur TxtNama — alur lama
                    ProcessInput(bufferText, (DateTime.Now - barcodeStartTime).TotalMilliseconds)
                    ResetBarcodeDetection()
                End If
            End If
        End If
    End Sub

    Private Sub SetQtyAndSatuan(qtyStr As String, satuanStr As String)
        Dim qty = ModuleAngka.ParseDecimal(qtyStr)
        TxtQty.Text = qty.ToString()
        TxtLevelSat.Text = satuanStr
    End Sub

    Private Sub SetQtyOnly(qtyStr As String)
        Dim qty = ModuleAngka.ParseDecimal(qtyStr)
        TxtQty.Text = qty.ToString()
        TxtLevelSat.Text = "1"
    End Sub

    Private Sub SetDefaultQtyAndSatuan()
        TxtQty.Text = "1"
        TxtLevelSat.Text = "1"
    End Sub


    ''' <summary>
    ''' Process input using same heuristics as FormPenjualan:
    ''' - qty*satuan*name
    ''' - qty*something (barcode candidate vs manual)
    ''' - barcode candidate when input fast OR DB contains barcode
    ''' </summary>
    Private Sub ProcessInput(inputText As String, totalTimeMs As Double)
        If String.IsNullOrEmpty(inputText) Then Return

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

            ' Jika bukan scan cepat, coba deteksi barcode di DB lalu fallback ke pencarian manual
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


    Private Function IsBarcodeCandidate(input As String) As Boolean
        If input.Length < BARCODE_MIN_LENGTH Then Return False
        Return BarcodeExistsInDatabase(input)
    End Function

    Private Function BarcodeExistsInDatabase(barcodeValue As String) As Boolean
        Const query = "SELECT 1 FROM tbl_barang " &
                  "WHERE STATUS = 'Aktif' AND (BARCODE_KECIL = @bc OR BARCODE_SEDANG = @bc OR BARCODE_BESAR = @bc) LIMIT 1"
        Try
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@bc", barcodeValue)
                Return cmd.ExecuteScalar() IsNot Nothing
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Function SearchByBarcode(barcodeText As String) As Boolean
        Dim query = "SELECT NAMA_BARANG FROM tbl_barang " &
               "WHERE STATUS = 'Aktif' AND (BARCODE_KECIL = @bc OR BARCODE_SEDANG = @bc OR BARCODE_BESAR = @bc) LIMIT 1"

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

            Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
            If qtyValue <= 0 Then qtyValue = 1D

            Dim levelValue As Integer = 1
            Integer.TryParse(TxtLevelSat.Text, levelValue)
            If levelValue < 1 OrElse levelValue > 3 Then levelValue = 1

            ' Cari baris kosong atau tambah baru
            Dim targetRowIdx As Integer = -1
            For i As Integer = 0 To DgvData.Rows.Count - 1
                If Not DgvData.Rows(i).IsNewRow Then
                    Dim idVal = Convert.ToString(DgvData.Rows(i).Cells("Id").Value).Trim()
                    Dim namaVal = Convert.ToString(DgvData.Rows(i).Cells("Nama").Value).Trim()
                    If String.IsNullOrEmpty(idVal) AndAlso String.IsNullOrEmpty(namaVal) Then
                        targetRowIdx = i
                        Exit For
                    End If
                End If
            Next

            If targetRowIdx = -1 Then
                targetRowIdx = DgvData.Rows.Add()
            End If

            ' Isi data ke baris menggunakan IsiBarangKeRow
            ' Kita bisa pakai namaBarang yang sudah ditemukan, atau pakai barcodeText langsung
            IsiBarangKeRow(targetRowIdx, namaBarang, qtyValue, levelValue, barcodeText)

            KosongTxtboxcari()
            SetupFocusToGrid()
            Return True
        End If

        Return False
    End Function


    Private Sub TriggerManualSearch(keyword As String)
        ' Stop barcode detection to avoid race closing listbox
        ResetBarcodeDetection()

        If keyword.Contains("*") Then
            Dim parts = keyword.Split("*"c)
            If parts.Length >= 2 Then
                keyword = parts.Last().Trim()
            End If
        End If

        If keyword.Length < 2 Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            If String.IsNullOrEmpty(TxtQty.Text) Then TxtQty.Text = "1"
            If String.IsNullOrEmpty(TxtLevelSat.Text) Then TxtLevelSat.Text = "1"
            Return
        End If

        ProcessManualSearchList(keyword)
    End Sub

    Private Sub ProcessManualSearchList(searchKeyword As String)
        SearchBarangToListBox(searchKeyword, "TXTNAMA")
    End Sub

    Private Sub ResetBarcodeDetection()
        isBarcodeMode = False
        barcodeChars.Clear()
        barcodeStartTime = DateTime.MinValue
        lastKeyTime = DateTime.MinValue
        barcodeTimer.Stop()
    End Sub

    ' [FP1-T08-1] TAMBAH: Event handler LstBarang_KeyDown untuk navigasi keyboard (Mode 2)
    ' Fungsi: Handle Up arrow (kembali ke TextBox) dan Enter (pilih item) dan Escape (tutup)
    ' Alur: Up di item pertama → kembali ke TextBox untuk refine search
    Private Sub LstBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles LstBarang.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If LstBarang.SelectedIndex <= 0 Then
                    _sedangPindahKeLstBarang = True
                    e.SuppressKeyPress = True
                    If _konteksLstBarang = "DGV" Then
                        Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                        _teksSebelumPindahKeLstBarang = ""
                        DgvData.Focus()
                        DgvData.BeginInvoke(New Action(Sub()
                                                           If DgvData.CurrentCell IsNot Nothing Then
                                                               DgvData.BeginEdit(True)
                                                               Dim editCtrl = TryCast(DgvData.EditingControl, TextBox)
                                                               If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                                                   editCtrl.Text = teksSimpan
                                                                   editCtrl.SelectionStart = teksSimpan.Length
                                                                   editCtrl.SelectionLength = 0
                                                               End If
                                                               editCtrl?.Focus()
                                                           End If
                                                           _sedangPindahKeLstBarang = False
                                                       End Sub))
                    Else
                        Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                        _teksSebelumPindahKeLstBarang = ""
                        TxtNama.Focus()
                        If Not String.IsNullOrEmpty(teksSimpan) Then
                            TxtNama.Text = teksSimpan
                            TxtNama.SelectionStart = teksSimpan.Length
                            TxtNama.SelectionLength = 0
                        End If
                        _sedangPindahKeLstBarang = False
                    End If
                End If

            Case Keys.Enter
                If LstBarang.SelectedIndex >= 0 Then
                    _sedangPindahKeLstBarang = True
                    AmbilDataDariListBox()
                    _sedangPindahKeLstBarang = False
                End If
                e.SuppressKeyPress = True

            Case Keys.Escape
                TutupListBox()
                ' Restore fokus dan teks ke tempat asal — sama seperti FormJual
                If _konteksLstBarang = "DGV" Then
                    Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                    _teksSebelumPindahKeLstBarang = ""
                    DgvData.Focus()
                    DgvData.BeginInvoke(New Action(Sub()
                                                       If DgvData.CurrentCell IsNot Nothing Then
                                                           DgvData.BeginEdit(True)
                                                           Dim editCtrl = TryCast(DgvData.EditingControl, TextBox)
                                                           If editCtrl IsNot Nothing AndAlso Not String.IsNullOrEmpty(teksSimpan) Then
                                                               editCtrl.Text = teksSimpan
                                                               editCtrl.SelectionStart = teksSimpan.Length
                                                               editCtrl.SelectionLength = 0
                                                           End If
                                                           editCtrl?.Focus()
                                                       End If
                                                       _sedangPindahKeLstBarang = False
                                                   End Sub))
                Else
                    Dim teksSimpan As String = _teksSebelumPindahKeLstBarang
                    _teksSebelumPindahKeLstBarang = ""
                    TxtNama.Focus()
                    If Not String.IsNullOrEmpty(teksSimpan) Then
                        TxtNama.Text = teksSimpan
                        TxtNama.SelectionStart = teksSimpan.Length
                        TxtNama.SelectionLength = 0
                    End If
                    _sedangPindahKeLstBarang = False
                End If
                e.SuppressKeyPress = True
        End Select
    End Sub

    ' [FP1-T08-2] UBAH: LstBarang_SelectedIndexChanged dikosongkan — pemilihan aktual via Click dan KeyDown (Enter)
    ' Event ini terpicu saat navigasi keyboard (Down/Up) juga, bukan hanya saat memilih.
    ' Jika langsung memanggil AmbilDataDariListBox() di sini, ListBox akan langsung tutup
    ' begitu user menekan Down pertama kali — tidak ada kesempatan untuk navigasi.
    Private Sub LstBarang_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles LstBarang.SelectedIndexChanged
        ' Hanya tracking — pemilihan aktual via LstBarang_Click dan LstBarang_KeyDown (Enter)
    End Sub

    ' [FP1-T08-3] TAMBAH: Event handler LstBarang_Click untuk mouse click
    ' Fungsi: Handle mouse click untuk memilih item dari ListBox
    Private Sub LstBarang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LstBarang.Click
        If LstBarang.SelectedIndex >= 0 Then
            AmbilDataDariListBox()
        End If
    End Sub

    Private Sub TutupListBox()
        LstBarang.Visible = False
        LstBarang.Items.Clear()
        _listBoxDibukaDiRow = -1
        _listBoxDibukaDiCol = -1
        If _konteksLstBarang = "DGV" Then
            If _dgvEditingTextBox IsNot Nothing Then
                _dgvEditingTextBox.Select()
            Else
                DgvData.Focus()
            End If
        Else
            TxtNama.Select()
        End If
    End Sub

    Private Sub AmbilDataDariListBox()
        ' [FP1-T12-1] UBAH: Parse string dari ListBox untuk mendapatkan nama barang
        ' Alasan: ListBox menggunakan format string, bukan ListViewItem
        ' Format: "Nama Barang | T: {stokToko} | G: {stokGudang}" atau "Nama Barang"

        ' Reset teks tersimpan — user sudah memilih item, teks lama tidak relevan lagi
        _teksSebelumPindahKeLstBarang = ""

        Dim selectedValue As String = ""

        If LstBarang.SelectedIndex >= 0 AndAlso LstBarang.SelectedIndex < LstBarang.Items.Count Then
            selectedValue = LstBarang.Items(LstBarang.SelectedIndex).ToString()
        ElseIf LstBarang.Items.Count = 1 Then
            selectedValue = LstBarang.Items(0).ToString()
        End If

        If String.IsNullOrEmpty(selectedValue) Then
            Return
        End If

        ' [FP1-T12-2] UBAH: Extract nama barang dari format string
        ' Split pada "|" untuk memisahkan nama barang dari info stok
        Dim namayangdiambil As String = selectedValue
        If selectedValue.Contains("|") Then
            Dim parts = selectedValue.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Length > 0 Then
                namayangdiambil = parts(0).Trim()
            End If
        End If

        TutupListBox()

        ' Konteks DGV inline edit
        If _konteksLstBarang = "DGV" AndAlso
           DgvData.CurrentCell IsNot Nothing AndAlso DgvData.CurrentCell.ColumnIndex = 1 Then

            ' Gunakan TxtQty & TxtLevelSat yang sudah disinkronkan oleh TextChanged
            Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
            If qtyValue <= 0 Then qtyValue = 1D

            Dim levelValue As Integer = 1
            Integer.TryParse(TxtLevelSat.Text, levelValue)
            If levelValue < 1 OrElse levelValue > 3 Then levelValue = 1

            ' Tentukan baris yang akan diisi:
            ' Gunakan CurrentCell.RowIndex sebagai default.
            ' Koreksi: cari baris dengan Id kosong pertama dari atas.
            ' Baris yang sedang diedit punya Nama = keyword tapi Id masih kosong — itu target yang benar.
            ' Tidak boleh cek Nama karena baris target justru berisi keyword yang diketik user.
            Dim barisDiisi As Integer = DgvData.CurrentCell.RowIndex

            For i As Integer = 0 To DgvData.Rows.Count - 1
                If Not DgvData.Rows(i).IsNewRow Then
                    Dim idVal = Convert.ToString(DgvData.Rows(i).Cells("Id").Value).Trim()
                    If String.IsNullOrEmpty(idVal) Then
                        ' Baris dengan Id kosong pertama — ini target yang benar
                        barisDiisi = i
                        Exit For
                    End If
                End If
            Next

            ' Selesaikan edit mode dulu sebelum mengubah cell
            ' Flag tetap True sampai seluruh proses selesai — blok CellEndEdit agar tidak ikut proses
            _sedangSetNilaiDariListBox = True
            DgvData.EndEdit(True)
            DgvData.CurrentCell = Nothing

            ' CEK DUPLIKAT berdasarkan SettingIzinkanSatuanBerbeda untuk konteks DGV
            If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
                ' Ambil ID_BARANG dari nama barang untuk cek duplikat
                Dim idBarangBaru As String = AmbilKodeBarangDariNama(namayangdiambil)
                If String.IsNullOrEmpty(idBarangBaru) Then
                    ' Jika tidak ditemukan, lanjutkan isi data biasa
                    IsiBarangKeRow(barisDiisi, namayangdiambil, qtyValue, levelValue)
                    _sedangSetNilaiDariListBox = False
                    SetupFocusToGrid()
                    Return
                End If

                ' Cek apakah barang yang sama sudah ada di baris lain
                For Each row As DataGridViewRow In DgvData.Rows
                    If row.Index <> barisDiisi AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() = idBarangBaru Then
                        Dim qtyLama As Decimal = If(IsDBNull(row.Cells("Qty").Value), 0D, ModuleAngka.ParseDecimal(row.Cells("Qty").Value))
                        Dim qtyBaru As Decimal = qtyValue
                        Dim qtyTotal As Decimal = qtyLama + qtyBaru

                        Dim isi As Integer = ModuleAngka.ParseInteger(row.Cells("Isi").Value, defaultValue:=1)

                        row.Cells("Qty").Value = qtyTotal
                        row.Cells("QtySat").Value = qtyTotal * isi

                        UpdateSemuaTotal()

                        ' Hapus baris yang sedang diisi (karena sudah digabungkan)
                        If Not DgvData.Rows(barisDiisi).IsNewRow Then
                            DgvData.Rows.RemoveAt(barisDiisi)
                        End If

                        _sedangSetNilaiDariListBox = False
                        SetupFocusToGrid()
                        Return
                    End If
                Next
            End If

            ' Isi semua data langsung ke baris — tidak lewat CellEndEdit
            IsiBarangKeRow(barisDiisi, namayangdiambil, qtyValue, levelValue)
            _sedangSetNilaiDariListBox = False

            ' Navigasi ke baris kosong berikutnya
            SetupFocusToGrid()
            Return
        End If

        ' Konteks TxtNamaBarang
        Dim qtyValueTxt As Decimal = ModuleAngka.ParseDecimal(TxtQty.Text)
        If qtyValueTxt <= 0 Then qtyValueTxt = 1D

        Dim levelValueTxt As Integer = 1
        Integer.TryParse(TxtLevelSat.Text, levelValueTxt)
        If levelValueTxt < 1 OrElse levelValueTxt > 3 Then levelValueTxt = 1

        ' Cari baris kosong atau tambah baru
        Dim targetRowIdx As Integer = -1
        For i As Integer = 0 To DgvData.Rows.Count - 1
            If Not DgvData.Rows(i).IsNewRow Then
                Dim idVal = Convert.ToString(DgvData.Rows(i).Cells("Id").Value).Trim()
                Dim namaVal = Convert.ToString(DgvData.Rows(i).Cells("Nama").Value).Trim()
                If String.IsNullOrEmpty(idVal) AndAlso String.IsNullOrEmpty(namaVal) Then
                    targetRowIdx = i
                    Exit For
                End If
            End If
        Next

        If targetRowIdx = -1 Then
            targetRowIdx = DgvData.Rows.Add()
        End If

        ' Cek Duplikat untuk Mode Manual (TxtNama)
        If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
            Dim idBarangBaru As String = AmbilKodeBarangDariNama(namayangdiambil)
            If Not String.IsNullOrEmpty(idBarangBaru) Then
                For Each row As DataGridViewRow In DgvData.Rows
                    If row.Index <> targetRowIdx AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() = idBarangBaru Then
                        ' Jika duplikat ditemukan, gabungkan qty ke baris yang sudah ada
                        Dim qtyLama As Decimal = ModuleAngka.ParseDecimal(row.Cells("Qty").Value)
                        row.Cells("Qty").Value = qtyLama + qtyValueTxt
                        UpdateSemuaTotal()

                        ' Bersihkan baris target yang tadi disiapkan (jika bukan IsNewRow)
                        If Not DgvData.Rows(targetRowIdx).IsNewRow Then
                            DgvData.Rows.RemoveAt(targetRowIdx)
                        End If

                        KosongTxtboxcari()
                        SetupFocusToGrid()
                        Return
                    End If
                Next
            End If
        End If

        ' Isi data ke baris
        IsiBarangKeRow(targetRowIdx, namayangdiambil, qtyValueTxt, levelValueTxt)
        KosongTxtboxcari()
        SetupFocusToGrid()
    End Sub

    ' ===== FUNGSI PENCARIAN BARU — LISTVIEW PATTERN =====

    ''' <summary>
    ''' Metode pencarian umum untuk mengisi ListBox dengan hasil pencarian barang.
    ''' [FP1-T11-1] UBAH: Populate ListBox dengan format string (adaptasi dari FormJual)
    ''' </summary>
    ''' <param name="searchKeyword">Keyword pencarian</param>
    ''' <param name="konteks">"TXTNAMA" untuk pencarian dari TxtNama, "DGV" untuk pencarian dari DataGridView</param>
    Private Sub SearchBarangToListBox(searchKeyword As String, konteks As String)
        searchKeyword = searchKeyword.Trim()

        ' Validasi min 2 karakter
        If searchKeyword.Length < 2 AndAlso Not searchKeyword.All(AddressOf Char.IsDigit) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        ' Untuk konteks DGV, validasi harus ada huruf
        If konteks = "DGV" AndAlso Not searchKeyword.Any(AddressOf Char.IsLetter) Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            Return
        End If

        ' [FP1-T11-2] UBAH: Query dinamis berdasarkan SettingTampilInfoStok
        ' Alasan: Optimasi database - hanya ambil kolom stok jika diperlukan
        Dim query As String
        If ModulHakAkses.SettingTampilInfoStok Then
            query = "SELECT NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
                    "WHERE STATUS = 'Aktif' AND (" &
                    "   ID_BARANG LIKE @key " &
                    "   OR NAMA_BARANG LIKE @key " &
                    "   OR BARCODE_KECIL LIKE @key " &
                    "   OR BARCODE_SEDANG LIKE @key " &
                    "   OR BARCODE_BESAR LIKE @key) LIMIT 200"
        Else
            query = "SELECT NAMA_BARANG FROM tbl_barang " &
                    "WHERE STATUS = 'Aktif' AND (" &
                    "   ID_BARANG LIKE @key " &
                    "   OR NAMA_BARANG LIKE @key " &
                    "   OR BARCODE_KECIL LIKE @key " &
                    "   OR BARCODE_SEDANG LIKE @key " &
                    "   OR BARCODE_BESAR LIKE @key) LIMIT 200"
        End If

        Try
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@key", "%" & searchKeyword & "%")

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    LstBarang.Items.Clear()

                    ' [FP1-T11-3] UBAH: Populate ListBox dengan format string
                    ' Format: "Nama Barang | T: {stokToko} | G: {stokGudang}" jika stok display enabled
                    '         "Nama Barang" jika stok display disabled
                    While rd.Read()
                        Dim namaBarang = rd("NAMA_BARANG").ToString()
                        Dim displayString As String

                        If ModulHakAkses.SettingTampilInfoStok Then
                            Dim stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                            Dim stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                            displayString = String.Format("{0} | T: {1} | G: {2}", namaBarang, stokToko.ToString("N0"), stokGudang.ToString("N0"))
                        Else
                            displayString = namaBarang
                        End If

                        LstBarang.Items.Add(displayString)
                    End While
                End Using
            End Using

            ' Tampilkan ListBox jika ada hasil
            If LstBarang.Items.Count > 0 Then
                If konteks = "TXTNAMA" Then
                    PosisikanLstBarangDiBawahTxtNama()
                ElseIf konteks = "DGV" Then
                    PosisikanLstBarangDiBawahSel()
                    LstBarang.BringToFront()
                End If
                ' Simpan posisi sel saat ListBox dibuka — untuk guard CellLeave (sama seperti FormJual)
                If konteks = "DGV" AndAlso DgvData.CurrentCell IsNot Nothing Then
                    _listBoxDibukaDiRow = DgvData.CurrentCell.RowIndex
                    _listBoxDibukaDiCol = DgvData.CurrentCell.ColumnIndex
                Else
                    _listBoxDibukaDiRow = -1
                    _listBoxDibukaDiCol = -1
                End If
                LstBarang.Visible = True
            Else
                LstBarang.Visible = False
                _listBoxDibukaDiRow = -1
                _listBoxDibukaDiCol = -1
            End If

            ' Set default qty = 1 hanya untuk konteks TXTNAMA
            If konteks = "TXTNAMA" Then
                If String.IsNullOrEmpty(TxtQty.Text) Then
                    TxtQty.Text = "1"
                End If
                If String.IsNullOrEmpty(TxtLevelSat.Text) Then
                    TxtLevelSat.Text = "1"
                End If
            End If
        Catch ex As Exception
            Debug.WriteLine("[ERROR] Search: " & ex.Message)
            MessageBox.Show("Error search: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Isi semua kolom baris DGV langsung dari DB berdasarkan nama barang — tanpa lewat CellEndEdit.
    ''' Tidak ada logika jenis pelanggan (pembelian tidak pakai Umum/Partai).
    ''' level: 1=kecil (default), 2=sedang, 3=besar. barcodeInput: jika dari scan, level ditentukan otomatis dari barcode.
    ''' </summary>
    Private Sub IsiBarangKeRow(rowIdx As Integer, namaBarang As String, qty As Decimal, Optional level As Integer = 1, Optional barcodeInput As String = "")
        If rowIdx < 0 OrElse rowIdx >= DgvData.Rows.Count Then Return

        Dim idBarang As String = ""
        Dim hargaBeli As Decimal = 0D
        Dim hargaBeliTerakhir As Decimal = 0D
        Dim stokToko As Decimal = 0D
        Dim stokGudang As Decimal = 0D
        Dim barcodeKecil As String = "" : Dim barcodeSedang As String = "" : Dim barcodeBesar As String = ""
        Dim satUmumKecil As String = "" : Dim isiUmumKecil As Integer = 1
        Dim satUmumSedang As String = "" : Dim isiUmumSedang As Integer = 1
        Dim satUmumBesar As String = "" : Dim isiUmumBesar As Integer = 1

        Try
            ' Jika barcodeInput diisi, cari berdasarkan barcode/ID dulu
            Dim query As String = ""
            If Not String.IsNullOrEmpty(barcodeInput) Then
                query = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_BELI_TERAKHIR, STOK_TOKO, STOK_GUDANG, " &
                        "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                        "SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, " &
                        "SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, " &
                        "SATUAN_UMUM_BESAR, ISI_UMUM_BESAR " &
                        "FROM tbl_barang WHERE STATUS='Aktif' AND " &
                        "(BARCODE_KECIL=@bc OR BARCODE_SEDANG=@bc OR BARCODE_BESAR=@bc OR ID_BARANG=@bc) LIMIT 1"
            Else
                query = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_BELI_TERAKHIR, STOK_TOKO, STOK_GUDANG, " &
                        "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
                        "SATUAN_UMUM_KECIL, ISI_UMUM_KECIL, " &
                        "SATUAN_UMUM_SEDANG, ISI_UMUM_SEDANG, " &
                        "SATUAN_UMUM_BESAR, ISI_UMUM_BESAR " &
                        "FROM tbl_barang WHERE STATUS='Aktif' AND NAMA_BARANG=@n LIMIT 1"
            End If

            Using cmd As New MySqlCommand(query, conn)
                If Not String.IsNullOrEmpty(barcodeInput) Then
                    cmd.Parameters.AddWithValue("@bc", barcodeInput.Trim())
                Else
                    cmd.Parameters.AddWithValue("@n", namaBarang.Trim())
                End If

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If Not rd.Read() Then Return
                    idBarang = rd("ID_BARANG").ToString()
                    namaBarang = rd("NAMA_BARANG").ToString() ' Ambil nama asli dari DB
                    hargaBeli = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                    hargaBeliTerakhir = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI_TERAKHIR", 0D)
                    stokToko = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D)
                    stokGudang = ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D)
                    barcodeKecil = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_KECIL", "")
                    barcodeSedang = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_SEDANG", "")
                    barcodeBesar = ModuleAngka.SafeGetValue(Of String)(rd, "BARCODE_BESAR", "")
                    satUmumKecil = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", "")
                    isiUmumKecil = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_KECIL", 1))
                    satUmumSedang = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", "")
                    isiUmumSedang = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_SEDANG", 1))
                    satUmumBesar = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")
                    isiUmumBesar = Math.Max(1, ModuleAngka.SafeGetValue(Of Integer)(rd, "ISI_UMUM_BESAR", 1))
                End Using
            End Using
        Catch
            Return
        End Try

        ' ── Tentukan satuan dan harga berdasarkan level ──
        ' [FP1-T14-1] UBAH: Selalu default ke satuan kecil (level 1) untuk pembelian
        ' Alasan: Pembelian selalu dicatat dalam satuan kecil (per unit)
        ' User bisa ubah manual lewat dropdown jika ada kasus khusus
        Dim options As New List(Of KeyValuePair(Of String, Integer))()

        ' [FP1-T14-2] HAPUS: Logika penentuan level dari barcode
        ' Selalu gunakan level 1 (kecil) sebagai default
        Dim levelAktif As Integer = 1

        ' Pilih satuan, isi sesuai levelAktif (selalu kecil untuk pembelian)
        Dim satuanAktif As String = satUmumKecil
        Dim isiAktif As Integer = isiUmumKecil

        ' Fallback ke kecil jika level yang diminta kosong
        If String.IsNullOrWhiteSpace(satuanAktif) Then
            satuanAktif = satUmumKecil : isiAktif = isiUmumKecil
        End If

        ' Isi options untuk combo box
        If Not String.IsNullOrWhiteSpace(satUmumKecil) Then options.Add(New KeyValuePair(Of String, Integer)(satUmumKecil, isiUmumKecil))
        If Not String.IsNullOrWhiteSpace(satUmumSedang) Then options.Add(New KeyValuePair(Of String, Integer)(satUmumSedang, isiUmumSedang))
        If Not String.IsNullOrWhiteSpace(satUmumBesar) Then options.Add(New KeyValuePair(Of String, Integer)(satUmumBesar, isiUmumBesar))

        ' Fallback jika tidak ada satuan sama sekali
        If options.Count = 0 Then options.Add(New KeyValuePair(Of String, Integer)("PCS", 1))
        If String.IsNullOrWhiteSpace(satuanAktif) Then satuanAktif = options(0).Key : isiAktif = options(0).Value

        ' Harga beli per satuan = harga beli dasar * isi satuan terpilih
        ' Di pembelian, kita biasanya pakai harga beli sebagai default
        Dim hargaBeliDefault As Decimal = hargaBeliTerakhir * isiAktif

        ' Isi baris DGV
        Dim row = DgvData.Rows(rowIdx)
        row.Cells("Id").Value = idBarang
        row.Cells("Nama").Value = namaBarang
        row.Cells("Qty").Value = qty
        row.Cells("Hargabeli").Value = hargaBeliDefault
        isiAktif = Math.Max(1, isiAktif)
        row.Cells("Isi").Value = isiAktif

        row.Cells("HargaBeliSatKecil").Value = hargaBeli / isiAktif
        row.Cells("QtySat").Value = qty * isiAktif
        row.Cells("Totalharga").Value = qty * hargaBeliDefault
        row.Cells("Average").Value = hargaBeli / isiAktif
        row.Cells("HargaSebelumnya").Value = hargaBeliTerakhir

        ' Isi stok jika kolom ada
        If DgvData.Columns.Contains("StokToko") Then row.Cells("StokToko").Value = stokToko
        If DgvData.Columns.Contains("StokGudang") Then row.Cells("StokGudang").Value = stokGudang

        ' Setup satuan combo box
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(row.Cells("Satuan"), DataGridViewComboBoxCell)
        kolomSatuan.Items.Clear()
        For Each opt In options
            kolomSatuan.Items.Add(opt.Key)
        Next
        kolomSatuan.Value = satuanAktif

        ' Set read-only setelah diisi
        row.Cells("Nama").ReadOnly = True
        row.Cells("Nama").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
        row.Cells("Nama").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)

        UpdateSemuaTotal()
    End Sub

    ''' <summary>Ambil ID_BARANG dari nama barang.</summary>
    Private Function AmbilKodeBarangDariNama(namaBarang As String) As String
        If String.IsNullOrWhiteSpace(namaBarang) Then Return ""
        Try
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG FROM tbl_barang WHERE STATUS='Aktif' AND NAMA_BARANG=@n LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@n", namaBarang)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return result.ToString()
                Else
                    Return ""
                End If
            End Using
        Catch
            Return ""
        End Try
    End Function

    ''' <summary>Handler TextChanged untuk TextBox edit inline kolom Nama di DGV.</summary>
    Private Sub DgvNamaBarang_TextChanged(sender As Object, e As EventArgs)
        If _sedangSetNilaiDariListBox Then Return
        _konteksLstBarang = "DGV"
        Dim txt As TextBox = TryCast(sender, TextBox)
        If txt Is Nothing Then Return
        Dim currentText = txt.Text.Trim()
        If String.IsNullOrEmpty(currentText) Then
            ' KRITIS: jangan sembunyikan jika ListBox masih visible dan fokus sedang pindah ke sana
            ' Teks kosong bisa terjadi karena DGV BeginEdit ulang (bukan user hapus teks)
            If _sedangPindahKeLstBarang OrElse LstBarang.Focused OrElse LstBarang.Visible Then
                Return
            End If
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            ResetBarcodeDetection()
            Return
        End If

        ' Feed karakter ke barcodeChars — pakai barcodeTimer yang sama dengan jalur TxtNama
        ' Jika input cepat (scanner) → barcodeTimer_Tick akan proses sebagai barcode
        ' Jika input lambat (ketik manual) → interval > BARCODE_CHAR_INTERVAL_MS → isBarcodeMode=False → manual search
        Dim currentTime = DateTime.Now
        If barcodeChars.Count = 0 Then
            barcodeStartTime = currentTime
        Else
            Dim intervalMs = (currentTime - lastKeyTime).TotalMilliseconds
            If intervalMs > BARCODE_CHAR_INTERVAL_MS Then isBarcodeMode = False
        End If
        ' Tambah semua karakter baru ke buffer (TextChanged bisa bawa >1 karakter saat paste/scan)
        For Each ch As Char In currentText
            If barcodeChars.Count < BARCODE_MAX_LENGTH Then barcodeChars.Add(ch)
        Next
        lastKeyTime = currentTime
        barcodeTimer.Stop()
        barcodeTimer.Start()

        ' Untuk input manual (ada huruf) — tampilkan ListView langsung tanpa tunggu timer
        Dim keyword As String = currentText
        Dim levelDGV As Integer = 1
        If currentText.Contains("*") Then
            Dim parts = currentText.Split("*"c)
            Dim qty As Decimal = ModuleAngka.ParseDecimal(parts(0).Trim())
            If qty > 0 Then TxtQty.Text = qty.ToString()
            If parts.Length >= 3 Then
                Dim lvl As Integer = 0
                If Integer.TryParse(parts(1).Trim(), lvl) AndAlso lvl >= 1 AndAlso lvl <= 3 Then levelDGV = lvl
                keyword = parts(parts.Length - 1).Trim()
            Else
                keyword = parts(parts.Length - 1).Trim()
            End If
        End If

        ' Hanya tampilkan ListView untuk input manual (ada huruf) — barcode murni ditangani timer
        If keyword.Any(AddressOf Char.IsLetter) Then
            TxtLevelSat.Text = levelDGV.ToString()
            SearchBarangToListBox(keyword, "DGV")
        End If
    End Sub

    ' [FP1-T06-1] HAPUS: DgvNamaBarang_KeyDown (versi ListView) - tidak diperlukan untuk ListBox
    ' Alasan: Navigasi ListView tidak diperlukan untuk ListBox
    ' Navigasi keyboard sederhana akan ditambahkan di TASK-08

    ' [FP1-T06-1] HAPUS: DgvNamaBarang_PreviewKeyDown (versi ListView) - tidak diperlukan untuk ListBox
    ' Alasan: Navigasi ListView tidak diperlukan untuk ListBox

    ''' <summary>
    ''' Guard penutup ListBox saat user pindah sel di DGV.
    ''' [FP1-T09-1] UBAH: Gunakan BeginInvoke dan flag _listBoxDibukaDiRow/Col
    ''' </summary>
    Private Sub DgvData_CellLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellLeave
        If LstBarang.Focused OrElse _sedangPindahKeLstBarang Then Return
        ' [FP1-T09-2] TAMBAH: Guard dengan flag posisi ListBox
        If LstBarang.Visible AndAlso e.RowIndex = _listBoxDibukaDiRow AndAlso e.ColumnIndex = _listBoxDibukaDiCol Then Return
        If LstBarang.Visible Then
            ' [FP1-T09-3] TAMBAH: BeginInvoke untuk delay penutupan ListBox
            Me.BeginInvoke(New Action(Sub()
                                          LstBarang.Visible = False
                                          LstBarang.Items.Clear()
                                          _listBoxDibukaDiRow = -1
                                          _listBoxDibukaDiCol = -1
                                      End Sub))
        End If
    End Sub

    ' [FP1-T10-1] HAPUS: LstBarang_SizeChanged - ListBox tidak memiliki kolom seperti ListView

    ''' <summary>Posisikan ListBox tepat di bawah sel yang sedang diedit di DGV.</summary>
    Private Sub PosisikanLstBarangDiBawahSel()
        If DgvData.CurrentCell Is Nothing Then Return
        Try
            ' [FP1-T10-2] TAMBAH: Simpan posisi sel saat membuka ListBox
            _listBoxDibukaDiRow = DgvData.CurrentCell.RowIndex
            _listBoxDibukaDiCol = DgvData.CurrentCell.ColumnIndex

            Dim cellRect = DgvData.GetCellDisplayRectangle(
                DgvData.CurrentCell.ColumnIndex, DgvData.CurrentCell.RowIndex, True)
            Dim ptDgv = DgvData.PointToScreen(New Point(cellRect.Left, cellRect.Bottom))
            Dim ptPanel = Me.PointToClient(ptDgv)
            
            LstBarang.Width = Math.Max(300, cellRect.Width)
            
            ' Cek sisa ruang vertikal di bawah sel aktif untuk menentukan posisi LstBarang (Atas/Bawah)
            Dim spaceBelow As Integer = Me.ClientSize.Height - ptPanel.Y
            If spaceBelow < LstBarang.Height + 40 Then
                ' Tampilkan di atas sel: Y = Bawah Sel - Tinggi Sel - Tinggi ListBox
                Dim targetY As Integer = ptPanel.Y - cellRect.Height - LstBarang.Height
                LstBarang.Location = New Point(ptPanel.X, targetY)
            Else
                ' Tampilkan di bawah sel
                LstBarang.Location = New Point(ptPanel.X, ptPanel.Y)
            End If
        Catch
        End Try
    End Sub

    ''' <summary>Posisikan ListView tepat di bawah TxtNama.</summary>
    Private Sub PosisikanLstBarangDiBawahTxtNama()
        Try
            Dim ptTxt = TxtNama.PointToScreen(New Point(0, TxtNama.Height))
            Dim ptPanel = Me.PointToClient(ptTxt)
            LstBarang.Location = New Point(ptPanel.X, ptPanel.Y)
            LstBarang.Width = Math.Max(300, TxtNama.Width)
        Catch
        End Try
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: STOK (STOCK)
    ' ═══════════════════════════════════════════════════════════════════
#Region "STOK"

    ' ===== FUNGSI STOK =====

    ''' <summary>
    ''' Ambil info stok terkini dari DB via SP.
    ''' Mode tambah  → sp_hlp_stok_ambil       (stok DB apa adanya)
    ''' Mode edit    → sp_hlp_stok_ambil_edit   (stok DB + qty di faktur lama yang akan dikembalikan)
    ''' </summary>
    Private Function AmbilInfoStok(kodeBarang As String, ByRef stokToko As Decimal, ByRef stokGudang As Decimal) As Boolean
        stokToko = 0D
        stokGudang = 0D
        If String.IsNullOrWhiteSpace(kodeBarang) Then Return False
        Try
            If IsModeTambahPembelian Then
                ' Mode tambah — stok DB apa adanya
                Using cmd As New MySqlCommand(
                    "CALL sp_hlp_stok_ambil(@kode, @toko, @gudang, @nama)", conn)
                    cmd.Parameters.AddWithValue("@kode", kodeBarang)
                    Dim pToko = cmd.Parameters.Add("@toko", MySqlDbType.Decimal)
                    pToko.Direction = ParameterDirection.Output
                    Dim pGudang = cmd.Parameters.Add("@gudang", MySqlDbType.Decimal)
                    pGudang.Direction = ParameterDirection.Output
                    Dim pNama = cmd.Parameters.Add("@nama", MySqlDbType.VarChar, 200)
                    pNama.Direction = ParameterDirection.Output
                    cmd.ExecuteNonQuery()
                    stokToko = ModuleAngka.ParseDecimal(pToko.Value)
                    stokGudang = ModuleAngka.ParseDecimal(pGudang.Value)
                    Return True
                End Using
            Else
                ' Mode edit — stok efektif = stok DB + qty di faktur lama yang akan dikembalikan
                Using cmd As New MySqlCommand(
                    "CALL sp_hlp_stok_ambil_edit(@kode, @faktur, @lokasi, @toko, @gudang, @nama)", conn)
                    cmd.Parameters.AddWithValue("@kode", kodeBarang)
                    cmd.Parameters.AddWithValue("@faktur", TxtIdPembelian.Text)
                    cmd.Parameters.AddWithValue("@lokasi", LblLokasiBarang.Text)
                    Dim pToko = cmd.Parameters.Add("@toko", MySqlDbType.Decimal)
                    pToko.Direction = ParameterDirection.Output
                    Dim pGudang = cmd.Parameters.Add("@gudang", MySqlDbType.Decimal)
                    pGudang.Direction = ParameterDirection.Output
                    Dim pNama = cmd.Parameters.Add("@nama", MySqlDbType.VarChar, 200)
                    pNama.Direction = ParameterDirection.Output
                    cmd.ExecuteNonQuery()
                    stokToko = ModuleAngka.ParseDecimal(pToko.Value)
                    stokGudang = ModuleAngka.ParseDecimal(pGudang.Value)
                    Return True
                End Using
            End If
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Refresh info stok satu baris DGV dari DB.
    ''' Tidak mengubah data transaksi — hanya update kolom StokToko/StokGudang.
    ''' </summary>
    Private Sub RefreshStokBaris(rowIdx As Integer)
        If rowIdx < 0 OrElse rowIdx >= DgvData.Rows.Count Then Return
        Dim row = DgvData.Rows(rowIdx)
        If row.IsNewRow Then Return
        Dim kode As String = Convert.ToString(row.Cells("Id").Value).Trim()
        If String.IsNullOrEmpty(kode) Then Return

        If Not DgvData.Columns.Contains("StokToko") OrElse Not DgvData.Columns.Contains("StokGudang") Then Return

        Dim stokToko As Decimal = 0D
        Dim stokGudang As Decimal = 0D
        If AmbilInfoStok(kode, stokToko, stokGudang) Then
            row.Cells("StokToko").Value = stokToko
            row.Cells("StokGudang").Value = stokGudang
        End If
    End Sub

    ''' <summary>
    ''' Refresh info stok semua baris DGV yang sudah terisi.
    ''' Dipakai untuk: load edit/draft agar stok selalu fresh.
    ''' </summary>
    Private Sub RefreshStokSemuaBaris()
        For i As Integer = 0 To DgvData.Rows.Count - 1
            RefreshStokBaris(i)
        Next
    End Sub

    ''' <summary>CellFormatting — highlight stok habis dengan warna amber dari ModuleTheme.</summary>
    Private Sub DgvData_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DgvData.CellFormatting
        ' ✅ Set read-only Nama jika ID sudah terisi (samakan dengan FormJual)
        If e.ColumnIndex = 1 Then ' Kolom Nama (index 1)
            Dim idValue = DgvData.Rows(e.RowIndex).Cells("Id").Value

            ' Jika ID sudah terisi, buat Nama read-only
            If idValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(idValue.ToString()) Then
                DgvData.Rows(e.RowIndex).Cells("Nama").ReadOnly = True
                DgvData.Rows(e.RowIndex).Cells("Nama").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
                DgvData.Rows(e.RowIndex).Cells("Nama").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
            Else
                DgvData.Rows(e.RowIndex).Cells("Nama").ReadOnly = False
                DgvData.Rows(e.RowIndex).Cells("Nama").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Surface, ModuleTheme.D_Surface)
            End If
        End If

        ' Guard: hanya proses jika kolom stok ada
        If Not DgvData.Columns.Contains("StokToko") OrElse Not DgvData.Columns.Contains("StokGudang") Then Return

        Dim stokTokoIndex As Integer = DgvData.Columns("StokToko").Index
        Dim stokGudangIndex As Integer = DgvData.Columns("StokGudang").Index

        If e.ColumnIndex = stokTokoIndex OrElse e.ColumnIndex = stokGudangIndex Then
            If e.Value IsNot Nothing AndAlso ModuleAngka.ParseDecimal(e.Value) < 1 Then
                ' Stok habis — warna informasi (amber), bukan merah
                e.CellStyle.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowStokHabis, ModuleTheme.D_DgvRowStokHabis)
                e.CellStyle.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
            End If
        End If
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: DGV EVENT HANDLERS
    ' ═══════════════════════════════════════════════════════════════════
#Region "DGV EVENT HANDLERS"

    ' ParseDecimal lokal dihapus — gunakan ModuleAngka.ParseDecimal

    ''' <summary>
    ''' CellEnter — khusus kolom Satuan langsung BeginEdit dan buka dropdown
    ''' agar user bisa langsung pilih satuan pakai panah atas/bawah tanpa F2 atau klik.
    ''' Kolom lain tetap EditOnKeystrokeOrF2.
    ''' </summary>
    Private Sub DgvData_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DgvData.CellEnter
        If DgvData.Columns(e.ColumnIndex).Name = "Satuan" Then
            DgvData.BeginInvoke(New Action(Sub()
                                               If DgvData.CurrentCell IsNot Nothing AndAlso
                   DgvData.CurrentCell.ColumnIndex = e.ColumnIndex AndAlso
                   DgvData.CurrentCell.RowIndex = e.RowIndex Then
                                                   DgvData.BeginEdit(True)
                                                   ' Buka dropdown agar panah atas/bawah langsung bisa memilih satuan
                                                   Dim combo = TryCast(DgvData.EditingControl, ComboBox)
                                                   If combo IsNot Nothing Then
                                                       combo.DroppedDown = True
                                                   End If
                                               End If
                                           End Sub))
        End If

        ' Kontrol read-only kolom Nama berdasarkan kolom Id — konsisten dengan FormJual
        ' Jika Id sudah terisi → Nama read-only (abu); jika kosong → Nama bisa diedit (putih)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 1 Then ' Kolom Nama (index 1)
            Dim idValue = DgvData.Rows(e.RowIndex).Cells("Id").Value
            If idValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(idValue.ToString()) Then
                DgvData.Rows(e.RowIndex).Cells("Nama").ReadOnly = True
                DgvData.Rows(e.RowIndex).Cells("Nama").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
                DgvData.Rows(e.RowIndex).Cells("Nama").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
            Else
                DgvData.Rows(e.RowIndex).Cells("Nama").ReadOnly = False
                DgvData.Rows(e.RowIndex).Cells("Nama").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Surface, ModuleTheme.D_Surface)
                DgvData.Rows(e.RowIndex).Cells("Nama").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
            End If
        End If
    End Sub

    Private Sub DgvDataData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellEndEdit
        ' ---> SOLUSI BUG BARCODE: Bersihkan handler TextBox DGV setiap kali selesai edit sel <---
        If _dgvEditingTextBox IsNot Nothing Then
            RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
            _dgvEditingTextBox = Nothing
        End If
        ResetBarcodeDetection()

        ' Guard: jangan proses jika sedang diisi dari ListView — IsiBarangKeRow yang akan mengisi
        If _sedangSetNilaiDariListBox Then Return

        '========================== Nama
        If e.ColumnIndex = 1 Then
            ' Samakan dengan FormJual: gunakan Not String.IsNullOrEmpty()
            If Not String.IsNullOrEmpty(DgvData.Rows(e.RowIndex).Cells("Nama").Value) Then
                Dim inputText As String = DgvData.Rows(e.RowIndex).Cells("Nama").Value.ToString().Trim()
                Dim qtyValue As Decimal = 1
                Dim namaBarangValue As String = inputText

                ' Cek apakah ada tanda bintang
                Dim indexAsteriskQty As Integer = inputText.IndexOf("*")
                Dim indexAsteriskHarga As Integer = -1

                If indexAsteriskQty >= 0 Then
                    indexAsteriskHarga = inputText.IndexOf("*", indexAsteriskQty + 1)
                End If

                If indexAsteriskQty >= 0 AndAlso indexAsteriskHarga > indexAsteriskQty Then
                    ' Format: qty * level * namaBarang
                    Dim angkaQty As String = inputText.Substring(0, indexAsteriskQty).Trim()
                    qtyValue = ModuleAngka.ParseDecimal(angkaQty)
                    namaBarangValue = inputText.Substring(indexAsteriskHarga + 1).Trim()

                ElseIf indexAsteriskQty >= 0 Then
                    ' Format: qty * namaBarang
                    Dim angkaQty As String = inputText.Substring(0, indexAsteriskQty).Trim()
                    qtyValue = ModuleAngka.ParseDecimal(angkaQty)
                    namaBarangValue = inputText.Substring(indexAsteriskQty + 1).Trim()
                End If

                DgvData.Rows(e.RowIndex).Cells("Nama").Value = namaBarangValue

                Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_BELI_TERAKHIR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR FROM tbl_barang WHERE STATUS = 'Aktif' AND (ID_BARANG = @NamaBarang OR NAMA_BARANG = @NamaBarang OR BARCODE_KECIL = @NamaBarang OR BARCODE_SEDANG = @NamaBarang OR BARCODE_BESAR = @NamaBarang)"

                Dim dataBarang As New Dictionary(Of String, Object)
                Dim found As Boolean = False

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@NamaBarang", namaBarangValue)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            found = True
                            For i As Integer = 0 To rd.FieldCount - 1
                                dataBarang(rd.GetName(i)) = rd.GetValue(i)
                            Next
                        End If
                    End Using
                End Using

                If found Then
                    DgvData.Rows(e.RowIndex).Cells("Id").Value = dataBarang("ID_BARANG")
                    DgvData.Rows(e.RowIndex).Cells("nama").Value = dataBarang("NAMA_BARANG")

                    Dim comboCell As DataGridViewComboBoxCell = CType(DgvData.Rows(e.RowIndex).Cells("Satuan"), DataGridViewComboBoxCell)
                    comboCell.Items.Clear()

                    Dim satuanKecil As String = If(IsDBNull(dataBarang("SATUAN_UMUM_KECIL")), "", dataBarang("SATUAN_UMUM_KECIL").ToString())
                    Dim satuanSedang As String = If(IsDBNull(dataBarang("SATUAN_UMUM_SEDANG")), "", dataBarang("SATUAN_UMUM_SEDANG").ToString())
                    Dim satuanBesar As String = If(IsDBNull(dataBarang("SATUAN_UMUM_BESAR")), "", dataBarang("SATUAN_UMUM_BESAR").ToString())

                    If Not String.IsNullOrEmpty(satuanKecil) Then comboCell.Items.Add(satuanKecil)
                    If Not String.IsNullOrEmpty(satuanSedang) Then comboCell.Items.Add(satuanSedang)
                    If Not String.IsNullOrEmpty(satuanBesar) Then comboCell.Items.Add(satuanBesar)

                    ' [FP1-T14-3] UBAH: Selalu default ke satuan kecil untuk pembelian
                    ' Alasan: Pembelian selalu dicatat dalam satuan kecil (per unit)
                    Dim satuan As String = satuanKecil
                    Dim isi As Integer = If(IsDBNull(dataBarang("ISI_UMUM_KECIL")), 1, Convert.ToInt32(dataBarang("ISI_UMUM_KECIL")))

                    Dim namaBarang As String = If(IsDBNull(dataBarang("NAMA_BARANG")), "", dataBarang("NAMA_BARANG").ToString())
                    ' [FP1-T14-4] HAPUS: Logika penentuan satuan dari barcode
                    ' Barcode tidak menentukan satuan untuk pembelian, selalu kecil

                    Dim hargaBeli As Decimal = ModuleAngka.ParseDecimal(dataBarang("HARGA_BELI"))
                    Dim hargaBeliTerakhir As Decimal = ModuleAngka.ParseDecimal(dataBarang("HARGA_BELI_TERAKHIR"))

                    ' Handle Isi = 0 → 1
                    isi = Math.Max(1, isi)

                    DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value = hargaBeliTerakhir * isi
                    DgvData.Rows(e.RowIndex).Cells("HargaBeliSatKecil").Value = hargaBeli / isi

                    ' Pastikan satuan valid sebelum diset ke cell
                    If Not String.IsNullOrEmpty(satuan) Then
                        If Not comboCell.Items.Contains(satuan) Then
                            comboCell.Items.Add(satuan)
                        End If
                        DgvData.Rows(e.RowIndex).Cells("Satuan").Value = satuan
                    End If

                    isi = Math.Max(1, isi)
                    DgvData.Rows(e.RowIndex).Cells("isi").Value = isi

                    DgvData.Rows(e.RowIndex).Cells("qty").Value = qtyValue
                    DgvData.Rows(e.RowIndex).Cells("Average").Value = hargaBeli / isi
                    DgvData.Rows(e.RowIndex).Cells("HargaSebelumnya").Value = hargaBeliTerakhir

                    ' ✅ Set read-only setelah data diisi (samakan dengan FormJual)
                    DgvData.Rows(e.RowIndex).Cells("Nama").ReadOnly = True
                    DgvData.Rows(e.RowIndex).Cells("Nama").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
                    DgvData.Rows(e.RowIndex).Cells("Nama").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)

                    If Not ModulHakAkses.SettingIzinkanSatuanBerbeda Then
                        For barisatas As Integer = 0 To DgvData.RowCount - 1
                            For barisbawah As Integer = barisatas + 1 To DgvData.RowCount - 1
                                Dim idAtas As Object = DgvData.Rows(barisatas).Cells("Id").Value
                                Dim idBawah As Object = DgvData.Rows(barisbawah).Cells("Id").Value

                                If idAtas IsNot Nothing AndAlso idBawah IsNot Nothing AndAlso idBawah.Equals(idAtas) Then
                                    Dim qtyLama As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(barisatas).Cells("qty").Value)
                                    Dim qtyBaru As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(barisbawah).Cells("qty").Value)

                                    ' Menambahkan qty ke baris atas
                                    DgvData.Rows(barisatas).Cells("qty").Value = qtyLama + qtyBaru

                                    ' Menghitung ulang QtySat agar akurat (Sesuai FormJual)
                                    Dim isiAtas As Integer = Math.Max(1, ModuleAngka.ParseInteger(DgvData.Rows(barisatas).Cells("Isi").Value, 1))
                                    Dim qtySatBaru As Decimal = (qtyLama + qtyBaru) * isiAtas
                                    DgvData.Rows(barisatas).Cells("QtySat").Value = qtySatBaru

                                    If Not DgvData.Rows(barisbawah).IsNewRow Then
                                        DgvData.Rows.RemoveAt(barisbawah)
                                    Else
                                        ' Kosongkan baris IsNewRow yang berisi duplikat
                                        DgvData.Rows(barisbawah).Cells("Id").Value = Nothing
                                        DgvData.Rows(barisbawah).Cells("Nama").Value = Nothing
                                        DgvData.Rows(barisbawah).Cells("Qty").Value = Nothing
                                        DgvData.Rows(barisbawah).Cells("Satuan").Value = Nothing
                                        DgvData.Rows(barisbawah).Cells("Isi").Value = Nothing
                                        DgvData.Rows(barisbawah).Cells("Hargabeli").Value = Nothing
                                        DgvData.Rows(barisbawah).Cells("QtySat").Value = Nothing
                                        DgvData.Rows(barisbawah).Cells("Totalharga").Value = Nothing
                                    End If


                                    UpdateSemuaTotal()
                                    ' Guard: CurrentCell bisa Nothing setelah EndEdit
                                    If DgvData.CurrentCell Is Nothing Then Return
                                    SetupFocusToGrid()
                                    Exit Sub
                                End If
                            Next
                        Next
                    End If

                    UpdateSemuaTotal()
                    ' Guard: CurrentCell bisa Nothing setelah EndEdit
                    If DgvData.CurrentCell Is Nothing Then Return
                    SetupFocusToGrid()
                Else
                    ' Barang tidak ditemukan — kosongkan nama
                    DgvData.Rows(e.RowIndex).Cells("nama").Value = ""

                    ' Cleanup row + refocus hanya untuk input yang pola scan barcode (tanpa huruf/bintang)
                    Dim isScanLikeInput As Boolean =
                        _konteksLstBarang = "DGV" AndAlso
                        Not namaBarangValue.Any(AddressOf Char.IsLetter) AndAlso
                        Not namaBarangValue.Contains("*"c) AndAlso
                        namaBarangValue.Length >= BARCODE_MIN_LENGTH

                    If isScanLikeInput Then
                        ' Jika row hasil scan tidak punya Id, jangan biarkan jadi row kosong permanen
                        Dim idNow As String = Convert.ToString(DgvData.Rows(e.RowIndex).Cells("Id").Value).Trim()

                        If String.IsNullOrEmpty(idNow) Then
                            If Not DgvData.Rows(e.RowIndex).IsNewRow Then
                                DgvData.Rows.RemoveAt(e.RowIndex)
                            End If

                            ' Kembalikan fokus ke IsNewRow yang tersedia
                            For i As Integer = 0 To DgvData.Rows.Count - 1
                                If DgvData.Rows(i).IsNewRow Then
                                    DgvData.CurrentCell = DgvData(1, i)
                                    Me.ActiveControl = DgvData
                                    DgvData.BeginInvoke(New Action(Sub()
                                                                       If DgvData.CurrentCell IsNot Nothing Then
                                                                           DgvData.BeginEdit(True)
                                                                           DgvData.EditingControl?.Focus()
                                                                       End If
                                                                   End Sub))
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If
            Else
                ' Barang tidak ditemukan atau nama kosong - biarkan kosong, jangan tampilkan pesan
                DgvData.Rows(e.RowIndex).Cells("nama").Value = ""
            End If
        End If

        If e.ColumnIndex = 5 Then
            Dim hargaBeliValue As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value)
            Dim hargaLama As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(e.RowIndex).Cells("Average").Value)
            Dim isiValue As Integer = ModuleAngka.ParseInteger(DgvData.Rows(e.RowIndex).Cells("Isi").Value, defaultValue:=1)
            ' Average adalah harga per satuan kecil — kalikan Isi untuk bandingkan dengan Hargabeli
            Dim hargaLamaPerSatuan As Decimal = hargaLama * isiValue

            ' Validasi nilai Harga Beli
            ' Hanya tampilkan pesan jika kolom ID tidak kosong (barang sudah ada)
            If hargaBeliValue <= 0 AndAlso DgvData.Rows(e.RowIndex).Cells("Id").Value IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(DgvData.Rows(e.RowIndex).Cells("Id").Value.ToString()) Then
                MessageBox.Show("Harga beli harus lebih besar dari 0.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                DgvData.Rows(e.RowIndex).Cells("Hargabeli").Value = 0
            ElseIf hargaBeliValue = hargaLamaPerSatuan Then
                ' Harga beli sama dengan average (sudah disesuaikan isi) — tidak ada perubahan, hitung ulang baris saja
                UpdatePerhitunganPerBaris(DgvData.Rows(e.RowIndex))
            Else
                ' Gunakan fungsi perhitungan baru yang konsisten
                UpdatePerhitunganPerBaris(DgvData.Rows(e.RowIndex))

                ' Hanya update harga jual jika harga beli berubah
                If ModulHakAkses.SettingBeliOtomatisUpdateHargaJual AndAlso hargaBeliValue <> hargaLamaPerSatuan Then
                    Dim QtySbl As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(e.RowIndex).Cells("QtySebelumnya").Value)
                    Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(e.RowIndex).Cells("Qty").Value)
                    Dim qtySatValue As Decimal = qtyValue * isiValue
                    Dim hargaBeliPerSatuan As Decimal = hargaBeliValue / isiValue


                    With TambahBarang
                        .LblHeaderForm.Text = "EDIT HARGA JUAL DARI PEMBELIAN"
                        .GBInput1.Visible = False
                        .GBInput4.Visible = False
                        .GBInput.Enabled = False
                        .GBInput5.Visible = False
                        .PanelInfoRubahHarga.Visible = True
                        .BtnTambahKategori.Visible = False
                        .BtnTambahSupliyer.Visible = False
                        .BtnTambahSatuan.Visible = False
                        .CBManual.Visible = False
                        .BtnBaru.Visible = False
                        '.BackColor = Color.DarkCyan
                        .Size = New Size(816, 705)
                        TambahBarang.Tampilkategori()
                        TambahBarang.TampilSatuan()
                        TambahBarang.Tampilsupliyer()
                        .TxtKode.Text = DgvData.Rows(e.RowIndex).Cells("Id").Value
                        .LblQtySbl.Text = QtySbl.ToString("N0")
                        .LblQtyBaru.Text = qtySatValue.ToString("N0")
                        .LblRpBaru.Text = hargaBeliPerSatuan.ToString("N2")
                        .LblRpLama.Text = hargaLama.ToString("N2")

                        If ModulHakAkses.SettingMetodeUpdateHargaBeli = "Metode Average (Rata - Rata)" Then
                            .LblJenisUpdate.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli & " " & ModulHakAkses.SettingAverageHargaBerdasarkanStok
                            .LblMetode.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                            .LblJenis.Text = ModulHakAkses.SettingAverageHargaBerdasarkanStok
                        Else
                            .LblJenisUpdate.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                            .LblMetode.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                        End If
                        .ShowDialog()
                    End With

                End If
            End If
        End If



        '========================== Qty
        If e.ColumnIndex = 2 Then
            Dim rowIndex As Integer = e.RowIndex
            Dim qtyCell As DataGridViewCell = DgvData.Rows(rowIndex).Cells("QTY")

            ' Validasi: kolom ID tidak boleh kosong sebelum memproses qty
            If DgvData.Rows(rowIndex).Cells("Id").Value Is Nothing OrElse String.IsNullOrWhiteSpace(DgvData.Rows(rowIndex).Cells("Id").Value.ToString()) Then
                MessageBox.Show("Nama barang harus diisi terlebih dahulu sebelum mengisi qty.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                qtyCell.Value = 1
                Exit Sub
            End If

            ' Pastikan nilai sel tidak null atau kosong
            If qtyCell.Value Is Nothing OrElse IsDBNull(qtyCell.Value) OrElse String.IsNullOrWhiteSpace(qtyCell.Value.ToString()) Then
                MessageBox.Show("Kolom QTY tidak boleh kosong. Mohon masukkan angka.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                qtyCell.Value = 1
            End If

            ' Gunakan ModuleAngka.ParseDecimal untuk parsing angka
            Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(qtyCell.Value.ToString())
            If qtyValue <= 0 Then
                MessageBox.Show("Qty harus lebih besar dari 0.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                qtyCell.Value = 1
            End If

            ' Optimalisasi refresh DataGridView
            DgvData.SuspendLayout()
            DgvData.ResumeLayout()
        End If

        UpdateSemuaTotal()
    End Sub


    Private Sub DgvData_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles DgvData.DataError
        ' ✅ PHASE 2: Suppress non-critical errors (especially ComboBox value issues)
        If e.Exception IsNot Nothing Then
            ' Jika error terkait ComboBox Value, abaikan secara silent atau log ke debug
            If e.Exception.Message.Contains("DataGridViewComboBoxCell") OrElse TypeOf e.Exception Is ArgumentException Then
                e.ThrowException = False
                e.Cancel = False ' ✅ Ubah ke False agar tidak me-revert perubahan row
                Return
            End If
        End If

        ' Untuk error lainnya, tampilkan pesan jika diperlukan
        Dim errorMessage As String = "Kesalahan data: " & e.Exception.Message & Environment.NewLine &
                                     "Periksa baris yang disorot dan perbaiki."

        ' MessageBox.Show(errorMessage, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)

        ' Menyorot baris yang bermasalah
        If e.RowIndex >= 0 Then
            For Each cell As DataGridViewCell In DgvData.Rows(e.RowIndex).Cells
                cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowError, ModuleTheme.D_DgvRowError)
            Next
        End If

        e.Cancel = True
    End Sub

    Private Sub DgvData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles DgvData.KeyDown
        If e.KeyCode = Keys.Delete Then
            If DgvData.SelectedCells.Count > 0 Then
                Dim selectedCell As DataGridViewCell = DgvData.SelectedCells(0)

                ' Periksa apakah sel yang dipilih berada di kolom "Nama"
                If selectedCell.ColumnIndex = DgvData.Columns("Nama").Index Then
                    Dim rowIndex As Integer = selectedCell.RowIndex

                    ' Periksa apakah nilai di kolom "Nama" tidak kosong
                    If Not String.IsNullOrEmpty(DgvData.Rows(rowIndex).Cells("Nama").Value.ToString()) Then
                        ' Hapus baris jika nilai di kolom "Nama" tidak kosong
                        DgvData.Rows.RemoveAt(rowIndex)
                        ' Setelah menghapus baris, pastikan untuk menghilangkan seleksi agar tidak ada baris yang dipilih secara default.
                        DgvData.ClearSelection()
                        ' [FP1-T14b-1] TAMBAH: SetupFocusToGrid setelah hapus baris untuk UX keyboard
                        SetupFocusToGrid()
                    Else
                        MessageBox.Show("Klik kanan pada baris yang tidak kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If
        End If

        UpdateSemuaTotal()
    End Sub

    Private Sub DgvData_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DgvData.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DgvData.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    Private Sub DgvData_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles DgvData.EditingControlShowing
        ' Blok NamaBarang — pasang handler untuk inline search via ListBox
        If DgvData.CurrentCell.ColumnIndex = 1 AndAlso DgvData.Columns(1).HeaderText = "Nama Barang" Then
            ' KRITIS: skip re-attach saat sedang pindah ke ListBox
            ' DGV BeginEdit ulang karena fokus kembali — biarkan handler lama tetap aktif
            If _sedangPindahKeLstBarang Then Return

            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.None
                If _dgvEditingTextBox IsNot Nothing Then
                    RemoveHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                    ' [FP1-T06-2] HAPUS: RemoveHandler untuk DgvNamaBarang_KeyDown dan DgvNamaBarang_PreviewKeyDown
                End If
                _dgvEditingTextBox = autoText
                AddHandler _dgvEditingTextBox.TextChanged, AddressOf DgvNamaBarang_TextChanged
                ' [FP1-T06-2] HAPUS: AddHandler untuk DgvNamaBarang_KeyDown dan DgvNamaBarang_PreviewKeyDown
                PosisikanLstBarangDiBawahSel()
            End If
        Else
            If Not LstBarang.Focused Then
                LstBarang.Visible = False
                LstBarang.Items.Clear()
            End If
        End If

        ' Periksa apakah kolom yang saat ini sedang diedit adalah kolom yang berisi ComboBox
        If DgvData.CurrentCell.ColumnIndex = 3 Then
            Dim comboBox As ComboBox = TryCast(e.Control, ComboBox)

            ' Hapus penanganan event SelectedIndexChanged jika ada
            RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged

            ' Tambahkan penanganan event SelectedIndexChanged ke ComboBox
            AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
        End If

    End Sub

    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox As ComboBox = DirectCast(sender, ComboBox)

        ' Dapatkan sel saat ini yang sedang diedit
        Dim cell As DataGridViewComboBoxCell = TryCast(DgvData.CurrentCell, DataGridViewComboBoxCell)
        If cell Is Nothing Then Return

        Dim selectedItemId As String = cell.OwningRow.Cells("Id").Value?.ToString()
        If String.IsNullOrEmpty(selectedItemId) Then
            MessageBox.Show("ID Barang tidak valid!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim isiValue As Decimal = 1
        Dim hargaBeli As Decimal = 0
        Dim hargaBeliterakhir As Decimal = 0
        Dim found As Boolean = False
        Dim selectedIdx As Integer = comboBox.SelectedIndex

        Using cmd As New MySqlCommand("SELECT HARGA_BELI, HARGA_BELI_TERAKHIR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = @ItemId", conn)
            cmd.Parameters.AddWithValue("@ItemId", selectedItemId)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    found = True
                    ' Ambil nilai berdasarkan pilihan di ComboBox
                    Select Case selectedIdx
                        Case 0
                            isiValue = Math.Max(1, ModuleAngka.ParseDecimal(rd("ISI_UMUM_KECIL")))
                        Case 1
                            isiValue = Math.Max(1, ModuleAngka.ParseDecimal(rd("ISI_UMUM_SEDANG")))
                        Case Else
                            isiValue = Math.Max(1, ModuleAngka.ParseDecimal(rd("ISI_UMUM_BESAR")))
                    End Select

                    ' Konversi nilai harga beli
                    hargaBeli = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    hargaBeliterakhir = ModuleAngka.ParseDecimal(rd("HARGA_BELI_TERAKHIR"))
                End If
            End Using
        End Using

        If found Then
            ' Update UI setelah reader ditutup
            cell.OwningRow.Cells("Isi").Value = Math.Max(1, isiValue)

            ' Dapatkan indeks baris
            Dim rowIndex As Integer = DgvData.CurrentCell.RowIndex

            ' Hitung nilai lainnya
            Dim isiQty As Decimal = ModuleAngka.ParseDecimal(DgvData("isi", rowIndex).Value)
            If isiQty = 0 Then isiQty = 1
            Dim qty As Decimal = ModuleAngka.ParseDecimal(DgvData("qty", rowIndex).Value)
            If qty = 0 Then qty = 1

            ' Menghitung HPP dan HPPAverage
            Dim HPP As Decimal = hargaBeliterakhir * isiQty
            Dim HPPAverage As Decimal = hargaBeli * isiQty

            ' Update sel di DataGridView
            DgvData("Hargabeli", rowIndex).Value = HPP
            DgvData("HargaBeliSatKecil", rowIndex).Value = hargaBeliterakhir  ' harga per satuan terkecil = HARGA_BELI_TERAKHIR
            DgvData("QtySat", rowIndex).Value = isiQty * qty
            DgvData("Totalharga", rowIndex).Value = HPP * qty

            ' Panggil metode untuk memperbarui total
            UpdateSemuaTotal()
        Else
            MessageBox.Show("Satuan barang dan atau harga jual belum di input !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: BUTTON HANDLERS
    ' ═══════════════════════════════════════════════════════════════════
#Region "BUTTON HANDLERS"

    Private Sub BtnKeluarForm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluarForm.Click
        If GBBayar.Visible Then
            Tekanbatal()
        ElseIf TxtNama.Text <> "" Then
            TxtNama.Clear()
        Else
            ' Menambahkan pertanyaan apakah akan keluar
            Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                Close()
            End If
        End If
    End Sub


    Private Sub DgvData_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DgvData.CellMouseUp
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' Periksa apakah sel yang diklik kanan ada
            Dim cell As DataGridViewCell = DgvData.Rows(e.RowIndex).Cells("Nama")
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


    Private Sub HapusToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HapusToolStripMenuItem.Click
        Call Hapusbaris()
        ' [FP1-T14b-2] TAMBAH: SetupFocusToGrid setelah hapus baris untuk UX keyboard
        SetupFocusToGrid()
    End Sub

    Private Sub EditHargaJualToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditHargaJualToolStripMenuItem.Click
        If DgvData.SelectedCells.Count > 0 Then
            Dim rowIndex As Integer = DgvData.SelectedCells(0).RowIndex
            Dim qtyValue As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(rowIndex).Cells("Qty").Value)
            Dim isiValue As Integer = ModuleAngka.ParseInteger(DgvData.Rows(rowIndex).Cells("isi").Value, defaultValue:=1)
            Dim hargaBeliValue As Decimal = If(isiValue = 0, 0D, ModuleAngka.ParseDecimal(DgvData.Rows(rowIndex).Cells("Hargabeli").Value) / isiValue)
            Dim qtySatValue As Decimal = qtyValue * isiValue

            DgvData.Rows(rowIndex).Cells("QtySat").Value = qtySatValue
            ' hargaBeliValue sudah hasil dari Hargabeli/isiValue, jadi ini adalah harga per satuan kecil
            DgvData.Rows(rowIndex).Cells("HargaBeliSatKecil").Value = hargaBeliValue
            DgvData.Rows(rowIndex).Cells("Totalharga").Value = hargaBeliValue * qtyValue

            Dim hargaLama As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(rowIndex).Cells("Average").Value)
            Dim QtySbl As Decimal = ModuleAngka.ParseDecimal(DgvData.Rows(rowIndex).Cells("QtySebelumnya").Value)

            With TambahBarang
                .LblHeaderForm.Text = "EDIT HARGA JUAL DARI PEMBELIAN"
                .GBInput1.Visible = False
                .GBInput4.Visible = False
                .GBInput.Enabled = False
                .GBInput5.Visible = False
                .PanelInfoRubahHarga.Visible = True
                .BtnTambahKategori.Visible = False
                .BtnTambahSupliyer.Visible = False
                .BtnTambahSatuan.Visible = False
                .CBManual.Visible = False
                .BtnBaru.Visible = False
                '.BackColor = Color.DarkCyan
                .Size = New Size(816, 705)
                TambahBarang.Tampilkategori()
                TambahBarang.TampilSatuan()
                TambahBarang.Tampilsupliyer()
                .TxtKode.Text = DgvData.Rows(rowIndex).Cells("Id").Value
                .LblQtySbl.Text = ModuleAngka.FormatAngka(QtySbl)
                .LblQtyBaru.Text = ModuleAngka.FormatAngka(qtySatValue)
                .LblRpBaru.Text = ModuleAngka.FormatAngka(hargaBeliValue)
                .LblRpLama.Text = ModuleAngka.FormatAngka(hargaLama)

                If ModulHakAkses.SettingMetodeUpdateHargaBeli = "Metode Average (Rata - Rata)" Then
                    .LblJenisUpdate.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli & " " & ModulHakAkses.SettingAverageHargaBerdasarkanStok
                    .LblMetode.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                    .LblJenis.Text = ModulHakAkses.SettingAverageHargaBerdasarkanStok
                Else
                    .LblJenisUpdate.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                    .LblMetode.Text = ModulHakAkses.SettingMetodeUpdateHargaBeli
                End If
                .ShowDialog()
                .TxtHArgaJUalUmumKecil.Select()
            End With

        End If
    End Sub

    ''' <summary>Refresh stok baris yang sedang dipilih.</summary>
    Private Sub RefreshStokBarisIniToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshStokBarisIniToolStripMenuItem.Click
        If DgvData.CurrentCell IsNot Nothing Then
            RefreshStokBaris(DgvData.CurrentCell.RowIndex)
        End If
    End Sub

    ''' <summary>Refresh stok semua baris yang sudah terisi.</summary>
    Private Sub RefreshStokSemuaBarisToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshStokSemuaBarisToolStripMenuItem.Click
        RefreshStokSemuaBaris()
    End Sub

    ' ── Diskon TextChanged — dua arah: persen ↔ Rp ──────────────────────
    Private Sub TxtDiskon_TextChanged(sender As Object, e As EventArgs) Handles TxtDiskonRupiah.TextChanged, TxtDiskonPersen.TextChanged
        If sender Is TxtDiskonRupiah Then
            HitungDiskon("diskonrupiah")
        ElseIf sender Is TxtDiskonPersen Then
            HitungDiskon("diskonpersen")
        End If
    End Sub

    Private Sub HitungDiskon(sumber As String)
        If isUpdatingDiskon Then Exit Sub
        isUpdatingDiskon = True

        Try
            Dim subtotalItem As Decimal = _subtotalBarang
            Dim diskonPersen As Decimal = ModuleAngka.ParseDecimal(TxtDiskonPersen.Text)
            Dim diskonRupiah As Decimal = ModuleAngka.ParseDecimal(TxtDiskonRupiah.Text)

            Select Case sumber.ToLower()
                Case "diskonpersen"
                    diskonPersen = Math.Min(diskonPersen, 100)
                    diskonRupiah = Math.Round(subtotalItem * diskonPersen / 100, 0)
                    ' TextBox input — format plain (InvariantCulture)
                    TxtDiskonRupiah.Text = diskonRupiah.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
                Case "diskonrupiah"
                    diskonPersen = If(subtotalItem = 0, 0, Math.Round((diskonRupiah / subtotalItem) * 100, 2))
                    TxtDiskonPersen.Text = diskonPersen.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
            End Select

            ' Label display — format Indonesia
            LblDiskonRupiah.Text = "Rp. " & ModuleAngka.FormatAngka(diskonRupiah)

            HitungGrandTotalBeli()
        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungGrandTotalBeli (TextChanged): " & ex.Message)
            ' Log error jika perlu, tapi untuk UI biasanya cukup abaikan atau reset ke 0
        Finally
            isUpdatingDiskon = False
        End Try
    End Sub

    ' ── PPN TextChanged — dua arah: persen ↔ Rp ─────────────────────────
    Private Sub TxtPpn_TextChanged(sender As Object, e As EventArgs) Handles TxtPpnRupiah.TextChanged, TxtPpnPersen.TextChanged
        If sender Is TxtPpnRupiah Then
            HitungPajak("pajakrupiah")
        ElseIf sender Is TxtPpnPersen Then
            HitungPajak("pajakpersen")
        End If
    End Sub

    Private Sub HitungPajak(sumber As String)
        If isUpdatingPajak Then Exit Sub
        isUpdatingPajak = True

        Try
            ' PPN dihitung dari subtotal setelah diskon (sama seperti penjualan)
            Dim subtotalItem As Decimal = _subtotalBarang
            Dim diskonRupiah As Decimal = ModuleAngka.ParseDecimal(TxtDiskonRupiah.Text)
            Dim dasarPPN As Decimal = subtotalItem - diskonRupiah

            Dim pajakPersen As Decimal = ModuleAngka.ParseDecimal(TxtPpnPersen.Text)
            Dim pajakRupiah As Decimal = ModuleAngka.ParseDecimal(TxtPpnRupiah.Text)

            Select Case sumber.ToLower()
                Case "pajakpersen"
                    pajakPersen = Math.Min(pajakPersen, 100)
                    pajakRupiah = Math.Round(dasarPPN * pajakPersen / 100, 0)
                    TxtPpnRupiah.Text = pajakRupiah.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
                Case "pajakrupiah"
                    pajakPersen = If(dasarPPN = 0, 0, Math.Round((pajakRupiah / dasarPPN) * 100, 2))
                    TxtPpnPersen.Text = pajakPersen.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
            End Select

            ' Label display — format Indonesia
            LblPpnRupiah.Text = "Rp. " & ModuleAngka.FormatAngka(pajakRupiah)

            HitungGrandTotalBeli()
        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungGrandTotalBeli (TextChanged Qty): " & ex.Message)
            ' Abaikan error parsing
        Finally
            isUpdatingPajak = False
        End Try
    End Sub

    ' ── Biaya kirim TextChanged ───────────────────────────────────────────
    Private Sub TxtBiayaKirim_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtBiayaKirim.TextChanged
        Try
            Dim biayaKirim As Decimal = ModuleAngka.ParseDecimal(TxtBiayaKirim.Text)
            ' Label display — format Indonesia
            LblBiayaKirim.Text = "Rp. " & ModuleAngka.FormatAngka(biayaKirim)
            HitungGrandTotalBeli()
        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungGrandTotalBeli (TextChanged BiayaKirim): " & ex.Message)
            ' Abaikan error
        End Try
    End Sub

    ''' <summary>Hitung grand total = subtotalItem - diskon + ppn + biayaKirim dan update semua display.</summary>
    Private Sub HitungGrandTotalBeli()
        Try
            ' Sync variabel komponen tambahan dari UI dulu
            SyncKomponenTambahanDariUI()
            ' Lalu hitung grand total via fungsi utama yang juga update _grandTotalPembelian
            HitungGrandTotalPembelian()
        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungGrandTotalBeli: " & ex.Message)
        End Try
    End Sub

    Private Sub TxtNominalBayarTunai_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtNominalBayarTunai.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub


    Private Sub Form_Pembelian_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F8
                ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
                If DgvData.IsCurrentCellInEditMode Then
                    DgvData.EndEdit()
                End If
                Tekanbayar()
            Case Keys.F2
                TxtNamaSupplier.Focus()
                TxtNamaSupplier.Text = "a"
                TxtNamaSupplier.SelectionStart = TxtNamaSupplier.Text.Length
            Case Keys.F3
                BtnSupliyer.PerformClick()
            Case Keys.F4
                BtnBarang.PerformClick()
            Case Keys.F5
                BtnSettingPrinter.PerformClick()
            Case Keys.F6
                If IsModeTambahPembelian Then Tekantahan()
            Case Keys.F7
                If IsModeTambahPembelian Then Tekanpanggil()
            Case Keys.Escape
                If GBBayar.Visible Then
                    Tekanbatal()
                ElseIf TxtNama.Text <> "" Then
                    TxtNama.Clear()
                Else
                    ' Menambahkan pertanyaan apakah akan keluar
                    Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Close()
                    End If
                End If
            Case Keys.F9
                ' Hide GBBayar
                If GBBayar.Visible Then
                    Tekanbatal()
                End If
            Case Keys.F10
                If GBBayar.Visible Then
                    SimpanTransaksi()
                End If
            Case Keys.F11
                ' Validasi: pastikan GBBayar visible
                If GBBayar.Visible Then
                    CmbAkunTunai.Select()
                    CmbAkunTunai.DroppedDown = True
                End If
            Case Keys.F12
                ' Validasi: pastikan GBBayar visible
                If GBBayar.Visible Then
                    CmbAkunTransfer.Select()
                    CmbAkunTransfer.DroppedDown = True
                End If
        End Select
    End Sub

    Private Sub BtnBayar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBayar.Click
        ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            DgvData.EndEdit()
        End If
        Tekanbayar()
    End Sub

    Private Sub BtnBarang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBarang.Click
        ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            DgvData.EndEdit()
        End If
        Tekanbarang()
    End Sub

    Private Sub BtnSupliyer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSupliyer.Click
        ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            DgvData.EndEdit()
        End If
        Tekansupliyer()
    End Sub

    Private Sub BtnSimpann_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpann.Click
        SimpanTransaksi()
    End Sub

    Private Sub BtnBatal_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBatal.Click
        Tekanbatal()
    End Sub

    Private Sub BtnTahan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTahan.Click
        ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            DgvData.EndEdit()
        End If
        Tekantahan()
    End Sub

    Private Sub BtnPanggil_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPanggil.Click
        ' Pastikan untuk keluar dari mode edit jika ada sel yang sedang dalam mode edit
        If DgvData.IsCurrentCellInEditMode Then
            DgvData.EndEdit()
        End If
        Tekanpanggil()
    End Sub

    Public Sub Tekanbayar()
        ' Cek apakah supplier belum dipilih
        If String.IsNullOrEmpty(TxtNamaSupplier.Text) AndAlso Not ModulHakAkses.SettingIzinkanBeliTanpaSupplier Then
            ' Tampilkan semua supplier di listSupplier agar user bisa langsung memilih
            TampilkanSemuaSupplier()
            TxtNamaSupplier.Focus()
            TxtNamaSupplier.Text = "a"
            TxtNamaSupplier.SelectionStart = TxtNamaSupplier.Text.Length
            Exit Sub
        End If

        ' Cek apakah belum ada transaksi pembelian
        If (_grandTotalPembelian = 0) AndAlso Not ModulHakAkses.SettingIzinkanNominalBeliNol OrElse DgvData.RowCount = 0 Then
            MessageBox.Show("Belum ada transaksi Pembelian", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            SetupFocusToGrid()
            Exit Sub
        End If

        If ModulHakAkses.SettingIzinkanBeliRugi Then
            If Cekjualrugi() Then
                ' Ada barang yang merugi, keluar dari fungsi atau lakukan tindakan yang sesuai
                Return
            End If
        End If


        CenterPanelBayar()
        GBBayar.Visible = True

        If IsModeTambahPembelian Then
            If ModulHakAkses.SettingLangsungIsiNominalTotal Then
                TxtNominalBayarTunai.Text = TxtGrandTotalPembelian.Text
            Else
                TxtNominalBayarTunai.Text = ""
            End If
        End If

        ' ✅ Hitung ulang TxtKembali agar selalu tepat saat panel bayar dibuka
        HitungUlangKembali()

        TxtNominalBayarTunai.Select()
        TxtNominalBayarTunai.SelectAll()

    End Sub

    ''' <summary>
    ''' Hitung ulang sisa pembayaran saat panel bayar dibuka.
    ''' Dipanggil setiap kali panel bayar dibuka (tambah maupun edit)
    ''' agar nilai tidak stale dari transaksi sebelumnya.
    ''' Lewat HitungPembayaran agar _sisaHutang, _kembalian, dan LblStatusPembayaran ikut terupdate.
    ''' </summary>
    Private Sub HitungUlangKembali()
        Try
            ' Pastikan _grandTotalPembelian sudah terisi
            If _grandTotalPembelian = 0 Then
                _grandTotalPembelian = ModuleAngka.ParseDecimal(TxtGrandTotalPembelian.Text)
            End If
            ' Lewat HitungPembayaran agar semua variabel (_sisaHutang, _kembalian,
            ' _statusPembayaran) dan UI (TxtKembaliHutang, LblStatusPembayaran) ikut terupdate
            HitungPembayaran()
        Catch ex As Exception
            Debug.WriteLine("[ERROR] HitungUlangKembali: " & ex.Message)
        End Try
    End Sub

    Private Sub CenterPanelBayar()
        Dim x As Integer = (ClientSize.Width - GBBayar.Width) \ 2
        Dim y As Integer = (Me.ClientSize.Height - GBBayar.Height) \ 2
        GBBayar.Location = New Point(x, y)
    End Sub

    ''' <summary>
    ''' Validasi harga beli vs harga jual dengan 3 level:
    ''' 1. Rugi Kritis (BLOCK): Rugi di SEMUA level harga jual → tidak boleh simpan
    ''' 2. Rugi Umum + Partai (WARNING): Rugi di kedua jenis → konfirmasi user
    ''' 3. Rugi Salah Satu (INFO): Rugi di satu jenis saja → beri info, bisa lanjut
    ''' Return True jika tidak boleh lanjut (BLOCK atau user pilih Cancel)
    ''' </summary>
    Public Function Cekjualrugi() As Boolean
        For Each dgvRow As DataGridViewRow In DgvData.Rows
            If Not dgvRow.IsNewRow AndAlso dgvRow.Cells("Id").Value IsNot Nothing AndAlso dgvRow.Cells("Id").Value.ToString() <> "" Then
                Dim kodeBarangValue As String = dgvRow.Cells("Id").Value.ToString()
                Dim namaBarang As String = dgvRow.Cells("Nama").Value.ToString()
                Dim hargaBeliTotal As Decimal = ModuleAngka.ParseDecimal(dgvRow.Cells("Totalharga").Value)
                Dim qtysat As Decimal = ModuleAngka.ParseDecimal(dgvRow.Cells("QtySat").Value)

                ' Mengumpulkan informasi barang menggunakan SP validasi v2.0.0
                Using cmd As New MySqlCommand("CALL sp_val_pembelian_harga_beli_vs_jual(@ID_BARANG, @HARGA_BELI, @QTY, @RUGI_KRITIS, @RUGI_UMUM, @RUGI_PARTAI, @HARGA_JUAL_MIN, @JUAL_UMUM, @JUAL_PARTAI)", conn)
                    cmd.Parameters.AddWithValue("@ID_BARANG", kodeBarangValue)
                    cmd.Parameters.AddWithValue("@HARGA_BELI", hargaBeliTotal)
                    cmd.Parameters.AddWithValue("@QTY", qtysat)

                    Dim pRugiKritis = cmd.Parameters.Add("@RUGI_KRITIS", MySqlDbType.Bit)
                    pRugiKritis.Direction = ParameterDirection.Output

                    Dim pRugiUmum = cmd.Parameters.Add("@RUGI_UMUM", MySqlDbType.Bit)
                    pRugiUmum.Direction = ParameterDirection.Output

                    Dim pRugiPartai = cmd.Parameters.Add("@RUGI_PARTAI", MySqlDbType.Bit)
                    pRugiPartai.Direction = ParameterDirection.Output

                    Dim pHargaJualMin = cmd.Parameters.Add("@HARGA_JUAL_MIN", MySqlDbType.Decimal)
                    pHargaJualMin.Direction = ParameterDirection.Output

                    Dim pJualUmum = cmd.Parameters.Add("@JUAL_UMUM", MySqlDbType.Decimal)
                    pJualUmum.Direction = ParameterDirection.Output

                    Dim pJualPartai = cmd.Parameters.Add("@JUAL_PARTAI", MySqlDbType.Decimal)
                    pJualPartai.Direction = ParameterDirection.Output

                    cmd.ExecuteNonQuery()

                    Dim rugiKritis As Boolean = Convert.ToBoolean(pRugiKritis.Value)
                    Dim rugiUmum As Boolean = Convert.ToBoolean(pRugiUmum.Value)
                    Dim rugiPartai As Boolean = Convert.ToBoolean(pRugiPartai.Value)
                    Dim hargaJualMin As Decimal = ModuleAngka.ParseDecimal(pHargaJualMin.Value)
                    Dim hargajualUmum As Decimal = ModuleAngka.ParseDecimal(pJualUmum.Value)
                    Dim hargajualPartai As Decimal = ModuleAngka.ParseDecimal(pJualPartai.Value)

                    ' ── Level 1: RUGI KRITIS (BLOCK) ─────────────────────────
                    ' Rugi di SEMUA level harga jual → tidak boleh simpan
                    If rugiKritis Then
                        Dim errorMessage As String = "TIDAK BISA SIMPAN!" & vbCrLf & vbCrLf &
                                                     "Barang: " & namaBarang & vbCrLf &
                                                     "Harga beli lebih tinggi dari SEMUA harga jual yang tersedia." & vbCrLf & vbCrLf &
                                                     "Harga beli: " & hargaBeliTotal.ToString("N0") & vbCrLf &
                                                     "Harga jual minimum: " & (hargaJualMin * qtysat).ToString("N0") & vbCrLf & vbCrLf &
                                                     "Silakan ubah harga beli atau harga jual terlebih dahulu."
                        MessageBox.Show(errorMessage, "Rugi Kritis - Tidak Boleh Simpan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        dgvRow.Selected = True
                        For Each cell As DataGridViewCell In dgvRow.Cells
                            cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowError, ModuleTheme.D_DgvRowError)
                        Next
                        Return True
                    End If

                    ' ── Level 2: RUGI UMUM + PARTAI (WARNING) ─────────────────
                    ' Rugi di kedua jenis → konfirmasi user
                    If rugiUmum AndAlso rugiPartai Then
                        Dim warningMessage As String = "PERINGATAN!" & vbCrLf & vbCrLf &
                                                       "Barang: " & namaBarang & vbCrLf &
                                                       "Harga beli lebih tinggi dari harga jual Umum DAN Partai." & vbCrLf & vbCrLf &
                                                       "Harga beli: " & hargaBeliTotal.ToString("N0") & vbCrLf &
                                                       "Harga jual Umum: " & hargajualUmum.ToString("N0") & vbCrLf &
                                                       "Harga jual Partai: " & hargajualPartai.ToString("N0") & vbCrLf & vbCrLf &
                                                       "Apakah Anda yakin ingin melanjutkan?"
                        Dim result As DialogResult = MessageBox.Show(warningMessage, "Peringatan Harga Pembelian", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        If result = DialogResult.No Then
                            dgvRow.Selected = True
                            For Each cell As DataGridViewCell In dgvRow.Cells
                                cell.Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowPeringatan, ModuleTheme.D_DgvRowPeringatan)
                            Next
                            Return True
                        End If
                    End If

                    ' ── Level 3: RUGI SALAH SATU (INFO) ───────────────────────
                    ' Rugi di satu jenis saja → beri info, bisa lanjut
                    If rugiUmum AndAlso Not rugiPartai Then
                        Dim infoMessage As String = "INFORMASI:" & vbCrLf & vbCrLf &
                                                    "Barang: " & namaBarang & vbCrLf &
                                                    "Harga beli lebih tinggi dari harga jual Umum, " &
                                                    "tapi masih untung jika dijual dengan harga Partai." & vbCrLf & vbCrLf &
                                                    "Harga beli: " & hargaBeliTotal.ToString("N0") & vbCrLf &
                                                    "Harga jual Umum: " & hargajualUmum.ToString("N0") & " (RUGI)" & vbCrLf &
                                                    "Harga jual Partai: " & hargajualPartai.ToString("N0") & " (UNTUNG)"
                        MessageBox.Show(infoMessage, "Informasi Harga Pembelian", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ElseIf rugiPartai AndAlso Not rugiUmum Then
                        Dim infoMessage As String = "INFORMASI:" & vbCrLf & vbCrLf &
                                                    "Barang: " & namaBarang & vbCrLf &
                                                    "Harga beli lebih tinggi dari harga jual Partai, " &
                                                    "tapi masih untung jika dijual dengan harga Umum." & vbCrLf & vbCrLf &
                                                    "Harga beli: " & hargaBeliTotal.ToString("N0") & vbCrLf &
                                                    "Harga jual Umum: " & hargajualUmum.ToString("N0") & " (UNTUNG)" & vbCrLf &
                                                    "Harga jual Partai: " & hargajualPartai.ToString("N0") & " (RUGI)"
                        MessageBox.Show(infoMessage, "Informasi Harga Pembelian", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using

                ' Kembalikan warna default
                For Each cell As DataGridViewCell In dgvRow.Cells
                    cell.Style.BackColor = Color.Empty
                Next
            End If
        Next

        Return False
    End Function

    Public Sub Tekanbarang()
        Using f As New TambahBarang()
            f.LblHeaderForm.Text = "T A M B A H   B A R A N G"
            f.ShowDialog()
        End Using
    End Sub

    Public Sub Tekansupliyer()
        TambahSupliyer.ShowDialog()
        AmbilDataSupplier()
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: SIMPAN TRANSAKSI (SAVE TRANSACTION)
    ' ═══════════════════════════════════════════════════════════════════
#Region "SIMPAN TRANSAKSI"

    Public Sub SimpanTransaksi()
        Dim bayarTunai = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        Dim bayarTransfer = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim totalBayar = bayarTunai + bayarTransfer
        Dim jenisTrans = TxtJenisTrans.Text
        Dim isBayarKosong = totalBayar = 0

        ' Konfirmasi jika belum bayar
        If isBayarKosong Then
            Dim pesan As DialogResult = MessageBox.Show(
        "Nominal Pembayaran (Tunai/Transfer) belum diisi. Lanjut sebagai hutang semua?" & vbCrLf &
        "Tekan OK jika lanjut, Cancel jika batal.",
        "Perhatian Penting", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If pesan <> DialogResult.OK Then Exit Sub
        End If

        ' Validasi supplier jika status hutang
        If LblStatusPembayaran.Text = "Belum Lunas" Then
            If String.IsNullOrWhiteSpace(TxtNamaSupplier.Text) Then
                ' Tampilkan semua supplier di listSupplier agar user bisa langsung memilih
                TampilkanSemuaSupplier()
                TxtNamaSupplier.Focus()
                TxtNamaSupplier.Text = "a"
                TxtNamaSupplier.SelectionStart = TxtNamaSupplier.Text.Length
                Exit Sub
            End If

            ' Cek jatuh tempo untuk hutang
            If DtpJatuhTempo.Value <= DtpTanggalPembelian.Value Then
                MessageBox.Show(
        "Tanggal jatuh tempo harus lebih besar dari tanggal transaksi." & vbCrLf &
        "Tanggal Transaksi: " & DtpTanggalPembelian.Value.ToString("dd/MM/yyyy") & vbCrLf &
        "Tanggal Jatuh Tempo: " & DtpJatuhTempo.Value.ToString("dd/MM/yyyy"),
        "Peringatan",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning
)
                DtpJatuhTempo.Select()
                Exit Sub
            End If
        End If


        Cursor = Cursors.WaitCursor

        If Not ModulHakAkses.SettingIzinkanTanggalLampau AndAlso jenisTrans = "TambahPembelian" AndAlso String.IsNullOrWhiteSpace(draftPembelianAktif) Then
            DtpTanggalPembelian.Value = DateTime.Now
            NomorBeli()
        ElseIf jenisTrans = "TambahPembelian" AndAlso String.IsNullOrWhiteSpace(draftPembelianAktif) Then
            NomorBeli()
        End If

        ' ── Cek duplikat faktur untuk mode draft ────────────────────────────
        ' Skenario: nomor faktur draft (draftPembelianAktif terisi) sudah dipakai
        ' oleh transaksi lain sejak draft disimpan (kasir lain, atau tanggal berbeda).
        '
        ' Fix (Opsi A* - FormPembelian):
        ' Berbeda dengan FormJual, NomorBeli() tidak punya guard draftPembelianAktif,
        ' sehingga bisa dipanggil langsung tanpa pola sementara-kosongkan-kembalikan.
        ' draftPembelianAktif TIDAK diubah agar HapusDraftPembelian menghapus
        ' draft lama (nomor lama) dengan benar setelah commit.
        ' ────────────────────────────────────────────────────────────────────
        If jenisTrans = "TambahPembelian" AndAlso Not String.IsNullOrWhiteSpace(draftPembelianAktif) Then
            Using cmdCekDuplikat As New MySqlCommand(
                "SELECT ID_PEMBELIAN FROM pembelian WHERE ID_PEMBELIAN = ?", conn)
                cmdCekDuplikat.Parameters.AddWithValue("@ID", TxtIdPembelian.Text)
                Dim hasilCek As Object = cmdCekDuplikat.ExecuteScalar()
                If hasilCek IsNot Nothing Then
                    ' Nomor draft sudah dipakai transaksi lain → generate nomor baru.
                    ' draftPembelianAktif tetap menunjuk ke nomor lama (untuk HapusDraftPembelian).
                    NomorBeli()
                End If
            End Using
        End If

        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        Try
            ' Jika edit: hapus dulu transaksi lama
            ' ReversalSaldoAkunDariFaktur dipanggil di dalam Hapusbelanja → HapusPembelian
            If Not IsModeTambahPembelian Then
                ' ========================================
                ' START: Audit Trail - Edit Pembelian
                ' ========================================
                ModuleAuditTrail.CatatAudit(TxtIdPembelian.Text, "EDIT", "Pembelian", ket:="[KRITIS] Edit pembelian", trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Edit Pembelian
                ' ========================================
                Hapusbelanja(transaction)
            End If

            ' Audit dictionaries
            Dim auditDGV As New Dictionary(Of String, Decimal)()
            Dim auditHistory As New Dictionary(Of String, Decimal)()
            Dim auditDetail As New Dictionary(Of String, Decimal)()
            Dim auditStokDelta As New Dictionary(Of String, Decimal)()

            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() <> "" Then
                    Dim kodeA As String = row.Cells("Id").Value.ToString()
                    Dim qtyA As Decimal = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
                    If auditDGV.ContainsKey(kodeA) Then auditDGV(kodeA) += qtyA Else auditDGV(kodeA) = qtyA
                End If
            Next

            ' Simpan header, detail, history
            SimpanPembelian(transaction)
            SimpanPembelianDetail(transaction, auditDetail)
            HistoryBarang(transaction, auditHistory)

            ' Jurnal — semua kasus (tunai, transfer, diskon, PPN, biaya kirim, hutang) ditangani di Simpanjurnal
            Dim jD As Decimal = 0D, jK As Decimal = 0D
            Simpanjurnal(transaction, jD, jK)

            ' Update stok & saldo
            For Each row As DataGridViewRow In DgvData.Rows
                If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() <> "" Then
                    Dim kodeD As String = row.Cells("Id").Value.ToString()
                    Dim stokSebelum As Decimal = BacaStokSaatIni(kodeD, LblLokasiBarang.Text, transaction)
                    HitungStokPerubahan(kodeD, transaction)
                    Dim stokSesudah As Decimal = BacaStokSaatIni(kodeD, LblLokasiBarang.Text, transaction)
                    Dim delta As Decimal = stokSesudah - stokSebelum
                    If auditStokDelta.ContainsKey(kodeD) Then auditStokDelta(kodeD) += delta Else auditStokDelta(kodeD) = delta
                End If
            Next


            ' Update saldo semua akun yang terlibat — incremental delta
            UpdateSaldoAkunDeltaDariFaktur(TxtIdPembelian.Text, transaction)
            UpdateHutangSupliyer(LblKodeSupplier.Text, transaction)

            ' Hapus draft jika ada
            If Not String.IsNullOrWhiteSpace(draftPembelianAktif) Then
                HapusDraftPembelian(transaction, draftPembelianAktif)
            End If

            AuditStokTransaksi(TxtIdPembelian.Text, "Pembelian", auditDGV, auditHistory, auditDetail, auditStokDelta, transaction)

            transaction.Commit()

        Catch ex As OperationCanceledException
            transaction.Rollback()
            ' Dibatalkan oleh pengguna — tidak perlu pesan error
            Exit Sub
        Catch ex As Exception
            Debug.WriteLine("[ERROR] SimpanPembelian: " & ex.Message)
            transaction.Rollback()
            MessageBox.Show("Transaksi dibatalkan karena kesalahan:" & vbCrLf & ex.Message, "Gagal Simpan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        Finally
            Cursor = Cursors.Default
        End Try

        JumlahTahanPembelian()

        ' CETAK DILUAR TRY utama agar tetap lanjut meski gagal
        Try
            Select Case BacaPengaturanPrinter("Beli", "CetakOtomatis", "IYA").Trim().ToUpper()
                Case "IYA"
                    LakukanCetakPembelian(TxtIdPembelian.Text)
                Case "SELALU TANYA"
                    If MessageBox.Show("Apakah Anda ingin mencetak Pembelian?", "Konfirmasi Cetak",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        LakukanCetakPembelian(TxtIdPembelian.Text)
                    End If
                Case "TAMPILKAN DI MONITOR"
                    ModulePrinterBeli.CetakPembelian(TxtIdPembelian.Text, "Tampilkan di Monitor")
            End Select
        Catch ex As Exception
            Debug.WriteLine("[ERROR] Cetak pembelian: " & ex.Message)
            MessageBox.Show("Gagal mencetak pembelian. Anda bisa mencetak ulang nanti." & vbCrLf &
                        "Detail: " & ex.Message, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            If IsModeTambahPembelian Then
                Kondisiawal()
                SetupFocusToGrid() ' _formSudahSiap sudah True di sini — aman dipanggil langsung
            Else
                Me.Close()
            End If
        End Try

    End Sub

    Private Sub TxtGrandTotalPembelian_TextChanged(sender As Object, e As EventArgs) Handles TxtGrandTotalPembelian.TextChanged
        Dim totalStlPajak As Decimal = ModuleAngka.ParseDecimal(TxtGrandTotalPembelian.Text)
        LblGrandTotalPembelian.Text = "Rp. " & ModuleAngka.FormatRupiah(totalStlPajak)
    End Sub

    Private Sub TxtNominalBayarTunai_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalBayarTunai.TextChanged
        ' Gunakan fungsi perhitungan baru
        HitungPembayaran()

        ' Update label display — selalu, tidak bergantung pada GBBayar.Visible
        Dim bayarTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        LblBayarTunai.Text = "Rp. " & ModuleAngka.FormatRupiah(bayarTunai)

        ' Validasi: pastikan GBBayar visible
        If Not GBBayar.Visible Then Return

        ' Validasi: pembayaran tidak boleh melebihi grand total
        Dim bayarTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim totalBayar As Decimal = bayarTunai + bayarTransfer

        If totalBayar > _grandTotalPembelian Then
            MessageBox.Show("Pembayaran melebihi total belanja! Nilai akan dikurangi otomatis.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ' Kurangi input tunai agar total = grand total
            Dim sisaKuota As Decimal = _grandTotalPembelian - bayarTransfer
            TxtNominalBayarTunai.Text = ModuleAngka.FormatUntukInput(sisaKuota)
            TxtNominalBayarTunai.Select()
            TxtNominalBayarTunai.SelectAll()
            HitungPembayaran() ' Recalculate setelah dikurangi
            ' Ambil nilai terbaru setelah dikurangi
            bayarTunai = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
            LblBayarTunai.Text = "Rp. " & ModuleAngka.FormatRupiah(bayarTunai)
        End If
    End Sub

    Private Sub TxtNominalBayarTransfer_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNominalBayarTransfer.TextChanged
        ' Gunakan fungsi perhitungan baru
        HitungPembayaran()

        ' Update label display — selalu, tidak bergantung pada GBBayar.Visible
        Dim bayarTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        LblBayarTransfer.Text = "Rp. " & ModuleAngka.FormatRupiah(bayarTransfer)

        ' Validasi: pastikan GBBayar visible
        If Not GBBayar.Visible Then Return

        ' Validasi: pembayaran tidak boleh melebihi grand total
        Dim bayarTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        Dim totalBayar As Decimal = bayarTunai + bayarTransfer

        If totalBayar > _grandTotalPembelian Then
            MessageBox.Show("Pembayaran melebihi total belanja! Nilai akan dikurangi otomatis.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ' Kurangi input transfer agar total = grand total
            Dim sisaKuota As Decimal = _grandTotalPembelian - bayarTunai
            TxtNominalBayarTransfer.Text = ModuleAngka.FormatUntukInput(sisaKuota)
            TxtNominalBayarTransfer.Select()
            TxtNominalBayarTransfer.SelectAll()
            HitungPembayaran() ' Recalculate setelah dikurangi
            ' Ambil nilai terbaru setelah dikurangi
            bayarTransfer = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
            LblBayarTransfer.Text = "Rp. " & ModuleAngka.FormatRupiah(bayarTransfer)
        End If

        ' Atur lebar GBBayar berdasarkan apakah ada pembayaran transfer
        If bayarTransfer > 0 Then
            GBBayar.Size = New Size(833, 344)
        Else
            GBBayar.Size = New Size(509, 344)
        End If
    End Sub

    Private Sub TxtKembaliHutang_TextChanged(sender As Object, e As EventArgs) Handles TxtKembaliHutang.TextChanged
        Dim nominalKembali As Decimal = ModuleAngka.ParseDecimal(TxtKembaliHutang.Text)
        LblKembalianHutang.Text = "Rp. " & ModuleAngka.FormatRupiah(nominalKembali)
    End Sub

    Public Sub Tekantahan()
        If Not IsModeTambahPembelian Then
            MessageBox.Show("Mode edit tidak dapat menggunakan fitur Tahan untuk menghindari bentrok data.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If String.IsNullOrWhiteSpace(TxtIdPembelian.Text) Then
            MessageBox.Show("Nomor faktur wajib diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtIdPembelian.Select()
            Return
        End If

        Dim adaItemValid As Boolean = False
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString().Trim() <> "" Then
                adaItemValid = True
                Exit For
            End If
        Next

        If Not adaItemValid Then
            MessageBox.Show("Belum ada barang yang diinput.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim transaction As MySqlTransaction = Nothing
        Try
            transaction = conn.BeginTransaction()

            If IsModeTambahPembelian AndAlso String.IsNullOrWhiteSpace(draftPembelianAktif) Then
                NomorBeli()
                
                ' ==============================================================================
                ' MULTI-ADMIN FIX: Pastikan Nomor Faktur untuk Draft BARU tidak bertabrakan!
                ' ==============================================================================
                If String.IsNullOrWhiteSpace(draftPembelianAktif) AndAlso TxtIdPembelian.Text.Length >= 9 Then
                    Dim isValidUnique As Boolean = False
                    While Not isValidUnique
                        Dim checkDraft As New MySqlCommand("SELECT COUNT(*) FROM pembelian_ditahan WHERE ID_PEMBELIAN = @Faktur", conn, transaction)
                        checkDraft.Parameters.AddWithValue("@Faktur", TxtIdPembelian.Text)
                        Dim existingDraftCount = Convert.ToInt32(checkDraft.ExecuteScalar())

                        If existingDraftCount > 0 Then
                            ' Nomor sudah dipakai draft lain! Cari urutan maksimum dan tambah 1
                            Dim idxPrefix As Integer = TxtIdPembelian.Text.LastIndexOf("-"c) + 7
                            If idxPrefix > 0 AndAlso idxPrefix < TxtIdPembelian.Text.Length Then
                                Dim prefixTgl As String = TxtIdPembelian.Text.Substring(0, idxPrefix)
                                Dim currentUrut As Integer = 0
                                Integer.TryParse(TxtIdPembelian.Text.Substring(idxPrefix), currentUrut)
                                
                                Dim checkMaxDraft As New MySqlCommand("SELECT MAX(ID_PEMBELIAN) FROM pembelian_ditahan WHERE ID_PEMBELIAN LIKE @Prefix", conn, transaction)
                                checkMaxDraft.Parameters.AddWithValue("@Prefix", prefixTgl & "%")
                                Dim maxDraft = checkMaxDraft.ExecuteScalar()
                                
                                If maxDraft IsNot DBNull.Value AndAlso maxDraft IsNot Nothing Then
                                    Dim maxUrut As Integer = 0
                                    Dim numStr As String = maxDraft.ToString()
                                    If numStr.Length >= idxPrefix AndAlso Integer.TryParse(numStr.Substring(idxPrefix), maxUrut) Then
                                        If maxUrut > currentUrut Then
                                            currentUrut = maxUrut
                                        End If
                                    End If
                                End If
                                
                                currentUrut += 1
                                TxtIdPembelian.Text = prefixTgl & currentUrut.ToString("D4")
                            Else
                                isValidUnique = True ' Fallback jika format tidak dikenali
                            End If
                        Else
                            isValidUnique = True
                        End If
                    End While
                End If
                ' ==============================================================================
            End If

            HapusDraftPembelian(transaction, TxtIdPembelian.Text)
            SimpanPembelianDitahanHeader(transaction)
            SimpanPembelianDitahanDetail(transaction)

            transaction.Commit()
            MessageBox.Show("Pembelian berhasil ditahan sementara.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

            If IsModeTambahPembelian Then
                Kondisiawal()
                SetupFocusToGrid() ' _formSudahSiap sudah True di sini — aman dipanggil langsung
            Else
                Me.Close()
            End If
        Catch ex As Exception
            Debug.WriteLine("[ERROR] SimpanPembelianDitahan: " & ex.Message)
            transaction?.Rollback()
            MessageBox.Show("Gagal menyimpan pembelian ditahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        JumlahTahanPembelian()
    End Sub

    Public Sub Tekanpanggil()
        If Not IsModeTambahPembelian Then
            MessageBox.Show("Mode edit tidak dapat memanggil transaksi ditahan untuk menghindari bentrok data.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using frm As New FormPembelianDitahan()
            If frm.ShowDialog() = DialogResult.OK AndAlso Not String.IsNullOrWhiteSpace(frm.SelectedFaktur) Then
                MuatDraftPembelian(frm.SelectedFaktur)
            End If
        End Using
        JumlahTahanPembelian()
    End Sub

    Public Sub JumlahTahanPembelian()
        Dim jumlah As Integer = 0
        Using cmd As New MySqlCommand("SELECT COUNT(ID_PEMBELIAN) FROM pembelian_ditahan", conn)
            Dim val = cmd.ExecuteScalar()
            If val IsNot Nothing AndAlso val IsNot DBNull.Value Then
                Integer.TryParse(val.ToString(), jumlah)
            End If
        End Using
        BtnPanggil.Text = " Panggil (F7) [" & jumlah.ToString() & "]"
    End Sub

    Private Sub SimpanPembelianDitahanHeader(ByVal transaction As MySqlTransaction)
        Dim sql As String =
            "INSERT INTO pembelian_ditahan (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, ID_USER, ID_KOMPUTER) " &
            "VALUES (@ID_PEMBELIAN, @ID_SUPPLIER, @NAMA_SUPLIYER, @NOTA_PEMBELIAN, @TGL_BELI, @LOKASI, @JENIS_BAYAR, @GRAND_TOTAL_BELI, @TOTAL_QTY, @TOTAL_BARANG, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(sql, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_PEMBELIAN", TxtIdPembelian.Text)
            cmd.Parameters.AddWithValue("@ID_SUPPLIER", If(String.IsNullOrWhiteSpace(LblKodeSupplier.Text), DBNull.Value, CType(LblKodeSupplier.Text, Object)))
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", If(String.IsNullOrWhiteSpace(TxtNamaSupplier.Text), DBNull.Value, CType(TxtNamaSupplier.Text, Object)))
            cmd.Parameters.AddWithValue("@NOTA_PEMBELIAN", TxtNotaPembelian.Text)
            cmd.Parameters.AddWithValue("@TGL_BELI", DtpTanggalPembelian.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@JENIS_BAYAR", CmbAkunTunai.Text)
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_BELI", _grandTotalPembelian)
            cmd.Parameters.AddWithValue("@TOTAL_QTY", _totalQtyDalamSatuanTerkecil)
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", _subtotalBarang)
            cmd.Parameters.AddWithValue("@ID_USER", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub SimpanPembelianDitahanDetail(ByVal transaction As MySqlTransaction)
        Dim sql As String =
            "INSERT INTO pembelian_ditahan_detail (FAKTUR_BELI, NOTA_BELI, TANGGAL_MASUK, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER, ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL) " &
            "VALUES (@FAKTUR_BELI, @NOTA_BELI, @TANGGAL_MASUK, @LOKASI, @ID_SUPLIYER, @NAMA_SUPLIYER, @ID_BARANG, @NAMA_BARANG, @HARGA_BELI, @HARGA_AVERAGE, @HARGA_BELI_SEBELUMNYA, @QTY, @SATUAN, @ISI_SATUAN, @HARGA_BELI_SATUAN, @QTY_SAT, @TOTAL)"

        For Each row As DataGridViewRow In DgvData.Rows
            If row.IsNewRow OrElse row.Cells("Id").Value Is Nothing OrElse row.Cells("Id").Value.ToString().Trim() = "" Then Continue For

            Using cmd As New MySqlCommand(sql, conn, transaction)
                cmd.Parameters.AddWithValue("@FAKTUR_BELI", TxtIdPembelian.Text)
                cmd.Parameters.AddWithValue("@NOTA_BELI", TxtNotaPembelian.Text)
                cmd.Parameters.AddWithValue("@TANGGAL_MASUK", DtpTanggalPembelian.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                cmd.Parameters.AddWithValue("@ID_SUPLIYER", LblKodeSupplier.Text)
                cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtNamaSupplier.Text)
                cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells("Id").Value.ToString())
                cmd.Parameters.AddWithValue("@NAMA_BARANG", If(row.Cells("Nama").Value, ""))
                cmd.Parameters.AddWithValue("@HARGA_BELI", ModuleAngka.ParseDecimal(row.Cells("Hargabeli").Value))
                cmd.Parameters.AddWithValue("@HARGA_AVERAGE", ModuleAngka.ParseDecimal(row.Cells("Average").Value))
                cmd.Parameters.AddWithValue("@HARGA_BELI_SEBELUMNYA", ModuleAngka.ParseDecimal(row.Cells("HargaSebelumnya").Value))
                cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(row.Cells("Qty").Value))
                cmd.Parameters.AddWithValue("@SATUAN", If(row.Cells("Satuan").Value, ""))
                cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseInteger(row.Cells("Isi").Value, defaultValue:=1))
                cmd.Parameters.AddWithValue("@HARGA_BELI_SATUAN", ModuleAngka.ParseDecimal(row.Cells("HargaBeliSatKecil").Value))
                cmd.Parameters.AddWithValue("@QTY_SAT", ModuleAngka.ParseDecimal(row.Cells("QtySat").Value))
                cmd.Parameters.AddWithValue("@TOTAL", ModuleAngka.ParseDecimal(row.Cells("Totalharga").Value))
                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub

    Private Sub HapusDraftPembelian(ByVal transaction As MySqlTransaction, ByVal faktur As String)
        If String.IsNullOrWhiteSpace(faktur) Then Return

        Using cmd As New MySqlCommand("DELETE FROM pembelian_ditahan_detail WHERE FAKTUR_BELI = @FAKTUR", conn, transaction)
            cmd.Parameters.AddWithValue("@FAKTUR", faktur)
            cmd.ExecuteNonQuery()
        End Using

        Using cmd As New MySqlCommand("DELETE FROM pembelian_ditahan WHERE ID_PEMBELIAN = @FAKTUR", conn, transaction)
            cmd.Parameters.AddWithValue("@FAKTUR", faktur)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub MuatDraftPembelian(ByVal faktur As String)
        DgvData.Rows.Clear()
        TxtIdPembelian.Text = faktur
        draftPembelianAktif = faktur

        Dim queryHeader As String =
            "SELECT ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, ID_USER, ID_KOMPUTER " &
            "FROM pembelian_ditahan WHERE ID_PEMBELIAN = @ID"

        Using cmd As New MySqlCommand(queryHeader, conn)
            cmd.Parameters.AddWithValue("@ID", faktur)
            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    LblKodeSupplier.Text = rd("ID_SUPPLIER").ToString()
                    TxtNamaSupplier.Text = rd("NAMA_SUPLIYER").ToString()
                    TxtNotaPembelian.Text = rd("NOTA_PEMBELIAN").ToString()
                    DtpTanggalPembelian.Value = ModuleAngka.SafeGetValue(Of DateTime)(rd, "TGL_BELI", DateTime.Now)
                    LblLokasiBarang.Text = rd("LOKASI").ToString()
                    CmbAkunTunai.Text = rd("JENIS_BAYAR").ToString()
                    ' Saat panggil draft, pembayaran selalu reset/default.
                    TxtNominalBayarTunai.Text = "0"
                    TxtNominalBayarTransfer.Text = "0"
                    TxtKembaliHutang.Text = "0"
                    LblStatusPembayaran.Text = "Lunas"
                    TxtLogin.Text = rd("ID_USER").ToString()
                    TxtKomputer.Text = rd("ID_KOMPUTER").ToString()
                    DtpJatuhTempo.Value = DtpTanggalPembelian.Value.AddMonths(1)
                Else
                    MessageBox.Show("Data draft tidak ditemukan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If
            End Using
        End Using

        Dim queryDetail As String =
            "SELECT pdd.ID_BARANG, pdd.NAMA_BARANG, pdd.HARGA_BELI, pdd.QTY, pdd.SATUAN, pdd.ISI_SATUAN, pdd.HARGA_BELI_SATUAN, pdd.QTY_SAT, pdd.TOTAL, pdd.HARGA_AVERAGE, pdd.HARGA_BELI_SEBELUMNYA, " &
            "tb.SATUAN_UMUM_KECIL, tb.SATUAN_UMUM_SEDANG, tb.SATUAN_UMUM_BESAR " &
            "FROM pembelian_ditahan_detail pdd " &
            "LEFT JOIN tbl_barang tb ON pdd.ID_BARANG = tb.ID_BARANG " &
            "WHERE pdd.FAKTUR_BELI = @FAKTUR"
        Using cmd As New MySqlCommand(queryDetail, conn)
            cmd.Parameters.AddWithValue("@FAKTUR", faktur)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    Dim baris As DataGridViewRow = DirectCast(DgvData.Rows(DgvData.Rows.Add()), DataGridViewRow)
                    baris.Cells("Id").Value = rd("ID_BARANG").ToString()
                    baris.Cells("Nama").Value = rd("NAMA_BARANG").ToString()
                    baris.Cells("Hargabeli").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    baris.Cells("Qty").Value = ModuleAngka.ParseDecimal(rd("QTY"))
                    baris.Cells("Isi").Value = CInt(Math.Max(1, ModuleAngka.ParseDecimal(rd("ISI_SATUAN"))))
                    baris.Cells("HargaBeliSatKecil").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_SATUAN"))
                    baris.Cells("QtySat").Value = ModuleAngka.ParseDecimal(rd("QTY_SAT"))
                    baris.Cells("Totalharga").Value = ModuleAngka.ParseDecimal(rd("TOTAL"))
                    baris.Cells("Average").Value = ModuleAngka.ParseDecimal(rd("HARGA_AVERAGE"))
                    baris.Cells("HargaSebelumnya").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_SEBELUMNYA"))
                    baris.Cells("QtySebelumnya").Value = ModuleAngka.ParseDecimal(rd("QTY_SAT"))
                    Dim comboCell As DataGridViewComboBoxCell = CType(baris.Cells("Satuan"), DataGridViewComboBoxCell)
                    comboCell.Items.Clear()
                    For Each satNama As String In {ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", ""),
                                                   ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", ""),
                                                   ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")}
                        If satNama <> "" AndAlso Not comboCell.Items.Contains(satNama) Then comboCell.Items.Add(satNama)
                    Next
                    Dim satuanDraft As String = rd("SATUAN").ToString()
                    If satuanDraft <> "" AndAlso Not comboCell.Items.Contains(satuanDraft) Then
                        comboCell.Items.Add(satuanDraft)
                    End If
                    baris.Cells("Satuan").Value = satuanDraft
                End While
            End Using
        End Using

        LblJatuhTempo.Visible = False
        DtpJatuhTempo.Visible = False
        LblPembayaran.Text = "Kembalian :"

        TxtJenisTrans.Text = "TambahPembelian"
        UpdateSemuaTotal()
        AmbilKodeAkun()

        ' Refresh info stok semua baris — stok mungkin berubah sejak draft disimpan
        RefreshStokSemuaBaris()
    End Sub


    Private Sub LakukanCetakPembelian(idPembelian As String)
        If BacaPengaturanPrinter("Beli", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterBeli.TanyaPilihPrinterBeli(idPembelian)
        Else
            ModulePrinterBeli.CetakPembelian(idPembelian)
        End If
    End Sub

    Public Sub Tekanbatal()
        GBBayar.Visible = False
        TxtNominalBayarTunai.Text = "0"
        TxtNominalBayarTransfer.Text = "0"
        LblBayarTunai.Text = "Rp. 0"
        LblBayarTransfer.Text = "Rp. 0"
    End Sub



    Public Sub Hapusbelanja(ByVal transaction As MySqlTransaction)
        ' ========================================
        ' START: Audit Trail - Hapus untuk Edit Belanja
        ' ========================================
        ModuleAuditTrail.CatatAudit(TxtIdPembelian.Text, "EDIT", "Pembelian", trans:=transaction)
        ' ========================================
        ' END: Audit Trail - Hapus untuk Edit Belanja
        ' ========================================

        ModuleHapusTransaksi.HapusPembelian(TxtIdPembelian.Text, LblLokasiBarang.Text, transaction)
    End Sub

    Private Sub SimpanPembelian(ByVal transaction As MySqlTransaction)
        Dim bayarTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim diskonSupplier As Decimal = ModuleAngka.ParseDecimal(TxtDiskonRupiah.Text)
        Dim ppnMasukan As Decimal = ModuleAngka.ParseDecimal(TxtPpnRupiah.Text)
        Dim biayaKirim As Decimal = ModuleAngka.ParseDecimal(TxtBiayaKirim.Text)

        Dim sql As String
        If LblStatusPembayaran.Text = "Lunas" Then
            sql = "INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, STATUS_JUAL, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER, NOMINAL_TRANSFER, KODE_AKUN_TF, NAMA_AKUN_TF, DISKON_SUPPLIER, PPN_MASUKAN, BIAYA_KIRIM) " &
                  "VALUES (@ID_PEMBELIAN, @ID_SUPPLIER, @NAMA_SUPLIYER, @NOTA_PEMBELIAN, @TGL_BELI, @LOKASI, @JENIS_BAYAR, @GRAND_TOTAL_BELI, @TOTAL_QTY, @TOTAL_BARANG, @PEMBAYARAN, @STATUS_JUAL, @STATUS_TRANSAKSI_BELI, @ID_USER, @ID_KOMPUTER, @NOMINAL_TRANSFER, @KODE_AKUN_TF, @NAMA_AKUN_TF, @DISKON_SUPPLIER, @PPN_MASUKAN, @BIAYA_KIRIM)"
        Else
            sql = "INSERT INTO pembelian (ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, TOTAL_QTY, TOTAL_BARANG, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, STATUS_JUAL, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER, NOMINAL_TRANSFER, KODE_AKUN_TF, NAMA_AKUN_TF, DISKON_SUPPLIER, PPN_MASUKAN, BIAYA_KIRIM) " &
                  "VALUES (@ID_PEMBELIAN, @ID_SUPPLIER, @NAMA_SUPLIYER, @NOTA_PEMBELIAN, @TGL_BELI, @LOKASI, @JENIS_BAYAR, @GRAND_TOTAL_BELI, @TOTAL_QTY, @TOTAL_BARANG, @PEMBAYARAN, @TAGIHAN, @JATUH_TEMPO, @STATUS_JUAL, @STATUS_TRANSAKSI_BELI, @ID_USER, @ID_KOMPUTER, @NOMINAL_TRANSFER, @KODE_AKUN_TF, @NAMA_AKUN_TF, @DISKON_SUPPLIER, @PPN_MASUKAN, @BIAYA_KIRIM)"
        End If

        Using cmd As New MySqlCommand(sql, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_PEMBELIAN", TxtIdPembelian.Text)
            cmd.Parameters.AddWithValue("@ID_SUPPLIER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtNamaSupplier.Text)
            cmd.Parameters.AddWithValue("@NOTA_PEMBELIAN", TxtNotaPembelian.Text)
            cmd.Parameters.AddWithValue("@TGL_BELI", DtpTanggalPembelian.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@JENIS_BAYAR", CmbAkunTunai.Text)
            cmd.Parameters.AddWithValue("@GRAND_TOTAL_BELI", _grandTotalPembelian)
            cmd.Parameters.AddWithValue("@TOTAL_QTY", _totalQtyDalamSatuanTerkecil)
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", _subtotalBarang)
            cmd.Parameters.AddWithValue("@PEMBAYARAN", ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text))
            cmd.Parameters.AddWithValue("@NOMINAL_TRANSFER", bayarTransfer)
            cmd.Parameters.AddWithValue("@KODE_AKUN_TF", If(bayarTransfer > 0, TxtKodeAkunTransfer.Text, ""))
            cmd.Parameters.AddWithValue("@NAMA_AKUN_TF", If(bayarTransfer > 0, CmbAkunTransfer.Text, ""))
            cmd.Parameters.AddWithValue("@DISKON_SUPPLIER", diskonSupplier)
            cmd.Parameters.AddWithValue("@PPN_MASUKAN", ppnMasukan)
            cmd.Parameters.AddWithValue("@BIAYA_KIRIM", biayaKirim)

            If LblStatusPembayaran.Text = "Lunas" Then
                cmd.Parameters.AddWithValue("@STATUS_JUAL", "TERBAYAR")
            Else
                cmd.Parameters.AddWithValue("@TAGIHAN", ModuleAngka.ParseDecimal(TxtKembaliHutang.Text))
                cmd.Parameters.AddWithValue("@JATUH_TEMPO", DtpJatuhTempo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@STATUS_JUAL", "TERHUTANG")
            End If

            cmd.Parameters.AddWithValue("@STATUS_TRANSAKSI_BELI", LblStatusPembayaran.Text)
            cmd.Parameters.AddWithValue("@ID_USER", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using

        ' ── Catat hutang timbul di hutang_detail (Requirement 3) ────────────────
        ' Hanya untuk pembelian kredit (sisa hutang > 0)
        Dim idPembelian As String = TxtIdPembelian.Text
        Dim grandTotalBeli As Decimal = _grandTotalPembelian
        Dim sisaHutang As Decimal = ModuleAngka.ParseDecimal(TxtKembaliHutang.Text)

        If sisaHutang > 0 Then
            Using cmdTimbul As New MySqlCommand(
                "INSERT INTO hutang_detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA, " &
                "JENIS, TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, " &
                "PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
                "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_BELI, @KODE, @NAMA, " &
                "'BELI', @TANGGAL_BELI, @TOTAL_HUTANG, 0, 0, @HUTANG, @JATUH_TEMPO, " &
                "0, 'Belum Lunas', @ID_USER, @ID_KOMPUTER)", conn, transaction)
                cmdTimbul.Parameters.AddWithValue("@ID_BAYAR", idPembelian)
                cmdTimbul.Parameters.AddWithValue("@TANGGAL_BAYAR", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdTimbul.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                cmdTimbul.Parameters.AddWithValue("@ID_BELI", idPembelian)
                cmdTimbul.Parameters.AddWithValue("@KODE", LblKodeSupplier.Text)
                cmdTimbul.Parameters.AddWithValue("@NAMA", TxtNamaSupplier.Text)
                cmdTimbul.Parameters.AddWithValue("@TANGGAL_BELI", DtpTanggalPembelian.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdTimbul.Parameters.AddWithValue("@TOTAL_HUTANG", grandTotalBeli)
                cmdTimbul.Parameters.AddWithValue("@HUTANG", sisaHutang)
                cmdTimbul.Parameters.AddWithValue("@JATUH_TEMPO", DtpJatuhTempo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdTimbul.Parameters.AddWithValue("@ID_USER", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
                cmdTimbul.Parameters.AddWithValue("@ID_KOMPUTER", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
                cmdTimbul.ExecuteNonQuery()
            End Using
        End If
    End Sub



    Private Sub SimpanPembelianDetail(ByVal transaction As MySqlTransaction, ByRef auditDetail As Dictionary(Of String, Decimal))
        For Each row As DataGridViewRow In DgvData.Rows
            If row.IsNewRow OrElse row.Cells("Id").Value Is Nothing OrElse row.Cells("Id").Value.ToString().Trim() = "" Then Continue For

            ' Ambil data dari row
            Dim IdBarang = row.Cells("Id").Value.ToString()
            Dim NamaBarang = row.Cells("Nama").Value?.ToString()
            Dim HargaBeli = ModuleAngka.ParseDecimal(row.Cells("HargaBeli").Value)
            Dim HargaAverage = ModuleAngka.ParseDecimal(row.Cells("Average").Value)
            Dim HargaSebelumnya = ModuleAngka.ParseDecimal(row.Cells("HargaSebelumnya").Value)
            Dim Qty = ModuleAngka.ParseDecimal(row.Cells("Qty").Value)
            Dim Satuan = row.Cells("Satuan").Value?.ToString()
            Dim Isi = SafeInt(row.Cells("Isi").Value, 1)
            Dim HargaBeliSatKecil = ModuleAngka.ParseDecimal(row.Cells("HargaBeliSatKecil").Value)
            Dim QtySat = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
            Dim Total = ModuleAngka.ParseDecimal(row.Cells("Totalharga").Value)

            ' Simpan detail pembelian
            Using cmd As New MySqlCommand("
            INSERT INTO pembelian_detail 
            (FAKTUR_BELI, NOTA_BELI, TANGGAL_MASUK, LOKASI, ID_SUPLIYER, NAMA_SUPLIYER, ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL, ID_USER, ID_KOMPUTER) 
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", conn, transaction)

                With cmd.Parameters
                    .AddWithValue("@FAKTUR_BELI", TxtIdPembelian.Text)
                    .AddWithValue("@NOTA_BELI", TxtNotaPembelian.Text)
                    .AddWithValue("@TANGGAL_MASUK", DtpTanggalPembelian.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    .AddWithValue("@LOKASI", LblLokasiBarang.Text)
                    .AddWithValue("@ID_SUPLIYER", LblKodeSupplier.Text)
                    .AddWithValue("@NAMA_SUPLIYER", TxtNamaSupplier.Text)
                    .AddWithValue("@ID_BARANG", IdBarang)
                    .AddWithValue("@NAMA_BARANG", NamaBarang)
                    .AddWithValue("@HARGA_BELI", HargaBeli)
                    .AddWithValue("@HARGA_AVERAGE", HargaAverage)
                    .AddWithValue("@HARGA_BELI_SEBELUMNYA", HargaSebelumnya)
                    .AddWithValue("@QTY", Qty)
                    .AddWithValue("@SATUAN", Satuan)
                    .AddWithValue("@ISI_SATUAN", Isi)
                    .AddWithValue("@HARGA_BELI_SATUAN", HargaBeliSatKecil)
                    .AddWithValue("@QTY_SAT", QtySat)
                    .AddWithValue("@TOTAL", Total)
                    .AddWithValue("@ID_USER", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
                    .AddWithValue("@ID_KOMPUTER", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
                End With
                cmd.ExecuteNonQuery()
            End Using

            ' Tentukan nama field stok
            Dim stokField As String = If(LblLokasiBarang.Text = "TOKO", "PEMBELIAN_TOKO", "PEMBELIAN_GUDANG")
            Dim HargaSatuan = If(Isi = 0, 0, HargaBeli / Isi)

            Select Case ModulHakAkses.SettingMetodeUpdateHargaBeli
                Case "Harga Terbaru"
                    UpdateHargaTerbaru(IdBarang, HargaSatuan, QtySat, stokField, transaction)
                Case "Metode Average (Rata - Rata)"
                    UpdateHargaAverage(IdBarang, HargaSatuan, HargaAverage, QtySat, stokField, transaction)
                Case "Tidak Ada"
                    UpdateStokSaja(IdBarang, QtySat, stokField, transaction)
            End Select

            ' Audit C
            If auditDetail.ContainsKey(IdBarang) Then auditDetail(IdBarang) += QtySat Else auditDetail(IdBarang) = QtySat
        Next
    End Sub


    Private Function SafeInt(value As Object, Optional defaultValue As Integer = 0) As Integer
        If IsDBNull(value) OrElse value Is Nothing OrElse Not IsNumeric(value) Then Return defaultValue
        Return Convert.ToInt32(value)
    End Function

    ''' <summary>
    ''' Update harga barang dengan metode Harga Terbaru
    ''' Selain update harga, juga menghitung selisih nilai persediaan dan akumulasi ke _totalSelisihHargaPokok (Requirement 21)
    ''' </summary>
    Private Sub UpdateHargaTerbaru(IdBarang As String, Harga As Decimal, QtySat As Decimal, StokField As String, tr As MySqlTransaction)
        Dim HargaLama As Decimal = 0D
        Dim StokToko As Decimal = 0D
        Dim StokGudang As Decimal = 0D

        ' STEP 1: Baca HARGA_BELI lama dan stok saat ini SEBELUM update
        Using cmd As New MySqlCommand("SELECT HARGA_BELI, STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?", conn, tr)
            cmd.Parameters.AddWithValue("@ID", IdBarang)
            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    HargaLama = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    StokToko = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                    StokGudang = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))
                End If
            End Using
        End Using

        ' STEP 2: Hitung total stok saat ini (berdasarkan setting)
        Dim TotalStokLama = If(ModulHakAkses.SettingAverageHargaBerdasarkanStok = "Toko", StokToko, If(ModulHakAkses.SettingAverageHargaBerdasarkanStok = "Gudang", StokGudang, StokToko + StokGudang))

        ' STEP 3: Hitung selisih nilai persediaan jika harga berubah dan stok > 0
        'If Harga <> HargaLama AndAlso TotalStokLama > 0 Then
        '    Dim selisih = (Harga - HargaLama) * TotalStokLama
        '    _totalSelisihHargaPokok += selisih
        'End If

        ' STEP 4: Update harga barang di tbl_barang
        Dim sql As String = $"UPDATE tbl_barang SET KODE_SUPLIYER = ?, NAMA_SUPLIYER = ?, HARGA_BELI = ?, HARGA_BELI_TERAKHIR = ?, {StokField} = {StokField} + ? WHERE ID_BARANG = ?"
        Using cmd As New MySqlCommand(sql, conn, tr)
            cmd.Parameters.AddWithValue("@KODE_SUPLIYER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtNamaSupplier.Text)
            cmd.Parameters.AddWithValue("@HARGA_BELI", Harga)
            cmd.Parameters.AddWithValue("@HARGA_BELI_TERAKHIR", Harga)
            cmd.Parameters.AddWithValue("@STOK", QtySat)
            cmd.Parameters.AddWithValue("@ID_BARANG", IdBarang)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' Update harga barang dengan metode Average (Rata-Rata)
    ''' Selain update harga, juga menghitung selisih nilai persediaan dan akumulasi ke _totalSelisihHargaPokok (Requirement 21)
    ''' </summary>
    Private Sub UpdateHargaAverage(idBarang As String, hargaBaru As Decimal, hargaLama As Decimal, qtySat As Decimal, stokField As String, tr As MySqlTransaction)
        Dim stokToko As Decimal = 0D
        Dim stokGudang As Decimal = 0D

        ' STEP 1: Baca stok saat ini SEBELUM update
        Using cmd As New MySqlCommand("SELECT STOK_TOKO, STOK_GUDANG FROM tbl_barang WHERE ID_BARANG = ?", conn, tr)
            cmd.Parameters.AddWithValue("@ID", idBarang)
            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    stokToko = ModuleAngka.ParseDecimal(rd("STOK_TOKO"))
                    stokGudang = ModuleAngka.ParseDecimal(rd("STOK_GUDANG"))
                End If
            End Using
        End Using

        Dim totalStokLama = If(ModulHakAkses.SettingAverageHargaBerdasarkanStok = "Toko", stokToko, If(ModulHakAkses.SettingAverageHargaBerdasarkanStok = "Gudang", stokGudang, stokToko + stokGudang))
        Dim totalHargaLama = hargaLama * totalStokLama
        Dim totalHargaBaru = hargaBaru * qtySat
        ' Round ke 4 desimal untuk presisi HPP — mengurangi akumulasi selisih rounding
        ' Contoh: harga 1.550 tidak dibulatkan menjadi 2.000
        Dim hargaAverageBaru = If(totalStokLama + qtySat = 0, hargaBaru, Math.Round((totalHargaLama + totalHargaBaru) / (totalStokLama + qtySat), 4))

        ' STEP 2: Hitung selisih nilai persediaan jika harga berubah dan stok > 0
        'If hargaAverageBaru <> hargaLama AndAlso totalStokLama > 0 Then
        '    Dim selisih = (hargaAverageBaru - hargaLama) * totalStokLama
        '    _totalSelisihHargaPokok += selisih
        'End If

        ' STEP 3: Update harga barang di tbl_barang
        Dim sql As String = $"UPDATE tbl_barang SET KODE_SUPLIYER = ?, NAMA_SUPLIYER = ?, HARGA_BELI = ?, HARGA_BELI_TERAKHIR = ?, {stokField} = {stokField} + ? WHERE ID_BARANG = ?"
        Using cmd As New MySqlCommand(sql, conn, tr)
            cmd.Parameters.AddWithValue("@KODE_SUPLIYER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtNamaSupplier.Text)
            cmd.Parameters.AddWithValue("@HARGA_BELI", hargaAverageBaru)
            cmd.Parameters.AddWithValue("@HARGA_BELI_TERAKHIR", hargaBaru)
            cmd.Parameters.AddWithValue("@STOK", qtySat)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub UpdateStokSaja(idBarang As String, qtySat As Decimal, stokField As String, tr As MySqlTransaction)
        Dim sql As String = $"UPDATE tbl_barang SET KODE_SUPLIYER = ?, NAMA_SUPLIYER = ?, {stokField} = {stokField} + ? WHERE ID_BARANG = ?"
        Using cmd As New MySqlCommand(sql, conn, tr)
            cmd.Parameters.AddWithValue("@KODE_SUPLIYER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", TxtNamaSupplier.Text)
            cmd.Parameters.AddWithValue("@STOK", qtySat)
            cmd.Parameters.AddWithValue("@ID_BARANG", idBarang)
            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction, ByRef auditHistory As Dictionary(Of String, Decimal))
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DgvData.Rows
            If Not row.IsNewRow AndAlso row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() <> "" Then
                Dim querySimpan As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                                            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(querySimpan, conn, transaction)
                    cmd.Parameters.AddWithValue("@FAKTUR", TxtIdPembelian.Text)
                    cmd.Parameters.AddWithValue("@TANGGAL", DtpTanggalPembelian.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@JENIS", "PEMBELIAN")
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasiBarang.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells("Id").Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells("Nama").Value)
                    cmd.Parameters.AddWithValue("@QTY", ModuleAngka.ParseDecimal(row.Cells("Qty").Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells("Satuan").Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", ModuleAngka.ParseInteger(row.Cells("Isi").Value, defaultValue:=1))
                    Dim qtySat As Decimal = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", qtySat)
                    cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(row.Cells("Totalharga").Value))
                    cmd.Parameters.AddWithValue("@ID_USER", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
                    cmd.ExecuteNonQuery()
                End Using

                ' Audit B
                Dim kodeB As String = row.Cells("Id").Value.ToString()
                Dim qtyB As Decimal = ModuleAngka.ParseDecimal(row.Cells("QtySat").Value)
                If auditHistory.ContainsKey(kodeB) Then auditHistory(kodeB) += qtyB Else auditHistory(kodeB) = qtyB
            End If
        Next
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: JURNAL (JOURNAL)
    ' ═══════════════════════════════════════════════════════════════════
#Region "JURNAL"

    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction, ByRef outDebet As Decimal, ByRef outKredit As Decimal)
        ' ═══════════════════════════════════════════════════════════════════
        ' PERHITUNGAN DASAR UNTUK JURNAL
        ' Pola: satu sisi per baris (sama seperti FormJual)
        ' D = K dijaga oleh SP, bukan per baris jurnal
        ' ═══════════════════════════════════════════════════════════════════
        Dim nominalBayarTunai As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTunai.Text)
        Dim nominalBayarTransfer As Decimal = ModuleAngka.ParseDecimal(TxtNominalBayarTransfer.Text)
        Dim diskonSupplier As Decimal = ModuleAngka.ParseDecimal(TxtDiskonRupiah.Text)
        Dim ppnMasukan As Decimal = ModuleAngka.ParseDecimal(TxtPpnRupiah.Text)
        Dim biayaKirim As Decimal = ModuleAngka.ParseDecimal(TxtBiayaKirim.Text)
        Dim statusLunas As Boolean = (LblStatusPembayaran.Text = "Lunas")
        Dim sisaHutang As Decimal = ModuleAngka.ParseDecimal(TxtKembaliHutang.Text)

        ' Kas tunai keluar = input user (nominalBayarTunai)
        ' Validasi sudah ada: bayarTunai + bayarTransfer tidak boleh melebihi grandTotal
        ' Jurnal mencatat sesuai input user untuk menjaga keseimbangan
        Dim kasTunaiKeluar As Decimal = nominalBayarTunai
        Debug.WriteLine("[JURNAL] Kas tunai keluar (input user): " & kasTunaiKeluar.ToString("N0"))

        Dim totalDebet As Decimal = 0D
        Dim totalKredit As Decimal = 0D

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 1: K KAS TUNAI (kas keluar untuk bayar pembelian)
        ' ═══════════════════════════════════════════════════════════════════
        If kasTunaiKeluar > 0 Then
            Dim uraianTunai As String = If(statusLunas AndAlso nominalBayarTransfer = 0,
                                           "Pembelian lunas tunai ke " & TxtNamaSupplier.Text,
                                           "Pembayaran tunai belanja ke " & TxtNamaSupplier.Text)
            InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
                uraianTunai,
                "", "",
                CmbAkunTunai.Text, TxtKodeAkunTunai.Text,
                kasTunaiKeluar)
            totalKredit += kasTunaiKeluar
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 2: K BANK TRANSFER (kas keluar via transfer)
        ' ═══════════════════════════════════════════════════════════════════
        If nominalBayarTransfer > 0 Then
            InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
                "Pembayaran transfer belanja ke " & TxtNamaSupplier.Text & " via " & CmbAkunTransfer.Text,
                "", "",
                CmbAkunTransfer.Text, TxtKodeAkunTransfer.Text,
                nominalBayarTransfer)
            totalKredit += nominalBayarTransfer
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 3: K HUTANG BELANJA (kewajiban yang belum dibayar)
        ' ═══════════════════════════════════════════════════════════════════
        If Not statusLunas AndAlso sisaHutang > 0 Then
            InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
                "Hutang belanja ke " & TxtNamaSupplier.Text & " jatuh tempo " & DtpJatuhTempo.Value.ToString("dd MMMM yyyy"),
                "", "",
                nama_rek_Hutang_Beli, Kode_rek_Hutang_Beli,
                sisaHutang)
            totalKredit += sisaHutang
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 4: K POTONGAN DISKON PEMBELIAN (kontra HPP — diskon dari supplier)
        ' ═══════════════════════════════════════════════════════════════════
        If diskonSupplier > 0 Then
            Dim namaAkunDiskon As String = AmbilNamaAkunDariReferensi("06.05.001", "POTONGAN DISKON PEMBELIAN")
            InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
                "Diskon supplier dari " & TxtNamaSupplier.Text,
                "", "",
                namaAkunDiskon, "06.05.001",
                diskonSupplier)
            totalKredit += diskonSupplier
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 5: D PERSEDIAAN BARANG (nilai barang masuk ke gudang)
        ' Nominal = subtotalItem (nilai murni barang, tanpa diskon/PPN/biaya)
        ' ═══════════════════════════════════════════════════════════════════
        If _subtotalBarang > 0 Then
            InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
                "Persediaan barang masuk dari " & TxtNamaSupplier.Text,
                NAMA_REK_BARANG, KODE_REK_BARANG,
                "", "",
                _subtotalBarang)
            totalDebet += _subtotalBarang
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 6: D PPN MASUKAN (aset pajak yang bisa dikreditkan)
        ' ═══════════════════════════════════════════════════════════════════
        If ppnMasukan > 0 Then
            Dim namaAkunPPN As String = AmbilNamaAkunDariReferensi("01.05.001", "PPN MASUKAN")
            InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
                "PPN Masukan pembelian dari " & TxtNamaSupplier.Text,
                namaAkunPPN, "01.05.001",
                "", "",
                ppnMasukan)
            totalDebet += ppnMasukan
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 7: D BIAYA KIRIM PEMBELIAN (beban angkut pembelian)
        ' ═══════════════════════════════════════════════════════════════════
        If biayaKirim > 0 Then
            Dim namaAkunBiayaKirim As String = AmbilNamaAkunDariReferensi("06.02.001", "BIAYA KIRIM PEMBELIAN")
            InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
                "Biaya kirim pembelian dari " & TxtNamaSupplier.Text,
                namaAkunBiayaKirim, "06.02.001",
                "", "",
                biayaKirim)
            totalDebet += biayaKirim
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' JURNAL 8: Penyesuaian Harga Pokok (Requirement 21) — DINONAKTIFKAN SEMENTARA
        ' Analisis: moving average perpetual tidak memerlukan revaluasi stok lama
        ' Nilai persediaan sudah akurat dari J5 (barang masuk) + rumus average
        ' ═══════════════════════════════════════════════════════════════════
        'If _totalSelisihHargaPokok <> 0 Then
        '    Dim nominalAbs As Decimal = Math.Abs(_totalSelisihHargaPokok)
        '    Dim uraianHP As String = "Penyesuaian nilai persediaan akibat perubahan harga pokok barang"
        '    If _totalSelisihHargaPokok > 0 Then
        '        InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
        '            uraianHP, NAMA_REK_BARANG, KODE_REK_BARANG, "", "", nominalAbs)
        '        totalDebet += nominalAbs
        '    Else
        '        InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
        '            uraianHP, "", "", NAMA_REK_BARANG, KODE_REK_BARANG, nominalAbs)
        '        totalKredit += nominalAbs
        '    End If
        '    If _totalSelisihHargaPokok > 0 Then
        '        Dim namaAkunPenyesuaian As String = AmbilNamaAkunDariReferensi("06.04.002", "PENYESUAIAN HARGA POKOK")
        '        InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
        '            uraianHP, "", "", namaAkunPenyesuaian, "06.04.002", nominalAbs)
        '        totalKredit += nominalAbs
        '    Else
        '        Dim namaAkunPenyesuaian As String = AmbilNamaAkunDariReferensi("06.04.002", "PENYESUAIAN HARGA POKOK")
        '        InsertJurnal(transaction, TxtIdPembelian.Text, TxtNotaPembelian.Text,
        '            uraianHP, namaAkunPenyesuaian, "06.04.002", "", "", nominalAbs)
        '        totalDebet += nominalAbs
        '    End If
        'End If

        ' ═══════════════════════════════════════════════════════════════════
        ' DEBUG SUMMARY
        ' ═══════════════════════════════════════════════════════════════════
        Debug.WriteLine("═══════════════════════════════════════════════════════════════════")
        Debug.WriteLine("DEBUG JURNAL PEMBELIAN - Faktur: " & TxtIdPembelian.Text & " | " & TxtNamaSupplier.Text)
        Debug.WriteLine("═══════════════════════════════════════════════════════════════════")
        Debug.WriteLine(String.Format("{0,-5} {1,-40} {2,-30} {3,-30} {4,15:N0} {5,15:N0}",
                    "No", "Uraian", "Akun Debet", "Akun Kredit", "Debet", "Kredit"))
        Debug.WriteLine(New String("-"c, 135))
        Debug.WriteLine(String.Format("{0,-5} {1,-40} {2,-30} {3,-30} {4,15:N0} {5,15:N0}",
                    "J1", "K Kas Tunai", "-", CmbAkunTunai.Text, 0, kasTunaiKeluar))
        Debug.WriteLine(String.Format("{0,-5} {1,-40} {2,-30} {3,-30} {4,15:N0} {5,15:N0}",
                    "J2", "K Transfer", "-", CmbAkunTransfer.Text, 0, nominalBayarTransfer))
        Debug.WriteLine(String.Format("{0,-5} {1,-40} {2,-30} {3,-30} {4,15:N0} {5,15:N0}",
                    "J3", "K Hutang Belanja" & If(Not statusLunas AndAlso sisaHutang > 0, "", " (tidak dieksekusi — lunas)"),
                    "-", Kode_rek_Hutang_Beli, 0, If(Not statusLunas AndAlso sisaHutang > 0, sisaHutang, 0D)))
        Debug.WriteLine(String.Format("{0,-5} {1,-40} {2,-30} {3,-30} {4,15:N0} {5,15:N0}",
                    "J4", "K Diskon Pembelian", "-", "06.05.001", 0, diskonSupplier))
        Debug.WriteLine(String.Format("{0,-5} {1,-40} {2,-30} {3,-30} {4,15:N0} {5,15:N0}",
                    "J5", "D Persediaan Barang", KODE_REK_BARANG, "-", _subtotalBarang, 0))
        Debug.WriteLine(String.Format("{0,-5} {1,-40} {2,-30} {3,-30} {4,15:N0} {5,15:N0}",
                    "J6", "D PPN Masukan", "01.05.001", "-", ppnMasukan, 0))
        Debug.WriteLine(String.Format("{0,-5} {1,-40} {2,-30} {3,-30} {4,15:N0} {5,15:N0}",
                    "J7", "D Biaya Kirim", "06.02.001", "-", biayaKirim, 0))
        Debug.WriteLine(New String("-"c, 135))
        Debug.WriteLine(String.Format("{0,-5} {1,-40} {2,-30} {3,-30} {4,15:N0} {5,15:N0}",
                    "", "TOTAL", "", "", totalDebet, totalKredit))

        Dim selisih As Decimal = totalDebet - totalKredit
        If selisih = 0 Then
            Debug.WriteLine("✅ JURNAL SEIMBANG - D=K=" & totalDebet.ToString("N0"))
        Else
            Debug.WriteLine("❌ JURNAL TIDAK SEIMBANG - Selisih=" & selisih.ToString("N0") &
                        " | D=" & totalDebet.ToString("N0") & " | K=" & totalKredit.ToString("N0"))
        End If
        Debug.WriteLine("═══════════════════════════════════════════════════════════════════")

        outDebet = totalDebet
        outKredit = totalKredit
    End Sub


    ''' <summary>Helper INSERT satu baris ke JurnalUmum</summary>
    Private Sub InsertJurnal(trans As MySqlTransaction, noTrans As String, noNota As String,
                              uraian As String, namaD As String, kodeD As String,
                              namaK As String, kodeK As String, nominal As Decimal)
        Using cmd As New MySqlCommand(
            "INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
            "NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, " &
            "JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
            "VALUES (@NO, @TGL, @NOTA, @URAIAN, @ND, @KD, @NK, @KK, @NOM, @JENIS, @LOK, @USR, @PC)",
            conn, trans)
            cmd.Parameters.AddWithValue("@NO", noTrans)
            cmd.Parameters.AddWithValue("@TGL", DtpTanggalPembelian.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NOTA", noNota)
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            cmd.Parameters.AddWithValue("@ND", namaD)
            cmd.Parameters.AddWithValue("@KD", kodeD)
            cmd.Parameters.AddWithValue("@NK", namaK)
            cmd.Parameters.AddWithValue("@KK", kodeK)
            cmd.Parameters.AddWithValue("@NOM", nominal)
            cmd.Parameters.AddWithValue("@JENIS", "Pembelian")
            cmd.Parameters.AddWithValue("@LOK", LblLokasiBarang.Text)
            cmd.Parameters.AddWithValue("@USR", If(IsModeTambahPembelian, FormUtama.StatusNamaUser.Text, TxtLogin.Text))
            cmd.Parameters.AddWithValue("@PC", If(IsModeTambahPembelian, FormUtama.StatusNamaPC.Text, TxtKomputer.Text))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' Ambil nama akun dari tbl_datareferensi berdasarkan kode akun
    ''' Jika tidak ditemukan, gunakan nama default sebagai fallback
    ''' </summary>
    Private Function AmbilNamaAkunDariReferensi(kodeAkun As String, namaDefault As String) As String
        Try
            Using cmd As New MySqlCommand("SELECT NAMA_AKUN FROM tbl_datareferensi WHERE KODE_AKUN = @KODE", conn)
                cmd.Parameters.AddWithValue("@KODE", kodeAkun)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    Return result.ToString()
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine("[ERROR] AmbilNamaAkunDariReferensi (" & kodeAkun & "): " & ex.Message)
        End Try
        Return namaDefault
    End Function
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: MODE EDIT - AMBIL DATA (FETCH DATA)
    ' ═══════════════════════════════════════════════════════════════════
#Region "MODE EDIT - AMBIL DATA"

    Private Sub AmbilDaftarBarangEditpembelian()
        ' Kosongkan baris yang ada di DataGridView
        DgvData.Rows.Clear()

        Dim queryPembelian As String =
        "SELECT pd.ID_BARANG, pd.NAMA_BARANG, pd.HARGA_BELI, pd.QTY, pd.SATUAN, pd.ISI_SATUAN, pd.HARGA_BELI_SATUAN, pd.QTY_SAT, pd.TOTAL, pd.HARGA_AVERAGE, pd.HARGA_BELI_SEBELUMNYA, " &
        "tb.SATUAN_UMUM_KECIL, tb.SATUAN_UMUM_SEDANG, tb.SATUAN_UMUM_BESAR " &
        "FROM pembelian_detail pd " &
        "LEFT JOIN tbl_barang tb ON pd.ID_BARANG = tb.ID_BARANG " &
        "WHERE pd.FAKTUR_BELI = ?"
        Using cmd As New MySqlCommand(queryPembelian, conn)
            cmd.Parameters.AddWithValue("@FAKTUR_BELI", TxtIdPembelian.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                ' Proses setiap record dari data reader
                While rd.Read()
                    ' Tambahkan baris baru ke DataGridView
                    Dim baris As DataGridViewRow = DirectCast(DgvData.Rows(DgvData.Rows.Add()), DataGridViewRow)

                    ' Isi nilai ke sel baris berdasarkan nama kolom
                    baris.Cells("Id").Value = ModuleAngka.SafeGetValue(Of String)(rd, "ID_BARANG", "")
                    baris.Cells("Nama").Value = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_BARANG", String.Empty)
                    baris.Cells("Hargabeli").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI"))
                    baris.Cells("Qty").Value = ModuleAngka.ParseDecimal(rd("QTY"))
                    baris.Cells("Isi").Value = CInt(Math.Max(1, ModuleAngka.ParseDecimal(rd("ISI_SATUAN"))))
                    baris.Cells("HargaBeliSatKecil").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_SATUAN"))
                    baris.Cells("QtySat").Value = ModuleAngka.ParseDecimal(rd("QTY_SAT"))
                    baris.Cells("Totalharga").Value = ModuleAngka.ParseDecimal(rd("TOTAL"))
                    baris.Cells("Average").Value = ModuleAngka.ParseDecimal(rd("HARGA_AVERAGE"))
                    baris.Cells("HargaSebelumnya").Value = ModuleAngka.ParseDecimal(rd("HARGA_BELI_SEBELUMNYA"))
                    baris.Cells("QtySebelumnya").Value = ModuleAngka.ParseDecimal(rd("QTY_SAT"))

                    Dim comboCell As DataGridViewComboBoxCell = CType(baris.Cells("Satuan"), DataGridViewComboBoxCell)
                    comboCell.Items.Clear()
                    For Each satNama As String In {ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_KECIL", ""),
                                               ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_SEDANG", ""),
                                               ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN_UMUM_BESAR", "")}
                        If satNama <> "" AndAlso Not comboCell.Items.Contains(satNama) Then comboCell.Items.Add(satNama)
                    Next

                    Dim satuanBeli As String = ModuleAngka.SafeGetValue(Of String)(rd, "SATUAN", "")
                    If satuanBeli <> "" AndAlso Not comboCell.Items.Contains(satuanBeli) Then
                        comboCell.Items.Add(satuanBeli)
                    End If
                    If satuanBeli <> "" Then
                        baris.Cells("Satuan").Value = satuanBeli
                    End If

                    ' Set Nama read-only — cegah user edit nama barang saat mode edit
                    baris.Cells("Nama").ReadOnly = True
                    baris.Cells("Nama").Style.BackColor = ModuleTheme.C(ModuleTheme.L_Subtle, ModuleTheme.D_Subtle)
                    baris.Cells("Nama").Style.ForeColor = ModuleTheme.C(ModuleTheme.L_Text, ModuleTheme.D_Text)
                End While
            End Using
        End Using

        ' Panggil UpdateSemuaTotal() di sini
        UpdateSemuaTotal()
        ' Refresh stok semua baris setelah DGV terisi
        RefreshStokSemuaBaris()

        ' Cek apakah DgvData memiliki baris
        If DgvData.Rows.Count > 0 Then
            ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
            DgvData.CurrentCell = DgvData(1, DgvData.Rows.Count - 1)

            ' Mengatur baris terakhir sebagai baris yang dipilih
            DgvData.Rows(DgvData.Rows.Count - 1).Selected = True
        End If

        SetupFocusToGrid()
    End Sub

    Private Sub AmbilDataPembelian()
        Dim queryString As String = "SELECT ID_SUPPLIER, NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, " &
                                "JENIS_BAYAR, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, " &
                                "STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER, " &
                                "IFNULL(NOMINAL_TRANSFER,0) AS NOMINAL_TRANSFER, " &
                                "IFNULL(KODE_AKUN_TF,'') AS KODE_AKUN_TF, " &
                                "IFNULL(NAMA_AKUN_TF,'') AS NAMA_AKUN_TF, " &
                                "IFNULL(DISKON_SUPPLIER,0) AS DISKON_SUPPLIER, " &
                                "IFNULL(PPN_MASUKAN,0) AS PPN_MASUKAN, " &
                                "IFNULL(BIAYA_KIRIM,0) AS BIAYA_KIRIM " &
                                "FROM pembelian WHERE ID_PEMBELIAN = ?"

        Dim IDSupplier As String
        Dim NamaSupplier As String = String.Empty
        Dim NotaPembelian As String = String.Empty
        Dim TanggalBeli As Date = Date.MinValue
        Dim Lokasi As String = String.Empty
        Dim JenisBayar As String = String.Empty
        Dim Pembayaran As Decimal = 0D
        Dim Tagihan As Decimal = 0D
        Dim JatuhTempo As Date = Date.MinValue
        Dim StatusTransaksi As String = "Lunas"
        Dim IDUser As String = String.Empty
        Dim IDKomputer As String = String.Empty
        Dim NominalTransfer As Decimal = 0D
        Dim KodeAkunTF As String = ""
        Dim NamaAkunTF As String = ""
        Dim DiskonSupplier As Decimal = 0D
        Dim PpnMasukan As Decimal = 0D
        Dim BiayaKirim As Decimal = 0D

        Using cmd As New MySqlCommand(queryString, conn)
            cmd.Parameters.AddWithValue("@ID_PEMBELIAN", TxtIdPembelian.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    rd.Read()
                    IDSupplier = rd("ID_SUPPLIER").ToString()
                    NamaSupplier = rd("NAMA_SUPLIYER").ToString()
                    NotaPembelian = rd("NOTA_PEMBELIAN").ToString()
                    TanggalBeli = CDate(rd("TGL_BELI"))
                    Lokasi = rd("LOKASI").ToString()
                    JenisBayar = rd("JENIS_BAYAR").ToString()
                    Pembayaran = ModuleAngka.ParseDecimal(rd("PEMBAYARAN"))
                    Tagihan = ModuleAngka.ParseDecimal(rd("TAGIHAN"))
                    JatuhTempo = ModuleAngka.SafeGetValue(Of DateTime)(rd, "JATUH_TEMPO", DtpTanggalPembelian.Value.AddMonths(1))
                    StatusTransaksi = ModuleAngka.SafeGetValue(Of String)(rd, "STATUS_TRANSAKSI_BELI", "Lunas")
                    IDUser = rd("ID_USER").ToString()
                    IDKomputer = rd("ID_KOMPUTER").ToString()
                    NominalTransfer = ModuleAngka.ParseDecimal(rd("NOMINAL_TRANSFER"))
                    KodeAkunTF = rd("KODE_AKUN_TF").ToString()
                    NamaAkunTF = rd("NAMA_AKUN_TF").ToString()
                    DiskonSupplier = ModuleAngka.ParseDecimal(rd("DISKON_SUPPLIER"))
                    PpnMasukan = ModuleAngka.ParseDecimal(rd("PPN_MASUKAN"))
                    BiayaKirim = ModuleAngka.ParseDecimal(rd("BIAYA_KIRIM"))
                Else
                    MessageBox.Show("Data tidak ditemukan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Sub
                End If
            End Using
        End Using

        ' ✅ Set semua field dari data DB - tidak ada nilai stale
        LblKodeSupplier.Text = IDSupplier
        TxtNamaSupplier.Text = NamaSupplier
        TxtNotaPembelian.Text = NotaPembelian
        DtpTanggalPembelian.Value = TanggalBeli
        LblLokasiBarang.Text = Lokasi
        CmbAkunTunai.Text = JenisBayar
        TxtNominalBayarTunai.Text = ModuleAngka.FormatUntukInput(Pembayaran)
        TxtKembaliHutang.Text = ModuleAngka.FormatUntukInput(Tagihan)
        LblStatusPembayaran.Text = StatusTransaksi
        TxtLogin.Text = IDUser
        TxtKomputer.Text = IDKomputer
        TxtNominalBayarTransfer.Text = ModuleAngka.FormatUntukInput(NominalTransfer)
        TxtKodeAkunTransfer.Text = KodeAkunTF
        CmbAkunTransfer.Text = NamaAkunTF

        ' Atur lebar GBBayar berdasarkan nominal transfer
        If NominalTransfer > 0 Then
            GBBayar.Size = New Size(833, 344)
        Else
            GBBayar.Size = New Size(509, 344)
        End If

        TxtDiskonRupiah.Text = ModuleAngka.FormatUntukInput(DiskonSupplier)
        TxtPpnRupiah.Text = ModuleAngka.FormatUntukInput(PpnMasukan)
        TxtBiayaKirim.Text = ModuleAngka.FormatUntukInput(BiayaKirim)
        ' Reset field persen — tidak disimpan di DB, cukup kosongkan
        TxtDiskonPersen.Text = ModuleAngka.FormatUntukInput(0)
        TxtPpnPersen.Text = ModuleAngka.FormatUntukInput(0)

        ' ✅ Set jatuh tempo dan tampilan status hutang
        If StatusTransaksi = "Belum Lunas" AndAlso JatuhTempo <> Date.MinValue Then
            DtpJatuhTempo.Value = JatuhTempo
            LblJatuhTempo.Visible = True
            DtpJatuhTempo.Visible = True
            LblPembayaran.Text = "Hutang :"
        Else
            LblJatuhTempo.Visible = False
            DtpJatuhTempo.Visible = False
            LblPembayaran.Text = "Kembalian :"
        End If
    End Sub
#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: HELPER & EVENT HANDLERS LAINNYA
    ' ═══════════════════════════════════════════════════════════════════
#Region "HELPER & EVENT HANDLERS LAINNYA"

    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "Beli"}
            frm.ShowDialog()
        End Using
        MuatSemuaPengaturan()
    End Sub

    ' ============================================
    ' FUNGSI: TAMPILKAN BANTUAN SHORTCUT
    ' ============================================
    Private Sub TampilkanBantuan()
        Dim helpText As String = "SHORTCUT KEYBOARD:" & vbCrLf & vbCrLf &
                           "F1      : Tampilkan bantuan ini" & vbCrLf &
                           "F2      : Fokus ke Supplier" & vbCrLf &
                           "F3      : Buka form pilih Supplier" & vbCrLf &
                           "F4      : Buka form pilih Barang" & vbCrLf &
                           "F5      : Buka form setting printer" & vbCrLf &
                           "F6      : Tahan transaksi (draft)" & vbCrLf &
                           "F7      : Panggil transaksi ditahan" & vbCrLf &
                           "F8      : Bayar / proses pembayaran" & vbCrLf &
                           "F9      : Hide panel bayar" & vbCrLf &
                           "F10     : Simpan transaksi" & vbCrLf &
                           "F11     : Fokus ke Akun Tunai" & vbCrLf &
                           "F12     : Fokus ke Akun Transfer" & vbCrLf &
                           "ESC     : Keluar / Tutup panel" & vbCrLf & vbCrLf &
                           "↑/↓ :" & vbCrLf &
                           "  - Di TxtNama/Barang: Navigasi list hasil pencarian" & vbCrLf &
                           "  - Di TxtNamaSupplier: Navigasi list supplier" & vbCrLf &
                           "  - Di kolom Satuan: Pilih satuan (atas/bawah)" & vbCrLf &
                           "  - Di LstBarang/listSupplier: Navigasi item" & vbCrLf & vbCrLf &
                           "ENTER   : Pilih item dari list" & vbCrLf &
                           "DELETE  : Hapus baris di grid"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub



#End Region
End Class
