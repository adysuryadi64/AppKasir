# Implementation Plan

## Overview

Implementasi fitur Sistem Poin Loyalitas Pelanggan untuk AppKasir. Fitur ini memungkinkan toko memberikan poin reward kepada pelanggan terdaftar pada setiap transaksi penjualan, dengan dua mekanisme perolehan poin (per item qty atau per kelipatan nominal). Penukaran poin dilakukan melalui form tersendiri (FormTukarPoin) dengan barang pilihan, bukan sebagai potongan harga di FormJual.

## Tasks

- [ ] 1. Migrasi Database — Tabel dan Kolom Loyalty Point
  - Buat file `32_loyalty_point_schema.sql` di folder `Database/`
  - Buat tabel `poin_config` dengan kolom: `ID` (PK auto), `AKTIF` (TINYINT), `MEKANISME` (VARCHAR: 'PER_ITEM'/'PER_NOMINAL'), `POIN_PER_QTY` (DECIMAL), `KELIPATAN_NOMINAL` (DECIMAL), `MINIMUM_REDEEM` (INT), `UPDATED_AT` (DATETIME)
  - Buat tabel `poin_ledger` dengan kolom: `ID` (PK auto), `KODE_PELANGGAN` (VARCHAR FK ke tbl_pelanggan.KODE), `TIPE` (ENUM: 'EARN','REDEEM','VOID_EARN'), `JUMLAH_POIN` (INT — positif untuk EARN, negatif untuk REDEEM/VOID_EARN), `NO_REFERENSI` (VARCHAR — nomor faktur atau nomor TP), `KETERANGAN` (VARCHAR), `CREATED_AT` (DATETIME), `ID_USER` (VARCHAR)
  - Buat tabel `poin_barang` dengan kolom: `ID_BARANG` (VARCHAR PK FK ke tbl_barang), `HARGA_POIN` (INT), `AKTIF` (TINYINT DEFAULT 1), `UPDATED_AT` (DATETIME)
  - Tambah kolom `SALDO_POIN INT NOT NULL DEFAULT 0` ke tabel `tbl_pelanggan` (ALTER TABLE)
  - Tambah index pada `poin_ledger(KODE_PELANGGAN)` dan `poin_ledger(NO_REFERENSI)` untuk performa query riwayat
  - Seed satu baris default ke `poin_config` (AKTIF=0, MEKANISME='PER_ITEM', POIN_PER_QTY=1, KELIPATAN_NOMINAL=10000, MINIMUM_REDEEM=100)
  - **Files:** `Database/32_loyalty_point_schema.sql`
  - **Requirement:** Req 1, Req 2, Req 3, Req 4, Req 8

- [ ] 2. ModuleLoyaltyPoin.vb — Engine Kalkulasi Poin
  - Buat file baru `ModuleLoyaltyPoin.vb` di folder `2Trans/` (sejajar dengan FormJual)
  - Deklarasikan `Module ModuleLoyaltyPoin` dengan variabel cache konfigurasi: `LP_Aktif As Boolean`, `LP_Mekanisme As String`, `LP_PoinPerQty As Decimal`, `LP_KelipatanNominal As Decimal`, `LP_MinimumRedeem As Integer`
  - Buat fungsi `MuatKonfigurasi()` — query `poin_config` dan isi variabel cache; dipanggil saat aplikasi start dan setelah simpan konfigurasi
  - Buat fungsi `HitungPoinEarn(daftarItem As List(Of ItemPoin), grandTotal As Decimal) As Integer` — implementasi dua mekanisme: PER_ITEM (sum qty × LP_PoinPerQty) dan PER_NOMINAL (floor(grandTotal / LP_KelipatanNominal))
  - Buat fungsi `CatatEarn(kodePelanggan As String, jumlahPoin As Integer, noFaktur As String, trans As MySqlTransaction)` — INSERT ke `poin_ledger` (TIPE='EARN') dan UPDATE `tbl_pelanggan SET SALDO_POIN = SALDO_POIN + jumlahPoin` dalam transaksi yang diberikan
  - Buat fungsi `CatatRedeem(kodePelanggan As String, jumlahPoin As Integer, noReferensi As String, trans As MySqlTransaction)` — INSERT ke `poin_ledger` (TIPE='REDEEM', JUMLAH_POIN negatif) dan UPDATE SALDO_POIN dalam transaksi yang diberikan
  - Buat fungsi `CatatVoidEarn(kodePelanggan As String, noFakturAsal As String, jumlahPoinVoid As Integer, trans As MySqlTransaction)` — INSERT ke `poin_ledger` (TIPE='VOID_EARN', JUMLAH_POIN negatif) dan UPDATE SALDO_POIN; pastikan SALDO_POIN tidak turun di bawah 0
  - Buat fungsi `AmbilSaldoPoin(kodePelanggan As String) As Integer` — query SALDO_POIN dari tbl_pelanggan
  - Buat fungsi `AmbilPoinEarnDariFaktur(noFaktur As String) As Integer` — query poin_ledger WHERE NO_REFERENSI=noFaktur AND TIPE='EARN'
  - Buat `Class ItemPoin` dengan properti `QtySatuan As Decimal` dan `TotalHarga As Decimal`
  - **Files:** `2Trans/ModuleLoyaltyPoin.vb`
  - **Requirement:** Req 1, Req 2, Req 5, Req 8

