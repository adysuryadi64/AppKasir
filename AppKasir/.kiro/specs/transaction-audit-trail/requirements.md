# Requirements Document

## Introduction

Fitur **Transaction Audit Trail** untuk AppKasir adalah mekanisme pencatatan otomatis setiap kali transaksi diedit atau dihapus oleh pengguna. Tujuan utamanya adalah mendeteksi kasir yang secara diam-diam menghapus atau mengubah transaksi (penjualan, pembelian, pembayaran hutang/piutang, retur) tanpa sepengetahuan pemilik atau admin.

Sistem ini bekerja di latar belakang — kasir tidak perlu melakukan tindakan apapun secara eksplisit. Setiap operasi hapus dan edit pada transaksi akan dicatat secara otomatis ke tabel audit khusus, lengkap dengan identitas pelaku, waktu kejadian, data sebelum perubahan (HEADER + DETAIL ITEM), dan ringkasan perubahan yang dilakukan.

Pemilik/admin dapat melihat laporan audit melalui form khusus, memfilter berdasarkan user, jenis transaksi, dan rentang tanggal, serta mengekspor laporan tersebut.

---

## Glossary

- **AuditTrail_System**: Komponen sistem yang bertanggung jawab mencatat semua aktivitas edit dan hapus transaksi ke tabel `tbl_audit_trail`.
- **AuditTrail_Viewer**: Form laporan yang menampilkan dan memfilter data audit trail kepada admin/pemilik.
- **Snapshot**: Salinan data transaksi (HEADER + DETAIL ITEM dalam format PLAIN TEXT) yang diambil sesaat sebelum operasi edit atau hapus dieksekusi.
- **Jenis_Aksi**: Kategori tindakan yang dicatat — nilai yang valid: `HAPUS`, `EDIT`, `TAMBAH_STOK`, `KURANG_STOK`. Nilai `TAMBAH_STOK` dan `KURANG_STOK` digunakan khusus untuk operasi tambah/kurang stok manual dari `FormBarang`, bukan untuk operasi edit atau hapus data.
- **Jenis_Transaksi**: Kategori transaksi yang diaudit — nilai yang valid: `Penjualan`, `Pembelian`, `Retur Penjualan`, `Retur Pembelian`, `Bayar Hutang`, `Bayar Piutang`.
- **User_Aktif**: Pengguna yang sedang login, diambil dari `FormUtama.StatusNamaUser.Text`.
- **Lokasi_Aktif**: Lokasi toko/gudang yang sedang aktif, diambil dari `FormUtama.StatusLokasi.Text`.
- **Komputer_Aktif**: Nama komputer/PC yang sedang digunakan, diambil dari `FormUtama.StatusNamaPC.Text`.
- **ModuleAuditTrail**: Module VB.NET global yang menyediakan prosedur pencatatan audit trail, dapat dipanggil dari semua form transaksi.
- **tbl_audit_trail**: Tabel MySQL yang menyimpan semua record audit trail.
- **Admin_Level**: Level user yang memiliki hak akses untuk melihat laporan audit trail (level `Admin` atau `Owner`).
- **Storage_Manager**: Komponen dalam `ModuleAuditTrail` yang bertanggung jawab atas arsip otomatis dan penerapan retensi policy pada data audit trail.
- **tbl_audit_trail_arsip**: Tabel MySQL cadangan yang menyimpan record audit trail yang sudah melewati batas usia retensi aktif, dipindahkan dari `tbl_audit_trail`.
- **Retensi_Aktif**: Jumlah bulan maksimum data audit disimpan di tabel `tbl_audit_trail` sebelum dipindahkan ke arsip. Nilai default: 3 bulan. Dapat dikonfigurasi oleh Admin/Owner.
- **AuditTrail_Arsip_Viewer**: Form laporan yang menampilkan data dari tabel `tbl_audit_trail_arsip` untuk keperluan penelusuran data lama.
- **Kategori_Risiko**: Tingkat kekritisan form yang diaudit — nilai yang valid: `KRITIS` (form yang dapat digunakan untuk eskalasi hak akses, manipulasi data master keuangan, atau penghapusan jejak) dan `MENENGAH` (form yang dapat digunakan untuk manipulasi data operasional dengan dampak finansial tidak langsung).
- **Identifier_Audit**: Pengganti `no_faktur` untuk form non-transaksi — kolom unik yang mengidentifikasi record yang diaudit, misalnya `kode_user`, `kode_barang`, `kode_karyawan`, `nama_setting`, `no_opname`, `kode_master_gaji`, `no_slip_gaji`, `no_bon`, `no_transfer`, `no_jurnal`, `kode_referensi`. Nilai ini disimpan di kolom `identifier` pada tabel `tbl_audit_trail` dengan prefix jenis form untuk menghindari tabrakan nilai.
- **Format Keterangan Lengkap**: Format kolom `ket` yang mencakup HEADER + SEMUA DETAIL ITEM transaksi dalam plain text yang mudah dibaca.

