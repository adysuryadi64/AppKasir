# Transaction Audit Trail — Design Document

## 1. Overview

Dokumen ini menjelaskan desain implementasi fitur **Transaction Audit Trail** untuk AppKasir. Fitur ini bertujuan mencatat otomatis setiap operasi edit dan hapus pada transaksi serta data master, lengkap dengan detail HEADER + SEMUA ITEM-LINE dalam plain text.

### Prinsip Utama
- **Tidak ada kompresi, tidak ada JSON:** Semua informasi disimpan sebagai plain text di kolom `ket` (tipe TEXT) yang dapat dibaca langsung tanpa perlu decompress atau parsing.
- **Kegagalan audit tidak memblokir transaksi utama:** Exception di level audit dicatat ke `History` saja, operasi hapus/edit tetap berjalan.
- **Konsistensi transaksional:** Audit disimpan dengan transaksi yang sama (jika ada), sehingga rollback transaksi utama juga rollback record audit.
- **HEADER + DETAIL ITEM:** Snapshot transaksi mencakup semua item-line, bukan hanya header, sehingga admin dapat melihat detail lengkap transaksi yang dihapus/diedit.

---

## 2. Struktur Tabel

```sql
-- ============================================================
-- Tabel tbl_audit_trail: Menyimpan semua record audit aktif.
-- Tidak ada kolom data_sebelum — semua info ada di kolom `ket`.
-- ============================================================
CREATE TABLE IF NOT EXISTS `tbl_audit_trail` (
    `id_audit`      INT           NOT NULL AUTO_INCREMENT,
    `waktu_aksi`    DATETIME      NOT NULL,
    `jenis_aksi`    CHAR(12)      NOT NULL COMMENT 'HAPUS | EDIT | TAMBAH_STOK | KURANG_STOK',
    `jenis_trans`   VARCHAR(20)   NOT NULL COMMENT 'Penjualan | Pembelian | Master User | dll',
    `identifier`    VARCHAR(35)   NOT NULL COMMENT 'no_faktur atau PREFIX:nilai untuk master',
    `id_user`       VARCHAR(30)   NOT NULL,
    `lokasi`        CHAR(6)       NULL     COMMENT 'TOKO atau GUDANG',
    `komputer`      VARCHAR(30)   NULL,
    `ket`           TEXT          NULL     COMMENT 'PLAIN TEXT LENGKAP: [KRITIS] + HEADER + DETAIL ITEM',
    PRIMARY KEY (`id_audit`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
  COMMENT='Audit trail edit dan hapus transaksi serta data master';

-- Indexes (hanya 3 index pada kolom dengan kardinalitas tinggi)
CREATE INDEX idx_audit_waktu ON tbl_audit_trail (waktu_aksi);
CREATE INDEX idx_audit_user ON tbl_audit_trail (id_user);
CREATE INDEX idx_audit_id ON tbl_audit_trail (identifier);

-- Tabel arsip (struktur identik)
CREATE TABLE IF NOT EXISTS `tbl_audit_trail_arsip` LIKE `tbl_audit_trail`;

-- Indexes untuk arsip
CREATE INDEX idx_audit_waktu ON tbl_audit_trail_arsip (waktu_aksi);
CREATE INDEX idx_audit_user ON tbl_audit_trail_arsip (id_user);
CREATE INDEX idx_audit_id ON tbl_audit_trail_arsip (identifier);
```

---

## 3. Format Kolom `ket` (Keterangan) Lengkap

### Untuk Transaksi dengan Detail Item (Penjualan, Pembelian, Retur)
Format:
```
[KRITIS] {Jenis Aksi} {Jenis Transaksi}
{NO_FAKTUR} | {TANGGAL} | Rp {TOTAL} | {NAMA_PELANGGAN/SUPPLIER} | {STATUS} | oleh:{USER}
  1. {NAMA_BARANG} | {QTY} {SATUAN} | Rp {HARGA} | Rp {TOTAL}
  2. {NAMA_BARANG} | {QTY} {SATUAN} | Rp {HARGA} | Rp {TOTAL}
  ...
```

