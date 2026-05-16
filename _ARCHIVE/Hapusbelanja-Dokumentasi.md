# Dokumentasi: Hapusbelanja & UpdateHargaAverage

> Dibuat: 2026-04-27
> Tujuan: Dokumentasi kondisi kode saat ini sebagai dasar analisa dan diskusi perbaikan.

---

## Ringkasan Tiga Fungsi

| Fungsi | File | Peran |
|---|---|---|
| `FormUtama.Hapusbelanja()` | `0Form/FormUtama.vb` | Hapus permanen dari daftar transaksi |
| `FormPembelian.Hapusbelanja(transaction)` | `2Trans/FormPembelian.vb` | Hapus data lama saat proses edit |
| `FormPembelian.UpdateHargaAverage(...)` | `2Trans/FormPembelian.vb` | Update HPP saat simpan pembelian |

---

## 1. `FormUtama.Hapusbelanja()`

**Dipanggil dari:** Tombol hapus di daftar transaksi FormUtama.
**Transaction:** Dibuka dan di-commit sendiri di dalam fungsi ini.

**Alur:**
1. Tentukan `updateStokField` dari `TxtLokasiUntukEdit` (`PEMBELIAN_TOKO` / `PEMBELIAN_GUDANG`)
2. Loop `DGVDetail` — per barang: `UPDATE tbl_barang SET HARGA_BELI = HARGA_AVERAGE, HARGA_BELI_TERAKHIR = HARGA_BELI_SEBELUMNYA, PEMBELIAN_x -= QTY_SAT`
3. Catat Audit Trail
4. Kumpulkan `akunTerlibat` dari `JurnalUmum` **sebelum** delete
5. `DELETE` dari: `pembelian`, `pembelian_detail`, `JurnalUmum`, `HistoryBarang`
6. `DELETE hutang_detail` WHERE `JENIS = 'BELI'`
7. Loop `DGVDetail` lagi: `HitungStokPerubahan` per barang + kumpulkan audit delta
8. `AuditStokTransaksi`
9. `UpdateSaldoAkun` untuk semua akun terlibat → update `tbl_datareferensi.SALDO_AKHIR`
10. Commit

**Kolom DGVDetail yang dibaca:** `ID_BARANG`, `QTY_SAT`, `HARGA_AVERAGE`, `HARGA_BELI_SEBELUMNYA`

**Catatan:**
- `akunTerlibat` wajib dikumpulkan sebelum `DELETE JurnalUmum`
- `UpdateSaldoAkun` dipanggil di sini — `tbl_datareferensi` terupdate
- Loop `DGVDetail` terjadi dua kali (update HPP + audit stok)

---

## 2. `FormPembelian.Hapusbelanja(transaction)`

**Dipanggil dari:** `TekanSimpan` di FormPembelian, hanya saat mode edit (`IsModeTambahPembelian = False`).
**Transaction:** Diterima dari caller, tidak di-commit di sini.

**Urutan pemanggilan di caller:**
```
Hapusbelanja(transaction)      ← hapus data lama
SimpanPembelian(transaction)   ← simpan data baru
SimpanPembelianDetail(...)
Simpanjurnal(...)
UpdateSaldoAkun(...)           ← dipanggil caller, bukan di sini
```

**Alur:**
1. Tentukan `updateStokField` dari `LblLokasiBarang`
2. Loop `FormUtama.DGVDetail` — per barang: `UPDATE tbl_barang SET HARGA_BELI = HARGA_AVERAGE, HARGA_BELI_TERAKHIR = HARGA_BELI_SEBELUMNYA, PEMBELIAN_x -= QTY_SAT`
3. `HitungStokPerubahan` per barang + kumpulkan audit delta
4. `AuditStokTransaksi`
5. Cek `hutang_detail` — jika ada `JENIS = 'BAYAR'` → konfirmasi user → jika batal: `throw OperationCanceledException`
6. `DELETE hutang_detail` WHERE `JENIS = 'BELI'`
7. `DELETE` dari: `pembelian`, `pembelian_detail`, `JurnalUmum`, `HistoryBarang`

**Kolom DGVDetail yang dibaca:** `ID_BARANG`, `QTY_SAT`, `HARGA_AVERAGE`, `HARGA_BELI_SEBELUMNYA`

**Catatan:**
- `UpdateSaldoAkun` **tidak** dipanggil di sini — tanggung jawab caller
- Caller gabungkan `akunTerlibatLama` + `akunTerlibatBaru` sebelum panggil `UpdateSaldoAkun`

---

## 3. `FormPembelian.UpdateHargaAverage(...)`

**Dipanggil dari:** `SimpanPembelianDetail` — per baris DGV, hanya jika `SettingMetodeUpdateHargaBeli = "Metode Average (Rata - Rata)"`.

**Signature:**
```vb
Private Sub UpdateHargaAverage(
    idBarang  As String,
    hargaBaru As Decimal,   ' = HargaBeli / Isi  (harga per satuan terkecil)
    hargaLama As Decimal,   ' = kolom Average di DGV = HARGA_AVERAGE dari pembelian_detail
    qtySat    As Decimal,   ' = qty dalam satuan terkecil
    stokField As String,    ' = "PEMBELIAN_TOKO" atau "PEMBELIAN_GUDANG"
    tr        As MySqlTransaction
)
```

**Asal parameter `hargaLama` (kolom `Average` di DGV):**

| Jalur | Nilai |
|---|---|
| Barang baru dipilih dari listbox | `tbl_barang.HARGA_BELI` saat barang dipilih |
| Load mode edit | `pembelian_detail.HARGA_AVERAGE` (snapshot saat transaksi dibuat) |

