Public Class CariBarang
    Private dragging As Boolean
    Private offsetX As Integer
    Private offsetY As Integer

    Private moduleName As String

    ' Event MouseDown pada form (MyBase) atau area yang diinginkan (misal: Panel4, LblNama)
    Private Sub CariBarang_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown, Panel4.MouseDown, LblNama.MouseDown
        If e.Button = MouseButtons.Left Then
            dragging = True
            offsetX = e.X
            offsetY = e.Y
        End If
    End Sub

    ' Event MouseUp pada form atau area yang diinginkan
    Private Sub CariBarang_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, Panel4.MouseUp, LblNama.MouseUp
        dragging = False
    End Sub

    ' Event MouseMove pada form atau area yang diinginkan
    Private Sub CariBarang_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove, Panel4.MouseMove, LblNama.MouseMove
        If dragging Then
            ' Memindahkan form sesuai pergerakan mouse
            Me.Location = New Point(Me.Location.X + e.X - offsetX, Me.Location.Y + e.Y - offsetY)
        End If
    End Sub

    Private Sub CariBarangPembelian_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        moduleName = ModulHakAkses.BacaHakAksesSemua(FormHakUser.LblBeliSatuan.Text)
        TextCari.Clear()
        'Data_Record()
        Kondisiawal()
        TextCari.Select()
        TextCari.Focus()
    End Sub

    Public Sub Kondisiawal()
        TxtKode.Clear()
        TxtNama.Clear()
        TxtIsiKecil.Clear()
        TxtHargabeli.Clear()
        TxtHargaJual.Clear()
        TxtQtyKecil.Clear()
        TxtStok.Clear()
        TampilSatuan()
    End Sub

    Public Sub TampilSatuan()
        CmbSatuanKecil.Items.Clear()
        CmbSatuanSedang.Items.Clear()
        CmbSatuanBesar.Items.Clear()
        ' Ambil data satuan dari database
        Using cmd As New MySqlCommand("SELECT nama FROM tbl_satuan ORDER BY nama ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                Do While rd.Read()
                    Dim namaSatuan As String = rd.Item("nama").ToString()

                    ' Tambahkan item jika belum ada di ComboBox
                    If Not CmbSatuanKecil.Items.Contains(namaSatuan) Then
                        CmbSatuanKecil.Items.Add(namaSatuan)
                    End If
                    If Not CmbSatuanSedang.Items.Contains(namaSatuan) Then
                        CmbSatuanSedang.Items.Add(namaSatuan)
                    End If
                    If Not CmbSatuanBesar.Items.Contains(namaSatuan) Then
                        CmbSatuanBesar.Items.Add(namaSatuan)
                    End If
                Loop
            End Using
        End Using

        ' Tambahkan item kosong di posisi terakhir jika belum ada
        If Not CmbSatuanKecil.Items.Contains("") Then
            CmbSatuanKecil.Items.Add("")
        End If
        If Not CmbSatuanSedang.Items.Contains("") Then
            CmbSatuanSedang.Items.Add("")
        End If
        If Not CmbSatuanBesar.Items.Contains("") Then
            CmbSatuanBesar.Items.Add("")
        End If
    End Sub


    Private Sub TextCari_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TextCari.TextChanged
        If TxtJenisTransaksi.Text = "Penjualan" Then
            CaribarangJual()
        Else
            CaribarangBeli()
        End If
    End Sub



    Private Sub CaribarangBeli()
        DGCariBarang.Columns.Clear()

        Dim queryString As String =
        "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_BELI_TERAKHIR, SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, STOK_TOKO, STOK_GUDANG " &
        "FROM tbl_barang " &
        "WHERE ID_BARANG LIKE @SearchKeyword OR " &
        "NAMA_BARANG LIKE @SearchKeyword OR " &
        "BARCODE_KECIL LIKE @SearchKeyword OR " &
        "BARCODE_SEDANG LIKE @SearchKeyword OR " &
        "BARCODE_BESAR LIKE @SearchKeyword"

        Using cmd As New MySqlCommand(queryString, conn)
            cmd.Parameters.AddWithValue("@SearchKeyword", "%" & TextCari.Text & "%")
            Using da As New MySqlDataAdapter(cmd)
                Dim ds As New DataSet
                da.Fill(ds, "tbl_barang")
                DGCariBarang.DataSource = ds.Tables("tbl_barang")
            End Using
        End Using

        DGCariBarang.Columns("ID_BARANG").HeaderText = "Kode"
        DGCariBarang.Columns("NAMA_BARANG").HeaderText = "Nama Barang"
        DGCariBarang.Columns("HARGA_BELI").HeaderText = "HPP"
        DGCariBarang.Columns("HARGA_BELI_TERAKHIR").HeaderText = "Beli Terakhir"
        DGCariBarang.Columns("SATUAN_UMUM_KECIL").HeaderText = "Satuan Kecil"
        DGCariBarang.Columns("SATUAN_UMUM_SEDANG").HeaderText = "Satuan Sedang"
        DGCariBarang.Columns("SATUAN_UMUM_BESAR").HeaderText = "Satuan Besar"
        DGCariBarang.Columns("ISI_UMUM_KECIL").Visible = False
        DGCariBarang.Columns("ISI_UMUM_SEDANG").Visible = False
        DGCariBarang.Columns("ISI_UMUM_BESAR").Visible = False
        DGCariBarang.Columns("STOK_TOKO").HeaderText = "Stok Toko"
        DGCariBarang.Columns("STOK_GUDANG").HeaderText = "Stok Gudang"


        Dim columnNames() As String = {"HARGA_BELI", "HARGA_BELI_TERAKHIR", "STOK_TOKO", "STOK_GUDANG"}
        For Each colName As String In columnNames
            If DGCariBarang.Columns.Contains(colName) Then
                DGCariBarang.Columns(colName).DefaultCellStyle.Format = "###,###"
                DGCariBarang.Columns(colName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next

        With DGCariBarang
            .ReadOnly = True
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = True
            .AllowUserToResizeColumns = True
            .AllowUserToResizeRows = False
            .Columns("NAMA_BARANG").FillWeight = 200
            .ClearSelection()
        End With
    End Sub

    Private Sub CaribarangJual()
        DGCariBarang.Columns.Clear()

        Dim queryString As String =
        "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, " &
        "SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR, " &
        "HARGA_JUAL_UMUM_KECIL, HARGA_JUAL_UMUM_SEDANG, HARGA_JUAL_UMUM_BESAR, SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, " &
        "SATUAN_PARTAI_BESAR, ISI_PARTAI_KECIL, ISI_PARTAI_SEDANG, ISI_PARTAI_BESAR, HARGA_JUAL_PARTAI_KECIL, " &
        "HARGA_JUAL_PARTAI_SEDANG, HARGA_JUAL_PARTAI_BESAR, STOK_TOKO, STOK_GUDANG " &
        "FROM tbl_barang " &
        "WHERE ID_BARANG LIKE @SearchKeyword OR " &
        "NAMA_BARANG LIKE @SearchKeyword OR " &
        "BARCODE_KECIL LIKE @SearchKeyword OR " &
        "BARCODE_SEDANG LIKE @SearchKeyword OR " &
        "BARCODE_BESAR LIKE @SearchKeyword"

        Using cmd As New MySqlCommand(queryString, conn)
            cmd.Parameters.AddWithValue("@SearchKeyword", "%" & TextCari.Text & "%")
            Using da As New MySqlDataAdapter(cmd)
                Dim ds As New DataSet
                da.Fill(ds, "tbl_barang")
                DGCariBarang.DataSource = ds.Tables("tbl_barang")
            End Using
        End Using

        DGCariBarang.Columns("ID_BARANG").HeaderText = "Kode"
        DGCariBarang.Columns("NAMA_BARANG").HeaderText = "Nama Barang"
        DGCariBarang.Columns("HARGA_BELI").Visible = False
        DGCariBarang.Columns("SATUAN_UMUM_KECIL").Visible = False
        DGCariBarang.Columns("SATUAN_UMUM_SEDANG").Visible = False
        DGCariBarang.Columns("SATUAN_UMUM_BESAR").Visible = False
        DGCariBarang.Columns("ISI_UMUM_KECIL").Visible = False
        DGCariBarang.Columns("ISI_UMUM_SEDANG").Visible = False
        DGCariBarang.Columns("ISI_UMUM_BESAR").Visible = False
        DGCariBarang.Columns("HARGA_JUAL_UMUM_KECIL").HeaderText = "Harga Jual Kecil"
        DGCariBarang.Columns("HARGA_JUAL_UMUM_SEDANG").HeaderText = "Harga Jual Sedang"
        DGCariBarang.Columns("HARGA_JUAL_UMUM_BESAR").HeaderText = "Harga Jual Besar"
        DGCariBarang.Columns("SATUAN_PARTAI_KECIL").Visible = False
        DGCariBarang.Columns("SATUAN_PARTAI_SEDANG").Visible = False
        DGCariBarang.Columns("SATUAN_PARTAI_BESAR").Visible = False
        DGCariBarang.Columns("ISI_PARTAI_KECIL").Visible = False
        DGCariBarang.Columns("ISI_PARTAI_SEDANG").Visible = False
        DGCariBarang.Columns("ISI_PARTAI_BESAR").Visible = False
        DGCariBarang.Columns("HARGA_JUAL_PARTAI_KECIL").HeaderText = "Harga Jual Partai Kecil"
        DGCariBarang.Columns("HARGA_JUAL_PARTAI_SEDANG").HeaderText = "Harga Jual Partai Sedang"
        DGCariBarang.Columns("HARGA_JUAL_PARTAI_BESAR").HeaderText = "Harga Jual Partai Besar"
        DGCariBarang.Columns("STOK_TOKO").HeaderText = "Stok Toko"
        DGCariBarang.Columns("STOK_GUDANG").HeaderText = "Stok Gudang"

        Dim columnNames() As String = {"HARGA_JUAL_UMUM_KECIL", "HARGA_JUAL_UMUM_SEDANG", "HARGA_JUAL_UMUM_BESAR", "HARGA_JUAL_PARTAI_KECIL", "HARGA_JUAL_PARTAI_SEDANG", "HARGA_JUAL_PARTAI_BESAR", "STOK_TOKO", "STOK_GUDANG"}
        For Each colName As String In columnNames
            If DGCariBarang.Columns.Contains(colName) Then
                DGCariBarang.Columns(colName).DefaultCellStyle.Format = "###,###"
                DGCariBarang.Columns(colName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next

        ' Menyembunyikan kolom harga jual berdasarkan jenis penjualan
        Dim hargaBeliColumnNames As String() ' Mendeklarasikan array kolom harga beli yang akan disembunyikan
        If FormPenjualan.LblJenisPl.Text = "Partai" Then
            hargaBeliColumnNames = {"HARGA_JUAL_PARTAI_KECIL", "HARGA_JUAL_PARTAI_SEDANG", "HARGA_JUAL_PARTAI_BESAR"}
        Else
            hargaBeliColumnNames = {"HARGA_JUAL_UMUM_KECIL", "HARGA_JUAL_UMUM_SEDANG", "HARGA_JUAL_UMUM_BESAR"}
        End If

        ' Iterasi melalui nama kolom yang ingin disembunyikan
        For Each colName As String In hargaBeliColumnNames
            ' Pastikan kolom dengan nama yang dimaksud ada dalam DataGridView
            If DGCariBarang.Columns.Contains(colName) Then
                ' Sembunyikan kolom tersebut
                DGCariBarang.Columns(colName).Visible = False
            End If
        Next


        With DGCariBarang
            .ReadOnly = True
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = True
            .AllowUserToResizeColumns = True
            .AllowUserToResizeRows = False
            .Columns("NAMA_BARANG").FillWeight = 200
            .ClearSelection()
        End With
    End Sub


    Private Sub TextCari_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextCari.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Down Then ' If Enter or Down arrow key is pressed
            If DGCariBarang.Rows.Count > 0 Then ' Ensure there are rows in the DataGridView
                DGCariBarang.Focus()
                DGCariBarang.CurrentCell = DGCariBarang.Rows(0).Cells(0) ' Select the first cell of the first row
                DGCariBarang.SelectionMode = DataGridViewSelectionMode.FullRowSelect ' Set selection mode to full row select
                DGCariBarang.Rows(0).Selected = True ' Select the first row
            End If
            e.SuppressKeyPress = True ' Prevent default action for Enter or Down arrow
        End If
    End Sub

    Private Sub DGCariBarang_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles DGCariBarang.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Isidata()
        End If
    End Sub

    Private Sub DGCariBarang_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGCariBarang.CellClick
        Isidata()
    End Sub


    Private Sub Isidata()
        Dim baris As Integer
        If DGCariBarang.CurrentRow IsNot Nothing Then
            baris = DGCariBarang.CurrentRow.Index

            If TxtJenisTransaksi.Text = "Penjualan" Then
                With DGCariBarang
                    If FormPenjualan.LblJenisPl.Text = "Partai" Then
                        TxtKode.Text = .Item("ID_BARANG", baris).Value.ToString()
                        TxtNama.Text = .Item("NAMA_BARANG", baris).Value.ToString()
                        TxtHargabeli.Text = Convert.ToDecimal(.Item("HARGA_BELI", baris).Value).ToString("###,###", Globalization.CultureInfo.GetCultureInfo("id-ID"))
                        CmbSatuanKecil.Text = .Item("SATUAN_PARTAI_KECIL", baris).Value.ToString()
                        TxtIsiKecil.Text = .Item("ISI_PARTAI_KECIL", baris).Value.ToString()
                        TxtHargaKecil.Text = .Item("HARGA_JUAL_PARTAI_KECIL", baris).Value.ToString()
                        CmbSatuanSedang.Text = .Item("SATUAN_PARTAI_SEDANG", baris).Value.ToString()
                        TxtIsiSedang.Text = .Item("ISI_PARTAI_SEDANG", baris).Value.ToString()
                        TxtHargaSedang.Text = .Item("HARGA_JUAL_PARTAI_SEDANG", baris).Value.ToString()
                        CmbSatuanBesar.Text = .Item("SATUAN_PARTAI_SEDANG", baris).Value.ToString()
                        TxtIsiBesar.Text = .Item("ISI_PARTAI_SEDANG", baris).Value.ToString()
                        TxtHargaBesar.Text = .Item("HARGA_JUAL_PARTAI_BESAR", baris).Value.ToString()
                        TxtStokToko.Text = .Item("STOK_TOKO", baris).Value.ToString()
                        TXtStokGudang.Text = .Item("STOK_GUDANG", baris).Value.ToString()
                    Else
                        TxtKode.Text = .Item("ID_BARANG", baris).Value.ToString()
                        TxtNama.Text = .Item("NAMA_BARANG", baris).Value.ToString()
                        TxtHargabeli.Text = Convert.ToDecimal(.Item("HARGA_BELI", baris).Value).ToString("###,###", Globalization.CultureInfo.GetCultureInfo("id-ID"))
                        CmbSatuanKecil.Text = .Item("SATUAN_UMUM_KECIL", baris).Value.ToString()
                        TxtIsiKecil.Text = .Item("ISI_UMUM_KECIL", baris).Value.ToString()
                        TxtHargaKecil.Text = .Item("HARGA_JUAL_UMUM_KECIL", baris).Value.ToString()
                        CmbSatuanSedang.Text = .Item("SATUAN_UMUM_SEDANG", baris).Value.ToString()
                        TxtIsiSedang.Text = .Item("ISI_UMUM_SEDANG", baris).Value.ToString()
                        TxtHargaSedang.Text = .Item("HARGA_JUAL_UMUM_SEDANG", baris).Value.ToString()
                        CmbSatuanBesar.Text = .Item("SATUAN_UMUM_SEDANG", baris).Value.ToString()
                        TxtIsiBesar.Text = .Item("ISI_UMUM_SEDANG", baris).Value.ToString()
                        TxtHargaBesar.Text = .Item("HARGA_JUAL_UMUM_BESAR", baris).Value.ToString()
                        TxtStokToko.Text = .Item("STOK_TOKO", baris).Value.ToString()
                        TXtStokGudang.Text = .Item("STOK_GUDANG", baris).Value.ToString()
                    End If

                    If FormPenjualan.LblLokasiBarang.Text = "GUDANG" Then
                        TxtStok.Text = TXtStokGudang.Text
                    ElseIf FormPenjualan.LblLokasiBarang.Text = "TOKO" Then
                        TxtStok.Text = TxtStokToko.Text
                    End If
                    TxtQtyKecil.Text = 0
                    TxtQtySedang.Text = 0
                    TxtQtyBesar.Text = 0
                End With

            ElseIf TxtJenisTransaksi.Text = "Pembelian" Then
                With DGCariBarang
                    TxtKode.Text = If(IsDBNull(.Item("ID_BARANG", baris).Value), String.Empty, .Item("ID_BARANG", baris).Value.ToString())
                    TxtNama.Text = If(IsDBNull(.Item("NAMA_BARANG", baris).Value), String.Empty, .Item("NAMA_BARANG", baris).Value.ToString())

                    TxtHargabeli.Text = If(IsDBNull(.Item("HARGA_BELI", baris).Value), "0", Convert.ToDecimal(.Item("HARGA_BELI", baris).Value).ToString("###,###", cultureIndonesia))
                    TxtHargaJual.Text = If(IsDBNull(.Item("HARGA_BELI_TERAKHIR", baris).Value), "0", Convert.ToDecimal(.Item("HARGA_BELI_TERAKHIR", baris).Value).ToString("###,###", cultureIndonesia))

                    CmbSatuanKecil.Text = If(IsDBNull(.Item("SATUAN_UMUM_KECIL", baris).Value), String.Empty, .Item("SATUAN_UMUM_KECIL", baris).Value.ToString())
                    TxtIsiKecil.Text = If(IsDBNull(.Item("ISI_UMUM_KECIL", baris).Value), "1", .Item("ISI_UMUM_KECIL", baris).Value.ToString())

                    CmbSatuanSedang.Text = If(IsDBNull(.Item("SATUAN_UMUM_SEDANG", baris).Value), String.Empty, .Item("SATUAN_UMUM_SEDANG", baris).Value.ToString())
                    TxtIsiSedang.Text = If(IsDBNull(.Item("ISI_UMUM_SEDANG", baris).Value), "1", .Item("ISI_UMUM_SEDANG", baris).Value.ToString())

                    CmbSatuanBesar.Text = If(IsDBNull(.Item("SATUAN_UMUM_BESAR", baris).Value), String.Empty, .Item("SATUAN_UMUM_BESAR", baris).Value.ToString())
                    TxtIsiBesar.Text = If(IsDBNull(.Item("ISI_UMUM_BESAR", baris).Value), "1", .Item("ISI_UMUM_BESAR", baris).Value.ToString())

                    TxtQtyKecil.Text = "0"
                    TxtQtySedang.Text = "0"
                    TxtQtyBesar.Text = "0"


                End With
            End If

            TxtQtyKecil.Select()
        Else
            MessageBox.Show("Tidak ada baris yang dipilih.")
        End If
    End Sub

    Private Sub BtnClose_Click_1(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub TxtQtyKecil_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtQtyKecil.TextChanged
        If Not String.IsNullOrEmpty(TxtQtyKecil.Text) AndAlso TxtQtyKecil.Text <> "0" Then
            TxtQtySedang.Text = "0"
            TxtQtyBesar.Text = "0"
        End If
    End Sub

    Private Sub TxtQtySedang_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtQtySedang.TextChanged
        If Not String.IsNullOrEmpty(TxtQtySedang.Text) AndAlso TxtQtySedang.Text <> "0" Then
            TxtQtyKecil.Text = "0"
            TxtQtyBesar.Text = "0"
        End If
    End Sub

    Private Sub TxtQtyBesar_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtQtyBesar.TextChanged
        If Not String.IsNullOrEmpty(TxtQtyBesar.Text) AndAlso TxtQtyBesar.Text <> "0" Then
            TxtQtyKecil.Text = "0"
            TxtQtySedang.Text = "0"
        End If
    End Sub

    Private Sub CmbSatuanSedang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbSatuanSedang.SelectedIndexChanged
        ' Jika tidak ada item yang dipilih atau nilai kosong
        If CmbSatuanSedang.SelectedIndex = -1 OrElse String.IsNullOrWhiteSpace(CmbSatuanSedang.Text) Then
            TxtQtySedang.Enabled = False
            TxtQtySedang.Text = "0"
        Else
            TxtQtySedang.Enabled = True
        End If
    End Sub

    Private Sub CmbSatuanBesar_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbSatuanBesar.SelectedIndexChanged
        ' Jika tidak ada item yang dipilih atau nilai kosong
        If CmbSatuanBesar.SelectedIndex = -1 OrElse String.IsNullOrWhiteSpace(CmbSatuanBesar.Text) Then
            TxtQtyBesar.Enabled = False
            TxtQtyBesar.Text = "0"
        Else
            TxtQtyBesar.Enabled = True
        End If
    End Sub

    Private Sub TxtJumlah_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtQtyKecil.KeyPress
        If e.KeyChar = Chr(13) Then
            Tambah()
        End If
    End Sub

    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Tambah()
    End Sub

    Private Sub CariBarangPembelian_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F10 Then
            Tambah()
        ElseIf e.KeyCode = Keys.Escape Then
            Close()
        End If
    End Sub

    Public Sub Tambah()
        If TxtKode.Text <> "" And TxtNama.Text <> "" Then
            If TxtJenisTransaksi.Text = "Penjualan" Then
                ' Cek apakah DgvData memiliki baris
                If FormPenjualan.DgvData.Rows.Count > 0 Then
                    ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
                    FormPenjualan.DgvData.CurrentCell = FormPenjualan.DgvData(1, FormPenjualan.DgvData.Rows.Count - 1)

                    ' Mengatur baris terakhir sebagai baris yang dipilih
                    FormPenjualan.DgvData.Rows(FormPenjualan.DgvData.Rows.Count - 1).Selected = True
                End If

                TambahDataPenjualan()

            ElseIf TxtJenisTransaksi.Text = "Pembelian" Then
                ' Cek apakah DgvData memiliki baris
                If FormPembelian.DgvData.Rows.Count > 0 Then
                    ' Mengatur sel aktif pada kolom kedua (indeks 1) dan baris terakhir
                    FormPembelian.DgvData.CurrentCell = FormPembelian.DgvData(1, FormPembelian.DgvData.Rows.Count - 1)

                    ' Mengatur baris terakhir sebagai baris yang dipilih
                    FormPembelian.DgvData.Rows(FormPembelian.DgvData.Rows.Count - 1).Selected = True
                End If

                TambahDataPembelian()

            End If
        Else
            TextCari.Select()
        End If
        Kondisiawal()
    End Sub

    Private Sub TambahDataPembelian()
        If moduleName = "Tidak" Then
            For Each row As DataGridViewRow In FormPembelian.DgvData.Rows
                If row.Cells("Id").Value IsNot Nothing AndAlso row.Cells("Id").Value.ToString() = TxtKode.Text Then
                    MessageBox.Show(TxtNama.Text & " sudah ada dalam daftar!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Kondisiawal()
                    Exit Sub
                End If
            Next
        End If



        ' Membuat baris baru hanya untuk sel yang dipilih
        Dim indeksBaris As Integer = FormPembelian.DgvData.SelectedCells(0).RowIndex
        Dim jumlahKolom As Integer = FormPembelian.DgvData.ColumnCount ' Mengambil jumlah kolom dari DataGridView

        ' Buat array string kosong sejumlah kolom
        Dim nilaiAwal() As String = Enumerable.Repeat("", jumlahKolom).ToArray()

        ' Sisipkan baris baru dengan nilai awal kosong
        FormPembelian.DgvData.Rows.Insert(indeksBaris, nilaiAwal)


        ' Mendapatkan kolom ComboBoxDataGridView dengan nama "SATUAN"
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(FormPembelian.DgvData.Rows(indeksBaris).Cells("Satuan"), DataGridViewComboBoxCell)

        ' Membersihkan item yang sudah ada di kolom ComboBoxDataGridView
        kolomSatuan.Items.Clear()

        ' Mengambil satuan yang berbeda dari database
        Dim sql As String = "SELECT DISTINCT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG LIKE ?"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@ID_BARANG", "%" & TxtKode.Text & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        Dim satuanKecil As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_KECIL")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_KECIL")), "")
                        Dim satuanSedang As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
                        Dim satuanBesar As String = If(Not rd.IsDBNull(rd.GetOrdinal("SATUAN_UMUM_BESAR")), rd.GetString(rd.GetOrdinal("SATUAN_UMUM_BESAR")), "")

                        ' Menambahkan item yang tidak kosong ke ComboBoxDataGridView
                        If Not String.IsNullOrEmpty(satuanKecil) Then
                            kolomSatuan.Items.Add(satuanKecil)
                        End If

                        If Not String.IsNullOrEmpty(satuanSedang) Then
                            kolomSatuan.Items.Add(satuanSedang)
                        End If

                        If Not String.IsNullOrEmpty(satuanBesar) Then
                            kolomSatuan.Items.Add(satuanBesar)
                        End If
                    End While
                End If
            End Using
        End Using


        ' Mengambil nilai dari input lainnya
        Dim kode As String = TxtKode.Text
        Dim satuan As String = CmbSatuanKecil.Text
        Dim isi As Decimal = 1
        Dim Average As Decimal = 0
        Dim hargaBeliterakhir As Decimal = 0

        ' Mengonversi input harga beli dan harga jual dengan aman
        Decimal.TryParse(TxtHargabeli.Text, Average)
        Decimal.TryParse(TxtHargaJual.Text, hargaBeliterakhir)

        Dim qty As Decimal = 1

        ' Memilih Qty dan Satuan yang sesuai
        If Not String.IsNullOrWhiteSpace(TxtQtyKecil.Text) AndAlso Decimal.TryParse(TxtQtyKecil.Text, qty) AndAlso qty > 0 Then
            satuan = CmbSatuanKecil.Text
            Decimal.TryParse(TxtIsiKecil.Text, isi)
        ElseIf Not String.IsNullOrWhiteSpace(TxtQtySedang.Text) AndAlso Decimal.TryParse(TxtQtySedang.Text, qty) AndAlso qty > 0 Then
            satuan = CmbSatuanSedang.Text
            Decimal.TryParse(TxtIsiSedang.Text, isi)
        ElseIf Not String.IsNullOrWhiteSpace(TxtQtyBesar.Text) AndAlso Decimal.TryParse(TxtQtyBesar.Text, qty) AndAlso qty > 0 Then
            satuan = CmbSatuanBesar.Text
            Decimal.TryParse(TxtIsiBesar.Text, isi)
        End If

        ' Menghitung HPP dan HPPAverage
        Dim HPP As Decimal = hargaBeliterakhir * isi
        Dim HPPAverage As Decimal = Average * isi

        ' Menetapkan nilai untuk baris yang baru ditambahkan
        With FormPembelian.DgvData
            .Rows(indeksBaris).Cells("Id").Value = kode
            .Rows(indeksBaris).Cells("nama").Value = TxtNama.Text
            .Rows(indeksBaris).Cells("Hargabeli").Value = HPP
            .Rows(indeksBaris).Cells("qty").Value = qty
            .Rows(indeksBaris).Cells("Satuan").Value = satuan
            .Rows(indeksBaris).Cells("isi").Value = isi
            .Rows(indeksBaris).Cells("HargaBeliSat").Value = HPPAverage
            .Rows(indeksBaris).Cells("QtySat").Value = qty * isi
            .Rows(indeksBaris).Cells("Totalharga").Value = qty * HPP
            .Rows(indeksBaris).Cells("Average").Value = Average
            .Rows(indeksBaris).Cells("HargaSebelumnya").Value = hargaBeliterakhir
        End With



        ' Melakukan pembaruan pada ringkasan atau operasi relevan lainnya
        FormPembelian.UpdateSemuaTotal()


    End Sub


    Private Sub TambahDataPenjualan()
        Dim DgvData As DataGridView = FormPenjualan.DgvData

        Dim moduleName As String = ModulHakAkses.BacaHakAksesSemua(FormHakUser.LblJualSatuan.Text)

        If moduleName = "Tidak" Then
            For Each row As DataGridViewRow In DgvData.Rows
                If row.Cells("Kode").Value IsNot Nothing AndAlso row.Cells("Kode").Value.ToString() = TxtKode.Text Then
                    MessageBox.Show(TxtNama.Text & " sudah ada dalam daftar!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Kondisiawal()
                    Exit Sub
                End If
            Next
        End If


        ' Membuat baris baru hanya untuk sel yang dipilih
        Dim indeksBaris As Integer = DgvData.SelectedCells(0).RowIndex
        Dim jumlahKolom As Integer = DgvData.ColumnCount ' Mengambil jumlah kolom dari DataGridView

        ' Buat array string kosong sejumlah kolom
        Dim nilaiAwal() As String = Enumerable.Repeat("", jumlahKolom).ToArray()

        ' Sisipkan baris baru dengan nilai awal kosong
        DgvData.Rows.Insert(indeksBaris, nilaiAwal)


        ' Sekarang tambahkan kode untuk mengisi ComboBoxCell dengan item yang sesuai

        Dim isPartai As Boolean = FormPenjualan.LblJenisPl.Text = "Partai"
        Dim kolomSatuan As DataGridViewComboBoxCell = CType(DgvData.Rows(indeksBaris).Cells("Satuan"), DataGridViewComboBoxCell)

        ' Membersihkan item yang sudah ada di kolom ComboBoxDataGridView
        kolomSatuan.Items.Clear()

        Dim querySatuanPartai As String = "SELECT DISTINCT SATUAN_PARTAI_KECIL, SATUAN_PARTAI_SEDANG, SATUAN_PARTAI_BESAR FROM tbl_barang WHERE ID_BARANG LIKE ?"
        Dim querySatuanUmum As String = "SELECT DISTINCT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG LIKE ?"

        Dim query As String = If(isPartai, querySatuanPartai, querySatuanUmum)
        Using cmdSatuan As New MySqlCommand(query, conn)
            cmdSatuan.Parameters.AddWithValue("@ID_BARANG", "%" & TxtKode.Text & "%")

            Using rdSatuan As MySqlDataReader = cmdSatuan.ExecuteReader()
                If rdSatuan.HasRows Then
                    While rdSatuan.Read()
                        Dim satuanKecil As String = If(rdSatuan(0) IsNot DBNull.Value, rdSatuan(0).ToString(), String.Empty)
                        Dim satuanSedang As String = If(rdSatuan(1) IsNot DBNull.Value, rdSatuan(1).ToString(), String.Empty)
                        Dim satuanBesar As String = If(rdSatuan(2) IsNot DBNull.Value, rdSatuan(2).ToString(), String.Empty)

                        ' Menambahkan item yang tidak kosong ke ComboBoxDataGridView
                        If Not String.IsNullOrEmpty(satuanKecil) Then
                            kolomSatuan.Items.Add(satuanKecil)
                        End If

                        If Not String.IsNullOrEmpty(satuanSedang) Then
                            kolomSatuan.Items.Add(satuanSedang)
                        End If

                        If Not String.IsNullOrEmpty(satuanBesar) Then
                            kolomSatuan.Items.Add(satuanBesar)
                        End If
                    End While
                End If
            End Using

        End Using



        ' Mengambil nilai dari input lainnya
        Dim hargaBeli As Decimal
        If Not Decimal.TryParse(TxtHargabeli.Text, hargaBeli) Then
            hargaBeli = 0D ' atau nilai default lainnya
        End If

        Dim qty As Decimal = 1
        Dim satuan As String = ""
        Dim isi As Decimal
        Dim hargajual As Decimal

        If TxtQtyKecil.Text <> "" AndAlso TxtQtyKecil.Text IsNot Nothing AndAlso TxtQtyKecil.Text <> "0" Then
            ' Check if TxtQtyKecil.Text is not empty, not null, and not "0"
            If Not Decimal.TryParse(TxtQtyKecil.Text, qty) Then qty = 1 ' Default to 1 if invalid
            satuan = CmbSatuanKecil.Text
            If Not Decimal.TryParse(TxtIsiKecil.Text, isi) Then isi = 0 ' Default to 0 if invalid
            If Not Decimal.TryParse(TxtHargaKecil.Text, hargajual) Then hargajual = 0 ' Default to 0 if invalid
        ElseIf TxtQtySedang.Text <> "" AndAlso TxtQtySedang.Text IsNot Nothing AndAlso TxtQtySedang.Text <> "0" Then
            ' Check if TxtQtySedang.Text is not empty, not null, and not "0"
            If Not Decimal.TryParse(TxtQtySedang.Text, qty) Then qty = 1 ' Default to 1 if invalid
            satuan = CmbSatuanSedang.Text
            If Not Decimal.TryParse(TxtIsiSedang.Text, isi) Then isi = 0 ' Default to 0 if invalid
            If Not Decimal.TryParse(TxtHargaSedang.Text, hargajual) Then hargajual = 0 ' Default to 0 if invalid
        ElseIf TxtQtyBesar.Text <> "" AndAlso TxtQtyBesar.Text IsNot Nothing AndAlso TxtQtyBesar.Text <> "0" Then
            ' Check if TxtQtyBesar.Text is not empty, not null, and not "0"
            If Not Decimal.TryParse(TxtQtyBesar.Text, qty) Then qty = 1 ' Default to 1 if invalid
            satuan = CmbSatuanBesar.Text
            If Not Decimal.TryParse(TxtIsiBesar.Text, isi) Then isi = 0 ' Default to 0 if invalid
            If Not Decimal.TryParse(TxtHargaBesar.Text, hargajual) Then hargajual = 0 ' Default to 0 if invalid
        End If

        Dim Stoktoko As Decimal = If(Decimal.TryParse(TxtStokToko.Text, Stoktoko), Stoktoko, 0)
        Dim Stokgudang As Decimal = If(Decimal.TryParse(TXtStokGudang.Text, Stokgudang), Stokgudang, 0)
        Dim Stok As Decimal = If(Decimal.TryParse(TxtStok.Text, Stok), Stok, 0)




        ' Menetapkan nilai untuk baris yang baru ditambahkan
        DgvData.Rows(indeksBaris).Cells("Kode").Value = TxtKode.Text
        DgvData.Rows(indeksBaris).Cells("NamaBarang").Value = TxtNama.Text
        DgvData.Rows(indeksBaris).Cells("HargaBeli").Value = hargaBeli
        DgvData.Rows(indeksBaris).Cells("QTY").Value = qty
        DgvData.Rows(indeksBaris).Cells("Satuan").Value = satuan
        DgvData.Rows(indeksBaris).Cells("Isi").Value = isi
        DgvData.Rows(indeksBaris).Cells("Totalhargabeli").Value = hargaBeli * isi * qty
        DgvData.Rows(indeksBaris).Cells("Harga").Value = hargajual
        DgvData.Rows(indeksBaris).Cells("QtySat").Value = qty * isi
        DgvData.Rows(indeksBaris).Cells("DiskonPersen").Value = 0
        DgvData.Rows(indeksBaris).Cells("DiskonRp").Value = 0
        DgvData.Rows(indeksBaris).Cells("TotalDiskon").Value = 0
        DgvData.Rows(indeksBaris).Cells("TotalHarga").Value = hargajual * isi * qty
        DgvData.Rows(indeksBaris).Cells("StokToko").Value = Stoktoko
        DgvData.Rows(indeksBaris).Cells("StokGudang").Value = Stokgudang
        DgvData.Rows(indeksBaris).Cells("Stok").Value = Stok
        ' Melakukan pembaruan pada ringkasan atau operasi relevan lainnya
        FormPenjualan.UpdateSemuaTotal()


    End Sub


End Class