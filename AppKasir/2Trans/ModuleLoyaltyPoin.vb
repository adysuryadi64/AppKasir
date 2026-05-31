''' <summary>
''' ModuleLoyaltyPoin — Engine Kalkulasi dan Pencatatan Poin Loyalitas Pelanggan.
'''
''' Tanggung jawab modul ini:
'''   - Cache konfigurasi poin dari tabel poin_config (MuatKonfigurasi)
'''   - Hitung poin yang diperoleh dari transaksi penjualan (HitungPoinEarn)
'''   - Catat EARN, REDEEM, dan VOID_EARN ke poin_ledger + update SALDO_POIN (CatatEarn, CatatRedeem, CatatVoidEarn)
'''   - Query saldo dan riwayat poin pelanggan (AmbilSaldoPoin, AmbilPoinEarnDariFaktur)
'''
''' Semua operasi tulis (CatatEarn, CatatRedeem, CatatVoidEarn) menerima MySqlTransaction
''' dari caller — commit/rollback tetap tanggung jawab caller untuk menjamin atomisitas.
'''
''' Requirement: Req 1, Req 2, Req 5, Req 8
''' </summary>
Module ModuleLoyaltyPoin

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: CACHE KONFIGURASI POIN
    ' ═══════════════════════════════════════════════════════════════════
    ' Variabel-variabel ini di-load sekali saat startup (MuatKonfigurasi)
    ' dan di-refresh setiap kali konfigurasi disimpan dari FormMasterPoin.
    ' ═══════════════════════════════════════════════════════════════════

#Region "Cache Konfigurasi"

    ''' <summary>True jika fitur poin loyalitas diaktifkan oleh pemilik toko.</summary>
    Public LP_Aktif As Boolean = False

    ''' <summary>Mekanisme perolehan poin: "PER_ITEM" atau "PER_NOMINAL".</summary>
    Public LP_Mekanisme As String = "PER_ITEM"

    ''' <summary>Jumlah poin yang diberikan per 1 satuan qty item terjual (dipakai saat PER_ITEM).</summary>
    Public LP_PoinPerQty As Decimal = 1D

    ''' <summary>Nilai belanja (Rp) yang menghasilkan 1 poin (dipakai saat PER_NOMINAL). Contoh: 10000 = Rp 10.000 → 1 poin.</summary>
    Public LP_KelipatanNominal As Decimal = 10000D



#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: KONFIGURASI
    ' ═══════════════════════════════════════════════════════════════════