- [ ] 3. FormGeneralSetting — Tambah Section Aktifkan Poin
  - Edit `FormGeneralSetting.vb` — tambah dua entri baru ke `RoleComboList` di constructor: `('LblPoinAktif', CmbPoinAktif, defaultIndex=1)` dan `('LblPoinMekanisme', CmbPoinMekanisme, defaultIndex=0)`
  - Edit `FormGeneralSetting.Designer.vb` — tambah GroupBox baru `GBLoyaltyPoin` di bawah section yang ada, berisi: Label "Aktifkan Sistem Poin Loyalitas" + ComboBox `CmbPoinAktif` (items: "Tidak","Iya"), Label "Mekanisme Perolehan Poin" + ComboBox `CmbPoinMekanisme` (items: "Per Item (Qty)","Per Kelipatan Nominal")
  - Di `FormGeneralSetting.vb` — tambah event handler `CmbPoinAktif_SelectedIndexChanged` untuk show/hide `CmbPoinMekanisme` (hanya tampil saat Aktif = "Iya"), pola sama seperti `TerapkanVisibilitasBatasSatuan()`
  - Di `BtnSimpan_Click` — tambah blok simpan dua setting baru ke `hakaksesuser` (pola INSERT ... ON DUPLICATE KEY UPDATE yang sudah ada), lalu panggil `ModuleLoyaltyPoin.MuatKonfigurasi()` untuk refresh cache
  - Di `BacaCombobox()` — tambah pembacaan dua setting baru dari `hakaksesuser`
  - Tambah tooltip deskriptif untuk kedua label baru di `SetupTooltipLabel()`
  - **Files:** `1Master/FormGeneralSetting.vb`, `1Master/FormGeneralSetting.Designer.vb`
  - **Requirement:** Req 1

