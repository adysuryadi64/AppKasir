# Design — Kasir Lancar Mobile: UI + Sinkronisasi Backend (v2)

## Arsitektur Perubahan

```
Flutter (UI)                API Service              PHP Backend              MySQL
─────────────               ───────────              ───────────              ─────
login_screen            →   getUsers             →   get_users.php        →   tbl_user
                        →   getHakAkses          →   get_hak_akses.php    →   tbl_hakakses
                        →   getDataPerusahaan    →   get_perusahaan.php   →   tbl_perusahaan

dashboard_screen        →   getDashboardSummary  →   get_dashboard_summary.php → penjualan
  + Drawer sidebar      →   getAIAnalytics       →   get_ai_analytics.php  → penjualan_detail
                                                                             → tbl_barang
                                                                             → tbl_pelanggan

penjualan_screen        →   getAkunCOA           →   get_akun_coa.php     →   tbl_datareferensi
  (multi-screen flow)   →   getKaryawan          →   get_karyawan.php     →   tbl_karyawan
                        →   syncPenjualan        →   sync_penjualan.php   →   penjualan

nota_preview_screen     →   (data dari state)    →   (tidak perlu API)
printer_settings_screen →   (SharedPreferences)

stok_opname_screen      →   syncStokOpname       →   sync_stokopname.php  →   Stok_Opname
opname_list_screen      →   getOpnameList        →   get_opname_list.php  →   Stok_Opname

laporan_stok_screen     →   getLaporanStok       →   get_laporan_stok.php →   tbl_barang
```

---

## Struktur File Baru

```
lib/
├── screens/
│   ├── login_screen.dart              ← redesign layout
│   ├── dashboard_screen.dart          ← sidebar + data real + AI Analytics cards
│   ├── ai_analytics/
│   │   ├── ai_produk_terlaris_modal.dart
│   │   ├── ai_barang_lambat_modal.dart
│   │   ├── ai_reorder_alert_modal.dart
│   │   ├── ai_jam_puncak_modal.dart
│   │   ├── ai_margin_profit_modal.dart
│   │   └── ai_pelanggan_aktif_modal.dart
│   ├── penjualan/
│   │   ├── penjualan_flow.dart        ← PageView controller (state holder)
│   │   ├── step1_header_screen.dart   ← tanggal, pelanggan, sales
│   │   ├── step2_items_screen.dart    ← tambah item, list item
│   │   ├── step3_payment_screen.dart  ← pembayaran, diskon, pajak, akun
│   │   ├── step4_preview_screen.dart  ← preview nota
│   │   └── printer_settings_screen.dart
│   ├── stok_opname_screen.dart        ← redesign
│   ├── opname_list_screen.dart        ← redesign card
│   ├── laporan_stok_screen.dart       ← baru
│   └── server_config_screen.dart
├── providers/
│   ├── auth_provider.dart             ← tambah hak akses + data perusahaan
│   └── penjualan_provider.dart        ← state management alur penjualan
├── services/
│   ├── api_service.dart               ← tambah endpoint baru
│   ├── storage_service.dart
│   └── thermal_print_service.dart     ← baru: cetak thermal Bluetooth
├── widgets/
│   ├── app_drawer.dart                ← sidebar navigasi
│   ├── nota_widget.dart               ← render nota sebagai widget
│   └── product_search_sheet.dart
└── models/
    ├── penjualan_model.dart
    ├── hak_akses_model.dart           ← baru
    └── perusahaan_model.dart          ← baru
```

---

## P0 — AI Analytics Design

### Prinsip
Semua kalkulasi dilakukan di PHP via SQL query — tidak ada model ML eksternal.
Flutter hanya menampilkan data yang sudah diolah server. Pendekatan ini:
- Tidak butuh library AI/ML tambahan
- Akurat karena langsung dari data transaksi nyata
- Bisa berjalan di server lokal (AppServ/XAMPP)

### Endpoint: `get_ai_analytics.php?type=xxx&lokasi=xxx`

