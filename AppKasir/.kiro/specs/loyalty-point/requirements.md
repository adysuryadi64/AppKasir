# Requirements Document

## Introduction

Fitur Sistem Poin Loyalitas Pelanggan memungkinkan toko memberikan poin reward kepada pelanggan terdaftar pada setiap transaksi penjualan. Poin dapat diperoleh berdasarkan dua mekanisme: per satuan item yang dibeli, atau berdasarkan kelipatan nominal total belanja.

**Pendekatan penukaran poin:** Poin **tidak** digunakan sebagai potongan harga di FormJual. Penukaran poin dilakukan melalui **form tersendiri (FormTukarPoin)** di mana pelanggan menukarkan poin dengan barang pilihan. Ini menjaga FormJual tetap tidak berubah dan implementasi lebih sederhana.

Pada struk penjualan, hanya ditampilkan **poin yang diperoleh dari transaksi** dan **saldo poin terkini** pelanggan — tanpa informasi redeem.

Referensi best practice yang diadopsi:
- **Ledger-based tracking**: Setiap perubahan poin dicatat sebagai baris transaksi di tabel ledger (tidak hanya update saldo), sehingga audit trail lengkap dan saldo dapat direkonstruksi kapan saja.
- **Dual earning mechanism**: Poin per item (berbasis qty) dan poin per kelipatan nominal adalah dua model paling umum di sistem POS ritel.
- **Real-time balance display**: Saldo poin ditampilkan saat transaksi agar pelanggan termotivasi.
- **Redemption as goods**: Poin ditukar dengan barang melalui transaksi tersendiri, bukan sebagai diskon — model ini umum di program loyalitas minimarket dan apotek.

---

## Glossary

- **Loyalty_Engine**: Modul VB.NET (`ModuleLoyaltyPoin.vb`) yang menangani kalkulasi perolehan poin dan penukaran poin dengan barang.
- **Poin_Ledger**: Tabel database `poin_ledger` yang mencatat setiap transaksi poin (EARN/REDEEM/VOID_EARN) sebagai baris immutable.
- **Saldo_Poin**: Jumlah poin aktif milik pelanggan, disimpan di kolom `SALDO_POIN` di `tbl_pelanggan` dan diperbarui setiap transaksi poin.
- **Earn_Rate**: Aturan perolehan poin — bisa berupa poin per qty satuan item, atau poin per kelipatan nominal belanja.
- **Poin_Per_Qty**: Jumlah poin yang diberikan per 1 satuan qty item terjual (dipakai saat mekanisme "Per Item").
- **Kelipatan_Nominal**: Nilai belanja (Rp) yang menghasilkan 1 poin (dipakai saat mekanisme "Per Kelipatan Nominal"). Contoh: Rp 10.000 = 1 poin.
- **Harga_Poin**: Nilai poin yang dibutuhkan untuk menebus 1 unit barang tertentu saat penukaran di FormTukarPoin.
- **Minimum_Redeem**: Jumlah poin minimum yang harus dimiliki pelanggan sebelum dapat melakukan penukaran barang.
- **FormJual**: Form transaksi penjualan yang sudah ada — **tidak diubah strukturnya**.
- **FormGeneralSetting**: Form pengaturan global aplikasi yang sudah ada.
- **FormMasterPoin**: Form master baru untuk mengatur konfigurasi sistem poin dan melihat riwayat poin per pelanggan.
- **FormTukarPoin**: Form baru khusus untuk proses penukaran poin pelanggan dengan barang.
- **ModulePrinterJual**: Modul cetak struk penjualan yang sudah ada (thermal/dot matrix/GDI+).
- **tbl_pelanggan**: Tabel master pelanggan yang sudah ada, dengan kolom kunci `KODE`.
- **penjualan**: Tabel header transaksi penjualan yang sudah ada.
- **penjualan_detail**: Tabel detail item transaksi penjualan yang sudah ada.

---

## Requirements

### Requirement 1: Konfigurasi Sistem Poin

**User Story:** Sebagai pemilik toko, saya ingin mengatur aturan perolehan poin melalui antarmuka yang mudah, agar sistem poin dapat disesuaikan dengan kebijakan bisnis toko.

#### Acceptance Criteria

