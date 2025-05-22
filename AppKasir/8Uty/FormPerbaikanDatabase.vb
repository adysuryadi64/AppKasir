Public Class FormPerbaikanDatabase
    Private Sub FormPerbaikanDatabase_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListBoxResults.Items.Clear()
    End Sub

    Private Sub BtnCleanup_Click(sender As Object, e As EventArgs) Handles BtnCleanup.Click
        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()

        Dim transaction As MySqlTransaction = Nothing

        Try
            transaction = conn.BeginTransaction()

            Dim operations As New List(Of String) From {
                    "UPDATE tbl_Armada SET KODE = TRIM(KODE), NOPOL = TRIM(NOPOL)",
                    "UPDATE tbl_barang SET NAMA_BARANG = TRIM(NAMA_BARANG), ID_BARANG = TRIM(ID_BARANG), BARCODE_KECIL = TRIM(BARCODE_KECIL), BARCODE_SEDANG = TRIM(BARCODE_SEDANG), BARCODE_BESAR = TRIM(BARCODE_BESAR)",
                    "UPDATE tbl_datareferensi SET Kode_akun = TRIM(Kode_akun), Nama_Akun = TRIM(Nama_Akun)",
                    "UPDATE tbl_karyawan SET Kode = TRIM(Kode), Nama = TRIM(Nama)",
                    "UPDATE tbl_kategori SET kode = TRIM(kode), nama = TRIM(nama)",
                    "UPDATE tbl_pelanggan SET KODE = TRIM(KODE), NAMA = TRIM(NAMA)",
                    "UPDATE tbl_satuan SET kode = TRIM(kode), nama = TRIM(nama)",
                    "UPDATE tbl_supliyer SET Nama = TRIM(Nama), ALamat = TRIM(ALamat)",
                    "UPDATE tbl_user SET nama_user = TRIM(nama_user), user_name = TRIM(user_name)"
                }

            ' Jalankan operasi perbaruan
            For Each query As String In operations
                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.ExecuteNonQuery()
                    ListBoxResults.Items.Add($"Query berhasil: {query}")
                End Using
            Next

            ' Commit jika semua berhasil
            transaction.Commit()
            ListBoxResults.Items.Add("Semua operasi selesai berhasil!")

        Catch ex As Exception
            transaction?.Rollback()
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")

        Finally
            Cursor = Cursors.Default
        End Try
    End Sub


    Private Sub BtnDuplikat_Click(sender As Object, e As EventArgs) Handles BtnDuplikat.Click
        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()

        Try
            Dim duplikatList As List(Of String) = CekDuplikatBarang()

            If duplikatList.Count > 0 Then
                For Each item In duplikatList
                    ListBoxResults.Items.Add(item)
                Next
            Else
                ListBoxResults.Items.Add("Tidak ditemukan data duplikat pada ID_BARANG atau NAMA_BARANG.")
            End If

        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")

        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Function CekDuplikatBarang() As List(Of String)
        Dim duplikatList As New List(Of String)

        ' Cek duplikat ID_BARANG
        Dim queryDuplikatID As String = "SELECT ID_BARANG, COUNT(*) AS JumlahDuplikat FROM tbl_barang GROUP BY ID_BARANG HAVING COUNT(*) > 1"
        Using cmdID As New MySqlCommand(queryDuplikatID, conn)
            Using reader As MySqlDataReader = cmdID.ExecuteReader()
                While reader.Read()
                    Dim idBarang As String = reader("ID_BARANG").ToString()
                    Dim jumlah As Integer = Convert.ToInt32(reader("JumlahDuplikat"))
                    duplikatList.Add($"Duplikat ID_BARANG: {idBarang}, Jumlah: {jumlah}")
                End While
            End Using
        End Using

        ' Cek duplikat NAMA_BARANG
        Dim queryDuplikatNama As String = "SELECT NAMA_BARANG, COUNT(*) AS JumlahDuplikat FROM tbl_barang GROUP BY NAMA_BARANG HAVING COUNT(*) > 1"
        Using cmdNama As New MySqlCommand(queryDuplikatNama, conn)
            Using reader As MySqlDataReader = cmdNama.ExecuteReader()
                While reader.Read()
                    Dim namaBarang As String = reader("NAMA_BARANG").ToString()
                    Dim jumlah As Integer = Convert.ToInt32(reader("JumlahDuplikat"))
                    duplikatList.Add($"Duplikat NAMA_BARANG: {namaBarang}, Jumlah: {jumlah}")
                End While
            End Using
        End Using

        Return duplikatList
    End Function



    Private Sub BtnAnalyze_Click(sender As Object, e As EventArgs) Handles BtnAnalyze.Click
        Cursor = Cursors.WaitCursor
        ' Daftar tabel yang akan dianalisis
        Dim tables As String() = {
            "bon_karyawan", "gaji_karyawan", "hakaksesuser", "history", "historybarang", "hutang", "hutang_detail", "jurnalumum",
            "pembelian", "pembelian_detail", "pembelian_ditahan", "pembelian_ditahan_detail", "penjualan", "penjualan_detail",
            "penjualan_ditahan", "penjualan_ditahan_detail", "piutang", "piutang_detail", "retur_pembelian", "retur_pembelian_detail",
            "retur_penjualan", "retur_penjualan_detail", "stoktambahkurang", "stok_opname", "surat_jalan", "surat_jalan_detail",
            "tbl_armada", "tbl_barang", "tbl_datareferensi", "tbl_gaji", "tbl_karyawan", "tbl_kategori", "tbl_merk", "tbl_pelanggan",
            "tbl_perusahaan", "tbl_satuan", "tbl_supliyer", "tbl_user", "tempbukubesarpembantu", "tempjurnalumum", "temp_bon_karyawan",
            "temp_datareferensi", "temp_jurnal", "temp_labarugi", "temp_loading", "temp_mutasi_barang", "temp_supliyerbayar",
            "temp_supliyerhutang", "transfer_barang", "transfer_barang_detail", "transfer_stok", "tukarbarang"
        }

        Try
            ListBoxResults.Items.Clear()

            For Each tableName As String In tables
                Dim query As String = $"ANALYZE TABLE `{tableName}`;"
                Using command As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            Dim table As String = reader("Table").ToString()
                            Dim operation As String = reader("Op").ToString()
                            Dim messageType As String = reader("Msg_type").ToString()
                            Dim messageText As String = reader("Msg_text").ToString()

                            ' Format hasil untuk ditampilkan di ListBox
                            Dim result As String = $"{table}: {operation} - {messageType} - {messageText}"
                            ListBoxResults.Items.Add(result)
                        End While
                    End Using
                End Using
            Next

        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub BtnCheckTables_Click(sender As Object, e As EventArgs) Handles BtnCheckTables.Click
        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()

        Try

            Dim tables As String() = {
                    "bon_karyawan", "gaji_karyawan", "hakaksesuser", "history", "historybarang", "hutang", "hutang_detail",
                    "jurnalumum", "pembelian", "pembelian_detail", "pembelian_ditahan", "pembelian_ditahan_detail",
                    "penjualan", "penjualan_detail", "penjualan_ditahan", "penjualan_ditahan_detail", "piutang",
                    "piutang_detail", "retur_pembelian", "retur_pembelian_detail", "retur_penjualan", "retur_penjualan_detail",
                    "stoktambahkurang", "stok_opname", "surat_jalan", "surat_jalan_detail", "tbl_armada", "tbl_barang",
                    "tbl_datareferensi", "tbl_gaji", "tbl_karyawan", "tbl_kategori", "tbl_merk", "tbl_pelanggan",
                    "tbl_perusahaan", "tbl_satuan", "tbl_supliyer", "tbl_user", "tempbukubesarpembantu", "tempjurnalumum",
                    "temp_bon_karyawan", "temp_datareferensi", "temp_jurnal", "temp_labarugi", "temp_loading",
                    "temp_mutasi_barang", "temp_supliyerbayar", "temp_supliyerhutang", "transfer_barang", "transfer_barang_detail",
                    "transfer_stok", "tukarbarang"
                }

            For Each table As String In tables
                Dim query As String = $"CHECK TABLE `{table}`"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim tableName As String = reader("Table").ToString()
                            Dim operation As String = reader("Op").ToString()
                            Dim messageType As String = reader("Msg_type").ToString()
                            Dim messageText As String = reader("Msg_text").ToString()

                            Dim result As String = $"{tableName} | {operation} | {messageType} | {messageText}"
                            ListBoxResults.Items.Add(result)
                        End While
                    End Using
                End Using
            Next


        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub BtnChecksumTables_Click(sender As Object, e As EventArgs) Handles BtnChecksumTables.Click
        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()

        Try

            Dim tables As String() = {
                    "bon_karyawan", "gaji_karyawan", "hakaksesuser", "history", "historybarang", "hutang", "hutang_detail",
                    "jurnalumum", "pembelian", "pembelian_detail", "pembelian_ditahan", "pembelian_ditahan_detail",
                    "penjualan", "penjualan_detail", "penjualan_ditahan", "penjualan_ditahan_detail", "piutang",
                    "piutang_detail", "retur_pembelian", "retur_pembelian_detail", "retur_penjualan", "retur_penjualan_detail",
                    "stoktambahkurang", "stok_opname", "surat_jalan", "surat_jalan_detail", "tbl_armada", "tbl_barang",
                    "tbl_datareferensi", "tbl_gaji", "tbl_karyawan", "tbl_kategori", "tbl_merk", "tbl_pelanggan",
                    "tbl_perusahaan", "tbl_satuan", "tbl_supliyer", "tbl_user", "tempbukubesarpembantu", "tempjurnalumum",
                    "temp_bon_karyawan", "temp_datareferensi", "temp_jurnal", "temp_labarugi", "temp_loading",
                    "temp_mutasi_barang", "temp_supliyerbayar", "temp_supliyerhutang", "transfer_barang", "transfer_barang_detail",
                    "transfer_stok", "tukarbarang"
                }

            For Each table As String In tables
                Dim query As String = $"CHECKSUM TABLE `{table}`"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim tableName As String = reader("Table").ToString()
                            Dim checksum As String = reader("Checksum").ToString()

                            Dim result As String = $"{tableName} | Checksum: {checksum}"
                            ListBoxResults.Items.Add(result)
                        End While
                    End Using
                End Using
            Next


        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub


End Class