**type=produk_terlaris**
```sql
SELECT pd.ID_BARANG, pd.NAMA_BARANG,
       SUM(pd.QTY_SATUAN)   AS total_qty,
       SUM(pd.TOTAL_HARGA)  AS total_omzet,
       -- Trend: bandingkan 7 hari ini vs 7 hari sebelumnya
       SUM(CASE WHEN p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
                THEN pd.QTY_SATUAN ELSE 0 END) AS qty_7hari,
       SUM(CASE WHEN p.TGL_TRANSAKSI BETWEEN DATE_SUB(CURDATE(), INTERVAL 14 DAY)
                                         AND DATE_SUB(CURDATE(), INTERVAL 7 DAY)
                THEN pd.QTY_SATUAN ELSE 0 END) AS qty_7hari_lalu
FROM penjualan_detail pd
JOIN penjualan p ON p.ID_PENJUALAN = pd.FAKTUR_JUAL
WHERE p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
  AND p.LOKASIBARANG = :lokasi
GROUP BY pd.ID_BARANG, pd.NAMA_BARANG
ORDER BY total_qty DESC
LIMIT 10
```
Response kartu: `{ top_item: "Nama Barang", qty: 150, omzet: 2500000, trend: "+12%" }`

**type=barang_lambat**
```sql
SELECT b.ID_BARANG, b.NAMA_BARANG,
       COALESCE(b.STOK_TOKO, 0) AS stok,
       b.HARGA_BELI,
       COALESCE(b.STOK_TOKO, 0) * b.HARGA_BELI AS nilai_tertahan,
       MAX(p.TGL_TRANSAKSI) AS terakhir_terjual,
       DATEDIFF(CURDATE(), MAX(p.TGL_TRANSAKSI)) AS hari_tidak_terjual
FROM tbl_barang b
LEFT JOIN penjualan_detail pd ON pd.ID_BARANG = b.ID_BARANG
LEFT JOIN penjualan p ON p.ID_PENJUALAN = pd.FAKTUR_JUAL
  AND p.LOKASIBARANG = :lokasi
WHERE COALESCE(b.STOK_TOKO, 0) > 0
GROUP BY b.ID_BARANG
HAVING hari_tidak_terjual > 30 OR terakhir_terjual IS NULL
ORDER BY nilai_tertahan DESC
LIMIT 20
```
Response kartu: `{ jumlah_item: 12, nilai_tertahan: 8500000, terparah: "Nama Barang (45 hari)" }`

**type=reorder_alert**
```sql
SELECT b.ID_BARANG, b.NAMA_BARANG,
       COALESCE(b.STOK_TOKO, 0) AS stok_saat_ini,
       -- Rata-rata penjualan per hari (7 hari terakhir)
       COALESCE(SUM(pd.QTY_SATUAN) / 7, 0) AS rata_per_hari,
       -- Estimasi hari habis
       CASE WHEN COALESCE(SUM(pd.QTY_SATUAN) / 7, 0) > 0
            THEN FLOOR(COALESCE(b.STOK_TOKO, 0) / (SUM(pd.QTY_SATUAN) / 7))
            ELSE 999 END AS estimasi_hari_habis
FROM tbl_barang b
LEFT JOIN penjualan_detail pd ON pd.ID_BARANG = b.ID_BARANG
LEFT JOIN penjualan p ON p.ID_PENJUALAN = pd.FAKTUR_JUAL
  AND p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
  AND p.LOKASIBARANG = :lokasi
WHERE COALESCE(b.STOK_TOKO, 0) >= 0
GROUP BY b.ID_BARANG
HAVING estimasi_hari_habis <= 7 AND rata_per_hari > 0
ORDER BY estimasi_hari_habis ASC
LIMIT 20
```
Response kartu: `{ jumlah_item: 5, paling_kritis: "Nama Barang (habis 2 hari lagi)" }`

**type=jam_puncak**
```sql
SELECT HOUR(TGL_TRANSAKSI) AS jam,
       COUNT(*) AS jumlah_transaksi,
       SUM(GRAND_TOTAL_STL_PAJAK) AS total_omzet
FROM penjualan
WHERE TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
  AND LOKASIBARANG = :lokasi
GROUP BY jam
ORDER BY jam ASC
```
Response kartu: `{ jam_puncak: "10:00-11:00", transaksi: 23, rekomendasi: "Siapkan kasir tambahan jam 10-12" }`