1. THE FormGeneralSetting SHALL menyediakan opsi untuk mengaktifkan atau menonaktifkan fitur sistem poin loyalitas secara keseluruhan.
2. WHEN fitur poin diaktifkan, THE FormGeneralSetting SHALL menampilkan pilihan mekanisme perolehan poin: "Per Item (Qty)" atau "Per Kelipatan Nominal".
3. WHERE mekanisme "Per Item (Qty)" dipilih, THE FormMasterPoin SHALL memungkinkan pengguna mengatur nilai Poin_Per_Qty (jumlah poin per 1 satuan qty item terjual).
4. WHERE mekanisme "Per Kelipatan Nominal" dipilih, THE FormMasterPoin SHALL memungkinkan pengguna mengatur nilai Kelipatan_Nominal (nilai Rp belanja yang menghasilkan 1 poin).
5. THE FormMasterPoin SHALL memungkinkan pengguna mengatur nilai Minimum_Redeem (jumlah poin minimum untuk dapat melakukan penukaran barang).
6. WHEN pengguna menyimpan konfigurasi poin, THE Loyalty_Engine SHALL menyimpan semua nilai konfigurasi ke tabel `poin_config` dalam satu transaksi atomik.
7. IF nilai konfigurasi poin tidak valid (negatif atau nol untuk field yang wajib positif), THEN THE FormMasterPoin SHALL menampilkan pesan kesalahan yang deskriptif dan menolak penyimpanan.

---

### Requirement 2: Perolehan Poin saat Transaksi Penjualan

**User Story:** Sebagai kasir, saya ingin sistem secara otomatis menghitung dan mencatat poin yang diperoleh pelanggan saat transaksi selesai, tanpa mengubah alur transaksi yang sudah berjalan.

#### Acceptance Criteria

1. WHEN pelanggan dipilih pada FormJual dan fitur poin aktif, THE FormJual SHALL menampilkan Saldo_Poin pelanggan saat ini di area informasi pelanggan.
2. WHEN transaksi penjualan disimpan dengan pelanggan terdaftar dan fitur poin aktif, THE Loyalty_Engine SHALL menghitung poin yang diperoleh berdasarkan Earn_Rate yang dikonfigurasi.
3. WHERE mekanisme "Per Item (Qty)" aktif, THE Loyalty_Engine SHALL menghitung total poin sebagai jumlah dari (QTY_SATUAN setiap item × Poin_Per_Qty) untuk semua item dalam transaksi.
4. WHERE mekanisme "Per Kelipatan Nominal" aktif, THE Loyalty_Engine SHALL menghitung total poin sebagai hasil pembagian bulat (floor) dari (GRAND_TOTAL_STL_PAJAK ÷ Kelipatan_Nominal).
5. WHEN poin berhasil dihitung dan nilainya lebih dari nol, THE Loyalty_Engine SHALL mencatat baris baru di Poin_Ledger dengan tipe "EARN", nomor faktur, jumlah poin, dan timestamp — dalam transaksi database yang sama dengan penyimpanan penjualan.
6. WHEN poin dicatat di Poin_Ledger, THE Loyalty_Engine SHALL menambahkan jumlah poin ke kolom SALDO_POIN di tbl_pelanggan dalam transaksi database yang sama.
7. IF transaksi penjualan dibatalkan (rollback database), THEN THE Loyalty_Engine SHALL memastikan tidak ada poin yang tercatat di Poin_Ledger untuk transaksi tersebut.
8. WHILE pelanggan tidak dipilih pada FormJual, THE Loyalty_Engine SHALL tidak menghitung atau mencatat poin apapun.
9. IF poin yang dihitung bernilai nol (misalnya total belanja di bawah Kelipatan_Nominal), THEN THE Loyalty_Engine SHALL tidak mencatat baris di Poin_Ledger.

---

### Requirement 3: Penukaran Poin dengan Barang (FormTukarPoin)

**User Story:** Sebagai kasir, saya ingin ada menu khusus penukaran poin di mana pelanggan dapat menukarkan poin mereka dengan barang pilihan, agar proses penukaran tidak mengganggu alur transaksi penjualan normal.

#### Acceptance Criteria

