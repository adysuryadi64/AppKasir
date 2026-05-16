Public Class FormLoading

    Public Sub MulaiLoading()
        'Dim today As Date = DateTime.Now.Date
        'Dim isAlreadyProcessed As Boolean = CheckIfProcessedToday(today)

        'If isAlreadyProcessed Then
        ' Menyimpan total langkah (jumlah fungsi yang akan dieksekusi)
        Dim totalLangkah As Integer = 5 ' Ubah sesuai dengan jumlah fungsi yang akan dieksekusi

        For i As Integer = 1 To totalLangkah
            ' Melakukan proses loading
            UpdateStatus(i, totalLangkah, GetTaskDescription(i, totalLangkah))

            ' Melakukan proses sesuai dengan fungsi yang dijalankan
            Select Case i
                Case 1
                    FormUtama.AmbilKomputer()
                    'Rekeningkasbank()
                Case 2
                    UpdateTotalBonDanTotalBayarKaryawan()
                    TambahPelanggan.UpdatePiutangDibayar()
                    TambahSupliyer.UpdateSupliyerFromPembelianHutangDibayar()
                    NotifikasiJatuhTempo.JumlahJatuhTempo()
                Case 3
                    AmbilDataMasterPerusahaan()
                Case 4
                    BacaHakAkseUser()
                    FormGeneralSetting.SinkronkanHakAksesTanpaDuplikat() ' Sinkronisasi hak akses tanpa duplikat
                Case 5
                    ' Mengaktifkan elemen antarmuka setelah proses selesai
                    UpdateUIComponents()
            End Select
        Next
        Close()

        'Else
        '    ' Jika belum diproses, hapus semua data di tabel Temp_Loading
        '    ClearTempLoadingTable()

        '    ' Masukkan tanggal hari ini ke tabel TempLoading
        '    InsertTodayDate(today)

        '    ' Menyimpan total langkah (jumlah fungsi yang akan dieksekusi)
        '    Dim totalLangkah As Integer = 9 ' Ubah sesuai dengan jumlah fungsi yang akan dieksekusi

        '    For i As Integer = 1 To totalLangkah
        '        ' Melakukan proses loading
        '        UpdateStatus(i, totalLangkah, GetTaskDescription(i, totalLangkah))

        '        ' Melakukan proses sesuai dengan fungsi yang dijalankan
        '        Select Case i
        '            Case 1
        '                FormUtama.AmbilKomputer()
        '                Rekeningkasbank()
        '            Case 2
        '                UpdateTotalBonDanTotalBayarKaryawan()
        '                TambahPelanggan.UpdatePiutangDibayar()
        '                TambahSupliyer.UpdateSupliyerFromPembelianHutangDibayar()
        '                NotifikasiJatuhTempo.JumlahJatuhTempo()
        '            Case 3
        '                AmbilDataMasterPerusahaan()
        '            Case 4
        '                BacaHakAkseUser()
        '            Case 5
        '                'ResetAllBarangToko()
        '                'ResetAllBarangGudang()
        '            Case 6
        '                'UpdateAllBarangTokoModule()
        '            Case 7
        '                'UpdateAllBarangGudangModule
        '            Case 8
        '                HitungSemuaKode()
        '            Case 9
        '                UpdateUIComponents()
        '        End Select
        '    Next
        '    Close()
        'End If
    End Sub

    Public Sub MulaiPosting(ByVal Jenis As String)
        ' Menyimpan total langkah (jumlah fungsi yang akan dieksekusi)
        Dim totalLangkah As Integer = 6 ' Ubah sesuai dengan jumlah fungsi yang akan dieksekusi

        For i As Integer = 1 To totalLangkah
            ' Melakukan proses loading
            UpdateStatus(i, totalLangkah, GetTaskDescription(i, totalLangkah))

            ' Melakukan proses sesuai dengan fungsi yang dijalankan
            Select Case i
                Case 1
                    FormLapNeracaLR.HITUNGSEMUASALDO()
                    UpdateTotalBonDanTotalBayarKaryawan()
                    TambahPelanggan.UpdatePiutangDibayar()
                    TambahSupliyer.UpdateSupliyerFromPembelianHutangDibayar()
                Case 2
                    NotifikasiJatuhTempo.JumlahJatuhTempo()
                Case 3
                    If Jenis = "Toko" Then
                        ResetAllBarangToko()
                    ElseIf Jenis = "Gudang" Then
                        ResetAllBarangGudang()
                    Else
                        ResetAllBarangToko()
                        ResetAllBarangGudang()
                    End If

                Case 4
                    If Jenis <> "Gudang" Then
                        UpdateAllBarangTokoModule()
                    End If

                Case 5
                    If Jenis <> "Toko" Then
                        UpdateAllBarangGudangModule()
                    End If

                Case 6

                    If Jenis = "Toko" Then
                        HitungStokToko()
                    ElseIf Jenis = "Gudang" Then
                        HitungStokGudang()
                    Else
                        ' Setelah UpdateAllBarangToko dan UpdateAllBarangGudang selesai
                        HitungSemuaKode()
                    End If

            End Select
        Next
        Close()

    End Sub

    Private Function CheckIfProcessedToday(ByVal today As Date) As Boolean
        ' Cek di TempLoading apakah sudah ada entry untuk tanggal hari ini
        Dim query As String = "SELECT COUNT(*) FROM Temp_Loading WHERE Tanggal = @TanggalHariIni"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@TanggalHariIni", today)
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return count > 0
        End Using
    End Function

    Private Sub ClearTempLoadingTable()
        ' Hapus semua data dari tabel Temp_Loading
        Dim query As String = "DELETE FROM Temp_Loading"
        Using cmd As New MySqlCommand(query, conn)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InsertTodayDate(ByVal today As Date)
        ' Masukkan tanggal hari ini ke TempLoading
        Dim query As String = "INSERT INTO Temp_Loading (Tanggal) VALUES (@TanggalHariIni)"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@TanggalHariIni", today)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub UpdateStatus(ByVal currentStep As Integer, ByVal totalSteps As Integer, ByVal taskDescription As String)
        ' Menghitung persentase selesai
        Dim completionPercentage As Integer = (currentStep / totalSteps) * 100

        ' Update progress bar
        ProgressBar1.Value = completionPercentage

        ' Update label status
        LabelProgress.Text = String.Format("{0} ({1}% - 100%)", taskDescription, completionPercentage)
        Application.DoEvents()

        ' Hanya tunggu jika belum selesai
        If completionPercentage < 100 Then
            Threading.Thread.Sleep(200) ' Mengurangi waktu tunggu
        End If
    End Sub

    Private Sub UpdateUIComponents()
        ' Mengaktifkan elemen UI
        With FormUtama
            .LogOutToolStripMenuItem.Enabled = True
            .LoginToolStripMenuItem.Enabled = False
            .HelpToolStripMenuItem.Enabled = True
            .WindowToolStripMenuItem.Visible = True
            .PanelTransaksi.Visible = True
            .BtnNotif.Visible = True
        End With
    End Sub

    Private Function GetTaskDescription(ByVal stepNumber As Integer, ByVal totalSteps As Integer) As String
        Dim taskDescriptions As Dictionary(Of Integer, String())

        ' Deskripsi langkah untuk masing-masing total langkah
        taskDescriptions = New Dictionary(Of Integer, String()) From {
            {5, New String() {
            "Memeriksa daftar jatuh tempo dan Mengambil data komputer dan rekening kas bank",
            "Mengupdate total bon dan total bayar karyawan, serta piutang dan supliyer",
            "Mengambil data master perusahaan",
            "Menyiapkan antarmuka setelah semua proses selesai"}},
            {9, New String() {
            "Mengambil data komputer dan rekening kas bank",
            "Mengupdate bon karyawan, hutang, dan jatuh  tempo hutang",
            "Mengambil data master perusahaan",
            "Mengambil hak akses pengguna",
            "Mereset barang di toko dan gudang",
            "Memperbarui semua barang di toko, proses ini agak lama mohon ditungu",
            "Memperbarui semua barang di gudang, proses ini agak lama mohon ditungu",
            "Menghitung semua kode",
            "Menyiapkan antarmuka setelah semua proses selesai"}},
            {6, New String() {
             "Mengupdate bon karyawan, hutang, saldo rekening, dan jatuh  tempo hutang",
             "Update data jatuh tempo",
             "Mereset barang di toko dan gudang",
             "Memperbarui semua barang di toko, proses ini agak lama mohon ditungu",
             "Memperbarui semua barang di gudang, proses ini agak lama mohon ditungu",
             "Menghitung semua kode"}}}


        ' Mengembalikan deskripsi berdasarkan stepNumber dan totalSteps
        If taskDescriptions.ContainsKey(totalSteps) AndAlso stepNumber >= 1 AndAlso stepNumber <= taskDescriptions(totalSteps).Length Then
            Return taskDescriptions(totalSteps)(stepNumber - 1)
        Else
            Return "Langkah tidak dikenal"
        End If
    End Function


    Public Sub ResetAllBarangToko()
        ' Query untuk mereset semua kolom terkait TOKO di tbl_barang
        Dim resetQuery As String = "UPDATE tbl_barang SET " &
                                   "TAMBAH_TOKO = 0, " &
                                   "KURANG_TOKO = 0, " &
                                   "PEMBELIAN_TOKO = 0, " &
                                   "PENJUALAN_TOKO = 0, " &
                                   "RETUR_BELI_TOKO = 0, " &
                                   "RETUR_JUAL_TOKO = 0, " &
                                   "OPNAME_TOKO = 0, " &
                                   "TRANSFER_STOK_MASUK_TOKO = 0, " &
                                   "TRANSFER_STOK_KELUAR_TOKO = 0, " &
                                   "TRANSFER_BARANG_MASUK_TOKO = 0, " &
                                   "TRANSFER_BARANG_KELUAR_TOKO = 0"

        ' Eksekusi query untuk reset semua kolom
        Using resetCmd As New MySqlCommand(resetQuery, conn)
            resetCmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub ResetAllBarangGudang()
        ' Query untuk mereset semua kolom terkait GUDANG di tbl_barang
        Dim resetQuery As String = "UPDATE tbl_barang SET " &
                                   "TAMBAH_GUDANG = 0, " &
                                   "KURANG_GUDANG = 0, " &
                                   "PEMBELIAN_GUDANG = 0, " &
                                   "PENJUALAN_GUDANG = 0, " &
                                   "RETUR_BELI_GUDANG = 0, " &
                                   "RETUR_JUAL_GUDANG = 0, " &
                                   "OPNAME_GUDANG = 0, " &
                                   "TRANSFER_STOK_MASUK_GUDANG = 0, " &
                                   "TRANSFER_STOK_KELUAR_GUDANG = 0, " &
                                   "TRANSFER_BARANG_MASUK_GUDANG = 0, " &
                                   "TRANSFER_BARANG_KELUAR_GUDANG = 0"

        ' Eksekusi query untuk reset semua kolom
        Using resetCmd As New MySqlCommand(resetQuery, conn)
            resetCmd.ExecuteNonQuery()
        End Using
    End Sub



    Public Sub BacaHakAkseUser()
        Dim UserLevel As String = FormUtama.SLevel.Text

        ' Permissions for MenuStrips
        FormUtama.MenuMaster.Visible = ModulHakAkses.BacaHakAksesDariCache("MASTER")(0)
        FormUtama.MenuTransaksi.Visible = ModulHakAkses.BacaHakAksesDariCache("TRANSAKSI")(0)
        FormUtama.MenuJurnal.Visible = ModulHakAkses.BacaHakAksesDariCache("JURNAL")(0)
        FormUtama.MenuKaryawan.Visible = ModulHakAkses.BacaHakAksesDariCache("MENUKARYAWAN")(0)
        FormUtama.MenuLaporan.Visible = ModulHakAkses.BacaHakAksesDariCache("LAPORAN")(0)
        FormUtama.MenuUtility.Visible = ModulHakAkses.BacaHakAksesDariCache("UTILITY")(0)
        FormUtama.MenuPosting.Visible = ModulHakAkses.BacaHakAksesDariCache("POSTING")(0)

        ' Dictionary for ToolStripMenuItems
        Dim toolStripItems As New Dictionary(Of String, ToolStripMenuItem) From {
            {"MASTER GAJI", FormUtama.MasterGajiToolStripMenuItem},
            {"BON", FormUtama.BonKaryawanToolStripMenuItem},
            {"BAYAR", FormUtama.BayarBonDiluarGajiToolStripMenuItem},
            {"LAP BON", FormUtama.LaporanBonToolStripMenuItem},
            {"LAP BON  KAR", FormUtama.LaporanBonPerKaryawanToolStripMenuItem},
            {"GAJI", FormUtama.GajiKaryawanToolStripMenuItem},
            {"LAP GAJI", FormUtama.LaporanGajiToolStripMenuItem},
            {"Mutasi saldo", FormUtama.MutasiSaldoToolStripMenuItem},
            {"Mutasi barang", FormUtama.MutasiBarangToolStripMenuItem},
            {"Jurnal Umum", FormUtama.JurnalUmumToolStripMenuItem},
            {"Neraca", FormUtama.NeracaToolStripMenuItem},
            {"Buku Besar", FormUtama.BukuBesarToolStripMenuItem},
            {"Buku Besar Pembantu", FormUtama.BukuBesarPembantuToolStripMenuItem},
            {"Lap Pembelian", FormUtama.PembelianToolStripMenuItem},
            {"Lap Penjualan", FormUtama.PenjualanToolStripMenuItem},
            {"Jual PPnNonPPn", FormUtama.PenjualanPPNNonPPNToolStripMenuItem},
            {"Retur Beli", FormUtama.ReturPembelianToolStripMenuItem},
            {"Retur Jual", FormUtama.ReturPenjualanToolStripMenuItem},
            {"Hutang", FormUtama.HutangToolStripMenuItem},
            {"Piutang", FormUtama.PiutangToolStripMenuItem},
            {"Kas Penjualan", FormUtama.KasPenjualanToolStripMenuItem},
            {"Transfer Stok", FormUtama.TransferStokToolStripMenuItem},
            {"Transfer Barang", FormUtama.TransferBarangToolStripMenuItem},
            {"Stok Opname", FormUtama.StokOpnameToolStripMenuItem},
            {"Stok Barang", FormUtama.StokBarangToolStripMenuItem},
            {"Grafik", FormUtama.GrafikToolStripMenuItem},
            {"History", FormUtama.HistoryToolStripMenuItem},
            {"Database", FormUtama.DatabaseToolStripMenuItem},
            {"Backup Database", FormUtama.BackupDatabaseToolStripMenuItem},
            {"Restore Database", FormUtama.RestoreDatabaseToolStripMenuItem},
            {"Perbaiki Database", FormUtama.PerbaikiDatabaseToolStripMenuItem},
            {"Setting Printer", FormUtama.SettingPrinterToolStripMenuItem}
        }

        ' Apply permissions to ToolStripMenuItems
        For Each moduleName As String In toolStripItems.Keys
            toolStripItems(moduleName).Visible = ModulHakAkses.BacaHakAksesDariCache(moduleName)(0)
        Next

        ' Dictionary for other controls
        Dim controls As New Dictionary(Of String, Control) From {
            {"Toko", FormUtama.BtnToko},
            {"Barang", FormUtama.BtnBarang},
            {"Pelanggan", FormUtama.BTnPelanggan},
            {"Supplier", FormUtama.BtnSupliyer},
            {"Tabel Referensi", FormUtama.BtnTabelRef},
            {"Armada", FormUtama.BtnArmada},
            {"Karyawan", FormUtama.BtnKaryawan},
            {"User", FormUtama.BtnUser},
            {"Hak Akses", FormUtama.BtnHakAksesUser},
            {"Pembelian", FormUtama.BtnBelanja},
            {"Penjualan", FormUtama.BtnPenjualan},
            {"Retur Pembelian", FormUtama.BtnRetuBelanja},
            {"Retur Penjualan", FormUtama.BtnReturPenjualan},
            {"Bayar Hutang", FormUtama.BtnBayarHutang},
            {"Bayar Piutang", FormUtama.BtnBayarPiutang},
            {"Transfer Stok", FormUtama.BtnPindahStok},
            {"Transfer Barang", FormUtama.BtnTransferBarang},
            {"Stok Opname", FormUtama.BtnStokOpname},
            {"Surat Jalan", FormUtama.BtnSuratJalan}
        }

        ' Apply permissions to other controls
        For Each moduleName As String In controls.Keys
            controls(moduleName).Visible = ModulHakAkses.BacaHakAksesDariCache(moduleName)(0)
        Next

        If FormUtama.SLevel.Text = "Master" Then
            With FormUtama
                .QueryDatabaseToolStripMenuItem.Visible = True
                .HapusTransaksiTokoToolStripMenuItem.Visible = True
                .HapusTransaksiGudangToolStripMenuItem.Visible = True
            End With
        Else
            With FormUtama
                .QueryDatabaseToolStripMenuItem.Visible = False
                .HapusTransaksiTokoToolStripMenuItem.Visible = False
                .HapusTransaksiGudangToolStripMenuItem.Visible = False
            End With
        End If

    End Sub


End Class
