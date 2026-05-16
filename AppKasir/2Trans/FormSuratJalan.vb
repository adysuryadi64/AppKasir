Public Class FormSuratJalan


    Private Sub FormSuratJalan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        ' Nilai keuangan otomatis via nama TxtGrandtotal
        ' Rename TxtTotalRupiah/TxtTotalPelanggan -> TxtGrandtotal untuk tema otomatis
        If LblJenisTrans.Text = "TambahSuratJalan" Then
            AmbilDataArmada()
            AmbilDataKaryawan()
            KondisiAwal()
        Else
            LoadSuratJalanDetail(LblNoNota.Text)
        End If
        PanelDataPenjualan.Visible = False
    End Sub


    Private Sub LakukanCetakSuratJalan(nota As String)
        If BacaPengaturanPrinter("SuratJalan", "PilihPrinter", "LANGSUNG CETAK") = "TANYA PILIH PRINTER" Then
            ModulePrinterSuratJalan.TanyaPilihPrinterSuratJalan(nota)
        Else
            ModulePrinterSuratJalan.CetakSuratJalan(nota)
        End If
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

        ModulHakAkses.ResetDTPKeTanggalHariIni(DtpSuratJalan)
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
                            ModuleAngka.ParseDecimal(rd("NILAI_BELANJA")).ToString("N2"),
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
        Dim queryArmada As String = "SELECT Nama FROM tbl_Karyawan WHERE Status = 'Aktif' ORDER BY Nama ASC"
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
        Using cmd As New MySqlCommand(
            "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
            cmd.Parameters.AddWithValue("@prefix", "SJ")
            cmd.Parameters.AddWithValue("@tgl", DtpSuratJalan.Value.Date)
            cmd.Parameters.AddWithValue("@tabel", "surat_jalan")
            cmd.Parameters.AddWithValue("@kolom", "NOTA")
            Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
            pNomor.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            LblNoNota.Text = pNomor.Value?.ToString()
        End Using
    End Sub

    Private Sub CenterPanel()
        Dim x As Integer = (ClientSize.Width - PanelDataPenjualan.Width) \ 2
        Dim y As Integer = (ClientSize.Height - PanelDataPenjualan.Height) \ 2
        'Dim y As Integer = 44
        PanelDataPenjualan.Location = New Point(x, y)
    End Sub


    Private Sub BtnHideDaftar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHideDaftar.Click
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

            ' Mengubah nama header kolom
            .Columns("ID_PENJUALAN").HeaderText = "NO NOTA"
            .Columns("NAMA_PELANGGAN").HeaderText = "PELANGGAN"
            .Columns("ALAMAT_PELANGGAN").HeaderText = "ALAMAT"
            .Columns("TGL_TRANSAKSI").HeaderText = "TANGGAL JUAL"
            .Columns("GRAND_TOTAL_STL_PAJAK").HeaderText = "NOMINAL"
            .Columns("LOKASIBARANG").HeaderText = "LOKASI"
            .ClearSelection()
        End With
        ModuleAngka.TerapkanFormatKolomAngka(DGVPenjualan, "GRAND_TOTAL_STL_PAJAK")

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
                Dim nilaiRupiah As Decimal = ModuleAngka.ParseDecimal(row.Cells("NOMINAL").Value)
                totalRupiah += nilaiRupiah
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
                ModulHakAkses.ResetDTPKeTanggalHariIni(DtpSuratJalan)
                GenerateNomorSuratJalan()
            Else
                Dim NoNota As String = LblNoNota.Text

                transaction = conn.BeginTransaction()

                ' ========================================
                ' START: Audit Trail - Edit Surat Jalan
                ' ========================================
                Dim sbSnapshot As New System.Text.StringBuilder()
                Try
                    Using cmdSnap As New MySqlCommand(
                        "SELECT NOTA, TANGGAL, KODE_SUPIR, NAMA_SUPIR, KODE_HELPER1, NAMA_HELPER1, KODE_HELPER2, NAMA_HELPER2, KETERANGAN " &
                        "FROM surat_jalan WHERE NOTA = @n LIMIT 1", conn, transaction)
                        cmdSnap.Parameters.AddWithValue("@n", NoNota)
                        Using rdSnap = cmdSnap.ExecuteReader()
                            If rdSnap.Read() Then
                                sbSnapshot.AppendLine($"Nota: {rdSnap("NOTA")}")
                                sbSnapshot.AppendLine($"Tanggal: {Convert.ToDateTime(rdSnap("TANGGAL")).ToString("dd/MM/yyyy HH:mm:ss")}")
                                sbSnapshot.AppendLine($"Kode Supir: {rdSnap("KODE_SUPIR")}")
                                sbSnapshot.AppendLine($"Nama Supir: {rdSnap("NAMA_SUPIR")}")
                                sbSnapshot.AppendLine($"Kode Helper 1: {rdSnap("KODE_HELPER1")}")
                                sbSnapshot.AppendLine($"Nama Helper 1: {rdSnap("NAMA_HELPER1")}")
                                sbSnapshot.AppendLine($"Kode Helper 2: {rdSnap("KODE_HELPER2")}")
                                sbSnapshot.AppendLine($"Nama Helper 2: {rdSnap("NAMA_HELPER2")}")
                                sbSnapshot.AppendLine($"Keterangan: {rdSnap("KETERANGAN")}")
                            End If
                        End Using
                    End Using

                    sbSnapshot.AppendLine(vbCrLf & "Detail Barang:")
                    Using cmdSnapDetail As New MySqlCommand(
                        "SELECT KODE_BARANG, NAMA_BARANG, QTY, KETERANGAN_DETAIL " &
                        "FROM surat_jalan_detail WHERE NOTA = @n ORDER BY KODE_BARANG", conn, transaction)
                        cmdSnapDetail.Parameters.AddWithValue("@n", NoNota)
                        Using rdSnapDetail = cmdSnapDetail.ExecuteReader()
                            While rdSnapDetail.Read()
                                sbSnapshot.AppendLine($"- {rdSnapDetail("KODE_BARANG")} - {rdSnapDetail("NAMA_BARANG")}: {rdSnapDetail("QTY")} unit - {rdSnapDetail("KETERANGAN_DETAIL")}")
                            End While
                        End Using
                    End Using
                Catch
                    sbSnapshot.AppendLine("Gagal baca data sebelum edit")
                End Try
                ModuleAuditTrail.CatatAuditMaster("SJ:" & NoNota, "EDIT", "Surat Jalan", sbSnapshot.ToString(), trans:=transaction)
                ' ========================================
                ' END: Audit Trail - Edit Surat Jalan
                ' ========================================

                Dim queryDeleteSuratJalanDetail As String = "DELETE FROM surat_jalan_detail WHERE NOTA = @NOTA"

                ' Hapus dari tabel surat_jalan_detail
                Using cmdDetail As New MySqlCommand(queryDeleteSuratJalanDetail, conn, transaction)
                    cmdDetail.Parameters.AddWithValue("@NOTA", NoNota)
                    cmdDetail.ExecuteNonQuery()
                End Using

                Dim queryDeleteSuratJalan As String = "DELETE FROM surat_jalan WHERE NOTA = @NOTA"
                ' Hapus dari tabel surat_jalan
                Using cmd As New MySqlCommand(queryDeleteSuratJalan, conn, transaction)
                    cmd.Parameters.AddWithValue("@NOTA", NoNota)
                    cmd.ExecuteNonQuery()
                End Using

            End If


            If transaction Is Nothing Then
                transaction = conn.BeginTransaction()
            End If

            SimpanSuratJalan(transaction)
            SimpanSuratJalanDetail(transaction)


            ' Commit transaksi jika berhasil
            transaction.Commit()

            Dim notaCetak As String = LblNoNota.Text

            If LblJenisTrans.Text = "TambahSuratJalan" Then
                KondisiAwal()
            Else
                Me.Close()
                FormUtama.DataSuratjalan()
                FormUtama.GBTransaksi.Visible = True
            End If

            Try
                Select Case BacaPengaturanPrinter("SuratJalan", "CetakOtomatis", "IYA").Trim().ToUpper()
                    Case "IYA"
                        LakukanCetakSuratJalan(notaCetak)
                    Case "SELALU TANYA"
                        If MessageBox.Show("Apakah Anda ingin mencetak surat jalan?",
                                           "Konfirmasi Cetak", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            LakukanCetakSuratJalan(notaCetak)
                        End If
                End Select
            Catch ex As Exception
                MessageBox.Show("Gagal mencetak surat jalan." & vbCrLf & "Detail: " & ex.Message,
                                "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try



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
            cmd.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
            cmd.Parameters.AddWithValue("@TOTAL_PELANGGAN", ModuleAngka.ParseInteger(TxtTotalPelanggan.Text))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", ModuleAngka.ParseDecimal(TxtTotalRupiah.Text))
            cmd.Parameters.AddWithValue("@KODE_ARMADA", LblKodeArmada.Text)
            cmd.Parameters.AddWithValue("@ARMADA", CmbArmada.Text)
            cmd.Parameters.AddWithValue("@JENIS_ARMADA", LblJenisArmada.Text)
            cmd.Parameters.AddWithValue("@KODE_SUPIR", LblKodeSupir.Text)
            cmd.Parameters.AddWithValue("@SUPIR", CmbSopir.Text)
            cmd.Parameters.AddWithValue("@KODE_HELPER1", LblKodeHelper1.Text)
            cmd.Parameters.AddWithValue("@HELPER1", CmbHelper1.Text)
            cmd.Parameters.AddWithValue("@KODE_HELPER2", LblKodeHelper2.Text)
            cmd.Parameters.AddWithValue("@HELPER2", CmbHelper2.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)

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
                    cmd.Parameters.AddWithValue("@LOKASISIMPAN", FormUtama.StatusLokasi.Text)
                    cmd.Parameters.AddWithValue("@NOTA_BELANJA", row.Cells("Nota").Value.ToString())
                    cmd.Parameters.AddWithValue("@KODE_PELANGGAN", row.Cells("Kode").Value.ToString())
                    cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", row.Cells("Pelanggan").Value.ToString())
                    cmd.Parameters.AddWithValue("@ALAMAT_PELANGGAN", row.Cells("Alamat").Value.ToString())
                    cmd.Parameters.AddWithValue("@TANGGAL_BELANJA", Convert.ToDateTime(row.Cells("Tanggal").Value).ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@NILAI_BELANJA", Convert.ToDecimal(row.Cells("Nominal").Value))
                    cmd.Parameters.AddWithValue("@LOKASI", row.Cells("Lokasi").Value.ToString())
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        Next
    End Sub


    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKeluarForm.Click
        FormUtama.Refresdatagridview()
        FormUtama.GBTransaksi.Visible = True
        Close()
    End Sub

    Private Sub FormSuratJalan_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                e.SuppressKeyPress = True
                TampilkanBantuan()
            Case Keys.F8
                BtnSimpann.PerformClick()
            Case Keys.Escape
                If PanelDataPenjualan.Visible = True Then
                    BtnHideDaftar.PerformClick()
                Else
                    BtnKeluarForm.PerformClick()
                End If

        End Select
    End Sub
    Private Sub BtnSettingPrinter_Click(sender As Object, e As EventArgs) Handles BtnSettingPrinter.Click
        Using frm As New FormPengaturanPrinter() With {.FilterTab = "SuratJalan"}
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
                           "F2      : Ambil daftar penjualan" & vbCrLf &
                           "F6      : Transfer ke surat jalan" & vbCrLf &
                           "F8      : Simpan surat jalan" & vbCrLf &
                           "ESC     : Tutup panel daftar / Keluar"
        MessageBox.Show(helpText, "Bantuan - Shortcut Keyboard",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

End Class
