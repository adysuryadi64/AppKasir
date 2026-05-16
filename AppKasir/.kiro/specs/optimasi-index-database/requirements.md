# Requirements — Optimasi Index Database AppKasir

## Latar Belakang

Verifikasi mendalam terhadap seluruh query SQL di ~50 file VB (0Form, 1Master, 2Trans, 3Jurnal,
4Gaji, 5Lap, 6Print, 8Uty, 9Sync, Modules) menghasilkan peta lengkap query → index.
Ditemukan index redundan, index tanpa query pendukung, dan satu gap index.

Semua perubahan dilakukan **hanya di `03_migrasi_index.sql`** — tidak ada file baru.
Urutan kerja: **backup → cleanup → edit `03_migrasi_index.sql`**.

## Referensi File

- `#[[file:Database/03_migrasi_index.sql]]`
- `#[[file:8Uty/FormHapusTransaksi.vb]]`
- `#[[file:6Print/CetakLaporanKas/FormLapMutasiKeuangan.vb]]`
- `#[[file:4Gaji/FormLapBonPerorang.vb]]`
- `#[[file:Modules/ModuleLaporanKalkulasi.vb]]`
- `#[[file:5Lap/FormLapkAS.vb]]`
- `#[[file:5Lap/FormLapPenjualanSales.vb]]`

---

## Metodologi Scan

Scan dilakukan folder per folder secara berurutan:
`0Form` → `1Master` → `2Trans` → `3Jurnal` → `4Gaji` → `5Lap` → `6Print` → `8Uty` → `9Sync` → `Modules`

Setiap query SQL diekstrak dan kolom di WHERE/JOIN/ORDER BY/GROUP BY dicocokkan dengan index yang ada.

---

## Hasil Verifikasi Lengkap per Tabel

### `jurnalumum` — 13 index saat ini

| Index | Kolom | Query Pendukung | Verdict |
|---|---|---|---|
| `idx_no_transaksi_jurnal` | `NO_TRANSAKSI` | DELETE di semua form transaksi | ✅ PERTAHANKAN |
| `idx_tgl_jenis_transaksi` | `TGL_TRANSAKSI,JENIS_TRANSAKSI` | `FormLapMutasiKeuangan` CASE WHEN 15+ jenis — WHERE hanya TGL, JENIS di CASE WHEN. Tapi index ini cover `DISTINCT ID_USER WHERE TGL` juga via prefix TGL | ✅ PERTAHANKAN |
| `idx_id_user_jurnal` | `ID_USER` | Tidak ada query WHERE ID_USER saja. Semua query pakai TGL+ID_USER → `idx_tgl_id_user_jurnal` lebih baik | ❌ HAPUS |
| `idx_nomor_akun_d_jurnal` | `NOMOR_AKUN_D,TGL_TRANSAKSI` | `FormLapBB`, `FormLapMutasiKeuangan` WHERE NOMOR_AKUN_D + TGL | ✅ PERTAHANKAN |
| `idx_nomor_akun_k_jurnal` | `NOMOR_AKUN_K,TGL_TRANSAKSI` | `FormLapBB`, `FormLapMutasiKeuangan` WHERE NOMOR_AKUN_K + TGL | ✅ PERTAHANKAN |
| `idx_tgl_akun_d_nominal` | `TGL_TRANSAKSI,NOMOR_AKUN_D,NOMINAL` | Prefix dari idx_tgl_jenis_akun_d_nominal. Tidak ada query filter NOMINAL | ❌ HAPUS |
| `idx_tgl_akun_k_nominal` | `TGL_TRANSAKSI,NOMOR_AKUN_K,NOMINAL` | Prefix dari idx_tgl_jenis_akun_k_nominal. Tidak ada query filter NOMINAL | ❌ HAPUS |
| `idx_akun_d_nominal` | `NOMOR_AKUN_D,NOMINAL` | Tidak ada query filter NOMINAL tanpa TGL | ❌ HAPUS |
| `idx_akun_k_nominal` | `NOMOR_AKUN_K,NOMINAL` | Tidak ada query filter NOMINAL tanpa TGL | ❌ HAPUS |
| `idx_tgl_jenis_akun_d_nominal` | `TGL_TRANSAKSI,JENIS_TRANSAKSI,NOMOR_AKUN_D,NOMINAL` | Query utama pakai CASE WHEN — JENIS/NOMOR_AKUN tidak di WHERE. ExecuteQuery lebih baik pakai idx_nomor_akun_d_jurnal | ❌ HAPUS |
| `idx_tgl_jenis_akun_k_nominal` | `TGL_TRANSAKSI,JENIS_TRANSAKSI,NOMOR_AKUN_K,NOMINAL` | Sama — CASE WHEN tidak bisa pakai index. idx_nomor_akun_k_jurnal lebih optimal | ❌ HAPUS |
| `idx_tgl_id_user_jurnal` | `TGL_TRANSAKSI,ID_USER` | `FormLapMutasiKeuangan` WHERE TGL + ID_USER LIKE | ✅ PERTAHANKAN |
| `idx_lokasi_jurnal` | `LOKASI` | `FormHapusTransaksi` DELETE WHERE LOKASI | ✅ PERTAHANKAN |

