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
        InisPelangganAutoComplete()
        MuatBarangTukar()
        UpdateTombolKonfirmasi()
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: PILIH PELANGGAN
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Pilih Pelanggan"

    ''' <summary>
    ''' Inisialisasi AutoComplete untuk TxtPelanggan dari tbl_pelanggan.
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
            TxtPelanggan.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            TxtPelanggan.AutoCompleteSource = AutoCompleteSource.CustomSource
            TxtPelanggan.AutoCompleteCustomSource = ac
        Catch ex As Exception
            Debug.WriteLine($"[FormTukarPoin.InisPelangganAutoComplete] {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Saat teks berubah, cari pelanggan dan tampilkan saldo poin.
    ''' Format input: "KODE - NAMA" atau hanya KODE.
    ''' Req 3.2, Req 3.3, Req 3.4
    ''' </summary>
    Private Sub TxtPelanggan_TextChanged(sender As Object, e As EventArgs) Handles TxtPelanggan.TextChanged
        Dim teks As String = TxtPelanggan.Text.Trim()
        If String.IsNullOrEmpty(teks) Then
            ResetPelanggan()
            Return
        End If

        ' Ekstrak kode dari format "KODE - NAMA"
        Dim kode As String = teks
        If teks.Contains(" - ") Then
            kode = teks.Split(New String() {" - "}, StringSplitOptions.None)(0).Trim()
        End If

        Try
            Using cmd As New MySqlCommand(
                "SELECT KODE, NAMA, SALDO_POIN FROM tbl_pelanggan " &
                "WHERE KODE = @kode OR NAMA LIKE @nama LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@kode", kode)
                cmd.Parameters.AddWithValue("@nama", "%" & teks & "%")
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        _kodePelanggan = rd("KODE").ToString()
                        _namaPelanggan = rd("NAMA").ToString()
                        _saldoPoin = If(IsDBNull(rd("SALDO_POIN")), 0, Convert.ToInt32(rd("SALDO_POIN")))
                        TampilkanSaldoPoin()
                    Else
                        ResetPelanggan()
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine($"[FormTukarPoin.TxtPelanggan_TextChanged] {ex.Message}")
            ResetPelanggan()
        End Try
    End Sub

    Private Sub ResetPelanggan()
        _kodePelanggan = ""
        _namaPelanggan = ""
        _saldoPoin = 0
        LblSaldoPoinTukar.Text = "Saldo Poin: -"
        LblStatusMinRedeem.Text = ""
        LblStatusMinRedeem.Visible = False
        UpdateTombolKonfirmasi()
        UpdateRingkasanPoin()
    End Sub

    ''' <summary>
    ''' Tampilkan saldo poin dan pesan minimum redeem.
    ''' Req 3.3, Req 3.4
    ''' </summary>
    Private Sub TampilkanSaldoPoin()
        LblSaldoPoinTukar.Text = "Saldo Poin: " & _saldoPoin.ToString("N0")

        If _saldoPoin < LP_MinimumRedeem Then
            LblStatusMinRedeem.Text = $"Poin belum mencukupi untuk penukaran. Minimum: {LP_MinimumRedeem:N0} poin."
            LblStatusMinRedeem.Visible = True
        Else
            LblStatusMinRedeem.Text = ""
            LblStatusMinRedeem.Visible = False
        End If

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
                $"SELECT b.ID_BARANG, b.NAMA_BARANG, b.{kolStok} AS STOK, pb.HARGA_POIN " &
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

        ' Sembunyikan kolom data source, tampilkan kolom yang relevan
        DgvBarangTukar.Columns("ID_BARANG").HeaderText = "Kode Barang"
        DgvBarangTukar.Columns("ID_BARANG").ReadOnly = True
        DgvBarangTukar.Columns("ID_BARANG").Width = 110

        DgvBarangTukar.Columns("NAMA_BARANG").HeaderText = "Nama Barang"
        DgvBarangTukar.Columns("NAMA_BARANG").ReadOnly = True
        DgvBarangTukar.Columns("NAMA_BARANG").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        DgvBarangTukar.Columns("STOK").HeaderText = "Stok"
        DgvBarangTukar.Columns("STOK").ReadOnly = True
        DgvBarangTukar.Columns("STOK").Width = 70
        DgvBarangTukar.Columns("STOK").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        DgvBarangTukar.Columns("HARGA_POIN").HeaderText = "Harga Poin"
        DgvBarangTukar.Columns("HARGA_POIN").ReadOnly = True
        DgvBarangTukar.Columns("HARGA_POIN").Width = 90
        DgvBarangTukar.Columns("HARGA_POIN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        ' Tambah kolom Qty (editable) jika belum ada
        If Not DgvBarangTukar.Columns.Contains("QTY") Then
            Dim colQty As New DataGridViewTextBoxColumn()
            colQty.Name = "QTY"
            colQty.HeaderText = "Qty"
            colQty.Width = 60
            colQty.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DgvBarangTukar.Columns.Add(colQty)
        End If

        ' Tambah kolom Total Poin (read-only, dihitung otomatis) jika belum ada
        If Not DgvBarangTukar.Columns.Contains("TOTAL_POIN") Then
            Dim colTotal As New DataGridViewTextBoxColumn()
            colTotal.Name = "TOTAL_POIN"
            colTotal.HeaderText = "Total Poin"
            colTotal.Width = 90
            colTotal.ReadOnly = True
            colTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DgvBarangTukar.Columns.Add(colTotal)
        End If

        ' Inisialisasi Qty = 0 dan Total Poin = 0 untuk semua baris
        For Each row As DataGridViewRow In DgvBarangTukar.Rows
            If row.IsNewRow Then Continue For
            If row.Cells("QTY").Value Is Nothing OrElse row.Cells("QTY").Value.ToString() = "" Then
                row.Cells("QTY").Value = 0
            End If
            row.Cells("TOTAL_POIN").Value = 0
        Next

        DgvBarangTukar.AllowUserToAddRows = False
        DgvBarangTukar.AllowUserToDeleteRows = False
        DgvBarangTukar.RowHeadersVisible = True
        DgvBarangTukar.RowHeadersWidth = 45
        DgvBarangTukar.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        ModuleTheme.ApplyThemeDataGridView(DgvBarangTukar)
    End Sub

#End Region

    ' ═══════════════════════════════════════════════════════════════════════
    ' REGION: EDIT QTY DI GRID — UPDATE REAL-TIME
    ' ═══════════════════════════════════════════════════════════════════════

#Region "Edit Qty Real-Time"

    ''' <summary>
    ''' Saat nilai sel berubah, hitung ulang Total Poin baris tersebut
    ''' dan update ringkasan total poin dibutuhkan + sisa poin.
    ''' Req 3.6, Req 3.7
    ''' </summary>
    Private Sub DgvBarangTukar_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DgvBarangTukar.CellValueChanged
        If e.RowIndex < 0 Then Return
        If DgvBarangTukar.Columns.Count = 0 Then Return
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

        ' Warna merah jika sisa negatif
        If sisaPoin < 0 Then
            LblSisaPoinSetelah.ForeColor = Drawing.Color.Red
        Else
            LblSisaPoinSetelah.ForeColor = Drawing.Color.DarkGreen
        End If
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

        ' Saldo harus >= minimum redeem (Req 3.4)
        If _saldoPoin < LP_MinimumRedeem Then bolehKonfirmasi = False

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
        ' Validasi ulang sebelum proses
        Dim totalDibutuhkan As Integer = HitungTotalPoinDibutuhkan()
        If String.IsNullOrEmpty(_kodePelanggan) Then
            MessageBox.Show("Pilih pelanggan terlebih dahulu.", "Validasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtPelanggan.Focus()
            Return
        End If
        If _saldoPoin < LP_MinimumRedeem Then
            MessageBox.Show($"Saldo poin pelanggan ({_saldoPoin:N0}) kurang dari minimum redeem ({LP_MinimumRedeem:N0}).",
                            "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
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

        ' Kumpulkan item yang ditukar (qty > 0)
        Dim itemTukar As New List(Of Tuple(Of String, String, Integer, Integer))() ' (kode, nama, qty, hargaPoin)
        For Each row As DataGridViewRow In DgvBarangTukar.Rows
            If row.IsNewRow Then Continue For
            Dim qty As Integer = 0
            Integer.TryParse(If(row.Cells("QTY").Value IsNot Nothing, row.Cells("QTY").Value.ToString(), "0"), qty)
            If qty <= 0 Then Continue For
            Dim kodeBarang As String = row.Cells("ID_BARANG").Value.ToString()
            Dim namaBarang As String = row.Cells("NAMA_BARANG").Value.ToString()
            Dim hargaPoin As Integer = 0
            Integer.TryParse(If(row.Cells("HARGA_POIN").Value IsNot Nothing, row.Cells("HARGA_POIN").Value.ToString(), "0"), hargaPoin)
            itemTukar.Add(Tuple.Create(kodeBarang, namaBarang, qty, hargaPoin))
        Next

        ' Generate nomor referensi
        Dim noReferensi As String = GenerateNoReferensi()

        ' Eksekusi dalam satu transaksi atomik (Req 8.3)
        Dim trans As MySqlTransaction = Nothing
        Try
            trans = conn.BeginTransaction()

            ' Catat REDEEM ke poin_ledger + kurangi SALDO_POIN (Req 3.8, Req 3.9)
            ModuleLoyaltyPoin.CatatRedeem(_kodePelanggan, totalDibutuhkan, noReferensi, trans)

            ' Kurangi stok barang sesuai lokasi login (Req 3.9)
            Dim kolStok As String = If(FormUtama.StatusLokasi.Text = "GUDANG", "STOK_GUDANG", "STOK_TOKO")
            For Each item In itemTukar
                Using cmdStok As New MySqlCommand(
                    $"UPDATE tbl_barang SET {kolStok} = {kolStok} - @qty WHERE ID_BARANG = @kode",
                    conn, trans)
                    cmdStok.Parameters.AddWithValue("@qty", item.Item3)
                    cmdStok.Parameters.AddWithValue("@kode", item.Item1)
                    cmdStok.ExecuteNonQuery()
                End Using
            Next

            trans.Commit()

            ' Hitung sisa poin setelah transaksi
            Dim sisaPoinAkhir As Integer = _saldoPoin - totalDibutuhkan

            ' Cetak bukti penukaran (Req 3.10)
            CetakBuktiPenukaran(noReferensi, itemTukar, totalDibutuhkan, sisaPoinAkhir)

            ' Refresh saldo di form
            _saldoPoin = ModuleLoyaltyPoin.AmbilSaldoPoin(_kodePelanggan)
            TampilkanSaldoPoin()

            ' Reset qty di grid
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
