import 'dart:convert';

class HakAksesModel {
  final bool izinkanUbahHarga;
  final bool izinkanJualRugi;
  final bool izinkanJualStokMinus;
  final bool izinkanSatuanBerbeda;
  final bool tampilInfoStok;
  final bool langsungIsiNominal;
  final bool izinkanNominalNol;
  final bool izinkanTanggalLampau;

  const HakAksesModel({
    this.izinkanUbahHarga = true,
    this.izinkanJualRugi = true,
    this.izinkanJualStokMinus = true,
    this.izinkanSatuanBerbeda = true,
    this.tampilInfoStok = true,
    this.langsungIsiNominal = false,
    this.izinkanNominalNol = false,
    this.izinkanTanggalLampau = false,
  });

  /// Default permissive — dipakai jika server tidak bisa dihubungi
  static const HakAksesModel defaultPermissive = HakAksesModel();

  factory HakAksesModel.fromJson(Map<String, dynamic> json) => HakAksesModel(
    izinkanUbahHarga: json['izinkan_ubah_harga'] as bool? ?? true,
    izinkanJualRugi: json['izinkan_jual_rugi'] as bool? ?? true,
    izinkanJualStokMinus: json['izinkan_jual_stok_minus'] as bool? ?? true,
    izinkanSatuanBerbeda: json['izinkan_satuan_berbeda'] as bool? ?? true,
    tampilInfoStok: json['tampil_info_stok'] as bool? ?? true,
    langsungIsiNominal: json['langsung_isi_nominal'] as bool? ?? false,
    izinkanNominalNol: json['izinkan_nominal_nol'] as bool? ?? false,
    izinkanTanggalLampau: json['izinkan_tanggal_lampau'] as bool? ?? false,
  );

  Map<String, dynamic> toJson() => {
    'izinkan_ubah_harga': izinkanUbahHarga,
    'izinkan_jual_rugi': izinkanJualRugi,
    'izinkan_jual_stok_minus': izinkanJualStokMinus,
    'izinkan_satuan_berbeda': izinkanSatuanBerbeda,
    'tampil_info_stok': tampilInfoStok,
    'langsung_isi_nominal': langsungIsiNominal,
    'izinkan_nominal_nol': izinkanNominalNol,
    'izinkan_tanggal_lampau': izinkanTanggalLampau,
  };

  String toJsonString() => jsonEncode(toJson());

  static HakAksesModel? fromJsonString(String? s) {
    if (s == null || s.isEmpty) return null;
    try {
      final decoded = jsonDecode(s);
      if (decoded is Map<String, dynamic>) {
        return HakAksesModel.fromJson(decoded);
      }
      return null;
    } catch (_) {
      return null;
    }
  }
}
