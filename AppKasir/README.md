# 🚀 AppKasir 2026 - Enterprise POS & Accounting System

**Versi:** 2.0.0 (Stabilized)  
**Bahasa:** Visual Basic .NET (VB.NET)  
**Database:** MySQL 5.7 / 8.0  
**Arsitektur:** Centralized Transaction Logic with Atomic Data Integrity

---

## 📌 Ringkasan Aplikasi

**AppKasir** adalah sistem **Point-of-Sale (POS)** dan **Akuntansi** terpadu yang dirancang untuk skala ritel menengah hingga besar. Berbeda dengan POS standar, AppKasir menggunakan pendekatan **Double-Entry Bookkeeping** dan **Atomic Stock Counters** untuk menjamin akurasi data 100% antara stok fisik, laporan keuangan, dan histori transaksi.

---

## 🏗️ Arsitektur & Teknologi Inti

### 1. Centralized Deletion Logic (`ModuleHapusTransaksi`)
Sistem menggunakan pola **"Delete-then-Reinsert"** untuk proses Edit. Semua logika pembatalan transaksi (reversal) dipusatkan di satu modul untuk menjamin:
- **Pembalikan Piutang/Hutang**: Saldo tagihan pelanggan/supplier kembali ke titik nol sebelum transaksi dihapus.
- **Pembalikan Jurnal**: Jurnal umum dihapus dan saldo akun di `tbl_datareferensi` dihitung ulang secara otomatis.
- **Pembalikan Stok**: Counter barang dikembalikan secara presisi sebelum record transaksi dihapus.

### 2. Counter-Based Inventory System
Aplikasi tidak melakukan update langsung pada kolom stok akhir, melainkan menggunakan sistem counter di `tbl_barang`:
- **Counters**: `PEMBELIAN_TOKO/GUDANG`, `PENJUALAN_TOKO/GUDANG`, `RETUR_BELI_x`, `RETUR_JUAL_x`.
- **Sync Engine**: Menggunakan fungsi `HitungStokPerubahan` yang mengkalkulasi ulang `STOK_TOKO = (AWAL + MASUK + RETUR_JUAL) - (KELUAR + RETUR_BELI)`.
- **Audit Trail**: Setiap perubahan stok dicatat di `HistoryBarang` dan diverifikasi melalui `AuditStokTransaksi`.

### 3. Rapid Entry & Hybrid UX
- **Asterisk Entry (`*`)**: Fitur entri cepat (Rapid Entry) untuk menambahkan item dalam jumlah besar secara instan di Grid.
- **Background Loading**: Proses loading detail transaksi di Dashboard (`FormUtama`) berjalan secara Asynchronous (Non-blocking UI).

---

## 📂 Struktur Proyek

```text
AppKasir/
├── 0Form/                  # Form Utama & Logika Pusat
│   ├── FormUtama.vb        # Dashboard & Management Center
│   └── ModuleHapusTransaksi.vb # Jantung Reversal Transaksi (Beli, Jual, Retur)
├── 1Master/                # Manajemen Data Master
│   ├── TambahBarang.vb     # Master Produk (Multi-Satuan)
│   └── TambahPelanggan.vb  # Master Pelanggan & Supplier
├── 2Trans/                 # Modul Transaksi Aktif
│   ├── FormJual.vb         # Modul Penjualan Retail & Partai
│   ├── FormPembelian.vb    # Modul Pembelian (Restock)
│   └── FormReturBeli.vb    # Reversal Pembelian
├── 3Jurnal/                # Akuntansi & Buku Besar
├── 5Lap/                   # Reporting & Audit Trail
├── Modules/                # Library & Variabel Global
│   ├── ModuleVariabel.vb   # Konstanta & Parameter Sistem
│   └── ModuleAuditTrail.vb # Engine Pencatat Jejak User
└── MySQL/                  # Driver & Konektor Database
```

---

## 🗄️ Skema Database Unggulan

### Tabel: `tbl_barang` (Master Inventori)
| Kolom | Deskripsi |
|-------|-----------|
| `ID_BARANG` | Primary Key (Barcode/Internal ID) |
| `HARGA_BELI` | HPP Terkini (Moving Average) |
| `STOK_AWAL_TOKO` | Stok awal saat pertama kali input |
| `PENJUALAN_TOKO` | Counter total barang keluar |
| `PEMBELIAN_TOKO` | Counter total barang masuk |
| `STOK_TOKO` | Saldo stok fisik real-time (Computed) |

### Tabel: `penjualan` & `pembelian`
- Mendukung status **Lunas** dan **Bel Lunas (Piutang/Hutang)**.
- Integrasi kolom `NILAI_RETUR` dan `TGL_RETUR` untuk pelacakan sejarah faktur.

---

## 🛠️ Panduan Developer

### Prinsip Edit Transaksi
Jangan pernah mengubah nilai stok secara manual di dalam form. Selalu gunakan alur berikut:
1. Panggil `ModuleHapusTransaksi.HapusXxxx()` untuk membatalkan transaksi lama (Reversal).
2. Biarkan user mengubah data di UI.
3. Simpan sebagai transaksi baru menggunakan `SimpanXxxx()`.

### Rekalkulasi Saldo Akun
Setiap kali melakukan modifikasi pada `JurnalUmum`, pastikan memanggil `UpdateSaldoAkun(kode_akun)` agar saldo di Dashboard tetap akurat tanpa perlu restart aplikasi.

---

## 📜 Lisensi & Kontribusi
Aplikasi ini dikembangkan secara internal untuk **App Kasir_2026**.  
**Copyright © 2026 adysuryadi64**. Seluruh hak cipta dilindungi undang-undang.
