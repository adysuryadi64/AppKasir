# Tasks — Migrasi Business Logic ke MySQL Stored Procedures

> **Versi**: 1.0.0  
> **Tanggal**: 2026-04-19  
> **Status**: In Progress  
> **Referensi**: requirements.md, design.md, design_part2.md  

---

## Legenda Status

- ✅ Selesai
- 🔄 Sedang dikerjakan
- ⬜ Belum dimulai

---

## Fase 1 — Helper SP & Batch SP

> **Status: ✅ SELESAI**  
> File: `Database/06_migrasi_stored_procedures.sql`  
> Tidak ada perubahan klien di fase ini.

### Task 1.1 — Implementasi Helper SP ✅

**File:** `Database/06_migrasi_stored_procedures.sql`

- [x] `sp_hlp_stok_hitung` — Recalculate STOK_TOKO & STOK_GUDANG dari semua counter (migrasi `HitungStokPerubahan`)
- [x] `sp_hlp_stok_validasi` — Cek stok cukup dengan `SELECT ... FOR UPDATE`
- [x] `sp_hlp_faktur_generate` — Generate nomor faktur unik format `{PREFIX}-{YYMMDD}{XXXX}`, aman multi-user
- [x] `INSERT INTO JurnalUmum` (inline) — INSERT satu baris ke `JurnalUmum`
- [x] `sp_hlp_saldo_akun_update` — Recalculate `Saldo_Akhir` satu akun dengan `CASE WHEN AKUN_DK` (rumus benar)
- [x] `sp_hlp_saldo_kas_validasi` — Cek saldo kas cukup dengan `SELECT ... FOR UPDATE`

**Verifikasi:**
- [x] Semua 6 Helper SP terbuat tanpa error
- [x] `sp_hlp_stok_hitung` menggunakan `COALESCE` untuk semua kolom counter
- [x] `sp_hlp_saldo_akun_update` menggunakan `CASE WHEN AKUN_DK` (berbeda dari VB lama yang bug)

---

### Task 1.2 — Implementasi Batch SP ✅

**File:** `Database/06_migrasi_stored_procedures.sql`

- [x] `sp_bat_stok_semua_barang` — Recalculate stok semua barang (migrasi `HitungSemuaKode`)
- [x] `sp_bat_stok_toko` — Recalculate STOK_TOKO saja (migrasi `HitungStokToko`)
- [x] `sp_bat_stok_gudang` — Recalculate STOK_GUDANG saja (migrasi `HitungStokGudang`)
- [x] `sp_bat_saldo_semua_akun` — Recalculate Saldo_Akhir semua akun dengan `CASE WHEN AKUN_DK`
- [x] `sp_bat_piutang_semua_pelanggan` — Recalculate HutangAkhir semua pelanggan dari `penjualan`
- [x] `sp_bat_hutang_semua_supplier` — Recalculate HutangAkhir semua supplier dari `pembelian`
- [x] `sp_bat_bon_semua_karyawan` — Recalculate SaldoAkhir semua karyawan dari `bon_karyawan`

**Verifikasi:**
- [x] Semua 7 Batch SP terbuat tanpa error
- [x] `sp_bat_saldo_semua_akun` menggunakan `CASE WHEN AKUN_DK` (perbaikan dari `UpdateSaldoSemuaAkun` lama)

---

### Task 1.3 — Skeleton SP Transaksi — Tidak diimplementasikan — simpan tetap inline di klien

> Simpan dan hapus transaksi tetap dilakukan di sisi klien (VB.NET inline, PHP inline).
> Validasi server (`sp_hlp_stok_validasi`, `sp_hlp_saldo_kas_validasi`) tetap dipakai oleh PHP.

---

### Task 1.4 — Migrasi Generate Nomor ke `sp_hlp_faktur_generate` di Semua Form ✅

**Referensi:** `requirements.md` Req 2 (Generate Nomor Transaksi yang Aman Multi-User)

> **Latar belakang:** Ini adalah alasan pertama migrasi dimulai — semua generate nomor transaksi
> yang pakai `SELECT MAX` inline rawan race condition multi-user. `sp_hlp_faktur_generate` sudah
> tersedia sejak Fase 1, jadi migrasi ini bisa dikerjakan segera tanpa menunggu fase lain.
>
> **Pola migrasi standar** (sama untuk semua form):
> ```vb
> Using cmd As New MySqlCommand(
>     "CALL sp_hlp_faktur_generate(@prefix, @tgl, @tabel, @kolom, @nomor)", conn)
>     cmd.Parameters.AddWithValue("@prefix", "XX")
>     cmd.Parameters.AddWithValue("@tgl", DTPTgl.Value.Date)
>     cmd.Parameters.AddWithValue("@tabel", "nama_tabel")
>     cmd.Parameters.AddWithValue("@kolom", "NAMA_KOLOM_PK")
>     Dim pNomor = cmd.Parameters.Add("@nomor", MySqlDbType.VarChar, 30)
>     pNomor.Direction = ParameterDirection.Output
>     cmd.ExecuteNonQuery()
>     LblNomor.Text = pNomor.Value?.ToString()
> End Using
> ```

**Form yang perlu dimigrasikan:**

| Form | Prefix | Tabel | Kolom PK | Fungsi | Status |
|------|--------|-------|----------|--------|--------|
| `2Trans/FormPenjualan.vb` | `PJ` | `penjualan` | `ID_PENJUALAN` | `Nomorjual()` | ✅ Selesai (Task 2.3) |
| `2Trans/FormPembelian.vb` | `PB` | `pembelian` | `ID_PEMBELIAN` | `NomorBeli()` | ⬜ |
| `2Trans/FormReturPenjualan.vb` | `RP` | `retur_penjualan` | `ID_RETUR_PENJUALAN` | `GenerateNomor()` | ⬜ |
| `2Trans/FormReturPembelian.vb` | `RB` | `retur_pembelian` | `ID_RETUR_PEMBELIAN` | `GenerateNomor()` | ⬜ |
| `2Trans/FormReturBeli.vb` | `RB` | `retur_pembelian` | `ID_RETUR_PEMBELIAN` | inline di simpan | ⬜ |
| `2Trans/FormStokOpname.vb` | `SO` | `stok_opname` | `ID_STOK_OPNAME` | `GenerateNomor()` | ⬜ |
| `2Trans/FormTransferStok.vb` | `TS` | `transfer_stok` | `ID_TRANSFER` | `GenerateNomor()` | ⬜ |
| `2Trans/FormTransferBarang.vb` | `TB` | `transfer_barang` | `ID_TRANSFER` | `GenerateNomor()` | ⬜ |
| `2Trans/FormBayarHutang.vb` | `BH` | `hutang` | `NOBAYARHUTANG` | `GenerateNomor()` | ⬜ |
| `2Trans/FormBayarPiutang.vb` | `BP` | `piutang` | `ID_BAYAR_PIUTANG` | `GenerateNomor()` | ⬜ |
| `2Trans/FormSuratJalan.vb` | `SJ` | `surat_jalan` | `NOTA` | `GenerateNomor()` | ⬜ |
| `4Gaji/FormGaji.vb` | `GJ` | `gaji_karyawan` | `NOMOR` | `GenerateNomor()` | ⬜ |
| `4Gaji/FormBon.vb` | `BK` | `bon_karyawan` | `FAKTUR` | `GenerateNomorBon()` | ⬜ |

- [x] `FormPenjualan.vb` — `Nomorjual()` sudah dimigrasikan (Task 2.3)
- [x] `FormPembelian.vb` — `NomorBeli()` → `sp_hlp_faktur_generate('PB', ...)`
- [x] `FormReturPenjualan.vb` — `GenerateNomorReturPenjualan()` → `sp_hlp_faktur_generate('RP', ...)`
- [x] `FormReturPembelian.vb` — `GenerateNomorReturPembelian()` → `sp_hlp_faktur_generate('RB', ...)`
- [x] `FormReturBeli.vb` — `NomorRetur()` → `sp_hlp_faktur_generate('RB', ...)`
- [x] `FormStokOpname.vb` — `GenerateNomorOpname()` → `sp_hlp_faktur_generate('SO', ...)`
- [x] `FormTransferStok.vb` — `GenerateNomorTransferstok()` → `sp_hlp_faktur_generate('TS', ...)`
- [x] `FormTransferBarang.vb` — `NomorTransfer()` → `sp_hlp_faktur_generate('TB', ...)`
- [x] `FormBayarHutang.vb` — `GenerateNomorBayarHutang()` → `sp_hlp_faktur_generate('BH', ...)`
- [x] `FormBayarPiutang.vb` — `GenerateNomorBayarHutang()` → `sp_hlp_faktur_generate('BP', ...)`
- [x] `FormSuratJalan.vb` — `GenerateNomorSuratJalan()` → `sp_hlp_faktur_generate('SJ', ...)`
- [x] `FormGaji.vb` — `GenerateNomorGaji()` → `sp_hlp_faktur_generate('GJ', ...)`
- [x] `FormBon.vb` — `GenerateNomorBon()` → `sp_hlp_faktur_generate('BK', ...)`

---

### Task 1.5 — Terapkan Helper DTP Backdate & Property Setting Global ke Semua Form Transaksi ✅

**Referensi:** `requirements.md` Req 16b

