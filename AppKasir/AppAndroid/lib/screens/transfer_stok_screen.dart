import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../utils/safe_convert.dart';

// ── Model satuan barang ──────────────────────────────────────────────────────
class _SatuanItem {
  final String nama;
  final int isi;
  const _SatuanItem(this.nama, this.isi);
}

class TransferStokScreen extends StatefulWidget {
  const TransferStokScreen({super.key});
  @override
  State<TransferStokScreen> createState() => _TransferStokScreenState();
}

class _TransferStokScreenState extends State<TransferStokScreen> {
  static const _green = Color(0xFF16A34A);
  static const _red = Color(0xFFDC2626);

  // ── Controller pencarian ─────────────────────────────────────────────────
  final _searchCtrlKlr = TextEditingController();
  final _searchFocusKlr = FocusNode();
  final _searchCtrlMsk = TextEditingController();
  final _searchFocusMsk = FocusNode();
  final _scrollCtrl = ScrollController();

  // ── Controller qty ───────────────────────────────────────────────────────
  final _qtyKlrCtrl = TextEditingController();
  final _qtyKlrFocus = FocusNode();
  final _qtyMskCtrl = TextEditingController();
  final _qtyMskFocus = FocusNode();

  // ── State ────────────────────────────────────────────────────────────────
  List<Map<String, dynamic>> _suggestKlr = [];
  List<Map<String, dynamic>> _suggestMsk = [];
  bool _isLoading = false;
  bool _isScanning = false;
  bool _scanForKlr = true; // true = scan untuk keluar, false = untuk masuk
  bool _showSuggestKlr = false;
  bool _showSuggestMsk = false;
  Timer? _debounceKlr;
  Timer? _debounceMsk;
  DateTime? _selectedDate;

  // ── Data barang terpilih ─────────────────────────────────────────────────
  Map<String, dynamic> _barangKlr = {};
  Map<String, dynamic> _barangMsk = {};

  // ── Daftar satuan per barang ─────────────────────────────────────────────
  List<_SatuanItem> _satuanListKlr = [];
  List<_SatuanItem> _satuanListMsk = [];
  _SatuanItem? _satuanKlr;
  _SatuanItem? _satuanMsk;

  @override
  void initState() {
    super.initState();
    _searchFocusKlr.addListener(() {
      if (!_searchFocusKlr.hasFocus) {
        Future.delayed(const Duration(milliseconds: 150), () {
          if (mounted) setState(() => _showSuggestKlr = false);
        });
      }
    });
    _searchFocusMsk.addListener(() {
      if (!_searchFocusMsk.hasFocus) {
        Future.delayed(const Duration(milliseconds: 150), () {
          if (mounted) setState(() => _showSuggestMsk = false);
        });
      }
    });
  }

  @override
  void dispose() {
    _debounceKlr?.cancel();
    _debounceMsk?.cancel();
    _searchCtrlKlr.dispose();
    _searchFocusKlr.dispose();
    _searchCtrlMsk.dispose();
    _searchFocusMsk.dispose();
    _qtyKlrCtrl.dispose();
    _qtyKlrFocus.dispose();
    _qtyMskCtrl.dispose();
    _qtyMskFocus.dispose();
    _scrollCtrl.dispose();
    super.dispose();
  }

  // ── Pencarian barang keluar ──────────────────────────────────────────────
  void _onSearchChangedKlr(String q) {
    _debounceKlr?.cancel();
    if (q.trim().length < 2) {
      setState(() {
        _suggestKlr = [];
        _showSuggestKlr = false;
      });
      return;
    }
    _debounceKlr = Timer(
      const Duration(milliseconds: 350),
      () => _runSearchKlr(q.trim()),
    );
  }

  Future<void> _runSearchKlr(String q) async {
    try {
      final res = await ApiService.getStock(search: q, limit: 50);
      if (!mounted) return;
      if (res['status'] == 'success') {
        setState(() {
          _suggestKlr = List<Map<String, dynamic>>.from(res['data'] ?? []);
          _showSuggestKlr = _suggestKlr.isNotEmpty;
        });
      }
    } catch (_) {}
  }

  // ── Pencarian barang masuk ───────────────────────────────────────────────
  void _onSearchChangedMsk(String q) {
    _debounceMsk?.cancel();
    if (q.trim().length < 2) {
      setState(() {
        _suggestMsk = [];
        _showSuggestMsk = false;
      });
      return;
    }
    _debounceMsk = Timer(
      const Duration(milliseconds: 350),
      () => _runSearchMsk(q.trim()),
    );
  }

