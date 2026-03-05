Imports Microsoft.Reporting.WinForms


Public Class FormLapMutasiBarang

    Private Sub AmbilDanSimpanDataMutasiBarang()
        ' Get the date range from DateTimePicker controls
        Dim tanggalAwal As Date = DateTimePicker1.Value.Date
        Dim tanggalAkhir As Date = DateTimePicker2.Value.Date.AddDays(1).AddTicks(-1)

        Try
            ' Begin the transaction
            transaction = conn.BeginTransaction()

            ' Clear the Temp_Mutasi_Barang table
            Dim clearTempTableQuery As String = "DELETE FROM Temp_Mutasi_Barang"
            Using cmdClear As New MySqlCommand(clearTempTableQuery, conn, transaction)
                cmdClear.ExecuteNonQuery()
            End Using

            ' Hitung saldo awal (saldo awal)
            Dim saldoAwal As Decimal = 0
            Dim querySaldoAwalBarang As String = "SELECT AWAL_TOKO, AWAL_GUDANG " &
                                                "FROM tbl_barang " &
                                                "WHERE ID_BARANG = ?"
            Using cmdSaldoAwal As New MySqlCommand(querySaldoAwalBarang, conn, transaction)
                cmdSaldoAwal.Parameters.AddWithValue("?", TxtKode.Text)

                Using reader As MySqlDataReader = cmdSaldoAwal.ExecuteReader()
                    While reader.Read()
                        Dim lokasi As String = CmbLokasi.Text
                        Select Case lokasi
                            Case "TOKO"
                                saldoAwal += Convert.ToDecimal(reader("AWAL_TOKO"))
                            Case "GUDANG"
                                saldoAwal += Convert.ToDecimal(reader("AWAL_GUDANG"))
                        End Select
                    End While
                End Using
            End Using


            Dim querySaldoAwal As String = "SELECT JENIS, SUM(TOTAL_QTY) AS TOTAL_QTY " &
                                     "FROM historybarang " &
                                     "WHERE TANGGAL < @TanggalAwal AND ID_BARANG = @IdBarang AND LOKASI = @Lokasi " &
                                     "GROUP BY JENIS"

            Using cmdSaldoAwal As New MySqlCommand(querySaldoAwal, conn, transaction)
                cmdSaldoAwal.Parameters.AddWithValue("@TanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdSaldoAwal.Parameters.AddWithValue("@IdBarang", TxtKode.Text)
                cmdSaldoAwal.Parameters.AddWithValue("@Lokasi", CmbLokasi.Text)

                Using reader As MySqlDataReader = cmdSaldoAwal.ExecuteReader()
                    While reader.Read()
                        Dim jenis As String = reader("JENIS").ToString()
                        Dim totalQty As Decimal = Convert.ToDecimal(reader("TOTAL_QTY"))

                        Select Case jenis
                            Case "TAMBAH", "PEMBELIAN", "RETUR JUAL", "OPNAME", "TRANSFER STOK MASUK", "TRANSFER BARANG MASUK"
                                saldoAwal += totalQty
                            Case "KURANG", "PENJUALAN", "RETUR BELI", "TRANSFER STOK KELUAR", "TRANSFER BARANG KELUAR"
                                saldoAwal -= totalQty
                        End Select
                    End While
                End Using
            End Using


            ' Insert initial balance record
            Dim insertSaldoAwalQuery As String = "INSERT INTO Temp_Mutasi_Barang (FAKTUR, TANGGAL, JENIS, LOKASI, QTY_MASUK, QTY_KELUAR, SALDO, ID_USER) " &
                                      "VALUES ('SA-000000001', @Tanggal, 'SALDO AWAL', @Lokasi, 0, 0, @Saldo, @IdUser)"

            Using cmdInsertSaldoAwal As New MySqlCommand(insertSaldoAwalQuery, conn, transaction)
                cmdInsertSaldoAwal.Parameters.AddWithValue("@Tanggal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdInsertSaldoAwal.Parameters.AddWithValue("@Lokasi", CmbLokasi.Text)
                cmdInsertSaldoAwal.Parameters.AddWithValue("@Saldo", saldoAwal)
                cmdInsertSaldoAwal.Parameters.AddWithValue("@IdUser", FormUtama.SLogin.Text)

                cmdInsertSaldoAwal.ExecuteNonQuery()
            End Using


            Dim records As New List(Of Dictionary(Of String, Object))

            ' Query untuk mengambil data
            Dim queryTanggalDitentukan As String = "SELECT FAKTUR, TANGGAL, JENIS, LOKASI, QTY, SATUAN, TOTAL_QTY, ID_USER " &
                                            "FROM historybarang " &
                                            "WHERE TANGGAL BETWEEN @TanggalAwal AND @TanggalAkhir AND ID_BARANG = @IdBarang AND LOKASI = @Lokasi ORDER BY TANGGAL"

            ' Mengambil data dari database
            Using cmdTanggalDitentukan As New MySqlCommand(queryTanggalDitentukan, conn, transaction)
                cmdTanggalDitentukan.Parameters.AddWithValue("@TanggalAwal", tanggalAwal.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdTanggalDitentukan.Parameters.AddWithValue("@TanggalAkhir", tanggalAkhir.ToString("yyyy-MM-dd HH:mm:ss"))
                cmdTanggalDitentukan.Parameters.AddWithValue("@IdBarang", TxtKode.Text)
                cmdTanggalDitentukan.Parameters.AddWithValue("@Lokasi", CmbLokasi.Text)

                Using reader As MySqlDataReader = cmdTanggalDitentukan.ExecuteReader()
                    While reader.Read()
                        ' Menyimpan setiap record dalam dictionary
                        Dim record As New Dictionary(Of String, Object) From {
                    {"FAKTUR", reader("FAKTUR").ToString()},
                    {"TANGGAL", Convert.ToDateTime(reader("TANGGAL"))},
                    {"JENIS", reader("JENIS").ToString()},
                    {"LOKASI", reader("LOKASI").ToString()},
                    {"TOTAL_QTY", Convert.ToDecimal(reader("TOTAL_QTY"))},
                    {"ID_USER", reader("ID_USER").ToString()}
                }
                        records.Add(record)
                    End While
                End Using
            End Using

            ' Memproses data yang disimpan dalam dictionary list
            For Each record As Dictionary(Of String, Object) In records
                Dim faktur As String = record("FAKTUR").ToString()
                Dim tanggal As Date = CType(record("TANGGAL"), Date)
                Dim jenis As String = record("JENIS").ToString()
                Dim lokasi As String = record("LOKASI").ToString()
                Dim totalQty As Decimal = CType(record("TOTAL_QTY"), Decimal)
                Dim idUser As String = record("ID_USER").ToString()

                Dim qtyMasuk As Decimal = 0
                Dim qtyKeluar As Decimal = 0

                ' Menghitung qty masuk dan keluar berdasarkan jenis transaksi
                Select Case jenis
                    Case "TAMBAH", "PEMBELIAN", "RETUR JUAL", "OPNAME", "TRANSFER STOK MASUK", "TRANSFER BARANG MASUK"
                        qtyMasuk = totalQty
                        saldoAwal += totalQty
                    Case "KURANG", "PENJUALAN", "RETUR BELI", "TRANSFER STOK KELUAR", "TRANSFER BARANG KELUAR"
                        qtyKeluar = totalQty
                        saldoAwal -= totalQty
                End Select

                ' Query untuk memasukkan data ke tabel sementara
                Dim insertQuery As String = "INSERT INTO Temp_Mutasi_Barang (FAKTUR, TANGGAL, JENIS, LOKASI, QTY_MASUK, QTY_KELUAR, SALDO, ID_USER) " &
                                     "VALUES (@Faktur, @Tanggal, @Jenis, @Lokasi, @QtyMasuk, @QtyKeluar, @Saldo, @IdUser)"

                ' Menyimpan data ke tabel sementara
                Using cmdInsert As New MySqlCommand(insertQuery, conn, transaction)
                    cmdInsert.Parameters.AddWithValue("@Faktur", faktur)
                    cmdInsert.Parameters.AddWithValue("@Tanggal", tanggal.ToString("yyyy-MM-dd HH:mm:ss"))
                    cmdInsert.Parameters.AddWithValue("@Jenis", jenis)
                    cmdInsert.Parameters.AddWithValue("@Lokasi", lokasi)
                    cmdInsert.Parameters.AddWithValue("@QtyMasuk", qtyMasuk)
                    cmdInsert.Parameters.AddWithValue("@QtyKeluar", qtyKeluar)
                    cmdInsert.Parameters.AddWithValue("@Saldo", saldoAwal)
                    cmdInsert.Parameters.AddWithValue("@IdUser", idUser)

                    cmdInsert.ExecuteNonQuery()
                End Using
            Next


            ' Commit the transaction 
            transaction.Commit()
        Catch ex As Exception
            ' Handle or log the exception as needed
            MessageBox.Show("Terjadi kesalahan saat mengambil saldo awal: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    ' Handler untuk event GotFocus pada TextBox
    Private Sub TxtNama_GotFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.GotFocus
        ' Ubah warna latar belakang saat TextBox mendapatkan fokus
        PanelCariNama.BackColor = Color.Yellow ' Ganti warna fokus sesuai kebutuhan
    End Sub

    ' Handler untuk event LostFocus pada TextBox
    Private Sub TxtNama_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TxtNama.LostFocus
        ' Kembalikan warna latar belakang ke warna asli saat TextBox kehilangan fokus
        PanelCariNama.BackColor = SystemColors.Control ' Ganti warna fokus sesuai kebutuhan
    End Sub


    Private Sub FormLapMutasiBarang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TxtNama.Clear()
        TxtKode.Clear()
        DateTimePicker1.Value = DateTime.Now
        DateTimePicker2.Value = DateTime.Now

        CmbLokasi.SelectedItem = FormUtama.SLokasi.Text

        Tampil()
        ReportViewer1.LocalReport.DataSources.Clear()
        TxtNama.Select()
    End Sub

    Public Sub Tampil()
        Dim searchTerm As String = TxtNama.Text.Trim()
        Dim query As String = "SELECT NAMA_BARANG FROM tbl_barang WHERE NAMA_BARANG LIKE @searchTerm OR BARCODE_KECIL LIKE @searchTerm OR BARCODE_SEDANG LIKE @searchTerm OR BARCODE_BESAR LIKE @searchTerm"

        Dim dt As New DataTable

        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

            Using da As New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Dim a As New AutoCompleteStringCollection
        For i As Integer = 0 To dt.Rows.Count - 1
            a.Add(dt.Rows(i)("NAMA_BARANG").ToString())
        Next

        TxtNama.AutoCompleteSource = AutoCompleteSource.CustomSource
        TxtNama.AutoCompleteCustomSource = a
        TxtNama.AutoCompleteMode = AutoCompleteMode.Suggest

        dt.Dispose() ' Pastikan Anda membebaskan objek DataTable setelah digunakan.
    End Sub

    Private Sub TxtNama_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtNama.TextChanged
        Dim sql As String = "SELECT ID_BARANG FROM tbl_barang WHERE NAMA_BARANG = @NAMA_BARANG"

        Using cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@NAMA_BARANG", TxtNama.Text.Trim())

            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    TxtKode.Text = rd("ID_BARANG").ToString()
                Else
                    TxtKode.Text = ""
                End If
            End Using
        End Using
    End Sub

    Private Sub TxtNama_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtNama.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim query As String = "SELECT NAMA_BARANG FROM tbl_barang WHERE NAMA_BARANG LIKE @searchTerm OR BARCODE_KECIL LIKE @searchTerm OR BARCODE_SEDANG LIKE @searchTerm OR BARCODE_BESAR LIKE @searchTerm"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@searchTerm", TxtNama.Text.Trim())

                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        TxtNama.Text = rd("NAMA_BARANG").ToString()
                    End If
                End Using
            End Using
        End If
    End Sub

    Private Sub AmbilData()
        ' Ambil data MutasiBarang
        Dim queryMutasiBarang As String = "SELECT FAKTUR, TANGGAL, JENIS, LOKASI, QTY_MASUK, QTY_KELUAR, SALDO, ID_USER FROM Temp_Mutasi_Barang"
        Using cmdMutasiBarang As New MySqlCommand(queryMutasiBarang, conn)
            Using rd As MySqlDataReader = cmdMutasiBarang.ExecuteReader()
                Using datasetMutasiBarang As New DataSet() ' Menggunakan DataSet standar
                    datasetMutasiBarang.Load(rd, LoadOption.OverwriteChanges, "Temp_Mutasi_Barang")
                    If ReportViewer1.LocalReport.DataSources.Count > 0 Then
                        ReportViewer1.LocalReport.DataSources.Clear() ' Bersihkan DataSources sebelumnya
                    End If

                    ' Menambahkan parameter ke laporan RDLC
                    Dim parameters As New ReportParameterCollection From {
                        New ReportParameter("NAMATOKO", NAMA_PERUSAHAAN),
                        New ReportParameter("Kode", TxtKode.Text),
                        New ReportParameter("Nama_Barang", TxtNama.Text),
                        New ReportParameter("Tanggal", "Tanggal : " & DateTimePicker1.Value.ToShortDateString() & " s/d " & DateTimePicker2.Value.ToShortDateString())
                    }

                    ' Menetapkan dataset ke laporan RDLC
                    ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", datasetMutasiBarang.Tables("Temp_Mutasi_Barang")))
                    ReportViewer1.LocalReport.SetParameters(parameters)

                    ' Menampilkan laporan RDLC
                    ReportViewer1.RefreshReport()
                End Using
            End Using
        End Using
    End Sub

    Private Sub BtnPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPreview.Click
        ReportViewer1.LocalReport.DataSources.Clear()
        AmbilDanSimpanDataMutasiBarang()
        AmbilData()
    End Sub

    Private Sub FormLapMutasiBarang_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.F8
                BtnPreview.PerformClick()
            Case Keys.Escape
                Me.Close()
        End Select
    End Sub
End Class