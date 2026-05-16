import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

// ── Helper format ─────────────────────────────────────────────────────────────
final _fmtRp = NumberFormat('#,##0', 'id_ID');
final _fmtQty = NumberFormat('#,##0.##', 'id_ID');

String _rp(dynamic v) => 'Rp ${_fmtRp.format((v as num?)?.toDouble() ?? 0)}';
String _qty(dynamic v) => _fmtQty.format((v as num?)?.toDouble() ?? 0);

// ── Base modal ────────────────────────────────────────────────────────────────
void showAIModal(
  BuildContext context, {
  required String title,
  required IconData icon,
  required Color color,
  required Widget Function(BuildContext) builder,
}) {
  showModalBottomSheet(
    context: context,
    isScrollControlled: true,
    backgroundColor: Colors.transparent,
    builder: (ctx) => DraggableScrollableSheet(
      initialChildSize: 0.75,
      minChildSize: 0.4,
      maxChildSize: 0.95,
      builder: (_, scrollCtrl) => Container(
        decoration: const BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
        ),
        child: Column(
          children: [
            // Handle
            Container(
              margin: const EdgeInsets.only(top: 10),
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: Colors.grey.shade300,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            // Header
            Padding(
              padding: const EdgeInsets.fromLTRB(20, 14, 20, 0),
              child: Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      color: color.withValues(alpha: 0.12),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Icon(icon, color: color, size: 22),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Text(
                      title,
                      style: const TextStyle(
                        fontSize: 17,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close),
                    onPressed: () => Navigator.of(ctx).pop(),
                  ),
                ],
              ),
            ),
            const Divider(height: 20),
            // Content
            Expanded(
              child: SingleChildScrollView(
                controller: scrollCtrl,
                padding: const EdgeInsets.fromLTRB(16, 0, 16, 24),
                child: builder(ctx),
              ),
            ),
          ],
        ),
      ),
    ),
  );
}

// ── 1. Produk Terlaris ────────────────────────────────────────────────────────
void showProdukTerlarisModal(BuildContext context, List<dynamic> data) {
  showAIModal(
    context,
    title: 'Produk Terlaris',
    icon: Icons.local_fire_department,
    color: const Color(0xFFEF4444),
    builder: (_) {
      if (data.isEmpty) {
        return _emptyState('Belum ada data penjualan 7 hari terakhir');
      }
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _sectionLabel('Top 10 Barang — 7 Hari Terakhir'),
          const SizedBox(height: 8),
          ...data.asMap().entries.map((e) {
            final i = e.key;
            final item = e.value as Map<String, dynamic>;
            final trend = item['trend']?.toString() ?? '—';
            final isUp = trend.startsWith('+') || trend == 'Baru';
            return _rankRow(
              rank: i + 1,
              nama: item['NAMA_BARANG']?.toString() ?? '',
              line1: '${_qty(item['qty_7hari'])} qty',
              line2: _rp(item['omzet_7hari']),
              badge: trend,
              badgeColor: isUp ? Colors.green : Colors.red,
            );
          }),
        ],
      );
    },
  );
}

// ── 2. Barang Lambat ──────────────────────────────────────────────────────────
void showBarangLambatModal(BuildContext context, List<dynamic> data) {
  showAIModal(
    context,
    title: 'Barang Lambat',
    icon: Icons.hourglass_bottom,
    color: const Color(0xFF8B5CF6),
    builder: (_) {
      if (data.isEmpty) {
        return _emptyState('Semua barang terjual dalam 30 hari terakhir 👍');
      }
      final totalNilai = data.fold<double>(
        0,
        (s, e) => s + ((e['nilai_tertahan'] as num?)?.toDouble() ?? 0),
      );
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _infoBox(
            'Total nilai stok tertahan: ${_rp(totalNilai)}',
            const Color(0xFF8B5CF6),
          ),
          const SizedBox(height: 12),
          _sectionLabel('Barang tidak terjual > 30 hari'),
          const SizedBox(height: 8),
          ...data.map((item) {
            final hari = (item['hari_tidak_terjual'] as num?)?.toInt() ?? 0;
            return _listRow(
              nama: item['NAMA_BARANG']?.toString() ?? '',
              line1:
                  'Stok: ${_qty(item['stok'])} · Nilai: ${_rp(item['nilai_tertahan'])}',
              line2: hari >= 999
                  ? 'Belum pernah terjual'
                  : '$hari hari tidak terjual',
              line2Color: hari > 60
                  ? Colors.red.shade600
                  : Colors.orange.shade700,
            );
          }),
          const SizedBox(height: 16),
          _tipBox(
            '💡 Pertimbangkan diskon atau retur ke supplier untuk barang dengan nilai tertahan tinggi.',
          ),
        ],
      );
    },
  );
}

