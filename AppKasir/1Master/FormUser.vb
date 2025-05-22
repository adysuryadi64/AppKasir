Imports System.Reflection

Public Class FormUser

    Private Sub Form_User_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor

        Dim User As Boolean() = ModulHakAkses.BacaHakAkses(FormUtama.SLevel.Text, "User", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BTNSimpan.Visible = User(1) ' CanAdd 
        'BTNSimpan.Visible = User(2) ' CanEdit 
        BTNHapus.Visible = User(3) ' CanDelete 



        Call Tampil_user()
        Call Bersih()

        Me.Cursor = Cursors.Default
    End Sub

    Public Sub Tampil_user()
        Using cmd As New MySqlCommand("select kode_user,nama_user,user_name,pwd,lvl from tbl_user", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvData.Rows.Clear()
                While rd.Read()
                    DgvData.Rows.Add(rd(0), rd(1), rd(2), rd(3), rd(4))
                End While
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

    Public Sub Bersih()
        TxtKode.Clear()
        TxtNama.Clear()
        TxtUsername.Clear()
        TxtPassword.Clear()
        CmbLevel.Text = ""
        TxtPAsswordLama.Clear()
        Label7.Visible = False
        TxtPAsswordLama.Visible = False
        Kodeuser()
        TxtNama.Focus()
    End Sub

    Public Sub Kodeuser()
        Dim maxKode As String = ""
        Dim existingKodes As New List(Of String)

        ' Mengambil semua kode yang sudah ada dari database
        Using cmd As New MySqlCommand("SELECT kode_user FROM tbl_user ORDER BY kode_user", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        ' Jika tidak ada kode yang ada, gunakan US-001
        If existingKodes.Count = 0 Then
            TxtKode.Text = "US-001"
            Exit Sub
        End If

        ' Mencari nomor berikutnya yang belum terpakai
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "US-" & i.ToString("000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        ' Jika tidak ada nomor berikutnya yang tersedia, gunakan nomor setelah kode terakhir
        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim Hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 3)) + 1
            maxKode = "US-" & Hitung.ToString("000")
        End If

        TxtKode.Text = maxKode

    End Sub

    Public Shared Function MD5DELISMAN(ByVal strToHash As String) As String
        Using MD5HULU As New System.Security.Cryptography.MD5CryptoServiceProvider()
            Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)

            ' ComputeHash should be wrapped in a Using block to ensure proper resource disposal.
            Using md5Hash = MD5HULU
                bytesToHash = md5Hash.ComputeHash(bytesToHash)
            End Using

            Dim strResult As String = ""
            Dim b As Byte

            For Each b In bytesToHash
                strResult += b.ToString("x2")
            Next

            Return strResult
        End Using
    End Function



    Private Sub TxtKode_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtKode.TextChanged
        ' Memanggil data user berdasarkan kode
        Using cmd As New MySqlCommand("SELECT kode_user, nama_user, user_name, pwd, lvl FROM tbl_user WHERE kode_user = @kode", conn)
            cmd.Parameters.AddWithValue("@kode", TxtKode.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TxtNama.Text = rd.GetString(1)
                    TxtUsername.Text = rd.GetString(2)
                    CmbLevel.Text = rd.GetString(4)
                Else
                    TxtNama.Clear()
                    TxtUsername.Clear()
                    TxtPassword.Clear()
                    CmbLevel.Text = ""
                End If
            End Using
        End Using
    End Sub


    Private Sub BTNSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        If String.IsNullOrWhiteSpace(TxtKode.Text) Or String.IsNullOrWhiteSpace(TxtNama.Text) Or String.IsNullOrWhiteSpace(TxtUsername.Text) Or String.IsNullOrWhiteSpace(TxtPassword.Text) Or String.IsNullOrWhiteSpace(CmbLevel.Text) Then
            MessageBox.Show("Semua data wajib diisi")
        Else
            Using cmd As New MySqlCommand("SELECT kode_user FROM tbl_user WHERE kode_user = @kode", conn)
                cmd.Parameters.AddWithValue("@kode", TxtKode.Text)
                rd = cmd.ExecuteReader()
                rd.Read()

                If Not rd.HasRows Then
                    rd.Close() ' Pastikan DataReader ditutup sebelum melanjutkan
                    ' Panggil sub untuk melakukan insert dengan transaksi
                    InsertUser()
                Else
                    rd.Close() ' Tutup DataReader sebelum memulai update
                    Label7.Visible = True
                    TxtPAsswordLama.Visible = True

                    If String.IsNullOrWhiteSpace(TxtPAsswordLama.Text) Then
                        MessageBox.Show("Isi Password lama")
                        TxtPAsswordLama.Focus()
                        Exit Sub
                    End If
                    ' Panggil sub untuk melakukan update dengan transaksi
                    UpdateUser()
                End If
            End Using
        End If
    End Sub

    Private Sub InsertUser()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Using cmdCheckNama As New MySqlCommand("SELECT user_name FROM tbl_user WHERE user_name = @user_name", conn, transaction)
                cmdCheckNama.Parameters.AddWithValue("@user_name", TxtUsername.Text)
                If cmdCheckNama.ExecuteScalar() IsNot Nothing Then
                    MessageBox.Show("Username user sudah ada, silahkan ganti dengan yang lain !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    TxtUsername.Focus()
                    transaction.Rollback()
                Else
                    Using insertCmd As New MySqlCommand("INSERT INTO tbl_user (kode_user, nama_user, user_name, pwd, lvl) VALUES (@kode, @nama, @username, @password, @level)", conn, transaction)
                        insertCmd.Parameters.AddWithValue("@kode", TxtKode.Text)
                        insertCmd.Parameters.AddWithValue("@nama", StrConv(TxtNama.Text, vbProperCase))
                        insertCmd.Parameters.AddWithValue("@username", StrConv(TxtUsername.Text, vbProperCase))
                        insertCmd.Parameters.AddWithValue("@password", MD5DELISMAN(TxtPassword.Text))
                        insertCmd.Parameters.AddWithValue("@level", CmbLevel.Text)
                        insertCmd.ExecuteNonQuery()
                    End Using


                    transaction.Commit()
                    DatabaseModule.CatatanAksiHistory("Simpan user " & TxtNama.Text)
                    Call Bersih()
                    Call Tampil_user()
                End If
            End Using
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Gagal menyimpan data: " & ex.Message)
        End Try
    End Sub

    Private Sub UpdateUser()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Using updateCmd As New MySqlCommand("UPDATE tbl_user SET nama_user = @nama, user_name = @username, pwd = @password, lvl = @level WHERE kode_user = @kode", conn, transaction)
                updateCmd.Parameters.AddWithValue("@nama", StrConv(TxtNama.Text, vbProperCase))
                updateCmd.Parameters.AddWithValue("@username", StrConv(TxtUsername.Text, vbProperCase))
                updateCmd.Parameters.AddWithValue("@password", MD5DELISMAN(TxtPassword.Text))
                updateCmd.Parameters.AddWithValue("@level", CmbLevel.Text)
                updateCmd.Parameters.AddWithValue("@kode", TxtKode.Text)
                updateCmd.ExecuteNonQuery()
            End Using

            transaction.Commit()
            DatabaseModule.CatatanAksiHistory("Update user " & TxtNama.Text)
            Call Bersih()
            Call Tampil_user()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Gagal mengupdate data: " & ex.Message)
        End Try
    End Sub

    Private Sub BTNHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNHapus.Click
        If MessageBox.Show("Apakah data akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Using cmd As New MySqlCommand("DELETE FROM tbl_user WHERE kode_user = @kode", conn)
                cmd.Parameters.AddWithValue("@kode", TxtKode.Text)
                cmd.ExecuteNonQuery()
            End Using
            MessageBox.Show("Data berhasil dihapus")
            DatabaseModule.CatatanAksiHistory("Hapus user " & TxtNama.Text)
            Call Bersih()
            Call Tampil_user()
        End If
    End Sub



    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Call Bersih()
    End Sub

    Private Sub BTNKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNKeluar.Click
        Close()
    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellClick
        TxtKode.Text = DgvData.Item(0, DgvData.CurrentRow.Index).Value
        Label7.Visible = True
        TxtPAsswordLama.Visible = True
    End Sub

    Private Sub Form_User_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                BTNSimpan.PerformClick()
            Case Keys.F3
                BTNHapus.PerformClick()
            Case Keys.F4
                BtnTambah.PerformClick()
            Case Keys.Escape
                BTNKeluar.PerformClick()
        End Select
    End Sub

End Class