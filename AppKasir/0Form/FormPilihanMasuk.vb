Public Class FormPilihanMasuk

    Private Sub FormPilihanMasuk_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Menambahkan pilihan jika belum ditambahkan
        If CmbMasuk.Items.Count = 0 Then
            CmbMasuk.Items.AddRange(New String() {"TOKO", "GUDANG", "SELALU TANYA"})
        End If

        ' Tampilkan pilihan yang tersimpan
        PilihanMasuk()

        ' Tooltip untuk tombol
        ToolTip1.SetToolTip(BtnSimpan, "Simpan pilihan masuk Anda")
        ToolTip1.SetToolTip(BtnClose, "Tutup form tanpa menyimpan")
        ToolTip1.SetToolTip(CmbMasuk,
    "Pilih tujuan masuk:" & vbCrLf &
    "- TOKO        : Masuk ke sistem toko" & vbCrLf &
    "- GUDANG      : Masuk ke sistem gudang" & vbCrLf &
    "- SELALU TANYA: Akan menanyakan pilihan setiap kali aplikasi dibuka")

    End Sub

    Private Sub PilihanMasuk()
        Dim pilihan = AppConfig.Instance.GetValue(Of String)("PilihanMasuk", "").ToUpper()

        If CmbMasuk.Items.Contains(pilihan) Then
            CmbMasuk.SelectedItem = pilihan
        Else
            CmbMasuk.SelectedItem = "TOKO" ' Default
        End If
    End Sub

    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        If CmbMasuk.SelectedItem Is Nothing Then
            MessageBox.Show("Silakan pilih opsi masuk terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        AppConfig.Instance.SetValue("PilihanMasuk", CmbMasuk.SelectedItem.ToString())
        AppConfig.Instance.Save()

        MessageBox.Show("Pilihan masuk berhasil disimpan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Me.Close() ' Optional: Tutup form setelah simpan
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class
