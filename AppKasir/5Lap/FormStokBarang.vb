Public Class FormStokBarang
    Private mRow As Integer = 0
    Private newpage As Boolean = True

    Public Sub Kondisiawal()
        Using da As New MySqlDataAdapter("SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, HARGA_BELI, STOK_TOKO, STOK_GUDANG FROM tbl_barang order by NAMA_BARANG ASC", conn)
            Using ds As New DataSet()
                da.Fill(ds, "tbl_barang")
                DataGridView1.DataSource = ds.Tables("tbl_barang")
                AturDataGridView()
            End Using
        End Using
    End Sub

    Public Sub AturDataGridView()
        With DataGridView1
            If FormUtama.StatusLevelUser.Text = "Kasir" Or FormUtama.StatusLevelUser.Text = "Admin" Then
                .Columns("HARGA_BELI").Visible = False
            End If

            .Columns("ID_BARANG").HeaderText = "ID BARANG"
            .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
            .Columns("NAMA_KATEGORI").HeaderText = "NAMA KATEGORI"
            .Columns("HARGA_BELI").HeaderText = "HARGA BELI"
            .Columns("STOK_TOKO").HeaderText = "STOK TOKO"
            .Columns("STOK_GUDANG").HeaderText = "STOK GUDANG"
        End With
        ModuleAngka.TerapkanFormatKolomAngka(DataGridView1, "HARGA_BELI")
        HitungTotalHarga()
        HitungJumlahBarisDataBarang()
    End Sub

    Public Sub HitungTotalHarga()
        Dim hitungRp As Decimal = 0D ' Menggunakan Decimal
        Dim totalStokToko As Integer = 0
        Dim totalStokGudang As Integer = 0

        For baris As Integer = 0 To DataGridView1.RowCount - 1
            Dim hargaBeli As Decimal = Convert.ToDecimal(DataGridView1.Rows(baris).Cells(3).Value)
            Dim stokToko As Decimal = Convert.ToDecimal(DataGridView1.Rows(baris).Cells(4).Value)
            Dim stokGudang As Decimal = Convert.ToDecimal(DataGridView1.Rows(baris).Cells(5).Value)

            Dim totalRp As Decimal = hargaBeli * (stokToko + stokGudang)
            hitungRp += totalRp

            ' Menghitung total stok toko dan total stok gudang
            totalStokToko += stokToko
            totalStokGudang += stokGudang
        Next

        ' Menetapkan hasil pada label
        LblRp.Text = hitungRp.ToString("N2")
        LblToko.Text = totalStokToko.ToString("N2")
        LblGudang.Text = totalStokGudang.ToString("N2")
        LblQty.Text = (totalStokToko + totalStokGudang).ToString("N2")
    End Sub

    Public Sub HitungJumlahBarisDataBarang()
        Dim jumlahBaris As Integer = DataGridView1.Rows.Count
        LblRecord.Text = jumlahBaris.ToString("N2")
    End Sub


    Private Sub FormStokBarang_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Cursor = Cursors.WaitCursor
        Kondisiawal()
        Cursor = Cursors.Default
    End Sub

    Private Sub TextBoxCari_TextChanged_1(ByVal sender As Object, ByVal e As EventArgs) Handles TextBoxCari.TextChanged
        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, HARGA_BELI, STOK_TOKO, STOK_GUDANG FROM tbl_barang where ID_BARANG like @parameter or NAMA_BARANG like @parameter"

        Using da As New MySqlDataAdapter(query, conn)
            da.SelectCommand.Parameters.AddWithValue("@parameterName", "%" & TextBoxCari.Text & "%")
            Using ds As New DataSet()
                da.Fill(ds)
                DataGridView1.DataSource = ds.Tables(0)
                AturDataGridView()
            End Using
        End Using
    End Sub

    Private Sub FormStokBarang_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
        Using i As New System.Drawing.Drawing2D.LinearGradientBrush(ClientRectangle, ModuleTheme.C(Color.MediumPurple, Color.FromArgb(40, 44, 52)), ModuleTheme.C(Color.ForestGreen, Color.FromArgb(60, 64, 72)), Drawing2D.LinearGradientMode.BackwardDiagonal)
            e.Graphics.FillRectangle(i, ClientRectangle)
        End Using
    End Sub

    Private Sub BtnStokKosong_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnStokKosong.Click
        LblHeaderForm.Text = "DATA BARANG KOSONG"

        Using da As New MySqlDataAdapter("SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, HARGA_BELI, STOK_TOKO, STOK_GUDANG FROM tbl_barang where STOK_TOKO = 0 and STOK_GUDANG = 0", conn)
            Using ds As New DataSet()
                da.Fill(ds, "tbl_barang")
                DataGridView1.DataSource = ds.Tables("tbl_barang")
                AturDataGridView()
            End Using
        End Using
    End Sub


    Private Sub BtnTampilSemua_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTampilSemua.Click
        LblHeaderForm.Text = "DATA BARANG"
        Kondisiawal()

    End Sub

    Private Sub BtnStok_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnStok.Click
        LblHeaderForm.Text = "D A T A   S T O K  S P A R E P A R T  T I D A K  K O S O N G"

        Using da As New MySqlDataAdapter("SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, HARGA_BELI, STOK_TOKO, STOK_GUDANG FROM tbl_barang where STOK_TOKO <> 0 and STOK_GUDANG <> 0", conn)
            Using ds As New DataSet()
                da.Fill(ds, "tbl_barang")
                DataGridView1.DataSource = ds.Tables("tbl_barang")
                AturDataGridView()
            End Using
        End Using
    End Sub


    Private Sub ButtonKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonKeluar.Click
        Close()
    End Sub


    Private Sub BtnCetak_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnCetak.Click
        If DataGridView1.Rows.Count < 1 Then
            MessageBox.Show("Tidak ada data", "Kosong", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Try
                With PrintPreviewDialog1
                    .Document = PrintDocument1
                    .ShowDialog()
                End With
            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

    End Sub

    Private Sub PrintDocument1_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage
        ' sets it to show '...' for long text
        Dim fmt As New StringFormat With {
    .LineAlignment = StringAlignment.Center,
    .Trimming = StringTrimming.EllipsisCharacter
}
        Dim y As Int32 = e.MarginBounds.Top
        Dim rc As System.Drawing.Rectangle
        Dim x As Int32
        Dim h As Int32 = 0
        Dim row As DataGridViewRow

        ' judul documen
        Dim fontheader As New System.Drawing.Font("Arial", 18, FontStyle.Bold)
        e.Graphics.DrawString(LblHeaderForm.Text, fontheader, Brushes.Black, 250, 50)

        ' print the header text for a new page
        '   use a grey bg just like the control
        If newpage Then
            row = DataGridView1.Rows(mRow)
            x = e.MarginBounds.Left
            For Each cell As DataGridViewCell In row.Cells
                ' since we are printing the control's view,
                ' skip invidible columns
                If cell.Visible Then
                    rc = New System.Drawing.Rectangle(x, y, cell.Size.Width, cell.Size.Height)

                    e.Graphics.FillRectangle(Brushes.LightGray, rc)
                    e.Graphics.DrawRectangle(Pens.Black, rc)

                    ' reused in the data pront - should be a function
                    Select Case DataGridView1.Columns(cell.ColumnIndex).DefaultCellStyle.Alignment
                        Case DataGridViewContentAlignment.BottomRight,
                             DataGridViewContentAlignment.MiddleRight
                            fmt.Alignment = StringAlignment.Far
                            rc.Offset(-1, 0)
                        Case DataGridViewContentAlignment.BottomCenter,
                            DataGridViewContentAlignment.MiddleCenter
                            fmt.Alignment = StringAlignment.Center
                        Case Else
                            fmt.Alignment = StringAlignment.Near
                            rc.Offset(2, 0)
                    End Select

                    e.Graphics.DrawString(DataGridView1.Columns(cell.ColumnIndex).HeaderText,
                                                DataGridView1.Font, Brushes.Black, rc, fmt)
                    x += rc.Width
                    h = Math.Max(h, rc.Height)
                End If
            Next
            y += h

        End If
        newpage = False

        ' now print the data for each row
        Dim thisNDX As Int32
        For thisNDX = mRow To DataGridView1.RowCount - 1
            ' no need to try to print the new row
            If DataGridView1.Rows(thisNDX).IsNewRow Then Exit For

            row = DataGridView1.Rows(thisNDX)
            h = 0

            ' reset X for data
            x = e.MarginBounds.Left

            ' print the data
            For Each cell As DataGridViewCell In row.Cells
                If cell.Visible Then
                    rc = New System.Drawing.Rectangle(x, y, cell.Size.Width, cell.Size.Height)

                    e.Graphics.DrawRectangle(Pens.Black, rc)

                    Select Case DataGridView1.Columns(cell.ColumnIndex).DefaultCellStyle.Alignment
                        Case DataGridViewContentAlignment.BottomRight,
                             DataGridViewContentAlignment.MiddleRight
                            fmt.Alignment = StringAlignment.Far
                            rc.Offset(-1, 0)
                        Case DataGridViewContentAlignment.BottomCenter,
                            DataGridViewContentAlignment.MiddleCenter
                            fmt.Alignment = StringAlignment.Center
                        Case Else
                            fmt.Alignment = StringAlignment.Near
                            rc.Offset(2, 0)
                    End Select

                    e.Graphics.DrawString(cell.FormattedValue.ToString(),
                                          DataGridView1.Font, Brushes.Black, rc, fmt)

                    x += rc.Width
                    h = Math.Max(h, rc.Height)
                End If

            Next
            y += h
            ' next row to print
            mRow = thisNDX + 1

            If y + h > e.MarginBounds.Bottom Then
                e.HasMorePages = True
                ' mRow -= 1   causes last row to rePrint on next page
                newpage = True
                Return
            End If
        Next
    End Sub

    Private Sub PrintDocument1_BeginPrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles PrintDocument1.BeginPrint
        mRow = 0
        newpage = True
        PrintPreviewDialog1.PrintPreviewControl.StartPage = 0
        PrintPreviewDialog1.PrintPreviewControl.Zoom = 1.0

    End Sub


    Private Sub PrintDocument1_QueryPageSettings(ByVal sender As Object, ByVal e As System.Drawing.Printing.QueryPageSettingsEventArgs) Handles PrintDocument1.QueryPageSettings
        e.PageSettings.Landscape = True
    End Sub




    'Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
    '    Cursor = Cursors.WaitCursor
    '    Dim xlApp As New Application
    '    Dim xlWorkBook As Workbook
    '    Dim xlWorkSheet As Worksheet
    '    xlWorkBook = xlApp.Workbooks.Add
    '    xlWorkSheet = xlWorkBook.Sheets("sheet1")


    '    For k As Integer = 0 To DataGridView1.Columns.Count - 1
    '        xlWorkSheet.Cells(1, k + 1) = DataGridView1.Columns(k).HeaderText
    '    Next

    '    'For i = 0 To DataGridView1.Rows.Count - 1
    '    '    For j = 0 To DataGridView1.Columns.Count - 1
    '    '        xlWorkSheet.Cells(i + 2, j + 1) = DataGridView1.Rows(i).Cells(j).Value.ToString()
    '    '    Next
    '    'Next
    '    If xlWorkSheet IsNot Nothing Then
    '        Dim i As Integer, j As Integer ' Declare variables i and j
    '        For i = 0 To DataGridView1.Rows.Count - 1
    '            For j = 0 To DataGridView1.Columns.Count - 1
    '                If DataGridView1.Rows(i).Cells(j).Value IsNot Nothing Then
    '                    xlWorkSheet.Cells(i + 2, j + 1) = DataGridView1.Rows(i).Cells(j).Value.ToString()
    '                End If
    '            Next
    '        Next
    '    End If

    '    xlWorkBook.SaveAs(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, LblJudul.Text))
    '    xlWorkBook.Close()
    '    xlApp.Quit()
    '    MsgBox("Tidak ada data", vbCritical, "Kosong")
    '    Cursor = Cursors.Default
    'End Sub



End Class
