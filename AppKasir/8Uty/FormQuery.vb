Imports System.IO
Imports System.Text.Json

Public Class FormQuery

    Private Sub BtnEksekusi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEksekusi.Click
        Dim query As String = RtbQuery.Text

        Try
            ' Hapus hasil sebelumnya di ListBox
            ListBoxHasil.Items.Clear()

            ' Periksa apakah query adalah SELECT atau bukan
            If query.Trim().ToUpper().StartsWith("SELECT") Then
                ' Gunakan DataAdapter untuk mengeksekusi query SELECT
                Using cmd As New MySqlCommand(query, conn)
                    Dim reader As MySqlDataReader = cmd.ExecuteReader()

                    ' Tampilkan hasil dari query SELECT ke ListBox
                    While reader.Read()
                        ' Ambil semua kolom dari baris hasil dan gabungkan ke satu string
                        Dim rowData As String = ""
                        For i As Integer = 0 To reader.FieldCount - 1
                            rowData &= reader.GetValue(i).ToString() & vbTab
                        Next
                        ' Tambahkan baris hasil ke ListBox
                        ListBoxHasil.Items.Add(rowData.Trim())
                    End While
                End Using
            Else
                ' Tampilkan peringatan bahaya sebelum eksekusi query non-SELECT
                Dim result As DialogResult = MessageBox.Show("Perubahan ini akan langsung berdampak pada database dan tidak bisa dikembalikan. Apakah Anda yakin ingin melanjutkan?", "Peringatan Bahaya", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

                ' Jika pengguna memilih 'Yes', lanjutkan eksekusi query
                If result = DialogResult.Yes Then
                    ' Eksekusi query non-SELECT (INSERT, UPDATE, DELETE)
                    Using cmd As New MySqlCommand(query, conn)
                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                        ' Tampilkan informasi query yang dieksekusi di ListBox
                        ListBoxHasil.Items.Add("Query executed: " & query)

                        ' Tampilkan jumlah baris yang terpengaruh
                        ListBoxHasil.Items.Add(rowsAffected.ToString() & " row(s) affected.")

                        ' Tambahkan detail lebih lanjut, jika diperlukan (misalnya, jenis operasi)
                        If query.Trim().ToUpper().StartsWith("INSERT") Then
                            ListBoxHasil.Items.Add("Insert operation completed.")
                        ElseIf query.Trim().ToUpper().StartsWith("UPDATE") Then
                            ListBoxHasil.Items.Add("Update operation completed.")
                        ElseIf query.Trim().ToUpper().StartsWith("DELETE") Then
                            ListBoxHasil.Items.Add("Delete operation completed.")
                        End If
                    End Using
                Else
                    ' Batalkan eksekusi query
                    ListBoxHasil.Items.Add("Eksekusi dibatalkan oleh pengguna.")
                End If
            End If
        Catch ex As Exception
            ' Tampilkan pesan error jika ada kesalahan
            ListBoxHasil.Items.Add("Error: " & ex.Message)
        End Try
    End Sub


    Private Sub FormQuery_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TampilTabel()
    End Sub


    Private Sub TampilTabel()
        ' Query untuk mendapatkan nama tabel dalam database
        Dim query As String = "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA = @DatabaseSchema;"

        ' Kosongkan ListBoxTabel sebelum menambahkan hasil baru
        ListBoxTabel.Items.Clear()

        Dim konfigurasi As DatabaseConfiguration
        ' Membaca konfigurasi dari file biner
        If Not File.Exists(configFilePath) Then
            MessageBox.Show("File konfigurasi tidak ditemukan!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If


        Dim json As String = File.ReadAllText(configFilePath)
        konfigurasi = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)

        Dim database As String = konfigurasi.Database ' Ganti dengan nama database Anda

        ' Eksekusi query dan tambahkan hasil ke ListBoxTabel
        Using cmd As New MySqlCommand(query, conn)
            ' Tambahkan parameter untuk keamanan
            cmd.Parameters.AddWithValue("@DatabaseSchema", database)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    ListBoxTabel.Items.Add(reader("TABLE_NAME").ToString())
                End While
            End Using
        End Using
    End Sub




    ' Ketika pengguna mengklik nama tabel pada ListBoxTabel
    Private Sub ListBoxTabel_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ListBoxTabel.SelectedIndexChanged
        ' Ambil nama tabel yang dipilih dari ListBoxTabel
        Dim selectedTable As String = ListBoxTabel.SelectedItem.ToString()

        Try
            ' Hapus hasil sebelumnya di ListBoxKolom
            ListBoxKolom.Items.Clear()

            ' Query untuk mendapatkan nama-nama kolom dari tabel yang dipilih
            Dim query As String = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = @DatabaseSchema AND TABLE_NAME = @tableName;"

            ' Eksekusi query untuk mendapatkan nama kolom
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@DatabaseSchema", "databasekl")
                cmd.Parameters.AddWithValue("@tableName", selectedTable)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()

                ' Tambahkan setiap nama kolom ke ListBoxKolom
                While reader.Read()
                    ListBoxKolom.Items.Add(reader("COLUMN_NAME").ToString())
                End While
            End Using
        Catch ex As Exception
            ' Tampilkan pesan error jika ada kesalahan
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub


    ' Event untuk menampilkan ContextMenuStrip pada ListBoxTabel
    Private Sub ListBoxTabel_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ListBoxTabel.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim index As Integer = ListBoxTabel.IndexFromPoint(e.Location)
            If index <> -1 Then
                ListBoxTabel.SelectedIndex = index
                ContextMenuStrip1.Show(ListBoxTabel, e.Location)
            End If
        End If
    End Sub

    ' Event untuk menampilkan ContextMenuStrip pada ListBoxKolom
    Private Sub ListBoxKolom_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ListBoxKolom.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim index As Integer = ListBoxKolom.IndexFromPoint(e.Location)
            If index <> -1 Then
                ListBoxKolom.SelectedIndex = index
                ContextMenuStrip1.Show(ListBoxKolom, e.Location)
            End If
        End If
    End Sub

    ' Tombol untuk menyalin nama tabel atau kolom yang dipilih ke clipboard
    Private Sub CopyToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CopyToolStripMenuItem.Click
        ' Cek apakah item dipilih di ListBoxTabel
        If ListBoxTabel.Focused Then ' Memastikan ListBoxTabel yang aktif
            If ListBoxTabel.SelectedIndex <> -1 Then
                Dim selectedTable As String = ListBoxTabel.SelectedItem.ToString()
                Clipboard.SetText(selectedTable)
                'MessageBox.Show("Tabel '" & selectedTable & "' disalin ke clipboard.", "Tabel Disalin", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            ' Cek apakah item dipilih di ListBoxKolom
        ElseIf ListBoxKolom.Focused Then ' Memastikan ListBoxKolom yang aktif
            If ListBoxKolom.SelectedIndex <> -1 Then
                Dim selectedColumn As String = ListBoxKolom.SelectedItem.ToString()
                Clipboard.SetText(selectedColumn)
                'MessageBox.Show("Kolom '" & selectedColumn & "' disalin ke clipboard.", "Kolom Disalin", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Else
            MessageBox.Show("Silakan pilih tabel atau kolom yang ingin disalin.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Private Sub RtbQuery_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles RtbQuery.KeyUp
        ' Periksa apakah tombol yang ditekan adalah spasi
        If e.KeyCode = Keys.Space Then
            ' Simpan posisi kursor agar tidak berpindah saat melakukan perubahan warna
            Dim currentPosition As Integer = RtbQuery.SelectionStart

            ' Simpan teks asli
            Dim originalText As String = RtbQuery.Text

            ' Reset format teks ke warna default
            RtbQuery.SelectAll()
            RtbQuery.SelectionColor = Color.Black

            Dim sqlKeywords As New Dictionary(Of String, Color) From {
    {"SELECT", Color.Blue},
    {"FROM", Color.Blue},
    {"WHERE", Color.Blue},
    {"INSERT", Color.Green},
    {"INTO", Color.Green},
    {"VALUES", Color.Green},
    {"UPDATE", Color.Purple},
    {"SET", Color.Purple},
    {"DELETE", Color.Red},
    {"LIKE", Color.Orange},
    {"AND", Color.Blue},
    {"OR", Color.Blue},
    {"NOT", Color.Blue}
}


            ' Pecah teks menjadi array kata
            Dim words() As String = originalText.Split({" "c, vbTab, vbCrLf}, StringSplitOptions.None)

            ' Looping untuk setiap kata dan berikan warna jika sesuai
            Dim index As Integer = 0
            For Each word As String In words
                ' Pilih teks di RichTextBox
                index = originalText.IndexOf(word, index)
                RtbQuery.Select(index, word.Length)

                ' Berikan warna jika kata dikenali sebagai perintah SQL
                If sqlKeywords.ContainsKey(word.ToUpper()) Then
                    RtbQuery.SelectionColor = sqlKeywords(word.ToUpper())
                Else
                    ' Atur kembali ke warna hitam jika bukan perintah SQL
                    RtbQuery.SelectionColor = Color.Black
                End If

                index += word.Length
            Next

            ' Kembalikan posisi kursor
            RtbQuery.SelectionStart = currentPosition
            RtbQuery.SelectionLength = 0
        End If
    End Sub


End Class