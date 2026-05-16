import 'dart:async';
import 'package:flutter/material.dart';
import 'package:connectivity_plus/connectivity_plus.dart';
import '../services/api_service.dart';

class ConnectivityProvider extends ChangeNotifier {
  // Status jaringan HP
  bool _hasNetwork = false;

  // Status koneksi ke server API (ping)
  bool _serverReachable = false;
  bool _isCheckingServer = false;

  Timer? _pingTimer;

  bool get hasNetwork => _hasNetwork;
  bool get serverReachable => _serverReachable;
  bool get isConnected => _hasNetwork && _serverReachable;

  ConnectivityProvider() {
    _init();
  }

  Future<void> _init() async {
    // Cek jaringan awal
    final result = await Connectivity().checkConnectivity();
    _hasNetwork = _isNetworkAvailable(result);
    notifyListeners();

    // Listen perubahan jaringan
    Connectivity().onConnectivityChanged.listen((result) {
      _hasNetwork = _isNetworkAvailable(result);
      notifyListeners();
      if (_hasNetwork) _pingServer();
    });

    // Ping server pertama kali
    await _pingServer();

    // Ping server setiap 30 detik
    _pingTimer = Timer.periodic(
      const Duration(seconds: 30),
      (_) => _pingServer(),
    );
  }

  bool _isNetworkAvailable(dynamic result) {
    // connectivity_plus v5+ return List<ConnectivityResult>
    if (result is List) {
      return result.any((r) => r != ConnectivityResult.none);
    }
    if (result is ConnectivityResult) {
      return result != ConnectivityResult.none;
    }
    return false;
  }

  Future<void> _pingServer() async {
    if (_isCheckingServer) return;
    _isCheckingServer = true;
    try {
      final res = await ApiService.getUsers().timeout(
        const Duration(seconds: 4),
      );
      _serverReachable = res['status'] == 'success';
    } catch (_) {
      _serverReachable = false;
    } finally {
      _isCheckingServer = false;
      notifyListeners();
    }
  }

  /// Paksa ping ulang — dipanggil saat pull-to-refresh
  Future<void> refresh() => _pingServer();

  @override
  void dispose() {
    _pingTimer?.cancel();
    super.dispose();
  }

  // ── Getters untuk UI ──────────────────────────────────────────

  /// Icon wifi di header — berdasarkan status server, bukan jaringan HP
  IconData get wifiIcon {
    if (!_hasNetwork) return Icons.wifi_off;
    if (_isCheckingServer) return Icons.wifi_find;
    return _serverReachable ? Icons.wifi : Icons.wifi_off;
  }

  Color get wifiColor {
    if (!_hasNetwork || !_serverReachable) return Colors.red.shade400;
    return const Color(0xFF16A34A); // hijau
  }

  // Kompatibilitas dengan kode lama
  String get connectionStatusText {
    if (!_hasNetwork) return 'Offline';
    if (_serverReachable) return 'Terhubung';
    return 'Server tidak terjangkau';
  }

  Color get connectionStatusColor => wifiColor;

  IconData get connectionStatusIcon => wifiIcon;
}
