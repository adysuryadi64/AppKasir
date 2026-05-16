# Requirements Document

## Introduction

Fitur ini memperbaiki sistem pencatatan histori hutang (pembelian kredit ke supplier) dan piutang
(penjualan kredit ke pelanggan) pada aplikasi kasir VB.NET. Saat ini, tabel `hutang_detail` dan
`piutang_detail` hanya terisi saat pembayaran dilakukan — bukan saat hutang/piutang timbul.
Akibatnya tidak ada histori lengkap per faktur (timbul → dibayar sebagian → lunas), kolom `JENIS`
tidak terisi, dan laporan hutang/piutang harus query langsung ke tabel `pembelian`/`penjualan`.

Perbaikan ini menjadikan `hutang_detail` dan `piutang_detail` sebagai **buku besar hutang/piutang
per faktur** yang mencatat setiap event: timbul, bayar, retur, dan pembatalan — sehingga laporan
dan audit trail dapat bersumber dari tabel khusus tersebut.

---

## Glossary

- **Hutang_Detail**: Tabel buku besar hutang per faktur pembelian. Setiap baris mewakili satu event
  perubahan hutang pada satu faktur.
- **Piutang_Detail**: Tabel buku besar piutang per faktur penjualan. Setiap baris mewakili satu
  event perubahan piutang pada satu faktur.
- **JENIS**: Kolom di `hutang_detail` dan `piutang_detail` yang membedakan jenis event:
  `TIMBUL` (hutang/piutang baru timbul), `BAYAR` (pembayaran), `RETUR` (retur barang),
  `HAPUS` (pembatalan faktur).
- **FormPembelian**: Form input pembelian kredit ke supplier (`2Trans/FormPembelian.vb`).
- **FormJual**: Form input penjualan kredit ke pelanggan (`2Trans/FormJual.vb`).
- **FormBayarHutang**: Form pembayaran hutang ke supplier (`2Trans/FormBayarHutang.vb`).
- **FormBayarPiutang**: Form penerimaan pembayaran piutang dari pelanggan (`2Trans/FormBayarPiutang.vb`).
- **FormEditBayarJual**: Form edit pembayaran faktur penjualan yang sudah ada — mengubah nominal tunai/transfer, recalculate SISA_TAGIHAN dan STATUS_TRANSAKSI (`2Trans/FormEditBayarJual.vb`).
- **FormReturBeli**: Form retur pembelian ke supplier (`2Trans/FormReturBeli.vb`).
- **FormReturPembelian**: Form retur pembelian ke supplier dengan 2 mode — Mode Normal (terikat nota, ada opsi potong hutang) dan Mode Bebas (`2Trans/FormReturPembelian.vb`).
- **FormReturPenjualan**: Form retur penjualan dari pelanggan dengan 2 mode — Mode Normal (terikat nota, ada opsi potong piutang) dan Mode Bebas (`2Trans/FormReturPenjualan.vb`).
- **FormLapHutang**: Form laporan hutang jatuh tempo (`5Lap/FormLapHutang.vb`).
- **FormLapPiutang**: Form laporan piutang jatuh tempo (`5Lap/FormLapPiutang.vb`).
- **Sistem**: Keseluruhan aplikasi kasir VB.NET dengan database MySQL.
- **Migrasi_Data**: Proses pengisian baris `JENIS='TIMBUL'` untuk faktur kredit lama yang belum
  memiliki entri di `hutang_detail`/`piutang_detail`.

---

## Requirements

### Requirement 1: Persiapan Kolom JENIS di Database

**User Story:** Sebagai developer, saya ingin kolom `JENIS` di `hutang_detail` dan `piutang_detail`
memiliki nilai yang konsisten, sehingga setiap event hutang/piutang dapat dibedakan dengan jelas.

#### Acceptance Criteria

1. THE Sistem SHALL memastikan kolom `JENIS` di tabel `hutang_detail` bertipe `VARCHAR(10)` dengan
   nilai default `'BAYAR'`.
2. THE Sistem SHALL memastikan kolom `JENIS` di tabel `piutang_detail` bertipe `VARCHAR(20)` dengan
   nilai default `'BAYAR'`.
3. WHEN kolom `JENIS` di `hutang_detail` bernilai `NULL` atau kosong, THE Sistem SHALL mengisi
   nilainya menjadi `'BAYAR'` (migrasi data lama).
