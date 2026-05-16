''' <summary>
''' ModuleLaporanKalkulasi - Module untuk Kalkulasi Laporan Keuangan
''' Fungsi: Memisahkan perhitungan laporan dari state transaksi nyata (tbl_datareferensi)
''' dan menyediakan fungsi kalkulasi untuk laporan periode dan posting resmi.
''' </summary>
''' <remarks>
''' Digunakan untuk Requirement 17: Pemisahan Kalkulasi Laporan dari State Transaksi
''' </remarks>
Module ModuleLaporanKalkulasi

    ''' <summary>
    ''' POSTING RESMI: Hitung semua saldo akun dan simpan hasilnya ke tbl_datareferensi (data asli).
    ''' Fungsi ini dipanggil saat user memilih mode "Semua" di laporan neraca/laba rugi,
    ''' atau saat melakukan posting resmi akhir periode.
    '''
    ''' Urutan langkah:
    ''' 1. Reset SALDO_SEBELUMNYA ke SALDO_AWAL untuk semua akun
    ''' 2. Hitung total mutasi DEBET dan KREDIT dari JurnalUmum untuk semua akun
    ''' 3. Hitung SALDO_AKHIR semua akun neraca (AKTIVA, PASIVA, MODAL) — kecuali akun LABA RUGI BERJALAN
    ''' 4. Hitung nilai laba/rugi bersih dari akun-akun pendapatan dan beban (SUB_AKUN = LABA atau RUGI)
    ''' 5. Simpan hasil laba/rugi ke akun LABA RUGI BERJALAN (05.01.001)
    '''
    ''' Catatan penting akun LABA RUGI BERJALAN (TYPE_AKUN = 'LABA RUGI'):
    ''' - SALDO_SEBELUMNYA = SALDO_AWAL (tidak pernah diubah oleh fungsi ini)
    ''' - S_DEBET  = total semua beban periode (akun SUB_AKUN='RUGI' + akun SUB_AKUN='LABA' sisi debet)
    ''' - S_KREDIT = total semua pendapatan periode (akun SUB_AKUN='LABA' sisi kredit)
    ''' - SALDO_AKHIR = SALDO_SEBELUMNYA - S_DEBET + S_KREDIT
    '''                (rumus akun KREDIT — konsisten dengan LANGKAH 3)
    ''' </summary>
    Public Sub PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()
        EnsureConnectionReady()

        ' ── CEK: Skip jika tidak ada jurnal baru sejak terakhir posting ──────────────
        Dim lastJurnalStr As String = ""
        Using cmdCekJurnal As New MySqlCommand(
            "SELECT COALESCE(MAX(updated_at), '1970-01-01') FROM JurnalUmum", conn)
            lastJurnalStr = cmdCekJurnal.ExecuteScalar().ToString()
        End Using

        Dim lastPostingStr As String = AppConfig.Instance.GetValue(Of String)("LastPostingJurnal", "")
        If lastJurnalStr = lastPostingStr AndAlso lastJurnalStr <> "" Then
            Exit Sub
        End If

        ' ── LANGKAH 1 ────────────────────────────────────────────────────────────────
        Using cmdResetSaldoAwal As New MySqlCommand(
            "UPDATE tbl_datareferensi SET SALDO_SEBELUMNYA = SALDO_AWAL", conn)
            cmdResetSaldoAwal.ExecuteNonQuery()
        End Using

        ' ── LANGKAH 2 ────────────────────────────────────────────────────────────────
        Using cmdHitungMutasiJurnal As New MySqlCommand(
            "UPDATE tbl_datareferensi AS akun " &
            "LEFT JOIN ( " &
            "    SELECT KODE_AKUN, " &
            "        SUM(CASE WHEN tipe = 'D' THEN NOMINAL ELSE 0 END) AS TOTAL_MUTASI_DEBET, " &
            "        SUM(CASE WHEN tipe = 'K' THEN NOMINAL ELSE 0 END) AS TOTAL_MUTASI_KREDIT " &
            "    FROM ( " &
            "        SELECT NOMOR_AKUN_D AS KODE_AKUN, 'D' AS tipe, NOMINAL FROM JurnalUmum WHERE NOMOR_AKUN_D <> '' " &
            "        UNION ALL " &
            "        SELECT NOMOR_AKUN_K, 'K', NOMINAL FROM JurnalUmum WHERE NOMOR_AKUN_K <> '' " &
            "    ) x " &
            "    GROUP BY KODE_AKUN " &
            ") mutasi ON mutasi.KODE_AKUN = akun.KODE_AKUN " &
            "SET " &
            "    akun.S_DEBET  = COALESCE(mutasi.TOTAL_MUTASI_DEBET,  0), " &
            "    akun.S_KREDIT = COALESCE(mutasi.TOTAL_MUTASI_KREDIT, 0)",
            conn)
            cmdHitungMutasiJurnal.ExecuteNonQuery()
        End Using

        ' ── LANGKAH 3 ────────────────────────────────────────────────────────────────
        Using cmdHitungSaldoAkhirNeraca As New MySqlCommand(
            "UPDATE tbl_datareferensi " &
            "SET SALDO_AKHIR = CASE " &
            "    WHEN AKUN_DK = 'DEBET'  THEN SALDO_SEBELUMNYA + S_DEBET - S_KREDIT " &
            "    WHEN AKUN_DK = 'KREDIT' THEN SALDO_SEBELUMNYA - S_DEBET + S_KREDIT " &
            "    ELSE 0 " &
            "END " &
            "WHERE TYPE_AKUN <> 'LABA RUGI'",
            conn)
            cmdHitungSaldoAkhirNeraca.ExecuteNonQuery()
        End Using

        ' ── LANGKAH 4 ────────────────────────────────────────────────────────────────
        Dim labaBersihPeriode As Decimal = 0D
        Dim totalBebanPeriode As Decimal = 0D
        Dim totalPendapatanPeriode As Decimal = 0D
        Using cmdHitungLabaRugi As New MySqlCommand(
            "SELECT " &
            "    SUM(CASE WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'KREDIT' THEN SALDO_AKHIR ELSE 0 END) " &
            "  - SUM(CASE WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'DEBET'  THEN SALDO_AKHIR ELSE 0 END) " &
            "  - SUM(CASE WHEN SUB_AKUN = 'RUGI' AND AKUN_DK = 'DEBET'  THEN SALDO_AKHIR ELSE 0 END) " &
            "  + SUM(CASE WHEN SUB_AKUN = 'RUGI' AND AKUN_DK = 'KREDIT' THEN SALDO_AKHIR ELSE 0 END) " &
            "    AS LABA_BERSIH_PERIODE, " &
            "    SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_DEBET  ELSE 0 END) AS TOTAL_BEBAN_PERIODE, " &
            "    SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_KREDIT ELSE 0 END) AS TOTAL_PENDAPATAN_PERIODE " &
            "FROM tbl_datareferensi WHERE SUB_AKUN IN ('LABA', 'RUGI')",
            conn)
            Using pembacaHasilLabaRugi As MySqlDataReader = cmdHitungLabaRugi.ExecuteReader()
                If pembacaHasilLabaRugi.Read() Then
                    labaBersihPeriode      = ModuleAngka.SafeGetValue(Of Decimal)(pembacaHasilLabaRugi, "LABA_BERSIH_PERIODE", 0D)
                    totalBebanPeriode      = ModuleAngka.SafeGetValue(Of Decimal)(pembacaHasilLabaRugi, "TOTAL_BEBAN_PERIODE", 0D)
                    totalPendapatanPeriode = ModuleAngka.SafeGetValue(Of Decimal)(pembacaHasilLabaRugi, "TOTAL_PENDAPATAN_PERIODE", 0D)
                End If
            End Using
        End Using

        ' ── LANGKAH 5 ────────────────────────────────────────────────────────────────
        Using cmdSimpanLabaRugiBerjalan As New MySqlCommand(
            "UPDATE tbl_datareferensi " &
            "SET S_DEBET = @totalBebanPeriode, S_KREDIT = @totalPendapatanPeriode, " &
            "    SALDO_AKHIR = SALDO_SEBELUMNYA - @totalBebanPeriode2 + @totalPendapatanPeriode2 " &
            "WHERE TYPE_AKUN = 'LABA RUGI'",
            conn)
            cmdSimpanLabaRugiBerjalan.Parameters.AddWithValue("@totalBebanPeriode", totalBebanPeriode)
            cmdSimpanLabaRugiBerjalan.Parameters.AddWithValue("@totalPendapatanPeriode", totalPendapatanPeriode)
            cmdSimpanLabaRugiBerjalan.Parameters.AddWithValue("@totalBebanPeriode2", totalBebanPeriode)
            cmdSimpanLabaRugiBerjalan.Parameters.AddWithValue("@totalPendapatanPeriode2", totalPendapatanPeriode)
            cmdSimpanLabaRugiBerjalan.ExecuteNonQuery()
        End Using

        AppConfig.Instance.SetValue("LastPostingJurnal", lastJurnalStr)
        AppConfig.Instance.Save()
    End Sub

    ''' <summary>
    ''' Siapkan temp_datareferensi sebagai ruang kerja kalkulasi laporan periode.
    ''' Fungsi ini WAJIB dipanggil sebelum HitungSaldoAwal, HitungDebetKredit, dan HitungSaldoAkhir.
    '''
    ''' Tujuan: semua kalkulasi laporan dilakukan di temp_datareferensi (bukan tbl_datareferensi)
    ''' sehingga data asli tidak berubah saat user membuka laporan periode.
    ''' </summary>
    Public Sub SiapkanTempDatareferensi_SalinDariTblDatareferensi()
        EnsureConnectionReady()

        Using cmdKosongkan As New MySqlCommand("TRUNCATE TABLE temp_datareferensi", conn)
            cmdKosongkan.ExecuteNonQuery()
        End Using

        Using cmdSalinMasterAkun As New MySqlCommand(
            "INSERT INTO temp_datareferensi " &
            "    (STATUS, JENIS_AKUN, TYPE_AKUN, KODE_AKUN, NAMA_AKUN, SUB_AKUN, AKUN_DK, AKUN_NRLR, " &
            "     SALDO_AWAL, SALDO_SEBELUMNYA, S_DEBET, S_KREDIT, SALDO_AKHIR) " &
            "SELECT " &
            "    STATUS, JENIS_AKUN, TYPE_AKUN, KODE_AKUN, NAMA_AKUN, SUB_AKUN, AKUN_DK, AKUN_NRLR, " &
            "    SALDO_AWAL, SALDO_SEBELUMNYA, S_DEBET, S_KREDIT, SALDO_AKHIR " &
            "FROM tbl_datareferensi",
            conn)
            cmdSalinMasterAkun.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' Hitung SALDO_SEBELUMNYA di temp_datareferensi untuk laporan periode.
    ''' SALDO_SEBELUMNYA = saldo kumulatif semua jurnal SEBELUM tanggalAwalPeriode.
    ''' Ini yang ditampilkan sebagai kolom "Periode Lalu / Bulan Lalu / Saldo Awal" di laporan.
    '''
    ''' Dipanggil setelah SiapkanTempDatareferensi, sebelum HitungDebetKredit.
    ''' Menulis ke temp_datareferensi — tidak mengubah tbl_datareferensi.
    ''' </summary>
    ''' <param name="tanggalAwalPeriode">Tanggal awal periode laporan (eksklusif — jurnal sebelum tanggal ini)</param>
    Public Sub HitungSaldoAwal_PeriodeLaporan_KeTempDatareferensi(ByVal tanggalAwalPeriode As Date)
        EnsureConnectionReady()

        ' ── LANGKAH 1 ────────────────────────────────────────────────────────────────
        Using cmdHitungSaldoAwalNeraca As New MySqlCommand(
            "UPDATE temp_datareferensi AS akun " &
            "LEFT JOIN ( " &
            "    SELECT NOMOR_AKUN_D AS KODE_AKUN, SUM(NOMINAL) AS TOTAL_MUTASI_DEBET " &
            "    FROM JurnalUmum " &
            "    WHERE TGL_TRANSAKSI < @tanggalAwalPeriode " &
            "      AND NOMOR_AKUN_D <> '' " &
            "    GROUP BY NOMOR_AKUN_D " &
            ") mutasi_debet ON mutasi_debet.KODE_AKUN = akun.KODE_AKUN " &
            "LEFT JOIN ( " &
            "    SELECT NOMOR_AKUN_K AS KODE_AKUN, SUM(NOMINAL) AS TOTAL_MUTASI_KREDIT " &
            "    FROM JurnalUmum " &
            "    WHERE TGL_TRANSAKSI < @tanggalAwalPeriode2 " &
            "      AND NOMOR_AKUN_K <> '' " &
            "    GROUP BY NOMOR_AKUN_K " &
            ") mutasi_kredit ON mutasi_kredit.KODE_AKUN = akun.KODE_AKUN " &
            "SET akun.SALDO_SEBELUMNYA = CASE " &
            "    WHEN akun.AKUN_DK = 'DEBET'  THEN akun.SALDO_AWAL + COALESCE(mutasi_debet.TOTAL_MUTASI_DEBET, 0) - COALESCE(mutasi_kredit.TOTAL_MUTASI_KREDIT, 0) " &
            "    WHEN akun.AKUN_DK = 'KREDIT' THEN akun.SALDO_AWAL - COALESCE(mutasi_debet.TOTAL_MUTASI_DEBET, 0) + COALESCE(mutasi_kredit.TOTAL_MUTASI_KREDIT, 0) " &
            "    ELSE 0 " &
            "END " &
            "WHERE akun.TYPE_AKUN <> 'LABA RUGI'",
            conn)
            cmdHitungSaldoAwalNeraca.Parameters.AddWithValue("@tanggalAwalPeriode", tanggalAwalPeriode.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungSaldoAwalNeraca.Parameters.AddWithValue("@tanggalAwalPeriode2", tanggalAwalPeriode.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungSaldoAwalNeraca.ExecuteNonQuery()
        End Using

        ' ── LANGKAH 2 ────────────────────────────────────────────────────────────────
        Dim labaBersihSebelumPeriode As Decimal = 0D
        Using cmdHitungLabaSebelumPeriode As New MySqlCommand(
            "SELECT " &
            "    SUM(CASE WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'KREDIT' THEN SALDO_SEBELUMNYA ELSE 0 END) " &
            "  - SUM(CASE WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'DEBET'  THEN SALDO_SEBELUMNYA ELSE 0 END) " &
            "  - SUM(CASE WHEN SUB_AKUN = 'RUGI' AND AKUN_DK = 'DEBET'  THEN SALDO_SEBELUMNYA ELSE 0 END) " &
            "  + SUM(CASE WHEN SUB_AKUN = 'RUGI' AND AKUN_DK = 'KREDIT' THEN SALDO_SEBELUMNYA ELSE 0 END) " &
            "    AS LABA_BERSIH_SEBELUM_PERIODE " &
            "FROM temp_datareferensi WHERE SUB_AKUN IN ('LABA', 'RUGI')",
            conn)
            Dim hasilSkalar As Object = cmdHitungLabaSebelumPeriode.ExecuteScalar()
            labaBersihSebelumPeriode = If(hasilSkalar Is DBNull.Value OrElse hasilSkalar Is Nothing, 0D, Convert.ToDecimal(hasilSkalar))
        End Using
        Using cmdSimpanSaldoAwalLabaRugi As New MySqlCommand(
            "UPDATE temp_datareferensi SET SALDO_SEBELUMNYA = @labaBersihSebelumPeriode WHERE TYPE_AKUN = 'LABA RUGI'",
            conn)
            cmdSimpanSaldoAwalLabaRugi.Parameters.AddWithValue("@labaBersihSebelumPeriode", labaBersihSebelumPeriode)
            cmdSimpanSaldoAwalLabaRugi.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' Hitung S_DEBET dan S_KREDIT di temp_datareferensi untuk laporan periode.
    ''' S_DEBET/S_KREDIT = total mutasi jurnal dalam rentang tanggalAwalPeriode s/d tanggalAkhirPeriode.
    ''' Ini yang ditampilkan sebagai kolom "Periode Ini / Bulan Ini / Tanggal Ini" di laporan.
    '''
    ''' Dipanggil setelah HitungSaldoAwal, sebelum HitungSaldoAkhir.
    ''' Menulis ke temp_datareferensi — tidak mengubah tbl_datareferensi.
    ''' </summary>
    ''' <param name="tanggalAwalPeriode">Tanggal awal periode (inklusif)</param>
    ''' <param name="tanggalAkhirPeriode">Tanggal akhir periode (inklusif)</param>
    Public Sub HitungDebetKredit_PeriodeLaporan_KeTempDatareferensi(ByVal tanggalAwalPeriode As Date, ByVal tanggalAkhirPeriode As Date)
        EnsureConnectionReady()

        ' ── LANGKAH 1 ────────────────────────────────────────────────────────────────
        Using cmdHitungMutasiPeriode As New MySqlCommand(
            "UPDATE temp_datareferensi AS akun " &
            "LEFT JOIN ( " &
            "    SELECT NOMOR_AKUN_D AS KODE_AKUN, SUM(NOMINAL) AS TOTAL_MUTASI_DEBET " &
            "    FROM JurnalUmum " &
            "    WHERE TGL_TRANSAKSI >= @tanggalAwalPeriode " &
            "      AND TGL_TRANSAKSI <= @tanggalAkhirPeriode " &
            "      AND NOMOR_AKUN_D <> '' " &
            "    GROUP BY NOMOR_AKUN_D " &
            ") mutasi_debet ON mutasi_debet.KODE_AKUN = akun.KODE_AKUN " &
            "LEFT JOIN ( " &
            "    SELECT NOMOR_AKUN_K AS KODE_AKUN, SUM(NOMINAL) AS TOTAL_MUTASI_KREDIT " &
            "    FROM JurnalUmum " &
            "    WHERE TGL_TRANSAKSI >= @tanggalAwalPeriode2 " &
            "      AND TGL_TRANSAKSI <= @tanggalAkhirPeriode2 " &
            "      AND NOMOR_AKUN_K <> '' " &
            "    GROUP BY NOMOR_AKUN_K " &
            ") mutasi_kredit ON mutasi_kredit.KODE_AKUN = akun.KODE_AKUN " &
            "SET " &
            "    akun.S_DEBET  = COALESCE(mutasi_debet.TOTAL_MUTASI_DEBET,   0), " &
            "    akun.S_KREDIT = COALESCE(mutasi_kredit.TOTAL_MUTASI_KREDIT, 0) " &
            "WHERE akun.TYPE_AKUN <> 'LABA RUGI'",
            conn)
            cmdHitungMutasiPeriode.Parameters.AddWithValue("@tanggalAwalPeriode", tanggalAwalPeriode.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungMutasiPeriode.Parameters.AddWithValue("@tanggalAkhirPeriode", tanggalAkhirPeriode.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungMutasiPeriode.Parameters.AddWithValue("@tanggalAwalPeriode2", tanggalAwalPeriode.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungMutasiPeriode.Parameters.AddWithValue("@tanggalAkhirPeriode2", tanggalAkhirPeriode.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungMutasiPeriode.ExecuteNonQuery()
        End Using

        ' ── LANGKAH 2 ────────────────────────────────────────────────────────────────
        Dim totalBebanPeriode As Decimal = 0D
        Dim totalPendapatanPeriode As Decimal = 0D
        Using cmdHitungMutasiLabaRugi As New MySqlCommand(
            "SELECT " &
            "    SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_DEBET  ELSE 0 END) AS TOTAL_BEBAN_PERIODE, " &
            "    SUM(CASE WHEN SUB_AKUN IN ('LABA','RUGI') THEN S_KREDIT ELSE 0 END) AS TOTAL_PENDAPATAN_PERIODE " &
            "FROM temp_datareferensi WHERE SUB_AKUN IN ('LABA', 'RUGI')",
            conn)
            Using pembacaHasil As MySqlDataReader = cmdHitungMutasiLabaRugi.ExecuteReader()
                If pembacaHasil.Read() Then
                    totalBebanPeriode      = ModuleAngka.SafeGetValue(Of Decimal)(pembacaHasil, "TOTAL_BEBAN_PERIODE", 0D)
                    totalPendapatanPeriode = ModuleAngka.SafeGetValue(Of Decimal)(pembacaHasil, "TOTAL_PENDAPATAN_PERIODE", 0D)
                End If
            End Using
        End Using
        Using cmdSimpanMutasiLabaRugi As New MySqlCommand(
            "UPDATE temp_datareferensi " &
            "SET S_DEBET = @totalBebanPeriode, S_KREDIT = @totalPendapatanPeriode " &
            "WHERE TYPE_AKUN = 'LABA RUGI'",
            conn)
            cmdSimpanMutasiLabaRugi.Parameters.AddWithValue("@totalBebanPeriode", totalBebanPeriode)
            cmdSimpanMutasiLabaRugi.Parameters.AddWithValue("@totalPendapatanPeriode", totalPendapatanPeriode)
            cmdSimpanMutasiLabaRugi.ExecuteNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' Hitung SALDO_AKHIR di temp_datareferensi untuk laporan periode.
    ''' SALDO_AKHIR = saldo kumulatif semua jurnal dari awal hingga tanggalAkhirPeriode.
    ''' Ini yang ditampilkan sebagai kolom "Saldo Akhir" di laporan neraca dan laba rugi.
    '''
    ''' Dipanggil setelah HitungDebetKredit.
    ''' Menulis ke temp_datareferensi — tidak mengubah tbl_datareferensi.
    ''' </summary>
    ''' <param name="tanggalAkhirPeriode">Tanggal akhir periode laporan (inklusif)</param>
    Public Sub HitungSaldoAkhir_PeriodeLaporan_KeTempDatareferensi(ByVal tanggalAkhirPeriode As Date)
        EnsureConnectionReady()

        ' ── LANGKAH 1 ────────────────────────────────────────────────────────────────
        Using cmdHitungSaldoAkhirNeraca As New MySqlCommand(
            "UPDATE temp_datareferensi AS akun " &
            "LEFT JOIN ( " &
            "    SELECT NOMOR_AKUN_D AS KODE_AKUN, SUM(NOMINAL) AS TOTAL_MUTASI_DEBET " &
            "    FROM JurnalUmum " &
            "    WHERE TGL_TRANSAKSI <= @tanggalAkhirPeriode " &
            "      AND NOMOR_AKUN_D <> '' " &
            "    GROUP BY NOMOR_AKUN_D " &
            ") mutasi_debet ON mutasi_debet.KODE_AKUN = akun.KODE_AKUN " &
            "LEFT JOIN ( " &
            "    SELECT NOMOR_AKUN_K AS KODE_AKUN, SUM(NOMINAL) AS TOTAL_MUTASI_KREDIT " &
            "    FROM JurnalUmum " &
            "    WHERE TGL_TRANSAKSI <= @tanggalAkhirPeriode2 " &
            "      AND NOMOR_AKUN_K <> '' " &
            "    GROUP BY NOMOR_AKUN_K " &
            ") mutasi_kredit ON mutasi_kredit.KODE_AKUN = akun.KODE_AKUN " &
            "SET akun.SALDO_AKHIR = CASE " &
            "    WHEN akun.AKUN_DK = 'DEBET'  THEN akun.SALDO_AWAL + COALESCE(mutasi_debet.TOTAL_MUTASI_DEBET, 0) - COALESCE(mutasi_kredit.TOTAL_MUTASI_KREDIT, 0) " &
            "    WHEN akun.AKUN_DK = 'KREDIT' THEN akun.SALDO_AWAL - COALESCE(mutasi_debet.TOTAL_MUTASI_DEBET, 0) + COALESCE(mutasi_kredit.TOTAL_MUTASI_KREDIT, 0) " &
            "    ELSE 0 " &
            "END " &
            "WHERE akun.TYPE_AKUN <> 'LABA RUGI'",
            conn)
            cmdHitungSaldoAkhirNeraca.Parameters.AddWithValue("@tanggalAkhirPeriode", tanggalAkhirPeriode.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungSaldoAkhirNeraca.Parameters.AddWithValue("@tanggalAkhirPeriode2", tanggalAkhirPeriode.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungSaldoAkhirNeraca.ExecuteNonQuery()
        End Using

        ' ── LANGKAH 2 ────────────────────────────────────────────────────────────────
        Dim labaBersihKumulatif As Decimal = 0D
        Using cmdHitungLabaKumulatif As New MySqlCommand(
            "SELECT " &
            "    SUM(CASE WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'KREDIT' THEN SALDO_AKHIR ELSE 0 END) " &
            "  - SUM(CASE WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'DEBET'  THEN SALDO_AKHIR ELSE 0 END) " &
            "  - SUM(CASE WHEN SUB_AKUN = 'RUGI' AND AKUN_DK = 'DEBET'  THEN SALDO_AKHIR ELSE 0 END) " &
            "  + SUM(CASE WHEN SUB_AKUN = 'RUGI' AND AKUN_DK = 'KREDIT' THEN SALDO_AKHIR ELSE 0 END) " &
            "    AS LABA_BERSIH_KUMULATIF " &
            "FROM temp_datareferensi WHERE SUB_AKUN IN ('LABA', 'RUGI')",
            conn)
            Dim hasilSkalar As Object = cmdHitungLabaKumulatif.ExecuteScalar()
            labaBersihKumulatif = If(hasilSkalar Is DBNull.Value OrElse hasilSkalar Is Nothing, 0D, Convert.ToDecimal(hasilSkalar))
        End Using
        Using cmdSimpanSaldoAkhirLabaRugi As New MySqlCommand(
            "UPDATE temp_datareferensi SET SALDO_AKHIR = @labaBersihKumulatif WHERE TYPE_AKUN = 'LABA RUGI'",
            conn)
            cmdSimpanSaldoAkhirLabaRugi.Parameters.AddWithValue("@labaBersihKumulatif", labaBersihKumulatif)
            cmdSimpanSaldoAkhirLabaRugi.ExecuteNonQuery()
        End Using
    End Sub

End Module