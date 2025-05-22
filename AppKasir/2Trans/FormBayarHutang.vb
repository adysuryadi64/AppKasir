Imports System.Globalization






Public Class FormBayarHutang
    Private Sub FormBayarHutang_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Kondisiawal()
    End Sub


    Private Sub GenerateNomorBayarHutang()
        Dim cekTanggal As String = Microsoft.VisualBasic.Format(DtpTanggal.Value, "yyMMdd")
        Dim UrutKOde As String = ""
        Dim cekNomor As String = "BH-" & cekTanggal

        ' Query untuk mendapatkan nomor maksimum berdasarkan format
        Using cmd As New MySqlCommand("SELECT MAX(NOBAYARHUTANG) FROM hutang WHERE NOBAYARHUTANG LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")

            ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
            Dim maxKode As Object = cmd.ExecuteScalar()

            If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                Dim MaxNilaiKode As String = maxKode.ToString()
                If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "BH-" & cekTanggal Then
                    ' Hitung nomor berikutnya
                    Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                    UrutKOde = "BH-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                End If
            End If
        End Using

        ' Jika UrutKOde masih kosong, buat nomor pertama
        If String.IsNullOrEmpty(UrutKOde) Then
            UrutKOde = "BH-" & cekTanggal & "0001"
        End If

        ' Tampilkan nomor pada label
        LblNomorBayar.Text = UrutKOde
    End Sub


    Private Sub Kondisiawal()
        PanelView.Visible = False
        TxtTotalHutang.Text = 0
        TxtTotalBayar.Text = 0
        TxtSisaHutang.Text = 0
        LblTotalHutang.Text = "0"
        LblTotalBayar.Text = "0"
        LblSisaHutang.Text = "0"
        CmbSupliyer.Text = ""
        LblKodeSupliyer.Text = ""
        DgvData.Rows.Clear()


        CmbRekening.Items.Clear()
        ' Isi ComboBox dengan data dari list
        CmbRekening.Items.AddRange(GetDaftarAkun().ToArray())


        GenerateNomorBayarHutang()
        SelectNamaSupliyer()

        '' Set akun berdasarkan lokasi
        'If FormUtama.TxtLokasi.Text = "TOKO" Then
        '    CmbRekening.SelectedItem = nama_rek_Jual_Toko
        '    TxtRekening.Text = Kode_rek_Jual_Toko
        'ElseIf FormUtama.TxtLokasi.Text = "GUDANG" Then
        '    CmbRekening.SelectedItem = nama_rek_Jual_Gudang
        '    TxtRekening.Text = Kode_rek_Jual_Gudang
        'End If
    End Sub


    Private Sub Rekeningbayar()
        CmbRekening.Items.Clear()
        Dim namaakun As String = "SELECT Nama_Akun FROM tbl_datareferensi WHERE Type_Akun LIKE 'KAS' OR Type_Akun LIKE 'BANK' OR Type_Akun LIKE 'EKUITAS' ORDER BY Kode_akun ASC"

        Using cmd As New MySqlCommand(namaakun, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        Dim combinedValue As String = rd("Nama_Akun").ToString()
                        CmbRekening.Items.Add(combinedValue)
                    End While
                End If
            End Using
        End Using

    End Sub

    Public Sub Tampildatahutang()
        Using cmd As New MySqlCommand("SELECT ID_PEMBELIAN, ID_SUPPLIER, NAMA_SUPLIYER, TGL_BELI, GRAND_TOTAL_BELI, PEMBAYARAN, RETUR, TAGIHAN, JATUH_TEMPO FROM pembelian WHERE ID_SUPPLIER = @ID_SUPPLIER AND STATUS_TRANSAKSI_BELI = 'Belum Lunas' ORDER BY ID_PEMBELIAN", conn)
            cmd.Parameters.AddWithValue("@ID_SUPPLIER", LblKodeSupliyer.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                DgvData.Rows.Clear()
                While rd.Read()
                    ' Add a DataGridViewCheckBoxCell in the first column
                    DgvData.Rows.Add(False, rd("ID_PEMBELIAN"), rd("ID_SUPPLIER"), rd("NAMA_SUPLIYER"), rd("TGL_BELI"), rd("GRAND_TOTAL_BELI"), rd("PEMBAYARAN"), rd("RETUR"), rd("TAGIHAN"), rd("JATUH_TEMPO"))
                End While
            End Using
        End Using


        With DgvData
            .Columns("TGL_BELI").DefaultCellStyle.Format = "dd/MM/yyyy"
            .Columns("GRAND_TOTAL_BELI").DefaultCellStyle.Format = "###,###"
            .Columns("GRAND_TOTAL_BELI").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("PEMBAYARAN").DefaultCellStyle.Format = "###,###"
            .Columns("PEMBAYARAN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("RETUR").DefaultCellStyle.Format = "###,###"
            .Columns("RETUR").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("TAGIHAN").DefaultCellStyle.Format = "###,###"
            .Columns("TAGIHAN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("JATUH_TEMPO").DefaultCellStyle.Format = "dd/MM/yyyy"
        End With


        ' Misalkan DgvData memiliki 10 kolom
        DgvData.Columns(0).ReadOnly = False ' Memungkinkan pengeditan pada Kolom 0

        For i As Integer = 1 To 9
            DgvData.Columns(i).ReadOnly = True ' Kolom 1 sampai 9 hanya-baca
        Next i

        DgvData.ClearSelection()

        PanelView.Visible = False
        Totalhutang()
    End Sub


    Private Sub DgvData_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgvData.CellContentClick
        ' Pastikan kolom yang di klik adalah kolom "View"
        If e.RowIndex >= 0 AndAlso DgvData.Columns(e.ColumnIndex).Name = "View" AndAlso Not IsDBNull(DgvData.Rows(e.RowIndex).Cells("ID_PEMBELIAN").Value) Then
            ' Ambil nilai dari kolom "kode" pada baris yang dipilih
            Dim fakturBeli As String = DgvData.Rows(e.RowIndex).Cells("ID_PEMBELIAN").Value.ToString()
            LblDetail.Text = "Detail pembelian dengan nota = " & fakturBeli

            Dim dt As New DataTable()

            Using cmd As New MySqlCommand("SELECT ID_BARANG, NAMA_BARANG, QTY, SATUAN, TOTAL, ID_USER FROM pembelian_detail WHERE FAKTUR_BELI LIKE @FAKTUR_BELI", conn)
                cmd.Parameters.AddWithValue("@FAKTUR_BELI", fakturBeli)
                Using rd As New MySqlDataAdapter(cmd)
                    rd.Fill(dt)
                End Using
            End Using

            DgvDetail.DataSource = dt

            Dim columnsToFormat As String() = {"QTY", "TOTAL"}
            Dim columnNames As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
                {"ID_BARANG", "Kode"},
                {"NAMA_BARANG", "Nama Barang"},
                {"QTY", "Qty"},
                {"SATUAN", "Satuan"},
                {"TOTAL", "Total"},
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

    Public Sub SelectNamaSupliyer()
        CmbSupliyer.Items.Clear()
        Using cmd As New MySqlCommand("SELECT DISTINCT NAMA_SUPLIYER FROM pembelian WHERE STATUS_TRANSAKSI_BELI = 'Belum Lunas'", conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    CmbSupliyer.Items.Add(reader("NAMA_SUPLIYER").ToString())
                End While
            End Using
        End Using
    End Sub

    Private Sub CmbSatuanMasuk_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbSupliyer.SelectedIndexChanged

        Using cmd As New MySqlCommand("SELECT ID_SUPPLIER FROM pembelian WHERE NAMA_SUPLIYER = @NAMA_SUPLIYER", conn)
            cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", CmbSupliyer.Text)
            Using rd As MySqlDataReader = cmd.ExecuteReader
                If rd.Read() Then
                    LblKodeSupliyer.Text = rd.GetString(0)
                End If
            End Using
        End Using
        Tampildatahutang()
    End Sub

    Private Sub Totalhutang()
        ' Inisialisasi variabel untuk menyimpan total
        Dim totalKolom8 As Decimal = 0

        ' Loop melalui setiap baris pada DataGridView
        For Each row As DataGridViewRow In DgvData.Rows
            ' Pastikan baris bukan baris baru yang ditandai untuk penambahan
            If Not row.IsNewRow Then
                ' Dapatkan nilai dari kolom "TAGIHAN" dan anggap sebagai 0 jika null
                totalKolom8 += If(row.Cells("TAGIHAN").Value IsNot Nothing AndAlso
                                  Decimal.TryParse(row.Cells("TAGIHAN").Value.ToString(), Nothing),
                                  Convert.ToDecimal(row.Cells("TAGIHAN").Value), 0)
            End If
        Next

        ' Tampilkan total pada TextBox
        TxtTotalHutang.Text = totalKolom8.ToString()
        LblTotalHutang.Text = totalKolom8.ToString("N0")
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

                ' Jika dicentang, isi kolom "Bayar" dengan nilai dari kolom "Hutang"
                If isChecked Then
                    currentRow.Cells("Bayar").Value = currentRow.Cells("TAGIHAN").Value
                Else
                    ' Jika ceklis dihapus, kosongkan kolom "Bayar"
                    currentRow.Cells("Bayar").Value = 0
                End If

                ' Terapkan format N0 untuk kolom "Bayar" (angka tanpa desimal)
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
            DgvData.Rows(e.RowIndex).Cells("Bayar").Style.Format = "N0"
            DgvData.Rows(e.RowIndex).Cells("Bayar").Style.Alignment = DataGridViewContentAlignment.MiddleRight
        End If

        HitungTotalBayar()
    End Sub


    Private Sub HitungTotalBayar()
        ' Inisialisasi variabel untuk menyimpan total
        Dim totalBayar As Decimal = 0

        ' Loop melalui setiap baris pada DataGridView
        For Each row As DataGridViewRow In DgvData.Rows
            ' Pastikan kolom centang diaktifkan
            If Convert.ToBoolean(row.Cells(0).Value) Then
                ' Dapatkan nilai dari kolom "Bayar" dan anggap 0 jika null
                totalBayar += If(row.Cells("Bayar").Value IsNot Nothing AndAlso
                                  Decimal.TryParse(row.Cells("Bayar").Value.ToString(), Nothing),
                                  Convert.ToDecimal(row.Cells("Bayar").Value), 0)
            End If
        Next

        ' Tampilkan total pada TextBox
        TxtTotalBayar.Text = totalBayar.ToString()
        LblTotalBayar.Text = totalBayar.ToString("N0")
    End Sub

    Private Sub Txt_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TxtTotalHutang.TextChanged, TxtTotalBayar.TextChanged
        HitungSisaHutang()
    End Sub

    Private Sub HitungSisaHutang()
        ' Pastikan TxtTotalHutang dan TxtTotalBayar berisi nilai numerik
        Dim totalHutang As Decimal
        Dim totalBayar As Decimal

        If Decimal.TryParse(TxtTotalHutang.Text, totalHutang) AndAlso Decimal.TryParse(TxtTotalBayar.Text, totalBayar) Then
            ' Hitung sisa hutang
            Dim sisaHutang As Decimal = totalHutang - totalBayar

            ' Tampilkan hasilnya pada TxtSisaHutang
            TxtSisaHutang.Text = sisaHutang.ToString()
            LblSisaHutang.Text = sisaHutang.ToString("N0")
        Else
            ' Jika entri tidak valid, atur TxtSisaHutang ke nilai default atau pesan kesalahan
            TxtSisaHutang.Text = 0
            LblSisaHutang.Text = "0"
        End If
    End Sub
    Private Function IsFormValid() As Boolean
        If CmbSupliyer.SelectedIndex = -1 Then
            MessageBox.Show("Silahkan pilih Supliyer.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbSupliyer.DroppedDown = True
            Return False
        End If


        Dim totalHutang As Decimal
        If Decimal.TryParse(TxtTotalHutang.Text, totalHutang) AndAlso totalHutang = 0 Then
            MessageBox.Show("Tidak ada hutang yang harus di bayar.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Dim totalBayar As Decimal
        If Decimal.TryParse(TxtTotalBayar.Text, totalBayar) AndAlso totalBayar = 0 Then
            MessageBox.Show("Silahkan Centang hutang yang mau di bayar.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If CmbRekening.SelectedIndex = -1 Then
            MessageBox.Show("Silahkan pilih Rekening sumber dana.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbRekening.DroppedDown = True
            Return False
        End If

        Return True
    End Function

    Private Sub BtnBayar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnBayar.Click
        If Not IsFormValid() Then
            Return
        End If
        Cursor = Cursors.WaitCursor

        DtpTanggal.Value = Now
        GenerateNomorBayarHutang()

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = Nothing

        Try
            transaction = conn.BeginTransaction()

            For baris As Integer = 0 To DgvData.Rows.Count - 1
                If Convert.ToBoolean(DgvData.Rows(baris).Cells(0).Value) = True Then

                    Dim Status As String
                    Dim hutang As Decimal = If(IsDBNull(DgvData.Rows(baris).Cells(8).Value) OrElse DgvData.Rows(baris).Cells(8).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(8).Value))
                    Dim bayar As Decimal = If(IsDBNull(DgvData.Rows(baris).Cells(10).Value) OrElse DgvData.Rows(baris).Cells(10).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(10).Value))

                    ' Memeriksa apakah nilai hutang sama dengan nilai bayar
                    Status = If(hutang = bayar, "Lunas", "Belum Lunas")

                    Using cmdUpdateBeli As New MySqlCommand("UPDATE pembelian SET PEMBAYARAN = PEMBAYARAN + @PEMBAYARAN, TAGIHAN = TAGIHAN - @TAGIHAN, TGL_BAYAR = @TGL_BAYAR, NOMINALBAYAR = NOMINALBAYAR + @NOMINALBAYAR, STATUS_TRANSAKSI_BELI = @STATUS_TRANSAKSI_BELI WHERE ID_PEMBELIAN = @ID_PEMBELIAN", conn, transaction)
                        cmdUpdateBeli.Parameters.AddWithValue("@PEMBAYARAN", bayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@TAGIHAN", bayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@TGL_BAYAR", Microsoft.VisualBasic.Format(DtpTanggal.Value, "yyyy-MM-dd HH:mm:ss"))
                        cmdUpdateBeli.Parameters.AddWithValue("@NOMINALBAYAR", bayar)
                        cmdUpdateBeli.Parameters.AddWithValue("@STATUS_TRANSAKSI_BELI", Status)
                        cmdUpdateBeli.Parameters.AddWithValue("@ID_PEMBELIAN", DgvData.Rows(baris).Cells("ID_PEMBELIAN").Value)

                        cmdUpdateBeli.ExecuteNonQuery()
                    End Using


                    Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_D, KODE_BANTU_D, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                  "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_D, @KODE_BANTU_D, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                        cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNomorBayar.Text)
                        cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmd.Parameters.AddWithValue("@NO_NOTA", DgvData.Rows(baris).Cells("ID_PEMBELIAN").Value)
                        cmd.Parameters.AddWithValue("@URAIAN", "Bayar hutang ke " & CmbSupliyer.Text & " Jatuh tempo " & DgvData.Rows(baris).Cells("JATUH_TEMPO").Value)
                        cmd.Parameters.AddWithValue("@NAMA_AKUN_D", nama_rek_Hutang_Beli)
                        cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", Kode_rek_Hutang_Beli)
                        cmd.Parameters.AddWithValue("@NAMA_AKUN_K", CmbRekening.Text)
                        cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", TxtRekening.Text)
                        cmd.Parameters.AddWithValue("@NAMA_BANTU_D", CmbSupliyer.Text)
                        cmd.Parameters.AddWithValue("@KODE_BANTU_D", LblKodeSupliyer.Text)
                        cmd.Parameters.AddWithValue("@NOMINAL", bayar)
                        cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Bayar hutang")
                        cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                        cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                        cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

                        cmd.ExecuteNonQuery()
                    End Using


                    Using cmdHutangDetail As New MySqlCommand("INSERT INTO Hutang_Detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA, TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
                                    "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_BELI, @KODE, @NAMA, @TANGGAL_BELI, @TOTAL_HUTANG, @DIBAYAR, @RETUR, @HUTANG, @JATUH_TEMPO, @PEMBAYARAN, @STATUS, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                        ' Set nilai untuk parameter
                        cmdHutangDetail.Parameters.AddWithValue("@ID_BAYAR", LblNomorBayar.Text)
                        cmdHutangDetail.Parameters.AddWithValue("@TANGGAL_BAYAR", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        cmdHutangDetail.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                        cmdHutangDetail.Parameters.AddWithValue("@ID_BELI", DgvData.Rows(baris).Cells(1).Value)
                        cmdHutangDetail.Parameters.AddWithValue("@KODE", DgvData.Rows(baris).Cells(2).Value)
                        cmdHutangDetail.Parameters.AddWithValue("@NAMA", DgvData.Rows(baris).Cells(3).Value)

                        ' Jika nilai TANGGAL_BELI adalah DateTime, format nilainya. Jika tidak, gunakan nilai default
                        Dim tanggalBeli As DateTime
                        If DateTime.TryParse(DgvData.Rows(baris).Cells(4).Value.ToString(), tanggalBeli) Then
                            cmdHutangDetail.Parameters.AddWithValue("@TANGGAL_BELI", tanggalBeli.ToString("yyyy-MM-dd HH:mm:ss"))
                        Else
                            cmdHutangDetail.Parameters.AddWithValue("@TANGGAL_BELI", DBNull.Value)
                        End If

                        cmdHutangDetail.Parameters.AddWithValue("@TOTAL_HUTANG", If(IsDBNull(DgvData.Rows(baris).Cells(5).Value) OrElse DgvData.Rows(baris).Cells(5).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(5).Value)))
                        cmdHutangDetail.Parameters.AddWithValue("@DIBAYAR", If(IsDBNull(DgvData.Rows(baris).Cells(6).Value) OrElse DgvData.Rows(baris).Cells(6).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(6).Value)))
                        cmdHutangDetail.Parameters.AddWithValue("@RETUR", If(IsDBNull(DgvData.Rows(baris).Cells(7).Value) OrElse DgvData.Rows(baris).Cells(7).Value Is Nothing, 0, Convert.ToDecimal(DgvData.Rows(baris).Cells(7).Value)))
                        cmdHutangDetail.Parameters.AddWithValue("@HUTANG", hutang)

                        ' Jika nilai JATUH_TEMPO adalah DateTime, format nilainya. Jika tidak, gunakan nilai default
                        Dim jatuhTempo As DateTime
                        If DateTime.TryParse(DgvData.Rows(baris).Cells(9).Value.ToString(), jatuhTempo) Then
                            cmdHutangDetail.Parameters.AddWithValue("@JATUH_TEMPO", jatuhTempo.ToString("yyyy-MM-dd HH:mm:ss"))
                        Else
                            cmdHutangDetail.Parameters.AddWithValue("@JATUH_TEMPO", DBNull.Value)
                        End If

                        cmdHutangDetail.Parameters.AddWithValue("@PEMBAYARAN", bayar)
                        cmdHutangDetail.Parameters.AddWithValue("@STATUS", Status)
                        cmdHutangDetail.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                        cmdHutangDetail.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

                        ' Eksekusi query
                        cmdHutangDetail.ExecuteNonQuery()
                    End Using

                End If
            Next

            Dim query As String = "INSERT INTO hutang (NOBAYARHUTANG, KODESUPLIYER, NAMASUPLIYER, TGLPEMBAYARAN, LOKASI, TOTALHUTANG, NOMINALBAYAR, SISAHUTANG, ID_USER_BAYAR, ID_KOMPUTER_BAYAR) " &
                          "VALUES (@NOBAYARHUTANG, @KODESUPLIYER, @NAMASUPLIYER, @TGLPEMBAYARAN, @LOKASI, @TOTALHUTANG, @NOMINALBAYAR, @SISAHUTANG, @IDUser, @IDKomputer)"

            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@NOBAYARHUTANG", LblNomorBayar.Text)
                cmd.Parameters.AddWithValue("@KODESUPLIYER", LblKodeSupliyer.Text)
                cmd.Parameters.AddWithValue("@NAMASUPLIYER", CmbSupliyer.Text)
                cmd.Parameters.AddWithValue("@TGLPEMBAYARAN", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
                cmd.Parameters.AddWithValue("@TOTALHUTANG", If(String.IsNullOrEmpty(TxtTotalHutang.Text), 0D, Convert.ToDecimal(TxtTotalHutang.Text)))
                cmd.Parameters.AddWithValue("@NOMINALBAYAR", If(String.IsNullOrEmpty(TxtTotalBayar.Text), 0D, Convert.ToDecimal(TxtTotalBayar.Text)))
                cmd.Parameters.AddWithValue("@SISAHUTANG", If(String.IsNullOrEmpty(TxtSisaHutang.Text), 0D, Convert.ToDecimal(TxtSisaHutang.Text)))
                cmd.Parameters.AddWithValue("@IDUser", FormUtama.SLogin.Text)
                cmd.Parameters.AddWithValue("@IDKomputer", FormUtama.Comp.Text)

                ' Eksekusi query
                cmd.ExecuteNonQuery()
            End Using


            ' Commit transaksi jika semua berhasil
            transaction.Commit()
            DatabaseModule.CatatanAksiHistory("Bayar hutang " & LblNomorBayar.Text)
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

    Private Sub FormBayarHutang_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
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