// ── 3. Reorder Alert ──────────────────────────────────────────────────────────
void showReorderAlertModal(BuildContext context, List<dynamic> data) {
  showAIModal(
    context,
    title: 'Reorder Alert',
    icon: Icons.warning_amber_rounded,
    color: const Color(0xFFF59E0B),
    builder: (_) {
      if (data.isEmpty) {
        return _emptyState('Stok semua barang aman untuk 7 hari ke depan 👍');
      }
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _infoBox(
            '${data.length} barang perlu segera dipesan',
            const Color(0xFFF59E0B),
          ),
          const SizedBox(height: 12),
          _sectionLabel('Estimasi berdasarkan rata penjualan 7 hari'),
          const SizedBox(height: 8),
          ...data.map((item) {
            final hari = (item['estimasi_hari_habis'] as num?)?.toInt() ?? 0;
            final saran = (item['saran_order_qty'] as num?)?.toInt() ?? 0;
            final isKritis = hari <= 2;
            return _listRow(
              nama: item['NAMA_BARANG']?.toString() ?? '',
              line1:
                  'Stok: ${_qty(item['stok_saat_ini'])} · Rata: ${_fmtQty.format((item['rata_per_hari'] as num?)?.toDouble() ?? 0)}/hari',
              line2: hari <= 0
                  ? '⚠️ Sudah habis!'
                  : 'Habis ~$hari hari · Saran order: $saran',
              line2Color: isKritis
                  ? Colors.red.shade700
                  : Colors.orange.shade700,
            );
          }),
          const SizedBox(height: 16),
          _tipBox(
            '💡 Order minimal 14 hari supply untuk menghindari kehabisan stok.',
          ),
        ],
      );
    },
  );
}

