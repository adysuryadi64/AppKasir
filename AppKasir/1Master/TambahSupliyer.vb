Class TambahSupliyer

    Private _isLoading As Boolean = False
    Private _isEditMode As Boolean = False
    Private _canAdd As Boolean = True
    Private _canEdit As Boolean = True
    Private _canDelete As Boolean = True
    Private _toolTip As New ToolTip()

#Region "Form Load"
    Private Sub TambahSupliyer_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        Me.Cursor = Cursors.WaitCursor
        Dim hakAkses As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Supplier")
        _canAdd = hakAkses(1)
        _canEdit = hakAkses(2)
        _canDelete = hakAkses(3)

        BTNSimpan.Visible = _canAdd

        Kondisiawal()
        Me.Cursor = Cursors.Default
    End Sub
#End Region

#Region "Kondisi Awal"
    Private Sub Kondisiawal()
        _isEditMode = False
        LblHeader.Text = "TAMBAH SUPPLIER"
        BTNSimpan.Text = "SIMPAN (F2)"
        BTNSimpan.Visible = _canAdd
        TxtKode.Clear()
        TxtNama.Clear()
        TxtAlamat.Clear()
        TxtTelp.Clear()
        TxtJAngkaHutang.Text = "0"
        TxtAwal.Text = "0"
        ' Task 9.1b — pastikan TxtAwal tidak terkunci saat mode tambah baru
        TxtAwal.ReadOnly = False
        ' TxtAwal.BackColor akan diatur otomatis oleh ModuleTheme (TextBox aktif = putih+hitam)
        _toolTip.SetToolTip(TxtAwal, "")
        UpdateSupliyerFromPembelianHutangDibayar()
        Kodesupliyer()
        Tampilsupliyer()
        Dgvdata.ClearSelection()
        TxtNama.Select()
    End Sub
#End Region

