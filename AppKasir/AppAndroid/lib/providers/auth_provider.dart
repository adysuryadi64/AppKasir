import 'package:flutter/foundation.dart';
import '../services/api_service.dart';
import '../services/storage_service.dart';
import '../services/role_mapping_service.dart';
import '../models/perusahaan_model.dart';
import '../models/hak_akses_model.dart';

class AuthProvider extends ChangeNotifier {
  bool _isLoggedIn = false;
  bool _isLoading = false;
  String? _errorMessage;
  Map<String, dynamic>? _userData;
  String? _selectedLocation;
  String? _serverUrl;
  String? _username;
  String? _database;
  bool _isServerConfigured = false;
  PerusahaanModel? _perusahaan;
  HakAksesModel _hakAkses = HakAksesModel.defaultPermissive;

  // ── Getters ───────────────────────────────────────────────────
  bool get isLoggedIn => _isLoggedIn;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;
  Map<String, dynamic>? get userData => _userData;
  String? get selectedLocation => _selectedLocation;
  String? get serverUrl => _serverUrl;
  String? get username => _username;
  String? get database => _database;
  bool get isServerConfigured => _isServerConfigured;
  PerusahaanModel? get perusahaan => _perusahaan;
  HakAksesModel get hakAkses => _hakAkses;

  String get userName => _userData?['USER_NAME'] ?? '';
  String get userFullName => _userData?['NAMA_USER'] ?? '';
  String get userLevel => _userData?['LVL'] ?? '';
  String get namaPerusahaan => _perusahaan?.nama ?? 'Kasir Lancar';
  String get deviceName => StorageService.getDeviceName();

  // ── Constructor ───────────────────────────────────────────────
  AuthProvider() {
    _initializeAuth();
  }

  // ── Init ──────────────────────────────────────────────────────
  Future<void> _initializeAuth() async {
    _isLoading = true;
    notifyListeners();

    try {
      _isServerConfigured = await StorageService.isServerConfigured();
      _serverUrl = await StorageService.getServerUrl();
      _username = await StorageService.getServerUsername();
      _database = await StorageService.getServerDatabase();

      // Jika database kosong, coba ambil dari server
      if ((_database == null || _database!.isEmpty) && _isServerConfigured) {
        try {
          final info = await ApiService.getServerInfo().timeout(
            const Duration(seconds: 4),
          );
          if (info['status'] == 'success') {
            final dbName = info['data']?['db_name']?.toString() ?? '';
            if (dbName.isNotEmpty) {
              _database = dbName;
              await StorageService.saveServerConfig(
                _serverUrl ?? '',
                _username ?? '',
                dbName,
              );
              debugPrint('[Auth]    database dari server: $dbName');
            }
          }
        } catch (_) {}
      }

      debugPrint('[Auth] 🔄 initializeAuth');
      debugPrint('[Auth]    serverConfigured=$_isServerConfigured');
      debugPrint('[Auth]    serverUrl=$_serverUrl');

      // Load cached perusahaan & hak akses (tetap dipakai walau harus login ulang)
      _perusahaan = PerusahaanModel.fromJsonString(
        StorageService.getPerusahaanJson(),
      );
      final cachedHak = HakAksesModel.fromJsonString(
        StorageService.getHakAksesJson(),
      );
      if (cachedHak != null) _hakAkses = cachedHak;

      debugPrint(
        '[Auth]    perusahaan=${_perusahaan != null ? '✅ ${_perusahaan!.nama}' : '❌ null'}',
      );

      // ── PRODUKSI: selalu paksa login ulang saat app dibuka ────
      // Token tidak dipakai untuk auto-login — user harus input password setiap sesi
      debugPrint('[Auth] 🔒 Produksi: paksa login ulang setiap sesi');
      _isLoggedIn = false;
    } catch (e) {
      debugPrint('[Auth] ❌ initializeAuth error: $e');
      _errorMessage = 'Gagal inisialisasi autentikasi';
      _isLoggedIn = false;
    }

    debugPrint('[Auth] 🏁 initializeAuth selesai: isLoggedIn=$_isLoggedIn');
    _isLoading = false;
    notifyListeners();
  }