> **Dua hal sekaligus:**
> 1. DTP backdate — `TerapkanModeDTP`, `ResetDTPKeTanggalHariIni`, `ValidasiTanggalTransaksi`
> 2. Setting global — ganti `BacaSettingDariCache(FormGeneralSetting.LblXxx.Text)` dengan property `ModulHakAkses.SettingXxx`
>
> **Tipe data property (sudah diimplementasikan di `ModulHakAkses.vb`):**
> - `"Iya"`/`"Tidak"` → **Boolean** — langsung pakai `If ModulHakAkses.SettingXxx Then`
> - Multi-nilai → **String** — `SettingMetodeUpdateHargaBeli`, `SettingAverageHargaBerdasarkanStok`
>
> **Property Boolean yang tersedia:**
> ```vb
> ' Global (Boolean)
> ModulHakAkses.SettingIzinkanTanggalLampau    ' True/False
> ModulHakAkses.SettingIzinkanBarangMinus       ' True/False
> ModulHakAkses.SettingFokusOtomatis            ' True/False
> ModulHakAkses.SettingIzinkanSatuanBerbeda     ' True/False
> ModulHakAkses.SettingLangsungIsiNominalTotal  ' True/False
> ModulHakAkses.SettingTampilInfoStok           ' True/False
> ModulHakAkses.SettingNavigasiSetelahCariNama  ' True/False
> ' Penjualan (Boolean)
> ModulHakAkses.SettingIzinkanUbahHargaJual
> ModulHakAkses.SettingIzinkanJualRugi
> ModulHakAkses.SettingIzinkanNominalJualNol
> ModulHakAkses.SettingHargaJualOtomatisUpdateMaster
> ModulHakAkses.SettingIzinkanDiskonItem
> ' Pembelian (Boolean)
> ModulHakAkses.SettingIzinkanUbahHargaBeli
> ModulHakAkses.SettingBeliOtomatisUpdateHargaJual
> ModulHakAkses.SettingIzinkanBeliTanpaSupplier
> ModulHakAkses.SettingIzinkanNominalBeliNol
> ModulHakAkses.SettingIzinkanBeliRugi
> ' Pembelian (String — multi-nilai)
> ModulHakAkses.SettingMetodeUpdateHargaBeli    ' "Harga Terbaru" / "Metode Average..." / "Tidak Ada"
> ModulHakAkses.SettingAverageHargaBerdasarkanStok  ' "Toko dan Gudang" / dll
> ' Retur (Boolean)
> ModulHakAkses.SettingWajibAlasanReturBeli
> ModulHakAkses.SettingWajibAlasanReturJual
> ```
>
> **Pola migrasi di form:**
> ```vb
> ' Sebelum (panjang, String):
> SettingIzinkanJualStokMinus = ModulHakAkses.BacaSettingDariCache(FormGeneralSetting.LblGlobalBarangMinus.Text)
> If SettingIzinkanJualStokMinus = "Iya" Then ...
>
> ' Sesudah (bersih, Boolean):
> If ModulHakAkses.SettingIzinkanBarangMinus Then ...
>
> ' Kirim ke SP (Integer):
> Dim izinkanStokMinus As Integer = If(ModulHakAkses.SettingIzinkanBarangMinus, 1, 0)
> ```

| Form | DTP | Status |
|------|-----|--------|
| `2Trans/FormPenjualan.vb` | `DTPTgl` | ✅ |
| `2Trans/FormPembelian.vb` | `DTPTgl` | ✅ |
| `2Trans/FormReturPenjualan.vb` | `DTPRetur` | ✅ |
| `2Trans/FormReturBeli.vb` | `DTPTgl` | ✅ |
| `2Trans/FormReturPembelian.vb` | `DTPRetur` | ✅ |
| `2Trans/FormStokOpname.vb` | `DTPTgl` | ✅ |
| `2Trans/FormTransferStok.vb` | `DtpTanggal` | ✅ |
| `2Trans/FormTransferBarang.vb` | `DTPTgl` | ✅ |
| `2Trans/FormBayarHutang.vb` | `DtpTanggal` | ✅ |
| `2Trans/FormBayarPiutang.vb` | `DtpTanggal` | ✅ |
| `2Trans/FormSuratJalan.vb` | `DtpSuratJalan` | ✅ |
| `4Gaji/FormGaji.vb` | `DtpTanggal` | ✅ |
| `4Gaji/FormBon.vb` | `DtpTanggal` | ✅ |
| `3Jurnal/FormKeuangan.vb` | `DTPTglKeuangan` | ✅ |

- [x] `ModulHakAkses.vb` — semua property setting global sudah ditambahkan (Boolean untuk Iya/Tidak, String untuk multi-nilai)
- [x] `FormPenjualan.vb` — DTP sudah dimigrasikan
- [x] `FormPenjualan.vb` — ganti `BacaSettingDariCache(...)` dengan `ModulHakAkses.SettingXxx` (Boolean)
- [x] `FormPembelian.vb` — DTP + setting
- [x] `FormReturPenjualan.vb` — DTP + setting
- [x] `FormReturBeli.vb` — DTP + setting
- [x] `FormReturPembelian.vb` — DTP + setting
- [x] `FormStokOpname.vb` — DTP + setting
- [x] `FormTransferStok.vb` — DTP + setting
- [x] `FormTransferBarang.vb` — DTP + setting
- [x] `FormBayarHutang.vb` — DTP + setting
- [x] `FormBayarPiutang.vb` — DTP + setting
- [x] `FormSuratJalan.vb` — DTP + setting
- [x] `FormGaji.vb` — DTP + setting
- [x] `FormBon.vb` — DTP + setting
- [x] `FormKeuangan.vb` — DTP + setting

---

## Fase 2 — Penjualan

> **Status: ✅ SELESAI**
>
> **Arsitektur:** Simpan dan hapus tetap di klien (VB.NET inline, PHP inline).
> PHP (`sync_penjualan.php`) melakukan INSERT inline dengan validasi server via SP helper.

### Task 2.1 — Simpan Penjualan — Tidak diimplementasikan — simpan tetap inline di klien

### Task 2.2 — Hapus Penjualan — Tidak diimplementasikan — simpan tetap inline di klien

---

### Task 2.3 — Update `FormPenjualan.vb` ✅

**File:** `2Trans/FormPenjualan.vb`

> **Scope:** Hanya generate nomor via SP + DTP backdate. Logika `Prosessimpan()` dan
> `Hapuspenjualan()` tetap inline — **tidak** dipindah ke SP.

- [x] Ganti `Nomorjual()` — `SELECT MAX` inline → `sp_hlp_faktur_generate('PJ', ...)`
- [x] DTP backdate — `TerapkanModeDTP`, `ResetDTPKeTanggalHariIni`, `ValidasiTanggalTransaksi`
- [x] Ganti `BacaSettingDariCache(...)` dengan `ModulHakAkses.SettingXxx` (Boolean)
- [x] `BuatTabelSementaraPenjualan()` dan `IsiItemPenjualan()` — tersedia sebagai helper untuk Flutter/PHP jika dibutuhkan di masa depan, tapi **tidak dipanggil** dari `Prosessimpan()`
- [x] `Hapuspenjualan()` di `FormUtama.vb` — tetap inline, memanggil `Hapusuntukedit()` dari `FormPenjualan`

---

### Task 2.4 — Refactor `sync_penjualan.php` ✅

**File:** `AppAndroid/api/sync_penjualan.php`

> **Arsitektur:** PHP melakukan INSERT inline (sama seperti VB.NET).
> Validasi wajib dilakukan via SP helper sebelum INSERT agar akurat dan konsisten.

- [x] Hapus logika generate nomor faktur inline (`SELECT MAX`) → ganti dengan `CALL sp_hlp_faktur_generate`
- [x] Tambahkan `CALL sp_hlp_stok_validasi` per item sebelum INSERT — validasi stok di server
- [x] INSERT header ke `penjualan`, detail ke `penjualan_detail`, `HistoryBarang` secara inline dalam satu transaksi PHP (`$conn->beginTransaction()`)
- [x] UPDATE counter `PENJUALAN_TOKO/GUDANG` per item, lalu `CALL sp_hlp_stok_hitung` per item
- [x] INSERT jurnal ke `JurnalUmum` inline, lalu `CALL sp_hlp_saldo_akun_update` per akun
- [x] UPDATE `tbl_pelanggan.HUTANGAKHIR`
- [x] Hapus draft dari `penjualan_ditahan` jika `id_draft` tidak kosong
- [x] Kembalikan response JSON standar: `{"status": "success", "id_penjualan": "..."}` atau `{"status": "error", "error_code": "...", "message": "..."}`
- [x] Terima `izinkan_stok_minus` dan `izinkan_backdate` dari payload Flutter — kirim ke `sp_hlp_stok_validasi`

---

### Task 2.5 — Verifikasi Fase 2 ⬜

- [ ] Test penjualan tunai dari VB.NET: stok berkurang, jurnal seimbang, saldo akun terupdate
- [ ] Test penjualan kredit dari VB.NET: `SISA_TAGIHAN > 0`, piutang pelanggan terupdate
- [ ] Test penjualan dari Flutter (via `sync_penjualan.php`): hasil identik dengan VB.NET
- [ ] Test stok kurang dari Flutter: `sp_hlp_stok_validasi` menolak, tidak ada INSERT
- [ ] Test backdate dari Flutter: validasi backdate di PHP menolak jika tidak diizinkan
- [ ] Test duplikat faktur dari Flutter: PHP mendeteksi dan mengembalikan error
- [ ] Verifikasi `STOK_TOKO/GUDANG` identik antara VB.NET dan Flutter untuk transaksi yang sama
- [ ] Verifikasi `Saldo_Akhir` per akun benar setelah transaksi dari kedua klien

---

## Fase 3 — Pembelian

> **Status: ⬜ BELUM DIMULAI**
>
> **Aturan:** VB.NET tetap pakai logika inline. Task 3.2 (update FormPembelian) **hanya**
> mencakup: generate nomor via SP + DTP backdate. Logika `SimpanTransaksi()` tetap inline.
>
> Prasyarat: Fase 2 selesai dan diverifikasi.

### Task 3.0 — DDL Migration untuk Kolom Baru Pembelian ✅

**File:** `Database/01_migrasi_kolom.sql` (tambahkan di akhir file, sebelum baris terakhir)

> **Catatan:** File `01_migrasi_kolom.sql` sudah ada dan berisi 1157 baris DDL. Task ini hanya menambahkan kolom baru yang diperlukan untuk fitur diskon/PPN/biaya tambahan di pembelian.

- [x] Tambahkan kolom `DISKON_SUPPLIER DECIMAL(15,2) DEFAULT 0` ke tabel `pembelian`
- [x] Tambahkan kolom `PPN_MASUKAN DECIMAL(15,2) DEFAULT 0` ke tabel `pembelian`
- [x] Tambahkan kolom `BIAYA_KIRIM DECIMAL(15,2) DEFAULT 0` ke tabel `pembelian`
- [x] Tambahkan kolom `BIAYA_LAIN DECIMAL(15,2) DEFAULT 0` ke tabel `pembelian`
- [x] Tambahkan kolom `KODE_AKUN_BIAYA_LAIN VARCHAR(20) DEFAULT ''` ke tabel `pembelian`
- [x] Tambahkan kolom `NAMA_AKUN_BIAYA_LAIN VARCHAR(50) DEFAULT ''` ke tabel `pembelian`
- [x] Tambahkan kolom `STATUS_TRANSAKSI_BELI VARCHAR(20) DEFAULT 'LUNAS'` ke tabel `pembelian`
- [x] Tambahkan kolom `HARGA_AVERAGE DECIMAL(15,2) DEFAULT 0` ke tabel `pembelian_detail`
- [x] Tambahkan kolom `HARGA_BELI_SEBELUMNYA DECIMAL(15,2) DEFAULT 0` ke tabel `pembelian_detail`
- [x] Gunakan pola idempotent (cek `INFORMATION_SCHEMA.COLUMNS` seperti kolom lain di file ini)

