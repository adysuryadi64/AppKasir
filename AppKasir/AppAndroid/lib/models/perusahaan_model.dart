import 'dart:convert';

class AkunCOA {
  final String kode;
  final String nama;

  const AkunCOA({required this.kode, required this.nama});

  factory AkunCOA.fromJson(Map<String, dynamic> json) => AkunCOA(
    kode: json['kode']?.toString() ?? '',
    nama: json['nama']?.toString() ?? '',
  );

  Map<String, dynamic> toJson() => {'kode': kode, 'nama': nama};

  bool get isEmpty => kode.isEmpty && nama.isEmpty;
}

class PerusahaanModel {
  final String kode;
  final String nama;
  final String alamat;
  final String kota;
  final String hp;
  final String pemilik;
  final String footer1;
  final String footer2;
  final String footer3;
  final AkunCOA akunKasToko;
  final AkunCOA akunKasGudang;
  final AkunCOA akunTransfer;
  final AkunCOA akunPiutang;
  final AkunCOA akunBarang;

  const PerusahaanModel({
    required this.kode,
    required this.nama,
    required this.alamat,
    required this.kota,
    required this.hp,
    required this.pemilik,
    required this.footer1,
    required this.footer2,
    required this.footer3,
    required this.akunKasToko,
    required this.akunKasGudang,
    required this.akunTransfer,
    required this.akunPiutang,
    required this.akunBarang,
  });

  factory PerusahaanModel.fromJson(Map<String, dynamic> json) {
    AkunCOA akun(String key) {
      final v = json[key];
      if (v is Map<String, dynamic>) {
        return AkunCOA.fromJson(v);
      }
      return const AkunCOA(kode: '', nama: '');
    }

    return PerusahaanModel(
      kode: json['kode']?.toString() ?? '',
      nama: json['nama']?.toString() ?? '',
      alamat: json['alamat']?.toString() ?? '',
      kota: json['kota']?.toString() ?? '',
      hp: json['hp']?.toString() ?? '',
      pemilik: json['pemilik']?.toString() ?? '',
      footer1: json['footer1']?.toString() ?? '',
      footer2: json['footer2']?.toString() ?? '',
      footer3: json['footer3']?.toString() ?? '',
      akunKasToko: akun('akun_kas_toko'),
      akunKasGudang: akun('akun_kas_gudang'),
      akunTransfer: akun('akun_transfer'),
      akunPiutang: akun('akun_piutang'),
      akunBarang: akun('akun_barang'),
    );
  }

  Map<String, dynamic> toJson() => {
    'kode': kode,
    'nama': nama,
    'alamat': alamat,
    'kota': kota,
    'hp': hp,
    'pemilik': pemilik,
    'footer1': footer1,
    'footer2': footer2,
    'footer3': footer3,
    'akun_kas_toko': akunKasToko.toJson(),
    'akun_kas_gudang': akunKasGudang.toJson(),
    'akun_transfer': akunTransfer.toJson(),
    'akun_piutang': akunPiutang.toJson(),
    'akun_barang': akunBarang.toJson(),
  };

  String toJsonString() => jsonEncode(toJson());

  static PerusahaanModel? fromJsonString(String? s) {
    if (s == null || s.isEmpty) return null;
    try {
      final decoded = jsonDecode(s);
      if (decoded is Map<String, dynamic>) {
        return PerusahaanModel.fromJson(decoded);
      }
      return null;
    } catch (_) {
      return null;
    }
  }

  /// Akun kas sesuai lokasi login
  AkunCOA akunKasUntukLokasi(String lokasi) =>
      lokasi == 'GUDANG' ? akunKasGudang : akunKasToko;
}
