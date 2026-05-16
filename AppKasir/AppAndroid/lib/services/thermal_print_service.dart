import 'dart:async';
import 'dart:typed_data';
import 'package:flutter_thermal_printer/flutter_thermal_printer.dart';
import 'package:flutter_thermal_printer/utils/printer.dart';
import 'package:intl/intl.dart';
import '../models/printer_config.dart';
import '../providers/penjualan_provider.dart';
import '../providers/auth_provider.dart';

class ThermalPrintService {
  static final FlutterThermalPrinter _printer = FlutterThermalPrinter.instance;

  // ── Scan devices ─────────────────────────────────────────────
  static Stream<List<Printer>> get scanStream => _printer.devicesStream;

  static Future<void> startScan() async {
    await _printer.getPrinters(
      connectionTypes: [ConnectionType.BLE, ConnectionType.USB],
    );
  }

  static Future<void> stopScan() async {
    await _printer.stopScan();
  }

  // ── Connect / disconnect ──────────────────────────────────────
  static Future<bool> connect(Printer device) async {
    try {
      final connected = await _printer.connect(device);
      return connected;
    } catch (_) {
      return false;
    }
  }

  static Future<void> disconnect(Printer device) async {
    await _printer.disconnect(device);
  }

  // ── Print nota ────────────────────────────────────────────────
  static Future<bool> printNota({
    required PenjualanProvider prov,
    required AuthProvider auth,
    required PrinterConfig cfg,
  }) async {
    try {
      final bytes = await buildNotaBytes(prov: prov, auth: auth, cfg: cfg);
      if (bytes == null) return false;

      // Cari printer yang sudah dikonfigurasi
      final printers = await _printer.devicesStream.first;

      Printer? targetPrinter;
      for (final p in printers) {
        if (p.address == cfg.deviceAddress) {
          targetPrinter = p;
          break;
        }
      }

      if (targetPrinter == null) return false;

      // Connect dan print
      final connected = await _printer.connect(targetPrinter);
      if (connected != true) return false;

      await Future.delayed(const Duration(milliseconds: 500));

      // Print raw bytes - printData returns void, so we assume success if no exception
      await _printer.printData(targetPrinter, bytes);

      return true;
    } catch (_) {
      return false;
    }
  }