4. WHEN kolom `JENIS` di `piutang_detail` bernilai `NULL` atau kosong, THE Sistem SHALL mengisi
   nilainya menjadi `'BAYAR'` (migrasi data lama).
5. THE Sistem SHALL menerima tepat empat nilai valid untuk kolom `JENIS`: `'TIMBUL'`, `'BAYAR'`,
   `'RETUR'`, dan `'HAPUS'`.

---

### Requirement 2: Migrasi Data Lama — Baris TIMBUL dari Faktur Kredit Lama

**User Story:** Sebagai pemilik toko, saya ingin faktur kredit lama yang belum memiliki catatan
timbulnya hutang/piutang tetap terwakili di `hutang_detail`/`piutang_detail`, sehingga laporan
histori tidak kosong untuk transaksi sebelum perbaikan ini diterapkan.

#### Acceptance Criteria

1. WHEN skrip migrasi dijalankan, THE Sistem SHALL menyisipkan satu baris `JENIS='TIMBUL'` ke
   `hutang_detail` untuk setiap faktur di tabel `pembelian` yang memenuhi kondisi: faktur berstatus
   kredit (`pembelian.STATUS_TRANSAKSI_BELI = 'Belum Lunas'`) dan belum memiliki baris
   `JENIS='TIMBUL'` di `hutang_detail`.
2. WHEN skrip migrasi dijalankan, THE Sistem SHALL menyisipkan satu baris `JENIS='TIMBUL'` ke
   `piutang_detail` untuk setiap faktur di tabel `penjualan` yang memenuhi kondisi: faktur berstatus
   kredit (`penjualan.STATUS_TRANSAKSI IN ('Belum Lunas', 'TERHUTANG')`) dan belum memiliki baris
   `JENIS='TIMBUL'` di `piutang_detail`.
3. THE Sistem SHALL mengisi kolom `ID_BAYAR` pada baris migrasi dengan format
   `'MIGRASI-{ID_PEMBELIAN}'` atau `'MIGRASI-{ID_PENJUALAN}'` agar dapat dibedakan dari baris
   normal.
4. IF skrip migrasi dijalankan lebih dari satu kali, THEN THE Sistem SHALL tidak menyisipkan baris
   duplikat (operasi idempoten).
5. THE Sistem SHALL mengisi nilai `TOTAL_HUTANG` (hutang_detail) dari kolom `pembelian.GRAND_TOTAL_BELI`
   dan `PIUTANG` (piutang_detail) dari kolom `penjualan.GRAND_TOTAL_STL_PAJAK` faktur asal.
6. THE Sistem SHALL mengisi nilai `hutang_detail.HUTANG` dari kolom `pembelian.TAGIHAN` dan
   `piutang_detail.HUTANG` dari kolom `penjualan.SISA_TAGIHAN` faktur asal pada saat migrasi.
7. THE Sistem SHALL mengisi nilai `hutang_detail.DIBAYAR` dari kolom `pembelian.PEMBAYARAN` dan
   `hutang_detail.RETUR` dari kolom `pembelian.RETUR` pada saat migrasi (bukan diisi 0).
8. THE Sistem SHALL mengisi nilai `piutang_detail.DIBAYAR` dari kolom `penjualan.NOMINALBAYARPIUTANG`
   pada saat migrasi (bukan diisi 0).

---

### Requirement 3: Pencatatan Timbulnya Hutang saat Simpan Pembelian Kredit

**User Story:** Sebagai staf pembelian, saya ingin setiap pembelian kredit baru langsung tercatat
di `hutang_detail` sebagai event `TIMBUL`, sehingga histori hutang lengkap sejak faktur dibuat.

#### Acceptance Criteria

1. WHEN FormPembelian menyimpan faktur pembelian kredit baru (sisa hutang > 0), THE FormPembelian
   SHALL menyisipkan satu baris ke `hutang_detail` dengan `JENIS='TIMBUL'` dalam transaksi database
   yang sama dengan penyimpanan faktur.
