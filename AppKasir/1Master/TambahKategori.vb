Imports System.Reflection
Imports System.Text

Public Class TambahKategori

    Private Sub TambahKategori_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        Call Kondisiawal()
        Me.Cursor = Cursors.Default
    End Sub

    Public Sub Tampilkategori()
        Using cmd As New MySqlCommand("select kode, nama, jenis from tbl_kategori", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvData.Rows.Clear()
                Do While rd.Read()
                    DgvData.Rows.Add(rd(0), rd(1), rd(2))
                Loop
            End Using
        End Using

        With DgvData
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False


            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Gray
            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            DataGridViewExtension.EnableDoubleBuffering(DgvData)
        End With
    End Sub



    Public Class DataGridViewExtension
        Public Shared Sub EnableDoubleBuffering(ByVal dataGridView As DataGridView)
            dataGridView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty, Nothing, dataGridView, New Object() {True})
        End Sub
    End Class


    Public Sub Kondisiawal()
        TxtKode.Text = ""
        TxtNama.Text = ""
        TxtJenis.Text = "Barang"
        Tampilkategori()
    End Sub
    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpan.Click
        If TxtKode.Text = "" Or TxtNama.Text = "" Or TxtJenis.Text = "" Then
            MessageBox.Show("Data harus diisi dengan lengkap !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Memulai transaksi
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Using cmd As New MySqlCommand("SELECT kode FROM tbl_kategori WHERE kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() AndAlso rd.HasRows Then
                        If MessageBox.Show("Kode kategori sudah ada. Apakah ingin melanjutkan untuk mengedit data?", "Peringatan", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                            rd.Close() ' Menutup reader sebelum melanjutkan

                            Using updateCmd As New MySqlCommand("UPDATE tbl_kategori SET nama = @Nama, jenis = @Jenis WHERE kode = @Kode", conn, transaction)
                                updateCmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbProperCase))
                                updateCmd.Parameters.AddWithValue("@Jenis", StrConv(TxtJenis.Text, vbProperCase))
                                updateCmd.Parameters.AddWithValue("@Kode", StrConv(TxtKode.Text, vbProperCase))
                                updateCmd.ExecuteNonQuery()
                                DatabaseModule.CatatanAksiHistory("Update kategori " & TxtNama.Text)
                            End Using

                            transaction.Commit() ' Menyimpan perubahan jika sukses
                            Call Kondisiawal()
                            Exit Sub
                        Else
                            rd.Close() ' Menutup reader jika pengguna memilih tidak
                            Exit Sub
                        End If
                    End If
                    rd.Close() ' Menutup reader jika tidak ada hasil
                End Using

                ' Memeriksa jika nama kategori sudah ada
                Using checkCmd As New MySqlCommand("SELECT nama FROM tbl_kategori WHERE nama = @Nama", conn, transaction)
                    checkCmd.Parameters.AddWithValue("@Nama", TxtNama.Text)
                    Using checkReader As MySqlDataReader = checkCmd.ExecuteReader()
                        If checkReader.Read() AndAlso checkReader.HasRows Then
                            MessageBox.Show("Nama kategori sudah ada, silahkan ganti dengan yang lain !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            TxtNama.Focus()
                            checkReader.Close() ' Menutup reader
                            transaction.Rollback() ' Membatalkan perubahan jika terjadi kesalahan
                            Exit Sub
                        End If
                        checkReader.Close() ' Menutup reader setelah selesai
                    End Using
                End Using

                ' Menyimpan data baru
                Using insertCmd As New MySqlCommand("INSERT INTO tbl_kategori VALUES(@Kode, @Nama, @Jenis)", conn, transaction)
                    insertCmd.Parameters.AddWithValue("@Kode", StrConv(TxtKode.Text, vbUpperCase))
                    insertCmd.Parameters.AddWithValue("@Nama", StrConv(TxtNama.Text, vbProperCase))
                    insertCmd.Parameters.AddWithValue("@Jenis", StrConv(TxtJenis.Text, vbProperCase))
                    insertCmd.ExecuteNonQuery()
                    DatabaseModule.CatatanAksiHistory("Tambah kategori " & TxtNama.Text)
                End Using

                transaction.Commit() ' Menyimpan perubahan
                Call Kondisiawal()
            End Using
        Catch ex As Exception
            ' Jika terjadi kesalahan, membatalkan transaksi
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnHapus.Click
        If TxtKode.Text = "" Or TxtNama.Text = "" Or TxtJenis.Text = "" Then
            MessageBox.Show("Pilih data yang akan dihapus !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Using cmd As New MySqlCommand("DELETE FROM tbl_kategori WHERE kode = @Kode", conn)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                cmd.ExecuteNonQuery()
                DatabaseModule.CatatanAksiHistory("Hapus kategori " & TxtNama.Text)
                Call Kondisiawal()
            End Using
        End If
    End Sub

    Private Sub DgvData_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellClick
        TxtKode.Text = If(DgvData.CurrentRow IsNot Nothing AndAlso DgvData.CurrentRow.Cells(0).Value IsNot Nothing, DgvData.Item(0, DgvData.CurrentRow.Index).Value.ToString(), "")
        TxtNama.Text = If(DgvData.CurrentRow IsNot Nothing AndAlso DgvData.CurrentRow.Cells(1).Value IsNot Nothing, DgvData.Item(1, DgvData.CurrentRow.Index).Value.ToString(), "")
        TxtJenis.Text = If(DgvData.CurrentRow IsNot Nothing AndAlso DgvData.CurrentRow.Cells(2).Value IsNot Nothing, DgvData.Item(2, DgvData.CurrentRow.Index).Value.ToString(), "")
    End Sub

    Private Sub BtnBaru_Click(sender As Object, e As EventArgs) Handles BtnBaru.Click
        Kondisiawal()
    End Sub

    Private Sub TambahKategori_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                BtnSimpan.PerformClick()
            Case Keys.F3
                BtnHapus.PerformClick()
            Case Keys.F4
                BtnBaru.PerformClick()
            Case Keys.Escape
                BtnClose.PerformClick()
        End Select
    End Sub

    Private Sub TxtNama_TextChanged(sender As Object, e As EventArgs) Handles TxtNama.TextChanged
        ' Panggil fungsi untuk mengupdate TextBox2 ketika TextBox1 berubah
        UpdateKodeFromNama()
    End Sub

    Private Sub UpdateKodeFromNama()
        Dim nama As String = TxtNama.Text
        Dim kode As String = GenerateUniqueKode(nama, 3)
        TxtKode.Text = kode
    End Sub

    Private Function GenerateUniqueKode(nama As String, length As Integer) As String
        Dim uniqueKode As String = GenerateRandomKode(nama, length)

        ' Periksa apakah kode sudah ada di tabel
        While IsKodeExists(uniqueKode)
            uniqueKode = GenerateRandomKode(nama, length) ' Generate kode baru
        End While

        Return uniqueKode
    End Function

    Private Function GenerateRandomKode(nama As String, length As Integer) As String
        Dim random As New Random()
        Dim kode As New StringBuilder()
        Dim uniqueChars As New List(Of Char)()

        ' Mengambil karakter pertama dari setiap kata
        Dim words As String() = nama.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
        For Each word As String In words
            If word.Length > 0 Then
                Dim firstChar As Char = Char.ToUpper(word(0))
                If Not uniqueChars.Contains(firstChar) Then
                    uniqueChars.Add(firstChar) ' Tambahkan huruf pertama jika belum ada
                End If
            End If
        Next

        ' Menambahkan karakter unik dari sisa nama
        For Each ch As Char In nama
            If Not Char.IsWhiteSpace(ch) AndAlso Not uniqueChars.Contains(Char.ToUpper(ch)) Then
                uniqueChars.Add(Char.ToUpper(ch)) ' Tambahkan karakter jika belum ada
            End If
        Next

        ' Pastikan panjang yang diminta tidak lebih besar dari jumlah karakter unik yang ada
        If length > uniqueChars.Count Then
            length = uniqueChars.Count ' Batasi panjang ke jumlah karakter unik yang ada
        End If

        ' Memastikan kode yang dihasilkan tidak memiliki huruf ganda
        Dim usedChars As New HashSet(Of Char)() ' Menggunakan HashSet untuk cek unik lebih efisien

        ' Memilih karakter secara acak untuk membentuk kode
        While kode.Length < length
            Dim randomIndex As Integer = random.Next(0, uniqueChars.Count)
            Dim randomChar As Char = uniqueChars(randomIndex)

            If Not usedChars.Contains(randomChar) Then
                kode.Append(randomChar)
                usedChars.Add(randomChar) ' Tandai huruf telah digunakan
            End If

            ' Jika sudah menggunakan semua karakter unik yang ada
            If usedChars.Count >= uniqueChars.Count Then
                Exit While ' Keluar dari loop jika semua karakter telah digunakan
            End If
        End While

        ' Mengembalikan kode yang dihasilkan
        Return kode.ToString()
    End Function

    Private Function IsKodeExists(kode As String) As Boolean
        Dim exists As Boolean = False
        Dim query As String = "SELECT COUNT(KODE) FROM tbl_kategori WHERE KODE = @KODE"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@KODE", kode)
            exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0
        End Using

        Return exists
    End Function

End Class