  Future<void> _runSearchMsk(String q) async {
    try {
      final res = await ApiService.getStock(search: q, limit: 50);
      if (!mounted) return;
      if (res['status'] == 'success') {
        setState(() {
          _suggestMsk = List<Map<String, dynamic>>.from(res['data'] ?? []);
          _showSuggestMsk = _suggestMsk.isNotEmpty;
        });
      }
    } catch (e) {
      debugPrint('[TransferStok] ❌ searchMsk error: $e');
    }
  }

  // ── Bangun daftar satuan dari data API ───────────────────────────────────
  List<_SatuanItem> _buildSatuanList(Map<String, dynamic> p) {
    final list = <_SatuanItem>[];
    final s1 =
        p['SATUAN_UMUM_KECIL']?.toString() ?? p['SATUAN']?.toString() ?? '';
    final i1 = safeToInt(p['ISI_UMUM_KECIL'] ?? p['ISI_SATUAN'] ?? 1);
    final s2 = p['SATUAN_UMUM_SEDANG']?.toString() ?? '';
    final i2 = safeToInt(p['ISI_UMUM_SEDANG'] ?? 0);
    final s3 = p['SATUAN_UMUM_BESAR']?.toString() ?? '';
    final i3 = safeToInt(p['ISI_UMUM_BESAR'] ?? 0);
    if (s1.isNotEmpty) list.add(_SatuanItem(s1, i1 > 0 ? i1 : 1));
    if (s2.isNotEmpty && i2 > 0) list.add(_SatuanItem(s2, i2));
    if (s3.isNotEmpty && i3 > 0) list.add(_SatuanItem(s3, i3));
    if (list.isEmpty) list.add(const _SatuanItem('PCS', 1));
    return list;
  }

  // ── Pilih barang keluar ──────────────────────────────────────────────────
  void _selectProductKlr(Map<String, dynamic> p) {
    final lokasi =
        Provider.of<AuthProvider>(context, listen: false).selectedLocation ??
        'TOKO';
    final stokKey = lokasi == 'GUDANG' ? 'STOK_GUDANG' : 'STOK_TOKO';
    final stok = safeToDouble(p[stokKey]);
    final satuanList = _buildSatuanList(p);
    final satuanDefault = satuanList.first;

    setState(() {
      _satuanListKlr = satuanList;
      _satuanKlr = satuanDefault;
      _barangKlr = {
        'ID_BARANG': p['ID_BARANG'],
        'NAMA_BARANG': p['NAMA_BARANG'],
        'HARGA_BELI': safeToDouble(p['HARGA_BELI']),
        'STOK_SYSTEM': stok,
        'qty': 1.0,
        'SATUAN': satuanDefault.nama,
        'ISI_SATUAN': satuanDefault.isi,
        'total_qty': safeToDouble(satuanDefault.isi),
        'total_harga': safeToDouble(p['HARGA_BELI']) * satuanDefault.isi,
      };
    });
    _qtyKlrCtrl.clear();
    _clearSearchKlr();
    WidgetsBinding.instance.addPostFrameCallback(
      (_) => _qtyKlrFocus.requestFocus(),
    );
  }

  // ── Pilih barang masuk ───────────────────────────────────────────────────
  void _selectProductMsk(Map<String, dynamic> p) {
    final lokasi =
        Provider.of<AuthProvider>(context, listen: false).selectedLocation ??
        'TOKO';
    final stokKey = lokasi == 'GUDANG' ? 'STOK_GUDANG' : 'STOK_TOKO';
    final stok = safeToDouble(p[stokKey]);
    final satuanList = _buildSatuanList(p);
    final satuanDefault = satuanList.first;

    setState(() {
      _satuanListMsk = satuanList;
      _satuanMsk = satuanDefault;
      _barangMsk = {
        'ID_BARANG': p['ID_BARANG'],
        'NAMA_BARANG': p['NAMA_BARANG'],
        'HARGA_BELI': safeToDouble(p['HARGA_BELI']),
        'STOK_SYSTEM': stok,
        'qty': 1.0,
        'SATUAN': satuanDefault.nama,
        'ISI_SATUAN': satuanDefault.isi,
        'total_qty': safeToDouble(satuanDefault.isi),
        'total_harga': safeToDouble(p['HARGA_BELI']) * satuanDefault.isi,
      };
    });
    _qtyMskCtrl.clear();
    _clearSearchMsk();
    WidgetsBinding.instance.addPostFrameCallback(
      (_) => _qtyMskFocus.requestFocus(),
    );
  }