2. THE FormPembelian SHALL mengisi `ID_BAYAR` baris TIMBUL dengan format `'TIMBUL-{ID_PEMBELIAN}'`.
3. THE FormPembelian SHALL mengisi `TOTAL_HUTANG` dari nilai `GRAND_TOTAL_BELI` faktur.
4. THE FormPembelian SHALL mengisi `HUTANG` dari nilai `TAGIHAN` (sisa hutang) faktur.
5. THE FormPembelian SHALL mengisi `DIBAYAR` dengan nilai `0` dan `PEMBAYARAN` dengan nilai `0`
   pada baris TIMBUL.
6. THE FormPembelian SHALL mengisi `JATUH_TEMPO` dari tanggal jatuh tempo faktur.
7. IF penyimpanan faktur pembelian gagal (rollback), THEN THE FormPembelian SHALL membatalkan
   penyimpanan baris `hutang_detail` TIMBUL secara bersamaan.

---

### Requirement 4: Pembaruan Baris TIMBUL saat Edit atau Hapus Pembelian Kredit

**User Story:** Sebagai staf pembelian, saya ingin perubahan atau pembatalan faktur pembelian kredit
langsung memperbarui catatan di `hutang_detail`, sehingga data histori tidak menjadi tidak konsisten
dengan data faktur.

#### Acceptance Criteria

1. WHEN FormPembelian menghapus atau mengedit faktur pembelian kredit, THE FormPembelian SHALL
   menghapus baris `hutang_detail` dengan `ID_BELI = {ID_PEMBELIAN}` dan `JENIS = 'TIMBUL'`
   sebelum menyimpan data baru.
2. WHEN FormPembelian menyimpan ulang faktur pembelian kredit setelah edit, THE FormPembelian SHALL
   menyisipkan kembali baris `JENIS='TIMBUL'` yang baru sesuai nilai faktur yang diperbarui
   (mengikuti Requirement 3).
3. IF faktur pembelian yang diedit sudah memiliki pembayaran (ada baris `JENIS='BAYAR'` di
   `hutang_detail`), THEN THE FormPembelian SHALL menampilkan peringatan kepada pengguna sebelum
   melanjutkan penghapusan baris TIMBUL.
4. IF penghapusan atau edit faktur gagal (rollback), THEN THE FormPembelian SHALL membatalkan semua
   perubahan pada `hutang_detail` secara bersamaan.

---

### Requirement 5: Pencatatan JENIS='BAYAR' saat Bayar Hutang

**User Story:** Sebagai staf keuangan, saya ingin setiap pembayaran hutang tercatat dengan
`JENIS='BAYAR'` di `hutang_detail`, sehingga histori pembayaran dapat dibedakan dari histori
timbulnya hutang.

#### Acceptance Criteria

1. WHEN FormBayarHutang menyimpan pembayaran hutang, THE FormBayarHutang SHALL menyisipkan baris
   ke `hutang_detail` dengan `JENIS='BAYAR'` (menggantikan baris yang sebelumnya tidak mengisi
   kolom JENIS).
2. WHEN FormBayarHutang menyimpan pembayaran hutang, THE FormBayarHutang SHALL memperbarui baris
   `JENIS='TIMBUL'` yang sesuai (`ID_BELI` sama) dengan mengurangi nilai `HUTANG` sebesar nominal
   yang dibayar dan menambah nilai `DIBAYAR` sebesar nominal yang dibayar.
3. THE FormBayarHutang SHALL memperbarui kolom `STATUS` pada baris `JENIS='TIMBUL'` menjadi
   `'Lunas'` jika nilai `HUTANG` setelah pengurangan sama dengan atau kurang dari nol, dan
   `'Belum Lunas'` jika masih lebih dari nol.
4. IF baris `JENIS='TIMBUL'` tidak ditemukan untuk faktur yang dibayar (faktur lama sebelum
   migrasi), THEN THE FormBayarHutang SHALL tetap menyimpan baris `JENIS='BAYAR'` tanpa error.
5. IF penyimpanan pembayaran gagal (rollback), THEN THE FormBayarHutang SHALL membatalkan semua
   perubahan pada `hutang_detail` secara bersamaan.

---

### Requirement 6: Pencatatan Timbulnya Piutang saat Simpan Penjualan Kredit

**User Story:** Sebagai staf penjualan, saya ingin setiap penjualan kredit baru langsung tercatat
di `piutang_detail` sebagai event `TIMBUL`, sehingga histori piutang lengkap sejak faktur dibuat.

