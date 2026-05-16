Imports System.Data

Public Class HistoriPembelianUC
    ' Events
    Public Event BarisDiklik(ByVal fakturBeli As String, ByVal harga As Decimal)
    Public Event RefreshData()

    ' Properties
    Private _kodeBarang As String = ""
    Private _maxRows As Integer = 20

    Public Property KodeBarang As String
        Get
            Return _kodeBarang
        End Get
        Set(value As String)
            _kodeBarang = value
            If Not String.IsNullOrEmpty(value) Then
                LoadData()
            End If
        End Set
    End Property

    Public Property MaxRows As Integer
        Get
            Return _maxRows
        End Get
        Set(value As Integer)
            _maxRows = value
            LoadData()
        End Set
    End Property

    ' Load Data Method
    Public Sub LoadData()
        If String.IsNullOrEmpty(_kodeBarang) Then
            DGVHistori.DataSource = Nothing
            LblStatistik.Text = "Silakan pilih barang"
            Return
        End If

        Cursor = Cursors.WaitCursor
        Try
            Dim query As String = "SELECT 
                `FAKTUR_BELI` as 'Faktur',
                `NOTA_BELI` as 'Nota Suplier',
                DATE_FORMAT(`TANGGAL_MASUK`, '%d/%m/%Y %H:%i') as 'Tanggal',
                `LOKASI` as 'Lokasi',
                `NAMA_SUPLIYER` as 'Suplier',
                FORMAT(`QTY`, 2) as 'Qty',
                `SATUAN` as 'Satuan',
                FORMAT(`HARGA_BELI`, 0) as 'Harga/Unit',
                FORMAT(`HARGA_BELI_SATUAN`, 0) as 'Harga/Satuan',
                FORMAT(`QTY_SAT`, 2) as 'Total Qty',
                FORMAT(`TOTAL`, 0) as 'Total Harga',
                `ID_USER` as 'User'
                FROM `pembelian_detail` 
                WHERE `ID_BARANG` = @ID_BARANG 
                ORDER BY `TANGGAL_MASUK` DESC 
                LIMIT @MaxRows"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@ID_BARANG", _kodeBarang)
                cmd.Parameters.AddWithValue("@MaxRows", _maxRows)

                Using da As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)

                    ' Bersihkan format .00 dan ,00 dari DataTable
                    CleanDecimalFormat(dt)

                    DGVHistori.DataSource = dt
                    FormatColumns()
                    UpdateStatistik(dt)
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Gagal memuat data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ' Membersihkan format .00 dan ,00 dari DataTable
    Private Sub CleanDecimalFormat(ByRef dt As DataTable)
        For Each row As DataRow In dt.Rows
            For Each column As DataColumn In dt.Columns
                If row(column) IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(row(column).ToString()) Then
                    Dim valueStr As String = row(column).ToString()

                    ' Hapus .00 atau ,00 di akhir angka
                    If valueStr.EndsWith(".00") Then
                        valueStr = valueStr.Replace(".00", "")
                    ElseIf valueStr.EndsWith(",00") Then
                        valueStr = valueStr.Replace(",00", "")
                    End If

                    ' Hapus pemisah ribuan untuk perhitungan statistik nanti
                    row(column) = valueStr
                End If
            Next
        Next
    End Sub

    Private Sub FormatColumns()
        ' Hapus baris kosong jika ada
        DGVHistori.AllowUserToAddRows = False

        ' Sembunyikan kolom yang tidak perlu
        If DGVHistori.Columns.Contains("User") Then
            DGVHistori.Columns("User").Visible = False
        End If

        ' Format kolom numeric dengan format kustom (tanpa .00)
        Dim numericColumns As String() = {"Harga/Unit", "Harga/Satuan", "Total Harga", "Qty", "Total Qty"}

        ModuleAngka.TerapkanFormatKolomAngka(DGVHistori, numericColumns)

        ' Format kolom tanggal agar rata kiri
        If DGVHistori.Columns.Contains("Tanggal") Then
            DGVHistori.Columns("Tanggal").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        End If

        ' Format kolom teks (Faktur, Nota, Lokasi, Suplier, Satuan)
        Dim textColumns As String() = {"Faktur", "Nota Suplier", "Lokasi", "Suplier", "Satuan"}
        For Each colName As String In textColumns
            If DGVHistori.Columns.Contains(colName) Then
                DGVHistori.Columns(colName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            End If
        Next

        ' Atur lebar kolom
        DGVHistori.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        DGVHistori.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        ' Atur warna header
        DGVHistori.EnableHeadersVisualStyles = False
        DGVHistori.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180) ' Steel Blue
        DGVHistori.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        ' Font diatur oleh tema - Font header DGV

        ' Alternating row colors
        DGVHistori.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240)

        ' Grid lines
        DGVHistori.GridColor = Color.FromArgb(200, 200, 200)

        ' Selection style
        DGVHistori.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DGVHistori.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 153, 255)
        DGVHistori.DefaultCellStyle.SelectionForeColor = Color.White

        ' Border style
        DGVHistori.BorderStyle = BorderStyle.FixedSingle
        DGVHistori.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        DGVHistori.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single

        ' Row headers
        DGVHistori.RowHeadersVisible = False

        ' Enable double buffering untuk performa lebih baik
        SetDoubleBuffered(DGVHistori)

        ' Auto size untuk kolom Suplier
        If DGVHistori.Columns.Contains("Suplier") Then
            DGVHistori.Columns("Suplier").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End If

        ' Atur tinggi header
        DGVHistori.ColumnHeadersHeight = 35

        ' Atur tinggi baris
        DGVHistori.RowTemplate.Height = 25
    End Sub

    ' Helper untuk double buffering
    Private Sub SetDoubleBuffered(ByVal dgv As DataGridView)
        Dim dgvType As Type = dgv.GetType()
        Dim pi As System.Reflection.PropertyInfo = dgvType.GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
        pi.SetValue(dgv, True, Nothing)
    End Sub

    Private Sub UpdateStatistik(ByVal dt As DataTable)
        If dt.Rows.Count > 0 Then
            Dim totalQty As Decimal = 0
            Dim totalNilai As Decimal = 0
            Dim hargaTerakhir As Decimal = 0

            For Each row As DataRow In dt.Rows
                ' Parse Total Qty (hilangkan pemisah ribuan jika ada)
                If row("Total Qty") IsNot DBNull.Value Then
                    Dim qtyStr As String = row("Total Qty").ToString().Replace(",", "").Replace(".", "")
                    Dim qty As Decimal = 0
                    If Decimal.TryParse(qtyStr, qty) Then
                        totalQty += qty
                    End If
                End If

                ' Parse Total Harga
                If row("Total Harga") IsNot DBNull.Value Then
                    Dim nilaiStr As String = row("Total Harga").ToString().Replace(".", "").Replace(",", "")
                    Dim nilai As Decimal = 0
                    If Decimal.TryParse(nilaiStr, nilai) Then
                        totalNilai += nilai
                    End If
                End If
            Next

            ' Ambil harga terakhir
            If dt.Rows(0)("Harga/Unit") IsNot DBNull.Value Then
                Dim hargaStr As String = dt.Rows(0)("Harga/Unit").ToString().Replace(".", "").Replace(",", "")
                Decimal.TryParse(hargaStr, hargaTerakhir)
            End If

            ' Format statistik tanpa .00
            LblStatistik.Text = String.Format(
                "{0} transaksi | Total Qty: {1:N0} | Total Nilai: Rp {2:N0} | Harga Terakhir: Rp {3:N0}",
                dt.Rows.Count, totalQty, totalNilai, hargaTerakhir)
        Else
            LblStatistik.Text = "Tidak ada data histori pembelian"
        End If
    End Sub

    ' Event Handlers
    Private Sub DGVHistori_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGVHistori.CellDoubleClick
        If e.RowIndex >= 0 AndAlso DGVHistori.Rows.Count > 0 Then
            Dim fakturBeli As String = ""
            Dim harga As Decimal = 0

            If DGVHistori.Rows(e.RowIndex).Cells("Faktur").Value IsNot Nothing Then
                fakturBeli = DGVHistori.Rows(e.RowIndex).Cells("Faktur").Value.ToString()
            End If

            If DGVHistori.Rows(e.RowIndex).Cells("Harga/Unit").Value IsNot Nothing Then
                Dim hargaStr As String = DGVHistori.Rows(e.RowIndex).Cells("Harga/Unit").Value.ToString()
                hargaStr = hargaStr.Replace(".", "").Replace(",", "")
                Decimal.TryParse(hargaStr, harga)
            End If

            RaiseEvent BarisDiklik(fakturBeli, harga)
        End If
    End Sub

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles BtnRefresh.Click
        LoadData()
    End Sub

    ' Tambahkan event untuk memastikan format tetap bersih saat data di-refresh
    Private Sub DGVHistori_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DGVHistori.DataBindingComplete
        ' Nonaktifkan sorting otomatis untuk mencegah baris kosong
        For Each column As DataGridViewColumn In DGVHistori.Columns
            column.SortMode = DataGridViewColumnSortMode.NotSortable
        Next

        ' Clear selection
        DGVHistori.ClearSelection()

        ' Hapus baris kosong di akhir
        DGVHistori.AllowUserToAddRows = False
    End Sub

End Class