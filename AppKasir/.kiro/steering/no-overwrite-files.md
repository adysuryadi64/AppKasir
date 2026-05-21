---
inclusion: always
---

# Aturan Kerja Wajib — AppKasir

---

## 1. LARANGAN KERAS — Modifikasi File

- **DILARANG** `fsWrite` pada file yang sudah ada (menulis ulang seluruh isi)
- **DILARANG** `deleteFile` tanpa diminta secara eksplisit oleh user
- **DILARANG** menghapus fungsi/sub/class/blok kode yang tidak diminta diubah
- Jika perubahan sangat besar dan benar-benar perlu tulis ulang → **WAJIB minta persetujuan user dulu**

**Yang wajib dilakukan:**
- `strReplace` untuk ubah bagian spesifik di file yang sudah ada
- `fsAppend` untuk tambah kode baru di akhir file yang sudah ada
- `fsWrite` hanya untuk **membuat file baru** yang belum ada

---

## 2. WAJIB BACA DULU SEBELUM UBAH

Sebelum mengubah apapun, **wajib baca file yang akan diubah terlebih dahulu** menggunakan `readFile` atau `readCode`. Tujuannya:
- Memahami konteks dan struktur yang sudah ada
- Menghindari duplikasi atau konflik dengan kode yang sudah ada
- Memastikan `strReplace` menemukan string yang tepat

---

## 3. ATURAN strReplace — TIDAK BOLEH PAKAI REGEX/POWERSHELL

`strReplace` adalah cara utama untuk mengubah kode di file yang sudah ada. Aturannya:

- **WAJIB** baca file dulu, salin exact string yang akan diganti
- **DILARANG** pakai PowerShell (`$content.Replace(...)`, `Set-Content`) untuk mengganti kode
- **DILARANG** pakai regex pattern untuk mengganti kode
- Jika `strReplace` gagal (string tidak ditemukan):
  1. Baca ulang file dengan `readFile` + `start_line`/`end_line` untuk lihat exact whitespace
  2. Coba lagi dengan string yang lebih tepat
  3. Jika masih gagal 2x → **laporkan ke user, jangan coba cara lain yang berisiko**
- Tidak ada git/version control → kesalahan tidak bisa di-undo → **hati-hati ekstra**

---

## 3b. ATURAN POWERSHELL — WAJIB VERIFIKASI SEBELUM BERTINDAK

PowerShell boleh dipakai untuk **membaca dan menganalisis** file, tapi **TIDAK untuk mengubah** kode.

### Penggunaan yang DIIZINKAN
```powershell
# ✅ Baca dan analisis — aman
Select-String -Path "file.vb" -Pattern "Debug\.WriteLine" | ForEach-Object { "Baris $($_.LineNumber): $($_.Line.Trim())" }
Compare-Object $lama $baru
Get-Content "file.vb" | Measure-Object -Line
```

### Penggunaan yang DILARANG
```powershell
# ❌ DILARANG — mengubah file
$content = Get-Content "file.vb" -Raw
$content = $content -replace "lama", "baru"
Set-Content "file.vb" $content
```

### Aturan WAJIB sebelum hapus/replace massal dengan PowerShell

Jika PowerShell dipakai untuk **mengidentifikasi** sesuatu yang akan dihapus/diubah (misal: daftar `Debug.WriteLine`), **WAJIB lakukan langkah ini sebelum bertindak:**

1. **Tampilkan dulu semua hasil** — lihat nomor baris dan isi baris lengkap
2. **Kategorikan** — mana yang dari sesi ini, mana yang sudah ada sebelumnya
3. **Konfirmasi ke user** jika ada keraguan tentang baris yang tidak familiar
4. **Baru hapus satu per satu** dengan `strReplace` — bukan bulk replace

> **Kasus nyata yang pernah terjadi:** PowerShell menemukan 38 `Debug.WriteLine`. Tanpa verifikasi,
> hampir menghapus semua termasuk debug jurnal penjualan (baris 4285, 5107-5144) yang sudah ada
> sebelumnya dan bukan bagian dari sesi debugging. Selalu cek nomor baris dan konteksnya dulu.

---