**Alur:**
1. Baca `STOK_TOKO`, `STOK_GUDANG` dari `tbl_barang` sebelum update
2. Tentukan `totalStokLama` berdasarkan `SettingAverageHargaBerdasarkanStok` (Toko / Gudang / keduanya)
3. Hitung HPP baru: `ROUND((hargaLama × totalStokLama + hargaBaru × qtySat) / (totalStokLama + qtySat), 2)`
4. `UPDATE tbl_barang SET HARGA_BELI = hargaAverageBaru, HARGA_BELI_TERAKHIR = hargaBaru, PEMBELIAN_x += qtySat`

**Yang tersimpan di `pembelian_detail.HARGA_AVERAGE`:**
Diisi dari kolom `Average` DGV = **HPP di `tbl_barang` sebelum faktur ini masuk** (bukan HPP hasil average).

---

## 4. Hubungan Ketiga Fungsi

```
SAAT SIMPAN (UpdateHargaAverage):
  hargaLama  = HPP tbl_barang sebelum faktur ini
  hargaBaru  = harga satuan terkecil faktur ini
  HPP_baru   = weighted average
  → tbl_barang.HARGA_BELI          = HPP_baru
  → pembelian_detail.HARGA_AVERAGE = hargaLama  ← snapshot HPP sebelum faktur ini

SAAT HAPUS (Hapusbelanja — kedua versi):
  Baca HARGA_AVERAGE dari DGVDetail
  UPDATE tbl_barang SET HARGA_BELI = HARGA_AVERAGE
  → HPP dikembalikan ke nilai sebelum faktur ini masuk
```

---

## 5. Masalah yang Diketahui

**HPP setelah hapus tidak akurat jika faktur yang dihapus bukan yang terakhir.**

`Hapusbelanja` mengembalikan `HARGA_BELI` ke `HARGA_AVERAGE` (HPP sebelum faktur ini masuk). Ini hanya benar jika faktur yang dihapus adalah faktur terakhir secara kronologis. Jika ada faktur lain sesudahnya yang masih ada, HPP seharusnya di-recalculate dari semua faktur yang tersisa.

Akibatnya: `HARGA_BELI × STOK_TOKO` di `tbl_barang` tidak sama dengan `SALDO_AKHIR` akun persediaan di `tbl_datareferensi`.

---

## 6. Alur Lengkap `SimpanTransaksi` di FormPembelian

**Entry point:** `BtnSimpann_Click` → `SimpanTransaksi()`

### Validasi Awal (sebelum transaction)

```
- Jika bayar = 0 → konfirmasi user (lanjut sebagai hutang semua?)
- Jika status hutang → wajib ada supplier + jatuh tempo > tanggal transaksi
- Jika tidak izinkan tanggal lampau + mode tambah → paksa tanggal = Now + generate nomor baru
```

### Dalam Transaction

```
LANGKAH 1 — Simpan akun lama (hanya mode edit)
  Kumpulkan akunTerlibatLama dari JurnalUmum WHERE NO_TRANSAKSI = faktur
  → WAJIB sebelum Hapusbelanja karena setelah delete tidak bisa dibaca

LANGKAH 2 — Hapus data lama (hanya mode edit)
  CatatAudit (Audit Trail)
  Hapusbelanja(transaction)
  → update HPP + kurangi stok counter + HitungStokPerubahan + delete semua data lama

LANGKAH 3 — Simpan data baru
  SimpanPembelian(transaction)         → INSERT header ke tbl_pembelian
  SimpanPembelianDetail(transaction)   → INSERT detail + UpdateHargaAverage per barang
  HistoryBarang(transaction)           → INSERT ke HistoryBarang

LANGKAH 4 — Jurnal
  Simpanjurnal(transaction)
  → J1: K Kas Tunai
  → J2: K Bank Transfer
  → J3: K Hutang Belanja
  → J4: K Diskon Supplier
  → J5: D Persediaan Barang  (nilai = _subtotalBarang)
  → J6: D PPN Masukan
  → J7: D Biaya Kirim

LANGKAH 5 — Recalculate stok fisik (data baru)
  Loop DgvData → HitungStokPerubahan per barang → kumpulkan auditStokDelta

LANGKAH 6 — Kumpulkan akun baru
  Kumpulkan akunTerlibatBaru dari JurnalUmum WHERE NO_TRANSAKSI = faktur (baru)

LANGKAH 7 — Update saldo akun
  Gabungkan akunTerlibatLama + akunTerlibatBaru
  UpdateSaldoAkun per akun → update tbl_datareferensi.SALDO_AKHIR
  UpdateHutangSupliyer

LANGKAH 8 — Selesai
  HapusDraftPembelian (jika ada draft)
  AuditStokTransaksi
  transaction.Commit()
```

### Dua DGV yang Berbeda

| DGV | Dipakai di langkah | Isi |
|---|---|---|
| `FormUtama.DGVDetail` | Langkah 2 (Hapusbelanja) | Data lama dari DB — diisi saat user klik baris di FormUtama |
| `DgvData` (FormPembelian) | Langkah 3, 4, 5 | Data baru yang sedang diedit user |

### Fokus: `SimpanPembelianDetail` → `UpdateHargaAverage`

Per baris `DgvData`:
```
HargaBeli   = row.Cells("HargaBeli")    → harga per satuan beli
Isi         = row.Cells("Isi")          → isi per satuan beli
HargaSatuan = HargaBeli / Isi           → harga per satuan terkecil → hargaBaru
HargaAverage = row.Cells("Average")    → HPP lama snapshot → hargaLama
QtySat      = row.Cells("QtySat")       → qty satuan terkecil

INSERT pembelian_detail:
  HARGA_BELI         = HargaBeli        (harga per satuan beli)
  HARGA_AVERAGE      = HargaAverage     (HPP lama = snapshot sebelum faktur ini)
  HARGA_BELI_SATUAN  = HargaSatuan      (harga per satuan terkecil)
  QTY_SAT            = QtySat

UpdateHargaAverage(idBarang, HargaSatuan, HargaAverage, QtySat, stokField, transaction)
  → HARGA_BELI di tbl_barang = weighted average baru
  → HARGA_BELI_TERAKHIR = HargaSatuan
  → PEMBELIAN_x += QtySat
```

