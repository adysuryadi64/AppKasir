import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../screens/dashboard_screen.dart';
import '../screens/laporan_stok_screen.dart';
import '../screens/location_selection_screen.dart';
import '../screens/login_screen.dart';
import '../screens/opname_list_screen.dart';
import '../screens/printer_settings_screen.dart';
import '../screens/riwayat_penjualan_screen.dart';
import '../screens/role_mapping_screen.dart';
import '../screens/transfer_list_screen.dart';
import '../services/storage_service.dart';

class AppDrawer extends StatelessWidget {
  const AppDrawer({super.key});

  static const _green1 = Color(0xFF16A34A);
  static const _green2 = Color(0xFF15803D);

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return Drawer(
      elevation: 0,
      backgroundColor: Colors.transparent,
      child: Container(
        margin: EdgeInsets.only(
          top: MediaQuery.of(context).padding.top + 10,
          bottom: MediaQuery.of(context).padding.bottom + 10,
          left: 10,
          right: MediaQuery.of(context).size.width * 0.2,
        ),
        decoration: BoxDecoration(
          color: scheme.surface,
          borderRadius: BorderRadius.circular(20),
          boxShadow: [
            BoxShadow(
              color: scheme.shadow.withValues(alpha: 0.15),
              blurRadius: 30,
              offset: const Offset(4, 4),
              spreadRadius: 0,
            ),
          ],
        ),
        child: ClipRRect(
          borderRadius: BorderRadius.circular(20),
          child: Column(
            children: [
              // ── Header — logo | nama app + perusahaan ───────────
              Consumer<AuthProvider>(
                builder: (_, auth, _) => Container(
                  width: double.infinity,
                  padding: const EdgeInsets.fromLTRB(16, 20, 16, 20),
                  decoration: const BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.topLeft,
                      end: Alignment.bottomRight,
                      colors: [_green1, _green2],
                    ),
                  ),
                  child: Row(
                    children: [
                      // Logo
                      ClipRRect(
                        borderRadius: BorderRadius.circular(10),
                        child: Image.asset(
                          'assets/images/LogoMobile.png',
                          width: 40,
                          height: 40,
                          fit: BoxFit.cover,
                          errorBuilder: (_, _, _) => Container(
                            width: 40,
                            height: 40,
                            decoration: BoxDecoration(
                              color: Colors.white.withValues(alpha: 0.25),
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: const Icon(
                              Icons.point_of_sale,
                              color: Colors.white,
                              size: 22,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(width: 12),
                      // Nama app + perusahaan sejajar di kanan logo
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            const Text(
                              'Kasir Lancar Mobile',
                              style: TextStyle(
                                color: Colors.white,
                                fontSize: 15,
                                fontWeight: FontWeight.bold,
                                letterSpacing: 0.2,
                              ),
                            ),
                            const SizedBox(height: 2),
                            Text(
                              auth.namaPerusahaan,
                              style: TextStyle(
                                color: Colors.white.withValues(alpha: 0.75),
                                fontSize: 12,
                              ),
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ),

              // ── Menu items ───────────────────────────────────────────
              Expanded(
                child: ListView(
                  padding: const EdgeInsets.symmetric(
                    vertical: 12,
                    horizontal: 10,
                  ),
                  children: [
                    // ── Dashboard card ───────────────────────────────────
                    _DashboardCard(
                      onTap: () {
                        Navigator.of(context).pop();
                        Navigator.of(context).pushReplacement(
                          MaterialPageRoute(
                            builder: (_) => const DashboardScreen(),
                          ),
                        );
                      },
                    ),
                    const SizedBox(height: 4),

                    // ── TRANSAKSI ────────────────────────────────────────
                    _SectionLabel(label: 'TRANSAKSI'),
                    _DrawerItem(
                      icon: Icons.shopping_cart_checkout,
                      label: 'Penjualan',
                      onTap: () {
                        Navigator.of(context).pop();
                        // Drawer → riwayat penjualan (bisa back ke dashboard)
                        Navigator.of(context).push(
                          MaterialPageRoute(
                            builder: (_) => const RiwayatPenjualanScreen(),
                          ),
                        );
                      },
                    ),
                    _DrawerItem(
                      icon: Icons.inventory_2_outlined,
                      label: 'Stok Opname',
                      onTap: () {
                        Navigator.of(context).pop();
                        // Drawer → daftar opname (bisa back ke dashboard)
                        Navigator.of(context).push(
                          MaterialPageRoute(
                            builder: (_) => const OpnameListScreen(),
                          ),
                        );
                      },
                    ),
                    _DrawerItem(
                      icon: Icons.swap_horiz,
                      label: 'Transfer Stok',
                      onTap: () {
                        Navigator.of(context).pop();
                        Navigator.of(context).push(
                          MaterialPageRoute(
                            builder: (_) => const TransferListScreen(),
                          ),
                        );
                      },
                    ),

                    // ── LAPORAN ──────────────────────────────────────────
                    _SectionLabel(label: 'LAPORAN'),
                    _DrawerItem(
                      icon: Icons.bar_chart_outlined,
                      label: 'Laporan Stok',
                      onTap: () {
                        Navigator.of(context).pop();
                        Navigator.of(context).push(
                          MaterialPageRoute(
                            builder: (_) => const LaporanStokScreen(),
                          ),
                        );
                      },
                    ),

                    // ── PENGATURAN ───────────────────────────────────────
                    _SectionLabel(label: 'PENGATURAN'),
                    _DrawerItem(
                      icon: Icons.print_outlined,
                      label: 'Printer',
                      onTap: () {
                        Navigator.of(context).pop();
                        Navigator.of(context).push(
                          MaterialPageRoute(
                            builder: (_) => const PrinterSettingsScreen(),
                          ),
                        );
                      },
                    ),
                    _DrawerItem(
                      icon: Icons.tune_outlined,
                      label: 'Mapping Role',
                      onTap: () {
                        Navigator.of(context).pop();
                        Navigator.of(context).push(
                          MaterialPageRoute(
                            builder: (_) => const RoleMappingScreen(),
                          ),
                        );
                      },
                    ),
                    _DrawerItem(
                      icon: Icons.swap_horiz,
                      label: 'Ganti Lokasi',
                      onTap: () {
                        Navigator.of(context).pop();
                        Navigator.of(context).pushReplacement(
                          MaterialPageRoute(
                            builder: (_) => const LocationSelectionScreen(),
                          ),
                        );
                      },
                    ),
                    _DrawerItem(
                      icon: Icons.bookmark_outline_rounded,
                      label: 'Lokasi Default',
                      onTap: () {
                        Navigator.of(context).pop();
                        _showLocationDefaultDialog(context);
                      },
                    ),
                  ],
                ),
              ),

              // ── Footer: Logout ───────────────────────────────────────
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 10,
                  vertical: 8,
                ),
                child: _DrawerItem(
                  icon: Icons.logout,
                  label: 'Logout',
                  color: Colors.red.shade600,
                  onTap: () {
                    Navigator.of(context).pop();
                    _showLogoutDialog(context);
                  },
                ),
              ),
              const SizedBox(height: 8),
            ],
          ),
        ),
      ),
    );
  }

  void _showLocationDefaultDialog(BuildContext context) {
    String current = StorageService.getLocationDefault();

    showDialog(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDlg) => AlertDialog(
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
          ),
          title: const Row(
            children: [
              Icon(Icons.bookmark_outline_rounded, color: _green1),
              SizedBox(width: 10),
              Text('Lokasi Default Login'),
            ],
          ),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                'Pilih lokasi yang otomatis dipakai setiap kali login.',
                style: TextStyle(fontSize: 13, color: Colors.grey.shade600),
              ),
              const SizedBox(height: 16),
              _locationOption(
                ctx: ctx,
                setDlg: setDlg,
                current: current,
                value: 'selalu_tanya',
                icon: Icons.help_outline_rounded,
                label: 'Selalu Tanya',
                sub: 'Tampilkan halaman pilih lokasi setiap login',
                onSelect: (v) => current = v,
              ),
              const SizedBox(height: 8),
              _locationOption(
                ctx: ctx,
                setDlg: setDlg,
                current: current,
                value: 'TOKO',
                icon: Icons.storefront_rounded,
                label: 'TOKO',
                sub: 'Langsung masuk sebagai lokasi Toko',
                onSelect: (v) => current = v,
              ),
              const SizedBox(height: 8),
              _locationOption(
                ctx: ctx,
                setDlg: setDlg,
                current: current,
                value: 'GUDANG',
                icon: Icons.warehouse_rounded,
                label: 'GUDANG',
                sub: 'Langsung masuk sebagai lokasi Gudang',
                onSelect: (v) => current = v,
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(ctx).pop(),
              child: const Text('Batal'),
            ),
            ElevatedButton(
              onPressed: () async {
                await StorageService.setLocationDefault(current);
                if (!ctx.mounted) return;
                Navigator.of(ctx).pop();
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text(
                      'Preferensi disimpan: ${current == 'selalu_tanya' ? 'Selalu Tanya' : current}',
                    ),
                    backgroundColor: _green1,
                    behavior: SnackBarBehavior.floating,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                    duration: const Duration(seconds: 2),
                  ),
                );
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: _green1,
                foregroundColor: Colors.white,
              ),
              child: const Text('Simpan'),
            ),
          ],
        ),
      ),
    );
  }

  Widget _locationOption({
    required BuildContext ctx,
    required StateSetter setDlg,
    required String current,
    required String value,
    required IconData icon,
    required String label,
    required String sub,
    required void Function(String) onSelect,
  }) {
    final isActive = current == value;
    return GestureDetector(
      onTap: () => setDlg(() => onSelect(value)),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 180),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
        decoration: BoxDecoration(
          color: isActive
              ? _green1.withValues(alpha: 0.07)
              : Colors.grey.shade50,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: isActive ? _green1 : Colors.grey.shade200,
            width: isActive ? 1.5 : 1,
          ),
        ),
        child: Row(
          children: [
            Icon(
              icon,
              size: 20,
              color: isActive ? _green1 : Colors.grey.shade500,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    label,
                    style: TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w600,
                      color: isActive ? _green1 : const Color(0xFF1E293B),
                    ),
                  ),
                  Text(
                    sub,
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                  ),
                ],
              ),
            ),
            if (isActive)
              Icon(Icons.check_circle_rounded, size: 18, color: _green1),
          ],
        ),
      ),
    );
  }

  void _showLogoutDialog(BuildContext context) {
    // Simpan navigator sebelum dialog dibuka — context drawer masih valid di sini
    final navigator = Navigator.of(context);

    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: Row(
          children: [
            Icon(Icons.logout, color: Colors.red.shade600),
            const SizedBox(width: 10),
            const Text('Logout'),
          ],
        ),
        content: const Text('Yakin ingin keluar dari aplikasi?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Batal'),
          ),
          ElevatedButton(
            onPressed: () async {
              Navigator.of(ctx).pop(); // tutup dialog
              await Provider.of<AuthProvider>(ctx, listen: false).logout();
              // Bersihkan seluruh stack dan kembali ke LoginScreen
              navigator.pushAndRemoveUntil(
                MaterialPageRoute(builder: (_) => const LoginScreen()),
                (route) => false,
              );
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red.shade600,
              foregroundColor: Colors.white,
            ),
            child: const Text('Logout'),
          ),
        ],
      ),
    );
  }
}