**Verifikasi:**
- [x] Jalankan `01_migrasi_kolom.sql` → tidak ada error
- [x] Jalankan kedua kali → semua kolom dilewati (sudah ada)
- [x] Cek `DESC pembelian` → semua kolom baru ada

> **Script test tersedia:** `Tests/Test-Task30-DDL.sql`
> ```
> Get-Content "Tests/Test-Task30-DDL.sql" | & "C:\AppServ\MySQL\bin\mysql.exe" -u root -p12345678 db_kasirlancar
> ```
> Hasil: semua 6 test **OK / PASS** ✅
> - 7 kolom baru `pembelian` ada ✅
> - 2 kolom baru `pembelian_detail` ada ✅
> - Tipe data benar ✅
> - Default value benar ✅
> - Idempotent (run kedua skip semua) ✅
> - Smoke test INSERT siap ✅

---

### Task 3.1 — Simpan Pembelian — Tidak diimplementasikan — simpan tetap inline di klien

---

### Task 3.2 — Update `FormPembelian.vb` ✅

**File:** `2Trans/FormPembelian.vb`

> **Scope:** Hanya generate nomor via SP + DTP backdate + setting global. Logika `SimpanTransaksi()`
> tetap inline — **tidak** dipindah ke SP.

- [x] Ganti `NomorBeli()` — `SELECT MAX` inline → `sp_hlp_faktur_generate('PB', ...)`
- [x] DTP backdate — `TerapkanModeDTP`, `ResetDTPKeTanggalHariIni`
- [x] Ganti `BacaSettingDariCache(...)` dengan `ModulHakAkses.SettingXxx` (Boolean)
- [x] `SimpanTransaksi()` tetap inline — `SimpanPembelian` + `SimpanPembelianDetail` + `HistoryBarang` + `Simpanjurnal`
- [x] `TxtBayarTransfer_TextChanged` — recalculate sisa saat transfer berubah
- [x] `HitungUlangKembali()` — gunakan `bayarTunai + bayarTransfer`

---

### Task 3.2b — Update `FormLapMutasiKeuangan.vb` — Tambah Pembelian Transfer ✅ SELESAI

**File:** `6Print/CetakLaporanKas/FormLapMutasiKeuangan.vb`

> **Latar belakang:** Setelah Fase 3, pembelian bisa dibayar sebagian via transfer bank.
> Jurnal pembelian transfer: D PERSEDIAAN / K BANK (bukan K KAS).
> Laporan mutasi kas/bank harus bisa menampilkan pembelian yang melibatkan rekening BANK.
>
> **Kondisi saat ini:** `BtnBeli_Click` query ke tabel `pembelian` dengan filter `JENIS_BAYAR = @jenisBayar`
> (nama rekening). Ini sudah benar untuk KAS. Untuk BANK, perlu tambah filter `KODE_AKUN_TF = @kodeAkun`
> agar pembelian transfer juga muncul saat user memilih rekening BANK.

- [x] Di `BtnBeli_Click`: pisahkan query menjadi dua — tunai (JENIS_BAYAR = namaRekening) vs transfer (KODE_AKUN_TF = kodeAkun) berdasarkan `TxtTypeAkun = "BANK"`. Sudah diimplementasikan dengan flag `isBank`.
- [x] Tambahkan kolom `JENIS_BAYAR_LABEL` di DGV untuk membedakan "Tunai" vs "Transfer". Sudah ada di query SELECT.
- [x] Update `LoadRekapSekaliBaca`: filter `NOMOR_AKUN_K=@AKUN` sudah benar — saat user pilih rekening BANK, pembelian transfer otomatis tertangkap. Tidak ada perubahan yang diperlukan.

**Verifikasi Task 3.2b:**
- [ ] Pilih rekening KAS → pembelian tunai muncul di DGV dan total
- [ ] Pilih rekening BANK → pembelian transfer muncul di DGV dan total
- [ ] Saldo akhir BANK berkurang sesuai total pembelian transfer

---

### Task 3.3 — Verifikasi Fase 3 ⬜

- [ ] Test pembelian tunai → stok bertambah, jurnal seimbang, hutang supplier terupdate
- [ ] Test pembelian kredit → `TAGIHAN > 0`, hutang supplier terupdate
- [ ] Test backdate ditolak oleh DTP (setting tidak izinkan)
- [ ] Verifikasi `PEMBELIAN_TOKO/GUDANG` counter terupdate
- [ ] Test split bayar tunai + transfer: jurnal D PERSEDIAAN, K KAS, K BANK seimbang
- [ ] Test diskon supplier, PPN masukan, biaya kirim: jurnal seimbang
- [ ] Test metode "Harga Terbaru", "Average", "Tidak Ada"

---

### Task 3.4 — Jurnal Penyesuaian Harga Pokok — Tidak diimplementasikan — simpan tetap inline di klien

---
---

## Fase 4 — Retur & Opname

> **Status: ⬜ BELUM DIMULAI**
>
> **Arsitektur:** Simpan dan hapus tetap di klien (VB.NET inline, PHP inline untuk stok opname).
> PHP (`sync_stokopname.php`) melakukan INSERT inline dengan validasi server via SP helper.
> Form VB.NET hanya perlu: generate nomor via SP + DTP backdate (sudah selesai di Task 1.4 & 1.5).
>
> Prasyarat: Fase 3 selesai dan diverifikasi.

### Task 4.1 — Simpan Retur Jual — Tidak diimplementasikan — simpan tetap inline di klien
### Task 4.2 — Simpan Retur Beli — Tidak diimplementasikan — simpan tetap inline di klien
### Task 4.3 — Simpan Opname — Tidak diimplementasikan — simpan tetap inline di klien

### Task 4.4 — Refactor `sync_stokopname.php` ✅

**File:** `AppAndroid/api/sync_stokopname.php`

> **Arsitektur:** PHP melakukan INSERT inline, bukan via SP.
> Validasi wajib dilakukan via SP helper sebelum INSERT.

- [x] Hapus logika generate nomor opname inline (`SELECT MAX`) → ganti dengan `CALL sp_hlp_faktur_generate('SO', ...)`
- [x] Pertahankan INSERT inline ke `stok_opname` dan `HistoryBarang` dalam satu transaksi PHP
- [x] Ganti `UPDATE tbl_barang SET STOK_TOKO/GUDANG` inline → `CALL sp_hlp_stok_hitung(kode_barang)` per item (di dalam transaksi, fast mode background, dan full mode)
- [x] Ganti `UPDATE tbl_datareferensi` inline (yang bug AKUN_DK) → `CALL sp_hlp_saldo_akun_update(kode_akun)` per akun terlibat (PERSEDIAAN + PENYESUAIAN STOK MINUS)
- [ ] UPDATE `tbl_supliyer.HUTANGAKHIR`
- [ ] COMMIT, set `p_success = 1`

---

### Task 4.5 — Verifikasi Fase 4 ⬜

- [ ] Test retur jual dari VB.NET: stok bertambah, jurnal seimbang, piutang pelanggan terupdate
- [ ] Test retur beli dari VB.NET: stok berkurang, jurnal seimbang, hutang supplier terupdate
- [ ] Test opname dari VB.NET: stok terupdate sesuai selisih, jurnal seimbang
- [ ] Test opname dari Flutter (via `sync_stokopname.php`): hasil identik dengan VB.NET
- [ ] Verifikasi counter `RETUR_JUAL_*`, `RETUR_BELI_*`, `OPNAME_*` terupdate dengan benar

---

## Fase 5 — Transfer & Bayar

> **Status: ⬜ BELUM DIMULAI**
>
> **Arsitektur:** Simpan dan hapus tetap di klien (VB.NET inline).
> Form VB.NET hanya perlu: generate nomor via SP + DTP backdate (sudah selesai di Task 1.4 & 1.5).
>
> Prasyarat: Fase 4 selesai dan diverifikasi.

### Task 5.1–5.6 — Implementasi SP Transfer & Bayar — Tidak diimplementasikan — simpan tetap inline di klien

### Task 5.7 — Verifikasi Fase 5 ⬜

- [ ] Test transfer stok: counter `TRANSFER_STOK_*` terupdate, stok berubah, jurnal selisih konversi benar
- [ ] Test transfer barang: validasi stok sumber, counter `TRANSFER_BARANG_*` terupdate
- [ ] Test transfer cabang: counter `TRANSFER_CABANG_*` terupdate
- [ ] Test bayar hutang dari faktur: saldo kas berkurang, `TAGIHAN` di `pembelian` berkurang, hutang supplier terupdate
- [ ] Test bayar hutang dari saldo awal: `tbl_supliyer.HutangAwal` berkurang, jurnal benar
- [ ] Test bayar piutang dari faktur: saldo kas bertambah, `SISA_TAGIHAN` di `penjualan` berkurang
- [ ] Test bayar piutang dari saldo awal: `tbl_pelanggan.HutangAwal` berkurang, jurnal benar

### Task 5.8 — Audit Akun Jurnal Saldo Awal Master ⬜

**File:** `1Master/TambahSupliyer.vb`, `1Master/TambahPelanggan.vb`

- [ ] Audit `TambahSupliyer.vb` — konfirmasi akun jurnal saldo awal hutang supplier sudah benar
- [ ] Audit `TambahPelanggan.vb` — konfirmasi akun jurnal saldo awal piutang pelanggan sudah benar
- [ ] Jika akun perlu diubah: update `SimpanJurnalSaldoAwal()` di kedua form

### Task 5.9 — Update `FormLapMutasiKeuangan.vb` — Verifikasi Bayar Hutang/Piutang via Bank ⬜

**File:** `6Print/CetakLaporanKas/FormLapMutasiKeuangan.vb`

- [ ] Verifikasi `LoadRekapSekaliBaca`: CASE WHEN BayarHutang (`NOMOR_AKUN_K=@AKUN`) menangkap bayar hutang via BANK
- [ ] Verifikasi CASE WHEN BayarPiutang (`NOMOR_AKUN_D=@AKUN`) menangkap bayar piutang via BANK
- [ ] Test pilih rekening BANK → bayar hutang/piutang via transfer muncul di DGV dan total

---

## Fase 6 — Hapus Transaksi

> **Status: ⬜ BELUM DIMULAI — tidak ada perubahan kode**
>
> **Arsitektur:** Hapus transaksi tetap inline di `FormUtama.vb` menggunakan method
> `Hapusuntukedit()` / `Hapusbelanja()` / dll dari masing-masing form.
> Tidak ada task kode di fase ini — hanya verifikasi.
>
> Prasyarat: Fase 5 selesai dan diverifikasi.

### Task 6.1 — Verifikasi Logika Hapus di `FormUtama.vb` ⬜

