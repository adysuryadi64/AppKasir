---
inclusion: always
---

# Referensi COA Lengkap — tbl_datareferensi (db_kasirlancar)

> Dibaca langsung dari database. Wajib dipakai sebagai acuan setiap kali menulis kode yang menyentuh JurnalUmum, tbl_datareferensi, atau akun COA.
> Jangan pernah hardcode kode akun tanpa mencocokkan tabel ini terlebih dahulu.

## Struktur Kolom

| Kolom | Keterangan |
|---|---|
| `KODE_AKUN` | Primary key, format `XX.XX.XXX` |
| `NAMA_AKUN` | Nama lengkap akun |
| `TYPE_AKUN` | Tipe akun (dipakai sebagai filter dropdown) |
| `SUB_AKUN` | Kelompok besar: AKTIVA / PASIVA / LABA / RUGI / LABA RUGI |
| `AKUN_DK` | Saldo normal: `DEBET` atau `KREDIT` |
| `STATUS` | `Terkunci` = tidak bisa dihapus user, `NULL` = bebas |
| `KETERANGAN` | Penjelasan akuntansi lengkap fungsi akun — wajib dibaca sebelum memetakan akun ke transaksi |

---

## Daftar Lengkap COA

### AKTIVA

| KODE_AKUN | NAMA_AKUN | TYPE_AKUN | AKUN_DK | STATUS | KETERANGAN |
|---|---|---|---|---|---|
| 01.01.001 | KAS DI TOKO | KAS | DEBET | Terkunci | Uang tunai fisik di mesin kasir/laci toko. Untuk transaksi penjualan tunai harian dan pengeluaran kecil. Rekonsiliasi fisik setiap tutup kasir. |
| 01.01.002 | KAS DI GUDANG | KAS | DEBET | Terkunci | Petty cash di gudang untuk operasional harian (beli bahan pembantu, bayar upah lepas). Sistem imprest atau fluktuasi. |
| 01.01.003 | KAS KIRIMAN TOKO | KAS | DEBET | NULL | Uang tunai sudah keluar dari bank/kas gudang tapi belum diterima fisik oleh toko. Akun transit antar lokasi, harus segera direkonsiliasi. |
| 01.01.004 | KAS KIRIMAN GUDANG | KAS | DEBET | NULL | Uang tunai dalam perjalanan menuju gudang. Akun transit, saldo harus nol setelah periode tertentu. |
| 01.02.001 | TRANSFER BANK | BANK | DEBET | NULL | Seluruh rekening bank perusahaan (giro, tabungan, deposito). Semua penerimaan/pengeluaran via transfer, kliring, setoran tunai. Rekonsiliasi bank bulanan wajib. |
| 01.03.001 | PIUTANG USAHA | PIUTANG | DEBET | Terkunci | Tagihan penjualan kredit kepada pelanggan/reseller jatuh tempo 12 bulan. Disajikan neto setelah cadangan kerugian piutang. |
| 01.03.002 | PIUTANG KARYAWAN | PIUTANG | DEBET | Terkunci | Uang dipinjam karyawan (kas bon) yang dipotong dari gaji atau dicicil. Jangka pendek, tidak boleh dikapitalisasi sebagai beban. |
| 01.04.001 | PERSEDIAAN BARANG | A LANCAR | DEBET | Terkunci | Nilai barang dagang tersedia di gudang/toko (harga perolehan). Sistem perpetual atau periodik. Sesuai PSAK 14. |
| 01.04.002 | TAGIHAN / SALDO PIUTANG | A LANCAR | DEBET | Terkunci | Tagihan jangka pendek selain piutang usaha & karyawan. Contoh: uang muka pembelian, klaim asuransi, deposit sewa. |
| 01.04.003 | PERLENGKAPAN KANTOR | A LANCAR | DEBET | NULL | Stok perlengkapan kantor belum terpakai. Saat dipakai diakui sebagai beban perlengkapan. |
| 01.05.001 | PPN MASUKAN | PAJAK AL | DEBET | NULL | PPN dibayar saat pembelian barang/jasa kena pajak. Dapat dikreditkan (di-offset) dengan PPN Keluaran. |
| 02.01.001 | TANAH | A TETAP | DEBET | NULL | Aset tetap tanah (harga perolehan + biaya hak). Tidak disusutkan. |
| 02.01.002 | GEDUNG | A TETAP | DEBET | NULL | Aset tetap bangunan/gudang/toko. Disusutkan garis lurus umumnya 20 tahun. |
| 02.01.003 | INVENTARIS | A TETAP | DEBET | NULL | Aset tetap perabotan, meja, AC, komputer, mesin kantor (nilai signifikan). Disusutkan 4-8 tahun. |
| 02.01.004 | KENDARAAN | A TETAP | DEBET | NULL | Aset tetap kendaraan operasional. Disusutkan 4-8 tahun. BPKB atas nama perusahaan. |
| 02.02.002 | AKUM. PENY. GEDUNG | AKM PENY. | KREDIT | NULL | Akun kontra aset pengurang nilai gedung. Akumulasi beban penyusutan gedung sejak perolehan. |
| 02.02.003 | AKUM. PENY. INVENTARIS | AKM PENY. | KREDIT | NULL | Akun kontra aset pengurang nilai inventaris. Akumulasi beban penyusutan inventaris. |
| 02.02.004 | AKUM. PENY. KENDARAAN | AKM PENY. | KREDIT | NULL | Akun kontra aset pengurang nilai kendaraan. Akumulasi beban penyusutan kendaraan. |