  // ── Hitung ulang saat qty atau satuan berubah ────────────────────────────
  void _hitungKlr() {
    if (_barangKlr.isEmpty || _satuanKlr == null) return;
    final qty = double.tryParse(_qtyKlrCtrl.text) ?? 0;
    final harga = safeToDouble(_barangKlr['HARGA_BELI']);
    final isi = _satuanKlr!.isi;
    setState(() {
      _barangKlr['qty'] = qty;
      _barangKlr['SATUAN'] = _satuanKlr!.nama;
      _barangKlr['ISI_SATUAN'] = isi;
      _barangKlr['total_qty'] = qty * isi;
      _barangKlr['total_harga'] = qty * isi * harga;
    });
  }

  void _hitungMsk() {
    if (_barangMsk.isEmpty || _satuanMsk == null) return;
    final qty = double.tryParse(_qtyMskCtrl.text) ?? 0;
    final harga = safeToDouble(_barangMsk['HARGA_BELI']);
    final isi = _satuanMsk!.isi;
    setState(() {
      _barangMsk['qty'] = qty;
      _barangMsk['SATUAN'] = _satuanMsk!.nama;
      _barangMsk['ISI_SATUAN'] = isi;
      _barangMsk['total_qty'] = qty * isi;
      _barangMsk['total_harga'] = qty * isi * harga;
    });
  }

  // ── Clear pencarian ──────────────────────────────────────────────────────
  void _clearSearchKlr() {
    _searchCtrlKlr.clear();
    _searchFocusKlr.unfocus();
    setState(() {
      _suggestKlr = [];
      _showSuggestKlr = false;
    });
  }

  void _clearSearchMsk() {
    _searchCtrlMsk.clear();
    _searchFocusMsk.unfocus();
    setState(() {
      _suggestMsk = [];
      _showSuggestMsk = false;
    });
  }

  // ── Scanner barcode ──────────────────────────────────────────────────────
  void _startScannerKlr() => setState(() {
    _isScanning = true;
    _scanForKlr = true;
  });
  void _startScannerMsk() => setState(() {
    _isScanning = true;
    _scanForKlr = false;
  });
  void _stopScanner() => setState(() => _isScanning = false);

  void _onBarcodeDetect(BarcodeCapture cap) {
    final raw = cap.barcodes.firstOrNull?.rawValue;
    if (raw == null) return;
    _stopScanner();
    if (_scanForKlr) {
      _searchCtrlKlr.text = raw;
      _runSearchKlr(raw);
      _searchFocusKlr.requestFocus();
    } else {
      _searchCtrlMsk.text = raw;
      _runSearchMsk(raw);
      _searchFocusMsk.requestFocus();
    }
  }

  // ── Pilih tanggal ────────────────────────────────────────────────────────
  Future<void> _pickDate() async {
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
    if (picked != null) setState(() => _selectedDate = picked);
  }

