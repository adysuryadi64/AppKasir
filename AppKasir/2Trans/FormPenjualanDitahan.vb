Public Class FormPenjualanDitahan
    Private Sub Form_PenjualanDitahan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        AmbilData()

    End Sub

    Private Sub AmbilData()
        TxtFaktur.Clear()
        TxtPel.Clear()
        Using cmd As New MySqlCommand("SELECT FAKTUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, TANGGAL_JUAL, GRAN_TOTAL, TOTAL_ITEM, ID_USER FROM penjualan_ditahan", conn)
            Using adapter As New MySqlDataAdapter(cmd)
                Using dt As New DataTable()
                    adapter.Fill(dt)
                    DgvData.DataSource = dt
                    AturDatagridview()
                    ' Pilih baris pertama jika ada data
                    If DgvData.Rows.Count > 0 Then
                        DgvData.Rows(0).Selected = True
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub AturDatagridview()
        ' Hapus kolom tombol jika sudah ada agar tidak ganda
        If DgvData.Columns.Contains("ColHapus") Then
            DgvData.Columns.Remove("ColHapus")
        End If

        With DgvData
            .ReadOnly = True
            .Columns(1).Visible = False

            .Columns(0).HeaderText = "NOMOR"
            .Columns(2).HeaderText = "PELANGGAN"
            .Columns(3).HeaderText = "TANGGAL"
            .Columns(4).HeaderText = "HARGA"
            .Columns(5).HeaderText = "ITEM"
            .Columns(6).HeaderText = "USER"

            .Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(4).DefaultCellStyle.Format = "N0"

            .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(5).DefaultCellStyle.Format = "N0"

            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
        End With

        ' Tambahkan ulang kolom tombol hapus
        Dim btnCol As New DataGridViewButtonColumn()
        btnCol.Name = "ColHapus"
        btnCol.HeaderText = "HAPUS"
        btnCol.Text = "Hapus"
        btnCol.UseColumnTextForButtonValue = True
        btnCol.FillWeight = 50
        btnCol.DefaultCellStyle.BackColor = Color.Red
        btnCol.DefaultCellStyle.ForeColor = Color.White
        DgvData.Columns.Add(btnCol)
    End Sub



    Private Sub BtnProses_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnProses.Click
        If String.IsNullOrEmpty(TxtFaktur.Text) Then
            MessageBox.Show("Pilih Data yang akan diambil !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Try
                ' Set nilai pada FormPenjualan jika transaksi berhasil
                FormPenjualan.CmbPelanggan.Text = TxtPel.Text
                FormPenjualan.TxtFaktur.Text = TxtFaktur.Text
                FormPenjualan.TxtJenistransaksi.Text = "TambahPenjualan"
                FormPenjualan.AmbilDataDitahan()

                ' Mulai transaksi
                Using transaction As MySqlTransaction = conn.BeginTransaction()
                    ' Setel transaksi untuk perintah
                    Using cmdDeletePenjualanDitahan As New MySqlCommand("DELETE FROM penjualan_ditahan WHERE FAKTUR_JUAL = @faktur", conn, transaction)
                        cmdDeletePenjualanDitahan.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
                        cmdDeletePenjualanDitahan.ExecuteNonQuery()
                    End Using

                    Using cmdDeleteTempPenjualanDitahan As New MySqlCommand("DELETE FROM penjualan_ditahan_detail WHERE FAKTUR_JUAL = @faktur", conn, transaction)
                        cmdDeleteTempPenjualanDitahan.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
                        cmdDeleteTempPenjualanDitahan.ExecuteNonQuery()
                    End Using

                    ' Commit transaksi
                    transaction.Commit()

                    ' Tutup form
                    Close()

                End Using

            Catch ex As Exception
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub




    Private Sub DgvData_CellClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DgvData.CellClick
        ' Cek jika klik terjadi pada baris data valid
        If e.RowIndex < 0 OrElse e.RowIndex >= DgvData.Rows.Count Then Exit Sub

        Dim row As DataGridViewRow = DgvData.Rows(e.RowIndex)

        ' Ambil nilai faktur dan pelanggan dari baris yang diklik
        TxtFaktur.Text = row.Cells("FAKTUR_JUAL").Value.ToString()
        TxtPel.Text = row.Cells("NAMA_PELANGGAN").Value.ToString()

        ' Jika kolom yang diklik adalah tombol hapus
        If DgvData.Columns(e.ColumnIndex).Name = "ColHapus" Then
            Dim faktur As String = row.Cells("FAKTUR_JUAL").Value.ToString()

            If MessageBox.Show($"Yakin ingin menghapus data dengan faktur: {faktur}?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    Using transaction As MySqlTransaction = conn.BeginTransaction()
                        ' Hapus dari tabel utama
                        Using cmd1 As New MySqlCommand("DELETE FROM penjualan_ditahan WHERE FAKTUR_JUAL = @faktur", conn, transaction)
                            cmd1.Parameters.AddWithValue("@faktur", faktur)
                            cmd1.ExecuteNonQuery()
                        End Using

                        ' Hapus dari detail
                        Using cmd2 As New MySqlCommand("DELETE FROM penjualan_ditahan_detail WHERE FAKTUR_JUAL = @faktur", conn, transaction)
                            cmd2.Parameters.AddWithValue("@faktur", faktur)
                            cmd2.ExecuteNonQuery()
                        End Using

                        transaction.Commit()
                    End Using

                    ' Refresh data
                    AmbilData()

                Catch ex As Exception
                    MessageBox.Show("Terjadi kesalahan saat menghapus: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End If
    End Sub



    Private Sub Form_PenjualanDitahan_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F9 Then
            ' Panggil prosedur BtnProses_Click
            BtnProses_Click(sender, e)
        ElseIf e.KeyCode = Keys.Escape Then
            ' Tombol ESC ditekan, keluar dari form
            Close()
        End If
    End Sub


    Private Sub DgvData_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles DgvData.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Panggil metode atau prosedur yang ingin dijalankan saat Enter ditekan
            DgvData_CellClick(sender, New DataGridViewCellEventArgs(DgvData.CurrentCell.ColumnIndex, DgvData.CurrentCell.RowIndex))
        End If
    End Sub

End Class