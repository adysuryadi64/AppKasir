Imports System.Globalization



Public Class FormTabelReferensi
    Public Sub KOndisiAwal()
        Me.Cursor = Cursors.WaitCursor
        CmbJenisAkun.Text = ""
        CmbType.Text = ""
        TxtKode.Text = ""
        TxtKode.Enabled = True
        TxtNama.Text = ""
        TxtSaldoAwal.Text = ""
        CmbSubAkun.Text = ""
        CmbDK.Text = ""
        CmbNRLR.Text = ""
        TxtSaldoAwal.Text = ""

        If Not String.IsNullOrEmpty(CmbType.Text) AndAlso CmbType.SelectedIndex <> -1 Then
            GeneratePurchaseOrderNumber(CmbType.Text)
        End If

        TampilAkun()
        ' Panggil untuk mengambil data rekening KAS dan BANK
        Rekeningkasbank()
        ' Panggil untuk mengambil data rekening KAS dan BANK dan MODAL
        AmbilAkunKasBankEkuitas()

        Me.Cursor = Cursors.Default
    End Sub


    Private Sub FormTabelReferensi_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call KOndisiAwal()

        Dim TabelReferensi As Boolean() = ModulHakAkses.BacaHakAkses(FormUtama.SLevel.Text, "Tabel Referensi", conn)
        ' Terapkan nilai hak akses ke tombol-tombol
        BtnSimpan.Visible = TabelReferensi(1) ' CanAdd 
        'BtnSimpan.Visible = TabelReferensi(2) ' CanEdit 
        BtnHapus.Visible = TabelReferensi(3) ' CanDelete 

        ' Mengisi CmbJenisAkun dengan nilai-nilai awal
        CmbJenisAkun.Items.AddRange(New String() {"ASET LANCAR", "ASET TETAP", "PASIVA", "MODAL", "HPP", "BIAYA", "PENDAPATAN LAIN", "PAJAK"})
        CmbJenisAkunCari.Items.AddRange(New String() {"ASET LANCAR", "ASET TETAP", "PASIVA", "MODAL", "HPP", "BIAYA", "PENDAPATAN LAIN", "PAJAK"})
    End Sub

    Private Sub CmbJenisAkun_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbJenisAkun.SelectedIndexChanged
        ' Membersihkan CmbType setiap kali pilihan di CmbJenisAkun berubah
        CmbType.Items.Clear()

        ' Mengisi CmbType berdasarkan pilihan di CmbJenisAkun
        Select Case CmbJenisAkun.SelectedItem.ToString()
            Case "ASET LANCAR"
                CmbType.Items.AddRange(New String() {"KAS", "BANK", "PIUTANG", "A LANCAR"})
            Case "ASET TETAP"
                CmbType.Items.AddRange(New String() {"A TETAP", "AKM PENY."})
            Case "PASIVA"
                CmbType.Items.AddRange(New String() {"HUTANG", "BEBAN", "SOSIAL"})
            Case "MODAL"
                CmbType.Items.AddRange(New String() {"EKUITAS", "LABA RUGI"})
            Case "HPP"
                CmbType.Items.Add("HPP")
            Case "BIAYA"
                CmbType.Items.Add("BIAYA")
            Case "PENDAPATAN LAIN"
                CmbType.Items.AddRange(New String() {"BUNGA", "PENDAPATAN"})
            Case "PAJAK"
                CmbType.Items.Add("PAJAK")
        End Select
    End Sub

    Private Sub CmbType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbType.SelectedIndexChanged
        GeneratePurchaseOrderNumber(CmbType.Text)

        ' Membersihkan CmbType setiap kali pilihan di CmbJenisAkun berubah
        CmbSubAkun.Items.Clear()
        CmbSubAkun.Items.AddRange(New String() {"AKTIVA", "PASIVA", "LABA RUGI", "LABA", "RUGI"})

        ' Mengisi CmbType berdasarkan pilihan di CmbJenisAkun
        Select Case CmbType.SelectedItem.ToString()
            Case "KAS", "BANK", "PIUTANG", "A LANCAR", "A TETAP", "AKM PENY."
                CmbSubAkun.SelectedIndex = 0
            Case "HUTANG", "BEBAN", "SOSIAL", "EKUITAS"
                CmbSubAkun.SelectedIndex = 1
            Case "LABA RUGI"
                CmbSubAkun.SelectedIndex = 2
            Case "HPP", "BUNGA", "PENDAPATAN"
                CmbSubAkun.SelectedIndex = 3
            Case "BIAYA", "PAJAK"
                CmbSubAkun.SelectedIndex = 4
        End Select


        CmbDK.Items.Clear()
        CmbDK.Items.AddRange(New String() {"DEBET", "KREDIT"})

        ' Mengisi CmbType berdasarkan pilihan di CmbJenisAkun
        Select Case CmbType.SelectedItem.ToString()
            Case "KAS", "BANK", "PIUTANG", "A LANCAR", "A TETAP", "AKM PENY.", "BIAYA", "PAJAK"
                CmbDK.SelectedIndex = 0
            Case "HUTANG", "BEBAN", "SOSIAL", "EKUITAS", "LABA RUGI", "HPP", "BUNGA", "PENDAPATAN"
                CmbDK.SelectedIndex = 1
        End Select


        CmbNRLR.Items.Clear()
        CmbNRLR.Items.AddRange(New String() {"NERACA", "LABA RUGI"})

        ' Mengisi CmbType berdasarkan pilihan di CmbJenisAkun
        Select Case CmbType.SelectedItem.ToString()
            Case "KAS", "BANK", "PIUTANG", "A LANCAR", "A TETAP", "AKM PENY.", "HUTANG", "BEBAN", "SOSIAL", "EKUITAS", "LABA RUGI"
                CmbNRLR.SelectedIndex = 0
            Case "HPP", "BUNGA", "PENDAPATAN", "BIAYA", "PAJAK"
                CmbNRLR.SelectedIndex = 1
        End Select


        TxtNama.Select()
    End Sub

    Private Sub GeneratePurchaseOrderNumber(ByVal typeAkun As String)

        Dim prefix As String = ""
        Select Case typeAkun
            Case "KAS" : prefix = "01.01."
            Case "BANK" : prefix = "01.02."
            Case "PIUTANG" : prefix = "01.03."
            Case "A LANCAR" : prefix = "01.04."
            Case "A TETAP" : prefix = "02.01."
            Case "AKM PENY." : prefix = "02.02."
            Case "HUTANG" : prefix = "03.01."
            Case "BEBAN" : prefix = "03.02."
            Case "SOSIAL" : prefix = "03.03."
            Case "EKUITAS" : prefix = "04.01."
            Case "HPP" : prefix = "06.01."
            Case "BIAYA" : prefix = "07.01."
            Case "PENDAPATAN LAIN" : prefix = "08.01."
            Case "PAJAK" : prefix = "09.01."
        End Select

        Dim maxKode As String = ""
        Dim existingKodes As New List(Of String)

        ' Mengambil semua kode yang sudah ada dari database
        Using cmd As New MySqlCommand("SELECT Kode_akun FROM tbl_datareferensi ORDER BY Kode_akun", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    existingKodes.Add(rd(0).ToString())
                End While
            End Using
        End Using

        ' Jika tidak ada kode yang ada, gunakan SPL-0001
        If existingKodes.Count = 0 Then
            TxtKode.Text = prefix & "001"
            Exit Sub
        End If

        ' Mencari nomor berikutnya yang belum terpakai
        For i As Integer = 1 To existingKodes.Count
            Dim expectedKode As String = prefix & i.ToString("000")
            If Not existingKodes.Contains(expectedKode) Then
                maxKode = expectedKode
                Exit For
            End If
        Next

        ' Jika tidak ada nomor berikutnya yang tersedia, gunakan nomor setelah kode terakhir
        If String.IsNullOrEmpty(maxKode) Then
            Dim lastKode As String = existingKodes(existingKodes.Count - 1)
            Dim Hitung As Integer = Integer.Parse(lastKode.Substring(lastKode.Length - 3)) + 1
            maxKode = prefix & Hitung.ToString("000")
        End If

        TxtKode.Text = maxKode
    End Sub

    Public bsAkun As New BindingSource()
    Public dtAkun As New DataTable()

    Public Sub TampilAkun()
        Dim query As String = "SELECT STATUS, JENIS_AKUN, TYPE_AKUN, KODE_AKUN, NAMA_AKUN, SUB_AKUN, AKUN_DK, AKUN_NRLR, SALDO_AWAL, S_DEBET, S_KREDIT, SALDO_AKHIR FROM tbl_datareferensi ORDER BY KODE_AKUN"

        Using cmd As New MySqlCommand(query, conn)
            Using da As New MySqlDataAdapter(cmd)
                dtAkun.Clear()
                da.Fill(dtAkun)
            End Using
        End Using

        bsAkun.DataSource = dtAkun
        Dgvdata.DataSource = bsAkun

        ' Setup DataGridView columns
        SetupDataGridViewColumns()
    End Sub

    Private Sub CmbJenisAkunCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbJenisAkunCari.SelectedIndexChanged
        Dim query As String = "SELECT STATUS, JENIS_AKUN, TYPE_AKUN, KODE_AKUN, NAMA_AKUN, SUB_AKUN, AKUN_DK, AKUN_NRLR, SALDO_AWAL, S_DEBET, S_KREDIT, SALDO_AKHIR FROM tbl_datareferensi WHERE JENIS_AKUN LIKE @Jenis_Akun ORDER BY KODE_AKUN"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Jenis_Akun", "%" & CmbJenisAkunCari.Text & "%")

            Using adapter As New MySqlDataAdapter(cmd)
                dtAkun.Clear()
                adapter.Fill(dtAkun)

                ' Bind the filtered data to the DataGridView
                bsAkun.DataSource = dtAkun
                Dgvdata.DataSource = bsAkun

                ' Setup DataGridView columns
                SetupDataGridViewColumns()
            End Using
        End Using
    End Sub

    Private Sub SetupDataGridViewColumns()
        ' Format and align columns
        With Dgvdata
            .Columns("STATUS").Visible = False
            .Columns("JENIS_AKUN").FillWeight = 90
            .Columns("TYPE_AKUN").FillWeight = 90
            .Columns("KODE_AKUN").FillWeight = 70
            .Columns("NAMA_AKUN").FillWeight = 250
            .Columns("SUB_AKUN").FillWeight = 70
            .Columns("AKUN_DK").FillWeight = 60
            .Columns("AKUN_NRLR").FillWeight = 80

            ' Set column format and alignment
            For Each column As DataGridViewColumn In .Columns
                Dim columnsToFormat As String() = {"SALDO_AWAL", "S_DEBET", "S_KREDIT", "SALDO_AKHIR"}
                If columnsToFormat.Contains(column.Name) Then
                    column.DefaultCellStyle.Format = "#,0.##"
                    column.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID")
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If

                ' Change column header names (if necessary)
                Select Case column.Name
                    Case "JENIS_AKUN" : column.HeaderText = "JENIS"
                    Case "TYPE_AKUN" : column.HeaderText = "TYPE"
                    Case "KODE_AKUN" : column.HeaderText = "KODE"
                    Case "NAMA_AKUN" : column.HeaderText = "NAMA"
                    Case "SUB_AKUN" : column.HeaderText = "SUB"
                    Case "AKUN_DK" : column.HeaderText = "DK"
                    Case "AKUN_NRLR" : column.HeaderText = "NRLR"
                    Case "SALDO_AWAL" : column.HeaderText = "S AWAL"
                    Case "S_DEBET" : column.HeaderText = "S DEBET"
                    Case "S_KREDIT" : column.HeaderText = "S KREDIT"
                    Case "SALDO_AKHIR" : column.HeaderText = "S AKHIR"
                End Select
            Next

            ' Set header style
            .ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow

            ' Set alternating row style
            .AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

            ' Set visual style
            .BorderStyle = BorderStyle.FixedSingle
            .GridColor = Color.Silver
            .BackgroundColor = Color.White

            ' Enable double buffering to reduce flickering (if needed)
            EnableDoubleBuffering(Dgvdata)
        End With
    End Sub

    ' Method to enable double buffering
    Public Shared Sub EnableDoubleBuffering(ByVal dgv As DataGridView)
        dgv.GetType().InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.SetProperty, Nothing, dgv, New Object() {True})
    End Sub

    Private Sub ButtonSimpan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpan.Click
        ' Reset semua pesan kesalahan
        ErrorProvider1.Clear()

        ' Validasi input
        If String.IsNullOrWhiteSpace(CmbJenisAkun.Text) Then
            ErrorProvider1.SetError(CmbJenisAkun, "Harus di isi")
            CmbJenisAkun.Focus()
            Exit Sub
        ElseIf String.IsNullOrWhiteSpace(CmbType.Text) Then
            ErrorProvider1.SetError(CmbType, "Harus di isi")
            CmbType.Focus()
            Exit Sub
        ElseIf String.IsNullOrWhiteSpace(TxtKode.Text) Then
            ErrorProvider1.SetError(TxtKode, "Harus di isi")
            TxtKode.Focus()
            Exit Sub
        ElseIf String.IsNullOrWhiteSpace(TxtNama.Text) Then
            ErrorProvider1.SetError(TxtNama, "Harus di isi")
            TxtNama.Focus()
            Exit Sub
        ElseIf String.IsNullOrWhiteSpace(CmbSubAkun.Text) Then
            ErrorProvider1.SetError(CmbSubAkun, "Harus di isi")
            CmbSubAkun.Focus()
            Exit Sub
        ElseIf String.IsNullOrWhiteSpace(CmbDK.Text) Then
            ErrorProvider1.SetError(CmbDK, "Harus di isi")
            CmbDK.Focus()
            Exit Sub
        ElseIf String.IsNullOrWhiteSpace(CmbNRLR.Text) Then
            ErrorProvider1.SetError(CmbNRLR, "Harus di isi")
            CmbNRLR.Focus()
            Exit Sub
        End If

        ' Mulai transaksi
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            ' Periksa apakah data dengan Kode_Akun tertentu sudah ada
            Dim queryCheck As String = "SELECT Kode_Akun FROM tbl_datareferensi WHERE Kode_Akun = @KodeAkun"
            Using cmdCheck As New MySqlCommand(queryCheck, conn, transaction)
                cmdCheck.Parameters.AddWithValue("@KodeAkun", TxtKode.Text)
                Using Rd As MySqlDataReader = cmdCheck.ExecuteReader()
                    If Rd.HasRows Then
                        ' Tutup DataReader sebelum update
                        Rd.Close()

                        ' Jika data sudah ada, lakukan pembaruan (Edit)
                        Dim EditData As String = "UPDATE tbl_datareferensi SET Jenis_Akun = @Jenis_Akun, Type_Akun = @TypeAkun, Nama_Akun = @NamaAkun, " &
                                             "Sub_Akun = @SubAkun, Akun_DK = @AkunDK, Akun_NRLR = @AkunNRLR, Saldo_awal = @SaldoAwal WHERE Kode_Akun = @KodeAkun"
                        Using cmd As New MySqlCommand(EditData, conn, transaction)
                            cmd.Parameters.AddWithValue("@Jenis_Akun", CmbJenisAkun.Text)
                            cmd.Parameters.AddWithValue("@TypeAkun", CmbType.Text)
                            cmd.Parameters.AddWithValue("@NamaAkun", StrConv(TxtNama.Text, vbUpperCase))
                            cmd.Parameters.AddWithValue("@SubAkun", CmbSubAkun.Text)
                            cmd.Parameters.AddWithValue("@AkunDK", CmbDK.Text)
                            cmd.Parameters.AddWithValue("@AkunNRLR", CmbNRLR.Text)
                            cmd.Parameters.AddWithValue("@SaldoAwal", If(String.IsNullOrEmpty(TxtSaldoAwal.Text), "0", Val(TxtSaldoAwal.Text)))
                            cmd.Parameters.AddWithValue("@KodeAkun", TxtKode.Text)
                            cmd.ExecuteNonQuery()
                        End Using
                    Else
                        ' Tutup DataReader sebelum insert
                        Rd.Close()

                        ' Jika data belum ada, lakukan insert
                        Dim InputData As String = "INSERT INTO tbl_datareferensi (Jenis_Akun, Type_Akun, Kode_Akun, Nama_Akun, Sub_Akun, Akun_DK, Akun_NRLR, Saldo_Awal) " &
                                              "VALUES (@Jenis_Akun, @TypeAkun, @Kode_Akun, @NamaAkun, @SubAkun, @AkunDK, @AkunNRLR, @Saldo_Awal)"
                        Using cmd As New MySqlCommand(InputData, conn, transaction)
                            cmd.Parameters.AddWithValue("@Jenis_Akun", CmbJenisAkun.Text)
                            cmd.Parameters.AddWithValue("@TypeAkun", CmbType.Text)
                            cmd.Parameters.AddWithValue("@Kode_Akun", TxtKode.Text)
                            cmd.Parameters.AddWithValue("@NamaAkun", StrConv(TxtNama.Text, vbUpperCase))
                            cmd.Parameters.AddWithValue("@SubAkun", CmbSubAkun.Text)
                            cmd.Parameters.AddWithValue("@AkunDK", CmbDK.Text)
                            cmd.Parameters.AddWithValue("@AkunNRLR", CmbNRLR.Text)
                            cmd.Parameters.AddWithValue("@Saldo_Awal", If(String.IsNullOrEmpty(TxtSaldoAwal.Text), "0", Val(TxtSaldoAwal.Text)))
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                End Using
            End Using


            transaction.Commit()
            ' Catatan aksi dan komit transaksi
            DatabaseModule.CatatanAksiHistory("Simpan Referensi " & TxtNama.Text)

            ' Reset kondisi awal
            Call KOndisiAwal()
            TxtNama.Select()
        Catch ex As Exception
            ' Rollback jika terjadi error
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message)
        End Try
    End Sub


    Private Sub ButtonHapus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHapus.Click
        Dim hapus As MsgBoxResult
        hapus = MsgBox("Apakah yakin akan menghapus data dengan Kode Akun " & TxtKode.Text & "?", MsgBoxStyle.OkCancel, MessageBoxIcon.Hand)
        If TxtStatus.Text = "Terkunci" Then
            MsgBox("Data dengan Kode Akun " & TxtKode.Text & " tidak dapat dihapus", , "Gagal")
        Else
            If hapus = MsgBoxResult.Ok Then
                If TxtNama.Text = "" Then
                    MsgBox("Karyawan belum dipilih", vbCritical, "Oops")
                    TxtNama.Focus()
                Else
                    Dim HapusData As String = "DELETE FROM tbl_datareferensi WHERE Kode_Akun = ?"
                    Using cmdDelete As New MySqlCommand(HapusData, conn)
                        cmdDelete.Parameters.AddWithValue("@KodeAkun", TxtKode.Text)
                        cmdDelete.ExecuteNonQuery()
                    End Using
                    MsgBox("Data dengan Kode Akun " & TxtKode.Text & " berhasil dihapus", , "Sukses")

                    DatabaseModule.CatatanAksiHistory("Hapus Referensi " & TxtNama.Text)
                End If
            End If
            Call KOndisiAwal()
        End If

    End Sub




    Private Sub Dgvdata_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Dgvdata.CellClick
        TxtKode.Enabled = False
        Dim i As Integer = e.RowIndex

        If i >= 0 AndAlso i < Dgvdata.Rows.Count - 1 Then ' Pastikan baris yang diklik valid
            TxtStatus.Text = Dgvdata.Item("status", i).Value.ToString()
            CmbJenisAkun.Text = Dgvdata.Item("Jenis_Akun", i).Value.ToString()
            CmbType.Text = Dgvdata.Item("Type_Akun", i).Value.ToString()
            TxtKode.Text = Dgvdata.Item("Kode_akun", i).Value.ToString()
            TxtNama.Text = Dgvdata.Item("Nama_Akun", i).Value.ToString()
            CmbSubAkun.Text = Dgvdata.Item("Sub_Akun", i).Value.ToString()
            CmbDK.Text = Dgvdata.Item("Akun_DK", i).Value.ToString()
            CmbNRLR.Text = Dgvdata.Item("Akun_NRLR", i).Value.ToString()

            Dim saldo_awal As Decimal

            If Decimal.TryParse(Dgvdata.Item("Saldo_Awal", i).Value.ToString(), saldo_awal) Then
                TxtSaldoAwal.Text = saldo_awal.ToString("0.##")
            Else
                TxtSaldoAwal.Text = "0"
            End If

        Else
            ' Reset semua kontrol jika baris tidak valid
            CmbType.Text = ""
            TxtKode.Text = ""
            TxtNama.Text = ""
            CmbSubAkun.Text = ""
            CmbDK.Text = ""
            CmbNRLR.Text = ""
            TxtSaldoAwal.Text = ""
        End If
    End Sub


    Private Sub BtnKOsong_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKOsong.Click
        Call KOndisiAwal()
    End Sub

    Private Sub TxtSaldoAwal_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtSaldoAwal.TextChanged
        Dim saldo_awal As Decimal

        If Decimal.TryParse(TxtSaldoAwal.Text, saldo_awal) Then
            Dim saldo_awal_formatted As String = saldo_awal.ToString("0.##")
            LblSAwal.Text = "Rp. " & saldo_awal_formatted
        Else
            LblSAwal.Text = "Rp. 0"
        End If

    End Sub


    Private Sub FormTabelReferensi_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2
                BtnSimpan.PerformClick()
            Case Keys.F3
                BtnKOsong.PerformClick()
            Case Keys.F4
                BtnHapus.PerformClick()
            Case Keys.Escape
                BtnClose.PerformClick()
        End Select
    End Sub


    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class