import 'dart:io';
import 'dart:typed_data';
import 'package:intl/intl.dart';
import 'package:path_provider/path_provider.dart';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import '../models/printer_config.dart';
import '../providers/auth_provider.dart';
import '../providers/penjualan_provider.dart';

/// NotaPdfService — generate PDF nota yang isinya identik dengan thermal print.
/// Menggunakan PrinterConfig (model, paperWidth) yang sama agar konsisten.
/// Jika PrinterConfig null, pakai default model=1, paperWidth=58.
class NotaPdfService {
  static final _fmt = NumberFormat('#,##0', 'id_ID');
  static final _fmtTgl = DateFormat('dd/MM/yyyy HH:mm');

  // ── Warna ─────────────────────────────────────────────────────
  static const _hijau = PdfColor.fromInt(0xFF16A34A);
  static const _putih = PdfColors.white;
  static const _abuBg = PdfColor.fromInt(0xFFF8FAFC);
  static const _abuGaris = PdfColor.fromInt(0xFFCBD5E1);
  static const _teksGelap = PdfColor.fromInt(0xFF1E293B);
  static const _teksAbu = PdfColor.fromInt(0xFF64748B);
  static const _merah = PdfColor.fromInt(0xFFDC2626);
  static const _oranye = PdfColor.fromInt(0xFFEA580C);

  // ── Ukuran halaman sesuai paperWidth ─────────────────────────
  // 58mm → lebar ~165pt, 80mm → lebar ~227pt, tinggi auto (panjang)
  static PdfPageFormat _pageFormat(int paperWidth) {
    final w = paperWidth == 80 ? 227.0 : 165.0;
    return PdfPageFormat(w, double.infinity, marginAll: 10);
  }

