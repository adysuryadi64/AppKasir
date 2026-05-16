Imports System.IO
Imports iTextSharp.text


Public Class FormPerbaikanDatabase
    Private Sub FormPerbaikanDatabase_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ModuleTheme.TerapkanTheme(Me)
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
                    "UPDATE tbl_Armada SET KODE = TRIM(KODE), NOPOL = TRIM(NOPOL), JENIS = TRIM(JENIS)",
                    "UPDATE tbl_barang SET NAMA_BARANG = TRIM(NAMA_BARANG), ID_BARANG = TRIM(ID_BARANG), BARCODE_KECIL = TRIM(BARCODE_KECIL), BARCODE_SEDANG = TRIM(BARCODE_SEDANG), BARCODE_BESAR = TRIM(BARCODE_BESAR), NAMA_KATEGORI = TRIM(NAMA_KATEGORI), KODE_KATEGORI = TRIM(KODE_KATEGORI), NAMA_MERK = TRIM(NAMA_MERK), KODE_MERK = TRIM(KODE_MERK), NAMA_SUPLIYER = TRIM(NAMA_SUPLIYER), KODE_SUPLIYER = TRIM(KODE_SUPLIYER), SATUAN_UMUM_KECIL = TRIM(SATUAN_UMUM_KECIL), SATUAN_UMUM_SEDANG = TRIM(SATUAN_UMUM_SEDANG), SATUAN_UMUM_BESAR = TRIM(SATUAN_UMUM_BESAR), SATUAN_PARTAI_KECIL = TRIM(SATUAN_PARTAI_KECIL), SATUAN_PARTAI_SEDANG = TRIM(SATUAN_PARTAI_SEDANG), SATUAN_PARTAI_BESAR = TRIM(SATUAN_PARTAI_BESAR), SATUAN_STOK = TRIM(SATUAN_STOK)",
                    "UPDATE tbl_cabang SET kode_cabang = TRIM(kode_cabang), nama_cabang = TRIM(nama_cabang), alamat = TRIM(alamat), kota = TRIM(kota), hp = TRIM(hp), pemilik = TRIM(pemilik)",
                    "UPDATE tbl_datareferensi SET Kode_akun = TRIM(Kode_akun), Nama_Akun = TRIM(Nama_Akun), TYPE_AKUN = TRIM(TYPE_AKUN), JENIS_AKUN = TRIM(JENIS_AKUN), SUB_AKUN = TRIM(SUB_AKUN), AKUN_DK = TRIM(AKUN_DK), AKUN_NRLR = TRIM(AKUN_NRLR)",
                    "UPDATE tbl_karyawan SET Kode = TRIM(Kode), Nama = TRIM(Nama), Jabatan = TRIM(Jabatan)",
                    "UPDATE tbl_kategori SET kode = TRIM(kode), nama = TRIM(nama), jenis = TRIM(jenis)",
                    "UPDATE tbl_merk SET kode = TRIM(kode), nama = TRIM(nama), keterangan = TRIM(keterangan)",
                    "UPDATE tbl_pelanggan SET KODE = TRIM(KODE), NAMA = TRIM(NAMA), ALAMAT = TRIM(ALAMAT), NO_TELP = TRIM(NO_TELP), JENIS = TRIM(JENIS)",
                    "UPDATE tbl_satuan SET kode = TRIM(kode), nama = TRIM(nama)",
                    "UPDATE tbl_supliyer SET Kode = TRIM(Kode), Nama = TRIM(Nama), ALamat = TRIM(ALamat), Hp = TRIM(Hp)",
                    "UPDATE tbl_user SET kode_user = TRIM(kode_user), nama_user = TRIM(nama_user), user_name = TRIM(user_name), lvl = TRIM(lvl)"
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




    ''' <summary>Ambil semua nama tabel BASE TABLE dari database aktif via INFORMATION_SCHEMA</summary>
    Private Function GetSemuaTabel() As List(Of String)
        Dim list As New List(Of String)
        Dim query As String = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME"
        Using cmd As New MySqlCommand(query, conn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    list.Add(reader("TABLE_NAME").ToString())
                End While
            End Using
        End Using
        Return list
    End Function

    Private Sub BtnAnalyze_Click(sender As Object, e As EventArgs) Handles BtnAnalyze.Click
        BtnCetak.Visible = False
        BtnSimpanPDF.Visible = False
        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()
        ListBoxResults.Items.Add("⏳ Menjalankan ANALYZE TABLE... harap tunggu")
        Application.DoEvents()

        Try
            Dim analyzeConn As New MySqlConnection(_connectionString)
            analyzeConn.Open()
            Dim tables As List(Of String) = GetSemuaTabel()
            If tables.Count = 0 Then
                ListBoxResults.Items.Add("Tidak ada tabel ditemukan.")
                Return
            End If

            ListBoxResults.Items(0) = $"⏳ ANALYZE {tables.Count} tabel... harap tunggu"
            Application.DoEvents()

            Dim tableList As String = String.Join(",", tables.Select(Function(t) $"`{t}`"))
            Dim query As String = $"ANALYZE TABLE {tableList}"
            Using cmd As New MySqlCommand(query, analyzeConn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    ListBoxResults.Items.Clear()
                    While reader.Read()
                        Dim result As String = $"{reader("Table")}: {reader("Op")} - {reader("Msg_type")} - {reader("Msg_text")}"
                        ListBoxResults.Items.Add(result)
                        ListBoxResults.TopIndex = ListBoxResults.Items.Count - 1
                        Application.DoEvents()
                    End While
                End Using
            End Using

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
        ListBoxResults.Items.Add("⏳ Menjalankan CHECK TABLE... harap tunggu")
        Application.DoEvents()

        Try
            Dim analyzeConn As New MySqlConnection(_connectionString)
            analyzeConn.Open()
            Dim tables As List(Of String) = GetSemuaTabel()
            If tables.Count = 0 Then
                ListBoxResults.Items.Add("Tidak ada tabel ditemukan.")
                Return
            End If

            ListBoxResults.Items(0) = $"⏳ CHECK {tables.Count} tabel... harap tunggu"
            Application.DoEvents()

            Dim tableList As String = String.Join(",", tables.Select(Function(t) $"`{t}`"))
            Dim query As String = $"CHECK TABLE {tableList}"
            Using cmd As New MySqlCommand(query, analyzeConn)
                cmd.CommandTimeout = 120
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    ListBoxResults.Items.Clear()
                    While reader.Read()
                        Dim result As String = $"{reader("Table")} | {reader("Op")} | {reader("Msg_type")} | {reader("Msg_text")}"
                        ListBoxResults.Items.Add(result)
                        ListBoxResults.TopIndex = ListBoxResults.Items.Count - 1
                        Application.DoEvents()
                    End While
                End Using
            End Using

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
        ListBoxResults.Items.Add("⏳ Menjalankan CHECKSUM TABLE... harap tunggu")
        Application.DoEvents()

        Try
            Dim analyzeConn As New MySqlConnection(_connectionString)
            analyzeConn.Open()
            Dim tables As List(Of String) = GetSemuaTabel()
            If tables.Count = 0 Then
                ListBoxResults.Items.Add("Tidak ada tabel ditemukan.")
                Return
            End If

            ListBoxResults.Items(0) = $"⏳ CHECKSUM {tables.Count} tabel... harap tunggu"
            Application.DoEvents()

            Dim tableList As String = String.Join(",", tables.Select(Function(t) $"`{t}`"))
            Dim query As String = $"CHECKSUM TABLE {tableList}"
            Using cmd As New MySqlCommand(query, analyzeConn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    ListBoxResults.Items.Clear()
                    While reader.Read()
                        Dim result As String = $"{reader("Table")} | Checksum: {reader("Checksum")}"
                        ListBoxResults.Items.Add(result)
                        ListBoxResults.TopIndex = ListBoxResults.Items.Count - 1
                        Application.DoEvents()
                    End While
                End Using
            End Using

        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub BtnConvertUtf8_Click(sender As Object, e As EventArgs) Handles BtnConvertUtf8.Click
        BtnCetak.Visible = False
        BtnSimpanPDF.Visible = False

        Dim konfirmasi As DialogResult = MessageBox.Show(
            "Operasi ini akan mengubah character set dan collation SEMUA tabel ke utf8mb4_unicode_ci." & Environment.NewLine &
            "Proses ini tidak dapat dibatalkan. Lanjutkan?",
            "Konfirmasi Convert utf8mb4",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If konfirmasi <> DialogResult.Yes Then Return

        Cursor = Cursors.WaitCursor
        ListBoxResults.Items.Clear()

        Dim convertConn As MySqlConnection = Nothing
        Try
            convertConn = New MySqlConnection(_connectionString)
            convertConn.Open()

            ' Konversi database itu sendiri dulu
            Dim dbQuery As String = "ALTER DATABASE CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci"
            Using cmd As New MySqlCommand(dbQuery, convertConn)
                cmd.ExecuteNonQuery()
                ListBoxResults.Items.Add("DATABASE: ALTER CHARACTER SET utf8mb4 - OK")
            End Using

            ' Konversi tiap tabel
            Dim tables As List(Of String) = GetSemuaTabel()
            Dim berhasil As Integer = 0
            Dim gagal As Integer = 0

            For Each tbl As String In tables
                Try
                    Dim q As String = $"ALTER TABLE `{tbl}` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci"
                    ListBoxResults.Items.Add($"⏳ {tbl}: sedang diproses...")
                    ListBoxResults.TopIndex = ListBoxResults.Items.Count - 1
                    Application.DoEvents()

                    Using cmd As New MySqlCommand(q, convertConn)
                        cmd.ExecuteNonQuery()
                    End Using

                    ListBoxResults.Items(ListBoxResults.Items.Count - 1) = $"✓ {tbl}: OK"
                    ListBoxResults.TopIndex = ListBoxResults.Items.Count - 1
                    berhasil += 1
                Catch exTbl As Exception
                    If ListBoxResults.Items.Count > 0 AndAlso
                       ListBoxResults.Items(ListBoxResults.Items.Count - 1).ToString().StartsWith("⏳") Then
                        ListBoxResults.Items(ListBoxResults.Items.Count - 1) = $"✗ {tbl}: GAGAL — {exTbl.Message}"
                    Else
                        ListBoxResults.Items.Add($"✗ {tbl}: GAGAL — {exTbl.Message}")
                    End If
                    ListBoxResults.TopIndex = ListBoxResults.Items.Count - 1
                    gagal += 1
                End Try
                Application.DoEvents()
            Next

            ListBoxResults.Items.Add("")
            ListBoxResults.Items.Add($"Selesai: {berhasil} tabel berhasil, {gagal} gagal.")

        Catch ex As Exception
            ListBoxResults.Items.Add($"Kesalahan: {ex.Message}")
        Finally
            convertConn?.Close()
            convertConn?.Dispose()
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

