
# Design Document — Perbaikan Histori Hutang/Piutang

## Arsitektur Solusi

Sistem saat ini mencatat hutang/piutang hanya saat pembayaran dilakukan. Perbaikan mengubah `hutang_detail` dan `piutang_detail` menjadi **buku besar per faktur** yang mencatat 4 jenis event:
- **TIMBUL**: Hutang/piutang baru terbentuk saat pembelian/penjualan kredit
- **BAYAR**: Pembayaran dilakukan
- **RETUR**: Retur barang yang memotong hutang/piutang
- **HAPUS**: Pembatalan faktur

Kolom `JENIS` pada kedua tabel akan menjadi pembeda event. Baris `JENIS='TIMBUL'` menjadi baris utama yang nilai `HUTANG`-nya selalu diperbarui untuk mencerminkan sisa terkini.

## Struktur Database

### Tabel hutang_detail (Existing — Perlu ALTER)
```sql
-- Kolom yang sudah ada:
-- ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA, JENIS, TANGGAL_BELI,
-- TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS,
-- ID_USER, ID_KOMPUTER, created_at, updated_at, sync_id

-- Kondisi aktual: JENIS VARCHAR(100) NULL (tidak ada default)
-- ALTER yang diperlukan:
UPDATE hutang_detail SET JENIS = 'BAYAR' WHERE JENIS IS NULL OR JENIS = '';
ALTER TABLE hutang_detail MODIFY COLUMN JENIS VARCHAR(10) NOT NULL DEFAULT 'BAYAR';
```

### Tabel piutang_detail (Existing — Perlu ALTER)
```sql
-- Kolom yang sudah ada:
-- ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_JUAL, KODE, NAMA, JENIS, TANGGAL_JUAL,
-- PIUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS,
-- ID_USER, ID_KOMPUTER, created_at, updated_at, sync_id

-- Kondisi aktual: JENIS VARCHAR(20) NULL (tidak ada default)
-- ALTER yang diperlukan:
UPDATE piutang_detail SET JENIS = 'BAYAR' WHERE JENIS IS NULL OR JENIS = '';
ALTER TABLE piutang_detail MODIFY COLUMN JENIS VARCHAR(20) NOT NULL DEFAULT 'BAYAR';
```

> **CATATAN PENTING — Nilai enum yang benar di database produksi:**
> - `pembelian.STATUS_TRANSAKSI_BELI`: nilai kredit = `'Belum Lunas'` (bukan `'TERHUTANG'`)
> - `penjualan.STATUS_TRANSAKSI`: nilai kredit = `'Belum Lunas'` ATAU `'TERHUTANG'` (keduanya ada)
> - Kolom total penjualan: `GRAND_TOTAL_STL_PAJAK` (bukan `GRAND_TOTAL`)
> - Kolom sisa piutang: `penjualan.SISA_TAGIHAN`
> - Kolom sudah dibayar piutang: `penjualan.NOMINALBAYARPIUTANG`

## Komponen yang Dimodifikasi

### 1. Skrip Migrasi Database (File Baru)
**File:** `Database/18_migrasi_hutang_piutang_detail.sql`

Isi skrip:
- ALTER TABLE untuk kolom JENIS (idempoten)
- INSERT baris TIMBUL untuk faktur kredit lama di `hutang_detail`:
  ```sql
  INSERT INTO hutang_detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA, JENIS,
    TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS,
    ID_USER, ID_KOMPUTER)
  SELECT 
    CONCAT('MIGRASI-', p.ID_PEMBELIAN),
    NOW(),
    IFNULL(p.LOKASI, ''),
    p.ID_PEMBELIAN,
    p.ID_SUPPLIER,
    p.NAMA_SUPLIYER,
    'TIMBUL',
    p.TGL_BELI,
    p.GRAND_TOTAL_BELI,
    p.PEMBAYARAN,          -- sudah dibayar sebelumnya (bukan 0)
    p.RETUR,               -- sudah diretur sebelumnya (bukan 0)
    p.TAGIHAN,             -- sisa hutang terkini
    p.JATUH_TEMPO,
    0,                     -- PEMBAYARAN di baris ini = 0 (baris TIMBUL bukan baris bayar)
    CASE WHEN p.TAGIHAN <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END,
    'MIGRASI',
    'MIGRASI'
  FROM pembelian p
  WHERE p.STATUS_TRANSAKSI_BELI = 'Belum Lunas'   -- nilai enum yang benar di DB
    AND NOT EXISTS (
      SELECT 1 FROM hutang_detail hd 
      WHERE hd.ID_BELI = p.ID_PEMBELIAN AND hd.JENIS = 'TIMBUL'
    );
  ```
