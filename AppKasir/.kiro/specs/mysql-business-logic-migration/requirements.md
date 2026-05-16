# Requirements — Migrasi Business Logic ke MySQL

## Pendahuluan

Sistem kasir AppKasir saat ini berjalan di tiga klien: POS Desktop (VB.NET), Mobile (Flutter),
dan API/Middleware (PHP). Ketiga klien terhubung ke satu database MySQL yang sama.

Masalah utama: logika bisnis kritis seperti generate nomor transaksi, update stok, dan posting
jurnal saat ini dijalankan di sisi klien (VB.NET). Ketika Flutter atau PHP melakukan transaksi
yang sama, logika tersebut harus diimplementasi ulang — dan rawan tidak konsisten.

Spec ini mendokumentasikan **panduan arsitektur** dengan pemisahan tanggung jawab yang tegas:

### Prinsip Utama: Simpan & Hapus Tetap di Klien

> **Semua klien (VB.NET dan PHP) mengelola logika simpan dan hapus secara inline dengan
> MySQL Transaction client-side.**

| Lapisan | VB.NET | Flutter/PHP |
|---------|--------|-------------|
| Generate nomor faktur | `sp_hlp_faktur_generate` ✅ | `sp_hlp_faktur_generate` ✅ |
| Simpan transaksi | Inline SQL + `conn.BeginTransaction()` | Inline SQL + `$pdo->beginTransaction()` |
| Hapus transaksi | Inline SQL + `conn.BeginTransaction()` | Inline SQL + `$pdo->beginTransaction()` |
| Validasi stok sebelum simpan | Inline di VB | `sp_hlp_stok_validasi` (PHP) |
| Hitung stok setelah simpan | `HitungStokPerubahan()` (VB) | `sp_hlp_stok_hitung` (PHP) |
| Update saldo akun | `UpdateSaldoSemuaAkun()` (VB) | `sp_hlp_saldo_akun_update` (PHP) |

**Alasan:** VB.NET sudah punya logika simpan/hapus yang teruji dan berjalan stabil. Memindahkan
logika ini ke SP menambah kompleksitas tanpa manfaat nyata — dan rawan menghilangkan nuansa
logika yang sudah ada (audit trail, cetak nota, reset form, dll). PHP juga melakukan INSERT
inline dengan validasi server via SP helper untuk konsistensi.

---

## Glosarium

- **SP**: Stored Procedure — prosedur tersimpan di MySQL yang dieksekusi di sisi server.
- **Klien**: Salah satu dari VB.NET, Flutter, atau PHP yang terhubung ke MySQL.
- **Transaksi MySQL**: Blok `START TRANSACTION … COMMIT / ROLLBACK` yang menjamin atomisitas.
- **Business Logic Kritis**: Logika yang mempengaruhi integritas data — stok, jurnal, nomor faktur, saldo.
- **Validasi Presentasi**: Validasi yang hanya relevan untuk UI — format input, field kosong, format angka.
- **HitungStokPerubahan**: Prosedur VB.NET existing yang merecalculate `STOK_TOKO` dan `STOK_GUDANG` dari semua counter di `tbl_barang`. Akan dimigrasikan ke `sp_hlp_stok_hitung` di MySQL.
- **UpdateSaldoSemuaAkun**: Prosedur VB.NET existing yang merecalculate `Saldo_Akhir` semua akun dari `JurnalUmum`. **Tetap di VB** tapi diperbaiki bug AKUN_DK-nya. Untuk multi-client (Flutter/PHP), digantikan oleh `sp_hlp_saldo_akun_update` yang dibuat dengan rumus benar dari awal.
- **Faktur**: Nomor unik transaksi dengan format `PJ-YYMMDD-XXXX` (penjualan), `PB-YYMMDD-XXXX` (pembelian), dll.
- **LOKASI**: Nilai `TOKO` atau `GUDANG` — menentukan kolom stok mana yang dioperasikan.
- **Idempoten**: Operasi yang menghasilkan hasil sama meskipun dieksekusi berulang kali.
- **Backdate**: Transaksi dengan tanggal lebih awal dari hari ini — diizinkan atau ditolak berdasarkan hak akses user.

---

## Konvensi Nama Stored Procedure

Semua SP mengikuti pola: `sp_{kategori}_{entitas}_{aksi}`

### Kategori

| Prefix | Kategori | Dipakai untuk |
|--------|----------|---------------|
| `hlp`  | Helper   | Sub-prosedur internal yang dipanggil SP lain — validasi, generate, hitung |
| `trx`  | Transaksi | SP utama yang dipanggil langsung oleh klien — penjualan, pembelian, dll |
| `mst`  | Master   | Operasi data master yang butuh validasi server — barang, pelanggan, dll |
| `bat`  | Batch    | Rekonsiliasi / recalculate massal — dijalankan terjadwal atau manual |

### Daftar SP

**Helper (internal):**

| Nama SP | Keterangan |
|---------|------------|
| `sp_hlp_faktur_generate` | Generate nomor faktur unik, aman multi-user |
| `sp_hlp_stok_validasi` | Cek stok cukup sebelum transaksi |
| `sp_hlp_stok_hitung` | Recalculate `STOK_TOKO` & `STOK_GUDANG` (migrasi `HitungStokPerubahan`) |
| `INSERT INTO JurnalUmum` (inline) | INSERT satu baris ke `JurnalUmum` |
| `sp_hlp_saldo_akun_update` | Recalculate `Saldo_Akhir` satu akun (migrasi `UpdateSaldoSemuaAkun`) |
| `sp_hlp_saldo_kas_validasi` | Cek saldo kas cukup sebelum pengeluaran |

**Transaksi (dipanggil klien):**

> Tidak ada SP transaksi (`sp_trx_*`) — simpan dan hapus transaksi dilakukan inline di klien.

**Batch (rekonsiliasi):**

| Nama SP | Keterangan |
|---------|------------|
| `sp_bat_saldo_semua_akun` | Recalculate `Saldo_Akhir` semua akun di `tbl_datareferensi` |
| `sp_bat_stok_semua_barang` | Recalculate stok semua barang di `tbl_barang` (toko + gudang) |
| `sp_bat_stok_toko` | Recalculate `STOK_TOKO` saja — untuk posting Jenis="Toko" |
| `sp_bat_stok_gudang` | Recalculate `STOK_GUDANG` saja — untuk posting Jenis="Gudang" |
| `sp_bat_piutang_semua_pelanggan` | Recalculate `HutangAkhir` semua pelanggan dari `penjualan` |
| `sp_bat_hutang_semua_supplier` | Recalculate `HutangAkhir` semua supplier dari `pembelian` |
| `sp_bat_bon_semua_karyawan` | Recalculate `SaldoAkhir` semua karyawan dari `Bon_karyawan` |

---

## Ruang Lingkup

### Yang TERMASUK dalam spec ini

1. Panduan arsitektur pemisahan tanggung jawab klien vs MySQL
2. Spesifikasi Helper SP dan Batch SP untuk operasi bisnis kritis
3. Standar pemanggilan SP helper dari VB.NET dan PHP
4. Standar penanganan error
5. Migrasi logika `HitungStokPerubahan` ke SP (`sp_hlp_stok_hitung`). `UpdateSaldoSemuaAkun` tetap di VB tapi diperbaiki bug-nya; untuk PHP digantikan `sp_hlp_saldo_akun_update`
6. Cegah duplicate transaksi via constraint UNIQUE KEY di tabel
7. Generate nomor faktur yang aman untuk multi-user via `sp_hlp_faktur_generate`

### Yang TIDAK termasuk dalam spec ini

- Perubahan skema tabel (DDL ALTER TABLE) — hanya logika prosedural
- Validasi presentasi (format angka, field kosong) — tetap di klien sesuai standar `ModuleAngka.vb`
- Fitur baru yang belum ada di sistem existing
- Migrasi data historis

---

## Requirements

### Requirement 1: Pemisahan Tanggung Jawab Klien vs MySQL

**User Story:** Sebagai developer, saya ingin panduan yang jelas tentang apa yang boleh dan tidak boleh
dilakukan di sisi klien, agar semua klien (VB.NET, Flutter, PHP) berperilaku konsisten.

#### Acceptance Criteria

1. THE Arsitektur_Sistem SHALL mendefinisikan dua lapisan tanggung jawab yang tidak tumpang tindih: lapisan Klien dan lapisan MySQL_Server.
2. THE Lapisan_Klien SHALL menangani validasi presentasi meliputi: format input angka (sesuai `ModuleAngka.vb`), validasi field kosong, validasi format angka, dan feedback UI.
3. THE Lapisan_MySQL_Server SHALL menangani: generate nomor transaksi (`sp_hlp_faktur_generate`), recalculate stok (`sp_hlp_stok_hitung`), recalculate saldo akun (`sp_hlp_saldo_akun_update`), dan validasi stok untuk PHP (`sp_hlp_stok_validasi`).
4. THE Klien_VBNet SHALL mengelola simpan dan hapus transaksi secara inline menggunakan `conn.BeginTransaction()`.
5. THE Klien_VBNet SHALL memanggil SP helper: `sp_hlp_faktur_generate` untuk generate nomor, `HitungStokPerubahan()` untuk recalculate stok (tetap di VB), dan `UpdateSaldoSemuaAkun()` untuk update saldo (tetap di VB dengan bug fix AKUN_DK).
6. THE Klien_PHP SHALL mengelola simpan dan hapus transaksi secara inline menggunakan `$pdo->beginTransaction()`, dengan validasi server via SP helper (`sp_hlp_stok_validasi`, `sp_hlp_stok_hitung`, `sp_hlp_saldo_akun_update`) sebelum/sesudah INSERT.
7. IF sebuah Stored Procedure mendeteksi pelanggaran aturan bisnis, THEN THE SP SHALL mengembalikan kode error beserta pesan deskriptif kepada klien.
8. THE Klien SHALL menampilkan pesan error dari SP kepada pengguna tanpa mengubah substansi pesan.

---

### Requirement 2: Generate Nomor Transaksi yang Aman (Multi-User) ✅ SELESAI

**User Story:** Sebagai kasir, saya ingin nomor faktur yang unik dan tidak pernah duplikat meskipun
dua kasir menyimpan transaksi secara bersamaan, agar tidak ada konflik data.

#### Acceptance Criteria

1. THE `sp_hlp_faktur_generate` SHALL menerima parameter: `p_prefix VARCHAR(5)` (contoh: `PJ`, `PB`, `SO`) dan `p_tanggal DATE`.
2. WHEN `sp_hlp_faktur_generate` dipanggil, THE `sp_hlp_faktur_generate` SHALL menggunakan `SELECT ... FOR UPDATE` pada tabel transaksi utama untuk mengunci baris terakhir sebelum menghitung urutan berikutnya.
3. THE `sp_hlp_faktur_generate` SHALL menghasilkan nomor dengan format `{PREFIX}-{YYMMDD}{XXXX}` di mana `XXXX` adalah urutan 4 digit yang dimulai dari `0001` setiap hari. Format ini konsisten dengan data lama (contoh: `PJ-2604010453`, `TC-2604150001`) — ada `-` setelah prefix, **tidak ada** `-` antara tanggal dan nomor urut.
4. WHEN dua klien memanggil `sp_hlp_faktur_generate` secara bersamaan untuk tanggal yang sama, THE `sp_hlp_faktur_generate` SHALL menghasilkan dua nomor yang berbeda tanpa race condition.
5. THE `sp_hlp_faktur_generate` SHALL hanya cek tabel transaksi utama (`penjualan`, `pembelian`, dst) — **tidak perlu** cek tabel ditahan/draft. Alasannya: nomor draft tidak di-generate ulang saat simpan; nomor draft dipakai langsung dan hanya dicek duplikat di tabel utama. **Catatan:** `sync_penjualan.php` saat ini masih cek `penjualan_ditahan` — ini akan dihapus saat refactor ke SP.
6. IF tidak ada transaksi sebelumnya pada tanggal tersebut, THEN THE `sp_hlp_faktur_generate` SHALL menghasilkan nomor dengan urutan `0001`.

#### Catatan Alur Draft

Dua skenario berbeda yang harus ditangani klien:

| Skenario | Alur |
|----------|------|
| **Transaksi baru** | Klien panggil `sp_hlp_faktur_generate` → dapat nomor baru → simpan inline |
| **Dari draft (ditahan)** | Klien **tidak** panggil `sp_hlp_faktur_generate` — nomor sudah ada dari draft. Klien langsung INSERT inline dengan nomor draft. Cek duplikat di tabel utama sebelum INSERT: jika nomor sudah ada → tampilkan error, jika belum → simpan + hapus dari tabel ditahan |

Ini konsisten dengan perilaku VB.NET: `Nomorjual()` langsung `Return` tanpa generate jika `draftPenjualanAktif` tidak kosong.

---

### Requirement 3: Validasi Stok di Server

**User Story:** Sebagai pemilik toko, saya ingin validasi stok dilakukan di MySQL server, agar
transaksi dari VB.NET maupun Flutter tidak bisa menjual barang yang stoknya tidak cukup
(kecuali hak akses mengizinkan).

#### Acceptance Criteria

1. THE `sp_hlp_stok_validasi` SHALL menerima parameter: `p_kode_barang VARCHAR(50)`, `p_qty_dibutuhkan DECIMAL(15,4)`, `p_lokasi VARCHAR(10)`, `p_izinkan_minus TINYINT(1)`.
2. WHEN `sp_hlp_stok_validasi` dipanggil dengan `p_izinkan_minus = 0`, THE `sp_hlp_stok_validasi` SHALL membaca `STOK_TOKO` atau `STOK_GUDANG` dari `tbl_barang` sesuai `p_lokasi` menggunakan `SELECT ... FOR UPDATE`.
3. IF stok yang tersedia kurang dari `p_qty_dibutuhkan` DAN `p_izinkan_minus = 0`, THEN THE `sp_hlp_stok_validasi` SHALL mengembalikan `p_error_code = 'STOK_KURANG'` beserta nama barang dan stok tersedia.
4. WHERE `p_izinkan_minus = 1`, THE `sp_hlp_stok_validasi` SHALL mengizinkan transaksi berlanjut meskipun stok akan menjadi negatif.
5. THE PHP API SHALL memanggil `sp_hlp_stok_validasi` untuk setiap baris item sebelum melakukan INSERT ke tabel detail transaksi.
6. IF `sp_hlp_stok_validasi` mengembalikan error untuk salah satu item, THEN THE PHP API SHALL melakukan ROLLBACK seluruh transaksi dan mengembalikan pesan error yang menyebutkan nama barang yang bermasalah.

---

### Requirement 4: Pencegahan Duplikat Transaksi

