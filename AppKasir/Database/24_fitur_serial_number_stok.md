# Desain Fitur: Manajemen Serial Number Stok Barang

**Nomor Dokumen:** 24  
**Tanggal:** 16 November 2026  
**Status:** DRAFT — Belum Diimplementasi  
**Prioritas:** Medium

---

## 1. Latar Belakang

Aplikasi sudah memiliki kolom `SERIAL_NUMBER` di beberapa tabel transaksi (`penjualan_detail`,
`penjualan_ditahan_detail`, `sales_order_detail`), namun sifatnya hanya **teks bebas** tanpa
validasi dan tanpa manajemen stok. Kolom `tbl_barang.SERIAL_NUMBER` (tinyint 1) sudah ada
sebagai flag penanda apakah suatu barang berseri, tapi belum pernah diekspos ke UI.

Kebutuhan yang muncul: toko yang menjual barang berseri (HP, elektronik, alat berat, dll.)
perlu bisa **melacak SN mana yang masuk, mana yang terjual, dan mana yang masih ada di stok**,
tanpa mengubah alur pembelian yang sudah berjalan.

---

## 2. Keputusan Desain Utama

### Input SN melalui Menu Master, Bukan dari Form Pembelian

Alasan:
- Form Pembelian (`FormPembelian.vb`) sudah kompleks dan tidak memiliki kolom SN sama sekali.
  Menambahkannya berisiko merusak alur yang sudah stabil.
- Tidak semua pembelian langsung diketahui SN-nya saat barang datang (pengiriman massal,
  kiriman dari gudang pusat, dll.).
- Pendekatan menu master lebih fleksibel: SN bisa diinput kapan saja setelah barang diterima,
  bisa di-scan batch, dan mudah dikoreksi tanpa menyentuh data transaksi pembelian.
- Pola ini mirip dengan Form Tambah Stok yang sudah ada di Master Barang — penambahan SN
  dianggap sebagai operasi master, bukan transaksi keuangan.

---

## 3. Komponen yang Dibutuhkan

### 3.1 Tabel Baru: `tbl_serial_stok`

```sql
CREATE TABLE tbl_serial_stok (
    ID             INT             AUTO_INCREMENT PRIMARY KEY,
    ID_BARANG      VARCHAR(50)     NOT NULL,
    NAMA_BARANG    VARCHAR(200)    NOT NULL,
    SERIAL_NUMBER  VARCHAR(100)    NOT NULL,
    STATUS         VARCHAR(20)     NOT NULL DEFAULT 'TERSEDIA',
    -- Nilai: TERSEDIA / TERJUAL / RETUR / CACAT / HILANG
    LOKASI         VARCHAR(10)     NOT NULL DEFAULT 'TOKO',
    -- Nilai: TOKO / GUDANG
    TGL_INPUT      DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    KETERANGAN     VARCHAR(255)    NULL,
    -- Referensi transaksi (opsional, diisi saat SN keluar)
    NO_PENJUALAN   VARCHAR(30)     NULL,
    TGL_JUAL       DATETIME        NULL,
    -- Audit
    ID_USER        VARCHAR(50)     NOT NULL,
    ID_KOMPUTER    VARCHAR(50)     NOT NULL,
    updated_at     DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP
                                   ON UPDATE CURRENT_TIMESTAMP,
    -- Constraint: satu SN hanya boleh ada sekali per barang
    UNIQUE KEY uq_barang_sn (ID_BARANG, SERIAL_NUMBER),
    KEY idx_status (STATUS),
    KEY idx_id_barang (ID_BARANG),
    KEY idx_lokasi (LOKASI)
);
```

**Catatan:**
- SN unik per `ID_BARANG` — dua merek berbeda bisa punya SN sama (berbeda barang).
- Tidak ada relasi ke `NO_PEMBELIAN` karena input SN dilakukan lewat menu master, bukan
  dari form pembelian.
- Kolom `NO_PENJUALAN` diisi otomatis saat barang dijual (oleh trigger atau logika simpan jual).

---

### 3.2 Form Baru: `FormSerialNumberStok.vb` (di folder `1Master/`)

**Fungsi:** Menu master untuk mengelola stok SN per barang.

**Fitur utama:**

| Fitur | Keterangan |
|---|---|
| Pilih Barang | Combo/search barang yang flag `SERIAL_NUMBER = 1` |
| Input SN | Input satu per satu atau scan barcode batch |
| Lihat Stok SN | Tabel daftar SN: status (TERSEDIA/TERJUAL/dll), lokasi, tanggal input |
| Ubah Status Manual | Tandai SN sebagai CACAT / HILANG |
| Hapus SN | Hapus SN yang belum pernah terjual (status TERSEDIA) |
| Filter | Filter per status, lokasi, tanggal |
| Export | Ekspor daftar SN ke file (opsional, fase berikutnya) |

**Navigasi:** Dibuka dari menu Barang di FormUtama, atau dari konteks menu kanan di FormBarang.

---

### 3.3 Perubahan di `TambahBarang.vb` (Master Barang)

- Ekspos checkbox `SERIAL_NUMBER` ke UI form edit barang.
- Kolom sudah ada di DB (`tinyint(1)`), tinggal tambahkan CheckBox di form dan
  sertakan dalam query `UPDATE tbl_barang`.