Contoh Nyata dari Database:
```
[KRITIS] Hapus penjualan
PJ-2604200003 | 2026-04-20 02:35 | Rp 17.700 | AGUNG JAYA NGLONGAH | Belum Lunas | oleh:Programer
  1. Gula Rose Brand Kuning Pcs | 1.00 Pcs | Rp 17.700 | Rp 17.700
```

### Untuk Master (User, Barang, Karyawan, dll)
Format:
```
[KRITIS] {Jenis Aksi} {Jenis Transaksi} — {Identifier} — {Nama/User}
{Detail tambahan jika ada}
```

---

## 4. ModuleAuditTrail — Prosedur Utama

```vb
Imports MySql.Data.MySqlClient

Module ModuleAuditTrail

    ''' <summary>
    ''' Catat audit untuk operasi pada transaksi (snapshot diambil internal dari DB).
    ''' Dipanggil SEBELUM operasi DELETE/UPDATE dieksekusi.
    ''' </summary>
    Public Sub CatatAudit(noFaktur As String,
                          jenisAksi As String,
                          jenisTransaksi As String,
                          Optional ket As String = "",
                          Optional trans As MySqlTransaction = Nothing)
        If String.IsNullOrWhiteSpace(noFaktur) Then Exit Sub

        Try
            Dim snapshot As String = AmbilSnapshotTransaksi(noFaktur, jenisTransaksi)
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
    ''' Catat audit untuk form master (snapshot disiapkan oleh pemanggil).
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
    ''' Ambil snapshot LENGKAP transaksi (HEADER + DETAIL ITEM) dari DB.
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
                    sqlDetail = "SELECT NAMA_BARANG, QTY, SATUAN, HARGA_BELI, TOTAL_HARGA " &
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
                        Dim fk  As String  = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(0), "")
                        Dim tgl As String  = If(rd.IsDBNull(1), "", Convert.ToDateTime(rd(1)).ToString("yyyy-MM-dd HH:mm"))
                        Dim tot As Decimal = ModuleAngka.ParseDecimal(rd(2))
                        Dim pel As String  = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(3), "")
                        Dim sts As String  = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(4), "")
                        Dim usr As String  = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(5), "")

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
                            Dim nama  As String  = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(0), "")
                            Dim qty   As Decimal = ModuleAngka.ParseDecimal(rd(1))
                            Dim sat   As String  = ModuleAngka.SafeGetValue(Of String)(rd, rd.GetName(2), "")
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
            cmd.Parameters.AddWithValue("@aksi",  jenisAksi)
            cmd.Parameters.AddWithValue("@trans", jenisTransaksi)
            cmd.Parameters.AddWithValue("@id",    identifier)
            cmd.Parameters.AddWithValue("@user",  FormUtama.StatusNamaUser.Text)
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

End Module
```

---

## 5. Contoh Integrasi di FormPenjualan