1. THE FormTukarPoin SHALL dapat diakses dari menu utama aplikasi sebagai menu tersendiri.
2. WHEN kasir membuka FormTukarPoin, THE FormTukarPoin SHALL memungkinkan kasir memilih pelanggan berdasarkan nama atau kode pelanggan.
3. WHEN pelanggan dipilih, THE FormTukarPoin SHALL menampilkan Saldo_Poin pelanggan saat ini.
4. IF Saldo_Poin pelanggan kurang dari Minimum_Redeem, THEN THE FormTukarPoin SHALL menampilkan pesan bahwa poin belum mencukupi untuk penukaran dan menonaktifkan tombol proses penukaran.
5. THE FormTukarPoin SHALL menampilkan daftar barang yang dapat ditukar beserta Harga_Poin masing-masing barang.
6. WHEN kasir memilih barang dan qty yang ingin ditukar, THE FormTukarPoin SHALL menghitung total poin yang dibutuhkan dan menampilkan sisa poin setelah penukaran.
7. IF total poin yang dibutuhkan melebihi Saldo_Poin pelanggan, THEN THE FormTukarPoin SHALL menampilkan pesan kekurangan poin dan menonaktifkan tombol konfirmasi.
8. WHEN kasir mengkonfirmasi penukaran, THE Loyalty_Engine SHALL mencatat baris baru di Poin_Ledger dengan tipe "REDEEM", nomor referensi penukaran, jumlah poin yang ditukarkan (nilai negatif), dan timestamp.
9. WHEN penukaran dikonfirmasi, THE Loyalty_Engine SHALL mengurangi SALDO_POIN di tbl_pelanggan dan mengurangi stok barang yang ditukar — dalam satu transaksi database atomik.
10. WHEN penukaran berhasil, THE FormTukarPoin SHALL mencetak bukti penukaran yang mencantumkan: nama pelanggan, barang yang ditukar, poin yang digunakan, dan sisa saldo poin.
11. THE FormTukarPoin SHALL menghasilkan nomor referensi penukaran yang unik dengan format "TP-YYYYMMDD-XXXX".

---

### Requirement 4: Pengaturan Harga Poin Barang

**User Story:** Sebagai pemilik toko, saya ingin mengatur berapa poin yang dibutuhkan untuk menebus setiap barang, agar nilai tukar poin dapat disesuaikan per produk.

#### Acceptance Criteria

1. THE FormMasterPoin SHALL menyediakan tab atau bagian khusus untuk mengatur Harga_Poin per barang.
2. WHEN pengguna membuka pengaturan Harga_Poin, THE FormMasterPoin SHALL menampilkan daftar barang dari tbl_barang yang dapat dicari berdasarkan nama atau kode barang.
3. THE FormMasterPoin SHALL memungkinkan pengguna mengatur nilai Harga_Poin untuk setiap barang (jumlah poin yang dibutuhkan untuk menebus 1 unit barang tersebut).
4. WHEN Harga_Poin suatu barang diatur, THE FormMasterPoin SHALL menyimpan nilai tersebut ke tabel `poin_barang` yang berelasi dengan ID barang.
5. IF Harga_Poin tidak diatur untuk suatu barang, THEN THE FormTukarPoin SHALL tidak menampilkan barang tersebut dalam daftar barang yang dapat ditukar.
6. THE FormMasterPoin SHALL memungkinkan pengguna mengaktifkan atau menonaktifkan ketersediaan penukaran untuk barang tertentu tanpa menghapus Harga_Poin yang sudah diatur.

---

### Requirement 5: Konsistensi Poin saat Retur Penjualan

**User Story:** Sebagai pemilik toko, saya ingin poin yang diperoleh dari transaksi yang diretur dikurangi secara otomatis, agar saldo poin pelanggan selalu akurat.

#### Acceptance Criteria

1. WHEN transaksi penjualan yang memiliki catatan poin EARN diretur penuh, THE Loyalty_Engine SHALL mencatat baris baru di Poin_Ledger dengan tipe "VOID_EARN" sebesar seluruh poin yang pernah diperoleh dari transaksi tersebut (nilai negatif).
2. WHEN void poin dicatat, THE Loyalty_Engine SHALL mengurangi SALDO_POIN di tbl_pelanggan dalam transaksi database yang sama dengan penyimpanan retur.
3. IF retur hanya sebagian item (retur parsial), THEN THE Loyalty_Engine SHALL menghitung poin yang dibatalkan secara proporsional berdasarkan nilai item yang diretur terhadap total nilai transaksi asal.
4. IF transaksi penjualan asal tidak memiliki catatan poin EARN di Poin_Ledger, THEN THE Loyalty_Engine SHALL tidak mencatat VOID_EARN dan tidak mengubah SALDO_POIN.
5. IF pengurangan poin saat retur akan menyebabkan SALDO_POIN menjadi negatif, THEN THE Loyalty_Engine SHALL membatasi pengurangan maksimal sebesar SALDO_POIN yang tersedia (tidak boleh negatif) dan mencatat selisihnya di log.

