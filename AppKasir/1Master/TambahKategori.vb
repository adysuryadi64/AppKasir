Imports System.Reflection
Imports System.Text

Public Class TambahKategori

    Private Sub TambahKategori_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        Call Kondisiawal()
        Me.Cursor = Cursors.Default
    End Sub

    Public Sub Tampilkategori()
        Dim dt As New DataTable()

        Using cmd As New MySqlCommand("SELECT kode, nama, jenis FROM tbl_kategori ORDER BY nama", conn),
      da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        DgvData.DataSource = dt


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

    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        If String.IsNullOrWhiteSpace(TxtKode.Text) OrElse
       String.IsNullOrWhiteSpace(TxtNama.Text) OrElse
       String.IsNullOrWhiteSpace(TxtJenis.Text) Then
            MessageBox.Show("Data harus diisi dengan lengkap !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim kode = StrConv(TxtKode.Text.Trim(), vbUpperCase)
        Dim nama = StrConv(TxtNama.Text.Trim(), vbProperCase)
        Dim jenis = StrConv(TxtJenis.Text.Trim(), vbProperCase)

        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' Cek apakah kode sudah ada
            Dim kodeExists As Boolean = False
            Using cmd As New MySqlCommand("SELECT 1 FROM tbl_kategori WHERE kode = @Kode LIMIT 1", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", kode)
                Using rd = cmd.ExecuteReader()
                    kodeExists = rd.Read()
                End Using
            End Using

            If kodeExists Then
                If MessageBox.Show("Kode kategori sudah ada. Apakah ingin mengedit data?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    ' Update
                    Using updateCmd As New MySqlCommand("UPDATE tbl_kategori SET nama = @Nama, jenis = @Jenis WHERE kode = @Kode", conn, transaction)
                        updateCmd.Parameters.AddWithValue("@Nama", nama)
                        updateCmd.Parameters.AddWithValue("@Jenis", jenis)
                        updateCmd.Parameters.AddWithValue("@Kode", kode)
                        updateCmd.ExecuteNonQuery()
                    End Using
                    DatabaseModule.CatatanAksiHistory("Update kategori " & nama)
                    transaction.Commit()
                    Kondisiawal()
                End If
                Exit Sub
            End If

            ' Cek apakah nama kategori sudah ada
            Dim namaExists As Boolean = False
            Using cmd As New MySqlCommand("SELECT 1 FROM tbl_kategori WHERE nama = @Nama LIMIT 1", conn, transaction)
                cmd.Parameters.AddWithValue("@Nama", nama)
                Using rd = cmd.ExecuteReader()
                    namaExists = rd.Read()
                End Using
            End Using

            If namaExists Then
                MessageBox.Show("Nama kategori sudah ada, silakan ganti dengan yang lain.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TxtNama.Focus()
                transaction.Rollback()
                Exit Sub
            End If

            ' Insert
            Using insertCmd As New MySqlCommand("INSERT INTO tbl_kategori (kode, nama, jenis) VALUES (@Kode, @Nama, @Jenis)", conn, transaction)
                insertCmd.Parameters.AddWithValue("@Kode", kode)
                insertCmd.Parameters.AddWithValue("@Nama", nama)
                insertCmd.Parameters.AddWithValue("@Jenis", jenis)
                insertCmd.ExecuteNonQuery()
            End Using
            DatabaseModule.CatatanAksiHistory("Tambah kategori " & nama)
            transaction.Commit()
            Kondisiawal()

        Catch ex As Exception
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