// ── Dashboard card — tombol utama di atas menu ────────────────────────────────
class _DashboardCard extends StatelessWidget {
  final VoidCallback onTap;
  const _DashboardCard({required this.onTap});

  static const _green1 = Color(0xFF16A34A);
  static const _green2 = Color(0xFF15803D);

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
        decoration: BoxDecoration(
          gradient: const LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [_green1, _green2],
          ),
          borderRadius: BorderRadius.circular(14),
          boxShadow: [
            BoxShadow(
              color: _green1.withValues(alpha: 0.25),
              blurRadius: 10,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: Colors.white.withValues(alpha: 0.2),
                borderRadius: BorderRadius.circular(10),
              ),
              child: const Icon(
                Icons.dashboard_rounded,
                color: Colors.white,
                size: 20,
              ),
            ),
            const SizedBox(width: 12),
            const Text(
              'Dashboard',
              style: TextStyle(
                color: Colors.white,
                fontSize: 15,
                fontWeight: FontWeight.bold,
              ),
            ),
            const Spacer(),
            Icon(
              Icons.arrow_forward_ios_rounded,
              color: Colors.white.withValues(alpha: 0.6),
              size: 14,
            ),
          ],
        ),
      ),
    );
  }
}

class _SectionLabel extends StatelessWidget {
  final String label;
  const _SectionLabel({required this.label});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Padding(
      padding: const EdgeInsets.fromLTRB(10, 14, 10, 4),
      child: Text(
        label,
        style: TextStyle(
          fontSize: 10,
          fontWeight: FontWeight.w700,
          color: scheme.onSurfaceVariant.withValues(alpha: 0.45),
          letterSpacing: 1.2,
        ),
      ),
    );
  }
}

