Imports System.Globalization
Imports System.IO
Imports System.Reflection

Public Class FormBarang
    Private dragging As Boolean
    Private offsetX As Integer
    Private offsetY As Integer

    ' Event untuk menangani ketika mouse ditekan pada Label26
    Private Sub Label26_MouseDown(sender As Object, e As MouseEventArgs) Handles Label26.MouseDown
        If e.Button = MouseButtons.Left Then
            dragging = True
            offsetX = e.X
            offsetY = e.Y
        End If
    End Sub

    ' Event untuk menangani ketika mouse dilepas pada Label26
    Private Sub Label26_MouseUp(sender As Object, e As MouseEventArgs) Handles Label26.MouseUp
        dragging = False
    End Sub

    ' Event untuk menangani ketika mouse digerakkan pada Label26
    Private Sub Label26_MouseMove(sender As Object, e As MouseEventArgs) Handles Label26.MouseMove
        If dragging Then
            ' Memindahkan posisi PanelDetailBarang, bukan Form
            PanelDetailBarang.Location = New Point(PanelDetailBarang.Location.X + e.X - offsetX, PanelDetailBarang.Location.Y + e.Y - offsetY)
        End If
    End Sub

    ' Event untuk menangani ketika mouse ditekan pada PanelDetailBarang
    Private Sub PanelDetailBarang_MouseDown(sender As Object, e As MouseEventArgs) Handles PanelDetailBarang.MouseDown
        If e.Button = MouseButtons.Left Then
            dragging = True
            offsetX = e.X
            offsetY = e.Y
        End If
    End Sub

    ' Event untuk menangani ketika mouse dilepas pada PanelDetailBarang
    Private Sub PanelDetailBarang_MouseUp(sender As Object, e As MouseEventArgs) Handles PanelDetailBarang.MouseUp
        dragging = False
    End Sub

    ' Event untuk menangani ketika mouse digerakkan pada PanelDetailBarang
    Private Sub PanelDetailBarang_MouseMove(sender As Object, e As MouseEventArgs) Handles PanelDetailBarang.MouseMove
        If dragging Then
            ' Memindahkan posisi PanelDetailBarang, bukan Form
            PanelDetailBarang.Location = New Point(PanelDetailBarang.Location.X + e.X - offsetX, PanelDetailBarang.Location.Y + e.Y - offsetY)
        End If
    End Sub



    ' Handler untuk event GotFocus pada TextBox
    Private Sub TextCari_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCari.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCari.BackColor = Color.Yellow ' Ganti warna fokus sesuai kebutuhan
    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TextCari_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtCari.LostFocus
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCari.BackColor = Color.White
    End Sub

    Private Sub Form_Barang_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        PAnelTambahKurang.Visible = False
        PanelDetailBarang.Visible = False

        Dim HABarang As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Barang")
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HABarang(1) ' CanAdd 
        TambahToolStripMenuItem.Visible = HABarang(1)
        BtnUbah.Visible = HABarang(2) ' CanEdit 
        EditToolStripMenuItem.Visible = HABarang(2)
        BtnHapus.Visible = HABarang(3) ' CanDelete 
        HapusStokToolStripMenuItem.Visible = HABarang(3)

        Dim KurangStok As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Kurang Stok")
        ' Terapkan nilai hak akses ke tombol-tombol
        KurangiStokToolStripMenuItem.Visible = KurangStok(2) ' CanEdit 

        Dim TambahStok As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Tambah Stok")
        ' Terapkan nilai hak akses ke tombol-tombol
        TambahStokToolStripMenuItem.Visible = TambahStok(2) ' CanEdit 

        Dim PerbaikiDataBarang As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Perbaiki Data Barang")
        ' Terapkan nilai hak akses ke tombol-tombol
        PerbaikiDatabase.Visible = PerbaikiDataBarang(2) ' CanEdit 

        Dim Perbaikiisisatuan As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Perbaiki isi satuan")
        ' Terapkan nilai hak akses ke tombol-tombol
        PerbaruiStokBarangToolStripMenuItem.Visible = Perbaikiisisatuan(2) ' CanEdit 


        TampilSatuan()

        ' Menampilkan ProgressBar
        ProgressBar1.Visible = False
        LabelProgress.Visible = False

        TxtCari.Clear()
        CariData()

        TxtCari.Select()
        DGBarang.ClearSelection()
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub TextCari_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtCari.KeyDown
        If e.KeyCode = Keys.Enter Then
            CariData()
        End If
    End Sub

    Private Sub CariData()
        Me.Cursor = Cursors.WaitCursor
        DGBarang.Columns.Clear()

        Dim searchText As String = "%" & TxtCari.Text & "%"
        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, NAMA_SUPLIYER, HARGA_BELI, HARGA_BELI_TERAKHIR, " &
                       "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                       "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                       "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, " &
                       "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, " &
                       "STOK_GUDANG, SATUAN_STOK, SATUAN_ISI_STOK " &
                       "FROM tbl_barang " &
                       "WHERE ID_BARANG LIKE @SearchText OR NAMA_BARANG LIKE @SearchText OR NAMA_KATEGORI LIKE @SearchText " &
                       "OR NAMA_SUPLIYER LIKE @SearchText OR BARCODE_KECIL LIKE @SearchText OR BARCODE_SEDANG LIKE @SearchText " &
                       "OR BARCODE_BESAR LIKE @SearchText " &
                       "ORDER BY NAMA_BARANG ASC"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@SearchText", searchText)
            Using ds As New DataSet
                Using da As New MySqlDataAdapter(cmd)
                    da.Fill(ds)
                    DGBarang.DataSource = ds.Tables(0)
                End Using
            End Using
        End Using

        Aturdgvbarangsimpel()
        DGBarang.ClearSelection()
        Me.Cursor = Cursors.Default
    End Sub

    Public Sub Aturdgvbarangsimpel()
        With DGBarang
            .Columns("ID_BARANG").HeaderText = "KODE"
            .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
            .Columns("NAMA_KATEGORI").HeaderText = "KATEGORI"
            .Columns("NAMA_SUPLIYER").HeaderText = "SUPLIYER"
            .Columns("HARGA_BELI").HeaderText = "HPP"
            .Columns("HARGA_BELI_TERAKHIR").HeaderText = "BELI TERAKHIR"
            .Columns("SATUAN_UMUM_KECIL").HeaderText = "SATUAN UMUM KECIL"
            .Columns("SATUAN_UMUM_SEDANG").HeaderText = "SATUAN UMUM SEDANG"
            .Columns("SATUAN_UMUM_BESAR").HeaderText = "SATUAN UMUM BESAR"
            .Columns("HARGA_JUAL_UMUM_KECIL").HeaderText = "HARGA JUAL UMUM KECIL"
            .Columns("HARGA_JUAL_UMUM_SEDANG").HeaderText = "HARGA JUAL UMUM SEDANG"
            .Columns("HARGA_JUAL_UMUM_BESAR").HeaderText = "HARGA JUAL UMUM BESAR"
            .Columns("SATUAN_PARTAI_KECIL").HeaderText = "SATUAN PARTAI KECIL"
            .Columns("SATUAN_PARTAI_SEDANG").HeaderText = "SATUAN PARTAI SEDANG"
            .Columns("SATUAN_PARTAI_BESAR").HeaderText = "SATUAN PARTAI BESAR"
            .Columns("HARGA_JUAL_PARTAI_KECIL").HeaderText = "HARGA JUAL PARTAI KECIL"
            .Columns("HARGA_JUAL_PARTAI_SEDANG").HeaderText = "HARGA JUAL PARTAI SEDANG"
            .Columns("HARGA_JUAL_PARTAI_BESAR").HeaderText = "HARGA JUAL PARTAI BESAR"
            .Columns("STOK_TOKO").HeaderText = "STOK TOKO"
            .Columns("STOK_GUDANG").HeaderText = "STOK GUDANG"
            .Columns("SATUAN_STOK").HeaderText = "SATUAN"
            .Columns("SATUAN_ISI_STOK").HeaderText = "ISI"


            ' Daftar nama kolom yang akan diatur format dan alignment 
            Dim columnsToFormat As String() = {
                "HARGA_BELI", "HARGA_BELI_TERAKHIR",
                "HARGA_JUAL_UMUM_KECIL",
                "HARGA_JUAL_UMUM_SEDANG",
                "HARGA_JUAL_UMUM_BESAR",
                "HARGA_JUAL_PARTAI_KECIL",
                "HARGA_JUAL_PARTAI_SEDANG",
                "HARGA_JUAL_PARTAI_BESAR",
                "STOK_TOKO",
                "STOK_GUDANG"
            }

            ' Loop melalui kolom dan atur format serta alignment
            For Each columnName As String In columnsToFormat
                If .Columns.Contains(columnName) Then
                    ' Gunakan format kustom untuk menampilkan angka di belakang koma jika bukan 0
                    .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                    .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next

            .Columns("ID_BARANG").Frozen = True
            .Columns("NAMA_BARANG").Frozen = True

            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Gray
            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            DataGridViewExtension.EnableDoubleBuffering(DGBarang)

            Dim HargaBeli As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Harga Beli")
            ' Terapkan nilai hak akses ke tombol-tombol
            .Columns("HARGA_BELI").Visible = HargaBeli(0)
        End With
    End Sub

    Public Class DataGridViewExtension
        Public Shared Sub EnableDoubleBuffering(ByVal dataGridView As DataGridView)
            dataGridView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty, Nothing, dataGridView, New Object() {True})
        End Sub
    End Class

    Private Sub DGBarang_CellClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DGBarang.CellClick
        Dim rowIndex As Integer = e.RowIndex

        If rowIndex >= 0 AndAlso rowIndex < DGBarang.Rows.Count Then
            Txtkodebarang.Text = DGBarang.Rows(rowIndex).Cells("ID_BARANG").Value.ToString()
            Txtnamabarang.Text = DGBarang.Rows(rowIndex).Cells("NAMA_BARANG").Value.ToString()

            If PanelDetailBarang.Visible Then
                Tampildetailbarang()
            End If
        End If
    End Sub

    Private Sub DGBarang_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DGBarang.CellMouseUp
        If e.Button = MouseButtons.Right Then
            If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then

                If PanelDetailBarang.Visible = True Then
                    PanelDetailBarang.Visible = False
                End If

                DGBarang.ClearSelection()
                DGBarang.Rows(e.RowIndex).Selected = True
                DGBarang.CurrentCell = DGBarang.Rows(e.RowIndex).Cells(e.ColumnIndex)
                Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
                ContextMenuStrip1.Show(DGBarang, DGBarang.PointToClient(cursorPosition))
            End If
        End If
    End Sub

    Private Sub Tampildetailbarang()
        PanelDetailBarang.Location = New System.Drawing.Point((ClientSize.Width - PanelDetailBarang.Width) \ 2, (ClientSize.Height - PanelDetailBarang.Height) \ 2)

        Label26.Text = "DETAIL BARANG"
        Dim i As Integer
        i = DGBarang.CurrentRow.Index
        ' Ambil nilai ID_BARANG dari sel di baris saat ini
        Dim idBarang As String = DGBarang.Item("ID_BARANG", i).Value.ToString()

        Dim query As String = "SELECT " &
     "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
     "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
     "ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, " &
     "AWAL_TOKO, TAMBAH_TOKO, KURANG_TOKO, PEMBELIAN_TOKO, PENJUALAN_TOKO, " &
     "RETUR_BELI_TOKO, RETUR_JUAL_TOKO, OPNAME_TOKO, TRANSFER_STOK_MASUK_TOKO, TRANSFER_STOK_KELUAR_TOKO, " &
     "TRANSFER_BARANG_MASUK_TOKO, TRANSFER_BARANG_KELUAR_TOKO, " &
     "AWAL_GUDANG, TAMBAH_GUDANG, KURANG_GUDANG, PEMBELIAN_GUDANG, PENJUALAN_GUDANG, " &
     "RETUR_BELI_GUDANG, RETUR_JUAL_GUDANG, OPNAME_GUDANG, TRANSFER_STOK_MASUK_GUDANG, TRANSFER_STOK_KELUAR_GUDANG, " &
     "TRANSFER_BARANG_MASUK_GUDANG, TRANSFER_BARANG_KELUAR_GUDANG " &
     "FROM tbl_barang WHERE ID_BARANG = @ID_BARANG"


        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@ID_BARANG", idBarang)
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.Read() Then
                    Label18.Text = idBarang
                    Label17.Text = DGBarang.Item("NAMA_BARANG", i).Value
                    Label11.Text = If(DGBarang.Item("NAMA_KATEGORI", i).Value Is Nothing, "", DGBarang.Item("NAMA_KATEGORI", i).Value.ToString())
                    Label1.Text = If(DGBarang.Item("NAMA_SUPLIYER", i).Value IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(DGBarang.Item("NAMA_SUPLIYER", i).Value.ToString()), DGBarang.Item("NAMA_SUPLIYER", i).Value.ToString(), "")
                    Label32.Text = "Rp. " & If(IsDBNull(DGBarang.Item("HARGA_BELI", i).Value), 0D, Convert.ToDecimal(DGBarang.Item("HARGA_BELI", i).Value)).ToString("#,0.##")
                    Label96.Text = "Rp. " & If(IsDBNull(DGBarang.Item("HARGA_BELI_TERAKHIR", i).Value), 0D, Convert.ToDecimal(DGBarang.Item("HARGA_BELI_TERAKHIR", i).Value)).ToString("#,0.##")
                    Label37.Text = reader("BARCODE_KECIL").ToString()
                    Label36.Text = reader("BARCODE_SEDANG").ToString()
                    Label34.Text = reader("BARCODE_BESAR").ToString()
                    Label40.Text = DGBarang.Item("SATUAN_UMUM_KECIL", i).Value & " (" & reader("ISI_UMUM_KECIL").ToString() & ")"
                    Label39.Text = DGBarang.Item("SATUAN_UMUM_SEDANG", i).Value & " (" & reader("ISI_UMUM_SEDANG").ToString() & ")"
                    Label38.Text = DGBarang.Item("SATUAN_UMUM_BESAR", i).Value & " (" & reader("ISI_UMUM_BESAR").ToString() & ")"
                    Label51.Text = If(IsDBNull(DGBarang.Item("HARGA_JUAL_UMUM_KECIL", i).Value), 0D, Convert.ToDecimal(DGBarang.Item("HARGA_JUAL_UMUM_KECIL", i).Value)).ToString("#,0.##")
                    Label50.Text = If(IsDBNull(DGBarang.Item("HARGA_JUAL_UMUM_SEDANG", i).Value), 0D, Convert.ToDecimal(DGBarang.Item("HARGA_JUAL_UMUM_SEDANG", i).Value)).ToString("#,0.##")
                    Label49.Text = If(IsDBNull(DGBarang.Item("HARGA_JUAL_UMUM_BESAR", i).Value), 0D, Convert.ToDecimal(DGBarang.Item("HARGA_JUAL_UMUM_BESAR", i).Value)).ToString("#,0.##")
                    Label67.Text = DGBarang.Item("SATUAN_PARTAI_KECIL", i).Value & " (" & reader("ISI_PARTAI_KECIL").ToString() & ")"
                    Label66.Text = DGBarang.Item("SATUAN_PARTAI_SEDANG", i).Value & " (" & reader("ISI_PARTAI_SEDANG").ToString() & ")"
                    Label65.Text = DGBarang.Item("SATUAN_PARTAI_BESAR", i).Value & " (" & reader("ISI_PARTAI_BESAR").ToString() & ")"
                    Label55.Text = If(IsDBNull(DGBarang.Item("HARGA_JUAL_PARTAI_KECIL", i).Value), 0D, Convert.ToDecimal(DGBarang.Item("HARGA_JUAL_PARTAI_KECIL", i).Value)).ToString("#,0.##")
                    Label54.Text = If(IsDBNull(DGBarang.Item("HARGA_JUAL_PARTAI_SEDANG", i).Value), 0D, Convert.ToDecimal(DGBarang.Item("HARGA_JUAL_PARTAI_SEDANG", i).Value)).ToString("#,0.##")
                    Label53.Text = If(IsDBNull(DGBarang.Item("HARGA_JUAL_PARTAI_BESAR", i).Value), 0D, Convert.ToDecimal(DGBarang.Item("HARGA_JUAL_PARTAI_BESAR", i).Value)).ToString("#,0.##")
                    Label83.Text = If(reader("AWAL_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("AWAL_TOKO").ToString()), Convert.ToDecimal(reader("AWAL_TOKO")).ToString("#,0.##"), "0")
                    Label25.Text = If(reader("TAMBAH_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TAMBAH_TOKO").ToString()), Convert.ToDecimal(reader("TAMBAH_TOKO")).ToString("#,0.##"), "0")
                    Label42.Text = If(reader("KURANG_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("KURANG_TOKO").ToString()), Convert.ToDecimal(reader("KURANG_TOKO")).ToString("#,0.##"), "0")
                    Label61.Text = If(reader("PEMBELIAN_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("PEMBELIAN_TOKO").ToString()), Convert.ToDecimal(reader("PEMBELIAN_TOKO")).ToString("#,0.##"), "0")
                    Label64.Text = If(reader("PENJUALAN_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("PENJUALAN_TOKO").ToString()), Convert.ToDecimal(reader("PENJUALAN_TOKO")).ToString("#,0.##"), "0")
                    Label71.Text = If(reader("RETUR_BELI_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("RETUR_BELI_TOKO").ToString()), Convert.ToDecimal(reader("RETUR_BELI_TOKO")).ToString("#,0.##"), "0")
                    Label72.Text = If(reader("RETUR_JUAL_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("RETUR_JUAL_TOKO").ToString()), Convert.ToDecimal(reader("RETUR_JUAL_TOKO")).ToString("#,0.##"), "0")
                    Label73.Text = If(reader("OPNAME_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("OPNAME_TOKO").ToString()), Convert.ToDecimal(reader("OPNAME_TOKO")).ToString("#,0.##"), "0")
                    Label74.Text = If(reader("TRANSFER_STOK_MASUK_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TRANSFER_STOK_MASUK_TOKO").ToString()), Convert.ToDecimal(reader("TRANSFER_STOK_MASUK_TOKO")).ToString("#,0.##"), "0")
                    Label45.Text = If(reader("TRANSFER_STOK_KELUAR_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TRANSFER_STOK_KELUAR_TOKO").ToString()), Convert.ToDecimal(reader("TRANSFER_STOK_KELUAR_TOKO")).ToString("#,0.##"), "0")
                    Label93.Text = If(reader("TRANSFER_BARANG_MASUK_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TRANSFER_BARANG_MASUK_TOKO").ToString()), Convert.ToDecimal(reader("TRANSFER_BARANG_MASUK_TOKO")).ToString("#,0.##"), "0")
                    Label92.Text = If(reader("TRANSFER_BARANG_KELUAR_TOKO") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TRANSFER_BARANG_KELUAR_TOKO").ToString()), Convert.ToDecimal(reader("TRANSFER_BARANG_KELUAR_TOKO")).ToString("#,0.##"), "0")
                    Label89.Text = If(DGBarang.Item("STOK_TOKO", i).Value IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(DGBarang.Item("STOK_TOKO", i).Value.ToString()), Convert.ToDecimal(DGBarang.Item("STOK_TOKO", i).Value).ToString("#,0.##"), "0")

                    Label79.Text = If(reader("AWAL_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("AWAL_GUDANG").ToString()), Convert.ToDecimal(reader("AWAL_GUDANG")).ToString("#,0.##"), "0")
                    Label31.Text = If(reader("TAMBAH_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TAMBAH_GUDANG").ToString()), Convert.ToDecimal(reader("TAMBAH_GUDANG")).ToString("#,0.##"), "0")
                    Label43.Text = If(reader("KURANG_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("KURANG_GUDANG").ToString()), Convert.ToDecimal(reader("KURANG_GUDANG")).ToString("#,0.##"), "0")
                    Label62.Text = If(reader("PEMBELIAN_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("PEMBELIAN_GUDANG").ToString()), Convert.ToDecimal(reader("PEMBELIAN_GUDANG")).ToString("#,0.##"), "0")
                    Label69.Text = If(reader("PENJUALAN_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("PENJUALAN_GUDANG").ToString()), Convert.ToDecimal(reader("PENJUALAN_GUDANG")).ToString("#,0.##"), "0")
                    Label75.Text = If(reader("RETUR_BELI_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("RETUR_BELI_GUDANG").ToString()), Convert.ToDecimal(reader("RETUR_BELI_GUDANG")).ToString("#,0.##"), "0")
                    Label76.Text = If(reader("RETUR_JUAL_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("RETUR_JUAL_GUDANG").ToString()), Convert.ToDecimal(reader("RETUR_JUAL_GUDANG")).ToString("#,0.##"), "0")
                    Label81.Text = If(reader("OPNAME_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("OPNAME_GUDANG").ToString()), Convert.ToDecimal(reader("OPNAME_GUDANG")).ToString("#,0.##"), "0")
                    Label86.Text = If(reader("TRANSFER_STOK_MASUK_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TRANSFER_STOK_MASUK_GUDANG").ToString()), Convert.ToDecimal(reader("TRANSFER_STOK_MASUK_GUDANG")).ToString("#,0.##"), "0")
                    Label46.Text = If(reader("TRANSFER_STOK_KELUAR_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TRANSFER_STOK_KELUAR_GUDANG").ToString()), Convert.ToDecimal(reader("TRANSFER_STOK_KELUAR_GUDANG")).ToString("#,0.##"), "0")
                    Label82.Text = If(reader("TRANSFER_BARANG_MASUK_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TRANSFER_BARANG_MASUK_GUDANG").ToString()), Convert.ToDecimal(reader("TRANSFER_BARANG_MASUK_GUDANG")).ToString("#,0.##"), "0")
                    Label78.Text = If(reader("TRANSFER_BARANG_KELUAR_GUDANG") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("TRANSFER_BARANG_KELUAR_GUDANG").ToString()), Convert.ToDecimal(reader("TRANSFER_BARANG_KELUAR_GUDANG")).ToString("#,0.##"), "0")
                    Label90.Text = If(DGBarang.Item("STOK_GUDANG", i).Value IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(DGBarang.Item("STOK_GUDANG", i).Value.ToString()), Convert.ToDecimal(DGBarang.Item("STOK_GUDANG", i).Value).ToString("#,0.##"), "0")

                End If
            End Using
        End Using

        Dim HargaBeli As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Harga Beli")
        ' Terapkan nilai hak akses ke tombol-tombol
        Label32.Visible = HargaBeli(0) ' CanEdit 

        PanelDetailBarang.Visible = True
    End Sub

    Private Sub BtnSembunyi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSembunyi.Click
        PanelDetailBarang.Visible = False
    End Sub



    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click, TambahToolStripMenuItem.Click
        If PanelDetailBarang.Visible = True Then
            PanelDetailBarang.Visible = False
        End If

        DGBarang.Enabled = False
        PanelOperasi.Enabled = False

        Tambahbrg()
        TxtCari.Select()
        DGBarang.ClearSelection()
    End Sub

    Private Sub Tambahbrg()
        Using f As New TambahBarang()
            f.LblUtama.Text = "T A M B A H   B A R A N G"
            f.ShowDialog()
        End Using

        DGBarang.Enabled = True
        PanelOperasi.Enabled = True
        CariData()
    End Sub


    Private Sub BtnUbah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnUbah.Click, EditToolStripMenuItem.Click
        If PanelDetailBarang.Visible = True Then
            PanelDetailBarang.Visible = False
        End If

        DGBarang.Enabled = False
        PanelOperasi.Enabled = False

        TambahBarang.Tampilkategori()
        TambahBarang.TampilSatuan()
        TambahBarang.Tampilsupliyer()
        UbahBarang()
        TxtCari.Select()
        DGBarang.ClearSelection()
    End Sub

    Private Sub UbahBarang()
        TambahBarang.LblUtama.Text = "E D I T   B A R A N G"
        Dim i As Integer = DGBarang.CurrentRow.Index

        Dim idBarang As String = String.Empty
        Dim barcodeKecil As String = String.Empty
        Dim barcodeSedang As String = String.Empty
        Dim barcodeBesar As String = String.Empty
        Dim satuanStok As String = String.Empty
        Dim satuanIsiStok As String = String.Empty
        Dim stokAwalToko As Decimal = 0D
        Dim stokAwalGudang As Decimal = 0D
        Dim jumlahTransaksiToko As Decimal = 0D
        Dim jumlahTransaksiGudang As Decimal = 0D
        Dim lokasiRakToko As String = String.Empty
        Dim lokasiRakGudang As String = String.Empty
        Dim pointMember As Decimal = 0D
        Dim pointKaryawan As Decimal = 0D
        Dim komisiSalesRp As Decimal = 0D
        Dim komisiSalesPersen As Decimal = 0D
        Dim stokMin As Decimal = 0D
        Dim stokMax As Decimal = 0D


        ' Ambil nilai ID_BARANG dari sel di baris saat ini
        idBarang = DGBarang.Item("ID_BARANG", i).Value.ToString()

        Dim query As String = "SELECT " &
            "BARCODE_KECIL, BARCODE_SEDANG, BARCODE_BESAR, " &
            "ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
            "ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, " &
            "AWAL_TOKO, " &
            "(TAMBAH_TOKO - KURANG_TOKO + PEMBELIAN_TOKO - PENJUALAN_TOKO - RETUR_BELI_TOKO + RETUR_JUAL_TOKO + OPNAME_TOKO + TRANSFER_STOK_MASUK_TOKO - TRANSFER_STOK_KELUAR_TOKO + TRANSFER_BARANG_MASUK_TOKO - TRANSFER_BARANG_KELUAR_TOKO) AS JUMLAHTRANSAKSITOKO, " &
            "AWAL_GUDANG, " &
            "(TAMBAH_GUDANG - KURANG_GUDANG + PEMBELIAN_GUDANG - PENJUALAN_GUDANG - RETUR_BELI_GUDANG + RETUR_JUAL_GUDANG + OPNAME_GUDANG + TRANSFER_STOK_MASUK_GUDANG - TRANSFER_STOK_KELUAR_GUDANG + TRANSFER_BARANG_MASUK_GUDANG - TRANSFER_BARANG_KELUAR_GUDANG) AS JUMLAHTRANSAKSIGUDANG, " &
            "SATUAN_STOK, SATUAN_ISI_STOK, SATUAN_STOK, SATUAN_ISI_STOK, " &
            "STOK_MIN, STOK_MAX, " &
            "LOKASI_RAK_TOKO, LOKASI_RAK_GUDANG, POINT_MEMBER, POINT_KARYAWAN, KOMISI_SALES_RP, KOMISI_SALES_PERSEN " &
            "FROM tbl_barang WHERE ID_BARANG = @ID_BARANG"

        Using command As New MySqlCommand(query, conn)
            command.Parameters.AddWithValue("@ID_BARANG", idBarang)
            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.Read() Then
                    barcodeKecil = reader("BARCODE_KECIL").ToString()
                    barcodeSedang = reader("BARCODE_SEDANG").ToString()
                    barcodeBesar = reader("BARCODE_BESAR").ToString()

                    satuanStok = reader("SATUAN_STOK").ToString()
                    satuanIsiStok = reader("SATUAN_ISI_STOK").ToString()

                    stokAwalToko = If(reader("AWAL_TOKO") IsNot DBNull.Value, Convert.ToDecimal(reader("AWAL_TOKO")), 0)
                    stokAwalGudang = If(reader("AWAL_GUDANG") IsNot DBNull.Value, Convert.ToDecimal(reader("AWAL_GUDANG")), 0)

                    jumlahTransaksiToko = If(reader("JUMLAHTRANSAKSITOKO") IsNot DBNull.Value, Convert.ToDecimal(reader("JUMLAHTRANSAKSITOKO")), 0)
                    jumlahTransaksiGudang = If(reader("JUMLAHTRANSAKSIGUDANG") IsNot DBNull.Value, Convert.ToDecimal(reader("JUMLAHTRANSAKSIGUDANG")), 0)

                    lokasiRakToko = reader("LOKASI_RAK_TOKO").ToString()
                    lokasiRakGudang = reader("LOKASI_RAK_GUDANG").ToString()

                    pointMember = If(reader("POINT_MEMBER") IsNot DBNull.Value, Convert.ToDecimal(reader("POINT_MEMBER")), 0)
                    pointKaryawan = If(reader("POINT_KARYAWAN") IsNot DBNull.Value, Convert.ToDecimal(reader("POINT_KARYAWAN")), 0)
                    komisiSalesRp = If(reader("KOMISI_SALES_RP") IsNot DBNull.Value, Convert.ToDecimal(reader("KOMISI_SALES_RP")), 0)
                    komisiSalesPersen = If(reader("KOMISI_SALES_PERSEN") IsNot DBNull.Value, Convert.ToDecimal(reader("KOMISI_SALES_PERSEN")), 0)

                    stokMin = If(reader("STOK_MIN") IsNot DBNull.Value, Convert.ToDecimal(reader("STOK_MIN")), 0)
                    stokMax = If(reader("STOK_MAX") IsNot DBNull.Value, Convert.ToDecimal(reader("STOK_MAX")), 0)
                End If
            End Using
        End Using


        With TambahBarang
            .TxtKode.Text = idBarang
            .TxtNama.Text = DGBarang.Item("NAMA_BARANG", i).Value.ToString()
            .CmbKategori.Text = DGBarang.Item("NAMA_KATEGORI", i).Value.ToString()
            .CmbSupliyer.Text = DGBarang.Item("NAMA_SUPLIYER", i).Value.ToString()

            .TxtBarcodeUmumKecil.Text = barcodeKecil
            .TxtBarcodeUmumSedang.Text = barcodeSedang
            .TxtBarcodeUmumBesar.Text = barcodeBesar

            .CmbSatUmumKecil.Text = DGBarang.Item("SATUAN_UMUM_KECIL", i).Value.ToString()
            .CmbSatUmumSedang.Text = DGBarang.Item("SATUAN_UMUM_SEDANG", i).Value.ToString()
            .CmbSatUmumBesar.Text = DGBarang.Item("SATUAN_UMUM_BESAR", i).Value.ToString()

            .TxtHArgaJUalUmumKecil.Text = Decimal.Parse(DGBarang.Item("HARGA_JUAL_UMUM_KECIL", i).Value.ToString()).ToString("0.##")
            .TxtHArgaJUalUmumSedang.Text = Decimal.Parse(DGBarang.Item("HARGA_JUAL_UMUM_SEDANG", i).Value.ToString()).ToString("0.##")
            .TxtHArgaJUalUmumBesar.Text = Decimal.Parse(DGBarang.Item("HARGA_JUAL_UMUM_BESAR", i).Value.ToString()).ToString("0.##")

            .CmbSatPartaiKecil.Text = DGBarang.Item("SATUAN_PARTAI_KECIL", i).Value.ToString()
            .CmbSatPartaiSedang.Text = DGBarang.Item("SATUAN_PARTAI_SEDANG", i).Value.ToString()
            .CmbSatPartaiBesar.Text = DGBarang.Item("SATUAN_PARTAI_BESAR", i).Value.ToString()

            .TxtHArgaJualPartaikecil.Text = Decimal.Parse(DGBarang.Item("HARGA_JUAL_PARTAI_KECIL", i).Value.ToString()).ToString("0.##")
            .TxtHArgaJualPartaiSedang.Text = Decimal.Parse(DGBarang.Item("HARGA_JUAL_PARTAI_SEDANG", i).Value.ToString()).ToString("0.##")
            .TxtHArgaJualPartaiBesar.Text = Decimal.Parse(DGBarang.Item("HARGA_JUAL_PARTAI_BESAR", i).Value.ToString()).ToString("0.##")

            If FormUtama.SLokasi.Text = "GUDANG" Then
                .TxtStokAwal.Text = stokAwalGudang.ToString("0.##")
            ElseIf FormUtama.SLokasi.Text = "TOKO" Then
                .TxtStokAwal.Text = stokAwalToko.ToString("0.##")
            End If

            .TxtJmlhToko.Text = jumlahTransaksiToko.ToString("0.##")
            .TxtJmlhGudang.Text = jumlahTransaksiGudang.ToString("0.##")

            If FormUtama.SLokasi.Text = "GUDANG" Then
                .TxtStokAkhir.Text = If(DGBarang.Item("STOK_GUDANG", i).Value IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(DGBarang.Item("STOK_GUDANG", i).Value.ToString()), Convert.ToDecimal(DGBarang.Item("STOK_GUDANG", i).Value).ToString("#,0.##"), "0")
                .LblStokUntukEdit.Text = If(DGBarang.Item("STOK_GUDANG", i).Value IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(DGBarang.Item("STOK_GUDANG", i).Value.ToString()), Convert.ToDecimal(DGBarang.Item("STOK_GUDANG", i).Value).ToString("#,0.##"), "0")
            ElseIf FormUtama.SLokasi.Text = "TOKO" Then
                .TxtStokAkhir.Text = If(DGBarang.Item("STOK_TOKO", i).Value IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(DGBarang.Item("STOK_TOKO", i).Value.ToString()), Convert.ToDecimal(DGBarang.Item("STOK_TOKO", i).Value).ToString("#,0.##"), "0")
                .LblStokUntukEdit.Text = If(DGBarang.Item("STOK_TOKO", i).Value IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(DGBarang.Item("STOK_TOKO", i).Value.ToString()), Convert.ToDecimal(DGBarang.Item("STOK_TOKO", i).Value).ToString("#,0.##"), "0")
            End If


            .TxtLokasiRak.Text = If(FormUtama.SLokasi.Text = "GUDANG", lokasiRakGudang, lokasiRakToko)
            .CmBstokAwal.Text = satuanStok
            .TxtIsiStokAwal.Text = satuanIsiStok

            .TextMin.Text = stokMin.ToString("0.##")
            .TxtStokMAx.Text = stokMax.ToString("0.##")

            .TxtPointMember.Text = pointMember.ToString("0.##")
            .TxtPointKaryawan.Text = pointKaryawan.ToString("0.##")
            .TxtKomisiSalesRp.Text = komisiSalesRp.ToString("0.##")
            .TxtKomisiSalesPersen.Text = komisiSalesPersen.ToString("0.##")

            .TxtHrgBeli.Text = Decimal.Parse(DGBarang.Item("HARGA_BELI", i).Value.ToString()).ToString("0.##")
            .LblHargaUntukEdit.Text = Decimal.Parse(DGBarang.Item("HARGA_BELI", i).Value.ToString()).ToString("0.##")
            .TxtHargaBeliTerakhir.Text = Decimal.Parse(DGBarang.Item("HARGA_BELI_TERAKHIR", i).Value.ToString()).ToString("0.##")
        End With

        ' Tampilkan form dialog
        TambahBarang.ShowDialog()

        ' Aktifkan kembali kontrol lainnya
        DGBarang.Enabled = True
        PanelOperasi.Enabled = True
        CariData()
    End Sub


    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnHapus.Click, HapusStokToolStripMenuItem.Click
        If PanelDetailBarang.Visible = True Then
            PanelDetailBarang.Visible = False
        End If
        Hapusbarang()
        TxtCari.Select()
        DGBarang.ClearSelection()
    End Sub


    Private Sub Hapusbarang()
        If Txtkodebarang.Text = "" Then
            MessageBox.Show("Mohon pilih data yang ingin dihapus terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If MessageBox.Show("Apakah barang ini akan dihapus ...???" & Chr(13) & "Kode : " & Txtkodebarang.Text & Chr(13) & "Nama : " & Txtnamabarang.Text, "Peringatan...!!!", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            ' Mulai transaksi
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    Dim noTransaksi As String = DateTime.Now.ToString("yyyyMMddHHmmss") ' Format unik berdasarkan tanggal dan waktu saat ini

                    ' Hitung dan tambahkan nominal sesuai dengan perhitungan Anda
                    Dim stokToko As Decimal = If(IsDBNull(DGBarang.Item("STOK_TOKO", DGBarang.CurrentRow.Index).Value), 0D, Convert.ToDecimal(DGBarang.Item("STOK_TOKO", DGBarang.CurrentRow.Index).Value))
                    Dim stokGudang As Decimal = If(IsDBNull(DGBarang.Item("STOK_GUDANG", DGBarang.CurrentRow.Index).Value), 0D, Convert.ToDecimal(DGBarang.Item("STOK_GUDANG", DGBarang.CurrentRow.Index).Value))
                    Dim hargaBeli As Decimal = If(IsDBNull(DGBarang.Item("HARGA_BELI", DGBarang.CurrentRow.Index).Value), 0D, Convert.ToDecimal(DGBarang.Item("HARGA_BELI", DGBarang.CurrentRow.Index).Value))

                    ' Menghitung nominal
                    Dim nominal As Decimal = (stokToko + stokGudang) * hargaBeli

                    If nominal <> 0 Then


                        ' Menyimpan transaksi ke JurnalUmum
                        Using cmdInsert As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                                              "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                            cmdInsert.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
                            cmdInsert.Parameters.AddWithValue("@TGL_TRANSAKSI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                            cmdInsert.Parameters.AddWithValue("@URAIAN", "Hapus barang " & Txtnamabarang.Text)
                            cmdInsert.Parameters.AddWithValue("@NAMA_AKUN_D", LAWAN_NAMA_REK_BARANG)
                            cmdInsert.Parameters.AddWithValue("@NOMOR_AKUN_D", LAWAN_KODE_REK_BARANG)
                            cmdInsert.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
                            cmdInsert.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
                            cmdInsert.Parameters.AddWithValue("@NOMINAL", nominal)
                            cmdInsert.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Hapus Barang")
                            cmdInsert.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                            cmdInsert.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                            cmdInsert.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

                            cmdInsert.ExecuteNonQuery()
                        End Using
                    End If

                    ' Hapus barang dari tabel
                    Using cmdDelete As New MySqlCommand("DELETE FROM tbl_barang WHERE ID_BARANG = @KodeBarang", conn, transaction)
                        cmdDelete.Parameters.AddWithValue("@KodeBarang", Txtkodebarang.Text)
                        cmdDelete.ExecuteNonQuery()
                    End Using

                    ' Commit transaksi jika semua operasi berhasil
                    transaction.Commit()

                    DatabaseModule.CatatanAksiHistory("Hapus barang " & Txtkodebarang.Text)
                    CariData()
                Catch ex As Exception
                    ' Rollback transaksi jika ada kesalahan
                    transaction.Rollback()
                    MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End If
    End Sub


    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluar.Click
        Close()
    End Sub


    Private Sub DGBarang_CellFormatting(ByVal sender As Object, ByVal e As DataGridViewCellFormattingEventArgs) Handles DGBarang.CellFormatting
        If DGBarang.Columns(e.ColumnIndex).Name = "STOK_TOKO" Then
            Dim value As Object = If(Convert.IsDBNull(e.Value), 0, e.Value)
            If (Convert.ToInt64(value) <= 1) Then
                e.CellStyle.BackColor = Color.IndianRed
                e.CellStyle.ForeColor = Color.LavenderBlush
            ElseIf (Convert.ToInt64(value) <= 10) Then
                e.CellStyle.BackColor = Color.LightSalmon
                e.CellStyle.ForeColor = Color.DarkRed
            Else
                e.CellStyle.BackColor = Color.PaleGreen
                e.CellStyle.ForeColor = Color.DarkGreen
            End If
        End If

        If DGBarang.Columns(e.ColumnIndex).Name = "STOK_GUDANG" Then
            Dim value As Object = If(Convert.IsDBNull(e.Value), 0, e.Value)
            If (Convert.ToInt64(value) <= 1) Then
                e.CellStyle.BackColor = Color.IndianRed
                e.CellStyle.ForeColor = Color.LavenderBlush
            ElseIf (Convert.ToInt64(value) <= 10) Then
                e.CellStyle.BackColor = Color.LightSalmon
                e.CellStyle.ForeColor = Color.DarkRed
            Else
                e.CellStyle.BackColor = Color.PaleGreen
                e.CellStyle.ForeColor = Color.DarkGreen
            End If
        End If

    End Sub


    Private Sub RefreshToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RefreshToolStripMenuItem.Click
        TxtCari.Clear()
        CariData()
        TxtCari.Select()
        DGBarang.ClearSelection()
    End Sub

    Private Sub HandleDataSorting(ByVal orderByClause As String)
        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, NAMA_SUPLIYER, HARGA_BELI, HARGA_BELI_TERAKHIR, " &
                       "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                       "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                       "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, " &
                       "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, " &
                       "STOK_GUDANG, SATUAN_STOK, SATUAN_ISI_STOK " &
                       "FROM tbl_barang ORDER BY " & orderByClause
        DGBarang.Columns.Clear()

        Using da As New MySqlDataAdapter(query, conn)
            ds = New DataSet
            da.Fill(ds)
            DGBarang.DataSource = ds.Tables(0)
            Aturdgvbarangsimpel()
        End Using
    End Sub

    Private Sub ByKodeToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ByKodeToolStripMenuItem1.Click
        HandleDataSorting("ID_BARANG ASC")
    End Sub

    Private Sub ByNamaToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ByNamaToolStripMenuItem1.Click
        HandleDataSorting("NAMA_BARANG ASC")
    End Sub

    Private Sub ByHargaBeliToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ByHargaBeliToolStripMenuItem1.Click
        HandleDataSorting("HARGA_BELI ASC")
    End Sub

    Private Sub ByStokTokoToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ByStokTokoToolStripMenuItem1.Click
        HandleDataSorting("STOK_TOKO ASC")
    End Sub

    Private Sub ByStokGudangToolStripMenuItem1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ByStokGudangToolStripMenuItem1.Click
        HandleDataSorting("STOK_GUDANG ASC")
    End Sub

    Private Sub PerbaruiStokBarangToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PerbaruiStokBarangToolStripMenuItem.Click
        Cursor = Cursors.WaitCursor
        PerbaruiStokBarangToolStripMenuItem.Enabled = False

        Dim transaction As MySqlTransaction = Nothing
        Try
            ' Mulai transaksi
            transaction = conn.BeginTransaction()

            ' Array query untuk tbl_barang berdasarkan tbl_satuan
            Dim updateQueries As String() = {
                "UPDATE tbl_barang SET ISI_UMUM_KECIL = (SELECT isi FROM tbl_satuan WHERE tbl_barang.SATUAN_UMUM_KECIL = tbl_satuan.nama);",
                "UPDATE tbl_barang SET ISI_UMUM_SEDANG = (SELECT isi FROM tbl_satuan WHERE tbl_barang.SATUAN_UMUM_SEDANG = tbl_satuan.nama);",
                "UPDATE tbl_barang SET ISI_UMUM_BESAR = (SELECT isi FROM tbl_satuan WHERE tbl_barang.SATUAN_UMUM_BESAR = tbl_satuan.nama);",
                "UPDATE tbl_barang SET ISI_PARTAI_KECIL = (SELECT isi FROM tbl_satuan WHERE tbl_barang.SATUAN_PARTAI_KECIL = tbl_satuan.nama);",
                "UPDATE tbl_barang SET ISI_PARTAI_SEDANG = (SELECT isi FROM tbl_satuan WHERE tbl_barang.SATUAN_PARTAI_SEDANG = tbl_satuan.nama);",
                "UPDATE tbl_barang SET ISI_PARTAI_BESAR = (SELECT isi FROM tbl_satuan WHERE tbl_barang.SATUAN_PARTAI_BESAR = tbl_satuan.nama);",
                "UPDATE tbl_barang SET SATUAN_ISI_STOK = (SELECT isi FROM tbl_satuan WHERE tbl_barang.SATUAN_STOK = tbl_satuan.nama);"
            }

            ' Jalankan setiap query untuk tabel tbl_barang
            For Each query As String In updateQueries
                Using cmdUpdate As New MySqlCommand(query, conn, transaction)
                    cmdUpdate.ExecuteNonQuery()
                End Using
            Next

            ' Query untuk update KODE_SUPLIYER berdasarkan tbl_supliyer
            Dim supplierQuery As String = "UPDATE tbl_barang SET KODE_SUPLIYER = (SELECT Kode FROM tbl_supliyer WHERE tbl_barang.NAMA_SUPLIYER = tbl_supliyer.Nama);"

            ' Query untuk update KODE_KATEGORI berdasarkan tbl_kategori
            Dim categoryQuery As String = "UPDATE tbl_barang SET KODE_KATEGORI = (SELECT KODE FROM tbl_kategori WHERE tbl_barang.NAMA_KATEGORI = tbl_kategori.NAMA);"

            ' Update supplier
            Using cmdUpdateSupplier As New MySqlCommand(supplierQuery, conn, transaction)
                cmdUpdateSupplier.ExecuteNonQuery()
            End Using

            ' Update category
            Using cmdUpdateCategory As New MySqlCommand(categoryQuery, conn, transaction)
                cmdUpdateCategory.ExecuteNonQuery()
            End Using

            ' Commit transaksi jika semua query berhasil
            transaction.Commit()

            MessageBox.Show("Isi satuan barang berhasil diperbarui!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            Cursor = Cursors.Default
            PerbaruiStokBarangToolStripMenuItem.Enabled = True
        End Try

        ' Muat ulang data setelah update
        CariData()
    End Sub

    Public Sub TampilSatuan()
        Using cmd As New MySqlCommand("SELECT nama FROM tbl_satuan order by nama ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                CmbIsiTokoT.Items.Clear()
                CmbIsiGUdangT.Items.Clear()
                Do While rd.Read()
                    CmbIsiTokoT.Items.Add(rd.Item("nama"))
                    CmbIsiGUdangT.Items.Add(rd.Item("nama"))
                Loop
            End Using
        End Using
    End Sub


    Private Sub TambahKurangstok()
        Dim query As String = "SELECT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = ?"
        Dim idBarang As String = DGBarang("ID_BARANG", DGBarang.CurrentRow.Index).Value

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@idBarang", idBarang)

            CmbIsiTokoT.Items.Clear()
            CmbIsiGUdangT.Items.Clear()

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim satuanKecil As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_KECIL")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_KECIL")), "")
                    Dim satuanSedang As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
                    Dim satuanBesar As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_BESAR")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_BESAR")), "")

                    If Not String.IsNullOrEmpty(satuanKecil) Or Not String.IsNullOrEmpty(satuanSedang) Or Not String.IsNullOrEmpty(satuanBesar) Then
                        CmbIsiTokoT.Items.Clear()
                        CmbIsiGUdangT.Items.Clear()

                        If Not String.IsNullOrEmpty(satuanKecil) Then
                            CmbIsiTokoT.Items.Add(satuanKecil)
                            CmbIsiGUdangT.Items.Add(satuanKecil)
                        End If

                        If Not String.IsNullOrEmpty(satuanSedang) Then
                            CmbIsiTokoT.Items.Add(satuanSedang)
                            CmbIsiGUdangT.Items.Add(satuanSedang)
                        End If

                        If Not String.IsNullOrEmpty(satuanBesar) Then
                            CmbIsiTokoT.Items.Add(satuanBesar)
                            CmbIsiGUdangT.Items.Add(satuanBesar)
                        End If
                    End If
                End If
            End Using
        End Using

        Dim i As Integer = DGBarang.CurrentRow.Index

        TxtKode.Text = idBarang
        TxtNama.Text = If(DGBarang.Item("NAMA_BARANG", i).Value IsNot DBNull.Value, DGBarang.Item("NAMA_BARANG", i).Value.ToString(), "")
        TxtNamaKategori.Text = If(DGBarang.Item("NAMA_KATEGORI", i).Value IsNot DBNull.Value, DGBarang.Item("NAMA_KATEGORI", i).Value.ToString(), "")
        TxtNamaSupliyer.Text = If(DGBarang.Item("NAMA_SUPLIYER", i).Value IsNot DBNull.Value, DGBarang.Item("NAMA_SUPLIYER", i).Value.ToString(), "")
        TxtHargaBeli.Text = If(DGBarang.Item("HARGA_BELI", i).Value IsNot DBNull.Value, Decimal.Parse(DGBarang.Item("HARGA_BELI", i).Value.ToString()).ToString("0.##"), "0")

        TxtIsiStokToko.Text = If(DGBarang.Item("STOK_TOKO", i).Value IsNot DBNull.Value, Decimal.Parse(DGBarang.Item("STOK_TOKO", i).Value.ToString()).ToString("0.##"), "0")
        TxtSatuanToko.Text = If(DGBarang.Item("SATUAN_STOK", i).Value IsNot DBNull.Value, DGBarang.Item("SATUAN_STOK", i).Value.ToString(), "")

        TxtIsiStokGudang.Text = If(DGBarang.Item("STOK_GUDANG", i).Value IsNot DBNull.Value, Decimal.Parse(DGBarang.Item("STOK_GUDANG", i).Value.ToString()).ToString("0.##"), "0")
        TxtSatuanGudang.Text = If(DGBarang.Item("SATUAN_STOK", i).Value IsNot DBNull.Value, DGBarang.Item("SATUAN_STOK", i).Value.ToString(), "")

        CmbIsiTokoT.Text = If(DGBarang.Item("SATUAN_STOK", i).Value IsNot DBNull.Value, DGBarang.Item("SATUAN_STOK", i).Value.ToString(), "")
        CmbIsiGUdangT.Text = If(DGBarang.Item("SATUAN_STOK", i).Value IsNot DBNull.Value, DGBarang.Item("SATUAN_STOK", i).Value.ToString(), "")
        TxtSatIsiToko.Text = If(DGBarang.Item("SATUAN_ISI_STOK", i).Value IsNot DBNull.Value, DGBarang.Item("SATUAN_ISI_STOK", i).Value.ToString(), "")
        TxtSatIsiGudang.Text = If(DGBarang.Item("SATUAN_ISI_STOK", i).Value IsNot DBNull.Value, DGBarang.Item("SATUAN_ISI_STOK", i).Value.ToString(), "")
    End Sub

    Private Sub TambahStokToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TambahStokToolStripMenuItem.Click

        PAnelTambahKurang.Left = (ClientSize.Width - PAnelTambahKurang.Width) \ 2
        PAnelTambahKurang.Top = (ClientSize.Height - PAnelTambahKurang.Height) \ 2

        If FormUtama.SLokasi.Text = "GUDANG" Then
            TxtIsiStokGudangT.Enabled = True
            CmbIsiGUdangT.Enabled = True
            TxtSatIsiGudangT.Enabled = True
            TxtStokTotalGudangT.Enabled = True

            TxtIsiStokTokoT.Enabled = False
            CmbIsiTokoT.Enabled = False
            TxtSatIsiTokoT.Enabled = False
            TxtStokTotalTokoT.Enabled = False
        ElseIf FormUtama.SLokasi.Text = "TOKO" Then
            TxtIsiStokGudangT.Enabled = False
            CmbIsiGUdangT.Enabled = False
            TxtSatIsiGudangT.Enabled = False
            TxtStokTotalGudangT.Enabled = False

            TxtIsiStokTokoT.Enabled = True
            CmbIsiTokoT.Enabled = True
            TxtSatIsiTokoT.Enabled = True
            TxtStokTotalTokoT.Enabled = True
        End If

        LblJudulStok.Text = "TAMBAH STOK BARANG"

        TambahKurangstok()
        PAnelTambahKurang.Visible = True
        DGBarang.Enabled = False
        PanelOperasi.Enabled = False

        If FormUtama.SLokasi.Text = "GUDANG" Then
            TxtIsiStokGudangT.Select()
        ElseIf FormUtama.SLokasi.Text = "TOKO" Then
            TxtIsiStokTokoT.Select()
        End If

    End Sub

    Private Sub KurangiStokToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles KurangiStokToolStripMenuItem.Click
        PAnelTambahKurang.Left = (ClientSize.Width - PAnelTambahKurang.Width) \ 2
        PAnelTambahKurang.Top = (ClientSize.Height - PAnelTambahKurang.Height) \ 2

        If FormUtama.SLokasi.Text = "GUDANG" Then
            TxtIsiStokGudangT.Enabled = True
            CmbIsiGUdangT.Enabled = True
            TxtSatIsiGudangT.Enabled = True
            TxtStokTotalGudangT.Enabled = True

            TxtIsiStokTokoT.Enabled = False
            CmbIsiTokoT.Enabled = False
            TxtSatIsiTokoT.Enabled = False
            TxtStokTotalTokoT.Enabled = False
        ElseIf FormUtama.SLokasi.Text = "TOKO" Then
            TxtIsiStokGudangT.Enabled = False
            CmbIsiGUdangT.Enabled = False
            TxtSatIsiGudangT.Enabled = False
            TxtStokTotalGudangT.Enabled = False

            TxtIsiStokTokoT.Enabled = True
            CmbIsiTokoT.Enabled = True
            TxtSatIsiTokoT.Enabled = True
            TxtStokTotalTokoT.Enabled = True
        End If

        LblJudulStok.Text = "KURANGI STOK BARANG"

        TambahKurangstok()
        PAnelTambahKurang.Visible = True
        DGBarang.Enabled = False
        PanelOperasi.Enabled = False

        If FormUtama.SLokasi.Text = "GUDANG" Then
            TxtIsiStokGudangT.Select()
        ElseIf FormUtama.SLokasi.Text = "TOKO" Then
            TxtIsiStokTokoT.Select()
        End If
    End Sub

    Private Sub TxtIsiStokTokoT_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtIsiStokTokoT.KeyDown
        ' Hanya izinkan input angka, backspace, dan tombol navigasi (panah, home, end)
        If Not (Char.IsDigit(ChrW(e.KeyCode)) OrElse
                e.KeyCode = Keys.Back OrElse
                e.KeyCode = Keys.Delete OrElse
                e.KeyCode = Keys.Left OrElse
                e.KeyCode = Keys.Right) Then
            ' Tolak input lainnya
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtIsiStokGudangT_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtIsiStokGudangT.KeyDown
        ' Hanya izinkan input angka, backspace, dan tombol navigasi (panah, home, end)
        If Not (Char.IsDigit(ChrW(e.KeyCode)) OrElse
                e.KeyCode = Keys.Back OrElse
                e.KeyCode = Keys.Delete OrElse
                e.KeyCode = Keys.Left OrElse
                e.KeyCode = Keys.Right) Then
            ' Tolak input lainnya
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtIsiStokTokoT_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiStokTokoT.TextChanged, TxtSatIsiTokoT.TextChanged
        If Not String.IsNullOrEmpty(TxtIsiStokTokoT.Text) AndAlso Not String.IsNullOrEmpty(TxtSatIsiTokoT.Text) Then
            Dim isiStokToko As Decimal = 0
            Dim isiToko As Decimal = 0
            Dim isisatuankecil As Decimal = 1

            Decimal.TryParse(TxtIsiStokTokoT.Text, isiStokToko)
            Decimal.TryParse(TxtSatIsiTokoT.Text, isiToko)
            Decimal.TryParse(TxtSatIsiToko.Text, isisatuankecil)

            If isisatuankecil = 0 Then
                isisatuankecil = 1
            End If


            TxtStokTotalTokoT.Text = (isiStokToko * isiToko / isisatuankecil).ToString()
        End If
    End Sub

    Private Sub TxtIsiStokGudangT_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtIsiStokGudangT.TextChanged, TxtSatIsiGudangT.TextChanged
        If Not String.IsNullOrEmpty(TxtIsiStokGudangT.Text) AndAlso Not String.IsNullOrEmpty(TxtSatIsiGudangT.Text) Then
            Dim isiStokGudang As Decimal = 0
            Dim isiGudang As Decimal = 0
            Dim isisatuankecil As Decimal = 1

            Decimal.TryParse(TxtIsiStokGudangT.Text, isiStokGudang)
            Decimal.TryParse(TxtSatIsiGudangT.Text, isiGudang)
            Decimal.TryParse(TxtSatIsiToko.Text, isisatuankecil)

            If isisatuankecil = 0 Then
                isisatuankecil = 1
            End If

            TxtStokTotalGudangT.Text = (isiStokGudang * isiGudang / isisatuankecil).ToString()
        End If
    End Sub

    Private Sub CmbIsiTokoT_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbIsiTokoT.SelectedIndexChanged
        Using cmd As New MySqlCommand("SELECT isi FROM tbl_satuan WHERE nama = ?", conn)
            cmd.Parameters.AddWithValue("@nama", CmbIsiTokoT.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TxtSatIsiTokoT.Text = rd("isi").ToString()
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbIsiGUdangT_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbIsiGUdangT.SelectedIndexChanged
        Using cmd As New MySqlCommand("SELECT isi FROM tbl_satuan WHERE nama = ?", conn)
            cmd.Parameters.AddWithValue("@nama", CmbIsiGUdangT.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TxtSatIsiGudangT.Text = rd("isi").ToString()
                End If
            End Using
        End Using
    End Sub


    Private Sub BtnSimpanStok_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpanStok.Click
        Dim stoktokoawal As Decimal = If(Decimal.TryParse(TxtIsiStokToko.Text, stoktokoawal), stoktokoawal, 0)
        Dim stokgudangawal As Decimal = If(Decimal.TryParse(TxtIsiStokGudang.Text, stokgudangawal), stokgudangawal, 0)

        ' Parsing nilai untuk isi toko dan isi gudang
        Dim isiToko As Decimal = If(Decimal.TryParse(TxtIsiStokTokoT.Text, isiToko), isiToko, 0)
        Dim isiGudang As Decimal = If(Decimal.TryParse(TxtIsiStokGudangT.Text, isiGudang), isiGudang, 0)

        ' Parsing nilai untuk stok toko, stok gudang, dan harga beli
        Dim stokToko As Decimal = If(Decimal.TryParse(TxtStokTotalTokoT.Text, stokToko), stokToko, 0)
        Dim stokGudang As Decimal = If(Decimal.TryParse(TxtStokTotalGudangT.Text, stokGudang), stokGudang, 0)
        Dim hargaBeli As Decimal = If(Decimal.TryParse(TxtHargaBeli.Text, hargaBeli), hargaBeli, 0)

        Dim totalNominaltoko As Decimal = stokToko * hargaBeli
        Dim totalNominalgudang As Decimal = stokGudang * hargaBeli

        Dim noTransaksi As String = DateTime.Now.ToString("yyyyMMddHHmmss") ' Format unik berdasarkan tanggal dan waktu saat ini

        Dim kodebarang As String = TxtKode.Text.Trim()


        If LblJudulStok.Text = "KURANGI STOK BARANG" Then
            If FormUtama.SLokasi.Text = "TOKO" Then
                If stoktokoawal <= 0 OrElse stokToko > stoktokoawal Then
                    MessageBox.Show("Stok toko tidak mencukupi untuk dikurangi", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Sub
                End If
            End If

            If FormUtama.SLokasi.Text = "GUDANG" Then
                If stokgudangawal <= 0 OrElse stokGudang > stokgudangawal Then
                    MessageBox.Show("Stok gudang tidak mencukupi untuk dikurangi", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Sub
                End If
            End If
        End If


        ' Mulai transaksi
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            InsertHistory(transaction, noTransaksi, isiToko, isiGudang, stokToko, stokGudang, totalNominaltoko, totalNominalgudang, kodebarang)
            InsertStokTambahKurang(transaction, noTransaksi, isiToko, isiGudang, stokToko, stokGudang, kodebarang, FormUtama.SLokasi.Text)
            InsertJurnalUmum(transaction, noTransaksi, totalNominaltoko, totalNominalgudang, FormUtama.SLokasi.Text)

            UpdateStok(transaction, kodebarang, stokToko, stokGudang, FormUtama.SLokasi.Text)

            transaction.Commit()
            If LblJudulStok.Text = "TAMBAH STOK BARANG" Then
                DatabaseModule.CatatanAksiHistory("Tambah stok barang " & TxtKode.Text)
            Else
                DatabaseModule.CatatanAksiHistory("Kurangi stok barang " & TxtKode.Text)
            End If

            HitungByKode(kodebarang)
            ClearFieldsAndReloadData()

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub

    Private Sub InsertHistory(ByVal transaction As MySqlTransaction, ByVal noTransaksi As String, ByVal isiToko As Decimal, ByVal isiGudang As Decimal, ByVal stokToko As Decimal, ByVal stokGudang As Decimal, ByVal totalNominaltoko As Decimal, ByVal totalNominalgudang As Decimal, ByVal kodebarang As String)
        Dim querySimpan As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                        "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"

        Using cmdInsert As New MySqlCommand(querySimpan, conn, transaction)
            cmdInsert.Parameters.AddWithValue("@FAKTUR", noTransaksi)
            cmdInsert.Parameters.AddWithValue("@TANGGAL", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))

            If LblJudulStok.Text = "TAMBAH STOK BARANG" Then
                cmdInsert.Parameters.AddWithValue("@JENIS", "TAMBAH")
            Else
                cmdInsert.Parameters.AddWithValue("@JENIS", "KURANG")
            End If

            cmdInsert.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmdInsert.Parameters.AddWithValue("@ID_BARANG", kodebarang)
            cmdInsert.Parameters.AddWithValue("@NAMA_BARANG", TxtNama.Text)

            Dim isi As String = String.Empty
            Dim satuan As String = String.Empty
            Dim isiSatuan As String = String.Empty
            Dim totalQty As Decimal = 0D
            Dim totalNominal As Decimal = 0D

            Select Case FormUtama.SLokasi.Text
                Case "TOKO"
                    isi = isiToko
                    satuan = CmbIsiTokoT.Text
                    isiSatuan = TxtSatIsiTokoT.Text
                    totalQty = stokToko
                    totalNominal = totalNominaltoko
                Case "GUDANG"
                    isi = isiGudang
                    satuan = CmbIsiGUdangT.Text
                    isiSatuan = TxtSatIsiGudangT.Text
                    totalQty = stokGudang
                    totalNominal = totalNominalgudang
                    'Case Else
                    '    Throw New InvalidOperationException("Status tidak valid.")
            End Select


            cmdInsert.Parameters.AddWithValue("@QTY", isi)
            cmdInsert.Parameters.AddWithValue("@SATUAN", satuan)
            cmdInsert.Parameters.AddWithValue("@ISI_SATUAN", isiSatuan)
            cmdInsert.Parameters.AddWithValue("@TOTAL_QTY", totalQty)
            cmdInsert.Parameters.AddWithValue("@TOTAL_RUPIAH", totalNominal)

            cmdInsert.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmdInsert.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

            cmdInsert.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InsertStokTambahKurang(ByVal transaction As MySqlTransaction, ByVal noTransaksi As String, ByVal isiToko As Decimal, ByVal isiGudang As Decimal, ByVal stokToko As Decimal, ByVal stokGudang As Decimal, ByVal kodebarang As String, ByVal status As String)
        Dim querytambah As String = "INSERT INTO StokTambahKurang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, ID_USER, ID_KOMPUTER) " &
                             "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @ID_USER, @ID_KOMPUTER)"

        Using cmdInsert As New MySqlCommand(querytambah, conn, transaction)
            cmdInsert.Parameters.AddWithValue("@FAKTUR", noTransaksi)
            cmdInsert.Parameters.AddWithValue("@TANGGAL", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))

            If LblJudulStok.Text = "TAMBAH STOK BARANG" Then
                cmdInsert.Parameters.AddWithValue("@JENIS", "TAMBAH")
            Else
                cmdInsert.Parameters.AddWithValue("@JENIS", "KURANG")
            End If

            cmdInsert.Parameters.AddWithValue("@LOKASI", status)
            cmdInsert.Parameters.AddWithValue("@ID_BARANG", kodebarang)
            cmdInsert.Parameters.AddWithValue("@NAMA_BARANG", TxtNama.Text)

            Dim isi As String = String.Empty
            Dim satuan As String = String.Empty
            Dim isiSatuan As String = String.Empty
            Dim totalQty As Decimal = 0D
            'Dim totalNominal As Decimal = 0D

            Select Case FormUtama.SLokasi.Text
                Case "TOKO"
                    isi = isiToko
                    satuan = CmbIsiTokoT.Text
                    isiSatuan = TxtSatIsiTokoT.Text
                    totalQty = stokToko
                Case "GUDANG"
                    isi = isiGudang
                    satuan = CmbIsiGUdangT.Text
                    isiSatuan = TxtSatIsiGudangT.Text
                    totalQty = stokGudang
                    'Case Else
                    '    Throw New InvalidOperationException("Status tidak valid.")
            End Select


            cmdInsert.Parameters.AddWithValue("@QTY", isi)
            cmdInsert.Parameters.AddWithValue("@SATUAN", satuan)
            cmdInsert.Parameters.AddWithValue("@ISI_SATUAN", isiSatuan)
            cmdInsert.Parameters.AddWithValue("@TOTAL_QTY", totalQty)
            cmdInsert.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmdInsert.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

            cmdInsert.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InsertJurnalUmum(ByVal transaction As MySqlTransaction, ByVal noTransaksi As String, ByVal totalNominaltoko As Decimal, ByVal totalNominalgudang As Decimal, ByVal status As String)
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                      "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))

            Dim uraian As String = If(LblJudulStok.Text = "TAMBAH STOK BARANG", "Tambah barang", "Kurang barang")
            Dim namaAkunD As String = If(LblJudulStok.Text = "TAMBAH STOK BARANG", NAMA_REK_BARANG, LAWAN_NAMA_REK_BARANG)
            Dim nomorAkunD As String = If(LblJudulStok.Text = "TAMBAH STOK BARANG", KODE_REK_BARANG, LAWAN_KODE_REK_BARANG)
            Dim namaAkunK As String = If(LblJudulStok.Text = "TAMBAH STOK BARANG", LAWAN_NAMA_REK_BARANG, NAMA_REK_BARANG)
            Dim nomorAkunK As String = If(LblJudulStok.Text = "TAMBAH STOK BARANG", LAWAN_KODE_REK_BARANG, KODE_REK_BARANG)

            Dim nominal As Decimal = 0D

            Select Case FormUtama.SLokasi.Text
                Case "TOKO"
                    nominal = totalNominaltoko
                Case "GUDANG"
                    nominal = totalNominalgudang
                Case Else
                    Throw New InvalidOperationException("Status tidak valid.")
            End Select

            cmd.Parameters.AddWithValue("@URAIAN", uraian & " " & status & " " & TxtNama.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", namaAkunD)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", nomorAkunD)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", namaAkunK)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", nomorAkunK)
            cmd.Parameters.AddWithValue("@NOMINAL", nominal)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", uraian.ToLower())
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub UpdateStok(ByVal transaction As MySqlTransaction, ByVal kodebarang As String, ByVal stokToko As Decimal, ByVal stokGudang As Decimal, ByVal status As String)
        Dim updateValue As Decimal = 0D

        Select Case status
            Case "TOKO"
                updateValue = stokToko
            Case "GUDANG"
                updateValue = stokGudang
                'Case Else
                '    Throw New InvalidOperationException("Status tidak valid.")
        End Select
        Dim updateQuery As String = String.Empty ' Inisialisasi dilakukan di luar blok percabangan

        Select Case LblJudulStok.Text
            Case "TAMBAH STOK BARANG"
                Select Case status
                    Case "TOKO"
                        updateQuery = "UPDATE tbl_barang SET TAMBAH_TOKO = TAMBAH_TOKO + @UpdateValue WHERE ID_BARANG = @idBarang"
                    Case "GUDANG"
                        updateQuery = "UPDATE tbl_barang SET TAMBAH_GUDANG = TAMBAH_GUDANG + @UpdateValue WHERE ID_BARANG = @idBarang"
                End Select
            Case "KURANGI STOK BARANG"
                Select Case status
                    Case "TOKO"
                        updateQuery = "UPDATE tbl_barang SET KURANG_TOKO = KURANG_TOKO + @UpdateValue WHERE ID_BARANG = @idBarang"
                    Case "GUDANG"
                        updateQuery = "UPDATE tbl_barang SET KURANG_GUDANG = KURANG_GUDANG + @UpdateValue WHERE ID_BARANG = @idBarang"
                End Select

        End Select


        If updateQuery <> "" Then ' Memeriksa apakah updateQuery telah diinisialisasi
            Using updateCmd As New MySqlCommand(updateQuery, conn, transaction)
                updateCmd.Parameters.AddWithValue("@UpdateValue", updateValue)
                updateCmd.Parameters.AddWithValue("@idBarang", kodebarang)
                updateCmd.ExecuteNonQuery()
            End Using
        Else
            MessageBox.Show("Peringatan: updateQuery tidak diinisialisasi karena kondisi tidak terpenuhi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub


    Private Sub ClearFieldsAndReloadData()
        PAnelTambahKurang.Visible = False
        TxtIsiStokTokoT.Clear()
        CmbIsiTokoT.SelectedIndex = -1
        TxtSatIsiTokoT.Clear()
        TxtStokTotalTokoT.Clear()
        TxtIsiStokGudangT.Clear()
        CmbIsiGUdangT.SelectedIndex = -1
        TxtSatIsiGudangT.Clear()
        TxtStokTotalGudangT.Clear()
        DGBarang.Enabled = True
        PanelOperasi.Enabled = True
        CariData()
    End Sub

    Private Sub BtnKeluarStok_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluarStok.Click
        TxtIsiStokTokoT.Text = ""
        CmbIsiTokoT.Text = ""
        TxtSatIsiTokoT.Text = ""
        TxtStokTotalTokoT.Text = ""
        TxtIsiStokGudangT.Text = ""
        CmbIsiGUdangT.Text = ""
        TxtSatIsiGudangT.Text = ""
        TxtStokTotalGudangT.Text = ""
        PAnelTambahKurang.Visible = False
        DGBarang.Enabled = True
        PanelOperasi.Enabled = True
    End Sub


    Private Sub DetailStokToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DetailStokToolStripMenuItem.Click
        Tampildetailbarang()
    End Sub

    Private Sub ExportDataBarangToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ExportDataBarangToolStripMenuItem.Click
        Cursor = Cursors.WaitCursor

        Dim query As String = "SELECT * FROM tbl_barang"

        ' Tentukan folder export dan file TSV di dalam direktori aplikasi
        Dim exportFolder As String = Path.Combine(Directory.GetCurrentDirectory(), "Export")

        ' Membuat folder jika belum ada
        If Not Directory.Exists(exportFolder) Then
            Directory.CreateDirectory(exportFolder)
        End If

        ' Tentukan path file TSV
        Dim filePath As String = Path.Combine(exportFolder, "tbl_barang_export.tsv")

        Try
            Using cmd As New MySqlCommand(query, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    ' Buat file TSV
                    Using writer As New StreamWriter(filePath, False)
                        ' Menulis header TSV (Nama kolom dari database)
                        Dim header As String = ""
                        For i As Integer = 0 To reader.FieldCount - 1
                            header &= reader.GetName(i) & vbTab ' Gunakan vbTab untuk pemisah antar kolom
                        Next
                        writer.WriteLine(header.TrimEnd(vbTab)) ' Menghapus tab terakhir

                        ' Menulis data tabel ke file TSV
                        While reader.Read()
                            Dim row As String = ""
                            For i As Integer = 0 To reader.FieldCount - 1
                                ' Menangani nilai teks yang mengandung tab atau tanda kutip
                                Dim value As String = reader(i).ToString()
                                ' Jika ada tab atau tanda kutip, tambahkan tanda kutip ganda di sekitar nilai
                                If value.Contains(vbTab) Or value.Contains("""") Then
                                    value = """" & value.Replace("""", """""") & """"
                                End If
                                row &= value & vbTab ' Gunakan vbTab untuk pemisah antar kolom
                            Next
                            writer.WriteLine(row.TrimEnd(vbTab)) ' Menghapus tab terakhir
                        End While
                    End Using
                End Using
            End Using

            MessageBox.Show("Data telah berhasil diexport ke TSV.", "Export Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Cursor = Cursors.Default
    End Sub
    Private Sub ImportDataBarangToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ImportDataBarangToolStripMenuItem.Click
        ' Tentukan folder dan path file TSV yang akan diimpor
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "TSV Files (*.tsv)|*.tsv"
        openFileDialog.Title = "Pilih File TSV untuk Diimpor"

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Dim filePath As String = openFileDialog.FileName

            Try
                ' Membaca file TSV
                Using reader As New StreamReader(filePath)
                    ' Mulai transaksi untuk import

                    Dim transaction As MySqlTransaction = conn.BeginTransaction()

                    Try
                        ' Dapatkan nama kolom tabel dari database
                        Dim columnNames As New List(Of String)
                        Using cmd As New MySqlCommand("DESCRIBE tbl_barang", conn)
                            Using rdr As MySqlDataReader = cmd.ExecuteReader()
                                While rdr.Read()
                                    columnNames.Add(rdr("Field").ToString())
                                End While
                            End Using
                        End Using

                        ' Membaca setiap baris data dalam file TSV
                        While Not reader.EndOfStream
                            Dim line As String = reader.ReadLine()
                            Dim fields As String() = line.Split(vbTab) ' Pisahkan berdasarkan tab

                            ' Pastikan jumlah kolom di file TSV sesuai dengan jumlah kolom di tabel
                            If fields.Length = columnNames.Count Then
                                ' Buat query INSERT berdasarkan nama kolom yang didapat
                                Dim insertQuery As String = "INSERT INTO tbl_barang (" & String.Join(",", columnNames) & ") VALUES (" & String.Join(",", Enumerable.Range(0, fields.Length).Select(Function(i) "@" & "col" & (i + 1))) & ")"

                                Using cmd As New MySqlCommand(insertQuery, conn, transaction)
                                    ' Menambahkan parameter untuk setiap kolom di baris ini
                                    For i As Integer = 0 To fields.Length - 1
                                        cmd.Parameters.AddWithValue("@col" & (i + 1), fields(i))
                                    Next

                                    ' Eksekusi perintah INSERT untuk memasukkan data ke tabel
                                    cmd.ExecuteNonQuery()
                                End Using
                            Else
                                ' Jika jumlah kolom tidak sesuai, tampilkan pesan kesalahan
                                MessageBox.Show("Jumlah kolom dalam file TSV tidak sesuai dengan tabel database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Exit Sub
                            End If
                        End While

                        ' Commit transaksi jika semua data berhasil disisipkan
                        transaction.Commit()

                        MessageBox.Show("Data telah berhasil diimpor.", "Import Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        ' Jika terjadi error, rollback transaksi
                        transaction.Rollback()
                        MessageBox.Show("Terjadi kesalahan saat mengimpor data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End Using

            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan saat membuka file: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub CetakBarcodeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CetakBarcodeToolStripMenuItem.Click

    End Sub

    Private Sub LabelBarangToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LabelBarangToolStripMenuItem.Click

    End Sub


    Private Sub PerbaikiDatabase_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PerbaikiDatabase.Click
        ' Mulai transaksi
        Using transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' Update tbl_barang
                Dim queryBrg As String = "UPDATE tbl_barang " &
                                        "SET NAMA_BARANG = TRIM(NAMA_BARANG), " &
                                        "ID_BARANG = TRIM(ID_BARANG), " &
                                        "BARCODE_KECIL = TRIM(BARCODE_KECIL), " &
                                        "BARCODE_SEDANG = TRIM(BARCODE_SEDANG), " &
                                        "BARCODE_BESAR = TRIM(BARCODE_BESAR)"
                Using cmd As New MySqlCommand(queryBrg, conn, transaction)
                    cmd.ExecuteNonQuery()
                End Using

                ' Pengecekan duplikat pada ID_BARANG
                Dim queryDuplikatID As String = "SELECT ID_BARANG, COUNT(*) AS JumlahDuplikat " &
                                                "FROM tbl_barang " &
                                                "GROUP BY ID_BARANG " &
                                                "HAVING COUNT(*) > 1"

                Using cmdID As New MySqlCommand(queryDuplikatID, conn, transaction)
                    Dim idBarang As String = ""
                    Dim jumlahDuplikat As Integer = 0
                    Dim adaDuplikat As Boolean = False

                    ' Baca data duplikat terlebih dahulu
                    Using reader As MySqlDataReader = cmdID.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                idBarang = reader("ID_BARANG").ToString()
                                jumlahDuplikat = Convert.ToInt32(reader("JumlahDuplikat"))
                                adaDuplikat = True
                            End While
                        End If
                    End Using ' Reader akan otomatis tertutup di sini

                    ' Jika ada duplikat, rollback transaksi
                    If adaDuplikat Then
                        MessageBox.Show("Ada " & jumlahDuplikat & " duplikat untuk ID_BARANG: " & idBarang, "Informasi Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Cursor = Cursors.Default
                        transaction.Rollback() ' Rollback dijalankan setelah reader tertutup
                        Exit Sub
                    End If
                End Using


                ' Pengecekan duplikat pada NAMA_BARANG
                Dim queryDuplikatNama As String = "SELECT NAMA_BARANG, COUNT(*) AS JumlahDuplikat " &
                                                  "FROM tbl_barang " &
                                                  "GROUP BY NAMA_BARANG " &
                                                  "HAVING COUNT(*) > 1"

                Using cmdNama As New MySqlCommand(queryDuplikatNama, conn, transaction)
                    Dim namaBarang As String = ""
                    Dim jumlahDuplikat As Integer = 0
                    Dim adaDuplikat As Boolean = False

                    ' Baca data duplikat dulu
                    Using reader As MySqlDataReader = cmdNama.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                namaBarang = reader("NAMA_BARANG").ToString()
                                jumlahDuplikat = Convert.ToInt32(reader("JumlahDuplikat"))
                                adaDuplikat = True
                            End While
                        End If
                    End Using ' Reader ditutup otomatis di sini

                    ' Jika ada duplikat, rollback transaksi
                    If adaDuplikat Then
                        MessageBox.Show("Ada " & jumlahDuplikat & " duplikat untuk NAMA_BARANG: " & namaBarang, "Informasi Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Cursor = Cursors.Default
                        transaction.Rollback()
                        Exit Sub
                    End If
                End Using


                ' Memanggil fungsi untuk memperbaiki kolom dalam transaksi
                PerbaikiKolom(transaction)

                ' Commit transaksi jika semua operasi berhasil
                transaction.Commit()

                ' Menampilkan pesan bahwa pembaruan berhasil
                MessageBox.Show("Perbaikan database berhasil!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                DatabaseModule.CatatanAksiHistory("Perbaikan database " & "barang")

            Catch ex As Exception
                ' Rollback transaksi jika ada kesalahan
                transaction.Rollback()
                MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub PerbaikiKolom(ByVal transaction As MySqlTransaction)
        ' Daftar nama kolom yang akan diperbaiki
        Dim columnsToFix As String() = {
            "TAMBAH_TOKO",
            "KURANG_TOKO",
            "PEMBELIAN_TOKO",
            "PENJUALAN_TOKO",
            "RETUR_BELI_TOKO",
            "RETUR_JUAL_TOKO",
            "OPNAME_TOKO",
            "TRANSFER_STOK_MASUK_TOKO",
            "TRANSFER_STOK_KELUAR_TOKO",
            "TRANSFER_BARANG_MASUK_TOKO",
            "TRANSFER_BARANG_KELUAR_TOKO",
            "TAMBAH_GUDANG",
            "KURANG_GUDANG",
            "PEMBELIAN_GUDANG",
            "PENJUALAN_GUDANG",
            "RETUR_BELI_GUDANG",
            "RETUR_JUAL_GUDANG",
            "OPNAME_GUDANG",
            "TRANSFER_STOK_MASUK_GUDANG",
            "TRANSFER_STOK_KELUAR_GUDANG",
            "TRANSFER_BARANG_MASUK_GUDANG",
            "TRANSFER_BARANG_KELUAR_GUDANG",
            "AWAL_TOKO",
            "AWAL_GUDANG",
            "POINT_MEMBER",
            "POINT_KARYAWAN",
            "KOMISI_SALES_RP",
            "KOMISI_SALES_PERSEN"
        }

        For Each columnName As String In columnsToFix
            Dim updateQuery As String = "UPDATE tbl_barang SET " & columnName & " = 0 WHERE " & columnName & " IS NULL"

            Using command As New MySqlCommand(updateQuery, conn, transaction)
                command.ExecuteNonQuery()
            End Using
        Next
    End Sub

    Private Sub CetakLabelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CetakLabelToolStripMenuItem.Click
        FormCetakLabel.ShowDialog()
    End Sub

    Private Sub CetakBarcodeToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles CetakBarcodeToolStripMenuItem1.Click
        ' Validasi ada barang yang dipilih
        If DGBarang.SelectedRows.Count = 0 Then
            MessageBox.Show("Silakan pilih barang terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Ambil data barang yang dipilih
        Dim selectedRow As DataGridViewRow = DGBarang.SelectedRows(0)
        Dim namaBarang As String = selectedRow.Cells("NAMA_BARANG").Value.ToString()
        Dim kodeBarang As String = selectedRow.Cells("ID_BARANG").Value.ToString()

        ' Buka form cetak barcode dan kirim data
        Dim formCetak As New CetakLabelBarcodeTSPL()
        formCetak.NamaBarangDikirim = namaBarang
        formCetak.KodeBarangDikirim = kodeBarang
        formCetak.ShowDialog()
    End Sub

    Private Sub Form_Barang_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                Tampildetailbarang()
            Case Keys.F2
                BtnTambah.PerformClick()
            Case Keys.F3
                BtnUbah.PerformClick()
            Case Keys.F4
                BtnHapus.PerformClick()
            Case Keys.F8
                BtnSimpanStok.PerformClick()
            Case Keys.Escape
                If PanelDetailBarang.Visible = True Then
                    BtnSembunyi.PerformClick()
                ElseIf PAnelTambahKurang.Visible = True Then
                    BtnKeluarStok.PerformClick()
                Else
                    BtnKeluar.PerformClick()
                End If
        End Select
    End Sub


    Private Sub HandleDataWhere(ByVal WhereClause As String)
        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, NAMA_KATEGORI, NAMA_SUPLIYER, HARGA_BELI, HARGA_BELI_TERAKHIR, " &
                       "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, " &
                       "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, " &
                       "SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR, " &
                       "HARGA_JUAL_PARTAI_KECIL, HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, " &
                       "STOK_GUDANG, SATUAN_STOK, SATUAN_ISI_STOK " &
                       "FROM tbl_barang WHERE " & WhereClause & " ORDER BY NAMA_BARANG"
        DGBarang.Columns.Clear()

        Using da As New MySqlDataAdapter(query, conn)
            ds = New DataSet
            da.Fill(ds)
            DGBarang.DataSource = ds.Tables(0)
            Aturdgvbarangsimpel()
        End Using
    End Sub


    Private Sub UmumKecilToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UmumKecilToolStripMenuItem.Click
        Dim WhereClause As String = "ID_BARANG IN (SELECT ID_BARANG FROM tbl_barang WHERE HARGA_JUAL_UMUM_KECIL <> 0 AND HARGA_JUAL_UMUM_KECIL IS NOT NULL AND HARGA_BELI * ISI_UMUM_KECIL > HARGA_JUAL_UMUM_KECIL)"
        HandleDataWhere(WhereClause)
    End Sub

    Private Sub UmumSedangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UmumSedangToolStripMenuItem.Click
        Dim WhereClause As String = "ID_BARANG IN (SELECT ID_BARANG FROM tbl_barang WHERE HARGA_JUAL_UMUM_SEDANG <> 0 AND HARGA_JUAL_UMUM_SEDANG IS NOT NULL AND HARGA_BELI * ISI_UMUM_SEDANG > HARGA_JUAL_UMUM_SEDANG)"
        HandleDataWhere(WhereClause)
    End Sub

    Private Sub UmumBesarToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UmumBesarToolStripMenuItem.Click
        Dim WhereClause As String = "ID_BARANG IN (SELECT ID_BARANG FROM tbl_barang WHERE HARGA_JUAL_UMUM_BESAR <> 0 AND HARGA_JUAL_UMUM_BESAR IS NOT NULL AND HARGA_BELI * ISI_UMUM_BESAR > HARGA_JUAL_UMUM_BESAR)"
        HandleDataWhere(WhereClause)
    End Sub

    Private Sub PartaiKecilToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PartaiKecilToolStripMenuItem.Click
        Dim WhereClause As String = "ID_BARANG IN (SELECT ID_BARANG FROM tbl_barang WHERE HARGA_JUAL_PARTAI_KECIL <> 0 AND HARGA_JUAL_PARTAI_KECIL IS NOT NULL AND HARGA_BELI * ISI_PARTAI_KECIL > HARGA_JUAL_PARTAI_KECIL)"
        HandleDataWhere(WhereClause)
    End Sub

    Private Sub PartaiSedangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PartaiSedangToolStripMenuItem.Click
        Dim WhereClause As String = "ID_BARANG IN (SELECT ID_BARANG FROM tbl_barang WHERE HARGA_JUAL_PARTAI_SEDANG <> 0 AND HARGA_JUAL_PARTAI_SEDANG IS NOT NULL AND HARGA_BELI * ISI_PARTAI_SEDANG > HARGA_JUAL_PARTAI_SEDANG)"
        HandleDataWhere(WhereClause)
    End Sub

    Private Sub PartaiBesarToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PartaiBesarToolStripMenuItem.Click
        Dim WhereClause As String = "ID_BARANG IN (SELECT ID_BARANG FROM tbl_barang WHERE HARGA_JUAL_PARTAI_BESAR <> 0 AND HARGA_JUAL_PARTAI_BESAR IS NOT NULL AND HARGA_BELI * ISI_PARTAI_BESAR > HARGA_JUAL_PARTAI_BESAR)"
        HandleDataWhere(WhereClause)
    End Sub

    Private Sub HistoriPembelianToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HistoriPembelianToolStripMenuItem.Click
        TampilkanHistoriPembelian()
    End Sub

    Public Sub TampilkanHistoriPembelian()
        ' Ambil dari baris yang sedang dipilih di DGVBarang
        If DGBarang.SelectedRows.Count = 0 Then
            MessageBox.Show("Silakan pilih barang terlebih dahulu!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedRow As DataGridViewRow = DGBarang.SelectedRows(0)
        Dim idBarang As String = selectedRow.Cells("ID_BARANG").Value.ToString()
        Dim namaBarang As String = selectedRow.Cells("NAMA_BARANG").Value.ToString()

        ' Buat form untuk menampilkan user control
        Dim formHistori As New Form()
        formHistori.Text = $"Histori Pembelian - {namaBarang}"
        formHistori.Size = New Size(1000, 600)
        formHistori.StartPosition = FormStartPosition.CenterScreen

        ' Buat instance user control
        Dim uc As New HistoriPembelianUC()
        uc.KodeBarang = idBarang
        uc.Dock = DockStyle.Fill

        ' Hubungkan event jika ingin update harga otomatis
        AddHandler uc.BarisDiklik, AddressOf HistoriPembelian_BarisDiklick

        ' Tambahkan ke form
        formHistori.Controls.Add(uc)

        ' Tampilkan sebagai modal dialog
        formHistori.ShowDialog()
    End Sub

    Private Sub HistoriPembelian_BarisDiklick(fakturBeli As String, harga As Decimal)
        ' Update harga beli di form barang jika diperlukan
        If harga > 0 AndAlso DGBarang.SelectedRows.Count > 0 Then
            ' Update tampilan sementara
            TxtHargaBeli.Text = harga.ToString("N0")

            ' Jika ingin update database juga:
            ' UpdateHargaBeliDiDatabase(DGBarang.SelectedRows(0).Cells("ID_BARANG").Value.ToString(), harga)
        End If
    End Sub

    Private Sub BarcodeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BarcodeToolStripMenuItem.Click

    End Sub
End Class