Public Class FormPenjualanDitahan
    Private Sub Form_PenjualanDitahan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        TxtFaktur.Clear()
        TxtPel.Clear()
        Using cmd As New MySqlCommand("SELECT FAKTUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, TANGGAL_JUAL, GRAN_TOTAL, TOTAL_QTY, TOTAL_ITEM FROM penjualan_ditahan", conn)
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

    Public Sub AturDatagridview()
        With DgvData
            .ReadOnly = True
            .Columns(1).Visible = False
            .Columns(6).Visible = False

            .Columns(0).HeaderText = "NOTA JUAL"
            .Columns(2).HeaderText = "PELANGGAN"
            .Columns(3).HeaderText = "JENIS"
            .Columns(4).HeaderText = "TANGGAL"
            .Columns(5).HeaderText = "TOTAL HARGA"
            .Columns(7).HeaderText = "TOTAL ITEM"

            .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(5).DefaultCellStyle.Format = "N0"

            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
            .ClearSelection()
        End With
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
        ' Memastikan klik terjadi di baris yang valid
        If e.RowIndex >= 0 AndAlso e.RowIndex < DgvData.Rows.Count Then
            ' Menyimpan nilai dari baris yang diklik
            Dim row As DataGridViewRow = DgvData.Rows(e.RowIndex)
            TxtFaktur.Text = row.Cells(0).Value.ToString()
            TxtPel.Text = row.Cells(2).Value.ToString()
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