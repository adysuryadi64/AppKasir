Public Class FormPembelianDitahan
    Public Property SelectedFaktur As String = ""

    Private Sub FormPembelianDitahan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        KeyPreview = True
        DgvDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DgvDetail.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells
        MuatDataDraft()
        SetupUi()
        MuatDetailDraft(AmbilFakturTerpilih())
    End Sub

    Private Sub FormPembelianDitahan_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DgvDraft.Focus()
    End Sub

    Private Sub SetupUi()
        If DgvDraft.Columns.Count = 0 Then Exit Sub

        If DgvDraft.Columns.Contains("ID_USER") Then
            DgvDraft.Columns("ID_USER").Visible = False
        End If
        If DgvDraft.Columns.Contains("ID_PEMBELIAN") Then
            DgvDraft.Columns("ID_PEMBELIAN").HeaderText = "Faktur"
            DgvDraft.Columns("ID_PEMBELIAN").Width = 140
        End If
        If DgvDraft.Columns.Contains("TGL_BELI") Then
            DgvDraft.Columns("TGL_BELI").HeaderText = "Tanggal"
            DgvDraft.Columns("TGL_BELI").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            DgvDraft.Columns("TGL_BELI").DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"
            DgvDraft.Columns("TGL_BELI").Width = 140
        End If
        If DgvDraft.Columns.Contains("NAMA_SUPLIYER") Then
            DgvDraft.Columns("NAMA_SUPLIYER").HeaderText = "Supplier"
            DgvDraft.Columns("NAMA_SUPLIYER").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End If
        If DgvDraft.Columns.Contains("NOTA_PEMBELIAN") Then
            DgvDraft.Columns("NOTA_PEMBELIAN").HeaderText = "Nota"
            DgvDraft.Columns("NOTA_PEMBELIAN").Width = 140
        End If
        If DgvDraft.Columns.Contains("TOTAL_BARANG") Then
            DgvDraft.Columns("TOTAL_BARANG").HeaderText = "Item"
            DgvDraft.Columns("TOTAL_BARANG").Width = 70
        End If
        If DgvDraft.Columns.Contains("TOTAL_QTY") Then
            DgvDraft.Columns("TOTAL_QTY").HeaderText = "Qty"
            DgvDraft.Columns("TOTAL_QTY").Width = 90
        End If
        If DgvDraft.Columns.Contains("GRAND_TOTAL_BELI") Then
            DgvDraft.Columns("GRAND_TOTAL_BELI").HeaderText = "Grand Total"
            DgvDraft.Columns("GRAND_TOTAL_BELI").Width = 130
        End If
        ModuleAngka.TerapkanFormatKolomAngka(DgvDraft, "TOTAL_BARANG", "TOTAL_QTY", "GRAND_TOTAL_BELI")
        DgvDraft.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
    End Sub

    Private Sub FormPembelianDitahan_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                If DgvDraft.Focused OrElse DgvDraft.ContainsFocus Then
                    e.SuppressKeyPress = True
                    BtnPilih_Click(sender, EventArgs.Empty)
                End If
            Case Keys.F5
                e.SuppressKeyPress = True
                MuatDetailDraft(AmbilFakturTerpilih())
            Case Keys.Delete
                If DgvDraft.Focused OrElse DgvDraft.ContainsFocus Then
                    e.SuppressKeyPress = True
                    BtnHapus_Click(sender, EventArgs.Empty)
                End If
            Case Keys.Escape
                e.SuppressKeyPress = True
                BtnTutup_Click(sender, EventArgs.Empty)
        End Select
    End Sub

    Private Sub MuatDataDraft()
        Dim dt As New DataTable()
        Dim sql As String =
            "SELECT ID_PEMBELIAN, TGL_BELI, NAMA_SUPLIYER, NOTA_PEMBELIAN, TOTAL_BARANG, TOTAL_QTY, GRAND_TOTAL_BELI, ID_USER " &
            "FROM pembelian_ditahan ORDER BY TGL_BELI DESC, ID_PEMBELIAN DESC"
        Using da As New MySqlDataAdapter(sql, conn)
            da.Fill(dt)
        End Using
        DgvDraft.DataSource = dt
        ModuleTheme.ApplyThemeDataGridView(DgvDraft)
        SetupUi()
        DgvDraft.ClearSelection()
        DgvDraft.Rows(0).Selected = True
        If DgvDraft.Columns.Count > 0 Then
            DgvDraft.CurrentCell = DgvDraft.Rows(0).Cells("ID_PEMBELIAN")
        End If
        DgvDraft.Focus()

    End Sub

    Private Sub MuatDetailDraft(faktur As String)
        Dim dt As New DataTable()
        If String.IsNullOrWhiteSpace(faktur) Then
            DgvDetail.DataSource = dt
            Return
        End If

        Dim sql As String =
            "SELECT ID_BARANG AS `Kode`, NAMA_BARANG AS `Nama Barang`, QTY AS `Qty`, SATUAN AS `Satuan`, ISI_SATUAN AS `Isi`, HARGA_BELI AS `Harga Beli`, TOTAL AS `Total` " &
            "FROM pembelian_ditahan_detail WHERE FAKTUR_BELI = @FAKTUR ORDER BY URUTAN"
        Using da As New MySqlDataAdapter(sql, conn)
            da.SelectCommand.Parameters.AddWithValue("@FAKTUR", faktur)
            da.Fill(dt)
        End Using
        DgvDetail.DataSource = dt
        ModuleAngka.TerapkanFormatKolomAngka(DgvDetail, "Harga Beli", "Total", "Qty", "Isi")
        ModuleTheme.ApplyThemeDataGridView(DgvDetail)
    End Sub

    Private Function AmbilFakturTerpilih() As String
        If DgvDraft.CurrentRow Is Nothing OrElse DgvDraft.CurrentRow.Cells("ID_PEMBELIAN").Value Is Nothing Then
            Return ""
        End If
        Return DgvDraft.CurrentRow.Cells("ID_PEMBELIAN").Value.ToString()
    End Function

    Private Sub BtnPilih_Click(sender As Object, e As EventArgs) Handles BtnPilih.Click
        Dim faktur = AmbilFakturTerpilih()
        If String.IsNullOrWhiteSpace(faktur) Then
            MessageBox.Show("Pilih data draft terlebih dahulu.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        SelectedFaktur = faktur
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub DgvDraft_SelectionChanged(sender As Object, e As EventArgs) Handles DgvDraft.SelectionChanged
        MuatDetailDraft(AmbilFakturTerpilih())
    End Sub

    Private Sub DgvDraft_KeyDown(sender As Object, e As KeyEventArgs) Handles DgvDraft.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            BtnPilih_Click(sender, EventArgs.Empty)
        ElseIf e.KeyCode = Keys.F5 Then
            e.SuppressKeyPress = True
            MuatDetailDraft(AmbilFakturTerpilih())
        ElseIf e.KeyCode = Keys.Delete Then
            e.SuppressKeyPress = True
            BtnHapus_Click(sender, EventArgs.Empty)
        End If
    End Sub



    Private Sub BtnHapus_Click(sender As Object, e As EventArgs) Handles BtnHapus.Click
        Dim faktur = AmbilFakturTerpilih()
        If String.IsNullOrWhiteSpace(faktur) Then
            MessageBox.Show("Pilih data draft yang akan dihapus.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If MessageBox.Show("Hapus draft pembelian " & faktur & " ?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Dim tr As MySqlTransaction = Nothing
        Try
            tr = conn.BeginTransaction()
            ' ========================================
            ' START: Audit Trail - Hapus Draft Pembelian
            ' ========================================
            Dim sbSnapshot As New System.Text.StringBuilder()
            Try
                Using cmdSnap As New MySqlCommand(
                    "SELECT ID_PEMBELIAN, TANGGAL, KODE_SUPLIYER, NAMA_SUPLIYER, TOTAL_BARANG, SUBTOTAL, TOTAL, KETERANGAN " &
                    "FROM pembelian_ditahan WHERE ID_PEMBELIAN = @f LIMIT 1", conn, tr)
                    cmdSnap.Parameters.AddWithValue("@f", faktur)
                    Using rdSnap = cmdSnap.ExecuteReader()
                        If rdSnap.Read() Then
                            sbSnapshot.AppendLine($"Faktur: {rdSnap("ID_PEMBELIAN")}")
                            sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(rdSnap("TANGGAL")).ToString("dd/MM/yyyy HH:mm:ss")}")
                            sbSnapshot.AppendLine($"Kode Supplier: {rdSnap("KODE_SUPLIYER")}")
                            sbSnapshot.AppendLine($"Nama Supplier: {rdSnap("NAMA_SUPLIYER")}")
                            sbSnapshot.AppendLine($"Total Barang: {ModuleAngka.ParseDecimal(rdSnap("TOTAL_BARANG"))} item")
                            sbSnapshot.AppendLine($"Subtotal: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("SUBTOTAL")))}")
                            sbSnapshot.AppendLine($"Total: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("TOTAL")))}")
                            sbSnapshot.AppendLine($"Keterangan: {rdSnap("KETERANGAN")}")
                        End If
                    End Using
                End Using

                sbSnapshot.AppendLine(vbCrLf & "Detail Barang:")
                Using cmdSnapDetail As New MySqlCommand(
                    "SELECT KODE_BARANG, NAMA_BARANG, QTY, HARGA_BELI, TOTAL_HARGA " &
                    "FROM pembelian_ditahan_detail WHERE FAKTUR_BELI = @f ORDER BY URUTAN", conn, tr)
                    cmdSnapDetail.Parameters.AddWithValue("@f", faktur)
                    Using rdSnapDetail = cmdSnapDetail.ExecuteReader()
                        While rdSnapDetail.Read()
                            sbSnapshot.AppendLine($"- {rdSnapDetail("KODE_BARANG")} - {rdSnapDetail("NAMA_BARANG")}: {rdSnapDetail("QTY")} x {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnapDetail("HARGA_BELI")))} = {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnapDetail("TOTAL_HARGA")))}")
                        End While
                    End Using
                End Using
            Catch
                sbSnapshot.AppendLine("Gagal baca data sebelum hapus")
            End Try
            ModuleAuditTrail.CatatAuditMaster("DRAFT-PB:" & faktur, "HAPUS", "Draft Pembelian", sbSnapshot.ToString(), trans:=tr)
            ' ========================================
            ' END: Audit Trail - Hapus Draft Pembelian
            ' ========================================

            Using cmd As New MySqlCommand("DELETE FROM pembelian_ditahan_detail WHERE FAKTUR_BELI = @FAKTUR", conn, tr)
                cmd.Parameters.AddWithValue("@FAKTUR", faktur)
                cmd.ExecuteNonQuery()
            End Using
            Using cmd As New MySqlCommand("DELETE FROM pembelian_ditahan WHERE ID_PEMBELIAN = @FAKTUR", conn, tr)
                cmd.Parameters.AddWithValue("@FAKTUR", faktur)
                cmd.ExecuteNonQuery()
            End Using
            tr.Commit()
            MuatDataDraft()
            MuatDetailDraft(AmbilFakturTerpilih())
        Catch ex As Exception
            tr?.Rollback()
            MessageBox.Show("Gagal menghapus draft: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnTutup_Click(sender As Object, e As EventArgs) Handles BtnTutup.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub
End Class

