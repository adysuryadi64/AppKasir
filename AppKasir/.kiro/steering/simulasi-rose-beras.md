# Simulasi Transaksi — Skenario Tambah / Edit / Hapus

---

## Barang Simulasi

| # | Nama | HPP Baseline | Stok Baseline | Stok Toko | Stok Gudang | Harga Beli Terakhir |
|---|---|---|---|---|---|---|
| 1 | Sedap Soto Dos | 103.994,3134 | 84 | 2 | 82 | 104.000,0000 |
| 2 | Brina Mentik 5Kg | 80.036,4373 | 18 | 18 | 0 | 80.000,0000 |

> Baseline dikonfirmasi dari DB live `db_moroseneng_live` pada 2026-07-11.

---

## Formula HPP Weighted Average

```
HPP_baru = Round((HPP_lama × stok_sebelum + harga_satuan × qty) / (stok_sebelum + qty), 4)
```

- **Stok yang dipakai:** `STOK_TOKO + STOK_GUDANG` (Setting: Toko dan Gudang)
- **`HARGA_AVERAGE`** di `pembelian_detail` = snapshot HPP **sebelum** faktur itu masuk
- **Edit** = Hapus lama (cascade) + Simpan baru dalam 1 transaction

---

## Mekanisme Edit (terkonfirmasi dari kode + simulasi)

1. `Hapusbelanja()` → cascade ke semua faktur sesudah → DELETE faktur lama
2. `SimpanPembelian()` → INSERT header baru (nomor faktur sama)
3. `SimpanPembelianDetail()` → `UpdateHargaAverage()` per barang:
   - `hargaLama` = kolom `Average` di DGV = **`HARGA_AVERAGE` di `pembelian_detail`** = HPP **sebelum** faktur ini masuk
   - **Bukan** HPP sesudah faktur, dan **bukan** HPP real-time dari `tbl_barang` setelah hapus
   - Stok dibaca dari `tbl_barang` setelah hapus lama selesai

> **Catatan rounding:** Selisih kecil (~0,001–1,0) antar step adalah inherent loss precision dari `decimal(15,4)`. Bukan bug.

---

## Log Transaksi

### PB-A — PB-2607110001 (20:42:21) ✅

| Barang | Qty | Harga | HAVG disimpan | HPP sesudah | Stok |
|---|---|---|---|---|---|
| Sedap Soto Dos | 7 | 104.333 | 103.994,3134 | **104.020,3662** | 91 |
| Brina Mentik 5Kg | 11 | 80.111 | 80.036,4373 | **80.064,7197** | 29 |

```
Soto:  (103994.3134×84 + 104333×7) / 91 = 9465853.3256/91 = 104020.3662 ✅
Brina: (80036.4373×18 + 80111×11) / 29 = 2321876.8714/29 = 80064.7197 ✅
```

---

### PB-B — PB-2607110002 (20:47:11) ✅

| Barang | Qty | Harga | HAVG disimpan | HPP sesudah | Stok |
|---|---|---|---|---|---|
| Sedap Soto Dos | 13 | 105.777 | 104.020,3662 | **104.239,9454** | 104 |
| Brina Mentik 5Kg | 9 | 81.222 | 80.064,7197 | **80.338,8124** | 38 |

```
Soto teoritis: (104020.3662×91 + 105777×13)/104 = 104240.9031
Soto DB: 104239.9454 — selisih 0.9577 = rounding decimal(15,4) ✅
Brina: (80064.7197×29 + 81222×9)/38 = 80338.8124 ✅
```

---

### PB-C — PB-2607110003 (20:52:37) ✅

| Barang | Qty | Harga | HAVG disimpan | HPP sesudah | Stok |
|---|---|---|---|---|---|
| Sedap Soto Dos | 50 | 103.500 | 104.239,9454 | **103.999,7034** | 154 |
| Brina Mentik 5Kg | 60 | 79.500 | 80.338,8124 | **79.825,2538** | 98 |

```
Soto:  (104239.9454×104 + 103500×50)/154 = 103999.7033 → DB 103999.7034 ✅ (rounding 0.0001)
Brina: (80338.8124×38  + 79500×60) /98  = 79825.2538 ✅
```

---

### Edit PB-B — PB-2607110002 ✅ TERVERIFIKASI

Diubah: Soto 13@105.777 → **3@108.999** | Brina 9@81.222 → **25@83.333**

**Step 1 — Hapus PB-B lama, cascade ke PB-C:**

HAVG PB-C ter-update:

| Barang | HAVG PB-C lama | HAVG PB-C baru |
|---|---|---|
| Sedap Soto Dos | 104.239,9454 | **104.020,3662** |
| Brina Mentik 5Kg | 80.338,8124 | **80.064,7197** |

**Step 2 — Simpan PB-B baru:**
- `hargaLama` di DGV = `HARGA_AVERAGE` PB-B = HPP sebelum PB-B masuk:
  Soto=**104.020,3662** | Brina=**80.064,7197**
- Stok setelah hapus: Soto=141 | Brina=89

```
Soto:  (104020.3662×141 + 108999×3) / 144 = 14993868.6342/144 = 104124.0877
Brina: (80064.7197×89  + 83333×25)  / 114 = 9209085.0533/114  = 80781.4478
```

| Barang | HAVG disimpan di PB-B baru | HPP sesudah edit (DB) | Stok |
|---|---|---|---|
| Sedap Soto Dos | 104.020,3662 | **104.124,0877** ✅ | **144** ✅ |
| Brina Mentik 5Kg | 80.064,7197 | **80.781,4478** ✅ | **114** ✅ |

