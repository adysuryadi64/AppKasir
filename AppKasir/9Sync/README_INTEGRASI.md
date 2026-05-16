# Sistem Sinkronisasi Multi-Toko — Panduan Lengkap

Sistem ini menghubungkan MySQL lokal tiap toko dengan Supabase sebagai pusat data cloud.
Konsep: **offline-first** — toko tetap berjalan tanpa internet, sync dilakukan saat koneksi tersedia.

---

## Arsitektur

```
[Toko A - MySQL Lokal]              [Supabase Cloud]              [Toko B - MySQL Lokal]
        ↕  upload/download                  ↕  upload/download
   tbl_barang                         barang_master                  tbl_barang
   tbl_pelanggan                      pelanggan_master               tbl_pelanggan
   tbl_supliyer                       supliyer_master                tbl_supliyer
   tbl_kategori                       kategori_master                tbl_kategori
   tbl_satuan                         satuan_master                  tbl_satuan
   tbl_merk                           merk_master                    tbl_merk
   tbl_armada                         armada_master                  tbl_armada
   tbl_cabang                         cabang_master                  tbl_cabang
                                      transfer_barang_cloud
                                      sync_conflict_log
                                      ─────────────────────
                                      [Laporan — READ ONLY]
                                      stok_per_toko
                                      hutang_supliyer_snapshot
                                      piutang_pelanggan_snapshot
                                      karyawan_snapshot
                                      gaji_ringkasan_snapshot
                                      coa_snapshot
                                      ─────────────────────
                                      [View Laporan]
                                      v_laporan_barang
                                      v_stok_detail_per_toko
                                      v_hutang_supliyer_total
                                      v_piutang_pelanggan_total
                                      v_neraca / v_laba_rugi
                                      v_ringkasan_laba_rugi
                                      ...dan 6 view lainnya
                                              ↑
                                    [Aplikasi Laporan Flutter]
```

---

## 1. Setup Database (jalankan sekali)

### MySQL Lokal — urutan eksekusi:

```sql
USE nama_database_kamu;
SOURCE Database/02_migrasi_sync_setup_supabase.sql;  -- kolom sync + tabel queue/log/config
SOURCE Database/01_migrasi_kolom.sql;                -- updated_at, sync_id, id_cloud, is_dirty, version
SOURCE Database/03_migrasi_index.sql;                -- index performa
```

### Supabase — urutan eksekusi di SQL Editor:

```
1. Database/supabase_setup.sql   — tabel master + sync + transfer + cabang_master
2. Database/supabase_laporan.sql — tabel snapshot + 12 view laporan
```

---

## 2. Tabel di Supabase

### Tabel Sinkronisasi Master
| Tabel | Sumber MySQL | Keterangan |
|---|---|---|
| `barang_master` | `tbl_barang` | Semua field master kecuali stok |
| `kategori_master` | `tbl_kategori` | |
| `satuan_master` | `tbl_satuan` | |
| `merk_master` | `tbl_merk` | |
| `supliyer_master` | `tbl_supliyer` | Tanpa saldo hutang |
| `pelanggan_master` | `tbl_pelanggan` | Tanpa saldo piutang |
| `armada_master` | `tbl_armada` | |
| `cabang_master` | `tbl_cabang` | Identitas cabang, kode unik per device |
| `transfer_barang_cloud` | — | Transfer barang antar toko |
| `sync_conflict_log` | — | Log konflik versi |

### Tabel Snapshot Laporan (upload saat sync, read-only untuk laporan)
| Tabel | Sumber MySQL | Keterangan |
|---|---|---|
| `stok_per_toko` | `tbl_barang` | Stok toko + gudang lengkap per toko |
| `hutang_supliyer_snapshot` | `tbl_supliyer` | Saldo hutang per toko |
| `piutang_pelanggan_snapshot` | `tbl_pelanggan` | Saldo piutang per toko |
| `karyawan_snapshot` | `tbl_karyawan` | Data + saldo bon karyawan |
| `gaji_ringkasan_snapshot` | `gaji_karyawan` | Ringkasan gaji bulan terakhir |
| `coa_snapshot` | `tbl_datareferensi` | COA + saldo neraca & laba rugi |