**Hasil: 13 → 6 index. Hemat 7 index update per INSERT.**

---

### `tbl_barang` — 14 index saat ini

| Index | Kolom | Query Pendukung | Verdict |
|---|---|---|---|
| `idx_nama_barang` | `NAMA_BARANG` | `FormBarang`, `FormKartuStok`, `FormLapBarang` WHERE NAMA_BARANG LIKE | ✅ PERTAHANKAN |
| `idx_barcode_kecil/sedang/besar` | masing-masing | `FormKartuStok`, `FormLapMutasiBarang` WHERE BARCODE = @bc (exact match, tanpa TRIM) | ✅ PERTAHANKAN |
| `idx_stok_minimum` | `STOK_MIN,STOK_TOKO,STOK_GUDANG` | `FormLapStokMinim` filter stok minimum | ✅ PERTAHANKAN |
| `idx_stok_toko_gudang` | `STOK_TOKO,STOK_GUDANG` | Prefix dari idx_stok_minimum — tidak ada query filter STOK_TOKO+STOK_GUDANG tanpa STOK_MIN | ❌ HAPUS |
| `idx_stok_toko_id_nama_harga` | `STOK_TOKO,ID_BARANG,NAMA_BARANG,HARGA_BELI` | `FormLapStokMinim_takGerak` covering index | ✅ PERTAHANKAN |
| `idx_stok_gudang_id_nama_harga` | `STOK_GUDANG,ID_BARANG,NAMA_BARANG,HARGA_BELI` | `FormLapStokMinim_takGerak` covering index | ✅ PERTAHANKAN |
| `idx_status_barang` | `STATUS` | `FormBarang`, `FormPembelian` WHERE STATUS = 'Aktif' | ✅ PERTAHANKAN |
| `idx_status_nama_barang` | `STATUS,NAMA_BARANG` | `TambahBarang` WHERE STATUS='Aktif' ORDER BY NAMA | ✅ PERTAHANKAN |
| `idx_kategori_barang` | `NAMA_KATEGORI` | `FormBarang` filter kategori | ✅ PERTAHANKAN |
| `idx_kode_kategori_barang` | `KODE_KATEGORI` | `TambahBarang` WHERE ID_BARANG LIKE kodeKategori% | ✅ PERTAHANKAN |
| `idx_id_barang_prefix` | `ID_BARANG` | Duplikat PRIMARY KEY — semua query `WHERE ID_BARANG = @id` pakai PK otomatis. Query `WHERE ID_BARANG LIKE @prefix%` bisa pakai PK juga | ❌ HAPUS |
| `idx_updated_at_barang` | `updated_at` | `SyncManager` sync delta | ✅ PERTAHANKAN |
| `idx_is_dirty` | `is_dirty` | `SyncTrigger` | ✅ PERTAHANKAN |
| `idx_id_cloud` | `id_cloud` | `SyncManager` WHERE id_cloud = @id | ✅ PERTAHANKAN |

**Hasil: 14 → 12 index.**

---

### `tbl_barang` — Perbaikan BARCODE + TRIM (Task Terpisah)

**Akar masalah:** Query pencarian barang di beberapa form menggunakan `TRIM()` pada kolom barcode:

```sql
-- FormPembelian.vb, FormTransferBarang.vb, FormTransferStok.vb
WHERE STATUS = 'Aktif' AND (
    TRIM(ID_BARANG) LIKE @Nama OR TRIM(NAMA_BARANG) LIKE @Nama OR
    TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(BARCODE_SEDANG) LIKE @Nama OR
    TRIM(BARCODE_BESAR) LIKE @Nama
)
```

`TRIM()` di dalam WHERE = MySQL tidak bisa pakai index → full table scan setiap pencarian barang saat kasir input.

**Solusi 3 langkah (harus berurutan):**

