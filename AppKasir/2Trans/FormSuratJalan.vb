Public Class FormSuratJalan


    Private Sub FormSuratJalan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If LblJenisTrans.Text = "TambahSuratJalan" Then
            AmbilDataArmada()
            AmbilDataKaryawan()
            KondisiAwal()
        Else
            LoadSuratJalanDetail(LblNoNota.Text)
        End If
        PanelDataPenjualan.Visible = False
    End Sub

    Private Sub KondisiAwal()
        CmbArmada.SelectedIndex = -1
        CmbSopir.SelectedIndex = -1
        CmbHelper1.SelectedIndex = -1
        CmbHelper2.SelectedIndex = -1
        LblKodeArmada.Text = ""
        LblJenisArmada.Text = ""
        LblKodeSupir.Text = ""
        LblKodeHelper1.Text = ""
        LblKodeHelper2.Text = ""
        TxtTotalRupiah.Clear()
        TxtTotalPelanggan.Clear()

        DtpPenjualan.Value = DateTime.Now
        DtpPenjualan.Format = DateTimePickerFormat.Custom
        DtpPenjualan.CustomFormat = "dd/MM/yyyy"

        DtpSuratJalan.Value = DateTime.Now
        DtpSuratJalan.Format = DateTimePickerFormat.Custom
        DtpSuratJalan.CustomFormat = "dd/MM/yyyy HH:mm:ss"

        DGVSuratJalan.DataSource = Nothing
        DGVSuratJalan.Rows.Clear()
        DGVPenjualan.DataSource = Nothing
        DGVPenjualan.Rows.Clear()
        GenerateNomorSuratJalan()
        AmbildataPenjualan()
    End Sub

    Private Sub LoadSuratJalanDetail(ByVal nota As String)
        ' Bersihkan DataGridView sebelum mengisi data
        DGVSuratJalan.Rows.Clear()

        ' Query untuk mengambil data berdasarkan NOTA
        Dim query As String = "SELECT NOTA_BELANJA, KODE_PELANGGAN, NAMA_PELANGGAN, ALAMAT_PELANGGAN, TANGGAL_BELANJA, NILAI_BELANJA, LOKASI FROM surat_jalan_detail WHERE NOTA = @Nota"

        Try
            Using cmd As New MySqlCommand(query, conn)
                ' Parameter untuk NOTA
                cmd.Parameters.AddWithValue("@Nota", nota)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    ' Periksa apakah ada data yang ditemukan
                    If rd.HasRows Then
                        While rd.Read()
                            ' Tambahkan baris ke DataGridView
                            DGVSuratJalan.Rows.Add(
                            rd("NOTA_BELANJA").ToString(),
                            rd("KODE_PELANGGAN").ToString(),
                            rd("NAMA_PELANGGAN").ToString(),
                            rd("ALAMAT_PELANGGAN").ToString(),
                            Convert.ToDateTime(rd("TANGGAL_BELANJA")).ToString("yyyy-MM-dd HH:mm:ss"),
                            Convert.ToDecimal(rd("NILAI_BELANJA")).ToString("N2"),
                            rd("LOKASI").ToString()
                        )
                        End While
                    End If
                End Using
            End Using
            HitungTotalDataDanRupiah()
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Public Sub AmbilDataArmada()
        CmbArmada.Items.Clear()

        ' Query untuk mengambil akun KAS atau BANK
        Dim queryArmada As String = "SELECT NOPOL FROM tbl_Armada ORDER BY NOPOL ASC"
        Using cmd As New MySqlCommand(queryArmada, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        CmbArmada.Items.Add(rd("NOPOL").ToString())
                    End While
                End If
            End Using
        End Using
    End Sub

    Public Sub AmbilDataKaryawan()
        ' Bersihkan item ComboBox sebelum diisi
        CmbSopir.Items.Clear()
        CmbHelper1.Items.Clear()
        CmbHelper2.Items.Clear()

        ' Query untuk mengambil nama karyawan dari database
        Dim queryArmada As String = "SELECT Nama FROM tbl_Karyawan ORDER BY Nama ASC"
        Using cmd As New MySqlCommand(queryArmada, conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    While rd.Read()
                        ' Tambahkan nama karyawan ke masing-masing ComboBox
                        CmbSopir.Items.Add(rd("Nama").ToString())
                        CmbHelper1.Items.Add(rd("Nama").ToString())
                        CmbHelper2.Items.Add(rd("Nama").ToString())
                    End While
                End If
            End Using
        End Using

        ' Tambahkan pilihan kosong di akhir setiap ComboBox
        CmbSopir.Items.Add("")
        CmbHelper1.Items.Add("")
        CmbHelper2.Items.Add("")
    End Sub



    Private Sub GenerateNomorSuratJalan()
        Dim cekTanggal As String = DtpSuratJalan.Value.ToString("yyMMdd")
        Dim UrutKOde As String = ""
        Dim cekNomor As String = "SJ-" & cekTanggal

        ' Query untuk mendapatkan nomor maksimum berdasarkan format
        Using cmd As New MySqlCommand("SELECT MAX(NOTA) FROM Surat_Jalan WHERE NOTA LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")

            ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
            Dim maxKode As Object = cmd.ExecuteScalar()

            If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                Dim MaxNilaiKode As String = maxKode.ToString()
                If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "SJ-" & cekTanggal Then
                    ' Hitung nomor berikutnya
                    Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                    UrutKOde = "SJ-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                End If
            End If
        End Using

        ' Jika UrutKOde masih kosong, buat nomor pertama
        If String.IsNullOrEmpty(UrutKOde) Then
            UrutKOde = "SJ-" & cekTanggal & "0001"
        End If


        LblNoNota.Text = UrutKOde

    End Sub

    Private Sub CenterPanel()
        Dim x As Integer = (ClientSize.Width - PanelDataPenjualan.Width) \ 2
        Dim y As Integer = (ClientSize.Height - PanelDataPenjualan.Height) \ 2
        'Dim y As Integer = 44
        PanelDataPenjualan.Location = New Point(x, y)
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        PanelDataPenjualan.Visible = False
        PanelHeader.Enabled = True
        PanelNota.Enabled = True
        PanelSimpan.Enabled = True
    End Sub

    Private Sub BtnDaftarBarang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDaftarBarang.Click
        CenterPanel()
        PanelDataPenjualan.Visible = True
        PanelHeader.Enabled = False
        PanelNota.Enabled = False
        PanelSimpan.Enabled = False

        ' Check data in DGVSuratJalan and mark corresponding rows in DGVPenjualan as checked
        For Each rowSJ As DataGridViewRow In DGVSuratJalan.Rows
            Dim idPenjualanSJ As Object = rowSJ.Cells("Nota").Value

            For Each rowPJ As DataGridViewRow In DGVPenjualan.Rows
                Dim idPenjualanPJ As Object = rowPJ.Cells("ID_PENJUALAN").Value

                If idPenjualanSJ IsNot Nothing AndAlso idPenjualanSJ.Equals(idPenjualanPJ) Then
                    ' Check the checkbox cell for the corresponding row in DGVPenjualan
                    Dim chkCell As DataGridViewCheckBoxCell = CType(rowPJ.Cells("chk"), DataGridViewCheckBoxCell)
                    chkCell.Value = True
                    Exit For ' Move to the next row in DGVSuratJalan
                End If
            Next
        Next
    End Sub


    Private Sub DtpPenjualan_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DtpPenjualan.ValueChanged
        AmbildataPenjualan()
    End Sub

    Private Sub AmbildataPenjualan()
        Dim tanggalAwal As Date = DtpPenjualan.Value.Date
        Dim tanggalAkhir As Date = DtpPenjualan.Value.Date.AddDays(1).AddTicks(-1)
        Dim query As String = "SELECT ID_PENJUALAN, ID_PELANGGAN, NAMA_PELANGGAN, ALAMAT_PELANGGAN, TGL_TRANSAKSI, GRAND_TOTAL_STL_PAJAK, LOKASIBARANG FROM penjualan WHERE TGL_TRANSAKSI BETWEEN @tanggalAwal AND @tanggalAkhir ORDER BY ID_PENJUALAN"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    DGVPenjualan.SuspendLayout() ' Suspend layout untuk meningkatkan kinerja

                    DGVPenjualan.Rows.Clear()

                    Do While rd.Read()
                        DGVPenjualan.Rows.Add(False, rd("ID_PENJUALAN"), rd("ID_PELANGGAN"), rd("NAMA_PELANGGAN"), rd("ALAMAT_PELANGGAN"), rd("TGL_TRANSAKSI"), rd("GRAND_TOTAL_STL_PAJAK"), rd("LOKASIBARANG"))
                    Loop

                    DGVPenjualan.ResumeLayout() ' Lanjutkan layout setelah menambahkan baris
                Else
                    DGVPenjualan.Rows.Clear()
                End If
            End Using
        End Using

        ' Pengaturan tampilan DataGridView
        With DGVPenjualan
            ' Pengaturan umum DataGridView
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False

            ' Menambahkan kolom checkbox jika belum ada
            If .Columns.Count = 0 Then
                Dim chk As New DataGridViewCheckBoxColumn() With {
                .HeaderText = "",
                .Name = "chk",
                .Width = 30
            }
                .Columns.Add(chk)

                .Columns.Add("ID_PENJUALAN", "NO NOTA")
                .Columns.Add("ID_PELANGGAN", "ID PELANGGAN")
                .Columns.Add("NAMA_PELANGGAN", "PELANGGAN")
                .Columns.Add("ALAMAT_PELANGGAN", "ALAMAT")
                .Columns.Add("TGL_TRANSAKSI", "TANGGAL JUAL")
                .Columns.Add("GRAND_TOTAL_STL_PAJAK", "NOMINAL")
                .Columns.Add("LOKASIBARANG", "LOKASI")
            End If


            ' Pengaturan format dan visibilitas kolom
            .Columns("ID_PELANGGAN").Visible = False
            .Columns("TGL_TRANSAKSI").DefaultCellStyle.Format = "dd/MM/yyyy"
            .Columns("GRAND_TOTAL_STL_PAJAK").DefaultCellStyle.Format = "N0"
            .Columns("GRAND_TOTAL_STL_PAJAK").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            ' Mengubah nama header kolom
            .Columns("ID_PENJUALAN").HeaderText = "NO NOTA"
            .Columns("NAMA_PELANGGAN").HeaderText = "PELANGGAN"
            .Columns("ALAMAT_PELANGGAN").HeaderText = "ALAMAT"
            .Columns("TGL_TRANSAKSI").HeaderText = "TANGGAL JUAL"
            .Columns("GRAND_TOTAL_STL_PAJAK").HeaderText = "NOMINAL"
            .Columns("LOKASIBARANG").HeaderText = "LOKASI"
            .ClearSelection()
        End With

    End Sub



    ' Add a button to transfer data from DGVPenjualan to DGVSuratJalan
    Private Sub BtnTransfer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnTransfer.Click
        TransferDataToSuratJalan()
        HitungTotalDataDanRupiah()
        PanelDataPenjualan.Visible = False
        PanelHeader.Enabled = True
        PanelNota.Enabled = True
        PanelSimpan.Enabled = True
    End Sub

    Private Sub TransferDataToSuratJalan()
        For Each row As DataGridViewRow In DGVPenjualan.Rows
            Dim chk As DataGridViewCheckBoxCell = CType(row.Cells("chk"), DataGridViewCheckBoxCell)

            ' Check if the checkbox is checked and the data is not already present in DGVSuratJalan
            If Convert.ToBoolean(chk.Value) AndAlso Not DataExistsInDGVSuratJalan(row.Cells("ID_PENJUALAN").Value) Then
                DGVSuratJalan.Rows.Add(row.Cells("ID_PENJUALAN").Value, row.Cells("ID_PELANGGAN").Value, row.Cells("NAMA_PELANGGAN").Value, row.Cells("ALAMAT_PELANGGAN").Value, Convert.ToDateTime(row.Cells("TGL_TRANSAKSI").Value).ToString("yyyy-MM-dd HH:mm:ss"), row.Cells("GRAND_TOTAL_STL_PAJAK").Value, row.Cells("LOKASIBARANG").Value)
            End If
        Next
        DGVSuratJalan.ClearSelection()
    End Sub

    Private Function DataExistsInDGVSuratJalan(ByVal idPenjualan As Object) As Boolean
        For Each row As DataGridViewRow In DGVSuratJalan.Rows
            If row.Cells("Nota").Value IsNot Nothing AndAlso row.Cells("Nota").Value.Equals(idPenjualan) Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub DGVSuratJalan_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DGVSuratJalan.CellContentClick
        If DGVSuratJalan.Rows.Count > 0 AndAlso e.ColumnIndex = DGVSuratJalan.Columns("btnHapus").Index AndAlso e.RowIndex >= 0 Then
            DGVSuratJalan.Rows.RemoveAt(e.RowIndex)
            HitungTotalDataDanRupiah()
        End If
    End Sub

    Private Sub HitungTotalDataDanRupiah()
        Dim totalData As Integer = 0
        Dim totalRupiah As Decimal = 0

        For Each row As DataGridViewRow In DGVSuratJalan.Rows
            If Not row.IsNewRow Then
                totalData += 1
                Dim nilaiRupiah As Decimal = 0
                If Decimal.TryParse(row.Cells("NOMINAL").Value.ToString(), nilaiRupiah) Then
                    totalRupiah += nilaiRupiah
                End If
            End If
        Next

        ' Menetapkan nilai total pada TextBox
        TxtTotalPelanggan.Text = totalData.ToString()
        TxtTotalRupiah.Text = totalRupiah.ToString("N0") ' Format nilai rupiah dengan pemisah ribuan
    End Sub

    Private Sub CmbArmada_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbArmada.SelectedIndexChanged
        Dim sql As String = "SELECT KODE, jenis FROM tbl_Armada WHERE NOPOL = @NOPOL"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NOPOL", CmbArmada.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblKodeArmada.Text = reader("KODE").ToString()
                    LblJenisArmada.Text = reader("jenis").ToString()
                Else
                    LblKodeArmada.Text = ""
                    LblJenisArmada.Text = ""
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbSopir_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSopir.SelectedIndexChanged
        Dim sql As String = "SELECT Kode FROM tbl_karyawan WHERE Nama = @Nama"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", CmbSopir.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblKodeSupir.Text = reader("Kode").ToString()
                Else
                    LblKodeSupir.Text = ""
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbHelper1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbHelper1.SelectedIndexChanged
        Dim sql As String = "SELECT Kode FROM tbl_karyawan WHERE Nama = @Nama"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", CmbHelper1.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblKodeHelper1.Text = reader("Kode").ToString()
                Else
                    LblKodeHelper1.Text = ""
                End If
            End Using
        End Using
    End Sub

    Private Sub CmbHelper2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbHelper2.SelectedIndexChanged
        Dim sql As String = "SELECT Kode FROM tbl_karyawan WHERE Nama = @Nama"
        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@Nama", CmbHelper2.Text)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    LblKodeHelper2.Text = reader("Kode").ToString()
                Else
                    LblKodeHelper2.Text = ""
                End If
            End Using
        End Using
    End Sub



    Private Sub BtnSimpann_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpann.Click
        If Not Validasi() Then
            Return ' Batalkan aksi jika validasi gagal
        End If

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = Nothing

        Try
            If LblJenisTrans.Text = "TambahSuratJalan" Then
                DtpSuratJalan.Value = DateTime.Now
                GenerateNomorSuratJalan()
            Else
                Dim NoNota As String = LblNoNota.Text

                Dim queryDeleteSuratJalanDetail As String = "DELETE FROM surat_jalan_detail WHERE NOTA = @NOTA"

                ' Hapus dari tabel surat_jalan_detail
                Using cmdDetail As New MySqlCommand(queryDeleteSuratJalanDetail, conn)
                    cmdDetail.Parameters.AddWithValue("@NOTA", NoNota)
                    cmdDetail.ExecuteNonQuery()
                End Using

                Dim queryDeleteSuratJalan As String = "DELETE FROM surat_jalan WHERE NOTA = @NOTA"
                ' Hapus dari tabel surat_jalan
                Using cmd As New MySqlCommand(queryDeleteSuratJalan, conn)
                    cmd.Parameters.AddWithValue("@NOTA", NoNota)
                    cmd.ExecuteNonQuery()
                End Using

            End If


            transaction = conn.BeginTransaction()

            SimpanSuratJalan(transaction)
            SimpanSuratJalanDetail(transaction)


            ' Commit transaksi jika berhasil
            transaction.Commit()


            With PrinterSuratJalan
                .TxtNota.Text = LblNoNota.Text
                .ProsesCetak()
            End With

            If LblJenisTrans.Text = "TambahSuratJalan" Then
                DatabaseModule.CatatanAksiHistory("Simpan surat jalan " & LblNoNota.Text)
                KondisiAwal()
            Else
                DatabaseModule.CatatanAksiHistory("Edit surat jalan " & LblNoNota.Text)
                Me.Close()
                FormUtama.DataSuratjalan()
                FormUtama.GBTransaksi.Visible = True
            End If



        Catch ex As Exception
            transaction.Rollback()

            ' Tampilkan pesan kesalahan kepada pengguna
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try



    End Sub


    Private Function Validasi() As Boolean
        If DGVSuratJalan.RowCount <= 1 Then
            MessageBox.Show("Belum ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If TxtTotalPelanggan.Text = "" Then
            MessageBox.Show("Belum ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If TxtTotalRupiah.Text = "" Then
            MessageBox.Show("Belum ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If CmbArmada.SelectedIndex = -1 Then
            MessageBox.Show("Silahkan pilih Armada", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbArmada.DroppedDown = True
            Return False
        End If

        If CmbSopir.SelectedIndex = -1 Then
            MessageBox.Show("Silahkan pilih Sopir", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbSopir.DroppedDown = True
            Return False
        End If

        Return True
    End Function

    Private Sub SimpanSuratJalan(ByVal transaction As MySqlTransaction)
        Dim query As String = "INSERT INTO surat_jalan (NOTA, TGL_PENGIRIMAN, LOKASI, TOTAL_PELANGGAN, TOTAL_RUPIAH, KODE_ARMADA, ARMADA, JENIS_ARMADA, KODE_SUPIR, SUPIR, KODE_HELPER1, HELPER1, KODE_HELPER2, HELPER2, ID_USER, ID_KOMPUTER) " &
                           "VALUES (@NOTA, @TGL_PENGIRIMAN, @LOKASI, @TOTAL_PELANGGAN, @TOTAL_RUPIAH, @KODE_ARMADA, @ARMADA, @JENIS_ARMADA, @KODE_SUPIR, @SUPIR, @KODE_HELPER1, @HELPER1, @KODE_HELPER2, @HELPER2, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(query, conn, transaction)
            cmd.Parameters.AddWithValue("@NOTA", LblNoNota.Text)
            cmd.Parameters.AddWithValue("@TGL_PENGIRIMAN", DtpSuratJalan.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.SLokasi.Text)
            cmd.Parameters.AddWithValue("@TOTAL_PELANGGAN", Convert.ToInt32(TxtTotalPelanggan.Text))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", Convert.ToDecimal(TxtTotalRupiah.Text))
            cmd.Parameters.AddWithValue("@KODE_ARMADA", LblKodeArmada.Text)
            cmd.Parameters.AddWithValue("@ARMADA", CmbArmada.Text)
            cmd.Parameters.AddWithValue("@JENIS_ARMADA", LblJenisArmada.Text)
            cmd.Parameters.AddWithValue("@KODE_SUPIR", LblKodeSupir.Text)
            cmd.Parameters.AddWithValue("@SUPIR", CmbSopir.Text)
            cmd.Parameters.AddWithValue("@KODE_HELPER1", LblKodeHelper1.Text)
            cmd.Parameters.AddWithValue("@HELPER1", CmbHelper1.Text)
            cmd.Parameters.AddWithValue("@KODE_HELPER2", LblKodeHelper2.Text)
            cmd.Parameters.AddWithValue("@HELPER2", CmbHelper2.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub SimpanSuratJalanDetail(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DGVSuratJalan.Rows
            If Not row.IsNewRow AndAlso row.Cells("NOTA").Value IsNot Nothing AndAlso row.Cells("NOTA").Value.ToString() <> "" Then
                Dim sqlrinci As String = "INSERT INTO Surat_Jalan_Detail (NOTA, TANGGAL_KIRIM, LOKASISIMPAN, NOTA_BELANJA, KODE_PELANGGAN, NAMA_PELANGGAN, ALAMAT_PELANGGAN, TANGGAL_BELANJA, NILAI_BELANJA, LOKASI, ID_USER, ID_KOMPUTER) " &
                                         "VALUES (@NOTA, @TANGGAL_KIRIM, @LOKASISIMPAN, @NOTA_BELANJA, @KODE_PELANGGAN, @NAMA_PELANGGAN, @ALAMAT_PELANGGAN, @TANGGAL_BELANJA, @NILAI_BELANJA, @LOKASI, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(sqlrinci, conn, transaction)
                    cmd.Parameters.AddWithValue("@NOTA", LblNoNota.Text)
                    cmd.Parameters.AddWithValue("@TANGGAL_KIRIM", DtpSuratJalan.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@LOKASISIMPAN", FormUtama.SLokasi.Text)
                    cmd.Parameters.AddWithValue("@NOTA_BELANJA", row.Cells("Nota").Value.ToString())
                    cmd.Parameters.AddWithValue("@KODE_PELANGGAN", row.Cells("Kode").Value.ToString())
                    cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", row.Cells("Pelanggan").Value.ToString())
                    cmd.Parameters.AddWithValue("@ALAMAT_PELANGGAN", row.Cells("Alamat").Value.ToString())
                    cmd.Parameters.AddWithValue("@TANGGAL_BELANJA", Convert.ToDateTime(row.Cells("Tanggal").Value).ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@NILAI_BELANJA", Convert.ToDecimal(row.Cells("Nominal").Value))
                    cmd.Parameters.AddWithValue("@LOKASI", row.Cells("Lokasi").Value.ToString())
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        Next
    End Sub


    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click, BtnBatal.Click
        FormUtama.Refresdatagridview()
        FormUtama.GBTransaksi.Visible = True
        Close()
    End Sub

    Private Sub FormSuratJalan_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F8
                BtnSimpann.PerformClick()
            Case Keys.Escape
                If PanelDataPenjualan.Visible = True Then
                    Button2.PerformClick()
                Else
                    BtnClose.PerformClick()
                End If

        End Select
    End Sub
End Class