### Nilai Kritis yang Tersimpan di pembelian_detail

```
HARGA_AVERAGE = HPP tbl_barang SEBELUM faktur ini masuk
              = nilai yang akan dipakai Hapusbelanja untuk mengembalikan HPP
```

---

## 7. Hubungan Simpan → Hapus

```
SAAT SIMPAN:
  UpdateHargaAverage menyimpan HPP_lama ke pembelian_detail.HARGA_AVERAGE
  tbl_barang.HARGA_BELI = HPP_baru (weighted average)

SAAT HAPUS:
  Hapusbelanja membaca HARGA_AVERAGE dari DGVDetail
  UPDATE tbl_barang SET HARGA_BELI = HARGA_AVERAGE
  → HPP dikembalikan ke nilai sebelum faktur ini masuk

MASALAH:
  Ini hanya benar jika faktur yang dihapus adalah yang TERAKHIR secara kronologis.
  Jika ada faktur lain sesudahnya yang masih ada:
  → HPP seharusnya = recalculate dari semua faktur yang tersisa
  → Tapi yang terjadi = HPP dikembalikan ke snapshot lama
  → HARGA_BELI × STOK ≠ SALDO_AKHIR persediaan di tbl_datareferensi → GAP
```

---

## 8. Temuan: Hapus Harus Membalik Persis Semua yang Dilakukan Simpan

### Prinsip Dasar

`FormUtama.Hapusbelanja()` dan `FormPembelian.Hapusbelanja(transaction)` adalah **operasi bisnis yang sama** — membalik semua efek satu faktur pembelian. Perbedaan hanya pada konteks pemanggilan, bukan isi operasi.

### Pemetaan Simpan → Hapus

| # | Saat Simpan | Saat Hapus | Status |
|---|---|---|---|
| 1 | `SimpanPembelian` → INSERT `pembelian` | DELETE `pembelian` | ✅ Sudah ada di keduanya |
| 2 | `SimpanPembelianDetail` → INSERT `pembelian_detail` | DELETE `pembelian_detail` | ✅ Sudah ada di keduanya |
| 3 | `HistoryBarang` → INSERT `HistoryBarang` | DELETE `HistoryBarang` | ✅ Sudah ada di keduanya |
| 4 | `Simpanjurnal` → INSERT `JurnalUmum` | DELETE `JurnalUmum` + `UpdateSaldoAkun` | ✅ Ada, tapi `UpdateSaldoAkun` hanya di FormUtama |
| 5 | `UpdateHargaAverage` → HPP baru (weighted average) | Kembalikan HPP ke nilai yang benar | ❌ **Belum benar** — hanya kembalikan ke snapshot lama |
| 6 | `HitungStokPerubahan` (+) → stok bertambah | `HitungStokPerubahan` (-) → stok berkurang | ⚠️ Ada, tapi urutan berbeda antara keduanya |
| 7 | `UpdateHutangSupliyer` + INSERT `hutang_detail BELI` | DELETE `hutang_detail BELI` | ✅ Sudah ada di keduanya |

### Perbedaan yang Tidak Seharusnya Ada

| Aspek | FormUtama | FormPembelian | Seharusnya |
|---|---|---|---|
| `UpdateSaldoAkun` | ✅ Dipanggil di sini | ❌ Tidak dipanggil (di caller) | Harus ada di keduanya atau konsisten |
| Cek hutang BAYAR | ❌ Tidak ada | ✅ Ada | Harus sama |
| Urutan `HitungStokPerubahan` vs DELETE | Setelah DELETE | Sebelum DELETE | Harus sama |
| Loop DGVDetail | Dua kali terpisah | Satu kali gabung | Harus sama |
| Audit Trail | ✅ Ada | ❌ Tidak ada (di caller) | Harus konsisten |

### Masalah Utama yang Harus Diselesaikan

**Masalah #5 — HPP setelah hapus tidak akurat:**

Saat simpan, `UpdateHargaAverage` menghitung HPP baru dengan rumus:
```
HPP_baru = (HPP_lama × stok_lama + harga_baru × qty_baru) / (stok_lama + qty_baru)
```
Dan menyimpan `HPP_lama` ke `pembelian_detail.HARGA_AVERAGE` sebagai snapshot.

Saat hapus, kode saat ini hanya melakukan:
```
tbl_barang.HARGA_BELI = pembelian_detail.HARGA_AVERAGE  ← snapshot HPP sebelum faktur ini
```

Ini hanya benar jika faktur yang dihapus adalah **faktur terakhir secara kronologis**.
Jika ada faktur lain sesudahnya yang masih ada, HPP harus di-recalculate dari semua faktur yang tersisa.

Akibatnya:
```
tbl_barang.HARGA_BELI × STOK  ≠  tbl_datareferensi.SALDO_AKHIR (akun persediaan)
```

### Langkah Selanjutnya

Sebelum menulis kode apapun, perlu disepakati:

1. Bagaimana cara recalculate HPP yang benar setelah hapus?
2. Di mana logika recalculate ini ditempatkan — VB atau SP?
3. Apakah kedua `Hapusbelanja` akan digabung menjadi satu fungsi bersama, atau tetap dua fungsi tapi disamakan isinya?

---

## 9. Keputusan Arsitektur

### Jawaban 3 Pertanyaan

| # | Pertanyaan | Keputusan |
|---|---|---|
| 1 | Bagaimana recalculate HPP? | Di VB — loop semua `pembelian_detail` yang tersisa, hitung weighted average dari awal |
| 2 | Di mana logika ditempatkan? | `0Form/ModuleHapusTransaksi.vb` — semua proses hapus dan recalculate |
| 3 | Digabung atau tetap dua fungsi? | Digabung — satu fungsi di Module, dipanggil dari kedua tempat |