**Langkah 1 — Bersihkan data lama sekali saja (SQL):**
```sql
UPDATE tbl_barang SET
  BARCODE_KECIL  = TRIM(BARCODE_KECIL),
  BARCODE_SEDANG = TRIM(BARCODE_SEDANG),
  BARCODE_BESAR  = TRIM(BARCODE_BESAR),
  ID_BARANG      = TRIM(ID_BARANG),
  NAMA_BARANG    = TRIM(NAMA_BARANG)
WHERE BARCODE_KECIL  != TRIM(BARCODE_KECIL)
   OR BARCODE_SEDANG != TRIM(BARCODE_SEDANG)
   OR BARCODE_BESAR  != TRIM(BARCODE_BESAR)
   OR ID_BARANG      != TRIM(ID_BARANG)
   OR NAMA_BARANG    != TRIM(NAMA_BARANG);
```

**Langkah 2 — TRIM saat INSERT/UPDATE di VB:**
Di semua form yang menyimpan data barang, pastikan `.Trim()` dipanggil sebelum `AddWithValue`:
```vb
cmd.Parameters.AddWithValue("@BARCODE_KECIL", TxtBarcodeKecil.Text.Trim())
cmd.Parameters.AddWithValue("@NAMA_BARANG",   TxtNama.Text.Trim())
cmd.Parameters.AddWithValue("@ID_BARANG",     TxtKode.Text.Trim())
```
File: `1Master/TambahBarang.vb`, `9Sync/SyncManager.vb` (saat sync dari cloud)

**Langkah 3 — Hapus TRIM dari query SELECT:**
Setelah data bersih, query pencarian tidak perlu TRIM lagi:
```vb
' Sebelum (tidak bisa pakai index)
Dim query = "WHERE TRIM(BARCODE_KECIL) LIKE @Nama OR TRIM(BARCODE_SEDANG) LIKE @Nama ..."
' Sesudah (bisa pakai index barcode)
Dim query = "WHERE BARCODE_KECIL LIKE @Nama OR BARCODE_SEDANG LIKE @Nama ..."
```
File: `2Trans/FormPembelian.vb`, `2Trans/FormTransferBarang.vb`, `2Trans/FormTransferStok.vb`

**Kriteria selesai:**
- [ ] SQL cleanup dijalankan sekali di production, hasilnya 0 rows affected (artinya data sudah bersih)
- [ ] Semua INSERT/UPDATE ke kolom barcode menggunakan `.Trim()` di VB
- [ ] Semua query SELECT tidak ada `TRIM()` di WHERE untuk kolom barcode/nama/kode
- [ ] `EXPLAIN` query pencarian barang menunjukkan `type: range` bukan `type: ALL`

---

### `penjualan` — 26 index saat ini