**type=margin_profit**
```sql
SELECT pd.ID_BARANG, pd.NAMA_BARANG,
       AVG(pd.HARGA_JUAL) AS avg_harga_jual,
       AVG(pd.HARGA_BELI)  AS avg_harga_beli,
       AVG((pd.HARGA_JUAL - pd.HARGA_BELI) / NULLIF(pd.HARGA_JUAL, 0) * 100) AS margin_persen,
       SUM(pd.TOTAL_HARGA) AS total_omzet
FROM penjualan_detail pd
JOIN penjualan p ON p.ID_PENJUALAN = pd.FAKTUR_JUAL
WHERE p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 30 DAY)
  AND p.LOKASIBARANG = :lokasi
  AND pd.HARGA_JUAL > 0
GROUP BY pd.ID_BARANG
HAVING total_omzet > 0
ORDER BY margin_persen DESC
LIMIT 10  -- top 5 + bottom 5 diambil di PHP
```
Response kartu: `{ avg_margin: "23.5%", top_item: "Nama Barang (45%)", bottom_item: "Nama Barang (2%)" }`

**type=pelanggan_aktif**
```sql
-- RFM sederhana: Recency (hari terakhir beli), Frequency (jumlah transaksi), Monetary (total belanja)
SELECT p.ID_PELANGGAN, p.NAMA_PELANGGAN,
       COUNT(*)                          AS frekuensi,
       SUM(p.GRAND_TOTAL_STL_PAJAK)     AS total_belanja,
       MAX(p.TGL_TRANSAKSI)             AS terakhir_beli,
       DATEDIFF(CURDATE(), MAX(p.TGL_TRANSAKSI)) AS hari_sejak_beli
FROM penjualan p
WHERE p.TGL_TRANSAKSI >= DATE_SUB(CURDATE(), INTERVAL 90 DAY)
  AND p.LOKASIBARANG = :lokasi
  AND p.ID_PELANGGAN != ''
GROUP BY p.ID_PELANGGAN, p.NAMA_PELANGGAN
ORDER BY total_belanja DESC
LIMIT 10
```
Response kartu: `{ total_pelanggan_aktif: 34, top_pelanggan: "Nama (Rp 5.2jt)", pelanggan_baru_bulan_ini: 8 }`

---

### Flutter: `AIAnalyticsCard` Widget

```dart
// Widget kartu ringkasan — dipakai 6x di dashboard
class AIAnalyticsCard extends StatelessWidget {
  final String title;
  final IconData icon;
  final Color color;
  final String? keyMetric;      // angka utama, misal "12 item"
  final String? insight;        // 1 kalimat, misal "Nilai tertahan Rp 8,5jt"
  final bool isLoading;
  final VoidCallback onTap;     // buka modal detail
}
```

### Layout di Dashboard

```
┌─────────────────────────────────────────┐
│  AI Analytics                    [↻]    │
│  ─────────────────────────────────────  │
│  ┌──────────────┐  ┌──────────────┐    │
│  │ 🔥 Terlaris  │  │ 🐌 Lambat    │    │
│  │ Mie Goreng   │  │ 12 item      │    │
│  │ 150 qty/7hr  │  │ Rp 8,5jt    │    │
│  └──────────────┘  └──────────────┘    │
│  ┌──────────────┐  ┌──────────────┐    │
│  │ ⚠️ Reorder   │  │ ⏰ Jam Puncak │    │
│  │ 5 item kritis│  │ 10:00-11:00  │    │
│  │ habis 2 hari │  │ 23 transaksi │    │
│  └──────────────┘  └──────────────┘    │
│  ┌──────────────┐  ┌──────────────┐    │
│  │ 💰 Margin    │  │ 👥 Pelanggan │    │
│  │ Avg 23.5%    │  │ 34 aktif     │    │
│  │ Top: 45%     │  │ 8 baru bulan │    │
│  └──────────────┘  └──────────────┘    │
└─────────────────────────────────────────┘
```

### Modal Detail — Contoh Reorder Alert

```
┌─────────────────────────────────────────┐
│  ⚠️ Reorder Alert              [✕]      │
│  5 barang perlu segera dipesan          │
│  ─────────────────────────────────────  │
│  Nama Barang A    Stok: 3   Habis: 2hr  │
│  Nama Barang B    Stok: 5   Habis: 4hr  │
│  Nama Barang C    Stok: 8   Habis: 6hr  │
│  ...                                    │
│  ─────────────────────────────────────  │
│  💡 Saran: Order minimal 7 hari supply  │
└─────────────────────────────────────────┘
```

---

## P1 — PHP Endpoints Lain

