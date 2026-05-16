# Requirements Document — Sistem Aktivasi Lisensi KasirLancar

## Pendahuluan

Fitur ini menggantikan sistem aktivasi lisensi yang sudah ada pada aplikasi kasir desktop **KasirLancar** (VB.NET Windows Forms). Sistem baru terdiri dari dua komponen utama:

1. **LicenseManager** — komponen di dalam KasirLancar yang memvalidasi lisensi secara berlapis (USB Key 4 lapis + aktivasi manual fallback).
2. **Aktivator** — aplikasi mobile Flutter milik pengembang (Adi) yang terhubung ke **Supabase** sebagai backend, digunakan untuk mengelola data pelanggan, menerbitkan lisensi, dan men-generate Activation Key.

Sistem ini dirancang untuk menggantikan:
- KeyGenerator dengan algoritma matematika sederhana yang mudah di-reverse
- Penyimpanan lisensi dalam `license.ini` plain text yang mudah dimanipulasi
- Trial period via `config.bin` yang tidak terproteksi

### Arsitektur Sistem (Ringkasan)

```
[KasirLancar - VB.NET]          [Aktivator - Flutter Mobile]
       |                                    |
  LicenseManager                    Supabase Backend
  - Validasi USB Key (4 lapis)      - Tabel: pelanggan
  - Validasi license.ini            - Tabel: komputer (multi per pelanggan)
  - Validasi Registry               - Tabel: lisensi
  - Tampilkan Activation_Form       - Auth: login pengembang
```

---

## Glosarium

- **KasirLancar**: Aplikasi kasir/POS desktop berbasis VB.NET Windows Forms.
- **LicenseManager**: Komponen di KasirLancar yang menangani seluruh validasi lisensi.
- **Aktivator**: Aplikasi mobile Flutter milik pengembang untuk menerbitkan dan mengelola lisensi.
- **Supabase**: Backend-as-a-Service (PostgreSQL) yang digunakan Aktivator sebagai database terpusat.
- **Pelanggan**: Entitas bisnis (toko/usaha) yang membeli lisensi KasirLancar. Satu pelanggan dapat memiliki banyak komputer terdaftar.
- **Komputer**: Satu unit mesin yang menjalankan KasirLancar, diidentifikasi oleh HW_ID uniknya.
- **USB_Key**: Perangkat USB fisik sebagai media lisensi hardware dengan 4 lapis keamanan.
- **USB_VSN**: Volume Serial Number dari USB Key (bisa diubah via software).
- **USB_PSN**: Physical Serial Number dari chip USB Key (permanen, tidak bisa diubah software).
- **HW_ID**: Hardware ID komputer — hash SHA-256 dari kombinasi serial motherboard + processor + volume serial drive C.
- **License_File**: File lisensi terenkripsi AES-256 yang disimpan di dalam USB Key.
- **Activation_Key**: Kunci aktivasi yang di-generate Aktivator, diberikan ke pelanggan untuk aktivasi manual.
- **Serial_Number (SN)**: Nomor seri unik per lisensi yang di-generate Aktivator saat mendaftarkan komputer pelanggan.
- **HMAC_Engine**: Komponen HMAC-SHA256 untuk pembuatan dan verifikasi tanda tangan kriptografis.
- **Secret_Key**: Kunci rahasia pengembang yang digunakan HMAC_Engine; tidak pernah didistribusikan.
- **License_Registry**: Backup data lisensi di Windows Registry pada komputer pelanggan.
- **Anti_Tamper**: Mekanisme deteksi modifikasi tidak sah pada file atau data lisensi.
- **Activation_Form**: UI di KasirLancar yang tampil ketika lisensi tidak valid.

---

## Requirements

### Requirement 1: Kalkulasi Hardware ID

**User Story:** Sebagai sistem KasirLancar, saya ingin menghitung Hardware ID yang unik dan konsisten dari komponen hardware komputer, agar lisensi dapat diikat secara spesifik ke satu mesin.

#### Acceptance Criteria

1. THE LicenseManager SHALL menghitung HW_ID dengan meng-hash kombinasi serial number motherboard, serial number processor, dan volume serial drive C menggunakan SHA-256.
2. WHEN salah satu komponen hardware tidak dapat dibaca, THEN THE LicenseManager SHALL mencatat error spesifik dan mengembalikan kode error tanpa crash.
3. THE LicenseManager SHALL menghasilkan nilai HW_ID yang identik pada setiap pemanggilan selama hardware tidak berubah.
4. WHEN salah satu dari tiga komponen hardware berubah, THE LicenseManager SHALL menghasilkan nilai HW_ID yang berbeda.