#### Acceptance Criteria

1. WHEN FormJual menyimpan faktur penjualan kredit baru (sisa piutang > 0), THE FormJual SHALL
   menyisipkan satu baris ke `piutang_detail` dengan `JENIS='TIMBUL'` dalam transaksi database
   yang sama dengan penyimpanan faktur.
2. THE FormJual SHALL mengisi `ID_BAYAR` baris TIMBUL dengan format `'TIMBUL-{ID_PENJUALAN}'`.
3. THE FormJual SHALL mengisi `piutang_detail.PIUTANG` dari nilai `penjualan.GRAND_TOTAL_STL_PAJAK`
   faktur penjualan.
4. THE FormJual SHALL mengisi `piutang_detail.HUTANG` (sisa piutang) dari nilai
   `penjualan.SISA_TAGIHAN` faktur.
5. THE FormJual SHALL mengisi `DIBAYAR` dengan nilai `0` dan `PEMBAYARAN` dengan nilai `0`
   pada baris TIMBUL.
6. THE FormJual SHALL mengisi `JATUH_TEMPO` dari tanggal jatuh tempo faktur penjualan.
7. IF penyimpanan faktur penjualan gagal (rollback), THEN THE FormJual SHALL membatalkan
   penyimpanan baris `piutang_detail` TIMBUL secara bersamaan.

---

### Requirement 7: Pembaruan Baris TIMBUL saat Edit atau Hapus Penjualan Kredit

**User Story:** Sebagai staf penjualan, saya ingin perubahan atau pembatalan faktur penjualan kredit
langsung memperbarui catatan di `piutang_detail`, sehingga data histori tidak menjadi tidak
konsisten dengan data faktur.

#### Acceptance Criteria

1. WHEN FormJual menghapus atau mengedit faktur penjualan kredit, THE FormJual SHALL menghapus
   baris `piutang_detail` dengan `ID_JUAL = {ID_PENJUALAN}` dan `JENIS = 'TIMBUL'` sebelum
   menyimpan data baru.
2. WHEN FormJual menyimpan ulang faktur penjualan kredit setelah edit, THE FormJual SHALL
   menyisipkan kembali baris `JENIS='TIMBUL'` yang baru sesuai nilai faktur yang diperbarui
   (mengikuti Requirement 6).
3. IF faktur penjualan yang diedit sudah memiliki pembayaran (ada baris `JENIS='BAYAR'` di
   `piutang_detail`), THEN THE FormJual SHALL menampilkan peringatan kepada pengguna sebelum
   melanjutkan penghapusan baris TIMBUL.
4. IF penghapusan atau edit faktur gagal (rollback), THEN THE FormJual SHALL membatalkan semua
   perubahan pada `piutang_detail` secara bersamaan.

---

### Requirement 8: Pencatatan JENIS='BAYAR' saat Terima Bayar Piutang

**User Story:** Sebagai staf keuangan, saya ingin setiap penerimaan pembayaran piutang tercatat
dengan `JENIS='BAYAR'` di `piutang_detail`, sehingga histori pembayaran dapat dibedakan dari
histori timbulnya piutang.

#### Acceptance Criteria

1. WHEN FormBayarPiutang menyimpan penerimaan pembayaran piutang, THE FormBayarPiutang SHALL
   menyisipkan baris ke `piutang_detail` dengan `JENIS='BAYAR'`.
2. WHEN FormBayarPiutang menyimpan penerimaan pembayaran piutang, THE FormBayarPiutang SHALL
   memperbarui baris `JENIS='TIMBUL'` yang sesuai (`ID_JUAL` sama) dengan mengurangi nilai
   `HUTANG` sebesar nominal yang diterima dan menambah nilai `DIBAYAR` sebesar nominal yang
   diterima.
3. THE FormBayarPiutang SHALL memperbarui kolom `STATUS` pada baris `JENIS='TIMBUL'` menjadi
   `'Lunas'` jika nilai `HUTANG` setelah pengurangan sama dengan atau kurang dari nol, dan
   `'Belum Lunas'` jika masih lebih dari nol.
4. IF baris `JENIS='TIMBUL'` tidak ditemukan untuk faktur yang dibayar (faktur lama sebelum
   migrasi), THEN THE FormBayarPiutang SHALL tetap menyimpan baris `JENIS='BAYAR'` tanpa error.