### View Laporan (akses via REST API)
| View | Keterangan |
|---|---|
| `v_laporan_barang` | Master barang + stok total semua toko + nilai stok |
| `v_stok_detail_per_toko` | Stok toko & gudang dipisah per toko |
| `v_hutang_supliyer` | Hutang supplier per toko |
| `v_hutang_supliyer_total` | Hutang supplier diringkas semua toko |
| `v_piutang_pelanggan` | Piutang pelanggan per toko |
| `v_piutang_pelanggan_total` | Piutang pelanggan diringkas semua toko |
| `v_karyawan` | Data karyawan semua toko |
| `v_gaji_ringkasan` | Gaji per karyawan per bulan |
| `v_gaji_total_per_bulan` | Total gaji per bulan per toko |
| `v_neraca` | Akun neraca per toko |
| `v_laba_rugi` | Akun laba rugi per toko |
| `v_ringkasan_laba_rugi` | Ringkasan laba bersih per toko |

---

## 3. Konfigurasi Aplikasi

Tambahkan di AppConfig / app.config:

```
SupabaseUrl = https://xxxx.supabase.co
SupabaseKey = eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

> `kode_cabang` tidak lagi dikonfigurasi manual — dibuat otomatis oleh sistem (lihat bagian 13).

Inisialisasi di `FormUtama` atau `Form_Load` utama:

```vb
SupabaseHelper.Init(
    AppConfig.Instance.GetValue(Of String)("SupabaseUrl", ""),
    AppConfig.Instance.GetValue(Of String)("SupabaseKey", "")
)
```

---

## 4. Kolom Tambahan di MySQL Lokal

| Kolom | Tipe | Keterangan |
|---|---|---|
| `id_cloud` | VARCHAR(50) | UUID dari Supabase setelah upload berhasil |
| `updated_at` | DATETIME | Auto-update saat data berubah |
| `updated_by` | VARCHAR(50) | User yang terakhir ubah |
| `is_dirty` | TINYINT | 1 = ada perubahan belum diupload |
| `version` | INT | Nomor versi untuk conflict handling |
| `sync_id` | VARCHAR(36) | UUID unik global (dibuat lokal) |

Tabel yang mendapat kolom ini:
`tbl_barang`, `tbl_pelanggan`, `tbl_supliyer`, `tbl_kategori`, `tbl_satuan`, `tbl_merk`, `tbl_armada`, `tbl_cabang`

---

## 5. Integrasi ke Form Master

Setiap kali data master disimpan, panggil `SyncTrigger` setelah `transaction.Commit()`:

```vb
' FormBarang
SyncTrigger.BarangBerubah(TxtKodeBarang.Text, "INSERT", ModuleVariabel.NamaUser)
SyncTrigger.BarangBerubah(TxtKodeBarang.Text, "UPDATE", ModuleVariabel.NamaUser)

' Form master lainnya — pola sama
SyncTrigger.MasterBerubah("tbl_pelanggan", TxtKode.Text, aksi, ModuleVariabel.NamaUser)
SyncTrigger.MasterBerubah("tbl_supliyer",  TxtKode.Text, aksi, ModuleVariabel.NamaUser)
SyncTrigger.MasterBerubah("tbl_kategori",  TxtKode.Text, aksi, ModuleVariabel.NamaUser)
SyncTrigger.MasterBerubah("tbl_satuan",    TxtKode.Text, aksi, ModuleVariabel.NamaUser)
SyncTrigger.MasterBerubah("tbl_merk",      TxtKode.Text, aksi, ModuleVariabel.NamaUser)
SyncTrigger.MasterBerubah("tbl_armada",    TxtKode.Text, aksi, ModuleVariabel.NamaUser)

' FormCabang — trigger internal, tidak perlu panggil manual
' (SyncTriggerCabang() dipanggil otomatis di BtnSimpanManual_Click)
```

---

## 6. Alur Sinkronisasi

### Upload (MySQL → Supabase)

```
[Data berubah di form]
        ↓
