Public Class FormTabelReferensi

    ' ── State ───────────────────────────────────────────────────────────────
    Private _mode As String = ""   ' "TAMBAH" | "EDIT" | "VIEW"
    Private _hakEdit As Boolean = False
    Private _hakHapus As Boolean = False

    ' ── Panel visibility ────────────────────────────────────────────────────
    ' Default: PanelKonten selalu tampil saat ada baris terpilih (mode VIEW)
    ' Mode VIEW  : semua kontrol Enabled=False, RichTextBoxKet ReadOnly
    ' Mode EDIT  : semua kontrol Enabled=True, data terisi dari baris
    ' Mode TAMBAH: semua kontrol Enabled=True, semua kosong
    Private Sub TampilkanPanel(mode As String)
        _mode = mode
        PanelInput.Visible = True

        Dim editable As Boolean = (mode = "EDIT" OrElse mode = "TAMBAH")

        ' Header label + warna sesuai mode (gunakan tema)
        Select Case mode
            Case "TAMBAH"
                LblModePanel.Text = "   TAMBAH AKUN BARU"
                LblModePanel.BackColor = ModuleTheme.C(ModuleTheme.L_Success, ModuleTheme.D_Success)
                BtnSimpan.Text = "Simpan (F2)"
            Case "EDIT"
                LblModePanel.Text = "   EDIT AKUN"
                LblModePanel.BackColor = ModuleTheme.C(ModuleTheme.L_Warning, ModuleTheme.D_Warning)
                BtnSimpan.Text = "Update (F2)"
            Case "VIEW"
                LblModePanel.Text = "   DETAIL AKUN"
                LblModePanel.BackColor = ModuleTheme.C(ModuleTheme.L_Muted, ModuleTheme.D_Muted)
                BtnSimpan.Text = "Simpan (F2)"
        End Select

        CmbJenisAkun.Enabled = editable
        CmbType.Enabled = editable
        TxtKode.Enabled = (mode = "TAMBAH")
        TxtNama.Enabled = editable
        CmbSubAkun.Enabled = editable
        CmbDK.Enabled = editable
        CmbNRLR.Enabled = editable
        TxtSaldoAwal.Enabled = editable
        BtnSimpan.Visible = editable
        RichTextBoxKet.ReadOnly = Not editable
    End Sub

    Private Sub SembunyikanPanel()
        _mode = ""
        PanelInput.Visible = False
    End Sub

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
        RichTextBoxKet.Clear()
        SembunyikanPanel()

        If Not String.IsNullOrEmpty(CmbType.Text) AndAlso CmbType.SelectedIndex <> -1 Then
            GeneratePurchaseOrderNumber(CmbType.Text)
        End If

        TampilAkun()
        TampilkanPanel("TAMBAH")
        Me.Cursor = Cursors.Default
    End Sub


    Private Sub FormTabelReferensi_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)

        ' Sembunyikan panel saat awal
        SembunyikanPanel()

        Dim TabelReferensi As Boolean() = ModulHakAkses.BacaHakAksesDariCache("Tabel Referensi")
        _hakEdit = TabelReferensi(2)
        _hakHapus = TabelReferensi(3)

        ' Isi dropdown
        CmbJenisAkun.Items.AddRange(New String() {"ASET LANCAR", "ASET TETAP", "PASIVA", "MODAL", "PENJUALAN", "HPP", "BIAYA", "PENDAPATAN LAIN", "PAJAK"})
        CmbJenisAkunCari.Items.AddRange(New String() {"ASET LANCAR", "ASET TETAP", "PASIVA", "MODAL", "PENJUALAN", "HPP", "BIAYA", "PENDAPATAN LAIN", "PAJAK"})

        TampilAkun()
    End Sub

    Private Sub CmbJenisAkun_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CmbJenisAkun.SelectedIndexChanged
        ' Membersihkan CmbType setiap kali pilihan di CmbJenisAkun berubah
        CmbType.Items.Clear()

        ' Mengisi CmbType berdasarkan pilihan di CmbJenisAkun
        Select Case CmbJenisAkun.SelectedItem.ToString()
            Case "ASET LANCAR"
                CmbType.Items.AddRange(New String() {"KAS", "BANK", "PIUTANG", "A LANCAR", "PAJAK AL"})
            Case "ASET TETAP"
                CmbType.Items.AddRange(New String() {"A TETAP", "AKM PENY."})
            Case "PASIVA"
                CmbType.Items.AddRange(New String() {"HUTANG", "BEBAN", "PAJAK", "SOSIAL"})
            Case "MODAL"
                CmbType.Items.AddRange(New String() {"EKUITAS", "PRIVE", "LABA RUGI"})
            Case "PENJUALAN"
                CmbType.Items.AddRange(New String() {"PEND. KOTOR", "RETUR PEND.", "DISKON PEND."})
            Case "HPP"
                CmbType.Items.AddRange(New String() {"HPP POKOK", "ANGKUT BELI", "ANGKUT JUAL", "PENY. STOK", "DISKON BELI"})
            Case "BIAYA"
                CmbType.Items.Add("BIAYA")
            Case "PENDAPATAN LAIN"
                CmbType.Items.AddRange(New String() {"PEND. BUNGA", "PEND. LAIN"})
            Case "PAJAK"
                CmbType.Items.Add("B PAJAK")
        End Select
    End Sub

    Private Sub CmbType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbType.SelectedIndexChanged
        GeneratePurchaseOrderNumber(CmbType.Text)

        ' Membersihkan CmbType setiap kali pilihan di CmbJenisAkun berubah
        CmbSubAkun.Items.Clear()
        CmbSubAkun.Items.AddRange(New String() {"AKTIVA", "PASIVA", "LABA RUGI", "LABA", "RUGI"})

        ' Mengisi CmbType berdasarkan pilihan di CmbJenisAkun
        Select Case CmbType.SelectedItem.ToString()
            Case "KAS", "BANK", "PIUTANG", "A LANCAR", "PAJAK AL", "A TETAP", "AKM PENY."
                CmbSubAkun.SelectedIndex = 0  ' AKTIVA
            Case "HUTANG", "BEBAN", "PAJAK", "SOSIAL", "EKUITAS", "PRIVE"
                CmbSubAkun.SelectedIndex = 1  ' PASIVA
            Case "LABA RUGI"
                CmbSubAkun.SelectedIndex = 2  ' LABA RUGI
            Case "PEND. KOTOR", "RETUR PEND.", "DISKON PEND.", "HPP POKOK", "ANGKUT BELI", "ANGKUT JUAL", "PENY. STOK", "DISKON BELI", "PEND. BUNGA", "PEND. LAIN"
                CmbSubAkun.SelectedIndex = 3  ' LABA
            Case "BIAYA", "B PAJAK"
                CmbSubAkun.SelectedIndex = 4  ' RUGI
        End Select

        CmbDK.Items.Clear()
        CmbDK.Items.AddRange(New String() {"DEBET", "KREDIT"})

        Select Case CmbType.SelectedItem.ToString()
            Case "KAS", "BANK", "PIUTANG", "A LANCAR", "PAJAK AL", "A TETAP", "BIAYA", "B PAJAK",
                 "PEND. KOTOR", "RETUR PEND.", "DISKON PEND.",
                 "HPP POKOK", "ANGKUT BELI", "ANGKUT JUAL", "PENY. STOK"
                CmbDK.SelectedIndex = 0  ' DEBET
            Case "AKM PENY.", "HUTANG", "BEBAN", "PAJAK", "SOSIAL", "EKUITAS", "PRIVE", "LABA RUGI",
                 "PEND. BUNGA", "PEND. LAIN", "DISKON BELI"
                CmbDK.SelectedIndex = 1  ' KREDIT
        End Select

        CmbNRLR.Items.Clear()
        CmbNRLR.Items.AddRange(New String() {"NERACA", "LABA RUGI"})

        Select Case CmbType.SelectedItem.ToString()
            Case "KAS", "BANK", "PIUTANG", "A LANCAR", "PAJAK AL", "A TETAP", "AKM PENY.",
                 "HUTANG", "BEBAN", "PAJAK", "SOSIAL", "EKUITAS", "PRIVE", "LABA RUGI"
                CmbNRLR.SelectedIndex = 0  ' NERACA
            Case "PEND. KOTOR", "RETUR PEND.", "DISKON PEND.",
                 "HPP POKOK", "ANGKUT BELI", "ANGKUT JUAL", "PENY. STOK", "DISKON BELI",
                 "BIAYA", "B PAJAK", "PEND. BUNGA", "PEND. LAIN"
                CmbNRLR.SelectedIndex = 1  ' LABA RUGI
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
            Case "PAJAK AL" : prefix = "01.05."
            Case "A TETAP" : prefix = "02.01."
            Case "AKM PENY." : prefix = "02.02."
            Case "HUTANG" : prefix = "03.01."
            Case "BEBAN" : prefix = "03.02."
            Case "PAJAK" : prefix = "03.02."
            Case "SOSIAL" : prefix = "03.03."
            Case "EKUITAS" : prefix = "04.01."
            Case "PRIVE" : prefix = "04.02."
            Case "LABA RUGI" : prefix = "05.01."
            Case "PEND. KOTOR" : prefix = "05.02."
            Case "RETUR PEND." : prefix = "05.03."
            Case "DISKON PEND." : prefix = "05.04."
            Case "HPP POKOK" : prefix = "06.01."
            Case "ANGKUT BELI" : prefix = "06.02."
            Case "ANGKUT JUAL" : prefix = "06.03."
            Case "PENY. STOK" : prefix = "06.04."
            Case "DISKON BELI" : prefix = "06.05."
            Case "BIAYA" : prefix = "07.01."
            Case "PEND. BUNGA" : prefix = "08.01."
            Case "PEND. LAIN" : prefix = "08.01."
            Case "B PAJAK" : prefix = "09.01."
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
        Dim query As String = "SELECT STATUS, JENIS_AKUN, TYPE_AKUN, KODE_AKUN, NAMA_AKUN, SUB_AKUN, AKUN_DK, AKUN_NRLR, KETERANGAN, SALDO_AWAL, S_DEBET, S_KREDIT, SALDO_AKHIR FROM tbl_datareferensi ORDER BY KODE_AKUN"

        Using cmd As New MySqlCommand(query, conn)
            Using da As New MySqlDataAdapter(cmd)
                dtAkun.Clear()
                da.Fill(dtAkun)
            End Using
        End Using

        bsAkun.DataSource = dtAkun
        Dgvdata.DataSource = bsAkun
        SetupDataGridViewColumns()
    End Sub

    Private Sub CmbJenisAkunCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbJenisAkunCari.SelectedIndexChanged
        Dim query As String = "SELECT STATUS, JENIS_AKUN, TYPE_AKUN, KODE_AKUN, NAMA_AKUN, SUB_AKUN, AKUN_DK, AKUN_NRLR, KETERANGAN, SALDO_AWAL, S_DEBET, S_KREDIT, SALDO_AKHIR FROM tbl_datareferensi WHERE JENIS_AKUN LIKE @Jenis_Akun ORDER BY KODE_AKUN"

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
        With Dgvdata
            ' ── Sembunyikan kolom yang tidak perlu tampil ────────────────────
            Dim kolHidden As String() = {"STATUS", "JENIS_AKUN", "TYPE_AKUN", "SUB_AKUN", "KETERANGAN"}
            For Each col As String In kolHidden
                If .Columns.Contains(col) Then .Columns(col).Visible = False
            Next

            ' ── Kolom data yang tampil — atur header dan lebar ───────────────
            If .Columns.Contains("KODE_AKUN") Then
                .Columns("KODE_AKUN").HeaderText = "KODE"
                .Columns("KODE_AKUN").FillWeight = 55
            End If
            If .Columns.Contains("NAMA_AKUN") Then
                .Columns("NAMA_AKUN").HeaderText = "NAMA AKUN"
                .Columns("NAMA_AKUN").FillWeight = 200
            End If
            If .Columns.Contains("AKUN_DK") Then
                .Columns("AKUN_DK").HeaderText = "DK"
                .Columns("AKUN_DK").FillWeight = 40
            End If
            If .Columns.Contains("AKUN_NRLR") Then
                .Columns("AKUN_NRLR").HeaderText = "NR/LR"
                .Columns("AKUN_NRLR").FillWeight = 50
            End If
            If .Columns.Contains("SALDO_AWAL") Then
                .Columns("SALDO_AWAL").HeaderText = "S AWAL"
                .Columns("SALDO_AWAL").FillWeight = 80
            End If
            If .Columns.Contains("S_DEBET") Then
                .Columns("S_DEBET").HeaderText = "DEBET"
                .Columns("S_DEBET").FillWeight = 80
            End If
            If .Columns.Contains("S_KREDIT") Then
                .Columns("S_KREDIT").HeaderText = "KREDIT"
                .Columns("S_KREDIT").FillWeight = 80
            End If
            If .Columns.Contains("SALDO_AKHIR") Then
                .Columns("SALDO_AKHIR").HeaderText = "S AKHIR"
                .Columns("SALDO_AKHIR").FillWeight = 80
            End If

            ' ── Tambah kolom button di KANAN — hapus dulu agar tidak duplikat
            For Each nama As String In {"BTN_EDIT", "BTN_HAPUS"}
                If .Columns.Contains(nama) Then .Columns.Remove(nama)
            Next

            Dim btnEdit As New DataGridViewButtonColumn With {
                .Name = "BTN_EDIT",
                .HeaderText = "",
                .Text = "✏ Edit",
                .UseColumnTextForButtonValue = True,
                .Width = 75,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .Resizable = DataGridViewTriState.False,
                .DefaultCellStyle = New DataGridViewCellStyle With {
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
            }
            .Columns.Add(btnEdit)

            Dim btnHapus As New DataGridViewButtonColumn With {
                .Name = "BTN_HAPUS",
                .HeaderText = "",
                .Text = "🗑 Hapus",
                .UseColumnTextForButtonValue = True,
                .Width = 80,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .Resizable = DataGridViewTriState.False,
                .DefaultCellStyle = New DataGridViewCellStyle With {
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                          }
            }
            .Columns.Add(btnHapus)
        End With

        ' -- Pengaturan standar dan tema DGV --
        ModuleTheme.ApplyStandardDataGridViewSettings(Dgvdata)
        ' Override beberapa setting khusus untuk form ini
        Dgvdata.RowHeadersVisible = True
        Dgvdata.RowHeadersWidth = 38
        ModuleAngka.TerapkanFormatKolomAngka(Dgvdata, "SALDO_AWAL", "S_DEBET", "S_KREDIT", "SALDO_AKHIR")

        ' -- Terapkan tema -- warna header, background, seleksi --
        ModuleTheme.ApplyThemeDataGridView(Dgvdata)

        ' -- Warnai tombol per baris sesuai tema dan status --
        WarnaiTombolDgv()
    End Sub

    ''' <summary>Warnai tombol Edit (biru) dan Hapus (merah/abu) per baris sesuai status Terkunci.</summary>
    Private Sub WarnaiTombolDgv()
        If Not Dgvdata.Columns.Contains("BTN_EDIT") Then Return
        If Not Dgvdata.Columns.Contains("BTN_HAPUS") Then Return
        If Not Dgvdata.Columns.Contains("STATUS") Then Return

        For Each row As DataGridViewRow In Dgvdata.Rows
            If row.IsNewRow Then Continue For
            Dim terkunci As Boolean = row.Cells("STATUS").Value?.ToString() = "Terkunci"
            ModuleTheme.SetWarnaDgvBtnEdit(row.Cells("BTN_EDIT"), _hakEdit)
            ModuleTheme.SetWarnaDgvBtnHapus(row.Cells("BTN_HAPUS"), _hakHapus AndAlso Not terkunci)
        Next
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

                        ' ========================================
                        ' START: Audit Trail - Edit Tabel Referensi
                        ' ========================================
                        Dim sbSnapshot As New System.Text.StringBuilder()
                        Try
                            Using cmdSnap As New MySqlCommand(
                                "SELECT Kode_Akun, Jenis_Akun, Type_Akun, Nama_Akun, Sub_Akun, Akun_DK, Akun_NRLR, Saldo_awal, KETERANGAN " &
                                "FROM tbl_datareferensi WHERE Kode_Akun = @k LIMIT 1", conn, transaction)
                                cmdSnap.Parameters.AddWithValue("@k", TxtKode.Text)
                                Using rdSnap = cmdSnap.ExecuteReader()
                                    If rdSnap.Read() Then
                                        sbSnapshot.AppendLine("Kode Akun: " & rdSnap("Kode_Akun").ToString())
                                        sbSnapshot.AppendLine("Nama Akun: " & rdSnap("Nama_Akun").ToString())
                                        sbSnapshot.AppendLine("Jenis Akun: " & rdSnap("Jenis_Akun").ToString())
                                        sbSnapshot.AppendLine("Type Akun: " & rdSnap("Type_Akun").ToString())
                                        sbSnapshot.AppendLine("Sub Akun: " & rdSnap("Sub_Akun").ToString())
                                        sbSnapshot.AppendLine("DK: " & rdSnap("Akun_DK").ToString())
                                        sbSnapshot.AppendLine("NR/LR: " & rdSnap("Akun_NRLR").ToString())
                                        Dim saldoAwal As Decimal = ModuleAngka.ParseDecimal(rdSnap("Saldo_awal"))
                                        sbSnapshot.AppendLine("Saldo Awal: " & ModuleAngka.FormatRupiah(saldoAwal))
                                        sbSnapshot.AppendLine("Keterangan: " & rdSnap("KETERANGAN").ToString())
                                    End If
                                End Using
                            End Using
                        Catch
                            sbSnapshot.AppendLine("Gagal baca data sebelum edit")
                        End Try
                        ModuleAuditTrail.CatatAuditMaster("REF:" & TxtKode.Text, "EDIT", "Tabel Referensi", sbSnapshot.ToString(), trans:=transaction)
                        ' ========================================
                        ' END: Audit Trail - Edit Tabel Referensi
                        ' ========================================

                        ' Jika data sudah ada, lakukan pembaruan (Edit)
                        Dim EditData As String = "UPDATE tbl_datareferensi SET Jenis_Akun = @Jenis_Akun, Type_Akun = @TypeAkun, Nama_Akun = @NamaAkun, " &
                                             "Sub_Akun = @SubAkun, Akun_DK = @AkunDK, Akun_NRLR = @AkunNRLR, Saldo_awal = @SaldoAwal, KETERANGAN = @Keterangan WHERE Kode_Akun = @KodeAkun"
                        Using cmd As New MySqlCommand(EditData, conn, transaction)
                            cmd.Parameters.AddWithValue("@Jenis_Akun", CmbJenisAkun.Text)
                            cmd.Parameters.AddWithValue("@TypeAkun", CmbType.Text)
                            cmd.Parameters.AddWithValue("@NamaAkun", StrConv(TxtNama.Text, vbUpperCase))
                            cmd.Parameters.AddWithValue("@SubAkun", CmbSubAkun.Text)
                            cmd.Parameters.AddWithValue("@AkunDK", CmbDK.Text)
                            cmd.Parameters.AddWithValue("@AkunNRLR", CmbNRLR.Text)
                            cmd.Parameters.AddWithValue("@SaldoAwal", ModuleAngka.ParseDecimal(TxtSaldoAwal.Text))
                            cmd.Parameters.AddWithValue("@Keterangan", RichTextBoxKet.Text)
                            cmd.Parameters.AddWithValue("@KodeAkun", TxtKode.Text)
                            cmd.ExecuteNonQuery()
                        End Using
                    Else
                        ' Tutup DataReader sebelum insert
                        Rd.Close()

                        ' Jika data belum ada, lakukan insert
                        Dim InputData As String = "INSERT INTO tbl_datareferensi (Jenis_Akun, Type_Akun, Kode_Akun, Nama_Akun, Sub_Akun, Akun_DK, Akun_NRLR, Saldo_Awal, KETERANGAN) " &
                                              "VALUES (@Jenis_Akun, @TypeAkun, @Kode_Akun, @NamaAkun, @SubAkun, @AkunDK, @AkunNRLR, @Saldo_Awal, @Keterangan)"
                        Using cmd As New MySqlCommand(InputData, conn, transaction)
                            cmd.Parameters.AddWithValue("@Jenis_Akun", CmbJenisAkun.Text)
                            cmd.Parameters.AddWithValue("@TypeAkun", CmbType.Text)
                            cmd.Parameters.AddWithValue("@Kode_Akun", TxtKode.Text)
                            cmd.Parameters.AddWithValue("@NamaAkun", StrConv(TxtNama.Text, vbUpperCase))
                            cmd.Parameters.AddWithValue("@SubAkun", CmbSubAkun.Text)
                            cmd.Parameters.AddWithValue("@AkunDK", CmbDK.Text)
                            cmd.Parameters.AddWithValue("@AkunNRLR", CmbNRLR.Text)
                            cmd.Parameters.AddWithValue("@Saldo_Awal", ModuleAngka.ParseDecimal(TxtSaldoAwal.Text))
                            cmd.Parameters.AddWithValue("@Keterangan", RichTextBoxKet.Text)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                End Using
            End Using


            transaction.Commit()
            ' Catatan aksi dan komit transaksi

            ' Reset kondisi awal
            Call KOndisiAwal()
            TxtNama.Select()
        Catch ex As Exception
            ' Rollback jika terjadi error
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message)
        End Try
    End Sub







    Private Sub Dgvdata_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs) Handles Dgvdata.SelectionChanged
        ' Saat baris berubah (termasuk panah atas/bawah), tampilkan detail dalam mode VIEW
        ' Kecuali sedang dalam mode EDIT atau TAMBAH agar tidak override data yang sedang diisi
        If _mode = "EDIT" OrElse _mode = "TAMBAH" Then Return
        If Dgvdata.CurrentRow Is Nothing OrElse Dgvdata.CurrentRow.IsNewRow Then Return

        MuatDataDariRow(Dgvdata.CurrentRow)
        TampilkanPanel("VIEW")
    End Sub

    Private Sub Dgvdata_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles Dgvdata.CellContentClick
        If e.RowIndex < 0 Then Return
        Dim row As DataGridViewRow = Dgvdata.Rows(e.RowIndex)
        Dim col As String = Dgvdata.Columns(e.ColumnIndex).Name

        If col = "BTN_EDIT" Then
            If Not _hakEdit Then Return
            MuatDataDariRow(row)
            TxtKode.Enabled = False
            TampilkanPanel("EDIT")

        ElseIf col = "BTN_HAPUS" Then
            If Not _hakHapus Then Return
            If row.Cells("STATUS").Value?.ToString() = "Terkunci" Then
                MessageBox.Show("Akun ini terkunci dan tidak dapat dihapus.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            Dim kode As String = row.Cells("KODE_AKUN").Value?.ToString()
            Dim nama As String = row.Cells("NAMA_AKUN").Value?.ToString()
            If MessageBox.Show("Hapus akun " & kode & " - " & nama & "?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                Dim transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    ' ========================================
                    ' START: Audit Trail - Hapus Tabel Referensi
                    ' ========================================
                    Dim sbSnapshot As New System.Text.StringBuilder()
                    Using cmdSnap As New MySqlCommand(
                        "SELECT Kode_Akun, Jenis_Akun, Type_Akun, Nama_Akun, Sub_Akun, Akun_DK, Akun_NRLR, Saldo_awal, KETERANGAN " &
                        "FROM tbl_datareferensi WHERE Kode_Akun = @k LIMIT 1", conn, transaction)
                        cmdSnap.Parameters.AddWithValue("@k", kode)
                        Using rdSnap = cmdSnap.ExecuteReader()
                            If rdSnap.Read() Then
                                sbSnapshot.AppendLine("Kode Akun: " & rdSnap("Kode_Akun").ToString())
                                sbSnapshot.AppendLine("Nama Akun: " & rdSnap("Nama_Akun").ToString())
                                sbSnapshot.AppendLine("Jenis Akun: " & rdSnap("Jenis_Akun").ToString())
                                sbSnapshot.AppendLine("Type Akun: " & rdSnap("Type_Akun").ToString())
                                sbSnapshot.AppendLine("Sub Akun: " & rdSnap("Sub_Akun").ToString())
                                sbSnapshot.AppendLine("DK: " & rdSnap("Akun_DK").ToString())
                                sbSnapshot.AppendLine("NR/LR: " & rdSnap("Akun_NRLR").ToString())
                                Dim saldoAwal As Decimal = ModuleAngka.ParseDecimal(rdSnap("Saldo_awal"))
                                sbSnapshot.AppendLine("Saldo Awal: " & ModuleAngka.FormatRupiah(saldoAwal))
                                sbSnapshot.AppendLine("Keterangan: " & rdSnap("KETERANGAN").ToString())
                            End If
                        End Using
                    End Using
                    ModuleAuditTrail.CatatAuditMaster("REF:" & kode, "HAPUS", "Tabel Referensi", sbSnapshot.ToString(), trans:=transaction)
                    ' ========================================
                    ' END: Audit Trail - Hapus Tabel Referensi
                    ' ========================================

                    Using cmd As New MySqlCommand("DELETE FROM tbl_datareferensi WHERE Kode_Akun = @k", conn, transaction)
                        cmd.Parameters.AddWithValue("@k", kode)
                        cmd.ExecuteNonQuery()
                    End Using
                    transaction.Commit()
                    KOndisiAwal()
                Catch ex As Exception
                    transaction.Rollback()
                    MessageBox.Show("Terjadi kesalahan: " & ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Sub MuatDataDariRow(row As DataGridViewRow)
        TxtStatus.Text = row.Cells("STATUS").Value?.ToString()
        CmbJenisAkun.Text = row.Cells("JENIS_AKUN").Value?.ToString()
        CmbType.Text = row.Cells("TYPE_AKUN").Value?.ToString()
        TxtKode.Text = row.Cells("KODE_AKUN").Value?.ToString()
        TxtNama.Text = row.Cells("NAMA_AKUN").Value?.ToString()
        CmbSubAkun.Text = row.Cells("SUB_AKUN").Value?.ToString()
        CmbDK.Text = row.Cells("AKUN_DK").Value?.ToString()
        CmbNRLR.Text = row.Cells("AKUN_NRLR").Value?.ToString()

        Dim saldoAwal As Decimal
        If Decimal.TryParse(row.Cells("SALDO_AWAL").Value?.ToString(), saldoAwal) Then
            TxtSaldoAwal.Text = saldoAwal.ToString("0.##")
        Else
            TxtSaldoAwal.Text = "0"
        End If

        ' Isi keterangan — kolom KETERANGAN selalu ada di query
        RichTextBoxKet.Text = row.Cells("KETERANGAN").Value?.ToString()
    End Sub


    Private Sub Dgvdata_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles Dgvdata.RowPostPaint
        ' Gambar nomor urut di RowHeader — konsisten dengan form lain
        Dim nomor As String = (e.RowIndex + 1).ToString()
        Dim centerFormat As New StringFormat() With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        Dim headerBounds As New Rectangle(
            e.RowBounds.Left,
            e.RowBounds.Top,
            Dgvdata.RowHeadersWidth,
            e.RowBounds.Height)
        Using b As New SolidBrush(Dgvdata.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString(nomor, Dgvdata.DefaultCellStyle.Font, b, headerBounds, centerFormat)
        End Using
    End Sub

    Private Sub BtnKOsong_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKOsong.Click
        If Not _hakEdit Then Return
        Call KOndisiAwal()
        TampilkanPanel("TAMBAH")
        TxtKode.Enabled = True
    End Sub

    Private Sub TxtSaldoAwal_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtSaldoAwal.TextChanged
        Dim saldo_awal As Decimal
        If Decimal.TryParse(TxtSaldoAwal.Text, saldo_awal) Then
            LblSAwal.Text = "Rp. " & saldo_awal.ToString("#,0.##", Globalization.CultureInfo.GetCultureInfo("id-ID"))
        Else
            LblSAwal.Text = "Rp. 0"
        End If
    End Sub


    Private Sub FormTabelReferensi_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F2 : BtnSimpan.PerformClick()
            Case Keys.F4 : BtnKOsong.PerformClick()
            Case Keys.Escape : BtnClose.PerformClick()
        End Select
    End Sub


    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub
End Class