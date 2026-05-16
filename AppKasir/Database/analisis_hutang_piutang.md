# Analisis & Task — Perbaikan Sistem Hutang/Piutang

## Kondisi Saat Ini (As-Is)

### Alur Hutang Pembelian

```
FormPembelian (kredit)
  → pembelian.TAGIHAN = sisa hutang
  → pembelian.STATUS_TRANSAKSI_BELI = 'Belum Lunas'
  → jurnalumum: D Persediaan / K Hutang Belanja
  → tbl_supliyer.HutangAkhir = SUM(TAGIHAN) dari pembelian

FormBayarHutang
  → UPDATE pembelian SET TAGIHAN -= bayar, STATUS = Lunas/Belum Lunas
  → INSERT hutang_detail (1 baris per faktur yang dibayar)
  → INSERT hutang (1 baris header per sesi bayar)
  → jurnalumum: D Hutang Belanja / K Kas
  → tbl_supliyer.HutangAkhir diupdate ulang
```

### Alur Piutang Penjualan

```
FormJual/FormPenjualan (kredit)
  → penjualan.SISA_TAGIHAN = sisa piutang
  → penjualan.STATUS_TRANSAKSI = 'Belum Lunas'
  → jurnalumum: D Piutang / K Penjualan
  → tbl_pelanggan.HutangAkhir = SUM(SISA_TAGIHAN) dari penjualan

FormBayarPiutang
  → UPDATE penjualan SET SISA_TAGIHAN -= bayar, STATUS = Lunas/Belum Lunas
  → INSERT piutang_detail (1 baris per faktur yang dibayar)
  → INSERT piutang (1 baris header per sesi bayar)
  → jurnalumum: D Kas / K Piutang
  → tbl_pelanggan.HutangAkhir diupdate ulang
```

---

## Masalah yang Ditemukan

### M1 — Tidak ada catatan timbulnya hutang/piutang di tabel khusus

`hutang_detail` dan `piutang_detail` hanya terisi saat **pembayaran**, bukan saat **hutang/piutang timbul**.

Akibat:
- Tidak bisa tahu kapan hutang timbul dari tabel hutang — harus JOIN ke `pembelian`
- Laporan hutang jatuh tempo harus query `pembelian WHERE TAGIHAN > 0`
- Tidak ada histori lengkap: timbul → dibayar sebagian → lunas

### M2 — hutang_detail tidak punya kolom JENIS (TIMBUL / BAYAR)

Saat ini `hutang_detail` hanya mencatat event pembayaran. Tidak ada pembeda antara:
- Baris timbul hutang (dari pembelian)
- Baris pembayaran hutang (dari bayar hutang)

### M3 — Edit/Hapus pembelian tidak mengupdate hutang_detail

Jika faktur pembelian diedit atau dihapus:
- `pembelian.TAGIHAN` berubah
- `tbl_supliyer.HutangAkhir` diupdate via `UpdateHutangSupliyer`
- Tapi `hutang_detail` tidak disentuh sama sekali

### M4 — Retur pembelian tidak mengupdate TAGIHAN di pembelian

Retur pembelian mengurangi nilai pembelian, tapi tidak ada kode yang mengupdate
`pembelian.TAGIHAN` atau `pembelian.RETUR` secara otomatis.
(Perlu verifikasi lebih lanjut di FormReturBeli.vb)

### M5 — Kolom JENIS di hutang_detail tidak diisi

`hutang_detail.JENIS` ada di struktur tabel tapi tidak diisi di FormBayarHutang.
Kolom ini seharusnya membedakan jenis transaksi.

### M6 — tbl_supliyer.HutangAkhir dihitung dari SUM(TAGIHAN) pembelian

Ini benar secara logika, tapi rentan jika ada koreksi manual di `pembelian.TAGIHAN`.
Tidak ada audit trail perubahan nilai TAGIHAN.

---

## Desain Target (To-Be)

### Prinsip

1. `hutang_detail` = **buku besar hutang per faktur** — setiap event dicatat
2. `piutang_detail` = **buku besar piutang per faktur** — setiap event dicatat
3. Setiap perubahan nilai hutang/piutang harus ada baris baru di detail, bukan UPDATE

