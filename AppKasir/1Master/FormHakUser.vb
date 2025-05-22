


Public Class FormHakUser
    ' List untuk menyimpan semua label
    Private labels As List(Of Label)
    ' List untuk menyimpan semua DataGridView
    Private dgvList As List(Of DataGridView)
    ' Label yang sedang aktif
    Private activeLabel As Label = Nothing



    Private Sub FormHakUser_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        Dim HAAkses As Boolean() = ModulHakAkses.BacaHakAkses(FormUtama.SLevel.Text, "Hak Akses", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnSimpan.Visible = HAAkses(2) ' CanEdit 

        DataHakaksesuser()

        BacaCombobox()
        CheckModuleAllUser()

        CmbUser.SelectedIndex = 0

        DGVMaster.ClearSelection()

        Cursor = Cursors.Default



        ' Masukkan semua label ke dalam list
        labels = New List(Of Label) From {LblMasterData, LblTransaksi, LblJurnal, LblKaryawan, LblLaporan, LblUtility, LblPosting}

        ' Masukkan semua DataGridView ke dalam list
        dgvList = New List(Of DataGridView) From {DGVMaster, DgvTransaksi, DgvJurnal, DgvKaryawan, DgvLaporan, DgvUtility, DgvPosting}

        ' Tambahkan event handler ke setiap label
        For Each lbl As Label In labels
            AddHandler lbl.Click, AddressOf Label_Click
        Next

    End Sub

    Private Sub Label_Click(sender As Object, e As EventArgs)
        Dim clickedLabel As Label = DirectCast(sender, Label)

        ' Kembalikan warna label sebelumnya ke warna semula jika ada
        If activeLabel IsNot Nothing Then
            activeLabel.BackColor = SystemColors.Control
        End If

        ' Ubah warna label yang diklik
        clickedLabel.BackColor = Color.LightBlue

        ' Simpan label yang aktif
        activeLabel = clickedLabel

        ' Atur ulang tampilan DataGridView
        UpdateDataGridView(clickedLabel)
    End Sub

    Private Sub UpdateDataGridView(clickedLabel As Label)
        ' Matikan semua DataGridView
        For Each dgv As DataGridView In dgvList
            dgv.Visible = False
        Next

        ' Reset checkbox
        ChkAll.Checked = False
        ChkNonAll.Checked = False

        ' Tampilkan DataGridView yang sesuai
        Select Case clickedLabel.Name
            Case "LblMasterData"
                DGVMaster.Visible = True
                DGVMaster.ClearSelection()
            Case "LblTransaksi"
                DgvTransaksi.Visible = True
                DgvTransaksi.Location = New Point(123, 72)
                DgvTransaksi.ClearSelection()
            Case "LblJurnal"
                DgvJurnal.Visible = True
                DgvJurnal.Location = New Point(123, 72)
                DgvJurnal.ClearSelection()
            Case "LblKaryawan"
                DgvKaryawan.Visible = True
                DgvKaryawan.Location = New Point(123, 72)
                DgvKaryawan.ClearSelection()
            Case "LblLaporan"
                DgvLaporan.Visible = True
                DgvLaporan.Location = New Point(123, 72)
                DgvLaporan.ClearSelection()
            Case "LblUtility"
                DgvUtility.Visible = True
                DgvUtility.Location = New Point(123, 72)
                DgvUtility.ClearSelection()
            Case "LblPosting"
                DgvPosting.Visible = True
                DgvPosting.Location = New Point(123, 72)
                DgvPosting.ClearSelection()
        End Select
    End Sub

    Public Sub DataHakaksesuser()
        ' Menampilkan hanya DataGridView yang sesuai
        DGVMaster.Visible = True
        DgvTransaksi.Visible = False
        DgvJurnal.Visible = False
        DgvKaryawan.Visible = False
        DgvLaporan.Visible = False
        DgvUtility.Visible = False
        DgvPosting.Visible = False

        ' Mengisi DataGridView dengan data yang sudah disediakan
        IsiDataGridView(DGVMaster, "MASTER", {"Toko", "Barang", "Harga Beli", "Tambah Stok", "Kurang Stok",
                                          "Export Barang", "Import Barang", "Perbaiki Data Barang",
                                          "Perbaiki isi satuan", "Pelanggan", "Supplier", "Tabel Referensi",
                                          "Armada", "Karyawan", "User", "Hak Akses"})

        IsiDataGridView(DgvTransaksi, "TRANSAKSI", {"Pembelian", "Penjualan", "Retur Pembelian", "Retur Penjualan",
                                                "Bayar Hutang", "Bayar Piutang", "Transfer Stok",
                                                "Transfer Barang", "Stok Opname", "Surat Jalan"})

        IsiDataGridView(DgvJurnal, "JURNAL", {})

        IsiDataGridView(DgvKaryawan, "MENUKARYAWAN", {"MASTER GAJI", "BON", "BAYAR", "LAP BON", "LAP BON KAR",
                                                  "GAJI", "LAP GAJI"})

        IsiDataGridView(DgvLaporan, "LAPORAN", {"Mutasi saldo", "Mutasi barang", "Jurnal Umum", "Neraca", "Buku Besar",
                                            "Buku Besar Pembantu", "Lap Pembelian", "Lap Penjualan",
                                            "Jual PPnNonPPn", "Retur Beli", "Retur Jual", "Hutang", "Piutang",
                                            "Kas Penjualan", "Transfer Stok", "Transfer Barang", "Stok Opname",
                                            "Stok Barang", "Grafik", "History"}, 1)

        IsiDataGridView(DgvUtility, "UTILITY", {"Database", "Backup Database", "Restore Database", "Perbaiki Database",
                                            "Query", "Setting Printer"}, 1)

        IsiDataGridView(DgvPosting, "POSTING", {}, 1)

        CheckAndSyncModule()

    End Sub

    Private Sub IsiDataGridView(dgv As DataGridView, header As String, items() As String, Optional colCount As Integer = 4)
        ' Pastikan DataGridView memiliki cukup kolom
        If dgv.ColumnCount < colCount + 1 Then Exit Sub

        ' Atur kolom pertama sebagai ReadOnly
        dgv.Columns(0).ReadOnly = True

        ' Atur tipe kolom selain kolom pertama menjadi Boolean
        For i As Integer = 1 To colCount
            dgv.Columns(i).ValueType = GetType(Boolean)
        Next

        ' Hapus data lama sebelum mengisi
        dgv.Rows.Clear()

        ' Tambahkan header
        dgv.Rows.Add(header, False, False, False, False)

        ' Tambahkan item lainnya
        For Each item As String In items
            dgv.Rows.Add(item, False, False, False, False)
        Next
    End Sub


    Public Sub BacaCombobox()
        Dim SelectQuery As String = "SELECT Role, ModuleName FROM hakaksesuser WHERE ModuleName <> ''"
        Dim moduleDict As New Dictionary(Of String, String)()

        ' Ambil semua data dalam satu query
        Using cmd As New MySqlCommand(SelectQuery, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    moduleDict(reader("Role").ToString()) = reader("ModuleName").ToString()
                End While
            End Using
        End Using

        ' Array ComboBox dan Label
        Dim comboboxes() As ComboBox = {CmbBeliFokus, CmbBeliSatuan, CmbBeliRugi, CmbBeliMuculJual, CmbBeliUpdate, CmbBeliEditHarga, CmbBeliAverage, CmbJualFokus, CmbJualSatuan, CmbJualEditHarga, CmbJualRugi, CmbJualMinus, CmbEditHargaJual, CmbTransferFocus, CmbTransferSatuan, CmbTransferMinus, CmbReturFokus, CmbReturSatuan, CmbReturMinus}
        Dim labels() As Label = {LblBeliFokus, LblBeliSatuan, LblBeliRugi, LblBeliMuculJual, LblBeliUpdate, LblBeliEditHarga, LblBeliAverage, LblJualFokus, LblJualSatuan, LblJualEditHarga, LblJualRugi, LblJualMinus, LblEditHargaJual, LblTransferFocus, LblTransferSatuan, LblTransferMinus, LblReturFokus, LblReturSatuan, LblReturMinus}

        Dim defaultValues() As Integer = {0, 1, 1, 0, 0, 0, 2, 0, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1}
        ' Set nilai pada ComboBox
        For i As Integer = 0 To comboboxes.Length - 1
            Dim role As String = labels(i).Text
            If moduleDict.ContainsKey(role) Then
                comboboxes(i).Text = moduleDict(role)
            Else
                comboboxes(i).SelectedIndex = defaultValues(i) ' Default jika tidak ditemukan
            End If
        Next
    End Sub

    Public Sub CheckModuleAllUser()
        Dim roles() As String = {LblBeliFokus.Text, LblBeliSatuan.Text, LblBeliRugi.Text, LblBeliUpdate.Text, LblBeliMuculJual.Text, LblBeliEditHarga.Text, LblBeliAverage.Text, LblJualFokus.Text, LblJualSatuan.Text, LblJualEditHarga.Text, LblJualRugi.Text, LblJualMinus.Text, LblEditHargaJual.Text, LblTransferFocus.Text, LblTransferSatuan.Text, LblTransferMinus.Text, LblReturFokus.Text, LblReturSatuan.Text, LblReturMinus.Text}

        ' Gunakan query tunggal untuk semua role
        Dim insertQuery As String = "INSERT IGNORE INTO hakaksesuser (UserName, Role) VALUES (@UserName, @Role)"
        Using cmd As New MySqlCommand(insertQuery, conn)
            cmd.Parameters.AddWithValue("@UserName", "Semua")

            ' Tambahkan deklarasi eksplisit untuk 'role'
            For Each role As String In roles
                cmd.Parameters.Clear() ' Bersihkan parameter sebelum menambah yang baru
                cmd.Parameters.AddWithValue("@UserName", "Semua")
                cmd.Parameters.AddWithValue("@Role", role)
                cmd.ExecuteNonQuery()
            Next
        End Using
    End Sub


    Public Sub CheckAndSyncModule()
        ' Panggil fungsi untuk setiap DataGridView
        CheckAndSyncModuleMaster(DGVMaster, "ModulMasterData")
        CheckAndSyncModuleMaster(DgvTransaksi, "ModulTransaksi")
        CheckAndSyncModuleMaster(DgvJurnal, "ModulJurnal")
        CheckAndSyncModuleMaster(DgvKaryawan, "ModulKaryawan")
        CheckAndSyncModuleMaster(DgvLaporan, "ModulLaporan")
        CheckAndSyncModuleMaster(DgvUtility, "ModulUtility")
        CheckAndSyncModuleMaster(DgvPosting, "ModulPosting")
    End Sub

    Public Sub CheckAndSyncModuleMaster(ByVal dgv As DataGridView, ByVal moduleNameColumn As String)
        Dim existingModules As New HashSet(Of String)

        ' 1️⃣ Ambil semua data dari database untuk mendeteksi duplikasi
        Dim fetchQuery As String = "SELECT CONCAT(UserName, '_', ModuleName) AS UniqueKey FROM hakaksesuser"
        Using cmd As New MySqlCommand(fetchQuery, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    existingModules.Add(reader("UniqueKey").ToString())
                End While
            End Using
        End Using

        Dim deleteDuplicatesQuery As String = "
    DELETE hakaksesuser FROM hakaksesuser
    JOIN (
        SELECT MIN(NO) AS KeepNO, UserName, ModuleName
        FROM hakaksesuser
        WHERE UserName <> 'Semua' -- Batasi UserName tidak boleh 'Semua'
        GROUP BY UserName, ModuleName
    ) AS KeepRows
    ON hakaksesuser.UserName = KeepRows.UserName 
    AND hakaksesuser.ModuleName = KeepRows.ModuleName
    WHERE hakaksesuser.NO > KeepRows.KeepNO
    AND hakaksesuser.UserName <> 'Semua'" '-- Pastikan hanya baris duplikat yang bukan 'Semua' dihapus

        Using deleteCmd As New MySqlCommand(deleteDuplicatesQuery, conn)
            deleteCmd.ExecuteNonQuery()
        End Using


        ' 3️⃣ Tambahkan Data Baru Jika Belum Ada
        For Each level As String In {"Master", "Admin", "Kasir", "Gudang"}
            Dim defaultValue As Integer = If(level = "Master", 1, 0)

            For Each row As DataGridViewRow In dgv.Rows
                If row.IsNewRow Then Continue For ' Hindari baris kosong

                Dim moduleName As String = row.Cells(moduleNameColumn).Value?.ToString()
                If String.IsNullOrWhiteSpace(moduleName) Then Continue For

                Dim uniqueKey As String = $"{level}_{moduleName}"

                ' Jika kombinasi UserName dan ModuleName belum ada, tambahkan
                If Not existingModules.Contains(uniqueKey) Then
                    Dim insertQuery As String = "INSERT INTO hakaksesuser (UserName, Role, ModuleName, CanRead, CanAdd, CanEdit, CanDelete) 
                                             VALUES (@UserName, @Role, @ModuleName, @CanRead, @CanAdd, @CanEdit, @CanDelete)"
                    Using insertCmd As New MySqlCommand(insertQuery, conn)
                        insertCmd.Parameters.AddWithValue("@UserName", level)
                        insertCmd.Parameters.AddWithValue("@Role", LblMasterData.Text)
                        insertCmd.Parameters.AddWithValue("@ModuleName", moduleName)

                        If moduleNameColumn = "ModulTransaksi" Then
                            insertCmd.Parameters.AddWithValue("@CanRead", 1)
                            insertCmd.Parameters.AddWithValue("@CanAdd", 1)
                        Else
                            insertCmd.Parameters.AddWithValue("@CanRead", defaultValue)
                            insertCmd.Parameters.AddWithValue("@CanAdd", defaultValue)
                        End If

                        insertCmd.Parameters.AddWithValue("@CanEdit", defaultValue)
                        insertCmd.Parameters.AddWithValue("@CanDelete", defaultValue)

                        insertCmd.ExecuteNonQuery()
                    End Using
                End If
            Next
        Next
    End Sub


    Private Sub DGVMaster_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles DGVMaster.CellPainting
        'Cek apakah kolom saat ini adalah kolom dengan checkbox
        If e.ColumnIndex = 2 AndAlso e.RowIndex >= 0 Then
            'Periksa apakah ini adalah baris "Hak Akses" atau "Barang"
            If DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Kurang Stok" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Tambah Stok" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Perbaiki Data Barang" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Perbaiki isi satuan" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Toko" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Harga Beli" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Hak Akses" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "MASTER" Then
                'Sembunyikan CheckBox
                e.PaintBackground(e.CellBounds, True)
                e.Handled = True
            End If
        End If


        'Cek apakah kolom saat ini adalah kolom dengan checkbox
        If e.ColumnIndex = 3 AndAlso e.RowIndex >= 0 Then
            'Periksa apakah ini adalah baris "Hak Akses" atau "Barang"
            If DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Harga Beli" OrElse
                DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "MASTER" Then
                'Sembunyikan CheckBox
                e.PaintBackground(e.CellBounds, True)
                e.Handled = True
            End If
        End If

        'Cek apakah kolom saat ini adalah kolom dengan checkbox
        If e.ColumnIndex = 4 AndAlso e.RowIndex >= 0 Then
            'Periksa apakah ini adalah baris "Hak Akses" atau "Barang"
            If DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Kurang Stok" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Tambah Stok" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Perbaiki Data Barang" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Perbaiki isi satuan" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Hak Akses" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "Harga Beli" OrElse
               DGVMaster.Rows(e.RowIndex).Cells(0).Value.ToString() = "MASTER" Then
                'Sembunyikan CheckBox
                e.PaintBackground(e.CellBounds, True)
                e.Handled = True
            End If
        End If

    End Sub

    Private Sub DgvTransaksi_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles DgvTransaksi.CellPainting
        ' Cek apakah kolom saat ini adalah kolom dengan checkbox
        If (e.ColumnIndex = 2 Or e.ColumnIndex = 3 Or e.ColumnIndex = 4) AndAlso e.RowIndex >= 0 Then
            ' Periksa apakah ini adalah baris "TRANSAKSI"
            If DgvTransaksi.Rows(e.RowIndex).Cells(0).Value.ToString() = "TRANSAKSI" Then
                ' Sembunyikan checkbox
                e.PaintBackground(e.CellBounds, True)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub DgvKaryawan_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles DgvKaryawan.CellPainting
        ' Cek apakah kolom saat ini adalah kolom dengan checkbox
        If (e.ColumnIndex = 2 Or e.ColumnIndex = 3 Or e.ColumnIndex = 4) AndAlso e.RowIndex >= 0 Then
            ' Periksa apakah ini adalah baris "TRANSAKSI"
            If DgvKaryawan.Rows(e.RowIndex).Cells(0).Value.ToString() = "MENUKARYAWAN" OrElse
               DgvKaryawan.Rows(e.RowIndex).Cells(0).Value.ToString() = "LAP BON" OrElse
               DgvKaryawan.Rows(e.RowIndex).Cells(0).Value.ToString() = "LAP GAJI" Then
                ' Sembunyikan checkbox
                e.PaintBackground(e.CellBounds, True)
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub Dgvposting_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles DgvPosting.CellPainting
        ' Cek apakah kolom saat ini adalah kolom dengan checkbox
        If (e.ColumnIndex = 2 Or e.ColumnIndex = 3 Or e.ColumnIndex = 4) AndAlso e.RowIndex >= 0 Then
            ' Periksa apakah ini adalah baris "posting"
            If DgvPosting.Rows(e.RowIndex).Cells(0).Value.ToString() = "POSTING" Then
                ' Sembunyikan checkbox
                e.PaintBackground(e.CellBounds, True)
                e.Handled = True
            End If
        End If
    End Sub


    Private Sub BtnKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKeluar.Click
        Close()
    End Sub

    Private Sub CmbUser_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbUser.SelectedIndexChanged
        ChkAll.Checked = False
        ChkNonAll.Checked = False

        ' Query utama untuk semua DataGridView kecuali laporan & posting
        Dim query As String = "SELECT CanRead, CanAdd, CanEdit, CanDelete FROM hakaksesuser WHERE UserName = @UserName AND ModuleName = @ModuleName"

        ' Query khusus untuk DataGridView yang hanya memiliki akses "Read"
        Dim queryBaca As String = "SELECT CanRead FROM hakaksesuser WHERE UserName = @UserName AND ModuleName = @ModuleName"

        ' Memproses semua DataGridView
        UpdateDataGridViewAkses(DGVMaster, query, True)
        UpdateDataGridViewAkses(DgvTransaksi, query, True)
        UpdateDataGridViewAkses(DgvJurnal, query, True)
        UpdateDataGridViewAkses(DgvKaryawan, query, True)
        UpdateDataGridViewAkses(DgvPosting, queryBaca, False) ' Hanya CanRead
        UpdateDataGridViewAkses(DgvLaporan, queryBaca, False) ' Hanya CanRead
        UpdateDataGridViewAkses(DgvUtility, queryBaca, False) ' Hanya CanRead
    End Sub

    ' Fungsi untuk memperbarui DataGridView berdasarkan hak akses
    Private Sub UpdateDataGridViewAkses(dgv As DataGridView, query As String, hasFullAccess As Boolean)
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@UserName", CmbUser.Text)

            For Each row As DataGridViewRow In dgv.Rows
                If Not row.IsNewRow Then
                    Dim modul As String = row.Cells(0).Value.ToString()
                    cmd.Parameters.AddWithValue("@ModuleName", modul)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ' Mengisi DataGridView berdasarkan jumlah kolom
                            row.Cells(1).Value = reader("CanRead")
                            If hasFullAccess AndAlso dgv.ColumnCount >= 5 Then
                                row.Cells(2).Value = reader("CanAdd")
                                row.Cells(3).Value = reader("CanEdit")
                                row.Cells(4).Value = reader("CanDelete")
                            End If
                        End If
                    End Using

                    ' Bersihkan parameter untuk iterasi berikutnya
                    cmd.Parameters.RemoveAt("@ModuleName")
                End If
            Next
        End Using
    End Sub


    ' Ketika ChkAll dicentang, semua hak akses diaktifkan
    Private Sub ChkAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkAll.CheckedChanged
        If ChkAll.Checked Then
            ChkNonAll.Checked = False
            SetAksesSemuaDataGridView(True)
        End If
    End Sub

    ' Ketika ChkNonAll dicentang, semua hak akses dinonaktifkan
    Private Sub ChkNonAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkNonAll.CheckedChanged
        If ChkNonAll.Checked Then
            ChkAll.Checked = False
            SetAksesSemuaDataGridView(False)
        End If
    End Sub

    ' Fungsi untuk mengatur akses semua DataGridView
    Private Sub SetAksesSemuaDataGridView(status As Boolean)
        ' Daftar semua DataGridView yang memiliki hak akses penuh (CanRead, CanAdd, CanEdit, CanDelete)
        Dim daftarDGV() As DataGridView = {DGVMaster, DgvTransaksi, DgvJurnal, DgvKaryawan}

        ' Looping untuk DataGridView yang memiliki akses penuh
        For Each dgv As DataGridView In daftarDGV
            If dgv.Visible Then
                SetAksesDataGridView(dgv, status, 4)
            End If
        Next

        ' DataGridView yang hanya memiliki akses "Read" (1 kolom hak akses)
        Dim daftarDGVReadOnly() As DataGridView = {DgvLaporan, DgvUtility, DgvPosting}
        For Each dgv As DataGridView In daftarDGVReadOnly
            If dgv.Visible Then
                SetAksesDataGridView(dgv, status, 1)
            End If
        Next
    End Sub

    ' Fungsi untuk mengubah hak akses dalam satu DataGridView
    Private Sub SetAksesDataGridView(dgv As DataGridView, status As Boolean, colCount As Integer)
        For Each row As DataGridViewRow In dgv.Rows
            If Not row.IsNewRow Then
                For i As Integer = 1 To colCount
                    row.Cells(i).Value = status
                Next
            End If
        Next
    End Sub

    Private Sub CmbUser_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles CmbUser.MouseMove
        ToolTip1.SetToolTip(CmbUser, "Sebelum ganti user klik simpan dulu")
    End Sub


    Private Sub FormHakUser_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                BtnSimpan.PerformClick()
            Case Keys.Escape
                BtnKeluar.PerformClick()
        End Select
    End Sub

    Private Sub BtnSimpan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpan.Click
        Dim transaksi As MySqlTransaction = Nothing
        Try
            transaksi = conn.BeginTransaction() ' Mulai transaksi

            UpdateHakAksesUser(transaksi)

            ' Gunakan Dictionary untuk mengelola DataGridView dan jumlah kolom yang digunakan
            Dim daftarGrid As New Dictionary(Of DataGridView, Integer()) From {
            {DGVMaster, {1, 2, 3, 4}},
            {DgvTransaksi, {1, 2, 3, 4}},
            {DgvJurnal, {1, 2, 3, 4}},
            {DgvKaryawan, {1, 2, 3, 4}},
            {DgvLaporan, {1}}, ' Hanya CanRead
            {DgvUtility, {1}}, ' Hanya CanRead
            {DgvPosting, {1}}  ' Hanya CanRead
        }

            For Each pair As KeyValuePair(Of DataGridView, Integer()) In daftarGrid
                UpdateHakAkses(pair.Key, pair.Value, transaksi)
            Next


            ' Commit transaksi jika tidak ada kesalahan
            transaksi.Commit()
            MessageBox.Show("Perubahan telah disimpan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            DatabaseModule.CatatanAksiHistory("Update hak akses user")
        Catch ex As Exception
            transaksi.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub UpdateHakAksesUser(transaksi As MySqlTransaction)
        Using cmd As New MySqlCommand("UPDATE hakaksesuser SET ModuleName = @ModuleName WHERE Role = @Role", conn, transaksi)
            cmd.Parameters.Add("@ModuleName", MySqlDbType.VarChar)
            cmd.Parameters.Add("@Role", MySqlDbType.VarChar)
            cmd.Prepare()

            ' List pasangan ComboBox dan Label
            Dim updatePairs As New List(Of Tuple(Of ComboBox, Label)) From {
            New Tuple(Of ComboBox, Label)(CmbBeliFokus, LblBeliFokus),
            New Tuple(Of ComboBox, Label)(CmbBeliSatuan, LblBeliSatuan),
            New Tuple(Of ComboBox, Label)(CmbBeliRugi, LblBeliRugi),
            New Tuple(Of ComboBox, Label)(CmbBeliMuculJual, LblBeliMuculJual),
            New Tuple(Of ComboBox, Label)(CmbBeliUpdate, LblBeliUpdate),
            New Tuple(Of ComboBox, Label)(CmbBeliEditHarga, LblBeliEditHarga),
            New Tuple(Of ComboBox, Label)(CmbBeliAverage, LblBeliAverage),
            New Tuple(Of ComboBox, Label)(CmbJualFokus, LblJualFokus),
            New Tuple(Of ComboBox, Label)(CmbJualSatuan, LblJualSatuan),
            New Tuple(Of ComboBox, Label)(CmbJualEditHarga, LblJualEditHarga),
            New Tuple(Of ComboBox, Label)(CmbJualRugi, LblJualRugi),
            New Tuple(Of ComboBox, Label)(CmbJualMinus, LblJualMinus),
            New Tuple(Of ComboBox, Label)(CmbEditHargaJual, LblEditHargaJual),
            New Tuple(Of ComboBox, Label)(CmbTransferFocus, LblTransferFocus),
            New Tuple(Of ComboBox, Label)(CmbTransferSatuan, LblTransferSatuan),
            New Tuple(Of ComboBox, Label)(CmbTransferMinus, LblTransferMinus),
            New Tuple(Of ComboBox, Label)(CmbReturFokus, LblReturFokus),
            New Tuple(Of ComboBox, Label)(CmbReturSatuan, LblReturSatuan),
            New Tuple(Of ComboBox, Label)(CmbReturMinus, LblReturMinus)
        }

            ' Loop untuk update data
            For Each pair As Tuple(Of ComboBox, Label) In updatePairs
                Dim moduleName As String = pair.Item1.Text.Trim()
                Dim roleName As String = pair.Item2.Text.Trim()

                ' Hanya update jika moduleName tidak kosong
                If Not String.IsNullOrEmpty(moduleName) AndAlso Not String.IsNullOrEmpty(roleName) Then
                    cmd.Parameters("@ModuleName").Value = moduleName
                    cmd.Parameters("@Role").Value = roleName
                    cmd.ExecuteNonQuery()
                End If
            Next
        End Using
    End Sub




    ' Fungsi untuk memperbarui hak akses dari DataGridView
    Private Sub UpdateHakAkses(grid As DataGridView, colIndex As Integer(), transaksi As MySqlTransaction)
        If Not grid.Visible Then Exit Sub

        For Each row As DataGridViewRow In grid.Rows
            If Not row.IsNewRow Then
                Dim modul As String = row.Cells(0).Value.ToString()
                Dim query As String

                If colIndex.Length = 1 Then
                    ' Untuk DgvLaporan, DgvUtility, dan DgvPosting yang hanya memiliki CanRead
                    query = "UPDATE hakaksesuser SET CanRead = @CanRead WHERE UserName = @UserName AND ModuleName = @ModuleName"
                Else
                    ' Untuk DGVMaster, DgvTransaksi, DgvJurnal, DgvKaryawan
                    query = "UPDATE hakaksesuser SET CanRead = @CanRead, CanAdd = @CanAdd, CanEdit = @CanEdit, CanDelete = @CanDelete WHERE UserName = @UserName AND ModuleName = @ModuleName"
                End If

                Using cmd As New MySqlCommand(query, conn, transaksi)
                    cmd.Parameters.AddWithValue("@CanRead", row.Cells(colIndex(0)).Value)
                    cmd.Parameters.AddWithValue("@UserName", CmbUser.Text)
                    cmd.Parameters.AddWithValue("@ModuleName", modul)

                    If colIndex.Length > 1 Then
                        cmd.Parameters.AddWithValue("@CanAdd", row.Cells(colIndex(1)).Value)
                        cmd.Parameters.AddWithValue("@CanEdit", row.Cells(colIndex(2)).Value)
                        cmd.Parameters.AddWithValue("@CanDelete", row.Cells(colIndex(3)).Value)
                    End If

                    cmd.ExecuteNonQuery()
                End Using
            End If
        Next
    End Sub


End Class