  // ── Build ESC/POS bytes ───────────────────────────────────────
  static Future<Uint8List?> buildNotaBytes({
    required PenjualanProvider prov,
    required AuthProvider auth,
    required PrinterConfig cfg,
  }) async {
    try {
      final profile = await CapabilityProfile.load();
      final generator = Generator(
        cfg.paperWidth == 80 ? PaperSize.mm80 : PaperSize.mm58,
        profile,
      );
      final fmt = NumberFormat('#,##0', 'id_ID');
      final fmtTgl = DateFormat('dd/MM/yyyy HH:mm');
      final perusahaan = auth.perusahaan;
      final w = cfg.charsPerLine; // 48 atau 32

      List<int> bytes = [];

      // ── Header toko ──────────────────────────────────────────
      if (perusahaan != null) {
        bytes += generator.text(
          perusahaan.nama,
          styles: const PosStyles(
            align: PosAlign.center,
            bold: true,
            height: PosTextSize.size2,
            width: PosTextSize.size2,
          ),
        );
        if (perusahaan.alamat.isNotEmpty) {
          bytes += generator.text(
            perusahaan.alamat,
            styles: const PosStyles(align: PosAlign.center),
          );
        }
        if (perusahaan.hp.isNotEmpty) {
          bytes += generator.text(
            'Telp: ${perusahaan.hp}',
            styles: const PosStyles(align: PosAlign.center),
          );
        }
      } else {
        bytes += generator.text(
          'KASIR LANCAR',
          styles: const PosStyles(
            align: PosAlign.center,
            bold: true,
            height: PosTextSize.size2,
            width: PosTextSize.size2,
          ),
        );
      }
      bytes += generator.hr(ch: '=');

      // ── Info transaksi ───────────────────────────────────────
      bytes += generator.text(_buildRow('Tgl', fmtTgl.format(prov.tanggal), w));
      bytes += generator.text(
        _buildRow(
          'Kasir',
          auth.userFullName.isNotEmpty ? auth.userFullName : auth.userName,
          w,
        ),
      );
      bytes += generator.text(
        _buildRow('Lokasi', auth.selectedLocation ?? 'TOKO', w),
      );
      if (prov.selectedPelanggan != null) {
        bytes += generator.text(
          _buildRow(
            'Pelanggan',
            prov.selectedPelanggan!['NAMA']?.toString() ?? '',
            w,
          ),
        );
      }
      if (prov.selectedSales != null) {
        bytes += generator.text(
          _buildRow('Sales', prov.selectedSales!['Nama']?.toString() ?? '', w),
        );
      }
      bytes += generator.hr();

      // ── Header kolom (sesuai model nota) ─────────────────────
      final showDiskon =
          cfg.model == 1 || cfg.model == 2 || cfg.model == 5 || cfg.model == 6;
      if (cfg.model <= 4) {
        // Model 1-4: tampilkan header kolom
        bytes += generator.text(_buildHeaderKolom(w, showDiskon));
        bytes += generator.hr(ch: '-');
      }

      // ── Item ─────────────────────────────────────────────────
      for (final item in prov.cartItems) {
        bytes += generator.text(
          item.namaBarang,
          styles: const PosStyles(bold: false),
        );
        bytes += generator.text(
          _buildItemRow(
            qty: '${fmt.format(item.qty)} ${item.satuan}',
            harga: fmt.format(item.hargaJual),
            diskon: showDiskon && item.totalDiskon > 0
                ? fmt.format(item.totalDiskon)
                : '',
            total: fmt.format(item.totalHarga),
            w: w,
            showDiskon: showDiskon,
          ),
        );
      }
      bytes += generator.hr();

      // ── Total ────────────────────────────────────────────────
      bytes += generator.text(
        _buildRow('Subtotal', 'Rp ${fmt.format(prov.subtotal)}', w),
      );
      if (prov.diskonGlobalRp > 0) {
        bytes += generator.text(
          _buildRow('Diskon', '-Rp ${fmt.format(prov.diskonGlobalRp)}', w),
        );
      }
      if (prov.pajakNominal > 0) {
        bytes += generator.text(
          _buildRow('Pajak', '+Rp ${fmt.format(prov.pajakNominal)}', w),
        );
      }
      if (prov.biayaKirim > 0) {
        bytes += generator.text(
          _buildRow('Kirim', '+Rp ${fmt.format(prov.biayaKirim)}', w),
        );
      }
      bytes += generator.hr(ch: '=');
      bytes += generator.text(
        _buildRow('TOTAL', 'Rp ${fmt.format(prov.grandTotal)}', w),
        styles: const PosStyles(bold: true),
      );
      bytes += generator.hr(ch: '=');

      // ── Pembayaran ───────────────────────────────────────────
      if (prov.nominalTunai > 0) {
        bytes += generator.text(
          _buildRow('Tunai', 'Rp ${fmt.format(prov.nominalTunai)}', w),
        );
      }
      if (prov.nominalTransfer > 0) {
        bytes += generator.text(
          _buildRow('Transfer', 'Rp ${fmt.format(prov.nominalTransfer)}', w),
        );
      }
      if (prov.isLunas && prov.kembali > 0) {
        bytes += generator.text(
          _buildRow('Kembali', 'Rp ${fmt.format(prov.kembali)}', w),
        );
      }

      // ── Sisa hutang (sesuai model nota) ──────────────────────
      final showSisaHutang =
          cfg.model == 1 || cfg.model == 3 || cfg.model == 5 || cfg.model == 7;
      if (showSisaHutang && !prov.isLunas) {
        bytes += generator.hr(ch: '-');
        bytes += generator.text(
          _buildRow('Sisa Hutang', 'Rp ${fmt.format(prov.sisaTagihan)}', w),
          styles: const PosStyles(bold: true),
        );
      }

      // ── Info bank ────────────────────────────────────────────
      if (prov.nominalTransfer > 0 && prov.bank.isNotEmpty) {
        bytes += generator.hr(ch: '-');
        bytes += generator.text(_buildRow('Bank', prov.bank, w));
        if (prov.noRek.isNotEmpty) {
          bytes += generator.text(_buildRow('No.Rek', prov.noRek, w));
        }
        if (prov.namaRek.isNotEmpty) {
          bytes += generator.text(_buildRow('Nama', prov.namaRek, w));
        }
        if (prov.noRef.isNotEmpty) {
          bytes += generator.text(_buildRow('Ref', prov.noRef, w));
        }
      }

      // ── Footer ───────────────────────────────────────────────
      if (perusahaan != null) {
        bytes += generator.hr(ch: '=');
        if (perusahaan.footer1.isNotEmpty) {
          bytes += generator.text(
            perusahaan.footer1,
            styles: const PosStyles(align: PosAlign.center),
          );
        }
        if (perusahaan.footer2.isNotEmpty) {
          bytes += generator.text(
            perusahaan.footer2,
            styles: const PosStyles(align: PosAlign.center),
          );
        }
        if (perusahaan.footer3.isNotEmpty) {
          bytes += generator.text(
            perusahaan.footer3,
            styles: const PosStyles(align: PosAlign.center),
          );
        }
      }

      bytes += generator.feed(3);
      bytes += generator.cut();

      return Uint8List.fromList(bytes);
    } catch (_) {
      return null;
    }
  }