- [ ] 4. FormMasterPoin — Form Konfigurasi Earn Rate, Harga Poin Barang, dan Riwayat Poin
  - Buat file baru `FormMasterPoin.vb`, `FormMasterPoin.Designer.vb`, `FormMasterPoin.resx` di folder `1Master/`
  - Desain form dengan TabControl berisi tiga tab: "Konfigurasi Poin", "Harga Poin Barang", "Riwayat Poin Pelanggan"
  - **Tab 1 — Konfigurasi Poin:** NumericUpDown `NudPoinPerQty` (label: "Poin per 1 Qty Item"), NumericUpDown `NudKelipatanNominal` (label: "Kelipatan Nominal (Rp)"), NumericUpDown `NudMinimumRedeem` (label: "Minimum Poin untuk Redeem"), tombol Simpan dan Reset; show/hide NudPoinPerQty vs NudKelipatanNominal sesuai mekanisme yang dipilih di GeneralSetting
  - Validasi di BtnSimpan: tolak nilai ≤ 0 untuk semua field, tampilkan MessageBox deskriptif; simpan ke tabel `poin_config` dalam satu transaksi atomik; panggil `ModuleLoyaltyPoin.MuatKonfigurasi()` setelah simpan
  - **Tab 2 — Harga Poin Barang:** DataGridView `DgvPoinBarang` dengan kolom Kode Barang, Nama Barang, Harga Poin (editable), Aktif (CheckBox); TextBox pencarian nama/kode barang; tombol Simpan Harga Poin; load data dari JOIN `tbl_barang LEFT JOIN poin_barang`; simpan perubahan ke `poin_barang` (INSERT ... ON DUPLICATE KEY UPDATE)
  - **Tab 3 — Riwayat Poin Pelanggan:** ComboBox/TextBox pilih pelanggan (cari by nama/kode), Label tampil Saldo Poin terkini, DateTimePicker filter tanggal awal dan akhir, DataGridView `DgvRiwayatPoin` dengan kolom Tanggal, No Referensi, Tipe, Jumlah Poin, Saldo Setelah; tombol Tampilkan; query dari `poin_ledger` WHERE KODE_PELANGGAN = @kode AND CREATED_AT BETWEEN @dari AND @sampai ORDER BY CREATED_AT DESC
  - Daftarkan form di menu utama `FormUtama.vb` di bawah menu Master
  - **Files:** `1Master/FormMasterPoin.vb`, `1Master/FormMasterPoin.Designer.vb`, `1Master/FormMasterPoin.resx`, `0Form/FormUtama.vb`
  - **Requirement:** Req 1, Req 4, Req 7

- [ ] 5. Integrasi FormJual — Tampil Saldo Poin dan Catat EARN saat Simpan
  - Edit `FormJual.vb` — tambah Label `LblSaldoPoin` di area informasi pelanggan (dekat label nama pelanggan), teks default kosong, hanya visible saat `LP_Aktif = True`
  - Di event pemilihan pelanggan (saat TxtPelanggan/CmbPelanggan berubah) — panggil `ModuleLoyaltyPoin.AmbilSaldoPoin(kodePelanggan)` dan tampilkan di `LblSaldoPoin` dengan format "Poin: {saldo}"
  - Di prosedur simpan transaksi (setelah INSERT ke tabel `penjualan` dan `penjualan_detail`, sebelum `transaksi.Commit()`) — tambah blok: jika `LP_Aktif = True` AND pelanggan dipilih, bangun `List(Of ItemPoin)` dari item di grid, panggil `ModuleLoyaltyPoin.HitungPoinEarn(...)`, jika hasil > 0 panggil `ModuleLoyaltyPoin.CatatEarn(...)` dengan transaksi yang sama
  - Pastikan blok poin berada di dalam `Try...Catch` yang sama dengan transaksi penjualan — jika gagal, rollback seluruh transaksi dan tampilkan pesan error
  - Saat pelanggan dikosongkan/diganti, reset `LblSaldoPoin` ke kosong
  - **Files:** `2Trans/FormJual.vb`, `2Trans/FormJual.Designer.vb`
  - **Requirement:** Req 2

- [ ] 6. FormTukarPoin — Form Penukaran Poin dengan Barang
  - Buat file baru `FormTukarPoin.vb`, `FormTukarPoin.Designer.vb`, `FormTukarPoin.resx` di folder `2Trans/`
  - Desain form: TextBox/ComboBox pilih pelanggan (cari by nama/kode), Label `LblSaldoPoinTukar` tampil saldo, Label `LblStatusMinRedeem` tampil pesan jika saldo < minimum redeem
  - DataGridView `DgvBarangTukar` — load dari JOIN `poin_barang INNER JOIN tbl_barang` WHERE `poin_barang.AKTIF = 1`; kolom: Kode Barang, Nama Barang, Stok, Harga Poin, Qty (editable NumericUpDown), Total Poin
  - Label `LblTotalPoinDibutuhkan` dan `LblSisaPoinSetelah` — update real-time saat qty berubah
  - Tombol `BtnKonfirmasiTukar` — disabled jika saldo < minimum redeem ATAU total poin dibutuhkan > saldo
  - Di `BtnKonfirmasiTukar_Click`: generate nomor referensi format "TP-YYYYMMDD-XXXX" (query MAX dari poin_ledger hari ini + increment), buka transaksi DB, panggil `ModuleLoyaltyPoin.CatatRedeem(...)`, kurangi stok barang di `tbl_barang` (UPDATE stok), commit; jika gagal rollback dan tampilkan pesan error
  - Setelah konfirmasi berhasil: cetak bukti penukaran sederhana (GDI+ atau MessageBox dengan detail: nama pelanggan, barang ditukar, poin digunakan, sisa saldo), refresh saldo di form
  - Validasi: jika total poin dibutuhkan > saldo, tampilkan pesan kekurangan poin dan disable tombol konfirmasi
  - Daftarkan form di menu utama `FormUtama.vb` di bawah menu Transaksi
  - **Files:** `2Trans/FormTukarPoin.vb`, `2Trans/FormTukarPoin.Designer.vb`, `2Trans/FormTukarPoin.resx`, `0Form/FormUtama.vb`
  - **Requirement:** Req 3

