import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import '../../providers/auth_provider.dart';
import '../../providers/penjualan_provider.dart';
import '../../services/api_service.dart';
import '../../utils/safe_convert.dart';

class Step2ItemsScreen extends StatefulWidget {
  final VoidCallback onNext;
  final VoidCallback onBack;
  const Step2ItemsScreen({
    super.key,
    required this.onNext,
    required this.onBack,
  });

  @override
  State<Step2ItemsScreen> createState() => _Step2ItemsScreenState();
}

class _Step2ItemsScreenState extends State<Step2ItemsScreen> {
  static const _green = Color(0xFF16A34A);
  final _fmt = NumberFormat('#,##0.##', 'id_ID');

  // ── Pencarian ────────────────────────────────────────────────
  final _searchCtrl = TextEditingController();
  final _searchFocus = FocusNode();
  List<Map<String, dynamic>> _suggest = [];
  bool _showSuggest = false;
  bool _isScanning = false;
  Timer? _debounce;

  // ── Barang yang sedang dipilih (sebelum ditambah ke cart) ────
  Map<String, dynamic>? _selectedProduct;
  List<Map<String, dynamic>> _satuanOpts = []; // [{key,nama,isi,harga}]
  int _satuanIdx = 0;

  // ── Input qty, harga, diskon untuk barang terpilih ──────────
  final _qtyCtrl = TextEditingController();
  final _hargaCtrl = TextEditingController();
  final _diskonCtrl = TextEditingController(); // diskon % per satuan
  final _diskonRpCtrl =
      TextEditingController(); // diskon Rp per satuan — sinkron dengan %
  bool _updatingDiskonPanel = false;
  final _qtyFocus = FocusNode();

  @override
  void initState() {
    super.initState();
    _searchFocus.addListener(() {
      if (!_searchFocus.hasFocus) {
        Future.delayed(const Duration(milliseconds: 150), () {
          if (mounted) setState(() => _showSuggest = false);
        });
      }
    });
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _searchCtrl.dispose();
    _searchFocus.dispose();
    _qtyCtrl.dispose();
    _hargaCtrl.dispose();
    _diskonCtrl.dispose();
    _diskonRpCtrl.dispose();
    _qtyFocus.dispose();
    super.dispose();
  }

