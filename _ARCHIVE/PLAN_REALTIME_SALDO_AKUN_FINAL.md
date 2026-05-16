# PLAN: Real-Time Saldo Akun Final

## Tujuan Utama
- Menghilangkan penggunaan `UpdateSaldoSemuaAkun()` (yang memanggil `sp_bat_saldo_semua_akun`) SEPENUHNYA
- Hanya menggunakan `UpdateSaldoAkun()` (yang memanggil `sp_hlp_saldo_akun_update`) untuk update akun yang terlibat saja (realtime)
- Untuk kebutuhan recalculate penuh, gunakan `ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()`

---

## Daftar Form yang Menggunakan JurnalUmum (INSERT/DELETE)

### 0Form
| No | Form / File | Keterangan | Lokasi | Prioritas |
|----|--------------|------------|--------|-----------|
| 1 | FormLoading | Ganti UpdateSaldoSemuaAkun() dengan PostingResmi | `0Form/FormLoading.vb:28,70` | High |
| 2 | FormUtama (Hapus Transaksi) | Simpan akun terlibat sebelum delete | `0Form/FormUtama.vb` (multiple) | High |

---

### 1Master
| No | Form / File | Keterangan | Lokasi | Prioritas |
|----|--------------|------------|--------|-----------|
| 3 | TambahSupliyer | Tambahkan SELECT akun lalu UpdateSaldoAkun() | `1Master/TambahSupliyer.vb:369` | Low |
| 4 | TambahPelanggan | Tambahkan SELECT akun lalu UpdateSaldoAkun() | `1Master/TambahPelanggan.vb:391` | Low |
| 5 | TambahBarang | Tambahkan SELECT akun lalu UpdateSaldoAkun() | `1Master/TambahBarang.vb:1583,1824` | Low |
| 6 | FormBarang | Tambahkan SELECT akun lalu UpdateSaldoAkun() | `1Master/FormBarang.vb:658,1316` | Low |

---

### 2Trans
| No | Form / File | Keterangan | Lokasi | Prioritas |
|----|--------------|------------|--------|-----------|
| 7 | FormPembelian | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormPembelian.vb:2044,2418,2768` | High |
| 8 | FormPenjualan | Sudah OK! | `2Trans/FormPenjualan.vb` | ✅ Done |
| 9 | FormReturPenjualan | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormReturPenjualan.vb:838,1107,1132,1158,1176` | High |
| 10 | FormReturPembelian | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormReturPembelian.vb:881,1070` | High |
| 11 | FormReturBeli | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormReturBeli.vb:3068,3176,3374` | Medium |
| 12 | FormBayarHutang | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormBayarHutang.vb:358,450` | Medium |
| 13 | FormBayarPiutang | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormBayarPiutang.vb:365,461` | Medium |
| 14 | FormEditBayarJual | Tambahkan SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormEditBayarJual.vb:522,899` | Medium |
| 15 | FormStokOpname | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormStokOpname.vb:599,696,800` | Medium |
| 16 | FormTransferBarang | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormTransferBarang.vb:1045,1140,1285` | Medium |
| 17 | FormTransferStok | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormTransferStok.vb:792,938` | Medium |
| 18 | FormTransferCabang | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `2Trans/FormTransferCabang.vb:1834,1854,2300,2313` | Medium |

---

### 3Jurnal
| No | Form / File | Keterangan | Lokasi | Prioritas |
|----|--------------|------------|--------|-----------|
| 19 | FormKeuangan | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `3Jurnal/FormKeuangan.vb:382,635,750,770` | Medium |

---

### 4Gaji
| No | Form / File | Keterangan | Lokasi | Prioritas |
|----|--------------|------------|--------|-----------|
| 20 | FormGaji | Ubah ke SELECT akun lalu UpdateSaldoAkun() | `4Gaji/FormGaji.vb:849,877,1046,1318,1345,1368` | Medium |
| 21 | FormBon | Tambahkan SELECT akun lalu UpdateSaldoAkun() | `4Gaji/FormBon.vb:272,464,533` | Medium |