### Struktur `ModuleHapusTransaksi.vb`

Semua proses hapus per jenis transaksi dipisah dengan Region yang jelas:

```vb
Module ModuleHapusTransaksi

    #Region "HAPUS PEMBELIAN"
        ' - HapusPembelian(faktur, lokasi, transaction)
        ' - RecalculateHppSetelahHapus(kodeBarang, fakturDikecualikan, lokasi, transaction)
    #End Region

    #Region "HAPUS PENJUALAN"
        ' ...
    #End Region

    ' dst per jenis transaksi

End Module
```

### Alur yang Akan Diimplementasikan

```
HapusPembelian(faktur, lokasi, transaction):

  Per barang di pembelian_detail faktur ini:
    1. Kurangi PEMBELIAN_TOKO/GUDANG (stok counter)
    2. RecalculateHppSetelahHapus → hitung HPP baru dari semua faktur tersisa
    3. HitungStokPerubahan → update STOK_TOKO/GUDANG

  Kumpulkan akunTerlibat dari JurnalUmum SEBELUM delete
  Cek hutang_detail JENIS='BAYAR' → konfirmasi user jika ada
  DELETE hutang_detail JENIS='BELI'
  DELETE pembelian, pembelian_detail, JurnalUmum, HistoryBarang
  UpdateSaldoAkun untuk semua akun terlibat
  AuditStokTransaksi
```

### RecalculateHppSetelahHapus — Logika

```
Input : kodeBarang, fakturDikecualikan, lokasi

1. Baca semua pembelian_detail WHERE ID_BARANG = kodeBarang
   AND LOKASI = lokasi
   AND FAKTUR_BELI <> fakturDikecualikan
   ORDER BY TGL_BELI ASC

2. Loop dari awal, hitung weighted average:
   stokRunning = 0, hppRunning = 0
   Per baris:
     hppRunning = (hppRunning × stokRunning + HARGA_BELI_SATUAN × QTY_SAT)
                  / (stokRunning + QTY_SAT)
     stokRunning += QTY_SAT

3. UPDATE tbl_barang SET HARGA_BELI = hppRunning
```

### Pemanggil Setelah Refactor

```
FormUtama.Hapusbelanja()
  → ModuleHapusTransaksi.HapusPembelian(faktur, lokasi, transaction)
  → transaction.Commit() tetap di FormUtama

FormPembelian.Hapusbelanja(transaction)
  → ModuleHapusTransaksi.HapusPembelian(faktur, lokasi, transaction)
  → transaction.Commit() tetap di caller (SimpanTransaksi)
```

### Yang Akan Dihapus / Digantikan

| File | Yang Digantikan |
|---|---|
| `0Form/FormUtama.vb` | Isi `Hapusbelanja()` → diganti panggil `ModuleHapusTransaksi.HapusPembelian` |
| `2Trans/FormPembelian.vb` | Isi `Hapusbelanja(transaction)` → diganti panggil `ModuleHapusTransaksi.HapusPembelian` |

---

## 10. Data Baseline untuk Pengujian

> Kondisi awal sebelum transaksi apapun dimulai.
> Semua transaksi, jurnal, dan history sudah dikosongkan.

### Barang Uji — Kategori Rokok

| Field | ROK-000001 | ROK-000002 |
|---|---|---|
| Nama | 1 Alami Dua Ribu 12 | 2 Dji Sam Sue Kretek |
| HARGA_BELI (HPP awal) | 15.000 | 20.000 |
| HARGA_BELI_TERAKHIR | 15.000 | 20.000 |
| STOK_TOKO | 0 | 0 |
| PEMBELIAN_TOKO | 0 | 0 |
| PENJUALAN_TOKO | 0 | 0 |
| Satuan kecil | Bungkus (isi 1) | Bungkus (isi 1) |
| Satuan sedang | Slop (isi 10) | Slop (isi 10) |
| Satuan besar | Bal (isi 100) | Bal (isi 100) |
| Barcode kecil | 8992100114379 | 8999909028234 |

### Akun Persediaan Baseline

| KODE_AKUN | NAMA_AKUN | SALDO_AWAL | S_DEBET | S_KREDIT | SALDO_AKHIR |
|---|---|---|---|---|---|
| 01.04.001 | PERSEDIAAN BARANG | 0 | 0 | 0 | 0 |

### Kondisi Database

| Tabel | Jumlah Record |
|---|---|
| pembelian | 0 |
| pembelian_detail | 0 |
| JurnalUmum | 0 |
| historybarang | 0 |

**Kondisi siap untuk pengujian dari nol.**

---

## 11. Baseline Data 4 Faktur — Sebelum Hapus

> Snapshot lengkap kondisi database setelah PB-001 s/d PB-004 tersimpan.
> Dipakai sebagai acuan verifikasi saat hapus — apakah nilai kembali ke kondisi yang benar.

---

### A. Pergerakan HPP per Faktur (Fokus Utama)

Semua nilai dalam **satuan terkecil (Bungkus)**. `HARGA_AVERAGE` = HPP tbl_barang **sebelum** faktur ini masuk.

#### ROK-000001 — 1 Alami Dua Ribu 12

| Faktur | Harga/Bal | Harga/Bungkus | Qty Bungkus | HARGA_AVERAGE (HPP sebelum) | HPP sesudah | Stok sesudah |
|---|---|---|---|---|---|---|
| Baseline | — | — | 0 | — | **15.000** | 0 |
| PB-2604270001 | 1.500.000 | 15.000 | 1.000 | 15.000 | **15.000** | 1.000 |
| PB-2604270002 | 1.600.000 | 16.000 | 500 | 15.000 | **15.333,33** | 1.500 |
| PB-2604270003 | 1.550.000 | 15.500 | 1.000 | 15.333,33 | **15.400** | 2.500 |
| PB-2604270004 | 1.700.000 | 17.000 | 10.000 | 15.400 | **16.680** | 12.500 |

