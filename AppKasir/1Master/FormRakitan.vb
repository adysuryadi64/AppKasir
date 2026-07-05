''' <summary>
''' FormRakitan — Daftar Paket Rakitan
''' Detail komponen BOM: popup floating dialog di atas DGV.
''' </summary>
Public Class FormRakitan

    ' ========================================================================
    '  LOAD
    ' ========================================================================
    Private Sub FormRakitan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        SetupDgv()
        SetupDgvPopup()
        MuatData()
        PanelPopup.Visible = False
    End Sub

    ' ========================================================================
    '  SETUP DGV PAKET
    ' ========================================================================
    Private Sub SetupDgv()
        With DgvDaftarPaket
            .AllowUserToAddRows    = False
            .AllowUserToDeleteRows = False
            .ReadOnly              = True
            .SelectionMode         = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect           = False
        End With
        ModuleTheme.ApplyThemeDataGridView(DgvDaftarPaket)
    End Sub

    ' ========================================================================
    '  SETUP DGV POPUP — komponen BOM
    ' ========================================================================
    Private Sub SetupDgvPopup()
        With DgvPopup
            .Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "No", .HeaderText = "#", .Width = 35})
            .Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Kode", .HeaderText = "Kode", .Width = 100})
            .Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Nama", .HeaderText = "Nama Komponen", .Width = 200})
            .Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Qty", .HeaderText = "Qty/Paket", .Width = 80})
            .Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Satuan", .HeaderText = "Satuan", .Width = 65})
            .Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "StokToko", .HeaderText = "Stok Toko", .Width = 85})
            .Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "StokGudang", .HeaderText = "Stok Gudang", .Width = 85})
            .Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "HargaBeli", .HeaderText = "HPP", .Width = 90})
            .Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "TotalHPP", .HeaderText = "Total", .Width = 90})
            ModuleAngka.TerapkanFormatKolomAngka(DgvPopup, {"Qty", "StokToko", "StokGudang", "HargaBeli", "TotalHPP"})
        End With
        ModuleTheme.ApplyThemeDataGridView(DgvPopup)
    End Sub

    ' ========================================================================
    '  TOGGLE POPUP — tampilkan/sembunyikan dialog komponen
    ' ========================================================================
    Private Sub TogglePopup()
        If PanelPopup.Visible Then
            PanelPopup.Visible = False
        Else
            If DgvDaftarPaket.CurrentRow Is Nothing Then Return
            Dim kode As String = DgvDaftarPaket.CurrentRow.Cells("Kode").Value?.ToString()
            Dim nama As String = DgvDaftarPaket.CurrentRow.Cells("Nama").Value?.ToString()
            If String.IsNullOrWhiteSpace(kode) Then Return
            LoadPopup(kode, nama)
            ' Posisikan di tengah DGV
            PanelPopup.Location = New Point(
                (Me.ClientSize.Width - PanelPopup.Width) \ 2,
                DgvDaftarPaket.Top + (DgvDaftarPaket.Height - PanelPopup.Height) \ 2)
            PanelPopup.BringToFront()
            PanelPopup.Visible = True
        End If
    End Sub

    Private Sub LoadPopup(kodeRakitan As String, namaPaket As String)
        DgvPopup.Rows.Clear()
        LblPopupTitle.Text = $"Komponen BOM — {namaPaket}"
        Try
            Using cmd As New MySqlCommand(
                "SELECT r.kode_komponen, r.nama_komponen, r.qty, r.satuan, " &
                "b.HARGA_BELI, b.STOK_TOKO, b.STOK_GUDANG " &
                "FROM tbl_rakitan_bom r " &
                "LEFT JOIN tbl_barang b ON b.ID_BARANG = r.kode_komponen " &
                "WHERE r.kode_rakitan = @kode ORDER BY r.urutan", conn)
                cmd.Parameters.AddWithValue("@kode", kodeRakitan)
                Using rd = cmd.ExecuteReader()
                    Dim no As Integer = 0
                    While rd.Read()
                        no += 1
                        Dim qty As Decimal = ModuleAngka.ParseDecimal(rd("qty"))
                        Dim hpp As Decimal = ModuleAngka.SafeGetValue(Of Decimal)(rd, "HARGA_BELI", 0D)
                        DgvPopup.Rows.Add(no, rd("kode_komponen").ToString(), rd("nama_komponen").ToString(),
                            qty.ToString("N2"), rd("satuan").ToString(),
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D).ToString("N2"),
                            ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D).ToString("N2"),
                            hpp.ToString("N0"), (hpp * qty).ToString("N0"))
                    End While
                End Using
            End Using
            LblPopupTitle.Text = $"Komponen BOM — {namaPaket} ({DgvPopup.Rows.Count} komponen)"
        Catch : End Try
    End Sub

    Private Sub BtnPopupClose_Click(sender As Object, e As EventArgs) Handles BtnPopupClose.Click
        PanelPopup.Visible = False
    End Sub

    ' ========================================================================
    '  MUAT DATA PAKET
    ' ========================================================================
    Public Sub MuatData()
        Dim cari As String = TxtCari.Text.Trim()
        Using cmd As New MySqlCommand(
            "SELECT b.ID_BARANG AS Kode, b.NAMA_BARANG AS Nama, " &
            "b.NAMA_KATEGORI AS Kategori, " &
            "b.HARGA_BELI AS HPP, " &
            "b.HARGA_JUAL_UMUM_KECIL AS HargaJual, " &
            "b.SATUAN_UMUM_KECIL AS Satuan, " &
            "b.STOK_TOKO AS StokToko, b.STOK_GUDANG AS StokGudang, " &
            "b.BARCODE_KECIL AS Barcode, " &
            "IFNULL((SELECT COUNT(*) FROM tbl_rakitan_bom r " &
            "        WHERE r.kode_rakitan = b.ID_BARANG), 0) AS JmlKomponen " &
            "FROM tbl_barang b " &
            "WHERE b.IS_PAKET = 1 AND b.STATUS = 'Aktif' " &
            "AND (b.ID_BARANG LIKE @cari OR b.NAMA_BARANG LIKE @cari " &
            "OR b.NAMA_KATEGORI LIKE @cari OR b.BARCODE_KECIL LIKE @cari) " &
            "ORDER BY b.NAMA_BARANG", conn)
            cmd.Parameters.AddWithValue("@cari", "%" & cari & "%")
            Using da As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)
                DgvDaftarPaket.DataSource = dt
            End Using
        End Using
        AturKolom()
        PanelPopup.Visible = False
    End Sub

    Private Sub AturKolom()
        With DgvDaftarPaket
            If Not .Columns.Contains("Kode") Then Return
            .Columns("Kode").HeaderText        = "Kode"          : .Columns("Kode").Width = 120
            .Columns("Nama").HeaderText        = "Nama Paket"    : .Columns("Nama").Width = 250
            .Columns("Kategori").HeaderText    = "Kategori"      : .Columns("Kategori").Width = 120
            .Columns("HPP").HeaderText         = "HPP"           : .Columns("HPP").Width = 100
            .Columns("HargaJual").HeaderText   = "Harga Jual"    : .Columns("HargaJual").Width = 100
            .Columns("Satuan").HeaderText      = "Satuan"        : .Columns("Satuan").Width = 70
            .Columns("StokToko").HeaderText    = "Stok Toko"     : .Columns("StokToko").Width = 90
            .Columns("StokGudang").HeaderText  = "Stok Gudang"   : .Columns("StokGudang").Width = 90
            .Columns("JmlKomponen").HeaderText = "Komponen"      : .Columns("JmlKomponen").Width = 80
            .Columns("Barcode").Visible = False
            .Columns("Kode").Frozen = True
            .Columns("Nama").Frozen = True
        End With
        ModuleAngka.TerapkanFormatKolomAngka(DgvDaftarPaket, {"HPP", "HargaJual", "StokToko", "StokGudang"})
    End Sub

    ' ========================================================================
    '  CARI
    ' ========================================================================
    Private Sub TxtCari_TextChanged(sender As Object, e As EventArgs) Handles TxtCari.TextChanged
        MuatData()
    End Sub

    ' ========================================================================
    '  TOOLBAR
    ' ========================================================================
    Private Sub BtnPaketBaru_Click(sender As Object, e As EventArgs) Handles BtnPaketBaru.Click
        Dim frm As New FormTambahEditRakitan()
        frm.SetModeTambah()
        frm.ShowDialog(Me)
        frm.Dispose()
        MuatData()
    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        BukaEdit()
    End Sub

    Private Sub DgvDaftarPaket_DoubleClick(sender As Object, e As EventArgs) Handles DgvDaftarPaket.DoubleClick
        BukaEdit()
    End Sub

    Private Sub BukaEdit()
        If DgvDaftarPaket.CurrentRow Is Nothing Then Return
        Dim kode As String = DgvDaftarPaket.CurrentRow.Cells("Kode").Value?.ToString()
        Dim nama As String = DgvDaftarPaket.CurrentRow.Cells("Nama").Value?.ToString()
        If String.IsNullOrWhiteSpace(kode) Then Return
        Dim frm As New FormTambahEditRakitan()
        frm.SetModeEdit(kode, nama)
        frm.ShowDialog(Me)
        frm.Dispose()
        MuatData()
    End Sub

    Private Sub BtnHapus_Click(sender As Object, e As EventArgs) Handles BtnHapus.Click
        If DgvDaftarPaket.CurrentRow Is Nothing Then Return
        Dim kode As String = DgvDaftarPaket.CurrentRow.Cells("Kode").Value?.ToString()
        Dim nama As String = DgvDaftarPaket.CurrentRow.Cells("Nama").Value?.ToString()
        If String.IsNullOrWhiteSpace(kode) Then Return
        Dim jawab As DialogResult = MessageBox.Show(
            $"Hapus paket rakitan '{nama}'?" & vbCrLf &
            "Stok komponen akan dikembalikan dan BOM dihapus.",
            "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If jawab <> DialogResult.Yes Then Return
        Dim lokasi As String = FormUtama.StatusLokasi.Text
        Try
            Dim trx As MySqlTransaction = conn.BeginTransaction()
            Try
                ModuleHapusTransaksi.HapusRakitan(kode, lokasi, trx)
                ModuleAuditTrail.CatatAuditMaster(kode, "DELETE", "Rakitan", $"Hapus paket rakitan {nama}", trans:=trx)
                trx.Commit()
            Catch ex As OperationCanceledException
                trx.Rollback()
            Catch
                trx.Rollback()
                Throw
            End Try
            MessageBox.Show("Paket dihapus. Stok komponen dikembalikan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            MuatData()
        Catch ex As Exception
            MessageBox.Show("Gagal hapus: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnDetail_Click(sender As Object, e As EventArgs) Handles BtnDetail.Click
        TogglePopup()
    End Sub

    ' ========================================================================
    '  CONTEXT MENU
    ' ========================================================================
    Private Sub CtxTambah_Click(sender As Object, e As EventArgs) Handles CtxTambah.Click
        BtnPaketBaru.PerformClick()
    End Sub

    Private Sub CtxEdit_Click(sender As Object, e As EventArgs) Handles CtxEdit.Click
        BukaEdit()
    End Sub

    Private Sub CtxHapus_Click(sender As Object, e As EventArgs) Handles CtxHapus.Click
        BtnHapus.PerformClick()
    End Sub

    Private Sub CtxDetail_Click(sender As Object, e As EventArgs) Handles CtxDetail.Click
        TogglePopup()
    End Sub

    Private Sub CtxMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles CtxMenu.Opening
        Dim adaPaket As Boolean = DgvDaftarPaket.CurrentRow IsNot Nothing AndAlso
            DgvDaftarPaket.CurrentRow.Cells("Kode").Value IsNot Nothing AndAlso
            Not String.IsNullOrWhiteSpace(DgvDaftarPaket.CurrentRow.Cells("Kode").Value.ToString())
        CtxEdit.Enabled = adaPaket
        CtxHapus.Enabled = adaPaket
        CtxDetail.Enabled = adaPaket
        CtxDetail.Text = If(PanelPopup.Visible, "📋 Sembunyikan Detail", "📋 Tampilkan Detail")
    End Sub

    ' ========================================================================
    '  ROW POST PAINT
    ' ========================================================================
    Private Sub DgvDaftarPaket_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DgvDaftarPaket.RowPostPaint
        Using b As New SolidBrush(DgvDaftarPaket.RowHeadersDefaultCellStyle.ForeColor)
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b,
                                  e.RowBounds.Location.X + 8, e.RowBounds.Location.Y + 4)
        End Using
    End Sub

    ' ========================================================================
    '  CLOSE
    ' ========================================================================
    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub FormRakitan_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            If PanelPopup.Visible Then
                PanelPopup.Visible = False
            Else
                Close()
            End If
        End If
    End Sub

End Class