### PASIVA

| KODE_AKUN | NAMA_AKUN | TYPE_AKUN | AKUN_DK | STATUS | KETERANGAN |
|---|---|---|---|---|---|
| 03.01.001 | HUTANG BELANJA | HUTANG | KREDIT | Terkunci | Hutang usaha kepada supplier atas pembelian barang dagang kredit (jatuh tempo 12 bulan). Dicatat saat faktur diterima. |
| 03.01.002 | HUTANG USAHA | HUTANG | KREDIT | NULL | Hutang operasional selain pembelian barang dagang. Contoh: hutang jasa perbaikan, hutang konsultan. |
| 03.01.003 | HUTANG LAIN LAIN | HUTANG | KREDIT | NULL | Hutang non-operasional jangka pendek. Contoh: hutang pihak ketiga bukan pemasok utama, uang muka pelanggan. |
| 03.02.001 | HUTANG PAJAK | BEBAN | KREDIT | Terkunci | Hutang PPh yang masih harus disetor: Pasal 21, 22, 23, 25, 29 (badan). |
| 03.02.002 | HUTANG BANK JANGKA PENDEK | BEBAN | KREDIT | NULL | Pinjaman bank jatuh tempo < 1 tahun (termasuk cicilan pokok utang jangka panjang yang jatuh tempo tahun berjalan). |
| 03.02.003 | HUTANG BANK JANGKA PANJANG | BEBAN | KREDIT | NULL | Pinjaman bank jatuh tempo > 1 tahun. Disajikan setelah dikurangi bagian jatuh tempo dalam 1 tahun. |
| 03.02.004 | PPN KELUARAN | PAJAK | KREDIT | NULL | PPN dipungut dari pembeli saat penjualan barang/jasa kena pajak. Disetor ke negara selisihnya dengan PPN Masukan. |
| 03.03.001 | DANA KESEJAHTERAAN | SOSIAL | KREDIT | NULL | Dana sosial karyawan (koperasi, iuran kegiatan) dipotong dari gaji atau kas perusahaan. Bersifat hutang kepada karyawan. |
| 04.01.001 | MODAL | EKUITAS | KREDIT | Terkunci | Modal dasar disetor pemilik (setoran awal dan tambahan yang disahkan). Akun permanen. |
| 04.01.002 | MODAL PEMILIK | EKUITAS | KREDIT | Terkunci | Penyesuaian modal pemilik (revaluasi, tambahan investasi non-tunai, koreksi modal). Bukan untuk laba/rugi. |
| 04.01.003 | REKENING KORAN PUSAT | EKUITAS | KREDIT | Terkunci | Penampung transfer aset internal antar cabang/toko tanpa kas. Pada konsolidasi saldo antar cabang saling menghapus. |
| 04.02.001 | PRIVE PEMILIK | PRIVE | DEBET | Terkunci | Pengambilan uang/aset perusahaan untuk keperluan pribadi pemilik. Mengurangi ekuitas. Ditutup ke modal akhir tahun. |

