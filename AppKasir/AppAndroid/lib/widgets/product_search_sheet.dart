import 'dart:async';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../services/api_service.dart';
import '../utils/safe_convert.dart';

/// Bottom-sheet pencarian barang dengan debounce 400 ms.
/// Dipakai oleh PenjualanScreen maupun StokOpnameScreen.
///
/// [onProductSelected] dipanggil saat user mengetuk sebuah produk.
/// [lokasi] menentukan kolom stok yang ditampilkan: 'TOKO' atau 'GUDANG'.
class ProductSearchSheet extends StatefulWidget {
  final void Function(Map<String, dynamic> product) onProductSelected;
  final double heightFactor;
  final int limit;
  final String lokasi;

  const ProductSearchSheet({
    super.key,
    required this.onProductSelected,
    this.heightFactor = 0.75,
    this.limit = 20,
    this.lokasi = 'TOKO',
  });

  @override
  State<ProductSearchSheet> createState() => _ProductSearchSheetState();
}

class _ProductSearchSheetState extends State<ProductSearchSheet> {
  final _searchController = TextEditingController();
  final _scrollController = ScrollController();

  List<Map<String, dynamic>> _results = [];
  bool _isLoading = false;
  bool _hasSearched = false;
  String _lastQuery = '';

  Timer? _debounce;

  @override
  void dispose() {
    _debounce?.cancel();
    _searchController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  // ── Pencarian dengan debounce ──────────────────────────────────

  void _onQueryChanged(String value) {
    _debounce?.cancel();
    final trimmed = value.trim();

    if (trimmed.length < 2) {
      if (_hasSearched) {
        setState(() {
          _results = [];
          _hasSearched = false;
        });
      }
      return;
    }

    _debounce = Timer(const Duration(milliseconds: 400), () {
      if (trimmed != _lastQuery) {
        _runSearch(trimmed);
      }
    });
  }

  Future<void> _runSearch(String query) async {
    if (!mounted) return;
    setState(() {
      _isLoading = true;
      _hasSearched = true;
      _lastQuery = query;
    });

    try {
      final response = await ApiService.getStock(search: query, limit: widget.limit);
      if (!mounted) return;

      if (response['status'] == 'success') {
        final raw = response['data'];
        setState(() {
          _results = raw is List
              ? List<Map<String, dynamic>>.from(raw)
              : [];
        });
      } else {
        _showError(response['message']?.toString() ?? 'Pencarian gagal');
      }
    } catch (e) {
      if (!mounted) return;
      _showError('Error: $e');
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  void _showError(String msg) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(msg),
        backgroundColor: Colors.red,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      ),
    );
  }

  // ── Build ──────────────────────────────────────────────────────

  @override
  Widget build(BuildContext context) {
    return Container(
      height: MediaQuery.of(context).size.height * widget.heightFactor,
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      child: Column(
        children: [
          _buildHandle(),
          _buildHeader(),
          _buildSearchField(),
          const Divider(height: 1),
          Expanded(child: _buildBody()),
        ],
      ),
    );
  }

  Widget _buildHandle() => Container(
        width: 40,
        height: 4,
        margin: const EdgeInsets.symmetric(vertical: 12),
        decoration: BoxDecoration(
          color: Colors.grey.shade300,
          borderRadius: BorderRadius.circular(2),
        ),
      );

  Widget _buildHeader() => Padding(
        padding: const EdgeInsets.fromLTRB(20, 0, 8, 8),
        child: Row(
          children: [
            const Text(
              'Cari Barang',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const Spacer(),
            IconButton(
              onPressed: () => Navigator.of(context).pop(),
              icon: const Icon(Icons.close),
            ),
          ],
        ),
      );

  Widget _buildSearchField() => Padding(
        padding: const EdgeInsets.fromLTRB(16, 0, 16, 12),
        child: TextField(
          controller: _searchController,
          autofocus: true,
          textInputAction: TextInputAction.search,
          decoration: InputDecoration(
            hintText: 'Nama, barcode, kategori, merk...',
            prefixIcon: const Icon(Icons.search),
            suffixIcon: _searchController.text.isNotEmpty
                ? IconButton(
                    icon: const Icon(Icons.clear),
                    onPressed: () {
                      _searchController.clear();
                      _onQueryChanged('');
                    },
                  )
                : null,
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
            contentPadding:
                const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
          ),
          onChanged: (v) {
            setState(() {}); // rebuild suffixIcon
            _onQueryChanged(v);
          },
          onSubmitted: (v) {
            _debounce?.cancel();
            final trimmed = v.trim();
            if (trimmed.length >= 2) _runSearch(trimmed);
          },
        ),
      );

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (!_hasSearched) {
      return _buildHint();
    }

    if (_results.isEmpty) {
      return _buildEmpty();
    }

    return ListView.builder(
      controller: _scrollController,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      itemCount: _results.length,
      itemBuilder: (context, index) => _buildProductTile(_results[index]),
    );
  }

