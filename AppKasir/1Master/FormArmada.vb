Imports System.Reflection

Public Class FormArmada

    Private Sub FormArmada_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor
        Dim Armada As Boolean() = ModulHakAkses.BacaHakAkses(FormUtama.SLevel.Text, "Armada", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BTNSimpan.Visible = Armada(1) ' CanAdd 
        'BTNSimpan.Visible = Armada(2) ' CanEdit 
        BTNHapus.Visible = Armada(3) ' CanDelete 


        Kondisiawal()

        Me.Cursor = Cursors.Default
    End Sub

    Public Sub Kondisiawal()
        TxtKode.Clear()
        TxtNopol.Clear()
        TxtJenis.Clear()
        TampilArmada()
        KodeArmada()

        TxtNopol.Focus()
    End Sub

    Public Sub TampilArmada()
        Dim dt As New DataTable()

        Using cmd As New MySqlCommand("SELECT KODE, NOPOL, JENIS FROM tbl_Armada ORDER BY KODE", conn)
            Using rd As New MySqlDataAdapter(cmd)
                rd.Fill(dt)
            End Using
        End Using

        Dgvdata.DataSource = dt
        With Dgvdata
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
            DataGridViewExtension.EnableDoubleBuffering(Dgvdata)
        End With
    End Sub

    Public Class DataGridViewExtension
        Public Shared Sub EnableDoubleBuffering(ByVal dataGridView As DataGridView)
            dataGridView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty, Nothing, dataGridView, New Object() {True})
        End Sub
    End Class

    Public Sub KodeArmada()
        Dim maxKode As String = ""
        Dim existingKodes As New List(Of String)

        ' Mengambil semua kode yang sudah ada dari database
        Using cmd As New MySqlCommand("SELECT KODE FROM tbl_Armada ORDER BY KODE", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        ' Jika tidak ada kode yang ada, gunakan SPL-0001
        If existingKodes.Count = 0 Then
            TxtKode.Text = "ARM-0001"
            Exit Sub
        End If

        ' Mencari nomor berikutnya yang belum terpakai
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = "ARM-" & i.ToString("0000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        ' Jika tidak ada nomor berikutnya yang tersedia, gunakan nomor setelah kode terakhir
        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim Hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 4)) + 1
            maxKode = "ARM-" & Hitung.ToString("0000")
        End If

        TxtKode.Text = maxKode
    End Sub

    Private Sub Dgvdata_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Dgvdata.CellClick
        If Dgvdata.Rows.Count >= 1 AndAlso Dgvdata.CurrentRow IsNot Nothing Then
            TxtKode.Text = If(Convert.IsDBNull(Dgvdata.Item(0, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(0, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtNopol.Text = If(Convert.IsDBNull(Dgvdata.Item(1, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(1, Dgvdata.CurrentRow.Index).Value.ToString())
            TxtJenis.Text = If(Convert.IsDBNull(Dgvdata.Item(2, Dgvdata.CurrentRow.Index).Value), "", Dgvdata.Item(2, Dgvdata.CurrentRow.Index).Value.ToString())
        End If

        TxtNopol.Focus()
    End Sub




    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs)
        Call Kondisiawal()
    End Sub

    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNHapus.Click
        If String.IsNullOrEmpty(TxtKode.Text) Or String.IsNullOrEmpty(TxtNopol.Text) Then
            MessageBox.Show("Pilih data yang akan dihapus", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If MessageBox.Show("Apakah data ini akan dihapus ...???", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using cmd As New MySqlCommand("DELETE FROM tbl_Armada WHERE kode = @Kode", conn)
                    cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                    cmd.ExecuteNonQuery()
                End Using
                Call Kondisiawal()
            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub


    Private Sub BtnSimpan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNSimpan.Click
        ' Validasi input
        If String.IsNullOrWhiteSpace(TxtKode.Text) OrElse String.IsNullOrWhiteSpace(TxtNopol.Text) OrElse String.IsNullOrWhiteSpace(TxtJenis.Text) Then
            MessageBox.Show("Data harus diisi dengan lengkap !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim transaction As MySqlTransaction = Nothing

        Try
            ' Mulai transaksi
            transaction = conn.BeginTransaction()

            Using cmd As New MySqlCommand("SELECT kode FROM tbl_Armada WHERE kode = @Kode", conn, transaction)
                cmd.Parameters.AddWithValue("@Kode", TxtKode.Text)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        rd.Close() ' Explicitly close the reader

                        If MessageBox.Show("Kode Armada sudah ada, Apakah lanjut edit data ...!!!", "Peringatan", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            Using cmdUpdate As New MySqlCommand("UPDATE tbl_Armada SET NOPOL = @NOPOL, JENIS = @JENIS WHERE Kode = @Kode", conn, transaction)
                                cmdUpdate.Parameters.AddWithValue("@NOPOL", StrConv(TxtNopol.Text, vbUpperCase))
                                cmdUpdate.Parameters.AddWithValue("@JENIS", StrConv(TxtJenis.Text, vbProperCase))
                                cmdUpdate.Parameters.AddWithValue("@Kode", TxtKode.Text)
                                cmdUpdate.ExecuteNonQuery()
                            End Using
                        End If
                    Else
                        rd.Close() ' Explicitly close the reader

                        Using cmdInsert As New MySqlCommand("INSERT INTO tbl_Armada (KODE, NOPOL, JENIS) VALUES (@KODE, @NOPOL, @JENIS)", conn, transaction)
                            cmdInsert.Parameters.AddWithValue("@KODE", TxtKode.Text)
                            cmdInsert.Parameters.AddWithValue("@NOPOL", StrConv(TxtNopol.Text, vbUpperCase))
                            cmdInsert.Parameters.AddWithValue("@JENIS", StrConv(TxtJenis.Text, vbProperCase))
                            cmdInsert.ExecuteNonQuery()
                        End Using
                    End If
                End Using
            End Using

            ' Commit transaksi
            transaction.Commit()

            DatabaseModule.CatatanAksiHistory("Tambah/edit Armada " & TxtKode.Text)

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