| Index | Kolom | Query Pendukung | Verdict |
|---|---|---|---|
| `idx_tgl_transaksi` | `TGL_TRANSAKSI` | Semua laporan penjualan, FormLapkAS, FormUtama | ✅ PERTAHANKAN |
| `idx_id_pelanggan` | `ID_PELANGGAN` | `ModuleVariabel` UPDATE hutang pelanggan | ✅ PERTAHANKAN |
| `idx_pelanggan_tagihan` | `ID_PELANGGAN,SISA_TAGIHAN` | `FormLapPiutang` filter piutang per pelanggan | ✅ PERTAHANKAN |
| `idx_status_transaksi` | `STATUS_TRANSAKSI` | `FormLapBBPembantu` WHERE STATUS='TERHUTANG' | ✅ PERTAHANKAN |
| `idx_lokasibarang` | `LOKASIBARANG` | `FormHapusTransaksi` DELETE WHERE LOKASIBARANG | ✅ PERTAHANKAN |
| `idx_id_user_penjualan` | `ID_USER` | `FormLapkAS` DISTINCT ID_USER WHERE TGL | ✅ PERTAHANKAN |
| `idx_tgl_kode_akun_jual` | `TGL_TRANSAKSI,KODE_AKUN` | `FormLapMutasiKeuangan` WHERE TGL+KODE_AKUN | ✅ PERTAHANKAN |
| `idx_tgl_kode_akun_tf` | `TGL_TRANSAKSI,KODE_AKUN_TF` | `FormLapMutasiKeuangan` WHERE TGL+KODE_AKUN_TF (transfer bank) | ✅ PERTAHANKAN |
| `idx_type_akun_jual` | `TYPE_AKUN` | `FormLapkAS` WHERE TYPE_AKUN='TUNAI'/'BANK'/'PIUTANG' | ✅ PERTAHANKAN |
| `idx_tgl_type_akun_jual` | `TGL_TRANSAKSI,TYPE_AKUN` | `FormLapkAS` WHERE TGL+TYPE_AKUN (query paling sering) | ✅ PERTAHANKAN |
| `idx_id_sales_tgl_jual` | `ID_SALES,TGL_TRANSAKSI` | `FormGaji` WHERE ID_SALES+TGL, `FormLapPenjualanSales` WHERE ID_SALES LIKE+TGL | ✅ PERTAHANKAN |
| `idx_nama_sales_jual` | `NAMA_SALES` | Hanya DISTINCT untuk dropdown ComboBox — bukan critical query. Filter TGL sudah cover via idx_tgl_transaksi | ❌ HAPUS |
| `idx_id_sales_jual` | `ID_SALES` | Prefix dari idx_id_sales_tgl_jual — tidak ada query filter ID_SALES saja | ❌ HAPUS |
| `idx_jatuh_tempo_status_jual` | `JATUH_TEMPO,STATUS_TRANSAKSI` | `FormNotifPiutang` WHERE JATUH_TEMPO+STATUS | ✅ PERTAHANKAN |
| `idx_jatuh_tempo_jual` | `JATUH_TEMPO` | Prefix dari idx_jatuh_tempo_status_jual | ❌ HAPUS |
| `idx_pelanggan_status` | `ID_PELANGGAN,STATUS_TRANSAKSI` | `FormLapPiutang` WHERE ID_PELANGGAN+STATUS | ✅ PERTAHANKAN |
| `idx_lokasi_tanggal` | `LOKASIBARANG,TGL_TRANSAKSI` | `FormLapPenjualan` filter lokasi+tgl | ✅ PERTAHANKAN |
| `idx_lokasi_tgl_pelanggan` | `LOKASIBARANG,TGL_TRANSAKSI,ID_PELANGGAN` | Covering untuk subquery barang lambat | ✅ PERTAHANKAN |
| `idx_pelanggan_tgl_jual` | `ID_PELANGGAN,TGL_TRANSAKSI` | Prefix dari idx_lokasi_tgl_pelanggan? Tidak — kolom pertama beda | ✅ PERTAHANKAN |
| `idx_id_tgl_lokasi` | `ID_PENJUALAN,TGL_TRANSAKSI,LOKASIBARANG` | Covering subquery barang lambat & reorder | ✅ PERTAHANKAN |
| `idx_jenis_pembayaran_jual` | `JENIS_PEMBAYARAN` | Tidak ada query WHERE JENIS_PEMBAYARAN di seluruh codebase — kolom ini hanya di SELECT/display | ❌ HAPUS |
| `idx_status_bayar_jual` | `STATUS_BAYAR` | Tidak ada query WHERE STATUS_BAYAR ditemukan di codebase | ❌ HAPUS |
| `idx_tgl_pembayaran_jual` | `TGL_PEMBAYARAN` | Tidak ada query WHERE TGL_PEMBAYARAN ditemukan di codebase | ❌ HAPUS |
| `idx_kode_akun_jual` | `KODE_AKUN` | Prefix dari idx_tgl_kode_akun_jual — tidak ada query filter KODE_AKUN saja | ❌ HAPUS |
| `idx_nama_pelanggan_jual` | `NAMA_PELANGGAN` | Hanya DISTINCT untuk dropdown + ORDER BY display — bukan critical query. idx_status_transaksi sudah cover query TERHUTANG | ❌ HAPUS |
| `idx_updated_at_jual` | `updated_at` | `SyncManager` sync delta | ✅ PERTAHANKAN |

**Hasil: 26 → 18 index. Hemat 8 index update per INSERT — tabel paling write-heavy di aplikasi kasir.**

---

### `bon_karyawan` — 9 index saat ini + 1 gap

Query di `FormLapBonPerorang.vb`:
```sql
WHERE KODE LIKE @KODE AND TANGGAL < @TANGGAL AND JENIS = 'BON'
WHERE KODE = @KODE AND TANGGAL >= @awal AND TANGGAL <= @akhir
```

Index `idx_kode_jenis_tanggal_bon (KODE,JENIS,TANGGAL)` sudah ada tapi urutan kurang optimal
untuk range TANGGAL — JENIS di posisi 2 memaksa scan setelah KODE.

**Tambah:** `idx_kode_tanggal_jenis_bon (KODE,TANGGAL,JENIS)` — KODE equality → TANGGAL range → JENIS covering.

---

### `historybarang` — 9 index, semua valid

Semua index terbukti dipakai:
- `idx_faktur_history` → DELETE WHERE FAKTUR di semua form transaksi
- `idx_lokasi_jenis_barang_qty` → `ModuleVariabel` pivot stok WHERE LOKASI GROUP BY ID_BARANG
- `idx_barang_jenis_tgl` → `FormLapMutasiBarang` WHERE ID_BARANG+LOKASI+TANGGAL
- `idx_barang_lokasi_tgl` → `FormLapStokLampau` JOIN ON ID_BARANG+TANGGAL

✅ Tidak ada perubahan.