[SyncTrigger → is_dirty=1, version+1, masuk sync_queue]
        ↓
[Klik ↑ UPLOAD atau SYNC SEMUA]
        ↓
[ValidasiKodeCabang() — cek kode cabang ini tidak konflik]
        ↓
[ProcessQueue()]
  ├─ tbl_barang  → ProsesQueueBarang()
  └─ tbl_cabang  → ProsesQueueCabang() → DoInsertCabang() / DoUpdateCabang()
       ├─ Kode bebas di cloud → POST + simpan id_cloud + set device_id (claim)
       ├─ Kode milik device ini → PATCH (update)
       └─ Kode diklaim device lain → auto-rename ke urutan berikutnya → POST
        ↓
[UploadSemuaSnapshot()]
  → stok_per_toko, hutang_supliyer_snapshot
  → piutang_pelanggan_snapshot, karyawan_snapshot
  → gaji_ringkasan_snapshot, coa_snapshot
```

### Download (Supabase → MySQL)

```
[Klik ↓ DOWNLOAD atau SYNC SEMUA]
        ↓
[GET dari Supabase: updated_at > last_sync]
        ↓
  Per baris:
  ├─ Tidak ada lokal → INSERT
  └─ Ada lokal, version cloud > lokal → UPDATE
        ↓
[Update last_sync di sync_config]
```

### Conflict Handling

```
version lokal < version cloud
        ↓
Jangan overwrite → simpan ke sync_conflict_log
        ↓
Queue ditandai 'failed' dengan pesan CONFLICT
        ↓
Lihat di FormSync → Lihat Log
```

---

## 7. Tombol di FormSync

| Tombol | Warna | Fungsi |
|---|---|---|
| `↑ UPLOAD` | Oranye | Kirim perubahan lokal ke cloud + upload semua snapshot |
| `↓ DOWNLOAD` | Hijau | Ambil data terbaru dari cloud ke lokal |
| `SYNC SEMUA` | Biru | Upload + Download sekaligus |
| `Cek Koneksi` | Abu | Cek status online/offline |
| `Refresh` | — | Refresh jumlah queue pending |
| `Lihat Log` | — | Tampilkan log aktivitas sync |

Tombol UPLOAD hanya aktif jika ada data pending (`is_dirty > 0`).

---

## 8. Transfer Barang Antar Toko

```vb
' Toko A kirim transfer
Dim ok As Boolean = SyncManager.KirimTransfer(
    kodeBarang:="BRG-001", namaBarang:="Nama Barang",
    keToko:="CB-A3F2-0001", qty:=10, satuan:="PCS",
    isiSatuan:=1, keterangan:="Transfer stok",
    idUser:=ModuleVariabel.NamaUser)

' Toko B terima transfer (setelah download)
SyncManager.TerimaTransfer(idCloud, kodeBarang, qtySatuan, ModuleVariabel.NamaUser)
' → stok lokal bertambah + status di Supabase = 'diterima'
```

---

## 9. Strategi Hemat Quota Supabase Free

- Tidak ada timer polling — request hanya saat user klik
- Upload hanya jika `is_dirty > 0` — tidak kirim data yang tidak berubah
- Snapshot laporan dikirim sekaligus saat upload, bukan per perubahan
- UPSERT (`merge-duplicates`) — 1 request per baris, tidak delete+insert
- Toko bisa full offline — semua perubahan antri di `sync_queue` lokal

---

## 10. Logging

| Jenis | Keterangan |
|---|---|
| `UPLOAD` | Data berhasil dikirim ke Supabase |
| `DOWNLOAD` | Data berhasil diterima dari Supabase |
| `CONFLICT` | Konflik versi — tidak dioverwrite |
| `ERROR` | Gagal koneksi atau error lainnya |

Log auto-rename cabang tercatat sebagai `UPLOAD` dengan pesan:
`Auto-rename konflik: CB-A3F2-0001 → CB-A3F2-0003`

---

## 11. Struktur File

```
9Sync/
├── SupabaseHelper.vb     — REST client GET/POST/PATCH/UPSERT/DELETE
├── SyncQueue.vb          — Antrian upload + retry maks 5x
├── SyncLog.vb            — Pencatatan aktivitas
├── SyncConfig.vb         — last_sync per tabel, kode_cabang, device_id
├── SyncManager.vb        — Upload, download, snapshot, transfer, conflict, cabang
├── SyncTrigger.vb        — Dipanggil setelah save di form master
└── FormSync.vb/.Designer — UI 3 tombol sync + log viewer

