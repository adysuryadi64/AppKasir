Public Class FormBon

    Private Sub FormBon_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If LblJenis.Text = "BON" Then
            Dim BON As Boolean() = ModulHakAkses.BacaHakAksesDariCache("BON")
            ' Terapkan nilai hak akses ke tombol-tombol
            BtnSimpann.Visible = BON(1) ' CanAdd 
            DgvKeuangan.Columns("EDITKEUANGAN").Visible = BON(2) ' CanEdit 
            DgvKeuangan.Columns("HAPUSKEUANGAN").Visible = BON(3) ' CanDelete 
            Label5.Text = "Saldo Bon Awal :"
            Label71.Text = "Nominal Bon :"
            Label8.Text = "Saldo Bon Akhir :"
        Else
            Dim BAYAR As Boolean() = ModulHakAkses.BacaHakAksesDariCache("BAYAR")
            ' Terapkan nilai hak akses ke tombol-tombol
            BtnSimpann.Visible = BAYAR(1) ' CanAdd 
            DgvKeuangan.Columns("EDITKEUANGAN").Visible = BAYAR(2) ' CanEdit 
            DgvKeuangan.Columns("HAPUSKEUANGAN").Visible = BAYAR(3) ' CanDelete 
            Label5.Text = "Saldo Bon Awal :"
            Label71.Text = "Nominal Bayar :"
            Label8.Text = "Saldo Bon Akhir :"
        End If
        DtpTanggal.Value = DateTime.Now
        ResetControls()

    End Sub
    Private Sub ResetControls()

        GenerateNomorBon()

        ' Panggil untuk mengambil data rekening KAS dan BANK dan MODAL
        IsiComboBoxAkun(CmbRekening, "KAS", "BANK", "EKUITAS")

        AmbilDataKaryawan()

        DtpTanggal.Format = DateTimePickerFormat.Custom
        DtpTanggal.CustomFormat = "dd/MM/yyyy"

        DGVtampildatakeuangan()
        UpdateTotalBonDanTotalBayarKaryawan()

        LblKode.Text = ""
        CmbNama.SelectedIndex = -1
        CmbRekening.SelectedIndex = -1
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
        Dim cekTanggal As String = DtpTanggal.Value.ToString("yyMMdd")
        Dim UrutKOde As String = ""
        Dim cekNomor As String = "BK-" & cekTanggal

        ' Query untuk mendapatkan nomor maksimum berdasarkan format
        Using cmd As New MySqlCommand("SELECT MAX(FAKTUR) FROM Bon_karyawan WHERE FAKTUR LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")

            ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
            Dim maxKode As Object = cmd.ExecuteScalar()

            If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                Dim MaxNilaiKode As String = maxKode.ToString()
                If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "BK-" & cekTanggal Then
                    ' Hitung nomor berikutnya
                    Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                    UrutKOde = "BK-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                End If
            End If
        End Using

        ' Jika UrutKOde masih kosong, buat nomor pertama
        If String.IsNullOrEmpty(UrutKOde) Then
            UrutKOde = "BK-" & cekTanggal & "0001"
        End If

        LblNomor.Text = UrutKOde

    End Sub



    Private Sub AmbilDataKaryawan()
        CmbNama.Items.Clear()
        ' Query untuk mengambil akun KAS atau BANK
        Dim queryArmada As String = "SELECT NAMA FROM tbl_Karyawan ORDER BY NAMA ASC"
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

        dgv.Columns("NOMINAL").DefaultCellStyle.Format = "N0"
        dgv.Columns("NOMINAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        dgv.Columns("AKHIR_BON").DefaultCellStyle.Format = "N0"
        dgv.Columns("AKHIR_BON").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        ' Set header style
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

        ' Set alternating row style
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

        ' Set visual style
        dgv.BorderStyle = BorderStyle.FixedSingle
        dgv.GridColor = Color.Silver
        dgv.BackgroundColor = Color.White

        ' Enable double buffering to reduce flickering
        EnableDoubleBuffering(dgv)
    End Sub

    ' Method to enable double buffering
    Public Shared Sub EnableDoubleBuffering(ByVal dgv As DataGridView)
        dgv.GetType().InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, dgv, New Object() {True})
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

                    Dim awal As Decimal
                    If Decimal.TryParse(DgvKeuangan.Rows(e.RowIndex).Cells("AWAL_BON").Value.ToString(), awal) Then
                        LblSaldoBon.Text = awal.ToString("N0")
                    Else
                        LblSaldoBon.Text = "0"
                    End If

                    Dim nominal As Decimal
                    If Decimal.TryParse(DgvKeuangan.Rows(e.RowIndex).Cells("NOMINAL").Value.ToString(), nominal) Then
                        TxtNominal.Text = nominal.ToString("N0")
                    Else
                        TxtNominal.Text = "0"
                    End If
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
                        Dim nominal As Decimal
                        Decimal.TryParse(DgvKeuangan.Rows(e.RowIndex).Cells("NOMINAL").Value.ToString(), nominal)

                        Using transaction As MySqlTransaction = conn.BeginTransaction()
                            Try
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
                Dim Nominal As Decimal

                If Not Decimal.TryParse(TxtNominal.Text.Replace(".", ""), Nominal) Then
                    Nominal = 0D
                End If

                If BtnSimpann.Text = "EDIT (F8)" Then
                    HapusUntukEdit(transaction, Nominal)
                End If

                InsertBonKaryawan(transaction, Nominal)
                Simpanjurnal(transaction, Nominal)

                transaction.Commit()


                ResetControls()

            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

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

            Dim sisaBon As Decimal = If(Not Decimal.TryParse(LblSisaBon.Text, sisaBon), 0, sisaBon)
            If sisaBon < 0 Then
                MessageBox.Show("Pembayaran bon lebih besar dari pada nominal bon", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                TxtNominal.Focus()
                Return False
            End If
        End If

        Return True
    End Function

    Private Sub HapusUntukEdit(ByVal transaction As MySqlTransaction, ByVal Nominal As Decimal)

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
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmd.Parameters.AddWithValue("@JENIS", LblJenis.Text)
            cmd.Parameters.AddWithValue("@KODE", LblKode.Text)
            cmd.Parameters.AddWithValue("@NAMA", CmbNama.Text)
            cmd.Parameters.AddWithValue("@KODE_REK", LblRekening.Text)
            cmd.Parameters.AddWithValue("@NAMA_REK", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@AWAL_BON", Decimal.Parse(LblSaldoBon.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@NOMINAL", Nominal)
            cmd.Parameters.AddWithValue("@AKHIR_BON", Decimal.Parse(LblSisaBon.Text.Replace(".", "")))
            cmd.Parameters.AddWithValue("@KETERANGAN", TxtKeterangan.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

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
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Bon")
            Else
                cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Bayar bon")
            End If

            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

            cmd.ExecuteNonQuery()
        End Using
    End Sub



    Private Sub Label3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblSaldoBon.TextChanged
        Dim label3Value As Decimal
        Dim txtNominalKeuanganValue As Decimal

        If Not Decimal.TryParse(LblSaldoBon.Text, label3Value) Then
            label3Value = 0
        End If

        If Not Decimal.TryParse(TxtNominal.Text, txtNominalKeuanganValue) Then
            txtNominalKeuanganValue = 0
        End If

        If LblJenis.Text = "BON" Then
            LblSisaBon.Text = (label3Value + txtNominalKeuanganValue).ToString("N0")
        Else
            LblSisaBon.Text = (label3Value - txtNominalKeuanganValue).ToString("N0")
        End If
    End Sub

    Private Sub TxtNominalKeuangan_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtNominal.TextChanged
        Dim label3Value As Decimal
        Dim txtNominalKeuanganValue As Decimal

        If Not Decimal.TryParse(LblSaldoBon.Text, label3Value) Then
            label3Value = 0
        End If

        If Not Decimal.TryParse(TxtNominal.Text, txtNominalKeuanganValue) Then
            txtNominalKeuanganValue = 0
        End If

        LblNominal.Text = "Rp. " & txtNominalKeuanganValue.ToString("N0")

        If LblJenis.Text = "BON" Then
            LblSisaBon.Text = (label3Value + txtNominalKeuanganValue).ToString("N0")
        Else
            LblSisaBon.Text = (label3Value - txtNominalKeuanganValue).ToString("N0")
        End If
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub


End Class