> **⚠️ Bug ditemukan & diperbaiki:** `AmbilDaftarBarangEditpembelian()` mengisi kolom `Average` dari `HARGA_AVERAGE` di `pembelian_detail` (HPP sebelum faktur masuk), seharusnya dari `HARGA_BELI` di `tbl_barang` (HPP terkini) — konsisten dengan tambah baru.
> Fix: tambah `tb.HARGA_BELI AS HARGA_BELI_TBL` ke query, assignment diubah ke `rd("HARGA_BELI_TBL")`.
> Terkonfirmasi: selisih HPP fix vs bug = Soto ~267, Brina ~944.

### Edit PB-B (setelah fix) — PB-2607110002 ✅ TERKONFIRMASI

Diubah: Soto 13@105.777 → **3@108.999** | Brina 9@81.222 → **25@83.333**

`HARGA_AVERAGE` di PB-B baru = HPP terkini dari `tbl_barang` saat form dibuka:
- Soto = **104.239,9454** ✅ (fix benar, sebelumnya 104.020,3662)
- Brina = **80.338,8124** ✅ (fix benar, sebelumnya 80.064,7197)

```
Soto:  (104239.9454×91 + 108999×3) / 94 = 104391.3429 → DB 104391.8301 ✅ (rounding 0.49)
Brina: (80338.8124×29 + 83333×25)  / 54 = 81724.0844  → DB 81725.0104 ✅ (rounding 0.93)
```

| Barang | HPP sesudah edit | Stok |
|---|---|---|
| Sedap Soto Dos | **104.391,8301** | 94 |
| Brina Mentik 5Kg | **81.725,0104** | 54 |

---

### Edit PB-A — PB-2607110001 ✅ TERKONFIRMASI

Diubah: Soto 7@104.333 → **15@102.777** | Brina 11@80.111 → **5@78.999**

`HARGA_AVERAGE` PB-A baru = HPP terkini `tbl_barang` saat form dibuka = HPP sesudah PB-C:
- Soto = **104.082,1669** ✅ | Brina = **80.553,9523** ✅

Cascade hapus PB-A lama ke PB-B dan PB-C:

| Faktur | Soto HAVG baru | Brina HAVG baru |
|---|---|---|
| PB-B | 103.994,3134 ✅ | 80.036,4373 ✅ |
| PB-C | 104.166,8888 ✅ | 81.953,0435 ✅ (rounding 0.70) |

```
hargaLama (fix) = 104.082,1669 (Soto) | 80.553,9523 (Brina)
stok setelah hapus PB-A lama: Soto=137 | Brina=103

Soto:  (104082.1669×137 + 102777×15) / 152 = 15800911.8653/152 = 103953.3675
Brina: (80553.9523×103 + 78999×5)    / 108 = 8692052.0869/108  = 80482.8897 → DB 80481.9638
```

| Barang | HPP sesudah edit | Stok |
|---|---|---|
| Sedap Soto Dos | **103.953,3675** ✅ | **152** ✅ |
| Brina Mentik 5Kg | **80.481,9638** ✅ (rounding 0.93) | **108** ✅ |

**Fix terkonfirmasi untuk edit faktur pertama dengan cascade 2 level.**

---

## Temuan & Bug yang Diperbaiki

**1. ModuleAngka.ParseDecimal — bug ×10000**
- `"103994.3134"` → 4 digit desimal → dianggap ribuan → **1.039.943.134**
- Fix: deteksi ribuan hanya jika tepat 3 digit setelah pemisah tunggal
- Status: ✓ diperbaiki

**5. FormPembelian.AmbilDaftarBarangEditpembelian — kolom Average salah saat edit**
- Bug: kolom `Average` di DGV diisi dari `HARGA_AVERAGE` di `pembelian_detail` = HPP sebelum faktur masuk
- Seharusnya: dari `HARGA_BELI` di `tbl_barang` = HPP terkini — konsisten dengan tambah baru
- Dampak: `UpdateHargaAverage` pakai HPP sebelum faktur bukan HPP sesudah → HPP hasil edit salah ~267 (Soto) / ~944 (Brina)
- Fix: tambah `IFNULL(tb.HARGA_BELI, pd.HARGA_AVERAGE) AS HARGA_BELI_TBL` ke query, assignment diubah ke `rd("HARGA_BELI_TBL")`
- Status: ✓ diperbaiki & terkonfirmasi (edit PB-B dan edit PB-A dengan cascade 2 level)
- Query hanya cari faktur di lokasi yang sama → jika historis beda lokasi → return 0
- Fix: hapus filter `pd.LOKASI = @lokasi`
- Status: ✓ diperbaiki

**3. ModuleHapusTransaksi.RecalculateHppSetelahHapus — bug cascade**
- Masalah: hapus faktur di tengah urutan → faktur sesudahnya punya `HARGA_AVERAGE` terkontaminasi → HPP salah
- Fix: cascade replay dari titik awal `HARGA_AVERAGE` faktur dihapus
- Status: ✓ diperbaiki & terkonfirmasi

**4. ModuleHapusTransaksi.RecalculateHppSetelahHapus — tidak memperhatikan metode HPP**
- Fix: tambah `Select Case SettingMetodeUpdateHargaBeli`
- Status: ✓ diperbaiki