  // ── Login ─────────────────────────────────────────────────────
  Future<bool> login(String username, String password) async {
    debugPrint('[Auth] 🔐 login: user=$username');
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      final response = await ApiService.login(username, password);
      debugPrint('[Auth]    response status=${response['status']}');

      if (response['status'] == 'success') {
        _userData = response['data'] as Map<String, dynamic>?;
        _isLoggedIn = true;

        final token = response['token']?.toString() ?? '';
        debugPrint(
          '[Auth]    token: ${token.isNotEmpty ? '✅ ${token.substring(0, 8)}...' : '⚠️  kosong'}',
        );
        await StorageService.setToken(token);
        await StorageService.setUserData(_userData!);

        // Restore lokasi: jika preferensi default bukan 'selalu_tanya',
        // langsung pakai nilai default tanpa tampilkan halaman pilih lokasi
        final locDefault = StorageService.getLocationDefault();
        if (locDefault != 'selalu_tanya') {
          _selectedLocation = locDefault;
          await StorageService.setSelectedLocation(locDefault);
          debugPrint('[Auth]    lokasi dari preferensi default: $locDefault');
        } else {
          // Selalu tanya — reset lokasi agar AuthWrapper arahkan ke LocationSelectionScreen
          _selectedLocation = null;
          debugPrint('[Auth]    preferensi: selalu tanya lokasi');
        }

        // Load data perusahaan & hak akses setelah login berhasil
        await _loadPerusahaanDanHakAkses();

        debugPrint(
          '[Auth] ✅ Login berhasil: ${_userData!['USER_NAME']} (${_userData!['NAMA_USER']}) level=${_userData!['LVL']}',
        );
        return true;
      } else {
        _errorMessage = response['message']?.toString() ?? 'Login gagal';
        debugPrint('[Auth] ❌ Login gagal: $_errorMessage');
        return false;
      }
    } catch (e) {
      _errorMessage = e.toString();
      debugPrint('[Auth] ❌ Login exception: $e');
      return false;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  /// Load data perusahaan dan hak akses dari server, simpan ke cache
  Future<void> _loadPerusahaanDanHakAkses() async {
    try {
      // Load role mapping dari SharedPreferences sebelum request
      final roleMapping = await RoleMappingService.load();

      final results = await Future.wait([
        ApiService.getDataPerusahaan().catchError((Object e) {
          debugPrint('[Auth] ⚠️  getDataPerusahaan gagal: $e');
          return <String, dynamic>{'status': 'error'};
        }),
        ApiService.getHakAkses(roleMapping: roleMapping).catchError((Object e) {
          debugPrint('[Auth] ⚠️  getHakAkses gagal: $e');
          return <String, dynamic>{'status': 'error'};
        }),
      ]);

      // Perusahaan
      final resPerusahaan = results[0];
      if (resPerusahaan['status'] == 'success' &&
          resPerusahaan['data'] != null) {
        _perusahaan = PerusahaanModel.fromJson(
          resPerusahaan['data'] as Map<String, dynamic>,
        );
        await StorageService.setPerusahaanJson(_perusahaan!.toJsonString());
        debugPrint('[Auth] ✅ Data perusahaan: ${_perusahaan!.nama}');
      } else {
        debugPrint('[Auth] ⚠️  Data perusahaan tidak tersedia, pakai cache');
      }

      // Hak akses
      final resHak = results[1];
      if (resHak['status'] == 'success' && resHak['data'] != null) {
        _hakAkses = HakAksesModel.fromJson(
          resHak['data'] as Map<String, dynamic>,
        );
        await StorageService.setHakAksesJson(_hakAkses.toJsonString());
        debugPrint('[Auth] ✅ Hak akses dimuat');
      } else {
        debugPrint(
          '[Auth] ⚠️  Hak akses tidak tersedia, pakai default permissive',
        );
      }
    } catch (e) {
      debugPrint('[Auth] ⚠️  _loadPerusahaanDanHakAkses error: $e');
    }
  }

  // ── Logout ────────────────────────────────────────────────────
  Future<void> logout() async {
    _isLoading = true;
    notifyListeners();

    try {
      await StorageService.clearAuthData();
      _isLoggedIn = false;
      _userData = null;
      _selectedLocation = null;
      _errorMessage = null;
      // Pertahankan _perusahaan dan _hakAkses agar tidak perlu reload saat login ulang
    } catch (e) {
      _errorMessage = 'Gagal logout';
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  // ── Lokasi ────────────────────────────────────────────────────
  Future<bool> selectLocation(String location) async {
    try {
      _selectedLocation = location;
      await StorageService.setSelectedLocation(location);
      notifyListeners();
      return true;
    } catch (e) {
      _errorMessage = 'Gagal memilih lokasi';
      return false;
    }
  }

  // ── Server Config ─────────────────────────────────────────────
  Future<bool> configureServer(
    String serverUrl, [
    String? username,
    String? password,
    String? database,
  ]) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      if (!serverUrl.startsWith('http://') &&
          !serverUrl.startsWith('https://')) {
        serverUrl = 'http://$serverUrl';
      }

      await StorageService.saveServerConfig(
        serverUrl,
        username ?? '',
        database ?? '',
      );

      _serverUrl = serverUrl;
      _username = username;
      _database = database;
      _isServerConfigured = true;

      return true;
    } catch (e) {
      _errorMessage = 'Gagal konfigurasi server: $e';
      return false;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  void clearError() {
    _errorMessage = null;
    notifyListeners();
  }
}
