import 'dart:async';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../widgets/app_drawer.dart';

class LaporanStokScreen extends StatefulWidget {
  const LaporanStokScreen({super.key});

  @override
  State<LaporanStokScreen> createState() => _LaporanStokScreenState();
}

class _LaporanStokScreenState extends State<LaporanStokScreen> {
  static const _pageSize = 50;

  final _searchCtrl = TextEditingController();
  final _scrollCtrl = ScrollController();
  Timer? _debounce;

  List<Map<String, dynamic>> _items = [];
  bool _isLoading = false;
  bool _isLoadingMore = false;
  bool _hasMore = true;
  int _offset = 0;
  int _totalCount = 0;
  String _search = '';
  String _selectedKategori = '';
  List<String> _kategoriList = [];

  final _fmt = NumberFormat('#,##0.##', 'id_ID');

  @override
  void initState() {
    super.initState();
    _scrollCtrl.addListener(_onScroll);
    WidgetsBinding.instance.addPostFrameCallback((_) => _loadData(reset: true));
  }

  @override
  void dispose() {
    _searchCtrl.dispose();
    _scrollCtrl.dispose();
    _debounce?.cancel();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollCtrl.position.pixels >=
        _scrollCtrl.position.maxScrollExtent - 200) {
      if (!_isLoadingMore && _hasMore) _loadMore();
    }
  }

  Future<void> _loadData({bool reset = false}) async {
    if (_isLoading) return;
    if (reset) {
      setState(() {
        _items = [];
        _offset = 0;
        _hasMore = true;
        _isLoading = true;
      });
    }

    try {
      final res = await ApiService.getLaporanStok(
        search: _search,
        kategori: _selectedKategori,
        limit: _pageSize,
        offset: _offset,
      );

      if (!mounted) return;

      if (res['status'] == 'success') {
        final data = List<Map<String, dynamic>>.from(res['data'] ?? []);
        final total = int.tryParse(res['total_count']?.toString() ?? '0') ?? 0;
        debugPrint(
          '[LaporanStok] ✅ load: ${data.length} item, total=$total, reset=$reset',
        );

        // Kumpulkan kategori unik untuk filter
        if (reset && _kategoriList.isEmpty) {
          final cats =
              data
                  .map((e) => e['NAMA_KATEGORI']?.toString() ?? '')
                  .where((c) => c.isNotEmpty)
                  .toSet()
                  .toList()
                ..sort();
          _kategoriList = cats;
        }

        setState(() {
          if (reset) {
            _items = data;
          } else {
            _items.addAll(data);
          }
          _totalCount = total;
          _offset = _items.length;
          _hasMore = _items.length < total;
        });
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Gagal memuat data: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _loadMore() async {
    if (_isLoadingMore || !_hasMore) return;
    setState(() => _isLoadingMore = true);

    try {
      final res = await ApiService.getLaporanStok(
        search: _search,
        kategori: _selectedKategori,
        limit: _pageSize,
        offset: _offset,
      );

      if (!mounted) return;

      if (res['status'] == 'success') {
        final data = List<Map<String, dynamic>>.from(res['data'] ?? []);
        setState(() {
          _items.addAll(data);
          _offset = _items.length;
          _hasMore = _items.length < _totalCount;
        });
      }
    } catch (e) {
      debugPrint('[LaporanStok] ❌ loadMore error: $e');
    } finally {
      if (mounted) setState(() => _isLoadingMore = false);
    }
  }

  void _onSearchChanged(String val) {
    _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 400), () {
      _search = val.trim();
      _loadData(reset: true);
    });
  }

  @override
  Widget build(BuildContext context) {
    final auth = Provider.of<AuthProvider>(context);
    final lokasi = auth.selectedLocation ?? '';

    return Scaffold(
      drawer: const AppDrawer(),
      appBar: AppBar(
        title: const Text('Laporan Stok'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => _loadData(reset: true),
            tooltip: 'Refresh',
          ),
        ],
      ),
      body: Column(
        children: [
          // ── Search + Filter ──────────────────────────────────────
          Container(
            color: Colors.white,
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 8),
            child: Column(
              children: [
                // Search field
                TextField(
                  controller: _searchCtrl,
                  onChanged: _onSearchChanged,
                  decoration: InputDecoration(
                    hintText: 'Cari nama barang atau barcode...',
                    prefixIcon: const Icon(Icons.search, size: 20),
                    suffixIcon: _searchCtrl.text.isNotEmpty
                        ? IconButton(
                            icon: const Icon(Icons.clear, size: 18),
                            onPressed: () {
                              _searchCtrl.clear();
                              _search = '';
                              _loadData(reset: true);
                            },
                          )
                        : null,
                    contentPadding: const EdgeInsets.symmetric(
                      horizontal: 16,
                      vertical: 12,
                    ),
                  ),
                ),
                const SizedBox(height: 8),
                // Filter kategori + info lokasi
                Row(
                  children: [
                    Expanded(
                      child: _kategoriList.isEmpty
                          ? const SizedBox.shrink()
                          : DropdownButtonFormField<String>(
                              // ignore: deprecated_member_use
                              value: _selectedKategori.isEmpty
                                  ? null
                                  : _selectedKategori,
                              hint: const Text(
                                'Semua Kategori',
                                style: TextStyle(fontSize: 13),
                              ),
                              isExpanded: true,
                              decoration: InputDecoration(
                                contentPadding: const EdgeInsets.symmetric(
                                  horizontal: 12,
                                  vertical: 10,
                                ),
                                border: OutlineInputBorder(
                                  borderRadius: BorderRadius.circular(10),
                                ),
                              ),
                              items: [
                                const DropdownMenuItem(
                                  value: '',
                                  child: Text(
                                    'Semua Kategori',
                                    style: TextStyle(fontSize: 13),
                                  ),
                                ),
                                ..._kategoriList.map(
                                  (k) => DropdownMenuItem(
                                    value: k,
                                    child: Text(
                                      k,
                                      style: const TextStyle(fontSize: 13),
                                    ),
                                  ),
                                ),
                              ],
                              onChanged: (v) {
                                setState(() => _selectedKategori = v ?? '');
                                _loadData(reset: true);
                              },
                            ),
                    ),
                    const SizedBox(width: 8),
                    // Badge lokasi
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 10,
                        vertical: 6,
                      ),
                      decoration: BoxDecoration(
                        color: lokasi == 'TOKO'
                            ? const Color(0xFF10B981).withValues(alpha: 0.1)
                            : const Color(0xFFF59E0B).withValues(alpha: 0.1),
                        borderRadius: BorderRadius.circular(8),
                        border: Border.all(
                          color: lokasi == 'TOKO'
                              ? const Color(0xFF10B981)
                              : const Color(0xFFF59E0B),
                        ),
                      ),
                      child: Text(
                        lokasi.isNotEmpty ? lokasi : '—',
                        style: TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.w600,
                          color: lokasi == 'TOKO'
                              ? const Color(0xFF059669)
                              : const Color(0xFFD97706),
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),

          // ── Info count ───────────────────────────────────────────
          if (!_isLoading)
            Container(
              color: Colors.grey.shade50,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
              child: Row(
                children: [
                  Text(
                    'Menampilkan ${_items.length} dari $_totalCount barang',
                    style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
                  ),
                ],
              ),
            ),

          // ── List ─────────────────────────────────────────────────
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _items.isEmpty
                ? _buildEmpty()
                : RefreshIndicator(
                    onRefresh: () => _loadData(reset: true),
                    child: ListView.builder(
                      controller: _scrollCtrl,
                      itemCount: _items.length + (_isLoadingMore ? 1 : 0),
                      itemBuilder: (ctx, i) {
                        if (i == _items.length) {
                          return const Padding(
                            padding: EdgeInsets.all(16),
                            child: Center(child: CircularProgressIndicator()),
                          );
                        }
                        return _buildItem(_items[i], lokasi);
                      },
                    ),
                  ),
          ),
        ],
      ),
    );
  }

  Widget _buildItem(Map<String, dynamic> item, String lokasi) {
    final stokToko = double.tryParse(item['STOK_TOKO']?.toString() ?? '0') ?? 0;
    final stokGudang =
        double.tryParse(item['STOK_GUDANG']?.toString() ?? '0') ?? 0;
    final stokAktif = lokasi == 'GUDANG' ? stokGudang : stokToko;
    final isLow = stokAktif <= 0;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: isLow ? Colors.red.shade200 : Colors.grey.shade200,
        ),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.03),
            blurRadius: 4,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          children: [
            // Stok badge
            Container(
              width: 56,
              height: 56,
              decoration: BoxDecoration(
                color: isLow
                    ? Colors.red.shade50
                    : const Color(0xFF2563EB).withValues(alpha: 0.08),
                borderRadius: BorderRadius.circular(10),
              ),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    _fmt.format(stokAktif),
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: isLow
                          ? Colors.red.shade600
                          : const Color(0xFF2563EB),
                    ),
                  ),
                  Text(
                    item['SATUAN']?.toString() ?? '',
                    style: TextStyle(
                      fontSize: 10,
                      color: isLow ? Colors.red.shade400 : Colors.grey.shade500,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: 12),
            // Info barang
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item['NAMA_BARANG']?.toString() ?? '',
                    style: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w600,
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 2),
                  Text(
                    item['ID_BARANG']?.toString() ?? '',
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                  ),
                  const SizedBox(height: 4),
                  Row(
                    children: [
                      if ((item['NAMA_KATEGORI']?.toString() ?? '').isNotEmpty)
                        _chip(item['NAMA_KATEGORI'].toString(), Colors.blue),
                      if ((item['NAMA_MERK']?.toString() ?? '').isNotEmpty) ...[
                        const SizedBox(width: 4),
                        _chip(item['NAMA_MERK'].toString(), Colors.purple),
                      ],
                    ],
                  ),
                ],
              ),
            ),
            // Stok toko & gudang
            Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                _stokBadge('Toko', stokToko, lokasi == 'TOKO'),
                const SizedBox(height: 4),
                _stokBadge('Gudang', stokGudang, lokasi == 'GUDANG'),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _chip(String label, MaterialColor color) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
      decoration: BoxDecoration(
        color: color.shade50,
        borderRadius: BorderRadius.circular(4),
      ),
      child: Text(
        label,
        style: TextStyle(fontSize: 10, color: color.shade700),
        maxLines: 1,
        overflow: TextOverflow.ellipsis,
      ),
    );
  }

  Widget _stokBadge(String label, double stok, bool isActive) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      decoration: BoxDecoration(
        color: isActive
            ? const Color(0xFF2563EB).withValues(alpha: 0.1)
            : Colors.grey.shade100,
        borderRadius: BorderRadius.circular(6),
        border: isActive
            ? Border.all(color: const Color(0xFF2563EB).withValues(alpha: 0.3))
            : null,
      ),
      child: Column(
        children: [
          Text(
            label,
            style: TextStyle(
              fontSize: 9,
              color: isActive ? const Color(0xFF2563EB) : Colors.grey.shade500,
            ),
          ),
          Text(
            _fmt.format(stok),
            style: TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w600,
              color: isActive ? const Color(0xFF2563EB) : Colors.grey.shade700,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildEmpty() {
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            Icons.inventory_2_outlined,
            size: 64,
            color: Colors.grey.shade300,
          ),
          const SizedBox(height: 16),
          Text(
            _search.isNotEmpty
                ? 'Barang tidak ditemukan'
                : 'Belum ada data barang',
            style: TextStyle(color: Colors.grey.shade500, fontSize: 15),
          ),
          if (_search.isNotEmpty) ...[
            const SizedBox(height: 8),
            TextButton(
              onPressed: () {
                _searchCtrl.clear();
                _search = '';
                _loadData(reset: true);
              },
              child: const Text('Hapus pencarian'),
            ),
          ],
        ],
      ),
    );
  }
}