5. IF penyimpanan penerimaan pembayaran gagal (rollback), THEN THE FormBayarPiutang SHALL
   membatalkan semua perubahan pada `piutang_detail` secara bersamaan.

---

### Requirement 8b: Sinkronisasi piutang_detail saat Edit Pembayaran Penjualan

**User Story:** Sebagai staf keuangan, saya ingin perubahan nominal pembayaran pada faktur penjualan
kredit melalui FormEditBayarJual langsung memperbarui baris `JENIS='TIMBUL'` di `piutang_detail`,
sehingga sisa piutang di buku besar tetap konsisten dengan data faktur.

#### Acceptance Criteria

1. WHEN FormEditBayarJual menyimpan perubahan pembayaran faktur penjualan kredit, THE
   FormEditBayarJual SHALL memperbarui baris `JENIS='TIMBUL'` di `piutang_detail` yang sesuai
   (`ID_JUAL = ID_PENJUALAN`) dengan nilai `HUTANG` baru dari `SISA_TAGIHAN` hasil kalkulasi.
2. THE FormEditBayarJual SHALL memperbarui kolom `DIBAYAR` pada baris `JENIS='TIMBUL'` dengan
   total pembayaran baru (tunai + transfer).
3. THE FormEditBayarJual SHALL memperbarui kolom `STATUS` pada baris `JENIS='TIMBUL'` menjadi
   `'Lunas'` jika `SISA_TAGIHAN` baru sama dengan nol, dan `'Belum Lunas'` jika masih lebih dari
   nol.
4. IF baris `JENIS='TIMBUL'` tidak ditemukan (faktur lama sebelum migrasi), THEN THE
   FormEditBayarJual SHALL tetap menyimpan perubahan faktur tanpa error — tidak membuat baris baru.
5. IF penyimpanan perubahan gagal (rollback), THEN THE FormEditBayarJual SHALL membatalkan semua
   perubahan pada `piutang_detail` secara bersamaan.

---

### Requirement 9: Pencatatan JENIS='RETUR' saat Retur Pembelian Kredit

**User Story:** Sebagai staf gudang, saya ingin retur pembelian yang memotong hutang tercatat di
`hutang_detail` sebagai event `RETUR`, sehingga pengurangan hutang akibat retur terdokumentasi
dengan jelas.

#### Acceptance Criteria

1. WHEN FormReturPembelian menyimpan retur dalam Mode Normal DAN `CbPotongHutang` dicentang, THE
   FormReturPembelian SHALL menyisipkan satu baris ke `hutang_detail` dengan `JENIS='RETUR'`
   dalam transaksi yang sama.
2. THE baris RETUR SHALL mengisi `ID_BAYAR` dengan format `'RETUR-{ID_RETUR_PEMBELIAN}'`,
   `ID_BELI` dengan `ID_PEMBELIAN` nota asal, `PEMBAYARAN` dengan nilai `TOTAL_RUPIAH` retur.
3. WHEN baris RETUR disisipkan, THE FormReturPembelian SHALL memperbarui baris `JENIS='TIMBUL'`
   yang sesuai (`ID_BELI` sama) dengan mengurangi `HUTANG` sebesar nilai retur dan menambah
   `RETUR` sebesar nilai retur.
4. THE FormReturPembelian SHALL memperbarui kolom `STATUS` pada baris `JENIS='TIMBUL'` menjadi
   `'Lunas'` jika `HUTANG` setelah pengurangan sama dengan atau kurang dari nol.
5. IF Mode Bebas (`CbJenisRetur.Checked = True`), THEN THE FormReturPembelian SHALL tidak
   menyisipkan baris ke `hutang_detail` karena tidak ada nota pembelian yang terikat.
6. IF `CbPotongHutang` tidak dicentang (retur tunai/transfer), THEN THE FormReturPembelian SHALL
   tidak menyisipkan baris ke `hutang_detail`.
7. IF baris `JENIS='TIMBUL'` tidak ditemukan untuk nota yang diretur (faktur lama sebelum
   migrasi), THEN THE FormReturPembelian SHALL tetap menyimpan baris `JENIS='RETUR'` tanpa error.