- [ ] 7. Integrasi ModulePrinterJual — Cetak Poin di Struk
  - Edit `ModulePrinterJual.vb` — tambah dua variabel publik: `Jual_PoinDiperoleh As Integer` dan `Jual_SaldoPoinAkhir As Integer`
  - Di `MuatDataPenjualan()` — setelah `MuatHeaderPenjualan()`, tambah blok: jika `LP_Aktif = True` AND `Jual_IdPelanggan <> ""`, query `poin_ledger` WHERE NO_REFERENSI = noFaktur AND TIPE = 'EARN' untuk isi `Jual_PoinDiperoleh`; query `tbl_pelanggan.SALDO_POIN` untuk isi `Jual_SaldoPoinAkhir`; jika pelanggan kosong set keduanya ke 0
  - Edit `EscPosCetakjualThermalMatrik.vb` — di bagian footer sebelum garis penutup, tambah blok kondisional: jika `LP_Aktif = True` AND `Jual_IdPelanggan <> ""`, cetak separator, cetak "Saldo Poin  : {Jual_SaldoPoinAkhir}", jika `Jual_PoinDiperoleh > 0` cetak "Poin Diperoleh: +{Jual_PoinDiperoleh}" (format ESC/POS raw bytes)
  - Edit `GdiCetakjualThermalMatrik.vb` — di metode `DrawFooter()` atau setara, tambah blok kondisional yang sama untuk GDI+ (menggunakan `e.Graphics.DrawString()`)
  - Pastikan blok poin tidak muncul sama sekali jika `Jual_IdPelanggan = ""` atau `LP_Aktif = False`
  - **Files:** `6Print/CetakPenjualan/ModulePrinterJual.vb`, `6Print/CetakPenjualan/EscPosCetakjualThermalMatrik.vb`, `6Print/CetakPenjualan/GdiCetakjualThermalMatrik.vb`
  - **Requirement:** Req 6

- [ ] 8. Integrasi FormReturPenjualan — Void EARN saat Retur
  - Edit `FormReturPenjualan.vb` — di prosedur simpan retur (setelah INSERT ke tabel retur, sebelum commit), tambah blok void poin
  - Ambil kode pelanggan dari header transaksi penjualan asal; jika kosong, skip void poin
  - Panggil `ModuleLoyaltyPoin.AmbilPoinEarnDariFaktur(noFakturAsal)` untuk cek apakah ada EARN; jika 0, skip
  - Untuk retur penuh: panggil `ModuleLoyaltyPoin.CatatVoidEarn(kodePelanggan, noFakturAsal, jumlahPoinEarnAsal, trans)` dengan jumlah poin = seluruh EARN dari faktur tersebut
  - Untuk retur parsial: hitung poin proporsional = `Floor(poinEarnAsal × (nilaiItemRetur / totalNilaiFakturAsal))`; panggil `CatatVoidEarn` dengan jumlah proporsional
  - `CatatVoidEarn` sudah menangani batas minimum 0 (tidak boleh negatif) — pastikan logika ini aktif
  - Seluruh blok void poin berada dalam transaksi DB yang sama dengan penyimpanan retur; jika gagal, rollback seluruh transaksi
  - **Files:** `2Trans/FormReturPenjualan.vb`
  - **Requirement:** Req 5

