Imports Microsoft.Reporting.WinForms

Public Class FormLapBon

    Private Sub FormLapBon_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.ReportViewer1.RefreshReport()
        TampilkanDataBon()
    End Sub

    Private Sub TampilkanDataBon()
        ' Initialize the DataSet
        Dim ds As New DataSet()

        ' Create the SQL query
        Dim sql As String = "SELECT Nama, Jabatan, SaldoAkhir FROM tbl_karyawan WHERE SaldoAkhir <> 0"

        ' Using block to ensure the resources are disposed properly
        Using cmd As New MySqlCommand(sql, conn)

            ' Using block to ensure the adapter is disposed properly
            Using adapter As New MySqlDataAdapter(cmd)
                ' Fill the DataSet with data from the database
                adapter.Fill(ds, "tbl_karyawan")
            End Using
        End Using


        ' Create a list to hold the report parameters
        Dim reportParams As New List(Of ReportParameter) From {
            New ReportParameter("TOKO", NAMA_PERUSAHAAN),
            New ReportParameter("USER", FormUtama.SLogin.Text)
        }

        ' Clear the existing DataSources
        ReportViewer1.LocalReport.DataSources.Clear()

        ' Add the new DataSource
        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("tbl_karyawan")))

        ' Set the parameters for the report
        ReportViewer1.LocalReport.SetParameters(reportParams)

        ' Refresh the ReportViewer
        ReportViewer1.RefreshReport()
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class
