import 'package:flutter/material.dart';
import 'package:package_info_plus/package_info_plus.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import 'server_config_screen.dart';
import 'location_selection_screen.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});
  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen>
    with SingleTickerProviderStateMixin {
  final _passCtrl = TextEditingController();
  final _passFocus = FocusNode();
  bool _obscure = true;
  bool _isLogging = false;
  bool _isLoadingUsers = false;
  String? _selectedUser;
  List<Map<String, dynamic>> _users = [];
  String _versi = '';

  static const _green1 = Color(0xFF16A34A);
  static const _green2 = Color(0xFF15803D);

  // ── Animasi ───────────────────────────────────────────────────
  late final AnimationController _animCtrl;
  late final Animation<double> _fadelogo;
  late final Animation<Offset> _slideLogo;
  late final Animation<double> _fadeCard;
  late final Animation<Offset> _slideCard;
  late final Animation<double> _fadeInfo;
  late final Animation<Offset> _slideInfo;

  @override
  void initState() {
    super.initState();

    _animCtrl = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 900),
    );

    // Logo — muncul pertama, dari atas
    _fadelogo = CurvedAnimation(
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

    // Card — muncul kedua, dari bawah
    _fadeCard = CurvedAnimation(
      parent: _animCtrl,
      curve: const Interval(0.25, 0.75, curve: Curves.easeOut),
    );
    _slideCard = Tween<Offset>(begin: const Offset(0, 0.25), end: Offset.zero)
        .animate(
          CurvedAnimation(
            parent: _animCtrl,
            curve: const Interval(0.25, 0.75, curve: Curves.easeOutCubic),
          ),
        );

    // Info bawah — muncul terakhir, dari bawah
    _fadeInfo = CurvedAnimation(
      parent: _animCtrl,
      curve: const Interval(0.5, 1.0, curve: Curves.easeOut),
    );
    _slideInfo = Tween<Offset>(begin: const Offset(0, 0.3), end: Offset.zero)
        .animate(
          CurvedAnimation(
            parent: _animCtrl,
            curve: const Interval(0.5, 1.0, curve: Curves.easeOutCubic),
          ),
        );

    _animCtrl.forward();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadUsers();
      _loadVersi();
    });
  }

  Future<void> _loadVersi() async {
    final info = await PackageInfo.fromPlatform();
    if (mounted) setState(() => _versi = 'v${info.version}');
  }

  @override
  void dispose() {
    _animCtrl.dispose();
    _passCtrl.dispose();
    _passFocus.dispose();
    super.dispose();
  }

  // ── Load users ────────────────────────────────────────────────
  Future<void> _loadUsers({bool force = false}) async {
    if (!mounted) return;
    if (_isLoadingUsers) return;
    if (!force && _users.isNotEmpty) {
      debugPrint('[Login] _loadUsers skip — sudah ada ${_users.length} user');
      return;
    }
    debugPrint('[Login] _loadUsers mulai (force=$force)');
    setState(() => _isLoadingUsers = true);
    try {
      final res = await ApiService.getUsers();
      if (!mounted) return;
      if (res['status'] == 'success') {
        setState(() {
          _users = List<Map<String, dynamic>>.from(res['data'] ?? []);
          if (_users.length == 1) _selectedUser = _users[0]['USER_NAME'];
        });
        debugPrint('[Login] _loadUsers berhasil — ${_users.length} user');
      } else {
        _showSnack(
          'Gagal memuat daftar user. Periksa koneksi server.',
          Colors.red,
        );
      }
    } catch (e) {
      if (!mounted) return;
      _showSnack(
        'Server tidak bisa dihubungi. Periksa konfigurasi.',
        Colors.red,
      );
    } finally {
      if (mounted) setState(() => _isLoadingUsers = false);
    }
  }

  // ── Login ─────────────────────────────────────────────────────
  Future<void> _login() async {
    if (_selectedUser == null) {
      _showSnack('Pilih username terlebih dahulu', Colors.orange);
      return;
    }
    if (_passCtrl.text.isEmpty) {
      _showSnack('Masukkan password', Colors.orange);
      return;
    }
    setState(() => _isLogging = true);
    final auth = Provider.of<AuthProvider>(context, listen: false);
    try {
      final ok = await auth.login(_selectedUser!, _passCtrl.text);
      if (!mounted) return;
      if (ok) {
        Navigator.of(context).pushReplacement(
          MaterialPageRoute(builder: (_) => const LocationSelectionScreen()),
        );
      } else {
        _showSnack(auth.errorMessage ?? 'Login gagal', Colors.red);
      }
    } finally {
      if (mounted) setState(() => _isLogging = false);
    }
  }

  void _showSnack(String msg, Color color) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(msg),
        backgroundColor: color,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      ),
    );
  }

  // ── Build ─────────────────────────────────────────────────────
  @override
  Widget build(BuildContext context) {
    final screenH = MediaQuery.of(context).size.height;

    return Scaffold(
      // Warna hijau — tidak ada area putih di bawah
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
          bottom: false, // biarkan gradient mengisi area bawah notch
          child: RefreshIndicator(
            onRefresh: () => _loadUsers(force: true),
            color: _green1,
            backgroundColor: Colors.white,
            child: SingleChildScrollView(
              physics: const AlwaysScrollableScrollPhysics(),
              child: ConstrainedBox(
                constraints: BoxConstraints(minHeight: screenH),
                child: IntrinsicHeight(
                  child: Column(
                    children: [
                      // ── Logo ────────────────────────────────────
                      const SizedBox(height: 32),
                      FadeTransition(
                        opacity: _fadelogo,
                        child: SlideTransition(
                          position: _slideLogo,
                          child: _buildLogo(),
                        ),
                      ),
                      const SizedBox(height: 24),

                      // ── Card login ───────────────────────────────
                      FadeTransition(
                        opacity: _fadeCard,
                        child: SlideTransition(
                          position: _slideCard,
                          child: Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 24),
                            child: _buildCard(),
                          ),
                        ),
                      ),

                      // ── Info bawah ───────────────────────────────
                      const SizedBox(height: 20),
                      FadeTransition(
                        opacity: _fadeInfo,
                        child: SlideTransition(
                          position: _slideInfo,
                          child: _buildInfoBawah(),
                        ),
                      ),

                      const Expanded(child: SizedBox()),
                      const SizedBox(height: 24),
                    ],
                  ),
                ),
              ),
            ), // SingleChildScrollView
          ), // RefreshIndicator
        ), // SafeArea
      ), // Container
    );
  }

  // ── Logo ──────────────────────────────────────────────────────
  Widget _buildLogo() {
    return Column(
      children: [
        // Logo dari assets
        ClipRRect(
          borderRadius: BorderRadius.circular(20),
          child: Image.asset(
            'assets/images/LogoMobile.png',
            width: 80,
            height: 80,
            fit: BoxFit.cover,
            errorBuilder: (ctx, err, stack) => Container(
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
        // Nama app — dua baris, font modern
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

  // ── Card login ────────────────────────────────────────────────
  Widget _buildCard() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 20),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.15),
            blurRadius: 24,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // Dropdown username
          _buildUserDropdown(),
          const SizedBox(height: 10),

          // Password
          TextField(
            controller: _passCtrl,
            focusNode: _passFocus,
            obscureText: _obscure,
            onSubmitted: (_) => _login(),
            decoration: InputDecoration(
              hintText: 'Password',
              prefixIcon: Icon(
                Icons.lock_outline,
                size: 18,
                color: Colors.grey.shade400,
              ),
              suffixIcon: IconButton(
                icon: Icon(
                  _obscure
                      ? Icons.visibility_outlined
                      : Icons.visibility_off_outlined,
                  size: 18,
                  color: Colors.grey.shade400,
                ),
                onPressed: () => setState(() => _obscure = !_obscure),
              ),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide(color: Colors.grey.shade300),
              ),
              enabledBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide(color: Colors.grey.shade300),
              ),
              focusedBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: const BorderSide(color: _green1, width: 2),
              ),
              contentPadding: const EdgeInsets.symmetric(
                horizontal: 14,
                vertical: 13,
              ),
            ),
          ),
          const SizedBox(height: 14),

          // Tombol Masuk
          SizedBox(
            height: 46,
            child: ElevatedButton(
              onPressed: (_isLogging || _isLoadingUsers) ? null : _login,
              style: ElevatedButton.styleFrom(
                backgroundColor: _green1,
                foregroundColor: Colors.white,
                disabledBackgroundColor: _green1.withValues(alpha: 0.45),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                elevation: 0,
              ),
              child: _isLogging
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(
                        strokeWidth: 2.5,
                        color: Colors.white,
                      ),
                    )
                  : const Text(
                      'Masuk',
                      style: TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.w600,
                        letterSpacing: 0.3,
                      ),
                    ),
            ),
          ),
        ],
      ),
    );
  }

  // ── Warna avatar berdasarkan level ───────────────────────────
  Color _levelColor(String lvl) {
    switch (lvl.toLowerCase()) {
      case 'owner':
      case 'master':
        return const Color(0xFF7C3AED);
      case 'admin':
        return const Color(0xFF0284C7);
      case 'kasir':
        return const Color(0xFF0D9488);
      default:
        return const Color(0xFF64748B);
    }
  }

  // ── Dropdown user ─────────────────────────────────────────────
  Widget _buildUserDropdown() {
    if (_isLoadingUsers) {
      return Container(
        height: 52,
        decoration: BoxDecoration(
          border: Border.all(color: Colors.grey.shade200),
          borderRadius: BorderRadius.circular(12),
          color: Colors.grey.shade50,
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            SizedBox(
              width: 15,
              height: 15,
              child: CircularProgressIndicator(
                strokeWidth: 2,
                color: Colors.grey.shade400,
              ),
            ),
            const SizedBox(width: 10),
            Text(
              'Memuat daftar user...',
              style: TextStyle(color: Colors.grey.shade400, fontSize: 13),
            ),
          ],
        ),
      );
    }

    if (_users.isEmpty) {
      return GestureDetector(
        onTap: () => _loadUsers(force: true),
        child: Container(
          height: 52,
          decoration: BoxDecoration(
            border: Border.all(color: Colors.red.shade200),
            borderRadius: BorderRadius.circular(12),
            color: Colors.red.shade50,
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(Icons.refresh_rounded, size: 16, color: Colors.red.shade400),
              const SizedBox(width: 8),
              Text(
                'Gagal memuat — tap untuk coba lagi',
                style: TextStyle(color: Colors.red.shade500, fontSize: 13),
              ),
            ],
          ),
        ),
      );
    }

    // Cari data user yang sedang dipilih untuk tampilan selected
    final selectedData = _selectedUser != null
        ? _users.firstWhere(
            (u) => u['USER_NAME'] == _selectedUser,
            orElse: () => {},
          )
        : null;
    final selectedLvl = selectedData?['LVL']?.toString() ?? '';

    return DropdownButtonHideUnderline(
      child: Container(
        decoration: BoxDecoration(
          border: Border.all(
            color: _selectedUser != null
                ? _levelColor(selectedLvl).withValues(alpha: 0.4)
                : Colors.grey.shade300,
            width: _selectedUser != null ? 1.5 : 1,
          ),
          borderRadius: BorderRadius.circular(12),
          color: Colors.white,
        ),
        child: DropdownButton<String>(
          value: _selectedUser,
          isExpanded: true,
          menuMaxHeight: 420,
          borderRadius: BorderRadius.circular(12),
          dropdownColor: Colors.white,
          icon: Padding(
            padding: const EdgeInsets.only(right: 12),
            child: Icon(
              Icons.unfold_more_rounded,
              color: Colors.grey.shade400,
              size: 20,
            ),
          ),
          // ── Tampilan saat belum dipilih ──────────────────────
          hint: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 14),
            child: Row(
              children: [
                Container(
                  width: 32,
                  height: 32,
                  decoration: BoxDecoration(
                    color: Colors.grey.shade100,
                    shape: BoxShape.circle,
                  ),
                  child: Icon(
                    Icons.person_outline_rounded,
                    size: 17,
                    color: Colors.grey.shade400,
                  ),
                ),
                const SizedBox(width: 10),
                Text(
                  'Pilih akun',
                  style: TextStyle(
                    fontSize: 14,
                    color: Colors.grey.shade400,
                    fontWeight: FontWeight.w400,
                  ),
                ),
              ],
            ),
          ),
          // ── Tampilan saat sudah dipilih ──────────────────────
          selectedItemBuilder: (_) => _users.map((u) {
            final nama = u['NAMA_USER']?.toString() ?? '';
            final user = u['USER_NAME']?.toString() ?? '';
            final lvl = u['LVL']?.toString() ?? '';
            final color = _levelColor(lvl);
            final initial = user.isNotEmpty ? user[0].toUpperCase() : '?';
            return Padding(
              padding: const EdgeInsets.symmetric(horizontal: 14),
              child: Row(
                children: [
                  Container(
                    width: 32,
                    height: 32,
                    decoration: BoxDecoration(
                      color: color,
                      shape: BoxShape.circle,
                    ),
                    child: Center(
                      child: Text(
                        initial,
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 14,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: RichText(
                      overflow: TextOverflow.ellipsis,
                      text: TextSpan(
                        children: [
                          TextSpan(
                            text: user,
                            style: const TextStyle(
                              fontSize: 14,
                              fontWeight: FontWeight.w700,
                              color: Color(0xFF1E293B),
                            ),
                          ),
                          TextSpan(
                            text: '  ·  $nama',
                            style: TextStyle(
                              fontSize: 13,
                              fontWeight: FontWeight.w400,
                              color: Colors.grey.shade500,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            );
          }).toList(),
          // ── Item di dalam list dropdown ──────────────────────
          items: _users.map((u) {
            final nama = u['NAMA_USER']?.toString() ?? '';
            final user = u['USER_NAME']?.toString() ?? '';
            final lvl = u['LVL']?.toString() ?? '';
            final color = _levelColor(lvl);
            final initial = user.isNotEmpty ? user[0].toUpperCase() : '?';
            final isSelected = _selectedUser == user;

            return DropdownMenuItem<String>(
              value: user,
              child: Container(
                padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 5),
                decoration: BoxDecoration(
                  color: isSelected
                      ? color.withValues(alpha: 0.06)
                      : Colors.transparent,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    Container(
                      width: 36,
                      height: 36,
                      decoration: BoxDecoration(
                        color: isSelected
                            ? color
                            : color.withValues(alpha: 0.12),
                        shape: BoxShape.circle,
                      ),
                      child: Center(
                        child: Text(
                          initial,
                          style: TextStyle(
                            color: isSelected ? Colors.white : color,
                            fontSize: 15,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: RichText(
                        overflow: TextOverflow.ellipsis,
                        text: TextSpan(
                          children: [
                            TextSpan(
                              text: user,
                              style: TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w700,
                                color: isSelected
                                    ? color
                                    : const Color(0xFF1E293B),
                              ),
                            ),
                            TextSpan(
                              text: '  ·  $nama',
                              style: TextStyle(
                                fontSize: 13,
                                fontWeight: FontWeight.w400,
                                color: Colors.grey.shade500,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    if (isSelected)
                      Icon(Icons.check_circle_rounded, size: 16, color: color),
                  ],
                ),
              ),
            );
          }).toList(),
          onChanged: (v) {
            setState(() => _selectedUser = v);
            Future.delayed(const Duration(milliseconds: 100), () {
              if (mounted) _passFocus.requestFocus();
            });
          },
        ),
      ),
    );
  }

  // ── Info elegan di bawah card ─────────────────────────────
  Widget _buildInfoBawah() {
    return Column(
      children: [
        // Tagline dengan styling elegan
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
          decoration: BoxDecoration(
            color: Colors.white.withValues(alpha: 0.15),
            borderRadius: BorderRadius.circular(20),
            border: Border.all(color: Colors.white.withValues(alpha: 0.25)),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(
                Icons.smartphone_outlined,
                size: 14,
                color: Colors.white.withValues(alpha: 0.9),
              ),
              const SizedBox(width: 6),
              Text(
                'Mobile Point of Sale',
                style: TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w600,
                  color: Colors.white.withValues(alpha: 0.95),
                  letterSpacing: 1,
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),

        // Deskripsi menarik
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 36),
          child: Column(
            children: [
              // Ikon dekoratif
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  _buildFeatureIcon(Icons.devices, 'Desktop'),
                  _buildFeatureDivider(),
                  _buildFeatureIcon(Icons.sync, 'Sync'),
                  _buildFeatureDivider(),
                  _buildFeatureIcon(Icons.inventory_2_outlined, 'Stok'),
                ],
              ),
              const SizedBox(height: 14),
              // Deskripsi utama
              RichText(
                textAlign: TextAlign.center,
                text: TextSpan(
                  style: TextStyle(
                    fontSize: 13,
                    color: Colors.white.withValues(alpha: 0.75),
                    height: 1.5,
                  ),
                  children: const [
                    TextSpan(text: 'Teman setia '),
                    TextSpan(
                      text: 'Kasir Lancar',
                      style: TextStyle(fontWeight: FontWeight.w700),
                    ),
                    TextSpan(
                      text:
                          ' desktop.\nKelola transaksi dari genggaman tangan.',
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),

        // Feature chips
        Wrap(
          spacing: 8,
          runSpacing: 6,
          alignment: WrapAlignment.center,
          children: [
            _buildFeatureChip(Icons.wifi, 'LAN / WiFi'),
            _buildFeatureChip(Icons.cloud_sync_outlined, 'Real-time'),
            _buildFeatureChip(Icons.security_outlined, 'Aman'),
          ],
        ),
        const SizedBox(height: 24),

        // Tombol server
        _buildServerBtn(),
        const SizedBox(height: 8),

        // Versi
        Text(
          _versi,
          style: TextStyle(
            fontSize: 11,
            color: Colors.white.withValues(alpha: 0.45),
          ),
        ),
      ],
    );
  }

  Widget _buildFeatureIcon(IconData icon, String tooltip) {
    return Tooltip(
      message: tooltip,
      child: Container(
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: Colors.white.withValues(alpha: 0.12),
          shape: BoxShape.circle,
        ),
        child: Icon(
          icon,
          size: 16,
          color: Colors.white.withValues(alpha: 0.85),
        ),
      ),
    );
  }

  Widget _buildFeatureDivider() {
    return Container(
      width: 20,
      height: 1,
      margin: const EdgeInsets.symmetric(horizontal: 4),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            Colors.white.withValues(alpha: 0.0),
            Colors.white.withValues(alpha: 0.4),
            Colors.white.withValues(alpha: 0.0),
          ],
        ),
      ),
    );
  }

  Widget _buildFeatureChip(IconData icon, String label) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: Colors.white.withValues(alpha: 0.18)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 12, color: Colors.white.withValues(alpha: 0.8)),
          const SizedBox(width: 5),
          Text(
            label,
            style: TextStyle(
              fontSize: 11,
              color: Colors.white.withValues(alpha: 0.8),
              fontWeight: FontWeight.w500,
            ),
          ),
        ],
      ),
    );
  }

  // ── Tombol konfigurasi server ─────────────────────────────────
  Widget _buildServerBtn() {
    return GestureDetector(
      onTap: () => Navigator.of(context)
          .push(MaterialPageRoute(builder: (_) => const ServerConfigScreen()))
          .then((_) => _loadUsers(force: true)),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
        decoration: BoxDecoration(
          color: Colors.white.withValues(alpha: 0.12),
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: Colors.white.withValues(alpha: 0.25),
            width: 1,
          ),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Icon dengan background
            Container(
              padding: const EdgeInsets.all(6),
              decoration: BoxDecoration(
                color: Colors.white.withValues(alpha: 0.15),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Icon(
                Icons.dns_outlined,
                size: 16,
                color: Colors.white.withValues(alpha: 0.9),
              ),
            ),
            const SizedBox(width: 10),
            // Label
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Konfigurasi Server',
                  style: TextStyle(
                    color: Colors.white.withValues(alpha: 0.95),
                    fontSize: 13,
                    fontWeight: FontWeight.w600,
                  ),
                ),
                Text(
                  'Atur alamat & koneksi',
                  style: TextStyle(
                    color: Colors.white.withValues(alpha: 0.6),
                    fontSize: 10,
                  ),
                ),
              ],
            ),
            const SizedBox(width: 8),
            Icon(
              Icons.chevron_right,
              size: 18,
              color: Colors.white.withValues(alpha: 0.6),
            ),
          ],
        ),
      ),
    );
  }
}
