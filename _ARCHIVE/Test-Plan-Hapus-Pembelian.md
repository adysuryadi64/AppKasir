# Test Plan: Hapus Pembelian (Setelah Perbaikan Bug)

> Dibuat: 2026-04-27
> Tujuan: Verifikasi perbaikan bug di `ModuleHapusTransaksi.HapusPembelian()`

---

## Bug yang Diperbaiki

### 1. HARGA_BELI_TERAKHIR Tidak Terupdate dengan Benar

**Sebelum:**
- Menggunakan `HARGA_BELI_SEBELUMNYA` dari faktur yang dihapus
- Ini adalah rollback untuk mode edit, bukan hapus permanen

**Sesudah:**
- Query `HARGA_BELI_SATUAN` dari faktur tersisa yang paling baru
- `ORDER BY TGL_BELI DESC, NO DESC LIMIT 1`

### 2. Jurnal Penyesuaian Salah Arah dan Nominal

**Sebelum:**
- Perhitungan per-barang: `(hppBaru - hppLama) × stok`
- `hppLama` ambigu (HPP sebelum atau sesudah faktur dihapus?)
- Logika if/else terbalik

**Sesudah:**
- Hitung total nilai persediaan: `SUM(HPP × Stok)` untuk semua barang terlibat
- Baca saldo 01.04.001 dari `tbl_datareferensi`
- Selisih = Nilai persediaan - Saldo jurnal
- Jika selisih > 0: D: 01.04.001, K: 06.04.002 (persediaan kurang dicatat)
- Jika selisih < 0: D: 06.04.002, K: 01.04.001 (persediaan lebih dicatat)

---

## Data Faktur yang Tersisa (Setelah PB-003 Dihapus)

| Faktur | Tanggal | Total | ROK-000001 | ROK-000002 |
|---|---|---|---|---|
| PB-2604270001 | 2026-04-27 17:05:15 | 350.000.000 | 100 Bal (10.000 pcs) @ 15.000 | 100 Bal (10.000 pcs) @ 20.000 |
| PB-2604270002 | 2026-04-27 17:06:13 | 175.000.000 | 50 Bal (5.000 pcs) @ 15.000 | 50 Bal (5.000 pcs) @ 20.000 |
| PB-2604270004 | 2026-04-27 17:08:21 | 380.000.000 | 100 Bal (10.000 pcs) @ 16.000 | 100 Bal (10.000 pcs) @ 22.000 |
| PB-2604270005 | 2026-04-27 17:09:26 | 1.430.000.000 | 250 Bal (25.000 pcs) @ 18.000 | 350 Bal (35.000 pcs) @ 28.000 |

**Kondisi Awal (Sebelum Test):**
- Stok ROK-000001: 50.000 pcs
- Stok ROK-000002: 60.000 pcs
- HPP ROK-000001: 16.700
- HPP ROK-000002: 25.000
- HARGA_BELI_TERAKHIR ROK-000001: 18.000
- HARGA_BELI_TERAKHIR ROK-000002: 28.000

---

## Test Case 1: Hapus Faktur Tengah (PB-2604270002)

**Skenario:** Hapus faktur kedua dari 4 faktur yang ada.

### Ekspektasi:

#### 1. Stok Berkurang
- ROK-000001: 50.000 - 5.000 = **45.000 pcs**
- ROK-000002: 60.000 - 5.000 = **55.000 pcs**

#### 2. HPP Recalculate (Weighted Average dari PB-001, PB-004, PB-005)

**ROK-000001:**
```
Titik awal (HARGA_AVERAGE PB-001) = 15.000, stok = 0
+ PB-001: (15.000×0 + 15.000×10.000) / 10.000 = 15.000, stok = 10.000
+ PB-004: (15.000×10.000 + 16.000×10.000) / 20.000 = 15.500, stok = 20.000
+ PB-005: (15.500×20.000 + 18.000×25.000) / 45.000 = 16.889, stok = 45.000
HPP akhir = 16.889 (dibulatkan 4 desimal)
```

**ROK-000002:**
```
Titik awal (HARGA_AVERAGE PB-001) = 20.000, stok = 0
+ PB-001: (20.000×0 + 20.000×10.000) / 10.000 = 20.000, stok = 10.000
+ PB-004: (20.000×10.000 + 22.000×10.000) / 20.000 = 21.000, stok = 20.000
+ PB-005: (21.000×20.000 + 28.000×35.000) / 55.000 = 25.455, stok = 55.000
HPP akhir = 25.455 (dibulatkan 4 desimal)
```