- Label: "Barang Serial Number" — konsisten dengan teks yang sudah ada di
  `FormPenjualanLama` (`ChkTampilSN.Text`).

---

### 3.4 Perubahan di Form Penjualan

**FormJual.vb dan FormPenjualanLama.vb:**

Saat ini kolom "S N" (index 16) diisi bebas tanpa validasi. Dengan fitur ini:

- Jika barang yang dijual punya `tbl_barang.SERIAL_NUMBER = 1`:
  - Kolom SN menjadi **wajib diisi**.
  - Tampil popup pilih SN dari `tbl_serial_stok` dengan filter
    `ID_BARANG = x AND STATUS = 'TERSEDIA' AND LOKASI = lokasi_saat_ini`.
  - SN yang dipilih dimasukkan ke kolom SerialNumber di DGV.
- Saat simpan penjualan:
  - Update `tbl_serial_stok` SET `STATUS = 'TERJUAL'`, `NO_PENJUALAN = faktur`,
    `TGL_JUAL = tgl_transaksi` WHERE `ID_BARANG = x AND SERIAL_NUMBER = sn`.

**FormSalesOrder.vb:** Perlakuan sama dengan FormJual.

---

### 3.5 Perubahan di Form Retur Penjualan

Saat barang berseri diretur:
- Update `tbl_serial_stok` SET `STATUS = 'TERSEDIA'`, `NO_PENJUALAN = NULL`,
  `TGL_JUAL = NULL` WHERE `SERIAL_NUMBER = sn`.
- Atau gunakan status `RETUR` jika barang tidak langsung kembali ke stok (cacat, menunggu
  pemeriksaan).

---

### 3.6 Migrasi Database

File migrasi: `24_migrasi_serial_number_stok.sql`

Isi:
1. `CREATE TABLE tbl_serial_stok` (seperti definisi di atas).
2. Tidak ada perubahan kolom di tabel yang sudah ada — migrasi ini additive.

---

## 4. Alur Lengkap

```
[Master Barang]
  → Centang "Barang Serial Number" di TambahBarang
       ↓
[Menu Master → Form Serial Number Stok]
  → Pilih barang
  → Input / scan SN satu per satu
  → SN masuk ke tbl_serial_stok (STATUS = TERSEDIA)
       ↓
[Kasir Jual — FormJual]
  → Tambah barang berseri ke DGV
  → Kolom SN: popup pilih dari stok TERSEDIA
  → Pilih SN → isi kolom SerialNumber di DGV
  → Simpan → STATUS di tbl_serial_stok = TERJUAL
       ↓
[Retur Penjualan — FormReturPenjualan]
  → Barang berseri diretur
  → STATUS di tbl_serial_stok = TERSEDIA (atau RETUR)
```

---

## 5. Yang TIDAK Berubah

| Komponen | Alasan Tidak Diubah |
|---|---|
| `FormPembelian.vb` | Input SN dilakukan via menu master, bukan saat beli |
| `pembelian_detail` | Tidak perlu kolom SN baru — SN tidak berasal dari pembelian |
| Tabel counter stok (`PEMBELIAN_TOKO`, dll.) | SN tidak mempengaruhi sistem counter |
| Jurnal akuntansi | SN tidak mempengaruhi pencatatan keuangan |
| `FormReturPembelian.vb` | Retur ke supplier tidak perlu lacak SN |

---

## 6. Urutan Implementasi (Fase)

### Fase 1 — Pondasi (Prioritas Tinggi)
1. Buat `24_migrasi_serial_number_stok.sql` → `CREATE TABLE tbl_serial_stok`
2. Ekspos checkbox SN di `TambahBarang.vb`
3. Buat `FormSerialNumberStok.vb` di `1Master/`
4. Daftarkan menu di `FormUtama.vb`

### Fase 2 — Integrasi Penjualan (Prioritas Menengah)
5. `FormJual.vb`: popup pilih SN + validasi wajib untuk barang berseri
6. `FormPenjualanLama.vb`: sama
7. `FormSalesOrder.vb`: sama
8. Update `tbl_serial_stok` saat simpan penjualan

### Fase 3 — Retur & Laporan (Prioritas Rendah)
9. `FormReturPenjualan.vb`: kembalikan status SN
10. Laporan riwayat SN (form baru di `5Lap/`)

---

## 7. Pertanyaan Terbuka (Harus Dijawab Sebelum Implementasi)

1. **Jika user menjual qty > 1 untuk barang berseri, apakah dipisah jadi baris berbeda
   (1 SN per baris), atau 1 baris dengan multi-SN?**  
   → Rekomendasi: 1 SN per baris (lebih sederhana, konsisten dengan kolom SN yang sudah ada).

2. **Apakah SN perlu ditampilkan di nota cetak?**  
   → Jika ya, perlu modifikasi semua modul cetak (6Print/).

3. **Apakah laporan garansi berbasis SN perlu di fase pertama?**  
   → Rekomendasi: tunda ke Fase 3.

4. **Jika SN diinput di menu master tapi barang sudah telanjur terjual (data lama),
   bagaimana penanganannya?**  
   → SN lama tidak bisa divalidasi retroaktif — cukup mulai lacak dari tanggal implementasi.