class _DrawerItem extends StatefulWidget {
  final IconData icon;
  final String label;
  final VoidCallback onTap;
  final Color? color;

  const _DrawerItem({
    required this.icon,
    required this.label,
    required this.onTap,
    this.color,
  });

  @override
  State<_DrawerItem> createState() => _DrawerItemState();
}

class _DrawerItemState extends State<_DrawerItem> {
  bool _isHovered = false;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final c = widget.color ?? scheme.onSurfaceVariant;
    final bgHover = (widget.color == null)
        ? scheme.primaryContainer.withValues(alpha: 0.12)
        : Colors.red.shade50;

    return MouseRegion(
      onEnter: (_) => setState(() => _isHovered = true),
      onExit: (_) => setState(() => _isHovered = false),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 150),
        curve: Curves.easeOutCubic,
        margin: const EdgeInsets.symmetric(vertical: 2),
        decoration: BoxDecoration(
          color: _isHovered ? bgHover : Colors.transparent,
          borderRadius: BorderRadius.circular(14),
        ),
        child: InkWell(
          onTap: widget.onTap,
          borderRadius: BorderRadius.circular(14),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 12),
            child: Row(
              children: [
                AnimatedScale(
                  scale: _isHovered ? 1.1 : 1.0,
                  duration: const Duration(milliseconds: 150),
                  curve: Curves.easeOutCubic,
                  child: Icon(
                    widget.icon,
                    color: _isHovered ? (widget.color ?? scheme.primary) : c,
                    size: 24,
                  ),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Text(
                    widget.label,
                    style: TextStyle(
                      color: _isHovered ? (widget.color ?? scheme.primary) : c,
                      fontSize: 15,
                      fontWeight: _isHovered
                          ? FontWeight.w600
                          : FontWeight.w500,
                      letterSpacing: 0.1,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