#### ROK-000002 — 2 Dji Sam Sue Kretek

| Faktur | Harga/Bal | Harga/Bungkus | Qty Bungkus | HARGA_AVERAGE (HPP sebelum) | HPP sesudah | Stok sesudah |
|---|---|---|---|---|---|---|
| Baseline | — | — | 0 | — | **20.000** | 0 |
| PB-2604270001 | 2.000.000 | 20.000 | 1.000 | 20.000 | **20.000** | 1.000 |
| PB-2604270002 | 2.200.000 | 22.000 | 500 | 20.000 | **20.666,67** | 1.500 |
| PB-2604270003 | 2.100.000 | 21.000 | 1.000 | 20.666,67 | **20.800** | 2.500 |
| PB-2604270004 | 2.400.000 | 24.000 | 10.000 | 20.800 | **23.360** | 12.500 |

**Catatan penting:** `HARGA_AVERAGE` di `pembelian_detail` adalah snapshot HPP `tbl_barang` **sebelum** faktur itu masuk. Inilah nilai yang dipakai `Hapusbelanja` untuk mengembalikan HPP.

---

### B. Kondisi tbl_barang Sekarang

| ID_BARANG | HARGA_BELI (HPP) | HARGA_BELI_TERAKHIR | STOK_TOKO | PEMBELIAN_TOKO |
|---|---|---|---|---|
| ROK-000001 | 16.680,00 | 17.000,00 | 12.500 | 12.500 |
| ROK-000002 | 23.360,00 | 24.000,00 | 12.500 | 12.500 |

---

### C. Jurnal per Faktur

#### PB-2604270001
| Akun D | Akun K | Nominal |
|---|---|---|
| 01.04.001 Persediaan | — | 35.000.000 |
| 06.02.001 Biaya Kirim | — | 300.000 |
| 01.05.001 PPN Masukan | — | 200.000 |
| — | 01.01.001 Kas Toko | 10.000.000 |
| — | 01.02.001 Transfer Bank | 20.000.000 |
| — | 03.01.001 Hutang Belanja | 5.400.000 |
| — | 06.05.001 Diskon Pembelian | 100.000 |
| **Total D = 35.500.000** | **Total K = 35.500.000** | ✅ |

#### PB-2604270002
| Akun D | Akun K | Nominal |
|---|---|---|
| 01.04.001 Persediaan | — | 19.000.000 |
| — | 01.01.001 Kas Toko | 19.000.000 |
| **Total D = 19.000.000** | **Total K = 19.000.000** | ✅ |

#### PB-2604270003
| Akun D | Akun K | Nominal |
|---|---|---|
| 01.04.001 Persediaan | — | 36.500.000 |
| 06.02.001 Biaya Kirim | — | 300.000 |
| 01.05.001 PPN Masukan | — | 20.000 |
| — | 01.01.001 Kas Toko | 10.000.000 |
| — | 01.02.001 Transfer Bank | 20.000.000 |
| — | 03.01.001 Hutang Belanja | 6.810.000 |
| — | 06.05.001 Diskon Pembelian | 10.000 |
| **Total D = 36.820.000** | **Total K = 36.820.000** | ✅ |

#### PB-2604270004
| Akun D | Akun K | Nominal |
|---|---|---|
| 01.04.001 Persediaan | — | 410.000.000 |
| 01.05.001 PPN Masukan | — | 2.000.000 |
| 06.02.001 Biaya Kirim | — | 300.000 |
| — | 01.01.001 Kas Toko | 100.000.000 |
| — | 01.02.001 Transfer Bank | 300.000.000 |
| — | 03.01.001 Hutang Belanja | 11.300.000 |
| — | 06.05.001 Diskon Pembelian | 1.000.000 |
| **Total D = 412.300.000** | **Total K = 412.300.000** | ✅ |

---

### D. Saldo tbl_datareferensi Sekarang

| KODE_AKUN | NAMA_AKUN | S_DEBET | S_KREDIT | SALDO_AKHIR |
|---|---|---|---|---|
| 01.01.001 | KAS DI TOKO | 0 | 139.000.000 | -139.000.000 |
| 01.02.001 | TRANSFER BANK | 0 | 340.000.000 | -340.000.000 |
| 01.04.001 | PERSEDIAAN BARANG | 500.500.000 | 0 | **500.500.000** |
| 01.05.001 | PPN MASUKAN | 2.220.000 | 0 | 2.220.000 |
| 03.01.001 | HUTANG BELANJA | 0 | 23.510.000 | 23.510.000 |
| 06.02.001 | BIAYA KIRIM PEMBELIAN | 900.000 | 0 | 900.000 |
| 06.05.001 | POTONGAN DISKON PEMBELIAN | 0 | 1.110.000 | 1.110.000 |

---

### E. Verifikasi Konsistensi Sekarang

```
HARGA_BELI × STOK_TOKO:
  ROK-000001: 16.680 × 12.500 = 208.500.000
  ROK-000002: 23.360 × 12.500 = 292.000.000
  Total      = 500.500.000

SALDO_AKHIR 01.04.001 = 500.500.000 ✅ SEIMBANG
```

---

### F. Yang Harus Dikembalikan saat Hapus (Acuan Verifikasi)

Tabel ini adalah **acuan utama** — setelah hapus satu faktur, nilai harus kembali ke baris sebelumnya.

#### Jika hapus PB-2604270004 (faktur terakhir):

| | ROK-000001 | ROK-000002 |
|---|---|---|
| HPP harus kembali ke | **15.400** | **20.800** |
| Stok harus kembali ke | **2.500** | **2.500** |
| PEMBELIAN_TOKO harus kembali ke | **2.500** | **2.500** |
| Saldo 01.04.001 harus kembali ke | **90.500.000** | (gabungan) |
| Nilai aktual harus = | 15.400×2.500 + 20.800×2.500 = **90.500.000** | ✅ |