// ── 4. Jam Puncak ─────────────────────────────────────────────────────────────
void showJamPuncakModal(
  BuildContext context,
  List<dynamic> data,
  String rekomendasi,
) {
  showAIModal(
    context,
    title: 'Jam Puncak Transaksi',
    icon: Icons.access_time,
    color: const Color(0xFF2563EB),
    builder: (_) {
      if (data.isEmpty) {
        return _emptyState('Belum ada data transaksi 7 hari terakhir');
      }

      final maxTrx = data.fold<int>(
        0,
        (m, e) => ((e['jumlah_transaksi'] as num?)?.toInt() ?? 0) > m
            ? (e['jumlah_transaksi'] as num).toInt()
            : m,
      );

      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _infoBox(rekomendasi, const Color(0xFF2563EB)),
          const SizedBox(height: 16),
          _sectionLabel('Distribusi Transaksi per Jam (7 hari terakhir)'),
          const SizedBox(height: 12),
          // Bar chart sederhana
          ...data.map((item) {
            final jam = (item['jam'] as num?)?.toInt() ?? 0;
            final trx = (item['jumlah_transaksi'] as num?)?.toInt() ?? 0;
            final pct = maxTrx > 0 ? trx / maxTrx : 0.0;
            final isPuncak = trx == maxTrx && trx > 0;

            return Padding(
              padding: const EdgeInsets.only(bottom: 6),
              child: Row(
                children: [
                  SizedBox(
                    width: 44,
                    child: Text(
                      '${jam.toString().padLeft(2, '0')}:00',
                      style: TextStyle(
                        fontSize: 11,
                        fontWeight: isPuncak
                            ? FontWeight.bold
                            : FontWeight.normal,
                        color: isPuncak
                            ? const Color(0xFF2563EB)
                            : Colors.grey.shade600,
                      ),
                    ),
                  ),
                  Expanded(
                    child: Stack(
                      children: [
                        Container(
                          height: 20,
                          decoration: BoxDecoration(
                            color: Colors.grey.shade100,
                            borderRadius: BorderRadius.circular(4),
                          ),
                        ),
                        FractionallySizedBox(
                          widthFactor: pct.clamp(0.0, 1.0),
                          child: Container(
                            height: 20,
                            decoration: BoxDecoration(
                              color: isPuncak
                                  ? const Color(0xFF2563EB)
                                  : const Color(
                                      0xFF2563EB,
                                    ).withValues(alpha: 0.4),
                              borderRadius: BorderRadius.circular(4),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(width: 8),
                  SizedBox(
                    width: 28,
                    child: Text(
                      '$trx',
                      style: TextStyle(
                        fontSize: 11,
                        fontWeight: isPuncak
                            ? FontWeight.bold
                            : FontWeight.normal,
                        color: isPuncak
                            ? const Color(0xFF2563EB)
                            : Colors.grey.shade600,
                      ),
                      textAlign: TextAlign.right,
                    ),
                  ),
                ],
              ),
            );
          }),
        ],
      );
    },
  );
}

// ── 5. Margin Profit ──────────────────────────────────────────────────────────
void showMarginProfitModal(BuildContext context, Map<String, dynamic> data) {
  final top = List<dynamic>.from(data['top'] ?? []);
  final bottom = List<dynamic>.from(data['bottom'] ?? []);

  showAIModal(
    context,
    title: 'Analisis Margin Profit',
    icon: Icons.trending_up,
    color: const Color(0xFF10B981),
    builder: (_) {
      if (top.isEmpty && bottom.isEmpty) {
        return _emptyState(
          'Data penjualan 30 hari belum cukup untuk analisis margin',
        );
      }
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _sectionLabel('🏆 Top 5 Margin Tertinggi'),
          const SizedBox(height: 8),
          ...top.asMap().entries.map((e) {
            final item = e.value as Map<String, dynamic>;
            final margin = (item['margin_persen'] as num?)?.toDouble() ?? 0;
            return _marginRow(item, margin, Colors.green.shade600);
          }),
          const SizedBox(height: 16),
          _sectionLabel('⚠️ 5 Margin Terendah'),
          const SizedBox(height: 8),
          ...bottom.asMap().entries.map((e) {
            final item = e.value as Map<String, dynamic>;
            final margin = (item['margin_persen'] as num?)?.toDouble() ?? 0;
            return _marginRow(item, margin, Colors.red.shade600);
          }),
          const SizedBox(height: 16),
          _tipBox(
            '💡 Fokus promosi pada barang margin tinggi. Evaluasi harga barang margin rendah.',
          ),
        ],
      );
    },
  );
}

Widget _marginRow(Map<String, dynamic> item, double margin, Color color) {
  return Container(
    margin: const EdgeInsets.only(bottom: 8),
    padding: const EdgeInsets.all(10),
    decoration: BoxDecoration(
      color: color.withValues(alpha: 0.05),
      borderRadius: BorderRadius.circular(10),
      border: Border.all(color: color.withValues(alpha: 0.2)),
    ),
    child: Row(
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                item['NAMA_BARANG']?.toString() ?? '',
                style: const TextStyle(
                  fontSize: 13,
                  fontWeight: FontWeight.w600,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              Text(
                'Beli: ${_rp(item['avg_harga_beli'])} · Jual: ${_rp(item['avg_harga_jual'])}',
                style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
              ),
            ],
          ),
        ),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
          decoration: BoxDecoration(
            color: color.withValues(alpha: 0.12),
            borderRadius: BorderRadius.circular(8),
          ),
          child: Text(
            '${margin.toStringAsFixed(1)}%',
            style: TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.bold,
              color: color,
            ),
          ),
        ),
      ],
    ),
  );
}

// ── 6. Pelanggan Aktif ────────────────────────────────────────────────────────
void showPelangganAktifModal(
  BuildContext context,
  List<dynamic> data,
  Map<String, dynamic> summary,
) {
  showAIModal(
    context,
    title: 'Pelanggan Aktif',
    icon: Icons.people_outline,
    color: const Color(0xFF0EA5E9),
    builder: (_) {
      if (data.isEmpty) {
        return _emptyState('Belum ada transaksi dengan pelanggan terdaftar');
      }
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: _statCard(
                  '${summary['total_aktif'] ?? 0}',
                  'Pelanggan Aktif\n(90 hari)',
                  const Color(0xFF0EA5E9),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: _statCard(
                  '${summary['pelanggan_baru'] ?? 0}',
                  'Pelanggan Baru\nBulan Ini',
                  const Color(0xFF10B981),
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          _sectionLabel('Top 10 Pelanggan (90 hari terakhir)'),
          const SizedBox(height: 8),
          ...data.asMap().entries.map((e) {
            final i = e.key;
            final item = e.value as Map<String, dynamic>;
            final badge = item['badge']?.toString() ?? 'Reguler';
            final badgeColor = badge == 'VIP'
                ? Colors.amber.shade700
                : badge == 'Baru'
                ? Colors.green.shade600
                : Colors.blue.shade600;

            return _rankRow(
              rank: i + 1,
              nama: item['NAMA_PELANGGAN']?.toString() ?? '',
              line1: _rp(item['total_belanja']),
              line2:
                  '${item['frekuensi']} transaksi · ${item['hari_sejak_beli']} hari lalu',
              badge: badge,
              badgeColor: badgeColor,
            );
          }),
        ],
      );
    },
  );
}

