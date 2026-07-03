Public Class FormMasterGaji
    Private Sub FormMasterGaji_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        AmbildataMasterGaji()
    End Sub

    Public Sub AmbildataMasterGaji()
        Dim query As String = "SELECT Kode, Hari_kerja, Prosentase_komisi, Bonus_Supir, " &
                              "Bonus_Helper, Bonus_Transport, Bonus_makan, Bonus_Lembur, " &
                              "Jenis_Potongan, Potongan_Absen, Potongan_Absen_Khusus, Potongan_Terlambat " &
                              "FROM tbl_Gaji"

        Using cmd As New MySqlCommand(query, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' Mengisi nilai dari reader ke kontrol yang sesuai
                    LblNomor.Text = reader("Kode").ToString()
                    TxtHariKerja.Text = If(Convert.IsDBNull(reader("Hari_kerja")), 0, Integer.Parse(reader("Hari_kerja").ToString()))
                    TxtProsentase.Text = ModuleAngka.FormatUntukInput(If(Convert.IsDBNull(reader("Prosentase_komisi")), 0D, ModuleAngka.ParseDecimal(reader("Prosentase_komisi"))))
                    TxtSupir.Text = ModuleAngka.FormatUntukInput(If(Convert.IsDBNull(reader("Bonus_Supir")), 0D, ModuleAngka.ParseDecimal(reader("Bonus_Supir"))))
                    TxtHelper.Text = ModuleAngka.FormatUntukInput(If(Convert.IsDBNull(reader("Bonus_Helper")), 0D, ModuleAngka.ParseDecimal(reader("Bonus_Helper"))))
                    TxtTransport.Text = ModuleAngka.FormatUntukInput(If(Convert.IsDBNull(reader("Bonus_Transport")), 0D, ModuleAngka.ParseDecimal(reader("Bonus_Transport"))))
                    TxtMakan.Text = ModuleAngka.FormatUntukInput(If(Convert.IsDBNull(reader("Bonus_makan")), 0D, ModuleAngka.ParseDecimal(reader("Bonus_makan"))))
                    TxtLembur.Text = ModuleAngka.FormatUntukInput(If(Convert.IsDBNull(reader("Bonus_Lembur")), 0D, ModuleAngka.ParseDecimal(reader("Bonus_Lembur"))))
                    CmbAbsen.Text = reader("Jenis_Potongan").ToString()
                    TxtAbsen.Text = ModuleAngka.FormatUntukInput(If(Convert.IsDBNull(reader("Potongan_Absen")), 0D, ModuleAngka.ParseDecimal(reader("Potongan_Absen"))))
                    TxtAbsenKhusus.Text = ModuleAngka.FormatUntukInput(If(Convert.IsDBNull(reader("Potongan_Absen_Khusus")), 0D, ModuleAngka.ParseDecimal(reader("Potongan_Absen_Khusus"))))
                    TxtTelat.Text = ModuleAngka.FormatUntukInput(If(Convert.IsDBNull(reader("Potongan_Terlambat")), 0D, ModuleAngka.ParseDecimal(reader("Potongan_Terlambat"))))
                End If
            End Using
        End Using
    End Sub

    Private Sub BtnSimpann_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpann.Click
        SaveOrUpdateDataMasterGaji()
    End Sub

    Public Sub SaveOrUpdateDataMasterGaji()
        ' Periksa apakah data dengan Kode tertentu sudah ada
        Dim isExisting As Boolean = False
        Dim checkQuery As String = "SELECT COUNT(*) FROM tbl_Gaji WHERE Kode = @Kode"

        Using checkCmd As New MySqlCommand(checkQuery, conn)
            checkCmd.Parameters.AddWithValue("@Kode", LblNomor.Text)
            Dim result As Object = checkCmd.ExecuteScalar()
            isExisting = Convert.ToInt32(result) > 0
        End Using

        ' SQL untuk INSERT atau UPDATE
        If isExisting Then
            ' Lakukan UPDATE jika data sudah ada
            Dim updateQuery As String = "UPDATE tbl_Gaji SET Hari_kerja = @Hari_kerja, Prosentase_komisi = @Prosentase_komisi, " &
                                    "Bonus_Supir = @Bonus_Supir, Bonus_Helper = @Bonus_Helper, " &
                                    "Bonus_Transport = @Bonus_Transport, Bonus_makan = @Bonus_makan, " &
                                    "Bonus_Lembur = @Bonus_Lembur, Jenis_Potongan = @Jenis_Potongan, " &
                                    "Potongan_Absen = @Potongan_Absen, Potongan_Absen_Khusus = @Potongan_Absen_Khusus, " &
                                    "Potongan_Terlambat = @Potongan_Terlambat WHERE Kode = @Kode"

            Using updateCmd As New MySqlCommand(updateQuery, conn)
                AddParameters(updateCmd)

                ' ========================================
                ' START: Audit Trail - Edit Master Gaji
                ' ========================================
                Dim sbSnapshot As New System.Text.StringBuilder()
                Using oldCmd As New MySqlCommand(
                    "SELECT Kode, Hari_kerja, Prosentase_komisi, Bonus_Supir, Bonus_Helper, " &
                    "Bonus_Transport, Bonus_makan, Bonus_Lembur, Potongan_Absen " &
                    "FROM tbl_Gaji WHERE Kode = @Kode LIMIT 1", conn)
                    oldCmd.Parameters.AddWithValue("@Kode", LblNomor.Text)
                    Using oldRd As MySqlDataReader = oldCmd.ExecuteReader()
                        If oldRd.Read() Then
                            Dim newHari As Integer = ModuleAngka.ParseInteger(TxtHariKerja.Text)
                            Dim newPros As Decimal = ModuleAngka.ParseDecimal(TxtProsentase.Text)
                            Dim newSupir As Decimal = ModuleAngka.ParseDecimal(TxtSupir.Text)
                            Dim newHelper As Decimal = ModuleAngka.ParseDecimal(TxtHelper.Text)
                            Dim newTransport As Decimal = ModuleAngka.ParseDecimal(TxtTransport.Text)
                            Dim newMakan As Decimal = ModuleAngka.ParseDecimal(TxtMakan.Text)
                            Dim newLembur As Decimal = ModuleAngka.ParseDecimal(TxtLembur.Text)
                            Dim newAbsen As Decimal = ModuleAngka.ParseDecimal(TxtAbsen.Text)

                            sbSnapshot.AppendLine($"Kode Gaji: {oldRd("Kode")}")
                            sbSnapshot.AppendLine($"Hari Kerja (sebelum): {oldRd("Hari_kerja")}")
                            sbSnapshot.AppendLine($"Hari Kerja (sesudah): {newHari}")
                            sbSnapshot.AppendLine($"Prosentase Komisi (sebelum): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(oldRd("Prosentase_komisi")))}")
                            sbSnapshot.AppendLine($"Prosentase Komisi (sesudah): {ModuleAngka.FormatRupiah(newPros)}")
                            sbSnapshot.AppendLine($"Bonus Supir (sebelum): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(oldRd("Bonus_Supir")))}")
                            sbSnapshot.AppendLine($"Bonus Supir (sesudah): {ModuleAngka.FormatRupiah(newSupir)}")
                            sbSnapshot.AppendLine($"Bonus Helper (sebelum): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(oldRd("Bonus_Helper")))}")
                            sbSnapshot.AppendLine($"Bonus Helper (sesudah): {ModuleAngka.FormatRupiah(newHelper)}")
                            sbSnapshot.AppendLine($"Bonus Transport (sebelum): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(oldRd("Bonus_Transport")))}")
                            sbSnapshot.AppendLine($"Bonus Transport (sesudah): {ModuleAngka.FormatRupiah(newTransport)}")
                            sbSnapshot.AppendLine($"Bonus Makan (sebelum): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(oldRd("Bonus_makan")))}")
                            sbSnapshot.AppendLine($"Bonus Makan (sesudah): {ModuleAngka.FormatRupiah(newMakan)}")
                            sbSnapshot.AppendLine($"Bonus Lembur (sebelum): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(oldRd("Bonus_Lembur")))}")
                            sbSnapshot.AppendLine($"Bonus Lembur (sesudah): {ModuleAngka.FormatRupiah(newLembur)}")
                            sbSnapshot.AppendLine($"Potongan Absen (sebelum): {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(oldRd("Potongan_Absen")))}")
                            sbSnapshot.AppendLine($"Potongan Absen (sesudah): {ModuleAngka.FormatRupiah(newAbsen)}")
                        End If
                    End Using
                End Using
                ModuleAuditTrail.CatatAuditMaster("GAJI:" & LblNomor.Text, "EDIT", "Master Gaji", sbSnapshot.ToString())
                ' ========================================
                ' END: Audit Trail - Edit Master Gaji
                ' ========================================

                updateCmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Data berhasil diperbarui.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            ' Lakukan INSERT jika data belum ada
            Dim insertQuery As String = "INSERT INTO tbl_Gaji (Kode, Hari_kerja, Prosentase_komisi, Bonus_Supir, Bonus_Helper, Bonus_Transport, Bonus_makan, Bonus_Lembur, Jenis_Potongan, Potongan_Absen, Potongan_Absen_Khusus, Potongan_Terlambat) " &
                                    "VALUES (@Kode, @Hari_kerja, @Prosentase_komisi, @Bonus_Supir, @Bonus_Helper, @Bonus_Transport, @Bonus_makan, @Bonus_Lembur, @Jenis_Potongan, @Potongan_Absen, @Potongan_Absen_Khusus, @Potongan_Terlambat)"

            Using insertCmd As New MySqlCommand(insertQuery, conn)
                AddParameters(insertCmd)
                insertCmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Data berhasil ditambahkan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ' Metode tambahan untuk menambahkan parameter ke perintah SQL
    Private Sub AddParameters(ByRef cmd As MySqlCommand)
        Dim hariKerja As Integer = ModuleAngka.ParseInteger(TxtHariKerja.Text)
        Dim prosentaseKomisi As Decimal = ModuleAngka.ParseDecimal(TxtProsentase.Text)
        Dim bonusSupir As Decimal = ModuleAngka.ParseDecimal(TxtSupir.Text)
        Dim bonusHelper As Decimal = ModuleAngka.ParseDecimal(TxtHelper.Text)
        Dim bonusTransport As Decimal = ModuleAngka.ParseDecimal(TxtTransport.Text)
        Dim bonusMakan As Decimal = ModuleAngka.ParseDecimal(TxtMakan.Text)
        Dim bonusLembur As Decimal = ModuleAngka.ParseDecimal(TxtLembur.Text)
        Dim potonganAbsen As Decimal = ModuleAngka.ParseDecimal(TxtAbsen.Text)
        Dim potonganAbsenKhusus As Decimal = ModuleAngka.ParseDecimal(TxtAbsenKhusus.Text)
        Dim potonganTerlambat As Decimal = ModuleAngka.ParseDecimal(TxtTelat.Text)

        ' Tambahkan parameter
        cmd.Parameters.AddWithValue("@Kode", LblNomor.Text)
        cmd.Parameters.AddWithValue("@Hari_kerja", hariKerja)
        cmd.Parameters.AddWithValue("@Prosentase_komisi", prosentaseKomisi)
        cmd.Parameters.AddWithValue("@Bonus_Supir", bonusSupir)
        cmd.Parameters.AddWithValue("@Bonus_Helper", bonusHelper)
        cmd.Parameters.AddWithValue("@Bonus_Transport", bonusTransport)
        cmd.Parameters.AddWithValue("@Bonus_makan", bonusMakan)
        cmd.Parameters.AddWithValue("@Bonus_Lembur", bonusLembur)
        cmd.Parameters.AddWithValue("@Jenis_Potongan", CmbAbsen.Text)
        cmd.Parameters.AddWithValue("@Potongan_Absen", potonganAbsen)
        cmd.Parameters.AddWithValue("@Potongan_Absen_Khusus", potonganAbsenKhusus)
        cmd.Parameters.AddWithValue("@Potongan_Terlambat", potonganTerlambat)
    End Sub


    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Private Sub CmbAbsen_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbAbsen.SelectedIndexChanged
        ' Jika SelectedIndex adalah 0, tampilkan TxtAbsen
        If CmbAbsen.SelectedIndex = 0 Then
            TxtAbsen.Visible = False
            ' Jika SelectedIndex adalah 1, sembunyikan TxtAbsen
        ElseIf CmbAbsen.SelectedIndex = 1 Then
            TxtAbsen.Visible = True
        End If
    End Sub

End Class
