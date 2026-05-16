Imports System.Globalization
Imports Microsoft.Reporting.WinForms


Public Class FormLapNeracaLR

    Private Sub FormLaporan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)

    End Sub


    Private Sub RbtRentang_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RbtTanggal.CheckedChanged
        ' Pastikan RadioButton dalam keadaan Checked sebelum memanggil Ambildataperusahaan
        If RbtTanggal.Checked Then
            'DtpAwal.Value = tanggalAwalPeriodeKerja
            'DtpAkhir.Value = tanggalAkhirPeriodeKerja
        End If
    End Sub

    Private Sub RbtBulan_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RbtBulan.CheckedChanged
        If RbtBulan.Checked Then
            MuatComboBoxBulanTahun(CmbBln, CmbThn)
        End If
    End Sub




    '---------------------------------------- TabLapNeraca --------------------------------------------------------------------------------------
    Dim Periode As String
    Private Sub BtnTampilNeraca_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTampilNeraca.Click
        Dim tTotal As DateTime = DateTime.Now
        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer2.LocalReport.DataSources.Clear()

        If RbtSemua.Checked Then
            Dim tanggalAwal As Date
            Dim tanggalAkhir As Date
            ' Ambil tanggal dari JurnalUmum
            Dim queryTanggal As String = "SELECT MIN(TGL_TRANSAKSI) AS TANGGAL_AWAL, MAX(TGL_TRANSAKSI) AS TANGGAL_AKHIR FROM JurnalUmum"

            Using cmd As New MySqlCommand(queryTanggal, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        tanggalAwal = If(IsDBNull(reader("TANGGAL_AWAL")), Date.MinValue, Convert.ToDateTime(reader("TANGGAL_AWAL")))
                        tanggalAkhir = If(IsDBNull(reader("TANGGAL_AKHIR")), Date.MinValue, Convert.ToDateTime(reader("TANGGAL_AKHIR")))
                    End If
                End Using
            End Using

            Periode = "Periode : " & tanggalAwal.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) & " - " & tanggalAkhir.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))
            ' RbtSemua: Memanggil ModuleLaporanKalkulasi untuk posting resmi
            ' Setelah selesai, sync ke temp_datareferensi agar Tampilnerca/TampilkanLabaRugi/TampilNeracaLajur
            ' bisa membaca dari sumber yang sama (temp_datareferensi)
            ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()
            ModuleLaporanKalkulasi.SiapkanTempDatareferensi_SalinDariTblDatareferensi()
        Else
            Dim tanggalAwal As Date
            Dim tanggalAkhir As Date

            If RbtTanggal.Checked Then
                tanggalAwal = DtpAwal.Value.Date
                tanggalAkhir = DtpAkhir.Value.Date.AddDays(1).AddTicks(-1)
            ElseIf RbtBulan.Checked Then
                If Not GetRentangBulan(CmbBln, CmbThn, tanggalAwal, tanggalAkhir) Then Exit Sub
            End If
            Periode = "Periode : " & tanggalAwal.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) & " - " & tanggalAkhir.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))

            ' Requirement 17: Semua kalkulasi laporan dipindahkan ke ModuleLaporanKalkulasi
            ' agar tbl_datareferensi.Saldo_Akhir tidak berubah saat user buka laporan
            ModuleLaporanKalkulasi.SiapkanTempDatareferensi_SalinDariTblDatareferensi()
            ModuleLaporanKalkulasi.HitungSaldoAwal_PeriodeLaporan_KeTempDatareferensi(tanggalAwal)
            ModuleLaporanKalkulasi.HitungDebetKredit_PeriodeLaporan_KeTempDatareferensi(tanggalAwal, tanggalAkhir)
            ModuleLaporanKalkulasi.HitungSaldoAkhir_PeriodeLaporan_KeTempDatareferensi(tanggalAkhir)
        End If

        Try
            EnsureConnectionReady()
            Dim t1 As DateTime = DateTime.Now
            Tampilnerca()
            Dim t2 As DateTime = DateTime.Now
            TampilkanLabaRugi()
            Dim t3 As DateTime = DateTime.Now
            TampilNeracaLajur()
        Catch ex As Exception
            MessageBox.Show("Gagal menampilkan laporan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' =======================================================================================================
    ' WRAPPER FUNCTIONS - Backward Compatibility
    ' Semua fungsi ini MEMANGGIL ModuleLaporanKalkulasi (hanya untuk kompatibilitas kode lama)
    ' =======================================================================================================

    ''' <summary>
    ''' [WRAPPER] Panggil ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()
    ''' </summary>
    Public Sub HITUNGSEMUASALDO()
        ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()
    End Sub

    ''' <summary>
    ''' [WRAPPER] Panggil ModuleLaporanKalkulasi.HitungSaldoAwal_PeriodeLaporan_KeTempDatareferensi()
    ''' </summary>
    Public Sub HITUNGSALDOAWAL(ByVal tanggalAwal As Date)
        ModuleLaporanKalkulasi.HitungSaldoAwal_PeriodeLaporan_KeTempDatareferensi(tanggalAwal)
    End Sub

    ''' <summary>
    ''' [WRAPPER] Panggil ModuleLaporanKalkulasi.HitungDebetKredit_PeriodeLaporan_KeTempDatareferensi()
    ''' </summary>
    Private Sub HITUNGDEBETKREDIT(ByVal tanggalawal As Date, ByVal tanggalakhir As Date)
        ModuleLaporanKalkulasi.HitungDebetKredit_PeriodeLaporan_KeTempDatareferensi(tanggalawal, tanggalakhir)
    End Sub

    ''' <summary>
    ''' [WRAPPER] Panggil ModuleLaporanKalkulasi.HitungSaldoAkhir_PeriodeLaporan_KeTempDatareferensi()
    ''' </summary>
    Public Sub HITUNGSALDOAKHIR(ByVal tanggalakhir As Date)
        ModuleLaporanKalkulasi.HitungSaldoAkhir_PeriodeLaporan_KeTempDatareferensi(tanggalakhir)
    End Sub

    ''' <summary>
    ''' [WRAPPER] Panggil ModuleLaporanKalkulasi.SiapkanTempDatareferensi_SalinDariTblDatareferensi()
    ''' </summary>
    Private Sub SiapkanTempDatareferensi()
        ModuleLaporanKalkulasi.SiapkanTempDatareferensi_SalinDariTblDatareferensi()
    End Sub


    Private Sub Tampilnerca()
        ReportViewer1.LocalReport.DataSources.Clear()

        ' ── Aturan akuntansi untuk nilai yang dikirim ke RDLC ────────────────────────
        ' Aktiva  DEBET  normal → +SALDO (positif)
        ' Aktiva  KREDIT normal → -SALDO (akumulasi penyusutan mengurangi aktiva)
        ' Pasiva  KREDIT normal → +SALDO (positif)
        ' Pasiva  DEBET  normal → -SALDO (PRIVE mengurangi modal)
        ' Laba Rugi Berjalan    → +SALDO (masuk sisi modal, laba menambah modal)
        ' RDLC cukup SUM() — tidak perlu logika tanda di sana

        Dim queries As New Dictionary(Of String, String) From {
            {"DataSet1",
             "SELECT KODE_AKUN, NAMA_AKUN, " &
             "  CASE WHEN AKUN_DK='DEBET'  THEN  SALDO_SEBELUMNYA " &
             "       WHEN AKUN_DK='KREDIT' THEN -SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
             "  CASE WHEN AKUN_DK='DEBET'  THEN  (SALDO_AKHIR - SALDO_SEBELUMNYA) " &
             "       WHEN AKUN_DK='KREDIT' THEN -(SALDO_AKHIR - SALDO_SEBELUMNYA) END AS Perubahan, " &
             "  CASE WHEN AKUN_DK='DEBET'  THEN  SALDO_AKHIR " &
             "       WHEN AKUN_DK='KREDIT' THEN -SALDO_AKHIR END AS SALDO_AKHIR " &
             "FROM temp_datareferensi WHERE JENIS_AKUN = 'ASET LANCAR' ORDER BY KODE_AKUN"},
            {"DataSet2",
             "SELECT KODE_AKUN, NAMA_AKUN, " &
             "  CASE WHEN AKUN_DK='DEBET'  THEN  SALDO_SEBELUMNYA " &
             "       WHEN AKUN_DK='KREDIT' THEN -SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
             "  CASE WHEN AKUN_DK='DEBET'  THEN  (SALDO_AKHIR - SALDO_SEBELUMNYA) " &
             "       WHEN AKUN_DK='KREDIT' THEN -(SALDO_AKHIR - SALDO_SEBELUMNYA) END AS Perubahan, " &
             "  CASE WHEN AKUN_DK='DEBET'  THEN  SALDO_AKHIR " &
             "       WHEN AKUN_DK='KREDIT' THEN -SALDO_AKHIR END AS SALDO_AKHIR " &
             "FROM temp_datareferensi WHERE JENIS_AKUN = 'ASET TETAP' ORDER BY KODE_AKUN"},
            {"DataSet3",
             "SELECT KODE_AKUN, NAMA_AKUN, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN  SALDO_SEBELUMNYA " &
             "       WHEN AKUN_DK='DEBET'  THEN -SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN  (SALDO_AKHIR - SALDO_SEBELUMNYA) " &
             "       WHEN AKUN_DK='DEBET'  THEN -(SALDO_AKHIR - SALDO_SEBELUMNYA) END AS Perubahan, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN  SALDO_AKHIR " &
             "       WHEN AKUN_DK='DEBET'  THEN -SALDO_AKHIR END AS SALDO_AKHIR " &
             "FROM temp_datareferensi WHERE JENIS_AKUN = 'PASIVA' ORDER BY KODE_AKUN"},
            {"DataSet4",
             "SELECT KODE_AKUN, NAMA_AKUN, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN  SALDO_SEBELUMNYA " &
             "       WHEN AKUN_DK='DEBET'  THEN -SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN  (SALDO_AKHIR - SALDO_SEBELUMNYA) " &
             "       WHEN AKUN_DK='DEBET'  THEN -(SALDO_AKHIR - SALDO_SEBELUMNYA) END AS Perubahan, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN  SALDO_AKHIR " &
             "       WHEN AKUN_DK='DEBET'  THEN -SALDO_AKHIR END AS SALDO_AKHIR " &
             "FROM temp_datareferensi " &
             "WHERE JENIS_AKUN = 'MODAL' OR KODE_AKUN = '05.01.001' " &
             "ORDER BY KODE_AKUN"}
        }

        ' Eksekusi query untuk setiap dataset dan tambahkan ke data source laporan
        For Each kvp As KeyValuePair(Of String, String) In queries
            Using cmd As New MySqlCommand(kvp.Value, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Dim ds As New DataSet()
                    ds.Load(rd, LoadOption.OverwriteChanges, kvp.Key)
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource(kvp.Key, ds.Tables(kvp.Key)))
                End Using
            End Using
        Next

        Dim AWAL As String = String.Empty
        Dim RUBAH As String = String.Empty
        Dim AKHIR As String = "SALDO AKHIR"

        Select Case True
            Case RbtSemua.Checked
                AWAL = "SALDO AWAL"
                RUBAH = "PERIODE INI"
            Case RbtTanggal.Checked
                AWAL = "PERIODE LALU"
                RUBAH = "PERIODE INI"
            Case RbtBulan.Checked
                AWAL = "BULAN LALU"
                RUBAH = "BULAN INI"
        End Select

        ' Set parameter laporan
        Dim parameters As ReportParameter() = {
            New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN),
            New ReportParameter("PERIODE", Periode),
            New ReportParameter("AWAL", AWAL),
            New ReportParameter("RUBAH", RUBAH),
            New ReportParameter("AKHIR", AKHIR),
            New ReportParameter("USER", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
        }

        ReportViewer1.LocalReport.SetParameters(parameters)
        ReportViewer1.RefreshReport()
    End Sub

    '---------------------------------------- TabLapLabaRugi ------------------------------------------------------------------------------------


    Private Sub TampilkanLabaRugi()
        ' Membersihkan sumber data laporan sebelum menambahkan yang baru
        ReportViewer2.LocalReport.DataSources.Clear()

        ' ── Aturan tanda nilai untuk laporan L/R ────────────────────────────────
        ' DataSet1 (PENJUALAN) — akun 05.xx, filter SUB_AKUN + AKUN_DK:
        '   LABA + KREDIT → positif  : 05.02 Penjualan
        '   LABA + DEBET  → negatif  : 05.03 Retur Jual, 05.04 Diskon Jual
        ' DataSet2 (HPP) — akun 06.xx, filter SUB_AKUN + AKUN_DK:
        '   RUGI + DEBET  → positif  : 06.01 HPP, 06.02-03 Biaya Kirim, 06.04 Peny. Stok
        '   RUGI + KREDIT → negatif  : 06.05 Diskon Beli, 06.06 Retur Beli
        ' DataSet3 (BIAYA): RUGI+DEBET → positif
        ' DataSet4 (PENDAPATAN LAIN): LABA+KREDIT → positif
        ' DataSet5 (PAJAK): RUGI+DEBET → positif
        ' RDLC menerima data matang — nilai sudah mencerminkan arah yang benar
        Dim accountTypeQueries As New Dictionary(Of String, String)() From {
            {"DataSet5",
             "SELECT KODE_AKUN, NAMA_AKUN, " &
             "  CASE " &
             "    WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'KREDIT' THEN  SALDO_SEBELUMNYA " &
             "    WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'DEBET'  THEN -SALDO_SEBELUMNYA " &
             "    ELSE SALDO_SEBELUMNYA " &
             "  END AS SALDO_SEBELUMNYA, " &
             "  CASE " &
             "    WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'KREDIT' THEN  (SALDO_AKHIR - SALDO_SEBELUMNYA) " &
             "    WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'DEBET'  THEN -(SALDO_AKHIR - SALDO_SEBELUMNYA) " &
             "    ELSE (SALDO_AKHIR - SALDO_SEBELUMNYA) " &
             "  END AS Perubahan, " &
             "  CASE " &
             "    WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'KREDIT' THEN  SALDO_AKHIR " &
             "    WHEN SUB_AKUN = 'LABA' AND AKUN_DK = 'DEBET'  THEN -SALDO_AKHIR " &
             "    ELSE SALDO_AKHIR " &
             "  END AS SALDO_AKHIR " &
             "FROM temp_datareferensi WHERE JENIS_AKUN = 'PENJUALAN' ORDER BY KODE_AKUN"},
            {"DataSet1",
             "SELECT KODE_AKUN, NAMA_AKUN, " &
             "  CASE " &
             "    WHEN SUB_AKUN = 'RUGI' AND AKUN_DK = 'KREDIT' THEN -SALDO_SEBELUMNYA " &
             "    ELSE SALDO_SEBELUMNYA " &
             "  END AS SALDO_SEBELUMNYA, " &
             "  CASE " &
             "    WHEN SUB_AKUN = 'RUGI' AND AKUN_DK = 'KREDIT' THEN -(SALDO_AKHIR - SALDO_SEBELUMNYA) " &
             "    ELSE (SALDO_AKHIR - SALDO_SEBELUMNYA) " &
             "  END AS Perubahan, " &
             "  CASE " &
             "    WHEN SUB_AKUN = 'RUGI' AND AKUN_DK = 'KREDIT' THEN -SALDO_AKHIR " &
             "    ELSE SALDO_AKHIR " &
             "  END AS SALDO_AKHIR " &
             "FROM temp_datareferensi WHERE JENIS_AKUN = 'HPP' ORDER BY KODE_AKUN"},
            {"DataSet2",
             "SELECT KODE_AKUN, NAMA_AKUN, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN -SALDO_SEBELUMNYA ELSE SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN -(SALDO_AKHIR - SALDO_SEBELUMNYA) ELSE (SALDO_AKHIR - SALDO_SEBELUMNYA) END AS Perubahan, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN -SALDO_AKHIR ELSE SALDO_AKHIR END AS SALDO_AKHIR " &
             "FROM temp_datareferensi WHERE JENIS_AKUN = 'BIAYA' ORDER BY KODE_AKUN"},
            {"DataSet3",
             "SELECT KODE_AKUN, NAMA_AKUN, " &
             "  CASE WHEN AKUN_DK='DEBET' THEN -SALDO_SEBELUMNYA ELSE SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
             "  CASE WHEN AKUN_DK='DEBET' THEN -(SALDO_AKHIR - SALDO_SEBELUMNYA) ELSE (SALDO_AKHIR - SALDO_SEBELUMNYA) END AS Perubahan, " &
             "  CASE WHEN AKUN_DK='DEBET' THEN -SALDO_AKHIR ELSE SALDO_AKHIR END AS SALDO_AKHIR " &
             "FROM temp_datareferensi WHERE JENIS_AKUN = 'PENDAPATAN LAIN' ORDER BY KODE_AKUN"},
            {"DataSet4",
             "SELECT KODE_AKUN, NAMA_AKUN, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN -SALDO_SEBELUMNYA ELSE SALDO_SEBELUMNYA END AS SALDO_SEBELUMNYA, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN -(SALDO_AKHIR - SALDO_SEBELUMNYA) ELSE (SALDO_AKHIR - SALDO_SEBELUMNYA) END AS Perubahan, " &
             "  CASE WHEN AKUN_DK='KREDIT' THEN -SALDO_AKHIR ELSE SALDO_AKHIR END AS SALDO_AKHIR " &
             "FROM temp_datareferensi WHERE JENIS_AKUN = 'PAJAK' ORDER BY KODE_AKUN"}
        }


        ' Menggunakan satu dataset untuk mengurangi penggunaan memori
        Dim dataset As New DataSet()

        ' Loop melalui query dan tambahkan data ke dataset
        For Each accountTypeQuery As KeyValuePair(Of String, String) In accountTypeQueries
            Dim query As String = accountTypeQuery.Value
            Dim datasetName As String = accountTypeQuery.Key

            Using cmd As New MySqlCommand(query, conn)
                Using adapter As New MySqlDataAdapter(cmd)
                    adapter.Fill(dataset, datasetName)
                End Using
            End Using

            ' Tambahkan data ke ReportViewer
            ReportViewer2.LocalReport.DataSources.Add(New ReportDataSource(datasetName, dataset.Tables(datasetName)))
        Next

        Dim AWAL As String = String.Empty
        Dim RUBAH As String = String.Empty
        Dim AKHIR As String = "SALDO AKHIR"

        Select Case True
            Case RbtSemua.Checked
                AWAL = "SALDO AWAL"
                RUBAH = "PERIODE INI"
            Case RbtTanggal.Checked
                AWAL = "TANGGAL LALU"
                RUBAH = "TANGGAL INI"
            Case RbtBulan.Checked
                AWAL = "BULAN LALU"
                RUBAH = "BULAN INI"
        End Select

        ' Set parameter laporan
        Dim parameters As ReportParameter() = {
            New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN),
            New ReportParameter("PERIODE", Periode),
            New ReportParameter("AWAL", AWAL),
            New ReportParameter("RUBAH", RUBAH),
            New ReportParameter("AKHIR", AKHIR),
            New ReportParameter("USER", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
        }

        ReportViewer2.LocalReport.SetParameters(parameters)
        ReportViewer2.RefreshReport()
    End Sub


    Private Sub TampilNeracaLajur()
        ReportViewer3.LocalReport.DataSources.Clear()
        Dim query As String = "SELECT KODE_AKUN, NAMA_AKUN, AKUN_NRLR, SALDO_SEBELUMNYA, S_DEBET, S_KREDIT, SALDO_AKHIR " &
                              "FROM temp_datareferensi " &
                              "ORDER BY KODE_AKUN"

        Using command As New MySqlCommand(query, conn)

            Using reader As MySqlDataReader = command.ExecuteReader()
                Dim dataset As New DataSetKL()
                dataset.Load(reader, LoadOption.OverwriteChanges, "NeracaLajur")

                ReportViewer3.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("NeracaLajur")))
                ' Set parameter laporan
                Dim parameters As ReportParameter() = {
                    New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN),
                     New ReportParameter("PERIODE", Periode)
                }

                ReportViewer3.LocalReport.SetParameters(parameters)
                ReportViewer3.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub FormLaporan_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer2.LocalReport.DataSources.Clear()
        ReportViewer3.LocalReport.DataSources.Clear()
    End Sub


    Private Sub FormLapNeracaLR_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnTampilNeraca.PerformClick()
        End Select
    End Sub

End Class