- [ ] Verifikasi hapus penjualan: stok kembali, jurnal terhapus, saldo akun terupdate
- [ ] Verifikasi hapus pembelian: stok kembali, jurnal terhapus, hutang supplier terupdate
- [ ] Verifikasi hapus retur jual: counter `RETUR_JUAL_*` kembali, jurnal terhapus
- [ ] Verifikasi hapus retur beli: counter `RETUR_BELI_*` kembali, jurnal terhapus
- [ ] Verifikasi hapus opname: counter `OPNAME_*` kembali, stok kembali
- [ ] Verifikasi hapus transfer stok/barang/cabang: counter kembali
- [ ] Verifikasi hapus bayar hutang/piutang: jurnal terhapus, saldo akun terupdate

---

## Fase 7 — Gaji & Bon

> **Status: ⬜ BELUM DIMULAI**
>
> **Arsitektur:** Simpan dan hapus tetap di klien (VB.NET inline).
> Form VB.NET hanya perlu: generate nomor via SP + DTP backdate (sudah selesai di Task 1.4 & 1.5).
>
> Prasyarat: Fase 6 selesai dan diverifikasi.

### Task 7.1–7.3 — Implementasi SP Gaji & Bon — Tidak diimplementasikan — simpan tetap inline di klien

### Task 7.4 — Verifikasi Fase 7 ⬜

- [ ] Test simpan gaji: jurnal D BEBAN GAJI K KAS, saldo karyawan terupdate
- [ ] Test simpan gaji dengan potongan bon: jurnal tambahan benar, saldo karyawan berkurang
- [ ] Test simpan bon (BON): saldo karyawan bertambah, saldo kas berkurang
- [ ] Test simpan bon (BAYAR): saldo karyawan berkurang, saldo kas bertambah

### Task 7.5 — Update `FormLapMutasiKeuangan.vb` — Verifikasi Gaji & Bon ⬜

**File:** `6Print/CetakLaporanKas/FormLapMutasiKeuangan.vb`

- [ ] Verifikasi `JENIS_TRANSAKSI = 'Gaji'` di `JurnalUmum` saat simpan gaji
- [ ] Verifikasi `JENIS_TRANSAKSI = 'Bon'` / `'Bayar bon'` saat simpan bon
- [ ] Test di `FormLapMutasiKeuangan`: gaji muncul di "(-) Gaji Karyawan", bon di "(-) Bon Karyawan", bayar bon di "(+) Bayar Bon"

---

## Fase 8 — Batch, Sync & Bug Fix

> **Status: ⬜ BELUM DIMULAI**  
> Prasyarat: Fase 7 selesai dan diverifikasi.  
> **PENTING:** Bug fix harus selesai sebelum `sp_hlp_saldo_kas_validasi` diaktifkan di production.

### Task 8.1 — Update `FormLoading.vb` → Panggil SP Batch ⬜

**File:** `0Form/FormLoading.vb`

- [ ] Ganti `HitungSemuaKode()` dengan `CALL sp_bat_stok_semua_barang()`
- [ ] Ganti `HitungStokToko()` dengan `CALL sp_bat_stok_toko()`
- [ ] Ganti `HitungStokGudang()` dengan `CALL sp_bat_stok_gudang()`
- [ ] Ganti `UpdateSaldoSemuaAkun()` di `MulaiPosting()` dengan `CALL sp_bat_saldo_semua_akun()`
- [ ] Ganti `UpdatePiutangDibayar()` dengan `CALL sp_bat_piutang_semua_pelanggan()`
- [ ] Ganti `UpdateSupliyerFromPembelianHutangDibayar()` dengan `CALL sp_bat_hutang_semua_supplier()`
- [ ] Ganti `UpdateTotalBonDanTotalBayarKaryawan()` dengan `CALL sp_bat_bon_semua_karyawan()`
- [ ] Catatan: `MulaiLoading()` (saat login) tetap memanggil `UpdateSaldoSemuaAkun()` dari `ModuleVariabel.vb` — **bukan** `HITUNGSEMUASALDO()`

---

### Task 8.2 — Update `SyncManager.vb` ⬜

**File:** `9Sync/SyncManager.vb`

- [ ] Ganti panggilan `HitungStokPerubahan(kode)` dengan `CALL sp_hlp_stok_hitung(kode)` saat sync download barang dari cloud

---

### Task 8.2b — Buat Wrapper Functions di `ModuleVariabel.vb` ⬜

**File:** `Modules/ModuleVariabel.vb`  
**Referensi:** `design.md` §8.1

Setelah semua SP tersedia (Fase 1–7 selesai), ganti implementasi inline dengan wrapper ke SP. Ini memastikan semua kode VB.NET yang masih memanggil fungsi lama otomatis menggunakan SP.

- [ ] Ganti body `HitungStokPerubahan(kode, transaction)` → `CALL sp_hlp_stok_hitung(kode)` via MySqlCommand
- [ ] Ganti body `HitungStokToko()` → `CALL sp_bat_stok_toko()`
- [ ] Ganti body `HitungStokGudang()` → `CALL sp_bat_stok_gudang()`
- [ ] Ganti body `HitungSemuaKode()` → `CALL sp_bat_stok_semua_barang()`
- [ ] Ganti body `UpdateSaldoAkun(kodeAkun, transaction)` → `CALL sp_hlp_saldo_akun_update(kodeAkun)` (setelah bug fix Task 8.3 selesai, wrapper ini menggantikan implementasi lama)
- [ ] Ganti body `UpdateSaldoSemuaAkun(transaction)` → `CALL sp_bat_saldo_semua_akun()`
- [ ] Ganti body `UpdateSaldoSemuaAkun()` (overload tanpa transaction) → `CALL sp_bat_saldo_semua_akun()`
- [ ] Ganti body `UpdatePiutangDibayar()` → `CALL sp_bat_piutang_semua_pelanggan()`
- [ ] Ganti body `UpdateSupliyerFromPembelianHutangDibayar()` → `CALL sp_bat_hutang_semua_supplier()`
- [ ] Ganti body `UpdateTotalBonDanTotalBayarKaryawan()` → `CALL sp_bat_bon_semua_karyawan()`
- [ ] Pertahankan signature fungsi yang sama agar semua caller tidak perlu diubah
- [ ] Catatan: `UpdatePiutangPelanggan(idPelanggan, transaction)` (per-pelanggan realtime) sudah digantikan oleh logika di dalam masing-masing SP transaksi — tidak perlu wrapper, tapi jangan hapus dulu sampai semua form dimigrasikan
- [ ] **Catatan (Konflik 5):** Wrapper ini hanya mengganti fungsi **batch** (`UpdateSupliyerFromPembelianHutangDibayar`, `UpdatePiutangDibayar`). Fungsi **realtime per-entitas** (`UpdateHutangSupliyer(kode, transaction)`, `UpdatePiutangPelanggan(kode, transaction)`) **tidak diganti** — masih dipakai di Task 9.3e dan Task 9.1b.

---

### Task 8.3 — Perbaiki Bug `UpdateSaldoAkun` / `UpdateSaldoSemuaAkun` di `ModuleVariabel.vb` ✅

**File:** `Modules/ModuleVariabel.vb`  
**Referensi:** `requirements.md` Req 18 Bug 2, `design.md` §6.2

- [x] Temukan fungsi `UpdateSaldoAkun()` (per-akun) di `ModuleVariabel.vb`
- [x] Ganti rumus `SET r.Saldo_Akhir = IFNULL(r.Saldo_Awal, 0) + IFNULL(d.total_debet, 0) - IFNULL(k.total_kredit, 0)` dengan:
  ```sql
  SET r.Saldo_Akhir = CASE
    WHEN r.AKUN_DK = 'DEBET'  THEN IFNULL(r.Saldo_Awal, 0) + IFNULL(d.total_debet, 0) - IFNULL(k.total_kredit, 0)
    WHEN r.AKUN_DK = 'KREDIT' THEN IFNULL(r.Saldo_Awal, 0) - IFNULL(d.total_debet, 0) + IFNULL(k.total_kredit, 0)
    ELSE 0 END
  ```
- [x] Temukan fungsi `UpdateSaldoSemuaAkun()` di `ModuleVariabel.vb`
- [x] Terapkan perbaikan rumus yang sama untuk `UpdateSaldoSemuaAkun()`
- [x] Verifikasi: setelah perbaikan, saldo akun KREDIT (hutang, pendapatan, modal) dihitung dengan benar

---

### Task 8.4 — Perbaiki Bug `HITUNGSEMUASALDO` Step 3 di `FormLapNeracaLR.vb` ✅

**File:** `5Lap/FormLapNeracaLR.vb`  
**Referensi:** `requirements.md` Req 18 Bug 1, `design.md` §6.1

- [x] Temukan fungsi `HITUNGSEMUASALDO()` di `FormLapNeracaLR.vb`
- [x] Temukan Step 3 yang membaca `SALDO_SEBELUMNYA` untuk akun LABA/RUGI (baris ~118 di kode aktual)
- [x] Ganti `SALDO_SEBELUMNYA` dengan `SALDO_AKHIR` di query Step 3:
  ```sql
  -- Sebelum (salah):
  SUM(CASE WHEN SUB_AKUN='LABA' THEN SALDO_SEBELUMNYA ELSE 0 END) -
  SUM(CASE WHEN SUB_AKUN='RUGI' THEN SALDO_SEBELUMNYA ELSE 0 END) AS LABA_RUGI
  
  -- Sesudah (benar):
  SUM(CASE WHEN SUB_AKUN='LABA' THEN SALDO_AKHIR ELSE 0 END) -
  SUM(CASE WHEN SUB_AKUN='RUGI' THEN SALDO_AKHIR ELSE 0 END) AS LABA_RUGI
  ```
- [x] Verifikasi: nilai LABA RUGI BERJALAN setelah posting = Pendapatan - Biaya (bukan selisih saldo awal)

---

### Task 8.4b — Perbaiki Bug `TerapkanDeltaSaldoAkun` di `FormEditBayarJual.vb` ⬜

**File:** `2Trans/FormEditBayarJual.vb`  
**Referensi:** `requirements.md` Req 18 Bug 2, `design.md` §6.2

> **Temuan dari kode aktual:** `FormEditBayarJual.vb` punya fungsi `TerapkanDeltaSaldoAkun()` yang mengupdate `Saldo_Akhir` dengan cara `+= delta` (bukan recalculate penuh). Ini berbeda dari `UpdateSaldoAkun` di `ModuleVariabel.vb`. Fungsi ini **tidak punya bug AKUN_DK** karena menggunakan delta (bukan rumus awal+D-K), tapi perlu diverifikasi apakah delta dihitung dengan benar untuk akun KREDIT.