- INSERT serupa untuk `piutang_detail` dari tabel `penjualan`:
  ```sql
  INSERT INTO piutang_detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_JUAL, KODE, NAMA, JENIS,
    TANGGAL_JUAL, PIUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS,
    ID_USER, ID_KOMPUTER)
  SELECT
    CONCAT('MIGRASI-', p.ID_PENJUALAN),
    NOW(),
    IFNULL(p.LOKASIBARANG, ''),
    p.ID_PENJUALAN,
    p.ID_PELANGGAN,
    p.NAMA_PELANGGAN,
    'TIMBUL',
    p.TGL_TRANSAKSI,
    p.GRAND_TOTAL_STL_PAJAK,    -- kolom total penjualan yang benar
    p.NOMINALBAYARPIUTANG,      -- sudah dibayar sebelumnya (bukan 0)
    0,                          -- RETUR (tidak ada kolom retur di penjualan)
    p.SISA_TAGIHAN,             -- sisa piutang terkini
    p.JATUH_TEMPO,
    0,
    CASE WHEN p.SISA_TAGIHAN <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END,
    'MIGRASI',
    'MIGRASI'
  FROM penjualan p
  WHERE p.STATUS_TRANSAKSI IN ('Belum Lunas', 'TERHUTANG')  -- kedua nilai ada di DB produksi
    AND NOT EXISTS (
      SELECT 1 FROM piutang_detail pd
      WHERE pd.ID_JUAL = p.ID_PENJUALAN AND pd.JENIS = 'TIMBUL'
    );
  ```
- INDEX untuk performa: `CREATE INDEX idx_hutang_detail_jenis_beli ON hutang_detail(JENIS, ID_BELI);`

### 2. FormPembelian.vb — Pencatatan TIMBUL saat Simpan
**File:** `2Trans/FormPembelian.vb` (3114 baris)

**Lokasi modifikasi:** Fungsi simpan pembelian baru (cari `INSERT INTO pembelian`)

Setelah INSERT ke `pembelian`, dalam transaksi yang sama, tambahkan:
```vb
If sisaHutang > 0 Then
    ' Insert baris TIMBUL ke hutang_detail
    Using cmdTimbul As New MySqlCommand(
        "INSERT INTO hutang_detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA, " &
        "JENIS, TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, " &
        "PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
        "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_BELI, @KODE, @NAMA, " &
        "'TIMBUL', @TANGGAL_BELI, @TOTAL_HUTANG, 0, 0, @HUTANG, @JATUH_TEMPO, " &
        "0, @STATUS, @ID_USER, @ID_KOMPUTER)", conn, transaction)
        
        cmdTimbul.Parameters.AddWithValue("@ID_BAYAR", "TIMBUL-" & idPembelian)
        cmdTimbul.Parameters.AddWithValue("@TANGGAL_BAYAR", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        cmdTimbul.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
        cmdTimbul.Parameters.AddWithValue("@ID_BELI", idPembelian)
        cmdTimbul.Parameters.AddWithValue("@KODE", LblKodeSupliyer.Text)
        cmdTimbul.Parameters.AddWithValue("@NAMA", CmbSupliyer.Text)
        cmdTimbul.Parameters.AddWithValue("@TANGGAL_BELI", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
        cmdTimbul.Parameters.AddWithValue("@TOTAL_HUTANG", grandTotalBeli)
        cmdTimbul.Parameters.AddWithValue("@HUTANG", sisaHutang)
        cmdTimbul.Parameters.AddWithValue("@JATUH_TEMPO", dtpJatuhTempo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
        cmdTimbul.Parameters.AddWithValue("@STATUS", "Belum Lunas")
        cmdTimbul.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
        cmdTimbul.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
        
        cmdTimbul.ExecuteNonQuery()
    End Using
End If
```

