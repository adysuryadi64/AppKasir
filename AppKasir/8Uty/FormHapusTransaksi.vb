Public Class FormHapusTransaksi

    ' ── Mode: "TOKO" | "GUDANG" | "SEMUA" — diset sebelum ShowDialog ────────
    Public Property Mode As String = "SEMUA"

    ' ── Helper log ────────────────────────────────────────────────────────────
    Private Sub Log(msg As String)
        ListBoxHasil.Items.Add(msg)
        ListBoxHasil.TopIndex = ListBoxHasil.Items.Count - 1
        Application.DoEvents()
    End Sub

    Private Sub LogSep()
        Log(New String("─"c, 70))
    End Sub

    ' ── Load: sesuaikan header & tombol berdasarkan Mode ─────────────────────
    Private Sub FormHapusTransaksi_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Select Case Mode.ToUpper()
            Case "TOKO"
                LblHeaderForm.Text = "HAPUS SEMUA TRANSAKSI DI TOKO"
                BtnHapusToko.Text = "Hapus Semua Transaksi TOKO"
            Case "GUDANG"
                LblHeaderForm.Text = "HAPUS SEMUA TRANSAKSI DI GUDANG"
                BtnHapusToko.Text = "Hapus Semua Transaksi GUDANG"
            Case Else
                Mode = "SEMUA"
                LblHeaderForm.Text = "HAPUS TOTAL TRANSAKSI (TOKO + GUDANG)"
                BtnHapusToko.Text = "Hapus Semua Transaksi TOKO + GUDANG"
        End Select

        BtnHapusToko.Visible = True
        BtnHapusToko.Location = New Point(25, 53)
    End Sub

    Private Sub BtnHapusToko_Click(sender As Object, e As EventArgs) Handles BtnHapusToko.Click
        HapusTransaksi(Mode.ToUpper())
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    ' =========================================================================
    ' Mapping kolom lokasi per tabel (hasil audit INFORMATION_SCHEMA):
    '
    ' LOKASIBARANG : penjualan, penjualan_detail
    ' LOKASI       : penjualan_ditahan, retur_penjualan_detail,
    '                stok_opname, transfer_barang, transfer_barang_detail,
    '                transfer_cabang, transfer_cabang_detail,
    '                surat_jalan, surat_jalan_detail, stoktambahkurang,
    '                historybarang, jurnalumum, hutang, hutang_detail,
    '                piutang, piutang_detail, bon_karyawan, gaji_karyawan
    ' JENIS_TRANSFER: transfer_stok  ('TOKO' atau 'GUDANG')
    '
    ' Tidak punya kolom lokasi → TRUNCATE (hapus semua):
    '   penjualan_ditahan_detail (hapus via FK dari penjualan_ditahan)
    '   retur_penjualan          (hapus via FK dari penjualan)
    '   history, tukarbarang
    ' =========================================================================
    Private Sub HapusTransaksi(mode As String)
        Dim label As String = If(mode = "SEMUA", "TOKO + GUDANG", mode)

        If MessageBox.Show(
            $"Anda akan menghapus SELURUH data transaksi {label}." & vbCrLf &
            "Data pembelian dan master data TIDAK akan dihapus." & vbCrLf & vbCrLf &
            "Apakah Anda yakin?",
            $"Peringatan: Hapus Transaksi {label}",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then Exit Sub

        If MessageBox.Show(
            "PERINGATAN TERAKHIR — TIDAK BISA DI-ROLLBACK!" & vbCrLf &
            $"Semua transaksi {label} akan dihapus permanen." & vbCrLf & vbCrLf &
            "Lanjutkan?",
            "Konfirmasi Akhir", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) = DialogResult.No Then Exit Sub

        ListBoxHasil.Items.Clear()
        BtnHapusToko.Enabled = False

        Dim sw As New System.Diagnostics.Stopwatch()
        sw.Start()
        Log($"[{Now:dd/MM/yyyy HH:mm:ss}] MULAI HAPUS TRANSAKSI: {label}")
        LogSep()

        Try
            Using cmd As New MySqlCommand("", conn)
                cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 0"
                cmd.ExecuteNonQuery()
                Log("✅ FOREIGN_KEY_CHECKS = 0")

                ' ── KELOMPOK A: Hapus data transaksi ─────────────────────────
                LogSep()
                Log($"[A] Hapus data transaksi {label}...")

                If mode = "SEMUA" Then
                    ' TRUNCATE — lebih cepat, tidak perlu filter lokasi
                    For Each t As String In {"penjualan", "penjualan_detail",
                                             "penjualan_ditahan", "penjualan_ditahan_detail",
                                             "pembelian, pembelian_detail, pembelian_ditahan, pembelian_ditahan_detail",
                                             "retur_penjualan", "retur_penjualan_detail",
                                             "stok_opname", "transfer_stok",
                                             "transfer_barang", "transfer_barang_detail",
                                             "transfer_cabang", "transfer_cabang_detail",
                                             "surat_jalan", "surat_jalan_detail",
                                             "stoktambahkurang", "tukarbarang",
                                             "historybarang", "jurnalumum", "history",
                                             "hutang", "hutang_detail",
                                             "piutang", "piutang_detail",
                                             "bon_karyawan", "gaji_karyawan"}
                        TruncateTabel(cmd, t)
                    Next
                Else
                    ' ── Tabel dengan kolom LOKASIBARANG ──────────────────────
                    HapusWhere(cmd, "penjualan", "LOKASIBARANG", mode)
                    HapusWhere(cmd, "penjualan_detail", "LOKASIBARANG", mode)

                    ' ── Tabel dengan kolom LOKASI ─────────────────────────────
                     HapusWhere(cmd, "pembelian", "LOKASI", mode)
                      HapusWhere(cmd, "pembelian_detail", "LOKASI", mode)
                    HapusWhere(cmd, "penjualan_ditahan", "LOKASI", mode)
                    HapusWhere(cmd, "retur_penjualan_detail", "LOKASI", mode)
                    HapusWhere(cmd, "stok_opname", "LOKASI", mode)
                    HapusWhere(cmd, "transfer_barang", "LOKASI", mode)
                    HapusWhere(cmd, "transfer_barang_detail", "LOKASI", mode)
                    HapusWhere(cmd, "transfer_cabang", "LOKASI", mode)
                    HapusWhere(cmd, "transfer_cabang_detail", "LOKASI", mode)
                    HapusWhere(cmd, "surat_jalan", "LOKASI", mode)
                    HapusWhere(cmd, "surat_jalan_detail", "LOKASI", mode)
                    HapusWhere(cmd, "stoktambahkurang", "LOKASI", mode)
                    HapusWhere(cmd, "historybarang", "LOKASI", mode)
                    HapusWhere(cmd, "jurnalumum", "LOKASI", mode)
                    HapusWhere(cmd, "hutang", "LOKASI", mode)
                    HapusWhere(cmd, "hutang_detail", "LOKASI", mode)
                    HapusWhere(cmd, "piutang", "LOKASI", mode)
                    HapusWhere(cmd, "piutang_detail", "LOKASI", mode)
                    HapusWhere(cmd, "bon_karyawan", "LOKASI", mode)
                    HapusWhere(cmd, "gaji_karyawan", "LOKASI", mode)

                    ' ── transfer_stok: kolom JENIS_TRANSFER = 'TOKO'/'GUDANG' ─
                    HapusWhere(cmd, "transfer_stok", "JENIS_TRANSFER", mode)

                    ' ── Tidak punya kolom lokasi → hapus via relasi ───────────
                    ' penjualan_ditahan_detail: ikut penjualan_ditahan yang sudah dihapus
                    HapusOrphan(cmd,
                        "DELETE d FROM penjualan_ditahan_detail d " &
                        "LEFT JOIN penjualan_ditahan h ON h.ID_PENJUALAN = d.FAKTUR_JUAL " &
                        "WHERE h.ID_PENJUALAN IS NULL",
                        "penjualan_ditahan_detail")

                    ' retur_penjualan: ikut penjualan yang sudah dihapus
                    HapusOrphan(cmd,
                        "DELETE r FROM retur_penjualan r " &
                        "LEFT JOIN penjualan p ON p.ID_PENJUALAN = r.ID_PENJUALAN " &
                        "WHERE p.ID_PENJUALAN IS NULL",
                        "retur_penjualan")

                    ' history & tukarbarang: tidak ada kolom lokasi → TRUNCATE
                    TruncateTabel(cmd, "history")
                    TruncateTabel(cmd, "tukarbarang")
                End If

                ' ── KELOMPOK B: Reset & recalculate stok ─────────────────────
                LogSep()
                Log($"[B] Reset & recalculate stok {label}...")
                cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1"
                cmd.ExecuteNonQuery()

                ' B0: Reset AWAL ke 0 (basis sp_bat_stok)
                Select Case mode
                    Case "TOKO"
                        cmd.CommandText = "UPDATE tbl_barang SET AWAL_TOKO=0, STOK_AWAL_TOKO=0"
                    Case "GUDANG"
                        cmd.CommandText = "UPDATE tbl_barang SET AWAL_GUDANG=0, STOK_AWAL_GUDANG=0"
                    Case "SEMUA"
                        cmd.CommandText = "UPDATE tbl_barang SET AWAL_TOKO=0, STOK_AWAL_TOKO=0, AWAL_GUDANG=0, STOK_AWAL_GUDANG=0"
                End Select
                Log($"   ✅ AWAL_{label} → 0  ({cmd.ExecuteNonQuery()} baris)")

                ' B1: Reset counter ke 0
                If mode = "TOKO" OrElse mode = "SEMUA" Then
                    FormLoading.ResetAllBarangToko()
                    Log("   ✅ Counter TOKO → 0")
                End If
                If mode = "GUDANG" OrElse mode = "SEMUA" Then
                    FormLoading.ResetAllBarangGudang()
                    Log("   ✅ Counter GUDANG → 0")
                End If

                ' B2: Isi ulang counter dari HistoryBarang
                If mode = "TOKO" OrElse mode = "SEMUA" Then
                    UpdateAllBarangTokoModule()
                    Log("   ✅ Counter TOKO diisi ulang dari HistoryBarang")
                End If
                If mode = "GUDANG" OrElse mode = "SEMUA" Then
                    UpdateAllBarangGudangModule()
                    Log("   ✅ Counter GUDANG diisi ulang dari HistoryBarang")
                End If

                ' B3: Hitung STOK final
                If mode = "TOKO" Then
                    HitungStokToko()
                    Log("   ✅ STOK_TOKO dihitung ulang")
                ElseIf mode = "GUDANG" Then
                    HitungStokGudang()
                    Log("   ✅ STOK_GUDANG dihitung ulang")
                Else
                    HitungSemuaKode()
                    Log("   ✅ STOK_TOKO + STOK_GUDANG dihitung ulang")
                End If

                ' ── KELOMPOK C: Recalculate neraca ───────────────────────────
                LogSep()
                Log("[C] Recalculate piutang, hutang supliyer & neraca...")

                UpdatePiutangDibayar()
                Log("   ✅ Piutang pelanggan dihitung ulang")

                UpdateSupliyerFromPembelianHutangDibayar()
                Log("   ✅ Hutang supliyer dihitung ulang")

                ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()
                Log("   ✅ Neraca dihitung ulang dari JurnalUmum")

                ' ── KELOMPOK D: TRUNCATE tabel temp ──────────────────────────
                LogSep()
                Log("[D] TRUNCATE tabel temporary...")
                cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 0"
                cmd.ExecuteNonQuery()

                For Each t As String In {"temp_bbpembantu", "temp_bon_karyawan",
                                         "temp_datareferensi", "temp_jurnal",
                                         "temp_labarugi", "temp_loading",
                                         "temp_mutasi_barang", "temp_supliyerbayar",
                                         "temp_supliyerhutang", "tempbukubesarpembantu",
                                         "tempjurnalumum"}
                    TruncateTabel(cmd, t)
                Next

                cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1"
                cmd.ExecuteNonQuery()
                Log("✅ FOREIGN_KEY_CHECKS = 1")

                LogSep()
                sw.Stop()
                Log($"✅ SELESAI — {sw.ElapsedMilliseconds} ms  |  {Now:dd/MM/yyyy HH:mm:ss}")
                Log("   Pembelian & master data TIDAK dihapus.")
                LogSep()

                MessageBox.Show(
                    $"Transaksi {label} berhasil dihapus." & vbCrLf &
                    $"Waktu: {sw.ElapsedMilliseconds} ms",
                    "Selesai", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

        Catch ex As Exception
            Try
                Using r As New MySqlCommand("SET FOREIGN_KEY_CHECKS = 1", conn)
                    r.ExecuteNonQuery()
                End Using
            Catch
            End Try
            Log($"❌ ERROR: {ex.Message}")
            MessageBox.Show("Terjadi kesalahan:" & vbCrLf & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            BtnHapusToko.Enabled = True
        End Try
    End Sub

    ' ── Helpers ───────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Pastikan index ada di kolom WHERE sebelum batch delete.
    ''' Cek apakah kolom sudah punya index (prefix match) — jika sudah, skip.
    ''' Jika belum, CREATE INDEX dengan nama idx_hapus_kolom>.
    ''' </summary>
    Private Sub EnsureIndex(cmd As MySqlCommand, tabel As String, kolom As String)
        Try
            ' Cek apakah kolom sudah punya index (sebagai kolom pertama di index manapun)
            cmd.CommandText = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS " &
                              $"WHERE TABLE_SCHEMA = DATABASE() " &
                              $"AND TABLE_NAME = '{tabel}' " &
                              $"AND COLUMN_NAME = '{kolom}' " &
                              $"AND SEQ_IN_INDEX = 1"
            Dim ada As Long = Convert.ToInt64(cmd.ExecuteScalar())
            If ada > 0 Then
                ' Sudah ada index yang dimulai dengan kolom ini — skip
                Return
            End If

            ' Belum ada — buat index baru
            Dim idxName As String = $"idx_hapus_{kolom.ToLower()}"
            cmd.CommandText = $"ALTER TABLE `{tabel}` ADD INDEX `{idxName}` (`{kolom}`)"
            cmd.ExecuteNonQuery()
            Log($"   🔧 Index {idxName} dibuat di {tabel}.{kolom}")
        Catch ex As Exception
            Log($"   ⚠️  EnsureIndex {tabel}.{kolom}: {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Batch DELETE dengan LIMIT 1000 per iterasi — hindari lock tabel lama.
    ''' Loop sampai tidak ada baris tersisa.
    ''' </summary>
    Private Sub HapusWhere(cmd As MySqlCommand, tabel As String, kolom As String, nilai As String,
                           Optional batchSize As Integer = 1000)
        Try
            EnsureIndex(cmd, tabel, kolom)

            Dim totalRows As Long = 0
            Dim batch As Integer
            Do
                cmd.CommandText = $"DELETE FROM `{tabel}` WHERE `{kolom}` = '{nilai}' LIMIT {batchSize}"
                batch = cmd.ExecuteNonQuery()
                totalRows += batch
                If batch > 0 Then Application.DoEvents()
            Loop While batch = batchSize   ' jika < batchSize berarti sudah habis

            If totalRows > 0 Then
                Log($"   ✅ {tabel} → {totalRows} baris dihapus")
            Else
                Log($"   ○  {tabel} — kosong")
            End If
        Catch ex As Exception
            Log($"   ⚠️  {tabel}: {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Batch DELETE orphan via LEFT JOIN — loop sampai habis.
    ''' </summary>
    Private Function HapusOrphan(cmd As MySqlCommand,
                                  sqlDelete As String,
                                  label As String,
                                  Optional batchSize As Integer = 1000) As Long
        Dim totalRows As Long = 0
        Try
            ' Sisipkan LIMIT ke query — tambahkan di akhir sebelum titik koma
            Dim sqlBatch As String = sqlDelete.TrimEnd() & $" LIMIT {batchSize}"
            Dim batch As Integer
            Do
                cmd.CommandText = sqlBatch
                batch = cmd.ExecuteNonQuery()
                totalRows += batch
                If batch > 0 Then Application.DoEvents()
            Loop While batch = batchSize
            Log($"   ✅ {label} (orphan) → {totalRows} baris")
        Catch ex As Exception
            Log($"   ⚠️  {label}: {ex.Message}")
        End Try
        Return totalRows
    End Function

    Private Sub TruncateTabel(cmd As MySqlCommand, tabel As String)
        Try
            cmd.CommandText = $"TRUNCATE `{tabel}`"
            cmd.ExecuteNonQuery()
            Log($"   ✅ TRUNCATE {tabel}")
        Catch ex As Exception
            Log($"   ⚠️  {tabel}: {ex.Message}")
        End Try
    End Sub

End Class