## 4. WAJIB CARI REFERENSI DI FILE LAIN

Sebelum menulis kode baru, **wajib cari referensi** di codebase yang sudah ada:
- Gunakan `grepSearch` untuk cari pola serupa yang sudah diimplementasikan
- Gunakan `readCode` untuk baca implementasi yang sudah ada
- Bandingkan dengan VB.NET (`2Trans/`, `Modules/`) jika ada fitur serupa
- Tujuan: konsistensi, tidak reinvent the wheel, tidak bertentangan dengan logika yang ada

> **Urutan wajib sebelum menulis kode:**
> 1. Baca steering yang relevan
> 2. `grepSearch` referensi serupa di codebase
> 3. Baca kode referensi — pahami **fungsinya**, bukan hanya namanya
> 4. Baru tulis kode

---

## 5. WAJIB BERPIKIR KRITIS & VERIFIKASI

Sebelum menyimpulkan atau menulis kode:
- **Jangan menebak** nama kolom, nama tabel, nilai enum, atau logika bisnis
- **Selalu verifikasi ke database** jika menyangkut struktur tabel atau data nyata
- **Selalu baca VB.NET** jika menyangkut logika bisnis yang sudah ada di desktop app
- **Bandingkan data nyata** dari `db_rejeki` (data produksi) vs `db_moroseneng` (development)
- Jika ada keraguan → tanya user, jangan asumsikan

### ⚠️ CATATAN KRITIS — JANGAN PERCAYA NAMA, PERCAYA FUNGSI

> "Ada kata *kambing di sungai makan daging* — padahal aslinya buaya, hanya diberi nama kambing."

**Nama variabel, field, parameter bisa menyesatkan.** Yang wajib ditelusuri:
1. **Apa yang masuk** ke variabel/parameter itu? (sumber datanya dari mana?)
2. **Bagaimana nilainya dihitung?** (baca kode yang mengisi, bukan hanya yang membaca)
3. **Bagaimana nilainya dipakai** di tempat lain? (baca semua consumer-nya)

**Contoh nyata di proyek ini:**
- `diskonRp` namanya terkesan "total diskon dalam rupiah" — ternyata setelah baca `HitungNilaiSetiapBaris` di `FormPenjualan.vb`, nilainya adalah **diskon per satuan**, dan `totalDiskon = qty * diskonRp`
- Jika langsung percaya nama tanpa baca fungsinya → kalkulasi salah

**Wajib lakukan sebelum pakai variabel/field yang belum dikenal:**
- `grepSearch` nama field tersebut di seluruh codebase
- Baca semua tempat field itu **diisi** (bukan hanya dibaca)
- Verifikasi dengan data nyata di database jika perlu

---

## 6. WAJIB CARI INFORMASI INTERNET JIKA DIPERLUKAN

Untuk teknologi, library, atau API yang mungkin sudah berubah:
- Gunakan `remote_web_search` untuk cari versi terbaru, breaking changes, atau best practice
- Prioritaskan dokumentasi resmi
- Jangan andalkan pengetahuan lama jika menyangkut versi spesifik Flutter, MySQL, PHP

---

## 7. ATURAN KODING — AppKasir (Visual Basic .NET)

**Bahasa:** Semua komentar, pesan log, dan pesan error ke user **wajib Bahasa Indonesia**. Nama variabel dan fungsi boleh campuran Indonesia-Inggris asal deskriptif dan konsisten dengan kode yang sudah ada.

**Komentar:**
- Setiap `Function` dan `Sub` wajib diberi komentar di atasnya
- Gunakan `' ── Judul ──` sebagai pemisah bagian
- Komentar inline untuk logika yang tidak langsung jelas

```vb
' ── Hitung total harga setelah diskon ──────────────────────────
' Menerima harga satuan dan persen diskon, mengembalikan harga akhir
Private Function HitungHargaDiskon(harga As Decimal, diskon As Decimal) As Decimal
    If diskon > 100 Then diskon = 100
    Return harga - (harga * diskon / 100)
End Function
```

**Struktur form:** Load → Setup → Event Handler → Fungsi Kalkulasi → Fungsi DB → Helper

**Error handling:** Selalu `Try/Catch` untuk operasi DB dan file I/O. Pesan error harus informatif.

