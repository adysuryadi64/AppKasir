import 'dart:async';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../widgets/app_drawer.dart';
import 'transfer_stok_screen.dart';
import 'detail_transfer_screen.dart';

class TransferListScreen extends StatefulWidget {
  const TransferListScreen({super.key});
  @override
  State<TransferListScreen> createState() => _TransferListScreenState();
}

class _TransferListScreenState extends State<TransferListScreen> {
  static const _teal = Color(0xFF0D9488);

  List<Map<String, dynamic>> _list = [];
  bool _isLoading = false;
  bool _isLoadingMore = false;
  bool _hasMore = true;
  bool _hasError = false;

  late DateTime _tglDari;
  late DateTime _tglSampai;

  final _searchCtrl = TextEditingController();
  Timer? _debounce;
  final _scrollCtrl = ScrollController();

  final _fmtNum = NumberFormat('#,##0.##', 'id_ID');
  final _fmtFilter = DateFormat('dd MMM yyyy');
  final _fmtTgl = DateFormat('dd MMM yyyy  HH:mm', 'id_ID');

  static const _pageSize = 30;

  @override
  void initState() {
    super.initState();
    _tglSampai = DateTime.now();
    _tglDari = DateTime(_tglSampai.year, _tglSampai.month, _tglSampai.day);
    _load(reset: true);
    _scrollCtrl.addListener(_onScroll);
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _searchCtrl.dispose();
    _scrollCtrl.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollCtrl.position.pixels >=
        _scrollCtrl.position.maxScrollExtent - 200) {
      if (!_isLoadingMore && _hasMore) _loadMore();
    }
  }

  Future<void> _load({bool reset = false}) async {
    if (reset) {
      setState(() {
        _isLoading = true;
        _hasError = false;
        _list = [];
        _hasMore = true;
      });
    }
    try {
      final lokasi = context.read<AuthProvider>().selectedLocation ?? '';
      final res = await ApiService.getTransferList(
        lokasi: lokasi,
        tglDari: DateFormat('yyyy-MM-dd').format(_tglDari),
        tglSampai: DateFormat('yyyy-MM-dd').format(_tglSampai),
        search: _searchCtrl.text.trim(),
        limit: _pageSize,
        offset: 0,
      );
      if (!mounted) return;
      if (res['status'] == 'success') {
        final data = List<Map<String, dynamic>>.from(res['data'] ?? []);
        setState(() {
          _list = data;
          _hasMore = data.length >= _pageSize;
        });
      } else {
        setState(() => _hasError = true);
      }
    } catch (e) {
      debugPrint('[TransferList] ❌ load error: $e');
      if (mounted) setState(() => _hasError = true);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _loadMore() async {
    if (_isLoadingMore || !_hasMore) return;
    setState(() => _isLoadingMore = true);
    try {
      final lokasi = context.read<AuthProvider>().selectedLocation ?? '';
      final res = await ApiService.getTransferList(
        lokasi: lokasi,
        tglDari: DateFormat('yyyy-MM-dd').format(_tglDari),
        tglSampai: DateFormat('yyyy-MM-dd').format(_tglSampai),
        search: _searchCtrl.text.trim(),
        limit: _pageSize,
        offset: _list.length,
      );
      if (!mounted) return;
      if (res['status'] == 'success') {
        final data = List<Map<String, dynamic>>.from(res['data'] ?? []);
        setState(() {
          _list.addAll(data);
          _hasMore = data.length >= _pageSize;
        });
      }
    } catch (e) {
      debugPrint('[TransferList] ❌ loadMore error: $e');
    } finally {
      if (mounted) setState(() => _isLoadingMore = false);
    }
  }

  void _onSearchChanged(String q) {
    _debounce?.cancel();
    _debounce = Timer(
      const Duration(milliseconds: 400),
      () => _load(reset: true),
    );
  }

  Future<void> _pickDateRange() async {
    final picked = await showDateRangePicker(
      context: context,
      firstDate: DateTime(2020),
      lastDate: DateTime.now(),
      initialDateRange: DateTimeRange(start: _tglDari, end: _tglSampai),
      locale: const Locale('id', 'ID'),
      builder: (ctx, child) => Theme(
        data: Theme.of(ctx).copyWith(
          colorScheme: const ColorScheme.light(
            primary: _teal,
            onPrimary: Colors.white,
            surface: Colors.white,
          ),
        ),
        child: child!,
      ),
    );
    if (picked != null) {
      setState(() {
        _tglDari = picked.start;
        _tglSampai = picked.end;
      });
      _load(reset: true);
    }
  }

  void _quickFilter(int days) {
    setState(() {
      _tglSampai = DateTime.now();
      _tglDari = days == -1
          ? DateTime(DateTime.now().year, DateTime.now().month, 1)
          : _tglSampai.subtract(Duration(days: days));
    });
    _load(reset: true);
  }

  void _openNew() {
    Navigator.of(context)
        .push(MaterialPageRoute(builder: (_) => const TransferStokScreen()))
        .then((_) => _load(reset: true));
  }

  String _formatTgl(String raw) {
    try {
      return _fmtTgl.format(DateTime.parse(raw));
    } catch (_) {
      return raw;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const AppDrawer(),
      backgroundColor: const Color(0xFFF0FDF4),
      appBar: AppBar(
        title: const Text('Transfer Stok'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => _load(reset: true),
            tooltip: 'Refresh',
          ),
        ],
      ),
      body: Column(
        children: [
          // ── Filter tanggal ──────────────────────────────────────
          _buildFilterBar(),
          // ── Search ─────────────────────────────────────────────
          Container(
            color: Colors.white,
            padding: const EdgeInsets.fromLTRB(14, 0, 14, 10),
            child: TextField(
              controller: _searchCtrl,
              onChanged: _onSearchChanged,
              style: const TextStyle(fontSize: 14),
              decoration: InputDecoration(
                hintText: 'Cari no. transfer atau nama barang...',
                hintStyle: TextStyle(fontSize: 13, color: Colors.grey.shade400),
                prefixIcon: const Icon(Icons.search, size: 20),
                suffixIcon: _searchCtrl.text.isNotEmpty
                    ? IconButton(
                        icon: const Icon(Icons.clear, size: 18),
                        onPressed: () {
                          _searchCtrl.clear();
                          _load(reset: true);
                        },
                      )
                    : null,
                filled: true,
                fillColor: Colors.grey.shade50,
                contentPadding: const EdgeInsets.symmetric(vertical: 10),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide: BorderSide(color: Colors.grey.shade300),
                ),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide: BorderSide(color: Colors.grey.shade300),
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide: const BorderSide(color: _teal, width: 1.5),
                ),
              ),
            ),
          ),
          // ── List ────────────────────────────────────────────────
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator(color: _teal))
                : _hasError
                ? _buildError()
                : _list.isEmpty
                ? _buildEmpty()
                : RefreshIndicator(
                    onRefresh: () => _load(reset: true),
                    color: _teal,
                    child: ListView.builder(
                      controller: _scrollCtrl,
                      padding: const EdgeInsets.fromLTRB(14, 10, 14, 90),
                      itemCount: _list.length + (_isLoadingMore ? 1 : 0),
                      itemBuilder: (_, i) {
                        if (i == _list.length) {
                          return const Center(
                            child: Padding(
                              padding: EdgeInsets.all(16),
                              child: CircularProgressIndicator(
                                color: _teal,
                                strokeWidth: 2,
                              ),
                            ),
                          );
                        }
                        return _buildCard(_list[i]);
                      },
                    ),
                  ),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _openNew,
        backgroundColor: _teal,
        foregroundColor: Colors.white,
        icon: const Icon(Icons.add),
        label: const Text(
          'Transfer Baru',
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
      ),
    );
  }

  Widget _buildFilterBar() {
    return Container(
      color: Colors.white,
      padding: const EdgeInsets.fromLTRB(14, 10, 14, 10),
      child: Row(
        children: [
          Icon(Icons.date_range, size: 18, color: Colors.grey.shade500),
          const SizedBox(width: 8),
          Expanded(
            child: GestureDetector(
              onTap: _pickDateRange,
              child: Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 8,
                ),
                decoration: BoxDecoration(
                  color: _teal.withValues(alpha: 0.06),
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: _teal.withValues(alpha: 0.3)),
                ),
                child: Row(
                  children: [
                    Text(
                      '${_fmtFilter.format(_tglDari)}  –  ${_fmtFilter.format(_tglSampai)}',
                      style: const TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.w600,
                        color: _teal,
                      ),
                    ),
                    const Spacer(),
                    const Icon(
                      Icons.edit_calendar_outlined,
                      size: 16,
                      color: _teal,
                    ),
                  ],
                ),
              ),
            ),
          ),
          const SizedBox(width: 8),
          _quickBtn('7H', 7),
          const SizedBox(width: 4),
          _quickBtn('30H', 30),
          const SizedBox(width: 4),
          _quickBtn('Bln', -1),
        ],
      ),
    );
  }

  Widget _quickBtn(String label, int days) => GestureDetector(
    onTap: () => _quickFilter(days),
    child: Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 6),
      decoration: BoxDecoration(
        color: Colors.grey.shade100,
        borderRadius: BorderRadius.circular(8),
      ),
      child: Text(
        label,
        style: TextStyle(
          fontSize: 11,
          fontWeight: FontWeight.w600,
          color: Colors.grey.shade600,
        ),
      ),
    ),
  );

  Widget _buildCard(Map<String, dynamic> item) {
    final noTransfer = item['ID_TRANSFER']?.toString() ?? '—';
    final tglRaw = item['TANGGAL']?.toString() ?? '';
    final user = item['ID_USER']?.toString() ?? '';
    final uraian = item['URAIAN']?.toString() ?? '';

    // Barang keluar
    final namaKlr = item['NAMA_BARANG_K']?.toString() ?? '—';
    final qtyKlr = double.tryParse(item['QTY_K']?.toString() ?? '0') ?? 0;
    final satuanKlr = item['SATUAN_K']?.toString() ?? '';
    final totalKlr =
        double.tryParse(item['TOTAL_HARGA_K']?.toString() ?? '0') ?? 0;

    // Barang masuk
    final namaMsk = item['NAMA_BARANG_M']?.toString() ?? '—';
    final qtyMsk = double.tryParse(item['QTY_M']?.toString() ?? '0') ?? 0;
    final satuanMsk = item['SATUAN_M']?.toString() ?? '';
    final totalMsk =
        double.tryParse(item['TOTAL_HARGA_M']?.toString() ?? '0') ?? 0;

    final selisih = double.tryParse(item['Selisih']?.toString() ?? '0') ?? 0;
    final lokasi = item['LOKASI']?.toString() ?? '';

    final tglFmt = _formatTgl(tglRaw);
    final isToko = lokasi != 'GUDANG';
    final lokasiColor = isToko
        ? const Color(0xFF16A34A)
        : const Color(0xFF0D9488);

    final selisihColor = selisih == 0
        ? Colors.grey.shade500
        : selisih > 0
        ? Colors.green.shade700
        : Colors.red.shade700;

    return GestureDetector(
      onTap: () => Navigator.of(context).push(
        MaterialPageRoute(
          builder: (_) => DetailTransferScreen(idTransfer: noTransfer),
        ),
      ),
      child: Container(
        margin: const EdgeInsets.only(bottom: 10),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: Colors.grey.shade200),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.04),
              blurRadius: 6,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Padding(
          padding: const EdgeInsets.all(14),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // ── Baris 1: No Transfer + badge lokasi ───────────
              Row(
                children: [
                  Expanded(
                    child: Text(
                      noTransfer,
                      style: const TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.bold,
                        color: Color(0xFF1E293B),
                      ),
                    ),
                  ),
                  _badge(
                    lokasi.isNotEmpty ? lokasi : '—',
                    lokasiColor,
                    isToko
                        ? Icons.storefront_outlined
                        : Icons.warehouse_outlined,
                  ),
                ],
              ),
              const SizedBox(height: 4),
              // ── Baris 2: Tanggal + user ────────────────────────
              Row(
                children: [
                  Icon(
                    Icons.access_time,
                    size: 11,
                    color: Colors.grey.shade400,
                  ),
                  const SizedBox(width: 3),
                  Text(
                    tglFmt,
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                  ),
                  const SizedBox(width: 10),
                  Icon(
                    Icons.person_outline,
                    size: 11,
                    color: Colors.grey.shade400,
                  ),
                  const SizedBox(width: 3),
                  Text(
                    user,
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                  ),
                ],
              ),
              if (uraian.isNotEmpty) ...[
                const SizedBox(height: 3),
                Text(
                  uraian,
                  style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
              ],
              const SizedBox(height: 10),
              const Divider(height: 1),
              const SizedBox(height: 10),

              // ── Barang keluar → masuk ──────────────────────────
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Keluar
                  Expanded(
                    child: _itemBox(
                      label: 'Keluar',
                      nama: namaKlr,
                      qty: qtyKlr,
                      satuan: satuanKlr,
                      total: totalKlr,
                      color: Colors.red.shade700,
                    ),
                  ),
                  // Panah
                  Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 8,
                      vertical: 12,
                    ),
                    child: Icon(
                      Icons.swap_horiz_rounded,
                      color: Colors.grey.shade400,
                      size: 20,
                    ),
                  ),
                  // Masuk
                  Expanded(
                    child: _itemBox(
                      label: 'Masuk',
                      nama: namaMsk,
                      qty: qtyMsk,
                      satuan: satuanMsk,
                      total: totalMsk,
                      color: _teal,
                    ),
                  ),
                ],
              ),

              // ── Selisih ────────────────────────────────────────
              if (selisih != 0) ...[
                const SizedBox(height: 8),
                Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    Text(
                      'Selisih: ',
                      style: TextStyle(
                        fontSize: 11,
                        color: Colors.grey.shade500,
                      ),
                    ),
                    Text(
                      '${selisih >= 0 ? '+' : ''}${_fmtNum.format(selisih)}',
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                        color: selisihColor,
                      ),
                    ),
                  ],
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  Widget _itemBox({
    required String label,
    required String nama,
    required double qty,
    required String satuan,
    required double total,
    required Color color,
  }) {
    return Container(
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.05),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: color.withValues(alpha: 0.2)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: TextStyle(
              fontSize: 9,
              fontWeight: FontWeight.w700,
              color: color,
              letterSpacing: 0.5,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            nama,
            style: const TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w600,
              color: Color(0xFF1E293B),
            ),
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
          const SizedBox(height: 4),
          Text(
            '${_fmtNum.format(qty)} $satuan',
            style: TextStyle(
              fontSize: 13,
              fontWeight: FontWeight.bold,
              color: color,
            ),
          ),
          Text(
            'Rp ${_fmtNum.format(total)}',
            style: TextStyle(fontSize: 10, color: Colors.grey.shade500),
          ),
        ],
      ),
    );
  }

  Widget _badge(String label, Color color, IconData icon) => Container(
    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
    decoration: BoxDecoration(
      color: color.withValues(alpha: 0.1),
      borderRadius: BorderRadius.circular(16),
      border: Border.all(color: color.withValues(alpha: 0.3)),
    ),
    child: Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 11, color: color),
        const SizedBox(width: 3),
        Text(
          label,
          style: TextStyle(
            fontSize: 10,
            fontWeight: FontWeight.w600,
            color: color,
          ),
        ),
      ],
    ),
  );

  Widget _buildEmpty() => Center(
    child: Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(Icons.swap_horiz, size: 64, color: Colors.grey.shade300),
        const SizedBox(height: 14),
        Text(
          'Belum ada transfer stok',
          style: TextStyle(color: Colors.grey.shade500, fontSize: 15),
        ),
        const SizedBox(height: 6),
        Text(
          'Tekan + untuk transfer baru',
          style: TextStyle(color: Colors.grey.shade400, fontSize: 12),
        ),
      ],
    ),
  );

  Widget _buildError() => Center(
    child: Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(Icons.wifi_off, size: 56, color: Colors.grey.shade300),
        const SizedBox(height: 14),
        Text(
          'Gagal memuat data',
          style: TextStyle(color: Colors.grey.shade500, fontSize: 15),
        ),
        const SizedBox(height: 14),
        ElevatedButton.icon(
          onPressed: () => _load(reset: true),
          icon: const Icon(Icons.refresh),
          label: const Text('Coba Lagi'),
          style: ElevatedButton.styleFrom(
            backgroundColor: _teal,
            foregroundColor: Colors.white,
          ),
        ),
      ],
    ),
  );
}
