import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../services/storage_service.dart';
import 'dashboard_screen.dart';

class LocationSelectionScreen extends StatefulWidget {
  const LocationSelectionScreen({super.key});

  @override
  State<LocationSelectionScreen> createState() =>
      _LocationSelectionScreenState();
}

class _LocationSelectionScreenState extends State<LocationSelectionScreen>
    with SingleTickerProviderStateMixin {
  bool _isLoading = false;
  String _defaultPref = 'selalu_tanya'; // nilai saat ini dari prefs

  static const _green1 = Color(0xFF16A34A);
  static const _green2 = Color(0xFF15803D);

  late final AnimationController _animCtrl;
  late final Animation<double> _fadeLogo;
  late final Animation<Offset> _slideLogo;
  late final Animation<double> _fadeGreet;
  late final Animation<Offset> _slideGreet;
  late final Animation<double> _fadeCards;
  late final Animation<Offset> _slideCards;

  @override
  void initState() {
    super.initState();
    _defaultPref = StorageService.getLocationDefault();

    _animCtrl = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 900),
    );
    _fadeLogo = CurvedAnimation(
      parent: _animCtrl,
      curve: const Interval(0.0, 0.55, curve: Curves.easeOut),
    );
    _slideLogo = Tween<Offset>(begin: const Offset(0, -0.3), end: Offset.zero)
        .animate(
          CurvedAnimation(
            parent: _animCtrl,
            curve: const Interval(0.0, 0.55, curve: Curves.easeOutCubic),
          ),
        );
    _fadeGreet = CurvedAnimation(
      parent: _animCtrl,
      curve: const Interval(0.2, 0.65, curve: Curves.easeOut),
    );
    _slideGreet = Tween<Offset>(begin: const Offset(0, 0.2), end: Offset.zero)
        .animate(
          CurvedAnimation(
            parent: _animCtrl,
            curve: const Interval(0.2, 0.65, curve: Curves.easeOutCubic),
          ),
        );
    _fadeCards = CurvedAnimation(
      parent: _animCtrl,
      curve: const Interval(0.45, 1.0, curve: Curves.easeOut),
    );
    _slideCards = Tween<Offset>(begin: const Offset(0, 0.3), end: Offset.zero)
        .animate(
          CurvedAnimation(
            parent: _animCtrl,
            curve: const Interval(0.45, 1.0, curve: Curves.easeOutCubic),
          ),
        );
    _animCtrl.forward();
  }

  @override
  void dispose() {
    _animCtrl.dispose();
    super.dispose();
  }

  Future<void> _selectLocation(String location) async {
    if (_isLoading) return;
    setState(() => _isLoading = true);
    final auth = Provider.of<AuthProvider>(context, listen: false);
    await auth.selectLocation(location);
    if (!mounted) return;
    Navigator.of(context).pushReplacement(
      MaterialPageRoute(builder: (_) => const DashboardScreen()),
    );
  }

  Future<void> _saveDefault(String val) async {
    await StorageService.setLocationDefault(val);
    if (!mounted) return;
    setState(() => _defaultPref = val);
    String label;
    switch (val) {
      case 'TOKO':
        label = 'Default: TOKO';
        break;
      case 'GUDANG':
        label = 'Default: GUDANG';
        break;
      default:
        label = 'Selalu tanya saat login';
    }
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Preferensi disimpan: $label'),
        backgroundColor: _green1,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
        duration: const Duration(seconds: 2),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final auth = Provider.of<AuthProvider>(context);
    final screenH = MediaQuery.of(context).size.height;
    final nama = auth.userFullName.isNotEmpty
        ? auth.userFullName
        : auth.userName;

    return Scaffold(
      backgroundColor: _green2,
      body: Container(
        width: double.infinity,
        height: double.infinity,
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [_green1, _green2],
          ),
        ),
        child: SafeArea(
          bottom: false,
          child: SingleChildScrollView(
            physics: const ClampingScrollPhysics(),
            child: ConstrainedBox(
              constraints: BoxConstraints(minHeight: screenH),
              child: IntrinsicHeight(
                child: Column(
                  children: [
                    const SizedBox(height: 32),

                    // ── Logo ────────────────────────────────────
                    FadeTransition(
                      opacity: _fadeLogo,
                      child: SlideTransition(
                        position: _slideLogo,
                        child: _buildLogo(),
                      ),
                    ),
                    const SizedBox(height: 20),

                    // ── Sapaan ──────────────────────────────────
                    FadeTransition(
                      opacity: _fadeGreet,
                      child: SlideTransition(
                        position: _slideGreet,
                        child: _buildGreeting(nama, auth.namaPerusahaan),
                      ),
                    ),
                    const SizedBox(height: 28),

                    // ── Kartu lokasi ─────────────────────────────
                    FadeTransition(
                      opacity: _fadeCards,
                      child: SlideTransition(
                        position: _slideCards,
                        child: Padding(
                          padding: const EdgeInsets.symmetric(horizontal: 24),
                          child: Column(
                            children: [
                              _buildLocationCard(
                                icon: Icons.storefront_rounded,
                                title: 'TOKO',
                                color: _green1,
                                onTap: () => _selectLocation('TOKO'),
                              ),
                              const SizedBox(height: 14),
                              _buildLocationCard(
                                icon: Icons.warehouse_rounded,
                                title: 'GUDANG',
                                color: const Color(0xFF0D9488),
                                onTap: () => _selectLocation('GUDANG'),
                              ),
                              const SizedBox(height: 24),

                              // ── Preferensi default ───────────
                              _buildDefaultPref(),
                            ],
                          ),
                        ),
                      ),
                    ),

                    const Expanded(child: SizedBox()),
                    const SizedBox(height: 24),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  // ── Logo ──────────────────────────────────────────────────────
  Widget _buildLogo() {
    return Column(
      children: [
        ClipRRect(
          borderRadius: BorderRadius.circular(20),
          child: Image.asset(
            'assets/images/LogoMobile.png',
            width: 80,
            height: 80,
            fit: BoxFit.cover,
            errorBuilder: (_, _, _) => Container(
              width: 80,
              height: 80,
              decoration: BoxDecoration(
                color: Colors.white.withValues(alpha: 0.2),
                borderRadius: BorderRadius.circular(20),
              ),
              child: const Icon(
                Icons.point_of_sale,
                size: 40,
                color: Colors.white,
              ),
            ),
          ),
        ),
        const SizedBox(height: 14),
        const Text(
          'Kasir Lancar',
          style: TextStyle(
            fontSize: 32,
            fontWeight: FontWeight.w800,
            color: Colors.white,
            letterSpacing: -0.5,
            height: 1.1,
          ),
        ),
        const Text(
          'Mobile',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.w300,
            color: Colors.white,
            letterSpacing: 6,
          ),
        ),
      ],
    );
  }

  // ── Sapaan ────────────────────────────────────────────────────
  Widget _buildGreeting(String nama, String perusahaan) {
    return Column(
      children: [
        Text(
          'Halo, $nama 👋',
          style: const TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.w600,
            color: Colors.white,
          ),
        ),
        if (perusahaan.isNotEmpty) ...[
          const SizedBox(height: 4),
          Text(
            perusahaan,
            style: TextStyle(
              fontSize: 13,
              color: Colors.white.withValues(alpha: 0.7),
            ),
            textAlign: TextAlign.center,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
        ],
        const SizedBox(height: 16),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
          decoration: BoxDecoration(
            color: Colors.white.withValues(alpha: 0.15),
            borderRadius: BorderRadius.circular(20),
            border: Border.all(color: Colors.white.withValues(alpha: 0.25)),
          ),
          child: Text(
            'Pilih Lokasi Kerja',
            style: TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w600,
              color: Colors.white.withValues(alpha: 0.95),
              letterSpacing: 1,
            ),
          ),
        ),
      ],
    );
  }

  // ── Kartu lokasi ──────────────────────────────────────────────
  Widget _buildLocationCard({
    required IconData icon,
    required String title,
    required Color color,
    required VoidCallback onTap,
  }) {
    return Material(
      color: Colors.transparent,
      borderRadius: BorderRadius.circular(18),
      child: InkWell(
        onTap: _isLoading ? null : onTap,
        borderRadius: BorderRadius.circular(18),
        splashColor: color.withValues(alpha: 0.15),
        highlightColor: color.withValues(alpha: 0.08),
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 20),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(18),
            boxShadow: [
              BoxShadow(
                color: color.withValues(alpha: 0.18),
                blurRadius: 20,
                offset: const Offset(0, 8),
              ),
            ],
          ),
          child: _isLoading
              ? SizedBox(
                  height: 56,
                  child: Center(
                    child: CircularProgressIndicator(
                      strokeWidth: 2.5,
                      color: color,
                    ),
                  ),
                )
              : Row(
                  children: [
                    Container(
                      width: 56,
                      height: 56,
                      decoration: BoxDecoration(
                        color: color.withValues(alpha: 0.1),
                        borderRadius: BorderRadius.circular(14),
                      ),
                      child: Icon(icon, size: 28, color: color),
                    ),
                    const SizedBox(width: 18),
                    Expanded(
                      child: Text(
                        title,
                        style: TextStyle(
                          fontSize: 22,
                          fontWeight: FontWeight.w800,
                          color: color,
                          letterSpacing: 1,
                        ),
                      ),
                    ),
                    Container(
                      width: 34,
                      height: 34,
                      decoration: BoxDecoration(
                        color: color.withValues(alpha: 0.1),
                        shape: BoxShape.circle,
                      ),
                      child: Icon(
                        Icons.arrow_forward_ios_rounded,
                        size: 14,
                        color: color,
                      ),
                    ),
                  ],
                ),
        ),
      ),
    );
  }

  // ── Preferensi default ────────────────────────────────────────
  Widget _buildDefaultPref() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: Colors.white.withValues(alpha: 0.2)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(
                Icons.bookmark_outline_rounded,
                size: 15,
                color: Colors.white.withValues(alpha: 0.8),
              ),
              const SizedBox(width: 6),
              Text(
                'Lokasi Default saat Login',
                style: TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w600,
                  color: Colors.white.withValues(alpha: 0.9),
                  letterSpacing: 0.3,
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          Row(
            children: [
              _prefChip(
                label: 'Selalu Tanya',
                value: 'selalu_tanya',
                icon: Icons.help_outline_rounded,
              ),
              const SizedBox(width: 8),
              _prefChip(
                label: 'TOKO',
                value: 'TOKO',
                icon: Icons.storefront_rounded,
              ),
              const SizedBox(width: 8),
              _prefChip(
                label: 'GUDANG',
                value: 'GUDANG',
                icon: Icons.warehouse_rounded,
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _prefChip({
    required String label,
    required String value,
    required IconData icon,
  }) {
    final isActive = _defaultPref == value;
    return Expanded(
      child: GestureDetector(
        onTap: () => _saveDefault(value),
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 200),
          padding: const EdgeInsets.symmetric(vertical: 8),
          decoration: BoxDecoration(
            color: isActive
                ? Colors.white
                : Colors.white.withValues(alpha: 0.1),
            borderRadius: BorderRadius.circular(10),
            border: Border.all(
              color: isActive
                  ? Colors.white
                  : Colors.white.withValues(alpha: 0.25),
              width: isActive ? 2 : 1,
            ),
          ),
          child: Column(
            children: [
              Icon(
                icon,
                size: 16,
                color: isActive ? _green1 : Colors.white.withValues(alpha: 0.7),
              ),
              const SizedBox(height: 4),
              Text(
                label,
                style: TextStyle(
                  fontSize: 10,
                  fontWeight: isActive ? FontWeight.w700 : FontWeight.w500,
                  color: isActive
                      ? _green1
                      : Colors.white.withValues(alpha: 0.8),
                ),
                textAlign: TextAlign.center,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