### Struktur Event di hutang_detail

| JENIS | Kapan | Nilai HUTANG | Nilai PEMBAYARAN |
|---|---|---|---|
| `TIMBUL` | Saat pembelian kredit | = TAGIHAN awal | 0 |
| `BAYAR` | Saat bayar hutang | = sisa setelah bayar | = nominal bayar |
| `RETUR` | Saat retur pembelian | = sisa setelah retur | 0 |
| `HAPUS` | Saat faktur dihapus | 0 | 0 (pembatalan) |

### Alur Target Hutang

```
FormPembelian (kredit)
  → pembelian.TAGIHAN = sisa hutang          [sudah ada]
  → jurnalumum: D Persediaan / K Hutang      [sudah ada]
  → INSERT hutang_detail JENIS='TIMBUL'      [BARU]
  → tbl_supliyer.HutangAkhir diupdate        [sudah ada]

FormBayarHutang
  → UPDATE pembelian.TAGIHAN -= bayar        [sudah ada]
  → INSERT hutang_detail JENIS='BAYAR'       [sudah ada, perlu tambah JENIS]
  → INSERT hutang (header)                   [sudah ada]
  → jurnalumum: D Hutang / K Kas             [sudah ada]
  → tbl_supliyer.HutangAkhir diupdate        [sudah ada]

FormReturBeli (jika ada hutang)
  → UPDATE pembelian.TAGIHAN -= nilai_retur  [BARU]
  → INSERT hutang_detail JENIS='RETUR'       [BARU]
  → jurnalumum: D Hutang / K Persediaan      [perlu cek]

FormPembelian (hapus/edit)
  → DELETE hutang_detail WHERE ID_BELI=faktur AND JENIS='TIMBUL'  [BARU]
  → INSERT hutang_detail JENIS='HAPUS' (jika ada pembayaran sebelumnya)  [BARU]
  → tbl_supliyer.HutangAkhir diupdate        [sudah ada]
```

---

## Daftar Task

### FASE 1 — Persiapan Database

**Task 1.1 — Tambah kolom JENIS di hutang_detail**
```sql
ALTER TABLE hutang_detail
    MODIFY COLUMN JENIS VARCHAR(10) NOT NULL DEFAULT 'BAYAR'
    COMMENT 'TIMBUL=saat pembelian kredit, BAYAR=saat bayar hutang, RETUR=saat retur, HAPUS=pembatalan';
```
- File: `Database/15_hutang_piutang_detail.sql`
- Isi nilai lama: `UPDATE hutang_detail SET JENIS = 'BAYAR' WHERE JENIS = '' OR JENIS IS NULL`

**Task 1.2 — Tambah kolom JENIS di piutang_detail**
```sql
ALTER TABLE piutang_detail
    MODIFY COLUMN JENIS VARCHAR(10) NOT NULL DEFAULT 'BAYAR'
    COMMENT 'TIMBUL=saat penjualan kredit, BAYAR=saat bayar piutang, RETUR=saat retur, HAPUS=pembatalan';
```
- Isi nilai lama: `UPDATE piutang_detail SET JENIS = 'BAYAR' WHERE JENIS = '' OR JENIS IS NULL`

**Task 1.3 — Migrasi data lama: isi hutang_detail JENIS='TIMBUL' dari pembelian**
```sql
-- Insert baris TIMBUL untuk semua pembelian kredit yang belum ada di hutang_detail
INSERT INTO hutang_detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA,
    TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS, JENIS)
SELECT
    CONCAT('MIGRASI-', p.ID_PEMBELIAN),
    p.TGL_BELI, p.LOKASI, p.ID_PEMBELIAN,
    p.ID_SUPPLIER, p.NAMA_SUPLIYER,
    p.TGL_BELI, p.GRAND_TOTAL_BELI,
    p.PEMBAYARAN, p.RETUR, p.TAGIHAN,
    p.JATUH_TEMPO, 0,
    IF(p.TAGIHAN = 0, 'Lunas', 'Belum Lunas'),
    'TIMBUL'
FROM pembelian p
WHERE (p.TAGIHAN > 0 OR p.STATUS_TRANSAKSI_BELI = 'Belum Lunas')
  AND p.ID_PEMBELIAN NOT IN (
      SELECT DISTINCT ID_BELI FROM hutang_detail WHERE JENIS = 'TIMBUL'
  );
```

