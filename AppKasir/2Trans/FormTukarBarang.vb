Public Class FormTukarBarang
    Private Sub Form_TukarBarang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Kondisiawal()
        TampilNotaJual()
        Autocomplit()

    End Sub
    Private Sub Kondisiawal()
        ListBoxFakturjual.Items.Clear()
        ' Kosongkan sumber data (contoh: DataTable)
        Dim dt As New DataTable()
        DgvData.DataSource = dt

        ' Hapus data yang ditampilkan di DataGridView
        DgvData.Columns.Clear()

        LblIdFakturJual.Text = ""
        LblIdTransaksi.Text = ""
        LblNamaPel.Text = ""
        LblKodePelanggan.Text = ""
        TxtHargaKeluar.Text = ""
        TxtHargaMasuk.Text = ""
        TxtIsiKeluar.Text = ""
        TxtIsiMasuk.Text = ""
        TxtKodeKeluar.Text = ""
        TxtKodeMasuk.Text = ""
        TxtNamaKeluar.Text = ""
        TxtNamaMasuk.Text = ""
        TxtTotalHargaKeluar.Text = ""
        TxtTotalHargaMasuk.Text = ""
        TxtDiskonKeluar.Text = 0
        LblHargaSatKeluar.Text = "Harga Satuan Rp. 0"
        LblHargaSatMasuk.Text = "Harga Satuan Rp. 0"
        GenerateNomorTukarBarang()
        GBPilihan.Visible = False
        ListBoxFakturjual.Visible = False
        DgvData.Visible = False
    End Sub

    Public Sub TampilNotaJual()
        ListBoxFakturjual.Items.Clear()
        Dim tanggalAwal As Date = DTPTgl.Value.Date
        Dim tanggalAkhir As Date = DTPTgl.Value.Date.AddDays(1).AddTicks(-1)
        Dim query As String = "SELECT DISTINCT ID_PENJUALAN FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then ' Pengecekan apakah ada data yang ditemukan
                    While rd.Read()
                        Dim idPenjualan As String = rd.GetString(0)
                        ListBoxFakturjual.Items.Add(idPenjualan)
                    End While
                    GBPilihan.Visible = True
                    ListBoxFakturjual.Visible = True
                Else
                    GBPilihan.Visible = False
                    ListBoxFakturjual.Visible = False ' Sembunyikan ListBox jika tidak ada data yang ditemukan
                End If
            End Using
        End Using
    End Sub
    Private Sub GenerateNomorTukarBarang()
        Dim cekTanggal As String = Microsoft.VisualBasic.Format(DtpTanggal.Value, "yyMMdd")
        Dim cekNomor As String = "TB-" & cekTanggal
        Dim urutKode As String = ""

        Dim query As String = "SELECT MAX(ID_TUKAR) AS MaxID FROM TukarBarang WHERE ID_TUKAR LIKE @CekNomor"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@CekNomor", cekNomor & "%")

            Dim maxID As Object = command.ExecuteScalar()
            Dim maxIDValue As Integer

            If maxID IsNot DBNull.Value AndAlso Integer.TryParse(maxID.ToString(), maxIDValue) Then
                Dim maxKodeTukarBarang As String = "TB-" & cekTanggal & maxIDValue.ToString("0000")

                If maxKodeTukarBarang.StartsWith("TB-" & cekTanggal) Then
                    Dim hitung As Integer = maxIDValue + 1
                    urutKode = "TB-" & cekTanggal & hitung.ToString("0000")
                End If
            Else
                urutKode = "TB-" & cekTanggal & "0001"
            End If
        End Using
        LblIdTransaksi.Text = urutKode
    End Sub



    Private Sub ListBoxFakturjual_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBoxFakturjual.SelectedIndexChanged
        If ListBoxFakturjual.SelectedItem IsNot Nothing Then
            Dim selectedData As String = ListBoxFakturjual.SelectedItem.ToString()
            Dim idJual As String = selectedData

            LblIdFakturJual.Text = idJual

            ' Ambil data pelanggan berdasarkan ID_PENJUALAN
            Using cmdPelanggan As New MySqlCommand("SELECT ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN FROM penjualan WHERE ID_PENJUALAN = @idJual", conn)
                cmdPelanggan.Parameters.AddWithValue("@idJual", idJual)
                Using rdPelanggan As MySqlDataReader = cmdPelanggan.ExecuteReader()
                    If rdPelanggan.Read() Then
                        If Not rdPelanggan.IsDBNull(0) Then
                            LblKodePelanggan.Text = rdPelanggan.GetString(0)
                        Else
                            LblKodePelanggan.Text = ""
                        End If

                        If Not rdPelanggan.IsDBNull(1) Then
                            LblNamaPel.Text = rdPelanggan.GetString(1)
                        Else
                            LblNamaPel.Text = ""
                        End If

                        If Not rdPelanggan.IsDBNull(2) Then
                            LblJenisPel.Text = rdPelanggan.GetString(2)
                        Else
                            LblJenisPel.Text = ""
                        End If
                    End If
                End Using
            End Using


            If ds IsNot Nothing Then
                ds.Dispose()
            End If

            ' Buat DataSet untuk menyimpan data dari database
            Using ds As New DataSet()
                DgvData.Columns.Clear()

                Using cmdBarang As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, HARGA_JUAL, QTY_SATUAN, TOTAL_DISKON, TOTAL_HARGA FROM penjualan_detail WHERE FAKTUR_JUAL = @idJual", conn)
                    cmdBarang.Parameters.AddWithValue("@idJual", idJual)
                    Using da As New MySqlDataAdapter(cmdBarang)
                        ' Isi data dari database ke DataSet menggunakan DataTable dengan nama "Barang"
                        da.Fill(ds, "Barang")
                    End Using
                End Using

                ' Tampilkan data dari DataSet ke DataGridView
                DgvData.DataSource = ds.Tables("Barang")

                ' Mengatur lebar kolom

                DgvData.Columns("NAMA_BARANG").Width = 200
                DgvData.Columns("QTY").Width = 50
                DgvData.Columns("SATUAN").Width = 60
                DgvData.Columns("TOTAL_HARGA").Width = 60

                ' Mengatur format kolom angka (ribuan) dan rata kanan
                DgvData.Columns("QTY").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                DgvData.Columns("TOTAL_HARGA").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                DgvData.Columns("TOTAL_HARGA").DefaultCellStyle.Format = "N0" ' Format untuk ribuan tanpa angka di belakang koma
                ' Sembunyikan kolom-kolom yang tidak ingin ditampilkan
                DgvData.Columns("ID_BARANG").Visible = False
                DgvData.Columns("ISI_SATUAN").Visible = False
                DgvData.Columns("HARGA_JUAL").Visible = False
                DgvData.Columns("QTY_SATUAN").Visible = False
                DgvData.Columns("TOTAL_DISKON").Visible = False

            End Using ' Objek DataSet akan dihapus dengan benar ketika keluar dari blok Using
            DgvData.Visible = True
            TxtKodeMasuk.Text = ""
            TxtNamaMasuk.Text = ""
            QtyMasuk.Text = ""
            CmbSatuanMasuk.Text = ""
            TxtIsiMasuk.Text = ""
            TxtHargaMasuk.Text = ""
            TxtQtySat.Text = ""
            TxtDiskonMasuk.Text = ""
            TxtTotalHargaMasuk.Text = ""
        Else
            DgvData.Visible = False
        End If
    End Sub
    Private Sub DgvData_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellClick
        If DgvData.Rows.Count > 0 Then
            Dim rowIndex As Integer = e.RowIndex
            If rowIndex >= 0 AndAlso rowIndex < DgvData.Rows.Count Then
                Dim kodeValue As String = DgvData.Item(0, rowIndex).Value.ToString().Trim()
                ' Memeriksa apakah kolom 0 (Kode) berisi data sebelum melanjutkan
                If Not String.IsNullOrEmpty(kodeValue) Then
                    ' Lakukan tindakan yang diinginkan karena kolom 0 berisi data
                    TxtKodeMasuk.Text = kodeValue
                    TxtNamaMasuk.Text = DgvData.Item(1, rowIndex).Value.ToString()
                    QtyMasuk.Text = DgvData.Item(2, rowIndex).Value.ToString()
                    CmbSatuanMasuk.Text = DgvData.Item(3, rowIndex).Value.ToString()
                    TxtIsiMasuk.Text = DgvData.Item(4, rowIndex).Value.ToString()
                    TxtHargaMasuk.Text = DgvData.Item(5, rowIndex).Value.ToString()
                    Dim hargaMasukValue As Double
                    If Double.TryParse(TxtHargaMasuk.Text, hargaMasukValue) Then
                        LblHargaSatMasuk.Text = "Harga Satuan Rp. " & hargaMasukValue.ToString("N0")
                    Else
                        LblHargaSatMasuk.Text = "Harga Satuan Rp. 0"
                    End If
                    TxtQtySat.Text = DgvData.Item(6, rowIndex).Value.ToString()
                    TxtDiskonMasuk.Text = DgvData.Item(7, rowIndex).Value.ToString()
                    TxtTotalHargaMasuk.Text = DgvData.Item(8, rowIndex).Value.ToString()
                Else
                    TxtKodeMasuk.Text = ""
                    TxtNamaMasuk.Text = ""
                    QtyMasuk.Text = ""
                    CmbSatuanMasuk.Text = ""
                    TxtIsiMasuk.Text = ""
                    TxtHargaMasuk.Text = ""
                    LblHargaSatMasuk.Text = "Harga Satuan Rp. 0"
                    TxtQtySat.Text = ""
                    TxtDiskonMasuk.Text = ""
                    TxtTotalHargaMasuk.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub DTPTgl_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DTPTgl.ValueChanged
        TampilNotaJual()
    End Sub

    Private Sub Autocomplit()
        ' Buatlah daftar pilihan AutoComplete
        Dim autoCompleteList As New AutoCompleteStringCollection()

        ' Lakukan query ke tabel "tbl_barang" untuk mendapatkan daftar nama
        Dim sqlQuery As String = "SELECT NAMA_BARANG FROM tbl_barang"
        Using cmd As New MySqlCommand(sqlQuery, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    ' Tambahkan setiap nama ke dalam daftar pilihan AutoComplete
                    autoCompleteList.Add(reader("NAMA_BARANG").ToString())
                End While
            End Using
        End Using

        ' Atur AutoComplete pada TextBox
        TxtNamaKeluar.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        TxtNamaKeluar.AutoCompleteSource = AutoCompleteSource.CustomSource
        TxtNamaKeluar.AutoCompleteCustomSource = autoCompleteList

    End Sub

    Private Sub TxtNamaKeluar_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtNamaKeluar.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Tombol "Enter" ditekan, maka lakukan pembaruan kontrol dari database
            UpdateControlsFromDatabase()
            QtyKeluar.Focus() ' Pindahkan fokus ke kontrol "QtyKeluar" setelah melakukan pembaruan
            e.Handled = True ' Menandai bahwa event ini telah ditangani
        End If
    End Sub

    Private Sub UpdateControlsFromDatabase()
        ' Lakukan query ke tabel "tbl_barang" untuk mendapatkan data terpilih berdasarkan input dari TxtNamaKeluar
        Dim sqlQuery As String = "SELECT ID_BARANG,SATUAN_UMUM_KECIL,SATUAN_UMUM_SEDANG,SATUAN_UMUM_BESAR,ISI_UMUM_KECIL,HARGA_JUAL_UMUM_KECIL,SATUAN_PARTAI_KECIL,SATUAN_PARTAI_SEDANG,SATUAN_PARTAI_BESAR,ISI_PARTAI_KECIL,HARGA_JUAL_PARTAI_KECIL FROM tbl_barang WHERE NAMA_BARANG LIKE @selectedData"

        Using cmd As New MySqlCommand(sqlQuery, conn)
            cmd.Parameters.AddWithValue("@selectedData", "%" & TxtNamaKeluar.Text & "%")
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Ambil data ID_BARANG dari hasil query
                    TxtKodeKeluar.Text = reader("ID_BARANG").ToString()

                    ' Clear ComboBox items before adding new ones
                    CmbSatuanKeluar.Items.Clear()

                    ' Jika LblJenisPel.Text adalah "Partai", lakukan sesuatu
                    If LblJenisPel.Text = "Partai" Then
                        CmbSatuanKeluar.Text = reader("SATUAN_PARTAI_KECIL")
                        TxtIsiKeluar.Text = reader("ISI_PARTAI_KECIL")
                        TxtHargaKeluar.Text = reader("HARGA_JUAL_PARTAI_KECIL")
                        Dim hargaKeluarValue As Double
                        If Double.TryParse(TxtHargaKeluar.Text, hargaKeluarValue) Then
                            LblHargaSatKeluar.Text = "Harga Satuan Rp. " & hargaKeluarValue.ToString("N0")
                        Else
                            LblHargaSatKeluar.Text = "Harga Satuan Rp. 0"
                        End If

                        Dim satuanKecil As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_PARTAI_KECIL")), rd.GetString(rd.GetOrdinal("SATUAN_PARTAI_KECIL")), "")
                        Dim satuanSedang As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_PARTAI_SEDANG")), rd.GetString(rd.GetOrdinal("SATUAN_PARTAI_SEDANG")), "")
                        Dim satuanBesar As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_PARTAI_BESAR")), rd.GetString(rd.GetOrdinal("SATUAN_PARTAI_BESAR")), "")

                        If Not String.IsNullOrEmpty(satuanKecil) Then
                            CmbSatuanKeluar.Items.Add(satuanKecil)
                        End If

                        If Not String.IsNullOrEmpty(satuanSedang) Then
                            CmbSatuanKeluar.Items.Add(satuanSedang)
                        End If

                        If Not String.IsNullOrEmpty(satuanBesar) Then
                            CmbSatuanKeluar.Items.Add(satuanBesar)
                        End If
                        ' Lakukan sesuatu untuk kasus "Partai"
                    Else
                        CmbSatuanKeluar.Text = reader("SATUAN_UMUM_KECIL")
                        TxtIsiKeluar.Text = reader("ISI_UMUM_KECIL")
                        TxtHargaKeluar.Text = reader("HARGA_JUAL_UMUM_KECIL")
                        Dim hargaKeluarValue As Double
                        If Double.TryParse(TxtHargaKeluar.Text, hargaKeluarValue) Then
                            LblHargaSatKeluar.Text = "Harga Satuan Rp. " & hargaKeluarValue.ToString("N0")
                        Else
                            LblHargaSatKeluar.Text = "Harga Satuan Rp. 0"
                        End If

                        Dim satuanKecil As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_KECIL")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_KECIL")), "")
                        Dim satuanSedang As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
                        Dim satuanBesar As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_BESAR")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_BESAR")), "")

                        If Not String.IsNullOrEmpty(satuanKecil) Then
                            CmbSatuanKeluar.Items.Add(satuanKecil)
                        End If

                        If Not String.IsNullOrEmpty(satuanSedang) Then
                            CmbSatuanKeluar.Items.Add(satuanSedang)
                        End If

                        If Not String.IsNullOrEmpty(satuanBesar) Then
                            CmbSatuanKeluar.Items.Add(satuanBesar)
                        End If

                        ' Lakukan sesuatu untuk kasus selain "Partai"
                    End If
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbSatuanKeluar_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSatuanKeluar.SelectedIndexChanged
        Dim sqlQuery As String = "SELECT ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR FROM tbl_barang WHERE ID_BARANG = @ID_BARANG"

        Using cmd As New MySqlCommand(sqlQuery, conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", TxtKodeKeluar.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    If LblJenisPel.Text = "Partai" Then
                        If CmbSatuanKeluar.SelectedIndex = 1 Then
                            TxtIsiKeluar.Text = reader("ISI_PARTAI_SEDANG").ToString()
                            TxtHargaKeluar.Text = reader("HARGA_JUAL_PARTAI_SEDANG").ToString()

                            Dim hargaKeluarValue As Double
                            If Double.TryParse(TxtHargaKeluar.Text, hargaKeluarValue) Then
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. " & hargaKeluarValue.ToString("N0")
                            Else
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. 0"
                            End If

                        ElseIf CmbSatuanKeluar.SelectedIndex = 2 Then
                            TxtIsiKeluar.Text = reader("ISI_PARTAI_BESAR").ToString()
                            TxtHargaKeluar.Text = reader("HARGA_JUAL_PARTAI_BESAR").ToString()
                            Dim hargaKeluarValue As Double
                            If Double.TryParse(TxtHargaKeluar.Text, hargaKeluarValue) Then
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. " & hargaKeluarValue.ToString("N0")
                            Else
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. 0"
                            End If
                        Else
                            TxtIsiKeluar.Text = reader("ISI_PARTAI_KECIL").ToString()
                            TxtHargaKeluar.Text = reader("HARGA_JUAL_PARTAI_KECIL").ToString()
                            Dim hargaKeluarValue As Double
                            If Double.TryParse(TxtHargaKeluar.Text, hargaKeluarValue) Then
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. " & hargaKeluarValue.ToString("N0")
                            Else
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. 0"
                            End If
                        End If
                    Else
                        If CmbSatuanKeluar.SelectedIndex = 1 Then
                            TxtIsiKeluar.Text = reader("ISI_UMUM_SEDANG").ToString()
                            TxtHargaKeluar.Text = reader("HARGA_JUAL_UMUM_SEDANG").ToString()
                            Dim hargaKeluarValue As Double
                            If Double.TryParse(TxtHargaKeluar.Text, hargaKeluarValue) Then
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. " & hargaKeluarValue.ToString("N0")
                            Else
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. 0"
                            End If
                        ElseIf CmbSatuanKeluar.SelectedIndex = 2 Then
                            TxtIsiKeluar.Text = reader("ISI_UMUM_BESAR").ToString()
                            TxtHargaKeluar.Text = reader("HARGA_JUAL_UMUM_BESAR").ToString()
                            Dim hargaKeluarValue As Double
                            If Double.TryParse(TxtHargaKeluar.Text, hargaKeluarValue) Then
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. " & hargaKeluarValue.ToString("N0")
                            Else
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. 0"
                            End If
                        Else
                            TxtIsiKeluar.Text = reader("ISI_UMUM_KECIL").ToString()
                            TxtHargaKeluar.Text = reader("HARGA_JUAL_UMUM_KECIL").ToString()
                            Dim hargaKeluarValue As Double
                            If Double.TryParse(TxtHargaKeluar.Text, hargaKeluarValue) Then
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. " & hargaKeluarValue.ToString("N0")
                            Else
                                LblHargaSatKeluar.Text = "Harga Satuan Rp. 0"
                            End If
                        End If
                    End If
                End If
            End Using
        End Using
    End Sub

    Private Sub TxtDiskonMasuk_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtDiskonMasuk.TextChanged
        ' Konversi teks pada TxtDiskon ke dalam format angka dengan pemisah ribuan
        Dim diskonValue As Double = Val(TxtDiskonMasuk.Text)
        LblDiskonMasuk.Text = "Rp. " & diskonValue.ToString("N0")
    End Sub

    Private Sub TxtTotalHargaMasuk_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtTotalHargaMasuk.TextChanged
        ' Konversi teks pada TxtTotalHargaMasuk ke dalam format angka dengan pemisah ribuan
        Dim totalHargaMasukValue As Double = Val(TxtTotalHargaMasuk.Text)
        LblTotalhargaMasuk.Text = "Rp. " & totalHargaMasukValue.ToString("N0")
        HitungDanTampilkanKeterangan()
    End Sub

    Private Sub TxtDiskonKeluar_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtDiskonKeluar.TextChanged
        UpdateTotalHargaKeluar()
        ' Konversi teks pada TxtDiskon ke dalam format angka dengan pemisah ribuan
        Dim diskonKeluarValue As Double = Val(TxtDiskonKeluar.Text)
        LblDiskonKeluar.Text = "Rp. " & diskonKeluarValue.ToString("N0")
    End Sub
    Private Sub TxtTotalHargaKeluar_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtTotalHargaKeluar.TextChanged
        ' Konversi teks pada TxtTotalHargaKeluar ke dalam format angka dengan pemisah ribuan
        Dim totalHargaKeluarValue As Double = Val(TxtTotalHargaKeluar.Text)
        LblTotalhargaKeluar.Text = "Rp. " & totalHargaKeluarValue.ToString("N0")
        HitungDanTampilkanKeterangan()
    End Sub

    Private Sub QtyKeluar_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QtyKeluar.TextChanged
        UpdateTotalHargaKeluar()
    End Sub


    Private Sub TxtIsiKeluar_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtIsiKeluar.TextChanged
        UpdateTotalHargaKeluar()
    End Sub

    Private Sub UpdateTotalHargaKeluar() Handles QtyKeluar.TextChanged, TxtIsiKeluar.TextChanged, TxtHargaKeluar.TextChanged, TxtDiskonKeluar.TextChanged
        ' Convert the text values to numeric types, assuming 0 if they are empty
        Dim qtyKeluarValue As Double = If(String.IsNullOrEmpty(QtyKeluar.Text), 0, Val(QtyKeluar.Text))
        Dim isiKeluarValue As Double = If(String.IsNullOrEmpty(TxtIsiKeluar.Text), 0, Val(TxtIsiKeluar.Text))
        Dim hargaKeluarValue As Double = If(String.IsNullOrEmpty(TxtHargaKeluar.Text), 0, Val(TxtHargaKeluar.Text))
        Dim diskon As Double = If(String.IsNullOrEmpty(TxtDiskonKeluar.Text), 0, Val(TxtDiskonKeluar.Text))

        ' Check if all relevant fields are not empty or equal to zero
        If qtyKeluarValue <> 0 AndAlso isiKeluarValue <> 0 AndAlso hargaKeluarValue <> 0 Then
            ' Calculate the new value for TxtQtySatKeluar
            Dim result As Double = qtyKeluarValue * isiKeluarValue

            ' Calculate the total price for the items with the discount
            Dim totalHargaKeluar As Double = (qtyKeluarValue * hargaKeluarValue) - diskon

            ' Update TxtQtySatKeluar and TxtTotalHargaKeluar with the calculated values
            TxtQtySatKeluar.Text = result.ToString()
            TxtTotalHargaKeluar.Text = totalHargaKeluar.ToString()
        Else
            ' Set TxtQtySatKeluar and TxtTotalHargaKeluar to 0 when the condition is not met
            TxtQtySatKeluar.Text = "0"
            TxtTotalHargaKeluar.Text = "0"
        End If
    End Sub

    Private Sub BtnKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKeluar.Click
        Close()
    End Sub
    Private Sub HitungDanTampilkanKeterangan()
        Dim totalHargaMasukValue As Double = If(String.IsNullOrEmpty(TxtTotalHargaMasuk.Text), 0, Val(TxtTotalHargaMasuk.Text))
        Dim totalHargaKeluarValue As Double = If(String.IsNullOrEmpty(TxtTotalHargaKeluar.Text), 0, Val(TxtTotalHargaKeluar.Text))

        Dim selisih As Double = totalHargaMasukValue - totalHargaKeluarValue
        LblNominalKet.Text = selisih
        ' Tampilkan keterangan berdasarkan nilai selisih
        If selisih > 0 Then
            LblKeterangan.Text = "Kembali Rp. " & selisih.ToString("N0")
        ElseIf selisih < 0 Then
            LblKeterangan.Text = "Kekurangan Rp. " & Math.Abs(selisih).ToString("N0")
        Else
            LblKeterangan.Text = "Uang pas"
        End If
    End Sub


    Private Sub BtnBayar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBayar.Click
        If Not IsFormValid() Then
            ' Jika validasi gagal, hentikan penyimpanan data
            Return
        End If
        Simpan()
        Updatestok()
        Kondisiawal()
    End Sub
    Private Function IsFormValid() As Boolean
        ' Validasi kolom masuk
        If String.IsNullOrEmpty(TxtNamaMasuk.Text) Then
            MessageBox.Show("Silahkan pilih barang berdasarkan data penjualan.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If String.IsNullOrEmpty(QtyMasuk.Text) Then
            MessageBox.Show("Kolom Qty Masuk tidak boleh kosong, pastikan data dari penjualan tidak bernilai 0.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        ' Validasi kolom keluar
        If String.IsNullOrEmpty(TxtNamaKeluar.Text) Then
            MessageBox.Show("Silahkan pilih barang yang akan digunakan untuk mengganti.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNamaKeluar.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(QtyKeluar.Text) Then
            MessageBox.Show("Kolom Qty tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            QtyKeluar.Focus()
            Return False
        End If

        Return True ' Jika semua kolom sudah diisi, maka kembalikan nilai true
    End Function

    Private Sub Simpan()
        Dim queryMasuk As String = "INSERT INTO TukarBarang (ID_TUKAR, ID_PENJUALAN, TANGGAL, DESKRIPSI, KodePel, NamaPel, JenisPel, ID_BARANG, NAMA_BARANG, " &
        "QTY, Satuan, ISiSatuan, QtySatuan, HargaSatuan, Diskon, TotalHarga, Selisih, ID_USER) " &
        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"

        Using cmdMasuk As New MySqlCommand(queryMasuk, conn)
            cmdMasuk.Parameters.AddWithValue("@Param1", If(LblIdTransaksi.Text, ""))
            cmdMasuk.Parameters.AddWithValue("@Param2", If(LblIdFakturJual.Text, ""))
            cmdMasuk.Parameters.AddWithValue("@Param3", Microsoft.VisualBasic.Format(DtpTanggal.Value, "yyyy-MM-dd HH:mm:ss"))
            cmdMasuk.Parameters.AddWithValue("@Param4", "Masuk")
            cmdMasuk.Parameters.AddWithValue("@Param5", If(LblKodePelanggan.Text, ""))
            cmdMasuk.Parameters.AddWithValue("@Param6", If(LblNamaPel.Text, ""))
            cmdMasuk.Parameters.AddWithValue("@Param7", If(LblJenisPel.Text, ""))
            cmdMasuk.Parameters.AddWithValue("@Param8", If(TxtKodeMasuk.Text, ""))
            cmdMasuk.Parameters.AddWithValue("@Param9", If(TxtNamaMasuk.Text, ""))
            cmdMasuk.Parameters.AddWithValue("@Param10", CDbl(If(QtyMasuk.Text, "0")))
            cmdMasuk.Parameters.AddWithValue("@Param11", If(CmbSatuanMasuk.Text, ""))
            cmdMasuk.Parameters.AddWithValue("@Param12", CDbl(If(TxtIsiMasuk.Text, "0")))
            cmdMasuk.Parameters.AddWithValue("@Param13", CDbl(If(TxtQtySat.Text, "0")))
            cmdMasuk.Parameters.AddWithValue("@Param14", CDbl(If(TxtHargaMasuk.Text, "0")))
            cmdMasuk.Parameters.AddWithValue("@Param15", CDbl(If(TxtDiskonMasuk.Text, "0")))
            cmdMasuk.Parameters.AddWithValue("@Param16", CDbl(If(TxtTotalHargaMasuk.Text, "0")))
            cmdMasuk.Parameters.AddWithValue("@Param17", CDbl(If(LblNominalKet.Text, "0")))
            cmdMasuk.Parameters.AddWithValue("@Param18", If(FormUtama.SLogin.Text, "")) ' Gunakan properti Text pada ToolStripStatusLabel

            cmdMasuk.ExecuteNonQuery()
        End Using

        Dim queryKeluar As String = "INSERT INTO TukarBarang (ID_TUKAR, ID_PENJUALAN, TANGGAL, DESKRIPSI, KodePel, NamaPel, JenisPel, ID_BARANG, NAMA_BARANG, " &
     "QTY, Satuan, ISiSatuan, QtySatuan, HargaSatuan, Diskon, TotalHarga, Selisih, ID_USER) " &
     "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"

        Using cmdKeluar As New MySqlCommand(queryKeluar, conn)
            cmdKeluar.Parameters.AddWithValue("@Param1", If(LblIdTransaksi.Text, ""))
            cmdKeluar.Parameters.AddWithValue("@Param2", If(LblIdFakturJual.Text, ""))
            cmdKeluar.Parameters.AddWithValue("@Param3", Microsoft.VisualBasic.Format(DtpTanggal.Value, "yyyy-MM-dd HH:mm:ss"))
            cmdKeluar.Parameters.AddWithValue("@Param4", "Keluar")
            cmdKeluar.Parameters.AddWithValue("@Param5", If(LblKodePelanggan.Text, ""))
            cmdKeluar.Parameters.AddWithValue("@Param6", If(LblNamaPel.Text, ""))
            cmdKeluar.Parameters.AddWithValue("@Param7", If(LblJenisPel.Text, ""))
            cmdKeluar.Parameters.AddWithValue("@Param8", If(TxtKodeKeluar.Text, ""))
            cmdKeluar.Parameters.AddWithValue("@Param9", If(TxtNamaKeluar.Text, ""))
            cmdKeluar.Parameters.AddWithValue("@Param10", CDbl(If(QtyKeluar.Text, "0"))) ' Menggunakan CDbl untuk mengonversi ke tipe data numerik
            cmdKeluar.Parameters.AddWithValue("@Param11", If(CmbSatuanKeluar.Text, ""))
            cmdKeluar.Parameters.AddWithValue("@Param12", CDbl(If(TxtIsiKeluar.Text, "0")))
            cmdKeluar.Parameters.AddWithValue("@Param13", CDbl(If(TxtQtySat.Text, "0")))
            cmdKeluar.Parameters.AddWithValue("@Param14", CDbl(If(TxtHargaKeluar.Text, "0")))
            cmdKeluar.Parameters.AddWithValue("@Param15", CDbl(If(TxtDiskonMasuk.Text, "0")))
            cmdKeluar.Parameters.AddWithValue("@Param16", CDbl(If(TxtTotalHargaKeluar.Text, "0")))
            cmdKeluar.Parameters.AddWithValue("@Param17", CDbl(If(LblNominalKet.Text, "0")))
            cmdKeluar.Parameters.AddWithValue("@Param18", If(FormUtama.SLogin.Text, "")) ' Gunakan properti Text pada ToolStripStatusLabel
            cmdKeluar.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub Updatestok()

        Dim idBarangMasuk As String = TxtKodeMasuk.Text
        Dim idBarangKeluar As String = TxtKodeKeluar.Text

        Dim qtySatMasuk As Double = Val(TxtQtySat.Text)
        Dim qtySatKeluar As Double = Val(TxtQtySatKeluar.Text)

        Using cmdUpdateMasuk As New MySqlCommand("UPDATE tbl_barang SET STOK_TOKO = STOK_TOKO + ? WHERE ID_BARANG = ?", conn)
            cmdUpdateMasuk.Parameters.AddWithValue("@QtySat", qtySatMasuk)
            cmdUpdateMasuk.Parameters.AddWithValue("@ID_BARANG", idBarangMasuk)
            cmdUpdateMasuk.ExecuteNonQuery()
        End Using

        Using cmdUpdateKeluar As New MySqlCommand("UPDATE tbl_barang SET STOK_TOKO = STOK_TOKO - ? WHERE ID_BARANG = ?", conn)
            cmdUpdateKeluar.Parameters.AddWithValue("@QtySat", qtySatKeluar)
            cmdUpdateKeluar.Parameters.AddWithValue("@ID_BARANG", idBarangKeluar)
            cmdUpdateKeluar.ExecuteNonQuery()
        End Using


    End Sub
End Class