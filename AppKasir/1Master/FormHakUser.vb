


Public Class FormHakUser
    ' List untuk menyimpan semua label
    Private labels As List(Of Label)
    ' List untuk menyimpan semua DataGridView
    Private dgvList As List(Of DataGridView)
    ' Label yang sedang aktif
    Private activeLabel As Label = Nothing



    Private Sub FormHakUser_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor

        Dim HAAkses As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Hak Akses")
        BtnSimpan.Visible = HAAkses(2)

        labels = New List(Of Label) From {LblMasterData, LblTransaksi, LblJurnal, LblKaryawan, LblLaporan, LblUtility, LblPosting}
        dgvList = New List(Of DataGridView) From {DGVMaster, DgvTransaksi, DgvJurnal, DgvKaryawan, DgvLaporan, DgvUtility, DgvPosting}

        For Each lbl As Label In labels
            AddHandler lbl.Click, AddressOf Label_Click
        Next

        ' Isi DataGridView dengan data template terlebih dahulu
        IsiDataGridViewTemplate()

        ' Sinkronkan database dengan template DataGridView
        'SinkronkanDatabaseDenganTemplate()

        If CmbUser.Items.Count > 0 Then
            CmbUser.SelectedIndex = 1
        End If

        Label_Click(LblMasterData, EventArgs.Empty)
        Me.Cursor = Cursors.Default
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

    Public Sub IsiDataGridViewTemplate()
        ' Menampilkan hanya DataGridView yang sesuai
        DGVMaster.Visible = True
        DgvTransaksi.Visible = False
        DgvJurnal.Visible = False
        DgvKaryawan.Visible = False
        DgvLaporan.Visible = False
        DgvUtility.Visible = False
        DgvPosting.Visible = False

        ' Mengisi DataGridView dengan data template
        IsiDataGridView(DGVMaster, "MASTER", {"Toko", "Barang", "Harga Beli", "Tambah Stok", "Kurang Stok",
                                          "Export Barang", "Import Barang", "Perbaiki Data Barang",
                                          "Perbaiki isi satuan", "Pelanggan", "Supplier", "Tabel Referensi",
                                          "Armada", "Karyawan", "User", "Hak Akses", "Cabang Master"})

        IsiDataGridView(DgvTransaksi, "TRANSAKSI", {"Pembelian", "Penjualan", "Retur Pembelian", "Retur Penjualan",
                                                "Bayar Hutang", "Bayar Piutang", "Transfer Stok",
                                                "Transfer Barang", "Stok Opname", "Surat Jalan", "Transfer Cabang", "Sales Order"})

        IsiDataGridView(DgvJurnal, "JURNAL", {})

        IsiDataGridView(DgvKaryawan, "MENUKARYAWAN", {"Master gaji", "Bon", "Bayar", "Lap bon", "Lap bon karyawan",
                                                  "Gaji", "Lap Gaji"})

        IsiDataGridView(DgvLaporan, "LAPORAN", {"Mutasi saldo", "Mutasi barang", "Jurnal Umum", "Neraca", "Buku Besar",
                                            "Buku Besar Pembantu",
                                            "Lap Pembelian", "Lap Pembelian Detail", "Lap Pembelian Barang", "Lap Pembelian Hutang",
                                            "Rekap Penjualan Nota", "Rekap Penjualan Barang",
                                            "Lap Penjualan", "Lap Penjualan Detail", "Lap Penjualan Barang",
                                            "Lap Penjualan Hutang", "Lap Penjualan Sales", "Lap Penjualan Qty", "Jual PPnNonPPn",
                                            "Retur Beli", "Retur Beli Detail", "Retur Beli Barang",
                                            "Retur Jual", "Retur Jual Detail", "Retur Jual Barang",
                                            "Hutang By Pembelian", "Hutang By Pelunasan", "Hutang By Jatuh Tempo", "Rekap Bayar Hutang",
                                            "Piutang By Penjualan", "Piutang By Pelunasan", "Piutang By Jatuh Tempo", "Rekap Bayar Piutang",
                                            "Kas Penjualan",
                                            "Lap Transfer Stok", "Lap Transfer Barang", "Lap Transfer Barang Detail",
                                            "Lap Stok Opname", "Stok Barang", "Kartu Stok",
                                            "Barang Terlaris", "Barang Tidak Bergerak", "Stok Minimum", "Stok Masa Lampau",
                                            "Ranking Supplier", "Ranking Kasir", "Ranking Barang Terbanyak Dibeli",
                                            "Ranking Pelanggan Piutang Terbesar", "Ranking Supplier Hutang Terbesar",
                                            "Omset Per Pelanggan", "Omset Per Kategori",
                                            "Grafik", "History"}, 1)

        IsiDataGridView(DgvUtility, "UTILITY", {"Database", "Backup Database", "Restore Database", "Perbaiki Database",
                                            "Setting Printer"}, 1)

        IsiDataGridView(DgvPosting, "POSTING", {"Posting Toko", "Posting Gudang", "Posting Semua"}, 1)

        SinkronkanDatabaseDenganTemplate()
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



    Public Sub SinkronkanDatabaseDenganTemplate()
        Try
            ' Hapus duplikasi data yang mungkin ada
            HapusDuplikasiDatabase()

            ' Sinkronkan setiap DataGridView
            SinkronkanDataGridView(DGVMaster, "MASTER")
            SinkronkanDataGridView(DgvTransaksi, "TRANSAKSI")
            SinkronkanDataGridView(DgvJurnal, "JURNAL")
            SinkronkanDataGridView(DgvKaryawan, "MENUKARYAWAN")
            SinkronkanDataGridView(DgvLaporan, "LAPORAN")
            SinkronkanDataGridView(DgvUtility, "UTILITY")
            SinkronkanDataGridView(DgvPosting, "POSTING")

        Catch ex As Exception
            MessageBox.Show("Error sinkronasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub HapusDuplikasiDatabase()
        Dim deleteDuplicatesQuery As String = "
            DELETE hakaksesuser FROM hakaksesuser
            JOIN (
                SELECT MIN(NO) AS KeepNO, UserName, ModuleName
                FROM hakaksesuser
                WHERE UserName <> 'Semua'
                GROUP BY UserName, ModuleName
            ) AS KeepRows
            ON hakaksesuser.UserName = KeepRows.UserName 
            AND hakaksesuser.ModuleName = KeepRows.ModuleName
            WHERE hakaksesuser.NO > KeepRows.KeepNO
            AND hakaksesuser.UserName <> 'Semua'"

        Using deleteCmd As New MySqlCommand(deleteDuplicatesQuery, conn)
            deleteCmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub SinkronkanDataGridView(dgv As DataGridView, roleCategory As String)
        Dim daftarLevel() As String = {"Owner", "Master", "Admin", "Kasir", "Gudang"}

        For Each level As String In daftarLevel
            ' 1. Ambil daftar modul yang ada di database untuk level ini
            Dim modulDiDatabase As New HashSet(Of String)
            Dim queryGetModul As String = "SELECT ModuleName FROM hakaksesuser WHERE UserName = @UserName AND Role = @Role"
            Using cmd As New MySqlCommand(queryGetModul, conn)
                cmd.Parameters.AddWithValue("@UserName", level)
                cmd.Parameters.AddWithValue("@Role", roleCategory)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        modulDiDatabase.Add(reader("ModuleName").ToString())
                    End While
                End Using
            End Using

            ' 2. Ambil daftar modul yang ada di DataGridView
            Dim modulDiDataGridView As New HashSet(Of String)
            For Each row As DataGridViewRow In dgv.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing Then
                    Dim moduleName As String = row.Cells(0).Value.ToString()
                    If Not String.IsNullOrWhiteSpace(moduleName) Then
                        modulDiDataGridView.Add(moduleName)
                    End If
                End If
            Next

            ' 3. Hapus modul dari database yang tidak ada di DataGridView
            For Each modulDB As String In modulDiDatabase
                If Not modulDiDataGridView.Contains(modulDB) Then
                    Dim deleteQuery As String = "DELETE FROM hakaksesuser WHERE UserName = @UserName AND ModuleName = @ModuleName AND Role = @Role"
                    Using deleteCmd As New MySqlCommand(deleteQuery, conn)
                        deleteCmd.Parameters.AddWithValue("@UserName", level)
                        deleteCmd.Parameters.AddWithValue("@ModuleName", modulDB)
                        deleteCmd.Parameters.AddWithValue("@Role", roleCategory)
                        deleteCmd.ExecuteNonQuery()
                    End Using
                End If
            Next

            ' 4. Tambah modul ke database yang ada di DataGridView tapi belum ada di database
            For Each modulDGV As String In modulDiDataGridView
                If Not modulDiDatabase.Contains(modulDGV) Then
                    Dim defaultValue As Integer = If(level = "Master" OrElse level = "Owner", 1, 0)

                    Dim insertQuery As String = "INSERT INTO hakaksesuser (UserName, Role, ModuleName, CanRead, CanAdd, CanEdit, CanDelete) 
                                               VALUES (@UserName, @Role, @ModuleName, @CanRead, @CanAdd, @CanEdit, @CanDelete)"
                    Using insertCmd As New MySqlCommand(insertQuery, conn)
                        insertCmd.Parameters.AddWithValue("@UserName", level)
                        insertCmd.Parameters.AddWithValue("@Role", roleCategory)
                        insertCmd.Parameters.AddWithValue("@ModuleName", modulDGV)

                        ' Untuk kategori TRANSAKSI, berikan akses Read dan Add secara default
                        If roleCategory = "TRANSAKSI" Then
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

    ' Event handlers untuk CellPainting tetap sama seperti sebelumnya
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
        SinkronkanModulYangBelumAda()
        Ambildatadaridatabase()
    End Sub

    Private Sub SinkronkanModulYangBelumAda()
        Dim daftarLevel() As String = {"Owner", "Master", "Admin", "Kasir", "Gudang"}
        Dim daftarGrid As New List(Of DataGridView) From {DGVMaster, DgvTransaksi, DgvJurnal, DgvKaryawan, DgvLaporan, DgvUtility, DgvPosting}

        For Each level As String In daftarLevel
            Dim existingModules As New HashSet(Of String)
            Dim query As String = "SELECT ModuleName FROM hakaksesuser WHERE UserName = @UserName"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@UserName", level)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        existingModules.Add(reader("ModuleName").ToString())
                    End While
                End Using
            End Using

            For Each dgv As DataGridView In daftarGrid
                For Each row As DataGridViewRow In dgv.Rows
                    If row.IsNewRow Then Continue For
                    Dim modul As String = row.Cells(0).Value?.ToString()
                    If String.IsNullOrWhiteSpace(modul) Then Continue For
                    If Not existingModules.Contains(modul) Then
                        Dim defVal As Integer = If(level = "Master", 1, 0)
                        Dim insertQuery As String = "INSERT INTO hakaksesuser (UserName, Role, ModuleName, CanRead, CanAdd, CanEdit, CanDelete) " &
                                                   "VALUES (@UserName, @Role, @ModuleName, @CanRead, @CanAdd, @CanEdit, @CanDelete)"
                        Using insertCmd As New MySqlCommand(insertQuery, conn)
                            insertCmd.Parameters.AddWithValue("@UserName", level)
                            insertCmd.Parameters.AddWithValue("@Role", LblMasterData.Text)
                            insertCmd.Parameters.AddWithValue("@ModuleName", modul)
                            insertCmd.Parameters.AddWithValue("@CanRead", defVal)
                            insertCmd.Parameters.AddWithValue("@CanAdd", defVal)
                            insertCmd.Parameters.AddWithValue("@CanEdit", defVal)
                            insertCmd.Parameters.AddWithValue("@CanDelete", defVal)
                            insertCmd.ExecuteNonQuery()
                        End Using
                    End If
                Next
            Next
        Next
    End Sub

    Private Sub Ambildatadaridatabase()
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

            ' ========================================
            ' START: Audit Trail - Ubah Hak Akses User
            ' ========================================
            Dim namaUserHak As String = CmbUser.Text
            Dim sbSnapshot As New System.Text.StringBuilder()
            Dim jumlahModul As Integer = 0
            sbSnapshot.AppendLine($"User: {namaUserHak}")
            sbSnapshot.AppendLine($"Daftar Modul:")
            For Each pair As KeyValuePair(Of DataGridView, Integer()) In daftarGrid
                For Each row As DataGridViewRow In pair.Key.Rows
                    If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing Then
                        jumlahModul += 1
                        Dim modulName As String = row.Cells(0).Value.ToString()
                        sbSnapshot.AppendLine($"  - {modulName}")
                    End If
                Next
            Next
            sbSnapshot.Insert(0, $"Jumlah Modul Diubah: {jumlahModul}" & vbNewLine)
            ModuleAuditTrail.CatatAuditMaster("USER:" & namaUserHak, "EDIT", "Hak Akses User", sbSnapshot.ToString(), trans:=transaksi)
            ' ========================================
            ' END: Audit Trail - Ubah Hak Akses User
            ' ========================================

            For Each pair As KeyValuePair(Of DataGridView, Integer()) In daftarGrid
                UpdateHakAkses(pair.Key, pair.Value, transaksi)
            Next


            ' Commit transaksi jika tidak ada kesalahan
            transaksi.Commit()
            MessageBox.Show("Perubahan telah disimpan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' === REFRESH CACHE SETELAH UPDATE ===
            ModulHakAkses.RefreshHakAksesCache()

        Catch ex As Exception
            transaksi.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub





    ' Fungsi untuk memperbarui hak akses dari DataGridView
    Private Sub UpdateHakAkses(grid As DataGridView, colIndex As Integer(), transaksi As MySqlTransaction)
        'If Not grid.Visible Then Exit Sub

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
