Imports System.IO
Imports System.Reflection
Imports System.Text.Json



Public Class FormUtama
    Private ReadOnly originalColor As Color
    Private ReadOnly originalColor1 As Color

    Public Sub ChangeBackgroundImage(ByVal imageFileName As String)
        Dim exePath As String = System.IO.Path.GetDirectoryName(Application.ExecutablePath)
        Dim imagePath As String = System.IO.Path.Combine(exePath, imageFileName)
        If System.IO.File.Exists(imagePath) Then
            Me.BackgroundImage = Image.FromFile(imagePath)
        End If
    End Sub




    Private Sub SetMenuBackgroundColor(ByVal clickedMenu As ToolStripMenuItem)
        ' Reset semua warna menu ke warna asli (originalColor1)
        For Each menu As ToolStripMenuItem In {FileToolStripMenuItem, MenuMaster, MenuTransaksi, MenuJurnal, MenuKaryawan, MenuLaporan, MenuUtility, MenuPosting, HelpToolStripMenuItem, WindowToolStripMenuItem}
            menu.BackColor = originalColor1
        Next
        ' Setel warna latar belakang menu yang diklik
        clickedMenu.BackColor = Color.SandyBrown
    End Sub

    Private Sub SetButtonBackgroundColor(ByVal clickedButton As Button)

        Dim buttons As Button() = {
        BtnToko, BtnBarang, BTnPelanggan, BtnSupliyer, BtnUser, BtnTabelRef, BtnHakAksesUser, BtnKaryawan, BtnArmada,
        BtnBelanja, BtnPenjualan, BtnRetuBelanja, BtnReturPenjualan, BtnBayarHutang, BtnBayarPiutang, BtnStokOpname, BtnPindahStok, BtnTransferBarang,
        BtnSuratJalan
}

        ' Reset semua warna tombol ke warna asli (originalColor)
        For Each button As Button In buttons
            button.BackColor = originalColor
        Next

        ' Setel warna latar belakang tombol yang diklik
        clickedButton.BackColor = Color.White
    End Sub

    Private Sub FormUtama_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Terkunci()

        OpenConnection()

        'DatabaseFile()
        CekaktivasiProgram()

        With FormLogin
            '.MdiParent = Nothing
            .BringToFront()
            .ShowDialog()
        End With


        With FormMasuk
            '.MdiParent = Nothing
            .BringToFront()
            .ShowDialog()
        End With

        With FormLoading
            .Label1.Text = "Selamat datang! Aplikasi saat ini dalam proses inisialisasi dan menunggu konfigurasi data"
            '.MdiParent = Nothing
            .BringToFront()
            .Show()
            .MulaiLoading()
        End With

        ' Panggil untuk mengambil data rekening KAS dan BANK
        Rekeningkasbank()
        ' Panggil untuk mengambil data rekening KAS dan BANK dan MODAL
        AmbilAkunKasBankEkuitas()


        DtpTransaksi.Value = DateTime.Today
        DtpTransaksi.Format = DateTimePickerFormat.Custom
        DtpTransaksi.CustomFormat = "dd/MM/yyyy"
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
    End Sub

    Public Sub AmbilKomputer()
        Dim filePath As String = "printer.ini"

        If File.Exists(filePath) Then
            Using reader As New StreamReader(filePath)
                Do While Not reader.EndOfStream
                    Dim parts As String() = reader.ReadLine().Split("="c)
                    If parts.Length = 2 Then
                        Select Case parts(0).Trim()
                            Case "StatusComp"
                                Comp.Text = parts(1).Trim()
                            Case "JenisPrinterJual"
                                TxtJenisPrinter.Text = parts(1).Trim()
                        End Select
                    End If
                Loop
            End Using
        Else
            Comp.Text = "Server"
            TxtJenisPrinter.Text = "Printer Thermal"
        End If
    End Sub

    Private Function GetIniValue(ByVal filePath As String, ByVal key As String) As String
        Dim value As String = ""
        If File.Exists(filePath) Then
            Using reader As New StreamReader(filePath)
                Dim line As String = reader.ReadLine()
                While line IsNot Nothing
                    Dim parts As String() = line.Split("="c)
                    If parts.Length = 2 Then
                        Dim iniKey As String = parts(0)
                        Dim iniValue As String = parts(1)

                        If iniKey = key Then
                            value = iniValue
                            Exit While
                        End If
                    End If
                    line = reader.ReadLine()
                End While
            End Using
        End If

        Return value
    End Function

    Public Sub Terkunci()
        BtnNotif.Visible = False
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        LogOutToolStripMenuItem.Enabled = False
        MenuMaster.Visible = False
        MenuTransaksi.Visible = False
        MenuJurnal.Visible = False
        MenuKaryawan.Visible = False
        MenuLaporan.Visible = False
        MenuUtility.Visible = False
        MenuPosting.Visible = False
        'HelpToolStripMenuItem.Visible = False
        WindowToolStripMenuItem.Visible = False
        SServer1.Text = "DB :"
        SServer.Text = ""
        STanggal.Text = ""
        SJam.Text = ""
        SVersi.Text = ""
        SServer.Text = ""
        SLogin.Text = ""
        SLevel.Text = ""
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False
        PanelTransaksi.Location = New Point(0, 31)
        PanelTransaksi.Dock = System.Windows.Forms.DockStyle.Top
    End Sub


    Public Sub CekaktivasiProgram()
        Dim serial As Long
        Dim sm As New SecurityManager
        Dim temp As String
        Dim pjg As Integer
        Dim newSerial As String = ""
        serial = sm.GetSerial
        temp = serial
        pjg = temp.Length
        For i As Integer = 1 To pjg
            Dim a As String
            Dim b As Integer
            a = Mid(temp, i, 1)
            b = Asc(a)
            newSerial = newSerial & a & b Mod 2
        Next
        serialTextBox.Text = newSerial

        CheckLicense()

        Dim kg As New KeyGenerator
        Dim key As String = kg.GenerateKey(serialTextBox.Text)

        If activationKeyTextBox.Text <> key Then
            statusLabel.ForeColor = Color.Red
            statusLabel.Text = "Not Activated/invalid key"
            RegristerToolStripMenuItem.Enabled = True

            With ACTIVATION_FORM
                .Activate()
                .BringToFront()
                .ShowDialog()
            End With

        Else
            statusLabel.ForeColor = Color.DarkGreen
            statusLabel.Text = "Activated"
            RegristerToolStripMenuItem.Enabled = False
        End If
    End Sub

    Public Sub CheckLicense()
        Dim kg As New KeyGenerator
        Dim key As String = kg.GenerateKey(serialTextBox.Text)
        If System.IO.File.Exists(bejoLicenseFile) Then
            BejoWriteSettings(bejoLicenseFile, "LICENSE", "serial", serialTextBox.Text)
            activationKeyTextBox.Text = BejoReadSettings(bejoLicenseFile, "LICENSE", "activation_key", "")
        Else
            BejoWriteSettings(bejoLicenseFile, "LICENSE", "serial", serialTextBox.Text)
            activationKeyTextBox.Text = ""
        End If
    End Sub

    Private Sub Timer2_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer2.Tick
        SJam.Text = TimeOfDay
    End Sub

    '----------------------------------------- MAIN MENU ---------------------------------------------------------------------------

    Private Sub FileToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles FileToolStripMenuItem.Click
        SetMenuBackgroundColor(FileToolStripMenuItem)
    End Sub

    Private Sub MenuMaster_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuMaster.Click
        SetMenuBackgroundColor(MenuMaster)

        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = True
        PanelTransaksi.Visible = False
        PanelMaster.Location = New Point(0, 31)
        PanelMaster.Dock = System.Windows.Forms.DockStyle.Top

        For Each frm As Form In MdiChildren
            frm.Close()
        Next

    End Sub

    Private Sub MenuTransaksi_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuTransaksi.Click
        SetMenuBackgroundColor(MenuTransaksi)

        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = False
        PanelTransaksi.Visible = True
        PanelTransaksi.Location = New Point(0, 31)
        PanelTransaksi.Dock = System.Windows.Forms.DockStyle.Top

        For Each frm As Form In MdiChildren
            frm.Close()
        Next

    End Sub

    Private Sub MenuJurnal_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuJurnal.Click
        SetMenuBackgroundColor(MenuJurnal)

        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False

        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormKeuangan
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub


    Private Sub MenuKaryawan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuKaryawan.Click
        SetMenuBackgroundColor(MenuKaryawan)

        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False

        For Each frm As Form In MdiChildren
            frm.Close()
        Next

    End Sub

    Private Sub MenuLaporan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuLaporan.Click
        SetMenuBackgroundColor(MenuLaporan)

        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False
    End Sub

    Private Sub MenuUtility_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuUtility.Click
        SetMenuBackgroundColor(MenuUtility)

        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False

        For Each frm As Form In MdiChildren
            frm.Close()
        Next
    End Sub

    Private Sub MenuPosting_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MenuPosting.Click
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False

        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        ' Tampilkan pesan untuk memastikan sinkronisasi data
        Dim result As DialogResult = MessageBox.Show("Penting! Jangan lupa untuk sering melakukan posting data agar sinkronisasi data tetap terjaga dan tidak terjadi perbedaan data antara sistem dan realita.",
                                                       "Pesan Penting: Posting Data",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Information)

        If result = DialogResult.No Then
            Exit Sub
        End If


        SetMenuBackgroundColor(MenuPosting)


        With FormLoading
            .Label1.Text = "Proses posting! Silahkan menunggu konfigurasi data"
            '.MdiParent = Nothing
            .BringToFront()
            .Show()
            .MulaiPosting()
        End With
        PanelTransaksi.Visible = True
    End Sub

    Private Sub WindowToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles WindowToolStripMenuItem.Click
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False

        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        SetMenuBackgroundColor(WindowToolStripMenuItem)


    End Sub

    Private Sub HelpToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HelpToolStripMenuItem.Click
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        PanelMaster.Visible = False
        PanelTransaksi.Visible = False

        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        SetMenuBackgroundColor(HelpToolStripMenuItem)

        Dim message As String = "TERIMA KASIH ... !!!" & vbCrLf & "UNTUK INFORMASI LEBIH LANJUT TENTANG PROGRAM INI HUBUNGI : 082 335 314 336 / ADI"
        MessageBox.Show(message, "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    '----------------------------------------- FILE ---------------------------------------------------------------------------

    Private Sub LoginToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LoginToolStripMenuItem.Click
        HandleLoginClick()
    End Sub

    Public Sub HandleLoginClick()
        For Each form As Form In Application.OpenForms
            If TypeOf form Is FormLogin Then
                ' Form form_login sudah terbuka, keluar dari sub
                Exit Sub
            End If
        Next

        ' Jika kode mencapai sini, berarti form_login belum terbuka
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        FormLogin.MdiParent = Nothing ' Pastikan properti MdiParent tidak teratur.
        FormLogin.BringToFront()
        FormLogin.ShowDialog()
    End Sub

    Private Sub LogOutToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LogOutToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()

        Call Terkunci()

        LoginToolStripMenuItem.Enabled = True

        With FormLogin
            .MdiParent = Nothing ' Pastikan properti MdiParent tidak teratur.
            .BringToFront()
            .ShowDialog()
        End With

        With FormMasuk
            .MdiParent = Nothing
            .BringToFront()
            .ShowDialog()
        End With

        With FormLoading
            .MdiParent = Nothing
            .BringToFront()
            .Show()
            .MulaiLoading()
        End With
    End Sub

    Private Sub RegristerToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RegristerToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        With ACTIVATION_FORM
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    '----------------------------------------- MASTER ---------------------------------------------------------------------------

    Private Sub BtnToko_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnToko.Click
        CekaktivasiProgram()
        SetButtonBackgroundColor(BtnToko)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next


        With FormCompany
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub BtnBarang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBarang.Click

        SetButtonBackgroundColor(BtnBarang)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormBarang
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With

    End Sub

    Private Sub BtnSupliyer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSupliyer.Click

        SetButtonBackgroundColor(BtnSupliyer)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With TambahSupliyer
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With

    End Sub

    Private Sub BTnPelanggan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTnPelanggan.Click

        SetButtonBackgroundColor(BTnPelanggan)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With TambahPelanggan
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub BtnUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnUser.Click

        SetButtonBackgroundColor(BtnUser)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormUser
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub BtnTabelRef_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTabelRef.Click

        SetButtonBackgroundColor(BtnTabelRef)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormTabelReferensi
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub BtnArmada_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnArmada.Click

        SetButtonBackgroundColor(BtnArmada)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormArmada
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub BtnKaryawan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKaryawan.Click

        SetButtonBackgroundColor(BtnKaryawan)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormKaryawan
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub BtnHakAksesUser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHakAksesUser.Click

        SetButtonBackgroundColor(BtnHakAksesUser)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormHakUser
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub


    '----------------------------------------- TRANSAKSI ---------------------------------------------------------------------------

    Private Sub BtnBelanja_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBelanja.Click
        SetButtonBackgroundColor(BtnBelanja)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Pembelian"
        Panel2.Width = 893
        DGVDetail.Visible = True
        LblDetailTransaksi.Visible = True
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        BtnTambah.Text = "Tambah Pembelian (F2)"
        BTNEdit.Text = "Edit Pembelian (F3)"
        BtnHapus.Text = "Hapus Pembelian (F4)"
        BtnPrint.Text = "Cetak Nota Beli (F5)"
        LblDetailTransaksi.Text = "Detail Pembelian : "

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Pembelian", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BtnPrint.Visible = HakAkses(1) ' CanAdd 
        Datapembelian()
    End Sub

    Private Sub UbahTampilanDataTransaksi()
        With DGVTransaksi
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False


            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Gray
            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering
            DataGridViewExtension.EnableDoubleBuffering(DGVTransaksi)
        End With
    End Sub

    Public Class DataGridViewExtension
        Public Shared Sub EnableDoubleBuffering(ByVal dataGridView As DataGridView)
            dataGridView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty, Nothing, dataGridView, New Object() {True})
        End Sub
    End Class

    Public Sub Datapembelian()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)


        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM(GRAND_TOTAL_BELI) AS TOTAL FROM pembelian WHERE TGL_BELI >= @tanggalAwal AND TGL_BELI <= @tanggalAkhir"
        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not Convert.IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not Convert.IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)

                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & Environment.NewLine & " , Total Belanja: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using

        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()
        Dim queryString As String = "SELECT ID_PEMBELIAN, NAMA_SUPLIYER, LOKASI, JENIS_BAYAR, GRAND_TOTAL_BELI, PEMBAYARAN, RETUR, TAGIHAN, STATUS_TRANSAKSI_BELI, ID_USER FROM pembelian WHERE TGL_BELI >= @tanggalAwal AND TGL_BELI <= @tanggalAkhir ORDER BY ID_PEMBELIAN ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using ds As New DataSet
                da.Fill(ds, "pembelian")
                DGVTransaksi.DataSource = ds.Tables("pembelian")
            End Using
        End Using

        With DGVTransaksi
            ' Pengaturan teks header kolom
            .Columns(0).HeaderText = "NOTA"
            .Columns(1).HeaderText = "SUPLIYER"
            .Columns(2).HeaderText = "LOKASI"
            .Columns(3).HeaderText = "R KREDIT"
            .Columns(4).HeaderText = "TOTAL"
            .Columns(5).HeaderText = "PEMBAYARAN"
            .Columns(6).HeaderText = "RETUR"
            .Columns(7).HeaderText = "HUTANG"
            .Columns(8).HeaderText = "STATUS"
            .Columns(9).HeaderText = "USER"

            ' Pengaturan format dan alignment kolom yang relevan
            .Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(4).DefaultCellStyle.Format = "#,0.##"
            .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(5).DefaultCellStyle.Format = "#,0.##"
            .Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(6).DefaultCellStyle.Format = "#,0.##"
            .Columns(7).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(7).DefaultCellStyle.Format = "#,0.##"

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With
        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
        LblDetailTransaksi.Text = "Detail Pembelian : "
    End Sub

    Private Sub BtnPenjualan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPenjualan.Click
        SetButtonBackgroundColor(BtnPenjualan)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Penjualan"
        Panel2.Width = 893
        DGVDetail.Visible = True
        LblDetailTransaksi.Visible = True
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Penjualan", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BtnPrint.Visible = HakAkses(1) ' CanAdd 

        BtnTambah.Text = "Tambah Penjualan (F2)"
        BTNEdit.Text = "Edit Penjualan (F3)"
        BtnHapus.Text = "Hapus Penjualan (F4)"
        BtnPrint.Text = "Cetak Struk Jual (F5)"
        LblDetailTransaksi.Text = "Detail Penjualan : "
        Datapenjualan()
    End Sub

    Public Sub Datapenjualan()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)


        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM(GRAND_TOTAL_STL_PAJAK) AS TOTAL FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir"
        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not Convert.IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not Convert.IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)

                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & Environment.NewLine & " , Total Penjualan: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using



        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()
        Dim queryString As String = "SELECT ID_PENJUALAN, NAMA_PELANGGAN, LOKASIBARANG, JENIS_PEMBAYARAN, GRAND_TOTAL_STL_PAJAK, BAYAR, KEMBALI, NILAI_RETUR, SISA_TAGIHAN, STATUS_TRANSAKSI, ID_USER FROM penjualan WHERE TGL_TRANSAKSI >= @tanggalAwal AND TGL_TRANSAKSI <= @tanggalAkhir ORDER BY ID_PENJUALAN ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using ds As New DataSet
                da.Fill(ds, "penjualan")
                DGVTransaksi.DataSource = ds.Tables("penjualan")
            End Using
        End Using

        With DGVTransaksi
            ' Nama kolom yang sesuai
            .Columns("ID_PENJUALAN").HeaderText = "NOTA"
            .Columns(0).FillWeight = 130
            .Columns("NAMA_PELANGGAN").HeaderText = "PELANGGAN"
            .Columns("LOKASIBARANG").HeaderText = "LOKASI"
            .Columns("JENIS_PEMBAYARAN").HeaderText = "R DEBET"
            .Columns("GRAND_TOTAL_STL_PAJAK").HeaderText = "TOTAL"
            .Columns("BAYAR").HeaderText = "BAYAR"
            .Columns("KEMBALI").HeaderText = "KEMBALI"
            .Columns("NILAI_RETUR").HeaderText = "RETUR"
            .Columns("SISA_TAGIHAN").HeaderText = "PIUTANG"
            .Columns("STATUS_TRANSAKSI").HeaderText = "STATUS"
            .Columns("ID_USER").HeaderText = "USER"

            Dim currencyStyle As New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleRight,
            .Format = "#,0.##"
        }

            ' Menggunakan nama kolom yang sesuai
            .Columns("GRAND_TOTAL_STL_PAJAK").DefaultCellStyle = currencyStyle
            .Columns("BAYAR").DefaultCellStyle = currencyStyle
            .Columns("NILAI_RETUR").DefaultCellStyle = currencyStyle
            .Columns("KEMBALI").DefaultCellStyle = currencyStyle
            .Columns("SISA_TAGIHAN").DefaultCellStyle = currencyStyle

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With
        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
        LblDetailTransaksi.Text = "Detail Penjualan : "
    End Sub

    Private Sub BtnRetuBelanja_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnRetuBelanja.Click
        SetButtonBackgroundColor(BtnRetuBelanja)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Retur Pembelian"
        Panel2.Width = 893
        DGVDetail.Visible = True
        LblDetailTransaksi.Visible = True
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Retur Pembelian", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BTNEdit.Visible = False
        BtnPrint.Visible = False

        BtnTambah.Text = "Tambah Retur Beli (F2)"
        BTNEdit.Text = "Edit Retur Beli (F3)"
        BtnHapus.Text = "Hapus Retur Beli (F4)"
        BtnPrint.Text = "Cetak Nota Retur Beli (F5)"
        LblDetailTransaksi.Text = "Detail Retur Pembelian : "
        DatareturPembelian()
    End Sub

    Public Sub DatareturPembelian()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)


        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM(TOTAL_RUPIAH) AS TOTAL FROM retur_pembelian WHERE TGL_RETUR_BELI >= @tanggalAwal AND TGL_RETUR_BELI <= @tanggalAkhir"
        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not Convert.IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not Convert.IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)

                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & Environment.NewLine & " , Total Retur Beli: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using


        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()
        Dim queryString As String = "SELECT ID_RETUR_PEMBELIAN, NAMA_SUPPLIER, ID_PEMBELIAN, TGL_PEMBELIAN, PENYIMPANAN, TOTAL_BARANG, TOTAL_RUPIAH, NAMA_REKENING, KODE_REKENING, ID_USER FROM retur_pembelian WHERE TGL_RETUR_BELI >= @tanggalAwal AND TGL_RETUR_BELI <= @tanggalAkhir ORDER BY ID_RETUR_PEMBELIAN ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using ds As New DataSet
                da.Fill(ds, "retur_pembelian")
                DGVTransaksi.DataSource = ds.Tables("retur_pembelian")
            End Using
        End Using

        With DGVTransaksi
            ' Pengaturan teks header kolom
            .Columns(0).HeaderText = "NOTA"
            .Columns(1).HeaderText = "SUPLIYER"
            .Columns(2).HeaderText = "NO BELI"
            .Columns(3).HeaderText = "TGL BELI"
            .Columns(4).HeaderText = "LOKASI"
            .Columns(5).HeaderText = "BARANG"
            .Columns(6).HeaderText = "TOTAL"
            .Columns(7).HeaderText = "REKENING"
            .Columns(8).Visible = False
            .Columns(9).HeaderText = "USER"

            ' Pengaturan format dan alignment kolom yang relevan
            .Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(6).DefaultCellStyle.Format = "#,0.##"

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With
        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
        LblDetailTransaksi.Text = "Detail Retur Pembelian : "
    End Sub

    Private Sub BtnReturPenjualan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnReturPenjualan.Click
        SetButtonBackgroundColor(BtnReturPenjualan)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Retur Penjualan"
        Panel2.Width = 893
        DGVDetail.Visible = True
        LblDetailTransaksi.Visible = True
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Retur Penjualan", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BTNEdit.Visible = False
        BtnPrint.Visible = HakAkses(1) ' CanAdd 

        BtnTambah.Text = "Tambah Retur Jual (F2)"
        BTNEdit.Text = "Edit Retur Jual (F3)"
        BtnHapus.Text = "Hapus Retur Jual (F4)"
        BtnPrint.Text = "Cetak Nota Retur Jual (F5)"
        LblDetailTransaksi.Text = "Detail Retur Penjualan : "
        DataReturPenjualan()
    End Sub

    Public Sub DataReturPenjualan()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)

        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM(TOTAL_RUPIAH) AS TOTAL FROM retur_penjualan WHERE TGL_RETUR_JUAL >= @tanggalAwal AND TGL_RETUR_JUAL <= @tanggalAkhir"
        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not Convert.IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not Convert.IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)

                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & Environment.NewLine & " , Total Retur Jual: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using


        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()
        Dim queryString As String = "SELECT ID_RETUR_PENJUALAN, NAMA_PELANGGAN, ID_PENJUALAN, TGL_PENJUALAN, PENYIMPANAN, TOTAL_BARANG, TOTAL_RUPIAH, NAMA_REKENING, ID_USER FROM retur_penjualan WHERE TGL_RETUR_JUAL >= @tanggalAwal AND TGL_RETUR_JUAL <= @tanggalAkhir ORDER BY ID_RETUR_PENJUALAN ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using ds As New DataSet
                da.Fill(ds, "retur_penjualan")
                DGVTransaksi.DataSource = ds.Tables("retur_penjualan")
            End Using
        End Using

        With DGVTransaksi
            ' Pengaturan teks header kolom
            .Columns(0).HeaderText = "NOTA"
            .Columns(1).HeaderText = "PELANGGAN"
            .Columns(2).HeaderText = "NO JUAL"
            .Columns(3).HeaderText = "TGL JUAL"
            .Columns(4).HeaderText = "LOKASI"
            .Columns(5).HeaderText = "BARANG"
            .Columns(6).HeaderText = "TOTAL"
            .Columns(7).HeaderText = "REKENING"
            .Columns(8).HeaderText = "USER"

            ' Pengaturan format dan alignment kolom yang relevan
            .Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(6).DefaultCellStyle.Format = "#,0.##"

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With
        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
        LblDetailTransaksi.Text = "Detail Retur Penjualan : "
    End Sub

    Private Sub BtnBayarHutang_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBayarHutang.Click
        SetButtonBackgroundColor(BtnBayarHutang)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Bayar Hutang"
        Panel2.Width = 893
        DGVDetail.Visible = True
        LblDetailTransaksi.Visible = True
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Bayar Hutang", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BTNEdit.Visible = False
        BtnPrint.Visible = False

        BtnTambah.Text = "Tambah Bayar Hutang (F2)"
        BTNEdit.Text = "Edit Bayar Hutang (F3)"
        BtnHapus.Text = "Hapus Bayar Hutang (F4)"
        BtnPrint.Text = "Cetak Nota Bayar Hutang (F5)"
        LblDetailTransaksi.Text = "Detail Bayar Hutang : "
        DataBayarHutang()
    End Sub

    Public Sub DataBayarHutang()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)


        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM(NOMINALBAYAR) AS TOTAL FROM hutang WHERE TGLPEMBAYARAN >= @tanggalAwal AND TGLPEMBAYARAN <= @tanggalAkhir"
        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not Convert.IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not Convert.IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)

                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & Environment.NewLine & " , Total Bayar Hutang: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using

        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()
        Dim queryString As String = "SELECT NOBAYARHUTANG, NAMASUPLIYER, TGLPEMBAYARAN, TOTALHUTANG, NOMINALBAYAR, SISAHUTANG, ID_USER_BAYAR FROM hutang WHERE TGLPEMBAYARAN >= @tanggalAwal AND TGLPEMBAYARAN <= @tanggalAkhir ORDER BY NOBAYARHUTANG ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using ds As New DataSet
                da.Fill(ds, "hutang")
                DGVTransaksi.DataSource = ds.Tables("hutang")
            End Using
        End Using

        With DGVTransaksi
            ' Pengaturan teks header kolom
            .Columns(0).HeaderText = "NOTA"
            .Columns(1).HeaderText = "SUPPLIER"
            .Columns(2).HeaderText = "TGL BAYAR"
            .Columns(3).HeaderText = "TOTAL"
            .Columns(4).HeaderText = "BAYAR"
            .Columns(5).HeaderText = "SISA"
            .Columns(6).HeaderText = "USER"

            ' Pengaturan format dan alignment kolom yang relevan
            .Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(3).DefaultCellStyle.Format = "#,0.##"
            .Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(4).DefaultCellStyle.Format = "#,0.##"
            .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(5).DefaultCellStyle.Format = "#,0.##"

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With
        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
        LblDetailTransaksi.Text = "Detail Bayar Hutang : "
    End Sub

    Private Sub BtnBayarPiutang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBayarPiutang.Click
        SetButtonBackgroundColor(BtnBayarPiutang)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Bayar Piutang"
        Panel2.Width = 893
        DGVDetail.Visible = True
        LblDetailTransaksi.Visible = True
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Bayar Piutang", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BTNEdit.Visible = False
        BtnPrint.Visible = HakAkses(1) ' CanAdd 

        BtnTambah.Text = "Tambah Bayar Piutang (F2)"
        BTNEdit.Text = "Edit Bayar Piutang (F3)"
        BtnHapus.Text = "Hapus Bayar Piutang (F4)"
        BtnPrint.Text = "Cetak Nota Bayar Piutang (F5)"
        LblDetailTransaksi.Text = "Detail Bayar Piutang : "
        DataBayarPiutang()
    End Sub

    Public Sub DataBayarPiutang()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)

        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM(NOMINAL_BAYAR) AS TOTAL FROM Piutang WHERE TGL_BAYAR >= @tanggalAwal AND TGL_BAYAR <= @tanggalAkhir"
        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not Convert.IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not Convert.IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)

                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & Environment.NewLine & " , Total Bayar Piutang: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using

        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()
        Dim queryString As String = "SELECT ID_BAYAR_PIUTANG, NAMA_PELANGGAN, TGL_BAYAR, TOTAL_PIUTANG, NOMINAL_BAYAR, SISA_PIUTANG, ID_USER_BAYAR FROM Piutang WHERE TGL_BAYAR >= @tanggalAwal AND TGL_BAYAR <= @tanggalAkhir ORDER BY ID_BAYAR_PIUTANG ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using ds As New DataSet
                da.Fill(ds, "Piutang")
                DGVTransaksi.DataSource = ds.Tables("Piutang")
            End Using
        End Using

        With DGVTransaksi
            ' Pengaturan teks header kolom
            .Columns(0).HeaderText = "NOTA"
            .Columns(1).HeaderText = "PELANGGAN"
            .Columns(2).HeaderText = "TGL BAYAR"
            .Columns(3).HeaderText = "TOTAL"
            .Columns(4).HeaderText = "BAYAR"
            .Columns(5).HeaderText = "SISA"
            .Columns(6).HeaderText = "USER"

            ' Pengaturan format dan alignment kolom yang relevan
            .Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(3).DefaultCellStyle.Format = "#,0.##"
            .Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(4).DefaultCellStyle.Format = "#,0.##"
            .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(5).DefaultCellStyle.Format = "#,0.##"

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With
        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
        LblDetailTransaksi.Text = "Detail Bayar Piutang : "
    End Sub

    Private Sub BtnStokOpname_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStokOpname.Click
        SetButtonBackgroundColor(BtnStokOpname)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Stok Opname"
        Panel2.Width = GBTransaksi.Width - 5
        DGVDetail.Visible = False
        LblDetailTransaksi.Visible = False
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Stok Opname", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BtnPrint.Visible = HakAkses(1) ' CanAdd 

        BtnTambah.Text = "Tambah Stok Opname (F2)"
        BTNEdit.Text = "Edit Stok Opname (F3)"
        BtnHapus.Text = "Hapus Stok Opname (F4)"
        BtnPrint.Text = "Cetak Nota Stok Opname (F5)"
        LblDetailTransaksi.Text = "Detail Stok Opname : "
        DataStokOpname()
    End Sub

    Public Sub DataStokOpname()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)

        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM((TOTAL_QTY)) AS TOTAL FROM stok_opname WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir"
        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not Convert.IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not Convert.IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)

                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & Environment.NewLine & " , Total Selisih: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using


        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()

        Dim queryString As String = "SELECT ID_STOK_OPNAME, LOKASI, ID_BARANG, NAMA_BARANG, KATEGORI, STOK_SYSTEM, STOK_NYATA, STOK_SELISIH, TOTAL_QTY, SATUAN, KETERANGAN, ID_USER FROM stok_opname WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir ORDER BY ID_STOK_OPNAME ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using ds As New DataSet
                da.Fill(ds, "stok_opname")
                DGVTransaksi.DataSource = ds.Tables("stok_opname")
            End Using
        End Using

        With DGVTransaksi
            ' Pengaturan teks header kolom
            .Columns(0).HeaderText = "NOTA"
            .Columns(1).HeaderText = "LOKASI"
            .Columns(2).HeaderText = "KODE"
            .Columns(3).HeaderText = "NAMA BARANG"
            .Columns(4).HeaderText = "KATEGORI"
            .Columns(5).HeaderText = "S SYSTEM"
            .Columns(6).HeaderText = "S NYATA"
            .Columns(7).HeaderText = "SELISIH"
            .Columns(8).Visible = False
            .Columns(9).HeaderText = "SATUAN"
            .Columns(10).HeaderText = "KETERANGAN"
            .Columns(11).HeaderText = "ID USER"

            ' Pengaturan FillWeight untuk kolom stok
            .Columns(5).FillWeight = 60 ' STOK_SYSTEM
            .Columns(6).FillWeight = 60 ' STOK_NYATA
            .Columns(7).FillWeight = 60 ' STOK_SELISIH

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With
        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
    End Sub

    Private Sub BtnSuratJalan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSuratJalan.Click
        SetButtonBackgroundColor(BtnSuratJalan)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Surat Jalan"
        Panel2.Width = 893
        DGVDetail.Visible = True
        LblDetailTransaksi.Visible = True
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Surat Jalan", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BtnPrint.Visible = HakAkses(1) ' CanAdd 

        BtnTambah.Text = "Tambah Surat Jalan (F2)"
        BTNEdit.Text = "Edit Surat Jalan (F3)"
        BtnHapus.Text = "Hapus Surat Jalan (F4)"
        BtnPrint.Text = "Cetak Surat Jalan (F5)"
        LblDetailTransaksi.Text = "Detail Surat Jalan : "
        DataSuratjalan()
    End Sub

    Public Sub DataSuratjalan()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)


        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM(TOTAL_RUPIAH) AS TOTAL FROM Surat_Jalan WHERE TGL_PENGIRIMAN >= @tanggalAwal AND TGL_PENGIRIMAN <= @tanggalAkhir"
        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not Convert.IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not Convert.IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)

                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & Environment.NewLine & " , Total Pengiriman: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using



        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()
        Dim queryString As String = "SELECT NOTA, TOTAL_PELANGGAN, TOTAL_RUPIAH, ARMADA, JENIS_ARMADA, SUPIR, HELPER1, HELPER2, ID_USER FROM Surat_Jalan WHERE TGL_PENGIRIMAN >= @tanggalAwal AND TGL_PENGIRIMAN <= @tanggalAkhir ORDER BY NOTA ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using ds As New DataSet()
                da.Fill(ds, "SuratJalan")
                DGVTransaksi.DataSource = ds.Tables("SuratJalan")
            End Using
        End Using

        With DGVTransaksi
            ' Set appropriate column headers and formats
            .Columns("NOTA").HeaderText = "NOTA"
            .Columns("TOTAL_PELANGGAN").HeaderText = "PELANGGAN"
            .Columns("TOTAL_RUPIAH").HeaderText = "TOTAL RUPIAH"
            .Columns("ARMADA").HeaderText = "ARMADA"
            .Columns("JENIS_ARMADA").HeaderText = "JENIS ARMADA"
            .Columns("SUPIR").HeaderText = "SUPIR"
            .Columns("HELPER1").HeaderText = "HELPER 1"
            .Columns("HELPER2").HeaderText = "HELPER 2"
            .Columns("ID_USER").HeaderText = "USER"

            ' Set the width for some columns
            .Columns("NOTA").FillWeight = 130

            ' Create a currency style for numeric columns
            Dim currencyStyle As New DataGridViewCellStyle With {
                .Alignment = DataGridViewContentAlignment.MiddleRight,
                .Format = "#,0.##"
            }

            ' Apply the currency style to relevant columns
            .Columns("TOTAL_RUPIAH").DefaultCellStyle = currencyStyle

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With
        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
        LblDetailTransaksi.Text = "Detail Surat Jalan : "
    End Sub

    Private Sub BtnTransferBarang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTransferBarang.Click
        SetButtonBackgroundColor(BtnTransferBarang)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Transfer Barang"
        Panel2.Width = 893
        DGVDetail.Visible = True
        LblDetailTransaksi.Visible = True
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Transfer Barang", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BtnPrint.Visible = HakAkses(1) ' CanAdd 

        BtnTambah.Text = "Tambah Transfer Barang (F2)"
        BTNEdit.Text = "Edit Transfer Barang (F3)"
        BtnHapus.Text = "Hapus Transfer Barang (F4)"
        BtnPrint.Text = "Cetak Transfer Barang (F5)"
        LblDetailTransaksi.Text = "Detail Transfer Barang : "
        DataTransferBarang()
    End Sub

    Public Sub DataTransferBarang()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)

        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM(TOTAL_RUPIAH) AS TOTAL " &
                                    "FROM Transfer_Barang " &
                                    "WHERE TGL_TRANSFER >= @tanggalAwal AND TGL_TRANSFER <= @tanggalAkhir"

        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)
                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & ", Total transfer barang: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using

        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()

        Dim queryString As String = "SELECT ID_TRANSFER, LOKASI, TOTAL_QTY, TOTAL_BARANG, TOTAL_RUPIAH, ID_USER " &
                                    "FROM Transfer_Barang " &
                                    "WHERE TGL_TRANSFER >= @tanggalAwal AND TGL_TRANSFER <= @tanggalAkhir " &
                                    "ORDER BY ID_TRANSFER ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using ds As New DataSet()
                da.Fill(ds, "TransferBarang")
                DGVTransaksi.DataSource = ds.Tables("TransferBarang")
            End Using
        End Using

        With DGVTransaksi
            ' Set appropriate column headers and formats
            .Columns("ID_TRANSFER").HeaderText = "ID Transfer"
            .Columns("LOKASI").HeaderText = "Lokasi"
            .Columns("TOTAL_QTY").HeaderText = "Total Qty"
            .Columns("TOTAL_BARANG").HeaderText = "Total Barang"
            .Columns("TOTAL_RUPIAH").HeaderText = "Total Rupiah"
            .Columns("ID_USER").HeaderText = "User"

            ' Set the width for some columns
            .Columns("ID_TRANSFER").FillWeight = 130

            ' Create a currency style for numeric columns
            Dim currencyStyle As New DataGridViewCellStyle With {
                .Alignment = DataGridViewContentAlignment.MiddleRight,
                .Format = "#,0.##"
            }

            ' Apply the currency style to relevant columns
            .Columns("TOTAL_QTY").DefaultCellStyle = currencyStyle
            .Columns("TOTAL_BARANG").DefaultCellStyle = currencyStyle
            .Columns("TOTAL_RUPIAH").DefaultCellStyle = currencyStyle

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With

        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
        LblDetailTransaksi.Text = "Detail Transfer Barang : "
    End Sub

    Private Sub BtnPindahStok_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPindahStok.Click
        SetButtonBackgroundColor(BtnPindahStok)
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        DtpTransaksi.Value = Now
        TxtTransaksi.Text = "Transfer Stok"
        Panel2.Width = GBTransaksi.Width - 5
        DGVDetail.Visible = False
        LblDetailTransaksi.Visible = False
        GBTransaksi.Visible = True
        TxtFakturTransaksi.Clear()

        Dim HakAkses As Boolean() = ModulHakAkses.BacaHakAkses(SLevel.Text, "Transfer Stok", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnTambah.Visible = HakAkses(1) ' CanAdd 
        BTNEdit.Visible = HakAkses(2) ' CanEdit 
        BtnHapus.Visible = HakAkses(3) ' CanDelete 
        BtnPrint.Visible = False
        BTNEdit.Visible = False

        BtnTambah.Text = "Tambah Transfer Stok (F2)"
        BTNEdit.Text = "Edit Transfer Stok (F3)"
        BtnHapus.Text = "Hapus Transfer Stok (F4)"
        BtnPrint.Text = "Cetak Nota Transfer Stok (F5)"
        LblDetailTransaksi.Text = "Detail Transfer Stok : "
        Datatransferstok()

    End Sub

    Public Sub Datatransferstok()
        Dim tanggalAwal As Date = DtpTransaksi.Value.Date
        Dim tanggalAkhir As Date = DtpTransaksi.Value.Date.AddDays(1).AddTicks(-1)

        Dim queryJumlah As String = "SELECT COUNT(*) AS RECORD, SUM(Selisih) AS TOTAL FROM Transfer_stok WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir"
        Using cmdHitungJumlah As New MySqlCommand(queryJumlah, conn)
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdHitungJumlah.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using rdJumlah As MySqlDataReader = cmdHitungJumlah.ExecuteReader()
                If rdJumlah.Read() Then
                    Dim jumlahRecord As Integer = If(Not Convert.IsDBNull(rdJumlah("RECORD")), CInt(rdJumlah("RECORD")), 0)
                    Dim totalBelanja As Decimal = If(Not Convert.IsDBNull(rdJumlah("TOTAL")), CDec(rdJumlah("TOTAL")), 0.0)

                    TxtRangkuman.Text = "Jumlah Record: " & Microsoft.VisualBasic.Format(jumlahRecord, "N0") & Environment.NewLine & " , Total Selisih: Rp. " & Microsoft.VisualBasic.Format(totalBelanja, "N0")
                Else
                    TxtRangkuman.Text = "0"
                End If
            End Using
        End Using

        DGVTransaksi.Columns.Clear()
        DGVDetail.Columns.Clear()
        Dim queryString As String = "SELECT ID_TRANSFER, JENIS_TRANSFER, URAIAN, TANGGAL, ID_BARANG_M, NAMA_BARANG_M, QTY_M, SATUAN_M, ISI_M, QTY_SAT_M, HARGA_SAT_M, TOTAL_HARGA_M, ID_BARANG_K, NAMA_BARANG_K, QTY_K, SATUAN_K, ISI_K, QTY_SAT_K, HARGA_SAT_K, TOTAL_HARGA_K, Selisih, ID_USER FROM Transfer_stok WHERE TANGGAL >= @tanggalAwal AND TANGGAL <= @tanggalAkhir ORDER BY ID_TRANSFER ASC"

        Using da As New MySqlDataAdapter(queryString, conn)
            da.SelectCommand.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            da.SelectCommand.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            Using ds As New DataSet
                da.Fill(ds, "Transfer_stok")
                DGVTransaksi.DataSource = ds.Tables("Transfer_stok")
            End Using
        End Using

        With DGVTransaksi
            ' Pengaturan teks header kolom
            .Columns(0).HeaderText = "NOTA"
            .Columns(1).Visible = False ' JENIS_TRANSFER
            .Columns(2).HeaderText = "URAIAN"
            .Columns(2).FillWeight = 170
            .Columns(3).Visible = False ' "TANGGAL"
            .Columns(4).Visible = False ' ID_BARANG_M
            .Columns(5).HeaderText = "BARANG MASUK"
            .Columns(5).FillWeight = 170
            .Columns(6).HeaderText = "QTY"
            .Columns(6).FillWeight = 50
            .Columns(7).HeaderText = "SAT"
            .Columns(7).FillWeight = 60
            .Columns(8).Visible = False ' ISI_M
            .Columns(9).Visible = False ' QTY_SAT_M
            .Columns(10).Visible = False ' HARGA_SAT_M
            .Columns(11).HeaderText = "HARGA MASUK"
            .Columns(12).Visible = False ' ID_BARANG_K
            .Columns(13).HeaderText = "BARANG KELUAR"
            .Columns(13).FillWeight = 170
            .Columns(14).HeaderText = "QTY"
            .Columns(14).FillWeight = 50
            .Columns(15).HeaderText = "SAT"
            .Columns(15).FillWeight = 60
            .Columns(16).Visible = False ' ISI_K
            .Columns(17).Visible = False ' QTY_SAT_K
            .Columns(18).Visible = False ' HARGA_SAT_K
            .Columns(19).HeaderText = "HARGA KELUAR"
            .Columns(20).HeaderText = "SELISIH"
            .Columns(20).FillWeight = 80
            .Columns(21).HeaderText = "USER"


            ' Pengaturan format dan alignment kolom yang relevan
            .Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(6).DefaultCellStyle.Format = "#,0.##"
            '.Columns(13).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            '.Columns(13).DefaultCellStyle.Format = "N0"
            .Columns(11).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(11).DefaultCellStyle.Format = "#,0.##"
            .Columns(19).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(19).DefaultCellStyle.Format = "#,0.##"
            .Columns(20).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(20).DefaultCellStyle.Format = "#,0.##"

            UbahTampilanDataTransaksi()
            .ClearSelection()
        End With

        TxtFakturTransaksi.Clear()
        TxtLokasiUntukEdit.Clear()
        DGVDetail.Columns.Clear()
    End Sub

    Private Sub BtnRakit_Click(sender As Object, e As EventArgs) Handles BtnRakit.Click

    End Sub

    Private Sub DtpTransaksi_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DtpTransaksi.ValueChanged
        Select Case TxtTransaksi.Text
            Case "Pembelian"
                Datapembelian()
            Case "Penjualan"
                Datapenjualan()
            Case "Retur Pembelian"
                DatareturPembelian()
            Case "Retur Penjualan"
                DataReturPenjualan()
            Case "Bayar Hutang"
                DataBayarHutang()
            Case "Bayar Piutang"
                DataBayarPiutang()
            Case "Stok Opname"
                DataStokOpname()
            Case "Transfer Stok"
                Datatransferstok()
            Case "Surat Jalan"
                DataSuratjalan()
            Case "Transfer Barang"
                DataTransferBarang()
        End Select
    End Sub

    Private Sub FormUtama_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                If BtnTambah.Visible = True Then
                    Tambahtransaksi()
                End If
            Case Keys.F3
                If BTNEdit.Visible = True Then
                    Edittransaksi()
                End If
            Case Keys.F4
                If BtnHapus.Visible = True Then
                    Hapustransaksi()
                End If
            Case Keys.F5
                If BtnPrint.Visible = True Then
                    Cetaktransaksi()
                End If
            Case Keys.Escape
                GBTransaksi.Hide()
        End Select
    End Sub

    Private Sub DGVTransaksi_CellMouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DGVTransaksi.CellMouseUp
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' Memastikan bahwa baris yang diklik valid
            If e.RowIndex < DGVTransaksi.Rows.Count AndAlso DGVTransaksi.Rows(e.RowIndex).Cells(0).Value IsNot Nothing Then
                DGVTransaksi.CurrentCell = DGVTransaksi.Rows(e.RowIndex).Cells(0)
                ' Memanggil event CellClick untuk baris yang valid
                DGVTransaksi_CellClick(sender, New System.Windows.Forms.DataGridViewCellEventArgs(0, e.RowIndex))

                Dim HakAkses As Boolean()
                Select Case TxtTransaksi.Text
                    Case "Pembelian"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Pembelian", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        CetakToolStripMenuItem.Visible = True

                    Case "Penjualan"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Penjualan", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        CetakToolStripMenuItem.Visible = True

                    Case "Retur Pembelian"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Retur Pembelian", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        EditToolStripMenuItem.Visible = False
                        CetakToolStripMenuItem.Visible = False

                    Case "Retur Penjualan"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Retur Penjualan", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        EditToolStripMenuItem.Visible = False
                        CetakToolStripMenuItem.Visible = True

                    Case "Bayar Hutang"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Bayar Hutang", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        EditToolStripMenuItem.Visible = False
                        CetakToolStripMenuItem.Visible = False

                    Case "Bayar Piutang"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Bayar Piutang", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        EditToolStripMenuItem.Visible = False
                        CetakToolStripMenuItem.Visible = True

                    Case "Stok Opname"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Stok Opname", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        CetakToolStripMenuItem.Visible = True

                    Case "Transfer Stok"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Transfer Stok", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        CetakToolStripMenuItem.Visible = False
                        EditToolStripMenuItem.Visible = False
                    Case "Transfer Barang"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Transfer Barang", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        CetakToolStripMenuItem.Visible = True
                    Case "Surat Jalan"
                        HakAkses = ModulHakAkses.BacaHakAkses(SLevel.Text, "Surat Jalan", conn)
                        ' Terapkan nilai hak akses ke tombol-tombol
                        TambahToolStripMenuItem.Visible = HakAkses(1) ' CanAdd 
                        EditToolStripMenuItem.Visible = HakAkses(2) ' CanEdit 
                        HapusToolStripMenuItem.Visible = HakAkses(3) ' CanDelete 
                        CetakToolStripMenuItem.Visible = True
                End Select
                Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
                CMSTransaksi.Show(cursorPosition)

            End If
        End If
    End Sub

    Private Sub TambahToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TambahToolStripMenuItem.Click
        Tambahtransaksi()
        Refresdatagridview()
    End Sub

    Private Sub EditToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditToolStripMenuItem.Click
        Edittransaksi()
        Refresdatagridview()
    End Sub

    Private Sub HapusToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HapusToolStripMenuItem.Click
        Hapustransaksi()
        Refresdatagridview()
    End Sub

    Private Sub CetakToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CetakToolStripMenuItem.Click
        Cetaktransaksi()
    End Sub

    Private Sub BtnTambah_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTambah.Click
        Try
            CekaktivasiProgram()
            Tambahtransaksi()
            Refresdatagridview()
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub Tambahtransaksi()
        Select Case TxtTransaksi.Text
            Case "Pembelian"
                With FormPembelian
                    .LblUtama.Text = "T A M B A H  P E M B E L I A N"
                    .TxtJenisTrans.Text = "TambahPembelian"
                    .DgvData.Rows.Clear()
                    .BringToFront()
                    .ShowDialog()
                End With

            Case "Penjualan"
                With FormPenjualan
                    .TxtJenistransaksi.Text = "TambahPenjualan"
                    .DgvData.Rows.Clear()
                    .BringToFront()
                    .ShowDialog()
                End With

            Case "Retur Pembelian"
                GBTransaksi.Visible = False
                With FormReturPembelian
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Retur Penjualan"
                GBTransaksi.Visible = False
                With FormReturPenjualan
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Bayar Hutang"
                GBTransaksi.Visible = False
                With FormBayarHutang
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Bayar Piutang"
                GBTransaksi.Visible = False
                With FormBayarPiutang
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Stok Opname"
                GBTransaksi.Visible = False
                With FormStokOpname
                    .LblUtama.Text = "TAMBAH STOK OPNAME"
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

            Case "Transfer Stok"
                GBTransaksi.Visible = False

                With FormTransferStok
                    .LblUtama.Text = "TRANSFER STOK ANTAR BARANG DI " & SLokasi.Text
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show() '
                End With

            Case "Transfer Barang"
                With FormTransferBarang
                    If SLokasi.Text = "TOKO" Then
                        .LblLokasiBarang.Text = "TOKO"
                        .LblUtama.Text = "TRANSFER STOK DARI TOKO KE GUDANG"
                    ElseIf SLokasi.Text = "GUDANG" Then
                        .LblLokasiBarang.Text = "GUDANG"
                        .LblUtama.Text = "TRANSFER STOK DARI GUDANG KE TOKO"
                    End If
                    .LblJenisTrans.Text = "TambahTransfer"
                    .DgvData.Rows.Clear()
                    .BringToFront()
                    .ShowDialog() 'Surat Jalan
                End With

            Case "Surat Jalan" '
                GBTransaksi.Visible = False
                With FormSuratJalan
                    .LblJenisTrans.Text = "TambahSuratJalan"
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show() 'Surat Jalan
                End With
        End Select

    End Sub

    Private Sub BTNEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNEdit.Click
        Try
            Edittransaksi()
            Refresdatagridview()
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub Edittransaksi()
        If TxtFakturTransaksi.Text = "" Then
            MessageBox.Show("Pilih Data yang akan di Edit ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        Select Case TxtTransaksi.Text
            Case "Pembelian"

                If SLokasi.Text <> TxtLokasiUntukEdit.Text Then
                    ' Pesan kesalahan jika pengguna tidak memiliki hak untuk menghapus
                    Dim pesan As String = "Oops! Tidak ada hak untuk edit pembelian ini." & Environment.NewLine &
                                          "User " & SLokasi.Text & " tidak berhak edit transaksi pembelian " & TxtLokasiUntukEdit.Text

                    ' Tampilkan MessageBox dengan ikon peringatan
                    MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Dim idPembelian As String = DGVTransaksi.CurrentRow.Cells("ID_PEMBELIAN").Value.ToString()
                Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM retur_pembelian WHERE ID_PEMBELIAN = @ID_PEMBELIAN", conn)
                    cmdCheck.Parameters.AddWithValue("@ID_PEMBELIAN", idPembelian)

                    Dim rowCount As Integer = cmdCheck.ExecuteScalar()

                    If rowCount = 0 Then
                        ' Record tidak ditemukan, lakukan tindakan dengan Form_Pembelian


                        With FormPembelian
                            .CmbJenisBayar.Items.Clear()
                            ' Isi ComboBox dengan data dari list
                            .CmbJenisBayar.Items.AddRange(GetDaftarAkun().ToArray())

                            .LblUtama.Text = "E D I T  P E M B E L I A N"
                            .TxtJenisTrans.Text = "EditPembelian"
                            .TxtFaktur.Text = idPembelian
                            .Tampilsupliyer()
                            .BringToFront()
                            .ShowDialog()
                        End With
                    Else
                        ' Record ditemukan, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat diedit karena terdapat Retur pembelian pada transaksi ini." & Environment.NewLine &
                                              "Silahkan hapus terlebih dahulu Retur pembelian pada transaksi ini jika ingin melanjutkan proses pengeditan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Penjualan"

                If SLokasi.Text <> TxtLokasiUntukEdit.Text Then
                    ' Pesan kesalahan jika pengguna tidak memiliki hak untuk menghapus
                    Dim pesan As String = "Oops! Tidak ada hak untuk edit penjualan ini." & Environment.NewLine &
                                          "User " & SLokasi.Text & " tidak berhak edit transaksi penjualan " & TxtLokasiUntukEdit.Text

                    ' Tampilkan MessageBox dengan ikon peringatan
                    MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Dim idpenjualan As String = DGVTransaksi.CurrentRow.Cells("ID_PENJUALAN").Value.ToString()

                Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM retur_penjualan WHERE ID_PENJUALAN = @ID_PENJUALAN", conn)
                    cmdCheck.Parameters.AddWithValue("@ID_PENJUALAN", idpenjualan)

                    Dim rowCount As Integer = cmdCheck.ExecuteScalar()

                    If rowCount = 0 Then
                        ' Record tidak ditemukan, lakukan tindakan dengan Form_Penjualan
                        With FormPenjualan
                            .TxtJenistransaksi.Text = "EditPenjualan"
                            .TxtFaktur.Text = idpenjualan
                            .TampilPelanggan()
                            .AmbilDataKaryawan()
                            .BringToFront()
                            .ShowDialog()
                        End With
                    Else
                        ' Record ditemukan, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat diedit karena terdapat Retur penjualan pada transaksi ini." & Environment.NewLine &
                                              "Silahkan hapus terlebih dahulu Retur penjualan pada transaksi ini jika ingin melanjutkan proses pengeditan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Retur Pembelian"
                ' tidak membuat edit
            Case "Retur Penjualan"
                ' tidak membuat edit
            Case "Bayar Hutang"
                ' tidak membuat edit
            Case "Bayar Piutang"
                ' tidak membuat edit
            Case "Stok Opname"
                GBTransaksi.Visible = False
                With FormStokOpname
                    .LblUtama.Text = "EDIT STOK OPNAME"
                    .TxtFaktur.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    .TxtQtyUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(8).Value.ToString()
                    .Panel3.Enabled = False
                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With
            Case "Transfer Stok"
                ' tidak membuat edit

            Case "Transfer Barang"
                If SLokasi.Text <> TxtLokasiUntukEdit.Text Then
                    ' Pesan kesalahan jika pengguna tidak memiliki hak untuk menghapus
                    Dim pesan As String = "Oops! Tidak ada hak untuk edit transfer barang ini." & Environment.NewLine &
                                          "User " & SLokasi.Text & " tidak berhak edit transaksi edit barang " & TxtLokasiUntukEdit.Text

                    ' Tampilkan MessageBox dengan ikon peringatan
                    MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                With FormTransferBarang
                    If TxtLokasiUntukEdit.Text = "TOKO" Then
                        .LblLokasiBarang.Text = "TOKO"
                        .LblUtama.Text = "EDIT TRANSFER STOK DARI TOKO KE GUDANG"
                    ElseIf TxtLokasiUntukEdit.Text = "GUDANG" Then
                        .LblLokasiBarang.Text = "GUDANG"
                        .LblUtama.Text = "EDIT TRANSFER STOK DARI GUDANG KE TOKO"
                    End If
                    .LblJenisTrans.Text = "EditTransfer"
                    .TxtFaktur.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    .DgvData.Rows.Clear()
                    .BringToFront()
                    .ShowDialog()
                End With
            Case "Surat Jalan"
                ' Mengedit data Surat Jalan
                With FormSuratJalan
                    GBTransaksi.Visible = False
                    ' Set jenis transaksi
                    .LblJenisTrans.Text = "EditSuratJalan"
                    .AmbilDataArmada()
                    .AmbilDataKaryawan()

                    ' Mengisi data dari DataGridView
                    .LblNoNota.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()

                    Dim armadaIndex As String = DGVTransaksi.CurrentRow.Cells(3).Value.ToString()
                    .CmbArmada.Text = armadaIndex

                    Dim sopir As String = DGVTransaksi.CurrentRow.Cells(5).Value.ToString()
                    .CmbSopir.Text = sopir

                    Dim helper1 As String = DGVTransaksi.CurrentRow.Cells(6).Value.ToString()
                    .CmbHelper1.Text = helper1

                    Dim helper2 As String = DGVTransaksi.CurrentRow.Cells(7).Value.ToString()
                    .CmbHelper2.Text = helper2

                    ' Mengatur tanggal
                    .DtpSuratJalan.Value = DtpTransaksi.Value
                    .DtpPenjualan.Value = DtpTransaksi.Value

                    ' Membersihkan dan menampilkan DataGridView Surat Jalan
                    .DGVSuratJalan.Rows.Clear()


                    .MdiParent = Me
                    .BringToFront()
                    .Dock = DockStyle.Fill
                    .Show()
                End With

        End Select

    End Sub

    Private Sub BtnHapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnHapus.Click
        Try
            Hapustransaksi()
            Refresdatagridview()
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CekLokasiBarang()
        If SLokasi.Text <> TxtLokasiUntukEdit.Text Then
            ' Pesan kesalahan jika pengguna tidak memiliki hak untuk menghapus
            Dim pesan As String = "Oops! Tidak ada hak untuk menghapus transaksi ini." & Environment.NewLine &
                                  "Karena login di " & SLokasi.Text & " tidak berhak menghapus/edit transaksi " & TxtLokasiUntukEdit.Text

            ' Tampilkan MessageBox dengan ikon peringatan
            MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
    End Sub

    Private Sub Hapustransaksi()
        If TxtFakturTransaksi.Text = "" Then
            MessageBox.Show("Pilih Data yang akan di Hapus ... !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Select Case TxtTransaksi.Text
            Case "Pembelian"

                CekLokasiBarang()

                Dim idPembelian As String = DGVTransaksi.CurrentRow.Cells("ID_PEMBELIAN").Value.ToString()

                Using cmdCheck As New MySqlCommand("SELECT COUNT(ID_RETUR_PEMBELIAN) FROM retur_pembelian WHERE ID_PEMBELIAN = @ID_PEMBELIAN", conn)
                    cmdCheck.Parameters.AddWithValue("@ID_PEMBELIAN", idPembelian)

                    Dim rowCount As Integer = cmdCheck.ExecuteScalar()

                    If rowCount = 0 Then
                        ' Record tidak ditemukan, lakukan tindakan dengan Form_Pembelian
                        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                            Hapusbelanja()
                        End If
                    Else
                        ' Record ditemukan, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat hapus karena terdapat Retur pembelian pada transaksi ini." & Environment.NewLine &
                                              "Silahkan hapus terlebih dahulu Retur pembelian pada transaksi ini jika ingin melanjutkan proses penghapusan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Penjualan"

                CekLokasiBarang()

                Dim idpenjualan As String = DGVTransaksi.CurrentRow.Cells("ID_PENJUALAN").Value.ToString()

                Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM retur_penjualan WHERE ID_PENJUALAN = @ID_PENJUALAN", conn)
                    cmdCheck.Parameters.AddWithValue("@ID_PENJUALAN", idpenjualan)

                    Dim rowCount As Integer = cmdCheck.ExecuteScalar()

                    If rowCount = 0 Then
                        ' Record tidak ditemukan, lakukan tindakan dengan Form_Penjualan
                        Hapuspenjualan()
                    Else
                        ' Record ditemukan, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat dihapus karena terdapat Retur penjualan pada transaksi ini." & Environment.NewLine &
                                              "Silahkan hapus terlebih dahulu Retur penjualan pada transaksi ini jika ingin melanjutkan proses penghapusan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Retur Pembelian"

                CekLokasiBarang()

                Dim idPembelian As String = DGVTransaksi.CurrentRow.Cells("ID_PEMBELIAN").Value.ToString()

                ' Query untuk mendapatkan NOBAYARHUTANG
                Dim queryNobayar As String = "SELECT ID_BAYAR FROM Hutang_Detail WHERE ID_BELI = @ID_BELI"
                Using cmdNobayar As New MySqlCommand(queryNobayar, conn)
                    cmdNobayar.Parameters.AddWithValue("@ID_BELI", idPembelian)

                    ' Mengecek NOBAYARHUTANG
                    Dim noBayarHutang As Object = cmdNobayar.ExecuteScalar()

                    If noBayarHutang Is Nothing OrElse IsDBNull(noBayarHutang) OrElse String.IsNullOrWhiteSpace(noBayarHutang.ToString()) Then
                        ' NOBAYARHUTANG kosong atau null, maka lanjutkan dengan menghapus retur pembelian
                        Hapusreturpembelian()
                        'RefreshDataGridView()
                    Else
                        ' NOBAYARHUTANG tidak kosong, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat dihapus karena terdapat pembayaran hutang terkait." & Environment.NewLine &
                                              "Silakan batalkan pembayaran hutang terlebih dahulu jika ingin melanjutkan proses penghapusan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Retur Penjualan"
                CekLokasiBarang()

                Dim idpenjualan As String = DGVTransaksi.CurrentRow.Cells("ID_PENJUALAN").Value.ToString()

                ' Query untuk mendapatkan NOBAYARHUTANG
                Dim queryNobayar As String = "SELECT ID_BAYAR FROM Piutang_Detail WHERE ID_JUAL = @ID_JUAL"
                Using cmdNobayar As New MySqlCommand(queryNobayar, conn)
                    cmdNobayar.Parameters.AddWithValue("@ID_JUAL", idpenjualan)

                    ' Mengecek NOBAYARHUTANG
                    Dim idBayarPiutang As Object = cmdNobayar.ExecuteScalar()

                    If idBayarPiutang Is Nothing OrElse IsDBNull(idBayarPiutang) OrElse String.IsNullOrWhiteSpace(idBayarPiutang.ToString()) Then
                        ' NOBAYARHUTANG kosong atau null, maka lanjutkan dengan menghapus retur pembelian
                        Hapusreturpenjualan()
                    Else
                        ' NOBAYARHUTANG tidak kosong, tampilkan pesan keren
                        Dim pesan As String = "Oops! Transaksi ini tidak dapat dihapus karena terdapat pembayaran piutang terkait." & Environment.NewLine &
                                              "Silakan batalkan pembayaran piutang terlebih dahulu jika ingin melanjutkan proses penghapusan."

                        ' Tampilkan MessageBox dengan ikon peringatan
                        MessageBox.Show(pesan, "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

            Case "Bayar Hutang"
                Hapusbayarhutang()
            Case "Bayar Piutang"
                HapusbayarPiutang()
            Case "Stok Opname"
                CekLokasiBarang()
                Hapusstokopname()
            Case "Transfer Stok"
                CekLokasiBarang()
                Hapustransferstok()
            Case "Surat Jalan"
                HapusSuratJalan()
            Case "Transfer Barang"
                CekLokasiBarang()
                HapusTransferBarang()
        End Select

    End Sub

    Public Sub Hapusbelanja()

        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Dim updateStokField As String = String.Empty

            Select Case TxtLokasiUntukEdit.Text
                Case "TOKO"
                    updateStokField = "PEMBELIAN_TOKO"
                Case "GUDANG"
                    updateStokField = "PEMBELIAN_GUDANG"
            End Select

            Dim updateQuery As String = "UPDATE tbl_barang SET HARGA_BELI = ?, HARGA_BELI_TERAKHIR = ?, " & updateStokField & " = " & updateStokField & " - ? WHERE ID_BARANG = ?"


            For Each row As DataGridViewRow In DGVDetail.Rows
                If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                    Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()
                    Dim stokPengurangan As Decimal = If(row.Cells("QTY_SAT").Value IsNot Nothing, CDec(row.Cells("QTY_SAT").Value), 0)
                    Dim Hargabeli As Decimal = If(IsDBNull(row.Cells("HARGA_AVERAGE").Value), 0, CDec(row.Cells("HARGA_AVERAGE").Value))
                    Dim HARGA_BELI_SEBELUMNYA As Decimal = If(IsDBNull(row.Cells("HARGA_BELI_SEBELUMNYA").Value), 0, CDec(row.Cells("HARGA_BELI_SEBELUMNYA").Value))


                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@Hargabeli", Hargabeli)
                        cmd.Parameters.AddWithValue("@HARGA_BELI_SEBELUMNYA", HARGA_BELI_SEBELUMNYA)
                        cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            Next

            Dim deleteQueries As String() = {
            "DELETE FROM pembelian WHERE ID_PEMBELIAN = @FakturPembelian",
            "DELETE FROM pembelian_detail WHERE FAKTUR_BELI = @FakturPembelian",
            "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FakturPembelian",
            "DELETE FROM HistoryBarang WHERE FAKTUR = @FakturPembelian"
        }

            For Each query As String In deleteQueries
                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.Parameters.AddWithValue("@FakturPembelian", TxtFakturTransaksi.Text)
                    cmd.ExecuteNonQuery()
                End Using
            Next
            transaction.Commit()

            DatabaseModule.CatatanAksiHistory("Hapus Pembelian " & TxtFakturTransaksi.Text)

            For Each row As DataGridViewRow In DGVDetail.Rows
                If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                    HitungByKode(row.Cells(0).Value)
                End If
            Next

        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                                "Detail kesalahan: " & ex.Message,
                                                "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub

    Public Sub Hapuspenjualan()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim updateStokField As String = ""

            Select Case TxtLokasiUntukEdit.Text
                Case "TOKO" : updateStokField = "PENJUALAN_TOKO"
                Case "GUDANG" : updateStokField = "PENJUALAN_GUDANG"
            End Select

            Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " - ? WHERE ID_BARANG = ?"
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()
                        Dim stokPengurangan As Decimal = If(row.Cells("QTY_SATUAN").Value IsNot Nothing, CDec(row.Cells("QTY_SATUAN").Value), 0)

                        Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                            cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                            cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                Next

                Dim deleteQueries As String() = {
                    "DELETE FROM penjualan WHERE ID_PENJUALAN = @FakturPenjualan",
                    "DELETE FROM penjualan_detail WHERE FAKTUR_JUAL = @FakturPenjualan",
                    "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FakturPenjualan",
                    "DELETE FROM HistoryBarang WHERE FAKTUR = @FakturPenjualan"
                }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@FakturPenjualan", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Hapus penjualan " & TxtFakturTransaksi.Text)

                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        HitungByKode(row.Cells(0).Value)
                    End If
                Next

            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                "Detail kesalahan: " & ex.Message,
                                "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub Hapusreturpembelian()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim updateStokField As String

            Select Case TxtLokasiUntukEdit.Text
                Case "TOKO" : updateStokField = "RETUR_BELI_TOKO"
                Case "GUDANG" : updateStokField = "RETUR_BELI_GUDANG"
                Case Else
                    Throw New InvalidOperationException("Lokasi barang tidak valid.")
            End Select

            Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " - ? WHERE ID_BARANG = ?"

            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                ' Update stok barang untuk setiap item penjualan
                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()
                        Dim stokPengurangan As Decimal = If(row.Cells("QTY_SAT").Value IsNot Nothing, CDec(row.Cells("QTY_SAT").Value), 0)

                        Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                            cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                            cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                Next

                Dim kodeRekening As String = DGVTransaksi.CurrentRow.Cells("KODE_REKENING").Value.ToString()
                Dim totalRupiah As Decimal = DGVTransaksi.CurrentRow.Cells("TOTAL_RUPIAH").Value
                Dim idPembelian As String = DGVTransaksi.CurrentRow.Cells("ID_PEMBELIAN").Value.ToString()

                ' Update pembelian untuk retur
                Dim updateReturQuery As String = If(kodeRekening = Kode_rek_Hutang_Beli, "UPDATE pembelian SET RETUR = RETUR - ?, TAGIHAN = TAGIHAN + ? WHERE ID_PEMBELIAN = ?", "UPDATE pembelian SET RETUR = RETUR - ? WHERE ID_PEMBELIAN = ?")

                Using cmdUpdateRetur As New MySqlCommand(updateReturQuery, conn, transaction)
                    cmdUpdateRetur.Parameters.AddWithValue("@RETUR", totalRupiah)

                    If kodeRekening = Kode_rek_Hutang_Beli Then
                        cmdUpdateRetur.Parameters.AddWithValue("@TAGIHAN", totalRupiah)
                    End If

                    cmdUpdateRetur.Parameters.AddWithValue("@ID_PEMBELIAN", idPembelian)
                    cmdUpdateRetur.ExecuteNonQuery()
                End Using

                ' Hapus data retur_pembelian
                Dim deleteQueries As String() = {
                    "DELETE FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN = @FAKTUR",
                    "DELETE FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = @FAKTUR",
                    "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FAKTUR",
                    "DELETE FROM HistoryBarang WHERE FAKTUR = @FAKTUR"
                }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@FAKTUR", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Commit transaksi jika berhasil
                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Hapus retur pembelian " & TxtFakturTransaksi.Text)

                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        HitungByKode(row.Cells(0).Value)
                    End If
                Next

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                 "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try
        End If
    End Sub

    Private Sub Hapusreturpenjualan()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim updateStokField As String = If(TxtLokasiUntukEdit.Text = "TOKO", "RETUR_JUAL_TOKO", If(TxtLokasiUntukEdit.Text = "GUDANG", "RETUR_JUAL_GUDANG", ""))

            Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " - ? WHERE ID_BARANG = ?"

            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()

                        If Not String.IsNullOrEmpty(kodeBarang) Then
                            Dim cellValue = row.Cells("QTY_SATUAN").Value
                            Dim stokPengurangan As Decimal = If(cellValue IsNot Nothing, cellValue.ToString(), "0")


                            Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                                cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                                cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                                cmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                Next

                Dim idPenjualan As String = DGVTransaksi.CurrentRow.Cells("ID_PENJUALAN").Value.ToString()
                Dim nilaiRetur As Decimal = DGVTransaksi.CurrentRow.Cells("TOTAL_RUPIAH").Value


                Dim tglReturJual As DateTime? = Nothing ' Variabel untuk menyimpan TGL_RETUR_JUAL

                ' Query untuk mengambil TGL_RETUR_JUAL terbaru dan kedua berdasarkan ID_PENJUALAN
                Dim query As String = "SELECT TGL_RETUR_JUAL FROM retur_penjualan WHERE ID_PENJUALAN = @ID_PENJUALAN ORDER BY TGL_RETUR_JUAL DESC LIMIT 2"

                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.Parameters.AddWithValue("@ID_PENJUALAN", idPenjualan)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        ' Cek apakah ada hasil
                        If reader.HasRows Then
                            ' Membaca baris pertama (terbaru)
                            reader.Read()

                            ' Cek apakah ada baris kedua
                            If reader.Read() Then
                                ' Simpan nilai TGL_RETUR_JUAL dalam variabel dari baris kedua
                                tglReturJual = reader.GetDateTime(0)
                            End If
                        End If
                    End Using
                End Using


                Dim updatePenjualanQuery As String

                If DGVTransaksi.CurrentRow.Cells(7).Value = "TAGIHAN / SALDO PIUTANG" Then
                    updatePenjualanQuery = "UPDATE penjualan SET TGL_RETUR = @TGL_RETUR, NILAI_RETUR = NILAI_RETUR - @NILAI_RETUR, SISA_TAGIHAN = SISA_TAGIHAN + @SISA_TAGIHAN WHERE ID_PENJUALAN = @ID_PENJUALAN"
                Else
                    updatePenjualanQuery = "UPDATE penjualan SET TGL_RETUR = @TGL_RETUR, NILAI_RETUR = NILAI_RETUR - @NILAI_RETUR WHERE ID_PENJUALAN = @ID_PENJUALAN"
                End If

                Using updatePenjualanCmd As New MySqlCommand(updatePenjualanQuery, conn, transaction)
                    If tglReturJual.HasValue Then
                        updatePenjualanCmd.Parameters.AddWithValue("@TGL_RETUR", tglReturJual.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    Else
                        updatePenjualanCmd.Parameters.AddWithValue("@TGL_RETUR", DBNull.Value)
                    End If

                    updatePenjualanCmd.Parameters.AddWithValue("@NILAI_RETUR", nilaiRetur)

                    If DGVTransaksi.CurrentRow.Cells(7).Value = "TAGIHAN / SALDO PIUTANG" Then
                        updatePenjualanCmd.Parameters.AddWithValue("@SISA_TAGIHAN", nilaiRetur)
                    End If

                    updatePenjualanCmd.Parameters.AddWithValue("@ID_PENJUALAN", idPenjualan)
                    ' Eksekusi query
                    updatePenjualanCmd.ExecuteNonQuery()
                End Using


                Dim deleteQueries As String() = {
      "DELETE FROM retur_penjualan WHERE ID_RETUR_PENJUALAN = @FAKTUR",
      "DELETE FROM retur_penjualan_detail WHERE ID_RETUR_PENJUALAN = @FAKTUR",
      "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FAKTUR",
      "DELETE FROM HistoryBarang WHERE FAKTUR = @FAKTUR"
  }

                For Each sqlQuery As String In deleteQueries
                    Using cmd As New MySqlCommand(sqlQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@FAKTUR", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next


                ' Commit transaksi jika berhasil
                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Hapus retur penjualan " & TxtFakturTransaksi.Text)

                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        HitungByKode(row.Cells(0).Value)
                    End If
                Next

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                 "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try
        End If
    End Sub

    Private Sub Hapusbayarhutang()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                For Each row As DataGridViewRow In DGVDetail.Rows
                    Using cmdUpdateBeli As New MySqlCommand("UPDATE pembelian SET PEMBAYARAN = PEMBAYARAN - @PEMBAYARAN, TAGIHAN = TAGIHAN + @TAGIHAN, TGL_BAYAR = NULL, NOMINALBAYAR = NOMINALBAYAR - @NOMINALBAYAR, STATUS_TRANSAKSI_BELI = 'Belum Lunas' WHERE ID_PEMBELIAN = @ID_PEMBELIAN", conn, transaction)

                        ' Menggunakan default 0 jika nilai tidak valid
                        Dim nominalBayar As Decimal = If(IsDBNull(row.Cells("PEMBAYARAN").Value) OrElse row.Cells("PEMBAYARAN").Value Is Nothing, 0D, Convert.ToDecimal(row.Cells("PEMBAYARAN").Value))

                        cmdUpdateBeli.Parameters.AddWithValue("@PEMBAYARAN", nominalBayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@TAGIHAN", nominalBayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@NOMINALBAYAR", nominalBayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@ID_PEMBELIAN", row.Cells("ID_BELI").Value)

                        ' Eksekusi perintah
                        cmdUpdateBeli.ExecuteNonQuery()
                    End Using
                Next


                Dim deleteQueries As String() = {
                    "DELETE FROM hutang WHERE NOBAYARHUTANG = @NO_TRANSAKSI",
                    "DELETE FROM Hutang_Detail WHERE ID_BAYAR = @NO_TRANSAKSI",
                    "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @NO_TRANSAKSI"
                }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Commit transaksi jika berhasil
                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Hapus bayar hutang " & TxtFakturTransaksi.Text)

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try
        End If
    End Sub

    Private Sub HapusbayarPiutang()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try 'SELECT ID_JUAL, KODE, NAMA, DIBAYAR, PIUTANG, TANGGAL_BAYAR, PEMBAYARAN, STATUS FROM Piutang_Detail
                For Each row As DataGridViewRow In DGVDetail.Rows

                    Using cmdUpdatePembelian As New MySqlCommand("UPDATE penjualan SET BAYAR = BAYAR - @BAYAR, SISA_TAGIHAN = SISA_TAGIHAN + @SISA_TAGIHAN, TGL_PEMBAYARAN = NULL, NOMINALBAYARPIUTANG = NOMINALBAYARPIUTANG - @NOMINALBAYARPIUTANG, STATUS_TRANSAKSI = 'Belum Lunas' WHERE ID_PENJUALAN = @ID_PENJUALAN", conn, transaction)

                        ' Menggunakan variabel untuk nilai BAYAR
                        Dim bayar As Decimal = If(IsDBNull(row.Cells(6).Value) OrElse row.Cells(6).Value Is Nothing, 0D, CDec(row.Cells(6).Value))

                        ' Menambahkan parameter dengan nilai yang sudah dicek
                        cmdUpdatePembelian.Parameters.AddWithValue("@BAYAR", bayar)
                        cmdUpdatePembelian.Parameters.AddWithValue("@SISA_TAGIHAN", bayar) ' Menggunakan nilai yang sama untuk SISA_TAGIHAN
                        cmdUpdatePembelian.Parameters.AddWithValue("@NOMINALBAYARPIUTANG", bayar) ' Menggunakan nilai yang sama untuk NOMINALBAYARPIUTANG
                        cmdUpdatePembelian.Parameters.AddWithValue("@ID_PENJUALAN", row.Cells(0).Value)

                        ' Eksekusi perintah
                        cmdUpdatePembelian.ExecuteNonQuery()
                    End Using

                Next

                Dim deleteQueries As String() = {
                    "DELETE FROM Piutang WHERE ID_BAYAR_PIUTANG = @NO_TRANSAKSI",
                    "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @NO_TRANSAKSI",
                    "DELETE FROM Piutang_Detail WHERE ID_BAYAR = @NO_TRANSAKSI"
                }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@NO_TRANSAKSI", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Commit transaksi jika berhasil
                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Hapus bayar piutang " & TxtFakturTransaksi.Text)

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try
        End If
    End Sub

    Public Sub Hapusstokopname()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = Nothing

            Try
                ' Mulai transaksi
                transaction = conn.BeginTransaction()

                Dim updateQuery As String = ""
                Dim stokField As String = ""

                Select Case TxtLokasiUntukEdit.Text
                    Case "TOKO"
                        stokField = "OPNAME_TOKO"
                    Case "GUDANG"
                        stokField = "OPNAME_GUDANG"
                End Select

                updateQuery = "UPDATE tbl_barang SET " & stokField & " = " & stokField & " - ? WHERE ID_BARANG = ?"

                Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                    cmd.Parameters.AddWithValue("@STOK_OPNAME", CDec(DGVTransaksi.CurrentRow.Cells(8).Value))
                    cmd.Parameters.AddWithValue("@ID_BARANG", DGVTransaksi.CurrentRow.Cells(2).Value.ToString())
                    cmd.ExecuteNonQuery()
                End Using


                Dim deleteQueries As String() = {
                          "DELETE FROM Stok_Opname WHERE ID_STOK_OPNAME = @ID_STOK_OPNAME",
                          "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @ID_STOK_OPNAME",
                          "DELETE FROM HistoryBarang WHERE FAKTUR = @ID_STOK_OPNAME"
                      }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@ID_STOK_OPNAME", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Commit transaksi jika berhasil
                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Hapus stok opname " & TxtFakturTransaksi.Text)

                HitungByKode(DGVTransaksi.CurrentRow.Cells(2).Value.ToString())

            Catch ex As Exception
                transaction.Rollback()

                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                "Detail kesalahan: " & ex.Message,
                                "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub Hapustransferstok()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then

            Dim idBarangMasuk As String = DGVTransaksi.CurrentRow.Cells(4).Value.ToString()
            Dim idBarangKeluar As String = DGVTransaksi.CurrentRow.Cells(12).Value.ToString()

            ' Mulai transaksi
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                Dim qtySatMasuk As Decimal = If(String.IsNullOrEmpty(DGVTransaksi.CurrentRow.Cells(9).Value.ToString()), 0, Decimal.Parse(DGVTransaksi.CurrentRow.Cells(9).Value.ToString()))
                Dim qtySatKeluar As Decimal = If(String.IsNullOrEmpty(DGVTransaksi.CurrentRow.Cells(17).Value.ToString()), 0, Decimal.Parse(DGVTransaksi.CurrentRow.Cells(17).Value.ToString()))

                Dim queryUpdateStokMasuk As String = String.Empty
                Dim queryUpdateStokKeluar As String = String.Empty

                ' Tentukan query berdasarkan lokasi
                Select Case TxtLokasiUntukEdit.Text
                    Case "GUDANG"
                        queryUpdateStokMasuk = "UPDATE tbl_barang SET TRANSFER_STOK_MASUK_GUDANG = TRANSFER_STOK_MASUK_GUDANG - ? WHERE ID_BARANG = ?"
                        queryUpdateStokKeluar = "UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_GUDANG = TRANSFER_STOK_KELUAR_GUDANG - ? WHERE ID_BARANG = ?"
                    Case "TOKO"
                        queryUpdateStokMasuk = "UPDATE tbl_barang SET TRANSFER_STOK_MASUK_TOKO = TRANSFER_STOK_MASUK_TOKO - ? WHERE ID_BARANG = ?"
                        queryUpdateStokKeluar = "UPDATE tbl_barang SET TRANSFER_STOK_KELUAR_TOKO = TRANSFER_STOK_KELUAR_TOKO - ? WHERE ID_BARANG = ?"
                End Select

                ' Update stok masuk
                Using cmdUpdateStok As New MySqlCommand(queryUpdateStokMasuk, conn, transaction)
                    cmdUpdateStok.Parameters.AddWithValue("@QtySat", qtySatMasuk)
                    cmdUpdateStok.Parameters.AddWithValue("@ID_BARANG", idBarangMasuk)
                    cmdUpdateStok.ExecuteNonQuery()
                End Using

                ' Update stok keluar
                Using cmdUpdateKeluar As New MySqlCommand(queryUpdateStokKeluar, conn, transaction)
                    cmdUpdateKeluar.Parameters.AddWithValue("@QtySat", qtySatKeluar)
                    cmdUpdateKeluar.Parameters.AddWithValue("@ID_BARANG", idBarangKeluar)
                    cmdUpdateKeluar.ExecuteNonQuery()
                End Using


                Dim deleteQueries As String() = {
                  "DELETE FROM Transfer_stok WHERE ID_TRANSFER = @ID_TRANSFER",
                  "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @ID_TRANSFER",
                  "DELETE FROM HistoryBarang WHERE FAKTUR = @ID_TRANSFER"
              }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@ID_TRANSFER", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next


                ' Commit transaksi jika berhasil
                transaction.Commit()


                DatabaseModule.CatatanAksiHistory("Hapus transfer stok " & TxtFakturTransaksi.Text)

                HitungByKode(idBarangMasuk)
                HitungByKode(idBarangKeluar)


            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                 "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try


        End If
    End Sub

    Private Sub HapusSuratJalan()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then

            ' Mulai transaksi
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try

                Dim deleteQueries As String() = {
                  "DELETE FROM Surat_Jalan WHERE NOTA = @NOTA",
                  "DELETE FROM Surat_Jalan_Detail WHERE NOTA = @NOTA"
              }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@NOTA", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Commit transaksi jika berhasil
                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Hapus surat jalan " & TxtFakturTransaksi.Text)

            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                 "Detail kesalahan: " & ex.Message,
                                 "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If
    End Sub

    Private Sub HapusTransferBarang()
        If MessageBox.Show("Apakah data ini akan dihapus ...???", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then

            ' Mulai transaksi
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                Dim stokKeluarField As String
                Dim stokMasukField As String

                Select Case TxtLokasiUntukEdit.Text
                    Case "TOKO"
                        stokKeluarField = "TRANSFER_BARANG_KELUAR_TOKO"
                        stokMasukField = "TRANSFER_BARANG_MASUK_GUDANG"
                    Case "GUDANG"
                        stokKeluarField = "TRANSFER_BARANG_KELUAR_GUDANG"
                        stokMasukField = "TRANSFER_BARANG_MASUK_TOKO"
                    Case Else
                        Throw New Exception("Lokasi barang tidak valid.")
                End Select

                Dim updateQuery As String = "UPDATE tbl_barang SET " & stokKeluarField & " = " & stokKeluarField & " - ?, " & stokMasukField & " = " & stokMasukField & " - ? WHERE ID_BARANG = ?"

                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()

                        If Not String.IsNullOrEmpty(kodeBarang) Then
                            Dim qtySat As Decimal = If(row.Cells("TOTAL_QTY").Value IsNot Nothing, Convert.ToDecimal(row.Cells("TOTAL_QTY").Value), 0D)

                            Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                                cmd.Parameters.AddWithValue("@QtySatKeluar", qtySat)
                                cmd.Parameters.AddWithValue("@QtySatMasuk", qtySat)
                                cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                                cmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                Next

                Dim deleteQueries As String() = {
                    "DELETE FROM Transfer_Barang WHERE ID_TRANSFER = @ID_TRANSFER",
                    "DELETE FROM Transfer_Barang_Detail WHERE ID_TRANSFER = @ID_TRANSFER",
                    "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @ID_TRANSFER",
                    "DELETE FROM HistoryBarang WHERE FAKTUR = @ID_TRANSFER"
                }

                For Each query As String In deleteQueries
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@ID_TRANSFER", TxtFakturTransaksi.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                ' Commit transaksi jika berhasil
                transaction.Commit()

                DatabaseModule.CatatanAksiHistory("Hapus transfer barang " & TxtFakturTransaksi.Text)

                For Each row As DataGridViewRow In DGVDetail.Rows
                    If Not row.IsNewRow AndAlso row.Cells("ID_BARANG").Value IsNot Nothing Then
                        HitungByKode(row.Cells(0).Value)
                    End If
                Next
            Catch ex As Exception
                ' Rollback transaksi jika terjadi kesalahan
                transaction.Rollback()
                MessageBox.Show("Oh tidak! Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                                 "Detail kesalahan: " & ex.Message,
                  "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try
        End If
    End Sub

    Private Sub BtnPrint_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPrint.Click
        If TxtFakturTransaksi.Text <> "" Then
            ' Panggil metode Cetaktransaksi
            Cetaktransaksi()
        Else
            ' Tampilkan pesan jika TxtFakturTransaksi kosong
            MessageBox.Show("Tidak ada transaksi yang dipilih untuk dicetak.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Cetaktransaksi()
        Select Case TxtTransaksi.Text
            Case "Pembelian"
                NotaPembelian.TxtIdPembelian.Text = TxtFakturTransaksi.Text
                NotaPembelian.ShowDialog()

            Case "Penjualan"
                'If TxtJenisPrinter.Text = "Printer Thermal" Then
                With PrintJual
                    .TxtFaktur.Text = TxtFakturTransaksi.Text
                    .ProsesCetak()
                End With
                'Else

                'ModuleCetakJual.PrintReport(TxtFakturTransaksi.Text)
                'End If
            Case "Retur Pembelian"


            Case "Retur Penjualan"
                If TxtJenisPrinter.Text = "Printer Thermal" Then
                    With PrintReturJual
                        .TxtFaktur.Text = TxtFakturTransaksi.Text
                    End With
                Else
                End If

            Case "Bayar Hutang"


            Case "Bayar Piutang"

            Case "Stok Opname"
                FormLapTransferStok.ShowDialog()
            Case "Transfer Stok"

            Case "Surat Jalan"
                With PrinterSuratJalan
                    .TxtNota.Text = TxtFakturTransaksi.Text
                    .ProsesCetak()
                End With

            Case "Transfer Barang"
                With PrintTransferBarang
                    .TxtNota.Text = TxtFakturTransaksi.Text
                    .ProsesCetak()
                End With
        End Select
    End Sub

    Private Sub DGVTransaksi_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVTransaksi.CellClick
        If DGVTransaksi.Rows.Count < 1 Then
            MessageBox.Show("Tidak ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else

            Select Case TxtTransaksi.Text
                Case "Pembelian"
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI, HARGA_AVERAGE, HARGA_BELI_SEBELUMNYA, QTY, SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL FROM pembelian_detail WHERE FAKTUR_BELI = ?"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@FAKTUR_BELI", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "pembelian_detail")

                                DGVDetail.DataSource = ds.Tables("pembelian_detail")

                                With DGVDetail
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
                                    .Columns("HARGA_BELI").HeaderText = "HARGA"
                                    .Columns("HARGA_AVERAGE").Visible = False
                                    .Columns("HARGA_BELI_SEBELUMNYA").Visible = False
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("HARGA_BELI_SATUAN").Visible = False
                                    .Columns("QTY_SAT").Visible = False
                                    .Columns("TOTAL").HeaderText = "TOTAL"

                                    .Columns("HARGA_BELI").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("HARGA_BELI").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("HARGA_BELI_SATUAN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("HARGA_BELI_SATUAN").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("TOTAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("TOTAL").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("HARGA_BELI").FillWeight = 60
                                    .Columns("QTY").FillWeight = 30
                                    .Columns("SATUAN").FillWeight = 50
                                    .Columns("HARGA_BELI_SATUAN").FillWeight = 70
                                    .Columns("TOTAL").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using

                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Belanja : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Penjualan"
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, QTY_SATUAN, TOTAL_DISKON, TOTAL_HARGA FROM penjualan_detail WHERE FAKTUR_JUAL = ?"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@FAKTUR_JUAL", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "penjualan_detail")
                                DGVDetail.DataSource = ds.Tables("penjualan_detail")
                                With DGVDetail
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("HARGA_JUAL").HeaderText = "HARGA"
                                    .Columns("QTY_SATUAN").Visible = False
                                    .Columns("TOTAL_DISKON").HeaderText = "DISKON"
                                    .Columns("TOTAL_HARGA").HeaderText = "TOTAL"

                                    .Columns("QTY").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("QTY").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("HARGA_JUAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("HARGA_JUAL").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("TOTAL_DISKON").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("TOTAL_DISKON").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("TOTAL_HARGA").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("TOTAL_HARGA").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("QTY").FillWeight = 30
                                    .Columns("SATUAN").FillWeight = 50
                                    .Columns("HARGA_JUAL").FillWeight = 60
                                    .Columns("TOTAL_DISKON").FillWeight = 60
                                    .Columns("TOTAL_HARGA").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Penjualan : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()

                Case "Retur Pembelian"

                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, QTY_SAT, TOTAL FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = ?"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "penjualan_detail")
                                DGVDetail.DataSource = ds.Tables("penjualan_detail")
                                With DGVDetail
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("TOTAL").HeaderText = "TOTAL"

                                    .Columns("QTY").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("QTY").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("QTY_SAT").Visible = False

                                    .Columns("TOTAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("TOTAL").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("QTY").FillWeight = 30
                                    .Columns("SATUAN").FillWeight = 50
                                    .Columns("TOTAL").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(4).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Retur Pembelian : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Retur Penjualan"

                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, QTY_SATUAN, TOTAL_DISKON, TOTAL_HARGA FROM retur_penjualan_detail WHERE ID_RETUR_PENJUALAN = ?"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@ID_RETUR_PENJUALAN", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "retur_penjualan_detail")
                                DGVDetail.DataSource = ds.Tables("retur_penjualan_detail")
                                With DGVDetail
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "NAMA BARANG"
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("HARGA_JUAL").HeaderText = "HARGA"
                                    .Columns("QTY_SATUAN").Visible = False
                                    .Columns("TOTAL_DISKON").HeaderText = "DISKON"
                                    .Columns("TOTAL_HARGA").HeaderText = "TOTAL"

                                    .Columns("QTY").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("QTY").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("HARGA_JUAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("HARGA_JUAL").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("TOTAL_DISKON").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("TOTAL_DISKON").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("TOTAL_HARGA").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("TOTAL_HARGA").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("QTY").FillWeight = 30
                                    .Columns("SATUAN").FillWeight = 50
                                    .Columns("HARGA_JUAL").FillWeight = 60
                                    .Columns("TOTAL_DISKON").FillWeight = 60
                                    .Columns("TOTAL_HARGA").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(4).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Retur Penjualan : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Bayar Hutang"

                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()

                    Dim sqlSelect As String = "SELECT ID_BELI, KODE, NAMA, TOTAL_HUTANG, DIBAYAR, TANGGAL_BAYAR, PEMBAYARAN, STATUS FROM Hutang_Detail WHERE ID_BAYAR = @ID_BAYAR"

                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        cmdSelect.Parameters.AddWithValue("@ID_BAYAR", DGVTransaksi.CurrentRow.Cells(0).Value)

                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                da.Fill(ds, "HutangDetail")
                                DGVDetail.DataSource = ds.Tables("HutangDetail")

                                With DGVDetail
                                    .Columns("ID_BELI").HeaderText = "NOTA BELI"
                                    .Columns("KODE").Visible = False
                                    .Columns("NAMA").HeaderText = "SUPLIYER"
                                    .Columns("DIBAYAR").Visible = False
                                    .Columns("TOTAL_HUTANG").Visible = False
                                    .Columns("TANGGAL_BAYAR").HeaderText = "TANGGAL"
                                    .Columns("PEMBAYARAN").HeaderText = "NOMINAL"
                                    .Columns("STATUS").HeaderText = "STATUS"

                                    .Columns("PEMBAYARAN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("PEMBAYARAN").DefaultCellStyle.Format = "#,0.##"

                                    .Columns("NAMA").FillWeight = 150
                                End With
                            End Using
                        End Using
                    End Using

                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Bayar Hutang : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()

                Case "Bayar Piutang"

                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    Dim sqlSelect As String = "SELECT ID_JUAL, KODE, NAMA, DIBAYAR, PIUTANG, TANGGAL_BAYAR, PEMBAYARAN, STATUS FROM Piutang_Detail WHERE ID_BAYAR = @ID_BAYAR"
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        ' Set parameter ID_BAYAR dari baris yang dipilih di DGVTransaksi
                        cmdSelect.Parameters.AddWithValue("@ID_BAYAR", DGVTransaksi.CurrentRow.Cells(0).Value)

                        ' Menggunakan data adapter untuk mengambil data dari database
                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                ' Mengisi dataset dengan data dari database
                                da.Fill(ds, "penjualan_piutang")
                                ' Menetapkan sumber data untuk DataGridView DGVDetail
                                DGVDetail.DataSource = ds.Tables("penjualan_piutang")

                                ' Konfigurasi tampilan kolom pada DataGridView
                                With DGVDetail
                                    .Columns("ID_JUAL").HeaderText = "NOTA JUAL"
                                    .Columns("KODE").Visible = False ' Kolom KODE mungkin tidak diperlukan
                                    .Columns("NAMA").HeaderText = "PELANGGAN"
                                    .Columns("DIBAYAR").Visible = False ' Kolom DIBAYAR mungkin tidak diperlukan
                                    .Columns("PIUTANG").Visible = False ' Kolom PIUTANG mungkin tidak diperlukan
                                    .Columns("TANGGAL_BAYAR").HeaderText = "TANGGAL"
                                    .Columns("PEMBAYARAN").HeaderText = "NOMINAL"
                                    .Columns("STATUS").HeaderText = "STATUS"

                                    ' Format dan pengaturan alignment untuk kolom NOMINAL
                                    .Columns("PEMBAYARAN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("PEMBAYARAN").DefaultCellStyle.Format = "#,0.##"

                                    ' Mengatur ukuran kolom NAMA_PELANGGAN
                                    .Columns("NAMA").FillWeight = 150
                                End With
                            End Using
                        End Using
                    End Using

                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(2).Value.ToString()
                    LblDetailTransaksi.Text = "Detail Bayar Piutang : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Stok Opname"
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(1).Value.ToString()

                Case "Transfer Stok"
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(1).Value.ToString()


                Case "Surat Jalan"
                    ' Clear existing data
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()

                    ' SQL query to select the necessary fields
                    Dim sqlSelect As String = "SELECT NOTA_BELANJA, NAMA_PELANGGAN, NILAI_BELANJA, LOKASI FROM Surat_Jalan_Detail WHERE NOTA = ?"

                    ' Prepare the command
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        ' Add the parameter
                        cmdSelect.Parameters.AddWithValue("@NOTA", DGVTransaksi.CurrentRow.Cells(0).Value)

                        ' Use a DataAdapter to fill a DataSet
                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                ' Fill the DataSet
                                da.Fill(ds, "Surat_Jalan_Detail")

                                ' Set the DataSource of the DataGridView
                                DGVDetail.DataSource = ds.Tables("Surat_Jalan_Detail")

                                ' Configure the DataGridView columns
                                With DGVDetail
                                    ' Ensure column names match those in the dataset
                                    .Columns("NOTA_BELANJA").Visible = False
                                    .Columns("NAMA_PELANGGAN").HeaderText = "NAMA PELANGGAN"
                                    .Columns("NILAI_BELANJA").HeaderText = "NILAI BELANJA"
                                    .Columns("LOKASI").HeaderText = "LOKASI"

                                    ' Align and format columns
                                    .Columns("NILAI_BELANJA").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("NILAI_BELANJA").DefaultCellStyle.Format = "#,0.##"

                                    ' Set column widths
                                    .Columns("NAMA_PELANGGAN").FillWeight = 150
                                    .Columns("NILAI_BELANJA").FillWeight = 60
                                    .Columns("LOKASI").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using

                    ' Update related controls
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Clear()
                    LblDetailTransaksi.Text = "Detail Surat Jalan : " + DGVTransaksi.CurrentRow.Cells(0).Value.ToString()


                Case "Transfer Barang"
                    ' Bersihkan data yang ada
                    DGVDetail.DataSource = Nothing
                    DGVDetail.Rows.Clear()

                    ' Query SQL untuk memilih kolom yang diperlukan
                    Dim sqlSelect As String = "SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, TOTAL_QTY, TOTAL FROM Transfer_Barang_Detail WHERE ID_TRANSFER = ?"

                    ' Persiapkan perintah
                    Using cmdSelect As New MySqlCommand(sqlSelect, conn)
                        ' Tambahkan parameter
                        cmdSelect.Parameters.AddWithValue("@ID_TRANSFER", DGVTransaksi.CurrentRow.Cells(0).Value)

                        ' Gunakan DataAdapter untuk mengisi DataSet
                        Using da As New MySqlDataAdapter(cmdSelect)
                            Using ds As New DataSet()
                                ' Isi DataSet
                                da.Fill(ds, "Transfer_Barang_Detail")

                                ' Atur DataSource dari DataGridView
                                DGVDetail.DataSource = ds.Tables("Transfer_Barang_Detail")

                                ' Konfigurasi kolom DataGridView
                                With DGVDetail
                                    ' Pastikan nama kolom cocok dengan dataset
                                    .Columns("ID_BARANG").Visible = False
                                    .Columns("NAMA_BARANG").HeaderText = "BARANG"
                                    .Columns("QTY").HeaderText = "QTY"
                                    .Columns("SATUAN").HeaderText = "SATUAN"
                                    .Columns("TOTAL_QTY").Visible = False
                                    .Columns("TOTAL").HeaderText = "TOTAL"

                                    ' Ratakan dan format kolom
                                    .Columns("TOTAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                                    .Columns("TOTAL").DefaultCellStyle.Format = "#,0.##"

                                    ' Set lebar kolom
                                    .Columns("NAMA_BARANG").FillWeight = 150
                                    .Columns("QTY").FillWeight = 40
                                    .Columns("SATUAN").FillWeight = 60
                                    .Columns("TOTAL").FillWeight = 60

                                End With
                            End Using
                        End Using
                    End Using

                    ' Perbarui kontrol terkait
                    TxtFakturTransaksi.Text = DGVTransaksi.CurrentRow.Cells(0).Value.ToString()
                    TxtLokasiUntukEdit.Text = DGVTransaksi.CurrentRow.Cells(1).Value.ToString()
                    LblDetailTransaksi.Text = "Detail transfer barang: " & DGVTransaksi.CurrentRow.Cells(0).Value.ToString()

            End Select

            With DGVDetail
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AllowUserToOrderColumns = False
                .AllowUserToResizeColumns = False
                .AllowUserToResizeRows = False


                .EnableHeadersVisualStyles = False
                .ColumnHeadersDefaultCellStyle.BackColor = Color.Gray
                ' Set alternating row style
                .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

                ' Set visual style
                .BorderStyle = BorderStyle.FixedSingle
                .GridColor = Color.Silver
                .BackgroundColor = Color.White

                ' Enable double buffering to reduce flickering
                DataGridViewExtension.EnableDoubleBuffering(DGVDetail)
            End With
        End If
    End Sub

    Private Sub BTNKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTNKeluar.Click
        GBTransaksi.Hide()
    End Sub

    Public Sub Refresdatagridview()
        Select Case TxtTransaksi.Text
            Case "Pembelian"
                Datapembelian()
            Case "Penjualan"
                Datapenjualan()
            Case "Retur Pembelian"
                DatareturPembelian()
            Case "Retur Penjualan"
                DataReturPenjualan()
            Case "Bayar Hutang"
                DataBayarHutang()
            Case "Bayar Piutang"
                DataBayarPiutang()
            Case "Stok Opname"
                DataStokOpname()
            Case "Transfer Stok"
                Datatransferstok()
            Case "Surat Jalan"
                DataSuratjalan()
            Case "Transfer Barang"
                DataTransferBarang()
        End Select
    End Sub

    '----------------------------------------- KARYAWAN ---------------------------------------------------------------------------
    Private Sub MasterGajiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MasterGajiToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormMasterGaji
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub



    Private Sub BonKaryawanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BonKaryawanToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormBon
            .LblUtama.Text = "BON KARYAWAN"
            .LblJenis.Text = "BON"
            '.LblDaftar.Text = "Daftar bon pada tanggal "
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub BayarBonDiluarGajiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BayarBonDiluarGajiToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormBon
            .LblUtama.Text = "BAYAR BON KARYAWAN DILUAR POTONGAN GAJI"
            .LblJenis.Text = "BAYAR"
            '.LblDaftar.Text = "Daftar pembayaran bon pada tanggal "
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub LaporanBonToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LaporanBonToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapBon
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub LaporanBonPerKaryawanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LaporanBonPerKaryawanToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapBonPerorang
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub GajiKaryawanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GajiKaryawanToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormGaji
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub


    Private Sub LaporanGajiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LaporanGajiToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLaporanGaji
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub
    '----------------------------------------- LAPORAN ---------------------------------------------------------------------------

    Private Sub MutasiSaldoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MutasiSaldoToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        Using cmdUpdateMasuk As New MySqlCommand("UPDATE JurnalUmum SET NOMOR_AKUN_D = @NOMOR_AKUN_D WHERE NAMA_AKUN_D = @NAMA_AKUN_D", conn)
            cmdUpdateMasuk.Parameters.AddWithValue("@NOMOR_AKUN_D", "01.01.001")
            cmdUpdateMasuk.Parameters.AddWithValue("@NAMA_AKUN_D", "KAS DI TOKO")
            cmdUpdateMasuk.ExecuteNonQuery()
        End Using


        With FormLapSaldo
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub MutasiBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MutasiBarangToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapMutasiBarang
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub JurnalUmumToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JurnalUmumToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        With FormLapJurnal
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub NeracaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NeracaToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        With FormLapNeracaLR
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub BukuBesarToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BukuBesarToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        With FormLapBB
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub


    Private Sub PembelianToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PembelianToolStripMenuItem1.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPembelian
            .LabelJudul.Text = "LAPORAN PEMBELIAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub PembelianDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PembelianDetailToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPembelian
            .LabelJudul.Text = "LAPORAN PEMBELIAN DETAIL"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub PembelianBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PembelianBarangToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPembelian
            .LabelJudul.Text = "LAPORAN BARANG PEMBELIAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub PembelianDihutangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PembelianDihutangToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPembelian
            .LabelJudul.Text = "LAPORAN PEMBELIAN DIHUTANG"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub RekapPenjualanByNotaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RekapPenjualanByNotaToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPenjualanBaru
            .LabelJudul.Text = "LAPORAN REKAP PENJUALAN NOTA"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub


    Private Sub RekapPenjualanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RekapPenjualanToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPenjualanBaru
            .LabelJudul.Text = "LAPORAN REKAP PENJUALAN BARANG"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub PenjualanToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanToolStripMenuItem1.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPenjualanBaru
            .LabelJudul.Text = "LAPORAN PENJUALAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub PenjualanDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanDetailToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPenjualanBaru
            .LabelJudul.Text = "LAPORAN PENJUALAN DETAIL"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub PenjualanBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanBarangToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPenjualanBaru
            .LabelJudul.Text = "LAPORAN BARANG PENJUALAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub PenjualanTerhutangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanTerhutangToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPenjualanBaru
            .LabelJudul.Text = "LAPORAN PENJUALAN DIHUTANG"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub PenjualanSalesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PenjualanSalesToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPenjualanSales
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub PenjualanPPNNonPPNToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PenjualanPPNNonPPNToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormPenjualanPPn
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub
    Private Sub ReturPembelianToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPembelianToolStripMenuItem1.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapReturBeli
            .LabelJudul.Text = "LAPORAN RETUR PEMBELIAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ReturPembelianDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPembelianDetailToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapReturBeli
            .LabelJudul.Text = "LAPORAN RETUR PEMBELIAN DETAIL"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ReturPembelianBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPembelianBarangToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapReturBeli
            .LabelJudul.Text = "LAPORAN BARANG RETUR PEMBELIAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub


    Private Sub ReturPenjualanToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPenjualanToolStripMenuItem1.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapReturJual
            .LabelJudul.Text = "LAPORAN RETUR PENJUALAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ReturPenjualanDetailToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPenjualanDetailToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapReturJual
            .LabelJudul.Text = "LAPORAN RETUR PENJUALAN DETAIL"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ReturPenjualanBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReturPenjualanBarangToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapReturJual
            .LabelJudul.Text = "LAPORAN BARANG RETUR PENJUALAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ByTanggalBelanjaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalBelanjaToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapHutang
            .LabelJudul.Text = "LAPORAN HUTANG KE SUPPLIER BY PEMBELIAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ByTanggalPelunasanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalPelunasanToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapHutang
            .LabelJudul.Text = "LAPORAN HUTANG KE SUPPLIER BY PELUNASAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ByTanggalJatuhTempoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalJatuhTempoToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapHutang
            .LabelJudul.Text = "LAPORAN HUTANG KE SUPPLIER BY JATUH TEMPO"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ByTanggalPenjualanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalPenjualanToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPiutang
            .LabelJudul.Text = "LAPORAN PIUTANG PELANGGAN BY PENJUALAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ByTanggalPelunasanToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalPelunasanToolStripMenuItem1.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPiutang
            .LabelJudul.Text = "LAPORAN PIUTANG PELANGGAN BY PELUNASAN"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub ByTanggalJatuhTempoToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByTanggalJatuhTempoToolStripMenuItem1.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapPiutang
            .LabelJudul.Text = "LAPORAN PIUTANG PELANGGAN BY JATUH TEMPO"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub


    Private Sub KasPenjualanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KasPenjualanToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        With FormLapkAS
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub TransferStokToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TransferStokToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        With FormLapTransferStok
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub TransferBarangToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles TransferBarangToolStripMenuItem1.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        With FormLapTransferBarang
            .LabelJudul.Text = "LAPORAN TRANSFER BARANG"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub TransferBarangDetailToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TransferBarangDetailToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        With FormLapTransferBarang
            .LabelJudul.Text = "LAPORAN TRANSFER BARANG DETAIL"
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub StokOpnameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StokOpnameToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        With NotaStokOpname
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub StokBarangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StokBarangToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormLapBarang
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub


    Private Sub GrafikToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles GrafikToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormGrafikLaba
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub HistoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HistoryToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        'With FormHistory
        '    .MdiParent = Me
        '    .BringToFront()
        '    .Dock = DockStyle.Fill
        '    .Show()
        'End With
    End Sub

    '----------------------------------------- UTILITY ---------------------------------------------------------------------------

    Private Sub SystemTutupBulanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SystemTutupBulanToolStripMenuItem.Click
        'With FormPenjualan1
        '    .MdiParent = Me
        '    .BringToFront()
        '    .Dock = DockStyle.Fill
        '    .Show()
        'End With
    End Sub

    Private Sub PilihanSaatMasukAplikasiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PilihanSaatMasukAplikasiToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormPilihanMasuk
            '.MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .ShowDialog()
        End With
    End Sub

    Private Sub DatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DatabaseToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With SettingDatabase
            '.MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .ShowDialog()
        End With
    End Sub

    Private Sub FormatSqlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FormatSqlToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        Cursor = Cursors.WaitCursor
        Dim typesql As String = "SQL"
        BackupDatabase(typesql)
        Cursor = Cursors.Default
    End Sub

    Private Sub FormatZipToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FormatZipToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
        Cursor = Cursors.WaitCursor
        Dim typesql As String = "ZIP"
        BackupDatabase(typesql)
        Cursor = Cursors.Default
    End Sub

    Private Sub FormatSqlToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FormatSqlToolStripMenuItem1.Click
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
            openFileDialog.Title = "Pilih File Backup"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim backupFilePath As String = openFileDialog.FileName

                ' Deserialisasi konfigurasi dari file biner
                Using stream As New FileStream(configFilePath, FileMode.Open, FileAccess.Read)
                    Dim json As String = File.ReadAllText(configFilePath)
                    Dim konfigurasi As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)
                    konfigurasi.Password = DecryptPassword(konfigurasi.Password)

                    Cursor = Cursors.WaitCursor
                    ' Panggil metode restore
                    DatabaseRestore.RestoreDatabase(konfigurasi, backupFilePath)
                    Cursor = Cursors.Default
                End Using
            End If
        End Using
    End Sub

    Private Sub FormatZipToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FormatZipToolStripMenuItem1.Click

    End Sub

    Private Sub PerbaikiDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PerbaikiDatabaseToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormPerbaikanDatabase
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub QueryDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QueryDatabaseToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormQuery
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub SettingPrinterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingPrinterToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormDefauldPrinter
            .MdiParent = Me
            .BringToFront()
            .Dock = DockStyle.Fill
            .Show()
        End With
    End Sub

    Private Sub HapusTransaksiTokoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HapusTransaksiTokoToolStripMenuItem.Click
        ' Tampilkan peringatan pertama
        Dim result1 As DialogResult = MessageBox.Show("Anda akan menghapus semua data yang terkait dengan TOKO. Apakah Anda yakin ingin melanjutkan?",
                                                       "Peringatan: Operasi Berbahaya",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Warning)

        If result1 = DialogResult.No Then
            ' Jika pengguna memilih No, hentikan operasi
            MessageBox.Show("Operasi dibatalkan oleh pengguna.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Tampilkan peringatan kedua untuk memastikan
        Dim result2 As DialogResult = MessageBox.Show("PERINGATAN KEDUA! Ini adalah operasi yang sangat berbahaya. Anda akan menghapus semua data TOKO secara permanen. Apakah Anda benar-benar yakin?",
                                                       "Peringatan Keras: Operasi Berbahaya",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Warning)

        If result2 = DialogResult.No Then
            ' Jika pengguna memilih No pada peringatan kedua, hentikan operasi
            MessageBox.Show("Operasi dibatalkan oleh pengguna setelah peringatan kedua.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        Try
            AmbilNilaiBarang(transaction, "TOKO")
            ' Array yang berisi semua query delete
            Dim deleteQueries As String() = {
                "UPDATE tbl_barang SET AWAL_TOKO='0'",
                "DELETE FROM Bon_karyawan WHERE LOKASI = 'TOKO'",
                "DELETE FROM Gaji_karyawan WHERE LOKASI = 'TOKO'",
                "DELETE FROM HistoryBarang WHERE LOKASI = 'TOKO'",
                "DELETE FROM hutang WHERE LOKASI = 'TOKO'",
                "DELETE FROM Hutang_Detail WHERE LOKASI = 'TOKO'",
                "DELETE FROM JurnalUmum WHERE LOKASI = 'TOKO'",
                "DELETE FROM pembelian WHERE LOKASI = 'TOKO'",
                "DELETE FROM pembelian_detail WHERE LOKASI = 'TOKO'",
                "DELETE FROM pembelian_ditahan WHERE LOKASI = 'TOKO'",
                "DELETE FROM pembelian_ditahan_detail WHERE LOKASI = 'TOKO'",
                "DELETE FROM penjualan WHERE LOKASIBARANG = 'TOKO'",
                "DELETE FROM penjualan_detail WHERE LOKASIBARANG = 'TOKO'",
                "DELETE FROM Piutang WHERE LOKASI = 'TOKO'",
                "DELETE FROM Piutang_Detail WHERE LOKASI = 'TOKO'",
                "DELETE FROM retur_pembelian WHERE PENYIMPANAN = 'TOKO'",
                "DELETE FROM retur_pembelian_detail WHERE PENYIMPANAN = 'TOKO'",
                "DELETE FROM retur_penjualan WHERE PENYIMPANAN = 'TOKO'",
                "DELETE FROM retur_penjualan_detail WHERE LOKASI = 'TOKO'",
                "DELETE FROM Stok_Opname WHERE LOKASI = 'TOKO'",
                "DELETE FROM StokTambahKurang WHERE LOKASI = 'TOKO'",
                "DELETE FROM Surat_Jalan WHERE LOKASI = 'TOKO'",
                "DELETE FROM Surat_Jalan_Detail WHERE LOKASI = 'TOKO'",
                "DELETE FROM Transfer_Barang WHERE LOKASI = 'TOKO'",
                "DELETE FROM Transfer_Barang_Detail WHERE LOKASI = 'TOKO'",
                "DELETE FROM Transfer_stok WHERE JENIS_TRANSFER = 'TOKO'"
            }

            ' Jalankan setiap query delete dalam loop
            For Each query As String In deleteQueries
                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.ExecuteNonQuery()
                End Using
            Next

            ' Query khusus untuk menghapus data di tabel History dengan wildcard yang dipisahkan
            Dim aksiParam As String = "TOKO" & "%" ' Menambahkan wildcard % ke variabel
            Using cmd As New MySqlCommand("DELETE FROM History WHERE Aksi LIKE ?", conn, transaction)
                cmd.Parameters.AddWithValue("@Aksi", aksiParam)
                cmd.ExecuteNonQuery()
            End Using

            'JurnalEksekusiTransaksi(transaction, "TOKO")

            ' Commit transaksi jika semua perintah berhasil
            transaction.Commit()

            MessageBox.Show("Semua transaksi TOKO berhasil di hapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Catat aksi di history
            DatabaseModule.CatatanAksiHistory("Berhasil menghapus transaksi toko")
        Catch ex As Exception
            ' Rollback transaksi jika terjadi kesalahan
            transaction.Rollback()
            MessageBox.Show("Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                            "Detail kesalahan: " & ex.Message,
                            "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub HapusTransaksiGudangToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HapusTransaksiGudangToolStripMenuItem.Click
        ' Tampilkan peringatan pertama
        Dim result1 As DialogResult = MessageBox.Show("Anda akan menghapus semua data yang terkait dengan TOKO. Apakah Anda yakin ingin melanjutkan?",
                                                       "Peringatan: Operasi Berbahaya",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Warning)

        If result1 = DialogResult.No Then
            ' Jika pengguna memilih No, hentikan operasi
            MessageBox.Show("Operasi dibatalkan oleh pengguna.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Tampilkan peringatan kedua untuk memastikan
        Dim result2 As DialogResult = MessageBox.Show("PERINGATAN KEDUA! Ini adalah operasi yang sangat berbahaya. Anda akan menghapus semua data TOKO secara permanen. Apakah Anda benar-benar yakin?",
                                                       "Peringatan Keras: Operasi Berbahaya",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Warning)

        If result2 = DialogResult.No Then
            ' Jika pengguna memilih No pada peringatan kedua, hentikan operasi
            MessageBox.Show("Operasi dibatalkan oleh pengguna setelah peringatan kedua.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = conn.BeginTransaction()

        Try
            AmbilNilaiBarang(transaction, "GUDANG")
            ' Array yang berisi semua query delete
            Dim deleteQueries As String() = {
                "UPDATE tbl_barang SET AWAL_GUDANG='0'",
                "DELETE FROM Bon_karyawan WHERE LOKASI = 'GUDANG'",
                "DELETE FROM Gaji_karyawan WHERE LOKASI = 'GUDANG'",
                "DELETE FROM HistoryBarang WHERE LOKASI = 'GUDANG'",
                "DELETE FROM hutang WHERE LOKASI = 'GUDANG'",
                "DELETE FROM Hutang_Detail WHERE LOKASI = 'GUDANG'",
                "DELETE FROM JurnalUmum WHERE LOKASI = 'GUDANG'",
                "DELETE FROM pembelian WHERE LOKASI = 'GUDANG'",
                "DELETE FROM pembelian_detail WHERE LOKASI = 'GUDANG'",
                "DELETE FROM pembelian_ditahan WHERE LOKASI = 'GUDANG'",
                "DELETE FROM pembelian_ditahan_detail WHERE LOKASI = 'GUDANG'",
                "DELETE FROM penjualan WHERE LOKASIBARANG = 'GUDANG'",
                "DELETE FROM penjualan_detail WHERE LOKASIBARANG = 'GUDANG'",
                "DELETE FROM Piutang WHERE LOKASI = 'GUDANG'",
                "DELETE FROM Piutang_Detail WHERE LOKASI = 'GUDANG'",
                "DELETE FROM retur_pembelian WHERE PENYIMPANAN = 'GUDANG'",
                "DELETE FROM retur_pembelian_detail WHERE PENYIMPANAN = 'GUDANG'",
                "DELETE FROM retur_penjualan WHERE PENYIMPANAN = 'GUDANG'",
                "DELETE FROM retur_penjualan_detail WHERE LOKASI = 'GUDANG'",
                "DELETE FROM Stok_Opname WHERE LOKASI = 'GUDANG'",
                "DELETE FROM StokTambahKurang WHERE LOKASI = 'GUDANG'",
                "DELETE FROM Surat_Jalan WHERE LOKASI = 'GUDANG'",
                "DELETE FROM Surat_Jalan_Detail WHERE LOKASI = 'GUDANG'",
                "DELETE FROM Transfer_Barang WHERE LOKASI = 'GUDANG'",
                "DELETE FROM Transfer_Barang_Detail WHERE LOKASI = 'GUDANG'",
                "DELETE FROM Transfer_stok WHERE JENIS_TRANSFER = 'GUDANG'"
            }

            ' Jalankan setiap query delete dalam loop
            For Each query As String In deleteQueries
                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.ExecuteNonQuery()
                End Using
            Next

            ' Query khusus untuk menghapus data di tabel History dengan wildcard yang dipisahkan
            Dim aksiParam As String = "GUDANG" & "%" ' Menambahkan wildcard % ke variabel
            Using cmd As New MySqlCommand("DELETE FROM History WHERE Aksi LIKE ?", conn, transaction)
                cmd.Parameters.AddWithValue("@Aksi", aksiParam)
                cmd.ExecuteNonQuery()
            End Using

            'JurnalEksekusiTransaksi(transaction, "GUDANG")

            ' Commit transaksi jika semua perintah berhasil
            transaction.Commit()

            MessageBox.Show("Semua transaksi GUDANG berhasil di hapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Catat aksi di history
            DatabaseModule.CatatanAksiHistory("Berhasil menghapus transaksi GUDANG")
        Catch ex As Exception
            ' Rollback transaksi jika terjadi kesalahan
            transaction.Rollback()
            MessageBox.Show("Transaksi dibatalkan karena terjadi kesalahan." & vbCrLf &
                            "Detail kesalahan: " & ex.Message,
                            "Oops! Ada masalah...", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PeriksaUpdateAplikasiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PeriksaUpdateAplikasiToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With FormCekUpdate
            .BringToFront()
            .Dock = DockStyle.Fill
            .ShowDialog()
        End With
    End Sub

    Private Sub CekIpKomputerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CekIpKomputerToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With Formipkomputer
            .BringToFront()
            .Dock = DockStyle.Fill
            .ShowDialog()
        End With
    End Sub


    Public Sub BackupDatabase(ByVal typedata As String)

        ' Deserialisasi konfigurasi dari file biner
        Using stream As New FileStream(configFilePath, FileMode.Open, FileAccess.Read)
            Dim json As String = File.ReadAllText(configFilePath)
            Dim konfigurasi As DatabaseConfiguration = JsonSerializer.Deserialize(Of DatabaseConfiguration)(json)
            konfigurasi.Password = DecryptPassword(konfigurasi.Password)

            ' Ganti informasi koneksi berikut sesuai dengan konfigurasi Anda

            ' Ambil nilai dari TextBox di form SettingDatabase
            Dim config As New DatabaseConfiguration() With {
            .Server = konfigurasi.Server,
            .Port = konfigurasi.Port,
            .User = konfigurasi.User,
            .Password = konfigurasi.Password,
            .Database = konfigurasi.Database
        }

            MySqlBackup.BackupDatabase(config, typedata)
        End Using
    End Sub



    ' Deklarasi variabel untuk menyimpan hasil query
    Dim record As String = ""
    Dim Qty As Decimal = 0
    Dim Rupiah As Decimal = 0
    Private Sub AmbilNilaiBarang(ByVal transaction As MySqlTransaction, ByVal LokasiQuery As String)
        Dim querySelect As String = "SELECT FAKTUR as record, Sum(QTY) as QTY, sum(TOTAL_RUPIAH) as Rupiah " &
                             "FROM HistoryBarang " &
                             "WHERE LOKASI Like @Lokasi"
        ' Eksekusi query SELECT
        Using cmdSelect As New MySqlCommand(querySelect, conn, transaction)
            cmdSelect.Parameters.AddWithValue("@Lokasi", LokasiQuery.ToUpper()) ' Mengisi parameter lokasi dengan huruf kapital
            Using reader As MySqlDataReader = cmdSelect.ExecuteReader()
                If reader.Read() Then
                    ' Mengambil data dari hasil query
                    record = If(reader("record") IsNot DBNull.Value, reader("record").ToString(), String.Empty)
                    Qty = If(reader("QTY") IsNot DBNull.Value, Convert.ToDecimal(reader("QTY")), 0D)
                    Rupiah = If(reader("Rupiah") IsNot DBNull.Value, Convert.ToDecimal(reader("Rupiah")), 0D)
                End If
            End Using
        End Using
    End Sub

    Private Sub JurnalEksekusiTransaksi(ByVal transaction As MySqlTransaction, ByVal LokasiQuery As String)
        ' Membuat nomor transaksi unik berdasarkan tanggal dan waktu saat ini
        Dim noTransaksi As String = DateTime.Now.ToString("yyyyMMddHHmmss")

        ' Eksekusi INSERT ke dalam JurnalUmum
        Using cmdInsert As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                             "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            ' Tambahkan parameter untuk query INSERT
            cmdInsert.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
            cmdInsert.Parameters.AddWithValue("@TGL_TRANSAKSI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdInsert.Parameters.AddWithValue("@URAIAN", "Hapus Transaksi " & LokasiQuery & " barang : " & record & " Qty : " & Qty.ToString())
            cmdInsert.Parameters.AddWithValue("@NAMA_AKUN_D", LAWAN_NAMA_REK_BARANG)
            cmdInsert.Parameters.AddWithValue("@NOMOR_AKUN_D", LAWAN_KODE_REK_BARANG)
            cmdInsert.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
            cmdInsert.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
            cmdInsert.Parameters.AddWithValue("@NOMINAL", Rupiah)
            cmdInsert.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Hapus Barang")
            cmdInsert.Parameters.AddWithValue("@LOKASI", "")
            cmdInsert.Parameters.AddWithValue("@ID_USER", SLogin.Text)
            cmdInsert.Parameters.AddWithValue("@ID_KOMPUTER", Comp.Text)

            ' Eksekusi perintah INSERT
            cmdInsert.ExecuteNonQuery()
        End Using
    End Sub




    '----------------------------------------- WINDOWS ---------------------------------------------------------------------------

    Private Sub CascadeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CascadeToolStripMenuItem.Click
        LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub TitleHorizontalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TitleHorizontalToolStripMenuItem.Click
        LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    Private Sub TitelVerticalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TitelVerticalToolStripMenuItem.Click
        LayoutMdi(MdiLayout.TileVertical)
    End Sub

    Private Sub ArrangeIconsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ArrangeIconsToolStripMenuItem.Click
        LayoutMdi(MdiLayout.ArrangeIcons)
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CloseAllToolStripMenuItem.Click
        GBTransaksi.Visible = False
        DGVTransaksi.Columns.Clear()
        For Each frm As Form In MdiChildren
            frm.Close()
        Next
    End Sub

    '----------------------------------------- KELUAR ---------------------------------------------------------------------------

    Private Sub KeluarToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles KeluarToolStripMenuItem.Click
        DatabaseModule.CatatanAksiHistory("Keluar aplikasi")
        Close()
    End Sub


    Private Sub FormUtama_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        ' Tanyakan apakah pengguna ingin melakukan backup sebelum keluar
        If MessageBox.Show("BACKUP DATA ?", "Konfirmasi Backup", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            ' Panggil metode BackupDatabase jika pengguna memilih "Yes"
            Me.Cursor = Cursors.WaitCursor
            Dim typesql As String = "SQL"
            BackupDatabase(typesql)
            Me.Cursor = Cursors.Default
        End If

        ' Tanyakan apakah pengguna ingin keluar dari aplikasi
        If MessageBox.Show("Apakah Anda yakin ingin keluar dari aplikasi ini?", "Informasi Keluar", MessageBoxButtons.YesNo) = DialogResult.No Then
            ' Batalkan penutupan form
            e.Cancel = True
        Else
            ' CATATAN HISTORY jika pengguna memilih "Yes"
            DatabaseModule.CatatanAksiHistory("Keluar aplikasi")
        End If
    End Sub


    Private Sub BtnNotif_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnNotif.Click
        ' Buat instance dari form NotifikasiJatuhTempo
        Dim notifForm As New NotifikasiJatuhTempo()

        ' Dapatkan posisi layar dari BtnNotif
        Dim btnPosition As Point = BtnNotif.PointToScreen(New Point(BtnNotif.Width, BtnNotif.Height))

        ' Setel posisi form NotifikasiJatuhTempo di samping pojok kanan atas BtnNotif
        notifForm.StartPosition = FormStartPosition.Manual

        ' Sesuaikan posisi form agar rata kanan dengan BtnNotif
        Dim notifFormX As Integer = btnPosition.X - notifForm.Width
        Dim notifFormY As Integer = btnPosition.Y

        ' Setel posisi form
        notifForm.Location = New Point(notifFormX, notifFormY)

        ' Tampilkan form NotifikasiJatuhTempo
        notifForm.Show()
    End Sub

    Private Sub DGVTransaksi_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DGVTransaksi.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DGVTransaksi.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    Private Sub DGVDetail_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DGVDetail.RowPostPaint
        ' Menggambar nomor urut pada row header
        Using b As New SolidBrush(DGVDetail.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    Private Sub JualToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JualToolStripMenuItem.Click
        For Each frm As Form In MdiChildren
            frm.Close()
        Next

        With XtraFormJual
            .BringToFront()
            .Dock = DockStyle.Fill
            .ShowDialog()
        End With
    End Sub


End Class