''' <summary>
''' FormTukarPoin — Form Penukaran Poin Pelanggan dengan Barang.
'''
''' Kasir memilih pelanggan, memilih barang yang ingin ditukar beserta qty,
''' lalu mengkonfirmasi penukaran. Sistem mencatat REDEEM ke poin_ledger,
''' mengurangi SALDO_POIN pelanggan, dan mengurangi stok barang — dalam
''' satu transaksi database atomik.
'''
''' Requirement: Req 3
''' </summary>
Public Class FormTukarPoin

    ' ─── State ───────────────────────────────────────────────────────────────
    Private _kodePelanggan As String = ""
    Private _namaPelanggan As String = ""
    Private _saldoPoin As Integer = 0

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: FORM LOAD
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Form Load"

    Private Sub FormTukarPoin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)

        MuatBarangTukar()
        UpdateTombolKonfirmasi()

        If TxtJenisTrans.Text = "EditTukarPoin" Then
            MuatDataEdit()
        End If
    End Sub

    Private Sub FormTukarPoin_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        TxtPelanggan.Focus()
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: PILIH PELANGGAN
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Pilih Pelanggan"

    ''' <summary>Cari pelanggan via ListBox (sama metode seperti di master).</summary>
    Private Sub TxtPelanggan_TextChanged(sender As Object, e As EventArgs) Handles TxtPelanggan.TextChanged
        Dim keyword As String = TxtPelanggan.Text.Trim()
        LstHasilCariPelanggan.Items.Clear()

        If String.IsNullOrEmpty(keyword) Then
            LstHasilCariPelanggan.Visible = False
            ResetPelanggan()
            Return
        End If

        Try
            Using cmd As New MySqlCommand(
                "SELECT NAMA FROM tbl_pelanggan " &
                "WHERE Status <> 'Non Aktif' " &
                "AND (KODE LIKE @cari OR NAMA LIKE @cari) " &
                "ORDER BY NAMA ASC LIMIT 30", conn)
                cmd.Parameters.AddWithValue("@cari", "%" & keyword & "%")
                Using dt As New DataTable
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                    For Each row As DataRow In dt.Rows
                        LstHasilCariPelanggan.Items.Add(row("NAMA").ToString())
                    Next
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormTukarPoin.TxtPelanggan_TextChanged] {ex.Message}")
        End Try

        If LstHasilCariPelanggan.Items.Count > 0 Then
            LstHasilCariPelanggan.Visible = True
            LstHasilCariPelanggan.BringToFront()
        Else
            LstHasilCariPelanggan.Visible = False
            ResetPelanggan()
        End If
    End Sub

    Private Sub LstHasilCariPelanggan_Click(sender As Object, e As EventArgs) Handles LstHasilCariPelanggan.Click
        PilihPelangganDariListBox()
    End Sub

    Private Sub LstHasilCariPelanggan_KeyDown(sender As Object, e As KeyEventArgs) Handles LstHasilCariPelanggan.KeyDown
        If e.KeyCode = Keys.Enter Then PilihPelangganDariListBox()
        If e.KeyCode = Keys.Escape Then
            LstHasilCariPelanggan.Visible = False
            TxtPelanggan.Focus()
        End If
    End Sub

    Private Sub TxtPelanggan_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtPelanggan.KeyDown
        If e.KeyCode = Keys.Down AndAlso LstHasilCariPelanggan.Visible AndAlso LstHasilCariPelanggan.Items.Count > 0 Then
            LstHasilCariPelanggan.SelectedIndex = 0
            LstHasilCariPelanggan.Focus()
        End If
        If e.KeyCode = Keys.Escape Then
            LstHasilCariPelanggan.Visible = False
        End If
    End Sub

    Private Sub PilihPelangganDariListBox()
        If LstHasilCariPelanggan.SelectedIndex < 0 Then Return
        Dim nama As String = LstHasilCariPelanggan.SelectedItem.ToString()
        Dim kode As String = ""
        Dim saldo As Integer = 0

        Try
            Using cmd As New MySqlCommand(
                "SELECT KODE, NAMA, SALDO_POIN FROM tbl_pelanggan " &
                "WHERE NAMA = @nama LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@nama", nama)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        kode = rd("KODE").ToString()
                        _namaPelanggan = rd("NAMA").ToString()
                        saldo = If(IsDBNull(rd("SALDO_POIN")), 0, Convert.ToInt32(rd("SALDO_POIN")))
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormTukarPoin.PilihPelangganDariListBox] {ex.Message}")
        End Try

        ' Reader closed — baru update UI
        If Not String.IsNullOrEmpty(kode) Then
            _kodePelanggan = kode
            _saldoPoin = saldo
            TxtPelanggan.Text = _namaPelanggan
            LblKodePelanggan.Text = "(" & kode & ")"
            TampilkanSaldoPoin()
        End If

        LstHasilCariPelanggan.Visible = False
        If DgvBarangTukar.Rows.Count > 0 Then
            DgvBarangTukar.Focus()
            If DgvBarangTukar.Columns.Contains("PILIH") Then
                DgvBarangTukar.CurrentCell = DgvBarangTukar.Rows(0).Cells("PILIH")
            End If
        Else
            TxtPelanggan.Focus()
        End If
    End Sub

    Private Sub ResetPelanggan()
        _kodePelanggan = ""
        _namaPelanggan = ""
        _saldoPoin = 0
        LblKodePelanggan.Text = ""
        LblSaldoPoinTukar.Text = "Saldo Poin: -"
        UpdateTombolKonfirmasi()
        UpdateRingkasanPoin()
    End Sub

    ''' <summary>
    ''' Tampilkan saldo poin.
    ''' Req 3.3
    ''' </summary>
    Private Sub TampilkanSaldoPoin()
        LblSaldoPoinTukar.Text = "Saldo Poin: " & _saldoPoin.ToString("N0")
        UpdateTombolKonfirmasi()
        UpdateRingkasanPoin()
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: LOAD BARANG TUKAR
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Load Barang Tukar"

    ''' <summary>
    ''' Load daftar barang yang dapat ditukar dari JOIN poin_barang INNER JOIN tbl_barang.
    ''' Req 3.5, Req 4.5
    ''' </summary>
    Private Sub MuatBarangTukar()
        Try
            ' Pilih kolom stok sesuai lokasi login, mengikuti pola form lain (FormStokOpname, TambahBarang, dll.)
            Dim kolStok As String = If(FormUtama.StatusLokasi.Text = "GUDANG", "STOK_GUDANG", "STOK_TOKO")

            Dim sql As String =
                $"SELECT b.ID_BARANG, b.NAMA_BARANG, b.{kolStok} AS STOK, b.SATUAN_STOK, pb.HARGA_POIN " &
                "FROM poin_barang pb " &
                "INNER JOIN tbl_barang b ON pb.ID_BARANG = b.ID_BARANG " &
                "WHERE pb.AKTIF = 1 AND pb.HARGA_POIN > 0 " &
                "ORDER BY b.NAMA_BARANG ASC"

            Using cmd As New MySqlCommand(sql, conn)
                Using ds As New DataSet
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(ds)
                        DgvBarangTukar.DataSource = ds.Tables(0)
                    End Using
                End Using
            End Using

            AturKolomDgvBarangTukar()

        Catch ex As Exception
            Debug.WriteLine($"[FormTukarPoin.MuatBarangTukar] {ex.Message}")
        End Try
    End Sub

    Private Sub AturKolomDgvBarangTukar()
        If DgvBarangTukar.Columns.Count = 0 Then Return

        DgvBarangTukar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DgvBarangTukar.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DgvBarangTukar.ColumnHeadersHeight = 26

        ' Tambah kolom PILIH (checkbox) jika belum ada
        If Not DgvBarangTukar.Columns.Contains("PILIH") Then
            Dim colPilih As New DataGridViewCheckBoxColumn()
            colPilih.Name = "PILIH"
            colPilih.HeaderText = ""
            colPilih.FillWeight = 30
            colPilih.MinimumWidth = 20
            colPilih.ReadOnly = False
            DgvBarangTukar.Columns.Insert(0, colPilih)
        End If

        ' Sembunyikan kolom data source, tampilkan kolom yang relevan
        DgvBarangTukar.Columns("ID_BARANG").Visible = False

        DgvBarangTukar.Columns("NAMA_BARANG").HeaderText = "Nama Barang"
        DgvBarangTukar.Columns("NAMA_BARANG").ReadOnly = True
        DgvBarangTukar.Columns("NAMA_BARANG").FillWeight = 414

        DgvBarangTukar.Columns("STOK").HeaderText = "Stok"
        DgvBarangTukar.Columns("STOK").ReadOnly = True
        DgvBarangTukar.Columns("STOK").FillWeight = 60
        DgvBarangTukar.Columns("STOK").MinimumWidth = 40
        DgvBarangTukar.Columns("STOK").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        DgvBarangTukar.Columns("SATUAN_STOK").HeaderText = "Satuan"
        DgvBarangTukar.Columns("SATUAN_STOK").ReadOnly = True
        DgvBarangTukar.Columns("SATUAN_STOK").FillWeight = 60
        DgvBarangTukar.Columns("SATUAN_STOK").MinimumWidth = 40
        DgvBarangTukar.Columns("SATUAN_STOK").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        DgvBarangTukar.Columns("HARGA_POIN").HeaderText = "Harga Poin"
        DgvBarangTukar.Columns("HARGA_POIN").ReadOnly = True
        DgvBarangTukar.Columns("HARGA_POIN").FillWeight = 80
        DgvBarangTukar.Columns("HARGA_POIN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        ' Tambah kolom Qty (editable) jika belum ada
        If Not DgvBarangTukar.Columns.Contains("QTY") Then
            Dim colQty As New DataGridViewTextBoxColumn()
            colQty.Name = "QTY"
            colQty.HeaderText = "Qty"
            colQty.FillWeight = 60
            colQty.MinimumWidth = 40
            colQty.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DgvBarangTukar.Columns.Add(colQty)
        End If

        ' Tambah kolom Total Poin (read-only, dihitung otomatis) jika belum ada
        If Not DgvBarangTukar.Columns.Contains("TOTAL_POIN") Then
            Dim colTotal As New DataGridViewTextBoxColumn()
            colTotal.Name = "TOTAL_POIN"
            colTotal.HeaderText = "Total Poin"
            colTotal.FillWeight = 80
            colTotal.ReadOnly = True
            colTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DgvBarangTukar.Columns.Add(colTotal)
        End If

        ' Inisialisasi PILIH = False, Qty = 0, Total Poin = 0
        For Each row As DataGridViewRow In DgvBarangTukar.Rows
            If row.IsNewRow Then Continue For
            If row.Cells("PILIH").Value Is Nothing Then row.Cells("PILIH").Value = False
            If row.Cells("QTY").Value Is Nothing OrElse row.Cells("QTY").Value.ToString() = "" Then
                row.Cells("QTY").Value = 0
            End If
            row.Cells("TOTAL_POIN").Value = 0
        Next

        DgvBarangTukar.AllowUserToAddRows = False
        DgvBarangTukar.AllowUserToDeleteRows = False
        DgvBarangTukar.RowHeadersVisible = True
        DgvBarangTukar.RowHeadersWidth = 40
        DgvBarangTukar.RowTemplate.Height = 24
        DgvBarangTukar.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        ModuleTheme.ApplyThemeDataGridView(DgvBarangTukar)
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: LOAD DATA EDIT
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Load Data Edit"

    ''' <summary>
    ''' Muat data transaksi Tukar Poin yang sudah ada untuk diedit.
    ''' Dipanggil dari FormTukarPoin_Load saat TxtJenisTrans = "EditTukarPoin".
    ''' </summary>
    Private Sub MuatDataEdit()
        ' ── Step 1: Baca header dari poin_ledger + data pelanggan ────────
        Dim kodePelanggan As String = ""
        Dim namaPelanggan As String = ""
        Dim poinLedger As Integer = 0
        Dim saldoPelanggan As Integer = 0

        Try
            Using cmd As New MySqlCommand(
                "SELECT pl.KODE_PELANGGAN, pl.JUMLAH_POIN, p.NAMA, p.SALDO_POIN " &
                "FROM poin_ledger pl " &
                "LEFT JOIN tbl_pelanggan p ON pl.KODE_PELANGGAN = p.KODE " &
                "WHERE pl.NO_REFERENSI = @faktur AND pl.TIPE = 'REDEEM' LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        kodePelanggan = rd("KODE_PELANGGAN").ToString()
                        namaPelanggan = If(IsDBNull(rd("NAMA")), "", rd("NAMA").ToString())
                        poinLedger = If(IsDBNull(rd("JUMLAH_POIN")), 0, Convert.ToInt32(rd("JUMLAH_POIN")))
                        saldoPelanggan = If(IsDBNull(rd("SALDO_POIN")), 0, Convert.ToInt32(rd("SALDO_POIN")))
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormTukarPoin.MuatDataEdit] Header: {ex.Message}")
        End Try

        If String.IsNullOrEmpty(kodePelanggan) Then
            MessageBox.Show("Data transaksi Tukar Poin tidak ditemukan.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Close()
            Return
        End If

        ' Saldo efektif = saldo saat ini + poin yang digunakan (kembalikan dulu)
        ' JUMLAH_POIN di ledger untuk REDEEM negatif, jadi saldoPelanggan - poinLedger = pre-redeem
        _saldoPoin = saldoPelanggan - poinLedger
        _kodePelanggan = kodePelanggan
        _namaPelanggan = namaPelanggan
        TxtPelanggan.Text = namaPelanggan
        LblKodePelanggan.Text = "(" & kodePelanggan & ")"
        TampilkanSaldoPoin()

        ' ── Step 2: Load qty item dari HistoryBarang ────────────────────
        Try
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG, TOTAL_QTY FROM HistoryBarang " &
                "WHERE FAKTUR = @faktur AND JENIS = 'KURANG'", conn)
                cmd.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim idBarang As String = rd("ID_BARANG").ToString()
                        Dim totalQty As Integer = If(IsDBNull(rd("TOTAL_QTY")), 0, Convert.ToInt32(rd("TOTAL_QTY")))

                        For Each row As DataGridViewRow In DgvBarangTukar.Rows
                            If row.IsNewRow Then Continue For
                            If row.Cells("ID_BARANG").Value IsNot Nothing AndAlso
                               row.Cells("ID_BARANG").Value.ToString() = idBarang Then
                                row.Cells("PILIH").Value = True
                                row.Cells("QTY").Value = totalQty
                                HitungTotalPoinBaris(row.Index)
                                Exit For
                            End If
                        Next
                    End While
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormTukarPoin.MuatDataEdit] Detail: {ex.Message}")
        End Try

        UpdateRingkasanPoin()
        UpdateTombolKonfirmasi()
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: EDIT QTY DI GRID — UPDATE REAL-TIME
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Edit Qty Real-Time"

    ''' <summary>
    ''' Commit edit segera saat checkbox diklik agar CellValueChanged terpicu.
    ''' </summary>
    Private Sub DgvBarangTukar_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DgvBarangTukar.CellContentClick
        If e.RowIndex < 0 Then Return
        If DgvBarangTukar.Columns.Contains("PILIH") AndAlso e.ColumnIndex = DgvBarangTukar.Columns("PILIH").Index Then
            DgvBarangTukar.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    ''' <summary>
    ''' Cegah editing Qty jika checkbox PILIH tidak dicentang.
    ''' </summary>
    Private Sub DgvBarangTukar_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles DgvBarangTukar.CellBeginEdit
        If e.RowIndex < 0 Then Return
        If Not DgvBarangTukar.Columns.Contains("PILIH") Then Return
        If Not DgvBarangTukar.Columns.Contains("QTY") Then Return
        If e.ColumnIndex <> DgvBarangTukar.Columns("QTY").Index Then Return

        Dim isChecked As Boolean = False
        If DgvBarangTukar.Rows(e.RowIndex).Cells("PILIH").Value IsNot Nothing Then
            Boolean.TryParse(DgvBarangTukar.Rows(e.RowIndex).Cells("PILIH").Value.ToString(), isChecked)
        End If
        If Not isChecked Then
            e.Cancel = True
        End If
    End Sub

    ''' <summary>
    ''' Saat nilai sel berubah, hitung ulang Total Poin baris tersebut
    ''' dan update ringkasan total poin dibutuhkan + sisa poin.
    ''' Req 3.6, Req 3.7
    ''' </summary>
    Private Sub DgvBarangTukar_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DgvBarangTukar.CellValueChanged
        If e.RowIndex < 0 Then Return
        If DgvBarangTukar.Columns.Count = 0 Then Return

        ' ── PILIH checkbox berubah — toggle Qty ──
        If DgvBarangTukar.Columns.Contains("PILIH") AndAlso e.ColumnIndex = DgvBarangTukar.Columns("PILIH").Index Then
            Dim isChecked As Boolean = False
            If DgvBarangTukar.Rows(e.RowIndex).Cells("PILIH").Value IsNot Nothing Then
                Boolean.TryParse(DgvBarangTukar.Rows(e.RowIndex).Cells("PILIH").Value.ToString(), isChecked)
            End If
            If isChecked Then
                Dim qty As Integer = 0
                If DgvBarangTukar.Rows(e.RowIndex).Cells("QTY").Value IsNot Nothing Then
                    Integer.TryParse(DgvBarangTukar.Rows(e.RowIndex).Cells("QTY").Value.ToString(), qty)
                End If
                If qty = 0 Then DgvBarangTukar.Rows(e.RowIndex).Cells("QTY").Value = 1
            Else
                DgvBarangTukar.Rows(e.RowIndex).Cells("QTY").Value = 0
            End If
            HitungTotalPoinBaris(e.RowIndex)
            UpdateRingkasanPoin()
            UpdateTombolKonfirmasi()
            Return
        End If

        ' ── QTY berubah — hitung ulang poin ──
        If Not DgvBarangTukar.Columns.Contains("QTY") Then Return
        If e.ColumnIndex <> DgvBarangTukar.Columns("QTY").Index Then Return

        HitungTotalPoinBaris(e.RowIndex)
        UpdateRingkasanPoin()
        UpdateTombolKonfirmasi()
    End Sub

    ''' <summary>
    ''' Validasi input Qty: hanya angka non-negatif, tidak melebihi stok.
    ''' </summary>
    Private Sub DgvBarangTukar_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles DgvBarangTukar.CellValidating
        If e.RowIndex < 0 Then Return
        If Not DgvBarangTukar.Columns.Contains("QTY") Then Return
        If e.ColumnIndex <> DgvBarangTukar.Columns("QTY").Index Then Return

        Dim val As Integer = 0
        If Not Integer.TryParse(e.FormattedValue.ToString(), val) OrElse val < 0 Then
            e.Cancel = True
            DgvBarangTukar.Rows(e.RowIndex).ErrorText = "Qty harus berupa angka ≥ 0"
            Return
        End If

        ' Cek stok
        Dim stok As Integer = 0
        If DgvBarangTukar.Columns.Contains("STOK") Then
            Integer.TryParse(If(DgvBarangTukar.Rows(e.RowIndex).Cells("STOK").Value IsNot Nothing,
                                DgvBarangTukar.Rows(e.RowIndex).Cells("STOK").Value.ToString(), "0"), stok)
        End If
        If val > stok Then
            e.Cancel = True
            DgvBarangTukar.Rows(e.RowIndex).ErrorText = $"Qty tidak boleh melebihi stok ({stok})"
            Return
        End If

        DgvBarangTukar.Rows(e.RowIndex).ErrorText = ""
    End Sub

    Private Sub DgvBarangTukar_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DgvBarangTukar.CurrentCellDirtyStateChanged
        If DgvBarangTukar.IsCurrentCellDirty Then
            DgvBarangTukar.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    ''' <summary>Hitung Total Poin = Qty × Harga Poin untuk satu baris.</summary>
    Private Sub HitungTotalPoinBaris(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= DgvBarangTukar.Rows.Count Then Return
        Dim row As DataGridViewRow = DgvBarangTukar.Rows(rowIndex)

        Dim qty As Integer = 0
        Dim hargaPoin As Integer = 0
        Integer.TryParse(If(row.Cells("QTY").Value IsNot Nothing, row.Cells("QTY").Value.ToString(), "0"), qty)
        Integer.TryParse(If(row.Cells("HARGA_POIN").Value IsNot Nothing, row.Cells("HARGA_POIN").Value.ToString(), "0"), hargaPoin)

        row.Cells("TOTAL_POIN").Value = qty * hargaPoin
    End Sub

    ''' <summary>Hitung total poin dibutuhkan dari semua baris DGV.</summary>
    Private Function HitungTotalPoinDibutuhkan() As Integer
        Dim total As Integer = 0
        If Not DgvBarangTukar.Columns.Contains("TOTAL_POIN") Then Return 0
        For Each row As DataGridViewRow In DgvBarangTukar.Rows
            If row.IsNewRow Then Continue For
            Dim v As Integer = 0
            Integer.TryParse(If(row.Cells("TOTAL_POIN").Value IsNot Nothing,
                                row.Cells("TOTAL_POIN").Value.ToString(), "0"), v)
            total += v
        Next
        Return total
    End Function

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: RINGKASAN POIN & TOMBOL KONFIRMASI
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Ringkasan Poin"

    ''' <summary>
    ''' Update label LblTotalPoinDibutuhkan dan LblSisaPoinSetelah secara real-time.
    ''' Req 3.6
    ''' </summary>
    Private Sub UpdateRingkasanPoin()
        Dim totalDibutuhkan As Integer = HitungTotalPoinDibutuhkan()
        Dim sisaPoin As Integer = _saldoPoin - totalDibutuhkan

        LblTotalPoinDibutuhkan.Text = "Total Poin Dibutuhkan: " & totalDibutuhkan.ToString("N0")
        LblSisaPoinSetelah.Text = "Sisa Poin Setelah Tukar: " & sisaPoin.ToString("N0")

        If sisaPoin < 0 Then
            LblSisaPoinSetelah.ForeColor = Drawing.Color.Red
        Else
            LblSisaPoinSetelah.ForeColor = Drawing.Color.DarkGreen
        End If

        HitungDanTampilkanSummaryGrid()
    End Sub

    Private Sub HitungDanTampilkanSummaryGrid()
        Dim itemCount As Integer = 0
        Dim totalQty As Integer = 0

        For Each row As DataGridViewRow In DgvBarangTukar.Rows
            If row.IsNewRow Then Continue For
            Dim pilih As Boolean = False
            Boolean.TryParse(If(row.Cells("PILIH").Value IsNot Nothing, row.Cells("PILIH").Value.ToString(), "False"), pilih)
            If Not pilih Then Continue For

            itemCount += 1
            Dim qty As Integer = 0
            Integer.TryParse(If(row.Cells("QTY").Value IsNot Nothing, row.Cells("QTY").Value.ToString(), "0"), qty)
            totalQty += qty
        Next

        LblSummary.Text = $"{itemCount} item | Total Qty: {totalQty}"
    End Sub

    ''' <summary>
    ''' Enable/disable BtnKonfirmasiTukar berdasarkan kondisi validasi.
    ''' Req 3.4, Req 3.7
    ''' </summary>
    Private Sub UpdateTombolKonfirmasi()
        Dim totalDibutuhkan As Integer = HitungTotalPoinDibutuhkan()
        Dim bolehKonfirmasi As Boolean = True

        ' Harus ada pelanggan terpilih
        If String.IsNullOrEmpty(_kodePelanggan) Then bolehKonfirmasi = False

        ' Total poin dibutuhkan tidak boleh melebihi saldo (Req 3.7)
        If totalDibutuhkan > _saldoPoin Then bolehKonfirmasi = False

        ' Harus ada minimal 1 item dengan qty > 0
        If totalDibutuhkan = 0 Then bolehKonfirmasi = False

        BtnKonfirmasiTukar.Enabled = bolehKonfirmasi
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: KONFIRMASI PENUKARAN
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Konfirmasi Penukaran"

    ''' <summary>
    ''' Generate nomor referensi penukaran format "TP-YYYYMMDD-XXXX".
    ''' Query MAX dari poin_ledger hari ini + increment.
    ''' Req 3.11
    ''' </summary>
    Private Function GenerateNoReferensi() As String
        Dim tanggalStr As String = DateTime.Today.ToString("yyyyMMdd")
        Dim prefix As String = "TP-" & tanggalStr & "-"
        Dim nomorUrut As Integer = 1

        Try
            Using cmd As New MySqlCommand(
                "SELECT MAX(CAST(SUBSTRING(NO_REFERENSI, 14) AS UNSIGNED)) " &
                "FROM poin_ledger " &
                "WHERE NO_REFERENSI LIKE @prefix AND TIPE = 'REDEEM' " &
                "AND DATE(CREATED_AT) = CURDATE()", conn)
                cmd.Parameters.AddWithValue("@prefix", prefix & "%")
                Dim val = cmd.ExecuteScalar()
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                    nomorUrut = Convert.ToInt32(val) + 1
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormTukarPoin.GenerateNoReferensi] {ex.Message}")
        End Try

        Return prefix & nomorUrut.ToString("D4")
    End Function

    ''' <summary>
    ''' Proses konfirmasi penukaran poin.
    ''' Req 3.8, Req 3.9, Req 3.10, Req 3.11, Req 8.3, Req 8.4
    ''' </summary>
    Private Sub BtnKonfirmasiTukar_Click(sender As Object, e As EventArgs) Handles BtnKonfirmasiTukar.Click
        ' ── Validasi ─────────────────────────────────────────────────────────
        Dim totalDibutuhkan As Integer = HitungTotalPoinDibutuhkan()
        If String.IsNullOrEmpty(_kodePelanggan) Then
            MessageBox.Show("Pilih pelanggan terlebih dahulu.", "Validasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtPelanggan.Focus()
            Return
        End If
        If totalDibutuhkan = 0 Then
            MessageBox.Show("Pilih minimal satu barang dengan qty > 0.", "Validasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If totalDibutuhkan > _saldoPoin Then
            Dim kurang As Integer = totalDibutuhkan - _saldoPoin
            MessageBox.Show($"Poin tidak mencukupi. Kekurangan: {kurang:N0} poin." & vbCrLf &
                            $"Saldo: {_saldoPoin:N0} | Dibutuhkan: {totalDibutuhkan:N0}",
                            "Poin Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Konfirmasi dari kasir
        Dim konfirmasi As DialogResult = MessageBox.Show(
            $"Konfirmasi penukaran poin?" & vbCrLf &
            $"Pelanggan : {_namaPelanggan}" & vbCrLf &
            $"Poin digunakan: {totalDibutuhkan:N0}" & vbCrLf &
            $"Sisa poin    : {_saldoPoin - totalDibutuhkan:N0}",
            "Konfirmasi Penukaran", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If konfirmasi <> DialogResult.Yes Then Return

        ' ── Kumpulkan item yang dicentang DAN qty > 0 ─────────────────────────
        Dim itemTukar As New List(Of Tuple(Of String, String, Integer, Integer))() ' (kode, nama, qty, hargaPoin)
        Dim ids As New List(Of String)
        For Each row As DataGridViewRow In DgvBarangTukar.Rows
            If row.IsNewRow Then Continue For
            Dim isChecked As Boolean = False
            If row.Cells("PILIH").Value IsNot Nothing Then
                Boolean.TryParse(row.Cells("PILIH").Value.ToString(), isChecked)
            End If
            If Not isChecked Then Continue For
            Dim qty As Integer = 0
            Integer.TryParse(If(row.Cells("QTY").Value IsNot Nothing, row.Cells("QTY").Value.ToString(), "0"), qty)
            If qty <= 0 Then Continue For
            Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()
            Dim namaBarang As String = row.Cells("NAMA_BARANG").Value.ToString()
            Dim hargaPoin As Integer = 0
            Integer.TryParse(If(row.Cells("HARGA_POIN").Value IsNot Nothing, row.Cells("HARGA_POIN").Value.ToString(), "0"), hargaPoin)
            itemTukar.Add(Tuple.Create(kodeBarang, namaBarang, qty, hargaPoin))
            ids.Add(kodeBarang)
        Next

        ' ── Pre-load HARGA_BELI, SATUAN, ISI_SATUAN from tbl_barang ────
        Dim dataBarang As New Dictionary(Of String, Tuple(Of Decimal, String, Integer))()
        If ids.Count > 0 Then
            Dim placeholders As New List(Of String)
            Using cmd As New MySqlCommand("", conn)
                For i As Integer = 0 To ids.Count - 1
                    Dim p As String = $"@id{i}"
                    placeholders.Add(p)
                    cmd.Parameters.AddWithValue(p, ids(i))
                Next
                cmd.CommandText = $"SELECT ID_BARANG, HARGA_BELI, SATUAN_STOK, SATUAN_ISI_STOK FROM tbl_barang WHERE ID_BARANG IN ({String.Join(",", placeholders)})"
                Using dt As New DataTable
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                    For Each dr As DataRow In dt.Rows
                        Dim kode As String = dr("ID_BARANG").ToString()
                        dataBarang(kode) = Tuple.Create(
                            If(IsDBNull(dr("HARGA_BELI")), 0D, Convert.ToDecimal(dr("HARGA_BELI"))),
                            If(dr("SATUAN_STOK") Is DBNull.Value, "", dr("SATUAN_STOK").ToString()),
                            If(IsDBNull(dr("SATUAN_ISI_STOK")), 1, Convert.ToInt32(dr("SATUAN_ISI_STOK"))))
                    Next
                End Using
            End Using
        End If

        ' ── Generate nomor referensi / reuse existing ─────────────────────────
        Dim noReferensi As String = If(TxtJenisTrans.Text = "EditTukarPoin", TxtFaktur.Text, GenerateNoReferensi())
        Dim lokasi As String = FormUtama.StatusLokasi.Text
        Dim kolKurang As String = If(lokasi = "GUDANG", "KURANG_GUDANG", "KURANG_TOKO")
        Dim idUser As String = FormUtama.StatusNamaUser.Text
        Dim idKomputer As String = FormUtama.StatusNamaPC.Text
        Dim tgl As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        ' Siapkan audit dictionaries
        Dim auditHistory As New Dictionary(Of String, Decimal)()
        Dim auditStokDelta As New Dictionary(Of String, Decimal)()

        ' ── Validasi stok real-time via SP (FOR UPDATE) — anti race condition ──
        ' Cek stok LANGSUNG dari DB, bukan dari data yang sudah di-load ke form.
        ' Menangkap kasus user lain sudah transaksi duluan sejak form dibuka.
        If Not ModulHakAkses.SettingIzinkanBarangMinus Then
            For Each item In itemTukar
                Try
                    Using cmdSP As New MySqlCommand("CALL sp_hlp_stok_validasi(@kode, @qty, @lokasi, @izinkan, @errcode, @errmsg)", conn)
                        cmdSP.Parameters.AddWithValue("@kode", item.Item1)
                        cmdSP.Parameters.AddWithValue("@qty", item.Item3)
                        cmdSP.Parameters.AddWithValue("@lokasi", lokasi)
                        cmdSP.Parameters.AddWithValue("@izinkan", 0)

                        Dim pErrCode = cmdSP.Parameters.Add("@errcode", MySqlDbType.VarChar, 50)
                        pErrCode.Direction = ParameterDirection.Output
                        Dim pErrMsg = cmdSP.Parameters.Add("@errmsg", MySqlDbType.VarChar, 255)
                        pErrMsg.Direction = ParameterDirection.Output

                        cmdSP.ExecuteNonQuery()

                        Dim errCode As String = If(pErrCode.Value IsNot Nothing, pErrCode.Value.ToString(), "")
                        If Not String.IsNullOrEmpty(errCode) Then
                            MessageBox.Show(
                                "Stok berubah sejak form dibuka!" & vbCrLf & vbCrLf &
                                pErrMsg.Value?.ToString() & vbCrLf & vbCrLf &
                                "Kemungkinan ada transaksi lain yang baru saja memproses barang ini." & vbCrLf &
                                "Silakan periksa kembali jumlah yang akan ditukar.",
                                "Konflik Stok",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
                            Return
                        End If
                    End Using
                Catch ex As Exception
                    ' Jika SP gagal (misal koneksi putus), lanjutkan saja
                    ' Validasi client-side di CellValidating sudah cukup sebagai lapisan pertama
                End Try
            Next
        End If

        ' Eksekusi dalam satu transaksi atomik (Req 8.3)
        Dim trans As MySqlTransaction = Nothing
        Try
            trans = conn.BeginTransaction()

            ' ── 0. Jika edit: audit + hapus data lama dulu (reverse + delete) ──
            If TxtJenisTrans.Text = "EditTukarPoin" Then
                ModuleAuditTrail.CatatAudit(noReferensi, "EDIT", "Tukar Poin", trans:=trans)
                ModuleHapusTransaksi.HapusTukarPoin(noReferensi, lokasi, trans)
            End If

            ' ── 1. Catat REDEEM ke poin_ledger + kurangi SALDO_POIN (Req 3.8, Req 3.9)
            ModuleLoyaltyPoin.CatatRedeem(_kodePelanggan, totalDibutuhkan, noReferensi, trans)

            ' ── 2. Counter update + HistoryBarang insert untuk setiap item ──
            Using cmdHistory As New MySqlCommand(
                "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)",
                conn, trans)

                For Each item In itemTukar
                    Dim kode As String = item.Item1
                    Dim qty As Integer = item.Item3
                    Dim hb As Decimal = 0D
                    Dim satuan As String = ""
                    Dim isiSatuan As Integer = 1
                    If dataBarang.ContainsKey(kode) Then
                        hb = dataBarang(kode).Item1
                        satuan = dataBarang(kode).Item2
                        isiSatuan = dataBarang(kode).Item3
                    End If
                    Dim totalQty As Decimal = qty * isiSatuan
                    Dim totalRupiah As Decimal = hb * qty

                    ' Update counter KURANG_TOKO / KURANG_GUDANG (+= qty)
                    Using cmdStok As New MySqlCommand(
                        $"UPDATE tbl_barang SET {kolKurang} = {kolKurang} + @qty WHERE ID_BARANG = @kode",
                        conn, trans)
                        cmdStok.Parameters.AddWithValue("@qty", qty)
                        cmdStok.Parameters.AddWithValue("@kode", kode)
                        cmdStok.ExecuteNonQuery()
                    End Using

                    ' Insert HistoryBarang dengan JENIS = 'KURANG'
                    cmdHistory.Parameters.Clear()
                    cmdHistory.Parameters.AddWithValue("@FAKTUR", noReferensi)
                    cmdHistory.Parameters.AddWithValue("@TANGGAL", tgl)
                    cmdHistory.Parameters.AddWithValue("@JENIS", "KURANG")
                    cmdHistory.Parameters.AddWithValue("@LOKASI", lokasi)
                    cmdHistory.Parameters.AddWithValue("@ID_BARANG", kode)
                    cmdHistory.Parameters.AddWithValue("@NAMA_BARANG", item.Item2)
                    cmdHistory.Parameters.AddWithValue("@QTY", qty)
                    cmdHistory.Parameters.AddWithValue("@SATUAN", satuan)
                    cmdHistory.Parameters.AddWithValue("@ISI_SATUAN", isiSatuan)
                    cmdHistory.Parameters.AddWithValue("@TOTAL_QTY", totalQty)
                    cmdHistory.Parameters.AddWithValue("@TOTAL_RUPIAH", totalRupiah)
                    cmdHistory.Parameters.AddWithValue("@ID_USER", idUser)
                    cmdHistory.Parameters.AddWithValue("@ID_KOMPUTER", idKomputer)
                    cmdHistory.ExecuteNonQuery()

                    ' Audit B — qty masuk HistoryBarang
                    If auditHistory.ContainsKey(kode) Then auditHistory(kode) += totalQty Else auditHistory(kode) = totalQty
                Next
            End Using

            ' ── 3. HitungStokPerubahan + catat delta stok ─────────────────────
            For Each item In itemTukar
                Dim kode As String = item.Item1
                Dim stokSebelum As Decimal = BacaStokSaatIni(kode, lokasi, trans)
                HitungStokPerubahan(kode, trans)
                Dim stokSesudah As Decimal = BacaStokSaatIni(kode, lokasi, trans)
                Dim delta As Decimal = stokSebelum - stokSesudah
                If auditStokDelta.ContainsKey(kode) Then auditStokDelta(kode) += delta Else auditStokDelta(kode) = delta
            Next

            ' ── 4. JurnalUmum — Debit HPP/Biaya, Kredit Persediaan Barang ─────
            Dim totalHPP As Decimal = 0D
            For Each item In itemTukar
                Dim kode As String = item.Item1
                Dim hb As Decimal = 0D
                If dataBarang.ContainsKey(kode) Then hb = dataBarang(kode).Item1
                totalHPP += hb * item.Item3
            Next

            If totalHPP > 0 Then
                Dim uraianJurnal As String = $"Penukaran poin {_namaPelanggan} — {noReferensi}"
                Using cmdJ As New MySqlCommand(
                    "INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                    "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)",
                    conn, trans)
                    cmdJ.Parameters.AddWithValue("@NO_TRANSAKSI", noReferensi)
                    cmdJ.Parameters.AddWithValue("@TGL_TRANSAKSI", tgl)
                    cmdJ.Parameters.AddWithValue("@URAIAN", uraianJurnal)
                    cmdJ.Parameters.AddWithValue("@NAMA_AKUN_D", LAWAN_NAMA_REK_BARANG)
                    cmdJ.Parameters.AddWithValue("@NOMOR_AKUN_D", LAWAN_KODE_REK_BARANG)
                    cmdJ.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
                    cmdJ.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)
                    cmdJ.Parameters.AddWithValue("@NOMINAL", totalHPP)
                    cmdJ.Parameters.AddWithValue("@JENIS_TRANSAKSI", "TUKAR POIN")
                    cmdJ.Parameters.AddWithValue("@LOKASI", lokasi)
                    cmdJ.Parameters.AddWithValue("@ID_USER", idUser)
                    cmdJ.Parameters.AddWithValue("@ID_KOMPUTER", idKomputer)
                    cmdJ.ExecuteNonQuery()
                End Using
            End If

            ' ── 5. AuditStokTransaksi ────────────────────────────────────────
            AuditStokTransaksi(noReferensi, "Tukar Poin", Nothing, auditHistory, Nothing, auditStokDelta, trans)

            trans.Commit()

            ' ── Post-commit ──────────────────────────────────────────────────
            Dim sisaPoinAkhir As Integer = _saldoPoin - totalDibutuhkan
            CetakBuktiPenukaran(noReferensi, itemTukar, totalDibutuhkan, sisaPoinAkhir)

            _saldoPoin = ModuleLoyaltyPoin.AmbilSaldoPoin(_kodePelanggan)
            TampilkanSaldoPoin()
            ResetQtyGrid()

        Catch ex As Exception
            If trans IsNot Nothing Then
                Try
                    trans.Rollback()
                Catch
                End Try
            End If
            MessageBox.Show("Terjadi kesalahan saat memproses penukaran:" & vbCrLf & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: CETAK BUKTI & HELPER
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Cetak Bukti & Helper"

    ''' <summary>
    ''' Cetak bukti penukaran sederhana via MessageBox.
    ''' Req 3.10
    ''' </summary>
    Private Sub CetakBuktiPenukaran(noReferensi As String,
                                     itemTukar As List(Of Tuple(Of String, String, Integer, Integer)),
                                     totalPoinDigunakan As Integer,
                                     sisaSaldo As Integer)
        Dim sb As New System.Text.StringBuilder()
        sb.AppendLine("══════════════════════════════════")
        sb.AppendLine("       BUKTI PENUKARAN POIN")
        sb.AppendLine("══════════════════════════════════")
        sb.AppendLine($"No. Referensi : {noReferensi}")
        sb.AppendLine($"Tanggal       : {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
        sb.AppendLine($"Pelanggan     : {_namaPelanggan}")
        sb.AppendLine("──────────────────────────────────")
        sb.AppendLine("Barang yang Ditukar:")
        For Each item In itemTukar
            sb.AppendLine($"  {item.Item2}")
            sb.AppendLine($"    {item.Item3} x {item.Item4:N0} poin = {item.Item3 * item.Item4:N0} poin")
        Next
        sb.AppendLine("──────────────────────────────────")
        sb.AppendLine($"Poin Digunakan: {totalPoinDigunakan:N0}")
        sb.AppendLine($"Sisa Saldo    : {sisaSaldo:N0} poin")
        sb.AppendLine("══════════════════════════════════")
        sb.AppendLine("       Terima kasih!")

        MessageBox.Show(sb.ToString(), "Bukti Penukaran Poin",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>Reset semua Qty di grid ke 0 setelah transaksi berhasil.</summary>
    Private Sub ResetQtyGrid()
        For Each row As DataGridViewRow In DgvBarangTukar.Rows
            If row.IsNewRow Then Continue For
            row.Cells("PILIH").Value = False
            row.Cells("QTY").Value = 0
            row.Cells("TOTAL_POIN").Value = 0
        Next
        UpdateRingkasanPoin()
        UpdateTombolKonfirmasi()
    End Sub

    Private Sub DgvBarangTukar_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvBarangTukar.RowPostPaint
        Dim nomor As String = (e.RowIndex + 1).ToString()
        Dim fmt As New StringFormat() With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        Dim bounds As New Rectangle(e.RowBounds.Left, e.RowBounds.Top,
                                    DgvBarangTukar.RowHeadersWidth, e.RowBounds.Height)
        e.Graphics.DrawString(nomor, DgvBarangTukar.DefaultCellStyle.Font,
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

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles BtnRefresh.Click
        MuatBarangTukar()
        If Not String.IsNullOrEmpty(_kodePelanggan) Then
            _saldoPoin = ModuleLoyaltyPoin.AmbilSaldoPoin(_kodePelanggan)
            TampilkanSaldoPoin()
        End If
    End Sub

    Private Sub FormTukarPoin_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then Close()
        If e.KeyCode = Keys.F2 AndAlso BtnKonfirmasiTukar.Enabled Then
            BtnKonfirmasiTukar.PerformClick()
        End If
    End Sub

#End Region

End Class
