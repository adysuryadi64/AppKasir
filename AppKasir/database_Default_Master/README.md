# Data Default Master

Folder ini berisi file SQL untuk data default master aplikasi Kasir.

## Struktur File

| No | File | Keterangan |
|----|------|------------|
| 01 | `01_kategori_default.sql` | 80+ kategori barang umum di Indonesia |
| 02 | `02_satuan_default.sql` | 40+ satuan/kemasan barang |
| 03 | `03_merk_default.sql` | 150+ merk/brand populer di Indonesia |

## Format Kode Kategori

Kode kategori menggunakan **singkatan 2-3 karakter** yang digenerate otomatis dari nama:

| Rule | Contoh Nama | Kode | Penjelasan |
|------|-------------|------|------------|
| 1 kata, 3 huruf pertama | Minuman | **MIN** | Min-uman |
| 2 kata, 1+2 huruf | Minyak Goreng | **MGO** | M-inyak + Go-reng |
| 3+ kata, 1+1+1 huruf | Makanan Ringan | **MRI** | M-akanan + R-ingan + I (kata ke-3) |
| Duplikat | Jika AMN sudah ada | **AM1** | Alternatif otomatis |

## Cara Penggunaan

### 1. Jalankan Satu Per Satu
```powershell
# Kategori
Get-Content Database/Defaults/01_kategori_default.sql | .\MySQL\mysql.exe -u root -p12345678 db_kasirlancar

# Satuan
Get-Content Database/Defaults/02_satuan_default.sql | .\MySQL\mysql.exe -u root -p12345678 db_kasirlancar

# Merk
Get-Content Database/Defaults/03_merk_default.sql | .\MySQL\mysql.exe -u root -p12345678 db_kasirlancar
```

### 2. Jalankan Semua Sekaligus
```powershell
cat Database/Defaults/01_kategori_default.sql, Database/Defaults/02_satuan_default.sql, Database/Defaults/03_merk_default.sql | .\MySQL\mysql.exe -u root -p12345678 db_kasirlancar
```

## Kategori yang Tersedia

### Minuman & Bahan Pokok
- MIN (Minuman), AMN (Air Minum), MGO (Minyak Goreng)
- GUL (Gula), BER (Beras), TEP (Tepung)
- SUS (Susu), KOP (Kopi), TEH (Teh)

### Makanan
- MRI (Makanan Ringan), MIE (Mie Instan)
- BIS (Biskuit), KUE (Kue), ROT (Roti)
- SNS (Snack), COK (Coklat), ESK (Es Krim)

### Sembako
- SMB (Sembako), MSG (MSG/Vetsin)
- BUM (Bumbu Instan)

### Kebersihan
- KEB (Kebersihan), DET (Deterjen), SAB (Sabun)
- SHP (Shampoo), PEW (Pewangi), TIS (Tisu)

### Perawatan Tubuh
- PRW (Perawatan Tubuh), PAS (Pasta Gigi)
- POP (Popok), VIT (Vitamin), OBT (Obat)

### Rokok
- ROK (Rokok), KRE (Kretek), FIL (Filter)

### Elektronik & Rumah Tangga
- ELK (Elektronik), BAT (Baterai)
- PRT (Perlengkapan Rumah), GAS (Gas LPG)

### Lainnya
- ATS (Alat Tulis), SAY (Sayur), BUH (Buah)

## Daftar Merk Populer

### Minuman
Aqua, Le Minerale, Nestle, Vit, Ades, Pocari Sweat, Milo, NutriBoost

### Makanan
Indomie, Mie Sedaap, Chitato, Taro, Beng-beng, Oreo, KitKat

### Sembako
Bimoli, Fortune, Sania, Tropical, Gulaku, Segitiga Biru

### Kebersihan
Rinso, Daia, Attack, Lifebuoy, Lux, Dettol, Paseo, Tempo

### Perawatan
Colgate, Pepsodent, Mamy Poko, Zwitsal, Cussons Baby

### Rokok
Gudang Garam, Sampoerna, Djarum, Marlboro, Surya

## Catatan Penting

1. **Idempotent**: Semua query menggunakan `ON DUPLICATE KEY UPDATE` sehingga aman dijalankan berulang kali
2. **Foreign Key**: Foreign key check dimatikan saat insert untuk menghindari error
3. **Database Target**: Default ke `db_kasirlancar` (development)
4. **Production**: Untuk production gunakan `db_moroseneng`

## Troubleshooting

### Error: Duplicate Entry
Jika ada error duplikat, artinya data sudah ada. Query akan otomatis update data existing.

### Error: Foreign Key
Pastikan tabel `tbl_kategori`, `tbl_satuan`, `tbl_merk` sudah ada di database.

## Update Data

Untuk menambah data baru:
1. Edit file SQL yang sesuai
2. Ikuti format insert yang sudah ada
3. Jalankan ulang file tersebut
