Imports System.Globalization

Public Class FormBayarHutang

    Private Sub FormBayarHutang_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Setting dibaca langsung dari ModulHakAkses property
        Kondisiawal()
    End Sub



    Private Sub GenerateNomorBayarHutang()
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "BH")
            cmd.Parameters.AddWithValue("@tgl", DtpTanggal.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "hutang")
            cmd.Parameters.AddWithValue("@kolom", "NOBAYARHUTANG")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            LblNomorBayar.Text = pNomor.Value?.ToString()
        End Using
    End Sub


    Private Sub Kondisiawal()
        PanelDatagridview.Visible = False
        TxtTotalHutang.Text = 0
        TxtTotalBayar.Text = 0
        TxtSisaHutang.Text = 0
        LblTotalHutang.Text = "0"
        LblTotalBayar.Text = "0"
        LblSisaHutang.Text = "0"
        CmbSupliyer.Text = ""
        LblKodeSupliyer.Text = ""
        DgvData.Rows.Clear()


        IsiComboBoxAkun(CmbRekening, "KAS", "BANK", "EKUITAS")
        CmbRekening.SelectedItem = nama_rek_Bayar_Hutang

        ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
        DtpTanggal.Format = DateTimePickerFormat.Custom
        DtpTanggal.CustomFormat = "dd/MM/yyyy HH:mm:ss"
        GenerateNomorBayarHutang()
        SelectNamaSupliyer()
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
            .Columns("JATUH_TEMPO").DefaultCellStyle.Format = "dd/MM/yyyy"
        End With
        ModuleAngka.TerapkanFormatKolomAngka(DgvData, "GRAND_TOTAL_BELI", "PEMBAYARAN", "RETUR", "TAGIHAN")


        ' Misalkan DgvData memiliki 10 kolom
        DgvData.Columns(0).ReadOnly = False ' Memungkinkan pengeditan pada Kolom 0

        For i As Integer = 1 To 9
            DgvData.Columns(i).ReadOnly = True ' Kolom 1 sampai 9 hanya-baca
        Next i

        DgvData.ClearSelection()

        PanelDatagridview.Visible = False
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
                ' Rename columns
                For Each column As DataGridViewColumn In .Columns
                    If columnNames.ContainsKey(column.Name) Then
                        column.HeaderText = columnNames(column.Name)
                    End If
                Next

                ' Set visual style
                .BorderStyle = BorderStyle.FixedSingle

                ' Enable double buffering to reduce flickering
                ModuleTheme.ApplyThemeDataGridView(DgvDetail)

                ' Set FillWeight for NAMA_BARANG column
                If .Columns.Contains("NAMA_BARANG") Then
                    .Columns("NAMA_BARANG").FillWeight = 200
                End If
            End With
            ModuleAngka.TerapkanFormatKolomAngka(DgvDetail, columnsToFormat)
            DgvDetail.ClearSelection()
            PanelDatagridview.Visible = True
        End If
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
        Dim totalKolom8 As Decimal = 0D

        ' Loop melalui setiap baris pada DataGridView
        For Each row As DataGridViewRow In DgvData.Rows
            ' Pastikan baris bukan baris baru yang ditandai untuk penambahan
            If Not row.IsNewRow Then
                If DgvData.Columns.Contains("TAGIHAN") Then
                    Dim cellVal = row.Cells("TAGIHAN").Value
                    totalKolom8 += ModuleAngka.ParseDecimal(cellVal)
                End If
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
        Dim totalBayar As Decimal = 0D

        ' Loop melalui setiap baris pada DataGridView
        For Each row As DataGridViewRow In DgvData.Rows
            If row.IsNewRow Then
                Continue For
            End If

            ' Pastikan kolom cek (index 0) ada dan tidak null
            Dim checkVal = row.Cells(0).Value
            Dim isChecked As Boolean = False
            If checkVal IsNot Nothing AndAlso Not IsDBNull(checkVal) Then
                Boolean.TryParse(Convert.ToString(checkVal), isChecked)
            End If

            If isChecked Then
                If DgvData.Columns.Contains("Bayar") Then
                    Dim cellVal = row.Cells("Bayar").Value
                    totalBayar += ModuleAngka.ParseDecimal(cellVal)
                End If
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
        Dim totalHutang As Decimal = ModuleAngka.ParseDecimal(TxtTotalHutang.Text)
        Dim totalBayar As Decimal = ModuleAngka.ParseDecimal(TxtTotalBayar.Text)
        Dim sisaHutang As Decimal = totalHutang - totalBayar
        TxtSisaHutang.Text = sisaHutang.ToString()
        LblSisaHutang.Text = ModuleAngka.FormatRupiah(sisaHutang)
    End Sub
    Private Function IsFormValid() As Boolean
        If CmbSupliyer.SelectedIndex = -1 Then
            MessageBox.Show("Silahkan pilih Supliyer.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbSupliyer.DroppedDown = True
            Return False
        End If


        If ModuleAngka.ParseDecimal(TxtTotalHutang.Text) = 0 Then
            MessageBox.Show("Tidak ada hutang yang harus di bayar.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If ModuleAngka.ParseDecimal(TxtTotalBayar.Text) = 0 Then
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

        If Not ModulHakAkses.SettingIzinkanTanggalLampau Then
            ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
            GenerateNomorBayarHutang()
        End If

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = Nothing

        Try
            transaction = conn.BeginTransaction()

            Dim totalNominalBayarHutang As Decimal = 0D
            For baris As Integer = 0 To DgvData.Rows.Count - 1
                If DgvData.Rows(baris).IsNewRow Then
                    Continue For
                End If

                Dim checkVal = DgvData.Rows(baris).Cells(0).Value
                If checkVal Is Nothing OrElse IsDBNull(checkVal) OrElse Not Convert.ToBoolean(checkVal) Then
                    Continue For
                End If

                Dim Status As String
                Dim hutang As Decimal = If(IsDBNull(DgvData.Rows(baris).Cells(8).Value) OrElse DgvData.Rows(baris).Cells(8).Value Is Nothing, 0D, ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(8).Value))
                Dim bayar As Decimal = If(IsDBNull(DgvData.Rows(baris).Cells(10).Value) OrElse DgvData.Rows(baris).Cells(10).Value Is Nothing, 0D, ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(10).Value))

                ' Memeriksa apakah nilai hutang sama dengan nilai bayar
                Status = If(hutang = bayar, "Lunas", "Belum Lunas")

                Using cmdUpdateBeli As New MySqlCommand("UPDATE pembelian SET PEMBAYARAN = PEMBAYARAN + @PEMBAYARAN, TAGIHAN = TAGIHAN - @TAGIHAN, TGL_BAYAR = @TGL_BAYAR, NOMINALBAYAR = NOMINALBAYAR + @NOMINALBAYAR, STATUS_TRANSAKSI_BELI = @STATUS_TRANSAKSI_BELI WHERE ID_PEMBELIAN = @ID_PEMBELIAN", conn, transaction)
                    cmdUpdateBeli.Parameters.AddWithValue("@PEMBAYARAN", bayar)
                    cmdUpdateBeli.Parameters.AddWithValue("@TAGIHAN", bayar)
                    cmdUpdateBeli.Parameters.AddWithValue("@TGL_BAYAR", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
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
                    cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "BAYAR HUTANG")
                    cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

                    cmd.ExecuteNonQuery()
                End Using

                Using cmdHutangDetail As New MySqlCommand("INSERT INTO Hutang_Detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA, JENIS, TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
                            "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_BELI, @KODE, @NAMA, 'BAYAR', @TANGGAL_BELI, @TOTAL_HUTANG, @DIBAYAR, @RETUR, @HUTANG, @JATUH_TEMPO, @PEMBAYARAN, @STATUS, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                    ' Set nilai untuk parameter
                    cmdHutangDetail.Parameters.AddWithValue("@ID_BAYAR", LblNomorBayar.Text)
                    cmdHutangDetail.Parameters.AddWithValue("@TANGGAL_BAYAR", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdHutangDetail.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
                    cmdHutangDetail.Parameters.AddWithValue("@ID_BELI", DgvData.Rows(baris).Cells(1).Value)
                    cmdHutangDetail.Parameters.AddWithValue("@KODE", DgvData.Rows(baris).Cells(2).Value)
                    cmdHutangDetail.Parameters.AddWithValue("@NAMA", DgvData.Rows(baris).Cells(3).Value)

                    ' Jika nilai TANGGAL_BELI adalah DateTime, format nilainya. Jika tidak, gunakan nilai default
                    Dim tanggalBeli As DateTime
                    Dim cellTanggalBeli = DgvData.Rows(baris).Cells(4).Value
                    If cellTanggalBeli IsNot Nothing AndAlso Not IsDBNull(cellTanggalBeli) AndAlso DateTime.TryParse(Convert.ToString(cellTanggalBeli), tanggalBeli) Then
                        cmdHutangDetail.Parameters.AddWithValue("@TANGGAL_BELI", tanggalBeli.ToString("yyyy-MM-dd HH:mm:ss"))
                    Else
                        cmdHutangDetail.Parameters.AddWithValue("@TANGGAL_BELI", DBNull.Value)
                    End If

                    cmdHutangDetail.Parameters.AddWithValue("@TOTAL_HUTANG", ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(5).Value))
                    cmdHutangDetail.Parameters.AddWithValue("@DIBAYAR", ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(6).Value))
                    cmdHutangDetail.Parameters.AddWithValue("@RETUR", ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(7).Value))
                    cmdHutangDetail.Parameters.AddWithValue("@HUTANG", hutang)

                    ' Jika nilai JATUH_TEMPO adalah DateTime, format nilainya. Jika tidak, gunakan nilai default
                    Dim jatuhTempo As DateTime
                    Dim cellJatuhTempo = DgvData.Rows(baris).Cells(9).Value
                    If cellJatuhTempo IsNot Nothing AndAlso Not IsDBNull(cellJatuhTempo) AndAlso DateTime.TryParse(Convert.ToString(cellJatuhTempo), jatuhTempo) Then
                        cmdHutangDetail.Parameters.AddWithValue("@JATUH_TEMPO", jatuhTempo.ToString("yyyy-MM-dd HH:mm:ss"))
                    Else
                        cmdHutangDetail.Parameters.AddWithValue("@JATUH_TEMPO", DBNull.Value)
                    End If

                    cmdHutangDetail.Parameters.AddWithValue("@PEMBAYARAN", bayar)
                    cmdHutangDetail.Parameters.AddWithValue("@STATUS", Status)
                    cmdHutangDetail.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                    cmdHutangDetail.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

                    ' Eksekusi query
                    cmdHutangDetail.ExecuteNonQuery()
                End Using

                ' Perbarui baris BELI di hutang_detail — kurangi sisa hutang
                Using cmdUpdateTimbul As New MySqlCommand(
                    "UPDATE hutang_detail SET " &
                    "HUTANG = HUTANG - @BAYAR, " &
                    "DIBAYAR = DIBAYAR + @BAYAR, " &
                    "STATUS = CASE WHEN (HUTANG - @BAYAR) <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END " &
                    "WHERE ID_BELI = @ID_BELI AND JENIS = 'BELI'", conn, transaction)
                    cmdUpdateTimbul.Parameters.AddWithValue("@BAYAR", bayar)
                    cmdUpdateTimbul.Parameters.AddWithValue("@ID_BELI", DgvData.Rows(baris).Cells("ID_PEMBELIAN").Value)
                    cmdUpdateTimbul.ExecuteNonQuery()
                    ' Jika baris BELI tidak ditemukan (faktur lama sebelum migrasi), tidak error — lanjutkan
                End Using

                totalNominalBayarHutang += bayar

            Next

            Dim query As String = "INSERT INTO hutang (NOBAYARHUTANG, KODESUPLIYER, NAMASUPLIYER, TGLPEMBAYARAN, LOKASI, TOTALHUTANG, NOMINALBAYAR, SISAHUTANG, ID_USER_BAYAR, ID_KOMPUTER_BAYAR) " &
                      "VALUES (@NOBAYARHUTANG, @KODESUPLIYER, @NAMASUPLIYER, @TGLPEMBAYARAN, @LOKASI, @TOTALHUTANG, @NOMINALBAYAR, @SISAHUTANG, @IDUser, @IDKomputer)"

            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@NOBAYARHUTANG", LblNomorBayar.Text)
                cmd.Parameters.AddWithValue("@KODESUPLIYER", LblKodeSupliyer.Text)
                cmd.Parameters.AddWithValue("@NAMASUPLIYER", CmbSupliyer.Text)
                cmd.Parameters.AddWithValue("@TGLPEMBAYARAN", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
                cmd.Parameters.AddWithValue("@TOTALHUTANG", ModuleAngka.ParseDecimal(TxtTotalHutang.Text))
                cmd.Parameters.AddWithValue("@NOMINALBAYAR", ModuleAngka.ParseDecimal(TxtTotalBayar.Text))
                cmd.Parameters.AddWithValue("@SISAHUTANG", ModuleAngka.ParseDecimal(TxtSisaHutang.Text))
                cmd.Parameters.AddWithValue("@IDUser", FormUtama.StatusNamaUser.Text)
                cmd.Parameters.AddWithValue("@IDKomputer", FormUtama.StatusNamaPC.Text)

                ' Eksekusi query
                cmd.ExecuteNonQuery()
            End Using

            ' Update hutang supplier secara realtime
            UpdateHutangSupliyer(LblKodeSupliyer.Text, transaction)


            Dim akunTerlibat As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using cmdAkun As New MySqlCommand(
                "SELECT DISTINCT NOMOR_AKUN_D FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_D <> '' " &
                "UNION " &
                "SELECT DISTINCT NOMOR_AKUN_K FROM JurnalUmum WHERE NO_TRANSAKSI = @fk AND NOMOR_AKUN_K <> ''",
                conn, transaction)
                cmdAkun.Parameters.AddWithValue("@fk", LblNomorBayar.Text)
                Using rd = cmdAkun.ExecuteReader()
                    While rd.Read()
                        Dim kode As String = rd(0).ToString().Trim()
                        If kode <> "" Then akunTerlibat.Add(kode)
                    End While
                End Using
            End Using
            For Each kodeAkun As String In akunTerlibat
                UpdateSaldoAkun(kodeAkun, transaction)
            Next

            ' Commit transaksi jika semua berhasil
            transaction.Commit()

            ' Audit jurnal keseimbangan
            CatatJurnalTidakSeimbang(LblNomorBayar.Text, totalNominalBayarHutang, totalNominalBayarHutang, "Bayar Hutang",
                {"BayarHutang"})

            Dim noBayar As String = LblNomorBayar.Text
            Kondisiawal()

            ' Cetak setelah simpan
            Try
                Select Case BacaPengaturanPrinter("BayarHutang", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakBayarHutang(noBayar)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak bukti bayar hutang?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakBayarHutang(noBayar)
                        End If
                End Select
            Catch ex As Exception
                MessageBox.Show("Gagal mencetak bukti bayar hutang." & vbCrLf & "Detail: " & ex.Message,
                                "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try

        Catch ex As Exception
            If transaction IsNot Nothing Then
                Try
                    transaction.Rollback()
                Catch rollEx As Exception
                    ' Log rollback error jika perlu, jangan override exception handling
                End Try
            End If

            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub LakukanCetakBayarHutang(noBayar As String)
        If BacaPengaturanPrinter("BayarHutang", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterBayarHutang.TanyaPilihPrinterBayarHutang(noBayar)
        Else
            ModulePrinterBayarHutang.CetakBayarHutang(noBayar)
        End If
    End Sub

    Private Sub BtnKeluar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnKeluarForm.Click
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
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F8
                BtnBayar.PerformClick()
            Case Keys.Escape
                If PanelDatagridview.Visible = True Then
                    PanelDatagridview.Visible = False
                Else
                    BtnKeluarForm.PerformClick()
                End If

        End Select
    End Sub

    Private Sub BtnHide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHide.Click
        PanelDatagridview.Visible = False
    End Sub
    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "BayarHutang"}
            frm.ShowDialog()
        End Using
        MuatSemuaPengaturan()
    End Sub

    ' ============================================
    ' FUNGSI: TAMPILKAN BANTUAN SHORTCUT
    ' ============================================
    Private Sub TampilkanBantuan()
        Dim helpText As String = "SHORTCUT KEYBOARD:" & vbCrLf & vbCrLf &
                           "F1      : Tampilkan bantuan ini" & vbCrLf &
                           "F8      : Bayar hutang" & vbCrLf &
                           "ESC     : Tutup panel detail / Keluar"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

End Class
