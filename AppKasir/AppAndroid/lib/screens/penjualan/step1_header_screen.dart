import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/penjualan_provider.dart';
import '../../services/api_service.dart';

class Step1HeaderScreen extends StatefulWidget {
  final VoidCallback onNext;
  const Step1HeaderScreen({super.key, required this.onNext});

  @override
  State<Step1HeaderScreen> createState() => _Step1HeaderScreenState();
}

class _Step1HeaderScreenState extends State<Step1HeaderScreen> {
  static const _green = Color(0xFF16A34A);

  // ── Data master ──────────────────────────────────────────────
  List<Map<String, dynamic>> _pelangganList = [];
  List<Map<String, dynamic>> _salesList = [];
  List<Map<String, dynamic>> _pelangganFiltered = [];
  List<Map<String, dynamic>> _salesFiltered = [];
  bool _loadingPelanggan = false;
  bool _loadingSales = false;

  // ── Controller pelanggan ─────────────────────────────────────
  final _pelangganCtrl = TextEditingController();
  final _pelangganFocus = FocusNode();
  bool _showPelangganDrop = false;

  // ── Controller sales ─────────────────────────────────────────
  final _salesCtrl = TextEditingController();
  final _salesFocus = FocusNode();
  bool _showSalesDrop = false;

  @override
  void initState() {
    super.initState();
    _loadData();

    _pelangganFocus.addListener(() {
      if (!_pelangganFocus.hasFocus) {
        Future.delayed(const Duration(milliseconds: 150), () {
          if (mounted) setState(() => _showPelangganDrop = false);
        });
      }
    });

    _salesFocus.addListener(() {
      if (!_salesFocus.hasFocus) {
        Future.delayed(const Duration(milliseconds: 150), () {
          if (mounted) setState(() => _showSalesDrop = false);
        });
      }
    });
  }

  @override
  void dispose() {
    _pelangganCtrl.dispose();
    _pelangganFocus.dispose();
    _salesCtrl.dispose();
    _salesFocus.dispose();
    super.dispose();
  }

  Future<void> _loadData() async {
    setState(() {
      _loadingPelanggan = true;
      _loadingSales = true;
    });
    final results = await Future.wait([
      ApiService.getPelanggan().catchError((_) => <String, dynamic>{}),
      ApiService.getKaryawan().catchError((_) => <String, dynamic>{}),
    ]);
    if (!mounted) return;
    setState(() {
      if (results[0]['status'] == 'success') {
        _pelangganList = List<Map<String, dynamic>>.from(
          results[0]['data'] ?? [],
        );
        _pelangganFiltered = List.from(_pelangganList);
      }
      if (results[1]['status'] == 'success') {
        _salesList = List<Map<String, dynamic>>.from(results[1]['data'] ?? []);
        _salesFiltered = List.from(_salesList);
      }
      _loadingPelanggan = false;
      _loadingSales = false;
    });

    // Sinkronkan controller dengan state provider
    final prov = context.read<PenjualanProvider>();
    if (prov.selectedPelanggan != null) {
      _pelangganCtrl.text = prov.selectedPelanggan!['NAMA']?.toString() ?? '';
    }
    if (prov.selectedSales != null) {
      _salesCtrl.text = prov.selectedSales!['Nama']?.toString() ?? '';
    }
  }

  // ── Filter & pilih pelanggan ─────────────────────────────────
  void _filterPelanggan(String q) {
    final lower = q.toLowerCase();
    setState(() {
      _pelangganFiltered = _pelangganList
          .where(
            (p) =>
                (p['NAMA']?.toString().toLowerCase().contains(lower) ??
                    false) ||
                (p['KODE']?.toString().toLowerCase().contains(lower) ?? false),
          )
          .toList();
      _showPelangganDrop = true;
    });
  }

  void _selectPelanggan(Map<String, dynamic> p) {
    context.read<PenjualanProvider>().selectedPelanggan = p;
    _pelangganCtrl.text = p['NAMA']?.toString() ?? '';
    _pelangganFocus.unfocus();
    setState(() => _showPelangganDrop = false);
  }

  void _clearPelanggan() {
    context.read<PenjualanProvider>().selectedPelanggan = null;
    _pelangganCtrl.clear();
    setState(() => _showPelangganDrop = false);
  }

  // ── Filter & pilih sales ─────────────────────────────────────
  void _filterSales(String q) {
    final lower = q.toLowerCase();
    setState(() {
      _salesFiltered = _salesList
          .where(
            (s) =>
                (s['Nama']?.toString().toLowerCase().contains(lower) ??
                    false) ||
                (s['Kode']?.toString().toLowerCase().contains(lower) ?? false),
          )
          .toList();
      _showSalesDrop = true;
    });
  }

