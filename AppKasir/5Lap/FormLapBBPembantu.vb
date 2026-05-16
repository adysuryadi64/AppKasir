Imports Microsoft.Reporting.WinForms

Public Class FormLapBBPembantu

    Public Property JenisLaporan As String = "Piutang"

    Private Sub FormLapBBPembantu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Label5/LblHeaderForm = peringatan (DarkRed)
        ModuleTheme.SetWarnaLabelWarning(Label5, LblHeaderForm)
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        TampilEntitas()
        AturJudul()
        ReportViewer1.LocalReport.DataSources.Clear()
    End Sub

    Private Sub AturJudul()
        If JenisLaporan = "Hutang" Then
            LblHeaderForm.Text = "BUKU BESAR PEMBANTU HUTANG (PER SUPPLIER)"
            LabelEntitas.Text = "Supplier :"
        Else
            LblHeaderForm.Text = "BUKU BESAR PEMBANTU PIUTANG (PER PELANGGAN)"
            LabelEntitas.Text = "Pelanggan :"
        End If
    End Sub

    Private Sub TampilEntitas()
        CmbEntitas.Items.Clear()
        CmbEntitas.Items.Add("SEMUA")
        Dim query As String
        If JenisLaporan = "Hutang" Then
            query = "SELECT DISTINCT NAMA_SUPLIYER FROM pembelian WHERE STATUS_JUAL = 'TERHUTANG' ORDER BY NAMA_SUPLIYER"
        Else
            query = "SELECT DISTINCT NAMA_PELANGGAN FROM penjualan WHERE STATUS_TRANSAKSI = 'TERHUTANG' ORDER BY NAMA_PELANGGAN"
        End If
        Using cmd As New MySqlCommand(query, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    CmbEntitas.Items.Add(rd(0).ToString())
                End While
            End Using
        End Using
        CmbEntitas.SelectedIndex = 0
    End Sub

    Private Sub BtnTampil_Click(sender As Object, e As EventArgs) Handles BtnTampil.Click
        Try
            Cursor = Cursors.WaitCursor
            Dim tAwal As Date = DTPAwal.Value.Date
            Dim tAkhir As Date = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
            Dim entitas As String = If(CmbEntitas.SelectedIndex <= 0, "", CmbEntitas.Text)

            ' Bersihkan temp table
            Using cmd As New MySqlCommand("DELETE FROM temp_bbpembantu", conn)
                cmd.ExecuteNonQuery()
            End Using

            If JenisLaporan = "Hutang" Then
                IsiTempHutang(tAwal, tAkhir, entitas)
            Else
                IsiTempPiutang(tAwal, tAkhir, entitas)
            End If

            HitungSaldo()
            TampilLaporan(tAwal, tAkhir, entitas)
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ' ==================== ISI TEMP PIUTANG ====================
    Private Sub IsiTempPiutang(tAwal As Date, tAkhir As Date, entitas As String)
        Dim filter As String = If(entitas = "", "%", "%" & entitas & "%")
        Dim nomor As Integer = 1

        ' 1. Penjualan kredit → Debet piutang
        Dim q1 As String =
            "SELECT TGL_TRANSAKSI, ID_PENJUALAN, NAMA_PELANGGAN, SISA_TAGIHAN " &
            "FROM penjualan " &
            "WHERE STATUS_TRANSAKSI = 'TERHUTANG' " &
            "  AND TGL_TRANSAKSI BETWEEN @tAwal AND @tAkhir " &
            "  AND NAMA_PELANGGAN LIKE @entitas " &
            "ORDER BY NAMA_PELANGGAN, TGL_TRANSAKSI"
        Using cmd As New MySqlCommand(q1, conn)
            cmd.Parameters.AddWithValue("@tAwal", tAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tAkhir", tAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@entitas", filter)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    InsertTemp(nomor, rd("TGL_TRANSAKSI"), rd("ID_PENJUALAN").ToString(),
                               rd("NAMA_PELANGGAN").ToString(), "Penjualan Kredit",
                               CDec(rd("SISA_TAGIHAN")), 0)
                    nomor += 1
                End While
            End Using
        End Using

        ' 2. Retur penjualan → Kredit piutang
        Dim q2 As String =
            "SELECT rj.TGL_RETUR_JUAL, rj.ID_RETUR_PENJUALAN, rj.NAMA_PELANGGAN, rj.TOTAL_RUPIAH " &
            "FROM retur_penjualan rj " &
            "INNER JOIN penjualan p ON rj.ID_PENJUALAN = p.ID_PENJUALAN " &
            "WHERE p.STATUS_TRANSAKSI = 'TERHUTANG' " &
            "  AND rj.TGL_RETUR_JUAL BETWEEN @tAwal AND @tAkhir " &
            "  AND rj.NAMA_PELANGGAN LIKE @entitas " &
            "ORDER BY rj.NAMA_PELANGGAN, rj.TGL_RETUR_JUAL"
        Using cmd As New MySqlCommand(q2, conn)
            cmd.Parameters.AddWithValue("@tAwal", tAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tAkhir", tAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@entitas", filter)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    InsertTemp(nomor, rd("TGL_RETUR_JUAL"), rd("ID_RETUR_PENJUALAN").ToString(),
                               rd("NAMA_PELANGGAN").ToString(), "Retur Penjualan",
                               0, CDec(rd("TOTAL_RUPIAH")))
                    nomor += 1
                End While
            End Using
        End Using

        ' 3. Bayar piutang → Kredit piutang — filter JENIS='BAYAR' agar baris TIMBUL tidak ikut
        Dim q3 As String =
            "SELECT pd.TANGGAL_BAYAR, pd.ID_BAYAR, pd.NAMA, pd.PEMBAYARAN " &
            "FROM piutang_detail pd " &
            "WHERE pd.JENIS = 'BAYAR' " &
            "  AND pd.TANGGAL_BAYAR BETWEEN @tAwal AND @tAkhir " &
            "  AND pd.NAMA LIKE @entitas " &
            "ORDER BY pd.NAMA, pd.TANGGAL_BAYAR"
        Using cmd As New MySqlCommand(q3, conn)
            cmd.Parameters.AddWithValue("@tAwal", tAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tAkhir", tAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@entitas", filter)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    InsertTemp(nomor, rd("TANGGAL_BAYAR"), rd("ID_BAYAR").ToString(),
                               rd("NAMA").ToString(), "Bayar Piutang",
                               0, CDec(rd("PEMBAYARAN")))
                    nomor += 1
                End While
            End Using
        End Using
    End Sub

    ' ==================== ISI TEMP HUTANG ====================
    Private Sub IsiTempHutang(tAwal As Date, tAkhir As Date, entitas As String)
        Dim filter As String = If(entitas = "", "%", "%" & entitas & "%")
        Dim nomor As Integer = 1

        ' 1. Pembelian kredit → Kredit hutang
        Dim q1 As String =
            "SELECT TGL_BELI, ID_PEMBELIAN, NAMA_SUPLIYER, TAGIHAN " &
            "FROM pembelian " &
            "WHERE STATUS_JUAL = 'TERHUTANG' " &
            "  AND TGL_BELI BETWEEN @tAwal AND @tAkhir " &
            "  AND NAMA_SUPLIYER LIKE @entitas " &
            "ORDER BY NAMA_SUPLIYER, TGL_BELI"
        Using cmd As New MySqlCommand(q1, conn)
            cmd.Parameters.AddWithValue("@tAwal", tAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tAkhir", tAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@entitas", filter)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    InsertTemp(nomor, rd("TGL_BELI"), rd("ID_PEMBELIAN").ToString(),
                               rd("NAMA_SUPLIYER").ToString(), "Pembelian Kredit",
                               0, CDec(rd("TAGIHAN")))
                    nomor += 1
                End While
            End Using
        End Using

        ' 2. Retur pembelian → Debet hutang
        Dim q2 As String =
            "SELECT rb.TGL_RETUR_BELI, rb.ID_RETUR_PEMBELIAN, rb.NAMA_SUPPLIER, rb.TOTAL_RUPIAH " &
            "FROM retur_pembelian rb " &
            "INNER JOIN pembelian p ON rb.ID_PEMBELIAN = p.ID_PEMBELIAN " &
            "WHERE p.STATUS_JUAL = 'TERHUTANG' " &
            "  AND rb.TGL_RETUR_BELI BETWEEN @tAwal AND @tAkhir " &
            "  AND rb.NAMA_SUPPLIER LIKE @entitas " &
            "ORDER BY rb.NAMA_SUPPLIER, rb.TGL_RETUR_BELI"
        Using cmd As New MySqlCommand(q2, conn)
            cmd.Parameters.AddWithValue("@tAwal", tAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tAkhir", tAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@entitas", filter)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    InsertTemp(nomor, rd("TGL_RETUR_BELI"), rd("ID_RETUR_PEMBELIAN").ToString(),
                               rd("NAMA_SUPPLIER").ToString(), "Retur Pembelian",
                               CDec(rd("TOTAL_RUPIAH")), 0)
                    nomor += 1
                End While
            End Using
        End Using

        ' 3. Bayar hutang → Debet hutang — filter JENIS='BAYAR' agar baris TIMBUL tidak ikut
        Dim q3 As String =
            "SELECT hd.TANGGAL_BAYAR, hd.ID_BAYAR, hd.NAMA, hd.PEMBAYARAN " &
            "FROM hutang_detail hd " &
            "WHERE hd.JENIS = 'BAYAR' " &
            "  AND hd.TANGGAL_BAYAR BETWEEN @tAwal AND @tAkhir " &
            "  AND hd.NAMA LIKE @entitas " &
            "ORDER BY hd.NAMA, hd.TANGGAL_BAYAR"
        Using cmd As New MySqlCommand(q3, conn)
            cmd.Parameters.AddWithValue("@tAwal", tAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tAkhir", tAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@entitas", filter)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    InsertTemp(nomor, rd("TANGGAL_BAYAR"), rd("ID_BAYAR").ToString(),
                               rd("NAMA").ToString(), "Bayar Hutang",
                               CDec(rd("PEMBAYARAN")), 0)
                    nomor += 1
                End While
            End Using
        End Using
    End Sub

    Private Sub InsertTemp(nomor As Integer, tanggal As Object, nota As String,
                           entitas As String, keterangan As String,
                           debet As Decimal, kredit As Decimal)
        Dim q As String =
            "INSERT INTO temp_bbpembantu (NOMOR, TANGGAL, NOTA, ENTITAS, KETERANGAN, DEBET, KREDIT, SALDO) " &
            "VALUES (@n, @t, @nota, @ent, @ket, @d, @k, 0)"
        Using cmd As New MySqlCommand(q, conn)
            cmd.Parameters.AddWithValue("@n", nomor)
            cmd.Parameters.AddWithValue("@t", tanggal)
            cmd.Parameters.AddWithValue("@nota", nota)
            cmd.Parameters.AddWithValue("@ent", entitas)
            cmd.Parameters.AddWithValue("@ket", keterangan)
            cmd.Parameters.AddWithValue("@d", debet)
            cmd.Parameters.AddWithValue("@k", kredit)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' Hitung saldo berjalan per entitas (reset tiap ganti entitas)
    Private Sub HitungSaldo()
        Dim dataList As New List(Of (id As Integer, debet As Decimal, kredit As Decimal, entitas As String))

        Using cmd As New MySqlCommand(
            "SELECT ID, DEBET, KREDIT, ENTITAS FROM temp_bbpembantu ORDER BY ENTITAS, TANGGAL, ID", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    dataList.Add((CInt(rd("ID")), CDec(rd("DEBET")), CDec(rd("KREDIT")), rd("ENTITAS").ToString()))
                End While
            End Using
        End Using

        Dim saldo As Decimal = 0
        Dim lastEntitas As String = ""
        For Each row In dataList
            If row.entitas <> lastEntitas Then
                saldo = 0
                lastEntitas = row.entitas
            End If
            If JenisLaporan = "Hutang" Then
                saldo += row.kredit - row.debet   ' hutang: kredit nambah, debet kurang
            Else
                saldo += row.debet - row.kredit   ' piutang: debet nambah, kredit kurang
            End If
            Using cmd As New MySqlCommand("UPDATE temp_bbpembantu SET SALDO = @s WHERE ID = @id", conn)
                cmd.Parameters.AddWithValue("@s", saldo)
                cmd.Parameters.AddWithValue("@id", row.id)
                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub

    Private Sub TampilLaporan(tAwal As Date, tAkhir As Date, entitas As String)
        ReportViewer1.LocalReport.DataSources.Clear()

        Using cmd As New MySqlCommand(
            "SELECT NOMOR, TANGGAL, NOTA, ENTITAS, KETERANGAN, DEBET, KREDIT, SALDO " &
            "FROM temp_bbpembantu ORDER BY ENTITAS, TANGGAL, ID", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using ds As New DataSet()
                    ds.Load(rd, LoadOption.OverwriteChanges, "BBPembantu")

                    Dim judulParam As String = LblHeaderForm.Text
                    Dim entitasParam As String = If(entitas = "", "SEMUA", entitas)
                    Dim periodeParam As String = tAwal.ToString("dd/MM/yyyy") & " s/d " & tAkhir.Date.ToString("dd/MM/yyyy")

                    ' Hitung total untuk panel bawah
                    Dim totalD As Decimal = 0, totalK As Decimal = 0
                    For Each row As DataRow In ds.Tables("BBPembantu").Rows
                        totalD += CDec(row("DEBET"))
                        totalK += CDec(row("KREDIT"))
                    Next
                    TxtTotalDebet.Text = totalD.ToString("N0")
                    TxtTotalKredit.Text = totalK.ToString("N0")
                    TxtSaldoAkhir.Text = Math.Abs(totalD - totalK).ToString("N0")

                    ReportViewer1.LocalReport.ReportEmbeddedResource = "AppKasir.ReportBBPembantu.rdlc"
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("BBPembantu")))
                    ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                        New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                        New ReportParameter("Judul", judulParam),
                        New ReportParameter("Entitas", If(entitas = "", "Semua", entitas)),
                        New ReportParameter("Periode", "Periode : " & periodeParam),
                        New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
                    })
                    ReportViewer1.RefreshReport()
                End Using
            End Using
        End Using
    End Sub

    Private Sub CmbEntitas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbEntitas.SelectedIndexChanged
        ReportViewer1.LocalReport.DataSources.Clear()
        TxtTotalDebet.Text = "0"
        TxtTotalKredit.Text = "0"
        TxtSaldoAkhir.Text = "0"
    End Sub

    Private Sub FormLapBBPembantu_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnTampil.PerformClick()
        End Select
    End Sub

End Class