8. IF penyimpanan retur gagal (rollback), THEN THE FormReturPembelian SHALL membatalkan semua
   perubahan pada `hutang_detail` secara bersamaan.

---

### Requirement 10: Pencatatan JENIS='RETUR' saat Retur Penjualan Kredit

**User Story:** Sebagai staf penjualan, saya ingin retur penjualan yang memotong piutang tercatat
di `piutang_detail` sebagai event `RETUR`, sehingga pengurangan piutang akibat retur
terdokumentasi dengan jelas.

#### Acceptance Criteria

1. WHEN FormReturPenjualan menyimpan retur dalam Mode Normal DAN `CbPotongHutang` dicentang, THE
   FormReturPenjualan SHALL menyisipkan satu baris ke `piutang_detail` dengan `JENIS='RETUR'`
   dalam transaksi yang sama.
2. THE baris RETUR SHALL mengisi `ID_BAYAR` dengan format `'RETUR-{ID_RETUR_PENJUALAN}'`,
   `ID_JUAL` dengan `ID_PENJUALAN` nota asal, `PEMBAYARAN` dengan nilai `TOTAL_RUPIAH` retur.
3. WHEN baris RETUR disisipkan, THE FormReturPenjualan SHALL memperbarui baris `JENIS='TIMBUL'`
   yang sesuai (`ID_JUAL` sama) dengan mengurangi `HUTANG` sebesar nilai retur dan menambah
   `RETUR` sebesar nilai retur.
4. THE FormReturPenjualan SHALL memperbarui kolom `STATUS` pada baris `JENIS='TIMBUL'` menjadi
   `'Lunas'` jika `HUTANG` setelah pengurangan sama dengan atau kurang dari nol.
5. IF Mode Bebas (`CbJenisRetur.Checked = True`), THEN THE FormReturPenjualan SHALL tidak
   menyisipkan baris ke `piutang_detail` karena tidak ada nota penjualan yang terikat.
6. IF `CbPotongHutang` tidak dicentang (retur tunai/transfer), THEN THE FormReturPenjualan SHALL
   tidak menyisipkan baris ke `piutang_detail`.
7. IF baris `JENIS='TIMBUL'` tidak ditemukan untuk nota yang diretur (faktur lama sebelum
   migrasi), THEN THE FormReturPenjualan SHALL tetap menyimpan baris `JENIS='RETUR'` tanpa error.
8. IF penyimpanan retur gagal (rollback), THEN THE FormReturPenjualan SHALL membatalkan semua
   perubahan pada `piutang_detail` secara bersamaan.

---

### Requirement 11: Konsistensi Data — Saldo HUTANG di hutang_detail

**User Story:** Sebagai akuntan, saya ingin nilai `HUTANG` pada baris `JENIS='TIMBUL'` di
`hutang_detail` selalu mencerminkan sisa hutang terkini per faktur, sehingga laporan dapat
bersumber dari tabel ini tanpa perlu JOIN ke `pembelian`.

#### Acceptance Criteria

1. THE Sistem SHALL memastikan bahwa untuk setiap faktur pembelian kredit, nilai `hutang_detail.HUTANG`
   pada baris `JENIS='TIMBUL'` sama dengan nilai `pembelian.TAGIHAN` setelah setiap operasi
   (bayar, retur, edit).
2. THE Sistem SHALL memastikan bahwa `hutang_detail.TOTAL_HUTANG` pada baris `JENIS='TIMBUL'` sama
   dengan `pembelian.GRAND_TOTAL_BELI` faktur asal dan tidak berubah setelah pembayaran atau retur.
3. THE Sistem SHALL memastikan bahwa `hutang_detail.DIBAYAR + hutang_detail.RETUR + hutang_detail.HUTANG = hutang_detail.TOTAL_HUTANG`
   pada baris `JENIS='TIMBUL'` setelah setiap operasi.

---

### Requirement 12: Konsistensi Data — Saldo HUTANG di piutang_detail

**User Story:** Sebagai akuntan, saya ingin nilai `HUTANG` pada baris `JENIS='TIMBUL'` di
`piutang_detail` selalu mencerminkan sisa piutang terkini per faktur, sehingga laporan dapat
bersumber dari tabel ini tanpa perlu JOIN ke `penjualan`.