---

### `pembelian` — 16 index saat ini

Dari scan mendalam ditemukan 3 index yang harus dihapus:

| Index | Kolom | Query Pendukung | Verdict |
|---|---|---|---|
| `idx_tgl_beli` | `TGL_BELI` | Semua laporan pembelian, FormUtama | ✅ PERTAHANKAN |
| `idx_jatuh_tempo_status_beli` | `JATUH_TEMPO,STATUS_TRANSAKSI_BELI` | `NotifikasiJatuhTempo` WHERE JATUH_TEMPO+STATUS | ✅ PERTAHANKAN |
| `idx_jatuh_tempo_beli` | `JATUH_TEMPO` | Prefix dari idx_jatuh_tempo_status_beli | ❌ HAPUS |
| `idx_nama_supliyer` | `NAMA_SUPLIYER` | Hanya DISTINCT dropdown di `FormLapPembelian` — bukan critical query | ❌ HAPUS |
| `idx_tgl_bayar_beli` | `TGL_BAYAR` | `FormLapHutang` WHERE TGL_BAYAR >= @a AND <= @b — tapi query ini ada di mode "BY PELUNASAN" | ✅ PERTAHANKAN |
| `idx_id_supplier` | `ID_SUPPLIER` | `ModuleVariabel` UPDATE hutang supplier | ✅ PERTAHANKAN |
| `idx_status_transaksi_beli` | `STATUS_TRANSAKSI_BELI` | `FormLapBBPembantu` WHERE STATUS='TERHUTANG' | ✅ PERTAHANKAN |
| `idx_jenis_bayar` | `JENIS_BAYAR` | `FormLapPembelian` WHERE JENIS_BAYAR LIKE @x | ✅ PERTAHANKAN |
| `idx_id_user_pembelian` | `ID_USER` | `FormLapPembelian` WHERE TGL+ID_USER | ✅ PERTAHANKAN |
| ... | ... | ... | ... |

**Hasil: 16 → 14 index. Hemat 2 index update per INSERT.**

> **Koreksi:** `idx_tgl_bayar_beli` ternyata **dipakai** di `FormLapHutang` mode "BY PELUNASAN" — query `WHERE TGL_BAYAR >= @a AND <= @b`. Index ini PERTAHANKAN.

---

### `penjualan_detail` — 8 index, semua valid

Semua terbukti dipakai. `idx_tgl_pelanggan_user` dipakai di `FormLapPenjualanBaru`.
`idx_barang_faktur` dan `idx_tgl_lokasi_barang` untuk covering subquery.
✅ Tidak ada perubahan.

---

### `tbl_karyawan` — 4 index saat ini

| Index | Kolom | Query Pendukung | Verdict |
|---|---|---|---|
| `idx_nama_karyawan` | `NAMA` | `FormGaji`, `FormBon`, `FormSuratJalan` WHERE Nama = @Nama (lookup saat pilih dari dropdown) | ✅ PERTAHANKAN |
| `idx_status_nama` | `Status,Nama` | `FormGaji`, `FormBon`, `FormSuratJalan` WHERE Status='Aktif' ORDER BY Nama | ✅ PERTAHANKAN |
| `idx_kode_karyawan` | `Kode` | `FormKaryawan` WHERE Kode = @Kode, UPDATE WHERE Kode = @Kode | ✅ PERTAHANKAN |
| `idx_saldo_akhir_karyawan` | `SaldoAkhir` | `FormLapBon` WHERE SaldoAkhir <> 0 — hanya untuk laporan | ⚠️ BORDERLINE |

`idx_saldo_akhir_karyawan` dipakai di `FormLapBon.vb`: `WHERE SaldoAkhir <> 0 ORDER BY Nama`. Tabel karyawan kecil (puluhan baris) — index ini tidak memberi manfaat nyata karena full scan tabel kecil lebih cepat dari index lookup. Tapi overhead-nya juga minimal karena tabel jarang di-UPDATE.

**Verdict:** ✅ PERTAHANKAN semua — tabel kecil, overhead minimal.

---

### `tbl_datareferensi` — 5 index saat ini

| Index | Kolom | Query Pendukung | Verdict |
|---|---|---|---|
| `idx_kode_akun_ref` | `KODE_AKUN` | `ModuleVerifikasiJurnal` WHERE KODE_AKUN = '05.01.001', UPDATE WHERE KODE_AKUN | ✅ PERTAHANKAN |
| `idx_type_akun` | `TYPE_AKUN` | `ModuleLaporanKalkulasi` UPDATE WHERE TYPE_AKUN = 'LABA RUGI' | ✅ PERTAHANKAN |
| `idx_nama_akun` | `NAMA_AKUN` | `FormKeuangan` WHERE Nama_Akun = @nama, `ModuleVariabel` WHERE Type_Akun LIKE | ✅ PERTAHANKAN |
| `idx_sub_akun` | `SUB_AKUN` | `ModuleLaporanKalkulasi` WHERE SUB_AKUN IN ('LABA','RUGI') | ✅ PERTAHANKAN |
| `idx_jenis_akun` | `JENIS_AKUN` | Tidak ada query WHERE JENIS_AKUN ditemukan di codebase | ❌ HAPUS |