---

## Requirements

### Requirement 1: Pencatatan Otomatis Saat Hapus Transaksi

**User Story:** Sebagai pemilik toko, saya ingin setiap penghapusan transaksi dicatat secara otomatis, sehingga saya dapat mengetahui siapa yang menghapus transaksi apa dan kapan, beserta detail semua item yang ada di transaksi tersebut.

#### Acceptance Criteria

1. WHEN operasi hapus transaksi dieksekusi pada tabel `penjualan`, `pembelian`, `retur_penjualan`, `retur_pembelian`, `hutang`, atau `Piutang`, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` sebelum data dihapus dari database.

2. THE AuditTrail_System SHALL mencatat kolom-kolom berikut pada setiap record audit: `id_audit` (auto increment), `waktu_aksi` (DATETIME dengan presisi detik), `jenis_aksi` (nilai: `HAPUS`), `jenis_trans` (nama jenis transaksi), `identifier` (no faktur), `id_user`, `lokasi`, `komputer`, `ket` (PLAIN TEXT LENGKAP berisi HEADER + DETAIL ITEM).

3. WHEN pengambilan Snapshot gagal karena data tidak ditemukan di database, THE AuditTrail_System SHALL tetap menyimpan record audit dengan kolom `ket` berisi pesan `"Data tidak ditemukan saat snapshot"`.

4. IF terjadi exception saat menyimpan record audit ke `tbl_audit_trail`, THEN THE AuditTrail_System SHALL mencatat pesan error ke tabel `History` (tabel log yang sudah ada) dan melanjutkan proses hapus transaksi tanpa menghentikan operasi utama.

5. THE AuditTrail_System SHALL mencatat audit hapus untuk semua jenis transaksi berikut: Penjualan (dari `FormUtama.Hapuspenjualan`), Pembelian (dari `FormUtama.Hapusbelanja`), Retur Penjualan (dari `FormUtama.Hapusreturpenjualan`), Retur Pembelian (dari `FormUtama.Hapusreturpembelian`), Bayar Hutang (dari `FormUtama.Hapusbayarhutang`), Bayar Piutang (dari `FormUtama.HapusbayarPiutang`).

6. FORMAT kolom `ket` untuk transaksi dengan detail item (Penjualan, Pembelian, Retur):
   ```
   [KRITIS] Hapus penjualan
   {NO_FAKTUR} | {TANGGAL} | Rp {TOTAL} | {NAMA_PELANGGAN/SUPPLIER} | {STATUS} | oleh:{USER}
     1. {NAMA_BARANG} | {QTY} {SATUAN} | Rp {HARGA} | Rp {TOTAL}
     2. {NAMA_BARANG} | {QTY} {SATUAN} | Rp {HARGA} | Rp {TOTAL}
   ```
   Contoh:
   ```
   [KRITIS] Hapus penjualan
   PJ-2604200003 | 2026-04-20 02:35 | Rp 17.700 | AGUNG JAYA NGLONGAH | Belum Lunas | oleh:Programer
     1. Gula Rose Brand Kuning Pcs | 1.00 Pcs | Rp 17.700 | Rp 17.700
   ```

---

### Requirement 2: Pencatatan Otomatis Saat Edit Transaksi

**User Story:** Sebagai pemilik toko, saya ingin setiap perubahan pada transaksi yang sudah tersimpan dicatat secara otomatis, sehingga saya dapat melihat nilai sebelum dan sesudah perubahan beserta semua detail itemnya.

#### Acceptance Criteria

1. WHEN operasi simpan pada mode edit transaksi dieksekusi (ditandai dengan `TxtJenisTrans.Text = "EditPembelian"` atau `TxtJenistransaksi.Text = "EditPenjualan"` atau mode edit pada form retur/transfer), THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_aksi` = `EDIT` sebelum data diperbarui di database.