---

### Requirement 2: Pembacaan Identitas USB Key

**User Story:** Sebagai LicenseManager, saya ingin membaca identitas lengkap USB Key yang terpasang, agar dapat memverifikasi keaslian USB Key secara berlapis.

#### Acceptance Criteria

1. WHEN sebuah USB drive terpasang, THE LicenseManager SHALL membaca USB_VSN dari sistem operasi Windows.
2. WHEN sebuah USB drive terpasang, THE LicenseManager SHALL membaca USB_PSN dari chip perangkat melalui Windows Device Management API (WMI Win32_DiskDrive).
3. IF USB_PSN tidak dapat dibaca, THEN THE LicenseManager SHALL menandai USB tersebut tidak valid dan tidak melanjutkan validasi.
4. THE LicenseManager SHALL membedakan USB Key sah dari USB biasa berdasarkan keberadaan License_File terenkripsi di folder tersembunyi `.kasirlancar\`.

---

### Requirement 3: Enkripsi dan Dekripsi License File di USB

**User Story:** Sebagai pengembang, saya ingin License_File di USB Key dienkripsi, agar isinya tidak dapat dibaca atau dimanipulasi meskipun file berhasil disalin ke media lain.

#### Acceptance Criteria

1. THE Aktivator SHALL mengenkripsi License_File menggunakan AES-256 dengan kunci yang diturunkan dari kombinasi USB_VSN + USB_PSN + HW_ID pelanggan.
2. THE LicenseManager SHALL mendekripsi License_File menggunakan kunci yang sama (diturunkan dari hardware saat itu).
3. WHEN License_File berhasil didekripsi, THE LicenseManager SHALL memverifikasi integritas isi menggunakan HMAC-SHA256 signature di dalam file.
4. IF dekripsi gagal karena kunci tidak cocok, THEN THE LicenseManager SHALL menandai USB tidak valid tanpa mengekspos detail teknis ke pengguna.
5. IF HMAC-SHA256 signature tidak valid setelah dekripsi, THEN THE LicenseManager SHALL menandai lisensi sebagai tampered dan menolak aktivasi.
6. FOR ALL License_File yang valid, proses enkripsi → dekripsi SHALL menghasilkan data identik dengan data asli (round-trip property).

---

### Requirement 4: Validasi USB Key Berlapis (4 Lapis)

**User Story:** Sebagai pengembang, saya ingin validasi USB Key dilakukan dalam 4 lapis berurutan, agar USB Key tidak dapat digunakan hanya dengan menyalin file atau mengubah volume serial.

#### Acceptance Criteria

1. THE LicenseManager SHALL memvalidasi USB Key secara berurutan: Lapis 1 (USB_VSN) → Lapis 2 (USB_PSN) → Lapis 3 (HW_ID) → Lapis 4 (HMAC-SHA256 signature).
2. WHEN validasi pada salah satu lapis gagal, THE LicenseManager SHALL menghentikan proses dan menandai USB tidak valid tanpa melanjutkan ke lapis berikutnya.
3. WHEN USB_VSN terbaca tidak cocok dengan USB_VSN di License_File, THE LicenseManager SHALL menggagalkan Lapis 1.
4. WHEN USB_PSN terbaca tidak cocok dengan USB_PSN di License_File, THE LicenseManager SHALL menggagalkan Lapis 2.
5. WHEN HW_ID komputer saat ini tidak cocok dengan HW_ID di License_File, THE LicenseManager SHALL menggagalkan Lapis 3.
6. WHEN HMAC-SHA256 signature tidak dapat diverifikasi menggunakan Secret_Key, THE LicenseManager SHALL menggagalkan Lapis 4.
7. THE LicenseManager SHALL menyatakan USB Key valid hanya jika keempat lapis berhasil dilewati.

---

### Requirement 5: Aktivasi Manual (Fallback tanpa USB)

**User Story:** Sebagai pengguna KasirLancar, saya ingin dapat mengaktifkan aplikasi secara manual menggunakan Serial Number dan Activation Key, agar aplikasi tetap dapat digunakan ketika USB Key tidak tersedia.

#### Acceptance Criteria

1. THE Activation_Form SHALL menyediakan kolom input untuk Serial_Number dan Activation_Key.
2. WHEN pengguna mengirimkan Serial_Number dan Activation_Key, THE LicenseManager SHALL memverifikasi dengan menghitung ulang HMAC-SHA256 dari kombinasi Serial_Number + HW_ID menggunakan Secret_Key.
3. WHEN hasil kalkulasi cocok dengan Activation_Key yang diinput, THE LicenseManager SHALL menyatakan aktivasi manual berhasil.
4. IF Activation_Key tidak cocok, THEN THE LicenseManager SHALL menolak aktivasi dan menampilkan pesan untuk menghubungi pengembang.
5. THE LicenseManager SHALL menyimpan data lisensi hasil aktivasi manual ke `license.ini` terenkripsi dan ke License_Registry sebagai backup.
6. WHEN aktivasi manual berhasil, THE LicenseManager SHALL mengikat lisensi ke HW_ID komputer sehingga Activation_Key yang sama tidak valid di komputer lain.

---

### Requirement 6: Penyimpanan Lisensi Terenkripsi dan Backup Registry

**User Story:** Sebagai pengembang, saya ingin data lisensi disimpan terenkripsi dengan backup di registry, agar lisensi tidak dapat dimanipulasi atau dipindahkan secara manual.

#### Acceptance Criteria

1. THE LicenseManager SHALL menyimpan data lisensi ke `license.ini` dalam format terenkripsi AES-256, bukan plain text.
2. THE LicenseManager SHALL menyimpan salinan data lisensi ke License_Registry di path Windows Registry yang tidak mudah ditemukan pengguna awam.
3. WHEN KasirLancar dijalankan, THE LicenseManager SHALL memverifikasi konsistensi antara `license.ini` dan License_Registry.
4. IF data di `license.ini` tidak konsisten dengan License_Registry, THEN THE LicenseManager SHALL menandai kondisi sebagai Anti_Tamper dan meminta aktivasi ulang.
5. THE LicenseManager SHALL menyertakan HMAC-SHA256 checksum dari seluruh isi data lisensi di dalam `license.ini` terenkripsi.
6. IF checksum tidak valid saat dibaca, THEN THE LicenseManager SHALL menolak data dan meminta aktivasi ulang.

---

### Requirement 7: Alur Validasi Berlapis saat Startup

**User Story:** Sebagai pengguna KasirLancar, saya ingin aplikasi memeriksa status lisensi saat startup dengan urutan yang jelas, agar pengalaman penggunaan tetap lancar selama lisensi valid.

#### Acceptance Criteria

1. WHEN KasirLancar dijalankan, THE LicenseManager SHALL memeriksa USB Key valid terlebih dahulu.
2. WHEN USB Key valid terdeteksi, THE LicenseManager SHALL mengizinkan aplikasi berjalan penuh.
3. WHEN tidak ada USB Key valid, THE LicenseManager SHALL memeriksa `license.ini` dan License_Registry.
4. WHEN `license.ini` dan License_Registry valid dan konsisten, THE LicenseManager SHALL mengizinkan aplikasi berjalan penuh.
5. WHEN semua metode validasi gagal, THE LicenseManager SHALL menampilkan Activation_Form dan mencegah akses ke fitur utama.
6. THE LicenseManager SHALL menyelesaikan seluruh proses validasi dalam waktu tidak lebih dari 3 detik.

---

### Requirement 8: Aplikasi Aktivator — Platform Flutter & Backend Supabase

**User Story:** Sebagai pengembang (Adi), saya ingin memiliki aplikasi mobile Aktivator berbasis Flutter yang terhubung ke Supabase, agar saya dapat mengelola data pelanggan dan menerbitkan lisensi kapan saja dari HP saya.

#### Acceptance Criteria

1. THE Aktivator SHALL dibangun menggunakan Flutter sehingga dapat diinstal di perangkat Android maupun iOS milik pengembang.
2. THE Aktivator SHALL menggunakan Supabase sebagai backend database terpusat untuk menyimpan seluruh data pelanggan dan lisensi.
3. THE Aktivator SHALL memerlukan autentikasi login (email + password via Supabase Auth) sebelum dapat digunakan.
4. WHEN pengembang login, THE Aktivator SHALL menampilkan dashboard dengan ringkasan: jumlah pelanggan aktif, jumlah total komputer terdaftar, dan lisensi terbaru.

---

### Requirement 9: Manajemen Data Pelanggan (Multi-Komputer)

**User Story:** Sebagai pengembang, saya ingin mengelola data pelanggan di Aktivator dengan dukungan satu pelanggan memiliki banyak komputer terdaftar, agar saya dapat melacak semua instalasi per pelanggan.

#### Acceptance Criteria

1. THE Aktivator SHALL menyimpan data pelanggan ke tabel `pelanggan` di Supabase dengan field: id, nama, alamat, no_hp, tanggal_daftar, status_aktif.
2. THE Aktivator SHALL menyimpan data komputer ke tabel `komputer` di Supabase dengan field: id, pelanggan_id (foreign key), hw_id, nama_komputer, tanggal_daftar, status_aktif.
3. WHEN pengembang membuka detail pelanggan, THE Aktivator SHALL menampilkan daftar semua komputer yang terdaftar untuk pelanggan tersebut.
4. THE Aktivator SHALL mengizinkan satu pelanggan memiliki lebih dari satu komputer terdaftar tanpa batasan jumlah.
5. THE Aktivator SHALL menyediakan fitur tambah, edit, dan nonaktifkan data pelanggan.
6. THE Aktivator SHALL menyediakan fitur tambah, edit, dan nonaktifkan data komputer per pelanggan.
7. THE Aktivator SHALL menyediakan fitur pencarian pelanggan berdasarkan nama atau nomor HP.

---

### Requirement 10: Penerbitan Lisensi via Aktivator

**User Story:** Sebagai pengembang, saya ingin menerbitkan Activation Key untuk komputer pelanggan melalui Aktivator, agar proses penerbitan lisensi terkontrol, tercatat, dan dapat dilakukan dari HP.

#### Acceptance Criteria

1. THE Aktivator SHALL menyediakan form input HW_ID komputer pelanggan untuk men-generate Activation_Key.
2. THE HMAC_Engine SHALL men-generate Activation_Key dengan menghitung HMAC-SHA256 dari kombinasi Serial_Number + HW_ID menggunakan Secret_Key.
3. THE Aktivator SHALL menyimpan setiap lisensi yang diterbitkan ke tabel `lisensi` di Supabase dengan field: id, komputer_id, serial_number, activation_key, tanggal_terbit, tipe_aktivasi (manual/usb), status.
4. WHEN pengembang men-generate Activation_Key, THE Aktivator SHALL menampilkan key dalam format yang mudah disalin dan dikirim ke pelanggan (misal via WhatsApp).
5. IF HW_ID yang dimasukkan sudah terdaftar di komputer lain milik pelanggan berbeda, THEN THE Aktivator SHALL menampilkan peringatan sebelum menerbitkan lisensi baru.
6. THE Aktivator SHALL menampilkan riwayat seluruh lisensi yang pernah diterbitkan, dapat difilter per pelanggan atau per komputer.
7. THE Aktivator SHALL menyediakan fitur generate License_File terenkripsi untuk aktivasi via USB, yang dapat disimpan ke penyimpanan HP lalu disalin ke USB Key.

---

### Requirement 11: Validasi SN Terdaftar di Sisi Client

**User Story:** Sebagai pengembang, saya ingin Activation_Key hanya valid jika Serial_Number yang digunakan sudah terdaftar dan di-generate oleh Aktivator, agar tidak ada yang bisa membuat key palsu.

#### Acceptance Criteria

1. THE HMAC_Engine SHALL menyematkan Serial_Number ke dalam kalkulasi Activation_Key sehingga key hanya valid untuk SN + HW_ID yang spesifik.
2. WHEN LicenseManager memverifikasi Activation_Key, THE LicenseManager SHALL menghitung ulang HMAC-SHA256 dari Serial_Number + HW_ID komputer saat itu dan membandingkan hasilnya.
3. IF Serial_Number diubah atau dipalsukan, THEN hasil kalkulasi HMAC SHALL berbeda dan validasi SHALL gagal.
4. THE Secret_Key yang digunakan HMAC_Engine di KasirLancar SHALL identik dengan Secret_Key di Aktivator, sehingga key yang di-generate Aktivator dapat diverifikasi oleh KasirLancar secara offline.
5. THE LicenseManager SHALL menolak Activation_Key yang formatnya tidak sesuai tanpa melakukan kalkulasi HMAC.

---

### Requirement 12: Keamanan Algoritma dan Secret Key

**User Story:** Sebagai pengembang, saya ingin algoritma key generation tidak dapat di-reverse tanpa Secret_Key, agar lisensi tidak dapat dipalsukan.

#### Acceptance Criteria

1. THE HMAC_Engine SHALL menggunakan HMAC-SHA256 sehingga tidak dapat di-reverse tanpa Secret_Key.
2. THE LicenseManager SHALL menyimpan Secret_Key menggunakan teknik obfuskasi sehingga tidak dapat diekstrak langsung dari binary aplikasi.
3. THE LicenseManager SHALL tidak pernah menampilkan, mencatat (log), atau mentransmisikan Secret_Key dalam bentuk apapun.
4. WHEN Activation_Key diverifikasi, THE LicenseManager SHALL menggunakan constant-time comparison untuk mencegah timing attack.
5. THE LicenseManager SHALL menggunakan IV (Initialization Vector) yang unik dan acak untuk setiap operasi enkripsi AES-256.

---

### Requirement 13: Deteksi Anti-Tamper

**User Story:** Sebagai pengembang, saya ingin sistem mendeteksi upaya manipulasi pada file atau data lisensi, agar lisensi yang dimodifikasi manual tidak dapat digunakan.

#### Acceptance Criteria

1. WHEN `license.ini` dimodifikasi di luar KasirLancar, THE Anti_Tamper SHALL mendeteksi perubahan melalui validasi HMAC-SHA256 checksum saat startup berikutnya.
2. WHEN data di License_Registry dimodifikasi di luar KasirLancar, THE Anti_Tamper SHALL mendeteksi inkonsistensi saat dibandingkan dengan `license.ini`.
3. IF Anti_Tamper mendeteksi manipulasi, THEN THE LicenseManager SHALL menghapus data lisensi yang terkorupsi, mencatat kejadian ke log, dan menampilkan Activation_Form.
4. THE Anti_Tamper SHALL memverifikasi integritas data lisensi setiap kali KasirLancar dijalankan.

---

### Requirement 14: Penanganan Error dan Pengalaman Pengguna

**User Story:** Sebagai pengguna KasirLancar, saya ingin mendapatkan pesan yang jelas ketika aktivasi gagal, agar saya mengetahui langkah selanjutnya.

#### Acceptance Criteria

1. WHEN validasi USB Key gagal, THE Activation_Form SHALL menampilkan pesan penyebab kegagalan tanpa mengekspos detail teknis kriptografis.
2. WHEN aktivasi manual gagal, THE Activation_Form SHALL menampilkan pesan yang menginstruksikan pengguna menghubungi pengembang beserta HW_ID komputer mereka.
3. THE Activation_Form SHALL menampilkan HW_ID komputer saat ini dalam format yang mudah disalin pengguna.
4. IF terjadi error tidak terduga, THEN THE LicenseManager SHALL mencatat detail ke log dan menampilkan pesan generik tanpa mengekspos stack trace.

---

## Skema Database Supabase (Referensi)

```sql
-- Tabel pelanggan
CREATE TABLE pelanggan (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nama        TEXT NOT NULL,
    alamat      TEXT,
    no_hp       TEXT,
    tgl_daftar  TIMESTAMPTZ DEFAULT NOW(),
    status      TEXT DEFAULT 'aktif'  -- aktif | nonaktif
);

