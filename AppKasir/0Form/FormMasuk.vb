Public Class FormMasuk
    Private Sub FormMasuk_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ModuleLoyaltyPoin.MuatKonfigurasi()
        Select Case AppConfig.Instance.GetValue(Of String)("PilihanMasuk", "").ToUpper()
            Case "TOKO"
                HandleButtonClick("Toko.jpg", "TOKO")
            Case "GUDANG"
                HandleButtonClick("Gudang.jpg", "GUDANG")
        End Select
    End Sub

    ''' <summary>
    ''' Dipanggil dari FormUtama saat PilihanMasuk sudah tersimpan di config.
    ''' Langsung terapkan lokasi tanpa menampilkan form pilihan.
    ''' </summary>
    Public Sub TerapkanLokasiKeFormUtama(ByVal lokasi As String)
        Dim bg As String = If(lokasi.ToUpper() = "GUDANG", "Gudang.jpg", "Toko.jpg")
        HandleButtonClick(bg, lokasi.ToUpper())
    End Sub

    Private Sub BtnToko_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnToko.Click
        HandleButtonClick("Toko.jpg", "TOKO")
    End Sub

    Private Sub BtnGudang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnGudang.Click
        HandleButtonClick("Gudang.jpg", "GUDANG")
    End Sub

    Public Sub HandleButtonClick(ByVal backgroundImage As String, ByVal lokasi As String)
        ' Set lokasi dan judul form
        FormUtama.Text = "KASIR LANCAR " & lokasi & " " & NAMA_PERUSAHAAN
        FormUtama.StatusLokasi.Text = lokasi

        ' Update icon StatusLokasi sesuai lokasi
        Dim iconName As String = If(lokasi = "GUDANG", "gudang_20.png", "toko_20.png")
        Dim iconPath As String = IO.Path.Combine(Application.StartupPath, "Resources", "Icons", iconName)
        If IO.File.Exists(iconPath) Then
            FormUtama.StatusLokasi.Image = Image.FromFile(iconPath)
        End If

        ' Tampilkan dashboard HTML sesuai lokasi (menggantikan BackgroundImage)
        FormUtama.TampilDashboard()

        Close()
    End Sub




    Private Sub BtnToko_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles BtnToko.GotFocus
        BtnToko.BackColor = Color.Yellow
    End Sub

    Private Sub BtnToko_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles BtnToko.LostFocus
        BtnToko.BackColor = Color.RoyalBlue
    End Sub

    Private Sub BtnGudang_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles BtnGudang.GotFocus
        BtnGudang.BackColor = Color.Yellow
    End Sub

    Private Sub BtnGudang_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles BtnGudang.LostFocus
        BtnGudang.BackColor = Color.RoyalBlue
    End Sub

End Class