**Lokasi modifikasi:** Fungsi hapus/edit pembelian (cari fungsi delete atau update pembelian)

Sebelum hapus/edit faktur kredit:
```vb
' Cek apakah sudah ada pembayaran
Using cmdCek As New MySqlCommand(
    "SELECT COUNT(*) FROM hutang_detail WHERE ID_BELI = @ID_BELI AND JENIS = 'BAYAR'", conn, transaction)
    cmdCek.Parameters.AddWithValue("@ID_BELI", idPembelian)
    Dim sudahDibayar As Integer = Convert.ToInt32(cmdCek.ExecuteScalar())
    
    If sudahDibayar > 0 Then
        If MessageBox.Show("Faktur ini sudah memiliki pembayaran. " &
           "Mengedit/menghapus akan mempengaruhi histori hutang. Lanjutkan?",
           "Peringatan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
            Return
        End If
    End If
End Using

' Hapus baris TIMBUL lama
Using cmdHapusTimbul As New MySqlCommand(
    "DELETE FROM hutang_detail WHERE ID_BELI = @ID_BELI AND JENIS = 'TIMBUL'", conn, transaction)
    cmdHapusTimbul.Parameters.AddWithValue("@ID_BELI", idPembelian)
    cmdHapusTimbul.ExecuteNonQuery()
End Using
```

### 3. FormBayarHutang.vb — Pencatatan BAYAR dan Update TIMBUL
**File:** `2Trans/FormBayarHutang.vb` (577 baris)

**Lokasi modifikasi:** `BtnBayar_Click` (baris 312), setelah INSERT ke `hutang_detail` existing (baris 380)

Saat ini kode sudah INSERT ke `hutang_detail` (baris 380-421) tetapi **tidak mengisi kolom JENIS**. Perlu:

**A. Tambahkan JENIS='BAYAR' pada INSERT existing:**
```vb
' Line 380 — Ubah query INSERT
Using cmdHutangDetail As New MySqlCommand(
    "INSERT INTO Hutang_Detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA, JENIS, " &
    "TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
    "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_BELI, @KODE, @NAMA, 'BAYAR', " &
    "@TANGGAL_BELI, @TOTAL_HUTANG, @DIBAYAR, @RETUR, @HUTANG, @JATUH_TEMPO, @PEMBAYARAN, @STATUS, @ID_USER, @ID_KOMPUTER)", 
    conn, transaction)
```

**B. Tambahkan UPDATE baris TIMBUL setelah INSERT BAYAR:**
```vb
' Setelah INSERT hutang_detail BAYAR (sekitar baris 421), tambahkan:
Using cmdUpdateTimbul As New MySqlCommand(
    "UPDATE hutang_detail SET " &
    "HUTANG = HUTANG - @BAYAR, " &
    "DIBAYAR = DIBAYAR + @BAYAR, " &
    "STATUS = CASE WHEN (HUTANG - @BAYAR) <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END " &
    "WHERE ID_BELI = @ID_BELI AND JENIS = 'TIMBUL'", conn, transaction)
    
    cmdUpdateTimbul.Parameters.AddWithValue("@BAYAR", bayar)
    cmdUpdateTimbul.Parameters.AddWithValue("@ID_BELI", DgvData.Rows(baris).Cells(1).Value)
    cmdUpdateTimbul.ExecuteNonQuery()
End Using
```

### 4. FormJual.vb — Pencatatan TIMBUL saat Simpan Penjualan Kredit
**File:** `2Trans/FormJual.vb` (5459 baris)

**Lokasi modifikasi:** Fungsi simpan penjualan baru (cari `INSERT INTO penjualan`)

