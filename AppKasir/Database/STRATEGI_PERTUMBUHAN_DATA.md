# Strategi Pertumbuhan Data & Tutup Tahun — AppKasir

> Dokumen ini dibuat berdasarkan analisa mendalam kode VB.NET, struktur database,
> dan data aktual dari `db_moroseneng` (produksi).
> Wajib dibaca sebelum membuat fitur arsip, tutup tahun, atau partisi tabel.

---

## 1. Kondisi Saat Ini

### Ukuran Database Aktual (db_moroseneng, April 2026)

| Tabel | Baris | Ukuran | Pertumbuhan |
|---|---|---|---|
| `historybarang` | 1.161.025 | 628 MB | Setiap transaksi barang = 1+ baris |
| `penjualan_detail` | 855.175 | 468 MB | Setiap item penjualan = 1 baris |
| `jurnalumum` | 627.177 | 276 MB | Setiap transaksi = 2–5 baris jurnal |
| `penjualan` | 161.209 | 172 MB | Setiap nota = 1 baris |
| `jurnalumum_backup_coa` | 612.420 | 164 MB | Backup migrasi COA — tidak aktif |
| `transfer_barang_detail` | 77.993 | 40 MB | |
| `transfer_stok` | 57.392 | 36 MB | |

**Total database: ~1.88 GB** setelah optimasi index.

### Estimasi Pertumbuhan Per Tahun

Berdasarkan data aktual (asumsi database sudah berjalan ~2 tahun):

| Tabel | Estimasi/Tahun | Ukuran/Tahun |
|---|---|---|
| `historybarang` | ~580.000 baris | ~300 MB |
| `penjualan_detail` | ~430.000 baris | ~230 MB |
| `jurnalumum` | ~310.000 baris | ~135 MB |
| `penjualan` | ~80.000 baris | ~85 MB |

**Tanpa penanganan: database tumbuh ~750 MB/tahun.**
Dalam 3 tahun ke depan: ~4 GB. Dalam 5 tahun: ~5.5 GB.

---

## 2. Masalah Arsitektur yang Perlu Diperhatikan

### 2a. Laporan Keuangan Bergantung pada Seluruh JurnalUmum

Ini adalah **constraint terpenting** yang membatasi strategi arsip.

`ModuleLaporanKalkulasi` menghitung saldo akun dengan query:
```sql
-- Saldo awal periode = SALDO_AWAL + SUM(jurnal SEBELUM tanggalAwal)
SELECT SUM(NOMINAL) FROM JurnalUmum WHERE TGL_TRANSAKSI < @tanggalAwal
```

Artinya: **jika jurnal lama dihapus, saldo awal periode akan salah.**

Solusi yang benar: sebelum arsip jurnal, saldo akhir tahun harus "dikunci" ke `tbl_datareferensi.SALDO_AWAL` sebagai titik awal baru.

### 2b. Stok Dihitung dari HistoryBarang

`FormHapusTransaksi` menunjukkan bahwa setelah hapus data, stok dihitung ulang dari `historybarang`:
```vb
UpdateAllBarangTokoModule()   ' isi ulang counter dari HistoryBarang
HitungStokToko()              ' hitung STOK final
```

Artinya: **jika historybarang lama dihapus tanpa prosedur yang benar, stok akan salah.**

Solusi: sebelum arsip historybarang, stok saat ini harus "dikunci" sebagai saldo awal baru.

### 2c. Tidak Ada Tutup Tahun Otomatis

Saat ini hanya ada:
- Tutup bulan berbasis tanggal cutoff (untuk filter laporan)
- `FormHapusTransaksi` — hapus semua data, bukan arsip

Tidak ada mekanisme:
- Tutup tahun dengan carry-forward saldo
- Arsip data lama ke tabel terpisah
- Partisi tabel per tahun

---

## 3. Rekomendasi Strategi

### Strategi A — Tutup Tahun dengan Carry-Forward Saldo (DIREKOMENDASIKAN)

Ini adalah mekanisme akuntansi yang benar. Dilakukan **sekali per tahun** (biasanya Januari).

**Langkah-langkah:**

#### Step 1 — Posting Resmi Akhir Tahun
Jalankan `PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()` untuk memastikan semua saldo akun di `tbl_datareferensi` sudah final dan benar.