#### Acceptance Criteria

1. THE Sistem SHALL memastikan bahwa untuk setiap faktur penjualan kredit, nilai `piutang_detail.HUTANG`
   pada baris `JENIS='TIMBUL'` sama dengan nilai `penjualan.SISA_TAGIHAN` setelah setiap operasi
   (bayar, retur, edit).
2. THE Sistem SHALL memastikan bahwa `piutang_detail.PIUTANG` pada baris `JENIS='TIMBUL'` sama
   dengan `penjualan.GRAND_TOTAL_STL_PAJAK` faktur asal dan tidak berubah setelah pembayaran atau
   retur.
3. THE Sistem SHALL memastikan bahwa `piutang_detail.DIBAYAR + piutang_detail.RETUR + piutang_detail.HUTANG = piutang_detail.PIUTANG`
   pada baris `JENIS='TIMBUL'` setelah setiap operasi.

---

### Requirement 13: Laporan Hutang Jatuh Tempo dari hutang_detail

**User Story:** Sebagai pemilik toko, saya ingin laporan hutang jatuh tempo bersumber dari
`hutang_detail` (bukan langsung dari `pembelian`), sehingga laporan mencerminkan histori yang
lengkap dan konsisten dengan buku besar hutang.

#### Acceptance Criteria

1. WHEN FormLapHutang menampilkan laporan hutang, THE FormLapHutang SHALL mengambil data dari
   `hutang_detail WHERE JENIS='TIMBUL' AND STATUS='Belum Lunas'` sebagai sumber utama.
2. THE FormLapHutang SHALL menampilkan kolom: nomor faktur, nama supplier, tanggal beli, total
   hutang awal, sudah dibayar, nilai retur, sisa hutang, dan tanggal jatuh tempo.
3. THE FormLapHutang SHALL menghasilkan total sisa hutang yang sama dengan hasil query lama dari
   tabel `pembelian` setelah migrasi data selesai.

---

### Requirement 14: Laporan Piutang Jatuh Tempo dari piutang_detail

**User Story:** Sebagai pemilik toko, saya ingin laporan piutang jatuh tempo bersumber dari
`piutang_detail` (bukan langsung dari `penjualan`), sehingga laporan mencerminkan histori yang
lengkap dan konsisten dengan buku besar piutang.

#### Acceptance Criteria

1. WHEN FormLapPiutang menampilkan laporan piutang, THE FormLapPiutang SHALL mengambil data dari
   `piutang_detail WHERE JENIS='TIMBUL' AND STATUS='Belum Lunas'` sebagai sumber utama.
2. THE FormLapPiutang SHALL menampilkan kolom: nomor faktur, nama pelanggan, tanggal jual, total
   piutang awal, sudah dibayar, nilai retur, sisa piutang, dan tanggal jatuh tempo.
3. THE FormLapPiutang SHALL menghasilkan total sisa piutang yang sama dengan hasil query lama dari
   tabel `penjualan` setelah migrasi data selesai.

---

### Requirement 15: Urutan Eksekusi dan Keamanan Migrasi

**User Story:** Sebagai developer, saya ingin perubahan database dan kode dapat diterapkan secara
bertahap tanpa merusak data yang sudah ada, sehingga risiko kesalahan dapat diminimalkan.

#### Acceptance Criteria

1. THE Sistem SHALL menerapkan perubahan skema database (ALTER TABLE) sebelum perubahan kode
   aplikasi diaktifkan.
2. THE Sistem SHALL menjalankan skrip migrasi data (Requirement 2) sebelum form-form yang
   menghasilkan baris TIMBUL baru diaktifkan.
3. IF database tidak memiliki backup sebelum skrip migrasi dijalankan, THEN THE Sistem SHALL
   menampilkan peringatan kepada developer dan meminta konfirmasi sebelum melanjutkan.
4. THE Sistem SHALL memastikan skrip migrasi dapat dijalankan ulang tanpa efek samping (idempoten).
5. WHILE skrip migrasi sedang berjalan pada tabel `penjualan` dengan 161.209 baris, THE
   Sistem SHALL menyelesaikan proses dalam waktu kurang dari 10 menit pada hardware produksi
   (estimasi: hanya ~30 baris kredit aktif di `db_moroseneng`, migrasi sangat cepat).