Setelah INSERT ke `penjualan`, dalam transaksi yang sama:
```vb
If sisaTagihan > 0 Then
    ' Insert baris TIMBUL ke piutang_detail
    Using cmdTimbul As New MySqlCommand(
        "INSERT INTO piutang_detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_JUAL, KODE, NAMA, " &
        "JENIS, TANGGAL_JUAL, PIUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, " &
        "PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
        "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_JUAL, @KODE, @NAMA, " &
        "'TIMBUL', @TANGGAL_JUAL, @PIUTANG, 0, 0, @HUTANG, @JATUH_TEMPO, " &
        "0, @STATUS, @ID_USER, @ID_KOMPUTER)", conn, transaction)
        
        cmdTimbul.Parameters.AddWithValue("@ID_BAYAR", "TIMBUL-" & idPenjualan)
        cmdTimbul.Parameters.AddWithValue("@TANGGAL_BAYAR", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        cmdTimbul.Parameters.AddWithValue("@LOKASI", FormUtama.StatusLokasi.Text)
        cmdTimbul.Parameters.AddWithValue("@ID_JUAL", idPenjualan)
        cmdTimbul.Parameters.AddWithValue("@KODE", LblKodePelanggan.Text)
        cmdTimbul.Parameters.AddWithValue("@NAMA", CmbPelanggan.Text)
        cmdTimbul.Parameters.AddWithValue("@TANGGAL_JUAL", DtpTanggal.Value.ToString("yyyy-MM-dd HH:mm:ss"))
        cmdTimbul.Parameters.AddWithValue("@PIUTANG", grandTotalStlPajak)   ' dari GRAND_TOTAL_STL_PAJAK
        cmdTimbul.Parameters.AddWithValue("@HUTANG", sisaTagihan)             ' dari SISA_TAGIHAN
        cmdTimbul.Parameters.AddWithValue("@JATUH_TEMPO", dtpJatuhTempo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
        cmdTimbul.Parameters.AddWithValue("@STATUS", "Belum Lunas")
        cmdTimbul.Parameters.AddWithValue("@ID_USER", FormUtama.StatusNamaUser.Text)
        cmdTimbul.Parameters.AddWithValue("@ID_KOMPUTER", FormUtama.StatusNamaPC.Text)
        
        cmdTimbul.ExecuteNonQuery()
    End Using
End If
```

**Lokasi modifikasi:** Fungsi hapus/edit penjualan — mirip dengan FormPembelian (hapus baris TIMBUL sebelum simpan ulang).

### 5. FormBayarPiutang.vb — Pencatatan BAYAR dan Update TIMBUL
**File:** `2Trans/FormBayarPiutang.vb` (580 baris)

**Lokasi modifikasi:** `BtnBayar_Click` (baris 319), setelah INSERT ke `piutang_detail` existing (baris 390)

**A. Pastikan JENIS='BAYAR' sudah terisi** — kode aktual membaca dari `DgvData.Rows(baris).Cells(4).Value`
yang berisi nilai dari kolom DGV, bukan hardcode `'BAYAR'`. Harus diganti dengan hardcode `'BAYAR'`.

**B. Tambahkan UPDATE baris TIMBUL:**
```vb
' Setelah INSERT piutang_detail BAYAR (sekitar baris 431), tambahkan:
Using cmdUpdateTimbul As New MySqlCommand(
    "UPDATE piutang_detail SET " &
    "HUTANG = HUTANG - @BAYAR, " &
    "DIBAYAR = DIBAYAR + @BAYAR, " &
    "STATUS = CASE WHEN (HUTANG - @BAYAR) <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END " &
    "WHERE ID_JUAL = @ID_JUAL AND JENIS = 'TIMBUL'", conn, transaction)
    
    cmdUpdateTimbul.Parameters.AddWithValue("@BAYAR", bayar)
    cmdUpdateTimbul.Parameters.AddWithValue("@ID_JUAL", DgvData.Rows(baris).Cells(1).Value)
    cmdUpdateTimbul.ExecuteNonQuery()
End Using
```

### 6. FormEditBayarJual.vb — Update TIMBUL saat Edit Pembayaran
**File:** `2Trans/FormEditBayarJual.vb` (925 baris)

**Lokasi modifikasi:** Fungsi simpan perubahan pembayaran (cari `UPDATE penjualan` atau `BtnSimpan_Click`)

