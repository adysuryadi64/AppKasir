Imports Microsoft.Reporting.WinForms

Public Class FormLapStokLampau

    Private Sub FormLapStokLampau_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' LblHeaderForm = peringatan stok lampau (DarkRed)
        ModuleTheme.SetWarnaLabelWarning(LblHeaderForm)
        DTPTanggal.Value = Date.Today.AddDays(-1)
        CmbLokasi.SelectedIndex = 0
        ReportViewer1.LocalReport.DataSources.Clear()
    End Sub

    Private Sub BtnTampil_Click(sender As Object, e As EventArgs) Handles BtnTampil.Click
        Try
            Cursor = Cursors.WaitCursor
            Dim tgl As Date = DTPTanggal.Value.Date.AddDays(1).AddTicks(-1) ' akhir hari
            Dim lokasi As String = CmbLokasi.Text  ' TOKO / GUDANG / SEMUA

            TampilLaporan(tgl, lokasi)
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub TampilLaporan(tgl As Date, lokasi As String)
        ' Logika:
        '   Stok pada tgl T = STOK_AWAL_TOKO/GUDANG + SUM(historybarang s/d T)
        '   STOK_AWAL_TOKO diisi saat posting (stok saat ini - total mutasi historybarang)
        '   historybarang.TOTAL_QTY sudah signed (+masuk / -keluar)

        Dim lokasiFilter As String
        Dim kolom As String
        Select Case lokasi
            Case "TOKO"
                lokasiFilter = "AND h.LOKASI = 'TOKO'"
                kolom = "TOKO"
            Case "GUDANG"
                lokasiFilter = "AND h.LOKASI = 'GUDANG'"
                kolom = "GUDANG"
            Case Else ' SEMUA
                lokasiFilter = ""
                kolom = "SEMUA"
        End Select

        Dim query As String
        If lokasi = "SEMUA" Then
            query =
                "SELECT b.ID_BARANG, b.NAMA_BARANG, b.NAMA_KATEGORI, b.SATUAN_STOK, " &
                "  b.HARGA_BELI, " &
                "  COALESCE(b.STOK_AWAL_TOKO, 0)   + COALESCE(SUM(CASE WHEN h.LOKASI='TOKO'   THEN h.TOTAL_QTY ELSE 0 END), 0) AS STOK_TOKO, " &
                "  COALESCE(b.STOK_AWAL_GUDANG, 0) + COALESCE(SUM(CASE WHEN h.LOKASI='GUDANG' THEN h.TOTAL_QTY ELSE 0 END), 0) AS STOK_GUDANG " &
                "FROM tbl_barang b " &
                "LEFT JOIN historybarang h ON h.ID_BARANG = b.ID_BARANG AND h.TANGGAL <= @tgl " &
                "WHERE b.STATUS = 'Aktif' " &
                "GROUP BY b.ID_BARANG, b.NAMA_BARANG, b.NAMA_KATEGORI, b.SATUAN_STOK, b.HARGA_BELI, b.STOK_AWAL_TOKO, b.STOK_AWAL_GUDANG " &
                "ORDER BY b.NAMA_KATEGORI, b.NAMA_BARANG"
        Else
            Dim awalKol As String = If(lokasi = "TOKO", "b.STOK_AWAL_TOKO", "b.STOK_AWAL_GUDANG")
            query =
                "SELECT b.ID_BARANG, b.NAMA_BARANG, b.NAMA_KATEGORI, b.SATUAN_STOK, " &
                "  b.HARGA_BELI, " &
                "  COALESCE(" & awalKol & ", 0) + COALESCE(SUM(h.TOTAL_QTY), 0) AS STOK_TOKO, " &
                "  0 AS STOK_GUDANG " &
                "FROM tbl_barang b " &
                "LEFT JOIN historybarang h ON h.ID_BARANG = b.ID_BARANG AND h.TANGGAL <= @tgl " &
                "  AND h.LOKASI = '" & lokasi & "' " &
                "WHERE b.STATUS = 'Aktif' " &
                "GROUP BY b.ID_BARANG, b.NAMA_BARANG, b.NAMA_KATEGORI, b.SATUAN_STOK, b.HARGA_BELI, " & awalKol &
                " ORDER BY b.NAMA_KATEGORI, b.NAMA_BARANG"
        End If

        ReportViewer1.LocalReport.DataSources.Clear()
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tgl", tgl.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using ds As New DataSet()
                    ds.Load(rd, LoadOption.OverwriteChanges, "StokLampau")

                    ' Hitung total untuk panel bawah
                    Dim totalItem As Integer = ds.Tables("StokLampau").Rows.Count
                    Dim totalNilaiToko As Decimal = 0
                    Dim totalNilaiGudang As Decimal = 0
                    For Each row As DataRow In ds.Tables("StokLampau").Rows
                        Dim hpp As Decimal = CDec(row("HARGA_BELI"))
                        totalNilaiToko += CDec(row("STOK_TOKO")) * hpp
                        totalNilaiGudang += CDec(row("STOK_GUDANG")) * hpp
                    Next
                    LblTotalItem.Text = totalItem.ToString("N0") & " item"
                    TxtNilaiToko.Text = totalNilaiToko.ToString("N0")
                    TxtNilaiGudang.Text = totalNilaiGudang.ToString("N0")

                    Dim judulLokasi As String = If(lokasi = "SEMUA", "Toko & Gudang", lokasi)
                    Dim periodeStr As String = "Per Tanggal : " & DTPTanggal.Value.ToString("dd MMMM yyyy")

                    ReportViewer1.LocalReport.ReportEmbeddedResource = "AppKasir.ReportStokLampau.rdlc"
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("StokLampau")))
                    ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                        New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                        New ReportParameter("Periode", periodeStr),
                        New ReportParameter("Lokasi", judulLokasi),
                        New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
                    })
                    ReportViewer1.RefreshReport()
                End Using
            End Using
        End Using
    End Sub

    Private Sub FormLapStokLampau_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnTampil.PerformClick()
        End Select
    End Sub

End Class