  // ── Pencarian teks ───────────────────────────────────────────
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
      final res = await ApiService.getStock(search: q, limit: 40);
      if (!mounted) return;
      if (res['status'] == 'success') {
        setState(() {
          _suggest = List<Map<String, dynamic>>.from(res['data'] ?? []);
          _showSuggest = _suggest.isNotEmpty;
        });
      }
    } catch (_) {}
  }

  void _clearSearch() {
    _searchCtrl.clear();
    _searchFocus.unfocus();
    setState(() {
      _suggest = [];
      _showSuggest = false;
    });
  }

  // ── Scanner barcode ──────────────────────────────────────────
  void _startScanner() => setState(() => _isScanning = true);
  void _stopScanner() => setState(() => _isScanning = false);

  void _onBarcodeDetect(BarcodeCapture cap) {
    final raw = cap.barcodes.firstOrNull?.rawValue;
    if (raw == null || raw.isEmpty) return;
    _stopScanner();
    _searchByBarcode(raw);
  }

  Future<void> _searchByBarcode(String barcode) async {
    try {
      final res = await ApiService.getStock(search: barcode, limit: 10);
      if (!mounted) return;
      if (res['status'] != 'success') return;
      final list = List<Map<String, dynamic>>.from(res['data'] ?? []);
      Map<String, dynamic>? found;
      String? matchedKey;
      for (final p in list) {
        for (final key in ['KECIL', 'SEDANG', 'BESAR']) {
          if (p['BARCODE_$key']?.toString() == barcode) {
            found = p;
            matchedKey = key;
            break;
          }
        }
        if (found != null) break;
      }
      if (found == null) {
        if (mounted) {
          _showSnack('Barcode "$barcode" tidak ditemukan', Colors.red);
        }
        return;
      }
      _selectProduct(found, preSelectKey: matchedKey);
    } catch (e) {
      debugPrint('[Step2] barcode error: $e');
    }
  }

  // ── Pilih barang dari suggest list ───────────────────────────
  void _selectProduct(Map<String, dynamic> p, {String? preSelectKey}) {
    _clearSearch();
    final prov = context.read<PenjualanProvider>();
    final jenisPelanggan =
        prov.selectedPelanggan?['JENIS']?.toString().toUpperCase() ?? 'UMUM';
    final isPartai = jenisPelanggan == 'PARTAI' || jenisPelanggan == 'GROSIR';

    // Bangun daftar satuan yang tersedia
    final opts = <Map<String, dynamic>>[];
    for (final key in ['KECIL', 'SEDANG', 'BESAR']) {
      final nama = p['SATUAN_UMUM_$key']?.toString() ?? '';
      if (nama.isEmpty) continue;
      final isi = safeToDouble(p['ISI_UMUM_$key']);
      final harga = safeToDouble(
        isPartai ? p['HARGA_JUAL_PARTAI_$key'] : p['HARGA_JUAL_UMUM_$key'],
      );
      opts.add({'key': key, 'nama': nama, 'isi': isi, 'harga': harga});
    }
    if (opts.isEmpty) return;

    int initialIdx = 0;
    if (preSelectKey != null) {
      final idx = opts.indexWhere((s) => s['key'] == preSelectKey);
      if (idx >= 0) initialIdx = idx;
    }

    setState(() {
      _selectedProduct = p;
      _satuanOpts = opts;
      _satuanIdx = initialIdx;
      _qtyCtrl.clear();
      _diskonCtrl.clear();
      _diskonRpCtrl.clear();
      _hargaCtrl.text = safeToDouble(
        opts[initialIdx]['harga'],
      ).toStringAsFixed(0);
    });

    // Fokus ke qty setelah frame selesai
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) _qtyFocus.requestFocus();
    });
  }

  void _clearSelectedProduct() {
    setState(() {
      _selectedProduct = null;
      _satuanOpts = [];
    });
    _qtyCtrl.clear();
    _hargaCtrl.clear();
    _diskonCtrl.clear();
    _diskonRpCtrl.clear();
  }

  void _onSatuanChanged(int idx) {
    setState(() {
      _satuanIdx = idx;
      _hargaCtrl.text = safeToDouble(
        _satuanOpts[idx]['harga'],
      ).toStringAsFixed(0);
    });
  }

  // ── Tambah ke cart ───────────────────────────────────────────
  void _addToCart() {
    if (_selectedProduct == null || _satuanOpts.isEmpty) return;
    final auth = context.read<AuthProvider>();
    final prov = context.read<PenjualanProvider>();
    final hakAkses = auth.hakAkses;
    final stokKey = (auth.selectedLocation ?? 'TOKO') == 'GUDANG'
        ? 'STOK_GUDANG'
        : 'STOK_TOKO';

    final opt = _satuanOpts[_satuanIdx];
    final p = _selectedProduct!;
    final qty = double.tryParse(_qtyCtrl.text) ?? 1;
    final harga = double.tryParse(_hargaCtrl.text) ?? 0;
    // diskonPersen dan diskonRp selalu sinkron — keduanya dikirim
    final diskonPersen = double.tryParse(_diskonCtrl.text) ?? 0;
    final diskonRp = double.tryParse(_diskonRpCtrl.text) ?? 0;
    final stok = safeToDouble(p[stokKey]);

    if (qty <= 0) {
      _showSnack('Qty harus lebih dari 0', Colors.red);
      return;
    }

    // Validasi stok
    if (!hakAkses.izinkanJualStokMinus) {
      final qtySatuan = qty * safeToDouble(opt['isi']);
      if (qtySatuan > stok) {
        _showSnack(
          'Stok tidak cukup — tersedia ${_fmt.format(stok)} ${opt['nama']}',
          Colors.red,
        );
        return;
      }
    }

    final item = CartItem(
      idBarang: p['ID_BARANG']?.toString() ?? '',
      namaBarang: p['NAMA_BARANG']?.toString() ?? '',
      satuan: opt['nama']?.toString() ?? '',
      isiSatuan: safeToDouble(opt['isi']),
      hargaBeli: safeToDouble(p['HARGA_BELI']),
      hargaJual: harga,
      qty: qty,
      diskonPersen: diskonPersen,
      diskonRp: diskonRp,
      stokToko: safeToDouble(p['STOK_TOKO']),
      stokGudang: safeToDouble(p['STOK_GUDANG']),
    );

    prov.addItem(item, izinkanSatuanBerbeda: hakAkses.izinkanSatuanBerbeda);
    _clearSelectedProduct();
    _showSnack('${item.namaBarang} ditambahkan', _green);
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

  // ── BUILD ────────────────────────────────────────────────────
  @override
  Widget build(BuildContext context) {
    if (_isScanning) {
      return Scaffold(
        appBar: AppBar(
          backgroundColor: _green,
          foregroundColor: Colors.white,
          title: const Text('Scan Barcode'),
          leading: IconButton(
            icon: const Icon(Icons.close),
            onPressed: _stopScanner,
          ),
        ),
        body: MobileScanner(onDetect: _onBarcodeDetect),
      );
    }

    final prov = context.watch<PenjualanProvider>();
    final auth = context.watch<AuthProvider>();
    final hakAkses = auth.hakAkses;
    final lokasi = auth.selectedLocation ?? 'TOKO';
    final stokKey = lokasi == 'GUDANG' ? 'STOK_GUDANG' : 'STOK_TOKO';

    return Column(
      children: [
        // ── Search bar + tombol scan ─────────────────────────
        Container(
          color: _green,
          padding: const EdgeInsets.fromLTRB(12, 0, 12, 12),
          child: Row(
            children: [
              Expanded(
                child: TextField(
                  controller: _searchCtrl,
                  focusNode: _searchFocus,
                  onChanged: _onSearchChanged,
                  style: const TextStyle(fontSize: 14),
                  decoration: InputDecoration(
                    hintText: 'Cari nama barang atau scan barcode...',
                    hintStyle: TextStyle(
                      color: Colors.grey.shade400,
                      fontSize: 13,
                    ),
                    prefixIcon: const Icon(Icons.search, size: 20),
                    suffixIcon: _searchCtrl.text.isNotEmpty
                        ? IconButton(
                            icon: const Icon(Icons.clear, size: 18),
                            onPressed: _clearSearch,
                          )
                        : null,
                    filled: true,
                    fillColor: Colors.white,
                    contentPadding: const EdgeInsets.symmetric(vertical: 10),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(10),
                      borderSide: BorderSide.none,
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 8),
              Material(
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
            ],
          ),
        ),

        // ── Konten utama ─────────────────────────────────────
        Expanded(
          child: Stack(
            children: [
              // ── Scroll area: panel barang terpilih + cart ──
              SingleChildScrollView(
                padding: EdgeInsets.fromLTRB(
                  12,
                  10,
                  12,
                  // Beri ruang ekstra saat keyboard muncul
                  MediaQuery.viewInsetsOf(context).bottom + 90,
                ),
                keyboardDismissBehavior:
                    ScrollViewKeyboardDismissBehavior.onDrag,
                child: Column(
                  children: [
                    // ── Panel barang terpilih (inline, bukan modal) ──
                    if (_selectedProduct != null)
                      _buildSelectedPanel(hakAkses, stokKey),

                    // ── Daftar cart ──────────────────────────
                    if (prov.cartItems.isEmpty && _selectedProduct == null)
                      const Padding(
                        padding: EdgeInsets.only(top: 60),
                        child: Center(
                          child: Text(
                            'Cari barang di atas untuk ditambahkan',
                            style: TextStyle(
                              color: Color(0xFF9CA3AF),
                              fontSize: 13,
                            ),
                          ),
                        ),
                      )
                    else
                      ...prov.cartItems.asMap().entries.map(
                        (e) => _CartItemCard(
                          index: e.key,
                          item: e.value,
                          hakAkses: hakAkses,
                          fmt: _fmt,
                          onRemove: () => prov.removeItem(e.key),
                          onQtyChanged: (v) => prov.updateQty(e.key, v),
                          onHargaChanged: (v) => prov.updateHarga(e.key, v),
                          onDiskonChanged: (p, r) =>
                              prov.updateDiskonItem(e.key, persen: p, rp: r),
                        ),
                      ),
                  ],
                ),
              ),

              // ── Suggest list mengambang ───────────────────
              if (_showSuggest)
                Positioned(
                  top: 0,
                  left: 0,
                  right: 0,
                  child: ConstrainedBox(
                    constraints: BoxConstraints(
                      maxHeight: MediaQuery.of(context).size.height * 0.45,
                    ),
                    child: Material(
                      elevation: 6,
                      borderRadius: const BorderRadius.vertical(
                        bottom: Radius.circular(12),
                      ),
                      child: ClipRRect(
                        borderRadius: const BorderRadius.vertical(
                          bottom: Radius.circular(12),
                        ),
                        child: ListView.separated(
                          shrinkWrap: true,
                          padding: EdgeInsets.zero,
                          itemCount: _suggest.length,
                          separatorBuilder: (_, idx) =>
                              const Divider(height: 1),
                          itemBuilder: (_, i) {
                            final p = _suggest[i];
                            final stok = safeToDouble(p[stokKey]);
                            final harga = safeToDouble(
                              p['HARGA_JUAL_UMUM_KECIL'],
                            );
                            return ListTile(
                              dense: true,
                              contentPadding: const EdgeInsets.symmetric(
                                horizontal: 14,
                                vertical: 2,
                              ),
                              title: Text(
                                p['NAMA_BARANG'] ?? '',
                                style: const TextStyle(
                                  fontSize: 13,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                              subtitle: Text(
                                'Rp ${_fmt.format(harga)}',
                                style: TextStyle(
                                  fontSize: 11,
                                  color: Colors.grey.shade500,
                                ),
                              ),
                              trailing: hakAkses.tampilInfoStok
                                  ? _stokBadge(stok)
                                  : null,
                              onTap: () => _selectProduct(p),
                            );
                          },
                        ),
                      ),
                    ),
                  ),
                ),
            ],
          ),
        ),

        // ── Footer: subtotal + tombol lanjut ─────────────────
        SafeArea(
          child: Container(
            padding: const EdgeInsets.fromLTRB(16, 10, 16, 12),
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
            child: Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        '${prov.cartItems.length} item',
                        style: TextStyle(
                          fontSize: 11,
                          color: Colors.grey.shade500,
                        ),
                      ),
                      Text(
                        'Rp ${_fmt.format(prov.subtotal)}',
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF1E293B),
                        ),
                      ),
                    ],
                  ),
                ),
                ElevatedButton.icon(
                  onPressed: prov.cartItems.isEmpty ? null : widget.onNext,
                  icon: const Icon(Icons.arrow_forward),
                  label: const Text(
                    'Lanjut ke Bayar →',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }

  // ── Panel barang terpilih — inline, bukan modal ──────────────
  Widget _buildSelectedPanel(dynamic hakAkses, String stokKey) {
    final p = _selectedProduct!;
    final opt = _satuanOpts[_satuanIdx];
    final stok = safeToDouble(p[stokKey]);
    final qty = double.tryParse(_qtyCtrl.text) ?? 0;
    final harga = double.tryParse(_hargaCtrl.text) ?? 0;
    final diskonRp = double.tryParse(_diskonRpCtrl.text) ?? 0;
    // Ikuti pola VB: totalDiskon = qty * diskonRp (diskonRp adalah per satuan)
    final totalDiskon = qty * diskonRp;
    final total = (harga * qty) - totalDiskon;
    final stokKurang =
        !hakAkses.izinkanJualStokMinus && qty * safeToDouble(opt['isi']) > stok;

    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: _green.withValues(alpha: 0.4), width: 1.5),
        boxShadow: [
          BoxShadow(
            color: _green.withValues(alpha: 0.08),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Padding(
        padding: const EdgeInsets.fromLTRB(12, 10, 12, 12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // ── Baris 1: Nama + Qty (kanan) + batal ───────
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Kiri: nama + satuan + stok
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        p['NAMA_BARANG']?.toString() ?? '',
                        style: const TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF1E293B),
                        ),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                      const SizedBox(height: 3),
                      _satuanOpts.length == 1
                          ? _satuanChipLocked(opt)
                          : Wrap(
                              spacing: 6,
                              children: List.generate(_satuanOpts.length, (i) {
                                final s = _satuanOpts[i];
                                final active = i == _satuanIdx;
                                return GestureDetector(
                                  onTap: () => _onSatuanChanged(i),
                                  child: Container(
                                    padding: const EdgeInsets.symmetric(
                                      horizontal: 8,
                                      vertical: 3,
                                    ),
                                    decoration: BoxDecoration(
                                      color: active
                                          ? _green
                                          : _green.withValues(alpha: 0.08),
                                      borderRadius: BorderRadius.circular(14),
                                      border: Border.all(
                                        color: active
                                            ? _green
                                            : _green.withValues(alpha: 0.3),
                                      ),
                                    ),
                                    child: Text(
                                      '${s['nama']} ×${_fmt.format(s['isi'])}',
                                      style: TextStyle(
                                        fontSize: 11,
                                        fontWeight: FontWeight.w600,
                                        color: active ? Colors.white : _green,
                                      ),
                                    ),
                                  ),
                                );
                              }),
                            ),
                      if (hakAkses.tampilInfoStok) ...[
                        const SizedBox(height: 2),
                        Text(
                          'Stok: ${_fmt.format(stok)} ${opt['nama']}',
                          style: TextStyle(
                            fontSize: 10,
                            color: stok <= 0
                                ? Colors.red
                                : Colors.grey.shade500,
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
                const SizedBox(width: 8),
                // Kanan: input qty (lebih kecil)
                Column(
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Text(
                      'Qty',
                      style: TextStyle(
                        fontSize: 10,
                        color: Colors.grey.shade500,
                      ),
                    ),
                    const SizedBox(height: 3),
                    SizedBox(
                      width: 68,
                      child: TextField(
                        controller: _qtyCtrl,
                        focusNode: _qtyFocus,
                        keyboardType: const TextInputType.numberWithOptions(
                          decimal: true,
                        ),
                        inputFormatters: [
                          FilteringTextInputFormatter.allow(
                            RegExp(r'^\d*\.?\d*'),
                          ),
                        ],
                        textAlign: TextAlign.center,
                        onChanged: (_) => setState(() {}),
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: _green,
                        ),
                        decoration: InputDecoration(
                          isDense: true,
                          hintText: 'Qty',
                          hintStyle: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: _green.withValues(alpha: 0.3),
                          ),
                          contentPadding: const EdgeInsets.symmetric(
                            vertical: 7,
                          ),
                          filled: true,
                          fillColor: _green.withValues(alpha: 0.06),
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(8),
                            borderSide: BorderSide(
                              color: _green.withValues(alpha: 0.3),
                            ),
                          ),
                          enabledBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(8),
                            borderSide: BorderSide(
                              color: _green.withValues(alpha: 0.3),
                            ),
                          ),
                          focusedBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(8),
                            borderSide: const BorderSide(
                              color: _green,
                              width: 1.5,
                            ),
                          ),
                        ),
                      ),
                    ),
                    Text(
                      opt['nama']?.toString() ?? '',
                      style: TextStyle(
                        fontSize: 9,
                        color: _green.withValues(alpha: 0.7),
                      ),
                    ),
                  ],
                ),
                const SizedBox(width: 6),
                // Tombol batal
                GestureDetector(
                  onTap: _clearSelectedProduct,
                  child: Container(
                    padding: const EdgeInsets.all(4),
                    decoration: BoxDecoration(
                      color: Colors.red.shade50,
                      shape: BoxShape.circle,
                    ),
                    child: Icon(
                      Icons.close,
                      size: 15,
                      color: Colors.red.shade400,
                    ),
                  ),
                ),
              ],
            ),

            const SizedBox(height: 8),

            // ── Baris 2: Harga | Diskon% | Diskon Rp ──────
            Row(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Expanded(
                  flex: 3,
                  child: _inputFieldSelected(
                    'Harga Jual',
                    _hargaCtrl,
                    enabled: hakAkses.izinkanUbahHarga,
                    formatRibuan: true,
                  ),
                ),
                const SizedBox(width: 6),
                Expanded(
                  flex: 2,
                  child: _inputFieldSelected(
                    'Disc %',
                    _diskonCtrl,
                    onChanged: (v) {
                      if (_updatingDiskonPanel) return;
                      _updatingDiskonPanel = true;
                      // Isi % → hitung Rp per satuan: diskonRp = harga * persen / 100
                      final persen = double.tryParse(v) ?? 0;
                      final h = double.tryParse(_hargaCtrl.text) ?? 0;
                      final rp = h * persen / 100;
                      _diskonRpCtrl.text = rp > 0 ? rp.toStringAsFixed(0) : '';
                      _updatingDiskonPanel = false;
                      setState(() {});
                    },
                  ),
                ),
                const SizedBox(width: 6),
                Expanded(
                  flex: 2,
                  child: _inputFieldSelected(
                    'Disc Rp',
                    _diskonRpCtrl,
                    onChanged: (v) {
                      if (_updatingDiskonPanel) return;
                      _updatingDiskonPanel = true;
                      // Isi Rp per satuan → hitung %: persen = (diskonRp / harga) * 100
                      final rp = double.tryParse(v) ?? 0;
                      final h = double.tryParse(_hargaCtrl.text) ?? 0;
                      final persen = h > 0 ? (rp / h) * 100 : 0.0;
                      _diskonCtrl.text = persen > 0
                          ? persen
                                .toStringAsFixed(2)
                                .replaceAll(RegExp(r'\.?0+$'), '')
                          : '';
                      _updatingDiskonPanel = false;
                      setState(() {});
                    },
                  ),
                ),
              ],
            ),

            const SizedBox(height: 8),

            // ── Baris 3: Total diskon + Total bersih + Tambah ──
            Row(
              children: [
                Expanded(
                  child: Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 10,
                      vertical: 7,
                    ),
                    decoration: BoxDecoration(
                      color: _green.withValues(alpha: 0.05),
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: _green.withValues(alpha: 0.2)),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (totalDiskon > 0)
                          Text(
                            'Disc: -${_fmt.format(totalDiskon)}',
                            style: const TextStyle(
                              fontSize: 10,
                              color: Color(0xFFDC2626),
                            ),
                          ),
                        Text(
                          'Rp ${_fmt.format(total)}',
                          style: const TextStyle(
                            fontSize: 14,
                            fontWeight: FontWeight.bold,
                            color: _green,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                SizedBox(
                  height: 44,
                  child: ElevatedButton.icon(
                    onPressed: stokKurang ? null : _addToCart,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: _green,
                      foregroundColor: Colors.white,
                      disabledBackgroundColor: Colors.grey.shade300,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10),
                      ),
                    ),
                    icon: const Icon(Icons.add_shopping_cart, size: 16),
                    label: const Text(
                      'Tambah',
                      style: TextStyle(fontWeight: FontWeight.bold),
                    ),
                  ),
                ),
              ],
            ),

            if (stokKurang) ...[
              const SizedBox(height: 6),
              Container(
                width: double.infinity,
                padding: const EdgeInsets.symmetric(
                  horizontal: 10,
                  vertical: 6,
                ),
                decoration: BoxDecoration(
                  color: Colors.red.shade50,
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.red.shade200),
                ),
                child: Row(
                  children: [
                    Icon(
                      Icons.warning_amber_rounded,
                      size: 14,
                      color: Colors.red.shade600,
                    ),
                    const SizedBox(width: 5),
                    Expanded(
                      child: Text(
                        'Stok tidak cukup — tersedia '
                        '${_fmt.format(stok)} ${opt['nama']}',
                        style: TextStyle(
                          fontSize: 11,
                          color: Colors.red.shade700,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  // ── Input field untuk panel barang terpilih ──────────────────
  Widget _inputFieldSelected(
    String label,
    TextEditingController ctrl, {
    bool enabled = true,
    bool formatRibuan = false,
    void Function(String)? onChanged,
  }) {
    return TextField(
      controller: ctrl,
      enabled: enabled,
      keyboardType: TextInputType.number,
      inputFormatters: [FilteringTextInputFormatter.digitsOnly],
      onChanged: (v) {
        setState(() {});
        onChanged?.call(v);
      },
      style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
      decoration: InputDecoration(
        isDense: true,
        hintText: label,
        hintStyle: TextStyle(
          fontSize: 12,
          fontWeight: FontWeight.normal,
          color: Colors.grey.shade400,
        ),
        contentPadding: const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
        filled: true,
        fillColor: enabled ? Colors.white : Colors.grey.shade100,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(color: Colors.grey.shade300),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(color: Colors.grey.shade300),
        ),
        focusedBorder: const OutlineInputBorder(
          borderRadius: BorderRadius.all(Radius.circular(8)),
          borderSide: BorderSide(color: _green, width: 1.5),
        ),
      ),
    );
  }

  // ── Chip satuan terkunci (hanya 1 satuan) ────────────────────
  Widget _satuanChipLocked(Map<String, dynamic> opt) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: _green.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: _green.withValues(alpha: 0.3)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(Icons.lock_outline, size: 11, color: _green),
          const SizedBox(width: 4),
          Text(
            '${opt['nama']} ×${_fmt.format(opt['isi'])}',
            style: TextStyle(
              fontSize: 11,
              fontWeight: FontWeight.w600,
              color: _green,
            ),
          ),
        ],
      ),
    );
  }

  // ── Badge stok ───────────────────────────────────────────────
  Widget _stokBadge(double stok) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      decoration: BoxDecoration(
        color: stok <= 0 ? Colors.red.shade50 : Colors.green.shade50,
        borderRadius: BorderRadius.circular(6),
      ),
      child: Text(
        _fmt.format(stok),
        style: TextStyle(
          fontSize: 12,
          fontWeight: FontWeight.bold,
          color: stok <= 0 ? Colors.red : _green,
        ),
      ),
    );
  }
}