2. THE AuditTrail_System SHALL mengisi kolom `ket` dengan PLAIN TEXT LENGKAP berisi HEADER + DETAIL ITEM transaksi yang diambil langsung dari database menggunakan `no_faktur` yang sedang diedit, bukan dari nilai yang tampil di form.

3. THE AuditTrail_System SHALL mencatat audit edit untuk semua jenis transaksi berikut: Edit Penjualan (dari `FormPenjualan` mode `EditPenjualan`), Edit Pembelian (dari `FormPembelian` mode `EditPembelian`), Edit Retur Pembelian (dari `FormReturBeli` mode edit), Edit Retur Penjualan (dari `FormReturPenjualan` mode edit), Edit Bayar Piutang (dari `FormEditBayarJual`).

4. IF `no_faktur` kosong atau NULL saat operasi edit dieksekusi, THEN THE AuditTrail_System SHALL melewati pencatatan audit dan mencatat pesan peringatan ke tabel `History`.

5. FORMAT kolom `ket` untuk edit transaksi sama dengan Requirement 1, hanya prefix yang berubah menjadi `[KRITIS] Edit {Jenis Transaksi}`.

---

### Requirement 3: Struktur Tabel Database Audit Trail

**User Story:** Sebagai developer, saya ingin tabel audit trail dirancang dengan benar, sehingga data audit dapat disimpan dan diquery secara efisien, dan semua informasi penting dapat ditampung tanpa kompresi atau JSON.

#### Acceptance Criteria

1. THE AuditTrail_System SHALL menggunakan tabel `tbl_audit_trail` dengan struktur kolom:
   - `id_audit` INT AUTO_INCREMENT PRIMARY KEY
   - `waktu_aksi` DATETIME NOT NULL
   - `jenis_aksi` CHAR(12) NOT NULL ('HAPUS','EDIT','TAMBAH_STOK','KURANG_STOK')
   - `jenis_trans` VARCHAR(20) NOT NULL
   - `identifier` VARCHAR(35) NOT NULL
   - `id_user` VARCHAR(30) NOT NULL
   - `lokasi` CHAR(6) NULL ('TOKO' atau 'GUDANG')
   - `komputer` VARCHAR(30) NULL
   - `ket` TEXT NULL (menyimpan HEADER + DETAIL ITEM dalam plain text)

2. THE AuditTrail_System SHALL membuat index pada kolom `waktu_aksi`, `id_user`, dan `identifier` untuk mendukung query filter yang cepat.

3. THE AuditTrail_System SHALL menyediakan script SQL migrasi yang aman dijalankan berulang kali (menggunakan `CREATE TABLE IF NOT EXISTS`) sehingga tidak merusak data yang sudah ada.

4. WHEN script migrasi dijalankan pada database yang sudah memiliki tabel `tbl_audit_trail`, THE AuditTrail_System SHALL tidak mengubah atau menghapus data yang sudah ada di tabel tersebut.

5. THE AuditTrail_System SHALL menghapus kolom `data_sebelum` jika masih ada (format lama menggunakan MEDIUMBLOB/JSON), dan memastikan kolom `ket` menggunakan tipe `TEXT` (bukan VARCHAR) untuk menampung informasi lengkap.

---

### Requirement 4: Module Global Pencatatan Audit Trail

**User Story:** Sebagai developer, saya ingin ada satu module global yang dapat dipanggil dari semua form transaksi, sehingga implementasi audit trail konsisten dan tidak duplikasi kode.

#### Acceptance Criteria

1. THE ModuleAuditTrail SHALL menyediakan prosedur `CatatAudit(noFaktur As String, jenisAksi As String, jenisTransaksi As String, Optional ket As String = "", Optional trans As MySqlTransaction = Nothing)` yang dapat dipanggil dari semua form transaksi.

