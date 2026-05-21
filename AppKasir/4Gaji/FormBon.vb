Public Class FormBon

    Private Sub FormBon_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        If LblJenis.Text = "BON" Then
            Dim BON As Boolean() = ModulHakAkses.BacaHakAksesDariCache("BON")
            BtnSimpann.Visible = BON(1)
            DgvKeuangan.Columns("EDITKEUANGAN").Visible = BON(2)
            DgvKeuangan.Columns("HAPUSKEUANGAN").Visible = BON(3)
            Label5.Text = "Saldo Bon Awal :"
            Label71.Text = "Nominal Bon :"
            Label8.Text = "Saldo Bon Akhir :"
        Else
            Dim BAYAR As Boolean() = ModulHakAkses.BacaHakAksesDariCache("BAYAR")
            BtnSimpann.Visible = BAYAR(1)
            DgvKeuangan.Columns("EDITKEUANGAN").Visible = BAYAR(2)
            DgvKeuangan.Columns("HAPUSKEUANGAN").Visible = BAYAR(3)
            Label5.Text = "Saldo Bon Awal :"
            Label71.Text = "Nominal Bayar :"
            Label8.Text = "Saldo Bon Akhir :"
        End If
        ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
        DtpTanggal.Enabled = True  ' 4Gaji: DTP selalu aktif, abaikan setting izin lampau
        ResetControls()
        CmbPilihCetak.Text = BacaPengaturanPrinter("BonKaryawan", "CetakOtomatis", "IYA")
        CmbProsesCetak.Text = BacaPengaturanPrinter("BonKaryawan", "PilihPrinter", "LANGSUNG CETAK")
    End Sub
    Private Sub ResetControls()

        GenerateNomorBon()

        ' Panggil untuk mengambil data rekening KAS dan BANK dan MODAL
        IsiComboBoxAkun(CmbRekening, "KAS", "BANK", "EKUITAS")

        AmbilDataKaryawan()

        ModulHakAkses.ResetDTPKeTanggalHariIni(DtpTanggal)
        DtpTanggal.Enabled = True  ' 4Gaji: DTP selalu aktif, abaikan setting izin lampau
        DtpTanggal.Format = DateTimePickerFormat.Custom
        DtpTanggal.CustomFormat = "dd/MM/yyyy"

        DGVtampildatakeuangan()
        UpdateTotalBonDanTotalBayarKaryawan()

        LblKode.Text = ""
        CmbNama.SelectedIndex = -1
        CmbRekening.SelectedItem = nama_rek_Bon_Karyawan
        LblSaldoBon.Text = ""
        TxtNominal.Text = ""
        LblKode.Text = ""
        LblRekening.Text = ""
        LblNominal.Text = "Rp. 0"
        LblSisaBon.Text = ""
        TxtKeterangan.Clear()

        BtnSimpann.Text = "SIMPAN (F8)"
    End Sub

    Private Sub GenerateNomorBon()
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "BK")
            cmd.Parameters.AddWithValue("@tgl", DtpTanggal.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "bon_karyawan")
            cmd.Parameters.AddWithValue("@kolom", "FAKTUR")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            LblNomor.Text = pNomor.Value?.ToString()
        End Using
    End Sub



    Private Sub AmbilDataKaryawan()
        CmbNama.Items.Clear()
        ' Query untuk mengambil akun KAS atau BANK
        Dim queryArmada As String = "SELECT NAMA FROM tbl_Karyawan WHERE Status = 'Aktif' ORDER BY NAMA ASC"
        Using cmd As New MySqlCommand(queryArmada, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        CmbNama.Items.Add(rd("NAMA").ToString())
                    End While
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbRekening_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekening.SelectedIndexChanged
        Dim namaAkunD As String = CmbRekening.Text

        Dim sql As String = "SELECT KODE_AKUN FROM tbl_datareferensi WHERE NAMA_AKUN = @NAMA_AKUN"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NAMA_AKUN", namaAkunD)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblRekening.Text = reader("KODE_AKUN").ToString()
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbNama_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbNama.SelectedIndexChanged
        Dim sql As String = "SELECT KODE, SALDOAKHIR FROM tbl_karyawan WHERE NAMA = @NAMA"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NAMA", CmbNama.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblKode.Text = reader("KODE").ToString()

                    ' Check if SaldoAkhir is DBNull and set it to 0 if true
                    Dim saldoAkhir As Decimal
                    If IsDBNull(reader("SALDOAKHIR")) Then
                        saldoAkhir = 0
                    Else
                        saldoAkhir = Convert.ToDecimal(reader("SALDOAKHIR"))
                    End If

                    LblSaldoBon.Text = saldoAkhir.ToString("N0")
                Else
                    LblKode.Text = ""
                    LblSaldoBon.Text = "0"
                End If
            End Using
        End Using
    End Sub

    Private Sub DtpTanggal_ValueChanged(sender As Object, e As EventArgs) Handles DtpTanggal.ValueChanged
        DGVtampildatakeuangan()
    End Sub

    Public Sub DGVtampildatakeuangan()
        Dim dt As New DataTable()
        Dim tanggalAwal As Date = DtpTanggal.Value.Date
        Dim tanggalAkhir As Date = DtpTanggal.Value.Date.AddDays(1).AddTicks(-1)
        Dim totalNominal As Decimal = 0

        ' Membuat query dengan parameter
        Using cmd As New MySqlCommand("SELECT FAKTUR, TANGGAL, KODE, NAMA, KODE_REK, NAMA_REK, AWAL_BON, NOMINAL, AKHIR_BON, KETERANGAN, ID_USER " &
                                    "FROM Bon_karyawan " &
                                    "WHERE TANGGAL >= @TanggalAwal AND TANGGAL <= @TanggalAkhir " &
                                    "AND JENIS LIKE @Jenis " &
                                    "AND KETERANGAN NOT LIKE 'POTONG GAJI' " &
                                    "ORDER BY FAKTUR", conn)

            ' Menambahkan parameter
            cmd.Parameters.AddWithValue("@TanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@TanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@Jenis", LblJenis.Text)

            ' Menggunakan MySqlDataAdapter untuk mengisi DataTable
            Using da As New MySqlDataAdapter(cmd)
                da.Fill(dt) ' Mengisi DataTable dengan hasil query
            End Using
        End Using

        ' Hitung total nominal dari DataTable
        For Each row As DataRow In dt.Rows
            totalNominal += CDec(row("NOMINAL"))
        Next

        ' Set DataTable sebagai DataSource dari DataGridView
        DgvKeuangan.DataSource = dt

        ' Konfigurasi kolom DataGridView
        ConfigureDgvColumns(DgvKeuangan)

        ' Menampilkan keterangan dan total nominal
        LblTotalNominal.Text = $"Daftar rincian tanggal : {DtpTanggal.Value:dd/MM/yyyy} Total Nominal Rp. {totalNominal:N0}"

    End Sub

    Private Sub ConfigureDgvColumns(ByVal dgv As DataGridView)
        ' Menyembunyikan kolom yang tidak diperlukan
        dgv.Columns("KODE").Visible = False
        dgv.Columns("KODE_REK").Visible = False
        dgv.Columns("AWAL_BON").Visible = False
        dgv.Columns("AKHIR_BON").Visible = False

        ' Ubah nama header kolom "AKHIR_BON" menjadi "SALDO"
        dgv.Columns("AKHIR_BON").HeaderText = "SALDO"

        ' Format tanggal dan nominal
        dgv.Columns("TANGGAL").DefaultCellStyle.Format = "dd/MM/yyyy"

        ModuleAngka.TerapkanFormatKolomAngka(dgv, "NOMINAL", "AKHIR_BON")

        ' Set header style

        ' Set alternating row style

        ' Set visual style
        dgv.BorderStyle = BorderStyle.FixedSingle

        ' Enable double buffering to reduce flickering
        ModuleTheme.ApplyThemeDataGridView(dgv)
    End Sub


    Private Sub DgvKeuangan_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DgvKeuangan.CellContentClick
        If e.RowIndex >= 0 Then
            ' EDIT
            If e.ColumnIndex = DgvKeuangan.Columns("EDITKEUANGAN").Index Then
                ' Pastikan kolom NOTA tidak kosong sebelum melanjutkan
                Dim notaValue As Object = DgvKeuangan.Rows(e.RowIndex).Cells("FAKTUR").Value
                If notaValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(notaValue.ToString()) Then
                    ' Transfer isi dari baris yang dipilih ke label dan textbox
                    LblNomor.Text = notaValue.ToString()
                    DtpTanggal.Value = CDate(DgvKeuangan.Rows(e.RowIndex).Cells("TANGGAL").Value)
                    LblKode.Text = DgvKeuangan.Rows(e.RowIndex).Cells("KODE").Value.ToString()
                    CmbNama.Text = DgvKeuangan.Rows(e.RowIndex).Cells("NAMA").Value.ToString()
                    LblRekening.Text = DgvKeuangan.Rows(e.RowIndex).Cells("KODE_REK").Value.ToString()
                    CmbRekening.Text = DgvKeuangan.Rows(e.RowIndex).Cells("NAMA_REK").Value.ToString()

                    Dim awal As Decimal = ModuleAngka.ParseDecimal(DgvKeuangan.Rows(e.RowIndex).Cells("AWAL_BON").Value)
                    LblSaldoBon.Text = ModuleAngka.FormatRupiah(awal)

                    Dim nominal As Decimal = ModuleAngka.ParseDecimal(DgvKeuangan.Rows(e.RowIndex).Cells("NOMINAL").Value)
                    TxtNominal.Text = nominal.ToString()
                    TxtKeterangan.Text = DgvKeuangan.Rows(e.RowIndex).Cells("KETERANGAN").Value.ToString()
                    BtnSimpann.Text = "EDIT (F8)"
                End If
            End If

            ' HAPUS
            If e.ColumnIndex = DgvKeuangan.Columns("HAPUSKEUANGAN").Index Then
                ' Pastikan kolom NOTA tidak kosong sebelum melanjutkan
                Dim notaValue As Object = DgvKeuangan.Rows(e.RowIndex).Cells("FAKTUR").Value
                If notaValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(notaValue.ToString()) Then
                    Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin akan menghapus data ini?", "Hapus Data", MessageBoxButtons.YesNo)
                    If result = DialogResult.Yes Then
                        Dim kodeTransaksi As String = notaValue.ToString()
                        Dim nominal As Decimal = ModuleAngka.ParseDecimal(DgvKeuangan.Rows(e.RowIndex).Cells("NOMINAL").Value)

                        Using transaction As MySqlTransaction = conn.BeginTransaction()
                            Try
                                ' ========================================
                                ' STEP 1: REVERSAL saldo akun SEBELUM DELETE JurnalUmum
                                ' ========================================
                                ReversalSaldoAkunDariFaktur(kodeTransaksi, transaction)
                                ' ========================================
                                ' START: Audit Trail - Hapus Bon Karyawan
                                ' ========================================
                                Dim sbSnapshot As New System.Text.StringBuilder()
                                Try
                                    Using cmdSnap As New MySqlCommand(
                                        "SELECT FAKTUR, TANGGAL, KODE, NAMA, KODE_REK, NAMA_REK, AWAL_BON, NOMINAL, AKHIR_BON, KETERANGAN " &
                                        "FROM Bon_karyawan WHERE FAKTUR = @f LIMIT 1", conn, transaction)
                                        cmdSnap.Parameters.AddWithValue("@f", kodeTransaksi)
                                        Using rdSnap = cmdSnap.ExecuteReader()
                                            If rdSnap.Read() Then
                                                sbSnapshot.AppendLine($"Faktur: {rdSnap("FAKTUR")}")
                                                sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(rdSnap("TANGGAL")).ToString("dd/MM/yyyy HH:mm:ss")}")
                                                sbSnapshot.AppendLine($"Kode Karyawan: {rdSnap("KODE")}")
                                                sbSnapshot.AppendLine($"Nama Karyawan: {rdSnap("NAMA")}")
                                                sbSnapshot.AppendLine($"Kode Rekening: {rdSnap("KODE_REK")}")
                                                sbSnapshot.AppendLine($"Nama Rekening: {rdSnap("NAMA_REK")}")
                                                sbSnapshot.AppendLine($"Saldo Awal: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("AWAL_BON")))}")
                                                sbSnapshot.AppendLine($"Nominal: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("NOMINAL")))}")
                                                sbSnapshot.AppendLine($"Saldo Akhir: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("AKHIR_BON")))}")
                                                sbSnapshot.AppendLine($"Keterangan: {rdSnap("KETERANGAN")}")
                                            End If
                                        End Using
                                    End Using
                                Catch
                                    sbSnapshot.AppendLine("Gagal baca data sebelum hapus")
                                End Try
                                ModuleAuditTrail.CatatAuditMaster("BON:" & kodeTransaksi, "HAPUS", "Bon Karyawan", sbSnapshot.ToString(), trans:=transaction)
                                ' ========================================
                                ' END: Audit Trail - Hapus Bon Karyawan
                                ' ========================================

                                Dim deleteQueries As String() = {
                                    "DELETE FROM Bon_karyawan WHERE FAKTUR = @FAKTUR",
                                    "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FAKTUR"
                                }

                                For Each query As String In deleteQueries
                                    Using cmd As New MySqlCommand(query, conn, transaction)
                                        If query.Contains("Bon_karyawan") Then
                                            cmd.Parameters.AddWithValue("@FAKTUR", kodeTransaksi)
                                        Else
                                            cmd.Parameters.AddWithValue("@FAKTUR", kodeTransaksi)
                                        End If
                                        cmd.ExecuteNonQuery()
                                    End Using
                                Next

                                Dim updateQuery As String
                                If LblJenis.Text = "BON" Then
                                    updateQuery = "UPDATE tbl_karyawan SET TotalBon = TotalBon - ? WHERE Kode = ?"
                                Else
                                    updateQuery = "UPDATE tbl_karyawan SET TotalBayar = TotalBayar - ? WHERE Kode = ?"
                                End If

                                Using cmdUpdate As New MySqlCommand(updateQuery, conn, transaction)
                                    cmdUpdate.Parameters.AddWithValue("@Nominal", nominal)
                                    cmdUpdate.Parameters.AddWithValue("@Kode", kodeTransaksi)
                                    cmdUpdate.ExecuteNonQuery()
                                End Using

                                ' Update saldo bon karyawan secara realtime
                                Dim kodeKaryawan As String = DgvKeuangan.Rows(e.RowIndex).Cells("KODE").Value.ToString()
                                UpdateBonKaryawan(kodeKaryawan, transaction)

                                ' Update saldo akun — sudah dilakukan sebelum DELETE di atas

                                ' Commit transaksi
                                transaction.Commit()
                                ' Reset kontrol atau refresh DataGridView setelah operasi berhasil
                                ResetControls()
                            Catch ex As Exception
                                ' Rollback transaksi jika terjadi kesalahan
                                transaction.Rollback()
                                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End Try
                        End Using
                    End If
                End If
            End If
        End If
    End Sub



    Private Sub BtnSimpann_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpann.Click
        If ValidateInputs() Then
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                Dim Nominal As Decimal = ModuleAngka.ParseDecimal(TxtNominal.Text)

                If BtnSimpann.Text = "EDIT (F8)" Then
                    ' ========================================
                    ' START: Audit Trail - Edit Bon Karyawan
                    ' ========================================
                    Dim sbSnapshot As New System.Text.StringBuilder()
                    Try
                        Using cmdSnap As New MySqlCommand(
                            "SELECT FAKTUR, TANGGAL, KODE, NAMA, KODE_REK, NAMA_REK, AWAL_BON, NOMINAL, AKHIR_BON, KETERANGAN " &
                            "FROM Bon_karyawan WHERE FAKTUR = @f LIMIT 1", conn, transaction)
                            cmdSnap.Parameters.AddWithValue("@f", LblNomor.Text)
                            Using rdSnap = cmdSnap.ExecuteReader()
                                If rdSnap.Read() Then
                                    sbSnapshot.AppendLine($"Faktur: {rdSnap("FAKTUR")}")
                                    sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(rdSnap("TANGGAL")).ToString("dd/MM/yyyy HH:mm:ss")}")
                                    sbSnapshot.AppendLine($"Kode Karyawan: {rdSnap("KODE")}")
                                    sbSnapshot.AppendLine($"Nama Karyawan: {rdSnap("NAMA")}")
                                    sbSnapshot.AppendLine($"Kode Rekening: {rdSnap("KODE_REK")}")
                                    sbSnapshot.AppendLine($"Nama Rekening: {rdSnap("NAMA_REK")}")
                                    sbSnapshot.AppendLine($"Saldo Awal: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("AWAL_BON")))}")
                                    sbSnapshot.AppendLine($"Nominal: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("NOMINAL")))}")
                                    sbSnapshot.AppendLine($"Saldo Akhir: {ModuleAngka.FormatRupiah(ModuleAngka.ParseDecimal(rdSnap("AKHIR_BON")))}")
                                    sbSnapshot.AppendLine($"Keterangan: {rdSnap("KETERANGAN")}")
                                End If
                            End Using
                        End Using
                    Catch
                        sbSnapshot.AppendLine("Gagal baca data sebelum edit")
                    End Try
                    ModuleAuditTrail.CatatAuditMaster("BON:" & LblNomor.Text, "EDIT", "Bon Karyawan", sbSnapshot.ToString(), trans:=transaction)
                    ' ========================================
                    ' END: Audit Trail - Edit Bon Karyawan
                    ' ========================================
                    HapusUntukEdit(transaction, Nominal)
                End If

                InsertBonKaryawan(transaction, Nominal)
                Simpanjurnal(transaction, Nominal)

                ' Update saldo bon karyawan secara realtime
                UpdateBonKaryawan(LblKode.Text, transaction)

                ' ========================================
                ' STEP 2: UPDATE saldo akun — incremental delta
                ' ========================================
                UpdateSaldoAkunDeltaDariFaktur(LblNomor.Text, transaction)

                transaction.Commit()

                ' Audit jurnal keseimbangan
                CatatJurnalTidakSeimbang(LblNomor.Text, Nominal, Nominal, "Bon Karyawan",
                    {"Bon/Bayar"})

                Dim noBon As String = LblNomor.Text
                ResetControls()

                ' Cetak setelah simpan
                Try
                    Select Case CmbPilihCetak.Text.Trim().ToUpper()
                        Case "IYA"
                            LakukanCetakBon(noBon)
                        Case "SELALU TANYA"
                            If MessageBox.Show("Apakah Anda ingin mencetak bon karyawan?",
                                               "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                                LakukanCetakBon(noBon)
                            End If
                        Case "TAMPILKAN DI MONITOR"
                            ModulePrinterBonKaryawan.CetakBonKaryawan(noBon, "Tampilkan di Monitor")
                    End Select
                Catch ex As Exception
                    MessageBox.Show("Gagal mencetak bon karyawan." & vbCrLf & "Detail: " & ex.Message,
                                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Try

            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If
    End Sub

    Private Sub LakukanCetakBon(faktur As String)
        If CmbProsesCetak.Text = "TANYA PILIH PRINTER" Then
            ModulePrinterBonKaryawan.TanyaPilihPrinterBonKaryawan(faktur)
        Else
            ModulePrinterBonKaryawan.CetakBonKaryawan(faktur)
        End If
    End Sub

    Private Function ValidateInputs() As Boolean
        If CmbNama.SelectedIndex = -1 Then
            MessageBox.Show("Karyawan belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            CmbNama.DroppedDown = True
            CmbNama.Focus()
            Return False
        End If

        If CmbRekening.SelectedIndex = -1 Then
            MessageBox.Show("Sumber dana untuk bayar gaji belum dipilih", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            CmbRekening.DroppedDown = True
            CmbRekening.Focus()
            Return False
        End If

        ' Validasi untuk TxtPokok
        If String.IsNullOrWhiteSpace(TxtNominal.Text) OrElse TxtNominal.Text.Trim() = "0" Then
            MessageBox.Show("Nominal harus diisi dan tidak boleh 0", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            TxtNominal.Focus()
            Return False
        End If

        If LblJenis.Text = "BAYAR" Then
            If String.IsNullOrWhiteSpace(LblSaldoBon.Text) OrElse LblSaldoBon.Text.Trim() = "0" Then
                MessageBox.Show("Tidak ada bon yang harus dibayar", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                TxtNominal.Focus()
                Return False
            End If

            Dim sisaBon As Decimal = ModuleAngka.ParseDecimal(LblSisaBon.Text)
            If sisaBon < 0 Then
                MessageBox.Show("Pembayaran bon lebih besar dari pada nominal bon", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                TxtNominal.Focus()
                Return False
            End If
        End If

        Return True
    End Function

    Private Sub HapusUntukEdit(ByVal transaction As MySqlTransaction, ByVal Nominal As Decimal)
        ' Reversal saldo akun SEBELUM DELETE JurnalUmum
        ReversalSaldoAkunDariFaktur(LblNomor.Text, transaction)

        Dim deleteQueries As String() = {
               "DELETE FROM Bon_karyawan WHERE FAKTUR = @FAKTUR",
               "DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @FAKTUR"
           }

        For Each query As String In deleteQueries
            Using cmd As New MySqlCommand(query, conn, transaction)
                cmd.Parameters.AddWithValue("@FAKTUR", LblNomor.Text)
                cmd.ExecuteNonQuery()
            End Using
        Next

        Dim updateQuery As String
        If LblJenis.Text = "BON" Then
            updateQuery = "UPDATE tbl_karyawan SET TotalBon = TotalBon - @Nominal WHERE Kode = @Kode"
        Else
            updateQuery = "UPDATE tbl_karyawan SET TotalBayar = TotalBayar - @Nominal WHERE Kode = @Kode"
        End If

        Using cmdUpdate As New MySqlCommand(updateQuery, conn, transaction)
            cmdUpdate.Parameters.AddWithValue("@Nominal", Nominal)
            cmdUpdate.Parameters.AddWithValue("@Kode", LblKode.Text)

            cmdUpdate.ExecuteNonQuery()
        End Using

    End Sub

    Private Sub InsertBonKaryawan(ByVal transaction As MySqlTransaction, ByVal Nominal As Decimal)
        ' Define the SQL Insert query
        Dim sql As String = "INSERT INTO Bon_karyawan (FAKTUR, TANGGAL, LOKASI, JENIS, KODE, NAMA, KODE_REK, NAMA_REK, AWAL_BON, NOMINAL, AKHIR_BON, KETERANGAN, ID_USER, ID_KOMPUTER) VALUES (@FAKTUR, @TANGGAL, @LOKASI, @JENIS, @KODE, @NAMA, @KODE_REK, @NAMA_REK, @AWAL_BON, @NOMINAL, @AKHIR_BON, @KETERANGAN, @ID_USER, @ID_KOMPUTER)"

        ' Create a new MySqlCommand
        Using cmd As New MySqlCommand(sql, conn, transaction)
            cmd.Parameters.AddWithValue("@FAKTUR", LblNomor.Text)
            cmd.Parameters.AddWithValue("@TANGGAL", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@JENIS", LblJenis.Text)
            cmd.Parameters.AddWithValue("@KODE", LblKode.Text)
            cmd.Parameters.AddWithValue("@NAMA", CmbNama.Text)
            cmd.Parameters.AddWithValue("@KODE_REK", LblRekening.Text)
            cmd.Parameters.AddWithValue("@NAMA_REK", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@AWAL_BON", ModuleAngka.ParseDecimal(LblSaldoBon.Text))
            cmd.Parameters.AddWithValue("@NOMINAL", Nominal)
            cmd.Parameters.AddWithValue("@AKHIR_BON", ModuleAngka.ParseDecimal(LblSisaBon.Text))
            cmd.Parameters.AddWithValue("@KETERANGAN", TxtKeterangan.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

            ' Execute the command
            cmd.ExecuteNonQuery()
        End Using

        Dim updateQuery As String
        If LblJenis.Text = "BON" Then
            updateQuery = "UPDATE tbl_karyawan SET TotalBon = TotalBon + @Nominal WHERE Kode = @Kode"
        Else
            updateQuery = "UPDATE tbl_karyawan SET TotalBayar = TotalBayar + @Nominal WHERE Kode = @Kode"
        End If

        Using cmdUpdate As New MySqlCommand(updateQuery, conn, transaction)
            cmdUpdate.Parameters.AddWithValue("@Nominal", Nominal)
            cmdUpdate.Parameters.AddWithValue("@Kode", LblKode.Text)

            cmdUpdate.ExecuteNonQuery()
        End Using

    End Sub

    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction, ByVal Nominal As Decimal)
        ' Simpan ke jurnal
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, NO_NOTA, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                        "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @NO_NOTA, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNomor.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@NO_NOTA", LblKode.Text)

            If LblJenis.Text = "BON" Then
                cmd.Parameters.AddWithValue("@URAIAN", "Bon An. " & CmbNama.Text & " " & TxtKeterangan.Text)
                cmd.Parameters.AddWithValue("@NAMA_AKUN_D", "PIUTANG KARYAWAN")
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", "01.03.002")
                cmd.Parameters.AddWithValue("@NAMA_AKUN_K", CmbRekening.Text)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", LblRekening.Text)
            Else
                cmd.Parameters.AddWithValue("@URAIAN", "Bayar bon An. " & CmbNama.Text & " " & TxtKeterangan.Text)
                cmd.Parameters.AddWithValue("@NAMA_AKUN_D", CmbRekening.Text)
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", LblRekening.Text)
                cmd.Parameters.AddWithValue("@NAMA_AKUN_K", "PIUTANG KARYAWAN")
                cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", "01.03.002")
            End If

            cmd.Parameters.AddWithValue("@NOMINAL", Nominal)

            If LblJenis.Text = "BON" Then
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "BON")
            Else
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "BAYAR BON")
            End If

            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

            cmd.ExecuteNonQuery()
        End Using

        ' Debug jurnal bon — D/K bertukar tergantung jenis BON vs BAYAR
        Debug.WriteLine("═══════════════════════════════════════════════════════")
        Debug.WriteLine("DEBUG JURNAL BON - Nomor: " & LblNomor.Text & " | Jenis: " & LblJenis.Text & " | " & CmbNama.Text)
        Debug.WriteLine("═══════════════════════════════════════════════════════")
        If LblJenis.Text = "BON" Then
            Debug.WriteLine(String.Format("{0,-4} {1,-20} {2,-30} {3,-30} {4,12:N0} {5,12:N0}", "J1", "Bon", "PIUTANG KARYAWAN [01.03.002]", CmbRekening.Text & " [" & LblRekening.Text & "]", Nominal, Nominal))
        Else
            Debug.WriteLine(String.Format("{0,-4} {1,-20} {2,-30} {3,-30} {4,12:N0} {5,12:N0}", "J1", "Bayar Bon", CmbRekening.Text & " [" & LblRekening.Text & "]", "PIUTANG KARYAWAN [01.03.002]", Nominal, Nominal))
        End If
        Debug.WriteLine("✅ JURNAL SEIMBANG - D=K=" & Nominal.ToString("N0"))
        Debug.WriteLine("═══════════════════════════════════════════════════════")
    End Sub

    Private Sub Label3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblSaldoBon.TextChanged
        Dim label3Value As Decimal = ModuleAngka.ParseDecimal(LblSaldoBon.Text)
        Dim txtNominalKeuanganValue As Decimal = ModuleAngka.ParseDecimal(TxtNominal.Text)

        If LblJenis.Text = "BON" Then
            LblSisaBon.Text = (label3Value + txtNominalKeuanganValue).ToString("N0")
        Else
            LblSisaBon.Text = (label3Value - txtNominalKeuanganValue).ToString("N0")
        End If
    End Sub

    Private Sub TxtNominalKeuangan_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtNominal.TextChanged
        Dim label3Value As Decimal = ModuleAngka.ParseDecimal(LblSaldoBon.Text)
        Dim txtNominalKeuanganValue As Decimal = ModuleAngka.ParseDecimal(TxtNominal.Text)

        LblNominal.Text = "Rp. " & ModuleAngka.FormatRupiah(txtNominalKeuanganValue)

        If LblJenis.Text = "BON" Then
            LblSisaBon.Text = (label3Value + txtNominalKeuanganValue).ToString("N0")
        Else
            LblSisaBon.Text = (label3Value - txtNominalKeuanganValue).ToString("N0")
        End If
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "Bon"}
            frm.ShowDialog()
        End Using
        MuatSemuaPengaturan()
    End Sub

End Class
