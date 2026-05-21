# Akses Database — AppKasir

## Koneksi

| Parameter | Nilai |
|---|---|
| Executable | `MySQL\mysql.exe` (relatif dari root proyek) |
| Password | `12345678` |
| DB Development | `db_moroseneng` |
| DB Produksi | `db_rejeki` |

---

## Cara Query via PowerShell

```powershell
# Lihat struktur tabel
.\MySQL\mysql.exe -u root -p12345678 db_moroseneng -e "DESCRIBE nama_tabel"

# Lihat semua tabel
.\MySQL\mysql.exe -u root -p12345678 db_moroseneng -e "SHOW TABLES"

# Query data
.\MySQL\mysql.exe -u root -p12345678 db_moroseneng -e "SELECT * FROM nama_tabel"

# Bandingkan data produksi
.\MySQL\mysql.exe -u root -p12345678 db_rejeki -e "SELECT * FROM nama_tabel"
```

---

## Kapan Wajib Query Database

Sebelum menulis kode yang menyentuh tabel/kolom, **wajib verifikasi** ke database terlebih dahulu:

- Nama tabel yang dipakai
- Nama kolom yang tepat (case-sensitive di beberapa konteks)
- Tipe data kolom (Decimal, Int, VarChar, dll)
- Nilai enum yang valid (misal: STATUS = 'Aktif' bukan 'aktif')
- Relasi antar tabel (foreign key)

**Jangan menebak nama kolom** — selalu cek dengan `DESCRIBE` atau `SHOW COLUMNS`.

---

## Aturan WAJIB — Jangan Ubah Nama Kolom Tanpa Persetujuan

> Mengubah nama kolom di database berdampak ke seluruh aplikasi (VB.NET, PHP, stored procedure).
> Satu kolom yang salah nama bisa menyebabkan crash di banyak tempat sekaligus.

- **DILARANG** mengubah nama kolom tanpa persetujuan user
- **DILARANG** mengubah tipe data kolom tanpa persetujuan user
- **DILARANG** menghapus kolom tanpa persetujuan user
- Jika menemukan inkonsistensi nama kolom → **laporkan ke user**, jangan langsung ubah
- Jika perlu kolom baru → **usulkan ke user** dulu, tunggu persetujuan

---

## Kapan Pakai `db_moroseneng` vs `db_rejeki`

| Database | Kapan Dipakai |
|---|---|
| `db_moroseneng` | Development — untuk cek struktur tabel, test query, verifikasi nama kolom |
| `db_rejeki` | Produksi — untuk verifikasi data nyata, nilai enum, format data aktual |

Jika ada perbedaan antara keduanya → **laporkan ke user**.

---

## Contoh Penggunaan

```powershell
# Cek struktur tabel barang
.\MySQL\mysql.exe -u root -p12345678 db_moroseneng -e "DESCRIBE tbl_barang"

# Cek nilai enum STATUS yang valid
.\MySQL\mysql.exe -u root -p12345678 db_moroseneng -e "SELECT DISTINCT STATUS FROM tbl_barang LIMIT 10"

# Cek nama kolom tabel penjualan
.\MySQL\mysql.exe -u root -p12345678 db_moroseneng -e "SHOW COLUMNS FROM penjualan"

# Bandingkan data produksi untuk verifikasi
.\MySQL\mysql.exe -u root -p12345678 db_rejeki -e "SELECT ID_PENJUALAN, TOTAL FROM penjualan ORDER BY ID_PENJUALAN DESC LIMIT 5"
```
