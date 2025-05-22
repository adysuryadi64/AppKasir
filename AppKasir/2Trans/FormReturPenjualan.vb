Imports System.IO


Public Class FormReturPenjualan
    Private jenisprintercetak As String = ""
    Private Sub FormReturPenjualan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        OpenConnection()
        AmbilJenisPrinter()
        Kondisiawalretur()
        Datagrid()
    End Sub


    Private Sub Kondisiawalretur()
        Using newFontheader As New Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            Dim headerCellStyle As New DataGridViewCellStyle With {
        .Font = newFontheader,
        .Alignment = DataGridViewContentAlignment.MiddleCenter
    }

            DGVPenjualan.ColumnHeadersDefaultCellStyle = headerCellStyle
            DGVReturjual.ColumnHeadersDefaultCellStyle = headerCellStyle
            DGVPilihBarang.ColumnHeadersDefaultCellStyle = headerCellStyle
        End Using


        DTPtglJual.Value = DateTime.Now
        DTPtglJual.Format = DateTimePickerFormat.Custom
        DTPtglJual.CustomFormat = "dd/MM/yyyy"

        DTPRetur.Value = DateTime.Now
        DTPRetur.Format = DateTimePickerFormat.Custom
        DTPRetur.CustomFormat = "dd/MM/yyyy HH:mm:ss"
        TxtNotaJual.Text = ""
        LblLokasi.Text = FormUtama.SLokasi.Text
        LblStatusJual.Text = ""
        LblTotalJual.Text = "Rp. 0"
        LblBayarJual.Text = "Rp. 0"
        LblSisaJual.Text = "Rp. 0"
        TxtTotalJual.Text = ""
        TxtBayarJual.Text = ""
        TxtSisaJual.Text = ""
        DGVReturjual.DataSource = Nothing
        DGVReturjual.Rows.Clear()
        DGVPenjualan.DataSource = Nothing
        DGVPenjualan.Rows.Clear()
        DGVPilihBarang.DataSource = Nothing
        DGVPilihBarang.Rows.Clear()
        RTBAlasanRetur.Text = ""
        PanelPencarian.Visible = False
        Panelcaribarang.Visible = False
        LblTotalBarang.Text = "Rp. 0"
        LblTotalQTY.Text = "Rp. 0"
        LblTotalRupiah.Text = "Rp. 0"
        TxtTotalBarang.Text = ""
        TxtTotalQTY.Text = ""
        TxtTotalRupiah.Text = ""
        TxtTotalLaba.Text = ""
        TxtHPP.Text = ""
        LblKodePel.Text = ""
        LblNAmaPel.Text = ""
        LblAlamatPel.Text = ""
        LblKontakPel.Text = ""
        LblJenisPel.Text = ""
        CbPotongHutang.Visible = True
        'AmbilRekeningKasBank()
        CbTunai.Checked = True
        GenerateNomorReturPenjualan()
    End Sub

    Public Sub AmbilJenisPrinter()
        Dim filePath As String = "printer.ini"

        Using reader As New StreamReader(filePath)
            Do While Not reader.EndOfStream
                Dim parts As String() = reader.ReadLine().Split("="c)
                If parts.Length = 2 AndAlso parts(0) = "JenisPrinterJual" Then
                    jenisprintercetak = parts(1)
                    Exit Do
                End If
            Loop
        End Using
    End Sub


    Private Sub GenerateNomorReturPenjualan()
        Dim cekTanggal As String = Microsoft.VisualBasic.Format(DTPRetur.Value, "yyMMdd")
        Dim UrutKOde As String = ""
        Dim cekNomor As String = "RP-" & cekTanggal

        ' Query untuk mendapatkan nomor maksimum berdasarkan format
        Using cmd As New MySqlCommand("SELECT MAX(ID_RETUR_PENJUALAN) FROM retur_penjualan WHERE ID_RETUR_PENJUALAN LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")

            ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
            Dim maxKode As Object = cmd.ExecuteScalar()

            If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                Dim MaxNilaiKode As String = maxKode.ToString()
                If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "RP-" & cekTanggal Then
                    ' Hitung nomor berikutnya
                    Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                    UrutKOde = "RP-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                End If
            End If
        End Using

        ' Jika UrutKOde masih kosong, buat nomor pertama
        If String.IsNullOrEmpty(UrutKOde) Then
            UrutKOde = "RP-" & cekTanggal & "0001"
        End If

        LblNoNotaRetur.Text = UrutKOde

    End Sub


    Private Sub CenterPanelPencarian()
        Dim x As Integer = (ClientSize.Width - PanelPencarian.Width) \ 2
        Dim y As Integer = (Me.ClientSize.Height - PanelPencarian.Height) \ 2
        PanelPencarian.Location = New Point(x, y)
    End Sub


    Private Sub PBcariNotaBeli_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PBcariNotaJual.Click, TxtNotaJual.Click
        DateTimePicker1.Value = DateTime.Now
        DateTimePicker1.Format = DateTimePickerFormat.Custom
        DateTimePicker1.CustomFormat = "dd/MM/yyyy"

        'PanelDataPel.Visible = False
        'DGVReturjual.Visible = False
        'PanelSimpan.Visible = False
        CenterPanelPencarian()
        PanelPencarian.Visible = True

    End Sub
    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        'PanelDataPel.Visible = True
        'DGVReturjual.Visible = True
        'PanelSimpan.Visible = True
        PanelPencarian.Visible = False
    End Sub

    Private Sub DateTimePicker1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateTimePicker1.ValueChanged
        Dim tanggalAwal As Date = DateTimePicker1.Value.Date
        Dim tanggalAkhir As Date = DateTimePicker1.Value.Date.AddDays(1).AddTicks(-1)
        Dim query As String = "SELECT ID_PENJUALAN, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, LOKASIBARANG, TGL_TRANSAKSI, GRAND_TOTAL_STL_PAJAK, (BAYAR-KEMBALI) AS PEMBAYARAN, SISA_TAGIHAN, STATUS_TRANSAKSI FROM penjualan WHERE TGL_TRANSAKSI BETWEEN @tanggalAwal AND @tanggalAkhir AND LOKASIBARANG = @LOKASIBARANG  ORDER BY ID_PENJUALAN"

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASIBARANG", LblLokasi.Text)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.HasRows Then
                    DGVPenjualan.SuspendLayout() ' Suspend layout untuk meningkatkan kinerja

                    DGVPenjualan.Rows.Clear()

                    Do While rd.Read()
                        DGVPenjualan.Rows.Add(rd("ID_PENJUALAN"), rd("ID_PELANGGAN"), rd("NAMA_PELANGGAN"), rd("JENIS_PELANGGAN"), rd("LOKASIBARANG"), rd("TGL_TRANSAKSI"), rd("GRAND_TOTAL_STL_PAJAK"), rd("PEMBAYARAN"), rd("SISA_TAGIHAN"), rd("STATUS_TRANSAKSI"))
                    Loop

                    DGVPenjualan.ResumeLayout() ' Lanjutkan layout setelah menambahkan baris
                Else
                    DGVPenjualan.Rows.Clear()
                End If
            End Using
        End Using

        ' ...

        With DGVPenjualan
            ' Pengaturan tampilan DataGridView
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False

            ' Loop untuk mengatur preferensi kolom
            For Each col As DataGridViewColumn In .Columns
                Select Case col.Name
                    Case "TGL_TRANSAKSI"
                        col.DefaultCellStyle.Format = "dd/MM/yyyy"
                    Case "GRAND_TOTAL_STL_PAJAK", "BAYAR", "SISA_TAGIHAN"
                        col.DefaultCellStyle.Format = "N0"
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End Select
            Next

            ' Mengubah nama header kolom
            .Columns("ID_PENJUALAN").HeaderText = "NO NOTA"
            .Columns("ID_PELANGGAN").Visible = False
            .Columns("NAMA_PELANGGAN").HeaderText = "PELANGGAN"
            .Columns("JENIS_PELANGGAN").HeaderText = "JENIS"
            .Columns("TGL_TRANSAKSI").HeaderText = "TANGGAL JUAL"
            .Columns("LOKASIBARANG").HeaderText = "LOKASI"
            .Columns("BAYAR").HeaderText = "PEMBAYARAN"
            .Columns("SISA_TAGIHAN").HeaderText = "HUTANG"
            .Columns("STATUS_TRANSAKSI").HeaderText = "STATUS"
        End With

    End Sub

    Private Sub LblKodePel_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblKodePel.TextChanged
        If LblKodePel.Text <> "" Then
            Using cmd As New MySqlCommand("SELECT ALAMAT, NO_TELP FROM tbl_pelanggan where KODE like @KODE", conn)
                cmd.Parameters.AddWithValue("@KODE", LblKodePel.Text) ' Move this line up

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        rd.Read()
                        LblAlamatPel.Text = rd("ALAMAT").ToString()
                        LblKontakPel.Text = rd("NO_TELP").ToString()
                    End If
                End Using
            End Using
        Else
            LblAlamatPel.Text = ""
            LblKontakPel.Text = ""
        End If
    End Sub

    Private Sub TxtNotaJual_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtNotaJual.TextChanged
        If TxtNotaJual.Text <> "" Then
            DGVReturjual.DataSource = Nothing
            DGVReturjual.Rows.Clear()
            HitungSemua()

            LblPilihbarang.Text = "Pilih barang yang akan diretur dari penjualan :" & TxtNotaJual.Text

            ' Query gabungan untuk menghindari reader bertumpuk
            Dim query As String = "
            SELECT 
                pd.ID_BARANG, 
                pd.NAMA_BARANG, 
                pd.HARGA_BELI, 
                pd.HARGA_JUAL, 
                pd.TOTAL_DISKON, 
                SUM(pd.QTY_SATUAN) AS QTYJUAL, 
                IFNULL(SUM(rpd.QTY_SATUAN), 0) AS QTYRETUR
            FROM penjualan_detail pd
            LEFT JOIN retur_penjualan rp ON TRIM(rp.ID_PENJUALAN) = TRIM(pd.FAKTUR_JUAL)
            LEFT JOIN retur_penjualan_detail rpd ON rp.ID_RETUR_PENJUALAN = rpd.ID_RETUR_PENJUALAN AND pd.ID_BARANG = rpd.ID_BARANG
            WHERE TRIM(pd.FAKTUR_JUAL) LIKE @FAKTUR_JUAL
            GROUP BY pd.ID_BARANG, pd.NAMA_BARANG, pd.HARGA_BELI, pd.HARGA_JUAL, pd.TOTAL_DISKON"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtNotaJual.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    DGVPilihBarang.Rows.Clear()

                    While rd.Read()
                        Dim kodebarang As String = rd("ID_BARANG")
                        Dim qtyretur As Decimal = rd("QTYRETUR")
                        Dim qtyjual As Decimal = rd("QTYJUAL")

                        ' Menghitung sisa qty yang bisa diretur
                        Dim sisaQty As Decimal = qtyjual - qtyretur

                        DGVPilihBarang.Rows.Add(rd("ID_BARANG"), rd("NAMA_BARANG"), rd("HARGA_BELI"), rd("HARGA_JUAL"), rd("TOTAL_DISKON"), sisaQty)
                    End While
                End Using
            End Using

            ' Pengaturan format tampilan
            DGVPilihBarang.Columns(2).DefaultCellStyle.Format = "N0"
            DGVPilihBarang.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DGVPilihBarang.Columns(2).Visible = False
            DGVPilihBarang.Columns(3).DefaultCellStyle.Format = "N0"
            DGVPilihBarang.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DGVPilihBarang.Columns(4).DefaultCellStyle.Format = "N0"
            DGVPilihBarang.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            ' Pengaturan tampilan DataGridView
            With DGVPilihBarang
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AllowUserToOrderColumns = False
                .AllowUserToResizeColumns = False
                .AllowUserToResizeRows = False
            End With
        End If
    End Sub


    Private Sub CenterPanelcaribarang()
        Dim x As Integer = (ClientSize.Width - Panelcaribarang.Width) \ 2
        Dim y As Integer = (Me.ClientSize.Height - Panelcaribarang.Height) \ 2
        Panelcaribarang.Location = New Point(x, y)
    End Sub

    Private Sub BtnDaftarBarang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDaftarBarang.Click
        If TxtNotaJual.Text <> "" Then
            Panelcaribarang.Visible = True
            CenterPanelcaribarang()
            Panelcaribarang.Visible = True
        Else
            MessageBox.Show("Silahkan isi nota jual terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    Private Sub DGVPenjualan_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVPenjualan.CellClick
        If DGVPenjualan.Rows.Count >= 1 Then
            TxtNotaJual.Text = DGVPenjualan.Item(0, DGVPenjualan.CurrentRow.Index).Value
            LblKodePel.Text = DGVPenjualan.Item(1, DGVPenjualan.CurrentRow.Index).Value
            LblNAmaPel.Text = DGVPenjualan.Item(2, DGVPenjualan.CurrentRow.Index).Value.ToString()
            LblJenisPel.Text = DGVPenjualan.Item(3, DGVPenjualan.CurrentRow.Index).Value.ToString()
            DTPtglJual.Text = DGVPenjualan.Item(5, DGVPenjualan.CurrentRow.Index).Value

            ' Mengganti tipe Double dengan Decimal
            Dim total As Decimal
            If Not Decimal.TryParse(DGVPenjualan.Item(6, DGVPenjualan.CurrentRow.Index).Value.ToString(), total) Then
                total = 0D
            End If
            TxtTotalJual.Text = total
            LblTotalJual.Text = "Rp. " & total.ToString("#,0.##", cultureIndonesia)

            Dim tagihan As Decimal
            If Not Decimal.TryParse(DGVPenjualan.Item(7, DGVPenjualan.CurrentRow.Index).Value.ToString(), tagihan) Then
                tagihan = 0D
            End If
            TxtBayarJual.Text = tagihan
            LblBayarJual.Text = "Rp. " & tagihan.ToString("#,0.##", cultureIndonesia)

            Dim sisaBayar As Decimal
            If Not Decimal.TryParse(DGVPenjualan.Item(8, DGVPenjualan.CurrentRow.Index).Value.ToString(), sisaBayar) Then
                sisaBayar = 0D
            End If
            TxtSisaJual.Text = sisaBayar
            LblSisaJual.Text = "Rp. " & sisaBayar.ToString("#,0.##", cultureIndonesia)

            LblStatusJual.Text = DGVPenjualan.Item(9, DGVPenjualan.CurrentRow.Index).Value
            PanelDataPel.Visible = True
            DGVReturjual.Visible = True
            PanelSimpan.Visible = True
            PanelPencarian.Visible = False
        Else
            ' Tambahkan logika jika diperlukan
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Panelcaribarang.Visible = False
    End Sub

    Private Sub DGVPilihBarang_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVPilihBarang.CellClick
        If DGVPilihBarang.Rows.Count >= 1 Then
            If DGVPilihBarang.Rows(e.RowIndex).Cells(5).Value <> 0 Then
                DGVReturjual.Rows.Add(1)
                Dim indeksBaris As Integer = DGVReturjual.RowCount - 2
                DGVReturjual.Rows(indeksBaris).Cells(0).Value = DGVPilihBarang.Rows(e.RowIndex).Cells(0).Value.ToString()
                DGVReturjual.Rows(indeksBaris).Cells(1).Value = DGVPilihBarang.Rows(e.RowIndex).Cells(1).Value.ToString()
                DGVReturjual.Rows(indeksBaris).Cells(2).Value = If(DGVPilihBarang.Rows(e.RowIndex).Cells(2).Value IsNot Nothing, Decimal.Parse(DGVPilihBarang.Rows(e.RowIndex).Cells(2).Value.ToString()).ToString("0.##"), "")
                DGVReturjual.Rows(indeksBaris).Cells(8).Value = If(DGVPilihBarang.Rows(e.RowIndex).Cells(3).Value IsNot Nothing, Decimal.Parse(DGVPilihBarang.Rows(e.RowIndex).Cells(3).Value.ToString()).ToString("0.##"), "")
                DGVReturjual.Rows(indeksBaris).Cells(9).Value = If(DGVPilihBarang.Rows(e.RowIndex).Cells(4).Value IsNot Nothing, Decimal.Parse(DGVPilihBarang.Rows(e.RowIndex).Cells(4).Value.ToString()).ToString("0.##"), "")

                Dim idBarang As String = DGVReturjual.Rows(indeksBaris).Cells(0).Value.ToString()

                If Not String.IsNullOrEmpty(idBarang) Then
                    Dim querySatuan As String = "SELECT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @Idbarang"

                    Using sqlSatuan As New MySqlCommand(querySatuan, conn)
                        sqlSatuan.Parameters.AddWithValue("@Idbarang", idBarang)

                        Using dataReader As MySqlDataReader = sqlSatuan.ExecuteReader()
                            If dataReader.Read() Then
                                Dim comboCell As DataGridViewComboBoxCell = CType(DGVReturjual.Rows(indeksBaris).Cells("SATUAN"), DataGridViewComboBoxCell)
                                comboCell.Items.Clear()

                                Dim satuanKecil As String = If(Not dataReader.IsDBNull(dataReader.GetOrdinal("SATUAN_UMUM_KECIL")), dataReader.GetString(dataReader.GetOrdinal("SATUAN_UMUM_KECIL")), "")
                                Dim satuanSedang As String = If(Not dataReader.IsDBNull(dataReader.GetOrdinal("SATUAN_UMUM_SEDANG")), dataReader.GetString(dataReader.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
                                Dim satuanBesar As String = If(Not dataReader.IsDBNull(dataReader.GetOrdinal("SATUAN_UMUM_BESAR")), dataReader.GetString(dataReader.GetOrdinal("SATUAN_UMUM_BESAR")), "")


                                If Not String.IsNullOrEmpty(satuanKecil) Then
                                    comboCell.Items.Add(satuanKecil)
                                End If

                                If Not String.IsNullOrEmpty(satuanSedang) Then
                                    comboCell.Items.Add(satuanSedang)
                                End If

                                If Not String.IsNullOrEmpty(satuanBesar) Then
                                    comboCell.Items.Add(satuanBesar)
                                End If

                                Dim satuan As String = dataReader("SATUAN_UMUM_KECIL")
                                Dim isi As Integer = dataReader("ISI_UMUM_KECIL")

                                DGVReturjual.Rows(indeksBaris).Cells("QTY").Value = 1
                                DGVReturjual.Rows(indeksBaris).Cells("SATUAN").Value = satuan
                                DGVReturjual.Rows(indeksBaris).Cells("ISI_SATUAN").Value = isi
                                DGVReturjual.Rows(indeksBaris).Cells("QTY_SATUAN").Value = CDec(DGVReturjual.Rows(indeksBaris).Cells("QTY").Value) * isi

                                DGVReturjual.Rows(indeksBaris).Cells("HARGA_BELI_SATUAN").Value = CDec(DGVReturjual.Rows(indeksBaris).Cells("HARGA_BELI").Value) * CDec(DGVReturjual.Rows(indeksBaris).Cells("QTY_SATUAN").Value)
                                DGVReturjual.Rows(indeksBaris).Cells("TOTAL_HARGA").Value = CDec(DGVReturjual.Rows(indeksBaris).Cells("HARGA_JUAL").Value) * CDec(DGVReturjual.Rows(indeksBaris).Cells("QTY_SATUAN").Value) + CDec(DGVReturjual.Rows(indeksBaris).Cells("TOTAL_DISKON").Value)
                            End If
                        End Using
                    End Using
                End If

                'Panelcaribarang.Visible = False
                HitungSemua()
            Else
                MessageBox.Show("Jumlah barang sudah habis, mungkin sudah diretur sebelumnya", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Public Sub AddItems(ByVal col As AutoCompleteStringCollection)
        If TxtNotaJual.Text <> "" Then
            Dim query As String = "SELECT DISTINCT NAMA_BARANG FROM penjualan_detail WHERE FAKTUR_JUAL LIKE @FAKTUR_JUAL"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@FAKTUR_JUAL", TxtNotaJual.Text)

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Do While rd.Read()
                        col.Add(rd("NAMA_BARANG").ToString())
                    Loop
                End Using
            End Using
        Else
            ' Tambahkan pesan di sini
            MessageBox.Show("Silahkan isi nota jual terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub DGVReturjual_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles DGVReturjual.EditingControlShowing

        Dim titleText As String = DGVReturjual.Columns(1).HeaderText
        If titleText.Equals("NAMA BARANG") Then
            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.Suggest
                autoText.AutoCompleteSource = AutoCompleteSource.CustomSource
                Dim DataCollection As New AutoCompleteStringCollection()
                AddItems(DataCollection) ' Pass only one argument, the collection
                autoText.AutoCompleteCustomSource = DataCollection
            End If
        End If

        ' Periksa apakah kolom yang saat ini sedang diedit adalah kolom yang berisi ComboBox (misalnya, kolom dengan indeks 4)
        If DGVReturjual.CurrentCell.ColumnIndex = 4 Then
            Dim comboBox As ComboBox = TryCast(e.Control, ComboBox)

            ' Hapus penanganan event SelectedIndexChanged jika ada
            RemoveHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged

            ' Tambahkan penanganan event SelectedIndexChanged ke ComboBox
            AddHandler comboBox.SelectedIndexChanged, AddressOf ComboBox_SelectedIndexChanged
        End If

    End Sub

    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox As ComboBox = DirectCast(sender, ComboBox)

        ' Dapatkan sel saat ini yang sedang diedit
        Dim cell As DataGridViewComboBoxCell = DirectCast(DGVReturjual.CurrentCell, DataGridViewComboBoxCell)
        Dim selectedItemId As String = cell.OwningRow.Cells("ID_BARANG").Value.ToString()

        Using cmd As New MySqlCommand("SELECT ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE ID_BARANG = @ItemId", conn)
            cmd.Parameters.AddWithValue("@ItemId", selectedItemId)

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    ' Update nilai pada kolom "Isi" berdasarkan indeks yang dipilih dalam ComboBox
                    Select Case comboBox.SelectedIndex
                        Case 0
                            cell.OwningRow.Cells("ISI_SATUAN").Value = rd("ISI_UMUM_KECIL").ToString()
                        Case 1
                            cell.OwningRow.Cells("ISI_SATUAN").Value = rd("ISI_UMUM_SEDANG").ToString()
                        Case Else
                            cell.OwningRow.Cells("ISI_SATUAN").Value = rd("ISI_UMUM_BESAR").ToString()
                    End Select

                    ' Lakukan perhitungan sel lain yang berkaitan dengan perubahan ini
                    Dim rowIndex As Integer = DGVReturjual.CurrentCell.RowIndex
                    DGVReturjual("QTY_SATUAN", rowIndex).Value = CDec(DGVReturjual("QTY", rowIndex).Value) * CDec(DGVReturjual("ISI_SATUAN", rowIndex).Value)
                    DGVReturjual("HARGA_BELI_SATUAN", rowIndex).Value = CDec(DGVReturjual("HARGA_BELI", rowIndex).Value) * CDec(DGVReturjual("QTY_SATUAN", rowIndex).Value)
                    DGVReturjual("TOTAL_HARGA", rowIndex).Value = CDec(DGVReturjual("HARGA_JUAL", rowIndex).Value) * CDec(DGVReturjual("QTY_SATUAN", rowIndex).Value) + CDec(DGVReturjual("TOTAL_DISKON", rowIndex).Value)

                    HitungSemua()
                Else
                    MessageBox.Show("Satuan barang dan atau harga jual belum di input !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            End Using
        End Using
    End Sub

    Private Sub DGVReturjual_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVReturjual.CellEndEdit
        '========================== Nama
        If e.ColumnIndex = 1 Then

            If DGVReturjual.Rows(e.RowIndex) IsNot Nothing AndAlso DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG") IsNot Nothing Then
                Dim namaCellValue As Object = DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG").Value
                If namaCellValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(namaCellValue.ToString().Trim()) Then
                    Dim namaValue As String = namaCellValue.ToString().Trim()

                    Dim query As String = "SELECT TOP 1 ID_BARANG, NAMA_BARANG, HARGA_BELI FROM penjualan_detail WHERE TRIM(NAMA_BARANG) LIKE @NamaValue"

                    Using cmd As New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@NamaValue", namaValue.Trim())

                        Using rd As MySqlDataReader = cmd.ExecuteReader()
                            If rd.Read() Then
                                DGVReturjual.Rows(e.RowIndex).Cells("ID_BARANG").Value = rd("ID_BARANG")
                                DGVReturjual.Rows(e.RowIndex).Cells("HARGA_BELI").Value = rd("HARGA_BELI")

                                DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG").Value = rd("NAMA_BARANG")
                            Else
                                DGVReturjual.Rows(e.RowIndex).Cells("ID_BARANG").Value = ""
                                DGVReturjual.Rows(e.RowIndex).Cells("HARGA_BELI").Value = ""

                                DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG").Value = ""
                                Hapusbaris()
                            End If
                            If DGVReturjual.Rows(e.RowIndex).Cells("ID_BARANG").Value <> "" Then
                                Dim querySatuan As String = "SELECT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @Idbarang"

                                Using sqlSatuan As New MySqlCommand(querySatuan, conn)
                                    sqlSatuan.Parameters.AddWithValue("@Idbarang", DGVReturjual.Rows(e.RowIndex).Cells("ID_BARANG").Value)

                                    Using dataReader As MySqlDataReader = sqlSatuan.ExecuteReader()
                                        If dataReader.Read() Then

                                            Dim comboCell As DataGridViewComboBoxCell = CType(DGVReturjual.Rows(e.RowIndex).Cells("SATUAN"), DataGridViewComboBoxCell)
                                            comboCell.Items.Clear()

                                            Dim satuanKecil As String = If(Not dataReader.IsDBNull(dataReader.GetOrdinal("SATUAN_UMUM_KECIL")), dataReader.GetString(dataReader.GetOrdinal("SATUAN_UMUM_KECIL")), "")
                                            Dim satuanSedang As String = If(Not dataReader.IsDBNull(dataReader.GetOrdinal("SATUAN_UMUM_SEDANG")), dataReader.GetString(dataReader.GetOrdinal("SATUAN_UMUM_SEDANG")), "")
                                            Dim satuanBesar As String = If(Not dataReader.IsDBNull(dataReader.GetOrdinal("SATUAN_UMUM_BESAR")), dataReader.GetString(dataReader.GetOrdinal("SATUAN_UMUM_BESAR")), "")

                                            If Not String.IsNullOrEmpty(satuanKecil) Then
                                                comboCell.Items.Add(satuanKecil)
                                            End If

                                            If Not String.IsNullOrEmpty(satuanSedang) Then
                                                comboCell.Items.Add(satuanSedang)
                                            End If

                                            If Not String.IsNullOrEmpty(satuanBesar) Then
                                                comboCell.Items.Add(satuanBesar)
                                            End If

                                            Dim satuan As String = dataReader("SATUAN_UMUM_KECIL")
                                            Dim isi As Integer = dataReader("ISI_UMUM_KECIL")

                                            DGVReturjual.Rows(e.RowIndex).Cells("QTY").Value = 1
                                            DGVReturjual.Rows(e.RowIndex).Cells("SATUAN").Value = satuan
                                            DGVReturjual.Rows(e.RowIndex).Cells("ISI_SATUAN").Value = isi

                                            DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value = CDec(DGVReturjual.Rows(e.RowIndex).Cells("QTY").Value) * isi
                                            DGVReturjual.Rows(e.RowIndex).Cells("HARGA_BELI_SATUAN").Value = CDec(DGVReturjual.Rows(e.RowIndex).Cells("HARGA_BELI").Value) * DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value
                                            DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_HARGA").Value = CDec(DGVReturjual.Rows(e.RowIndex).Cells("HARGA_JUAL").Value) * CDec(DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value) + CDec(DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value)
                                        End If
                                    End Using
                                End Using

                                For barisatas As Integer = 0 To DGVReturjual.RowCount - 1
                                    For barisbawah As Integer = barisatas + 1 To DGVReturjual.RowCount - 1
                                        Dim kodeBarisAtas As Object = DGVReturjual.Rows(barisatas).Cells("ID_BARANG").Value
                                        Dim kodeBarisBawah As Object = DGVReturjual.Rows(barisbawah).Cells("ID_BARANG").Value

                                        If kodeBarisAtas IsNot Nothing AndAlso kodeBarisBawah IsNot Nothing AndAlso kodeBarisBawah.Equals(kodeBarisAtas) Then
                                            DGVReturjual.Rows(barisatas).Cells("QTY").Value = Convert.ToDecimal(DGVReturjual.Rows(barisatas).Cells("QTY").Value) + 1

                                            Dim isiValue As Decimal = Convert.ToDecimal(DGVReturjual.Rows(barisatas).Cells("ISI_SATUAN").Value)
                                            Dim qtyValue As Decimal = Convert.ToDecimal(DGVReturjual.Rows(barisatas).Cells("QTY").Value)

                                            If isiValue = 0 Then
                                                DGVReturjual.Rows(barisatas).Cells("QTY_SATUAN").Value = Convert.ToDecimal(DGVReturjual.Rows(barisatas).Cells("QTY_SATUAN").Value) + 1
                                            Else
                                                DGVReturjual.Rows(barisatas).Cells("QTY_SATUAN").Value = isiValue * qtyValue
                                            End If

                                            Dim hargaValue As Decimal
                                            Dim totalDiskonValue As Decimal
                                            Dim qtysatuan As Decimal

                                            ' Pastikan Cell Value Tidak Null
                                            If DGVReturjual.Rows(barisatas).Cells("HARGA_JUAL").Value IsNot Nothing AndAlso
                                               DGVReturjual.Rows(barisatas).Cells("TOTAL_DISKON").Value IsNot Nothing AndAlso
                                               DGVReturjual.Rows(barisatas).Cells("QTY_SATUAN").Value IsNot Nothing Then

                                                ' Konversi nilai jika valid
                                                If Decimal.TryParse(DGVReturjual.Rows(barisatas).Cells("HARGA_JUAL").Value.ToString(), hargaValue) AndAlso
                                                   Decimal.TryParse(DGVReturjual.Rows(barisatas).Cells("TOTAL_DISKON").Value.ToString(), totalDiskonValue) AndAlso
                                                   Decimal.TryParse(DGVReturjual.Rows(barisatas).Cells("QTY_SATUAN").Value.ToString(), qtysatuan) Then

                                                    ' Hitung dan atur nilai TOTAL_HARGA
                                                    DGVReturjual.Rows(barisatas).Cells("TOTAL_HARGA").Value = (hargaValue * qtysatuan) + totalDiskonValue
                                                End If
                                            End If

                                            If Not DGVReturjual.Rows(barisbawah).IsNewRow Then
                                                DGVReturjual.Rows.RemoveAt(barisbawah)
                                            End If

                                            SendKeys.Send("{down}")
                                            DGVReturjual.Rows(barisbawah).Cells("NAMA_BARANG").Value = ""
                                            HitungSemua()
                                            Exit Sub
                                        End If
                                    Next
                                Next

                                DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value = CDec(DGVReturjual.Rows(e.RowIndex).Cells("QTY").Value) * DGVReturjual.Rows(e.RowIndex).Cells("ISI_SATUAN").Value
                                DGVReturjual.Rows(e.RowIndex).Cells("HARGA_BELI_SATUAN").Value = CDec(DGVReturjual.Rows(e.RowIndex).Cells("HARGA_BELI").Value) * DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value
                                DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_HARGA").Value = CDec(DGVReturjual.Rows(e.RowIndex).Cells("HARGA_JUAL").Value) * CDec(DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value) + CDec(DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value)

                            End If
                            If DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG").Value = "" Then
                                SendKeys.Send("{down}")
                                CenterPanelcaribarang()
                                Panelcaribarang.Visible = True
                            End If
                        End Using
                    End Using
                Else
                    SendKeys.Send("{down}")
                    CenterPanelcaribarang()
                    Panelcaribarang.Visible = True
                End If
            End If
        End If

        '========================== qty
        If e.ColumnIndex = 3 Then
            Try
                DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value = DGVReturjual.Rows(e.RowIndex).Cells("QTY").Value * DGVReturjual.Rows(e.RowIndex).Cells("ISI_SATUAN").Value
                DGVReturjual.Rows(e.RowIndex).Cells("HARGA_BELI_SATUAN").Value = DGVReturjual.Rows(e.RowIndex).Cells("HARGA_BELI").Value * DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value
                DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_HARGA").Value = DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value * DGVReturjual.Rows(e.RowIndex).Cells("HARGA_JUAL").Value + DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value
            Catch ex As Exception
                MsgBox("harus angka", vbCritical, "Gagal ...!!!")
                SendKeys.Send("{up}")
                DGVReturjual.Rows(e.RowIndex).Cells("QTY").Value = 1
                DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_HARGA").Value = DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value * DGVReturjual.Rows(e.RowIndex).Cells("HARGA_JUAL").Value + DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value
            End Try

        End If
        '========================== harga jual
        If e.ColumnIndex = 8 Then
            Try
                DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_HARGA").Value = DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value * DGVReturjual.Rows(e.RowIndex).Cells("HARGA_JUAL").Value + DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value
            Catch ex As Exception
                MsgBox("harus angka", vbCritical, "Gagal ...!!!")
                SendKeys.Send("{up}")
            End Try
            DGVReturjual.Columns("HARGA_JUAL").DefaultCellStyle.Format = "###,###"
            DGVReturjual.Columns("HARGA_JUAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End If
        '========================== totaldiskon
        If e.ColumnIndex = 9 Then
            Try
                DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_HARGA").Value = DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value * DGVReturjual.Rows(e.RowIndex).Cells("HARGA_JUAL").Value + DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value
            Catch ex As Exception
                MsgBox("harus angka", vbCritical, "Gagal ...!!!")
                SendKeys.Send("{up}")
                DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value = 0
                DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_HARGA").Value = DGVReturjual.Rows(e.RowIndex).Cells("QTY_SATUAN").Value * DGVReturjual.Rows(e.RowIndex).Cells("HARGA_JUAL").Value + DGVReturjual.Rows(e.RowIndex).Cells("TOTAL_DISKON").Value
            End Try
        End If

        Datagrid()
        HitungSemua()
    End Sub

    Private Sub DGVReturPembelian_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs)
        e.Cancel = True
    End Sub
    Private Sub Datagrid()
        With DGVReturjual
            .Columns("HARGA_BELI").DefaultCellStyle.Format = "###,###"
            .Columns("HARGA_BELI").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("HARGA_BELI_SATUAN").DefaultCellStyle.Format = "###,###"
            .Columns("HARGA_BELI_SATUAN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("HARGA_JUAL").DefaultCellStyle.Format = "###,###"
            .Columns("HARGA_JUAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("TOTAL_DISKON").DefaultCellStyle.Format = "###,###"
            .Columns("TOTAL_DISKON").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("TOTAL_HARGA").DefaultCellStyle.Format = "###,###"
            .Columns("TOTAL_HARGA").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
        End With
    End Sub


    Private Sub Hapusbaris()
        Dim baris As Integer = DGVReturjual.CurrentCell.RowIndex

        ' Cek apakah baris yang dipilih adalah baris baru yang belum dikonfirmasi.
        If baris < DGVReturjual.Rows.Count - 1 AndAlso Not DGVReturjual.Rows(baris).IsNewRow Then
            ' Jika bukan baris baru, hapus baris tersebut.
            DGVReturjual.Rows.RemoveAt(baris)
        Else
            ' Batalkan pengeditan dan hapus baris baru.
            ' Pastikan untuk mengonfirmasi terlebih dahulu apakah baris bukan baris baru sebelum mencoba membatalkan edit.
            If DGVReturjual.IsCurrentCellInEditMode Then
                DGVReturjual.EndEdit()
            End If

            ' Hapus baris baru (setelah konfirmasi edit).
            DGVReturjual.Rows.RemoveAt(baris)
        End If

        ' Panggil fungsi-fungsi lainnya
        HitungSemua()
    End Sub

    Private Sub HitungSemua()
        Dim totalBarang As Integer = DGVReturjual.RowCount - 1
        Dim totalQty As Decimal = 0
        Dim totalHPP As Decimal = 0
        Dim grandTotal As Decimal = 0
        Dim totalLaba As Decimal = 0

        For i As Integer = 0 To DGVReturjual.Rows.Count - 1
            If DGVReturjual.Rows(i).Cells("QTY_SATUAN").Value IsNot Nothing Then
                totalQty += Convert.ToDecimal(DGVReturjual.Rows(i).Cells("QTY_SATUAN").Value)
            End If

            If DGVReturjual.Rows(i).Cells("HARGA_BELI_SATUAN").Value IsNot Nothing Then
                totalHPP += Convert.ToDecimal(DGVReturjual.Rows(i).Cells("HARGA_BELI_SATUAN").Value)
            End If

            If DGVReturjual.Rows(i).Cells("TOTAL_HARGA").Value IsNot Nothing Then
                grandTotal += Convert.ToDecimal(DGVReturjual.Rows(i).Cells("TOTAL_HARGA").Value)
                totalLaba += Convert.ToDecimal(DGVReturjual.Rows(i).Cells("TOTAL_HARGA").Value) - Convert.ToDecimal(DGVReturjual.Rows(i).Cells("HARGA_BELI_SATUAN").Value)
            End If
        Next

        ' Update hasil perhitungan ke textbox dan label
        TxtTotalBarang.Text = totalBarang.ToString()
        LblTotalBarang.Text = totalBarang.ToString("N0")

        TxtTotalQTY.Text = totalQty.ToString()
        LblTotalQTY.Text = totalQty.ToString("N0")

        TxtHPP.Text = totalHPP.ToString()

        TxtTotalRupiah.Text = grandTotal.ToString()
        LblTotalRupiah.Text = "Rp. " & grandTotal.ToString("N0")

        TxtTotalLaba.Text = totalLaba.ToString()
    End Sub



    Private Sub DGVReturPembelian_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs)
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            DGVReturjual.CurrentCell = DGVReturjual.Rows(e.RowIndex).Cells("NAMA_BARANG")
            Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
            CMSHapus.Show(cursorPosition)
        End If
    End Sub

    Private Sub TSMhapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TSMhapus.Click
        Call Hapusbaris()
    End Sub

    Private Sub AmbilRekeningKasBank()
        CmbRekening.Items.Clear()
        ' Isi ComboBox dengan data dari list
        CmbRekening.Items.AddRange(GetDaftarAkun().ToArray())

        ' Set akun berdasarkan lokasi
        If LblLokasi.Text = "TOKO" Then
            CmbRekening.SelectedItem = nama_rek_Jual_Toko
            LblKodeAkun.Text = Kode_rek_Jual_Toko
        ElseIf LblLokasi.Text = "GUDANG" Then
            CmbRekening.SelectedItem = nama_rek_Jual_Gudang
            LblKodeAkun.Text = Kode_rek_Jual_Gudang
        End If
    End Sub

    Private Sub CbTunai_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTunai.CheckedChanged
        ' Cek apakah perubahan ini berasal dari interaksi pengguna
        If CbTunai.Checked Then
            CbPotongHutang.Checked = False
            AmbilRekeningKasBank()
        End If
    End Sub

    Private Sub CbPotongHutang_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPotongHutang.CheckedChanged
        ' Cek apakah perubahan ini berasal dari interaksi pengguna
        If CbPotongHutang.Checked Then
            CbTunai.Checked = False
            CmbRekening.Items.Clear()

            ' Query untuk mengambil akun dengan kode tertentu
            Dim queryAkun As String = "SELECT Nama_Akun FROM tbl_datareferensi WHERE Kode_akun LIKE '01.04.002'"
            Using cmd As New MySqlCommand(queryAkun, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.HasRows Then
                        While rd.Read()
                            CmbRekening.Items.Add(rd("Nama_Akun").ToString())
                        End While
                    End If
                End Using
            End Using

            CmbRekening.SelectedItem = nama_rek_Piutang_Jual
            'CmbRekening.SelectedIndex = 0
            LblKodeAkun.Text = Kode_rek_Piutang_Jual

            ' Periksa status pembayaran
            Dim sisaJual As Decimal
            Dim totalRupiah As Decimal

            ' Gunakan TryParse untuk menghindari error konversi
            If Decimal.TryParse(TxtSisaJual.Text, sisaJual) AndAlso Decimal.TryParse(TxtTotalRupiah.Text, totalRupiah) Then
                If sisaJual = totalRupiah Then
                    LblStatusPiutang.Text = "Lunas"
                Else
                    LblStatusPiutang.Text = "Belum Lunas"
                End If
            Else
                ' Jika salah satu nilai tidak dapat dikonversi, set status piutang sebagai "Data tidak valid"
                LblStatusPiutang.Text = "Lunas"
            End If

        End If
    End Sub


    Private Sub CmbRekening_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbRekening.SelectedIndexChanged
        If CbTunai.Checked = True Then
            Dim namaAkunD As String = CmbRekening.Text
            Dim sql As String = "SELECT Kode_akun FROM tbl_datareferensi WHERE Nama_Akun = @selectedNAMA"
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@selectedNAMA", namaAkunD)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        LblKodeAkun.Text = reader("Kode_akun").ToString()
                    End If
                End Using
            End Using
        End If
    End Sub

    Private Sub LblStatusJual_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblStatusJual.TextChanged
        If LblLokasi.Text = "Lunas" Then
            CbPotongHutang.Visible = False
            CbTunai.Checked = True
        Else
            CbPotongHutang.Visible = True
        End If
    End Sub

    Private Sub TxtTotalRupiah_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtTotalRupiah.TextChanged
        Dim totalRupiah As Double = 0
        Dim sisaJual As Double = 0

        Double.TryParse(TxtTotalRupiah.Text, totalRupiah)
        Double.TryParse(TxtSisaJual.Text, sisaJual)

        If totalRupiah > sisaJual Then
            CbPotongHutang.Visible = False
            CbTunai.Checked = True
        Else
            CbPotongHutang.Visible = True
        End If
    End Sub

    Private Sub BtnSimpan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpan.Click
        If Not Validasi() Then
            Return ' Batalkan aksi jika validasi gagal
        End If

        If CekQtyRetur() Then
            ' Mulai transaksi
            Dim transaction As MySqlTransaction = Nothing

            Try

                GenerateNomorReturPenjualan()

                transaction = conn.BeginTransaction()
                SimpanUpdatePiutangPembelian(transaction)
                Simpanreturpenjualan(transaction)
                Simpanreturpenjualandetail(transaction)
                HistoryBarang(transaction)
                Simpanjurnal(transaction)

                ' Commit transaksi jika berhasil
                transaction.Commit()

                'If jenisprintercetak = "Printer Thermal" Then
                With PrintReturJual
                    .TxtFaktur.Text = LblNoNotaRetur.Text
                End With
                'End If

                For Each row As DataGridViewRow In DGVReturjual.Rows
                    If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                        HitungByKode(row.Cells(0).Value)
                    End If
                Next

                DatabaseModule.CatatanAksiHistory("Retur penjualan " & LblNoNotaRetur.Text)

                ' Jika semuanya berhasil, kembalikan kondisi awal
                Kondisiawalretur()

            Catch ex As Exception
                transaction.Rollback()

                ' Tampilkan pesan kesalahan kepada pengguna
                MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try


        End If

    End Sub

    Private Function Validasi() As Boolean
        If DGVReturjual.RowCount <= 1 Then
            MessageBox.Show("Belum ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If TxtNotaJual.Text = "" Then
            MessageBox.Show("Belum ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If TxtTotalRupiah.Text = "" Then
            MessageBox.Show("Belum ada transaksi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If CmbRekening.SelectedIndex = -1 Then
            MessageBox.Show("Silahkan pilih metode pengembalian pembayaran", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If CbPotongHutang.Visible Then
            Dim totalRupiah As Decimal = Convert.ToDecimal(TxtTotalRupiah.Text)
            Dim sisaJual As Decimal = Convert.ToDecimal(TxtSisaJual.Text)

            ' Pastikan bahwa nilai totalRupiah tidak melebihi sisaJual.
            If totalRupiah > sisaJual Then
                MessageBox.Show("Jumlah nilai retur melebihi piutang.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        End If


        Return True
    End Function


    Private Function CekQtyRetur() As Boolean
        ' Kembalikan semua baris ke warna default sebelum memeriksa pelanggaran
        For Each row As DataGridViewRow In DGVReturjual.Rows
            row.DefaultCellStyle.BackColor = DGVReturjual.DefaultCellStyle.BackColor
        Next

        Dim adaPelanggaran As Boolean = False

        For Each row As DataGridViewRow In DGVReturjual.Rows
            ' Pastikan baris memiliki data
            If Not row.IsNewRow Then
                Dim idBarang As String = row.Cells(0).Value.ToString()
                Dim qtySatuanRetur As Decimal = Convert.ToDecimal(row.Cells(6).Value)

                For Each dgvpRow As DataGridViewRow In DGVPilihBarang.Rows
                    Dim idBarangPilih As String = dgvpRow.Cells(0).Value.ToString()

                    If idBarang = idBarangPilih Then
                        Dim qtySatuanPilih As Decimal = Convert.ToDecimal(dgvpRow.Cells(5).Value)

                        If qtySatuanRetur > qtySatuanPilih Then
                            row.DefaultCellStyle.BackColor = Color.Red
                            MessageBox.Show("Jumlah barang yang diretur melebihi barang yang dibeli", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            adaPelanggaran = True
                        End If
                    End If
                Next
            End If
        Next

        Return Not adaPelanggaran ' Return False jika ada pelanggaran, True jika tidak ada
    End Function


    Private Sub SimpanUpdatePiutangPembelian(ByVal transaction As MySqlTransaction)
        Dim updateQuery As String

        If CbPotongHutang.Checked Then
            updateQuery = "UPDATE PENJUALAN SET TGL_RETUR = @TGL_RETUR, NILAI_RETUR = NILAI_RETUR + @NILAI_RETUR, " &
                          "SISA_TAGIHAN = SISA_TAGIHAN - @SISA_TAGIHAN, STATUS_TRANSAKSI = @STATUS_TRANSAKSI " &
                          "WHERE ID_PENJUALAN = @ID_PENJUALAN"
        Else
            updateQuery = "UPDATE PENJUALAN SET TGL_RETUR = @TGL_RETUR, NILAI_RETUR = NILAI_RETUR + @NILAI_RETUR " &
                          "WHERE ID_PENJUALAN = @ID_PENJUALAN"
        End If

        Using cmdUpdate As New MySqlCommand(updateQuery, conn, transaction)
            ' Tambahkan parameter
            cmdUpdate.Parameters.AddWithValue("@TGL_RETUR", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdUpdate.Parameters.AddWithValue("@NILAI_RETUR", Convert.ToDecimal(TxtTotalRupiah.Text))

            ' Jika CbPotongHutang diceklis, tambahkan parameter tambahan
            If CbPotongHutang.Checked Then
                cmdUpdate.Parameters.AddWithValue("@SISA_TAGIHAN", Convert.ToDecimal(TxtTotalRupiah.Text))
                cmdUpdate.Parameters.AddWithValue("@STATUS_TRANSAKSI", LblStatusPiutang.Text)
            End If

            ' Parameter terakhir untuk ID_PENJUALAN
            cmdUpdate.Parameters.AddWithValue("@ID_PENJUALAN", TxtNotaJual.Text)

            ' Eksekusi query
            cmdUpdate.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub Simpanreturpenjualan(ByVal transaction As MySqlTransaction)
        Dim query As String = "INSERT INTO retur_penjualan (ID_RETUR_PENJUALAN, TGL_RETUR_JUAL, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, ALAMAT_PELANGGAN, KONTAK_PELANGGAN, ID_PENJUALAN, TGL_PENJUALAN, STATUS_PENJUALAN, PENYIMPANAN, BAYAR_PENJUALAN, HUTANG_PENJUALAN, TOTAL_BARANG, TOTAL_QTY, TOTAL_RUPIAH, NAMA_REKENING, KODE_REKENING, ALASAN_RETUR, ID_USER, ID_KOMPUTER) " &
                       "VALUES (@ID_RETUR_PENJUALAN, @TGL_RETUR_JUAL, @ID_PELANGGAN, @NAMA_PELANGGAN, @JENIS_PELANGGAN, @ALAMAT_PELANGGAN, @KONTAK_PELANGGAN, @ID_PENJUALAN, @TGL_PENJUALAN, @STATUS_PENJUALAN, @PENYIMPANAN, @BAYAR_PENJUALAN, @HUTANG_PENJUALAN, @TOTAL_BARANG, @TOTAL_QTY, @TOTAL_RUPIAH, @NAMA_REKENING, @KODE_REKENING, @ALASAN_RETUR, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(query, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_RETUR_PENJUALAN", LblNoNotaRetur.Text)
            cmd.Parameters.AddWithValue("@TGL_RETUR_JUAL", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@ID_PELANGGAN", LblKodePel.Text)
            cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", LblNAmaPel.Text)
            cmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPel.Text)
            cmd.Parameters.AddWithValue("@ALAMAT_PELANGGAN", LblAlamatPel.Text)
            cmd.Parameters.AddWithValue("@KONTAK_PELANGGAN", LblKontakPel.Text)
            cmd.Parameters.AddWithValue("@ID_PENJUALAN", TxtNotaJual.Text)
            cmd.Parameters.AddWithValue("@TGL_PENJUALAN", DTPtglJual.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@STATUS_PENJUALAN", LblStatusJual.Text)
            cmd.Parameters.AddWithValue("@PENYIMPANAN", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@BAYAR_PENJUALAN", Convert.ToDecimal(TxtBayarJual.Text))
            cmd.Parameters.AddWithValue("@HUTANG_PENJUALAN", Convert.ToDecimal(TxtSisaJual.Text))
            cmd.Parameters.AddWithValue("@TOTAL_BARANG", Convert.ToDecimal(TxtTotalBarang.Text))
            cmd.Parameters.AddWithValue("@TOTAL_QTY", Convert.ToDecimal(TxtTotalQTY.Text))
            cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", Convert.ToDecimal(TxtTotalRupiah.Text))
            cmd.Parameters.AddWithValue("@NAMA_REKENING", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@KODE_REKENING", LblKodeAkun.Text)
            cmd.Parameters.AddWithValue("@ALASAN_RETUR", RTBAlasanRetur.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DGVReturjual.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim querySimpan As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                                            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(querySimpan, conn, transaction)
                    cmd.Parameters.AddWithValue("@FAKTUR", LblNoNotaRetur.Text)
                    cmd.Parameters.AddWithValue("@TANGGAL", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@JENIS", "RETUR JUAL")
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
                    cmd.Parameters.AddWithValue("@QTY", Convert.ToDecimal(row.Cells(3).Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", Convert.ToDecimal(row.Cells(5).Value))
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", Convert.ToDecimal(row.Cells(6).Value))
                    cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", Convert.ToDecimal(row.Cells(10).Value))
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        Next
    End Sub

    Private Sub Simpanreturpenjualandetail(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DGVReturjual.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim sqlrinci As String = "INSERT INTO retur_penjualan_detail (ID_RETUR_PENJUALAN, TGL_RETUR_JUAL, LOKASI, ID_PELANGGAN, NAMA_PELANGGAN, JENIS_PELANGGAN, ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, QTY_SATUAN, HARGA_BELI_SATUAN, HARGA_JUAL, TOTAL_DISKON, TOTAL_HARGA, LABA, ID_USER, ID_KOMPUTER) VALUES " &
                                         "(@ID_RETUR_PENJUALAN, @TGL_RETUR_JUAL, @LOKASI, @ID_PELANGGAN, @NAMA_PELANGGAN, @JENIS_PELANGGAN, @ID_BARANG, @NAMA_BARANG, @HARGA_BELI, @QTY, @SATUAN, @ISI_SATUAN, @QTY_SATUAN, @HARGA_BELI_SATUAN, @HARGA_JUAL, @TOTAL_DISKON, @TOTAL_HARGA, @LABA, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(sqlrinci, conn, transaction)
                    cmd.Parameters.AddWithValue("@ID_RETUR_PENJUALAN", LblNoNotaRetur.Text)
                    cmd.Parameters.AddWithValue("@TGL_RETUR_JUAL", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
                    cmd.Parameters.AddWithValue("@ID_PELANGGAN", LblKodePel.Text)
                    cmd.Parameters.AddWithValue("@NAMA_PELANGGAN", LblNAmaPel.Text)
                    cmd.Parameters.AddWithValue("@JENIS_PELANGGAN", LblJenisPel.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
                    cmd.Parameters.AddWithValue("@HARGA_BELI", Convert.ToDecimal(row.Cells(2).Value))
                    cmd.Parameters.AddWithValue("@QTY", Convert.ToDecimal(row.Cells(3).Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", Convert.ToDecimal(row.Cells(5).Value))
                    cmd.Parameters.AddWithValue("@QTY_SATUAN", Convert.ToDecimal(row.Cells(6).Value))
                    cmd.Parameters.AddWithValue("@HARGA_BELI_SATUAN", Convert.ToDecimal(row.Cells(7).Value))
                    cmd.Parameters.AddWithValue("@HARGA_JUAL", Convert.ToDecimal(row.Cells(8).Value))
                    cmd.Parameters.AddWithValue("@TOTAL_DISKON", Convert.ToDecimal(row.Cells(9).Value))
                    cmd.Parameters.AddWithValue("@TOTAL_HARGA", Convert.ToDecimal(row.Cells(10).Value))
                    cmd.Parameters.AddWithValue("@LABA", Convert.ToDecimal(row.Cells(10).Value) - Convert.ToDecimal(row.Cells(7).Value))
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
                    cmd.ExecuteNonQuery()
                End Using

                Dim updateStokField As String = String.Empty ' Inisialisasi dengan nilai default

                Select Case LblLokasi.Text
                    Case "TOKO"
                        updateStokField = "RETUR_JUAL_TOKO"
                    Case "GUDANG"
                        updateStokField = "RETUR_JUAL_GUDANG"
                    Case Else
                        Throw New InvalidOperationException("Lokasi barang tidak valid.")
                End Select

                Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " + @StokPengurangan WHERE ID_BARANG = @KodeBarang"

                Dim kodeBarang As String = row.Cells(0).Value.ToString()

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    Dim stokPengurangan As Decimal = If(row.Cells(6).Value IsNot Nothing, Convert.ToDecimal(row.Cells(6).Value), 0D)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            End If
        Next
    End Sub

    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction)

        ' Simpan ke jurnal KAS/BANK dan PIUTANG
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_K, KODE_BANTU_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                      "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_K, @KODE_BANTU_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNoNotaRetur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", "Retur penjualan dari " & LblNAmaPel.Text & " Jmlh Item " & TxtTotalBarang.Text & " Qty " & TxtTotalQTY.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", LblKodeAkun.Text)

            If CbTunai.Checked Then
                cmd.Parameters.AddWithValue("@NAMA_BANTU_K", DBNull.Value)
                cmd.Parameters.AddWithValue("@KODE_BANTU_K", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@NAMA_BANTU_K", LblNAmaPel.Text)
                cmd.Parameters.AddWithValue("@KODE_BANTU_K", LblKodePel.Text)
            End If

            cmd.Parameters.AddWithValue("@NOMINAL", Convert.ToDecimal(TxtTotalRupiah.Text))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Retur Penjualan")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
            cmd.ExecuteNonQuery()
        End Using

        ' Simpan ke jurnal persediaan barang
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                      "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNoNotaRetur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", "Retur penjualan dari " & LblNAmaPel.Text & " Jmlh Item " & TxtTotalBarang.Text & " Qty " & TxtTotalQTY.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", NAMA_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", KODE_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMINAL", Convert.ToDecimal(TxtHPP.Text))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Retur Penjualan")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
            cmd.ExecuteNonQuery()
        End Using

        ' Simpan jurnal laba
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                      "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNoNotaRetur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", "Retur penjualan dari " & LblNAmaPel.Text & " Jmlh Item " & TxtTotalBarang.Text & " Qty " & TxtTotalQTY.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", "LABA KOTOR PENJUALAN")
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", "06.01.001")
            cmd.Parameters.AddWithValue("@NOMINAL", Convert.ToDecimal(TxtTotalLaba.Text))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Retur Penjualan")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
            cmd.ExecuteNonQuery()
        End Using

    End Sub


    Private Sub FormReturPenjualan_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F8
                BtnSimpan.PerformClick()
            Case Keys.F12
                BtnReset.PerformClick()
            Case Keys.F2
                BtnDaftarBarang.PerformClick()
            Case Keys.F1
                DateTimePicker1.Value = DateTime.Now
                DateTimePicker1.Format = DateTimePickerFormat.Custom
                DateTimePicker1.CustomFormat = "dd/MM/yyyy"

                'PanelDataPel.Visible = False
                'DGVReturjual.Visible = False
                'PanelSimpan.Visible = False
                CenterPanelPencarian()
                PanelPencarian.Visible = True

            Case Keys.Escape
                If Panelcaribarang.Visible = True Then
                    Button1.PerformClick()
                ElseIf PanelPencarian.Visible = True Then
                    BtnClose.PerformClick()
                Else
                    Button2.PerformClick()
                End If

        End Select
    End Sub

    Private Sub BtnKeluar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnReset.Click
        Kondisiawalretur()
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        FormUtama.GBTransaksi.Visible = True
        FormUtama.Refresdatagridview()
        Close()
    End Sub



End Class