Setelah update `SISA_TAGIHAN` di tabel `penjualan`:
```vb
' Update baris TIMBUL di piutang_detail
Using cmdUpdateTimbul As New MySqlCommand(
    "UPDATE piutang_detail SET " &
    "HUTANG = @SISA_TAGIHAN_BARU, " &
    "DIBAYAR = @TOTAL_BAYAR_BARU, " &
    "STATUS = CASE WHEN @SISA_TAGIHAN_BARU <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END " &
    "WHERE ID_JUAL = @ID_JUAL AND JENIS = 'TIMBUL'", conn, transaction)
    
    cmdUpdateTimbul.Parameters.AddWithValue("@SISA_TAGIHAN_BARU", sisaTagihanBaru)
    cmdUpdateTimbul.Parameters.AddWithValue("@TOTAL_BAYAR_BARU", totalBayarBaru)
    cmdUpdateTimbul.Parameters.AddWithValue("@ID_JUAL", idPenjualan)
    
    cmdUpdateTimbul.ExecuteNonQuery()
    ' Jika TIMBUL tidak ditemukan (faktur lama), tidak error — lanjutkan
End Using
```

### 7. FormReturPembelian.vb — Pencatatan RETUR
**File:** `2Trans/FormReturPembelian.vb` (1184 baris)

**Lokasi modifikasi:** Fungsi simpan retur (cari `BtnSimpan_Click` atau INSERT ke `retur_pembelian`)

Dalam Mode Normal (`CbJenisRetur.Checked = False`) DAN `CbPotongHutang.Checked = True`:
```vb
' Setelah INSERT retur_pembelian, tambahkan:
Using cmdRetur As New MySqlCommand(
    "INSERT INTO hutang_detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA, " &
    "JENIS, TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, " &
    "PEMBAYARAN, STATUS, ID_USER, ID_KOMPUTER) " &
    "VALUES (@ID_BAYAR, @TANGGAL_BAYAR, @LOKASI, @ID_BELI, @KODE, @NAMA, " &
    "'RETUR', @TANGGAL_BELI, @TOTAL_HUTANG, @DIBAYAR, @RETUR, @HUTANG, @JATUH_TEMPO, " &
    "@PEMBAYARAN, @STATUS, @ID_USER, @ID_KOMPUTER)", conn, transaction)
    
    cmdRetur.Parameters.AddWithValue("@ID_BAYAR", "RETUR-" & idReturPembelian)
    cmdRetur.Parameters.AddWithValue("@ID_BELI", idPembelianAsal)
    cmdRetur.Parameters.AddWithValue("@PEMBAYARAN", totalRupiahRetur)
    ' ... isi parameter lainnya ...
    cmdRetur.ExecuteNonQuery()
End Using

' Update baris TIMBUL
Using cmdUpdateTimbul As New MySqlCommand(
    "UPDATE hutang_detail SET " &
    "HUTANG = HUTANG - @RETUR, " &
    "RETUR = RETUR + @RETUR, " &
    "STATUS = CASE WHEN (HUTANG - @RETUR) <= 0 THEN 'Lunas' ELSE 'Belum Lunas' END " &
    "WHERE ID_BELI = @ID_BELI AND JENIS = 'TIMBUL'", conn, transaction)
    
    cmdUpdateTimbul.Parameters.AddWithValue("@RETUR", totalRupiahRetur)
    cmdUpdateTimbul.Parameters.AddWithValue("@ID_BELI", idPembelianAsal)
    cmdUpdateTimbul.ExecuteNonQuery()
End Using
```

### 8. FormReturPenjualan.vb — Pencatatan RETUR
**File:** `2Trans/FormReturPenjualan.vb` (2043 baris)

**Lokasi modifikasi:** Mirip FormReturPembelian, tetapi untuk `piutang_detail`

### 9. FormLapHutang.vb — Ubah Query ke hutang_detail
**File:** `5Lap/FormLapHutang.vb` (248 baris)

**Lokasi modifikasi:** Fungsi yang mengisi DataGridView laporan (cari query `SELECT ... FROM pembelian`)

Ganti query dari:
```sql
SELECT ... FROM pembelian WHERE STATUS_TRANSAKSI_BELI = 'Belum Lunas'
```

Menjadi:
```sql
SELECT 
    hd.ID_BELI AS NO_FAKTUR,
    hd.NAMA AS NAMA_SUPPLIER,
    hd.TANGGAL_BELI,
    hd.TOTAL_HUTANG,
    hd.DIBAYAR,
    hd.RETUR,
    hd.HUTANG AS SISA_HUTANG,
    hd.JATUH_TEMPO
FROM hutang_detail hd
WHERE hd.JENIS = 'TIMBUL' 
  AND hd.STATUS = 'Belum Lunas'
ORDER BY hd.JATUH_TEMPO ASC
```

