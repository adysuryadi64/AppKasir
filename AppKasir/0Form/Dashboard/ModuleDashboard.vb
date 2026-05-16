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
    ''' Lokasi dikirim sebagai parameter — default TOKO.
    ''' </summary>
    Public Function BangunHTML(Optional lokasi As String = "TOKO") As String
        lokasi = lokasi.Trim().ToUpper()
        Dim isGudang As Boolean = (lokasi = "GUDANG")
        Dim isDark As Boolean = ModuleTheme.IsDarkMode

        ' ── Warna tema dari ModuleTheme ───────────────────────────────
        Dim cBg As String = ColorToHex(If(isDark, D_Bg, L_Bg))
        Dim cSurface As String = ColorToHex(If(isDark, D_Surface, L_Surface))
        Dim cText As String = ColorToHex(If(isDark, D_Text, L_Text))
        Dim cMuted As String = ColorToHex(If(isDark, D_Muted, L_Muted))
        Dim cBorder As String = ColorToHex(If(isDark, D_Border, L_Border))
        Dim cSubtle As String = ColorToHex(If(isDark, D_Subtle, L_Subtle))
        Dim cSuccess As String = ColorToHex(If(isDark, D_Success, L_Success))
        Dim cWarning As String = ColorToHex(If(isDark, D_Warning, L_Warning))
        Dim cDanger As String = ColorToHex(If(isDark, D_Danger, L_Danger))

        ' ── Warna primer (tetap bedakan Toko/Gudang tapi sesuaikan tone) ──
        ' Toko  : Biru Theme
        ' Gudang: Hijau Theme
        Dim cPrimary As String = ColorToHex(If(isGudang, If(isDark, D_Success, L_Success), If(isDark, D_Primary, L_Primary)))
        Dim cSecondary As String = ColorToHex(If(isDark, D_Secondary, L_Secondary))

        Dim labelLokasi As String = If(isGudang, "GUDANG", "TOKO")

        ' ── Ambil data ────────────────────────────────────────────────
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

        ' Kolom stok sesuai lokasi
        Dim kolStok As String = If(isGudang, "STOK_GUDANG", "STOK_TOKO")

        Try
            EnsureConnectionReady()

            ' ── Rentang waktu ─────────────────────────────────────────
            Dim tglHariIni As Date    = Now.Date
            Dim tglBesok As Date      = tglHariIni.AddDays(1)
            Dim tglBulanAwal As Date  = New Date(Now.Year, Now.Month, 1)
            Dim tglBulanAkhir As Date = tglBulanAwal.AddMonths(1)
            Dim tglBulanLaluAwal As Date  = tglBulanAwal.AddMonths(-1)
            Dim tglBulanLaluAkhir As Date = tglBulanAwal
            Dim tgl7HariLalu As Date  = tglHariIni.AddDays(-6)

            ' ══════════════════════════════════════════════════════════
            ' QUERY 1 — Semua agregasi penjualan dalam 1 query
            ' Mencakup: omset hari ini, bulan ini, bulan lalu,
            '           piutang belum lunas, dan data 7 hari (GROUP BY)
            ' Pakai range >= AND < agar index TGL_TRANSAKSI terpakai
            ' ══════════════════════════════════════════════════════════
            Using cmd As New MySqlCommand(
                "SELECT " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @hariIni AND TGL_TRANSAKSI < @besok " &
                "    THEN GRAND_TOTAL_STL_PAJAK ELSE 0 END), 0) AS OMSET_HARI, " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @hariIni AND TGL_TRANSAKSI < @besok " &
                "    THEN 1 ELSE 0 END), 0) AS JML_HARI, " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @bulanAwal AND TGL_TRANSAKSI < @bulanAkhir " &
                "    THEN GRAND_TOTAL_STL_PAJAK ELSE 0 END), 0) AS OMSET_BULAN, " &
                "  COALESCE(SUM(CASE WHEN TGL_TRANSAKSI >= @bulanLaluAwal AND TGL_TRANSAKSI < @bulanLaluAkhir " &
                "    THEN GRAND_TOTAL_STL_PAJAK ELSE 0 END), 0) AS OMSET_LALU, " &
                "  COALESCE(SUM(CASE WHEN STATUS_BAYAR = 'TERHUTANG' " &
                "    THEN SISA_TAGIHAN ELSE 0 END), 0) AS TOTAL_PIUTANG, " &
                "  COALESCE(SUM(CASE WHEN STATUS_BAYAR = 'TERHUTANG' THEN 1 ELSE 0 END), 0) AS JML_PIUTANG " &
                "FROM penjualan " &
                "WHERE STATUS_TRANSAKSI <> 'BATAL' AND LOKASIBARANG = @lok " &
                "  AND TGL_TRANSAKSI >= @bulanLaluAwal", conn)
                cmd.Parameters.AddWithValue("@hariIni", tglHariIni)
                cmd.Parameters.AddWithValue("@besok", tglBesok)
                cmd.Parameters.AddWithValue("@bulanAwal", tglBulanAwal)
                cmd.Parameters.AddWithValue("@bulanAkhir", tglBulanAkhir)
                cmd.Parameters.AddWithValue("@bulanLaluAwal", tglBulanLaluAwal)
                cmd.Parameters.AddWithValue("@bulanLaluAkhir", tglBulanLaluAkhir)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        omsetHariIni         = ModuleAngka.ParseDecimal(rd("OMSET_HARI"))
                        jmlTransaksi         = ModuleAngka.ParseInteger(rd("JML_HARI"))
                        omsetBulanIni        = ModuleAngka.ParseDecimal(rd("OMSET_BULAN"))
                        omsetBulanLalu       = ModuleAngka.ParseDecimal(rd("OMSET_LALU"))
                        totalPiutang         = ModuleAngka.ParseDecimal(rd("TOTAL_PIUTANG"))
                        piutangBelumLunas    = ModuleAngka.ParseInteger(rd("JML_PIUTANG"))
                    End If
                End Using
            End Using
            nilaiRataTransaksi = If(jmlTransaksi > 0, omsetHariIni / jmlTransaksi, 0D)

            ' ══════════════════════════════════════════════════════════
            ' QUERY 2 — Pembelian bulan ini + hutang jatuh tempo + data 7 hari
            ' Semua dari tabel pembelian dalam 1 query
            ' ══════════════════════════════════════════════════════════
            Using cmd As New MySqlCommand(
                "SELECT " &
                "  COALESCE(SUM(CASE WHEN TGL_BELI >= @bulanAwal AND TGL_BELI < @bulanAkhir " &
                "    THEN GRAND_TOTAL_BELI ELSE 0 END), 0) AS BELI_BULAN, " &
                "  COALESCE(SUM(CASE WHEN STATUS_JUAL = 'TERHUTANG' AND JATUH_TEMPO < @besok " &
                "    THEN TAGIHAN - NOMINALBAYAR ELSE 0 END), 0) AS TOTAL_HUTANG, " &
                "  COALESCE(SUM(CASE WHEN STATUS_JUAL = 'TERHUTANG' AND JATUH_TEMPO < @besok " &
                "    THEN 1 ELSE 0 END), 0) AS JML_HUTANG " &
                "FROM pembelian WHERE LOKASI = @lok", conn)
                cmd.Parameters.AddWithValue("@bulanAwal", tglBulanAwal)
                cmd.Parameters.AddWithValue("@bulanAkhir", tglBulanAkhir)
                cmd.Parameters.AddWithValue("@besok", tglBesok)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        totalPembelianBulan = ModuleAngka.ParseDecimal(rd("BELI_BULAN"))
                        totalHutang         = ModuleAngka.ParseDecimal(rd("TOTAL_HUTANG"))
                        hutangJatuhTempo    = ModuleAngka.ParseInteger(rd("JML_HUTANG"))
                    End If
                End Using
            End Using

            ' ══════════════════════════════════════════════════════════
            ' QUERY 3 — Status stok (tbl_barang kecil, cepat)
            ' ══════════════════════════════════════════════════════════
            Using cmd As New MySqlCommand(
                $"SELECT " &
                $"  SUM(CASE WHEN {kolStok} <= 0 THEN 1 ELSE 0 END) AS HABIS, " &
                $"  SUM(CASE WHEN {kolStok} > 0 AND STOK_MIN > 0 AND {kolStok} <= STOK_MIN THEN 1 ELSE 0 END) AS KRITIS, " &
                $"  SUM(CASE WHEN {kolStok} > 0 AND ({kolStok} > STOK_MIN OR STOK_MIN = 0) THEN 1 ELSE 0 END) AS AMAN, " &
                $"  COALESCE(SUM(CASE WHEN {kolStok} > 0 THEN {kolStok} * HARGA_BELI ELSE 0 END), 0) AS NILAI " &
                "FROM tbl_barang WHERE STATUS = 'Aktif'", conn)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        stokHabis       = ModuleAngka.ParseInteger(rd("HABIS"))
                        stokKritis      = ModuleAngka.ParseInteger(rd("KRITIS"))
                        stokAman        = ModuleAngka.ParseInteger(rd("AMAN"))
                        nilaiPersediaan = ModuleAngka.ParseDecimal(rd("NILAI"))
                    End If
                End Using
            End Using

        Catch
            ' Gagal query — tampilkan dashboard kosong tanpa crash
        End Try

        ' ── Hitung pertumbuhan omset ──────────────────────────────────
        Dim tumbuh As Decimal = 0D
        Dim tumbuhStr As String = "-"
        Dim tumbuhCss As String = "netral"
        If omsetBulanLalu > 0 Then
            tumbuh = ((omsetBulanIni - omsetBulanLalu) / omsetBulanLalu) * 100
            tumbuhStr = (If(tumbuh >= 0, "+", "")) & tumbuh.ToString("0.0", _id) & "%"
            tumbuhCss = If(tumbuh >= 0, "naik", "turun")
        End If

        ' ── Data 7 hari untuk chart — 1 query GROUP BY ───────────────
        Dim omset7Hari(6) As Decimal
        Dim transaksi7Hari(6) As Integer
        Dim beli7Hari(6) As Decimal
        Dim label7Hari(6) As String

        ' Inisialisasi label dulu
        For i As Integer = 0 To 6
            Dim tgl As Date = Now.Date.AddDays(i - 6)
            label7Hari(i) = tgl.ToString("dd/MM", _id)
        Next

        Try
            EnsureConnectionReady()
            Dim tgl7HariLalu As Date = Now.Date.AddDays(-6)
            Dim tglBesok2 As Date    = Now.Date.AddDays(1)

            ' Penjualan 7 hari — 1 query GROUP BY
            Using cmd As New MySqlCommand(
                "SELECT DATE(TGL_TRANSAKSI) AS TGL, " &
                "  COALESCE(SUM(GRAND_TOTAL_STL_PAJAK),0) AS OMSET, COUNT(*) AS JML " &
                "FROM penjualan " &
                "WHERE TGL_TRANSAKSI >= @awal AND TGL_TRANSAKSI < @akhir " &
                "  AND STATUS_TRANSAKSI <> 'BATAL' AND LOKASIBARANG = @lok " &
                "GROUP BY DATE(TGL_TRANSAKSI)", conn)
                cmd.Parameters.AddWithValue("@awal", tgl7HariLalu)
                cmd.Parameters.AddWithValue("@akhir", tglBesok2)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim tglRow As Date = Convert.ToDateTime(rd("TGL"))
                        Dim idx As Integer = CInt((tglRow.Date - tgl7HariLalu).TotalDays)
                        If idx >= 0 AndAlso idx <= 6 Then
                            omset7Hari(idx)     = ModuleAngka.ParseDecimal(rd("OMSET"))
                            transaksi7Hari(idx) = ModuleAngka.ParseInteger(rd("JML"))
                        End If
                    End While
                End Using
            End Using

            ' Pembelian 7 hari — 1 query GROUP BY
            Using cmd As New MySqlCommand(
                "SELECT DATE(TGL_BELI) AS TGL, COALESCE(SUM(GRAND_TOTAL_BELI),0) AS BELI " &
                "FROM pembelian " &
                "WHERE TGL_BELI >= @awal AND TGL_BELI < @akhir AND LOKASI = @lok " &
                "GROUP BY DATE(TGL_BELI)", conn)
                cmd.Parameters.AddWithValue("@awal", tgl7HariLalu)
                cmd.Parameters.AddWithValue("@akhir", tglBesok2)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim tglRow As Date = Convert.ToDateTime(rd("TGL"))
                        Dim idx As Integer = CInt((tglRow.Date - tgl7HariLalu).TotalDays)
                        If idx >= 0 AndAlso idx <= 6 Then
                            beli7Hari(idx) = ModuleAngka.ParseDecimal(rd("BELI"))
                        End If
                    End While
                End Using
            End Using
        Catch
        End Try

        ' ── Bangun HTML dari Template Embedded Resource ─────────────────────────────────
        ' Template HTML disimpan sebagai Embedded Resource di project — tidak perlu file eksternal
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
            ' Fallback jika resource tidak ditemukan
            Return "Error: Dashboard template embedded resource not found"
        End Try

        If String.IsNullOrEmpty(html) Then
            Return "Error: Dashboard template is empty"
        End If

        Dim tglStr As String = Now.ToString("dddd, dd MMMM yyyy", _id)

        ' Variabel untuk JS
        Dim omsetJs As String = String.Join(",", omset7Hari.Select(Function(v) v.ToString("0", CultureInfo.InvariantCulture)))
        Dim trxJs As String = String.Join(",", transaksi7Hari.Select(Function(v) v.ToString()))
        Dim beliJs As String = String.Join(",", beli7Hari.Select(Function(v) v.ToString("0", CultureInfo.InvariantCulture)))
        Dim labelsJs As String = String.Join(",", label7Hari) ' Tanpa quotes karena akan di-split di JS
        Dim stokJs As String = "80,85,82,88,84,90,100" ' Dummy trend stok

        ' Alerts HTML
        Dim alertsHtml As New StringBuilder()
        If hutangJatuhTempo > 0 Then
            alertsHtml.AppendLine($"<div class='alert-box danger'><i class='fas fa-clock'></i> <span><b>{hutangJatuhTempo} Hutang</b> jatuh tempo</span></div>")
        End If
        If piutangBelumLunas > 0 Then
            alertsHtml.AppendLine($"<div class='alert-box warning'><i class='fas fa-hand-holding-usd'></i> <span><b>{piutangBelumLunas} Piutang</b> belum lunas</span></div>")
        End If

        ' Hitung persentase stok untuk donut chart & progress bars
        ' Berdasarkan jumlah barang sebenarnya dari database
        Dim totalBarang As Integer = stokKritis + stokHabis + stokAman
        If totalBarang = 0 Then totalBarang = 1  ' Hindari pembagian dengan 0
        Dim pAman As Integer = Int(stokAman / totalBarang * 100)
        Dim pKritis As Integer = Int(stokKritis / totalBarang * 100)
        Dim pHabis As Integer = Int(stokHabis / totalBarang * 100)

        ' Replace Placeholders
        Dim footerTahun As String = If(Now.Year > 2023, "2023 - " & Now.Year.ToString(), "2023")
        Dim footerVersi As String = My.Application.Info.Version.ToString()
        Dim footerUser As String = If(String.IsNullOrEmpty(NamaUser), "-", NamaUser)
        Dim footerLevel As String = If(String.IsNullOrEmpty(LevelUser), "-", LevelUser)

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
                   .Replace("{{TUMBUH_CSS}}", tumbuhCss) _
                   .Replace("{{TUMBUH_ICON}}", If(tumbuh >= 0, "fa-arrow-up", "fa-arrow-down")) _
                   .Replace("{{TUMBUH_STR}}", tumbuhStr) _
                   .Replace("{{JML_TRANSAKSI}}", jmlTransaksi.ToString()) _
                   .Replace("{{BELI_STR}}", Fmt(totalPembelianBulan)) _
                   .Replace("{{STOK_TOTAL}}", (stokHabis + stokKritis).ToString()) _
                   .Replace("{{STOK_AMAN}}", pAman.ToString()) _
                   .Replace("{{STOK_KRITIS}}", pKritis.ToString()) _
                   .Replace("{{STOK_HABIS}}", pHabis.ToString()) _
                   .Replace("{{ALERTS_HTML}}", alertsHtml.ToString()) _
                   .Replace("{{OMSET_JS}}", omsetJs) _
                   .Replace("{{TRX_JS}}", trxJs) _
                   .Replace("{{BELI_JS}}", beliJs) _
                   .Replace("{{STOK_JS}}", stokJs) _
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

    ''' <summary>
    ''' Helper konversi Color ke Hex string untuk CSS.
    ''' </summary>
    Private Function ColorToHex(c As Color) As String
        Return "#" & c.R.ToString("X2") & c.G.ToString("X2") & c.B.ToString("X2")
    End Function

    Private Function Fmt(nilai As Decimal) As String
        If nilai = 0 Then Return "Rp -"
        Return "Rp " & nilai.ToString("#,##0", _id)
    End Function

End Module
