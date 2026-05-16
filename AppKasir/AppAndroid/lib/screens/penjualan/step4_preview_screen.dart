import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import 'package:share_plus/share_plus.dart';
import '../../providers/auth_provider.dart';
import '../../providers/penjualan_provider.dart';
import '../../services/api_service.dart';
import '../../services/thermal_print_service.dart';
import '../../services/nota_pdf_service.dart';
import '../../models/printer_config.dart';

class Step4PreviewScreen extends StatefulWidget {
  final VoidCallback onBack;
  const Step4PreviewScreen({super.key, required this.onBack});

  @override
  State<Step4PreviewScreen> createState() => _Step4PreviewScreenState();
}

class _Step4PreviewScreenState extends State<Step4PreviewScreen> {
  static const _green = Color(0xFF16A34A);
  final _fmt = NumberFormat('#,##0.##', 'id_ID');
  final _fmtTgl = DateFormat('dd/MM/yyyy HH:mm', 'id_ID');

  bool _isSaving = false;
  String? _savedId;

  Future<String?> _simpan() async {
    if (_savedId != null) return _savedId; // sudah tersimpan
    final prov = context.read<PenjualanProvider>();
    final auth = context.read<AuthProvider>();
    final hakAkses = auth.hakAkses;
    final lokasi = auth.selectedLocation ?? 'TOKO';

    // Validasi nominal nol
    if (!hakAkses.izinkanNominalNol && prov.grandTotal <= 0) {
      _showSnack('Total penjualan 0 tidak diizinkan', Colors.red);
      return null;
    }

    // Validasi rugi
    if (!hakAkses.izinkanJualRugi && prov.laba < 0) {
      _showSnack('Transaksi rugi tidak diizinkan', Colors.red);
      return null;
    }

    // ── Validasi stok real-time dari server ───────────────────
    if (!hakAkses.izinkanJualStokMinus) {
      setState(() => _isSaving = true);
      final stokOk = await _validasiStokRealtime(prov, lokasi);
      if (!mounted) return null;
      if (!stokOk) {
        setState(() => _isSaving = false);
        return null;
      }
    }

    setState(() => _isSaving = true);
    try {
      final payload = prov.buildPayload(
        idUser: auth.userFullName.isNotEmpty
            ? auth.userFullName
            : auth.userName,
        idKomputer: auth.deviceName,
        lokasi: lokasi,
      );
      final res = await ApiService.syncPenjualan(payload);
      if (res['status'] == 'success') {
        final id = res['id_penjualan']?.toString() ?? '';
        setState(() => _savedId = id);
        return id;
      } else {
        _showSnack(res['message'] ?? 'Gagal menyimpan', Colors.red);
        return null;
      }
    } catch (e) {
      _showSnack('Error: $e', Colors.red);
      return null;
    } finally {
      if (mounted) setState(() => _isSaving = false);
    }
  }

  /// Fetch stok terbaru dari server dan validasi semua item cart.
  /// Return false jika ada item yang stoknya tidak cukup.
  Future<bool> _validasiStokRealtime(
    PenjualanProvider prov,
    String lokasi,
  ) async {
    final stokKey = lokasi == 'GUDANG' ? 'STOK_GUDANG' : 'STOK_TOKO';
    try {
      // Ambil semua id barang yang unik
      final ids = prov.cartItems.map((e) => e.idBarang).toSet().toList();
      for (final idBarang in ids) {
        final res = await ApiService.getStock(search: idBarang, limit: 5);
        if (!mounted) return false;
        if (res['status'] != 'success') continue;
        final list = List<Map<String, dynamic>>.from(res['data'] ?? []);
        final found = list.firstWhere(
          (b) => b['ID_BARANG']?.toString() == idBarang,
          orElse: () => {},
        );
        if (found.isEmpty) continue;

        final stokServer =
            double.tryParse(found[stokKey]?.toString() ?? '0') ?? 0;

        // Hitung total qty satuan untuk barang ini di cart
        final totalQtySatuan = prov.cartItems
            .where((e) => e.idBarang == idBarang)
            .fold<double>(0, (s, e) => s + e.qty * e.isiSatuan);

        if (totalQtySatuan > stokServer) {
          final namaBarang = found['NAMA_BARANG']?.toString() ?? idBarang;
          _showSnack(
            'Stok $namaBarang tidak cukup '
            '(tersedia ${stokServer.toStringAsFixed(0)})',
            Colors.red,
          );
          return false;
        }
      }
      return true;
    } catch (e) {
      // Jika gagal fetch, loloskan — server akan validasi ulang
      debugPrint('[validasiStok] error: $e');
      return true;
    }
  }