---

## Task List

### Task 1: Ubah FormLoading
**File**: `0Form/FormLoading.vb`
**Perubahan**: Ganti `UpdateSaldoSemuaAkun()` dengan `ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()`
**Alasan**: Tujuannya adalah recalculate penuh (posting resmi), bukan real-time

---

### Task 2: Ubah FormUtama (Saat Hapus Transaksi)
**File**: `0Form/FormUtama.vb`
**Perubahan**:
1.  **Sebelum** menghapus `JurnalUmum`: SELECT dan simpan daftar akun yang terlibat (NO_TRANSAKSI)
2.  Hapus `JurnalUmum` dan data transaksi lainnya
3.  Panggil `UpdateSaldoAkun()` per akun yang disimpan
**Alasan**: Setelah JurnalUmum dihapus, tidak bisa lagi SELECT akun yang terlibat

---

### Task 3: Ubah FormPembelian
**File**: `2Trans/FormPembelian.vb`
**Perubahan**: Ganti `UpdateSaldoSemuaAkun(transaction)` dengan pola seperti FormPenjualan:
```vb
' 1. SELECT akun terlibat dari JurnalUmum (NO_TRANSAKSI)
' 2. Panggil UpdateSaldoAkun() per akun
```

---

### Task 4: Ubah FormReturPenjualan, FormReturPembelian, FormReturBeli
**File**: 
- `2Trans/FormReturPenjualan.vb`
- `2Trans/FormReturPembelian.vb`
- `2Trans/FormReturBeli.vb`
**Perubahan**: Sama seperti FormPembelian dan FormPenjualan

---

### Task 5: Ubah FormBayarHutang, FormBayarPiutang, FormEditBayarJual
**File**:
- `2Trans/FormBayarHutang.vb`
- `2Trans/FormBayarPiutang.vb`
- `2Trans/FormEditBayarJual.vb`
**Perubahan**: Sama seperti FormPembelian dan FormPenjualan

---

### Task 6: Ubah FormStokOpname, FormTransferBarang, FormTransferStok, FormTransferCabang
**File**:
- `2Trans/FormStokOpname.vb`
- `2Trans/FormTransferBarang.vb`
- `2Trans/FormTransferStok.vb`
- `2Trans/FormTransferCabang.vb`
**Perubahan**: Sama seperti FormPembelian dan FormPenjualan

---

### Task 7: Ubah FormKeuangan
**File**: `3Jurnal/FormKeuangan.vb`
**Perubahan**: Sama seperti FormPembelian dan FormPenjualan

---

### Task 8: Ubah FormGaji dan FormBon
**File**:
- `4Gaji/FormGaji.vb`
- `4Gaji/FormBon.vb`
**Perubahan**: Sama seperti FormPembelian dan FormPenjualan

---

### Task 9: Ubah Form Master (TambahSupliyer, TambahPelanggan, TambahBarang, FormBarang)
**File**:
- `1Master/TambahSupliyer.vb`
- `1Master/TambahPelanggan.vb`
- `1Master/TambahBarang.vb`
- `1Master/FormBarang.vb`
**Perubahan**: Tambahkan SELECT akun lalu UpdateSaldoAkun() (jika menggunakan JurnalUmum)

---

### Task 10: Hapus UpdateSaldoSemuaAkun() dari ModuleVariabel.vb
**File**: `Modules/ModuleVariabel.vb`
**Perubahan**: Hapus kedua overload `UpdateSaldoSemuaAkun()` (tanpa dan dengan parameter transaction)

---