---

### Requirement 6: Tampilan Poin pada Struk Penjualan

**User Story:** Sebagai pelanggan, saya ingin melihat poin yang saya peroleh dan saldo poin terkini di struk belanja, agar saya mengetahui akumulasi reward saya.

#### Acceptance Criteria

1. WHEN struk penjualan dicetak untuk transaksi dengan pelanggan terdaftar dan fitur poin aktif, THE ModulePrinterJual SHALL mencetak jumlah poin yang diperoleh dari transaksi tersebut.
2. WHEN struk penjualan dicetak untuk pelanggan terdaftar dan fitur poin aktif, THE ModulePrinterJual SHALL mencetak Saldo_Poin pelanggan setelah transaksi (saldo akhir terkini).
3. WHILE pelanggan tidak dipilih pada transaksi, THE ModulePrinterJual SHALL tidak mencetak informasi poin apapun pada struk.
4. THE ModulePrinterJual SHALL mencetak informasi poin pada semua mode cetak yang didukung: thermal (ESC/POS), dot matrix, dan GDI+.
5. IF poin yang diperoleh dari transaksi bernilai nol, THEN THE ModulePrinterJual SHALL tetap mencetak Saldo_Poin terkini pelanggan tanpa baris "Poin Diperoleh".

---

### Requirement 7: Riwayat Poin Pelanggan

**User Story:** Sebagai pemilik toko, saya ingin melihat riwayat transaksi poin per pelanggan, agar saya dapat memantau aktivitas program loyalitas dan menyelesaikan sengketa poin.

#### Acceptance Criteria

1. THE FormMasterPoin SHALL menyediakan tampilan riwayat transaksi poin per pelanggan yang menampilkan: tanggal, nomor referensi (faktur/penukaran), tipe transaksi (EARN/REDEEM/VOID_EARN), jumlah poin, dan saldo poin setelah transaksi.
2. WHEN pengguna memilih pelanggan di riwayat poin, THE FormMasterPoin SHALL menampilkan Saldo_Poin terkini pelanggan tersebut.
3. THE FormMasterPoin SHALL memungkinkan pengguna memfilter riwayat poin berdasarkan rentang tanggal.
4. THE Loyalty_Engine SHALL memastikan bahwa rekonstruksi Saldo_Poin dari agregasi seluruh baris Poin_Ledger menghasilkan nilai yang identik dengan kolom SALDO_POIN di tbl_pelanggan.

---

### Requirement 8: Integritas Data Poin

**User Story:** Sebagai pemilik toko, saya ingin setiap perubahan poin tercatat dengan aman, agar program loyalitas dapat dipercaya.

#### Acceptance Criteria

1. THE Poin_Ledger SHALL menyimpan setiap transaksi poin sebagai baris immutable — baris yang sudah tersimpan tidak dapat diubah atau dihapus melalui antarmuka aplikasi.
2. WHEN poin dicatat (EARN atau REDEEM), THE Loyalty_Engine SHALL menyimpan referensi nomor faktur atau nomor penukaran yang menjadi dasar transaksi poin tersebut.
3. THE Loyalty_Engine SHALL memastikan bahwa operasi pencatatan poin dan penyimpanan transaksi (penjualan atau penukaran) dilakukan dalam satu transaksi database atomik yang sama.
4. IF terjadi kegagalan database saat menyimpan poin, THEN THE Loyalty_Engine SHALL melakukan rollback seluruh transaksi dan menampilkan pesan kesalahan dalam bahasa Indonesia kepada kasir.
5. THE Loyalty_Engine SHALL memastikan SALDO_POIN di tbl_pelanggan tidak pernah bernilai negatif setelah operasi apapun.