- [ ] 9. Testing dan Verifikasi End-to-End
  - Jalankan migrasi `32_loyalty_point_schema.sql` di database development; verifikasi tabel `poin_config`, `poin_ledger`, `poin_barang` terbentuk dan kolom `SALDO_POIN` ada di `tbl_pelanggan`
  - Test skenario mekanisme Per Item: aktifkan poin, set mekanisme Per Item, lakukan transaksi penjualan dengan pelanggan terdaftar, verifikasi baris EARN di `poin_ledger` dan SALDO_POIN di `tbl_pelanggan` bertambah sesuai formula
  - Test skenario mekanisme Per Kelipatan Nominal: ganti mekanisme, lakukan transaksi, verifikasi kalkulasi floor(grandTotal / kelipatan) benar
  - Test saldo poin tampil di FormJual saat pelanggan dipilih dan hilang saat pelanggan dikosongkan
  - Test FormTukarPoin: pilih pelanggan, pilih barang, konfirmasi — verifikasi baris REDEEM di `poin_ledger`, SALDO_POIN berkurang, stok barang berkurang
  - Test blokir redeem saat saldo < minimum redeem (tombol konfirmasi harus disabled)
  - Test cetak struk: verifikasi baris "Saldo Poin" dan "Poin Diperoleh" muncul di output ESC/POS dan GDI+ untuk pelanggan terdaftar, tidak muncul untuk transaksi tanpa pelanggan
  - Test retur penuh: retur transaksi yang ada EARN, verifikasi baris VOID_EARN di `poin_ledger` dan SALDO_POIN berkurang; verifikasi SALDO_POIN tidak pernah negatif
  - Test retur parsial: verifikasi poin void proporsional dihitung dengan benar
  - Test atomisitas: simulasikan kegagalan DB saat simpan penjualan, verifikasi tidak ada baris EARN tersimpan (rollback bersih)
  - Verifikasi rekonstruksi saldo: `SELECT SUM(JUMLAH_POIN) FROM poin_ledger WHERE KODE_PELANGGAN = @kode` harus sama dengan `SALDO_POIN` di `tbl_pelanggan` untuk semua pelanggan
  - **Files:** `Database/32_loyalty_point_schema.sql`, semua file yang dimodifikasi di task 2–8
  - **Requirement:** Req 1, Req 2, Req 3, Req 4, Req 5, Req 6, Req 7, Req 8

## Task Dependency Graph

```json
{
  "waves": [
    { "wave": 1, "tasks": ["1"] },
    { "wave": 2, "tasks": ["2"] },
    { "wave": 3, "tasks": ["3", "4", "5", "6", "7", "8"] },
    { "wave": 4, "tasks": ["9"] }
  ]
}
```

Task 1 harus selesai sebelum task lainnya karena semua modul bergantung pada skema database baru. Task 2 (ModuleLoyaltyPoin) harus selesai sebelum task 3–8 karena semua form memanggil fungsi dari modul ini. Task 9 dapat dimulai setelah semua task 1–8 selesai.

## Notes

- Nomor migrasi database dilanjutkan dari `31_tambah_kolom_sumber_surat_jalan_detail.sql` → file baru adalah `32_loyalty_point_schema.sql`
- Setting aktif/mekanisme poin disimpan di tabel `hakaksesuser` (pola yang sudah ada di FormGeneralSetting) dengan Role key `'PoinAktif'` dan `'PoinMekanisme'`; konfigurasi earn rate (nilai numerik) disimpan di tabel `poin_config` yang baru
- FormJual **tidak diubah strukturnya** — hanya ditambah label saldo poin dan blok pencatatan EARN di prosedur simpan yang sudah ada
- Semua operasi poin (EARN, REDEEM, VOID_EARN) harus berada dalam transaksi DB yang sama dengan transaksi induknya (penjualan/penukaran/retur) untuk menjamin atomisitas
- SALDO_POIN di `tbl_pelanggan` tidak boleh pernah bernilai negatif — `CatatVoidEarn` harus membatasi pengurangan maksimal sebesar saldo yang tersedia
- Pola pembuatan form baru mengikuti konvensi yang ada: file `.vb` + `.Designer.vb` + `.resx` di folder yang sesuai