**User Story:** Sebagai developer, saya ingin sistem mencegah transaksi duplikat secara otomatis,
agar tidak ada dua record dengan nomor faktur yang sama meskipun klien mengirim request dua kali.

#### Acceptance Criteria

1. THE Tabel_Penjualan SHALL memiliki constraint `UNIQUE KEY` pada kolom `ID_PENJUALAN`.
2. THE Klien SHALL mengecek duplikat faktur sebelum INSERT — jika nomor sudah ada di tabel utama, tampilkan error `DUPLIKAT_FAKTUR` tanpa melakukan INSERT apapun.
3. THE `sp_hlp_faktur_generate` SHALL menggunakan `SELECT ... FOR UPDATE` sehingga nomor yang digenerate tidak bisa dipakai oleh transaksi lain sebelum di-commit.
4. WHEN klien PHP mengirim request yang sama dua kali karena timeout jaringan, THE klien SHALL mendeteksi duplikat via UNIQUE KEY constraint dan mengembalikan error yang tepat kepada pengguna.

---

### Requirement 5: Update Stok (Migrasi HitungStokPerubahan)

**User Story:** Sebagai developer, saya ingin logika `HitungStokPerubahan` dari VB.NET dimigrasikan
ke MySQL Stored Procedure, agar semua klien menggunakan kalkulasi stok yang identik.

#### Acceptance Criteria

1. THE `sp_hlp_stok_hitung` SHALL menerima parameter `p_kode_barang VARCHAR(50)`.
2. THE `sp_hlp_stok_hitung` SHALL merecalculate `STOK_TOKO` menggunakan formula:
   `STOK_TOKO = AWAL_TOKO + TAMBAH_TOKO - KURANG_TOKO + PEMBELIAN_TOKO - PENJUALAN_TOKO - RETUR_BELI_TOKO + RETUR_JUAL_TOKO + OPNAME_TOKO + TRANSFER_STOK_MASUK_TOKO - TRANSFER_STOK_KELUAR_TOKO + TRANSFER_BARANG_MASUK_TOKO - TRANSFER_BARANG_KELUAR_TOKO + TRANSFER_CABANG_MASUK_TOKO - TRANSFER_CABANG_KELUAR_TOKO`
   dengan `COALESCE(kolom, 0)` untuk setiap kolom.
3. THE `sp_hlp_stok_hitung` SHALL merecalculate `STOK_GUDANG` menggunakan formula yang identik dengan formula TOKO namun menggunakan kolom `*_GUDANG`.
4. THE `sp_hlp_stok_hitung` SHALL dieksekusi di dalam transaksi yang sama dengan operasi yang memicu perubahan stok (penjualan, pembelian, opname, transfer).
5. WHEN `sp_hlp_stok_hitung` dipanggil untuk kode barang yang tidak ada di `tbl_barang`, THE `sp_hlp_stok_hitung` SHALL menyelesaikan eksekusi tanpa error (zero rows affected dianggap valid).
6. THE `sp_hlp_stok_hitung` SHALL idempoten: memanggil SP ini dua kali berturut-turut untuk barang yang sama menghasilkan nilai `STOK_TOKO` dan `STOK_GUDANG` yang sama.

---

### Requirement 6: Insert Jurnal Akuntansi

**User Story:** Sebagai akuntan, saya ingin jurnal akuntansi diposting otomatis oleh MySQL saat
transaksi disimpan, agar tidak ada transaksi yang tersimpan tanpa jurnal yang seimbang.

#### Acceptance Criteria

1. THE `INSERT INTO JurnalUmum` (inline) SHALL menerima parameter: `p_no_transaksi`, `p_tgl_transaksi`, `p_uraian`, `p_nama_akun_d`, `p_nomor_akun_d`, `p_nama_akun_k`, `p_nomor_akun_k`, `p_nominal DECIMAL(15,2)`, `p_jenis_transaksi`, `p_lokasi`, `p_id_user`, `p_id_komputer`.
2. WHEN `INSERT INTO JurnalUmum` (inline) dipanggil, THE `INSERT INTO JurnalUmum` (inline) SHALL melakukan INSERT satu baris ke tabel `JurnalUmum` dengan semua parameter yang diberikan.
3. THE PHP API SHALL memanggil `INSERT INTO JurnalUmum` (inline) untuk setiap entri jurnal yang dibutuhkan setelah INSERT transaksi berhasil.
4. THE `INSERT INTO JurnalUmum` (inline) SHALL dieksekusi di dalam transaksi yang sama dengan INSERT ke tabel transaksi sehingga tidak ada kondisi di mana transaksi tersimpan tanpa jurnal.

---

### Requirement 7: Update Saldo Akun (sp_hlp_saldo_akun_update)

**User Story:** Sebagai developer, saya ingin SP yang menghitung saldo akun dengan benar sesuai
kaidah double-entry, agar saldo akun selalu konsisten setelah setiap transaksi dari klien manapun.

#### Acceptance Criteria

1. THE `sp_hlp_saldo_akun_update` SHALL menerima parameter `p_kode_akun VARCHAR(20)` untuk update saldo satu akun secara targeted.
2. THE `sp_hlp_saldo_akun_update` SHALL merecalculate `Saldo_Akhir` menggunakan formula yang menghormati `AKUN_DK`:
   - Akun `AKUN_DK = 'DEBET'` (kas, piutang, persediaan, biaya): `Saldo_Akhir = Saldo_Awal + SUM(DEBET) - SUM(KREDIT)`
   - Akun `AKUN_DK = 'KREDIT'` (hutang, modal, pendapatan): `Saldo_Akhir = Saldo_Awal - SUM(DEBET) + SUM(KREDIT)`
   - Ini berbeda dari `UpdateSaldoSemuaAkun()` lama di VB.NET yang salah (tidak pakai CASE WHEN AKUN_DK) — SP ini dibuat dengan rumus yang benar dari awal.
3. THE `sp_hlp_saldo_akun_update` SHALL dieksekusi di dalam transaksi yang sama dengan `INSERT INTO JurnalUmum` (inline), setelah semua entri jurnal untuk transaksi tersebut di-INSERT.
4. THE `sp_bat_saldo_semua_akun` (tanpa parameter) SHALL merecalculate `Saldo_Akhir` semua akun sekaligus menggunakan LEFT JOIN aggregate dari `JurnalUmum` dengan rumus `CASE WHEN AKUN_DK` — dipakai untuk rekonsiliasi batch.
5. THE `sp_hlp_saldo_akun_update` SHALL idempoten: memanggil SP ini dua kali untuk akun yang sama menghasilkan nilai `Saldo_Akhir` yang sama.
6. WHEN `sp_hlp_saldo_akun_update` dipanggil untuk kode akun yang tidak ada di `tbl_datareferensi`, THE `sp_hlp_saldo_akun_update` SHALL menyelesaikan eksekusi tanpa error.

---

### Requirement 11: Validasi Saldo Kas / Kas Tidak Cukup

**User Story:** Sebagai pemilik toko, saya ingin sistem mencegah pengeluaran kas yang melebihi
saldo kas yang tersedia, agar saldo akun kas tidak menjadi negatif tanpa disengaja.

#### Acceptance Criteria

1. THE `sp_hlp_saldo_kas_validasi` SHALL menerima parameter: `p_kode_akun VARCHAR(20)`, `p_nominal_keluar DECIMAL(15,2)`.
2. WHEN `sp_hlp_saldo_kas_validasi` dipanggil, THE `sp_hlp_saldo_kas_validasi` SHALL membaca `Saldo_Akhir` dari `tbl_datareferensi` untuk `p_kode_akun` menggunakan `SELECT ... FOR UPDATE`.
3. IF `Saldo_Akhir < p_nominal_keluar`, THEN THE `sp_hlp_saldo_kas_validasi` SHALL mengembalikan `p_error_code = 'SALDO_KAS_KURANG'` beserta nama akun dan saldo tersedia.
4. THE PHP API SHALL memanggil `sp_hlp_saldo_kas_validasi` sebelum melakukan INSERT jurnal pengeluaran kas (bayar hutang, bayar piutang).
5. WHERE konfigurasi `izinkan_saldo_minus = 1` aktif untuk akun tertentu, THE `sp_hlp_saldo_kas_validasi` SHALL melewati pengecekan saldo dan mengizinkan transaksi berlanjut.

---

### Requirement 12: Standar Pemanggilan SP dari Klien

**User Story:** Sebagai developer VB.NET, Flutter, dan PHP, saya ingin standar yang seragam untuk
memanggil Stored Procedure, agar kode di ketiga klien konsisten dan mudah di-maintain.

#### Pembagian Tanggung Jawab per Klien

| SP | VB.NET | PHP |
|----|--------|-----|
| `sp_hlp_faktur_generate` | ✅ Wajib dipakai | ✅ Wajib dipakai |
| `sp_hlp_stok_validasi` | ❌ Validasi inline di VB | ✅ Wajib dipanggil sebelum INSERT |
| `sp_hlp_stok_hitung` | ❌ Pakai `HitungStokPerubahan()` | ✅ Wajib dipanggil setelah UPDATE counter |
| `sp_hlp_saldo_akun_update` | ❌ Pakai `UpdateSaldoSemuaAkun()` | ✅ Wajib dipanggil setelah INSERT jurnal |
| `sp_bat_*` | ✅ Untuk rekonsiliasi manual | ✅ Untuk rekonsiliasi manual |

#### Acceptance Criteria

1. THE Klien_VBNet SHALL memanggil `sp_hlp_faktur_generate` untuk generate nomor faktur menggunakan `MySqlCommand` dengan parameter `MySqlDbType` yang sesuai.
2. THE Klien_VBNet SHALL **tidak** memanggil SP transaksi — logika simpan dan hapus tetap inline dengan `conn.BeginTransaction()`.
3. THE Klien_PHP SHALL memanggil `sp_hlp_faktur_generate`, `sp_hlp_stok_validasi`, `sp_hlp_stok_hitung`, dan `sp_hlp_saldo_akun_update` menggunakan PDO dalam satu transaksi inline.
4. THE Klien_Flutter SHALL memanggil PHP API endpoint — Flutter tidak boleh terhubung langsung ke MySQL.
5. WHEN sebuah SP helper mengembalikan error, THE Klien SHALL membaca kode error dan menampilkan pesan yang sesuai kepada pengguna.
6. THE Klien SHALL tidak melakukan retry otomatis untuk error bisnis (STOK_KURANG, DUPLIKAT_FAKTUR, SALDO_KAS_KURANG) — error ini harus ditampilkan ke pengguna untuk ditindaklanjuti.
7. THE Klien SHALL melakukan retry maksimal 1 kali untuk error koneksi/timeout sebelum menampilkan pesan error kepada pengguna.
8. THE `sync_penjualan.php` SHALL melakukan INSERT inline dengan validasi server via SP helper — menghapus semua logika `SELECT MAX` inline untuk generate nomor.
9. THE `sync_stokopname.php` SHALL melakukan INSERT inline dengan validasi server via SP helper — menghapus semua logika `SELECT MAX` inline untuk generate nomor.
10. THE PHP API SHALL menggunakan SP helper untuk semua operasi yang mempengaruhi integritas data (stok, jurnal, saldo).

---

### Requirement 13: Penanganan Error dan Rollback

**User Story:** Sebagai developer, saya ingin setiap SP menangani error secara konsisten dengan
rollback otomatis, agar tidak ada data yang tersimpan sebagian (partial commit).

#### Acceptance Criteria

1. THE SP SHALL menggunakan blok `DECLARE EXIT HANDLER FOR SQLEXCEPTION` untuk menangkap semua error SQL yang tidak terduga.
2. WHEN `EXIT HANDLER` terpicu, THE SP SHALL melakukan `ROLLBACK`, mengisi `p_success = 0`, dan mengisi `p_error_message` dengan pesan error dari `GET DIAGNOSTICS CONDITION`.
3. THE SP SHALL menggunakan `SAVEPOINT` untuk operasi yang bisa di-retry secara parsial di dalam transaksi yang lebih besar.
4. IF sebuah SP dipanggil di luar transaksi eksplisit, THEN THE SP SHALL memulai transaksinya sendiri dengan `START TRANSACTION` dan melakukan `COMMIT` atau `ROLLBACK` sebelum return.
5. THE Klien SHALL selalu memeriksa `p_success` dari OUT parameter sebelum menganggap operasi berhasil — tidak boleh menganggap berhasil hanya karena tidak ada exception di sisi klien.

---

### Requirement 14: Konsistensi Kalkulasi Angka antara Klien dan SP

**User Story:** Sebagai developer, saya ingin kalkulasi angka di SP MySQL menghasilkan nilai yang
identik dengan kalkulasi di VB.NET menggunakan `ModuleAngka`, agar tidak ada selisih pembulatan.

#### Acceptance Criteria

1. THE SP SHALL menggunakan tipe data `DECIMAL(15,4)` untuk qty dan `DECIMAL(15,2)` untuk nilai rupiah — konsisten dengan tipe kolom di tabel MySQL.
2. THE SP SHALL menggunakan `ROUND(nilai, 2)` untuk nilai rupiah akhir yang disimpan ke tabel, konsisten dengan pembulatan yang dilakukan `ModuleAngka.ParseDecimal` di VB.NET.
3. THE Klien SHALL mengirim nilai angka ke SP sebagai tipe numerik (bukan string) — VB.NET menggunakan `MySqlDbType.Decimal`, PHP menggunakan PDO binding dengan nilai float/int, Flutter mengirim via JSON dengan tipe number.
4. THE SP SHALL menggunakan `COALESCE(kolom, 0)` untuk semua kolom yang bisa NULL dalam kalkulasi, konsisten dengan pola `If(IsDBNull(rd("kolom")), 0D, ...)` di VB.NET.
5. WHEN klien mengirim nilai yang sudah diparse oleh `ModuleAngka.ParseDecimal` (VB.NET) atau `double.parse` (Flutter/PHP), THE SP SHALL menerima nilai tersebut tanpa konversi tambahan.

---

### Requirement 15: Migrasi Bertahap — Kompatibilitas Mundur

**User Story:** Sebagai developer, saya ingin migrasi dilakukan secara bertahap tanpa mematikan
sistem yang sedang berjalan, agar POS desktop tetap bisa beroperasi selama proses migrasi.

#### Acceptance Criteria