2. WHEN `CatatAudit` dipanggil, THE ModuleAuditTrail SHALL mengambil Snapshot data HEADER + DETAIL ITEM transaksi dari database berdasarkan `noFaktur` dan `jenisTransaksi` menggunakan query SELECT sebelum operasi DML dieksekusi.

3. THE ModuleAuditTrail SHALL mengambil nilai `id_user` dari `FormUtama.StatusNamaUser.Text`, `lokasi` dari `FormUtama.StatusLokasi.Text`, dan `komputer` dari `FormUtama.StatusNamaPC.Text` pada saat prosedur dipanggil.

4. THE ModuleAuditTrail SHALL menggunakan koneksi database `conn` yang sudah ada (variabel global di `ModuleVariabel`) dan TIDAK membuat koneksi baru.

5. IF `CatatAudit` dipanggil dengan `noFaktur` yang kosong atau hanya spasi, THEN THE ModuleAuditTrail SHALL keluar dari prosedur tanpa melakukan operasi apapun ke database.

6. THE ModuleAuditTrail SHALL menyerialisasi data Snapshot ke format PLAIN TEXT dengan struktur HEADER diikuti DETAIL ITEM, tanpa kompresi dan tanpa JSON.

7. THE ModuleAuditTrail SHALL menyediakan prosedur tambahan `CatatAuditMaster(identifier As String, jenisAksi As String, jenisTransaksi As String, snapshotTeks As String, Optional ket As String = "", Optional trans As MySqlTransaction = Nothing)` untuk form non-transaksi, di mana pemanggil bertanggung jawab menyiapkan `snapshotTeks` sebelum memanggil prosedur.

---

### Requirement 5: Form Laporan Audit Trail

**User Story:** Sebagai pemilik/admin, saya ingin melihat laporan semua aktivitas edit dan hapus transaksi dalam satu form, sehingga saya dapat mendeteksi kasir yang berperilaku mencurigakan dan melihat detail semua item transaksi secara langsung.

#### Acceptance Criteria

1. THE AuditTrail_Viewer SHALL menampilkan daftar record audit trail dalam DataGridView dengan kolom: Waktu Aksi, Jenis Aksi, Jenis Transaksi, Identifier, User, Lokasi, Komputer, Keterangan.

2. THE AuditTrail_Viewer SHALL menyediakan filter berdasarkan: rentang tanggal (DateTimePicker awal dan akhir), nama user (ComboBox berisi daftar user yang pernah tercatat), jenis aksi (ComboBox: Semua / HAPUS / EDIT / TAMBAH_STOK / KURANG_STOK), dan jenis transaksi (ComboBox: Semua / Penjualan / Pembelian / dll).

3. WHEN pengguna mengklik baris di DataGridView, THE AuditTrail_Viewer SHALL menampilkan isi kolom `ket` (PLAIN TEXT LENGKAP) dalam panel detail, dengan format yang mudah dibaca (menghormati line break).

4. THE AuditTrail_Viewer SHALL hanya dapat diakses oleh user dengan level `Admin` atau `Owner`, dan TIDAK menampilkan menu atau tombol akses ke form ini untuk user dengan level lain.

5. THE AuditTrail_Viewer SHALL menampilkan jumlah total record yang ditemukan sesuai filter aktif.

6. WHEN pengguna mengklik tombol Export, THE AuditTrail_Viewer SHALL mengekspor data yang sedang ditampilkan (sesuai filter aktif) ke file CSV dengan nama `AuditTrail_[tanggal_export].csv` di folder yang dipilih pengguna melalui dialog SaveFileDialog.

7. WHILE data audit trail sedang dimuat dari database, THE AuditTrail_Viewer SHALL menampilkan indikator loading dan menonaktifkan tombol filter untuk mencegah query ganda.

---

### Requirement 6: Hak Akses dan Keamanan Audit Trail

**User Story:** Sebagai pemilik toko, saya ingin data audit trail tidak dapat dihapus atau dimanipulasi oleh kasir, sehingga integritas data audit terjaga.

#### Acceptance Criteria

1. THE AuditTrail_System SHALL menyimpan record audit menggunakan koneksi database yang sama dengan transaksi utama, sehingga jika transaksi di-rollback maka record audit juga ikut di-rollback (konsistensi transaksional).