### Task 11: Sederhanakan FormEditBayarJual agar Konsisten
**File**: `2Trans/FormEditBayarJual.vb`
**Perubahan**:
1. **Hapus** penggunaan `AmbilDampakNetJurnalTransaksi()`
2. **Ganti** dengan SELECT akun langsung dari JurnalUmum:
   - Sebelum delete: SELECT daftar akun **lama**
   - Setelah insert: SELECT daftar akun **baru**
3. Gabungkan kedua daftar dengan `HashSet(Of String)`
4. Panggil `UpdateSaldoAkun()` per akun yang digabungkan
5. **Hapus** fungsi `TerapkanDeltaSaldoAkun()` (atau biarkan jika ingin tetap backward compatible, tapi lebih baik disederhanakan)
**Alasan**: Menjadikan konsistensi dengan semua form lain dan menghilangkan ketergantungan ke fungsi delta yang tidak relevan lagi (karena `sp_hlp_saldo_akun_update` sudah recalculate penuh)
**Status**: ✅ Selesai!

---

### Task 12: Perbaiki FormPenjualan (Mode Edit Belum SELECT Akun Lama)
**File**: `2Trans/FormPenjualan.vb`
**Lokasi**: `Prosessimpan()` (line sekitar 3814-3827)
**Perubahan**:
1. Di dalam blok `If Not IsModeTambahPenjualan Then` (mode edit):
   - SEBELUM memanggil `Hapusuntukedit(transaction)`, SELECT dan simpan daftar akun LAMA dari JurnalUmum
2. Setelah `Simpanjurnal()` (insert jurnal baru), SELECT dan simpan daftar akun BARU
3. Gabungkan kedua daftar (akun lama + akun baru) dengan `HashSet`
4. Panggil `UpdateSaldoAkun()` per akun yang digabungkan
**Alasan**: Akun transaksi lama juga harus di-update karena jurnalnya dihapus!
**Status**: ✅ Selesai!

---

### Task 13: Perbaiki FormPembelian (Mode Edit Belum SELECT Akun Lama)
**File**: `2Trans/FormPembelian.vb`
**Lokasi**: `SimpanTransaksi()` (line sekitar 1998-2007)
**Perubahan**:
1. Di dalam blok `If Not IsModeTambahPembelian Then` (mode edit):
   - SEBELUM memanggil `Hapusbelanja(transaction)`, SELECT dan simpan daftar akun LAMA dari JurnalUmum
2. Setelah `Simpanjurnal()` (insert jurnal baru), SELECT dan simpan daftar akun BARU
3. Gabungkan kedua daftar (akun lama + akun baru) dengan `HashSet`
4. Panggil `UpdateSaldoAkun()` per akun yang digabungkan
**Alasan**: Akun transaksi lama juga harus di-update karena jurnalnya dihapus!
**Status**: ✅ Selesai!

---

### Task 14: Perbaiki FormKeuangan (Hapus & Edit Belum SELECT Akun Lama)
**File**: `3Jurnal/FormKeuangan.vb`
**Lokasi**: 
- Fungsi `HapusTransaksi()` (line sekitar 382-393)
- Fungsi `EditTransaksi()` (line sekitar 749-817)
**Perubahan**:
1. **HapusTransaksi**: Sudah OK (sudah SELECT akun sebelum delete!)
2. **EditTransaksi**: Sudah OK (sudah SELECT akun lama sebelum delete dan update akun baru!)
**Status**: ✅ Sudah OK!

---

### Task 15: Perbaiki FormGaji (Hapus & Edit Belum SELECT Akun Lama)
**File**: `4Gaji/FormGaji.vb`
**Lokasi**:
- Fungsi `DGVGaji_CellContentClick` (Hapus Slip Gaji, line sekitar 849-893)
- Fungsi `BtnSimpann_Click` (Edit Slip Gaji, line sekitar 1054-1128)
**Perubahan**:
1. **Hapus Slip Gaji**: SEBELUM DELETE JurnalUmum, SELECT dan simpan daftar akun lama
2. **Edit Slip Gaji**: SEBELUM DELETE JurnalUmum, SELECT dan simpan daftar akun lama, kemudian gabungkan dengan akun baru, lalu panggil UpdateSaldoAkun() per akun
**Alasan**: FormGaji saat ini SELECT akun SETELAH delete JurnalUmum (tidak bisa!), dan update akun secara manual (tidak konsisten!)
**Status**: ✅ Sudah diperbaiki!

