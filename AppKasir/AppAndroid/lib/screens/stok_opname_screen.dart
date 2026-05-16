import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../utils/safe_convert.dart';
import 'kategori_merk_screen.dart';

class StokOpnameScreen extends StatefulWidget {
  const StokOpnameScreen({super.key});
  @override
  State<StokOpnameScreen> createState() => _StokOpnameScreenState();
}

class _StokOpnameScreenState extends State<StokOpnameScreen> {
  static const _green = Color(0xFF16A34A);

  final _searchCtrl = TextEditingController();
  final _searchFocus = FocusNode();
  final _scrollCtrl = ScrollController();

  final List<Map<String, dynamic>> _items = [];
  List<Map<String, dynamic>> _suggest = [];
  List<Map<String, dynamic>> _categories = [];
  List<Map<String, dynamic>> _merks = [];

  bool _isLoading = false;
  bool _isScanning = false;
  bool _showSuggest = false;
  Timer? _debounce;

  // Tanggal transaksi — null = gunakan DateTime.now() saat simpan
  DateTime? _selectedDate;

  @override
  void initState() {
    super.initState();
    _loadMasterData();
    _searchFocus.addListener(() {
      if (!_searchFocus.hasFocus) {
        setState(() => _showSuggest = false);
      }
    });
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _searchCtrl.dispose();
    _searchFocus.dispose();
    _stokNyataCtrl.dispose();
    _stokNyataFocus.dispose();
    _scrollCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadMasterData() async {
    try {
      final r1 = await ApiService.getKategori();
      final r2 = await ApiService.getMerk();
      if (mounted) {
        setState(() {
          if (r1['status'] == 'success') {
            _categories = List<Map<String, dynamic>>.from(r1['data'] ?? []);
          }
          if (r2['status'] == 'success') {
            _merks = List<Map<String, dynamic>>.from(r2['data'] ?? []);
          }
        });
      }
    } catch (_) {}
  }

  void _onSearchChanged(String q) {
    _debounce?.cancel();
    if (q.trim().length < 2) {
      setState(() {
        _suggest = [];
        _showSuggest = false;
      });
      return;
    }
    _debounce = Timer(
      const Duration(milliseconds: 350),
      () => _runSearch(q.trim()),
    );
  }

  Future<void> _runSearch(String q) async {
    try {
      final res = await ApiService.getStock(search: q, limit: 50);
      if (!mounted) return;
      if (res['status'] == 'success') {
        setState(() {
          _suggest = List<Map<String, dynamic>>.from(res['data'] ?? []);
          _showSuggest = _suggest.isNotEmpty;
        });
      }
    } catch (e) {
      debugPrint('[StokOpname] ❌ search error: $e');
    }
  }

  void _selectProduct(Map<String, dynamic> p) {
    final lokasi =
        Provider.of<AuthProvider>(context, listen: false).selectedLocation ??
        'TOKO';
    final stokKey = lokasi == 'GUDANG' ? 'STOK_GUDANG' : 'STOK_TOKO';
    final stok = safeToDouble(p[stokKey]);

    // 1 transaksi = 1 item — ganti item yang ada (bukan tambah)
    setState(() {
      _items.clear();
      _items.add({
        'ID_BARANG': p['ID_BARANG'],
        'NAMA_BARANG': p['NAMA_BARANG'],
        'NAMA_KATEGORI': p['Kategori'] ?? '',
        'NAMA_MERK': p['Merk'] ?? '',
        'HARGA_BELI': safeToDouble(p['HARGA_BELI']),
        'STOK_SYSTEM': stok,
        'stok_nyata': stok,
        'stok_selisih': 0.0,
        'SATUAN': p['SATUAN'] ?? 'PCS',
        'ISI_SATUAN': p['ISI_SATUAN'] ?? 1,
        'total_qty': stok,
        'total_harga': 0.0,
        'keterangan': '',
      });
    });
    // Sync controller stok nyata — kosongkan agar hint tampil (hint = stok sistem)
    _stokNyataCtrl.clear();
    _clearSearch();
    // Fokus ke stok nyata setelah build selesai
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _stokNyataFocus.requestFocus();
    });
  }

  void _clearSearch() {
    _searchCtrl.clear();
    _searchFocus.unfocus();
    setState(() {
      _suggest = [];
      _showSuggest = false;
    });
  }

  // ── Controller stok nyata — dibuat sekali, bukan di dalam build ──
  final _stokNyataCtrl = TextEditingController();
  final _stokNyataFocus = FocusNode();

  void _setStokNyata(int index, double val) {
    if (val < 0) return;
    final stokSys = safeToDouble(_items[index]['STOK_SYSTEM']);
    final harga = safeToDouble(_items[index]['HARGA_BELI']);
    final selisih = val - stokSys;
    setState(() {
      _items[index]['stok_nyata'] = val;
      _items[index]['stok_selisih'] = selisih;
      _items[index]['total_qty'] = val;
      _items[index]['total_harga'] = selisih * harga;
    });
  }

  void _removeItem(int i) {
    final nama = _items[i]['NAMA_BARANG'];
    setState(() => _items.removeAt(i));
    _showSnack('$nama dihapus', Colors.grey.shade700);
  }

  void _updateKategori(int i, String v) {
    setState(() => _items[i]['NAMA_KATEGORI'] = v);
    ApiService.updateProductCategoryMerk(_items[i]['ID_BARANG'], v, null);
  }

  void _updateMerk(int i, String v) {
    setState(() => _items[i]['NAMA_MERK'] = v);
    ApiService.updateProductCategoryMerk(_items[i]['ID_BARANG'], null, v);
  }

  void _onBarcodeDetect(BarcodeCapture cap) {
    final raw = cap.barcodes.firstOrNull?.rawValue;
    if (raw == null) return;
    _stopScanner();
    _searchCtrl.text = raw;
    _runSearch(raw);
    _searchFocus.requestFocus();
  }

  void _startScanner() => setState(() => _isScanning = true);
  void _stopScanner() => setState(() => _isScanning = false);

  Future<void> _pickDate(bool izinkan) async {
    if (!izinkan) return;
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: context,
      initialDate: _selectedDate ?? now,
      firstDate: DateTime(now.year - 1),
      lastDate: now,
      locale: const Locale('id', 'ID'),
      builder: (ctx, child) => Theme(
        data: Theme.of(ctx).copyWith(
          colorScheme: const ColorScheme.light(
            primary: _green,
            onPrimary: Colors.white,
          ),
        ),
        child: child!,
      ),
    );
    if (picked != null) {
      setState(() => _selectedDate = picked);
    }
  }

  Future<void> _save() async {
    if (_items.isEmpty) {
      _showSnack('Belum ada barang', Colors.red);
      return;
    }
    for (final it in _items) {
      if (safeToDouble(it['stok_nyata']) < 0) {
        _showSnack(
          'Stok nyata "${it['NAMA_BARANG']}" tidak boleh negatif',
          Colors.red,
        );
        return;
      }
    }
    final auth = Provider.of<AuthProvider>(context, listen: false);
    if (auth.selectedLocation == null) {
      _showSnack('Lokasi belum dipilih', Colors.red);
      return;
    }

    setState(() => _isLoading = true);
    try {
      final tglSimpan = _selectedDate != null
          ? DateTime(
              _selectedDate!.year,
              _selectedDate!.month,
              _selectedDate!.day,
              DateTime.now().hour,
              DateTime.now().minute,
              DateTime.now().second,
            )
          : DateTime.now();

      debugPrint('[StokOpname] 💾 _save() mulai — ${_items.length} item');
      debugPrint('[StokOpname]    lokasi     : ${auth.selectedLocation}');
      debugPrint(
        '[StokOpname]    id_user    : ${auth.userFullName.isNotEmpty ? auth.userFullName : auth.userName}',
      );
      debugPrint('[StokOpname]    id_komputer: ${auth.deviceName}');
      debugPrint('[StokOpname]    tgl        : ${tglSimpan.toIso8601String()}');
      for (final it in _items) {
        debugPrint(
          '[StokOpname]    item: ${it['NAMA_BARANG']} | sistem=${it['STOK_SYSTEM']} | nyata=${it['stok_nyata']} | selisih=${it['stok_selisih']}',
        );
      }

      final res = await ApiService.syncStokOpname({
        'tgl_transaksi': tglSimpan.toIso8601String(),
        'lokasi': auth.selectedLocation,
        'id_user': auth.userFullName.isNotEmpty
            ? auth.userFullName
            : auth.userName,
        'id_komputer': auth.deviceName,
        'items': _items
            .map(
              (it) => {
                'id_barang': it['ID_BARANG'],
                'nama_barang': it['NAMA_BARANG'],
                'kategori': it['NAMA_KATEGORI'] ?? '',
                'merk': it['NAMA_MERK'] ?? '',
                'harga': it['HARGA_BELI'] ?? 0,
                'stok_system': it['STOK_SYSTEM'] ?? 0,
                'stok_nyata': it['stok_nyata'] ?? 0,
                'stok_selisih': it['stok_selisih'] ?? 0,
                'satuan': it['SATUAN'] ?? 'PCS',
                'isi_satuan': it['ISI_SATUAN'] ?? 1,
                'total_qty': it['total_qty'] ?? 0,
                'total_harga': it['total_harga'] ?? 0,
                'keterangan': it['keterangan'] ?? '',
              },
            )
            .toList(),
      });

      debugPrint('[StokOpname]    response status : ${res['status']}');
      debugPrint('[StokOpname]    response message: ${res['message']}');
      debugPrint('[StokOpname]    id_so           : ${res['id_so']}');

      if (res['status'] == 'success') {
        debugPrint('[StokOpname] ✅ Berhasil: ${res['id_so']}');
        setState(() => _items.clear());
        _showSnack('Opname ${res['id_so'] ?? ''} berhasil disimpan', _green);
      } else {
        debugPrint('[StokOpname] ❌ Gagal: ${res['message']}');
        _showSnack(res['message'] ?? 'Gagal menyimpan', Colors.red);
      }
    } catch (e, st) {
      debugPrint('[StokOpname] ❌ Exception: $e');
      debugPrint('[StokOpname]    StackTrace: $st');
      _showSnack('Error: $e', Colors.red);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  void _showSnack(String msg, Color color) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(msg),
        backgroundColor: color,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        duration: const Duration(seconds: 2),
      ),
    );
  }

  // ══════════════════════════════════════════════════════════════
  @override
  Widget build(BuildContext context) {
    if (_isScanning) return _buildScanner();

    final auth = Provider.of<AuthProvider>(context);
    final izinkanTglLampau = auth.hakAkses.izinkanTanggalLampau;
    final tglLabel = _selectedDate != null
        ? '${_selectedDate!.day.toString().padLeft(2, '0')}/'
              '${_selectedDate!.month.toString().padLeft(2, '0')}/'
              '${_selectedDate!.year}'
        : null;

    return Scaffold(
      resizeToAvoidBottomInset: true,
      backgroundColor: const Color(0xFFF0FDF4),
      appBar: AppBar(
        backgroundColor: _green,
        foregroundColor: Colors.white,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          tooltip: 'Kembali',
          onPressed: () => Navigator.of(context).maybePop(),
        ),
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              _items.isEmpty ? 'Stok Opname' : 'Stok Opname (${_items.length})',
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            ),
            if (tglLabel != null)
              Text(
                tglLabel,
                style: TextStyle(
                  fontSize: 11,
                  fontWeight: FontWeight.normal,
                  color: Colors.white.withValues(alpha: 0.85),
                ),
              ),
          ],
        ),
        actions: [
          // Icon tanggal — aktif hanya jika izinkan_tanggal_lampau = true
          IconButton(
            icon: Icon(
              Icons.calendar_today_outlined,
              color: izinkanTglLampau
                  ? Colors.white
                  : Colors.white.withValues(alpha: 0.3),
            ),
            tooltip: izinkanTglLampau
                ? 'Pilih tanggal transaksi'
                : 'Tanggal lampau tidak diizinkan',
            onPressed: izinkanTglLampau ? () => _pickDate(true) : null,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _green))
          : Column(
              children: [
                _buildSearchBar(),
                // Autocomplete — Flexible agar tidak overflow keyboard
                if (_showSuggest) Flexible(child: _buildSuggestList()),
                // Item tunggal — SingleChildScrollView + resizeToAvoidBottomInset
                // agar konten naik saat keyboard muncul
                if (_items.isNotEmpty)
                  Expanded(
                    child: SingleChildScrollView(
                      controller: _scrollCtrl,
                      // padding bawah besar agar konten tidak tertutup keyboard
                      padding: EdgeInsets.fromLTRB(
                        14,
                        10,
                        14,
                        MediaQuery.of(context).viewInsets.bottom + 100,
                      ),
                      keyboardDismissBehavior:
                          ScrollViewKeyboardDismissBehavior.onDrag,
                      child: _buildItemCard(0),
                    ),
                  )
                else if (!_showSuggest)
                  Expanded(child: _buildEmpty()),
              ],
            ),
      // ── Tombol simpan sticky di bawah ─────────────────────────
      bottomNavigationBar: _items.isEmpty
          ? null
          : SafeArea(
              child: Container(
                padding: const EdgeInsets.fromLTRB(16, 8, 16, 12),
                decoration: BoxDecoration(
                  color: Colors.white,
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.06),
                      blurRadius: 8,
                      offset: const Offset(0, -2),
                    ),
                  ],
                ),
                child: SizedBox(
                  height: 52,
                  child: ElevatedButton.icon(
                    onPressed: _isLoading ? null : _save,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: _green,
                      foregroundColor: Colors.white,
                      disabledBackgroundColor: _green.withValues(alpha: 0.5),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                      elevation: 0,
                    ),
                    icon: _isLoading
                        ? const SizedBox(
                            width: 18,
                            height: 18,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : const Icon(Icons.save_outlined),
                    label: Text(
                      _isLoading
                          ? 'Menyimpan...'
                          : 'Simpan ${_items.length} Item',
                      style: const TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ),
              ),
            ),
    );
  }

  // ── Search bar ────────────────────────────────────────────────
  Widget _buildSearchBar() {
    return Container(
      color: _green,
      padding: const EdgeInsets.fromLTRB(12, 8, 12, 12),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _searchCtrl,
              focusNode: _searchFocus,
              onChanged: _onSearchChanged,
              onSubmitted: (q) {
                if (q.trim().length >= 2) _runSearch(q.trim());
              },
              style: const TextStyle(fontSize: 14, color: Color(0xFF1E293B)),
              decoration: InputDecoration(
                hintText: 'Nama barang, barcode, atau kategori...',
                hintStyle: TextStyle(color: Colors.grey.shade400, fontSize: 13),
                prefixIcon: Icon(
                  Icons.search,
                  size: 20,
                  color: Colors.grey.shade500,
                ),
                suffixIcon: _searchCtrl.text.isNotEmpty
                    ? IconButton(
                        icon: Icon(
                          Icons.clear,
                          size: 18,
                          color: Colors.grey.shade500,
                        ),
                        onPressed: _clearSearch,
                      )
                    : null,
                filled: true,
                fillColor: Colors.white,
                contentPadding: const EdgeInsets.symmetric(vertical: 11),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide: BorderSide.none,
                ),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide: BorderSide.none,
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide: const BorderSide(
                    color: Color(0xFF86EFAC),
                    width: 2,
                  ),
                ),
              ),
            ),
          ),
          const SizedBox(width: 8),
          Tooltip(
            message: 'Scan Barcode',
            child: Material(
              color: const Color(0xFF15803D),
              borderRadius: BorderRadius.circular(10),
              child: InkWell(
                onTap: _startScanner,
                borderRadius: BorderRadius.circular(10),
                child: const Padding(
                  padding: EdgeInsets.all(11),
                  child: Icon(
                    Icons.qr_code_scanner,
                    color: Colors.white,
                    size: 22,
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  // ── Autocomplete ──────────────────────────────────────────────
  Widget _buildSuggestList() {
    final auth = Provider.of<AuthProvider>(context, listen: false);
    final lokasi = auth.selectedLocation ?? 'TOKO';
    final stokKey = lokasi == 'GUDANG' ? 'STOK_GUDANG' : 'STOK_TOKO';
    final fmtHarga = NumberFormat('#,##0', 'id_ID');

    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.08),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: ListView.separated(
        shrinkWrap: true,
        padding: EdgeInsets.zero,
        itemCount: _suggest.length,
        separatorBuilder: (ctx2, idx) => const Divider(height: 1),
        itemBuilder: (_, i) {
          final p = _suggest[i];
          final stok = safeToDouble(p[stokKey]);
          final kat = p['Kategori']?.toString() ?? '';
          final harga = safeToDouble(p['HARGA_BELI']);
          return ListTile(
            dense: true,
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 14,
              vertical: 2,
            ),
            leading: Container(
              width: 36,
              height: 36,
              decoration: BoxDecoration(
                color: _green.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Icon(Icons.inventory_2_outlined, size: 18, color: _green),
            ),
            title: Text(
              p['NAMA_BARANG'] ?? '',
              style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
            ),
            subtitle: Row(
              children: [
                if (kat.isNotEmpty) ...[
                  Text(
                    kat,
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                  ),
                  Text(
                    '  ·  ',
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade400),
                  ),
                ],
                Text(
                  'Rp ${fmtHarga.format(harga)}',
                  style: TextStyle(
                    fontSize: 11,
                    color: Colors.grey.shade600,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ],
            ),
            trailing: Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
              decoration: BoxDecoration(
                color: stok <= 0 ? Colors.red.shade50 : Colors.green.shade50,
                borderRadius: BorderRadius.circular(6),
              ),
              child: Text(
                stok.toStringAsFixed(0),
                style: TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.bold,
                  color: stok <= 0 ? Colors.red : Colors.green.shade700,
                ),
              ),
            ),
            onTap: () => _selectProduct(p),
          );
        },
      ),
    );
  }

  // ── Empty state ───────────────────────────────────────────────
  Widget _buildEmpty() {
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            padding: const EdgeInsets.all(20),
            decoration: BoxDecoration(
              color: _green.withValues(alpha: 0.08),
              shape: BoxShape.circle,
            ),
            child: Icon(
              Icons.inventory_2_outlined,
              size: 40,
              color: _green.withValues(alpha: 0.5),
            ),
          ),
          const SizedBox(height: 14),
          const Text(
            'Cari barang untuk mulai opname',
            style: TextStyle(
              color: Color(0xFF6B7280),
              fontSize: 14,
              fontWeight: FontWeight.w500,
            ),
          ),
          const SizedBox(height: 4),
          const Text(
            'Ketik nama, barcode, atau kategori di kolom pencarian',
            style: TextStyle(color: Color(0xFF9CA3AF), fontSize: 12),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }

  // ── Item card — angka besar, satuan, keterangan ───────────────
  Widget _buildItemCard(int i) {
    final item = _items[i];
    final stokSys = safeToDouble(item['STOK_SYSTEM']);
    final stokNyata = safeToDouble(item['stok_nyata']);
    final selisih = safeToDouble(item['stok_selisih']);
    final satuan = item['SATUAN']?.toString() ?? 'PCS';

    final selisihColor = selisih == 0
        ? Colors.grey.shade500
        : selisih > 0
        ? Colors.green.shade700
        : Colors.red.shade700;

    final katList = _categories
        .map((c) => c['nama']?.toString() ?? '')
        .where((s) => s.isNotEmpty)
        .toList();
    final merkList = _merks
        .map((m) => m['nama']?.toString() ?? '')
        .where((s) => s.isNotEmpty)
        .toList();

    return Container(
      margin: const EdgeInsets.only(bottom: 12),
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
            // ── Nama + hapus ───────────────────────────────────
            Row(
              children: [
                Expanded(
                  child: Text(
                    item['NAMA_BARANG'] ?? '',
                    style: const TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFF1E293B),
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
                GestureDetector(
                  onTap: () => _removeItem(i),
                  child: Container(
                    padding: const EdgeInsets.all(4),
                    decoration: BoxDecoration(
                      color: Colors.red.shade50,
                      shape: BoxShape.circle,
                    ),
                    child: Icon(
                      Icons.close,
                      size: 16,
                      color: Colors.red.shade400,
                    ),
                  ),
                ),
              ],
            ),
            Text(
              item['ID_BARANG'] ?? '',
              style: TextStyle(fontSize: 11, color: Colors.grey.shade400),
            ),
            // ── Harga beli ─────────────────────────────────────
            Builder(
              builder: (_) {
                final harga = safeToDouble(item['HARGA_BELI']);
                final fmt = NumberFormat('#,##0', 'id_ID');
                return Text(
                  'Harga Beli: Rp ${fmt.format(harga)}',
                  style: TextStyle(
                    fontSize: 11,
                    color: Colors.grey.shade600,
                    fontWeight: FontWeight.w500,
                  ),
                );
              },
            ),
            const SizedBox(height: 14),

            // ── Stok sistem | Input stok nyata | Selisih ───────
            Row(
              children: [
                // Stok sistem
                Expanded(
                  child: _bigStatBox(
                    label: 'Stok Sistem',
                    value: stokSys.toStringAsFixed(0),
                    satuan: satuan,
                    color: const Color(0xFF2563EB),
                  ),
                ),
                const SizedBox(width: 10),
                // Input stok nyata
                Expanded(child: _stokNyataBox(i, stokNyata, satuan)),
                const SizedBox(width: 10),
                // Selisih
                Expanded(
                  child: _bigStatBox(
                    label: 'Selisih',
                    value:
                        '${selisih >= 0 ? '+' : ''}${selisih.toStringAsFixed(0)}',
                    satuan: satuan,
                    color: selisihColor,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),

            // ── Selisih Rupiah ─────────────────────────────────
            Builder(
              builder: (_) {
                final rupiah = safeToDouble(item['total_harga']);
                final rupiahColor = rupiah == 0
                    ? Colors.grey.shade500
                    : rupiah > 0
                    ? Colors.green.shade700
                    : Colors.red.shade700;
                final fmt = NumberFormat('#,##0', 'id_ID');
                return Container(
                  width: double.infinity,
                  padding: const EdgeInsets.symmetric(
                    vertical: 8,
                    horizontal: 12,
                  ),
                  decoration: BoxDecoration(
                    color: rupiahColor.withValues(alpha: 0.06),
                    borderRadius: BorderRadius.circular(10),
                    border: Border.all(
                      color: rupiahColor.withValues(alpha: 0.2),
                    ),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Selisih Rupiah',
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey.shade600,
                        ),
                      ),
                      Text(
                        rupiah == 0
                            ? '—'
                            : '${rupiah >= 0 ? '+' : ''}Rp ${fmt.format(rupiah)}',
                        style: TextStyle(
                          fontSize: 13,
                          fontWeight: FontWeight.bold,
                          color: rupiahColor,
                        ),
                      ),
                    ],
                  ),
                );
              },
            ),
            const SizedBox(height: 10),

            // ── Keterangan ─────────────────────────────────────
            TextField(
              onChanged: (v) => _items[i]['keterangan'] = v,
              style: const TextStyle(fontSize: 13),
              decoration: InputDecoration(
                hintText: 'Keterangan (opsional)',
                hintStyle: TextStyle(fontSize: 12, color: Colors.grey.shade400),
                prefixIcon: Icon(
                  Icons.notes,
                  size: 16,
                  color: Colors.grey.shade400,
                ),
                isDense: true,
                contentPadding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 10,
                ),
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
            const SizedBox(height: 10),

            // ── Kategori & Merk ────────────────────────────────
            Row(
              children: [
                Expanded(
                  child: _dropdown(
                    label: 'Kategori',
                    value: item['NAMA_KATEGORI']?.toString() ?? '',
                    items: katList,
                    onChanged: (v) => _updateKategori(i, v),
                    type: 'kategori',
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: _dropdown(
                    label: 'Merk',
                    value: item['NAMA_MERK']?.toString() ?? '',
                    items: merkList,
                    onChanged: (v) => _updateMerk(i, v),
                    type: 'merk',
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  // ── Stat box besar ────────────────────────────────────────────
  Widget _bigStatBox({
    required String label,
    required String value,
    required String satuan,
    required Color color,
  }) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 8),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.06),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withValues(alpha: 0.2)),
      ),
      child: Column(
        children: [
          Text(
            label,
            style: TextStyle(fontSize: 10, color: Colors.grey.shade500),
          ),
          const SizedBox(height: 4),
          Text(
            value,
            style: TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: color,
            ),
          ),
          Text(
            satuan,
            style: TextStyle(fontSize: 10, color: color.withValues(alpha: 0.7)),
          ),
        ],
      ),
    );
  }

  // ── Input stok nyata — controller persisten, hint = stok sistem ─────────
  Widget _stokNyataBox(int index, double current, String satuan) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 8),
      decoration: BoxDecoration(
        color: _green.withValues(alpha: 0.06),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: _green.withValues(alpha: 0.3)),
      ),
      child: Column(
        children: [
          Text(
            'Stok Nyata',
            style: TextStyle(fontSize: 10, color: Colors.grey.shade500),
          ),
          const SizedBox(height: 4),
          TextField(
            controller: _stokNyataCtrl,
            focusNode: _stokNyataFocus,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            inputFormatters: [
              FilteringTextInputFormatter.allow(RegExp(r'^\d*\.?\d*')),
            ],
            textAlign: TextAlign.center,
            style: const TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: _green,
            ),
            decoration: InputDecoration(
              isDense: true,
              contentPadding: const EdgeInsets.symmetric(vertical: 2),
              border: InputBorder.none,
              hintText: current.toStringAsFixed(0),
              hintStyle: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
                color: _green.withValues(alpha: 0.35),
              ),
            ),
            onChanged: (v) {
              final d = double.tryParse(v);
              // Jika kosong → anggap sama dengan stok sistem (selisih 0)
              _setStokNyata(
                index,
                d ?? safeToDouble(_items[index]['STOK_SYSTEM']),
              );
            },
          ),
          Text(
            satuan,
            style: TextStyle(
              fontSize: 10,
              color: _green.withValues(alpha: 0.7),
            ),
          ),
        ],
      ),
    );
  }

  Widget _dropdown({
    required String label,
    required String value,
    required List<String> items,
    required void Function(String) onChanged,
    required String type, // 'kategori' atau 'merk'
  }) {
    final allItems = items.contains(value) || value.isEmpty
        ? items
        : [value, ...items];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: TextStyle(fontSize: 10, color: Colors.grey.shade500),
        ),
        const SizedBox(height: 3),
        Row(
          children: [
            Expanded(
              child: DropdownButtonFormField<String>(
                // ignore: deprecated_member_use
                value: value.isEmpty ? null : value,
                isExpanded: true,
                isDense: true,
                hint: Text(
                  'Pilih $label',
                  style: TextStyle(fontSize: 12, color: Colors.grey.shade400),
                ),
                decoration: InputDecoration(
                  isDense: true,
                  contentPadding: const EdgeInsets.symmetric(
                    horizontal: 10,
                    vertical: 8,
                  ),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(8),
                    borderSide: BorderSide(color: Colors.grey.shade300),
                  ),
                  enabledBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(8),
                    borderSide: BorderSide(color: Colors.grey.shade300),
                  ),
                ),
                style: const TextStyle(fontSize: 12, color: Colors.black87),
                items: allItems
                    .map(
                      (s) => DropdownMenuItem(
                        value: s,
                        child: Text(s, overflow: TextOverflow.ellipsis),
                      ),
                    )
                    .toList(),
                onChanged: (v) {
                  if (v != null) onChanged(v);
                },
              ),
            ),
            const SizedBox(width: 4),
            // Tombol pensil → halaman CRUD kategori/merk
            GestureDetector(
              onTap: () => Navigator.of(context)
                  .push(
                    MaterialPageRoute(
                      builder: (_) => KategoriMerkScreen(initialTab: type),
                    ),
                  )
                  .then((_) => _loadMasterData()),
              child: Container(
                padding: const EdgeInsets.all(7),
                decoration: BoxDecoration(
                  color: _green.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: _green.withValues(alpha: 0.3)),
                ),
                child: Icon(Icons.edit_outlined, size: 16, color: _green),
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildScanner() {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Scan Barcode'),
        leading: IconButton(
          onPressed: _stopScanner,
          icon: const Icon(Icons.close),
        ),
      ),
      body: MobileScanner(onDetect: _onBarcodeDetect),
    );
  }
}
