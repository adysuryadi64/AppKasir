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
            Dim tglHariIni As String = Now.ToString("yyyy-MM-dd")
            Dim tglBulanAwal As String = New Date(Now.Year, Now.Month, 1).ToString("yyyy-MM-dd")
            Dim tglBulanLaluAwal As String = New Date(Now.Year, Now.Month, 1).AddMonths(-1).ToString("yyyy-MM-dd")
            Dim tglBulanLaluAkhir As String = New Date(Now.Year, Now.Month, 1).AddDays(-1).ToString("yyyy-MM-dd")

            ' ── Omset & transaksi hari ini ────────────────────────────
            Using cmd As New MySqlCommand(
                "SELECT COALESCE(SUM(GRAND_TOTAL_STL_PAJAK),0) AS OMSET, COUNT(*) AS JML " &
                "FROM penjualan " &
                "WHERE DATE(TGL_TRANSAKSI) = @tgl AND STATUS_TRANSAKSI <> 'BATAL' " &
                "  AND LOKASIBARANG = @lok", conn)
                cmd.Parameters.AddWithValue("@tgl", tglHariIni)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        omsetHariIni = ModuleAngka.ParseDecimal(rd("OMSET"))
                        jmlTransaksi = ModuleAngka.ParseInteger(rd("JML"))
                    End If
                End Using
            End Using
            nilaiRataTransaksi = If(jmlTransaksi > 0, omsetHariIni / jmlTransaksi, 0D)

            ' ── Omset bulan ini ───────────────────────────────────────
            Using cmd As New MySqlCommand(
                "SELECT COALESCE(SUM(GRAND_TOTAL_STL_PAJAK),0) FROM penjualan " &
                "WHERE DATE(TGL_TRANSAKSI) >= @tgl AND STATUS_TRANSAKSI <> 'BATAL' " &
                "  AND LOKASIBARANG = @lok", conn)
                cmd.Parameters.AddWithValue("@tgl", tglBulanAwal)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                omsetBulanIni = ModuleAngka.ParseDecimal(cmd.ExecuteScalar())
            End Using

            ' ── Omset bulan lalu ──────────────────────────────────────
            Using cmd As New MySqlCommand(
                "SELECT COALESCE(SUM(GRAND_TOTAL_STL_PAJAK),0) FROM penjualan " &
                "WHERE DATE(TGL_TRANSAKSI) BETWEEN @awal AND @akhir AND STATUS_TRANSAKSI <> 'BATAL' " &
                "  AND LOKASIBARANG = @lok", conn)
                cmd.Parameters.AddWithValue("@awal", tglBulanLaluAwal)
                cmd.Parameters.AddWithValue("@akhir", tglBulanLaluAkhir)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                omsetBulanLalu = ModuleAngka.ParseDecimal(cmd.ExecuteScalar())
            End Using

            ' ── Pembelian bulan ini ───────────────────────────────────
            Using cmd As New MySqlCommand(
                "SELECT COALESCE(SUM(GRAND_TOTAL_BELI),0) FROM pembelian " &
                "WHERE DATE(TGL_BELI) >= @tgl AND LOKASI = @lok", conn)
                cmd.Parameters.AddWithValue("@tgl", tglBulanAwal)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                totalPembelianBulan = ModuleAngka.ParseDecimal(cmd.ExecuteScalar())
            End Using

            ' ── Stok kritis, habis & aman ────────────────────────────────
            Using cmd As New MySqlCommand(
                $"SELECT " &
                $"  SUM(CASE WHEN {kolStok} <= STOK_MIN AND STOK_MIN > 0 AND {kolStok} > 0 THEN 1 ELSE 0 END) AS KRITIS, " &
                $"  SUM(CASE WHEN {kolStok} <= 0 THEN 1 ELSE 0 END) AS HABIS, " &
                $"  SUM(CASE WHEN {kolStok} > STOK_MIN OR STOK_MIN = 0 THEN 1 ELSE 0 END) AS AMAN, " &
                $"  COALESCE(SUM({kolStok} * HARGA_BELI), 0) AS NILAI " &
                "FROM tbl_barang WHERE STATUS = 'Aktif'", conn)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        stokKritis = ModuleAngka.ParseInteger(rd("KRITIS"))
                        stokHabis = ModuleAngka.ParseInteger(rd("HABIS"))
                        stokAman = ModuleAngka.ParseInteger(rd("AMAN"))
                        nilaiPersediaan = ModuleAngka.ParseDecimal(rd("NILAI"))
                    End If
                End Using
            End Using

            ' ── Hutang jatuh tempo ────────────────────────────────────
            ' Hutang ada di tabel pembelian: STATUS_JUAL='TERHUTANG', sisa = TAGIHAN - NOMINALBAYAR
            Using cmd As New MySqlCommand(
                "SELECT COUNT(*), COALESCE(SUM(TAGIHAN - NOMINALBAYAR),0) FROM pembelian " &
                "WHERE DATE(JATUH_TEMPO) <= @tgl AND STATUS_JUAL = 'TERHUTANG' " &
                "  AND LOKASI = @lok", conn)
                cmd.Parameters.AddWithValue("@tgl", tglHariIni)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        hutangJatuhTempo = ModuleAngka.ParseInteger(rd(0))
                        totalHutang = ModuleAngka.ParseDecimal(rd(1))
                    End If
                End Using
            End Using

            ' ── Piutang belum lunas ───────────────────────────────────
            Using cmd As New MySqlCommand(
                "SELECT COUNT(*), COALESCE(SUM(SISA_TAGIHAN),0) FROM penjualan " &
                "WHERE STATUS_BAYAR = 'TERHUTANG' AND LOKASIBARANG = @lok", conn)
                cmd.Parameters.AddWithValue("@lok", lokasi)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        piutangBelumLunas = ModuleAngka.ParseInteger(rd(0))
                        totalPiutang = ModuleAngka.ParseDecimal(rd(1))
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

        ' ── Omset & Data 7 hari terakhir untuk chart ────────────────────────
        Dim omset7Hari(6) As Decimal
        Dim transaksi7Hari(6) As Integer
        Dim beli7Hari(6) As Decimal
        Dim label7Hari(6) As String

        For i As Integer = 6 To 0 Step -1
            Dim tgl As Date = Now.Date.AddDays(-i)
            label7Hari(6 - i) = tgl.ToString("dd/MM", _id)
            Try
                ' Omset & Transaksi
                Using cmd As New MySqlCommand(
                    "SELECT COALESCE(SUM(GRAND_TOTAL_STL_PAJAK),0) AS OMSET, COUNT(*) AS JML FROM penjualan " &
                    "WHERE DATE(TGL_TRANSAKSI) = @tgl AND STATUS_TRANSAKSI <> 'BATAL' " &
                    "  AND LOKASIBARANG = @lok", conn)
                    cmd.Parameters.AddWithValue("@tgl", tgl.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@lok", lokasi)
                    Using rd = cmd.ExecuteReader()
                        If rd.Read() Then
                            omset7Hari(6 - i) = ModuleAngka.ParseDecimal(rd("OMSET"))
                            transaksi7Hari(6 - i) = ModuleAngka.ParseInteger(rd("JML"))
                        End If
                    End Using
                End Using

                ' Pembelian
                Using cmd As New MySqlCommand(
                    "SELECT COALESCE(SUM(GRAND_TOTAL_BELI),0) FROM pembelian " &
                    "WHERE DATE(TGL_BELI) = @tgl AND LOKASI = @lok", conn)
                    cmd.Parameters.AddWithValue("@tgl", tgl.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@lok", lokasi)
                    beli7Hari(6 - i) = ModuleAngka.ParseDecimal(cmd.ExecuteScalar())
                End Using
            Catch
            End Try
        Next

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