**Hasil: 5 → 4 index.**

---

### `tbl_satuan` — 4 index saat ini + 2 gap

Query yang ada:
- `WHERE kode = @Kode` — `TambahSatuan.vb` (cek duplikat, UPDATE, DELETE)
- `WHERE nama = @Nama` — `TambahSatuan.vb`, `TambahBarang.vb` (SELECT isi WHERE nama = ?)
- `ORDER BY isi` — `TambahSatuan.vb` tampil grid
- `ORDER BY nama` — `TambahBarang.vb`, `FormBarang.vb`

Index yang ada: `idx_nama_satuan (nama)` ✅, `idx_updated_at_satuan`, `idx_is_dirty`, `idx_id_cloud`.

**Gap:** Tidak ada index pada `kode` dan `isi`.

**Tambah:**
- `idx_kode_satuan (kode)` — untuk `WHERE kode = @Kode`
- `idx_isi_satuan (isi)` — untuk `ORDER BY isi`

---

| Index | Query Pendukung | Verdict |
|---|---|---|
| `idx_username_hakakses` | `ModulHakAkses` WHERE UserName | ✅ PERTAHANKAN |
| `idx_username_role_hakakses` | `FormHakUser` WHERE UserName+Role | ✅ PERTAHANKAN |
| `idx_username_module_hakakses` | `ModulHakAkses` WHERE UserName+ModuleName | ✅ PERTAHANKAN |
| `idx_updated_at_hakakses` | `ModulHakAkses` MAX(updated_at) untuk cache refresh | ✅ PERTAHANKAN |
| `idx_role_hakakses` | `FormHakUser` WHERE Role | ✅ PERTAHANKAN |

✅ Tidak ada perubahan.

---

### `surat_jalan` — 5 index + 1 LOKASI

Semua dipakai: `idx_nota_sj` → WHERE NOTA, `idx_kode_supir_tgl` → `FormGaji` WHERE KODE_SUPIR+TGL,
`idx_kode_helper1/2_tgl` → `FormGaji` WHERE KODE_HELPER+TGL.
`idx_lokasi_surat_jalan` → `FormHapusTransaksi`. ✅ Tidak ada perubahan.

---

### `sync_queue` dan `sync_log`

`idx_status_queue` → `SyncQueue.vb` WHERE status='pending'.
`idx_waktu_log` → `SyncLog.vb` ORDER BY waktu DESC.
✅ Tidak ada perubahan.

---

### Temuan Baru dari Scan Mendalam Semua Folder

#### `tbl_barang` — Query BARCODE dengan TRIM()

Sudah dibahas lengkap di section `tbl_barang` di atas beserta solusi 3 langkah. Lihat **Task 4** di tasks.md.

#### `tbl_barang` — `idx_id_barang_prefix` redundan dengan PRIMARY KEY

Semua query `WHERE ID_BARANG = @id` sudah menggunakan PRIMARY KEY secara otomatis. Index `idx_id_barang_prefix (ID_BARANG)` adalah duplikat dari PRIMARY KEY.

**Verdict:** ❌ HAPUS `idx_id_barang_prefix` — PRIMARY KEY sudah cover ini.

#### `tbl_satuan` — Index yang hilang

Query di `TambahSatuan.vb`:
```sql
WHERE kode = @Kode   -- equality
WHERE nama = @Nama   -- cek duplikat
ORDER BY isi         -- sort
ORDER BY kode        -- sort
```

Index yang ada: `idx_nama_satuan (NAMA)`, `idx_updated_at_satuan`, `idx_is_dirty`, `idx_id_cloud`.
**Gap:** Tidak ada index pada kolom `kode` dan `isi`. Query `WHERE kode = @Kode` dan `ORDER BY isi` tidak punya index.

**Rekomendasi tambah:**
```sql
CALL add_index_if_not_exists('tbl_satuan', 'idx_kode_satuan', 'kode');
CALL add_index_if_not_exists('tbl_satuan', 'idx_isi_satuan', 'isi');
```

#### `tbl_merk` — Index yang hilang

