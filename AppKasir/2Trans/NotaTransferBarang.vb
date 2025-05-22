Imports Microsoft.Reporting.WinForms

Public Class NotaTransferBarang

    Public Sub AmbilDataTransferBarang(ByVal ID_TRANSFER As String)
        ' Query untuk mendapatkan data dari Transfer_Barang dan Transfer_Barang_Detail
        Dim queryTransferBarang As String = "SELECT TGL_TRANSFER, LOKASI, TOTAL_RUPIAH, ID_USER FROM Transfer_Barang WHERE ID_TRANSFER = @ID_TRANSFER"
        Dim queryTransferBarangDetail As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA, QTY, SATUAN, TOTAL FROM Transfer_Barang_Detail WHERE ID_TRANSFER = @ID_TRANSFER"

        ' Buat DataSet baru
        Dim ds As New DataSet()

        ' Definisikan variabel untuk menyimpan data dari Transfer_Barang
        Dim tglTransfer As String = String.Empty
        Dim lokasi As String = String.Empty
        Dim totalRupiah As String = String.Empty
        Dim idUser As String = String.Empty
        Dim keteranganLokasi As String = String.Empty

        ' Ambil data dari Transfer_Barang
        Using command As New MySqlCommand(queryTransferBarang, conn)
            command.Parameters.AddWithValue("@ID_TRANSFER", ID_TRANSFER)
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.Read() Then
                    tglTransfer = If(IsDBNull(reader("TGL_TRANSFER")), String.Empty, reader("TGL_TRANSFER").ToString())
                    lokasi = If(IsDBNull(reader("LOKASI")), String.Empty, reader("LOKASI").ToString())

                    If lokasi = "TOKO" Then
                        keteranganLokasi = "TRANSFER BARANG TOKO KE GUDANG"
                    ElseIf lokasi = "GUDANG" Then
                        keteranganLokasi = "TRANSFER BARANG GUDANG KE TOKO"
                    End If

                    Dim totalRupiahNumeric As Decimal = If(IsDBNull(reader("TOTAL_RUPIAH")), 0, Convert.ToDecimal(reader("TOTAL_RUPIAH")))
                    totalRupiah = Terbilang(totalRupiahNumeric)
                    idUser = If(IsDBNull(reader("ID_USER")), String.Empty, reader("ID_USER").ToString())
                End If
            End Using
        End Using

        ' Ambil data dari Transfer_Barang_Detail
        Using command As New MySqlCommand(queryTransferBarangDetail, conn)
            command.Parameters.AddWithValue("@ID_TRANSFER", ID_TRANSFER)
            Using adapter As New MySqlDataAdapter(command)
                adapter.Fill(ds, "Transfer_Barang_Detail")
            End Using
        End Using

        ' Atur parameter laporan
        Dim parameters As New List(Of ReportParameter) From {
            New ReportParameter("TOKO", NAMA_PERUSAHAAN),
            New ReportParameter("NOTA", ID_TRANSFER),
            New ReportParameter("TGL_TRANSFER", tglTransfer),
            New ReportParameter("LOKASI", lokasi),
            New ReportParameter("KETERANGAN_LOKASI", keteranganLokasi),
            New ReportParameter("TOTAL_RUPIAH", totalRupiah),
            New ReportParameter("ID_USER", idUser)
        }

        ' Asumsikan ReportViewer dan DataSources sudah diatur
        ' Bersihkan DataSources yang ada
        ReportViewer1.LocalReport.DataSources.Clear()

        ' Tambahkan DataSource baru
        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("Transfer_Barang_Detail")))

        ' Setel parameter untuk laporan
        ReportViewer1.LocalReport.SetParameters(parameters)

        ' Refresh ReportViewer
        ReportViewer1.RefreshReport()
    End Sub



End Class