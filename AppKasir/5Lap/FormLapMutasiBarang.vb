Imports Microsoft.Reporting.WinForms

Public Class FormLapMutasiBarang

#Region "Konstanta & Variabel"
    Private Const BARCODE_MIN_LENGTH As Integer = 6
    Private Const BARCODE_TOTAL_TIME_MS As Double = 300

    Private barcodeTimer As New System.Windows.Forms.Timer()
    Private barcodeStartTime As DateTime
    Private lastKeyTime As DateTime
    Private barcodeChars As New List(Of Char)()
    Private _sedangPilihListBox As Boolean = False
    Private _kodeBarang As New Dictionary(Of String, String)() ' display text → kode
#End Region

#Region "Form Events"
    Private Sub FormLapMutasiBarang_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        TxtNama.Clear()
        TxtKode.Clear()
        DateTimePicker1.Value = DateTime.Now
        DateTimePicker2.Value = DateTime.Now
        CbTanggal.Checked = True
        CmbLokasi.SelectedIndex = 0
        ReportViewer1.LocalReport.DataSources.Clear()

        barcodeTimer.Interval = 120
        AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick

        LstBarang.BringToFront()
        LstBarang.DrawMode = DrawMode.OwnerDrawFixed
        AddHandler LstBarang.DrawItem, AddressOf LstBarang_DrawItem
        TxtNama.Focus()
    End Sub

    Private Sub FormLapMutasiBarang_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        TxtNama.Focus()
        TxtNama.Select()
    End Sub

    Private Sub FormLapMutasiBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F5 : BtnPreview.PerformClick()
            Case Keys.Escape : Me.Close()
        End Select
    End Sub
#End Region

