Imports Microsoft.Reporting.WinForms

Public Class FormHistory

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim batas As Integer = Integer.Parse(ComboBox1.Text)

        ' Mengambil tanggal batas dari DateTimePicker
        Dim tanggalBatas As DateTime = DateTimePicker2.Value.AddMonths(-batas)

        Dim query As String = "DELETE FROM History WHERE Tanggal < @tanggalBatas"
        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@tanggalBatas", tanggalBatas.ToString("yyyy-MM-dd HH:mm:ss"))
            command.ExecuteNonQuery()
        End Using

        ' Tampilkan pesan sukses
        MessageBox.Show("History lebih dari " & batas & " bulan telah dihapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub


    Private Sub FormHistory_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Mengatur DateTimePicker ke tanggal saat ini
        DateTimePicker1.Value = DateTime.Now
        DateTimePicker2.Value = DateTime.Now
        ReportViewer1.RefreshReport()
    End Sub

    Private Sub BtnPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPreview.Click
        ReportViewer1.LocalReport.DataSources.Clear()
        Dim tanggalAwal As Date = DateTimePicker1.Value.Date
        Dim tanggalAkhir As Date = DateTimePicker1.Value.Date.AddDays(1).AddTicks(-1)

        ' Ambil data History
        Dim queryHistory As String = "SELECT TANGGAL, AKSI FROM History WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir ORDER BY TANGGAL"

        Using cmdHistory As New MySqlCommand(queryHistory, conn)
            cmdHistory.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHistory.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rd As MySqlDataReader = cmdHistory.ExecuteReader()
                Using datasetHistory As New DataSetKL()
                    datasetHistory.Load(rd, LoadOption.OverwriteChanges, "History")
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetHistory.Tables("History")))
                End Using
            End Using
        End Using

        ' Set parameter laporan
        Dim parameters As ReportParameter() = New ReportParameter(0) {}
        parameters(0) = New ReportParameter("NAMATOKO", TxtPerusahaan.Text)

        ReportViewer1.LocalReport.SetParameters(parameters)

        ReportViewer1.RefreshReport()
    End Sub
End Class