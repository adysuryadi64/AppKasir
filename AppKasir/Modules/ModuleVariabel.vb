Module ModuleVariabel
    Public NamaUser As String = ""
    Public LevelUser As String = ""
    Public KodeUser As String = ""
    Public WaktuLogin As DateTime = DateTime.MinValue
    Public AplikasiSedangUpdate As Boolean = False

    ''' <summary>
    ''' Cek apakah MySqlException disebabkan kolom/tabel belum ada,
    ''' lalu tawarkan buka FormMigrasiDB. Return True jika error migrasi.
    ''' </summary>
    Public Function TawarMigrasi(ex As MySqlException) As Boolean
        If ex Is Nothing Then Return False
        Dim pesanEx As String = ex.Message.ToLower()
        ' MySQL error 1054 = Unknown column
        ' MySQL error 1146 = Table doesn't exist
        ' MySQL error 1060 = Duplicate column (migrasi sudah jalan sebagian)
        Dim adalahMasalahSkema As Boolean =
            ex.Number = 1054 OrElse   ' Unknown column
            ex.Number = 1146 OrElse   ' Table doesn't exist
            ex.Number = 1060 OrElse   ' Duplicate column
            pesanEx.Contains("unknown column") OrElse
            pesanEx.Contains("doesn't exist") OrElse
            pesanEx.Contains("table") AndAlso pesanEx.Contains("exist")

        If adalahMasalahSkema Then
            Dim jawab As DialogResult = MessageBox.Show(
                "Database perlu diperbarui — ada kolom atau tabel yang belum ada." & vbCrLf & vbCrLf &
                "Detail: " & ex.Message & vbCrLf & vbCrLf &
                "Buka Form Migrasi Database sekarang?",
                "Migrasi Database Diperlukan",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If jawab = DialogResult.Yes Then
                Using frm As New FormMigrasiDB()
                    frm.ShowDialog()
                End Using
            End If
            Return True
        End If
        Return False
    End Function

#Region "Akun"
    Public Function AmbilAkun(ParamArray tipeAkun() As String) As List(Of String)
        EnsureConnectionReady()
        Dim result As New HashSet(Of String)()

        Dim kondisi As String = String.Join(" OR ", tipeAkun.Select(Function(t) $"Type_Akun LIKE '{t}'"))

        Dim query As String = $"SELECT Nama_Akun 
                           FROM tbl_datareferensi 
                           WHERE {kondisi}
                           ORDER BY Kode_akun ASC"

        Using cmd As New MySqlCommand(query, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    result.Add(rd("Nama_Akun").ToString())
                End While
            End Using
        End Using

        Return result.ToList()
    End Function

    Public Sub IsiComboBoxAkun(cb As ComboBox, ParamArray tipeAkun() As String)
        Dim data = AmbilAkun(tipeAkun)

        cb.BeginUpdate()
        cb.Items.Clear()

        For Each item In data
            cb.Items.Add(item)
        Next

        cb.EndUpdate()
    End Sub

#End Region

#Region "Perusahaan"
    Public KODE_PERUSAHAAN As String = ""
    Public NAMA_PERUSAHAAN As String = ""
    Public NAMA_CLOUD As String = ""
    Public ALAMAT_CLOUD As String = ""
    Public ALAMAT_PERUSAHAAN As String = ""
    Public KOTA_PERUSAHAAN As String = ""
    Public KONTAK_PERUSAHAAN As String = ""
    Public PEMILIK_PERUSAHAAN As String = ""
    Public FOOTER1 As String = ""
    Public FOOTER2 As String = ""
    Public FOOTER3 As String = ""
    Public JENIS_TUTUP_BULAN As String = ""
    Public TANGGAL_TUTUP_BULAN As Integer = 1

    Public KODE_REK_BARANG As String = ""
    Public NAMA_REK_BARANG As String = ""
    Public LAWAN_KODE_REK_BARANG As String = ""
    Public LAWAN_NAMA_REK_BARANG As String = ""
    Public Kode_rek_Beli_toko As String = ""
    Public nama_rek_Beli_toko As String = ""
    Public Kode_rek_Beli_Gudang As String = ""
    Public nama_rek_Beli_Gudang As String = ""
    Public Kode_rek_Jual_Toko As String = ""
    Public nama_rek_Jual_Toko As String = ""
    Public Kode_rek_Jual_Gudang As String = ""
    Public nama_rek_Jual_Gudang As String = ""
    Public Kode_rek_Hutang_Beli As String = ""
    Public nama_rek_Hutang_Beli As String = ""
    Public Kode_rek_Piutang_Jual As String = ""
    Public nama_rek_Piutang_Jual As String = ""
    Public Kode_rek_Retur_Pembelian_Toko As String = ""
    Public nama_rek_Retur_Pembelian_Toko As String = ""
    Public Kode_rek_Retur_Penjualan_Toko As String = ""
    Public nama_rek_Retur_Penjualan_Toko As String = ""
    Public Kode_rek_Retur_Pembelian_Gudang As String = ""
    Public nama_rek_Retur_Pembelian_Gudang As String = ""
    Public Kode_rek_Retur_Penjualan_Gudang As String = ""
    Public nama_rek_Retur_Penjualan_Gudang As String = ""
    Public Kode_rek_Bon_Karyawan As String = ""
    Public nama_rek_Bon_Karyawan As String = ""
    Public Kode_rek_Gaji_Karyawan As String = ""
    Public nama_rek_Gaji_Karyawan As String = ""
    Public Kode_rek_Bayar_Hutang As String = ""
    Public nama_rek_Bayar_Hutang As String = ""
    Public Kode_rek_Bayar_Piutang As String = ""
    Public nama_rek_Bayar_Piutang As String = ""
    Public Kode_rek_Transfer_Jual As String = ""
    Public nama_rek_Transfer_Jual As String = ""


    Public Sub AmbilDataMasterPerusahaan()
        EnsureConnectionReady()
        Dim sql As String = "SELECT KODE, NAMA_CLOUD, ALAMAT_CLOUD, NAMA, ALAMAT, KOTA, HP, PEMILIK, FOOTER1, FOOTER2, FOOTER3, System_tutup_bulan, Tanggal_Tutup_bulan, " &
                            "KODE_REK_BARANG, NAMA_REK_BARANG, lawan_nama_rek_barang, lawan_Kode_rek_barang, " &
                            "Kode_rek_Beli_toko, nama_rek_Beli_toko, Kode_rek_Beli_Gudang, nama_rek_Beli_Gudang, " &
                            "Kode_rek_Jual_Toko, nama_rek_Jual_Toko, Kode_rek_Jual_Gudang, nama_rek_Jual_Gudang, " &
                            "nama_rek_Hutang_Beli, Kode_rek_Hutang_Beli, nama_rek_Piutang_Jual, Kode_rek_Piutang_Jual, " &
                            "NAMA_REK_RETUR_PEMBELIAN_TOKO, KODE_REK_RETUR_PEMBELIAN_TOKO, " &
                            "NAMA_REK_RETUR_PENJUALAN_TOKO, KODE_REK_RETUR_PENJUALAN_TOKO, " &
                            "NAMA_REK_RETUR_PEMBELIAN_GUDANG, KODE_REK_RETUR_PEMBELIAN_GUDANG, " &
                            "NAMA_REK_RETUR_PENJUALAN_GUDANG, KODE_REK_RETUR_PENJUALAN_GUDANG, " &
                            "NAMA_REK_BON_KARYAWAN, KODE_REK_BON_KARYAWAN, " &
                            "NAMA_REK_GAJI_KARYAWAN, KODE_REK_GAJI_KARYAWAN, " &
                            "NAMA_REK_BAYAR_HUTANG, KODE_REK_BAYAR_HUTANG, " &
                            "NAMA_REK_BAYAR_PIUTANG, KODE_REK_BAYAR_PIUTANG, " &
                            "NAMA_REK_TRANSFER_JUAL, KODE_REK_TRANSFER_JUAL " &
                            "FROM tbl_perusahaan"

        Using cmd As New MySqlCommand(sql, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Set nilai ke variabel-variabel perusahaan
                    KODE_PERUSAHAAN = reader("KODE").ToString()
                    NAMA_CLOUD = reader("NAMA_CLOUD").ToString()
                    ALAMAT_CLOUD = reader("ALAMAT_CLOUD").ToString()
                    NAMA_PERUSAHAAN = reader("NAMA").ToString()
                    ALAMAT_PERUSAHAAN = reader("ALAMAT").ToString()
                    KOTA_PERUSAHAAN = reader("KOTA").ToString()
                    KONTAK_PERUSAHAAN = reader("HP").ToString()
                    PEMILIK_PERUSAHAAN = reader("PEMILIK").ToString()
                    FOOTER1 = reader("FOOTER1").ToString()
                    FOOTER2 = reader("FOOTER2").ToString()
                    FOOTER3 = reader("FOOTER3").ToString()
                    JENIS_TUTUP_BULAN = reader("System_tutup_bulan").ToString()
                    TANGGAL_TUTUP_BULAN = CInt(If(IsDBNull(reader("Tanggal_Tutup_bulan")), 1, reader("Tanggal_Tutup_bulan")))
                    KODE_REK_BARANG = reader("KODE_REK_BARANG").ToString()
                    NAMA_REK_BARANG = reader("NAMA_REK_BARANG").ToString()
                    LAWAN_KODE_REK_BARANG = reader("lawan_Kode_rek_barang").ToString()
                    LAWAN_NAMA_REK_BARANG = reader("lawan_nama_rek_barang").ToString()
                    Kode_rek_Beli_toko = reader("Kode_rek_Beli_toko").ToString()
                    nama_rek_Beli_toko = reader("nama_rek_Beli_toko").ToString()
                    Kode_rek_Beli_Gudang = reader("Kode_rek_Beli_Gudang").ToString()
                    nama_rek_Beli_Gudang = reader("nama_rek_Beli_Gudang").ToString()
                    Kode_rek_Jual_Toko = reader("Kode_rek_Jual_Toko").ToString()
                    nama_rek_Jual_Toko = reader("nama_rek_Jual_Toko").ToString()
                    Kode_rek_Jual_Gudang = reader("Kode_rek_Jual_Gudang").ToString()
                    nama_rek_Jual_Gudang = reader("nama_rek_Jual_Gudang").ToString()
                    Kode_rek_Hutang_Beli = reader("Kode_rek_Hutang_Beli").ToString()
                    nama_rek_Hutang_Beli = reader("nama_rek_Hutang_Beli").ToString()
                    Kode_rek_Piutang_Jual = reader("Kode_rek_Piutang_Jual").ToString()
                    nama_rek_Piutang_Jual = reader("nama_rek_Piutang_Jual").ToString()
                    Kode_rek_Retur_Pembelian_Toko = reader("KODE_REK_RETUR_PEMBELIAN_TOKO").ToString()
                    nama_rek_Retur_Pembelian_Toko = reader("NAMA_REK_RETUR_PEMBELIAN_TOKO").ToString()
                    Kode_rek_Retur_Penjualan_Toko = reader("KODE_REK_RETUR_PENJUALAN_TOKO").ToString()
                    nama_rek_Retur_Penjualan_Toko = reader("NAMA_REK_RETUR_PENJUALAN_TOKO").ToString()
                    Kode_rek_Retur_Pembelian_Gudang = reader("KODE_REK_RETUR_PEMBELIAN_GUDANG").ToString()
                    nama_rek_Retur_Pembelian_Gudang = reader("NAMA_REK_RETUR_PEMBELIAN_GUDANG").ToString()
                    Kode_rek_Retur_Penjualan_Gudang = reader("KODE_REK_RETUR_PENJUALAN_GUDANG").ToString()
                    nama_rek_Retur_Penjualan_Gudang = reader("NAMA_REK_RETUR_PENJUALAN_GUDANG").ToString()
                    Kode_rek_Bon_Karyawan = reader("KODE_REK_BON_KARYAWAN").ToString()
                    nama_rek_Bon_Karyawan = reader("NAMA_REK_BON_KARYAWAN").ToString()
                    Kode_rek_Gaji_Karyawan = reader("KODE_REK_GAJI_KARYAWAN").ToString()
                    nama_rek_Gaji_Karyawan = reader("NAMA_REK_GAJI_KARYAWAN").ToString()
                    Kode_rek_Bayar_Hutang = reader("KODE_REK_BAYAR_HUTANG").ToString()
                    nama_rek_Bayar_Hutang = reader("NAMA_REK_BAYAR_HUTANG").ToString()
                    Kode_rek_Bayar_Piutang = reader("KODE_REK_BAYAR_PIUTANG").ToString()
                    nama_rek_Bayar_Piutang = reader("NAMA_REK_BAYAR_PIUTANG").ToString()
                    Kode_rek_Transfer_Jual = reader("KODE_REK_TRANSFER_JUAL").ToString()
                    nama_rek_Transfer_Jual = reader("NAMA_REK_TRANSFER_JUAL").ToString()

                    ' Mengisi data ke form yang relevan
                    FormUtama.Text = "KASIR LANCAR " & FormUtama.StatusLokasi.Text & " " & NAMA_PERUSAHAAN

                    FormStokOpnameBahan.TxtPerusahaan.Text = NAMA_PERUSAHAAN
                    FormLapTransferStok.TxtPerusahaan.Text = NAMA_PERUSAHAAN
                    FormHistory.TxtPerusahaan.Text = NAMA_PERUSAHAAN

                    ' Update label berjalan di FormPenjualan jika sedang terbuka
                    For Each frm As Form In FormUtama.MdiChildren
                        If TypeOf frm Is FormPenjualanLama Then
                            CType(frm, FormPenjualanLama).UpdateNamaPerusahaan(NAMA_PERUSAHAAN)
                        End If
                    Next

                    AmbilDataPeriodeTanggal()
                End If
            End Using
        End Using
    End Sub

    ' Deklarasi variabel tanggal awal dan akhir sebagai public
    Public tanggalAwalPeriodeKerja As Date
    Public tanggalAkhirPeriodeKerja As Date

    Public Sub AmbilDataPeriodeTanggal()
        ' Cek apakah TANGGAL_TUTUP_BULAN dapat dikonversi menjadi Integer
        Dim tanggalInput As Integer
        If Integer.TryParse(TANGGAL_TUTUP_BULAN.ToString(), tanggalInput) Then
            ' Dapatkan bulan dan tahun sekarang
            Dim bulanSekarang As Integer = DateTime.Now.Month ' Mendapatkan bulan sekarang
            Dim tahunSekarang As Integer = DateTime.Now.Year ' Mendapatkan tahun sekarang

            ' Tentukan tanggal awal dan akhir
            Dim tanggalAwal As Date
            Dim tanggalAkhir As Date

            If JENIS_TUTUP_BULAN = "Berdasar tanggal manual" Then
                ' Jika hari ini lebih besar dari tanggal input (10)
                If DateTime.Now.Day > tanggalInput Then
                    ' Tentukan tanggal awal dan akhir untuk periode berikutnya
                    tanggalAwal = New Date(tahunSekarang, bulanSekarang, tanggalInput + 1) ' 11 bulan ini
                    tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1) ' 10 bulan depan
                Else
                    ' Tentukan tanggal awal dan akhir untuk periode sebelumnya
                    If bulanSekarang = 1 Then
                        ' Jika bulan Januari, tanggal awal adalah 11 Desember tahun sebelumnya
                        tanggalAwal = New Date(tahunSekarang - 1, 12, tanggalInput + 1)
                        tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1) ' 10 Januari
                    Else
                        ' Jika tidak Januari, tanggal awal adalah 11 bulan sebelumnya
                        tanggalAwal = New Date(tahunSekarang, bulanSekarang - 1, tanggalInput + 1)
                        tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1) ' 10 bulan depan
                    End If
                End If

            Else
                ' Berdasar bulan saat ini
                tanggalAwal = New Date(tahunSekarang, bulanSekarang, 1) ' Tanggal pertama bulan ini
                tanggalAkhir = tanggalAwal.AddMonths(1).AddDays(-1) ' Tanggal terakhir bulan ini

                ' Cek apakah tanggal input lebih besar dari hari ini pada bulan ini
                If tanggalInput > DateTime.Now.Day Then
                    ' Jika tanggal input lebih besar, atur tanggal akhir ke akhir bulan ini
                    tanggalAkhir = New Date(tahunSekarang, bulanSekarang, 1).AddMonths(1).AddDays(-1)
                End If
            End If

            ' Menetapkan nilai ke variabel public
            tanggalAwalPeriodeKerja = tanggalAwal
            tanggalAkhirPeriodeKerja = tanggalAkhir

        Else
            ' Menangani jika TANGGAL_TUTUP_BULAN tidak valid
            MessageBox.Show("Tanggal tutup bulan tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

#End Region

#Region "Stok Barang"
    Public Sub HitungStokToko()
        ' Task 8.2b — wrapper ke sp_bat_stok_toko (hasil identik, diverifikasi Tests/Test-VB-vs-SP.sql)
        Try
            Using cmd As New MySqlCommand("CALL sp_bat_stok_toko()", conn)
                cmd.CommandTimeout = 120
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            If Not TawarMigrasi(TryCast(ex, MySqlException)) Then
                MessageBox.Show("Terjadi kesalahan (Toko): " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

    Public Sub HitungStokGudang()
        ' Task 8.2b — wrapper ke sp_bat_stok_gudang (hasil identik, diverifikasi Tests/Test-VB-vs-SP.sql)
        Try
            Using cmd As New MySqlCommand("CALL sp_bat_stok_gudang()", conn)
                cmd.CommandTimeout = 120
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            If Not TawarMigrasi(TryCast(ex, MySqlException)) Then
                MessageBox.Show("Terjadi kesalahan (Gudang): " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

    Public Sub HitungSemuaKode()
        ' Task 8.2b — wrapper ke sp_bat_stok_semua_barang (hasil identik, diverifikasi Tests/Test-VB-vs-SP.sql)
        Try
            Using cmd As New MySqlCommand("CALL sp_bat_stok_semua_barang()", conn)
                cmd.CommandTimeout = 120
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            If Not TawarMigrasi(TryCast(ex, MySqlException)) Then
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

    Public Sub HitungStokPerubahan(ByVal kode As String, ByVal transaction As MySqlTransaction)
        ' Task 8.2b — wrapper ke sp_hlp_stok_hitung (hasil identik dengan rumus inline lama)
        Using cmd As New MySqlCommand("CALL sp_hlp_stok_hitung(@kode)", conn, transaction)
            cmd.Parameters.AddWithValue("@kode", kode)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

#End Region

#Region "Posting Stok"
    Public Sub UpdateAllBarangTokoModule()
        Dim pivotQuery As String =
            "UPDATE tbl_barang b " &
            "LEFT JOIN (SELECT ID_BARANG, SUM(CASE WHEN JENIS='TAMBAH' THEN TOTAL_QTY ELSE 0 END) AS q_TAMBAH, " &
            "SUM(CASE WHEN JENIS='KURANG' THEN TOTAL_QTY ELSE 0 END) AS q_KURANG, " &
            "SUM(CASE WHEN JENIS='PEMBELIAN' THEN TOTAL_QTY ELSE 0 END) AS q_PEMBELIAN, " &
            "SUM(CASE WHEN JENIS='PENJUALAN' THEN TOTAL_QTY ELSE 0 END) AS q_PENJUALAN, " &
            "SUM(CASE WHEN JENIS='RETUR BELI' THEN TOTAL_QTY ELSE 0 END) AS q_RETUR_BELI, " &
            "SUM(CASE WHEN JENIS='RETUR JUAL' THEN TOTAL_QTY ELSE 0 END) AS q_RETUR_JUAL, " &
            "SUM(CASE WHEN JENIS='OPNAME' THEN TOTAL_QTY ELSE 0 END) AS q_OPNAME, " &
            "SUM(CASE WHEN JENIS='TRANSFER STOK MASUK' THEN TOTAL_QTY ELSE 0 END) AS q_TSM, " &
            "SUM(CASE WHEN JENIS='TRANSFER STOK KELUAR' THEN TOTAL_QTY ELSE 0 END) AS q_TSK, " &
            "SUM(CASE WHEN JENIS='TRANSFER BARANG MASUK' THEN TOTAL_QTY ELSE 0 END) AS q_TBM, " &
            "SUM(CASE WHEN JENIS='TRANSFER BARANG KELUAR' THEN TOTAL_QTY ELSE 0 END) AS q_TBK, " &
            "SUM(CASE WHEN JENIS='TRANSFER_CABANG_MASUK' THEN TOTAL_QTY ELSE 0 END) AS q_TCM, " &
            "SUM(CASE WHEN JENIS='TRANSFER_CABANG_KELUAR' THEN TOTAL_QTY ELSE 0 END) AS q_TCK " &
            "FROM HistoryBarang WHERE LOKASI='TOKO' GROUP BY ID_BARANG) h ON h.ID_BARANG = b.ID_BARANG " &
            "SET b.TAMBAH_TOKO                   = IFNULL(h.q_TAMBAH, 0), " &
            "    b.KURANG_TOKO                   = IFNULL(h.q_KURANG, 0), " &
            "    b.PEMBELIAN_TOKO                = IFNULL(h.q_PEMBELIAN, 0), " &
            "    b.PENJUALAN_TOKO                = IFNULL(h.q_PENJUALAN, 0), " &
            "    b.RETUR_BELI_TOKO               = IFNULL(h.q_RETUR_BELI, 0), " &
            "    b.RETUR_JUAL_TOKO               = IFNULL(h.q_RETUR_JUAL, 0), " &
            "    b.OPNAME_TOKO                   = IFNULL(h.q_OPNAME, 0), " &
            "    b.TRANSFER_STOK_MASUK_TOKO      = IFNULL(h.q_TSM, 0), " &
            "    b.TRANSFER_STOK_KELUAR_TOKO     = IFNULL(h.q_TSK, 0), " &
            "    b.TRANSFER_BARANG_MASUK_TOKO    = IFNULL(h.q_TBM, 0), " &
            "    b.TRANSFER_BARANG_KELUAR_TOKO   = IFNULL(h.q_TBK, 0), " &
            "    b.TRANSFER_CABANG_MASUK_TOKO    = IFNULL(h.q_TCM, 0), " &
            "    b.TRANSFER_CABANG_KELUAR_TOKO   = IFNULL(h.q_TCK, 0)"

        Using cmd As New MySqlCommand(pivotQuery, conn)
            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Public Sub UpdateAllBarangGudangModule()
        Dim pivotQuery As String =
            "UPDATE tbl_barang b " &
            "LEFT JOIN (SELECT ID_BARANG, SUM(CASE WHEN JENIS='TAMBAH' THEN TOTAL_QTY ELSE 0 END) AS q_TAMBAH, " &
            "SUM(CASE WHEN JENIS='KURANG' THEN TOTAL_QTY ELSE 0 END) AS q_KURANG, " &
            "SUM(CASE WHEN JENIS='PEMBELIAN' THEN TOTAL_QTY ELSE 0 END) AS q_PEMBELIAN, " &
            "SUM(CASE WHEN JENIS='PENJUALAN' THEN TOTAL_QTY ELSE 0 END) AS q_PENJUALAN, " &
            "SUM(CASE WHEN JENIS='RETUR BELI' THEN TOTAL_QTY ELSE 0 END) AS q_RETUR_BELI, " &
            "SUM(CASE WHEN JENIS='RETUR JUAL' THEN TOTAL_QTY ELSE 0 END) AS q_RETUR_JUAL, " &
            "SUM(CASE WHEN JENIS='OPNAME' THEN TOTAL_QTY ELSE 0 END) AS q_OPNAME, " &
            "SUM(CASE WHEN JENIS='TRANSFER STOK MASUK' THEN TOTAL_QTY ELSE 0 END) AS q_TSM, " &
            "SUM(CASE WHEN JENIS='TRANSFER STOK KELUAR' THEN TOTAL_QTY ELSE 0 END) AS q_TSK, " &
            "SUM(CASE WHEN JENIS='TRANSFER BARANG MASUK' THEN TOTAL_QTY ELSE 0 END) AS q_TBM, " &
            "SUM(CASE WHEN JENIS='TRANSFER BARANG KELUAR' THEN TOTAL_QTY ELSE 0 END) AS q_TBK, " &
            "SUM(CASE WHEN JENIS='TRANSFER_CABANG_MASUK' THEN TOTAL_QTY ELSE 0 END) AS q_TCM, " &
            "SUM(CASE WHEN JENIS='TRANSFER_CABANG_KELUAR' THEN TOTAL_QTY ELSE 0 END) AS q_TCK " &
            "FROM HistoryBarang WHERE LOKASI='GUDANG' GROUP BY ID_BARANG) h ON h.ID_BARANG = b.ID_BARANG " &
            "SET b.TAMBAH_GUDANG                   = IFNULL(h.q_TAMBAH, 0), " &
            "    b.KURANG_GUDANG                   = IFNULL(h.q_KURANG, 0), " &
            "    b.PEMBELIAN_GUDANG                = IFNULL(h.q_PEMBELIAN, 0), " &
            "    b.PENJUALAN_GUDANG                = IFNULL(h.q_PENJUALAN, 0), " &
            "    b.RETUR_BELI_GUDANG               = IFNULL(h.q_RETUR_BELI, 0), " &
            "    b.RETUR_JUAL_GUDANG               = IFNULL(h.q_RETUR_JUAL, 0), " &
            "    b.OPNAME_GUDANG                   = IFNULL(h.q_OPNAME, 0), " &
            "    b.TRANSFER_STOK_MASUK_GUDANG      = IFNULL(h.q_TSM, 0), " &
            "    b.TRANSFER_STOK_KELUAR_GUDANG     = IFNULL(h.q_TSK, 0), " &
            "    b.TRANSFER_BARANG_MASUK_GUDANG    = IFNULL(h.q_TBM, 0), " &
            "    b.TRANSFER_BARANG_KELUAR_GUDANG   = IFNULL(h.q_TBK, 0), " &
            "    b.TRANSFER_CABANG_MASUK_GUDANG    = IFNULL(h.q_TCM, 0), " &
            "    b.TRANSFER_CABANG_KELUAR_GUDANG   = IFNULL(h.q_TCK, 0)"

        Using cmd As New MySqlCommand(pivotQuery, conn)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

#End Region

#Region "Saldo Karyawan"
    Public Sub UpdateTotalBonDanTotalBayarKaryawan()
        ' Task 8.2b — wrapper ke sp_bat_bon_semua_karyawan (hasil identik, diverifikasi Tests/Test-VB-vs-SP.sql)
        Try
            Using cmd As New MySqlCommand("CALL sp_bat_bon_semua_karyawan()", conn)
                cmd.CommandTimeout = 120
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            If Not TawarMigrasi(TryCast(ex, MySqlException)) Then
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Update SaldoAkhir untuk satu karyawan secara realtime.
    ''' Panggil di dalam transaction yang sama setelah transaksi bon/bayar.
    ''' </summary>
    Public Sub UpdateBonKaryawan(ByVal kodeKaryawan As String, ByVal transaction As MySqlTransaction)
        If String.IsNullOrEmpty(kodeKaryawan) Then Exit Sub
        Using cmd As New MySqlCommand(
            "UPDATE tbl_karyawan k " &
            "LEFT JOIN (SELECT Kode, SUM(NOMINAL) AS TotalBon FROM Bon_karyawan WHERE JENIS = 'BON' AND Kode = @Kode GROUP BY Kode) b ON b.Kode = k.Kode " &
            "LEFT JOIN (SELECT Kode, SUM(NOMINAL) AS TotalBayar FROM Bon_karyawan WHERE JENIS = 'BAYAR' AND Kode = @Kode GROUP BY Kode) p ON p.Kode = k.Kode " &
            "SET k.TotalBon    = IFNULL(b.TotalBon, 0), " &
            "    k.TotalBayar  = IFNULL(p.TotalBayar, 0), " &
            "    k.SaldoAkhir  = k.SaldoAwal + IFNULL(b.TotalBon, 0) - IFNULL(p.TotalBayar, 0) " &
            "WHERE k.Kode = @Kode", conn, transaction)
            cmd.Parameters.AddWithValue("@Kode", kodeKaryawan)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

#End Region

#Region "Piutang Pelanggan"
    ''' <summary>
    ''' Update HutangAkhir untuk satu pelanggan secara realtime.
    ''' Panggil di dalam transaction yang sama setelah transaksi yang mempengaruhi piutang.
    ''' </summary>
    Public Sub UpdatePiutangPelanggan(ByVal idPelanggan As String, ByVal transaction As MySqlTransaction)
        If String.IsNullOrEmpty(idPelanggan) Then Exit Sub
        Using cmd As New MySqlCommand(
            "UPDATE tbl_pelanggan p " &
            "LEFT JOIN (SELECT ID_PELANGGAN, SUM(IFNULL(SISA_TAGIHAN, 0)) AS HUTANG " &
            "           FROM penjualan WHERE ID_PELANGGAN = @ID_PELANGGAN GROUP BY ID_PELANGGAN) x " &
            "ON x.ID_PELANGGAN = p.KODE " &
            "SET p.HutangAkhir = IFNULL(x.HUTANG, 0) + p.HutangAwal " &
            "WHERE p.KODE = @ID_PELANGGAN", conn, transaction)
            cmd.Parameters.AddWithValue("@ID_PELANGGAN", idPelanggan)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' Recalculate HutangAkhir semua pelanggan dari tabel penjualan.
    ''' </summary>
    Public Sub UpdatePiutangDibayar()
        ' Task 8.2b — wrapper ke sp_bat_piutang_semua_pelanggan (hasil identik, diverifikasi Tests/Test-VB-vs-SP.sql)
        Try
            Using cmd As New MySqlCommand("CALL sp_bat_piutang_semua_pelanggan()", conn)
                cmd.CommandTimeout = 120
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            If Not TawarMigrasi(TryCast(ex, MySqlException)) Then
                MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

#End Region

#Region "Hutang Supplier"
    ''' <summary>
    ''' Update HutangAkhir untuk satu supplier secara realtime.
    ''' Panggil di dalam transaction yang sama setelah transaksi yang mempengaruhi hutang.
    ''' </summary>
    Public Sub UpdateHutangSupliyer(ByVal idSupliyer As String, ByVal transaction As MySqlTransaction)
        If String.IsNullOrEmpty(idSupliyer) Then Exit Sub
        Using cmd As New MySqlCommand(
            "UPDATE tbl_supliyer s " &
            "LEFT JOIN (SELECT ID_SUPPLIER, SUM(IFNULL(TAGIHAN, 0)) AS HUTANG " &
            "           FROM pembelian WHERE ID_SUPPLIER = @ID_SUPPLIER GROUP BY ID_SUPPLIER) x " &
            "ON x.ID_SUPPLIER = s.KODE " &
            "SET s.HutangAkhir = IFNULL(x.HUTANG, 0) + s.HutangAwal " &
            "WHERE s.KODE = @ID_SUPPLIER", conn, transaction)
            cmd.Parameters.AddWithValue("@ID_SUPPLIER", idSupliyer)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' Recalculate HutangAkhir semua supplier dari tabel pembelian.
    ''' </summary>
    Public Sub UpdateSupliyerFromPembelianHutangDibayar()
        ' Task 8.2b — wrapper ke sp_bat_hutang_semua_supplier (hasil identik, diverifikasi Tests/Test-VB-vs-SP.sql)
        Try
            Using cmd As New MySqlCommand("CALL sp_bat_hutang_semua_supplier()", conn)
                cmd.CommandTimeout = 120
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            If Not TawarMigrasi(TryCast(ex, MySqlException)) Then
                MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

#End Region

#Region "Saldo Akun Jurnal"
    ''' <summary>
    ''' Recalculate Saldo_Akhir untuk satu akun dari JurnalUmum — dipanggil dalam transaction yang sama.
    ''' Panggil setelah INSERT/DELETE JurnalUmum, sebelum Commit.
    ''' </summary>
    Public Sub UpdateSaldoAkun(ByVal kodeAkun As String, ByVal transaction As MySqlTransaction)
        If String.IsNullOrEmpty(kodeAkun) Then Exit Sub
        Using cmd As New MySqlCommand("CALL sp_hlp_saldo_akun_update(@kode)", conn, transaction)
            cmd.Parameters.AddWithValue("@kode", kodeAkun)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' Update saldo akun secara INCREMENTAL (delta) tanpa scan seluruh JurnalUmum.
    ''' Jauh lebih cepat dari UpdateSaldoAkun — tidak menyentuh JurnalUmum sama sekali.
    ''' Panggil setelah INSERT/DELETE JurnalUmum dengan delta yang sudah dihitung.
    ''' </summary>
    Public Sub UpdateSaldoAkunDelta(ByVal kodeAkun As String,
                                    ByVal deltaDebet As Decimal,
                                    ByVal deltaKredit As Decimal,
                                    ByVal transaction As MySqlTransaction)
        If String.IsNullOrEmpty(kodeAkun) Then Exit Sub
        Using cmd As New MySqlCommand("CALL sp_hlp_saldo_akun_delta(@kode, @d, @k)", conn, transaction)
            cmd.Parameters.AddWithValue("@kode", kodeAkun)
            cmd.Parameters.AddWithValue("@d", deltaDebet)
            cmd.Parameters.AddWithValue("@k", deltaKredit)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' Baca jurnal faktur yang baru di-INSERT, hitung delta per akun,
    ''' lalu update saldo semua akun terlibat dengan sp_hlp_saldo_akun_delta.
    ''' Pengganti loop UpdateSaldoAkun — tidak scan seluruh JurnalUmum.
    ''' </summary>
    Public Sub UpdateSaldoAkunDeltaDariFaktur(ByVal noFaktur As String,
                                               ByVal transaction As MySqlTransaction)
        If String.IsNullOrEmpty(noFaktur) Then Exit Sub

        ' Baca delta dari jurnal faktur ini saja (N baris kecil, bukan 600k+)
        Dim deltaDebet As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
        Dim deltaKredit As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

        Using cmd As New MySqlCommand(
            "SELECT NOMOR_AKUN_D, NOMOR_AKUN_K, NOMINAL FROM JurnalUmum WHERE NO_TRANSAKSI = @no",
            conn, transaction)
            cmd.Parameters.AddWithValue("@no", noFaktur)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    Dim nominal As Decimal = If(IsDBNull(rd("NOMINAL")), 0D, Convert.ToDecimal(rd("NOMINAL")))
                    Dim kD As String = rd("NOMOR_AKUN_D").ToString().Trim()
                    Dim kK As String = rd("NOMOR_AKUN_K").ToString().Trim()
                    If kD <> "" Then
                        If deltaDebet.ContainsKey(kD) Then deltaDebet(kD) += nominal Else deltaDebet(kD) = nominal
                        If Not deltaKredit.ContainsKey(kD) Then deltaKredit(kD) = 0D
                    End If
                    If kK <> "" Then
                        If deltaKredit.ContainsKey(kK) Then deltaKredit(kK) += nominal Else deltaKredit(kK) = nominal
                        If Not deltaDebet.ContainsKey(kK) Then deltaDebet(kK) = 0D
                    End If
                End While
            End Using
        End Using

        ' Update saldo per akun dengan delta — tidak scan JurnalUmum
        Dim semuaAkun As New HashSet(Of String)(deltaDebet.Keys, StringComparer.OrdinalIgnoreCase)
        For Each k In deltaKredit.Keys
            semuaAkun.Add(k)
        Next

        For Each kode As String In semuaAkun
            Dim d As Decimal = If(deltaDebet.ContainsKey(kode), deltaDebet(kode), 0D)
            Dim k As Decimal = If(deltaKredit.ContainsKey(kode), deltaKredit(kode), 0D)
            UpdateSaldoAkunDelta(kode, d, k, transaction)
        Next
    End Sub

    ''' <summary>
    ''' Reversal saldo akun dari jurnal faktur yang akan dihapus.
    ''' Baca jurnal faktur lama, hitung delta negatif, update saldo.
    ''' Dipanggil SEBELUM DELETE JurnalUmum.
    ''' </summary>
    Public Sub ReversalSaldoAkunDariFaktur(ByVal noFaktur As String,
                                            ByVal transaction As MySqlTransaction)
        If String.IsNullOrEmpty(noFaktur) Then Exit Sub

        Dim deltaDebet As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
        Dim deltaKredit As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

        Using cmd As New MySqlCommand(
            "SELECT NOMOR_AKUN_D, NOMOR_AKUN_K, NOMINAL FROM JurnalUmum WHERE NO_TRANSAKSI = @no",
            conn, transaction)
            cmd.Parameters.AddWithValue("@no", noFaktur)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    Dim nominal As Decimal = If(IsDBNull(rd("NOMINAL")), 0D, Convert.ToDecimal(rd("NOMINAL")))
                    Dim kD As String = rd("NOMOR_AKUN_D").ToString().Trim()
                    Dim kK As String = rd("NOMOR_AKUN_K").ToString().Trim()
                    If kD <> "" Then
                        If deltaDebet.ContainsKey(kD) Then deltaDebet(kD) += nominal Else deltaDebet(kD) = nominal
                        If Not deltaKredit.ContainsKey(kD) Then deltaKredit(kD) = 0D
                    End If
                    If kK <> "" Then
                        If deltaKredit.ContainsKey(kK) Then deltaKredit(kK) += nominal Else deltaKredit(kK) = nominal
                        If Not deltaDebet.ContainsKey(kK) Then deltaDebet(kK) = 0D
                    End If
                End While
            End Using
        End Using

        ' Delta negatif = reversal
        Dim semuaAkun As New HashSet(Of String)(deltaDebet.Keys, StringComparer.OrdinalIgnoreCase)
        For Each k In deltaKredit.Keys
            semuaAkun.Add(k)
        Next

        For Each kode As String In semuaAkun
            Dim d As Decimal = If(deltaDebet.ContainsKey(kode), deltaDebet(kode), 0D)
            Dim k As Decimal = If(deltaKredit.ContainsKey(kode), deltaKredit(kode), 0D)
            UpdateSaldoAkunDelta(kode, -d, -k, transaction)
        Next
    End Sub

#End Region

#Region "Jurnal Audit"
    ''' <summary>
    ''' Audit jurnal setelah commit — query per baris dari DB, gabungkan dengan label hardcoded dari caller.
    ''' Format output: J1=Tunai[D:KAS TUNAI=500.000 K:-] | J2=HPP[D:- K:PERSEDIAAN=800.000]
    ''' labelBaris: array label hardcoded sesuai urutan baris jurnal, misal {"Tunai","Transfer","Piutang","HPP","LabaKotor"}
    ''' Dipanggil setelah transaction.Commit().
    ''' </summary>
    Public Sub CatatJurnalTidakSeimbang(ByVal noTransaksi As String, ByVal expectedDebet As Decimal, ByVal expectedKredit As Decimal, ByVal jenisTransaksi As String, Optional ByVal labelBaris As String() = Nothing)
        Try
            Dim actualDebet As Decimal = 0D
            Dim actualKredit As Decimal = 0D
            Dim barisList As New List(Of String)()
            Dim idx As Integer = 0

            Dim sqlJurnal As String =
                "SELECT NOMOR_AKUN_D, NAMA_AKUN_D, NOMOR_AKUN_K, NAMA_AKUN_K, NOMINAL " &
                "FROM JurnalUmum WHERE NO_TRANSAKSI = @NoTransaksi"
            Using cmd As New MySqlCommand(sqlJurnal, conn)
                cmd.Parameters.AddWithValue("@NoTransaksi", noTransaksi)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        idx += 1
                        Dim nominal As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "NOMINAL", 0D)
                        Dim akunD As String = ModuleAngka.SafeGetValue(Of String)(rd, "NOMOR_AKUN_D", "").Trim()
                        Dim akunK As String = ModuleAngka.SafeGetValue(Of String)(rd, "NOMOR_AKUN_K", "").Trim()
                        Dim namaD As String = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_AKUN_D", "-")
                        Dim namaK As String = ModuleAngka.SafeGetValue(Of String)(rd, "NAMA_AKUN_K", "-")

                        If akunD <> "" Then actualDebet += nominal
                        If akunK <> "" Then actualKredit += nominal

                        ' Label hardcoded dari caller, fallback ke J{n} jika tidak ada
                        Dim label As String = If(labelBaris IsNot Nothing AndAlso idx <= labelBaris.Length, labelBaris(idx - 1), $"J{idx}")
                        Dim sisiD As String = If(akunD <> "", namaD & "=" & nominal.ToString("N0"), "-")
                        Dim sisiK As String = If(akunK <> "", namaK & "=" & nominal.ToString("N0"), "-")
                        barisList.Add($"{label}[D:{sisiD} K:{sisiK}]")
                    End While
                End Using
            End Using

            Dim jumlahBaris As Integer = idx
            Dim seimbangActual As Boolean = (actualDebet = actualKredit)
            Dim sesuaiExpected As Boolean = (actualDebet = expectedDebet AndAlso actualKredit = expectedKredit)

            Dim status As String
            If seimbangActual AndAlso sesuaiExpected Then
                status = "[JURNAL ✅ SEIMBANG]"
            ElseIf seimbangActual AndAlso Not sesuaiExpected Then
                status = "[JURNAL ⚠️ SEIMBANG BEDA EXPECTED]"
            Else
                status = "[JURNAL ❌ TIDAK SEIMBANG]"
            End If

            Dim ringkasan As String = $"Baris={jumlahBaris} D={actualDebet:N0} K={actualKredit:N0}"
            If Not sesuaiExpected Then
                ringkasan &= $" ExpD={expectedDebet:N0} ExpK={expectedKredit:N0}"
            End If

            Dim pesan As String = $"{status} {noTransaksi} | {jenisTransaksi} | {ringkasan} || {String.Join(" | ", barisList)}"

            ' Hanya simpan ke History jika jurnal TIDAK seimbang
            If Not seimbangActual Then
                Dim queryInsert As String = "INSERT INTO History (Tanggal, Aksi) VALUES (@Tanggal, @Aksi)"
                Using cmd As New MySqlCommand(queryInsert, conn)
                    cmd.Parameters.AddWithValue("@Tanggal", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@Aksi", FormUtama.StatusLokasi.Text & " = [" & FormUtama.StatusNamaPC.Text & " - " & FormUtama.StatusNamaUser.Text & "] " & pesan)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        Catch
            ' Jangan crash aplikasi hanya karena audit log gagal
        End Try
    End Sub

#End Region

#Region "Stok Audit"
    ''' <summary>
    ''' Baca STOK_TOKO atau STOK_GUDANG saat ini dari DB dalam transaction yang sama.
    ''' Dipakai untuk snapshot sebelum HitungStokPerubahan.
    ''' </summary>
    Public Function BacaStokSaatIni(ByVal kodeBarang As String, ByVal lokasi As String, ByVal transaction As MySqlTransaction) As Decimal
        Dim kolom As String = If(lokasi = "GUDANG", "STOK_GUDANG", "STOK_TOKO")
        Using cmd As New MySqlCommand($"SELECT {kolom} FROM tbl_barang WHERE ID_BARANG = @Kode", conn, transaction)
            cmd.Parameters.AddWithValue("@Kode", kodeBarang)
            Dim val = cmd.ExecuteScalar()
            Return If(val Is Nothing OrElse IsDBNull(val), 0D, Convert.ToDecimal(val))
        End Using
    End Function

    ''' <summary>
    ''' Bandingkan 4 sumber data stok per barang dan catat ke History — selalu, apapun hasilnya.
    ''' Status SEIMBANG dicatat untuk konfirmasi aplikasi berjalan benar.
    ''' Status TIDAK SEIMBANG dicatat untuk deteksi bug.
    ''' 
    ''' auditDGV      : qty dari DGV (sumber user)
    ''' auditHistory  : qty yang diinsert ke HistoryBarang
    ''' auditDetail   : qty yang diinsert ke tabel detail transaksi (penjualan_detail, dll)
    ''' auditStokDelta: selisih stok master sebelum vs sesudah HitungStokPerubahan (nilai absolut)
    ''' 
    ''' Tidak semua parameter wajib diisi — kosongkan Dictionary yang tidak relevan.
    ''' </summary>
    Public Sub AuditStokTransaksi(
            ByVal noTransaksi As String,
            ByVal jenisTransaksi As String,
            ByVal auditDGV As Dictionary(Of String, Decimal),
            ByVal auditHistory As Dictionary(Of String, Decimal),
            ByVal auditDetail As Dictionary(Of String, Decimal),
            ByVal auditStokDelta As Dictionary(Of String, Decimal),
            ByVal transaction As MySqlTransaction)

        ' Kumpulkan semua kode barang dari semua sumber
        Dim semuaKode As New HashSet(Of String)()
        For Each d In {auditDGV, auditHistory, auditDetail, auditStokDelta}
            If d IsNot Nothing Then
                For Each k In d.Keys
                    semuaKode.Add(k)
                Next
            End If
        Next

        If semuaKode.Count = 0 Then Exit Sub

        Dim temuanTidakSeimbang As New List(Of String)()
        Dim temuanSeimbang As New List(Of String)()

        For Each kode In semuaKode
            Dim a As Decimal = If(auditDGV IsNot Nothing AndAlso auditDGV.ContainsKey(kode), auditDGV(kode), -1)
            Dim b As Decimal = If(auditHistory IsNot Nothing AndAlso auditHistory.ContainsKey(kode), auditHistory(kode), -1)
            Dim c As Decimal = If(auditDetail IsNot Nothing AndAlso auditDetail.ContainsKey(kode), auditDetail(kode), -1)
            Dim d As Decimal = If(auditStokDelta IsNot Nothing AndAlso auditStokDelta.ContainsKey(kode), auditStokDelta(kode), -1)

            Dim nilaiTersedia As New List(Of Decimal)()
            If a >= 0 Then nilaiTersedia.Add(a)
            If b >= 0 Then nilaiTersedia.Add(b)
            If c >= 0 Then nilaiTersedia.Add(c)
            If d >= 0 Then nilaiTersedia.Add(d)

            Dim bagian As New List(Of String)()
            If a >= 0 Then bagian.Add($"DGV={a:N0}")
            If b >= 0 Then bagian.Add($"History={b:N0}")
            If c >= 0 Then bagian.Add($"Detail={c:N0}")
            If d >= 0 Then bagian.Add($"StokDelta={d:N0}")
            Dim detail As String = $"{kode}: {String.Join(" ", bagian)}"

            If nilaiTersedia.Count > 1 AndAlso nilaiTersedia.Distinct().Count() > 1 Then
                temuanTidakSeimbang.Add(detail)
            Else
                temuanSeimbang.Add(detail)
            End If
        Next

        Try
            Dim status As String = If(temuanTidakSeimbang.Count > 0, "[AUDIT STOK ❌ TIDAK SEIMBANG]", "[AUDIT STOK ✅ SEIMBANG]")
            Dim semuaDetail As New List(Of String)()
            semuaDetail.AddRange(temuanTidakSeimbang)
            semuaDetail.AddRange(temuanSeimbang)
            Dim pesan As String = $"{status} {noTransaksi} | {jenisTransaksi} | {String.Join(" || ", semuaDetail)}"

            Dim query As String = "INSERT INTO History (Tanggal, Aksi) VALUES (@Tanggal, @Aksi)"
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@Tanggal", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@Aksi", FormUtama.StatusLokasi.Text & " = [" & FormUtama.StatusNamaPC.Text & " - " & FormUtama.StatusNamaUser.Text & "] " & pesan)
                cmd.ExecuteNonQuery()
            End Using
        Catch
            ' Jangan crash aplikasi hanya karena audit log gagal
        End Try
    End Sub

#End Region

#Region "Combobox Helper"

    ''' <summary>
    ''' Isi CmbBln dan CmbThn dengan daftar bulan dan tahun.
    ''' Default: bulan dan tahun sekarang.
    ''' Panggil dari form manapun yang butuh filter per bulan.
    ''' </summary>
    Public Sub MuatComboBoxBulanTahun(cmbBln As ComboBox, cmbThn As ComboBox)
        cmbThn.Items.Clear()
        For i As Integer = 2022 To Year(Now)
            cmbThn.Items.Add(i)
        Next
        cmbThn.SelectedItem = Year(Now)

        cmbBln.Items.Clear()
        cmbBln.Items.AddRange({"Januari", "Februari", "Maret", "April", "Mei",
                                "Juni", "Juli", "Agustus", "September",
                                "Oktober", "November", "Desember"})
        cmbBln.SelectedIndex = Month(Now) - 1
    End Sub

    ''' <summary>
    ''' Ambil rentang DateTime awal dan akhir dari CmbBln + CmbThn.
    ''' Return False jika bulan/tahun belum dipilih.
    ''' </summary>
    Public Function GetRentangBulan(cmbBln As ComboBox, cmbThn As ComboBox,
                                    ByRef tglAwal As DateTime, ByRef tglAkhir As DateTime) As Boolean
        If cmbBln.SelectedIndex = -1 OrElse cmbThn.SelectedIndex = -1 Then
            MessageBox.Show("Harap pilih bulan dan tahun terlebih dahulu.", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbBln.DroppedDown = True
            Return False
        End If
        Dim bulan As Integer = cmbBln.SelectedIndex + 1
        Dim tahun As Integer = CInt(cmbThn.SelectedItem)
        tglAwal = New DateTime(tahun, bulan, 1)
        tglAkhir = tglAwal.AddMonths(1).AddTicks(-1)
        Return True
    End Function

    ''' <summary>
    ''' Konversi kolom String di DataTable menjadi DateTime agar ReportViewer
    ''' tidak menampilkan #Error saat field diformat sebagai tanggal di RDLC.
    ''' </summary>
    Public Function ConvertColumnToDateTime(dt As DataTable, columnName As String) As DataTable
        If dt Is Nothing OrElse Not dt.Columns.Contains(columnName) Then Return dt

        Dim dtCloned As DataTable = dt.Clone()
        dtCloned.Columns(columnName).DataType = GetType(Date)

        For Each row As DataRow In dt.Rows
            Dim newRow As DataRow = dtCloned.NewRow()
            For Each col As DataColumn In dt.Columns
                If col.ColumnName = columnName Then
                    If row(col.ColumnName) Is DBNull.Value Then
                        newRow(col.ColumnName) = DBNull.Value
                    Else
                        Try
                            newRow(col.ColumnName) = Convert.ToDateTime(row(col.ColumnName))
                        Catch
                            newRow(col.ColumnName) = DBNull.Value
                        End Try
                    End If
                Else
                    newRow(col.ColumnName) = row(col.ColumnName)
                End If
            Next
            dtCloned.Rows.Add(newRow)
        Next

        Return dtCloned
    End Function

#End Region

End Module