#Region "TxtNama & LstBarang"
    Private Sub TxtNama_GotFocus(sender As Object, e As EventArgs) Handles TxtNama.GotFocus
        PanelCari.BackColor = ModuleTheme.C(Color.Yellow, Color.FromArgb(255, 204, 0))
    End Sub

    Private Sub TxtNama_LostFocus(sender As Object, e As EventArgs) Handles TxtNama.LostFocus
        PanelCari.BackColor = SystemColors.Control
        If Not _sedangPilihListBox Then
            LstBarang.Visible = False
        End If
    End Sub

    Private Sub TxtNama_TextChanged(sender As Object, e As EventArgs) Handles TxtNama.TextChanged
        Dim currentText As String = TxtNama.Text.Trim()

        If String.IsNullOrEmpty(currentText) OrElse currentText.Length < 2 Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            If Not _sedangPilihListBox Then TxtKode.Text = ""
            Return
        End If

        ' Input murni angka → kemungkinan barcode, jangan tampilkan listbox
        If Not currentText.Any(AddressOf Char.IsLetter) Then Return

        Using cmd As New MySqlCommand(
            "SELECT ID_BARANG, NAMA_BARANG, STOK_TOKO, STOK_GUDANG FROM tbl_barang " &
            "WHERE (NAMA_BARANG LIKE @s OR ID_BARANG LIKE @s) " &
            "ORDER BY NAMA_BARANG LIMIT 50", conn)
            cmd.Parameters.AddWithValue("@s", "%" & currentText & "%")
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                LstBarang.Items.Clear()
                _kodeBarang.Clear()
                While rd.Read()
                    Dim display As String = rd("NAMA_BARANG").ToString() & "  [T:" &
                                           ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_TOKO", 0D).ToString("N0") & " G:" &
                                           ModuleAngka.SafeGetValue(Of Decimal)(rd, "STOK_GUDANG", 0D).ToString("N0") & "]"
                    LstBarang.Items.Add(display)
                    _kodeBarang(display) = rd("ID_BARANG").ToString()
                End While
            End Using
        End Using

        LstBarang.Visible = (LstBarang.Items.Count > 0)
    End Sub

    Private Sub TxtNama_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtNama.KeyDown
        lastKeyTime = DateTime.Now
        If barcodeChars.Count = 0 Then barcodeStartTime = DateTime.Now
        barcodeChars.Add(ChrW(e.KeyValue))
        If Not barcodeTimer.Enabled Then barcodeTimer.Start()

        Select Case e.KeyCode
            Case Keys.Down
                If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
                    _sedangPilihListBox = True
                    LstBarang.Focus()
                    LstBarang.SelectedIndex = 0
                    e.SuppressKeyPress = True
                End If

            Case Keys.Enter
                barcodeTimer.Stop()
                Dim inputText As String = TxtNama.Text.Trim()

                If inputText.Length >= BARCODE_MIN_LENGTH AndAlso Not inputText.Any(AddressOf Char.IsLetter) Then
                    If CariDenganBarcode(inputText) Then
                        ResetBarcodeDetection()
                        e.SuppressKeyPress = True
                        Return
                    End If
                End If

                If LstBarang.Visible AndAlso LstBarang.Items.Count > 0 Then
                    AmbilDariListBox()
                End If
                ResetBarcodeDetection()
                e.SuppressKeyPress = True

            Case Keys.Escape
                LstBarang.Items.Clear()
                LstBarang.Visible = False
                ResetBarcodeDetection()
        End Select
    End Sub

    Private Sub LstBarang_MouseDown(sender As Object, e As MouseEventArgs) Handles LstBarang.MouseDown
        _sedangPilihListBox = True
    End Sub

    Private Sub LstBarang_MouseClick(sender As Object, e As MouseEventArgs) Handles LstBarang.MouseClick
        If LstBarang.SelectedItem IsNot Nothing Then
            AmbilDariListBox()
        End If
        _sedangPilihListBox = False
        TxtNama.Focus()
    End Sub

    Private Sub LstBarang_KeyDown(sender As Object, e As KeyEventArgs) Handles LstBarang.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                If LstBarang.SelectedItem IsNot Nothing Then
                    AmbilDariListBox()
                    _sedangPilihListBox = False
                    TxtNama.Focus()
                End If
                e.SuppressKeyPress = True

            Case Keys.Escape
                LstBarang.Visible = False
                _sedangPilihListBox = False
                TxtNama.Focus()
                e.SuppressKeyPress = True

            Case Keys.Up
                If LstBarang.SelectedIndex <= 0 Then
                    LstBarang.SelectedIndex = -1
                    LstBarang.Visible = False
                    _sedangPilihListBox = False
                    TxtNama.Focus()
                    e.SuppressKeyPress = True
                End If
        End Select
    End Sub

    Private Sub LstBarang_DrawItem(sender As Object, e As DrawItemEventArgs)
        If e.Index < 0 Then Return
        e.DrawBackground()

        Dim display As String = LstBarang.Items(e.Index).ToString()
        Dim bracketIdx As Integer = display.LastIndexOf("  [")
        Dim nama As String = If(bracketIdx > 0, display.Substring(0, bracketIdx), display)
        Dim stok As String = If(bracketIdx > 0, display.Substring(bracketIdx).Trim(), "")

        Dim fg As Color = If((e.State And DrawItemState.Selected) <> 0, SystemColors.HighlightText, e.ForeColor)
        Using br As New SolidBrush(fg)
            e.Graphics.DrawString(nama, e.Font, br, e.Bounds.Left + 2, e.Bounds.Top + 1)
            If stok.Length > 0 Then
                Dim stokSize As SizeF = e.Graphics.MeasureString(stok, e.Font)
                e.Graphics.DrawString(stok, e.Font, br,
                    e.Bounds.Right - stokSize.Width - 4, e.Bounds.Top + 1)
            End If
        End Using
        e.DrawFocusRectangle()
    End Sub

    Private Sub AmbilDariListBox()
        If LstBarang.Items.Count = 0 Then Return

        Dim display As String = If(LstBarang.SelectedItem IsNot Nothing,
                                   LstBarang.SelectedItem.ToString(),
                                   LstBarang.Items(0).ToString())

        Dim bracketIdx As Integer = display.LastIndexOf("  [")
        Dim nama As String = If(bracketIdx > 0, display.Substring(0, bracketIdx).Trim(), display.Trim())

        _sedangPilihListBox = True
        TxtKode.Text = If(_kodeBarang.ContainsKey(display), _kodeBarang(display), "")
        TxtNama.Text = nama
        _sedangPilihListBox = False

        LstBarang.Items.Clear()
        LstBarang.Visible = False
    End Sub