### LABA RUGI

| KODE_AKUN | NAMA_AKUN | TYPE_AKUN | SUB_AKUN | AKUN_DK | STATUS | KETERANGAN |
|---|---|---|---|---|---|---|
| 05.01.001 | LABA RUGI BERJALAN | LABA RUGI | LABA RUGI | KREDIT | Terkunci | Penampung laba/rugi tahun berjalan. Saldo kredit = laba, debet = rugi. Ditutup ke modal pada akhir periode. |

### PENJUALAN (JENIS_AKUN = PENJUALAN, SUB_AKUN = LABA)

> Akun pendapatan dan kontra-pendapatan. Formula laporan L/R: `LABA+KREDIT → positif`, `LABA+DEBET → negatif`.

| KODE_AKUN | NAMA_AKUN | TYPE_AKUN | SUB_AKUN | AKUN_DK | STATUS | KETERANGAN |
|---|---|---|---|---|---|---|
| 05.02.001 | PENJUALAN | PEND. KOTOR | LABA | KREDIT | Terkunci | Pendapatan kotor dari penjualan barang dagang (belum dikurangi retur, diskon, potongan). Akun nominal. |
| 05.03.001 | RETUR PENJUALAN | RETUR PEND. | LABA | DEBET | Terkunci | Pengembalian barang oleh pelanggan karena cacat/tidak sesuai. Mengurangi penjualan kotor. |
| 05.04.001 | POTONGAN DISKON PENJUALAN | DISKON PEND. | LABA | DEBET | Terkunci | Potongan harga tunai kepada pelanggan. Akun kontra pendapatan. |

### HPP DAN BIAYA POKOK (JENIS_AKUN = HPP, SUB_AKUN = RUGI)

> Akun beban pokok dan kontra-beban. Formula laporan L/R: `RUGI+DEBET → positif (beban)`, `RUGI+KREDIT → negatif (kontra-beban)`.

| KODE_AKUN | NAMA_AKUN | TYPE_AKUN | SUB_AKUN | AKUN_DK | STATUS | KETERANGAN |
|---|---|---|---|---|---|---|
| 06.01.001 | HPP POKOK PENJUALAN | HPP POKOK | RUGI | DEBET | Terkunci | Harga perolehan barang terjual (COGS). Dihitung dari persediaan awal + pembelian bersih - persediaan akhir. |
| 06.02.001 | BIAYA KIRIM PEMBELIAN | ANGKUT BELI | RUGI | DEBET | NULL | Biaya angkut pembelian (freight in) untuk mendatangkan barang ke gudang. Menambah nilai persediaan/HPP. |
| 06.03.001 | BIAYA KIRIM PENJUALAN | ANGKUT JUAL | RUGI | DEBET | NULL | Ongkos kirim (freight out) ditanggung perusahaan untuk mengirim barang ke pelanggan. |
| 06.04.001 | PENYESUAIAN STOK MINUS | PENY. STOK | RUGI | DEBET | Terkunci | Selisih kurang (rugi) saat stok opname: barang hilang, rusak, expired. |
| 06.04.002 | PENYESUAIAN HARGA POKOK | PENY. STOK | RUGI | DEBET | NULL | Selisih nilai persediaan akibat perubahan harga pokok barang (harga terbaru atau average cost). Menjaga neraca tetap seimbang saat harga pokok diupdate saat pembelian. |
| 06.05.001 | POTONGAN DISKON PEMBELIAN | DISKON BELI | RUGI | KREDIT | NULL | Diskon dari supplier karena pembayaran lebih awal. Mengurangi HPP. Akun kontra-HPP (kredit). |
| 06.06.001 | RETUR PEMBELIAN | RETUR BELI | RUGI | KREDIT | NULL | Pengembalian barang ke supplier karena cacat/tidak sesuai. Mengurangi nilai pembelian/HPP. |