#### Jika hapus PB-2604270003 (faktur tengah):

| | ROK-000001 | ROK-000002 |
|---|---|---|
| HPP harus recalculate dari PB-001+PB-002+PB-004 | **(15.000×1.000 + 16.000×500 + 17.000×10.000) / 11.500 = 16.826,09** | **(20.000×1.000 + 22.000×500 + 24.000×10.000) / 11.500 = 23.565,22** |
| Stok harus kembali ke | **11.500** | **11.500** |
| Saldo 01.04.001 harus kembali ke | **500.500.000 - 36.500.000 = 464.000.000** | |
| Nilai aktual harus = | 16.826,09×11.500 + 23.565,22×11.500 ≈ **464.000.000** | ✅ |

#### Jika hapus PB-2604270002 (faktur tengah):

| | ROK-000001 | ROK-000002 |
|---|---|---|
| HPP harus recalculate dari PB-001+PB-003+PB-004 | **(15.000×1.000 + 15.500×1.000 + 17.000×10.000) / 12.000 = 16.791,67** | **(20.000×1.000 + 21.000×1.000 + 24.000×10.000) / 12.000 = 23.750** |
| Stok harus kembali ke | **12.000** | **12.000** |
| Saldo 01.04.001 harus kembali ke | **500.500.000 - 19.000.000 = 481.500.000** | |

#### Jika hapus PB-2604270001 (faktur pertama):

| | ROK-000001 | ROK-000002 |
|---|---|---|
| HPP harus recalculate dari PB-002+PB-003+PB-004 | **(16.000×500 + 15.500×1.000 + 17.000×10.000) / 11.500 = 16.804,35** | **(22.000×500 + 21.000×1.000 + 24.000×10.000) / 11.500 = 23.608,70** |
| Stok harus kembali ke | **11.500** | **11.500** |
| Saldo 01.04.001 harus kembali ke | **500.500.000 - 35.000.000 = 465.500.000** | |


---

## 12. Baseline Data 5 Faktur — Test Sesi 2 (Kode Baru ModuleHapusTransaksi)

> Snapshot kondisi database setelah PB-2604270001 s/d PB-2604270005 tersimpan.
> Dipakai sebagai acuan verifikasi saat hapus dengan kode baru.
> Semua faktur di lokasi TOKO, supplier PT Gudang Garam Tbk.

---

### A. Header Faktur

| Faktur | Grand Total | Tunai | Transfer | Hutang | Status |
|---|---|---|---|---|---|
| PB-2604270001 | 350.000.000 | 350.000.000 | 0 | 0 | Lunas |
| PB-2604270002 | 175.000.000 | 175.000.000 | 0 | 0 | Lunas |
| PB-2604270003 | 61.000.000 | 61.000.000 | 0 | 0 | Lunas |
| PB-2604270004 | 380.000.000 | 380.000.000 | 0 | 0 | Lunas |
| PB-2604270005 | 1.110.000.000 | 500.000.000 | 400.000.000 | 210.000.000 | Belum Lunas |

---

### B. Detail per Faktur — Pergerakan HPP

Semua nilai dalam satuan terkecil (Bungkus). `HARGA_AVERAGE` = HPP tbl_barang **sebelum** faktur itu masuk.

#### ROK-000001 — 1 Alami Dua Ribu 12

| Faktur | Satuan | Qty Beli | Isi | QTY_SAT | Harga/Satuan Beli | Harga/Bungkus | HARGA_AVERAGE (HPP sebelum) | HPP sesudah | Stok sesudah |
|---|---|---|---|---|---|---|---|---|---|
| Baseline | — | — | — | 0 | — | — | — | **15.000** | 0 |
| PB-2604270001 | Bal | 100 | 100 | 10.000 | 1.500.000 | 15.000 | 15.000 | **15.000** | 10.000 |
| PB-2604270002 | Bal | 50 | 100 | 5.000 | 1.500.000 | 15.000 | 15.000 | **15.000** | 15.000 |
| PB-2604270003 | Slop | 200 | 10 | 2.000 | 180.000 | 1.800 | 15.000 | **15.352,94** | 17.000 |
| PB-2604270004 | Bal | 100 | 100 | 10.000 | 1.600.000 | 16.000 | 15.352,94 | **15.592,59** | 27.000 |
| PB-2604270005 | Bal | 150 | 100 | 15.000 | 1.800.000 | 18.000 | 15.592,59 | **16.452,38** | 42.000 |

#### ROK-000002 — 2 Dji Sam Sue Kretek

| Faktur | Satuan | Qty Beli | Isi | QTY_SAT | Harga/Satuan Beli | Harga/Bungkus | HARGA_AVERAGE (HPP sebelum) | HPP sesudah | Stok sesudah |
|---|---|---|---|---|---|---|---|---|---|
| Baseline | — | — | — | 0 | — | — | — | **20.000** | 0 |
| PB-2604270001 | Bal | 100 | 100 | 10.000 | 2.000.000 | 20.000 | 20.000 | **20.000** | 10.000 |
| PB-2604270002 | Bal | 50 | 100 | 5.000 | 2.000.000 | 20.000 | 20.000 | **20.000** | 15.000 |
| PB-2604270003 | Slop | 100 | 10 | 1.000 | 250.000 | 2.500 | 20.000 | **20.312,50** | 16.000 |
| PB-2604270004 | Bal | 100 | 100 | 10.000 | 2.200.000 | 22.000 | 20.312,50 | **20.961,54** | 26.000 |
| PB-2604270005 | Bal | 300 | 100 | 30.000 | 2.800.000 | 28.000 | 20.961,54 | **24.732,14** | 56.000 |

---

### C. Kondisi tbl_barang Sekarang

