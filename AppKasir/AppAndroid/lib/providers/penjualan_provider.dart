import 'dart:math';
import 'package:flutter/foundation.dart';

/// Satu item di keranjang penjualan
class CartItem {
  final String idBarang;
  final String namaBarang;
  final String satuan;
  final double isiSatuan;
  final double hargaBeli;
  double hargaJual;
  double qty;
  double diskonPersen;
  double diskonRp;
  double stokToko;
  double stokGudang;

  CartItem({
    required this.idBarang,
    required this.namaBarang,
    required this.satuan,
    required this.isiSatuan,
    required this.hargaBeli,
    required this.hargaJual,
    required this.qty,
    this.diskonPersen = 0,
    this.diskonRp = 0,
    this.stokToko = 0,
    this.stokGudang = 0,
  });

  /// Total diskon per baris (Rp) — diskonRp adalah per satuan, sama dengan pola VB
  double get totalDiskon => qty * diskonRp;

  /// Total harga setelah diskon
  double get totalHarga => (hargaJual * qty) - totalDiskon;

  /// Qty dalam satuan terkecil
  double get qtySatuan => qty * isiSatuan;

  /// Laba per baris
  double get laba => totalHarga - (hargaBeli * qtySatuan);

  Map<String, dynamic> toPayload() => {
    'id_barang': idBarang,
    'nama_barang': namaBarang,
    'satuan': satuan,
    'isi_satuan': isiSatuan,
    'harga_beli': hargaBeli,
    'harga_jual': hargaJual,
    'qty': qty,
    'qty_satuan': qtySatuan,
    'diskon_persen': diskonPersen,
    'diskon_rp': diskonRp,
    'total_diskon': totalDiskon,
    'total_harga': totalHarga,
    'laba': laba,
    'serial_number': '',
    'harga_beli_satuan': hargaBeli,
  };
}

class PenjualanProvider extends ChangeNotifier {
  // ── Step 1: Header ────────────────────────────────────────────
  DateTime tanggal = DateTime.now();
  Map<String, dynamic>? selectedPelanggan;
  Map<String, dynamic>? selectedSales;

  // ── Step 2: Items ─────────────────────────────────────────────
  final List<CartItem> cartItems = [];

  // ── Step 3: Rincian harga ─────────────────────────────────────
  double diskonPersen = 0;
  double diskonRp = 0;
  double pajakPersen = 0;
  double pajakRp = 0;
  double biayaKirim = 0;

  // ── Step 4: Pembayaran ────────────────────────────────────────
  double nominalTunai = 0;
  double nominalTransfer = 0;

  // Akun COA — diisi dari tbl_perusahaan saat screen pembayaran dibuka
  Map<String, dynamic>? akunKas;
  Map<String, dynamic>? akunTransfer;

  // Info transfer
  String bank = '';
  String noRek = '';
  String namaRek = '';
  String noRef = '';

  // Jatuh tempo (untuk transaksi hutang)
  DateTime? jatuhTempo;

  // ── Computed ──────────────────────────────────────────────────
  double get subtotal => cartItems.fold(0, (s, i) => s + i.totalHarga);

  double get diskonGlobalRp {
    if (diskonPersen > 0) return subtotal * diskonPersen / 100;
    return diskonRp;
  }

  double get pajakNominal {
    if (pajakPersen > 0) return (subtotal - diskonGlobalRp) * pajakPersen / 100;
    return pajakRp;
  }

  double get grandTotal =>
      subtotal - diskonGlobalRp + pajakNominal + biayaKirim;

  double get totalBayar => nominalTunai + nominalTransfer;

  double get sisaTagihan => max(0, grandTotal - totalBayar);

  double get kembali => max(0, totalBayar - grandTotal);

  bool get isLunas => sisaTagihan == 0;

  double get totalHPP =>
      cartItems.fold(0, (s, i) => s + (i.hargaBeli * i.qtySatuan));

  /// Laba header = SUM(TotalHarga per baris) - SUM(Totalhargabeli) - diskonGlobal
  /// Identik dengan VB Simpanpenjualan:
  ///   laba = (totalHarga - totalHargaBeli) - diskon
  ///   totalHarga     = SUM(row.Cells("TotalHarga"))    = subtotal (sudah dikurangi diskon item)
  ///   totalHargaBeli = SUM(row.Cells("Totalhargabeli")) = totalHPP
  ///   diskon         = TxtDiskonRp                     = diskonGlobalRp
  double get laba => subtotal - totalHPP - diskonGlobalRp;

  double get diskonItemTotal => cartItems.fold(0, (s, i) => s + i.totalDiskon);

  // ── Cart operations ───────────────────────────────────────────

  /// Tambah item — jika sudah ada (id + satuan sama) tambah qty
  void addItem(CartItem item, {bool izinkanSatuanBerbeda = true}) {
    final idx = cartItems.indexWhere(
      (e) =>
          e.idBarang == item.idBarang &&
          (izinkanSatuanBerbeda || e.satuan == item.satuan),
    );
    if (idx >= 0) {
      cartItems[idx].qty += item.qty;
    } else {
      cartItems.add(item);
    }
    notifyListeners();
  }

  void removeItem(int index) {
    if (index >= 0 && index < cartItems.length) {
      cartItems.removeAt(index);
      notifyListeners();
    }
  }

  void updateQty(int index, double qty) {
    if (index >= 0 && index < cartItems.length) {
      cartItems[index].qty = qty;
      notifyListeners();
    }
  }

  void updateHarga(int index, double harga) {
    if (index >= 0 && index < cartItems.length) {
      cartItems[index].hargaJual = harga;
      notifyListeners();
    }
  }