-- Tabel komputer (multi per pelanggan)
CREATE TABLE komputer (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    pelanggan_id    UUID REFERENCES pelanggan(id) ON DELETE CASCADE,
    hw_id           TEXT NOT NULL UNIQUE,
    nama_komputer   TEXT,
    tgl_daftar      TIMESTAMPTZ DEFAULT NOW(),
    status          TEXT DEFAULT 'aktif'
);

-- Tabel lisensi
CREATE TABLE lisensi (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    komputer_id     UUID REFERENCES komputer(id) ON DELETE CASCADE,
    serial_number   TEXT NOT NULL UNIQUE,
    activation_key  TEXT NOT NULL,
    tgl_terbit      TIMESTAMPTZ DEFAULT NOW(),
    tipe_aktivasi   TEXT DEFAULT 'manual',  -- manual | usb
    status          TEXT DEFAULT 'aktif'
);
```

---

## Ringkasan Komponen yang Perlu Dibangun

| Komponen | Platform | Teknologi |
|---|---|---|
| LicenseManager | KasirLancar (existing) | VB.NET, WMI, AES-256, HMAC-SHA256 |
| Activation_Form (update) | KasirLancar (existing) | VB.NET Windows Forms |
| Aktivator | Mobile | Flutter + Supabase |
| HMAC_Engine (shared logic) | Keduanya | Algoritma identik di VB.NET & Dart |
