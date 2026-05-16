Imports System.Globalization

Public Class FormLapLabaRugi

    Private Periode As String = ""

    ' ── Form Load ─────────────────────────────────────────────────────────────
    Private Sub FormLapLabaRugi_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        MuatComboBoxBulanTahun(CmbBln, CmbThn)
    End Sub

    Private Sub RbtBulan_CheckedChanged(sender As Object, e As EventArgs) Handles RbtBulan.CheckedChanged
        If RbtBulan.Checked Then
            PanelBulan.Visible = True
            PanelTanggal.Visible = False
        End If
    End Sub

    Private Sub RbtTanggal_CheckedChanged(sender As Object, e As EventArgs) Handles RbtTanggal.CheckedChanged
        If RbtTanggal.Checked Then
            PanelTanggal.Visible = True
            PanelBulan.Visible = False
        End If
    End Sub

    Private Sub RbtSemua_CheckedChanged(sender As Object, e As EventArgs) Handles RbtSemua.CheckedChanged
        If RbtSemua.Checked Then
            PanelBulan.Visible = False
            PanelTanggal.Visible = False
        End If
    End Sub

    ' ── Tombol Tampil ─────────────────────────────────────────────────────────
    Private Sub BtnTampil_Click(sender As Object, e As EventArgs) Handles BtnTampil.Click
        Try
            Me.Cursor = Cursors.WaitCursor

            If RbtSemua.Checked Then
                ' Ambil rentang dari JurnalUmum
                Dim tglAwal As Date = Date.MinValue
                Dim tglAkhir As Date = Date.MinValue
                EnsureConnectionReady()
                Using cmd As New MySqlCommand(
                    "SELECT MIN(TGL_TRANSAKSI) AS A, MAX(TGL_TRANSAKSI) AS B FROM JurnalUmum", conn)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            tglAwal = If(IsDBNull(rd("A")), Date.Today, Convert.ToDateTime(rd("A")))
                            tglAkhir = If(IsDBNull(rd("B")), Date.Today, Convert.ToDateTime(rd("B")))
                        End If
                    End Using
                End Using
                Periode = "Periode : " & tglAwal.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) &
                          " - " & tglAkhir.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))
                ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()
                ModuleLaporanKalkulasi.SiapkanTempDatareferensi_SalinDariTblDatareferensi()

            ElseIf RbtTanggal.Checked Then
                Dim tglAwal As Date = DtpAwal.Value.Date
                Dim tglAkhir As Date = DtpAkhir.Value.Date.AddDays(1).AddTicks(-1)
                Periode = "Periode : " & tglAwal.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) &
                          " - " & DtpAkhir.Value.Date.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))
                ModuleLaporanKalkulasi.SiapkanTempDatareferensi_SalinDariTblDatareferensi()
                ModuleLaporanKalkulasi.HitungSaldoAwal_PeriodeLaporan_KeTempDatareferensi(tglAwal)
                ModuleLaporanKalkulasi.HitungDebetKredit_PeriodeLaporan_KeTempDatareferensi(tglAwal, tglAkhir)
                ModuleLaporanKalkulasi.HitungSaldoAkhir_PeriodeLaporan_KeTempDatareferensi(tglAkhir)

            ElseIf RbtBulan.Checked Then
                Dim tglAwal As Date, tglAkhir As Date
                If Not GetRentangBulan(CmbBln, CmbThn, tglAwal, tglAkhir) Then Exit Sub
                Periode = "Periode : " & tglAwal.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) &
                          " - " & tglAkhir.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))
                ModuleLaporanKalkulasi.SiapkanTempDatareferensi_SalinDariTblDatareferensi()
                ModuleLaporanKalkulasi.HitungSaldoAwal_PeriodeLaporan_KeTempDatareferensi(tglAwal)
                ModuleLaporanKalkulasi.HitungDebetKredit_PeriodeLaporan_KeTempDatareferensi(tglAwal, tglAkhir)
                ModuleLaporanKalkulasi.HitungSaldoAkhir_PeriodeLaporan_KeTempDatareferensi(tglAkhir)
            End If

            TampilkanLabaRugiHTML()

        Catch ex As Exception
            MessageBox.Show("Gagal menampilkan laporan: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub FormLapLabaRugi_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F5 Then BtnTampil.PerformClick()
    End Sub
    ' ── Laporan Laba Rugi via HTML di WebBrowser ──────────────────────────────

    ''' <summary>
    ''' Tampilkan laporan Laba Rugi sebagai HTML di WebBrowser control.
    ''' Query dilakukan di ModuleLabaRugiHTML per JENIS_AKUN — identik dengan FormLapNeracaLR.
    ''' </summary>
    Private Sub TampilkanLabaRugiHTML()
        Try
            Dim html As String = ModuleLabaRugiHTML.BangunHTML(
                NAMA_PERUSAHAAN, Periode,
                "SALDO AWAL", "PERIODE INI",
                "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)

            WebBrowserLR.DocumentText = html

        Catch ex As Exception
            MessageBox.Show("Gagal membangun laporan: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class