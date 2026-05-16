Imports System.IO
Imports System.Runtime.InteropServices
Imports Excel = Microsoft.Office.Interop.Excel

Public Class FormPenjualanPPn


    Private Sub FormPenjualanPPn_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        MuatComboBoxBulanTahun(CmbBln, CmbThn)
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

    Private Sub ExportToExcel(ByVal JENIS As String)
        Dim excelApp As Excel.Application = Nothing
        Dim workBook As Excel.Workbook = Nothing
        Dim worksheet As Excel.Worksheet = Nothing

        Try
            Me.Cursor = Cursors.WaitCursor
            ProgressBar1.Visible = True
            ProgressBar1.Value = 0
            If LabelProgress IsNot Nothing Then LabelProgress.Text = "Menyiapkan data..."

            Application.DoEvents()

            ' ================= FILTER =================
            Dim TanggalAwal As Date
            Dim TanggalAkhir As Date

            If CbTanggal.Checked Then
                TanggalAwal = DTPAwal.Value.Date
                TanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
            ElseIf CbBulan.Checked Then
                If Not GetRentangBulan(CmbBln, CmbThn, TanggalAwal, TanggalAkhir) Then Exit Sub
            Else
                MessageBox.Show("Pilih filter tanggal atau bulan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If

            ' ================= VALIDASI =================
            If DGVFilter.Rows.Count = 0 Then
                MessageBox.Show("Data belum tersedia!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If

            ' ================= ID BARANG =================
            Dim idBarang As New List(Of String)
            For Each r As DataGridViewRow In DGVFilter.Rows
                If Not r.IsNewRow Then idBarang.Add(r.Cells(0).Value.ToString())
            Next

            ' ================= QUERY =================
            Dim paramNames As New List(Of String)
            Dim paramList As New List(Of MySqlParameter)

            For i As Integer = 0 To idBarang.Count - 1
                Dim p As String = "@id" & i
                paramNames.Add(p)
                paramList.Add(New MySqlParameter(p, idBarang(i)))
            Next

            Dim query As String =
            "SELECT TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN, SUM(TOTAL_HARGA) TOTAL_HARGA " &
            "FROM penjualan_detail " &
            If(JENIS = "NonPPn", "WHERE ID_BARANG IN ", "WHERE ID_BARANG NOT IN ") &
            "(" & String.Join(",", paramNames) & ") " &
            "AND TANGGAL_JUAL BETWEEN @Awal AND @Akhir " &
            "GROUP BY TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN"

            Dim dt As New DataTable
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddRange(paramList.ToArray())
                cmd.Parameters.AddWithValue("@Awal", TanggalAwal)
                cmd.Parameters.AddWithValue("@Akhir", TanggalAkhir)
                Using adp As New MySqlDataAdapter(cmd)
                    adp.Fill(dt)
                End Using
            End Using

            If dt.Rows.Count = 0 Then
                MessageBox.Show("Tidak ada data!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            ' ================= SAVE DIALOG =================
            Using sfd As New SaveFileDialog
                sfd.Filter = "Excel Files|*.xlsx"
                sfd.FileName = If(JENIS = "NonPPn", "Laporan_Non_PPN.xlsx", "Laporan_PPN.xlsx")
                If sfd.ShowDialog <> DialogResult.OK Then Exit Sub
                If IsFileOpen(sfd.FileName) Then
                    MessageBox.Show("File Excel sedang terbuka!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Exit Sub
                End If

                ' ================= EXCEL INIT =================
                excelApp = New Excel.Application With {
                .Visible = False,
                .DisplayAlerts = False,
                .ScreenUpdating = False,
                .EnableEvents = False
            }

                workBook = excelApp.Workbooks.Add()
                worksheet = CType(workBook.Sheets(1), Excel.Worksheet)

                ' ================= JUDUL =================
                ' ===== HEADER LAPORAN (FIX FINAL) =====
                worksheet.Cells(1, 1).Value = "LAPORAN PENJUALAN " & NAMA_PERUSAHAAN
                worksheet.Cells(2, 1).Value =
    If(JENIS = "NonPPn",
       "JENIS BARANG : " & TxtKunci.Text.ToUpper(),
       "JENIS BARANG SELAIN : " & TxtKunci.Text.ToUpper())
                worksheet.Cells(3, 1).Value =
    "Periode: " & TanggalAwal.ToString("dd/MM/yy HH:mm:ss") &
    " - " & TanggalAkhir.ToString("dd/MM/yy HH:mm:ss")
                worksheet.Cells(4, 1).Value =
    "Dicetak pada: " & DateTime.Now.ToString("dd/MM/yy HH:mm:ss")

                worksheet.Range("A1:E1").Merge()
                worksheet.Range("A2:E2").Merge()
                worksheet.Range("A3:E3").Merge()
                worksheet.Range("A4:E4").Merge()

                worksheet.Range("A1:E4").HorizontalAlignment =
    Excel.XlHAlign.xlHAlignCenter


                ' ================= HEADER =================
                worksheet.Range("A5:E5").Value =
                New Object() {"NO", "TANGGAL", "FAKTUR", "PELANGGAN", "TOTAL"}
                worksheet.Range("A5:E5").Font.Bold = True
                worksheet.Range("A5:E5").Interior.Color =
                ColorTranslator.ToOle(ModuleTheme.C(Color.LightGray, Color.FromArgb(64, 64, 64)))

                ' ================= PROGRESS SETUP =================
                Dim rowCount As Integer = dt.Rows.Count
                ProgressBar1.Maximum = rowCount
                ProgressBar1.Value = 0

                If LabelProgress IsNot Nothing Then
                    LabelProgress.Text = "Menyiapkan data Excel..."
                End If
                Application.DoEvents()

                ' ================= ARRAY BUILD =================
                Dim data(rowCount - 1, 4) As Object
                Dim subtotal As Double = 0

                For i As Integer = 0 To rowCount - 1
                    data(i, 0) = i + 1
                    data(i, 1) = CDate(dt.Rows(i)("TANGGAL_JUAL"))
                    data(i, 2) = dt.Rows(i)("FAKTUR_JUAL").ToString()
                    data(i, 3) = dt.Rows(i)("NAMA_PELANGGAN").ToString()
                    data(i, 4) = CDbl(dt.Rows(i)("TOTAL_HARGA"))

                    subtotal += CDbl(data(i, 4))

                    ProgressBar1.Value = i + 1
                    If LabelProgress IsNot Nothing Then
                        LabelProgress.Text = $"Menulis data {i + 1:N0} / {rowCount:N0}"
                    End If

                    If i Mod 50 = 0 Then Application.DoEvents()
                Next

                ' ================= WRITE TO EXCEL =================
                worksheet.Range("A6").Resize(rowCount, 5).Value2 = data
                worksheet.Range("B6:B" & (5 + rowCount)).NumberFormat = "dd/MM/yy HH:mm:ss"

                ' ================= SUBTOTAL =================
                worksheet.Cells(6 + rowCount, 4).Value = "SUBTOTAL"
                worksheet.Cells(6 + rowCount, 5).Value = subtotal
                worksheet.Range("D" & (6 + rowCount) & ":E" & (6 + rowCount)).Font.Bold = True

                worksheet.UsedRange.Columns.AutoFit()

                workBook.SaveAs(sfd.FileName)
                workBook.Close(False)
                excelApp.Quit()

                If LabelProgress IsNot Nothing Then
                    LabelProgress.Text = "Export selesai"
                End If

                MessageBox.Show("Export Excel selesai dengan progress realtime!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            If worksheet IsNot Nothing Then Marshal.ReleaseComObject(worksheet)
            If workBook IsNot Nothing Then Marshal.ReleaseComObject(workBook)
            If excelApp IsNot Nothing Then Marshal.ReleaseComObject(excelApp)

            worksheet = Nothing
            workBook = Nothing
            excelApp = Nothing

            ProgressBar1.Value = 0
            ProgressBar1.Visible = False
            Me.Cursor = Cursors.Default

            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
    End Sub

    Private Sub BtnCekDataNonPPn_Click(sender As Object, e As EventArgs) Handles BtnCekDataNonPPn.Click
        TampilkanDataKeGrid("NonPPn")
    End Sub

    Private Sub BtnCekDataPpn_Click(sender As Object, e As EventArgs) Handles BtnCekDataPpn.Click
        TampilkanDataKeGrid("PPn")
    End Sub

    Private Sub TampilkanDataKeGrid(ByVal JENIS As String)
        Try
            Me.Cursor = Cursors.WaitCursor

            ' ================= FILTER TANGGAL =================
            Dim TanggalAwal As Date
            Dim TanggalAkhir As Date

            If CbTanggal.Checked Then
                TanggalAwal = DTPAwal.Value.Date
                TanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
            ElseIf CbBulan.Checked Then
                If Not GetRentangBulan(CmbBln, CmbThn, TanggalAwal, TanggalAkhir) Then Exit Sub
            Else
                MessageBox.Show("Pilih filter tanggal atau bulan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If

            ' ================= VALIDASI =================
            If DGVFilter.Rows.Count = 0 Then
                MessageBox.Show("Daftar barang masih kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If

            ' ================= ID BARANG =================
            Dim idBarang As New List(Of String)
            For Each r As DataGridViewRow In DGVFilter.Rows
                If Not r.IsNewRow Then
                    idBarang.Add(r.Cells(0).Value.ToString())
                End If
            Next

            ' ================= PARAMETER =================
            Dim paramNames As New List(Of String)
            Dim paramList As New List(Of MySqlParameter)

            For i As Integer = 0 To idBarang.Count - 1
                Dim p As String = "@id" & i
                paramNames.Add(p)
                paramList.Add(New MySqlParameter(p, idBarang(i)))
            Next

            ' ================= QUERY (SAMA DENGAN EXPORT) =================
            Dim query As String =
            "SELECT " &
            "TANGGAL_JUAL, FAKTUR_JUAL, NAMA_PELANGGAN, ID_BARANG, TOTAL_HARGA " &
            "FROM penjualan_detail " &
            If(JENIS = "NonPPn", "WHERE ID_BARANG IN ", "WHERE ID_BARANG NOT IN ") &
            "(" & String.Join(",", paramNames) & ") " &
            "AND TANGGAL_JUAL BETWEEN @Awal AND @Akhir " &
            "ORDER BY TANGGAL_JUAL, FAKTUR_JUAL"

            ' ================= AMBIL DATA =================
            Dim dt As New DataTable
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddRange(paramList.ToArray())
                cmd.Parameters.AddWithValue("@Awal", TanggalAwal)
                cmd.Parameters.AddWithValue("@Akhir", TanggalAkhir)

                Using adp As New MySqlDataAdapter(cmd)
                    adp.Fill(dt)
                End Using
            End Using

            ' ================= TAMPILKAN KE GRID =================
            DgvData.AutoGenerateColumns = True
            DgvData.DataSource = dt

            ' ================= FORMAT GRID =================
            With DgvData
                .Columns("TANGGAL_JUAL").DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss"
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            End With
            ModuleAngka.TerapkanFormatKolomAngka(DgvData, "TOTAL_HARGA")

            If dt.Rows.Count = 0 Then
                MessageBox.Show("Tidak ada data untuk ditampilkan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show("Gagal menampilkan data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub DgvData_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvData.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DgvData.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub
End Class