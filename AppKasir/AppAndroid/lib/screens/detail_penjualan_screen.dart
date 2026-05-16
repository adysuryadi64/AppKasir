import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import 'package:share_plus/share_plus.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';
import '../models/printer_config.dart';
import 'printer_settings_screen.dart';

class DetailPenjualanScreen extends StatefulWidget {
  final String idPenjualan;
  const DetailPenjualanScreen({super.key, required this.idPenjualan});

  @override
  State<DetailPenjualanScreen> createState() => _DetailPenjualanScreenState();
}

class _DetailPenjualanScreenState extends State<DetailPenjualanScreen> {
  static const _green = Color(0xFF16A34A);
  final _fmt = NumberFormat('#,##0.##', 'id_ID');
  final _fmtTgl = DateFormat('dd/MM/yyyy HH:mm', 'id_ID');

  Map<String, dynamic>? _header;
  List<Map<String, dynamic>> _items = [];
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
      final res = await ApiService.getDetailPenjualan(widget.idPenjualan);
      if (!mounted) return;
      if (res['status'] == 'success') {
        setState(() {
          _header = res['header'] as Map<String, dynamic>?;
          _items = List<Map<String, dynamic>>.from(res['items'] ?? []);
        });
      } else {
        setState(() => _hasError = true);
      }
    } catch (e) {
      debugPrint('[DetailPenjualan] ❌ load error: $e');
      if (mounted) setState(() => _hasError = true);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _onCetak() async {
    final auth = context.read<AuthProvider>();
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

    // Untuk saat ini, gunakan share saja karena koneksi bluetooth perlu setup lebih lanjut
    final teks = _buildNotaTeks(auth);
    await SharePlus.instance.share(
      ShareParams(text: teks, subject: 'Nota ${widget.idPenjualan}'),
    );
    _showSnack('Nota dibagikan', _green);
  }

  Future<void> _onKirimWA() async {
    final auth = context.read<AuthProvider>();
    final teks = _buildNotaTeks(auth);
    await SharePlus.instance.share(
      ShareParams(text: teks, subject: 'Nota ${widget.idPenjualan}'),
    );
  }

  String _buildNotaTeks(AuthProvider auth) {
    if (_header == null) return '';
    final h = _header!;
    final perusahaan = auth.perusahaan;
    final sb = StringBuffer();

    if (perusahaan != null) {
      sb.writeln('*${perusahaan.nama}*');
      if (perusahaan.alamat.isNotEmpty) sb.writeln(perusahaan.alamat);
    } else {
      sb.writeln('*KASIR LANCAR*');
    }
    sb.writeln('================================');
    sb.writeln('Nota  : ${h['ID_PENJUALAN']}');
    try {
      sb.writeln(
        'Tgl   : ${_fmtTgl.format(DateTime.parse(h['TGL_TRANSAKSI']?.toString() ?? ''))}',
      );
    } catch (_) {
      sb.writeln('Tgl   : ${h['TGL_TRANSAKSI']}');
    }
    sb.writeln('Kasir : ${h['ID_USER']}');
    if ((h['NAMA_PELANGGAN']?.toString() ?? '').isNotEmpty) {
      sb.writeln('Pelanggan: ${h['NAMA_PELANGGAN']}');
    }
    sb.writeln('--------------------------------');
    for (final item in _items) {
      sb.writeln(item['NAMA_BARANG'] ?? '');
      final qty = double.tryParse(item['QTY']?.toString() ?? '0') ?? 0;
      final harga = double.tryParse(item['HARGA_JUAL']?.toString() ?? '0') ?? 0;
      final total =
          double.tryParse(item['TOTAL_HARGA']?.toString() ?? '0') ?? 0;
      final diskon =
          double.tryParse(item['TOTAL_DISKON']?.toString() ?? '0') ?? 0;
      sb.writeln(
        '  ${_fmt.format(qty)} ${item['SATUAN']} x ${_fmt.format(harga)} = Rp ${_fmt.format(total)}',
      );
      if (diskon > 0) sb.writeln('  Disc: -Rp ${_fmt.format(diskon)}');
    }
    sb.writeln('================================');
    final grandTotal =
        double.tryParse(h['GRAND_TOTAL_STL_PAJAK']?.toString() ?? '0') ?? 0;
    final diskonGlobal =
        double.tryParse(h['DISKON_TOTAL_RP']?.toString() ?? '0') ?? 0;
    final pajak = double.tryParse(h['PAJAK_RP']?.toString() ?? '0') ?? 0;
    final kirim = double.tryParse(h['BIAYA_KIRIM']?.toString() ?? '0') ?? 0;
    final bayar = double.tryParse(h['BAYAR']?.toString() ?? '0') ?? 0;
    final transfer =
        double.tryParse(h['NOMINAL_TRANSFER']?.toString() ?? '0') ?? 0;
    final kembali = double.tryParse(h['KEMBALI']?.toString() ?? '0') ?? 0;
    final sisa = double.tryParse(h['SISA_TAGIHAN']?.toString() ?? '0') ?? 0;

    if (diskonGlobal > 0) {
      sb.writeln('Diskon   : -Rp ${_fmt.format(diskonGlobal)}');
    }
    if (pajak > 0) {
      sb.writeln('Pajak    : +Rp ${_fmt.format(pajak)}');
    }
    if (kirim > 0) {
      sb.writeln('Kirim    : +Rp ${_fmt.format(kirim)}');
    }
    sb.writeln('*TOTAL   : Rp ${_fmt.format(grandTotal)}*');
    if (bayar > 0) {
      sb.writeln('Tunai    : Rp ${_fmt.format(bayar)}');
    }
    if (transfer > 0) {
      sb.writeln('Transfer : Rp ${_fmt.format(transfer)}');
    }
    if (kembali > 0) {
      sb.writeln('Kembali  : Rp ${_fmt.format(kembali)}');
    }
    if (sisa > 0) {
      sb.writeln('*Hutang  : Rp ${_fmt.format(sisa)}*');
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
      appBar: AppBar(title: Text(widget.idPenjualan)),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator(color: _green))
          : _hasError
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
                          ),
                        ),
                        const SizedBox(width: 8),
                        Expanded(
                          child: OutlinedButton.icon(
                            onPressed: _onKirimWA,
                            icon: const Icon(Icons.message_outlined, size: 18),
                            label: const Text('Kirim WA'),
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
    if (_header == null) return const SizedBox.shrink();
    final h = _header!;

    String tglFmt = h['TGL_TRANSAKSI']?.toString() ?? '';
    try {
      tglFmt = _fmtTgl.format(DateTime.parse(tglFmt));
    } catch (_) {}

    final grandTotal =
        double.tryParse(h['GRAND_TOTAL_STL_PAJAK']?.toString() ?? '0') ?? 0;
    final diskonGlobal =
        double.tryParse(h['DISKON_TOTAL_RP']?.toString() ?? '0') ?? 0;
    final pajak = double.tryParse(h['PAJAK_RP']?.toString() ?? '0') ?? 0;
    final kirim = double.tryParse(h['BIAYA_KIRIM']?.toString() ?? '0') ?? 0;
    final bayar = double.tryParse(h['BAYAR']?.toString() ?? '0') ?? 0;
    final transfer =
        double.tryParse(h['NOMINAL_TRANSFER']?.toString() ?? '0') ?? 0;
    final kembali = double.tryParse(h['KEMBALI']?.toString() ?? '0') ?? 0;
    final sisa = double.tryParse(h['SISA_TAGIHAN']?.toString() ?? '0') ?? 0;
    final isLunas = sisa == 0;

    final auth = context.read<AuthProvider>();
    final perusahaan = auth.perusahaan;

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
          // Header toko
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

          // Info transaksi
          _row('Nota', h['ID_PENJUALAN']?.toString() ?? ''),
          _row('Tanggal', tglFmt),
          _row('Kasir', h['ID_USER']?.toString() ?? ''),
          _row('Lokasi', h['LOKASIBARANG']?.toString() ?? ''),
          if ((h['NAMA_PELANGGAN']?.toString() ?? '').isNotEmpty)
            _row('Pelanggan', h['NAMA_PELANGGAN']?.toString() ?? ''),
          if ((h['NAMA_SALES']?.toString() ?? '').isNotEmpty)
            _row('Sales', h['NAMA_SALES']?.toString() ?? ''),
          const SizedBox(height: 8),
          const Divider(),
          const SizedBox(height: 4),

          // Items — nama barang baris sendiri full lebar, qty/harga/total di baris berikutnya
          ..._items.map((item) {
            final qty = double.tryParse(item['QTY']?.toString() ?? '0') ?? 0;
            final harga =
                double.tryParse(item['HARGA_JUAL']?.toString() ?? '0') ?? 0;
            final total =
                double.tryParse(item['TOTAL_HARGA']?.toString() ?? '0') ?? 0;
            final diskon =
                double.tryParse(item['TOTAL_DISKON']?.toString() ?? '0') ?? 0;
            return Padding(
              padding: const EdgeInsets.symmetric(vertical: 4),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Nama barang — full lebar
                  Text(
                    item['NAMA_BARANG'] ?? '',
                    style: const TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  const SizedBox(height: 2),
                  // Qty × Harga = Total
                  Row(
                    children: [
                      Text(
                        '${_fmt.format(qty)} ${item['SATUAN'] ?? ''}',
                        style: TextStyle(
                          fontSize: 11,
                          color: Colors.grey.shade600,
                        ),
                      ),
                      Text(
                        '  ×  ${_fmt.format(harga)}',
                        style: TextStyle(
                          fontSize: 11,
                          color: Colors.grey.shade600,
                        ),
                      ),
                      const Spacer(),
                      Text(
                        _fmt.format(total),
                        style: const TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
                  if (diskon > 0)
                    Text(
                      'Disc: -${_fmt.format(diskon)}',
                      style: TextStyle(
                        fontSize: 10,
                        color: Colors.red.shade400,
                      ),
                    ),
                ],
              ),
            );
          }),

          const Divider(height: 12),

          // Totals
          if (diskonGlobal > 0)
            _row(
              'Diskon',
              '- Rp ${_fmt.format(diskonGlobal)}',
              valueColor: Colors.red,
            ),
          if (pajak > 0)
            _row(
              'Pajak',
              '+ Rp ${_fmt.format(pajak)}',
              valueColor: Colors.orange,
            ),
          if (kirim > 0) _row('Biaya Kirim', '+ Rp ${_fmt.format(kirim)}'),
          const Divider(height: 8),
          _row(
            'GRAND TOTAL',
            'Rp ${_fmt.format(grandTotal)}',
            bold: true,
            valueColor: _green,
          ),
          const SizedBox(height: 6),
          if (bayar > 0) _row('Tunai', 'Rp ${_fmt.format(bayar)}'),
          if (transfer > 0) _row('Transfer', 'Rp ${_fmt.format(transfer)}'),
          if (isLunas && kembali > 0)
            _row('Kembali', 'Rp ${_fmt.format(kembali)}', valueColor: _green),
          if (!isLunas)
            _row(
              'Sisa Hutang',
              'Rp ${_fmt.format(sisa)}',
              valueColor: Colors.orange,
              bold: true,
            ),

          // Info bank
          if (transfer > 0 && (h['BANK']?.toString() ?? '').isNotEmpty) ...[
            const SizedBox(height: 8),
            const Divider(),
            _row('Bank', h['BANK']?.toString() ?? ''),
            if ((h['NO_REKENING']?.toString() ?? '').isNotEmpty)
              _row('No. Rek', h['NO_REKENING']?.toString() ?? ''),
            if ((h['NAMA_REKENING']?.toString() ?? '').isNotEmpty)
              _row('Nama Rek', h['NAMA_REKENING']?.toString() ?? ''),
            if ((h['NO_REFFERENSI']?.toString() ?? '').isNotEmpty)
              _row('No. Ref', h['NO_REFFERENSI']?.toString() ?? ''),
          ],

          // Footer — cukup 1 baris
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
