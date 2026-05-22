' ================================================================
' ModuleAuditTrail — Pencatatan audit trail edit & hapus transaksi
' dan data master AppKasir.
'
' Prinsip utama:
'   - Kegagalan audit TIDAK boleh memblokir operasi transaksi utama
'   - Semua exception ditangkap di level paling atas, dicatat ke log
'   - Nilai numerik selalu via ModuleAngka.ParseDecimal
'
' Format kolom `ket` (plain text):
'   "[KRITIS] Hapus penjualan | PJ-001 | 2026-04-20 | Rp 1.500.000 | Budi | Lunas"
'   Tujuan: jejak audit siapa mengubah apa kapan — BUKAN untuk restore data.
'   Tidak ada kompresi, tidak ada JSON, langsung bisa dibaca di FormAuditTrail.
' ================================================================
Module ModuleAuditTrail

    ' ────────────────────────────────────────────────────────────
    ' PROSEDUR PUBLIK
    ' ────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Catat audit untuk operasi pada transaksi (snapshot diambil internal dari DB).
    ''' Dipanggil SEBELUM operasi DELETE/UPDATE dieksekusi.
    ''' Tidak pernah throw exception — kegagalan dicatat ke log dan diabaikan.
    ''' </summary>
    Public Sub CatatAudit(noFaktur As String,
                          jenisAksi As String,
                          jenisTransaksi As String,
                          Optional ket As String = "",
                          Optional trans As MySqlTransaction = Nothing)
        If String.IsNullOrWhiteSpace(noFaktur) Then Exit Sub

        Try
            Dim snapshot As String = AmbilSnapshotTransaksi(noFaktur, jenisTransaksi)

            ' Gabungkan keterangan + snapshot menjadi satu plain text
            Dim ketFinal As String
            If String.IsNullOrEmpty(snapshot) Then
                ketFinal = If(String.IsNullOrEmpty(ket), "Data tidak ditemukan saat snapshot",
                              ket & Environment.NewLine & "Data tidak ditemukan saat snapshot")
            Else
                ketFinal = If(String.IsNullOrEmpty(ket), snapshot,
                              ket & Environment.NewLine & snapshot)
            End If

            InsertAuditRecord(noFaktur, jenisAksi, jenisTransaksi, ketFinal, trans)

        Catch ex As Exception
            TulisLogError("CatatAudit gagal [" & jenisAksi & "/" & noFaktur & "]: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Catat audit untuk form master (keterangan disiapkan oleh pemanggil).
    ''' Pemanggil bertanggung jawab menyiapkan isi ket sebelum memanggil prosedur ini.
    ''' </summary>
    Public Sub CatatAuditMaster(identifier As String,
                                jenisAksi As String,
                                jenisTransaksi As String,
                                snapshotTeks As String,
                                Optional ket As String = "",
                                Optional trans As MySqlTransaction = Nothing)
        If String.IsNullOrWhiteSpace(identifier) Then Exit Sub

        Try
            Dim ketFinal As String = If(String.IsNullOrEmpty(ket), snapshotTeks,
                                        If(String.IsNullOrEmpty(snapshotTeks), ket,
                                           ket & Environment.NewLine & snapshotTeks))

            InsertAuditRecord(identifier, jenisAksi, jenisTransaksi, ketFinal, trans)

        Catch ex As Exception
            TulisLogError("CatatAuditMaster gagal [" & jenisAksi & "/" & identifier & "]: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Jalankan arsip otomatis jika belum dijalankan hari ini.
    ''' Dipanggil di FormUtama_Load untuk user level Admin/Owner.
    ''' </summary>
    Public Sub JalankanArsipJikaPerlu()
        Try
            Dim tanggalTerakhir As String = ""
            Using cmd As New MySqlCommand(
                "SELECT config_value FROM tbl_audit_config " &
                "WHERE config_key = 'AuditArsipTerakhir' LIMIT 1", conn)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    tanggalTerakhir = result.ToString()
                End If
            End Using

            If tanggalTerakhir = DateTime.Today.ToString("yyyy-MM-dd") Then Exit Sub

            Dim retensiBulan As Integer = BacaRetensiBulan()
            Dim batasWaktu As DateTime = DateTime.Now.AddMonths(-retensiBulan)

            Dim arsipTrans As MySqlTransaction = conn.BeginTransaction()
            Try
                Using cmd As New MySqlCommand(
                    "INSERT INTO tbl_audit_trail_arsip " &
                    "SELECT * FROM tbl_audit_trail WHERE waktu_aksi < @batas",
                    conn, arsipTrans)
                    cmd.Parameters.AddWithValue("@batas", batasWaktu)
                    cmd.ExecuteNonQuery()
                End Using

                Using cmd As New MySqlCommand(
                    "DELETE FROM tbl_audit_trail WHERE waktu_aksi < @batas",
                    conn, arsipTrans)
                    cmd.Parameters.AddWithValue("@batas", batasWaktu)
                    cmd.ExecuteNonQuery()
                End Using

                arsipTrans.Commit()

                ' Catat tanggal arsip terakhir ke tbl_audit_config (bukan hakaksesuser)
                Using cmd As New MySqlCommand(
                    "UPDATE tbl_audit_config SET config_value = @tgl " &
                    "WHERE config_key = 'AuditArsipTerakhir'", conn)
                    cmd.Parameters.AddWithValue("@tgl", DateTime.Today.ToString("yyyy-MM-dd"))
                    cmd.ExecuteNonQuery()
                End Using

            Catch ex As Exception
                arsipTrans.Rollback()
                TulisLogError("JalankanArsipJikaPerlu (arsip) gagal: " & ex.Message)
            End Try

        Catch ex As Exception
            TulisLogError("JalankanArsipJikaPerlu gagal: " & ex.Message)
        End Try
    End Sub

    ' ────────────────────────────────────────────────────────────
    ' HELPER INTERNAL
    ' ────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Ambil snapshot lengkap transaksi dari DB sebagai plain text.
    ''' Format header: "faktur | tgl | Rp nominal | nama | status | oleh:user"
    ''' Format detail: "+ NamaBarang | qty sat | Rp harga | Rp total"
    ''' Kembalikan "" jika tidak ditemukan atau terjadi exception.
    ''' </summary>
    Private Function AmbilSnapshotTransaksi(noFaktur As String,
                                            jenisTransaksi As String) As String
        Try
            Dim sqlHeader As String = ""
            Dim sqlDetail As String = ""

            Select Case jenisTransaksi
                Case "Penjualan"
                    sqlHeader = "SELECT ID_PENJUALAN, TGL_TRANSAKSI, GRAND_TOTAL_STL_PAJAK, " &
                                "NAMA_PELANGGAN, STATUS_TRANSAKSI, ID_USER " &
                                "FROM penjualan WHERE ID_PENJUALAN = @fk LIMIT 1"
                    sqlDetail = "SELECT NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, TOTAL_HARGA " &
                                "FROM penjualan_detail WHERE FAKTUR_JUAL = @fk"
                Case "Pembelian"
                    sqlHeader = "SELECT ID_PEMBELIAN, TGL_BELI, GRAND_TOTAL_BELI, " &
                                "NAMA_SUPLIYER, STATUS_TRANSAKSI_BELI, ID_USER " &
                                "FROM pembelian WHERE ID_PEMBELIAN = @fk LIMIT 1"
                    sqlDetail = "SELECT NAMA_BARANG, QTY, SATUAN, HARGA_BELI, TOTAL " &
                                "FROM pembelian_detail WHERE FAKTUR_BELI = @fk"
                Case "Retur Penjualan"
                    sqlHeader = "SELECT ID_RETUR_PENJUALAN, TGL_RETUR_JUAL, TOTAL_RUPIAH, " &
                                "NAMA_PELANGGAN, STATUS_PENJUALAN, ID_USER " &
                                "FROM retur_penjualan WHERE ID_RETUR_PENJUALAN = @fk LIMIT 1"
                    sqlDetail = "SELECT NAMA_BARANG, QTY, SATUAN, HARGA_JUAL, TOTAL_HARGA " &
                                "FROM retur_penjualan_detail WHERE ID_RETUR_PENJUALAN = @fk"
                Case "Retur Pembelian"
                    sqlHeader = "SELECT ID_RETUR_PEMBELIAN, TGL_RETUR_BELI, TOTAL_RUPIAH, " &
                "NAMA_SUPPLIER, '' AS STATUS, ID_USER " &
                "FROM retur_pembelian WHERE ID_RETUR_PEMBELIAN = @fk LIMIT 1"
                    sqlDetail = "SELECT NAMA_BARANG, QTY, SATUAN, HARGA_BELI, TOTAL " &
                "FROM retur_pembelian_detail WHERE ID_RETUR_PEMBELIAN = @fk"
                Case "Bayar Hutang"
                    sqlHeader = "SELECT NOBAYARHUTANG, TGLPEMBAYARAN, NOMINALBAYAR, " &
                                "NAMASUPLIYER, '' AS STATUS, ID_USER_BAYAR " &
                                "FROM hutang WHERE NOBAYARHUTANG = @fk LIMIT 1"
                Case "Bayar Piutang"
                    sqlHeader = "SELECT ID_BAYAR_PIUTANG, TGL_BAYAR, NOMINAL_BAYAR, " &
                                "NAMA_PELANGGAN, '' AS STATUS, ID_USER_BAYAR " &
                                "FROM Piutang WHERE ID_BAYAR_PIUTANG = @fk LIMIT 1"
                Case Else
                    Return ""
            End Select

            Dim sb As New System.Text.StringBuilder()

            ' ── Header ──────────────────────────────────────────
            Using cmd As New MySqlCommand(sqlHeader, conn)
                cmd.Parameters.AddWithValue("@fk", noFaktur)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim fk As String = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(0), "")
                        Dim tgl As String = If(rd.IsDBNull(1), "", Convert.ToDateTime(rd(1)).ToString("yyyy-MM-dd HH:mm"))
                        Dim tot As Decimal = ModuleAngka.ParseDecimal(rd(2))
                        Dim pel As String = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(3), "")
                        Dim sts As String = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(4), "")
                        Dim usr As String = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(5), "")

                        sb.AppendLine(fk & " | " & tgl & " | Rp " &
                                      ModuleAngka.FormatRupiah(tot) & " | " &
                                      pel & " | " & sts & " | oleh:" & usr)
                    Else
                        Return ""
                    End If
                End Using
            End Using

            ' ── Detail item (jika ada query detail) ─────────────
            If Not String.IsNullOrEmpty(sqlDetail) Then
                Using cmd As New MySqlCommand(sqlDetail, conn)
                    cmd.Parameters.AddWithValue("@fk", noFaktur)
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        Dim no As Integer = 0
                        While rd.Read()
                            no += 1
                            Dim nama As String = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(0), "")
                            Dim qty As Decimal = ModuleAngka.ParseDecimal(rd(1))
                            Dim sat As String = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(2), "")
                            Dim harga As Decimal = ModuleAngka.ParseDecimal(rd(3))
                            Dim total As Decimal = ModuleAngka.ParseDecimal(rd(4))

                            sb.AppendLine($"  {no}. {nama} | {qty} {sat} | Rp {ModuleAngka.FormatRupiah(harga)} | Rp {ModuleAngka.FormatRupiah(total)}")
                        End While
                    End Using
                End Using
            End If

            Return sb.ToString().TrimEnd()

        Catch ex As Exception
            TulisLogError("AmbilSnapshotTransaksi gagal [" & jenisTransaksi & "/" & noFaktur & "]: " & ex.Message)
        End Try
        Return ""
    End Function

    ''' <summary>
    ''' INSERT satu record ke tbl_audit_trail.
    ''' Tidak ada kolom data_sebelum — semua info ada di kolom ket (TEXT).
    ''' </summary>
    Private Sub InsertAuditRecord(identifier As String,
                                  jenisAksi As String,
                                  jenisTransaksi As String,
                                  ket As String,
                                  trans As MySqlTransaction)
        Dim sql As String =
            "INSERT INTO tbl_audit_trail " &
            "(waktu_aksi, jenis_aksi, jenis_trans, identifier, id_user, lokasi, komputer, ket) " &
            "VALUES (@waktu, @aksi, @trans, @id, @user, @lok, @pc, @ket)"

        Using cmd As New MySqlCommand(sql, conn, trans)
            cmd.Parameters.AddWithValue("@waktu", DateTime.Now)
            cmd.Parameters.AddWithValue("@aksi", jenisAksi)
            cmd.Parameters.AddWithValue("@trans", jenisTransaksi)
            cmd.Parameters.AddWithValue("@id", identifier)
            cmd.Parameters.AddWithValue("@user", FormUtama.StatusNamaUser.Text)
            cmd.Parameters.AddWithValue("@lok",
                If(String.IsNullOrEmpty(FormUtama.StatusLokasi.Text),
                   CObj(DBNull.Value), CObj(FormUtama.StatusLokasi.Text)))
            cmd.Parameters.AddWithValue("@pc",
                If(String.IsNullOrEmpty(FormUtama.StatusNamaPC.Text),
                   CObj(DBNull.Value), CObj(FormUtama.StatusNamaPC.Text)))
            cmd.Parameters.AddWithValue("@ket",
                If(String.IsNullOrEmpty(ket), CObj(DBNull.Value), CObj(ket)))
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Function BacaRetensiBulan() As Integer
        Try
            Using cmd As New MySqlCommand(
                "SELECT config_value FROM tbl_audit_config " &
                "WHERE config_key = 'AuditRetensi' LIMIT 1", conn)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    Dim nilai As Integer = ModuleAngka.ParseInteger(result, 3)
                    Return If(nilai < 1, 1, nilai)
                End If
            End Using
        Catch ex As Exception
            TulisLogError("BacaRetensiBulan gagal: " & ex.Message)
        End Try
        Return 3
    End Function

    Private Sub TulisLogError(pesan As String)
        Try
            Using cmd As New MySqlCommand(
                "INSERT INTO History (TANGGAL, Aksi) VALUES (@tgl, @aksi)", conn)
                cmd.Parameters.AddWithValue("@tgl", DateTime.Now)
                Dim pesanPendek As String = If(pesan.Length > 500, pesan.Substring(0, 500), pesan)
                cmd.Parameters.AddWithValue("@aksi",
                    "[AUDIT_ERROR] " & FormUtama.StatusNamaUser.Text & " @ " &
                    FormUtama.StatusNamaPC.Text & " — " & pesanPendek)
                cmd.ExecuteNonQuery()
            End Using
        Catch
            ' Abaikan — jika log pun gagal, tidak ada yang bisa dilakukan
        End Try
    End Sub

End Module