// ── Shared widgets ────────────────────────────────────────────────────────────

Widget _emptyState(String msg) {
  return Padding(
    padding: const EdgeInsets.symmetric(vertical: 40),
    child: Center(
      child: Column(
        children: [
          Icon(Icons.info_outline, size: 48, color: Colors.grey.shade300),
          const SizedBox(height: 12),
          Text(
            msg,
            textAlign: TextAlign.center,
            style: TextStyle(color: Colors.grey.shade500, fontSize: 14),
          ),
        ],
      ),
    ),
  );
}

Widget _sectionLabel(String text) {
  return Text(
    text,
    style: const TextStyle(
      fontSize: 13,
      fontWeight: FontWeight.w700,
      color: Color(0xFF374151),
    ),
  );
}

Widget _infoBox(String text, Color color) {
  return Container(
    width: double.infinity,
    padding: const EdgeInsets.all(12),
    decoration: BoxDecoration(
      color: color.withValues(alpha: 0.08),
      borderRadius: BorderRadius.circular(10),
      border: Border.all(color: color.withValues(alpha: 0.25)),
    ),
    child: Text(
      text,
      style: TextStyle(fontSize: 13, color: color, fontWeight: FontWeight.w600),
    ),
  );
}

Widget _tipBox(String text) {
  return Container(
    width: double.infinity,
    padding: const EdgeInsets.all(12),
    decoration: BoxDecoration(
      color: Colors.amber.shade50,
      borderRadius: BorderRadius.circular(10),
      border: Border.all(color: Colors.amber.shade200),
    ),
    child: Text(
      text,
      style: TextStyle(fontSize: 12, color: Colors.amber.shade800),
    ),
  );
}

Widget _rankRow({
  required int rank,
  required String nama,
  required String line1,
  String? line2,
  required String badge,
  required Color badgeColor,
}) {
  return Container(
    margin: const EdgeInsets.only(bottom: 8),
    padding: const EdgeInsets.all(10),
    decoration: BoxDecoration(
      color: Colors.grey.shade50,
      borderRadius: BorderRadius.circular(10),
      border: Border.all(color: Colors.grey.shade200),
    ),
    child: Row(
      children: [
        // Rank number
        Container(
          width: 28,
          height: 28,
          decoration: BoxDecoration(
            color: rank <= 3
                ? [
                    Colors.amber,
                    Colors.grey.shade400,
                    Colors.brown.shade300,
                  ][rank - 1]
                : Colors.grey.shade200,
            shape: BoxShape.circle,
          ),
          child: Center(
            child: Text(
              '$rank',
              style: TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.bold,
                color: rank <= 3 ? Colors.white : Colors.grey.shade600,
              ),
            ),
          ),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                nama,
                style: const TextStyle(
                  fontSize: 13,
                  fontWeight: FontWeight.w600,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              Text(
                line1,
                style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
              ),
              if (line2 != null)
                Text(
                  line2,
                  style: TextStyle(fontSize: 10, color: Colors.grey.shade500),
                ),
            ],
          ),
        ),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
          decoration: BoxDecoration(
            color: badgeColor.withValues(alpha: 0.12),
            borderRadius: BorderRadius.circular(6),
          ),
          child: Text(
            badge,
            style: TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w600,
              color: badgeColor,
            ),
          ),
        ),
      ],
    ),
  );
}

Widget _listRow({
  required String nama,
  required String line1,
  String? line2,
  Color? line2Color,
}) {
  return Container(
    margin: const EdgeInsets.only(bottom: 8),
    padding: const EdgeInsets.all(10),
    decoration: BoxDecoration(
      color: Colors.grey.shade50,
      borderRadius: BorderRadius.circular(10),
      border: Border.all(color: Colors.grey.shade200),
    ),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          nama,
          style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
        ),
        const SizedBox(height: 2),
        Text(
          line1,
          style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
        ),
        if (line2 != null)
          Text(
            line2,
            style: TextStyle(
              fontSize: 11,
              color: line2Color ?? Colors.grey.shade500,
              fontWeight: FontWeight.w500,
            ),
          ),
      ],
    ),
  );
}

Widget _statCard(String value, String label, Color color) {
  return Container(
    padding: const EdgeInsets.all(12),
    decoration: BoxDecoration(
      color: color.withValues(alpha: 0.08),
      borderRadius: BorderRadius.circular(12),
      border: Border.all(color: color.withValues(alpha: 0.2)),
    ),
    child: Column(
      children: [
        Text(
          value,
          style: TextStyle(
            fontSize: 24,
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
        Text(
          label,
          textAlign: TextAlign.center,
          style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
        ),
      ],
    ),
  );
}
