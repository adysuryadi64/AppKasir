Imports System.Globalization

Public Class TambahPelanggan
    Public bsPelanggan As New BindingSource()
    Public dtPelanggan As New DataTable()

    Private Sub TambahPelanggan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor

        Dim Pelanggan As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Pelanggan")
        ' Terapkan nilai hak akses ke tombol-tombol
        BTNSimpan.Visible = Pelanggan(1) ' CanAdd 
        'BTNSimpan.Visible = Pelanggan(2) ' CanEdit 
        BTNHapus.Visible = Pelanggan(3) ' CanDelete 



        Call Kondisiawal()

        Me.Cursor = Cursors.Default
    End Sub

    Public Sub TampilPelanggan()
        Dim query As String = "SELECT KODE, NAMA, ALAMAT, NO_TELP, JENIS, JangkaPiutang, HutangAwal, TotalHutang, Totalbayar, HutangAkhir FROM tbl_pelanggan"

        Using cmd As New MySqlCommand(query, conn)
            Using da As New MySqlDataAdapter(cmd)
                dtPelanggan.Clear()
                da.Fill(dtPelanggan)
            End Using
        End Using

        bsPelanggan.DataSource = dtPelanggan
        Dgvdata.DataSource = bsPelanggan

        Dim columnsToFormat As String() = {"JangkaPiutang", "HutangAwal", "TotalHutang", "Totalbayar", "HutangAkhir"}
        Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
            {"KODE", "Kode"},
            {"NAMA", "Nama"},
            {"ALAMAT", "Alamat"},
            {"NO_TELP", "Nomor Telepon"},
            {"JENIS", "Jenis"},
            {"JangkaPiutang", "Jangka Piutang"},
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

    Public Sub UpdatePiutangDibayar()
        'Mulai transaksi
        Using transaction As MySqlTransaction = conn.BeginTransaction()
            Try

                ' Reset TotalHutang, TotalBayar, dan HutangAkhir untuk semua pelanggan
                Using updateCmd As New MySqlCommand("UPDATE tbl_pelanggan SET TotalHutang = 0, TotalBayar = 0, HutangAkhir = 0", conn)
                    updateCmd.ExecuteNonQuery()
                End Using

                ' Simpan hasil pembacaan query ke dalam dictionary
                Dim pelangganHutang As New Dictionary(Of String, Decimal)()

                ' Ambil dan hitung total hutang dari tabel penjualan
                Dim query As String = "SELECT ID_PELANGGAN, SUM(IFNULL(SISA_TAGIHAN, 0)) AS HUTANG FROM penjualan GROUP BY ID_PELANGGAN"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim kodePelanggan As String = If(IsDBNull(reader("ID_PELANGGAN")), String.Empty, Convert.ToString(reader("ID_PELANGGAN")))
                            Dim totalHutang As Decimal = If(IsDBNull(reader("HUTANG")), 0D, Convert.ToDecimal(reader("HUTANG")))

                            If Not String.IsNullOrEmpty(kodePelanggan) Then
                                pelangganHutang(kodePelanggan) = totalHutang
                            End If
                        End While
                    End Using
                End Using

                ' Perbarui tabel tbl_pelanggan berdasarkan data dalam dictionary
                For Each entry As KeyValuePair(Of String, Decimal) In pelangganHutang
                    Using updateCmd As New MySqlCommand("UPDATE tbl_pelanggan SET HutangAkhir = @HutangAkhir WHERE KODE = @Kode", conn)
                        updateCmd.Parameters.AddWithValue("@HutangAkhir", entry.Value)
                        updateCmd.Parameters.AddWithValue("@Kode", entry.Key)
                        updateCmd.ExecuteNonQuery()
                    End Using
                Next


                ' Tambahkan HutangAwal ke HutangAkhir
                Using updateCmd As New MySqlCommand("UPDATE tbl_pelanggan SET HutangAkhir = HutangAkhir + HutangAwal", conn)
                    updateCmd.ExecuteNonQuery()
                End Using

                ' Commit transaksi setelah semua operasi berhasil
                transaction.Commit()

            Catch ex As Exception
                ' Rollback jika ada kesalahan
                transaction.Rollback()
                MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub


    Public Sub Kondisiawal()
        TxtKode.Clear()
        TxtNama.Clear()
        TxtAlamat.Clear()
        TxtTelp.Clear()
        CmbJenis.Text = ""
        BTNSimpan.Text = "SIMPAN (F2)"
        TxtAwal.Text = 0
        TxtBayar.Text = 0
        TxtTotal.Text = 0
        TxtSisa.Text = 0
        UpdatePiutangDibayar()
        TampilPelanggan()
        Kodepelanggan()
    End Sub

    Public Sub Kodepelanggan()
        Dim maxKode As String = ""
        Dim existingKodes As New List(Of String)

        ' Mengambil semua kode yang sudah ada dari database
        Using cmd As New MySqlCommand("SELECT KODE FROM tbl_pelanggan ORDER BY KODE", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        ' Jika tidak ada kode yang ada, gunakan SPL-0001
        If existingKodes.Count = 0 Then
            TxtKode.Text = "PEL-0001"
            Exit Sub
        End If

        ' Mencari nomor berikutnya yang belum terpakai
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "PEL-" & i.ToString("0000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        ' Jika tidak ada nomor berikutnya yang tersedia, gunakan nomor setelah kode terakhir
        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim Hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 4)) + 1
            maxKode = "PEL-" & Hitung.ToString("0000")
        End If

        TxtKode.Text = maxKode
    End Sub


    Private Sub Dgvdata_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Dgvdata.CellClick
        If Dgvdata.Rows.Count >= 1 Then
            BTNSimpan.Text = "EDIT (F2)"

            If BTNSimpan.Text = "EDIT (F2)" Then
                Dim Pelanggan As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Pelanggan")
                ' Terapkan nilai hak akses ke tombol-tombol
                BTNSimpan.Visible = Pelanggan(2) ' CanEdit 
            End If

            TxtKode.Text = If(IsDBNull(Dgvdata.Item(0, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(0, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtNama.Text = If(IsDBNull(Dgvdata.Item(1, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(1, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtAlamat.Text = If(IsDBNull(Dgvdata.Item(2, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(2, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtTelp.Text = If(IsDBNull(Dgvdata.Item(3, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(3, Dgvdata.CurrentRow.Index).Value.ToString())
            CmbJenis.Text = If(IsDBNull(Dgvdata.Item(4, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(4, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtJangkaPiutang.Text = If(IsDBNull(Dgvdata.Item(5, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(5, Dgvdata.CurrentRow.Index).Value.ToString())

            ' Konversi nilai TxtTotal dan TxtSisa ke tipe data Decimal untuk memanipulasi angka
            Dim awalValue As Decimal = Decimal.Parse(Dgvdata.Item(6, Dgvdata.CurrentRow.Index).Value.ToString())
            Dim bayarValue As Decimal = Decimal.Parse(Dgvdata.Item(7, Dgvdata.CurrentRow.Index).Value.ToString())
            Dim totalValue As Decimal = Decimal.Parse(Dgvdata.Item(8, Dgvdata.CurrentRow.Index).Value.ToString())
            Dim sisaValue As Decimal = Decimal.Parse(Dgvdata.Item(9, Dgvdata.CurrentRow.Index).Value.ToString())

            ' Format nilai TxtAwal, TxtBayar, TxtTotal, dan TxtSisa
            TxtAwal.Text = awalValue.ToString("0.##") ' Memastikan 2 angka di belakang koma
            TxtBayar.Text = bayarValue.ToString("0.##") ' Memastikan 2 angka di belakang koma
            TxtTotal.Text = totalValue.ToString("0.##") ' Memastikan 2 angka di belakang koma
            TxtSisa.Text = sisaValue.ToString("0.##")   ' Memastikan 2 angka di belakang koma
        End If

    End Sub

    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Call Kondisiawal()
    End Sub


    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        If Not Decimal.TryParse(TxtAwal.Text, Nothing) Then
            TxtAwal.Text = "0"
        End If
        If TxtKode.Text = "" Then
            MessageBox.Show("Kode pelanggan harus di isi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
            TxtKode.Focus()
        ElseIf TxtNama.Text = "" Then
            MessageBox.Show("Nama pelanggan harus di isi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
            TxtNama.Focus()
        ElseIf TxtAlamat.Text = "" Then
            MessageBox.Show("Alamat pelanggan harus di isi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
            TxtAlamat.Focus()
        ElseIf CmbJenis.Text = "" Then
            MessageBox.Show("Jenis pelanggan harus di isi !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
            CmbJenis.Focus()
        End If

        ' Inisialisasi nilai default jika TextBox kosong
        Dim jangkaPiutang As Integer
        If Integer.TryParse(TxtJangkaPiutang.Text, jangkaPiutang) = False Then
            jangkaPiutang = 0 ' Jika kosong atau tidak valid, setel ke 0
        End If

        Dim hutangAwal As Decimal
        If Decimal.TryParse(TxtAwal.Text, hutangAwal) = False Then
            hutangAwal = 0D ' Jika kosong atau tidak valid, setel ke 0
        End If

        Dim totalHutang As Decimal
        If Decimal.TryParse(TxtTotal.Text, totalHutang) = False Then
            totalHutang = 0D ' Jika kosong atau tidak valid, setel ke 0
        End If

        Dim totalBayar As Decimal
        If Decimal.TryParse(TxtBayar.Text, totalBayar) = False Then
            totalBayar = 0D ' Jika kosong atau tidak valid, setel ke 0
        End If

        Dim hutangAkhir As Decimal
        If Decimal.TryParse(TxtSisa.Text, hutangAkhir) = False Then
            hutangAkhir = 0D ' Jika kosong atau tidak valid, setel ke 0
        End If

        Dim transaction As MySqlTransaction = Nothing

        Try
            ' Mulai transaksi
            transaction = conn.BeginTransaction()

            If BTNSimpan.Text = "SIMPAN (F2)" Then
                Using cmd As New MySqlCommand("SELECT nama FROM tbl_pelanggan WHERE nama = @Nama", conn, transaction)
                    cmd.Parameters.AddWithValue("@Nama", TxtNama.Text)

                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.HasRows Then
                            ' Jika kode dan nama sudah ada, tampilkan pesan dan keluar dari subroutine
                            MessageBox.Show("Nama sudah ada dalam database.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            TxtNama.Select()
                            Exit Sub
                        End If
                    End Using
                End Using

                ' Definisikan query INSERT dengan lebih rapi
                Dim insertQuery As String = "INSERT INTO tbl_pelanggan (Kode, Nama, Alamat, NO_TELP, Jenis, JangkaPiutang, HutangAwal, TotalHutang, TotalBayar, HutangAkhir) " &
                                            "VALUES (@Kode, @Nama, @Alamat, @NoTelp, @Jenis, @JangkaPiutang, @HutangAwal, @TotalHutang, @TotalBayar, @HutangAkhir)"

                Using insertCmd As New MySqlCommand(insertQuery, conn, transaction)
                    ' Menambahkan parameter dengan lebih rapi
                    insertCmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                    insertCmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                    insertCmd.Parameters.AddWithValue("@Alamat", StrConv(TxtAlamat.Text, vbProperCase))
                    insertCmd.Parameters.AddWithValue("@NoTelp", TxtTelp.Text)
                    insertCmd.Parameters.AddWithValue("@Jenis", StrConv(CmbJenis.Text, vbProperCase))
                    insertCmd.Parameters.AddWithValue("@JangkaPiutang", jangkaPiutang)
                    insertCmd.Parameters.AddWithValue("@HutangAwal", hutangAwal)
                    insertCmd.Parameters.AddWithValue("@TotalHutang", totalHutang)
                    insertCmd.Parameters.AddWithValue("@TotalBayar", totalBayar)
                    insertCmd.Parameters.AddWithValue("@HutangAkhir", hutangAkhir)

                    ' Eksekusi perintah INSERT
                    insertCmd.ExecuteNonQuery()
                End Using


                ' Commit transaksi
                transaction.Commit()
                DatabaseModule.CatatanAksiHistory("Tambah pelanggan " & TxtNama.Text)
                Call Kondisiawal()
            Else


                ' Definisikan query SQL dengan lebih rapi
                Dim updateQuery As String = "UPDATE tbl_pelanggan " &
                                            "SET Nama = @Nama, ALamat = @Alamat, NO_TELP = @NoTelp, Jenis = @Jenis, " &
                                            "JangkaPiutang = @JangkaPiutang, HutangAwal = @HutangAwal, TotalHutang = @TotalHutang, " &
                                            "Totalbayar = @TotalBayar, HutangAkhir = @HutangAkhir " &
                                            "WHERE Kode = @Kode"

                Using updateCmd As New MySqlCommand(updateQuery, conn, transaction)
                    ' Menambahkan parameter dengan lebih rapi
                    updateCmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbUpperCase))
                    updateCmd.Parameters.AddWithValue("@Alamat", StrConv(TxtAlamat.Text, vbUpperCase))
                    updateCmd.Parameters.AddWithValue("@NoTelp", TxtTelp.Text)
                    updateCmd.Parameters.AddWithValue("@Jenis", StrConv(CmbJenis.Text, vbProperCase))
                    updateCmd.Parameters.AddWithValue("@JangkaPiutang", jangkaPiutang)
                    updateCmd.Parameters.AddWithValue("@HutangAwal", hutangAwal)
                    updateCmd.Parameters.AddWithValue("@TotalHutang", totalHutang)
                    updateCmd.Parameters.AddWithValue("@TotalBayar", totalBayar)
                    updateCmd.Parameters.AddWithValue("@HutangAkhir", hutangAkhir)
                    updateCmd.Parameters.AddWithValue("@Kode", TxtKode.Text)

                    ' Eksekusi perintah SQL
                    updateCmd.ExecuteNonQuery()
                End Using


                ' Commit transaksi
                transaction.Commit()
                DatabaseModule.CatatanAksiHistory("Update pelanggan " & TxtNama.Text)
                Call Kondisiawal()
            End If
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNHapus.Click
        ' Cek apakah kode atau nama kosong
        If String.IsNullOrEmpty(TxtKode.Text) OrElse String.IsNullOrEmpty(TxtNama.Text) Then
            MessageBox.Show("Pilih data yang akan dihapus !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Cek apakah pelanggan masih punya hutang
        Dim hutangAkhir As Decimal
        Using cmdCheck As New MySqlCommand("SELECT HutangAkhir FROM tbl_pelanggan WHERE kode = @Kode", conn)
            cmdCheck.Parameters.AddWithValue("@Kode", TxtKode.Text)
            Dim result = cmdCheck.ExecuteScalar()

            If result IsNot Nothing AndAlso Decimal.TryParse(result.ToString(), hutangAkhir) AndAlso hutangAkhir > 0 Then
                MessageBox.Show("Pelanggan masih memiliki hutang. Data tidak dapat dihapus.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
        End Using

        ' Konfirmasi penghapusan
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            ' Mulai transaction
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    ' Hapus data pelanggan
                    Using cmdDelete As New MySqlCommand("DELETE FROM tbl_pelanggan WHERE kode = @Kode", conn, transaction)
                        cmdDelete.Parameters.AddWithValue("@Kode", TxtKode.Text)
                        cmdDelete.ExecuteNonQuery()
                    End Using

                    ' Commit transaction jika penghapusan berhasil
                    transaction.Commit()

                    ' Catatan aksi
                    DatabaseModule.CatatanAksiHistory("Hapus pelanggan " & TxtNama.Text)

                    ' Refresh form setelah penghapusan
                    Call Kondisiawal()

                Catch ex As Exception
                    ' Rollback transaction jika ada kesalahan
                    transaction.Rollback()
                    MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End If
    End Sub


    Private Sub TxtValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtTotal.TextChanged, TxtAwal.TextChanged, TxtBayar.TextChanged
        Dim awal As Integer
        Dim total As Integer
        Dim bayar As Integer

        If Not Integer.TryParse(TxtAwal.Text, awal) Then
            awal = 0
        End If

        If Integer.TryParse(TxtTotal.Text, total) AndAlso Integer.TryParse(TxtBayar.Text, bayar) Then
            TxtSisa.Text = (awal + total - bayar).ToString()
        End If
    End Sub

    Private Sub TxtAwal_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TxtAwal.KeyPress
        If Not (Char.IsDigit(e.KeyChar) Or e.KeyChar = "." Or e.KeyChar = "," Or e.KeyChar = vbBack) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TambahPelanggan_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
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


    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class