Imports Microsoft.Reporting.WinForms

Public Class NotaPembelian

    ' ── Entry point statis — dipanggil dari ModulePrinterBeli ────
    Public Shared Sub TampilkanNota(idPembelian As String)
        Dim frm As New NotaPembelian()
        frm.TxtIdPembelian.Text = idPembelian
        frm.ShowDialog()
    End Sub

    Private Sub NotaPembelian_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ReportViewer1.LocalReport.DataSources.Clear()
        AMbildata()
    End Sub

    Private Sub AMbildata()
        Dim sql As String = "SELECT NAMA_SUPLIYER, NOTA_PEMBELIAN, TGL_BELI, LOKASI, PEMBAYARAN, TAGIHAN, JATUH_TEMPO, TGL_BAYAR, NOMINALBAYAR, STATUS_TRANSAKSI_BELI, ID_USER, ID_KOMPUTER FROM pembelian WHERE ID_PEMBELIAN = @ID_PEMBELIAN"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@ID_PEMBELIAN", TxtIdPembelian.Text.Trim())

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Ambil nilai-nilai dari database
                    Dim namaSupliyer As String = rd("NAMA_SUPLIYER").ToString()
                    Dim notaPembelian As String = rd("NOTA_PEMBELIAN").ToString()
                    Dim tglBeliString As String = rd("TGL_BELI").ToString()
                    Dim tglBeli As String = ""
                    Dim tglBeliDateTime As DateTime
                    If DateTime.TryParse(tglBeliString, tglBeliDateTime) Then
                        tglBeli = tglBeliDateTime.ToString("dd-MM-yyyy HH:mm:ss")
                    End If
                    Dim tempatSimpan As String = rd("LOKASI").ToString()
                    Dim pembayaran As String = If(String.IsNullOrEmpty(rd("PEMBAYARAN").ToString()), "0", Decimal.Parse(rd("PEMBAYARAN").ToString()).ToString("N0"))
                    Dim tagihan As String = If(String.IsNullOrEmpty(rd("TAGIHAN").ToString()), "0", Decimal.Parse(rd("TAGIHAN").ToString()).ToString("N0"))
                    Dim jatuhTempoString As String = rd("JATUH_TEMPO").ToString()
                    Dim jatuhTempo As String = ""
                    Dim jatuhTempoDateTime As DateTime
                    If DateTime.TryParse(jatuhTempoString, jatuhTempoDateTime) Then
                        jatuhTempo = jatuhTempoDateTime.ToString("dd-MM-yyyy")
                    End If
                    Dim tglBayarString As String = rd("TGL_BAYAR").ToString()
                    Dim tglBayar As String = ""
                    Dim tglBayarDateTime As DateTime
                    If DateTime.TryParse(tglBayarString, tglBayarDateTime) Then
                        tglBayar = tglBayarDateTime.ToString("dd-MM-yyyy HH:mm:ss")
                    End If
                    Dim nominalBayar As String = If(String.IsNullOrEmpty(rd("NOMINALBAYAR").ToString()), "0", Decimal.Parse(rd("NOMINALBAYAR").ToString()).ToString("N0"))
                    Dim statusTransaksiBeli As String = rd("STATUS_TRANSAKSI_BELI").ToString()
                    Dim idUser As String = rd("ID_USER").ToString()
                    Dim idKomputer As String = rd("ID_KOMPUTER").ToString()

                    ' Set parameter laporan
                    Dim parameters As ReportParameter() = New ReportParameter(12) {}
                    parameters(0) = New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN)
                    parameters(1) = New ReportParameter("namaSupliyer", namaSupliyer)
                    parameters(2) = New ReportParameter("notaPembelian", notaPembelian)
                    parameters(3) = New ReportParameter("tglBeli", tglBeli)
                    parameters(4) = New ReportParameter("tempatSimpan", tempatSimpan)
                    parameters(5) = New ReportParameter("pembayaran", pembayaran)
                    parameters(6) = New ReportParameter("tagihan", tagihan)
                    parameters(7) = New ReportParameter("jatuhTempo", jatuhTempo)
                    parameters(8) = New ReportParameter("tglBayar", tglBayar)
                    parameters(9) = New ReportParameter("nominalBayar", nominalBayar)
                    parameters(10) = New ReportParameter("statusTransaksiBeli", statusTransaksiBeli)
                    parameters(11) = New ReportParameter("idUser", idUser & " / " & idKomputer)
                    parameters(12) = New ReportParameter("NotaBeli", TxtIdPembelian.Text)

                    ' Bersihkan data sumber laporan sebelum memuat data baru
                    ReportViewer1.LocalReport.DataSources.Clear()
                    ReportViewer1.LocalReport.SetParameters(parameters)
                End If
            End Using
        End Using

        ' Ambil data NotaBeli
        Dim queryNotaBeli As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, TOTAL FROM pembelian_detail WHERE FAKTUR_BELI = @FAKTUR_BELI"
        Using cmdNotaBeli As New MySqlCommand(queryNotaBeli, conn)
            cmdNotaBeli.Parameters.AddWithValue("@FAKTUR_BELI", TxtIdPembelian.Text.Trim())
            Using rd As MySqlDataReader = cmdNotaBeli.ExecuteReader()
                Using datasetNotaBeli As New DataSetKL()
                    datasetNotaBeli.Load(rd, LoadOption.OverwriteChanges, "NotaPembelian")
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetNotaBeli.Tables("NotaPembelian")))
                End Using
            End Using
        End Using
        ReportViewer1.RefreshReport()
    End Sub

    Private Sub NotaPembelian_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        TxtIdPembelian.Text = ""
    End Sub
End Class