#End Region

#Region "Deteksi Barcode"
    Private Sub BarcodeTimer_Tick(sender As Object, e As EventArgs)
        If (DateTime.Now - lastKeyTime).TotalMilliseconds < 100 Then Return
        barcodeTimer.Stop()

        Dim inputText As String = TxtNama.Text.Trim()
        If String.IsNullOrWhiteSpace(inputText) Then
            ResetBarcodeDetection()
            Return
        End If

        Dim totalMs As Double = (DateTime.Now - barcodeStartTime).TotalMilliseconds

        If totalMs <= BARCODE_TOTAL_TIME_MS AndAlso
           inputText.Length >= BARCODE_MIN_LENGTH AndAlso
           Not inputText.Any(AddressOf Char.IsLetter) Then
            CariDenganBarcode(inputText)
        End If

        ResetBarcodeDetection()
    End Sub

    Private Sub ResetBarcodeDetection()
        barcodeTimer.Stop()
        barcodeChars.Clear()
    End Sub

    Private Function CariDenganBarcode(barcodeText As String) As Boolean
        Try
            Using cmd As New MySqlCommand(
                "SELECT ID_BARANG, NAMA_BARANG FROM tbl_barang " &
                "WHERE BARCODE_KECIL = @bc OR BARCODE_SEDANG = @bc OR BARCODE_BESAR = @bc LIMIT 1", conn)
                cmd.Parameters.AddWithValue("@bc", barcodeText)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        _sedangPilihListBox = True
                        TxtKode.Text = rd("ID_BARANG").ToString()
                        TxtNama.Text = rd("NAMA_BARANG").ToString()
                        _sedangPilihListBox = False
                        LstBarang.Items.Clear()
                        LstBarang.Visible = False
                        Return True
                    End If
                End Using
            End Using
        Catch
        End Try
        Return False
    End Function
#End Region