#### 3. HARGA_BELI_TERAKHIR (dari PB-005, faktur tersisa terbaru)
- ROK-000001: **18.000** ✅ (tetap, karena PB-005 masih ada)
- ROK-000002: **28.000** ✅ (tetap, karena PB-005 masih ada)

#### 4. Nilai Persediaan Baru
```
ROK-000001: 16.889 × 45.000 = 760.005.000
ROK-000002: 25.455 × 55.000 = 1.400.025.000
Total: 2.160.030.000
```

#### 5. Jurnal Penyesuaian
```
Saldo 01.04.001 sebelum hapus: 2.528.649.961
Saldo setelah hapus jurnal PB-002: 2.528.649.961 - 175.000.000 = 2.353.649.961
Nilai persediaan baru: 2.160.030.000
Selisih: 2.160.030.000 - 2.353.649.961 = -193.619.961

Karena selisih negatif (persediaan lebih dicatat):
  D: 06.04.002 PENYESUAIAN HARGA POKOK  193.619.961
  K: 01.04.001 PERSEDIAAN BARANG        193.619.961
```

#### 6. Saldo Akhir
- 01.04.001: 2.353.649.961 - 193.619.961 = **2.160.030.000** ✅
- 06.04.002: 193.649.961 (K) + 193.619.961 (D) = **30.000** (K)

---

## Test Case 2: Hapus Faktur Terakhir (PB-2604270005)

**Skenario:** Hapus faktur terbaru (paling mahal).

### Ekspektasi:

#### 1. Stok Berkurang
- ROK-000001: 50.000 - 25.000 = **25.000 pcs**
- ROK-000002: 60.000 - 35.000 = **25.000 pcs**

#### 2. HPP Recalculate (dari PB-001, PB-002, PB-004)

**ROK-000001:**
```
+ PB-001: 15.000, stok = 10.000
+ PB-002: 15.000, stok = 15.000
+ PB-004: (15.000×15.000 + 16.000×10.000) / 25.000 = 15.400
HPP akhir = 15.400
```

**ROK-000002:**
```
+ PB-001: 20.000, stok = 10.000
+ PB-002: 20.000, stok = 15.000
+ PB-004: (20.000×15.000 + 22.000×10.000) / 25.000 = 20.800
HPP akhir = 20.800
```

#### 3. HARGA_BELI_TERAKHIR (dari PB-004, faktur tersisa terbaru)
- ROK-000001: **16.000** ✅ (berubah dari 18.000)
- ROK-000002: **22.000** ✅ (berubah dari 28.000)

#### 4. Nilai Persediaan Baru
```
ROK-000001: 15.400 × 25.000 = 385.000.000
ROK-000002: 20.800 × 25.000 = 520.000.000
Total: 905.000.000
```

#### 5. Jurnal Penyesuaian
```
Saldo setelah hapus: 2.528.649.961 - 1.430.000.000 = 1.098.649.961
Nilai persediaan: 905.000.000
Selisih: 905.000.000 - 1.098.649.961 = -193.649.961

D: 06.04.002  193.649.961
K: 01.04.001  193.649.961
```

#### 6. Saldo Akhir
- 01.04.001: **905.000.000** ✅
- 06.04.002: **0** (seimbang)

---

## Test Case 3: Hapus Faktur Pertama (PB-2604270001)

**Skenario:** Hapus faktur paling lama.

### Ekspektasi:

#### 1. Stok Berkurang
- ROK-000001: 50.000 - 10.000 = **40.000 pcs**
- ROK-000002: 60.000 - 10.000 = **50.000 pcs**

#### 2. HPP Recalculate (dari PB-002, PB-004, PB-005)

**ROK-000001:**
```
Titik awal (HARGA_AVERAGE PB-002) = 15.000
+ PB-002: 15.000, stok = 5.000
+ PB-004: (15.000×5.000 + 16.000×10.000) / 15.000 = 15.667
+ PB-005: (15.667×15.000 + 18.000×25.000) / 40.000 = 17.125
HPP akhir = 17.125
```

**ROK-000002:**
```
Titik awal = 20.000
+ PB-002: 20.000, stok = 5.000
+ PB-004: (20.000×5.000 + 22.000×10.000) / 15.000 = 21.333
+ PB-005: (21.333×15.000 + 28.000×35.000) / 50.000 = 26.000
HPP akhir = 26.000
```

