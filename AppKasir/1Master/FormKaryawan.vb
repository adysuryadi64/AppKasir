Imports System.Reflection

Public Class FormKaryawan

    Private Sub FormKaryawan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        Dim Karyawan As Boolean() = ModulHakAkses.BacaHakAkses(FormUtama.SLevel.Text, "Karyawan", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BTNSimpan.Visible = Karyawan(1) ' CanAdd 
        'BTNSimpan.Visible = Karyawan(2) ' CanEdit 
        BTNHapus.Visible = Karyawan(3) ' CanDelete 

        Kondisiawal()
        UpdateTotalBonDanTotalBayarKaryawan()

        Me.Cursor = Cursors.Default
    End Sub

    Public Sub Kondisiawal()
        TxtKode.Clear()
        TxtNama.Clear()
        TxtJabatan.Clear()
        TxtAwal.Clear()
        DtpTransaksi.Value = DateTime.Today
        DtpTransaksi.Format = DateTimePickerFormat.Custom
        DtpTransaksi.CustomFormat = "dd/MM/yyyy"
        TampilKaryawan()
        KodeKaryawan()

        TxtNama.Focus()
    End Sub

    Public Sub TampilKaryawan()
        Dim dt As New DataTable()

        Using cmd As New MySqlCommand("SELECT Kode, Nama, Jabatan, TglMasuk, Gaji, SaldoAkhir FROM tbl_karyawan ORDER BY Kode", conn)
            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        Dgvdata.DataSource = dt

        ' Mengatur header kolom
        With Dgvdata
            .Columns(0).HeaderText = "Kode Karyawan"
            .Columns(1).HeaderText = "Nama Karyawan"
            .Columns(2).HeaderText = "Jabatan"
            .Columns(3).HeaderText = "Tanggal Masuk"
            .Columns(4).HeaderText = "Gaji"
            .Columns(5).HeaderText = "Saldo Bon"

            ' Mengatur format dan penjajaran sel
            .Columns(4).DefaultCellStyle.Format = "###,###"
            .Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(5).DefaultCellStyle.Format = "###,###"
            .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Gray
            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            DataGridViewExtension.EnableDoubleBuffering(Dgvdata)
        End With
    End Sub


    Public Class DataGridViewExtension
        Public Shared Sub EnableDoubleBuffering(ByVal dataGridView As DataGridView)
            dataGridView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty, Nothing, dataGridView, New Object() {True})
        End Sub
    End Class

    Public Sub KodeKaryawan()
        Dim maxKode As String = ""
        Dim existingKodes As New List(Of String)

        ' Mengambil semua kode yang sudah ada dari database
        Using cmd As New MySqlCommand("SELECT kode FROM tbl_karyawan ORDER BY Kode", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        ' Jika tidak ada kode yang ada, gunakan SPL-0001
        If existingKodes.Count = 0 Then
            TxtKode.Text = "KRY-0001"
            Exit Sub
        End If

        ' Mencari nomor berikutnya yang belum terpakai
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "KRY-" & i.ToString("0000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        ' Jika tidak ada nomor berikutnya yang tersedia, gunakan nomor setelah kode terakhir
        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim Hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 4)) + 1
            maxKode = "KRY-" & Hitung.ToString("0000")
        End If

        TxtKode.Text = maxKode
    End Sub

    Private Sub Dgvdata_CellClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles Dgvdata.CellClick
        If Dgvdata.Rows.Count >= 1 AndAlso Dgvdata.CurrentRow IsNot Nothing Then
            TxtKode.Text = If(IsDBNull(Dgvdata.Item(0, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(0, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtNama.Text = If(IsDBNull(Dgvdata.Item(1, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(1, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtJabatan.Text = If(IsDBNull(Dgvdata.Item(2, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(2, Dgvdata.CurrentRow.Index).Value.ToString())
            DtpTransaksi.Value = If(IsDBNull(Dgvdata.Item(3, Dgvdata.CurrentRow.Index).Value), DateTime.Now, Convert.ToDateTime(Dgvdata.Item(3, Dgvdata.CurrentRow.Index).Value))

            Dim awalValue As Decimal
            If Decimal.TryParse(If(IsDBNull(Dgvdata.Item(4, Dgvdata.CurrentRow.Index).Value), "0", Dgvdata.Item(4, Dgvdata.CurrentRow.Index).Value.ToString()), awalValue) Then
                TxtAwal.Text = awalValue.ToString("0.##") ' Memastikan 2 angka di belakang koma
            Else
                TxtAwal.Text = "0.00"
            End If
        End If

        TxtNama.Focus()
    End Sub



    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Call Kondisiawal()
    End Sub

    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNHapus.Click
        If TxtKode.Text = "" Or TxtNama.Text = "" Then
            MessageBox.Show("Pilih data yang akan dihapus !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Using cmd As New MySqlCommand("SELECT SaldoAkhir FROM tbl_karyawan WHERE kode = @Kode", conn)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                Dim saldoAkhir As Object = cmd.ExecuteScalar()

                If saldoAkhir IsNot Nothing Then
                    ' Konversi nilai saldoAkhir menjadi angka
                    Dim saldo As Decimal = Convert.ToDecimal(saldoAkhir)

                    ' Periksa apakah saldo akhir sama dengan 0
                    If saldo = 0 Then
                        ' Jika SaldoAkhir 0, hapus data
                        Using deleteCmd As New MySqlCommand("DELETE FROM tbl_karyawan WHERE kode = @Kode AND SaldoAkhir = '0'", conn)
                            deleteCmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                            deleteCmd.ExecuteNonQuery()
                        End Using
                        DatabaseModule.CatatanAksiHistory("Hapus karyawan " & TxtKode.Text)
                        Call Kondisiawal()
                    Else
                        ' Jika SaldoAkhir tidak 0, tampilkan peringatan
                        MessageBox.Show("Data tidak dapat dihapus karena masih memiliki bon.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End Using


        End If
    End Sub

    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        ' Validasi input
        If String.IsNullOrWhiteSpace(TxtKode.Text) OrElse String.IsNullOrWhiteSpace(TxtNama.Text) OrElse String.IsNullOrWhiteSpace(TxtJabatan.Text) Then
            MessageBox.Show("Data harus diisi dengan lengkap !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Inisialisasi nilai default jika TextBox kosong
        TxtAwal.Text = If(String.IsNullOrEmpty(TxtAwal.Text), "0", TxtAwal.Text)

        Dim transaction As MySqlTransaction = Nothing

        Try
            ' Mulai transaksi
            transaction = conn.BeginTransaction()

            Using cmd As New MySqlCommand("SELECT kode FROM tbl_karyawan WHERE kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        rd.Close() ' Explicitly close the reader

                        If MessageBox.Show("Kode Karyawan sudah ada, Apakah lanjut edit data ...!!!", "Peringatan", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            Using cmdUpdate As New MySqlCommand("UPDATE tbl_karyawan SET Nama = @Nama, Jabatan = @Jabatan, TglMasuk = @TglMasuk, Gaji = @Gaji WHERE Kode = @Kode", conn, transaction)
                                cmdUpdate.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                                cmdUpdate.Parameters.AddWithValue("@Jabatan", StrConv(TxtJabatan.Text, vbUpperCase))
                                cmdUpdate.Parameters.AddWithValue("@TglMasuk", DtpTransaksi.Value.ToString("yyyy-MM-dd"))
                                cmdUpdate.Parameters.AddWithValue("@Gaji", Decimal.Parse(TxtAwal.Text))
                                cmdUpdate.Parameters.AddWithValue("@Kode", TxtKode.Text)
                                cmdUpdate.ExecuteNonQuery()
                            End Using
                        End If
                    Else
                        rd.Close() ' Explicitly close the reader

                        Using cmdInsert As New MySqlCommand("INSERT INTO tbl_karyawan (Kode, Nama, Jabatan, TglMasuk, Gaji) VALUES (@Kode, @Nama, @Jabatan, @TglMasuk, @Gaji)", conn, transaction)
                            cmdInsert.Parameters.AddWithValue("@Kode", TxtKode.Text)
                            cmdInsert.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                            cmdInsert.Parameters.AddWithValue("@Jabatan", StrConv(TxtJabatan.Text, vbProperCase))
                            cmdInsert.Parameters.AddWithValue("@TglMasuk", DtpTransaksi.Value.ToString("yyyy-MM-dd"))
                            cmdInsert.Parameters.AddWithValue("@Gaji", Decimal.Parse(TxtAwal.Text))
                            cmdInsert.ExecuteNonQuery()
                        End Using
                    End If
                End Using
            End Using

            ' Commit transaksi
            transaction.Commit()

            DatabaseModule.CatatanAksiHistory("Simpan karyawan " & TxtKode.Text)
            Call Kondisiawal()

            'MessageBox.Show("Data berhasil disimpan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            ' Rollback transaksi jika terjadi kesalahan
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


    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        Close()
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