### `get_perusahaan.php`
```php
// Ambil data perusahaan untuk header nota dan default akun COA
SELECT KODE, NAMA, ALAMAT, KOTA, HP, PEMILIK,
       FOOTER1, FOOTER2, FOOTER3,
       Kode_rek_Jual_Toko, nama_rek_Jual_Toko,
       Kode_rek_Jual_Gudang, nama_rek_Jual_Gudang,
       Kode_rek_Transfer_Jual, nama_rek_Transfer_Jual,
       KODE_REK_PIUTANG_JUAL, NAMA_REK_PIUTANG_JUAL,
       KODE_REK_BARANG, NAMA_REK_BARANG
FROM tbl_perusahaan LIMIT 1
```
Response: `{ status, data: { nama, alamat, kota, hp, footer1..3, akun_kas_toko, akun_kas_gudang, akun_transfer, ... } }`

### `get_hak_akses.php`
```php
// Ambil setting hak akses untuk user yang login
// Sama dengan ModulHakAkses.BacaSettingDariCache di VB
SELECT nama_setting, nilai
FROM tbl_hakakses
WHERE id_user = :id_user OR id_user = 'ALL'
```
Response: `{ status, data: { izinkan_jual_stok_minus, izinkan_satuan_berbeda, izinkan_jual_rugi, izinkan_ubah_harga, tampil_info_stok, langsung_isi_nominal, izinkan_nominal_nol, izinkan_tanggal_lampau } }`

### `get_dashboard_summary.php`
```php
SELECT SUM(GRAND_TOTAL_STL_PAJAK) AS total_penjualan,
       COUNT(*) AS jumlah_transaksi
FROM penjualan
WHERE DATE(TGL_TRANSAKSI) = CURDATE() AND LOKASIBARANG = :lokasi

SELECT COUNT(*) AS jumlah_opname
FROM Stok_Opname
WHERE DATE(TANGGAL) = CURDATE() AND LOKASI = :lokasi
```

### `get_karyawan.php`
```php
SELECT Kode, Nama FROM tbl_karyawan WHERE Status = 'Aktif' ORDER BY Nama ASC
```

### `get_akun_coa.php`
```php
// Filter berdasarkan tipe: KAS, BANK, atau semua
SELECT KODE_AKUN, NAMA_AKUN, TYPE_AKUN
FROM tbl_datareferensi
WHERE (:tipe = '' OR Type_Akun LIKE :tipe) AND STATUS = 'Aktif'
ORDER BY KODE_AKUN ASC
```

### `get_laporan_stok.php`
```php
SELECT b.ID_BARANG, b.NAMA_BARANG, b.BARCODE,
       b.STOK_TOKO, b.STOK_GUDANG,
       b.NAMA_KATEGORI, b.NAMA_MERK,
       b.SATUAN_KECIL, b.SATUAN_SEDANG, b.SATUAN_BESAR
FROM tbl_barang b
WHERE (:search = '' OR b.NAMA_BARANG LIKE :search OR b.BARCODE = :search)
  AND (:kategori = '' OR b.NAMA_KATEGORI = :kategori)
ORDER BY b.NAMA_BARANG ASC
LIMIT :limit OFFSET :offset
```

---

## P1 — Login Screen Redesign

### Layout (dari atas ke bawah)
```
┌─────────────────────────────────┐
│  [Logo]                         │  ← compact, max 80px
│  Kasir Lancar                   │  ← nama app
│                                 │
│  ┌─────────────────────────┐    │
│  │ Pilih User ▼            │    │  ← dropdown
│  └─────────────────────────┘    │
│  ┌─────────────────────────┐    │
│  │ Password          👁    │    │  ← input password
│  └─────────────────────────┘    │
│  [        MASUK        ]        │  ← tombol login
│                                 │
│  ─────────────────────────────  │
│  v1.0.0 · Konfigurasi Server    │  ← info di bawah
└─────────────────────────────────┘
```

---

## P2 — Dashboard dengan Sidebar

### Drawer (Sidebar)
```
┌──────────────────┐
│ [Avatar] Nama    │
│ TOKO             │
├──────────────────┤
│ 🏠 Dashboard     │
│ 🛒 Penjualan     │
│ 📦 Stok Opname   │
│ 📊 Laporan Stok  │
│ ⚙️  Pengaturan   │
├──────────────────┤
│ 🚪 Logout        │
└──────────────────┘
```

