# Aplikasi Laporan Flutter — Plan & Dokumentasi

Aplikasi mobile/web untuk melihat laporan master dari semua toko.
Data diambil langsung dari Supabase via REST API — read-only, tidak ada write.

---

## Arsitektur

```
[Supabase Cloud]
   View Laporan (PostgreSQL)
        ↓  REST API (HTTPS)
[Flutter App]
   ├── Laporan Barang & Stok
   ├── Hutang Supplier
   ├── Piutang Pelanggan
   ├── Karyawan & Gaji
   └── Neraca & Laba Rugi
```

---

## Stack Teknologi

| Komponen | Pilihan |
|---|---|
| Framework | Flutter 3.x |
| State Management | Riverpod 2.x |
| HTTP Client | `supabase_flutter` package |
| Tabel/Grid | `data_table_2` |
| Chart | `fl_chart` |
| Export PDF | `pdf` + `printing` |
| Export Excel | `excel` |
| Local Cache | `hive` atau `shared_preferences` |

---

## Struktur Proyek

```
lib/
├── main.dart
├── core/
│   ├── supabase_client.dart     — init Supabase
│   ├── constants.dart           — URL, key, nama view
│   └── formatters.dart          — format angka, tanggal, mata uang
├── models/
│   ├── barang_model.dart
│   ├── stok_model.dart
│   ├── hutang_model.dart
│   ├── piutang_model.dart
│   ├── karyawan_model.dart
│   ├── gaji_model.dart
│   └── coa_model.dart
├── repositories/
│   ├── barang_repository.dart
│   ├── stok_repository.dart
│   ├── hutang_repository.dart
│   ├── piutang_repository.dart
│   ├── karyawan_repository.dart
│   ├── gaji_repository.dart
│   └── coa_repository.dart
├── providers/                   — Riverpod providers
│   ├── barang_provider.dart
│   ├── stok_provider.dart
│   └── ...
└── screens/
    ├── home/
    │   └── home_screen.dart
    ├── barang/
    │   ├── laporan_barang_screen.dart
    │   └── detail_barang_screen.dart
    ├── stok/
    │   ├── stok_semua_toko_screen.dart
    │   └── stok_per_toko_screen.dart
    ├── hutang/
    │   └── hutang_supliyer_screen.dart
    ├── piutang/
    │   └── piutang_pelanggan_screen.dart
    ├── karyawan/
    │   ├── karyawan_screen.dart
    │   └── gaji_screen.dart
    └── keuangan/
        ├── neraca_screen.dart
        ├── laba_rugi_screen.dart
        └── ringkasan_screen.dart
```

---

## Setup Supabase di Flutter

```yaml
# pubspec.yaml
dependencies:
  supabase_flutter: ^2.0.0
  riverpod: ^2.0.0
  flutter_riverpod: ^2.0.0
  data_table_2: ^2.5.0
  fl_chart: ^0.66.0
  pdf: ^3.10.0
  printing: ^5.12.0
  excel: ^4.0.0
  intl: ^0.19.0
```

```dart
// main.dart
void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Supabase.initialize(
    url: 'https://xxxx.supabase.co',
    anonKey: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...',
  );
  runApp(const ProviderScope(child: MyApp()));
}

// Akses client
final supabase = Supabase.instance.client;
```

---

## Contoh Repository

```dart
// repositories/barang_repository.dart
class BarangRepository {
  final _client = Supabase.instance.client;

  // Semua barang + stok total
  Future<List<Map<String, dynamic>>> getLaporanBarang({
    String? kodeKategori,
    String? search,
    int limit = 100,
    int offset = 0,
  }) async {
    var query = _client
        .from('v_laporan_barang')
        .select()
        .order('nama_barang');

    if (kodeKategori != null) {
      query = query.eq('kode_kategori', kodeKategori);
    }
    if (search != null && search.isNotEmpty) {
      query = query.ilike('nama_barang', '%$search%');
    }

    return await query.range(offset, offset + limit - 1);
  }

  // Stok detail per toko
  Future<List<Map<String, dynamic>>> getStokPerToko({
    String? kodeToko,
    String? idBarang,
  }) async {
    var query = _client
        .from('v_stok_detail_per_toko')
        .select()
        .order('nama_barang');

    if (kodeToko != null) query = query.eq('kode_toko', kodeToko);
    if (idBarang != null) query = query.eq('id_barang', idBarang);

    return await query;
  }
}
```

```dart
// repositories/keuangan_repository.dart
class KeuanganRepository {
  final _client = Supabase.instance.client;

  // Neraca per toko
  Future<List<Map<String, dynamic>>> getNeraca(String kodeToko) async {
    return await _client
        .from('v_neraca')
        .select()
        .eq('kode_toko', kodeToko)
        .order('kode_akun');
  }

  // Laba rugi per toko
  Future<List<Map<String, dynamic>>> getLabaRugi(String kodeToko) async {
    return await _client
        .from('v_laba_rugi')
        .select()
        .eq('kode_toko', kodeToko)
        .order('kode_akun');
  }

  // Ringkasan laba bersih semua toko
  Future<List<Map<String, dynamic>>> getRingkasanLabaRugi() async {
    return await _client
        .from('v_ringkasan_laba_rugi')
        .select()
        .order('kode_toko');
  }

  // Hutang supplier total semua toko
  Future<List<Map<String, dynamic>>> getHutangSupliyer() async {
    return await _client
        .from('v_hutang_supliyer_total')
        .select()
        .order('total_sisa_hutang', ascending: false);
  }

  // Piutang pelanggan total semua toko
  Future<List<Map<String, dynamic>>> getPiutangPelanggan() async {
    return await _client
        .from('v_piutang_pelanggan_total')
        .select()
        .order('total_sisa_piutang', ascending: false);
  }
}
```