#Region "Konfigurasi"

    ''' <summary>
    ''' Muat konfigurasi poin dari tabel poin_config ke variabel cache modul ini.
    ''' Dipanggil saat aplikasi start (FormUtama.Load atau setelah login)
    ''' dan setelah pengguna menyimpan konfigurasi di FormMasterPoin.
    '''
    ''' Jika tabel poin_config belum ada (migrasi belum dijalankan), fungsi ini
    ''' diam-diam melewati error dan membiarkan nilai default cache tetap berlaku.
    ''' </summary>
    Public Sub MuatKonfigurasi()
        Try
            EnsureConnectionReady()

            ' Baca semua setting langsung dari poin_config (single source of truth)
            Dim sqlCfg As String =
                "SELECT AKTIF, MEKANISME, POIN_PER_QTY, KELIPATAN_NOMINAL " &
                "FROM poin_config ORDER BY ID DESC LIMIT 1"

            Using cmd As New MySqlCommand(sqlCfg, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        LP_Aktif = If(IsDBNull(rd("AKTIF")), False, Convert.ToBoolean(rd("AKTIF")))
                        LP_Mekanisme = If(IsDBNull(rd("MEKANISME")), "PER_ITEM", rd("MEKANISME").ToString())
                        LP_PoinPerQty = If(IsDBNull(rd("POIN_PER_QTY")), 1D, Convert.ToDecimal(rd("POIN_PER_QTY")))
                        LP_KelipatanNominal = If(IsDBNull(rd("KELIPATAN_NOMINAL")), 10000D, Convert.ToDecimal(rd("KELIPATAN_NOMINAL")))
                    Else
                        ' Tabel kosong — pakai default
                        LP_Aktif = False
                        LP_Mekanisme = "PER_ITEM"
                        LP_PoinPerQty = 1D
                        LP_KelipatanNominal = 10000D
                    End If
                End Using
            End Using

        Catch ex As MySqlException
            Debug.WriteLine($"[ModuleLoyaltyPoin.MuatKonfigurasi] Lewati — {ex.Message}")
        Catch ex As Exception
            Debug.WriteLine($"[ModuleLoyaltyPoin.MuatKonfigurasi] Error — {ex.Message}")
        End Try
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: KALKULASI POIN
    ' ═══════════════════════════════════════════════════════════════════

#Region "Kalkulasi Poin"

    ''' <summary>
    ''' Hitung jumlah poin yang diperoleh dari satu transaksi penjualan.
    '''
    ''' Mekanisme PER_ITEM  : total poin = SUM(QtySatuan × LP_PoinPerQty) untuk semua item.
    ''' Mekanisme PER_NOMINAL: total poin = Floor(grandTotal ÷ LP_KelipatanNominal).
    '''
    ''' Mengembalikan 0 jika fitur poin tidak aktif, daftar item kosong,
    ''' atau hasil kalkulasi ≤ 0.
    ''' </summary>
    ''' <param name="daftarItem">Daftar item transaksi yang akan dihitung poinnya.</param>
    ''' <param name="grandTotal">Grand total transaksi setelah pajak (GRAND_TOTAL_STL_PAJAK).</param>
    ''' <returns>Jumlah poin bulat yang diperoleh (≥ 0).</returns>
    Public Function HitungPoinEarn(ByVal daftarItem As List(Of ItemPoin),
                                    ByVal grandTotal As Decimal) As Integer
        ' Guard: fitur tidak aktif atau tidak ada item
        If Not LP_Aktif Then Return 0
        If daftarItem Is Nothing OrElse daftarItem.Count = 0 Then Return 0

        Dim totalPoin As Integer = 0

        Select Case LP_Mekanisme.ToUpper().Trim()

            Case "PER_ITEM"
                ' Req 2.3: total poin = SUM(QTY_SATUAN × LP_PoinPerQty) untuk semua item
                Dim totalRaw As Decimal = 0D
                For Each item As ItemPoin In daftarItem
                    totalRaw += item.QtySatuan * LP_PoinPerQty
                Next
                totalPoin = CInt(Math.Floor(totalRaw))

            Case "PER_NOMINAL"
                ' Req 2.4: total poin = Floor(grandTotal ÷ LP_KelipatanNominal)
                If LP_KelipatanNominal > 0D Then
                    totalPoin = CInt(Math.Floor(grandTotal / LP_KelipatanNominal))
                End If

            Case Else
                ' Mekanisme tidak dikenal — kembalikan 0
                totalPoin = 0

        End Select

        ' Req 2.9: jika hasil ≤ 0, tidak ada poin yang dicatat
        Return Math.Max(0, totalPoin)
    End Function

#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: PENCATATAN POIN (EARN / REDEEM / VOID_EARN)
    ' ═══════════════════════════════════════════════════════════════════
    ' Semua fungsi di region ini menerima MySqlTransaction dari caller.
    ' Commit/rollback tetap tanggung jawab caller.
    ' Req 8.3: operasi poin dan transaksi induk harus dalam satu transaksi atomik.
    ' ═══════════════════════════════════════════════════════════════════

#Region "Pencatatan Poin"

    ''' <summary>
    ''' Catat perolehan poin (EARN) ke poin_ledger dan tambahkan ke SALDO_POIN pelanggan.
    '''
    ''' INSERT ke poin_ledger (TIPE='EARN', JUMLAH_POIN positif).
    ''' UPDATE tbl_pelanggan SET SALDO_POIN = SALDO_POIN + jumlahPoin.
    '''
    ''' Dipanggil dari FormJual setelah INSERT penjualan/penjualan_detail,
    ''' sebelum transaksi di-commit.
    ''' </summary>
    ''' <param name="kodePelanggan">Kode pelanggan (tbl_pelanggan.KODE).</param>
    ''' <param name="jumlahPoin">Jumlah poin yang diperoleh (harus > 0).</param>
    ''' <param name="noFaktur">Nomor faktur penjualan sebagai referensi.</param>
    ''' <param name="trans">Transaksi database aktif dari caller.</param>
    Public Sub CatatEarn(ByVal kodePelanggan As String,
                          ByVal jumlahPoin As Integer,
                          ByVal noFaktur As String,
                          ByVal trans As MySqlTransaction)

        If String.IsNullOrEmpty(kodePelanggan) Then Exit Sub
        If jumlahPoin <= 0 Then Exit Sub

        ' INSERT ke poin_ledger
        Using cmdInsert As New MySqlCommand(
            "INSERT INTO poin_ledger " &
            "(KODE_PELANGGAN, TIPE, JUMLAH_POIN, NO_REFERENSI, KETERANGAN, CREATED_AT, ID_USER) " &
            "VALUES (@kode, 'EARN', @poin, @faktur, @ket, NOW(), @user)",
            conn, trans)

            cmdInsert.Parameters.AddWithValue("@kode", kodePelanggan)
            cmdInsert.Parameters.AddWithValue("@poin", jumlahPoin)
            cmdInsert.Parameters.AddWithValue("@faktur", noFaktur)
            cmdInsert.Parameters.AddWithValue("@ket", $"Poin dari penjualan {noFaktur}")
            cmdInsert.Parameters.AddWithValue("@user", NamaUser)
            cmdInsert.ExecuteNonQuery()
        End Using

        ' UPDATE SALDO_POIN pelanggan
        Using cmdUpdate As New MySqlCommand(
            "UPDATE tbl_pelanggan SET SALDO_POIN = SALDO_POIN + @poin WHERE KODE = @kode",
            conn, trans)

            cmdUpdate.Parameters.AddWithValue("@poin", jumlahPoin)
            cmdUpdate.Parameters.AddWithValue("@kode", kodePelanggan)
            cmdUpdate.ExecuteNonQuery()
        End Using

    End Sub

    ''' <summary>
    ''' Catat penukaran poin (REDEEM) ke poin_ledger dan kurangi SALDO_POIN pelanggan.
    '''
    ''' INSERT ke poin_ledger (TIPE='REDEEM', JUMLAH_POIN negatif).
    ''' UPDATE tbl_pelanggan SET SALDO_POIN = SALDO_POIN - jumlahPoin.
    '''
    ''' Dipanggil dari FormTukarPoin setelah validasi saldo mencukupi,
    ''' sebelum transaksi di-commit.
    ''' </summary>
    ''' <param name="kodePelanggan">Kode pelanggan (tbl_pelanggan.KODE).</param>
    ''' <param name="jumlahPoin">Jumlah poin yang ditukarkan (harus > 0; disimpan sebagai nilai negatif di ledger).</param>
    ''' <param name="noReferensi">Nomor referensi penukaran (format "TP-YYYYMMDD-XXXX").</param>
    ''' <param name="trans">Transaksi database aktif dari caller.</param>
    Public Sub CatatRedeem(ByVal kodePelanggan As String,
                            ByVal jumlahPoin As Integer,
                            ByVal noReferensi As String,
                            ByVal trans As MySqlTransaction)

        If String.IsNullOrEmpty(kodePelanggan) Then Exit Sub
        If jumlahPoin <= 0 Then Exit Sub

        ' INSERT ke poin_ledger — JUMLAH_POIN disimpan negatif untuk REDEEM
        Using cmdInsert As New MySqlCommand(
            "INSERT INTO poin_ledger " &
            "(KODE_PELANGGAN, TIPE, JUMLAH_POIN, NO_REFERENSI, KETERANGAN, CREATED_AT, ID_USER) " &
            "VALUES (@kode, 'REDEEM', @poin, @ref, @ket, NOW(), @user)",
            conn, trans)

            cmdInsert.Parameters.AddWithValue("@kode", kodePelanggan)
            cmdInsert.Parameters.AddWithValue("@poin", -jumlahPoin)   ' negatif
            cmdInsert.Parameters.AddWithValue("@ref", noReferensi)
            cmdInsert.Parameters.AddWithValue("@ket", $"Penukaran poin {noReferensi}")
            cmdInsert.Parameters.AddWithValue("@user", NamaUser)
            cmdInsert.ExecuteNonQuery()
        End Using

        ' UPDATE SALDO_POIN pelanggan — kurangi saldo
        Using cmdUpdate As New MySqlCommand(
            "UPDATE tbl_pelanggan SET SALDO_POIN = SALDO_POIN - @poin WHERE KODE = @kode",
            conn, trans)

            cmdUpdate.Parameters.AddWithValue("@poin", jumlahPoin)
            cmdUpdate.Parameters.AddWithValue("@kode", kodePelanggan)
            cmdUpdate.ExecuteNonQuery()
        End Using

    End Sub

    ''' <summary>
    ''' Catat pembatalan poin EARN (VOID_EARN) ke poin_ledger dan kurangi SALDO_POIN pelanggan.
    '''
    ''' INSERT ke poin_ledger (TIPE='VOID_EARN', JUMLAH_POIN negatif).
    ''' UPDATE tbl_pelanggan SET SALDO_POIN = SALDO_POIN - jumlahPoinVoid,
    '''   dengan batas minimum 0 (SALDO_POIN tidak boleh negatif — Req 8.5).
    '''
    ''' Dipanggil dari FormReturPenjualan setelah INSERT retur,
    ''' sebelum transaksi di-commit.
    ''' </summary>
    ''' <param name="kodePelanggan">Kode pelanggan (tbl_pelanggan.KODE).</param>
    ''' <param name="noFakturAsal">Nomor faktur penjualan asal yang diretur.</param>
    ''' <param name="jumlahPoinVoid">Jumlah poin yang dibatalkan (harus > 0; disimpan sebagai nilai negatif di ledger).</param>
    ''' <param name="trans">Transaksi database aktif dari caller.</param>
    Public Sub CatatVoidEarn(ByVal kodePelanggan As String,
                              ByVal noFakturAsal As String,
                              ByVal jumlahPoinVoid As Integer,
                              ByVal trans As MySqlTransaction)

        If String.IsNullOrEmpty(kodePelanggan) Then Exit Sub
        If jumlahPoinVoid <= 0 Then Exit Sub

        ' Req 5.5 / Req 8.5: Baca saldo terkini dalam transaksi yang sama
        ' untuk memastikan pengurangan tidak melebihi saldo yang tersedia.
        Dim saldoTerkini As Integer = 0
        Using cmdSaldo As New MySqlCommand(
            "SELECT SALDO_POIN FROM tbl_pelanggan WHERE KODE = @kode LIMIT 1",
            conn, trans)

            cmdSaldo.Parameters.AddWithValue("@kode", kodePelanggan)
            Dim val = cmdSaldo.ExecuteScalar()
            saldoTerkini = If(val Is Nothing OrElse IsDBNull(val), 0, Convert.ToInt32(val))
        End Using

        ' Batasi pengurangan maksimal sebesar saldo yang tersedia
        Dim poinYangDikurangi As Integer = Math.Min(jumlahPoinVoid, saldoTerkini)

        If poinYangDikurangi <= 0 Then
            ' Saldo sudah 0 — tidak ada yang perlu dikurangi, tidak perlu catat ledger
            Debug.WriteLine($"[ModuleLoyaltyPoin.CatatVoidEarn] Lewati — saldo pelanggan {kodePelanggan} sudah 0.")
            Exit Sub
        End If

        ' INSERT ke poin_ledger — JUMLAH_POIN disimpan negatif untuk VOID_EARN
        Using cmdInsert As New MySqlCommand(
            "INSERT INTO poin_ledger " &
            "(KODE_PELANGGAN, TIPE, JUMLAH_POIN, NO_REFERENSI, KETERANGAN, CREATED_AT, ID_USER) " &
            "VALUES (@kode, 'VOID_EARN', @poin, @faktur, @ket, NOW(), @user)",
            conn, trans)

            cmdInsert.Parameters.AddWithValue("@kode", kodePelanggan)
            cmdInsert.Parameters.AddWithValue("@poin", -poinYangDikurangi)   ' negatif
            cmdInsert.Parameters.AddWithValue("@faktur", noFakturAsal)
            cmdInsert.Parameters.AddWithValue("@ket", $"Void poin dari retur faktur {noFakturAsal}")
            cmdInsert.Parameters.AddWithValue("@user", NamaUser)
            cmdInsert.ExecuteNonQuery()
        End Using

        ' UPDATE SALDO_POIN — gunakan GREATEST(0, ...) sebagai safety net di level DB
        ' agar SALDO_POIN tidak pernah negatif meskipun ada race condition.
        Using cmdUpdate As New MySqlCommand(
            "UPDATE tbl_pelanggan " &
            "SET SALDO_POIN = GREATEST(0, SALDO_POIN - @poin) " &
            "WHERE KODE = @kode",
            conn, trans)

            cmdUpdate.Parameters.AddWithValue("@poin", poinYangDikurangi)
            cmdUpdate.Parameters.AddWithValue("@kode", kodePelanggan)
            cmdUpdate.ExecuteNonQuery()
        End Using

    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════
    ' REGION: QUERY POIN
    ' ═══════════════════════════════════════════════════════════════════

#Region "Query Poin"

    ''' <summary>
    ''' Ambil saldo poin terkini milik pelanggan dari tbl_pelanggan.
    ''' Mengembalikan 0 jika pelanggan tidak ditemukan atau kolom NULL.
    ''' </summary>
    ''' <param name="kodePelanggan">Kode pelanggan (tbl_pelanggan.KODE).</param>
    ''' <returns>Saldo poin pelanggan (≥ 0).</returns>
    Public Function AmbilSaldoPoin(ByVal kodePelanggan As String) As Integer
        If String.IsNullOrEmpty(kodePelanggan) Then Return 0

        Try
            EnsureConnectionReady()

            Using cmd As New MySqlCommand(
                "SELECT SALDO_POIN FROM tbl_pelanggan WHERE KODE = @kode LIMIT 1",
                conn)

                cmd.Parameters.AddWithValue("@kode", kodePelanggan)
                Dim val = cmd.ExecuteScalar()
                Return If(val Is Nothing OrElse IsDBNull(val), 0, Convert.ToInt32(val))
            End Using

        Catch ex As MySqlException
            If Not TawarMigrasi(ex) Then
                Debug.WriteLine($"[ModuleLoyaltyPoin.AmbilSaldoPoin] Error — {ex.Message}")
            End If
            Return 0
        Catch ex As Exception
            Debug.WriteLine($"[ModuleLoyaltyPoin.AmbilSaldoPoin] Error — {ex.Message}")
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Ambil jumlah poin EARN yang pernah dicatat untuk satu nomor faktur penjualan.
    ''' Query: SELECT JUMLAH_POIN FROM poin_ledger WHERE NO_REFERENSI = noFaktur AND TIPE = 'EARN'.
    ''' Mengembalikan 0 jika tidak ada catatan EARN untuk faktur tersebut.
    ''' Dipakai oleh FormReturPenjualan untuk menentukan berapa poin yang harus di-void.
    ''' </summary>
    ''' <param name="noFaktur">Nomor faktur penjualan.</param>
    ''' <returns>Jumlah poin EARN dari faktur tersebut (≥ 0).</returns>
    Public Function AmbilPoinEarnDariFaktur(ByVal noFaktur As String) As Integer
        If String.IsNullOrEmpty(noFaktur) Then Return 0

        Try
            EnsureConnectionReady()

            Using cmd As New MySqlCommand(
                "SELECT JUMLAH_POIN FROM poin_ledger " &
                "WHERE NO_REFERENSI = @faktur AND TIPE = 'EARN' " &
                "LIMIT 1",
                conn)

                cmd.Parameters.AddWithValue("@faktur", noFaktur)
                Dim val = cmd.ExecuteScalar()
                ' JUMLAH_POIN untuk EARN selalu positif di DB
                Return If(val Is Nothing OrElse IsDBNull(val), 0, Math.Max(0, Convert.ToInt32(val)))
            End Using

        Catch ex As MySqlException
            If Not TawarMigrasi(ex) Then
                Debug.WriteLine($"[ModuleLoyaltyPoin.AmbilPoinEarnDariFaktur] Error — {ex.Message}")
            End If
            Return 0
        Catch ex As Exception
            Debug.WriteLine($"[ModuleLoyaltyPoin.AmbilPoinEarnDariFaktur] Error — {ex.Message}")
            Return 0
        End Try
    End Function

#End Region

End Module

' ═══════════════════════════════════════════════════════════════════════
' CLASS: ItemPoin
' ═══════════════════════════════════════════════════════════════════════
' Data transfer object yang merepresentasikan satu baris item transaksi
' untuk keperluan kalkulasi poin di HitungPoinEarn.
' Diisi oleh FormJual dari data DGV sebelum memanggil HitungPoinEarn.
' ═══════════════════════════════════════════════════════════════════════

''' <summary>
''' Merepresentasikan satu item transaksi penjualan untuk kalkulasi poin.
''' Diisi dari baris DGVDataTransaksi di FormJual sebelum memanggil HitungPoinEarn.
''' </summary>
Public Class ItemPoin

    ''' <summary>
    ''' Qty dalam satuan terkecil (QTY_SATUAN dari penjualan_detail).
    ''' Dipakai pada mekanisme PER_ITEM: poin += QtySatuan × LP_PoinPerQty.
    ''' </summary>
    Public Property QtySatuan As Decimal

    ''' <summary>
    ''' Total harga item (qty × harga satuan, sebelum diskon item).
    ''' Disediakan untuk keperluan kalkulasi proporsional di retur parsial.
    ''' Tidak dipakai langsung oleh HitungPoinEarn (yang memakai grandTotal).
    ''' </summary>
    Public Property TotalHarga As Decimal

End Class
