Imports System.Globalization

Class TambahSupliyer
    Private Sub TambahSupliyer_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor

        Dim Supplier As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Supplier")
        ' Terapkan nilai hak akses ke tombol-tombol
        BTNSimpan.Visible = Supplier(1) ' CanAdd 
        'BTNSimpan.Visible = Supplier(2) ' CanEdit 
        BTNHapus.Visible = Supplier(3) ' CanDelete 

        Call Kondisiawal()

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub Kondisiawal()
        TxtKode.Clear()
        TxtNama.Clear()
        TxtAlamat.Clear()
        TxtTelp.Clear()
        TxtAwal.Text = 0
        BTNSimpan.Text = "SIMPAN (F2)"
        UpdateSupliyerFromPembelianHutangDibayar()
        Tampilsupliyer()
        Kodesupliyer()
        Dgvdata.ClearSelection()
        TxtNama.Select()
    End Sub

    Public Sub UpdateSupliyerFromPembelianHutangDibayar()
        ' Mulai transaksi
        Using transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' Set TotalHutang dan TotalBayar menjadi 0 untuk semua supplier
                Using resetCmd As New MySqlCommand("UPDATE tbl_supliyer SET TotalHutang = 0, TotalBayar = 0, HutangAkhir = 0", conn, transaction)
                    resetCmd.ExecuteNonQuery()
                End Using

                ' Daftar untuk menyimpan ID_SUPPLIER dan total hutang sebelum digunakan untuk update
                Dim hutangList As New Dictionary(Of String, Decimal)()

                ' Ambil total hutang per supplier
                Using cmd As New MySqlCommand("SELECT ID_SUPPLIER, SUM(IFNULL(TAGIHAN, 0)) AS HUTANG FROM pembelian GROUP BY ID_SUPPLIER", conn, transaction)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim kodeSupplier As String = Convert.ToString(reader("ID_SUPPLIER"))

                            ' Check if hutang is DBNull
                            Dim totalHutang As Decimal = If(IsDBNull(reader("HUTANG")), 0, Convert.ToDecimal(reader("HUTANG")))

                            ' Simpan data yang diambil ke dalam list (Dictionary)
                            If Not hutangList.ContainsKey(kodeSupplier) Then
                                hutangList.Add(kodeSupplier, totalHutang)
                            End If
                        End While
                    End Using
                End Using

                ' Setelah data terkumpul dalam hutangList, lakukan update
                For Each kvp As KeyValuePair(Of String, Decimal) In hutangList
                    Dim kodeSupplier As String = kvp.Key
                    Dim totalHutang As Decimal = kvp.Value

                    ' Update tabel tbl_supliyer dengan nilai yang disimpan di hutangList
                    Using updateCmd As New MySqlCommand("UPDATE tbl_supliyer SET HutangAkhir = @HutangAkhir WHERE KODE = @Kode", conn, transaction)
                        updateCmd.Parameters.AddWithValue("@HutangAkhir", totalHutang)
                        updateCmd.Parameters.AddWithValue("@Kode", kodeSupplier)
                        updateCmd.ExecuteNonQuery()
                    End Using
                Next


                ' Update HutangAkhir dengan HutangAwal
                Using updateFinalCmd As New MySqlCommand("UPDATE tbl_supliyer SET HutangAkhir = HutangAkhir + HutangAwal", conn, transaction)
                    updateFinalCmd.ExecuteNonQuery()
                End Using

                ' Commit transaksi jika berhasil
                transaction.Commit()
            Catch ex As Exception
                ' Rollback transaksi jika ada kesalahan
                transaction.Rollback()
                MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub



    Public Sub Tampilsupliyer()
        Dim dt As New DataTable()

        Using cmd As New MySqlCommand("SELECT Kode, Nama, Alamat, Hp, JangkaHutang, HutangAwal, TotalHutang, Totalbayar, HutangAkhir FROM tbl_supliyer ORDER BY Kode", conn)
            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        Dgvdata.DataSource = dt

        Dim columnsToFormat As String() = {"JangkaHutang", "HutangAwal", "TotalHutang", "Totalbayar", "HutangAkhir"}
        Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
            {"Kode", "Kode"},
            {"Nama", "Nama Supplier"},
            {"Alamat", "Alamat"},
            {"Hp", "Nomor HP"},
            {"JangkaHutang", "Jangka Hutang"},
            {"HutangAwal", "Hutang Awal"},
            {"TotalHutang", "Total Hutang"},
            {"Totalbayar", "Total Bayar"},
            {"HutangAkhir", "Hutang Akhir"}
        }

        With Dgvdata
            ' Menyembunyikan kolom TotalHutang dan TotalBayar
            .Columns("TotalHutang").Visible = False
            .Columns("TotalBayar").Visible = False

            ' Loop through columns and set format and alignment
            For Each columnName As String In columnsToFormat
                If .Columns.Contains(columnName) Then
                    ' Use custom format to display numbers with commas and up to two decimal places if not zero
                    .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                    .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next

            ' Rename columns
            For Each column As DataGridViewColumn In .Columns
                If columnNames.ContainsKey(column.Name) Then
                    column.HeaderText = columnNames(column.Name)
                End If
            Next

            ' Set header style
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            EnableDoubleBuffering(Dgvdata)
        End With
    End Sub

    ' Method to enable double buffering
    Public Shared Sub EnableDoubleBuffering(ByVal dgv As DataGridView)
        dgv.GetType().InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, dgv, New Object() {True})
    End Sub


    Public Sub Kodesupliyer()
        Dim maxKode As String = ""
        Dim existingKodes As New List(Of String)

        ' Mengambil semua kode yang sudah ada dari database
        Using cmd As New MySqlCommand("SELECT kode FROM tbl_supliyer ORDER BY Kode", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        ' Jika tidak ada kode yang ada, gunakan SPL-0001
        If existingKodes.Count = 0 Then
            TxtKode.Text = "SPL-0001"
            Exit Sub
        End If

        ' Mencari nomor berikutnya yang belum terpakai
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "SPL-" & i.ToString("0000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        ' Jika tidak ada nomor berikutnya yang tersedia, gunakan nomor setelah kode terakhir
        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim Hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 4)) + 1
            maxKode = "SPL-" & Hitung.ToString("0000")
        End If

        TxtKode.Text = maxKode
    End Sub

    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub Dgvdata_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Dgvdata.CellClick
        If Dgvdata.Rows.Count >= 1 AndAlso Dgvdata.CurrentRow IsNot Nothing Then
            TxtKode.Text = If(IsDBNull(Dgvdata.Item(0, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(0, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtNama.Text = If(IsDBNull(Dgvdata.Item(1, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(1, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtAlamat.Text = If(IsDBNull(Dgvdata.Item(2, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(2, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtTelp.Text = If(IsDBNull(Dgvdata.Item(3, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(3, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtJAngkaHutang.Text = If(IsDBNull(Dgvdata.Item(4, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(4, Dgvdata.CurrentRow.Index).Value.ToString())

            Dim awalValue As Decimal
            If Decimal.TryParse(If(IsDBNull(Dgvdata.Item(5, Dgvdata.CurrentRow.Index).Value), "0", Dgvdata.Item(5, Dgvdata.CurrentRow.Index).Value.ToString()), awalValue) Then
                TxtAwal.Text = awalValue.ToString("0.##") ' Memastikan 2 angka di belakang koma
            Else
                TxtAwal.Text = "0.00"
            End If
        End If

        BTNSimpan.Text = "EDIT (F2)"

        If BTNSimpan.Text = "EDIT (F2)" Then
            Dim Supplier As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Supplier")
            ' Terapkan nilai hak akses ke tombol-tombol
            BTNSimpan.Visible = Supplier(2) ' CanEdit 
        End If


        TxtNama.Focus()
    End Sub


    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Call Kondisiawal()
    End Sub

    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNHapus.Click
        ' Cek apakah kode atau nama kosong
        If String.IsNullOrEmpty(TxtKode.Text) Or String.IsNullOrEmpty(TxtNama.Text) Then
            MessageBox.Show("Pilih data yang akan dihapus !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Cek apakah ada saldo hutang
        Dim hutangAkhir As Decimal
        Using cmdCheck As New MySqlCommand("SELECT HutangAkhir FROM tbl_supliyer WHERE kode = @Kode", conn)
            cmdCheck.Parameters.AddWithValue("@Kode", TxtKode.Text)
            Dim result = cmdCheck.ExecuteScalar()
            If result IsNot Nothing AndAlso Decimal.TryParse(result.ToString(), hutangAkhir) AndAlso hutangAkhir > 0 Then
                MessageBox.Show("Data tidak dapat dihapus karena masih memiliki saldo hutang.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
        End Using

        ' Konfirmasi penghapusan
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            ' Mulai transaksi
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    ' Eksekusi query DELETE jika tidak ada hutang
                    Using cmdDelete As New MySqlCommand("DELETE FROM tbl_supliyer WHERE kode = @Kode", conn, transaction)
                        cmdDelete.Parameters.AddWithValue("@Kode", TxtKode.Text)
                        cmdDelete.ExecuteNonQuery()
                    End Using

                    ' Commit transaksi jika berhasil
                    transaction.Commit()
                    DatabaseModule.CatatanAksiHistory("Hapus suppliyer " & TxtNama.Text)
                    Call Kondisiawal() ' Refresh form
                Catch ex As Exception
                    ' Rollback transaksi jika ada kesalahan
                    transaction.Rollback()
                    MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End If
    End Sub


    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        If TxtKode.Text = "" Then
            MessageBox.Show("Kode supliyer harus di isi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtKode.Focus()
            Exit Sub
        ElseIf TxtNama.Text = "" Then
            MessageBox.Show("Nama supliyer harus di isi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNama.Focus()
            Exit Sub
        ElseIf TxtAlamat.Text = "" Then
            MessageBox.Show("Alamat supliyer harus di isi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtAlamat.Focus()
            Exit Sub
        End If

        ' Validate and parse numeric inputs
        Dim hutangAwal As Decimal
        If Not Decimal.TryParse(TxtAwal.Text, hutangAwal) Then
            MessageBox.Show("Hutang Awal harus diisi dengan angka yang valid.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtAwal.Text = "0"
            TxtAwal.Focus()
            Exit Sub
        End If

        Dim jangkaHutang As Integer
        If Not Integer.TryParse(TxtJAngkaHutang.Text, jangkaHutang) Then
            MessageBox.Show("Jangka Hutang harus diisi dengan angka yang valid.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtJAngkaHutang.Text = "0"
            TxtJAngkaHutang.Focus()
            Exit Sub
        End If

        Dim transaction As MySqlTransaction = Nothing

        Try
            ' Begin transaction
            transaction = conn.BeginTransaction()



            If BTNSimpan.Text = "SIMPAN (F2)" Then
                ' Check if supplier name already exists
                Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM tbl_supliyer WHERE Nama = @nama", conn, transaction)
                    cmdCheck.Parameters.AddWithValue("@nama", TxtNama.Text)
                    Dim count As Integer = CInt(cmdCheck.ExecuteScalar())
                    If count > 0 Then
                        MessageBox.Show("Nama supliyer sudah ada dalam database.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TxtNama.Focus()
                        Exit Sub
                    End If
                End Using

                ' Insert new supplier
                Using cmdInsert As New MySqlCommand("INSERT INTO tbl_supliyer (Kode, Nama, Alamat, Hp, JangkaHutang, HutangAwal) VALUES (@Kode, @Nama, @Alamat, @Hp, @JangkaHutang, @HutangAwal)", conn, transaction)
                    cmdInsert.Parameters.AddWithValue("@Kode", TxtKode.Text)
                    cmdInsert.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                    cmdInsert.Parameters.AddWithValue("@Alamat", StrConv(TxtAlamat.Text, vbProperCase))
                    cmdInsert.Parameters.AddWithValue("@Hp", TxtTelp.Text)
                    cmdInsert.Parameters.AddWithValue("@JangkaHutang", jangkaHutang)
                    cmdInsert.Parameters.AddWithValue("@HutangAwal", hutangAwal)

                    cmdInsert.ExecuteNonQuery()
                End Using

                ' Commit transaction if successful
                transaction.Commit()
                DatabaseModule.CatatanAksiHistory("Simpan suppliyer " & TxtNama.Text)

            Else
                ' Update existing supplier
                Using cmdUpdate As New MySqlCommand("UPDATE tbl_supliyer SET Nama = @Nama, Alamat = @Alamat, Hp = @Hp, JangkaHutang = @JangkaHutang, HutangAwal = @HutangAwal WHERE Kode = @Kode", conn, transaction)
                    cmdUpdate.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                    cmdUpdate.Parameters.AddWithValue("@Alamat", StrConv(TxtAlamat.Text, vbUpperCase))
                    cmdUpdate.Parameters.AddWithValue("@Hp", TxtTelp.Text)
                    cmdUpdate.Parameters.AddWithValue("@JangkaHutang", jangkaHutang)
                    cmdUpdate.Parameters.AddWithValue("@HutangAwal", hutangAwal)
                    cmdUpdate.Parameters.AddWithValue("@Kode", TxtKode.Text)

                    cmdUpdate.ExecuteNonQuery()
                End Using

                ' Commit transaction if successful
                transaction.Commit()
                DatabaseModule.CatatanAksiHistory("Edit suppliyer " & TxtNama.Text)
            End If


            ' Reset form controls
            Call Kondisiawal()

        Catch ex As Exception
            ' Rollback transaction on error
            If transaction IsNot Nothing Then
                transaction.Rollback()
            End If
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub TxtValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtAwal.TextChanged
        Dim awal As Integer

        If Not Integer.TryParse(TxtAwal.Text, awal) Then
            awal = 0
        End If
        Label6.Text = "Rp. " + FormatNumber(awal.ToString(), 0)

    End Sub

    Private Sub TxtAwal_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtAwal.KeyPress
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub


    Private Sub TambahSupliyer_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                BTNSimpan.PerformClick()
            Case Keys.F3
                BTNHapus.PerformClick()
            Case Keys.F4
                BtnTambah.PerformClick()
            Case Keys.Escape
                BtnClose.PerformClick()
        End Select

    End Sub


End Class