```vb
Private Sub Simpanpenjualan()
    If TxtFaktur.Text = "" Then Exit Sub

    Dim trans As MySqlTransaction = conn.BeginTransaction()
    Try
        If TxtJenistransaksi.Text = "EditPenjualan" Then
            ' ✅ CATAT AUDIT SEBELUM UPDATE
            CatatAudit(TxtFaktur.Text, "EDIT", "Penjualan",
                      "[KRITIS] Edit penjualan", trans)
        End If

        ' ... (sisa kode simpan penjualan)

        trans.Commit()
    Catch ex As Exception
        trans.Rollback()
        MessageBox.Show("Gagal menyimpan: " & ex.Message)
    End Try
End Sub

Private Sub Hapuspenjualan()
    If DgvPenjualan.Rows.Count = 0 Then Exit Sub

    Dim noFaktur As String = DgvPenjualan.CurrentRow.Cells(0).Value.ToString()
    Dim trans As MySqlTransaction = conn.BeginTransaction()
    Try
        ' ✅ CATAT AUDIT SEBELUM DELETE
        CatatAudit(noFaktur, "HAPUS", "Penjualan",
                  "[KRITIS] Hapus penjualan", trans)

        Using cmd As New MySqlCommand(
            "DELETE FROM penjualan_detail WHERE FAKTUR_JUAL = @fk", conn, trans)
            cmd.Parameters.AddWithValue("@fk", noFaktur)
            cmd.ExecuteNonQuery()
        End Using

        Using cmd As New MySqlCommand(
            "DELETE FROM penjualan WHERE ID_PENJUALAN = @fk", conn, trans)
            cmd.Parameters.AddWithValue("@fk", noFaktur)
            cmd.ExecuteNonQuery()
        End Using

        trans.Commit()
    Catch ex As Exception
        trans.Rollback()
        MessageBox.Show("Gagal menghapus: " & ex.Message)
    End Try
End Sub
```

---

## 6. Desain FormAuditTrail

### Layout Windows Form

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│  AUDIT TRAIL — Laporan Aktivitas Edit & Hapus                                  [X]  │
├─────────────────────────────────────────────────────────────────────────────────────┤
│  [Panel Filter]                                                                      │
│  Dari: [DtpAwal▼]  s/d: [DtpAkhir▼]  User: [CmbUser▼]                             │
│  Aksi: [CmbJenisAksi▼]  Jenis: [CmbJenisTrans▼]  [🔍 Cari] [📥 Export]           │
├─────────────────────────────────────────────────────────────────────────────────────┤
│  [Panel Statistik]                                                                   │
│  Hapus hari ini: 3  |  Edit hari ini: 12  |  User aktif hapus: Programer (3x)   │
├─────────────────────────────────────────────────────────────────────────────────────┤
│  [DataGridView — DgvAudit]                                                           │
│  Waktu          | Aksi      | Jenis       | Identifier | User | Lokasi | Komputer | Keterangan               │
│  2026-04-20 ... | HAPUS     | Penjualan   | PJ-260420  | Prog | TOKO   | Server   | [KRITIS] Hapus penjualan... │
│  2026-04-20 ... | EDIT      | Retur Penjualan | RP-260420 | Prog | TOKO   | Server   | [KRITIS] Edit retur penjualan... │
│  ← baris HAPUS oleh user yang hapus >5x hari ini: highlight merah muda →              │
├─────────────────────────────────────────────────────────────────────────────────────┤
│  [Panel Detail — Panel dengan TextBox Multiline]                                      │
│  [KRITIS] Hapus penjualan                                                             │
│  PJ-2604200003 | 2026-04-20 02:35 | Rp 17.700 | AGUNG JAYA NGLONGAH | Belum Lunas | oleh:Programer │
│    1. Gula Rose Brand Kuning Pcs | 1.00 Pcs | Rp 17.700 | Rp 17.700          │
│  Total record: 9                                                                        │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

### Pseudocode FormAuditTrail