| ID_BARANG | HARGA_BELI (HPP) | HARGA_BELI_TERAKHIR | STOK_TOKO | PEMBELIAN_TOKO |
|---|---|---|---|---|
| ROK-000001 | 16.452,38 | 18.000 | 42.000 | 42.000 |
| ROK-000002 | 24.732,14 | 28.000 | 56.000 | 56.000 |

---

### D. Jurnal per Faktur

| Faktur | Akun D | Akun K | Nominal |
|---|---|---|---|
| PB-2604270001 | 01.04.001 Persediaan | — | 350.000.000 |
| PB-2604270001 | — | 01.01.001 Kas Toko | 350.000.000 |
| PB-2604270002 | 01.04.001 Persediaan | — | 175.000.000 |
| PB-2604270002 | — | 01.01.001 Kas Toko | 175.000.000 |
| PB-2604270003 | 01.04.001 Persediaan | — | 61.000.000 |
| PB-2604270003 | — | 01.01.001 Kas Toko | 61.000.000 |
| PB-2604270004 | 01.04.001 Persediaan | — | 380.000.000 |
| PB-2604270004 | — | 01.01.001 Kas Toko | 380.000.000 |
| PB-2604270005 | 01.04.001 Persediaan | — | 1.110.000.000 |
| PB-2604270005 | — | 01.01.001 Kas Toko | 500.000.000 |
| PB-2604270005 | — | 01.02.001 Transfer Bank | 400.000.000 |
| PB-2604270005 | — | 03.01.001 Hutang Belanja | 210.000.000 |

---

### E. Saldo tbl_datareferensi Sekarang

| KODE_AKUN | NAMA_AKUN | S_DEBET | S_KREDIT | SALDO_AKHIR |
|---|---|---|---|---|
| 01.01.001 | KAS DI TOKO | 0 | 1.466.000.000 | -1.466.000.000 |
| 01.02.001 | TRANSFER BANK | 0 | 400.000.000 | -400.000.000 |
| 01.04.001 | PERSEDIAAN BARANG | 2.076.000.000 | 0 | **2.076.000.000** |
| 03.01.001 | HUTANG BELANJA | 0 | 210.000.000 | 210.000.000 |

---

### F. Verifikasi Konsistensi Sekarang

```
HARGA_BELI × STOK_TOKO:
  ROK-000001: 16.452,38 × 42.000 = 690.999.960
  ROK-000002: 24.732,14 × 56.000 = 1.384.999.840
  Total tbl_barang               = 2.075.999.800

SALDO_AKHIR 01.04.001            = 2.076.000.000
Selisih (akumulasi rounding)     = 200
```

> Selisih 200 dari akumulasi rounding `Decimal(15,2)` sebelum migrasi ke `Decimal(15,4)`.
> Faktur berikutnya akan menggunakan presisi 4 desimal sehingga selisih tidak bertambah.

---

### G. Acuan Verifikasi — Hapus PB-003

Skenario: hapus PB-2604270003, faktur tersisa = PB-001 + PB-002 + PB-004 + PB-005.

**Data PB-003 yang akan dihapus:**
- Faktur: PB-2604270003
- Supplier: PT Gudang Garam Tbk
- Grand Total: 61.000.000
- Pembayaran Tunai: 61.000.000
- Tagihan: 0

Detail barang:
| Barang | Harga Beli | Qty | Satuan | Isi | Qty Sat | Total |
|---|---|---|---|---|---|
| 1 Alami Dua Ribu 12 | 180.000 | 200 | Slop | 10 | 2.000 | 36.000.000 |
| 2 Dji Sam Sue Kretek | 250.000 | 100 | Slop | 10 | 1.000 | 25.000.000 |

**Kondisi SEBELUM hapus:**
| | ROK-000001 | ROK-000002 |
|---|---|---|
| STOK_TOKO | 52.000 | 61.000 |
| HARGA_BELI_TERAKHIR | 18.000 | 28.000 |
| Saldo 01.01.001 (KAS DI TOKO) | -1.966.000.000 | |
| Saldo 01.04.001 (PERSEDIAAN) | 2.204.850.016 | |
| Saldo 03.01.001 (HUTANG BELANJA) | 30.000.000 | |

**HPP recalculate (weighted average dari faktur tersisa, titik awal = HARGA_AVERAGE PB-001):**

ROK-000001:
```
Titik awal (HARGA_AVERAGE PB-001) = 15.000, stok = 0
+ PB-001: (15.000×0 + 15.000×10.000) / 10.000    = 15.000,  stok = 10.000
+ PB-002: (15.000×10.000 + 15.000×5.000) / 15.000 = 15.000,  stok = 15.000
+ PB-004: (15.000×15.000 + 16.000×10.000) / 25.000 = 15.400, stok = 25.000
+ PB-005: (15.400×25.000 + 18.000×25.000) / 50.000 = 16.700, stok = 50.000
HPP seharusnya = 16.700
```

ROK-000002:
```
Titik awal (HARGA_AVERAGE PB-001) = 20.000, stok = 0
+ PB-001: (20.000×0 + 20.000×10.000) / 10.000     = 20.000,  stok = 10.000
+ PB-002: (20.000×10.000 + 20.000×5.000) / 15.000  = 20.000,  stok = 15.000
+ PB-004: (20.000×15.000 + 22.000×10.000) / 25.000 = 20.800,  stok = 25.000
+ PB-005: (20.800×25.000 + 28.000×35.000) / 60.000 = 25.000,  stok = 60.000
HPP seharusnya = 25.000
```

**Kondisi SETELAH hapus PB-003:**

| | ROK-000001 | ROK-000002 |
|---|---|---|
| HPP harus = | **16.700** | **25.000** |
| Stok harus = | **50.000** | **60.000** |
| PEMBELIAN_TOKO harus = | **50.000** | **60.000** |
| HARGA_BELI_TERAKHIR harus = | **18.000** (dari PB-005) | **28.000** (dari PB-005) |

