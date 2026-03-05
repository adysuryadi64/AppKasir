

Public Class FormGrafikLaba

    Public Sub DataCombox()
        Dim SQL As String = "SELECT DISTINCT DATE_FORMAT(TGL_TRANSAKSI, '%Y') AS TAHUN FROM penjualan ORDER BY TAHUN"

        Using cmd As New MySqlCommand(SQL, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                CmbTahun.Items.Clear() ' Membersihkan item sebelumnya

                While rd.Read()
                    ' Menambahkan item hanya tahun
                    CmbTahun.Items.Add(rd("TAHUN").ToString())
                End While
            End Using
        End Using
    End Sub




    Private Sub FormGrafikLaba_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        DataCombox()

    End Sub

    Private Sub CmbTahun_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbTahun.SelectedIndexChanged
        ChartLabaPenjualan()
        ChartPenjualanBarang()
    End Sub
    Public Sub ChartLabaPenjualan()
        ' Perbaikan query dengan sintaks MySQL yang benar
        Dim query As String = "SELECT DATE_FORMAT(TANGGAL_JUAL, '%m') AS BULAN, " &
                          "SUM(LABA) AS LABA " &
                          "FROM penjualan_detail " &
                          "WHERE DATE_FORMAT(TANGGAL_JUAL, '%Y') = @Tahun " &
                          "GROUP BY DATE_FORMAT(TANGGAL_JUAL, '%m') " &
                          "ORDER BY DATE_FORMAT(TANGGAL_JUAL, '%m') ASC"

        ' Dictionary untuk menyimpan data laba per bulan
        Dim bulanData As New Dictionary(Of String, Decimal)

        ' Inisialisasi semua bulan dengan nilai 0
        For i As Integer = 1 To 12
            bulanData(i.ToString("D2")) = 0D
        Next

        Try
            Using cmd As New MySqlCommand(query, conn)
                ' Parameter untuk tahun
                cmd.Parameters.AddWithValue("@Tahun", CmbTahun.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim bulan As String = rd.GetString("BULAN") ' Ambil nilai kolom BULAN
                        Dim total As Decimal = rd.GetDecimal("LABA") ' Ambil nilai kolom LABA
                        bulanData(bulan) = total
                    End While
                End Using
            End Using

            ' Bersihkan data lama pada chart
            Chart3.Series("Series1").Points.Clear()

            ' Tambahkan data ke chart
            For Each bulan As String In bulanData.Keys
                Chart3.Series("Series1").Points.AddXY(bulan, bulanData(bulan))
            Next

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub ChartPenjualanBarang()
        ' Query SQL untuk mendapatkan data penjualan per bulan
        Dim query As String = "SELECT DATE_FORMAT(TGL_TRANSAKSI, '%m') AS BULAN, " &
                              "SUM(GRAND_TOTAL_STL_PAJAK) AS TOTAL " &
                              "FROM penjualan " &
                              "WHERE DATE_FORMAT(TGL_TRANSAKSI, '%Y') = @Tahun " &
                              "GROUP BY DATE_FORMAT(TGL_TRANSAKSI, '%m') " &
                              "ORDER BY DATE_FORMAT(TGL_TRANSAKSI, '%m') ASC"

        ' Dictionary untuk menyimpan data penjualan per bulan
        Dim bulanData As New Dictionary(Of String, Decimal)

        ' Inisialisasi semua bulan dengan nilai 0
        For i As Integer = 1 To 12
            bulanData(i.ToString("D2")) = 0D
        Next

        Try
            Using cmd As New MySqlCommand(query, conn)
                ' Tambahkan parameter tahun
                cmd.Parameters.AddWithValue("@Tahun", CmbTahun.Text)

                ' Membaca data dari database
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim bulan As String = rd.GetString("BULAN") ' Ambil nilai kolom BULAN
                        Dim total As Decimal = rd.GetDecimal("TOTAL") ' Ambil nilai kolom TOTAL
                        bulanData(bulan) = total
                    End While
                End Using
            End Using

            ' Hapus data lama pada chart
            Chart2.Series("Series1").Points.Clear()

            ' Tambahkan data ke chart
            For Each bulan As String In bulanData.Keys
                Chart2.Series("Series1").Points.AddXY(bulan, bulanData(bulan))
            Next

        Catch ex As Exception
            ' Tangani kesalahan dan tampilkan pesan
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


End Class