**Minimal code:** Tulis hanya yang diperlukan. Jangan tambah fungsi/property/import yang tidak diminta.

---

## 8. ATURAN DATABASE & COA

- **Jangan hardcode** kode akun COA yang bersumber dari `tbl_perusahaan` (lihat steering `coa-tbl-datareferensi.md`)
- Akun yang boleh hardcode di SP: `05.02.001`, `05.04.001`, `06.01.001`, `06.04.001`, `03.02.001`, `08.01.002`
- Selalu verifikasi struktur tabel ke database sebelum menulis query atau SP
- Selalu bandingkan dengan data nyata di `db_moroseneng` untuk memastikan nilai enum, format, dan logika benar
- Filter dropdown akun: **jangan pakai** `STATUS = 'Aktif'` — tidak ada nilai tersebut di database

---

## 9. TIDAK ADA GIT — ZERO TOLERANCE UNTUK KESALAHAN DESTRUKTIF

Proyek ini **tidak menggunakan git**. Tidak ada cara undo jika file rusak.

Konsekuensinya:
- Setiap perubahan harus dipastikan benar sebelum dieksekusi
- Lebih baik lambat dan benar daripada cepat dan merusak
- Jika tidak yakin → tanya user terlebih dahulu
- Jika `strReplace` gagal → **STOP, laporkan ke user, jangan coba workaround berisiko**

---

## 10. ANALISIS DAMPAK — SATU PERUBAHAN BISA MEMPENGARUHI BANYAK FILE

Setiap kali diminta mengubah sesuatu, **wajib analisis dampak ke file lain** sebelum mulai:

- Jika ubah **provider/state** → cek semua screen yang consume provider tersebut
- Jika ubah **API endpoint/payload** → cek PHP, SP, dan semua screen yang memanggil API tersebut
- Jika ubah **widget/komponen** → cek semua tempat widget itu dipakai
- Jika ubah **SP/query DB** → cek PHP yang memanggil SP dan semua kolom yang terlibat
- Jika ubah **nama parameter** → cek semua caller, jangan hanya ubah definisi

**Cara kerja:**
1. Gunakan `grepSearch` untuk cari semua referensi sebelum ubah
2. Buat daftar file yang terdampak
3. Update semua file yang terdampak dalam satu sesi — jangan setengah-setengah
4. Cek diagnostics setelah semua perubahan selesai

### ⚠️ WAJIB — Cari Pola Masalah yang Sama di File yang Sama

Saat memperbaiki satu kesalahan, **wajib cari apakah pola masalah yang sama ada di tempat lain** dalam file yang sama sebelum selesai:

- Jika memperbaiki nama kolom DGV yang salah (`TotalHarga` → `Totalharga`) → `grepSearch` nama kolom salah itu di seluruh file, perbaiki semua sekaligus
- Jika memperbaiki `ParseDecimal` yang seharusnya `ParseInteger` → cari semua `ParseDecimal` untuk kolom yang sama di file yang sama
- Jika memperbaiki format TextBox yang salah → cari semua TextBox serupa yang mungkin punya masalah sama
- Jika memperbaiki guard `IsDBNull` yang hilang → cari semua pembacaan kolom yang sama tanpa guard

**Cara kerja:**
```powershell
# Contoh: setelah perbaiki nama kolom, cari sisa yang masih salah
Select-String -Path "2Trans/FormNama.vb" -Pattern 'Cells\("NamaKolomSalah"\)'
```

> **Prinsip:** Satu bug yang ditemukan biasanya punya saudara kembar di tempat lain.
> Jangan tunggu user menemukan sendiri — cari dan perbaiki sekaligus dalam satu sesi.

---

## 11. ATURAN UI/UX — FLUTTER (WAJIB DIPATUHI KETAT)

### Prinsip Utama
- UI **wajib rapi, konsisten, dan profesional** — ini aplikasi kasir bisnis, bukan prototype
- Setiap screen harus bisa dipakai dengan nyaman di layar HP ukuran 5–6.5 inch
- Semua teks label, tombol, dan pesan **wajib Bahasa Indonesia**