  Future<void> _onSimpanSaja() async {
    final id = await _simpan();
    if (id == null) return;
    if (!mounted) return;
    _selesai(id);
  }

  Future<void> _onCetak() async {
    // Simpan referensi context sebelum semua await
    final auth = context.read<AuthProvider>();
    final prov = context.read<PenjualanProvider>();

    final id = await _simpan();
    if (id == null) return; // simpan gagal — tetap di halaman
    if (!mounted) return;

    // Load config printer
    final cfg = await PrinterConfig.load();
    if (cfg == null || !cfg.isConfigured) {
      if (!mounted) return;
      // Printer belum dikonfigurasi — transaksi sudah tersimpan, langsung keluar
      _selesai(id, pesanTambahan: 'Printer belum dikonfigurasi');
      return;
    }

    // Cetak nota
    final ok = await ThermalPrintService.printNota(
      prov: prov,
      auth: auth,
      cfg: cfg,
    );
    if (!mounted) return;

    if (ok) {
      _selesai(id, pesanTambahan: 'Nota dicetak');
    } else {
      // Cetak gagal — transaksi sudah tersimpan, tetap keluar
      _selesai(id, pesanTambahan: 'Gagal cetak, transaksi tersimpan');
    }
  }

  /// Navigasi ke list penjualan setelah transaksi selesai
  void _selesai(String id, {String? pesanTambahan}) {
    context.read<PenjualanProvider>().reset();
    Navigator.of(context).popUntil((r) => r.isFirst);
    final pesan = pesanTambahan != null
        ? 'Transaksi $id tersimpan. $pesanTambahan'
        : 'Transaksi $id berhasil disimpan';
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(pesan),
        backgroundColor: _green,
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  Future<void> _onKirimWA() async {
    final id = await _simpan();
    if (id == null) return;
    if (!mounted) return;

    final auth = context.read<AuthProvider>();
    final prov = context.read<PenjualanProvider>();

    setState(() => _isSaving = true);
    try {
      // Load config printer agar model nota konsisten dengan cetak thermal
      final cfg = await PrinterConfig.load();

      final file = await NotaPdfService.buildAndSave(
        prov: prov,
        auth: auth,
        cfg: cfg, // null = pakai default model 1, 58mm
        idPenjualan: id,
      );
      if (!mounted) return;

      await SharePlus.instance.share(
        ShareParams(
          files: [XFile(file.path, mimeType: 'application/pdf')],
          subject: 'Nota Penjualan $id',
        ),
      );
    } catch (e) {
      if (mounted) _showSnack('Gagal membuat PDF: $e', Colors.red);
      return;
    } finally {
      if (mounted) setState(() => _isSaving = false);
    }

    if (!mounted) return;
    _selesai(id, pesanTambahan: 'Nota dikirim');
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

  Widget _roundBtn({
    required IconData icon,
    required String label,
    required Color color,
    required VoidCallback onTap,
    bool large = false,
  }) {
    final size = large ? 64.0 : 56.0;
    return GestureDetector(
      onTap: onTap,
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: size,
            height: size,
            decoration: BoxDecoration(
              color: color,
              shape: BoxShape.circle,
              boxShadow: [
                BoxShadow(
                  color: color.withValues(alpha: 0.35),
                  blurRadius: 8,
                  offset: const Offset(0, 3),
                ),
              ],
            ),
            child: Icon(icon, color: Colors.white, size: large ? 28 : 24),
          ),
          const SizedBox(height: 5),
          Text(
            label,
            style: TextStyle(
              fontSize: 11,
              fontWeight: FontWeight.w600,
              color: color,
            ),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final prov = context.watch<PenjualanProvider>();
    final auth = context.watch<AuthProvider>();
    final hak = auth.hakAkses;

    // ── DEBUG: log nilai hak akses setiap rebuild ─────────────
    debugPrint('[HakAkses] izinkanUbahHarga     : ${hak.izinkanUbahHarga}');
    debugPrint('[HakAkses] izinkanJualRugi       : ${hak.izinkanJualRugi}');
    debugPrint(
      '[HakAkses] izinkanJualStokMinus  : ${hak.izinkanJualStokMinus}',
    );
    debugPrint(
      '[HakAkses] izinkanSatuanBerbeda  : ${hak.izinkanSatuanBerbeda}',
    );
    debugPrint('[HakAkses] tampilInfoStok        : ${hak.tampilInfoStok}');
    debugPrint('[HakAkses] langsungIsiNominal    : ${hak.langsungIsiNominal}');
    debugPrint('[HakAkses] izinkanNominalNol     : ${hak.izinkanNominalNol}');
    debugPrint(
      '[HakAkses] izinkanTanggalLampau  : ${hak.izinkanTanggalLampau}',
    );

    return Column(
      children: [
        Expanded(
          child: SingleChildScrollView(
            padding: const EdgeInsets.fromLTRB(16, 16, 16, 20),
            child: _NotaPreview(
              prov: prov,
              auth: auth,
              fmt: _fmt,
              fmtTgl: _fmtTgl,
            ),
          ),
        ),
        // ── Tombol aksi ─────────────────────────────────────────
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
            child: _isSaving
                ? const Center(
                    child: Padding(
                      padding: EdgeInsets.all(12),
                      child: CircularProgressIndicator(color: _green),
                    ),
                  )
                : Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      if (_savedId != null)
                        Padding(
                          padding: const EdgeInsets.only(bottom: 8),
                          child: Text(
                            'Tersimpan: $_savedId',
                            style: const TextStyle(
                              fontSize: 12,
                              color: _green,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                        children: [
                          _roundBtn(
                            icon: Icons.print_outlined,
                            label: 'Cetak',
                            color: _green,
                            onTap: _onCetak,
                          ),
                          _roundBtn(
                            icon: Icons.send_outlined,
                            label: 'Kirim',
                            color: Colors.blue.shade600,
                            onTap: _onKirimWA,
                          ),
                          if (_savedId == null)
                            _roundBtn(
                              icon: Icons.save_outlined,
                              label: 'Simpan',
                              color: Colors.orange.shade700,
                              onTap: _onSimpanSaja,
                              large: true,
                            ),
                        ],
                      ),
                    ],
                  ),
          ),
        ),
      ],
    );
  }
}

// ── Widget preview nota ───────────────────────────────────────────────────
class _NotaPreview extends StatelessWidget {
  final PenjualanProvider prov;
  final AuthProvider auth;
  final NumberFormat fmt;
  final DateFormat fmtTgl;

