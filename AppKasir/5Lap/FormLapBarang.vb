Imports Microsoft.Reporting.WinForms

Public Class FormLapBarang

    ' Variabel sumber kebenaran — Label hanya untuk display
    Private _totalStokToko As Decimal = 0D
    Private _totalRpToko As Decimal = 0D
    Private _totalStokGudang As Decimal = 0D
    Private _totalRpGudang As Decimal = 0D

    Private Sub FormLapBarang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' LblJudul header laporan otomatis via nama LblHeader*
        ' Rename LblJudul -> LblHeader untuk tema otomatis
        TxtCari.Text = ""
        Tampil()
        ReportViewer1.RefreshReport()
    End Sub


    ' Handler untuk event GotFocus pada TextBox
    Private Sub TxtCari_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCari.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCari.BackColor = ModuleTheme.C(Color.Yellow, Color.FromArgb(255, 204, 0)) ' Ganti warna fokus sesuai kebutuhan
    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TxtCari_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCari.LostFocus
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCari.BackColor = SystemColors.ActiveCaption
    End Sub

    Public Sub Tampil()
        Dim searchTerm As String = TxtCari.Text.Trim()
        Dim query As String = "SELECT NAMA_BARANG FROM tbl_barang WHERE NAMA_BARANG LIKE @searchTerm OR BARCODE_KECIL LIKE @searchTerm OR BARCODE_SEDANG LIKE @searchTerm OR BARCODE_BESAR LIKE @searchTerm"

        Dim dt As New DataTable

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

            Using da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Dim a As New AutoCompleteStringCollection
        For i As Integer = 0 To dt.Rows.Count - 1
            a.Add(dt.Rows(i)("NAMA_BARANG").ToString())
        Next

        TxtCari.AutoCompleteSource = AutoCompleteSource.CustomSource
        TxtCari.AutoCompleteCustomSource = a
        TxtCari.AutoCompleteMode = AutoCompleteMode.Suggest

        dt.Dispose() ' Pastikan Anda membebaskan objek DataTable setelah digunakan.
    End Sub


    Private Sub Tampilsemua()
        ' Query untuk menghitung jumlah barang di toko berdasarkan nama barang
        Dim queryJumlahToko As String = "SELECT COUNT(*) AS RECORD FROM tbl_barang WHERE NAMA_BARANG LIKE @NAMA_BARANG"

        ' Query untuk menghitung jumlah barang di gudang berdasarkan nama barang
        Dim queryJumlahGudang As String = "SELECT COUNT(*) AS RECORD FROM tbl_barang WHERE NAMA_BARANG LIKE @NAMA_BARANG"

        ' Hitung jumlah record di toko berdasarkan nama barang
        Using cmdHitungJumlahToko As New MySqlCommand(queryJumlahToko, conn)
            cmdHitungJumlahToko.Parameters.AddWithValue("@NAMA_BARANG", "%" & TxtCari.Text & "%")
            Using rdJumlahToko As MySqlDataReader = cmdHitungJumlahToko.ExecuteReader()
                If rdJumlahToko.Read() AndAlso Not rdJumlahToko.IsDBNull(rdJumlahToko.GetOrdinal("RECORD")) Then
                    Dim jumlahToko As Integer
                    If Integer.TryParse(rdJumlahToko("RECORD").ToString(), jumlahToko) Then
                        LblRecordToko.Text = jumlahToko.ToString("N0")
                    Else
                        LblRecordToko.Text = "0"
                    End If
                End If
            End Using
        End Using

        ' Hitung jumlah record di gudang berdasarkan nama barang
        Using cmdHitungJumlahGudang As New MySqlCommand(queryJumlahGudang, conn)
            cmdHitungJumlahGudang.Parameters.AddWithValue("@NAMA_BARANG", "%" & TxtCari.Text & "%")
            Using rdJumlahGudang As MySqlDataReader = cmdHitungJumlahGudang.ExecuteReader()
                If rdJumlahGudang.Read() AndAlso Not rdJumlahGudang.IsDBNull(rdJumlahGudang.GetOrdinal("RECORD")) Then
                    Dim jumlahGudang As Integer
                    If Integer.TryParse(rdJumlahGudang("RECORD").ToString(), jumlahGudang) Then
                        LblRecordGudang.Text = jumlahGudang.ToString("N0")
                    Else
                        LblRecordGudang.Text = "0"
                    End If
                End If
            End Using
        End Using

        ' Query untuk menghitung total stok di toko berdasarkan nama barang
        Dim queryToko As String = "SELECT HARGA_BELI, STOK_TOKO, SATUAN_ISI_STOK FROM tbl_barang WHERE NAMA_BARANG LIKE @NAMA_BARANG"

        ' Query untuk menghitung total stok di gudang berdasarkan nama barang
        Dim queryGudang As String = "SELECT HARGA_BELI, STOK_GUDANG, SATUAN_ISI_STOK FROM tbl_barang WHERE NAMA_BARANG LIKE @NAMA_BARANG"

        ' Menghitung total stok dan nilai di toko
        Using cmdToko As New MySqlCommand(queryToko, conn)
            cmdToko.Parameters.AddWithValue("@NAMA_BARANG", "%" & TxtCari.Text & "%")
            Using rdToko As MySqlDataReader = cmdToko.ExecuteReader()
                Dim totalRupiahToko As Decimal = 0D
                Dim totalToko As Decimal = 0D

                While rdToko.Read()
                    ' Pastikan nilai tidak DBNull, jika iya maka gunakan nilai default (0)
                    Dim hargaBeli As Decimal = If(IsDBNull(rdToko("HARGA_BELI")), 0D, Convert.ToDecimal(rdToko("HARGA_BELI")))
                    Dim stokToko As Decimal = If(IsDBNull(rdToko("STOK_TOKO")), 0D, Convert.ToDecimal(rdToko("STOK_TOKO")))
                    Dim isiStokToko As Decimal = If(IsDBNull(rdToko("SATUAN_ISI_STOK")), 0D, Convert.ToDecimal(rdToko("SATUAN_ISI_STOK")))

                    totalRupiahToko += (hargaBeli * (stokToko * isiStokToko))
                    totalToko += stokToko
                End While

                LblStokToko.Text = totalToko.ToString("N0")
                LblRpToko.Text = totalRupiahToko.ToString("N0")
            End Using
        End Using

        ' Menghitung total stok dan nilai di gudang
        Using cmdGudang As New MySqlCommand(queryGudang, conn)
            cmdGudang.Parameters.AddWithValue("@NAMA_BARANG", "%" & TxtCari.Text & "%")
            Using rdGudang As MySqlDataReader = cmdGudang.ExecuteReader()
                Dim totalRupiahGudang As Decimal = 0D
                Dim totalGudang As Decimal = 0D

                While rdGudang.Read()
                    ' Pastikan nilai tidak DBNull, jika iya gunakan nilai default (0)
                    Dim hargaBeli As Decimal = If(IsDBNull(rdGudang("HARGA_BELI")), 0D, Convert.ToDecimal(rdGudang("HARGA_BELI")))
                    Dim stokGudang As Decimal = If(IsDBNull(rdGudang("STOK_GUDANG")), 0D, Convert.ToDecimal(rdGudang("STOK_GUDANG")))
                    Dim isiStokGudang As Decimal = If(IsDBNull(rdGudang("SATUAN_ISI_STOK")), 0D, Convert.ToDecimal(rdGudang("SATUAN_ISI_STOK")))

                    totalRupiahGudang += (hargaBeli * (stokGudang * isiStokGudang))
                    totalGudang += stokGudang
                End While


                LblStokGudang.Text = totalGudang.ToString("N0")
                LblRpGudang.Text = totalRupiahGudang.ToString("N0")

            End Using
        End Using

        ' Menghitung total kuantitas dan nilai rupiah
        LblTotalQty.Text = ModuleAngka.FormatAngka(_totalStokToko + _totalStokGudang)
        LblTotalRp.Text = ModuleAngka.FormatAngka(_totalRpToko + _totalRpGudang)

        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, HARGA_BELI, STOK_TOKO, (HARGA_BELI * (STOK_TOKO * SATUAN_ISI_STOK)) AS RP_TOKO, STOK_GUDANG, (HARGA_BELI * (STOK_GUDANG * SATUAN_ISI_STOK)) AS RP_GUDANG, SATUAN_STOK FROM tbl_barang WHERE NAMA_BARANG like @NAMA_BARANG ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@NAMA_BARANG", "%" & TxtCari.Text & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using dataset As New DataSetKL()
                    dataset.Load(rd, LoadOption.OverwriteChanges, "StokBarang")

                    'Menetapkan dataset ke laporan RDLC
                    ReportViewer1.LocalReport.DataSources.Clear()
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("StokBarang")))

                    ' Setel parameter untuk laporan RDLC
                    Dim totalQtyParameter As New ReportParameter("totalqty", ModuleAngka.FormatAngka(_totalStokToko + _totalStokGudang))
                    Dim totalRupiahParameter As New ReportParameter("TotalRupiah", "Rp. " & ModuleAngka.FormatAngka(_totalRpToko + _totalRpGudang))
                    Dim perusahaanParameter As New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                    Dim jenisLaporanParameter As New ReportParameter("JenisLaporan", "Laporan Stok Barang " & TxtCari.Text)

                    ' Menambahkan parameter ke ReportViewer
                    ReportViewer1.LocalReport.SetParameters(New ReportParameter() {totalQtyParameter, totalRupiahParameter, perusahaanParameter, jenisLaporanParameter})

                    'Menampilkan laporan RDLC
                    ReportViewer1.RefreshReport()
                End Using
            End Using
        End Using

    End Sub

    Private Sub Tampilkosong()
        ' Query untuk menghitung jumlah barang yang stoknya habis di toko
        Dim queryJumlahToko As String = "SELECT COUNT(*) AS RECORD FROM tbl_barang WHERE STOK_TOKO = 0"

        ' Query untuk menghitung jumlah barang yang stoknya habis di gudang
        Dim queryJumlahGudang As String = "SELECT COUNT(*) AS RECORD FROM tbl_barang WHERE STOK_GUDANG = 0"

        ' Hitung jumlah record stok toko yang habis
        Using cmdHitungJumlahToko As New MySqlCommand(queryJumlahToko, conn)
            Using rdJumlahToko As MySqlDataReader = cmdHitungJumlahToko.ExecuteReader()
                If rdJumlahToko.Read() AndAlso Not rdJumlahToko.IsDBNull(rdJumlahToko.GetOrdinal("RECORD")) Then
                    Dim jumlahToko As Integer
                    If Integer.TryParse(rdJumlahToko("RECORD").ToString(), jumlahToko) Then
                        LblRecordToko.Text = jumlahToko.ToString("N0")
                    Else
                        LblRecordToko.Text = "0"
                    End If
                End If
            End Using
        End Using

        ' Hitung jumlah record stok gudang yang habis
        Using cmdHitungJumlahGudang As New MySqlCommand(queryJumlahGudang, conn)
            Using rdJumlahGudang As MySqlDataReader = cmdHitungJumlahGudang.ExecuteReader()
                If rdJumlahGudang.Read() AndAlso Not rdJumlahGudang.IsDBNull(rdJumlahGudang.GetOrdinal("RECORD")) Then
                    Dim jumlahGudang As Integer
                    If Integer.TryParse(rdJumlahGudang("RECORD").ToString(), jumlahGudang) Then
                        LblRecordGudang.Text = jumlahGudang.ToString("N0")
                    Else
                        LblRecordGudang.Text = "0"
                    End If
                End If
            End Using
        End Using

        Dim queryToko As String = "SELECT HARGA_BELI, STOK_TOKO, SATUAN_ISI_STOK FROM tbl_barang WHERE STOK_TOKO = 0"

        Using cmdToko As New MySqlCommand(queryToko, conn)
            Using rdToko As MySqlDataReader = cmdToko.ExecuteReader()
                Dim totalRupiahToko As Decimal = 0D
                Dim totalToko As Decimal = 0D

                While rdToko.Read()
                    ' Pastikan nilai tidak DBNull, jika iya maka gunakan nilai default (0)
                    Dim hargaBeli As Decimal = If(IsDBNull(rdToko("HARGA_BELI")), 0D, Convert.ToDecimal(rdToko("HARGA_BELI")))
                    Dim stokToko As Decimal = If(IsDBNull(rdToko("STOK_TOKO")), 0D, Convert.ToDecimal(rdToko("STOK_TOKO")))
                    Dim isiStokToko As Decimal = If(IsDBNull(rdToko("SATUAN_ISI_STOK")), 0D, Convert.ToDecimal(rdToko("SATUAN_ISI_STOK")))

                    totalRupiahToko += (hargaBeli * (stokToko * isiStokToko))
                    totalToko += stokToko
                End While

                LblStokToko.Text = totalToko.ToString("N0")
                LblRpToko.Text = totalRupiahToko.ToString("N0")
            End Using
        End Using

        Dim queryGudang As String = "SELECT HARGA_BELI, STOK_GUDANG, SATUAN_ISI_STOK FROM tbl_barang WHERE STOK_GUDANG = 0"

        Using cmdGudang As New MySqlCommand(queryGudang, conn)
            Using rdGudang As MySqlDataReader = cmdGudang.ExecuteReader()
                Dim totalRupiahGudang As Decimal = 0D
                Dim totalGudang As Decimal = 0D

                While rdGudang.Read()
                    ' Pastikan nilai tidak DBNull, jika iya gunakan nilai default (0)
                    Dim hargaBeli As Decimal = If(IsDBNull(rdGudang("HARGA_BELI")), 0D, Convert.ToDecimal(rdGudang("HARGA_BELI")))
                    Dim stokGudang As Decimal = If(IsDBNull(rdGudang("STOK_GUDANG")), 0D, Convert.ToDecimal(rdGudang("STOK_GUDANG")))
                    Dim isiStokGudang As Decimal = If(IsDBNull(rdGudang("SATUAN_ISI_STOK")), 0D, Convert.ToDecimal(rdGudang("SATUAN_ISI_STOK")))

                    totalRupiahGudang += (hargaBeli * (stokGudang * isiStokGudang))
                    totalGudang += stokGudang
                End While


                LblStokGudang.Text = totalGudang.ToString("N0")
                LblRpGudang.Text = totalRupiahGudang.ToString("N0")
            End Using
        End Using

        LblTotalQty.Text = ModuleAngka.FormatAngka(_totalStokToko + _totalStokGudang)
        LblTotalRp.Text = ModuleAngka.FormatAngka(_totalRpToko + _totalRpGudang)




        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, HARGA_BELI, STOK_TOKO, (HARGA_BELI * (STOK_TOKO * SATUAN_ISI_STOK)) AS RP_TOKO, STOK_GUDANG, (HARGA_BELI * (STOK_GUDANG * SATUAN_ISI_STOK)) AS RP_GUDANG, SATUAN_STOK FROM tbl_barang Where STOK_TOKO = 0 AND STOK_GUDANG = 0 ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using dataset As New DataSetKL()
                    dataset.Load(rd, LoadOption.OverwriteChanges, "StokBarang")

                    'Menetapkan dataset ke laporan RDLC
                    ReportViewer1.LocalReport.DataSources.Clear()
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("StokBarang")))

                    ' Setel parameter untuk laporan RDLC
                    Dim totalQtyParameter As New ReportParameter("totalqty", ModuleAngka.FormatAngka(_totalStokToko + _totalStokGudang))
                    Dim totalRupiahParameter As New ReportParameter("TotalRupiah", "Rp. " & ModuleAngka.FormatAngka(_totalRpToko + _totalRpGudang))
                    Dim perusahaanParameter As New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                    Dim jenisLaporanParameter As New ReportParameter("JenisLaporan", "Laporan Stok Barang Kosong")

                    ' Menambahkan parameter ke ReportViewer
                    ReportViewer1.LocalReport.SetParameters(New ReportParameter() {totalQtyParameter, totalRupiahParameter, perusahaanParameter, jenisLaporanParameter})

                    'Menampilkan laporan RDLC
                    ReportViewer1.RefreshReport()
                End Using
            End Using
        End Using

    End Sub

    Private Sub Tampilada()
        ' Query untuk menghitung jumlah stok toko yang lebih dari 0
        Dim queryJumlahToko As String = "SELECT COUNT(*) AS RECORD FROM tbl_barang WHERE STOK_TOKO > 0"

        ' Query untuk menghitung jumlah stok gudang yang lebih dari 0
        Dim queryJumlahGudang As String = "SELECT COUNT(*) AS RECORD FROM tbl_barang WHERE STOK_GUDANG > 0"

        ' Hitung jumlah record stok toko yang lebih dari 0
        Using cmdHitungJumlahToko As New MySqlCommand(queryJumlahToko, conn)
            Using rdJumlahToko As MySqlDataReader = cmdHitungJumlahToko.ExecuteReader()
                If rdJumlahToko.Read() AndAlso Not rdJumlahToko.IsDBNull(rdJumlahToko.GetOrdinal("RECORD")) Then
                    Dim jumlahToko As Integer
                    If Integer.TryParse(rdJumlahToko("RECORD").ToString(), jumlahToko) Then
                        LblRecordToko.Text = jumlahToko.ToString("N0")
                    Else
                        LblRecordToko.Text = "0"
                    End If

                End If
            End Using
        End Using

        ' Hitung jumlah record stok gudang yang lebih dari 0
        Using cmdHitungJumlahGudang As New MySqlCommand(queryJumlahGudang, conn)
            Using rdJumlahGudang As MySqlDataReader = cmdHitungJumlahGudang.ExecuteReader()
                If rdJumlahGudang.Read() AndAlso Not rdJumlahGudang.IsDBNull(rdJumlahGudang.GetOrdinal("RECORD")) Then
                    Dim jumlahGudang As Integer
                    If Integer.TryParse(rdJumlahGudang("RECORD").ToString(), jumlahGudang) Then
                        LblRecordGudang.Text = jumlahGudang.ToString("N0")
                    Else
                        LblRecordGudang.Text = "0"
                    End If

                End If
            End Using
        End Using

        Dim queryToko As String = "SELECT HARGA_BELI, STOK_TOKO, SATUAN_ISI_STOK FROM tbl_barang WHERE STOK_TOKO > 0"

        Using cmdToko As New MySqlCommand(queryToko, conn)
            Using rdToko As MySqlDataReader = cmdToko.ExecuteReader()
                Dim totalRupiahToko As Decimal = 0D
                Dim totalToko As Decimal = 0D

                While rdToko.Read()
                    ' Pastikan nilai tidak DBNull, jika iya maka gunakan nilai default (0)
                    Dim hargaBeli As Decimal = If(IsDBNull(rdToko("HARGA_BELI")), 0D, Convert.ToDecimal(rdToko("HARGA_BELI")))
                    Dim stokToko As Decimal = If(IsDBNull(rdToko("STOK_TOKO")), 0D, Convert.ToDecimal(rdToko("STOK_TOKO")))
                    Dim isiStokToko As Decimal = If(IsDBNull(rdToko("SATUAN_ISI_STOK")), 0D, Convert.ToDecimal(rdToko("SATUAN_ISI_STOK")))

                    totalRupiahToko += (hargaBeli * (stokToko * isiStokToko))
                    totalToko += stokToko
                End While

                LblStokToko.Text = totalToko.ToString("N0")
                LblRpToko.Text = totalRupiahToko.ToString("N0")
            End Using
        End Using

        Dim queryGudang As String = "SELECT HARGA_BELI, STOK_GUDANG, SATUAN_ISI_STOK FROM tbl_barang WHERE STOK_GUDANG > 0"

        Using cmdGudang As New MySqlCommand(queryGudang, conn)
            Using rdGudang As MySqlDataReader = cmdGudang.ExecuteReader()
                Dim totalRupiahGudang As Decimal = 0D
                Dim totalGudang As Decimal = 0D

                While rdGudang.Read()
                    ' Pastikan nilai tidak DBNull, jika iya gunakan nilai default (0)
                    Dim hargaBeli As Decimal = If(IsDBNull(rdGudang("HARGA_BELI")), 0D, Convert.ToDecimal(rdGudang("HARGA_BELI")))
                    Dim stokGudang As Decimal = If(IsDBNull(rdGudang("STOK_GUDANG")), 0D, Convert.ToDecimal(rdGudang("STOK_GUDANG")))
                    Dim isiStokGudang As Decimal = If(IsDBNull(rdGudang("SATUAN_ISI_STOK")), 0D, Convert.ToDecimal(rdGudang("SATUAN_ISI_STOK")))

                    totalRupiahGudang += (hargaBeli * (stokGudang * isiStokGudang))
                    totalGudang += stokGudang
                End While


                LblStokGudang.Text = totalGudang.ToString("N0")
                LblRpGudang.Text = totalRupiahGudang.ToString("N0")
            End Using
        End Using

        LblTotalQty.Text = ModuleAngka.FormatAngka(_totalStokToko + _totalStokGudang)
        LblTotalRp.Text = ModuleAngka.FormatAngka(_totalRpToko + _totalRpGudang)


        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, HARGA_BELI, STOK_TOKO, (HARGA_BELI * (STOK_TOKO * SATUAN_ISI_STOK)) AS RP_TOKO, STOK_GUDANG, (HARGA_BELI * (STOK_GUDANG * SATUAN_ISI_STOK)) AS RP_GUDANG, SATUAN_STOK FROM tbl_barang WHERE STOK_TOKO > 0 OR STOK_GUDANG > 0 ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using dataset As New DataSetKL()
                    dataset.Load(rd, LoadOption.OverwriteChanges, "StokBarang")

                    'Menetapkan dataset ke laporan RDLC
                    ReportViewer1.LocalReport.DataSources.Clear()
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("StokBarang")))

                    ' Setel parameter untuk laporan RDLC
                    Dim totalQtyParameter As New ReportParameter("totalqty", ModuleAngka.FormatAngka(_totalStokToko + _totalStokGudang))
                    Dim totalRupiahParameter As New ReportParameter("TotalRupiah", "Rp. " & ModuleAngka.FormatAngka(_totalRpToko + _totalRpGudang))
                    Dim perusahaanParameter As New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                    Dim jenisLaporanParameter As New ReportParameter("JenisLaporan", "Laporan Stok Barang Tidak Kosong")

                    ' Menambahkan parameter ke ReportViewer
                    ReportViewer1.LocalReport.SetParameters(New ReportParameter() {totalQtyParameter, totalRupiahParameter, perusahaanParameter, jenisLaporanParameter})

                    'Menampilkan laporan RDLC
                    ReportViewer1.RefreshReport()
                End Using
            End Using
        End Using


    End Sub

    Private Sub TampilMinus()
        ' Query untuk menghitung jumlah stok toko yang negatif
        Dim queryJumlahToko As String = "SELECT COUNT(*) AS RECORD FROM tbl_barang WHERE STOK_TOKO < 0"

        ' Query untuk menghitung jumlah stok gudang yang negatif
        Dim queryJumlahGudang As String = "SELECT COUNT(*) AS RECORD FROM tbl_barang WHERE STOK_GUDANG < 0"

        ' Hitung jumlah record stok toko yang negatif
        Using cmdHitungJumlahToko As New MySqlCommand(queryJumlahToko, conn)
            Using rdJumlahToko As MySqlDataReader = cmdHitungJumlahToko.ExecuteReader()
                If rdJumlahToko.Read() AndAlso Not rdJumlahToko.IsDBNull(rdJumlahToko.GetOrdinal("RECORD")) Then
                    Dim jumlahToko As Integer
                    If Integer.TryParse(rdJumlahToko("RECORD").ToString(), jumlahToko) Then
                        LblRecordToko.Text = jumlahToko.ToString("N0")
                    Else
                        LblRecordToko.Text = "0"
                    End If

                End If
            End Using
        End Using

        ' Hitung jumlah record stok gudang yang negatif
        Using cmdHitungJumlahGudang As New MySqlCommand(queryJumlahGudang, conn)
            Using rdJumlahGudang As MySqlDataReader = cmdHitungJumlahGudang.ExecuteReader()
                If rdJumlahGudang.Read() AndAlso Not rdJumlahGudang.IsDBNull(rdJumlahGudang.GetOrdinal("RECORD")) Then
                    Dim jumlahGudang As Integer
                    If Integer.TryParse(rdJumlahGudang("RECORD").ToString(), jumlahGudang) Then
                        LblRecordGudang.Text = jumlahGudang.ToString("N0")
                    Else
                        LblRecordGudang.Text = "0"
                    End If
                End If
            End Using
        End Using

        Dim queryToko As String = "SELECT HARGA_BELI, STOK_TOKO, SATUAN_ISI_STOK FROM tbl_barang WHERE STOK_TOKO < 0"

        Using cmdToko As New MySqlCommand(queryToko, conn)
            Using rdToko As MySqlDataReader = cmdToko.ExecuteReader()
                Dim totalRupiahToko As Decimal = 0D
                Dim totalToko As Decimal = 0D

                While rdToko.Read()
                    ' Pastikan nilai tidak DBNull, jika iya maka gunakan nilai default (0)
                    Dim hargaBeli As Decimal = If(IsDBNull(rdToko("HARGA_BELI")), 0D, Convert.ToDecimal(rdToko("HARGA_BELI")))
                    Dim stokToko As Decimal = If(IsDBNull(rdToko("STOK_TOKO")), 0D, Convert.ToDecimal(rdToko("STOK_TOKO")))
                    Dim isiStokToko As Decimal = If(IsDBNull(rdToko("SATUAN_ISI_STOK")), 0D, Convert.ToDecimal(rdToko("SATUAN_ISI_STOK")))

                    totalRupiahToko += (hargaBeli * (stokToko * isiStokToko))
                    totalToko += stokToko
                End While

                LblStokToko.Text = totalToko.ToString("N0")
                LblRpToko.Text = totalRupiahToko.ToString("N0")
            End Using
        End Using

        Dim queryGudang As String = "SELECT HARGA_BELI, STOK_GUDANG, SATUAN_ISI_STOK FROM tbl_barang WHERE STOK_GUDANG < 0"

        Using cmdGudang As New MySqlCommand(queryGudang, conn)
            Using rdGudang As MySqlDataReader = cmdGudang.ExecuteReader()
                Dim totalRupiahGudang As Decimal = 0D
                Dim totalGudang As Decimal = 0D

                While rdGudang.Read()
                    ' Pastikan nilai tidak DBNull, jika iya gunakan nilai default (0)
                    Dim hargaBeli As Decimal = If(IsDBNull(rdGudang("HARGA_BELI")), 0D, Convert.ToDecimal(rdGudang("HARGA_BELI")))
                    Dim stokGudang As Decimal = If(IsDBNull(rdGudang("STOK_GUDANG")), 0D, Convert.ToDecimal(rdGudang("STOK_GUDANG")))
                    Dim isiStokGudang As Decimal = If(IsDBNull(rdGudang("SATUAN_ISI_STOK")), 0D, Convert.ToDecimal(rdGudang("SATUAN_ISI_STOK")))

                    totalRupiahGudang += (hargaBeli * (stokGudang * isiStokGudang))
                    totalGudang += stokGudang
                End While


                LblStokGudang.Text = totalGudang.ToString("N0")
                LblRpGudang.Text = totalRupiahGudang.ToString("N0")
            End Using
        End Using

        LblTotalQty.Text = ModuleAngka.FormatAngka(_totalStokToko + _totalStokGudang)
        LblTotalRp.Text = ModuleAngka.FormatAngka(_totalRpToko + _totalRpGudang)


        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, HARGA_BELI, STOK_TOKO, (HARGA_BELI * (STOK_TOKO * SATUAN_ISI_STOK)) AS RP_TOKO, STOK_GUDANG, (HARGA_BELI * (STOK_GUDANG * SATUAN_ISI_STOK)) AS RP_GUDANG, SATUAN_STOK FROM tbl_barang WHERE STOK_TOKO < 0 OR STOK_GUDANG < 0 ORDER BY NAMA_BARANG"

        Using cmd As New MySqlCommand(query, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Using dataset As New DataSetKL()
                    dataset.Load(rd, LoadOption.OverwriteChanges, "StokBarang")

                    'Menetapkan dataset ke laporan RDLC
                    ReportViewer1.LocalReport.DataSources.Clear()
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dataset.Tables("StokBarang")))

                    ' Setel parameter untuk laporan RDLC
                    Dim totalQtyParameter As New ReportParameter("totalqty", ModuleAngka.FormatAngka(_totalStokToko + _totalStokGudang))
                    Dim totalRupiahParameter As New ReportParameter("TotalRupiah", "Rp. " & ModuleAngka.FormatAngka(_totalRpToko + _totalRpGudang))
                    Dim perusahaanParameter As New ReportParameter("Perusahaan", NAMA_PERUSAHAAN)
                    Dim jenisLaporanParameter As New ReportParameter("JenisLaporan", "Laporan Stok Barang Minus")

                    ' Menambahkan parameter ke ReportViewer
                    ReportViewer1.LocalReport.SetParameters(New ReportParameter() {totalQtyParameter, totalRupiahParameter, perusahaanParameter, jenisLaporanParameter})


                    'Menampilkan laporan RDLC
                    ReportViewer1.RefreshReport()
                End Using
            End Using
        End Using


    End Sub


    Private Sub BtnStokKosong_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStokKosong.Click
        Tampilkosong()
    End Sub

    Private Sub BtnStok_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStok.Click
        Tampilada()
    End Sub

    Private Sub BtnStokMinus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStokMinus.Click
        TampilMinus()
    End Sub

    Private Sub BtnCari_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCari.Click
        Tampilsemua()
    End Sub

    Private Sub TxtCari_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtCari.KeyDown
        If e.KeyCode = Keys.Enter Then
            Tampilsemua()
        End If
    End Sub

    Private Sub BtnSemua_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSemua.Click
        TxtCari.Clear()
        Tampilsemua()
    End Sub
    Private Sub FormLapBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnSemua.PerformClick()
        End Select
    End Sub

End Class
