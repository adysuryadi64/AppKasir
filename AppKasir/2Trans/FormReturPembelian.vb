Imports System.Globalization

Public Class FormReturPembelian
    Private Sub FormReturPembelian_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LblLokasi.Text = FormUtama.SLokasi.Text
        Ambildatasupplier()
        Datagrid()

        Kondisiawalretur()
    End Sub

    Private Sub Ambildatasupplier()
        Using cmd As New MySqlCommand("SELECT Nama FROM tbl_supliyer ORDER BY Nama ASC", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                CmbSupplier.Items.Clear()
                CmbSupplier.Items.Add("")
                While rd.Read()
                    CmbSupplier.Items.Add(rd("Nama").ToString())
                End While
            End Using
        End Using
    End Sub


    Private Sub Kondisiawalretur()
        DTPtglBeli.Value = DateTime.Now
        DTPtglBeli.Format = DateTimePickerFormat.Custom
        DTPtglBeli.CustomFormat = "dd/MM/yyyy"
        LblLokasi.Text = FormUtama.SLokasi.Text
        DTPRetur.Value = DateTime.Now
        DTPRetur.Format = DateTimePickerFormat.Custom
        DTPRetur.CustomFormat = "dd/MM/yyyy HH:mm:ss"
        DTPtglBeli.Enabled = True
        TxtNotaBeli.Text = ""
        TxtNotaBeli.Enabled = True
        PBcariNotaBeli.Enabled = True
        LblStatusBeli.Text = ""
        LblSisaBayar.Text = "Rp. 0"
        TxtSisaBayar.Text = 0
        PanelDatagridview.Visible = False
        CmbSupplier.SelectedIndex = 0
        TxtBayarBeli.Text = 0
        LblBayarBeli.Text = ""
        TxtTotalBarang.Clear()
        TxtTotalQTY.Clear()
        TxtTotalRupiah.Clear()
        LblTotalBarang.Text = "Rp. 0"
        LblTotalQTY.Text = "Rp. 0"
        LblTotalRupiah.Text = "Rp. 0"
        LblStatusHutang.Text = "Status"
        DGVReturPembelian.Rows.Clear()
        If dtBarang IsNot Nothing Then
            dtBarang.Clear()
        End If

        GenerateNomorReturPembelian()

        CmbRekening.Items.Clear()
        ' Isi ComboBox dengan data dari list
        CmbRekening.Items.AddRange(GetDaftarAkun().ToArray())


        CbTunai.Checked = True
        If LblLokasi.Text = "TOKO" Then
            CmbRekening.SelectedItem = nama_rek_Jual_Toko
            LblKodeAkun.Text = Kode_rek_Jual_Toko
        ElseIf LblLokasi.Text = "GUDANG" Then
            CmbRekening.SelectedItem = nama_rek_Jual_Gudang
            LblKodeAkun.Text = Kode_rek_Jual_Gudang
        End If

        TxtNotaBeli.Focus()
    End Sub

    Private Sub GenerateNomorReturPembelian()

        Dim cekTanggal As String = Microsoft.VisualBasic.Format(DTPRetur.Value, "yyMMdd")
        Dim UrutKOde As String = ""
        Dim cekNomor As String = "RB-" & cekTanggal

        ' Query untuk mendapatkan nomor maksimum berdasarkan format
        Using cmd As New MySqlCommand("SELECT MAX(ID_RETUR_PEMBELIAN) FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN LIKE @ceknomor", conn)
            cmd.Parameters.AddWithValue("@ceknomor", cekNomor & "%")

            ' Gunakan ExecuteScalar untuk mendapatkan nilai maksimum
            Dim maxKode As Object = cmd.ExecuteScalar()

            If Not IsDBNull(maxKode) AndAlso maxKode IsNot Nothing Then
                Dim MaxNilaiKode As String = maxKode.ToString()
                If Microsoft.VisualBasic.Left(MaxNilaiKode, 9) = "RB-" & cekTanggal Then
                    ' Hitung nomor berikutnya
                    Dim Hitung As Integer = CInt(Microsoft.VisualBasic.Right(MaxNilaiKode, 4)) + 1
                    UrutKOde = "RB-" & cekTanggal & Microsoft.VisualBasic.Right("0000" & Hitung.ToString(), 4)
                End If
            End If
        End Using

        ' Jika UrutKOde masih kosong, buat nomor pertama
        If String.IsNullOrEmpty(UrutKOde) Then
            UrutKOde = "RB-" & cekTanggal & "0001"
        End If

        ' Tampilkan nomor pada label
        LblNoNotaRetur.Text = UrutKOde

    End Sub

    Private Sub CenterPanel6()
        Dim x As Integer = (ClientSize.Width - PanelDatagridview.Width) \ 2
        'Dim y As Integer = (Me.ClientSize.Height - Panel6.Height) \ 2
        Dim y As Integer = 44
        PanelDatagridview.Location = New Point(x, y)
        Panelcaribarang.Location = New Point(x, y)
    End Sub

    Private Sub CbJenisRetur_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbJenisRetur.CheckedChanged
        If CbJenisRetur.Checked = True Then
            PanelNota.Visible = False
            CbPotongHutang.Visible = False
        Else
            DTPtglBeli.Value = DateTime.Now
            DTPtglBeli.Format = DateTimePickerFormat.Custom
            DTPtglBeli.CustomFormat = "dd/MM/yyyy"
            PanelNota.Visible = True
            CbPotongHutang.Visible = True
        End If
        Kondisiawalretur()
        Daftarbarangbynota()
    End Sub

    Private Sub CmbSupplier_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbSupplier.SelectedIndexChanged
        If CmbSupplier.Text <> "" AndAlso CmbSupplier.SelectedIndex <> 0 Then
            Using cmd As New MySqlCommand("SELECT kode, ALamat, Hp FROM tbl_supliyer WHERE nama = @SupliyerNama", conn)
                cmd.Parameters.AddWithValue("@SupliyerNama", CmbSupplier.Text)
                Using rd As MySqlDataReader = cmd.ExecuteReader
                    If rd.Read() Then
                        ' Menggunakan metode yang sesuai untuk membaca data berdasarkan tipe datanya
                        If Not rd.IsDBNull(0) Then
                            LblKodeSupplier.Text = rd.GetString(0)
                        Else
                            LblKodeSupplier.Text = ""
                        End If
                        If Not rd.IsDBNull(1) Then
                            LblAlamatSupplier.Text = rd.GetString(1)
                        Else
                            LblAlamatSupplier.Text = ""
                        End If
                        If Not rd.IsDBNull(2) Then
                            LblKontakSupplier.Text = rd.GetString(2)
                        Else
                            LblKontakSupplier.Text = ""
                        End If

                    Else
                        LblKodeSupplier.Text = ""
                        LblAlamatSupplier.Text = ""
                        LblKontakSupplier.Text = ""
                    End If
                End Using
                Daftarbarangbynota()
            End Using
        Else
            LblKodeSupplier.Text = ""
            LblAlamatSupplier.Text = ""
            LblKontakSupplier.Text = ""
        End If
        If PanelDatagridview.Visible = False Then
            TxtNotaBeli.Text = ""
            TxtBayarBeli.Text = 0
            TxtSisaBayar.Text = 0
            LblBayarBeli.Text = ""
            LblSisaBayar.Text = ""
            LblStatusBeli.Text = ""
        End If

        DGVReturPembelian.Rows.Clear()
    End Sub


    Private Sub PBcariNotaBeli_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PBcariNotaBeli.Click, TxtNotaBeli.Click
        DtpBelanja.Value = DateTime.Now
        DtpBelanja.Format = DateTimePickerFormat.Custom
        DtpBelanja.CustomFormat = "dd/MM/yyyy"

        DGVReturPembelian.Visible = False
        PanelSimpan.Visible = False
        PanelDatagridview.Visible = True
        CenterPanel6()
    End Sub


    Private Sub TxtNotaBeli_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TxtNotaBeli.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Pemrosesan yang ingin Anda lakukan saat tombol Enter ditekan
            DtpBelanja.Value = DateTime.Now
            DtpBelanja.Format = DateTimePickerFormat.Custom
            DtpBelanja.CustomFormat = "dd/MM/yyyy"

            DGVReturPembelian.Visible = False
            PanelSimpan.Visible = False
            PanelDatagridview.Visible = True
            CenterPanel6()

            ' Sisipkan logika tambahan jika diperlukan
        End If
    End Sub

    Private Sub TxtNotaBeli_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtNotaBeli.TextChanged
        If TxtNotaBeli.Text <> "" Then
            Ambildatapembelian()
            Daftarbarangbynota()
        End If
    End Sub


    Private dtBarang As DataTable
    'Private Sub Daftarbarangbynota()
    '    CenterPanel6()
    '    LblPilihbarang.Text = "Pilih barang yang akan diretur dari pembelian : " & TxtNotaBeli.Text

    '    ' Menggunakan fungsi MAX untuk mendapatkan harga terbaru
    '    Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, MAX(HARGA_BELI) AS HARGA_BELI FROM pembelian_detail"

    '    ' Menambahkan kondisi WHERE sesuai dengan CheckBox CbJenisRetur
    '    If CbJenisRetur.Checked Then
    '        'query &= " WHERE NAMA_SUPLIYER LIKE @Supplier"

    '    Else
    '        query &= " WHERE FAKTUR_BELI LIKE @FAKTUR_BELI"
    '    End If

    '    query &= " GROUP BY ID_BARANG, NAMA_BARANG ORDER BY NAMA_BARANG"


    '    Using cmd As New MySqlCommand(query, conn)
    '        ' Menambahkan parameter jika CbJenisRetur.Checked
    '        If CbJenisRetur.Checked Then
    '            'cmd.Parameters.AddWithValue("@Supplier", CmbSupplier.Text)
    '        Else
    '            cmd.Parameters.AddWithValue("@FAKTUR_BELI", TxtNotaBeli.Text)
    '        End If

    '        Using rd As MySqlDataReader = cmd.ExecuteReader()
    '            DGVPilihBarang.Rows.Clear()

    '            If rd.HasRows Then
    '                Do While rd.Read()
    '                    DGVPilihBarang.Rows.Add(rd("ID_BARANG"), rd("NAMA_BARANG"), rd("HARGA_BELI"))
    '                Loop
    '            End If
    '        End Using
    '    End Using

    '    DGVPilihBarang.Columns(2).DefaultCellStyle.Format = "N0"
    '    DGVPilihBarang.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

    'End Sub

    Private Sub Daftarbarangbynota()
        CenterPanel6()
        LblPilihbarang.Text = "Pilih barang yang akan diretur dari pembelian : " & TxtNotaBeli.Text

        Dim culture As CultureInfo = CultureInfo.GetCultureInfo("id-ID")
        Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, MAX(HARGA_BELI) AS HARGA_BELI FROM pembelian_detail"

        If CbJenisRetur.Checked Then
            'query &= " WHERE NAMA_SUPLIYER LIKE @Supplier"
        Else
            query &= " WHERE FAKTUR_BELI LIKE @FAKTUR_BELI"
        End If

        query &= " GROUP BY ID_BARANG, NAMA_BARANG ORDER BY NAMA_BARANG"

        dtBarang = New DataTable()

        Using cmd As New MySqlCommand(query, conn)
            If CbJenisRetur.Checked Then
                'cmd.Parameters.AddWithValue("@Supplier", CmbSupplier.Text)
            Else
                cmd.Parameters.AddWithValue("@FAKTUR_BELI", TxtNotaBeli.Text)
            End If

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                dtBarang.Load(rd)
            End Using
        End Using

        ' Tampilkan data pertama kali (belum difilter)
        DGVPilihBarang.DataSource = dtBarang

        ' Atur tampilan kolom
        With DGVPilihBarang
            If .Columns.Count >= 3 Then
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                .Columns(0).FillWeight = 50
                .Columns(1).FillWeight = 150
                .Columns(2).FillWeight = 100
                .Columns(2).DefaultCellStyle.FormatProvider = cultureIndonesia
                .Columns(2).DefaultCellStyle.Format = "N0"
                .Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        End With
    End Sub

    Private Sub TxtCariRetur_TextChanged(sender As Object, e As EventArgs) Handles TxtCariRetur.TextChanged
        If dtBarang Is Nothing Then Exit Sub

        Dim dv As New DataView(dtBarang)
        Dim filterText As String = TxtCariRetur.Text.Replace("'", "''") ' untuk menghindari error kutip
        dv.RowFilter = $"ID_BARANG LIKE '%{filterText}%' OR NAMA_BARANG LIKE '%{filterText}%'"

        DGVPilihBarang.DataSource = dv
    End Sub

    Private Sub TxtCariRetur_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtCariRetur.KeyDown
        ' Cek jika tombol yang ditekan adalah panah bawah
        If e.KeyCode = Keys.Down Then
            ' Cek apakah DataGridView memiliki baris
            If DGVPilihBarang.Rows.Count > 0 Then
                DGVPilihBarang.Focus()
                DGVPilihBarang.CurrentCell = DGVPilihBarang.Rows(0).Cells(0)
            End If

        End If
    End Sub



    Private Sub DtpBelanja_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DtpBelanja.ValueChanged
        Ambildatapembelian()
    End Sub

    Private Sub Ambildatapembelian()
        Dim tanggalAwal As Date = DtpBelanja.Value.Date
        Dim tanggalAkhir As Date = DtpBelanja.Value.Date.AddDays(1).AddTicks(-1)
        Dim query As String = "SELECT ID_PEMBELIAN, TGL_BELI, NAMA_SUPLIYER, PEMBAYARAN, TAGIHAN, STATUS_TRANSAKSI_BELI FROM pembelian WHERE TGL_BELI BETWEEN @tanggalAwal AND @tanggalAkhir AND LOKASI =@LOKASI"

        Dim dataTable As New DataTable()

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@tanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@tanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)

            ' Use a DataAdapter to fill the DataTable
            Dim adapter As New MySqlDataAdapter(cmd)
            adapter.Fill(dataTable)
        End Using


        ' Bind the DataTable to the DataGridView
        DGVPembelian.DataSource = dataTable

        ' Set DataGridView properties
        With DGVPembelian
            .Columns("TGL_BELI").DefaultCellStyle.Format = "dd/MM/yyyy"
            .Columns("PEMBAYARAN").DefaultCellStyle.Format = "N0"
            .Columns("PEMBAYARAN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("TAGIHAN").DefaultCellStyle.Format = "N0"
            .Columns("TAGIHAN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("ID_PEMBELIAN").HeaderText = "No Nota"
            .Columns("TGL_BELI").HeaderText = "Tanggal"
            .Columns("NAMA_SUPLIYER").HeaderText = "Supplier"
            .Columns("PEMBAYARAN").HeaderText = "Pembayaran"
            .Columns("TAGIHAN").HeaderText = "Hutang"
            .Columns("STATUS_TRANSAKSI_BELI").HeaderText = "Status"
        End With
    End Sub


    Private Sub DGVPembelian_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVPembelian.CellClick
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then ' Pastikan yang diklik adalah sel di dalam baris
            TxtNotaBeli.Text = DGVPembelian.Item(0, e.RowIndex).Value.ToString()
            DTPtglBeli.Text = DGVPembelian.Item(1, e.RowIndex).Value.ToString()
            CmbSupplier.Text = DGVPembelian.Item(2, e.RowIndex).Value.ToString()

            Dim tagihan As Decimal
            If Decimal.TryParse(DGVPembelian.Item(3, e.RowIndex).Value.ToString(), tagihan) Then
                TxtBayarBeli.Text = tagihan.ToString()
                LblBayarBeli.Text = "Rp. " & tagihan.ToString("N0") ' Format tagihan dengan Rp. dan N0
            Else
                TxtBayarBeli.Text = "0"
                LblBayarBeli.Text = "Rp. 0"
            End If

            Dim sisaBayar As Decimal
            If Decimal.TryParse(DGVPembelian.Item(4, e.RowIndex).Value.ToString(), sisaBayar) Then
                TxtSisaBayar.Text = sisaBayar.ToString()
                LblSisaBayar.Text = "Rp. " & sisaBayar.ToString("N0") ' Format sisa bayar dengan Rp. dan N0
            Else
                TxtSisaBayar.Text = "0"
                LblSisaBayar.Text = "Rp. 0"
            End If

            LblStatusBeli.Text = DGVPembelian.Item(5, e.RowIndex).Value.ToString()

            DGVReturPembelian.Visible = True
            PanelSimpan.Visible = True
            PanelDatagridview.Visible = False
        End If
    End Sub


    Private Sub DGVPilihBarang_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVPilihBarang.CellClick
        If e.RowIndex >= 0 AndAlso DGVPilihBarang.Rows(e.RowIndex).Cells(0).Value IsNot Nothing Then
            If Not String.IsNullOrEmpty(DGVPilihBarang.Rows(e.RowIndex).Cells(0).Value.ToString()) Then
                DGVReturPembelian.Rows.Add(1)
                Dim indeksBaris As Integer = DGVReturPembelian.RowCount - 2
                DGVReturPembelian.Rows(indeksBaris).Cells(0).Value = DGVPilihBarang.Rows(e.RowIndex).Cells(0).Value.ToString()
                DGVReturPembelian.Rows(indeksBaris).Cells(1).Value = DGVPilihBarang.Rows(e.RowIndex).Cells(1).Value.ToString()
                DGVReturPembelian.Rows(indeksBaris).Cells(2).Value = DGVPilihBarang.Rows(e.RowIndex).Cells(2).Value.ToString()

                Dim idBarang As String = DGVReturPembelian.Rows(indeksBaris).Cells(0).Value.ToString()

                If Not String.IsNullOrEmpty(idBarang) Then
                    Dim querySatuan As String = "SELECT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @Idbarang"

                    Using sqlSatuan As New MySqlCommand(querySatuan, conn)
                        sqlSatuan.Parameters.AddWithValue("@Idbarang", idBarang)

                        Using dataReader As MySqlDataReader = sqlSatuan.ExecuteReader()
                            If dataReader.Read() Then
                                Dim comboCell As DataGridViewComboBoxCell = CType(DGVReturPembelian.Rows(indeksBaris).Cells("SATUAN"), DataGridViewComboBoxCell)
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

                                DGVReturPembelian.Rows(indeksBaris).Cells("QTY").Value = 1
                                DGVReturPembelian.Rows(indeksBaris).Cells("SATUAN").Value = satuan
                                DGVReturPembelian.Rows(indeksBaris).Cells("ISI_SATUAN").Value = isi
                                DGVReturPembelian.Rows(indeksBaris).Cells("HARGA_BELI_SATUAN").Value = CDec(DGVReturPembelian.Rows(indeksBaris).Cells("HARGA_BELI").Value) * isi

                                DGVReturPembelian.Rows(indeksBaris).Cells("QTY_SAT").Value = CDec(DGVReturPembelian.Rows(indeksBaris).Cells("QTY").Value) * isi
                                DGVReturPembelian.Rows(indeksBaris).Cells("TOTAL").Value = CDec(DGVReturPembelian.Rows(indeksBaris).Cells("HARGA_BELI").Value) * CDec(DGVReturPembelian.Rows(indeksBaris).Cells("QTY_SAT").Value)
                            End If
                        End Using
                    End Using
                End If
            End If
            Panelcaribarang.Visible = False
            Datagrid()
            Grand_total()
            Hitungbarang()
            Hitungqty()
        End If
    End Sub


    Public Sub AddItems(ByVal col As AutoCompleteStringCollection, ByVal namaValue As String)
        If CbJenisRetur.Checked = False Then
            If String.IsNullOrEmpty(TxtNotaBeli.Text.Trim()) Then
                MessageBox.Show("Silahkan isi nota beli terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
        End If

        If Not String.IsNullOrEmpty(CmbSupplier.Text) AndAlso CmbSupplier.SelectedIndex <> 0 Then
            Dim query As String = "SELECT DISTINCT NAMA_BARANG FROM pembelian_detail WHERE NAMA_BARANG LIKE @Nama AND NAMA_SUPLIYER LIKE @Supplier"

            If CbJenisRetur.Checked = False Then
                query &= " AND FAKTUR_BELI LIKE @FAKTUR_BELI"
            End If

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Nama", "%" & namaValue & "%")
                cmd.Parameters.AddWithValue("@Supplier", CmbSupplier.Text)

                If CbJenisRetur.Checked = False Then
                    cmd.Parameters.AddWithValue("@FAKTUR_BELI", TxtNotaBeli.Text)
                End If

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    Do While rd.Read()
                        col.Add(rd("NAMA_BARANG").ToString())
                    Loop
                End Using
            End Using
        Else
            MessageBox.Show("Silahkan pilih supplier terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub



    Private Sub DGVReturPembelian_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles DGVReturPembelian.EditingControlShowing

        Dim titleText As String = DGVReturPembelian.Columns(1).HeaderText
        If titleText.Equals("NAMA BARANG") Then
            Dim autoText As TextBox = TryCast(e.Control, TextBox)
            If autoText IsNot Nothing Then
                autoText.AutoCompleteMode = AutoCompleteMode.Suggest
                autoText.AutoCompleteSource = AutoCompleteSource.CustomSource
                Dim DataCollection As New AutoCompleteStringCollection()
                AddItems(DataCollection, autoText.Text.Trim()) ' Melewati nilai autoText.Text sebagai namaValue.
                autoText.AutoCompleteCustomSource = DataCollection
            End If
        End If

        ' Periksa apakah kolom yang saat ini sedang diedit adalah kolom yang berisi ComboBox (misalnya, kolom dengan indeks 4)
        If DGVReturPembelian.CurrentCell.ColumnIndex = 4 Then
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
        Dim cell As DataGridViewComboBoxCell = DirectCast(DGVReturPembelian.CurrentCell, DataGridViewComboBoxCell)
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
                    Dim rowIndex As Integer = DGVReturPembelian.CurrentCell.RowIndex
                    DGVReturPembelian("HARGA_BELI_SATUAN", rowIndex).Value = CDec(DGVReturPembelian("HARGA_BELI", rowIndex).Value) * CDec(DGVReturPembelian("ISI_SATUAN", rowIndex).Value)
                    DGVReturPembelian("QTY_SAT", rowIndex).Value = CDec(DGVReturPembelian("ISI_SATUAN", rowIndex).Value) * CDec(DGVReturPembelian("QTY", rowIndex).Value)
                    DGVReturPembelian("TOTAL", rowIndex).Value = CDec(DGVReturPembelian("HARGA_BELI", rowIndex).Value) * CDec(DGVReturPembelian("QTY_SAT", rowIndex).Value)

                    Grand_total()
                    Hitungbarang()
                    Hitungqty()
                Else
                    MessageBox.Show("Satuan barang dan atau harga jual belum di input !!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            End Using
        End Using
    End Sub

    Private Sub DGVReturPembelian_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVReturPembelian.CellEndEdit
        '========================== Nama
        If e.ColumnIndex = 1 Then
            If CmbSupplier.Text = "" Or CmbSupplier.SelectedIndex = 0 Then
                MessageBox.Show("Silahkan pilih supplier terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            If DGVReturPembelian.Rows(e.RowIndex) IsNot Nothing AndAlso DGVReturPembelian.Rows(e.RowIndex).Cells("NAMA_BARANG") IsNot Nothing Then
                Dim namaCellValue As Object = DGVReturPembelian.Rows(e.RowIndex).Cells("NAMA_BARANG").Value
                If namaCellValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(namaCellValue.ToString().Trim()) Then
                    Dim namaValue As String = namaCellValue.ToString().Trim()

                    Dim query As String = "SELECT ID_BARANG, NAMA_BARANG, HARGA_BELI FROM pembelian_detail WHERE TRIM(NAMA_BARANG) LIKE @NamaValue LIMIT 1"


                    Using cmd As New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@NamaValue", namaValue.Trim())

                        Using rd As MySqlDataReader = cmd.ExecuteReader()
                            If rd.Read() Then
                                DGVReturPembelian.Rows(e.RowIndex).Cells("ID_BARANG").Value = rd("ID_BARANG")
                                DGVReturPembelian.Rows(e.RowIndex).Cells("HARGA_BELI").Value = rd("HARGA_BELI")
                                DGVReturPembelian.Rows(e.RowIndex).Cells("NAMA_BARANG").Value = rd("NAMA_BARANG")
                            Else
                                DGVReturPembelian.Rows(e.RowIndex).Cells("ID_BARANG").Value = ""
                                DGVReturPembelian.Rows(e.RowIndex).Cells("HARGA_BELI").Value = ""
                                DGVReturPembelian.Rows(e.RowIndex).Cells("NAMA_BARANG").Value = ""
                                SendKeys.Send("{down}")
                                Panelcaribarang.Visible = True
                            End If
                            rd.Close()

                            If DGVReturPembelian.Rows(e.RowIndex).Cells("ID_BARANG").Value <> "" Then
                                Dim querySatuan As String = "SELECT SATUAN_UMUM_KECIL, SATUAN_UMUM_SEDANG, SATUAN_UMUM_BESAR, ISI_UMUM_KECIL, ISI_UMUM_SEDANG, ISI_UMUM_BESAR FROM tbl_barang WHERE TRIM(ID_BARANG) LIKE @Idbarang"

                                Using sqlSatuan As New MySqlCommand(querySatuan, conn)
                                    sqlSatuan.Parameters.AddWithValue("@Idbarang", DGVReturPembelian.Rows(e.RowIndex).Cells("ID_BARANG").Value)

                                    Using dataReader As MySqlDataReader = sqlSatuan.ExecuteReader()
                                        If dataReader.Read() Then

                                            Dim comboCell As DataGridViewComboBoxCell = CType(DGVReturPembelian.Rows(e.RowIndex).Cells("SATUAN"), DataGridViewComboBoxCell)
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

                                            DGVReturPembelian.Rows(e.RowIndex).Cells("QTY").Value = 1
                                            DGVReturPembelian.Rows(e.RowIndex).Cells("SATUAN").Value = satuan
                                            DGVReturPembelian.Rows(e.RowIndex).Cells("ISI_SATUAN").Value = isi
                                            DGVReturPembelian.Rows(e.RowIndex).Cells("HARGA_BELI_SATUAN").Value = CDec(DGVReturPembelian.Rows(e.RowIndex).Cells("HARGA_BELI").Value) * isi

                                            DGVReturPembelian.Rows(e.RowIndex).Cells("QTY_SAT").Value = CDec(DGVReturPembelian.Rows(e.RowIndex).Cells("QTY").Value) * isi
                                            DGVReturPembelian.Rows(e.RowIndex).Cells("TOTAL").Value = CDec(DGVReturPembelian.Rows(e.RowIndex).Cells("HARGA_BELI").Value) * CDec(DGVReturPembelian.Rows(e.RowIndex).Cells("QTY_SAT").Value)
                                        End If
                                    End Using
                                End Using
                            End If
                            If DGVReturPembelian.Rows(e.RowIndex).Cells("NAMA_BARANG").Value = "" Then
                                SendKeys.Send("{down}")
                                Panelcaribarang.Visible = True
                            End If
                        End Using
                    End Using
                End If
            Else
                Hapusbaris()
            End If
        End If
        '========================== Harga beli
        If e.ColumnIndex = 2 Then
            Dim hargaBeliValue As Decimal

            If Decimal.TryParse(DGVReturPembelian.Rows(e.RowIndex).Cells("HARGA_BELI").Value, hargaBeliValue) Then
                If hargaBeliValue <= 0 Then
                    MessageBox.Show("Harga beli harus lebih besar dari 0.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    DGVReturPembelian.Rows(e.RowIndex).Cells("Hargabeli").Value = 0
                Else
                    Dim qtyValue As Decimal = DGVReturPembelian.Rows(e.RowIndex).Cells("QTY").Value
                    Dim isiValue As Decimal = DGVReturPembelian.Rows(e.RowIndex).Cells("ISI_SATUAN").Value
                    Dim qtySatValue As Decimal = qtyValue * isiValue

                    DGVReturPembelian.Rows(e.RowIndex).Cells("QTY_SAT").Value = qtySatValue
                    DGVReturPembelian.Rows(e.RowIndex).Cells("HARGA_BELI_SATUAN").Value = CDec(DGVReturPembelian.Rows(e.RowIndex).Cells("HARGA_BELI").Value) * isiValue
                    DGVReturPembelian.Rows(e.RowIndex).Cells("TOTAL").Value = hargaBeliValue * qtySatValue
                End If
            Else
                MessageBox.Show("Harga beli harus berupa angka.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                DGVReturPembelian.Rows(e.RowIndex).Cells("HARGA_BELI").Value = 0
            End If

        End If

        '========================== Qty
        If e.ColumnIndex = 3 Then
            Dim rowIndex As Integer = e.RowIndex

            Dim qtyCell As DataGridViewCell = DGVReturPembelian.Rows(rowIndex).Cells("QTY")
            Dim qtySatCell As DataGridViewCell = DGVReturPembelian.Rows(rowIndex).Cells("QTY_SAT")
            Dim hargaBeliCell As DataGridViewCell = DGVReturPembelian.Rows(rowIndex).Cells("HARGA_BELI")
            Dim isiCell As DataGridViewCell = DGVReturPembelian.Rows(rowIndex).Cells("ISI_SATUAN")
            Dim totalHargaCell As DataGridViewCell = DGVReturPembelian.Rows(rowIndex).Cells("TOTAL")

            Dim qtyValue As Decimal = Convert.ToDecimal(qtyCell.Value)

            If qtyValue <= 0 Then
                MessageBox.Show("Qty harus lebih besar dari 0.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                qtyCell.Value = 1
            End If

            qtySatCell.Value = qtyValue * CDec(isiCell.Value)
            totalHargaCell.Value = CDec(hargaBeliCell.Value) * CDec(qtySatCell.Value)

            ' Matikan penggambaran ulang DataGridView untuk efisiensi.
            DGVReturPembelian.SuspendLayout()
            DGVReturPembelian.ResumeLayout()
        End If
        Datagrid()
        Grand_total()
        Hitungbarang()
        Hitungqty()
    End Sub

    Private Sub DGVReturPembelian_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles DGVReturPembelian.DataError
        e.Cancel = True
    End Sub
    Private Sub Datagrid()
        With DGVReturPembelian
            .Columns("HARGA_BELI").DefaultCellStyle.Format = "###,###"
            .Columns("HARGA_BELI").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("HARGA_BELI_SATUAN").DefaultCellStyle.Format = "###,###"
            .Columns("HARGA_BELI_SATUAN").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Columns("TOTAL").DefaultCellStyle.Format = "###,###"
            .Columns("TOTAL").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
        End With
    End Sub


    Private Sub Hapusbaris()
        Dim baris As Integer = DGVReturPembelian.CurrentCell.RowIndex

        If baris < DGVReturPembelian.Rows.Count - 1 Then
            ' Hapus baris yang sudah ada.
            DGVReturPembelian.Rows.RemoveAt(baris)
        Else
            ' Batalkan pengeditan jika ini adalah baris baru yang belum dikonfirmasi.
            If DGVReturPembelian.IsCurrentRowDirty Then
                DGVReturPembelian.CancelEdit()
            End If

            ' Hapus baris baru.
            DGVReturPembelian.Rows.RemoveAt(baris)
        End If

        Grand_total()
        Hitungbarang()
        Hitungqty()
    End Sub



    Public Sub Grand_total()
        Dim jumlah As Decimal = 0
        For i As Integer = 0 To DGVReturPembelian.Rows.Count - 1
            jumlah += Convert.ToDecimal(DGVReturPembelian.Rows(i).Cells("TOTAL").Value)
        Next
        TxtTotalRupiah.Text = jumlah.ToString()
        LblTotalRupiah.Text = "Rp. " & jumlah.ToString("N0")
    End Sub

    Private Sub Hitungbarang()
        Dim jumlahBaris As Integer = DGVReturPembelian.RowCount - 1
        TxtTotalBarang.Text = jumlahBaris.ToString()
        LblTotalBarang.Text = jumlahBaris.ToString("N0")
    End Sub

    Private Sub Hitungqty()
        Dim jumlah As Decimal = 0
        For i As Integer = 0 To DGVReturPembelian.Rows.Count - 1
            jumlah += Convert.ToDecimal(DGVReturPembelian.Rows(i).Cells("QTY_SAT").Value)
        Next
        TxtTotalQTY.Text = jumlah.ToString()
        LblTotalQTY.Text = jumlah.ToString("N0")
    End Sub

    Private Sub DGVReturPembelian_CellMouseUp(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DGVReturPembelian.CellMouseUp
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            DGVReturPembelian.CurrentCell = DGVReturPembelian.Rows(e.RowIndex).Cells("NAMA_BARANG")
            Dim cursorPosition As Point = System.Windows.Forms.Cursor.Position
            CMSHapus.Show(cursorPosition)
        End If
    End Sub

    Private Sub TSMhapus_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TSMhapus.Click
        Call Hapusbaris()
    End Sub


    Private Sub CbTunai_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbTunai.CheckedChanged
        If CbTunai.Checked Then
            LblKodeAkun.Text = ""
            CbPotongHutang.Checked = False
            Rekeningkasbank()

            If LblLokasi.Text = "TOKO" Then
                CmbRekening.SelectedItem = nama_rek_Jual_Toko
                LblKodeAkun.Text = Kode_rek_Jual_Toko
            ElseIf LblLokasi.Text = "GUDANG" Then
                CmbRekening.SelectedItem = nama_rek_Jual_Gudang
                LblKodeAkun.Text = Kode_rek_Jual_Gudang
            End If
        End If
    End Sub

    Private Sub CbPotongHutang_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CbPotongHutang.CheckedChanged
        If CbPotongHutang.Checked Then
            CbTunai.Checked = False
            CmbRekening.Items.Clear()
            CmbRekening.Items.Add(nama_rek_Hutang_Beli)
            CmbRekening.SelectedIndex = 0
            LblKodeAkun.Text = Kode_rek_Hutang_Beli

            Dim sisaBayar As Decimal
            Dim totalRupiah As Decimal

            If Decimal.TryParse(TxtSisaBayar.Text, sisaBayar) AndAlso Decimal.TryParse(TxtTotalRupiah.Text, totalRupiah) Then
                If sisaBayar = totalRupiah Then
                    LblStatusHutang.Text = "Lunas"
                Else
                    LblStatusHutang.Text = "Belum Lunas"
                End If
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

    Private Sub LblStatusBeli_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LblStatusBeli.TextChanged
        If LblStatusBeli.Text = "Lunas" Then
            CbPotongHutang.Visible = False
            CbTunai.Checked = True
        Else
            CbPotongHutang.Visible = True
        End If
    End Sub


    Private Sub BtnSimpan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSimpan.Click
        ' Mulai transaksi
        Dim transaction As MySqlTransaction = Nothing

        Try
            transaction = conn.BeginTransaction()

            Simpanreturpembelian(transaction)
            SimpanUpdateHutangpembelian(transaction)
            HistoryBarang(transaction)
            Simpanreturpembeliandetail(transaction)
            Simpanjurnal(transaction)

            ' Commit transaksi jika berhasil
            transaction.Commit()
            ' Jika semuanya berhasil, kembalikan kondisi awal
            For Each row As DataGridViewRow In DGVReturPembelian.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                    HitungByKode(row.Cells(0).Value)
                End If
            Next

            DatabaseModule.CatatanAksiHistory("Simpan retur pembelian " & LblNoNotaRetur.Text)
            Kondisiawalretur()

        Catch ex As Exception
            transaction.Rollback()

            ' Tampilkan pesan kesalahan kepada pengguna
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SimpanUpdateHutangpembelian(ByVal transaction As MySqlTransaction)
        Dim updateQuery As String

        If CbPotongHutang.Checked Then
            updateQuery = "UPDATE pembelian SET TGL_RETUR = ?, RETUR = RETUR + ?, TAGIHAN = TAGIHAN - ?, STATUS_TRANSAKSI_BELI = ? WHERE ID_PEMBELIAN = ?"
        Else
            updateQuery = "UPDATE pembelian SET TGL_RETUR = ?, RETUR = RETUR + ? WHERE ID_PEMBELIAN = ?"
        End If

        Using cmdUpdate As New MySqlCommand(updateQuery, conn, transaction)
            cmdUpdate.Parameters.AddWithValue("@TGL_RETUR", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmdUpdate.Parameters.AddWithValue("@RETUR", Convert.ToDecimal(TxtTotalRupiah.Text))

            If CbPotongHutang.Checked Then
                ' Pastikan nilai di TxtSisaBayar bersifat numerik sebelum menggunakannya
                Dim tagihanSaatIni As Decimal = Convert.ToDecimal(TxtSisaBayar.Text)
                cmdUpdate.Parameters.AddWithValue("@TAGIHAN", tagihanSaatIni - Convert.ToDecimal(TxtTotalRupiah.Text))
                cmdUpdate.Parameters.AddWithValue("@STATUS_TRANSAKSI_BELI", LblStatusHutang.Text)
            End If

            cmdUpdate.Parameters.AddWithValue("@ID_PEMBELIAN", TxtNotaBeli.Text)
            cmdUpdate.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub Simpanreturpembelian(ByVal transaction As MySqlTransaction)
        Dim query As String = "INSERT INTO retur_pembelian (ID_RETUR_PEMBELIAN, TGL_RETUR_BELI, ID_SUPPLIER, NAMA_SUPPLIER, ALAMAT_SUPLPLIER, KONTAK_SUPPLIER, ID_PEMBELIAN, TGL_PEMBELIAN, STATUS_PEMBELIAN, PENYIMPANAN, BAYAR_PEMBELIAN, SISA_PEMBELIAN, TOTAL_BARANG, TOTAL_QTY, TOTAL_RUPIAH, NAMA_REKENING, KODE_REKENING, ALASAN_RETUR, ID_USER, ID_KOMPUTER) VALUES (@ID_RETUR_PEMBELIAN, @TGL_RETUR_BELI, @ID_SUPPLIER, @NAMA_SUPPLIER, @ALAMAT_SUPLPLIER, @KONTAK_SUPPLIER, @ID_PEMBELIAN, @TGL_PEMBELIAN, @STATUS_PEMBELIAN, @PENYIMPANAN, @BAYAR_PEMBELIAN, @SISA_PEMBELIAN, @TOTAL_BARANG, @TOTAL_QTY, @TOTAL_RUPIAH, @NAMA_REKENING, @KODE_REKENING, @ALASAN_RETUR, @ID_USER, @ID_KOMPUTER)"

        Using cmd As New MySqlCommand(query, conn, transaction)
            cmd.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", LblNoNotaRetur.Text)
            cmd.Parameters.AddWithValue("@TGL_RETUR_BELI", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@ID_SUPPLIER", LblKodeSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_SUPPLIER", CmbSupplier.Text)
            cmd.Parameters.AddWithValue("@ALAMAT_SUPLPLIER", LblAlamatSupplier.Text)
            cmd.Parameters.AddWithValue("@KONTAK_SUPPLIER", LblKontakSupplier.Text)
            cmd.Parameters.AddWithValue("@ID_PEMBELIAN", TxtNotaBeli.Text)
            cmd.Parameters.AddWithValue("@TGL_PEMBELIAN", DTPtglBeli.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@STATUS_PEMBELIAN", LblStatusBeli.Text)
            cmd.Parameters.AddWithValue("@PENYIMPANAN", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@BAYAR_PEMBELIAN", Convert.ToDecimal(TxtBayarBeli.Text))
            cmd.Parameters.AddWithValue("@SISA_PEMBELIAN", Convert.ToDecimal(TxtSisaBayar.Text))
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


    Private Sub Simpanreturpembeliandetail(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DGVReturPembelian.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim sqlrinci As String = "INSERT INTO retur_pembelian_detail (ID_RETUR_PEMBELIAN, TGL_RETUR_BELI, ID_SUPLIYER, NAMA_SUPLIYER, ID_BARANG, NAMA_BARANG, HARGA_BELI, QTY, SATUAN, ISI_SATUAN, HARGA_BELI_SATUAN, QTY_SAT, TOTAL, PENYIMPANAN, ID_USER, ID_KOMPUTER) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
                Using cmd As New MySqlCommand(sqlrinci, conn, transaction)
                    cmd.Parameters.AddWithValue("@ID_RETUR_PEMBELIAN", LblNoNotaRetur.Text)
                    cmd.Parameters.AddWithValue("@TGL_RETUR_BELI", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@ID_SUPLIYER", LblKodeSupplier.Text)
                    cmd.Parameters.AddWithValue("@NAMA_SUPLIYER", CmbSupplier.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
                    cmd.Parameters.AddWithValue("@HARGA_BELI", Convert.ToDecimal(row.Cells(2).Value))
                    cmd.Parameters.AddWithValue("@QTY", Convert.ToDecimal(row.Cells(3).Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", Convert.ToDecimal(row.Cells(5).Value))
                    cmd.Parameters.AddWithValue("@HARGA_BELI_SATUAN", Convert.ToDecimal(row.Cells(6).Value))
                    cmd.Parameters.AddWithValue("@QTY_SAT", Convert.ToDecimal(row.Cells(7).Value))
                    cmd.Parameters.AddWithValue("@TOTAL", Convert.ToDecimal(row.Cells(8).Value))
                    cmd.Parameters.AddWithValue("@PENYIMPANAN", LblLokasi.Text)
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
                    cmd.ExecuteNonQuery()
                End Using

                Dim updateStokField As String = String.Empty ' Inisialisasi dengan nilai default

                Select Case LblLokasi.Text
                    Case "TOKO"
                        updateStokField = "RETUR_BELI_TOKO"
                    Case "GUDANG"
                        updateStokField = "RETUR_BELI_GUDANG"
                    Case Else
                        Throw New InvalidOperationException("Lokasi barang tidak valid.")
                End Select

                Dim updateQuery As String = "UPDATE tbl_barang SET " & updateStokField & " = " & updateStokField & " + ? WHERE ID_BARANG = ?"

                Dim kodeBarang As String = row.Cells(0).Value.ToString()

                If Not String.IsNullOrEmpty(kodeBarang) Then
                    Dim stokPengurangan As Decimal = If(row.Cells(7).Value IsNot Nothing, Convert.ToDecimal(row.Cells(7).Value), 0D)

                    Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                        cmd.Parameters.AddWithValue("@StokPengurangan", stokPengurangan)
                        cmd.Parameters.AddWithValue("@KodeBarang", kodeBarang)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            End If
        Next
    End Sub


    Private Sub HistoryBarang(ByVal transaction As MySqlTransaction)
        ' Simpan data rincian barang dari gridview ke tbl_rinci_BELI
        For Each row As DataGridViewRow In DGVReturPembelian.Rows
            If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() <> "" Then
                Dim querySimpan As String = "INSERT INTO HistoryBarang (FAKTUR, TANGGAL, JENIS, LOKASI, ID_BARANG, NAMA_BARANG, QTY, SATUAN, ISI_SATUAN, TOTAL_QTY, TOTAL_RUPIAH, ID_USER, ID_KOMPUTER) " &
                                            "VALUES (@FAKTUR, @TANGGAL, @JENIS, @LOKASI, @ID_BARANG, @NAMA_BARANG, @QTY, @SATUAN, @ISI_SATUAN, @TOTAL_QTY, @TOTAL_RUPIAH, @ID_USER, @ID_KOMPUTER)"
                Using cmd As New MySqlCommand(querySimpan, conn, transaction)
                    cmd.Parameters.AddWithValue("@FAKTUR", LblNoNotaRetur.Text)
                    cmd.Parameters.AddWithValue("@TANGGAL", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@JENIS", "RETUR BELI")
                    cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
                    cmd.Parameters.AddWithValue("@ID_BARANG", row.Cells(0).Value)
                    cmd.Parameters.AddWithValue("@NAMA_BARANG", row.Cells(1).Value)
                    cmd.Parameters.AddWithValue("@QTY", Convert.ToDecimal(row.Cells(3).Value))
                    cmd.Parameters.AddWithValue("@SATUAN", row.Cells(4).Value)
                    cmd.Parameters.AddWithValue("@ISI_SATUAN", Convert.ToDecimal(row.Cells(5).Value))
                    cmd.Parameters.AddWithValue("@TOTAL_QTY", Convert.ToDecimal(row.Cells(7).Value))
                    cmd.Parameters.AddWithValue("@TOTAL_RUPIAH", Convert.ToDecimal(row.Cells(8).Value))
                    cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
                    cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        Next
    End Sub

    Private Sub Simpanjurnal(ByVal transaction As MySqlTransaction)
        ' Simpan ke jurnal
        Using cmd As New MySqlCommand("INSERT INTO JurnalUmum (NO_TRANSAKSI, TGL_TRANSAKSI, URAIAN, NAMA_AKUN_D, NOMOR_AKUN_D, NAMA_AKUN_K, NOMOR_AKUN_K, NAMA_BANTU_K, KODE_BANTU_K, NOMINAL, JENIS_TRANSAKSI, LOKASI, ID_USER, ID_KOMPUTER) " &
                                          "VALUES (@NO_TRANSAKSI, @TGL_TRANSAKSI, @URAIAN, @NAMA_AKUN_D, @NOMOR_AKUN_D, @NAMA_AKUN_K, @NOMOR_AKUN_K, @NAMA_BANTU_K, @KODE_BANTU_K, @NOMINAL, @JENIS_TRANSAKSI, @LOKASI, @ID_USER, @ID_KOMPUTER)", conn, transaction)

            cmd.Parameters.AddWithValue("@NO_TRANSAKSI", LblNoNotaRetur.Text)
            cmd.Parameters.AddWithValue("@TGL_TRANSAKSI", DTPRetur.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@URAIAN", "Retur belanja ke " & CmbSupplier.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_D", CmbRekening.Text)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_D", LblKodeAkun.Text)
            cmd.Parameters.AddWithValue("@NAMA_AKUN_K", NAMA_REK_BARANG)
            cmd.Parameters.AddWithValue("@NOMOR_AKUN_K", KODE_REK_BARANG)

            ' Kondisi jika tunai, bantu akun dikosongkan
            If CbTunai.Checked Then
                cmd.Parameters.AddWithValue("@NAMA_BANTU_K", DBNull.Value)
                cmd.Parameters.AddWithValue("@KODE_BANTU_K", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@NAMA_BANTU_K", CmbSupplier.Text)
                cmd.Parameters.AddWithValue("@KODE_BANTU_K", LblKodeSupplier.Text)
            End If

            ' Konversi nominal menjadi decimal dengan validasi
            cmd.Parameters.AddWithValue("@NOMINAL", Convert.ToDecimal(TxtTotalRupiah.Text))
            cmd.Parameters.AddWithValue("@JENIS_TRANSAKSI", "Retur Pembelian")
            cmd.Parameters.AddWithValue("@LOKASI", LblLokasi.Text)
            cmd.Parameters.AddWithValue("@ID_USER", FormUtama.SLogin.Text)
            cmd.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.Comp.Text)

            ' Eksekusi perintah SQL
            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Private Sub BtnKeluarDaftar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKeluarDaftar.Click
        DGVReturPembelian.Visible = True
        PanelSimpan.Visible = True
        PanelDatagridview.Visible = False
    End Sub

    Private Sub BtnKeluarBarang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnKeluarBarang.Click
        Panelcaribarang.Visible = False
    End Sub

    Private Sub BtnDaftarBarang_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDaftarBarang.Click
        If CmbSupplier.Text = "" Then
            MessageBox.Show("Silahkan pilih supplier terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CmbSupplier.Focus()
            Exit Sub
        End If
        Panelcaribarang.Visible = True
    End Sub

    Private Sub FormReturPembelian_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F8
                BtnSimpan.PerformClick()
            Case Keys.Escape
                If PanelDatagridview.Visible = True Then
                    BtnKeluarDaftar.PerformClick()
                ElseIf Panelcaribarang.Visible = True Then
                    BtnKeluarBarang.PerformClick()
                Else
                    BtnClose.PerformClick()
                End If

        End Select
    End Sub

    Private Sub BtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnClose.Click
        FormUtama.GBTransaksi.Visible = True
        FormUtama.Refresdatagridview()
        Close()
    End Sub


End Class