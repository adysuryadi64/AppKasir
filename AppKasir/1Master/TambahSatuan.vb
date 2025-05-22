Imports System.Reflection

Public Class TambahSatuan
    Public Sub Kondisiawal()
        TxtKode.Text = ""
        TxtNama.Text = ""
        TxtIsi.Text = ""
        Call TampilSatuan()
    End Sub

    Public Sub TampilSatuan()
        Using cmd As New MySqlCommand("SELECT kode, nama, isi FROM tbl_satuan Order By isi", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvData.Rows.Clear()
                While rd.Read()
                    DgvData.Rows.Add(rd(0), rd(1), rd(2))
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

    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub TambahSatuan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        Kondisiawal()
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub DgvData_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellClick
        If DgvData.Rows.Count < 1 Then
            MessageBox.Show("Tidak ada data !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            TxtKode.Text = DgvData.Item(0, DgvData.CurrentRow.Index).Value
            TxtNama.Text = DgvData.Item(1, DgvData.CurrentRow.Index).Value
            TxtIsi.Text = DgvData.Item(2, DgvData.CurrentRow.Index).Value
        End If
    End Sub

    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnHapus.Click
        If String.IsNullOrEmpty(TxtKode.Text) Then
            MessageBox.Show("Pilih data yang akan dihapus !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            ' Mulai transaksi
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    ' Hapus data berdasarkan kode
                    Dim query As String = "DELETE FROM tbl_satuan WHERE kode = @Kode"
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' Commit transaksi jika berhasil
                    transaction.Commit()

                    ' Catat aksi penghapusan
                    DatabaseModule.CatatanAksiHistory("Hapus satuan " & TxtNama.Text)

                    ' Refresh form setelah penghapusan
                    Call Kondisiawal()

                Catch ex As Exception
                    ' Rollback transaksi jika ada kesalahan
                    transaction.Rollback()
                    MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End If
    End Sub

    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSimpan.Click
        Dim kode As String = TxtKode.Text.Trim()
        Dim nama As String = TxtNama.Text.Trim()
        Dim isi As Integer

        ' Validasi input kosong dan pastikan 'isi' adalah angka
        If String.IsNullOrEmpty(kode) Or String.IsNullOrEmpty(nama) Or Not Integer.TryParse(TxtIsi.Text.Trim(), isi) Then
            MessageBox.Show("Data harus diisi dengan lengkap dan isi harus berupa angka !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Mulai transaksi
        Using transaction As MySqlTransaction = conn.BeginTransaction()
            Try
                ' Cek apakah kode sudah ada
                Dim kodeExists As Boolean = CheckKodeExists(kode, transaction)
                If kodeExists Then
                    ' Jika kode sudah ada, konfirmasi pembaruan data
                    If MessageBox.Show("Kode Satuan sudah ada, Apakah lanjut edit data ...!!!", "Peringatan", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                        UpdateSatuan(kode, nama, isi, transaction)
                    End If
                Else
                    ' Jika kode belum ada, tambahkan data baru
                    InsertSatuan(kode, nama, isi, transaction)
                End If

                ' Commit transaksi jika berhasil
                transaction.Commit()
                DatabaseModule.CatatanAksiHistory("Update satuan " & nama)
                Call Kondisiawal()

            Catch ex As Exception
                ' Rollback transaksi jika ada kesalahan
                transaction.Rollback()
                MessageBox.Show("Error: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Function CheckKodeExists(kode As String, transaction As MySqlTransaction) As Boolean
        Dim queryCheck As String = "SELECT kode FROM tbl_satuan WHERE kode = @Kode"
        Using cmdCheck As New MySqlCommand(queryCheck, conn, transaction)
            cmdCheck.Parameters.AddWithValue("@Kode", kode)
            Using reader As MySqlDataReader = cmdCheck.ExecuteReader()
                Return reader.HasRows
            End Using
        End Using
    End Function

    Private Sub UpdateSatuan(kode As String, nama As String, isi As Integer, transaction As MySqlTransaction)
        Dim updateQuery As String = "UPDATE tbl_satuan SET nama = @Nama, isi = @Isi WHERE kode = @Kode"
        Using updateCmd As New MySqlCommand(updateQuery, conn, transaction)
            updateCmd.Parameters.AddWithValue("@Nama", StrConv(nama, vbProperCase))
            updateCmd.Parameters.AddWithValue("@Isi", isi)
            updateCmd.Parameters.AddWithValue("@Kode", kode)
            updateCmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InsertSatuan(kode As String, nama As String, isi As Integer, transaction As MySqlTransaction)
        Dim insertQuery As String = "INSERT INTO tbl_satuan (kode, nama, isi) VALUES(@Kode, @Nama, @Isi)"
        Using insertCmd As New MySqlCommand(insertQuery, conn, transaction)
            insertCmd.Parameters.AddWithValue("@Kode", StrConv(kode, vbProperCase))
            insertCmd.Parameters.AddWithValue("@Nama", StrConv(nama, vbProperCase))
            insertCmd.Parameters.AddWithValue("@Isi", isi)
            insertCmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub TambahSatuan_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
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


    Private Sub BtnBaru_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBaru.Click
        Kondisiawal()
    End Sub
End Class