```vb
Private Sub FormAuditTrail_Load()
    DtpAwal.Value = DateTime.Today
    DtpAkhir.Value = DateTime.Today.AddDays(1).AddSeconds(-1)
    IsiCmbUser()
    IsiCmbJenisAksi()
    IsiCmbJenisTrans()
    TampilkanStatistik()
    MuatData()
End Sub

Private Sub MuatData()
    DgvAudit.Rows.Clear()

    ' Ambil daftar user yang hapus >5x hari ini
    Dim userHapusBanyak As New HashSet(Of String)
    Using cmdHapus As New MySqlCommand(
        "SELECT id_user FROM tbl_audit_trail " &
        "WHERE DATE(waktu_aksi) = CURDATE() AND jenis_aksi = 'HAPUS' " &
        "GROUP BY id_user HAVING COUNT(*) > 5", conn)
        Using rd = cmdHapus.ExecuteReader()
            While rd.Read()
                userHapusBanyak.Add(rd("id_user").ToString())
            End While
        End Using
    End Using

    ' Query utama
    Dim sb As New StringBuilder()
    sb.Append("SELECT id_audit, waktu_aksi, jenis_aksi, jenis_trans, identifier, " &
              "id_user, lokasi, komputer, ket " &
              "FROM tbl_audit_trail " &
              "WHERE waktu_aksi BETWEEN @awal AND @akhir")
    If CmbUser.Text <> "Semua" Then sb.Append(" AND id_user = @user")
    If CmbJenisAksi.Text <> "Semua" Then sb.Append(" AND jenis_aksi = @aksi")
    If CmbJenisTrans.Text <> "Semua" Then sb.Append(" AND jenis_trans = @trans")
    sb.Append(" ORDER BY waktu_aksi DESC")

    Using cmd As New MySqlCommand(sb.ToString(), conn)
        cmd.Parameters.AddWithValue("@awal", DtpAwal.Value)
        cmd.Parameters.AddWithValue("@akhir", DtpAkhir.Value)
        ' Tambahkan parameter lain sesuai filter

        Using rd As MySqlDataReader = cmd.ExecuteReader()
            While rd.Read()
                ' Baca semua kolom
                Dim waktu As String = Convert.ToDateTime(rd("waktu_aksi")).ToString("dd/MM/yyyy HH:mm:ss")
                Dim aksi As String = rd("jenis_aksi").ToString()
                Dim trans As String = rd("jenis_trans").ToString()
                Dim ident As String = rd("identifier").ToString()
                Dim user As String = rd("id_user").ToString()
                Dim lok As String = If(IsDBNull(rd("lokasi")), "", rd("lokasi").ToString())
                Dim pc As String = If(IsDBNull(rd("komputer")), "", rd("komputer").ToString())
                Dim ket As String = If(IsDBNull(rd("ket")), "", rd("ket").ToString())
                Dim idAudit As Integer = Convert.ToInt32(rd("id_audit"))

                ' Tambahkan ke DataGridView
                Dim idx As Integer = DgvAudit.Rows.Add(waktu, aksi, trans, ident, user, lok, pc, ket, idAudit)

                ' Highlight jika user hapus banyak
                If aksi = "HAPUS" AndAlso userHapusBanyak.Contains(user) Then
                    DgvAudit.Rows(idx).DefaultCellStyle.BackColor = Color.MistyRose
                End If
            End While
        End Using
    End Using

    LblTotalRecord.Text = "Total record: " & DgvAudit.Rows.Count
End Sub

Private Sub DgvAudit_SelectionChanged()
    ' Tampilkan kolom `ket` langsung di panel detail
    If DgvAudit.CurrentRow IsNot Nothing Then
        Dim ketCell As Object = DgvAudit.CurrentRow.Cells("ColKet").Value
        TxtDetail.Text = If(ketCell IsNot Nothing AndAlso Not IsDBNull(ketCell),
                            ketCell.ToString(), "(Tidak ada keterangan)")
    End If
End Sub
```

---

## 7. Ringkasan Perbedaan dari Dokumentasi Lama (Outdated)

| Aspek | Dokumentasi Lama | Sistem Yang Sudah Berjalan |
|-------|-------------------|------------------------------|
| Format penyimpanan | JSON + GZip MEDIUMBLOB | PLAIN TEXT (tipe TEXT) |
| Kolom `data_sebelum` | Ada | Sudah dihapus |
| Snapshot | Hanya header | HEADER + SEMUA ITEM-LINE |
| Kolom `ket` | VARCHAR(100) / VARCHAR(255) | TEXT (lebar) |
| Keterbacaan | Butuh parsing JSON/decompress | Langsung dibaca |
