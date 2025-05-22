Imports Microsoft.Reporting.WinForms

Public Class FormLaporanGaji
    Dim teksBulanTahunTerpilih As String

    Private Sub FormLaporanGaji_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.ReportViewer1.RefreshReport()
        MuatComboBoxBulanTahun()
    End Sub

    Private Sub MuatComboBoxBulanTahun()
        ' Bersihkan item sebelum menambahkannya kembali
        CmbTahun.Items.Clear()

        ' Tambahkan tahun dari 2022 hingga tahun sekarang
        For i As Integer = 2022 To Year(Now)
            CmbTahun.Items.Add(i)
        Next

        ' Set tahun sekarang sebagai tahun default
        CmbTahun.SelectedItem = Year(Now)

        ' Bersihkan item sebelum menambahkannya kembali
        CmbBulan.Items.Clear()

        ' Tambahkan daftar bulan
        Dim daftarBulan As String() = {"Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember"}
        CmbBulan.Items.AddRange(daftarBulan)

        ' Set bulan sekarang sebagai bulan default
        CmbBulan.SelectedIndex = Month(Now) - 1 ' Index bulan dimulai dari 0, jadi dikurangi 1
    End Sub


    Private Sub CmbBulan_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbBulan.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub CmbTahun_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbTahun.SelectedIndexChanged
        PerbaruiTeksBulanTahunTerpilih()
    End Sub

    Private Sub PerbaruiTeksBulanTahunTerpilih()
        If Not String.IsNullOrEmpty(CmbBulan.Text) Then
            teksBulanTahunTerpilih = CmbBulan.Text & "/" & CmbTahun.Text
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
            New ReportParameter("USER", FormUtama.SLogin.Text)
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