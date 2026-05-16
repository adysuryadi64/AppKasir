Module ModuleHapusTransaksi

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: HAPUS PEMBELIAN
    ' ═══════════════════════════════════════════════════════════════════
    ' CATATAN MIGRASI:
    ' Fungsi ini menggabungkan logika dari dua sumber:
    '   [FU] = FormUtama.Hapusbelanja()          — hapus permanen dari daftar
    '   [FP] = FormPembelian.Hapusbelanja(tr)    — hapus lama saat proses edit
    '
    ' Perbedaan yang ditemukan (dicatat, belum diubah di sumber asli):
    '   1. Cek hutang BAYAR  : [FU] tidak ada  | [FP] ada → diambil dari [FP]
    '   2. UpdateSaldoAkun   : [FU] di sini    | [FP] di caller → diambil dari [FU]
    '   3. Urutan stok vs DELETE : [FU] stok SETELAH delete | [FP] stok SEBELUM delete
    '      → diambil dari [FP] (sebelum delete) karena HitungStokPerubahan butuh data detail
    '   4. Loop DGVDetail    : [FU] dua kali   | [FP] satu kali → disatukan
    '   5. HPP recalculate   : [FU] & [FP] keduanya hanya kembalikan ke snapshot lama
    '      → DIPERBAIKI di sini dengan RecalculateHppSetelahHapus (weighted average)
    '   6. HARGA_BELI_TERAKHIR : [FU] diupdate ke HARGA_BELI_SEBELUMNYA | [FP] sama
    '      → DIPERTAHANKAN — masih diupdate seperti kode lama
    ' ═══════════════════════════════════════════════════════════════════