2. WHERE fitur hak akses user diaktifkan, THE AuditTrail_Viewer SHALL hanya menampilkan menu akses ke laporan audit trail untuk user dengan level `Admin` atau `Owner` berdasarkan data dari `ModulHakAkses.BacaHakAksesDariCache`.

3. THE tbl_audit_trail SHALL tidak menyediakan tombol hapus atau edit record di dalam AuditTrail_Viewer — data audit bersifat append-only dari sisi aplikasi.

4. IF user dengan level selain `Admin` atau `Owner` mencoba mengakses form AuditTrail_Viewer secara langsung (misalnya melalui kode), THEN THE AuditTrail_Viewer SHALL menutup form dan menampilkan pesan "Akses ditolak. Fitur ini hanya untuk Admin/Owner."

---

### Requirement 7: Integrasi dengan Alur Transaksi yang Ada

**User Story:** Sebagai developer, saya ingin integrasi audit trail tidak mengubah alur transaksi yang sudah berjalan, sehingga risiko regresi pada fitur yang sudah ada minimal.

#### Acceptance Criteria

1. THE ModuleAuditTrail SHALL dipanggil sesaat sebelum operasi DELETE atau UPDATE dieksekusi pada transaksi, bukan sesudahnya, sehingga Snapshot berisi data yang masih ada di database.

2. IF pencatatan audit trail gagal karena exception apapun, THEN THE AuditTrail_System SHALL melanjutkan eksekusi operasi hapus/edit transaksi tanpa menampilkan pesan error kepada kasir — kegagalan audit tidak boleh memblokir operasi transaksi.

3. THE ModuleAuditTrail SHALL menggunakan `ModuleAngka.ParseDecimal` untuk membaca nilai numerik dari database reader sesuai standar input angka AppKasir, dan TIDAK menggunakan `Convert.ToDecimal` langsung pada field yang berpotensi NULL.

4. WHEN `CatatAudit` dipanggil dalam konteks MySqlTransaction yang sedang aktif, THE ModuleAuditTrail SHALL menerima parameter opsional `transaction As MySqlTransaction` dan menggunakan transaksi tersebut untuk INSERT ke `tbl_audit_trail`, sehingga atomisitas terjaga.

5. THE ModuleAuditTrail SHALL tidak menggunakan `Val()`, `Convert.ToDecimal(TextBox.Text)`, atau `AddWithValue` dengan nilai string untuk field angka — semua nilai numerik wajib menggunakan `ModuleAngka.ParseDecimal` sesuai standar AppKasir.

---

### Requirement 8: Notifikasi Aktivitas Mencurigakan

**User Story:** Sebagai pemilik toko, saya ingin mendapat peringatan visual saat ada aktivitas hapus/edit transaksi yang tidak biasa, sehingga saya dapat segera menindaklanjuti.

#### Acceptance Criteria

1. WHEN jumlah record audit dengan `jenis_aksi = 'HAPUS'` oleh satu user dalam satu hari melebihi 5 transaksi, THE AuditTrail_Viewer SHALL menampilkan baris tersebut dengan warna latar belakang merah muda (highlight) di DataGridView untuk memudahkan identifikasi visual.

2. WHEN pemilik/admin membuka AuditTrail_Viewer, THE AuditTrail_Viewer SHALL secara otomatis menampilkan data hari ini sebagai filter default, sehingga aktivitas terbaru langsung terlihat tanpa perlu mengatur filter manual.

3. THE AuditTrail_Viewer SHALL menampilkan ringkasan statistik di bagian atas form: total hapus hari ini, total edit hari ini, dan nama user dengan aktivitas hapus terbanyak hari ini.

---

### Requirement 9: Manajemen Ruang Penyimpanan Audit Trail

**User Story:** Sebagai pemilik toko, saya ingin data audit trail tidak membebani database seiring bertambahnya transaksi, sehingga tabel aktif tetap kecil dan query laporan tetap cepat meskipun toko memproses ratusan transaksi per hari.

#### Acceptance Criteria

1. THE AuditTrail_System SHALL menyediakan tabel `tbl_audit_trail_arsip` dengan struktur kolom yang identik dengan `tbl_audit_trail` untuk menampung record yang sudah melewati batas Retensi_Aktif.

