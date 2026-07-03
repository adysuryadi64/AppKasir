Imports System.Globalization

Public Class FormEditBayarJual

    Public Property IdPenjualan As String = ""

    Private ReadOnly cultureId As New CultureInfo("id-ID")

    Private _isLoaded As Boolean = False
    Private _idPelanggan As String = ""
    Private _namaPelanggan As String = ""
    Private _alamatPelanggan As String = ""
    Private _jenisPelanggan As String = ""
    Private _lokasiBarang As String = ""
    Private _tglTransaksi As DateTime = DateTime.Today
    Private _idUserTransaksi As String = ""
    Private _idKomputerTransaksi As String = ""
    Private _totalQty As Decimal = 0D
    Private _grandTotalSblPajak As Decimal = 0D
    Private _diskonTotalPersen As Decimal = 0D
    Private _pajakPersen As Decimal = 0D
    Private _grandTotal As Decimal = 0D
    Private _diskonTotalRp As Decimal = 0D
    Private _pajakRp As Decimal = 0D
    Private _biayaKirim As Decimal = 0D
    Private _totalHpp As Decimal = 0D
    Private _diskonItem As Decimal = 0D

    Private Sub FormEditBayarJual_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        ModuleTheme.TerapkanTheme(Me)
        AturDesainForm()
        MuatDataAkun()
        MuatDataPenjualan()
        _isLoaded = True
        HitungPembayaran()
    End Sub

    ''' <summary>F8 = Simpan, Escape = Batal — berlaku dari manapun fokus berada di form ini.</summary>
    Private Sub FormEditBayarJual_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.F8
                e.SuppressKeyPress = True
                BtnSimpan.PerformClick()
            Case Keys.Escape
                e.SuppressKeyPress = True
                BtnBatal.PerformClick()
        End Select
    End Sub

    Private Sub FormEditBayarJual_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        SesuaikanTinggiForm()
        txtNominalTunai.Focus()
        txtNominalTunai.SelectAll()
    End Sub

    ''' <summary>Enter = pindah field berikutnya, F8 = Simpan, Escape = Batal</summary>
    Private Sub Input_KeyDown(sender As Object, e As KeyEventArgs) _
        Handles txtNominalTunai.KeyDown, txtNominalTransfer.KeyDown,
                cmbBayarTunai.KeyDown, cmbBayarTransfer.KeyDown,
                dtpJatuhTempo.KeyDown,
                cmbBankPengirim.KeyDown, txtNoRek.KeyDown, txtNamaRek.KeyDown, txtNoReff.KeyDown

        Select Case e.KeyCode
            Case Keys.F8
                e.SuppressKeyPress = True
                BtnSimpan.PerformClick()

            Case Keys.Escape
                e.SuppressKeyPress = True
                BtnBatal.PerformClick()

            Case Keys.Enter
                e.SuppressKeyPress = True
                Me.SelectNextControl(CType(sender, Control), True, True, True, True)
        End Select
    End Sub

    Private Sub SesuaikanTinggiForm()
        ' Hitung tinggi secara eksplisit dari semua komponen yang tampil
        ' PanelHeader (fixed) + TableHeader (fixed rows) + PanelSeparator (1px)
        ' + TablePembayaran (rows yang visible) + PanelFooter (dinamis)

        Dim tinggiHeader As Integer = PanelHeader.Height

        ' TableHeader: 4 baris x 28px + padding PanelBody atas
        Dim tinggiInfoPelanggan As Integer = TableHeader.RowCount * 28

        ' TablePembayaran: hitung baris yang visible
        Dim tinggiTabel As Integer = 0
        For i As Integer = 0 To TablePembayaran.RowStyles.Count - 1
            tinggiTabel += CInt(TablePembayaran.RowStyles(i).Height)
        Next

        ' PanelBody padding: top + bottom + separator 1px
        Dim tinggiBody As Integer = PanelBody.Padding.Top + tinggiInfoPelanggan +
                                    PanelSeparator.Height + tinggiTabel +
                                    PanelBody.Padding.Bottom

        ' PanelFooter sudah dihitung di AturTampilanInformasiTransfer
        Dim tinggiFooter As Integer = PanelFooter.Height

        Dim tinggi As Integer = tinggiHeader + tinggiBody + tinggiFooter + 4
        If tinggi < 300 Then tinggi = 300
        Me.ClientSize = New Size(480, tinggi)
    End Sub

    ' (tidak ada field warna — semua warna semantik inline di HitungPembayaran dan AturDesainForm)

    Private Sub AturDesainForm()
        ' TerapkanTheme sudah handle: BackColor form, Panel, Label, TextBox, Button, ComboBox, DateTimePicker
        ' Di sini hanya set hal yang bersifat logika bisnis / visual khusus form ini

        ' === Garis pemisah ===
        PanelSeparator.BackColor = Color.FromArgb(210, 218, 226)

        ' === Label info pelanggan: border agar terlihat seperti field read-only ===
        For Each lbl As Label In {lblFakturValue, lblPelangganValue, lblAlamatPelangganValue, lblJenisPelangganValue}
            lbl.BorderStyle = BorderStyle.FixedSingle
        Next

        ' === Display-only labels: warna abu-biru agar user tahu tidak bisa diedit ===
        Dim clrDisplay As Color = ModuleTheme.C(
            Color.FromArgb(230, 236, 245),
            Color.FromArgb(30, 41, 59))
        Dim clrDisplayFore As Color = ModuleTheme.C(
            Color.FromArgb(30, 50, 80),
            Color.FromArgb(200, 210, 230))
        For Each lbl As Label In {lblTotalBelanja, lblHasilValue, lblTotalFmt}
            lbl.BackColor = clrDisplay
            lbl.ForeColor = clrDisplayFore
        Next

        ' === Panel info transfer: border ===
        PanelInfoTransfer.BorderStyle = BorderStyle.FixedSingle

        ' === Sembunyikan field internal ===
        Label6.Visible = False
        txtKodeBayarTunai.Visible = False
        Label9.Visible = False
        txtKodeBayarTransfer.Visible = False
        lblTunaiFmt.Visible = False
        lblTransferFmt.Visible = False

        ' === Row kode akun disembunyikan ===
        TablePembayaran.RowStyles(2).Height = 0
        TablePembayaran.RowStyles(4).Height = 0

        TablePembayaran.AutoSize = True
        TablePembayaran.AutoSizeMode = AutoSizeMode.GrowAndShrink

        AturTampilanInformasiTransfer(ModuleAngka.ParseDecimal(txtNominalTransfer.Text))
        ' SesuaikanTinggiForm dipanggil di Shown event setelah layout selesai
    End Sub

    Private Sub MuatDataAkun()
        IsiComboBoxAkun(cmbBayarTunai, "KAS")
        IsiComboBoxAkun(cmbBayarTransfer, "BANK")
    End Sub

    Private Sub MuatDataPenjualan()
        If String.IsNullOrWhiteSpace(IdPenjualan) Then
            Throw New InvalidOperationException("IdPenjualan belum diisi.")
        End If

        Dim jenisPembayaranTunai As String = ""
        Dim kodeAkunTunai As String = ""
        Dim namaAkunTransfer As String = ""
        Dim kodeAkunTransfer As String = ""
        Dim nominalTunai As Decimal = 0D
        Dim nominalTransfer As Decimal = 0D
        Dim bank As String = ""
        Dim noRek As String = ""
        Dim namaRek As String = ""
        Dim noReff As String = ""
        Dim jatuhTempo As DateTime? = Nothing

        Dim sql As String =
            "SELECT ID_PELANGGAN, NAMA_PELANGGAN, ALAMAT_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, ID_USER, ID_KOMPUTER, " &
            "GRAND_TOTAL_SBL_PAJAK, DISKON_TOTAL_PERSEN, DISKON_TOTAL_RP, PAJAK_PERSEN, PAJAK_RP, GRAND_TOTAL_STL_PAJAK, BIAYA_KIRIM, TOTAL_HPP, " &
            "BAYAR, NOMINAL_TRANSFER, KEMBALI, SISA_TAGIHAN, JATUH_TEMPO, STATUS_TRANSAKSI, " &
            "KODE_AKUN, JENIS_PEMBAYARAN, KODE_AKUNBANK, NAMA_AKUN_TF, BANK, NO_REKENING, NAMA_REKENING, NO_REFFERENSI " &
            "FROM penjualan WHERE ID_PENJUALAN = @ID_PENJUALAN LIMIT 1"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@ID_PENJUALAN", IdPenjualan)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If Not rd.Read() Then
                    Throw New Exception("Data penjualan tidak ditemukan.")
                End If

                _idPelanggan = rd("ID_PELANGGAN").ToString()
                _namaPelanggan = rd("NAMA_PELANGGAN").ToString()
                _alamatPelanggan = ModuleAngka.SafeGetValue(Of String)(rd, "ALAMAT_PELANGGAN", "")
                _jenisPelanggan = ModuleAngka.SafeGetValue(Of String)(rd, "JENIS_PELANGGAN", "")
                _lokasiBarang = rd("LOKASIBARANG").ToString()
                _tglTransaksi = ModuleAngka.SafeGetValue(Of DateTime)(rd, "TGL_TRANSAKSI", Date.Today)
                _idUserTransaksi = rd("ID_USER").ToString()
                _idKomputerTransaksi = rd("ID_KOMPUTER").ToString()
                _grandTotalSblPajak = ModuleAngka.ParseDecimal(rd("GRAND_TOTAL_SBL_PAJAK"))
                _diskonTotalPersen = ModuleAngka.ParseDecimal(rd("DISKON_TOTAL_PERSEN"))
                _pajakPersen = ModuleAngka.ParseDecimal(rd("PAJAK_PERSEN"))
                _grandTotal = ModuleAngka.ParseDecimal(rd("GRAND_TOTAL_STL_PAJAK"))
                _diskonTotalRp = ModuleAngka.ParseDecimal(rd("DISKON_TOTAL_RP"))
                _pajakRp = ModuleAngka.ParseDecimal(rd("PAJAK_RP"))
                _biayaKirim = ModuleAngka.ParseDecimal(rd("BIAYA_KIRIM"))
                _totalHpp = ModuleAngka.ParseDecimal(rd("TOTAL_HPP"))

                jenisPembayaranTunai = rd("JENIS_PEMBAYARAN").ToString()
                kodeAkunTunai = rd("KODE_AKUN").ToString()
                nominalTunai = ModuleAngka.ParseDecimal(rd("BAYAR"))

                namaAkunTransfer = rd("NAMA_AKUN_TF").ToString()
                kodeAkunTransfer = rd("KODE_AKUNBANK").ToString()
                nominalTransfer = ModuleAngka.ParseDecimal(rd("NOMINAL_TRANSFER"))

                bank = rd("BANK").ToString()
                noRek = rd("NO_REKENING").ToString()
                namaRek = rd("NAMA_REKENING").ToString()
                noReff = rd("NO_REFFERENSI").ToString()

                If Not IsDBNull(rd("JATUH_TEMPO")) Then
                    jatuhTempo = Convert.ToDateTime(rd("JATUH_TEMPO"))
                End If
            End Using
        End Using

        lblFakturValue.Text = IdPenjualan
        lblPelangganValue.Text = _namaPelanggan
        lblAlamatPelangganValue.Text = _alamatPelanggan
        lblJenisPelangganValue.Text = _jenisPelanggan

        PilihCombo(cmbBayarTunai, jenisPembayaranTunai)
        txtKodeBayarTunai.Text = kodeAkunTunai
        txtNominalTunai.Text = nominalTunai.ToString("0.####", CultureInfo.InvariantCulture)

        PilihCombo(cmbBayarTransfer, namaAkunTransfer)
        txtKodeBayarTransfer.Text = kodeAkunTransfer
        txtNominalTransfer.Text = nominalTransfer.ToString("0.####", CultureInfo.InvariantCulture)

        cmbBankPengirim.Text = bank
        txtNoRek.Text = noRek
        txtNamaRek.Text = namaRek
        txtNoReff.Text = noReff

        If jatuhTempo.HasValue Then
            dtpJatuhTempo.Value = jatuhTempo.Value
        Else
            dtpJatuhTempo.Value = _tglTransaksi.AddMonths(1)
        End If

        Using cmdDiskon As New MySqlCommand("SELECT COALESCE(SUM(TOTAL_DISKON), 0) FROM penjualan_detail WHERE FAKTUR_JUAL = @FAKTUR", conn)
            cmdDiskon.Parameters.AddWithValue("@FAKTUR", IdPenjualan)
            _diskonItem = ModuleAngka.ParseDecimal(cmdDiskon.ExecuteScalar())
        End Using

        Using cmdQty As New MySqlCommand("SELECT COALESCE(SUM(QTY_SATUAN), 0) FROM penjualan_detail WHERE FAKTUR_JUAL = @FAKTUR", conn)
            cmdQty.Parameters.AddWithValue("@FAKTUR", IdPenjualan)
            _totalQty = ModuleAngka.ParseDecimal(cmdQty.ExecuteScalar())
        End Using
    End Sub

    Private Sub PilihCombo(combo As ComboBox, value As String)
        If String.IsNullOrWhiteSpace(value) Then Exit Sub
        Dim idx As Integer = combo.FindStringExact(value)
        If idx >= 0 Then
            combo.SelectedIndex = idx
        Else
            combo.Items.Add(value)
            combo.SelectedItem = value
        End If
    End Sub

    Private Sub CmbBayarTunai_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBayarTunai.SelectedIndexChanged
        txtKodeBayarTunai.Text = GetKodeAkun(cmbBayarTunai.Text)
    End Sub

    Private Sub CmbBayarTransfer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBayarTransfer.SelectedIndexChanged
        txtKodeBayarTransfer.Text = GetKodeAkun(cmbBayarTransfer.Text)
    End Sub

    Private Sub Nominal_TextChanged(sender As Object, e As EventArgs) Handles txtNominalTunai.TextChanged, txtNominalTransfer.TextChanged
        If Not _isLoaded Then Exit Sub
        HitungPembayaran()
    End Sub

    Private Sub HitungPembayaran()
        Dim tunai As Decimal = ModuleAngka.ParseDecimal(txtNominalTunai.Text)
        Dim transfer As Decimal = ModuleAngka.ParseDecimal(txtNominalTransfer.Text)
        Dim totalBayar As Decimal = tunai + transfer
        Dim selisih As Decimal = totalBayar - _grandTotal

        AturTampilanInformasiTransfer(transfer)

        lblTotalFmt.Text = _grandTotal.ToString("#,0.####", cultureId)
        lblTotalBelanja.Text = _grandTotal.ToString("#,0.####", cultureId)

        ' Row 6 = Jatuh Tempo (index 6, height 0 saat hidden)
        Const rowJatuhTempo As Integer = 6

        If selisih < 0 Then
            lblStatusValue.Text = "Belum Lunas"
            ModuleTheme.SetWarnaStatusTransaksi(lblStatusValue, False)
            lblHasilCaption.Text = "Hutang :"
            lblHasilValue.Text = Math.Abs(selisih).ToString("#,0.####", cultureId)
            lblHasilValue.BackColor = ModuleTheme.C(ModuleTheme.L_NotifDanger, ModuleTheme.D_NotifDanger)
            lblHasilValue.ForeColor = ModuleTheme.C(ModuleTheme.L_Danger, ModuleTheme.D_Danger)
            lblJatuhTempo.Visible = True
            dtpJatuhTempo.Visible = True
            If TablePembayaran.RowStyles.Count > rowJatuhTempo Then
                TablePembayaran.RowStyles(rowJatuhTempo).Height = 36
            End If
            SesuaikanTinggiForm()
        Else
            lblStatusValue.Text = "Lunas"
            ModuleTheme.SetWarnaStatusTransaksi(lblStatusValue, True)
            lblHasilCaption.Text = "Kembalian :"
            lblHasilValue.Text = selisih.ToString("#,0.####", cultureId)
            lblHasilValue.BackColor = ModuleTheme.C(Color.FromArgb(220, 252, 231), Color.FromArgb(20, 83, 45))  ' Green-100 / Green-900
            lblHasilValue.ForeColor = ModuleTheme.C(ModuleTheme.L_Success, ModuleTheme.D_Success)
            lblJatuhTempo.Visible = False
            dtpJatuhTempo.Visible = False
            If TablePembayaran.RowStyles.Count > rowJatuhTempo Then
                TablePembayaran.RowStyles(rowJatuhTempo).Height = 0
            End If
            SesuaikanTinggiForm()
        End If
    End Sub

    Private Sub AturTampilanInformasiTransfer(nominalTransfer As Decimal)
        Dim tampil As Boolean = nominalTransfer > 0

        PanelInfoTransfer.Visible = tampil

        ' Hitung tinggi footer
        Const gap As Integer = 8
        Dim btnH As Integer = If(BtnSimpan.Height > 0, BtnSimpan.Height, 32)
        Dim padV As Integer = PanelFooter.Padding.Top + PanelFooter.Padding.Bottom
        Dim transferH As Integer = If(tampil, PanelInfoTransfer.Height + gap, 0)
        PanelFooter.Height = padV + transferH + btnH + gap

        ' Posisi tombol
        Dim tombolTop As Integer = PanelFooter.ClientSize.Height - PanelFooter.Padding.Bottom - btnH
        If tombolTop < 0 Then tombolTop = PanelFooter.Padding.Top
        BtnSimpan.Top = tombolTop
        BtnBatal.Top = tombolTop
        BtnSimpan.Left = PanelFooter.Padding.Left
        BtnBatal.Left = PanelFooter.ClientSize.Width - PanelFooter.Padding.Right - BtnBatal.Width

        If Not tampil Then
            cmbBankPengirim.Text = ""
            txtNoRek.Clear()
            txtNamaRek.Clear()
            txtNoReff.Clear()
            txtKodeBayarTransfer.Text = ""
        End If

        SesuaikanTinggiForm()
    End Sub

    Private Function GetKodeAkun(namaAkun As String) As String
        If String.IsNullOrWhiteSpace(namaAkun) Then Return ""

        Using cmd As New MySqlCommand("SELECT KODE_AKUN FROM tbl_datareferensi WHERE NAMA_AKUN = @NAMA LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@NAMA", namaAkun)
            Dim result As Object = cmd.ExecuteScalar()
            Return If(result Is Nothing OrElse IsDBNull(result), "", result.ToString())
        End Using
    End Function

    ' ParseDecimal lokal dihapus — gunakan ModuleAngka.ParseDecimal

    Private Function ValidasiInput() As Boolean
        Dim tunai As Decimal = ModuleAngka.ParseDecimal(txtNominalTunai.Text)
        Dim transfer As Decimal = ModuleAngka.ParseDecimal(txtNominalTransfer.Text)

        If tunai > 0 AndAlso String.IsNullOrWhiteSpace(cmbBayarTunai.Text) Then
            MessageBox.Show("Pilih akun pembayaran tunai terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbBayarTunai.Focus()
            Return False
        End If

        If transfer > 0 Then
            If String.IsNullOrWhiteSpace(cmbBayarTransfer.Text) Then
                MessageBox.Show("Pilih akun pembayaran transfer terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbBayarTransfer.Focus()
                Return False
            End If

            If String.IsNullOrWhiteSpace(cmbBankPengirim.Text) Then
                MessageBox.Show("Nama bank wajib diisi jika ada pembayaran transfer.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbBankPengirim.Focus()
                Return False
            End If

            If String.IsNullOrWhiteSpace(txtNoRek.Text) Then
                MessageBox.Show("Nomor rekening wajib diisi jika ada pembayaran transfer.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtNoRek.Focus()
                Return False
            End If

            If String.IsNullOrWhiteSpace(txtNamaRek.Text) Then
                MessageBox.Show("Nama rekening wajib diisi jika ada pembayaran transfer.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtNamaRek.Focus()
                Return False
            End If
        End If

        If lblStatusValue.Text = "Belum Lunas" AndAlso dtpJatuhTempo.Value.Date <= _tglTransaksi.Date Then
            MessageBox.Show("Tanggal jatuh tempo harus lebih besar dari tanggal transaksi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            dtpJatuhTempo.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles BtnSimpan.Click
        If Not ValidasiInput() Then Exit Sub

        Dim sw As System.Diagnostics.Stopwatch = System.Diagnostics.Stopwatch.StartNew()
        Dim lap As Long = 0
        Dim logPrefix As String = $"[PERF][EditBayarJual][{IdPenjualan}] "
        Dim logStep As Action(Of String) =
            Sub(nama As String)
                Dim nowMs As Long = sw.ElapsedMilliseconds
                System.Diagnostics.Debug.WriteLine(logPrefix & $"{nama} -> +{nowMs - lap} ms (total {nowMs} ms)")
                lap = nowMs
            End Sub

        logStep("Mulai BtnSimpan_Click")

        Dim tunai As Decimal = ModuleAngka.ParseDecimal(txtNominalTunai.Text)
        Dim transfer As Decimal = ModuleAngka.ParseDecimal(txtNominalTransfer.Text)
        Dim totalBayar As Decimal = tunai + transfer
        Dim selisih As Decimal = totalBayar - _grandTotal
        Dim sisaTagihan As Decimal = If(selisih < 0, Math.Abs(selisih), 0D)
        Dim kembali As Decimal = If(selisih > 0, selisih, 0D)
        Dim statusTransaksi As String = If(sisaTagihan > 0, "Belum Lunas", "Lunas")
        Dim statusBayar As String = If(statusTransaksi = "Lunas", "TERBAYAR", "TERHUTANG")
        Dim metode As String = "Tunai"
        Dim userTransaksi As String = If(String.IsNullOrWhiteSpace(_idUserTransaksi), FormUtama.StatusNamaUser.Text, _idUserTransaksi)
        Dim komputerTransaksi As String = If(String.IsNullOrWhiteSpace(_idKomputerTransaksi), FormUtama.StatusNamaPC.Text, _idKomputerTransaksi)

        If transfer > 0 Then
            metode = "Tunai + Transfer"
        End If

        Dim transaksi As MySqlTransaction = conn.BeginTransaction()
        logStep("BeginTransaction")

        Try
            ' ========================================
            ' ========================================
            ' LANGKAH 1: AUDIT TRAIL
            ' ========================================
            ModuleAuditTrail.CatatAudit(IdPenjualan, "EDIT", "Bayar Piutang", ket:="[KRITIS] Edit bayar piutang", trans:=transaksi)
            logStep("✅ Catat audit trail")

            ' ========================================
            ' LANGKAH 3: UPDATE DATA PENJUALAN
            ' ========================================
            Dim sqlUpdate As String =
                "UPDATE penjualan SET " &
                "BAYAR = @BAYAR, " &
                "NOMINAL_TRANSFER = @NOMINAL_TRANSFER, " &
                "KEMBALI = @KEMBALI, " &
                "SISA_TAGIHAN = @SISA_TAGIHAN, " &
                "JATUH_TEMPO = @JATUH_TEMPO, " &
                "STATUS_BAYAR = @STATUS_BAYAR, " &
                "STATUS_TRANSAKSI = @STATUS_TRANSAKSI, " &
                "GRAND_TOTAL_SBL_PAJAK = @GRAND_TOTAL_SBL_PAJAK, " &
                "DISKON_TOTAL_PERSEN = @DISKON_TOTAL_PERSEN, " &
                "DISKON_TOTAL_RP = @DISKON_TOTAL_RP, " &
                "PAJAK_PERSEN = @PAJAK_PERSEN, " &
                "PAJAK_RP = @PAJAK_RP, " &
                "GRAND_TOTAL_STL_PAJAK = @GRAND_TOTAL_STL_PAJAK, " &
                "TOTAL_HPP = @TOTAL_HPP, " &
                "BIAYA_KIRIM = @BIAYA_KIRIM, " &
                "TYPE_AKUN = @TYPE_AKUN, " &
                "KODE_AKUN = @KODE_AKUN, " &
                "JENIS_PEMBAYARAN = @JENIS_PEMBAYARAN, " &
                "KODE_AKUN_TF = @KODE_AKUN_TF, " &
                "NAMA_AKUN_TF = @NAMA_AKUN_TF, " &
                "TYPE_AKUNBANK = @TYPE_AKUNBANK, " &
                "KODE_AKUNBANK = @KODE_AKUNBANK, " &
                "JENIS_PEMBAYARANBANK = @JENIS_PEMBAYARANBANK, " &
                "METODE = @METODE, " &
                "BANK = @BANK, " &
                "NO_REKENING = @NO_REKENING, " &
                "NAMA_REKENING = @NAMA_REKENING, " &
                "NO_REFFERENSI = @NO_REFFERENSI, " &
                "ID_USER = @ID_USER, " &
                "ID_KOMPUTER = @ID_KOMPUTER " &
                "WHERE ID_PENJUALAN = @ID_PENJUALAN"

            Using cmd As New MySqlCommand(sqlUpdate, conn, transaksi)
                cmd.Parameters.AddWithValue("@BAYAR", tunai)
                cmd.Parameters.AddWithValue("@NOMINAL_TRANSFER", transfer)
                cmd.Parameters.AddWithValue("@KEMBALI", kembali)
                cmd.Parameters.AddWithValue("@SISA_TAGIHAN", sisaTagihan)
                cmd.Parameters.AddWithValue("@JATUH_TEMPO", If(sisaTagihan > 0, CType(dtpJatuhTempo.Value.ToString("yyyy-MM-dd"), Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@STATUS_BAYAR", statusBayar)
                cmd.Parameters.AddWithValue("@STATUS_TRANSAKSI", statusTransaksi)
                cmd.Parameters.AddWithValue("@GRAND_TOTAL_SBL_PAJAK", _grandTotalSblPajak)
                cmd.Parameters.AddWithValue("@DISKON_TOTAL_PERSEN", _diskonTotalPersen)
                cmd.Parameters.AddWithValue("@DISKON_TOTAL_RP", _diskonTotalRp)
                cmd.Parameters.AddWithValue("@PAJAK_PERSEN", _pajakPersen)
                cmd.Parameters.AddWithValue("@PAJAK_RP", _pajakRp)
                cmd.Parameters.AddWithValue("@GRAND_TOTAL_STL_PAJAK", _grandTotal)
                cmd.Parameters.AddWithValue("@TOTAL_HPP", _totalHpp)
                cmd.Parameters.AddWithValue("@BIAYA_KIRIM", _biayaKirim)
                cmd.Parameters.AddWithValue("@TYPE_AKUN", "KAS")
                cmd.Parameters.AddWithValue("@KODE_AKUN", txtKodeBayarTunai.Text)
                cmd.Parameters.AddWithValue("@JENIS_PEMBAYARAN", cmbBayarTunai.Text)
                cmd.Parameters.AddWithValue("@KODE_AKUN_TF", If(transfer > 0, txtKodeBayarTransfer.Text, ""))
                cmd.Parameters.AddWithValue("@NAMA_AKUN_TF", If(transfer > 0, cmbBayarTransfer.Text, ""))
                cmd.Parameters.AddWithValue("@TYPE_AKUNBANK", "BANK")
                cmd.Parameters.AddWithValue("@KODE_AKUNBANK", txtKodeBayarTransfer.Text)
                cmd.Parameters.AddWithValue("@JENIS_PEMBAYARANBANK", cmbBayarTransfer.Text)
                cmd.Parameters.AddWithValue("@METODE", metode)
                cmd.Parameters.AddWithValue("@BANK", If(transfer > 0, cmbBankPengirim.Text.Trim(), ""))
                cmd.Parameters.AddWithValue("@NO_REKENING", If(transfer > 0, txtNoRek.Text.Trim(), ""))
                cmd.Parameters.AddWithValue("@NAMA_REKENING", If(transfer > 0, txtNamaRek.Text.Trim(), ""))
                cmd.Parameters.AddWithValue("@NO_REFFERENSI", If(transfer > 0, txtNoReff.Text.Trim(), ""))
                cmd.Parameters.AddWithValue("@ID_USER", userTransaksi)
                cmd.Parameters.AddWithValue("@ID_KOMPUTER", komputerTransaksi)
                cmd.Parameters.AddWithValue("@ID_PENJUALAN", IdPenjualan)
                cmd.ExecuteNonQuery()
            End Using
            logStep("✅ UPDATE tabel penjualan")

            ' ========================================
            ' LANGKAH 3b: SINKRONISASI BARIS TIMBUL DI piutang_detail
            ' Update nilai HUTANG dan DIBAYAR pada baris TIMBUL agar mencerminkan
            ' nilai pembayaran terbaru. Jika baris TIMBUL tidak ditemukan (faktur lama
            ' sebelum migrasi), tidak error — lanjutkan saja.
            ' ========================================
            Using cmdUpdateTimbul As New MySqlCommand(
                "UPDATE piutang_detail SET " &
                "HUTANG = @SISA_TAGIHAN_BARU, " &
                "DIBAYAR = @TOTAL_BAYAR_BARU, " &
                "STATUS = CASE WHEN @SISA_TAGIHAN_BARU <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END " &
                "WHERE ID_JUAL = @ID_JUAL AND JENIS = 'JUAL'", conn, transaksi)
                cmdUpdateTimbul.Parameters.AddWithValue("@SISA_TAGIHAN_BARU", sisaTagihan)
                cmdUpdateTimbul.Parameters.AddWithValue("@TOTAL_BAYAR_BARU", totalBayar)
                cmdUpdateTimbul.Parameters.AddWithValue("@ID_JUAL", IdPenjualan)
                cmdUpdateTimbul.ExecuteNonQuery()
            End Using
            logStep("✅ UPDATE piutang_detail TIMBUL")

            ' ========================================
            ' LANGKAH 4: DELETE JURNAL UMUM LAMA
            ' ========================================
            ' Reversal saldo akun SEBELUM DELETE JurnalUmum
            ReversalSaldoAkunDariFaktur(IdPenjualan, transaksi)
            logStep("✅ ReversalSaldoAkun jurnal lama")

            Using cmdDeleteJurnal As New MySqlCommand("DELETE FROM JurnalUmum WHERE NO_TRANSAKSI = @NO_TRANSAKSI", conn, transaksi)
                cmdDeleteJurnal.Parameters.AddWithValue("@NO_TRANSAKSI", IdPenjualan)
                cmdDeleteJurnal.ExecuteNonQuery()
            End Using
            logStep("✅ DELETE JurnalUmum lama")

            ' ========================================
            ' LANGKAH 5: SINCRONKAN PIUTANG
            ' ========================================
            SinkronkanPenjualanPiutang(transaksi, tunai, sisaTagihan, statusTransaksi, userTransaksi, komputerTransaksi)
            logStep("✅ SinkronkanPenjualanPiutang")

            ' ========================================
            ' LANGKAH 6: INSERT JURNAL UMUM BARU
            ' ========================================
            Dim jD As Decimal = 0D
            Dim jK As Decimal = 0D
            BangunUlangJurnal(transaksi, tunai, transfer, sisaTagihan, userTransaksi, komputerTransaksi, jD, jK)
            logStep("✅ BangunUlangJurnal (insert jurnal baru)")

            ' ========================================
            ' LANGKAH 7: UPDATE SALDO AKUN — incremental delta
            ' ========================================
            UpdateSaldoAkunDeltaDariFaktur(IdPenjualan, transaksi)
            logStep("✅ UpdateSaldoAkunDelta")

            ' ========================================
            ' LANGKAH 10: UPDATE PIUTANG PELANGGAN & COMMIT
            ' ========================================
            UpdatePiutangPelanggan(_idPelanggan, transaksi)
            logStep("✅ UpdatePiutangPelanggan")
            transaksi.Commit()
            logStep("✅ Commit transaksi")

            CatatJurnalTidakSeimbang(IdPenjualan, jD, jK, "Edit Bayar Penjualan",
                {"Tunai", "Transfer", "Piutang", "DiskonItem", "DiskonTotal", "HPP_Debet", "Penjualan_Kredit", "HPP_Kredit", "Pajak", "BiayaKirim"})
            logStep("CatatJurnalTidakSeimbang")

            MessageBox.Show("Pembayaran penjualan berhasil diperbarui.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            DialogResult = DialogResult.OK
            Close()
        Catch ex As Exception
            transaksi.Rollback()
            logStep("Rollback")
            System.Diagnostics.Debug.WriteLine(logPrefix & "ERROR: " & ex.Message)
            MessageBox.Show("Gagal mengubah pembayaran penjualan." & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub BangunUlangJurnal(transaction As MySqlTransaction,
                                  bayarTunai As Decimal,
                                  bayarTransfer As Decimal,
                                  sisaTagihan As Decimal,
                                  idUser As String,
                                  idKomputer As String,
                                  ByRef outDebet As Decimal,
                                  ByRef outKredit As Decimal)

        ' ═══════════════════════════════════════════════════════════════════
        ' PERHITUNGAN DASAR — sama persis dengan Simpanjurnal di FormPenjualan
        ' ═══════════════════════════════════════════════════════════════════

        ' Hitung kas tunai yang benar-benar diterima (bukan nominal bayar penuh)
        ' Kembalian tidak dijurnal
        Dim kasTransfer As Decimal = bayarTransfer
        Dim kasTunaiDiterima As Decimal = _grandTotal - kasTransfer - sisaTagihan
        If kasTunaiDiterima < 0 Then kasTunaiDiterima = 0

        ' nilaiPenjualanKotor = total jual sebelum semua diskon
        ' _grandTotalSblPajak sudah dikurangi diskon item, tambahkan kembali _diskonItem
        Dim nilaiPenjualanKotor As Decimal = _grandTotalSblPajak + _diskonItem

        Dim totalDebet As Decimal = 0D
        Dim totalKredit As Decimal = 0D

        ' ═══════════════════════════════════════════════════════════════════
        ' J1: KAS TUNAI DITERIMA — Debet
        ' ═══════════════════════════════════════════════════════════════════
        If kasTunaiDiterima > 0 Then
            Dim uraianTunai As String = If(sisaTagihan = 0 AndAlso kasTransfer = 0,
                                            "Penjualan tunai lunas dari " & _namaPelanggan,
                                            "Penjualan pembayaran tunai (sebagian) dari " & _namaPelanggan)
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi, uraianTunai,
                                 cmbBayarTunai.Text, txtKodeBayarTunai.Text, "", "",
                                 kasTunaiDiterima, "Penjualan", "", "", idUser, idKomputer)
            totalDebet += kasTunaiDiterima
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' J2: TRANSFER DITERIMA — Debet
        ' ═══════════════════════════════════════════════════════════════════
        If kasTransfer > 0 Then
            Dim uraianTransfer As String = "Penjualan pembayaran transfer ke " & cmbBayarTransfer.Text & " a.n " & txtNamaRek.Text
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi, uraianTransfer,
                                 cmbBayarTransfer.Text, txtKodeBayarTransfer.Text, "", "",
                                 kasTransfer, "Penjualan", "", "", idUser, idKomputer)
            totalDebet += kasTransfer
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' J3: PIUTANG USAHA — Debet (jika belum lunas)
        ' ═══════════════════════════════════════════════════════════════════
        If sisaTagihan > 0 Then
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi,
                                 "Piutang penjualan dari " & _namaPelanggan,
                                 nama_rek_Piutang_Jual, Kode_rek_Piutang_Jual, "", "",
                                 sisaTagihan, "Penjualan", _namaPelanggan, _idPelanggan, idUser, idKomputer)
            totalDebet += sisaTagihan
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' J4: DISKON ITEM — Debet ke POTONGAN DISKON PENJUALAN (05.04.001)
        ' ═══════════════════════════════════════════════════════════════════
        If _diskonItem > 0 Then
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi,
                                 "Diskon item penjualan dari " & _namaPelanggan,
                                 "POTONGAN DISKON PENJUALAN", "05.04.001", "", "",
                                 _diskonItem, "Penjualan", "", "", idUser, idKomputer)
            totalDebet += _diskonItem
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' J5: DISKON TOTAL — Debet ke POTONGAN DISKON PENJUALAN (05.04.001)
        ' ═══════════════════════════════════════════════════════════════════
        If _diskonTotalRp > 0 Then
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi,
                                 "Diskon total penjualan dari " & _namaPelanggan,
                                 "POTONGAN DISKON PENJUALAN", "05.04.001", "", "",
                                 _diskonTotalRp, "Penjualan", "", "", idUser, idKomputer)
            totalDebet += _diskonTotalRp
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' J6: HPP POKOK PENJUALAN — Debet (06.01.001)
        ' ═══════════════════════════════════════════════════════════════════
        If _totalHpp > 0 Then
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi,
                                 "HPP penjualan kepada " & _namaPelanggan,
                                 "HPP POKOK PENJUALAN", "06.01.001", "", "",
                                 _totalHpp, "Penjualan", "", "", idUser, idKomputer)
            totalDebet += _totalHpp
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' J7: PENJUALAN KOTOR — Kredit ke PENJUALAN (05.02.001)
        ' ═══════════════════════════════════════════════════════════════════
        If nilaiPenjualanKotor > 0 Then
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi,
                                 "Penjualan kepada " & _namaPelanggan,
                                 "", "", "PENJUALAN", "05.02.001",
                                 nilaiPenjualanKotor, "Penjualan", "", "", idUser, idKomputer)
            totalKredit += nilaiPenjualanKotor
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' J8: PERSEDIAAN BARANG KELUAR — Kredit (HPP keluar dari gudang)
        ' ═══════════════════════════════════════════════════════════════════
        If _totalHpp > 0 Then
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi,
                                 "Keluar persediaan HPP penjualan kepada " & _namaPelanggan,
                                 "", "", NAMA_REK_BARANG, KODE_REK_BARANG,
                                 _totalHpp, "Penjualan", "", "", idUser, idKomputer)
            totalKredit += _totalHpp
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' J9: HUTANG PAJAK — Kredit (03.02.001)
        ' ═══════════════════════════════════════════════════════════════════
        If _pajakRp > 0 Then
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi,
                                 "Hutang pajak penjualan dari " & _namaPelanggan,
                                 "", "", "HUTANG PAJAK", "03.02.001",
                                 _pajakRp, "Penjualan", "", "", idUser, idKomputer)
            totalKredit += _pajakRp
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' J10: BIAYA KIRIM — Kredit ke PENDAPATAN LAIN LAIN (08.01.002)
        ' ═══════════════════════════════════════════════════════════════════
        If _biayaKirim > 0 Then
            SimpanJurnalUmumEdit(transaction, IdPenjualan, _tglTransaksi,
                                 "Jasa kirim/Lain " & _namaPelanggan,
                                 "", "", "PENDAPATAN LAIN LAIN", "08.01.002",
                                 _biayaKirim, "Penjualan", "", "", idUser, idKomputer)
            totalKredit += _biayaKirim
        End If

        ' ═══════════════════════════════════════════════════════════════════
        ' DEBUG: Cek keseimbangan debet vs kredit
        ' ═══════════════════════════════════════════════════════════════════
        Debug.WriteLine("═══════════════════════════════════════════════════════════════════")
        Debug.WriteLine("DEBUG JURNAL EDIT BAYAR JUAL - Faktur: " & IdPenjualan & " | " & _namaPelanggan)
        Debug.WriteLine("═══════════════════════════════════════════════════════════════════")
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J1", "Kas Tunai", cmbBayarTunai.Text & "[" & txtKodeBayarTunai.Text & "]", "-", kasTunaiDiterima, 0))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J2", "Transfer", cmbBayarTransfer.Text & "[" & txtKodeBayarTransfer.Text & "]", "-", kasTransfer, 0))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J3", "Piutang", Kode_rek_Piutang_Jual, "-", sisaTagihan, 0))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J4", "Diskon Item", "05.04.001", "-", _diskonItem, 0))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J5", "Diskon Total", "05.04.001", "-", _diskonTotalRp, 0))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J6", "HPP Debet", "06.01.001", "-", _totalHpp, 0))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J7", "Penjualan Kotor", "-", "05.02.001", 0, nilaiPenjualanKotor))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J8", "Persediaan Keluar", "-", KODE_REK_BARANG, 0, _totalHpp))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J9", "Hutang Pajak", "-", "03.02.001", 0, _pajakRp))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "J10", "Biaya Kirim", "-", "08.01.002", 0, _biayaKirim))
        Debug.WriteLine(New String("-", 135))
        Debug.WriteLine(String.Format("{0,-5} {1,-35} {2,-30} {3,-30} {4,15:N0} {5,15:N0}", "", "TOTAL", "", "", totalDebet, totalKredit))
        Dim selisihDebKred As Decimal = totalDebet - totalKredit
        If selisihDebKred = 0 Then
            Debug.WriteLine("✅ JURNAL SEIMBANG - D=K=" & totalDebet.ToString("N0"))
        Else
            Debug.WriteLine("❌ JURNAL TIDAK SEIMBANG - Selisih=" & selisihDebKred.ToString("N0") &
                            " | D=" & totalDebet.ToString("N0") & " | K=" & totalKredit.ToString("N0"))
        End If
        Debug.WriteLine("═══════════════════════════════════════════════════════════════════")

        outDebet = totalDebet
        outKredit = totalKredit
    End Sub

    Private Sub SinkronkanPenjualanPiutang(transaction As MySqlTransaction,
                                           bayarTunai As Decimal,
                                           sisaTagihan As Decimal,
                                           statusTransaksi As String,
                                           idUser As String,
                                           idKomputer As String)

        Dim kolom As HashSet(Of String) = AmbilKolomTabel("penjualan_Piutang", transaction)
        If kolom.Count = 0 Then Exit Sub

        Dim kolomId As String = PilihNamaKolom(kolom, {"IDPENJUALAN", "ID_PENJUALAN"})
        If String.IsNullOrWhiteSpace(kolomId) Then Exit Sub

        Using cmdDelete As New MySqlCommand($"DELETE FROM penjualan_Piutang WHERE {kolomId} = @ID", conn, transaction)
            cmdDelete.Parameters.AddWithValue("@ID", IdPenjualan)
            cmdDelete.ExecuteNonQuery()
        End Using

        If sisaTagihan <= 0 Then Exit Sub

        Dim daftarKolom As New List(Of String)
        Dim daftarParameter As New List(Of String)
        Dim nilaiKolom As New Dictionary(Of String, Object)

        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"IDPENJUALAN", "ID_PENJUALAN"}, IdPenjualan)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"TGL_BELANJA", "TGL_TRANSAKSI"}, _tglTransaksi.ToString("yyyy-MM-dd HH:mm:ss"))
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"LOKASI"}, _lokasiBarang)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"KODE_PELANGGAN", "ID_PELANGGAN"}, _idPelanggan)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"NAMA_PELANGGAN"}, _namaPelanggan)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"QTY"}, _totalQty)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"TOTAL_RUPIAH", "GRAND_TOTAL_STL_PAJAK"}, _grandTotal)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"BAYAR"}, bayarTunai)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"HUTANG", "SISA_TAGIHAN"}, sisaTagihan)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"JATUH_TEMPO"}, dtpJatuhTempo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"STATUS"}, statusTransaksi)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"ID_USER"}, idUser)
        TambahKolomPenjualanPiutang(kolom, daftarKolom, daftarParameter, nilaiKolom, {"ID_KOMPUTER"}, idKomputer)

        If daftarKolom.Count = 0 Then Exit Sub

        Dim sql As String = $"INSERT INTO penjualan_Piutang ({String.Join(", ", daftarKolom)}) VALUES ({String.Join(", ", daftarParameter)})"
        Using cmdInsert As New MySqlCommand(sql, conn, transaction)
            For Each item In nilaiKolom
                cmdInsert.Parameters.AddWithValue(item.Key, item.Value)
            Next
            cmdInsert.ExecuteNonQuery()
        End Using
    End Sub

    Private Function AmbilKolomTabel(namaTabel As String, transaction As MySqlTransaction) As HashSet(Of String)
        Dim hasil As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim sql As String = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @DB AND TABLE_NAME = @TABEL"

        Using cmd As New MySqlCommand(sql, conn, transaction)
            cmd.Parameters.AddWithValue("@DB", conn.Database)
            cmd.Parameters.AddWithValue("@TABEL", namaTabel)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                While rd.Read()
                    hasil.Add(rd("COLUMN_NAME").ToString())
                End While
            End Using
        End Using

        Return hasil
    End Function

    Private Function PilihNamaKolom(kolomTersedia As HashSet(Of String), kandidat As IEnumerable(Of String)) As String
        For Each nama In kandidat
            If kolomTersedia.Contains(nama) Then
                Return nama
            End If
        Next

        Return ""
    End Function

    Private Sub TambahKolomPenjualanPiutang(kolomTersedia As HashSet(Of String),
                                            daftarKolom As List(Of String),
                                            daftarParameter As List(Of String),
                                            nilaiKolom As Dictionary(Of String, Object),
                                            kandidat As IEnumerable(Of String),
                                            nilai As Object)

        Dim namaKolom As String = PilihNamaKolom(kolomTersedia, kandidat)
        If String.IsNullOrWhiteSpace(namaKolom) Then Exit Sub

        Dim namaParameter As String = "@P_" & namaKolom
        daftarKolom.Add(namaKolom)
        daftarParameter.Add(namaParameter)
        nilaiKolom(namaParameter) = nilai
    End Sub

    Private Sub SimpanJurnalUmumEdit(transaction As MySqlTransaction,
                                     noTransaksi As String,
                                     tglTransaksi As DateTime,
                                     uraian As String,
                                     namaAkunD As String,
                                     nomorAkunD As String,
                                     namaAkunK As String,
                                     nomorAkunK As String,
                                     nominal As Decimal,
                                     jenisTransaksi As String,
                                     namaBantuD As String,
                                     kodeBantuD As String,
                                     idUser As String,
                                     idKomputer As String)

        If nominal = 0D Then Exit Sub

        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_D, KODE_BANTU_D, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_D, @KODE_BANTU_D, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)
            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", noTransaksi)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", tglTransaksi.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", uraian)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", namaAkunD)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", nomorAkunD)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", namaAkunK)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", nomorAkunK)
            cmd.Parameters.AddWithValue("@NAMA_BANTU_D", namaBantuD)
            cmd.Parameters.AddWithValue("@KODE_BANTU_D", kodeBantuD)
            cmd.Parameters.AddWithValue("@NOMINAL", nominal)
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", jenisTransaksi)
            cmd.Parameters.AddWithValue("@LOKASI", _lokasiBarang)
            cmd.Parameters.AddWithValue("@ID_USER", idUser)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", idKomputer)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub BtnBatal_Click(sender As Object, e As EventArgs) Handles BtnBatal.Click
        Close()
    End Sub
End Class
