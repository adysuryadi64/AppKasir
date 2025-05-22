Imports Microsoft.Reporting.WinForms

Public Class NotaSuratJalan

    Public Sub AmbilaDataSuratJalan(ByVal NOTA As String)

        ' Queries to get data from Surat_Jalan and Surat_Jalan_Detail
        Dim querySuratJalan As String = "SELECT TGL_PENGIRIMAN, TOTAL_RUPIAH, ARMADA, JENIS_ARMADA, SUPIR, HELPER1, HELPER2, ID_USER FROM Surat_Jalan WHERE NOTA LIKE @NOTA"
        Dim querySuratJalanDetail As String = "SELECT NOTA_BELANJA, NAMA_PELANGGAN, TANGGAL_BELANJA, NILAI_BELANJA, LOKASI FROM Surat_Jalan_Detail WHERE NOTA LIKE @NOTA"

        ' Create a new DataSet
        Dim ds As New DataSet()

        Try
            ' Define variables to hold data from Surat_Jalan
            Dim tglPengiriman As String = String.Empty
            Dim totalRupiah As String = String.Empty
            Dim armada As String = String.Empty
            Dim supir As String = String.Empty
            Dim helper1 As String = String.Empty
            Dim helper2 As String = String.Empty
            Dim idUser As String = String.Empty

            ' Retrieve data from Surat_Jalan
            Using command As New MySqlCommand(querySuratJalan, conn)
                command.Parameters.AddWithValue("@NOTA", NOTA)
                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        tglPengiriman = If(IsDBNull(reader("TGL_PENGIRIMAN")), String.Empty, reader("TGL_PENGIRIMAN").ToString())
                        Dim totalRupiahNumeric As Decimal = If(IsDBNull(reader("TOTAL_RUPIAH")), 0, Convert.ToDecimal(reader("TOTAL_RUPIAH")))
                        totalRupiah = Terbilang(totalRupiahNumeric)
                        armada = If(IsDBNull(reader("ARMADA")), String.Empty, reader("ARMADA").ToString()) & " " & If(IsDBNull(reader("JENIS_ARMADA")), String.Empty, reader("JENIS_ARMADA").ToString())
                        supir = If(IsDBNull(reader("SUPIR")), String.Empty, reader("SUPIR").ToString())
                        helper1 = If(IsDBNull(reader("HELPER1")), String.Empty, reader("HELPER1").ToString())
                        helper2 = If(IsDBNull(reader("HELPER2")), String.Empty, reader("HELPER2").ToString())
                        idUser = If(IsDBNull(reader("ID_USER")), String.Empty, reader("ID_USER").ToString())
                    End If
                End Using
            End Using

            ' Retrieve data from Surat_Jalan_Detail
            Using command As New MySqlCommand(querySuratJalanDetail, conn)
                command.Parameters.AddWithValue("@NOTA", NOTA)
                Using adapter As New MySqlDataAdapter(command)
                    adapter.Fill(ds, "Surat_Jalan_Detail")
                End Using
            End Using


            ' Set up the report parameters
            Dim parameters As New List(Of ReportParameter) From {
                New ReportParameter("TOKO", NAMA_PERUSAHAAN),
                New ReportParameter("NOTA", NOTA),
                New ReportParameter("TGL_PENGIRIMAN", tglPengiriman),
                New ReportParameter("TOTAL_RUPIAH", totalRupiah),
                New ReportParameter("ARMADA", armada),
                New ReportParameter("SUPIR", supir),
                New ReportParameter("HELPER1", helper1),
                New ReportParameter("HELPER2", helper2),
                New ReportParameter("ID_USER", idUser)
            }

            ' Assuming you have already set up your ReportViewer and its DataSources
            ' Clear any existing DataSources
            ReportViewer1.LocalReport.DataSources.Clear()

            ' Add new DataSources
            ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("Surat_Jalan_Detail")))

            ' Set the parameters for the report
            ReportViewer1.LocalReport.SetParameters(parameters)

            ' Refresh the ReportViewer
            ReportViewer1.RefreshReport()

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
End Class