  void _selectSales(Map<String, dynamic> s) {
    context.read<PenjualanProvider>().setSelectedSales(s);
    _salesCtrl.text = s['Nama']?.toString() ?? '';
    _salesFocus.unfocus();
    setState(() => _showSalesDrop = false);
  }

  void _clearSales() {
    context.read<PenjualanProvider>().setSelectedSales(null);
    _salesCtrl.clear();
    setState(() => _showSalesDrop = false);
  }

  @override
  Widget build(BuildContext context) {
    final prov = context.watch<PenjualanProvider>();

    return GestureDetector(
      onTap: () => FocusScope.of(context).unfocus(),
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(16, 20, 16, 100),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // ── Pelanggan ────────────────────────────────────────
            _sectionLabel('Pelanggan'),
            const SizedBox(height: 6),
            _buildPelangganField(prov),
            if (prov.selectedPelanggan != null) ...[
              const SizedBox(height: 8),
              _pelangganBadge(prov.selectedPelanggan!),
            ],

            const SizedBox(height: 20),

            // ── Sales ────────────────────────────────────────────
            _sectionLabel('Sales (Opsional)'),
            const SizedBox(height: 6),
            _buildSalesField(prov),
            if (prov.selectedSales != null) ...[
              const SizedBox(height: 8),
              _salesBadge(prov.selectedSales!),
            ],

            const SizedBox(height: 36),

            // ── Tombol Lanjut ────────────────────────────────────
            SizedBox(
              width: double.infinity,
              height: 52,
              child: ElevatedButton.icon(
                onPressed: widget.onNext,
                icon: const Icon(Icons.arrow_forward),
                label: const Text(
                  'Lanjut ke Barang →',
                  style: TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  // ── Field pencarian pelanggan + dropdown ─────────────────────
  Widget _buildPelangganField(PenjualanProvider prov) {
    return Column(
      children: [
        TextField(
          controller: _pelangganCtrl,
          focusNode: _pelangganFocus,
          onChanged: _filterPelanggan,
          onTap: () {
            _filterPelanggan(_pelangganCtrl.text);
            setState(() => _showPelangganDrop = true);
          },
          style: const TextStyle(fontSize: 14),
          decoration: InputDecoration(
            hintText: 'Cari pelanggan (opsional)...',
            hintStyle: TextStyle(fontSize: 13, color: Colors.grey.shade400),
            prefixIcon: const Icon(Icons.person_search_outlined),
            suffixIcon: prov.selectedPelanggan != null
                ? IconButton(
                    icon: const Icon(Icons.clear, size: 18),
                    onPressed: _clearPelanggan,
                  )
                : _loadingPelanggan
                ? const Padding(
                    padding: EdgeInsets.all(12),
                    child: SizedBox(
                      width: 16,
                      height: 16,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    ),
                  )
                : null,
            filled: true,
            fillColor: Colors.white,
            contentPadding: const EdgeInsets.symmetric(vertical: 12),
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
              borderSide: const BorderSide(color: _green, width: 1.5),
            ),
          ),
        ),
        if (_showPelangganDrop && _pelangganFiltered.isNotEmpty)
          _dropdownList(
            items: _pelangganFiltered,
            nameKey: 'NAMA',
            subBuilder: (p) {
              final jenis = p['JENIS']?.toString().toUpperCase() ?? 'UMUM';
              return '${p['KODE'] ?? ''}  ·  $jenis';
            },
            iconBuilder: (p) {
              final jenis = p['JENIS']?.toString().toUpperCase() ?? 'UMUM';
              final isPartai = jenis == 'PARTAI' || jenis == 'GROSIR';
              return Icon(
                isPartai ? Icons.local_shipping_outlined : Icons.person_outline,
                size: 16,
                color: isPartai ? Colors.orange : _green,
              );
            },
            bgBuilder: (p) {
              final jenis = p['JENIS']?.toString().toUpperCase() ?? 'UMUM';
              final isPartai = jenis == 'PARTAI' || jenis == 'GROSIR';
              return isPartai
                  ? Colors.orange.shade50
                  : _green.withValues(alpha: 0.1);
            },
            onSelect: _selectPelanggan,
          ),
      ],
    );
  }

  // ── Field pencarian sales + dropdown (identik pola pelanggan) ─
  Widget _buildSalesField(PenjualanProvider prov) {
    return Column(
      children: [
        TextField(
          controller: _salesCtrl,
          focusNode: _salesFocus,
          onChanged: _filterSales,
          onTap: () {
            _filterSales(_salesCtrl.text);
            setState(() => _showSalesDrop = true);
          },
          style: const TextStyle(fontSize: 14),
          decoration: InputDecoration(
            hintText: 'Cari sales (opsional)...',
            hintStyle: TextStyle(fontSize: 13, color: Colors.grey.shade400),
            prefixIcon: const Icon(Icons.badge_outlined),
            suffixIcon: prov.selectedSales != null
                ? IconButton(
                    icon: const Icon(Icons.clear, size: 18),
                    onPressed: _clearSales,
                  )
                : _loadingSales
                ? const Padding(
                    padding: EdgeInsets.all(12),
                    child: SizedBox(
                      width: 16,
                      height: 16,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    ),
                  )
                : null,
            filled: true,
            fillColor: Colors.white,
            contentPadding: const EdgeInsets.symmetric(vertical: 12),
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
              borderSide: const BorderSide(color: _green, width: 1.5),
            ),
          ),
        ),
        if (_showSalesDrop && _salesFiltered.isNotEmpty)
          _dropdownList(
            items: _salesFiltered,
            nameKey: 'Nama',
            subBuilder: (s) => s['Kode']?.toString() ?? '',
            iconBuilder: (_) =>
                const Icon(Icons.badge_outlined, size: 16, color: _green),
            bgBuilder: (_) => _green.withValues(alpha: 0.1),
            onSelect: _selectSales,
          ),
      ],
    );
  }

  // ── Dropdown list generik ────────────────────────────────────
  Widget _dropdownList({
    required List<Map<String, dynamic>> items,
    required String nameKey,
    required String Function(Map<String, dynamic>) subBuilder,
    required Widget Function(Map<String, dynamic>) iconBuilder,
    required Color Function(Map<String, dynamic>) bgBuilder,
    required void Function(Map<String, dynamic>) onSelect,
  }) {
    return Container(
      constraints: const BoxConstraints(maxHeight: 200),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: const BorderRadius.vertical(bottom: Radius.circular(12)),
        border: Border.all(color: Colors.grey.shade200),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.06),
            blurRadius: 8,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: ListView.separated(
        shrinkWrap: true,
        padding: EdgeInsets.zero,
        itemCount: items.length,
        separatorBuilder: (_, idx) => const Divider(height: 1),
        itemBuilder: (_, i) {
          final item = items[i];
          return ListTile(
            dense: true,
            leading: CircleAvatar(
              radius: 16,
              backgroundColor: bgBuilder(item),
              child: iconBuilder(item),
            ),
            title: Text(
              item[nameKey]?.toString() ?? '',
              style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
            ),
            subtitle: Text(
              subBuilder(item),
              style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
            ),
            onTap: () => onSelect(item),
          );
        },
      ),
    );
  }

  // ── Badge pelanggan terpilih ─────────────────────────────────
  Widget _pelangganBadge(Map<String, dynamic> p) {
    final jenis = p['JENIS']?.toString().toUpperCase() ?? 'UMUM';
    final isPartai = jenis == 'PARTAI' || jenis == 'GROSIR';
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      decoration: BoxDecoration(
        color: isPartai
            ? Colors.orange.shade50
            : _green.withValues(alpha: 0.06),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(
          color: isPartai
              ? Colors.orange.shade200
              : _green.withValues(alpha: 0.3),
        ),
      ),
      child: Row(
        children: [
          Icon(
            isPartai ? Icons.local_shipping_outlined : Icons.person_outline,
            size: 16,
            color: isPartai ? Colors.orange : _green,
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  p['NAMA']?.toString() ?? '',
                  style: const TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Text(
                  '${p['KODE'] ?? ''}  ·  Harga ${isPartai ? 'Partai' : 'Umum'}',
                  style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  // ── Badge sales terpilih ─────────────────────────────────────
  Widget _salesBadge(Map<String, dynamic> s) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      decoration: BoxDecoration(
        color: _green.withValues(alpha: 0.06),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: _green.withValues(alpha: 0.3)),
      ),
      child: Row(
        children: [
          const Icon(Icons.badge_outlined, size: 16, color: _green),
          const SizedBox(width: 8),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  s['Nama']?.toString() ?? '',
                  style: const TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Text(
                  s['Kode']?.toString() ?? '',
                  style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _sectionLabel(String text) => Text(
    text,
    style: const TextStyle(
      fontSize: 12,
      fontWeight: FontWeight.w600,
      color: Color(0xFF64748B),
      letterSpacing: 0.5,
    ),
  );
}
