Imports System.Globalization
Imports System.Text

''' <summary>
''' Modul dashboard FormUtama — menampilkan KPI ringkasan bisnis per lokasi.
''' Data difilter berdasarkan StatusLokasi.Text (TOKO / GUDANG).
''' Laba tidak ditampilkan — terlalu sensitif untuk layar utama.
''' </summary>
Module ModuleDashboard

    Private ReadOnly _id As New CultureInfo("id-ID")

    ''' <summary>
    ''' Bangun HTML dashboard berdasarkan lokasi aktif.
    ''' </summary>
    Public Function BangunHTML(Optional lokasi As String = "TOKO") As String
        lokasi = lokasi.Trim().ToUpper()
        Dim isGudang As Boolean = (lokasi = "GUDANG")
        Dim isDark As Boolean = ModuleTheme.IsDarkMode

        ' ── Warna tema ───────────────────────────────────────────────
        Dim cBg As String = ColorToHex(If(isDark, D_Bg, L_Bg))
        Dim cSurface As String = ColorToHex(If(isDark, D_Surface, L_Surface))
        Dim cText As String = ColorToHex(If(isDark, D_Text, L_Text))
        Dim cMuted As String = ColorToHex(If(isDark, D_Muted, L_Muted))
        Dim cBorder As String = ColorToHex(If(isDark, D_Border, L_Border))
        Dim cSubtle As String = ColorToHex(If(isDark, D_Subtle, L_Subtle))
        Dim cSuccess As String = ColorToHex(If(isDark, D_Success, L_Success))
        Dim cWarning As String = ColorToHex(If(isDark, D_Warning, L_Warning))
        Dim cDanger As String = ColorToHex(If(isDark, D_Danger, L_Danger))
        Dim cPrimary As String = ColorToHex(If(isGudang, If(isDark, D_Success, L_Success), If(isDark, D_Primary, L_Primary)))
        Dim cSecondary As String = ColorToHex(If(isDark, D_Secondary, L_Secondary))
        Dim labelLokasi As String = If(isGudang, "GUDANG", "TOKO")
        Dim kolStok As String = If(isGudang, "STOK_GUDANG", "STOK_TOKO")

        ' ── Deklarasi semua variabel data ────────────────────────────
        Dim omsetHariIni As Decimal = 0D
        Dim jmlTransaksi As Integer = 0
        Dim nilaiRataTransaksi As Decimal = 0D
        Dim omsetBulanIni As Decimal = 0D
        Dim omsetBulanLalu As Decimal = 0D
        Dim totalPembelianBulan As Decimal = 0D
        Dim stokKritis As Integer = 0
        Dim stokHabis As Integer = 0
        Dim stokAman As Integer = 0
        Dim nilaiPersediaan As Decimal = 0D
        Dim hutangJatuhTempo As Integer = 0
        Dim totalHutang As Decimal = 0D
        Dim piutangBelumLunas As Integer = 0
        Dim totalPiutang As Decimal = 0D
        Dim omsetLogin As Decimal = 0D
        Dim jmlTransaksiLogin As Integer = 0
        Dim omsetBulanIniUser As Decimal = 0D
        Dim omsetBulanLaluUser As Decimal = 0D

        ' ── Array 12 bulan ───────────────────────────────────────────
        Dim omset12Bulan(11) As Decimal
        Dim transaksi12Bulan(11) As Integer
        Dim omsetLogin12Bulan(11) As Decimal
        Dim label12Bulan(11) As String
        For i As Integer = 0 To 11
            Dim tgl As Date = DateSerial(Now.Year, Now.Month - 11 + i, 1)
            label12Bulan(i) = tgl.ToString("MMM yyyy", _id)
        Next

        ' ── Satu Try untuk semua query ───────────────────────────────
        Try
            EnsureConnectionReady()

            Dim tglHariIni As Date = Now.Date
            Dim tglBesok As Date = tglHariIni.AddDays(1)
            Dim tglAkhirHari As Date = tglHariIni.AddDays(1).AddTicks(-1) ' 23:59:59 hari ini
            Dim tglBulanAwal As Date = New Date(Now.Year, Now.Month, 1)
            Dim tglBulanAkhir As Date = tglBulanAwal.AddMonths(1)
            Dim tglBulanLaluAwal As Date = tglBulanAwal.AddMonths(-1)
            Dim tglBulanLaluAkhir As Date = tglBulanAwal
            Dim tgl12BulanLalu As Date = DateSerial(Now.Year, Now.Month - 11, 1)

            ' ── QUERY 1: agregasi penjualan global (omset saja) ───────
            Using cmd As New MySqlCommand(
                "SELECT " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @hariIni AND TGL_TRANSAKSI < @besok THEN GRAND_TOTAL_STL_PAJAK ELSE 0 END),0) AS OMSET_HARI, " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @hariIni AND TGL_TRANSAKSI < @besok THEN 1 ELSE 0 END),0) AS JML_HARI, " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @bulanAwal AND TGL_TRANSAKSI < @bulanAkhir THEN GRAND_TOTAL_STL_PAJAK ELSE 0 END),0) AS OMSET_BULAN, " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @bulanLaluAwal AND TGL_TRANSAKSI < @bulanLaluAkhir THEN GRAND_TOTAL_STL_PAJAK ELSE 0 END),0) AS OMSET_LALU " &
                "FROM penjualan " &
                "WHERE LOKASIBARANG = @lok AND TGL_TRANSAKSI >= @bulanLaluAwal", conn)
                cmd.Parameters.AddWithValue("@hariIni",        tglHariIni)
                cmd.Parameters.AddWithValue("@besok",          tglBesok)
                cmd.Parameters.AddWithValue("@bulanAwal",      tglBulanAwal)
                cmd.Parameters.AddWithValue("@bulanAkhir",     tglBulanAkhir)
                cmd.Parameters.AddWithValue("@bulanLaluAwal",  tglBulanLaluAwal)
                cmd.Parameters.AddWithValue("@bulanLaluAkhir", tglBulanLaluAkhir)
                cmd.Parameters.AddWithValue("@lok",            lokasi)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        omsetHariIni  = ModuleAngka.ParseDecimal(rd("OMSET_HARI"))
                        jmlTransaksi  = ModuleAngka.ParseInteger(rd("JML_HARI"))
                        omsetBulanIni = ModuleAngka.ParseDecimal(rd("OMSET_BULAN"))
                        omsetBulanLalu = ModuleAngka.ParseDecimal(rd("OMSET_LALU"))
                    End If
                End Using
            End Using
            nilaiRataTransaksi = If(jmlTransaksi > 0, omsetHariIni / jmlTransaksi, 0D)

            ' ── QUERY PIUTANG: tanpa filter lokasi & tanggal transaksi
            ' Identik NotifikasiJatuhTempo — hanya STATUS_TRANSAKSI + JATUH_TEMPO
            Using cmd As New MySqlCommand(
                "SELECT " &
                "  COALESCE(SUM(SISA_TAGIHAN),0) AS TOTAL_PIUTANG, " &
                "  COUNT(*) AS JML_PIUTANG " &
                "FROM penjualan " &
                "WHERE STATUS_TRANSAKSI = 'Belum Lunas' AND JATUH_TEMPO <= @akhirHari", conn)
                cmd.Parameters.AddWithValue("@akhirHari", tglAkhirHari)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        totalPiutang      = ModuleAngka.ParseDecimal(rd("TOTAL_PIUTANG"))
                        piutangBelumLunas = ModuleAngka.ParseInteger(rd("JML_PIUTANG"))
                    End If
                End Using
            End Using

            ' ── QUERY 2: pembelian (omset) + hutang terpisah ─────────
            Using cmd As New MySqlCommand(
                "SELECT " &
                "  COALESCE(SUM(CASE WHEN TGL_BELI >= @bulanAwal AND TGL_BELI < @bulanAkhir THEN GRAND_TOTAL_BELI ELSE 0 END),0) AS BELI_BULAN " &
                "FROM pembelian WHERE LOKASI = @lok", conn)
                cmd.Parameters.AddWithValue("@bulanAwal",  tglBulanAwal)
                cmd.Parameters.AddWithValue("@bulanAkhir", tglBulanAkhir)
                cmd.Parameters.AddWithValue("@lok",        lokasi)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        totalPembelianBulan = ModuleAngka.ParseDecimal(rd("BELI_BULAN"))
                    End If
                End Using
            End Using

            ' ── QUERY HUTANG: tanpa filter lokasi & tanggal beli
            ' Identik NotifikasiJatuhTempo — hanya STATUS_TRANSAKSI_BELI + JATUH_TEMPO
            Using cmd As New MySqlCommand(
                "SELECT " &
                "  COALESCE(SUM(TAGIHAN - NOMINALBAYAR),0) AS TOTAL_HUTANG, " &
                "  COUNT(*) AS JML_HUTANG " &
                "FROM pembelian " &
                "WHERE STATUS_TRANSAKSI_BELI = 'Belum Lunas' AND JATUH_TEMPO <= @akhirHari", conn)
                cmd.Parameters.AddWithValue("@akhirHari", tglAkhirHari)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        totalHutang      = ModuleAngka.ParseDecimal(rd("TOTAL_HUTANG"))
                        hutangJatuhTempo = ModuleAngka.ParseInteger(rd("JML_HUTANG"))
                    End If
                End Using
            End Using

            ' ── QUERY 3: status stok ─────────────────────────────────
            Using cmd As New MySqlCommand(
                $"SELECT " &
                $"  SUM(CASE WHEN {kolStok} <= 0 THEN 1 ELSE 0 END) AS HABIS, " &
                $"  SUM(CASE WHEN {kolStok} > 0 AND STOK_MIN > 0 AND {kolStok} <= STOK_MIN THEN 1 ELSE 0 END) AS KRITIS, " &
                $"  SUM(CASE WHEN {kolStok} > 0 AND ({kolStok} > STOK_MIN OR STOK_MIN = 0) THEN 1 ELSE 0 END) AS AMAN, " &
                $"  COALESCE(SUM(CASE WHEN {kolStok} > 0 THEN {kolStok} * HARGA_BELI ELSE 0 END),0) AS NILAI " &
                "FROM tbl_barang WHERE STATUS = 'Aktif'", conn)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        stokHabis = ModuleAngka.ParseInteger(rd("HABIS"))
                        stokKritis = ModuleAngka.ParseInteger(rd("KRITIS"))
                        stokAman = ModuleAngka.ParseInteger(rd("AMAN"))
                        nilaiPersediaan = ModuleAngka.ParseDecimal(rd("NILAI"))
                    End If
                End Using
            End Using

            ' ── QUERY 12 BULAN global: penjualan ─────────────────────
            Using cmd As New MySqlCommand(
                "SELECT DATE_FORMAT(TGL_TRANSAKSI,'%Y-%m') AS BLN, " &
                "  COALESCE(SUM(GRAND_TOTAL_STL_PAJAK),0) AS OMSET, COUNT(*) AS JML " &
                "FROM penjualan " &
                "WHERE TGL_TRANSAKSI >= @awal12 AND LOKASIBARANG = @lok " &
                "GROUP BY DATE_FORMAT(TGL_TRANSAKSI,'%Y-%m') " &
                "ORDER BY BLN", conn)
                cmd.Parameters.AddWithValue("@awal12", tgl12BulanLalu)
                cmd.Parameters.AddWithValue("@lok",    lokasi)
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim blnStr As String = rd("BLN").ToString()
                        Dim dtBln As Date = Date.Parse(blnStr & "-01")
                        Dim idx As Integer = ((dtBln.Year - tgl12BulanLalu.Year) * 12) + (dtBln.Month - tgl12BulanLalu.Month)
                        If idx >= 0 AndAlso idx <= 11 Then
                            omset12Bulan(idx)     = ModuleAngka.ParseDecimal(rd("OMSET"))
                            transaksi12Bulan(idx) = ModuleAngka.ParseInteger(rd("JML"))
                        End If
                    End While
                End Using
            End Using

            ' ── QUERY 4: identik Query 1, tambah filter ID_USER ──────
            ' Bedanya hanya AND ID_USER = @kodeUser — range waktu SAMA PERSIS
            Dim filterUser As String = If(String.IsNullOrEmpty(KodeUser), "", " AND ID_USER = @kodeUser")
            Using cmd As New MySqlCommand(
                "SELECT " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @hariIni AND TGL_TRANSAKSI < @besok THEN GRAND_TOTAL_STL_PAJAK ELSE 0 END),0) AS OMSET_HARI, " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @hariIni AND TGL_TRANSAKSI < @besok THEN 1 ELSE 0 END),0) AS JML_HARI, " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @bulanAwal AND TGL_TRANSAKSI < @bulanAkhir THEN GRAND_TOTAL_STL_PAJAK ELSE 0 END),0) AS OMSET_BULAN, " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @bulanLaluAwal AND TGL_TRANSAKSI < @bulanLaluAkhir THEN GRAND_TOTAL_STL_PAJAK ELSE 0 END),0) AS OMSET_LALU " &
                "FROM penjualan " &
                "WHERE LOKASIBARANG = @lok " &
                "  AND TGL_TRANSAKSI >= @bulanLaluAwal" & filterUser, conn)
                cmd.Parameters.AddWithValue("@hariIni", tglHariIni)
                cmd.Parameters.AddWithValue("@besok", tglBesok)
                cmd.Parameters.AddWithValue("@bulanAwal", tglBulanAwal)
                cmd.Parameters.AddWithValue("@bulanAkhir", tglBulanAkhir)
                cmd.Parameters.AddWithValue("@bulanLaluAwal", tglBulanLaluAwal)
                cmd.Parameters.AddWithValue("@bulanLaluAkhir", tglBulanLaluAkhir)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                If Not String.IsNullOrEmpty(KodeUser) Then
                    cmd.Parameters.AddWithValue("@kodeUser", KodeUser)
                End If
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        omsetLogin = ModuleAngka.ParseDecimal(rd("OMSET_HARI"))
                        jmlTransaksiLogin = ModuleAngka.ParseInteger(rd("JML_HARI"))
                        omsetBulanIniUser = ModuleAngka.ParseDecimal(rd("OMSET_BULAN"))
                        omsetBulanLaluUser = ModuleAngka.ParseDecimal(rd("OMSET_LALU"))
                    End If
                End Using
            End Using

            ' ── QUERY 12 BULAN per user: sparkline card sesi ─────────
            Using cmd As New MySqlCommand(
                "SELECT DATE_FORMAT(TGL_TRANSAKSI,'%Y-%m') AS BLN, COALESCE(SUM(GRAND_TOTAL_STL_PAJAK),0) AS OMSET " &
                "FROM penjualan " &
                "WHERE TGL_TRANSAKSI >= @awal12 AND LOKASIBARANG = @lok" & filterUser &
                " GROUP BY DATE_FORMAT(TGL_TRANSAKSI,'%Y-%m') " &
                "ORDER BY BLN", conn)
                cmd.Parameters.AddWithValue("@awal12", tgl12BulanLalu)
                cmd.Parameters.AddWithValue("@lok",    lokasi)
                If Not String.IsNullOrEmpty(KodeUser) Then
                    cmd.Parameters.AddWithValue("@kodeUser", KodeUser)
                End If
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim blnStr As String = rd("BLN").ToString()
                        Dim dtBln As Date = Date.Parse(blnStr & "-01")
                        Dim idx As Integer = ((dtBln.Year - tgl12BulanLalu.Year) * 12) + (dtBln.Month - tgl12BulanLalu.Month)
                        If idx >= 0 AndAlso idx <= 11 Then
                            omsetLogin12Bulan(idx) = ModuleAngka.ParseDecimal(rd("OMSET"))
                        End If
                    End While
                End Using
            End Using

        Catch
            ' Gagal query — tampilkan dashboard kosong tanpa crash
        End Try

        ' ── Hitung pertumbuhan omset global ──────────────────────────
        Dim tumbuh As Decimal = 0D
        Dim tumbuhStr As String = "-"
        Dim tumbuhCss As String = "netral"
        If omsetBulanLalu > 0 Then
            tumbuh = ((omsetBulanIni - omsetBulanLalu) / omsetBulanLalu) * 100
            tumbuhStr = (If(tumbuh >= 0, "+", "")) & tumbuh.ToString("0.0", _id) & "%"
            tumbuhCss = If(tumbuh >= 0, "naik", "turun")
        End If

        ' ── Hitung pertumbuhan omset per user ─────────────────────────
        Dim tumbuhUser As Decimal = 0D
        Dim tumbuhStrUser As String = "-"
        Dim tumbuhCssUser As String = "netral"
        Dim tumbuhIconUser As String = "fa-arrow-up"
        If omsetBulanLaluUser > 0 Then
            tumbuhUser = ((omsetBulanIniUser - omsetBulanLaluUser) / omsetBulanLaluUser) * 100
            tumbuhStrUser = (If(tumbuhUser >= 0, "+", "")) & tumbuhUser.ToString("0.0", _id) & "%"
            tumbuhCssUser = If(tumbuhUser >= 0, "naik", "turun")
            tumbuhIconUser = If(tumbuhUser >= 0, "fa-arrow-up", "fa-arrow-down")
        End If

        ' ── Baca template embedded resource ──────────────────────────
        Dim html As String = ""
        Try
            Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            Using stream = asm.GetManifestResourceStream("AppKasir.Resources.DashboardTemplate.html")
                If stream IsNot Nothing Then
                    Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
                        html = reader.ReadToEnd()
                    End Using
                End If
            End Using
        Catch
            Return "Error: Dashboard template embedded resource not found"
        End Try

        If String.IsNullOrEmpty(html) Then Return "Error: Dashboard template is empty"

        Dim tglStr As String = Now.ToString("dddd, dd MMMM yyyy", _id)

        ' ── Siapkan data JS ──────────────────────────────────────────
        Dim omsetJs As String = String.Join(",", omset12Bulan.Select(Function(v) v.ToString("0", CultureInfo.InvariantCulture)))
        Dim trxJs As String = String.Join(",", transaksi12Bulan.Select(Function(v) v.ToString()))
        Dim loginJs As String = String.Join(",", omsetLogin12Bulan.Select(Function(v) v.ToString("0", CultureInfo.InvariantCulture)))
        Dim labelsJs As String = String.Join(",", label12Bulan)

        ' ── Alerts ───────────────────────────────────────────────────
        Dim alertsHtml As New StringBuilder()
        If hutangJatuhTempo > 0 Then
            alertsHtml.AppendLine($"<div class='alert-box danger'><i class='fas fa-clock'></i> <span><b>{hutangJatuhTempo} Hutang</b> jatuh tempo</span></div>")
        End If
        If piutangBelumLunas > 0 Then
            alertsHtml.AppendLine($"<div class='alert-box warning'><i class='fas fa-hand-holding-usd'></i> <span><b>{piutangBelumLunas} Piutang</b> belum lunas</span></div>")
        End If

        ' ── Persentase stok ──────────────────────────────────────────
        Dim totalBarang As Integer = Math.Max(1, stokKritis + stokHabis + stokAman)
        Dim pAman As Integer = Int(stokAman / totalBarang * 100)
        Dim pKritis As Integer = Int(stokKritis / totalBarang * 100)
        Dim pHabis As Integer = Int(stokHabis / totalBarang * 100)

        ' ── Footer ───────────────────────────────────────────────────
        Dim footerTahun As String = If(Now.Year > 2023, "2023 - " & Now.Year.ToString(), "2023")
        Dim footerVersi As String = My.Application.Info.Version.ToString()
        Dim footerUser As String = If(String.IsNullOrEmpty(NamaUser), "-", NamaUser)
        Dim footerLevel As String = If(String.IsNullOrEmpty(LevelUser), "-", LevelUser)

        ' ── Replace placeholders ─────────────────────────────────────
        html = html.Replace("{{C_PRIMARY}}", cPrimary) _
                   .Replace("{{C_SECONDARY}}", cSecondary) _
                   .Replace("{{C_BG}}", cBg) _
                   .Replace("{{C_SURFACE}}", cSurface) _
                   .Replace("{{C_TEXT}}", cText) _
                   .Replace("{{C_MUTED}}", cMuted) _
                   .Replace("{{C_BORDER}}", cBorder) _
                   .Replace("{{C_SUBTLE}}", cSubtle) _
                   .Replace("{{C_SUCCESS}}", cSuccess) _
                   .Replace("{{C_WARNING}}", cWarning) _
                   .Replace("{{C_DANGER}}", cDanger) _
                   .Replace("{{LOKASI}}", labelLokasi) _
                   .Replace("{{TGL_STR}}", tglStr) _
                   .Replace("{{NAMA_PERUSAHAAN}}", NAMA_PERUSAHAAN) _
                   .Replace("{{OMSET_STR}}", Fmt(omsetHariIni)) _
                   .Replace("{{OMSET_BULAN_STR}}", Fmt(omsetBulanIni)) _
                   .Replace("{{OMSET_LOGIN_STR}}", Fmt(omsetLogin)) _
                   .Replace("{{OMSET_BULAN_USER_STR}}", Fmt(omsetBulanIniUser)) _
                   .Replace("{{JML_TRANSAKSI_LOGIN}}", jmlTransaksiLogin.ToString()) _
                   .Replace("{{WAKTU_LOGIN}}", If(String.IsNullOrEmpty(NamaUser), "semua user", NamaUser)) _
                   .Replace("{{TUMBUH_CSS_USER}}", tumbuhCssUser) _
                   .Replace("{{TUMBUH_ICON_USER}}", tumbuhIconUser) _
                   .Replace("{{TUMBUH_STR_USER}}", tumbuhStrUser) _
                   .Replace("{{TUMBUH_CSS}}", tumbuhCss) _
                   .Replace("{{TUMBUH_ICON}}", If(tumbuh >= 0, "fa-arrow-up", "fa-arrow-down")) _
                   .Replace("{{TUMBUH_STR}}", tumbuhStr) _
                   .Replace("{{JML_TRANSAKSI}}", jmlTransaksi.ToString()) _
                   .Replace("{{STOK_AMAN}}", pAman.ToString()) _
                   .Replace("{{STOK_KRITIS}}", pKritis.ToString()) _
                   .Replace("{{STOK_HABIS}}", pHabis.ToString()) _
                   .Replace("{{ALERTS_HTML}}", alertsHtml.ToString()) _
                   .Replace("{{OMSET_JS}}", omsetJs) _
                   .Replace("{{TRX_JS}}", trxJs) _
                   .Replace("{{LOGIN_JS}}", loginJs) _
                   .Replace("{{LABELS_JS}}", labelsJs) _
                   .Replace("{{FOOTER_USER}}", footerUser) _
                   .Replace("{{FOOTER_LEVEL}}", footerLevel) _
                   .Replace("{{FOOTER_VERSI}}", "v" & footerVersi) _
                   .Replace("{{FOOTER_TAHUN}}", "© " & footerTahun)

        Return html
    End Function

    Private Function BuildProgressItem(label As String, value As Integer, color As String) As String
        Return $"<div class='dist-item'><div class='dist-info'><span>{label}</span><span style='font-weight:700;'>{value}</span></div>" &
               $"<div class='progress-bg'><div class='progress-fill' style='width:{(Math.Min(100, value))}%; background:{color};'></div></div></div>"
    End Function

    Private Function ColorToHex(c As Color) As String
        Return "#" & c.R.ToString("X2") & c.G.ToString("X2") & c.B.ToString("X2")
    End Function

    Private Function Fmt(nilai As Decimal) As String
        If nilai = 0 Then Return "Rp -"
        Return "Rp " & nilai.ToString("#,##0", _id)
    End Function

End Module
