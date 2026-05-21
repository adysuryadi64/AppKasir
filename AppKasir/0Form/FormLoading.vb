Public Class FormLoading

    Private Sub FormLoading_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TerapkanTheme(Me)
        ' Label progress & peringatan — sesuaikan warna agar terbaca di atas background tema
        LabelProgress.ForeColor = ModuleTheme.C(ModuleTheme.L_TransNilai, ModuleTheme.D_TransNilai)
        Label1.ForeColor = ModuleTheme.C(ModuleTheme.L_SurfaceFore, ModuleTheme.D_SurfaceFore)
        Label2.ForeColor = ModuleTheme.C(ModuleTheme.L_StatusBelumLunas, ModuleTheme.D_StatusBelumLunas)
    End Sub

    Public Sub MulaiLoading()
        Dim totalLangkah As Integer = 7

        For i As Integer = 1 To totalLangkah
            UpdateStatus(i, totalLangkah, GetTaskDescriptionLoading(i))

            Try
                Select Case i
                    Case 1
                        MuatSemuaPengaturan()
                        PastikanPerilakuLengkap()
                        FormUtama.StatusNamaPC.Text = AppStatusKomputer
                    Case 2
                        UpdateTotalBonDanTotalBayarKaryawan()
                    Case 3
                        UpdatePiutangDibayar()
                        UpdateSupliyerFromPembelianHutangDibayar()
                        ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()
                    Case 4
                        NotifikasiJatuhTempo.JumlahJatuhTempo()
                    Case 5
                        AmbilDataMasterPerusahaan()
                    Case 6
                        BacaHakAkseUser()
                        FormGeneralSetting.SinkronkanHakAksesTanpaDuplikat()
                        ModulHakAkses.CacheGeneralSetting()
                        ModulHakAkses.CacheBatasSatuan()
                        FormHakUser.SinkronkanDatabaseDenganTemplate()
                        ModulHakAkses.CacheHakAksesUser(FormUtama.StatusLevelUser.Text)
                    Case 7
                        UpdateUIComponents()
                End Select
            Catch ex As MySqlException
                If TawarMigrasi(ex) Then
                    ' Skema DB belum update — hentikan loading, user perlu restart setelah migrasi
                    Close()
                    Return
                Else
                    MessageBox.Show($"Kesalahan saat loading langkah {i}: {ex.Message}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try
        Next

        Close()
    End Sub
    Public Sub MulaiPosting(ByVal Jenis As String)
        Dim totalLangkah As Integer = 7

        For i As Integer = 1 To totalLangkah
            UpdateStatus(i, totalLangkah, GetTaskDescriptionPosting(i, Jenis))

            Try
                Select Case i
                    Case 1
                        FormLapNeracaLR.HITUNGSEMUASALDO()
                    Case 2
                        UpdateTotalBonDanTotalBayarKaryawan()
                        UpdatePiutangDibayar()
                        UpdateSupliyerFromPembelianHutangDibayar()
                        ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()
                    Case 3
                        NotifikasiJatuhTempo.JumlahJatuhTempo()
                    Case 4
                        If Jenis = "Toko" Then
                            ResetAllBarangToko()
                        ElseIf Jenis = "Gudang" Then
                            ResetAllBarangGudang()
                        Else
                            ResetAllBarangToko()
                            ResetAllBarangGudang()
                        End If
                    Case 5
                        If Jenis <> "Gudang" Then UpdateAllBarangTokoModule()
                    Case 6
                        If Jenis <> "Toko" Then UpdateAllBarangGudangModule()
                    Case 7
                        If Jenis = "Toko" Then
                            HitungStokToko()
                        ElseIf Jenis = "Gudang" Then
                            HitungStokGudang()
                        Else
                            HitungSemuaKode()
                        End If
                End Select
            Catch ex As MySqlException
                If TawarMigrasi(ex) Then
                    Close()
                    Return
                Else
                    MessageBox.Show($"Kesalahan saat posting langkah {i}: {ex.Message}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try
        Next

        Close()
    End Sub

    Private Function CheckIfProcessedToday(ByVal today As Date) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM Temp_Loading WHERE Tanggal = @TanggalHariIni"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@TanggalHariIni", today)
            Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
        End Using
    End Function

    Private Sub ClearTempLoadingTable()
        Dim query As String = "DELETE FROM Temp_Loading"
        Using cmd As New MySqlCommand(query, conn)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InsertTodayDate(ByVal today As Date)
        Dim query As String = "INSERT INTO Temp_Loading (Tanggal) VALUES (@TanggalHariIni)"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@TanggalHariIni", today)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub UpdateStatus(ByVal currentStep As Integer, ByVal totalSteps As Integer, ByVal taskDescription As String)
        Dim completionPercentage As Integer = (currentStep / totalSteps) * 100
        ProgressBar1.Value = completionPercentage
        LabelProgress.Text = String.Format("{0} ({1}% - 100%)", taskDescription, completionPercentage)
        Application.DoEvents()
        If completionPercentage < 100 Then
            Threading.Thread.Sleep(200)
        End If
    End Sub

    Private Sub UpdateUIComponents()
        With FormUtama
            .LogOutToolStripMenuItem.Enabled = True
            .LoginToolStripMenuItem.Enabled = False
            .HelpToolStripMenuItem.Enabled = True
            .WindowToolStripMenuItem.Visible = True
            .PanelTransaksi.Visible = True
            .BtnNotif.Visible = True
        End With
    End Sub

    Private Function GetTaskDescriptionLoading(ByVal langkah As Integer) As String
        If langkah = 1 Then
            Return "Mengambil data komputer dan rekening kas bank..."
        ElseIf langkah = 2 Then
            Return "Menghitung ulang saldo bon karyawan..."
        ElseIf langkah = 3 Then
            Return "Menghitung ulang piutang pelanggan dan hutang supplier..."
        ElseIf langkah = 4 Then
            Return "Memeriksa jatuh tempo hutang..."
        ElseIf langkah = 5 Then
            Return "Mengambil data master perusahaan..."
        ElseIf langkah = 6 Then
            Return "Memuat hak akses pengguna..."
        ElseIf langkah = 7 Then
            Return "Menyiapkan tampilan..."
        Else
            Return "Memproses..."
        End If
    End Function

    Private Function GetTaskDescriptionPosting(ByVal langkah As Integer, ByVal jenis As String) As String
        If langkah = 1 Then
            Return "Menghitung ulang saldo neraca dari jurnal umum..."
        ElseIf langkah = 2 Then
            Return "Menghitung ulang saldo bon, piutang, hutang, dan akun jurnal..."
        ElseIf langkah = 3 Then
            Return "Memeriksa jatuh tempo hutang..."
        ElseIf langkah = 4 Then
            Return "Mereset data stok " & If(jenis = "Semua", "toko dan gudang", jenis.ToLower()) & "..."
        ElseIf langkah = 5 Then
            Return If(jenis <> "Gudang", "Memperbarui history stok toko...", "Dilewati (hanya gudang)")
        ElseIf langkah = 6 Then
            Return If(jenis <> "Toko", "Memperbarui history stok gudang...", "Dilewati (hanya toko)")
        ElseIf langkah = 7 Then
            Return "Menghitung ulang stok " & If(jenis = "Semua", "toko dan gudang", jenis.ToLower()) & "..."
        Else
            Return "Memproses..."
        End If
    End Function

    Public Sub ResetAllBarangToko()
        Dim resetQuery As String = "UPDATE tbl_barang SET " &
                                   "TAMBAH_TOKO = 0, KURANG_TOKO = 0, PEMBELIAN_TOKO = 0, " &
                                   "PENJUALAN_TOKO = 0, RETUR_BELI_TOKO = 0, RETUR_JUAL_TOKO = 0, " &
                                   "OPNAME_TOKO = 0, TRANSFER_STOK_MASUK_TOKO = 0, " &
                                   "TRANSFER_STOK_KELUAR_TOKO = 0, TRANSFER_BARANG_MASUK_TOKO = 0, " &
                                   "TRANSFER_BARANG_KELUAR_TOKO = 0, " &
                                   "TRANSFER_CABANG_MASUK_TOKO = 0, TRANSFER_CABANG_KELUAR_TOKO = 0"
        Using resetCmd As New MySqlCommand(resetQuery, conn)
            resetCmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub ResetAllBarangGudang()
        Dim resetQuery As String = "UPDATE tbl_barang SET " &
                                   "TAMBAH_GUDANG = 0, KURANG_GUDANG = 0, PEMBELIAN_GUDANG = 0, " &
                                   "PENJUALAN_GUDANG = 0, RETUR_BELI_GUDANG = 0, RETUR_JUAL_GUDANG = 0, " &
                                   "OPNAME_GUDANG = 0, TRANSFER_STOK_MASUK_GUDANG = 0, " &
                                   "TRANSFER_STOK_KELUAR_GUDANG = 0, TRANSFER_BARANG_MASUK_GUDANG = 0, " &
                                   "TRANSFER_BARANG_KELUAR_GUDANG = 0, " &
                                   "TRANSFER_CABANG_MASUK_GUDANG = 0, TRANSFER_CABANG_KELUAR_GUDANG = 0"
        Using resetCmd As New MySqlCommand(resetQuery, conn)
            resetCmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub BacaHakAkseUser()
        FormUtama.MenuMaster.Visible = ModulHakAkses.BacaHakAksesDariCache("MASTER")(0)
        FormUtama.MenuTransaksi.Visible = ModulHakAkses.BacaHakAksesDariCache("TRANSAKSI")(0)
        FormUtama.MenuJurnal.Visible = ModulHakAkses.BacaHakAksesDariCache("JURNAL")(0)
        FormUtama.MenuKaryawan.Visible = ModulHakAkses.BacaHakAksesDariCache("MENUKARYAWAN")(0)
        FormUtama.MenuLaporan.Visible = ModulHakAkses.BacaHakAksesDariCache("LAPORAN")(0)
        FormUtama.MenuUtility.Visible = ModulHakAkses.BacaHakAksesDariCache("UTILITY")(0)
        FormUtama.MenuPosting.Visible = ModulHakAkses.BacaHakAksesDariCache("POSTING")(0)

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
            {"Lap Pembelian", FormUtama.PembelianToolStripMenuItem1},
            {"Lap Pembelian Detail", FormUtama.PembelianDetailToolStripMenuItem},
            {"Lap Pembelian Barang", FormUtama.PembelianBarangToolStripMenuItem},
            {"Lap Pembelian Hutang", FormUtama.PembelianDihutangToolStripMenuItem},
            {"Rekap Penjualan Nota", FormUtama.RekapPenjualanByNotaToolStripMenuItem},
            {"Rekap Penjualan Barang", FormUtama.RekapPenjualanToolStripMenuItem},
            {"Lap Penjualan", FormUtama.PenjualanToolStripMenuItem1},
            {"Lap Penjualan Detail", FormUtama.PenjualanDetailToolStripMenuItem},
            {"Lap Penjualan Barang", FormUtama.PenjualanBarangToolStripMenuItem},
            {"Lap Penjualan Hutang", FormUtama.PenjualanTerhutangToolStripMenuItem},
            {"Lap Penjualan Sales", FormUtama.PenjualanSalesToolStripMenuItem},
            {"Lap Penjualan Qty", FormUtama.PenjualanQtyToolStripMenuItem},
            {"Jual PPnNonPPn", FormUtama.PenjualanPPNNonPPNToolStripMenuItem},
            {"Retur Beli", FormUtama.ReturPembelianToolStripMenuItem1},
            {"Retur Beli Detail", FormUtama.ReturPembelianDetailToolStripMenuItem},
            {"Retur Beli Barang", FormUtama.ReturPembelianBarangToolStripMenuItem},
            {"Retur Jual", FormUtama.ReturPenjualanToolStripMenuItem1},
            {"Retur Jual Detail", FormUtama.ReturPenjualanDetailToolStripMenuItem},
            {"Retur Jual Barang", FormUtama.ReturPenjualanBarangToolStripMenuItem},
            {"Hutang By Pembelian", FormUtama.ByTanggalBelanjaToolStripMenuItem},
            {"Hutang By Pelunasan", FormUtama.ByTanggalPelunasanToolStripMenuItem},
            {"Hutang By Jatuh Tempo", FormUtama.ByTanggalJatuhTempoToolStripMenuItem},
            {"Rekap Bayar Hutang", FormUtama.RekapBayarHutangToolStripMenuItem},
            {"Piutang By Penjualan", FormUtama.ByTanggalPenjualanToolStripMenuItem},
            {"Piutang By Pelunasan", FormUtama.ByTanggalPelunasanToolStripMenuItem1},
            {"Piutang By Jatuh Tempo", FormUtama.ByTanggalJatuhTempoToolStripMenuItem1},
            {"Rekap Bayar Piutang", FormUtama.RekapBayarPiutangToolStripMenuItem},
            {"Kas Penjualan", FormUtama.KasPenjualanToolStripMenuItem},
            {"Lap Transfer Stok", FormUtama.TransferStokToolStripMenuItem},
            {"Lap Transfer Barang", FormUtama.TransferBarangToolStripMenuItem1},
            {"Lap Transfer Barang Detail", FormUtama.TransferBarangDetailToolStripMenuItem},
            {"Lap Stok Opname", FormUtama.StokOpnameToolStripMenuItem},
            {"Stok Barang", FormUtama.StokBarangToolStripMenuItem1},
            {"Kartu Stok", FormUtama.KartuStokToolStripMenuItem1},
            {"Barang Terlaris", FormUtama.StokBarangTerlarisToolStripMenuItem},
            {"Barang Tidak Bergerak", FormUtama.StokBarangTakBergerakToolStripMenuItem},
            {"Stok Minimum", FormUtama.StokMinimumToolStripMenuItem1},
            {"Stok Masa Lampau", FormUtama.StokLampauToolStripMenuItem},
            {"Ranking Supplier", FormUtama.RankingSupplierToolStripMenuItem},
            {"Ranking Kasir", FormUtama.RankingKasirUserPenjualanToolStripMenuItem},
            {"Ranking Barang Terbanyak Dibeli", FormUtama.RankingBarangTerbanyakDibeliToolStripMenuItem},
            {"Ranking Pelanggan Piutang Terbesar", FormUtama.RankingPelangganPiutangTerbesarToolStripMenuItem},
            {"Ranking Supplier Hutang Terbesar", FormUtama.RankingSupplierHutangTerbesarToolStripMenuItem},
            {"Omset Per Pelanggan", FormUtama.OmsetPerPelangganToolStripMenuItem},
            {"Omset Per Kategori", FormUtama.OmsetPerKategoriToolStripMenuItem},
            {"Grafik", FormUtama.GrafikToolStripMenuItem},
            {"History", FormUtama.HistoryToolStripMenuItem},
            {"Posting Toko", FormUtama.PostingTokoToolStripMenuItem},
            {"Posting Gudang", FormUtama.PostingGudangToolStripMenuItem},
            {"Posting Semua", FormUtama.PostingSemuaToolStripMenuItem},
            {"Database", FormUtama.DatabaseToolStripMenuItem},
            {"Backup Database", FormUtama.BackupDatabaseToolStripMenuItem},
            {"Restore Database", FormUtama.RestoreDatabaseToolStripMenuItem},
            {"Perbaiki Database", FormUtama.PerbaikiDatabaseToolStripMenuItem},
            {"Setting Printer", FormUtama.SettingPrinterToolStripMenuItem}
        }

        For Each moduleName As String In toolStripItems.Keys
            toolStripItems(moduleName).Visible = ModulHakAkses.BacaHakAksesDariCache(moduleName)(0)
        Next

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
            {"Surat Jalan", FormUtama.BtnSuratJalan},
            {"Transfer Cabang", FormUtama.BtnKirimCabang},
            {"Cabang", FormUtama.BtnKirimCabang},
            {"Cabang Master", FormUtama.BtnMasterCabang},
            {"Sales Order", FormUtama.BtnSalesOrder},
            {"Pembayaran Sales Order", FormUtama.BtnSalesOrder}
            }

        For Each moduleName As String In controls.Keys
            controls(moduleName).Visible = ModulHakAkses.BacaHakAksesDariCache(moduleName)(0)
        Next

        Dim isMaster As Boolean = FormUtama.StatusLevelUser.Text = "Master"
        With FormUtama
            .QueryDatabaseToolStripMenuItem.Visible = isMaster
            .HapusTransaksiTokoToolStripMenuItem.Visible = isMaster
            .HapusTransaksiGudangToolStripMenuItem.Visible = isMaster
            .HapusTransaksiSemuaToolStripMenuItem.Visible = isMaster
        End With
    End Sub

End Class