---

### Task 16: Perbaiki FormReturBeli (Edit Belum SELECT Akun Lama)
**File**: `2Trans/FormReturBeli.vb`
**Lokasi**: Fungsi edit dengan `HapusUntukEdit()` (line sekitar 3190-3202)
**Perubahan**: SEBELUM DELETE JurnalUmum, SELECT dan simpan daftar akun lama
**Alasan**: Saat ini SELECT akun SETELAH delete JurnalUmum!
**Status**: ✅ Sudah diperbaiki!

---

### Task 17: Perbaiki FormTransferBarang (Edit Belum SELECT Akun Lama)
**File**: `2Trans/FormTransferBarang.vb`
**Lokasi**: Fungsi `HapusUntukEdit()` (line sekitar 1154-1166)
**Perubahan**: SEBELUM DELETE JurnalUmum, SELECT dan simpan daftar akun lama
**Alasan**: Saat ini SELECT akun SETELAH delete JurnalUmum!
**Status**: ✅ Sudah diperbaiki!

---

### Task 18: Perbaiki FormStokOpname (Edit Belum SELECT Akun Lama)
**File**: `2Trans/FormStokOpname.vb`
**Lokasi**: Fungsi `Hapusstokopname()` (line sekitar 815-826)
**Perubahan**: SEBELUM DELETE JurnalUmum, SELECT dan simpan daftar akun lama
**Alasan**: Saat ini SELECT akun SETELAH delete JurnalUmum!
**Status**: ✅ Sudah diperbaiki!

---

### Task 19: Perbaiki FormBon (Hapus & Edit Belum SELECT Akun Lama)
**File**: `4Gaji/FormBon.vb`
**Lokasi**:
- Fungsi `BtnHapus_Click` (Hapus Bon, line sekitar 270-275)
- Fungsi `HapusUntukEdit()` (Edit Bon, line sekitar 462-472)
**Perubahan**:
1. **Hapus Bon**: SEBELUM DELETE JurnalUmum, SELECT dan simpan daftar akun lama
2. **Edit Bon**: SEBELUM DELETE JurnalUmum, SELECT dan simpan daftar akun lama, kemudian gabungkan dengan akun baru, lalu panggil UpdateSaldoAkun() per akun
**Alasan**: Saat ini SELECT akun SETELAH delete JurnalUmum!
**Status**: ✅ Sudah diperbaiki!

---

## Panduan Metode (KRITIS!)

### Pola Standar yang Benar

#### Pola 1: Mode Tambah Transaksi (Tidak Ada Operasi Delete)
1. INSERT JurnalUmum baru
2. SELECT akun terlibat dari JurnalUmum baru
3. Panggil UpdateSaldoAkun() per akun yang terlibat

#### Pola 2: Mode Edit Transaksi (Ada Operasi Delete + Insert)
1. **SEBELUM DELETE**: SELECT dan simpan daftar akun dari JurnalUmum **lama**
2. DELETE JurnalUmum lama dan data transaksi lainnya
3. INSERT JurnalUmum baru
4. SELECT dan simpan daftar akun dari JurnalUmum **baru**
5. Gabungkan daftar akun lama dan baru (gunakan HashSet untuk menghindari duplikat)
6. Panggil UpdateSaldoAkun() per akun yang digabungkan

#### Pola 3: Hapus Transaksi Permanen (Hanya Delete)
1. **SEBELUM DELETE**: SELECT dan simpan daftar akun dari JurnalUmum
2. DELETE JurnalUmum dan data transaksi lainnya
3. Panggil UpdateSaldoAkun() per akun yang disimpan