Query di `TambahMerk.vb`:
```sql
WHERE kode = @Kode   -- equality
WHERE nama = @Nama   -- cek duplikat
ORDER BY nama        -- sort
ORDER BY kode        -- sort
```

Index yang ada: `idx_nama_merk (nama)`, `idx_kode_merk (kode)`, `idx_updated_at_merk`, `idx_is_dirty`, `idx_id_cloud`.
✅ Sudah lengkap — `idx_kode_merk` dan `idx_nama_merk` sudah ada.

#### `tbl_kategori` — Index yang hilang

Query di `TambahKategori.vb`:
```sql
WHERE kode = @Kode   -- equality
WHERE nama = @Nama   -- cek duplikat
ORDER BY nama        -- sort
ORDER BY kode        -- sort
```

Index yang ada: `idx_nama_kategori (nama)`, `idx_kode_kategori (kode)`, `idx_updated_at_kategori`, `idx_is_dirty`, `idx_id_cloud`.
✅ Sudah lengkap.

#### `History` — Index yang ada sudah cukup

`FormHistory.vb`: `WHERE TANGGAL >= @awal AND TANGGAL <= @akhir ORDER BY TANGGAL`
→ `idx_tanggal_history (Tanggal)` sudah cover. ✅

#### `pembelian` — `idx_jatuh_tempo_beli` vs `idx_jatuh_tempo_status_beli`

`NotifikasiJatuhTempo.vb`:
```sql
WHERE JATUH_TEMPO <= @Tanggal AND STATUS_TRANSAKSI_BELI = 'Belum Lunas'
```

`idx_jatuh_tempo_status_beli (JATUH_TEMPO, STATUS_TRANSAKSI_BELI)` sudah cover.
`idx_jatuh_tempo_beli (JATUH_TEMPO)` adalah prefix — **redundan**.

**Verdict:** ❌ HAPUS `idx_jatuh_tempo_beli` dari `pembelian`.

#### `pembelian` — `idx_nama_supliyer` hanya untuk display

Query di `FormLapPembelian.vb`:
```sql
SELECT DISTINCT NAMA_SUPLIYER FROM pembelian WHERE TGL_BELI >= @a AND <= @b ORDER BY NAMA_SUPLIYER
```

Sama persis dengan kasus `idx_nama_pelanggan_jual` — hanya untuk populate dropdown, bukan critical query. Filter TGL_BELI sudah ada `idx_tgl_beli`.

**Verdict:** ❌ HAPUS `idx_nama_supliyer` dari `pembelian`.

#### `pembelian` — `idx_tgl_bayar_beli` tanpa query pendukung

Scan seluruh codebase: tidak ada query `WHERE TGL_BAYAR = @x` di tabel `pembelian`. `TGL_BAYAR` hanya muncul di `FormLapHutang` sebagai filter alternatif tapi query itu filter `TGL_BAYAR >= @a AND <= @b` — dan tidak ada index yang dipakai karena query juga filter `JENIS_BAYAR LIKE @x` yang lebih selektif.

**Verdict:** ❌ HAPUS `idx_tgl_bayar_beli` dari `pembelian`.

#### `stok_opname` — `idx_id_user_opname` hanya untuk display

Query di `FormStokOpname.vb`:
```sql
WHERE TANGGAL >= @a AND TANGGAL <= @b OR ID_USER LIKE @user
```

Kondisi `OR` membuat index tidak efektif — MySQL harus full scan karena OR. Index `idx_id_user_opname` tidak berguna di sini.

**Verdict:** ❌ HAPUS `idx_id_user_opname` dari `stok_opname`.

#### `retur_pembelian` — `idx_nama_rekening_retur_beli` hanya untuk display

Query di `FormLapReturBeli.vb`:
```sql
SELECT DISTINCT NAMA_REKENING FROM retur_pembelian WHERE TGL_RETUR_BELI >= @a AND <= @b ORDER BY NAMA_REKENING
```

Hanya untuk populate dropdown. Filter TGL sudah ada `idx_tgl_retur_beli`.

**Verdict:** ❌ HAPUS `idx_nama_rekening_retur_beli` dari `retur_pembelian`.

#### `retur_penjualan` — `idx_nama_rekening_retur_jual` hanya untuk display

Sama persis — `FormLapReturJual.vb` DISTINCT NAMA_REKENING untuk dropdown.

**Verdict:** ❌ HAPUS `idx_nama_rekening_retur_jual` dari `retur_penjualan`.

---

## Ringkasan Perubahan