#Region "Filter Tanggal & Bulan"
    Private Sub CbTanggal_CheckedChanged(sender As Object, e As EventArgs) Handles CbTanggal.CheckedChanged
        If CbTanggal.Checked Then
            CbBulan.Checked = False
            DateTimePicker1.Enabled = True
            DateTimePicker2.Enabled = True
            LblSd.Enabled = True
            CmbBln.Enabled = False
            CmbThn.Enabled = False
            CmbBln.Items.Clear()
            CmbThn.Items.Clear()
        End If
    End Sub

    Private Sub CbBulan_CheckedChanged(sender As Object, e As EventArgs) Handles CbBulan.CheckedChanged
        If CbBulan.Checked Then
            CbTanggal.Checked = False
            DateTimePicker1.Enabled = False
            DateTimePicker2.Enabled = False
            LblSd.Enabled = False
            CmbBln.Enabled = True
            CmbThn.Enabled = True
            MuatComboBoxBulanTahun(CmbBln, CmbThn)
        End If
    End Sub

    Private Function GetRentangTanggal(ByRef tglAwal As DateTime, ByRef tglAkhir As DateTime) As Boolean
        If CbTanggal.Checked Then
            tglAwal = DateTimePicker1.Value.Date
            tglAkhir = DateTimePicker2.Value.Date.AddDays(1).AddTicks(-1)
            Return True
        ElseIf CbBulan.Checked Then
            Return GetRentangBulan(CmbBln, CmbThn, tglAwal, tglAkhir)
        Else
            MessageBox.Show("Harap pilih mode filter (Tanggal atau Bulan).", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
    End Function
#End Region

#Region "Proses Data & Laporan"
    Private Sub AmbilDanSimpanDataMutasiBarang()
        Dim tanggalAwal As Date
        Dim tanggalAkhir As Date
        If Not GetRentangTanggal(tanggalAwal, tanggalAkhir) Then Return

        Dim lokasiPilihan As String = CmbLokasi.Text   ' "TOKO", "GUDANG", atau "SEMUA"
        Dim isSemua As Boolean = (lokasiPilihan = "SEMUA")

        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        Try
            Using cmdClear As New MySqlCommand("DELETE FROM Temp_Mutasi_Barang", conn, transaction)
                cmdClear.ExecuteNonQuery()
            End Using

            ' Hitung saldo awal dari tbl_barang
            Dim saldoAwal As Decimal = 0
            Using cmdSaldoAwal As New MySqlCommand(
                "SELECT AWAL_TOKO, AWAL_GUDANG FROM tbl_barang WHERE ID_BARANG = ?", conn, transaction)
                cmdSaldoAwal.Parameters.AddWithValue("?", TxtKode.Text)
                Using reader As MySqlDataReader = cmdSaldoAwal.ExecuteReader()
                    While reader.Read()
                        If isSemua Then
                            saldoAwal += Convert.ToDecimal(reader("AWAL_TOKO")) +
                                         Convert.ToDecimal(reader("AWAL_GUDANG"))
                        ElseIf lokasiPilihan = "TOKO" Then
                            saldoAwal += Convert.ToDecimal(reader("AWAL_TOKO"))
                        ElseIf lokasiPilihan = "GUDANG" Then
                            saldoAwal += Convert.ToDecimal(reader("AWAL_GUDANG"))
                        End If
                    End While
                End Using
            End Using

            ' Akumulasi mutasi sebelum periode
            Dim sqlHist As String =
                "SELECT JENIS, SUM(TOTAL_QTY) AS TOTAL_QTY FROM historybarang " &
                "WHERE TANGGAL < @TanggalAwal AND ID_BARANG = @IdBarang" &
                If(isSemua, "", " AND LOKASI = @Lokasi") &
                " GROUP BY JENIS"
            Using cmdHist As New MySqlCommand(sqlHist, conn, transaction)
                cmdHist.Parameters.AddWithValue("@TanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdHist.Parameters.AddWithValue("@IdBarang", TxtKode.Text)
                If Not isSemua Then cmdHist.Parameters.AddWithValue("@Lokasi", lokasiPilihan)
                Using reader As MySqlDataReader = cmdHist.ExecuteReader()
                    While reader.Read()
                        Dim qty As Decimal = Convert.ToDecimal(reader("TOTAL_QTY"))
                        Select Case reader("JENIS").ToString()
                            Case "TAMBAH", "PEMBELIAN", "RETUR JUAL", "OPNAME", "TRANSFER STOK MASUK", "TRANSFER BARANG MASUK"
                                saldoAwal += qty
                            Case "KURANG", "PENJUALAN", "RETUR BELI", "TRANSFER STOK KELUAR", "TRANSFER BARANG KELUAR"
                                saldoAwal -= qty
                        End Select
                    End While
                End Using
            End Using

            ' Insert saldo awal
            Using cmdInsertSA As New MySqlCommand(
                "INSERT INTO Temp_Mutasi_Barang (FAKTUR, TANGGAL, JENIS, LOKASI, QTY_MASUK, QTY_KELUAR, SALDO, ID_USER) " &
                "VALUES ('SA-000000001', @Tanggal, 'SALDO AWAL', @Lokasi, 0, 0, @Saldo, @IdUser)", conn, transaction)
                cmdInsertSA.Parameters.AddWithValue("@Tanggal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdInsertSA.Parameters.AddWithValue("@Lokasi", lokasiPilihan)
                cmdInsertSA.Parameters.AddWithValue("@Saldo", saldoAwal)
                cmdInsertSA.Parameters.AddWithValue("@IdUser", FormUtama.StatusNamaUser.Text)
                cmdInsertSA.ExecuteNonQuery()
            End Using

            ' Ambil transaksi dalam periode
            Dim sqlTrans As String =
                "SELECT FAKTUR, TANGGAL, JENIS, LOKASI, TOTAL_QTY, ID_USER FROM historybarang " &
                "WHERE TANGGAL BETWEEN @TanggalAwal AND @TanggalAkhir AND ID_BARANG = @IdBarang" &
                If(isSemua, "", " AND LOKASI = @Lokasi") &
                " ORDER BY TANGGAL"
            Dim records As New List(Of Dictionary(Of String, Object))
            Using cmdTrans As New MySqlCommand(sqlTrans, conn, transaction)
                cmdTrans.Parameters.AddWithValue("@TanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdTrans.Parameters.AddWithValue("@TanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdTrans.Parameters.AddWithValue("@IdBarang", TxtKode.Text)
                If Not isSemua Then cmdTrans.Parameters.AddWithValue("@Lokasi", lokasiPilihan)
                Using reader As MySqlDataReader = cmdTrans.ExecuteReader()
                    While reader.Read()
                        records.Add(New Dictionary(Of String, Object) From {
                            {"FAKTUR", reader("FAKTUR").ToString()},
                            {"TANGGAL", Convert.ToDateTime(reader("TANGGAL"))},
                            {"JENIS", reader("JENIS").ToString()},
                            {"LOKASI", reader("LOKASI").ToString()},
                            {"TOTAL_QTY", Convert.ToDecimal(reader("TOTAL_QTY"))},
                            {"ID_USER", reader("ID_USER").ToString()}
                        })
                    End While
                End Using
            End Using

            ' Insert tiap transaksi ke temp table
            For Each record As Dictionary(Of String, Object) In records
                Dim jenis As String = record("JENIS").ToString()
                Dim totalQty As Decimal = CType(record("TOTAL_QTY"), Decimal)
                Dim qtyMasuk As Decimal = 0
                Dim qtyKeluar As Decimal = 0

                Select Case jenis
                    Case "TAMBAH", "PEMBELIAN", "RETUR JUAL", "OPNAME", "TRANSFER STOK MASUK", "TRANSFER BARANG MASUK"
                        qtyMasuk = totalQty
                        saldoAwal += totalQty
                    Case "KURANG", "PENJUALAN", "RETUR BELI", "TRANSFER STOK KELUAR", "TRANSFER BARANG KELUAR"
                        qtyKeluar = totalQty
                        saldoAwal -= totalQty
                End Select

                Using cmdInsert As New MySqlCommand(
                    "INSERT INTO Temp_Mutasi_Barang (FAKTUR, TANGGAL, JENIS, LOKASI, QTY_MASUK, QTY_KELUAR, SALDO, ID_USER) " &
                    "VALUES (@Faktur, @Tanggal, @Jenis, @Lokasi, @QtyMasuk, @QtyKeluar, @Saldo, @IdUser)", conn, transaction)
                    cmdInsert.Parameters.AddWithValue("@Faktur", record("FAKTUR").ToString())
                    cmdInsert.Parameters.AddWithValue("@Tanggal", CType(record("TANGGAL"), Date).ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdInsert.Parameters.AddWithValue("@Jenis", jenis)
                    cmdInsert.Parameters.AddWithValue("@Lokasi", record("LOKASI").ToString())
                    cmdInsert.Parameters.AddWithValue("@QtyMasuk", qtyMasuk)
                    cmdInsert.Parameters.AddWithValue("@QtyKeluar", qtyKeluar)
                    cmdInsert.Parameters.AddWithValue("@Saldo", saldoAwal)
                    cmdInsert.Parameters.AddWithValue("@IdUser", record("ID_USER").ToString())
                    cmdInsert.ExecuteNonQuery()
                End Using
            Next

            transaction.Commit()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AmbilData()
        Using cmdMutasi As New MySqlCommand(
            "SELECT FAKTUR, TANGGAL, JENIS, LOKASI, QTY_MASUK, QTY_KELUAR, SALDO, ID_USER FROM Temp_Mutasi_Barang", conn)
            Using rd As MySqlDataReader = cmdMutasi.ExecuteReader()
                ' Buat DataTable dengan tipe kolom eksplisit agar RDLC tidak error saat format DateTime
                Dim dt As New DataTable("Temp_Mutasi_Barang")
                dt.Columns.Add("FAKTUR", GetType(String))
                dt.Columns.Add("TANGGAL", GetType(DateTime))
                dt.Columns.Add("JENIS", GetType(String))
                dt.Columns.Add("LOKASI", GetType(String))
                dt.Columns.Add("QTY_MASUK", GetType(Decimal))
                dt.Columns.Add("QTY_KELUAR", GetType(Decimal))
                dt.Columns.Add("SALDO", GetType(Decimal))
                dt.Columns.Add("ID_USER", GetType(String))

                While rd.Read()
                    Dim row As DataRow = dt.NewRow()
                    row("FAKTUR") = rd("FAKTUR").ToString()
                    row("TANGGAL") = If(IsDBNull(rd("TANGGAL")), CType(DBNull.Value, Object), Convert.ToDateTime(rd("TANGGAL")))
                    row("JENIS") = rd("JENIS").ToString()
                    row("LOKASI") = rd("LOKASI").ToString()
                    row("QTY_MASUK") = Convert.ToDecimal(rd("QTY_MASUK"))
                    row("QTY_KELUAR") = Convert.ToDecimal(rd("QTY_KELUAR"))
                    row("SALDO") = Convert.ToDecimal(rd("SALDO"))
                    row("ID_USER") = rd("ID_USER").ToString()
                    dt.Rows.Add(row)
                End While

                ' Tentukan label periode sesuai filter aktif
                Dim periodeLabel As String
                If CbBulan.Checked Then
                    Dim bln As String = If(CmbBln.SelectedItem IsNot Nothing, CmbBln.SelectedItem.ToString(), "")
                    Dim thn As String = If(CmbThn.SelectedItem IsNot Nothing, CmbThn.SelectedItem.ToString(), "")
                    periodeLabel = "Periode : " & bln & " " & thn
                Else
                    periodeLabel = "Tanggal : " & DateTimePicker1.Value.ToShortDateString() & " s/d " & DateTimePicker2.Value.ToShortDateString()
                End If

                ReportViewer1.LocalReport.DataSources.Clear()
                ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dt))
                ReportViewer1.LocalReport.SetParameters(New ReportParameterCollection From {
                    New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN),
                    New ReportParameter("Kode", TxtKode.Text),
                    New ReportParameter("Nama_Barang", TxtNama.Text),
                    New ReportParameter("Tanggal", periodeLabel)
                })
                ReportViewer1.RefreshReport()
            End Using
        End Using
    End Sub

    Private Sub BtnPreview_Click(sender As Object, e As EventArgs) Handles BtnPreview.Click
        If String.IsNullOrEmpty(TxtKode.Text) Then
            MessageBox.Show("Harap pilih barang terlebih dahulu.", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtNama.Focus()
            Return
        End If
        ReportViewer1.LocalReport.DataSources.Clear()
        AmbilDanSimpanDataMutasiBarang()
        AmbilData()
    End Sub
#End Region

End Class
