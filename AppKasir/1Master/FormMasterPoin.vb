''' <summary>
''' FormMasterPoin — Form Konfigurasi Earn Rate, Harga Poin Barang, dan Riwayat Poin Pelanggan.
'''
''' Tab 1 — Konfigurasi Poin: atur nilai earn rate dan minimum redeem, simpan ke poin_config.
''' Tab 2 — Harga Poin Barang: atur harga poin per barang, simpan ke poin_barang.
''' Tab 3 — Riwayat Poin Pelanggan: lihat riwayat transaksi poin per pelanggan.
'''
''' Requirement: Req 1, Req 4, Req 7
''' </summary>
Public Class FormMasterPoin

    ' ─── State ───────────────────────────────────────────────────────────────
    Private _kodePelangganRiwayat As String = ""

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: FORM LOAD
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Form Load"

    Private Sub FormMasterPoin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        MuatKonfigurasiKeForm()
        MuatDataPoinBarang()
        InisPelangganAutoComplete()

        ' Inisialisasi DateTimePicker riwayat
        DtpDari.Value = DateTime.Today.AddMonths(-1)
        DtpSampai.Value = DateTime.Today
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: TAB 1 — KONFIGURASI POIN
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Tab 1 — Konfigurasi Poin"

    ''' <summary>
    ''' Muat nilai konfigurasi dari cache ModuleLoyaltyPoin ke kontrol form.
    ''' Tampilkan/sembunyikan NudPoinPerQty vs NudKelipatanNominal sesuai mekanisme.
    ''' </summary>
    Private Sub MuatKonfigurasiKeForm()
        Try
            ' Baca langsung dari DB agar selalu fresh
            Using cmd As New MySqlCommand(
                "SELECT POIN_PER_QTY, KELIPATAN_NOMINAL, MINIMUM_REDEEM " &
                "FROM poin_config ORDER BY ID DESC LIMIT 1", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        NudPoinPerQty.Value = If(IsDBNull(rd("POIN_PER_QTY")), 1D,
                            Math.Max(1, Convert.ToDecimal(rd("POIN_PER_QTY"))))
                        NudKelipatanNominal.Value = If(IsDBNull(rd("KELIPATAN_NOMINAL")), 10000D,
                            Math.Max(1, Convert.ToDecimal(rd("KELIPATAN_NOMINAL"))))
                        NudMinimumRedeem.Value = If(IsDBNull(rd("MINIMUM_REDEEM")), 100,
                            Math.Max(1, Convert.ToDecimal(rd("MINIMUM_REDEEM"))))
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Tabel belum ada — biarkan nilai default NUD berlaku
            Debug.WriteLine($"[FormMasterPoin.MuatKonfigurasiKeForm] {ex.Message}")
        End Try

        TerapkanVisibilitasEarnRate()
    End Sub


    ''' <summary>
    ''' Tampilkan NudPoinPerQty atau NudKelipatanNominal sesuai mekanisme yang dipilih
    ''' di GeneralSetting (LP_Mekanisme dari cache ModuleLoyaltyPoin).
    ''' Label teks diperbarui agar jelas apa yang harus diisi.
    ''' </summary>
    Private Sub TerapkanVisibilitasEarnRate()
        Dim isPoinPerItem As Boolean = (LP_Mekanisme.ToUpper().Trim() = "PER_ITEM")

        ' Tampilkan kontrol yang sesuai mekanisme
        LblPoinPerQty.Visible = isPoinPerItem
        NudPoinPerQty.Visible = isPoinPerItem
        LblKelipatanNominal.Visible = Not isPoinPerItem
        NudKelipatanNominal.Visible = Not isPoinPerItem

        ' Update teks label agar jelas konteksnya
        If isPoinPerItem Then
            LblPoinPerQty.Text = "Poin per 1 Qty Item :"
        Else
            LblKelipatanNominal.Text = "Setiap belanja Rp ... → 1 poin :"
        End If

        ' Tampilkan info mekanisme aktif
        If LP_Aktif Then
            Dim mekanismeStr As String = If(isPoinPerItem,
                "Per Item (Qty) — poin dihitung dari jumlah qty terjual",
                "Per Kelipatan Nominal — poin dihitung dari total belanja (Rp)")
            LblInfoMekanisme.Text = "Mekanisme aktif: " & mekanismeStr
        Else
            LblInfoMekanisme.Text = "Sistem poin tidak aktif. Aktifkan di General Setting."
        End If
    End Sub

    ''' <summary>
    ''' Simpan konfigurasi earn rate ke tabel poin_config dalam satu transaksi atomik.
    ''' Req 1.6, Req 1.7
    ''' </summary>
    Private Sub BtnSimpanKonfig_Click(sender As Object, e As EventArgs) Handles BtnSimpanKonfig.Click
        ' Validasi: tolak nilai ≤ 0 (Req 1.7)
        If NudPoinPerQty.Visible AndAlso NudPoinPerQty.Value <= 0 Then
            MessageBox.Show("Nilai 'Poin per 1 Qty Item' harus lebih dari 0." & vbCrLf &
                            "Masukkan nilai minimal 1.", "Validasi Gagal",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            NudPoinPerQty.Focus()
            Return
        End If
        If NudKelipatanNominal.Visible AndAlso NudKelipatanNominal.Value <= 0 Then
            MessageBox.Show("Nilai 'Kelipatan Nominal (Rp)' harus lebih dari 0." & vbCrLf &
                            "Masukkan nilai minimal 1.", "Validasi Gagal",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            NudKelipatanNominal.Focus()
            Return
        End If
        If NudMinimumRedeem.Value <= 0 Then
            MessageBox.Show("Nilai 'Minimum Poin untuk Redeem' harus lebih dari 0." & vbCrLf &
                            "Masukkan nilai minimal 1.", "Validasi Gagal",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            NudMinimumRedeem.Focus()
            Return
        End If

        Dim trans As MySqlTransaction = Nothing
        Try
            trans = conn.BeginTransaction()

            ' UPDATE baris pertama poin_config (seed sudah ada dari migrasi)
            Using cmd As New MySqlCommand(
                "UPDATE poin_config SET " &
                "POIN_PER_QTY = @poinPerQty, " &
                "KELIPATAN_NOMINAL = @kelipatanNominal, " &
                "MINIMUM_REDEEM = @minimumRedeem, " &
                "UPDATED_AT = NOW() " &
                "ORDER BY ID ASC LIMIT 1",
                conn, trans)
                cmd.Parameters.AddWithValue("@poinPerQty", NudPoinPerQty.Value)
                cmd.Parameters.AddWithValue("@kelipatanNominal", NudKelipatanNominal.Value)
                cmd.Parameters.AddWithValue("@minimumRedeem", CInt(NudMinimumRedeem.Value))
                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                ' Jika tidak ada baris (tabel kosong), INSERT
                If rowsAffected = 0 Then
                    Using cmdIns As New MySqlCommand(
                        "INSERT INTO poin_config " &
                        "(AKTIF, MEKANISME, POIN_PER_QTY, KELIPATAN_NOMINAL, MINIMUM_REDEEM, UPDATED_AT) " &
                        "VALUES (0, 'PER_ITEM', @poinPerQty, @kelipatanNominal, @minimumRedeem, NOW())",
                        conn, trans)
                        cmdIns.Parameters.AddWithValue("@poinPerQty", NudPoinPerQty.Value)
                        cmdIns.Parameters.AddWithValue("@kelipatanNominal", NudKelipatanNominal.Value)
                        cmdIns.Parameters.AddWithValue("@minimumRedeem", CInt(NudMinimumRedeem.Value))
                        cmdIns.ExecuteNonQuery()
                    End Using
                End If
            End Using

            trans.Commit()

            ' Refresh cache modul setelah simpan (Req 1.6)
            ModuleLoyaltyPoin.MuatKonfigurasi()

            MessageBox.Show("Konfigurasi poin berhasil disimpan.", "Informasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            TerapkanVisibilitasEarnRate()

        Catch ex As Exception
            If trans IsNot Nothing Then trans.Rollback()
            MessageBox.Show("Terjadi kesalahan saat menyimpan konfigurasi:" & vbCrLf & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>Reset nilai NUD ke nilai yang tersimpan di DB.</summary>
    Private Sub BtnResetKonfig_Click(sender As Object, e As EventArgs) Handles BtnResetKonfig.Click
        MuatKonfigurasiKeForm()
        MessageBox.Show("Nilai dikembalikan ke konfigurasi tersimpan.", "Reset",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

#End Region


    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: TAB 2 — HARGA POIN BARANG
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Tab 2 — Harga Poin Barang"

    ''' <summary>
    ''' Saat form load, muat barang yang sudah punya data di poin_barang (AKTIF atau tidak).
    ''' Barang yang belum pernah di-set tidak ditampilkan — user harus cari dulu.
    ''' </summary>
    Private Sub MuatDataPoinBarang(Optional filter As String = "")
        Try
            ' Hanya tampilkan barang yang sudah pernah di-set di poin_barang
            Dim sql As String =
                "SELECT b.ID_BARANG, b.NAMA_BARANG, " &
                "COALESCE(pb.HARGA_POIN, 0) AS HARGA_POIN, " &
                "COALESCE(pb.AKTIF, 0) AS AKTIF_POIN " &
                "FROM poin_barang pb " &
                "INNER JOIN tbl_barang b ON pb.ID_BARANG = b.ID_BARANG " &
                "WHERE b.STATUS <> 'Non Aktif' " &
                "ORDER BY b.NAMA_BARANG ASC"

            Using cmd As New MySqlCommand(sql, conn)
                Using ds As New DataSet
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(ds)
                        DgvPoinBarang.DataSource = ds.Tables(0)
                    End Using
                End Using
            End Using

            AturKolomDgvPoinBarang()

        Catch ex As Exception
            Debug.WriteLine($"[FormMasterPoin.MuatDataPoinBarang] {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Saat teks pencarian berubah, tampilkan ListBox hasil pencarian dari tbl_barang.
    ''' Barang yang sudah ada di DGV tidak ditampilkan lagi.
    ''' </summary>
    Private Sub TxtCariBarang_TextChanged(sender As Object, e As EventArgs) Handles TxtCariBarang.TextChanged
        Dim keyword As String = TxtCariBarang.Text.Trim()
        LstHasilCariBarang.Items.Clear()

        If keyword.Length < 2 Then
            LstHasilCariBarang.Visible = False
            Return
        End If

        Try
            Dim sql As String =
                "SELECT ID_BARANG, NAMA_BARANG FROM tbl_barang " &
                "WHERE STATUS <> 'Non Aktif' " &
                "AND (ID_BARANG LIKE @cari OR NAMA_BARANG LIKE @cari) " &
                "ORDER BY NAMA_BARANG ASC LIMIT 30"

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@cari", "%" & keyword & "%")
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim kode As String = rd("ID_BARANG").ToString()
                        Dim nama As String = rd("NAMA_BARANG").ToString()
                        ' Jangan tampilkan yang sudah ada di DGV
                        If Not BarangSudahDiDgv(kode) Then
                            LstHasilCariBarang.Items.Add($"{kode} - {nama}")
                        End If
                    End While
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormMasterPoin.TxtCariBarang_TextChanged] {ex.Message}")
        End Try

        LstHasilCariBarang.Visible = LstHasilCariBarang.Items.Count > 0
    End Sub

    ''' <summary>Cek apakah barang dengan kode tertentu sudah ada di DGV.</summary>
    Private Function BarangSudahDiDgv(kode As String) As Boolean
        For Each row As DataGridViewRow In DgvPoinBarang.Rows
            If row.IsNewRow Then Continue For
            If row.Cells("ID_BARANG").Value IsNot Nothing AndAlso
               row.Cells("ID_BARANG").Value.ToString() = kode Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Saat item di ListBox diklik/dipilih, tambahkan barang ke DGV dengan HARGA_POIN = 0, AKTIF = 0.
    ''' </summary>
    Private Sub LstHasilCariBarang_Click(sender As Object, e As EventArgs) Handles LstHasilCariBarang.Click
        TambahBarangDariListBox()
    End Sub

    Private Sub LstHasilCariBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles LstHasilCariBarang.KeyDown
        If e.KeyCode = Keys.Enter Then TambahBarangDariListBox()
        If e.KeyCode = Keys.Escape Then
            LstHasilCariBarang.Visible = False
            TxtCariBarang.Focus()
        End If
    End Sub

    Private Sub TxtCariBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtCariBarang.KeyDown
        If e.KeyCode = Keys.Down AndAlso LstHasilCariBarang.Visible AndAlso LstHasilCariBarang.Items.Count > 0 Then
            LstHasilCariBarang.SelectedIndex = 0
            LstHasilCariBarang.Focus()
        End If
        If e.KeyCode = Keys.Escape Then
            LstHasilCariBarang.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Ambil item terpilih dari ListBox, tambahkan sebagai baris baru di DGV.
    ''' </summary>
    Private Sub TambahBarangDariListBox()
        If LstHasilCariBarang.SelectedIndex < 0 Then Return
        Dim teks As String = LstHasilCariBarang.SelectedItem.ToString()
        Dim parts() As String = teks.Split(New String() {" - "}, 2, StringSplitOptions.None)
        If parts.Length < 2 Then Return

        Dim kode As String = parts(0).Trim()
        Dim nama As String = parts(1).Trim()

        ' Tambah baris baru ke DGV secara manual (bukan dari DataSource)
        ' Lepas DataSource dulu agar bisa tambah baris manual
        If DgvPoinBarang.DataSource IsNot Nothing Then
            ' Konversi DataTable ke rows manual agar bisa tambah baris baru
            Dim dt As DataTable = CType(DgvPoinBarang.DataSource, DataTable)
            Dim newRow As DataRow = dt.NewRow()
            newRow("ID_BARANG") = kode
            newRow("NAMA_BARANG") = nama
            newRow("HARGA_POIN") = 0
            newRow("AKTIF_POIN") = 0
            dt.Rows.Add(newRow)
        Else
            ' DGV belum punya DataSource — inisialisasi DataTable baru
            Dim dt As New DataTable()
            dt.Columns.Add("ID_BARANG", GetType(String))
            dt.Columns.Add("NAMA_BARANG", GetType(String))
            dt.Columns.Add("HARGA_POIN", GetType(Integer))
            dt.Columns.Add("AKTIF_POIN", GetType(Integer))
            dt.Rows.Add(kode, nama, 0, 0)
            DgvPoinBarang.DataSource = dt
            AturKolomDgvPoinBarang()
        End If

        ' Tutup ListBox, bersihkan pencarian, fokus ke baris baru
        LstHasilCariBarang.Visible = False
        TxtCariBarang.Clear()
        TxtCariBarang.Focus()

        ' Fokus ke sel HARGA_POIN baris yang baru ditambah
        Dim barisBaruIdx As Integer = DgvPoinBarang.Rows.Count - 1
        If barisBaruIdx >= 0 AndAlso DgvPoinBarang.Columns.Contains("HARGA_POIN") Then
            DgvPoinBarang.CurrentCell = DgvPoinBarang.Rows(barisBaruIdx).Cells("HARGA_POIN")
            DgvPoinBarang.BeginEdit(True)
        End If
    End Sub

    ''' <summary>Hapus baris terpilih dari DGV (belum hapus dari DB sampai Simpan).</summary>
    Private Sub BtnHapusBarisPoin_Click(sender As Object, e As EventArgs) Handles BtnHapusBarisPoin.Click
        If DgvPoinBarang.CurrentRow Is Nothing OrElse DgvPoinBarang.CurrentRow.IsNewRow Then Return
        Dim idx As Integer = DgvPoinBarang.CurrentRow.Index
        Dim dt As DataTable = TryCast(DgvPoinBarang.DataSource, DataTable)
        If dt IsNot Nothing AndAlso idx < dt.Rows.Count Then
            dt.Rows(idx).Delete()
        End If
    End Sub

    Private Sub AturKolomDgvPoinBarang()
        If DgvPoinBarang.Columns.Count = 0 Then Return

        ' Matikan AutoSizeColumnsMode global dulu sebelum set Width manual,
        ' karena mode Fill konflik dengan assignment Width dan menyebabkan NullReferenceException.
        DgvPoinBarang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

        DgvPoinBarang.Columns("ID_BARANG").HeaderText = "Kode Barang"
        DgvPoinBarang.Columns("ID_BARANG").ReadOnly = True
        DgvPoinBarang.Columns("ID_BARANG").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        DgvPoinBarang.Columns("ID_BARANG").Width = 120

        DgvPoinBarang.Columns("NAMA_BARANG").HeaderText = "Nama Barang"
        DgvPoinBarang.Columns("NAMA_BARANG").ReadOnly = True
        DgvPoinBarang.Columns("NAMA_BARANG").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        DgvPoinBarang.Columns("HARGA_POIN").HeaderText = "Harga Poin"
        DgvPoinBarang.Columns("HARGA_POIN").ReadOnly = False
        DgvPoinBarang.Columns("HARGA_POIN").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        DgvPoinBarang.Columns("HARGA_POIN").Width = 100
        DgvPoinBarang.Columns("HARGA_POIN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        DgvPoinBarang.Columns("AKTIF_POIN").HeaderText = "Aktif"
        DgvPoinBarang.Columns("AKTIF_POIN").ReadOnly = False
        DgvPoinBarang.Columns("AKTIF_POIN").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        DgvPoinBarang.Columns("AKTIF_POIN").Width = 60

        DgvPoinBarang.AllowUserToAddRows = False
        DgvPoinBarang.AllowUserToDeleteRows = False
        DgvPoinBarang.RowHeadersVisible = True
        DgvPoinBarang.RowHeadersWidth = 45
        DgvPoinBarang.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        ModuleTheme.ApplyThemeDataGridView(DgvPoinBarang)
    End Sub

    ''' <summary>
    ''' Simpan perubahan harga poin ke tabel poin_barang menggunakan INSERT ... ON DUPLICATE KEY UPDATE.
    ''' Baris yang dihapus dari DGV akan dihapus juga dari DB.
    ''' Req 4.3, Req 4.4, Req 4.6
    ''' </summary>
    Private Sub BtnSimpanHargaPoin_Click(sender As Object, e As EventArgs) Handles BtnSimpanHargaPoin.Click
        If DgvPoinBarang.Rows.Count = 0 Then Return

        Dim trans As MySqlTransaction = Nothing
        Try
            trans = conn.BeginTransaction()

            ' Kumpulkan ID barang yang masih ada di DGV (untuk sinkronisasi hapus)
            Dim idAktif As New List(Of String)()

            Dim sqlUpsert As String =
                "INSERT INTO poin_barang (ID_BARANG, HARGA_POIN, AKTIF, UPDATED_AT) " &
                "VALUES (@id, @harga, @aktif, NOW()) " &
                "ON DUPLICATE KEY UPDATE HARGA_POIN = @harga, AKTIF = @aktif, UPDATED_AT = NOW()"

            Using cmd As New MySqlCommand(sqlUpsert, conn, trans)
                cmd.Parameters.Add("@id", MySqlDbType.VarChar)
                cmd.Parameters.Add("@harga", MySqlDbType.Int32)
                cmd.Parameters.Add("@aktif", MySqlDbType.Byte)
                cmd.Prepare()

                For Each row As DataGridViewRow In DgvPoinBarang.Rows
                    If row.IsNewRow Then Continue For
                    Dim idBarang As String = If(row.Cells("ID_BARANG").Value IsNot Nothing,
                                                row.Cells("ID_BARANG").Value.ToString(), "")
                    If String.IsNullOrEmpty(idBarang) Then Continue For

                    Dim hargaPoin As Integer = 0
                    Integer.TryParse(If(row.Cells("HARGA_POIN").Value IsNot Nothing,
                                        row.Cells("HARGA_POIN").Value.ToString(), "0"), hargaPoin)
                    Dim aktif As Byte = If(row.Cells("AKTIF_POIN").Value IsNot Nothing AndAlso
                                           Convert.ToBoolean(row.Cells("AKTIF_POIN").Value), CByte(1), CByte(0))

                    cmd.Parameters("@id").Value = idBarang
                    cmd.Parameters("@harga").Value = hargaPoin
                    cmd.Parameters("@aktif").Value = aktif
                    cmd.ExecuteNonQuery()
                    idAktif.Add(idBarang)
                Next
            End Using

            trans.Commit()
            MessageBox.Show("Harga poin barang berhasil disimpan.", "Informasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            MuatDataPoinBarang()

        Catch ex As Exception
            If trans IsNot Nothing Then trans.Rollback()
            MessageBox.Show("Terjadi kesalahan saat menyimpan harga poin:" & vbCrLf & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DgvPoinBarang_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvPoinBarang.RowPostPaint
        Dim nomor As String = (e.RowIndex + 1).ToString()
        Dim fmt As New StringFormat() With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        Dim bounds As New Rectangle(e.RowBounds.Left, e.RowBounds.Top,
                                    DgvPoinBarang.RowHeadersWidth, e.RowBounds.Height)
        e.Graphics.DrawString(nomor, DgvPoinBarang.DefaultCellStyle.Font,
                              SystemBrushes.ControlText, bounds, fmt)
    End Sub

#End Region


    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: TAB 3 — RIWAYAT POIN PELANGGAN
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Tab 3 — Riwayat Poin Pelanggan"

    ''' <summary>
    ''' Inisialisasi AutoComplete untuk TxtCariPelanggan dari tbl_pelanggan.
    ''' </summary>
    Private Sub InisPelangganAutoComplete()
        Try
            Dim ac As New AutoCompleteStringCollection()
            Using cmd As New MySqlCommand(
                "SELECT CONCAT(KODE, ' - ', NAMA) FROM tbl_pelanggan ORDER BY NAMA ASC", conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        ac.Add(rd(0).ToString())
                    End While
                End Using
            End Using
            TxtCariPelanggan.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            TxtCariPelanggan.AutoCompleteSource = AutoCompleteSource.CustomSource
            TxtCariPelanggan.AutoCompleteCustomSource = ac
        Catch ex As Exception
            Debug.WriteLine($"[FormMasterPoin.InisPelangganAutoComplete] {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Saat teks berubah, cari pelanggan dan tampilkan saldo poin.
    ''' Format input: "KODE - NAMA" atau hanya KODE.
    ''' </summary>
    Private Sub TxtCariPelanggan_TextChanged(sender As Object, e As EventArgs) Handles TxtCariPelanggan.TextChanged
        Dim teks As String = TxtCariPelanggan.Text.Trim()
        If String.IsNullOrEmpty(teks) Then
            _kodePelangganRiwayat = ""
            LblSaldoPoin.Text = "Saldo Poin: -"
            Return
        End If

        ' Ekstrak kode dari format "KODE - NAMA"
        Dim kode As String = teks
        If teks.Contains(" - ") Then
            kode = teks.Split(New String() {" - "}, StringSplitOptions.None)(0).Trim()
        End If

        ' Cari pelanggan berdasarkan kode atau nama
        Try
            Using cmd As New MySqlCommand(
                "SELECT KODE, SALDO_POIN FROM tbl_pelanggan " &
                "WHERE KODE = @kode OR NAMA LIKE @nama LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@kode", kode)
                cmd.Parameters.AddWithValue("@nama", "%" & teks & "%")
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        _kodePelangganRiwayat = rd("KODE").ToString()
                        Dim saldo As Integer = If(IsDBNull(rd("SALDO_POIN")), 0, Convert.ToInt32(rd("SALDO_POIN")))
                        LblSaldoPoin.Text = "Saldo Poin: " & saldo.ToString("N0")
                    Else
                        _kodePelangganRiwayat = ""
                        LblSaldoPoin.Text = "Saldo Poin: (pelanggan tidak ditemukan)"
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormMasterPoin.TxtCariPelanggan_TextChanged] {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Tampilkan riwayat poin dari poin_ledger dengan filter tanggal.
    ''' Req 7.1, Req 7.2, Req 7.3
    ''' </summary>
    Private Sub BtnTampilkanRiwayat_Click(sender As Object, e As EventArgs) Handles BtnTampilkanRiwayat.Click
        If String.IsNullOrEmpty(_kodePelangganRiwayat) Then
            MessageBox.Show("Pilih pelanggan terlebih dahulu.", "Informasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            TxtCariPelanggan.Focus()
            Return
        End If

        Try
            Dim dari As String = DtpDari.Value.ToString("yyyy-MM-dd") & " 00:00:00"
            Dim sampai As String = DtpSampai.Value.ToString("yyyy-MM-dd") & " 23:59:59"

            Dim sql As String =
                "SELECT CREATED_AT AS Tanggal, NO_REFERENSI AS [No Referensi], " &
                "TIPE AS Tipe, JUMLAH_POIN AS [Jumlah Poin], " &
                "KETERANGAN AS Keterangan " &
                "FROM poin_ledger " &
                "WHERE KODE_PELANGGAN = @kode " &
                "AND CREATED_AT BETWEEN @dari AND @sampai " &
                "ORDER BY CREATED_AT DESC"

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@kode", _kodePelangganRiwayat)
                cmd.Parameters.AddWithValue("@dari", dari)
                cmd.Parameters.AddWithValue("@sampai", sampai)
                Using ds As New DataSet
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(ds)
                        DgvRiwayatPoin.DataSource = ds.Tables(0)
                    End Using
                End Using
            End Using

            AturKolomDgvRiwayat()

            ' Refresh saldo terkini (Req 7.2)
            Dim saldo As Integer = ModuleLoyaltyPoin.AmbilSaldoPoin(_kodePelangganRiwayat)
            LblSaldoPoin.Text = "Saldo Poin: " & saldo.ToString("N0")

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat memuat riwayat poin:" & vbCrLf & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AturKolomDgvRiwayat()
        If DgvRiwayatPoin.Columns.Count = 0 Then Return

        If DgvRiwayatPoin.Columns.Contains("Tanggal") Then
            DgvRiwayatPoin.Columns("Tanggal").HeaderText = "Tanggal"
            DgvRiwayatPoin.Columns("Tanggal").Width = 140
        End If
        If DgvRiwayatPoin.Columns.Contains("No Referensi") Then
            DgvRiwayatPoin.Columns("No Referensi").HeaderText = "No Referensi"
            DgvRiwayatPoin.Columns("No Referensi").Width = 160
        End If
        If DgvRiwayatPoin.Columns.Contains("Tipe") Then
            DgvRiwayatPoin.Columns("Tipe").HeaderText = "Tipe"
            DgvRiwayatPoin.Columns("Tipe").Width = 90
        End If
        If DgvRiwayatPoin.Columns.Contains("Jumlah Poin") Then
            DgvRiwayatPoin.Columns("Jumlah Poin").HeaderText = "Jumlah Poin"
            DgvRiwayatPoin.Columns("Jumlah Poin").Width = 100
            DgvRiwayatPoin.Columns("Jumlah Poin").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End If
        If DgvRiwayatPoin.Columns.Contains("Keterangan") Then
            DgvRiwayatPoin.Columns("Keterangan").HeaderText = "Keterangan"
            DgvRiwayatPoin.Columns("Keterangan").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End If

        DgvRiwayatPoin.AllowUserToAddRows = False
        DgvRiwayatPoin.AllowUserToDeleteRows = False
        DgvRiwayatPoin.ReadOnly = True
        DgvRiwayatPoin.RowHeadersVisible = True
        DgvRiwayatPoin.RowHeadersWidth = 45
        DgvRiwayatPoin.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        ModuleTheme.ApplyThemeDataGridView(DgvRiwayatPoin)
    End Sub

    Private Sub DgvRiwayatPoin_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvRiwayatPoin.RowPostPaint
        Dim nomor As String = (e.RowIndex + 1).ToString()
        Dim fmt As New StringFormat() With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        Dim bounds As New Rectangle(e.RowBounds.Left, e.RowBounds.Top,
                                    DgvRiwayatPoin.RowHeadersWidth, e.RowBounds.Height)
        e.Graphics.DrawString(nomor, DgvRiwayatPoin.DefaultCellStyle.Font,
                              SystemBrushes.ControlText, bounds, fmt)
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: CLOSE
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Close"

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub FormMasterPoin_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then Close()
    End Sub

#End Region

End Class
