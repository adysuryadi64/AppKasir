import 'dart:convert';
import 'package:device_info_plus/device_info_plus.dart';
import 'package:flutter/foundation.dart';
import 'package:package_info_plus/package_info_plus.dart';
import 'package:shared_preferences/shared_preferences.dart';

class StorageService {
  static late SharedPreferences _prefs;

  static Future<void> init() async {
    _prefs = await SharedPreferences.getInstance();
    // Ambil & simpan nama device dan versi app sekali saat init
    await Future.wait([_initDeviceName(), _initAppVersion()]);
  }

  // ── App Version ───────────────────────────────────────────────
  static const _keyAppVersion = 'app_version';

  static String getAppVersion() => _prefs.getString(_keyAppVersion) ?? '1.0.0';

  static Future<void> _initAppVersion() async {
    try {
      final info = await PackageInfo.fromPlatform();
      await _prefs.setString(_keyAppVersion, info.version);
      debugPrint('[App] 📦 versi: ${info.version}');
    } catch (e) {
      debugPrint('[App] ⚠️  gagal baca versi: $e');
    }
  }

  // ── Device Name ───────────────────────────────────────────────
  static const _keyDeviceName = 'device_name';

  static String getDeviceName() =>
      _prefs.getString(_keyDeviceName) ?? 'Android';

  static Future<void> _initDeviceName() async {
    // Jika sudah tersimpan, skip
    if (_prefs.containsKey(_keyDeviceName)) return;
    try {
      final info = DeviceInfoPlugin();
      final android = await info.androidInfo;
      // Contoh: "TECNO KI7" atau "Samsung Galaxy A54"
      final name = '${android.manufacturer} ${android.model}'.trim();
      await _prefs.setString(
        _keyDeviceName,
        name.isNotEmpty ? name : 'Android',
      );
      debugPrint('[Device] 📱 nama device: $name');
    } catch (e) {
      debugPrint('[Device] ⚠️  gagal baca device info: $e');
      await _prefs.setString(_keyDeviceName, 'Android');
    }
  }

  // ── Server Configuration ──────────────────────────────────────
  static const _keyServerUrl = 'server_url';
  static const _keyServerOk = 'server_configured';
  static const _keyServerUsername = 'server_username';
  static const _keyServerDatabase = 'server_database';

  static Future<void> saveServerConfig(
    String url,
    String username,
    String database,
  ) async {
    await _prefs.setString(_keyServerUrl, url);
    await _prefs.setString(_keyServerUsername, username);
    await _prefs.setString(_keyServerDatabase, database);
    await _prefs.setBool(_keyServerOk, true);
  }

  static Future<String?> getServerUrl() async =>
      _prefs.getString(_keyServerUrl);
  static Future<String?> getServerUsername() async =>
      _prefs.getString(_keyServerUsername);
  static Future<String?> getServerDatabase() async =>
      _prefs.getString(_keyServerDatabase);
  static Future<bool> isServerConfigured() async =>
      _prefs.getBool(_keyServerOk) ?? false;

  // ── Auth ──────────────────────────────────────────────────────
  static const _keyToken = 'auth_token';
  static const _keyUser = 'user_data';
  static const _keyLocation = 'selected_location';

  /// Token sesi — disimpan sebagai plain string
  static String? getToken() => _prefs.getString(_keyToken);
  static Future<void> setToken(String token) =>
      _prefs.setString(_keyToken, token);

  /// Data user — disimpan sebagai JSON string
  /// Return null jika belum pernah login atau sudah di-clear
  static Map<String, dynamic>? getUserData() {
    final raw = _prefs.getString(_keyUser);
    if (raw == null || raw.isEmpty) return null;
    try {
      final decoded = jsonDecode(raw);
      if (decoded is Map<String, dynamic>) return decoded;
      return null;
    } catch (_) {
      return null;
    }
  }

  static Future<void> setUserData(Map<String, dynamic> data) =>
      _prefs.setString(_keyUser, jsonEncode(data));

  /// Lokasi aktif (TOKO / GUDANG)
  static String? getSelectedLocation() => _prefs.getString(_keyLocation);
  static Future<void> setSelectedLocation(String loc) =>
      _prefs.setString(_keyLocation, loc);

  /// Hapus semua data sesi — dipanggil saat logout
  static Future<void> clearAuthData() async {
    await _prefs.remove(_keyToken);
    await _prefs.remove(_keyUser);
    await _prefs.remove(_keyLocation);
  }

  // ── Perusahaan & Hak Akses ────────────────────────────────────
  static const _keyPerusahaan = 'perusahaan_data';
  static const _keyHakAkses = 'hak_akses_data';

  static String? getPerusahaanJson() => _prefs.getString(_keyPerusahaan);
  static Future<void> setPerusahaanJson(String json) =>
      _prefs.setString(_keyPerusahaan, json);

  static String? getHakAksesJson() => _prefs.getString(_keyHakAkses);
  static Future<void> setHakAksesJson(String json) =>
      _prefs.setString(_keyHakAkses, json);

  /// Hapus semua data sesi termasuk perusahaan & hak akses
  static Future<void> clearAllData() async {
    await clearAuthData();
    await _prefs.remove(_keyPerusahaan);
    await _prefs.remove(_keyHakAkses);
  }

  // ── Location Default Preference ──────────────────────────────
  // Nilai: 'selalu_tanya' | 'TOKO' | 'GUDANG'
  static const _keyLocationDefault = 'location_default';

  static String getLocationDefault() =>
      _prefs.getString(_keyLocationDefault) ?? 'selalu_tanya';

  static Future<void> setLocationDefault(String val) =>
      _prefs.setString(_keyLocationDefault, val);

  // ── Settings ──────────────────────────────────────────────────
  static const _keyTheme = 'theme_mode';
  static const _keyLanguage = 'language';

  static String? getTheme() => _prefs.getString(_keyTheme);
  static Future<void> setTheme(String v) => _prefs.setString(_keyTheme, v);
  static String? getLanguage() => _prefs.getString(_keyLanguage);
  static Future<void> setLanguage(String v) =>
      _prefs.setString(_keyLanguage, v);
}