- [ ] Review `TerapkanDeltaSaldoAkun()` di `FormEditBayarJual.vb` — verifikasi apakah delta jurnal dihitung dengan benar untuk akun KREDIT (hutang, pendapatan)
- [ ] Jika delta tidak menghormati AKUN_DK: ganti dengan panggilan `CALL sp_hlp_saldo_akun_update(kode_akun)` untuk setiap akun yang terlibat
- [ ] Jika delta sudah benar: tambahkan komentar dokumentasi bahwa fungsi ini sudah benar dan tidak perlu diubah
- [ ] Verifikasi: edit pembayaran penjualan kredit → saldo KAS dan PIUTANG terupdate dengan benar

---

### Task 8.4c — Migrasi `FormSuratJalan.vb` — Generate Nomor ke SP ⬜

**File:** `2Trans/FormSuratJalan.vb`  
**Referensi:** `requirements.md` Appendix A (FormSuratJalan — GN ✅)

> **Catatan:** Sudah dipindahkan ke Task 1.4 — dikerjakan bersamaan dengan semua form lain.
> Lihat Task 1.4 untuk detail implementasi.

### Task 8.5 — Implementasi Redirect Laporan ke `temp_datareferensi` ✅ SELESAI

**File:** `5Lap/FormLapNeracaLR.vb`  
**Referensi:** `requirements.md` Req 17, `design.md` §6.3

> **Catatan:** Req 17 sudah selesai! `FormLapNeracaLR.vb` sudah menggunakan `temp_datareferensi` untuk kalkulasi laporan per periode, dan `tbl_datareferensi` tetap digunakan untuk state transaksi realtime dan posting resmi.

- [x] `FormLapNeracaLR.vb` sudah menggunakan `temp_datareferensi` untuk laporan per periode
- [x] `HITUNGSEMUASALDO()` (dari `MulaiPosting()`) tetap menulis ke `tbl_datareferensi`
- [x] Verifikasi: membuka laporan neraca per periode tidak mengubah `tbl_datareferensi.Saldo_Akhir`

---

### Task 8.6 — Verifikasi Fase 8 ⬜

- [ ] Verifikasi `sp_bat_stok_semua_barang` menghasilkan nilai identik dengan `HitungSemuaKode()` lama
- [ ] Verifikasi `sp_bat_saldo_semua_akun` menghasilkan nilai yang **benar** (bukan identik dengan VB lama yang bug)
- [ ] Test bug fix AKUN_DK: penjualan 1.000.000 → saldo PENJUALAN (KREDIT) bertambah 1.000.000 ✓
- [ ] Test bug fix AKUN_DK: pembelian kredit 500.000 → saldo HUTANG BELANJA (KREDIT) bertambah 500.000 ✓
- [ ] Test bug fix HITUNGSEMUASALDO: pendapatan 1M - biaya 600K → LABA RUGI BERJALAN = 400.000 ✓
- [ ] Test laporan neraca per periode: `tbl_datareferensi.Saldo_Akhir` tidak berubah setelah buka laporan
- [ ] Test dua user buka laporan bersamaan: tidak saling menimpa (keduanya tulis ke `temp_datareferensi`)
- [ ] Verifikasi neraca seimbang: Aset = Pasiva + Modal setelah semua bug fix
- [ ] Verifikasi wrapper `ModuleVariabel.vb`: panggil `HitungStokPerubahan` → SP dipanggil, hasil identik
- [ ] Verifikasi `FormSuratJalan.vb`: generate nomor tidak race condition saat dua user simpan bersamaan
- [ ] Verifikasi `FormEditBayarJual.vb`: edit pembayaran → saldo akun terupdate dengan benar

---

## Fase 9 — Master dengan Jurnal

> **Status: ⬜ BELUM DIMULAI**  
> Prasyarat: Fase 8 selesai dan diverifikasi.

### Task 9.1 — Update `FormBarang.vb` — Generate Nomor Standar & SP ⬜

**File:** `1Master/FormBarang.vb`  
**Referensi:** `requirements.md` Appendix A (FormBarang — GN ⚠️)

> **Keputusan Arsitektur: LEWATI — tidak perlu SP untuk master barang**
>
> **Alasan berdasarkan data aktual:**
> - `stoktambahkurang` pakai format timestamp `20260401082832` — bukan format faktur transaksi. Tidak ada race condition karena operasi ini dilakukan admin/pemilik, bukan kasir multi-user bersamaan.
> - `update_product.php` (PHP) hanya update `NAMA_KATEGORI` dan `NAMA_MERK` — tidak ada jurnal, tidak ada stok. Tidak perlu SP.
> - Tidak ada PHP endpoint untuk tambah/kurang stok manual dari Flutter.
> - Frekuensi: `kurang barang` 2.768 entri, `tambah barang` 336 entri — jarang dibanding penjualan 484.018 entri.
> - Tidak ada validasi stok atau logika kompleks yang perlu dikonsistensikan antar klien.
>
> **Yang tetap perlu diperbaiki (tanpa SP):**
> - [ ] Ganti `HitungStokPerubahan()` dengan `CALL sp_hlp_stok_hitung(kode)` — sudah ada SP-nya (Task 8.2b)
> - [ ] Ganti `UpdateSaldoAkun()` dengan `CALL sp_hlp_saldo_akun_update(kode_akun)` — sudah ada SP-nya (Task 8.2b)
> - [ ] Format nomor `DateTime.Now.ToString("yyyyMMddHHmmss")` — **biarkan**, bukan format faktur transaksi, tidak rawan race condition untuk operasi admin

---

### Task 9.1b — Kunci TxtAwal ReadOnly & Perbaiki Recalculate HutangAkhir Saat Edit Master ⬜

**File:** `1Master/TambahSupliyer.vb`, `1Master/TambahPelanggan.vb`  
**Referensi:** `requirements.md` Req 24 AC #5, AC #6, AC #7, AC #8

> **Masalah 1:** Saat `HutangAwal` diubah, `HutangAkhir` tidak langsung direcalculate.
>
> **Masalah 2 (kritis — dilaporkan user):** Jika `HutangAwal` diturunkan setelah ada
> pembayaran, `HutangAkhir` bisa menjadi negatif.
>
> **Solusi yang dipilih: ReadOnly** — lebih aman dari peringatan yang bisa diabaikan.
> Jika `HutangAwal > 0` saat mode edit, field dikunci. Tidak bisa diubah sama sekali.

**File:** `1Master/TambahSupliyer.vb`
- [ ] Di event load form edit (saat `_isEditMode = True`): baca `HutangAwal` dari DB
  - Jika `HutangAwal > 0`: set `TxtAwal.ReadOnly = True`, ubah warna background ke abu-abu
    sesuai tema, tambahkan tooltip: "Saldo awal tidak bisa diubah setelah ada nilai. Gunakan jurnal manual untuk koreksi."
  - Jika `HutangAwal = 0`: `TxtAwal.ReadOnly = False` — bisa diisi
- [ ] Di `UpdateSupliyer()`, setelah UPDATE `tbl_supliyer` dan jurnal selisih:
  - Tambahkan panggilan `UpdateHutangSupliyer(kode, transaction)` agar `HutangAkhir` langsung terupdate

**File:** `1Master/TambahPelanggan.vb`
- [ ] Di event load form edit (saat `_isEditMode = True`): baca `HutangAwal` dari DB
  - Jika `HutangAwal > 0`: set `TxtAwal.ReadOnly = True`, ubah warna background, tambahkan tooltip
  - Jika `HutangAwal = 0`: `TxtAwal.ReadOnly = False`
- [ ] Di `UpdatePelanggan()`, setelah UPDATE `tbl_pelanggan` dan jurnal selisih:
  - Tambahkan panggilan `UpdatePiutangPelanggan(kode, transaction)` agar `HutangAkhir` langsung terupdate

**Verifikasi Task 9.1b:**
- [ ] Test edit supplier `HutangAwal = 1.000.000`: `TxtAwal` readonly, tidak bisa diketik
- [ ] Test edit supplier `HutangAwal = 0`: `TxtAwal` bisa diisi, simpan normal
- [ ] Test isi saldo awal pertama kali (dari 0 → 500.000): berhasil, jurnal saldo awal dibuat
- [ ] Test buka lagi form edit setelah isi saldo awal: `TxtAwal` sudah readonly
- [ ] Test edit data lain (nama, alamat) saat `TxtAwal` readonly: simpan berhasil, `HutangAkhir` tetap benar
- [ ] Test recalculate: edit nama supplier → `HutangAkhir` langsung terupdate setelah simpan
- [ ] Test yang sama untuk pelanggan (piutang)

---

### Task 9.2 — Update `TambahBarang.vb` — Jurnal Penyesuaian HPP ⬜

**File:** `1Master/TambahBarang.vb`

> **Keputusan Arsitektur: LEWATI — tidak perlu SP untuk jurnal penyesuaian HPP**
>
> **Alasan berdasarkan data aktual:**
> - `Edit Barang` 2.118 entri, `Hapus Barang` 44 entri — operasi admin, bukan multi-user kasir.
> - Tidak ada PHP endpoint untuk edit/hapus barang dari Flutter yang butuh jurnal.
> - `update_product.php` hanya update kategori/merk — tidak menyentuh HPP atau jurnal.
>
> **Yang tetap perlu diperbaiki (tanpa SP):**
> - [ ] Ganti `INSERT INTO JurnalUmum` inline dengan `CALL INSERT INTO JurnalUmum inline(...)` — konsisten dengan pola helper SP
> - [ ] Ganti `UpdateSaldoAkun()` dengan `CALL sp_hlp_saldo_akun_update(kode_akun)` — sudah ada SP-nya (Task 8.2b)

---

### Task 9.3 — Perbaiki Bug & Migrasi `FormKeuangan.vb` ⬜

**File:** `3Jurnal/FormKeuangan.vb`  
**Referensi:** `requirements.md` Appendix A (FormKeuangan — PJ ✅, US ✅)

> **Temuan dari kode aktual** — ada 4 bug kritis yang harus diperbaiki terlepas dari keputusan arsitektur SP:

#### Bug 1 — `DeleteTransaction` tidak update saldo akun ✅ DIPERBAIKI

`SaveNewTransaction()` dan `UpdateExistingTransaction()` keduanya memanggil `UpdateSaldoSemuaAkun(transaction)` setelah operasi. Tapi `DeleteTransaction()` **tidak memanggil** `UpdateSaldoSemuaAkun` sama sekali — saldo akun tidak terupdate setelah jurnal dihapus.