---

## Contoh Provider (Riverpod)

```dart
// providers/barang_provider.dart
final barangRepositoryProvider = Provider((ref) => BarangRepository());

final laporanBarangProvider = FutureProvider.family<
    List<Map<String, dynamic>>, Map<String, dynamic>>((ref, params) async {
  final repo = ref.read(barangRepositoryProvider);
  return repo.getLaporanBarang(
    kodeKategori: params['kode_kategori'],
    search: params['search'],
    limit: params['limit'] ?? 100,
    offset: params['offset'] ?? 0,
  );
});

final ringkasanLabaRugiProvider = FutureProvider((ref) async {
  final repo = KeuanganRepository();
  return repo.getRingkasanLabaRugi();
});
```

---

## Halaman Utama (Home)

```
┌─────────────────────────────────────┐
│  LAPORAN KASIR LANCAR               │
│  [Pilih Toko: Semua ▼]              │
├──────────┬──────────┬───────────────┤
│ 📦 Barang│ 📊 Stok  │ 💰 Keuangan  │
│  & Harga │ per Toko │               │
├──────────┴──────────┴───────────────┤
│ 👥 Hutang    │ 📋 Piutang           │
│  Supplier    │  Pelanggan           │
├──────────────┴──────────────────────┤
│ 👨 Karyawan  │ 💵 Gaji              │
├──────────────┴──────────────────────┤
│ 📈 Neraca    │ 📉 Laba Rugi         │
└─────────────────────────────────────┘
```

---

## Fitur Per Halaman

### Laporan Barang
- Tabel: kode, nama, kategori, supplier, harga beli, harga jual (3 level), stok total, nilai stok
- Filter: kategori, supplier, search nama/barcode
- Sort: nama, harga, stok
- Export: PDF, Excel

### Stok Per Toko
- Tabel: toko, barang, stok toko, stok gudang, stok total, nilai stok
- Filter: per toko, per barang
- Highlight: stok di bawah minimum (merah), di atas maksimum (kuning)

### Hutang Supplier
- Tabel: supplier, hutang awal, total hutang, total bayar, sisa hutang
- Filter: per toko atau semua toko
- Sort: sisa hutang terbesar

### Piutang Pelanggan
- Tabel: pelanggan, jenis, piutang awal, total piutang, total bayar, sisa piutang
- Filter: per toko, jenis pelanggan
- Sort: sisa piutang terbesar

### Karyawan & Gaji
- Karyawan: kode, nama, jabatan, gaji pokok, saldo bon
- Gaji: bulan, nama, pendapatan, potongan, terima bersih
- Filter: per toko, per bulan

### Neraca & Laba Rugi
- Neraca: akun aktiva, pasiva, modal dengan saldo akhir
- Laba Rugi: pendapatan, HPP, beban, laba bersih
- Ringkasan: total pendapatan, total beban, laba bersih per toko
- Chart: pie chart komposisi aset, bar chart laba per toko

---

## Endpoint Supabase yang Dipakai

```
GET /rest/v1/v_laporan_barang
GET /rest/v1/v_stok_detail_per_toko?kode_toko=eq.TOKO1
GET /rest/v1/v_hutang_supliyer_total
GET /rest/v1/v_piutang_pelanggan_total
GET /rest/v1/v_karyawan?kode_toko=eq.TOKO1
GET /rest/v1/v_gaji_ringkasan?bulan=eq.April 2026
GET /rest/v1/v_gaji_total_per_bulan
GET /rest/v1/v_neraca?kode_toko=eq.TOKO1
GET /rest/v1/v_laba_rugi?kode_toko=eq.TOKO1
GET /rest/v1/v_ringkasan_laba_rugi
```

Header yang diperlukan:
```
apikey: <supabase_anon_key>
Authorization: Bearer <supabase_anon_key>
```

---

## Keamanan

Untuk production, aktifkan RLS di Supabase dan buat policy:

```sql
-- Hanya user yang login bisa baca view laporan
ALTER TABLE stok_per_toko ENABLE ROW LEVEL SECURITY;
CREATE POLICY "read_only" ON stok_per_toko
    FOR SELECT USING (auth.role() = 'authenticated');
```

Opsi autentikasi Flutter:
- Email + password via `supabase.auth.signInWithPassword()`
- Atau gunakan API key khusus yang dibatasi hanya SELECT

---

## Urutan Pengembangan

1. Setup project Flutter + koneksi Supabase
2. Model + Repository untuk barang & stok
3. Home screen + navigasi
4. Laporan barang (tabel + filter + search)
5. Stok per toko
6. Hutang & piutang
7. Karyawan & gaji
8. Neraca & laba rugi
9. Chart & dashboard
10. Export PDF/Excel
11. Autentikasi
12. Deploy (Android/iOS/Web)