1. THE SP SHALL dibuat sebagai tambahan (CREATE PROCEDURE IF NOT EXISTS) — tidak menghapus atau mengubah tabel yang sudah ada.
2. WHEN SP helper sudah tersedia di MySQL, THE Klien_VBNet SHALL diupdate untuk memanggil SP helper menggantikan logika inline (generate nomor, hitung stok, update saldo), satu modul per satu modul.
3. WHILE proses migrasi berlangsung, THE Sistem SHALL memastikan tidak ada dua versi logika yang berjalan bersamaan untuk transaksi yang sama.
4. THE `sp_hlp_stok_hitung` di MySQL SHALL menghasilkan nilai `STOK_TOKO` dan `STOK_GUDANG` yang identik dengan hasil `HitungStokPerubahan` di `ModuleVariabel.vb` untuk data yang sama.
5. THE `sp_hlp_saldo_akun_update` di MySQL SHALL menghasilkan nilai `Saldo_Akhir` yang **benar secara akuntansi** (menggunakan `CASE WHEN AKUN_DK`) — **bukan** identik dengan `UpdateSaldoSemuaAkun` lama di VB.NET yang memiliki bug AKUN_DK. Verifikasi dilakukan dengan test case dari COA nyata (lihat Requirement 18 AC #7).
6. WHEN migrasi satu modul selesai dan diverifikasi, THE Tim_Developer SHALL mendokumentasikan modul tersebut sebagai "migrated" di changelog migrasi.

---

### Requirement 16: Validasi Tanggal Backdate Berbasis Hak Akses

**User Story:** Sebagai pemilik toko, saya ingin transaksi dengan tanggal lampau (backdate) hanya
bisa dilakukan oleh user yang memiliki hak akses, agar tidak ada manipulasi tanggal transaksi
yang tidak sah.

#### Acceptance Criteria

1. SEMUA SP helper yang menerima parameter backdate SHALL memvalidasi tanggal — nilai `p_izinkan_backdate` dikirim oleh klien berdasarkan hak akses user.
2. WHEN sebuah operasi menerima `p_tgl_transaksi < CURDATE()` DAN `p_izinkan_backdate = 0`, THE klien SHALL menolak transaksi dan menampilkan pesan yang menyebutkan tanggal yang dipilih dan tanggal hari ini.
3. WHEN sebuah operasi menerima `p_tgl_transaksi < CURDATE()` DAN `p_izinkan_backdate = 1`, THE klien SHALL melanjutkan proses transaksi dengan `p_tgl_transaksi` yang dipilih.
4. WHEN `p_tgl_transaksi = CURDATE()` atau `p_tgl_transaksi > CURDATE()`, THE klien SHALL melanjutkan proses tanpa memeriksa flag `p_izinkan_backdate` — tanggal hari ini dan tanggal depan selalu diizinkan.
5. THE `sp_hlp_faktur_generate` SHALL menggunakan `p_tanggal` yang dikirim (bukan `CURDATE()`) untuk menghitung urutan nomor faktur — sehingga nomor faktur backdate mengikuti urutan tanggal tersebut, bukan urutan hari ini.
6. WHEN `sp_hlp_faktur_generate` dipanggil dengan `p_tanggal` lampau, THE `sp_hlp_faktur_generate` SHALL menghitung urutan `XXXX` berdasarkan transaksi yang sudah ada pada tanggal tersebut — bukan urutan global.
7. THE Klien SHALL membaca hak akses backdate dari cache `ModulHakAkses` (VB.NET) atau dari response login (Flutter/PHP) sebelum memanggil SP — klien tidak boleh menentukan nilai `p_izinkan_backdate` secara hardcode.
8. IF `p_tgl_transaksi` adalah NULL atau bukan tanggal valid, THEN THE SP SHALL mengembalikan `p_error_code = 'TANGGAL_TIDAK_VALID'` dan melakukan ROLLBACK.


---

## Appendix A: Daftar Form Terpengaruh

Dokumen ini mendaftar semua form VB.NET yang harus diubah saat migrasi ke Stored Procedures.
Setiap form dikelompokkan berdasarkan jenis pengaruhnya.

---

### Legenda Pengaruh

| Kode | Pengaruh |
|------|----------|
| **GN** | Generate Nomor Faktur **Transaksi** — logika `GenerateNomor*` / `Nomorjual` / `NomorBeli` dipindah ke `sp_hlp_faktur_generate`. Format: `PREFIX-YYMMDDXXXX` (ada `-` setelah prefix, tidak ada `-` antara tanggal dan urut), contoh: `PJ-2604010454`, `TC-2604150001`. Reset urut setiap hari, rawan race condition multi-user. **Berbeda dengan auto kode master** (`ARM-0001`, `PEL-0001`, dll) yang tidak punya komponen tanggal, tidak rawan race condition, dan tidak perlu dimigrasikan ke SP. Tanda ⚠️ = ada generate nomor tapi format non-standar (timestamp) — perlu distandarisasi. |
| **VS** | Validasi Stok — cek `SettingIzinkanJualStokMinus` / `CekStok()` dipindah ke `sp_hlp_stok_validasi` |
| **HS** | Hitung Stok — panggilan `HitungStokPerubahan()` diganti dengan `sp_hlp_stok_hitung` |
| **IJ** | Insert Jurnal — INSERT INTO JurnalUmum dilakukan inline di klien (tidak ada SP khusus)
| **US** | Update Saldo — panggilan `UpdateSaldoAkun()` / `UpdateSaldoSemuaAkun()` diganti dengan `sp_hlp_saldo_akun_update` |
| **VB** | Validasi Backdate — `SettingIzinkanTanggalLampau` dikirim sebagai `p_izinkan_backdate` ke SP helper |
| **VK** | Validasi Kas — cek saldo kas sebelum bayar dipindah ke `sp_hlp_saldo_kas_validasi` |

---

### Folder 2Trans — Form Transaksi Utama

| Form | GN | VS | HS | IJ | US | VB | VK | SP Helper |
|------|:--:|:--:|:--:|:--:|:--:|:--:|:--:|-----------|
| `FormPenjualan.vb` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | — | `sp_hlp_faktur_generate`, `sp_hlp_stok_validasi` (PHP), `sp_hlp_stok_hitung` (PHP) |
| `FormPembelian.vb` | ✅ | — | ✅ | ✅ | ✅ | ✅ | — | `sp_hlp_faktur_generate` |
| `FormReturBeli.vb` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | — | `sp_hlp_faktur_generate` |
| `FormReturPembelian.vb` | ✅ | — | ✅ | ✅ | ✅ | — | — | `sp_hlp_faktur_generate` |
| `FormReturPenjualan.vb` | ✅ | — | ✅ | ✅ | ✅ | ✅ | — | `sp_hlp_faktur_generate` |
| `FormStokOpname.vb` | ✅ | — | ✅ | ✅ | ✅ | ✅ | — | `sp_hlp_faktur_generate` |
| `FormStokOpnameBahan.vb` | ✅ | — | ✅ | ✅ | ✅ | — | — | `sp_hlp_faktur_generate` |
| `FormTransferBarang.vb` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | — | `sp_hlp_faktur_generate` |
| `FormTransferStok.vb` | ✅ | — | ✅ | ✅ | ✅ | ✅ | — | `sp_hlp_faktur_generate` |
| `FormTransferCabang.vb` | ✅ | — | ✅ | — | ✅ | — | — | `sp_hlp_faktur_generate` |
| `FormBayarHutang.vb` | ✅ | — | — | ✅ | ✅ | ✅ | ✅ | `sp_hlp_faktur_generate`, `sp_hlp_saldo_kas_validasi` |
| `FormBayarPiutang.vb` | ✅ | — | — | ✅ | ✅ | ✅ | — | `sp_hlp_faktur_generate` |
| `FormSuratJalan.vb` | ✅ | — | — | — | — | — | — | `sp_hlp_faktur_generate` |
| `FormPenjualanDitahan.vb` | — | — | — | — | — | — | — | Tidak ada logika bisnis kritis — draft saja |
| `FormPembelianDitahan.vb` | — | — | — | — | — | — | — | Tidak ada logika bisnis kritis — draft saja |
| `FormEditBayarJual.vb` | — | — | — | ✅ | ✅ | — | — | Edit pembayaran — perlu review |

**Catatan `FormReturBeli` vs `FormReturPembelian`:**
Dua form ini menangani retur pembelian dengan alur berbeda. Keduanya melakukan INSERT inline namun dengan parameter yang sedikit berbeda. Perlu diverifikasi saat implementasi.

---

### Folder 0Form — Form Utama & Loading

| Form | GN | VS | HS | IJ | US | VB | VK | Keterangan |
|------|:--:|:--:|:--:|:--:|:--:|:--:|:--:|------------|
| `FormUtama.vb` | — | — | ✅ | ✅ | ✅ | — | — | Hapus transaksi (klik kanan DGV) — memanggil `HitungStokPerubahan` dan `UpdateSaldoSemuaAkun` untuk semua jenis hapus |
| `FormLoading.vb` | — | — | ✅ | — | ✅ | — | — | Recalculate batch saat startup — akan diganti dengan `sp_bat_stok_semua_barang` dan `sp_bat_saldo_semua_akun` |

---

### Folder 1Master — Form Master

> **Catatan GN:** Form master punya auto kode (`ARM-0001`, `PEL-0001`, `SPL-0001`, dll) tapi **bukan** nomor faktur transaksi. Auto kode master tidak punya komponen tanggal, tidak reset harian, dan tidak rawan race condition — sehingga **tidak perlu dimigrasikan ke SP**. Kolom GN kosong di semua form master adalah benar.

| Form | GN | VS | HS | IJ | US | VB | VK | Keterangan |
|------|:--:|:--:|:--:|:--:|:--:|:--:|:--:|------------|
| `FormBarang.vb` | ⚠️ | — | ✅ | ✅ | ✅ | — | — | Tambah/kurang stok manual — generate `noTransaksi` pakai timestamp. **Keputusan: LEWATI SP** — cukup ganti `HitungStokPerubahan` → `sp_hlp_stok_hitung` dan `UpdateSaldoAkun` → `sp_hlp_saldo_akun_update` |
| `TambahBarang.vb` | — | — | — | ✅ | ✅ | — | — | Saat tambah/edit barang dengan nilai HPP — **Keputusan: LEWATI SP** — cukup ganti inline INSERT → `INSERT INTO JurnalUmum` (inline) dan `UpdateSaldoAkun` → `sp_hlp_saldo_akun_update` |
| `FormArmada.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** — CRUD sederhana tanpa logika bisnis kritis |
| `FormCabang.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** |
| `FormKaryawan.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** |
| `TambahPelanggan.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** |
| `TambahSupliyer.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** |
| `TambahKategori.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** |
| `TambahSatuan.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** |
| `TambahMerk.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** |
| `FormUser.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** |
| `FormHakUser.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** |
| `FormGeneralSetting.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** — sumber setting `SettingIzinkanTanggalLampau` dll |

---

### Folder 3Jurnal — Jurnal Manual

| Form | GN | VS | HS | IJ | US | VB | VK | Keterangan |
|------|:--:|:--:|:--:|:--:|:--:|:--:|:--:|------------|
| `FormKeuangan.vb` | — | — | — | ✅ | ✅ | — | — | Jurnal manual — INSERT inline + `sp_hlp_saldo_akun_update` per akun |

---

### Folder 4Gaji — Penggajian

| Form | GN | VS | HS | IJ | US | VB | VK | Keterangan |
|------|:--:|:--:|:--:|:--:|:--:|:--:|:--:|------------|
| `FormGaji.vb` | ✅ | — | — | ✅ | ✅ | — | — | Generate nomor gaji (`GJ-YYMMDD-XXXX`), INSERT jurnal gaji ke `JurnalUmum`, `UpdateSaldoAkun` per akun gaji |
| `FormBon.vb` | ✅ | — | — | ✅ | ✅ | — | — | Generate nomor bon (`BK-YYMMDD-XXXX`), INSERT jurnal bon ke `JurnalUmum`, `UpdateSaldoAkun` |
| `FormMasterGaji.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** — master data gaji saja |
| `FormLapBon.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** — laporan saja |
| `FormLapBonPerorang.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** — laporan saja |
| `FormLaporanGaji.vb` | — | — | — | — | — | — | — | — | **Tidak terpengaruh** — laporan saja |

---

### Folder 5Lap — Laporan

Semua form di folder `5Lap` **tidak terpengaruh** — hanya membaca data (SELECT), tidak ada operasi tulis.

| Form | Keterangan |
|------|------------|
| `FormStokBarang.vb` | Baca stok — tidak terpengaruh |
| `FormKartuStok.vb` | Baca history — tidak terpengaruh |
| `FormLapPenjualanBaru.vb` | Baca penjualan — tidak terpengaruh |
| `FormLapPembelian.vb` | Baca pembelian — tidak terpengaruh |
| `FormLapJurnal.vb` | Baca jurnal — tidak terpengaruh |
| `FormLapNeracaLR.vb` | ⚠️ | — | — | — | — | — | — | — | **Terpengaruh (Req 17 & 18)** — `HITUNGSALDOAWAL/AKHIR` harus dipindah ke `temp_datareferensi`. Bug Step 3 `HITUNGSEMUASALDO` harus diperbaiki. Tetap di VB, tidak dimigrasikan ke SP. |
| `FormLapHutang.vb` | Baca hutang — tidak terpengaruh |
| `FormLapPiutang.vb` | Baca piutang — tidak terpengaruh |
| *(semua form 5Lap lainnya)* | Baca saja — tidak terpengaruh |

---

### Folder 9Sync — Sinkronisasi Cloud

| Form/Module | GN | VS | HS | IJ | US | VB | VK | Keterangan |
|-------------|:--:|:--:|:--:|:--:|:--:|:--:|:--:|------------|
| `SyncManager.vb` | — | — | ✅ | — | — | — | — | Memanggil `HitungStokPerubahan` saat sync download barang dari cloud. Perlu diganti dengan `sp_hlp_stok_hitung` |

---

### Folder 8Uty — Utilitas

| Form | GN | VS | HS | IJ | US | VB | VK | Keterangan |
|------|:--:|:--:|:--:|:--:|:--:|:--:|:--:|------------|
| `FormPerbaikanDatabase.vb` | — | — | — | — | — | — | — | **Tidak terpengaruh** — utilitas trim data saja |
| `FormHistory.vb` | — | — | — | — | — | — | — | **Tidak terpengaruh** — baca history saja |

---

### Modules — Modul Global

| Module | Pengaruh | Keterangan |
|--------|----------|------------|
| `ModuleVariabel.vb` | **SEBAGIAN dimigrasikan** | `HitungStokPerubahan`, `HitungStokToko`, `HitungStokGudang`, `HitungSemuaKode` → menjadi wrapper ke SP. `UpdateSaldoAkun`, `UpdateSaldoSemuaAkun` → **tetap di VB** tapi diperbaiki bug AKUN_DK-nya. `HITUNGSEMUASALDO` → **tetap di VB**, tidak dimigrasikan ke SP karena kompleksitas tinggi dan hanya dipanggil dari satu titik (FormLoading) |
| `ModulHakAkses.vb` | **Tidak terpengaruh** — tetap di client | Sumber nilai `SettingIzinkanTanggalLampau`, `SettingIzinkanJualStokMinus`, dll yang dikirim sebagai parameter ke SP |
| `ModuleAngka.vb` | **Tidak terpengaruh** | Validasi presentasi — tetap di client sesuai standar |
| `ModuleAuditTrail.vb` | **Tidak terpengaruh** | Audit trail tetap di client |

---

### Ringkasan Jumlah Form Terpengaruh

| Kategori | Jumlah Form | Keterangan |
|----------|:-----------:|------------|
| Form transaksi utama (generate nomor + DTP) | **12** | `2Trans/` — penjualan, pembelian, retur, opname, transfer, bayar |
| Form dengan hapus transaksi | **1** | `FormUtama.vb` — hapus via klik kanan |
| Form master dengan logika stok/jurnal | **2** | `FormBarang.vb`, `TambahBarang.vb` |
| Form gaji/bon | **2** | `FormGaji.vb`, `FormBon.vb` |
| Form jurnal manual (pakai SP helper) | **1** | `FormKeuangan.vb` — INSERT inline + `sp_hlp_saldo_akun_update` |
| Form loading/batch | **1** | `FormLoading.vb` — recalculate batch |
| Module global (dimigrasikan) | **1** | `ModuleVariabel.vb` |
| Sync module | **1** | `SyncManager.vb` |
| **Total form yang perlu diubah** | **21** | |
| Form tidak terpengaruh | **~40+** | Semua form laporan, master sederhana, utilitas |

---

### Urutan Migrasi yang Direkomendasikan

Berdasarkan dependensi dan risiko:

1. **Fase 1 — Helper SP & Batch SP** (tidak ada perubahan client dulu)
   - Buat dan test: `sp_hlp_faktur_generate`, `sp_hlp_stok_validasi`, `sp_hlp_stok_hitung`, `INSERT INTO JurnalUmum` (inline), `sp_hlp_saldo_akun_update`, `sp_hlp_saldo_kas_validasi`
   - Buat dan test batch SP: `sp_bat_stok_semua_barang`, `sp_bat_saldo_semua_akun`, `sp_bat_piutang_semua_pelanggan`, `sp_bat_hutang_semua_supplier`, `sp_bat_bon_semua_karyawan`

2. **Fase 2 — Penjualan** (volume tertinggi)
   - Generate nomor via `sp_hlp_faktur_generate` → update `FormPenjualan.vb`
   - Refactor `sync_penjualan.php` → INSERT inline + validasi via SP helper

3. **Fase 3 — Pembelian**
   - Generate nomor via `sp_hlp_faktur_generate` → update `FormPembelian.vb`

4. **Fase 4 — Retur & Opname**
   - Update `FormReturPenjualan.vb`, `FormReturBeli.vb`, `FormReturPembelian.vb`, `FormStokOpname.vb`
   - Refactor `sync_stokopname.php` → INSERT inline + validasi via SP helper

5. **Fase 5 — Transfer & Bayar**
   - Update semua form transfer dan bayar

6. **Fase 6 — Hapus Transaksi**
   - Verifikasi logika hapus di `FormUtama.vb` tetap benar

7. **Fase 7 — Gaji & Bon**
   - Update `FormGaji.vb`, `FormBon.vb`

8. **Fase 8 — Batch, Sync & Bug Fix**
   - Update `FormLoading.vb` → `sp_bat_stok_semua_barang`, `sp_bat_saldo_semua_akun`
   - Update `SyncManager.vb` → `sp_hlp_stok_hitung`
   - Perbaiki bug `UpdateSaldoAkun` / `UpdateSaldoSemuaAkun` di `ModuleVariabel.vb`
   - Perbaiki bug `HITUNGSEMUASALDO` Step 3 di `FormLapNeracaLR.vb`

9. **Fase 9 — Master dengan Jurnal**
   - Update `FormBarang.vb`, `TambahBarang.vb`
   - `FormKeuangan.vb` → INSERT inline + `sp_hlp_saldo_akun_update`

---

### Requirement 16b: Helper DTP Backdate Terpusat di `ModulHakAkses`

**User Story:** Sebagai developer, saya ingin semua form menggunakan helper terpusat untuk
mengatur DateTimePicker berdasarkan hak akses backdate, agar tidak ada duplikasi logika
dan form cukup memanggil satu fungsi tanpa perlu cek setting sendiri.

#### Latar Belakang

Sebelumnya setiap form harus:
1. Deklarasi `Private SettingIzinkanTanggalLampau As String = "Tidak"`
2. Isi dari cache: `SettingIzinkanTanggalLampau = ModulHakAkses.BacaSettingDariCache(...)`
3. Cek sendiri: `If SettingIzinkanTanggalLampau = "Tidak" Then DTPTgl.Enabled = False`

Ini duplikasi di setiap form dan rawan lupa. Solusi: helper terpusat di `ModulHakAkses.vb`.

#### Acceptance Criteria

1. THE `ModulHakAkses` SHALL menyediakan fungsi `TerapkanModeDTP(dtp, isEditMode, [tanggalEdit])` yang membaca setting backdate langsung dari cache internal — form tidak perlu kirim parameter setting.
2. THE `ModulHakAkses` SHALL menyediakan fungsi `ResetDTPKeTanggalHariIni(dtp)` yang selalu set `dtp.Value = DateTime.Now` dan mengunci DTP jika backdate tidak diizinkan.
3. THE `ModulHakAkses` SHALL menyediakan fungsi `ValidasiTanggalTransaksi(tglDipilih)` yang return `True` jika tanggal valid, `False` jika backdate tidak diizinkan.
4. WHEN `isEditMode = False` (mode tambah), THE `TerapkanModeDTP` SHALL selalu set `dtp.Value = DateTime.Now` — memastikan DTP tidak menampilkan tanggal lama dari sesi sebelumnya.
5. WHEN `isEditMode = True` (mode edit), THE `TerapkanModeDTP` SHALL set `dtp.Value = tanggalEdit` — menampilkan tanggal transaksi yang sedang diedit.
6. WHEN backdate tidak diizinkan, THE `TerapkanModeDTP` dan `ResetDTPKeTanggalHariIni` SHALL set `dtp.Enabled = False` — DTP dikunci secara visual sehingga user tidak bisa mengubah tanggal.
7. WHEN backdate diizinkan, THE `TerapkanModeDTP` dan `ResetDTPKeTanggalHariIni` SHALL set `dtp.Enabled = True` — DTP bisa diubah user.
8. THE Form SHALL memanggil `ResetDTPKeTanggalHariIni(DTPTgl)` di Form_Load (mode tambah) dan setiap kali form di-reset ke mode tambah — tanpa perlu cek setting sendiri.
9. THE Form SHALL memanggil `TerapkanModeDTP(DTPTgl, isEditMode:=True, tanggalEdit:=tglDariDB)` saat memuat data untuk mode edit.
10. THE Form SHALL memanggil `ValidasiTanggalTransaksi(DTPTgl.Value)` sebagai lapisan kedua sebelum simpan — sebagai safety net jika DTP berhasil diubah melalui cara lain.

#### Contoh Penggunaan di Form

```vb
' Form_Load (mode tambah)
ResetDTPKeTanggalHariIni(DTPTgl)

' Saat load data edit
TerapkanModeDTP(DTPTgl, isEditMode:=True, tanggalEdit:=tglDariDB)

' Validasi sebelum simpan
If Not ModulHakAkses.ValidasiTanggalTransaksi(DTPTgl.Value) Then
    ResetDTPKeTanggalHariIni(DTPTgl)
    Exit Sub
End If

' Kirim ke SP — setting dibaca otomatis oleh SP dari parameter p_izinkan_backdate
Dim izinkanBackdate As Integer = If(ModulHakAkses.BacaSettingDariCache(
    FormGeneralSetting.LblGlobalTransaksiLampau.Text) = "Iya", 1, 0)
```

---

### Requirement 17: Pemisahan Kalkulasi Laporan dari State Transaksi (tbl_datareferensi) ✅ SELESAI

**User Story:** Sebagai developer, saya ingin laporan neraca per periode tidak merusak nilai
`Saldo_Akhir` di `tbl_datareferensi`, agar validasi saldo kas di transaksi selalu menggunakan
nilai yang benar dan tidak terpengaruh oleh filter tanggal laporan.

**Status:** ✅ SELESAI — `FormLapNeracaLR.vb` sudah menggunakan `temp_datareferensi` untuk kalkulasi laporan per periode, dan `tbl_datareferensi` tetap digunakan untuk state transaksi realtime dan posting resmi.

#### Dua Fungsi yang Berbeda di FormLapNeracaLR

| Fungsi | Dipanggil dari | Tujuan | Tulis ke mana? |
|--------|---------------|--------|----------------|
| `HITUNGSEMUASALDO()` | `MulaiPosting()` di `FormLoading` | Recalculate penuh semua akun + laba/rugi dari seluruh `JurnalUmum` tanpa filter tanggal. **Sumber kebenaran** untuk `Saldo_Akhir` realtime dan nilai akun LABA RUGI | ✅ `tbl_datareferensi` — posting resmi |
| `HITUNGSALDOAWAL()` + `HITUNGDEBETKREDIT()` + `HITUNGSALDOAKHIR()` | `BtnTampilNeraca_Click` dengan filter tanggal | Kalkulasi sementara untuk laporan per periode | ✅ `temp_datareferensi` — sudah diimplementasikan |

---

### Requirement 18: Perbaikan Bug Kalkulasi Saldo Akun ✅ SELESAI

**User Story:** Sebagai akuntan, saya ingin saldo akun dihitung dengan rumus yang benar sesuai
kaidah akuntansi double-entry, agar laporan neraca, laba rugi, dan validasi saldo kas
menghasilkan angka yang akurat.

#### Keputusan Arsitektur

Perbaikan bug ini dilakukan **di VB.NET** (bukan dimigrasikan ke MySQL SP), kecuali
`sp_hlp_saldo_akun_update` yang memang sudah direncanakan sebagai SP baru untuk dipanggil
dari semua client. Alasan:

- `HITUNGSEMUASALDO` dan `UpdateSaldoSemuaAkun` hanya dipanggil dari VB desktop — tidak ada
  manfaat memindahkan ke SP dengan kompleksitas yang ada
- Bug bisa diperbaiki langsung di VB dengan perubahan minimal (1-2 baris SQL)
- `sp_hlp_saldo_akun_update` tetap dibuat sebagai SP baru yang sudah benar dari awal —
  menggantikan `UpdateSaldoAkun` untuk panggilan dari Flutter dan PHP

#### Bug 1 — HITUNGSEMUASALDO Step 3: Membaca SALDO_SEBELUMNYA Bukan SALDO_AKHIR

**Lokasi:** `5Lap/FormLapNeracaLR.vb` — fungsi `HITUNGSEMUASALDO()`, Step 3.

**Kode yang salah:**
```sql
SUM(CASE WHEN SUB_AKUN='LABA' THEN SALDO_SEBELUMNYA ELSE 0 END) -
SUM(CASE WHEN SUB_AKUN='RUGI' THEN SALDO_SEBELUMNYA ELSE 0 END) AS LABA_RUGI
```

**Mengapa salah:**
- Step 0 me-reset `SALDO_SEBELUMNYA = SALDO_AWAL` untuk semua akun
- Step 2 mengupdate `SALDO_AKHIR` (bukan `SALDO_SEBELUMNYA`) untuk akun pendapatan (SUB_AKUN='LABA') dan biaya (SUB_AKUN='RUGI')
- Step 3 membaca `SALDO_SEBELUMNYA` dari akun LABA/RUGI → yang terbaca adalah `SALDO_AWAL`, bukan hasil kalkulasi transaksi
- Akibatnya: nilai laba/rugi yang ditulis ke akun LABA RUGI adalah selisih saldo awal, bukan laba/rugi periode berjalan

**Kode yang benar:**
```sql
SUM(CASE WHEN SUB_AKUN='LABA' THEN SALDO_AKHIR ELSE 0 END) -
SUM(CASE WHEN SUB_AKUN='RUGI' THEN SALDO_AKHIR ELSE 0 END) AS LABA_RUGI
```

**Bukti:** Fungsi `HITUNGSALDOAKHIR()` di file yang sama sudah benar — membaca `SALDO_AKHIR`:
```sql
SELECT SUM(SALDO_AKHIR) FROM tbl_datareferensi WHERE SUB_AKUN = 'LABA'
SELECT SUM(SALDO_AKHIR) FROM tbl_datareferensi WHERE SUB_AKUN = 'RUGI'
```

#### Bug 2 — UpdateSaldoSemuaAkun: Tidak Menghormati AKUN_DK

**Lokasi:** `Modules/ModuleVariabel.vb` — fungsi `UpdateSaldoAkun()` dan `UpdateSaldoSemuaAkun()`.

**Kode yang salah:**
```sql
SET r.Saldo_Akhir = IFNULL(r.Saldo_Awal, 0) + IFNULL(d.total_debet, 0) - IFNULL(k.total_kredit, 0)
```

**Mengapa salah:**
Rumus ini selalu `+DEBET -KREDIT` untuk semua akun. Dari COA nyata (`tbl_datareferensi_backup.sql`),
**22 dari 52 akun** memiliki `AKUN_DK = 'KREDIT'` dan akan dihitung terbalik:

| Akun | AKUN_DK | Rumus Benar | Rumus Saat Ini | Status |
|------|---------|-------------|----------------|--------|
| HUTANG BELANJA (03.01.001) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| HUTANG PAJAK (03.02.001) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| HUTANG BANK JP (03.02.002) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| PPN KELUARAN (03.02.004) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| MODAL (04.01.001) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| LABA RUGI BERJALAN (05.01.001) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| PENJUALAN (05.02.001) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| POTONGAN DISKON PEMBELIAN (06.05.001) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| PENDAPATAN BUNGA BANK (08.01.001) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| PENDAPATAN LAIN LAIN (08.01.002) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| AKUM. PENY. GEDUNG (02.02.002) | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |
| *(dan 11 akun KREDIT lainnya)* | KREDIT | Awal - D + K | Awal + D - K | ❌ Terbalik |

**Dampak bisnis:**
- Neraca tidak seimbang (Aset ≠ Pasiva + Modal)
- Saldo hutang supplier yang dipakai `sp_hlp_saldo_kas_validasi` salah
- Laporan laba rugi salah karena saldo pendapatan terbalik
- Hanya akun DEBET (kas, piutang, persediaan, biaya) yang benar

**Kode yang benar:**
```sql
SET r.Saldo_Akhir = CASE
  WHEN r.AKUN_DK = 'DEBET'  THEN IFNULL(r.Saldo_Awal, 0) + IFNULL(d.total_debet, 0) - IFNULL(k.total_kredit, 0)
  WHEN r.AKUN_DK = 'KREDIT' THEN IFNULL(r.Saldo_Awal, 0) - IFNULL(d.total_debet, 0) + IFNULL(k.total_kredit, 0)
  ELSE 0 END
```

Perbaikan yang sama berlaku untuk `UpdateSaldoAkun()` (versi per-akun) dan `sp_hlp_saldo_akun_update` yang akan dibuat.

#### Catatan: HITUNGSEMUASALDO Step 2 Sudah Benar

Step 2 di `HITUNGSEMUASALDO` sudah menggunakan `CASE WHEN AKUN_DK` dengan benar:
```sql
SET SALDO_AKHIR = CASE
  WHEN AKUN_DK = 'DEBET'  THEN SALDO_SEBELUMNYA + S_DEBET - S_KREDIT
  WHEN AKUN_DK = 'KREDIT' THEN SALDO_SEBELUMNYA - S_DEBET + S_KREDIT
  ELSE 0 END
WHERE TYPE_AKUN <> 'LABA RUGI'
```
Ini sudah benar untuk semua 52 akun di COA. Bug hanya ada di Step 3 (baca kolom yang salah).

#### Acceptance Criteria

1. THE `HITUNGSEMUASALDO()` Step 3 SHALL membaca `SALDO_AKHIR` (bukan `SALDO_SEBELUMNYA`) dari akun dengan `SUB_AKUN IN ('LABA','RUGI')` untuk menghitung nilai laba/rugi periode berjalan.

2. WHEN `HITUNGSEMUASALDO()` dijalankan, THE nilai `SALDO_AKHIR` pada akun `TYPE_AKUN = 'LABA RUGI'` SHALL sama dengan: `SUM(SALDO_AKHIR akun SUB_AKUN='LABA') - SUM(SALDO_AKHIR akun SUB_AKUN='RUGI')`.

3. THE `UpdateSaldoSemuaAkun()` SHALL menggunakan `CASE WHEN AKUN_DK` untuk menentukan arah kalkulasi:
   - Akun `AKUN_DK = 'DEBET'`: `Saldo_Akhir = Saldo_Awal + total_debet - total_kredit`
   - Akun `AKUN_DK = 'KREDIT'`: `Saldo_Akhir = Saldo_Awal - total_debet + total_kredit`

4. THE `UpdateSaldoAkun()` (per-akun) SHALL menggunakan rumus yang sama dengan `UpdateSaldoSemuaAkun()` — menghormati `AKUN_DK`.

5. THE `sp_hlp_saldo_akun_update` (SP yang akan dibuat) SHALL mengimplementasikan rumus yang sama dengan `CASE WHEN AKUN_DK` — konsisten dengan perbaikan di VB.NET.

6. WHEN perbaikan Bug 2 diimplementasikan, THE `sp_hlp_saldo_kas_validasi` SHALL tetap membaca `Saldo_Akhir` dari `tbl_datareferensi` — nilai yang dibaca sekarang sudah benar untuk semua jenis akun termasuk kas (DEBET) dan hutang (KREDIT).

7. THE perbaikan Bug 1 dan Bug 2 SHALL diverifikasi dengan test case berdasarkan COA nyata:
   - **Kas (DEBET):** penjualan tunai 1.000.000 → `Saldo_Akhir KAS DI TOKO` bertambah 1.000.000 ✓
   - **Hutang (KREDIT):** pembelian kredit 500.000 → `Saldo_Akhir HUTANG BELANJA` bertambah 500.000 ✓
   - **Pendapatan (KREDIT/LABA):** penjualan 1.000.000 → `Saldo_Akhir PENJUALAN` bertambah 1.000.000 ✓
   - **Akun LABA RUGI:** pendapatan 1.000.000 - biaya 600.000 → `Saldo_Akhir LABA RUGI BERJALAN` = 400.000 ✓
   - **Akumulasi Penyusutan (KREDIT):** penyusutan 50.000 → `Saldo_Akhir AKUM. PENY. GEDUNG` bertambah 50.000 ✓

---

### Requirement 19: Update Saldo Hutang Supplier dan Piutang Pelanggan ✅ SELESAI

**User Story:** Sebagai pemilik toko, saya ingin saldo hutang ke supplier dan piutang dari
pelanggan selalu akurat setelah setiap transaksi, agar laporan hutang/piutang dan notifikasi
jatuh tempo menampilkan angka yang benar dari semua client.

#### Latar Belakang

Ada **dua lapisan** saldo yang berbeda dan keduanya harus diupdate:

| Lapisan | Tabel | Kolom | Sumber Data | Dipakai untuk |
|---------|-------|-------|-------------|---------------|
| **Saldo tagihan per pelanggan** | `tbl_pelanggan` | `HutangAkhir` | `SUM(SISA_TAGIHAN)` dari `penjualan` | Laporan piutang, notifikasi jatuh tempo, ranking piutang |
| **Saldo tagihan per supplier** | `tbl_supliyer` | `HutangAkhir` | `SUM(TAGIHAN)` dari `pembelian` | Laporan hutang, notifikasi jatuh tempo, ranking hutang |
| **Saldo akun jurnal piutang** | `tbl_datareferensi` | `Saldo_Akhir` | `JurnalUmum` | Neraca (akun 01.03.001 PIUTANG USAHA) |
| **Saldo akun jurnal hutang** | `tbl_datareferensi` | `Saldo_Akhir` | `JurnalUmum` | Neraca (akun 03.01.001 HUTANG BELANJA) |

Lapisan 3 dan 4 sudah ditangani oleh `sp_hlp_saldo_akun_update` (Requirement 7).
Requirement ini khusus untuk **Lapisan 1 dan 2** yang belum ada di requirements sebelumnya.

#### Rumus yang Ada di VB.NET (ModuleVariabel.vb)

**Piutang pelanggan (realtime per pelanggan):**
```sql
UPDATE tbl_pelanggan p
LEFT JOIN (SELECT ID_PELANGGAN, SUM(IFNULL(SISA_TAGIHAN, 0)) AS HUTANG
           FROM penjualan WHERE ID_PELANGGAN = @ID GROUP BY ID_PELANGGAN) x
ON x.ID_PELANGGAN = p.KODE
SET p.HutangAkhir = IFNULL(x.HUTANG, 0) + p.HutangAwal
WHERE p.KODE = @ID
```

**Hutang supplier (realtime per supplier):**
```sql
UPDATE tbl_supliyer s
LEFT JOIN (SELECT ID_SUPPLIER, SUM(IFNULL(TAGIHAN, 0)) AS HUTANG
           FROM pembelian WHERE ID_SUPPLIER = @ID GROUP BY ID_SUPPLIER) x
ON x.ID_SUPPLIER = s.KODE
SET s.HutangAkhir = IFNULL(x.HUTANG, 0) + s.HutangAwal
WHERE s.KODE = @ID
```

#### Acceptance Criteria

1. WHEN simpan penjualan, THE klien SHALL memanggil update `tbl_pelanggan.HutangAkhir` untuk pelanggan yang terlibat setelah INSERT ke tabel `penjualan` — menggunakan rumus `HutangAwal + SUM(SISA_TAGIHAN dari penjualan WHERE ID_PELANGGAN)`.

2. WHEN simpan bayar piutang, THE klien SHALL memanggil update `tbl_pelanggan.HutangAkhir` untuk pelanggan yang dibayar setelah UPDATE `SISA_TAGIHAN` di tabel `penjualan`.

3. WHEN simpan pembelian, THE klien SHALL memanggil update `tbl_supliyer.HutangAkhir` untuk supplier yang terlibat setelah INSERT ke tabel `pembelian` — menggunakan rumus `HutangAwal + SUM(TAGIHAN dari pembelian WHERE ID_SUPPLIER)`.

4. WHEN simpan bayar hutang, THE klien SHALL memanggil update `tbl_supliyer.HutangAkhir` untuk supplier yang dibayar setelah UPDATE `TAGIHAN` di tabel `pembelian`.

5. WHEN simpan retur jual, THE klien SHALL menerima pilihan user (`CbPotongHutang`) sebagai `p_potong_piutang`:
   - Jika `p_potong_piutang = 1`: klien SHALL mengurangi `SISA_TAGIHAN` di tabel `penjualan` sebesar nilai retur, mengupdate `STATUS_TRANSAKSI` (Lunas/Belum Lunas), lalu memanggil update `tbl_pelanggan.HutangAkhir`
   - Jika `p_potong_piutang = 0`: tidak mengubah `SISA_TAGIHAN` — pengembalian via kas/bank yang dicatat di jurnal

6. WHEN simpan retur beli (dari `FormReturPembelian`), THE klien SHALL menerima pilihan user (`CbPotongHutang`) sebagai `p_potong_hutang`:
   - Jika `p_potong_hutang = 1`: klien SHALL mengurangi `TAGIHAN` di tabel `pembelian` sebesar nilai retur, mengupdate `STATUS_TRANSAKSI_BELI`, lalu memanggil update `tbl_supliyer.HutangAkhir`
   - Jika `p_potong_hutang = 0`: tidak mengubah `TAGIHAN` — pengembalian via kas/bank yang dicatat di jurnal

7. THE `FormReturBeli` (form retur beli alternatif) **tidak memiliki** fitur potong hutang — SP-nya tidak perlu parameter `p_potong_hutang`.

8. THE update `HutangAkhir` SHALL dieksekusi di dalam transaksi yang sama dengan operasi utama — tidak boleh ada kondisi di mana transaksi berhasil tapi `HutangAkhir` belum diupdate.

9. THE `sp_bat_piutang_semua_pelanggan` (batch) SHALL merecalculate `HutangAkhir` semua pelanggan sekaligus dari `penjualan` — menggantikan `UpdatePiutangDibayar()` yang dipanggil dari `FormLoading.MulaiLoading()` dan `MulaiPosting()`.

10. THE `sp_bat_hutang_semua_supplier` (batch) SHALL merecalculate `HutangAkhir` semua supplier sekaligus dari `pembelian` — menggantikan `UpdateSupliyerFromPembelianHutangDibayar()` yang dipanggil dari `FormLoading`.

11. WHEN `sp_bat_piutang_semua_pelanggan` atau `sp_bat_hutang_semua_supplier` dipanggil, THE SP SHALL idempoten — memanggil dua kali menghasilkan nilai yang sama.

#### Tambahan ke Daftar SP

SP baru yang perlu ditambahkan ke Konvensi Nama:

| Nama SP | Keterangan |
|---------|------------|
| `sp_bat_piutang_semua_pelanggan` | Recalculate `HutangAkhir` semua pelanggan dari `penjualan` |
| `sp_bat_hutang_semua_supplier` | Recalculate `HutangAkhir` semua supplier dari `pembelian` |
| `sp_bat_bon_semua_karyawan` | Recalculate `SaldoAkhir` semua karyawan dari `Bon_karyawan` |

#### Saldo Bon Karyawan

Selain hutang/piutang, ada saldo ketiga yang juga perlu diupdate: **saldo bon karyawan** di `tbl_karyawan`.

**Rumus:**
```sql
-- Realtime per karyawan (UpdateBonKaryawan di ModuleVariabel.vb)
UPDATE tbl_karyawan
SET TotalBon   = SUM(NOMINAL FROM Bon_karyawan WHERE JENIS='BON'  AND Kode=@Kode),
    TotalBayar = SUM(NOMINAL FROM Bon_karyawan WHERE JENIS='BAYAR' AND Kode=@Kode),
    SaldoAkhir = SaldoAwal + TotalBon - TotalBayar
WHERE Kode = @Kode
```

**Acceptance Criteria tambahan:**

12. WHEN simpan bon, THE klien SHALL memanggil update `tbl_karyawan.SaldoAkhir` untuk karyawan yang terlibat setelah INSERT ke `Bon_karyawan` — menggunakan rumus `SaldoAwal + TotalBon - TotalBayar`.

13. WHEN simpan gaji, THE klien SHALL memanggil update `tbl_karyawan.SaldoAkhir` untuk karyawan yang digaji, karena gaji dapat memotong saldo bon (potongan bon dari gaji).

14. THE `sp_bat_bon_semua_karyawan` (batch) SHALL merecalculate `TotalBon`, `TotalBayar`, dan `SaldoAkhir` semua karyawan sekaligus dari `Bon_karyawan` — menggantikan `UpdateTotalBonDanTotalBayarKaryawan()` yang dipanggil dari `FormLoading`.

15. THE update `SaldoAkhir` karyawan SHALL dieksekusi di dalam transaksi yang sama dengan INSERT ke `Bon_karyawan` — tidak boleh ada kondisi di mana bon tersimpan tapi `SaldoAkhir` belum diupdate.

#### Urutan Implementasi

- Update `HutangAkhir` per pelanggan/supplier masuk ke dalam SP transaksi masing-masing (Fase 2-5)
- `sp_bat_piutang_semua_pelanggan` dan `sp_bat_hutang_semua_supplier` dibuat di Fase 1 bersama helper SP lainnya
- `FormLoading.MulaiLoading()` dan `MulaiPosting()` diupdate di Fase 8 untuk memanggil SP batch ini

---

### Requirement 20: Refactor PHP API — Hapus Logika Bisnis Inline

**User Story:** Sebagai developer, saya ingin PHP API tidak lagi mengimplementasikan logika bisnis
kritis secara inline, agar tidak ada duplikasi kode antara VB.NET dan PHP yang rawan tidak sinkron.

#### Latar Belakang

File PHP di `AppAndroid/api/` saat ini mengimplementasikan logika bisnis secara inline — duplikasi
dari VB.NET. Ini menyebabkan dua masalah utama:

1. **Bug yang sama terulang** — `UpdateSaldoSemuaAkun` di PHP tidak pakai `CASE WHEN AKUN_DK`,
   sama persis dengan bug di VB.NET (Requirement 18 Bug 2)
2. **Gap keamanan** — PHP tidak validasi stok, tidak validasi backdate, tidak cek duplikat faktur

#### Perbandingan VB vs PHP Saat Ini

| Operasi | VB.NET | PHP (`sync_penjualan.php`) | Gap |
|---------|--------|---------------------------|-----|
| Generate nomor faktur | `Nomorjual()` — cek `penjualan` + `penjualan_ditahan` | Inline — cek `penjualan` + `penjualan_ditahan` | Sama tapi rawan race condition |
| Validasi stok | `CekStok()` + `SettingIzinkanJualStokMinus` | ❌ Tidak ada | **Gap kritis** |
| Validasi backdate | `SettingIzinkanTanggalLampau` | ❌ Tidak ada | **Gap kritis** |
| Cek duplikat faktur | Via UNIQUE KEY + error handling | ❌ Tidak ada pengecekan eksplisit | **Gap kritis** |
| Hitung stok | `HitungStokPerubahan()` | Inline SQL (copy dari VB) | Duplikasi |
| Update saldo akun | `UpdateSaldoSemuaAkun()` — **bug AKUN_DK** | Inline SQL — **bug AKUN_DK sama** | Duplikasi + bug |
| Update piutang pelanggan | `UpdatePiutangPelanggan()` | Inline SQL | Duplikasi |

#### Acceptance Criteria

1. WHEN `sync_penjualan.php` direfactor, THE `sync_penjualan.php` SHALL melakukan INSERT inline dengan validasi server via SP helper:
   ```php
   // Validasi stok via sp_hlp_stok_validasi, generate nomor via sp_hlp_faktur_generate
   // INSERT inline dalam $pdo->beginTransaction()
   // Update stok via sp_hlp_stok_hitung, update saldo via sp_hlp_saldo_akun_update
   ```
   Semua logika `SELECT MAX` inline untuk generate nomor dihapus.

2. WHEN `sync_stokopname.php` direfactor, THE `sync_stokopname.php` SHALL melakukan INSERT inline dengan cara yang sama — menghapus semua logika `SELECT MAX` inline untuk generate nomor.

3. THE refactored PHP SHALL menerima `p_izinkan_stok_minus` dan `p_izinkan_backdate` dari payload Flutter — nilai ini dibaca dari hak akses user yang sudah di-cache di Flutter saat login.

4. THE refactored PHP SHALL mengembalikan response yang konsisten:
   ```json
   // Sukses
   {"status": "success", "id_penjualan": "PJ-260419-0001"}
   
   // Error bisnis
   {"status": "error", "error_code": "STOK_KURANG", "message": "Stok barang X tidak cukup"}
   
   // Error duplikat
   {"status": "error", "error_code": "DUPLIKAT_FAKTUR", "message": "Nomor faktur sudah digunakan"}
   ```

5. THE PHP API SHALL tidak mengimplementasikan logika bisnis kritis secara inline setelah refactor — semua operasi tulis yang mempengaruhi integritas data harus melalui SP.

6. THE `update_product.php` (update kategori/merk dari mobile) **tidak perlu** dimigrasikan ke SP — ini adalah operasi master sederhana tanpa logika bisnis kritis.

#### Urutan Implementasi

- Refactor `sync_penjualan.php` dilakukan di Fase 2 (bersamaan dengan update `FormPenjualan.vb`)
- Refactor `sync_stokopname.php` dilakukan di Fase 4 (bersamaan dengan update `FormStokOpname.vb`)
- Kedua file PHP harus direfactor **setelah** SP-nya diverifikasi berjalan benar di VB

---

### Requirement 21: Update Harga Pokok Barang dari Pembelian dan Gap Jurnal ✅ SELESAI

**User Story:** Sebagai pemilik toko, saya ingin harga pokok barang diperbarui secara otomatis
saat pembelian disimpan — baik menggunakan harga baru (last purchase price) maupun harga rata-rata
(average cost) — dan jika ada selisih antara harga lama dan harga baru, selisih tersebut dicatat
sebagai jurnal penyesuaian agar neraca tetap seimbang.

#### Latar Belakang

Saat ini `FormPembelian.vb` menyimpan harga beli ke `tbl_barang` (kolom `HARGA_BELI` atau
`HARGA_BELI_RATA`) namun **tidak mencatat jurnal penyesuaian** atas selisih nilai persediaan
yang timbul akibat perubahan harga pokok. Ini menyebabkan:

1. **Nilai persediaan di neraca tidak akurat** — stok yang sudah ada dihitung dengan harga lama,
   tapi harga pokok di `tbl_barang` sudah berubah ke harga baru.
2. **Gap jurnal** — tidak ada entri debet/kredit untuk selisih `(harga_baru - harga_lama) × stok_saat_ini`.
3. **Dua metode harga** yang perlu ditangani berbeda:
   - **Harga Baru (Last Purchase Price):** `HARGA_BELI = harga_beli_terakhir` — selisih = `(harga_baru - harga_lama) × stok_saat_ini`
   - **Harga Rata-rata (Average Cost / AVCO):** `HARGA_BELI_RATA = (nilai_stok_lama + nilai_pembelian_baru) / (stok_lama + qty_beli)` — selisih = `harga_rata_baru × stok_lama - nilai_stok_lama`

#### Catatan Implementasi

Requirement ini **hanya dicatat sebagai task** di fase implementasi — belum diimplementasikan.
Perlu investigasi lebih lanjut terhadap:
- Kolom mana di `tbl_barang` yang dipakai (`HARGA_BELI` vs `HARGA_BELI_RATA`)
- Apakah sistem saat ini sudah pakai AVCO atau last price
- Akun jurnal yang tepat untuk penyesuaian nilai persediaan (PERSEDIAAN vs PENYESUAIAN PERSEDIAAN)
- Apakah penyesuaian dilakukan per item atau per transaksi pembelian


#### Acceptance Criteria

1. WHEN simpan pembelian, THE klien SHALL membaca `HARGA_BELI` (atau `HARGA_BELI_RATA`) lama dari `tbl_barang` sebelum diupdate.

2. IF metode harga yang dipakai adalah **Harga Baru (Last Price)** DAN `harga_beli_baru ≠ harga_beli_lama`, THEN THE SP SHALL menghitung selisih nilai persediaan:
   `selisih = (harga_beli_baru - harga_beli_lama) × stok_saat_ini`

3. IF metode harga yang dipakai adalah **Harga Rata-rata (AVCO)**, THEN THE SP SHALL menghitung harga rata-rata baru:
   `harga_rata_baru = (harga_beli_lama × stok_saat_ini + harga_beli_baru × qty_beli) / (stok_saat_ini + qty_beli)`
   dan selisih nilai persediaan:
   `selisih = (harga_rata_baru - harga_beli_lama) × stok_saat_ini`

4. IF `selisih ≠ 0`, THEN THE SP SHALL memanggil `INSERT INTO JurnalUmum` (inline) untuk mencatat jurnal penyesuaian nilai persediaan:
   - Jika `selisih > 0` (harga naik): Debet PERSEDIAAN, Kredit PENYESUAIAN PERSEDIAAN / SELISIH HARGA POKOK
   - Jika `selisih < 0` (harga turun): Debet PENYESUAIAN PERSEDIAAN / SELISIH HARGA POKOK, Kredit PERSEDIAAN

5. THE jurnal penyesuaian SHALL dieksekusi di dalam transaksi yang sama dengan simpan pembelian — tidak boleh ada kondisi di mana harga pokok diupdate tapi jurnal penyesuaian belum dicatat.

6. THE `sp_hlp_saldo_akun_update` SHALL dipanggil untuk akun PERSEDIAAN dan akun PENYESUAIAN PERSEDIAAN setelah jurnal penyesuaian di-INSERT.

7. IF `stok_saat_ini = 0` saat pembelian disimpan, THEN THE SP SHALL melewati kalkulasi selisih dan langsung update harga pokok tanpa jurnal penyesuaian — tidak ada stok lama yang perlu disesuaikan nilainya.

8. THE metode harga (Last Price atau AVCO) SHALL dibaca dari konfigurasi `GeneralSetting` atau parameter yang dikirim klien — tidak boleh hardcode di SP.

#### Urutan Implementasi

Requirement ini masuk **Fase 3 (Pembelian)** — diimplementasikan bersamaan dengan
implementasi simpan pembelian. Perlu investigasi dan keputusan arsitektur sebelum implementasi:
- Audit `FormPembelian.vb` untuk memahami logika update harga saat ini
- Tentukan akun jurnal penyesuaian yang tepat dari COA (`tbl_datareferensi`)
- Konfirmasi metode harga yang dipakai (Last Price / AVCO / keduanya bisa dipilih)

---

### Requirement 22: Biaya Tambahan Pembelian — Diskon Supplier, PPN Masukan, Biaya Kirim/Lainnya ✅ SELESAI

**User Story:** Sebagai pemilik toko, saya ingin bisa mencatat diskon yang diberikan supplier,
PPN masukan yang dibayar, dan biaya kirim/biaya lain yang timbul saat pembelian, agar nilai
persediaan, hutang, dan laporan keuangan mencerminkan biaya perolehan yang sesungguhnya.

#### Latar Belakang

Saat ini `FormPembelian.vb` dan tabel `pembelian` **tidak memiliki** field untuk:
- Diskon dari supplier (potongan harga dari total pembelian)
- PPN Masukan (pajak yang dibayar saat beli barang kena pajak)
- Biaya kirim/ongkos angkut pembelian (freight in)
- Biaya lain-lain yang dikeluarkan saat pembelian

Akibatnya, nilai `GRAND_TOTAL_BELI` yang tersimpan tidak mencerminkan biaya perolehan
sesungguhnya, dan jurnal pembelian tidak lengkap.

#### Akun COA yang Dipakai (dari `tbl_datareferensi_backup.sql`)

| Akun | Kode | AKUN_DK | Keterangan |
|------|------|---------|------------|
| POTONGAN DISKON PEMBELIAN | `06.05.001` | KREDIT | Diskon dari supplier — mengurangi HPP |
| PPN MASUKAN | `01.05.001` | DEBET | Pajak masukan yang bisa dikreditkan |
| BIAYA KIRIM PEMBELIAN | `06.02.001` | DEBET | Freight in — menambah HPP |

#### Definisi Biaya Tambahan

| Komponen | Arah | Efek ke Grand Total | Jurnal |
|----------|------|---------------------|--------|
| **Diskon supplier** | Mengurangi | `Grand Total = Subtotal - Diskon` | D PERSEDIAAN, K KAS/HUTANG (sudah ada) + D KAS/HUTANG, K POTONGAN DISKON PEMBELIAN |
| **PPN Masukan** | Menambah | `Grand Total = Subtotal + PPN` | D PPN MASUKAN, K KAS/HUTANG |
| **Biaya kirim** | Menambah | `Grand Total = Subtotal + Biaya Kirim` | D BIAYA KIRIM PEMBELIAN, K KAS/HUTANG |
| **Biaya lain-lain** | Menambah | `Grand Total = Subtotal + Biaya Lain` | D akun biaya yang dipilih, K KAS/HUTANG |

#### Formula Grand Total Pembelian

```
Grand Total = Subtotal Item
            - Diskon Supplier
            + PPN Masukan
            + Biaya Kirim
            + Biaya Lain-lain
```

Nilai yang dibayar/dihutang ke supplier = Grand Total (setelah semua komponen).

#### Acceptance Criteria

1. THE tabel `pembelian` SHALL ditambahkan kolom baru (DDL ALTER TABLE):
   - `DISKON_SUPPLIER DECIMAL(15,2) DEFAULT 0` — nominal diskon dari supplier
   - `DISKON_SUPPLIER_PERSEN DECIMAL(5,2) DEFAULT 0` — persentase diskon (opsional, untuk display)
   - `PPN_MASUKAN DECIMAL(15,2) DEFAULT 0` — nominal PPN masukan
   - `PPN_MASUKAN_PERSEN DECIMAL(5,2) DEFAULT 0` — persentase PPN (opsional)
   - `BIAYA_KIRIM DECIMAL(15,2) DEFAULT 0` — biaya kirim/freight in
   - `BIAYA_LAIN DECIMAL(15,2) DEFAULT 0` — biaya lain-lain
   - `KODE_AKUN_BIAYA_LAIN VARCHAR(20) DEFAULT ''` — akun untuk biaya lain-lain
   - `NAMA_AKUN_BIAYA_LAIN VARCHAR(50) DEFAULT ''` — nama akun biaya lain-lain

2. THE `FormPembelian.vb` SHALL menampilkan field input untuk:
   - Diskon supplier (nominal atau persen, keduanya bisa diisi)
   - PPN Masukan (nominal atau persen)
   - Biaya kirim (nominal)
   - Biaya lain-lain (nominal + pilih akun dari COA)

3. THE `FormPembelian.vb` SHALL menghitung `GRAND_TOTAL_BELI` secara realtime:
   `Grand Total = Subtotal - Diskon + PPN Masukan + Biaya Kirim + Biaya Lain`

4. WHEN simpan pembelian, THE klien SHALL menerima parameter tambahan:
   `p_diskon_supplier`, `p_ppn_masukan`, `p_biaya_kirim`, `p_biaya_lain`,
   `p_kode_akun_biaya_lain`, `p_nama_akun_biaya_lain`

5. WHEN simpan pembelian memposting jurnal, THE klien SHALL membuat entri jurnal
   untuk setiap komponen biaya tambahan yang nilainya > 0:
   - **Diskon supplier** (`p_diskon_supplier > 0`):
     Jurnal tambahan: D KAS/HUTANG (akun pembayaran), K POTONGAN DISKON PEMBELIAN (`06.05.001`)
     dengan nominal = `p_diskon_supplier`
   - **PPN Masukan** (`p_ppn_masukan > 0`):
     Jurnal tambahan: D PPN MASUKAN (`01.05.001`), K KAS/HUTANG
     dengan nominal = `p_ppn_masukan`
   - **Biaya Kirim** (`p_biaya_kirim > 0`):
     Jurnal tambahan: D BIAYA KIRIM PEMBELIAN (`06.02.001`), K KAS/HUTANG
     dengan nominal = `p_biaya_kirim`
   - **Biaya Lain** (`p_biaya_lain > 0`):
     Jurnal tambahan: D `p_kode_akun_biaya_lain`, K KAS/HUTANG
     dengan nominal = `p_biaya_lain`

6. THE jurnal utama pembelian (D PERSEDIAAN / K KAS atau K HUTANG) SHALL menggunakan
   nilai `p_subtotal_item` (sebelum diskon/PPN/biaya) — bukan `p_grand_total`.
   Setiap komponen biaya tambahan dicatat sebagai jurnal terpisah agar laporan lebih detail.

7. WHEN total Debet ≠ total Kredit setelah semua jurnal diposting, THE SP SHALL melakukan
   ROLLBACK dan mengembalikan error `JURNAL_TIDAK_SEIMBANG`.

8. THE `sp_hlp_saldo_akun_update` SHALL dipanggil untuk semua akun yang terlibat:
   PERSEDIAAN, KAS/HUTANG, POTONGAN DISKON PEMBELIAN, PPN MASUKAN, BIAYA KIRIM PEMBELIAN,
   dan akun biaya lain-lain (jika ada).

9. THE tabel `pembelian_ditahan` dan `pembelian_ditahan_detail` SHALL juga ditambahkan
   kolom yang sama agar draft pembelian bisa menyimpan nilai biaya tambahan.

10. IF semua komponen biaya tambahan = 0, THE klien SHALL berperilaku identik dengan
    simpan pembelian sebelum perubahan ini — backward compatible.

#### Urutan Implementasi

Requirement ini masuk **Fase 3 (Pembelian)** sebagai Task 3.5, dikerjakan setelah Task 3.1
(simpan pembelian dasar) selesai. Kolom baru ditambahkan ke file DDL yang sudah ada
(`Database/01_migrasi_kolom.sql`) menggunakan pola `IF NOT EXISTS` yang sudah ada di file tersebut.

> **Catatan:** Ini adalah **fitur baru** — tabel `pembelian` saat ini tidak memiliki kolom
> diskon/PPN/biaya kirim. Kolom baru harus ditambahkan ke `01_migrasi_kolom.sql` dan dijalankan
> di semua environment (dev, staging, production) sebelum kode baru di-deploy.
> Tabel `pembelian_ditahan` **tidak perlu** kolom biaya tambahan — berdasarkan `01_migrasi_kolom.sql`,
> draft sudah di-cleanup dari semua kolom pembayaran (hanya simpan header + detail item).

---

### Requirement 23: Split Bayar Tunai + Transfer untuk Pembelian ✅ SELESAI

**User Story:** Sebagai kasir, saya ingin bisa membayar pembelian ke supplier dengan kombinasi
tunai dan transfer bank sekaligus (split payment), seperti yang sudah bisa dilakukan di
penjualan, agar pembayaran yang melibatkan dua rekening tercatat dengan benar di jurnal.

#### Latar Belakang

`FormPenjualan.vb` sudah mendukung split payment penuh:
- `TxtNominalBayarTunai` → akun KAS (pilih via `CmbBayarTunai`)
- `TxtNominalBayarTransfer` → akun BANK (pilih via `CmbBayarTransfer`)
- Jurnal terpisah: J1 D KAS + J2 D BANK, keduanya K PENJUALAN

`FormPembelian.vb` kondisi saat ini:
- `CmbJenisBayarTransfer` sudah ada di UI tapi **tidak dipakai** di logika simpan
- Tabel `pembelian` sudah punya kolom `NOMINAL_TRANSFER`, `KODE_AKUN_TF`, `NAMA_AKUN_TF` tapi tidak diisi
- Jurnal pembelian hanya satu entri: D PERSEDIAAN / K KAS — tidak ada jurnal terpisah untuk transfer
- `TxtNominalBayarTunai` saat ini menampung **total bayar** (bukan hanya tunai)

Ini berarti infrastruktur sebagian sudah ada — perlu disambungkan ke logika simpan dan SP.

#### Pola Split Bayar Pembelian

```
Grand Total = Bayar Tunai (KAS) + Bayar Transfer (BANK) + Sisa Hutang
```

| Skenario | Tunai | Transfer | Hutang | Jurnal |
|----------|-------|----------|--------|--------|
| Lunas tunai | > 0 | 0 | 0 | D PERSEDIAAN / K KAS |
| Lunas transfer | 0 | > 0 | 0 | D PERSEDIAAN / K BANK |
| Split tunai+transfer | > 0 | > 0 | 0 | D PERSEDIAAN / K KAS + D PERSEDIAAN / K BANK |
| DP tunai + hutang | > 0 | 0 | > 0 | D PERSEDIAAN / K KAS + D PERSEDIAAN / K HUTANG |
| DP transfer + hutang | 0 | > 0 | > 0 | D PERSEDIAAN / K BANK + D PERSEDIAAN / K HUTANG |
| Split + hutang | > 0 | > 0 | > 0 | D PERSEDIAAN / K KAS + D PERSEDIAAN / K BANK + D PERSEDIAAN / K HUTANG |
| Hutang semua | 0 | 0 | > 0 | D PERSEDIAAN / K HUTANG |

#### Acceptance Criteria

1. THE `FormPembelian.vb` SHALL memisahkan field bayar menjadi dua:
   - `TxtNominalBayarTunai` — nominal bayar via KAS (pilih akun via `CmbJenisBayarTunai`)
   - `TxtNominalBayarTransfer` — nominal bayar via BANK (pilih akun via `CmbJenisBayarTransfer`)

2. THE `FormPembelian.vb` SHALL menghitung sisa hutang secara realtime:
   `Sisa Hutang = Grand Total - Bayar Tunai - Bayar Transfer`
   Jika `Sisa Hutang > 0` → status "Belum Lunas", tampilkan field jatuh tempo.
   Jika `Sisa Hutang ≤ 0` → status "Lunas".

3. THE `FormPembelian.vb` SHALL menampilkan panel info transfer (nama bank, no. rekening)
   saat `TxtNominalBayarTransfer > 0` — konsisten dengan pola di `FormPenjualan.vb`.

4. THE tabel `pembelian` kolom `NOMINAL_TRANSFER`, `KODE_AKUN_TF`, `NAMA_AKUN_TF` yang
   sudah ada SHALL diisi dengan benar saat simpan.

5. WHEN simpan pembelian, THE klien SHALL menerima parameter:
   `p_bayar_tunai DECIMAL(15,2)`, `p_bayar_transfer DECIMAL(15,2)`,
   `p_kode_akun_transfer VARCHAR(20)`, `p_nama_akun_transfer VARCHAR(50)`

6. WHEN simpan pembelian memposting jurnal, THE klien SHALL membuat entri jurnal
   terpisah untuk setiap metode bayar yang nilainya > 0:
   - Jika `p_bayar_tunai > 0`: Jurnal D PERSEDIAAN / K KAS (`p_kode_akun_kas`) nominal `p_bayar_tunai`
   - Jika `p_bayar_transfer > 0`: Jurnal D PERSEDIAAN / K BANK (`p_kode_akun_transfer`) nominal `p_bayar_transfer`
   - Jika `p_tagihan > 0`: Jurnal D PERSEDIAAN / K HUTANG BELANJA (`p_kode_rek_hutang`) nominal `p_tagihan`

7. THE `sp_hlp_saldo_akun_update` SHALL dipanggil untuk semua akun yang terlibat:
   PERSEDIAAN, KAS (jika tunai > 0), BANK (jika transfer > 0), HUTANG (jika tagihan > 0).

8. THE validasi SHALL memastikan: `p_bayar_tunai + p_bayar_transfer + p_tagihan = p_grand_total`
   — jika tidak seimbang, SP mengembalikan error `NOMINAL_BAYAR_TIDAK_SEIMBANG`.

9. IF `p_bayar_transfer > 0` DAN `p_kode_akun_transfer` kosong, THEN THE SP SHALL
   mengembalikan error `AKUN_TRANSFER_TIDAK_VALID`.

10. THE tabel `pembelian_ditahan` SHALL menyimpan `NOMINAL_TRANSFER`, `KODE_AKUN_TF`,
    `NAMA_AKUN_TF` agar draft pembelian dengan split bayar bisa dipanggil kembali dengan benar.

#### Urutan Implementasi

Requirement ini masuk **Fase 3 (Pembelian)** sebagai Task 3.6, dikerjakan bersamaan atau
setelah Task 3.5 (biaya tambahan). Tidak perlu DDL baru — kolom `NOMINAL_TRANSFER`,
`KODE_AKUN_TF`, `NAMA_AKUN_TF` sudah ada di tabel `pembelian` (sudah di-migrate via
`Database/01_migrasi_kolom.sql` migrasi #4-6). Tabel `pembelian_ditahan` tidak perlu
kolom transfer — draft sudah di-cleanup dari semua kolom pembayaran.

---

### Requirement 24: Pelunasan Saldo Awal Hutang/Piutang dari Master ✅ SELESAI

**User Story:** Sebagai kasir, saya ingin bisa melunasi hutang ke supplier dan piutang dari
pelanggan yang berasal dari saldo awal pembukaan (bukan dari transaksi pembelian/penjualan),
agar semua hutang/piutang bisa diselesaikan melalui satu form yang sama.

#### Analisa Kondisi Saat Ini (dari kode aktual)

**FormBayarHutang.vb:**
```sql
SELECT ... FROM pembelian WHERE ID_SUPPLIER = @ID AND STATUS_TRANSAKSI_BELI = 'Belum Lunas'
```
Hanya menampilkan hutang dari tabel `pembelian`. Hutang dari `HutangAwal` di `tbl_supliyer`
**tidak muncul** — tidak ada faktur pembelian yang mewakilinya.

**FormBayarPiutang.vb:**
```sql
SELECT ... FROM penjualan WHERE ID_PELANGGAN = @ID AND STATUS_TRANSAKSI = 'Belum Lunas'
```
Sama — hanya dari tabel `penjualan`. Piutang dari `HutangAwal` di `tbl_pelanggan` tidak muncul.

**Rumus HutangAkhir yang sudah benar:**
```sql
-- Supplier
SET s.HutangAkhir = IFNULL(x.HUTANG, 0) + s.HutangAwal
-- Pelanggan
SET p.HutangAkhir = IFNULL(x.HUTANG, 0) + p.HutangAwal
```
Saldo awal sudah masuk ke `HutangAkhir` — tapi tidak bisa dilunasi karena tidak ada baris
di tabel transaksi yang mewakilinya.

**Jurnal saldo awal (TambahSupliyer.vb / TambahPelanggan.vb):**
- Sudah membuat jurnal saat input/edit `HutangAwal` ✅
- Tapi akun yang dipakai: D MODAL (`04.01.001`) / K TAGIHAN SALDO PIUTANG (`01.04.002`)
- Berbeda dari akun hutang belanja (`03.01.001`) yang dipakai di transaksi normal
- Ini perlu dikonfirmasi apakah sudah sesuai dengan COA yang diinginkan

**Masalah saat HutangAwal diubah setelah ada transaksi:**
- `UpdateSupliyer()` sudah membuat jurnal selisih ✅
- Tapi `HutangAkhir` **tidak langsung direcalculate** — hanya diupdate saat ada transaksi
  berikutnya yang memanggil `UpdateHutangSupliyer()` atau saat `FormLoading` dijalankan
- Jika tidak ada transaksi baru, `HutangAkhir` yang ditampilkan bisa stale

**Bon Karyawan:**
- `SaldoAwal` di `tbl_karyawan` tidak pernah diisi dari form master (selalu 0)
- `FormBon.vb` menampilkan `SaldoAkhir` langsung dari `tbl_karyawan` — sudah mencerminkan
  semua bon yang pernah dibuat, tidak ada masalah saldo awal yang perlu dilunasi

#### Masalah Kritis: Edit HutangAwal Setelah Ada Pembayaran → HutangAkhir Minus ✅ SELESAI

**Skenario bahaya konkret (dari laporan user):**

```
Kondisi awal:
  HutangAwal supplier  = 1.000.000
  Sudah dibayar        = 1.000.000  (via FormBayarHutang — mengurangi TAGIHAN di pembelian)
  HutangAkhir          = SUM(TAGIHAN pembelian) + HutangAwal
                       = 0 + 1.000.000 = 1.000.000
  (catatan: pembayaran via FormBayarHutang hanya mengurangi TAGIHAN di tabel pembelian,
   TIDAK mengurangi HutangAwal di tbl_supliyer)

User edit HutangAwal dari 1.000.000 → 500.000:
  HutangAkhir baru = SUM(TAGIHAN pembelian) + HutangAwal
                   = 0 + 500.000 = 500.000
  Tapi di jurnal sudah ada pembayaran 1.000.000 untuk hutang ini
  → Saldo akun hutang di neraca = -500.000 (MINUS)
  → Neraca tidak seimbang
```

**Akar masalah:** Tidak ada kolom `HutangAwalTerbayar` yang melacak berapa dari `HutangAwal`
yang sudah dilunasi. `HutangAwal` adalah angka tunggal yang bisa diubah bebas tanpa validasi
terhadap riwayat pembayaran. Sistem tidak bisa membedakan:
- "Saldo awal 1.000.000 belum pernah dibayar" vs
- "Saldo awal 1.000.000 sudah dibayar 1.000.000 via jurnal manual"

**Skenario yang sama berlaku untuk piutang pelanggan.**

**Dua pendekatan solusi:**

| Pendekatan | Kelebihan | Kekurangan |
|------------|-----------|------------|
| **A — Validasi + peringatan** | Sederhana, tidak perlu DDL | Tidak mencegah 100%, hanya peringatan |
| **B — Tambah kolom `HutangAwalTerbayar`** | Akurat, bisa validasi ketat | Perlu DDL + migrasi data historis |

**Rekomendasi: Pendekatan A** — Saat user menurunkan `HutangAwal`, hitung estimasi yang sudah
terbayar dari riwayat jurnal (`JurnalUmum WHERE JENIS_TRANSAKSI = 'Bayar Hutang Saldo Awal'`),
tampilkan peringatan, dan minta konfirmasi eksplisit.

#### Acceptance Criteria

1. THE `FormBayarHutang.vb` SHALL menampilkan baris tambahan untuk saldo awal hutang supplier
   jika `tbl_supliyer.HutangAwal > 0` — baris ini tidak berasal dari tabel `pembelian` tapi
   dari kolom `HutangAwal` di `tbl_supliyer`.

2. WHEN kasir melunasi saldo awal hutang supplier, THE sistem SHALL:
   - Mengurangi `tbl_supliyer.HutangAwal` sebesar nominal yang dibayar
   - Memanggil `UpdateHutangSupliyer()` untuk recalculate `HutangAkhir`
   - Membuat jurnal: D HUTANG BELANJA (`03.01.001`) / K KAS dengan `JENIS_TRANSAKSI = 'Bayar Hutang Saldo Awal'`

3. THE `FormBayarPiutang.vb` SHALL menampilkan baris tambahan untuk saldo awal piutang pelanggan
   jika `tbl_pelanggan.HutangAwal > 0`.

4. WHEN kasir menerima pelunasan saldo awal piutang pelanggan, THE sistem SHALL:
   - Mengurangi `tbl_pelanggan.HutangAwal` sebesar nominal yang diterima
   - Memanggil `UpdatePiutangPelanggan()` untuk recalculate `HutangAkhir`
   - Membuat jurnal: D KAS / K PIUTANG USAHA (`01.03.001`) dengan `JENIS_TRANSAKSI = 'Bayar Piutang Saldo Awal'`

5. WHEN `HutangAwal` supplier atau pelanggan diubah di form master, THE sistem SHALL
   langsung memanggil `UpdateHutangSupliyer()` atau `UpdatePiutangPelanggan()` setelah
   UPDATE — sehingga `HutangAkhir` selalu up-to-date tanpa menunggu transaksi berikutnya.

6. WHEN user membuka form edit supplier atau pelanggan yang sudah memiliki `HutangAwal > 0`,
   THE `TxtAwal` (field input saldo awal) SHALL di-set `ReadOnly = True` — tidak bisa diubah
   sama sekali. Ini mencegah kondisi `HutangAkhir` menjadi negatif akibat penurunan saldo awal
   yang sudah pernah dibayar.

7. IF user perlu mengkoreksi `HutangAwal` yang sudah ada, THE sistem SHALL menyediakan
   mekanisme koreksi yang aman: hapus supplier/pelanggan dan buat ulang, atau gunakan jurnal
   manual untuk penyesuaian — bukan edit langsung field `HutangAwal`.

8. WHEN `HutangAwal = 0` (supplier/pelanggan baru atau belum pernah ada saldo awal),
   THE `TxtAwal` SHALL tetap bisa diisi — readonly hanya berlaku jika `HutangAwal > 0`.

7. WHEN simpan bayar hutang atau bayar piutang, THE klien SHALL mendukung
   flag `p_dari_saldo_awal TINYINT(1)` untuk membedakan pembayaran dari saldo awal
   vs dari faktur transaksi — agar jurnal yang dibuat menggunakan `JENIS_TRANSAKSI` yang tepat.

8. THE jurnal saldo awal di `TambahSupliyer.vb` dan `TambahPelanggan.vb` SHALL dikonfirmasi
   menggunakan akun yang benar:
   - Hutang supplier: D MODAL (`04.01.001`) / K HUTANG BELANJA (`03.01.001`) — bukan `01.04.002`
   - Piutang pelanggan: D PIUTANG USAHA (`01.03.001`) / K MODAL (`04.01.001`)
   - Perlu audit apakah akun `01.04.002` (TAGIHAN/SALDO PIUTANG) yang saat ini dipakai sudah benar

#### Urutan Implementasi

- Requirement ini masuk **Fase 5 (Bayar)** — dikerjakan bersamaan dengan Task 5.4 dan 5.5
- Audit akun jurnal saldo awal (AC #8) bisa dilakukan lebih awal di Fase 1 tanpa perubahan kode
- Perbaikan recalculate `HutangAkhir` saat edit master (AC #5) dan validasi edit (AC #6)
  masuk **Fase 9** bersama perbaikan form master lainnya

---

### Requirement 25: Pencatatan Hutang/Piutang Non-Transaksi via FormKeuangan ✅ SELESAI

**User Story:** Sebagai pemilik toko, saya ingin bisa mencatat hutang ke supplier atau piutang
dari pelanggan yang bukan berasal dari transaksi pembelian/penjualan tetapi melibatkan uang
tunai (misal: supplier beri pinjaman modal tunai, pelanggan pinjam uang tunai dari toko),
dan melunasinya melalui `FormBayarHutang`/`FormBayarPiutang` yang sudah ada.

#### Latar Belakang

Setelah `TxtAwal` dikunci readonly (Req 24), tidak ada cara untuk menambah hutang/piutang
non-transaksi ke supplier/pelanggan. Saldo awal hanya bisa diisi sekali saat pertama kali
membuat master.

**Dua skenario konkret yang perlu didukung:**

| Skenario | Contoh | Kas | Hutang/Piutang |
|----------|--------|-----|----------------|
| **Supplier beri pinjaman tunai** | Supplier kasih modal 5 juta tunai | KAS masuk (+) | Hutang ke supplier bertambah (+) |
| **Pelanggan pinjam uang tunai** | Pelanggan pinjam 2 juta dari toko | KAS keluar (-) | Piutang ke pelanggan bertambah (+) |

**Jurnal yang benar:**

```
Skenario 1 — Supplier beri pinjaman tunai:
  D KAS (01.01.001)              5.000.000
  K HUTANG BELANJA (03.01.001)   5.000.000
  → KAS bertambah, hutang ke supplier bertambah

Skenario 2 — Pelanggan pinjam uang tunai:
  D PIUTANG USAHA (01.03.001)    2.000.000
  K KAS (01.01.001)              2.000.000
  → KAS berkurang, piutang ke pelanggan bertambah
```

**Implikasi ke laporan mutasi kas:**
- Skenario 1: KAS masuk → muncul di laporan mutasi kas sebagai pemasukan
- Skenario 2: KAS keluar → muncul di laporan mutasi kas sebagai pengeluaran

`FormKeuangan` sudah punya infrastruktur yang tepat:
- `CmbBantuD/K` — field pembantu untuk nama entitas (supplier/pelanggan)
- Sudah ada jenis PEMASUKAN (D KAS / K akun lain) dan PENGELUARAN (D akun lain / K KAS)

Yang belum ada: jenis transaksi yang **sekaligus mengupdate `HutangAwal`** di master
supplier/pelanggan agar hutang/piutang non-transaksi bisa muncul di `FormBayarHutang`/`FormBayarPiutang`.

#### Mekanisme yang Direkomendasikan

Tambah dua jenis transaksi baru di `FormKeuangan`:

| Jenis | Tombol | Akun Debet | Akun Kredit | Efek Kas | Update Master |
|-------|--------|-----------|-------------|----------|---------------|
| **PINJAMAN SUPPLIER** | BtnPinjamanSupplier | KAS (pilih rekening) | HUTANG BELANJA (`03.01.001`) | KAS masuk (+) | `tbl_supliyer.HutangAwal += nominal` |
| **PINJAMAN PELANGGAN** | BtnPinjamanPelanggan | PIUTANG USAHA (`01.03.001`) | KAS (pilih rekening) | KAS keluar (-) | `tbl_pelanggan.HutangAwal += nominal` |

Dengan mekanisme ini:
- Kas bergerak tercatat di jurnal ✅
- Muncul di laporan mutasi kas ✅
- `HutangAwal` supplier/pelanggan bertambah ✅
- `HutangAkhir` otomatis terupdate ✅
- Muncul di `FormBayarHutang`/`FormBayarPiutang` sebagai baris "Saldo Awal" yang bisa dilunasi ✅

#### Acceptance Criteria

1. THE `FormKeuangan.vb` SHALL menambahkan dua tombol baru di toolbar:
   - `BtnPinjamanSupplier` — "PINJAMAN SUPPLIER"
   - `BtnPinjamanPelanggan` — "PINJAMAN PELANGGAN"

2. WHEN user klik `BtnPinjamanSupplier`, THE form SHALL:
   - Menampilkan `CmbBantuKKeuangan` dengan daftar supplier aktif dari `tbl_supliyer`
   - Akun Debet: filter ke KAS/BANK saja (`AddFromTypes(debetItems, byType, {"KAS", "BANK"})`)
   - Akun Kredit: filter ke HUTANG saja (`AddFromTypes(kreditItems, byType, {"HUTANG"})`)
   - `JENIS_TRANSAKSI = 'PINJAMAN SUPPLIER'`

3. WHEN user klik `BtnPinjamanPelanggan`, THE form SHALL:
   - Menampilkan `CmbBantuDKeuangan` dengan daftar pelanggan aktif dari `tbl_pelanggan`
   - Akun Debet: filter ke PIUTANG saja (`AddFromTypes(debetItems, byType, {"PIUTANG"})`)
   - Akun Kredit: filter ke KAS/BANK saja (`AddFromTypes(kreditItems, byType, {"KAS", "BANK"})`)
   - `JENIS_TRANSAKSI = 'PINJAMAN PELANGGAN'`

4. WHEN transaksi PINJAMAN SUPPLIER disimpan, THE sistem SHALL:
   - INSERT ke `JurnalUmum` (D KAS / K HUTANG BELANJA)
   - UPDATE `tbl_supliyer.HutangAwal = HutangAwal + nominal`
   - CALL `UpdateHutangSupliyer(kode_supplier)` untuk recalculate `HutangAkhir`
   - Semua dalam satu transaksi MySQL

5. WHEN transaksi PINJAMAN PELANGGAN disimpan, THE sistem SHALL:
   - INSERT ke `JurnalUmum` (D PIUTANG USAHA / K KAS)
   - UPDATE `tbl_pelanggan.HutangAwal = HutangAwal + nominal`
   - CALL `UpdatePiutangPelanggan(kode_pelanggan)` untuk recalculate `HutangAkhir`
   - Semua dalam satu transaksi MySQL

6. WHEN transaksi PINJAMAN SUPPLIER atau PINJAMAN PELANGGAN dihapus/dibatalkan, THE sistem SHALL:
   - Mengurangi `HutangAwal` sebesar nominal yang dibatalkan
   - CALL `UpdateHutangSupliyer()` atau `UpdatePiutangPelanggan()` untuk recalculate

7. THE `FormBayarHutang.vb` SHALL menampilkan baris "Saldo Awal" jika `tbl_supliyer.HutangAwal > 0`
   — termasuk hutang yang ditambah via PINJAMAN SUPPLIER di `FormKeuangan`.

8. THE `FormBayarPiutang.vb` SHALL menampilkan baris "Saldo Awal" jika `tbl_pelanggan.HutangAwal > 0`
   — termasuk piutang yang ditambah via PINJAMAN PELANGGAN di `FormKeuangan`.

9. THE `FormLapMutasiKeuangan.vb` SHALL menampilkan transaksi PINJAMAN SUPPLIER dan
   PINJAMAN PELANGGAN di laporan mutasi kas:
   - PINJAMAN SUPPLIER (D KAS): muncul di baris "(+) Jurnal Pemasukan" atau baris baru "(+) Pinjaman Supplier"
   - PINJAMAN PELANGGAN (K KAS): muncul di baris "(-) Jurnal Pengeluaran" atau baris baru "(-) Pinjaman Pelanggan"

10. THE update `HutangAwal` SHALL dilakukan langsung di `FormKeuangan.vb` setelah INSERT jurnal
    berhasil — dalam satu transaksi MySQL yang sama. Logika ini spesifik untuk dua jenis transaksi baru
    (PINJAMAN SUPPLIER dan PINJAMAN PELANGGAN) yang hanya ada di `FormKeuangan`.

#### Urutan Implementasi

- Requirement ini masuk **Fase 9** — dikerjakan bersamaan dengan Task 9.3 (`FormKeuangan.vb`)
- Bergantung pada Task 5.4 dan 5.5 (bayar hutang/piutang saldo awal) yang sudah selesai
- Tidak perlu DDL baru — `HutangAwal` sudah ada di `tbl_supliyer` dan `tbl_pelanggan`

---

### Requirement 26: Laporan Mutasi Keuangan — Tampilkan Bon, Bayar Bon, dan Gaji Terpisah ✅ SELESAI

**User Story:** Sebagai pemilik toko, saya ingin laporan mutasi keuangan menampilkan bon
karyawan, bayar bon, dan gaji sebagai baris terpisah — bukan digabung ke "Jurnal Pengeluaran"
atau "Jurnal Pemasukan" — agar kasir bisa mempertanggungjawabkan setiap kas yang keluar
secara detail.

#### Kondisi Saat Ini

Di `FormLapMutasiKeuangan.vb`, `LoadRekapSekaliBaca` menggabungkan:
- `'Bayar bon'` → masuk ke baris **Jurnal Pemasukan** (D KAS)
- `'Bon'` dan `'Gaji'` → masuk ke baris **Jurnal Pengeluaran** (K KAS)

Akibatnya kasir tidak bisa melihat berapa kas yang keluar khusus untuk bon dan gaji.

#### Perubahan yang Diinginkan

Pisahkan menjadi baris tersendiri:

| Baris Baru | Jenis Transaksi | Arah Kas | Tanda |
|------------|----------------|----------|-------|
| **Bon Karyawan** | `'Bon'` | KAS keluar | (-) |
| **Bayar Bon** | `'Bayar bon'` | KAS masuk | (+) |
| **Gaji Karyawan** | `'Gaji'` | KAS keluar | (-) |

Dan hapus `'Bayar bon'`, `'Bon'`, `'Gaji'` dari baris Pemasukan/Pengeluaran yang lama agar tidak double-count.

#### Acceptance Criteria

1. THE `LoadRekapSekaliBaca` SHALL memisahkan bon, bayar bon, dan gaji menjadi CASE WHEN tersendiri:
   - `BonTotal` / `BonNota`: `JENIS_TRANSAKSI='Bon' AND NOMOR_AKUN_K=@AKUN`
   - `BayarBonTotal` / `BayarBonNota`: `JENIS_TRANSAKSI='Bayar bon' AND NOMOR_AKUN_D=@AKUN`
   - `GajiTotal` / `GajiNota`: `JENIS_TRANSAKSI='Gaji' AND NOMOR_AKUN_K=@AKUN`

2. THE CASE WHEN untuk Pemasukan SHALL dihapus `'Bayar bon'` dari IN clause:
   ```sql
   -- Sebelum: JENIS_TRANSAKSI IN ('Pemasukan','Bayar bon')
   -- Sesudah: JENIS_TRANSAKSI = 'Pemasukan'
   ```

3. THE CASE WHEN untuk Pengeluaran SHALL dihapus `'Bon'` dan `'Gaji'` dari IN clause:
   ```sql
   -- Sebelum: JENIS_TRANSAKSI IN ('Pengeluaran','Bon','Gaji')
   -- Sesudah: JENIS_TRANSAKSI = 'Pengeluaran'
   ```

4. THE `TxtTotal_TextChanged` SHALL menambahkan kalkulasi baru:
   - `totalHariIni -= Bon` (KAS keluar)
   - `totalHariIni += BayarBon` (KAS masuk)
   - `totalHariIni -= Gaji` (KAS keluar)

5. THE print layout (`PD_PrintPage` dan `PDDot_PrintPage`) SHALL menampilkan baris baru
   secara kondisional (hanya jika nilai ≠ 0):
   - `"(-) Bon Karyawan"`
   - `"(+) Bayar Bon"`
   - `"(-) Gaji Karyawan"`

6. THE designer form SHALL menambahkan TextBox baru:
   - `TxtTotalBon`, `TxtNotaBon`
   - `TxtTotalBayarBon`, `TxtNotaBayarBon`
   - `TxtTotalGaji`, `TxtNotaGaji`

7. THE saldo akhir SHALL tetap sama setelah pemisahan — tidak ada perubahan nilai total,
   hanya perubahan tampilan dari digabung menjadi terpisah.

#### Urutan Implementasi

Requirement ini masuk **Fase 9** — dikerjakan bersamaan dengan Task 9.3f
(`FormLapMutasiKeuangan.vb`).

---

### Catatan COA: Akun Retur Pembelian Ditambahkan

Berdasarkan `Database/MigrasiCOA_Baru.sql` dan `Database/tbl_datareferensi_backup.sql`, COA baru tidak memiliki akun khusus untuk RETUR PEMBELIAN. Akun baru telah ditambahkan langsung ke kedua file SQL:

- **`06.06.001` RETUR PEMBELIAN** — HPP, KREDIT, LABA RUGI (kontra-HPP)
  - Konsisten dengan pola `05.03.001` RETUR PENJUALAN di sisi pendapatan
  - Dipakai sebagai akun kredit jurnal retur pembelian (Task 4.2)