2. WHEN prosedur arsip dijalankan, THE Storage_Manager SHALL memindahkan semua record dari `tbl_audit_trail` yang nilai `waktu_aksi`-nya lebih lama dari Retensi_Aktif bulan ke tabel `tbl_audit_trail_arsip` menggunakan operasi INSERT INTO ... SELECT diikuti DELETE, dalam satu MySqlTransaction.

3. IF terjadi exception selama proses pemindahan arsip, THEN THE Storage_Manager SHALL melakukan rollback transaksi sehingga tidak ada record yang hilang dari kedua tabel, dan mencatat pesan error ke tabel `History`.

4. THE AuditTrail_System SHALL menyediakan konfigurasi Retensi_Aktif yang dapat diubah oleh Admin/Owner melalui form pengaturan, dengan nilai default 3 bulan dan nilai minimum 1 bulan.

5. WHEN Admin/Owner menyimpan perubahan nilai Retensi_Aktif, THE AuditTrail_System SHALL menyimpan nilai tersebut ke tabel konfigurasi yang sudah ada di database dan membaca nilai terbaru setiap kali prosedur arsip dijalankan.

6. THE Storage_Manager SHALL menjalankan prosedur arsip otomatis satu kali per hari, dipicu saat aplikasi pertama kali dibuka oleh user dengan level `Admin` atau `Owner`, dan TIDAK menjalankan prosedur arsip lebih dari satu kali dalam periode 24 jam yang sama.

7. THE AuditTrail_Arsip_Viewer SHALL menampilkan data dari tabel `tbl_audit_trail_arsip` dengan filter dan tampilan yang identik dengan AuditTrail_Viewer, sehingga Admin/Owner dapat menelusuri data lama tanpa perbedaan antarmuka.

8. THE AuditTrail_System SHALL membuat index pada tabel `tbl_audit_trail` hanya pada kolom `waktu_aksi`, `id_user`, dan `identifier` — kolom `jenis_trans` dan `jenis_aksi` TIDAK diberi index tersendiri karena kardinalitasnya rendah dan cukup dicakup oleh query filter biasa.

9. WHEN script migrasi dijalankan, THE AuditTrail_System SHALL membuat tabel `tbl_audit_trail_arsip` menggunakan `CREATE TABLE IF NOT EXISTS` dengan struktur yang identik dengan `tbl_audit_trail`, sehingga script aman dijalankan berulang kali tanpa merusak data arsip yang sudah ada.

---

### Requirement 10: Cakupan Audit Trail yang Diperluas ke Form Non-Transaksi

**User Story:** Sebagai pemilik toko, saya ingin setiap perubahan pada form master dan pengaturan yang rawan dimanipulasi juga dicatat secara otomatis, sehingga saya dapat mendeteksi kasir yang mengubah hak akses, data karyawan, harga barang, atau pengaturan sistem tanpa sepengetahuan saya.

#### Acceptance Criteria

**10.1 — Cakupan Form KRITIS**

1. WHEN operasi `HapusUser`, `UpdateUser`, `NonaktifkanUser`, atau `AktifkanUser` dieksekusi pada `FormUser`, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Master User`, `identifier` = `USER:{kode_user}` yang dioperasikan, dan kolom `ket` berisi ringkasan perubahan dalam plain text.

2. WHEN `BtnSimpan_Click` dieksekusi pada `FormHakUser` dan memanggil `UpdateHakAkses`, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Hak Akses User`, `identifier` = `USER:{UserName}` yang haknya diubah, dan kolom `ket` berisi ringkasan perubahan.

3. WHEN `BtnSimpan_Click` dieksekusi pada `FormGeneralSetting` dan melakukan UPDATE massal tabel `hakaksesuser` berdasarkan Role, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `General Setting`, `identifier` = `SET:{Role}` yang diubah, dan kolom `ket` berisi ringkasan.