### Konsistensi Visual
- **Wajib** gunakan warna, font size, padding, dan border radius yang sama dengan screen lain yang sudah ada
- Sebelum buat widget baru, **baca dulu screen lain** untuk tahu pola yang sudah dipakai
- Jangan campur gaya — jika screen lain pakai `Card` dengan `elevation: 2`, ikuti pola yang sama
- Gunakan konstanta warna yang sudah ada (misal `_green`, `Colors.grey.shade300`) — jangan buat warna baru sembarangan

### Layout & Spacing
- Padding konsisten: gunakan nilai yang sama dengan screen lain (biasanya `16` atau `12`)
- Jangan biarkan widget terlalu rapat atau terlalu renggang dibanding elemen sekitarnya
- Gunakan `IntrinsicHeight` atau `CrossAxisAlignment.stretch` agar baris sejajar tingginya
- Hindari overflow: selalu test apakah konten muat di layar kecil

### TextField & Input
- Semua `TextField` input angka: `keyboardType: TextInputType.number`
- Label di atas field, bukan di dalam (kecuali `hintText` untuk placeholder)
- `isDense: true` dan `contentPadding` konsisten dengan field lain di form yang sama
- Jangan format angka dengan separator ribuan di TextBox yang dibaca logika (ikuti `standar-input-angka.md`)

### Tombol & Aksi
- Tombol aksi utama (Simpan, Lanjut) selalu di bawah layar, full width atau prominent
- Warna tombol konsisten: aksi positif = hijau/primary, aksi destruktif = merah, aksi sekunder = abu
- Tombol disabled harus terlihat jelas berbeda (opacity atau warna berbeda)
- Setiap tombol yang trigger operasi async **wajib** ada loading state — jangan biarkan user tap berkali-kali

### Feedback & State
- Selalu tampilkan loading indicator saat fetch data atau simpan
- Tampilkan pesan error yang informatif, bukan hanya "Terjadi kesalahan"
- Gunakan `SnackBar` untuk notifikasi singkat, `Dialog` untuk konfirmasi destruktif
- Empty state (data kosong) harus ditampilkan dengan pesan yang jelas, bukan layar kosong

### Responsivitas
- Gunakan `Expanded`, `Flexible`, atau `LayoutBuilder` — hindari lebar/tinggi hardcode dalam pixel
- Teks panjang harus bisa wrap atau overflow dengan `TextOverflow.ellipsis`
- Scroll harus berfungsi jika konten melebihi layar

### Perubahan UI — Aturan Khusus
- Jika diminta ubah **satu elemen UI**, cek apakah elemen serupa di screen/widget lain perlu disamakan
- Jika ubah **widget yang dipakai di banyak tempat**, update semua pemanggilnya
- Jangan ubah ukuran/warna/spacing satu elemen tanpa mempertimbangkan keselarasan dengan elemen di sekitarnya
- Setelah ubah UI, **bayangkan tampilan akhirnya** — apakah rapi? apakah konsisten? apakah ada yang janggal?

---

## 12. CHECKLIST SEBELUM SELESAI

Sebelum menyatakan pekerjaan selesai, pastikan:

- [ ] Semua file yang terdampak sudah diupdate (bukan hanya file yang diminta)
- [ ] Tidak ada kode yang dihapus tanpa disengaja
- [ ] UI konsisten dengan screen lain yang sudah ada
- [ ] Tidak ada hardcode yang seharusnya dinamis (akun COA, warna, string label)
- [ ] Semua teks user-facing dalam Bahasa Indonesia
- [ ] Tidak ada `strReplace` yang gagal dibiarkan begitu saja

---

## 13. ATURAN KOMUNIKASI KE USER

- Jika menemukan bug atau inkonsistensi **di luar scope permintaan** → laporkan, tapi jangan ubah tanpa izin
- Jika ada dua cara untuk menyelesaikan sesuatu → jelaskan trade-off, minta keputusan user
- Jika permintaan ambigu → tanya dulu, jangan asumsikan
- Ringkasan akhir: singkat dan padat — sebutkan apa yang diubah dan file mana saja, tanpa bertele-tele
- Jangan ulangi hal yang sudah jelas dari konteks percakapan
