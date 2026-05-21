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

        ' ── Step 1: Ambil ID_SUPPLIER SEBELUM delete — untuk UpdateHutangSupliyer di akhir ──
        ' Harus diambil di sini karena DELETE di Step 6 akan menghapus baris pembelian.
        Dim idSupplierHapus As String = ""
        Using cmdAmbilSupplier As New MySqlCommand(
            "SELECT ID_SUPPLIER FROM pembelian WHERE ID_PEMBELIAN = @fk LIMIT 1",
            conn, transaction)
            cmdAmbilSupplier.Parameters.AddWithValue("@fk", faktur)
            Dim valSupplier = cmdAmbilSupplier.ExecuteScalar()
            If valSupplier IsNot Nothing AndAlso Not IsDBNull(valSupplier) Then
                idSupplierHapus = valSupplier.ToString().Trim()
            End If
        End Using

        ' ── Step 3: Baca detail faktur yang akan dihapus dari DB ─────────────────
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

        ' ── Step 4: Cek hutang_detail JENIS='BAYAR' — konfirmasi jika ada ────────
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
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum agar masih bisa baca jurnal
        ReversalSaldoAkunDariFaktur(faktur, transaction)

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
        '    ReversalSaldoAkunDariFaktur(faktur, transaction)
        'End If

        ' ── Step 9: Update HutangAkhir supplier ──────────────────────────────────
        ' Simetris dengan SimpanTransaksi di FormPembelian yang memanggil UpdateHutangSupliyer.
        ' ID_SUPPLIER sudah diambil di Step 1 sebelum data dihapus.
        UpdateHutangSupliyer(idSupplierHapus, transaction)

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

        ' 1. Ambil ID_PELANGGAN SEBELUM delete — untuk UpdatePiutangPelanggan di akhir
        ' Harus diambil di sini karena DELETE di Step 6 akan menghapus baris penjualan.
        Dim idPelangganHapus As String = ""
        Using cmdAmbilPelanggan As New MySqlCommand(
            "SELECT ID_PELANGGAN FROM penjualan WHERE ID_PENJUALAN = @fk LIMIT 1",
            conn, transaction)
            cmdAmbilPelanggan.Parameters.AddWithValue("@fk", faktur)
            Dim valPelanggan = cmdAmbilPelanggan.ExecuteScalar()
            If valPelanggan IsNot Nothing AndAlso Not IsDBNull(valPelanggan) Then
                idPelangganHapus = valPelanggan.ToString().Trim()
            End If
        End Using

        ' 3. Baca detail penjualan dari DB untuk pembalikan counter stok
        Dim detailJual As New List(Of (IdBarang As String, QtySat As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_BARANG, QTY_SATUAN FROM penjualan_detail WHERE FAKTUR_JUAL = @fk",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    Dim qty As Decimal = ModuleAngka.ParseDecimal(rd("QTY_SATUAN"))
                    ' Guard: QTY_SATUAN = 0 berarti data tidak valid (ISI_SATUAN=0 saat simpan)
                    ' Jika dibiarkan, PENJUALAN_TOKO -= 0 → stok tidak kembali
                    If qty <= 0 Then
                        Debug.WriteLine($"[HapusPenjualan] SKIP {rd("ID_BARANG")} — QTY_SATUAN={qty} (tidak valid)")
                        Continue While
                    End If
                    detailJual.Add((rd("ID_BARANG").ToString(), qty))
                End While
            End Using
        End Using
        Debug.WriteLine($"[HapusPenjualan] ══════════════════════════════════════════")
        Debug.WriteLine($"[HapusPenjualan] faktur={faktur}, lokasi={lokasi}, field={updateStokField}")
        Debug.WriteLine($"[HapusPenjualan] Detail dari DB ({detailJual.Count} item):")
        For Each d In detailJual
            Debug.WriteLine($"[HapusPenjualan]   kode={d.IdBarang}, qtySat={d.QtySat}")
        Next        ' 4. Cek apakah sudah ada pembayaran piutang (Jika status Piutang)
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

        ' 5. Proses pembalikan counter stok per barang
        Dim auditDGV As New Dictionary(Of String, Decimal)()
        Dim auditDelta As New Dictionary(Of String, Decimal)()
        Dim _swHapus As New System.Diagnostics.Stopwatch()
        _swHapus.Start()

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
            If auditDelta.ContainsKey(item.IdBarang) Then auditDelta(item.IdBarang) += delta Else auditDelta(item.IdBarang) = delta
        Next
        Debug.WriteLine($"[PERF-HAPUSJUAL] ReversalStok ({detailJual.Count} barang) : {_swHapus.ElapsedMilliseconds} ms")
        ' 5. Hapus data piutang detail (JENIS='JUAL')
        Using cmdHapusPiutang As New MySqlCommand(
            "DELETE FROM piutang_detail WHERE ID_JUAL = @fk AND JENIS = 'JUAL'", conn, transaction)
            cmdHapusPiutang.Parameters.AddWithValue("@fk", faktur)
            cmdHapusPiutang.ExecuteNonQuery()
        End Using

        ' 6. Hapus data dari semua tabel terkait
        ' PENTING: ReversalSaldoAkun dipanggil SEBELUM DELETE JurnalUmum
        ' agar masih bisa baca jurnal faktur ini untuk hitung delta
        ReversalSaldoAkunDariFaktur(faktur, transaction)

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

        ' 9. Update PiutangAkhir pelanggan ────────────────────────────────────────
        ' Simetris dengan Prosessimpan di FormJual yang memanggil UpdatePiutangPelanggan.
        ' ID_PELANGGAN sudah diambil di Step 1 sebelum data dihapus.
        Debug.WriteLine($"[PERF-HAPUSJUAL] UpdatePiutangPelanggan={idPelangganHapus}")
        UpdatePiutangPelanggan(idPelangganHapus, transaction)

    End Sub

#End Region

#Region "HAPUS SALES ORDER"

    ''' <summary>
    ''' Hapus satu faktur sales order secara permanen.
    ''' Efek: Menambah stok kembali karena pesanan dibatalkan (membebaskan stok yang ditahan).
    ''' </summary>
    Public Sub HapusSalesOrder(ByVal faktur As String,
                               ByVal lokasi As String,
                               ByVal transaction As MySqlTransaction)

        Dim updateStokField As String = If(lokasi = "GUDANG", "PENJUALAN_GUDANG", "PENJUALAN_TOKO")

        ' 1. Baca detail sales order dari DB untuk pembalikan counter stok yang ditahan
        Dim detailSO As New List(Of (IdBarang As String, QtySat As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_BARANG, QTY_SATUAN FROM sales_order_detail WHERE FAKTUR_JUAL = @fk",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    Dim qty As Decimal = ModuleAngka.ParseDecimal(rd("QTY_SATUAN"))
                    If qty <= 0 Then Continue While
                    detailSO.Add((rd("ID_BARANG").ToString(), qty))
                End While
            End Using
        End Using

        ' 2. Proses pembalikan counter stok per barang
        Dim auditDGV As New Dictionary(Of String, Decimal)()
        Dim auditDelta As New Dictionary(Of String, Decimal)()

        For Each item In detailSO
            If auditDGV.ContainsKey(item.IdBarang) Then auditDGV(item.IdBarang) += item.QtySat Else auditDGV(item.IdBarang) = item.QtySat

            ' Kurangi counter PENJUALAN_x (karena stok dibebaskan kembali dari pesanan)
            Using cmdCounter As New MySqlCommand(
                $"UPDATE tbl_barang SET {updateStokField} = {updateStokField} - @qty WHERE ID_BARANG = @kode",
                conn, transaction)
                cmdCounter.Parameters.AddWithValue("@qty", item.QtySat)
                cmdCounter.Parameters.AddWithValue("@kode", item.IdBarang)
                cmdCounter.ExecuteNonQuery()
            End Using

            ' Hitung ulang stok fisik
            Dim sebelum As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)
            HitungStokPerubahan(item.IdBarang, transaction)
            Dim sesudah As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)

            ' Delta stok
            Dim delta As Decimal = sesudah - sebelum
            If auditDelta.ContainsKey(item.IdBarang) Then auditDelta(item.IdBarang) += delta Else auditDelta(item.IdBarang) = delta
        Next

        ' 3. Hapus data dari tabel terkait (Tidak ada Jurnal/Piutang di SO)
        For Each query As String In {
            "DELETE FROM sales_order WHERE ID_PENJUALAN = @fk",
            "DELETE FROM sales_order_detail WHERE FAKTUR_JUAL = @fk",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @fk"
        }
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' 4. Audit stok transaksi (jika diaktifkan)
        AuditStokTransaksi(faktur, "Hapus Sales Order", auditDGV, Nothing, Nothing, auditDelta, transaction)

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
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum
        ReversalSaldoAkunDariFaktur(faktur, transaction)

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

        ' 8. Update saldo akun — sudah dilakukan sebelum DELETE di atas
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

        ' 1b. Ambil ID_PELANGGAN SEBELUM delete — untuk UpdatePiutangPelanggan di akhir
        ' Harus diambil sekarang karena DELETE di Step 7 akan menghapus baris retur_penjualan.
        ' Hanya relevan jika mode PotongHutang (kodeRekening = Kode_rek_Piutang_Jual).
        Dim idPelangganRetur As String = ""
        If kodeRekening = Kode_rek_Piutang_Jual AndAlso Not String.IsNullOrEmpty(idPenjualan) Then
            Using cmdPelanggan As New MySqlCommand(
                "SELECT ID_PELANGGAN FROM penjualan WHERE ID_PENJUALAN = @idj LIMIT 1",
                conn, transaction)
                cmdPelanggan.Parameters.AddWithValue("@idj", idPenjualan)
                Dim valPelanggan = cmdPelanggan.ExecuteScalar()
                If valPelanggan IsNot Nothing AndAlso Not IsDBNull(valPelanggan) Then
                    idPelangganRetur = valPelanggan.ToString().Trim()
                End If
            End Using
        End If

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
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum
        ReversalSaldoAkunDariFaktur(faktur, transaction)

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

        ' 9. Update saldo akun — sudah dilakukan sebelum DELETE di atas

        ' 10. Update PiutangAkhir pelanggan — hanya jika mode PotongHutang
        ' Simetris dengan BtnSimpan_Click di FormReturPenjualan yang memanggil
        ' UpdatePiutangPelanggan kondisional If CbPotongHutang.Checked.
        ' Kondisi PotongHutang diketahui dari KODE_REKENING = Kode_rek_Piutang_Jual.
        If kodeRekening = Kode_rek_Piutang_Jual Then
            UpdatePiutangPelanggan(idPelangganRetur, transaction)
        End If
    End Sub

#End Region

#Region "HAPUS TRANSFER BARANG"

    ''' <summary>
    ''' Hapus satu faktur transfer barang secara permanen atau sebagai langkah awal edit.
    ''' Membaca detail dari DATABASE (bukan dari grid UI) agar tidak bergantung pada state UI.
    ''' Urutan: kurangi counter stok → HitungStokPerubahan → ReversalSaldoAkun → DELETE.
    ''' HitungStokPerubahan dilakukan SEBELUM DELETE agar sp_hlp_stok_hitung masih bisa
    ''' membaca counter yang sudah dikurangi.
    '''
    ''' Commit/Rollback tetap tanggung jawab caller.
    ''' Dipanggil dari: FormUtama.HapusTransferBarang() dan FormTransferBarang.HapusUntukEdit().
    ''' </summary>
    ''' <param name="faktur">Nomor faktur yang akan dihapus (ID_TRANSFER)</param>
    ''' <param name="lokasi">Lokasi asal barang: "TOKO" atau "GUDANG"</param>
    ''' <param name="labelAudit">Label untuk AuditStokTransaksi, misal "Hapus Transfer Barang" atau "Edit Transfer Barang (hapus lama)"</param>
    ''' <param name="transaction">Transaction aktif dari caller</param>
    Public Sub HapusTransferBarang(ByVal faktur As String,
                                    ByVal lokasi As String,
                                    ByVal labelAudit As String,
                                    ByVal transaction As MySqlTransaction)

        ' ── Tentukan field stok berdasarkan lokasi asal ──────────────────────────
        Dim stokKeluarField As String
        Dim stokMasukField As String

        Select Case lokasi.ToUpper()
            Case "TOKO"
                stokKeluarField = "TRANSFER_BARANG_KELUAR_TOKO"
                stokMasukField = "TRANSFER_BARANG_MASUK_GUDANG"
            Case "GUDANG"
                stokKeluarField = "TRANSFER_BARANG_KELUAR_GUDANG"
                stokMasukField = "TRANSFER_BARANG_MASUK_TOKO"
            Case Else
                Throw New Exception("Lokasi barang tidak valid: " & lokasi)
        End Select

        ' ── Step 2: Baca detail faktur dari DATABASE ──────────────────────────────
        ' Tidak bergantung pada state grid UI (DGVDetail / DgvData).
        Dim detailFaktur As New List(Of (IdBarang As String, NamaBarang As String, QtySat As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_BARANG, NAMA_BARANG, TOTAL_QTY FROM Transfer_Barang_Detail WHERE ID_TRANSFER = @fk",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    detailFaktur.Add((
                        rd("ID_BARANG").ToString(),
                        rd("NAMA_BARANG").ToString(),
                        If(IsDBNull(rd("TOTAL_QTY")), 0D, Convert.ToDecimal(rd("TOTAL_QTY")))
                    ))
                End While
            End Using
        End Using

        ' ── Step 3: Per barang — kurangi counter stok, hitung stok, kumpulkan audit ─
        ' HitungStokPerubahan dilakukan SEBELUM DELETE agar sp_hlp_stok_hitung
        ' masih bisa membaca counter TRANSFER_BARANG_KELUAR/MASUK yang sudah dikurangi.
        Dim updateQuery As String =
            $"UPDATE tbl_barang SET {stokKeluarField} = {stokKeluarField} - @QtySatKeluar, " &
            $"{stokMasukField} = {stokMasukField} - @QtySatMasuk WHERE ID_BARANG = @KodeBarang"

        Dim auditDGV As New Dictionary(Of String, Decimal)()
        Dim auditDelta As New Dictionary(Of String, Decimal)()

        For Each item In detailFaktur
            ' Kurangi counter stok keluar dan masuk
            Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                cmd.Parameters.AddWithValue("@QtySatKeluar", item.QtySat)
                cmd.Parameters.AddWithValue("@QtySatMasuk", item.QtySat)
                cmd.Parameters.AddWithValue("@KodeBarang", item.IdBarang)
                cmd.ExecuteNonQuery()
            End Using

            ' Kumpulkan audit qty
            If auditDGV.ContainsKey(item.IdBarang) Then
                auditDGV(item.IdBarang) += item.QtySat
            Else
                auditDGV(item.IdBarang) = item.QtySat
            End If

            ' Hitung ulang stok fisik SEBELUM DELETE
            Dim sebelum As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)
            HitungStokPerubahan(item.IdBarang, transaction)
            Dim sesudah As Decimal = BacaStokSaatIni(item.IdBarang, lokasi, transaction)

            ' Delta: hapus transfer mengembalikan stok asal (sesudah > sebelum)
            Dim delta As Decimal = sesudah - sebelum
            If auditDelta.ContainsKey(item.IdBarang) Then
                auditDelta(item.IdBarang) += delta
            Else
                auditDelta(item.IdBarang) = delta
            End If
        Next

        ' ── Step 4: Audit stok transaksi ─────────────────────────────────────────
        AuditStokTransaksi(faktur, labelAudit, auditDGV, Nothing, Nothing, auditDelta, transaction)

        ' ── Step 5: DELETE semua data faktur ─────────────────────────────────────
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum
        ReversalSaldoAkunDariFaktur(faktur, transaction)

        For Each query As String In {
            "DELETE FROM Transfer_Barang WHERE ID_TRANSFER = @fk",
            "DELETE FROM Transfer_Barang_Detail WHERE ID_TRANSFER = @fk",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @fk"
        }
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

    End Sub

#End Region

#Region "HAPUS TRANSFER STOK"

    ''' <summary>
    ''' Hapus satu faktur transfer stok secara permanen.
    ''' Transfer stok = tukar barang A (keluar) dengan barang B (masuk) dalam lokasi yang sama.
    ''' Membaca data dari DATABASE (bukan dari DGVTransaksi) agar tidak bergantung pada state UI.
    ''' Urutan: kurangi counter stok → ReversalSaldoAkun → DELETE →
    '''         HitungStokPerubahan (setelah DELETE karena Transfer_stok sudah tidak ada) →
    '''         AuditStokTransaksi.
    ''' Commit/Rollback tetap tanggung jawab caller.
    ''' </summary>
    ''' <param name="faktur">Nomor faktur yang akan dihapus (ID_TRANSFER)</param>
    ''' <param name="lokasi">Lokasi: "TOKO" atau "GUDANG"</param>
    ''' <param name="transaction">Transaction aktif dari caller</param>
    Public Sub HapusTransferStok(ByVal faktur As String,
                                  ByVal lokasi As String,
                                  ByVal transaction As MySqlTransaction)

        ' ── Step 1: Baca data dari DB — tidak bergantung pada DGVTransaksi ────────
        Dim idBarangMasuk As String = ""
        Dim idBarangKeluar As String = ""
        Dim namaBarangMasuk As String = ""
        Dim namaBarangKeluar As String = ""
        Dim qtySatMasuk As Decimal = 0D
        Dim qtySatKeluar As Decimal = 0D

        Using cmd As New MySqlCommand(
            "SELECT ID_BARANG_M, NAMA_BARANG_M, QTY_SAT_M, " &
            "ID_BARANG_K, NAMA_BARANG_K, QTY_SAT_K " &
            "FROM Transfer_stok WHERE ID_TRANSFER = @fk LIMIT 1",
            conn, transaction)
            cmd.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    idBarangMasuk   = rd("ID_BARANG_M").ToString()
                    namaBarangMasuk = rd("NAMA_BARANG_M").ToString()
                    qtySatMasuk     = If(IsDBNull(rd("QTY_SAT_M")), 0D, Convert.ToDecimal(rd("QTY_SAT_M")))
                    idBarangKeluar  = rd("ID_BARANG_K").ToString()
                    namaBarangKeluar = rd("NAMA_BARANG_K").ToString()
                    qtySatKeluar    = If(IsDBNull(rd("QTY_SAT_K")), 0D, Convert.ToDecimal(rd("QTY_SAT_K")))
                Else
                    Throw New Exception("Data transfer stok tidak ditemukan: " & faktur)
                End If
            End Using
        End Using

        ' ── Step 2: Tentukan field counter stok berdasarkan lokasi ───────────────
        Dim fieldMasuk As String
        Dim fieldKeluar As String
        Select Case lokasi.ToUpper()
            Case "GUDANG"
                fieldMasuk  = "TRANSFER_STOK_MASUK_GUDANG"
                fieldKeluar = "TRANSFER_STOK_KELUAR_GUDANG"
            Case "TOKO"
                fieldMasuk  = "TRANSFER_STOK_MASUK_TOKO"
                fieldKeluar = "TRANSFER_STOK_KELUAR_TOKO"
            Case Else
                Throw New Exception("Lokasi tidak valid: " & lokasi)
        End Select

        ' ── Step 3: Kurangi counter stok masuk dan keluar ────────────────────────
        Using cmd As New MySqlCommand(
            $"UPDATE tbl_barang SET {fieldMasuk} = {fieldMasuk} - @qty WHERE ID_BARANG = @kode",
            conn, transaction)
            cmd.Parameters.AddWithValue("@qty", qtySatMasuk)
            cmd.Parameters.AddWithValue("@kode", idBarangMasuk)
            cmd.ExecuteNonQuery()
        End Using

        Using cmd As New MySqlCommand(
            $"UPDATE tbl_barang SET {fieldKeluar} = {fieldKeluar} - @qty WHERE ID_BARANG = @kode",
            conn, transaction)
            cmd.Parameters.AddWithValue("@qty", qtySatKeluar)
            cmd.Parameters.AddWithValue("@kode", idBarangKeluar)
            cmd.ExecuteNonQuery()
        End Using

        ' ── Step 4: DELETE semua data faktur ─────────────────────────────────────
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum
        ReversalSaldoAkunDariFaktur(faktur, transaction)

        For Each q As String In {
            "DELETE FROM Transfer_stok WHERE ID_TRANSFER = @fk",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @fk"
        }
            Using cmd As New MySqlCommand(q, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' ── Step 5: Recalculate stok fisik + kumpulkan audit delta ───────────────
        ' HitungStokPerubahan dilakukan SETELAH DELETE karena Transfer_stok
        ' sudah tidak ada — sp_hlp_stok_hitung membaca dari HistoryBarang/counter.
        Dim sebelumMsk As Decimal = BacaStokSaatIni(idBarangMasuk, lokasi, transaction)
        HitungStokPerubahan(idBarangMasuk, transaction)
        Dim sesudahMsk As Decimal = BacaStokSaatIni(idBarangMasuk, lokasi, transaction)

        Dim sebelumKlr As Decimal = BacaStokSaatIni(idBarangKeluar, lokasi, transaction)
        HitungStokPerubahan(idBarangKeluar, transaction)
        Dim sesudahKlr As Decimal = BacaStokSaatIni(idBarangKeluar, lokasi, transaction)

        ' Delta: barang masuk berkurang (sebelum > sesudah), barang keluar bertambah (sesudah > sebelum)
        Dim auditDelta As New Dictionary(Of String, Decimal)() From {
            {idBarangMasuk,  sebelumMsk - sesudahMsk},
            {idBarangKeluar, sesudahKlr - sebelumKlr}
        }
        ' ── Step 6: Audit stok transaksi ─────────────────────────────────────────
        AuditStokTransaksi(faktur, "Hapus Transfer Stok", Nothing, Nothing, Nothing, auditDelta, transaction)

    End Sub

#End Region

#Region "HAPUS TRANSFER CABANG"

    ''' <summary>
    ''' Hapus satu faktur transfer cabang secara permanen.
    ''' Membaca detail dari DATABASE (transfer_cabang_detail) — tidak bergantung pada DGVDetail.
    ''' Lokasi asal dibaca dari HistoryBarang (JENIS = 'TRANSFER_CABANG_KELUAR').
    ''' Urutan: baca lokasi → kurangi counter → ReversalSaldoAkun → DELETE →
    '''         HitungStokPerubahan.
    ''' Commit/Rollback tetap tanggung jawab caller.
    ''' </summary>
    ''' <param name="faktur">Nomor faktur yang akan dihapus (ID_TRANSFER)</param>
    ''' <param name="transaction">Transaction aktif dari caller</param>
    Public Sub HapusTransferCabang(ByVal faktur As String,
                                    ByVal transaction As MySqlTransaction)

        ' ── Step 1: Baca lokasi asal dari transfer_cabang.LOKASI_ASAL ───────────────
        ' LOKASI_ASAL adalah sumber primer — diisi saat simpan dengan nilai "TOKO"/"GUDANG".
        ' Fallback ke HistoryBarang jika LOKASI_ASAL NULL (data lama sebelum kolom ini ada).
        ' Jika keduanya tidak ditemukan → default "TOKO" (aman untuk data baru).
        Dim lokasiAsal As String = "TOKO"

        ' Coba baca dari transfer_cabang.LOKASI_ASAL (sumber paling reliable)
        Using cmdLokAsal As New MySqlCommand(
            "SELECT LOKASI_ASAL FROM transfer_cabang WHERE ID_TRANSFER = @id LIMIT 1",
            conn, transaction)
            cmdLokAsal.Parameters.AddWithValue("@id", faktur)
            Dim val = cmdLokAsal.ExecuteScalar()
            If val IsNot Nothing AndAlso Not IsDBNull(val) AndAlso Not String.IsNullOrEmpty(val.ToString()) Then
                lokasiAsal = val.ToString().ToUpper().Trim()
                If lokasiAsal <> "GUDANG" Then lokasiAsal = "TOKO"
            Else
                ' Fallback: baca dari HistoryBarang (data lama atau LOKASI_ASAL belum diisi)
                Using cmdLok As New MySqlCommand(
                    "SELECT LOKASI FROM HistoryBarang " &
                    "WHERE FAKTUR = @id AND JENIS = 'TRANSFER_CABANG_KELUAR' LIMIT 1",
                    conn, transaction)
                    cmdLok.Parameters.AddWithValue("@id", faktur)
                    Dim valHist = cmdLok.ExecuteScalar()
                    If valHist IsNot Nothing AndAlso Not IsDBNull(valHist) Then
                        If valHist.ToString().ToUpper() = "GUDANG" Then lokasiAsal = "GUDANG"
                    End If
                End Using
            End If
        End Using

        Dim kolomKeluar As String = If(lokasiAsal = "GUDANG",
            "TRANSFER_CABANG_KELUAR_GUDANG", "TRANSFER_CABANG_KELUAR_TOKO")

        ' ── Step 2: Baca detail dari DB — tidak bergantung pada DGVDetail ─────────
        Dim detailItems As New List(Of (IdBarang As String, QtySat As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_BARANG, TOTAL_QTY FROM transfer_cabang_detail WHERE ID_TRANSFER = @id",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@id", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    Dim kode As String = rd("ID_BARANG").ToString()
                    Dim qty As Decimal = If(IsDBNull(rd("TOTAL_QTY")), 0D, Convert.ToDecimal(rd("TOTAL_QTY")))
                    If Not String.IsNullOrEmpty(kode) Then
                        detailItems.Add((kode, qty))
                    End If
                End While
            End Using
        End Using

        ' ── Step 3: Kurangi counter stok keluar per barang ───────────────────────
        Dim kodeItems As New List(Of String)()
        For Each item In detailItems
            Using cmd As New MySqlCommand(
                $"UPDATE tbl_barang SET {kolomKeluar} = {kolomKeluar} - @qty WHERE ID_BARANG = @kode",
                conn, transaction)
                cmd.Parameters.AddWithValue("@qty", item.QtySat)
                cmd.Parameters.AddWithValue("@kode", item.IdBarang)
                cmd.ExecuteNonQuery()
            End Using
            kodeItems.Add(item.IdBarang)
        Next

        ' ── Step 4: DELETE semua data faktur ─────────────────────────────────────
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum
        ReversalSaldoAkunDariFaktur(faktur, transaction)

        For Each q As String In {
            "DELETE FROM HistoryBarang WHERE FAKTUR = @id",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @id",
            "DELETE FROM transfer_cabang_detail WHERE ID_TRANSFER = @id",
            "DELETE FROM transfer_cabang WHERE ID_TRANSFER = @id"
        }
            Using cmd As New MySqlCommand(q, conn, transaction)
                cmd.Parameters.AddWithValue("@id", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' ── Step 5: Recalculate stok fisik untuk semua barang terlibat ───────────
        ' HitungStokPerubahan dilakukan SETELAH DELETE HistoryBarang
        ' agar sp_hlp_stok_hitung tidak menghitung entry yang sudah dihapus.
        For Each kode As String In kodeItems
            HitungStokPerubahan(kode, transaction)
        Next

    End Sub

#End Region

#Region "HAPUS BAYAR HUTANG"

    ''' <summary>
    ''' Hapus satu faktur bayar hutang secara permanen.
    ''' Membaca detail dari DATABASE (hutang_detail) — tidak bergantung pada DGVDetail.
    ''' Efek: membalik UPDATE pembelian (PEMBAYARAN, TAGIHAN, NOMINALBAYAR, STATUS).
    ''' Commit/Rollback tetap tanggung jawab caller.
    ''' Dipanggil dari: FormUtama.Hapusbayarhutang().
    ''' </summary>
    ''' <param name="faktur">Nomor faktur bayar hutang (NOBAYARHUTANG)</param>
    ''' <param name="transaction">Transaction aktif dari caller</param>
    Public Sub HapusBayarHutang(ByVal faktur As String,
                                 ByVal transaction As MySqlTransaction)

        ' ── Step 1: Baca detail dari DB — tidak bergantung pada DGVDetail ─────────
        Dim detailItems As New List(Of (IdBeli As String, Pembayaran As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_BELI, PEMBAYARAN FROM hutang_detail WHERE ID_BAYAR = @fk",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    detailItems.Add((
                        rd("ID_BELI").ToString(),
                        If(IsDBNull(rd("PEMBAYARAN")), 0D, Convert.ToDecimal(rd("PEMBAYARAN")))
                    ))
                End While
            End Using
        End Using

        ' ── Step 2: Balik UPDATE pembelian per baris detail ───────────────────────
        For Each item In detailItems
            Using cmd As New MySqlCommand(
                "UPDATE pembelian SET " &
                "PEMBAYARAN = PEMBAYARAN - @nominal, " &
                "TAGIHAN = TAGIHAN + @nominal, " &
                "TGL_BAYAR = NULL, " &
                "NOMINALBAYAR = NOMINALBAYAR - @nominal, " &
                "STATUS_TRANSAKSI_BELI = 'Belum Lunas' " &
                "WHERE ID_PEMBELIAN = @idBeli",
                conn, transaction)
                cmd.Parameters.AddWithValue("@nominal", item.Pembayaran)
                cmd.Parameters.AddWithValue("@idBeli", item.IdBeli)
                cmd.ExecuteNonQuery()
            End Using

            ' Balik UPDATE hutang_detail baris JENIS='BELI' — simetris dengan simpan
            ' Saat simpan: HUTANG -, DIBAYAR +, STATUS diupdate
            ' Saat hapus : HUTANG +, DIBAYAR -, STATUS dikembalikan ke 'Belum Lunas'
            Using cmdHD As New MySqlCommand(
                "UPDATE hutang_detail SET " &
                "HUTANG = HUTANG + @nominal, " &
                "DIBAYAR = DIBAYAR - @nominal, " &
                "STATUS = 'Belum Lunas' " &
                "WHERE ID_BELI = @idBeli AND JENIS = 'BELI'",
                conn, transaction)
                cmdHD.Parameters.AddWithValue("@nominal", item.Pembayaran)
                cmdHD.Parameters.AddWithValue("@idBeli", item.IdBeli)
                cmdHD.ExecuteNonQuery()
            End Using
        Next

        ' ── Step 4: DELETE semua data faktur ─────────────────────────────────────
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum
        ReversalSaldoAkunDariFaktur(faktur, transaction)

        For Each q As String In {
            "DELETE FROM hutang WHERE NOBAYARHUTANG = @fk",
            "DELETE FROM Hutang_Detail WHERE ID_BAYAR = @fk",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk"
        }
            Using cmd As New MySqlCommand(q, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' ── Step 5: Update saldo akun — sudah dilakukan sebelum DELETE di atas

    End Sub

#End Region

#Region "HAPUS BAYAR PIUTANG"

    ''' <summary>
    ''' Hapus satu faktur bayar piutang secara permanen.
    ''' Membaca detail dari DATABASE (piutang_detail) — tidak bergantung pada DGVDetail.
    ''' Efek: membalik UPDATE penjualan (BAYAR, SISA_TAGIHAN, NOMINALBAYARPIUTANG, STATUS).
    ''' Commit/Rollback tetap tanggung jawab caller.
    ''' Dipanggil dari: FormUtama.HapusbayarPiutang().
    ''' </summary>
    ''' <param name="faktur">Nomor faktur bayar piutang (ID_BAYAR_PIUTANG)</param>
    ''' <param name="transaction">Transaction aktif dari caller</param>
    Public Sub HapusBayarPiutang(ByVal faktur As String,
                                  ByVal transaction As MySqlTransaction)

        ' ── Step 1: Baca detail dari DB — tidak bergantung pada DGVDetail ─────────
        Dim detailItems As New List(Of (IdJual As String, Pembayaran As Decimal))
        Using cmdDetail As New MySqlCommand(
            "SELECT ID_JUAL, PEMBAYARAN FROM piutang_detail WHERE ID_BAYAR = @fk",
            conn, transaction)
            cmdDetail.Parameters.AddWithValue("@fk", faktur)
            Using rd = cmdDetail.ExecuteReader()
                While rd.Read()
                    detailItems.Add((
                        rd("ID_JUAL").ToString(),
                        If(IsDBNull(rd("PEMBAYARAN")), 0D, Convert.ToDecimal(rd("PEMBAYARAN")))
                    ))
                End While
            End Using
        End Using

        ' ── Step 2: Balik UPDATE penjualan per baris detail ───────────────────────
        For Each item In detailItems
            Using cmd As New MySqlCommand(
                "UPDATE penjualan SET " &
                "BAYAR = BAYAR - @nominal, " &
                "SISA_TAGIHAN = SISA_TAGIHAN + @nominal, " &
                "TGL_PEMBAYARAN = NULL, " &
                "NOMINALBAYARPIUTANG = NOMINALBAYARPIUTANG - @nominal, " &
                "STATUS_TRANSAKSI = 'Belum Lunas' " &
                "WHERE ID_PENJUALAN = @idJual",
                conn, transaction)
                cmd.Parameters.AddWithValue("@nominal", item.Pembayaran)
                cmd.Parameters.AddWithValue("@idJual", item.IdJual)
                cmd.ExecuteNonQuery()
            End Using

            ' Balik UPDATE piutang_detail baris JENIS='JUAL' — simetris dengan simpan
            ' Saat simpan: HUTANG -, DIBAYAR +, STATUS diupdate
            ' Saat hapus : HUTANG +, DIBAYAR -, STATUS dikembalikan ke 'Belum Lunas'
            Using cmdPD As New MySqlCommand(
                "UPDATE piutang_detail SET " &
                "HUTANG = HUTANG + @nominal, " &
                "DIBAYAR = DIBAYAR - @nominal, " &
                "STATUS = 'Belum Lunas' " &
                "WHERE ID_JUAL = @idJual AND JENIS = 'JUAL'",
                conn, transaction)
                cmdPD.Parameters.AddWithValue("@nominal", item.Pembayaran)
                cmdPD.Parameters.AddWithValue("@idJual", item.IdJual)
                cmdPD.ExecuteNonQuery()
            End Using
        Next

        ' ── Step 4: DELETE semua data faktur ─────────────────────────────────────
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum
        ReversalSaldoAkunDariFaktur(faktur, transaction)

        For Each q As String In {
            "DELETE FROM Piutang WHERE ID_BAYAR_PIUTANG = @fk",
            "DELETE FROM Piutang_Detail WHERE ID_BAYAR = @fk",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk"
        }
            Using cmd As New MySqlCommand(q, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' ── Step 5: Update saldo akun — sudah dilakukan sebelum DELETE di atas

    End Sub

#End Region

#Region "HAPUS STOK OPNAME"

    ''' <summary>
    ''' Hapus satu faktur stok opname — dipakai untuk hapus permanen maupun hapus-untuk-edit.
    ''' Membaca data dari parameter (bukan dari grid UI) agar bisa dipanggil dari dua konteks.
    ''' Urutan: kurangi counter OPNAME → HitungStokPerubahan → AuditStokTransaksi →
    '''         DELETE → UpdateSaldoAkun.
    ''' Commit/Rollback tetap tanggung jawab caller.
    ''' Dipanggil dari: FormUtama.Hapusstokopname() dan FormStokOpname.Hapusstokopname().
    ''' </summary>
    ''' <param name="faktur">Nomor faktur stok opname (ID_STOK_OPNAME)</param>
    ''' <param name="idBarang">ID barang yang di-opname</param>
    ''' <param name="qtySat">Qty satuan yang di-opname (untuk reversal counter)</param>
    ''' <param name="lokasi">Lokasi: "TOKO" atau "GUDANG"</param>
    ''' <param name="labelAudit">Label untuk AuditStokTransaksi</param>
    ''' <param name="transaction">Transaction aktif dari caller</param>
    Public Sub HapusStokOpname(ByVal faktur As String,
                                ByVal idBarang As String,
                                ByVal qtySat As Decimal,
                                ByVal lokasi As String,
                                ByVal labelAudit As String,
                                ByVal transaction As MySqlTransaction)

        ' ── Step 1: Tentukan field counter berdasarkan lokasi ────────────────────
        Dim stokField As String
        Select Case lokasi.ToUpper()
            Case "TOKO"   : stokField = "OPNAME_TOKO"
            Case "GUDANG" : stokField = "OPNAME_GUDANG"
            Case Else     : Throw New Exception("Lokasi tidak valid: " & lokasi)
        End Select

        ' ── Step 2: Kurangi counter OPNAME ───────────────────────────────────────
        Using cmd As New MySqlCommand(
            $"UPDATE tbl_barang SET {stokField} = {stokField} - @qty WHERE ID_BARANG = @kode",
            conn, transaction)
            cmd.Parameters.AddWithValue("@qty", qtySat)
            cmd.Parameters.AddWithValue("@kode", idBarang)
            cmd.ExecuteNonQuery()
        End Using

        ' ── Step 4: DELETE semua data faktur ─────────────────────────────────────
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum
        ReversalSaldoAkunDariFaktur(faktur, transaction)

        For Each q As String In {
            "DELETE FROM Stok_Opname WHERE ID_STOK_OPNAME = @fk",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @fk",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @fk"
        }
            Using cmd As New MySqlCommand(q, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' ── Step 5: Recalculate stok + audit delta ────────────────────────────────
        ' HitungStokPerubahan dilakukan SETELAH DELETE HistoryBarang agar
        ' sp_hlp_stok_hitung tidak menghitung entry yang sudah dihapus.
        ' Counter OPNAME sudah dikurangi di Step 2 — sp membaca counter, bukan tabel Stok_Opname.
        Dim sebelum As Decimal = BacaStokSaatIni(idBarang, lokasi, transaction)
        HitungStokPerubahan(idBarang, transaction)
        Dim sesudah As Decimal = BacaStokSaatIni(idBarang, lokasi, transaction)
        Dim auditDelta As New Dictionary(Of String, Decimal)() From {{idBarang, Math.Abs(sesudah - sebelum)}}
        AuditStokTransaksi(faktur, labelAudit, Nothing, Nothing, Nothing, auditDelta, transaction)

    End Sub

#End Region

#Region "HAPUS SURAT JALAN"

    ''' <summary>
    ''' Hapus satu faktur surat jalan secara permanen.
    ''' Surat jalan tidak mempengaruhi stok atau jurnal — hanya delete 2 tabel.
    ''' Commit/Rollback tetap tanggung jawab caller.
    ''' Dipanggil dari: FormUtama.HapusSuratJalan().
    ''' </summary>
    ''' <param name="faktur">Nomor nota surat jalan (NOTA)</param>
    ''' <param name="transaction">Transaction aktif dari caller</param>
    Public Sub HapusSuratJalan(ByVal faktur As String,
                                ByVal transaction As MySqlTransaction)

        For Each q As String In {
            "DELETE FROM Surat_Jalan WHERE NOTA = @fk",
            "DELETE FROM Surat_Jalan_Detail WHERE NOTA = @fk"
        }
            Using cmd As New MySqlCommand(q, conn, transaction)
                cmd.Parameters.AddWithValue("@fk", faktur)
                cmd.ExecuteNonQuery()
            End Using
        Next

    End Sub

#End Region

End Module
