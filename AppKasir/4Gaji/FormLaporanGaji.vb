Imports Microsoft.Reporting.WinForms

Public Class FormLaporanGaji
    Dim teksBulanTahunTerpilih As String

    Private Sub FormLaporanGaji_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.ReportViewer1.RefreshReport()
        MuatComboBoxBulanTahun(CmbBln, CmbThn)
    End Sub

    Private Sub CmbBulan_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbBln.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub CmbTahun_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbThn.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub PerbaruiTeksBulanTahunTerpilih()
        If Not String.IsNullOrEmpty(CmbBln.Text) Then
            teksBulanTahunTerpilih = CmbBln.Text & "/" & CmbThn.Text
            TampilkanDataGaji(teksBulanTahunTerpilih)
        End If
    End Sub

    Private Sub TampilkanDataGaji(ByVal teksBulanTahunTerpilih As String)
        ' Initialize the DataSet
        Dim ds As New DataSet()

        Dim query As String = "SELECT Nama, POKOK, KOMISI_JUAL, SUPIR_RP, HELPER_RP, LEMBUR_RP, " &
                       "TUNJANGAN, TRANSPORT, UANG_MAKAN, POT_BON, ANGSURAN, ABSEN_RP, " &
                       "ABSEN_KHUSUS_RP, TERLAMBAT_RP, POT_LAIN, PENDAPATAN, POTONGAN, " &
                       "TERIMA, REKENING " &
                       "FROM Gaji_karyawan WHERE BULAN LIKE ?"


        ' Using block to ensure the resources are disposed properly
        Using cmd As New MySqlCommand(query, conn)
            ' Add the parameter to the SQL query
            cmd.Parameters.AddWithValue("@BULAN", teksBulanTahunTerpilih)

            ' Using block to ensure the adapter is disposed properly
            Using adapter As New MySqlDataAdapter(cmd)
                ' Fill the DataSet with data from the database
                adapter.Fill(ds, "Laporan_Gaji_karyawan")
            End Using
        End Using

        ' Create a list to hold the report parameters
        Dim reportParams As New List(Of ReportParameter) From {
            New ReportParameter("TOKO", teksBulanTahunTerpilih & "                     " & NAMA_PERUSAHAAN),
            New ReportParameter("USER", FormUtama.StatusNamaUser.Text)
                   }




        ' Clear the existing DataSources
        ReportViewer1.LocalReport.DataSources.Clear()

        ' Add the new DataSource
        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("Laporan_Gaji_karyawan")))

        ' Set the parameters for the report
        ReportViewer1.LocalReport.SetParameters(reportParams)

        ' Refresh the ReportViewer
        ReportViewer1.RefreshReport()
    End Sub



    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class
