import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'step1_header_screen.dart';
import 'step2_items_screen.dart';
import 'step3_payment_screen.dart';
import 'step4_preview_screen.dart';
import '../../providers/auth_provider.dart';
import '../../providers/penjualan_provider.dart';

/// PenjualanFlow — container PageView 4 langkah.
/// State disimpan di PenjualanProvider sehingga back/forward tidak kehilangan data.
class PenjualanFlow extends StatefulWidget {
  const PenjualanFlow({super.key});

  @override
  State<PenjualanFlow> createState() => _PenjualanFlowState();
}

class _PenjualanFlowState extends State<PenjualanFlow> {
  static const _green = Color(0xFF16A34A);

  final _pageCtrl = PageController();
  int _currentPage = 0;

  static const _stepLabels = ['Header', 'Barang', 'Bayar', 'Preview'];

  @override
  void dispose() {
    _pageCtrl.dispose();
    super.dispose();
  }

  void goToPage(int page) {
    if (page < 0 || page > 3) return;
    _pageCtrl.animateToPage(
      page,
      duration: const Duration(milliseconds: 300),
      curve: Curves.easeInOut,
    );
  }

  void nextPage() => goToPage(_currentPage + 1);
  void prevPage() => goToPage(_currentPage - 1);

  @override
  Widget build(BuildContext context) {
    final izinkanTglLampau = context
        .watch<AuthProvider>()
        .hakAkses
        .izinkanTanggalLampau;
    final prov = context.watch<PenjualanProvider>();
    final tglLabel =
        '${prov.tanggal.day.toString().padLeft(2, '0')}/'
        '${prov.tanggal.month.toString().padLeft(2, '0')}/'
        '${prov.tanggal.year}';

    return PopScope(
      canPop: _currentPage == 0,
      onPopInvokedWithResult: (didPop, _) {
        if (!didPop && _currentPage > 0) prevPage();
      },
      child: Scaffold(
        backgroundColor: const Color(0xFFF0FDF4),
        appBar: AppBar(
          title: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                'Penjualan',
                style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
              ),
              // Tanggal tampil sebagai subtitle kecil di AppBar
              Text(
                tglLabel,
                style: const TextStyle(
                  fontSize: 11,
                  fontWeight: FontWeight.normal,
                ),
              ),
            ],
          ),
          leading: IconButton(
            icon: const Icon(Icons.arrow_back),
            onPressed: () {
              if (_currentPage > 0) {
                prevPage();
              } else {
                Navigator.of(context).pop();
              }
            },
          ),
          actions: [
            // Icon kalender — hanya aktif jika izin tanggal lampau
            IconButton(
              icon: Icon(
                Icons.calendar_today_outlined,
                color: izinkanTglLampau
                    ? Colors.white
                    : Colors.white.withValues(alpha: 0.3),
              ),
              tooltip: izinkanTglLampau
                  ? 'Ubah tanggal transaksi'
                  : 'Tanggal lampau tidak diizinkan',
              onPressed: izinkanTglLampau
                  ? () => _pickDate(context, prov)
                  : null,
            ),
          ],
          bottom: PreferredSize(
            preferredSize: const Size.fromHeight(24),
            child: _buildStepIndicator(),
          ),
        ),
        body: PageView(
          controller: _pageCtrl,
          physics: const NeverScrollableScrollPhysics(),
          onPageChanged: (p) => setState(() => _currentPage = p),
          children: [
            Step1HeaderScreen(onNext: nextPage),
            Step2ItemsScreen(onNext: nextPage, onBack: prevPage),
            Step3PaymentScreen(onNext: nextPage, onBack: prevPage),
            Step4PreviewScreen(onBack: prevPage),
          ],
        ),
      ),
    );
  }

  Future<void> _pickDate(BuildContext ctx, PenjualanProvider prov) async {
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: ctx,
      initialDate: prov.tanggal,
      firstDate: DateTime(now.year - 1),
      lastDate: now,
      locale: const Locale('id', 'ID'),
      builder: (c, child) => Theme(
        data: Theme.of(c).copyWith(
          colorScheme: const ColorScheme.light(
            primary: _green,
            onPrimary: Colors.white,
          ),
        ),
        child: child!,
      ),
    );
    if (picked != null) {
      prov.setTanggal(
        DateTime(
          picked.year,
          picked.month,
          picked.day,
          prov.tanggal.hour,
          prov.tanggal.minute,
        ),
      );
    }
  }

  Widget _buildStepIndicator() {
    return Container(
      color: _green,
      padding: const EdgeInsets.fromLTRB(16, 0, 16, 8),
      child: Row(
        children: List.generate(4, (i) {
          final isActive = i == _currentPage;
          final isDone = i < _currentPage;
          final labelColor = isActive
              ? Colors.white
              : isDone
              ? Colors.white70
              : Colors.white38;

          return Expanded(
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                // Dot / check
                Container(
                  width: 20,
                  height: 20,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: isDone
                        ? Colors.white
                        : isActive
                        ? Colors.white24
                        : Colors.transparent,
                    border: Border.all(color: labelColor, width: 1.2),
                  ),
                  child: Center(
                    child: isDone
                        ? Icon(Icons.check, size: 11, color: _green)
                        : Text(
                            '${i + 1}',
                            style: TextStyle(
                              fontSize: 10,
                              fontWeight: FontWeight.bold,
                              color: labelColor,
                            ),
                          ),
                  ),
                ),
                const SizedBox(width: 3),
                Flexible(
                  child: Text(
                    _stepLabels[i],
                    style: TextStyle(
                      fontSize: 10,
                      color: labelColor,
                      fontWeight: isActive
                          ? FontWeight.bold
                          : FontWeight.normal,
                    ),
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
                if (i < 3)
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 3),
                    child: Container(
                      width: 8,
                      height: 1,
                      color: Colors.white24,
                    ),
                  ),
              ],
            ),
          );
        }),
      ),
    );
  }
}