### Alasan Penting
- Setelah JurnalUmum dihapus, tidak bisa lagi SELECT akun yang terlibat dari transaksi lama!
- Akun transaksi lama juga harus di-update karena jurnalnya sudah dihapus, jika tidak saldo akun tersebut akan salah!


---

## Daftar Form yang Memiliki Operasi DELETE + INSERT JurnalUmum

### Status: Hasil Verifikasi Final

| No | Form / File | Lokasi Pola Delete+Insert | Prioritas | Status | Task |
|----|--------------|----------------------------|-----------|--------|------|
| 1 | FormPenjualan | `Prosessimpan()` → `Hapusuntukedit()` → `Simpanjurnal()` | 🔴 High | ✅ Sudah diperbaiki | 12 |
| 2 | FormPembelian | `SimpanTransaksi()` → `Hapusbelanja()` → `Simpanjurnal()` | 🔴 High | ✅ Sudah diperbaiki | 13 |
| 3 | FormReturPenjualan | Tidak ada operasi DELETE (hanya INSERT) | 🔴 High | ✅ Sudah OK (tidak butuh perbaikan) | - |
| 4 | FormReturPembelian | Tidak ada operasi DELETE (hanya INSERT) | 🔴 High | ✅ Sudah OK (tidak butuh perbaikan) | - |
| 5 | FormReturBeli | Edit mode dengan `HapusUntukEdit()` | 🟡 Medium | ✅ Sudah diperbaiki | 16 |
| 6 | FormTransferBarang | Edit mode dengan `HapusUntukEdit()` | 🟡 Medium | ✅ Sudah diperbaiki | 17 |
| 7 | FormStokOpname | Edit mode dengan `Hapusstokopname()` | 🟡 Medium | ✅ Sudah diperbaiki | 18 |
| 8 | FormTransferStok | Hanya TODO comment, belum ada implementasi hapus/edit | 🟡 Medium | ✅ Sudah OK (tidak butuh perbaikan) | - |
| 9 | FormTransferCabang | Tidak ada operasi DELETE (hanya INSERT) | 🟡 Medium | ✅ Sudah OK (tidak butuh perbaikan) | - |
| 10 | FormBayarHutang | Tidak ada operasi DELETE (hanya INSERT) | 🟡 Medium | ✅ Sudah OK (tidak butuh perbaikan) | - |
| 11 | FormBayarPiutang | Tidak ada operasi DELETE (hanya INSERT) | 🟡 Medium | ✅ Sudah OK (tidak butuh perbaikan) | - |
| 12 | FormEditBayarJual | Mode edit (delete + insert) | 🟢 Low | ✅ Sudah diperbaiki! | 11 |
| 13 | FormKeuangan | Hapus & Edit transaksi | 🟡 Medium | ✅ Sudah OK | 14 |
| 14 | FormGaji | Hapus & Edit Slip Gaji | 🟡 Medium | ✅ Sudah diperbaiki | 15 |
| 15 | FormBon | Hapus & Edit Bon | 🟡 Medium | ✅ Sudah diperbaiki | 19 |


---

## Catatan Penting
- `sp_hlp_saldo_akun_update` sudah diupdate untuk mengupdate S_DEBET, S_KREDIT, SALDO_AKHIR, dan akun LABA RUGI terakhir
- `ModuleLaporanKalkulasi.PostingResmi_HitungSemuaSaldo_KeTblDatareferensi()` untuk kebutuhan posting resmi dan recalculate penuh (ganti dari `UpdateSaldoSemuaAkun()`)
- `UpdateSaldoSemuaAkun()` akan dihapus sepenuhnya
- FormEditBayarJual sudah OK! Form ini menggunakan pola `AmbilDampakNetJurnalTransaksi()` sebelum delete dan `TerapkanDeltaSaldoAkun()` yang menggabungkan akun lama dan baru