  void updateDiskonItem(int index, {double persen = 0, double rp = 0}) {
    if (index >= 0 && index < cartItems.length) {
      cartItems[index].diskonPersen = persen;
      cartItems[index].diskonRp = rp;
      notifyListeners();
    }
  }

  // ── Diskon global ─────────────────────────────────────────────
  void setDiskonPersen(double persen) {
    diskonPersen = persen;
    if (persen > 0) diskonRp = 0; // persen override Rp
    notifyListeners();
  }

  void setDiskonRp(double rp) {
    diskonRp = rp;
    if (rp > 0) diskonPersen = 0; // Rp override persen
    notifyListeners();
  }

  void setPajakPersen(double persen) {
    pajakPersen = persen;
    if (persen > 0) pajakRp = 0;
    notifyListeners();
  }

  void setPajakRp(double rp) {
    pajakRp = rp;
    if (rp > 0) pajakPersen = 0;
    notifyListeners();
  }

  void setBiayaKirim(double rp) {
    biayaKirim = rp;
    notifyListeners();
  }

  void setTanggal(DateTime tgl) {
    tanggal = tgl;
    notifyListeners();
  }

  void setSelectedSales(Map<String, dynamic>? sales) {
    selectedSales = sales;
    notifyListeners();
  }

  // ── Pembayaran ────────────────────────────────────────────────
  void setNominalTunai(double v) {
    nominalTunai = v;
    notifyListeners();
  }

  void setNominalTransfer(double v) {
    nominalTransfer = v;
    notifyListeners();
  }

  void setAkunKas(Map<String, dynamic>? akun) {
    akunKas = akun;
    notifyListeners();
  }

  void setAkunTransfer(Map<String, dynamic>? akun) {
    akunTransfer = akun;
    notifyListeners();
  }

  void setInfoTransfer({
    String? bankVal,
    String? noRekVal,
    String? namaRekVal,
    String? noRefVal,
  }) {
    if (bankVal != null) bank = bankVal;
    if (noRekVal != null) noRek = noRekVal;
    if (namaRekVal != null) namaRek = namaRekVal;
    if (noRefVal != null) noRef = noRefVal;
    notifyListeners();
  }

  void setJatuhTempo(DateTime? tgl) {
    jatuhTempo = tgl;
    notifyListeners();
  }

  // ── Reset ─────────────────────────────────────────────────────
  void reset() {
    tanggal = DateTime.now();
    selectedPelanggan = null;
    selectedSales = null;
    cartItems.clear();
    diskonPersen = 0;
    diskonRp = 0;
    pajakPersen = 0;
    pajakRp = 0;
    biayaKirim = 0;
    nominalTunai = 0;
    nominalTransfer = 0;
    akunKas = null;
    akunTransfer = null;
    bank = '';
    noRek = '';
    namaRek = '';
    noRef = '';
    jatuhTempo = null;
    notifyListeners();
  }

  // ── Build payload untuk sync_penjualan.php ────────────────────

  Map<String, dynamic> buildPayload({
    required String idUser,
    required String idKomputer,
    required String lokasi,
  }) {
    final statusBayar = isLunas ? 'TERBAYAR' : 'TERHUTANG';
    final metode = nominalTransfer > 0 ? 'Tunai + Transfer' : 'Tunai';
    final jatuhTempoStr = jatuhTempo != null
        ? jatuhTempo!.toIso8601String()
        : isLunas
        ? ''
        : DateTime.now().add(const Duration(days: 30)).toIso8601String();

    return {
      'lokasi': lokasi,
      'id_user': idUser,
      'id_komputer': idKomputer,
      'tgl_transaksi': tanggal.toIso8601String(),
      'id_pelanggan': selectedPelanggan?['KODE']?.toString() ?? '',
      'nama_pelanggan': selectedPelanggan?['NAMA']?.toString() ?? '',
      'alamat_pelanggan': selectedPelanggan?['ALAMAT']?.toString() ?? '',
      'jenis_pelanggan': selectedPelanggan?['JENIS']?.toString() ?? 'UMUM',
      'grand_total_sbl_pajak': subtotal,
      'diskon_total_persen': diskonPersen,
      'diskon_total_rp': diskonGlobalRp,
      'pajak_persen': pajakPersen,
      'pajak_rp': pajakNominal,
      'biaya_kirim': biayaKirim,
      'grand_total_stl_pajak': grandTotal,
      'total_hpp': totalHPP,
      'laba': laba,
      'bayar': nominalTunai,
      'nominal_transfer': nominalTransfer,
      'kembali': kembali,
      'sisa_tagihan': sisaTagihan,
      'status_bayar': statusBayar,
      'status_transaksi': isLunas ? 'Lunas' : 'Belum Lunas',
      'jatuh_tempo': jatuhTempoStr,
      'metode': metode,
      // Akun kas — dari dropdown step3, diambil dari tbl_perusahaan
      'kode_akun_kas': akunKas?['KODE_AKUN']?.toString() ?? '',
      'nama_akun_kas': akunKas?['NAMA_AKUN']?.toString() ?? '',
      // Akun transfer — dari dropdown step3, diambil dari tbl_perusahaan
      'kode_akun_transfer': akunTransfer?['KODE_AKUN']?.toString() ?? '',
      'nama_akun_transfer': akunTransfer?['NAMA_AKUN']?.toString() ?? '',
      'bank': bank,
      'no_rekening': noRek,
      'nama_rekening': namaRek,
      'no_referensi': noRef,
      'id_sales': selectedSales?['Kode']?.toString() ?? '',
      'nama_sales': selectedSales?['Nama']?.toString() ?? '',
      'items': cartItems.map((i) => i.toPayload()).toList(),
    };
  }
}
