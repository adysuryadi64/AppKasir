import 'dart:async';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../widgets/app_drawer.dart';
import 'detail_penjualan_screen.dart';
import 'penjualan/penjualan_flow.dart';

class RiwayatPenjualanScreen extends StatefulWidget {
  const RiwayatPenjualanScreen({super.key});

  @override
  State<RiwayatPenjualanScreen> createState() => _RiwayatPenjualanScreenState();
}

class _RiwayatPenjualanScreenState extends State<RiwayatPenjualanScreen> {
  static const _green = Color(0xFF16A34A);

  List<Map<String, dynamic>> _list = [];
  Map<String, dynamic>? _summary;
  bool _isLoading = false;
  bool _isLoadingMore = false;
  bool _hasMore = true;
  bool _hasError = false;

  late DateTime _tglDari;
  late DateTime _tglSampai;

  final _searchCtrl = TextEditingController();
  Timer? _debounce;
  final _scrollCtrl = ScrollController();

  final _fmtNum = NumberFormat('#,##0', 'id_ID');
  final _fmtFilter = DateFormat('dd MMM yyyy');
  final _fmtTgl = DateFormat('dd/MM/yyyy HH:mm', 'id_ID');

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
      final res = await ApiService.getRiwayatPenjualan(
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
          _summary = res['summary'] as Map<String, dynamic>?;
          _hasMore = data.length >= _pageSize;
        });
      } else {
        setState(() => _hasError = true);
      }
    } catch (e) {
      debugPrint('[RiwayatPenjualan] ❌ load error: $e');
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
      final res = await ApiService.getRiwayatPenjualan(
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
      debugPrint('[RiwayatPenjualan] ❌ loadMore error: $e');
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
            primary: _green,
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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const AppDrawer(),
      backgroundColor: const Color(0xFFF0FDF4),
      appBar: AppBar(
        title: const Text('Riwayat Penjualan'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => _load(reset: true),
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
                hintText: 'Cari nomor faktur atau pelanggan...',
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
                  borderSide: const BorderSide(color: _green, width: 1.5),
                ),
              ),
            ),
          ),
          // ── Summary ─────────────────────────────────────────────
          if (_summary != null) _buildSummary(),
          // ── List ────────────────────────────────────────────────
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator(color: _green))
                : _hasError
                ? _buildError()
                : _list.isEmpty
                ? _buildEmpty()
                : RefreshIndicator(
                    onRefresh: () => _load(reset: true),
                    color: _green,
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
                                color: _green,
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
        onPressed: () => Navigator.of(context)
            .push(MaterialPageRoute(builder: (_) => const PenjualanFlow()))
            .then((_) => _load(reset: true)),
        icon: const Icon(Icons.add),
        label: const Text(
          'Penjualan Baru',
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
                  color: _green.withValues(alpha: 0.06),
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: _green.withValues(alpha: 0.3)),
                ),
                child: Row(
                  children: [
                    Text(
                      '${_fmtFilter.format(_tglDari)}  –  ${_fmtFilter.format(_tglSampai)}',
                      style: const TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.w600,
                        color: _green,
                      ),
                    ),
                    const Spacer(),
                    Icon(Icons.edit_calendar_outlined, size: 16, color: _green),
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

  Widget _buildSummary() {
    final total = double.tryParse(_summary!['TOTAL']?.toString() ?? '0') ?? 0;
    final record = _summary!['RECORD']?.toString() ?? '0';
    return Container(
      color: _green,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            '$record transaksi',
            style: const TextStyle(color: Colors.white70, fontSize: 12),
          ),
          Text(
            'Rp ${_fmtNum.format(total)}',
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.bold,
              fontSize: 14,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCard(Map<String, dynamic> item) {
    final noFaktur = item['ID_PENJUALAN']?.toString() ?? '—';
    final tglRaw = item['TGL_TRANSAKSI']?.toString() ?? '';
    final pelanggan = item['NAMA_PELANGGAN']?.toString() ?? 'Umum';
    final lokasi = item['LOKASIBARANG']?.toString() ?? '';
    final metode = item['JENIS_PEMBAYARAN']?.toString() ?? '';
    final total =
        double.tryParse(item['GRAND_TOTAL_STL_PAJAK']?.toString() ?? '0') ?? 0;
    final sisa = double.tryParse(item['SISA_TAGIHAN']?.toString() ?? '0') ?? 0;
    final status = item['STATUS_TRANSAKSI']?.toString() ?? '';
    final user = item['ID_USER']?.toString() ?? '';

    final isLunas = status.toUpperCase() == 'COMPLETED' || sisa == 0;
    final statusColor = isLunas ? _green : Colors.orange;
    final statusLabel = isLunas ? 'LUNAS' : 'BELUM LUNAS';

    String tglFmt = tglRaw;
    try {
      tglFmt = _fmtTgl.format(DateTime.parse(tglRaw));
    } catch (_) {}

    final isToko = lokasi != 'GUDANG';
    final lokasiColor = isToko ? _green : const Color(0xFF0D9488);

    return GestureDetector(
      onTap: () => Navigator.of(context).push(
        MaterialPageRoute(
          builder: (_) => DetailPenjualanScreen(idPenjualan: noFaktur),
        ),
      ),
      child: Container(
        margin: const EdgeInsets.only(bottom: 10),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: Colors.grey.shade200),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.03),
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
              // Baris 1: Faktur + badge lokasi
              Row(
                children: [
                  Expanded(
                    child: Text(
                      noFaktur,
                      style: const TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.bold,
                        color: Color(0xFF1E293B),
                      ),
                    ),
                  ),
                  _badge(
                    lokasi,
                    lokasiColor,
                    isToko
                        ? Icons.storefront_outlined
                        : Icons.warehouse_outlined,
                  ),
                ],
              ),
              const SizedBox(height: 4),
              // Baris 2: Tanggal + user
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
              const SizedBox(height: 8),
              const Divider(height: 1),
              const SizedBox(height: 8),
              // Pelanggan
              Text(
                pelanggan,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w700,
                  color: Color(0xFF1E293B),
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 8),
              // Total + status + metode
              Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Total',
                          style: TextStyle(
                            fontSize: 10,
                            color: Colors.grey.shade500,
                          ),
                        ),
                        Text(
                          'Rp ${_fmtNum.format(total)}',
                          style: const TextStyle(
                            fontSize: 14,
                            fontWeight: FontWeight.bold,
                            color: Color(0xFF1E293B),
                          ),
                        ),
                        if (sisa > 0)
                          Text(
                            'Piutang: Rp ${_fmtNum.format(sisa)}',
                            style: TextStyle(
                              fontSize: 11,
                              color: Colors.orange.shade700,
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                      ],
                    ),
                  ),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      _badge(
                        statusLabel,
                        statusColor,
                        isLunas
                            ? Icons.check_circle_outline
                            : Icons.warning_amber_outlined,
                      ),
                      const SizedBox(height: 4),
                      if (metode.isNotEmpty)
                        _badge(
                          metode,
                          Colors.blue.shade600,
                          Icons.payment_outlined,
                        ),
                    ],
                  ),
                ],
              ),
            ],
          ),
        ),
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
        Icon(
          Icons.receipt_long_outlined,
          size: 64,
          color: Colors.grey.shade300,
        ),
        const SizedBox(height: 14),
        Text(
          'Belum ada transaksi',
          style: TextStyle(color: Colors.grey.shade500, fontSize: 15),
        ),
        const SizedBox(height: 6),
        Text(
          'Tekan + untuk penjualan baru',
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
        ),
      ],
    ),
  );
}