| Tabel | Aksi | Index | Alasan |
|---|---|---|---|
| `jurnalumum` | ❌ HAPUS | `idx_tgl_akun_d_nominal` | Prefix dari versi 4-kolom |
| `jurnalumum` | ❌ HAPUS | `idx_tgl_akun_k_nominal` | Prefix dari versi 4-kolom |
| `jurnalumum` | ❌ HAPUS | `idx_akun_d_nominal` | Tidak ada query filter NOMINAL tanpa TGL |
| `jurnalumum` | ❌ HAPUS | `idx_akun_k_nominal` | Tidak ada query filter NOMINAL tanpa TGL |
| `jurnalumum` | ❌ HAPUS | `idx_tgl_jenis_akun_d_nominal` | CASE WHEN tidak di WHERE; idx_nomor_akun_d_jurnal lebih optimal |
| `jurnalumum` | ❌ HAPUS | `idx_tgl_jenis_akun_k_nominal` | CASE WHEN tidak di WHERE; idx_nomor_akun_k_jurnal lebih optimal |
| `jurnalumum` | ❌ HAPUS | `idx_id_user_jurnal` | Tidak ada query WHERE ID_USER saja; idx_tgl_id_user_jurnal sudah cover |
| `tbl_barang` | ❌ HAPUS | `idx_stok_toko_gudang` | Prefix dari `idx_stok_minimum` |
| `tbl_barang` | ❌ HAPUS | `idx_id_barang_prefix` | Duplikat PRIMARY KEY |
| `tbl_datareferensi` | ❌ HAPUS | `idx_jenis_akun` | Tidak ada query WHERE JENIS_AKUN di codebase |
| `pembelian` | ❌ HAPUS | `idx_jatuh_tempo_beli` | Prefix dari `idx_jatuh_tempo_status_beli` |
| `pembelian` | ❌ HAPUS | `idx_nama_supliyer` | Hanya DISTINCT dropdown — bukan critical query |
| `stok_opname` | ❌ HAPUS | `idx_id_user_opname` | Query pakai OR — index tidak efektif |
| `retur_pembelian` | ❌ HAPUS | `idx_nama_rekening_retur_beli` | Hanya DISTINCT dropdown |
| `retur_penjualan` | ❌ HAPUS | `idx_nama_rekening_retur_jual` | Hanya DISTINCT dropdown |
| `penjualan` | ❌ HAPUS | `idx_id_sales_jual` | Prefix dari `idx_id_sales_tgl_jual` |
| `penjualan` | ❌ HAPUS | `idx_jatuh_tempo_jual` | Prefix dari `idx_jatuh_tempo_status_jual` |
| `penjualan` | ❌ HAPUS | `idx_status_bayar_jual` | Tidak ada query WHERE STATUS_BAYAR |
| `penjualan` | ❌ HAPUS | `idx_tgl_pembayaran_jual` | Tidak ada query WHERE TGL_PEMBAYARAN |
| `penjualan` | ❌ HAPUS | `idx_kode_akun_jual` | Prefix dari `idx_tgl_kode_akun_jual` |
| `penjualan` | ❌ HAPUS | `idx_nama_sales_jual` | Hanya DISTINCT dropdown — bukan critical query |
| `penjualan` | ❌ HAPUS | `idx_nama_pelanggan_jual` | Hanya DISTINCT dropdown + ORDER BY display |
| `penjualan` | ❌ HAPUS | `idx_jenis_pembayaran_jual` | Tidak ada query WHERE JENIS_PEMBAYARAN |
| `tbl_satuan` | ✅ TAMBAH | `idx_kode_satuan` | Gap: `WHERE kode = @Kode` di TambahSatuan |
| `tbl_satuan` | ✅ TAMBAH | `idx_isi_satuan` | Gap: `ORDER BY isi` di TambahSatuan |
| `bon_karyawan` | ✅ TAMBAH | `idx_kode_tanggal_jenis_bon` | Gap: query range TANGGAL per karyawan |

**Total: 23 index dihapus, 3 ditambah.**

Dampak terbesar:
- `jurnalumum` turun dari 13 → 6 index — setiap INSERT jurnal hemat 7 index update
- `penjualan` turun dari 26 → 18 index — setiap INSERT penjualan hemat 8 index update
- `pembelian` turun 2 index — hemat overhead INSERT pembelian
- `tbl_barang` turun 2 index — hemat overhead UPDATE stok (terjadi setiap transaksi)

**Koreksi dari analisis sebelumnya:**
- `idx_tgl_bayar_beli` di `pembelian` → **PERTAHANKAN** (dipakai di `FormLapHutang` mode BY PELUNASAN)
- `idx_id_user_jurnal` di `jurnalumum` → **HAPUS** (tidak ada query WHERE ID_USER saja)
- `idx_jenis_akun` di `tbl_datareferensi` → **HAPUS** (tidak ada query WHERE JENIS_AKUN)