  /// Build PDF bytes — identik dengan buildNotaBytes di ThermalPrintService.
  static Future<Uint8List> buildBytes({
    required PenjualanProvider prov,
    required AuthProvider auth,
    PrinterConfig? cfg,
    String? idPenjualan,
  }) async {
    final model = cfg?.model ?? 1;
    final paperWidth = cfg?.paperWidth ?? 58;

    // Logika model sama persis dengan thermal
    final showDiskon = model == 1 || model == 2 || model == 5 || model == 6;
    final showHeaderKolom = model <= 4;
    final showSisaHutang = model == 1 || model == 3 || model == 5 || model == 7;

    final perusahaan = auth.perusahaan;
    final pdf = pw.Document();

    pdf.addPage(
      pw.Page(
        pageFormat: _pageFormat(paperWidth),
        build: (ctx) {
          final sections = <pw.Widget>[];

          // ── Header toko ──────────────────────────────────────
          if (perusahaan != null) {
            sections.add(
              _teksCenter(
                perusahaan.nama,
                fontSize: 13,
                bold: true,
                color: _hijau,
              ),
            );
            if (perusahaan.alamat.isNotEmpty) {
              sections.add(_teksCenter(perusahaan.alamat, fontSize: 8));
            }
            if (perusahaan.hp.isNotEmpty) {
              sections.add(_teksCenter('Telp: ${perusahaan.hp}', fontSize: 8));
            }
          } else {
            sections.add(
              _teksCenter(
                'KASIR LANCAR',
                fontSize: 13,
                bold: true,
                color: _hijau,
              ),
            );
          }
          sections.add(_hr(tebal: true));

          // ── Info transaksi ───────────────────────────────────
          if (idPenjualan != null && idPenjualan.isNotEmpty) {
            sections.add(_baris('No. Nota', idPenjualan, bold: true));
          }
          sections.add(_baris('Tgl', _fmtTgl.format(prov.tanggal)));
          sections.add(
            _baris(
              'Kasir',
              auth.userFullName.isNotEmpty ? auth.userFullName : auth.userName,
            ),
          );
          sections.add(_baris('Lokasi', auth.selectedLocation ?? 'TOKO'));
          if (prov.selectedPelanggan != null) {
            sections.add(
              _baris(
                'Pelanggan',
                prov.selectedPelanggan!['NAMA']?.toString() ?? '',
              ),
            );
          }
          if (prov.selectedSales != null) {
            sections.add(
              _baris('Sales', prov.selectedSales!['Nama']?.toString() ?? ''),
            );
          }
          sections.add(_hr());

          // ── Header kolom item (model 1-4) ────────────────────
          if (showHeaderKolom) {
            sections.add(_headerKolom(showDiskon));
            sections.add(_hr(tipis: true));
          }

          // ── Item ─────────────────────────────────────────────
          for (final item in prov.cartItems) {
            sections.add(_itemNama(item.namaBarang));
            sections.add(
              _itemBaris(
                qty: '${_fmt.format(item.qty)} ${item.satuan}',
                harga: _fmt.format(item.hargaJual),
                diskon: showDiskon && item.totalDiskon > 0
                    ? _fmt.format(item.totalDiskon)
                    : '',
                total: _fmt.format(item.totalHarga),
                showDiskon: showDiskon,
              ),
            );
          }
          sections.add(_hr());

          // ── Total ────────────────────────────────────────────
          sections.add(_baris('Subtotal', 'Rp ${_fmt.format(prov.subtotal)}'));
          if (prov.diskonGlobalRp > 0) {
            sections.add(
              _baris(
                'Diskon',
                '-Rp ${_fmt.format(prov.diskonGlobalRp)}',
                valueColor: _merah,
              ),
            );
          }
          if (prov.pajakNominal > 0) {
            sections.add(
              _baris(
                'Pajak',
                '+Rp ${_fmt.format(prov.pajakNominal)}',
                valueColor: _oranye,
              ),
            );
          }
          if (prov.biayaKirim > 0) {
            sections.add(
              _baris('Kirim', '+Rp ${_fmt.format(prov.biayaKirim)}'),
            );
          }
          sections.add(_hr(tebal: true));
          // Grand total — kotak hijau
          sections.add(_grandTotal('Rp ${_fmt.format(prov.grandTotal)}'));
          sections.add(_hr(tebal: true));

          // ── Pembayaran ───────────────────────────────────────
          if (prov.nominalTunai > 0) {
            sections.add(
              _baris('Tunai', 'Rp ${_fmt.format(prov.nominalTunai)}'),
            );
          }
          if (prov.nominalTransfer > 0) {
            sections.add(
              _baris('Transfer', 'Rp ${_fmt.format(prov.nominalTransfer)}'),
            );
          }
          if (prov.isLunas && prov.kembali > 0) {
            sections.add(
              _baris(
                'Kembali',
                'Rp ${_fmt.format(prov.kembali)}',
                valueColor: _hijau,
              ),
            );
          }

          // ── Sisa hutang (sesuai model) ───────────────────────
          if (showSisaHutang && !prov.isLunas) {
            sections.add(_hr(tipis: true));
            sections.add(
              _baris(
                'Sisa Hutang',
                'Rp ${_fmt.format(prov.sisaTagihan)}',
                bold: true,
                valueColor: _oranye,
              ),
            );
          }

          // ── Info bank ────────────────────────────────────────
          if (prov.nominalTransfer > 0 && prov.bank.isNotEmpty) {
            sections.add(_hr(tipis: true));
            sections.add(_baris('Bank', prov.bank));
            if (prov.noRek.isNotEmpty) {
              sections.add(_baris('No.Rek', prov.noRek));
            }
            if (prov.namaRek.isNotEmpty) {
              sections.add(_baris('Nama', prov.namaRek));
            }
            if (prov.noRef.isNotEmpty) {
              sections.add(_baris('Ref', prov.noRef));
            }
          }

          // ── Footer ───────────────────────────────────────────
          if (perusahaan != null) {
            sections.add(_hr(tebal: true));
            if (perusahaan.footer1.isNotEmpty) {
              sections.add(_teksCenter(perusahaan.footer1, fontSize: 8));
            }
            if (perusahaan.footer2.isNotEmpty) {
              sections.add(_teksCenter(perusahaan.footer2, fontSize: 8));
            }
            if (perusahaan.footer3.isNotEmpty) {
              sections.add(_teksCenter(perusahaan.footer3, fontSize: 8));
            }
          }

          return pw.Column(
            crossAxisAlignment: pw.CrossAxisAlignment.stretch,
            children: sections,
          );
        },
      ),
    );

    return pdf.save();
  }

