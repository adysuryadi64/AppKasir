import 'dart:async';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';
import '../providers/auth_provider.dart';
import '../providers/connectivity_provider.dart';
import '../services/api_service.dart';
import '../services/storage_service.dart';
import '../widgets/app_drawer.dart';
import '../widgets/ai_analytics_card.dart';
import 'ai_analytics/ai_modals.dart';
import 'penjualan/penjualan_flow.dart';
import 'stok_opname_screen.dart';
import 'location_selection_screen.dart';
import 'server_config_screen.dart';

class DashboardScreen extends StatefulWidget {
  const DashboardScreen({super.key});

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  // ── Summary ───────────────────────────────────────────────────
  Map<String, dynamic>? _summary;
  bool _isLoadingSummary = false;

  // ── AI Analytics ──────────────────────────────────────────────
  // Key: type string, Value: response map dari server
  final Map<String, Map<String, dynamic>?> _aiData = {
    'produk_terlaris': null,
    'barang_lambat': null,
    'reorder_alert': null,
    'jam_puncak': null,
    'margin_profit': null,
    'pelanggan_aktif': null,
  };
  final Map<String, bool> _aiLoading = {
    'produk_terlaris': true,
    'barang_lambat': true,
    'reorder_alert': true,
    'jam_puncak': true,
    'margin_profit': true,
    'pelanggan_aktif': true,
  };
  Timer? _aiRefreshTimer;