  // ── Simpan transaksi ─────────────────────────────────────────────────────
  Future<void> _save() async {
    if (_barangKlr.isEmpty) {
      _showSnack('Belum ada barang keluar', Colors.red);
      return;
    }
    if (_barangMsk.isEmpty) {
      _showSnack('Belum ada barang masuk', Colors.red);
      return;
    }
    if (safeToDouble(_barangKlr['qty']) <= 0) {
      _showSnack('Qty keluar harus > 0', Colors.red);
      return;
    }
    if (safeToDouble(_barangMsk['qty']) <= 0) {
      _showSnack('Qty masuk harus > 0', Colors.red);
      return;
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

      final payload = {
        'tgl_transfer': tglSimpan.toIso8601String(),
        'lokasi': auth.selectedLocation,
        'id_user': auth.userFullName.isNotEmpty
            ? auth.userFullName
            : auth.userName,
        'id_komputer': auth.deviceName,
        'jenis_transfer': auth.selectedLocation ?? 'TOKO',
        'uraian': auth.selectedLocation == 'GUDANG'
            ? 'Transfer stok gudang antar barang'
            : 'Transfer stok toko antar barang',
        'izinkan_backdate': auth.hakAkses.izinkanTanggalLampau ? 1 : 0,
        'izinkan_stok_minus': auth.hakAkses.izinkanJualStokMinus ? 1 : 0,
        'id_barang_k': _barangKlr['ID_BARANG'],
        'nama_barang_k': _barangKlr['NAMA_BARANG'],
        'qty_k': _barangKlr['qty'],
        'satuan_k': _barangKlr['SATUAN'],
        'isi_k': _barangKlr['ISI_SATUAN'],
        'qty_sat_k': _barangKlr['total_qty'],
        'harga_sat_k': _barangKlr['HARGA_BELI'],
        'total_harga_k': _barangKlr['total_harga'],
        'id_barang_m': _barangMsk['ID_BARANG'],
        'nama_barang_m': _barangMsk['NAMA_BARANG'],
        'qty_m': _barangMsk['qty'],
        'satuan_m': _barangMsk['SATUAN'],
        'isi_m': _barangMsk['ISI_SATUAN'],
        'qty_sat_m': _barangMsk['total_qty'],
        'harga_sat_m': _barangMsk['HARGA_BELI'],
        'total_harga_m': _barangMsk['total_harga'],
      };

      debugPrint('[TransferStok] 💾 payload: $payload');
      final res = await ApiService.syncTransferStok(payload);
      debugPrint('[TransferStok] response: $res');

      if (res['status'] == 'success') {
        setState(() {
          _barangKlr = {};
          _barangMsk = {};
          _satuanListKlr = [];
          _satuanListMsk = [];
        });
        _qtyKlrCtrl.clear();
        _qtyMskCtrl.clear();
        _showSnack(
          'Transfer ${res['id_transfer'] ?? ''} berhasil disimpan',
          _green,
        );
      } else {
        _showSnack(res['message'] ?? 'Gagal menyimpan', Colors.red);
      }
    } catch (e, st) {
      debugPrint('[TransferStok] ❌ Exception: $e\n$st');
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
        duration: const Duration(seconds: 3),
      ),
    );
  }

  // ── BUILD UTAMA ──────────────────────────────────────────────────────────
  @override
  Widget build(BuildContext context) {
    if (_isScanning) return _buildScanner();

    final auth = Provider.of<AuthProvider>(context);
    final izinkanTglLampau = auth.hakAkses.izinkanTanggalLampau;
    final fmtTgl = _selectedDate != null
        ? '${_selectedDate!.day.toString().padLeft(2, '0')}/'
              '${_selectedDate!.month.toString().padLeft(2, '0')}/'
              '${_selectedDate!.year}'
        : null;

    // Hitung selisih nilai untuk info
    final totalKlr = safeToDouble(_barangKlr['total_harga']);
    final totalMsk = safeToDouble(_barangMsk['total_harga']);
    final selisih = totalMsk - totalKlr;

    return Scaffold(
      backgroundColor: const Color(0xFFF0FDF4),
      appBar: AppBar(
        backgroundColor: _green,
        foregroundColor: Colors.white,
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Transfer Stok',
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
            if (fmtTgl != null)
              Text(
                fmtTgl,
                style: const TextStyle(
                  fontSize: 11,
                  fontWeight: FontWeight.normal,
                ),
              ),
          ],
        ),
        actions: [
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
            onPressed: izinkanTglLampau ? _pickDate : null,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _green))
          : SingleChildScrollView(
              controller: _scrollCtrl,
              padding: const EdgeInsets.fromLTRB(14, 12, 14, 120),
              child: Column(
                children: [
                  // ── Kartu Barang Keluar ──────────────────────────────
                  _buildSectionCard(
                    label: 'Barang Keluar',
                    icon: Icons.arrow_upward_rounded,
                    color: _red,
                    child: _barangKlr.isEmpty
                        ? _buildSearchArea(
                            ctrl: _searchCtrlKlr,
                            focus: _searchFocusKlr,
                            onChanged: _onSearchChangedKlr,
                            onSubmit: _runSearchKlr,
                            onClear: _clearSearchKlr,
                            onScan: _startScannerKlr,
                            showSuggest: _showSuggestKlr,
                            suggestList: _suggestKlr,
                            onSelect: _selectProductKlr,
                          )
                        : _buildItemCard(
                            barang: _barangKlr,
                            satuanList: _satuanListKlr,
                            satuanSelected: _satuanKlr,
                            qtyCtrl: _qtyKlrCtrl,
                            qtyFocus: _qtyKlrFocus,
                            color: _red,
                            onSatuanChanged: (s) {
                              setState(() => _satuanKlr = s);
                              _hitungKlr();
                            },
                            onQtyChanged: (_) => _hitungKlr(),
                            onHapus: () => setState(() {
                              _barangKlr = {};
                              _satuanListKlr = [];
                              _satuanKlr = null;
                              _qtyKlrCtrl.clear();
                            }),
                          ),
                  ),

                  // ── Panah tengah ─────────────────────────────────────
                  Padding(
                    padding: const EdgeInsets.symmetric(vertical: 6),
                    child: Row(
                      children: [
                        Expanded(child: Divider(color: Colors.grey.shade300)),
                        Container(
                          margin: const EdgeInsets.symmetric(horizontal: 10),
                          padding: const EdgeInsets.all(6),
                          decoration: BoxDecoration(
                            color: Colors.white,
                            shape: BoxShape.circle,
                            border: Border.all(color: Colors.grey.shade300),
                          ),
                          child: const Icon(
                            Icons.swap_vert_rounded,
                            size: 20,
                            color: Color(0xFF64748B),
                          ),
                        ),
                        Expanded(child: Divider(color: Colors.grey.shade300)),
                      ],
                    ),
                  ),

                  // ── Kartu Barang Masuk ───────────────────────────────
                  _buildSectionCard(
                    label: 'Barang Masuk',
                    icon: Icons.arrow_downward_rounded,
                    color: _green,
                    child: _barangMsk.isEmpty
                        ? _buildSearchArea(
                            ctrl: _searchCtrlMsk,
                            focus: _searchFocusMsk,
                            onChanged: _onSearchChangedMsk,
                            onSubmit: _runSearchMsk,
                            onClear: _clearSearchMsk,
                            onScan: _startScannerMsk,
                            showSuggest: _showSuggestMsk,
                            suggestList: _suggestMsk,
                            onSelect: _selectProductMsk,
                          )
                        : _buildItemCard(
                            barang: _barangMsk,
                            satuanList: _satuanListMsk,
                            satuanSelected: _satuanMsk,
                            qtyCtrl: _qtyMskCtrl,
                            qtyFocus: _qtyMskFocus,
                            color: _green,
                            onSatuanChanged: (s) {
                              setState(() => _satuanMsk = s);
                              _hitungMsk();
                            },
                            onQtyChanged: (_) => _hitungMsk(),
                            onHapus: () => setState(() {
                              _barangMsk = {};
                              _satuanListMsk = [];
                              _satuanMsk = null;
                              _qtyMskCtrl.clear();
                            }),
                          ),
                  ),

                  // ── Info selisih nilai ───────────────────────────────
                  if (_barangKlr.isNotEmpty && _barangMsk.isNotEmpty)
                    _buildSelisihInfo(selisih),
                ],
              ),
            ),

      // ── Tombol simpan ────────────────────────────────────────────────────
      bottomNavigationBar: (_barangKlr.isEmpty || _barangMsk.isEmpty)
          ? null
          : SafeArea(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 8, 16, 12),
                child: SizedBox(
                  height: 52,
                  child: ElevatedButton.icon(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: _green,
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                    ),
                    onPressed: _isLoading ? null : _save,
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
                      _isLoading ? 'Menyimpan...' : 'Simpan Transfer',
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

  // ── Widget: kartu section dengan header berwarna ─────────────────────────
  Widget _buildSectionCard({
    required String label,
    required IconData icon,
    required Color color,
    required Widget child,
  }) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: color.withValues(alpha: 0.25)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 6,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Header
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.08),
              borderRadius: const BorderRadius.vertical(
                top: Radius.circular(16),
              ),
            ),
            child: Row(
              children: [
                Icon(icon, size: 16, color: color),
                const SizedBox(width: 6),
                Text(
                  label,
                  style: TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.bold,
                    color: color,
                  ),
                ),
              ],
            ),
          ),
          Padding(padding: const EdgeInsets.all(14), child: child),
        ],
      ),
    );
  }

  // ── Widget: area pencarian + daftar saran ────────────────────────────────
  Widget _buildSearchArea({
    required TextEditingController ctrl,
    required FocusNode focus,
    required void Function(String) onChanged,
    required Future<void> Function(String) onSubmit,
    required VoidCallback onClear,
    required VoidCallback onScan,
    required bool showSuggest,
    required List<Map<String, dynamic>> suggestList,
    required void Function(Map<String, dynamic>) onSelect,
  }) {
    return Column(
      children: [
        // Search bar
        Row(
          children: [
            Expanded(
              child: TextField(
                controller: ctrl,
                focusNode: focus,
                onChanged: onChanged,
                onSubmitted: (q) {
                  if (q.trim().length >= 2) onSubmit(q.trim());
                },
                style: const TextStyle(fontSize: 14),
                decoration: InputDecoration(
                  hintText: 'Cari nama barang atau scan barcode...',
                  hintStyle: TextStyle(
                    color: Colors.grey.shade400,
                    fontSize: 13,
                  ),
                  prefixIcon: const Icon(Icons.search, size: 20),
                  suffixIcon: ctrl.text.isNotEmpty
                      ? IconButton(
                          icon: const Icon(Icons.clear, size: 18),
                          onPressed: onClear,
                        )
                      : null,
                  filled: true,
                  fillColor: const Color(0xFFF8FAFC),
                  contentPadding: const EdgeInsets.symmetric(vertical: 10),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                    borderSide: BorderSide(color: Colors.grey.shade200),
                  ),
                  enabledBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                    borderSide: BorderSide(color: Colors.grey.shade200),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                    borderSide: const BorderSide(color: _green, width: 1.5),
                  ),
                ),
              ),
            ),
            const SizedBox(width: 8),
            // Tombol scan
            Material(
              color: _green,
              borderRadius: BorderRadius.circular(10),
              child: InkWell(
                onTap: onScan,
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
          ],
        ),

        // Daftar saran
        if (showSuggest && suggestList.isNotEmpty) ...[
          const SizedBox(height: 6),
          _buildSuggestList(suggestList, onSelect),
        ],
      ],
    );
  }

  // ── Widget: daftar saran barang ──────────────────────────────────────────
  Widget _buildSuggestList(
    List<Map<String, dynamic>> list,
    void Function(Map<String, dynamic>) onSelect,
  ) {
    final auth = Provider.of<AuthProvider>(context, listen: false);
    final lokasi = auth.selectedLocation ?? 'TOKO';
    final stokKey = lokasi == 'GUDANG' ? 'STOK_GUDANG' : 'STOK_TOKO';
    final fmtHarga = NumberFormat('#,##0', 'id_ID');

    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: Colors.grey.shade200),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.06),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: ListView.separated(
        shrinkWrap: true,
        physics: const NeverScrollableScrollPhysics(),
        padding: EdgeInsets.zero,
        itemCount: list.length,
        separatorBuilder: (_, idx) =>
            Divider(height: 1, color: Colors.grey.shade100),
        itemBuilder: (_, i) {
          final p = list[i];
          final stok = safeToDouble(p[stokKey]);
          final harga = safeToDouble(p['HARGA_BELI']);
          final kat = p['Kategori']?.toString() ?? '';
          return ListTile(
            dense: true,
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 12,
              vertical: 2,
            ),
            leading: Container(
              width: 34,
              height: 34,
              decoration: BoxDecoration(
                color: _green.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(8),
              ),
              child: const Icon(
                Icons.inventory_2_outlined,
                size: 17,
                color: _green,
              ),
            ),
            title: Text(
              p['NAMA_BARANG'] ?? '',
              style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
            ),
            subtitle: Text(
              '${kat.isNotEmpty ? '$kat  ·  ' : ''}Rp ${fmtHarga.format(harga)}',
              style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
            ),
            trailing: Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
              decoration: BoxDecoration(
                color: stok <= 0 ? Colors.red.shade50 : Colors.green.shade50,
                borderRadius: BorderRadius.circular(6),
              ),
              child: Text(
                NumberFormat('#,##0', 'id_ID').format(stok),
                style: TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.bold,
                  color: stok <= 0 ? Colors.red : Colors.green.shade700,
                ),
              ),
            ),
            onTap: () => onSelect(p),
          );
        },
      ),
    );
  }

  // ── Widget: kartu barang terpilih (dengan qty + konversi satuan) ──────────
  Widget _buildItemCard({
    required Map<String, dynamic> barang,
    required List<_SatuanItem> satuanList,
    required _SatuanItem? satuanSelected,
    required TextEditingController qtyCtrl,
    required FocusNode qtyFocus,
    required Color color,
    required void Function(_SatuanItem) onSatuanChanged,
    required void Function(String) onQtyChanged,
    required VoidCallback onHapus,
  }) {
    final fmtHarga = NumberFormat('#,##0', 'id_ID');
    final totalHarga = safeToDouble(barang['total_harga']);
    final totalQty = safeToDouble(barang['total_qty']);
    final stok = safeToDouble(barang['STOK_SYSTEM']);
    final hargaSat = safeToDouble(barang['HARGA_BELI']);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // ── Nama barang + tombol hapus ─────────────────────────────────
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    barang['NAMA_BARANG'] ?? '',
                    style: const TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFF1E293B),
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 2),
                  Text(
                    barang['ID_BARANG'] ?? '',
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade400),
                  ),
                ],
              ),
            ),
            const SizedBox(width: 8),
            GestureDetector(
              onTap: onHapus,
              child: Container(
                padding: const EdgeInsets.all(5),
                decoration: BoxDecoration(
                  color: Colors.red.shade50,
                  shape: BoxShape.circle,
                ),
                child: Icon(Icons.close, size: 16, color: Colors.red.shade400),
              ),
            ),
          ],
        ),

        const SizedBox(height: 12),

        // ── Baris: Stok & Harga beli ───────────────────────────────────
        Row(
          children: [
            _infoChip(
              icon: Icons.inventory_2_outlined,
              label: 'Stok',
              value: NumberFormat('#,##0.##', 'id_ID').format(stok),
              color: stok <= 0 ? Colors.red : Colors.blueGrey,
            ),
            const SizedBox(width: 8),
            _infoChip(
              icon: Icons.price_change_outlined,
              label: 'Harga/sat',
              value: 'Rp ${fmtHarga.format(hargaSat)}',
              color: Colors.blueGrey,
            ),
          ],
        ),

        const SizedBox(height: 12),

        // ── Baris: Input Qty + Dropdown Satuan ────────────────────────
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Input qty
            Expanded(
              flex: 2,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Jumlah',
                    style: TextStyle(
                      fontSize: 11,
                      color: Colors.grey.shade500,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  const SizedBox(height: 4),
                  TextField(
                    controller: qtyCtrl,
                    focusNode: qtyFocus,
                    keyboardType: const TextInputType.numberWithOptions(
                      decimal: true,
                    ),
                    inputFormatters: [
                      FilteringTextInputFormatter.allow(RegExp(r'^\d*\.?\d*')),
                    ],
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.bold,
                      color: color,
                    ),
                    onChanged: onQtyChanged,
                    decoration: InputDecoration(
                      isDense: true,
                      hintText: 'Qty',
                      hintStyle: TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.bold,
                        color: color.withValues(alpha: 0.3),
                      ),
                      contentPadding: const EdgeInsets.symmetric(
                        vertical: 10,
                        horizontal: 8,
                      ),
                      filled: true,
                      fillColor: color.withValues(alpha: 0.06),
                      border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(10),
                        borderSide: BorderSide(
                          color: color.withValues(alpha: 0.3),
                        ),
                      ),
                      enabledBorder: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(10),
                        borderSide: BorderSide(
                          color: color.withValues(alpha: 0.3),
                        ),
                      ),
                      focusedBorder: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(10),
                        borderSide: BorderSide(color: color, width: 1.5),
                      ),
                    ),
                  ),
                ],
              ),
            ),

            const SizedBox(width: 10),

            // Dropdown satuan
            Expanded(
              flex: 3,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Satuan',
                    style: TextStyle(
                      fontSize: 11,
                      color: Colors.grey.shade500,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  const SizedBox(height: 4),
                  satuanList.length <= 1
                      // Hanya 1 satuan — tampilkan sebagai chip statis
                      ? Container(
                          width: double.infinity,
                          padding: const EdgeInsets.symmetric(
                            vertical: 12,
                            horizontal: 12,
                          ),
                          decoration: BoxDecoration(
                            color: color.withValues(alpha: 0.06),
                            borderRadius: BorderRadius.circular(10),
                            border: Border.all(
                              color: color.withValues(alpha: 0.3),
                            ),
                          ),
                          child: Text(
                            satuanSelected?.nama ?? '-',
                            style: TextStyle(
                              fontSize: 14,
                              fontWeight: FontWeight.bold,
                              color: color,
                            ),
                            textAlign: TextAlign.center,
                          ),
                        )
                      // Lebih dari 1 satuan — tampilkan dropdown
                      : DropdownButtonFormField<_SatuanItem>(
                          initialValue: satuanSelected,
                          isExpanded: true,
                          decoration: InputDecoration(
                            isDense: true,
                            contentPadding: const EdgeInsets.symmetric(
                              vertical: 10,
                              horizontal: 12,
                            ),
                            filled: true,
                            fillColor: color.withValues(alpha: 0.06),
                            border: OutlineInputBorder(
                              borderRadius: BorderRadius.circular(10),
                              borderSide: BorderSide(
                                color: color.withValues(alpha: 0.3),
                              ),
                            ),
                            enabledBorder: OutlineInputBorder(
                              borderRadius: BorderRadius.circular(10),
                              borderSide: BorderSide(
                                color: color.withValues(alpha: 0.3),
                              ),
                            ),
                            focusedBorder: OutlineInputBorder(
                              borderRadius: BorderRadius.circular(10),
                              borderSide: BorderSide(color: color, width: 1.5),
                            ),
                          ),
                          style: TextStyle(
                            fontSize: 14,
                            fontWeight: FontWeight.bold,
                            color: color,
                          ),
                          items: satuanList
                              .map(
                                (s) => DropdownMenuItem(
                                  value: s,
                                  child: Text(
                                    '${s.nama} (isi ${s.isi})',
                                    style: TextStyle(
                                      fontSize: 13,
                                      color: color,
                                    ),
                                  ),
                                ),
                              )
                              .toList(),
                          onChanged: (s) {
                            if (s != null) onSatuanChanged(s);
                          },
                        ),
                ],
              ),
            ),
          ],
        ),

        const SizedBox(height: 12),

        // ── Ringkasan: total qty satuan & total harga ──────────────────
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
          decoration: BoxDecoration(
            color: color.withValues(alpha: 0.05),
            borderRadius: BorderRadius.circular(10),
            border: Border.all(color: color.withValues(alpha: 0.15)),
          ),
          child: Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Total Qty (satuan)',
                      style: TextStyle(
                        fontSize: 10,
                        color: Colors.grey.shade500,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      '${NumberFormat('#,##0.##', 'id_ID').format(totalQty)} ${satuanSelected?.nama ?? ''}',
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.bold,
                        color: color,
                      ),
                    ),
                  ],
                ),
              ),
              Container(
                width: 1,
                height: 32,
                color: color.withValues(alpha: 0.2),
              ),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    Text(
                      'Total Harga',
                      style: TextStyle(
                        fontSize: 10,
                        color: Colors.grey.shade500,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      'Rp ${fmtHarga.format(totalHarga)}',
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.bold,
                        color: color,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  // ── Widget: chip info kecil ──────────────────────────────────────────────
  Widget _infoChip({
    required IconData icon,
    required String label,
    required String value,
    required Color color,
  }) {
    return Expanded(
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 7),
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.06),
          borderRadius: BorderRadius.circular(8),
          border: Border.all(color: color.withValues(alpha: 0.15)),
        ),
        child: Row(
          children: [
            Icon(icon, size: 14, color: color),
            const SizedBox(width: 5),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    label,
                    style: TextStyle(fontSize: 9, color: Colors.grey.shade500),
                  ),
                  Text(
                    value,
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                      color: color,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  // ── Widget: info selisih nilai ───────────────────────────────────────────
  Widget _buildSelisihInfo(double selisih) {
    final fmtHarga = NumberFormat('#,##0', 'id_ID');
    final isLebih = selisih > 0;
    final isKurang = selisih < 0;
    final color = isLebih
        ? Colors.blue.shade700
        : (isKurang ? Colors.orange.shade700 : _green);
    final bgColor = isLebih
        ? Colors.blue.shade50
        : (isKurang ? Colors.orange.shade50 : Colors.green.shade50);
    final label = isLebih
        ? 'Kelebihan nilai Rp ${fmtHarga.format(selisih.abs())}'
        : isKurang
        ? 'Kekurangan nilai Rp ${fmtHarga.format(selisih.abs())}'
        : 'Nilai HPP masuk dan keluar sama';
    final icon = isLebih
        ? Icons.trending_up
        : (isKurang ? Icons.trending_down : Icons.check_circle_outline);

    return Padding(
      padding: const EdgeInsets.only(top: 8),
      child: Container(
        width: double.infinity,
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
        decoration: BoxDecoration(
          color: bgColor,
          borderRadius: BorderRadius.circular(10),
          border: Border.all(color: color.withValues(alpha: 0.3)),
        ),
        child: Row(
          children: [
            Icon(icon, size: 16, color: color),
            const SizedBox(width: 8),
            Text(
              label,
              style: TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.w600,
                color: color,
              ),
            ),
          ],
        ),
      ),
    );
  }

  // ── Widget: scanner barcode ──────────────────────────────────────────────
  Widget _buildScanner() {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: _green,
        foregroundColor: Colors.white,
        title: Text(
          _scanForKlr ? 'Scan Barcode — Keluar' : 'Scan Barcode — Masuk',
        ),
        leading: IconButton(
          onPressed: _stopScanner,
          icon: const Icon(Icons.close),
        ),
      ),
      body: MobileScanner(onDetect: _onBarcodeDetect),
    );
  }
}
