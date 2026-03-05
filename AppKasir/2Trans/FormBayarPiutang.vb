Imports System.Globalization


Public Class FormBayarPiutang
    Private TransaksiLampau As String
    Private Sub FormBayarPiutang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TransaksiLampau = ModulHakAkses.BacaHakAksesSemua(FormGeneralSetting.LblTransaksiTanggalLampau.Text)
        Kondisiawal()
    End Sub

    Private Sub GenerateNomorBayarHutang()
        Dim cekTanggal As String = DtpTanggal.Value.ToString("yyMMdd")
        Dim UrutKOde As String = ""
        Dim cekNomor As String = "BP-" & cekTanggal

        ' Query untuk mendapatkan nomor maksimum berdasarkan format
        Using cmd As New MySqlCommand("SELECT MAX(ID_BAYAR_PIUTANG) FROM Piutang WHERE ID_BAYAR_PIUTANG LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")

            ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
            Dim maxKode As Object = cmd.ExecuteScalar()

            If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                Dim MaxNilaiKode As String = maxKode.ToString()
                If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "BP-" & cekTanggal Then
                    ' Hitung nomor berikutnya
                    Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                    UrutKOde = "BP-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                End If
            End If
        End Using

        ' Jika UrutKOde masih kosong, buat nomor pertama
        If String.IsNullOrEmpty(UrutKOde) Then
            UrutKOde = "BP-" & cekTanggal & "0001"
        End If

        ' Tampilkan nomor pada label
        LblNomorBayar.Text = UrutKOde

    End Sub

    Private Sub Kondisiawal()
        PanelView.Visible = False
        TxtTotalPiutang.Text = 0
        TxtTotalBayar.Text = 0
        TxtSisaPiutang.Text = 0
        LblTotalPiutang.Text = "0"
        LblTotalBayar.Text = "0"
        LblSisaPiutang.Text = "0"
        CmbPelanggan.Text = ""
        LblKodePelanggan.Text = ""
        DgvData.Rows.Clear()

        CmbRekening.Items.Clear()
        ' Isi ComboBox dengan data dari list
        CmbRekening.Items.AddRange(GetDaftarAkun().ToArray())

        GenerateNomorBayarHutang()
        SelectNamaPelanggan()

    End Sub

    Public Sub TampildataPiutang()
        Using cmd As New MySqlCommand("SELECT ID_PENJUALAN, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, TGL_TRANSAKSI, GRAND_TOTAL_STL_PAJAK, BAYAR, NILAI_RETUR, SISA_TAGIHAN, JATUH_TEMPO FROM penjualan WHERE ID_PELANGGAN = @ID_PELANGGAN AND STATUS_TRANSAKSI = 'Belum Lunas' ORDER BY ID_PENJUALAN", conn)
            cmd.Parameters.AddWithValue("@ID_PELANGGAN", LblKodePelanggan.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvData.Rows.Clear()
                While rd.Read()
                    ' Add a DataGridViewCheckBoxCell in the first column
                    DgvData.Rows.Add(False, rd(0), rd(1), rd(2), rd(3), rd(4), rd(5), rd(6), rd(7), rd(8), rd(9))
                End While
            End Using
        End Using

        With DgvData
            .Columns(5).DefaultCellStyle.Format = "dd/MM/yyyy"
            .Columns(6).DefaultCellStyle.Format = "#,0.##"
            .Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(7).DefaultCellStyle.Format = "#,0.##"
            .Columns(7).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(8).DefaultCellStyle.Format = "#,0.##"
            .Columns(8).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(9).DefaultCellStyle.Format = "#,0.##"
            .Columns(9).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns(10).DefaultCellStyle.Format = "dd/MM/yyyy"
            .Columns(11).DefaultCellStyle.Format = "#,0.##"
            .Columns(11).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End With

        ' Misalkan DgvData memiliki 10 kolom
        DgvData.Columns(0).ReadOnly = False ' Memungkinkan pengeditan pada Kolom 0

        For i As Integer = 1 To 11
            DgvData.Columns(i).ReadOnly = True ' Kolom 1 sampai 9 hanya-baca
        Next i

        DgvData.ClearSelection()


        Totalhutang()
    End Sub

    Private Sub DgvData_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellContentClick
        ' Pastikan kolom yang di klik adalah kolom "View"
        If e.RowIndex >= 0 AndAlso DgvData.Columns(e.ColumnIndex).Name = "View" AndAlso Not IsDBNull(DgvData.Rows(e.RowIndex).Cells("IDPEMBELIAN").Value) Then
            ' Ambil nilai dari kolom "kode" pada baris yang dipilih
            Dim fakturBeli As String = DgvData.Rows(e.RowIndex).Cells("IDPEMBELIAN").Value.ToString()
            LblDetail.Text = "Detail penjualan dengan nota = " & fakturBeli

            Dim dt As New DataTable()

            Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, TOTAL_HARGA, ID_USER FROM penjualan_detail WHERE FAKTUR_JUAL LIKE @FAKTUR_JUAL", conn)
                cmd.Parameters.AddWithValue("@FAKTUR_JUAL", fakturBeli)
                Using rd As New MySqlDataAdapter(cmd)
                    rd.Fill(dt)
                End Using
            End Using

            DgvDetail.DataSource = dt

            Dim columnsToFormat As String() = {"QTY", "TOTAL_HARGA"}
            Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
                {"ID_BARANG", "Kode"},
                {"NAMA_BARANG", "Nama Barang"},
                {"QTY", "Qty"},
                {"SATUAN", "Satuan"},
                {"TOTAL_HARGA", "Harga"},
                {"ID_USER", "User"}
            }

            With DgvDetail
                ' Loop through columns and set format and alignment
                For Each columnName As String In columnsToFormat
                    If .Columns.Contains(columnName) Then
                        ' Use custom format to display numbers with commas and up to two decimal places if not zero
                        .Columns(columnName).DefaultCellStyle.Format = "#,0.##"
                        .Columns(columnName).DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                        .Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    End If
                Next

                ' Rename columns
                For Each column As DataGridViewColumn In .Columns
                    If columnNames.ContainsKey(column.Name) Then
                        column.HeaderText = columnNames(column.Name)
                    End If
                Next

                ' Set header style
                .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

                ' Set alternating row style
                .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

                ' Set visual style
                .BorderStyle = BorderStyle.FixedSingle
                .GridColor = Color.Silver
                .BackgroundColor = Color.White

                ' Enable double buffering to reduce flickering
                EnableDoubleBuffering(DgvDetail)

                ' Set FillWeight for NAMA_BARANG column
                If .Columns.Contains("NAMA_BARANG") Then
                    .Columns("NAMA_BARANG").FillWeight = 200
                End If
            End With
            DgvDetail.ClearSelection()
            PanelView.Visible = True
        End If
    End Sub

    ' Method to enable double buffering
    Public Shared Sub EnableDoubleBuffering(ByVal dgv As DataGridView)
        dgv.GetType().InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, dgv, New Object() {True})
    End Sub

    Public Sub SelectNamaPelanggan()
        CmbPelanggan.Items.Clear()
        Using cmd As New MySqlCommand("SELECT DISTINCT NAMA_PELANGGAN FROM penjualan WHERE STATUS_TRANSAKSI = 'Belum Lunas'", conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    CmbPelanggan.Items.Add(reader("NAMA_PELANGGAN").ToString())
                End While
            End Using
        End Using
    End Sub

    Private Sub CmbPelanggan_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbPelanggan.SelectedIndexChanged
        Using cmd As New MySqlCommand("SELECT ID_PELANGGAN FROM penjualan WHERE NAMA_PELANGGAN = @NAMA_PELANGGAN", conn)
            cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbPelanggan.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader
                If rd.Read() Then
                    LblKodePelanggan.Text = rd.GetString(0)
                End If
            End Using
        End Using
        TampildataPiutang()
    End Sub

    Private Sub Totalhutang()
        ' Inisialisasi variabel untuk menyimpan total
        Dim TotalHUtang As Decimal = 0

        ' Loop melalui setiap baris pada DataGridView
        For Each row As DataGridViewRow In DgvData.Rows
            ' Pastikan baris bukan baris baru yang ditandai untuk penambahan
            If Not row.IsNewRow Then
                ' Dapatkan nilai dari kolom ke-9 dan tambahkan ke total
                Dim nilaiHutang As Decimal = If(IsDBNull(row.Cells(9).Value) OrElse row.Cells(9).Value Is Nothing, 0, Convert.ToDecimal(row.Cells(9).Value))
                TotalHUtang += nilaiHutang
            End If
        Next

        ' Tampilkan total pada TextBox
        TxtTotalPiutang.Text = TotalHUtang.ToString()
        LblTotalPiutang.Text = TotalHUtang.ToString("#,0.##")
    End Sub

    Private Sub DgvData_CurrentCellDirtyStateChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DgvData.CurrentCellDirtyStateChanged
        If DgvData.IsCurrentCellDirty Then
            DgvData.CommitEdit(DataGridViewDataErrorContexts.Commit)

            ' Cek apakah sel yang diubah adalah kolom cekbox
            If TypeOf DgvData.CurrentCell Is DataGridViewCheckBoxCell Then
                ' Ambil baris saat ini
                Dim currentRow As DataGridViewRow = DgvData.CurrentRow

                ' Periksa apakah ceklis diaktifkan
                Dim isChecked As Boolean = Convert.ToBoolean(currentRow.Cells("Check").Value)

                ' Jika dicentang, isi kolom "Bayar" dengan nilai dari kolom "NOMINALHUTANG"
                If isChecked Then
                    currentRow.Cells("Bayar").Value = currentRow.Cells("NOMINALHUTANG").Value
                    currentRow.Cells("Bayar").ReadOnly = False
                Else
                    ' Jika ceklis dihapus, kosongkan kolom "Bayar"
                    currentRow.Cells("Bayar").Value = 0
                    currentRow.Cells("Bayar").ReadOnly = True
                End If

                ' Terapkan format #,0.## untuk kolom "Bayar" (angka dengan dua desimal jika ada, tanpa trailing zero)
                currentRow.Cells("Bayar").Style.Format = "#,0.##"
                currentRow.Cells("Bayar").Style.Alignment = DataGridViewContentAlignment.MiddleRight
            End If

            ' Panggil metode untuk menghitung total pembayaran jika diperlukan
            HitungTotalBayar()
        End If
    End Sub

    ' Event untuk menangani perubahan format kolom setelah selesai diedit
    Private Sub DgvData_CellEndEdit(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DgvData.CellEndEdit
        ' Periksa apakah kolom yang diedit adalah kolom "Bayar"
        If DgvData.Columns(e.ColumnIndex).Name = "Bayar" Then
            ' Terapkan format N0 untuk kolom "Bayar"
            DgvData.Rows(e.RowIndex).Cells("Bayar").Style.Format = "#,0.##"
            DgvData.Rows(e.RowIndex).Cells("Bayar").Style.Alignment = DataGridViewContentAlignment.MiddleRight
        End If

        ' Panggil metode untuk menghitung total pembayaran jika diperlukan
        HitungTotalBayar()
    End Sub

    Private Sub HitungTotalBayar()
        ' Inisialisasi variabel untuk menyimpan total
        Dim totalBayar As Decimal = 0

        ' Loop melalui setiap baris pada DataGridView
        For Each row As DataGridViewRow In DgvData.Rows
            ' Pastikan kolom centang diaktifkan
            If Convert.ToBoolean(row.Cells(0).Value) Then
                ' Dapatkan nilai dari kolom "Bayar" dan tambahkan ke total
                Dim bayar As Decimal = If(IsDBNull(row.Cells("Bayar").Value) OrElse row.Cells("Bayar").Value Is Nothing, 0, Convert.ToDecimal(row.Cells("Bayar").Value))
                totalBayar += bayar
            End If
        Next

        ' Tampilkan total pada TextBox
        TxtTotalBayar.Text = totalBayar.ToString()
        LblTotalBayar.Text = totalBayar.ToString("N0")
    End Sub


    Private Sub Txt_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtTotalPiutang.TextChanged, TxtTotalBayar.TextChanged
        HitungSisaHutang()
    End Sub

    Private Sub HitungSisaHutang()
        ' Pastikan TxtTotalHutang dan TxtTotalBayar berisi nilai numerik
        Dim totalHutang As Decimal = If(Decimal.TryParse(TxtTotalPiutang.Text, totalHutang), totalHutang, 0)
        Dim totalBayar As Decimal = If(Decimal.TryParse(TxtTotalBayar.Text, totalBayar), totalBayar, 0)

        ' Hitung sisa hutang
        Dim sisaHutang As Decimal = totalHutang - totalBayar

        ' Tampilkan hasilnya pada TxtSisaHutang
        TxtSisaPiutang.Text = sisaHutang.ToString()
        LblSisaPiutang.Text = sisaHutang.ToString("N0")
    End Sub


    Private Function IsFormValid() As Boolean
        ' Pengecekan Pelanggan
        If CmbPelanggan.SelectedIndex = -1 Then
            MessageBox.Show("Silahkan pilih Pelanggan.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbPelanggan.DroppedDown = True
            Return False
        End If

        ' Pengecekan Total Piutang
        Dim totalHutang As Decimal
        If Decimal.TryParse(TxtTotalPiutang.Text, totalHutang) AndAlso totalHutang = 0 Then
            MessageBox.Show("Tidak ada Piutang yang harus di bayar.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        ' Pengecekan Total Bayar
        Dim totalBayar As Decimal
        If Decimal.TryParse(TxtTotalBayar.Text, totalBayar) AndAlso totalBayar = 0 Then
            MessageBox.Show("Silahkan Centang Piutang yang mau di bayar.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        ' Pengecekan Rekening
        If CmbRekening.SelectedIndex = -1 Then
            MessageBox.Show("Silahkan pilih Rekening sumber dana.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbRekening.DroppedDown = True
            Return False
        End If

        ' Pengecekan apakah pembayaran (kolom 11) lebih besar daripada hutang (kolom 9)
        Dim adaKesalahan As Boolean = False
        For Each row As DataGridViewRow In DgvData.Rows
            Dim hutang As Decimal = If(IsDBNull(row.Cells(9).Value), 0, Convert.ToDecimal(row.Cells(9).Value))
            Dim pembayaran As Decimal = If(IsDBNull(row.Cells(11).Value), 0, Convert.ToDecimal(row.Cells(11).Value))

            If pembayaran > hutang Then
                ' Mewarnai sel kolom 11 dengan warna merah
                row.Cells(11).Style.BackColor = Color.Red
                DgvData.ClearSelection()
                adaKesalahan = True
            Else
                ' Reset warna jika tidak ada kesalahan
                row.Cells(11).Style.BackColor = Color.White
            End If
        Next

        ' Jika ada kesalahan, munculkan pesan dan kembalikan False
        If adaKesalahan Then
            MessageBox.Show("Jumlah pembayaran lebih besar daripada hutang.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        ' Semua validasi berhasil
        Return True
    End Function



    Private Sub BtnBayar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBayar.Click
        If Not IsFormValid() Then
            Return
        End If

        Cursor = Cursors.WaitCursor

        If TransaksiLampau = "Tidak" Then
            DtpTanggal.Value = Now
            GenerateNomorBayarHutang()
        End If


        Dim transaction As MySqlTransaction = Nothing

        Try
            transaction = conn.BeginTransaction()

            For baris As Integer = 0 To DgvData.Rows.Count - 1
                If Convert.ToBoolean(DgvData.Rows(baris).Cells(0).Value) = True Then

                    Dim Status As String
                    Dim hutang As Decimal = If(IsDBNull(DgvData.Rows(baris).Cells(9).Value) OrElse DgvData.Rows(baris).Cells(9).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(9).Value))
                    Dim bayar As Decimal = If(IsDBNull(DgvData.Rows(baris).Cells(11).Value) OrElse DgvData.Rows(baris).Cells(11).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(11).Value))

                    ' Memeriksa apakah nilai hutang sama dengan nilai bayar
                    Status = If(hutang = bayar, "Lunas", "Belum Lunas")

                    ' Update tabel penjualan
                    Using cmdUpdateBeli As New MySqlCommand("UPDATE penjualan SET BAYAR = BAYAR + @BAYAR, SISA_TAGIHAN = SISA_TAGIHAN - @SISA_TAGIHAN, TGL_PEMBAYARAN = @TGL_PEMBAYARAN, NOMINALBAYARPIUTANG = NOMINALBAYARPIUTANG + @NOMINALBAYARPIUTANG, STATUS_TRANSAKSI = @STATUS_TRANSAKSI WHERE ID_PENJUALAN = @ID_PENJUALAN", conn, transaction)
                        ' Menambahkan nilai bayar ke kolom BAYAR
                        cmdUpdateBeli.Parameters.AddWithValue("@BAYAR", bayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@SISA_TAGIHAN", bayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@TGL_PEMBAYARAN", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmdUpdateBeli.Parameters.AddWithValue("@NOMINALBAYARPIUTANG", bayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@STATUS_TRANSAKSI", Status)
                        cmdUpdateBeli.Parameters.AddWithValue("@ID_PENJUALAN", DgvData.Rows(baris).Cells(1).Value)
                        cmdUpdateBeli.ExecuteNonQuery()
                    End Using


                    Using cmdJurnal As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_K, KODE_BANTU_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                       "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_K, @KODE_BANTU_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
                        ' Set nilai untuk parameter
                        cmdJurnal.Parameters.AddWithValue("@NO_TRANSAKSI", LblNomorBayar.Text)
                        cmdJurnal.Parameters.AddWithValue("@TGL_TRANSAKSI", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmdJurnal.Parameters.AddWithValue("@NO_NOTA", DgvData.Rows(baris).Cells(1).Value)
                        cmdJurnal.Parameters.AddWithValue("@URAIAN", "Bayar piutang dari " & CmbPelanggan.Text & " Jatuh tempo " & DgvData.Rows(baris).Cells(10).Value)
                        cmdJurnal.Parameters.AddWithValue("@NAMA_AKUN_D", CmbRekening.Text) ' Akun Debet
                        cmdJurnal.Parameters.AddWithValue("@NOMOR_AKUN_D", TxtRekening.Text) ' Nomor Akun Debet
                        cmdJurnal.Parameters.AddWithValue("@NAMA_AKUN_K", nama_rek_Piutang_Jual) ' Akun Kredit
                        cmdJurnal.Parameters.AddWithValue("@NOMOR_AKUN_K", Kode_rek_Piutang_Jual) ' Nomor Akun Kredit
                        cmdJurnal.Parameters.AddWithValue("@NAMA_BANTU_K", CmbPelanggan.Text) ' Nama Bantu Kredit (pelanggan)
                        cmdJurnal.Parameters.AddWithValue("@KODE_BANTU_K", LblKodePelanggan.Text) ' Kode Bantu Kredit
                        cmdJurnal.Parameters.AddWithValue("@NOMINAL", bayar) ' Nilai nominal transaksi
                        cmdJurnal.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Bayar piutang")
                        cmdJurnal.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text) ' Lokasi transaksi
                        cmdJurnal.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text) ' User ID
                        cmdJurnal.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text) ' ID Komputer

                        ' Eksekusi query
                        cmdJurnal.ExecuteNonQuery()
                    End Using



                    Using cmdPiutangDetail As New MySqlCommand("INSERT INTO Piutang_Detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_JUAL, KODE, NAMA, JENIS, TANGGAL_JUAL, PIUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
                                     "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_JUAL, @KODE, @NAMA, @JENIS, @TANGGAL_JUAL, @PIUTANG, @DIBAYAR, @RETUR, @HUTANG, @JATUH_TEMPO, @PEMBAYARAN, @STATUS, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                        ' Set nilai untuk parameter
                        cmdPiutangDetail.Parameters.AddWithValue("@ID_BAYAR", LblNomorBayar.Text)
                        cmdPiutangDetail.Parameters.AddWithValue("@TANGGAL_BAYAR", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmdPiutangDetail.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                        cmdPiutangDetail.Parameters.AddWithValue("@ID_JUAL", DgvData.Rows(baris).Cells(1).Value)
                        cmdPiutangDetail.Parameters.AddWithValue("@KODE", DgvData.Rows(baris).Cells(2).Value)
                        cmdPiutangDetail.Parameters.AddWithValue("@NAMA", DgvData.Rows(baris).Cells(3).Value)
                        cmdPiutangDetail.Parameters.AddWithValue("@JENIS", DgvData.Rows(baris).Cells(4).Value)

                        ' Jika nilai TANGGAL_JUAL adalah DateTime, format nilainya. Jika tidak, gunakan nilai default
                        Dim tanggalJual As DateTime
                        If DateTime.TryParse(DgvData.Rows(baris).Cells(5).Value.ToString(), tanggalJual) Then
                            cmdPiutangDetail.Parameters.AddWithValue("@TANGGAL_JUAL", tanggalJual.ToString("yyyy-MM-dd HH:mm:ss"))
                        Else
                            cmdPiutangDetail.Parameters.AddWithValue("@TANGGAL_JUAL", DBNull.Value)
                        End If

                        cmdPiutangDetail.Parameters.AddWithValue("@PIUTANG", If(IsDBNull(DgvData.Rows(baris).Cells(6).Value) OrElse DgvData.Rows(baris).Cells(6).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(6).Value)))
                        cmdPiutangDetail.Parameters.AddWithValue("@DIBAYAR", If(IsDBNull(DgvData.Rows(baris).Cells(7).Value) OrElse DgvData.Rows(baris).Cells(7).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(7).Value)))
                        cmdPiutangDetail.Parameters.AddWithValue("@RETUR", If(IsDBNull(DgvData.Rows(baris).Cells(8).Value) OrElse DgvData.Rows(baris).Cells(8).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(8).Value)))
                        cmdPiutangDetail.Parameters.AddWithValue("@HUTANG", hutang)

                        ' Jika nilai JATUH_TEMPO adalah DateTime, format nilainya. Jika tidak, gunakan nilai default
                        Dim jatuhTempo As DateTime
                        If DateTime.TryParse(DgvData.Rows(baris).Cells(10).Value.ToString(), jatuhTempo) Then
                            cmdPiutangDetail.Parameters.AddWithValue("@JATUH_TEMPO", jatuhTempo.ToString("yyyy-MM-dd HH:mm:ss"))
                        Else
                            cmdPiutangDetail.Parameters.AddWithValue("@JATUH_TEMPO", DBNull.Value)
                        End If

                        cmdPiutangDetail.Parameters.AddWithValue("@PEMBAYARAN", bayar)
                        cmdPiutangDetail.Parameters.AddWithValue("@STATUS", Status)
                        cmdPiutangDetail.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                        cmdPiutangDetail.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

                        ' Eksekusi query
                        cmdPiutangDetail.ExecuteNonQuery()
                    End Using


                End If
            Next

            Dim query As String = "INSERT INTO Piutang (ID_BAYAR_PIUTANG, KODE_PELANGGAN, NAMA_PELANGGAN, TGL_BAYAR, TOTAL_PIUTANG, NOMINAL_BAYAR, SISA_PIUTANG, LOKASI, ID_USER_BAYAR, ID_KOMPUTER_BAYAR) " &
                       "VALUES (@ID_BAYAR_PIUTANG, @KODE_PELANGGAN, @NAMA_PELANGGAN, @TGL_BAYAR, @TOTAL_PIUTANG, @NOMINAL_BAYAR, @SISA_PIUTANG, @LOKASI, @ID_USER_BAYAR, @ID_KOMPUTER_BAYAR)"

            Using cmd As New MySqlCommand(query, conn, transaction)
                ' Menambahkan parameter untuk query
                cmd.Parameters.AddWithValue("@ID_BAYAR_PIUTANG", LblNomorBayar.Text)
                cmd.Parameters.AddWithValue("@KODE_PELANGGAN", LblKodePelanggan.Text)
                cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbPelanggan.Text)
                cmd.Parameters.AddWithValue("@TGL_BAYAR", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TOTAL_PIUTANG", If(String.IsNullOrEmpty(TxtTotalPiutang.Text), 0D, Convert.ToDecimal(TxtTotalPiutang.Text)))
                cmd.Parameters.AddWithValue("@NOMINAL_BAYAR", If(String.IsNullOrEmpty(TxtTotalBayar.Text), 0D, Convert.ToDecimal(TxtTotalBayar.Text)))
                cmd.Parameters.AddWithValue("@SISA_PIUTANG", If(String.IsNullOrEmpty(TxtSisaPiutang.Text), 0D, Convert.ToDecimal(TxtSisaPiutang.Text)))
                cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                cmd.Parameters.AddWithValue("@ID_USER_BAYAR", FormUtama.SLogin.Text)
                cmd.Parameters.AddWithValue("@ID_KOMPUTER_BAYAR", FormUtama.Comp.Text)

                ' Eksekusi query
                cmd.ExecuteNonQuery()
            End Using


            transaction.Commit()

            DatabaseModule.CatatanAksiHistory("Bayar Piutang " & LblNomorBayar.Text)
            Kondisiawal()
        Catch ex As Exception
            transaction.Rollback()

            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub


    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluar.Click
        Close()
        FormUtama.GBTransaksi.Visible = True
        FormUtama.Refresdatagridview()
    End Sub


    Private Sub CmbRekening_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekening.SelectedIndexChanged
        Dim sql As String = "SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@selectedNAMA", CmbRekening.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TxtRekening.Text = reader("Kode_akun").ToString()
                End If
            End Using
        End Using
    End Sub

    Private Sub FormBayarPiutang_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F8
                BtnBayar.PerformClick()
            Case Keys.Escape
                If PanelView.Visible = True Then
                    PanelView.Visible = False
                Else
                    BtnKeluar.PerformClick()
                End If
        End Select
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        PanelView.Visible = False
    End Sub
End Class