4a. WHEN operasi simpan edit data barang dieksekusi pada form `TambahBarang` dalam mode edit (dipanggil dari `BtnUbah_Click` di `FormBarang`), THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_aksi` = `EDIT`, `jenis_trans` = `Master Barang`, `identifier` = `BRG:{kode_barang}`, dan kolom `ket` berisi ringkasan perubahan.

4b. WHEN `BtnSimpanStok_Click` dieksekusi pada `FormBarang` untuk operasi tambah atau kurang stok manual (dipicu dari `TambahStokToolStripMenuItem_Click` atau `KurangiStokToolStripMenuItem_Click`), THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_aksi` = `TAMBAH_STOK` atau `KURANG_STOK` sesuai operasi yang dijalankan, `jenis_trans` = `Stok Manual`, `identifier` = `STK:{noTransaksi}` (format `yyyyMMddHHmmss` dari `DateTime.Now`), dan kolom `ket` berisi ringkasan detail stok. Record ini dicatat pada SETIAP eksekusi tambah/kurang stok manual tanpa terkecuali, dan `Kategori_Risiko` = `KRITIS`.

5. WHEN operasi edit pada record stok opname yang sudah tersimpan dieksekusi pada `FormStokOpname`, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Stok Opname`, `identifier` = `OPN:{no_opname}`, dan kolom `ket` berisi data header stok opname sebelum perubahan.

6. WHEN `SaveOrUpdateDataMasterGaji` dieksekusi pada `FormMasterGaji` dalam mode update (bukan insert baru), THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Master Gaji`, `identifier` = `GAJI:{Kode}` master gaji yang diubah, dan kolom `ket` berisi ringkasan perubahan.

7. WHEN operasi edit pada `FormKaryawan` dieksekusi untuk menyimpan perubahan data karyawan, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Master Karyawan`, `identifier` = `KRY:{kode_karyawan}`, dan kolom `ket` berisi ringkasan perubahan.

**10.2 — Cakupan Form MENENGAH**

8. WHEN operasi edit atau hapus pada slip gaji dieksekusi pada `FormGaji`, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Slip Gaji`, `identifier` = `SLIP:{no_slip_gaji}`, dan kolom `ket` berisi data header slip gaji sebelum perubahan.

9. WHEN operasi edit atau hapus bon karyawan dieksekusi pada `FormBon`, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Bon Karyawan`, `identifier` = `BON:{no_bon}`, dan kolom `ket` berisi data bon sebelum perubahan.

10. WHEN operasi edit atau hapus transfer barang dieksekusi pada form transfer barang antar gudang atau cabang (`FormTransferBarang`, `FormTransferStok`, atau `FormTransferCabang`), THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Transfer Barang`, `identifier` = `TRF:{no_transfer}`, dan kolom `ket` berisi data header transfer sebelum perubahan.

11. WHEN operasi edit atau hapus jurnal keuangan manual dieksekusi pada `FormKeuangan`, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Jurnal Keuangan`, `identifier` = `JRN:{no_jurnal}`, dan kolom `ket` berisi data jurnal sebelum perubahan.

12. WHEN operasi edit atau hapus data referensi dieksekusi pada `FormTabelReferensi`, THE AuditTrail_System SHALL menyimpan satu record ke tabel `tbl_audit_trail` dengan `jenis_trans` = `Tabel Referensi`, `identifier` = `REF:{kode_referensi}`, dan kolom `ket` berisi data referensi sebelum perubahan.

**10.3 — Aturan Umum Form Non-Transaksi**

13. THE AuditTrail_System SHALL menyimpan `identifier` ke kolom `identifier` pada tabel `tbl_audit_trail` dengan format `{PREFIX}:{nilai}` — contoh: `USER:KSR001`, `BRG:B-0042`, `KRY:K-005` — sehingga nilai dari form berbeda tidak bertabrakan dan tetap dapat difilter.

14. IF Snapshot untuk form non-transaksi gagal diambil karena data tidak ditemukan di database, THEN THE AuditTrail_System SHALL tetap menyimpan record audit dengan `ket` = `"Data tidak ditemukan saat snapshot"`, sesuai perilaku yang sama dengan Requirement 1 Kriteria 3.

15. THE AuditTrail_System SHALL mencatat `Kategori_Risiko` pada kolom `ket` sebagai prefix — contoh: `"[KRITIS] Ubah level user"` atau `"[MENENGAH] Edit slip gaji"` — sehingga admin dapat memfilter dan memprioritaskan investigasi berdasarkan tingkat risiko.