### 10. FormLapPiutang.vb — Ubah Query ke piutang_detail
**File:** `5Lap/FormLapPiutang.vb` (189 baris)

**Lokasi modifikasi:** Mirip FormLapHutang, ganti query dari `penjualan` ke `piutang_detail WHERE JENIS='TIMBUL' AND STATUS='Belum Lunas'`

## Flow Data

### Pembelian Kredit Baru:
```
FormPembelian.Save → INSERT pembelian (STATUS_TRANSAKSI_BELI='Belum Lunas') 
                   → INSERT hutang_detail (JENIS='TIMBUL', HUTANG=sisa_hutang)
                   → JIKA bayar_dp > 0 → INSERT hutang_detail (JENIS='BAYAR') 
                                       → UPDATE hutang_detail TIMBUL (HUTANG -= dp, DIBAYAR += dp)
```

### Bayar Hutang:
```
FormBayarHutang.Save → INSERT hutang (header pembayaran)
                     → INSERT hutang_detail (JENIS='BAYAR', PEMBAYARAN=nominal)
                     → UPDATE hutang_detail TIMBUL (HUTANG -= nominal, DIBAYAR += nominal, STATUS=kalkulasi)
```

### Retur Pembelian (Potong Hutang):
```
FormReturPembelian.Save → INSERT retur_pembelian
                        → INSERT hutang_detail (JENIS='RETUR', PEMBAYARAN=total_retur)
                        → UPDATE hutang_detail TIMBUL (HUTANG -= retur, RETUR += retur, STATUS=kalkulasi)
```

### Konsistensi Data:
```
Invariant: DIBAYAR + RETUR + HUTANG = TOTAL_HUTANG  (pada baris JENIS='TIMBUL')
```

## Testing Strategy

1. **Unit Test Manual:**
   - Buat pembelian kredit baru → Cek `hutang_detail` ada baris TIMBUL
   - Bayar sebagian → Cek baris BAYAR masuk dan TIMBUL terupdate
   - Bayar lunas → Cek STATUS berubah jadi 'Lunas'
   - Retur dengan potong hutang → Cek baris RETUR dan TIMBUL terupdate
   - Edit pembelian kredit → Cek TIMBUL dihapus dan dibuat baru

2. **Migrasi Test:**
   - Backup database
   - Jalankan skrip migrasi
   - Verifikasi: `SELECT COUNT(*) FROM hutang_detail WHERE JENIS='TIMBUL'` = jumlah faktur kredit di `pembelian`
   - Jalankan ulang skrip → Verifikasi tidak ada duplikat

3. **Laporan Test:**
   - Bandingkan total hutang di FormLapHutang sebelum dan sesudah migrasi (harus sama)
   - Bandingkan total piutang di FormLapPiutang sebelum dan sesudah migrasi (harus sama)

## Risiko dan Mitigasi

| Risiko | Mitigasi |
|--------|----------|
| Data hilang saat migrasi | Backup database sebelum migrasi, skrip idempoten |
| Performa migrasi lambat (161K baris) | Gunakan batch INSERT, tambah INDEX sebelum migrasi |
| Rollback tidak konsisten | Semua operasi dalam transaksi database |
| User edit faktur yang sudah dibayar | Tampilkan peringatan sebelum menghapus baris TIMBUL |

## Urutan Implementasi

1. Skrip ALTER TABLE dan migrasi data (Requirement 1 & 2)
2. FormPembelian — Pencatatan TIMBUL (Requirement 3 & 4)
3. FormBayarHutang — Pencatatan BAYAR + Update TIMBUL (Requirement 5)
4. FormJual — Pencatatan TIMBUL (Requirement 6 & 7)
5. FormBayarPiutang — Pencatatan BAYAR + Update TIMBUL (Requirement 8)
6. FormEditBayarJual — Update TIMBUL (Requirement 8b)
7. FormReturPembelian — Pencatatan RETUR (Requirement 9)
8. FormReturPenjualan — Pencatatan RETUR (Requirement 10)
9. FormLapHutang — Query dari hutang_detail (Requirement 13)
10. FormLapPiutang — Query dari piutang_detail (Requirement 14)
11. Testing dan verifikasi konsistensi data (Requirement 11 & 12)