  Widget _buildHint() => Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.search, size: 64, color: Colors.grey.shade300),
            const SizedBox(height: 12),
            Text(
              'Ketik minimal 2 karakter untuk mencari',
              style: TextStyle(color: Colors.grey.shade500),
            ),
          ],
        ),
      );

  Widget _buildEmpty() => Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.inventory_2_outlined, size: 64, color: Colors.grey.shade300),
            const SizedBox(height: 12),
            Text(
              'Barang tidak ditemukan',
              style: TextStyle(
                  color: Colors.grey.shade600, fontWeight: FontWeight.w500),
            ),
            const SizedBox(height: 4),
            Text(
              'Coba kata kunci lain',
              style: TextStyle(color: Colors.grey.shade400, fontSize: 13),
            ),
          ],
        ),
      );

  Widget _buildProductTile(Map<String, dynamic> product) {
    // Tampilkan stok sesuai lokasi login — TOKO atau GUDANG
    final stok = widget.lokasi == 'GUDANG'
        ? safeToDouble(product['STOK_GUDANG'])
        : safeToDouble(product['STOK_TOKO']);
    final harga = safeToDouble(product['HARGA_JUAL']);
    final kategori = product['Kategori']?.toString() ?? '';
    final merk = product['Merk']?.toString() ?? '';
    final subtitle = [kategori, merk].where((s) => s.isNotEmpty).join(' · ');

    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      elevation: 1,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      child: InkWell(
        borderRadius: BorderRadius.circular(10),
        onTap: () => widget.onProductSelected(product),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              // Icon
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  color: Colors.blue.shade50,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Icon(Icons.inventory_2,
                    color: Colors.blue.shade300, size: 26),
              ),
              const SizedBox(width: 12),

              // Info
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      product['NAMA_BARANG']?.toString() ?? '-',
                      style: const TextStyle(
                          fontSize: 14, fontWeight: FontWeight.w600),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                    if (subtitle.isNotEmpty) ...[
                      const SizedBox(height: 2),
                      Text(
                        subtitle,
                        style: TextStyle(
                            fontSize: 11, color: Colors.grey.shade500),
                      ),
                    ],
                    const SizedBox(height: 4),
                    Text(
                      NumberFormat.currency(
                              locale: 'id_ID', symbol: 'Rp ', decimalDigits: 0)
                          .format(harga),
                      style: const TextStyle(
                          fontSize: 13,
                          fontWeight: FontWeight.bold,
                          color: Colors.green),
                    ),
                  ],
                ),
              ),

              // Stok badge
              _StokBadge(stok: stok),
            ],
          ),
        ),
      ),
    );
  }
}

// ── Stok badge ─────────────────────────────────────────────────────────────

class _StokBadge extends StatelessWidget {
  final double stok;
  const _StokBadge({required this.stok});

  Color get _color {
    if (stok <= 0) return Colors.red;
    if (stok <= 5) return Colors.orange;
    if (stok <= 20) return Colors.amber.shade700;
    return Colors.green;
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 6),
      decoration: BoxDecoration(
        color: _color,
        borderRadius: BorderRadius.circular(6),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Text('STOK',
              style: TextStyle(
                  fontSize: 8,
                  color: Colors.white,
                  fontWeight: FontWeight.bold)),
          Text(
            stok.truncate() == stok
                ? stok.toInt().toString()
                : stok.toStringAsFixed(1),
            style: const TextStyle(
                fontSize: 13,
                color: Colors.white,
                fontWeight: FontWeight.bold),
          ),
        ],
      ),
    );
  }
}
