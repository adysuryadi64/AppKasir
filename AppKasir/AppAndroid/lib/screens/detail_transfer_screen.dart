import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import 'package:share_plus/share_plus.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../models/printer_config.dart';
import 'printer_settings_screen.dart';

class DetailTransferScreen extends StatefulWidget {
  final String idTransfer;
  const DetailTransferScreen({super.key, required this.idTransfer});

  @override
  State<DetailTransferScreen> createState() => _DetailTransferScreenState();
}

class _DetailTransferScreenState extends State<DetailTransferScreen> {
  static const _teal = Color(0xFF0D9488);
  final _fmt = NumberFormat('#,##0.##', 'id_ID');
  final _fmtTgl = DateFormat('dd/MM/yyyy HH:mm', 'id_ID');

  Map<String, dynamic>? _data;
  bool _isLoading = true;
  bool _hasError = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _hasError = false;
    });
    try {
      // Ambil dari list dengan filter ID — reuse endpoint yang sudah ada
      final res = await ApiService.getTransferList(
        search: widget.idTransfer,
        limit: 1,
      );
      if (!mounted) return;
      if (res['status'] == 'success') {
        final list = List<Map<String, dynamic>>.from(res['data'] ?? []);
        // Cari yang ID-nya persis sama
        final found = list.firstWhere(
          (e) => e['ID_TRANSFER']?.toString() == widget.idTransfer,
          orElse: () => list.isNotEmpty ? list.first : {},
        );
        setState(() => _data = found.isNotEmpty ? found : null);
        if (_data == null) setState(() => _hasError = true);
      } else {
        setState(() => _hasError = true);
      }
    } catch (e) {
      debugPrint('[DetailTransfer] ❌ load error: $e');
      if (mounted) setState(() => _hasError = true);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _onCetak() async {
    final cfg = await PrinterConfig.load();
    if (!mounted) return;
    if (cfg == null || !cfg.isConfigured) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: const Text('Printer belum dikonfigurasi'),
          backgroundColor: Colors.orange,
          action: SnackBarAction(
            label: 'Atur',
            onPressed: () => Navigator.of(context).push(
              MaterialPageRoute(builder: (_) => const PrinterSettingsScreen()),
            ),
          ),
        ),
      );
      return;
    }
    final auth = context.read<AuthProvider>();
    final teks = _buildNotaTeks(auth);
    await SharePlus.instance.share(
      ShareParams(text: teks, subject: 'Transfer ${widget.idTransfer}'),
    );
    _showSnack('Nota dibagikan', _teal);
  }

  Future<void> _onKirimWA() async {
    final auth = context.read<AuthProvider>();
    final teks = _buildNotaTeks(auth);
    await SharePlus.instance.share(
      ShareParams(text: teks, subject: 'Transfer ${widget.idTransfer}'),
    );
  }

  String _buildNotaTeks(AuthProvider auth) {
    if (_data == null) return '';
    final d = _data!;
    final perusahaan = auth.perusahaan;
    final sb = StringBuffer();

    if (perusahaan != null) {
      sb.writeln('*${perusahaan.nama}*');
      if (perusahaan.alamat.isNotEmpty) sb.writeln(perusahaan.alamat);
    }
    sb.writeln('================================');
    sb.writeln('Transfer : ${d['ID_TRANSFER']}');
    try {
      sb.writeln(
        'Tgl      : ${_fmtTgl.format(DateTime.parse(d['TANGGAL']?.toString() ?? ''))}',
      );
    } catch (_) {
      sb.writeln('Tgl      : ${d['TANGGAL']}');
    }
    sb.writeln('Kasir    : ${d['ID_USER']}');
    sb.writeln('Lokasi   : ${d['LOKASI']}');
    if ((d['URAIAN']?.toString() ?? '').isNotEmpty) {
      sb.writeln('Uraian   : ${d['URAIAN']}');
    }
    sb.writeln('--------------------------------');
    sb.writeln('BARANG KELUAR:');
    sb.writeln(d['NAMA_BARANG_K'] ?? '');
    final qtyK = double.tryParse(d['QTY_K']?.toString() ?? '0') ?? 0;
    final totK = double.tryParse(d['TOTAL_HARGA_K']?.toString() ?? '0') ?? 0;
    sb.writeln(
      '  ${_fmt.format(qtyK)} ${d['SATUAN_K']}  =  Rp ${_fmt.format(totK)}',
    );
    sb.writeln('BARANG MASUK:');
    sb.writeln(d['NAMA_BARANG_M'] ?? '');
    final qtyM = double.tryParse(d['QTY_M']?.toString() ?? '0') ?? 0;
    final totM = double.tryParse(d['TOTAL_HARGA_M']?.toString() ?? '0') ?? 0;
    sb.writeln(
      '  ${_fmt.format(qtyM)} ${d['SATUAN_M']}  =  Rp ${_fmt.format(totM)}',
    );
    final selisih = double.tryParse(d['Selisih']?.toString() ?? '0') ?? 0;
    if (selisih != 0) {
      sb.writeln(
        'Selisih  : ${selisih >= 0 ? '+' : ''}${_fmt.format(selisih)}',
      );
    }
    if (perusahaan != null && perusahaan.footer1.isNotEmpty) {
      sb.writeln('================================');
      sb.writeln(perusahaan.footer1);
    }
    return sb.toString();
  }

  void _showSnack(String msg, Color color) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(msg),
        backgroundColor: color,
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF0FDF4),
      appBar: AppBar(backgroundColor: _teal, title: Text(widget.idTransfer)),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _teal))
          : _hasError || _data == null
          ? Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(Icons.wifi_off, size: 56, color: Colors.grey.shade300),
                  const SizedBox(height: 14),
                  ElevatedButton.icon(
                    onPressed: _load,
                    icon: const Icon(Icons.refresh),
                    label: const Text('Coba Lagi'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: _teal,
                      foregroundColor: Colors.white,
                    ),
                  ),
                ],
              ),
            )
          : Column(
              children: [
                Expanded(
                  child: SingleChildScrollView(
                    padding: const EdgeInsets.fromLTRB(16, 16, 16, 20),
                    child: _buildNota(),
                  ),
                ),
                // ── Tombol aksi ─────────────────────────────────
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
                          child: OutlinedButton.icon(
                            onPressed: _onCetak,
                            icon: const Icon(Icons.print_outlined, size: 18),
                            label: const Text('Cetak'),
                            style: OutlinedButton.styleFrom(
                              foregroundColor: _teal,
                              side: const BorderSide(color: _teal),
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Expanded(
                          child: OutlinedButton.icon(
                            onPressed: _onKirimWA,
                            icon: const Icon(Icons.message_outlined, size: 18),
                            label: const Text('Kirim WA'),
                            style: OutlinedButton.styleFrom(
                              foregroundColor: _teal,
                              side: const BorderSide(color: _teal),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
    );
  }

  Widget _buildNota() {
    final d = _data!;
    final auth = context.read<AuthProvider>();
    final perusahaan = auth.perusahaan;

    String tglFmt = d['TANGGAL']?.toString() ?? '';
    try {
      tglFmt = _fmtTgl.format(DateTime.parse(tglFmt));
    } catch (_) {}

    final qtyK = double.tryParse(d['QTY_K']?.toString() ?? '0') ?? 0;
    final totK = double.tryParse(d['TOTAL_HARGA_K']?.toString() ?? '0') ?? 0;
    final qtyM = double.tryParse(d['QTY_M']?.toString() ?? '0') ?? 0;
    final totM = double.tryParse(d['TOTAL_HARGA_M']?.toString() ?? '0') ?? 0;
    final selisih = double.tryParse(d['Selisih']?.toString() ?? '0') ?? 0;
    final selisihColor = selisih == 0
        ? Colors.grey.shade500
        : selisih > 0
        ? Colors.green.shade700
        : Colors.red.shade700;

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: Colors.grey.shade200),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          // ── Header toko ──────────────────────────────────────
          if (perusahaan != null) ...[
            Text(
              perusahaan.nama,
              style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
              textAlign: TextAlign.center,
            ),
            if (perusahaan.alamat.isNotEmpty)
              Text(
                perusahaan.alamat,
                style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                textAlign: TextAlign.center,
              ),
          ] else
            const Text(
              'KASIR LANCAR',
              style: TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
            ),
          const SizedBox(height: 8),
          const Divider(),
          const SizedBox(height: 4),

          // ── Info transaksi ───────────────────────────────────
          _row('Transfer', d['ID_TRANSFER']?.toString() ?? ''),
          _row('Tanggal', tglFmt),
          _row('Kasir', d['ID_USER']?.toString() ?? ''),
          _row('Lokasi', d['LOKASI']?.toString() ?? ''),
          if ((d['URAIAN']?.toString() ?? '').isNotEmpty)
            _row('Uraian', d['URAIAN']?.toString() ?? ''),
          const SizedBox(height: 8),
          const Divider(),
          const SizedBox(height: 6),

          // ── Barang Keluar ────────────────────────────────────
          _sectionLabel('BARANG KELUAR', Colors.red.shade700),
          const SizedBox(height: 6),
          _itemBox(
            nama: d['NAMA_BARANG_K']?.toString() ?? '—',
            qty: qtyK,
            satuan: d['SATUAN_K']?.toString() ?? '',
            total: totK,
            color: Colors.red.shade700,
          ),
          const SizedBox(height: 10),

          // Panah
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(Icons.arrow_downward_rounded, color: _teal, size: 20),
              const SizedBox(width: 6),
              Text(
                'Dikonversi menjadi',
                style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
              ),
              const SizedBox(width: 6),
              Icon(Icons.arrow_downward_rounded, color: _teal, size: 20),
            ],
          ),
          const SizedBox(height: 10),

          // ── Barang Masuk ─────────────────────────────────────
          _sectionLabel('BARANG MASUK', _teal),
          const SizedBox(height: 6),
          _itemBox(
            nama: d['NAMA_BARANG_M']?.toString() ?? '—',
            qty: qtyM,
            satuan: d['SATUAN_M']?.toString() ?? '',
            total: totM,
            color: _teal,
          ),

          // ── Selisih ──────────────────────────────────────────
          if (selisih != 0) ...[
            const SizedBox(height: 10),
            const Divider(height: 8),
            _row(
              'Selisih',
              '${selisih >= 0 ? '+' : ''}${_fmt.format(selisih)}',
              valueColor: selisihColor,
              bold: true,
            ),
          ],

          // ── Footer ───────────────────────────────────────────
          if (perusahaan != null && perusahaan.footer1.isNotEmpty) ...[
            const SizedBox(height: 10),
            const Divider(),
            Text(
              perusahaan.footer1,
              style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
              textAlign: TextAlign.center,
            ),
          ],
        ],
      ),
    );
  }

  Widget _sectionLabel(String text, Color color) => Align(
    alignment: Alignment.centerLeft,
    child: Text(
      text,
      style: TextStyle(
        fontSize: 10,
        fontWeight: FontWeight.w700,
        color: color,
        letterSpacing: 0.8,
      ),
    ),
  );

  Widget _itemBox({
    required String nama,
    required double qty,
    required String satuan,
    required double total,
    required Color color,
  }) => Container(
    width: double.infinity,
    padding: const EdgeInsets.all(12),
    decoration: BoxDecoration(
      color: color.withValues(alpha: 0.05),
      borderRadius: BorderRadius.circular(10),
      border: Border.all(color: color.withValues(alpha: 0.2)),
    ),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          nama,
          style: const TextStyle(
            fontSize: 13,
            fontWeight: FontWeight.w600,
            color: Color(0xFF1E293B),
          ),
        ),
        const SizedBox(height: 4),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              '${_fmt.format(qty)} $satuan',
              style: TextStyle(fontSize: 12, color: color),
            ),
            Text(
              'Rp ${_fmt.format(total)}',
              style: TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.bold,
                color: color,
              ),
            ),
          ],
        ),
      ],
    ),
  );

  Widget _row(
    String label,
    String value, {
    bool bold = false,
    Color? valueColor,
  }) => Padding(
    padding: const EdgeInsets.symmetric(vertical: 2),
    child: Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          label,
          style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
        ),
        Text(
          value,
          style: TextStyle(
            fontSize: 12,
            fontWeight: bold ? FontWeight.bold : FontWeight.w500,
            color: valueColor ?? const Color(0xFF1E293B),
          ),
        ),
      ],
    ),
  );
}