### Header Dashboard
- Nama perusahaan (dari tbl_perusahaan)
- Badge lokasi aktif (TOKO/GUDANG)
- Nama user + tombol menu

### Summary Cards (data real)
- Total penjualan hari ini (Rp)
- Jumlah transaksi hari ini
- Jumlah item opname hari ini

---

## P3 — Penjualan Multi-Screen Flow

### State Management: `PenjualanProvider`
```dart
class PenjualanProvider extends ChangeNotifier {
  // Step 1
  DateTime tanggal = DateTime.now();
  Map<String, dynamic>? selectedPelanggan;
  Map<String, dynamic>? selectedSales;

  // Step 2 & 3
  List<Map<String, dynamic>> cartItems = [];
  double diskonPersen = 0, diskonRp = 0;
  double pajakPersen = 0, pajakRp = 0;
  double biayaKirim = 0;

  // Step 4
  double nominalTunai = 0, nominalTransfer = 0;
  Map<String, dynamic>? akunKas;    // default dari tbl_perusahaan
  Map<String, dynamic>? akunTransfer; // default dari tbl_perusahaan
  String bank = '', noRek = '', namaRek = '', noRef = '';

  // Computed
  double get subtotal => cartItems.fold(0, (s, i) => s + i['total_harga']);
  double get grandTotal => subtotal - diskonRp + pajakRp + biayaKirim;
  double get sisaTagihan => max(0, grandTotal - nominalTunai - nominalTransfer);
  double get kembali => max(0, nominalTunai + nominalTransfer - grandTotal);
}
```

### Screen 1 — Header Transaksi
- DatePicker tanggal
- Dropdown pelanggan (search + pilih)
- Dropdown sales (opsional)
- Tombol "Lanjut →"

### Screen 2 — Item
- Search barang (inline autocomplete)
- List item dengan:
  - Nama barang, satuan, qty (editable)
  - Harga jual (editable jika `izinkan_ubah_harga = Iya`)
  - Diskon per item (persen atau Rp)
  - Total per baris
  - Stok (tampil jika `tampil_info_stok = Iya`)
- Tombol "Lanjut ke Pembayaran →"

### Screen 3 — Pembayaran
```
Subtotal          Rp xxx
Diskon [_%] [Rp_]
Pajak  [_%] [Rp_]
Biaya Kirim  [Rp_]
─────────────────────
Grand Total       Rp xxx

Tunai [Rp_______]  Akun: [dropdown KAS]
Transfer [Rp____]  Akun: [dropdown BANK]

[jika transfer > 0]
Bank     : [________]
No. Rek  : [________]
Nama Rek : [________]
No. Ref  : [________]

Kembalian / Hutang: Rp xxx
Status: LUNAS / Belum Lunas
```

### Screen 4 — Preview Nota
- Render `NotaWidget` — identik dengan cetakan thermal
- Tombol: [Cetak] [Kirim WA] [Simpan Saja]
- Semua tombol: simpan dulu → jika berhasil → aksi cetak/kirim

---

## P4 — Nota Thermal Bluetooth

### Package
```yaml
bluetooth_print: ^4.4.0      # ESC/POS via Bluetooth
esc_pos_utils: ^0.4.1        # Helper format ESC/POS
```

### Layout Kolom (persentase, sama dengan VB)
```
Thermal 80mm (48 kar):
  Dengan diskon: Nama=0, Qty=11%, Sat=15%, Harga=51%, Disc=70%, Jml=95%
  Tanpa diskon:  Nama=0, Qty=11%, Sat=15%, Harga=65%, Jml=95%

Thermal 58mm (32 kar):
  Sama persentase, lebar total 32 karakter
```

### 8 Model Nota
| Model | Header Kolom | Diskon | Sisa Hutang |
|-------|-------------|--------|-------------|
| 1     | ✅           | ✅      | ✅           |
| 2     | ✅           | ✅      | ❌           |
| 3     | ✅           | ❌      | ✅           |
| 4     | ✅           | ❌      | ❌           |
| 5     | ❌           | ✅      | ✅           |
| 6     | ❌           | ✅      | ❌           |
| 7     | ❌           | ❌      | ✅           |
| 8     | ❌           | ❌      | ❌           |