  /// Build dan simpan ke temp file, kembalikan path.
  static Future<File> buildAndSave({
    required PenjualanProvider prov,
    required AuthProvider auth,
    PrinterConfig? cfg,
    String? idPenjualan,
  }) async {
    final bytes = await buildBytes(
      prov: prov,
      auth: auth,
      cfg: cfg,
      idPenjualan: idPenjualan,
    );
    final dir = await getTemporaryDirectory();
    final file = File('${dir.path}/nota_${idPenjualan ?? 'penjualan'}.pdf');
    await file.writeAsBytes(bytes);
    return file;
  }

  // ── Widget helpers ────────────────────────────────────────────

  /// Teks rata tengah
  static pw.Widget _teksCenter(
    String text, {
    double fontSize = 9,
    bool bold = false,
    PdfColor color = _teksGelap,
  }) {
    return pw.Padding(
      padding: const pw.EdgeInsets.symmetric(vertical: 1),
      child: pw.Text(
        text,
        textAlign: pw.TextAlign.center,
        style: pw.TextStyle(
          fontSize: fontSize,
          fontWeight: bold ? pw.FontWeight.bold : pw.FontWeight.normal,
          color: color,
        ),
      ),
    );
  }

  /// Baris kiri–kanan (label : value)
  static pw.Widget _baris(
    String label,
    String value, {
    bool bold = false,
    PdfColor? valueColor,
  }) {
    return pw.Padding(
      padding: const pw.EdgeInsets.symmetric(vertical: 1.2),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(label, style: pw.TextStyle(fontSize: 8, color: _teksAbu)),
          pw.Text(
            value,
            style: pw.TextStyle(
              fontSize: 8,
              fontWeight: bold ? pw.FontWeight.bold : pw.FontWeight.normal,
              color: valueColor ?? _teksGelap,
            ),
          ),
        ],
      ),
    );
  }

  /// Kotak grand total hijau
  static pw.Widget _grandTotal(String value) {
    return pw.Container(
      margin: const pw.EdgeInsets.symmetric(vertical: 3),
      padding: const pw.EdgeInsets.symmetric(horizontal: 6, vertical: 5),
      decoration: pw.BoxDecoration(
        color: _hijau,
        borderRadius: pw.BorderRadius.circular(3),
      ),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(
            'TOTAL',
            style: pw.TextStyle(
              fontSize: 10,
              fontWeight: pw.FontWeight.bold,
              color: _putih,
            ),
          ),
          pw.Text(
            value,
            style: pw.TextStyle(
              fontSize: 11,
              fontWeight: pw.FontWeight.bold,
              color: _putih,
            ),
          ),
        ],
      ),
    );
  }

  /// Header kolom item
  static pw.Widget _headerKolom(bool showDiskon) {
    return pw.Container(
      color: _abuBg,
      padding: const pw.EdgeInsets.symmetric(vertical: 2),
      child: showDiskon
          ? pw.Row(
              children: [
                pw.Expanded(flex: 4, child: _kolHead('Barang')),
                pw.SizedBox(width: 36, child: _kolHead('Qty', center: true)),
                pw.SizedBox(width: 40, child: _kolHead('Harga', right: true)),
                pw.SizedBox(width: 28, child: _kolHead('Disc', right: true)),
                pw.SizedBox(width: 40, child: _kolHead('Jml', right: true)),
              ],
            )
          : pw.Row(
              children: [
                pw.Expanded(flex: 4, child: _kolHead('Barang')),
                pw.SizedBox(width: 36, child: _kolHead('Qty', center: true)),
                pw.SizedBox(width: 40, child: _kolHead('Harga', right: true)),
                pw.SizedBox(width: 40, child: _kolHead('Jml', right: true)),
              ],
            ),
    );
  }

  static pw.Widget _kolHead(
    String t, {
    bool center = false,
    bool right = false,
  }) {
    return pw.Text(
      t,
      style: pw.TextStyle(
        fontSize: 7,
        fontWeight: pw.FontWeight.bold,
        color: _teksAbu,
      ),
      textAlign: right
          ? pw.TextAlign.right
          : center
          ? pw.TextAlign.center
          : pw.TextAlign.left,
    );
  }

  /// Nama barang (baris pertama item)
  static pw.Widget _itemNama(String nama) {
    return pw.Padding(
      padding: const pw.EdgeInsets.only(top: 3),
      child: pw.Text(nama, style: const pw.TextStyle(fontSize: 8), maxLines: 2),
    );
  }

  /// Baris angka item (qty · harga · diskon · total)
  static pw.Widget _itemBaris({
    required String qty,
    required String harga,
    required String diskon,
    required String total,
    required bool showDiskon,
  }) {
    return pw.Padding(
      padding: const pw.EdgeInsets.only(bottom: 2),
      child: showDiskon
          ? pw.Row(
              children: [
                pw.Expanded(flex: 4, child: pw.SizedBox()),
                pw.SizedBox(
                  width: 36,
                  child: pw.Text(
                    qty,
                    style: const pw.TextStyle(fontSize: 8),
                    textAlign: pw.TextAlign.center,
                  ),
                ),
                pw.SizedBox(
                  width: 40,
                  child: pw.Text(
                    harga,
                    style: const pw.TextStyle(fontSize: 8),
                    textAlign: pw.TextAlign.right,
                  ),
                ),
                pw.SizedBox(
                  width: 28,
                  child: pw.Text(
                    diskon,
                    style: pw.TextStyle(
                      fontSize: 8,
                      color: diskon.isEmpty ? _putih : _merah,
                    ),
                    textAlign: pw.TextAlign.right,
                  ),
                ),
                pw.SizedBox(
                  width: 40,
                  child: pw.Text(
                    total,
                    style: pw.TextStyle(
                      fontSize: 8,
                      fontWeight: pw.FontWeight.bold,
                    ),
                    textAlign: pw.TextAlign.right,
                  ),
                ),
              ],
            )
          : pw.Row(
              children: [
                pw.Expanded(flex: 4, child: pw.SizedBox()),
                pw.SizedBox(
                  width: 36,
                  child: pw.Text(
                    qty,
                    style: const pw.TextStyle(fontSize: 8),
                    textAlign: pw.TextAlign.center,
                  ),
                ),
                pw.SizedBox(
                  width: 40,
                  child: pw.Text(
                    harga,
                    style: const pw.TextStyle(fontSize: 8),
                    textAlign: pw.TextAlign.right,
                  ),
                ),
                pw.SizedBox(
                  width: 40,
                  child: pw.Text(
                    total,
                    style: pw.TextStyle(
                      fontSize: 8,
                      fontWeight: pw.FontWeight.bold,
                    ),
                    textAlign: pw.TextAlign.right,
                  ),
                ),
              ],
            ),
    );
  }

  /// Garis pembatas
  static pw.Widget _hr({bool tebal = false, bool tipis = false}) {
    return pw.Padding(
      padding: const pw.EdgeInsets.symmetric(vertical: 2),
      child: pw.Divider(
        color: _abuGaris,
        thickness: tebal ? 1.0 : (tipis ? 0.3 : 0.5),
      ),
    );
  }
}
