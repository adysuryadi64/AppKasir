Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Text

''' <summary>
''' Viewer untuk data audit trail arsip (tbl_audit_trail_arsip).
''' Identik dengan FormAuditTrail tetapi membaca dari tabel arsip.
''' </summary>
Public Class FormAuditTrailArsip

#Region "Fields"
    Private _isLoading As Boolean = False
#End Region

#Region "Form Load"
    Private Sub FormAuditTrailArsip_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)

        ' Filter default: bulan lalu
        DtpAwal.Value = DateTime.Today.AddMonths(-1)
        DtpAkhir.Value = DateTime.Today.AddDays(1).AddSeconds(-1)

        IsiCmbUser()
        IsiCmbJenisAksi()
        IsiCmbJenisTrans()
        MuatData()
    End Sub
#End Region

#Region "Inisialisasi ComboBox"
    Private Sub IsiCmbUser()
        CmbUser.Items.Clear()
        CmbUser.Items.Add("Semua")
        Try
            Using cmd As New MySqlCommand(
                "SELECT DISTINCT id_user FROM tbl_audit_trail_arsip ORDER BY id_user", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        If Not IsDBNull(rd("id_user")) Then
                            CmbUser.Items.Add(rd("id_user").ToString())
                        End If
                    End While
                End Using
            End Using
        Catch
        End Try
        CmbUser.SelectedIndex = 0
    End Sub

    Private Sub IsiCmbJenisAksi()
        CmbJenisAksi.Items.Clear()
        CmbJenisAksi.Items.AddRange(New String() {"Semua", "HAPUS", "EDIT", "TAMBAH_STOK", "KURANG_STOK"})
        CmbJenisAksi.SelectedIndex = 0
    End Sub

    Private Sub IsiCmbJenisTrans()
        CmbJenisTrans.Items.Clear()
        CmbJenisTrans.Items.AddRange(New String() {
            "Semua", "Penjualan", "Pembelian", "Retur Penjualan", "Retur Pembelian",
            "Bayar Hutang", "Bayar Piutang", "Master User", "Hak Akses User",
            "General Setting", "Master Barang", "Stok Manual", "Stok Opname",
            "Master Gaji", "Master Karyawan", "Slip Gaji", "Bon Karyawan",
            "Transfer Barang", "Jurnal Keuangan", "Tabel Referensi"})
        CmbJenisTrans.SelectedIndex = 0
    End Sub
#End Region

#Region "Muat Data"
    Private Sub MuatData()
        If _isLoading Then Return
        _isLoading = True
        BtnCari.Enabled = False
        DgvAudit.Rows.Clear()

        Try
            Dim sb As New StringBuilder()
            sb.Append("SELECT id_audit, waktu_aksi, jenis_aksi, jenis_trans, identifier, " &
                      "id_user, lokasi, komputer, ket " &
                      "FROM tbl_audit_trail_arsip " &
                      "WHERE waktu_aksi BETWEEN @awal AND @akhir")

            If CmbUser.Text <> "Semua" AndAlso Not String.IsNullOrEmpty(CmbUser.Text) Then
                sb.Append(" AND id_user = @user")
            End If
            If CmbJenisAksi.Text <> "Semua" AndAlso Not String.IsNullOrEmpty(CmbJenisAksi.Text) Then
                sb.Append(" AND jenis_aksi = @aksi")
            End If
            If CmbJenisTrans.Text <> "Semua" AndAlso Not String.IsNullOrEmpty(CmbJenisTrans.Text) Then
                sb.Append(" AND jenis_trans = @trans")
            End If
            sb.Append(" ORDER BY waktu_aksi DESC")

            Using cmd As New MySqlCommand(sb.ToString(), conn)
                cmd.Parameters.AddWithValue("@awal", DtpAwal.Value)
                cmd.Parameters.AddWithValue("@akhir", DtpAkhir.Value)
                If CmbUser.Text <> "Semua" AndAlso Not String.IsNullOrEmpty(CmbUser.Text) Then
                    cmd.Parameters.AddWithValue("@user", CmbUser.Text)
                End If
                If CmbJenisAksi.Text <> "Semua" AndAlso Not String.IsNullOrEmpty(CmbJenisAksi.Text) Then
                    cmd.Parameters.AddWithValue("@aksi", CmbJenisAksi.Text)
                End If
                If CmbJenisTrans.Text <> "Semua" AndAlso Not String.IsNullOrEmpty(CmbJenisTrans.Text) Then
                    cmd.Parameters.AddWithValue("@trans", CmbJenisTrans.Text)
                End If

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim idAudit As Integer = Convert.ToInt32(rd("id_audit"))
                        Dim waktu As String = If(IsDBNull(rd("waktu_aksi")), "",
                            Convert.ToDateTime(rd("waktu_aksi")).ToString("dd/MM/yyyy HH:mm:ss"))
                        Dim aksi As String = If(IsDBNull(rd("jenis_aksi")), "", rd("jenis_aksi").ToString())
                        Dim trans As String = If(IsDBNull(rd("jenis_trans")), "", rd("jenis_trans").ToString())
                        Dim ident As String = If(IsDBNull(rd("identifier")), "", rd("identifier").ToString())
                        Dim user As String = If(IsDBNull(rd("id_user")), "", rd("id_user").ToString())
                        Dim lok As String = If(IsDBNull(rd("lokasi")), "", rd("lokasi").ToString())
                        Dim pc As String = If(IsDBNull(rd("komputer")), "", rd("komputer").ToString())
                        Dim ket As String = If(IsDBNull(rd("ket")), "", rd("ket").ToString())

                        DgvAudit.Rows.Add(waktu, aksi, trans, ident, user, lok, pc, ket, idAudit)
                    End While
                End Using
            End Using

            LblTotalRecord.Text = "Total record arsip: " & DgvAudit.Rows.Count

        Catch ex As Exception
            MessageBox.Show("Gagal memuat data arsip: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _isLoading = False
            BtnCari.Enabled = True
        End Try
    End Sub
#End Region

#Region "Detail JSON"
    Private Sub DgvAudit_SelectionChanged(sender As Object, e As EventArgs) Handles DgvAudit.SelectionChanged
        If _isLoading OrElse DgvAudit.CurrentRow Is Nothing Then Exit Sub

        Dim ketCell As Object = DgvAudit.CurrentRow.Cells("ColKet").Value
        Dim ket As String = If(ketCell IsNot Nothing AndAlso Not IsDBNull(ketCell),
                               ketCell.ToString(), "")

        TxtDetail.Text = If(String.IsNullOrEmpty(ket), "(Tidak ada keterangan)", ket)
    End Sub
#End Region

#Region "Tombol"
    Private Sub BtnCari_Click(sender As Object, e As EventArgs) Handles BtnCari.Click
        MuatData()
    End Sub

    Private Sub BtnExport_Click(sender As Object, e As EventArgs) Handles BtnExport.Click
        Using dlg As New SaveFileDialog()
            dlg.Filter = "CSV Files (*.csv)|*.csv"
            dlg.FileName = "AuditTrailArsip_" & DateTime.Today.ToString("yyyyMMdd") & ".csv"
            If dlg.ShowDialog() = DialogResult.OK Then
                Try
                    Using sw As New StreamWriter(dlg.FileName, False, Encoding.UTF8)
                        Dim headers As New List(Of String)
                        For Each col As DataGridViewColumn In DgvAudit.Columns
                            If col.Visible Then headers.Add(If(col.HeaderText.Contains(","), """" & col.HeaderText & """", col.HeaderText))
                        Next
                        sw.WriteLine(String.Join(",", headers))
                        For Each row As DataGridViewRow In DgvAudit.Rows
                            Dim values As New List(Of String)
                            For Each col As DataGridViewColumn In DgvAudit.Columns
                                If col.Visible Then
                                    Dim val As String = If(row.Cells(col.Index).Value IsNot Nothing, row.Cells(col.Index).Value.ToString(), "")
                                    values.Add(If(val.Contains(",") OrElse val.Contains(""""), """" & val.Replace("""", """""") & """", val))
                                End If
                            Next
                            sw.WriteLine(String.Join(",", values))
                        Next
                    End Using
                    MessageBox.Show("Export berhasil: " & dlg.FileName, "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Gagal export: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Private Sub BtnTutup_Click(sender As Object, e As EventArgs) Handles BtnTutup.Click
        Me.Close()
    End Sub
#End Region

#Region "Keyboard"
    Private Sub FormAuditTrailArsip_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnCari.PerformClick()
            Case Keys.Escape : BtnTutup.PerformClick()
        End Select
    End Sub
#End Region

End Class