#### Step 2 — Kunci Saldo Akhir Tahun sebagai Saldo Awal Tahun Baru
```sql
-- Pindahkan SALDO_AKHIR → SALDO_AWAL untuk semua akun
-- Ini adalah "carry-forward" saldo ke tahun baru
UPDATE tbl_datareferensi
SET SALDO_AWAL = SALDO_AKHIR,
    SALDO_SEBELUMNYA = SALDO_AKHIR,
    S_DEBET = 0,
    S_KREDIT = 0,
    SALDO_AKHIR = SALDO_AKHIR  -- tetap sama sampai ada transaksi baru
WHERE TYPE_AKUN NOT IN ('LABA RUGI');

-- Akun LABA RUGI: reset ke 0 (laba/rugi tahun lalu sudah masuk ke modal)
UPDATE tbl_datareferensi
SET SALDO_AWAL = 0, SALDO_SEBELUMNYA = 0, S_DEBET = 0, S_KREDIT = 0, SALDO_AKHIR = 0
WHERE TYPE_AKUN = 'LABA RUGI';
```

#### Step 3 — Kunci Stok Saat Ini sebagai Saldo Awal Stok
```sql
-- Stok saat ini sudah benar di tbl_barang.STOK_TOKO dan STOK_GUDANG
-- Simpan sebagai AWAL_TOKO dan AWAL_GUDANG (basis perhitungan stok baru)
UPDATE tbl_barang SET
    AWAL_TOKO   = STOK_TOKO,
    AWAL_GUDANG = STOK_GUDANG,
    STOK_AWAL_TOKO   = STOK_TOKO,
    STOK_AWAL_GUDANG = STOK_GUDANG;
```

#### Step 4 — Arsip Data Transaksi Lama
Pindahkan data transaksi tahun lalu ke tabel arsip (bukan hapus):
```sql
-- Contoh untuk jurnalumum
INSERT INTO jurnalumum_arsip_2024
SELECT * FROM jurnalumum WHERE YEAR(TGL_TRANSAKSI) = 2024;

DELETE FROM jurnalumum WHERE YEAR(TGL_TRANSAKSI) = 2024;
```

#### Step 5 — Reset Counter HistoryBarang
Setelah arsip, reset counter di `tbl_barang`:
```sql
UPDATE tbl_barang SET
    TAMBAH_TOKO = 0, KURANG_TOKO = 0,
    TAMBAH_GUDANG = 0, KURANG_GUDANG = 0;
```

**Hasil:** Laporan tahun baru dimulai dari saldo yang benar, data lama tersimpan di tabel arsip, database aktif jauh lebih kecil.

---

### Strategi B — Arsip Rolling 2 Tahun (Tanpa Tutup Tahun)

Untuk tabel yang tidak mempengaruhi saldo (historybarang, penjualan_detail, penjualan):

```sql
-- Arsip data > 2 tahun ke tabel terpisah
-- Jalankan setiap awal tahun atau saat database > 2 GB

CREATE TABLE IF NOT EXISTS historybarang_arsip LIKE historybarang;
INSERT INTO historybarang_arsip
SELECT * FROM historybarang
WHERE TANGGAL < DATE_SUB(NOW(), INTERVAL 2 YEAR);

DELETE FROM historybarang
WHERE TANGGAL < DATE_SUB(NOW(), INTERVAL 2 YEAR);

OPTIMIZE TABLE historybarang;
```

**Catatan penting:**
- `historybarang` bisa diarsip karena stok sudah dikunci di `tbl_barang`
- `jurnalumum` TIDAK boleh diarsip tanpa carry-forward saldo terlebih dahulu
- `penjualan` dan `penjualan_detail` bisa diarsip jika piutang sudah lunas semua

---

### Strategi C — Partisi Tabel per Tahun (Jangka Panjang)

Untuk database yang sudah sangat besar (> 5 GB), pertimbangkan partisi MySQL:

```sql
ALTER TABLE historybarang
PARTITION BY RANGE (YEAR(TANGGAL)) (
    PARTITION p2023 VALUES LESS THAN (2024),
    PARTITION p2024 VALUES LESS THAN (2025),
    PARTITION p2025 VALUES LESS THAN (2026),
    PARTITION p_future VALUES LESS THAN MAXVALUE
);
```

**Keuntungan:** Query dengan filter tahun hanya scan partisi yang relevan.
**Risiko:** Perubahan struktural besar, butuh testing menyeluruh.
**Rekomendasi:** Terapkan hanya jika database sudah > 5 GB dan query laporan mulai lambat.

---

## 4. Tabel Arsip yang Perlu Dibuat

Jika memilih Strategi A atau B, buat tabel arsip berikut:

| Tabel Aktif | Tabel Arsip | Kapan Diarsip |
|---|---|---|
| `jurnalumum` | `jurnalumum_arsip_YYYY` | Setelah tutup tahun + carry-forward |
| `historybarang` | `historybarang_arsip_YYYY` | Data > 2 tahun |
| `penjualan` | `penjualan_arsip_YYYY` | Data > 2 tahun, piutang lunas |
| `penjualan_detail` | `penjualan_detail_arsip_YYYY` | Ikut penjualan |
| `pembelian` | `pembelian_arsip_YYYY` | Data > 2 tahun, hutang lunas |
| `pembelian_detail` | `pembelian_detail_arsip_YYYY` | Ikut pembelian |
| `stok_opname` | `stok_opname_arsip_YYYY` | Data > 2 tahun |
| `transfer_barang` | `transfer_barang_arsip_YYYY` | Data > 2 tahun |
| `transfer_barang_detail` | `transfer_barang_detail_arsip_YYYY` | Ikut transfer_barang |

---

## 5. Urutan Implementasi yang Aman

```
TAHAP 1 — Persiapan (sekarang)
  ✅ Optimasi index (sudah selesai)
  ✅ Resize VARCHAR (sudah selesai)
  [ ] Buat file SQL tutup tahun (15_tutup_tahun.sql)
  [ ] Test di db_kasirlancar dulu

TAHAP 2 — Tutup Tahun Pertama (Januari 2027)
  [ ] Backup penuh database
  [ ] Jalankan 15_tutup_tahun.sql
  [ ] Verifikasi saldo neraca sebelum dan sesudah
  [ ] Verifikasi stok sebelum dan sesudah
  [ ] Arsip data 2024 ke tabel arsip

TAHAP 3 — Arsip Rolling (setiap Januari)
  [ ] Arsip data > 2 tahun
  [ ] OPTIMIZE TABLE semua tabel yang diarsip
  [ ] Verifikasi laporan masih benar

TAHAP 4 — Partisi (jika database > 5 GB)
  [ ] Analisa query yang paling lambat
  [ ] Implementasi partisi di tabel terbesar
  [ ] Test performa sebelum dan sesudah
```

---

## 6. Hal yang TIDAK Boleh Dilakukan

| Larangan | Alasan |
|---|---|
| Hapus `jurnalumum` tanpa carry-forward saldo | Saldo neraca akan salah — laporan keuangan tidak bisa dipercaya |
| Hapus `historybarang` tanpa kunci stok | Stok akan dihitung ulang dari 0 — semua stok jadi 0 |
| Arsip `penjualan` yang masih ada piutang | Piutang tidak bisa dilacak, laporan piutang salah |
| Arsip `pembelian` yang masih ada hutang | Hutang tidak bisa dilacak, laporan hutang salah |
| Jalankan arsip tanpa backup | Tidak ada git, tidak ada undo |
| Arsip sebagian tabel tanpa arsip tabel relasinya | Orphan data — laporan detail tidak cocok dengan header |

---

## 7. Checklist Sebelum Tutup Tahun

- [ ] Backup penuh database (mysqldump)
- [ ] Semua piutang pelanggan sudah diverifikasi (yang belum lunas dicatat)
- [ ] Semua hutang supplier sudah diverifikasi
- [ ] Stok opname sudah dilakukan dan selisih sudah dijurnal
- [ ] Laporan laba rugi tahun berjalan sudah dicetak dan disimpan
- [ ] Neraca akhir tahun sudah dicetak dan disimpan
- [ ] `PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()` sudah dijalankan
- [ ] Saldo semua akun di `tbl_datareferensi` sudah diverifikasi
- [ ] Jalankan `15_tutup_tahun.sql` di database test dulu
- [ ] Verifikasi laporan setelah tutup tahun di database test
- [ ] Baru jalankan di database produksi

---

## 8. File SQL yang Perlu Dibuat

| File | Fungsi | Status |
|---|---|---|
| `12_hapus_index_orphan.sql` | Hapus index orphan + OPTIMIZE | ✅ Selesai |
| `13_trim_cleanup_barang.sql` | Bersihkan spasi di kolom barang | ✅ Selesai |
| `14_resize_varchar.sql` | Perkecil definisi VARCHAR | ✅ Selesai |
| `15_tutup_tahun.sql` | Carry-forward saldo + arsip data | ⏳ Perlu dibuat |
| `16_arsip_rolling.sql` | Arsip data > 2 tahun (tanpa tutup tahun) | ⏳ Perlu dibuat |
| `17_buat_tabel_arsip.sql` | Buat struktur tabel arsip | ⏳ Perlu dibuat |
