Imports System.IO
Imports Excel = Microsoft.Office.Interop.Excel

Public Class FormPenjualanPPn


    Private BulanTerpilih As Integer

    Private Sub KonversiBulanKeAngka()
        Dim bulanDict As New Dictionary(Of String, Integer) From {
        {"Januari", 1}, {"Februari", 2}, {"Maret", 3}, {"April", 4},
        {"Mei", 5}, {"Juni", 6}, {"Juli", 7}, {"Agustus", 8},
        {"September", 9}, {"Oktober", 10}, {"November", 11}, {"Desember", 12}
    }
        BulanTerpilih = If(bulanDict.ContainsKey(CmbBln.Text), bulanDict(CmbBln.Text), 0)
    End Sub

    Private Sub MuatComboBoxBulanTahun()
        ' Isi ComboBox Bulan
        CmbBln.Items.Clear()
        CmbBln.Items.AddRange({"Januari", "Februari", "Maret", "April", "Mei", "Juni",
                           "Juli", "Agustus", "September", "Oktober", "November", "Desember"})

        ' Isi ComboBox Tahun
        CmbThn.Items.Clear()
        For i As Integer = 2022 To Year(Now)
            CmbThn.Items.Add(i)
        Next

        ' Set tahun sekarang sebagai default
        CmbThn.SelectedItem = Year(Now)
    End Sub

    Private Sub FormPenjualanPPn_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MuatComboBoxBulanTahun()
    End Sub

    Private Sub CbBulan_CheckedChanged(sender As Object, e As EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked Then
            CbTanggal.Checked = False
            CmbBln.SelectedIndex = DateTime.Now.Month - 1 ' Menyesuaikan indeks (0-based)
        End If
    End Sub


    Private Sub CbTanggal_CheckedChanged(sender As Object, e As EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked Then
            CbBulan.Checked = False
        End If
    End Sub

    Private Sub BtnTampilkan_Click(sender As Object, e As EventArgs) Handles BtnTampilkan.Click
        CariData()
    End Sub
    Private Sub TxtKunci_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtKunci.KeyDown
        If e.KeyCode = Keys.Enter Then
            CariData()
        End If
    End Sub

    Private Sub CariData()
        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI FROM tbl_barang WHERE "
        Dim kondisi As New List(Of String)
        Dim keywords As String() = TxtKunci.Text.Split(","c) ' Pisahkan berdasarkan koma
        Dim parameters As New List(Of MySqlParameter)

        ' Buat kondisi LIKE untuk setiap kata kunci
        For i As Integer = 0 To keywords.Length - 1
            Dim paramName As String = "@keyword" & i
            Dim keyword As String = keywords(i).Trim() ' Hapus spasi di awal/akhir
            If RbtNama.Checked Then
                kondisi.Add("NAMA_BARANG LIKE " & paramName)
            ElseIf RbtKategori.Checked Then
                kondisi.Add("NAMA_KATEGORI LIKE " & paramName)
            End If
            parameters.Add(New MySqlParameter(paramName, "%" & keyword & "%"))
        Next

        ' Gabungkan kondisi dengan OR
        query &= String.Join(" OR ", kondisi)

        Using cmd As New MySqlCommand(query, conn)

            ' Tambahkan parameter ke perintah SQL
            cmd.Parameters.AddRange(parameters.ToArray())

            Dim adapter As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            ' Tampilkan hasil pencarian ke DGVFilter
            DGVFilter.DataSource = dt

            ' Pastikan kolom "Hapus" selalu ada di akhir
            If Not DGVFilter.Columns.Contains("Hapus") Then
                Dim btnHapus As New DataGridViewButtonColumn()
                btnHapus.Name = "Hapus"
                btnHapus.HeaderText = "Hapus"
                btnHapus.Text = "X"
                btnHapus.UseColumnTextForButtonValue = True
                DGVFilter.Columns.Add(btnHapus)
            End If

            ' Pindahkan kolom "Hapus" ke posisi terakhir
            DGVFilter.Columns("Hapus").DisplayIndex = DGVFilter.Columns.Count - 1
        End Using
    End Sub

    ' Event handler untuk tombol hapus di DataGridView
    Private Sub DGVFilter_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGVFilter.CellContentClick
        If e.ColumnIndex = DGVFilter.Columns("Hapus").Index AndAlso e.RowIndex >= 0 Then
            DGVFilter.Rows.RemoveAt(e.RowIndex)
        End If
    End Sub

    Private Sub DGVFilter_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DGVFilter.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DGVFilter.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    Private Sub BtnFilter_Click(sender As Object, e As EventArgs) Handles BtnFilter.Click
        ExportToExcel("NonPPn")
    End Sub

    Private Sub BtnNonFilter_Click(sender As Object, e As EventArgs) Handles BtnNonFilter.Click
        ExportToExcel("PPn")
    End Sub

    'Private Sub ExportToExcel(ByVal JENIS As String)
    '    Dim TanggalAwal As Date
    '    Dim TanggalAkhir As Date

    '    ' Tentukan rentang tanggal berdasarkan filter yang dipilih
    '    If CbTanggal.Checked Then
    '        TanggalAwal = DTPAwal.Value.Date
    '        TanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
    '    ElseIf CbBulan.Checked Then
    '        KonversiBulanKeAngka()
    '        Dim Bulan As Integer = BulanTerpilih
    '        Dim Tahun As Integer = CmbThn.Text
    '        TanggalAwal = New DateTime(Tahun, Bulan, 1)
    '        TanggalAkhir = TanggalAwal.AddMonths(1).AddSeconds(-1)
    '    Else
    '        MsgBox("Pilih filter tanggal atau bulan!", MsgBoxStyle.Exclamation)
    '        Exit Sub
    '    End If

    '    ' Pastikan DGVFilter memiliki data
    '    If DGVFilter.Rows.Count = 0 Then
    '        MsgBox("Data belum tersedia di DGVFilter!", MsgBoxStyle.Exclamation)
    '        Exit Sub
    '    End If

    '    ' Ambil semua ID_BARANG dari kolom pertama DGVFilter
    '    Dim idBarangList As New List(Of String)
    '    For Each row As DataGridViewRow In DGVFilter.Rows
    '        If Not row.IsNewRow Then
    '            idBarangList.Add(row.Cells(0).Value.ToString())
    '        End If
    '    Next

    '    ' Buat parameter untuk setiap ID_BARANG
    '    Dim idParams As New List(Of String)
    '    Dim parameters As New List(Of MySqlParameter)
    '    For i As Integer = 0 To idBarangList.Count - 1
    '        Dim paramName As String = "@id" & i
    '        idParams.Add(paramName)
    '        parameters.Add(New MySqlParameter(paramName, idBarangList(i)))
    '    Next

    '    Dim query As String

    '    If JENIS = "NonPPn" Then
    '        ' Bangun query dengan multiple parameter
    '        query = "SELECT TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN, SUM(TOTAL_HARGA) AS TOTAL_HARGA " &
    '                      "FROM penjualan_detail " &
    '                      "WHERE ID_BARANG IN (" & String.Join(",", idParams) & ") " &
    '                      "AND TANGGAL_JUAL BETWEEN @AwalBulan AND @AkhirBulan " &
    '                      "GROUP BY TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN"
    '    Else
    '        query = "SELECT TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN, SUM(TOTAL_HARGA) AS TOTAL_HARGA " &
    '                 "FROM penjualan_detail " &
    '                 "WHERE ID_BARANG NOT IN (" & String.Join(",", idParams) & ") " &
    '                 "AND TANGGAL_JUAL BETWEEN @AwalBulan AND @AkhirBulan " &
    '                 "GROUP BY TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN"
    '    End If

    '    ' Koneksi ke MySQL
    '    Using cmd As New MySqlCommand(query, conn)

    '        ' Tambahkan parameter ID_BARANG
    '        cmd.Parameters.AddRange(parameters.ToArray())

    '        ' Tambahkan parameter tanggal
    '        cmd.Parameters.AddWithValue("@AwalBulan", TanggalAwal)
    '        cmd.Parameters.AddWithValue("@AkhirBulan", TanggalAkhir)

    '        ' Eksekusi query dan simpan dalam DataTable
    '        Dim adapter As New MySqlDataAdapter(cmd)
    '        Dim dt As New DataTable()
    '        adapter.Fill(dt)

    '        ' Cek apakah ada data
    '        If dt.Rows.Count = 0 Then
    '            MsgBox("Tidak ada data untuk diekspor!", MsgBoxStyle.Information)
    '            Exit Sub
    '        End If

    '        Dim Namalaporan As String
    '        If JENIS = "NonPPn" Then
    '            Namalaporan = "Laporan_Barang_Non_PPN.xlsx"
    '        Else
    '            Namalaporan = "Laporan_Barang_PPN.xlsx"
    '        End If

    '        ' Ekspor ke Excel menggunakan Interop
    '        Dim saveDialog As New SaveFileDialog()
    '        saveDialog.Filter = "Excel Files|*.xlsx"
    '        saveDialog.Title = "Simpan Laporan Excel"
    '        saveDialog.FileName = Namalaporan

    '        If saveDialog.ShowDialog() = DialogResult.OK Then
    '            ' Membuka aplikasi Excel
    '            Dim excelApp As New Excel.Application()
    '            Dim workBook As Excel.Workbook = excelApp.Workbooks.Add()
    '            Dim worksheet As Excel.Worksheet = workBook.Sheets(1)
    '            excelApp.Visible = False ' Bisa diset True jika ingin Excel tampil

    '            ' Menambahkan 4 Baris Judul
    '            worksheet.Cells(1, 1).Value = "LAPORAN PENJUALAN " & NAMA_PERUSAHAAN
    '            worksheet.Cells(2, 1).Value = "JENIS BARANG : " & TxtKunci.Text.ToUpper()
    '            worksheet.Cells(3, 1).Value = "Periode: " & TanggalAwal.ToString("dd/MM/yy HH:mm:ss") & " - " & TanggalAkhir.ToString("dd/MM/yy HH:mm:ss")
    '            worksheet.Cells(4, 1).Value = "Dicetak pada: " & DateTime.Now.ToString("dd/MM/yy HH:mm:ss")

    '            ' Merge kolom untuk judul
    '            worksheet.Range("A1", "E1").Merge()
    '            worksheet.Range("A2", "E2").Merge()
    '            worksheet.Range("A3", "E3").Merge()
    '            worksheet.Range("A4", "E4").Merge()

    '            worksheet.Cells(1, 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
    '            worksheet.Cells(2, 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
    '            worksheet.Cells(3, 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
    '            worksheet.Cells(4, 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter

    '            ' Menambahkan Header dengan Nama Kolom yang Diperbaiki
    '            Dim headers As String() = {"NO", "TANGGAL", "FAKTUR", "PELANGGAN", "TOTAL"}
    '            For i As Integer = 0 To headers.Length - 1
    '                worksheet.Cells(5, i + 1).Value = headers(i)
    '                worksheet.Cells(5, i + 1).Font.Bold = True
    '                worksheet.Cells(5, i + 1).Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray)
    '                worksheet.Cells(5, i + 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
    '            Next

    '            ' Menambahkan Data
    '            Dim rowIndex As Integer = 6
    '            Dim nomor As Integer = 1
    '            Dim subTotal As Double = 0
    '            For Each row As DataRow In dt.Rows
    '                worksheet.Cells(rowIndex, 1).Value = nomor ' NOMOR
    '                worksheet.Cells(rowIndex, 2).Value = Convert.ToDateTime(row("TANGGAL_JUAL")).ToString("dd/MM/yy HH:mm:ss") ' TANGGAL
    '                worksheet.Cells(rowIndex, 3).Value = row("FAKTUR_JUAL").ToString() ' FAKTUR
    '                worksheet.Cells(rowIndex, 4).Value = row("NAMA_PELANGGAN").ToString() ' PELANGGAN
    '                worksheet.Cells(rowIndex, 5).Value = Convert.ToDouble(row("TOTAL_HARGA")) ' TOTAL HARGA

    '                ' Set format sel untuk kolom tanggal (Kolom B)
    '                With worksheet
    '                    .Cells(rowIndex, 2).Value = Convert.ToDateTime(row("TANGGAL_JUAL")).ToString("dd/MM/yy HH:mm:ss")
    '                    .Cells(rowIndex, 2).NumberFormat = "dd/MM/yy HH:mm:ss"
    '                End With


    '                subTotal += Convert.ToDouble(row("TOTAL_HARGA"))
    '                nomor += 1
    '                rowIndex += 1
    '            Next

    '            ' Menambahkan baris Subtotal
    '            worksheet.Cells(rowIndex, 4).Value = "SUBTOTAL"
    '            worksheet.Cells(rowIndex, 5).Value = subTotal
    '            worksheet.Cells(rowIndex, 4).Font.Bold = True
    '            worksheet.Cells(rowIndex, 5).Font.Bold = True
    '            worksheet.Cells(rowIndex, 4).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
    '            worksheet.Cells(rowIndex, 5).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight

    '            ' Mengatur AutoFit kolom
    '            worksheet.Columns.AutoFit()

    '            ' Simpan File
    '            workBook.SaveAs(saveDialog.FileName)
    '            workBook.Close()
    '            excelApp.Quit()

    '            MsgBox("Laporan berhasil diekspor ke Excel!", MsgBoxStyle.Information)
    '        End If

    '    End Using
    'End Sub




    ' Fungsi untuk mengecek apakah file sedang terbuka
    Private Function IsFileOpen(ByVal filePath As String) As Boolean
        If Not File.Exists(filePath) Then Return False ' Jika file tidak ada, langsung return False

        Try
            Using fs As FileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None)
                fs.Close()
            End Using
            Return False ' File tidak terbuka
        Catch ex As IOException
            Return True ' File sedang terbuka
        End Try
    End Function

    ' Fungsi utama untuk ekspor data ke Excel
    Private Sub ExportToExcel(ByVal JENIS As String)
        Try

            Me.Cursor = Cursors.WaitCursor ' Set cursor ke wait

            Dim TanggalAwal As Date
            Dim TanggalAkhir As Date

            ' Tentukan rentang tanggal berdasarkan filter yang dipilih
            If CbTanggal.Checked Then
                TanggalAwal = DTPAwal.Value.Date
                TanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
            ElseIf CbBulan.Checked Then
                KonversiBulanKeAngka()
                Dim Bulan As Integer = BulanTerpilih
                Dim Tahun As Integer = CmbThn.Text
                TanggalAwal = New DateTime(Tahun, Bulan, 1)
                TanggalAkhir = TanggalAwal.AddMonths(1).AddSeconds(-1)
            Else
                MsgBox("Pilih filter tanggal atau bulan!", MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            ' Pastikan DGVFilter memiliki data
            If DGVFilter.Rows.Count = 0 Then
                MsgBox("Data belum tersedia di DGVFilter!", MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            ' Ambil semua ID_BARANG dari DataGridView
            Dim idBarangList As New List(Of String)
            For Each row As DataGridViewRow In DGVFilter.Rows
                If Not row.IsNewRow Then
                    idBarangList.Add(row.Cells(0).Value.ToString())
                End If
            Next

            ' Buat parameter untuk setiap ID_BARANG
            Dim idParams As New List(Of String)
            Dim parameters As New List(Of MySqlParameter)
            For i As Integer = 0 To idBarangList.Count - 1
                Dim paramName As String = "@id" & i
                idParams.Add(paramName)
                parameters.Add(New MySqlParameter(paramName, idBarangList(i)))
            Next

            ' Tentukan query SQL berdasarkan jenis barang
            Dim query As String
            If JENIS = "NonPPn" Then
                query = "SELECT TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN, SUM(TOTAL_HARGA) AS TOTAL_HARGA " &
                    "FROM penjualan_detail " &
                    "WHERE ID_BARANG IN (" & String.Join(",", idParams) & ") " &
                    "AND TANGGAL_JUAL BETWEEN @AwalBulan AND @AkhirBulan " &
                    "GROUP BY TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN"
            Else
                query = "SELECT TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN, SUM(TOTAL_HARGA) AS TOTAL_HARGA " &
                    "FROM penjualan_detail " &
                    "WHERE ID_BARANG NOT IN (" & String.Join(",", idParams) & ") " &
                    "AND TANGGAL_JUAL BETWEEN @AwalBulan AND @AkhirBulan " &
                    "GROUP BY TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN"
            End If

            ' Koneksi ke MySQL
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddRange(parameters.ToArray())
                cmd.Parameters.AddWithValue("@AwalBulan", TanggalAwal)
                cmd.Parameters.AddWithValue("@AkhirBulan", TanggalAkhir)

                Dim adapter As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)

                ' Jika tidak ada data, beri peringatan
                If dt.Rows.Count = 0 Then
                    MsgBox("Tidak ada data untuk diekspor!", MsgBoxStyle.Information)
                    Exit Sub
                End If

                ' Nama file berdasarkan jenis barang
                Dim Namalaporan As String = If(JENIS = "NonPPn", "Laporan_Barang_Non_PPN.xlsx", "Laporan_Barang_PPN.xlsx")

                ' Dialog penyimpanan file
                Dim saveDialog As New SaveFileDialog()
                saveDialog.Filter = "Excel Files|*.xlsx"
                saveDialog.Title = "Simpan Laporan Excel"
                saveDialog.FileName = Namalaporan

                If saveDialog.ShowDialog() = DialogResult.OK Then
                    Dim filePath As String = saveDialog.FileName

                    ' Cek apakah file Excel sudah terbuka
                    If IsFileOpen(filePath) Then
                        MsgBox("File excell sudah terbuka, silakan tutup terlebih dahulu sebelum menyimpan!", MsgBoxStyle.Exclamation)
                        Exit Sub
                    End If

                    ' Membuka aplikasi Excel
                    Dim excelApp As New Excel.Application()
                    Dim workBook As Excel.Workbook = excelApp.Workbooks.Add()
                    Dim worksheet As Excel.Worksheet = workBook.Sheets(1)
                    excelApp.Visible = False ' Bisa diset True jika ingin Excel tampil

                    ' Menambahkan 4 Baris Judul
                    Dim Jenisbarang As String = If(JENIS = "NonPPn", "JENIS BARANG : " & TxtKunci.Text.ToUpper(), "JENIS BARANG SELAIN : " & TxtKunci.Text.ToUpper())

                    worksheet.Cells(1, 1).Value = "LAPORAN PENJUALAN " & NAMA_PERUSAHAAN
                    worksheet.Cells(2, 1).Value = Jenisbarang
                    worksheet.Cells(3, 1).Value = "Periode: " & TanggalAwal.ToString("dd/MM/yy HH:mm:ss") & " - " & TanggalAkhir.ToString("dd/MM/yy HH:mm:ss")
                    worksheet.Cells(4, 1).Value = "Dicetak pada: " & DateTime.Now.ToString("dd/MM/yy HH:mm:ss")

                    ' Merge kolom untuk judul
                    worksheet.Range("A1", "E1").Merge()
                    worksheet.Range("A2", "E2").Merge()
                    worksheet.Range("A3", "E3").Merge()
                    worksheet.Range("A4", "E4").Merge()

                    worksheet.Cells(1, 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
                    worksheet.Cells(2, 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
                    worksheet.Cells(3, 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
                    worksheet.Cells(4, 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter


                    ' Menambahkan Header dengan Nama Kolom yang Diperbaiki
                    Dim headers As String() = {"NO", "TANGGAL", "FAKTUR", "PELANGGAN", "TOTAL"}
                    For i As Integer = 0 To headers.Length - 1
                        worksheet.Cells(5, i + 1).Value = headers(i)
                        worksheet.Cells(5, i + 1).Font.Bold = True
                        worksheet.Cells(5, i + 1).Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray)
                        worksheet.Cells(5, i + 1).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
                    Next

                    ' Menambahkan Data
                    Dim rowIndex As Integer = 6
                    Dim nomor As Integer = 1
                    Dim subTotal As Double = 0
                    For Each row As DataRow In dt.Rows
                        worksheet.Cells(rowIndex, 1).Value = nomor ' NOMOR
                        worksheet.Cells(rowIndex, 2).Value = Convert.ToDateTime(row("TANGGAL_JUAL")).ToString("dd/MM/yy HH:mm:ss") ' TANGGAL
                        worksheet.Cells(rowIndex, 3).Value = row("FAKTUR_JUAL").ToString() ' FAKTUR
                        worksheet.Cells(rowIndex, 4).Value = row("NAMA_PELANGGAN").ToString() ' PELANGGAN
                        worksheet.Cells(rowIndex, 5).Value = Convert.ToDouble(row("TOTAL_HARGA")) ' TOTAL HARGA

                        ' Set format sel untuk kolom tanggal (Kolom B)
                        With worksheet
                            .Cells(rowIndex, 2).Value = Convert.ToDateTime(row("TANGGAL_JUAL")).ToString("dd/MM/yy HH:mm:ss")
                            .Cells(rowIndex, 2).NumberFormat = "dd/MM/yy HH:mm:ss"
                        End With


                        subTotal += Convert.ToDouble(row("TOTAL_HARGA"))
                        nomor += 1
                        rowIndex += 1
                    Next

                    ' Menambahkan baris Subtotal
                    worksheet.Cells(rowIndex, 4).Value = "SUBTOTAL"
                    worksheet.Cells(rowIndex, 5).Value = subTotal
                    worksheet.Cells(rowIndex, 4).Font.Bold = True
                    worksheet.Cells(rowIndex, 5).Font.Bold = True
                    worksheet.Cells(rowIndex, 4).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
                    worksheet.Cells(rowIndex, 5).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight

                    ' Mengatur AutoFit kolom
                    worksheet.Columns.AutoFit()

                    ' Simpan file
                    workBook.SaveAs(filePath)
                    workBook.Close()
                    excelApp.Quit()

                    MsgBox("Laporan berhasil diekspor ke Excel!", MsgBoxStyle.Information)
                End If
            End Using

        Catch ex As Exception
            MsgBox("Terjadi kesalahan: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            Me.Cursor = Cursors.Default ' Kembalikan cursor ke default
        End Try
    End Sub



End Class