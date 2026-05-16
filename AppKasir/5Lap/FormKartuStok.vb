Imports Microsoft.Reporting.WinForms

Public Class FormKartuStok

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
    Private Sub FormKartuStok_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
        DTPAwal.Value = tanggalAwalPeriodeKerja
        DTPAkhir.Value = tanggalAkhirPeriodeKerja
        CbTanggal.Checked = True
        ReportViewer1.LocalReport.DataSources.Clear()

        barcodeTimer.Interval = 120
        AddHandler barcodeTimer.Tick, AddressOf BarcodeTimer_Tick

        LstBarang.BringToFront()
        LstBarang.DrawMode = DrawMode.OwnerDrawFixed
        AddHandler LstBarang.DrawItem, AddressOf LstBarang_DrawItem
        CmbLokasi.SelectedIndex = 0
        TxtCari.Focus()
    End Sub

    Private Sub FormKartuStok_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        TxtCari.Focus()
        TxtCari.Select()
    End Sub

    Private Sub FormKartuStok_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F5 Then BtnTampil.PerformClick()
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub
#End Region

#Region "TxtCari & LstBarang"
    Private Sub TxtCari_GotFocus(sender As Object, e As EventArgs) Handles TxtCari.GotFocus
        PanelCari.BackColor = ModuleTheme.C(Color.Yellow, Color.FromArgb(255, 204, 0))
    End Sub

    Private Sub TxtCari_LostFocus(sender As Object, e As EventArgs) Handles TxtCari.LostFocus
        PanelCari.BackColor = SystemColors.ActiveCaption
        If Not _sedangPilihListBox Then
            LstBarang.Visible = False
        End If
    End Sub

    Private Sub TxtCari_TextChanged(sender As Object, e As EventArgs) Handles TxtCari.TextChanged
        Dim currentText As String = TxtCari.Text.Trim()

        If String.IsNullOrEmpty(currentText) OrElse currentText.Length < 2 Then
            LstBarang.Items.Clear()
            LstBarang.Visible = False
            If Not _sedangPilihListBox Then LblKodeBarang.Text = ""
            Return
        End If

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

    Private Sub TxtCari_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtCari.KeyDown
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
                Dim inputText As String = TxtCari.Text.Trim()

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

    Private Sub LstBarang_MouseDown(sender As Object, e As MouseEventArgs)
        _sedangPilihListBox = True
    End Sub

    Private Sub LstBarang_MouseClick(sender As Object, e As MouseEventArgs)

    End Sub

    Private Sub LstBarang_KeyDown(sender As Object, e As KeyEventArgs)

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
            ' Nama di kiri
            e.Graphics.DrawString(nama, e.Font, br, e.Bounds.Left + 2, e.Bounds.Top + 1)
            ' Stok di kanan
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
        LblKodeBarang.Text = If(_kodeBarang.ContainsKey(display), _kodeBarang(display), "")
        TxtCari.Text = nama
        _sedangPilihListBox = False

        LstBarang.Items.Clear()
        LstBarang.Visible = False
    End Sub
#End Region

#Region "Deteksi Barcode"
    Private Sub BarcodeTimer_Tick(sender As Object, e As EventArgs)
        If (DateTime.Now - lastKeyTime).TotalMilliseconds < 100 Then Return
        barcodeTimer.Stop()

        Dim inputText As String = TxtCari.Text.Trim()
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
                        LblKodeBarang.Text = rd("ID_BARANG").ToString()
                        TxtCari.Text = rd("NAMA_BARANG").ToString()
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

    End Sub

    Private Sub CbBulan_CheckedChanged(sender As Object, e As EventArgs) Handles CbBulan.CheckedChanged

    End Sub

    Private Function GetRentangTanggal(ByRef tglAwal As DateTime, ByRef tglAkhir As DateTime) As Boolean
        If CbTanggal.Checked Then
            tglAwal = DTPAwal.Value.Date
            tglAkhir = DTPAkhir.Value.Date.AddDays(1).AddTicks(-1)
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

#Region "Tampil Laporan"
    Private Sub BtnTampil_Click(sender As Object, e As EventArgs) Handles BtnTampil.Click
        If String.IsNullOrEmpty(LblKodeBarang.Text) Then
            MessageBox.Show("Harap pilih barang terlebih dahulu.", "Peringatan",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtCari.Focus()
            Return
        End If

        Dim tglAwal As DateTime
        Dim tglAkhir As DateTime
        If Not GetRentangTanggal(tglAwal, tglAkhir) Then Return

        Dim lokasi As String = CmbLokasi.Text
        Dim lokasiFilter As String = If(lokasi = "SEMUA", "", "AND LOKASI = @LOKASI")

        Dim query As String =
            "SELECT JENIS, LOKASI, " &
            "SUM(CASE WHEN TOTAL_QTY > 0 THEN TOTAL_QTY ELSE 0 END) AS QTY_MASUK, " &
            "SUM(CASE WHEN TOTAL_QTY < 0 THEN ABS(TOTAL_QTY) ELSE 0 END) AS QTY_KELUAR, " &
            "COUNT(*) AS JUMLAH_TRANSAKSI " &
            "FROM historybarang " &
            "WHERE ID_BARANG = @ID_BARANG " &
            "AND TANGGAL BETWEEN @TGL_AWAL AND @TGL_AKHIR " &
            lokasiFilter & " " &
            "GROUP BY JENIS, LOKASI " &
            "ORDER BY JENIS, LOKASI"

        Dim totalMasuk As Decimal = 0
        Dim totalKeluar As Decimal = 0

        Try
            Cursor = Cursors.WaitCursor
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@ID_BARANG", LblKodeBarang.Text)
                cmd.Parameters.AddWithValue("@TGL_AWAL", tglAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@TGL_AKHIR", tglAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                If lokasi <> "SEMUA" Then cmd.Parameters.AddWithValue("@LOKASI", lokasi)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Using ds As New DataSet()
                        ds.Load(rd, LoadOption.OverwriteChanges, "KartuStok")

                        For Each row As DataRow In ds.Tables("KartuStok").Rows
                            totalMasuk += Convert.ToDecimal(row("QTY_MASUK"))
                            totalKeluar += Convert.ToDecimal(row("QTY_KELUAR"))
                        Next

                        LblTotalMasuk.Text = totalMasuk.ToString("N2")
                        LblTotalKeluar.Text = totalKeluar.ToString("N2")
                        LblSaldoAkhir.Text = (totalMasuk - totalKeluar).ToString("N2")

                        Dim judulTgl As String = tglAwal.ToString("dd/MM/yyyy") & " s/d " & tglAkhir.Date.ToString("dd/MM/yyyy")

                        ReportViewer1.LocalReport.DataSources.Clear()
                        ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", ds.Tables("KartuStok")))
                        ReportViewer1.LocalReport.SetParameters(New ReportParameter() {
                            New ReportParameter("Perusahaan", NAMA_PERUSAHAAN),
                            New ReportParameter("NamaBarang", TxtCari.Text),
                            New ReportParameter("KodeBarang", LblKodeBarang.Text),
                            New ReportParameter("Periode", judulTgl),
                            New ReportParameter("Lokasi", If(lokasi = "SEMUA", "Toko & Gudang", lokasi)),
                            New ReportParameter("Kasir", "Dicetak oleh : " & FormUtama.StatusNamaUser.Text)
                        })
                        ReportViewer1.RefreshReport()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Kesalahan",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub


#End Region

End Class