### `ThermalPrintService`
```dart
class ThermalPrintService {
  static Future<List<BluetoothDevice>> scanDevices();
  static Future<bool> printNota(Map<String, dynamic> notaData, PrinterConfig cfg);
  static String buildLine(String left, String right, int width);
  static String buildColumns(List<String> cols, List<int> widths);
}
```

### `PrinterConfig` (disimpan di SharedPreferences)
```dart
class PrinterConfig {
  String deviceAddress;   // MAC address printer
  String deviceName;
  int paperWidth;         // 58 atau 80 (mm)
  int charsPerLine;       // 32 atau 48
  int model;              // 1-8
  bool autocut;
  int copies;
}
```

---

## P5 — Stok Opname Redesign

### Layout Item Opname
```
┌─────────────────────────────────────┐
│ Nama Barang                         │
│ Kode: xxx  Kategori: xxx  Merk: xxx │
├─────────────────────────────────────┤
│  Stok Sistem    │  Stok Nyata       │
│  [  1.250  ]    │  [ 1.200  ]       │  ← font besar
│  Satuan: Pcs    │  Selisih: -50     │
├─────────────────────────────────────┤
│ Keterangan: [_____________________] │
└─────────────────────────────────────┘
```

- Tombol +/- untuk stok nyata
- TextField langsung untuk input angka
- Selisih dihitung otomatis (merah jika minus, hijau jika plus)
- Tombol Simpan di bawah layar (sticky)

---

## P6 — Laporan Stok

### `laporan_stok_screen.dart`
- AppBar dengan search dan filter kategori
- List barang: nama, kode, stok toko, stok gudang, satuan
- Pull-to-refresh
- Pagination (load more)

---

## P7 — Hak Akses User

### `HakAksesModel`
```dart
class HakAksesModel {
  final bool izinkanJualStokMinus;
  final bool izinkanSatuanBerbeda;
  final bool izinkanJualRugi;
  final bool izinkanUbahHarga;
  final bool tampilInfoStok;
  final bool langsungIsiNominal;
  final bool izinkanNominalNol;
  final bool izinkanTanggalLampau;
}
```

### Penerapan di Penjualan
- `izinkanJualStokMinus = false` → validasi sebelum simpan, blokir jika stok < qty
- `izinkanSatuanBerbeda = false` → cek duplikat item saat tambah barang
- `izinkanJualRugi = false` → validasi harga jual >= harga beli sebelum simpan
- `izinkanUbahHarga = false` → field harga di item `readOnly = true`
- `tampilInfoStok = true` → tampilkan kolom stok di list item

---

## P8 — Data Perusahaan

### `PerusahaanModel`
```dart
class PerusahaanModel {
  final String nama, alamat, kota, hp;
  final String footer1, footer2, footer3;
  final String kodeAkunKasToko, namaAkunKasToko;
  final String kodeAkunKasGudang, namaAkunKasGudang;
  final String kodeAkunTransfer, namaAkunTransfer;
  final String kodeAkunPiutang, namaAkunPiutang;
  final String kodeAkunBarang, namaAkunBarang;
}
```

Disimpan di `AuthProvider` saat login, dipakai di:
- Header nota (nama, alamat, footer)
- Default akun kas di screen pembayaran
- Default akun transfer di screen pembayaran

---

## Urutan Implementasi

```
Fase 1 — Backend PHP (tidak ada risiko break existing)
  1. get_perusahaan.php
  2. get_hak_akses.php
  3. get_dashboard_summary.php
  4. get_karyawan.php
  5. get_akun_coa.php
  6. get_laporan_stok.php
  7. Validasi get_opname_list.php

Fase 2 — Model & Provider Flutter
  8. PerusahaanModel + HakAksesModel
  9. Update AuthProvider (ambil perusahaan + hak akses saat login)
  10. PenjualanProvider (state management multi-screen)
  11. Update ApiService (tambah semua endpoint baru)

Fase 3 — UI Screens
  12. Login redesign
  13. Dashboard + Drawer sidebar
  14. Penjualan multi-screen (5 screen)
  15. Stok Opname redesign
  16. Laporan Stok screen baru
  17. List Opname redesign

Fase 4 — Cetak Thermal
  18. ThermalPrintService
  19. PrinterConfig + PrinterSettingsScreen
  20. NotaWidget (preview)
  21. Integrasi di Step 4 penjualan

Fase 5 — Verifikasi
  22. flutter analyze
  23. Test end-to-end
  24. Konsistensi nama "Kasir Lancar"
```
