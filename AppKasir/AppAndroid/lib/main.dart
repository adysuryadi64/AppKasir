import 'package:flutter/material.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:provider/provider.dart';

import 'providers/auth_provider.dart';
import 'providers/connectivity_provider.dart';
import 'providers/penjualan_provider.dart';
import 'screens/login_screen.dart';
import 'screens/dashboard_screen.dart';
import 'screens/location_selection_screen.dart';
import 'screens/server_config_screen.dart';
import 'services/storage_service.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await StorageService.init();
  await initializeDateFormatting('id_ID'); // inisialisasi locale Indonesia
  runApp(const KasirLancarApp());
}

// ── Warna hijau utama ─────────────────────────────────────────────────────────
const _kGreen = Color(0xFF16A34A); // green-600

class KasirLancarApp extends StatelessWidget {
  const KasirLancarApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => ConnectivityProvider()),
        ChangeNotifierProvider(create: (_) => PenjualanProvider()),
      ],
      child: MaterialApp(
        title: 'Kasir Lancar Mobile',
        theme: _buildTheme(),
        home: const AuthWrapper(),
        debugShowCheckedModeBanner: false,
      ),
    );
  }

  ThemeData _buildTheme() {
    return ThemeData(
      useMaterial3: true,
      colorScheme: ColorScheme.fromSeed(
        seedColor: _kGreen,
        brightness: Brightness.light,
      ),
      // AppBar hijau
      appBarTheme: const AppBarTheme(
        backgroundColor: _kGreen,
        foregroundColor: Colors.white,
        elevation: 0,
        centerTitle: true,
        titleTextStyle: TextStyle(
          fontSize: 18,
          fontWeight: FontWeight.w600,
          color: Colors.white,
        ),
      ),
      // ElevatedButton hijau
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: _kGreen,
          foregroundColor: Colors.white,
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          elevation: 2,
        ),
      ),
      // FAB hijau
      floatingActionButtonTheme: const FloatingActionButtonThemeData(
        backgroundColor: _kGreen,
        foregroundColor: Colors.white,
      ),
      // Progress indicator hijau
      progressIndicatorTheme: const ProgressIndicatorThemeData(color: _kGreen),
      // Input
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: Colors.grey.shade50,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide.none,
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide(color: Colors.grey.shade300),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: _kGreen, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: Colors.red, width: 2),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: Colors.red, width: 2),
        ),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 20,
          vertical: 16,
        ),
      ),
      // SnackBar
      snackBarTheme: SnackBarThemeData(
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        elevation: 4,
      ),
    );
  }
}

// ── Auth routing ──────────────────────────────────────────────────────────────
class AuthWrapper extends StatelessWidget {
  const AuthWrapper({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<AuthProvider>(
      builder: (context, auth, _) {
        // Loading splash
        if (auth.isLoading) {
          return const Scaffold(
            backgroundColor: Color(0xFFF0FDF4), // green-50
            body: Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  CircularProgressIndicator(color: _kGreen),
                  SizedBox(height: 20),
                  Text('Kasir Lancar Mobile'),
                  SizedBox(height: 6),
                  Text(
                    'Memuat...',
                    style: TextStyle(fontSize: 13, color: Colors.grey),
                  ),
                ],
              ),
            ),
          );
        }

        // Server belum dikonfigurasi
        if (!auth.isServerConfigured) return const ServerConfigScreen();

        // Belum login
        if (!auth.isLoggedIn) return const LoginScreen();

        // Sudah login tapi belum pilih lokasi → wajib pilih dulu
        if (auth.selectedLocation == null) {
          return const LocationSelectionScreen();
        }

        // Sudah login + sudah pilih lokasi → dashboard
        return const DashboardScreen();
      },
    );
  }
}
