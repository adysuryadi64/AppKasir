import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../widgets/app_drawer.dart';
import 'stok_opname_screen.dart';

class OpnameListScreen extends StatefulWidget {
  const OpnameListScreen({super.key});
  @override
  State<OpnameListScreen> createState() => _OpnameListScreenState();
}

class _OpnameListScreenState extends State<OpnameListScreen> {
  static const _green = Color(0xFF16A34A);

  List<Map<String, dynamic>> _list = [];
  bool _isLoading = false;
  bool _hasError = false;

  // Filter tanggal — default hari ini saja
  late DateTime _tglDari;
  late DateTime _tglSampai;

  final _fmtNum = NumberFormat('#,##0.##', 'id_ID');
  final _fmtFilter = DateFormat('dd MMM yyyy');

  String _formatTgl(String raw) {
    try {
      return DateFormat(
        'dd MMM yyyy  HH:mm',
        'id_ID',
      ).format(DateTime.parse(raw));
    } catch (_) {
      return raw;
    }
  }

  @override
  void initState() {
    super.initState();
    _tglSampai = DateTime.now();
    _tglDari = DateTime(
      _tglSampai.year,
      _tglSampai.month,
      _tglSampai.day,
    ); // hari ini
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _hasError = false;
    });
    try {
      final lokasi =
          Provider.of<AuthProvider>(context, listen: false).selectedLocation ??
          '';
      final res = await ApiService.getOpnameList(
        lokasi: lokasi,
        tglDari: DateFormat('yyyy-MM-dd').format(_tglDari),
        tglSampai: DateFormat('yyyy-MM-dd').format(_tglSampai),
        limit: 50,
      );
      if (res['status'] == 'success') {
        setState(
          () => _list = List<Map<String, dynamic>>.from(res['data'] ?? []),
        );
      } else {
        setState(() => _hasError = true);
      }
    } catch (e) {
      debugPrint('[OpnameList] ❌ load error: $e');
      setState(() => _hasError = true);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  void _openNew() {
    Navigator.of(context)
        .push(MaterialPageRoute(builder: (_) => const StokOpnameScreen()))
        .then((_) => _load());
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
      _load();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: const AppDrawer(),
      backgroundColor: const Color(0xFFF0FDF4),
      appBar: AppBar(
        title: const Text('Stok Opname'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _load,
            tooltip: 'Refresh',
          ),
        ],
      ),
      body: Column(
        children: [
          // ── Filter tanggal ─────────────────────────────────────
          _buildFilterBar(),
          // ── Konten ────────────────────────────────────────────
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator(color: _green))
                : _hasError
                ? _buildError()
                : _list.isEmpty
                ? _buildEmpty()
                : RefreshIndicator(
                    onRefresh: _load,
                    color: _green,
                    child: ListView.builder(
                      padding: const EdgeInsets.fromLTRB(14, 14, 14, 90),
                      itemCount: _list.length,
                      itemBuilder: (_, i) => _buildCard(_list[i]),
                    ),
                  ),
          ), // Expanded
        ],
      ), // Column
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _openNew,
        icon: const Icon(Icons.add),
        label: const Text(
          'Opname Baru',
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
          // Shortcut: 7 hari / 30 hari / bulan ini
          _quickFilter('7H', 7),
          const SizedBox(width: 4),
          _quickFilter('30H', 30),
          const SizedBox(width: 4),
          _quickFilter('Bln', -1),
        ],
      ),
    );
  }

  Widget _quickFilter(String label, int days) {
    return GestureDetector(
      onTap: () {
        setState(() {
          _tglSampai = DateTime.now();
          if (days == -1) {
            // Bulan ini
            _tglDari = DateTime(DateTime.now().year, DateTime.now().month, 1);
          } else {
            _tglDari = _tglSampai.subtract(Duration(days: days));
          }
        });
        _load();
      },
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
  }

  Widget _buildCard(Map<String, dynamic> item) {
    final noSo = item['ID_STOK_OPNAME']?.toString() ?? '—';
    final tglRaw = item['TANGGAL']?.toString() ?? '';
    final lokasi = item['LOKASI']?.toString() ?? '';
    final user = item['ID_USER']?.toString() ?? '';
    final namaBarang = item['NAMA_BARANG']?.toString() ?? '—';
    final satuan = item['SATUAN']?.toString() ?? '';

    // Parse angka — PDO bisa return String
    final stokSystem =
        double.tryParse(item['STOK_SYSTEM']?.toString() ?? '0') ?? 0.0;
    final stokNyata =
        double.tryParse(item['STOK_NYATA']?.toString() ?? '0') ?? 0.0;
    final selisih =
        double.tryParse(item['STOK_SELISIH']?.toString() ?? '0') ?? 0.0;
    final rupiah =
        double.tryParse(item['TOTAL_RUPIAH']?.toString() ?? '0') ?? 0.0;

    final tglFmt = _formatTgl(tglRaw);

    final isToko = lokasi != 'GUDANG';
    final lokasiColor = isToko ? _green : const Color(0xFF0D9488);

    final selisihPositif = selisih > 0;
    final selisihNol = selisih == 0;
    final selisihColor = selisihNol
        ? Colors.grey.shade500
        : selisihPositif
        ? Colors.green.shade700
        : Colors.red.shade700;

    return Container(
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
            // ── Baris 1: No SO + badge lokasi ─────────────────
            Row(
              children: [
                Expanded(
                  child: Text(
                    noSo,
                    style: const TextStyle(
                      fontSize: 13,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFF1E293B),
                    ),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 8,
                    vertical: 3,
                  ),
                  decoration: BoxDecoration(
                    color: lokasiColor.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(
                      color: lokasiColor.withValues(alpha: 0.3),
                    ),
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(
                        isToko
                            ? Icons.storefront_outlined
                            : Icons.warehouse_outlined,
                        size: 11,
                        color: lokasiColor,
                      ),
                      const SizedBox(width: 3),
                      Text(
                        lokasi,
                        style: TextStyle(
                          fontSize: 10,
                          fontWeight: FontWeight.w600,
                          color: lokasiColor,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 4),

            // ── Baris 2: Tanggal + user ────────────────────────
            Row(
              children: [
                Icon(Icons.access_time, size: 11, color: Colors.grey.shade400),
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
            const SizedBox(height: 10),
            const Divider(height: 1),
            const SizedBox(height: 8),

            // ── Nama barang ────────────────────────────────────
            Text(
              namaBarang,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w700,
                color: Color(0xFF1E293B),
              ),
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
            ),
            const SizedBox(height: 8),

            // ── 4 kolom: Sistem | Nyata | Selisih | Rupiah ─────
            Row(
              children: [
                Expanded(
                  child: _numCol(
                    'Sistem',
                    '${_fmtNum.format(stokSystem)} $satuan',
                    Colors.blue.shade700,
                  ),
                ),
                Expanded(
                  child: _numCol(
                    'Nyata',
                    '${_fmtNum.format(stokNyata)} $satuan',
                    _green,
                  ),
                ),
                Expanded(
                  child: _numCol(
                    'Selisih',
                    '${selisih >= 0 ? '+' : ''}${_fmtNum.format(selisih)}',
                    selisihColor,
                  ),
                ),
                Expanded(
                  child: _numCol(
                    'Rupiah',
                    rupiah == 0 ? '—' : _fmtNum.format(rupiah),
                    rupiah == 0 ? Colors.grey.shade400 : selisihColor,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _numCol(String label, String value, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: TextStyle(fontSize: 9, color: Colors.grey.shade500)),
        const SizedBox(height: 2),
        Text(
          value,
          style: TextStyle(
            fontSize: 12,
            fontWeight: FontWeight.bold,
            color: color,
          ),
          overflow: TextOverflow.ellipsis,
        ),
      ],
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
          const SizedBox(height: 14),
          Text(
            'Belum ada data opname',
            style: TextStyle(color: Colors.grey.shade500, fontSize: 15),
          ),
          const SizedBox(height: 6),
          Text(
            'Tekan + untuk membuat opname baru',
            style: TextStyle(color: Colors.grey.shade400, fontSize: 12),
          ),
        ],
      ),
    );
  }

  Widget _buildError() {
    return Center(
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
            onPressed: _load,
            icon: const Icon(Icons.refresh),
            label: const Text('Coba Lagi'),
          ),
        ],
      ),
    );
  }
}