// ── Card item di keranjang ────────────────────────────────────────────────
class _CartItemCard extends StatefulWidget {
  final int index;
  final CartItem item;
  final dynamic hakAkses;
  final NumberFormat fmt;
  final VoidCallback onRemove;
  final void Function(double) onQtyChanged;
  final void Function(double) onHargaChanged;
  final void Function(double persen, double rp) onDiskonChanged;

  const _CartItemCard({
    required this.index,
    required this.item,
    required this.hakAkses,
    required this.fmt,
    required this.onRemove,
    required this.onQtyChanged,
    required this.onHargaChanged,
    required this.onDiskonChanged,
  });

  @override
  State<_CartItemCard> createState() => _CartItemCardState();
}

class _CartItemCardState extends State<_CartItemCard> {
  static const _green = Color(0xFF16A34A);

  bool _editMode = false;
  late final TextEditingController _qtyCtrl;
  late final TextEditingController _hargaCtrl;
  late final TextEditingController _diskonPersenCtrl;
  late final TextEditingController _diskonRpCtrl;

  @override
  void initState() {
    super.initState();
    final item = widget.item;
    _qtyCtrl = TextEditingController(
      text: item.qty.toStringAsFixed(item.qty % 1 == 0 ? 0 : 2),
    );
    _hargaCtrl = TextEditingController(text: item.hargaJual.toStringAsFixed(0));
    _diskonPersenCtrl = TextEditingController(
      text: item.diskonPersen > 0 ? item.diskonPersen.toStringAsFixed(0) : '',
    );
    _diskonRpCtrl = TextEditingController(
      text: item.diskonRp > 0 ? item.diskonRp.toStringAsFixed(0) : '',
    );
  }

