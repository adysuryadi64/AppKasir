import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../providers/auth_provider.dart';
import '../providers/penjualan_provider.dart';
import '../models/printer_config.dart';

/// NotaWidget — render nota sebagai widget Flutter dengan font monospace.
/// Layout identik dengan cetakan thermal (persentase kolom).
class NotaWidget extends StatelessWidget {
  final PenjualanProvider prov;
  final AuthProvider auth;
  final PrinterConfig cfg;

  const NotaWidget({
    super.key,
    required this.prov,
    required this.auth,
    required this.cfg,
  });

  @override
  Widget build(BuildContext context) {
    final fmt = NumberFormat('#,##0', 'id_ID');
    final fmtTgl = DateFormat('dd/MM/yyyy HH:mm', 'id_ID');
    final perusahaan = auth.perusahaan;
    final w = cfg.charsPerLine;
    final showDiskon =
        cfg.model == 1 || cfg.model == 2 || cfg.model == 5 || cfg.model == 6;
    final showHeader = cfg.model <= 4;
    final showSisaHutang =
        cfg.model == 1 || cfg.model == 3 || cfg.model == 5 || cfg.model == 7;

    return Container(
      padding: const EdgeInsets.all(12),
      color: Colors.white,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // ── Header toko ──────────────────────────────────────
          if (perusahaan != null) ...[
            Center(
              child: Text(
                perusahaan.nama,
                style: const TextStyle(
                  fontFamily: 'monospace',
                  fontWeight: FontWeight.bold,
                  fontSize: 14,
                ),
              ),
            ),
            if (perusahaan.alamat.isNotEmpty)
              Center(child: Text(perusahaan.alamat, style: _mono(11))),
            if (perusahaan.hp.isNotEmpty)
              Center(child: Text('Telp: ${perusahaan.hp}', style: _mono(11))),
          ] else
            Center(child: Text('KASIR LANCAR', style: _mono(14, bold: true))),
          _hr('=', w),

          // ── Info transaksi ───────────────────────────────────
          _row('Tgl', fmtTgl.format(prov.tanggal), w),
          _row(
            'Kasir',
            auth.userFullName.isNotEmpty ? auth.userFullName : auth.userName,
            w,
          ),
          _row('Lokasi', auth.selectedLocation ?? 'TOKO', w),
          if (prov.selectedPelanggan != null)
            _row(
              'Pelanggan',
              prov.selectedPelanggan!['NAMA']?.toString() ?? '',
              w,
            ),
          if (prov.selectedSales != null)
            _row('Sales', prov.selectedSales!['Nama']?.toString() ?? '', w),
          _hr('-', w),

          // ── Header kolom ─────────────────────────────────────
          if (showHeader) ...[
            Text(_headerKolom(w, showDiskon), style: _mono(11, bold: true)),
            _hr('-', w),
          ],

          // ── Item ─────────────────────────────────────────────
          ...prov.cartItems.map(
            (item) => Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(item.namaBarang, style: _mono(12)),
                Text(
                  _itemRow(
                    qty: '${fmt.format(item.qty)} ${item.satuan}',
                    harga: fmt.format(item.hargaJual),
                    diskon: showDiskon && item.totalDiskon > 0
                        ? fmt.format(item.totalDiskon)
                        : '',
                    total: fmt.format(item.totalHarga),
                    w: w,
                    showDiskon: showDiskon,
                  ),
                  style: _mono(11),
                ),
              ],
            ),
          ),
          _hr('-', w),

          // ── Total ────────────────────────────────────────────
          _row('Subtotal', 'Rp ${fmt.format(prov.subtotal)}', w),
          if (prov.diskonGlobalRp > 0)
            _row('Diskon', '-Rp ${fmt.format(prov.diskonGlobalRp)}', w),
          if (prov.pajakNominal > 0)
            _row('Pajak', '+Rp ${fmt.format(prov.pajakNominal)}', w),
          if (prov.biayaKirim > 0)
            _row('Kirim', '+Rp ${fmt.format(prov.biayaKirim)}', w),
          _hr('=', w),
          Text(
            _buildRow('TOTAL', 'Rp ${fmt.format(prov.grandTotal)}', w),
            style: _mono(12, bold: true),
          ),
          _hr('=', w),