- [x] Tambahkan `MySqlTransaction` wrapper di `DeleteTransaction()` (saat ini tidak ada — DELETE berjalan tanpa transaksi)
- [x] Tambahkan `UpdateSaldoSemuaAkun(transaction)` setelah `DELETE FROM JurnalUmum` di dalam transaksi
- [x] Pindahkan `ModuleAuditTrail.CatatAuditMaster` ke dalam blok transaksi yang sama

#### Bug 2 — `UpdateSaldoSemuaAkun` masih pakai rumus lama (bug AKUN_DK) ❌ KRITIS

`SaveNewTransaction()` dan `UpdateExistingTransaction()` memanggil `UpdateSaldoSemuaAkun(transaction)` dari `ModuleVariabel.vb` — yang **belum diperbaiki** bug AKUN_DK-nya (Task 8.3). Akun KREDIT (hutang, pendapatan) dihitung terbalik.

- [ ] Setelah Task 8.3 selesai (perbaikan `UpdateSaldoSemuaAkun` di `ModuleVariabel.vb`), verifikasi `FormKeuangan.vb` otomatis ikut terperbaiki karena memanggil fungsi yang sama
- [ ] Atau ganti langsung dengan `CALL sp_hlp_saldo_akun_update(@kode_akun_d)` + `CALL sp_hlp_saldo_akun_update(@kode_akun_k)` — lebih targeted, tidak recalculate semua akun

#### Bug 3 — `GenerateTransactionId` rawan race condition ✅ DIPERBAIKI

Menggunakan `SELECT MAX(CAST(RIGHT(...,4) AS UNSIGNED)) + 1` tanpa `FOR UPDATE` — dua user bisa dapat nomor yang sama.

- [x] Ganti `GenerateTransactionId()` dengan `CALL sp_hlp_faktur_generate(prefix, DATE(tgl), 'jurnalumum', 'NO_TRANSAKSI', @nomor)`
- [x] Baca OUT parameter `@nomor` dan isi ke `LblIdBayar.Text`

#### Bug 4 — Format nomor tidak konsisten ✅ DIPERBAIKI

`GenerateTransactionId()` menghasilkan `MS-260419-0001` (ada `-` antara tanggal dan urut). Semua transaksi lain pakai `MS-2604190001` (tanpa `-` ke-2). Lihat `GetTransactionPrefix()` — prefix sudah benar (2 huruf: MS, KL, BY, SB, BB, PR).

- [x] Setelah migrasi ke `sp_hlp_faktur_generate`, format otomatis benar (`PREFIX-YYMMDDXXXX`)

#### Keputusan Arsitektur — Simpan & Hapus Tetap Inline

- [x] **Keputusan: Simpan dan hapus jurnal manual tetap inline di `FormKeuangan.vb`**
  - Saldo akun diupdate via `sp_hlp_saldo_akun_update` (rumus AKUN_DK benar) setelah INSERT/DELETE inline
  - Tidak bergantung pada perbaikan `UpdateSaldoSemuaAkun` di VB (Task 8.3)

#### Task 9.3e — Tambah Jenis Transaksi PINJAMAN SUPPLIER & PINJAMAN PELANGGAN di FormKeuangan 🔄

**File:** `3Jurnal/FormKeuangan.vb`  
**Referensi:** `requirements.md` Req 25

> **Skenario yang didukung:**
> - **PINJAMAN SUPPLIER**: Supplier beri pinjaman tunai ke toko → KAS masuk, hutang ke supplier bertambah
>   Jurnal: D KAS / K HUTANG BELANJA (`03.01.001`)
> - **PINJAMAN PELANGGAN**: Pelanggan pinjam uang tunai dari toko → KAS keluar, piutang ke pelanggan bertambah
>   Jurnal: D PIUTANG USAHA (`01.03.001`) / K KAS

- [x] Tambahkan tombol `BtnBayarBon` diubah menjadi "PINJAMAN SUPPLIER (F7)" di toolbar `FormKeuangan`
- [x] Tambahkan tombol `Button1` diubah menjadi "PINJAMAN PELANGGAN (F10)" di toolbar `FormKeuangan`
- [x] Update `ResetButtonColors()` — sertakan dua tombol baru
- [x] Saat `BtnBayarBon` diklik — tambahkan `Case "PINJAMAN SUPPLIER"` ke `Select Case currentType`:
  ```vb
  Case "PINJAMAN SUPPLIER"
      AddFromTypes(debetItems, byType, {"KAS", "BANK"})       ' KAS/BANK masuk
      AddFromTypes(kreditItems, byType, {"HUTANG"})            ' Hutang ke supplier
  ```
- [x] Saat `Button1` diklik — tambahkan `Case "PINJAMAN PELANGGAN"` ke `Select Case currentType`:
  ```vb
  Case "PINJAMAN PELANGGAN"
      AddFromTypes(debetItems, byType, {"PIUTANG"})            ' Piutang ke pelanggan
      AddFromTypes(kreditItems, byType, {"KAS", "BANK"})       ' KAS/BANK keluar
  ```
- [x] Prefix SP: `PINJAMAN SUPPLIER` → `PS`, `PINJAMAN PELANGGAN` → `PP` (di SP dan `GetTransactionPrefix`)
- [x] Tooltip ditambahkan untuk kedua tombol baru
- [x] Di `SaveNewTransaction()` untuk PINJAMAN SUPPLIER:
  - Setelah INSERT jurnal: `UPDATE tbl_supliyer SET HutangAwal = HutangAwal + @nominal WHERE Kode = @kode`
  - CALL `UpdateHutangSupliyer(kode, transaction)` untuk recalculate `HutangAkhir`
  - **Catatan (Konflik 5):** Ini memanggil versi **realtime per-supplier** dari `ModuleVariabel.vb`,
    bukan batch. Wrapper di Task 8.2b hanya mengganti `UpdateSupliyerFromPembelianHutangDibayar()`
    (batch semua supplier) — bukan `UpdateHutangSupliyer(kode, transaction)` (per-supplier).
- [x] Di `SaveNewTransaction()` untuk PINJAMAN PELANGGAN:
  - Setelah INSERT jurnal: `UPDATE tbl_pelanggan SET HutangAwal = HutangAwal + @nominal WHERE Kode = @kode`
  - CALL `UpdatePiutangPelanggan(kode, transaction)` untuk recalculate `HutangAkhir`
  - **Catatan (Konflik 5):** Sama — ini versi realtime per-pelanggan, bukan batch.
- [x] Di `DeleteTransaction()` untuk PINJAMAN SUPPLIER:
  - Setelah DELETE jurnal: `UPDATE tbl_supliyer SET HutangAwal = HutangAwal - @nominal WHERE Kode = @kode`
  - CALL `UpdateHutangSupliyer(kode, transaction)`
- [x] Di `DeleteTransaction()` untuk PINJAMAN PELANGGAN:
  - Setelah DELETE jurnal: `UPDATE tbl_pelanggan SET HutangAwal = HutangAwal - @nominal WHERE Kode = @kode`
  - CALL `UpdatePiutangPelanggan(kode, transaction)`

**Verifikasi Task 9.3e:**
- [ ] Test PINJAMAN SUPPLIER 5.000.000 dari supplier X: jurnal D KAS K HUTANG BELANJA ada, `HutangAwal` supplier X bertambah 5.000.000, `HutangAkhir` terupdate, KAS bertambah
- [ ] Test buka `FormBayarHutang` supplier X: baris "Saldo Awal" 5.000.000 muncul dan bisa dicentang
- [ ] Test PINJAMAN PELANGGAN 2.000.000 ke pelanggan Y: jurnal D PIUTANG USAHA K KAS ada, `HutangAwal` pelanggan Y bertambah 2.000.000, KAS berkurang
- [ ] Test buka `FormBayarPiutang` pelanggan Y: baris "Saldo Awal" 2.000.000 muncul
- [ ] Test hapus jurnal PINJAMAN SUPPLIER: `HutangAwal` supplier berkurang kembali, KAS berkurang kembali
- [ ] Test laporan mutasi kas: PINJAMAN SUPPLIER muncul sebagai pemasukan (+), PINJAMAN PELANGGAN muncul sebagai pengeluaran (-)

---

#### Task 9.3f — Update `FormLapMutasiKeuangan.vb` — Tambah Jenis Transaksi Baru & Pisahkan Bon/Gaji ✅

**File:** `6Print/CetakLaporanKas/FormLapMutasiKeuangan.vb` (kode + designer)  
**Referensi:** `requirements.md` Req 25, Req 26

> **Dua perubahan sekaligus:**
> 1. Tambah baris PINJAMAN SUPPLIER (+) dan PINJAMAN PELANGGAN (-) — Req 25
> 2. Pisahkan Bon, Bayar Bon, Gaji dari baris Pemasukan/Pengeluaran — Req 26
>
> **Kondisi saat ini (Req 26):**
> - `'Bayar bon'` digabung ke baris Pemasukan → tidak terlihat terpisah
> - `'Bon'` dan `'Gaji'` digabung ke baris Pengeluaran → tidak terlihat terpisah

**Perubahan SQL di `LoadRekapSekaliBaca`:**

- [x] Pisahkan `'Bayar bon'` dari Pemasukan:
  ```sql
  -- Sebelum: JENIS_TRANSAKSI IN ('Pemasukan','Bayar bon')
  -- Sesudah: JENIS_TRANSAKSI = 'Pemasukan'
  ```
- [x] Pisahkan `'Bon'` dan `'Gaji'` dari Pengeluaran:
  ```sql
  -- Sebelum: JENIS_TRANSAKSI IN ('Pengeluaran','Bon','Gaji')
  -- Sesudah: JENIS_TRANSAKSI = 'Pengeluaran'
  ```
- [x] Tambah CASE WHEN baru untuk Bon (K KAS = keluar):
  ```sql
  IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Bon' AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS BonTotal,
  IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Bon' AND NOMOR_AKUN_K=@AKUN THEN 1 END),0) AS BonNota,
  ```
- [x] Tambah CASE WHEN baru untuk Bayar Bon (D KAS = masuk):
  ```sql
  IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Bayar bon' AND NOMOR_AKUN_D=@AKUN THEN NOMINAL ELSE 0 END),0) AS BayarBonTotal,
  IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Bayar bon' AND NOMOR_AKUN_D=@AKUN THEN 1 END),0) AS BayarBonNota,
  ```
- [x] Tambah CASE WHEN baru untuk Gaji (K KAS = keluar):
  ```sql
  IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='Gaji' AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS GajiTotal,
  IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='Gaji' AND NOMOR_AKUN_K=@AKUN THEN 1 END),0) AS GajiNota,
  ```
