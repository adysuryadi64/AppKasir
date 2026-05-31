Imports System.Globalization
Imports Microsoft.Reporting.WinForms

Public Class FormLapPoin

    Private Sub FormLapPoin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        DtpAwal.Value = New Date(DateTime.Now.Year, DateTime.Now.Month, 1)
        DtpAkhir.Value = DateTime.Now
    End Sub

    Private Sub FormLapPoin_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        PilihPanelBerdasarkanMode()
    End Sub

    Private Sub PilihPanelBerdasarkanMode()
        Panel1.Visible = False
        Panel2.Visible = False
        Panel3.Visible = False

        Select Case LblHeader.Text
            Case "LAPORAN MUTASI POIN"
                Panel1.Visible = True
            Case "LAPORAN REKAP TUKAR POIN"
                Panel2.Visible = True
            Case "LAPORAN SALDO POIN"
                Panel3.Visible = True
        End Select
    End Sub

    Private Sub BtnTampilkan_Click(sender As Object, e As EventArgs) Handles BtnTampilkan.Click
        Dim tAwal As Date = DtpAwal.Value.Date
        Dim tAkhir As Date = DtpAkhir.Value.Date.AddDays(1).AddTicks(-1)
        Dim periode As String = tAwal.ToString("dd MMMM yyyy", New CultureInfo("id-ID")) & " s.d. " & DtpAkhir.Value.ToString("dd MMMM yyyy", New CultureInfo("id-ID"))

        Select Case LblHeader.Text
            Case "LAPORAN MUTASI POIN"
                TampilMutasiPoin(tAwal, tAkhir, periode)
            Case "LAPORAN REKAP TUKAR POIN"
                TampilRekapTukarPoin(tAwal, tAkhir, periode)
            Case "LAPORAN SALDO POIN"
                TampilSaldoPoin(periode)
        End Select
    End Sub

    Private Sub TampilMutasiPoin(tAwal As Date, tAkhir As Date, periode As String)
        Dim rv As ReportViewer = ReportViewer1
        rv.LocalReport.DataSources.Clear()

        Using cmd As New MySqlCommand(
            "SELECT pl.CREATED_AT AS Tanggal, pl.NO_REFERENSI AS NoReferensi, " &
            "COALESCE(p.NAMA, '') AS Pelanggan, " &
            "pl.TIPE AS Tipe, pl.JUMLAH_POIN AS JumlahPoin, " &
            "COALESCE(pl.KETERANGAN, '') AS Keterangan, COALESCE(pl.ID_USER, '') AS 'User' " &
            "FROM poin_ledger pl " &
            "LEFT JOIN tbl_pelanggan p ON pl.KODE_PELANGGAN = p.KODE " &
            "WHERE pl.CREATED_AT BETWEEN @dari AND @sampai " &
            "ORDER BY pl.CREATED_AT DESC", conn)
            cmd.Parameters.AddWithValue("@dari", tAwal)
            cmd.Parameters.AddWithValue("@sampai", tAkhir)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using ds As New DataSet()
                    ds.Load(rd, LoadOption.OverwriteChanges, "DataSet1")
                    Dim dtConverted As DataTable = ConvertColumnToDateTime(ds.Tables("DataSet1"), "Tanggal")
                    rv.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtConverted))
                End Using
            End Using
        End Using

        Dim param As New List(Of ReportParameter) From {
            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
            New ReportParameter("Judul", LblHeader.Text),
            New ReportParameter("Entitas", FormUtama.StatusLokasi.Text),
            New ReportParameter("Periode", "Periode: " & periode),
            New ReportParameter("Kasir", FormUtama.StatusNamaUser.Text)
        }
        rv.LocalReport.SetParameters(param)
        rv.RefreshReport()
    End Sub

    Private Sub TampilRekapTukarPoin(tAwal As Date, tAkhir As Date, periode As String)
        Dim rv As ReportViewer = ReportViewer2
        rv.LocalReport.DataSources.Clear()

        Using cmd As New MySqlCommand(
            "SELECT COALESCE(h.NAMA_BARANG, '') AS NamaBarang, " &
            "COALESCE(h.QTY, 0) AS Jumlah, " &
            "COALESCE(h.QTY * pb.HARGA_POIN, 0) AS TotalPoin, " &
            "COALESCE(pl.CREATED_AT, h.TANGGAL) AS TanggalTransaksi " &
            "FROM poin_ledger pl " &
            "LEFT JOIN HistoryBarang h ON pl.NO_REFERENSI = h.FAKTUR " &
            "LEFT JOIN poin_barang pb ON h.ID_BARANG = pb.ID_BARANG " &
            "WHERE pl.TIPE = 'REDEEM' " &
            "AND pl.CREATED_AT BETWEEN @dari AND @sampai " &
            "ORDER BY pl.CREATED_AT DESC, h.NAMA_BARANG", conn)
            cmd.Parameters.AddWithValue("@dari", tAwal)
            cmd.Parameters.AddWithValue("@sampai", tAkhir)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using ds As New DataSet()
                    ds.Load(rd, LoadOption.OverwriteChanges, "DataSet1")
                    Dim dtConverted As DataTable = ConvertColumnToDateTime(ds.Tables("DataSet1"), "TanggalTransaksi")
                    rv.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dtConverted))
                End Using
            End Using
        End Using

        Dim param As New List(Of ReportParameter) From {
            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
            New ReportParameter("Judul", LblHeader.Text),
            New ReportParameter("Entitas", FormUtama.StatusLokasi.Text),
            New ReportParameter("Periode", "Periode: " & periode),
            New ReportParameter("Kasir", FormUtama.StatusNamaUser.Text)
        }
        rv.LocalReport.SetParameters(param)
        rv.RefreshReport()
    End Sub

    Private Sub TampilSaldoPoin(periode As String)
        Dim rv As ReportViewer = ReportViewer3
        rv.LocalReport.DataSources.Clear()

        Using cmd As New MySqlCommand(
            "SELECT p.NAMA AS Pelanggan, " &
            "COALESCE(SUM(CASE WHEN pl.TIPE = 'EARN' THEN pl.JUMLAH_POIN ELSE 0 END), 0) AS TotalPoinMasuk, " &
            "COALESCE(SUM(CASE WHEN pl.TIPE IN ('REDEEM','VOID_EARN') THEN ABS(pl.JUMLAH_POIN) ELSE 0 END), 0) AS TotalPoinKeluar, " &
            "COALESCE(p.SALDO_POIN, 0) AS SaldoAkhir " &
            "FROM tbl_pelanggan p " &
            "LEFT JOIN poin_ledger pl ON p.KODE = pl.KODE_PELANGGAN " &
            "GROUP BY p.KODE, p.NAMA, p.SALDO_POIN " &
            "ORDER BY p.NAMA ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using ds As New DataSet()
                    ds.Load(rd, LoadOption.OverwriteChanges, "DataSet1")
                    rv.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("DataSet1")))
                End Using
            End Using
        End Using

        Dim param As New List(Of ReportParameter) From {
            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
            New ReportParameter("Judul", LblHeader.Text),
            New ReportParameter("Entitas", FormUtama.StatusLokasi.Text),
            New ReportParameter("Periode", "Data per: " & DateTime.Now.ToString("dd MMMM yyyy HH:mm", New CultureInfo("id-ID"))),
            New ReportParameter("Kasir", FormUtama.StatusNamaUser.Text)
        }
        rv.LocalReport.SetParameters(param)
        rv.RefreshReport()
    End Sub

    Private Function ConvertColumnToDateTime(dt As DataTable, columnName As String) As DataTable
        If dt Is Nothing OrElse Not dt.Columns.Contains(columnName) Then
            Return dt
        End If

        Dim dtCloned As DataTable = dt.Clone()
        dtCloned.Columns(columnName).DataType = GetType(Date)

        For Each row As DataRow In dt.Rows
            Dim newRow As DataRow = dtCloned.NewRow()
            For Each col As DataColumn In dt.Columns
                If col.ColumnName = columnName Then
                    If row(col.ColumnName) Is DBNull.Value Then
                        newRow(col.ColumnName) = DBNull.Value
                    Else
                        Try
                            newRow(col.ColumnName) = Convert.ToDateTime(row(col.ColumnName))
                        Catch
                            newRow(col.ColumnName) = DBNull.Value
                        End Try
                    End If
                Else
                    newRow(col.ColumnName) = row(col.ColumnName)
                End If
            Next
            dtCloned.Rows.Add(newRow)
        Next

        Return dtCloned
    End Function

    Private Sub FormLapPoin_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer2.LocalReport.DataSources.Clear()
        ReportViewer3.LocalReport.DataSources.Clear()
    End Sub
End Class