### PENDAPATAN LAIN (JENIS_AKUN = PENDAPATAN LAIN, SUB_AKUN = LABA)

| KODE_AKUN | NAMA_AKUN | TYPE_AKUN | SUB_AKUN | AKUN_DK | STATUS | KETERANGAN |
|---|---|---|---|---|---|---|
| 08.01.001 | PENDAPATAN BUNGA BANK | PEND. BUNGA | LABA | KREDIT | NULL | Pendapatan bunga dari saldo rekening giro, tabungan, atau deposito. |
| 08.01.002 | PENDAPATAN LAIN LAIN | PEND. LAIN | LABA | KREDIT | Terkunci | Pendapatan non-operasional: laba jual aset tetap, klaim asuransi, komplain supplier, hibah, dll. |

### BEBAN OPERASIONAL (JENIS_AKUN = BIAYA, SUB_AKUN = RUGI)

| KODE_AKUN | NAMA_AKUN | TYPE_AKUN | SUB_AKUN | AKUN_DK | STATUS | KETERANGAN |
|---|---|---|---|---|---|---|
| 07.01.001 | BEBAN GAJI KARYAWAN | BIAYA | RUGI | DEBET | Terkunci | Gaji, upah, tunjangan, bonus, dan THR karyawan tetap & harian. |
| 07.01.002 | BEBAN PERLENGKAPAN ATK | BIAYA | RUGI | DEBET | NULL | Biaya pemakaian alat tulis kantor yang sudah habis pakai dalam periode berjalan. |
| 07.01.003 | BEBAN LISTRIK & AIR | BIAYA | RUGI | DEBET | NULL | Biaya utilitas: listrik (PLN), air (PDAM), telepon, internet untuk operasional. |
| 07.01.004 | BEBAN BBM DAN ONGKOS KIRIM | BIAYA | RUGI | DEBET | NULL | BBM kendaraan operasional dan ongkos kirim tidak terkait langsung dengan penjualan. |
| 07.01.005 | BEBAN PEMELIHARAAN GEDUNG | BIAYA | RUGI | DEBET | NULL | Biaya perbaikan, perawatan, cat, perpipaan, kebersihan gedung. Bukan biaya penyusutan. |
| 07.01.007 | BEBAN PENYUSUTAN GEDUNG | BIAYA | RUGI | DEBET | NULL | Beban penyusutan aset tetap gedung per periode (metode garis lurus). |
| 07.01.008 | BEBAN PENYUSUTAN INVENTARIS | BIAYA | RUGI | DEBET | NULL | Beban penyusutan inventaris/furniture per periode. |
| 07.01.009 | BEBAN PENYUSUTAN KENDARAAN | BIAYA | RUGI | DEBET | NULL | Beban penyusutan kendaraan operasional per periode. |
| 07.01.011 | BEBAN ADM DAN BUNGA BANK | BIAYA | RUGI | DEBET | NULL | Biaya administrasi bank (fee bulanan, transfer, materai) dan bunga pinjaman bank. |
| 07.01.012 | BEBAN ADM DAN UMUM LAINNYA | BIAYA | RUGI | DEBET | NULL | Beban operasional kecil lain-lain yang tidak material atau tidak sering terjadi. |

### PAJAK (JENIS_AKUN = PAJAK, SUB_AKUN = RUGI)

| KODE_AKUN | NAMA_AKUN | TYPE_AKUN | SUB_AKUN | AKUN_DK | STATUS | KETERANGAN |
|---|---|---|---|---|---|---|
| 09.01.001 | PAJAK PENGHASILAN | B PAJAK | RUGI | DEBET | NULL | Beban PPh Badan terutang untuk tahun berjalan (PPh 25/29). |

---

## Formula Laba Rugi — Wajib Dipakai di Semua Kalkulasi

```
Laba Bersih = LABA+KREDIT - LABA+DEBET - RUGI+DEBET + RUGI+KREDIT
```

Atau dalam SQL:
```sql
SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END)
- SUM(CASE WHEN SUB_AKUN='LABA' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END)
- SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='DEBET'  THEN SALDO_AKHIR ELSE 0 END)
+ SUM(CASE WHEN SUB_AKUN='RUGI' AND AKUN_DK='KREDIT' THEN SALDO_AKHIR ELSE 0 END)
```