          // ── Pembayaran ───────────────────────────────────────
          if (prov.nominalTunai > 0)
            _row('Tunai', 'Rp ${fmt.format(prov.nominalTunai)}', w),
          if (prov.nominalTransfer > 0)
            _row('Transfer', 'Rp ${fmt.format(prov.nominalTransfer)}', w),
          if (prov.isLunas && prov.kembali > 0)
            _row('Kembali', 'Rp ${fmt.format(prov.kembali)}', w),

          // ── Sisa hutang ──────────────────────────────────────
          if (showSisaHutang && !prov.isLunas) ...[
            _hr('-', w),
            Text(
              _buildRow('Sisa Hutang', 'Rp ${fmt.format(prov.sisaTagihan)}', w),
              style: _mono(12, bold: true),
            ),
          ],

          // ── Info bank ────────────────────────────────────────
          if (prov.nominalTransfer > 0 && prov.bank.isNotEmpty) ...[
            _hr('-', w),
            _row('Bank', prov.bank, w),
            if (prov.noRek.isNotEmpty) _row('No.Rek', prov.noRek, w),
            if (prov.namaRek.isNotEmpty) _row('Nama', prov.namaRek, w),
            if (prov.noRef.isNotEmpty) _row('Ref', prov.noRef, w),
          ],

          // ── Footer ───────────────────────────────────────────
          if (perusahaan != null && perusahaan.footer1.isNotEmpty) ...[
            _hr('=', w),
            Center(child: Text(perusahaan.footer1, style: _mono(11))),
            if (perusahaan.footer2.isNotEmpty)
              Center(child: Text(perusahaan.footer2, style: _mono(11))),
            if (perusahaan.footer3.isNotEmpty)
              Center(child: Text(perusahaan.footer3, style: _mono(11))),
          ],
        ],
      ),
    );
  }

  TextStyle _mono(double size, {bool bold = false}) => TextStyle(
    fontFamily: 'monospace',
    fontSize: size,
    fontWeight: bold ? FontWeight.bold : FontWeight.normal,
  );

  Widget _hr(String ch, int w) => Text(ch * w, style: _mono(11));

  Widget _row(String left, String right, int w) =>
      Text(_buildRow(left, right, w), style: _mono(11));

  String _buildRow(String left, String right, int w) {
    final maxLeft = w - right.length - 1;
    final l = left.length > maxLeft ? left.substring(0, maxLeft) : left;
    final spaces = w - l.length - right.length;
    return l + (' ' * (spaces > 0 ? spaces : 1)) + right;
  }

  String _headerKolom(int w, bool showDiskon) {
    if (showDiskon) {
      final qtyW = (w * 0.11).round();
      final hargaW = (w * 0.40).round();
      final discW = (w * 0.19).round();
      final jmlW = w - qtyW - hargaW - discW;
      return 'Qty'.padRight(qtyW) +
          'Harga'.padRight(hargaW) +
          'Disc'.padRight(discW) +
          'Jml'.padLeft(jmlW);
    } else {
      final qtyW = (w * 0.11).round();
      final hargaW = (w * 0.54).round();
      final jmlW = w - qtyW - hargaW;
      return 'Qty'.padRight(qtyW) +
          'Harga'.padRight(hargaW) +
          'Jml'.padLeft(jmlW);
    }
  }

  String _itemRow({
    required String qty,
    required String harga,
    required String diskon,
    required String total,
    required int w,
    required bool showDiskon,
  }) {
    if (showDiskon) {
      final qtyW = (w * 0.11).round();
      final hargaW = (w * 0.40).round();
      final discW = (w * 0.19).round();
      final jmlW = w - qtyW - hargaW - discW;
      return qty.padRight(qtyW) +
          harga.padRight(hargaW) +
          (diskon.isEmpty ? '' : diskon).padRight(discW) +
          total.padLeft(jmlW);
    } else {
      final qtyW = (w * 0.11).round();
      final hargaW = (w * 0.54).round();
      final jmlW = w - qtyW - hargaW;
      return qty.padRight(qtyW) + harga.padRight(hargaW) + total.padLeft(jmlW);
    }
  }
}
