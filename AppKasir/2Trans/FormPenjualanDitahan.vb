Public Class FormPenjualanDitahan
    Private Sub Form_PenjualanDitahan_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        KeyPreview = True
        MuatDataHeader()
        SetupUi()
        MuatDetail(AmbilFakturTerpilih())
    End Sub

    Private Sub Form_PenjualanDitahan_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DgvData.Focus()
    End Sub

    Private Sub SetupUi()
        ' Konfigurasi kolom — dipanggil setelah DataSource di-set
        If DgvData.Columns.Count = 0 Then Exit Sub

               ' Alignment dan format
        If DgvData.Columns.Contains("FAKTUR_JUAL") Then
            DgvData.Columns("FAKTUR_JUAL").HeaderText = "Faktur"
            DgvData.Columns("FAKTUR_JUAL").Width = 130
        End If
        If DgvData.Columns.Contains("TANGGAL_JUAL") Then
            DgvData.Columns("TANGGAL_JUAL").HeaderText = "Tanggal"
            DgvData.Columns("TANGGAL_JUAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            DgvData.Columns("TANGGAL_JUAL").DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"
            DgvData.Columns("TANGGAL_JUAL").Width = 130
        End If
        If DgvData.Columns.Contains("NAMA_PELANGGAN") Then
            DgvData.Columns("NAMA_PELANGGAN").HeaderText = "Pelanggan"
            DgvData.Columns("NAMA_PELANGGAN").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End If
        If DgvData.Columns.Contains("GRAN_TOTAL") Then
            DgvData.Columns("GRAN_TOTAL").HeaderText = "Grand Total"
            DgvData.Columns("GRAN_TOTAL").Width = 120
        End If
        If DgvData.Columns.Contains("TOTAL_ITEM") Then
            DgvData.Columns("TOTAL_ITEM").HeaderText = "Item"
            DgvData.Columns("TOTAL_ITEM").Width = 60
        End If
        If DgvData.Columns.Contains("TOTAL_QTY") Then
            DgvData.Columns("TOTAL_QTY").HeaderText = "Qty"
            DgvData.Columns("TOTAL_QTY").Width = 60
        End If
        ModuleAngka.TerapkanFormatKolomAngka(DgvData, "GRAN_TOTAL", "TOTAL_ITEM", "TOTAL_QTY")

        DgvDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DgvDetail.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells
    End Sub

    Private Sub MuatDataHeader()
        Dim dt As New DataTable()
        Dim sql As String =
            "SELECT FAKTUR_JUAL, NAMA_PELANGGAN, TANGGAL_JUAL, LOKASI, GRAN_TOTAL, TOTAL_ITEM, TOTAL_QTY, ID_USER " &
            "FROM penjualan_ditahan ORDER BY TANGGAL_JUAL DESC, FAKTUR_JUAL DESC"
        Using da As New MySqlDataAdapter(sql, conn)
            da.Fill(dt)
        End Using
        DgvData.DataSource = dt
        ModuleTheme.ApplyThemeDataGridView(DgvData)
        SetupUi()
        DgvData.ClearSelection()
        DgvData.Rows(0).Selected = True
        If DgvData.Columns.Count > 0 Then
            DgvData.CurrentCell = DgvData.Rows(0).Cells("FAKTUR_JUAL")
        End If
        DgvData.Focus()

    End Sub

    Private Function AmbilFakturTerpilih() As String
        If DgvData.CurrentRow Is Nothing OrElse DgvData.CurrentRow.Cells("FAKTUR_JUAL").Value Is Nothing Then
            Return ""
        End If
        Return DgvData.CurrentRow.Cells("FAKTUR_JUAL").Value.ToString()
    End Function

    Private Sub MuatDetail(faktur As String)
        Dim dt As New DataTable()
        If String.IsNullOrWhiteSpace(faktur) Then
            DgvDetail.DataSource = dt
            Return
        End If

        Dim sql As String =
            "SELECT ID_BARANG AS `Kode`, NAMA_BARANG AS `Nama Barang`, QTY AS `Qty`, SATUAN AS `Satuan`, " &
            "ISI_SATUAN AS `Isi`, HARGA_JUAL AS `Harga Jual`, TOTAL_HARGA AS `Total` " &
            "FROM penjualan_ditahan_detail WHERE FAKTUR_JUAL = @FAKTUR ORDER BY NAMA_BARANG"
        Using da As New MySqlDataAdapter(sql, conn)
            da.SelectCommand.Parameters.AddWithValue("@FAKTUR", faktur)
            da.Fill(dt)
        End Using
        DgvDetail.DataSource = dt
        If DgvDetail.Columns.Contains("Harga Jual") Then
            DgvDetail.Columns("Harga Jual").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        End If
        ModuleAngka.TerapkanFormatKolomAngka(DgvDetail, "Harga Jual", "Total", "Qty", "Isi")
        ModuleTheme.ApplyThemeDataGridView(DgvDetail)
    End Sub

    Private Sub ProsesAmbilDraft()
        Dim faktur As String = AmbilFakturTerpilih()
        If String.IsNullOrWhiteSpace(faktur) Then
            MessageBox.Show("Pilih data yang akan diproses.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Validasi LOKASI — hanya boleh ambil draft milik lokasi sendiri
        Dim lokasiDraft As String = ""
        If DgvData.CurrentRow IsNot Nothing AndAlso DgvData.CurrentRow.Cells("LOKASI").Value IsNot Nothing Then
            lokasiDraft = DgvData.CurrentRow.Cells("LOKASI").Value.ToString()
        End If
        Dim lokasiUser As String = FormUtama.StatusLokasi.Text
        If Not String.IsNullOrEmpty(lokasiDraft) AndAlso
           Not String.Equals(lokasiDraft, lokasiUser, StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show($"Draft ini milik lokasi {lokasiDraft}. Anda login di {lokasiUser}.",
                            "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim namaPelanggan As String = ""
            If DgvData.CurrentRow IsNot Nothing AndAlso DgvData.CurrentRow.Cells("NAMA_PELANGGAN").Value IsNot Nothing Then
                namaPelanggan = DgvData.CurrentRow.Cells("NAMA_PELANGGAN").Value.ToString()
            End If

            FormJual.CmbPelanggan.Text = namaPelanggan
            FormJual.TxtFaktur.Text = faktur
            FormJual.TxtJenistransaksi.Text = "TambahPenjualan"
            FormJual.SetDraftPenjualanAktif(faktur)
            FormJual.AmbilDataDitahan()

            Close()
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub HapusDraftTerpilih()
        Dim faktur As String = AmbilFakturTerpilih()
        If String.IsNullOrWhiteSpace(faktur) Then
            MessageBox.Show("Pilih data draft yang akan dihapus.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If MessageBox.Show($"Yakin ingin menghapus data dengan faktur: {faktur}?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Try
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                ' ========================================
                ' START: Audit Trail - Hapus Draft Penjualan
                ' ========================================
                Dim sbSnapshot As New System.Text.StringBuilder()
                Try
                    Using cmdSnap As New MySqlCommand(
                        "SELECT FAKTUR_JUAL, TANGGAL, KODE_PELANGGAN, NAMA_PELANGGAN, TOTAL_BARANG, SUBTOTAL, TOTAL, KETERANGAN " &
                        "FROM penjualan_ditahan WHERE FAKTUR_JUAL = @f LIMIT 1", conn, transaction)
                        cmdSnap.Parameters.AddWithValue("@f", faktur)
                        Using rdSnap = cmdSnap.ExecuteReader()
                            If rdSnap.Read() Then
                                sbSnapshot.AppendLine($"Faktur: {rdSnap("FAKTUR_JUAL")}")
                                sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(rdSnap("TANGGAL")).ToString("dd/MM/yyyy HH:mm:ss")}")
                                sbSnapshot.AppendLine($"Kode Pelanggan: {rdSnap("KODE_PELANGGAN")}")
                                sbSnapshot.AppendLine($"Nama Pelanggan: {rdSnap("NAMA_PELANGGAN")}")
                                sbSnapshot.AppendLine($"Total Barang: {ModuleAngka.ParseDecimal(rdSnap("TOTAL_BARANG"))} item")
                                sbSnapshot.AppendLine($"Subtotal: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("SUBTOTAL")))}")
                                sbSnapshot.AppendLine($"Total: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TOTAL")))}")
                                sbSnapshot.AppendLine($"Keterangan: {rdSnap("KETERANGAN")}")
                            End If
                        End Using
                    End Using

                    sbSnapshot.AppendLine(vbCrLf & "Detail Barang:")
                    Using cmdSnapDetail As New MySqlCommand(
                        "SELECT KODE_BARANG, NAMA_BARANG, QTY, HARGA_JUAL, TOTAL_HARGA " &
                        "FROM penjualan_ditahan_detail WHERE FAKTUR_JUAL = @f ORDER BY KODE_BARANG", conn, transaction)
                        cmdSnapDetail.Parameters.AddWithValue("@f", faktur)
                        Using rdSnapDetail = cmdSnapDetail.ExecuteReader()
                            While rdSnapDetail.Read()
                                sbSnapshot.AppendLine($"- {rdSnapDetail("KODE_BARANG")} - {rdSnapDetail("NAMA_BARANG")}: {rdSnapDetail("QTY")} x {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnapDetail("HARGA_JUAL")))} = {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnapDetail("TOTAL_HARGA")))}")
                            End While
                        End Using
                    End Using
                Catch
                    sbSnapshot.AppendLine("Gagal baca data sebelum hapus")
                End Try
                ModuleAuditTrail.CatatAuditMaster("DRAFT-JL:" & faktur, "HAPUS", "Draft Penjualan", sbSnapshot.ToString(), trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Hapus Draft Penjualan
                ' ========================================

                Using cmd1 As New MySqlCommand("DELETE FROM penjualan_ditahan WHERE FAKTUR_JUAL = @faktur", conn, transaction)
                    cmd1.Parameters.AddWithValue("@faktur", faktur)
                    cmd1.ExecuteNonQuery()
                End Using
                Using cmd2 As New MySqlCommand("DELETE FROM penjualan_ditahan_detail WHERE FAKTUR_JUAL = @faktur", conn, transaction)
                    cmd2.Parameters.AddWithValue("@faktur", faktur)
                    cmd2.ExecuteNonQuery()
                End Using
                transaction.Commit()
            End Using

            MuatDataHeader()
            MuatDetail(AmbilFakturTerpilih())
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat menghapus: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnProses_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnProses.Click
        ProsesAmbilDraft()
    End Sub

    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnHapus.Click
        HapusDraftTerpilih()
    End Sub


    Private Sub BtnTutup_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTutup.Click
        Close()
    End Sub

    Private Sub DgvData_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DgvData.SelectionChanged
        Dim faktur As String = AmbilFakturTerpilih()
        MuatDetail(faktur)
    End Sub

    Private Sub DgvData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles DgvData.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            ProsesAmbilDraft()
        ElseIf e.KeyCode = Keys.Delete Then
            e.SuppressKeyPress = True
            HapusDraftTerpilih()
        End If
    End Sub

    Private Sub Form_PenjualanDitahan_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F9, Keys.Enter
                e.SuppressKeyPress = True
                ProsesAmbilDraft()
            Case Keys.Delete
                e.SuppressKeyPress = True
                HapusDraftTerpilih()
            Case Keys.Escape
                e.SuppressKeyPress = True
                Close()
        End Select
    End Sub
End Class