  @override
  void dispose() {
    _qtyCtrl.dispose();
    _hargaCtrl.dispose();
    _diskonPersenCtrl.dispose();
    _diskonRpCtrl.dispose();
    super.dispose();
  }

  void _simpanEdit() {
    final qty = double.tryParse(_qtyCtrl.text) ?? widget.item.qty;
    final harga = double.tryParse(_hargaCtrl.text) ?? widget.item.hargaJual;
    final diskonPersen = double.tryParse(_diskonPersenCtrl.text) ?? 0;
    final diskonRp = double.tryParse(_diskonRpCtrl.text) ?? 0;
    widget.onQtyChanged(qty);
    widget.onHargaChanged(harga);
    widget.onDiskonChanged(diskonPersen, diskonRp);
    setState(() => _editMode = false);
    FocusScope.of(context).unfocus();
  }

  @override
  Widget build(BuildContext context) {
    final item = widget.item;
    final fmt = widget.fmt;
    final totalDiskon = item.totalDiskon;
    final totalHarga = item.totalHarga;

    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(
          color: _editMode
              ? _green.withValues(alpha: 0.5)
              : Colors.grey.shade200,
          width: _editMode ? 1.5 : 1,
        ),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.03),
            blurRadius: 6,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Padding(
        padding: const EdgeInsets.fromLTRB(12, 10, 8, 10),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            // ── Kiri: semua data ─────────────────────────────
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Nama barang
                  Text(
                    item.namaBarang,
                    style: const TextStyle(
                      fontSize: 13,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFF1E293B),
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),

                  // ── Mode VIEW ──────────────────────────────
                  if (!_editMode) ...[
                    const SizedBox(height: 3),
                    Text(
                      '${fmt.format(item.qty)} ${item.satuan}  ×  Rp ${fmt.format(item.hargaJual)}',
                      style: TextStyle(
                        fontSize: 11,
                        color: Colors.grey.shade600,
                      ),
                    ),
                    if (totalDiskon > 0) ...[
                      const SizedBox(height: 1),
                      Text(
                        'Disc: -Rp ${fmt.format(totalDiskon)}',
                        style: const TextStyle(
                          fontSize: 11,
                          color: Color(0xFFDC2626),
                        ),
                      ),
                    ],
                    const SizedBox(height: 3),
                    Text(
                      'Rp ${fmt.format(totalHarga)}',
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.bold,
                        color: _green,
                      ),
                    ),
                  ]
                  // ── Mode EDIT ──────────────────────────────
                  else ...[
                    const SizedBox(height: 6),
                    Row(
                      children: [
                        _editField('Qty', _qtyCtrl, isDecimal: true, flex: 2),
                        const SizedBox(width: 5),
                        _editField(
                          'Harga',
                          _hargaCtrl,
                          enabled: widget.hakAkses.izinkanUbahHarga,
                          flex: 3,
                        ),
                        const SizedBox(width: 5),
                        _editField('Disc%', _diskonPersenCtrl, flex: 2),
                        const SizedBox(width: 5),
                        _editField('Disc Rp', _diskonRpCtrl, flex: 3),
                      ],
                    ),
                    const SizedBox(height: 5),
                    Builder(
                      builder: (_) {
                        final qty = double.tryParse(_qtyCtrl.text) ?? item.qty;
                        final harga =
                            double.tryParse(_hargaCtrl.text) ?? item.hargaJual;
                        final dp = double.tryParse(_diskonPersenCtrl.text) ?? 0;
                        final dr = double.tryParse(_diskonRpCtrl.text) ?? 0;
                        final disc = dp > 0 ? harga * qty * dp / 100 : dr * qty;
                        final tot = harga * qty - disc;
                        return Row(
                          children: [
                            if (disc > 0) ...[
                              Text(
                                'Disc -${fmt.format(disc)}',
                                style: const TextStyle(
                                  fontSize: 11,
                                  color: Color(0xFFDC2626),
                                ),
                              ),
                              const SizedBox(width: 8),
                            ],
                            Text(
                              'Rp ${fmt.format(tot)}',
                              style: const TextStyle(
                                fontSize: 13,
                                fontWeight: FontWeight.bold,
                                color: _green,
                              ),
                            ),
                          ],
                        );
                      },
                    ),
                  ],
                ],
              ),
            ),

            const SizedBox(width: 8),

            // ── Kanan: tombol edit (atas) + hapus (bawah) ───
            Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                // Tombol edit / simpan
                GestureDetector(
                  onTap: _editMode
                      ? _simpanEdit
                      : () => setState(() => _editMode = true),
                  child: Container(
                    padding: const EdgeInsets.all(7),
                    decoration: BoxDecoration(
                      color: _editMode
                          ? _green.withValues(alpha: 0.1)
                          : Colors.blue.shade50,
                      shape: BoxShape.circle,
                    ),
                    child: Icon(
                      _editMode ? Icons.check : Icons.edit_outlined,
                      size: 16,
                      color: _editMode ? _green : Colors.blue.shade600,
                    ),
                  ),
                ),
                // Tombol hapus — sembunyi saat mode edit
                if (!_editMode) ...[
                  const SizedBox(height: 6),
                  GestureDetector(
                    onTap: widget.onRemove,
                    child: Container(
                      padding: const EdgeInsets.all(7),
                      decoration: BoxDecoration(
                        color: Colors.red.shade50,
                        shape: BoxShape.circle,
                      ),
                      child: Icon(
                        Icons.delete_outline,
                        size: 16,
                        color: Colors.red.shade400,
                      ),
                    ),
                  ),
                ],
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _editField(
    String label,
    TextEditingController ctrl, {
    bool isDecimal = false,
    bool enabled = true,
    int flex = 1,
  }) {
    return Expanded(
      flex: flex,
      child: TextField(
        controller: ctrl,
        enabled: enabled,
        keyboardType: TextInputType.numberWithOptions(decimal: isDecimal),
        inputFormatters: [
          FilteringTextInputFormatter.allow(RegExp(r'^\d*\.?\d*')),
        ],
        onChanged: (_) => setState(() {}),
        style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600),
        decoration: InputDecoration(
          isDense: true,
          hintText: label,
          hintStyle: TextStyle(
            fontSize: 11,
            fontWeight: FontWeight.normal,
            color: Colors.grey.shade400,
          ),
          contentPadding: const EdgeInsets.symmetric(
            horizontal: 6,
            vertical: 8,
          ),
          filled: true,
          fillColor: enabled ? Colors.white : Colors.grey.shade100,
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(7),
            borderSide: BorderSide(color: Colors.grey.shade300),
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(7),
            borderSide: BorderSide(color: Colors.grey.shade300),
          ),
          focusedBorder: const OutlineInputBorder(
            borderRadius: BorderRadius.all(Radius.circular(7)),
            borderSide: BorderSide(color: Color(0xFF16A34A), width: 1.5),
          ),
        ),
      ),
    );
  }
}
