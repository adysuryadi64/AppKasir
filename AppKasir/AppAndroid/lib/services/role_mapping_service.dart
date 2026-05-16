import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

/// Mapping antara key JSON hak akses → nama Role di tabel hakaksesuser DB.
/// Nama Role diambil dari Label.Text di FormGeneralSetting VB.
/// User bisa mengubah nama ini jika label di VB berubah.
class RoleMappingService {
  static const _prefKey = 'hak_akses_role_mapping';

  /// Default mapping — sesuai Label.Text di FormGeneralSetting.Designer.vb
  static const Map<String, String> defaultMapping = {
    'izinkan_ubah_harga': 'Izinkan user mengubah harga jual',
    'izinkan_jual_rugi': 'Izinkan jual barang di bawah harga beli',
    'izinkan_jual_stok_minus':
        'Izinkan transaksi keluar barang meski stok jadi minus',
    'izinkan_satuan_berbeda': 'Izinkan kode barang dengan satuan berbeda',
    'tampil_info_stok': 'Tampilkan informasi stok saat transaksi',
    'langsung_isi_nominal': 'Langsung isi nominal total transaksi',
    'izinkan_nominal_nol': 'Izinkan penjualan dengan nominal 0',
    'izinkan_tanggal_lampau':
        'Semua transaksi boleh menggunakan tanggal lampau',
  };

  /// Label deskriptif untuk ditampilkan di UI
  static const Map<String, String> keyLabels = {
    'izinkan_ubah_harga': 'Ubah Harga Jual',
    'izinkan_jual_rugi': 'Jual di Bawah Harga Beli',
    'izinkan_jual_stok_minus': 'Jual Stok Minus',
    'izinkan_satuan_berbeda': 'Satuan Berbeda 1 Transaksi',
    'tampil_info_stok': 'Tampil Info Stok',
    'langsung_isi_nominal': 'Langsung Isi Nominal',
    'izinkan_nominal_nol': 'Nominal Jual 0',
    'izinkan_tanggal_lampau': 'Transaksi Tanggal Lampau',
  };

  /// Baca mapping dari SharedPreferences.
  /// Jika belum ada, kembalikan defaultMapping.
  static Future<Map<String, String>> load() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final json = prefs.getString(_prefKey);
      if (json == null || json.isEmpty) return Map.from(defaultMapping);
      final decoded = jsonDecode(json) as Map<String, dynamic>;
      // Merge dengan default agar key baru tidak hilang
      final result = Map<String, String>.from(defaultMapping);
      decoded.forEach((k, v) {
        if (result.containsKey(k) && v is String && v.isNotEmpty) {
          result[k] = v;
        }
      });
      return result;
    } catch (e) {
      debugPrint('[RoleMapping] load error: $e');
      return Map.from(defaultMapping);
    }
  }

  /// Simpan mapping ke SharedPreferences.
  static Future<void> save(Map<String, String> mapping) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      await prefs.setString(_prefKey, jsonEncode(mapping));
      debugPrint('[RoleMapping] saved');
    } catch (e) {
      debugPrint('[RoleMapping] save error: $e');
    }
  }

  /// Reset ke default.
  static Future<void> reset() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      await prefs.remove(_prefKey);
      debugPrint('[RoleMapping] reset to default');
    } catch (e) {
      debugPrint('[RoleMapping] reset error: $e');
    }
  }
}