#Region "HAPUS PEMBELIAN"

    ''' <summary>
    ''' Hapus satu faktur pembelian secara permanen dalam transaction yang diberikan.
    ''' Menggabungkan logika dari FormUtama.Hapusbelanja() dan FormPembelian.Hapusbelanja(tr).
    ''' Mengembalikan HPP via RecalculateHppSetelahHapus (weighted average semua faktur tersisa).
    ''' Commit/Rollback tetap tanggung jawab caller.
    ''' Dipanggil dari: FormUtama.Hapusbelanja() dan FormPembelian.Hapusbelanja(transaction).
    ''' </summary>
    ''' <param name="faktur">Nomor faktur yang akan dihapus (ID_PEMBELIAN)</param>
    ''' <param name="lokasi">Lokasi barang: "TOKO" atau "GUDANG"</param>
    ''' <param name="transaction">Transaction aktif dari caller</param>
    Public Sub HapusPembelian(ByVal faktur As String,
                               ByVal lokasi As String,
                               ByVal transaction As MySqlTransaction)

        Dim updateStokField As String = If(lokasi = "GUDANG", "PEMBELIAN_GUDANG", "PEMBELIAN_TOKO")

        ' ── Step 1: Kumpulkan akun terlibat SEBELUM delete JurnalUmum ────────────
        ' [FU] melakukan ini, [FP] tidak (UpdateSaldoAkun ada di caller FP)
        ' → diambil dari [FU]: UpdateSaldoAkun menjadi tanggung jawab fungsi ini
        Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Using cmdAkun As New MySqlCommand(
            "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
            "UNION " &
            "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
            conn, transaction)
            cmdAkun.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdAkun.ExecuteReader()
                While rd.Read()
                    Dim kode As String = rd(0).ToString().Trim()
                    If kode <> "" Then akunTerlibat.Add(kode)
                End While
            End Using
        End Using

        ' ── Step 2: Baca detail faktur yang akan dihapus dari DB ─────────────────
        ' [FU] & [FP] membaca dari DGVDetail (data sudah ada di grid)
        ' → di sini baca langsung dari DB agar tidak bergantung pada state UI
        Dim detailFaktur As New List(Of (IdBarang As String, QtySat As Decimal, HargaAverage As Decimal, HargaBeliSebelumnya As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_BARANG, QTY_SAT, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA " &
            "FROM pembelian_detail WHERE FAKTUR_BELI = @fk",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    detailFaktur.Add((
                        rd("ID_BARANG").ToString(),
                        If(IsDBNull(rd("QTY_SAT")), 0D, Convert.ToDecimal(rd("QTY_SAT"))),
                        If(IsDBNull(rd("HARGA_AVERAGE")), 0D, Convert.ToDecimal(rd("HARGA_AVERAGE"))),
                        If(IsDBNull(rd("HARGA_BELI_SEBELUMNYA")), 0D, Convert.ToDecimal(rd("HARGA_BELI_SEBELUMNYA")))
                    ))
                End While
            End Using
        End Using

        ' ── Step 3: Cek hutang_detail JENIS='BAYAR' — konfirmasi jika ada ────────
        ' [FU] tidak ada pengecekan ini | [FP] ada → diambil dari [FP]
        Using cmdCekBayar As New MySqlCommand(
            "SELECT COUNT(*) FROM hutang_detail WHERE ID_BELI = @fk AND JENIS = 'BAYAR'",
            conn, transaction)
            cmdCekBayar.Parameters.AddWithValue("@fk", faktur)
            Dim sudahDibayar As Integer = Convert.ToInt32(cmdCekBayar.ExecuteScalar())
            If sudahDibayar > 0 Then
                If MessageBox.Show("Faktur ini sudah memiliki pembayaran. " &
                   "Menghapus akan mempengaruhi histori hutang. Lanjutkan?",
                   "Peringatan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                    Throw New OperationCanceledException("Dibatalkan oleh pengguna.")
                End If
            End If
        End Using

        ' ── Step 4–6: Per barang — kurangi counter, recalculate HPP, hitung stok ─
        ' [FU] urutan: update HPP → (delete) → HitungStokPerubahan
        ' [FP] urutan: update HPP → HitungStokPerubahan → (delete)
        ' → diambil dari [FP]: HitungStokPerubahan SEBELUM delete
        '   karena sp_hlp_stok_hitung membaca PEMBELIAN_x yang sudah dikurangi di Step 4
        Dim auditDGV As New Dictionary(Of String, Decimal)()
        Dim auditDelta As New Dictionary(Of String, Decimal)()

        ' Untuk jurnal penyesuaian moving average — kumpulkan total selisih nilai persediaan
        ' Selisih = (HPP_baru - HPP_lama) × Stok_tersisa setelah hapus
        Dim totalSelisihPersediaan As Decimal = 0D

        For Each item In detailFaktur
            ' Step 4a: Kurangi stok counter PEMBELIAN_TOKO/GUDANG
            Using cmd As New MySqlCommand(
                $"UPDATE tbl_barang SET {updateStokField} = {updateStokField} - @qty " &
                "WHERE ID_BARANG = @kode",
                conn, transaction)
                cmd.Parameters.AddWithValue("@qty", item.QtySat)
                cmd.Parameters.AddWithValue("@kode", item.IdBarang)
                cmd.ExecuteNonQuery()
            End Using

            ' Step 4b: Simpan HPP lama sebelum recalculate — untuk hitung selisih jurnal
            Dim hppLama As Decimal = BacaHppSaatIni(item.IdBarang, transaction)

            ' Step 4c: Recalculate HPP dari semua faktur tersisa (perbaikan utama)
            ' [FU] & [FP]: SET HARGA_BELI = HARGA_AVERAGE (snapshot lama — hanya benar jika faktur terakhir)
            ' → DIPERBAIKI: weighted average dari semua faktur tersisa
            RecalculateHppSetelahHapus(item.IdBarang, faktur, lokasi, transaction)

            ' Step 4d: Update HARGA_BELI_TERAKHIR dari faktur tersisa yang paling baru
            ' BUKAN dari HARGA_BELI_SEBELUMNYA faktur yang dihapus (itu rollback untuk edit, bukan hapus)
            Dim hargaBeliTerakhirBaru As Decimal = QueryHargaBeliTerakhirDariFakturTersisa(
                item.IdBarang, faktur, lokasi, transaction)
            Using cmdHBT As New MySqlCommand(
                "UPDATE tbl_barang SET HARGA_BELI_TERAKHIR = @hbt WHERE ID_BARANG = @kode",
                conn, transaction)
                cmdHBT.Parameters.AddWithValue("@hbt", hargaBeliTerakhirBaru)
                cmdHBT.Parameters.AddWithValue("@kode", item.IdBarang)
                cmdHBT.ExecuteNonQuery()
            End Using

            ' Step 4e: Recalculate stok fisik SEBELUM delete (ikut [FP])
            Dim sebelum As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)
            HitungStokPerubahan(item.IdBarang, transaction)
            Dim sesudah As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)

            ' Kumpulkan untuk audit
            If auditDGV.ContainsKey(item.IdBarang) Then
                auditDGV(item.IdBarang) += item.QtySat
            Else
                auditDGV(item.IdBarang) = item.QtySat
            End If
            Dim delta As Decimal = sebelum - sesudah
            If auditDelta.ContainsKey(item.IdBarang) Then
                auditDelta(item.IdBarang) += delta
            Else
                auditDelta(item.IdBarang) = delta
            End If
        Next

        ' ── Step 5: Hapus hutang_detail JENIS='BELI' ─────────────────────────────
        Using cmdHapusHutang As New MySqlCommand(
            "DELETE FROM hutang_detail WHERE ID_BELI = @fk AND JENIS = 'BELI'",
            conn, transaction)
            cmdHapusHutang.Parameters.AddWithValue("@fk", faktur)
            cmdHapusHutang.ExecuteNonQuery()
        End Using

        ' ── Step 6: Delete semua data faktur ─────────────────────────────────────
        For Each query As String In {
            "DELETE FROM pembelian WHERE ID_PEMBELIAN = @fk",
            "DELETE FROM pembelian_detail WHERE FAKTUR_BELI = @fk",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @fk"
        }
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' ── Step 7: Audit stok transaksi ─────────────────────────────────────────
        AuditStokTransaksi(faktur, "Hapus Pembelian", auditDGV, Nothing, Nothing, auditDelta, transaction)

        ' ── Step 7b: Jurnal penyesuaian moving average ───────────────────────────
        ' DINONAKTIFKAN SEMENTARA — untuk test apakah jurnal penyesuaian benar-benar diperlukan
        ' Setelah hapus PB-005, nilai persediaan = 905M, saldo jurnal = 905M (sudah seimbang)
        ' Tapi kode lama membuat jurnal penyesuaian 1.623.649.961 yang tidak diperlukan
        ' Root cause: Logika perhitungan selisih menghitung SETELAH jurnal pembelian dihapus,
        ' tapi tidak memperhitungkan bahwa saldo jurnal pembelian sudah pas dengan nilai persediaan
        '
        ' Kemungkinan jurnal penyesuaian hanya diperlukan jika ada transaksi lain
        ' (penjualan, retur, opname) yang mengubah stok/HPP setelah pembelian
        '
        ' TODO: Analisis kapan jurnal penyesuaian benar-benar diperlukan setelah test hapus PB-002
        '
        ' Logika yang benar (jika diperlukan):
        ' 1. Hitung total nilai persediaan menurut tbl_barang (HPP × Stok untuk semua barang)
        ' 2. Baca saldo 01.04.001 dari tbl_datareferensi (setelah jurnal pembelian dihapus)
        ' 3. Selisih = Nilai persediaan - Saldo 01.04.001
        ' 4. Jika selisih > 0: persediaan kurang dicatat, D: 01.04.001, K: 06.04.002
        '    Jika selisih < 0: persediaan lebih dicatat, D: 06.04.002, K: 01.04.001

        '' Hitung total nilai persediaan dari tbl_barang untuk barang yang terlibat
        'Dim totalNilaiPersediaan As Decimal = 0D
        'For Each item In detailFaktur
        '    Using cmdNilai As New MySqlCommand(
        '        "SELECT HARGA_BELI, " &
        '        If(lokasi = "GUDANG", "STOK_GUDANG", "STOK_TOKO") &
        '        " FROM tbl_barang WHERE ID_BARANG = @kode",
        '        conn, transaction)
        '        cmdNilai.Parameters.AddWithValue("@kode", item.IdBarang)
        '        Using rd = cmdNilai.ExecuteReader()
        '            If rd.Read() Then
        '                Dim hpp As Decimal = If(IsDBNull(rd(0)), 0D, Convert.ToDecimal(rd(0)))
        '                Dim stok As Decimal = If(IsDBNull(rd(1)), 0D, Convert.ToDecimal(rd(1)))
        '                totalNilaiPersediaan += Math.Round(hpp * stok, 0)
        '            End If
        '        End Using
        '    End Using
        'Next

        '' Baca saldo 01.04.001 saat ini (sebelum jurnal penyesuaian)
        'Dim saldoPersediaanJurnal As Decimal = 0D
        'Using cmdSaldo As New MySqlCommand(
        '    "SELECT SALDO_AKHIR FROM tbl_datareferensi WHERE KODE_AKUN = @kode",
        '    conn, transaction)
        '    cmdSaldo.Parameters.AddWithValue("@kode", KODE_REK_BARANG)
        '    Dim val = cmdSaldo.ExecuteScalar()
        '    saldoPersediaanJurnal = If(val Is Nothing OrElse IsDBNull(val), 0D, Convert.ToDecimal(val))
        'End Using

        '' Hitung selisih
        'totalSelisihPersediaan = totalNilaiPersediaan - saldoPersediaanJurnal

        '' NO_TRANSAKSI pakai nomor faktur yang dihapus — aman karena sudah di-delete di Step 6
        'If totalSelisihPersediaan <> 0D Then
        '    Dim nominalAdj As Decimal = Math.Abs(totalSelisihPersediaan)
        '    Dim kodeD As String, namaD As String, kodeK As String, namaK As String
        '    Dim uraianAdj As String

        '    If totalSelisihPersediaan > 0 Then
        '        ' Nilai persediaan > Saldo jurnal → persediaan kurang dicatat
        '        ' Tambah saldo persediaan
        '        kodeD = KODE_REK_BARANG
        '        namaD = "PERSEDIAAN BARANG"
        '        kodeK = "06.04.002"
        '        namaK = "PENYESUAIAN HARGA POKOK"
        '        uraianAdj = "Penyesuaian HPP moving average — hapus faktur " & faktur &
        '                    " (nilai persediaan kurang dicatat Rp " &
        '                    ModuleAngka.FormatRupiah(nominalAdj) & ")"
        '    Else
        '        ' Nilai persediaan < Saldo jurnal → persediaan lebih dicatat
        '        ' Kurangi saldo persediaan
        '        kodeD = "06.04.002"
        '        namaD = "PENYESUAIAN HARGA POKOK"
        '        kodeK = KODE_REK_BARANG
        '        namaK = "PERSEDIAAN BARANG"
        '        uraianAdj = "Penyesuaian HPP moving average — hapus faktur " & faktur &
        '                    " (nilai persediaan lebih dicatat Rp " &
        '                    ModuleAngka.FormatRupiah(nominalAdj) & ")"
        '    End If

        '    Using cmdAdj As New MySqlCommand(
        '        "INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
        '        "NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, " &
        '        "JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
        '        "VALUES (@NO, NOW(), @NOTA, @URAIAN, @ND, @KD, @NK, @KK, @NOM, " &
        '        "'Penyesuaian HPP', @LOK, @USR, @PC)",
        '        conn, transaction)
        '        cmdAdj.Parameters.AddWithValue("@NO", faktur)
        '        cmdAdj.Parameters.AddWithValue("@NOTA", "ADJ-" & faktur)
        '        cmdAdj.Parameters.AddWithValue("@URAIAN", uraianAdj)
        '        cmdAdj.Parameters.AddWithValue("@ND", namaD)
        '        cmdAdj.Parameters.AddWithValue("@KD", kodeD)
        '        cmdAdj.Parameters.AddWithValue("@NK", namaK)
        '        cmdAdj.Parameters.AddWithValue("@KK", kodeK)
        '        cmdAdj.Parameters.AddWithValue("@NOM", nominalAdj)
        '        cmdAdj.Parameters.AddWithValue("@LOK", lokasi)
        '        cmdAdj.Parameters.AddWithValue("@USR", NamaUser)
        '        cmdAdj.Parameters.AddWithValue("@PC", Environment.MachineName)
        '        cmdAdj.ExecuteNonQuery()
        '    End Using

        '    ' Tambahkan kedua akun ke daftar yang perlu di-update saldo
        '    akunTerlibat.Add(KODE_REK_BARANG)
        '    akunTerlibat.Add("06.04.002")
        'End If

        ' ── Step 8: Update saldo akun di tbl_datareferensi ───────────────────────
        ' [FU] melakukan ini | [FP] tidak (di caller) → diambil dari [FU]
        For Each kodeAkun As String In akunTerlibat
            UpdateSaldoAkun(kodeAkun, transaction)
        Next

    End Sub

    ''' <summary>
    ''' Baca HARGA_BELI (HPP) terkini dari tbl_barang dalam transaction aktif.
    ''' Dipakai untuk snapshot HPP sebelum dan sesudah RecalculateHppSetelahHapus.
    ''' </summary>
    Private Function BacaHppSaatIni(ByVal kodeBarang As String,
                                     ByVal transaction As MySqlTransaction) As Decimal
        Using cmd As New MySqlCommand(
            "SELECT HARGA_BELI FROM tbl_barang WHERE ID_BARANG = @kode",
            conn, transaction)
            cmd.Parameters.AddWithValue("@kode", kodeBarang)
            Dim val = cmd.ExecuteScalar()
            Return If(val Is Nothing OrElse IsDBNull(val), 0D, Convert.ToDecimal(val))
        End Using
    End Function

    ''' <summary>
    ''' Query HARGA_BELI_SATUAN dari faktur tersisa yang paling baru (ORDER BY TGL_BELI DESC).
    ''' Dipakai untuk update HARGA_BELI_TERAKHIR setelah hapus faktur.
    ''' Jika tidak ada faktur tersisa, return 0.
    ''' </summary>
    Private Function QueryHargaBeliTerakhirDariFakturTersisa(ByVal kodeBarang As String,
                                                              ByVal fakturDikecualikan As String,
                                                              ByVal lokasi As String,
                                                              ByVal transaction As MySqlTransaction) As Decimal
        Using cmd As New MySqlCommand(
            "SELECT pd.HARGA_BELI_SATUAN " &
            "FROM pembelian_detail pd " &
            "INNER JOIN pembelian p ON p.ID_PEMBELIAN = pd.FAKTUR_BELI " &
            "WHERE pd.ID_BARANG = @kode " &
            "AND pd.LOKASI = @lokasi " &
            "AND pd.FAKTUR_BELI <> @dikecualikan " &
            "ORDER BY p.TGL_BELI DESC, pd.NO DESC " &
            "LIMIT 1",
            conn, transaction)
            cmd.Parameters.AddWithValue("@kode", kodeBarang)
            cmd.Parameters.AddWithValue("@lokasi", lokasi)
            cmd.Parameters.AddWithValue("@dikecualikan", fakturDikecualikan)
            Dim val = cmd.ExecuteScalar()
            Return If(val Is Nothing OrElse IsDBNull(val), 0D, Convert.ToDecimal(val))
        End Using
    End Function

    ''' <summary>
    ''' Hitung ulang HPP (HARGA_BELI) di tbl_barang untuk satu barang
    ''' berdasarkan weighted average dari semua pembelian_detail yang tersisa
    ''' (tidak termasuk faktur yang sedang dihapus).
    ''' Titik awal kalkulasi: HARGA_AVERAGE dari baris pertama tersisa
    ''' = HPP tbl_barang sebelum faktur pertama itu masuk (Opsi B).
    ''' Jika tidak ada faktur tersisa: kembalikan ke HARGA_AVERAGE faktur yang dihapus
    ''' = HPP sebelum faktur itu pernah masuk.
    ''' </summary>
    ''' <param name="kodeBarang">ID_BARANG yang akan di-recalculate</param>
    ''' <param name="fakturDikecualikan">Nomor faktur yang sedang dihapus</param>
    ''' <param name="lokasi">Lokasi: "TOKO" atau "GUDANG"</param>
    ''' <param name="transaction">Transaction aktif dari caller</param>
    Private Sub RecalculateHppSetelahHapus(ByVal kodeBarang As String,
                                            ByVal fakturDikecualikan As String,
                                            ByVal lokasi As String,
                                            ByVal transaction As MySqlTransaction)

        ' Ambil semua faktur tersisa untuk barang ini, urut dari terlama ke terbaru
        Dim rows As New List(Of (HargaSatuan As Decimal, QtySat As Decimal, HargaAverage As Decimal))
        Using cmd As New MySqlCommand(
            "SELECT pd.HARGA_BELI_SATUAN, pd.QTY_SAT, pd.HARGA_AVERAGE " &
            "FROM pembelian_detail pd " &
            "INNER JOIN pembelian p ON p.ID_PEMBELIAN = pd.FAKTUR_BELI " &
            "WHERE pd.ID_BARANG = @kode " &
            "AND pd.LOKASI = @lokasi " &
            "AND pd.FAKTUR_BELI <> @dikecualikan " &
            "ORDER BY p.TGL_BELI ASC, pd.NO ASC",
            conn, transaction)
            cmd.Parameters.AddWithValue("@kode", kodeBarang)
            cmd.Parameters.AddWithValue("@lokasi", lokasi)
            cmd.Parameters.AddWithValue("@dikecualikan", fakturDikecualikan)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    rows.Add((
                        If(IsDBNull(rd("HARGA_BELI_SATUAN")), 0D, Convert.ToDecimal(rd("HARGA_BELI_SATUAN"))),
                        If(IsDBNull(rd("QTY_SAT")), 0D, Convert.ToDecimal(rd("QTY_SAT"))),
                        If(IsDBNull(rd("HARGA_AVERAGE")), 0D, Convert.ToDecimal(rd("HARGA_AVERAGE")))
                    ))
                End While
            End Using
        End Using

        Dim hppBaru As Decimal

        If rows.Count = 0 Then
            ' Tidak ada faktur tersisa — kembalikan ke HPP sebelum faktur yang dihapus pernah masuk
            ' HARGA_AVERAGE di faktur yang dihapus = snapshot HPP sebelum faktur itu masuk
            Using cmdAwal As New MySqlCommand(
                "SELECT HARGA_AVERAGE FROM pembelian_detail " &
                "WHERE ID_BARANG = @kode AND FAKTUR_BELI = @fk LIMIT 1",
                conn, transaction)
                cmdAwal.Parameters.AddWithValue("@kode", kodeBarang)
                cmdAwal.Parameters.AddWithValue("@fk", fakturDikecualikan)
                Dim val = cmdAwal.ExecuteScalar()
                hppBaru = If(val Is Nothing OrElse IsDBNull(val), 0D, Convert.ToDecimal(val))
            End Using
        Else
            ' Opsi B: titik awal = HARGA_AVERAGE baris pertama tersisa
            ' = HPP tbl_barang sebelum faktur pertama itu masuk
            Dim stokRunning As Decimal = 0D
            Dim hppRunning As Decimal = rows(0).HargaAverage

            For Each r In rows
                If stokRunning + r.QtySat > 0 Then
                    hppRunning = Math.Round(
                        (hppRunning * stokRunning + r.HargaSatuan * r.QtySat) /
                        (stokRunning + r.QtySat), 4)
                End If
                stokRunning += r.QtySat
            Next
            hppBaru = hppRunning
        End If

        ' Update HARGA_BELI di tbl_barang
        Using cmdUpdate As New MySqlCommand(
            "UPDATE tbl_barang SET HARGA_BELI = @hpp WHERE ID_BARANG = @kode",
            conn, transaction)
            cmdUpdate.Parameters.AddWithValue("@hpp", hppBaru)
            cmdUpdate.Parameters.AddWithValue("@kode", kodeBarang)
            cmdUpdate.ExecuteNonQuery()
        End Using

    End Sub

#End Region

#Region "HAPUS PENJUALAN"

    ''' <summary>
    ''' Hapus satu faktur penjualan secara permanen.
    ''' Efek: Menambah stok kembali (karena penjualan sebelumnya mengeluarkan barang).
    ''' </summary>
    Public Sub HapusPenjualan(ByVal faktur As String,
                               ByVal lokasi As String,
                               ByVal transaction As MySqlTransaction)

        Dim updateStokField As String = If(lokasi = "GUDANG", "PENJUALAN_GUDANG", "PENJUALAN_TOKO")

        ' 1. Kumpulkan akun terlibat SEBELUM delete JurnalUmum
        Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Using cmdAkun As New MySqlCommand(
            "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
            "UNION " &
            "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
            conn, transaction)
            cmdAkun.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdAkun.ExecuteReader()
                While rd.Read()
                    Dim kode As String = rd(0).ToString().Trim()
                    If kode <> "" Then akunTerlibat.Add(kode)
                End While
            End Using
        End Using

        ' 2. Baca detail penjualan dari DB untuk pembalikan counter stok
        Dim detailJual As New List(Of (IdBarang As String, QtySat As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_BARANG, QTY_SATUAN FROM penjualan_detail WHERE FAKTUR_JUAL = @fk",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    detailJual.Add((rd("ID_BARANG").ToString(), ModuleAngka.ParseDecimal(rd("QTY_SATUAN"))))
                End While
            End Using
        End Using

        ' 3. Cek apakah sudah ada pembayaran piutang (Jika status Piutang)
        Using cmdCekBayar As New MySqlCommand(
            "SELECT COUNT(*) FROM piutang_detail WHERE ID_JUAL = @fk AND JENIS = 'BAYAR'", conn, transaction)
            cmdCekBayar.Parameters.AddWithValue("@fk", faktur)
            Dim sudahDibayar As Integer = Convert.ToInt32(cmdCekBayar.ExecuteScalar())
            If sudahDibayar > 0 Then
                If MessageBox.Show("Faktur penjualan ini sudah memiliki pembayaran piutang. " &
                   "Menghapus akan mempengaruhi histori piutang. Lanjutkan?",
                   "Peringatan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                    Throw New OperationCanceledException("Dibatalkan oleh pengguna.")
                End If
            End If
        End Using

        ' 4. Proses pembalikan counter stok per barang
        Dim auditDGV As New Dictionary(Of String, Decimal)()
        Dim auditDelta As New Dictionary(Of String, Decimal)()

        For Each item In detailJual
            If auditDGV.ContainsKey(item.IdBarang) Then auditDGV(item.IdBarang) += item.QtySat Else auditDGV(item.IdBarang) = item.QtySat

            ' Kurangi counter PENJUALAN_x (Hapus penjualan = membatalkan pengeluaran barang)
            Using cmdCounter As New MySqlCommand(
                $"UPDATE tbl_barang SET {updateStokField} = {updateStokField} - @qty WHERE ID_BARANG = @kode",
                conn, transaction)
                cmdCounter.Parameters.AddWithValue("@qty", item.QtySat)
                cmdCounter.Parameters.AddWithValue("@kode", item.IdBarang)
                cmdCounter.ExecuteNonQuery()
            End Using

            ' Hitung ulang stok fisik dari counter
            Dim sebelum As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)
            HitungStokPerubahan(item.IdBarang, transaction)
            Dim sesudah As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)

            ' Delta stok (sesudah - sebelum. Hapus penjualan menambah stok kembali)
            Dim delta As Decimal = sesudah - sebelum
            If auditDelta.ContainsKey(item.IdBarang) Then auditDelta.Add(item.IdBarang, delta) Else auditDelta(item.IdBarang) += delta
        Next

        ' 5. Hapus data piutang detail (JENIS='JUAL')
        Using cmdHapusPiutang As New MySqlCommand(
            "DELETE FROM piutang_detail WHERE ID_JUAL = @fk AND JENIS = 'JUAL'", conn, transaction)
            cmdHapusPiutang.Parameters.AddWithValue("@fk", faktur)
            cmdHapusPiutang.ExecuteNonQuery()
        End Using

        ' 6. Hapus data dari semua tabel terkait
        For Each query As String In {
            "DELETE FROM penjualan WHERE ID_PENJUALAN = @fk",
            "DELETE FROM penjualan_detail WHERE FAKTUR_JUAL = @fk",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @fk"
        }
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' 7. Audit stok transaksi
        AuditStokTransaksi(faktur, "Hapus Penjualan", auditDGV, Nothing, Nothing, auditDelta, transaction)

        ' 8. Update saldo akun di tbl_datareferensi
        For Each kodeAkun As String In akunTerlibat
            UpdateSaldoAkun(kodeAkun, transaction)
        Next
    End Sub

#End Region

#Region "HAPUS RETUR (BELI & JUAL)"

    ''' <summary>
    ''' Hapus satu faktur retur pembelian secara permanen.
    ''' Efek: Menambah stok kembali (karena retur beli sebelumnya mengeluarkan barang).
    ''' </summary>
    Public Sub HapusReturPembelian(ByVal faktur As String,
                                    ByVal lokasi As String,
                                    ByVal transaction As MySqlTransaction)

        Dim updateStokField As String = If(lokasi = "GUDANG", "RETUR_BELI_GUDANG", "RETUR_BELI_TOKO")

        ' 1. Ambil data Header Retur (PENTING: untuk update tabel pembelian asal)
        Dim idPembelian As String = ""
        Dim totalRupiah As Decimal = 0D
        Dim kodeRekening As String = ""
        Using cmdHeader As New MySqlCommand(
            "SELECT ID_PEMBELIAN, TOTAL_RUPIAH, KODE_REKENING FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN = @fk",
            conn, transaction)
            cmdHeader.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdHeader.ExecuteReader()
                If rd.Read() Then
                    idPembelian = rd("ID_PEMBELIAN").ToString()
                    totalRupiah = ModuleAngka.ParseDecimal(rd("TOTAL_RUPIAH"))
                    kodeRekening = rd("KODE_REKENING").ToString()
                End If
            End Using
        End Using

        ' 2. Kumpulkan akun terlibat SEBELUM delete JurnalUmum
        Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Using cmdAkun As New MySqlCommand(
            "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
            "UNION " &
            "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
            conn, transaction)
            cmdAkun.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdAkun.ExecuteReader()
                While rd.Read()
                    Dim kode As String = rd(0).ToString().Trim()
                    If kode <> "" Then akunTerlibat.Add(kode)
                End While
            End Using
        End Using

        ' 3. Update tabel PEMBELIAN (Pembalikan nilai Retur & Tagihan)
        ' Jika kode rekening adalah Hutang Beli, maka ini adalah retur "Potong Hutang"
        If idPembelian <> "" Then
            Dim updateBeliQuery As String =
                If(kodeRekening = Kode_rek_Hutang_Beli,
                   "UPDATE pembelian SET RETUR = RETUR - @rp, TAGIHAN = TAGIHAN + @rp WHERE ID_PEMBELIAN = @id",
                   "UPDATE pembelian SET RETUR = RETUR - @rp WHERE ID_PEMBELIAN = @id")

            Using cmdUpdateBeli As New MySqlCommand(updateBeliQuery, conn, transaction)
                cmdUpdateBeli.Parameters.AddWithValue("@rp", totalRupiah)
                cmdUpdateBeli.Parameters.AddWithValue("@id", idPembelian)
                cmdUpdateBeli.ExecuteNonQuery()
            End Using
        End If

        ' 4. Baca detail retur untuk pembalikan counter stok
        Dim detailRetur As New List(Of (IdBarang As String, QtySat As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_BARANG, QTY_SAT FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = @fk",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    detailRetur.Add((rd("ID_BARANG").ToString(), ModuleAngka.ParseDecimal(rd("QTY_SAT"))))
                End While
            End Using
        End Using

        ' 5. Proses pembalikan counter stok per barang
        Dim auditDGV As New Dictionary(Of String, Decimal)()
        Dim auditDelta As New Dictionary(Of String, Decimal)()

        For Each item In detailRetur
            If auditDGV.ContainsKey(item.IdBarang) Then auditDGV(item.IdBarang) += item.QtySat Else auditDGV(item.IdBarang) = item.QtySat

            ' Kurangi counter RETUR_BELI_x (Hapus retur = membatalkan pengembalian barang)
            Using cmdCounter As New MySqlCommand(
                $"UPDATE tbl_barang SET {updateStokField} = {updateStokField} - @qty WHERE ID_BARANG = @kode",
                conn, transaction)
                cmdCounter.Parameters.AddWithValue("@qty", item.QtySat)
                cmdCounter.Parameters.AddWithValue("@kode", item.IdBarang)
                cmdCounter.ExecuteNonQuery()
            End Using

            ' Hitung ulang stok fisik dari counter
            Dim sebelum As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)
            HitungStokPerubahan(item.IdBarang, transaction)
            Dim sesudah As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)

            ' Delta stok (sesudah - sebelum karena hapus retur beli menambah stok kembali)
            Dim delta As Decimal = sesudah - sebelum
            If auditDelta.ContainsKey(item.IdBarang) Then auditDelta(item.IdBarang) += delta Else auditDelta(item.IdBarang) = delta
        Next

        ' 6. Hapus data dari semua tabel terkait
        For Each query As String In {
            "DELETE FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN = @fk",
            "DELETE FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = @fk",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @fk"
        }
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' 7. Audit stok transaksi
        AuditStokTransaksi(faktur, "Hapus Retur Pembelian", auditDGV, Nothing, Nothing, auditDelta, transaction)

        ' 8. Update saldo akun di tbl_datareferensi
        For Each kodeAkun As String In akunTerlibat
            UpdateSaldoAkun(kodeAkun, transaction)
        Next
    End Sub

    ''' <summary>
    ''' Hapus satu faktur retur penjualan secara permanen.
    ''' Efek: Mengurangi stok kembali (karena retur jual sebelumnya menambah barang masuk).
    ''' </summary>
    Public Sub HapusReturPenjualan(ByVal faktur As String,
                                    ByVal lokasi As String,
                                    ByVal transaction As MySqlTransaction)

        Dim updateStokField As String = If(lokasi = "GUDANG", "RETUR_JUAL_GUDANG", "RETUR_JUAL_TOKO")

        ' 1. Ambil data Header Retur (PENTING: untuk update tabel penjualan asal)
        Dim idPenjualan As String = ""
        Dim totalRupiah As Decimal = 0D
        Dim kodeRekening As String = ""
        Using cmdHeader As New MySqlCommand(
            "SELECT ID_PENJUALAN, TOTAL_RUPIAH, KODE_REKENING FROM retur_penjualan WHERE ID_RETUR_PENJUALAN = @fk",
            conn, transaction)
            cmdHeader.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdHeader.ExecuteReader()
                If rd.Read() Then
                    idPenjualan = rd("ID_PENJUALAN").ToString()
                    totalRupiah = ModuleAngka.ParseDecimal(rd("TOTAL_RUPIAH"))
                    kodeRekening = rd("KODE_REKENING").ToString()
                End If
            End Using
        End Using

        ' 2. Cari tanggal retur sebelumnya (untuk restore TGL_RETUR di tabel penjualan)
        Dim tglReturSebelumnya As Object = DBNull.Value
        If idPenjualan <> "" Then
            Using cmdTgl As New MySqlCommand(
                "SELECT TGL_RETUR_JUAL FROM retur_penjualan " &
                "WHERE ID_PENJUALAN = @idj AND ID_RETUR_PENJUALAN <> @fk " &
                "ORDER BY TGL_RETUR_JUAL DESC LIMIT 1",
                conn, transaction)
                cmdTgl.Parameters.AddWithValue("@idj", idPenjualan)
                cmdTgl.Parameters.AddWithValue("@fk", faktur)
                Dim val = cmdTgl.ExecuteScalar()
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then tglReturSebelumnya = val
            End Using
        End If

        ' 3. Kumpulkan akun terlibat SEBELUM delete JurnalUmum
        Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Using cmdAkun As New MySqlCommand(
            "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
            "UNION " &
            "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
            conn, transaction)
            cmdAkun.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdAkun.ExecuteReader()
                While rd.Read()
                    Dim kode As String = rd(0).ToString().Trim()
                    If kode <> "" Then akunTerlibat.Add(kode)
                End While
            End Using
        End Using

        ' 4. Update tabel PENJUALAN (Pembalikan nilai Retur, Tagihan, & Tanggal)
        If idPenjualan <> "" Then
            Dim updateJualQuery As String =
                If(kodeRekening = Kode_rek_Piutang_Jual,
                   "UPDATE penjualan SET NILAI_RETUR = NILAI_RETUR - @rp, SISA_TAGIHAN = SISA_TAGIHAN + @rp, TGL_RETUR = @tgl WHERE ID_PENJUALAN = @id",
                   "UPDATE penjualan SET NILAI_RETUR = NILAI_RETUR - @rp, TGL_RETUR = @tgl WHERE ID_PENJUALAN = @id")

            Using cmdUpdateJual As New MySqlCommand(updateJualQuery, conn, transaction)
                cmdUpdateJual.Parameters.AddWithValue("@rp", totalRupiah)
                cmdUpdateJual.Parameters.AddWithValue("@tgl", tglReturSebelumnya)
                cmdUpdateJual.Parameters.AddWithValue("@id", idPenjualan)
                cmdUpdateJual.ExecuteNonQuery()
            End Using
        End If

        ' 5. Baca detail retur untuk pembalikan counter stok
        Dim detailRetur As New List(Of (IdBarang As String, QtySat As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_BARANG, QTY_SATUAN FROM retur_penjualan_detail WHERE ID_RETUR_PENJUALAN = @fk",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    detailRetur.Add((rd("ID_BARANG").ToString(), ModuleAngka.ParseDecimal(rd("QTY_SATUAN"))))
                End While
            End Using
        End Using

        ' 6. Proses pembalikan counter stok per barang
        Dim auditDGV As New Dictionary(Of String, Decimal)()
        Dim auditDelta As New Dictionary(Of String, Decimal)()

        For Each item In detailRetur
            If auditDGV.ContainsKey(item.IdBarang) Then auditDGV(item.IdBarang) += item.QtySat Else auditDGV(item.IdBarang) = item.QtySat

            ' Kurangi counter RETUR_JUAL_x (Hapus retur jual = membatalkan penambahan barang dari pelanggan)
            Using cmdCounter As New MySqlCommand(
                $"UPDATE tbl_barang SET {updateStokField} = {updateStokField} - @qty WHERE ID_BARANG = @kode",
                conn, transaction)
                cmdCounter.Parameters.AddWithValue("@qty", item.QtySat)
                cmdCounter.Parameters.AddWithValue("@kode", item.IdBarang)
                cmdCounter.ExecuteNonQuery()
            End Using

            ' Hitung ulang stok fisik dari counter
            Dim sebelum As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)
            HitungStokPerubahan(item.IdBarang, transaction)
            Dim sesudah As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)

            ' Delta stok (sesudah - sebelum. Karena hapus retur jual, stok berkurang)
            Dim delta As Decimal = sesudah - sebelum
            If auditDelta.ContainsKey(item.IdBarang) Then auditDelta(item.IdBarang) += delta Else auditDelta(item.IdBarang) = delta
        Next

        ' 7. Hapus data dari semua tabel terkait
        For Each query As String In {
            "DELETE FROM retur_penjualan WHERE ID_RETUR_PENJUALAN = @fk",
            "DELETE FROM retur_penjualan_detail WHERE ID_RETUR_PENJUALAN = @fk",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @fk"
        }
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' 8. Audit stok transaksi
        AuditStokTransaksi(faktur, "Hapus Retur Penjualan", auditDGV, Nothing, Nothing, auditDelta, transaction)

        ' 9. Update saldo akun di tbl_datareferensi
        For Each kodeAkun As String In akunTerlibat
            UpdateSaldoAkun(kodeAkun, transaction)
        Next
    End Sub

#End Region

End Module