  const _NotaPreview({
    required this.prov,
    required this.auth,
    required this.fmt,
    required this.fmtTgl,
  });

  static const _green = Color(0xFF16A34A);

  @override
  Widget build(BuildContext context) {
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
            if (perusahaan.hp.isNotEmpty)
              Text(
                'Telp: ${perusahaan.hp}',
                style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
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
          _row('Tanggal', fmtTgl.format(prov.tanggal)),
          _row(
            'Kasir',
            auth.userFullName.isNotEmpty ? auth.userFullName : auth.userName,
          ),
          _row('Lokasi', auth.selectedLocation ?? 'TOKO'),
          if (prov.selectedPelanggan != null)
            _row(
              'Pelanggan',
              prov.selectedPelanggan!['NAMA']?.toString() ?? '',
            ),
          if (prov.selectedSales != null)
            _row('Sales', prov.selectedSales!['Nama']?.toString() ?? ''),
          const SizedBox(height: 8),
          const Divider(),
          const SizedBox(height: 4),

          // Header kolom
          Row(
            children: const [
              Expanded(
                flex: 4,
                child: Text(
                  'Barang',
                  style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold),
                ),
              ),
              Expanded(
                flex: 2,
                child: Text(
                  'Qty',
                  style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold),
                  textAlign: TextAlign.center,
                ),
              ),
              Expanded(
                flex: 3,
                child: Text(
                  'Harga',
                  style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold),
                  textAlign: TextAlign.right,
                ),
              ),
              Expanded(
                flex: 3,
                child: Text(
                  'Total',
                  style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold),
                  textAlign: TextAlign.right,
                ),
              ),
            ],
          ),
          const Divider(height: 8),

          // Item
          ...prov.cartItems.map(
            (item) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 3),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        flex: 4,
                        child: Text(
                          item.namaBarang,
                          style: const TextStyle(fontSize: 12),
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      Expanded(
                        flex: 2,
                        child: Text(
                          '${fmt.format(item.qty)} ${item.satuan}',
                          style: const TextStyle(fontSize: 11),
                          textAlign: TextAlign.center,
                        ),
                      ),
                      Expanded(
                        flex: 3,
                        child: Text(
                          fmt.format(item.hargaJual),
                          style: const TextStyle(fontSize: 11),
                          textAlign: TextAlign.right,
                        ),
                      ),
                      Expanded(
                        flex: 3,
                        child: Text(
                          fmt.format(item.totalHarga),
                          style: const TextStyle(
                            fontSize: 11,
                            fontWeight: FontWeight.w600,
                          ),
                          textAlign: TextAlign.right,
                        ),
                      ),
                    ],
                  ),
                  if (item.totalDiskon > 0)
                    Padding(
                      padding: const EdgeInsets.only(left: 4),
                      child: Text(
                        'Disc: -${fmt.format(item.totalDiskon)}',
                        style: TextStyle(
                          fontSize: 10,
                          color: Colors.red.shade400,
                        ),
                      ),
                    ),
                ],
              ),
            ),
          ),

          const Divider(height: 12),

          // Totals
          _row('Subtotal', 'Rp ${fmt.format(prov.subtotal)}'),
          if (prov.diskonGlobalRp > 0)
            _row(
              'Diskon',
              '- Rp ${fmt.format(prov.diskonGlobalRp)}',
              valueColor: Colors.red,
            ),
          if (prov.pajakNominal > 0)
            _row(
              'Pajak',
              '+ Rp ${fmt.format(prov.pajakNominal)}',
              valueColor: Colors.orange,
            ),
          if (prov.biayaKirim > 0)
            _row('Biaya Kirim', '+ Rp ${fmt.format(prov.biayaKirim)}'),
          const Divider(height: 8),
          _row(
            'GRAND TOTAL',
            'Rp ${fmt.format(prov.grandTotal)}',
            bold: true,
            valueColor: _green,
          ),
          const SizedBox(height: 6),
          if (prov.nominalTunai > 0)
            _row('Tunai', 'Rp ${fmt.format(prov.nominalTunai)}'),
          if (prov.nominalTransfer > 0)
            _row('Transfer', 'Rp ${fmt.format(prov.nominalTransfer)}'),
          if (prov.isLunas && prov.kembali > 0)
            _row(
              'Kembali',
              'Rp ${fmt.format(prov.kembali)}',
              valueColor: _green,
            ),
          if (!prov.isLunas)
            _row(
              'Sisa Tagihan',
              'Rp ${fmt.format(prov.sisaTagihan)}',
              valueColor: Colors.orange,
              bold: true,
            ),

          // Info bank
          if (prov.nominalTransfer > 0 && prov.bank.isNotEmpty) ...[
            const SizedBox(height: 8),
            const Divider(),
            _row('Bank', prov.bank),
            if (prov.noRek.isNotEmpty) _row('No. Rek', prov.noRek),
            if (prov.namaRek.isNotEmpty) _row('Nama Rek', prov.namaRek),
            if (prov.noRef.isNotEmpty) _row('No. Ref', prov.noRef),
          ],

          // Footer
          if (perusahaan != null && perusahaan.footer1.isNotEmpty) ...[
            const SizedBox(height: 10),
            const Divider(),
            Text(
              perusahaan.footer1,
              style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
              textAlign: TextAlign.center,
            ),
            if (perusahaan.footer2.isNotEmpty)
              Text(
                perusahaan.footer2,
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
  }) {
    return Padding(
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
}