> ⚠️ **DILARANG** menggunakan `WHEN SUB_AKUN='RUGI' THEN SALDO_AKHIR` tanpa filter `AKUN_DK` — akan salah menghitung kontra-beban (06.05, 06.06) yang `RUGI+KREDIT` seharusnya menambah laba.

---

## Pemetaan Akun per Transaksi

### Penjualan
| Jurnal | Akun D | Akun K |
|---|---|---|
| Bayar tunai | 01.01.001/002 (dari tbl_perusahaan) | — |
| Bayar transfer | 01.02.001 (dari tbl_perusahaan) | — |
| Piutang | 01.04.002 (dari tbl_perusahaan) | — |
| Diskon item | 05.04.001 | — |
| HPP | 06.01.001 | — |
| Pendapatan kotor | — | 05.02.001 |
| Keluar persediaan | — | 01.04.001 (dari tbl_perusahaan) |
| Hutang pajak | — | 03.02.001 |
| Biaya kirim | — | 08.01.002 |

### Stok Opname
| Kondisi | Akun D | Akun K |
|---|---|---|
| Selisih negatif (stok kurang) | 06.04.001 | 01.04.001 (dari tbl_perusahaan) |
| Selisih positif (stok lebih) | 01.04.001 (dari tbl_perusahaan) | 06.04.001 |

### Transfer Stok (jika ada selisih nilai)
| Kondisi | Akun D | Akun K |
|---|---|---|
| Nilai masuk > keluar | 01.04.001 (dari tbl_perusahaan) | 06.04.001 |
| Nilai masuk < keluar | 06.04.001 | 01.04.001 (dari tbl_perusahaan) |

---

## Aturan Penting

1. **Akun persediaan (`01.04.001`) TIDAK hardcode** — selalu baca dari `tbl_perusahaan.KODE_REK_BARANG`
2. **Akun kas (`01.01.001`/`01.01.002`) TIDAK hardcode** — dari dropdown user (step3) atau `tbl_perusahaan.KODE_REK_JUAL_TOKO/GUDANG`
3. **Akun transfer (`01.02.001`) TIDAK hardcode** — dari dropdown user atau `tbl_perusahaan.KODE_REK_TRANSFER_JUAL`
4. **Akun piutang (`01.04.002`) TIDAK hardcode** — dari `tbl_perusahaan.KODE_REK_PIUTANG_JUAL`
5. Akun yang boleh hardcode di SP: `05.02.001`, `05.04.001`, `06.01.001`, `06.04.001`, `03.02.001`, `08.01.002` — karena tidak ada di tbl_perusahaan dan tidak berubah per toko
6. **STATUS kolom**: `Terkunci` = tidak bisa dihapus, tapi **bukan berarti aktif** — filter dropdown jangan pakai `STATUS = 'Aktif'` karena tidak ada nilai tersebut
7. Untuk dropdown akun di Flutter: query `WHERE TYPE_AKUN LIKE '%KAS%'` untuk kas, `WHERE TYPE_AKUN LIKE '%BANK%'` untuk transfer

---

## ⚠️ WAJIB — Nilai Dinamis dari tbl_perusahaan

> **Kesalahan memakai akun COA yang salah bisa mengakibatkan laporan keuangan keliru secara fatal.**
> Contoh: seharusnya laba Rp 1 miliar, bisa tercatat rugi Rp 5 miliar hanya karena akun tertukar.

**Setiap kali menulis jurnal atau SP yang menyentuh akun COA, wajib cek tabel ini terlebih dahulu.**
Jangan pernah hardcode kode akun yang ada di kolom berikut — selalu baca dari `tbl_perusahaan` saat runtime.

Di VB.NET, semua nilai ini dimuat ke variabel global di `ModuleVariabel` saat aplikasi start.
Di PHP/Flutter, wajib query `tbl_perusahaan` atau ambil dari API sebelum membuat jurnal.

### Daftar Lengkap Kolom Dinamis tbl_perusahaan