#Region "DataGridView"
    Public Sub Tampilsupliyer()
        _isLoading = True

        Dim dt As New DataTable()
        Using cmd As New MySqlCommand(
            "SELECT Kode, Nama, Alamat, Hp, JangkaHutang, HutangAwal, HutangAkhir, Status FROM tbl_supliyer ORDER BY Nama", conn),
              da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        Dgvdata.DataSource = Nothing
        Dgvdata.Rows.Clear()
        Dgvdata.Columns.Clear()

        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColKode", .HeaderText = "Kode", .FillWeight = 70, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColNama", .HeaderText = "Nama Supplier", .FillWeight = 160, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColAlamat", .HeaderText = "Alamat", .FillWeight = 160, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColHp", .HeaderText = "No. HP", .FillWeight = 90, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColJangka", .HeaderText = "Jangka", .FillWeight = 55, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColHutangAwal", .HeaderText = "Hutang Awal", .FillWeight = 90, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColHutangAkhir", .HeaderText = "Hutang Akhir", .FillWeight = 90, .ReadOnly = True})
        Dgvdata.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ColStatus", .HeaderText = "Status", .FillWeight = 70, .ReadOnly = True})

        ModuleAngka.TerapkanFormatKolomAngka(Dgvdata, "ColJangka", "ColHutangAwal", "ColHutangAkhir")

        If _canEdit Then
            Dim colEdit As New DataGridViewButtonColumn() With {
                .Name = "ColEdit", .HeaderText = "Edit",
                .Text = "✎ Edit", .UseColumnTextForButtonValue = True,
                .Width = 70, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            ' Font diatur oleh tema - Calibri 9.75 Bold
            Dgvdata.Columns.Add(colEdit)
        End If

        If _canDelete Then
            Dim colNonaktif As New DataGridViewButtonColumn() With {
                .Name = "ColNonaktif", .HeaderText = "Nonaktif",
                .UseColumnTextForButtonValue = False,
                .Width = 100, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            ' Font diatur oleh tema - Calibri 9.75 Bold
            Dgvdata.Columns.Add(colNonaktif)

            Dim colHapus As New DataGridViewButtonColumn() With {
                .Name = "ColHapus", .HeaderText = "Hapus",
                .Text = "✖ Hapus", .UseColumnTextForButtonValue = True,
                .Width = 75, .FlatStyle = FlatStyle.Flat,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            ' Font diatur oleh tema - Calibri 9.75 Bold
            Dgvdata.Columns.Add(colHapus)
        End If

        For Each row As DataRow In dt.Rows
            Dim status As String = If(row("Status") Is DBNull.Value, "Aktif", row("Status").ToString())
            Dim hutang As Decimal = If(row("HutangAkhir") Is DBNull.Value, 0D, Convert.ToDecimal(row("HutangAkhir")))
            Dim rowIdx As Integer = Dgvdata.Rows.Add(
                row("Kode"), row("Nama"), row("Alamat"), row("Hp"),
                row("JangkaHutang"), row("HutangAwal"), row("HutangAkhir"), status)

            ' Warna baris nonaktif
            ModuleTheme.SetWarnaBarisDgvNonaktif(Dgvdata.Rows(rowIdx), status = "Nonaktif")

            If _canDelete Then
                ' Tombol Nonaktif/Aktifkan
                If status = "Aktif" Then
                    Dgvdata.Rows(rowIdx).Cells("ColNonaktif").Value = "⊘ Nonaktifkan"
                Else
                    Dgvdata.Rows(rowIdx).Cells("ColNonaktif").Value = "✔ Aktifkan"
                End If
                ModuleTheme.SetWarnaDgvBtnStatus(Dgvdata.Rows(rowIdx).Cells("ColNonaktif"), status = "Aktif")

                ' Tombol Hapus — merah jika bisa hapus, abu-abu jika tidak
                ModuleTheme.SetWarnaDgvBtnHapus(Dgvdata.Rows(rowIdx).Cells("ColHapus"), hutang = 0 AndAlso status <> "Nonaktif")
            End If

            ' Tombol Edit — ikut warna status
            If _canEdit Then
                ModuleTheme.SetWarnaDgvBtnEdit(Dgvdata.Rows(rowIdx).Cells("ColEdit"), status <> "Nonaktif")
            End If
        Next

        ' Pengaturan standar dan tema DGV
        ModuleTheme.ApplyStandardDataGridViewSettings(Dgvdata)
        ModuleTheme.ApplyThemeDataGridView(Dgvdata)

        Dgvdata.ClearSelection()
        _isLoading = False
    End Sub

    Private Sub Dgvdata_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles Dgvdata.CellContentClick
        If _isLoading OrElse e.RowIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = Dgvdata.Rows(e.RowIndex)
        Dim kode As String = If(row.Cells("ColKode").Value IsNot Nothing, row.Cells("ColKode").Value.ToString(), "")
        Dim nama As String = If(row.Cells("ColNama").Value IsNot Nothing, row.Cells("ColNama").Value.ToString(), "")

        Select Case Dgvdata.Columns(e.ColumnIndex).Name
            Case "ColEdit"
                _isEditMode = True
                LblHeader.Text = "EDIT SUPPLIER"
                BTNSimpan.Text = "UPDATE (F2)"
                BTNSimpan.Visible = _canEdit
                TxtKode.Text = kode
                TxtNama.Text = nama
                TxtAlamat.Text = If(row.Cells("ColAlamat").Value IsNot Nothing, row.Cells("ColAlamat").Value.ToString(), "")
                TxtTelp.Text = If(row.Cells("ColHp").Value IsNot Nothing, row.Cells("ColHp").Value.ToString(), "")
                TxtJAngkaHutang.Text = If(row.Cells("ColJangka").Value IsNot Nothing, row.Cells("ColJangka").Value.ToString(), "0")
                TxtAwal.Text = If(row.Cells("ColHutangAwal").Value IsNot Nothing, Convert.ToDecimal(row.Cells("ColHutangAwal").Value).ToString("0"), "0")

                ' Task 9.1b — kunci TxtAwal jika HutangAwal > 0 agar tidak bisa diubah
                Dim hutangAwalEdit As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text)
                If hutangAwalEdit > 0 Then
                    TxtAwal.ReadOnly = True
                    ' TxtAwal.BackColor akan diatur otomatis oleh ModuleTheme (TextBox tidak aktif = abu-abu)
                    _toolTip.SetToolTip(TxtAwal, "Saldo awal tidak bisa diubah setelah ada nilai. Gunakan jurnal manual untuk koreksi.")
                Else
                    TxtAwal.ReadOnly = False
                    ' TxtAwal.BackColor akan diatur otomatis oleh ModuleTheme (TextBox aktif = putih+hitam)
                    _toolTip.SetToolTip(TxtAwal, "")
                End If

                TxtNama.Focus()

            Case "ColNonaktif"
                Dim status As String = If(row.Cells("ColStatus").Value IsNot Nothing, row.Cells("ColStatus").Value.ToString(), "Aktif")
                If status = "Aktif" Then
                    NonaktifkanSupliyer(kode, nama)
                Else
                    AktifkanSupliyer(kode, nama)
                End If

            Case "ColHapus"
                Dim hutang As Decimal = 0D
                If row.Cells("ColHutangAkhir").Value IsNot Nothing Then
                    hutang = ModuleAngka.ParseDecimal(row.Cells("ColHutangAkhir").Value)
                End If
                Dim statusHapus As String = If(row.Cells("ColStatus").Value IsNot Nothing, row.Cells("ColStatus").Value.ToString(), "Aktif")
                If hutang > 0 OrElse statusHapus = "Nonaktif" Then
                    MessageBox.Show("Supplier tidak dapat dihapus karena masih memiliki hutang atau berstatus Nonaktif." & vbCrLf &
                                    "Gunakan tombol 'Nonaktifkan' untuk menonaktifkan supplier ini.",
                                    "Tidak Dapat Dihapus", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    HapusSupliyer(kode, nama)
                End If
        End Select
    End Sub

#End Region

#Region "Auto Kode"
    Public Sub Kodesupliyer()
        Dim existingKodes As New List(Of String)
        Using cmd As New MySqlCommand("SELECT kode FROM tbl_supliyer ORDER BY Kode", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        If existingKodes.Count = 0 Then
            TxtKode.Text = "SPL-0001"
            Exit Sub
        End If

        Dim maxKode As String = ""
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "SPL-" & i.ToString("0000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 4)) + 1
            maxKode = "SPL-" & hitung.ToString("0000")
        End If

        TxtKode.Text = maxKode
    End Sub
#End Region


#Region "Simpan"
    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        If String.IsNullOrWhiteSpace(TxtNama.Text) Then
            MessageBox.Show("Nama supplier harus diisi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNama.Focus() : Exit Sub
        ElseIf String.IsNullOrWhiteSpace(TxtAlamat.Text) Then
            MessageBox.Show("Alamat supplier harus diisi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtAlamat.Focus() : Exit Sub
        End If

        Dim hutangAwal As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text)
        Dim jangkaHutang As Integer = ModuleAngka.ParseInteger(TxtJAngkaHutang.Text)

        If _isEditMode Then
            UpdateSupliyer(hutangAwal, jangkaHutang)
        Else
            InsertSupliyer(hutangAwal, jangkaHutang)
        End If
    End Sub

    Private Sub InsertSupliyer(hutangAwal As Decimal, jangkaHutang As Integer)
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Using cmd As New MySqlCommand("SELECT COUNT(*) FROM tbl_supliyer WHERE Nama = @Nama", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", TxtNama.Text)
                If Convert.ToInt32(cmd.ExecuteScalar()) > 0 Then
                    MessageBox.Show("Nama supplier sudah ada dalam database.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    TxtNama.Focus()
                    transaction.Rollback()
                    Exit Sub
                End If
            End Using

            Using cmd As New MySqlCommand(
                "INSERT INTO tbl_supliyer (Kode, Nama, Alamat, Hp, JangkaHutang, HutangAwal, Status) " &
                "VALUES (@Kode, @Nama, @Alamat, @Hp, @JangkaHutang, @HutangAwal, 'Aktif')", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                cmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@Alamat", StrConv(TxtAlamat.Text, vbProperCase))
                cmd.Parameters.AddWithValue("@Hp", TxtTelp.Text)
                cmd.Parameters.AddWithValue("@JangkaHutang", jangkaHutang)
                cmd.Parameters.AddWithValue("@HutangAwal", hutangAwal)
                cmd.ExecuteNonQuery()
            End Using

            ' Jurnal saldo awal hutang supplier
            If hutangAwal <> 0 Then
                SimpanJurnalSaldoAwal(transaction, "Saldo awal hutang supplier " & TxtNama.Text,
                                      "MODAL", "04.01.001",
                                      "TAGIHAN / SALDO PIUTANG", "01.04.002",
                                      hutangAwal)
            End If

            transaction.Commit()
            SyncTrigger.MasterBerubah("tbl_supliyer", TxtKode.Text, "INSERT", ModuleVariabel.NamaUser)
            Kondisiawal()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateSupliyer(hutangAwal As Decimal, jangkaHutang As Integer)
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' Ambil saldo awal lama untuk hitung selisih jurnal
            Dim hutangAwalLama As Decimal = 0
            Using cmd As New MySqlCommand("SELECT HutangAwal FROM tbl_supliyer WHERE Kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                Dim val = cmd.ExecuteScalar()
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then hutangAwalLama = Convert.ToDecimal(val)
            End Using

            Using cmd As New MySqlCommand(
                "UPDATE tbl_supliyer SET Nama = @Nama, Alamat = @Alamat, Hp = @Hp, " &
                "JangkaHutang = @JangkaHutang, HutangAwal = @HutangAwal WHERE Kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@Alamat", StrConv(TxtAlamat.Text, vbUpperCase))
                cmd.Parameters.AddWithValue("@Hp", TxtTelp.Text)
                cmd.Parameters.AddWithValue("@JangkaHutang", jangkaHutang)
                cmd.Parameters.AddWithValue("@HutangAwal", hutangAwal)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                cmd.ExecuteNonQuery()
            End Using

            ' Jurnal selisih perubahan saldo awal
            Dim selisih As Decimal = hutangAwal - hutangAwalLama
            If selisih <> 0 Then
                Dim uraian As String = "Perubahan saldo awal hutang supplier " & TxtNama.Text
                If selisih > 0 Then
                    SimpanJurnalSaldoAwal(transaction, uraian,
                                          "MODAL", "04.01.001",
                                          "TAGIHAN / SALDO PIUTANG", "01.04.002",
                                          selisih)
                Else
                    SimpanJurnalSaldoAwal(transaction, uraian,
                                          "TAGIHAN / SALDO PIUTANG", "01.04.002",
                                          "MODAL", "04.01.001",
                                          Math.Abs(selisih))
                End If
            End If

            transaction.Commit()
            ' Task 9.1b — recalculate HutangAkhir setelah simpan agar langsung terupdate di DGV
            UpdateHutangSupliyer(TxtKode.Text, Nothing)
            SyncTrigger.MasterBerubah("tbl_supliyer", TxtKode.Text, "UPDATE", ModuleVariabel.NamaUser)
            Kondisiawal()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SimpanJurnalSaldoAwal(ByVal transaction As MySqlTransaction,
                                       ByVal uraian As String,
                                       ByVal namaAkunD As String, ByVal nomorAkunD As String,
                                       ByVal namaAkunK As String, ByVal nomorAkunK As String,
                                       ByVal nominal As Decimal)
        Dim noTransaksi As String = "SA-" & DateTime.Now.ToString("yyyyMMddHHmmss") & "-" & TxtKode.Text
        Using cmd As New MySqlCommand(
            "INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, " &
            "NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, " &
            "NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
            "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, " &
            "@NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, " &
            "@NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NO_NOTA", TxtKode.Text)
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", namaAkunD)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", nomorAkunD)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", namaAkunK)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", nomorAkunK)
            cmd.Parameters.AddWithValue("@NOMINAL", nominal)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "SALDO AWAL")
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
            cmd.ExecuteNonQuery()
        End Using
    End Sub
#End Region

#Region "Hapus & Nonaktif"
    Private Sub HapusSupliyer(kode As String, nama As String)
        If MessageBox.Show($"Hapus supplier '{nama}'?", "Konfirmasi Hapus",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                Using cmd As New MySqlCommand("DELETE FROM tbl_supliyer WHERE Kode = @Kode", conn, transaction)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                transaction.Commit()
                Kondisiawal()
            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Gagal menghapus data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub NonaktifkanSupliyer(kode As String, nama As String)
        If MessageBox.Show($"Nonaktifkan supplier '{nama}'?", "Konfirmasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("UPDATE tbl_supliyer SET Status = 'Nonaktif' WHERE Kode = @Kode", conn)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                Tampilsupliyer()
            Catch ex As Exception
                MessageBox.Show("Gagal menonaktifkan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub AktifkanSupliyer(kode As String, nama As String)
        If MessageBox.Show($"Aktifkan kembali supplier '{nama}'?", "Konfirmasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("UPDATE tbl_supliyer SET Status = 'Aktif' WHERE Kode = @Kode", conn)
                    cmd.Parameters.AddWithValue("@Kode", kode)
                    cmd.ExecuteNonQuery()
                End Using
                Tampilsupliyer()
            Catch ex As Exception
                MessageBox.Show("Gagal mengaktifkan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
#End Region

#Region "Input Helper"
    Private Sub TxtValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtAwal.TextChanged
        Dim awal As Decimal = ModuleAngka.ParseDecimal(TxtAwal.Text)
        Label6.Text = "Rp. " & ModuleAngka.FormatRupiah(awal)
    End Sub

    Private Sub TxtAwal_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtAwal.KeyPress
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub
#End Region

#Region "Tombol & Keyboard"
    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Kondisiawal()
    End Sub

    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub TambahSupliyer_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : BTNSimpan.PerformClick()
            Case Keys.F4 : BtnTambah.PerformClick()
            Case Keys.Escape : BtnClose.PerformClick()
        End Select
    End Sub
#End Region

End Class