**Nilai persediaan baru (HPP × Stok):**
```
ROK-000001: 16.700 × 50.000 =   835.000.000
ROK-000002: 25.000 × 60.000 = 1.500.000.000
Total                        = 2.335.000.000
```

**Jurnal penyesuaian moving average yang diharapkan muncul:**
```
Selisih = Nilai persediaan baru - Saldo jurnal setelah hapus PB-003
        = 2.335.000.000 - (2.204.850.016 - 61.000.000)
        = 2.335.000.000 - 2.143.850.016
        = 191.149.984

Jurnal:
  D: 01.04.001 PERSEDIAAN BARANG       191.149.984
  K: 06.04.002 PENYESUAIAN HARGA POKOK 191.149.984
  NO_TRANSAKSI = PB-2604270003
  URAIAN = "Penyesuaian HPP moving average — hapus faktur PB-2604270003 ..."
```

**Saldo akun setelah hapus:**
| Akun | Saldo Sebelum | Perubahan | Saldo Setelah |
|---|---|---|---|
| 01.01.001 (KAS DI TOKO) | -1.966.000.000 | +61.000.000 | -1.905.000.000 |
| 01.04.001 (PERSEDIAAN) | 2.204.850.016 | -61.000.000 + 191.149.984 | 2.335.000.000 |
| 03.01.001 (HUTANG BELANJA) | 30.000.000 | 0 (PB-003 lunas tunai) | 30.000.000 |
| 06.04.002 (PENYESUAIAN HARGA POKOK) | 0 | +191.149.984 (K) | 191.149.984 (K) |

**Verifikasi keseimbangan:**
```
Nilai tbl_barang = 16.700×50.000 + 25.000×60.000 = 2.335.000.000
Saldo 01.04.001  = 2.335.000.000
✅ Seimbang
```

**Catatan:**
- PB-003 menggunakan satuan "Slop" (isi 10), bukan "Bal" (isi 100) seperti faktur lainnya
- Qty Sat PB-003: ROK-000001 = 2.000, ROK-000002 = 1.000
- Setelah hapus, stok berkurang sesuai Qty Sat yang dihapus
- HARGA_BELI_TERAKHIR tetap 18.000 dan 28.000 karena PB-005 (faktur terakhir) tidak terhapus



---

## 13. Bug: HARGA_BELI_SATUAN Tersimpan Salah di pembelian_detail

> Ditemukan: 2026-04-27, saat analisa data PB-2604270005 setelah edit.

### Gejala

`pembelian_detail.HARGA_BELI_SATUAN` tersimpan nilai yang jauh lebih kecil dari yang benar:

| Faktur | Barang | HARGA_BELI_SATUAN aktual | Seharusnya |
|---|---|---|---|
| PB-2604270005 (setelah edit) | ROK-000001 | 180 | 18.000 |
| PB-2604270005 (setelah edit) | ROK-000002 | 280 | 28.000 |

`TOTAL` juga ikut salah karena menggunakan nilai yang sama:
- ROK-000001: 4.500.000 (seharusnya 450.000.000)
- ROK-000002: 9.800.000 (seharusnya 980.000.000)

### Yang Tidak Terpengaruh

- `tbl_barang.HARGA_BELI` (HPP) → **benar** — dihitung dari `HargaBeli / Isi` di `SimpanPembelianDetail`, bukan dari kolom DGV `HargaBeliSatKecil`
- `tbl_datareferensi.SALDO_AKHIR` → **benar** — jurnal menggunakan `TOTAL` dari DGV `Totalharga`, bukan dari `HARGA_BELI_SATUAN`
- `UpdateHargaAverage` → **benar** — pakai `HargaSatuan = HargaBeli / Isi` yang dihitung inline

### Akar Masalah

Di `SimpanPembelianDetail`:
```vb
Dim HargaBeliSatKecil = ModuleAngka.ParseDecimal(row.Cells("HargaBeliSatKecil").Value)
' ...
.AddWithValue("@HARGA_BELI_SATUAN", HargaBeliSatKecil)  ← dari kolom DGV
```

Kolom DGV `HargaBeliSatKecil` kemungkinan tidak terupdate dengan benar saat user mengubah qty atau satuan di mode edit — nilainya masih dari kondisi sebelumnya atau tidak ter-recalculate.

Sementara untuk `UpdateHargaAverage`, nilai yang dipakai dihitung ulang secara inline:
```vb
Dim HargaSatuan = If(Isi = 0, 0, HargaBeli / Isi)  ← dihitung ulang, benar
UpdateHargaAverage(IdBarang, HargaSatuan, ...)
```

### Dampak

`HARGA_BELI_SATUAN` di `pembelian_detail` dipakai oleh:
1. `RecalculateHppSetelahHapus` di `ModuleHapusTransaksi` — **terdampak** jika nilai salah
2. Laporan yang membaca `HARGA_BELI_SATUAN` langsung dari tabel

### Status

- Bug ini **sudah diperbaiki** di `FormPembelian.vb`
- Akar masalah: di handler `CellEnter` saat satuan dipilih, `HargaBeliSatKecil` diisi dengan `hargaBeli` (= `HARGA_BELI` dari `tbl_barang` = HPP per satuan terkecil) bukan `hargaBeliterakhir` (= harga beli terakhir per satuan terkecil)
- Perbaikan: ubah satu baris di region DGV EVENT HANDLERS:
  ```vb
  ' Sebelum (salah):
  DgvData("HargaBeliSatKecil", rowIndex).Value = hargaBeli
  ' Sesudah (benar):
  DgvData("HargaBeliSatKecil", rowIndex).Value = hargaBeliterakhir
  ```
- Bug ini hanya terjadi saat **simpan pertama** (mode tambah) — saat load edit, nilai dihitung ulang dari DB sehingga tampil benar di form
- Data PB-005 yang sudah tersimpan salah perlu di-edit ulang dari aplikasi agar `HARGA_BELI_SATUAN` dan `TOTAL` di DB ikut terkoreksi