| Kolom di tbl_perusahaan | Variabel Global VB | Fungsi / Dipakai untuk Transaksi Apa |
|---|---|---|
| `KODE_REK_BARANG` | `KODE_REK_BARANG` | Persediaan barang — D/K saat penjualan, pembelian, opname, transfer stok |
| `LAWAN_KODE_REK_BARANG` | `LAWAN_KODE_REK_BARANG` | Lawan akun persediaan (penyesuaian harga pokok) |
| `KODE_REK_BELI_TOKO` | `Kode_rek_Beli_toko` | Kas/akun debet saat pembelian di toko |
| `KODE_REK_BELI_GUDANG` | `Kode_rek_Beli_Gudang` | Kas/akun debet saat pembelian di gudang |
| `KODE_REK_JUAL_TOKO` | `Kode_rek_Jual_Toko` | Kas masuk saat penjualan tunai di toko |
| `KODE_REK_JUAL_GUDANG` | `Kode_rek_Jual_Gudang` | Kas masuk saat penjualan tunai di gudang |
| `KODE_REK_HUTANG_BELI` | `Kode_rek_Hutang_Beli` | Hutang dagang saat pembelian kredit |
| `KODE_REK_PIUTANG_JUAL` | `Kode_rek_Piutang_Jual` | Piutang dagang saat penjualan kredit |
| `KODE_REK_RETUR_PEMBELIAN_TOKO` | `Kode_rek_Retur_Pembelian_Toko` | Akun retur pembelian di toko |
| `KODE_REK_RETUR_PENJUALAN_TOKO` | `Kode_rek_Retur_Penjualan_Toko` | Akun retur penjualan di toko |
| `KODE_REK_RETUR_PEMBELIAN_GUDANG` | `Kode_rek_Retur_Pembelian_Gudang` | Akun retur pembelian di gudang |
| `KODE_REK_RETUR_PENJUALAN_GUDANG` | `Kode_rek_Retur_Penjualan_Gudang` | Akun retur penjualan di gudang |
| `KODE_REK_BON_KARYAWAN` | `Kode_rek_Bon_Karyawan` | Piutang karyawan saat kasbon |
| `KODE_REK_GAJI_KARYAWAN` | `Kode_rek_Gaji_Karyawan` | Beban gaji saat pembayaran gaji |
| `KODE_REK_BAYAR_HUTANG` | `Kode_rek_Bayar_Hutang` | Kas/bank keluar saat bayar hutang ke supplier |
| `KODE_REK_BAYAR_PIUTANG` | `Kode_rek_Bayar_Piutang` | Kas/bank masuk saat terima bayar piutang dari pelanggan |
| `KODE_REK_TRANSFER_JUAL` | `Kode_rek_Transfer_Jual` | Rekening bank untuk penjualan via transfer |

### Cara Pakai yang Benar

**VB.NET** — gunakan variabel global yang sudah dimuat saat startup:
```vb
' ✅ Benar — pakai variabel global dari ModuleVariabel
cmd.Parameters.AddWithValue("@akun_kas", Kode_rek_Jual_Toko)
cmd.Parameters.AddWithValue("@akun_persediaan", KODE_REK_BARANG)

' ❌ DILARANG — hardcode kode akun yang seharusnya dinamis
cmd.Parameters.AddWithValue("@akun_kas", "01.01.001")
cmd.Parameters.AddWithValue("@akun_persediaan", "01.04.001")
```

**PHP / Stored Procedure** — query `tbl_perusahaan` dulu, baru pakai nilainya:
```php
// ✅ Benar — ambil dari tbl_perusahaan
$perusahaan = DB::table('tbl_perusahaan')->first();
$akunKas = $perusahaan->KODE_REK_JUAL_TOKO;

// ❌ DILARANG
$akunKas = '01.01.001';
```

**Flutter** — akun dinamis wajib dikirim dari backend atau disimpan di state setelah login:
```dart
// ✅ Benar — akun dari data perusahaan yang sudah di-fetch
final akunKas = perusahaan['KODE_REK_JUAL_TOKO'];

// ❌ DILARANG
const akunKas = '01.01.001';
```
