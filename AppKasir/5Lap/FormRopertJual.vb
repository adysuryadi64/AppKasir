Imports ClosedXML.Excel

Public Class FormRopertJual


    Private Sub CbBulan_CheckedChanged(sender As Object, e As EventArgs) Handles CBBulan.CheckedChanged
        If CBBulan.Checked Then
            CBTanggal.Checked = False
            CmbBln.SelectedIndex = DateTime.Now.Month - 1 ' Menyesuaikan indeks (0-based)
        End If
    End Sub


    Private Sub CbTanggal_CheckedChanged(sender As Object, e As EventArgs) Handles CBTanggal.CheckedChanged
        If CBTanggal.Checked Then
            CBBulan.Checked = False
        End If
    End Sub

    Private Sub FormRopertJual_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        MuatComboBoxBulanTahun(CmbBln, CmbThn)
        Tampilkankategori()
        CBBulan.Checked = True ' Set default ke filter tanggal
    End Sub


    Private Sub Tampilkankategori()
        CmbKategori.Items.Clear()

        Dim query As String = "
        SELECT nama 
        FROM tbl_kategori 
        WHERE nama IS NOT NULL AND nama <> ''
        ORDER BY nama;
    "

        Try
            Using cmd As New MySqlCommand(query, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        ' Tambahkan opsi "Semua Kategori" di awal
                        CmbKategori.Items.Add("-- Semua Kategori --")
                        ' Tambahkan opsi kosong
                        CmbKategori.Items.Add("")

                        ' Tambahkan semua kategori dari database, jika tidak kosong
                        While reader.Read()
                            Dim kategori As String = reader("nama").ToString().Trim()
                            If kategori <> "" Then
                                CmbKategori.Items.Add(kategori)
                            End If
                        End While

                        ' Pilih default sebagai "Semua Kategori"
                        CmbKategori.SelectedIndex = 0
                        CmbKategori.Visible = True
                    Else
                        CmbKategori.Visible = False ' Tidak ada kategori valid
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Gagal mengambil kategori: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Dim TanggalAwal As Date
    Dim TanggalAkhir As Date
    Private Sub TentukanTanggalAwalAkhir()
        ' Tentukan rentang tanggal berdasarkan filter yang dipilih
        If CBTanggal.Checked Then
            TanggalAwal = DTPAwal.Value.Date
            TanggalAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
        ElseIf CBBulan.Checked Then
            If Not GetRentangBulan(CmbBln, CmbThn, TanggalAwal, TanggalAkhir) Then Exit Sub
        Else
            MessageBox.Show("Pilih filter tanggal atau bulan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If
    End Sub
    Private Sub BtnProses_Click(sender As Object, e As EventArgs) Handles BtnProses.Click
        LblStatus.Text = "detail"
        TentukanTanggalAwalAkhir()

        Dim query As String = "
        SELECT 
            ip.FAKTUR_JUAL AS kode,
            ip.TANGGAL_JUAL AS tanggal,
            IFNULL(b.BARCODE_KECIL, '') AS BARCODE,
            ip.ID_BARANG AS kode_barang,
            ip.NAMA_BARANG AS nama_barang,
            ip.QTY_SATUAN AS qty,
            IFNULL(b.SATUAN_STOK, '') AS satuan,
            IFNULL(b.KODE_KATEGORI, '') AS kode_kategori,
            IFNULL(b.NAMA_KATEGORI, '') AS nama_kategori
        FROM penjualan_detail ip
        LEFT JOIN tbl_barang b ON ip.ID_BARANG = b.ID_BARANG
        WHERE ip.TANGGAL_JUAL BETWEEN @AwalBulan AND @AkhirBulan
        AND b.NAMA_KATEGORI LIKE @Kategori
    "

        Try
            Cursor = Cursors.WaitCursor

            Using cmd As New MySqlCommand(query, conn)
                With cmd.Parameters
                    .AddWithValue("@AwalBulan", TanggalAwal)
                    .AddWithValue("@AkhirBulan", TanggalAkhir)

                    Dim kategori = CmbKategori.Text.Trim()
                    If kategori = "" OrElse kategori = "-- Semua Kategori --" Then
                        .AddWithValue("@Kategori", "%")
                    Else
                        .AddWithValue("@Kategori", kategori)
                    End If
                End With

                Using adapter As New MySqlDataAdapter(cmd)
                    Dim table As New DataTable
                    adapter.Fill(table)
                    DgvData.DataSource = table
                End Using

                With DgvData
                    .Columns("kode").HeaderText = "No Transaksi"
                    .Columns("tanggal").HeaderText = "Tanggal"
                    .Columns("BARCODE").HeaderText = "Barcode"
                    .Columns("kode_barang").HeaderText = "Kode Barang"
                    .Columns("nama_barang").HeaderText = "Nama Barang"
                    .Columns("qty").HeaderText = "Qty"
                    .Columns("satuan").HeaderText = "Satuan"
                    .Columns("nama_kategori").HeaderText = "Nama Kategori"

                    .Columns("kode_kategori").Visible = False

                    .Columns("tanggal").DefaultCellStyle.Format = "dd-MM-yyyy"
                    .Columns("tanggal").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

                    ' Lebar kolom (opsional)
                    .Columns("kode").Width = 100
                    .Columns("tanggal").Width = 90
                    .Columns("BARCODE").Width = 100
                    .Columns("kode_barang").Width = 100
                    .Columns("nama_barang").Width = 300
                    .Columns("qty").Width = 70
                    .Columns("satuan").Width = 80
                    .Columns("nama_kategori").Width = 130
                End With
                ModuleAngka.TerapkanFormatKolomAngka(DgvData, "qty")
            End Using

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            Cursor = Cursors.Default
        End Try
    End Sub


    Private Sub BtnRekap_Click(sender As Object, e As EventArgs) Handles BtnRekap.Click
        LblStatus.Text = "rekap"
        TentukanTanggalAwalAkhir()

        Dim query As String = "
        SELECT 
            IFNULL(b.BARCODE_KECIL, '') AS BARCODE,
            ip.ID_BARANG AS kode_barang,
            ip.NAMA_BARANG AS nama_barang,
            SUM(ip.QTY_SATUAN) AS qty,
            IFNULL(b.SATUAN_STOK, '') AS satuan,
            IFNULL(b.KODE_KATEGORI, '') AS kode_kategori,
            IFNULL(b.NAMA_KATEGORI, '') AS nama_kategori
        FROM penjualan_detail ip
        LEFT JOIN tbl_barang b ON ip.ID_BARANG = b.ID_BARANG
        WHERE ip.TANGGAL_JUAL BETWEEN @AwalBulan AND @AkhirBulan
        AND b.NAMA_KATEGORI LIKE @Kategori
        GROUP BY ip.ID_BARANG, ip.NAMA_BARANG, b.BARCODE_KECIL, b.SATUAN_STOK, b.KODE_KATEGORI, b.NAMA_KATEGORI
        ORDER BY ip.NAMA_BARANG;
    "

        Try
            Cursor = Cursors.WaitCursor

            Using cmd As New MySqlCommand(query, conn)
                With cmd.Parameters
                    .AddWithValue("@AwalBulan", TanggalAwal)
                    .AddWithValue("@AkhirBulan", TanggalAkhir)

                    Dim filterKategori = CmbKategori.Text.Trim()
                    If filterKategori = "-- Semua Kategori --" OrElse filterKategori = "" Then
                        .AddWithValue("@Kategori", "%")
                    Else
                        .AddWithValue("@Kategori", filterKategori)
                    End If
                End With

                Using adapter As New MySqlDataAdapter(cmd)
                    Dim table As New DataTable
                    adapter.Fill(table)
                    DgvData.DataSource = table
                End Using

                With DgvData
                    .Columns("BARCODE").HeaderText = "Barcode"
                    .Columns("kode_barang").HeaderText = "Kode Barang"
                    .Columns("nama_barang").HeaderText = "Nama Barang"
                    .Columns("qty").HeaderText = "Qty"
                    .Columns("satuan").HeaderText = "Satuan"
                    .Columns("nama_kategori").HeaderText = "Nama Kategori"

                    .Columns("kode_kategori").Visible = False

                    .Columns("BARCODE").Width = 100
                    .Columns("kode_barang").Width = 100
                    .Columns("nama_barang").Width = 300
                    .Columns("qty").Width = 70
                    .Columns("satuan").Width = 80
                    .Columns("nama_kategori").Width = 130
                End With
                ModuleAngka.TerapkanFormatKolomAngka(DgvData, "qty")
            End Using

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            Cursor = Cursors.Default
        End Try
    End Sub



    Private Sub DgvData_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvData.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DgvData.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub


    Private Sub BtnExport_Click(sender As Object, e As EventArgs) Handles BtnExport.Click
        If DgvData.Rows.Count = 0 Then
            MessageBox.Show("Tidak ada data yang bisa diekspor.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        Cursor = Cursors.WaitCursor

        ' Ambil nama perusahaan
        Dim namaPerusahaan As String = ""
        Using cmd As New MySqlCommand("SELECT NAMA FROM tbl_perusahaan LIMIT 1", conn)
            Dim result = cmd.ExecuteScalar()
            namaPerusahaan = If(result IsNot Nothing, result.ToString(), "Nama Perusahaan")
        End Using

        ' Tentukan jenis laporan berdasarkan status
        Dim jenisLaporan As String = If(LblStatus.Text = "detail", "LAPORAN DETAIL PENJUALAN PER INVOICE", "REKAPITULASI PENJUALAN PER NAMA BARANG")

        ' Tentukan periode laporan berdasarkan pilihan tanggal atau bulan
        Dim periodeLaporan As String = If(CBTanggal.Checked,
    $"PERIODE: {TanggalAwal:dd/MM/yyyy} S.D. {TanggalAkhir:dd/MM/yyyy}",
    $"PERIODE: BULAN {CmbBln.Text.ToUpper()} {CmbThn.Text}"
)


        Dim saveDialog As New SaveFileDialog With {
            .Filter = "Excel Files (*.xlsx)|*.xlsx",
            .FileName = "Laporan_Penjualan_" & CmbKategori.Text & "_" & Now.ToString("yyMMdd_HHmm") & ".xlsx"
        }

        If saveDialog.ShowDialog() = DialogResult.OK Then
            Dim filePath As String = saveDialog.FileName

            Using workbook As New XLWorkbook()
                Dim sheet = workbook.Worksheets.Add("Laporan")

                Dim visibleColsCount = DgvData.Columns.Cast(Of DataGridViewColumn).Count(Function(c) c.Visible)
                Dim totalCols = visibleColsCount + 1

                sheet.Range(sheet.Cell(1, 1), sheet.Cell(1, totalCols)).Merge()
                sheet.Range(sheet.Cell(2, 1), sheet.Cell(2, totalCols)).Merge()
                sheet.Range(sheet.Cell(3, 1), sheet.Cell(3, totalCols)).Merge()

                sheet.Cell(1, 1).Value = namaPerusahaan.ToUpper()
                sheet.Cell(2, 1).Value = jenisLaporan
                sheet.Cell(3, 1).Value = periodeLaporan

                Dim headerRange = sheet.Range(sheet.Cell(1, 1), sheet.Cell(3, 1))
                headerRange.Style.Font.Bold = True
                headerRange.Style.Font.FontSize = 12
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center

                sheet.Cell(5, 1).Value = "No"
                sheet.Cell(5, 1).Style.Font.Bold = True
                sheet.Cell(5, 1).Style.Fill.BackgroundColor = XLColor.FromColor(ModuleTheme.C(Color.LightGray, Color.FromArgb(64, 64, 64)))
                sheet.Cell(5, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center

                Dim colIndex As Integer = 2
                For Each col As DataGridViewColumn In DgvData.Columns
                    If col.Visible Then
                        sheet.Cell(5, colIndex).Value = col.HeaderText
                        sheet.Cell(5, colIndex).Style.Font.Bold = True
                        sheet.Cell(5, colIndex).Style.Fill.BackgroundColor = XLColor.FromColor(ModuleTheme.C(Color.LightGray, Color.FromArgb(64, 64, 64)))
                        sheet.Cell(5, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center
                        colIndex += 1
                    End If
                Next

                Dim rowIndex As Integer = 6
                For Each row As DataGridViewRow In DgvData.Rows
                    If Not row.IsNewRow Then
                        sheet.Cell(rowIndex, 1).Value = rowIndex - 5
                        sheet.Cell(rowIndex, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center

                        colIndex = 2
                        For Each col As DataGridViewColumn In DgvData.Columns
                            If col.Visible Then
                                Dim cellValue = row.Cells(col.Index).Value
                                Dim colName = col.Name.ToLower()

                                If cellValue Is Nothing OrElse IsDBNull(cellValue) Then
                                    sheet.Cell(rowIndex, colIndex).Value = ""
                                ElseIf colName = "tanggal" Then
                                    Dim tanggalValue As Date
                                    If Date.TryParse(cellValue.ToString(), tanggalValue) Then
                                        sheet.Cell(rowIndex, colIndex).Value = tanggalValue
                                        sheet.Cell(rowIndex, colIndex).Style.DateFormat.Format = "dd/MM/yyyy"
                                    Else
                                        sheet.Cell(rowIndex, colIndex).Value = cellValue.ToString()
                                    End If
                                ElseIf colName = "qty" Then
                                    Dim qtyVal As Double
                                    If Double.TryParse(cellValue.ToString().Replace(",", ""), qtyVal) Then
                                        sheet.Cell(rowIndex, colIndex).Value = qtyVal
                                        sheet.Cell(rowIndex, colIndex).Style.NumberFormat.Format = "0"
                                    Else
                                        sheet.Cell(rowIndex, colIndex).Value = cellValue.ToString()
                                    End If
                                Else
                                    sheet.Cell(rowIndex, colIndex).Value = cellValue.ToString()
                                End If
                                colIndex += 1
                            End If
                        Next
                        rowIndex += 1
                    End If
                Next

                sheet.Columns().AdjustToContents()

                ' Format tanggal jika ada
                Dim tanggalColName = "tanggal"
                If DgvData.Columns.Contains(tanggalColName) Then
                    Dim dateColIndex = DgvData.Columns(tanggalColName).Index + 2
                    sheet.Range(sheet.Cell(6, dateColIndex), sheet.Cell(rowIndex - 1, dateColIndex)).Style.DateFormat.Format = "dd/MM/yyyy"
                End If

                ' Format qty jika ada
                Dim qtyColName = "qty"
                If DgvData.Columns.Contains(qtyColName) Then
                    Dim qtyColIndex = DgvData.Columns(qtyColName).Index + 2
                    sheet.Range(sheet.Cell(6, qtyColIndex), sheet.Cell(rowIndex - 1, qtyColIndex)).Style.NumberFormat.Format = "0"
                End If

                workbook.SaveAs(filePath)
            End Using

            Cursor = Cursors.Default

            Dim folderPath As String = IO.Path.GetDirectoryName(filePath)
            MessageBox.Show("Ekspor data berhasil disimpan." & Environment.NewLine &
                   "Folder penyimpanan akan dibuka dan file hasil ekspor akan dipilih untuk memudahkan akses Anda.",
                   "Sukses Ekspor", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Process.Start("explorer.exe", "/select,""" & filePath & """")
        End If
    End Sub



End Class