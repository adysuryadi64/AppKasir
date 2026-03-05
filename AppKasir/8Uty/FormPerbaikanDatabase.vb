Imports System.IO
Imports iTextSharp.text


Public Class FormPerbaikanDatabase
    Private Sub FormPerbaikanDatabase_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BtnCetak.Visible = False
        BtnSimpanPDF.Visible = False

        ListBoxResults.Items.Clear()
    End Sub

    Private Sub BtnCleanup_Click(sender As Object, e As EventArgs) Handles BtnCleanup.Click
        BtnCetak.Visible = False
        BtnSimpanPDF.Visible = False

        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()

        Dim transaction As MySqlTransaction = Nothing

        Try
            transaction = conn.BeginTransaction()

            Dim operations As New List(Of String) From {
                    "UPDATE tbl_Armada SET KODE = TRIM(KODE), NOPOL = TRIM(NOPOL)",
                    "UPDATE tbl_barang SET NAMA_BARANG = TRIM(NAMA_BARANG), ID_BARANG = TRIM(ID_BARANG), BARCODE_KECIL = TRIM(BARCODE_KECIL), BARCODE_SEDANG = TRIM(BARCODE_SEDANG), BARCODE_BESAR = TRIM(BARCODE_BESAR)",
                    "UPDATE tbl_datareferensi SET Kode_akun = TRIM(Kode_akun), Nama_Akun = TRIM(Nama_Akun)",
                    "UPDATE tbl_karyawan SET Kode = TRIM(Kode), Nama = TRIM(Nama)",
                    "UPDATE tbl_kategori SET kode = TRIM(kode), nama = TRIM(nama)",
                    "UPDATE tbl_pelanggan SET KODE = TRIM(KODE), NAMA = TRIM(NAMA)",
                    "UPDATE tbl_satuan SET kode = TRIM(kode), nama = TRIM(nama)",
                    "UPDATE tbl_supliyer SET Nama = TRIM(Nama), ALamat = TRIM(ALamat)",
                    "UPDATE tbl_user SET nama_user = TRIM(nama_user), user_name = TRIM(user_name)"
                }

            ' Jalankan operasi perbaruan
            For Each query As String In operations
                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.ExecuteNonQuery()
                    ListBoxResults.Items.Add($"Query berhasil: {query}")
                End Using
            Next

            ' Commit jika semua berhasil
            transaction.Commit()
            ListBoxResults.Items.Add("Semua operasi selesai berhasil!")

        Catch ex As Exception
            transaction?.Rollback()
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")

        Finally
            Cursor = Cursors.Default
        End Try
    End Sub


    Private Sub BtnDuplikat_Click(sender As Object, e As EventArgs) Handles BtnDuplikat.Click
        BtnCetak.Visible = True
        BtnSimpanPDF.Visible = True

        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()

        Try
            Dim duplikatList As List(Of String) = CekDuplikatBarang()

            If duplikatList.Count > 0 Then
                For Each item In duplikatList
                    ListBoxResults.Items.Add(item)
                Next
            Else
                ListBoxResults.Items.Add("Tidak ditemukan data duplikat pada ID_BARANG atau NAMA_BARANG.")
            End If

        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")

        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Function CekDuplikatBarang() As List(Of String)
        Dim duplikatList As New List(Of String)
        Dim nomor As Integer = 1
        Dim adaDuplikatKode As Boolean = False
        Dim adaDuplikatNama As Boolean = False

        ' Tambahkan header untuk duplikat KODE
        duplikatList.Add("== Duplikat KODE ==")

        ' Cek duplikat ID_BARANG
        Dim queryDuplikatID As String = "SELECT ID_BARANG, COUNT(*) AS JumlahDuplikat FROM tbl_barang GROUP BY ID_BARANG HAVING COUNT(*) > 1"
        Using cmdID As New MySqlCommand(queryDuplikatID, conn)
            Using reader As MySqlDataReader = cmdID.ExecuteReader()
                While reader.Read()
                    Dim idBarang As String = reader("ID_BARANG").ToString()
                    Dim jumlah As Integer = Convert.ToInt32(reader("JumlahDuplikat"))
                    duplikatList.Add($"{nomor}. {idBarang}, Jumlah: {jumlah}")
                    nomor += 1
                    adaDuplikatKode = True
                End While
            End Using
        End Using

        ' Jika tidak ada duplikat kode, tambahkan keterangan
        If Not adaDuplikatKode Then
            duplikatList.Add("Tidak ada duplikat KODE barang.")
        End If

        ' Tambahkan header untuk duplikat NAMA
        duplikatList.Add(Environment.NewLine & "== Duplikat NAMA ==")

        ' Cek duplikat NAMA_BARANG
        Dim queryDuplikatNama As String = "SELECT NAMA_BARANG, COUNT(*) AS JumlahDuplikat FROM tbl_barang GROUP BY NAMA_BARANG HAVING COUNT(*) > 1"
        Using cmdNama As New MySqlCommand(queryDuplikatNama, conn)
            Using reader As MySqlDataReader = cmdNama.ExecuteReader()
                While reader.Read()
                    Dim namaBarang As String = reader("NAMA_BARANG").ToString()
                    Dim jumlah As Integer = Convert.ToInt32(reader("JumlahDuplikat"))
                    duplikatList.Add($"{nomor}. {namaBarang}, Jumlah: {jumlah}")
                    nomor += 1
                    adaDuplikatNama = True
                End While
            End Using
        End Using

        ' Jika tidak ada duplikat nama, tambahkan keterangan
        If Not adaDuplikatNama Then
            duplikatList.Add("Tidak ada duplikat NAMA barang.")
        End If

        Return duplikatList
    End Function




    Private Sub BtnAnalyze_Click(sender As Object, e As EventArgs) Handles BtnAnalyze.Click
        BtnCetak.Visible = False
        BtnSimpanPDF.Visible = False

        Cursor = Cursors.WaitCursor
        ' Daftar tabel yang akan dianalisis
        Dim tables As String() = {
            "bon_karyawan", "gaji_karyawan", "hakaksesuser", "history", "historybarang", "hutang", "hutang_detail", "jurnalumum",
            "pembelian", "pembelian_detail", "pembelian_ditahan", "pembelian_ditahan_detail", "penjualan", "penjualan_detail",
            "penjualan_ditahan", "penjualan_ditahan_detail", "piutang", "piutang_detail", "retur_pembelian", "retur_pembelian_detail",
            "retur_penjualan", "retur_penjualan_detail", "stoktambahkurang", "stok_opname", "surat_jalan", "surat_jalan_detail",
            "tbl_armada", "tbl_barang", "tbl_datareferensi", "tbl_gaji", "tbl_karyawan", "tbl_kategori", "tbl_merk", "tbl_pelanggan",
            "tbl_perusahaan", "tbl_satuan", "tbl_supliyer", "tbl_user", "tempbukubesarpembantu", "tempjurnalumum", "temp_bon_karyawan",
            "temp_datareferensi", "temp_jurnal", "temp_labarugi", "temp_loading", "temp_mutasi_barang", "temp_supliyerbayar",
            "temp_supliyerhutang", "transfer_barang", "transfer_barang_detail", "transfer_stok", "tukarbarang"
        }

        Try
            ListBoxResults.Items.Clear()

            For Each tableName As String In tables
                Dim query As String = $"ANALYZE TABLE `{tableName}`;"
                Using command As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            Dim table As String = reader("Table").ToString()
                            Dim operation As String = reader("Op").ToString()
                            Dim messageType As String = reader("Msg_type").ToString()
                            Dim messageText As String = reader("Msg_text").ToString()

                            ' Format hasil untuk ditampilkan di ListBox
                            Dim result As String = $"{table}: {operation} - {messageType} - {messageText}"
                            ListBoxResults.Items.Add(result)
                        End While
                    End Using
                End Using
            Next

        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub BtnCheckTables_Click(sender As Object, e As EventArgs) Handles BtnCheckTables.Click
        BtnCetak.Visible = False
        BtnSimpanPDF.Visible = False


        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()

        Try

            Dim tables As String() = {
                    "bon_karyawan", "gaji_karyawan", "hakaksesuser", "history", "historybarang", "hutang", "hutang_detail",
                    "jurnalumum", "pembelian", "pembelian_detail", "pembelian_ditahan", "pembelian_ditahan_detail",
                    "penjualan", "penjualan_detail", "penjualan_ditahan", "penjualan_ditahan_detail", "piutang",
                    "piutang_detail", "retur_pembelian", "retur_pembelian_detail", "retur_penjualan", "retur_penjualan_detail",
                    "stoktambahkurang", "stok_opname", "surat_jalan", "surat_jalan_detail", "tbl_armada", "tbl_barang",
                    "tbl_datareferensi", "tbl_gaji", "tbl_karyawan", "tbl_kategori", "tbl_merk", "tbl_pelanggan",
                    "tbl_perusahaan", "tbl_satuan", "tbl_supliyer", "tbl_user", "tempbukubesarpembantu", "tempjurnalumum",
                    "temp_bon_karyawan", "temp_datareferensi", "temp_jurnal", "temp_labarugi", "temp_loading",
                    "temp_mutasi_barang", "temp_supliyerbayar", "temp_supliyerhutang", "transfer_barang", "transfer_barang_detail",
                    "transfer_stok", "tukarbarang"
                }

            For Each table As String In tables
                Dim query As String = $"CHECK TABLE `{table}`"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim tableName As String = reader("Table").ToString()
                            Dim operation As String = reader("Op").ToString()
                            Dim messageType As String = reader("Msg_type").ToString()
                            Dim messageText As String = reader("Msg_text").ToString()

                            Dim result As String = $"{tableName} | {operation} | {messageType} | {messageText}"
                            ListBoxResults.Items.Add(result)
                        End While
                    End Using
                End Using
            Next


        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub BtnChecksumTables_Click(sender As Object, e As EventArgs) Handles BtnChecksumTables.Click
        BtnCetak.Visible = False
        BtnSimpanPDF.Visible = False


        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()

        Try

            Dim tables As String() = {
                    "bon_karyawan", "gaji_karyawan", "hakaksesuser", "history", "historybarang", "hutang", "hutang_detail",
                    "jurnalumum", "pembelian", "pembelian_detail", "pembelian_ditahan", "pembelian_ditahan_detail",
                    "penjualan", "penjualan_detail", "penjualan_ditahan", "penjualan_ditahan_detail", "piutang",
                    "piutang_detail", "retur_pembelian", "retur_pembelian_detail", "retur_penjualan", "retur_penjualan_detail",
                    "stoktambahkurang", "stok_opname", "surat_jalan", "surat_jalan_detail", "tbl_armada", "tbl_barang",
                    "tbl_datareferensi", "tbl_gaji", "tbl_karyawan", "tbl_kategori", "tbl_merk", "tbl_pelanggan",
                    "tbl_perusahaan", "tbl_satuan", "tbl_supliyer", "tbl_user", "tempbukubesarpembantu", "tempjurnalumum",
                    "temp_bon_karyawan", "temp_datareferensi", "temp_jurnal", "temp_labarugi", "temp_loading",
                    "temp_mutasi_barang", "temp_supliyerbayar", "temp_supliyerhutang", "transfer_barang", "transfer_barang_detail",
                    "transfer_stok", "tukarbarang"
                }

            For Each table As String In tables
                Dim query As String = $"CHECKSUM TABLE `{table}`"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim tableName As String = reader("Table").ToString()
                            Dim checksum As String = reader("Checksum").ToString()

                            Dim result As String = $"{tableName} | Checksum: {checksum}"
                            ListBoxResults.Items.Add(result)
                        End While
                    End Using
                End Using
            Next


        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private currentPrintIndex As Integer = 0
    Private printItems As New List(Of String)

    Private Sub BtnCetak_Click(sender As Object, e As EventArgs) Handles BtnCetak.Click
        ' Cek apakah ListBoxResults memiliki item
        If ListBoxResults.Items.Count = 0 Then
            MessageBox.Show("Tidak ada data untuk dicetak.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Simpan item ke dalam list untuk dicetak
        printItems = ListBoxResults.Items.Cast(Of String)().ToList()
        currentPrintIndex = 0

        ' Siapkan dokumen cetak
        Dim printDoc As New Printing.PrintDocument()
        AddHandler printDoc.PrintPage, AddressOf PrintPageHandler

        ' Tampilkan preview
        Dim preview As New PrintPreviewDialog()
        preview.Document = printDoc
        preview.ShowDialog()
    End Sub

    Private Sub PrintPageHandler(sender As Object, ev As Printing.PrintPageEventArgs)
        Dim font As New System.Drawing.Font("Arial", 10)
        Dim lineHeight As Single = font.GetHeight(ev.Graphics)
        Dim leftMargin As Single = ev.MarginBounds.Left
        Dim topMargin As Single = ev.MarginBounds.Top
        Dim yPosition As Single = topMargin
        Dim itemsPerPage As Integer = CInt(Math.Floor(ev.MarginBounds.Height / lineHeight))

        Dim i As Integer
        For i = 0 To itemsPerPage - 1
            If currentPrintIndex >= printItems.Count Then Exit For

            Dim line As String = printItems(currentPrintIndex)
            ev.Graphics.DrawString(line, font, System.Drawing.Brushes.Black, leftMargin, yPosition)
            yPosition += lineHeight
            currentPrintIndex += 1
        Next

        ev.HasMorePages = (currentPrintIndex < printItems.Count)
    End Sub



    Private Sub SimpanListBoxKePDF(filePath As String)
        If ListBoxResults.Items.Count = 0 Then
            MessageBox.Show("Tidak ada data untuk disimpan.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' Buat dokumen PDF baru
            Dim doc As New iTextSharp.text.Document(PageSize.A4, 40, 40, 40, 40)
            iTextSharp.text.pdf.PdfWriter.GetInstance(doc, New FileStream(filePath, FileMode.Create))
            doc.Open()

            ' Buat font PDF
            Dim bf As iTextSharp.text.pdf.BaseFont = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED)
            Dim titleFont As New iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.BOLD)
            Dim normalFont As New iTextSharp.text.Font(bf, 10)

            ' Tambahkan judul
            doc.Add(New iTextSharp.text.Paragraph("Daftar Hasil Duplikat Kode dan Nama Barang", titleFont))
            doc.Add(New iTextSharp.text.Paragraph("Tanggal: " & DateTime.Now.ToString("dd MMMM yyyy"), normalFont))
            doc.Add(New iTextSharp.text.Paragraph(Environment.NewLine))

            ' Tambahkan item dari ListBox
            For Each item As String In ListBoxResults.Items
                doc.Add(New iTextSharp.text.Paragraph(item, normalFont))
            Next

            doc.Close()
            MessageBox.Show("Data berhasil disimpan ke PDF.", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Gagal menyimpan PDF: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub BtnSimpanPDF_Click(sender As Object, e As EventArgs) Handles BtnSimpanPDF.Click
        Dim sfd As New SaveFileDialog()
        sfd.Filter = "PDF File|*.pdf"
        sfd.Title = "Simpan hasil ke PDF"
        sfd.FileName = "LaporandataDuplikat.pdf"

        If sfd.ShowDialog() = DialogResult.OK Then
            SimpanListBoxKePDF(sfd.FileName)
        End If
    End Sub



End Class