- [x] Tambah CASE WHEN baru untuk PINJAMAN SUPPLIER (D KAS = masuk):
  ```sql
  IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='PINJAMAN SUPPLIER' AND NOMOR_AKUN_D=@AKUN THEN NOMINAL ELSE 0 END),0) AS PinjamanSupplierTotal,
  IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='PINJAMAN SUPPLIER' AND NOMOR_AKUN_D=@AKUN THEN 1 END),0) AS PinjamanSupplierNota,
  ```
- [x] Tambah CASE WHEN baru untuk PINJAMAN PELANGGAN (K KAS = keluar):
  ```sql
  IFNULL(SUM(CASE WHEN JENIS_TRANSAKSI='PINJAMAN PELANGGAN' AND NOMOR_AKUN_K=@AKUN THEN NOMINAL ELSE 0 END),0) AS PinjamanPelangganTotal,
  IFNULL(COUNT(CASE WHEN JENIS_TRANSAKSI='PINJAMAN PELANGGAN' AND NOMOR_AKUN_K=@AKUN THEN 1 END),0) AS PinjamanPelangganNota,
  ```
- [x] Tambahkan `'Bayar Hutang Saldo Awal'` ke CASE WHEN BayarHutang (K KAS)
- [x] Tambahkan `'Bayar Piutang Saldo Awal'` ke CASE WHEN BayarPiutang (D KAS)

**Perubahan di VB (form code):**

- [x] Tambahkan di designer: `TxtTotalBon`, `TxtNotaBon`, `TxtTotalBayarBon`, `TxtNotaBayarBon`, `TxtTotalGaji`, `TxtNotaGaji`, `TxtTotalPinjamanSupplier`, `TxtNotaPinjamanSupplier`, `TxtTotalPinjamanPelanggan`, `TxtNotaPinjamanPelanggan`
- [x] Di `LoadRekapSekaliBaca`: isi semua TextBox baru dari hasil query
- [x] Di `TxtTotal_TextChanged` — tambahkan ke handles dan kalkulasi:
  ```vb
  totalHariIni -= Bon              ' KAS keluar (-)
  totalHariIni += BayarBon         ' KAS masuk (+)
  totalHariIni -= Gaji             ' KAS keluar (-)
  totalHariIni += PinjamanSupplier ' KAS masuk (+)
  totalHariIni -= PinjamanPelanggan' KAS keluar (-)
  ```
- [x] Di `TxtNota_TextChanged`: tambahkan ke handles dan kalkulasi nota
- [x] Di `PD_PrintPage` (thermal) — tambahkan baris cetak kondisional:
  - `"(-) Bon Karyawan"` jika `totalBon <> 0`
  - `"(+) Bayar Bon"` jika `totalBayarBon <> 0`
  - `"(-) Gaji Karyawan"` jika `totalGaji <> 0`
  - `"(+) Pinjaman Supplier"` jika `totalPinjamanSupplier <> 0`
  - `"(-) Pinjaman Pelanggan"` jika `totalPinjamanPelanggan <> 0`
- [x] Di `PDDot_PrintPage` (dot matrix): tambahkan baris cetak yang sama

**Verifikasi Task 9.3f:**
- [ ] Test bon karyawan: muncul di baris "(-) Bon Karyawan", **tidak** muncul di "Jurnal Pengeluaran"
- [ ] Test bayar bon: muncul di baris "(+) Bayar Bon", **tidak** muncul di "Jurnal Pemasukan"
- [ ] Test gaji: muncul di baris "(-) Gaji Karyawan", **tidak** muncul di "Jurnal Pengeluaran"
- [ ] Test PINJAMAN SUPPLIER: muncul di baris "(+) Pinjaman Supplier"
- [ ] Test PINJAMAN PELANGGAN: muncul di baris "(-) Pinjaman Pelanggan"
- [ ] Test bayar hutang saldo awal: muncul di baris "(-) Bayar hutang"
- [ ] Test bayar piutang saldo awal: muncul di baris "(+) Piutang di bayar"
- [ ] Verifikasi saldo akhir tidak berubah setelah pemisahan bon/gaji (nilai total sama, hanya tampilan berbeda)
- [ ] Test cetak thermal dan dot matrix: semua baris baru muncul dengan benar

#### Verifikasi setelah perbaikan

- [ ] Test simpan jurnal PEMASUKAN: saldo akun DEBET bertambah, saldo akun KREDIT bertambah (rumus AKUN_DK benar)
- [ ] Test simpan jurnal PENGELUARAN: saldo kas berkurang
- [ ] Test hapus jurnal: saldo akun kembali ke nilai sebelum jurnal
- [ ] Test dua user simpan jurnal bersamaan: tidak ada duplikat `NO_TRANSAKSI`
- [ ] Test format nomor: `MS-2604190001` (bukan `MS-260419-0001`) ✅ sudah benar

---

### Task 9.4 — Verifikasi Fase 9 ⬜

- [ ] Test tambah stok manual di FormBarang: nomor format standar, stok terupdate, jurnal ada
- [ ] Test kurang stok manual di FormBarang: nomor format standar, stok terupdate, jurnal ada
- [ ] Test tambah barang baru dengan HPP di TambahBarang: jurnal penyesuaian persediaan ada
- [ ] Verifikasi tidak ada lagi `DateTime.Now.ToString("yyyyMMddHHmmss")` sebagai nomor transaksi

---

### Task 9.5 — Buat `CHANGELOG_MIGRASI.md` ⬜

**File:** `CHANGELOG_MIGRASI.md` (root workspace)

- [ ] Buat file `CHANGELOG_MIGRASI.md` dengan template:
  ```markdown
  # Changelog Migrasi Business Logic ke MySQL SP
  
  ## Fase 1 — Helper SP & Batch SP
  - Tanggal selesai: 2026-04-19
  - SP diimplementasikan: sp_hlp_stok_hitung, sp_hlp_stok_validasi, sp_hlp_faktur_generate,
    INSERT INTO JurnalUmum inline, sp_hlp_saldo_akun_update, sp_hlp_saldo_kas_validasi,
    sp_bat_stok_semua_barang, sp_bat_stok_toko, sp_bat_stok_gudang,
    sp_bat_saldo_semua_akun, sp_bat_piutang_semua_pelanggan,
    sp_bat_hutang_semua_supplier, sp_bat_bon_semua_karyawan
  - Form diupdate: —
  - Hasil verifikasi: ✅ Semua 13 SP terbuat tanpa error
  
  ## Fase 2 — Penjualan
  - Tanggal selesai: [TBD]
  - SP diimplementasikan: [daftar]
  - Form diupdate: [daftar]
  - Hasil verifikasi: [hasil parallel run]
  ```
- [ ] Update file ini setelah setiap fase selesai dan diverifikasi

---

## Ringkasan Progress

| Fase | Deskripsi | Status | Form VB.NET | PHP API |
|------|-----------|--------|-------------|---------|
| 1 | Helper SP & Batch SP | ✅ Selesai | — | — |
| 2 | Penjualan | ✅ Selesai | ✅ generate nomor + DTP | ✅ `sync_penjualan.php` (inline + validasi SP) |
| 3 | Pembelian | ✅ Form selesai | ✅ generate nomor + DTP | — |
| 4 | Retur & Opname | ⬜ | ✅ generate nomor + DTP | ⬜ `sync_stokopname.php` (inline + validasi SP) |
| 5 | Transfer & Bayar | ⬜ | ✅ generate nomor + DTP | — |
| 6 | Hapus Transaksi | ⬜ verifikasi saja | ✅ tetap inline | — |
| 7 | Gaji & Bon | ⬜ | ✅ generate nomor + DTP | — |
| 8 | Batch, Sync & Bug Fix | 🔄 Bug 8.3+8.4 ✅ | ⬜ 3 form + 2 module | — |
| 9 | Master dengan Jurnal | 🔄 | 🔄 `FormKeuangan.vb` | — |

**Aturan utama yang tidak boleh dilanggar:**
> - Simpan dan hapus transaksi **selalu inline** di klien (VB.NET dan PHP) dengan `conn.BeginTransaction()`
> - SP helper **wajib dipakai**: `sp_hlp_faktur_generate` (semua klien), `sp_hlp_stok_validasi` (PHP sebelum INSERT), `sp_hlp_stok_hitung` (PHP setelah UPDATE counter), `sp_hlp_saldo_akun_update` (PHP setelah INSERT jurnal)
> - PHP (`sync_penjualan.php`, `sync_stokopname.php`) melakukan INSERT inline + validasi via SP helper

---

## Catatan Penting

### Aturan Wajib — Jangan Ulangi Kesalahan Ini

> **Kesalahan yang pernah terjadi:** Task 2.3 dan 3.2 sempat diimplementasikan dengan memindahkan
> logika simpan VB.NET ke SP. Ini salah dan sudah di-revert. Jangan ulangi untuk fase berikutnya.

**Yang BOLEH dilakukan di form VB.NET:**
- Ganti generate nomor (`SELECT MAX` inline) → `sp_hlp_faktur_generate`
- Terapkan DTP backdate + `ModulHakAkses.SettingXxx`
- Perbaiki bug `UpdateSaldoSemuaAkun` (AKUN_DK)

**Yang DILARANG dilakukan di form VB.NET:**
- Mengganti `SimpanTransaksi()` / `Prosessimpan()` dengan logika SP
- Mengganti `Hapuspenjualan()` / `Hapusbelanja()` dengan logika SP
- Membuat `BuatTabelSementara*()` dan `IsiItem*()` lalu memanggilnya dari logika simpan VB

### Urutan Wajib
1. Fase 1 harus selesai sebelum fase lainnya (sudah selesai ✅)
2. Fase 2 harus selesai dan diverifikasi sebelum Fase 3
3. Bug fix di Fase 8 (Task 8.3, 8.4, 8.4b, 8.5) harus selesai sebelum `sp_hlp_saldo_kas_validasi` diaktifkan di production
4. Task 8.2b (wrapper `ModuleVariabel.vb`) harus dikerjakan **setelah** semua SP Fase 2–7 selesai — jangan wrapper dulu sebelum SP-nya ada
5. Jangan jalankan dua versi logika bersamaan untuk transaksi yang sama

### Aturan Task FormLapMutasiKeuangan

> **Setiap fase yang menambah jenis transaksi baru atau mengubah cara pembayaran WAJIB diikuti task update `FormLapMutasiKeuangan`.**

Pola yang harus diikuti:
- Setiap fase punya sub-task `Task X.Y — Update FormLapMutasiKeuangan` di akhir fase (sebelum verifikasi)
- Sub-task ini mencakup: (1) verifikasi `LoadRekapSekaliBaca` sudah menangkap jenis transaksi baru, (2) verifikasi tombol View di DGV menampilkan data yang benar, (3) verifikasi cetak thermal/dot matrix/inkjet/PDF
- Jika ada jenis transaksi baru yang belum ada di `LoadRekapSekaliBaca`, tambahkan CASE WHEN baru
- Jika ada jenis transaksi baru yang belum ada tombol View-nya, tambahkan handler di form