#### 3. HARGA_BELI_TERAKHIR (dari PB-005)
- ROK-000001: **18.000** ✅ (tetap)
- ROK-000002: **28.000** ✅ (tetap)

#### 4. Nilai Persediaan Baru
```
ROK-000001: 17.125 × 40.000 = 685.000.000
ROK-000002: 26.000 × 50.000 = 1.300.000.000
Total: 1.985.000.000
```

#### 5. Jurnal Penyesuaian
```
Saldo setelah hapus: 2.528.649.961 - 350.000.000 = 2.178.649.961
Nilai persediaan: 1.985.000.000
Selisih: 1.985.000.000 - 2.178.649.961 = -193.649.961

D: 06.04.002  193.649.961
K: 01.04.001  193.649.961
```

#### 6. Saldo Akhir
- 01.04.001: **1.985.000.000** ✅
- 06.04.002: **0** (seimbang)

---

## Rekomendasi Test

**Pilih Test Case 2 (Hapus PB-2604270005)** karena:
1. ✅ Test `HARGA_BELI_TERAKHIR` berubah (dari 18.000 → 16.000, dari 28.000 → 22.000)
2. ✅ HPP turun (hapus faktur mahal) → test jurnal D: 06.04.002, K: 01.04.001
3. ✅ Saldo 06.04.002 jadi 0 (seimbang sempurna) → mudah diverifikasi
4. ✅ Qty besar (25.000 dan 35.000) → dampak signifikan, mudah dilihat

---

## Checklist Verifikasi

Setelah hapus PB-2604270005, verifikasi:

- [ ] Stok: ROK-000001 = 25.000, ROK-000002 = 25.000
- [ ] HPP: ROK-000001 = 15.400, ROK-000002 = 20.800
- [ ] HARGA_BELI_TERAKHIR: ROK-000001 = 16.000, ROK-000002 = 22.000
- [ ] Jurnal penyesuaian: D: 06.04.002, K: 01.04.001, nominal 193.649.961
- [ ] Saldo 01.04.001 = 905.000.000
- [ ] Saldo 06.04.002 = 0
- [ ] Nilai persediaan = Saldo jurnal (seimbang)
- [ ] Tidak ada error/exception

---

## Query Verifikasi

```sql
-- Verifikasi stok dan HPP
SELECT 
    ID_BARANG,
    NAMA_BARANG,
    STOK_TOKO,
    HARGA_BELI AS HPP,
    HARGA_BELI_TERAKHIR,
    ROUND(HARGA_BELI * STOK_TOKO, 0) AS NILAI_PERSEDIAAN
FROM tbl_barang 
WHERE ID_BARANG IN ('ROK-000001', 'ROK-000002');

-- Verifikasi saldo akun
SELECT 
    KODE_AKUN,
    NAMA_AKUN,
    SALDO_AKHIR
FROM tbl_datareferensi 
WHERE KODE_AKUN IN ('01.04.001', '06.04.002');

-- Verifikasi keseimbangan
SELECT 
    (SELECT SUM(ROUND(HARGA_BELI * STOK_TOKO, 0)) 
     FROM tbl_barang 
     WHERE ID_BARANG IN ('ROK-000001', 'ROK-000002')) AS NILAI_PERSEDIAAN,
    (SELECT SALDO_AKHIR 
     FROM tbl_datareferensi 
     WHERE KODE_AKUN = '01.04.001') AS SALDO_JURNAL,
    (SELECT SUM(ROUND(HARGA_BELI * STOK_TOKO, 0)) 
     FROM tbl_barang 
     WHERE ID_BARANG IN ('ROK-000001', 'ROK-000002')) -
    (SELECT SALDO_AKHIR 
     FROM tbl_datareferensi 
     WHERE KODE_AKUN = '01.04.001') AS SELISIH;

-- Verifikasi jurnal penyesuaian
SELECT 
    NO_TRANSAKSI,
    URAIAN,
    NOMOR_AKUN_D,
    NAMA_AKUN_D,
    NOMOR_AKUN_K,
    NAMA_AKUN_K,
    NOMINAL
FROM JurnalUmum 
WHERE NO_TRANSAKSI = 'PB-2604270005'
AND JENIS_TRANSAKSI = 'Penyesuaian HPP';

-- Verifikasi PB-005 sudah terhapus
SELECT COUNT(*) AS JUMLAH FROM pembelian WHERE ID_PEMBELIAN = 'PB-2604270005';
-- Harus return 0
```