Database/
├── 01_migrasi_kolom.sql              — Tambah kolom ke tabel yang sudah ada
├── 02_migrasi_sync_setup_supabase.sql — Tabel queue/log/config + kolom sync MySQL
├── 03_migrasi_index.sql              — Index performa
├── supabase_setup.sql                — Tabel master + sync + cabang_master di Supabase
└── supabase_laporan.sql              — Tabel snapshot + 12 view laporan
```

---

## 12. Dependency

- `Newtonsoft.Json` — install via NuGet
- `MySql.Data` — sudah ada di proyek
- .NET Framework 4.7+

---

## 13. Kode Cabang — Otomatis & Conflict-Safe

### Format Kode

```
CB-[4 char DeviceId]-[urutan 4 digit]
Contoh: CB-A3F2-0001
        CB-A3F2-0002
```

- `CB` = prefix tetap
- `A3F2` = 4 karakter pertama dari `DeviceId` (UUID unik per instalasi, disimpan di `sync_config`)
- `0001` = urutan berdasarkan jumlah cabang lokal yang sudah ada

### Alur Pembuatan Kode (Offline)

```
[User buka FormCabang → Tambah Baru]
        ↓
[GenerateKodeCabang() — baca DeviceId dari sync_config]
        ↓
[Cari urutan tertinggi di tbl_cabang dengan prefix CB-XXXX-]
        ↓
[Tampilkan kode otomatis di TxtKodeCabang (readonly)]
        ↓
[User isi nama, alamat, dll → Simpan]
        ↓
[Simpan lokal: is_dirty=1, version=1]
```

### Alur Saat Online (Upload / Sync)

```
[ProcessQueue → ProsesQueueCabang]
        ↓
[Cek kode di cabang_master Supabase]
        ├─ Belum ada → INSERT + set device_id (claim kode ini)
        ├─ Ada, device_id = device ini → UPDATE (kode milik kita)
        └─ Ada, device_id = device lain → KONFLIK
                        ↓
              [CariKodeBerikutnyaCloud()]
              Cari urutan bebas di cloud + lokal
                        ↓
              [RenameKodeCabangLokalStatic()]
              Update PK di tbl_cabang + sync_queue
                        ↓
              [INSERT dengan kode baru + device_id]
              Log: "Auto-rename: CB-A3F2-0001 → CB-A3F2-0003"
```

### Alur Saat Klik "Sinkron Dari Cloud" di FormCabang

```
[AutoRenameKonflikCabang()]
  → Cek semua cabang is_dirty=1 yang belum punya id_cloud
  → Jika kode diklaim device lain → rename otomatis
        ↓
[Download cabang_master dari Supabase (incremental: updated_at > last_sync)]
  → Cabang baru dari cloud → INSERT lokal
  → Version cloud > lokal → UPDATE lokal
        ↓
[Update last_sync_cabang]
```

### Kolom Tambahan di cabang_master (Supabase)

| Kolom | Keterangan |
|---|---|
| `device_id` | UUID instalasi yang mengklaim kode ini |
| `claimed_at` | Waktu klaim pertama kali |
| `version` | Nomor versi untuk conflict handling |
| `updated_by` | User yang terakhir ubah |

### Catatan Penting

- `TxtKodeCabang` di FormCabang selalu **readonly** — user tidak bisa ketik kode manual
- Kode yang sudah di-claim di cloud tidak bisa dipakai toko lain
- Jika terjadi rename, semua referensi di `sync_queue` ikut diupdate otomatis
- `DeviceId` dibuat sekali saat pertama install, disimpan di `sync_config`, tidak berubah