**Task 1.4 — Migrasi data lama: isi piutang_detail JENIS='TIMBUL' dari penjualan**
- Sama seperti Task 1.3 tapi untuk penjualan → piutang_detail

---

### FASE 2 — Perubahan FormPembelian.vb

**Task 2.1 — Tambah INSERT hutang_detail JENIS='TIMBUL' saat simpan pembelian kredit**

Lokasi: `FormPembelian.vb` → `Simpanatauedit()` → setelah `Simpanjurnal()`

Kondisi: hanya jika `sisaHutang > 0` (pembelian kredit)

```vb
' Catat timbulnya hutang di hutang_detail
If Not statusLunas AndAlso sisaHutang > 0 Then
    Using cmdHD As New MySqlCommand(
        "INSERT INTO hutang_detail (ID_BAYAR, TANGGAL_BAYAR, LOKASI, ID_BELI, KODE, NAMA, " &
        "TANGGAL_BELI, TOTAL_HUTANG, DIBAYAR, RETUR, HUTANG, JATUH_TEMPO, PEMBAYARAN, STATUS, JENIS, ID_USER, ID_KOMPUTER) " &
        "VALUES (@ID_BAYAR, @TGL, @LOK, @ID_BELI, @KODE, @NAMA, @TGL_BELI, @TOTAL, 0, 0, @HUTANG, @JT, 0, 'Belum Lunas', 'TIMBUL', @USER, @PC)",
        conn, transaction)
        cmdHD.Parameters.AddWithValue("@ID_BAYAR", "TIMBUL-" & TxtFaktur.Text)
        cmdHD.Parameters.AddWithValue("@TGL", DTPTgl.Value)
        cmdHD.Parameters.AddWithValue("@LOK", LblLokasiBarang.Text)
        cmdHD.Parameters.AddWithValue("@ID_BELI", TxtFaktur.Text)
        cmdHD.Parameters.AddWithValue("@KODE", LblKodeSupplier.Text)
        cmdHD.Parameters.AddWithValue("@NAMA", TxtSupplier.Text)
        cmdHD.Parameters.AddWithValue("@TGL_BELI", DTPTgl.Value)
        cmdHD.Parameters.AddWithValue("@TOTAL", grandTotal)
        cmdHD.Parameters.AddWithValue("@HUTANG", sisaHutang)
        cmdHD.Parameters.AddWithValue("@JT", DTPJatuhTempo.Value)
        cmdHD.Parameters.AddWithValue("@USER", NamaUser)
        cmdHD.Parameters.AddWithValue("@PC", NamaPC)
        cmdHD.ExecuteNonQuery()
    End Using
End If
```

**Task 2.2 — Saat hapus/edit pembelian: hapus baris TIMBUL di hutang_detail**

Lokasi: `FormPembelian.vb` → blok hapus lama sebelum simpan ulang

```vb
' Hapus baris TIMBUL lama — akan dibuat ulang saat simpan baru
Using cmdDel As New MySqlCommand(
    "DELETE FROM hutang_detail WHERE ID_BELI = @faktur AND JENIS = 'TIMBUL'",
    conn, transaction)
    cmdDel.Parameters.AddWithValue("@faktur", TxtFaktur.Text)
    cmdDel.ExecuteNonQuery()
End Using
```

---

### FASE 3 — Perubahan FormBayarHutang.vb

**Task 3.1 — Tambah kolom JENIS='BAYAR' di INSERT hutang_detail**

Lokasi: `FormBayarHutang.vb` → `BtnBayar_Click` → `cmdHutangDetail`

Tambahkan parameter `@JENIS = 'BAYAR'` di INSERT yang sudah ada.

**Task 3.2 — Update baris TIMBUL di hutang_detail saat pembayaran**

Setelah INSERT baris BAYAR, update baris TIMBUL yang sesuai:

```vb
' Update baris TIMBUL — kurangi HUTANG sesuai yang dibayar
Using cmdUpd As New MySqlCommand(
    "UPDATE hutang_detail SET HUTANG = HUTANG - @bayar, DIBAYAR = DIBAYAR + @bayar, " &
    "STATUS = IF(HUTANG - @bayar <= 0, 'Lunas', 'Belum Lunas') " &
    "WHERE ID_BELI = @id_beli AND JENIS = 'TIMBUL'",
    conn, transaction)
    cmdUpd.Parameters.AddWithValue("@bayar", bayar)
    cmdUpd.Parameters.AddWithValue("@id_beli", DgvData.Rows(baris).Cells("ID_PEMBELIAN").Value)
    cmdUpd.ExecuteNonQuery()
End Using
```

---

### FASE 4 — Perubahan FormJual/FormPenjualan.vb (Piutang)

**Task 4.1 — Tambah INSERT piutang_detail JENIS='TIMBUL' saat simpan penjualan kredit**

Sama seperti Task 2.1 tapi untuk penjualan → piutang_detail.

**Task 4.2 — Saat hapus/edit penjualan: hapus baris TIMBUL di piutang_detail**

Sama seperti Task 2.2 tapi untuk piutang_detail.

---

### FASE 5 — Perubahan FormBayarPiutang.vb

**Task 5.1 — Tambah kolom JENIS='BAYAR' di INSERT piutang_detail**

Sama seperti Task 3.1 tapi untuk piutang_detail.

**Task 5.2 — Update baris TIMBUL di piutang_detail saat pembayaran**

Sama seperti Task 3.2 tapi untuk piutang_detail.

---

### FASE 6 — Retur (Perlu Investigasi Dulu)

**Task 6.1 — Investigasi FormReturBeli.vb**

Cek apakah retur pembelian:
- Mengupdate `pembelian.TAGIHAN` dan `pembelian.RETUR`
- Membuat jurnal yang benar
- Perlu INSERT hutang_detail JENIS='RETUR'

**Task 6.2 — Investigasi FormReturPenjualan.vb / FormReturJual.vb**

Sama seperti Task 6.1 tapi untuk piutang.

---

### FASE 7 — Laporan

**Task 7.1 — Verifikasi laporan hutang jatuh tempo**

Cek apakah laporan hutang di `5Lap/` sudah query dari `hutang_detail` atau masih dari `pembelian WHERE TAGIHAN > 0`.
Jika masih dari `pembelian`, pertimbangkan migrasi ke query dari `hutang_detail WHERE JENIS='TIMBUL' AND STATUS='Belum Lunas'`.

**Task 7.2 — Verifikasi laporan piutang jatuh tempo**

Sama seperti Task 7.1 tapi untuk piutang.

---

## Urutan Eksekusi yang Aman

```
1. Task 1.1 + 1.2  → ALTER TABLE (tidak merusak data lama)
2. Task 1.3 + 1.4  → Migrasi data lama (idempotent, bisa diulang)
3. Task 2.1        → FormPembelian: tambah INSERT TIMBUL
4. Task 4.1        → FormJual: tambah INSERT TIMBUL
5. Task 3.1 + 3.2  → FormBayarHutang: tambah JENIS + update TIMBUL
6. Task 5.1 + 5.2  → FormBayarPiutang: tambah JENIS + update TIMBUL
7. Task 2.2        → FormPembelian: hapus TIMBUL saat edit/hapus
8. Task 4.2        → FormJual: hapus TIMBUL saat edit/hapus
9. Task 6.1 + 6.2  → Investigasi + perbaikan retur
10. Task 7.1 + 7.2 → Verifikasi laporan
```

---

## Risiko & Mitigasi

| Risiko | Mitigasi |
|---|---|
| Data lama tidak punya baris TIMBUL | Task 1.3/1.4 migrasi dulu sebelum deploy |
| Duplikat baris TIMBUL jika migrasi dijalankan 2x | Gunakan `NOT IN` di query migrasi |
| Edit pembelian lama (sebelum Task 2.2) tidak hapus TIMBUL | Task 2.2 pakai `DELETE WHERE JENIS='TIMBUL'` — aman jika baris tidak ada |
| Laporan berubah setelah migrasi | Task 7 verifikasi laporan sebelum dan sesudah |
| Tidak ada git — tidak bisa rollback | Backup database sebelum Task 1.1 |