**Mapping jenis transaksi → JENIS_TRANSAKSI di JurnalUmum:**

| Transaksi | JENIS_TRANSAKSI | Arah KAS |
|-----------|-----------------|----------|
| Penjualan tunai | `'Penjualan'` | D KAS (+) |
| Penjualan transfer | `'Penjualan'` | D BANK (+) |
| Pembelian tunai | `'Pembelian'` | K KAS (-) |
| Pembelian transfer | `'Pembelian'` | K BANK (-) |
| Retur Beli (terima kas) | `'Retur Pembelian'` | D KAS (+) |
| Retur Jual (kembalikan kas) | `'Retur Penjualan'` | K KAS (-) |
| Bayar Hutang | `'Bayar Hutang'` | K KAS/BANK (-) |
| Bayar Hutang Saldo Awal | `'Bayar Hutang Saldo Awal'` | K KAS/BANK (-) |
| Bayar Piutang | `'Bayar Piutang'` | D KAS/BANK (+) |
| Bayar Piutang Saldo Awal | `'Bayar Piutang Saldo Awal'` | D KAS/BANK (+) |
| Jurnal Pemasukan | `'Pemasukan'` | D KAS (+) |
| Jurnal Pengeluaran | `'Pengeluaran'` | K KAS (-) |
| Jurnal Biaya | `'Biaya'` | K KAS (-) |
| Pindah Rekening masuk | `'PINDAH REKENING'` | D KAS/BANK (+) |
| Pindah Rekening keluar | `'PINDAH REKENING'` | K KAS/BANK (-) |
| Setor ke Bos | `'SETOR KE BOS'` | K KAS (-) |
| Bon Karyawan | `'Bon'` | K KAS (-) |
| Bayar Bon | `'Bayar bon'` | D KAS (+) |
| Gaji Karyawan | `'Gaji'` | K KAS (-) |
| Pinjaman Supplier | `'PINJAMAN SUPPLIER'` | D KAS (+) |
| Pinjaman Pelanggan | `'PINJAMAN PELANGGAN'` | K KAS (-) |

### Parallel Run
Setiap fase harus melalui parallel run minimal 1 minggu sebelum cutover penuh:
- Simpan transaksi yang sama via VB.NET lama dan SP baru
- Bandingkan: STOK_TOKO/GUDANG, Saldo_Akhir per akun, HutangAkhir pelanggan/supplier
- Verifikasi jurnal seimbang: `SUM(DEBET) = SUM(KREDIT)` per transaksi

### Changelog Migrasi
Setelah setiap fase selesai dan diverifikasi, update file `CHANGELOG_MIGRASI.md` (dibuat di Task 9.5):
- Tanggal selesai
- SP yang diimplementasikan
- Form yang diupdate
- Hasil verifikasi

---

## ✅ Audit Requirement 21: Update Harga Pokok Barang dari Pembelian dan Gap Jurnal

### 1. Audit FormPembelian.vb (Logika Update Harga Saat Ini)

FormPembelian.vb sudah memiliki **3 metode update harga** yang siap dipilih via pengaturan (ModulHakAkses.SettingMetodeUpdateHargaBeli):

| Metode | Fungsi yang Dipanggil | Keterangan |
|--------|------------------------|------------|
| 1. **Harga Terbaru** | `UpdateHargaTerbaru()` | `HARGA_BELI` dan `HARGA_BELI_TERAKHIR` di-set ke harga beli terakhir |
| 2. **Metode Average (Rata - Rata)** | `UpdateHargaAverage()` | `HARGA_BELI` = rata-rata, `HARGA_BELI_TERAKHIR` = harga beli terakhir |
| 3. **Tidak Ada** | `UpdateStokSaja()` | Hanya update stok, tidak update harga |

#### Lokasi Kode di FormPembelian.vb:
- Baris 2580–2587: `Select Case ModulHakAkses.SettingMetodeUpdateHargaBeli`
- Baris 2600–2611: `UpdateHargaTerbaru()`
- Baris 2613–2640: `UpdateHargaAverage()`
- Baris 2642–2651: `UpdateStokSaja()`

---

### 2. Cek COA di 11_migrasi_akun_coa.sql untuk Akun Penyesuaian

Dari `11_migrasi_akun_coa.sql`, ditemukan akun yang relevan untuk penyesuaian nilai persediaan:

| Kode Akun | Nama Akun | AKUN_DK | Keterangan |
|-----------|-----------|---------|------------|
| `01.04.001` | PERSEDIAAN BARANG | DEBET | Nilai barang dagangan di gudang/toko (harga perolehan) |
| `06.04.001` | PENYESUAIAN STOK MINUS | DEBET | Pencatatan selisih kurang (rugi) saat stok opname |

**Catatan:** Tidak ditemukan akun khusus untuk "PENYESUAIAN HARGA POKOK" atau "SELISIH HARGA POKOK" di COA saat ini.

---

### 3. Analisa Metode Harga yang Dipakai

Sistem sudah memiliki **3 metode** yang siap, dengan setting di `ModulHakAkses.SettingMetodeUpdateHargaBeli`:

| Metode | Rumus Perhitungan Selisih |
|--------|----------------------------|
| 1. **Harga Terbaru** | `selisih = (harga_baru - harga_lama) × stok_saat_ini` |
| 2. **Average (Rata-Rata)** | `harga_rata_baru = (harga_lama × stok_lama + harga_baru × qty_beli) / (stok_lama + qty_beli); selisih = (harga_rata_baru - harga_lama) × stok_lama` |
| 3. **Tidak Ada** | Tidak ada perubahan harga, tidak perlu jurnal penyesuaian |

---

### 4. Detail Implementasi Requirement 21

#### 4.1 Akun Jurnal Penyesuaian
Buat akun baru di `tbl_datareferensi` dengan kode `06.04.002`:

| Kolom | Nilai |
|-------|-------|
| STATUS | `'NULL'` |
| JENIS_AKUN | `'HPP'` |
| TYPE_AKUN | `'PENY. STOK'` |
| KODE_AKUN | `'06.04.002'` |
| NAMA_AKUN | `'PENYESUAIAN HARGA POKOK'` |
| SUB_AKUN | `'LABA'` |
| AKUN_DK | `'DEBET'` |
| AKUN_NRLR | `'LABA RUGI'` |
| KETERANGAN | `'Pencatatan selisih nilai persediaan akibat perubahan harga pokok barang (harga terbaru atau average cost). Digunakan untuk menjaga neraca tetap seimbang ketika harga pokok barang diupdate saat pembelian.'` |

**Cara migrasi:** Tambahkan INSERT baru di `11_migrasi_akun_coa.sql` dengan struktur yang sama dengan akun lainnya, sebelum `ON DUPLICATE KEY UPDATE`.

---

#### 4.2 Sumber Setting Metode Harga
`ModulHakAkses.SettingMetodeUpdateHargaBeli` **sumbernya dari GeneralSetting** (tidak hardcode).

---

#### 4.3 Cara Penyesuaian
Penyesuaian harga pokok dan gap jurnal dilakukan **per transaksi pembelian** (tidak per item), di dalam transaksi yang sama dengan simpan pembelian.

---

#### 4.4 Langkah Implementasi Berurutan ✅ SELESAI
1. [x] Tambahkan akun `06.04.002 PENYESUAIAN HARGA POKOK` di `11_migrasi_akun_coa.sql`
2. [x] Cek dan pastikan `ModulHakAkses.SettingMetodeUpdateHargaBeli` membaca dari GeneralSetting
3. [x] Modifikasi `UpdateHargaTerbaru()` dan `UpdateHargaAverage()` di FormPembelian.vb untuk:
   - Membaca `HARGA_BELI` lama SEBELUM update
   - Menghitung selisih nilai persediaan
   - Menyimpan informasi selisih untuk dipakai di bagian jurnal
4. [x] Modifikasi `Simpanjurnal()` di FormPembelian.vb untuk menambahkan jurnal penyesuaian jika `selisih ≠ 0`:
   - Jika `selisih > 0` (harga naik): D PERSEDIAAN BARANG (`01.04.001`), K PENYESUAIAN HARGA POKOK (`06.04.002`)
   - Jika `selisih < 0` (harga turun): D PENYESUAIAN HARGA POKOK (`06.04.002`), K PERSEDIAAN BARANG (`01.04.001`)
5. [x] Panggil `ModuleVariabel.UpdateSaldoAkun()` untuk kedua akun setelah jurnal penyesuaian di-INSERT (sudah otomatis ditangani oleh kode yang sudah ada)
6. [x] Pastikan semua dijalankan di dalam **transaksi yang sama** dengan simpan pembelian

---

## 5. Fase Migrasi AppAndroid/api ke Stored Procedure

Tujuan: Migrasikan AppAndroid/api (PHP) untuk menggunakan `sp_trx_*` alih-alih inline SQL, agar validasi stok dan bisnis logic dilakukan di server-side.

### 5.1 Prasyarat
- [x] File `07_migrasi_sp_transaksi.sql` sudah dibuat dan dijalankan ke database
- [x] Semua helper SP (`sp_hlp_*`) sudah ada di database

### 5.2 Langkah Migrasi AppAndroid/api
1. [ ] **Implementasi detail `sp_trx_penjualan_simpan`** di `07_migrasi_sp_transaksi.sql` (hilangkan `NOT_IMPLEMENTED`)
   - Tujuan: Agar SP bisa dipanggil oleh `sync_penjualan.php`
2. [ ] **Perbaiki `sync_penjualan.php`** untuk CALL `sp_trx_penjualan_simpan` alih-alih inline SQL
3. [ ] **Implementasi detail `sp_trx_opname_simpan`** di `07_migrasi_sp_transaksi.sql`
4. [ ] **Buat/Perbaiki `sync_stokopname.php`** untuk CALL `sp_trx_opname_simpan`
5. [ ] **Implementasi detail `sp_trx_transfer_stok_simpan`** di `07_migrasi_sp_transaksi.sql`
6. [ ] **Buat `sync_transfer_stok.php`** untuk CALL `sp_trx_transfer_stok_simpan`

### 5.3 Catatan Penting
- Semua migrasi dilakukan di sisi **PHP (AppAndroid/api)** SAJA
- VB.NET **TIDAK DIUBAH** di fase ini (sesuai prinsip migrasi)
- Validasi stok dan bisnis logic **harus 100% di SP**, tidak boleh di PHP