  // ── Helper: baris kiri-kanan ──────────────────────────────────
  static String _buildRow(String left, String right, int w) {
    final maxLeft = w - right.length - 1;
    final l = left.length > maxLeft ? left.substring(0, maxLeft) : left;
    final spaces = w - l.length - right.length;
    return l + (' ' * (spaces > 0 ? spaces : 1)) + right;
  }

  // ── Helper: header kolom item ─────────────────────────────────
  static String _buildHeaderKolom(int w, bool showDiskon) {
    // Layout persentase sama dengan VB:
    // Qty=11%, Harga=51%/65%, Disc=70%, Jml=95%
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

  // ── Helper: baris item ────────────────────────────────────────
  static String _buildItemRow({
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

  // ── Build teks nota untuk share WA ───────────────────────────
  static String buildNotaText({
    required PenjualanProvider prov,
    required AuthProvider auth,
  }) {
    final fmt = NumberFormat('#,##0', 'id_ID');
    final fmtTgl = DateFormat('dd/MM/yyyy HH:mm');
    final perusahaan = auth.perusahaan;
    final sb = StringBuffer();

    if (perusahaan != null) {
      sb.writeln('*${perusahaan.nama}*');
      if (perusahaan.alamat.isNotEmpty) sb.writeln(perusahaan.alamat);
    } else {
      sb.writeln('*KASIR LANCAR*');
    }
    sb.writeln('================================');
    sb.writeln('Tgl   : ${fmtTgl.format(prov.tanggal)}');
    sb.writeln(
      'Kasir : ${auth.userFullName.isNotEmpty ? auth.userFullName : auth.userName}',
    );
    if (prov.selectedPelanggan != null) {
      sb.writeln('Pelanggan: ${prov.selectedPelanggan!['NAMA']}');
    }
    sb.writeln('--------------------------------');
    for (final item in prov.cartItems) {
      sb.writeln(item.namaBarang);
      sb.writeln(
        '  ${fmt.format(item.qty)} ${item.satuan} x ${fmt.format(item.hargaJual)} = Rp ${fmt.format(item.totalHarga)}',
      );
      if (item.totalDiskon > 0) {
        sb.writeln('  Disc: -Rp ${fmt.format(item.totalDiskon)}');
      }
    }
    sb.writeln('================================');
    sb.writeln('Subtotal : Rp ${fmt.format(prov.subtotal)}');
    if (prov.diskonGlobalRp > 0) {
      sb.writeln('Diskon   : -Rp ${fmt.format(prov.diskonGlobalRp)}');
    }
    if (prov.pajakNominal > 0) {
      sb.writeln('Pajak    : +Rp ${fmt.format(prov.pajakNominal)}');
    }
    if (prov.biayaKirim > 0) {
      sb.writeln('Kirim    : +Rp ${fmt.format(prov.biayaKirim)}');
    }
    sb.writeln('*TOTAL   : Rp ${fmt.format(prov.grandTotal)}*');
    if (prov.nominalTunai > 0) {
      sb.writeln('Tunai    : Rp ${fmt.format(prov.nominalTunai)}');
    }
    if (prov.nominalTransfer > 0) {
      sb.writeln('Transfer : Rp ${fmt.format(prov.nominalTransfer)}');
    }
    if (prov.isLunas && prov.kembali > 0) {
      sb.writeln('Kembali  : Rp ${fmt.format(prov.kembali)}');
    }
    if (!prov.isLunas) {
      sb.writeln('*Hutang  : Rp ${fmt.format(prov.sisaTagihan)}*');
    }
    if (perusahaan != null && perusahaan.footer1.isNotEmpty) {
      sb.writeln('================================');
      sb.writeln(perusahaan.footer1);
    }
    return sb.toString();
  }
}
