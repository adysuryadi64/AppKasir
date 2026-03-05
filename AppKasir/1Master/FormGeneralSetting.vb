

Public Class FormGeneralSetting

    ' Inisialisasi RoleComboList setelah InitializeComponent()
    Private ReadOnly RoleComboList As List(Of (Label As Label, ComboBox As ComboBox, DefaultValue As Integer))

    Public Sub New()
        InitializeComponent()

        ' Inisialisasi RoleComboList di sini, setelah InitializeComponent supaya kontrol sudah siap
        RoleComboList = New List(Of (Label, ComboBox, Integer)) From {
            (LblBeliFokus, CmbBeliFokus, 0),
            (LblBeliSatuan, CmbBeliSatuan, 1),
            (LblBeliRugi, CmbBeliRugi, 1),
            (LblBeliMuculJual, CmbBeliMuculJual, 0),
            (LblBeliUpdate, CmbBeliUpdate, 0),
            (LblBeliEditHarga, CmbBeliEditHarga, 0),
            (LblBeliAverage, CmbBeliAverage, 2),
            (LblBeliTanpaSupplier, CmbBeliTanpaSupplier, 1),
            (LblbeliIsiNominal, CmbbeliIsiNominal, 1),
            (LblbeliNominal0, CmbbeliNominal0, 0),
            (LblJualFokus, CmbJualFokus, 0),
            (LblJualSatuan, CmbJualSatuan, 1),
            (LblJualEditHarga, CmbJualEditHarga, 0),
            (LblJualRugi, CmbJualRugi, 1),
            (LblJualMinus, CmbJualMinus, 1),
            (LblTampilstokJual, CmbTampilstokJual, 0),
            (LblDiskonItem, CmbDiskonItem, 0),
            (LblJualIsiNominal, CmbJualIsiNominal, 1),
            (LblJualNominal0, CmbJualNominal0, 0),
            (LblJualJenisKertasCetak, CmbJualJenisKertasCetak, 1),
            (LblEditHargaJual, CmbEditHargaJual, 1),
            (LblTampilstokJual, CmbTampilstokJual, 0),
            (LblTransferFocus, CmbTransferFocus, 0),
            (LblTransferSatuan, CmbTransferSatuan, 1),
            (LblTransferMinus, CmbTransferMinus, 1),
            (LblReturBeliFokus, CmbReturBeliFokus, 0),
            (LblReturBeliSatuan, CmbReturBeliSatuan, 1),
            (LblReturBeliMinus, CmbReturBeliMinus, 1),
            (LblReturBeliAlasan, CmbReturBeliAlasan, 0),
            (LblReturJualFokus, CmbReturJualFokus, 0),
            (LblReturJualSatuan, CmbReturJualSatuan, 1),
            (LblReturJualMinus, CmbReturJualMinus, 1),
            (LblReturJualAlasan, CmbReturJualAlasan, 0),
            (LblStokFocus, CmbStokFocus, 0),
            (LblStokSatuan, CmbStokSatuan, 1),
            (LblStokMinus, CmbStokMinus, 1),
            (LblTransaksiTanggalLampau, CmbTransaksiTanggalLampau, 1)
        }
    End Sub





    Private Sub FormGeneralSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SinkronkanHakAksesTanpaDuplikat()
        BacaCombobox()
    End Sub


    Public Sub SinkronkanHakAksesTanpaDuplikat()
        Dim listRoleDariLabel = RoleComboList.Select(Function(item) item.Label.Text).ToList()

        ' Ambil data role-user dari DB
        Dim listRoleDB As New Dictionary(Of String, List(Of String))
        Using cmd As New MySqlCommand("SELECT UserName, Role FROM hakaksesuser", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    Dim role = rd("Role").ToString()
                    Dim user = rd("UserName").ToString()
                    If Not listRoleDB.ContainsKey(role) Then
                        listRoleDB(role) = New List(Of String)
                    End If
                    listRoleDB(role).Add(user)
                End While
            End Using
        End Using

        ' Hapus duplikat untuk 'Semua'
        For Each role In listRoleDB.Keys
            Dim userList = listRoleDB(role)
            If userList.Count > 1 Then
                Using delCmd As New MySqlCommand("DELETE FROM hakaksesuser WHERE Role = @Role AND UserName = 'Semua'", conn)
                    delCmd.Parameters.AddWithValue("@Role", role)
                    delCmd.ExecuteNonQuery()
                End Using
            End If
        Next

        ' Tambahkan role yang belum ada
        Dim roleDBSaatIni = listRoleDB.Keys.ToList()
        Dim roleBaru = listRoleDariLabel.Distinct().Except(roleDBSaatIni).ToList()

        If roleBaru.Count > 0 Then
            Using insBaru As New MySqlCommand("INSERT INTO hakaksesuser (UserName, Role, ModuleName) VALUES (@UserName, @Role, @ModuleName)", conn)
                For Each role In roleBaru
                    ' Cari dari RoleComboList berdasarkan label.Text (yaitu role)
                    Dim match = RoleComboList.FirstOrDefault(Function(item) item.Label.Text = role)

                    If match.ComboBox IsNot Nothing Then
                        Dim defaultValue As String = match.ComboBox.Items(match.DefaultValue).ToString()

                        insBaru.Parameters.Clear()
                        insBaru.Parameters.AddWithValue("@UserName", "Semua")
                        insBaru.Parameters.AddWithValue("@Role", role)
                        insBaru.Parameters.AddWithValue("@ModuleName", defaultValue)
                        insBaru.ExecuteNonQuery()
                    End If
                Next
            End Using
        End If

    End Sub




    Public Sub BacaCombobox()
        Dim SelectQuery As String = "SELECT Role, ModuleName FROM hakaksesuser WHERE ModuleName <> ''"
        Dim moduleDict As New Dictionary(Of String, String)()

        Using cmd As New MySqlCommand(SelectQuery, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    moduleDict(reader("Role").ToString()) = reader("ModuleName").ToString()
                End While
            End Using
        End Using

        For Each item In RoleComboList
            Dim label = item.Label
            Dim combo = item.ComboBox
            Dim defaultValue = item.DefaultValue

            Dim role = label.Text
            If moduleDict.ContainsKey(role) Then
                combo.Text = moduleDict(role)
            Else
                combo.SelectedIndex = defaultValue
            End If
        Next
    End Sub



    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        Dim transaksi As MySqlTransaction = Nothing
        Try
            transaksi = conn.BeginTransaction()

            Using cmd As New MySqlCommand("UPDATE hakaksesuser SET ModuleName = @ModuleName WHERE Role = @Role", conn, transaksi)
                cmd.Parameters.Add("@ModuleName", MySqlDbType.VarChar)
                cmd.Parameters.Add("@Role", MySqlDbType.VarChar)
                cmd.Prepare()

                ' Gunakan RoleComboList langsung
                For Each item In RoleComboList
                    Dim moduleName As String = item.ComboBox.Text.Trim()
                    Dim roleName As String = item.Label.Text.Trim()

                    If Not String.IsNullOrEmpty(moduleName) AndAlso Not String.IsNullOrEmpty(roleName) Then
                        cmd.Parameters("@ModuleName").Value = moduleName
                        cmd.Parameters("@Role").Value = roleName
                        cmd.ExecuteNonQuery()
                    End If
                Next
            End Using

            transaksi.Commit()
            MessageBox.Show("Perubahan telah disimpan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            DatabaseModule.CatatanAksiHistory("Update hak akses user")
        Catch ex As Exception
            If transaksi IsNot Nothing Then transaksi.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class