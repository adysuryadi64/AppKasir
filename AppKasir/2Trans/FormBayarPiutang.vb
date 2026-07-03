Public Class FormBayarPiutang
    Private Sub FormBayarPiutang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Setting dibaca langsung dari ModulHakAkses property
        Kondisiawal()
    End Sub


    Private Sub GenerateNomorBayarHutang()
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "BP")
            cmd.Parameters.AddWithValue("@tgl", DtpTanggal.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "piutang")
            cmd.Parameters.AddWithValue("@kolom", "ID_BAYAR_PIUTANG")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            LblNomorBayar.Text = pNomor.Value?.ToString()
        End Using
    End Sub

    Private Sub Kondisiawal()
        PanelGrid.Visible = False
        TxtTotalPiutang.Text = 0
        TxtTotalBayar.Text = 0
        TxtSisaPiutang.Text = 0
        LblTotalPiutang.Text = "0"
        LblTotalBayar.Text = "0"
        LblSisaPiutang.Text = "0"
        CmbPelanggan.Text = ""
        LblKodePelanggan.Text = ""
        DgvData.Rows.Clear()

        IsiComboBoxAkun(CmbRekening, "KAS", "BANK", "EKUITAS")
        CmbRekening.SelectedItem = nama_rek_Bayar_Piutang

        ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
        DtpTanggal.Format = DateTimePickerFormat.Custom
        DtpTanggal.CustomFormat = "dd/MM/yyyy HH:mm:ss"
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
            .Columns(10).DefaultCellStyle.Format = "dd/MM/yyyy"
        End With
        ModuleAngka.TerapkanFormatKolomAngka(DgvData, "GRAND_TOTAL_STL_PAJAK", "BAYAR", "NILAI_RETUR", "SISA_TAGIHAN", "Bayar")

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
        If e.RowIndex >= 0 AndAlso DgvData.Columns(e.ColumnIndex).Name = "View" AndAlso Not IsDBNull(DgvData.Rows(e.RowIndex).Cells("IDPENJUALAN").Value) Then
            ' Ambil nilai dari kolom "kode" pada baris yang dipilih
            Dim fakturBeli As String = DgvData.Rows(e.RowIndex).Cells("IDPENJUALAN").Value.ToString()
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
                ' Rename columns
                For Each column As DataGridViewColumn In .Columns
                    If columnNames.ContainsKey(column.Name) Then
                        column.HeaderText = columnNames(column.Name)
                    End If
                Next

                ' Set header style

                ' Set alternating row style

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
            PanelGrid.Visible = True
        End If
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
        LblTotalPiutang.Text = TotalHUtang.ToString("#,0.####")
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

                ' Jika dicentang, isi kolom "Bayar" dengan nilai dari kolom "NOMINALPIUTANG"
                If isChecked Then
                    currentRow.Cells("Bayar").Value = currentRow.Cells("NOMINALPIUTANG").Value
                    currentRow.Cells("Bayar").ReadOnly = False
                Else
                    ' Jika ceklis dihapus, kosongkan kolom "Bayar"
                    currentRow.Cells("Bayar").Value = 0
                    currentRow.Cells("Bayar").ReadOnly = True
                End If

                ' Terapkan format #,0.#### untuk kolom "Bayar" (angka dengan empat desimal jika ada, tanpa trailing zero)
                currentRow.Cells("Bayar").Style.Format = "#,0.####"
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
            ' Terapkan format #,0.#### untuk kolom "Bayar"
            DgvData.Rows(e.RowIndex).Cells("Bayar").Style.Format = "#,0.####"
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
        Dim totalHutang As Decimal = ModuleAngka.ParseDecimal(TxtTotalPiutang.Text)
        Dim totalBayar As Decimal = ModuleAngka.ParseDecimal(TxtTotalBayar.Text)

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
        If ModuleAngka.ParseDecimal(TxtTotalPiutang.Text) = 0 Then
            MessageBox.Show("Tidak ada Piutang yang harus di bayar.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        ' Pengecekan Total Bayar
        If ModuleAngka.ParseDecimal(TxtTotalBayar.Text) = 0 Then
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
                ' Mewarnai sel kolom 11 dengan warna error
                row.Cells(11).Style.BackColor = ModuleTheme.C(ModuleTheme.L_DgvRowError, ModuleTheme.D_DgvRowError)
                DgvData.ClearSelection()
                adaKesalahan = True
            Else
                ' Reset warna jika tidak ada kesalahan
                row.Cells(11).Style.BackColor = Color.Empty
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

        If Not ModulHakAkses.SettingIzinkanTanggalLampau Then
            ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
            GenerateNomorBayarHutang()
        End If


        Dim transaction As MySqlTransaction = Nothing

        Try
            transaction = conn.BeginTransaction()

            Dim totalNominalBayarPiutang As Decimal = 0D
            For baris As Integer = 0 To DgvData.Rows.Count - 1
                If DgvData.Rows(baris).IsNewRow Then Continue For
                Dim checkVal = DgvData.Rows(baris).Cells(0).Value
                If checkVal Is Nothing OrElse IsDBNull(checkVal) OrElse Not Convert.ToBoolean(checkVal) Then
                    Continue For
                End If

                Dim Status As String
                Dim hutang As Decimal = If(IsDBNull(DgvData.Rows(baris).Cells(9).Value) OrElse DgvData.Rows(baris).Cells(9).Value Is Nothing, 0, ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(9).Value))
                Dim bayar As Decimal = If(IsDBNull(DgvData.Rows(baris).Cells(11).Value) OrElse DgvData.Rows(baris).Cells(11).Value Is Nothing, 0, ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(11).Value))

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
                    cmdJurnal.Parameters.AddWithValue("@JENIS_TRANSAKSI", "BAYAR PIUTANG")
                    cmdJurnal.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text) ' Lokasi transaksi
                    cmdJurnal.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text) ' User ID
                    cmdJurnal.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text) ' ID Komputer

                    ' Eksekusi query
                    cmdJurnal.ExecuteNonQuery()
                End Using



                Using cmdPiutangDetail As New MySqlCommand("INSERT INTO Piutang_Detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_JUAL, KODE, NAMA, JENIS, TANGGAL_JUAL, PIUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
                                 "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_JUAL, @KODE, @NAMA, 'BAYAR', @TANGGAL_JUAL, @PIUTANG, @DIBAYAR, @RETUR, @HUTANG, @JATUH_TEMPO, @PEMBAYARAN, @STATUS, @ID_USER, @ID_KOMPUTER)", conn, transaction)

                    ' Set nilai untuk parameter
                    cmdPiutangDetail.Parameters.AddWithValue("@ID_BAYAR", LblNomorBayar.Text)
                    cmdPiutangDetail.Parameters.AddWithValue("@TANGGAL_BAYAR", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdPiutangDetail.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
                    cmdPiutangDetail.Parameters.AddWithValue("@ID_JUAL", DgvData.Rows(baris).Cells(1).Value)
                    cmdPiutangDetail.Parameters.AddWithValue("@KODE", DgvData.Rows(baris).Cells(2).Value)
                    cmdPiutangDetail.Parameters.AddWithValue("@NAMA", DgvData.Rows(baris).Cells(3).Value)

                    ' Jika nilai TANGGAL_JUAL adalah DateTime, format nilainya. Jika tidak, gunakan nilai default
                    Dim tanggalJual As DateTime
                    Dim cellTglJual = DgvData.Rows(baris).Cells(5).Value
                    If cellTglJual IsNot Nothing AndAlso Not IsDBNull(cellTglJual) AndAlso DateTime.TryParse(Convert.ToString(cellTglJual), tanggalJual) Then
                        cmdPiutangDetail.Parameters.AddWithValue("@TANGGAL_JUAL", tanggalJual.ToString("yyyy-MM-dd HH:mm:ss"))
                    Else
                        cmdPiutangDetail.Parameters.AddWithValue("@TANGGAL_JUAL", DBNull.Value)
                    End If

                    cmdPiutangDetail.Parameters.AddWithValue("@PIUTANG", ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(6).Value))
                    cmdPiutangDetail.Parameters.AddWithValue("@DIBAYAR", ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(7).Value))
                    cmdPiutangDetail.Parameters.AddWithValue("@RETUR", ModuleAngka.ParseDecimal(DgvData.Rows(baris).Cells(8).Value))
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
                    cmdPiutangDetail.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                    cmdPiutangDetail.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

                    ' Eksekusi query
                    cmdPiutangDetail.ExecuteNonQuery()

                    ' Perbarui baris JUAL di piutang_detail — kurangi sisa piutang
                    Using cmdUpdateTimbul As New MySqlCommand(
                        "UPDATE piutang_detail SET " &
                        "HUTANG = HUTANG - @BAYAR, " &
                        "DIBAYAR = DIBAYAR + @BAYAR, " &
                        "STATUS = CASE WHEN (HUTANG - @BAYAR) <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END " &
                        "WHERE ID_JUAL = @ID_JUAL AND JENIS = 'JUAL'", conn, transaction)
                        cmdUpdateTimbul.Parameters.AddWithValue("@BAYAR", bayar)
                        cmdUpdateTimbul.Parameters.AddWithValue("@ID_JUAL", DgvData.Rows(baris).Cells("IDPENJUALAN").Value)
                        cmdUpdateTimbul.ExecuteNonQuery()
                        ' Jika baris JUAL tidak ditemukan (faktur lama sebelum migrasi), tidak error — lanjutkan
                    End Using
                End Using

                totalNominalBayarPiutang += bayar

            Next

            Dim query As String = "INSERT INTO Piutang (ID_BAYAR_PIUTANG, KODE_PELANGGAN, NAMA_PELANGGAN, TGL_BAYAR, TOTAL_PIUTANG, NOMINAL_BAYAR, SISA_PIUTANG, LOKASI, ID_USER_BAYAR, ID_KOMPUTER_BAYAR) " &
                       "VALUES (@ID_BAYAR_PIUTANG, @KODE_PELANGGAN, @NAMA_PELANGGAN, @TGL_BAYAR, @TOTAL_PIUTANG, @NOMINAL_BAYAR, @SISA_PIUTANG, @LOKASI, @ID_USER_BAYAR, @ID_KOMPUTER_BAYAR)"

            Using cmd As New MySqlCommand(query, conn, transaction)
                ' Menambahkan parameter untuk query
                cmd.Parameters.AddWithValue("@ID_BAYAR_PIUTANG", LblNomorBayar.Text)
                cmd.Parameters.AddWithValue("@KODE_PELANGGAN", LblKodePelanggan.Text)
                cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", CmbPelanggan.Text)
                cmd.Parameters.AddWithValue("@TGL_BAYAR", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TOTAL_PIUTANG", ModuleAngka.ParseDecimal(TxtTotalPiutang.Text))
                cmd.Parameters.AddWithValue("@NOMINAL_BAYAR", ModuleAngka.ParseDecimal(TxtTotalBayar.Text))
                cmd.Parameters.AddWithValue("@SISA_PIUTANG", ModuleAngka.ParseDecimal(TxtSisaPiutang.Text))
                cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
                cmd.Parameters.AddWithValue("@ID_USER_BAYAR", FormUtama.StatusNamaUser.Text)
                cmd.Parameters.AddWithValue("@ID_KOMPUTER_BAYAR", FormUtama.StatusNamaPC.Text)

                ' Eksekusi query
                cmd.ExecuteNonQuery()
            End Using

            ' Update piutang pelanggan secara realtime
            UpdatePiutangPelanggan(LblKodePelanggan.Text, transaction)


            ' Update saldo akun — incremental delta
            UpdateSaldoAkunDeltaDariFaktur(LblNomorBayar.Text, transaction)

            transaction.Commit()

            ' Audit jurnal keseimbangan
            CatatJurnalTidakSeimbang(LblNomorBayar.Text, totalNominalBayarPiutang, totalNominalBayarPiutang, "Bayar Piutang",
                {"BayarPiutang"})

            Dim noBayar As String = LblNomorBayar.Text
            Kondisiawal()

            ' Cetak setelah simpan
            Try
                Select Case BacaPengaturanPrinter("BayarPiutang", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakBayarPiutang(noBayar)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak bukti bayar piutang?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakBayarPiutang(noBayar)
                        End If
                End Select
            Catch ex As Exception
                MessageBox.Show("Gagal mencetak bukti bayar piutang." & vbCrLf & "Detail: " & ex.Message,
                                "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        Catch ex As Exception
            transaction.Rollback()

            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub


    Private Sub LakukanCetakBayarPiutang(noBayar As String)
        If BacaPengaturanPrinter("BayarPiutang", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterBayarPiutang.TanyaPilihPrinterBayarPiutang(noBayar)
        Else
            ModulePrinterBayarPiutang.CetakBayarPiutang(noBayar)
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

    Private Sub FormBayarPiutang_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F8
                BtnBayar.PerformClick()
            Case Keys.Escape
                If PanelGrid.Visible = True Then
                    PanelGrid.Visible = False
                Else
                    BtnKeluarForm.PerformClick()
                End If
        End Select
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        PanelGrid.Visible = False
    End Sub
    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "BayarPiutang"}
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
                           "F8      : Bayar piutang" & vbCrLf &
                           "ESC     : Tutup panel detail / Keluar"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

End Class
