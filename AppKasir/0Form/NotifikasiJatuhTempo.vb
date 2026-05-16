Public Class NotifikasiJatuhTempo
    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const HTCAPTION As Integer = &H2
    Private Sub NotifikasiJatuhTempo_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Panel1-4 = border notifikasi biru (SteelBlue)
        ModuleTheme.SetWarnaPanelNotifBiru(Panel1, Panel2, Panel3, Panel4)
        JumlahJatuhTempo()
    End Sub

    Public Sub JumlahJatuhTempo()
        ' Query SQL untuk menghitung jumlah item yang jatuh tempo
        Dim queryHutang As String = "SELECT COUNT(ID_PEMBELIAN) FROM pembelian WHERE (@Tanggal IS NULL OR JATUH_TEMPO <= @Tanggal) AND STATUS_TRANSAKSI_BELI = 'Belum Lunas'"
        Dim queryPiutang As String = "SELECT COUNT(ID_PENJUALAN) FROM penjualan WHERE (@Tanggal IS NULL OR JATUH_TEMPO <= @Tanggal) AND STATUS_TRANSAKSI = 'Belum Lunas'"

        ' Mendapatkan tanggal dari form utama
        Dim tanggalJatuhTempo As DateTime = Now
        If DateTime.TryParse(FormUtama.StatusTanggal.Text, tanggalJatuhTempo) Then
            ' Mengatur waktu menjadi akhir hari
            tanggalJatuhTempo = tanggalJatuhTempo.AddDays(1).AddTicks(-1)

            ' Format tanggal ke yyyy-MM-dd HH:mm:ss (tanpa milidetik) dan tambahkan wildcard %
            Dim formattedDate As String = tanggalJatuhTempo.ToString("yyyy-MM-dd HH:mm:ss") & "%"

            ' Membuat command untuk query hutang
            Dim commandHutang As New MySqlCommand(queryHutang, conn)
            commandHutang.Parameters.AddWithValue("@Tanggal", formattedDate)

            Dim jumlahHutang As Object = commandHutang.ExecuteScalar()
            If jumlahHutang IsNot Nothing Then
                Dim jumlahHutangInt As Integer = Convert.ToInt32(jumlahHutang)
            End If


            LblHutang.Text = jumlahHutang & " Hutang"

            ' Membuat command untuk query piutang
            Dim commandPiutang As New MySqlCommand(queryPiutang, conn)
            commandPiutang.Parameters.AddWithValue("@Tanggal", formattedDate)

            Dim jumlahPiutang As Integer = Convert.ToInt32(commandPiutang.ExecuteScalar())

            LblPiutang.Text = jumlahPiutang & " Piutang"

            ' Menghitung jumlah total dari kedua query
            Dim jumlahTotal As Integer = jumlahHutang + jumlahPiutang

            ' Menampilkan hasil total (sesuaikan dengan kebutuhan Anda)
            FormUtama.BtnNotif.Text = jumlahTotal.ToString()
        End If
    End Sub


    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        'JumlahJatuhTempo()
        Close()
    End Sub

    Private Sub NotifikasiJatuhTempo_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        Capture = False
        Dim msg As Message = Message.Create(Handle, WM_NCLBUTTONDOWN, New IntPtr(HTCAPTION), IntPtr.Zero)
        DefWndProc(msg)
    End Sub

    Private Sub LinkLabelHutang_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles LinkLabelHutang.LinkClicked
        ' Mengubah kursor menjadi menunggu
        Cursor = Cursors.WaitCursor
        FormNotifHutang.IsiComboBoxSuplier()
        ' Memanggil fungsi TampilHutang() dari FormNotifHutang
        FormNotifHutang.TampilHutang()

        Me.Close()
        ' Menampilkan form FormNotifHutang
        FormNotifHutang.ShowDialog()

        ' Mengembalikan kursor ke default setelah selesai
        Cursor = Cursors.Default
    End Sub

    Private Sub LinkLabelPiutang_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles LinkLabelPiutang.LinkClicked
        ' Mengubah kursor menjadi menunggu
        Cursor = Cursors.WaitCursor

        ' Memanggil fungsi TampilPiutang() dari FormNotifPiutang
        FormNotifPiutang.TampilPiutang()

        Me.Close()
        ' Menampilkan form FormNotifPiutang
        FormNotifPiutang.ShowDialog()

        ' Mengembalikan kursor ke default setelah selesai
        Cursor = Cursors.Default
    End Sub

End Class