  final _fmtRp = NumberFormat('#,##0', 'id_ID');

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadSummary();
      _loadAllAI();
      // Auto-refresh AI setiap 5 menit
      _aiRefreshTimer = Timer.periodic(
        const Duration(minutes: 5),
        (_) => _loadAllAI(),
      );
    });
  }

  @override
  void dispose() {
    _aiRefreshTimer?.cancel();
    super.dispose();
  }

  // ── Load summary ──────────────────────────────────────────────
  Future<void> _loadSummary() async {
    if (!mounted) return;
    setState(() => _isLoadingSummary = true);
    try {
      final lokasi =
          Provider.of<AuthProvider>(context, listen: false).selectedLocation ??
          '';
      final res = await ApiService.getDashboardSummary(lokasi: lokasi);
      if (!mounted) return;
      if (res['status'] == 'success') {
        setState(() => _summary = res['data'] as Map<String, dynamic>?);
      }
    } catch (e) {
      debugPrint('[Dashboard] ❌ _loadSummary error: $e');
    } finally {
      if (mounted) setState(() => _isLoadingSummary = false);
    }
  }

  // ── Load semua AI Analytics paralel ──────────────────────────
  Future<void> _loadAllAI() async {
    if (!mounted) return;
    final lokasi =
        Provider.of<AuthProvider>(context, listen: false).selectedLocation ??
        '';

    // Set semua loading
    setState(() {
      for (final k in _aiLoading.keys) {
        _aiLoading[k] = true;
      }
    });

    // Fetch paralel
    final types = _aiData.keys.toList();
    final futures = types.map(
      (t) => ApiService.getAIAnalytics(
        type: t,
        lokasi: lokasi,
      ).catchError((Object _) => <String, dynamic>{'status': 'error'}),
    );

    final results = await Future.wait(futures);
    if (!mounted) return;

    setState(() {
      for (var i = 0; i < types.length; i++) {
        final t = types[i];
        final res = results[i];
        if (res['status'] == 'success') {
          _aiData[t] = res;
        }
        _aiLoading[t] = false;
      }
    });
  }

  // ── Helper: ambil summary dari AI data ───────────────────────
  String? _aiMetric(String type) =>
      (_aiData[type]?['summary'] as Map<String, dynamic>?)?['key_metric']
          ?.toString();

  String? _aiInsight(String type) =>
      (_aiData[type]?['summary'] as Map<String, dynamic>?)?['insight']
          ?.toString();

  // ── Pull-to-refresh ───────────────────────────────────────────
  Future<void> _onRefresh() async {
    await Future.wait([_loadSummary(), _loadAllAI()]);
  }

  @override
  Widget build(BuildContext context) {
    final auth = Provider.of<AuthProvider>(context);
    final connectivity = Provider.of<ConnectivityProvider>(context);

    return Scaffold(
      drawer: const AppDrawer(),
      body: Container(
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [Color(0xFFF8FAFC), Color(0xFFF1F5F9)],
          ),
        ),
        child: SafeArea(
          child: RefreshIndicator(
            onRefresh: _onRefresh,
            child: CustomScrollView(
              physics: const AlwaysScrollableScrollPhysics(),
              slivers: [
                // ── App Bar ──────────────────────────────────────
                SliverToBoxAdapter(child: _buildHeader(auth, connectivity)),

                // ── Section: Rangkuman ────────────────────────────
                SliverToBoxAdapter(
                  child: _buildSectionLabel('📊 Rangkuman Hari Ini'),
                ),
                SliverToBoxAdapter(
                  child: Padding(
                    padding: const EdgeInsets.fromLTRB(16, 0, 16, 0),
                    child: _buildSummaryCards(),
                  ),
                ),

                // ── Section: Aksi Cepat ───────────────────────────
                SliverToBoxAdapter(child: _buildSectionLabel('⚡ Aksi Cepat')),
                SliverToBoxAdapter(
                  child: Padding(
                    padding: const EdgeInsets.fromLTRB(16, 0, 16, 0),
                    child: _buildMainMenu(auth),
                  ),
                ),

                // ── Section: AI Analytics ─────────────────────────
                SliverToBoxAdapter(
                  child: _buildSectionLabel('🤖 AI Analytics'),
                ),
                SliverToBoxAdapter(
                  child: Padding(
                    padding: const EdgeInsets.fromLTRB(16, 0, 16, 0),
                    child: _buildAISection(),
                  ),
                ),

                // ── Footer ────────────────────────────────────────
                SliverToBoxAdapter(
                  child: Padding(
                    padding: const EdgeInsets.fromLTRB(16, 16, 16, 24),
                    child: _buildStatusBar(connectivity, auth),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildHeader(AuthProvider auth, ConnectivityProvider conn) {
    final isToko = auth.selectedLocation == 'TOKO';
    final isGudang = auth.selectedLocation == 'GUDANG';
    final lokasiColor = isToko
        ? const Color(0xFF10B981)
        : isGudang
        ? const Color(0xFFF59E0B)
        : const Color(0xFF64748B);
    final lokasiIcon = isToko
        ? Icons.storefront_rounded
        : isGudang
        ? Icons.warehouse_rounded
        : Icons.location_off_outlined;

    return Container(
      margin: const EdgeInsets.fromLTRB(12, 12, 12, 0),
      padding: const EdgeInsets.fromLTRB(16, 14, 12, 14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.07),
            blurRadius: 16,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Row(
        children: [
          // ── Hamburger ──────────────────────────────────────
          Builder(
            builder: (ctx) => GestureDetector(
              onTap: () => Scaffold.of(ctx).openDrawer(),
              child: Container(
                padding: const EdgeInsets.all(7),
                decoration: BoxDecoration(
                  color: const Color(0xFFF1F5F9),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Icon(
                  Icons.menu_rounded,
                  color: Colors.grey.shade700,
                  size: 20,
                ),
              ),
            ),
          ),
          const SizedBox(width: 12),

          // ── Nama user + perusahaan ──────────────────────────
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  auth.userFullName.isNotEmpty
                      ? auth.userFullName
                      : auth.userName,
                  style: const TextStyle(
                    fontSize: 15,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF1E293B),
                  ),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
                Text(
                  auth.namaPerusahaan,
                  style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
              ],
            ),
          ),
          const SizedBox(width: 8),

          // ── Badge lokasi ────────────────────────────────────
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
            decoration: BoxDecoration(
              color: lokasiColor.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(10),
              border: Border.all(color: lokasiColor.withValues(alpha: 0.3)),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(lokasiIcon, size: 14, color: lokasiColor),
                const SizedBox(width: 5),
                Text(
                  auth.selectedLocation ?? '—',
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w700,
                    color: lokasiColor,
                    letterSpacing: 0.3,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(width: 8),

          // ── WiFi status ─────────────────────────────────────
          GestureDetector(
            onTap: () {
              if (!conn.serverReachable) {
                Navigator.of(context).push(
                  MaterialPageRoute(builder: (_) => const ServerConfigScreen()),
                );
              } else {
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: const Row(
                      children: [
                        Icon(Icons.wifi, color: Colors.white, size: 16),
                        SizedBox(width: 8),
                        Text('Server terhubung'),
                      ],
                    ),
                    backgroundColor: const Color(0xFF16A34A),
                    duration: const Duration(seconds: 2),
                    behavior: SnackBarBehavior.floating,
                  ),
                );
              }
            },
            child: Container(
              padding: const EdgeInsets.all(7),
              decoration: BoxDecoration(
                color: const Color(0xFFF1F5F9),
                borderRadius: BorderRadius.circular(10),
              ),
              child: Icon(conn.wifiIcon, color: conn.wifiColor, size: 18),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSectionLabel(String text) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 20, 16, 8),
      child: Text(
        text,
        style: const TextStyle(
          fontSize: 14,
          fontWeight: FontWeight.w700,
          color: Color(0xFF374151),
        ),
      ),
    );
  }

  // ── Summary cards ─────────────────────────────────────────────
  Widget _buildSummaryCards() {
    final totalPenjualan =
        (_summary?['total_penjualan'] as num?)?.toDouble() ?? 0;
    final jumlahTrx = (_summary?['jumlah_transaksi'] as num?)?.toInt() ?? 0;
    final jumlahOpname = (_summary?['jumlah_opname'] as num?)?.toInt() ?? 0;

    return Column(
      children: [
        // ── Baris 1: Penjualan — 1 kolom penuh, angka Rp besar ──────
        _cardPenjualan(isLoading: _isLoadingSummary, total: totalPenjualan),
        const SizedBox(height: 10),
        // ── Baris 2: Transaksi + Opname — 2 kolom berjajar ──────────
        Row(
          children: [
            Expanded(
              child: _cardAngka(
                icon: Icons.receipt_long_outlined,
                label: 'Transaksi',
                value: _isLoadingSummary ? '—' : '$jumlahTrx',
                color: const Color(0xFF2563EB),
                isLoading: _isLoadingSummary,
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: _cardAngka(
                icon: Icons.inventory_2_outlined,
                label: 'Stok Opname',
                value: _isLoadingSummary ? '—' : '$jumlahOpname',
                color: const Color(0xFFF59E0B),
                isLoading: _isLoadingSummary,
              ),
            ),
          ],
        ),
      ],
    );
  }

  /// Card penjualan — 1 kolom penuh, angka Rp besar dengan FittedBox
  Widget _cardPenjualan({required bool isLoading, required double total}) {
    const c1 = Color(0xFF16A34A);
    const c2 = Color(0xFF15803D);
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 18),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [c1, c2],
        ),
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: c1.withValues(alpha: 0.3),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: Colors.white.withValues(alpha: 0.2),
              shape: BoxShape.circle,
            ),
            child: const Icon(
              Icons.point_of_sale,
              color: Colors.white,
              size: 24,
            ),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Penjualan Hari Ini',
                  style: TextStyle(
                    fontSize: 12,
                    color: Colors.white.withValues(alpha: 0.85),
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(height: 4),
                isLoading
                    ? Container(
                        height: 22,
                        width: 160,
                        decoration: BoxDecoration(
                          color: Colors.white.withValues(alpha: 0.3),
                          borderRadius: BorderRadius.circular(4),
                        ),
                      )
                    : FittedBox(
                        fit: BoxFit.scaleDown,
                        alignment: Alignment.centerLeft,
                        child: Text(
                          'Rp ${_fmtRp.format(total)}',
                          style: const TextStyle(
                            fontSize: 28,
                            fontWeight: FontWeight.bold,
                            color: Colors.white,
                          ),
                        ),
                      ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  /// Card angka kecil — untuk transaksi & opname (2 kolom berjajar)
  Widget _cardAngka({
    required IconData icon,
    required String label,
    required String value,
    required Color color,
    bool isLoading = false,
  }) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: color.withValues(alpha: 0.2)),
        boxShadow: [
          BoxShadow(
            color: color.withValues(alpha: 0.08),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.1),
              shape: BoxShape.circle,
            ),
            child: Icon(icon, color: color, size: 18),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: TextStyle(
                    fontSize: 11,
                    color: Colors.grey.shade500,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(height: 2),
                isLoading
                    ? Container(
                        height: 20,
                        width: 40,
                        decoration: BoxDecoration(
                          color: Colors.grey.shade200,
                          borderRadius: BorderRadius.circular(4),
                        ),
                      )
                    : Text(
                        value,
                        style: TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.bold,
                          color: color,
                        ),
                      ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  // ── Main menu ─────────────────────────────────────────────────
  Widget _buildMainMenu(AuthProvider auth) {
    return Column(
      children: [
        _menuButton(
          title: 'Penjualan',
          subtitle: 'Proses transaksi penjualan',
          icon: Icons.shopping_cart_checkout,
          gradient: const LinearGradient(
            colors: [Color(0xFF10B981), Color(0xFF059669)],
          ),
          onTap: () {
            if (auth.selectedLocation == null) {
              _showLocationRequired();
              return;
            }
            Navigator.of(
              context,
            ).push(MaterialPageRoute(builder: (_) => const PenjualanFlow()));
          },
        ),
        const SizedBox(height: 12),
        _menuButton(
          title: 'Stok Opname',
          subtitle: 'Hitung dan sesuaikan stok',
          icon: Icons.inventory_2,
          gradient: const LinearGradient(
            colors: [Color(0xFFF59E0B), Color(0xFFD97706)],
          ),
          onTap: () {
            if (auth.selectedLocation == null) {
              _showLocationRequired();
              return;
            }
            Navigator.of(
              context,
            ).push(MaterialPageRoute(builder: (_) => const StokOpnameScreen()));
          },
        ),
      ],
    );
  }

  Widget _menuButton({
    required String title,
    required String subtitle,
    required IconData icon,
    required Gradient gradient,
    required VoidCallback onTap,
  }) {
    return Material(
      color: Colors.transparent,
      borderRadius: BorderRadius.circular(16),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
          decoration: BoxDecoration(
            gradient: gradient,
            borderRadius: BorderRadius.circular(16),
            boxShadow: [
              BoxShadow(
                color: (gradient as LinearGradient).colors[0].withValues(
                  alpha: 0.2,
                ),
                blurRadius: 12,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          child: Row(
            children: [
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  color: Colors.white.withValues(alpha: 0.2),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(icon, size: 24, color: Colors.white),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      title,
                      style: const TextStyle(
                        fontSize: 17,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                      ),
                    ),
                    Text(
                      subtitle,
                      style: TextStyle(
                        fontSize: 12,
                        color: Colors.white.withValues(alpha: 0.85),
                      ),
                    ),
                  ],
                ),
              ),
              Icon(
                Icons.arrow_forward_ios,
                color: Colors.white.withValues(alpha: 0.7),
                size: 16,
              ),
            ],
          ),
        ),
      ),
    );
  }

  // ── AI Analytics section ──────────────────────────────────────
  Widget _buildAISection() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 12,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Header section
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  gradient: const LinearGradient(
                    colors: [Color(0xFF8B5CF6), Color(0xFF7C3AED)],
                  ),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: const Icon(
                  Icons.psychology_outlined,
                  color: Colors.white,
                  size: 20,
                ),
              ),
              const SizedBox(width: 10),
              const Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'AI Analytics',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: Color(0xFF1E293B),
                      ),
                    ),
                    Text(
                      'Insight bisnis dari data transaksi',
                      style: TextStyle(fontSize: 11, color: Color(0xFF64748B)),
                    ),
                  ],
                ),
              ),
              // Refresh AI
              GestureDetector(
                onTap: _loadAllAI,
                child: Container(
                  padding: const EdgeInsets.all(6),
                  decoration: BoxDecoration(
                    color: const Color(0xFF8B5CF6).withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: const Icon(
                    Icons.refresh,
                    size: 16,
                    color: Color(0xFF8B5CF6),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          // Grid 2 kolom — pakai LayoutBuilder agar tidak overflow
          LayoutBuilder(
            builder: (ctx, constraints) {
              final cardW = (constraints.maxWidth - 10) / 2;
              return Wrap(
                spacing: 10,
                runSpacing: 10,
                children: [
                  SizedBox(
                    width: cardW,
                    child: _aiCard(
                      type: 'produk_terlaris',
                      title: 'Produk Terlaris',
                      icon: Icons.local_fire_department,
                      color: const Color(0xFFEF4444),
                      onTap: () {
                        final data = _aiData['produk_terlaris'];
                        showProdukTerlarisModal(
                          context,
                          List<dynamic>.from(data?['data'] ?? []),
                        );
                      },
                    ),
                  ),
                  SizedBox(
                    width: cardW,
                    child: _aiCard(
                      type: 'barang_lambat',
                      title: 'Barang Lambat',
                      icon: Icons.hourglass_bottom,
                      color: const Color(0xFF8B5CF6),
                      onTap: () {
                        final data = _aiData['barang_lambat'];
                        showBarangLambatModal(
                          context,
                          List<dynamic>.from(data?['data'] ?? []),
                        );
                      },
                    ),
                  ),
                  SizedBox(
                    width: cardW,
                    child: _aiCard(
                      type: 'reorder_alert',
                      title: 'Reorder Alert',
                      icon: Icons.warning_amber_rounded,
                      color: const Color(0xFFF59E0B),
                      onTap: () {
                        final data = _aiData['reorder_alert'];
                        showReorderAlertModal(
                          context,
                          List<dynamic>.from(data?['data'] ?? []),
                        );
                      },
                    ),
                  ),
                  SizedBox(
                    width: cardW,
                    child: _aiCard(
                      type: 'jam_puncak',
                      title: 'Jam Puncak',
                      icon: Icons.access_time,
                      color: const Color(0xFF2563EB),
                      onTap: () {
                        final data = _aiData['jam_puncak'];
                        final summary =
                            data?['summary'] as Map<String, dynamic>? ?? {};
                        showJamPuncakModal(
                          context,
                          List<dynamic>.from(data?['data'] ?? []),
                          summary['rekomendasi']?.toString() ?? '',
                        );
                      },
                    ),
                  ),
                  SizedBox(
                    width: cardW,
                    child: _aiCard(
                      type: 'margin_profit',
                      title: 'Margin Profit',
                      icon: Icons.trending_up,
                      color: const Color(0xFF10B981),
                      onTap: () {
                        final data = _aiData['margin_profit'];
                        showMarginProfitModal(
                          context,
                          (data?['data'] as Map<String, dynamic>?) ?? {},
                        );
                      },
                    ),
                  ),
                  SizedBox(
                    width: cardW,
                    child: _aiCard(
                      type: 'pelanggan_aktif',
                      title: 'Pelanggan Aktif',
                      icon: Icons.people_outline,
                      color: const Color(0xFF0EA5E9),
                      onTap: () {
                        final data = _aiData['pelanggan_aktif'];
                        showPelangganAktifModal(
                          context,
                          List<dynamic>.from(data?['data'] ?? []),
                          (data?['summary'] as Map<String, dynamic>?) ?? {},
                        );
                      },
                    ),
                  ),
                ],
              );
            },
          ),
        ],
      ),
    );
  }

  Widget _aiCard({
    required String type,
    required String title,
    required IconData icon,
    required Color color,
    required VoidCallback onTap,
  }) {
    return AIAnalyticsCard(
      title: title,
      icon: icon,
      color: color,
      keyMetric: _aiMetric(type),
      insight: _aiInsight(type),
      isLoading: _aiLoading[type] ?? false,
      onTap: onTap,
    );
  }

  // ── Status bar ────────────────────────────────────────────────
  Widget _buildStatusBar(ConnectivityProvider provider, AuthProvider auth) {
    final deviceName = StorageService.getDeviceName();
    final appVersion = StorageService.getAppVersion();
    final serverUrl = auth.serverUrl ?? '—';
    final database = auth.database ?? '—';

    String ipDisplay = serverUrl;
    try {
      final uri = Uri.parse(serverUrl);
      if (uri.host.isNotEmpty) ipDisplay = uri.host;
    } catch (_) {}

    final tahun = DateTime.now().year;
    final copyright = tahun > 2023 ? '2023–$tahun' : '2023';
    final s = TextStyle(fontSize: 10, color: Colors.grey.shade500);
    final ic = Colors.grey.shade400;

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.grey.shade200),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // Baris 1: IP · DB · Device
          Row(
            children: [
              Icon(Icons.dns_outlined, size: 11, color: ic),
              const SizedBox(width: 3),
              Flexible(
                child: Text(
                  ipDisplay,
                  style: s,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
              Text('  ·  ', style: s),
              Icon(Icons.storage_outlined, size: 11, color: ic),
              const SizedBox(width: 3),
              Flexible(
                child: Text(
                  database,
                  style: s,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
              Text('  ·  ', style: s),
              Icon(Icons.phone_android, size: 11, color: ic),
              const SizedBox(width: 3),
              Flexible(
                child: Text(
                  deviceName,
                  style: s,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
          const SizedBox(height: 4),
          // Baris 2: versi + copyright
          Row(
            children: [
              Icon(Icons.info_outline, size: 11, color: ic),
              const SizedBox(width: 3),
              Text('Kasir Lancar v$appVersion', style: s),
              Text('  ·  ', style: s),
              Text('© $copyright', style: s),
            ],
          ),
        ],
      ),
    );
  }

  // ── Dialogs ───────────────────────────────────────────────────
  void _showLocationRequired() {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('Lokasi Diperlukan'),
        content: const Text('Pilih lokasi (TOKO atau GUDANG) terlebih dahulu.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Batal'),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.of(ctx).pop();
              Navigator.of(context).pushReplacement(
                MaterialPageRoute(
                  builder: (_) => const LocationSelectionScreen(),
                ),
              );
            },
            child: const Text('Pilih Lokasi'),
          ),
        ],
      ),
    );
  }
}
